using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using OpenTK;
using static Helper3D;
using static IOextent;
using System.Text;
using System.ComponentModel;

public class Model : Block
{
	public enum ModelType: ushort
    {
		NORMAL = 0x0,
		NORMAL_GEN2_5 = 0x3800,
		NORMAL_GEN2_5_s = 0x3801,
		DEFORMABLE = 0x4,
		SHADOW = 0x8,
		MORPHTARGET = 0x600,
		DEFORMABLE_GEN2 = 0x1004,
		DEFORMABLE_GEN2_5 = 0x3804,
		DEFORMABLE_GEN2_5_S = 0x3004,
		RIGID_GEN2_NO_COLOR = 0x0200,
		RIGID_GEN2_COLOR = 0x1000,
		RIGID_GEN2_NO_COLOR2 = 0x1200,
		MORPHTARGET_GEN2 = 0x400,
		UNKNOW
	};

	#region Sub Structures
	public struct BoneID
	{
		public int Bone1; // = 0;
		public int Bone2; // = 0;
		public int Bone3;
		public int Bone4;

		public BoneID(int id1, int id2, int id3 = 0, int id4 = 0)
		{
			Bone1 = id1;
			Bone2 = id2;
			Bone3 = id3;
			Bone4 = id4;
		}
	}

	public struct ModelVertex
	{
		public Vector3 Position;
		public Vector3 Position2;
		public Vector3 Position3;   //Last Recode Compatibility
		public Vector3 Position4;   //Last Recode Compatibility

		public Vector2 TexCoords;

		public Vector4 Color;
		public Vector3 Normal;

		public BoneID BoneIDs;
		public Vector4 Weights;

		public byte TriFlag;
		public float VScale;
		public uint VertexParams;
		public uint SecondVertexParams;

		public bool ContainsParams;

		[DisplayName("X")]
		[Description("Modify the vertex position coordinates.")]
		[Category("Spatial Position")]
		public decimal _x
		{
			get => (decimal)Position.X;
			set => Position.X = (float)value;
		}
		[DisplayName("Y")]
		[Description("Modify the vertex position coordinates.")]
		[Category("Spatial Position")]
		public decimal _y
		{
			get => (decimal)Position.Y;
			set => Position.Y = (float)value;
		}
		[DisplayName("Z")]
		[Description("Modify the vertex position coordinates.")]
		[Category("Spatial Position")]
		public decimal _z
		{
			get => (decimal)Position.Z;
			set => Position.Z = (float)value;
		}
		[DisplayName("X")]
		[Description("Modify the vertex position coordinates. [2]")]
		[Category("Spatial Position 2")]
		public decimal _x2
		{
			get => (decimal)Position2.X;
			set => Position2.X = (float)value;
		}
		[DisplayName("Y")]
		[Description("Modify the vertex position coordinates. [2]")]
		[Category("Spatial Position 2")]
		public decimal _y2
		{
			get => (decimal)Position2.Y;
			set => Position2.Y = (float)value;
		}
		[DisplayName("Z")]
		[Description("Modify the vertex position coordinates. [2]")]
		[Category("Spatial Position 2")]
		public decimal _z2
		{
			get => (decimal)Position2.Z;
			set => Position2.Z = (float)value;
		}
		[DisplayName("X")]
		[Description("Modify the vertex normal.")]
		[Category("Normal")]
		public decimal _normalx
		{
			get => (decimal)Normal.X;
			set => Normal.X = (float)value;
		}
		[DisplayName("Y")]
		[Description("Modify the vertex normal.")]
		[Category("Normal")]
		public decimal _normaly
		{
			get => (decimal)Normal.Y;
			set => Normal.Y = (float)value;
		}
		[DisplayName("Z")]
		[Description("Modify the vertex normal.")]
		[Category("Normal")]
		public decimal _normalz
		{
			get => (decimal)Normal.Z;
			set => Normal.Z = (float)value;
		}
		[DisplayName("Color")]
		[Description("Modify the vertex color.")]
		[Category("Vertex")]
		public Color _color
		{
			get => System.Drawing.Color.FromArgb((int)(Color.W * Helper3D.COLOR_SCALE), (int)(Color.X * Helper3D.COLOR_SCALE), (int)(Color.Y * Helper3D.COLOR_SCALE), 
				(int)(Color.Z * Helper3D.COLOR_SCALE));
			set => Color = Helper3D.FromColor(value);
		}
		internal static ModelVertex ReadVertex(Stream Input, float VertexScale, int boneID, bool containsParams = false) => new ModelVertex()
		{
			VScale = VertexScale,
			Color = new Vector4(0.5f, 0.5f, 0.5f, 1.0f),
			Position = ReadVec3Half(Input, VertexScale),
			ContainsParams = containsParams,
			VertexParams = containsParams == true ? Input.ReadUInt(16) : 0,
			Weights = new Vector4(1.0f, 0.0f, 0.0f, 0.0f),
			BoneIDs = new BoneID(boneID, 0)
		};

	}
	public struct ModelTriangle
	{
		public int ID1; // = 0;
		public int ID2; // = 1;
		public int ID3; // = 2;

