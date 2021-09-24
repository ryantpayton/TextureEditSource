using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TextureEdit.Properties;

namespace TextureEdit
{
    public partial class Form1 : Form
    {
        Bitmap texture = new Bitmap(16, 16, PixelFormat.Format32bppArgb);
        Bitmap mer = new Bitmap(16, 16, PixelFormat.Format32bppArgb);
        Bitmap normal = new Bitmap(16, 16, PixelFormat.Format32bppArgb);
        Bitmap workspaceImage = new Bitmap(16, 16, PixelFormat.Format32bppArgb);
        Bitmap sync = new Bitmap(512, 512, PixelFormat.Format32bppArgb);

        Bitmap shadowBmp;
        Graphics shadowG;

        TileView currentTileView = null;
        Graphics syncGraphics;
        //Graphics pictureBoxGraphics;
        Graphics workspaceGraphics;
        Rectangle zoomRegion = new Rectangle(0, 0, 16, 16);
        WorkspaceManager workspaceManager = new WorkspaceManager();
        System.Timers.Timer flipBookAnimationTimer;
        

        Bitmap flipBookReference_texture = null;
        Bitmap flipBookReference_mer = null;
        Bitmap flipBookReference_normal = null;
        Bitmap transparencyBg = TextureEdit.Properties.Resources.transparencybg;
        bool flipbookMode = false;
        int flipbookIndex = 0;
        int flipbookLength = 0;
        int resolution = 16;
        bool waitForRefresh = false;
        string openedFile = "";

        Tool currentTool = Tool.Pen;
        Point selectStartPoint;
        bool chanelMode = false;
        bool greyscaleTextureMode = false;
        Point lastPenPoint;

        const string versionId = "v1.20.32";
        const bool unreleasedMode = false;
        const string baseTitle = "TextureEdit (" + versionId + ")";

        bool mouseDown = false;
        bool mouseEntered = false;

        double workspaceWidth = 0;
        double workspaceHeight = 0;

        public Form1()
        {
            if (!unreleasedMode)
            {
                updateSequence();
            }
            flipBookAnimationTimer = new System.Timers.Timer();
            flipBookAnimationTimer.Elapsed += FlipBookAnimationTimer_Tick;
            flipBookAnimationTimer.Interval = 100;

            this.KeyPreview = true;
            InitializeComponent();
            this.Opacity = 0.9999;
            this.Text = baseTitle;
            syncGraphics = Graphics.FromImage(sync);
            centerWorkspace();
            toolTip1.BackColor = Color.FromArgb(64, 64, 64);
            toolTip1.ForeColor = Color.White;
            toolTip1.SetToolTip(toolPenButton, "P");
            toolTip1.SetToolTip(toolSelectButton, "O (Hold shift for smart select)");
            toolTip1.SetToolTip(toolPickerButton, "I");
            toolTip1.SetToolTip(chanelTextureButton, "Ctrl+T");
            toolTip1.SetToolTip(chanelMetallicButton, "Ctrl+M");
            toolTip1.SetToolTip(chanelEmissiveButton, "Ctrl+E");
            toolTip1.SetToolTip(chanelRoughButton, "Ctrl+R");
            toolTip1.SetToolTip(chanelHeightButton, "Ctrl+H");
            toolTip1.SetToolTip(toolZoomButton, "Z (R to reset)");
            Console.WriteLine(workSpace.Size.Width);
            //pictureBoxImage = new Bitmap(workSpace.Size.Width, workSpace.Size.Height, PixelFormat.Format32bppArgb);
            //pictureBoxGraphics = Graphics.FromImage(pictureBoxImage);
            //pictureBoxGraphics = workSpace.CreateGraphics();
            //pictureBoxGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            //pictureBoxGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            //pictureBoxGraphics.CompositingMode = CompositingMode.SourceCopy;

            workSpaceShadowClip.Paint += dropShadow<PictureBox>;
            controlsShadowClip.Paint += dropShadow<Panel>;
            colorPreviewShadowClip.Paint += subtleShadow<PictureBox>;

            initializeTextureAsset(texture, TextureAssetType.texture);
            initializeTextureAsset(mer, TextureAssetType.mer);
            initializeTextureAsset(normal, TextureAssetType.normal);

            //workSpace.Image = pictureBoxImage;
            workspaceGraphics = Graphics.FromImage(workspaceImage);
            //updateWorkspace();

            workspaceManager.Source = texture;
            workspaceManager.Destination = workspaceImage;
            workspaceManager.EditingDisplayMode = WorkspaceDisplayMode.Texture;
            workspaceManager.OnWorkspaceChange = updateWorkspace;
            workspaceManager.InitializeWorkspace();

            workSpace.MouseWheel += WorkSpace_MouseWheel;
            workSpace.Paint += updateWorkspaceP;

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && args[1] != "updated")
            {
                openTexture(Path.ChangeExtension(args[1], null), false);
                openedFile = args[1];
            }

            //pictureBoxGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            //pictureBoxGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            //pictureBoxGraphics.CompositingMode = CompositingMode.SourceCopy;
            //workspaceManager.Hover = new Point(1, 4);
            try
            {
                defaultEditor();
            } catch { }
        }

        private void defaultEditor()
        {
            OpenWith openWith = new OpenWith("temp.png");
            bool progIDSet = false;
            foreach (OpenWithApplication app in openWith.GetApplications())
            {
                if (app.ProgID == Settings.Default.DefaultEditorProgID)
                {
                    progIDSet = true;
                    exportButton.Text = "Edit in " + app.Name.Replace("&", "&&");
                }
            }
            if (!progIDSet)
            {
                OpenWithApplication defaultApp = openWith.GetDefault();
                Settings.Default.DefaultEditorProgID = defaultApp.ProgID;
                Settings.Default.Save();
                exportButton.Text = "Edit in " + defaultApp.Name.Replace("&", "&&");
            }
            toolTip1.SetToolTip(exportButton, exportButton.Text.Replace("&&", "&"));
        }

        private void updateSequence()
        {
            if (Environment.GetCommandLineArgs().Length > 1 && Environment.GetCommandLineArgs()[1] == "updated")
            {
                string file = Process.GetCurrentProcess().MainModule.FileName;
                if (File.Exists(file + "_"))
                {
                    File.Delete(file + "_");
                }
                UpdateComplete dialog = new UpdateComplete();
                dialog.ShowDialog();
                return;
            }
            if (!File.Exists("TgaLib.dll"))
            {
                byte[] tgaLib = GetB("https://github.com/ReinaStreufert/TextureEdit/raw/master/TgaLib.dll");
                File.WriteAllBytes("TgaLib.dll", tgaLib);
            }
            string upVersionString = Get("https://raw.githubusercontent.com/ReinaStreufert/TextureEdit/master/versionid.txt");
            Console.WriteLine(upVersionString);
            string upVersionId = upVersionString.Split('\n')[0];
            string upVersionDescription = upVersionString.Split('\n')[1];
            if (upVersionId != versionId)
            {
                Console.WriteLine("need update: " + upVersionId);
                Update up = new TextureEdit.Update(upVersionDescription);
                up.ShowDialog();
            }
        }

        public static string Get(string uri)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            request.Headers.Add("Cache-Control: no-cache");
            //request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        public static byte[] GetB(string uri)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            request.Headers.Add("Cache-Control: no-cache");
            //request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private void WorkSpace_MouseWheel(object sender, MouseEventArgs e)
        {
            workspaceManager.BrushSize += e.Delta / SystemInformation.MouseWheelScrollDelta;
        }

