using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace CCSF_Asset_Explorer
{
    partial class Sobre : Form
    {
        public Sobre()
        {
            InitializeComponent();
            verslb.Text = $"Versão {Assembly.GetExecutingAssembly().GetName().Version.Major}.{Assembly.GetExecutingAssembly().GetName().Version.Minor}  Rev {Assembly.GetExecutingAssembly().GetName().Version.Revision}";
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }


        private void Sobre_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void atalhoslink(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var at = new Atalhos();
            at.ShowDialog();
        }

        private void doclink(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("Manual.txt");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("www.bitmundo.xyz");
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/MiguelQueiroz010");
        }

        private void Closebt_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
