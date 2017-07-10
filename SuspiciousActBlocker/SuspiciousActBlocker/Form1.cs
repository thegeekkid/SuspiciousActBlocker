using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace SuspiciousActBlocker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // Get the name of the executable they were trying to run (since we use the same exe for everything and the name is dynamic).
                vars.target_name = Process.GetCurrentProcess().MainModule.FileName;

                // Placeholder for the arguments that this was started with - we will pass those along if an override happens.
                vars.args = "";

                // Load each argument into the placeholder.
                foreach (string arg in Environment.GetCommandLineArgs())
                {
                    // The first argument tends to be the executable name - just skip that one.
                    if (arg != vars.target_name)
                    {
                        // Add a space to our placeholder if it isn't the first argument in there.
                        if (vars.args != "")
                        {
                            vars.args += " ";
                        }
                        vars.args += arg;
                    }

                }

                // Call function to load settings from the registry into the vars class.
                load_settings();

                // If we are a branded install, load the branding.
                if (vars.install_type == "msp")
                {
                    if (vars.logo != "")
                    {
                        if (File.Exists(vars.install_location + vars.logo))
                        {
                            // Reposition the label so it isn't over/under the logo.
                            label2.Location = new Point(9, 253);
                            // Load the logo into the picturebox.
                            pictureBox1.Image = Image.FromFile(vars.install_location + vars.logo);
                        }
                    }

                    // Replace variables.
                    label2.Text = label2.Text.Replace(@"%company%", vars.company).Replace(@"%contactInfo%", vars.contactInfo);

                    // Check if they specified a website - otherwise just hide the button.
                    if (vars.website != "")
                    {
                        button1.Text = button1.Text.Replace(@"%company%", vars.company);
                    }
                    else
                    {
                        button1.Visible = false;
                    }

                    // Same with their remote support site.
                    if (vars.remoteSite != "")
                    {
                        button2.Text = button2.Text.Replace(@"%company%", vars.company);
                    }
                    else
                    {
                        button2.Visible = false;
                    }


                    // Check if there is a password hash.
                    if (vars.passHash == "")
                    {
                        // If not, no need to show the things related to the password or say that someone has a password.  Talk about confusing!
                        textBox1.Visible = false;
                        label3.Visible = false;
                        button4.Visible = true;
                        if (label2.Text.Contains(@", they have the password to bypass this message."))
                        {
                            label2.Text.Replace(@", they have the password to bypass this message.", ".");
                        }
                    }
                    if (vars.lockdown_enabled == "False")
                    {
                        // Hide the lockdown option if the installer didn't want that.
                        button5.Visible = false;
                    }
                }
                else
                {
                    // Not branded, change the form accordingly.
                    textBox1.Visible = false;
                    label3.Visible = false;
                    button4.Visible = true;
                    button1.Visible = false;
                    button2.Visible = false;
                    button4.Location = new Point(546, 452);
                    label2.Text = "This program is commonly used by telephone scammers in order to trick people into giving them money." + Environment.NewLine +
    "If you were called out of the blue, then this is most likely a scam - please hang up." + Environment.NewLine +
    @"If you are already connected to them, please click the ""Lockdown remote sessions"" button until you can uninstall" + Environment.NewLine +
    "the remote software.";
                }
            }
            catch (Exception ex)
            {
                // Herp derp - show a error.
                MessageBox.Show("Warning!  This program is commonly used by telephone scammers to trick people into giving them money.  Normally we would have an override screen where you could bypass this warning; however, there is a problem with the applicaiton.  Please send this to support for troubleshooting: " + Environment.NewLine + ex.ToString());
            }
            
        }

        // Load the settings from the registry into the vars class.
        private void load_settings()
        {
            try
            {
                // Yes - I'm lazy and use a function instead of writing out the same subkey structure over and over again.
                vars.install_type = get_setting("install_type");
                vars.install_location = get_setting("install_location");
                vars.company = get_setting("company");
                vars.logo = get_setting("logo");
                vars.contactInfo = get_setting("contactInfo").Replace(@"\n", Environment.NewLine);
                vars.website = get_setting("website");
                vars.remoteSite = get_setting("remote_site");
                vars.passHash = get_setting("passHash");
                vars.lockdown_enabled = get_setting("lockdown_enabled");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error loading settings.  Please send this information to support: " + Environment.NewLine + ex.ToString());
            }
        }


        // Triggered when the user has successfully authenticated or if there is no password and the user clicked continue.
        private void openAnyway()
        {
            try
            {
                // We need an external exe to run this so we can close out and not get an "in use" warning.
                Process proc = new Process();
                proc.StartInfo.FileName = vars.install_location + @"executor.exe";
                string pw = "";
                if (textBox1.Text == "")
                {
                    // Bogus placeholder - this isn't evaled if the hash is empty anyway.
                    pw = "na";
                }
                else
                {
                    // Placeholder for the plaintext password.  May re-do this later for security.  (Lol - ya... riiiigggghhhhhttttttttttt)
                    pw = textBox1.Text;
                }

                // Just pass auth, target, and arguments to the executor app.
                proc.StartInfo.Arguments = @"""" + pw + @""" """ + vars.target_name + @"""";
                if (vars.args != "")
                {
                    proc.StartInfo.Arguments += @" """ + vars.args + @"""";
                }

                // No need to show it.  Just shows debug info anyway.
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                // Bye for now!
                Environment.Exit(0);
                this.Close();
                Application.Exit();
            }
            catch (Exception ex)
            {
                // ooooopppppsssss....
                MessageBox.Show("Error overriding: " + Environment.NewLine + ex.ToString());
            }

        }

        

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.Count() > 0)
                {
                    button4.Visible = true;
                }
                else
                {
                    button4.Visible = false;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(vars.website);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry - I couldn't load the website.  Please send the following error message to support: " + ex.ToString());
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(vars.remoteSite);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sorry - I couldn't load the website.  Please send the following error message to support: " + ex.ToString());
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Environment.Exit(0);
                this.Close();
                Application.Exit();
            }catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (vars.passHash == "")
                {
                    openAnyway();
                }
                else
                {
                    if (sha256(get_setting("s") + textBox1.Text) == vars.passHash)
                    {
                        openAnyway();
                    }
                    else
                    {
                        MessageBox.Show("Incorrect password.");
                        textBox1.Text = "";
                        button4.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                lockdown ld = new lockdown();
                this.Visible = false;
                ld.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error entering lockdown: " + Environment.NewLine + ex.ToString());
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
                MessageBox.Show("Error hashing input." + Environment.NewLine + ex.ToString());
                return "";
            }
            
            
        }

        private string get_setting(string name)
        {
            try
            {
                return Registry.LocalMachine.OpenSubKey("SOFTWARE", false).OpenSubKey("Semrau Software Consulting", false).OpenSubKey("SuspiciousActBlocker", false).GetValue(name).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting " + name + " setting.  Please send this error to support: " + Environment.NewLine + ex.ToString());
                return "";
            }

        }

        
    }

    public class vars
    {
        public static string install_type { get; set; }
        public static string install_location { get; set; }
        public static string company { get; set; }
        public static string logo { get; set; }
        public static string contactInfo { get; set; }
        public static string website { get; set; }
        public static string remoteSite { get; set; }
        public static string passHash { get; set; }
        public static string lockdown_enabled { get; set; }
        public static string target_name { get; set; }
        public static string args { get; set; }
    }
}