		public ModelTriangle(int _id1, int _id2, int _id3)
		{
			ID1 = _id1;
			ID2 = _id2;
			ID3 = _id3;
		}
	}

	public class SubModel
	{
		public Index _CCStoc;

		public string ObjectName, MaterialObjName;
		public uint ObjectID = 0xFFFFFFFF; 
		public uint MaterialID;
		public uint UVCount;

		public uint VertexCount;
		public uint TriangleCount;

		Model mdlRef;

		public ModelVertex[] Vertices;
		public Vector2[] UVs;
		public ModelTriangle[] Triangles;
		public SubModelType subMDLType = SubModelType.DEFORMABLE;

		public Model.ModelType _mdlType;
		public enum SubModelType
        {
			RIGID,
			DEFORMABLE
        };

		[DisplayName("Object Index")]
		[Description("Define the object index for the Sub-Model.")]
		[Category("Model Base")]
		public uint _ObjectIndex
		{
			get => ObjectID;
			set => ObjectID = value;
		}
		[DisplayName("Object Name")]
		[Description("See the object name for the Sub-Model.")]
		[Category("Model Base")]
		public string _ObjectName
		{
			get => _CCStoc.GetObjectName(ObjectID);
		}
		[DisplayName("Linked Material Index")]
		[Description("Define the material index for the Sub-Model.")]
		[Category("Model Base")]
		public uint MatIndex
		{
			get => MaterialID;
			set => MaterialID = value;
		}
		[DisplayName("Linked Material Name")]
		[Description("See the linked material name for the Sub-Model.")]
		[Category("Model Base")]
		public string MatName
		{
			get => _CCStoc.GetObjectName(MaterialID);
		}

		[DisplayName("UV Count")]
		[Description("See the uv texture coordinates count for the Sub-Model.")]
		[Category("Model")]
		public uint _uvcount
		{
			get => UVCount;
		}

		[DisplayName("Vertex Count")]
		[Description("See the vertex count for the Sub-Model.")]
		[Category("Model")]
		public uint _vertexcount
		{
			get => VertexCount;
		}
		[DisplayName("Triangle Count")]
		[Description("See the triangles/faces count for the Sub-Model.")]
		[Category("Model")]
		public uint _tricount
		{
			get => TriangleCount;
		}

