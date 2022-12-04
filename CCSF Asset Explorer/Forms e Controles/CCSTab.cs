using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace CCSF_Asset_Explorer
{
    public partial class CCSTab : TabPage  
    {
        public CCSF CCSFile;

        private Principal _principal;

        public PictureBox textureBox = new PictureBox()
        {
            Name = "textureBox",
            Size = new System.Drawing.Size(321, 150),
            SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        };
        FileEntry SelectedFile;
        CLUT SelectedClut;

        public CCSNode[] Copy;
        public List<CCSNode> PastChecked;
        bool UncheckInProgress = false;

        #region Functions
        public void CopyNodes()
        {
            Copy = GetCheckedNodes(frameView);
            if (Copy.Length > 0)
            {
                foreach (CCSNode node in GetCheckedNodes(frameView))
                    node.ForeColor = Color.Red;
            }
        }
        public void PasteNodes()
        {
            CCSNode ccsn = null;

            CCSNode[] FrameGroups = Copy.Where(x => x.Level == 0).ToArray();
            CCSNode[] FrameBlocks = Copy.Where(x => x.Level == 1).ToArray();

            if (FrameGroups.Length > 0)
            {
                foreach (CCSNode node in FrameGroups)
                {
                    ccsn = new CCSNode(node.Text);
                    ccsn.FrameBlocks = node.FrameBlocks;
                    foreach (CCSNode subn in node.Nodes)
                        ccsn.Nodes.Add(new CCSNode(subn.Text)
                        {
                            Block = subn.Block
                        });
                    CCSFile.CCS_Header.FrameCounter++;
                }
                frameView.Nodes.Insert(frameView.SelectedNode.Index + 1, ccsn);
            }

            if (FrameBlocks.Length > 0)
            {
                foreach (CCSNode node in FrameBlocks)
                {
                    ccsn = new CCSNode(node.Text);
                    ccsn.Block = node.Block;
                    //ccsn.FrameBlocks = node.FrameBlocks;
                }
                if (frameView.SelectedNode.Level == 1)
                {
                    frameView.SelectedNode.Parent.Nodes.Insert(frameView.SelectedNode.Index + 1, ccsn);
                    //frameView.Nodes.Add(ccsn);
                    //Copy = null;

                    foreach (CCSNode node in frameView.Nodes.Cast<CCSNode>().Where(x => x.ForeColor == Color.Red))
                        node.ForeColor = Color.Black;

                }
                else if(frameView.SelectedNode.Level==0)
                {
                    frameView.SelectedNode.Nodes.Add(ccsn);
                    //frameView.Nodes.Add(ccsn);
                    //Copy = null;

                    foreach (CCSNode node in frameView.Nodes.Cast<CCSNode>().Where(x => x.ForeColor == Color.Red))
                        node.ForeColor = Color.Black;

                }
            }
        }
        private void Populate(CCSNode node, bool Frame = false)
        {
            if (Frame == true)
            {
                if (node.FrameBlocks != null)
                {
                    propertyGrid1.SelectedObject = null;
                }
                else
                {
                    if (node.Block != null && node.Text != "Resources")
                    {
                        propertyGrid1.SelectedObject = node.Block;
                        label2.Text = node.Block.GetObjectName();

                        if(node.Block.GetBlockType()=="Texture")
                        {
                            propPanel.Controls.Add(textureBox);
                        }
                        else
                        {
                            propPanel.Controls.Remove(textureBox);
                        }
                    }
                    else
                    {
                        propertyGrid1.SelectedObject = null;
                        label2.Text = "Resources";
                    }
                    }
                }
            else
            {

                if (node.File != null)
                {
                    propertyGrid1.SelectedObject = node.File;
                    label2.Text = Path.GetFileName(node.File.FileName);
                }
                else if (node.Object != null)
                {
                    propertyGrid1.SelectedObject = node.Object;
                    label2.Text = node.Object.ObjectName;
                }
                else
                {
                    propertyGrid1.SelectedObject = node.Block;
                    label2.Text = node.Block.GetObjectName();

                    if (node.Block.GetBlockType() == "Texture")
                    {
                        propPanel.Controls.Add(textureBox);
                    }
                    else
                    {
                        propPanel.Controls.Remove(textureBox);
                    }
                }

            }
        }

        public CCSNode[] GetCheckedNodes(TreeView treev)
        {

            var list = new List<CCSNode>();
            list.AddRange(treev.Nodes.Cast<CCSNode>().Where(x => x.Checked));
            list.AddRange(treev.Nodes.Cast<CCSNode>().SelectMany(x => x.Nodes.Cast<CCSNode>()
            .Where(c => c.Checked)));
            return list.ToArray();
        }

        public void UncheckNodes(TreeView treev)
        {
            UncheckInProgress = true;
            var nodes = GetCheckedNodes(treev);
            if (nodes.Length > 0)
                foreach (CCSNode node in nodes)
                    node.Checked = false;
            UncheckInProgress = false;

            UpdateCheckedCount();
        }

        public bool isFrameNodesChecked() => frameView.Nodes.Cast<CCSNode>().Any(x => x.Nodes.Cast<CCSNode>().Any(c => c.Checked));

        public CCSNode GetRootGroup() => frameView.SelectedNode.Level == 0 ? (frameView.SelectedNode as CCSNode) :
            frameView.SelectedNode.Level == 1 ? (frameView.SelectedNode.Parent as CCSNode) : null;
        public List<Block> GetFrameBlocks() => frameView.SelectedNode.Level == 0 ? (frameView.SelectedNode as CCSNode).FrameBlocks :
            frameView.SelectedNode.Level == 1 ? (frameView.SelectedNode.Parent as CCSNode).FrameBlocks : null;
        public Block GetFrameRoot() => frameView.SelectedNode.Level == 0 ? (frameView.SelectedNode as CCSNode).Block :
            frameView.SelectedNode.Level == 1 ? (frameView.SelectedNode.Parent as CCSNode).Block : null;
        public FileEntry GetSelectedFile() => !CCSFile.CCS_TOC.Files.Where(x => x.FileName == @"%\").ToArray()[0]
                        .Objects.Any(o => o.ObjectName == (resourceView.SelectedNode.Level == 0 ?
            resourceView.SelectedNode.Text :

            resourceView.SelectedNode.Level == 1 ?
            resourceView.SelectedNode.Parent.Text : null)
                        ) ? 
            (
            resourceView.SelectedNode.Level == 0 ?
            CCSFile.CCS_TOC.GetFile(resourceView.SelectedNode.Text) :

            resourceView.SelectedNode.Level == 1 ?
            CCSFile.CCS_TOC.GetFile(resourceView.SelectedNode.Parent.Text) :

            resourceView.SelectedNode.Level == 2 ?
            CCSFile.CCS_TOC.GetFile(resourceView.SelectedNode.Parent.Parent.Text) :
            null
            ) : CCSFile.CCS_TOC.GetFile(@"%\")
           
        ; //substituir por caso de root object
        public ObjectEntry GetSelectedObject() => resourceView.Visible ?
            //Resource View
            (resourceView.SelectedNode.Level == 1 ?
            (resourceView.SelectedNode as CCSNode).Object :

            resourceView.SelectedNode.Level == 2 ?
            (resourceView.SelectedNode.Parent as CCSNode).Object :

            null) : 
            //FrameView
            frameView.Visible ?

            (frameView.SelectedNode.Level == 1 ?
            (frameView.SelectedNode as CCSNode).Object :

            null) 
            
            : null;
            
        public CCSTab(CCSF file)
        {
            foreach (Form form in Application.OpenForms)
                if (form.Name == "Principal")
                    _principal = form as Principal;

            InitializeComponent();
            CCSFile = file;
            this.Text = CCSFile.Name;

            textureBox.DoubleClick += textureBox_DoubleClick;

            if (CCSFile.CCS_TOC.isFrameScene)
            {
                resourceView.Visible = false;
                frameView.Visible = true;

                _principal.frameModeSceneToolStripMenuItem.Checked = true;
                _principal.resourceTreeToolStripMenuItem.Checked = false;
                _principal.frameModeSceneToolStripMenuItem.Checked = true;
                _principal.frameModeSceneToolStripMenuItem.Visible = true;
                resourceViewbt.Visible = true;
                CCSFile.ToTreeView(this.frameView);
                CCSFile.ToTreeView(this.resourceView, true);
            }
            else
            {
                _principal.frameModeSceneToolStripMenuItem.Checked = false;
                _principal.resourceTreeToolStripMenuItem.Checked = true;

                resourceView.Visible = true;
                frameView.Visible = false;
                _principal.resourceTreeToolStripMenuItem.Checked = true;
                _principal.resourceTreeToolStripMenuItem.Enabled = false;
                _principal.frameModeSceneToolStripMenuItem.Checked = false;
                _principal.frameModeSceneToolStripMenuItem.Visible = false;
                resourceViewbt.Visible = false;
                CCSFile.ToTreeView(this.resourceView);
            }

            var k = new List<string>();
            foreach (var ffile in CCSFile.CCS_TOC.Files)
                foreach (var objectx in ffile.Objects)
                    k.Add(objectx.ObjectName);
            CustomTypeConverter.Names = k.ToArray();         
            
            UpdateInformation();
        }

        
        public void UpdateInformation()
        {
            label1.Text = $"CCSF Type:   {(CCSFile.CCS_TOC.isFrameScene ? "Scene" : "Assets")}";
            linkLabel1.Text = CCSFile.File_path;
        }
        public void ClearTextureBox()
        {
            if(textureBox!=null)
            if (textureBox.Image != null)
                textureBox.CreateGraphics().Clear(textureBox.BackColor);
        }
        void Update(bool first = true)
        {
            if (frameView.Visible)
            {
                CCSNode node = frameView.SelectedNode as CCSNode;
                //nfo1lbl.Text = $"Data blocks count: {(GetRootGroup().Text == "Resources" ? GetRootGroup().Nodes.Count : GetFrameBlocks().Count)}";
                //info2lbl.Text = $"Frame Index: {GetRootGroup().Index}";

                if (node.Block != null)
                    if (node.Block.GetBlockType() == "Texture")
                    {
                        var tex = node.Block as Texture;
                        var clut = (CCSFile.Blocks.Where(t => t.ObjectID == tex.CLUTID).ToArray()[0] as CLUT);
                        textureBox.Image = tex.ToBitmap(clut);
                    }
                    else
                    {
                        propPanel.Controls.Remove(textureBox);
                    }
            }
            else
            {

                switch (SelectedFile.Ftype)
                {
                    case FileType.Bitmap:
                        if (SelectedFile.Objects.Any(x => x.Blocks.Any(c => c.Type == 0xcccc0300)) &&
                            SelectedFile.Objects.Any(x => x.Blocks.Any(c => c.Type == 0xcccc0400)))
                        {
                            var cluts = SelectedFile.Objects.Where(t => t.ObjectName.StartsWith("CLT")).ToArray();
                            if (first == true)
                            {
                                //if (palettesBox.Items.Count > 0)
                                //    palettesBox.Items.Clear();

                                //palettesBox.Items.AddRange(Enumerable.Range(0, (int)cluts.Count()).Select(x => cluts[x].ObjectName).ToArray());

                                //if (palettesBox.SelectedItem == null)
                                //    palettesBox.SelectedItem = palettesBox.Items[0];
                            }


                            var tex = (SelectedFile.Objects.Where(t => t.ObjectName.StartsWith("TEX")).ToArray()[0].Blocks
                               [0] as Texture);
                            var clut = CCSFile.Blocks.Where(t => t.ObjectID == tex.CLUTID).ToArray();

                            if (clut != null && clut.Length > 0 && clut[0].GetBlockType()=="CLUT")
                            {

                                propPanel.Controls.Add(textureBox);
                                //old1 = propPanel.RowStyles[1];

                                SelectedClut = (clut[0] as CLUT);
                                textureBox.Image = tex.ToBitmap(SelectedClut);
                            }

                        }
                        break;
                    default:
                        propPanel.Controls.Remove(textureBox);

                        break;
                }
            }
        }
        void UpdateButtons(bool visible)
        {
            _principal.extractToolStripMenuItem1.Enabled = visible;
            _principal.removeToolStripMenuItem.Enabled = visible;
            _principal.replaceToolStripMenuItem.Enabled = visible;
            _principal.propriedadesToolStripMenuItem.Enabled = visible;
            if (frameView.Nodes.Count > 0 &&
                GetCheckedNodes(frameView).Length>0)
            {
                _principal.uncheckAllToolStripMenuItem.Enabled = true;
                unchkbt.Visible = true;
            }
            else
            {
                _principal.uncheckAllToolStripMenuItem.Enabled = false;
                unchkbt.Visible = false;
            }
            extractBt.Enabled = visible;
            replaceBt.Enabled = visible;
            addBt.Enabled = visible;
            replaceBt.Enabled = visible;
            removeBt.Enabled = visible;
            
        }
        #endregion

        #region Events and Buttons
        private void textureBox_DoubleClick(object sender, EventArgs e)
        {
            textureBox.Image.Save(label2.Text+".png", System.Drawing.Imaging.ImageFormat.Png);
            File.SetAttributes(label2.Text + ".png", FileAttributes.Hidden);
            Program.OpenWithProgram(label2.Text + ".png",_principal.DefaultTextureVE);
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedClut != null)
                Update(false);
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer.exe", Path.GetDirectoryName(linkLabel1.Text));
        }
        private void frameView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            /*_principal.extractToolStripMenuItem1.Enabled = false;
            _principal.removeToolStripMenuItem.Enabled = false;
            _principal.replaceToolStripMenuItem.Enabled = false;
            _principal.propriedadesToolStripMenuItem.Enabled = false;
            extractBt.Enabled = false;
            replaceBt.Enabled = false;
            addBt.Enabled = false;
            replaceBt.Enabled = false;
            removeBt.Enabled = false;
            propBt.Enabled = false;*/

            if (frameView.SelectedNode != null)
            {
                /*_principal.extractToolStripMenuItem1.Enabled = true;
                _principal.removeToolStripMenuItem.Enabled = true;
                _principal.replaceToolStripMenuItem.Enabled = true;
                _principal.propriedadesToolStripMenuItem.Enabled = true;
                extractBt.Enabled = true;
                replaceBt.Enabled = true;
                addBt.Enabled = true;
                replaceBt.Enabled = true;
                removeBt.Enabled = true;
                propBt.Enabled = true;*/

                if (Copy != null)
                {
                    _principal.pasteToolStripMenuItem.Enabled = true;

                    foreach (CCSNode node in frameView.Nodes.Cast<CCSNode>().Where(x => x.BackColor == Color.Green))
                        node.BackColor = Color.White;

                    if(frameView.SelectedNode.Level==0)
                        frameView.SelectedNode.BackColor = Color.Green;
                }
                Populate(frameView.SelectedNode as CCSNode, true);
            }
            /*switch (frameView.SelectedNode.Level)
            {
                case 0: //Root - Files and Objects[Without file types] || Scene - Frame: [n]
                    info2lbl.Text = " ";
                    break;

                case 1: //Primary - File Objects || Scene - Blocks

                    Block block = null;
                    if (GetRootGroup().Text != "Resources")
                    {
                        var blocks = GetFrameBlocks();
                        block = blocks[frameView.SelectedNode.Index];
                    }
                    else
                    {
                        block = (frameView.SelectedNode as CCSNode).Block;
                    }
                    Console.WriteLine($"\r\nBlock selected: {block.GetBlockType()}" +
                        $"\nLinked with Object: {(CCSFile.CCS_TOC.GetObject(block) != null ? CCSFile.CCS_TOC.GetObject(block).ObjectName : "NO OBJECT")}");

                    break;
            }

            Update(true);*/
        }
        private void frameView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            //Console.WriteLine("Marcou");
            if (UncheckInProgress == false)
            {
                UpdateCheckedCount();
                PastChecked = GetCheckedNodes(frameView).ToList();
                var cas = frameView.Nodes.Cast<CCSNode>().Where(x => x.Level == 0).ToArray();

                #region Event Magic Check/Uncheck
                if (cas != null)
                {
                    frameView.AfterCheck -= frameView_AfterCheck;

                    frameView.BeginUpdate();

                    for (int i = 0; i < cas.Count(); i++)
                    {
                        foreach (CCSNode nodex in cas[i].Nodes)
                            if(nodex.Parent.Checked)
                                nodex.Checked = nodex.Parent.Checked;
                    }

                    frameView.EndUpdate();

                    UpdateCheckedCount();
                    frameView.AfterCheck += frameView_AfterCheck;
                }


                #endregion
            }
        }
        void UpdateCheckedCount()
        {
            #region Checked Count and Infos
            int checkcount = GetCheckedNodes(frameView).Length;
            if (checkcount == 0)
            {
                _principal.extractToolStripMenuItem1.Enabled = false;
                _principal.removeToolStripMenuItem.Enabled = false;
                _principal.replaceToolStripMenuItem.Enabled = false;
                _principal.propriedadesToolStripMenuItem.Enabled = false;
                _principal.copyToolStripMenuItem.Enabled = false;
                _principal.pasteToolStripMenuItem.Enabled = false;
                extractBt.Enabled = false;
                replaceBt.Enabled = false;
                addBt.Enabled = false;
                replaceBt.Enabled = false;
                removeBt.Enabled = false;
                _principal.uncheckAllToolStripMenuItem.Enabled = false;
            }
            else
            {
                _principal.copyToolStripMenuItem.Enabled = true;
                _principal.pasteToolStripMenuItem.Enabled = true;
                _principal.extractToolStripMenuItem1.Enabled = true;
                _principal.removeToolStripMenuItem.Enabled = true;
                _principal.replaceToolStripMenuItem.Enabled = true;
                _principal.propriedadesToolStripMenuItem.Enabled = true;
                extractBt.Enabled = true;
                replaceBt.Enabled = true;
                addBt.Enabled = true;
                replaceBt.Enabled = true;
                removeBt.Enabled = true;
                
                _principal.uncheckAllToolStripMenuItem.Enabled = true;
                unchkbt.Visible = true;
                //label2.Text = $"Checked: {checkcount}";
            }
            #endregion
        }
        private void resourceView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SelectedFile = GetSelectedFile();

            UpdateButtons(false);

            if (resourceView.SelectedNode != null)
            {
                UpdateButtons(true);
            }
            switch (resourceView.SelectedNode.Level)
            {
                case 0: //Root - Files and Objects[Without file types]
                    ClearTextureBox();
                    _principal.removeToolStripMenuItem.Enabled = false;
                    removeBt.Enabled = false;
                    break;

                case 1: //Primary - File Objects 

                    _principal.removeToolStripMenuItem.Enabled = false;
                    removeBt.Enabled = false;
                    ClearTextureBox();
                    break;

                case 2://Secondary Final - Object Data Blocks
                    ClearTextureBox();
                    _principal.removeToolStripMenuItem.Enabled = true;
                    removeBt.Enabled = true;
                    //if ((resourceView.SelectedNode as CCSNode).Block.GetBlockType() == "Animation")
                    //{
                    //    var anim = (resourceView.SelectedNode as CCSNode).Block as Animation;
                    //    new FramesBox(anim).ShowDialog();
                    //}
                    break;
            }

            Update(true);
            Populate(resourceView.SelectedNode as CCSNode, false);
        }
        private void resourceView_VisibleChanged(object sender, EventArgs e)
        {
            UncheckNodes(frameView);
            UpdateButtons(false);
        }
        private void extractBt_Click(object sender, EventArgs e)
        {
            _principal.extractToolStripMenuItem1.PerformClick();
        }
        private void replaceBt_Click(object sender, EventArgs e)
        {
            _principal.replaceToolStripMenuItem.PerformClick();
        }
        private void addBt_Click(object sender, EventArgs e)
        {
            _principal.pasteToolStripMenuItem.PerformClick();
        }
        private void removeBt_Click(object sender, EventArgs e)
        {
            _principal.removeToolStripMenuItem.PerformClick();
        }
        private void propBt_Click(object sender, EventArgs e)
        {
            _principal.propriedadesToolStripMenuItem.PerformClick();
        }
        private void resourceViewbt_Click(object sender, EventArgs e)
        {
            _principal.resourceTreeToolStripMenuItem.PerformClick();
        }
        private void unchkbt_Click(object sender, EventArgs e)
        {
            UncheckNodes(frameView);
            unchkbt.Visible = false;
        }
        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            Update(false);
            propertyGrid1.Refresh();
        }

        private void frameView_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {

        }
        #endregion

    }
}
