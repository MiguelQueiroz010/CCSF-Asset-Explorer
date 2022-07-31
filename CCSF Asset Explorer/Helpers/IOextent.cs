using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;

/// <summary>
/// Classe de extensões de tipos de dados conhecidos, para usos diversos,
/// conversões e leituras.
/// </summary>
/// Bit.Raiden/Dev C-Sharp, uso não comercial no momento.
/// Existe conhecimento, mas apenas o conhecimento de Cristo é poder.
/// Novembro/2021
public static class IOextent
{
    public static void ReplaceBlock(this Block block, string blockpath)
    {
        block = Block.ReadAllBlocks(File.OpenRead(blockpath), false, false, block._ccsHeader, true)[0];
    }
    #region Leitores de Dados

    #region BITS
    /// <summary>
    /// Lê um bit de um determinado array[byte].
    /// </summary>
    /// <param name="offset">Posição para iniciar a leitura(0-7).</param>
    /// <returns>Bool.</returns>
    public static bool ReadBit(this byte array, int offset)
    {
        BitArray arra = new BitArray(new byte[] { array });
        return arra[offset];
    }

    /// <summary>
    /// Lê todos os bits de um determinado array[byte].
    /// </summary>
    /// <returns>BitArray.</returns>
    public static BitArray ReadBits(this byte array)
    {
        BitArray arra = new BitArray(new byte[] { array });
        return arra;
    }

    /// <summary>
    /// Converte todos os bits do array em uma string.
    /// </summary>
    /// <returns>string.</returns>
    public static string ToSTR(this BitArray array)
    {
        string result = "";
        foreach (var inte in array)
            result += Convert.ToInt32(inte).ToString();
        result = new string(result.Reverse().ToArray());
        return result;
    }
    #endregion

    #region BYTES
    /// <summary>
    /// Lê uma quantia específica de bytes de um determinado array[buffer].
    /// </summary>
    /// <param name="offset">Posição para iniciar a leitura.</param>
    /// <param name="size">Tamanho da leitura em bytes.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadBytes(this byte[] array, int offset, int size)
    {
        byte[] result = array.Skip(offset).ToArray().Take(size).ToArray();
        return result;
    }

    /// <summary>
    /// Lê uma quantia específica de bytes de um fluxo determinado e avança a posição na mesma quantia[Stream].
    /// </summary>
    /// <param name="offset">Posição para iniciar a leitura.</param>
    /// <param name="size">Tamanho da leitura em bytes.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadBytes(this Stream stream, int offset, int size)
    {
        var result = new List<byte>();
        long oldoffset = stream.Position;
        stream.Position = (long)offset;

        for (int i = 0; i < size; i++)
            result.Add((byte)stream.ReadByte()); stream.Flush();

        stream.Position = oldoffset;
        return result.ToArray();
    }
    public static List<byte[]> ReadBlocks(this Stream stream, int quant)
    {
        var ObjBlock = new List<byte>();
        bool end = false;
        while (end==false)
        {
            if (stream.Position >= stream.Length)
                end = true;
            else
            {
                if (stream.ReadBytes((int)stream.Position, 5).ConvertTo(Encoding.Default) != "[OBJ]")
                    ObjBlock.Add((byte)stream.ReadByte());
                else
                    end = true;
            }
        }

        var result = new List<byte[]>();
        var objstream = new MemoryStream(ObjBlock.ToArray());
        for(int i =0;i<quant;i++)
        {
            uint coint = 0;
            uint oldoffs = (uint)objstream.Position;
            for (; objstream.ReadBytes(4, false).ConvertTo(Encoding.Default) != "<BL>";
             coint+=4)
            {
            }
            objstream.Position = oldoffs;
            byte[] block = objstream.ReadBytes((int)coint);
            objstream.Position += 4;
            if(!result.Contains(block))
                result.Add(block);
        }
        return result;
    }
    /// <summary>
    /// Lê uma quantia específica de bytes de um fluxo determinado e avança a posição na mesma quantia[Stream].
    /// </summary>
    /// <param name="offset">Posição para iniciar a leitura.</param>
    /// <param name="size">Tamanho da leitura em bytes.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadBytes(this Stream stream, int size, bool maintain = false)
    {
        long pos = stream.Position;
        var result = new List<byte>();
        for (int i = 0; i < size; i++)
            result.Add((byte)stream.ReadByte());
        if (maintain)
            stream.Position = pos;
        return result.ToArray();
    }