		[DisplayName("Vertices")]
		[Description("Modify the model vertices.")]
		[Category("Model")]
		public ModelVertex[] _vertex
		{
			get => Vertices;
			set => Vertices = value;
		}
		internal static SubModel Read(Stream Input, Model model, Index ccstoc)
		{
			SubModel subModel = new SubModel();
			subModel._CCStoc = ccstoc;
			subModel.mdlRef = model;
			subModel._mdlType = model.MDLType;
			if (model.MDLType==ModelType.DEFORMABLE ||
				model.MDLType==ModelType.DEFORMABLE_GEN2||
				model.MDLType==ModelType.DEFORMABLE_GEN2_5 ||
				model.MDLType==ModelType.DEFORMABLE_GEN2_5_S)
            {
				subModel.subMDLType = SubModelType.DEFORMABLE;

				//Console.WriteLine($"Leitor de Modelos/sub, posição: 0x{Input.Position.ToString("X2")}");

				subModel.MaterialID = Input.ReadUInt(32);
				subModel.UVCount = Input.ReadUInt(32);
				subModel.VertexCount = Input.ReadUInt(32);

				//SubModel SubType
				if (subModel.VertexCount == 0)
				{
					subModel.subMDLType = SubModelType.RIGID;

					subModel.VertexCount = subModel.UVCount;
					subModel.UVCount = 0;

					subModel.ObjectID = Input.ReadUInt(32);
				}

				//Vértice BoneIDs e Weights
				if (subModel.subMDLType == SubModelType.DEFORMABLE)
				{
					subModel.Vertices = new ModelVertex[subModel.VertexCount];
					for (int i = 0; i < subModel.VertexCount; i++)
					{
						subModel.Vertices[i].Position = ReadVec3Half(Input, model.VertexScale);
						subModel.Vertices[i].VertexParams = Input.ReadUInt(16);
						subModel.Vertices[i].ContainsParams = true;

						var Vertex = subModel.Vertices[i];

						uint boneID1 = Vertex.VertexParams >> 10;
						uint boneID2 = 0;

						float weight1 = (Vertex.VertexParams & 0x1ff) * Helper3D.WEIGHT_SCALE;
						float weight2 = 0;

						bool dualFlag = ((Vertex.VertexParams >> 9) & 0x1) == 0;

						if (dualFlag)
						{
							subModel.Vertices[i].Position2 = Helper3D.ReadVec3Half(Input, model.VertexScale);
							subModel.Vertices[i].SecondVertexParams = Input.ReadUInt(16);

							weight2 = (subModel.Vertices[i].SecondVertexParams & 0x1ff) * Helper3D.WEIGHT_SCALE;
							boneID2 = (subModel.Vertices[i].SecondVertexParams >> 10);
						}

						if (model.LT != null && 
							boneID1 < model.LTCount &&
							boneID2 < model.LTCount)
						{
							subModel.Vertices[i].BoneIDs = new BoneID((int)model.LT[boneID1],
								(int)model.LT[boneID2]);
						}
						else
						{
							subModel.Vertices[i].BoneIDs = new BoneID((int)boneID1, (int)boneID2);
						}

						subModel.Vertices[i].Weights = new Vector4(weight1, weight2, 0.0f, 0.0f);

						//Set Color
						subModel.Vertices[i].Color = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
					}
				}
				else if(subModel.subMDLType==SubModelType.RIGID)
                {
					int boneID = 0;//ParentFile.SearchClump(ParentFile.LastObject.ObjectID);

					subModel.Vertices = Enumerable.Range(0, (int)subModel.VertexCount).Select(
					x => ModelVertex.ReadVertex(Input, model.VertexScale, boneID
					)).ToArray();

					//Resolve Padding of %4
					while (Input.Position % 4 != 0)
						Input.Position++;
				}

				//Faces/Triângulos
				var TrianglesList = new List<ModelTriangle>();
				byte lastFlag = 1;
				int sCount = 0;
				for (int i = 0; i < subModel.VertexCount; i++)
				{
					subModel.Vertices[i].Normal = Helper3D.ReadVec3Normal8(Input);
					byte triFlag = (byte)Input.ReadByte();
					subModel.Vertices[i].TriFlag = triFlag;
					//TODO: CCSModel: Gen1 CCS Files don't care about vertex winding order (everything is drawn double sided)
					// Can we derive proper order from normal direction? Probably not.
					if (triFlag == 0)
					{
						if ((sCount % 2) == 0)
						{
							TrianglesList.Add(new ModelTriangle(i, i - 1, i - 2));
						}
						else
						{
							TrianglesList.Add(new ModelTriangle(i - 2, i - 1, i));
						}
						sCount += 1;
						lastFlag = triFlag;
					}
					else
					{
						if (lastFlag == 0)
						{
							sCount = 0;
						}
					}
				}

				subModel.TriangleCount = (uint)TrianglesList.Count();
				subModel.Triangles = TrianglesList.ToArray();

				//Coordenadas de UV
				if (subModel.subMDLType == SubModelType.RIGID)
				{
					for (int i = 0; i < subModel.VertexCount; i++)
					{
						subModel.Vertices[i].TexCoords = Helper3D.ReadVec2UV(Input);
					}
				}
				else
				{
					subModel.UVs = new Vector2[subModel.UVCount];
					Input.Position -= subModel.UVCount * 4;
					for (int i = 0; i < subModel.UVCount; i++)
					{
						subModel.UVs[i] = Helper3D.ReadVec2UV(Input);
					}
				}
			}
			else if (model.MDLType == ModelType.SHADOW)
			{
				subModel.VertexCount = Input.ReadUInt(32);
				subModel.TriangleCount = Input.ReadUInt(32);

				//Vértices
				subModel.Vertices = Enumerable.Range(0, (int)subModel.VertexCount).Select(
					x => ModelVertex.ReadVertex(Input, model.VertexScale, 0
					)).ToArray();
				//Resolve Padding of %4
				while (Input.Position % 4 != 0)
					Input.Position++;

				//Faces/Triângulos
				subModel.Triangles = new ModelTriangle[subModel.TriangleCount / 3];
				for (int i = 0; i < subModel.TriangleCount / 3; i++)
					subModel.Triangles[i] = new ModelTriangle()
					{
						ID1 = (int)Input.ReadUInt(32),
						ID2 = (int)Input.ReadUInt(32),
						ID3 = (int)Input.ReadUInt(32)
					};
			}
			else
            {
				//Console.WriteLine($"Leitor de Modelos/sub, posição: 0x{Input.Position.ToString("X2")}");

				subModel.ObjectID = Input.ReadUInt(32);
				subModel.MaterialID = Input.ReadUInt(32);
				subModel.VertexCount = Input.ReadUInt(32);

				int boneID = 0;//ParentFile.SearchClump(ParentFile.LastObject.ObjectID);

				//Vértices
				subModel.Vertices = Enumerable.Range(0, (int)subModel.VertexCount).Select(
					x => ModelVertex.ReadVertex(Input, model.VertexScale, boneID
					)).ToArray();
				//Resolve Padding of %4
				while (Input.Position % 4 != 0)
					Input.Position++;

				//Faces/Triângulos
				var TrianglesList = new List<ModelTriangle>();
				byte lastFlag = 1;
				int sCount = 0;
				for (int i = 0; i < subModel.VertexCount; i++)
				{
					subModel.Vertices[i].Normal = Helper3D.ReadVec3Normal8(Input);
					byte triFlag = (byte)Input.ReadByte();
					subModel.Vertices[i].TriFlag = triFlag;
					//TODO: CCSModel: Gen1 CCS Files don't care about vertex winding order (everything is drawn double sided)
					// Can we derive proper order from normal direction? Probably not.
					if (triFlag == 0)
					{
						if ((sCount % 2) == 0)
						{
							TrianglesList.Add(new ModelTriangle(i, i - 1, i - 2));
						}
						else
						{
							TrianglesList.Add(new ModelTriangle(i - 2, i - 1, i));
						}
						sCount += 1;
						lastFlag = triFlag;
					}
					else
					{
						if (lastFlag == 0)
						{
							sCount = 0;
						}
					}
				}

				subModel.TriangleCount = (uint)TrianglesList.Count();
				subModel.Triangles = TrianglesList.ToArray();

				//Cores de Vértice
				if (model.MDLType < ModelType.DEFORMABLE ||
		model.MDLType == ModelType.RIGID_GEN2_COLOR ||
		model.MDLType == ModelType.MORPHTARGET_GEN2 ||
		model.MDLType == ModelType.NORMAL_GEN2_5 ||
		model.MDLType == ModelType.NORMAL_GEN2_5_s)
				{
					for (int i = 0; i < subModel.VertexCount; i++)
					{
						subModel.Vertices[i].Color = Helper3D.ReadVec4RGBA32(Input);
					}
				}

				//Coordenadas de UV
				if (model.MDLType != ModelType.MORPHTARGET &&
					model.MDLType != ModelType.MORPHTARGET_GEN2)
				{
					for (int i = 0; i < subModel.VertexCount; i++)
					{
						subModel.Vertices[i].TexCoords = Helper3D.ReadVec2UV(Input);
					}
				}


			}

			return subModel;
		}

