using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using static IOextent;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;

#region Tracks of Values
public class Vec3Position_Track
{
    [Category("Track")]
    [DisplayName("Keys Count")]
    [Description("Track's keys count.")]
    public uint _count
    {
        get => KeyCount;
        set => KeyCount = value;
    }
    [Category("Track")]
    [DisplayName("Keys")]
    [Description("Track's keys.")]
    public Position[] _keys
    {
        get => _posKeys;
        set => _posKeys = value;
    }
    public struct Position
    {
        [Category("Position Key")]
        [DisplayName("Frame Number")]
        [Description("Position's frame number.")]
        public uint _num
        {
            get => FrameNumber;
            set => FrameNumber = value;
        }
        [Category("Position Key")]
        [DisplayName("X")]
        [Description("Position's x coordinate.")]
        public float _x
        {
            get => Pos.X;
            set => Pos.X = value;
        }
        [Category("Position Key")]
        [DisplayName("Y")]
        [Description("Position's y coordinate.")]
        public float _y
        {
            get => Pos.Y;
            set => Pos.Y = value;
        }
        [Category("Position Key")]
        [DisplayName("Z")]
        [Description("Position's z coordinate.")]
        public float _z
        {
            get => Pos.Z;
            set => Pos.Z = value;
        }
        public uint FrameNumber, FrameCount;
        public Vector3 Pos;

        internal static Position Read(Stream input, bool readframe = true, uint framenum = 0) => new Position()
        {
            FrameNumber = readframe ? input.ReadUInt(32): framenum,
            Pos = Helper3D.ReadVec3(input)
        };
    }

    public uint KeyCount;
    public Position[] _posKeys;
    public Position _fixedPos;
    public Animation.TrackType TType;

    internal static Vec3Position_Track Read(Stream input, Animation.TrackType trackType)   
    {
        var track = new Vec3Position_Track();
        track.TType = trackType;
        switch(trackType)
        {
            case Animation.TrackType.Fixed:
                track._fixedPos = Position.Read(input, false);
                break;
            case Animation.TrackType.Animated:
                track.KeyCount = input.ReadUInt(32);

                track._posKeys = Enumerable.Range(0, (int)track.KeyCount).Select
                    (x => Position.Read(input)).ToArray();
                break;

        }  
        return track;
    }
}
public class Vec4Rotation_Track
{
    [Category("Track")]
    [DisplayName("Keys Count")]
    [Description("Track's keys count.")]
    public uint _count
    {
        get => KeyCount;
        set => KeyCount = value;
    }
    [Category("Track")]
    [DisplayName("Keys")]
    [Description("Track's keys.")]
    public Quat[] _keys
    {
        get => _rotKeys;
        set => _rotKeys = value;
    }
    public struct Quat
    {
        [Category("Rotation4 Key")]
        [DisplayName("Frame Number")]
        [Description("Rotation's frame number.")]
        public uint _num
        {
            get => FrameNumber;
            set => FrameNumber = value;
        }
        [Category("Rotation4 Key")]
        [DisplayName("X")]
        [Description("Rotation's x coordinate.")]
        public float _x
        {
            get => Rot.X;
            set => Rot.X = value;
        }
        [Category("Rotation4 Key")]
        [DisplayName("Y")]
        [Description("Rotation's y coordinate.")]
        public float _y
        {
            get => Rot.Y;
            set => Rot.Y = value;
        }
        [Category("Rotation4 Key")]
        [DisplayName("Z")]
        [Description("Rotation's z coordinate.")]
        public float _z
        {
            get => Rot.Z;
            set => Rot.Z = value;
        }
        public uint FrameNumber, FrameCount;
        public Quaternion Rot;

        internal static Quat Read(Stream input, bool readframe = true, uint framenum = 0) => new Quat()
        {
            FrameNumber = readframe ? input.ReadUInt(32) : framenum,
            Rot = Helper3D.ReadVec4(input)
        };
    }

    public uint KeyCount;
    public Quat[] _rotKeys;
    public Quat _fixedRot;
    public Animation.TrackType TType;

