using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using static IOextent;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text;

public class BinaryBlob : Block
{
    //TESTE
    public override byte[] DataArray
    {
        get
        {
            var writer = new BinaryWriter(new MemoryStream(Data));

            writer.BaseStream.Position = 0xC;


            return Data;
        }
    }
    public override Block ReadBlock(Stream Input, Header header) => new BinaryBlob()
    {
        Type = Input.ReadUInt(32),
        Size = Input.ReadUInt(32) * 4,
        ObjectID = Input.ReadUInt(32),
        Data = Input.ReadBytes(0, (int)Size),


    };
}
