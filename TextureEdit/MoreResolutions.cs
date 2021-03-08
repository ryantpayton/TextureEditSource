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
    public partial class MoreResolutions : Form
    {
        public int currentVal;
        public bool cancelled = true;
        public MoreResolutions(int val)
        {
            currentVal = val;
            InitializeComponent();
            resBox.Value = val;
        }

        private void scaleButton_Click(object sender, EventArgs e)
        {
            cancelled = false;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void resBox_ValueChanged(object sender, EventArgs e)
        {
            if (resBox.Value > currentVal)
            {
                resBox.Value = currentVal * 2;
            }
            if (resBox.Value < currentVal)
            {
                resBox.Value = currentVal / 2;
            }
            if (resBox.Value > 1024)
            {
                resBox.Value = 1024;
            }
            if (resBox.Value < 1)
            {
                resBox.Value = 1;
            }
            currentVal = (int)resBox.Value;
        }
    }
}