    internal static Vec4Rotation_Track Read(Stream input, Animation.TrackType trackType)
    {
        var track = new Vec4Rotation_Track();
        track.TType = trackType;
        switch (trackType)
        {
            case Animation.TrackType.Fixed:
                track._fixedRot = Quat.Read(input, false);
                break;

            case Animation.TrackType.Animated:
                track.KeyCount = input.ReadUInt(32);

                track._rotKeys = Enumerable.Range(0, (int)track.KeyCount).Select
                    (x => Quat.Read(input)).ToArray();
                break;
        }



        return track;
    }
}
public class Vec3Rotation_Track
{
    [Category("Track")]
    [DisplayName("Keys Count")]
    [Description("Track's keys count.")]
    public uint _count
    {
        get => KeyCount;
        set => KeyCount = value;
    }
    [Category("Track")]
    [DisplayName("Keys")]
    [Description("Track's keys.")]
    public Rotation[] _keys
    {
        get => _rotKeys;
        set => _rotKeys = value;
    }
    public struct Rotation
    {
        [Category("Rotation Key")]
        [DisplayName("Frame Number")]
        [Description("Rotation's frame number.")]
        public uint _num
        {
            get => FrameNumber;
            set => FrameNumber = value;
        }
        [Category("Rotation Key")]
        [DisplayName("X")]
        [Description("Rotation's x coordinate.")]
        public float _x
        {
            get => Rot.X;
            set => Rot.X = value;
        }
        [Category("Rotation Key")]
        [DisplayName("Y")]
        [Description("Rotation's y coordinate.")]
        public float _y
        {
            get => Rot.Y;
            set => Rot.Y = value;
        }
        [Category("Rotation Key")]
        [DisplayName("Z")]
        [Description("Rotation's z coordinate.")]
        public float _z
        {
            get => Rot.Z;
            set => Rot.Z = value;
        }
        public uint FrameNumber, FrameCount;
        public Vector3 Rot;

        internal static Rotation Read(Stream input, bool readframe = true, uint framenum = 0) => new Rotation()
        {
            FrameNumber = readframe ? input.ReadUInt(32): framenum,
            Rot = Helper3D.ReadVec3(input)
        };
    }

    public uint KeyCount;
    public Rotation[] _rotKeys;
    public Rotation _fixedRot;
    public Animation.TrackType TType;

    internal static Vec3Rotation_Track Read(Stream input, Animation.TrackType trackType)
    {
        var track = new Vec3Rotation_Track();
        track.TType = trackType;
        switch(trackType)
        {
            case Animation.TrackType.Fixed:
                track._fixedRot = Rotation.Read(input, false);
                break;

            case Animation.TrackType.Animated:
                track.KeyCount = input.ReadUInt(32);

                track._rotKeys = Enumerable.Range(0, (int)track.KeyCount).Select
                    (x => Rotation.Read(input)).ToArray();
                break;
        }

        

        return track;
    }
}
public class Vec3Scale_Track
{
    [Category("Track")]
    [DisplayName("Keys Count")]
    [Description("Track's keys count.")]
    public uint _count
    {
        get => KeyCount;
        set => KeyCount = value;
    }
    [Category("Track")]
    [DisplayName("Keys")]
    [Description("Track's keys.")]
    public Scale[] _keys
    {
        get => _scaleKeys;
        set => _scaleKeys = value;
    }
    public struct Scale
    {
        [Category("Scale Key")]
        [DisplayName("Frame Number")]
        [Description("Scale's frame number.")]
        public uint _num
        {
            get => FrameNumber;
            set => FrameNumber = value;
        }
        [Category("Scale Key")]
        [DisplayName("X")]
        [Description("Scale's x coordinate.")]
        public float _x
        {
            get => Scal.X;
            set => Scal.X = value;
        }
        [Category("Scale Key")]
        [DisplayName("Y")]
        [Description("Scale's y coordinate.")]
        public float _y
        {
            get => Scal.Y;
            set => Scal.Y = value;
        }
        [Category("Scale Key")]
        [DisplayName("Z")]
        [Description("Scale's z coordinate.")]
        public float _z
        {
            get => Scal.Z;
            set => Scal.Z = value;
        }
        public uint FrameNumber, FrameCount;
        public Vector3 Scal;

        internal static Scale Read(Stream input, bool readframe = true, uint framenum = 0) => new Scale()
        {
            FrameNumber = readframe ? input.ReadUInt(32): framenum,
            Scal = Helper3D.ReadVec3(input)
        };
    }

    public uint KeyCount;
    public Scale[] _scaleKeys;
    public Scale _fixedScale;
    public Animation.TrackType TType;

