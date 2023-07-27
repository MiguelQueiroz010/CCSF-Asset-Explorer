using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

public class CCSF
{
    internal bool isGziped = false;
    internal Stream InternalFile;

    public Header CCS_Header;
    public Index CCS_TOC;

    public string Name => new String(CCS_Header.FileName.TakeWhile(x => x != '\0').ToArray());
    public string File_path;
    public Header.CCSFVersion Version => CCS_Header.Version;

    public List<Block> Blocks;

    [Category("CCSF")]
    [DisplayName("CCSF Name")]
    [Description("CCS File Name.")]
    public string _name
    {
        get => Name;
    }
    [Category("CCSF")]
    [DisplayName("Gziped")]
    [Description("CCS is gziped?.")]
    public bool _isgzip
    {
        get => isGziped;
    }
    [Category("CCSF")]
    [DisplayName("Type")]
    [Description("CCSF type [Resource/Scene]")]
    public string _type
    {
        get => CCS_TOC.isFrameScene ? "Scene" : "Resource";
    }
    [Category("CCSF")]
    [DisplayName("Blocks")]
    [Description("All file blocks.")]
    public List<Block> _blocks
    {
        get => Blocks;
        set => Blocks = value;
    }

    public CCSF(string FilePath, bool logBlockReadWrite = false)
    {
        File_path = FilePath;

        byte[] InternalData = File.ReadAllBytes(FilePath);
        if (FileHelper.isGzipMagic(InternalData))
        {
            isGziped = true;
            InternalData = FileHelper.unzipArray(InternalData);
        }

        InternalFile = new MemoryStream(InternalData);

        //Read Blocks
        Blocks = Block.ReadAllBlocks(InternalFile, logBlockReadWrite, false, null, false, this);

        //Init Header and Toc Index sections
        CCS_Header = Blocks[0] as Header;
        CCS_TOC = Blocks[1] as Index;

        //Set Linked blocks on TOC
        foreach (var file in CCS_TOC.Files)
            foreach (var obj in file.Objects)
                obj.SetLinkedBlocks(Blocks.ToArray());


        //Set Linked submodels objname and CCSF
        foreach (var file in CCS_TOC.Files)
            foreach (var obj in file.Objects)
                foreach (var block in obj.Blocks)
                    if (block.Type == 0xcccc0800)
                    {
                        var mdl = block as Model;
                        if (mdl.SubModelCount > 0)
                            foreach (var submdl in mdl.SubModels)
                                foreach (var filex in CCS_TOC.Files)
                                    foreach (var objx in file.Objects)
                                        if (submdl.ObjectID != 0xFFFFFFFF)
                                            if (objx.Index == submdl.ObjectID && !submdl.useclumpref)
                                            {
                                                submdl.ObjectName = objx.ObjectName;
                                            }
                                            else if (submdl.useclumpref)
                                            {
                                                Clump refcmp = null;
                                                try
                                                {
                                                    foreach (var xobj in file.Objects)
                                                        if (xobj.ObjectName.StartsWith("CMP"))
                                                            refcmp = xobj.Blocks.Where(x => x.BlockType == "Clump").Where
                                                            (c => (c as Clump).Nodes.Any(n => n.ID == submdl.ObjectID)).ToArray()[0] as Clump
                                                            ;


                                                    if (refcmp != null)
                                                    {
                                                        submdl.cmpRef = refcmp;
                                                        submdl.ObjectName = refcmp.Nodes[submdl.ObjectID]._oname;
                                                    }
                                                }
                                                catch (Exception) { }
                                            }
                    }



        //Set frame type
        foreach (var file in CCS_TOC.Files)
            foreach (var obj in file.Objects)
                if (obj.Blocks.Any(x => x.Type == 0xcccc0005))
                    CCS_TOC.isFrameScene = false;
    }
    internal int IndexOf(Block block) => Blocks.IndexOf(block);

