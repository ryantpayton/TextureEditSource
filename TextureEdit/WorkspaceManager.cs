using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Windows;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;

namespace TextureEdit
{
    public class WorkspaceManager
    {
        public Action OnWorkspaceChange;
        public Bitmap Source;
        public Bitmap Destination;
        public WorkspaceDisplayMode EditingDisplayMode = WorkspaceDisplayMode.Texture;
        public bool SuspendDrawing = false;
        public List<Point> SmartSelection = new List<Point>();

        private Graphics sourceGraphics;
        private Graphics destinationGraphics;
        private Stack<UndoEntry> undoStack = new Stack<UndoEntry>();
        private Stack<UndoEntry> redoStack = new Stack<UndoEntry>();
        private bool suspendSmartSelect = false;
        private Bitmap smartSelectOverlayBitmap = null;

        private Point hover = new Point(-1, -1);
        private Rectangle selection = new Rectangle(-1, -1, -1, -1);
        private int brushSize = 1;
        private bool brushMode = true;
        private int highlightOpacity = 200;

        private Bitmap previewBase = null;

        public Point Hover
        {
            get
            {
                return hover;
            }
            set
            {
                if (value != hover)
                {
                    hover = value;
                    Redraw();
                }
            }
        }
        public Rectangle Selection
        {
            get
            {
                return selection;
            }
            set
            {
                if (value != selection)
                {
                    selection = value;
                    Redraw();
                }
            }
        }
        public int BrushSize
        {
            get
            {
                return brushSize;
            }
            set
            {
                int val = value;
                if (val < 1)
                {
                    val = 1;
                }
                brushSize = val;
                Redraw();
            }
        }
        public bool BrushMode
        {
            get
            {
                return brushMode;
            } set
            {
                brushMode = value;
                Redraw();
            }
        }
        public int HighlightOpacity
        {
            get
            {
                return highlightOpacity;
            }
            set
            {
                highlightOpacity = value;
                Redraw();
            }
        }
        public bool EffectsMode
        {
            get
            {
                return (previewBase != null);
            }
        }
        public void FillSelection(Color color)
        {
            if (previewBase != null)
            {
                return;
            }
            if (SmartSelection.Count > 0 && !suspendSmartSelect)
            {
                /*suspendSmartSelect = true;
                Rectangle originalSelect = Selection;
                SuspendDrawing = true;
                foreach (Point p in SmartSelection)
                {
                    selection = new Rectangle(p, new Size(1, 1));
                    FillSelection(color);
                }
                SuspendDrawing = false;
                Selection = originalSelect;
                suspendSmartSelect = false;*/
                using (BitmapProcessor bp = new BitmapProcessor(Source))
                {
                    if (EditingDisplayMode == WorkspaceDisplayMode.Texture)
                    {
                        foreach (Point p in SmartSelection)
                        {
                            bp.SetPixel(p.X, p.Y, color);
                        }
                    } else if (EditingDisplayMode == WorkspaceDisplayMode.Metallic || EditingDisplayMode == WorkspaceDisplayMode.HeightMap)
                    {
                        foreach (Point p in SmartSelection)
                        {
                            Color getColor = bp.GetPixel(p.X, p.Y);
                            bp.SetPixel(p.X, p.Y, Color.FromArgb(color.R, getColor.G, getColor.B));
                        }
                    } else if (EditingDisplayMode == WorkspaceDisplayMode.Emissive)
                    {
                        foreach (Point p in SmartSelection)
                        {
                            Color getColor = bp.GetPixel(p.X, p.Y);
                            bp.SetPixel(p.X, p.Y, Color.FromArgb(getColor.R, color.R, getColor.B));
                        }
                    } else if (EditingDisplayMode == WorkspaceDisplayMode.Roughness)
                    {
                        foreach (Point p in SmartSelection)
                        {
                            Color getColor = bp.GetPixel(p.X, p.Y);
                            bp.SetPixel(p.X, p.Y, Color.FromArgb(getColor.R, getColor.G, color.R));
                        }
                    }
                }
                Redraw();
                return;
            }
            Rectangle fillRegion;
            if (selection.X != -1)
            {
                fillRegion = selection;
            } else if (hover.X != -1)
            {
                fillRegion = new Rectangle(hover.X - brushSize / 2, hover.Y - brushSize / 2, brushSize, brushSize);
            } else
            {
                return;
            }
            if (EditingDisplayMode == WorkspaceDisplayMode.Texture)
            {
                sourceGraphics.Clip = new Region(fillRegion);
                sourceGraphics.Clear(color);
                sourceGraphics.ResetClip();
            } else if (EditingDisplayMode == WorkspaceDisplayMode.Metallic || EditingDisplayMode == WorkspaceDisplayMode.HeightMap)
            {
                chanelSpecificFill(Source, fillRegion, color.R, ChanelType.R);
            } else if (EditingDisplayMode == WorkspaceDisplayMode.Emissive)
            {
                chanelSpecificFill(Source, fillRegion, color.R, ChanelType.G);
            } else if (EditingDisplayMode == WorkspaceDisplayMode.Roughness)
            {
                chanelSpecificFill(Source, fillRegion, color.R, ChanelType.B);
            }
            Redraw();
        }
        public void DrawLine(Color color, Point p1, Point p2)
        {
            /*Bitmap b;
            if (EditingDisplayMode == WorkspaceDisplayMode.Texture)
            {
                b = Source;
            } else
            {
                b = new Bitmap(Source.Width, Source.Height, Source.PixelFormat);
                BitmapData sourceData = Source.LockBits(new Rectangle(0, 0, Source.Width, Source.Height), ImageLockMode.ReadWrite, Source.PixelFormat);
                BitmapData destData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
                copyChanel(sourceData, destData, displayModeToChanelType());
                color = Color.FromArgb(color.A, color.R, color.R, color.R);
                b.UnlockBits(destData);
                Source.UnlockBits(sourceData);
            }
            using (Graphics g = Graphics.FromImage(b))
            {
                using (Pen p = new Pen(color, brushSize))
                {
                    p.StartCap = System.Drawing.Drawing2D.LineCap.Square;
                    p.EndCap = System.Drawing.Drawing2D.LineCap.Square;
                    g.DrawLine(p, p1, p2);
                }
            }
            if (EditingDisplayMode != WorkspaceDisplayMode.Texture)
            {
                copyGreyscaleToChanel(b, Source, new Rectangle(0, 0, Source.Width, Source.Height), displayModeToChanelType());
                b.Dispose();
            }*/
            int rise = p2.Y - p1.Y;
            int run = p2.X - p1.X;
            double ystep = (double)rise / (double)run;
            double xstep = (double)run / (double)rise;
            if (run < 0)
            {
                xstep = -Math.Abs(xstep);
            } else
            {
                xstep = Math.Abs(xstep);
            }
            if (rise < 0)
            {
                ystep = -Math.Abs(ystep);
            } else
            {
                ystep = Math.Abs(ystep);
            }
            double x = p1.X;
            double y = p1.Y;
            //Console.WriteLine(xstep + " " + ystep);
            BitmapProcessor.AllowCacheing = true;
            while (compare(new Point((int)x, (int)y), p1, p2))
            {
                Rectangle fillRegion = new Rectangle((int)x - brushSize / 2, (int)y - brushSize / 2, brushSize, brushSize);
                if (EditingDisplayMode == WorkspaceDisplayMode.Texture)
                {
                    sourceGraphics.Clip = new Region(fillRegion);
                    sourceGraphics.Clear(color);
                    sourceGraphics.ResetClip();
                }
                else if (EditingDisplayMode == WorkspaceDisplayMode.Metallic || EditingDisplayMode == WorkspaceDisplayMode.HeightMap)
                {
                    chanelSpecificFill(Source, fillRegion, color.R, ChanelType.R);
                }
                else if (EditingDisplayMode == WorkspaceDisplayMode.Emissive)
                {
                    chanelSpecificFill(Source, fillRegion, color.R, ChanelType.G);
                }
                else if (EditingDisplayMode == WorkspaceDisplayMode.Roughness)
                {
                    chanelSpecificFill(Source, fillRegion, color.R, ChanelType.B);
                }
                if (Math.Abs(ystep) < 1)
                {
                    if (xstep < 0)
                    {
                        x--;
                    } else
                    {
                        x++;
                    }
                    y += ystep;
                } else
                {
                    if (ystep < 0)
                    {
                        y--;
                    } else
                    {
                        y++;
                    }
                    x += xstep;
                }
            }
            BitmapProcessor.DisableCache();
        }
        private bool compare(Point p, Point start, Point end)
        {
            if (start.X > end.X)
            {
                if (p.X < end.X)
                {
                    return false;
                }
            } else
            {
                if (p.X > end.X)
                {
                    return false;
                }
            }
            if (start.Y > end.Y)
            {
                if (p.Y < end.Y)
                {
                    return false;
                }
            }
            else
            {
                if (p.Y > end.Y)
                {
                    return false;
                }
            }
            return true;
        }
        private ChanelType displayModeToChanelType()
        {
            if (EditingDisplayMode == WorkspaceDisplayMode.Metallic || EditingDisplayMode == WorkspaceDisplayMode.HeightMap)
            {
                return ChanelType.R;
            } else if (EditingDisplayMode == WorkspaceDisplayMode.Emissive)
            {
                return ChanelType.G;
            } else if (EditingDisplayMode == WorkspaceDisplayMode.Roughness)
            {
                return ChanelType.B;
            } else
            {
                return ChanelType.None;
            }
        }
        public Color SampleColor(int x, int y)
        {
            Color clr = Source.GetPixel(x, y);
            if (EditingDisplayMode == WorkspaceDisplayMode.Metallic || EditingDisplayMode == WorkspaceDisplayMode.HeightMap)
            {
                clr = Color.FromArgb(255, clr.R, clr.R, clr.R);
            } else if (EditingDisplayMode == WorkspaceDisplayMode.Emissive)
            {
                clr = Color.FromArgb(255, clr.G, clr.G, clr.G);
            } else if (EditingDisplayMode == WorkspaceDisplayMode.Roughness)
            {
                clr = Color.FromArgb(255, clr.B, clr.B, clr.B);
            }
            return clr;
        }
        public void PreviewAddNoiseToSelection(int amount, Random rand = null)
        {
            if (previewBase == null)
            {
                previewBase = ArgbClipboard.CloneImage(Source);
            }
            if (rand == null)
            {
                rand = new Random();
            }
            Rectangle fillRegion = selection;
            using (BitmapProcessor previewBaseProcessor = new BitmapProcessor(previewBase, fillRegion))
            {
                using (BitmapProcessor sourceProcessor = new BitmapProcessor(Source, fillRegion))
                {
                    if (SmartSelection.Count > 0)
                    {
                        foreach (Point p in SmartSelection)
                        {
                            Color clr = previewBaseProcessor.GetPixel(p.X, p.Y);
                            int add = rand.Next(-amount, amount);
                            int r = clr.R;
                            int g = clr.G;
                            int b = clr.B;

                            if (EditingDisplayMode == WorkspaceDisplayMode.Texture || EditingDisplayMode == WorkspaceDisplayMode.Metallic || EditingDisplayMode == WorkspaceDisplayMode.HeightMap)
                            {
                                r = clr.R + add;
                            }
                            if (EditingDisplayMode == WorkspaceDisplayMode.Texture || EditingDisplayMode == WorkspaceDisplayMode.Emissive)
                            {
                                g = clr.G + add;
                            }
                            if (EditingDisplayMode == WorkspaceDisplayMode.Roughness || EditingDisplayMode == WorkspaceDisplayMode.Texture)
                            {
                                b = clr.B + add;
                            }
                            if (r > 255)
                            {
                                r = 255;
                            }
                            if (r < 0)
                            {
                                r = 0;
                            }
                            if (g > 255)
                            {
                                g = 255;
                            }
                            if (g < 0)
                            {
                                g = 0;
                            }
                            if (b > 255)
                            {
                                b = 255;
                            }
                            if (b < 0)
                            {
                                b = 0;
                            }
                            clr = Color.FromArgb(clr.A, r, g, b);
                            sourceProcessor.SetPixel(p.X, p.Y, clr);
                        }
                    } else
                    {
                        while (previewBaseProcessor.PixelAvailable)
                        {
                            Color clr = previewBaseProcessor.NextPixel();
                            sourceProcessor.NextPixel();
                            int add = rand.Next(-amount, amount);
                            int r = clr.R;
                            int g = clr.G;
                            int b = clr.B;

                            if (EditingDisplayMode == WorkspaceDisplayMode.Texture || EditingDisplayMode == WorkspaceDisplayMode.Metallic || EditingDisplayMode == WorkspaceDisplayMode.HeightMap)
                            {
                                r = clr.R + add;
                            }
                            if (EditingDisplayMode == WorkspaceDisplayMode.Texture || EditingDisplayMode == WorkspaceDisplayMode.Emissive)
                            {
                                g = clr.G + add;
                            }
                            if (EditingDisplayMode == WorkspaceDisplayMode.Roughness || EditingDisplayMode == WorkspaceDisplayMode.Texture)
                            {
                                b = clr.B + add;
                            }
                            if (r > 255)
                            {
                                r = 255;
                            }
                            if (r < 0)
                            {
                                r = 0;
                            }
                            if (g > 255)
                            {
                                g = 255;
                            }
                            if (g < 0)
                            {
                                g = 0;
                            }
                            if (b > 255)
                            {
                                b = 255;
                            }
                            if (b < 0)
                            {
                                b = 0;
                            }
                            clr = Color.FromArgb(clr.A, r, g, b);
                            sourceProcessor.ProcessPixel(clr);
                        }
                    }
                }
            }
            Redraw();
        }
        private unsafe void chanelSpecificFill(Bitmap destination, Rectangle fillRegion, byte value, ChanelType chanel)
        {
            using (BitmapProcessor destProcessor = new BitmapProcessor(destination, fillRegion))
            {
                /*while (destProcessor.PixelAvailable)
                {
                    Color clr = destProcessor.NextPixel();
                    if (chanel == ChanelType.B)
                    {
                        clr = Color.FromArgb(clr.A, clr.R, clr.G, value);
                    }
                    else if (chanel == ChanelType.G)
                    {
                        clr = Color.FromArgb(clr.A, clr.R, value, clr.B);
                    }
                    else if (chanel == ChanelType.R)
                    {
                        clr = Color.FromArgb(clr.A, value, clr.G, clr.B);
                    }
                    else if (chanel == ChanelType.A)
                    {
                        clr = Color.FromArgb(value, clr.R, clr.G, clr.B);
                    }
                    destProcessor.ProcessPixel(clr);
                }*/
                byte* destArr = (byte*)destProcessor.srcData.Scan0;
                int offset;
                if (chanel == ChanelType.B)
                {
                    offset = 0;
                }
                else if (chanel == ChanelType.G)
                {
                    offset = 1;
                }
                else if (chanel == ChanelType.R)
                {
                    offset = 2;
                }
                else
                {
                    offset = 3;
                }
                int width = destination.Width;
                int height = destination.Height;
                for (int y = fillRegion.Y; y < fillRegion.Y + fillRegion.Height; y++)
                {
                    for (int x = fillRegion.X; x < fillRegion.X + fillRegion.Width; x++)
                    {
                        if (x >= width)
                        {
                            continue;
                        }
                        if (x < 0)
                        {
                            continue;
                        }
                        if (y >= height)
                        {
                            continue;
                        }
                        if (y < 0)
                        {
                            continue;
                        }
                        int basePtr = ((y * width) + x) * 4;
                        destArr[basePtr + offset] = value;
                    }
                }
            }
        }
        public void CopySelection()
        {
            if (selection.X != -1)
            {
                Bitmap clipboard = new Bitmap(selection.Width, selection.Height, PixelFormat.Format32bppArgb);
                Graphics clipboardG = Graphics.FromImage(clipboard);
                clipboardG.DrawImage(Source, 0, 0, selection, GraphicsUnit.Pixel);
                if (EditingDisplayMode != WorkspaceDisplayMode.Texture)
                {
                    BitmapData bmpData = clipboard.LockBits(new Rectangle(0, 0, clipboard.Width, clipboard.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                    ChanelType chanelType = ChanelType.A;
                    if (EditingDisplayMode == WorkspaceDisplayMode.Metallic || EditingDisplayMode == WorkspaceDisplayMode.HeightMap)
                    {
                        chanelType = ChanelType.R;
                    } else if (EditingDisplayMode == WorkspaceDisplayMode.Emissive)
                    {
                        chanelType = ChanelType.G;
                    } else if (EditingDisplayMode == WorkspaceDisplayMode.Roughness)
                    {
                        chanelType = ChanelType.B;
                    }
                    copyChanel(bmpData, bmpData, chanelType);
                    clipboard.UnlockBits(bmpData);
                }
                ArgbClipboard.CopyImage(clipboard);
                clipboardG.Dispose();
                clipboard.Dispose();
            }
        }
        public void ExportWorkspace(string path)
        {
            Bitmap exp = new Bitmap(Source.Width, Source.Height, PixelFormat.Format32bppArgb);
            Graphics expG = Graphics.FromImage(exp);
            expG.DrawImage(Source, new Point(0, 0));
            if (EditingDisplayMode != WorkspaceDisplayMode.Texture)
            {
                BitmapData bmpData = exp.LockBits(new Rectangle(0, 0, exp.Width, exp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                ChanelType chanelType = ChanelType.A;
                if (EditingDisplayMode == WorkspaceDisplayMode.Metallic || EditingDisplayMode == WorkspaceDisplayMode.HeightMap)
                {
                    chanelType = ChanelType.R;
                }
                else if (EditingDisplayMode == WorkspaceDisplayMode.Emissive)
                {
                    chanelType = ChanelType.G;
                }
                else if (EditingDisplayMode == WorkspaceDisplayMode.Roughness)
                {
                    chanelType = ChanelType.B;
                }
                copyChanel(bmpData, bmpData, chanelType);
                exp.UnlockBits(bmpData);
            }
            exp.Save(path);
            exp.Dispose();
        }
        public void PasteToSelection()
        {
            Bitmap clipboard = ArgbClipboard.GetImage();
            if (clipboard == null)
            {
                return;
            }
            SaveToUndo();
            //sourceGraphics.Clear(Color.Transparent);
            if (EditingDisplayMode == WorkspaceDisplayMode.Texture)
            {
                sourceGraphics.Clip = new Region(selection);
                sourceGraphics.DrawImageUnscaled(clipboard, selection.X, selection.Y);
                sourceGraphics.ResetClip();
            } else if (EditingDisplayMode == WorkspaceDisplayMode.Metallic || EditingDisplayMode == WorkspaceDisplayMode.HeightMap)
            {
                copyGreyscaleToChanel(clipboard, Source, selection, ChanelType.R);
            } else if (EditingDisplayMode == WorkspaceDisplayMode.Emissive)
            {
                copyGreyscaleToChanel(clipboard, Source, selection, ChanelType.G);
            } else if (EditingDisplayMode == WorkspaceDisplayMode.Roughness)
            {
                copyGreyscaleToChanel(clipboard, Source, selection, ChanelType.B);
            }
            Redraw();
            clipboard.Dispose();
        }
        public void MagicWand(Point p)
        {
            SmartSelection.Clear();
            selection = new Rectangle(-1, -1, -1, -1);
            bool[,] workingSmartSelection = new bool[Source.Width, Source.Height];
            workingSmartSelection[p.X, p.Y] = true;
            bool[,] checkedPoints = new bool[Source.Width, Source.Height];
            Stack<Point> checkQueue = new Stack<Point>();
            using (BitmapProcessor bp = new BitmapProcessor(Source))
            {
                Color searchColor = bp.GetPixel(p.X, p.Y);
                checkQueue.Push(p);
                while (checkQueue.Count > 0)
                {
                    Point currentPoint = checkQueue.Pop();
                    magicWand(currentPoint, searchColor, bp, checkQueue, workingSmartSelection, checkedPoints);
                }
            }
            for (int x = 0; x < Source.Width; x++)
            {
                for (int y = 0; y < Source.Height; y++)
                {
                    if (workingSmartSelection[x, y])
                    {
                        SmartSelection.Add(new Point(x, y));
                    }
                }
            }
            Redraw();
        }
        public void ShiftMagicWand(Point p)
        {
            SmartSelection.Clear();
            using (BitmapProcessor bp = new BitmapProcessor(Source))
            {
                Color matchColor = bp.GetPixel(p.X, p.Y);
                while (bp.PixelAvailable)
                {
                    Color thisColor = bp.NextPixel();
                    if (colorsEqual(matchColor, thisColor))
                    {
                        SmartSelection.Add(bp.CurrentLocation);
                    }
                }
            }
        }
        private void magicWand(Point currentPoint, Color searchColor, BitmapProcessor bp, Stack<Point> checkQueue, bool[,] workingSmartSelect, bool[,] checkedPoints)
        {
            Point left = new Point(currentPoint.X - 1, currentPoint.Y);
            Point right = new Point(currentPoint.X + 1, currentPoint.Y);
            Point below = new Point(currentPoint.X, currentPoint.Y - 1);
            Point above = new Point(currentPoint.X, currentPoint.Y + 1);
            if (inRange(left, bp.Clip))
            {
                if (!checkedPoints[left.X, left.Y] && colorsEqual(bp.GetPixel(left.X, left.Y), searchColor))
                {
                    workingSmartSelect[left.X, left.Y] = true;
                    checkQueue.Push(left);
                }
                checkedPoints[left.X, left.Y] = true;
            }
            if (inRange(right, bp.Clip))
            {
                if (!checkedPoints[right.X, right.Y] && colorsEqual(bp.GetPixel(right.X, right.Y), searchColor))
                {
                    workingSmartSelect[right.X, right.Y] = true;
                    checkQueue.Push(right);
                }
                checkedPoints[right.X, right.Y] = true;
            }
            if (inRange(above, bp.Clip))
            {
                if (!checkedPoints[above.X, above.Y] && colorsEqual(bp.GetPixel(above.X, above.Y), searchColor))
                {
                    workingSmartSelect[above.X, above.Y] = true;
                    checkQueue.Push(above);
                }
                checkedPoints[above.X, above.Y] = true;
            }
            if (inRange(below, bp.Clip))
            {
                if (!checkedPoints[below.X, below.Y] && colorsEqual(bp.GetPixel(below.X, below.Y), searchColor))
                {
                    workingSmartSelect[below.X, below.Y] = true;
                    checkQueue.Push(below);
                }
                checkedPoints[below.X, below.Y] = true;
            }
        }
        private bool inRange(Point currentPoint, Rectangle clip)
        {
            if (currentPoint.X < 0 || currentPoint.X >= clip.Width || currentPoint.Y < 0 || currentPoint.Y >= clip.Height)
            {
                return false;
            } else
            {
                return true;
            }
        }
        private bool colorsEqual(Color c1, Color c2)
        {
            if (EditingDisplayMode == WorkspaceDisplayMode.Texture)
            {
                return ((c1.R == c2.R) && (c1.G == c2.G) && (c1.B == c2.B) && (c1.A == c2.A));
            } else if (EditingDisplayMode == WorkspaceDisplayMode.Metallic || EditingDisplayMode == WorkspaceDisplayMode.HeightMap)
            {
                return (c1.R == c2.R);
            } else if (EditingDisplayMode == WorkspaceDisplayMode.Emissive)
            {
                return (c1.G == c2.G);
            } else if (EditingDisplayMode == WorkspaceDisplayMode.Roughness)
            {
                return (c1.B == c2.B);
            }
            return false;
            
        }
        private bool smartSelectIncludes(Point p)
        {
            foreach (Point pp in SmartSelection)
            {
                if (p == pp)
                {
                    return true;
                }
            }
            return false;
        }
        private bool includes(List<Point> points, Point p)
        {
            foreach (Point pp in points)
            {
                if (p == pp)
                {
                    return true;
                }
            }
            return false;
        }
        public void ImportWorkspace(string path)
        {
            Bitmap imp = new Bitmap(Source.Width, Source.Height, PixelFormat.Format32bppArgb);
            using (Bitmap b = (Bitmap)Bitmap.FromFile(path))
            {
                using (Graphics g = Graphics.FromImage(imp))
                {
                    g.DrawImage(b, new Rectangle(0, 0, b.Width, b.Height));
                }
            }
            if (EditingDisplayMode == WorkspaceDisplayMode.Texture)
            {
                sourceGraphics.Clear(Color.Transparent);
                sourceGraphics.DrawImageUnscaled(imp, 0, 0);
            } else
            {
                if (!IsGreyscale(imp))
                {
                    ColorWarning cw = new ColorWarning();
                    cw.ShowDialog();
                    if (!cw.result)
                    {
                        Redraw();
                        imp.Dispose();
                        return;
                    }
                }
                sourceGraphics.Clear(Color.Transparent);
                if (EditingDisplayMode == WorkspaceDisplayMode.Metallic || EditingDisplayMode == WorkspaceDisplayMode.HeightMap)
                {
                    copyGreyscaleToChanel(imp, Source, new Rectangle(0, 0, Source.Width, Source.Height), ChanelType.R);
                }
                else if (EditingDisplayMode == WorkspaceDisplayMode.Emissive)
                {
                    copyGreyscaleToChanel(imp, Source, new Rectangle(0, 0, Source.Width, Source.Height), ChanelType.G);
                }
                else if (EditingDisplayMode == WorkspaceDisplayMode.Roughness)
                {
                    copyGreyscaleToChanel(imp, Source, new Rectangle(0, 0, Source.Width, Source.Height), ChanelType.B);
                }
            }
            
            Redraw();
            imp.Dispose();
        }
        private void copyGreyscaleToChanel(Bitmap src, Bitmap dest, Rectangle destClip, ChanelType chanel)
        {
            if (destClip.Width > src.Width)
            {
                destClip.Width = src.Width;
            }
            if (destClip.Height > src.Height)
            {
                destClip.Height = src.Height;
            }
            using (BitmapProcessor srcProcessor = new BitmapProcessor(src))
            {
                using (BitmapProcessor destProcessor = new BitmapProcessor(dest, destClip))
                {
                    while (destProcessor.PixelAvailable)
                    {
                        Color srcClr = srcProcessor.NextPixel();
                        int lightness = (srcClr.R + srcClr.G + srcClr.B) / 3;
                        lightness = (int)(lightness * (srcClr.A / 255F));
                        Color destClr = destProcessor.NextPixel();
                        if (chanel == ChanelType.R)
                        {
                            destClr = Color.FromArgb(255, lightness, destClr.G, destClr.B);
                        }
                        else if (chanel == ChanelType.G)
                        {
                            destClr = Color.FromArgb(255, destClr.R, lightness, destClr.B);
                        }
                        else if (chanel == ChanelType.B)
                        {
                            destClr = Color.FromArgb(255, destClr.R, destClr.G, lightness);
                        }
                        destProcessor.ProcessPixel(destClr);
                    }
                }
            }
        }
        public void EnforceHeightMap(Bitmap bmp)
        {
            using (BitmapProcessor bmpP = new BitmapProcessor(bmp))
            {
                while (bmpP.PixelAvailable)
                {
                    Color clr = bmpP.NextPixel();
                    bmpP.ProcessPixel(Color.FromArgb(255, clr.R, 0, 0));
                }
            }
        }
        public void PreviewGreyscaleAdjustSelection(double Contrast, int Offset, bool Invert, bool OffsetFirst)
        {
            if (previewBase == null)
            {
                previewBase = ArgbClipboard.CloneImage(Source);
            }
            ChanelType chanelType = ChanelType.A;
            if (EditingDisplayMode == WorkspaceDisplayMode.Metallic || EditingDisplayMode == WorkspaceDisplayMode.HeightMap)
            {
                chanelType = ChanelType.R;
            } else if (EditingDisplayMode == WorkspaceDisplayMode.Emissive)
            {
                chanelType = ChanelType.G;
            } else if (EditingDisplayMode == WorkspaceDisplayMode.Roughness)
            {
                chanelType = ChanelType.B;
            } else if (EditingDisplayMode == WorkspaceDisplayMode.Texture)
            {
                chanelType = ChanelType.All;
            }
            greyscaleAdjust(previewBase, Source, selection, Contrast, Offset, Invert, OffsetFirst, chanelType);
            Redraw();
        }
        public void PreviewColorShiftSelection(float R, float G, float B, int A, bool HSLMode)
        {
            if (EditingDisplayMode != WorkspaceDisplayMode.Texture)
            {
                return;
            }
            if (previewBase == null)
            {
                previewBase = ArgbClipboard.CloneImage(Source);
            }
            colorShift(previewBase, Source, selection, R, G, B, A, HSLMode);
            Redraw();
        }
        public void CommitEffect()
        {
            undoStack.Push(new UndoEntry(previewBase, Source));
            previewBase.Dispose();
            previewBase = null;
            Redraw();
        }
       public void CancelEffect()
        {
            sourceGraphics.Clear(Color.Transparent);
            sourceGraphics.DrawImageUnscaled(previewBase, new Point(0, 0));
            previewBase.Dispose();
            previewBase = null;
            InitializeWorkspace();
        }
        private void greyscaleAdjust(Bitmap src, Bitmap dest, Rectangle rect, double contrast, int offset, bool invert, bool offsetFirst, ChanelType chanelType)
        {
            using (BitmapProcessor srcP = new BitmapProcessor(src, rect))
            {
                using (BitmapProcessor destP = new BitmapProcessor(dest, rect))
                {
                    if (SmartSelection.Count > 0)
                    {
                        foreach (Point p in SmartSelection)
                        {
                            Color pxColor = srcP.GetPixel(p.X, p.Y);
                            double val = 0;
                            if (chanelType == ChanelType.A)
                            {
                                val = pxColor.A;
                            }
                            else if (chanelType == ChanelType.R)
                            {
                                val = pxColor.R;
                            }
                            else if (chanelType == ChanelType.G)
                            {
                                val = pxColor.G;
                            }
                            else if (chanelType == ChanelType.B)
                            {
                                val = pxColor.B;
                            }
                            else if (chanelType == ChanelType.All)
                            {
                                val = pxColor.R;
                            }
                            if (offsetFirst)
                            {
                                val += offset;
                            }
                            val /= 255;
                            if (invert)
                            {
                                val = 1 - val;
                            }
                            val -= 0.5;

                            val *= contrast;

                            val += 0.5;
                            val *= 255;

                            if (!offsetFirst)
                            {
                                val += offset;
                            }
                            if (val > 255)
                            {
                                val = 255;
                            }
                            if (val < 0)
                            {
                                val = 0;
                            }
                            Color replaceColor = Color.Black;
                            if (chanelType == ChanelType.A)
                            {
                                replaceColor = Color.FromArgb((int)Math.Round(val), pxColor.R, pxColor.G, pxColor.B);
                            }
                            else if (chanelType == ChanelType.R)
                            {
                                replaceColor = Color.FromArgb(pxColor.A, (int)Math.Round(val), pxColor.G, pxColor.B);
                            }
                            else if (chanelType == ChanelType.G)
                            {
                                replaceColor = Color.FromArgb(pxColor.A, pxColor.R, (int)Math.Round(val), pxColor.B);
                            }
                            else if (chanelType == ChanelType.B)
                            {
                                replaceColor = Color.FromArgb(pxColor.A, pxColor.R, pxColor.G, (int)Math.Round(val));
                            }
                            else if (chanelType == ChanelType.All)
                            {
                                replaceColor = Color.FromArgb(pxColor.A, (int)Math.Round(val), (int)Math.Round(val), (int)Math.Round(val));
                            }
                            destP.SetPixel(p.X, p.Y, replaceColor);
                        }
                    } else
                    {
                        while (srcP.PixelAvailable)
                        {
                            Color pxColor = srcP.NextPixel();
                            destP.NextPixel();
                            double val = 0;
                            if (chanelType == ChanelType.A)
                            {
                                val = pxColor.A;
                            }
                            else if (chanelType == ChanelType.R)
                            {
                                val = pxColor.R;
                            }
                            else if (chanelType == ChanelType.G)
                            {
                                val = pxColor.G;
                            }
                            else if (chanelType == ChanelType.B)
                            {
                                val = pxColor.B;
                            }
                            else if (chanelType == ChanelType.All)
                            {
                                val = pxColor.R;
                            }
                            if (offsetFirst)
                            {
                                val += offset;
                            }
                            val /= 255;
                            if (invert)
                            {
                                val = 1 - val;
                            }
                            val -= 0.5;

                            val *= contrast;

                            val += 0.5;
                            val *= 255;

                            if (!offsetFirst)
                            {
                                val += offset;
                            }
                            if (val > 255)
                            {
                                val = 255;
                            }
                            if (val < 0)
                            {
                                val = 0;
                            }
                            Color replaceColor = Color.Black;
                            if (chanelType == ChanelType.A)
                            {
                                replaceColor = Color.FromArgb((int)Math.Round(val), pxColor.R, pxColor.G, pxColor.B);
                            }
                            else if (chanelType == ChanelType.R)
                            {
                                replaceColor = Color.FromArgb(pxColor.A, (int)Math.Round(val), pxColor.G, pxColor.B);
                            }
                            else if (chanelType == ChanelType.G)
                            {
                                replaceColor = Color.FromArgb(pxColor.A, pxColor.R, (int)Math.Round(val), pxColor.B);
                            }
                            else if (chanelType == ChanelType.B)
                            {
                                replaceColor = Color.FromArgb(pxColor.A, pxColor.R, pxColor.G, (int)Math.Round(val));
                            }
                            else if (chanelType == ChanelType.All)
                            {
                                replaceColor = Color.FromArgb(pxColor.A, (int)Math.Round(val), (int)Math.Round(val), (int)Math.Round(val));
                            }
                            destP.ProcessPixel(replaceColor);
                        }
                    }
                }
            }
        }
        private void colorShift(Bitmap src, Bitmap dest, Rectangle rect, float r, float g, float b, int a, bool hslMode)
        {
            using (BitmapProcessor srcP = new BitmapProcessor(src, rect))
            {
                using (BitmapProcessor destP = new BitmapProcessor(dest, rect))
                {
                    if (SmartSelection.Count > 0)
                    {
                        foreach (Point p in SmartSelection)
                        {
                            Color pxColor = srcP.GetPixel(p.X, p.Y);
                            int newR = 0;
                            int newG = 0;
                            int newB = 0;
                            int newA = 0;

                            if (hslMode)
                            {
                                ColorRGB colorRGB = (ColorRGB)pxColor;
                                float h = colorRGB.H + r;
                                float s = colorRGB.S + g;
                                float l = colorRGB.L + b;
                                if (h > 1)
                                {
                                    h -= 1;
                                }
                                if (h < 0)
                                {
                                    h += 1;
                                }
                                if (s > 1)
                                {
                                    s = 1;
                                }
                                if (s < 0)
                                {
                                    s = 0;
                                }
                                if (l > 1)
                                {
                                    l = 1;
                                }
                                if (l < 0)
                                {
                                    l = 0;
                                }
                                colorRGB = ColorRGB.FromHSL(h, s, l);
                                newR = colorRGB.R;
                                newG = colorRGB.G;
                                newB = colorRGB.B;
                                newA = pxColor.A + a;
                            }
                            else
                            {
                                newR = pxColor.R + (int)r;
                                newG = pxColor.G + (int)g;
                                newB = pxColor.B + (int)b;
                                newA = pxColor.A + a;
                            }
                            if (newR > 255)
                            {
                                newR = 255;
                            }
                            if (newR < 0)
                            {
                                newR = 0;
                            }
                            if (newG > 255)
                            {
                                newG = 255;
                            }
                            if (newG < 0)
                            {
                                newG = 0;
                            }
                            if (newB > 255)
                            {
                                newB = 255;
                            }
                            if (newB < 0)
                            {
                                newB = 0;
                            }
                            if (newA > 255)
                            {
                                newA = 255;
                            }
                            if (newA < 0)
                            {
                                newA = 0;
                            }
                            destP.SetPixel(p.X, p.Y, Color.FromArgb(newA, newR, newG, newB));
                        }
                    } else
                    {
                        while (srcP.PixelAvailable)
                        {
                            Color pxColor = srcP.NextPixel();
                            destP.NextPixel();
                            int newR = 0;
                            int newG = 0;
                            int newB = 0;
                            int newA = 0;

                            if (hslMode)
                            {
                                ColorRGB colorRGB = (ColorRGB)pxColor;
                                float h = colorRGB.H + r;
                                float s = colorRGB.S + g;
                                float l = colorRGB.L + b;
                                if (h > 1)
                                {
                                    h -= 1;
                                }
                                if (h < 0)
                                {
                                    h += 1;
                                }
                                if (s > 1)
                                {
                                    s = 1;
                                }
                                if (s < 0)
                                {
                                    s = 0;
                                }
                                if (l > 1)
                                {
                                    l = 1;
                                }
                                if (l < 0)
                                {
                                    l = 0;
                                }
                                colorRGB = ColorRGB.FromHSL(h, s, l);
                                newR = colorRGB.R;
                                newG = colorRGB.G;
                                newB = colorRGB.B;
                                newA = pxColor.A + a;
                            }
                            else
                            {
                                newR = pxColor.R + (int)r;
                                newG = pxColor.G + (int)g;
                                newB = pxColor.B + (int)b;
                                newA = pxColor.A + a;
                            }
                            if (newR > 255)
                            {
                                newR = 255;
                            }
                            if (newR < 0)
                            {
                                newR = 0;
                            }
                            if (newG > 255)
                            {
                                newG = 255;
                            }
                            if (newG < 0)
                            {
                                newG = 0;
                            }
                            if (newB > 255)
                            {
                                newB = 255;
                            }
                            if (newB < 0)
                            {
                                newB = 0;
                            }
                            if (newA > 255)
                            {
                                newA = 255;
                            }
                            if (newA < 0)
                            {
                                newA = 0;
                            }
                            destP.ProcessPixel(Color.FromArgb(newA, newR, newG, newB));
                        }
                    }
                }
            }
        }
        private void updateSmartSelectOverlay()
        {
            if (smartSelectOverlayBitmap == null)
            {
                smartSelectOverlayBitmap = new Bitmap(Source.Width, Source.Height, PixelFormat.Format32bppArgb);
            } else if (smartSelectOverlayBitmap.Size != Source.Size)
            {
                smartSelectOverlayBitmap.Dispose();
                smartSelectOverlayBitmap = new Bitmap(Source.Width, Source.Height, PixelFormat.Format32bppArgb);
            }
            using (Graphics g = Graphics.FromImage(smartSelectOverlayBitmap))
                g.Clear(Color.Transparent);
            using (BitmapProcessor bp = new BitmapProcessor(smartSelectOverlayBitmap))
            {
                foreach (Point p in SmartSelection)
                {
                    bp.SetPixel(p.X, p.Y, Color.FromArgb(highlightOpacity, 0, 128, 255));
                }
            }
        }
        public void Redraw()
        {
            if (SuspendDrawing)
            {
                return;
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();
            destinationGraphics.Clear(Color.Transparent);
            if (EditingDisplayMode == WorkspaceDisplayMode.Texture)
            {
                destinationGraphics.DrawImageUnscaled(Source, new Point(0, 0));
            } else
            {
                BitmapData sourceBits = Source.LockBits(new Rectangle(0, 0, Source.Width, Source.Height), ImageLockMode.ReadOnly, Source.PixelFormat);
                BitmapData destinationBits = Destination.LockBits(new Rectangle(0, 0, Source.Width, Source.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, Destination.PixelFormat);
                ChanelType chanelToCopy = ChanelType.R;
                if (EditingDisplayMode == WorkspaceDisplayMode.Metallic || EditingDisplayMode == WorkspaceDisplayMode.HeightMap)
                {
                    chanelToCopy = ChanelType.R;
                } else if (EditingDisplayMode == WorkspaceDisplayMode.Emissive)
                {
                    chanelToCopy = ChanelType.G;
                } else if (EditingDisplayMode == WorkspaceDisplayMode.Roughness)
                {
                    chanelToCopy = ChanelType.B;
                }
                copyChanel(sourceBits, destinationBits, chanelToCopy);
                ////Console.WriteLine("Copying Chanel " + chanelToCopy);
                Source.UnlockBits(sourceBits);
                Destination.UnlockBits(destinationBits);
            }
            if (selection.X != -1 && previewBase == null)
            {
                ////Console.WriteLine(selection);
                destinationGraphics.FillRectangle(new SolidBrush(Color.FromArgb(highlightOpacity, 0, 128, 255)), selection);
            }
            if (hover.X != -1)
            {
                int workingBrushSize;
                if (BrushMode)
                {
                    workingBrushSize = brushSize;
                } else
                {
                    workingBrushSize = 1;
                }
                destinationGraphics.FillRectangle(new SolidBrush(Color.FromArgb(highlightOpacity, 0, 128, 255)), new Rectangle(hover.X - workingBrushSize / 2, hover.Y - workingBrushSize / 2, workingBrushSize, workingBrushSize));
            }
            if (SmartSelection.Count > 0 && previewBase == null)
            {
                updateSmartSelectOverlay();
                destinationGraphics.DrawImage(smartSelectOverlayBitmap, new Point(0, 0));
                /*foreach (Point p in SmartSelection)
                {
                    destinationGraphics.FillRectangle(new SolidBrush(Color.FromArgb(highlightOpacity, 0, 128, 255)), new Rectangle(p, new Size(1, 1)));
                }*/
            }
            OnWorkspaceChange();
            sw.Stop();
            //Console.WriteLine(sw.ElapsedMilliseconds + "ms");
        }
        public void SaveToUndo()
        {
            redoStack.Clear();
            undoStack.Push(new UndoEntry(Source));
        }
        public void Redo()
        {
            if (redoStack.Count < 1)
            {
                return;
            }
            redoStack.Pop().Restore(undoStack);
            Redraw();
        }
        public void RestoreFromUndo()
        {
            if (undoStack.Count < 1)
            {
                return;
            }
            undoStack.Pop().Restore(redoStack);
            Redraw();
        }
        public void ClearUndo()
        {
            undoStack.Clear();
            redoStack.Clear();
        }
        public void InitializeWorkspace()
        {
            destinationGraphics = Graphics.FromImage(Destination);
            sourceGraphics = Graphics.FromImage(Source);
            Redraw();
        }
        public bool HasOpacity(Bitmap bmp)
        {
            using (BitmapProcessor bmpP = new BitmapProcessor(bmp))
            {
                while (bmpP.PixelAvailable)
                {
                    if (bmpP.NextPixel().A < 255)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool IsGreyscale(Bitmap bmp)
        {
            using (BitmapProcessor bmpP = new BitmapProcessor(bmp))
            {
                while (bmpP.PixelAvailable)
                {
                    Color px = bmpP.NextPixel();
                    if (px.R != px.G || px.G != px.B || px.B != px.R)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private unsafe void copyChanel(BitmapData source, BitmapData dest, ChanelType chanel)
        {
            IntPtr sourcePtr = source.Scan0;
            IntPtr destPtr = dest.Scan0;
            int size = source.Stride * source.Height;
            byte* sourceArr = (byte*)sourcePtr;
            byte* destArr = (byte*)destPtr;
            int offset;
            if (chanel == ChanelType.B)
            {
                offset = 0;
            } else if (chanel == ChanelType.G)
            {
                offset = 1;
            } else if (chanel == ChanelType.R)
            {
                offset = 2;
            } else
            {
                offset = 3;
            }
            for (int i = 0; i < size; i += 4)
            {
                byte newVal = sourceArr[i + offset];
                destArr[i] = newVal;
                destArr[i + 1] = newVal;
                destArr[i + 2] = newVal;
                destArr[i + 3] = 255;
            }
            /*for (int i = 0; i < size; i += 4)
            {
                byte b = Marshal.ReadByte(IntPtr.Add(sourcePtr, i));
                byte g = Marshal.ReadByte(IntPtr.Add(sourcePtr, i + 1));
                byte r = Marshal.ReadByte(IntPtr.Add(sourcePtr, i + 2));
                byte a = Marshal.ReadByte(IntPtr.Add(sourcePtr, i + 3));
                ////Console.WriteLine(a);

                byte copyB = 0;
                byte copyG = 0;
                byte copyR = 0;
                byte copyA = 255;
                if (chanel == ChanelType.A)
                {
                    copyR = a;
                    copyG = a;
                    copyB = a;
                } else if (chanel == ChanelType.R)
                {
                    copyR = r;
                    copyG = r;
                    copyB = r;
                } else if (chanel == ChanelType.G)
                {
                    copyR = g;
                    copyG = g;
                    copyB = g;
                } else if (chanel == ChanelType.B)
                {
                    copyR = b;
                    copyG = b;
                    copyB = b;
                }
                Marshal.WriteByte(IntPtr.Add(destPtr, i), copyB);
                Marshal.WriteByte(IntPtr.Add(destPtr, i + 1), copyG);
                Marshal.WriteByte(IntPtr.Add(destPtr, i + 2), copyR);
                Marshal.WriteByte(IntPtr.Add(destPtr, i + 3), copyA);
            }*/
        }
    }
    public enum ChanelType : byte
    {
        A = 0,
        R = 1,
        G = 2,
        B = 3,
        None = 4,
        All = 5
    }
    public enum WorkspaceDisplayMode : byte
    {
        Texture = 0,
        Metallic = 1,
        Emissive = 2,
        Roughness = 3,
        HeightMap = 4
    }
    public class UndoEntry
    {
        public Bitmap Bmp;
        public Bitmap UndoSource;
        public UndoEntry(Bitmap Bitmap)
        {
            this.Bmp = new Bitmap(Bitmap.Width, Bitmap.Height, Bitmap.PixelFormat);
            using (Graphics g = Graphics.FromImage(this.Bmp))
            {
                g.Clear(Color.Transparent);
                g.DrawImageUnscaled(Bitmap, new Point(0, 0));
            }
            this.UndoSource = Bitmap;
        }
        public UndoEntry(Bitmap Bitmap, Bitmap UndoSource)
        {
            this.Bmp = new Bitmap(Bitmap.Width, Bitmap.Height, Bitmap.PixelFormat);
            using (Graphics g = Graphics.FromImage(this.Bmp))
            {
                g.Clear(Color.Transparent);
                g.DrawImageUnscaled(Bitmap, new Point(0, 0));
            }
            this.UndoSource = UndoSource;
        }
        public void Restore(Stack<UndoEntry> RedoStack)
        {
            RedoStack.Push(new UndoEntry(UndoSource, UndoSource));
            using (Graphics g = Graphics.FromImage(UndoSource))
            {
                g.Clear(Color.Transparent);
                g.DrawImageUnscaled(Bmp, new Point(0, 0));
            }
            Bmp.Dispose();
        }
    }
}
