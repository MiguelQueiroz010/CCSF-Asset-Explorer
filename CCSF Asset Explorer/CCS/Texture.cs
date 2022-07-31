using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using static IOextent;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using Rainbow.ImgLib.Encoding;

public class Texture : Block
{
    public enum TEXType: byte
    {
        RGBA32 = 0,
        I8 = 0X13,
        I4 = 0x14,
        DXT1 = 0X87,
        DXT5 = 0X89
    };
    public string CLUTOBJ;
    public uint CLUTID;
    public uint BlitGroupID;
    public byte CLUTCount;
    
    public TEXType TextureType;

    public byte MipCount;
    public uint Width; //Byte type  1 << value
    public uint Height; //Byte type  1 << value

    public byte[] PixelData;

    private uint Unknow1, Unknow2, Unknow3;

    private bool NonP2 = false;

    [DisplayName("Clut Index")]
    [Description("Index of the Color Palette of the texture.")]
    [Category("Texture")]
    public uint ClutIDX
    {
        get => CLUTID;
        set => CLUTID = value;
    }

    [DisplayName("Clut Object Name")]
    [Description("Color Lookup Table Object's name.")]
    [Category("Texture")]
    public string ClutObj
    {
        get => _ccsToc.GetObjectName(CLUTID);
    }

    [DisplayName("Blit Index")]
    [Description("Index of the Blit Group of the texture.")]
    [Category("Texture")]
    public uint BlitIDX
    {
        get => BlitGroupID;
        set => BlitGroupID = value;
    }
    [DisplayName("Blit Group Object Name")]
    [Description("Blit Group Object's name.")]
    [Category("Texture")]
    public string BlitObj
    {
        get => _ccsToc.GetObjectName(BlitGroupID);
    }

    [DisplayName("Pixel Type")]
    [Description("Type of the pixel storage of the texture.")]
    [Category("Pixel Attributes")]
    public TEXType TexType
    {
        get => TextureType;
    }

    [DisplayName("Mipmap Count")]
    [Description("Count of mipmaps present in the texture.")]
    [Category("Pixel Attributes")]
    public byte MipCnt
    {
        get => MipCount;
    }

    [DisplayName("Width")]
    [Description("Width of the texture.")]
    [Category("Pixel Attributes")]
    public uint _width
    {
        get => Width;
    }

    [DisplayName("Height")]
    [Description("Height of the texture.")]
    [Category("Pixel Attributes")]
    public uint _height
    {
        get => Height;
    }

    public override byte[] DataArray
    {
        get
        {
            var writer = new BinaryWriter(new MemoryStream(Data));

            writer.BaseStream.Position = 0xC;

            writer.Write(CLUTID);
            writer.Write(BlitGroupID);
            writer.Write(CLUTCount);

            writer.BaseStream.Position = 0x18;
            writer.Write(GetTrueWH(Width));
            writer.Write(GetTrueWH(Height));

            return Data;
        }
    }

