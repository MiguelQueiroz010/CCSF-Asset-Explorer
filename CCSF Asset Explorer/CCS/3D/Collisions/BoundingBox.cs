using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using static IOextent;
using System.ComponentModel;
using System.Text;
public struct BBox
{
    public Vector3 Minimum;// = Vector3.Zero;
    public Vector3 Maximum; // = Vector3.Zero;
    public Vector4 Color; // = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);

    public static BBox Read(Stream Input) => new BBox()
    {
        Minimum = Helper3D.ReadVec3(Input),
        Maximum = Helper3D.ReadVec3(Input)
    };

    public byte[] GetBBBin()
    {
        var b = new List<byte>();
        b.AddRange(Minimum.GetVec3());
        b.AddRange(Maximum.GetVec3());
        return b.ToArray();
    }
}
public class BoundingBox : Block
{
    public uint ModelIndex;
    public BBox Box;

    [DisplayName("Linked Model Index")]
    [Description("Bounding Box linked model index.")]
    [Category("Boundings")]
    public uint _midx
    {
        get => ModelIndex;
        set => ModelIndex = value;
    }
    [DisplayName("Linked Model Name")]
    [Description("Bounding Box linked model.")]
    [Category("Boundings")]
    public string _mname
    {
        get => _ccsToc.GetObjectName(ModelIndex);
    }
    [DisplayName("Boundary Box")]
    [Description("Bounding Box.")]
    [Category("Boundings")]
    public BBox[] _box
    {
        get => new BBox[] { Box };
        set => Box = value[0];
    }

    public override byte[] DataArray
    {
        get
        {
            var writer = new BinaryWriter(new MemoryStream(Data));

            writer.BaseStream.Position = 0xC;
            writer.Write(ModelIndex);
            writer.Write(Box.GetBBBin());

            return Data;
        }
    }
    public override Block ReadBlock(Stream Input) => new BoundingBox()
    {
        Type = Input.ReadUInt(0, 32),
        Size = Input.ReadUInt(4, 32) * 4,
        ObjectID = Input.ReadUInt(8, 32),
        ModelIndex = Input.ReadUInt(0xC, 32),
        Box = BBox.Read(Input)
    };
    public override byte[] ToArray()
    {
        var result = new List<byte>();
        result.AddRange(Type.ToLEBE(32));
        result.AddRange((Size / 4).ToLEBE(32));
        result.AddRange(ObjectID.ToLEBE(32));

        result.AddRange(ModelIndex.ToLEBE(32));
        result.AddRange(Box.GetBBBin());

        return result.ToArray();
    }
}
