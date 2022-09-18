using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using static IOextent;
using System.Text;
using System.ComponentModel;
using OpenTK;

public class Clump : Block
{
    public class Node
    {
        public Header _ccsHeader;
        public Index _ccsTOC;
        public CCSF _CCSf;

        internal Object[] GetObject() => _CCSf.Blocks.Where(x => x.GetObjectName()==_oname).ToArray() as Object[];
        internal Model GetModel(Object obj) => _CCSf.Blocks.Where(x => x.ObjectID == obj.ModelID).ToArray()[0] as Model;

        [DisplayName("Linked Object")]
        [Description("See the linked object for the model bone.")]
        [Category("Bone")]
        public Object[] _obj
        {
            get => GetObject();
            set => _CCSf.Blocks[_CCSf.IndexOf(_obj[0])] = value[0];
        }
        public struct BoneVis
        {
            public int HeadID;
            public int TailID;
        }

        public uint ID, Index;

        public Matrix4 PoseMatrix, FinalMatrix;

        public Vector3 BindPosition;
        public Vector3 BindRotation, BindScale,
            PosePosition, PoseRotation, PoseScale;

        public Quaternion PoseQuat;

        public BoneVis BoneVisBone;

        [DisplayName("Object Index")]
        [Description("Linked object/model index.")]
        [Category("Bone")]
        public uint _oindx
        {
            get => ID;
            set => ID = value;
        }
        [DisplayName("Object Name")]
        [Description("Name of the object/model linked.")]
        [Category("Bone")]
        public string _oname
        {
            get => _ccsTOC.GetObjectName(ID);
        }
        [DisplayName("X")]
        [Description("Position of the bones in 3d space.")]
        [Category("Bind Position")]
        public decimal _positionx
        {
            get => (decimal)BindPosition.X;
            set => BindPosition.X = (float)value;
        }
        [DisplayName("Y")]
        [Description("Position of the bones in 3d space.")]
        [Category("Bind Position")]
        public decimal _positiony
        {
            get => (decimal)BindPosition.Y;
            set => BindPosition.Y = (float)value;
        }
        [DisplayName("Z")]
        [Description("Position of the bones in 3d space.")]
        [Category("Bind Position")]
        public decimal _positionz
        {
            get => (decimal)BindPosition.Z;
            set => BindPosition.Z = (float)value;
        }
        [DisplayName("X")]
        [Description("Rotation of the bones in 3d space.")]
        [Category("Bind Rotation")]
        public decimal _rotationx
        {
            get => (decimal)BindRotation.X;
            set => BindRotation.X = (float)value;
        }
        [DisplayName("Y")]
        [Description("Rotation of the bones in 3d space.")]
        [Category("Bind Rotation")]
        public decimal _rotationy
        {
            get => (decimal)BindRotation.Y;
            set => BindRotation.Y = (float)value;
        }
        [DisplayName("Z")]
        [Description("Rotation of the bones in 3d space.")]
        [Category("Bind Rotation")]
        public decimal _rotationz
        {
            get => (decimal)BindRotation.Z;
            set => BindRotation.Z = (float)value;
        }
        [DisplayName("X")]
        [Description("Scale of the bones in 3d space.")]
        [Category("Bind Scale")]
        public decimal _scalex
        {
            get => (decimal)BindScale.X;
            set => BindScale.X = (float)value;
        }
        [DisplayName("Y")]
        [Description("Scale of the bones in 3d space.")]
        [Category("Bind Scale")]
        public decimal _scaley
        {
            get => (decimal)BindScale.Y;
            set => BindScale.Y = (float)value;
        }
        [DisplayName("Z")]
        [Description("Scale of the bones in 3d space.")]
        [Category("Bind Scale")]
        public decimal _scalez
        {
            get => (decimal)BindScale.Z;
            set => BindScale.Z = (float)value;
        }
        [DisplayName("Head Index")]
        [Description("Index of the Head Model Object.")]
        [Category("Bone")]
        public int _head
        {
            get => BoneVisBone.HeadID;
            set => BoneVisBone.HeadID = value;
        }
        [DisplayName("Head Object Name")]
        [Description("Name of the Head Model Object.")]
        [Category("Bone")]
        public string _headname
        {
            get => _ccsTOC.GetObjectName((uint)BoneVisBone.HeadID);
        }
        [DisplayName("Tail Index")]
        [Description("Index of the Tail Model Object.")]
        [Category("Bone")]
        public int _tail
        {
            get => BoneVisBone.TailID;
            set => BoneVisBone.TailID = value;
        }
        [DisplayName("Head Object Name")]
        [Description("Name of the Tail Model Object.")]
        [Category("Bone")]
        public string _tailname
        {
            get => _ccsTOC.GetObjectName((uint)BoneVisBone.TailID);
        }
        public static Node ReadNode(Stream Input, Header header, int i, Index ccstoc, CCSF ccsf, uint[] IDs, Clump clump)
        {
            Node result = new Node();
            result._ccsHeader = header;
            result.ID = IDs[i];
            result._ccsTOC = ccstoc;
            result._CCSf = ccsf;
            if (header.Version <= Header.CCSFVersion.GEN1)
            {
                result.BindPosition = Vector3.Zero;
                result.PosePosition = Vector3.Zero;

                result.BindRotation = Vector3.Zero;
                result.PoseRotation = Vector3.Zero;

                result.BindScale = Vector3.One;
                result.PoseScale = Vector3.One;

                result.BoneVisBone.HeadID = i;
                result.BoneVisBone.TailID = i;
            }
            else
            {
                Vector3 pos = Helper3D.ReadVec3Position(Input);
                Vector3 rot = Helper3D.ReadVec3(Input);
                Vector3 scale = Helper3D.ReadVec3(Input);

                result.BindPosition = pos;
                result.BindRotation = rot;
                result.BindScale = scale;

                result.PosePosition = Vector3.Zero;
                result.PoseQuat = Quaternion.Identity;
                result.PoseScale = Vector3.One;
                result.BoneVisBone.HeadID = i;
                result.BoneVisBone.TailID = i;
            }

            
            return result;
        }

