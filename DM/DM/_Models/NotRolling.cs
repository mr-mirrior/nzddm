using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DM.Models
{
    public class NotRolling
    {
        Polygon vertex = new Polygon();

        public Polygon Vertex
        {
            get { return vertex; }
            set { vertex = value; }
        }
        Deck owner = null;

        public double Area
        {
            get { return vertex.ActualArea; }
        }
        public Deck Owner
        {
            get { return owner; }
            set { owner = value; }
        }
        string comment = "";
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        public NotRolling(){}
        public NotRolling(Deck dk) { Owner = dk; }
        public static List<NotRolling> ReadFromDB(Deck dk)
        {
            if (dk == null)
                return null;

            List<NotRolling> nrs = new List<NotRolling>();
            DB.SegmentDAO dao = DB.SegmentDAO.getInstance();
            string vtx, cmt;
            if (!dao.ReadSegmentNotRolling(dk.DeckInfo.BlockID, dk.DeckInfo.DesignZ, dk.DeckInfo.SegmentID, out vtx, out cmt))
            {
                return nrs;
            }
            // 解析
            return ParseFromString(dk, vtx, cmt);
        }
        private static List<NotRolling> ParseFromString(Deck dk, string vtx, string cmt)
        {
            List<NotRolling> nrs = new List<NotRolling>();
            vtx.Trim();
            string[] s0 = vtx.Split(new char[] { '|' });
            foreach (string batch in s0)
            {
                batch.Trim();
                string[] s1 = batch.Split(new char[] { ';' });
                if (s1.Length == 0)
                    return nrs;
                List<Geo.Coord> lst = new List<DM.Geo.Coord>();
                foreach (string s2 in s1)
                {
                    s2.Trim();
                    string[] s3 = s2.Split(new char[] { ',' });
                    if (s3.Length != 2)
                        continue;

                    float x, y;
                    if (float.TryParse(s3[0], out x) &&
                        float.TryParse(s3[1], out y))
                    {
                        lst.Add(new DM.Geo.Coord(x, y));
                    }
                }
                if (lst.Count != 0)
                {
                    NotRolling nr = new NotRolling(dk);
                    nr.SetVertex(lst);
                    nrs.Add(nr);
                }
            }

            s0 = cmt.Split(new char[] { '|' });
            for (int i = 0; i < s0.Length; i++ )
            {
                if (i < nrs.Count)
                    nrs[i].Comment = s0[i];
            }
            return nrs;
        }

        // lst中的坐标必须是大地坐标
        public void SetVertex(List<Geo.Coord> lst)
        {
            vertex.SetVertex(lst);
        }
        // 转换为屏幕坐标，内部保存
        public void CreateScreen()
        {
            if (owner != null && owner.Owner != null)
            {
                List<Geo.Coord> scrlst = new List<DM.Geo.Coord>();
                Layer l = owner.Owner;
                List<Geo.Coord> lst = vertex.Vertex;
                for (int i = 0; i < lst.Count; i++)
                {
                    Geo.Coord c = new DM.Geo.Coord(l.DamToScreen(lst[i]));
                    scrlst.Add(c);
                }
                vertex.SetScreenVertex(scrlst);
                Color fill, line;
                owner.Owner.Partition.PredefinedColor(out line, out fill);
                vertex.FillColor = fill;
            }
        }
        public void Draw(Graphics g, Font ft, bool text)
        {
            if (vertex != null)
            {
                vertex.Draw(g);
                g.ResetClip();
                StringFormat sf = new StringFormat(StringFormatFlags.NoClip | StringFormatFlags.NoWrap);
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                if( Comment!=null && text && (ft!=null) )
                {
                    SizeF sz = g.MeasureString(Comment, ft);
                    RectangleF rf = Vertex.ScreenBoundary.RF;
                    if( sz.Width > rf.Width )
                    {
                        rf.Offset(-(sz.Width-rf.Width) / 2, 0);
                        rf.Width = sz.Width;
                    }
                    if (sz.Height > rf.Height)
                    {
                        rf.Offset(0, -(sz.Height - rf.Height) / 2);
                        rf.Height = sz.Height;
                    }
                    Utils.Graph.OutGlow.DrawOutglowText(g, Comment, ft, rf, sf, Brushes.White, Brushes.Black);
                }
            }
        }
        public bool IsScreenVisible(PointF pt)
        {
            return vertex.IsScreenVisible(new Geo.Coord(pt));
        }
    }
}
