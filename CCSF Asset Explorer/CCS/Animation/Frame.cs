using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using static IOextent;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text;

public class Frame : Block
{
    [Category("Frame")]
    [DisplayName("Frame Flag/Index")]
    [Description("Frame's index or flag for scene play mode.")]
    public string _index
    {
        get => IndexOrFlag < 0 ? GetFlag().ToString() : $"Index: {IndexOrFlag}";
    }
    public Flags GetFlag() => (Flags)IndexOrFlag;
    public enum Flags: int
    {
        PlayOnce = -1,
        Repeat = -2
    };

    public int IndexOrFlag;
    public virtual byte[] DataArray
    {
        get
        {
            var writer = new BinaryWriter(new MemoryStream(Data));

            writer.BaseStream.Position = 0x8;

            writer.Write(IndexOrFlag);

            return this.Data;
        }
    }
    public override Block ReadBlock(Stream Input) => new Frame()
    {
        Type = Input.ReadUInt(32),
        Size = Input.ReadUInt(32) * 4,
        IndexOrFlag = (int)Input.ReadUInt(32),
        Data = Input.ReadBytes(0, (int)Size)
    };
}