    /// <summary>
    /// Lê uma quantia específica de bytes de um fluxo determinado e avança a posição na mesma quantia[Stream].
    /// </summary>
    /// <param name="offset">Posição para iniciar a leitura.</param>
    /// <param name="size">Tamanho da leitura em bytes.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadBytesTM2(this Stream stream)
    {
        var result = new List<byte>();
        uint size = stream.ReadUInt((int)stream.Position, 32);
        for (int i = 0; i < (int)size; i++)
            result.Add((byte)stream.ReadByte());
        return result.ToArray();
    }
    /// <summary>
    /// Lê uma quantia específica de bytes de um fluxo determinado[Stream].
    /// </summary>
    /// <param name="offset">Posição para iniciar a leitura.</param>
    /// <param name="size">Tamanho da leitura em bytes.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadBytesLong(this Stream stream, long offset, int size)
    {
        var result = new List<byte>();
        stream.Position = offset;
        for (int i = 0; i < size; i++)
            result.Add((byte)stream.ReadByte());
        return result.ToArray();
    }

    /// <summary>
    /// Lê uma seção de arquivos de um fluxo e posição determinado[Stream].
    /// </summary>
    /// <param name="lba">Posição LBA para iniciar a leitura.</param>
    /// <param name="size">Tamanho dos setores.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadFiles(this Stream stream, int lba, int size = 2048)
    {
        var result = new List<byte>();
        int offset = lba * size;
        bool stop = false;
        while(stop==false)
        {
            stream.Position = offset;
            byte sizex = (byte)stream.ReadByte();
            if (sizex == 0)
                stop = true;
            if (stop != true)
            {
                stream.Position = offset;
                byte[] section = stream.ReadBytes(offset, sizex);
                result.AddRange(section);
                offset += section.Length;
            }
        }
        return result.ToArray();
    }

    /// <summary>
    /// Lê um setor de um array de bytes[buffer].
    /// </summary>
    /// <param name="lba">Localização de bloco lógico.[LBA]</param>
    /// <param name="size">Tamanho do setor para leitura.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadSector(this byte[] array, int lba, int size = 2048)
    {
        byte[] result = array.ReadBytes(lba * size, size);
        return result;
    }

    /// <summary>
    /// Lê um setor de um fluxo determinado[Stream].
    /// </summary>
    /// <param name="lba">Localização de bloco lógico.[LBA]</param>
    /// <param name="size">Tamanho do setor para leitura.</param>
    /// <returns>Byte[].</returns>
    public static byte[] ReadSector(this Stream stream, int lba, int size = 2048)
    {
        byte[] result = stream.ReadBytes(lba * size, size);
        return result;
    }
    #endregion

    #endregion

    #region Escrita de Dados

    /// <summary>
    /// Escreve uma quantia específica de bytes em um fluxo determinado[Stream].
    /// </summary>
    /// <param name="offset">Posição para iniciar a leitura.</param>
    /// <param name="offsetwrite">Posição para iniciar a escrita.</param>
    /// <param name="size">Tamanho da escrita em bytes.</param>
    public static void WriteBytes(this Stream stream, Stream write, long offsetwrite, long offset, long size)
    {
        stream.Position = offsetwrite;
        write.Position = offset;
        int c = 0;
        while(c<size)
        {
            stream.WriteByte((byte)write.ReadByte());
            c++;
        }
    }

    #endregion

