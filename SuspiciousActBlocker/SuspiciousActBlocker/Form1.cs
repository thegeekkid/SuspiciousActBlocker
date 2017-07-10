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
                vars.target_name = Process.GetCurrentProcess().MainModule.FileName;
                vars.args = "";
                foreach (string arg in Environment.GetCommandLineArgs())
                {
                    if (arg != vars.target_name)
                    {
                        if (vars.args != "")
                        {
                            vars.args += " ";
                        }
                        vars.args += arg;
                    }

                }
                load_settings();

                if (vars.install_type == "msp")
                {
                    if (vars.logo != "")
                    {
                        if (File.Exists(vars.install_location + vars.logo))
                        {
                            //label1.Location = new Point(218, 219);
                            label2.Location = new Point(9, 253);
                            pictureBox1.Image = Image.FromFile(vars.install_location + vars.logo);
                        }
                    }

                    label2.Text = label2.Text.Replace(@"%company%", vars.company).Replace(@"%contactInfo%", vars.contactInfo);

                    if (vars.website != "")
                    {
                        button1.Text = button1.Text.Replace(@"%company%", vars.company);
                    }
                    else
                    {
                        button1.Visible = false;
                    }

                    if (vars.remoteSite != "")
                    {
                        button2.Text = button2.Text.Replace(@"%company%", vars.company);
                    }
                    else
                    {
                        button2.Visible = false;
                    }

                    if (vars.passHash == "")
                    {
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
                        button5.Visible = false;
                    }
                }
                else
                {
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
                MessageBox.Show("Warning!  This program is commonly used by telephone scammers to trick people into giving them money.  Normally we would have an override screen where you could bypass this warning; however, there is a problem with the applicaiton.  Please send this to support for troubleshooting: " + Environment.NewLine + ex.ToString());
            }
            
        }

        private void load_settings()
        {
            try
            {
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

        private void openAnyway()
        {

            try
            {
                Process proc = new Process();
                proc.StartInfo.FileName = vars.install_location + @"executor.exe";
                string pw = "";
                if (textBox1.Text == "")
                {
                    pw = "na";
                }
                else
                {
                    pw = textBox1.Text;
                }
                proc.StartInfo.Arguments = @"""" + pw + @""" """ + vars.target_name + @"""";
                if (vars.args != "")
                {
                    proc.StartInfo.Arguments += @" """ + vars.args + @"""";
                }
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.CreateNoWindow = true;
                //MessageBox.Show(proc.StartInfo.Arguments);
                proc.Start();
                Environment.Exit(0);
                this.Close();
                Application.Exit();
            }
            catch (Exception ex)
            {
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