        public byte[] ToArray()
        {
            var result = new List<byte>();
            if (_ccsHeader.Version > Header.CCSFVersion.GEN1)
            {
                result.AddRange(BindPosition.GetVec3Position());
                result.AddRange(BindRotation.GetVec3());
                result.AddRange(BindScale.GetVec3());
            }

            return result.ToArray();
        }
    }

	public uint NodeCount;
    public Node[] Nodes;

    [DisplayName("Node Array")]
    [Description("Array of bones that form the Object.")]
    [Category("Clump")]
    public Node[] _nodes
    {
        get => Nodes;
        set => Nodes = value;
    }
    public override byte[] DataArray
    {
        get
        {
            var writer = new BinaryWriter(new MemoryStream(Data));

            writer.BaseStream.Position = 0xC;

            writer.Write(NodeCount);

            //IDs
            foreach (var node in Nodes)
                writer.Write(node.ID);

            //Content
            foreach (var node in Nodes)
                writer.Write(node.ToArray());

            return Data;
        }
    }
    public override Block ReadBlock(Stream Input, Header header, CCSF file)
    {
        Clump result = new Clump();
        result._ccsf = file;
        result.Type = Input.ReadUInt(32);
        result.Size = Input.ReadUInt(32) * 4;
        result.ObjectID = Input.ReadUInt(32);
        result.Data = Input.ReadBytes(0, (int)Size);

        result.NodeCount = Input.ReadUInt(32);
        uint[] ids = Enumerable.Range(0, (int)result.NodeCount).
          Select(c => Input.ReadUInt(32)).ToArray();

        result.Nodes = Enumerable.Range(0, (int)result.NodeCount).Select(x =>
          Node.ReadNode(Input, header, x, _ccsToc, file, ids, result)).ToArray();

        return result;
    }
    public override void SetIndexes(Index.ObjectStream Object, Index.ObjectStream[] AllObjects)
    {
        ObjectID = (uint)Object.ObjIndex;
        bool stop = false;
        for (int b = 0; stop != true &&
            b < Object.Blocks.Count; b++)
        {
            if (Object.Blocks[b].ReadUInt(0, 32) == 0xcccc0400)
            {
                var clut = new CLUT().ReadBlock(new MemoryStream(Object.Blocks[b]));
                stop = true;
            }
        }
    }
    public override byte[] ToArray()
    {
        var result = new List<byte>();
        result.AddRange(Type.ToLEBE(32));
        result.AddRange((Size / 4).ToLEBE(32));
        result.AddRange(ObjectID.ToLEBE(32));

        result.AddRange(NodeCount.ToLEBE(32));

        //IDs
        foreach (var node in Nodes)
            result.AddRange(node.ID.ToLEBE(32));
        //Content
        foreach (var node in Nodes)
            result.AddRange(node.ToArray());

        return result.ToArray();
    }
}
