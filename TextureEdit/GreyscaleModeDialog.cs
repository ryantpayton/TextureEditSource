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
    public partial class GreyscaleModeDialog : Form
    {
        public bool result;
        public GreyscaleModeDialog()
        {
            InitializeComponent();
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            result = true;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            result = false;
            this.Close();
        }
    }
}
