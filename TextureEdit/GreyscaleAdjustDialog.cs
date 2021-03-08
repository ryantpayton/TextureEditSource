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
    public partial class GreyscaleAdjustDialog : Form
    {
        public Bitmap flipbookReference;
        public WorkspaceManager workspaceManager;
        private bool closingFromCode = false;
        public GreyscaleAdjustDialog()
        {
            InitializeComponent();
            trackBar1.Value = 50;
        }

        private void updateEffect()
        {
            bool invert = checkBox1.Checked;
            float value = (trackBar1.Value / 100F) * 2F;
            workspaceManager.PreviewGreyscaleAdjustSelection(value, trackBar2.Value, invert, checkBox2.Checked);
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            cBox.Text = (trackBar1.Value - 50).ToString();
            updateEffect();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            updateEffect();
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
            } else
            {
                workspaceManager.CommitEffect();
            }
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            closingFromCode = true;
            workspaceManager.CancelEffect();
            this.Close();
        }

        private void GreyscaleAdjustDialog_Shown(object sender, EventArgs e)
        {
            if (flipbookReference != null)
            {
                checkBox3.Enabled = true;
            }
            updateEffect();
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            oBox.Text = trackBar2.Value.ToString();
            updateEffect();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            updateEffect();
        }

        private void GreyscaleAdjustDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!closingFromCode)
            {
                workspaceManager.CancelEffect();
            }
        }
        private void backBox(TextBox box, TrackBar slider, int min, int max, int add)
        {
            int val;
            if (int.TryParse(box.Text, out val) && (val <= max) && (val >= min))
            {
                slider.Value = val + add;
            }
            else
            {
                box.Text = (slider.Value - add).ToString();
            }
        }

        private void cBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                backBox(cBox, trackBar1, -50, 100, 50);
            }
        }

        private void oBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                backBox(oBox, trackBar2, -255, 255, 0);
            }
        }
    }
}
