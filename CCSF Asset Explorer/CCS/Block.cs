//#define DEBUGX
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
public class CustomTypeConverter : TypeConverter
{
    public static string[] Names = null;
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    {
        return true;
    }
    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
    {
        return true;
    }
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    { 
        return new StandardValuesCollection(Names);
    }
}
public class Block
{
    public uint Type;//0xCC type
    public uint ObjectID;
    public byte[] Data;
    public uint Size;

    public bool ErrorSizeLogic = false;

    private uint BlockOffset;
    public Header _ccsHeader;
    public Index _ccsToc;
    public CCSF _ccsf;

    [DisplayName("Block Type")]
    [Description("Define the block type.")]
    [Category("Base Block")]
    public string BlockType { get => this.GetBlockType();}

    [DisplayName("Object Index")]
    [Description("Define the block's Object linked index.")]
    [Category("Base Block")]
    public uint Object
    {
        get => ObjectID;
        set => ObjectID = value;
    }
    [DisplayName("Object Name")]
    [Description("The Object Name of the linked object;")]
    [Category("Base Block")]
    public string ObjectName
    {
        get => GetObjectName();
    }
    [Description("Get the data modified from property to property.")]
    [Category("Base Block")]
    public virtual byte[] DataArray 
    {
        get
        {
            //var m = new BinaryWriter(new MemoryStream(this.Data));
            //m.BaseStream.Position = 0x8;
            //m.Write(ObjectID);
            return this.Data;
        }
            
    }
    
    public byte[] BlockData(Stream Input, long offst)
    {
        var writer = new BinaryWriter(new MemoryStream(Data));
        long oldof = Input.Position;
        Input.Position = offst;
        Input.Write(Data, 0, (int)Size + 8);
        Input.Position = oldof;
        return Data;
    }

    #region TIPOS DE BLOCO
    //Useful defines
    //Higher level sections
    public const int SECTION_HEADER = 0x0001;
    public const int SECTION_INDEX = 0x0002;
    public const int SECTION_SETUP = 0x0003;
    public const int SECTION_STREAM = 0x0005;

    //Lower level sections
    public const int SECTION_OBJECT = 0x0100;
    public const int SECTION_OBJECT_KEYFRAME = 0x0101;
    public const int SECTION_OBJECT_CONTROLLER = 0x0102;
    public const int SECTION_MATERIAL = 0x0200;
    public const int SECTION_MATERIAL_KEYFRAME = 0x0201;
    public const int SECTION_MATERIAL_CONTROLLER = 0x0202;
    public const int SECTION_TEXTURE = 0x0300;
    public const int SECTION_CLUT = 0x0400;
    public const int SECTION_CAMERA = 0x0500;
    public const int SECTION_CAMERA_KEYFRAME = 0x0502;
    public const int SECTION_LIGHT_AMBIENT = 0x0600;
    public const int SECTION_LIGHT_AMBIENT_KEYFRAME = 0x0601;
    public const int SECTION_LIGHT_DIRECTIONAL_CONTROLLER = 0x0603;
    public const int SECTION_LIGHT_OMNI_CONTROLLER = 0x0609;
    //public const int SECTION_LIGHT_DIRECTIONAL_KEYFRAME = 0x0602;
    public const int SECTION_ANIMATION_CONTAINER = 0x0700;
    public const int SECTION_MODEL_CONTAINER = 0x0800;
    public const int SECTION_CLUMP = 0x0900;
    public const int SECTION_EXTERNAL = 0x0a00;
    public const int SECTION_HITMESH = 0x0b00;
    public const int SECTION_BBOX = 0x0c00;
    public const int SECTION_PARTICLE = 0x0d00;
    public const int SECTION_PGE = 0x0d80;
    public const int SECTION_PAC = 0x0d90;
    public const int SECTION_EFFECT = 0x0e00;
    public const int SECTION_BLTGROUP = 0x1000;
    public const int SECTION_FBPAGE = 0x1100;
    public const int SECTION_FBRECT = 0x1200;
    public const int SECTION_DUMMYPOS = 0x1300;
    public const int SECTION_DUMMYPOSROT = 0x1400;
    public const int SECTION_LAYER = 0x1700;
    public const int SECTION_SHADOW = 0x1800;
    public const int SECTION_MORPHER = 0x1900;
    public const int SECTION_MORPHER_KEYFRAME = 0x1901;
    public const int SECTION_LYR = 0x1A00;
    public const int SECTION_LYR_KEYFRAME = 0x1A01;
    public const int SECTION_CSP = 0x1B00;
    public const int SECTION_CSP_KEYFRAME = 0x1B01;
    public const int SECTION_FBS = 0x1D00;
    public const int SECTION_FBS_KEYFRAME = 0x1D01;
    public const int SECTION_OBJECT2 = 0x2000;
    public const int SECTION_PCM = 0x2200; 
    public const int SECTION_BINARYBLOB = 0x2400;
    public const int SECTION_FRAME = 0xFF01;

