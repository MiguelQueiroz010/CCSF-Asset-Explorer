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

public class Dummy : Block
{
    public Vector3 Value;
    private string TName;

    [DisplayName("X")]
    [Description("Value in 3d space.")]
    [Category("Bind Value")]
    public decimal _positionx
    {
        get => (decimal)Value.X;
        set => Value.X = (float)value;
    }
    [DisplayName("Y")]
    [Description("Value in 3d space.")]
    [Category("Bind Value")]
    public decimal _positiony
    {
        get => (decimal)Value.Y;
        set => Value.Y = (float)value;
    }
    [DisplayName("Z")]
    [Description("Value in 3d space.")]
    [Category("Bind Value")]
    public decimal _positionz
    {
        get => (decimal)Value.Z;
        set => Value.Z = (float)value;
    }
    public override byte[] DataArray
    {
        get
        {
            var writer = new BinaryWriter(new MemoryStream(Data));

            writer.BaseStream.Position = 0xC;

            writer.Write(Value.GetVec3());

            return Data;
        }
    }
    public override Block ReadBlock(Stream Input, string typename) => new Dummy()
    {
        TName = typename,
        Type = Input.ReadUInt(32),
        Size = Input.ReadUInt(32) * 4,
        ObjectID = Input.ReadUInt(32),
        Data = Input.ReadBytes(0, (int)Size),

        Value = Helper3D.ReadVec3(Input)

    };
}
