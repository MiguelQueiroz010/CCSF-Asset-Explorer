using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CCSF_Asset_Explorer
{
    
    public static class BINHelper
    {
        public static FileStream fs;
        public static void UnpackToFolder(string binlist, string folder, ProgressBar pb1 = null, Label strip = null)
        {
            string baseFolder = Path.GetDirectoryName(binlist) + @"\";

            string list = File.ReadAllText(binlist);
            string[] entries = list.Split(new string[] {"\t", "\r\n","\n",
            " "}, StringSplitOptions.RemoveEmptyEntries).Skip(8).ToArray();
            string binFile = "";
            int prog = 0;
            string baseF = "";
            if (pb1 != null) pb1.Maximum = entries.Length;
            for (int e = 0; e < entries.Length;)
            {
                if (entries[e] == "binEnd")
                {
                    fs.Close();
                    e++;

                    if (entries[e] == "binFileEnd")
                        goto endInstr;
                    else
                        continue;
                }

                if (entries[e].Contains(".bin"))
                {
                    binFile = baseFolder + entries[e];
                    baseF = folder + @"\"+Path.GetFileNameWithoutExtension(entries[e])+@"\";
                    e+=2; // FileList
                    fs = new FileStream(binFile, FileMode.Open);//BIN STREAM
                }

                if (binFile == "")
                    throw new Exception("No binfiles found on list!");

                if(File.Exists(binFile))
                {
                    string PathSave = baseF + entries[e];
                    if (!Directory.Exists(Path.GetDirectoryName(PathSave)))
                        Directory.CreateDirectory(Path.GetDirectoryName(PathSave));

                    long offset = Convert.ToInt64(entries[e + 1],16);
                    long size = Convert.ToInt64(entries[e + 2],16);
                    long gzsize = Convert.ToInt64(entries[e + 3],16);

                    byte[] GzipedCCS = fs.ReadBytes((int)offset, (int)size);
                    File.WriteAllBytes(PathSave, FileHelper.unzipArray(GzipedCCS));

                    if (pb1 != null)
                    {
                        pb1.Value = e;
                        if (strip != null) strip.Text = Path.GetFileNameWithoutExtension(PathSave);
                        Application.DoEvents();
                    }

                    e += 4;
                }
            }
        endInstr:
            MessageBox.Show("Feito!");
            
        }
        public static void RepackFromFolder(string[] binFiles, string folder, ProgressBar pb1 = null, Label strip = null)
        {
            var binList = new StringBuilder("#\t\tname\t\t\t\toffset\t\tsize\t\tgzip\r\nbinNum 2\r\nbinFile\r\n"); //HEADER

            string baseSave = new DirectoryInfo(folder).Parent.FullName + @"\";

            Int64 PositronBIN = 0;
           foreach(string binFile in binFiles)
            {
                var BinStream = new FileStream(baseSave + Path.GetFileName(binFile), FileMode.Create);

                binList.Append($"\t{Path.GetFileName(binFile)}\t0x{PositronBIN}\n");

                string path = folder + @"\" + Path.GetFileNameWithoutExtension(binFile);

                var allfiles = Directory.EnumerateFiles(path, "*.ccs", SearchOption.AllDirectories);

                long offset = 0;
                if (pb1 != null) pb1.Maximum = allfiles.Count();
                int c = 0;
                foreach (var file in allfiles)
                {
                    byte[] CCS = File.ReadAllBytes(file);
                    byte[] ZipCCS = FileHelper.zipArray(CCS,
                        Path.GetFileNameWithoutExtension(file)+".cmp");

                    string fileName = new String(file.Skip(path.Length+1).ToArray()).
                        PadRight(24).ToString();

                    //Entries on binlist
                    binList.Append($"\t\t{fileName}\t0x{offset:X8}\t0x{ZipCCS.Length:X8}\t0x{CCS.Length:X8}\n");


                    //Write File
                    BinStream.Write(ZipCCS, 0, ZipCCS.Length);

                    //Alinhamento
                    while (BinStream.Length % 0x800 != 0)
                        BinStream.WriteByte(0);

                    if (pb1 != null)
                    {
                        pb1.Value = c;
                        if (strip != null) strip.Text = Path.GetFileNameWithoutExtension(file);
                        Application.DoEvents();
                    }
                    PositronBIN += ZipCCS.Length;
                    //Alinhamento
                    while (PositronBIN % 0x800 != 0)
                        PositronBIN++;

                    offset = BinStream.Position;
                    c++;
                }

                binList.Append("\t\tbinEnd\n");
                BinStream.Close();

            }
            binList.Append("binFileEnd");
            binList.Replace("\r\n", "\n");
            File.WriteAllText(baseSave + "newlist.txt", binList.ToString(), Encoding.Default);

            MessageBox.Show("Feito!");
        }
        
        
    }
}
