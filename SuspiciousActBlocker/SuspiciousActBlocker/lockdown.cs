using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace SuspiciousActBlocker
{
    public partial class lockdown : Form
    {
        public Thread lmikill;
        public Thread tvkill;
        public Thread sckill;
        public Thread jmekill;
        public Thread zakill;
        public Thread splashkill;
        public Thread bomgarkill;
        public Thread inetkill;
        public Thread count;
        public lockdown()
        {
            InitializeComponent();
        }

        private void lockdown_Load(object sender, EventArgs e)
        {
            try
            {
                lmikill = new Thread(lmi);
                tvkill = new Thread(tv);
                sckill = new Thread(SC);
                jmekill = new Thread(jme);
                zakill = new Thread(za);
                splashkill = new Thread(splashtop);
                bomgarkill = new Thread(bomgar);
                inetkill = new Thread(inet);

                lmikill.Start();
                button1.BackColor = Color.Orange;
                tvkill.Start();
                button2.BackColor = Color.Orange;
                sckill.Start();
                button3.BackColor = Color.Orange;
                jmekill.Start();
                button5.BackColor = Color.Orange;
                zakill.Start();
                button6.BackColor = Color.Orange;
                splashkill.Start();
                button7.BackColor = Color.Orange;
                bomgarkill.Start();
                button4.BackColor = Color.Orange;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing: " + Environment.NewLine + ex.ToString());
            }
            
        }
        private void lmi()
        {
            try
            {
                do
                {
                    tkill(@"LMI_Rescue.exe,LMI_RescueRC.exe");
                    Thread.Sleep(3000);
                } while (1 == 1);
            }
            catch
            {

            }
           
        }
        private void tv()
        {
            try
            {
                do
                {
                    tkill(@"TeamViewer.exe,TeamViewer_Desktop.exe");
                    Thread.Sleep(3000);
                } while (1 == 1);
            }
            catch
            {

            }
            
        }
        private void SC()
        {
            try
            {
                do
                {
                    tkill(@"ScreenConnect.WindowsClient.exe,ScreenConnect.ClientService.exe");
                    Thread.Sleep(3000);
                } while (1 == 1);
            }
            catch
            {

            }
            
        }
        private void jme()
        {
            try
            {
                do
                {
                    tkill(@"join.me.exe,join.me.installer.exe,LMISupportM32.exe");
                    Thread.Sleep(3000);
                } while (1 == 1);
            }
            catch
            {

            }
            
        }
        private void za()
        {
            try
            {
                do
                {
                    tkill(@"agent.exe,agent.exe");
                    Thread.Sleep(3000);
                } while (1 == 1);
            }
            catch
            {

            }
            
        }
        private void splashtop()
        {
            try
            {
                do
                {
                    tkill(@"SplashtopSOS.exe,SRServerSOS.exe,SRFeatureSOS.exe,SRManagerSOS.exe");
                    Thread.Sleep(3000);
                } while (1 == 1);
            }
            catch
            {

            }
           
        }
        private void bomgar()
        {
            try
            {
                do
                {
                    tkill(@"bomgar-scc.exe,bomgar*");
                    Thread.Sleep(3000);
                } while (1 == 1);
            }
            catch
            {

            }
            
        }
        private void tkill(string names)
        {
            try
            {
                foreach (string name in names.Split(','))
                {
                    Process proc = new Process();
                    proc.StartInfo.FileName = @"C:\Windows\System32\taskkill.exe";
                    proc.StartInfo.Arguments = @"/f /im " + name;
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.Start();
                }
            }catch
            {

            }
            
        }
        private void inet()
        {
            try
            {
                do
                {
                    Process proc = new Process();
                    proc.StartInfo.FileName = @"C:\Windows\System32\ipconfig.exe";
                    proc.StartInfo.Arguments = @"/release";
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.Start();
                    Thread.Sleep(1000);
                } while (1 == 1);
            }
            catch
            {

            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (button1.BackColor == Color.Orange)
                {
                    lmikill.Abort();
                    button1.BackColor = Color.Green;
                    button1.Text = "LogMeIn Rescue" + Environment.NewLine + "(Enabled)";
                }
                else
                {
                    lmikill = new Thread(lmi);
                    lmikill.Start();
                    button1.BackColor = Color.Orange;
                    button1.Text = "LogMeIn Rescue" + Environment.NewLine + "(Disabled)";
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (button2.BackColor == Color.Orange)
                {
                    tvkill.Abort();
                    button2.BackColor = Color.Green;
                    button2.Text = "TeamViewer" + Environment.NewLine + "(Enabled)";
                }
                else
                {
                    tvkill = new Thread(tv);
                    tvkill.Start();
                    button2.BackColor = Color.Orange;
                    button2.Text = "TeamViewer" + Environment.NewLine + "(Disabled)";
                }
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
                if (button3.BackColor == Color.Orange)
                {
                    sckill.Abort();
                    button3.BackColor = Color.Green;
                    button3.Text = "ScreenConnect" + Environment.NewLine + "(Enabled)";
                }
                else
                {
                    sckill = new Thread(SC);
                    sckill.Start();
                    button3.BackColor = Color.Orange;
                    button3.Text = "ScreenConnect" + Environment.NewLine + "(Disabled)";
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
                if (button5.BackColor == Color.Orange)
                {
                    jmekill.Abort();
                    button5.BackColor = Color.Green;
                    button5.Text = "Join.Me" + Environment.NewLine + "(Enabled)";
                }
                else
                {
                    jmekill = new Thread(jme);
                    jmekill.Start();
                    button5.BackColor = Color.Orange;
                    button5.Text = "Join.Me" + Environment.NewLine + "(Disabled)";
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
                if (button4.BackColor == Color.Orange)
                {
                    bomgarkill.Abort();
                    button4.BackColor = Color.Green;
                    button4.Text = "Bomgar" + Environment.NewLine + "(Enabled)";
                }
                else
                {
                    bomgarkill = new Thread(bomgar);
                    bomgarkill.Start();
                    button4.BackColor = Color.Orange;
                    button4.Text = "Bomgar" + Environment.NewLine + "(Disabled)";
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
                if (button6.BackColor == Color.Orange)
                {
                    zakill.Abort();
                    button6.BackColor = Color.Green;
                    button6.Text = "Zoho Assist" + Environment.NewLine + "(Enabled)";
                }
                else
                {
                    zakill = new Thread(za);
                    zakill.Start();
                    button6.BackColor = Color.Orange;
                    button6.Text = "Zoho Assist" + Environment.NewLine + "(Disabled)";
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
                if (button7.BackColor == Color.Orange)
                {
                    splashkill.Abort();
                    button7.BackColor = Color.Green;
                    button7.Text = "Splashtop" + Environment.NewLine + "(Enabled)";
                }
                else
                {
                    splashkill = new Thread(za);
                    splashkill.Start();
                    button7.BackColor = Color.Orange;
                    button7.Text = "Splashtop" + Environment.NewLine + "(Disabled)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try {
                if (button8.BackColor == Color.Red)
                {
                    inetkill = new Thread(inet);
                    inetkill.Start();
                    button8.BackColor = Color.Green;
                    button8.Text = "Enable Internet";
                }
                else
                {
                    inetkill.Abort();
                    button8.BackColor = Color.Red;
                    button8.Text = "Drastic - Disable Internet";
                    Process proc = new Process();
                    proc.StartInfo.FileName = @"C:\Windows\System32\ipconfig.exe";
                    proc.StartInfo.Arguments = @"/renew";
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                exit.Visible = true;
                exitProcedure();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }
        private void exitProcedure()
        {
            try
            {
                count = new Thread(countdown);
                count.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }
        private void countdown()
        {
            try
            {
                int elapsed_time = 0;

                do
                {
                    if (label4.InvokeRequired)
                    {
                        label4.BeginInvoke((MethodInvoker)delegate ()
                        {
                            label4.Text = (20 - elapsed_time).ToString();
                        });
                    }
                    else
                    {
                        label4.Text = (20 - elapsed_time).ToString();
                    }

                    Thread.Sleep(1000);
                    elapsed_time++;
                } while (elapsed_time <= 20);

                Environment.Exit(0);
                this.Close();
                Application.Exit();
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
                count.Abort();
                exit.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }
    }
}
