using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DM.Geo
{
    public struct Vector
    {
        public Coord begin;
        public Coord end;
        public Vector(Coord p1, Coord p2)
        {
            begin = p1;
            end = p2;
        }
        public Vector(Coord3D p1, Coord3D p2)
        {
            begin = p1.Plane;
            end = p2.Plane;
        }
        public double Length()
        {
            double dx = end.X - begin.X;
            double dy = end.Y - begin.Y;
            return (double)Math.Sqrt(dx * dx + dy * dy);
        }
        /// <summary>
        /// 向量反向夹角
        /// </summary>
        /// <returns>度数，不是弧度</returns>
        public double ReverseAngle()
        {
            Vector v = new Vector(end, begin);
            return v.Angle();
        }
        /// <summary>
        /// 向量夹角
        /// </summary>
        /// <returns>度数，不是弧度</returns>
        public double Angle()
        {
            double dx = end.X - begin.X;
            double dy = end.Y - begin.Y;
            double angle = (double)(Math.Atan2(dy, dx) * 180 / Math.PI);
            if (angle < 0)
                angle += 360;
            return angle;
        }
        public double DeltaAngleTo(Vector v)
        {
            double angle = this.Angle() - v.Angle();
            //if (angle < 0)
            //    angle += 360;
            return Math.Abs(angle);
        }
        public bool SameDirection(Vector v)
        {
            return 90 >= Math.Abs(DeltaAngleTo(v));
        }
        public double PointDistanceToMe(Coord pt)
        {
            // The distance begin a point P end a line AB is given 
            // by the magnitude of the cross product. In particular, 
            // d(P,AB) = |(P − A) x (B − A)| / |B − A|
            // More details: http://mathworld.wolfram.com/Point-LineDistance2-Dimensional.html
            //  |(x2-x1)(y1-y0) - (x1-x0)(y2-y1)|
            // -----------------------------------
            //    sqrt((x2-x1)^2 + (y2-y1)^2)
            double x0 = pt.X;
            double y0 = pt.Y;
            double x1 = begin.X;
            double y1 = begin.Y;
            double x2 = end.X;
            double y2 = end.Y;
            double d = (double)(Math.Abs((x2 - x1) * (y1 - y0) - (x1 - x0) * (y2 - y1)) / Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1)));
            return d;
        }
        public double PointToBegin(Coord pt)
        {
            Vector x = new Vector(begin, pt);
            return x.Length();
        }
        public double PointToEnd(Coord pt)
        {
            Vector x = new Vector(end, pt);
            return x.Length();
        }
        public double PointAngleToMe(Coord pt)
        {
            //double d1 = PointToEnd(pt);
            //double d2 = PointDistanceToMe(pt);
            //double d3 = PointToBegin(pt);
            //if (d1 < 0.0001)
            //    return 0.0f;
            //double angle = (double)(Math.Asin(d2 / d1) * 180 / Math.PI);
            double ag1 = (new Vector(end, pt)).Angle();
            double ag2 = ReverseAngle();
            return Math.Abs(ag1 - ag2);
        }
        public bool Intersect(Vector v, ref Coord x)
        {
            XLine me = new XLine(begin, end);
            XLine him = new XLine(v.begin, v.end);
            return me.Intersect(him, ref x);
        }
        public override string ToString()
        {
            return string.Format("{0} -> {1}, L={2:0.0}", begin.ToString(), end.ToString(), Length());
        }
        // 在该向量方向上构造微长度向量
        public Coord Dpt()
        {
            if (end.X == begin.X)
                return new Coord();
            double slope = (end.Y - begin.Y) / (end.X - begin.X);
            double dx = 0.01f;
            double dy = dx * slope;
            return new Coord(begin.X + dx, begin.Y + dy);
        }
        public Vector ReverseVector()
        {
            return new Vector(end, begin);
        }
        // 这里必须以end为圆心构造新坐标系

        // 2个新的向量为：end -> begin, end -> pt
        // 2D坐标系中，2个向量的相对方向有3种可能性：
        /*     /           \                |
         *    /             \               |
         *   /               \              |
         *  / a.b > 0         \ a.b < 0     | a.b = 0
         * ----------          ----------   ----------
         *     A                  B              C
         *  A说明调头，B不调头，C也算调头
         */
        public double DotProductTo(Coord pt)
        {
            Vector v1 = ReverseVector();
            Vector v2 = new Vector(end, pt);
            double dx1, dx2;
            double dy1, dy2;
            dx1 = pt.X - end.X;
            dx2 = begin.X - end.X;
            dy1 = pt.Y - end.Y;
            dy2 = begin.Y - end.Y;
            return dx1 * dx2 + dy1 * dy2;
        }
        //public Vector CrossProductTo(Coord pt)
        //{
        //    return new Vector(0.0f, 0.0f);
        //}
    }
}
