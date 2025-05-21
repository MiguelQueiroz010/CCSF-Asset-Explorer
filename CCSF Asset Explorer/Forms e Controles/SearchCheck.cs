using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CCSF_Asset_Explorer
{
    public partial class SearchCheck : Form
    {
        Principal Main;

        ObjectEntry objectSelected;
        public SearchCheck(Principal _main)
        {
            Main = _main;
            InitializeComponent();
            Start();
        }

        public void Start()
        {
            if (checkFromObj.Checked)
            {
                var ccsfile = Main.GetSelectedTab().CCSFile;
                string[] objects = Enumerable.Range(0, ccsfile.Blocks.Count).Select
                    (x => ccsfile.Blocks[x].GetObjectName()).ToArray();

                var sort = new List<string>();
                foreach (var obj in objects)
                    if (!sort.Contains(obj))
                        sort.Add(obj);

                sort.Sort();
                sort.RemoveAll(x => x == "NO OBJECT");

                objects = sort.ToArray();

                objectCombo.Items.AddRange(objects.ToArray());
                searchText.Text = "";
            }
            else if (textSearch.Checked)
            {
                searchText.Text = "";
                objectCombo.Items.Clear();
                blockCombo.Items.Clear();
                objectCombo.Enabled = false;
                blockCombo.Visible = false;
                blockCombo.Enabled = false;
            }
        }

        private void textSearch_CheckedChanged(object sender, EventArgs e)
        {
            if (textSearch.Checked)
            {
                checkFromObj.Checked = false;
                objectCombo.Enabled = false;
                secheck_bt.Text = "Search";
            }
            Start();
        }

        private void checkFromObj_CheckedChanged(object sender, EventArgs e)
        {
            if (checkFromObj.Checked)
            {
                textSearch.Checked = false;
                objectCombo.Enabled = true;
                secheck_bt.Text = "Check All";
            }
            Start();
        }

        private void objectCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (objectCombo.SelectedItem != "")
            {
                var ccsfile = Main.GetSelectedTab().CCSFile;
                blockCombo.Items.Clear();

                blockCombo.Visible = true;
                blockCombo.Enabled = true;

                var blocks = new List<Block>();
                foreach (var fil in ccsfile.CCS_TOC.Files)
                    foreach (var obj in fil.Objects.Where(o => o.ObjectName == objectCombo.SelectedItem))
                    {
                        objectSelected = obj;
                        int b = 0;
                        foreach (var block in obj.Blocks)
                        {
                            blockCombo.Items.Add($"Block_{b}");
                            b++;
                        }
                    }
            }
        }

        private void secheck_bt_Click(object sender, EventArgs e)
        {
            if (objectSelected != null)
            {
                foreach (CCSTab tab in Main.tabControl1.TabPages)
                {
                    var res = tab.resourceView;
                    foreach (CCSNode rnode in res.Nodes)
                        foreach (CCSNode rnode1 in rnode.Nodes)
                            foreach (CCSNode node in rnode1.Nodes)
                            {
                                if (objectSelected.Blocks.Contains(node.Block))
                                {
                                    node.BackColor = Color.Green;
                                    rnode1.Checked = true;
                                    node.Checked = true;
                                }
                            }
                }
            }
        }
    }
}
