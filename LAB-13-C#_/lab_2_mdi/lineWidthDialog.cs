using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace imageeditor
{
    public partial class lineWidthDialog : Form
    {
        public float lineWidth;

        public lineWidthDialog()
        {
            InitializeComponent();
            button1.DialogResult = DialogResult.OK;
            button2.DialogResult = DialogResult.Cancel;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            lineWidth = (float)Convert.ToInt32(comboBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        { }
    }
}
