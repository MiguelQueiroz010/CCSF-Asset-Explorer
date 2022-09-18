using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using static IOextent;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text;
using OpenTK;

public class Light : Block
{
    public uint LightType;

    [DisplayName("Light Type")]
    [Description("Light type value.")]
    [Category("Light")]
    public uint _positionx
    {
        get => LightType;
        set => LightType = value;
    }
    public override byte[] DataArray
    {
        get
        {
            var writer = new BinaryWriter(new MemoryStream(Data));

            writer.BaseStream.Position = 0xC;

            writer.Write(LightType);

            return Data;
        }
    }
    public override Block ReadBlock(Stream Input) => new Light()
    {
        Type = Input.ReadUInt(32),
        Size = Input.ReadUInt(32) * 4,
        ObjectID = Input.ReadUInt(32),
        Data = Input.ReadBytes(0, (int)Size),

        LightType = Input.ReadUInt(32)

    };
}
