using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextureEdit
{
    public partial class TileView : Form
    {
        private Bitmap oldBmp;
        public WorkspaceManager Source;
        private Point last = new Point(-1, -1);
        private List<Bitmap> loadedBmps;
        private int[,] vals;
        public CheckBox SolidBGCheck;
        private Point mouse = new Point(-1, -1);
        int oldWidth = 0;
        int oldHeight = 0;
        bool mouseDown = false;
        bool mouseRight = false;
        
        public TileView(WorkspaceManager Source, CheckBox SolidBGCheck)
        {
            this.Source = Source;
            this.SolidBGCheck = SolidBGCheck;
            oldBmp = Source.Source;
            vals = new int[2, 2];
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 1; y++)
                {
                    vals[x, y] = 0;
                }
            }
            loadedBmps = new List<Bitmap>() { Source.Source };
            InitializeComponent();
            TileView_ResizeEnd(null, null);
        }

        private void TileView_SizeChanged(object sender, EventArgs e)
        {
            if (notificationText.Visible)
            {
                notificationText.Location = new Point(0, 0);
                notificationText.Size = this.Size;
            }
        }

        public void Retile()
        {
            if (oldBmp != Source.Source)
            {
                oldBmp = Source.Source;
                loadedBmps[0] = Source.Source;
                oldWidth = 0;
                oldHeight = 0;
                TileView_ResizeEnd(null, null);
            } else
            {
                canvas.Invalidate();
            }
        }

        private void TileView_ResizeEnd(object sender, EventArgs e)
        {
            notificationText.Visible = false;
            int widthDiff = Math.Abs(this.Width - oldWidth);
            int heightDiff = Math.Abs(this.Height - oldHeight);
            if (widthDiff > heightDiff)
            {
                double aspect = (double)Source.Source.Height / Source.Source.Width;
                this.Height = (int)(aspect * this.Width);
            }
            else
            {
                double aspect = (double)Source.Source.Width / Source.Source.Height;
                this.Width = (int)(aspect * this.Height);
            }
            oldWidth = this.Width;
            oldHeight = this.Height;
            canvas.Size = new Size(this.ClientSize.Width, this.ClientSize.Height);
            canvas.Invalidate();
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
            if (SolidBGCheck.Checked)
            {
                e.Graphics.Clear(TextureEdit.Properties.Settings.Default.SolidBgColor);
            } else
            {
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.DrawImage(TextureEdit.Properties.Resources.transparencybg, new Rectangle(0, 0, Math.Max(oldWidth, oldHeight), Math.Max(oldWidth, oldHeight)));
            }
            //e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            double tileWidth = (double)((double)canvas.Width / (double)tiles.Value);
            double tileHeight = (double)((double)canvas.Height / (double)tiles.Value);
            if (Math.Min(tileWidth, tileHeight) < Math.Max(canvas.Width, canvas.Height))
            {
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            } else
            {
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Default;
            }
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            int thisWidth = canvas.Width;
            int thisHeight = canvas.Height;
            int ceilingTileWidth = (int)Math.Ceiling(tileWidth);
            int ceilingTileHeight = (int)Math.Ceiling(tileHeight);
            int ix = 0;
            for (double x = 0; x < thisWidth; x += tileWidth)
            {
                int iy = 0;
                for (double y = 0; y < thisHeight; y += tileHeight)
                {
                    Rectangle rect = new Rectangle((int)Math.Round(x), (int)Math.Round(y), ceilingTileWidth, ceilingTileHeight);
                    if (ix < tiles.Value && iy < tiles.Value)
                    {
                        e.Graphics.DrawImage(loadedBmps[vals[ix, iy]], rect);
                    }
                    if (rect.Contains(mouse))
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(60, 0, 0, 0)), rect);
                    }
                    iy++;
                }
                ix++;
            }
            ix = 0;
        }

        private void tiles_ValueChanged(object sender, EventArgs e)
        {
            vals = new int[(int)tiles.Value, (int)tiles.Value];
            for (int x = 0; x < (int)tiles.Value; x++)
            {
                for (int y = 0; y < (int)tiles.Value; y++)
                {
                    vals[x, y] = 0;
                }
            }
            canvas.Invalidate();
        }

        private void TileView_ResizeBegin(object sender, EventArgs e)
        {
            notificationText.Visible = true;
            TileView_SizeChanged(null, null);
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            mouse = new Point(e.X, e.Y);
            Point current = canvasPointToWorkspacePoint(new Point(e.X, e.Y));
            if (current.X < 0 || current.X >= tiles.Value || current.Y < 0 || current.Y >= tiles.Value)
            {
                return;
            }
            if (current != last && mouseDown)
            {
                if (mouseRight)
                {
                    vals[current.X, current.Y] = 0;
                }
                else
                {
                    int currentVal = vals[current.X, current.Y];
                    currentVal++;
                    if (currentVal >= loadedBmps.Count)
                    {
                        Console.WriteLine("EEEE");
                        OpenFileDialog ofd = new OpenFileDialog();
                        ofd.Filter = "Images (*.png, *.tga)|*.png;*.tga";
                        ofd.ShowDialog();
                        try
                        {
                            loadedBmps.Add(TargaCrap.OpenImageAutoDetect(ofd.FileName));
                        }
                        catch
                        {
                            currentVal--;
                        }
                    }
                    vals[current.X, current.Y] = currentVal;
                }
                last = current;
            }
            canvas.Invalidate();
        }

        private void canvas_MouseLeave(object sender, EventArgs e)
        {
            mouse = new Point(-1, -1);
            mouseDown = false;
            canvas.Invalidate();
        }

        private void canvas_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            if (e.Button == MouseButtons.Right)
            {
                mouseRight = true;
            } else
            {
                mouseRight = false;
            }
            canvas_MouseMove(null, e);
            //canvas.Invalidate();
        }
        private Point canvasPointToWorkspacePoint(Point pt)
        {
            Point rpt;
            float x = pt.X / (canvas.Width / (float)tiles.Value);
            float y = pt.Y / (canvas.Height / (float)tiles.Value);
            rpt = new Point((int)Math.Floor(x), (int)Math.Floor(y));
            return rpt;
        }

        private void TileView_FormClosed(object sender, FormClosedEventArgs e)
        {
            for (int i = 1; i < loadedBmps.Count; i++)
            {
                loadedBmps[i].Dispose();
            }
        }

        private void canvas_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            last = new Point(-1, -1);
        }
    }
}
