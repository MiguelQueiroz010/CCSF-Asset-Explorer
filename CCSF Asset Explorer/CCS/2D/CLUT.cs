using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using static IOextent;
using System.ComponentModel;
using System.Text;

public class CLUT : Block
{
    public uint BlitGroup;
    public uint Unknow1, Unknow2;
    public uint ColorCount;

    public Color[] Palette;

    [DisplayName("Blit Group Index")]
    [Description("Index of the Blit Group of the texture.")]
    [Category("CLUT")]
    public uint BlitIDX
    {
        get => BlitGroup;
        set => BlitGroup = value;
    }
    [DisplayName("Blit Group Object Name")]
    [Description("Blit Group Object's name.")]
    [Category("CLUT")]
    public string BlitObj
    {
        get => _ccsToc.GetObjectName(BlitGroup);
    }

    [DisplayName("Color Palette[CLT]")]
    [Description("Array of colors.")]
    [Category("CLUT")]
    public Color[] _colors
    {
        get => Palette;
        set => Palette = value;
    }
    public override byte[] DataArray
    {
        get
        {
            var writer = new BinaryWriter(new MemoryStream(Data));

            writer.BaseStream.Position = 0xC;

            writer.Write(BlitGroup);
            writer.Write(Unknow1);
            writer.Write(Unknow2);
            writer.Write(ColorCount);

            writer.Write(GetColorData(Palette));


            return Data;
        }
    }
    public override Block ReadBlock(Stream Input) => new CLUT()
    {
        Type = Input.ReadUInt(0, 32),
        Size = Input.ReadUInt(4, 32) * 4,
        ObjectID = Input.ReadUInt(8, 32),

        BlitGroup = Input.ReadUInt(0xc,32),
        Unknow1 = Input.ReadUInt(0x10,32),
        Unknow2 = Input.ReadUInt(0x14,32),

        ColorCount = Input.ReadUInt(0x18,32),

        Palette = Enumerable.Range(0, (int)Input.ReadUInt(0x18, 32)).Select(
            x => ReadColor(Input.ReadBytes((int)(0x1c + (x * 4)), 4)
            )).ToArray()
    };
    public override byte[] ToArray()
    {
        var result = new List<byte>();
        result.AddRange(Type.ToLEBE(32));
        result.AddRange((Size / 4).ToLEBE(32));
        result.AddRange(ObjectID.ToLEBE(32));

        result.AddRange(BlitGroup.ToLEBE(32));
        result.AddRange(Unknow1.ToLEBE(32));
        result.AddRange(Unknow2.ToLEBE(32));
        result.AddRange(ColorCount.ToLEBE(32));

        result.AddRange(GetColorData(Palette, false));
        return result.ToArray();
    }
    public static byte HalfAlpha(byte alpha)
    {
        alpha = (byte)((alpha * 128) / 255);
        return alpha;
    }
    public static byte FullAlpha(byte alpha)
    {
        alpha = (byte)((alpha * 255) / 128);
        return alpha;
    }
    public static Color ReadColor(byte[] color, bool convertalpha = true)
    {
        return Color.FromArgb(
            convertalpha ? FullAlpha(color[3]): color[3], //A
            color[0], //R
            color[1], //G
            color[2]);//B
    }

    public byte[] GetColorData(Color[] colors, bool convert=true)
    {
        var result = new List<byte>();
        foreach(var color in colors)
        {
            result.Add(color.R);
            result.Add(color.G);
            result.Add(color.B);
            result.Add(convert ? HalfAlpha(color.A) : color.A);
        }
        return result.ToArray();
    }
}
