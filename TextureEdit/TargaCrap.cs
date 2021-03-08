using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TgaLib;

namespace TextureEdit
{
    public static class TargaCrap
    {
        public static Bitmap OpenTGA(string path)
        {
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(path)))
            {
                //return DmitryBrant.ImageFormats.TgaReader.Load(ms);
                //Surface surface = Surface.LoadFromStream(ms, true);
                //return getBitmapFromSurface(surface);
                using (BinaryReader br = new BinaryReader(ms)) 
                {
                    TgaImage tga = new TgaImage(br);
                    BitmapSource bmpSource = tga.GetBitmap();
                    return getBitmapFromBitmapSource(bmpSource);
                    
                }
            }
        }
        public static void SaveTGA(Bitmap bmp, string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            // --Header--

            // ID Field length
            fs.WriteByte(0); // No ID Field
            // Color map type
            fs.WriteByte(0); // No color map
            // Image type
            fs.WriteByte(2); // Uncompressed true-color image
            // Color map information, write 5 bytes of null since there isn't one
            for (int i = 0; i < 5; i++)
            {
                fs.WriteByte(0);
            }
            // --Image Specification--

            // X-Origin (0)
            fs.Write(numberToByteArray(0, 2), 0, 2);
            // Y-Origin (0)
            fs.Write(numberToByteArray(0, 2), 0, 2);
            // Width
            fs.Write(numberToByteArray(bmp.Width, 2), 0, 2);
            // Height
            fs.Write(numberToByteArray(bmp.Height, 2), 0, 2);
            // Pixel depth (for this limited purpose, we'll always use 32)
            fs.WriteByte(32);
            // Image descriptor (40 = left to write, top to bottom I think...)
            fs.WriteByte(40);

            // --The fun part, pixel data
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int cnt = 0;
            for (int scan = 0; scan < bmp.Height; scan++)
            {
                IntPtr scanPtr = IntPtr.Add(bmpData.Scan0, bmpData.Stride * scan);
                for (int xOffset = 0; xOffset < bmp.Width * 4; xOffset += 4)
                {
                    IntPtr pxPointer = scanPtr + xOffset;
                    byte alpha = Marshal.ReadByte(pxPointer + 3);
                    //Console.WriteLine(alpha);
                    if (alpha == 0)
                    {
                        fs.WriteByte(255);
                        fs.WriteByte(255);
                        fs.WriteByte(255);
                    } else
                    {
                        fs.WriteByte(Marshal.ReadByte(pxPointer));
                        fs.WriteByte(Marshal.ReadByte(pxPointer + 1));
                        fs.WriteByte(Marshal.ReadByte(pxPointer + 2));
                    }
                    fs.WriteByte(alpha);
                    cnt++;
                }
            }
            bmp.UnlockBits(bmpData);
            fs.Close();

            // Aight this prolly won't work but its chill
        }
        private static byte[] numberToByteArray(long num, int length)
        {
            if (length == 1)
            {
                return new byte[1] { (byte)num };
            } else if (length == 2)
            {
                return BitConverter.GetBytes((short)num);
            } else if (length == 4)
            {
                return BitConverter.GetBytes((int)num);
            } else
            {
                return BitConverter.GetBytes(num);
            }
        }
        public static Bitmap OpenImageAutoDetect(string path)
        {
            if (Path.GetExtension(path) == ".tga")
            {
                return OpenTGA(path);
            } else
            {
                return (Bitmap)Image.FromStream(new MemoryStream(File.ReadAllBytes(path)));
            }
        }
        private static Bitmap getBitmapFromBitmapSource(BitmapSource source)
        {
            Bitmap bmp = new Bitmap(
              source.PixelWidth,
              source.PixelHeight,
              PixelFormat.Format32bppPArgb);
            BitmapData data = bmp.LockBits(
              new Rectangle(Point.Empty, bmp.Size),
              ImageLockMode.WriteOnly,
              PixelFormat.Format32bppPArgb);
            source.CopyPixels(
              System.Windows.Int32Rect.Empty,
              data.Scan0,
              data.Height * data.Stride,
              data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }
    }
}
