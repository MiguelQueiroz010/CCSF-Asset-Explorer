using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using static IOextent;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text;

public class External : Block
{

    public uint ReferenceParent, ReferenceObject;

    [DisplayName("Reference Scene Object")]
    [Description("Index of the referenced scene object in this CCSF.")]
    [Category("External Link")]
    public uint ParentIndex
    {
        get => ReferenceParent;
        set => ReferenceParent = value;
    }
    [DisplayName("Referenced Object Index")]
    [Description("Index of the referenced link object in this CCSF.")]
    [Category("External Link")]
    public uint ObjectIndex
    {
        get => ReferenceObject;
        set => ReferenceObject = value;
    }
    public override byte[] DataArray
    {
        get
        {
            var writer = new BinaryWriter(new MemoryStream(Data));

            writer.BaseStream.Position = 0xC;

            writer.Write(ReferenceParent);
            writer.Write(ReferenceObject);

            return Data;
        }
    }
    public override Block ReadBlock(Stream Input, Header header) => new External()
    {
        Type = Input.ReadUInt(32),
        Size = Input.ReadUInt(32) * 4,
        ObjectID = Input.ReadUInt(32),
        Data = Input.ReadBytes(0, (int)Size),

        ReferenceParent = Input.ReadUInt(32),
        ReferenceObject = Input.ReadUInt(32)
    };
}
