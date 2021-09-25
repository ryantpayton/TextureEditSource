using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextureEdit
{
    public partial class Update : Form
    {
        public Update(string updateDescription)
        {
            InitializeComponent();
            description.Text = updateDescription;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            byte[] newTextureEdit = Form1.GetB("https://github.com/ryantpayton/TextureEdit/raw/master/TextureEdit.exe");
            string file = Process.GetCurrentProcess().MainModule.FileName;
            if (File.Exists(file + "_"))
            {
                File.Delete(file + "_");
            }
            File.Move(file, file + "_");
            File.WriteAllBytes(file, newTextureEdit);
            Process.Start(file, "updated");
            Environment.Exit(1);
        }
    }
}
