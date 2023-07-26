using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
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
	public struct ModelUV
    {
		public int U, V;

		[DisplayName("X")]
		[Description("Modify the vertex uv coordinates.")]
		[Category("Texture Coordinates")]
		public int _x
		{
			get => U;
			set => U = value;
		}
		[DisplayName("Y")]
		[Description("Modify the vertex uv coordinates.")]
		[Category("Texture Coordinates")]
		public int _y
		{
			get => V;
			set => V = value;
		}
		internal static ModelUV Read(Stream input) => new ModelUV()
		{
			U = (int)input.ReadUInt(16),
			V = (int)input.ReadUInt(16)
		};
		    
		internal byte[] GetUV()
        {
			var b = new List<byte>();
			b.AddRange(((uint)U).ToLEBE(16));
			b.AddRange(((uint)V).ToLEBE(16));
			return b.ToArray();
        }
		
	}
	public struct ModelVertex
	{
		public Vector3H Position;
		public Vector3H Position2;
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
			get => Position.X;
			set => Position.X = value;
		}
		[DisplayName("Y")]
		[Description("Modify the vertex position coordinates.")]
		[Category("Spatial Position")]
		public decimal _y
		{
			get => Position.Y;
			set => Position.Y = value;
		}
		[DisplayName("Z")]
		[Description("Modify the vertex position coordinates.")]
		[Category("Spatial Position")]
		public decimal _z
		{
			get => Position.Z;
			set => Position.Z = value;
		}
		[DisplayName("X")]
		[Description("Modify the vertex position coordinates. [2]")]
		[Category("Spatial Position 2")]
		public decimal _x2
		{
			get => Position2.X;
			set => Position2.X = value;
		}
		[DisplayName("Y")]
		[Description("Modify the vertex position coordinates. [2]")]
		[Category("Spatial Position 2")]
		public decimal _y2
		{
			get => Position2.Y;
			set => Position2.Y = value;
		}
		[DisplayName("Z")]
		[Description("Modify the vertex position coordinates. [2]")]
		[Category("Spatial Position 2")]
		public decimal _z2
		{
			get => Position2.Z;
			set => Position2.Z = value;
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
			get => System.Drawing.Color.FromArgb((int)(Color.W), (int)(Color.X), (int)(Color.Y), 
				(int)(Color.Z));
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

		public Model mdlRef;
		public Clump cmpRef;
		public bool useclumpref = false;

		public ModelVertex[] Vertices;
		public ModelTriangle[] Triangles;
		public ModelUV[] UVs;

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
			get => ObjectName;
		}
		[DisplayName("Sub Model Type")]
		[Description("See the sub-model type.")]
		[Category("Model Base")]
		public SubModelType _type
		{
			get => subMDLType;
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
		[Description("See the uv texture coordinates count for the Sub-Model(deformable).")]
		[Category("Model")]
		public uint _uvcount
		{
			get => UVs != null && UVs.Length>0 ? (uint)UVs.Length:
				0;
		}

		[DisplayName("Vertex Count")]
		[Description("See the vertex count for the Sub-Model.")]
		[Category("Model")]
		public uint _vertexcount
		{
			get => (uint)Vertices.Length;
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
		[DisplayName("UVs")]
		[Description("Modify the model uvs.")]
		[Category("Model")]
		public ModelUV[] _uvs
		{
			get => UVs;
			set => UVs = value;
		}
		internal static SubModel Read(Stream Input, Model model, Index ccstoc, Clump cref)
		{
			SubModel subModel = new SubModel();
			subModel._CCStoc = ccstoc;
			subModel.cmpRef = cref;
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

					uint ltindex = Input.ReadUInt(32);
					subModel.ObjectID = model.LT[ltindex];
					subModel.useclumpref = true;
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

						//bool dualFlag = ((Vertex.VertexParams >> 9) & 0x1) == 0;

						//if (dualFlag)
						//{
						//	subModel.Vertices[i].Position2 = Helper3D.ReadVec3Half(Input, model.VertexScale);
						//	subModel.Vertices[i].SecondVertexParams = Input.ReadUInt(16);

						//	weight2 = (subModel.Vertices[i].SecondVertexParams & 0x1ff) * Helper3D.WEIGHT_SCALE;
						//	boneID2 = (subModel.Vertices[i].SecondVertexParams >> 10);
						//}

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
				for (int i = 0,t=1;  i < subModel.VertexCount; i++)
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
                //for (int t = 1; t < subModel.VertexCount-1; t++)
                //{
                //	//TODO: CCSModel: Gen1 CCS Files don't care about vertex winding order (everything is drawn double sided)
                //	// Can we derive proper order from normal direction? Probably not.
                //	TrianglesList.Add(new ModelTriangle(t+2,t+1,t));
                //}
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
					subModel.UVs = new ModelUV[subModel.UVCount];
					for (int i = 0; i < subModel.UVCount; i++)
					{
						subModel.UVs[i] = ModelUV.Read(Input);
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
			UVCount = UVs != null ? (uint)UVs.Length : 0;
			VertexCount = (uint)Vertices.Length;
			TriangleCount = (uint)Vertices.Length;

			if (_mdlType == ModelType.DEFORMABLE ||
				_mdlType == ModelType.DEFORMABLE_GEN2 ||
				_mdlType == ModelType.DEFORMABLE_GEN2_5 ||
				_mdlType == ModelType.DEFORMABLE_GEN2_5_S)
			{
				result.AddRange(MaterialID.ToLEBE(32));

				//Vértice BoneIDs e Weights
				if (subMDLType == SubModelType.DEFORMABLE)
				{
                    result.AddRange(UVCount.ToLEBE(32));
                    result.AddRange(VertexCount.ToLEBE(32));
                    foreach (var vertex in Vertices)
					{
						byte[] vertexBIN = vertex.Position.GetVec3Half(mdlRef.VertexScale);
						result.AddRange(vertexBIN);
						if(vertex.ContainsParams)
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
					result.AddRange(VertexCount.ToLEBE(32));
					result.AddRange(UVCount.ToLEBE(32));
					uint ltindex = (uint)mdlRef.LT.ToList().IndexOf(ObjectID);
					result.AddRange(ltindex.ToLEBE(32));

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
						result.AddRange(uv.GetUV());
					}
				else
					foreach (var vertex in Vertices)
					{
						result.AddRange(vertex.TexCoords.GetVec2UV());
					}
			}
			else if (_mdlType == ModelType.SHADOW)
			{
				TriangleCount = Triangles != null ? (uint)Triangles.Count() : 0;

				result.AddRange(VertexCount.ToLEBE(32));
				result.AddRange((TriangleCount * 3).ToLEBE(32));

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
		internal void GetOBJECT3D(StringBuilder Writer,  out StringBuilder matBuilder, out string mtlname, out Bitmap texture, out string texname)
        {
			Material mat = null;
			Texture tex = null;
			texture = null;

			try
			{
				mat = _CCStoc._ccsf.Blocks.Where(x => x.ObjectID == this.MaterialID).ToArray()[0] as Material;
				tex = _CCStoc._ccsf.Blocks.Where(x => x.ObjectID == mat.TextureID).ToArray()[0] as Texture;
				CLUT clt = _CCStoc._ccsf.Blocks.Where(x => x.ObjectID == tex.CLUTID).ToArray()[0] as CLUT;
				texture = tex.ToBitmap(clt);
			}
			catch (NullReferenceException) { }

			if (mat!=null && tex!=null)
			{
				Writer.AppendLine($"mtllib MAT_{mat.ObjectID}.mtl");
			}

			Writer.AppendLine($"o {(_type == Model.SubModel.SubModelType.DEFORMABLE ? mdlRef.ObjectName : ObjectName)}");
			mtlname = "";
			texname = "";
			matBuilder = null;
			//Vertices, UVs and Normals
			foreach (var vertex in Vertices)
			{
				Writer.Append($"v {vertex.Position.X} {vertex.Position.Y} {vertex.Position.Z}\r\n" +
					$"vt {vertex.TexCoords.X} {vertex.TexCoords.Y}\r\n" +
					$"vn {vertex.Normal.X} {vertex.Normal.Y} {vertex.Normal.Z}\r\n");//Vertices
			}
			//Material
			if (mat != null && tex != null)
			{
				Writer.AppendLine($"usemtl {mat.ObjectName}");
				Writer.AppendLine($"s off");

				mtlname = $"MAT_{mat.ObjectID}";
				texname = tex.ObjectName;

				matBuilder = new StringBuilder();
				matBuilder.AppendLine($"newmtl {mat.ObjectName}");
				matBuilder.AppendLine($"Ka 1.0 1.0 1.0");
				matBuilder.AppendLine($"Kd 1.0 1.0 1.0");
				matBuilder.AppendLine($"Ks 0.0 0.0 0.0");

				matBuilder.AppendLine($"map_Kd {tex.ObjectName}.png");

			}
			//Faces
			foreach (var triangle in Triangles)
				Writer.AppendLine($"f {triangle.ID1 +1}/{triangle.ID1 +1} {triangle.ID2+1}/{triangle.ID2 +1} " +
					$"{triangle.ID3 + 1}/{triangle.ID3 + 1}");//Triangles
		}
		internal void SetfromOBJECT3D(StreamReader reader)
        {
			string entireOBJ = reader.ReadToEnd();
			string[] entries = entireOBJ.Split(new string[] {"\r\n", "#CCSF ASSET EXPLORER - MODEL CONVERTER",
				"#BIT.RAIDEN - 2022",
				$"o {mdlRef.ObjectName}"
				}, StringSplitOptions.RemoveEmptyEntries);
			var verticeList = new List<string>();
			var normalList = new List<string>();
			var uvList = new List<string>();
			foreach (var str in entries)
			{
				if (str.StartsWith("v "))
					verticeList.Add(str);
				else if (str.StartsWith("vn "))
					normalList.Add(str);
				else if (str.StartsWith("vt "))
					uvList.Add(str);
			}

			var vertices = new List<ModelVertex>();
			for (int v = 0; v< verticeList.Count; v++)
            {
				string[] vertexx = verticeList[v].Split(new string[] {"v "," "}, StringSplitOptions.RemoveEmptyEntries);
				string[] vertexNormal = normalList.Count > 0 ? normalList[v].Split(new string[] {"vn "," "}, StringSplitOptions.RemoveEmptyEntries): null;
				string[] uv = uvList.Count > 0 ? uvList[v].Split(new string[] { "vt ", " " }, StringSplitOptions.RemoveEmptyEntries) : null;

				var vertex = new ModelVertex()
				{
					Normal = vertexNormal!=null ? new Vector3(Convert.ToSingle(vertexNormal[0]),
					Convert.ToSingle(vertexNormal[1]),
					Convert.ToSingle(vertexNormal[2])) : Vector3.Zero,

					Position = new Vector3H(Convert.ToDecimal(vertexx[0]),
					Convert.ToDecimal(vertexx[1]),
					Convert.ToDecimal(vertexx[2])),

					TexCoords = uv != null ? new Vector2(Convert.ToSingle(uv[0]),
					Convert.ToSingle(uv[1])): Vector2.Zero
				};
				vertices.Add(vertex);

			}
			VertexCount = (uint)vertices.Count;
			Vertices = vertices.ToArray();
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

	public Clump clumpRef;
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
	[DisplayName("Link Table")]
	[Description("Array of objects index for deformable.")]
	[Category("Model Container")]
	public string[] _lts
	{
		get => Enumerable.Range(0, (int)LTCount).Select
			(x=> _ccsToc.GetObjectName(LT[x])).ToArray();

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
			return Data;
			//return ToArray();
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
			x => SubModel.Read(Input, model, _ccsToc, clumpRef)).ToArray();


		return model;
    }
	public override byte[] ToArray()
	{
		SubModelCount = (uint)SubModels.Length;
		

		var LTb = new List<byte>();
		var SubMDLb = new List<byte>();

#region Generate Fields
		//LT
		if(MDLType == ModelType.DEFORMABLE ||
		   MDLType == ModelType.DEFORMABLE_GEN2 ||
		   MDLType == ModelType.DEFORMABLE_GEN2_5 ||
		   MDLType == ModelType.DEFORMABLE_GEN2_5_S)
		{
			LTCount = LT != null ? (uint)LT.Length : 0;
			//Lookup Table
			foreach (var b in LT)
				LTb.Add((byte)b);
			while (LTb.Count % 0x4 != 0)
				LTb.Add(0);
		}

		//SubModels
		foreach (var submdl in SubModels)
			SubMDLb.AddRange(submdl.ToArray());

#endregion

#region Size Calculation
		Size = 0x14;
		if (_ccsHeader.Version >= Header.CCSFVersion.GEN2)
			Size += 8;
		Size += (uint)LTb.Count;
		Size += (uint)SubMDLb.Count;

		//Deformable type 0x3c thing
		if (MDLType == ModelType.DEFORMABLE ||
		   MDLType == ModelType.DEFORMABLE_GEN2 ||
		   MDLType == ModelType.DEFORMABLE_GEN2_5 ||
		   MDLType == ModelType.DEFORMABLE_GEN2_5_S)
			Size += (uint)(0x3c * SubModelCount);
#endregion

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

		result.AddRange(LTb.ToArray());
		result.AddRange(SubMDLb.ToArray());
		return result.ToArray();
	}

    public override void SetIndexes(Index.ObjectStream Object, Index.ObjectStream[] AllObjects)
    {
		ObjectID = (uint)Object.ObjIndex;
	}

}
