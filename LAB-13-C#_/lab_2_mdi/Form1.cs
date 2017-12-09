using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace imageeditor
{
    public partial class Form1 : Form
    {
        public float lineWidth = 2F;
        public Color penColor = Color.DarkGreen;
        public Color solidBrushColor = Color.LightGreen;
        public Font drawTextFont = new Font(FontFamily.GenericSansSerif, 8F);
        Size pictureSize = new Size(800, 600);
        public int selectedFigure = 0;
        public bool isFilled = true;
        public bool selectSwitch = false;
        public int MeshSize = 10;
        public bool MeshOn = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2(pictureSize);
            f.MdiParent = this;
            f.Text = "Picture " + this.MdiChildren.Length.ToString();
            f.Show();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Form2 f;
                if (MdiChildren.Length > 0)
                {
                    f = (Form2)this.ActiveMdiChild;
                    // Если есть дочерние окна то сначала проверяем
                    // Если сохранена 
                    if (f.isModified)
                    {
                        if (MessageBox.Show("do you want to save changes?", "document has been modified", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            saveToFile(f);
                        }
                    }
                    OpenFileDialog ofd = new OpenFileDialog();
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        f.saveFileName = ofd.FileName;
                        openFromStream(f);
                        f.pictureSize = f.listFigure[0].rectangle.Size;
                        f.Size = f.pictureSize;
                        f.AutoScrollMinSize = f.pictureSize;
                        f.Text = Path.GetFileName(f.saveFileName);
                        f.isSaved = true;
                        f.bufferedGraphics.Dispose();
                        f.bufferedGraphics = f.bufferedGraphicsContext.Allocate(f.CreateGraphics(), f.DisplayRectangle);
                    }
                }
                else
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        f = new Form2(pictureSize);
                        f.MdiParent = this;
                        f.saveFileName = ofd.FileName;
                        openFromStream(f);

                        f.pictureSize = f.listFigure[0].rectangle.Size;
                        f.Size = f.pictureSize;
                        f.AutoScrollMinSize = f.pictureSize;
                        f.Text = Path.GetFileName(f.saveFileName);
                        f.isSaved = true;
                        f.Show();

                        f.bufferedGraphics.Dispose();
                        f.bufferedGraphics = f.bufferedGraphicsContext.Allocate(f.CreateGraphics(), f.DisplayRectangle);
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: {0}", ex.Message);
            }
        }

        public void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Form2 f = (Form2)ActiveMdiChild;
                saveToFile(f);
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: {0}", ex.Message);
            }


        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Form2 f = (Form2)this.ActiveMdiChild;
                SaveFileDialog sfd = new SaveFileDialog();
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    f.saveFileName = sfd.FileName;
                    saveToStream(f);
                    f.Text = Path.GetFileName(f.saveFileName);
                    f.isSaved = true;
                    f.isModified = false;
                    f.Invalidate();
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception: {0}", ex.Message);
            }


        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length > 0)
            {
                saveToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;
            }
            else
            {
                saveToolStripMenuItem.Enabled = false;
                saveAsToolStripMenuItem.Enabled = false;
            }
        }

        private void lineWidthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lineWidthDialog d = new lineWidthDialog();
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                lineWidth = d.lineWidth;
            }
            sb_penSize.Text = lineWidth.ToString();
        }

        private void lineColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog d = new ColorDialog();
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                penColor = d.Color;
            }
            statusBar1.Invalidate();
        }

        private void fillColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog d = new ColorDialog();
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                solidBrushColor = d.Color;
            }
            statusBar1.Invalidate();
        }

        private void pictureSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PictureSizeDialog pictureSizeDialog = new PictureSizeDialog();
            if (pictureSizeDialog.ShowDialog() == DialogResult.OK)
            {
                pictureSize = pictureSizeDialog.pictureSize;
            }
            // sb_pictureSize.Text = pictureSize.ToString();
        }

        void saveToStream(Form2 f)
        {
            BinaryFormatter bf = new BinaryFormatter();
            Stream fs = new FileStream(f.saveFileName, FileMode.Create, FileAccess.Write, FileShare.None);
            bf.Serialize(fs, f.listFigure);
            fs.Close();
        }

        void openFromStream(Form2 f)
        {
            BinaryFormatter bf = new BinaryFormatter();
            Stream fs = new FileStream(f.saveFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            f.listFigure = (List<Figure>)bf.Deserialize(fs);
            fs.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public bool saveToFile(Form2 f)
        {
            if (f.isSaved)
            {
                saveToStream(f);
                f.isModified = false;
                f.Invalidate();
                return true;
            }
            else
            {
                SaveFileDialog sfd = new SaveFileDialog();
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    f.saveFileName = sfd.FileName;
                    saveToStream(f);
                    f.Text = Path.GetFileName(f.saveFileName);
                    f.isSaved = true;
                    f.isModified = false;
                    f.Invalidate();
                    return true;
                }
            }
            return false;
        }

        private void rectangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedFigure = 0; // rectangle
            rectangleToolStripMenuItem.Checked = true;
            ellipseToolStripMenuItem.Checked = false;
            lineToolStripMenuItem.Checked = false;
            curveToolStripMenuItem.Checked = false;

            toolStripButton8.Checked = true;
            toolStripButton9.Checked = false;
            toolStripButton10.Checked = false;
            toolStripButton11.Checked = false;

            text_tool_strip_button.Checked = false;
            textToolStripMenuItem.Checked = false;
        }

        private void ellipseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedFigure = 1; // ellipse
            rectangleToolStripMenuItem.Checked = false;
            ellipseToolStripMenuItem.Checked = true;
            lineToolStripMenuItem.Checked = false;
            curveToolStripMenuItem.Checked = false;

            toolStripButton8.Checked = false;
            toolStripButton9.Checked = true;
            toolStripButton10.Checked = false;
            toolStripButton11.Checked = false;

            text_tool_strip_button.Checked = false;
            textToolStripMenuItem.Checked = false;
        }

        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedFigure = 2; // line
            rectangleToolStripMenuItem.Checked = false;
            ellipseToolStripMenuItem.Checked = false;
            lineToolStripMenuItem.Checked = true;
            curveToolStripMenuItem.Checked = false;

            toolStripButton8.Checked = false;
            toolStripButton9.Checked = false;
            toolStripButton10.Checked = true;
            toolStripButton11.Checked = false;

            text_tool_strip_button.Checked = false;
            textToolStripMenuItem.Checked = false;
        }

        private void curveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedFigure = 3; // curve
            rectangleToolStripMenuItem.Checked = false;
            ellipseToolStripMenuItem.Checked = false;
            lineToolStripMenuItem.Checked = false;
            curveToolStripMenuItem.Checked = true;

            toolStripButton8.Checked = false;
            toolStripButton9.Checked = false;
            toolStripButton10.Checked = false;
            toolStripButton11.Checked = true;

            text_tool_strip_button.Checked = false;
            textToolStripMenuItem.Checked = false;
        }

        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedFigure = 4; // Text
            rectangleToolStripMenuItem.Checked = false;
            ellipseToolStripMenuItem.Checked = false;
            lineToolStripMenuItem.Checked = false;
            curveToolStripMenuItem.Checked = false;
            textToolStripMenuItem.Checked = true;

            toolStripButton8.Checked = false;
            toolStripButton9.Checked = false;
            toolStripButton10.Checked = false;
            toolStripButton11.Checked = false;
            text_tool_strip_button.Checked = true;
        }

        private void filledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isFilled)
            {
                toolStripButton12.Checked = false;
                filledToolStripMenuItem.Checked = false;
                isFilled = false;
            }
            else
            {
                toolStripButton12.Checked = true;
                filledToolStripMenuItem.Checked = true;
                isFilled = true;
            }
        }

        private void statusBar1_DrawItem(object sender, StatusBarDrawItemEventArgs sbdevent)
        {

            sb_penSize.Text = lineWidth.ToString();

            if (sbdevent.Panel == sb_penColor)
            {
                sbdevent.Graphics.FillRectangle(new SolidBrush(penColor), sbdevent.Bounds);
            }
            else if (sbdevent.Panel == sb_brushColor)
            {
                sbdevent.Graphics.FillRectangle(new SolidBrush(solidBrushColor), sbdevent.Bounds);
            }
        }

        private void text_tool_strip_button_Click(object sender, EventArgs e)
        {

        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                drawTextFont = fontDialog.Font;
                Console.WriteLine(drawTextFont);

                sb_font.Text = drawTextFont.Name + " " + drawTextFont.Size;
            }

        }

        private void font_dialog_tool_strip_button_Click(object sender, EventArgs e)
        {

        }

        private void selectToolStripButton_Click(object sender, EventArgs e)
        {
            if (selectSwitch == true)
            {
                selectSwitch = false;
                selectToolStripButton.Checked = false;
                selectToolStripMenuItem.Checked = false;
            }
            else
            {
                selectSwitch = true;
                selectToolStripButton.Checked = true;
                selectToolStripMenuItem.Checked = true;
            }
        }

        public void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            List<Figure> toRemoveList = new List<Figure>();
            toRemoveList.Clear();
            for (int i = 1; i < ((Form2)ActiveMdiChild).listFigure.Count; i++)
            {
                if (((Form2)ActiveMdiChild).listFigure[i].isSelected)
                {
                    toRemoveList.Add(((Form2)ActiveMdiChild).listFigure[i]);
                    Console.WriteLine("item {0} is added to remove", i);
                }
            }
            foreach (var item in toRemoveList)
            {
                ((Form2)ActiveMdiChild).listFigure.Remove(item);
            }
            ((Form2)ActiveMdiChild).Invalidate();
        }

        // Copy
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 activeForm = (Form2)ActiveMdiChild;
            List<Figure> listSelObj = new List<Figure>();

            // Добавляем выделенные фигуры в listSelObj
            for (int i = 1; i < activeForm.listFigure.Count; i++)
            {
                if (activeForm.listFigure[i].isSelected)
                {
                    listSelObj.Add(activeForm.listFigure[i]);
                }
            }

            // Put to clipboard

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, listSelObj);

            DataObject dataObj1 = new DataObject();
            dataObj1.SetData("my_application_format", ms);
            Clipboard.SetDataObject(dataObj1, true);

        }

        // Paste
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IDataObject idata_obj = Clipboard.GetDataObject();
            MemoryStream ms_new = (MemoryStream)idata_obj.GetData("my_application_format");
            BinaryFormatter bf = new BinaryFormatter();
            List<Figure> list_new = (List<Figure>)bf.Deserialize(ms_new);

            Form2 f2 = (Form2)ActiveMdiChild;
            // Добавление к существующей картинке
            foreach (var item in list_new)
            {
                f2.listFigure.Add(item);
            }
            f2.Invalidate();
        }

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            copyToolStripMenuItem.Enabled = false;
            copyAsMetaFileToolStripMenuItem.Enabled = false;
            cutToolStripMenuItem.Enabled = false;
            pasteToolStripMenuItem.Enabled = false;
            selectAllToolStripMenuItem.Enabled = false;

            if (MdiChildren.Length == 0)
            {
                // Если нет открытых окон
                copyToolStripMenuItem.Enabled = false;
                copyAsMetaFileToolStripMenuItem.Enabled = false;
                cutToolStripMenuItem.Enabled = false;
                pasteToolStripMenuItem.Enabled = false;
                selectAllToolStripMenuItem.Enabled = false;
            }
            else
            {
                Form2 activeForm = (Form2)ActiveMdiChild;
                // Проверяем если выделена хоть одна фигура
                bool anySelected = false;
                for (int i = 1; i < activeForm.listFigure.Count; i++)
                {
                    if (activeForm.listFigure[i].isSelected)
                    {
                        anySelected = true;
                        break;
                    }
                }

                if (anySelected == true)
                {
                    copyToolStripMenuItem.Enabled = true;
                    copyAsMetaFileToolStripMenuItem.Enabled = true;
                    cutToolStripMenuItem.Enabled = true;
                    pasteToolStripMenuItem.Enabled = false;
                    selectAllToolStripMenuItem.Enabled = false;
                }
                else
                {
                    copyToolStripMenuItem.Enabled = false;
                    copyAsMetaFileToolStripMenuItem.Enabled = false;
                    cutToolStripMenuItem.Enabled = false;
                    pasteToolStripMenuItem.Enabled = true;
                    selectAllToolStripMenuItem.Enabled = true;
                }
            }
        }

        private void lOLToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void test1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double sum = 0;
            double x = 1;
            double n;
            for (int i = 0; i < 250000; i++)
            {
                sum += 1 / x;
                n = sum - Math.Log(x);
                Console.WriteLine(sum + " " + n);
                x++;
            }
        }

        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void setSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetMeshSize sms = new SetMeshSize();
            sms.ShowDialog();
            if (sms.DialogResult == DialogResult.OK)
            {
                MeshSize = Convert.ToInt32(sms.textBox1.Text);
            }
        }

        private void onToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MeshOn == false)
            {
                MeshOn = true;
                onToolStripMenuItem.Checked = true;
                ((Form2)ActiveMdiChild).Invalidate();
            }
            else
            {
                MeshOn = false;
                onToolStripMenuItem.Checked = false;
                ((Form2)ActiveMdiChild).Invalidate();
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 activeForm = (Form2)ActiveMdiChild;
            List<Figure> listSelObj = new List<Figure>();

            // Добавляем выделенные фигуры в listSelObj
            for (int i = 1; i < activeForm.listFigure.Count; i++)
            {
                if (activeForm.listFigure[i].isSelected)
                {
                    listSelObj.Add(activeForm.listFigure[i]);
                }
            }

            // Put to clipboard

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, listSelObj);

            DataObject dataObj1 = new DataObject();
            dataObj1.SetData("my_application_format", ms);
            Clipboard.SetDataObject(dataObj1, true);

            // Delete
            for (int i = 1; i < activeForm.listFigure.Count; i++)
            {
                if (activeForm.listFigure[i].isSelected == true)
                {
                    activeForm.listFigure.RemoveAt(i);
                    // Если удалили обьект то остальные элементы смещаются назад
                    i--;
                }
            }

            activeForm.Invalidate();

        }

        private void copyAsMetaFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Graphics g = CreateGraphics();
            IntPtr dc = g.GetHdc();
            Metafile mf = new Metafile(dc, EmfType.EmfOnly);

            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, mf);
            DataObject data_obj = new DataObject();
            data_obj.SetData("myprogmetafile", ms);
            Clipboard.SetDataObject(data_obj);

            Console.WriteLine("ms lenght: " + ms.Length);

            g.ReleaseHdc(dc);
            g.Dispose();
        }

        //// Флаг модификации -- тест --
        //public bool toEdit = false;
        //private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        //{
        //    if (toEdit == true)
        //    {
        //        toEdit = false;
        //        editToolStripMenuItem1.Checked = false;
        //    }
        //    else
        //    {
        //        toEdit = true;
        //        editToolStripMenuItem1.Checked = true;
        //    }
        //}
    }
}
