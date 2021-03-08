namespace TextureEdit
{
    partial class ColorShiftDialog
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
            this.rLabel = new System.Windows.Forms.Label();
            this.rSlider = new System.Windows.Forms.TrackBar();
            this.gSlider = new System.Windows.Forms.TrackBar();
            this.gLabel = new System.Windows.Forms.Label();
            this.bSlider = new System.Windows.Forms.TrackBar();
            this.bLabel = new System.Windows.Forms.Label();
            this.aSlider = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.applyEffectButton = new System.Windows.Forms.Button();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.rBox = new System.Windows.Forms.TextBox();
            this.gBox = new System.Windows.Forms.TextBox();
            this.bBox = new System.Windows.Forms.TextBox();
            this.aBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.rSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bSlider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.aSlider)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(175, 47);
            this.label1.TabIndex = 1;
            this.label1.Text = "Color Shift";
            // 
            // rLabel
            // 
            this.rLabel.AutoSize = true;
            this.rLabel.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.rLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rLabel.Location = new System.Drawing.Point(16, 56);
            this.rLabel.Name = "rLabel";
            this.rLabel.Size = new System.Drawing.Size(36, 21);
            this.rLabel.TabIndex = 3;
            this.rLabel.Text = "Red";
            // 
            // rSlider
            // 
            this.rSlider.Location = new System.Drawing.Point(20, 81);
            this.rSlider.Maximum = 255;
            this.rSlider.Minimum = -255;
            this.rSlider.Name = "rSlider";
            this.rSlider.Size = new System.Drawing.Size(321, 45);
            this.rSlider.TabIndex = 4;
            this.rSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            this.rSlider.ValueChanged += new System.EventHandler(this.rSlider_ValueChanged);
            // 
            // gSlider
            // 
            this.gSlider.Location = new System.Drawing.Point(20, 132);
            this.gSlider.Maximum = 255;
            this.gSlider.Minimum = -255;
            this.gSlider.Name = "gSlider";
            this.gSlider.Size = new System.Drawing.Size(321, 45);
            this.gSlider.TabIndex = 6;
            this.gSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            this.gSlider.ValueChanged += new System.EventHandler(this.gSlider_ValueChanged);
            // 
            // gLabel
            // 
            this.gLabel.AutoSize = true;
            this.gLabel.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.gLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.gLabel.Location = new System.Drawing.Point(16, 107);
            this.gLabel.Name = "gLabel";
            this.gLabel.Size = new System.Drawing.Size(51, 21);
            this.gLabel.TabIndex = 5;
            this.gLabel.Text = "Green";
            // 
            // bSlider
            // 
            this.bSlider.Location = new System.Drawing.Point(20, 183);
            this.bSlider.Maximum = 255;
            this.bSlider.Minimum = -255;
            this.bSlider.Name = "bSlider";
            this.bSlider.Size = new System.Drawing.Size(321, 45);
            this.bSlider.TabIndex = 8;
            this.bSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            this.bSlider.ValueChanged += new System.EventHandler(this.bSlider_ValueChanged);
            // 
            // bLabel
            // 
            this.bLabel.AutoSize = true;
            this.bLabel.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.bLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.bLabel.Location = new System.Drawing.Point(16, 158);
            this.bLabel.Name = "bLabel";
            this.bLabel.Size = new System.Drawing.Size(39, 21);
            this.bLabel.TabIndex = 7;
            this.bLabel.Text = "Blue";
            // 
            // aSlider
            // 
            this.aSlider.Location = new System.Drawing.Point(20, 234);
            this.aSlider.Maximum = 255;
            this.aSlider.Minimum = -255;
            this.aSlider.Name = "aSlider";
            this.aSlider.Size = new System.Drawing.Size(321, 45);
            this.aSlider.TabIndex = 10;
            this.aSlider.TickStyle = System.Windows.Forms.TickStyle.None;
            this.aSlider.ValueChanged += new System.EventHandler(this.aSlider_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.label5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label5.Location = new System.Drawing.Point(16, 209);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(142, 21);
            this.label5.TabIndex = 9;
            this.label5.Text = "Alpha/Transparency";
            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.cancelButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cancelButton.Location = new System.Drawing.Point(187, 297);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 30);
            this.cancelButton.TabIndex = 12;
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
            this.applyEffectButton.Location = new System.Drawing.Point(268, 297);
            this.applyEffectButton.Name = "applyEffectButton";
            this.applyEffectButton.Size = new System.Drawing.Size(114, 30);
            this.applyEffectButton.TabIndex = 11;
            this.applyEffectButton.Text = "Apply Effect";
            this.applyEffectButton.UseVisualStyleBackColor = false;
            this.applyEffectButton.Click += new System.EventHandler(this.applyEffectButton_Click);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Enabled = false;
            this.checkBox3.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.checkBox3.Location = new System.Drawing.Point(25, 302);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(156, 25);
            this.checkBox3.TabIndex = 16;
            this.checkBox3.Text = "Apply To All Frames";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.checkBox1.Location = new System.Drawing.Point(25, 271);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(83, 25);
            this.checkBox1.TabIndex = 17;
            this.checkBox1.Text = "Use HSL";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // rBox
            // 
            this.rBox.Font = new System.Drawing.Font("Segoe UI Light", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rBox.Location = new System.Drawing.Point(347, 81);
            this.rBox.Name = "rBox";
            this.rBox.Size = new System.Drawing.Size(35, 22);
            this.rBox.TabIndex = 18;
            this.rBox.Text = "0";
            this.rBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rBox_KeyDown);
            // 
            // gBox
            // 
            this.gBox.Font = new System.Drawing.Font("Segoe UI Light", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gBox.Location = new System.Drawing.Point(347, 132);
            this.gBox.Name = "gBox";
            this.gBox.Size = new System.Drawing.Size(35, 22);
            this.gBox.TabIndex = 19;
            this.gBox.Text = "0";
            this.gBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gBox_KeyDown);
            // 
            // bBox
            // 
            this.bBox.Font = new System.Drawing.Font("Segoe UI Light", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bBox.Location = new System.Drawing.Point(347, 183);
            this.bBox.Name = "bBox";
            this.bBox.Size = new System.Drawing.Size(35, 22);
            this.bBox.TabIndex = 20;
            this.bBox.Text = "0";
            this.bBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.bBox_KeyDown);
            // 
            // aBox
            // 
            this.aBox.Font = new System.Drawing.Font("Segoe UI Light", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aBox.Location = new System.Drawing.Point(347, 234);
            this.aBox.Name = "aBox";
            this.aBox.Size = new System.Drawing.Size(35, 22);
            this.aBox.TabIndex = 21;
            this.aBox.Text = "0";
            this.aBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.aBox_KeyDown);
            // 
            // ColorShiftDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(403, 341);
            this.Controls.Add(this.aBox);
            this.Controls.Add(this.bBox);
            this.Controls.Add(this.gBox);
            this.Controls.Add(this.rBox);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.applyEffectButton);
            this.Controls.Add(this.aSlider);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.bSlider);
            this.Controls.Add(this.bLabel);
            this.Controls.Add(this.gSlider);
            this.Controls.Add(this.gLabel);
            this.Controls.Add(this.rSlider);
            this.Controls.Add(this.rLabel);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColorShiftDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Color Shift";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ColorShiftDialog_FormClosing);
            this.Shown += new System.EventHandler(this.ColorShiftDialog_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.rSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bSlider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.aSlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label rLabel;
        private System.Windows.Forms.TrackBar rSlider;
        private System.Windows.Forms.TrackBar gSlider;
        private System.Windows.Forms.Label gLabel;
        private System.Windows.Forms.TrackBar bSlider;
        private System.Windows.Forms.Label bLabel;
        private System.Windows.Forms.TrackBar aSlider;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button applyEffectButton;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox rBox;
        private System.Windows.Forms.TextBox gBox;
        private System.Windows.Forms.TextBox bBox;
        private System.Windows.Forms.TextBox aBox;
    }
}