using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using static IOextent;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text;

public class Object_KF : Block
{
	public uint Unknow;

    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;
    public float Opacity;
	public uint Flag;

	[DisplayName("X")]
	[Description("Modify the object position coordinates.")]
	[Category("Position")]
	public decimal _x
	{
		get => (decimal)Position.X;
		set => Position.X = (float)value;
	}
	[DisplayName("Y")]
	[Description("Modify the object position coordinates.")]
	[Category("Position")]
	public decimal _y
	{
		get => (decimal)Position.Y;
		set => Position.Y = (float)value;
	}
	[DisplayName("Z")]
	[Description("Modify the object position coordinates.")]
	[Category("Position")]
	public decimal _z
	{
		get => (decimal)Position.Z;
		set => Position.Z = (float)value;
	}

	[DisplayName("X")]
	[Description("Modify the object rotation coordinates.")]
	[Category("Rotation")]
	public decimal _rx
	{
		get => (decimal)Rotation.X;
		set => Rotation.X = (float)value;
	}
	[DisplayName("Y")]
	[Description("Modify the object rotation coordinates.")]
	[Category("Rotation")]
	public decimal _ry
	{
		get => (decimal)Rotation.Y;
		set => Rotation.Y = (float)value;
	}
	[DisplayName("Z")]
	[Description("Modify the object rotation coordinates.")]
	[Category("Rotation")]
	public decimal _rz
	{
		get => (decimal)Rotation.Z;
		set => Rotation.Z = (float)value;
	}
	[DisplayName("X")]
	[Description("Modify the object scale.")]
	[Category("Scale")]
	public decimal _sx
	{
		get => (decimal)Scale.X;
		set => Scale.X = (float)value;
	}
	[DisplayName("Y")]
	[Description("Modify the object scale.")]
	[Category("Scale")]
	public decimal _sy
	{
		get => (decimal)Scale.Y;
		set => Scale.Y = (float)value;
	}
	[DisplayName("Z")]
	[Description("Modify the object scale.")]
	[Category("Scale")]
	public decimal _sz
	{
		get => (decimal)Scale.Z;
		set => Scale.Z = (float)value;
	}
	[DisplayName("Opacity")]
	[Description("Modify the object opacity atribute.")]
	[Category("KeyFrame")]
	public decimal _opacity
	{
		get => (decimal)Opacity;
		set => Opacity = (float)value;
	}
	[DisplayName("Texture Flag")]
	[Description("Modify the object texture flag.")]
	[Category("KeyFrame")]
	public uint _flag
	{
		get => Flag;
		set => Flag = value;
	}
	public override byte[] DataArray
    {
        get
        {
   //         var writer = new BinaryWriter(new MemoryStream(Data));

   //         writer.BaseStream.Position = 0xC;

			//writer.Write(Unknow.ToLEBE(32));

			//writer.Write(Position.GetVec3());
   //         writer.Write(Rotation.GetVec3());
   //         writer.Write(Scale.GetVec3());

   //         writer.Write(BitConverter.GetBytes(Opacity));
			//writer.Write(Flag.ToLEBE(32));

			return Data;
        }
    }
 
    public override Block ReadBlock(Stream Input, Header header) => new Object_KF()
    {
        Type = Input.ReadUInt(32),
        Size = Input.ReadUInt(32) * 4,
        ObjectID = Input.ReadUInt(32),
        Data = Input.ReadBytes(0, (int)Size),

		Unknow = Input.ReadUInt(32),

		Position = Helper3D.ReadVec3(Input),
		Rotation = Helper3D.ReadVec3(Input),
		Scale = Helper3D.ReadVec3(Input),

		Opacity = new BinaryReader(Input).ReadSingle(),
		Flag = Input.ReadUInt(32)
	};
}
