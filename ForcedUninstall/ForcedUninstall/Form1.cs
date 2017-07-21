using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

namespace ForcedUninstall
{
    public partial class Form1 : Form
    {
        public string debug = "";




        string install_location = "";
        string regedit = "";
        string cmd = "";
        string eventvwr = "";
        string mmc = "";
        string msconfig = "";
        string notepad = "";
        string perfmon = "";
        string syskey = "";
        string install_type = "";
        bool company = false;
        bool contactInfo = false;
        bool lockdown_enabled = false;
        bool logo = false;
        bool pass = false;
        bool salt = false;
        bool remote_site = false;
        bool website = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            label2.Visible = checkBox1.Checked;
            richTextBox1.Visible = checkBox1.Checked;
        }

        private void trad_uninstall()
        {

            gatherInfo();

            if (regedit != "false")
            {
                cleanFile(regedit, @"C:\Windows\regedit.exe");
            }

            if (cmd != "false")
            {
                cleanFile(cmd, @"C:\Windows\System32\cmd.exe");
            }

            if (eventvwr != "false")
            {
                cleanFile(eventvwr, @"C:\Windows\System32\eventvwr.exe");
            }

            if (mmc != "false")
            {
                cleanFile(mmc, @"C:\Windows\System32\mmc.exe");
            }

            if (msconfig != "false")
            {
                cleanFile(msconfig, @"C:\Windows\System32\msconfig.exe");
            }

            if (notepad != "false")
            {
                cleanFile(notepad, @"C:\Windows\System32\notepad.exe");
            }

            if (perfmon != "false")
            {
                cleanFile(perfmon, @"C:\Windows\System32\perfmon.exe");
            }

            if (syskey != "false")
            {
                cleanFile(syskey, @"C:\Windows\System32\syskey.exe");
            }

            if (Directory.Exists(install_location))
            {
                logit("Install location exists - clearing and deleting");
                Directory.Delete(install_location, true);
            }

            Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("Semrau Software Consulting", true).DeleteSubKey("SuspiciousActBlocker");

            logit("Deleted registry key");

            if (checkBox1.Checked == true)
            {
                submitDebug();
            }

            logit("Uninstall finished.");

            MessageBox.Show("Uninstall complete!");
            Environment.Exit(0);
            Application.Exit();
            this.Close();
        }

        private void submitDebug()
        {
            File.WriteAllText(Environment.CurrentDirectory + @"\debug.txt", ("Submitted issue: " + Environment.NewLine + richTextBox1.Text + Environment.NewLine + Environment.NewLine + "Debug info: " + Environment.NewLine + Environment.NewLine + debug));
        }

        private void cleanFile(string coded_file, string target)
        {
            
            if (File.Exists(coded_file))
            {
                try
                {
                    logit("Unprotecting " + target + ".  Coded file found.  Running clean.");
                    perm_file(coded_file);
                    perm_file(target);
                    File.Delete(target);
                    File.Copy(coded_file, target);
                }
                catch (Exception ex)
                {
                    logit("Error unprotecting " + target + ": " + Environment.NewLine + ex.ToString());
                    MessageBox.Show(debug);
                }
                
            }
            
        }

        private void perm_file(string file)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = @"C:\Windows\System32\takeown.exe";
            proc.StartInfo.Arguments = @"/f " + file;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Start();
            proc.WaitForExit();

            proc = new Process();
            proc.StartInfo.FileName = @"C:\Windows\System32\icacls.exe";
            proc.StartInfo.Arguments = file + @"/grant """ + Environment.UserName + @""":F";
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Start();
            proc.WaitForExit();
        }

        private void gatherInfo()
        {
            try
            {
                install_location = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker", "install_location", "null").ToString();

                regedit = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker", @"C:\Windows\regedit.exe", "null").ToString();

                cmd = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker", @"C:\Windows\System32\cmd.exe", "null").ToString();

                eventvwr = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker", @"C:\Windows\System32\eventvwr.exe", "null").ToString();

                mmc = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker", @"C:\Windows\System32\mmc.exe", "null").ToString();

                msconfig = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker", @"C:\Windows\System32\msconfig.exe", "null").ToString();

                notepad = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker", @"C:\Windows\System32\notepad.exe", "null").ToString();

                perfmon = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker", @"C:\Windows\System32\perfmon.exe", "null").ToString();

                syskey = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker", @"C:\Windows\System32\syskey.exe", "null").ToString();

                install_type = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker", @"install_type", "null").ToString();

                if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker", @"company", null) != null)
                {
                    company = true;
                }

                if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker", @"contactInfo", null) != null)
                {
                    contactInfo = true;
                }

                if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker", @"lockdown_enabled", null) != null)
                {
                    lockdown_enabled = true;
                }

                if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker", @"logo", null) != null)
                {
                    logo = true;
                }

                if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker", @"passHash", null) != null)
                {
                    pass = true;
                }

                if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker", @"s", null) != null)
                {
                    salt = true;
                }

                if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker", @"remote_site", null) != null)
                {
                    remote_site = true;
                }

                if (Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker", @"website", null) != null)
                {
                    website = true;
                }
            }
            catch (Exception ex)
            {
                logit("Error getting settings from registry: " + Environment.NewLine + ex.ToString());
                MessageBox.Show(debug);
            }

            try
            {
                logit("Install location: " + install_location);
                logit("Install type: " + install_type);
                logit("regedit: " + regedit);
                logit("cmd: " + cmd);
                logit("Event Viewer: " + eventvwr);
                logit("mmc: " + mmc);
                logit("msconfig: " + msconfig);
                logit("notepad: " + notepad);
                logit("perfmon: " + perfmon);
                logit("syskey: " + syskey);

                logit("Company set: " + company.ToString());
                logit("Contact info set: " + contactInfo.ToString());
                logit("Lockdown enabled: " + lockdown_enabled.ToString());
                logit("Logo set: " + logo.ToString());
                logit("Password set: " + pass.ToString());
                logit("Salt set: " + salt.ToString());
                logit("Remote site set: " + remote_site.ToString());
                logit("Website set: " + website.ToString());
            }
            catch (Exception ex)
            {
                logit("Error logging settings: " + Environment.NewLine + ex.ToString());
                MessageBox.Show(debug);
            }
        }

        private void logit(string message)
        {
            if (debug != "")
            {
                debug += Environment.NewLine + Environment.NewLine;
            }
            debug += message;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                trad_uninstall();
            }
        }
    }
}