    internal static Vec3Scale_Track Read(Stream input, Animation.TrackType trackType)
    {
        var track = new Vec3Scale_Track();
        track.TType = trackType;
        switch (trackType)
        {
            case Animation.TrackType.Fixed:
                track._fixedScale = Scale.Read(input, false);
                break;

            case Animation.TrackType.Animated:
                track.KeyCount = input.ReadUInt(32);

                track._scaleKeys = Enumerable.Range(0, (int)track.KeyCount).Select
                    (x => Scale.Read(input)).ToArray();
                break;

        }
        return track;
    }
}
public class UV_Track
{

}
public class F32_Track
{
    [Category("Track")]
    [DisplayName("Keys Count")]
    [Description("Track's keys count.")]
    public uint _count
    {
        get => KeyCount;
        set => KeyCount = value;
    }
    [Category("Track")]
    [DisplayName("Keys")]
    [Description("Track's keys.")]
    public F32[] _keys
    {
        get => _alphaKeys;
        set => _alphaKeys = value;
    }
    public struct F32
    {
        [Category("F32 Key")]
        [DisplayName("Frame Number")]
        [Description("F32's frame number.")]
        public uint _num
        {
            get => FrameNumber;
            set => FrameNumber = value;
        }
        [Category("F32 Key")]
        [DisplayName("Key")]
        [Description("F32's key value.")]
        public float _key
        {
            get => Key;
            set => Key = value;
        }
        public uint FrameNumber, FrameCount;
        public float Key;

        internal static F32 Read(Stream input, bool readframe = true, uint framenum = 0) => new F32()
        {
            FrameNumber = readframe ? input.ReadUInt(32) : framenum,
            Key = input.ReadSingle()
        };
    }

    public uint KeyCount;
    public F32[] _alphaKeys;
    public F32 _fixedAlpha;
    public Animation.TrackType TType;

    internal static F32_Track Read(Stream input, Animation.TrackType trackType)
    {
        var track = new F32_Track();
        track.TType = trackType;
        switch (trackType)
        {
            case Animation.TrackType.Fixed:
                track._fixedAlpha = F32.Read(input,false);
                break;
            case Animation.TrackType.Animated:
                track.KeyCount = input.ReadUInt(32);

                track._alphaKeys = Enumerable.Range(0, (int)track.KeyCount).Select
                    (x => F32.Read(input)).ToArray();
                break;

        }
        return track;
    }
}
public class INT32_Track
{

}

#endregion
public class Animation : Block
{
    [Category("Animation")]
    [DisplayName("Frames/Resources")]
    [Description("Animation's blocks of frames and resources.")]
    public Block[] frames
    {
        get => Frames;
        set => Frames = value;
    }
    public enum TrackType: uint
    {
        None = 0,
        Fixed = 1,
        Animated = 2
    };
    public static TrackType GetTrack(int trackID, int param)
    {
        if (trackID > 10) return 0;
        return (TrackType)((param >> (3 * trackID)) & 0x7);
    }

    public int FrameCount;
    public Block[] Frames;
    internal void ToTreeView(TreeView treeView, bool forceResource = false, bool forceFrame = false)
    {
        treeView.BeginUpdate();
            //Get Resources
            var resourcenode = new CCSNode("Resources");
            foreach (var block in Frames.Skip(1).TakeWhile(x => x.Type != 0xccccff01))
                resourcenode.Nodes.Add(new CCSNode(block.GetBlockType())
                {
                    Block = block
                });
            treeView.Nodes.Add(resourcenode);

            //Get frame groups
            int framec = 0;
            var frame = new CCSNode();
            var framegroup = new List<Block>();
            foreach (var block in Frames.Skip(1).SkipWhile(x => x.Type != 0xccccff01))
            {
                if (block.Type == 0xccccff01)
                {
                    if (frame.Text != "")
                    {
                        frame.FrameBlocks = framegroup;

                        treeView.Nodes.Add(frame);
                    }
                    frame = new CCSNode("Frame Group");
                    framegroup = new List<Block>();
                    frame.Block = block;
                    framec++;
                }
                else
                {
                    framegroup.Add(block);
                    frame.Nodes.Add(new CCSNode(block.GetBlockType()) { Block = block });
                }
            }
        treeView.EndUpdate();
    }
    public virtual byte[] DataArray
    {
        get
        {
            var writer = new BinaryWriter(new MemoryStream(Data));

            writer.BaseStream.Position = 0xC;

            writer.Write(FrameCount);

            foreach(var frame in Frames)
                writer.Write(frame.DataArray);

            writer.Close();

            return this.Data;
        }
    }
    public override Block ReadBlock(Stream Input) => new Animation()
    {
        Type = Input.ReadUInt(32),
        Size = Input.ReadUInt(32) * 4,
        ObjectID = Input.ReadUInt(32),

        FrameCount = (int)Input.ReadUInt(32),

        //And then more one Uint32 for ContainerSize, then read blocks in container
        Frames = Block.ReadAllBlocks(new MemoryStream(Input.ReadBytes((int)(Input.ReadUInt(32)*4))),false, false,
            _ccsHeader,false,
            _ccsf,
            this).ToArray()
    };
    
}
