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
    public partial class FramesBox : Form
    {
        Animation Anim;
        public FramesBox(Animation anim)
        {
            InitializeComponent();
            Anim = anim;
            Anim.ToTreeView(frameView);
        }
    }
}
