using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using static IOextent;
using System.ComponentModel;
using System.Text;
using OpenTK;
public class HitGroup
{
    public Vector3[] Vertices = null;

    public uint VertexCount = 0;
    public Color color;
    public Vector3 Minimum = new Vector3(0.0f);
    public Vector3 Maximum = new Vector3(0.0f);
    public static HitGroup Read(Stream Input)
    {
        var hitg = new HitGroup();
        hitg.VertexCount = Input.ReadUInt(32) * 2;
        hitg.color = Color.FromArgb(Input.ReadByte(), Input.ReadByte(), Input.ReadByte(), Input.ReadByte());
        hitg.Vertices = Enumerable.Range(0, (int)hitg.VertexCount).Select
            (x=> Helper3D.ReadVec3(Input)).ToArray();
        return hitg;
    }

    public byte[] GetHGBin()
    {
        var b = new List<byte>();
        b.AddRange(BitConverter.GetBytes((UInt32)(VertexCount / 2)));
        b.AddRange(new byte[] {color.A,color.R,color.G,color.B });
        foreach (var vertex in Vertices)
            b.AddRange(vertex.GetVec3());
        return b.ToArray();
    }
}
public class HitGroups : Block
{
    public uint UnkIndex, HitGroupsCount, VertexCount;
    public HitGroup[] hitGroups;

    [DisplayName("Linked Obj Index")]
    [Description("HitGroups linked obj index.")]
    [Category("HitGroup")]
    public uint _midx
    {
        get => UnkIndex;
        set => UnkIndex = value;
    }
    [DisplayName("Linked Obj Name")]
    [Description("HitGroups linked obj.")]
    [Category("HitGroup")]
    public string _mname
    {
        get => _ccsToc.GetObjectName(UnkIndex);
    }
    [DisplayName("Hit Boxes")]
    [Description("Hit Boxes.")]
    [Category("Hit Groups")]
    public HitGroup[] _box
    {
        get => hitGroups;
        set => hitGroups = value;
    }

    public override byte[] DataArray
    {
        get
        {
            var writer = new BinaryWriter(new MemoryStream(Data));

            writer.BaseStream.Position = 0xC;
            writer.Write(UnkIndex);
            writer.Write(Convert.ToUInt32(hitGroups.Length));
            writer.Write(VertexCount);

            foreach (var hit in hitGroups)
                writer.Write(hit.GetHGBin());

            return Data;
        }
    }
    public override Block ReadBlock(Stream Input)
    {
        var hitg = new HitGroups();

        hitg.Type = Input.ReadUInt(0, 32);
        hitg.Size = Input.ReadUInt(4, 32) * 4;
        hitg.ObjectID = Input.ReadUInt(8, 32);
        hitg.UnkIndex = Input.ReadUInt(0xC, 32);
        hitg.HitGroupsCount = Input.ReadUInt(0x10, 32);
        
        hitg.hitGroups = new HitGroup[hitg.HitGroupsCount];
        hitg.VertexCount = Input.ReadUInt(0x14, 32);

        hitg.hitGroups = Enumerable.Range(0, (int)hitg.VertexCount).Select
            (x=> HitGroup.Read(Input)).ToArray();

        return hitg;   
    }
    public override byte[] ToArray()
    {
        var result = new List<byte>();
        result.AddRange(Type.ToLEBE(32));
        result.AddRange((Size / 4).ToLEBE(32));
        result.AddRange(ObjectID.ToLEBE(32));



        return result.ToArray();
    }
}
