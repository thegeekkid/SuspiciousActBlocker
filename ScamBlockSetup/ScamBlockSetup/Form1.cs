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

namespace ScamBlockSetup
{
    public partial class Form1 : Form
    {
        public string stat;
        public int progress = 0;
        public static int target = 5;
        public bool install_Finished = false;


        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            welcome.Visible = false;
            install_type.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            install_type.Visible = false;
            welcome.Visible = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                radioButton1.Checked = false;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                radioButton2.Checked = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            install_type.Visible = false;
            if (radioButton2.Checked)
            {
                branding.Visible = true;
            }else
            {
                protected_items.Visible = true;
            }
        }




        private void textBox5_TextChanged_1(object sender, EventArgs e)
        {
            if (File.Exists(textBox5.Text))
            {
                pictureBox1.Image = Image.FromFile(textBox5.Text);
                pictureBox1.Visible = true;
            }else
            {
                pictureBox1.Visible = false;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox5.Text = openFileDialog1.FileName;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            protected_items.Visible = false;
            if (radioButton2.Checked)
            {
                branding.Visible = true;
            }else
            {
                install_type.Visible = true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            branding.Visible = false;
            protected_items.Visible = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox6.Text = Environment.SpecialFolder.Programs + @"\Semrau Software Consulting\Scam Block\";
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox6.Text = folderBrowserDialog1.SelectedPath + @"\";
            }
        }

        private void trigger_install()
        {
            Thread watcher = new Thread(status_updater);
            Thread worker = new Thread(do_install);

            worker.Start();
            watcher.Start();
        }

        private void status_updater()
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

        private void do_install()
        {
            stat = "Creating folders";
            if (Directory.Exists(textBox6.Text))
            {
                Directory.Delete(textBox6.Text);
            }
            Directory.CreateDirectory(textBox6.Text);
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
                DirectoryInfo dirinfo = new DirectoryInfo(textBox5.Text);
                logo_name = dirinfo.Name;
                stat = "Copying files";
                File.Copy(textBox5.Text, textBox6.Text + logo_name);
            }

            //Progress 3
            progress++;

            stat = "Saving settings";
            set_setting("install_location", textBox6.Text);
            set_setting("company", textBox2.Text);
            set_setting("website", textBox3.Text);
            set_setting("contactInfo", richTextBox2.Text.Replace(Environment.NewLine, @"\n"));
            set_setting("remote_site", textBox1.Text);

            if (textBox4.Text != "")
            {

                string salt = get_salt();
                set_setting("passHash", sha256(salt + textBox4.Text));
                set_setting("s", salt);
            }else
            {
                set_setting("passHash", "");
                set_setting("s", "");
            }
            

            set_setting("lockdown_enabled", checkBox1.Checked.ToString());
            set_setting("logo", logo_name);

            if (radioButton2.Checked)
            {
                set_setting("install_type", "msp");
            }else
            {
                set_setting("install_type", "consumer");
            }

            // Progress 4
            progress++;

            stat = "Protecting system";
            do_protection();

            // Progress 5
            progress++;
            stat = "Finished";

            install_Finished = true;
        }

        private void do_protection()
        {
            if (checkBox2.Checked)
            {
                protect_file(@"C:\Windows\System32\syskey.exe");
            }
            else
            {
                set_setting(@"C:\Windows\System32\syskey.exe", "false");
            }
            if (checkBox3.Checked)
            {
                protect_file(@"C:\Windows\System32\eventvwr.exe");
            }
            else
            {
                set_setting(@"C:\Windows\System32\eventvwr.exe", "false");
            }
            if (checkBox4.Checked)
            {
                protect_file(@"C:\Windows\System32\mmc.exe");
            }
            else
            {
                set_setting(@"C:\Windows\System32\mmc.exe", "false");
            }
            if (checkBox5.Checked)
            {
                protect_file(@"C:\Windows\regedit.exe");
            }
            else
            {
                set_setting(@"C:\Windows\regedit.exe", "false");
            }
            if (checkBox6.Checked)
            {
                protect_file(@"C:\Windows\System32\msconfig.exe");
            }
            else
            {
                set_setting(@"C:\Windows\System32\msconfig.exe", "false");
            }
            if (checkBox7.Checked)
            {
                protect_file(@"C:\Windows\System32\services.exe");
            }
            else
            {
                set_setting(@"C:\Windows\System32\services.exe", "false");
            }
            if (checkBox8.Checked)
            {
                protect_file(@"C:\Windows\System32\cmd.exe");
            }
            else
            {
                set_setting(@"C:\Windows\System32\cmd.exe", "false");
            }
            if (checkBox9.Checked)
            {
                protect_file(@"C:\Windows\System32\perfmon.exe");
            }
            else
            {
                set_setting(@"C:\Windows\System32\perfmon.exe", "false");
            }
            if (checkBox10.Checked)
            {
                protect_file(@"C:\Windows\System32\notepad.exe");
            }
            else
            {
                set_setting(@"C:\Windows\System32\notepad.exe", "false");
            }
        }
        private void protect_file(string file)
        {
            string name = get_name();
            DirectoryInfo dirinfo = new DirectoryInfo(file);
            File.Copy(file, file.Replace(dirinfo.Name, name + ".log"));
            set_setting(file, file.Replace(dirinfo.Name, name + ".log"));
        }

        private string get_salt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[1024];
            rng.GetBytes(buffer);
            return BitConverter.ToString(buffer);
        }

        private string get_name()
        {
            string name = sha256(get_salt());
            do
            {
                name = sha256(get_salt());
            } while ((File.Exists(@"C:\Windows\" + name + ".log") || (File.Exists(@"C:\Windows\System32\" + name + ".log"))));
            return name;
        }

        static string sha256(string input)
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



        private void set_setting(string name, string value)
        {
            Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("Semrau Software Consulting").OpenSubKey("SuspiciousActBlocker", true).SetValue(name, value);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            install_dir.Visible = false;
            protected_items.Visible = true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if ((Directory.Exists(textBox6.Text)) && (MessageBox.Show("Warning!  The folder exists and all data will be erased.  Are you sure that you want to continue?", "Warning", MessageBoxButtons.YesNo) == DialogResult.No))
            {
                // Directory exists and user doesn't want to continue
                textBox6.Text = "";
            }else
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
    }
}
