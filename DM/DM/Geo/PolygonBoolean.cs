using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DM.Geo
{
    public static class PolygonUtils
    {
        public static double AreaOfPolygon(List<Coord> vertex)
        {
            if (vertex == null)
                return 0;
            // A = 0.5 * Sigma[i=0, N-1] { (Xi + Xi+1)( XiYi+1 - Xi+1Yi) }
            int i, j;
            double area = 0;

            for (i = 0; i < vertex.Count; i++)
            {
                j = (i + 1) % vertex.Count;
                area += vertex[i].X * vertex[j].Y;
                area -= vertex[i].Y * vertex[j].X;
            }

            area /= 2;
            return (area < 0 ? -area : area);
        }
        public static Coord CentroidOfPolygon(List<Coord> vertex)
        {
            double area = AreaOfPolygon(vertex);
            if (area < 0.0001)
                return new Coord();
            int i, j;
            double x = 0, y = 0;
            for (i = 0; i < vertex.Count; i++)
            {
                j = (i + 1) % vertex.Count;
                x += (vertex[i].X + vertex[j].X) * (vertex[i].X * vertex[j].Y - vertex[j].X * vertex[i].Y);
                y += (vertex[i].Y + vertex[j].Y) * (vertex[i].X * vertex[j].Y - vertex[j].X * vertex[i].Y);
            }
            x /= 6 * area;
            y /= 6 * area;
            return new Coord(x, y);
        }
    }
    public class BorderPoint
        {
            public enum PointAttribute
            {
                Unknown = 0,
                Connection = 1,     // 连接点

                HVertex = 2,        // 水平方向顶点
                VVertex = 4,
                Transition = 8,   // 过渡点

                Downing = 0x100,
                Upping = 0x200
            }
            int x = 0;
            int y = 0;
            PointAttribute attr = PointAttribute.Connection;
            void Set(int _x, int _y) { x = _x; y = _y; }
            void Set(double _x, double _y) { Set((int)_x, (int)_y); }
//             void Set(double _x, double _y) { Set((int)_x, (int)_y); }
            public BorderPoint(int _x, int _y) { Set(_x, _y); }
            public BorderPoint(double _x, double _y) { Set(_x, _y); }
//             public BorderPoint(double _x, double _y) { Set(_x, _y); }
            public int X { get { return x; } set { x = value; } }
            public int Y { get { return y; } set { y = value; } }

            public Point Point() { return new Point(x, y); }
            public Coord Coord() { return new Coord(x, y); }
            public BorderPoint(Point pt) { Set(pt.X, pt.Y); }
            public BorderPoint(Coord pt) { Set(pt.X, pt.Y); }
            public override string ToString()
            {
                return string.Format("{0},{1}:{2}", x, y, attr.ToString());
            }
            public bool Equals(BorderPoint pt)
            {
                return (x == pt.x && y == pt.y);
            }
            public bool IsAdjacent(BorderPoint pt)
            {
                int delta_x = Math.Abs(x - pt.x);
                int delta_y = Math.Abs(y - pt.y);

                return delta_x <= 1 && delta_y <= 1;
            }
            public bool GetAttribute(PointAttribute a)
            {
                return (0 != (attr & a));
            }
            public void SetAttribute(PointAttribute a, bool value)
            {
                if (value)
                    attr |= a;
                else
                    attr &= ~a;
            }
            public bool Connection
            {
                get { return GetAttribute(PointAttribute.Connection); }
                set { if (value) Transition = false; SetAttribute(PointAttribute.Connection, value); }
            }
            public bool Transition
            {
                get { return GetAttribute(PointAttribute.Transition); }
                set { if (value) Connection = false; SetAttribute(PointAttribute.Transition, value); }
            }
            public bool HVertex
            {
                get { return GetAttribute(PointAttribute.HVertex); }
                set { VVertex = false; SetAttribute(PointAttribute.HVertex, value); }
            }
            public bool VVertex
            {
                get { return GetAttribute(PointAttribute.VVertex); }
                set { HVertex = false; SetAttribute(PointAttribute.VVertex, value); }
            }
            public bool Downing
            {
                get { return GetAttribute(PointAttribute.Downing); }
                set { Upping = false; SetAttribute(PointAttribute.Downing, value); }
            }
            public bool Upping
            {
                get { return GetAttribute(PointAttribute.Upping); }
                set { Downing = false; SetAttribute(PointAttribute.Upping, value); }
            }
        }
        internal class BorderRectangle
        {
            BorderPoint left = new BorderPoint(0, 0);
            BorderPoint right = new BorderPoint(0, 0);
            public BorderRectangle() { }
            public int Left { get { return left.X; } set { left.X = value; } }
            public int Top { get { return left.Y; } set { left.Y = value; } }
            public int Right { get { return right.X; } set { right.X = value; } }
            public int Bottom { get { return right.Y; } set { right.Y = value; } }
            public int Width { get { return right.X - left.X; } }
            public int Height { get { return right.Y - left.Y; } }
            public bool IsPtInside(BorderPoint pt)
            {
                bool result = true;
                result &= (pt.X >= Left && pt.X <= Right);
                result &= (pt.Y >= Top && pt.Y <= Bottom);
                return result;
            }
        }

        public class BorderShapeII : ICloneable
        {
            private bool isClosed = false;
            private List<Coord> borderII = new List<Coord>();
            private static int CycleIndex(int idx, int count)
            {
                return (idx + count) % count;
            }
            private static int FindBeginWith(List<Vector> lv, Coord pt)
            {
                int idx = 0;
                foreach (Vector v in lv)
                {
                    if (v.begin.Equals(pt))
                        return idx;
                    idx++;
                }
                return -1;
            }
            private static int FindEndWith(List<Vector> lv, Coord pt)
            {
                int idx = 0;
                foreach (Vector v in lv)
                {
                    if (v.end.Equals(pt))
                        return idx;
                    idx++;
                }
                return -1;
            }
            private static void TRACE(string fmt, params object[] pm)
            {
                System.Diagnostics.Debug.Print(fmt, pm);
            }
            private static void TRACE(string fmt)
            {
                System.Diagnostics.Debug.Print(fmt);
            }
            private static void SortX(ref List<Coord> pts, Vector v)
            {
                if (pts == null)
                    return;
                if (pts.Count == 0)
                    return;

                Coord pt = pts[0];
                pts.Sort(delegate(Coord pt1, Coord pt2)
                {
                    double delta = v.PointToBegin(pt1) - v.PointToBegin(pt2);
                    return Math.Sign(delta);
                });
                if (!pt.Equals(pts[0]))
                {
                    TRACE("Sorted List<Coord>");
                }
            }
            private bool AddPoint(Coord pt)
            {
                if (IsClosed)
                    return true;
                if (borderII.Count != 0)
                {
                    if (pt.Equals(borderII[0]))
                    {
                        //borderII.SetVertex(pt);
                        isClosed = true;
                        return true;
                    }
                }
                if (!IsPtExists(pt))
                    borderII.Add(pt);
                return false;
            }
            private bool AddPointNoClose(Coord pt)
            {
                if (!IsPtExists(pt))
                    borderII.Add(pt);
                else
                    return false;

                return true;
            }
            private bool IsPtExists(Coord pt)
            {
                return (borderII.IndexOf(pt) != -1);
            }
            private Vector Edge(int idx)
            {
                if (borderII.Count < 2)
                    return new Vector();
                int p1 = CycleIndex(idx, borderII.Count);
                int p0 = CycleIndex(idx - 1, borderII.Count);
                return new Vector(borderII[p0], borderII[p1]);
            }
            private List<Vector> VisibleEdgesIn(BorderShapeII q)
            {
                List<Vector> vs = new List<Vector>();
                List<Coord> pts = new List<Coord>();
                Coord x = new Coord();
                for (int i = 0; i < Count; i++)
                {
                    Vector pv = Edge(i + 1);
                    pts.Clear();
                    for (int j = 0; j < q.Count; j++)
                    {
                        Vector qv = q.Edge(j + 1);
                        if (pv.Intersect(qv, ref x))
                        {
                            pts.Add(x);
                        }
                    }
                    SortX(ref pts, pv);
                    if (q.IsInsideIII(pv.begin))
                        pts.Insert(0, pv.begin);
                    if (q.IsInsideIII(pv.end))
                        pts.Add(pv.end);
                    for (int j = 0; j < pts.Count - 1; j++)
                    {
                        int j1 = (j + 1 + pts.Count) % pts.Count;
                        vs.Add(new Vector(pts[j], pts[j1]));
                    }
                }

                return vs;
            }
            private BorderShapeII CombineEdges(List<Vector> v1, List<Vector> v2)
            {
                if (v1.Count == 0 && v2.Count == 0)
                    return null;

                Vector v;
                if (v1.Count != 0)
                    v = v1[0];
                else
                    v = v2[0];
                BorderShapeII x = new BorderShapeII();

                int idx = 0;
                for (int i = 0; i < v1.Count + v2.Count; i++)
                {
                    if (x.AddPoint(v.begin))
                        break;
                    idx = FindBeginWith(v1, v.end);
                    if (-1 == idx)
                    {
                        idx = FindBeginWith(v2, v.end);
                        if (-1 == idx)
                        {
                            return null;
                        }
                        else
                            v = v2[idx];
                    }
                    else
                        v = v1[idx];
                }
                return x;
            }

            public bool IsClosed { get { return isClosed; } }
            public int Count { get { return borderII.Count; } }
            public bool IsEmpty { get { return Count == 0; } }
            public List<Coord> Data { get { return borderII; } }
            public void Intersect(DMRectangle rc)
            {
                BorderShapeII r = new BorderShapeII(rc);
                Intersect(r);
            }
            public void Clear()
            {
                borderII.Clear();
                isClosed = false;
            }
            public object Clone()
            {
                BorderShapeII n = new BorderShapeII(borderII);
                n.isClosed = isClosed;
                return n;
            }
            public BorderShapeII()
            {
                borderII = new List<Coord>();
            }
            public BorderShapeII(List<Coord> pts)
            {
                if (pts.Count < 2)
                    return;
                borderII = new List<Coord>(pts);
                if (borderII.First().Equals(borderII.Last()))
                {
                    borderII.RemoveAt(borderII.Count-1);
                    isClosed = true;
                }
            }
            public BorderShapeII(DMRectangle rc)
            {
                AddPoint(rc.LeftTop);
                AddPoint(new Coord(rc.Left, rc.Top + rc.Height));
                AddPoint(new Coord(rc.Left + rc.Width, rc.Top + rc.Height));
                AddPoint(new Coord(rc.Left + rc.Width, rc.Top));
                AddPoint(rc.LeftTop);
            }
            // Joseph O'Rourke
            public bool IsInsideII(Coord q)
            {
                int i, i1;      /* point index; i1 = i-1 mod n */
                //int d;          /* dimension index */
                double x;          /* x intersection of e with ray */
                int Rcross = 0; /* number of right edge/ray crossings */
                int Lcross = 0; /* number of left edge/ray crossings */

                Coord[] P = borderII.ToArray();
                int n = P.Length;
                //printf("\n==>InPoly: q = "); PrintPoint(q); putchar('\n');

                /* Shift so that q is the origin. Note this destroys the polygon.
                   This is done for pedogical clarity. */
                for (i = 0; i < n; i++)
                {
                    //for (d = 0; d < DIM; d++)
                    //    P[i][d] = P[i][d] - q[d];
                    P[i].X -= q.X;
                    P[i].Y -= q.Y;
                }

                /* For each edge e=(i-1,i), see if crosses ray. */
                for (i = 0; i < n; i++)
                {
                    /* First see if q=(0,0) is a vertex. */
                    //if (P[i][X] == 0 && P[i][Y] == 0) return 'v';
                    if (P[i].X == 0 && P[i].Y == 0) return true;
                    i1 = (i + n - 1) % n;
                    /* printf("e=(%d,%d)\t", i1, i); */

                    /* if e "straddles" the x-axis... */
                    /* The commented-out statement is logically equivalent to the one 
                       following. */
                    /* if( ( ( P[i][Y] > 0 ) && ( P[i1][Y] <= 0 ) ) ||
                       ( ( P[i1][Y] > 0 ) && ( P[i] [Y] <= 0 ) ) ) { */

                    if ((P[i].Y > 0) != (P[i1].Y > 0))
                    {

                        /* e straddles ray, so compute intersection with ray. */
                        x = (P[i].X * (double)P[i1].Y - P[i1].X * (double)P[i].Y)
                      / (double)(P[i1].Y - P[i].Y);
                        /* printf("straddles: x = %g\t", x); */

                        /* crosses ray if strictly positive intersection. */
                        if (x > 0) Rcross++;
                    }
                    /* printf("Right cross=%d\t", Rcross); */

                    /* if e straddles the x-axis when reversed... */
                    /* if( ( ( P[i] [Y] < 0 ) && ( P[i1][Y] >= 0 ) ) ||
                       ( ( P[i1][Y] < 0 ) && ( P[i] [Y] >= 0 ) ) )  { */

                    if ((P[i].Y < 0) != (P[i1].Y < 0))
                    {

                        /* e straddles ray, so compute intersection with ray. */
                        x = (P[i].X * P[i1].Y - P[i1].X * P[i].Y)
                            / (double)(P[i1].Y - P[i].Y);
                        /* printf("straddles: x = %g\t", x); */

                        /* crosses ray if strictly positive intersection. */
                        if (x < 0) Lcross++;
                    }
                    /* printf("Left cross=%d\n", Lcross); */
                }

                /* q on the edge if left and right cross are not the same parity. */
                if ((Rcross % 2) != (Lcross % 2))
                    //    return 'e';
                    return false;

                /* q inside iff an odd number of crossings. */
                if ((Rcross % 2) == 1)
                    //return 'i';
                    return true;
                //else return 'o';
                else return false;
            }
            private double WhichSide(Coord P0, Coord P1, Coord P2)
            {
                return ((P1.X - P0.X) * (P2.Y - P0.Y)
                        - (P2.X - P0.X) * (P1.Y - P0.Y));
            }
            public bool IsInsideIII(Coord q)
            {
                int winding = 0;
                for (int i = 0; i < borderII.Count; i++)
                {
                    int i1 = CycleIndex(i + 1, borderII.Count);
                    Coord ptthis = borderII[i];
                    Coord ptnext = borderII[i1];
                    if (ptthis.Y <= q.Y)
                    {
                        if (ptnext.Y > q.Y)
                            if (WhichSide(ptthis, ptnext, q) > 0)
                                winding++;
                    }
                    else
                    {
                        if (ptnext.Y <= q.Y)
                            if (WhichSide(ptthis, ptnext, q) < 0)
                                winding--;
                    }
                }
                if (winding == 0)
                    return false;
                return true;
            }
            public void Intersect(BorderShapeII q)
            {
                List<Vector> v1 = this.VisibleEdgesIn(q);
                List<Vector> v2 = q.VisibleEdgesIn(this);
                BorderShapeII intersect = this.CombineEdges(v1, v2);
                if (intersect == null)
                {
                    q.Data.Reverse();
                    v1 = this.VisibleEdgesIn(q);
                    v2 = q.VisibleEdgesIn(this);
                    intersect = this.CombineEdges(v1, v2);
                    if( intersect == null )
                    {
                        Clear();
                        return;
                    }
                }
                if (intersect.Count <= 2)
                {
                    Clear();
                    return;
                }
                intersect.Data.Add(intersect.Data.First());
                borderII = intersect.borderII;
                isClosed = true;
                //borderII.Clear();
            }
        }
}
