using System;
using System.Drawing;

namespace DM.Geo
{
    public struct DMRectangle
    {
        public DMRectangle(DMRectangle rc) { leftTop = new Coord(rc.leftTop); rightBottom = new Coord(rc.rightBottom); }
        public DMRectangle(RectangleF rc) { leftTop = new Coord(rc.Location); rightBottom = new Coord(rc.Right, rc.Bottom); }
        Coord leftTop;
        Coord rightBottom;
        public Coord LeftTop { get { return leftTop; } set { leftTop = value; } }
        public Coord RightBottom { get { return rightBottom; } set { rightBottom = value; } }
        public Coord LeftBottom { get { return new Coord(Left, Bottom); } }
        public Coord RightTop { get { return new Coord(Right, Top); } }
        public RectangleF RF { get { return new RectangleF((float)Left, (float)Top, (float)Width, (float)Height); } }
        public Rectangle RC { get { return DM.Utils.Graph.Rect.Translate(RF); } }
        public bool IsEqual(DMRectangle rc){return this.LeftTop.IsEqual(rc.LeftTop) && this.RightBottom.IsEqual(rc.RightBottom);}
        public DMRectangle(double l, double t, double w, double h)
        {
            leftTop = new Coord();
            rightBottom = new Coord();
            leftTop.X = l; leftTop.Y = t;
            rightBottom.X = l+w; rightBottom.Y = t+h;
        }
        public DMRectangle(float l, float t, float w, float h)
        {
            leftTop = new Coord();
            rightBottom = new Coord();
            leftTop.XF = l; leftTop.YF = t;
            rightBottom.XF = l+w; rightBottom.YF = t+h;
        }
        public double Left { get { return leftTop.X; } set { leftTop.X = value; } }
        public double Top { get { return leftTop.Y; } set { leftTop.Y = value; } }
        public double Right { get { return rightBottom.X; } set { rightBottom.X = value; } }
        public double Bottom { get { return rightBottom.Y; } set { rightBottom.Y = value; } }

        public double Width { get { return Math.Abs(rightBottom.X - leftTop.X); } }
        public double Height { get { return Math.Abs(rightBottom.Y - leftTop.Y); } }

        public void Normalize()
        {
            double l, t, r, b;
            l = Math.Min(leftTop.X, rightBottom.X);
            t = Math.Min(leftTop.Y, rightBottom.Y);
            r = Math.Max(leftTop.X, rightBottom.X);
            b = Math.Max(leftTop.Y, rightBottom.Y);
            leftTop.X = l;
            leftTop.Y = t;
            rightBottom.X = r;
            rightBottom.Y = b;
        }

        public void Offset(double x, double y)
        {
            leftTop.X += x;
            leftTop.Y += y;
            rightBottom.X += x;
            rightBottom.Y += y;
        }
        public void Offset(float x, float y) { Offset((double)x, (double)y); }
        public void Offset(Coord pt) { Offset(pt.X, pt.Y); }
        public void Offset(Point pt) { Offset(pt.X, pt.Y); }
        public void Offset(PointF pt) { Offset(pt.X, pt.Y); }
        public override string ToString()
        {
            return string.Format("X={0:0.0},Y={1:0.0},Width={2:0.0},Height={3:0.0}", leftTop.X, leftTop.Y, Width, Height);
        }
        public Coord Center
        {
            get
            {
                return new Coord(leftTop.X + Width / 2, LeftTop.Y + Height / 2);
            }
        }
        public bool Contains(PointF pt)
        {
            return pt.X >= this.Left && pt.X <= this.Right && pt.Y >= this.Top && pt.Y <= this.Bottom;
        }
    }
}
