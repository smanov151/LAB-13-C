using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace imageeditor
{
    public partial class Form2 : Form
    {
        Graphics g;

        // Switches -------------------
        bool isClicked;
        public bool isModified; // Модифицировался ли файл
        public bool isSaved; // Имеет ли файл имя
        public bool isMoving = false;
        public bool addSelection = false;
        public bool figModification = false; // Фигура модифицируется

        // Switches -------------------

        public List<Figure> listFigure;
        public List<Figure> listFigureCopy; // Copy
        Figure myFigure;

        public string saveFileName;
        public Size pictureSize;

        public BufferedGraphics bufferedGraphics;
        public BufferedGraphicsContext bufferedGraphicsContext;

        
        public Point mouseDownPoint = new Point();

        

        public Form2(Size pictureSize)
        {
            InitializeComponent();
            listFigure = new List<Figure>();
            listFigureCopy = new List<Figure>(); // Copy

            this.pictureSize = pictureSize;
            this.Size = pictureSize;

            // Создается первый Rectangle для белого фона и сетки
            myFigure = new Rect(new Point(0, 0), (Point)pictureSize, Color.White, 1F, Color.White, true);
            listFigure.Add(myFigure);

            this.AutoScrollMinSize = pictureSize;

            saveFileName = null;
            isSaved = false;
            isModified = false;
            isClicked = false;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            bufferedGraphicsContext = BufferedGraphicsManager.Current;
            bufferedGraphicsContext.MaximumBuffer = SystemInformation.PrimaryMonitorMaximizedWindowSize;
            bufferedGraphics = bufferedGraphicsContext.Allocate(this.CreateGraphics(), this.DisplayRectangle);
        }

        private void Form2_MouseDown(object sender, MouseEventArgs e)
        {
            g = CreateGraphics();
            isClicked = true;

            mouseDownPoint = e.Location;

            if ( ((Form1)MdiParent).selectSwitch )
            {
                // Если щелчок над выделенной фигурой то устанавливается переключатель перемещения isMoving
                for (int i = 1; i < listFigure.Count; i++) // if click is on top of any selected figure
                {
                    if (listFigure[i].isSelected)
                    {
                        if (listFigure[i].rectangle.IntersectsWith(new Rectangle(Point.Subtract(e.Location, (Size)AutoScrollPosition), new Size(0,0))))
                        {
                            // Клик над любой из выделенных фигур
                            isMoving = true;
                            break;
                        }
                    }
                }

                if (isMoving)
                // Клонирование
                {
                    listFigureCopy.Clear();
                    foreach (var item in listFigure)
                    {
                        listFigureCopy.Add((Figure)item.Clone());
                    }
                }
                // Клик в любом другом месте кроме выделенных фигур
                // Создание прямоугольника выделения
                else
                {
                    myFigure = new Rect(Point.Subtract(e.Location, (Size)AutoScrollPosition),
                        Point.Subtract(e.Location, (Size)AutoScrollPosition),
                        Color.Black, 1F, ((Form1)ParentForm).solidBrushColor);

                    for (int i = 1; i < listFigure.Count; i++)
                    {
                        if (listFigure[i].rectangle.IntersectsWith(new Rectangle(Point.Subtract(e.Location, (Size)AutoScrollPosition), new Size(0, 0))))
                        {
                            // Флаг addSelection значит если позиция мыши над любой
                            // не выделенной фигурой значит выделение продолжается
                            // в методе mouseMove
                            // и добавляется к предыдущему выделению
                            addSelection = true;
                            break;
                        }
                    }
                    if (addSelection)
                    {
                        addSelection = false;
                    }
                    // А иначе сброс всех предыдущих выделений
                    // но выделение продолжается в mouseMove
                    // то есть выделение продолжается но не добавляется к предыдущему
                    else
                    {
                        for (int i = 1; i < listFigure.Count; i++) 
                        {
                            listFigure[i].isSelected = false;
                            // Так же сброс флага модификации
                            listFigure[i].isModifingSwitch = false;
                        }
                        figModification = false;
                    }
                }
            }
            else
            {
                // если включена сетка изменить координаты
                int meshSize = ((Form1)MdiParent).MeshSize;
                if ( ((Form1)MdiParent).MeshOn == true )
                {
                    truePosition.X = (truePosition.X / meshSize) * meshSize;
                    truePosition.Y = (truePosition.Y / meshSize) * meshSize;
                }

                if (((Form1)this.MdiParent).selectedFigure == 0)
                {
                    myFigure = new Rect(truePosition, truePosition,
                        ((Form1)ParentForm).penColor, ((Form1)ParentForm).lineWidth, ((Form1)ParentForm).solidBrushColor);
                }
                else if (((Form1)this.MdiParent).selectedFigure == 1)
                {
                    myFigure = new _Ellipse(truePosition, truePosition,
                        ((Form1)ParentForm).penColor, ((Form1)ParentForm).lineWidth, ((Form1)ParentForm).solidBrushColor);
                }
                else if (((Form1)this.MdiParent).selectedFigure == 2)
                {
                    myFigure = new _Line(truePosition, truePosition,
                        ((Form1)ParentForm).penColor, ((Form1)ParentForm).lineWidth);
                }
                else if (((Form1)this.MdiParent).selectedFigure == 3)
                {
                    myFigure = new _Curve(truePosition, truePosition,
                        ((Form1)ParentForm).penColor, ((Form1)ParentForm).lineWidth);
                }
                else if (((Form1)this.MdiParent).selectedFigure == 4)
                {
                    myFigure = new _Text(truePosition, truePosition,
                        ((Form1)ParentForm).penColor, ((Form1)ParentForm).drawTextFont);
                }

                myFigure.isFilled = ((Form1)MdiParent).isFilled;
            }
        }

        Point truePosition;
        Figure curFig;
        private void Form2_MouseMove(object sender, MouseEventArgs e)
        {
            // истиная позиция обновляется всегда
            truePosition = Point.Subtract(e.Location, (Size)AutoScrollPosition);
            if (isClicked)
            { 
                if (((Form1)MdiParent).selectSwitch)
                {
                    if (isMoving)
                    {
                        // Прорисовка копии listFigure
                        bufferedGraphics.Render();
                        for (int i = 1; i < listFigureCopy.Count; i++)
                        {
                            if (listFigureCopy[i].isSelected)
                            {
                                // Передается разница в виде вектора между текущей позицией и предыдущей
                                // Пока mouseDownPoint хранит предыдущую позуцию
                                listFigureCopy[i].MouseMove(Point.Subtract(e.Location, (Size)mouseDownPoint));
                                // И прорисовка с новой позицией
                                listFigureCopy[i].DrawDash(g, (Size)AutoScrollPosition);
                            }
                        }

                        // Глобальная переменная сохраняет последнюю позицию курсора
                        mouseDownPoint = e.Location;
                    }
                    else
                    {
                        // Если фигура(ы) не передвигаются то прорисовка прямоугольника выделения
                        bufferedGraphics.Render();
                        myFigure.setPoint2(truePosition);
                        myFigure.DrawDash(g, (Size)AutoScrollPosition);
                    }
                }
                else
                {
                    // если включена сетка изменить координаты
                    int meshSize = ((Form1)MdiParent).MeshSize;
                    if (((Form1)MdiParent).MeshOn == true)
                    {
                        truePosition.X = (truePosition.X / meshSize) * meshSize;
                        truePosition.Y = (truePosition.Y / meshSize) * meshSize;
                    }

                    bufferedGraphics.Render();
                    myFigure.setPoint2(truePosition);
                    // Изначальная прорисовка
                    myFigure.DrawDash(g, (Size)AutoScrollPosition);
                }
            }
            // Если установлен флаг модификации для всего класса, то 
            if (figModification)
            {
                Console.WriteLine("mouse move mod");
                Rectangle zeroRect = new Rectangle(e.Location, new Size(0, 0));
                
                for (int i = 0; i < 4; i++)
                {
                    if (curFig.rArray[i].IntersectsWith(zeroRect))
                    {
                        Console.WriteLine("Cursor Hand");
                        Cursor.Current = Cursors.Hand;
                        return;
                    }
                }
                Cursor.Current = Cursors.Default;
            }

            ((Form1)this.MdiParent).sb_coordinates.Text = truePosition.ToString();
        }


        private void Form2_MouseUp(object sender, MouseEventArgs e)
        {
            if (((Form1)MdiParent).selectSwitch)
            {
                if (isMoving)
                {
                    // Проверка если после перемещения фигура вышла за рамки
                    bool isMoved = true;
                    foreach (var item in listFigureCopy)
                    {
                        if (!isInsideDrawingArea(item))
                        {
                            isMoved = false;
                            break;
                        }
                    }
                    // Если не вышла, то копирование обратно в listFigure
                    if (isMoved)
                    {
                        listFigure.Clear();
                        foreach (var item in listFigureCopy)
                        {
                            listFigure.Add((Figure)item.Clone());
                        }
                    }
                }
                else
                {
                    // Если стоит selectSwitch и если не было передвижения, то проверяет
                    // пересакется ли прямоугольник выделения с фигурами и добавляет их к выделенным
                    // в том числе если прямоугольник выделения размером (0, 0)
                    for (int i = 1; i < listFigure.Count; i++) 
                    {
                        if (listFigure[i].rectangle.IntersectsWith(myFigure.rectangle))
                        {
                            // Однако если фигура модифицируется то нет
                            if (figModification == false)
                            {
                                listFigure[i].isSelected = true;
                            }
                        }
                    }
                }

            }
            else
            {
                if (isInsideDrawingArea(myFigure))
                {
                    if ((((Form1)MdiParent).selectedFigure == 4)) // Text
                    {
                        TextInputForm1 textInputForm1 = new TextInputForm1();
                        if (textInputForm1.ShowDialog() == DialogResult.OK)
                        {
                            ((_Text)myFigure).text = textInputForm1.textBox1.Text;
                            listFigure.Add(myFigure);
                            isModified = true;
                        }
                    }
                    else
                    {
                        listFigure.Add(myFigure);
                        isModified = true;
                    }
                }
            }

            isClicked = false;
            isMoving = false;  // set Moving back

            g.Dispose();
            Invalidate();
        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            if (isModified)
            {
                ((Form1)MdiParent).statusBarPanel1_isModified.Text = "*";
            }
            else
            {
                ((Form1)MdiParent).statusBarPanel1_isModified.Text = " ";
            }

            // Сетка
            int meshSize = ((Form1)MdiParent).MeshSize;
            if (((Form1)MdiParent).MeshOn == true)
            {
                Console.WriteLine("mesh draw");
                listFigure[0].DrawWithMesh(bufferedGraphics.Graphics, (Size)AutoScrollPosition, meshSize);
            }
            else
            {
                listFigure[0].DrawSolid(bufferedGraphics.Graphics, (Size)AutoScrollPosition);
            }

            // Прорисовка в bufferedGraphics
            Figure item;
            for (int i = 1; i < listFigure.Count; i++)
            {
                item = listFigure[i];

                if (item.isFilled)
                {
                    item.DrawSolid(bufferedGraphics.Graphics, (Size)AutoScrollPosition);
                }
                if (item.isSelected)
                {
                    item.DrawDash(bufferedGraphics.Graphics, (Size)AutoScrollPosition);
                }
                if (!item.isSelected)
                {
                    item.Draw(bufferedGraphics.Graphics, (Size)AutoScrollPosition);
                }
                if (item.isModifingSwitch == true)
                {
                    item.DrawModify(bufferedGraphics.Graphics, (Size)AutoScrollPosition);
                }
            }

            bufferedGraphics.Render(e.Graphics);
        }



        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isModified)
            {
                DialogResult dr = MessageBox.Show("Save modified document?", "Document has been modified", MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Yes)
                {
                    if (!((Form1)this.ParentForm).saveToFile(this))
                    {
                        e.Cancel = true;
                    }
                }
                else if (dr == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            bufferedGraphics.Dispose();
        }

        private void Form2_Scroll(object sender, ScrollEventArgs e)
        {
            
        }

        bool isInsideDrawingArea(Figure figure)
        {
            if (figure.rectangle.Right > pictureSize.Width | figure.rectangle.Bottom > pictureSize.Height)
            {
                return false;
            }
            return true;
        }

        private void Form2_Activated(object sender, EventArgs e)
        {
            ((Form1)this.MdiParent).sb_pictureSize.Text = pictureSize.ToString();
        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                ((Form1)MdiParent).toolStripButtonDelete_Click(this, new EventArgs());
            }
        }

        private void Form2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (((Form1)MdiParent).selectSwitch == true)
            {
                Point truePosition = Point.Subtract(e.Location, (Size)AutoScrollPosition);
                Rectangle r = new Rectangle(truePosition, new Size(0, 0));

                // Сброс всех предыдущих флагов модификации и isSelected
                for (int i = 1; i < listFigure.Count; i++)
                {
                    listFigure[i].isModifingSwitch = false;
                    listFigure[i].isSelected = false;
                }
                // 
                figModification = false;
                isMoving = false;
                curFig = null;

                // Поиск фигуры над которой был двойной щелчок
                bool test = false;
                for (int i = 1; i < listFigure.Count; i++)
                {
                    test = r.IntersectsWith(listFigure[i].rectangle);
                    if (test)
                    {
                        // Устанавливаем флаг модификации фигуры
                        listFigure[i].isModifingSwitch = true;
                        // curFig текущая фигура для модификации
                        curFig = listFigure[i];
                        // Общий флаг модификации для класса
                        figModification = true;
                        break;
                    }
                }
            }
            // Вызывается метод Paint
            Invalidate();
        }
    }
}
