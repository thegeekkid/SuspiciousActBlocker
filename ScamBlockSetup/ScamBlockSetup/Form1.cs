using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using Microsoft.Win32;
using System.Diagnostics;

namespace ScamBlockSetup
{
    public partial class Form1 : Form
    {
        // Variable that is used for cross-thread data updates and keeps the GUI up-to-date with the current status during the actual install.
        public string stat;
        // Placeholder for how far we are.
        public int progress = 0;
        // Total number of progress increments that the program has to traverse.
        public static int target = 5;
        // Placeholder for loops running on other threads to know when to exit.
        public bool install_Finished = false;

        // Instance of settings that we can use to write to or read from an XML file.
        settings set = new settings();


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // Set the default install directory.
                textBox6.Text = @"C:\Program Files\Semrau Software Consulting\Scam Block\";


                // If a silent.xml file exists, load the settings into the placeholders and perform the install.
                if (File.Exists(Environment.CurrentDirectory + @"\silent.xml"))
                {
                    // Read into our settings instance.
                    set = LoadXML(Environment.CurrentDirectory + @"\silent.xml");

                    // We are silent, so hide the GUI.
                    this.Visible = false;

                    // For now a silent install will always be MSP, but leaving the condition here for any possible future use.
                    if (set.install_type == "msp")
                    {
                        radioButton1.Checked = false;
                        radioButton2.Checked = true;
                    }

                    // Load the settings into their GUI elements so we don't have to re-write a ton of code when we do the install.
                    textBox6.Text = set.install_dir;
                    textBox2.Text = set.company;
                    textBox3.Text = set.website;
                    richTextBox2.Text = set.contactInfo;
                    textBox1.Text = set.supportUrl;


                    // If there is a passhash specified in the XML, set a placeholder so the install process knows to just copy that hash/salt and not re-hash the existing hash.
                    if (set.passHash != "")
                    {
                        textBox4.Text = "|||***|||";
                    }


                    checkBox1.Checked = bool.Parse(set.lockout_enabled);

                    // Double check that the logo exists in this directory before pulling in this setting.  We will just leave it blank (no logo used in notification window) if the file doesn't exist.
                    if ((set.logo != "") && (File.Exists(Environment.CurrentDirectory + @"\" + set.logo)))
                    {
                        textBox5.Text = Environment.CurrentDirectory + @"\" + set.logo;
                    }

                    // Continue reading settings into their GUI elements.
                    checkBox2.Checked = set.syskey;
                    checkBox3.Checked = set.eventView;
                    checkBox4.Checked = set.MMC;
                    checkBox5.Checked = set.regedit;
                    checkBox6.Checked = set.msconfig;
                    checkBox8.Checked = set.cmd;
                    checkBox9.Checked = set.prm;
                    checkBox10.Checked = set.notepad;


                    // Trigger the install since we aren't waiting for user input
                    do_install();

                    // Install finished - close the application.  Some systems work better with these different commands, so putting them in just to be sure we actually close.
                    Environment.Exit(0);
                    this.Close();
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                // Oops... let's let them know what the error was.
                MessageBox.Show(ex.ToString());
            }

        }




        private void trigger_install()
        {
            // Use this function when the GUI needs to be maintained; otherwise, just go directly to do_install.
            try
            {
                // Status updater just keeps the GUI up to date.
                Thread watcher = new Thread(status_updater);

                // Do Install actually does the install (go figure!)
                Thread worker = new Thread(do_install);

                // Boom goes the dynamite.
                worker.Start();
                watcher.Start();
            }
            catch (Exception ex)
            {
                // Error 404: Brain not found.
                MessageBox.Show(ex.ToString());
            }

        }

        private void do_install()
        {
            try
            {
                // Update variable, the watcher thread will just pull this into the GUI on a timer.
                stat = "Creating folders";

                // We need to start with a clean dir - just in case.
                if (Directory.Exists(textBox6.Text))
                {
                    foreach (string file in Directory.GetFiles(textBox6.Text))
                    {
                        File.Delete(file);
                    }
                    Directory.Delete(textBox6.Text, true);
                }
                Directory.CreateDirectory(textBox6.Text);

                // Load these into settings in case this is the GUI - if it's a silent install, it will not do any harm.
                set.install_dir = textBox6.Text;


                // Progress 1
                progress++;

                // Again update the status
                stat = "Downloading components";

                // Download the main file and the executor file.  The main file technically could be deleted after install, but we leave it in the program dir for any future use.
                // Executor file absolutely needs to be present if we ever want to be able to run anything.
                WebClient wc = new WebClient();
                wc.DownloadFile(@"https://downloads.semrauconsulting.com/ScamBlock/SuspiciousActBlocker.exe", textBox6.Text + @"SuspiciousActBlocker.exe");
                wc.DownloadFile(@"https://downloads.semrauconsulting.com/ScamBlock/executor.exe", textBox6.Text + @"executor.exe");


                // Progress 2
                progress++;

                // Just start with a blank name - if that gets written to registry, the program will just know there isn't a logo.
                string logo_name = "";

                if (textBox5.Text != "")
                {
                    // Get the name of the file instead of the full path, but use the full path to copy to the install dir.
                    FileInfo dirinfo = new FileInfo(textBox5.Text);
                    logo_name = dirinfo.Name;
                    stat = "Copying files";
                    File.Copy(textBox5.Text, textBox6.Text + logo_name);
                }
                // Again, used for if we are in GUI mode and might need to export an XML.
                set.logo = logo_name;


                //Progress 3
                progress++;


                // Basically just setting registry settings at this point.
                stat = "Saving settings";
                try
                {
                    // Setup our subkeys in the registry - show a message if there's an issue... possibly permissions related?
                    Registry.LocalMachine.OpenSubKey("SOFTWARE", true).CreateSubKey("Semrau Software Consulting");
                }
                catch (Exception ex2)
                {
                    MessageBox.Show(ex2.ToString());
                }
                try
                { 
                    Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("Semrau Software Consulting", true).CreateSubKey("SuspiciousActBlocker");
                }
                catch (Exception ex2)
                {
                    MessageBox.Show(ex2.ToString());
                }

                // Just load all of the settings into the registry at this point.
                set_setting("install_location", textBox6.Text);
                set_setting("company", textBox2.Text);
                set.company = textBox2.Text;
                set_setting("website", textBox3.Text);
                set.website = textBox3.Text;
                set_setting("contactInfo", richTextBox2.Text.Replace(Environment.NewLine, @"\n"));
                set.contactInfo = richTextBox2.Text.Replace(Environment.NewLine, @"\n");
                set_setting("remote_site", textBox1.Text);
                set.supportUrl = textBox1.Text;

                if (textBox4.Text != "")
                {
                    if (textBox4.Text == "|||***|||")
                    {
                        // This means that we should just copy the hash and salt from the xml.

                        set_setting("passHash", set.passHash);
                        set_setting("s", set.s);

                    }
                    else
                    {
                        // In this case we actually need to generate a salt and a hash.
                        string salt = get_salt();
                        set_setting("passHash", sha256(salt + textBox4.Text));
                        set_setting("s", salt);
                        set.s = salt;
                        set.passHash = sha256(salt + textBox4.Text);
                    }


                }
                else
                {
                    // No password, so just put blank values in.
                    set_setting("passHash", "");
                    set_setting("s", "");
                    set.s = "";
                }


                set_setting("lockdown_enabled", checkBox1.Checked.ToString());
                set.lockout_enabled = checkBox1.Checked.ToString();
                set_setting("logo", logo_name);

                if (radioButton2.Checked)
                {
                    set_setting("install_type", "msp");
                    set.install_type = "msp";
                }
                else
                {
                    set_setting("install_type", "consumer");
                    set.install_type = "consumer";
                }

                // Progress 4
                progress++;


                // Seperate function to actually work with the system directories.
                stat = "Protecting system";
                do_protection();

                // Once the system directories have been "protected", the install is finished.

                // Progress 5
                progress++;
                stat = "Finished";

                install_Finished = true;
            }
            catch (Exception ex)
            {
                // No comment.
                MessageBox.Show(ex.ToString());
            }

        }

        private void do_protection()
        {
            try
            {
                // For each of the possible "protected" objects, call the protect_file function to find a random name, rename, and copy our exe over the old one (assuming they were chosen to be protected).
                if (checkBox2.Checked)
                {
                    protect_file(@"C:\Windows\System32\syskey.exe");
                    set.syskey = true;
                }
                else
                {
                    set_setting(@"C:\Windows\System32\syskey.exe", "false");
                    set.syskey = false;
                }
                if (checkBox3.Checked)
                {
                    protect_file(@"C:\Windows\System32\eventvwr.exe");
                    set.eventView = true;
                }
                else
                {
                    set_setting(@"C:\Windows\System32\eventvwr.exe", "false");
                    set.eventView = false;
                }
                if (checkBox4.Checked)
                {
                    protect_file(@"C:\Windows\System32\mmc.exe");
                    set.MMC = true;
                }
                else
                {
                    set_setting(@"C:\Windows\System32\mmc.exe", "false");
                    set.MMC = false;
                }
                if (checkBox5.Checked)
                {
                    protect_file(@"C:\Windows\regedit.exe");
                    set.regedit = true;
                }
                else
                {
                    set_setting(@"C:\Windows\regedit.exe", "false");
                    set.regedit = false;
                }
                if (checkBox6.Checked)
                {
                    protect_file(@"C:\Windows\System32\msconfig.exe");
                    set.msconfig = true;
                }
                else
                {
                    set_setting(@"C:\Windows\System32\msconfig.exe", "false");
                    set.msconfig = false;
                }
                if (checkBox8.Checked)
                {
                    protect_file(@"C:\Windows\System32\cmd.exe");
                    set.cmd = true;
                }
                else
                {
                    set_setting(@"C:\Windows\System32\cmd.exe", "false");
                    set.cmd = false;
                }
                if (checkBox9.Checked)
                {
                    protect_file(@"C:\Windows\System32\perfmon.exe");
                    set.prm = true;
                }
                else
                {
                    set_setting(@"C:\Windows\System32\perfmon.exe", "false");
                    set.prm = false;
                }
                if (checkBox10.Checked)
                {
                    protect_file(@"C:\Windows\System32\notepad.exe");
                    set.notepad = true;
                }
                else
                {
                    set_setting(@"C:\Windows\System32\notepad.exe", "false");
                    set.notepad = false;
                }
            }
            catch (Exception ex)
            {
                // Sometimes I wonder if anyone ever actually reads these things.  I'm sure I'll come back a few years from now and have myself in stitches (when I'm not crying at the quality of the code).
                // Ya................. I'm weird that way.
                MessageBox.Show(ex.ToString());
            }

        }
        private void protect_file(string file)
        {
            try
            {
                // Reminded myself the hard way - you have to take permissions of the system files first.  Derp.
                // Start by taking ownership
                Process proc = new Process();
                proc.StartInfo.FileName = @"C:\Windows\System32\takeown.exe";
                proc.StartInfo.Arguments = @"/f " + file;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.Start();
                proc.WaitForExit();
                string err = proc.StandardError.ReadToEnd();
                if (err != "")
                {
                    MessageBox.Show(err);
                }

                // Then we move to actually giving ourselves permissions.
                proc = new Process();
                proc.StartInfo.FileName = @"C:\Windows\System32\icacls.exe";
                proc.StartInfo.Arguments = file + @" /grant """ + Environment.UserName + @":F""";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.Start();
                proc.WaitForExit();
                err = proc.StandardError.ReadToEnd();
                if (err != "")
                {
                    MessageBox.Show(err);
                }

                // Get a random name that doesn't already exist.
                string name = get_name();
                // I don't know why I used directoryinfo instead of fileinfo - but it works.  Just leave it.  Lol!
                DirectoryInfo dirinfo = new DirectoryInfo(file);
                // File the actual executable to a .log file
                File.Copy(file, file.Replace(dirinfo.Name, name + ".log"));
                // Keep track of what we called it so we don't loose it...
                set_setting(file, file.Replace(dirinfo.Name, name + ".log"));
            
                // Bye bye.
                File.Delete(file);

                // Oh ya... that's the stuff.  (Copy our exe from the program dir in place of the orig file.
                File.Copy(textBox6.Text + "SuspiciousActBlocker.exe", file);
            }
            catch (Exception ex)
            {
                // somedev1 - 7/9/2017: Adding temporary catch - should come back and make this more robust.
                // somedev2 - 7/9/2019: Temporary my a**!
                MessageBox.Show(ex.ToString());
            }

        }
        
        private void set_setting(string name, string value)
        {
            try
            {
                // I'm lazy.  I don't want to write this same subkey progression 24 times.
                Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("Semrau Software Consulting").OpenSubKey("SuspiciousActBlocker", true).SetValue(name, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }




        private void status_updater()
        {
            // Just loop until the install is finished and update the GUI.
            try
            {
                do
                {
                    // We are probably always going to be cross-thread, so invoke the update.
                    if (statustxt.InvokeRequired)
                    {
                        statustxt.BeginInvoke((MethodInvoker)delegate ()
                        {
                            statustxt.Text = stat;
                        });
                    }
                    else
                    {
                        // Just incase we aren't cross-thread.
                        statustxt.Text = stat;
                    }

                    // Get the percentage that we are done - even though it normally shoots from next to nothing to finished almost instantly....
                    int pvalue = ((progress / target) * 100);
                    if (pvalue > 100)
                    {
                        pvalue = 100;
                    }

                    // Again, probably cross thread.
                    if (progressBar1.InvokeRequired)
                    {
                        progressBar1.BeginInvoke((MethodInvoker)delegate ()
                        {
                            progressBar1.Value = pvalue;
                        });
                    }
                    else
                    {
                        progressBar1.Value = pvalue;
                    }

                    Thread.Sleep(500);
                } while (install_Finished == false);


                // Update it to 100 for appearances sake - even though it should be updated by the percentage calculation already.
                if (progressBar1.InvokeRequired)
                {
                    progressBar1.BeginInvoke((MethodInvoker)delegate ()
                    {
                        progressBar1.Value = 100;
                    });
                }
                else
                {
                    progressBar1.Value = 100;
                }


                // Just setup the GUI to tell the user that we are done.
                if (label14.InvokeRequired)
                {
                    label14.BeginInvoke((MethodInvoker)delegate ()
                    {
                        label14.Visible = true;
                    });
                }
                else
                {
                    label14.Visible = true;
                }

                if (radioButton2.Checked)
                {
                    // if it's a branded setup, give them the option to export the XML.
                    if (xml_export.InvokeRequired)
                    {
                        xml_export.BeginInvoke((MethodInvoker)delegate ()
                        {
                            xml_export.Visible = true;
                        });
                    }
                    else
                    {
                        xml_export.Visible = true;
                    }
                }

                if (button15.InvokeRequired)
                {
                    button15.BeginInvoke((MethodInvoker)delegate ()
                    {
                        button15.Visible = true;
                    });
                }
                else
                {
                    button15.Visible = true;
                }

            }
            catch (Exception ex)
            {
                // Magic.  Do not touch.
                MessageBox.Show(ex.ToString());
            }

        }






        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Advance to the next screen and save that they accepted the license.
                set.acceptLicense = true;
                welcome.Visible = false;
                install_type.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Don't want to accept my license?  Bye Felicia!
            try
            {
                Environment.Exit(0);
                this.Close();
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Backup one screen (panel).
            try
            {
                install_type.Visible = false;
                welcome.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Jump to protected items if this is a consumer installation.
            try
            {
                install_type.Visible = false;
                if (radioButton2.Checked)
                {
                    branding.Visible = true;
                }
                else
                {
                    protected_items.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Go back.
            branding.Visible = false;
            install_type.Visible = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // And here we go (forward)!
            try
            {
                branding.Visible = false;
                protected_items.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Look for their logo and load it into textBox5.text.
            try
            {
                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    textBox5.Text = openFileDialog1.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            // Move back again.
            try
            {
                install_dir.Visible = false;
                protected_items.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            // And here we go back again.
            try
            {
                protected_items.Visible = false;
                if (radioButton2.Checked)
                {
                    branding.Visible = true;
                }
                else
                {
                    install_type.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void button10_Click(object sender, EventArgs e)
        {
            // And here we can see the elusive end user going forward...
            try
            {
                protected_items.Visible = false;
                install_dir.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if the dir exists, prompt, and do install if it doesn't or the user doesn't care.
                if ((Directory.Exists(textBox6.Text)) && (MessageBox.Show("Warning!  The folder exists and all data will be erased.  Are you sure that you want to continue?", "Warning", MessageBoxButtons.YesNo) == DialogResult.No))
                {
                    // Directory exists and user doesn't want to continue
                    textBox6.Text = "";
                }
                else
                {
                    // Directory doesn't exist, or the user doesn't care.
                    if (!(textBox6.Text.EndsWith(@"\")))
                    {
                        textBox6.Text += @"\";
                    }
                    // Just moving forward on the panels.
                    install_dir.Visible = false;
                    status.Visible = true;
                    trigger_install();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void button12_Click(object sender, EventArgs e)
        {
            // Select the install dir
            try
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    textBox6.Text = folderBrowserDialog1.SelectedPath + @"\";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void button13_Click(object sender, EventArgs e)
        {
            // Export the XML file
            using (var writer = new StreamWriter(Environment.CurrentDirectory + @"\silent.xml"))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(set.GetType());
                serializer.Serialize(writer, set);
                writer.Flush();
            }

            MessageBox.Show(@"The xml has been placed in the same directory as this setup executable.  To run the installer silently, keep the xml named ""silent.xml"" and make sure it is in the same directory as the setup executable when you run it.  If you specified a logo, that must also be in the same directory as the setup executable.");

        }

        // Button 14 no longer exists

        private void button15_Click(object sender, EventArgs e)
        {
            // Stop.  Hammertime!
            Environment.Exit(0);
            this.Close();
            Application.Exit();
        }




        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            // Toggle function
            try
            {
                if (radioButton1.Checked)
                {
                    radioButton2.Checked = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            // Toggle function part 2
            try
            {
                if (radioButton2.Checked)
                {
                    radioButton1.Checked = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        

        private void textBox5_TextChanged_1(object sender, EventArgs e)
        {
            // Preview the logo if possible.
            try
            {
                if (File.Exists(textBox5.Text))
                {
                    pictureBox1.Image = Image.FromFile(textBox5.Text);
                    pictureBox1.Visible = true;
                }
                else
                {
                    pictureBox1.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }       

        
        // Just a function to generate a random salt.
        private string get_salt()
        {
            try
            {
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] buffer = new byte[1024];
                rng.GetBytes(buffer);
                return BitConverter.ToString(buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return "";
            }
            
        }

        // Function to get a unique random name
        private string get_name()
        {
            try
            {
                string name = sha256(get_salt());
                do
                {
                    name = sha256(get_salt());
                } while ((File.Exists(@"C:\Windows\" + name + ".log") || (File.Exists(@"C:\Windows\System32\" + name + ".log"))));
                return name;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return "";
            }
            
        }


        // Hash that string.
        static string sha256(string input)
        {
            try
            {
                System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
                System.Text.StringBuilder hash = new System.Text.StringBuilder();
                byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(input), 0, Encoding.UTF8.GetByteCount(input));
                foreach (byte theByte in crypto)
                {
                    hash.Append(theByte.ToString("x2"));
                }
                return hash.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return "";
            }
            
        }

        // Method to load the XML into a settings class.
        public static settings LoadXML(string FileName)
        {
            using (var stream = System.IO.File.OpenRead(FileName))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(settings));
                return serializer.Deserialize(stream) as settings;
            }
        }
    }
}
public class settings
{
    // Just a placeholder object for settings going to or from an XML.
    public bool acceptLicense { get; set; }
    public string install_type { get; set; }

    public string company { get; set; }
    public string website { get; set; }
    public string contactInfo { get; set; }
    public string supportUrl { get; set; }
    public string passHash { get; set; }

    public string s { get; set; }
    public string lockout_enabled { get; set; }
    public string logo { get; set; }

    public bool syskey { get; set; }
    public bool eventView { get; set; }
    public bool MMC { get; set; }
    public bool regedit { get; set; }
    public bool msconfig { get; set; }
    public bool cmd { get; set; }
    public bool prm { get; set; }
    public bool notepad { get; set; }

    public string install_dir { get; set; }
}