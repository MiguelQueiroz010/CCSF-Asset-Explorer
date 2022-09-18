using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using static IOextent;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text;

public class Object : Block
{
    internal Model[] GetModel(Block[] blocks) => blocks.Where(x => x.ObjectID == ModelID).ToArray() as Model[];

    public Clump ParentClump;

    [DisplayName("Linked Model")]
    [Description("See the linked model for the bone.")]
    [Category("Object")]
    public Model[] _mdl
    {
        get => GetModel(_ccsf.Blocks.ToArray());
        set => _ccsf.Blocks[_ccsf.IndexOf(_mdl[0])] = value[0];
    }

    public uint ParentObjectID;
    public uint ModelID;
    public uint ShadowID;
    public uint UnknowData;

    [DisplayName("Parent Object Index")]
    [Description("Index of the parent object.")]
    [Category("Object")]
    public uint ParentIndex
    {
        get => ParentObjectID;
    }
    [DisplayName("Parent Object Name")]
    [Description("Name of the parent object.")]
    [Category("Object")]
    public string ParentName
    {
        get => ParentObjectID != null ? _ccsToc.GetObjectName(ParentObjectID) : "NO OBJECT LINKED";
    }
    [DisplayName("Model Object Index")]
    [Description("Index of the model object.")]
    [Category("Object")]
    public uint ModelIndex
    {
        get => ModelID;
        set => ModelID = value;
    }
    [DisplayName("Model Object Name")]
    [Description("Name of the model object.")]
    [Category("Object")]
    public string _modelobject
    {
        get => _ccsToc.GetObjectName(ModelID);
    }
    [DisplayName("Shadow Model Object Index")]
    [Description("Index of the shadow model object.")]
    [Category("Object")]
    public uint ShadowModel
    {
        get => ShadowID;
        set => ShadowID = value;
    }
    [DisplayName("Shadow Model Object Name")]
    [Description("Name of the shadow model object.")]
    [Category("Object")]
    public string _shadowmodel
    {
        get => _ccsToc.GetObjectName(ShadowID);
    }
    public override byte[] DataArray
    {
        get
        {
            var writer = new BinaryWriter(new MemoryStream(Data));

            writer.BaseStream.Position = 0xC;

            writer.Write(ParentObjectID);
            writer.Write(ModelID);
            writer.Write(ShadowID);

            return Data;
        }
    }
    public override Block ReadBlock(Stream Input, Header header) => new Object()
    {
        Type = Input.ReadUInt(32),
        Size = Input.ReadUInt(32) * 4,
        ObjectID = Input.ReadUInt(32),
        Data = Input.ReadBytes(0, (int)Size),

        ParentObjectID = Input.ReadUInt(32),
        ModelID = Input.ReadUInt(32),
        ShadowID = Input.ReadUInt(32),
        UnknowData = header.Version > Header.CCSFVersion.GEN1 ? Input.ReadUInt(32) : 0
    };
}