		internal byte[] ToArray()
        {
			var result = new List<byte>();
			if (_mdlType == ModelType.DEFORMABLE ||
				_mdlType == ModelType.DEFORMABLE_GEN2 ||
				_mdlType == ModelType.DEFORMABLE_GEN2_5 ||
				_mdlType == ModelType.DEFORMABLE_GEN2_5_S)
			{
					result.AddRange(MaterialID.ToLEBE(32));
					result.AddRange(UVCount.ToLEBE(32));
					result.AddRange(VertexCount.ToLEBE(32));

				if(subMDLType==SubModelType.RIGID)
					result.AddRange(ObjectID.ToLEBE(32));


				//Vértice BoneIDs e Weights
				if (subMDLType == SubModelType.DEFORMABLE)
				{
					foreach(var vertex in Vertices)
					{
						byte[] vertexBIN = vertex.Position.GetVec3Half(mdlRef.VertexScale);
						result.AddRange(vertexBIN);
						result.AddRange(vertex.VertexParams.ToLEBE(16));

                        //bool dualFlag = ((vertex.VertexParams >> 9) & 0x1) == 0;

                        //if (dualFlag)
                        //{
                        //    result.AddRange(vertex.Position2.GetVec3Half(vertex.VScale));
                        //    result.AddRange(vertex.SecondVertexParams.ToLEBE(16));
                        //}

                    }
				}
				else if (subMDLType == SubModelType.RIGID)
				{
					foreach (var vertex in Vertices)
					{
						result.AddRange(vertex.Position.GetVec3Half(vertex.VScale));
						if(vertex.ContainsParams)
							result.AddRange(vertex.VertexParams.ToLEBE(16));
					}

					//Resolve Padding of %4
					while (result.Count % 4 != 0)
						result.Add(0);
				}

				//Faces/Triângulos
				foreach (var vertex in Vertices)
				{
					result.AddRange(vertex.Normal.GetVec3Normal8());
					result.Add(vertex.TriFlag);
				}

				if (subMDLType == SubModelType.DEFORMABLE)
					foreach (var uv in UVs)
					{
						result.AddRange(uv.GetVec2UV());
					}
				else
					foreach (var vertex in Vertices)
					{
						result.AddRange(vertex.TexCoords.GetVec2UV());
					}
			}
			else if (_mdlType == ModelType.SHADOW)
			{
				result.AddRange(VertexCount.ToLEBE(32));
				result.AddRange(TriangleCount.ToLEBE(32));

				//Vértices
				foreach (var vertex in Vertices)
				{
					result.AddRange(vertex.Position.GetVec3Half(vertex.VScale));
				}

				//Resolve Padding of %4
				while (result.Count % 4 != 0)
					result.Add(0);

				//Faces/Triângulos
				foreach (var triangle in Triangles)
				{
					result.AddRange(((uint)(triangle.ID1)).ToLEBE(32));
					result.AddRange(((uint)(triangle.ID2)).ToLEBE(32));
					result.AddRange(((uint)(triangle.ID3)).ToLEBE(32));
				}
			}
            else //Verified Normal Type Work!!
            {
				result.AddRange(ObjectID.ToLEBE(32));
				result.AddRange(MaterialID.ToLEBE(32));
				result.AddRange(VertexCount.ToLEBE(32));

				//Vértices
				foreach (var vertex in Vertices)
				{
					result.AddRange(vertex.Position.GetVec3Half(vertex.VScale));
					if(vertex.ContainsParams)
						result.AddRange(vertex.VertexParams.ToLEBE(16));
				}

				//Resolve Padding of %4
				while (result.Count % 4 != 0)
					result.Add(0);

				//Faces/Triângulos
				foreach (var vertex in Vertices)
				{
					result.AddRange(vertex.Normal.GetVec3Normal8());
					result.Add(vertex.TriFlag);
				}

				//Cores de Vértice
				if (_mdlType < ModelType.DEFORMABLE ||
		_mdlType == ModelType.RIGID_GEN2_COLOR ||
		_mdlType == ModelType.MORPHTARGET_GEN2 ||
		_mdlType == ModelType.NORMAL_GEN2_5 ||
		_mdlType == ModelType.NORMAL_GEN2_5_s)
				{
					foreach (var vertex in Vertices)
					{
						result.AddRange(vertex.Color.GetVec4RGBA32());
					}
				}

				//Coordenadas de UV
				if (_mdlType != ModelType.MORPHTARGET &&
					_mdlType != ModelType.MORPHTARGET_GEN2)
				{
					foreach (var vertex in Vertices)
					{
						result.AddRange(vertex.TexCoords.GetVec2UV());
					}
				}
			}

			return result.ToArray();
        }
		internal void GetOBJECT3D(StringBuilder Writer, bool exportMaterial = true)
        {
			foreach (var vertex in Vertices)
				Writer.Append($"v {vertex.Position.X} {vertex.Position.Y} {vertex.Position.Z}\r\n" +
					$"vt {vertex.TexCoords.X} {vertex.TexCoords.Y}\r\n" +
					$"vn {vertex.Normal.X} {vertex.Normal.Y} {vertex.Normal.Z}\r\n");//Vertices

			foreach (var triangle in Triangles)
				Writer.Append($"f {triangle.ID1 + 1}/{triangle.ID1 + 1} {triangle.ID2 + 1}/{triangle.ID2 + 1} " +
					$"{triangle.ID3 + 1}/{triangle.ID3 + 1}\r\n");//Triangles
		}
	}
    #endregion

