using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using static Helper3D;
using static IOextent;
using System.Text;
using System.ComponentModel;
using static Model.Mesh;
using static Model;
using System.IO.Ports;
using static Clump.Node;

/// <summary>
/// Model class. Treats with the model data from 0xCCCC0800 blocks.
/// Code completely documented by https://github.com/Al-Hydra/blender_ccs_importer/blob/main/ccs_lib
/// zMath3us and HydraBladeZ  // and NCDyson and WarrantyVoider
/// </summary>

public class Model : Block
{
    public enum ModelType : int
    {
        Rigid1 = 0,
        Rigid2 = 1,
        TrianglesList = 2,
        Deformable = 4,
        ShadowMesh = 8
    };
    public override Block ReadBlock(Stream Input, Header header)
    {
        Data = Input.ReadBytes(0, (int)Size);
        Model ccModel = new Model();
        _ccsHeader = header;

        ccModel.ObjectID = Input.ReadUInt(32);
        ccModel.VertexScale = Input.ReadSingle();
        ccModel.Mdl_Type = (int)Input.ReadByte();
        ccModel.Mdl_Flags = (byte)Input.ReadByte();
        ccModel.Mesh_Count = Input.ReadUInt(16);

        ccModel.MatFlags1 = (byte)Input.ReadByte();
        ccModel.MatFlags2 = (byte)Input.ReadByte();
        ccModel.UnkFlags = Input.ReadUInt(16);
        ccModel.LT_Count = (byte)Input.ReadByte();
        ccModel.ExtraFlags = (byte)Input.ReadByte();
        ccModel.TangentBinormalsFlag = Input.ReadUInt(16);

        if (_ccsHeader._version > 0x110)
        {
            byte r = (byte)Input.ReadByte();
            byte g = (byte)Input.ReadByte();
            byte b = (byte)Input.ReadByte();
            byte a = (byte)Input.ReadByte();

            ccModel.OutlineColor = Color.FromArgb(a, r, g, b);
            ccModel.OutlineWidth = Input.ReadSingle();
        }

        //LT READER
        if (ccModel.Mdl_Type >= 4 &&
            ccModel.Mdl_Type <= 7 &&
            _ccsHeader._version > 0x111)
        {
            ccModel.LookupList = new byte[ccModel.LT_Count];
            for (int i = 0; i < ccModel.LT_Count; i++)
            {
                ccModel.LookupList[i] = (byte)Input.ReadByte();
            }

            //Alinhar a 4 bytes
            while (Input.Position % 0x4 != 0)
                Input.Position++;
        }
        else
            ccModel.LookupList = null;

        //MESHS READER
        if (ccModel.Mesh_Count > 0)
        {
            Meshes = new List<Mesh>();

            if ((ccModel.Mdl_Type & (int)ModelType.Deformable) != 0 &&
       (ccModel.Mdl_Type & (int)ModelType.TrianglesList) == 0)
            {
                Meshes.AddRange(Enumerable.Range(0, (int)ccModel.Mesh_Count)
                    .Select(x => new Deformable(Input, ccModel._ccsHeader._version, ccModel.VertexScale,
                    ccModel.TangentBinormalsFlag)));
            }
            else if (ccModel.Mdl_Type == (int)ModelType.ShadowMesh)
            {
                Meshes.AddRange(Enumerable.Range(0, (int)ccModel.Mesh_Count)
                    .Select(x => new Shadow(Input, ccModel.VertexScale)));
            }
            else if ((ccModel.Mdl_Type & (int)ModelType.TrianglesList) != 0)
            {
                Meshes.AddRange(Enumerable.Range(0, (int)ccModel.Mesh_Count)
                    .Select(x => new Unk(Input, ccModel.VertexScale)));
            }
            else
            {
                Meshes.AddRange(Enumerable.Range(0, (int)ccModel.Mesh_Count)
                    .Select(x => new Rigid(Input, ccModel.VertexScale, ccModel._ccsHeader._version,
                    ccModel.Mdl_Flags, ccModel.TangentBinormalsFlag
                    )));
            }
        }

        return ccModel;
    }