    public static Dictionary<int, string> ObjectTypeNames = new Dictionary<int, string>()
        {
            {SECTION_HEADER, "Header"},
            {SECTION_INDEX, "Index"},
            {SECTION_SETUP, "Setup"},
            {SECTION_STREAM, "Stream"},
            {SECTION_OBJECT, "Object"},
            {SECTION_OBJECT_KEYFRAME, "Object[KEYFRAME]"},
            {SECTION_OBJECT_CONTROLLER, "Object[CONTROLLER]"},
            {SECTION_MATERIAL, "Material"},
            {SECTION_MATERIAL_KEYFRAME, "Material[KEYFRAME]"},
            {SECTION_MATERIAL_CONTROLLER, "Material[CONTROLLER]"},
            {SECTION_TEXTURE, "Texture"},
            {SECTION_CLUT, "CLUT"},
            {SECTION_CSP, "CSP"},
            {SECTION_CSP_KEYFRAME, "CSP[KEYFRAME]"},
            {SECTION_CAMERA, "Camera"},
            {SECTION_CAMERA_KEYFRAME, "Camera[KEYFRAME]"},
            {SECTION_LIGHT_AMBIENT, "Ambient Light"},
            {SECTION_LIGHT_AMBIENT_KEYFRAME, "Ambient Light[KEYFRAME]"},
            {SECTION_LIGHT_DIRECTIONAL_CONTROLLER, "Directional Light Controller"},
            {SECTION_LIGHT_OMNI_CONTROLLER, "Omni Light Controller"},
            {SECTION_ANIMATION_CONTAINER, "Animation"},
            {SECTION_MODEL_CONTAINER, "Model"},
            {SECTION_CLUMP, "Clump"},
            {SECTION_EXTERNAL, "External"},
            {SECTION_HITMESH, "Hit Mesh"},
            {SECTION_BBOX, "Bounding Box"},
            {SECTION_PARTICLE, "Particle"},
            {SECTION_PGE, "PGE"},
            {SECTION_PAC, "PAC"},
            {SECTION_EFFECT, "Effect"},
            {SECTION_BLTGROUP, "Blit Group"},
            {SECTION_FBPAGE, "FrameBuffer Page"},
            {SECTION_FBRECT, "FrameBuffer Rect"},
            {SECTION_DUMMYPOS, "Dummy(Position)"},
            {SECTION_DUMMYPOSROT, "Dummy(Position & Rotation)"},
            {SECTION_LAYER, "Layer"},
            {SECTION_LYR, "LYR Something"},
            {SECTION_LYR_KEYFRAME, "LYR Something[KEYFRAME]"},
            {SECTION_FBS, "FBS"},
            {SECTION_FBS_KEYFRAME, "FBS[KEYFRAME]"},
            {SECTION_SHADOW, "Shadow"},
            {SECTION_MORPHER, "Morpher"},
            {SECTION_MORPHER_KEYFRAME, "Morpher[KEYFRAME]"},
            {SECTION_OBJECT2, "Object 2"},
            {SECTION_PCM, "PCM Audio"},
            {SECTION_BINARYBLOB, "Binary Blob"},
            {SECTION_FRAME, "Frame"}
        };
    #endregion
    public Block() { }
    public Block(uint type, uint ObjIndex , byte[] data=null)
    {
        Type = type;
        ObjectID = ObjIndex;
        Size = (uint)(4 + (data != null ? data.Length : 0));

        Data = new byte[(int)Size + 8];
        Array.Copy(Type.ToLEBE(32), 0, Data, 0, 4);
        Array.Copy((Size/4).ToLEBE(32), 0, Data, 4, 4);
        Array.Copy(ObjectID.ToLEBE(32), 0, Data, 8, 4);
        if(data!=null)
            Array.Copy(data, 0, Data, 0xC, data.Length);
    }
    public string GetObjectName()
    {
        if (this._ccsToc != null)
        {
            foreach (var file in this._ccsToc.Files)
                foreach (var objectx in file.Objects)
                    if (objectx.Index + 1 == this.ObjectID)
                        return objectx.ObjectName;
        }
        return "NO OBJECT";
    }
    public virtual void SetIndexes(ObjectEntry Object, ObjectEntry[] AllObjects)
    {
        ObjectID = (uint)Object.Index;
        var writer = new BinaryWriter(new MemoryStream(this.Data));
        writer.BaseStream.Position = 0x8;
        writer.Write((UInt32)ObjectID);
        writer.Close();
    }
    public static void SetIndexes(byte[] block, Index.ObjectStream Object)
    {
        Array.Copy(BitConverter.GetBytes((UInt32)Object.ObjIndex),0,block,8,4);
    }
    public virtual byte[] ToArray()
    {
        var result = new List<byte>();
        result.AddRange(Type.ToLEBE(32));
        result.AddRange((Size / 4).ToLEBE(32));
        result.AddRange(ObjectID.ToLEBE(32));
        if(Data.Length>0xC)
            result.AddRange(Data.ReadBytes(0xC, Data.Length - 0xC));
        return result.ToArray();
    }
    public virtual Block ReadBlock(Stream Input) => new Block()
    {
        Type = Input.ReadUInt(0, 32),
        ObjectID = Input.ReadUInt(8, 32),
        Size = Input.ReadUInt(4, 32) * 4
    }; 
    public virtual Block ReadBlock(Stream Input,string typename) => new Block()
    {
        Type = Input.ReadUInt(0, 32),
        ObjectID = Input.ReadUInt(8, 32),
        Size = Input.ReadUInt(4, 32) * 4
    };
    public virtual Block ReadBlock(Stream Input, Header header) => new Block()
    {
        Type = Input.ReadUInt(0, 32),
        ObjectID = Input.ReadUInt(8, 32),
        Size = Input.ReadUInt(4, 32) * 4
    };
    public virtual Block ReadBlock(Stream Input, Header header, Animation anim) => new Block()
    {
        Type = Input.ReadUInt(0, 32),
        ObjectID = Input.ReadUInt(8, 32),
        Size = Input.ReadUInt(4, 32) * 4
    };
    public virtual Block ReadBlock(Stream Input, Header header, CCSF file) => new Block()
    {
        Type = Input.ReadUInt(0, 32),
        ObjectID = Input.ReadUInt(8, 32),
        Size = Input.ReadUInt(4, 32) * 4
    };
    public static List<Block> ReadAllBlocks(Stream Input, bool logAllBlocks = false, bool logExport = false, Header cch = null, bool readone = false, CCSF ccsfile = null, Animation anm = null)
    {
        Console.WriteLine("Iniciando leitura de dados...\r\n");

        Block header, index;
        Header CCSHeader = null;
        Index CCSToc = null;
        if (cch != null)
            CCSHeader = cch;

        var blocks = new List<Block>();
        bool End = false;
        while (End == false)
        {
            long oldOffset = Input.Position;

            uint SizeBlock = Input.ReadUInt((int)((int)Input.Position + 4), 32) * 4;
            SizeBlock += 8;

            Input.Position = oldOffset;


            byte[] BlockData = Input.ReadBytes((int)SizeBlock);

            var mem = new MemoryStream(BlockData);
            uint BlockType = mem.ReadUInt(0, 32);
            mem.Position = 0;

            //if(BlockType == 0xcccc0800)
            //{
            //    var data = new List<byte>();
            //    byte[] readed = mem.ReadBytes(4);
            //    data.AddRange(readed);
            //    readed = mem.ReadBytes(4);
            //    data.AddRange(readed);

            //    bool end = false;
            //    while (end == false)
            //    {
            //        if (readed.ReadUInt(2, 16) != 0xCCCC)
            //        {
            //            readed = mem.ReadBytes(4);
            //            data.AddRange(readed);
            //        }
            //        else
            //            end = true;
            //    }
            //    readed = mem.ReadBytes(4);
            //    data.AddRange(readed);
            //    BlockData = data.ToArray();
            //}
            //mem = new MemoryStream(BlockData);
            //mem.Position = 0;

            string objName = "";
            if (logAllBlocks)
            {
                ObjectTypeNames.TryGetValue((int)(BlockType & 0xFFFF), out objName);

                Console.WriteLine($"Lendo bloco {objName} na posição 0x{oldOffset.ToString("X2")}...\r\n");

            }

            switch (BlockType & 0xFFFF)
            {
                #region Header Types
                case SECTION_HEADER: //Header
                    header = new Header().ReadBlock(mem);
                    CCSHeader = header as Header;
                    blocks.Add(header);
                    break;

                case SECTION_INDEX: //Index
                    //Fix Block padding 0x03000000 00000000
                    Input.Position = oldOffset;

                    SizeBlock += 8;

                    BlockData = Input.ReadBytes((int)SizeBlock);
                    mem = new MemoryStream(BlockData);

                    index = new Index().ReadBlock(mem);
                    CCSToc = index as Index;
                    blocks.Add(index);
                    break;
                #endregion
                #region 3D
                #region Collisions
                case SECTION_BBOX: //BOUNDARY BOX
                    blocks.Add(new BoundingBox().ReadBlock(mem));
                    break;
                #endregion
                #region Modelling
                case SECTION_OBJECT: //BONE
                    blocks.Add(new Object().ReadBlock(mem, CCSHeader));
                    break;
                case SECTION_MODEL_CONTAINER: //MODEL
                    blocks.Add(new Model().ReadBlock(mem, CCSHeader));
                    break;

                case SECTION_CLUMP: //CLUMP
                    var clump = new Clump();
                    clump._ccsToc = CCSToc;
                    blocks.Add(clump.ReadBlock(mem, CCSHeader, ccsfile));
                    break;
                #endregion
                #region Light and Environment
                case SECTION_LIGHT_AMBIENT:
                    blocks.Add(new Light().ReadBlock(mem));
                    break;
                #endregion
                #region Others
                case SECTION_DUMMYPOS:
                    blocks.Add(new Dummy().ReadBlock(mem,"Position"));
                    break;
                case SECTION_DUMMYPOSROT:
                    blocks.Add(new Dummy().ReadBlock(mem, "Rotation"));
                    break;
                #endregion  
                #endregion
                #region 2D
                case SECTION_TEXTURE: //TEXTURE
                    //Fix Block size sum with 0x32
                    Input.Position = oldOffset;

                    SizeBlock = (SizeBlock / 4) - 0x32;
                    SizeBlock *= 4;

                    BlockData = Input.ReadBytes((int)SizeBlock);
                    mem = new MemoryStream(BlockData);

                    blocks.Add(new Texture().ReadBlock(mem));
                    break;

                case SECTION_CLUT: //CLUT
                    blocks.Add(new CLUT().ReadBlock(mem));
                    break;

                case SECTION_MATERIAL: //MATERIAL
                    if (CCSHeader.Version == Header.CCSFVersion.GEN2)
                    {
                        //Fix Block size subtract with 3
                        Input.Position = oldOffset;

                        SizeBlock = (SizeBlock / 4) + 3;
                        SizeBlock *= 4;

                        BlockData = Input.ReadBytes((int)SizeBlock);
                        mem = new MemoryStream(BlockData);
                    }

                    blocks.Add(new Material().ReadBlock(mem, CCSHeader));
                    break;

                #endregion
                #region Animation
                #region Keyframe Types
                case SECTION_OBJECT_KEYFRAME: //BONE_KEYFRAME
                    blocks.Add(new Object_KF().ReadBlock(mem, CCSHeader));
                    break;
                case SECTION_CAMERA_KEYFRAME: //Camera KEYFRAME
                    blocks.Add(new Camera_KF().ReadBlock(mem, CCSHeader));
                    break;
                #endregion
                #region Controllers Types
                case SECTION_OBJECT_CONTROLLER: //BONE_CONTROLLER
                    blocks.Add(new Object_CT().ReadBlock(mem, CCSHeader, anm));
                    break;
                case SECTION_MATERIAL_CONTROLLER: //MATERIAL_CONTROLLER
                    blocks.Add(new Material_CT().ReadBlock(mem, CCSHeader, anm));
                    break;
                #endregion

                case SECTION_ANIMATION_CONTAINER:
                    blocks.Add(new Animation().ReadBlock(mem));
                    break;
                case SECTION_FRAME: //FINAL MARK
                    var final = new Frame().ReadBlock(mem);
                    if (Input.Position == Input.Length)
                        if (Convert.ToInt32((final as Frame).IndexOrFlag) < 0)
                            End = true;
                    blocks.Add(final);
                    break;
                #endregion
                #region Scripting
                case SECTION_MORPHER: //External Link
                    blocks.Add(new Morpher().ReadBlock(mem, CCSHeader));
                    break;
                case SECTION_EXTERNAL: //External Link
                    blocks.Add(new External().ReadBlock(mem, CCSHeader));
                    break;
                case SECTION_BINARYBLOB: //BINARY BLOB
                    blocks.Add(new BinaryBlob().ReadBlock(mem));
                    break;
                #endregion

                default:
                    blocks.Add(new Block().ReadBlock(mem));
                    break;
            }

            //Leitura de Dados de Blocos
            blocks.Last()._ccsHeader = CCSHeader;
            blocks.Last()._ccsToc = CCSToc;
            blocks.Last()._ccsf = ccsfile;
            blocks.Last().BlockOffset = (uint)oldOffset;
            blocks.Last().Data = Input.ReadBytes((int)blocks.Last().BlockOffset, (int)(Input.Position - blocks.Last().BlockOffset));

            if (readone)
                End = true;

            if (logAllBlocks)
                Console.WriteLine($"Bloco tipo {objName} lido com sucesso na posição 0x{oldOffset.ToString("X2")}!\r\n");
#if DEBUGX
            Console.ReadLine();
#endif
        }
        Console.WriteLine($"{blocks.Count()} blocos lidos com sucesso!\r\n");
        return blocks;
    }

