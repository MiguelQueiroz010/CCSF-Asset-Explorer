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
        string raidengithub = "https://github.com/MiguelQueiroz010";
        public Sobre()
        {
            InitializeComponent();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(raidengithub);
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
    }
}
