using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TextureEdit
{
    class BitmapProcessor : IDisposable
    {
        private Bitmap src;
        private Size srcSize;
        public BitmapData srcData;
        private IntPtr currentLocation_ptr;
        private IntPtr scan0;

        public Rectangle Clip;
        public Point CurrentLocation = new Point(-1, 0);

        public static bool AllowCacheing = false;
        private static List<CacheEntry> cachedData = new List<CacheEntry>();
        public bool PixelAvailable
        {
            get
            {
                if (CurrentLocation.X + 1 >= (Clip.X + Clip.Width) && CurrentLocation.Y + 1 >= (Clip.Y + Clip.Height))
                {
                    return false;
                } else
                {
                    return true;
                }
            }
        }
        public static void DisableCache()
        {
            foreach (CacheEntry ce in cachedData)
            {
                ce.b.UnlockBits(ce.bData);
            }
            cachedData.Clear();
            AllowCacheing = false;
        }
        public BitmapProcessor(Bitmap b)
        {
            src = b;
            srcSize = src.Size;
            if (AllowCacheing)
            {
                List<CacheEntry> entry = cachedData.Where((CacheEntry ce) => { return (ce.b == b); }).ToList();
                if (entry.Count == 0)
                {
                    srcData = b.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadWrite, src.PixelFormat);
                } else
                {
                    srcData = entry[0].bData;
                    cachedData.Remove(entry[0]);
                }
            } else
            {
                srcData = b.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadWrite, src.PixelFormat);
            }
            scan0 = srcData.Scan0;
            Clip = new Rectangle(0, 0, src.Width, src.Height);
            currentLocation_ptr = srcData.Scan0 - 4;
        }
        public BitmapProcessor(Bitmap b, Rectangle clip)
        {
            src = b;
            srcSize = src.Size;
            if (AllowCacheing)
            {
                List<CacheEntry> entry = cachedData.Where((CacheEntry ce) => { return (ce.b == b); }).ToList();
                if (entry.Count == 0)
                {
                    srcData = b.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadWrite, src.PixelFormat);
                }
                else
                {
                    srcData = entry[0].bData;
                    cachedData.Remove(entry[0]);
                }
            }
            else
            {
                srcData = b.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadWrite, src.PixelFormat);
            }
            scan0 = srcData.Scan0;
            Clip = clip;
            int left = Clip.Left;
            int right = Clip.Right;
            int top = Clip.Top;
            int bottom = Clip.Bottom;
            if (left < 0)
            {
                left = 0;
            }
            if (right > src.Width)
            {
                right = src.Width;
            }
            if (top < 0)
            {
                top = 0;
            }
            if (bottom > src.Height)
            {
                bottom = src.Height;
            }
            Clip = Rectangle.FromLTRB(left, top, right, bottom);
            currentLocation_ptr = srcData.Scan0 - 4;
        }
        public Color NextPixel()
        {
            bool first = true;
            while (!Clip.Contains(CurrentLocation) || first)
            {
                first = false;
                CurrentLocation.X += 1;
                if (CurrentLocation.X >= srcSize.Width)
                {
                    CurrentLocation.X = 0;
                    CurrentLocation.Y++;
                }
                if (CurrentLocation.Y >= srcSize.Height)
                {
                    throw new ArgumentOutOfRangeException("No more pixels to read");
                }
                currentLocation_ptr += 4;
            }
            return readColor(currentLocation_ptr);
        }
        public void ProcessPixel(Color color)
        {
            writeColor(currentLocation_ptr, color);
        }
        public Color GetPixel(int X, int Y)
        {
            int pxIndex = (srcSize.Width * Y) + X;
            return readColor(scan0 + pxIndex * 4);
        }
        public void SetPixel(int X, int Y, Color color)
        {
            int pxIndex = (srcSize.Width * Y) + X;
            writeColor(scan0 + pxIndex * 4, color);
        }
        private static void writeColor(IntPtr ptr, Color color)
        {
            Marshal.WriteByte(ptr, color.B);
            Marshal.WriteByte(ptr + 1, color.G);
            Marshal.WriteByte(ptr + 2, color.R);
            Marshal.WriteByte(ptr + 3, color.A);
        }
        private static Color readColor(IntPtr ptr)
        {
            return Color.FromArgb(Marshal.ReadByte(ptr + 3), Marshal.ReadByte(ptr + 2), Marshal.ReadByte(ptr + 1), Marshal.ReadByte(ptr));
        }

        public void Dispose()
        {
            if (AllowCacheing)
            {
                cachedData.Add(new CacheEntry(src, srcData));
            } else
            {
                src.UnlockBits(srcData);
                srcData = null;
            }
        }
        private class CacheEntry
        {
            public Bitmap b;
            public BitmapData bData;
            public CacheEntry(Bitmap b, BitmapData bData)
            {
                this.b = b;
                this.bData = bData;
            }
        }
    }
}
