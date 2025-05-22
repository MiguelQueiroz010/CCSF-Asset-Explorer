using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using static IOextent;
using System.Text;

public class Header: Block
{
    public enum CCSFVersion: uint
    {
        GEN1 = 0x110,
        GEN1_5 = 0x111,
        GEN2 = 0x120,
        GEN2_5 = 0x123,
        GEN3 = 0x125,
        Unknow
    };

    const int Magic = 0x46534343;
    public int _version;
    public CCSFVersion Version;
    public string FileName;

    public uint FrameCounter, Unk2, Unk3;

    public override Block ReadBlock(Stream Input) => new Header()
    {
        Type = Input.ReadUInt(0, 32),
        Size = Input.ReadUInt(4, 32) * 4,
        ObjectID = Input.ReadUInt(8, 32),
        

        FileName = Input.ReadBytes(0xC,0x20).ConvertTo(Encoding.Default),
        Version = (CCSFVersion)Input.ReadUInt(0x2c, 32),
        _version = (int)Input.ReadUInt(0x2c, 32),
        FrameCounter = Input.ReadUInt(0x30, 32),
        Unk2 = Input.ReadUInt(0x34, 32),
        Unk3 = Input.ReadUInt(0x38, 32)
    };
    public Header() { }
    public Header(string filename, CCSFVersion version, uint framecount = 0)
    {
        Type = 0xcccc0001;
        Size = 0x34;
        ObjectID = BitConverter.ToUInt32(Encoding.Default.GetBytes("CCSF"), 0);
        FileName = filename;
        Version = version;
        FrameCounter = framecount;
        Unk2 = 1;
        Unk3 = 0;
        //unkdata
    }

    public override byte[] ToArray()
    {
        var result = new List<byte>();
        result.AddRange(Type.ToLEBE(32));
        result.AddRange((Size/4).ToLEBE(32));
        result.AddRange(ObjectID.ToLEBE(32));

        byte[] filenby = new byte[0x20];
        Array.Copy(Encoding.Default.GetBytes(FileName), filenby, FileName.Length);
        result.AddRange(filenby);

        result.AddRange(((UInt32)Version).ToLEBE(32));
        result.AddRange(FrameCounter.ToLEBE(32));
        result.AddRange(Unk2.ToLEBE(32));
        result.AddRange(Unk3.ToLEBE(32));

        return result.ToArray();
    }
}