    public override byte[] ToArray()
    {
        return Data;
    }

    public float VertexScale;
    public int Mdl_Type;
    public byte Mdl_Flags;

    public uint Mesh_Count;
    public byte MatFlags1;
    public byte MatFlags2;

    public uint UnkFlags;

    public byte LT_Count;
    public byte[] LookupList;
    public byte ExtraFlags;

    public uint TangentBinormalsFlag;

    public Color OutlineColor;
    public float OutlineWidth;

    public List<Mesh> Meshes;

    public class Mesh
    {
        public class DeformableVertex
        {
            public List<Vector3H> _positions;
            public List<Vector3H> _normals;
            public List<float> Weights;
            public Vector2 _uv;
            public List<int> BoneId;
            public byte TF;
            public bool multiWeight = false;
        }
        public class Vertex
        {
            public Vector3H _position;
            public Vector3H _normal, _tangent, _binormal;
            public Color _color;

            public Vector2 _uv;
            public byte TF, TangentTF, BiNormalTF;


            public Vertex(Vector3H position, Vector3H normal, Color color,
                Vector2 uv, byte triangleFlag, float VertexScale = CCS_GLOBAL_SCALE)
            {
                _position = position.Multiply((decimal)VertexScale);
                _normal = normal.Divide(64);
                _color = color;
                _uv = uv.Divide(uv, VertexScale);
                TF = triangleFlag;
            }
        }

        public int VertexCount;
        public Vertex[] Vertices;

        public class Rigid : Mesh
        {
            public int ParentIndex;
            public int MaterialID;

            public float FinalScale;

            public Rigid(Stream Input,
                float VertexScale = 64.0f,
            int _version = 0x110,
            byte _mdlflags = 0,
            uint tangentBinormalsFlag = 0)
            {
                ParentIndex = (int)Input.ReadUInt(32);
                MaterialID = (int)Input.ReadUInt(32);
                VertexCount = (int)Input.ReadUInt(32);

                FinalScale = (float)(((VertexScale / 256) / 16) * 0.01);

                Vertices = new Vertex[VertexCount];

                var positionsRead = new List<Vector3H>();
                var normalsRead = new List<Vector4>();

                var _tangnormalsRead = new List<Vector4>();
                var _binnormalsRead = new List<Vector4>();

                var uvReads = new List<Vector2>();
                var colors = new List<Color>();

                for (int i = 0; i < VertexCount; i++)
                {
                    Vector3H position = new Vector3H(Input.ReadUInt(16),
                        Input.ReadUInt(16),
                        Input.ReadUInt(16));
                    positionsRead.Add(position);

                    //Align to 4
                    while (Input.Position % 0x4 != 0)
                        Input.Position++;
                }

                for (int i = 0; i < VertexCount; i++)
                {
                    Vector3H normal = new Vector3H(Input.ReadByte(),
                        Input.ReadByte(),
                        Input.ReadByte());
                    byte TF = (byte)Input.ReadByte();
                    normalsRead.Add(new Vector4((float)normal.X,
                        (float)normal.Y,
                        (float)normal.Z, TF));
                }

                for (int i = 0; i < VertexCount; i++)
                {
                    Color color = Color.Black;
                    if ((_mdlflags & 2) == 0)
                    {
                        byte r = (byte)Math.Min(255, Input.ReadByte() * 2);
                        byte g = (byte)Math.Min(255, Input.ReadByte() * 2);
                        byte b = (byte)Math.Min(255, Input.ReadByte() * 2);
                        byte a = (byte)Math.Min(255, Input.ReadByte() * 2);
                        color = Color.FromArgb(a, r, g, b);
                    }
                    colors.Add(color);
                }

                if ((_mdlflags & 4) == 0)
                    for (int i = 0; i < VertexCount; i++)
                    {
                        Vector2 uv = new Vector2();
                        if (_version > 0x125)
                        {
                            uv = new Vector2(Input.ReadUInt(32) / 65536,
                                Input.ReadUInt(32) / 65536);
                        }
                        else
                        {
                            uv = new Vector2(Input.ReadUInt(16) / 256,
                                Input.ReadUInt(16) / 256);
                        }
                        uvReads.Add(uv);
                    }

                if (tangentBinormalsFlag != 0)
                    for (int i = 0; i < VertexCount; i++)
                    {
                        Vector3H _Tangentnormal = new Vector3H(Input.ReadByte(),
                            Input.ReadByte(),
                            Input.ReadByte());
                        byte _TangentTF = (byte)Input.ReadByte();

                        _tangnormalsRead.Add(new Vector4((float)_Tangentnormal.X,
                            (float)_Tangentnormal.Y,
                            (float)_Tangentnormal.Z, _TangentTF));

                        Vector3H _Binormal = new Vector3H(Input.ReadByte(),
                            Input.ReadByte(),
                            Input.ReadByte());
                        byte _BiTF = (byte)Input.ReadByte();

                        _binnormalsRead.Add(new Vector4((float)_Binormal.X,
                            (float)_Binormal.Y,
                            (float)_Binormal.Z, _BiTF));
                    }
                for (int i = 0; i < VertexCount; i++)
                {
                    Vertices[i] = new Vertex(positionsRead[i],
                        new Vector3H((decimal)normalsRead[i].X
                        , (decimal)normalsRead[i].Y, (decimal)normalsRead[i].Z),
                        colors[i],
                        uvReads[i],
                        (byte)normalsRead[i].W, VertexScale);

                    if (_tangnormalsRead.Count > 0)
                    {
                        Vertices[i]._tangent = new Vector3H((decimal)_tangnormalsRead[i].X,
                            (decimal)_tangnormalsRead[i].Y,
                            (decimal)_tangnormalsRead[i].Z);
                        Vertices[i].TangentTF = (byte)_tangnormalsRead[i].W;
                    }
                    if (_binnormalsRead.Count > 0)
                    {
                        Vertices[i]._binormal = new Vector3H((decimal)_binnormalsRead[i].X,
                            (decimal)_binnormalsRead[i].Y,
                            (decimal)_binnormalsRead[i].Z);
                        Vertices[i].BiNormalTF = (byte)_binnormalsRead[i].W;
                    }
                }

            }
        }

