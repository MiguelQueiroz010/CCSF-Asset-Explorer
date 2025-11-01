namespace CCSF_Asset_Explorer
{
    partial class VFWEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VFWEditor));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.BaseIMG = new System.Windows.Forms.PictureBox();
            this.VFW = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.entrySelector = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.Heightupd = new System.Windows.Forms.NumericUpDown();
            this.widthUpd = new System.Windows.Forms.NumericUpDown();
            this.yUpd = new System.Windows.Forms.NumericUpDown();
            this.xUpd = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.openBT = new System.Windows.Forms.Button();
            this.exportBT = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BaseIMG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.VFW)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.entrySelector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Heightupd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.widthUpd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.yUpd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xUpd)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.BaseIMG, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.VFW, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 68.43374F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 31.56627F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(825, 415);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // BaseIMG
            // 
            this.BaseIMG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BaseIMG.Location = new System.Drawing.Point(3, 3);
            this.BaseIMG.Name = "BaseIMG";
            this.BaseIMG.Size = new System.Drawing.Size(406, 278);
            this.BaseIMG.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.BaseIMG.TabIndex = 0;
            this.BaseIMG.TabStop = false;
            // 
            // VFW
            // 
            this.VFW.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VFW.Location = new System.Drawing.Point(415, 3);
            this.VFW.Name = "VFW";
            this.VFW.Size = new System.Drawing.Size(407, 278);
            this.VFW.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.VFW.TabIndex = 1;
            this.VFW.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.entrySelector);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.Heightupd);
            this.groupBox1.Controls.Add(this.widthUpd);
            this.groupBox1.Controls.Add(this.yUpd);
            this.groupBox1.Controls.Add(this.xUpd);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 287);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(406, 125);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Control";
            // 
            // entrySelector
            // 
            this.entrySelector.Location = new System.Drawing.Point(49, 61);
            this.entrySelector.Name = "entrySelector";
            this.entrySelector.Size = new System.Drawing.Size(59, 20);
            this.entrySelector.TabIndex = 9;
            this.entrySelector.ValueChanged += new System.EventHandler(this.entrySelector_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(54, 39);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "ENTRY";
            // 
            // Heightupd
            // 
            this.Heightupd.Location = new System.Drawing.Point(328, 62);
            this.Heightupd.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.Heightupd.Name = "Heightupd";
            this.Heightupd.Size = new System.Drawing.Size(49, 20);
            this.Heightupd.TabIndex = 7;
            this.Heightupd.ValueChanged += new System.EventHandler(this.xUpd_ValueChanged);
            // 
            // widthUpd
            // 
            this.widthUpd.Location = new System.Drawing.Point(219, 62);
            this.widthUpd.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.widthUpd.Name = "widthUpd";
            this.widthUpd.Size = new System.Drawing.Size(49, 20);
            this.widthUpd.TabIndex = 6;
            this.widthUpd.ValueChanged += new System.EventHandler(this.xUpd_ValueChanged);
            // 
            // yUpd
            // 
            this.yUpd.Location = new System.Drawing.Point(301, 36);
            this.yUpd.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.yUpd.Name = "yUpd";
            this.yUpd.Size = new System.Drawing.Size(49, 20);
            this.yUpd.TabIndex = 5;
            this.yUpd.ValueChanged += new System.EventHandler(this.xUpd_ValueChanged);
            // 
            // xUpd
            // 
            this.xUpd.Location = new System.Drawing.Point(223, 36);
            this.xUpd.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
            this.xUpd.Name = "xUpd";
            this.xUpd.Size = new System.Drawing.Size(49, 20);
            this.xUpd.TabIndex = 4;
            this.xUpd.ValueChanged += new System.EventHandler(this.xUpd_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(175, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Width:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(281, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Height:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(278, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Y:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(200, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "X:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.openBT);
            this.groupBox2.Controls.Add(this.exportBT);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(415, 287);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(407, 125);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Export";
            // 
            // openBT
            // 
            this.openBT.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.openBT.Location = new System.Drawing.Point(55, 45);
            this.openBT.Name = "openBT";
            this.openBT.Size = new System.Drawing.Size(134, 44);
            this.openBT.TabIndex = 1;
            this.openBT.Text = "Open BIN";
            this.openBT.UseVisualStyleBackColor = true;
            this.openBT.Click += new System.EventHandler(this.openBT_Click);
            // 
            // exportBT
            // 
            this.exportBT.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.exportBT.Location = new System.Drawing.Point(224, 46);
            this.exportBT.Name = "exportBT";
            this.exportBT.Size = new System.Drawing.Size(134, 44);
            this.exportBT.TabIndex = 0;
            this.exportBT.Text = "Export BIN";
            this.exportBT.UseVisualStyleBackColor = true;
            this.exportBT.Click += new System.EventHandler(this.exportBT_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(119, 103);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(131, 16);
            this.label6.TabIndex = 10;
            this.label6.Text = "TEXTURE INDEX:";
            // 
            // VFWEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 415);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "VFWEditor";
            this.Text = "VFWEditor";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.BaseIMG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.VFW)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.entrySelector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Heightupd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.widthUpd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.yUpd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xUpd)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.PictureBox BaseIMG;
        public System.Windows.Forms.PictureBox VFW;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button exportBT;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown Heightupd;
        private System.Windows.Forms.NumericUpDown widthUpd;
        private System.Windows.Forms.NumericUpDown yUpd;
        private System.Windows.Forms.NumericUpDown xUpd;
        private System.Windows.Forms.NumericUpDown entrySelector;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button openBT;
        private System.Windows.Forms.Label label6;
    }
}