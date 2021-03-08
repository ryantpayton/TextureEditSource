namespace TextureEdit
{
    partial class MoreResolutions
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
            this.resBox = new System.Windows.Forms.NumericUpDown();
            this.scaleButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.resBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(9, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Set Resolution";
            // 
            // resBox
            // 
            this.resBox.BackColor = System.Drawing.Color.DimGray;
            this.resBox.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resBox.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.resBox.Location = new System.Drawing.Point(13, 38);
            this.resBox.Maximum = new decimal(new int[] {
            2048,
            0,
            0,
            0});
            this.resBox.Name = "resBox";
            this.resBox.Size = new System.Drawing.Size(375, 29);
            this.resBox.TabIndex = 1;
            this.resBox.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.resBox.ValueChanged += new System.EventHandler(this.resBox_ValueChanged);
            // 
            // scaleButton
            // 
            this.scaleButton.BackColor = System.Drawing.Color.DodgerBlue;
            this.scaleButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.scaleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.scaleButton.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scaleButton.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.scaleButton.Location = new System.Drawing.Point(304, 73);
            this.scaleButton.Name = "scaleButton";
            this.scaleButton.Size = new System.Drawing.Size(84, 30);
            this.scaleButton.TabIndex = 12;
            this.scaleButton.Text = "Scale";
            this.scaleButton.UseVisualStyleBackColor = false;
            this.scaleButton.Click += new System.EventHandler(this.scaleButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.cancelButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cancelButton.Location = new System.Drawing.Point(223, 73);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 30);
            this.cancelButton.TabIndex = 13;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = false;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // MoreResolutions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(400, 116);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.scaleButton);
            this.Controls.Add(this.resBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MoreResolutions";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "More Resolutions";
            ((System.ComponentModel.ISupportInitialize)(this.resBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown resBox;
        private System.Windows.Forms.Button scaleButton;
        private System.Windows.Forms.Button cancelButton;
    }
}