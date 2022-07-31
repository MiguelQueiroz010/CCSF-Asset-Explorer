using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


    public class CCSNode: TreeNode
    {
        public FileEntry File;
        public ObjectEntry Object;
        public Block Block;

        public List<Block> FrameBlocks;
    public CCSNode()
    {

    }
    public CCSNode(string text)
    {
        this.Text = text;
    }
}
    
