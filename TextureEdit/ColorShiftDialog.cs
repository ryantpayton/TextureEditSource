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
    public partial class ColorShiftDialog : Form
    {
        public WorkspaceManager workspaceManager;
        public Bitmap flipbookReference;
        private bool closingFromCode = false;
        public ColorShiftDialog()
        {
            InitializeComponent();
        }

        private void updateEffect()
        {
            float r = 0;
            float g = 0;
            float b = 0;
            int a = aSlider.Value;
            if (checkBox1.Checked)
            {
                r = rSlider.Value / 255F;
                g = gSlider.Value / 255F;
                b = bSlider.Value / 255F;
            } else
            {
                r = rSlider.Value;
                g = gSlider.Value;
                b = bSlider.Value;
            }
            workspaceManager.PreviewColorShiftSelection(r, g, b, a, checkBox1.Checked);
        }

        private void applyEffectButton_Click(object sender, EventArgs e)
        {
            closingFromCode = true;
            if (flipbookReference != null && checkBox3.Checked)
            {
                Rectangle oldSelection = workspaceManager.Selection;

                workspaceManager.SuspendDrawing = true;
                workspaceManager.CancelEffect();
                Bitmap oldSource = workspaceManager.Source;
                workspaceManager.Source = flipbookReference;
                workspaceManager.InitializeWorkspace();
                workspaceManager.Selection = new Rectangle(0, 0, flipbookReference.Width, flipbookReference.Height);
                updateEffect();
                workspaceManager.CommitEffect();
                workspaceManager.Source = oldSource;
                workspaceManager.SuspendDrawing = false;
                workspaceManager.Selection = new Rectangle(-1, -1, -1, -1);
                workspaceManager.InitializeWorkspace();

                workspaceManager.Selection = oldSelection;
                updateEffect();
                workspaceManager.CommitEffect();
                //workspaceManager.Redraw();
            }
            else
            {
                workspaceManager.CommitEffect();
            }
            this.Close();
        }

        private void ColorShiftDialog_Shown(object sender, EventArgs e)
        {
            if (flipbookReference != null)
            {
                checkBox3.Enabled = true;
            }
            updateEffect();
        }

        private void rSlider_ValueChanged(object sender, EventArgs e)
        {
            rBox.Text = rSlider.Value.ToString();
            updateEffect();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            closingFromCode = true;
            workspaceManager.CancelEffect();
            this.Close();
        }

        private void gSlider_ValueChanged(object sender, EventArgs e)
        {
            gBox.Text = gSlider.Value.ToString();
            updateEffect();
        }

        private void bSlider_ValueChanged(object sender, EventArgs e)
        {
            bBox.Text = bSlider.Value.ToString();
            updateEffect();
        }

        private void aSlider_ValueChanged(object sender, EventArgs e)
        {
            aBox.Text = aSlider.Value.ToString();
            updateEffect();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                rLabel.Text = "Hue";
                gLabel.Text = "Saturation";
                bLabel.Text = "Lightness";
            } else
            {
                rLabel.Text = "Red";
                gLabel.Text = "Green";
                bLabel.Text = "Blue";
            }
            updateEffect();
        }

        private void ColorShiftDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!closingFromCode)
            {
                workspaceManager.CancelEffect();
            }
        }

        private void rBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                backBox(rBox, rSlider);
            }
        }
        private void backBox(TextBox box, TrackBar slider)
        {
            int val;
            if (int.TryParse(box.Text, out val) && (val <= 255) && (val >= -255))
            {
                slider.Value = val;
            }
            else
            {
                box.Text = slider.Value.ToString();
            }
        }

        private void gBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                backBox(gBox, gSlider);
            }
        }

        private void bBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                backBox(bBox, bSlider);
            }
        }

        private void aBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                backBox(aBox, aSlider);
            }
        }
    }
}
