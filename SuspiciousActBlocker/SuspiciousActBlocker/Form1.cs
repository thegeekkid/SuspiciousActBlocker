﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
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
            vars.target_name = Process.GetCurrentProcess().MainModule.FileName;
            load_settings();
            if (vars.logo != "")
            {
                label1.Location = new Point(218, 219);
                label2.Location = new Point(9, 284);
                pictureBox1.Image = Image.FromFile(vars.logo);
            }

            label2.Text = label2.Text.Replace(@"%company%", vars.company).Replace(@"%contactInfo%", vars.contactInfo);

            button1.Text = button1.Text.Replace(@"%company%", vars.company);
            button2.Text = button2.Text.Replace(@"%company%", vars.company);
        }

        private void load_settings()
        {
            vars.install_location = get_setting("install_location");
            vars.company = get_setting("company");
            vars.logo = get_setting("logo");
            vars.contactInfo = get_setting("contactInfo").Replace(@"\n", Environment.NewLine);
            vars.website = get_setting("website");
            vars.remoteSite = get_setting("remote_site");
            vars.passHash = get_setting("passHash");
        }

        private string get_setting(string name)
        {
            return Registry.LocalMachine.OpenSubKey("SOFTWARE", false).OpenSubKey("Semrau Software Consulting", false).OpenSubKey("SuspiciousActBlocker", false).GetValue(name).ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Count() > 0)
            {
                button4.Visible = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start(vars.website);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start(vars.remoteSite);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    public class vars
    {
        public static string install_location { get; set; }
        public static string company { get; set; }
        public static string logo { get; set; }
        public static string contactInfo { get; set; }
        public static string website { get; set; }
        public static string remoteSite { get; set; }
        public static string passHash { get; set; }
        public static string target_name { get; set; }
    }
}
