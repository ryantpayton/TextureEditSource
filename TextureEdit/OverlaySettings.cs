using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TextureEdit.Properties;

namespace TextureEdit
{
    public partial class OverlaySettings : Form
    {
        public WorkspaceManager wMan;
        public int originalOpacity;
        private bool ClosedInCode = false;
        public OverlaySettings()
        {
            InitializeComponent();
        }

        private void aSlider_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.OverlayOpacity = aSlider.Value;
            wMan.Redraw();
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            Settings.Default.Save();
            ClosedInCode = true;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OverlaySettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ClosedInCode)
            {
                Settings.Default.OverlayOpacity = originalOpacity;
            }
        }

        private void OverlaySettings_Shown(object sender, EventArgs e)
        {
            aSlider.Value = originalOpacity;
        }
    }
}
