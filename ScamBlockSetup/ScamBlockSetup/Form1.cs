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
        public string stat;
        public int progress = 0;
        public static int target = 5;
        public bool install_Finished = false;

        settings set = new settings();


        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                set.acceptLicense = true;
                welcome.Visible = false;
                install_type.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
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

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
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

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
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

        private void button4_Click(object sender, EventArgs e)
        {
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




        private void textBox5_TextChanged_1(object sender, EventArgs e)
        {
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

        private void button7_Click(object sender, EventArgs e)
        {
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


        private void button6_Click(object sender, EventArgs e)
        {
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

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                textBox6.Text = @"C:\Program Files\Semrau Software Consulting\Scam Block\";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void button12_Click(object sender, EventArgs e)
        {
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

        private void trigger_install()
        {
            try
            {
                Thread watcher = new Thread(status_updater);
                Thread worker = new Thread(do_install);

                worker.Start();
                watcher.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void status_updater()
        {
            try
            {
                do
                {
                    if (statustxt.InvokeRequired)
                    {
                        statustxt.BeginInvoke((MethodInvoker)delegate ()
                        {
                            statustxt.Text = stat;
                        });
                    }
                    else
                    {
                        statustxt.Text = stat;
                    }

                    int pvalue = ((progress / target) * 100);
                    if (pvalue > 100)
                    {
                        pvalue = 100;
                    }

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
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void do_install()
        {
            try
            {
                stat = "Creating folders";
                if (Directory.Exists(textBox6.Text))
                {
                    foreach (string file in Directory.GetFiles(textBox6.Text))
                    {
                        File.Delete(file);
                    }
                    Directory.Delete(textBox6.Text, true);
                }
                Directory.CreateDirectory(textBox6.Text);
                set.install_dir = textBox6.Text;


                // Progress 1
                progress++;

                stat = "Downloading components";
                WebClient wc = new WebClient();
                wc.DownloadFile(@"https://downloads.semrauconsulting.com/ScamBlock/SuspiciousActBlocker.exe", textBox6.Text + @"SuspiciousActBlocker.exe");
                // Progress 2
                progress++;

                string logo_name = "";

                if (textBox5.Text != "")
                {
                    FileInfo dirinfo = new FileInfo(textBox5.Text);
                    logo_name = dirinfo.Name;
                    stat = "Copying files";
                    File.Copy(textBox5.Text, textBox6.Text + logo_name);
                }
                set.logo = logo_name;
                

                //Progress 3
                progress++;

                stat = "Saving settings";
                try
                {
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

                    string salt = get_salt();
                    set_setting("passHash", sha256(salt + textBox4.Text));
                    set_setting("s", salt);
                    set.s = salt;
                    set.passHash = sha256(salt + textBox4.Text);
                }
                else
                {
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

                stat = "Protecting system";
                do_protection();

                // Progress 5
                progress++;
                stat = "Finished";

                install_Finished = true;
            }catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void do_protection()
        {
            try
            {
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
                MessageBox.Show(ex.ToString());
            }
           
        }
        private void protect_file(string file)
        {
            try
            {
                string name = get_name();
                DirectoryInfo dirinfo = new DirectoryInfo(file);
                File.Copy(file, file.Replace(dirinfo.Name, name + ".log"));
                set_setting(file, file.Replace(dirinfo.Name, name + ".log"));
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
                    MessageBox.Show("First");
                    MessageBox.Show(err);
                }
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
                File.Delete(file);
                File.Copy(textBox6.Text + "SuspiciousActBlocker.exe", file);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

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



        private void set_setting(string name, string value)
        {
            try
            {
                Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("Semrau Software Consulting").OpenSubKey("SuspiciousActBlocker", true).SetValue(name, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
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

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
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

        private void button9_Click_1(object sender, EventArgs e)
        {
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

        private void button15_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"In the next screen you will select where to save the XML. Note, this XML *MUST* have the name ""silent.xml"" and it *MUST* be in the same directory that the installer is executing from to work properly.  If you have a logo that you specified, that must also be in the same folder.");
            saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (var writer = new StreamWriter(saveFileDialog1.FileName))
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(set.GetType());
                    serializer.Serialize(writer, set);
                    writer.Flush();
                }

            }
        }
    }
}
public class settings
{
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