        public class Shadow : Mesh
        {
            public int TriangleVerticesCount;
            public Vector3[] Triangles;

            public Shadow(Stream Input,
                float VertexScale = 64.0f)
            {
                VertexCount = (int)Input.ReadUInt(32);
                TriangleVerticesCount = (int)Input.ReadUInt(32);

                for (int i = 0; i < VertexCount; i++)
                {
                    Vector3H position = new Vector3H(Input.ReadUInt(16),
                        Input.ReadUInt(16),
                        Input.ReadUInt(16));
                    Vertices[i] = new Vertex(position, Vector3H.Zero, Color.Black,
                        Vector2.Zero, 0, VertexScale);
                }

                //Alinhar a 4 bytes
                while (Input.Position % 0x4 != 0)
                    Input.Position++;

                //Triangle vertices
                Triangles = new Vector3[TriangleVerticesCount / 3];
                for (int i = 0; i < TriangleVerticesCount / 3; i++)
                {
                    Vector3 triangle = new Vector3(Input.ReadUInt(32),
                        Input.ReadUInt(32),
                        Input.ReadUInt(32));
                    Triangles[i] = triangle;
                }
            }
        }

        public class Deformable : Mesh
        {
            public int MaterialID;
            public int DeformableVerticesCount;
            public List<DeformableVertex> DefVertices;

