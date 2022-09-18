using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using static IOextent;
using System.Text;
using System.ComponentModel;
using OpenTK;

public class Material : Block
{
    public string TextureOBJ;
    public uint TextureID;
    public float Alpha;

    public Vector2 TextureOffset = Vector2.Zero;
    public int X, Y, SWidth, SHeight;
    public bool Gen1 = false;
    byte[] RestData;

    [DisplayName("Texture Index")]
    [Description("Define the texture index for the Material.")]
    [Category("Material")]
    public uint TextureIndex
    {
        get => TextureID;
        set => TextureID = value;
    }
    [DisplayName("Texture Object Name")]
    [Description("Define the texture object for the Material.")]
    [Category("Material")]
    public string TextureObject
    {
        get => _ccsToc.GetObjectName(TextureID);
    }
    [DisplayName("Alpha")]
    [Description("Define the material alpha threshold.")]
    [Category("Material")]
    public decimal AlphaVal
    {
        get => (decimal)Alpha;
        set => Alpha = (float)value;
    }
    [DisplayName("X")]
    [Description("Define the texture offset.")]
    [Category("Texture Offset")]
    public int Textureoffs_x
    {
        get => X;
        set => X = value;
    }
    [DisplayName("Y")]
    [Description("Define the texture offset.")]
    [Category("Texture Offset")]
    public int Textureoffs_y
    {
        get => Y;
        set => Y = value;
    }
    [DisplayName("Width")]
    [Description("Define the texture offset.")]
    [Category("Texture Size")]
    public int sx
    {
        get => SWidth;
        set => SWidth = value;
    }
    [DisplayName("Height")]
    [Description("Define the texture offset.")]
    [Category("Texture Size")]
    public int sy
    {
        get => SHeight;
        set => SHeight = value;
    }
    public override byte[] DataArray
    {
        get
        {
            var writer = new BinaryWriter(new MemoryStream(Data));
            writer.BaseStream.Position = 0xC;
            writer.Write(TextureID);
            writer.Write(Alpha);
            writer.Write(BitConverter.GetBytes((Int16)X));
            writer.Write(BitConverter.GetBytes((Int16)Y));
            writer.Write(BitConverter.GetBytes((Int16)SWidth));
            writer.Write(BitConverter.GetBytes((Int16)SHeight));
            return Data;
        }
    }

    
    public override Block ReadBlock(Stream Input, Header header) => new Material()
    {
        Type = Input.ReadUInt(0, 32),
        Size = Input.ReadUInt(4, 32) * 4,
        ObjectID = Input.ReadUInt(8, 32),
        Data = Input.ReadBytes(0, (int)Size),

        TextureID = Input.ReadUInt(0xC, 32),
        Alpha = BitConverter.ToSingle(Input.ReadBytes(0x10,4),0),
        Gen1 = header.Version == Header.CCSFVersion.GEN1 ? true : false,
        X = (int)Input.ReadUInt(0x14,16),
        Y = (int)Input.ReadUInt(0x16,16),
        SWidth = (int)Input.ReadUInt(0x18,16),
        SHeight = (int)Input.ReadUInt(0x1a,16),
        RestData = Input.ReadBytes(8)
    };
    public override byte[] ToArray()
    {
        var result = new List<byte>();
        result.AddRange(Type.ToLEBE(32));
        result.AddRange((Size / 4).ToLEBE(32));
        result.AddRange(ObjectID.ToLEBE(32));
        result.AddRange(TextureID.ToLEBE(32));
        result.AddRange(BitConverter.GetBytes((Int32)Alpha));

        result.AddRange(BitConverter.GetBytes((Int16)X));
        result.AddRange(BitConverter.GetBytes((Int16)Y));
        result.AddRange(BitConverter.GetBytes((Int16)SWidth));
        result.AddRange(BitConverter.GetBytes((Int16)SHeight));

        if (RestData != null)
            result.AddRange(RestData);
        return result.ToArray();
    }

    public override void SetIndexes(Index.ObjectStream Object, Index.ObjectStream[] AllObjects)
    {
        ObjectID = (uint)Object.ObjIndex;
        bool stop = false;
        foreach(var obj in AllObjects)
            if(obj.ObjName==TextureOBJ)
                for (int b = 0; stop != true &&
                    b < obj.Blocks.Count; b++)
                {
                    if (obj.Blocks[b].ReadUInt(0, 32) == 0xcccc0300)
                    {
                        TextureID = obj.Blocks[b].ReadUInt(8, 32);
                        stop = true;
                    }
                }
    }
}