    #region Leitores de Inteiros
    /// <summary>
    /// Lê um Inteiro sem sinal do array de bytes[buffer].
    /// </summary>
    /// <param name="offset">Posição para ler o inteiro.</param>
    /// <param name="bits">Quantia de bits a serem lidos.</param>
    /// <param name="bigendian">Usar codificação BigEndian ao invés de LittleEndian padrão.</param>
    /// <returns>Inteiro sem sinal(uint)</returns>
    public static uint ReadUInt(this byte[] array, int offset, int bits, bool bigendian = false)
    {
        var reader = new BinaryReader(new MemoryStream(array));
        reader.BaseStream.Position = offset;
        uint result = 0;
        switch (bits)
        {
            case 8:
                result = (uint)reader.ReadByte();
                break;

            case 16:
                result = reader.ReadUInt16();
                break;

            case 32:
                result = reader.ReadUInt32();
                break;
        }
        reader.Close();
        result = bigendian ? BitConverter.ToUInt32(BitConverter.GetBytes(result).Reverse().ToArray(),0) : BitConverter.ToUInt32(BitConverter.GetBytes(result), 0);
        return result;
    }
    /// <summary>
    /// Lê um float de 32 bits.[Stream].
    /// </summary>
    /// <param name="offset">Posição para ler o inteiro.</param>
    /// <param name="bigendian">Usar codificação BigEndian ao invés de LittleEndian padrão.</param>
    /// <returns>Número de ponto flutuante(Single)</returns>
    public static Single ReadSingle(this Stream strean, int offset, bool bigendian = false)
    {
        strean.Position = offset;
        byte[] bitssx = strean.ReadBytes(4);
        if (bigendian == true)
            Array.Reverse(bitssx);
        Single result = 0;
        result = BitConverter.ToSingle(bitssx, 0);
        strean.Flush();
        //strean.Position = offset;
        return result;
    }
    /// Lê um float de 32 bits e avança a posição em 4 bytes.[Stream].
    /// </summary>
    /// <param name="bigendian">Usar codificação BigEndian ao invés de LittleEndian padrão.</param>
    /// <returns>Número de ponto flutuante(Single)</returns>
    public static Single ReadSingle(this Stream strean, bool bigendian = false)
    {
        byte[] bitssx = strean.ReadBytes(4);
        if (bigendian == true)
            Array.Reverse(bitssx);
        Single result = 0;
        result = BitConverter.ToSingle(bitssx, 0);
        strean.Flush();
        //strean.Position = offset;
        return result;
    }
    /// <summary>
    /// Lê um Inteiro sem sinal do fluxo[Stream].
    /// </summary>
    /// <param name="offset">Posição para ler o inteiro.</param>
    /// <param name="bits">Quantia de bits a serem lidos.</param>
    /// <param name="bigendian">Usar codificação BigEndian ao invés de LittleEndian padrão.</param>
    /// <returns>Inteiro sem sinal(uint)</returns>
    public static uint ReadUInt(this Stream strean, int offset, int bits, bool bigendian = false)
    {
        strean.Position = offset;
        byte[] bitssx = strean.ReadBytes((int)(bits / 8));
        uint result = 0;
        switch(bits)
        {
            case 8:
                result = bitssx[0];
                break;
            case 16:
                result = bigendian ? BitConverter.ToUInt16(bitssx.Reverse().ToArray(), 0) : BitConverter.ToUInt16(bitssx, 0);
                break;
            case 32:
                result = bigendian ? BitConverter.ToUInt32(bitssx.Reverse().ToArray(), 0) : BitConverter.ToUInt32(bitssx, 0);
                break;
        }
        strean.Flush();
        //strean.Position = offset;
        return result;
    }
    /// <summary>
    /// Lê um Inteiro sem sinal do fluxo[Stream] e avança a posição a quantia de bytes por bits.
    /// </summary>
    /// <param name="bits">Quantia de bits a serem lidos.</param>
    /// <param name="bigendian">Usar codificação BigEndian ao invés de LittleEndian padrão.</param>
    /// <returns>Inteiro sem sinal(uint)</returns>
    public static uint ReadUInt(this Stream strean, int bits, bool bigendian = false)
    {
        byte[] bitssx = strean.ReadBytes((int)(bits / 8));
        uint result = 0;
        switch (bits)
        {
            case 8:
                result = bitssx[0];
                break;
            case 16:
                result = bigendian ? BitConverter.ToUInt16(bitssx.Reverse().ToArray(), 0) : BitConverter.ToUInt16(bitssx, 0);
                break;
            case 32:
                result = bigendian ? BitConverter.ToUInt32(bitssx.Reverse().ToArray(), 0) : BitConverter.ToUInt32(bitssx, 0);
                break;
        }
        //strean.Position = offset;
        return result;
    }
    /// <summary>
    /// Lê um Long sem sinal do array de bytes[buffer].
    /// </summary>
    /// <param name="offset">Posição para ler o inteiro.</param>
    /// <param name="bits">Quantia de bits a serem lidos.</param>
    /// <param name="bigendian">Usar codificação BigEndian ao invés de LittleEndian padrão.</param>
    /// <returns>Long sem sinal(ulong)</returns>
    public static ulong ReadULong(this byte[] array, int offset, bool bigendian = false)
    {
        ulong result = bigendian ? BitConverter.ToUInt64(array.ReadBytes(offset, 8).Reverse().ToArray(), 0) : BitConverter.ToUInt64(array.ReadBytes(offset, 8), 0);
        return result;
    }

