using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CCSF_Asset_Explorer
{
    public static class BINHelper
    {
        public static void UnpackToFolder(string filename, string folder, ProgressBar pb1 = null, Label strip = null)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            int pos = 0;
            int start = 0;
            int tpos;
            string name;
            fs.Seek(0, SeekOrigin.End);
            long size = fs.Position;
            fs.Seek(0, 0);
            byte[] buff = new byte[4];
            if(pb1 != null) pb1.Maximum = (int)size;
            int fileindex = 0;
            while (fs.Position < size)
            {
                fs.Read(buff, 0, 4);
                if (FileHelper.isGzipMagic(buff, 0))
                {
                    pos = (int)fs.Position - 4;
                    start = pos;
                    while (pos < size)
                    {
                        pos += 0x800;
                        fs.Seek(0x7FC, SeekOrigin.Current);
                        fs.Read(buff, 0, 4);
                        if (FileHelper.isGzipMagic(buff, 0))
                        {
                            fs.Seek(-4, SeekOrigin.Current);
                            break;
                        }
                    }
                    fs.Seek(start, 0);
                    buff = new byte[pos - start];
                    fs.Read(buff, 0, pos - start);
                    buff = FileHelper.unzipArray(buff);
                    name = "";
                    tpos = 0xc;
                    while (buff[tpos] != 0)
                        name += (char)buff[tpos++];
                    File.WriteAllBytes(folder + name + ".ccs", buff);
                    fileindex++;
                    if (pb1 != null)
                    {
                        pb1.Value = start;
                        strip.Text = name;
                        Application.DoEvents();
                    }
                    buff = new byte[4];
                }
                else
                    fs.Seek(0x7FC, SeekOrigin.Current);
            }
            if (pb1 != null)
            {
                pb1.Value = 0;
                strip.Text = "";
            }
            fs.Close();
        }

        public static void RepackFromFolder(string filename, string folder, ProgressBar pb1 = null, Label strip = null)
        {
            string[] files = Directory.GetFiles(folder, "*.ccs", SearchOption.TopDirectoryOnly);
            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            byte[] buff;
            int count = 0;
            string infilename;
            if (pb1 != null) pb1.Maximum = files.Length;
            foreach (string file in files)
            {
                if (pb1 != null)
                {
                    pb1.Value = count++;
                    strip.Text = Path.GetFileNameWithoutExtension(file);
                    Application.DoEvents();
                }
                buff = File.ReadAllBytes(file);
                MemoryStream m = new MemoryStream();
                    infilename = Path.GetFileNameWithoutExtension(file).Substring(0) + ".cmp";
                buff = FileHelper.zipArray(buff, infilename);
                buff[8] = 0;
                buff[9] = 0xB;
                m.Write(buff, 0, buff.Length);
                while (m.Length % 0x800 != 0)
                    m.WriteByte(0);
                m.Seek(-3, SeekOrigin.Current);
                m.Read(buff, 0, 3);
                if (buff[0] / 0x10 != 0 || buff[1] != 0 || buff[2] != 0)
                    m.Write(new byte[0x800], 0, 0x800);
                buff = m.ToArray();
                fs.Write(buff, 0, buff.Length);
            }
            if (pb1 != null)
            {
                pb1.Value = 0;
                strip.Text = "";
            }
            fs.Close();
            
        }
        public static void MoveWithReplace(string sourceFileName, string destFileName)
        {

            //first, delete target file if exists, as File.Move() does not support overwrite
            if (File.Exists(destFileName))
            {
                if (MessageBox.Show("Arquivo CCS com o mesmo nome encontrado. \nDeseja sobrescrever o arquivo? \n *Caso não, o CCS não substituirá o anterior e terá a extensão *.tmp2", "Atenção!!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes)

                {
                    File.Delete(destFileName);
                }
                else
                {
                    return;
                }
            }

            File.Move(sourceFileName, destFileName);

        }
        public static void UpdateBinList(string binlistPath, string folder, string outputList = null)
        {
            if (outputList == null) outputList = binlistPath;

            var lines = File.ReadAllLines(binlistPath).ToList();
            var result = new List<string>();
            long currentOffset = 0;
            bool insideFileList = false;

            foreach (string raw in lines)
            {
                string line = raw.TrimEnd();
                if (string.IsNullOrWhiteSpace(line))
                {
                    result.Add(line);
                    continue;
                }

                // Mantém cabeçalhos intactos
                if (line.StartsWith("#") || line.StartsWith("binNum") || line.StartsWith("binFile"))
                {
                    result.Add(line);
                    if (line.StartsWith("binFile")) insideFileList = true;
                    continue;
                }

                // Só processa entradas de arquivo reais (.ccs)
                if (!insideFileList || !line.Contains(".ccs"))
                {
                    result.Add(line);
                    continue;
                }

                var parts = line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 4)
                {
                    result.Add(line);
                    continue;
                }

                string name = parts[0].Trim();
                string filePath = Path.Combine(folder, name.Replace('\\', Path.DirectorySeparatorChar));

                if (!File.Exists(filePath))
                {
                    // mantém linha original se o arquivo não foi encontrado
                    result.Add(line);
                    continue;
                }

                // Lê o arquivo e calcula tamanhos
                byte[] data = File.ReadAllBytes(filePath);
                int size = data.Length;

                // comprime para saber o tamanho comprimido real
                string cmpName = Path.GetFileNameWithoutExtension(filePath) + ".cmp";
                byte[] compressed = FileHelper.zipArray(data, cmpName);
                int gzsize = compressed.Length;

                // adiciona padding de 0x800 (como no pack original)
                long alignedSize = (gzsize + 0x7FF) & ~0x7FF;

                // reescreve linha formatada
                string newLine = $"\t\t{name.PadRight(24)}\t0x{currentOffset:X8}\t0x{size:X8}\t0x{gzsize:X8}";
                result.Add(newLine);

                // avança offset
                currentOffset += alignedSize;
            }
            string final = "";
            foreach (var line in result)
                final += line + "\n";
            File.WriteAllText(outputList, final);
            Console.WriteLine($"binlist atualizado com sucesso: {outputList}");
        }

        public static void Repack(string filename, string folder, string name, ProgressBar pb1 = null, ToolStripStatusLabel strip = null)
        {
            string[] files = Directory.GetFiles(folder, name, SearchOption.TopDirectoryOnly);
            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            byte[] buff;
            int count = 0;
            string infilename;
            if (pb1 != null) pb1.Maximum = files.Length;
            foreach (string file in files)
            {
                if (pb1 != null)
                {
                    pb1.Value = count++;
                    strip.Text = Path.GetFileNameWithoutExtension(file);
                    Application.DoEvents();
                }
                buff = File.ReadAllBytes(file);
                MemoryStream m = new MemoryStream();
                infilename = Path.GetFileNameWithoutExtension(file).Substring(0) + ".cmp";
                buff = FileHelper.zipArray(buff, infilename);
                buff[8] = 0;
                buff[9] = 0xB;
                m.Write(buff, 0, buff.Length);
                while (m.Length % 0x800 != 0)
                    m.WriteByte(0);
                m.Seek(-3, SeekOrigin.Current);
                m.Read(buff, 0, 3);
                if (buff[0] / 0x10 != 0 || buff[1] != 0 || buff[2] != 0)
                    m.Write(new byte[0x800], 0, 0x800);
                buff = m.ToArray();
                fs.Write(buff, 0, buff.Length);
            }
            if (pb1 != null)
            {
                pb1.Value = 0;
                strip.Text = "";
            }
            fs.Close();
        }
    }
}