    public Bitmap ToBitmap(CLUT clut)
    {
        byte[] pixel = PixelData;
        Bitmap OutBitmap = new Bitmap((int)Width, (int)Height);
        if (TextureType == TEXType.I4 || TextureType == TEXType.I8)
        {
            Color[] Palette = clut.Palette;
            if (TextureType == TEXType.I4)
            {
                int pos = 0;
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < (Width / 2); x++)
                    {
                        int indiceOffset = pos;
                        byte indices = pixel[indiceOffset];
                        OutBitmap.SetPixel(x * 2, y , Palette[indices % 0x10]);
                        OutBitmap.SetPixel((x * 2) + 1, y , Palette[(indices / 0x10)]);
                        pos++;
                    }
                }

            }
            else if (TextureType == TEXType.I8)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        int indiceOffset = (int)(y * Width) + x;
                        OutBitmap.SetPixel(x, y, Palette[pixel[indiceOffset]]);
                    }
                }

            }
        }
        else if (TextureType == TEXType.RGBA32)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int i = (int)((y * Width) + x) * 4;
                    OutBitmap.SetPixel(x, y, Color.FromArgb(pixel[i + 3], pixel[i + 2], pixel[i + 1], pixel[i]));
                }
            }
        }
        OutBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

        return OutBitmap;
    }

    public bool SetfromPNG(Image input, out byte[] CLTcolors)
    {
        Width = (uint)input.Width;
        Height = (uint)input.Height;

        CLTcolors = null;
        var colors = new HashSet<Color>();
        byte[] coresbyte = null;
        Color[] cores;
        Bitmap bit = new Bitmap(input);

        int colorcount = 0;
        #region Obter cores no eixo cartesiano 2D        
        for (int y = 0; y < bit.Height; y++)
        {
            for (int x = 0; x < bit.Width; x++)
            {
                colors.Add(bit.GetPixel(x, y));
            }
        }
        colorcount = colors.ToArray().Length;
        #region Calcular quantia de cores
        if (TextureType == TEXType.I8)
            colorcount = 256;
        else if (TextureType == TEXType.I4)
            colorcount = 16;
        else if(TextureType == TEXType.RGBA32)
        {
            if (MessageBox.Show("The image has too much colors!\n" +
            "Expected: " + (PixelData.Length / 4).ToString() + " Colors, " + TextureType.ToString() + "\n" +
            "Got: " + colorcount.ToString() + " Colors, " + bit.PixelFormat.ToString() + "\n\n" +
            "Want to convert to 32bpp?", "System", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                TextureType = TEXType.RGBA32;
                CLUTID = 0;
                CLUTOBJ = null;
                CLUTCount = 0;
            }
            else
            {
                MessageBox.Show("Canceled.");
                return false;
            }
        }
        #endregion
        #region Separar cores para array

        if (TextureType == TEXType.I4 || TextureType == TEXType.I8)
        {
            if((TextureType == TEXType.I4 && 
                colors.Count > 16)||
                (TextureType == TEXType.I8 &&
                colors.Count > 256))
            {
                MessageBox.Show($"The image has too much colors. ({colors.Count}), expected reduction, {TextureType}.",
                    "System");
                return false;
            }
            cores = new Color[colorcount];
            Array.Copy(colors.ToArray(), 0, cores, 0, colors.Count);
            coresbyte = new byte[colorcount * 4];//1024 bytes = 256 cores
            for (int i = 0; i < coresbyte.Length; i += 4)
            {
                if ((i / 4) < cores.Length)
                {
                    coresbyte[i] = cores[i / 4].R;
                    coresbyte[i + 1] = cores[i / 4].G;
                    coresbyte[i + 2] = cores[i / 4].B;
                    int alpha = cores[i / 4].A;
                    alpha *= 128;
                    alpha /= 255;
                    coresbyte[i + 3] = (byte)alpha;
                }

            }
        }
        #endregion
        #endregion
        #region Obter índices de pixel no eixo cartesiano 2D
        var pixeldata = new List<byte>();
        Color c1, c2;
        int flagx = bit.Width;
        if (TextureType == TEXType.I4)
            flagx /= 2;

        for (int y = 0; y < bit.Height; y++)
            for (int x = 0; x < flagx; x++)
            {
                if (TextureType == TEXType.I4)
                {
                    c1 = bit.GetPixel(x * 2 + 1, bit.Height - y - 1);
                    c2 = bit.GetPixel(x * 2, bit.Height - y - 1);
                    pixeldata.Add((byte)((Texture.FindColorIndex(c1, colors.ToArray()) << 4) + Texture.FindColorIndex(c2, colors.ToArray())));

                }
                else if(TextureType == TEXType.I8)
                {
                    c1 = bit.GetPixel(x, bit.Height - y - 1);
                    pixeldata.Add(Texture.FindColorIndex(c1, colors.ToArray()));
                }
                else if(TextureType == TEXType.RGBA32)
                {
                    c1 = bit.GetPixel(x, bit.Height - y - 1);    
                    pixeldata.Add(c1.B);
                    pixeldata.Add(c1.G);
                    pixeldata.Add(c1.R);
                    pixeldata.Add(c1.A);
                }
            }
        #endregion
        PixelData = pixeldata.ToArray();

        if (TextureType == TEXType.I4 || TextureType == TEXType.I8)
            CLTcolors = coresbyte;
        return true;
    }

    byte[] ReadPixelData(Stream Reader, 
        uint Width, uint Height, TEXType TEXtype)
    {
        Reader.Position = 0x24;//Talvez colocar isso no escopo do ReadBlock...
        byte[] result = new byte[0];

        uint BaseSize = Width * Height;

        switch(TEXtype)
        {
            case TEXType.I4:
                result = new byte[BaseSize / 2]; //4bpp
                Reader.ReadBytes((int)(BaseSize / 2)).CopyTo(result, 0);
                break;

            case TEXType.I8:
                result = new byte[BaseSize]; //8bpp
                Reader.ReadBytes((int)BaseSize).CopyTo(result, 0);
                break;
            case TEXType.RGBA32:
                result = new byte[BaseSize * 4]; //32bpp
                Reader.ReadBytes((int)BaseSize*4).CopyTo(result, 0);
                break;
        }
        return result;
    }

    uint SetTrueWH(byte val)
    {
        int vall = 1 << (int)val;
        return (uint)vall;
    }
    byte GetTrueWH(uint val)
    {
        byte vall = 0;
        for (byte i = 1; i<11; i++) 
        {
            int test = ((int)val >> i);
            if (test == 1)
            {
                return i;
            }
        }
        return vall;
    }
    public override Block ReadBlock(Stream Input) => new Texture()
    {
        Type = Input.ReadUInt(0, 32),
        Size = (Input.ReadUInt(4, 32) - 0x32) * 4,
        ObjectID = Input.ReadUInt(8, 32),

        CLUTID = Input.ReadUInt(0xC,32),
        BlitGroupID = Input.ReadUInt(0x10,32),
        CLUTCount = Input.ReadBytes(0x14,1)[0],

        TextureType = (TEXType)Input.ReadBytes(0x15,1)[0],
        MipCount = Input.ReadBytes(0x16,1)[0],
        Unknow1 = Input.ReadBytes(0x17,1)[0],

        Width = SetTrueWH(Input.ReadBytes(0x18, 1)[0]),
        Height = SetTrueWH(Input.ReadBytes(0x19, 1)[0]),

        Unknow2 = Input.ReadUInt(0x1A, 16),

        Unknow3 = Input.ReadUInt(0x1C, 32),

        PixelData = ReadPixelData(Input, (uint)(1 << Input.ReadBytes(0x18, 1)[0]),
            (uint)(1 << Input.ReadBytes(0x19, 1)[0]),
            (TEXType)Input.ReadBytes(0x15, 1)[0])
    };

    public override void SetIndexes(Index.ObjectStream Object, Index.ObjectStream[] AllObjects)
    {
        ObjectID = (uint)Object.ObjIndex;
        bool stop = false;
        foreach (var obj in AllObjects)
            if (obj.ObjName == CLUTOBJ)
                for (int b = 0; stop != true &&
                    b < obj.Blocks.Count; b++)
                {
                    if (obj.Blocks[b].ReadUInt(0, 32) == 0xcccc0300)
                    {
                        CLUTID = obj.Blocks[b].ReadUInt(8, 32);
                        stop = true;
                    }
                }
    }

    public override byte[] ToArray()
    {
        var result = new List<byte>();
        Size = 0x1C + (uint)PixelData.Length;
        result.AddRange(Type.ToLEBE(32));
        result.AddRange(((Size / 4)+0x32).ToLEBE(32));
        result.AddRange(ObjectID.ToLEBE(32));

        result.AddRange(CLUTID.ToLEBE(32));
        result.AddRange(BlitGroupID.ToLEBE(32));
        result.Add(CLUTCount);

        result.Add((byte)TextureType);
        result.Add(MipCount);
        result.Add((byte)Unknow1);

        result.Add(GetTrueWH(Width));
        result.Add(GetTrueWH(Height));

        result.AddRange(Unknow2.ToLEBE(16));
        result.AddRange(Unknow3.ToLEBE(32));
        result.AddRange(((uint)(PixelData.Length/4)).ToLEBE(32));
        result.AddRange(PixelData);


        return result.ToArray();
    }

    public static byte FindColorIndex(Color v, Color[] pal)
    {
        byte index = 0;
        for (byte i = 0; i < pal.Length; i++)
            if (pal[i].R == v.R &&
                pal[i].G == v.G &&
                pal[i].B == v.B &&
                pal[i].A == v.A)
                return i;

        return index;
    }
}
