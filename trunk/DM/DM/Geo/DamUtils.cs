using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DM.Geo
{
    public static class DamUtils
    {
        private const double ZOOM = 1;
        public static List<Coord> RelativeCoords(List<Coord> pts, ref Coord origin)
        {
            if (pts.Count == 0)
                return null;
            List<Coord> newpts = new List<Coord>();
            double minx = pts[0].X, miny = pts[0].Y;
            foreach (Coord p in pts)
            {
                minx = Math.Min(minx, p.X);
                miny = Math.Min(miny, p.Y);
            }
            foreach (Coord p in pts)
            {
                newpts.Add(new Coord(ZOOM * (p.X - minx), ZOOM * (p.Y - miny)));
            }
            origin.X = ZOOM * minx;
            origin.Y = ZOOM * miny;
            return newpts;
        }

        public static DMRectangle MinBoundary(List<Coord> pts)
        {
            if (pts.Count == 0)
                return new DMRectangle();
            List<Coord> copy = new List<Coord>(pts);
            double l, t, r, b;
            copy.Sort(Coord.XGreater);
            l = copy.First().X;
            r = copy.Last().X;
            copy.Sort(Coord.YGreater);
            t = copy.First().Y;
            b = copy.Last().Y;

            return new DMRectangle(l, t, r-l, b-t);
        }
        public static double Degree2Radian(double degree)
        {
            return Math.PI * degree / 180;
        }
        public static double Radian2Degree(double radian)
        {
            return radian * 180 / Math.PI;
        }
//         public static Coord RotateDegree(Coord pt, double theta)
//         {
//             theta %= 360;
//             theta = Degree2Radian(theta);
//             return RotateRadian(pt, theta);
//         }
//         public static PointF RotateDegree(PointF pt, PointF at, double degree)
//         {
//             Coord c = new Coord(pt);
//             Coord cat = new Coord(at);
//             c = RotateRadian(c, cat, Degree2Radian(degree));
//             return c.PF;
//         }
        public static Coord RotateDegree(Coord pt, Coord at, double degree)
        {
            return RotateRadian(pt, at, Degree2Radian(degree));
        }
        public static Coord RotateRadian(Coord pt, Coord at, double theta)
        {
            double cos = Math.Cos(theta);
            double sin = Math.Sin(theta);

            double x = pt.X;
            double y = pt.Y;
            x -= at.X;
            y -= at.Y;

            double xnew = x * cos - y * sin;
            double ynew = y * cos + x * sin;

            xnew += at.X;
            ynew += at.Y;

            Coord newpt = new Coord(xnew, ynew);
            return newpt;
        }
        public static List<Coord> RotateDegree(List<Coord> pts, Coord at, double theta)
        {
            theta %= 360;
            if (theta == 0.00)
                return pts;

            theta = Degree2Radian(theta);
            List<Coord> newpts = new List<Coord>();
            foreach (Coord pt in pts)
            {
                newpts.Add(RotateRadian(pt, at, theta));
            }
            return newpts;
        }
        public static List<PointF> TranslatePoints(List<Coord> pts)
        {
            List<PointF> lst = new List<PointF>();
            foreach (Coord c in pts)
            {
                lst.Add(c.PF);
            }
            return lst;
        }
        public static PointF[] TranslatePoints(List<Coord3D> lst)
        {
            List<PointF> pts = new List<PointF>();
            foreach (Coord3D c in lst)
            {
                pts.Add(c.Plane.PF);
            }
            return pts.ToArray();
        }
        public static Coord CenterPoint(DMRectangle rc)
        {
            return new Coord(rc.Left + rc.Width / 2, rc.Top + rc.Height / 2);
        }
        public static PointF[] Translate(List<Coord3D> lst)
        {
            PointF[] res = new PointF[lst.Count];
            for (int i = 0; i < lst.Count; i++ )
            {
                res[i] = lst[i].Plane.PF;
            }
            return res;
        }
    }
}