    internal void ToTreeView(TreeView treeView, bool forceResource = false, bool forceFrame = false)
    {
        treeView.BeginUpdate();
        if (CCS_TOC.isFrameScene && !forceResource || forceFrame)
        {
            //Get Resources
            var resourcenode = new CCSNode("Resources");
            foreach (var block in Blocks.TakeWhile(x => x.Type != 0xccccff01).Skip(2))
                resourcenode.Nodes.Add(new CCSNode(block.GetBlockType())
                {
                    Block = block,
                    Object = CCS_TOC.GetObject(block)
                });
            treeView.Nodes.Add(resourcenode);

            //Get frame groups
            int framec = 0;
            var frame = new CCSNode();
            var framegroup = new List<Block>();
            foreach (var block in Blocks.SkipWhile(x => x.Type != 0xccccff01))
            {
                if (block.Type == 0xccccff01)
                {
                    if (frame.Text != "")
                    {
                        frame.FrameBlocks = framegroup;

                        treeView.Nodes.Add(frame);
                    }
                    frame = new CCSNode("Frame Group");
                    framegroup = new List<Block>();
                    frame.Block = block;
                    frame.Object = CCS_TOC.GetObject(block);
                    framec++;
                }
                else
                {
                    framegroup.Add(block);
                    frame.Nodes.Add(new CCSNode(block.GetBlockType()) { Block = block });
                }
            }

        }
        else if (!CCS_TOC.isFrameScene || forceResource)
            foreach (var file in CCS_TOC.Files)
            {
                if (file.FileName != @"%\")
                {
                    var fileNode = new CCSNode(file.FileName);
                    fileNode.File = file;
                    foreach (var obj in file.Objects)
                    {
                        var objNode = new CCSNode(obj.ObjectName);
                        objNode.Object = obj;
                        foreach (var block in obj.Blocks)
                        {
                            objNode.Nodes.Add(new CCSNode(block.GetBlockType())
                            {
                                Block = block
                            });

                        }
                        fileNode.Nodes.Add(objNode);

                    }
                    treeView.Nodes.Add(fileNode);
                }
                else
                {
                    foreach (var obj in file.Objects)
                    {
                        var objNode = new CCSNode(obj.ObjectName);
                        objNode.Object = obj;
                        foreach (var block in obj.Blocks)
                            objNode.Nodes.Add(new CCSNode(block.GetBlockType())
                            {
                                Block = block
                            });
                        treeView.Nodes.Add(objNode);
                    }

                }

            }
        treeView.EndUpdate();
    }

    internal byte[] Rebuild(TreeView treev = null, bool resource = true)
    {
        var result = new List<byte>();

        if (CCS_TOC.isFrameScene && !resource)
        {
            result.AddRange(Blocks[0].ToArray());//Header
            result.AddRange(Blocks[1].Data);//TOC

            //Resources
            foreach (CCSNode resourceBlock in treev.Nodes[0].Nodes)
                result.AddRange(resourceBlock.Block.DataArray);

            //FrameGroups
            int f = 0;
            foreach (CCSNode frameGroup in treev.Nodes)
            {
                if (f > 0)
                {
                    result.AddRange(new Block(0xCCCCFF01, (uint)(f - 1)).Data);//FrameID
                    foreach (var block in frameGroup.FrameBlocks)//Data Blocks
                        result.AddRange(block.DataArray);
                }
                f++;
            }

            result.AddRange(new Block(0xCCCCFF01, 0xFFFFFFFF).Data);//FrameEND
        }
        else if (resource)
        {
            ////GetData
            ///
            Blocks[1].Data = CCS_TOC.ToArray();
            //foreach (var block in Blocks)
            //        result.AddRange(block.DataArray);

            #region Files Alg
            //Header and TOC
            result.AddRange(CCS_Header.DataArray);
            result.AddRange(CCS_TOC.ToArray(out var objectEntries));

            var subblocks = Blocks.Skip(2);

            //Set Indexes
            foreach (var block in subblocks)
            {
                foreach (var rootobj in objectEntries)
                    foreach (var blockRoot in rootobj.Blocks)
                    {
                        if (blockRoot == block)
                        {
                            if (rootobj.FileIndex == 0)
                            {
                                rootobj.Index = 0;
                                rootobj.WMDLIndex = 0;
                            }
                            block.SetIndexes(rootobj, objectEntries.ToArray());

                        }
                    }
                result.AddRange(block.DataArray);
            }



            #endregion
        }

        byte[] res = result.ToArray();
        return isGziped ? FileHelper.zipArray(res, Name + ".tmp") : res;
    }

    internal void ExtractAll(string savePath, bool Convert)
    {
        Console.WriteLine("Iniciando extração...");

        string savepath = savePath + @"\Extracted\" + Name.Split(new string[] { "\0" }, StringSplitOptions.RemoveEmptyEntries)[0];
        Console.WriteLine($"Salvando aqui: {savepath}");

        if (!Directory.Exists(savepath))
            Directory.CreateDirectory(savepath);

        savepath += @"\";

        Console.WriteLine($"Salvando aqui: {savepath}");

        //INI Setup for Recreation
        var SetupCCSF = new StringBuilder();
        SetupCCSF.Append("CCSF Setup File\r\n" +
            "---------------------\r\n\r\n" +
            $"Version: {CCS_Header.Version}\r\n\r\n" +
            "-------------------------------\r\n");


        foreach (var file in CCS_TOC.Files)
        {
            if (file.FileName != @"%\")
            {
                foreach (var obj in file.Objects)
                    SetupCCSF.Append($"\r\nIDX: {obj.Index} : {obj.ObjectName}");

                Console.WriteLine($"Extraindo o arquivo: {file.FileName.Split(new string[] { "\0" }, StringSplitOptions.RemoveEmptyEntries)[0]}");

                if (!Directory.Exists(savepath + Path.GetDirectoryName(file.FileName.Split(new string[] { "\0" }, StringSplitOptions.RemoveEmptyEntries)[0])))
                    Directory.CreateDirectory(savepath + Path.GetDirectoryName(file.FileName.Split(new string[] { "\0" }, StringSplitOptions.RemoveEmptyEntries)[0]));

                Console.WriteLine($"Salvando: {savepath + Path.GetDirectoryName(file.FileName.Split(new string[] { "\0" }, StringSplitOptions.RemoveEmptyEntries)[0])}");

                string fname = file.FileName.Split(new string[] { "\0" }, StringSplitOptions.RemoveEmptyEntries)[0];

                if (fname.Contains("ps2dev") && fname.EndsWith(@"\"))
                    fname += "script.bin";


                //Trocas em tipos de arquivo
                switch (file.Ftype)
                {
                    case FileType.Bitmap:
                        if (Convert == true)
                            fname = fname.Replace(".bmp", ".png");
                        break;
                    case FileType.Link:
                        fname += "#";
                        break;
                }

                File.WriteAllBytes(savepath + fname, file.GetFile(Convert));
            }
            else
            {
                foreach (var obj in file.Objects)
                    SetupCCSF.Append($"\r\nIDX: {obj.Index} : {obj.ObjectName}");

                Console.WriteLine($"Extraindo o arquivo: {file.FileName.Split(new string[] { "\0" }, StringSplitOptions.RemoveEmptyEntries)[0]}");



                Console.WriteLine($"Salvando: {savepath + Path.GetDirectoryName(file.FileName.Split(new string[] { "\0" }, StringSplitOptions.RemoveEmptyEntries)[0])}");

                File.WriteAllBytes(savepath + "root.bin", file.GetFile(Convert));
            }
        }
        File.WriteAllText(savepath + "setup.ini", SetupCCSF.ToString());
        Console.WriteLine("Extraído com sucesso!");
    }

    public static void CreateCCSF(string inputFolder, string savePath)
    {
        Console.Clear();
        Console.WriteLine("Criando CCSF...");
        Console.WriteLine("Computando arquivos:");

        //Setup
        string SetupIni = File.ReadAllText(inputFolder + @"\setup.ini");
        string[] SetupLines = SetupIni.Split(new string[] { "\r\n", "---------------------",
        "-------------------------------"}, StringSplitOptions.RemoveEmptyEntries);
        string[] ObjectList = SetupLines.Skip(3).ToArray();


        #region Build CCSF
        //HEADER - TOC
        var version = SetupLines[1].Split(new string[] {
            "Version: "
        }, StringSplitOptions.RemoveEmptyEntries);
        var dirinfo = new DirectoryInfo(inputFolder);

        Header header = new Header(dirinfo.Name, version[0].GetVersion());
        Index toc = new Index(inputFolder, out byte[] Blocks, header);

        //Merge ALL
        var ccsf = new List<byte>();
        ccsf.AddRange(header.ToArray());
        ccsf.AddRange(toc.ToArray());

        ccsf.AddRange(Blocks);

        //Is frame playable Scene
        if (!toc.isFrameScene)
            ccsf.AddRange(new Block(0xCCCC0005, 1).ToArray());
        //END FILE BLOCK
        ccsf.AddRange(new Block(0xCCCCFF01, 0xFFFFFFFF).ToArray());

        //SAVE
        File.WriteAllBytes(savePath, ccsf.ToArray());
        #endregion
    }

    internal bool Closed() => CCS_Header == null ? true :
        CCS_TOC == null ? true : Blocks == null ? true : false;

    internal void Close()
    {
        InternalFile.Close();
        CCS_Header = null;
        CCS_TOC = null;
        Blocks = null;
    }

}
