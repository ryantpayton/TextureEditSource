using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace TextureEdit
{
    class Layer
    {
        public string Name = "";
        public bool Visible = true;
        public Bitmap Bmp;

        public static void EncodeTXE(List<Layer> Layers, string FilePath)
        {
            using (FileStream fs = new FileStream(FilePath, FileMode.Append, FileAccess.Write))
            {
                foreach (Layer layer in Layers)
                {
                    if (!layer.Visible) { continue; }
                    byte[] nameB = Encoding.ASCII.GetBytes(layer.Name);
                    fs.Write(nameB, 0, nameB.Length);
                    fs.WriteByte(0);
                    layer.Bmp.Save(fs, ImageFormat.Png);
                }
            }
        }
        public static List<Layer> DecodeTXE(string FilePath)
        {
            List<Layer> result = new List<Layer>();
            using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                while (fs.Position < fs.Length)
                {
                    Layer layer = new Layer();
                    layer.Name = ReadNullTerminatingString(fs);
                    layer.Bmp = (Bitmap)Image.FromStream(fs);
                }
            }
            return result;
        }
        public static string ReadNullTerminatingString(Stream stream)
        {
            string s = "";
            while (true)
            {
                byte charB = (byte)stream.ReadByte();
                if (charB == 0) { break; }
                s += (char)charB;
            }
            return s;
        }
    }
}
