using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using OpenTK;

public static class Helper3D
{
	//Helpful defines
	public const float UV_SCALE = 1.0f / 256.0f;
	public const float WEIGHT_SCALE = 1.0f / 256.0f;
	public const float COLOR_SCALE = 1.0f / 255.0f;
	public const float NORMAL_SCALE = 1.0f / 64.0f;
	//These numbers are finagled...
	public const float VTEX_SCALE = 0.0625f * 0.01f;
	public const float CCS_GLOBAL_SCALE = 0.0625f * 0.1f;
	public const float NINETY_RADS = -90.0f * (float)Math.PI / 180.0f;
	public static bool Vector3LessThan(Vector3 a, Vector3 b)
	{
		return (a.X < b.X) && (a.Y < b.Y) && (a.Z < b.Z);
	}

	public static Vector3 FixAxisRotation(Vector3 input)
	{
		//return input;
		//Works pretty good:
		return new Vector3(input.Z, -input.Y, input.X);

		//return new Vector3(input.Z, input.Y, input.X);
	}

	public static Vector3 UnFixAxisRotation(Vector3 input)
	{
		return new Vector3(input.X, -input.Y, input.Z);
	}

	public static Vector4 FixAxisRotation4(Vector4 input)
	{
		//Quaternion derp = new Quaternion(input.X, input.Y, input.Z, input.W);
		return input;
		//return new Vector4(input.Z, input.X, input.Y, input.W);
	}
	public static Vector2 ReadVec2UV(Stream bStream)
	{
		float u = bStream.ReadUInt(16);
		float v = bStream.ReadUInt(16);
		return new Vector2(u, v);
	}
	public static byte[] GetVec2UV(this Vector2 vec2)
	{
		float u = (vec2.X);
		float v = (vec2.Y);

		Vector2 UV = new Vector2(u, v);

		var result = new List<byte>();

		result.AddRange(BitConverter.GetBytes((Int16)UV.X));
		result.AddRange(BitConverter.GetBytes((Int16)UV.Y));

		return result.ToArray();
	}
	public static Vector3 ReadVec3Rotation(Stream bStream)
	{
		//rx, rZ, rY
		//Actually: rz, rx, -ry
		float rX = bStream.ReadSingle();
		float rY = bStream.ReadSingle();
		float rZ = bStream.ReadSingle();

		//float rY = bStream.ReadSingle();
		//float rZ = bStream.ReadSingle();
		//float rX = bStream.ReadSingle();


		float pi = 3.141592653589793f;
		//float toRads = pi / 180.0f;
		//return new Vector3(-rZ * toRads, rY * toRads, -rX * toRads); //FixAxis(new Vector3(rX, rY, rZ));
		return FixAxisRotation(new Vector3((rX * pi) / 180.0f, (rY * pi) / 180.0f, (rZ * pi) / 180.0f));
	}
	public static byte[] GetVec3Rotation(this Vector3 vec3)
	{
		float pi = 3.141592653589793f;

		Vector3 reductum = FixAxisRotation(new Vector3(
			(vec3.X / pi) * 180.0f,
			(vec3.Y / pi) * 180.0f,
			(vec3.Z / pi) * 180.0f));

		var result = new List<byte>();

		result.AddRange(BitConverter.GetBytes(reductum.X));
		result.AddRange(BitConverter.GetBytes(reductum.Y));
		result.AddRange(BitConverter.GetBytes(reductum.Z));

		return result.ToArray();
	}
	public static Vector3 ReadVec3Position(Stream bStream)
	{
		//Close enough? 
		//float scaleVar = 1.6f;
		//-pX, pZ, pY
		//float pX = -bStream.ReadSingle() * scaleVar;
		//float pZ = bStream.ReadSingle() * scaleVar;
		//float pY = bStream.ReadSingle() * scaleVar;

		//float pY = -bStream.ReadSingle() * scaleVar;
		//float pZ = -bStream.ReadSingle() * scaleVar;
		//float pX = bStream.ReadSingle() * scaleVar;

		float pX = bStream.ReadSingle();

		float pY = bStream.ReadSingle();

		float pZ = bStream.ReadSingle();
		Vector3 result = FixAxis(new Vector3(pX,
			pY,
			pZ));
		;
		return Vector3.Divide(result, 100);
	}

