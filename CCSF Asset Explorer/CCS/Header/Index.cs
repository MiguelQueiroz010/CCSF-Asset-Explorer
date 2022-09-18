using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using static IOextent;
using Rainbow.ImgLib.Encoding;
using Rainbow.ImgLib.Common;
using Rainbow.ImgLib;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using OpenTK;

public enum FileType
{
    Bitmap,
    Binary,
    Max,
    CCpart,
    Link,
    Unknow
}

public class FileEntry
{
    public string FileName;
    public int Index;
    public bool Link = false;
    public FileType Ftype;
    public ObjectEntry[] Objects;
    [Category("File")]
    [DisplayName("File Name")]
    [Description("File Name.")]
    public string _name
    {
        get => FileName;
    }
    [Category("File")]
    [DisplayName("File Index")]
    [Description("File Index.")]
    public int _index
    {
        get => Index;
    }
    [Category("File")]
    [DisplayName("File Objects")]
    [Description("File Objects.")]
    public ObjectEntry[] _objects
    {
        get => Objects;
        set => Objects = value;
    }
    public ObjectEntry GetObject(string name) => Objects.Where(x => x.ObjectName == name).ToArray()[0];
    public ObjectEntry[] GetObjects(string startswith) => Objects.Where(x => x.ObjectName.StartsWith(startswith)).ToArray();
    

    public byte[] ImageToByteArray(Image imageIn)
    {
        imageIn.Save("temp", ImageFormat.Png);
        return File.ReadAllBytes("temp");
    }
    public byte[] GetFile(bool Convert = false, bool root = false)
    {
        var FileData = new List<byte>();
        if(Convert==true)
            switch (Ftype) 
            {
                #region Textures
                case FileType.Bitmap:
                    CLUT CLUT = null;
                    Texture TEX = Objects.Where(x => x.ObjectName.StartsWith("TEX")).ToArray()[0].Blocks[0] as Texture;
                    ObjectEntry[] objCLUTs = Objects.Where(x => x.ObjectName.StartsWith("CLT")).ToArray();
                    foreach (var obj in objCLUTs)
                    {
                        if(obj.Blocks[0].ObjectID==TEX.CLUTID)
                            CLUT = obj.Blocks[0] as CLUT;
                    }

                    if (CLUT != null)
                    { 
                            var bitmap = TEX.ToBitmap(CLUT);
                            FileData.AddRange(ImageToByteArray(bitmap));
                        }
                    break;
                #endregion
                default:
                    FileData.AddRange(Encoding.Default.GetBytes("FOBJ"));
                    FileData.AddRange(BitConverter.GetBytes((UInt32)Objects.Length));
                    foreach (var obj in Objects)
                    {
                        //Object Name
                        byte[] name = new byte[0x20];
                        byte[] namex = Encoding.Default.GetBytes("[OBJ]"+obj.ObjectName);
                        Array.Copy(namex, name, namex.Length);
                        FileData.AddRange(name);

                        //Data blocks count
                        if (obj.Blocks != null && obj.Blocks.Count() > 0)
                            FileData.AddRange(BitConverter.GetBytes((UInt32)obj.Blocks.Length));
                        else
                            FileData.AddRange(BitConverter.GetBytes((UInt32)0));

                        //Data blocks
                        if (obj.Blocks != null && obj.Blocks.Count() > 0)
                            foreach (var block in obj.Blocks)
                            {
                                FileData.AddRange(block.Data);
                                FileData.AddRange(Encoding.Default.GetBytes("<BL>"));
                            }

                    }
                    break;
            }
        else
        {
            FileData.AddRange(Encoding.Default.GetBytes("FOBJ"));
            FileData.AddRange(BitConverter.GetBytes((UInt32)Objects.Length));
            foreach (var obj in Objects)
            {
                //Object Name
                byte[] name = new byte[0x20];
                byte[] namex = Encoding.Default.GetBytes("[OBJ]" + obj.ObjectName);
                Array.Copy(namex, name, namex.Length);
                FileData.AddRange(name);

                //Data blocks count
                if (obj.Blocks != null && obj.Blocks.Count() > 0)
                    FileData.AddRange(BitConverter.GetBytes((UInt32)obj.Blocks.Length));
                else
                    FileData.AddRange(BitConverter.GetBytes((UInt32)0));

                //Data blocks
                if (obj.Blocks != null && obj.Blocks.Count() > 0)
                    foreach (var block in obj.Blocks.Distinct())
                    {
                        FileData.AddRange(block.Data);
                        FileData.AddRange(Encoding.Default.GetBytes("<BL>"));
                    }

            }
        }
        return FileData.ToArray();
    }
    