            public float FinalScale;
            public Deformable(Stream Input,
                int _version = 0x100,
                float VertexScale = 256.0f,
                uint tangentBinormalFlag = 0)
            {
                MaterialID = (int)Input.ReadUInt(32);
                VertexCount = (int)Input.ReadUInt(32);
                DeformableVerticesCount = (int)Input.ReadUInt(32);

                FinalScale = (float)(((VertexScale / 256) / 16) * 0.01);

                if (DeformableVerticesCount == 0)
                {
                    uint boneID = Input.ReadUInt(32);
                    var vpBuffer = new BinaryReader(
                        new MemoryStream(Input.ReadBytes(VertexCount * 6))
                        , Encoding.GetEncoding(932));

                    //Alinhar a 4 bytes
                    while (Input.Position % 0x4 != 0)
                        Input.Position++;

                    var vnBuffer = new BinaryReader(
                        new MemoryStream(Input.ReadBytes(VertexCount * 4))
                        , Encoding.GetEncoding(932));

                    for (int i = 0; i < VertexCount; i++)
                    {
                        var vertex = new DeformableVertex();
                        vertex._positions = new List<Vector3H>(1);
                        vertex._normals = new List<Vector3H>(1);
                        vertex.Weights = new List<float>(1);
                        vertex.BoneId = new List<int>(1);

                        vertex._positions[0] = new Vector3H(
                            (decimal)(vpBuffer.ReadInt16() * FinalScale),
                            (decimal)(vpBuffer.ReadInt16() * FinalScale),
                            (decimal)(vpBuffer.ReadInt16() * FinalScale));

                        vertex.BoneId[0] = (int)boneID;
                        vertex.Weights[0] = 1;
                        vertex._normals[0] = new Vector3H(
                            (decimal)(vnBuffer.ReadByte() / 64),
                            (decimal)(vnBuffer.ReadByte() / 64),
                            (decimal)(vnBuffer.ReadByte() / 64));
                        vertex.TF = (byte)vnBuffer.ReadByte();
                        DefVertices.Add(vertex);
                    }

                    if (_version > 0x125)
                        foreach (var v in DefVertices)
                        {
                            v._uv = new Vector2(
                                (float)Input.ReadUInt(32) / 65536,
                                (float)Input.ReadUInt(32) / 65536);
                        }
                    else
                        foreach (var v in DefVertices)
                        {
                            v._uv = new Vector2(
                                (float)Input.ReadUInt(16) / 256,
                                (float)Input.ReadUInt(16) / 256);
                        }

                    if (tangentBinormalFlag != 0)
                    {
                        var vtBuffer = new BinaryReader(
                            new MemoryStream(Input.ReadBytes(VertexCount * 4))
                            , Encoding.GetEncoding(932));
                        var vbnBuffer = new BinaryReader(
                            new MemoryStream(Input.ReadBytes(VertexCount * 4))
                            , Encoding.GetEncoding(932));
                    }
                }
                else
                {
                    if (_version < 0x125)
                    {
                        var vpBuffer = new BinaryReader(
                        new MemoryStream(Input.ReadBytes(DeformableVerticesCount * 8))
                        , Encoding.GetEncoding(932));

                        var vnBuffer = new BinaryReader(
                        new MemoryStream(Input.ReadBytes(DeformableVerticesCount * 4))
                        , Encoding.GetEncoding(932));

                        var uvBuffer = new BinaryReader(
                        new MemoryStream(Input.ReadBytes(VertexCount * 4))
                        , Encoding.GetEncoding(932));

                        for (int i = 0; i < VertexCount; i++)
                        {
                            var vertex = new DeformableVertex();
                            vertex._positions = new List<Vector3H>(2);
                            vertex._normals = new List<Vector3H>(2);
                            vertex.Weights = new List<float>(2);
                            vertex.BoneId = new List<int>(2);

                            vertex._positions[0] = new Vector3H(
                                (decimal)(vpBuffer.ReadInt16() * FinalScale),
                                (decimal)(vpBuffer.ReadInt16() * FinalScale),
                                (decimal)(vpBuffer.ReadInt16() * FinalScale));
                            var vertParams = vpBuffer.ReadInt16();
                            vertex.BoneId[0] = vertParams >> 10;
                            vertex.Weights[0] = (vertParams & 0x1ff) / 256;
                            vertex._normals[0] = new Vector3H(
                                (decimal)(vnBuffer.ReadByte() / 64),
                                (decimal)(vnBuffer.ReadByte() / 64),
                                (decimal)(vnBuffer.ReadByte() / 64));
                            vertex.TF = (byte)vnBuffer.ReadByte();

                            if (((vertParams >> 9) & 0x1) == 0)
                            {
                                vertex._positions[1] = new Vector3H(
                                (decimal)(vpBuffer.ReadInt16() * FinalScale),
                                (decimal)(vpBuffer.ReadInt16() * FinalScale),
                                (decimal)(vpBuffer.ReadInt16() * FinalScale));
                                var secondvertParams = vpBuffer.ReadInt16();
                                vertex.BoneId[1] = secondvertParams >> 10;
                                vertex.Weights[1] = (secondvertParams & 0x1ff) / 256;
                                vertex._normals[1] = new Vector3H(
                                    (decimal)(vnBuffer.ReadByte() / 64),
                                    (decimal)(vnBuffer.ReadByte() / 64),
                                    (decimal)(vnBuffer.ReadByte() / 64));
                                vertex.TF = (byte)vnBuffer.ReadByte();
                            }

                            vertex._uv = new Vector2(
                                (float)uvBuffer.ReadInt16() / 256,
                                (float)uvBuffer.ReadInt16() / 256);

                            DefVertices.Add(vertex);
                        }
                    }
                    else
                    {

                        var vpBuffer = new BinaryReader(
                        new MemoryStream(Input.ReadBytes(DeformableVerticesCount * 0xC))
                        , Encoding.GetEncoding(932));

                        var vnBuffer = new BinaryReader(
                        new MemoryStream(Input.ReadBytes(DeformableVerticesCount * 4))
                        , Encoding.GetEncoding(932));

                        var uvBuffer = new BinaryReader(
                        new MemoryStream(Input.ReadBytes(VertexCount * 8))
                        , Encoding.GetEncoding(932));

                        if (tangentBinormalFlag != 0)
                        {
                            var vtBuffer = new BinaryReader(
                            new MemoryStream(Input.ReadBytes(DeformableVerticesCount * 4))
                            , Encoding.GetEncoding(932));

                            var vbnBuffer = new BinaryReader(
                            new MemoryStream(Input.ReadBytes(DeformableVerticesCount * 4))
                            , Encoding.GetEncoding(932));
                        }

                        for (int i = 0; i < VertexCount; i++)
                        {
                            var vertex = new DeformableVertex();
                            vertex._positions = new List<Vector3H>();
                            vertex._normals = new List<Vector3H>();
                            vertex.Weights = new List<float>();
                            vertex.BoneId = new List<int>();

                            uint stopBit = 0;
                            i = 0;
                            while (stopBit == 0)
                            {
                                vertex._positions.Add(new Vector3H(
                                    (decimal)(vpBuffer.ReadInt16() * FinalScale),
                                    (decimal)(vpBuffer.ReadInt16() * FinalScale),
                                    (decimal)(vpBuffer.ReadInt16() * FinalScale))
                                    );
                                vertex.Weights.Add(vpBuffer.ReadInt16() / 256);
                                stopBit = (uint)(vpBuffer.ReadInt16());

                                vertex.BoneId.Add(vpBuffer.ReadInt16());

                                vertex._normals.Add(new Vector3H(
                                    (decimal)(vnBuffer.ReadByte() / 64),
                                    (decimal)(vnBuffer.ReadByte() / 64),
                                    (decimal)(vnBuffer.ReadByte() / 64)));

                                vertex.TF = (byte)vnBuffer.ReadByte();
                                i++;
                            }

                            vertex._uv = new Vector2(
                                    (float)uvBuffer.ReadInt32() / 65536,
                                    (float)uvBuffer.ReadInt32() / 65536);

                            DefVertices.Add(vertex);
                        }

                    }


                }

            }
        }

