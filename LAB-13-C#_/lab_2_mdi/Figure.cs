using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace imageeditor
{
    [Serializable()]
    public abstract class Figure
    {
        // Флаг выделения
        [NonSerialized] public bool isSelected;
		public bool WTF = false; // WTF is this variable for?
        public bool FuckYou = false; // Fuck You !!
        public string FuckOff = "Fuck Off";
        // Флаг модификации
        [NonSerialized] public bool isModifingSwitch = false;

        public Point p1;
        public Point p2;
        public Rectangle localRectangle;
        public Point localP1, localP2;

        protected Color penColor;
        protected float penWidth;

        public Rectangle rectangle;
        public bool isFilled;

        // Конструкторы
        public Figure(Point p1, Point p2, Color penColor, float penWidth)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.penColor = penColor;
            this.penWidth = penWidth;
            setRectangle();
            isSelected = false;
        }
        
        public Figure(Point p1, Point p2, Color penColor)
            : this(p1, p2, penColor, 1F) { }

        public Figure()
            : this(new Point(), new Point(), Color.Black) { }

        // Абстрактные методы

        public abstract void Draw(Graphics g, Size scrollPosition);
        public abstract void Draw(Graphics g, Color penColor, Size scrollPosition);
        public abstract void DrawSolid(Graphics g, Size scrollPosition);
        public abstract void DrawDash(Graphics g, Size scrollPosition);
        public abstract void Hide(Graphics g, Size scrollPosition);


        // Виртуальные методы

        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        public virtual void setPoint1(Point p)
        {
            p1 = p;
            setRectangle();
        }

        public virtual void setPoint2(Point p)
        {
            p2 = p;
            setRectangle();
        }

        public virtual void MouseMove( Point offset )
        {
            rectangle.Location = Point.Add(rectangle.Location, (Size)offset);
        }

        public virtual void DrawWithMesh(Graphics g, Size scrollPosition, int meshSize) { }

        // создать listSquresMod для квадратов модификации
        public List<Figure> listSquaresMod = new List<Figure>();
        public virtual void DrawModify(Graphics g, Size scrollPosition)
        {

            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;

            Pen p = new Pen(Color.Red);
            p.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
            g.DrawRectangle(p, localRectangle);

            // Вычисление позиции для квадратов
            listSquaresMod.Clear();
            Point p1mod_lt = new Point(rectangle.Location.X - 5,
                rectangle.Location.Y - 5);
            Point p1mod_br = new Point(rectangle.Location.X + 5,
                rectangle.Location.Y + 5);

            Point p2mod_lt = new Point(rectangle.Location.X + rectangle.Width - 5,
                rectangle.Location.Y - 5);
            Point p2mod_br = new Point(rectangle.Location.X + rectangle.Width + 5,
                rectangle.Location.Y + 5);

            Point p3mod_lt = new Point(rectangle.Location.X - 5,
                 rectangle.Location.Y + rectangle.Height - 5);
            Point p3mod_br = new Point(rectangle.Location.X + 5,
                rectangle.Location.Y + rectangle.Height + 5);

            Point p4mod_lt = new Point(rectangle.Location.X + rectangle.Width - 5,
                rectangle.Location.Y + rectangle.Height - 5);
            Point p4mod_br = new Point(rectangle.Location.X + rectangle.Width + 5,
                rectangle.Location.Y + rectangle.Height + 5);

            listSquaresMod.Add(new Rect(p1mod_lt, p1mod_br, Color.Black, 1F, Color.Green, true));
            listSquaresMod.Add(new Rect(p2mod_lt, p2mod_br, Color.Black, 1F, Color.YellowGreen, true));
            listSquaresMod.Add(new Rect(p3mod_lt, p3mod_br, Color.Black, 1F, Color.DarkOliveGreen, true));
            listSquaresMod.Add(new Rect(p4mod_lt, p4mod_br, Color.Black, 1F, Color.ForestGreen, true));

            foreach (var item in listSquaresMod)
            {
                item.DrawSolid(g, scrollPosition);
            }
        }


        // Методы абстрактного класса

        // Создает Rectangle из 2х точек p1 и p2
        // Хранящие глобальные координаты
        Point loc = new Point();
        Size size = new Size();
        public void setRectangle()
        {
            loc.X = Math.Min(p1.X, p2.X);
            loc.Y = Math.Min(p1.Y, p2.Y);
            size.Width = Math.Abs(p1.X - p2.X);
            size.Height = Math.Abs(p1.Y - p2.Y);
            rectangle.Location = loc;
            rectangle.Size = size;
        }


    }

    [Serializable()]
    public class Rect : Figure
    {       
        Color solidBrushColor = Color.White;

        public Rect(Point p1, Point p2, Color penColor, float penWidth, Color solidBrushColor)
            : base (p1, p2, penColor, penWidth)
        {
            this.solidBrushColor = solidBrushColor;
        }

        public Rect(Point p1, Point p2, Color penColor, float penWidth, Color solidBrushColor, bool isFilled)
            : this(p1, p2, penColor, penWidth, solidBrushColor)
        {
            this.isFilled = isFilled;
        }

        public override void Draw(Graphics g, Size scrollPosition)
        {
            Pen penSolid = new Pen(penColor, penWidth);
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            g.DrawRectangle(penSolid, localRectangle);
        }
        public override void Draw(Graphics g, Color penColor, Size scrollPosition)
        {
            Pen penSolid = new Pen(penColor, penWidth);
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            g.DrawRectangle(penSolid, localRectangle);
        }
        public override void DrawSolid(Graphics g, Size scrollPosition)
        {
            Pen penSolid = new Pen(penColor, penWidth);
            Brush solidBrush = new SolidBrush(solidBrushColor);
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            g.FillRectangle(solidBrush, localRectangle);
        }
        public override void DrawDash(Graphics g, Size scrollPosition)
        {



            Pen penDashed = new Pen(penColor, penWidth);
            penDashed.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            g.DrawRectangle(penDashed, localRectangle);
        }
        // Отрисовка фигуры с сеткой
        public override void DrawWithMesh(Graphics g, Size scrollPosition, int meshSize)
        {
            DrawSolid(g, scrollPosition);
            int x = 0;
            Point p1, p2;
            // Vertical Lines
            while (x < rectangle.Size.Width)
            {
                p1 = new Point(x, 0);
                p2 = new Point(x, localRectangle.Height);
                g.DrawLine(Pens.Black, p1, p2);
                x += meshSize;
            }
            x = 0;
            // Horizontal Lines
            int y = 0;
            while (y < rectangle.Height)
            {
                p1 = new Point(0, y);
                p2 = new Point(localRectangle.Width, y);
                g.DrawLine(Pens.Black, p1, p2);
                y += meshSize;
            }
            y = 0;
        }
        public override void Hide(Graphics g, Size scrollPosition)
        {
            throw new NotImplementedException();
        }

    }

    [Serializable()]
    public class _Ellipse : Figure
    {
        Color solidBrushColor = Color.White;

        public _Ellipse(Point p1, Point p2, Color penColor, float penWidth, Color solidBrushColor)
            : base (p1, p2, penColor, penWidth)
        {
            this.solidBrushColor = solidBrushColor;
        }
        public _Ellipse(Point p1, Point p2, Color penColor, float penWidth, Color solidBrushColor, bool isFilled)
            : this(p1, p2, penColor, penWidth, solidBrushColor)
        {
            this.isFilled = isFilled;
        }
        public override void Draw(Graphics g, Size scrollPosition)
        {
            Pen penSolid = new Pen(penColor, penWidth);
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            g.DrawEllipse(penSolid, localRectangle);
        }
        public override void Draw(Graphics g, Color penColor, Size scrollPosition)
        {
            Pen penSolid = new Pen(penColor, penWidth);
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            g.DrawEllipse(penSolid, localRectangle);
        }
        public override void DrawSolid(Graphics g, Size scrollPosition)
        {
            Pen penSolid = new Pen(penColor, penWidth);
            Brush solidBrush = new SolidBrush(solidBrushColor);
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            g.FillEllipse(solidBrush, localRectangle);
        }
        public override void DrawDash(Graphics g, Size scrollPosition)
        {
            Pen penDashed = new Pen(penColor, penWidth);
            penDashed.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            g.DrawEllipse(penDashed, localRectangle);
        }
        public override void Hide(Graphics g, Size scrollPosition)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable()]
    public class _Line : Figure
    {
        public _Line(Point p1, Point p2, Color penColor, float penWidth)
            : base(p1, p2, penColor, penWidth) { }
        public override void Draw(Graphics g, Size scrollPosition)
        {
            Pen penSolid = new Pen(penColor, penWidth);
            localP1 = Point.Add(p1, scrollPosition);
            localP2 = Point.Add(p2, scrollPosition);
            g.DrawLine(penSolid, localP1, localP2);

        }
        public override void Draw(Graphics g, Color penColor, Size scrollPosition)
        {
            Pen penSolid = new Pen(penColor, penWidth);
            localP1 = Point.Add(p1, scrollPosition);
            localP2 = Point.Add(p2, scrollPosition);
            g.DrawLine(penSolid, localP1, localP2);

        }
        public override void DrawSolid(Graphics g, Size scrollPosition)
        {
            // no solid line
        }
        public override void DrawDash(Graphics g, Size scrollPosition)
        {
            Pen penDashed = new Pen(penColor, penWidth);
            penDashed.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            localP1 = Point.Add(p1, scrollPosition);
            localP2 = Point.Add(p2, scrollPosition);
            g.DrawLine(penDashed, localP1, localP2);
        }
        public override void Hide(Graphics g, Size scrollPosition)
        {
            throw new NotImplementedException();
        }
        public override void MouseMove(Point offset)
        {
            base.MouseMove(offset);
            p1 = Point.Add(p1, (Size)offset);
            p2 = Point.Add(p2, (Size)offset);
        }
    }

    [Serializable()]
    class _Curve : Figure
    {
        public List<Point> curvePointList;
        List<Point> localCurvePointList;

        public _Curve(Point p1, Point p2, Color penColor, float penWidth)
            : base(p1, p2, penColor, penWidth)
        {
            curvePointList = new List<Point>();
            curvePointList.Add(p1);
            curvePointList.Add(p2);
            localCurvePointList = new List<Point>();      
        }

        public override void setPoint2(Point p)
        {
            base.setPoint2(p);
            curvePointList.Add(p);
        }

        public override void Draw(Graphics g, Size scrollPosition)
        {
            Pen penSolid = new Pen(penColor, penWidth);
            localCurvePointList.Clear();
            foreach (var item in curvePointList)
            {
                localCurvePointList.Add(Point.Add(item, scrollPosition));
            }
            g.DrawCurve(penSolid, localCurvePointList.ToArray());
            // test
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            g.DrawRectangle(Pens.Red, localRectangle);
            //
        }
        public override void Draw(Graphics g, Color penColor, Size scrollPosition)
        {
            Pen penSolid = new Pen(penColor, penWidth);
            localCurvePointList.Clear();
            foreach (var item in curvePointList)
            {
                localCurvePointList.Add(Point.Add(item, scrollPosition));
            }
            g.DrawCurve(penSolid, localCurvePointList.ToArray());
        }
        public override void DrawSolid(Graphics g, Size scrollPosition)
        {
            // no solid
        }
        public override void DrawDash(Graphics g, Size scrollPosition)
        {
            Pen penDashed = new Pen(penColor, penWidth);
            penDashed.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            localCurvePointList.Clear();
            foreach (var item in curvePointList)
            {
                localCurvePointList.Add(Point.Add(item, scrollPosition));
            }
            g.DrawCurve(penDashed, localCurvePointList.ToArray());
        }
        public override void Hide(Graphics g, Size scrollPosition)
        {
            
        }
        public override void MouseMove(Point offset)
        {
            base.MouseMove(offset);
            for (int i = 0; i < curvePointList.Count; i++)
            {
                curvePointList[i] = Point.Add(curvePointList[i], (Size)offset);
            }
        }

        //public override object Clone()
        //{
        //    _Curve newCurve = (_Curve)MemberwiseClone();
        //    newCurve.curvePointList.Clear();
        //    foreach (var item in curvePointList)
        //    {
        //        newCurve.curvePointList.Add(item);
        //    }
        //    return newCurve;
        //}
    }

    [Serializable()]
    class _Text : Figure
    {
        public string text;
        public Font font;

        public _Text(Point p1, Point p2, Color penColor)
            : this(p1, p2, penColor, new Font(FontFamily.GenericSansSerif, 8F)) { }

        public _Text(Point p1, Point p2, Color penColor, Font font)
            : base(p1, p2, penColor)
        {
            this.font = font;
        }

        public override void Draw(Graphics g, Color penColor, Size scrollPosition)
        {
            
        }
        public override void Draw(Graphics g, Size scrollPosition)
        {
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            Brush solidBrush = new SolidBrush(penColor);
            g.DrawString(text, font, solidBrush, localRectangle);
        }
        public override void DrawDash(Graphics g, Size scrollPosition)
        {
            Pen penDashed = new Pen(Color.Black, 1F);
            penDashed.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            localRectangle.Location = Point.Add(rectangle.Location, scrollPosition);
            localRectangle.Size = rectangle.Size;
            Brush solidBrush = new SolidBrush(penColor);
            g.DrawRectangle(penDashed, localRectangle); // Draw rectangle Black for text position
            g.DrawString(text, font, solidBrush, localRectangle);


        }
        public override void DrawSolid(Graphics g, Size scrollPosition)
        {
            
        }
        public override void Hide(Graphics g, Size scrollPosition)
        {
            throw new NotImplementedException();
        }
    }
}
