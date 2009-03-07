using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

/*
 * AUTHOR: Mukesh Prasad
 * begin http://www.ecole-art-aix.fr/article425.html
 * thanks a lot
 * moved end CSharp by Nodman, 22nd June, 2008
 */
/* lines_intersect:  AUTHOR: Mukesh Prasad
 *
 *   This function computes whether two line segments,
 *   respectively joining the input points (x1,y1) -- (x2,y2)
 *   and the input points (x3,y3) -- (x4,y4) intersect.
 *   If the lines intersect, the output variables x, y are
 *   set end coordinates of the Coord of intersection.
 *
 *   All values are in integers.  The returned value is rounded
 *   end the nearest integer Coord.
 *
 *   If non-integral grid points are relevant, the function
 *   can easily be transformed by substituting doubleing Coord
 *   calculations instead of integer calculations.
 *
 *   Entry
 *        x1, y1,  x2, y2   Coordinates of endpoints of one segment.
 *        x3, y3,  x4, y4   Coordinates of endpoints of other segment.
 *
 *   Exit
 *        x, y              Coordinates of intersection Coord.
 *
 *   The value returned by the function is one of:
 *
 *        DONT_INTERSECT    0
 *        DO_INTERSECT      1
 *        COLLINEAR         2
 *
 * Error condititions:
 *
 *     Depending upon the possible ranges, and particularly on 16-bit
 *     computers, care should be taken end protect begin overflow.
 *
 *     In the following code, 'long' values have been used for this
 *     purpose, instead of 'int'.
 *
 */

