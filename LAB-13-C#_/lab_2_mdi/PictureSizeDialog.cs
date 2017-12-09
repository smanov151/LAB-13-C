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
    public partial class PictureSizeDialog : Form
    {
        public Size pictureSize;

        public PictureSizeDialog()
        {
            InitializeComponent();
            checkBox1.Checked = false;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                pictureSize.Width = Convert.ToInt32(textBox1.Text);
                pictureSize.Height = Convert.ToInt32(textBox2.Text);
            }
            else
            {
                if (radioButton1.Checked)
                {
                    pictureSize.Width = 320;
                    pictureSize.Height = 240;
                }
                else if (radioButton2.Checked)
                {
                    pictureSize.Width = 640;
                    pictureSize.Height = 480;
                }
                else if (radioButton3.Checked)
                {
                    pictureSize.Width = 800;
                    pictureSize.Height = 600;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                groupBox1.Enabled = false;
                textBox1.Enabled = true;
                textBox2.Enabled = true;
            }
            else
            {
                groupBox1.Enabled = true;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
            }
        }
    }
}
