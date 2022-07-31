
namespace CCSF_Asset_Explorer
{
    partial class Options
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.cancelbt = new System.Windows.Forms.Button();
            this.okbt = new System.Windows.Forms.Button();
            this.applybt = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.General = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.openCheck = new System.Windows.Forms.CheckBox();
            this.pathBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.editcheck = new System.Windows.Forms.CheckBox();
            this.appiconBox = new System.Windows.Forms.PictureBox();
            this.appName = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.General.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.appiconBox)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 91.20879F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.791209F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(499, 364);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 74.39024F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.60976F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel2.Controls.Add(this.cancelbt, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.okbt, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.applybt, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 335);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(493, 26);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // cancelbt
            // 
            this.cancelbt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cancelbt.Location = new System.Drawing.Point(405, 3);
            this.cancelbt.Name = "cancelbt";
            this.cancelbt.Size = new System.Drawing.Size(85, 20);
            this.cancelbt.TabIndex = 1;
            this.cancelbt.Text = "Cancelar";
            this.cancelbt.UseVisualStyleBackColor = true;
            this.cancelbt.Click += new System.EventHandler(this.cancelbt_Click);
            // 
            // okbt
            // 
            this.okbt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.okbt.Location = new System.Drawing.Point(353, 3);
            this.okbt.Name = "okbt";
            this.okbt.Size = new System.Drawing.Size(46, 20);
            this.okbt.TabIndex = 0;
            this.okbt.Text = "Ok";
            this.okbt.UseVisualStyleBackColor = true;
            this.okbt.Click += new System.EventHandler(this.okbt_Click);
            // 
            // applybt
            // 
            this.applybt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.applybt.Location = new System.Drawing.Point(264, 3);
            this.applybt.Name = "applybt";
            this.applybt.Size = new System.Drawing.Size(83, 20);
            this.applybt.TabIndex = 2;
            this.applybt.Text = "Aplicar";
            this.applybt.UseVisualStyleBackColor = true;
            this.applybt.Click += new System.EventHandler(this.applybt_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.General);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(493, 326);
            this.tabControl1.TabIndex = 1;
            // 
            // General
            // 
            this.General.Controls.Add(this.tableLayoutPanel3);
            this.General.Location = new System.Drawing.Point(4, 22);
            this.General.Name = "General";
            this.General.Padding = new System.Windows.Forms.Padding(3);
            this.General.Size = new System.Drawing.Size(485, 300);
            this.General.TabIndex = 0;
            this.General.Text = "General";
            this.General.ToolTipText = "General tool options.";
            this.General.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel3.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.163265F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 169F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(479, 294);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.appName);
            this.groupBox1.Controls.Add(this.appiconBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.pathBox);
            this.groupBox1.Controls.Add(this.editcheck);
            this.groupBox1.Controls.Add(this.openCheck);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(313, 119);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Texture Double Click";
            // 
            // openCheck
            // 
            this.openCheck.AutoSize = true;
            this.openCheck.Location = new System.Drawing.Point(92, 48);
            this.openCheck.Name = "openCheck";
            this.openCheck.Size = new System.Drawing.Size(77, 17);
            this.openCheck.TabIndex = 0;
            this.openCheck.Text = "Open With";
            this.openCheck.UseVisualStyleBackColor = true;
            this.openCheck.CheckedChanged += new System.EventHandler(this.openCheck_CheckedChanged);
            // 
            // pathBox
            // 
            this.pathBox.Location = new System.Drawing.Point(81, 93);
            this.pathBox.Name = "pathBox";
            this.pathBox.ReadOnly = true;
            this.pathBox.Size = new System.Drawing.Size(178, 20);
            this.pathBox.TabIndex = 2;
            this.pathBox.Text = "Click to select a Application.";
            this.pathBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.pathBox.Click += new System.EventHandler(this.pathBox_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "App path:";
            // 
            // editcheck
            // 
            this.editcheck.AutoSize = true;
            this.editcheck.Location = new System.Drawing.Point(190, 48);
            this.editcheck.Name = "editcheck";
            this.editcheck.Size = new System.Drawing.Size(69, 17);
            this.editcheck.TabIndex = 1;
            this.editcheck.Text = "Edit With";
            this.editcheck.UseVisualStyleBackColor = true;
            this.editcheck.CheckedChanged += new System.EventHandler(this.editcheck_CheckedChanged);
            // 
            // appiconBox
            // 
            this.appiconBox.Location = new System.Drawing.Point(25, 32);
            this.appiconBox.Name = "appiconBox";
            this.appiconBox.Size = new System.Drawing.Size(38, 33);
            this.appiconBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.appiconBox.TabIndex = 4;
            this.appiconBox.TabStop = false;
            // 
            // appName
            // 
            this.appName.AutoSize = true;
            this.appName.Location = new System.Drawing.Point(15, 68);
            this.appName.Name = "appName";
            this.appName.Size = new System.Drawing.Size(0, 13);
            this.appName.TabIndex = 5;
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 364);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Options";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.General.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.appiconBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button cancelbt;
        private System.Windows.Forms.Button okbt;
        private System.Windows.Forms.Button applybt;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage General;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox pathBox;
        private System.Windows.Forms.CheckBox openCheck;
        private System.Windows.Forms.CheckBox editcheck;
        private System.Windows.Forms.PictureBox appiconBox;
        private System.Windows.Forms.Label appName;
    }
}