        public class Unk : Mesh
        {
            public int MaterialID;
            public int SectionCount;
            public int ClumpIndex;
            public int _Unk;

            public List<DeformableVertex> vertices;
            public List<Vector3H> Normals;
            public List<Vector2> Uvs;
            public List<Color> Colors;
            public List<int> BoneIDs;
            public List<float> Weights;
            public List<byte> TFs;
            public List<int> TriangleIndices;

            public Unk(Stream Input,
                float VertexScale = 64.0f)
            {
                MaterialID = (int)Input.ReadUInt(32);
                SectionCount = (int)Input.ReadUInt(32);
                _Unk = (int)Input.ReadUInt(32);

                Normals = new List<Vector3H>();
                vertices = new List<DeformableVertex>();
                Uvs = new List<Vector2>();
                Colors = new List<Color>();
                BoneIDs = new List<int>();
                TFs = new List<byte>();
                TriangleIndices = new List<int>();
                Weights = new List<float>();

                for (int i = 0; i < SectionCount; i++)
                {
                    byte sectionFlags = (byte)Input.ReadByte();
                    byte sectionType = (byte)Input.ReadByte();

                    uint count = Input.ReadUInt(32);
                    float _vertexScale = Input.ReadSingle();

                    float FinalScale = (float)(((_vertexScale / 256) / 16) * 0.01);

                    //Vertex Normals
                    if (sectionType == 0)
                        for (int c = 0; i < count; c++)
                            Normals.Add(new Vector3H((decimal)(Input.ReadByte() / 64),
                                (decimal)(Input.ReadByte() / 64),
                                (decimal)(Input.ReadByte() / 64)));
                    else if (sectionType == 1)//UVS
                        for (int c = 0; i < count; c++)
                            Uvs.Add(new Vector2(Input.ReadUInt(16) / 256,
                                Input.ReadUInt(16) / 256));
                    else if (sectionType == 7)//TriangleFlags
                        for (int c = 0; i < count; c++)
                            TFs.Add((byte)Input.ReadByte());
                    else if (sectionType == 8)//TriangleIndices
                        for (int c = 0; i < count; c++)
                            TriangleIndices.Add((int)Input.ReadUInt(16));
                    else if (sectionType == 32)//Vertex Pos and Weights
                        if (sectionFlags == 33)
                            for (int c = 0; i < count; c++)
                            {
                                var vertex = new DeformableVertex();
                                vertex._positions = new List<Vector3H>(1);
                                vertex._normals = new List<Vector3H>(1);
                                vertex.Weights = new List<float>(1);
                                vertex.BoneId = new List<int>(1);

                                vertex._positions[0] = new Vector3H(
                                    (decimal)(Input.ReadUInt(16) * FinalScale),
                                    (decimal)(Input.ReadUInt(16) * FinalScale),
                                    (decimal)(Input.ReadUInt(16) * FinalScale));

                                var vertParams = (int)Input.ReadUInt(16);
                                vertex.BoneId[0] = vertParams >> 10;
                                vertex.Weights[0] = (vertParams & 0x1ff) / 256;

                                vertices.Add(vertex);
                            }
                        else if (sectionFlags == 34)
                            for (int c = 0; i < count / 2; c++)
                            {
                                var vertex = new DeformableVertex();
                                vertex._positions = new List<Vector3H>(2);
                                vertex._normals = new List<Vector3H>(2);
                                vertex.Weights = new List<float>(2);
                                vertex.BoneId = new List<int>(2);

                                vertex._positions[0] = new Vector3H(
                                    (decimal)(Input.ReadUInt(16) * FinalScale),
                                    (decimal)(Input.ReadUInt(16) * FinalScale),
                                    (decimal)(Input.ReadUInt(16) * FinalScale));

                                var vertParams = (int)Input.ReadUInt(16);
                                vertex.BoneId[0] = vertParams >> 10;
                                vertex.Weights[0] = (vertParams & 0x1ff) / 256;

                                vertex._positions[1] = new Vector3H(
                                    (decimal)(Input.ReadUInt(16) * FinalScale),
                                    (decimal)(Input.ReadUInt(16) * FinalScale),
                                    (decimal)(Input.ReadUInt(16) * FinalScale));

                                vertParams = (int)Input.ReadUInt(16);
                                vertex.BoneId[1] = vertParams >> 10;
                                vertex.Weights[1] = (vertParams & 0x1ff) / 256;

                                vertices.Add(vertex);
                            }
                    else if (sectionType == 33)
                        ClumpIndex = (int)Input.ReadUInt(32);

                    //Alinhar a 4 bytes
                    while(Input.Position % 0x4 != 0)
                        Input.Position++;
                }
            }
        }

    }
}
