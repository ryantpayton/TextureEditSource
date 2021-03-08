namespace TextureEdit
{
    partial class HelpDialog
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
            this.topic = new System.Windows.Forms.ListBox();
            this.title = new System.Windows.Forms.Label();
            this.description = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.shortcuts = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 47);
            this.label1.TabIndex = 1;
            this.label1.Text = "Help";
            // 
            // topic
            // 
            this.topic.BackColor = System.Drawing.Color.DimGray;
            this.topic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.topic.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.topic.ForeColor = System.Drawing.Color.White;
            this.topic.FormattingEnabled = true;
            this.topic.ItemHeight = 21;
            this.topic.Items.AddRange(new object[] {
            "Pen/Brush",
            "Select",
            "Picker",
            "Zoom",
            "Channels",
            "Opening & Saving",
            "Scaling & Resolutions",
            "Undo",
            "Color Shifting",
            "Greyscale Adjustments",
            "Copy/paste",
            "Flipbook textures",
            "r/TextureEdit"});
            this.topic.Location = new System.Drawing.Point(20, 59);
            this.topic.Name = "topic";
            this.topic.Size = new System.Drawing.Size(160, 338);
            this.topic.TabIndex = 2;
            this.topic.SelectedIndexChanged += new System.EventHandler(this.topic_SelectedIndexChanged);
            // 
            // title
            // 
            this.title.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.title.Location = new System.Drawing.Point(187, 9);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(240, 66);
            this.title.TabIndex = 3;
            this.title.Text = "Pen/Brush Tool";
            this.title.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // description
            // 
            this.description.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.description.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.description.Location = new System.Drawing.Point(187, 75);
            this.description.Name = "description";
            this.description.Size = new System.Drawing.Size(240, 274);
            this.description.TabIndex = 4;
            this.description.Text = "Blah blah blah blah blah blah blah";
            // 
            // closeButton
            // 
            this.closeButton.BackColor = System.Drawing.Color.DimGray;
            this.closeButton.FlatAppearance.BorderSize = 0;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.closeButton.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.closeButton.Location = new System.Drawing.Point(20, 405);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(407, 27);
            this.closeButton.TabIndex = 5;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(187, 349);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 21);
            this.label2.TabIndex = 6;
            this.label2.Text = "Shortcuts";
            // 
            // shortcuts
            // 
            this.shortcuts.Font = new System.Drawing.Font("Segoe UI Light", 12F);
            this.shortcuts.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.shortcuts.Location = new System.Drawing.Point(187, 370);
            this.shortcuts.Name = "shortcuts";
            this.shortcuts.Size = new System.Drawing.Size(270, 27);
            this.shortcuts.TabIndex = 7;
            this.shortcuts.Text = "Blah blah blah blah blah blah blah";
            // 
            // HelpDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(450, 445);
            this.Controls.Add(this.shortcuts);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.description);
            this.Controls.Add(this.title);
            this.Controls.Add(this.topic);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HelpDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Help";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox topic;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.Label description;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label shortcuts;
    }
}