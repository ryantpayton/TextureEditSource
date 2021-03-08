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
    public partial class GridIntensity : Form
    {
        public WorkspaceManager wMan;
        public int originalValue;
        private bool ClosedInCode = false;

        public GridIntensity()
        {
            InitializeComponent();
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

        private void gSlider_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.GridIntensity = gSlider.Value;
            wMan.Redraw();
        }

        private void GridIntensity_Shown(object sender, EventArgs e)
        {
            gSlider.Value = originalValue;
        }

        private void GridIntensity_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ClosedInCode)
            {
                Settings.Default.GridIntensity = originalValue;
            }
        }
    }
}
