using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

public struct Vector3H
{
	public decimal X, Y, Z;
	public static Vector3H Zero
	{
		get => new Vector3H(0.0m, 0.0m, 0.0m);
	}
	public static Vector3H One
	{
		get => new Vector3H(1.0m, 1.0m, 1.0m);
	}
	public Vector3H(decimal _X, decimal _Y, decimal _Z)
    {
		X = _X;
		Y = _Y;
		Z = _Z;
	}
	
	internal Vector3H Divide(decimal divisor)
    {
		return new Vector3H(X / divisor, Y / divisor, Z / divisor);
    }
	internal Vector3H Multiply(decimal multiplier)
	{
		return new Vector3H(X * multiplier, Y * multiplier, Z * multiplier);
	}
	internal byte[] GetBytes()
    {
		var b = new List<byte>();
		b.AddRange(BitConverter.GetBytes(Decimal.ToSingle(X)));
		b.AddRange(BitConverter.GetBytes(Decimal.ToSingle(Y)));
		b.AddRange(BitConverter.GetBytes(Decimal.ToSingle(Z)));
		return b.ToArray();
    }
	internal byte[] GetHalfBytes()
	{
		var b = new List<byte>();
		b.AddRange(BitConverter.GetBytes(Decimal.ToInt16(X)));
		b.AddRange(BitConverter.GetBytes(Decimal.ToInt16(Y)));
		b.AddRange(BitConverter.GetBytes(Decimal.ToInt16(Z)));
		return b.ToArray();
	}
}

public static class Helper3D
{
	
	//Helpful defines
	public const float UV_SCALE = 1.0f / 256.0f;
	public const float WEIGHT_SCALE = 1.0f / 256.0f;
	public const float COLOR_SCALE = 1.0f / 255.0f;
	public const float NORMAL_SCALE = 1.0f / 64.0f;
	//These numbers are finagled...
	public const decimal VTEX_SCALE = 256.0m;
	public const float CCS_GLOBAL_SCALE = 256.0f;
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
		float u = Convert.ToSingle(bStream.ReadUInt(16))* UV_SCALE;
		float v = Convert.ToSingle(bStream.ReadUInt(16)) * UV_SCALE;
		return new Vector2(u, v);
	}
	public static byte[] GetVec2UV(this Vector2 vec2)
	{
		var result = new List<byte>();
		float u = vec2.X / UV_SCALE;
		float v = vec2.Y / UV_SCALE;
		result.AddRange(BitConverter.GetBytes((Int16)u));
		result.AddRange(BitConverter.GetBytes((Int16)v));

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
		return new Vector3(rX, rY, rZ );
	}
	public static byte[] GetVec3Rotation(this Vector3 vec3)
	{
		float pi = 3.141592653589793f;

		Vector3 reductum = new Vector3(
			(vec3.X ) ,
			(vec3.Y ) ,
			(vec3.Z ) );

		var result = new List<byte>();

		result.AddRange(BitConverter.GetBytes(reductum.X));
		result.AddRange(BitConverter.GetBytes(reductum.Y));
		result.AddRange(BitConverter.GetBytes(reductum.Z));

		return result.ToArray();
	}
	public static Vector3 ReadVec3Position(Stream bStream)
	{
		//Close enough? 
		float scaleVar = 1.6f;
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
		Vector3 result = new Vector3(pX,
			pY,
			pZ);
		;
		return Vector3.Divide(result,100.0f);
	}
	public static Vector3 ReadVec3(Stream bStream)
	{
		float pX = bStream.ReadSingle();
		float pY = bStream.ReadSingle();
		float pZ = bStream.ReadSingle();

		return new Vector3(pX, pY, pZ);
	}
	public static Vector3H ReadVec3H(Stream bStream)
	{
		decimal pX = new Decimal(bStream.ReadSingle());
		decimal pY = new Decimal(bStream.ReadSingle());
		decimal pZ = new Decimal(bStream.ReadSingle());

		return new Vector3H(pX, pY, pZ);
	}
	public static byte[] GetVec3Position(this Vector3 vec3)
    {
		//float scaleVar = 1.6f;

		Vector3 reductum = Vector3.Multiply(vec3,100.0f);

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
		byte bx = (byte)bStream.ReadByte();
		byte by = (byte)bStream.ReadByte();
		byte bz = (byte)bStream.ReadByte();
		float nX = -bx * NORMAL_SCALE;
		float nY = by * NORMAL_SCALE;
		float nZ = bz * NORMAL_SCALE;

		return new Vector3(nX, nY, nZ);
	}
	public static byte[] GetVec3Normal8(this Vector3 vec3)
	{
		float nX = -vec3.X / NORMAL_SCALE;
		float nY = vec3.Y / NORMAL_SCALE;
		float nZ = vec3.Z / NORMAL_SCALE;

		Vector3 n = new Vector3(nX, nY, nZ);

		var result = new List<byte>();

		result.Add((byte)n.X);
		result.Add((byte)n.Y);
		result.Add((byte)n.Z);

		return result.ToArray();
	}
	public static Vector3H ReadVec3Half(Stream bStream, float scale)
	{
		//scale /= 256.0f;
		decimal nx = new Decimal(Convert.ToSingle((short)bStream.ReadUInt(16)));
		decimal ny = new Decimal(Convert.ToSingle((short)bStream.ReadUInt(16)));
		decimal nz = new Decimal(Convert.ToSingle((short)bStream.ReadUInt(16)));

		return new Vector3H(nx, ny, nz).Divide(VTEX_SCALE);
	}
	public static byte[] GetVec3Half(this Vector3H vec3, float scale)
	{
		var result = new List<byte>();
		
		result.AddRange(vec3.Multiply(VTEX_SCALE).GetHalfBytes());

		byte[] res = result.ToArray();

		return res;
	}
	public static Quaternion ReadVec4(Stream input) => new Quaternion(input.ReadSingle(), input.ReadSingle()
		, input.ReadSingle(), input.ReadSingle());
	public static byte[] GetVec4(this Quaternion quat)
    {
		var b = new List<byte>();
		b.AddRange(quat.Xyz.GetVec3());
		b.AddRange(BitConverter.GetBytes(quat.W));
		return b.ToArray();
    }
	public static Vector4 FromColor(Color col)
    {
		float a = (float)col.A;
		if (col.A >= 0x7f) a = 0xff;
		else a = (float)(0x80);
		return new Vector4(col.R, col.G, col.B, a);
    }
	public static Vector4 ReadVec4RGBA32(Stream bStream)
	{
		byte uR = (byte)bStream.ReadByte();
		byte uG = (byte)bStream.ReadByte();
		byte uB = (byte)bStream.ReadByte();
		byte uA = (byte)bStream.ReadByte();
		if (uA >= 0x7f) uA = 0xff;
		else uA = (byte)(uA << 1);

		return new Vector4(uR, uG, uB, uA);
	}

	public static byte[] GetVec4RGBA32(this Vector4 vec4)
	{
		var result = new List<byte>();

		result.Add((byte)(vec4.X));
		result.Add((byte)(vec4.Y));
		result.Add((byte)(vec4.Z));
		byte uA = (byte)(vec4.W);
		if (uA == 0xFE) 
			uA = 0x80;
		else 
			uA = (byte)(uA >> 1);
		result.Add(uA);

		return result.ToArray();
	}
}