    /// <summary>
    /// Lê um Long sem sinal do fluxo[Stream].
    /// </summary>
    /// <param name="offset">Posição para ler o inteiro.</param>
    /// <param name="bits">Quantia de bits a serem lidos.</param>
    /// <param name="bigendian">Usar codificação BigEndian ao invés de LittleEndian padrão.</param>
    /// <returns>Long sem sinal(ulong)</returns>
    public static ulong ReadULong(this Stream strean, int offset, bool bigendian = false)
    {
        ulong result = bigendian ? BitConverter.ToUInt64(strean.ReadBytes(offset, 8).Reverse().ToArray(), 0) : BitConverter.ToUInt64(strean.ReadBytes(offset, 8), 0);
        return result;
    }
    #endregion

    #region Leitores de Texto

    /// <summary>
    /// Lê um array de bytes enquanto diferente de uma quebra no array(algum byte)[buffer].
    /// </summary>
    /// <param name="offset">Posição para fazer a leitura.</param>
    /// <param name="breakeroff">Byte de quebra de leitura(limitador), valor padrão é o byte 0[NULL].</param>
    /// <returns>byte[]</returns>
    public static byte[] ReadBroke(this byte[] file, int offset, byte breakeroff=0 )
    {
        byte[] result = file.Skip(offset).ToArray().TakeWhile(x=>x!=breakeroff).ToArray();
        return result;
    }

    /// <summary>
    /// Lê um array de bytes enquanto diferente de uma quebra no fluxo(algum byte)[Stream].
    /// </summary>
    /// <param name="offset">Posição para fazer a leitura.</param>
    /// <param name="breakeroff">Byte de quebra de leitura(limitador), valor padrão é o byte 0[NULL].</param>
    /// <returns>byte[]</returns>
    public static byte[] ReadBroke(this Stream file, int offset, byte breakeroff = 0)
    {
        var result = new List<byte>();
        file.Position = offset;
        while(file.ReadBytes((int)file.Position,1)[0]!=breakeroff)
        {
            result.Add((byte)file.ReadByte());
        }
        return result.ToArray();
    }

    //Stream
    /// <summary>
    /// Lê uma string enquanto diferente de uma quebra no fluxo(algum byte)[Stream].
    /// </summary>
    /// <param name="offset">Posição para fazer a leitura.</param>
    /// <param name="encoding">Codificação de leitura.</param>
    /// <param name="breakeroff">Byte de quebra de leitura(limitador), valor padrão é o byte 0[NULL].</param>
    /// <returns>string</returns>
    public static string ReadString(this Stream file, int offset, Encoding encoding, byte breakeroff = 0)
    {
        byte[] traw = ReadBroke(file, offset, breakeroff);
        return encoding.GetString(traw);
    }

    /// <summary>
    /// Lê uma string enquanto diferente de uma quebra no fluxo(algum byte)[Stream].
    /// </summary>
    /// <param name="offset">Posição para fazer a leitura.</param>
    /// <param name="breakeroff">Byte de quebra de leitura(limitador), valor padrão é o byte 0[NULL].</param>
    /// <returns>string</returns>
    public static string ReadString(this Stream file, int offset, byte breakeroff = 0)
    {
        byte[] traw = ReadBroke(file, offset, breakeroff);
        return Encoding.Default.GetString(traw);
    }

    //Array
    /// <summary>
    /// Lê uma string enquanto diferente de uma quebra no fluxo(algum byte)[Array].
    /// </summary>
    /// <param name="offset">Posição para fazer a leitura.</param>
    /// <param name="encoding">Codificação de leitura.</param>
    /// <param name="breakeroff">Byte de quebra de leitura(limitador), valor padrão é o byte 0[NULL].</param>
    /// <returns>string</returns>
    public static string ReadString(this byte[] file, int offset, Encoding encoding, byte breakeroff = 0)
    {
        byte[] traw = ReadBroke(file, offset, breakeroff);
        return encoding.GetString(traw);
    }

    /// <summary>
    /// Lê uma string enquanto diferente de uma quebra no fluxo(algum byte)[Array].
    /// </summary>
    /// <param name="offset">Posição para fazer a leitura.</param>
    /// <param name="breakeroff">Byte de quebra de leitura(limitador), valor padrão é o byte 0[NULL].</param>
    /// <returns>string</returns>
    public static string ReadString(this byte[] file, int offset, byte breakeroff = 0)
    {
        byte[] traw = ReadBroke(file, offset, breakeroff);
        return Encoding.Default.GetString(traw);
    }

    //Conversores
    /// <summary>
    /// Converte uma string para uma codificação específica[string].
    /// </summary>
    /// <param name="encoding">Codificação de saída.</param>
    /// <returns>string</returns>
    public static string ConvertTo(this string file, Encoding encoding)
    {
        return encoding.GetString(encoding.GetBytes(file));
    }

