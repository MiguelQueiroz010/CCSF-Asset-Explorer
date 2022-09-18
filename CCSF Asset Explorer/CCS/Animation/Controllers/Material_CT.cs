using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using static IOextent;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text;
using OpenTK;

public class Material_CT : Block
{
    //Animation REF
    public Animation _refAnimation;

	public uint Params;

    public F32_Track _UTrack;
    public F32_Track _VTrack;
    public F32_Track _unk1f32Track;
    public F32_Track _unk2f32Track;

    [Category("Controller")]
    [DisplayName("U Coordinates Track")]
    [Description("Controller's U Coordinates track.")]
    public F32_Track[] _utrack
    {
        get => new F32_Track[] { _UTrack };
        set => _UTrack = value[0];
    }
    [Category("Controller")]
    [DisplayName("V Coordinates Track")]
    [Description("Controller's V Coordinates track.")]
    public F32_Track[] _vtrack
    {
        get => new F32_Track[] { _VTrack };
        set => _VTrack = value[0];
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
        var _ct = new Material_CT();
        _ct._refAnimation = animation;

        _ct.Type = Input.ReadUInt(32);
        _ct.Size = Input.ReadUInt(32) * 4;
        _ct.ObjectID = Input.ReadUInt(32);
        //Console.WriteLine($"Lendo bloco do objeto: 0x{_ct.ObjectID.ToString("X2")}");
        _ct.Data = Input.ReadBytes(0, (int)Size);

        _ct.Params = Input.ReadUInt(32);

        _ct._UTrack = F32_Track.Read(Input, Animation.GetTrack(0, (int)_ct.Params));
        _ct._VTrack = F32_Track.Read(Input, Animation.GetTrack(1, (int)_ct.Params));
        _ct._unk1f32Track = F32_Track.Read(Input, Animation.GetTrack(2, (int)_ct.Params));
        _ct._unk2f32Track = F32_Track.Read(Input, Animation.GetTrack(3, (int)_ct.Params));
        return _ct;
    }
}
