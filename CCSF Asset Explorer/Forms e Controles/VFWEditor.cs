using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CCSF_Asset_Explorer
{
    public partial class VFWEditor : Form
    {
        public class TextureEntry
        {
            public ushort Unknow { get; set; }
            public ushort X { get; set; }
            public ushort Y { get; set; }
            public ushort Width { get; set; }
            public ushort Height { get; set; }
            public ushort TextureIndex { get; set; }

            public static TextureEntry FromBytes(byte[] data)
            {
                return new TextureEntry
                {
                    Unknow = (ushort)data.ReadUInt(0, 16),
                    X = (ushort)data.ReadUInt(2, 16),
                    Y = (ushort)data.ReadUInt(4, 16),
                    Width = (ushort)data.ReadUInt(6, 16),
                    Height = (ushort)data.ReadUInt(8, 16),
                    TextureIndex = (ushort)data.ReadUInt(10, 16)
                };
            }

            public void WriteTo(byte[] data, int offset)
            {
                BitConverter.GetBytes(Unknow).CopyTo(data, offset);
                BitConverter.GetBytes(X).CopyTo(data, offset + 2);
                BitConverter.GetBytes(Y).CopyTo(data, offset + 4);
                BitConverter.GetBytes(Width).CopyTo(data, offset + 6);
                BitConverter.GetBytes(Height).CopyTo(data, offset + 8);
                BitConverter.GetBytes(TextureIndex).CopyTo(data, offset + 10);
            }
        }
        public class TextureEntryManager
        {
            public List<TextureEntry> Entries = new List<TextureEntry>();

            public void Load(byte[] data)
            {

                for (int i = 0; i < data.Length; i+=12)
                {
                    byte[] bytes = new byte[12];
                    bytes = data.ReadBytes(i, 12);
                    var entry = TextureEntry.FromBytes(bytes);
                    Entries.Add(entry);
                }
            }

            public byte[] Save()
            {
                byte[] data = new byte[Entries.Count * 12];

                for (int i = 0; i < Entries.Count; i++)
                {
                    Entries[i].WriteTo(data, i * 12);
                }

                return data;
            }
        }

        Principal _main;
        TextureEntryManager manager;
        public VFWEditor(Principal main)
        {
            InitializeComponent();
            _main = main;
            BaseIMG.Image = _main.GetSelectedTab().selected;
            Text = main.GetSelectedTab().resourceView.SelectedNode.Text + " - VFW Editor";
        }

        private void CopyEntryRegion(TextureEntry entry, Bitmap originalImage, Bitmap outputImage, int destX, int destY)
        {
            Rectangle srcRect = new Rectangle(entry.X, entry.Y, entry.Width, entry.Height);
            Rectangle destRect = new Rectangle(destX, destY, entry.Width, entry.Height);

            using (Graphics g = Graphics.FromImage(outputImage))
            {
                g.DrawImage(originalImage, destRect, srcRect, GraphicsUnit.Pixel);
            }
        }
        
        private void exportBT_Click(object sender, EventArgs e)
        {
            var save = new SaveFileDialog();
            save.Filter = "Bin files (*.bin)|*.bin|All files (*.*)|*.*";
            save.FileName = "VFWData.bin";
            if (save.ShowDialog() != DialogResult.OK)
                return;

            // Salvar de volta em byte[]
            byte[] updatedData = manager.Save();
            File.WriteAllBytes(save.FileName, updatedData);
        }

        private void openBT_Click(object sender, EventArgs e)
        {
            var open = new OpenFileDialog();
            open.Filter = "Bin files (*.bin)|*.bin|All files (*.*)|*.*";
            if (open.ShowDialog() != DialogResult.OK)
                return;

            // Suponha que você tenha um array de bytes
            byte[] rawData = File.ReadAllBytes(open.FileName);
            manager = new TextureEntryManager();
            manager.Load(rawData);
            entrySelector.Maximum = manager.Entries.Count - 1;

            LoadEntryToControls(manager.Entries[0]);
        }
        bool loading = false;
        private void LoadEntryToControls(TextureEntry entry)
        {
            loading = true;
            xUpd.Value = entry.X;
            yUpd.Value = entry.Y;
            widthUpd.Value = entry.Width;
            Heightupd.Value = entry.Height;
            label6.Text = "TEXTURE INDEX: " + entry.TextureIndex;

            // Exemplo: mostra o recorte em um PictureBox
            Bitmap cropped = new Bitmap(entry.Width, entry.Height);
            CopyEntryRegion(entry, new Bitmap(BaseIMG.Image), cropped, 0, 0);
            VFW.Image = cropped;

            loading = false;
        }
        private void SaveControlsToEntry(TextureEntry entry)
        {
            entry.X = (ushort)xUpd.Value;
            entry.Y = (ushort)yUpd.Value;
            entry.Width = (ushort)widthUpd.Value;
            entry.Height = (ushort)Heightupd.Value;
        }

        private void entrySelector_ValueChanged(object sender, EventArgs e)
        {
            int index = (int)entrySelector.Value;
            LoadEntryToControls(manager.Entries[index]);
        }

        private void xUpd_ValueChanged(object sender, EventArgs e)
        {
            if (loading == false)
            {
                int index = (int)entrySelector.Value;
                SaveControlsToEntry(manager.Entries[index]);
                var entry = manager.Entries[index];
                // Exemplo: mostra o recorte em um PictureBox
                Bitmap cropped = new Bitmap(entry.Width, entry.Height);
                CopyEntryRegion(entry, new Bitmap(BaseIMG.Image), cropped, 0, 0);
                VFW.Image = cropped;

            }
        }
    }
}