    public static uint GetBlockSize(Stream Input, bool maintain = false, string breakerAlt = "")
    {
        uint oldoffs = (uint)Input.Position;

        Input.Position += 4;

        uint result = 4;

        for(; Input.Position <= Input.Length-4 && 
            !ObjectTypeNames.ContainsKey(GetTypeNext(Input));
             result += 4)
        {


        }
        if (maintain)
            Input.Position = oldoffs;

        
        return result;
    }
    static int GetTypeNext(Stream Input)
    {
        byte[] Type = Input.ReadBytes(4);
        if (Type.Skip(2).All(x => x == 0xCC))
        {
            return BitConverter.ToInt32(Type, 0) & 0xFFFF;
        }
        return 0;
    }
    public string GetBlockType() 
    {
        string valuex = "";
        if (ObjectTypeNames.ContainsKey((int)(Type & 0xFFFF)))
            ObjectTypeNames.TryGetValue((int)(Type & 0xFFFF), out valuex);
        else
            valuex = Type.ToString("X2");
        return valuex;
    }
    public static string GetBlockType(uint type)
    {
        string valuex = "";
        if (ObjectTypeNames.ContainsKey((int)(type & 0xFFFF)))
            ObjectTypeNames.TryGetValue((int)(type & 0xFFFF), out valuex);
        else
            valuex = type.ToString("X2");
        return valuex;
    }

}

