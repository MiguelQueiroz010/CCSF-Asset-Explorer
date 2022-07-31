
namespace CCSF_Asset_Explorer
{
    partial class SearchCheck
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchCheck));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.secheck_bt = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.blockCombo = new System.Windows.Forms.ComboBox();
            this.blkseparator = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.blocklbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(71, 42);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(163, 20);
            this.textBox1.TabIndex = 1;
            // 
            // comboBox1
            // 
            this.comboBox1.Enabled = false;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(71, 102);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(163, 21);
            this.comboBox1.TabIndex = 3;
            // 
            // secheck_bt
            // 
            this.secheck_bt.Location = new System.Drawing.Point(207, 185);
            this.secheck_bt.Name = "secheck_bt";
            this.secheck_bt.Size = new System.Drawing.Size(75, 23);
            this.secheck_bt.TabIndex = 4;
            this.secheck_bt.Text = "Search";
            this.secheck_bt.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(47, 79);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(83, 17);
            this.checkBox1.TabIndex = 5;
            this.checkBox1.Text = "Check from:";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Location = new System.Drawing.Point(47, 19);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(63, 17);
            this.checkBox2.TabIndex = 6;
            this.checkBox2.Text = "Search:";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // blockCombo
            // 
            this.blockCombo.Enabled = false;
            this.blockCombo.FormattingEnabled = true;
            this.blockCombo.Location = new System.Drawing.Point(80, 145);
            this.blockCombo.Name = "blockCombo";
            this.blockCombo.Size = new System.Drawing.Size(163, 21);
            this.blockCombo.TabIndex = 7;
            this.blockCombo.Visible = false;
            // 
            // blkseparator
            // 
            this.blkseparator.AutoSize = true;
            this.blkseparator.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.blkseparator.Location = new System.Drawing.Point(77, 126);
            this.blkseparator.Name = "blkseparator";
            this.blkseparator.Size = new System.Drawing.Size(11, 16);
            this.blkseparator.TabIndex = 8;
            this.blkseparator.Text = "|";
            this.blkseparator.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Object:";
            // 
            // blocklbl
            // 
            this.blocklbl.AutoSize = true;
            this.blocklbl.Location = new System.Drawing.Point(37, 148);
            this.blocklbl.Name = "blocklbl";
            this.blocklbl.Size = new System.Drawing.Size(37, 13);
            this.blocklbl.TabIndex = 10;
            this.blocklbl.Text = "Block:";
            this.blocklbl.Visible = false;
            // 
            // SearchCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 220);
            this.Controls.Add(this.blocklbl);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.blkseparator);
            this.Controls.Add(this.blockCombo);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.secheck_bt);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.textBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SearchCheck";
            this.Text = "Search and Check";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button secheck_bt;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.ComboBox blockCombo;
        private System.Windows.Forms.Label blkseparator;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label blocklbl;
    }
}