using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using DM.Geo;
using System.Xml.Serialization;

namespace DM.Models
{
    /// <summary>
    /// 多边形

    /// </summary>
    public class Polygon: IDisposable
    {
        public Polygon() { }
        public Polygon(List<Coord> vtx) { SetVertex(vtx); }
        public Polygon(DMRectangle rc)
        {
            Coord p1 = rc.LeftTop;
            Coord p2 = new Coord(rc.Right, rc.Top);
            Coord p3 = new Coord(rc.Right, rc.Bottom);
            Coord p4 = new Coord(rc.Left, rc.Bottom);
            List<Coord> lst = new List<Coord>();
            lst.Add(p1);
            lst.Add(p2);
            lst.Add(p3);
            lst.Add(p4);
            lst.Add(p1);
            SetVertex(lst);
        }
        // 施工坐标（真实坐标）
        List<Coord> vertex = new List<Coord>();
        public List<Coord> Vertex { get { return vertex; } set { vertex = value; } }
        public override string ToString()
        {
            return string.Format("Polygon Vertex={0}", vertex.Count);
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        DMRectangle boundary = new DMRectangle();

        public DMRectangle Boundary
        {
            get { return boundary; }
            set { boundary = value; }
        }
        public void SetVertex(List<Coord> vtx)
        {
            vertex = vtx;
            UpdateBoundary();
        }
        public void UpdateBoundary()
        {
            List<Coord> copy = new List<Coord>(vertex);
            if (copy.Count == 0) return;
            copy.Sort(Coord.XGreater);

            float xmin = copy.First().XF;
            float xmax = copy.Last().XF;

            copy.Sort(Coord.YGreater);
            float ymin = copy.First().YF;
            float ymax = copy.Last().YF;
            boundary = new DMRectangle(xmin, ymin, xmax - xmin, ymax - ymin);
        }

        // 实际面积 平方米

        public double ActualArea{get{return Geo.PolygonUtils.AreaOfPolygon(vertex);}}
        // 屏幕面积 像素数

        public double ScreenArea { get { return Geo.PolygonUtils.AreaOfPolygon(screenVertex); } }
        // 实际质心
        public Coord Centroid { get { return Geo.PolygonUtils.CentroidOfPolygon(vertex); } }
        // 屏幕质心
        public Coord ScreenCentroid { get { return Geo.PolygonUtils.CentroidOfPolygon(screenVertex); } }

        #region 画图相关
        Pen penLine = new Pen(Color.BlueViolet);
        SolidBrush brFill = new SolidBrush(Color.Lavender);
        bool antialias = true;
        bool inCurve = false;
        bool token = false;
        bool needClose = true;
        Geo.DMMatrix mtx;

        public Geo.DMMatrix DMMatrix
        {
            get { return mtx; }
            set { mtx = value; }
        }

        object syncLock = new object();
        [XmlIgnore]
        //public Pen Line {get { return penLine; } set { penLine = value; }}
        public Color LineColor { get { return penLine.Color; } set { lock (syncLock) penLine.Color = value; } }
        public float LineWidth { set { lock (syncLock) penLine.Width = value; } }
        public DashStyle LineDashStyle { set { lock (syncLock) penLine.DashStyle = value; } }
        public float[] LineDashPattern { set { lock (syncLock) penLine.DashPattern = value; } }
        [XmlIgnore]
        //public SolidBrush Fill { get { return brFill; } set { brFill = value; } }
        public Color FillColor { get { return brFill.Color; } set { lock (syncLock) brFill.Color = value; } }
        public bool AntiAlias { get { return antialias; } set { antialias = value; } }
        public bool InCurve { get { return inCurve; } set { inCurve = value; } }
        [XmlIgnore]
        public bool NeedClose { get { return needClose; } set { needClose = value; } }

        //[XmlIgnore]
        //public Geo.DMMatrix Matrix { get { return mtx; } set { mtx = new Geo.DMMatrix(value); } }
        [XmlIgnore]
        public bool Token { get { return token; } set { token = value; } }

        Partition partition;
        Elevation elevation;
        [XmlIgnore]
        public Partition Partition { get { return partition; } set { partition = value; } }
        [XmlIgnore]
        public Elevation Elevation { get { return elevation; } set { elevation = value; } }

        GraphicsPath gp = new GraphicsPath();
        List<Coord> screenVertex = new List<Coord>();
        
        DMRectangle screenBoundary;
        [XmlIgnore]
        public DMRectangle ScreenBoundary { get { return screenBoundary; } }
        public void CreateScreen(Geo.DMMatrix matrix)
        {
            mtx = matrix;
            CopyCoords();
            Relative();
            Scale();
            Rotate();
            //Offset();
            CalcVisible();
            CreatePath();
        }
        private void CopyCoords()
        {
            screenVertex = new List<Coord>(vertex);
        }
        private void Rotate()
        {
            /*screenVertex = new List<Coord>(vertex);*/
            screenVertex = Geo.DamUtils.RotateDegree(screenVertex, new Coord(0,0), mtx.degrees);

//             screenBoundary = Transform.DamUtils.MinBoundary(mtx.at);
//             screenOrigin = screenBoundary.LeftTop;
        }
        private void Relative()
        {
            for (int i = 0; i < screenVertex.Count; i++ )
            {
                screenVertex[i] = screenVertex[i].Origin(mtx.at);
            }
            mtx.offset = mtx.at;
        }
        private void Scale()
        {
            for (int i = 0; i < screenVertex.Count; i++)
            {
                screenVertex[i] = screenVertex[i].Scale(mtx.zoom);
            }
        }
//         private void Offset()
//         {
//             CalcVisible();
//         }
        public void OffsetGraphics(Coord c)
        {
            mtx.offset = c;
            for (int i = 0; i < screenVertex.Count; i++ )
            {
                screenVertex[i] = screenVertex[i].Offset(c);
            }
            CalcVisible();
            CreatePath();
        }
        private void CalcVisible()
        {
            screenBoundary = Geo.DamUtils.MinBoundary(screenVertex);
        }
        private void CreatePath()
        {
            gp.Reset();
            if (screenVertex.Count < 3 && NeedClose)
                return;
            List<PointF> lst = Geo.DamUtils.TranslatePoints(screenVertex);

            if (inCurve)
                gp.AddCurve(lst.ToArray());
            else
            {
                if (needClose)
                    gp.AddPolygon(lst.ToArray());
                else
                    gp.AddLines(lst.ToArray());
            }
//             if( needClose )
//                 gp.CloseAllFigures();
        }
        private void DrawTokenPoint(Graphics g, PointF pt)
        {
            g.FillEllipse(Brushes.OrangeRed, pt.X-2, pt.Y-2, 4, 4);
        }
        private void DrawToken(Graphics g)
        {
            foreach (Coord c in screenVertex)
            {
                DrawTokenPoint(g, c.PF);
            }
        }
        // 直接添加屏幕坐标，无转换
        public void SetScreenVertex(List<Coord> lc)
        {
            screenVertex = lc;
            CalcVisible();
            CreatePath();
        }
        public void OffsetScreen(Coord c)
        {
            for (int i = 0; i < screenVertex.Count; i++ )
            {
                screenVertex[i] = screenVertex[i].Offset(c);
            }
            CreatePath();
        }
        public Region SetDrawClip(Graphics g)
        {
            Region r = new Region(gp);
            Region old = g.Clip;
            g.Clip = r;
            return old;
        }
        public void Draw(Graphics g)
        {
//             SmoothingMode old = g.SmoothingMode;
//             if (antialias)
//                 g.SmoothingMode = SmoothingMode.AntiAlias;

//             if (!autoCenter)
//                 g.TranslateTransform((float)-ScreenBoundary.Left, (float)-ScreenBoundary.Top);

            // 填充多边形

            g.FillPath(brFill, gp);

//             // 描边
//             Pen penBack;
//             if( (penLine.DashStyle & DashStyle.Dash) != 0)
//             {
//                 penBack = new Pen(Color.White);
//                 if (penLine.Color == Color.White)
//                     penBack = new Pen(Color.Black);
//                 penBack.Width = penLine.Width;
//                 g.DrawPath(penBack, gp);
//             }
            // 
            g.DrawPath(penLine, gp);

            if (Token)
                DrawToken(g);

//             g.SmoothingMode = old;
        }
//         public void Offset(Coord c)
//         {
//             for (int i = 0; i < vertex.Count; i++ )
//             {
//                 vertex[i] = vertex[i].Offset(c);
//             }
//         }
        public Polygon CutByOfEarth(Polygon scrCut)
        {
            //PartitionDirectory p = new PartitionDirectory(@"C:\"+this.vertex);
            List<Coord> copy = new List<Coord>(this.Vertex);
            BorderShapeII shape = new BorderShapeII(copy);
            BorderShapeII cutShape = new BorderShapeII(scrCut.vertex);
            shape.Intersect(cutShape);
            //if (shape.IsEmpty)
            //{
            //    Utils.MB.Warning("你输入的仓面顶点坐标不全在该层范围内。请检查后重新输入！");
            //    return null;
            //}
            Polygon result = new Polygon();
            result.Vertex = shape.Data;

            return result;

/*            return cut;*/
        }

        public Polygon CutBy(Polygon scrCut)
        {
            List<Coord> copy = new List<Coord>(screenVertex);
            BorderShapeII shape = new BorderShapeII(copy);
            BorderShapeII cutShape = new BorderShapeII(scrCut.Vertex);
            shape.Intersect(cutShape);
            if (shape.IsEmpty)
                return null;
            Polygon result = new Polygon();
            result.Vertex = shape.Data;

            return result;

            /*            return cut;*/
        }

        public bool IsScreenVisible(Coord pt)
        {
            return gp.IsVisible(pt.PF);
        }
        #endregion
    }
}
