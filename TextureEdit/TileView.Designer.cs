namespace TextureEdit
{
    partial class TileView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.canvas = new System.Windows.Forms.PictureBox();
            this.tiles = new System.Windows.Forms.NumericUpDown();
            this.notificationText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tiles)).BeginInit();
            this.SuspendLayout();
            // 
            // canvas
            // 
            this.canvas.Location = new System.Drawing.Point(0, 0);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(503, 477);
            this.canvas.TabIndex = 0;
            this.canvas.TabStop = false;
            this.canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.canvas_Paint);
            this.canvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseDown);
            this.canvas.MouseLeave += new System.EventHandler(this.canvas_MouseLeave);
            this.canvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseMove);
            this.canvas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.canvas_MouseUp);
            // 
            // tiles
            // 
            this.tiles.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tiles.Location = new System.Drawing.Point(12, 12);
            this.tiles.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.tiles.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tiles.Name = "tiles";
            this.tiles.Size = new System.Drawing.Size(47, 29);
            this.tiles.TabIndex = 1;
            this.tiles.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.tiles.ValueChanged += new System.EventHandler(this.tiles_ValueChanged);
            // 
            // notificationText
            // 
            this.notificationText.BackColor = System.Drawing.Color.Transparent;
            this.notificationText.Font = new System.Drawing.Font("Segoe UI Light", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.notificationText.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.notificationText.Location = new System.Drawing.Point(135, 197);
            this.notificationText.Name = "notificationText";
            this.notificationText.Size = new System.Drawing.Size(267, 83);
            this.notificationText.TabIndex = 0;
            this.notificationText.Text = "Tile View will update when you let go";
            this.notificationText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.notificationText.Visible = false;
            // 
            // TileView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(496, 473);
            this.Controls.Add(this.notificationText);
            this.Controls.Add(this.tiles);
            this.Controls.Add(this.canvas);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TileView";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tile View";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TileView_FormClosed);
            this.ResizeBegin += new System.EventHandler(this.TileView_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.TileView_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.TileView_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tiles)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox canvas;
        private System.Windows.Forms.NumericUpDown tiles;
        private System.Windows.Forms.Label notificationText;
    }
}