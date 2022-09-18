using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using static IOextent;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text;

public class Object_CT : Block
{
    //Animation REF
    public Animation _refAnimation;

	public uint Params;

    //Fixed
    public Vec3Position_Track.Position _fixedPos;
    public Vec3Rotation_Track.Rotation _fixedRot;
    public Vec3Scale_Track.Scale _fixedScale;

    //Animated Tracks
    public Vec3Position_Track _posTrack;
    public Vec3Rotation_Track _rotTrack;
    public Vec4Rotation_Track _rot4Track;
    public Vec3Scale_Track _scaleTrack;
    public F32_Track _alphaTrack;

    [Category("Controller")]
    [DisplayName("Position Track")]
    [Description("Controller's position track.")]
    public Vec3Position_Track[] _ptrack
    {
        get => new Vec3Position_Track[] { _posTrack };
        set => _posTrack = value[0];
    }
    [Category("Controller")]
    [DisplayName("Rotation Track")]
    [Description("Controller's rotation track.")]
    public Vec3Rotation_Track[] _rtrack
    {
        get => new Vec3Rotation_Track[] { _rotTrack };
        set => _rotTrack = value[0];
    }
    [Category("Controller")]
    [DisplayName("Scale Track")]
    [Description("Controller's scale track.")]
    public Vec3Scale_Track[] _sctrack
    {
        get => new Vec3Scale_Track[] { _scaleTrack };
        set => _scaleTrack = value[0];
    }
    public override byte[] DataArray
    {
        get
        {
            //var writer = new BinaryWriter(new MemoryStream(Data));

            //writer.BaseStream.Position = 0xC;


			return Data;
        }
    }
 
    public override Block ReadBlock(Stream Input, Header header, Animation animation)
    {
        var _ct = new Object_CT();
        _ct._refAnimation = animation;

        _ct.Type = Input.ReadUInt(32);
        _ct.Size = Input.ReadUInt(32) * 4;
        _ct.ObjectID = Input.ReadUInt(32);
        //Console.WriteLine($"Lendo bloco do objeto: 0x{_ct.ObjectID.ToString("X2")}");
        _ct.Data = Input.ReadBytes(0, (int)Size);

        _ct.Params = Input.ReadUInt(32);


        _ct._posTrack = Vec3Position_Track.Read(Input, Animation.GetTrack(0, (int)_ct.Params));
        _ct._rotTrack = Vec3Rotation_Track.Read(Input, Animation.GetTrack(1, (int)_ct.Params));
        _ct._rot4Track = Vec4Rotation_Track.Read(Input, Animation.GetTrack(8, (int)_ct.Params));
        _ct._scaleTrack = Vec3Scale_Track.Read(Input, Animation.GetTrack(2, (int)_ct.Params));
        _ct._alphaTrack = F32_Track.Read(Input, Animation.GetTrack(3, (int)_ct.Params));

        return _ct;
    }
}
