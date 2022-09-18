
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
            this.searchText = new System.Windows.Forms.TextBox();
            this.objectCombo = new System.Windows.Forms.ComboBox();
            this.secheck_bt = new System.Windows.Forms.Button();
            this.checkFromObj = new System.Windows.Forms.CheckBox();
            this.textSearch = new System.Windows.Forms.CheckBox();
            this.blockCombo = new System.Windows.Forms.ComboBox();
            this.blkseparator = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.blocklbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // searchText
            // 
            this.searchText.Location = new System.Drawing.Point(71, 42);
            this.searchText.Name = "searchText";
            this.searchText.Size = new System.Drawing.Size(163, 20);
            this.searchText.TabIndex = 1;
            // 
            // objectCombo
            // 
            this.objectCombo.Enabled = false;
            this.objectCombo.FormattingEnabled = true;
            this.objectCombo.Location = new System.Drawing.Point(71, 102);
            this.objectCombo.Name = "objectCombo";
            this.objectCombo.Size = new System.Drawing.Size(163, 21);
            this.objectCombo.TabIndex = 3;
            this.objectCombo.SelectedIndexChanged += new System.EventHandler(this.objectCombo_SelectedIndexChanged);
            // 
            // secheck_bt
            // 
            this.secheck_bt.Location = new System.Drawing.Point(207, 185);
            this.secheck_bt.Name = "secheck_bt";
            this.secheck_bt.Size = new System.Drawing.Size(75, 23);
            this.secheck_bt.TabIndex = 4;
            this.secheck_bt.Text = "Search";
            this.secheck_bt.UseVisualStyleBackColor = true;
            this.secheck_bt.Click += new System.EventHandler(this.secheck_bt_Click);
            // 
            // checkFromObj
            // 
            this.checkFromObj.AutoSize = true;
            this.checkFromObj.Location = new System.Drawing.Point(47, 79);
            this.checkFromObj.Name = "checkFromObj";
            this.checkFromObj.Size = new System.Drawing.Size(83, 17);
            this.checkFromObj.TabIndex = 5;
            this.checkFromObj.Text = "Check from:";
            this.checkFromObj.UseVisualStyleBackColor = true;
            this.checkFromObj.CheckedChanged += new System.EventHandler(this.checkFromObj_CheckedChanged);
            // 
            // textSearch
            // 
            this.textSearch.AutoSize = true;
            this.textSearch.Checked = true;
            this.textSearch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.textSearch.Location = new System.Drawing.Point(47, 19);
            this.textSearch.Name = "textSearch";
            this.textSearch.Size = new System.Drawing.Size(63, 17);
            this.textSearch.TabIndex = 6;
            this.textSearch.Text = "Search:";
            this.textSearch.UseVisualStyleBackColor = true;
            this.textSearch.CheckedChanged += new System.EventHandler(this.textSearch_CheckedChanged);
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
            this.Controls.Add(this.textSearch);
            this.Controls.Add(this.checkFromObj);
            this.Controls.Add(this.secheck_bt);
            this.Controls.Add(this.objectCombo);
            this.Controls.Add(this.searchText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SearchCheck";
            this.Text = "Search and Check";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox searchText;
        private System.Windows.Forms.ComboBox objectCombo;
        private System.Windows.Forms.Button secheck_bt;
        private System.Windows.Forms.CheckBox checkFromObj;
        private System.Windows.Forms.CheckBox textSearch;
        private System.Windows.Forms.ComboBox blockCombo;
        private System.Windows.Forms.Label blkseparator;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label blocklbl;
    }
}