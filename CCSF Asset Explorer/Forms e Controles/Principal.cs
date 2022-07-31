using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace CCSF_Asset_Explorer
{
    public partial class Principal : Form
    {
        SearchCheck sc;
        Size oldPIC2size;
        Point oldPIC2loc;

        public string DefaultTextureVE = "explorer";
        bool Animations;
        public Principal()
        {
            InitializeComponent();
            Config();
            pictureBox1.Enabled = Animations;
            pictureBox2.Enabled = Animations;
        }

        #region Functions

        public PropertyBox propBox;
        public CCSTab GetSelectedTab() => 
            (CCSTab)this.tabControl1.SelectedTab;

        public void Config()
        {
            if (File.Exists("config.xml"))
            {
                Stream xml = File.OpenText("config.xml").BaseStream;
                XmlDocument reader = new XmlDocument();
                reader.Load(xml); //Carregando o arquivo

                #region Console Options
                XmlNodeList xnList = reader.GetElementsByTagName("Console");
                foreach (XmlNode node in xnList)
                    foreach (XmlNode child in node.ChildNodes)
                        switch (child.Name)
                        {
                            case "Visible":
                                bool visible = Convert.ToBoolean(child.InnerText);
                                consoleToolStripMenuItem.Checked = visible;
                                if (!visible)
                                {
                                    Program.HideConsoleWindow();
                                    consoleToolStripMenuItem.DropDown.Enabled = false;
                                }
                                break;

                            case "Read-Write_Operations":
                                readWriteToolStripMenuItem.Checked = Convert.ToBoolean(child.InnerText);
                                break;

                            case "System_Messages":
                                systemToolStripMenuItem1.Checked = Convert.ToBoolean(child.InnerText);
                                break;
                        }
                #endregion
                #region View Options
                xnList = reader.GetElementsByTagName("View");
                foreach (XmlNode node in xnList)
                    foreach (XmlNode child in node.ChildNodes)
                        switch (child.Name)
                        {
                            case "Animations":
                                bool anim = Convert.ToBoolean(child.InnerText);
                                animationsToolStripMenuItem.Checked = anim;
                                break;
                        }
                #endregion
                #region Options
                xnList = reader.GetElementsByTagName("Options");
                foreach (XmlNode node in xnList)
                    foreach (XmlNode child in node.ChildNodes)
                        switch (child.Name)
                        {
                            case "Default_Texture_Viewer_Editor":
                                DefaultTextureVE = child.InnerText;
                                break;
                        }
                #endregion
                xml.Close();
            }
            else
            {
                CreateConfig();
            }
        }
        void CreateConfig()
        {
            Stream xml = File.CreateText("config.xml").BaseStream;
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("CCS_ASSET_EDITOR_CONFIG");

            #region Config Childs
            #region Console
            XmlElement consoleoptions = doc.CreateElement("Console");

            //Visible
            var visible = doc.CreateElement("Visible");
            visible.AppendChild(doc.CreateTextNode(consoleToolStripMenuItem.Checked.ToString().ToLower()));

            //RW Operations
            var rwop = doc.CreateElement("Read-Write_Operations");
            rwop.AppendChild(doc.CreateTextNode(blocksReadWriteToolStripMenuItem.Checked.ToString().ToLower()));

            //System Messages
            var sysmsg = doc.CreateElement("System_Messages");
            sysmsg.AppendChild(doc.CreateTextNode(systemToolStripMenuItem1.Checked.ToString().ToLower()));

            consoleoptions.AppendChild(visible);
            consoleoptions.AppendChild(rwop);
            consoleoptions.AppendChild(sysmsg);
            #endregion

            #region View Options
            XmlElement viewopt = doc.CreateElement("View");

            //Animations
            var anim = doc.CreateElement("Animations");
            anim.AppendChild(doc.CreateTextNode(Animations.ToString().ToLower()));

            viewopt.AppendChild(anim);
            #endregion

            #region Options
            XmlElement opt = doc.CreateElement("Options");

            //Default Texture Viewer/Editor
            var defaulttexture = doc.CreateElement("Default_Texture_Viewer_Editor");
            defaulttexture.AppendChild(doc.CreateTextNode(DefaultTextureVE));

            opt.AppendChild(defaulttexture);
            #endregion

            root.AppendChild(consoleoptions);//Console Options
            root.AppendChild(viewopt);//View Options
            root.AppendChild(opt);//Options
            #endregion

            doc.AppendChild(root);
            doc.Save(xml);
            xml.Close();
        }
        public void RefreshControls()
        {
            saveToolStripMenuItem.Enabled = !saveToolStripMenuItem.Enabled;
            saveAllToolStripMenuItem.Enabled = !saveAllToolStripMenuItem.Enabled;
            closeToolStripMenuItem.Enabled = !closeToolStripMenuItem.Enabled;
            closeAllToolStripMenuItem.Enabled = !closeAllToolStripMenuItem.Enabled;
            extractAllToolStripMenuItem.Enabled = !extractAllToolStripMenuItem.Enabled;
            saveAsToolStripMenuItem.Enabled = !saveAsToolStripMenuItem.Enabled;
            addToolStripMenuItem.Enabled = !addToolStripMenuItem.Enabled;
            resourceTreeToolStripMenuItem.Enabled = !resourceTreeToolStripMenuItem.Enabled;
            frameModeSceneToolStripMenuItem.Enabled =!frameModeSceneToolStripMenuItem.Enabled;
            tabControl1.Visible = !tabControl1.Visible;
            tableLayoutPanel1.Visible = !tableLayoutPanel1.Visible;
            pictureBox2.Visible = !pictureBox2.Visible;
        }
        async void Open(bool drag=false, string[] filenames = null)
        {
            oldPIC2size = pictureBox2.Size;
            oldPIC2loc = pictureBox2.Location;
            if (drag==true)
            {
                if (Animations)
                {
                    pictureBox1.Image = Properties.Resources.cf5586409dc3f2b30b9dba080dfb5c2e4a9b7cfe_00;
                    pictureBox2.Image = Properties.Resources.sasuke_demonio_gif_9;
                    pictureBox2.BackColor = SystemColors.Control;
                    pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox2.Dock = DockStyle.Fill;
                }
                commandLBL.Text = "Opening file(s)...";
                opLBL.Visible = true;
                progressBar1.Visible = true;
                progressBar1.Step = Convert.ToInt32((decimal)100 / filenames.Length);
                progressBar1.Value = Convert.ToInt32((decimal)(100 / filenames.Length));
                foreach (var file in filenames)
                {
                    opLBL.Text = Path.GetFileName(file);
                    CCSF fileccs = null;
                    await Task.Run(() =>
                    {
                        fileccs = new CCSF(file, readWriteToolStripMenuItem.Checked && consoleToolStripMenuItem.Checked);

                    });
                    var tab = new CCSTab(fileccs);
                    tabControl1.TabPages.Add(tab);
                    progressBar1.PerformStep();
                }
            }
            else
            {
                var open = new OpenFileDialog();
                open.Multiselect = true;
                open.Filter = "CyberConnectSystemFile(*.ccs,.tmp)|*.ccs;*.tmp";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    if (Animations)
                    {
                        pictureBox1.Image = Properties.Resources.cf5586409dc3f2b30b9dba080dfb5c2e4a9b7cfe_00;

                        pictureBox2.Image = Properties.Resources.YhSG;
                        pictureBox2.BackColor = SystemColors.Control;
                        pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                        pictureBox2.Dock = DockStyle.Fill;
                    }
                    commandLBL.Text = "Opening file(s)...";
                    opLBL.Visible = true;
                    progressBar1.Visible = true;
                    progressBar1.Step = Convert.ToInt32((decimal)(100 / open.FileNames.Length));
                    progressBar1.Value = Convert.ToInt32((decimal)(100 / open.FileNames.Length));
                    foreach (string file in open.FileNames)
                    {
                        opLBL.Text = Path.GetFileName(file);

                        CCSF fileccs = null;
                        await Task.Run(() =>
                        {
                            fileccs = new CCSF(file, readWriteToolStripMenuItem.Checked && consoleToolStripMenuItem.Checked);
                            
                        });
                        var tab = new CCSTab(fileccs);
                        tabControl1.TabPages.Add(tab);
                        progressBar1.PerformStep();
                    }
                    
                }
            }

            if (tabControl1.TabCount > 0 && closeAllToolStripMenuItem.Enabled == false)
                RefreshControls();


            opLBL.Visible = false;
            progressBar1.Visible = false;
            progressBar1.Value = 0;
            commandLBL.Text = "Open one or multiple CCS/TMP or drag'n drop them.";
            pictureBox1.Image = Properties.Resources.sigma_another_tale_22859624_210820211143;

            pictureBox2.Dock = DockStyle.None;
            pictureBox2.Size = oldPIC2size;
            pictureBox2.Location = oldPIC2loc;
            pictureBox2.BackColor = Color.Transparent;
            pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox2.Image = Properties.Resources.damusx8_f0941559_bbf9_4edc_825b_08674ca1bfe4;
        }
        public void Salvar()
        {
            var selected = GetSelectedTab();
            File.WriteAllBytes(selected.CCSFile.File_path, selected.CCSFile.Rebuild(selected.frameView.Visible ? selected.frameView :
                selected.resourceView.Visible ? selected.resourceView : null, selected.resourceView.Visible));
            MessageBox.Show("Salvo com sucesso!!", "Sistema", MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
        public void SalvarComo()
        {
            var save = new SaveFileDialog();
            save.Filter = "CyberConnectSystemFile(*.ccs,.tmp)|*.ccs;*.tmp";
            if (!GetSelectedTab().CCSFile.isGziped)
                save.DefaultExt = ".tmp";
            save.Title = "Escolha onde quer salvar o arquivo:";
            save.FileName = GetSelectedTab().CCSFile.Name;
            if (save.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(save.FileName, GetSelectedTab().CCSFile.Rebuild(GetSelectedTab().frameView.Visible ? GetSelectedTab().frameView : null));
                MessageBox.Show("Salvo com sucesso!!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        void ReplaceOne()
        {
            var ccstab = (tabControl1.SelectedTab as CCSTab);
            var open = new OpenFileDialog();
            string openpath = "";
            bool replaced = false;
            if (ccstab.CCSFile.CCS_TOC.isFrameScene && ccstab.frameView.Visible)
            {
                open.Filter = $"CCSF Block Array File (*.bin)|*.bin";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    openpath = open.FileName;

                    byte[] blocks = File.ReadAllBytes(open.FileName);
                    for(int i=0;i<blocks.Length;i++)
                    {
                        string framen = Encoding.Default.GetString(blocks.TakeWhile(x => x != 0x3e).ToArray());
                        int framindx = Convert.ToInt32(framen.Substring(7, framen.Length - 7));
                        blocks = blocks.Skip(framen.Length).ToArray();
                        while (Encoding.Default.GetString(blocks.TakeWhile(x => x != 0x3e).ToArray()).StartsWith("<Block"))
                        {
                            string blocken = Encoding.Default.GetString(blocks.TakeWhile(x => x != 0x3e).ToArray());
                            int blockindx = Convert.ToInt32(blocken.Substring(7, blocken.Length - 7));
                            blocks = blocks.Skip(framen.Length).ToArray();

                            Block replace = new Block().ReadBlock(new MemoryStream(blocks));

                            (ccstab.frameView.Nodes[framindx] as CCSNode).FrameBlocks[blockindx] = replace;
                            (ccstab.frameView.Nodes[framindx].Nodes[blockindx] as CCSNode).Block = replace;
                        }

                    }
                }
            }
            else if (ccstab.resourceView.Visible)
                switch (ccstab.resourceView.SelectedNode.Level)
                {
                    case 0: //File
                        var file = ccstab.GetSelectedFile();
                        string fext = file.FileName.Substring(file.FileName.Length - 4, 4);

                        fext = fext == ".bmp" ? ".png" : fext;

                        open.Filter = $"CCSF Internal File (*{fext})|*{fext}";
                        if (open.ShowDialog() == DialogResult.OK)
                        {
                            replaced = file.Replace(open.FileName);
                            openpath = open.FileName;
                        }
                        break;
                    case 2: //Block
                        var ccsnode = (ccstab.resourceView.SelectedNode as CCSNode);
                        fext = ".bin";
                        open.Filter = $"Binary CCSF Block Data (*{fext})|*{fext}";
                        if (open.ShowDialog() == DialogResult.OK)
                        {
                            openpath = open.FileName;
                            var replaceBlock = Block.ReadAllBlocks(File.OpenRead(open.FileName), false, false
                                 , ccstab.CCSFile.CCS_Header, true);
                            for (int i = 0; i < ccstab.CCSFile.Blocks.Count; i++)
                            {
                                if (ccstab.CCSFile.Blocks[i] == ccsnode.Block)
                                {
                                    if (replaceBlock[0].ObjectID != ccstab.CCSFile.Blocks[i].ObjectID)
                                    {
                                        MessageBox.Show("Different Object Block identified, error!!");
                                        return;
                                    }
                                    ccstab.CCSFile.Blocks[i] = replaceBlock[0];
                                    
                                }
                            }

                            ccsnode.Block = replaceBlock[0];
                            replaced = true;
                        }
                        break;
                }

            if (openpath != "" && replaced)
                MessageBox.Show($"Replaced sucessfully from: {openpath}.", "Import");
        }
        void ExtractOne()
        {
            var ccstab = (tabControl1.SelectedTab as CCSTab);
            
            string savepath = "";
            if (ccstab.CCSFile.CCS_TOC.isFrameScene && ccstab.frameView.Visible)
            {
                var check = ccstab.GetCheckedNodes(ccstab.frameView);
                if (check.Count() > 0)
                {
                    List<CCSNode> frames = new List<CCSNode>();
                    foreach (CCSNode node in check)
                        if(node.Level==1)
                            frames.Add((node.Parent as CCSNode));

                    frames = frames.Distinct().ToList();

                    var savefol = new FolderBrowserDialog();
                    savefol.Description = "Select where you want to save the extracted Frame(s)/Block(s)...";
                    if (savefol.ShowDialog() == DialogResult.OK)
                    {
                        string savePATH = savefol.SelectedPath + $@"/{ccstab.Text}_Frames";
                        foreach (CCSNode frame in frames)
                            if (frame.Nodes.Cast<CCSNode>().Any(x => x.Checked == true))
                            {
                                string framePATH = savePATH + $@"/Frame_{frame.Index}";

                                if (!Directory.Exists(framePATH))
                                    Directory.CreateDirectory(framePATH);

                                framePATH += @"/";

                                foreach (CCSNode block in frame.Nodes.Cast<CCSNode>().Where(
                                    c => c.Checked
                                    ))
                                {
                                    File.WriteAllBytes(framePATH + $"{block.Block.GetBlockType()}_{block.Block.ObjectID}.bin", block.Block.DataArray);
                                }
                            }
                        savepath = savePATH;
                    }
                }
            }
            else if (ccstab.resourceView.Visible)
            {
                var save = new SaveFileDialog();
                switch (ccstab.resourceView.SelectedNode.Level)
                {

                    case 0: //File
                        var file = ccstab.GetSelectedFile();
                        string fext = file.FileName.Substring(file.FileName.Length - 4, 4);

                        fext = fext == ".bmp" ? ".png" : fext;

                        save.FileName = Path.GetFileNameWithoutExtension(file.FileName);
                        save.Filter = $"CCSF Internal File (*{fext})|*{fext}";
                        if (save.ShowDialog() == DialogResult.OK)
                        {
                            File.WriteAllBytes(save.FileName, file.GetFile(fext == ".png" ? true : false));
                            savepath = save.FileName;
                        }

                        break;
                    case 2: //Block
                        file = ccstab.GetSelectedFile();
                        fext = ".bin";

                        save.FileName = ccstab.resourceView.SelectedNode.Text + $"_{(ccstab.resourceView.SelectedNode as CCSNode).Block.ObjectID}";
                        save.Filter = $"Binary CCSF Block Data (*{fext})|*{fext}";
                        if (save.ShowDialog() == DialogResult.OK)
                        {
                            File.WriteAllBytes(save.FileName, (ccstab.resourceView.SelectedNode as CCSNode).Block.DataArray);
                            savepath = save.FileName;
                        }
                        break;
                }
            }

            if (savepath != "")
                MessageBox.Show($"Saved sucessfully to: {savepath}.","Export");
        }
        void Fechar(bool all=false)
        {
            if (all)
            {
                tabControl1.TabPages.Clear();
                RefreshControls();
            }
            else
            {
                tabControl1.TabPages.Remove(tabControl1.SelectedTab);
                if(tabControl1.TabCount<=0)
                    RefreshControls();
            }
        }

        #endregion
        #region Events and Buttons
        private void abrirToolStripMenuItem_Click(object sender, EventArgs e) => 
            Open(false);

        private void fecharClick(object sender, EventArgs e) => 
            Fechar(false);

        private void fecharTodosClick(object sender, EventArgs e) =>
            Fechar(true);
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateConfig();
            Environment.Exit(0);
        }

        private void extractAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var saveFolder = new FolderBrowserDialog();
            if (saveFolder.ShowDialog() == DialogResult.OK)
            {
                GetSelectedTab().CCSFile.ExtractAll(saveFolder.SelectedPath,
                    MessageBox.Show("Want to convert the formats?","Conversion",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question)== DialogResult.Yes ? true : false);
            }
        }

        private void repackAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var inputFolder = new FolderBrowserDialog();
            if(inputFolder.ShowDialog()==DialogResult.OK)
            {
                var savePath = new SaveFileDialog();
                savePath.Filter = "CyberConnectSystemFile(*.ccs,.tmp)|*.ccs;*.tmp";
                savePath.FileName = new DirectoryInfo(inputFolder.SelectedPath).Name;
                if (savePath.ShowDialog() == DialogResult.OK)
                    CCSF.CreateCCSF(inputFolder.SelectedPath, savePath.FileName);
            }
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SalvarComo();
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Salvar();
        }
        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(CCSTab tab in tabControl1.TabPages)
            {
                File.WriteAllBytes(tab.CCSFile.File_path, tab.CCSFile.Rebuild());
            }
            MessageBox.Show("Salvo(s) com sucesso!!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void extractToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ExtractOne();
        }
        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReplaceOne();
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var about = new Sobre();
            about.ShowDialog();
            MessageBox.Show("oi", "ola");
        }
        private void convertToI8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (GetSelectedTab().GetSelectedFile().Objects.Where(x => x.ObjectName.StartsWith("TEX"))
                .ToArray()[0].Blocks[0] as Texture).TextureType = Texture.TEXType.I8;
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedTab();
            if (selected.frameView.Visible)
            {
                foreach (CCSNode node in selected.GetCheckedNodes(selected.frameView))
                {
                    if (node.Level == 0)
                    {
                        node.Remove();
                        if (node.Text != "Resources")
                            (GetSelectedTab().CCSFile.Blocks[0] as Header).FrameCounter--;
                    }
                    else if (node.Level == 1)
                    {
                        if (node.Parent.Text != "Resources")
                            (node.Parent as CCSNode).FrameBlocks.Remove(node.Block);
                        node.Remove();
                    }
                }
            }
            else if (selected.resourceView.Visible)
            {
                if (selected.resourceView.SelectedNode.Level == 2)//Block level
                {
                    selected.CCSFile.Blocks.Remove((selected.resourceView.SelectedNode as CCSNode)
                        .Block);
                    selected.resourceView.SelectedNode.Remove();
                }
            }
        }

        private void resourceTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedTab = GetSelectedTab();
            selectedTab.resourceView.Visible = !selectedTab.resourceView.Visible;
            selectedTab.frameView.Visible = !selectedTab.frameView.Visible;
            resourceTreeToolStripMenuItem.Checked = !resourceTreeToolStripMenuItem.Checked;
            frameModeSceneToolStripMenuItem.Checked = !frameModeSceneToolStripMenuItem.Checked;

            if (!frameModeSceneToolStripMenuItem.Checked)
            {
                selectedTab.resourceViewbt.Text = "Frame Mode (Scene)";
            }
            else
            {
                selectedTab.resourceViewbt.Text = "Resource Mode";
            }
            

        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedTab();
            var k = selected.GetCheckedNodes(selected.frameView);
            if (k.Length > 0)
                selected.CopyNodes();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedTab();
            if (selected.Copy.Length > 0)
                selected.PasteNodes();
        }

        private void propriedadesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tab = GetSelectedTab();
            if (tab.frameView.Visible)
                foreach (var check in tab.GetCheckedNodes(tab.frameView))
                    new PropertyBox(check, tab).Show();
            else if (tab.resourceView.Visible)
                new PropertyBox(tab.resourceView.SelectedNode as CCSNode, tab).Show();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.TabCount > 0)
            {
                var k = new List<string>();
                foreach (var file in GetSelectedTab().CCSFile.CCS_TOC.Files)
                    foreach (var objectx in file.Objects)
                        k.Add(objectx.ObjectName);
                CustomTypeConverter.Names = k.ToArray();

                if (GetSelectedTab().frameView.Visible)
                {
                    frameModeSceneToolStripMenuItem.Checked = true;
                    resourceTreeToolStripMenuItem.Checked = false;
                }
                else
                {
                    frameModeSceneToolStripMenuItem.Checked = false;
                    resourceTreeToolStripMenuItem.Checked = true;
                }
            }
        }

        private void searchAndCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sc = new SearchCheck();
            sc.Show();
        }

        private void uncheckAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tab = GetSelectedTab();
            if (tab.GetCheckedNodes(tab.frameView).Length > 0)
                tab.UncheckNodes(tab.frameView);
        }

        private void frameModeSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedTab = GetSelectedTab();
            selectedTab.resourceView.Visible = !selectedTab.resourceView.Visible;
            selectedTab.frameView.Visible = !selectedTab.frameView.Visible;
            resourceTreeToolStripMenuItem.Checked = !resourceTreeToolStripMenuItem.Checked;
            frameModeSceneToolStripMenuItem.Checked = !frameModeSceneToolStripMenuItem.Checked;

            if (!frameModeSceneToolStripMenuItem.Checked)
            {
                selectedTab.resourceViewbt.Text = "Frame Mode (Scene)";
            }
            else
            {
                selectedTab.resourceViewbt.Text = "Resource Mode";
            }

        }

        private void Principal_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
                copyToolStripMenuItem.PerformClick();
            if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
                pasteToolStripMenuItem.PerformClick();
        }

        private void consoleToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if (consoleToolStripMenuItem.Checked)
            {
                Program.ShowConsoleWindow();
                consoleToolStripMenuItem.DropDown.Enabled = true;
            }
            else
            {
                Program.HideConsoleWindow();
                consoleToolStripMenuItem.DropDown.Enabled = false;

            }
        }

        private void Principal_FormClosing(object sender, FormClosingEventArgs e)
        {
            CreateConfig();
        }
        private void animationsToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            Animations = animationsToolStripMenuItem.Checked;
            pictureBox1.Enabled = Animations;
            pictureBox2.Enabled = Animations;
        }
        private void clearConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Console.Clear();
        }
        #region Drag'n Drop
        private void Principal_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void Principal_DragDrop(object sender, DragEventArgs e)
        {
            var dropped = (string[])e.Data.GetData(DataFormats.FileDrop);
            Open(true, dropped);
        }


        #endregion

        #endregion

        private void setTextureViewerEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var open = new OpenFileDialog();
            open.Filter = "Windows Program(*.exe)|*.exe";
            if(open.ShowDialog()==DialogResult.OK)
            {
                DefaultTextureVE = open.FileName;
            }
        }
    }
}