    public bool Replace(string freplace)
    {
        switch(Ftype)
        {
            case FileType.Bitmap:
                var obj = Objects.Where(x => x.ObjectName.StartsWith("TEX")).ToArray()
                    [0];
                var cobj = Objects.Where(x => x.ObjectName.StartsWith("CLT")).ToArray()
                    [0];
                Texture texture = obj.Blocks.Where(b => b.Type == 0xcccc0300).ToArray()[0] as Texture;
                CLUT clut = cobj.Blocks.Where(c => c.Type == 0xcccc0400).ToArray()[0] as CLUT;

                if (texture.SetfromPNG(Image.FromFile(freplace), out byte[] CLT))
                {
                    if (texture.TextureType == Texture.TEXType.I4 || texture.TextureType == Texture.TEXType.I8)
                    {
                        clut.Palette = Enumerable.Range(0, (int)(CLT.Length / 4)).Select(
                    x => CLUT.ReadColor(CLT.ReadBytes((int)(x * 4), 4)
                    , false)).ToArray();
                        clut.Data = clut.ToArray();
                    }

                    texture.Data = texture.ToArray();
                    return true;
                }
                else
                    return false;

            default:
                return false;
        }
    }
    internal string GetExtension(FileType fileType)
    {
        switch(fileType)
        {
            case FileType.Bitmap:
                return ".png";
            case FileType.Max:
                return ".max";
            case FileType.Binary:
                return ".bin";
            case FileType.CCpart:
                return ".ccpart";
            case FileType.Link:
                return "#";
        }
        return "";
    }
    static FileType GetFileType(string Extension, bool link)
    {
        if (link)
            return FileType.Link;
        switch(Extension.ToLower())
        {
            case ".max":
                return FileType.Max;
            case ".ccpart":
                return FileType.CCpart;
            case ".bmp":
                return FileType.Bitmap;
            case ".bin":
                return FileType.Binary;
            default:
                return FileType.Unknow;
        }
    }
    internal static FileEntry Read(int index, byte[] _fileName, ObjectEntry[] AllObjects) => new FileEntry()
    {
        Index = index+1,
        FileName = _fileName.All(x=> x==0) ? @"%\" : _fileName.TakeWhile(x=>x!= 0).ToArray().ConvertTo(Encoding.Default),
        Objects = AllObjects.Where(x=> index == -1 ? x.FileIndex == 0 : x.FileIndex-1 == index).ToArray(),
        Link = (_fileName.All(x => x == 0) ? @"%\" : _fileName.TakeWhile(x => x != 0).ToArray().ConvertTo(Encoding.Default)).StartsWith("#"),
        Ftype = GetFileType(Path.GetExtension(_fileName.All(x => x == 0) ? @"%\" : _fileName.TakeWhile(x => x != 0).ToArray().ConvertTo(Encoding.Default)),
            (_fileName.All(x => x == 0) ? @"%\" : _fileName.TakeWhile(x => x != 0).ToArray().ConvertTo(Encoding.Default)).StartsWith("#"))
    };
}

public class ObjectEntry
{
    public string ObjectName;
    public Block[] Blocks;
    public uint FileIndex, Index, WMDLIndex;

    [Category("Object")]
    [DisplayName("Object Name")]
    [Description("Object Name.")]
    public string _name
    {
        get => ObjectName;
    }
    [Category("Object")]
    [DisplayName("Object Index")]
    [Description("Object Index.")]
    public uint _index
    {
        get => Index;
    }
    [Category("Object")]
    [DisplayName("Object Blocks")]
    [Description("Object's blocks.")]
    public Block[] _blocks
    {
        get => Blocks;
        set
        {
            Blocks = value;
            
        }
    }
    internal static ObjectEntry Read(byte[] ObjData, uint index)
    {
        var obj = new ObjectEntry();
        obj.ObjectName = new String(ObjData.ReadBytes(0, 0x1e).ConvertTo(Encoding.GetEncoding(932))
            .TakeWhile(x => x != '\0').ToArray());
        obj.FileIndex = ObjData.ReadUInt(0x1e, 16);
        obj.Index = index;
        obj.WMDLIndex = index;
        return obj;
    }
    internal void SetLinkedBlocks(Block[] AllBlocks)
    {
        Blocks = AllBlocks.Where(x => x.ObjectID - 1 == Index).ToArray();
    }
}


public class Index : Block
{
    public bool isFrameScene = true;
    public struct ObjectStream
    {
        public string ObjName;
        public int ObjIndex, FileIndex;
        public List<byte[]> Blocks;

        public static ObjectStream Read(Stream file, int index, int findex)
        {
            var objstr = new ObjectStream();
            objstr.FileIndex = findex;
            objstr.ObjName = new String(file.ReadBytes(0x20).ConvertTo(Encoding.Default).Skip(5).TakeWhile(x => x != '\0').ToArray());
            int blockquant = (int)file.ReadUInt(32);
            objstr.Blocks = file.ReadBlocks(blockquant);
            objstr.ObjIndex = index;
            return objstr;
        }
        public static void SetBlocksInternalIndexes(List<ObjectStream> objs, Header cch, CCSF _ccsf)
        {
            var blocks = new List<byte>();
            foreach (var obj in objs)
                if(obj.Blocks.Count>0)
                    for (int i = 0; i < obj.Blocks.Count; i++)
                        blocks.AddRange(obj.Blocks[i]);
            blocks.AddRange(new Block(0xCCCCFF01, 0xFFFFFFFF).ToArray());

            byte[] allblock = blocks.ToArray();
            File.WriteAllBytes("tt", allblock);

            var Blocks = Block.ReadAllBlocks(new MemoryStream(allblock), true, false, cch, false, _ccsf);

            //Set links
            foreach (var objx in objs)
            {
                switch (objx.ObjName.Substring(0, 3))
                {
                    case "TEX":
                        foreach (var bx in Blocks)
                            for (int k = 0; k < objx.Blocks.Count; k++)
                                if (Array.Equals(bx.Data, objx.Blocks[k]))
                                {
                                    var tex = bx as Texture;
                                    tex.CLUTOBJ = objs.Where(x => x.ObjName.StartsWith("CLT")).Where(c => c.Blocks.Where(r => r.ReadUInt(8, 32) == tex.CLUTID).Count() > 0).ToArray()[0].ObjName;
                                }
                        break;
                    case "MAT":
                        foreach (var bx in Blocks)
                            for (int k = 0; k < objx.Blocks.Count; k++)
                                if (Array.Equals(bx.Data, objx.Blocks[k]))
                                {
                                    var mat = bx as Material;
                                    mat.TextureOBJ = objs.Where(x => x.ObjName.StartsWith("TEX")).Where(c => c.Blocks.Where(r => r.ReadUInt(8, 32) == mat.TextureID).Count() > 0).ToArray()[0].ObjName;
                                }
                        break;
                        
                        //case "MDL":
                        //foreach (var bx in Blocks)
                        //    for (int k = 0; k < objx.Blocks.Count; k++)
                        //        if (Array.Equals(bx.Data, objx.Blocks[k]))
                        //        {
                        //            var mdl = bx as Model;
                        //            for(int s = 0; s < mdl.SubModelCount; s++)
                        //                mdl.SubModels[s].MaterialObjName = objs.Where(x => x.ObjName.StartsWith("MAT")).Where(c => c.Blocks.Where(r => r.ReadUInt(8, 32) == mdl.SubModels[s].MaterialID).Count() > 0).ToArray()[0].ObjName;
                        //        }

                        //break;
                }
                
            }

            //Set indexes
            foreach (var obj in objs)
                foreach (var b in Blocks)
                    for(int k = 0;k<obj.Blocks.Count;k++)
                        if (Array.Equals(b.Data, obj.Blocks[k]))
                        {
                            b.SetIndexes(obj, objs.ToArray());
                            obj.Blocks[k] = b.ToArray();
                        }
        }
    }

    public uint FilesCount, ObjectsCount;

    public FileEntry[] Files;

    public ulong UnkEndData;

    //Rebuild only
    private byte[] FilesNam, ObjectsNam;

    public FileEntry GetFile(string file) => Files.Where(x => x.FileName == file).ToArray()[0];
    public ObjectEntry GetObject(Block block) => Files.Any(f =>
    f.Objects.Any(o =>
    o.Blocks.Any(x => x == block))) ? Files.Where(f =>
    f.Objects.Any(o =>
    o.Blocks.Any(x => x == block))).ToArray()[0].Objects.Where(p => p.Blocks.Any(x => x == block)).ToArray()[0]
        : null;
    public string GetObjectName(uint index)
    {
        foreach (var file in this._ccsToc.Files)
            foreach (var objectx in file.Objects)
                if (objectx.WMDLIndex+1 == index)
                    return objectx.ObjectName;
        return "NO OBJECT";
    }
    public Index() { }

    public Index(string inputFolder, out byte[] blocks, Header cch)
    {
        Type = 0xcccc0002;
        
    //TOC GENERATOR
        var FilesNames = new List<byte>();

        var FileObjs = new List<ObjectStream>();
        var ObjectNames = new List<byte>();

        var Blocks = new List<byte>();

        int FileIndex = 1, ObjectIndex = 1;
        foreach (string file in Directory.EnumerateFiles(inputFolder, "*.*", SearchOption.AllDirectories))
        {
            string absFileName = new String(file.Skip(inputFolder.Length + 1).ToArray());
            string filename = Path.GetFileName(file);
            if (filename != "setup.ini" &&
                filename != "root.bin")
            {
                //Console.WriteLine("SCaminho: " +file);
                Console.WriteLine("Caminho inside: " + absFileName);

                //File stream
                var fileStream = File.Open(file, FileMode.Open);

                //Objects and Data blocks separation
                fileStream.Position = 4;
                uint objCount = fileStream.ReadUInt(32);


                for (int i = 0; i < objCount; i++)
                {
                    FileObjs.Add(ObjectStream.Read(fileStream, ObjectIndex, FileIndex));
                    ObjectIndex++;
                }
                
                //Files Names
                if (absFileName.EndsWith("#"))
                    absFileName = absFileName.Substring(0,absFileName.Length-1);
                if (absFileName.StartsWith("ps2dev2"))
                    absFileName = @"\\" + absFileName.Substring(0,absFileName.Length-10);

                byte[] FName = new byte[0x20];
                Array.Copy(Encoding.Default.GetBytes(absFileName), FName, absFileName.Length);
                FilesNames.AddRange(FName);

                FileIndex++;
            }
            else if(filename == "root.bin")
            {
                //Console.WriteLine("SCaminho: " +file);
                Console.WriteLine("Caminho inside: " + absFileName);

                //File stream
                var fileStream = File.Open(file, FileMode.Open);

                //Objects and Data blocks separation
                fileStream.Position = 4;
                uint objCount = fileStream.ReadUInt(32);


                for (int i = 0; i < objCount; i++)
                {
                    FileObjs.Add(ObjectStream.Read(fileStream, ObjectIndex, 0));
                    ObjectIndex++;
                }
                
            }
        }

        //Blocks indexes
        ObjectStream.SetBlocksInternalIndexes(FileObjs, cch, _ccsf);

        foreach (var obj in FileObjs)
        {
            //Object TOC
            byte[] OName = new byte[0x20];
            Array.Copy(Encoding.Default.GetBytes(obj.ObjName), OName, obj.ObjName.Length);
            Array.Copy(BitConverter.GetBytes((UInt16)obj.FileIndex), 0, OName, 0x1e, 2);
            ObjectNames.AddRange(OName);

            //Blocks Data
            foreach (var b in obj.Blocks)
            {
                if (b.ReadUInt(0, 32) == 0xcccc0005)
                    isFrameScene = false;
                else
                {
                    Blocks.AddRange(b);
                }
            }
        }

        FilesCount = (uint)(FilesNames.Count / 0x20) + 1;
        ObjectsCount = (uint)(ObjectNames.Count / 0x20) + 1;
        FilesNam = FilesNames.ToArray();
        ObjectsNam = ObjectNames.ToArray();
        Size = (uint)(0x48 + FilesNam.Length + ObjectsNam.Length);

        blocks = Blocks.ToArray();
    }

    public override byte[] ToArray()
    {
        var result = new List<byte>();
        result.AddRange(Type.ToLEBE(32));
        result.AddRange((Size / 4).ToLEBE(32));

        result.AddRange(FilesCount.ToLEBE(32));
        result.AddRange(ObjectsCount.ToLEBE(32));

        //Root name space [FILES]
        result.AddRange(new byte[0x20]);
        result.AddRange(FilesNam);

        //Root name space [OBJECTS]
        result.AddRange(new byte[0x20]);
        result.AddRange(ObjectsNam);

        //Strange '3'
        uint three = 3;
        result.AddRange(three.ToLEBE(32));
        result.AddRange(new byte[4]);

        return result.ToArray();
    }

    public override Block ReadBlock(Stream Input)
    {
        Index TOC = new Index();
        TOC.Type = Input.ReadUInt(0, 32);
        TOC.Size = Input.ReadUInt(4, 32) * 4;


        TOC.FilesCount = Input.ReadUInt(8, 32);
        TOC.ObjectsCount = Input.ReadUInt(0xC, 32) - 1;

        var Objects = Enumerable.Range(0, (int)(Input.ReadUInt(0xC, 32) - 1)).Select(
                    o => ObjectEntry.Read(Input.ReadBytes((0x30 + ((int)Input.ReadUInt(8, 32) * 0x20) + (o * 0x20)), 0x20),
                        (uint)o)
                    ).OrderBy(a => !a.ObjectName.StartsWith("CLT")
                    ).ThenBy(a => !a.ObjectName.StartsWith("MAT")
                    ).ThenBy(a => !a.ObjectName.StartsWith("MDL")
                    ).ThenBy(a => !a.ObjectName.StartsWith("OBJ")
                    ).ToArray();
        TOC.Files = Enumerable.Range(-1, (int)(Input.ReadUInt(8, 32))).Select(
            x => FileEntry.Read(x,

                Input.ReadBytes(0x30 + (x * 0x20), 0x20),
                Objects
                )
            ).OrderBy(
            n => n.FileName != @"%\"
            ).ThenBy(
            n => n.Ftype != FileType.Bitmap
).ToArray();

        TOC.UnkEndData = Input.ReadULong((int)(Input.ReadUInt(4, 32) * 4) - 8);
        return TOC;
    }

}