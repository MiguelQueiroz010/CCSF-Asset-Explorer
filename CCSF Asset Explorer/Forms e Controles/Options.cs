using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CCSF_Asset_Explorer
{
    public partial class Options : Form
    {
        Principal _principal;
        public Options(Principal principal)
        {
            InitializeComponent();
            _principal = principal;
            GetConfig();
        }

        private void GetConfig()
        {
            #region General
            if(_principal.DefaultTextureVE=="")
            {
                openCheck.Checked = false;
                editcheck.Checked = false;
            }
            else
            {
                if (!_principal.EditTexture)
                {
                    openCheck.Checked = true;
                    editcheck.Checked = false;
                }
                else
                {
                    openCheck.Checked = false;
                    editcheck.Checked = true;
                }
                pathBox.Text = _principal.DefaultTextureVE;
                if (_principal.DefaultTextureVE != "")
                {
                    var icon = Program.ExtractIconFromFilePath(_principal.DefaultTextureVE);
                    if (icon != null)
                    {
                        appiconBox.Image = icon.ToBitmap();
                        appName.Text = new FileInfo(_principal.DefaultTextureVE).Name;
                    }
                }
            }

            #endregion
        }

        private void cancelbt_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void okbt_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void applybt_Click(object sender, EventArgs e)
        {
            _principal.CreateConfig();
            _principal.Config();
            GetConfig();
        }

        private void editcheck_CheckedChanged(object sender, EventArgs e)
        {
            if (editcheck.Checked)
            {
                openCheck.Checked = false;
                _principal.EditTexture = true;
            }
        }

        private void openCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (openCheck.Checked)
            {
                editcheck.Checked = false;
                _principal.EditTexture = false;
            }
            else
            {
                _principal.DefaultTextureVE = "";
                appName.Text = "";
                appiconBox.CreateGraphics().Clear(Color.Transparent);
                pathBox.Text = "Click to select a Application.";
            }
        }

        private void pathBox_Click(object sender, EventArgs e)
        {
            var open = new OpenFileDialog();
            open.Filter = "Windows Application Executable(*.exe)|*.exe";
            open.Title = "Open the Application file.";
            if(open.ShowDialog()==DialogResult.OK)
            {
                _principal.DefaultTextureVE = open.FileName;
                pathBox.Text = _principal.DefaultTextureVE;
                appName.Text = new FileInfo(_principal.DefaultTextureVE).Name;
                appiconBox.Image = Program.ExtractIconFromFilePath(_principal.DefaultTextureVE).ToBitmap();
            }
        }
    }
}
