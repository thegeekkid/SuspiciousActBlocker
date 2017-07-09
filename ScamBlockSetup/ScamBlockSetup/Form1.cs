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

namespace ScamBlockSetup
{
    public partial class Form1 : Form
    {
        public string stat;
        public int progress = 0;
        public static int target = 0;
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

        }

        private void status_updater()
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
        }

        private void button8_Click(object sender, EventArgs e)
        {
            install_dir.Visible = false;
            protected_items.Visible = true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            install_dir.Visible = false;
            status.Visible = true;
            trigger_install();
        }
    }
}
