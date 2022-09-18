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

public class Morpher : Block
{
    public uint LinkedModelIndex;

    [DisplayName("Linked Model Index")]
    [Description("Index of the referenced link model in this CCSF.")]
    [Category("Morpher")]
    public uint ObjectIndex
    {
        get => LinkedModelIndex;
        set => LinkedModelIndex = value;
    }
    public override byte[] DataArray
    {
        get
        {
            var writer = new BinaryWriter(new MemoryStream(Data));

            writer.BaseStream.Position = 0xC;

            writer.Write(LinkedModelIndex);

            return Data;
        }
    }
    public override Block ReadBlock(Stream Input) => new Morpher()
    {
        Type = Input.ReadUInt(32),
        Size = Input.ReadUInt(32) * 4,
        ObjectID = Input.ReadUInt(32),
        Data = Input.ReadBytes(0, (int)Size),

        LinkedModelIndex = Input.ReadUInt(32)
    };
}
