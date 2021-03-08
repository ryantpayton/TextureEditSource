namespace TextureEdit
{
    partial class AddNoiseDialog
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
            this.intensitySlider = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.applyEffectButton = new System.Windows.Forms.Button();
            this.iBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.intensitySlider)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 47);
            this.label1.TabIndex = 1;
            this.label1.Text = "Add Noise";
            // 
            // intensitySlider
            // 
            this.intensitySlider.Location = new System.Drawing.Point(20, 80);
            this.intensitySlider.Maximum = 255;
            this.intensitySlider.Name = "intensitySlider";
            this.intensitySlider.Size = new System.Drawing.Size(321, 45);
            this.intensitySlider.TabIndex = 3;
            this.intensitySlider.TickStyle = System.Windows.Forms.TickStyle.None;
            this.intensitySlider.ValueChanged += new System.EventHandler(this.intensitySlider_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(16, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 21);
            this.label2.TabIndex = 4;
            this.label2.Text = "Intensity";
            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.cancelButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cancelButton.Location = new System.Drawing.Point(187, 114);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 30);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = false;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // applyEffectButton
            // 
            this.applyEffectButton.BackColor = System.Drawing.Color.DodgerBlue;
            this.applyEffectButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.applyEffectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.applyEffectButton.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.applyEffectButton.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.applyEffectButton.Location = new System.Drawing.Point(268, 114);
            this.applyEffectButton.Name = "applyEffectButton";
            this.applyEffectButton.Size = new System.Drawing.Size(114, 30);
            this.applyEffectButton.TabIndex = 9;
            this.applyEffectButton.Text = "Apply Effect";
            this.applyEffectButton.UseVisualStyleBackColor = false;
            this.applyEffectButton.Click += new System.EventHandler(this.applyEffectButton_Click);
            // 
            // iBox
            // 
            this.iBox.Font = new System.Drawing.Font("Segoe UI Light", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iBox.Location = new System.Drawing.Point(347, 80);
            this.iBox.Name = "iBox";
            this.iBox.Size = new System.Drawing.Size(35, 22);
            this.iBox.TabIndex = 20;
            this.iBox.Text = "5";
            this.iBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.iBox_KeyDown);
            // 
            // AddNoiseDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(397, 159);
            this.Controls.Add(this.iBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.applyEffectButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.intensitySlider);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddNoiseDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Noise";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AddNoiseDialog_FormClosing);
            this.Shown += new System.EventHandler(this.AddNoiseDialog_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.intensitySlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar intensitySlider;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button applyEffectButton;
        private System.Windows.Forms.TextBox iBox;
    }
}