    /// <summary>
    /// Converte uma string para uma codificação específica[string].
    /// </summary>
    /// <param name="encoding">Codificação de saída.</param>
    /// <returns>string</returns>
    public static string ConvertTo(this byte[] file, Encoding encoding)
    {
        return encoding.GetString(file);
    }
    #endregion

    #region Extra
    public static Header.CCSFVersion GetVersion(this string str)
    {
        switch(str)
        {
            case "GEN1":
                return Header.CCSFVersion.GEN1;
            case "GEN1_5":
                return Header.CCSFVersion.GEN1_5;
            case "GEN2":
                return Header.CCSFVersion.GEN2;
            case "GEN2_5":
                return Header.CCSFVersion.GEN2_5;
            case "GEN3":
                return Header.CCSFVersion.GEN3;
            default:
                return Header.CCSFVersion.Unknow;
        }
    }
    public static byte[] GetFilledString(this string str,int size, byte fillwith)
    {
        byte[] outbin = new byte[size];
        outbin.FillArray(fillwith);
        Encoding.Default.GetBytes(str).CopyTo(outbin, 0);
        return outbin;
    }
    public static byte[] FillArray(this byte[] array, byte value)
    {
        for (int i = 0; i < array.Length; i++)
            if (array[i] != value)
                array[i] = value;
        return array;
    }
    public static byte ToByte(this BitArray array)
    {
        byte[] outbin = new byte[1];
        array.CopyTo(outbin, 0);
        return outbin[0];
    }
    public static byte[] ToLEBE(this uint entry, int bits, bool flip = false)
    {
        var outbin = new List<byte>();
        if (bits == 16)
        {
            outbin.AddRange(BitConverter.GetBytes((UInt16)entry));
            if(flip)
                outbin.AddRange(BitConverter.GetBytes((UInt16)entry).Reverse().ToArray());
        }
        else if(bits==64)
        {
            outbin.AddRange(BitConverter.GetBytes((UInt64)entry));
            if (flip)
                outbin.AddRange(BitConverter.GetBytes((UInt64)entry).Reverse().ToArray());
        }
        else if(bits==32)
        {
            outbin.AddRange(BitConverter.GetBytes((UInt32)entry));
            if (flip)
                outbin.AddRange(BitConverter.GetBytes((UInt32)entry).Reverse().ToArray());
        }
        return outbin.ToArray();
    }
    public static DateTime GetDateTimeDir(this byte[] file)
    {
        try
        {
            int ano = (int)file.ReadUInt(0,16);
            int mês = (int)file.ReadUInt( 2, 16);
            int dia = (int)file.ReadUInt( 4, 16);
            int hora = (int)file.ReadUInt( 6, 16);
            return new DateTime(ano, mês, dia, hora,0,0);
        }
        catch (Exception) { return new DateTime(); }
    }
    public static byte[] GetDateTimeData(this DateTime time)
    {
        var outbin = new List<byte>();
        outbin.AddRange(BitConverter.GetBytes((UInt16)time.Year));
        outbin.AddRange(BitConverter.GetBytes((UInt16)time.Month));
        outbin.AddRange(BitConverter.GetBytes((UInt16)time.Day));
        outbin.AddRange(BitConverter.GetBytes((UInt16)time.Hour));
        return outbin.ToArray();
    }
    public static byte[] GetDateTimeOffsetData(this DateTimeOffset time)
    {
        var outbin = new List<byte>();
        outbin.AddRange(BitConverter.GetBytes((UInt16)0));//GMT Position(REVIEW)
        outbin.AddRange(BitConverter.GetBytes((Int16)time.Year));
        outbin.Add((byte)time.Month);
        outbin.Add((byte)time.Day);
        outbin.Add((byte)time.Hour);
        outbin.Add((byte)time.Minute);
        outbin.Add((byte)time.Second);
        outbin.Add((byte)0);
        outbin.Add((byte)time.Millisecond);
        outbin.Add((byte)0);
        return outbin.ToArray();
    }

    public static Color[] GetPalette(this byte[] PAL)
    {
        var colors = new List<Color>();
        for(int i =0;i<PAL.Length;i+=4)
        {
            var color = Color.FromArgb(
                (PAL[i+3] * 255) / 128,//Alfa
                PAL[i],//R
                PAL[i+1],//G
                PAL[i+2]//B
                );
                colors.Add(color);
        }
        return colors.ToArray();
    }
    #endregion
}