    public float VertexScale;

	public ModelType MDLType;

	public uint SubModelCount; 

	public uint DrawFlags;
	public uint UnkFlags;

	public uint[] LT;
	public uint LTCount;

	public float Unknow1, Unknow2;

	public SubModel[] SubModels;

	[DisplayName("Vertex Scale")]
	[Description("Define the vertex scale for the Sub-Models.")]
	[Category("Model Container")]
	public decimal _vscale
	{
		get => (decimal)VertexScale;
		set => VertexScale = (float)value;
	}
	[DisplayName("Unknow 1")]
	[Description("???")]
	[Category("Model Container")]
	public decimal _unk1
	{
		get => (decimal)Unknow1;
		set => Unknow1 = (float)value;
	}
	[DisplayName("Unknow 2")]
	[Description("???")]
	[Category("Model Container")]
	public decimal _unk2
	{
		get => (decimal)Unknow2;
		set => Unknow2 = (float)value;
	}
	[DisplayName("Model Container Type")]
	[Description("See the type of container of models.")]
	[Category("Model Container")]
	public ModelType _mdltype
	{
		get => MDLType;
	}
	[DisplayName("Sub-Model Count")]
	[Description("See the count of sub-models.")]
	[Category("Model Container")]
	public uint _submdlcount
	{
		get => SubModelCount;
	}
	[DisplayName("Sub-Models")]
	[Description("Array of sub-models.")]
	[Category("Model Container")]
	public SubModel[] _submodels
	{
		get => SubModels;
		set => SubModels = value;
	}
	public override byte[] DataArray
	{
		get
		{
			var writer = new BinaryWriter(new MemoryStream(Data));

			writer.BaseStream.Position = 0xC;

			writer.Write(BitConverter.GetBytes(VertexScale));
			writer.Write((ushort)MDLType);
			writer.Write((ushort)SubModelCount);
			writer.Write((ushort)DrawFlags);
			writer.Write((ushort)UnkFlags);
			writer.Write(LTCount);

			if(_ccsHeader.Version >= Header.CCSFVersion.GEN2)
            {
				writer.Write(BitConverter.GetBytes((Single)Unknow1));
				writer.Write(BitConverter.GetBytes((Single)Unknow2));
			}

			if(LT!=null&&LT.Length>0)
            {
				foreach (var u in LT)
					writer.Write((byte)u);

				if (writer.BaseStream.Position % 4 != 0)
					writer.BaseStream.Position++;
            }

			foreach (var submodel in SubModels)
			{
				writer.Write(submodel.ToArray());
			}
			return Data;
		}
	}
	public override Block ReadBlock(Stream Input, Header header)
    {
		_ccsHeader = header;
		Model model = new Model();

		model.Type = Input.ReadUInt(32);
		model.Size = Input.ReadUInt(32);
		model.ObjectID = Input.ReadUInt(32);

		Input.Position = 0xC;

		model.VertexScale = new BinaryReader(Input).ReadSingle();
		model.MDLType = (ModelType)(ushort)Input.ReadUInt(0x10, 16);

		model.SubModelCount = Input.ReadUInt(0x12, 16);

		model.DrawFlags = Input.ReadUInt(0x14, 16);
		model.UnkFlags = Input.ReadUInt(0x16, 16);

		model.LTCount = Input.ReadUInt(0x18, 32);

		if (_ccsHeader.Version >= Header.CCSFVersion.GEN2)
		{
			model.Unknow1 = BitConverter.ToSingle(Input.ReadBytes(0x1c,4),0);
			model.Unknow2 = BitConverter.ToSingle(Input.ReadBytes(0x20, 4), 0);
			Input.Position = 0x24;
		}
		else
		{
			Input.Position = 0x1c;
		}


		if (model.MDLType == ModelType.DEFORMABLE ||
			model.MDLType == ModelType.DEFORMABLE_GEN2 ||
			model.MDLType == ModelType.DEFORMABLE_GEN2_5||
			model.MDLType == ModelType.DEFORMABLE_GEN2_5_S)
		{
			//Lookup Table
			long oldPos = Input.Position;
			model.LT = new uint[model.LTCount];
			for (int i = 0; i < model.LTCount; i++)
				model.LT[i] = (uint)Input.ReadByte();
			Input.Position = oldPos + model.LTCount;

			//LookupTable Padding
			while ((float)((decimal)Input.Position / 4) != (int)(Input.Position / 4))
				Input.Position++;
		}

		model.SubModels = Enumerable.Range(0, (int)model.SubModelCount).Select(
			x => SubModel.Read(Input, model, _ccsToc)).ToArray();


		return model;
    }
	public override byte[] ToArray()
	{
		var result = new List<byte>();
		result.AddRange(Type.ToLEBE(32));
		result.AddRange((Size / 4).ToLEBE(32));
		result.AddRange(ObjectID.ToLEBE(32));

		result.AddRange(BitConverter.GetBytes(VertexScale));
		result.AddRange(((uint)MDLType).ToLEBE(16));

		result.AddRange(SubModelCount.ToLEBE(16));
		result.AddRange(DrawFlags.ToLEBE(16));
		result.AddRange(UnkFlags.ToLEBE(16));
		result.AddRange(LTCount.ToLEBE(32));

		if (_ccsHeader.Version >= Header.CCSFVersion.GEN2)
		{
			result.AddRange(BitConverter.GetBytes((Single)Unknow1));
			result.AddRange(BitConverter.GetBytes((Single)Unknow2));
		}
		if (MDLType == ModelType.DEFORMABLE ||
			MDLType == ModelType.DEFORMABLE_GEN2 ||
			MDLType == ModelType.DEFORMABLE_GEN2_5 ||
			MDLType == ModelType.DEFORMABLE_GEN2_5_S)
		{
			//Lookup Table
			foreach (var b in LT)
				result.Add((byte)b);
			while (result.Count % 0x4 != 0)
				result.Add(0);
		}

		//SubModels
		foreach (var submdl in SubModels)
			result.AddRange(submdl.ToArray());

		return result.ToArray();
	}

    public override void SetIndexes(Index.ObjectStream Object, Index.ObjectStream[] AllObjects)
    {
		ObjectID = (uint)Object.ObjIndex;
		//for (int s = 0; s < SubModelCount; s++)
		//{
		//	if(SubModels[s].ObjectID!= 0xFFFFFFFF)
		//		SubModels[s].ObjectID = (uint)AllObjects.Where(x => x.ObjName == SubModels[s].ObjectName).ToArray()[0].ObjIndex;

		//	bool stop = false;
		//	foreach (var obj in AllObjects)
		//		if (obj.ObjName == SubModels[s].MaterialObjName)
		//			for (int b = 0; stop != true &&
		//				b < obj.Blocks.Count; b++)
		//			{
		//				if (obj.Blocks[b].ReadUInt(0, 32) == 0xcccc0200)
		//				{
		//					SubModels[s].MaterialID = obj.Blocks[b].ReadUInt(8, 32);
		//					stop = true;
		//				}
		//			}
		//}
	}

}
