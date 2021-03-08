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
    public partial class AddNoiseDialog : Form
    {
        public WorkspaceManager workspace;
        private bool closingFromCode = false;
        public AddNoiseDialog()
        {
            InitializeComponent();
        }

        private void intensitySlider_ValueChanged(object sender, EventArgs e)
        {
            iBox.Text = intensitySlider.Value.ToString();
            workspace.PreviewAddNoiseToSelection(intensitySlider.Value);
        }

        private void applyEffectButton_Click(object sender, EventArgs e)
        {
            closingFromCode = true;
            workspace.CommitEffect();
            this.Close();
        }

        private void AddNoiseDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!closingFromCode)
            {
                workspace.CancelEffect();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AddNoiseDialog_Shown(object sender, EventArgs e)
        {
            intensitySlider.Value = 5;
        }

        private void iBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                backBox(iBox, intensitySlider, 0, 255, 0);
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
    }
}
