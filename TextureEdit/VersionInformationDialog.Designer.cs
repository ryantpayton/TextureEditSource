namespace TextureEdit
{
    partial class VersionInformationDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.versionsBox = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.changesBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(309, 47);
            this.label1.TabIndex = 0;
            this.label1.Text = "Version Information";
            // 
            // versionsBox
            // 
            this.versionsBox.BackColor = System.Drawing.Color.DimGray;
            this.versionsBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.versionsBox.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionsBox.ForeColor = System.Drawing.Color.White;
            this.versionsBox.FormattingEnabled = true;
            this.versionsBox.ItemHeight = 21;
            this.versionsBox.Items.AddRange(new object[] {
            "v1.14 (Current Version)"});
            this.versionsBox.Location = new System.Drawing.Point(21, 64);
            this.versionsBox.Name = "versionsBox";
            this.versionsBox.Size = new System.Drawing.Size(451, 128);
            this.versionsBox.TabIndex = 1;
            this.versionsBox.SelectedValueChanged += new System.EventHandler(this.versionsBox_SelectedValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(21, 201);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 21);
            this.label2.TabIndex = 2;
            this.label2.Text = "Changes";
            // 
            // changesBox
            // 
            this.changesBox.BackColor = System.Drawing.Color.DimGray;
            this.changesBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.changesBox.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.changesBox.ForeColor = System.Drawing.Color.White;
            this.changesBox.FormattingEnabled = true;
            this.changesBox.ItemHeight = 21;
            this.changesBox.Items.AddRange(new object[] {
            "v1.14 (Current Version)"});
            this.changesBox.Location = new System.Drawing.Point(21, 225);
            this.changesBox.Name = "changesBox";
            this.changesBox.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.changesBox.Size = new System.Drawing.Size(451, 86);
            this.changesBox.TabIndex = 3;
            // 
            // VersionInformationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(493, 335);
            this.Controls.Add(this.changesBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.versionsBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VersionInformationDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Version Info";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox versionsBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox changesBox;
    }
}