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
    public partial class DefaultEditor : Form
    {
        OpenWith openWith = new OpenWith("temp.png");
        List<OpenWithApplication> apps;
        public DefaultEditor()
        {
            InitializeComponent();
            apps = openWith.GetApplications();
            foreach (OpenWithApplication app in apps)
            {
                listBox1.Items.Add(app.Name);
            }
            listBox1.SelectedIndex = 0;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void setButton_Click(object sender, EventArgs e)
        {
            Settings.Default.DefaultEditorProgID = apps[listBox1.SelectedIndex].ProgID;
            Settings.Default.Save();
            this.Close();
        }
    }
}
