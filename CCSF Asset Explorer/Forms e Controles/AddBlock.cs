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
    public partial class AddBlock : Form
    {
        public Block newBlock;
        public AddBlock()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(comboBox1.SelectedItem)
            {
                case "Texture":
                    newBlock = new Texture();
                    break;
                default:
                    newBlock = new Block();
                    break;
            }
            propertyGrid1.SelectedObject = newBlock;
        }
    }
}