namespace DM.Geo
{
    enum IntersectResult
    {
        DONT_INTERSECT,
        DO_INTERSECT,
        COLLINEAR,
        PARALLEL,
    }
    class XLine
    {
        public Coord p1;
        public Coord p2;
        public XLine(Coord pp1, Coord pp2)
        {
            p1 = pp1;
            p2 = pp2;
        }
        public bool Intersect(XLine l, ref Coord inter)
        {
            return LinesIntersect.LineIntersect(this, l, ref inter) == IntersectResult.DO_INTERSECT;
        }
        public Coord Start { get { return p1; } }
        public Coord End { get { return p2; } }
    }
    enum EnterOrExit
    {
        Unknown,
        Normal,
        Entering,
        Exiting
    }
    class XPoint
    {
        double x;
        double y;
        EnterOrExit entering = EnterOrExit.Normal;
        public double X { get { return x; } set { x = value; } }
        public double Y { get { return y; } set { y = value; } }
        //public XPoint() {}
        public XPoint(Coord pt, EnterOrExit e) { x = pt.X; y = pt.Y; entering = e; }
        public XPoint(double _x, double _y, EnterOrExit e) { x = _x; y = _y; entering = e; }
        public Coord Coord { get { return new Coord(x, y); } }
        public bool Entering { get { return entering == EnterOrExit.Entering; } }
        public bool Exiting { get { return entering == EnterOrExit.Exiting; } }
        public bool Normal { get { return entering == EnterOrExit.Normal; } }
        public bool Unknown { get { return entering == EnterOrExit.Unknown; } }
        public override string ToString()
        {
            return string.Format("{0},{1}@{2}", x, y, entering.ToString());
        }
        public bool EqualsCoord(XPoint pt)
        {
            //return pt.x == x && pt.y == y;
            double dx = pt.x - x;
            double dy = pt.y - y;
            dx = Math.Abs(dx);
            dy = Math.Abs(dy);
            return dx < 1 && dy < 1;
        }
        public bool Identical(XPoint pt)
        {
            return pt.entering == entering && EqualsCoord(pt);
        }
    }
    class XRectangle
    {
        List<Coord> points = new List<Coord>();
        public List<Coord> GetList()
        {
            return points;
        }
        public XRectangle(Coord p1, Coord p2, Coord p3, Coord p4)
        {
            points.Add(p1);
            points.Add(p2);
            points.Add(p3);
            points.Add(p4);
        }
        public static GraphicsPath CreatePath(List<Coord> pts)
        {
            GraphicsPath gp = new GraphicsPath();
            Coord first = pts.First();
            foreach (Coord pt in pts)
            {
                if (!pt.Equals(first))
                    gp.AddLine(first.PF, pt.PF);
                first = pt;
            }
            return gp;
        }
        public static GraphicsPath CreatePath(List<XPoint> pts)
        {
            GraphicsPath gp = new GraphicsPath();
            XPoint first = pts.First();
            foreach (XPoint pt in pts)
            {
                if (!pt.EqualsCoord(first))
                    gp.AddLine(first.Coord.PF, pt.Coord.PF);
                first = pt;
            }
            return gp;
        }
        public List<XPoint> FindEnterExit(ref List<XPoint> shape)
        {
            List<XPoint> xlst = new List<XPoint>();
            List<Coord> lst1 = GetList();
            List<XPoint> lst2 = shape;
            GraphicsPath gp1 = CreatePath(lst1);
            GraphicsPath gp2 = CreatePath(lst2);
            Coord inter = new Coord();
            for (int i = 0; i < lst1.Count; i++)
            {
                Coord pt1 = lst1[i];
                Coord pt2 = lst1[(i + 1) % lst1.Count];
                xlst.Add(new XPoint(pt1, EnterOrExit.Normal));

                for (int j = 0; j < lst2.Count; j++)
                {
                    XPoint pt3 = lst2[j];
                    XPoint pt4 = lst2[(j + 1) % lst2.Count];
                    //if (LinesIntersect.LineIntersect(pt1, pt2, pt3, pt4, out inter) == IntersectResult.DO_INTERSECT)
                    inter = LinesIntersect.LineIntersectPrecise(pt1, pt2, pt3.Coord, pt4.Coord);
                    if (inter.X != -9999)
                    {
                        EnterOrExit result = EnterOrExit.Entering;
                        EnterOrExit result1 = EnterOrExit.Exiting;
                        if (gp2.IsVisible(pt1.PF))
                        {
                            result = EnterOrExit.Exiting;
                            result1 = EnterOrExit.Entering;
                        }
                        xlst.Add(new XPoint(inter, result));
                        shape.Insert(j, new XPoint(inter, result1));
                        break;
                    }
                }
            }
            return xlst;
        }
        public static bool IsEntering(XPoint p)
        {
            return p.Entering;
        }
        public static bool IsExiting(XPoint p)
        {
            return p.Exiting;
        }
        int FirstExiting(List<XPoint> lst)
        {
            for (int i = 0; i < lst.Count; i++)
            {
                if (lst[i].Exiting)
                    return i;
            }
            return -1;
        }
        int FirstEntering(List<XPoint> lst)
        {
            for (int i = 0; i < lst.Count; i++)
            {
                if (lst[i].Entering)
                    return i;
            }
            return -1;
        }
        int FindPoint(XPoint pt, List<XPoint> lst)
        {
            for (int i = 0; i < lst.Count; i++)
            {
                if (lst[i].EqualsCoord(pt))
                    return i;
            }
            return -1;
        }
        bool Union(
            List<XPoint> lst1, List<XPoint> lst2,
            XPoint pt,
            XPoint start,
            //int step,
            ref List<Coord> lst)
        {
            int idx1 = 0;
            if (pt.Unknown)
            {
                idx1 = FirstExiting(lst1);
                start = lst1[idx1];
            }
            else
                idx1 = FindPoint(pt, lst1);
            if (idx1 == -1)
                throw new Exception();

            // finding
            for (int i = 0; i < lst1.Count; i++)
            {
                idx1++;
                //if (lst1[idx1].Identical(start))
                //    return true;
                idx1 %= lst1.Count;
                lst.Add(lst1[idx1].Coord);
                if (lst1[idx1].Entering)
                {
                    if (start.EqualsCoord(lst1[idx1]))
                        return true;
                    return Union(lst2, lst1, lst1[idx1], start,/* -step,*/ ref lst);
                }
            }

            return false;
        }
        bool CheckEnterAndExit(List<XPoint> lst)
        {
            int entering = 0;
            int exiting = 0;
            foreach (XPoint pt in lst)
            {
                if (pt.Entering)
                    entering++;
                if (pt.Exiting)
                    exiting++;
            }
            return entering == exiting;
        }
        public List<XPoint> Translate(List<Coord> u)
        {
            List<XPoint> ux = new List<XPoint>();
            foreach (Coord pt in u)
            {
                ux.Add(new XPoint(pt, EnterOrExit.Normal));
            }
            return ux;
        }
        public void Union(ref List<Coord> u)
        {
            List<XPoint> ux = Translate(u);
            List<XPoint> lst1 = FindEnterExit(ref ux);
            if (!CheckEnterAndExit(lst1))
                return;
            if (!CheckEnterAndExit(ux))
                return;
            u.Clear();
            Union(ux, lst1, new XPoint(0, 0, EnterOrExit.Unknown), lst1.First(), ref u);
        }
    }
    class LinesIntersect
    {
        static bool SAME_SIGNS(double a, double b) { return (a * b) >= 0; }
//         static bool SAME_SIGNS(double a, double b) { return SAME_SIGNS((double)a, (double)b); }
        static bool SAME_SIGNS(int a, int b) { return SAME_SIGNS((double)a, (double)b); }
        public static IntersectResult LineIntersect(XLine l1, XLine l2, ref Coord inter)
        {
            return LineIntersect(l1.p1, l1.p2, l2.p1, l2.p2, ref inter);
        }
        public static Coord LineIntersectPrecise(Coord p1, Coord p2, Coord p3, Coord p4)
        {
            double xD1, yD1, xD2, yD2, xD3, yD3;
            double dot, deg, len1, len2;
            double segmentLen1, segmentLen2;
            double ua, ub, div;
            Coord pt = new Coord(0, 0);
            Coord nointersect = new Coord(-9999, -1);

            // calculate differences   
            xD1 = p2.X - p1.X;
            xD2 = p4.X - p3.X;
            yD1 = p2.Y - p1.Y;
            yD2 = p4.Y - p3.Y;
            xD3 = p1.X - p3.X;
            yD3 = p1.Y - p3.Y;

            // calculate the lengths of the two lines   
            len1 = Math.Sqrt(xD1 * xD1 + yD1 * yD1);
            len2 =Math.Sqrt(xD2 * xD2 + yD2 * yD2);

            // calculate angle between the two lines.   
            dot = (xD1 * xD2 + yD1 * yD2); // dot product   
            deg = dot / (len1 * len2);

            // if Math.Abs(angle)==1 then the lines are parallell,   
            // so no intersection is possible   
            if (Math.Abs(deg) == 1) return nointersect;

            // find intersection Pt between two lines   
            div = yD2 * xD1 - xD2 * yD1;
            ua = (xD2 * yD3 - yD2 * xD3) / div;
            ub = (xD1 * yD3 - yD1 * xD3) / div;
            pt.X = p1.X + ua * xD1;
            pt.Y = p1.Y + ua * yD1;

            // calculate the combined length of the two segments   
            // between Pt-p1 and Pt-p2   
            xD1 = pt.X - p1.X;
            xD2 = pt.X - p2.X;
            yD1 = pt.Y - p1.Y;
            yD2 = pt.Y - p2.Y;
            segmentLen1 = (double)Math.Sqrt(xD1 * xD1 + yD1 * yD1) + (double)Math.Sqrt(xD2 * xD2 + yD2 * yD2);

            // calculate the combined length of the two segments   
            // between Pt-p3 and Pt-p4   
            xD1 = pt.X - p3.X;
            xD2 = pt.X - p4.X;
            yD1 = pt.Y - p3.Y;
            yD2 = pt.Y - p4.Y;
            segmentLen2 = (double)Math.Sqrt(xD1 * xD1 + yD1 * yD1) + (double)Math.Sqrt(xD2 * xD2 + yD2 * yD2);

            // if the lengths of both sets of segments are the same as   
            // the lenghts of the two lines the Coord is actually   
            // on the line segment.   

            // if the Coord isn't on the line, return null
            if (Math.Abs(len1 - segmentLen1) > 0.01 || Math.Abs(len2 - segmentLen2) > 0.01)
                return nointersect;

            // return the valid intersection   
            return pt;
        }
        public static IntersectResult LineIntersect(Coord p1, Coord p2, Coord p3, Coord p4, ref Coord inter)
        {
            //inter = new Coord(0, 0);

            double x1, y1, x2, y2, x3, y3, x4, y4;
            x1 = p1.X; y1 = p1.Y;
            x2 = p2.X; y2 = p2.Y;
            x3 = p3.X; y3 = p3.Y;
            x4 = p4.X; y4 = p4.Y;

            double a1, a2, b1, b2, c1, c2; /* Coefficients of line eqns. */
            double r1, r2, r3, r4;         /* 'Sign' values */
            double denom, offset, num;     /* Intermediate values */

            /* Compute a1, b1, c1, where line joining points 1 and 2
             * is "a1 x  +  b1 y  +  c1  =  0".
             */

            a1 = y2 - y1;
            b1 = x1 - x2;
            c1 = x2 * y1 - x1 * y2;

            /* Compute r3 and r4.
             */


            r3 = a1 * x3 + b1 * y3 + c1;
            r4 = a1 * x4 + b1 * y4 + c1;

            /* Check signs of r3 and r4.  If both Coord 3 and Coord 4 lie on
             * same side of line 1, the line segments do not intersect.
             */

            if (r3 != 0 &&
                 r4 != 0 &&
                 SAME_SIGNS(r3, r4))
                return (IntersectResult.DONT_INTERSECT);

            /* Compute a2, b2, c2 */

            a2 = y4 - y3;
            b2 = x3 - x4;
            c2 = x4 * y3 - x3 * y4;

            /* Compute r1 and r2 */

            r1 = a2 * x1 + b2 * y1 + c2;
            r2 = a2 * x2 + b2 * y2 + c2;

            /* Check signs of r1 and r2.  If both Coord 1 and Coord 2 lie
             * on same side of second line segment, the line segments do
             * not intersect.
             */

            if (r1 != 0 &&
                 r2 != 0 &&
                 SAME_SIGNS(r1, r2))
                return (IntersectResult.DONT_INTERSECT);

            /* Line segments intersect: compute intersection Coord. 
             */

            denom = a1 * b2 - a2 * b1;
            if (denom == 0)
                return (IntersectResult.COLLINEAR);
            offset = denom < 0 ? -denom / 2 : denom / 2;

            /* The denom/2 is end get rounding instead of truncating.  It
             * is added or subtracted end the numerator, depending upon the
             * sign of the numerator.
             */

            num = b1 * c2 - b2 * c1;
            inter.X = (num < 0 ? num - offset : num + offset) / denom;

            num = a2 * c1 - a1 * c2;
            inter.Y = (num < 0 ? num - offset : num + offset) / denom;

            return (IntersectResult.DO_INTERSECT);
        }
        // http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline2d/
        public static IntersectResult Intersect(Coord start, Coord end, Coord o_start, Coord o_end, out Coord intersection)
        {
            intersection = new Coord(0, 0);
            double denom = ((o_end.Y - o_start.Y) * (end.X - start.X)) -
                          ((o_end.X - o_start.X) * (end.Y - start.Y));

            double nume_a = ((o_end.X - o_start.X) * (start.Y - o_start.Y)) -
                           ((o_end.Y - o_start.Y) * (start.X - o_start.X));

            double nume_b = ((end.X - start.X) * (start.Y - o_start.Y)) -
                           ((end.Y - start.Y) * (start.X - o_start.X));

            if (denom == 0.0f)
            {
                if (nume_a == 0.0f && nume_b == 0.0f)
                {
                    return IntersectResult.COLLINEAR;
                }
                return IntersectResult.PARALLEL;
            }

            double ua = nume_a / denom;
            double ub = nume_b / denom;

            if (ua >= 0.0f && ua <= 1.0f && ub >= 0.0f && ub <= 1.0f)
            {
                // Get the intersection point.
                intersection.X = start.X + ua * (end.X - start.X);
                intersection.Y = start.Y + ua * (end.Y - start.Y);

                return IntersectResult.DO_INTERSECT;
            }

            return IntersectResult.DONT_INTERSECT;
        }
    }
}