        private void FlipBookAnimationTimer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                flipbookForwardButton_Click(null, null);
            }));
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
        private void dropShadow<T>(object sender, PaintEventArgs e) where T : Control
        {
            Panel panel = (Panel)sender;
            if (shadowBmp == null || shadowBmp.Size != this.Size)
            {
                shadowBmp?.Dispose();
                shadowBmp = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
                shadowG?.Dispose();
                shadowG = Graphics.FromImage(shadowBmp);
            }
            foreach (T p in panel.Controls.OfType<T>())
            {
                if (!p.Visible)
                {
                    continue;
                }
                if (p.Tag != null && p.Tag.GetType() == typeof(int))
                {
                    if ((int)p.Tag < 1)
                    {
                        continue;
                    } else
                    {
                        p.Tag = (int)p.Tag - 1;
                    }
                } else
                {
                    continue;
                }
                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddRectangle(new Rectangle(p.Location.X, p.Location.Y, p.Width, p.Height));
                    DrawShadowSmooth(gp, 100, 100, shadowG);
                    e.Graphics.DrawImage(shadowBmp, new Point(0, 0));
                }
                p.Tag = true;
            }
        }
        private static void DrawShadowSmooth(GraphicsPath gp, int intensity, int radius, Graphics g)
        {
            g.Clear(Color.Transparent);
            g.CompositingMode = CompositingMode.SourceCopy;
            double alpha = 0;
            double astep = 0;
            double astepstep = (double)intensity / radius / (radius / 2D);
            using (Pen p = new Pen(Color.FromArgb((int)alpha, 0, 0, 0), radius))
            {
                p.LineJoin = LineJoin.Round;
                for (int thickness = radius; thickness > 0; thickness--)
                {
                    p.Color = Color.FromArgb((int)alpha, 0, 0, 0);
                    p.Width = thickness;
                    g.DrawPath(p, gp);
                    alpha += astep;
                    astep += astepstep;
                }
            }
        }
        private void subtleShadow<T>(object sender, PaintEventArgs e) where T : Control
        {
            Panel panel = (Panel)sender;
            /*Color[] shadow = generateShadow(15, 5);
            SolidBrush brush = new SolidBrush(shadow[0]);
            using (brush)
            {
                foreach (T p in panel.Controls.OfType<T>())
                {
                    if (!p.Visible)
                    {
                        continue;
                    }
                    Rectangle pt = new Rectangle(p.Location, p.Size);
                    pt.X -= shadow.Length;
                    pt.Y -= shadow.Length;
                    pt.Width += shadow.Length * 2;
                    pt.Height += shadow.Length * 2;
                    for (var sp = 0; sp < shadow.Length; sp++)
                    {
                        brush.Color = shadow[sp];
                        e.Graphics.FillRectangle(brush, pt);
                        pt.X++;
                        pt.Y++;
                        pt.Width -= 2;
                        pt.Height -= 2;
                    }
                }
            }*/
            if (shadowBmp == null || shadowBmp.Size != this.Size)
            {
                shadowBmp?.Dispose();
                shadowBmp = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
                shadowG?.Dispose();
                shadowG = Graphics.FromImage(shadowBmp);
            }
            foreach (T p in panel.Controls.OfType<T>())
            {
                if (!p.Visible)
                {
                    continue;
                }
                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddRectangle(new Rectangle(p.Location.X, p.Location.Y, p.Width, p.Height));
                    DrawShadowSmooth(gp, 50, 20, shadowG);
                    e.Graphics.DrawImage(shadowBmp, new Point(0, 0));
                }
            }
        }
        public enum TextureAssetType : byte
        {
            texture = 0,
            mer = 1,
            normal = 2
        }
        private void initializeTextureAsset(Bitmap tex, TextureAssetType assetType)
        {
            using (Graphics g = Graphics.FromImage(tex))
            {
                if (assetType == TextureAssetType.texture)
                {
                    g.Clear(Color.White);
                } else if (assetType == TextureAssetType.mer)
                {
                    g.Clear(Color.FromArgb(255, 0, 0, 255));
                } else if (assetType == TextureAssetType.normal)
                {
                    g.Clear(Color.FromArgb(255, 128, 0, 0));
                }
            }
        }
        private Color[] generateShadow(int radius, int startingAlpha)
        {
            List<Color> colors = new List<Color>();
            int step = startingAlpha / radius;
            for (int i = 0; i < radius; i++)
            {
                colors.Add(Color.FromArgb(startingAlpha, 0, 0, 0));
                startingAlpha -= step;
            }
            return colors.ToArray();
        }
        private Point canvasPointToWorkspacePoint(Point pt)
        {
            Point rpt;
            if (currentTool != Tool.Zoom)
            {
                float x = pt.X / (workSpace.Width / (float)zoomRegion.Width);
                float y = pt.Y / (workSpace.Height / (float)zoomRegion.Height);
                rpt = new Point((int)Math.Floor(x), (int)Math.Floor(y));
                rpt.X += zoomRegion.X;
                rpt.Y += zoomRegion.Y;
            } else
            {
                rpt = new Point((int)(pt.X / ((float)workSpace.Width / (float)texture.Width)), (int)(pt.Y / ((float)workSpace.Height / (float)texture.Height)));
            }
            return rpt;
        }
        private void updateWorkspace()
        {
            workSpace.Invalidate();
            if (workspaceManager.EffectsMode)
            {
                workSpace.Refresh();
            }
        }
        private void updateWorkspaceP(object sender, PaintEventArgs e)
        {
            Rectangle srcRect;
            if (currentTool != Tool.Zoom)
            {
                srcRect = zoomRegion;
            } else
            {
                srcRect = new Rectangle(0, 0, texture.Width, texture.Height);
            }
            double aspect = (double)srcRect.Height / (double)srcRect.Width;
            //Console.WriteLine(aspect);
            double currentAspect = (double)workspaceHeight / (double)workspaceWidth;
            //Console.WriteLine(currentAspect);
            if (Math.Abs(aspect - currentAspect) > 0.01)
            {
                workSpace.Height = (int)(aspect * workSpace.Width);
                Form1_Resize(null, null);
                //workSpace.Refresh();
            }
            if (sync.Size != workSpace.Size)
            {
                syncGraphics.Dispose();
                sync.Dispose();
                sync = new Bitmap(workSpace.Width, workSpace.Height, PixelFormat.Format32bppArgb);
                syncGraphics = Graphics.FromImage(sync);
                syncGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                /*if (Math.Min(zoomRegion.Width, zoomRegion.Height) < Math.Max(workSpace.Size.Width, workSpace.Size.Height))
                {
                    syncGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                }*/
            }
            Rectangle overlayRegion;
            if (currentTool != Tool.Zoom)
            {
                overlayRegion = zoomRegion;
            }
            else
            {
                overlayRegion = new Rectangle(0, 0, texture.Width, texture.Height);
            }
            if (Math.Max(overlayRegion.Width, overlayRegion.Height) < Math.Min(workSpace.Size.Width, workSpace.Size.Height))
            {
                syncGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            } else
            {
                syncGraphics.InterpolationMode = InterpolationMode.Default;
            }
            //syncGraphics.Clear(Color.Transparent);
            if (solidBgCheckbox.Checked)
            {
                syncGraphics.Clear(Settings.Default.SolidBgColor);
            } else
            {
                syncGraphics.DrawImage(transparencyBg, new Rectangle(0, 0, Math.Max(workSpace.Width, workSpace.Height), Math.Max(workSpace.Width, workSpace.Height)));
            }
            syncGraphics.DrawImage(workspaceImage, new Rectangle(0, 0, workSpace.Width, workSpace.Height), srcRect, GraphicsUnit.Pixel);

            //pictureBoxGraphics.DrawImage(workspaceImage, 0, 0, workSpace.Size.Width, workSpace.Size.Height);
            if (overlayTextureCheckBox.Checked && workspaceManager.EditingDisplayMode != WorkspaceDisplayMode.Texture)
            {
                ImageAttributes imgAttr = new ImageAttributes();
                float[][] colorMatrixElements = {
                   new float[] {1,  0,  0,  0, 0},
                   new float[] {0,  1,  0,  0, 0},
                   new float[] {0,  0,  1,  0, 0},
                   new float[] {0,  0,  0,  Settings.Default.OverlayOpacity / 100F, 0},
                   new float[] {0,  0,  0,  0,    1F}};
                imgAttr.SetColorMatrix(new ColorMatrix(colorMatrixElements));
                syncGraphics.DrawImage(
                   texture,
                   new Rectangle(0, 0, workSpace.Size.Width, workSpace.Size.Height),  // destination rectangle
                   overlayRegion.X, overlayRegion.Y,        // upper-left corner of source rectangle
                   overlayRegion.Width,       // width of source rectangle
                   overlayRegion.Height,      // height of source rectangle
                   GraphicsUnit.Pixel,
                   imgAttr);
            }
            if (flipbookMode)
            {
                updateFlipBook();
            }
            if (gridCheckBox.Checked)
            {
                double gap;
                if (currentTool != Tool.Zoom)
                {
                    gap = (double)workSpace.Width / zoomRegion.Width;
                } else
                {
                    gap = (double)workSpace.Width / resolution;
                }
                drawGrid(syncGraphics, workSpace.Width, workSpace.Height, gap, Color.Black, Color.White);
            }

            if (currentTileView != null)
            {
                currentTileView.Retile();
            }

            Graphics pictureBoxGraphics = e.Graphics;//workSpace.CreateGraphics();
            pictureBoxGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            if (Math.Max(zoomRegion.Size.Width, zoomRegion.Size.Height) < Math.Min(workSpace.Size.Width, workSpace.Size.Height))
            {
                pictureBoxGraphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            }
            pictureBoxGraphics.CompositingMode = CompositingMode.SourceCopy;
            pictureBoxGraphics.DrawImageUnscaled(sync, new Point(0, 0));
            //pictureBoxGraphics.Dispose();

            waitForRefresh = false;
            //Stopwatch sw = new Stopwatch();
            //workSpace.Refresh();
        }

        private void centerWorkspace()
        {
            int x = (workSpaceShadowClip.Width / 2) - (workSpace.Width / 2);
            int y = (workSpaceShadowClip.Height / 2) - (workSpace.Height / 2);
            double quad = (workSpace.Width - 40) / 4D;
            double bi = workSpace.Width / 2D;
            res16x16.Location = new Point(x, y - 30);
            res16x16.Size = new Size((int)Math.Ceiling(quad), 27);
            res32x32.Location = new Point(x + (int)quad, y - 30);
            res32x32.Size = new Size((int)Math.Ceiling(quad), 27);
            res64x64.Location = new Point(x + (int)(quad * 2), y - 30);
            res64x64.Size = new Size((int)Math.Ceiling(quad), 27);
            res128x128.Location = new Point(x + (int)(quad * 3), y - 30);
            res128x128.Size = new Size((int)Math.Ceiling(quad), 27);
            moreResolutions.Location = new Point(x + (workSpace.Width) - 40, y - 30);

            workSpace.Location = new Point(x, y);

            flipbookBackButton.Location = new Point(x, y + workSpace.Height + 3);
            flipbookBackButton.Size = new Size((int)Math.Ceiling(bi), 27);
            flipbookForwardButton.Location = new Point(x + (int)bi, y + workSpace.Height + 3);
            flipbookForwardButton.Size = new Size((int)Math.Ceiling(bi), 27);

            int controlsX = x + ((workSpace.Width / 2) - ((flipBookPlayPause.Width + pageNumberLabel.Width) / 2));
            flipBookPlayPause.Location = new Point(controlsX, y + workSpace.Height + 3 + 39);
            pageNumberLabel.Location = new Point(controlsX + 37, y + workSpace.Height + 3 + 39);
            workSpaceShadowClip.Refresh();
        }

        private void workSpace_MouseMove(object sender, MouseEventArgs e)
        {
            if (waitForRefresh)
            {
                return;
            }
            Point current = canvasPointToWorkspacePoint(new Point(e.X, e.Y));
            if (current.X > resolution - 1 || current.Y > resolution - 1 || current.X < 0 || current.Y < 0)
            {
                return;
            }
            workspaceManager.SuspendDrawing = true;
            workspaceManager.Hover = current;
            if (mouseDown)
            {
                if (currentTool == Tool.Pen)
                {
                    //workspaceManager.FillSelection(Color.FromArgb(aSlider.Value, rSlider.Value, gSlider.Value, bSlider.Value));
                    workspaceManager.DrawLine(Color.FromArgb(aSlider.Value, rSlider.Value, gSlider.Value, bSlider.Value), lastPenPoint, current);
                    lastPenPoint = current;

                } else if (currentTool == Tool.Select || currentTool == Tool.Zoom)
                {
                    if (Control.ModifierKeys != Keys.Shift || currentTool == Tool.Zoom)
                    {
                        workspaceManager.SmartSelection.Clear();
                        //workspaceManager.Redraw();
                        int top = Math.Min(current.Y, selectStartPoint.Y);
                        int bottom = Math.Max(current.Y, selectStartPoint.Y);
                        int right = Math.Max(current.X, selectStartPoint.X);
                        int left = Math.Min(current.X, selectStartPoint.X);
                        if (left != current.X)
                        {
                            right++;
                        }
                        if (top != current.Y)
                        {
                            bottom++;
                        }
                        if (left == current.X)
                        {
                            right++;
                        }
                        if (top == current.Y)
                        {
                            bottom++;
                        }
                        if (left < 0)
                        {
                            left = 0;
                        }
                        if (right > texture.Width)
                        {
                            right = texture.Width;
                        }
                        if (top < 0)
                        {
                            top = 0;
                        }
                        if (bottom > texture.Height)
                        {
                            bottom = texture.Height;
                        }
                        Rectangle rect = Rectangle.FromLTRB(left, top, right, bottom);
                        if (rect.Height == 0)
                        {
                            rect.Height = 1;
                        }
                        if (rect.Width == 0)
                        {
                            rect.Width = 1;
                        }
                        if (rect.X < 0)
                        {
                            rect.X = 0;
                        }
                        if (rect.Y < 0)
                        {
                            rect.Y = 0;
                        }
                        if (currentTool != Tool.Zoom)
                        {
                            if (rect.Width > zoomRegion.Width)
                            {
                                rect.Width = zoomRegion.Width;
                            }
                            if (rect.Height > zoomRegion.Height)
                            {
                                rect.Height = zoomRegion.Height;
                            }
                            workspaceManager.Selection = rect;
                        } else
                        {
                            int size = Math.Max(rect.Width, rect.Height);
                            //rect.Width = size;
                            //rect.Height = size;
                            workspaceManager.Selection = rect;
                            zoomRegion = rect;
                        }
                    } else
                    {
                        workspaceManager.Selection = new Rectangle(-1, -1, -1, -1);
                        if (!workspaceManager.SmartSelection.Exists((Point p) => { return p.X == current.X && p.Y == current.Y; }))
                        {
                            workspaceManager.SmartSelection.Add(current);
                            workspaceManager.Redraw();
                        }
                    }
                    addNoiseButton.Enabled = true;
                    greyscaleAdjustButton.Enabled = true;
                }
            }
            if (currentTool == Tool.Picker)
            {
                colorPreview.BackColor = workspaceManager.SampleColor(current.X, current.Y);
            }
            workspaceManager.SuspendDrawing = false;
            waitForRefresh = true;
            workspaceManager.Redraw();
        }

        private void workSpace_MouseLeave(object sender, EventArgs e)
        {
            mouseEntered = false;
            workspaceManager.Hover = new Point(-1, -1);
            mouseDown = false;
            updateCursor();
            if (currentTool == Tool.Picker)
            {
                colorPreview.BackColor = Color.FromArgb(aSlider.Value, rSlider.Value, gSlider.Value, bSlider.Value);
            }
        }

        private void workSpace_MouseDown(object sender, MouseEventArgs e)
        {
            //Console.WriteLine(mouseDown);
            mouseDown = true;
            Point current = canvasPointToWorkspacePoint(new Point(e.X, e.Y));
            if (currentTool == Tool.Pen)
            {
                workspaceManager.SaveToUndo();
                workspaceManager.FillSelection(Color.FromArgb(aSlider.Value, rSlider.Value, gSlider.Value, bSlider.Value));
                lastPenPoint = current;
            } else if (currentTool == Tool.Select || currentTool == Tool.Zoom)
            {
                addNoiseButton.Enabled = false;
                greyscaleAdjustButton.Enabled = false;
                workspaceManager.Selection = new Rectangle(-1, -1, -1, -1);
                if (Control.ModifierKeys != Keys.Shift)
                {
                    workspaceManager.SmartSelection.Clear();
                }
                workspaceManager.Redraw();
                selectStartPoint = new Point(current.X, current.Y);
            } else if (currentTool == Tool.Picker)
            {
                Color clr = workspaceManager.SampleColor(current.X, current.Y);
                rSlider.Value = clr.R;
                gSlider.Value = clr.G;
                bSlider.Value = clr.B;
                aSlider.Value = clr.A;
            } else if (currentTool == Tool.Zoom)
            {
                int x = current.X;
                int y = current.Y;
                zoomRegion = new Rectangle(x - workspaceManager.BrushSize / 2, y - workspaceManager.BrushSize / 2, workspaceManager.BrushSize, workspaceManager.BrushSize);
                if (zoomRegion.X < 0)
                {
                    zoomRegion.X = 0;
                }
                if (zoomRegion.Y < 0)
                {
                    zoomRegion.Y = 0;
                }
                if (zoomRegion.Width > texture.Width)
                {
                    zoomRegion.Width = texture.Width;
                }
                if (zoomRegion.Height > texture.Height)
                {
                    zoomRegion.Height = texture.Height;
                }
                workspaceManager.Selection = zoomRegion;
                workspaceManager.Redraw();
            } else if (currentTool == Tool.Wand)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    workspaceManager.ShiftMagicWand(current);
                } else
                {
                    workspaceManager.MagicWand(current);
                }
                addNoiseButton.Enabled = true;
                greyscaleAdjustButton.Enabled = true;
                toolSelectButton_Click(null, null);
            }
        }

        private void workSpace_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void workSpace_MouseEnter(object sender, EventArgs e)
        {
            mouseEntered = true;
            updateCursor();
        }
        private void updateCursor()
        {
            if (mouseEntered)
            {
                if (currentTool == Tool.Pen)
                {
                    this.Cursor = Cursors.Cross;
                }
                else if (currentTool == Tool.Select)
                {
                    this.Cursor = Cursors.Hand;
                } else if (currentTool == Tool.Picker || currentTool == Tool.Zoom)
                {
                    this.Cursor = Cursors.Default;
                }
            } else
            {
                this.Cursor = Cursors.Default;
            }
        }
        private void setButtonState(Button b, bool on)
        {
            if (on)
            {
                b.BackColor = Color.DodgerBlue;
                b.ForeColor = Color.White;
            }
            else
            {
                b.BackColor = Color.White;
                b.ForeColor = Color.Black;
            }
            b.Refresh();
        }

        private void chanelTextureButton_Click(object sender, EventArgs e)
        {
            if (!greyscaleTextureMode)
            {
                greyscaleAdjustButton.Text = "Color Shift";
            } else
            {
                greyscaleAdjustButton.Text = "Greyscale Adjust";
            }
            setButtonState(chanelTextureButton, true);
            setButtonState(chanelMetallicButton, false);
            setButtonState(chanelEmissiveButton, false);
            setButtonState(chanelRoughButton, false);
            setButtonState(chanelHeightButton, false);

            Tool prevTool = currentTool;
            if (currentTool == Tool.Select)
            {
                currentTool = Tool.Picker;
            }
            workspaceManager.Source = texture;
            workspaceManager.EditingDisplayMode = WorkspaceDisplayMode.Texture;
            workspaceManager.InitializeWorkspace();

            resetSliders();
            if (greyscaleTextureMode)
            {
                makeSlidersChanel("Color", "Black", "White", false);
            }
            currentTool = prevTool;
        }

        private void chanelMetallicButton_Click(object sender, EventArgs e)
        {
            greyscaleAdjustButton.Text = "Greyscale Adjust";
            setButtonState(chanelTextureButton, false);
            setButtonState(chanelMetallicButton, true);
            setButtonState(chanelEmissiveButton, false);
            setButtonState(chanelRoughButton, false);
            setButtonState(chanelHeightButton, false);

            Tool prevTool = currentTool;
            if (currentTool == Tool.Select)
            {
                currentTool = Tool.Picker;
            }
            workspaceManager.Source = mer;
            workspaceManager.EditingDisplayMode = WorkspaceDisplayMode.Metallic;
            workspaceManager.InitializeWorkspace();

            makeSlidersChanel("Metalness", "Matte", "Reflective");
            currentTool = prevTool;
            //rSlider.Value = 0;
        }

        private void chanelEmissiveButton_Click(object sender, EventArgs e)
        {
            greyscaleAdjustButton.Text = "Greyscale Adjust";
            setButtonState(chanelTextureButton, false);
            setButtonState(chanelMetallicButton, false);
            setButtonState(chanelEmissiveButton, true);
            setButtonState(chanelRoughButton, false);
            setButtonState(chanelHeightButton, false);

            Tool prevTool = currentTool;
            if (currentTool == Tool.Select)
            {
                currentTool = Tool.Picker;
            }
            workspaceManager.Source = mer;
            workspaceManager.EditingDisplayMode = WorkspaceDisplayMode.Emissive;
            workspaceManager.InitializeWorkspace();

            makeSlidersChanel("Emissiveness", "Dark", "Bright");
            currentTool = prevTool;
            //rSlider.Value = 0;
        }

        private void chanelRoughButton_Click(object sender, EventArgs e)
        {
            greyscaleAdjustButton.Text = "Greyscale Adjust";
            setButtonState(chanelTextureButton, false);
            setButtonState(chanelMetallicButton, false);
            setButtonState(chanelEmissiveButton, false);
            setButtonState(chanelRoughButton, true);
            setButtonState(chanelHeightButton, false);

            Tool prevTool = currentTool;
            if (currentTool == Tool.Select)
            {
                currentTool = Tool.Picker;
            }
            workspaceManager.Source = mer;
            workspaceManager.EditingDisplayMode = WorkspaceDisplayMode.Roughness;
            workspaceManager.InitializeWorkspace();

            makeSlidersChanel("Roughness", "Smooth/Shiny", "Rough");
            currentTool = prevTool;
            //rSlider.Value = 255;
        }

        private void chanelHeightButton_Click(object sender, EventArgs e)
        {
            greyscaleAdjustButton.Text = "Greyscale Adjust";
            setButtonState(chanelTextureButton, false);
            setButtonState(chanelMetallicButton, false);
            setButtonState(chanelEmissiveButton, false);
            setButtonState(chanelRoughButton, false);
            setButtonState(chanelHeightButton, true);

            Tool prevTool = currentTool;
            if (currentTool == Tool.Select)
            {
                currentTool = Tool.Picker;
            }
            workspaceManager.Source = normal;
            workspaceManager.EditingDisplayMode = WorkspaceDisplayMode.HeightMap;
            workspaceManager.InitializeWorkspace();

            makeSlidersChanel("Height", "Lowered", "Raised");
            currentTool = prevTool;
            //rSlider.Value = 128;
        }

        private void rSlider_ValueChanged(object sender, EventArgs e)
        {
            Color c = Color.FromArgb(aSlider.Value, rSlider.Value, gSlider.Value, bSlider.Value);
            if (currentTool == Tool.Select)
            {
                workspaceManager.FillSelection(c);
            }
            if (chanelMode)
            {
                c = Color.FromArgb(255, c.R, c.R, c.R);
                rBox.Text = Math.Round(((rSlider.Value / 255F)) * 100) + "%";
            } else
            {
                rBox.Text = rSlider.Value.ToString();
            }
            if (greyscaleTextureMode)
            {
                gSlider.Value = rSlider.Value;
                bSlider.Value = rSlider.Value;
                aSlider.Value = 255;
            }
            colorPreview.BackColor = c;
        }

        private void gSlider_ValueChanged(object sender, EventArgs e)
        {
            gBox.Text = gSlider.Value.ToString();
            Color c = Color.FromArgb(aSlider.Value, rSlider.Value, gSlider.Value, bSlider.Value);
            if (currentTool == Tool.Select)
            {
                workspaceManager.FillSelection(c);
            }
            colorPreview.BackColor = c;
        }

        private void bSlider_ValueChanged(object sender, EventArgs e)
        {
            bBox.Text = bSlider.Value.ToString();
            Color c = Color.FromArgb(aSlider.Value, rSlider.Value, gSlider.Value, bSlider.Value);
            if (currentTool == Tool.Select)
            {
                workspaceManager.FillSelection(c);
            }
            colorPreview.BackColor = c;
        }

        private void aSlider_ValueChanged(object sender, EventArgs e)
        {
            aBox.Text = aSlider.Value.ToString();
            Color c = Color.FromArgb(aSlider.Value, rSlider.Value, gSlider.Value, bSlider.Value);
            if (currentTool == Tool.Select)
            {
                workspaceManager.FillSelection(c);
            }
            colorPreview.BackColor = c;
        }

        private void rBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                backBox(rBox, rSlider);
                res16x16.Focus();
            }
        }

        private void gBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                backBox(gBox, gSlider);
                res16x16.Focus();
            }
        }

        private void bBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                backBox(bBox, bSlider);
                res16x16.Focus();
            }
        }

        private void aBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                backBox(aBox, aSlider);
                res16x16.Focus();
            }
        }
        private void backBox(TextBox box, TrackBar slider)
        {
            byte val;
            if (box.Text.Last() == '%' && chanelMode)
            {
                box.Text = box.Text.Substring(0, box.Text.Length - 1);
            }
            if (byte.TryParse(box.Text, out val) && (val <= 100 || !chanelMode))
            {
                if (chanelMode)
                {
                    slider.Value = (int)Math.Round((val / 100F) * 255);
                } else
                {
                    slider.Value = val;
                }
            }
            else
            {
                if (chanelMode)
                {
                    box.Text = Math.Round(((slider.Value / 255F)) * 100) + "%";
                } else
                {
                    box.Text = slider.Value.ToString();
                }
            }
        }

        private void rBox_Leave(object sender, EventArgs e)
        {
            backBox(rBox, rSlider);
        }

        private void gBox_Leave(object sender, EventArgs e)
        {
            backBox(gBox, gSlider);
        }

        private void bBox_Leave(object sender, EventArgs e)
        {
            backBox(bBox, bSlider);
        }

        private void aBox_Leave(object sender, EventArgs e)
        {
            backBox(aBox, aSlider);
        }

        private void toolPenButton_Click(object sender, EventArgs e)
        {
            setButtonState(toolPenButton, true);
            setButtonState(toolSelectButton, false);
            setButtonState(toolPickerButton, false);
            setButtonState(toolZoomButton, false);
            setButtonState(toolWandButton, false);
            currentTool = Tool.Pen;
            workspaceManager.BrushMode = true;
            workspaceManager.SmartSelection.Clear();
            workspaceManager.Selection = new Rectangle(-1, -1, -1, -1);
            workspaceManager.HighlightOpacity = 200;
            workspaceManager.Redraw();
            updateCursor();
        }

        private void toolSelectButton_Click(object sender, EventArgs e)
        {
            setButtonState(toolPenButton, false);
            setButtonState(toolSelectButton, true);
            setButtonState(toolPickerButton, false);
            setButtonState(toolZoomButton, false);
            setButtonState(toolWandButton, false);
            if (currentTool == Tool.Zoom)
            {
                workspaceManager.Selection = new Rectangle(-1, -1, -1, -1);
                workspaceManager.Redraw();
            }
            currentTool = Tool.Select;
            workspaceManager.HighlightOpacity = 200;
            workspaceManager.BrushMode = false;
            updateCursor();
        }

        private void toolZoomButton_Click(object sender, EventArgs e)
        {
            setButtonState(toolPenButton, false);
            setButtonState(toolSelectButton, false);
            setButtonState(toolPickerButton, false);
            setButtonState(toolZoomButton, true);
            setButtonState(toolWandButton, false);
            currentTool = Tool.Zoom;
            workspaceManager.SmartSelection.Clear();
            workspaceManager.Selection = zoomRegion;
            workspaceManager.HighlightOpacity = 80;
            workspaceManager.BrushMode = false;
            updateCursor();
        }

        private void colorPickerButton_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();
            rSlider.Value = cd.Color.R;
            gSlider.Value = cd.Color.G;
            bSlider.Value = cd.Color.B;
            aSlider.Value = cd.Color.A;
        }

        private void toolPickerButton_Click(object sender, EventArgs e)
        {
            setButtonState(toolPenButton, false);
            setButtonState(toolSelectButton, false);
            setButtonState(toolPickerButton, true);
            setButtonState(toolZoomButton, false);
            setButtonState(toolWandButton, false);
            if (currentTool == Tool.Zoom)
            {
                workspaceManager.Selection = new Rectangle(-1, -1, -1, -1);
                workspaceManager.Redraw();
            }
            workspaceManager.SmartSelection.Clear();
            currentTool = Tool.Picker;
            workspaceManager.HighlightOpacity = 200;
            workspaceManager.BrushMode = false;
            updateCursor();
        }

        private void toolWandButton_Click(object sender, EventArgs e)
        {
            setButtonState(toolPenButton, false);
            setButtonState(toolSelectButton, false);
            setButtonState(toolPickerButton, false);
            setButtonState(toolZoomButton, false);
            setButtonState(toolWandButton, true);
            currentTool = Tool.Wand;
            workspaceManager.HighlightOpacity = 200;
            workspaceManager.BrushMode = false;
            updateCursor();
        }

        private void makeSlidersChanel(string channelText, string lowerText, string upperText, bool newChanelMode = true)
        {
            dependantArea.Visible = false;
            chanelSliderLabels.Visible = true;
            rText.Text = channelText;
            chanelLowBoundText.Text = lowerText;
            chanelUpBoundText.Text = upperText;
            chanelMode = newChanelMode;
        }
        private void resetSliders()
        {
            chanelMode = false;
            chanelSliderLabels.Visible = false;
            dependantArea.Visible = true;
            rText.Text = "Red";
            rSlider.Value = 0;
        }

        private void addNoiseButton_Click(object sender, EventArgs e)
        {
            AddNoiseDialog addNoiseDialog = new AddNoiseDialog();
            addNoiseDialog.workspace = workspaceManager;
            addNoiseDialog.ShowDialog();
        }

        private void saveTexture(string nameWithoutExtension, bool alt, bool shift)
        {
            if (File.Exists(nameWithoutExtension + ".png"))
            {
                File.Delete(nameWithoutExtension + ".png");
            }
            if (File.Exists(nameWithoutExtension + "_mer.png"))
            {
                File.Delete(nameWithoutExtension + "_mer.png");
            }
            if (File.Exists(nameWithoutExtension + "_normal.png"))
            {
                File.Delete(nameWithoutExtension + "_normal.png");
            }
            if (File.Exists(nameWithoutExtension + ".tga"))
            {
                File.Delete(nameWithoutExtension + ".tga");
            }
            if (File.Exists(nameWithoutExtension + "_mer.tga"))
            {
                File.Delete(nameWithoutExtension + "_mer.tga");
            }
            if (File.Exists(nameWithoutExtension + "_normal.tga"))
            {
                File.Delete(nameWithoutExtension + "_normal.tga");
            }
            if (File.Exists(nameWithoutExtension + ".texture_set.json"))
            {
                File.Delete(nameWithoutExtension + ".texture_set.json");
            }
            if (flipbookMode)
            {
                workspaceManager.EnforceHeightMap(flipBookReference_normal);
                if (workspaceManager.HasOpacity(flipBookReference_texture))
                {
                    TargaCrap.SaveTGA(flipBookReference_texture, nameWithoutExtension + ".tga");
                }
                else
                {
                    flipBookReference_texture.Save(nameWithoutExtension + ".png");
                }
                flipBookReference_mer.Save(nameWithoutExtension + "_mer.png");
                flipBookReference_normal.Save(nameWithoutExtension + "_normal.png");
            }
            else
            {
                workspaceManager.EnforceHeightMap(normal);
                //texture.Save(nameWithoutExtension + ".png");
                if (workspaceManager.HasOpacity(texture) && (!shift))
                {
                    TargaCrap.SaveTGA(texture, nameWithoutExtension + ".tga");
                }
                else
                {
                    texture.Save(nameWithoutExtension + ".png");
                }
                if (!alt)
                {
                    mer.Save(nameWithoutExtension + "_mer.png");
                    normal.Save(nameWithoutExtension + "_normal.png");
                }
            }
            TextureSetJson textureSetJson = new TextureSetJson(nameWithoutExtension);
            string path = $"{nameWithoutExtension}.texture_set.json";
            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, textureSetJson);
            }
        }
        private void openTexture(string nameWithoutExtension, bool noFlipBook)
        {
            this.Text = baseTitle + " - " + Path.GetFileName(nameWithoutExtension);
            flipbookMode = false;

            string texExtension = ".png";
            if (File.Exists(nameWithoutExtension + ".tga"))
            {
                texExtension = ".tga";
            }
            string merExtension = "_mer.png";
            if (File.Exists(nameWithoutExtension + "_mer.tga"))
            {
                merExtension = "_mer.tga";
            }
            string normalExtension = "_normal.png";
            if (File.Exists(nameWithoutExtension + "_normal.tga"))
            {
                normalExtension = "_normal.tga";
            }
            int iWidth = resolution;
            int iHeight = resolution;
            if (File.Exists(nameWithoutExtension + texExtension))
            {
                //texture.Dispose();
                using (Bitmap img = TargaCrap.OpenImageAutoDetect(nameWithoutExtension + texExtension))
                {
                    Graphics g;
                    resolution = img.Width;
                    if (img.Height > resolution && img.Height % resolution == 0 && !noFlipBook)
                    {
                        flipbookMode = true;
                        flipbookLength = img.Height / resolution;
                        try
                        {
                            flipBookReference_texture.Dispose();
                        }
                        catch { }
                        flipBookReference_texture = new Bitmap(img.Width, img.Height);
                        g = Graphics.FromImage(flipBookReference_texture);
                        iWidth = resolution;
                        iHeight = resolution;
                    }
                    else
                    {
                        if (img.Width != img.Height)
                        {
                            resolution = Math.Max(img.Width, img.Height);
                            texture = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
                            iWidth = img.Width;
                            iHeight = img.Height;
                        }
                        else
                        {
                            if (texture.Width != resolution)
                            {
                                texture = new Bitmap(resolution, resolution, PixelFormat.Format32bppArgb);
                            }
                            iWidth = resolution;
                            iHeight = resolution;
                        }
                        g = Graphics.FromImage(texture);
                    }
                    if (workspaceManager.IsGreyscale(img))
                    {
                        GreyscaleModeDialog gMode = new GreyscaleModeDialog();
                        gMode.ShowDialog();
                        if (gMode.result)
                        {
                            greyscaleTextureMode = true;
                            makeSlidersChanel("Color", "Black", "White", false);
                        }
                        else
                        {
                            greyscaleTextureMode = false;
                            if (!chanelMode)
                            {
                                resetSliders();
                            }
                        }
                    }
                    else
                    {
                        greyscaleTextureMode = false;
                        if (!chanelMode)
                        {
                            resetSliders();
                        }
                    }
                    Console.WriteLine(texture.Width);
                    g.Clear(Color.Transparent);
                    //g.DrawImageUnscaled(img, new Point(0, 0));
                    g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
                    g.Dispose();
                }
            }
            if (File.Exists(nameWithoutExtension + merExtension))
            {
                using (Bitmap img = TargaCrap.OpenImageAutoDetect(nameWithoutExtension + merExtension))
                {
                    Graphics g;
                    resolution = Math.Max(resolution, img.Width);
                    if (img.Height > resolution)
                    {
                        flipbookMode = true;
                        flipbookLength = img.Height / resolution;
                        try
                        {
                            flipBookReference_mer.Dispose();
                        }
                        catch { }
                        flipBookReference_mer = new Bitmap(img.Width, img.Height);
                        g = Graphics.FromImage(flipBookReference_mer);
                    }
                    else
                    {
                        if (mer.Width != resolution)
                        {
                            mer = new Bitmap(resolution, resolution, PixelFormat.Format32bppArgb);
                        }
                        g = Graphics.FromImage(mer);
                    }
                    g.Clear(Color.Transparent);
                    g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
                    g.Dispose();
                }
            }
            if (File.Exists(nameWithoutExtension + normalExtension))
            {
                using (Bitmap img = TargaCrap.OpenImageAutoDetect(nameWithoutExtension + normalExtension))
                {
                    Graphics g;
                    resolution = Math.Max(resolution, img.Width);
                    if (img.Height > resolution)
                    {
                        flipbookMode = true;
                        flipbookLength = img.Height / resolution;
                        try
                        {
                            flipBookReference_normal.Dispose();
                        }
                        catch { }
                        flipBookReference_normal = new Bitmap(img.Width, img.Height);
                        g = Graphics.FromImage(flipBookReference_normal);
                    }
                    else
                    {
                        if (normal.Width != resolution)
                        {
                            normal = new Bitmap(resolution, resolution, PixelFormat.Format32bppArgb);
                        }
                        g = Graphics.FromImage(normal);
                    }
                    g.Clear(Color.Transparent);
                    g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
                    g.Dispose();
                }
            }
            if (texture.Width != iWidth || texture.Height != iHeight)
            {
                texture = new Bitmap(texture, new Size(iWidth, iHeight));
            }
            if (mer.Width != iWidth || mer.Height != iHeight)
            {
                mer = new Bitmap(mer, new Size(iWidth, iHeight));
            }
            if (normal.Width != iWidth || normal.Height != iHeight)
            {
                normal = new Bitmap(normal, new Size(iWidth, iHeight));
            }
            if (workspaceManager.EditingDisplayMode == WorkspaceDisplayMode.Texture)
            {
                workspaceManager.Source = texture;
            }
            if (workspaceManager.EditingDisplayMode == WorkspaceDisplayMode.Metallic || workspaceManager.EditingDisplayMode == WorkspaceDisplayMode.Emissive || workspaceManager.EditingDisplayMode == WorkspaceDisplayMode.Roughness)
            {
                workspaceManager.Source = mer;
            }
            if (workspaceManager.EditingDisplayMode == WorkspaceDisplayMode.HeightMap)
            {
                workspaceManager.Source = normal;
            }
            workspaceImage = new Bitmap(resolution, resolution, PixelFormat.Format32bppArgb);
            workspaceManager.Destination = workspaceImage;
            if (flipbookMode)
            {
                if (flipBookReference_mer == null)
                {
                    flipBookReference_mer = new Bitmap(flipBookReference_texture.Width, flipBookReference_texture.Height);
                    initializeTextureAsset(flipBookReference_mer, TextureAssetType.mer);
                }
                if (flipBookReference_normal == null)
                {
                    flipBookReference_normal = new Bitmap(flipBookReference_texture.Width, flipBookReference_texture.Height);
                    initializeTextureAsset(flipBookReference_normal, TextureAssetType.normal);
                }
                flipbookIndex = 0;
                setFlipBook();
            }
            else
            {
                try
                {
                    flipBookReference_texture.Dispose();
                }
                catch { }
                try
                {
                    flipBookReference_mer.Dispose();
                }
                catch { }
                try
                {
                    flipBookReference_normal.Dispose();
                }
                catch { }
                flipBookReference_texture = null;
                flipBookReference_mer = null;
                flipBookReference_normal = null;

                disableFlipBook();
            }
            zoomRegion = new Rectangle(0, 0, iWidth, iHeight);
            workspaceManager.InitializeWorkspace();
            workspaceManager.ClearUndo();
            if (greyscaleTextureMode)
            {
                greyscaleAdjustButton.Text = "Greyscale Adjust";
            }
            else if (workspaceManager.EditingDisplayMode == WorkspaceDisplayMode.Texture)
            {
                greyscaleAdjustButton.Text = "Color Shift";
            }
            refreshResButtons();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Modifiers.HasFlag(Keys.Control))
            {
                //Console.WriteLine("Ctrl+S!");
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Images (*.png, *.tga)|*.png;*.tga";
                if (openedFile != "")
                {
                    dialog.FileName = Path.GetFileName(openedFile);
                }
                if (dialog.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                string nameWithoutExtension = Path.ChangeExtension(dialog.FileName, null);
                saveTexture(nameWithoutExtension, e.Modifiers.HasFlag(Keys.Alt), e.Modifiers.HasFlag(Keys.Shift));
                openedFile = dialog.FileName;
            } else if (e.KeyCode == Keys.O && e.Modifiers.HasFlag(Keys.Control))
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Images (*.png, *.tga)|*.png;*.tga";
                if (dialog.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                string nameWithoutExtension = Path.ChangeExtension(dialog.FileName, null);
                openTexture(nameWithoutExtension, e.Modifiers.HasFlag(Keys.Shift));
                openedFile = dialog.FileName;

            } else if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {
                workspaceManager.CopySelection();
            } else if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
            {
                workspaceManager.PasteToSelection();
            } else if (e.KeyCode == Keys.P)
            {
                toolPenButton_Click(null, null);
            } else if (e.KeyCode == Keys.O)
            {
                toolSelectButton_Click(null, null);
            } else if (e.KeyCode == Keys.I)
            {
                toolPickerButton_Click(null, null);
            } else if (e.KeyCode == Keys.T && e.Modifiers == Keys.Control)
            {
                chanelTextureButton_Click(null, null);
            } else if (e.KeyCode == Keys.M && e.Modifiers == Keys.Control)
            {
                chanelMetallicButton_Click(null, null);
            } else if (e.KeyCode == Keys.E && e.Modifiers == Keys.Control)
            {
                chanelEmissiveButton_Click(null, null);
            } else if (e.KeyCode == Keys.R && e.Modifiers == Keys.Control)
            {
                chanelRoughButton_Click(null, null);
            } else if (e.KeyCode == Keys.H && e.Modifiers == Keys.Control)
            {
                chanelHeightButton_Click(null, null);
            } else if (e.KeyCode == Keys.U && e.Modifiers == Keys.Control)
            {
                VersionInformationDialog vid = new VersionInformationDialog();
                vid.ShowDialog();
            } else if (e.KeyCode == Keys.Z && e.Modifiers == Keys.Control)
            {
                workspaceManager.RestoreFromUndo();
            } else if (e.KeyCode == Keys.Z)
            {
                toolZoomButton_Click(null, null);
            } else if (e.KeyCode == Keys.R)
            {
                zoomRegion = new Rectangle(0, 0, texture.Width, texture.Height);
                if (currentTool == Tool.Zoom)
                {
                    workspaceManager.Selection = zoomRegion;
                }
                workspaceManager.Redraw();
            } else if (e.KeyCode == Keys.Y && e.Modifiers == Keys.Control)
            {
                workspaceManager.Redo();
            } else if (e.KeyCode == Keys.W)
            {
                toolWandButton_Click(null, null);
            } else if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
            {
                toolSelectButton_Click(null, null);
                workspaceManager.SmartSelection.Clear();
                workspaceManager.Selection = new Rectangle(0, 0, texture.Width, texture.Height);
                addNoiseButton.Enabled = true;
                greyscaleAdjustButton.Enabled = true;
            }
        }
        private Rectangle getPageRegion(int index)
        {
            return new Rectangle(0, flipbookIndex * resolution, resolution, resolution);
        }
        private void setFlipBook(bool initWorkspace = true)
        {
            Rectangle pageRegion = getPageRegion(flipbookIndex);
            using (Graphics g = Graphics.FromImage(texture))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(flipBookReference_texture, new Rectangle(0, 0, resolution, resolution), pageRegion, GraphicsUnit.Pixel);
            }
            using (Graphics g = Graphics.FromImage(mer))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(flipBookReference_mer, new Rectangle(0, 0, resolution, resolution), pageRegion, GraphicsUnit.Pixel);
            }
            using (Graphics g = Graphics.FromImage(normal))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(flipBookReference_normal, new Rectangle(0, 0, resolution, resolution), pageRegion, GraphicsUnit.Pixel);
            }
            pageNumberLabel.Text = (flipbookIndex + 1) + "/" + flipbookLength;
            pageNumberLabel.Refresh();
            flipbookBackButton.Visible = true;
            flipbookForwardButton.Visible = true;
            pageNumberLabel.Visible = true;
            flipBookPlayPause.Visible = true;
            workspaceManager.ClearUndo();
            Region r = new Region();
            if (initWorkspace)
            {
                workspaceManager.InitializeWorkspace();
            }
        }
        private void updateFlipBook()
        {
            Rectangle pageRegion = getPageRegion(flipbookIndex);
            using (Graphics g = Graphics.FromImage(flipBookReference_texture))
            {
                g.Clip = new Region(pageRegion);
                g.Clear(Color.Transparent);
                g.ResetClip();
                g.DrawImageUnscaled(texture, pageRegion);
            }
            using (Graphics g = Graphics.FromImage(flipBookReference_mer))
            {
                g.Clip = new Region(pageRegion);
                g.Clear(Color.Transparent);
                g.ResetClip();
                g.DrawImageUnscaled(mer, pageRegion);
            }
            using (Graphics g = Graphics.FromImage(flipBookReference_normal))
            {
                g.Clip = new Region(pageRegion);
                g.Clear(Color.Transparent);
                g.ResetClip();
                g.DrawImageUnscaled(normal, pageRegion);
            }
        }
        private void disableFlipBook()
        {
            flipbookMode = false;
            flipbookBackButton.Visible = false;
            flipbookForwardButton.Visible = false;
            flipBookPlayPause.Visible = false;
            pageNumberLabel.Visible = false;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            workspaceManager.Redraw();
        }

        private void flipbookForwardButton_Click(object sender, EventArgs e)
        {
            flipbookIndex++;
            if (flipbookIndex >= flipbookLength)
            {
                flipbookIndex = 0;
            }
            setFlipBook();
        }

        private void flipbookBackButton_Click(object sender, EventArgs e)
        {
            flipbookIndex--;
            if (flipbookIndex < 0)
            {
                flipbookIndex = flipbookLength - 1;
            }
            setFlipBook();
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            workspaceManager.Redraw();
        }
        private void drawGrid(Graphics g, int width, int height, double step, Color color, Color secondaryColor)
        {
            using (Pen c1 = new Pen(color, 1))
            {
                using (Pen c2 = new Pen(secondaryColor, 1))
                {
                    if (Settings.Default.GridIntensity != 5)
                    {
                        c1.DashPattern = new float[] { 1, 5 - Settings.Default.GridIntensity };
                        c2.DashPattern = new float[] { 1, 5 - Settings.Default.GridIntensity };
                    }
                    c2.DashOffset = 1;
                    for (double x = 0.5D; x < width - 1; x += step)
                    {
                        g.DrawLine(c1, new Point((int)x, 0), new Point((int)x, height));
                        g.DrawLine(c2, new Point((int)x + 1, 1), new Point((int)x + 1, height + 1));
                    }
                    for (double y = 0.5D; y < height - 1; y += step)
                    {
                        g.DrawLine(c1, new Point(0, (int)y), new Point(width, (int)y));
                        g.DrawLine(c2, new Point(1, (int)y), new Point(width, (int)y + 1));
                    }
                }
            }
        }

        private void greyscaleAdjustButton_Click(object sender, EventArgs e)
        {
            if (workspaceManager.EditingDisplayMode == WorkspaceDisplayMode.Texture && !greyscaleTextureMode)
            {
                ColorShiftDialog dialog = new ColorShiftDialog();
                if (flipbookMode)
                {
                    dialog.flipbookReference = flipBookReference_texture;
                }
                dialog.workspaceManager = workspaceManager;
                dialog.ShowDialog();
            } else
            {
                GreyscaleAdjustDialog dialog = new GreyscaleAdjustDialog();
                if (flipbookMode)
                {
                    if (workspaceManager.EditingDisplayMode == WorkspaceDisplayMode.Metallic || workspaceManager.EditingDisplayMode == WorkspaceDisplayMode.Emissive || workspaceManager.EditingDisplayMode == WorkspaceDisplayMode.HeightMap)
                    {
                        dialog.flipbookReference = flipBookReference_mer;
                    } else
                    {
                        dialog.flipbookReference = flipBookReference_normal;
                    }
                }
                dialog.workspaceManager = workspaceManager;
                dialog.ShowDialog();
            }
        }

        private void flipBookPlayPause_Click(object sender, EventArgs e)
        {
            if (flipBookAnimationTimer.Enabled)
            {
                flipBookAnimationTimer.Stop();
                flipBookPlayPause.Text = "\uE768";
            } else
            {
                flipBookAnimationTimer.Start();
                flipBookPlayPause.Text = "\uE769";
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void res16x16_Click(object sender, EventArgs e)
        {
            rescale(16);
            refreshResButtons();
        }

        private void res32x32_Click(object sender, EventArgs e)
        {
            rescale(32);
            refreshResButtons();
        }

        private void res64x64_Click(object sender, EventArgs e)
        {
            rescale(64);
            refreshResButtons();
        }

        private void res128x128_Click(object sender, EventArgs e)
        {
            rescale(128);
            refreshResButtons();
        }

        private void refreshResButtons()
        {
            res16x16.BackColor = Color.DimGray;
            res32x32.BackColor = Color.DimGray;
            res64x64.BackColor = Color.DimGray;
            res128x128.BackColor = Color.DimGray;
            moreResolutions.BackColor = Color.DimGray;
            if (resolution == 16)
            {
                res16x16.BackColor = Color.DodgerBlue;
            } else if (resolution == 32)
            {
                res32x32.BackColor = Color.DodgerBlue;
            } else if (resolution == 64)
            {
                res64x64.BackColor = Color.DodgerBlue;
            } else if (resolution == 128)
            {
                res128x128.BackColor = Color.DodgerBlue;
            } else
            {
                moreResolutions.BackColor = Color.DodgerBlue;
            }
            if (texture.Width > 1024 || texture.Height > 1024)
            {
                toolWandButton.Enabled = false;
            } else
            {
                toolWandButton.Enabled = true;
            }
        }

        private void rescale(int newResolution)
        {
            if (resolution == newResolution)
            {
                return;
            }
            if (newResolution < resolution)
            {
                DownscaleWarning warningBox = new DownscaleWarning();
                warningBox.ShowDialog();
                if (!warningBox.Cont)
                {
                    return;
                }
            }
            Bitmap oldTexture = texture;
            texture = new Bitmap(newResolution, newResolution, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(texture))
            {
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(oldTexture, new Rectangle(0, 0, newResolution, newResolution));
            }
            oldTexture.Dispose();

            Bitmap oldmer = mer;
            mer = new Bitmap(newResolution, newResolution, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(mer))
            {
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(oldmer, new Rectangle(0, 0, newResolution, newResolution));
            }
            oldmer.Dispose();

            Bitmap oldnormal = normal;
            normal = new Bitmap(newResolution, newResolution, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(normal))
            {
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(oldnormal, new Rectangle(0, 0, newResolution, newResolution));
            }
            oldnormal.Dispose();

            if (flipbookMode)
            {
                Bitmap oldflipBookReference_texture = flipBookReference_texture;
                flipBookReference_texture = new Bitmap(newResolution, newResolution * flipbookLength, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(flipBookReference_texture))
                {
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(oldflipBookReference_texture, new Rectangle(0, 0, newResolution, newResolution * flipbookLength));
                }
                oldflipBookReference_texture.Dispose();

                Bitmap oldflipBookReference_mer = flipBookReference_mer;
                flipBookReference_mer = new Bitmap(newResolution, newResolution * flipbookLength, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(flipBookReference_mer))
                {
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(oldflipBookReference_mer, new Rectangle(0, 0, newResolution, newResolution * flipbookLength));
                }
                oldflipBookReference_mer.Dispose();

                Bitmap oldflipBookReference_normal = flipBookReference_normal;
                flipBookReference_normal = new Bitmap(newResolution, newResolution * flipbookLength, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(flipBookReference_normal))
                {
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(oldflipBookReference_normal, new Rectangle(0, 0, newResolution, newResolution * flipbookLength));
                }
                oldflipBookReference_normal.Dispose();
                resolution = newResolution;
                setFlipBook(false);
            }
            Bitmap oldWorkspaceImage = workspaceImage;
            workspaceImage = new Bitmap(newResolution, newResolution, PixelFormat.Format32bppArgb);
            workspaceGraphics.Dispose();
            workspaceGraphics = Graphics.FromImage(workspaceImage);
            oldWorkspaceImage.Dispose();
            resolution = newResolution;
            if (workspaceManager.EditingDisplayMode == WorkspaceDisplayMode.Texture)
            {
                workspaceManager.Source = texture;
            } else if (workspaceManager.EditingDisplayMode == WorkspaceDisplayMode.Metallic || workspaceManager.EditingDisplayMode == WorkspaceDisplayMode.Emissive || workspaceManager.EditingDisplayMode == WorkspaceDisplayMode.Roughness)
            {
                workspaceManager.Source = mer;
            } else if (workspaceManager.EditingDisplayMode == WorkspaceDisplayMode.HeightMap)
            {
                workspaceManager.Source = normal;
            }
            workspaceManager.Destination = workspaceImage;
            workspaceManager.InitializeWorkspace();
            workspaceManager.ClearUndo();
            zoomRegion = new Rectangle(0, 0, resolution, resolution);
            workspaceManager.Redraw();
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            HelpDialog helpDialog = new HelpDialog();
            helpDialog.ShowDialog();
        }

        public static Process ShowOpenWithDialog(string path)
        {
            var args = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shell32.dll");
            args += ",OpenAs_RunDLL " + path;
            return Process.Start("rundll32.exe", args);
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            workspaceManager.SaveToUndo();
            string temp = Path.GetTempFileName() + ".png";
            File.Delete(temp.Substring(0, temp.Length - 4));
            workspaceManager.ExportWorkspace(temp);
            OpenWith openWith = new OpenWith(temp);
            IEnumerable<OpenWithApplication> apps = openWith.GetApplications().Where((OpenWithApplication owa) => { return (owa.ProgID == Settings.Default.DefaultEditorProgID); });
            Process p = null;
            if (apps.Count() > 0)
            {
                p = apps.First().Start();
            } else
            {
                p = openWith.GetDefault().Start();
            }
            
            if (currentTileView != null)
            {
                currentTileView.Visible = false;
            }
            this.Visible = false;
            p.WaitForExit();
            Thread.Sleep(100);
            workspaceManager.ImportWorkspace(temp);
            File.Delete(temp);
            this.Visible = true;
            workSpace.Tag = (int)1;
            controls.Tag = (int)1;
            workSpaceShadowClip.Invalidate();
            controlsShadowClip.Invalidate();
            if (currentTileView != null)
            {
                currentTileView.Visible = true;
            }
            this.BringToFront();
            this.Activate();
        }

        private void overlayTextureSettingsButton_Click(object sender, EventArgs e)
        {
            OverlaySettings oSet = new OverlaySettings();
            oSet.wMan = workspaceManager;
            oSet.originalOpacity = Settings.Default.OverlayOpacity;
            oSet.ShowDialog();
        }

        private void gridSettings_Click(object sender, EventArgs e)
        {
            GridIntensity gInt = new GridIntensity();
            gInt.wMan = workspaceManager;
            gInt.originalValue = Settings.Default.GridIntensity;
            gInt.ShowDialog();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.Size.Width < this.MinimumSize.Width || this.Size.Height < this.MinimumSize.Height)
            {
                return;
            }
            int workingHeight = this.Height - 37;
            
            controlsShadowClip.Size = new Size(controlsShadowClip.Width, workingHeight);
            controls.Size = new Size(controls.Width, workingHeight);
            workSpaceShadowClip.Size = new Size(this.Width - 222, workingHeight);
            helpButton.Location = new Point(this.Width - 131, workingHeight - 41);
            exportSettings.Location = new Point(this.Width - 363, workingHeight - 38);
            exportButton.Location = new Point(this.Width - 492, workingHeight - 41);
            tileViewButton.Location = new Point(this.Width - 596, workingHeight - 41);

            Size rect;
            if (currentTool == Tool.Zoom)
            {
                rect = texture.Size;
            } else
            {
                rect = zoomRegion.Size;
            }

            int workSpaceWidth = workSpaceShadowClip.Width - 326;
            int workSpaceHeight = workSpaceShadowClip.Height - 284;
            SizeF widthBasedSize = new SizeF(workSpaceWidth, (float)(((double)rect.Height / (double)rect.Width) * (double)workSpaceWidth));
            SizeF heightBasedSize = new SizeF((float)(((double)rect.Width / (double)rect.Height) * (double)workSpaceHeight), workSpaceHeight);
            if (heightBasedSize.Width > workSpaceWidth)
            {
                workSpace.Size = widthBasedSize.ToSize();
                workspaceWidth = widthBasedSize.Width;
                workspaceHeight = widthBasedSize.Height;
            } else
            {
                workSpace.Size = heightBasedSize.ToSize();
                workspaceWidth = heightBasedSize.Width;
                workspaceHeight = heightBasedSize.Height;
            }

            centerWorkspace();
            updateWorkspace();

            workSpace.Tag = (int)1;
            controls.Tag = (int)1;
            workSpaceShadowClip.Invalidate();
            controlsShadowClip.Invalidate();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            workspaceManager.Redraw();
        }

        private void workSpace_Paint(object sender, PaintEventArgs e)
        {
            /*Timer t = new Timer();
            t.Interval = 100;
            t.Tick += (object senderr, EventArgs ee) => 
            {
                workspaceManager.Redraw();
                t.Stop();
                t.Dispose();
            };
            t.Start();*/
        }

        private void rSlider_MouseDown(object sender, MouseEventArgs e)
        {
            if (workspaceManager.Selection.X != -1)
            {
                workspaceManager.SaveToUndo();
            }
        }

        private void gSlider_MouseDown(object sender, MouseEventArgs e)
        {
            if (workspaceManager.Selection.X != -1)
            {
                workspaceManager.SaveToUndo();
            }
        }

        private void bSlider_MouseDown(object sender, MouseEventArgs e)
        {
            if (workspaceManager.Selection.X != -1)
            {
                workspaceManager.SaveToUndo();
            }
        }

        private void aSlider_MouseDown(object sender, MouseEventArgs e)
        {
            if (workspaceManager.Selection.X != -1)
            {
                workspaceManager.SaveToUndo();
            }
        }

        private void moreResolutions_Click(object sender, EventArgs e)
        {
            MoreResolutions mr = new MoreResolutions(resolution);
            mr.ShowDialog();
            if (!mr.cancelled)
            {
                rescale(mr.currentVal);
                refreshResButtons();
            }
        }

        private void solidBgCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            workspaceManager.Redraw();
        }

        private void solidBgSettings_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.SolidColorOnly = true;
            cd.ShowDialog();
            Settings.Default.SolidBgColor = cd.Color;
            Settings.Default.Save();
            workspaceManager.Redraw();
        }

        private void tileViewButton_Click(object sender, EventArgs e)
        {
            tileViewButton.Enabled = false;
            currentTileView = new TileView(workspaceManager, solidBgCheckbox);
            currentTileView.Show();
            currentTileView.FormClosed += CurrentTileView_FormClosed;
        }

        private void CurrentTileView_FormClosed(object sender, FormClosedEventArgs e)
        {
            tileViewButton.Enabled = true;
            currentTileView = null;
        }

        private void exportSettings_Click(object sender, EventArgs e)
        {
            using (DefaultEditor de = new DefaultEditor())
            {
                de.ShowDialog();
            }
            defaultEditor();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            using (CloseConfirm cc = new CloseConfirm())
            {
                cc.ShowDialog();
                e.Cancel = cc.cancel;
            }
        }
    }
    public enum Tool : byte
    {
        Pen = 0,
        Select = 1,
        Picker = 2,
        Zoom = 3,
        Wand = 4
    }
    public static class ProcessExtensions
    {
        public static IEnumerable<Process> GetChildProcesses(this Process process)
        {
            List<Process> children = new List<Process>();
            ManagementObjectSearcher mos = new ManagementObjectSearcher(String.Format("Select * From Win32_Process Where ParentProcessID={0}", process.Id));

            foreach (ManagementObject mo in mos.Get())
            {
                children.Add(Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])));
            }

            return children;
        }
    }
}