	public static Vector3 ReadVec3(Stream bStream)
	{
		float pX = bStream.ReadSingle();
		float pY = bStream.ReadSingle();
		float pZ = bStream.ReadSingle();

		return new Vector3(pX, pY, pZ);
	}
	public static byte[] GetVec3Position(this Vector3 vec3)
    {
		//float scaleVar = 1.6f;

		Vector3 reductum = FixAxis(Vector3.Multiply(vec3, 100.0f));

		float pX = reductum.X;
		float pY = reductum.Y;
		float pZ = reductum.Z;

		var result = new List<byte>();

		result.AddRange(BitConverter.GetBytes(pX));
		result.AddRange(BitConverter.GetBytes(pY));
		result.AddRange(BitConverter.GetBytes(pZ));

		return result.ToArray();
	}
	public static Vector3 ReadVec3Scale(Stream bStream)
	{
		float sX = bStream.ReadSingle();
		float sY = bStream.ReadSingle();
		float sZ = bStream.ReadSingle();

		return new Vector3(sX, sY, sZ);
	}
	public static byte[] GetVec3(this Vector3 vec3)
	{
		var result = new List<byte>();

		result.AddRange(BitConverter.GetBytes(vec3.X));
		result.AddRange(BitConverter.GetBytes(vec3.Y));
		result.AddRange(BitConverter.GetBytes(vec3.Z));

		return result.ToArray();
	}
	public static Vector2 ReadVec2UV_Gen3(Stream bStream)
	{
		float uvscale2 = 1.0f / 65535.0f;
		float u = (bStream.ReadUInt(32) * uvscale2);
		float v = (bStream.ReadUInt(32) * uvscale2);
		//float u = bStream.ReadSingle();
		//float v = bStream.ReadSingle();

		return new Vector2(u, v);
	}
	public static byte[] GetVec2UV_Gen3(this Vector2 vec2)
	{
		float uvscale2 = 1.0f / 65535.0f;
		float u = (vec2.X / uvscale2);
		float v = (vec2.Y / uvscale2);

		Vector2 UV = new Vector2(u, v);

		var result = new List<byte>();

		result.AddRange(BitConverter.GetBytes(UV.X));
		result.AddRange(BitConverter.GetBytes(UV.Y));

		return result.ToArray();
	}

	public static Vector3 FixAxis(Vector3 input)
	{
		return new Vector3(input.X, input.Y, input.Z);
	}
	public static Vector3 ReadVec3Normal8(Stream bStream)
	{
		float nX = -bStream.ReadByte() * NORMAL_SCALE;
		float nY = bStream.ReadByte() * NORMAL_SCALE;
		float nZ = bStream.ReadByte() * NORMAL_SCALE;

		return new Vector3(nX, nY, nZ);
	}
	public static byte[] GetVec3Normal8(this Vector3 vec3)
	{
		float nX = +vec3.X / NORMAL_SCALE;
		float nY = vec3.Y / NORMAL_SCALE;
		float nZ = vec3.Z / NORMAL_SCALE;

		Vector3 n = new Vector3(nX, nY, nZ);

		var result = new List<byte>();

		result.Add((byte)n.X);
		result.Add((byte)n.Y);
		result.Add((byte)n.Z);

		return result.ToArray();
	}
	public static Vector3 ReadVec3Half(Stream bStream, float scale)
	{
		scale /= 256.0f;
		float vX = bStream.ReadUInt(16) * VTEX_SCALE;
		float vY = bStream.ReadUInt(16) * VTEX_SCALE;
		float vZ = bStream.ReadUInt(16) * VTEX_SCALE;
		return FixAxis(new Vector3(vX * scale, vY * scale, vZ * scale));
	}
	public static byte[] GetVec3Half(this Vector3 vec3, float scale)
	{
		scale /= 256.0f;

		float vX = (vec3.X / VTEX_SCALE) / scale;
		float vY = (vec3.Y / VTEX_SCALE) / scale;
		float vZ = (vec3.Z / VTEX_SCALE) / scale;

		Vector3 n = new Vector3(vX , vY, vZ);

		var result = new List<byte>();

		result.AddRange(BitConverter.GetBytes((Int16)n.X));
		result.AddRange(BitConverter.GetBytes((Int16)n.Y));
		result.AddRange(BitConverter.GetBytes((Int16)n.Z));

		return result.ToArray();
	}

	public static Vector4 FromColor(Color col)
    {
		float a = col.A / COLOR_SCALE;
		if (col.A >= 0x7f) a = 0xff;
		else a = (float)(0x80);
		return new Vector4(col.R / COLOR_SCALE, col.G / COLOR_SCALE, col.B / COLOR_SCALE, a);
    }
	public static Vector4 ReadVec4RGBA32(Stream bStream)
	{
		byte uR = (byte)bStream.ReadByte();
		byte uG = (byte)bStream.ReadByte();
		byte uB = (byte)bStream.ReadByte();
		byte uA = (byte)bStream.ReadByte();
		if (uA >= 0x7f) uA = 0xff;
		else uA = (byte)(uA << 1);

		return new Vector4(uR * COLOR_SCALE, uG * COLOR_SCALE, uB * COLOR_SCALE, uA * COLOR_SCALE);
	}

	public static byte[] GetVec4RGBA32(this Vector4 vec4)
	{
		var result = new List<byte>();

		result.Add((byte)(vec4.X / COLOR_SCALE));
		result.Add((byte)(vec4.Y / COLOR_SCALE));
		result.Add((byte)(vec4.Z / COLOR_SCALE));
		byte uA = (byte)(vec4.W / COLOR_SCALE);
		if (uA == 0xFE) 
			uA = 0x80;
		else 
			uA = (byte)(uA >> 1);
		result.Add(uA);

		return result.ToArray();
	}
}
