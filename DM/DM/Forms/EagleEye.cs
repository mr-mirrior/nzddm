using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace DM.Forms
{
    public partial class EagleEye : Form
    {
        #region - 形状 -
// 323.29,.07
// 374.41,10
// 434.76,10
// 434.36,12
// 374.22,12
// 323.72,2.19
// 323.29,.07
// 0,94.2
// 82,53.2
// 128.92,53.2
// 317.92,188.2
// 342.92,188.2
// 351.22,229.7
// 361.22,229.7
// 364.22,244.7
// 370.07,255.7
// 372.38,255.7
// 373.33,258.2
// 376.02,258.2
// 376.02,258.7
// 373.02,258.7
// 373.02,259.7
// 58.57,94.2
// 0,94.2
// 82,53.2
// 179.02,4.69
// 213.42,2.16
// 258.66,2.76
// 296.05,.15
// 305.28,0
// 324.12,94.2
// 186.32,94.2
// 128.92,53.2
// 82,53.2
// 186.32,94.2
// 324.12,94.2
// 342.92,188.2
// 317.92,188.2
// 186.32,94.2
// 305.28,0
// 315.28,0
// 361.22,229.7
// 351.22,229.7
// 305.28,0
// 315.28,0
// 323.29,.07
// 374.92,258.2
// 373.33,258.2
// 372.38,255.7
// 370.07,255.7
// 364.22,244.7
// 315.28,0
// 354.92,158.2
// 405.12,158.2
// 385.02,258.7
// 376.02,258.7
// 376.02,258.2
// 374.92,258.2
// 354.92,158.2
// 323.72,2.19
// 374.22,12
// 434.36,12
// 405.12,158.2
// 354.92,158.2
// 323.72,2.19
// 434.76,10
// 448.92,10
// 453.28,14.36
// 454.99,14.33
// 505.6,20.07
// 508.85,20.53
// 513.24,20.55
// 509.2,24.59
// 451.45,18.19
// 447.26,14
// 445.96,14
// 399.82,244.7
// 391.02,258.7
// 385.02,258.7
// 434.76,10
// 445.96,14
// 447.26,14
// 451.45,18.19
// 509.2,24.59
// 513.24,20.55
// 516.24,20.57
// 510.1,26.7
// 456.01,20.7
// 454.62,20.7
// 412.82,229.7
// 402.82,229.7
// 445.96,14
// 454.62,20.7
// 456.01,20.7
// 510.1,26.7
// 516.24,20.57
// 567.57,20.85
// 648.34,17.15
// 713.62,15.38
// 725.79,15.3
// 783.22,47.2
// 773.52,47.2
// 391.02,259.7
// 373.52,259.7
// 373.52,260.9
// 373.02,260.9
// 373.02,258.7
// 391.02,258.7
// 399.82,244.7
// 402.82,229.7
// 412.82,229.7
// 419.12,198.2
// 479.12,198.2
// 711.32,69.2
// 444.92,69.2
// 454.62,20.7
// 444.92,69.2
// 711.32,69.2
// 479.12,198.2
// 419.12,198.2
// 444.92,69.2


        Geo.Coord[] DATA = new Geo.Coord[]{
new Geo.Coord(-323.29,.07 ),
new Geo.Coord(-374.41,10 ),
new Geo.Coord(-434.76,10 ),
new Geo.Coord(-434.36,12 ),
new Geo.Coord(-374.22,12 ),
new Geo.Coord(-323.72,2.19 ),
new Geo.Coord(-323.29,.07 ),
new Geo.Coord(-0,94.2 ),
new Geo.Coord(-82,53.2),
new Geo.Coord(-128.92,53.2),
new Geo.Coord(-317.92,188.2),
new Geo.Coord(-342.92,188.2),
new Geo.Coord(-351.22,229.7),
new Geo.Coord(-361.22,229.7),
new Geo.Coord(-364.22,244.7),
new Geo.Coord(-370.07,255.7),
new Geo.Coord(-372.38,255.7),
new Geo.Coord(-373.33,258.2),
new Geo.Coord(-376.02,258.2),
new Geo.Coord(-376.02,258.7),
new Geo.Coord(-373.02,258.7),
new Geo.Coord(-373.02,259.7),
new Geo.Coord(-58.57,94.2),
new Geo.Coord(-0,94.2),
new Geo.Coord(-82,53.2),
new Geo.Coord(-179.02,4.69),
new Geo.Coord(-213.42,2.16),
new Geo.Coord(-258.66,2.76),
new Geo.Coord(-296.05,.15),
new Geo.Coord(-305.28,0),
new Geo.Coord(-324.12,94.2),
new Geo.Coord(-186.32,94.2),
new Geo.Coord(-128.92,53.2),
new Geo.Coord(-82,53.2),
new Geo.Coord(-186.32,94.2),
new Geo.Coord(-324.12,94.2),
new Geo.Coord(-342.92,188.2),
new Geo.Coord(-317.92,188.2),
new Geo.Coord(-186.32,94.2),
new Geo.Coord(-305.28,0),
new Geo.Coord(-315.28,0),
new Geo.Coord(-361.22,229.7),
new Geo.Coord(-351.22,229.7),
new Geo.Coord(-305.28,0),
new Geo.Coord(-315.28,0),
new Geo.Coord(-323.29,.07),
new Geo.Coord(-374.92,258.2),
new Geo.Coord(-373.33,258.2),
new Geo.Coord(-372.38,255.7),
new Geo.Coord(-370.07,255.7),
new Geo.Coord(-364.22,244.7),
new Geo.Coord(-315.28,0),
new Geo.Coord(-354.92,158.2),
new Geo.Coord(-405.12,158.2),
new Geo.Coord(-385.02,258.7),
new Geo.Coord(-376.02,258.7),
new Geo.Coord(-376.02,258.2),
new Geo.Coord(-374.92,258.2),
new Geo.Coord(-354.92,158.2),
new Geo.Coord(-323.72,2.19),
new Geo.Coord(-374.22,12),
new Geo.Coord(-434.36,12),
new Geo.Coord(-405.12,158.2),
new Geo.Coord(-354.92,158.2),
new Geo.Coord(-323.72,2.19),
new Geo.Coord(-434.76,10),
new Geo.Coord(-448.92,10),
new Geo.Coord(-453.28,14.36),
new Geo.Coord(-454.99,14.33),
new Geo.Coord(-505.6,20.07),
new Geo.Coord(-508.85,20.53),
new Geo.Coord(-513.24,20.55),
new Geo.Coord(-509.2,24.59),
new Geo.Coord(-451.45,18.19),
new Geo.Coord(-447.26,14),
new Geo.Coord(-445.96,14),
new Geo.Coord(-399.82,244.7),
new Geo.Coord(-391.02,258.7),
new Geo.Coord(-385.02,258.7),
new Geo.Coord(-434.76,10),
new Geo.Coord(-445.96,14),
new Geo.Coord(-447.26,14),
new Geo.Coord(-451.45,18.19),
new Geo.Coord(-509.2,24.59),
new Geo.Coord(-513.24,20.55),
new Geo.Coord(-516.24,20.57),
new Geo.Coord(-510.1,26.7),
new Geo.Coord(-456.01,20.7),
new Geo.Coord(-454.62,20.7),
new Geo.Coord(-412.82,229.7),
new Geo.Coord(-402.82,229.7),
new Geo.Coord(-445.96,14),
new Geo.Coord(-454.62,20.7),
new Geo.Coord(-456.01,20.7),
new Geo.Coord(-510.1,26.7),
new Geo.Coord(-516.24,20.57),
new Geo.Coord(-567.57,20.85),
new Geo.Coord(-648.34,17.15),
new Geo.Coord(-713.62,15.38),
new Geo.Coord(-725.79,15.3),
new Geo.Coord(-783.22,47.2),
new Geo.Coord(-773.52,47.2),
new Geo.Coord(-391.02,259.7),
new Geo.Coord(-373.52,259.7),
new Geo.Coord(-373.52,260.9),
new Geo.Coord(-373.02,260.9),
new Geo.Coord(-373.02,258.7),
new Geo.Coord(-391.02,258.7),
new Geo.Coord(-399.82,244.7),
new Geo.Coord(-402.82,229.7),
new Geo.Coord(-412.82,229.7),
new Geo.Coord(-419.12,198.2),
new Geo.Coord(-479.12,198.2),
new Geo.Coord(-711.32,69.2),
new Geo.Coord(-444.92,69.2),
new Geo.Coord(-454.62,20.7),
new Geo.Coord(-444.92,69.2),
new Geo.Coord(-711.32,69.2),
new Geo.Coord(-479.12,198.2),
new Geo.Coord(-419.12,198.2),
new Geo.Coord(-444.92,69.2)
};
        /*
                case "EU":
                    fill = Color.FromArgb(0xb4, 0x3d, 0x00);
                    line = Color.Black;
                    break;
                case "FU":
                case "FD":
                case "ED":
                    fill = Color.FromArgb(0xeb, 0xda, 0xc6);
                    line = Color.BlueViolet;
                    break;
                case "RU1":
                case "RD1":
                    fill = Color.FromArgb(0x6e, 0x6e, 0x6e);
                    line = Color.Black;
                    break;
                case "RU2":
                case "RD2":
                    fill = Color.FromArgb(0x9d, 0x9b, 0x9c);
                    line = Color.Black;
                    break;
                default:
                    fill = Color.FromArgb(0xff, 0xff, 0xcc);
                    line = Color.BlueViolet;
                    break;
         */
        Color[] colors = new Color[]{
            Color.FromArgb(0xff, 0xff, 0xcc),// EJ
            Color.FromArgb(0x6e, 0x6e, 0x6e),// RU1
            Color.FromArgb(0x9d, 0x9b, 0x9c),// RU2
            Color.FromArgb(0xff, 0xff, 0xcc),// RU4
            Color.FromArgb(0xff, 0xff, 0xcc),// RU3
            Color.FromArgb(0xeb, 0xda, 0xc6),// FU
            Color.FromArgb(0xb4, 0x3d, 0x00),// EU
            Color.FromArgb(0xeb, 0xda, 0xc6),// ED
            Color.FromArgb(0xeb, 0xda, 0xc6),// FD
            Color.FromArgb(0xff, 0xff, 0xcc),// RD3
            Color.FromArgb(0x6e, 0x6e, 0x6e),// RD1
            Color.FromArgb(0x9d, 0x9b, 0x9c)// RD2
        };
        #endregion
        public EagleEye()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;

            Geo.DMMatrix mtx = new DM.Geo.DMMatrix();
            mtx.zoom = 1;
            mtx.degrees = 180;

            List<Geo.Coord> lst = new List<DM.Geo.Coord>();
            Geo.Coord first = DATA[0];
            lst.Add(first);
            for (int i = 1; i < DATA.Length; i++ )
            {
                lst.Add(DATA[i]);
                if (DATA[i].IsEqual(first))
                {
                    Models.Polygon pl = new Models.Polygon(lst);
                    pl.CreateScreen(mtx);

                    if (scrBoundary.Width == 0)
                        scrBoundary = pl.ScreenBoundary.RF;
                    else
                        scrBoundary = RectangleF.Union(scrBoundary, pl.ScreenBoundary.RF);
                    if (boundary.Width == 0)
                        boundary = pl.Boundary.RF;
                    else
                        boundary = RectangleF.Union(boundary, pl.Boundary.RF);

                    pl.FillColor = colors[pls.Count];

                    pls.Add(pl);
                    if (i == DATA.Length - 1)
                        break;

                    lst = new List<DM.Geo.Coord>();
                    first = DATA[++i];
                    lst.Add(first);
                }
            }

            foreach(Models.Polygon pl in pls)
            {
                Geo.Coord c = new DM.Geo.Coord(scrBoundary.Location);
                pl.OffsetScreen(c.Negative());
            }
            scrBoundary.Offset(-scrBoundary.Left, -scrBoundary.Top);
            // {X = 0.0 Y = 0.0 Width = 783.22 Height = 260.900024}

            int dx = this.Width - ClientRectangle.Width;
            int dy = this.Height - ClientRectangle.Height;
            float h = (float)this.ClientRectangle.Width / scrBoundary.Width;
            float v = (float)this.ClientRectangle.Height / scrBoundary.Height;
            float zoom = Math.Min(h, v);

            DMControl.LayerControl.Instance.OnWorkingLayersChange += OnWorkingLayersChange;
        }
        private void OnWorkingLayersChange(object sender, EventArgs e)
        {
            partitions.Clear();
            elevations.Clear();
            foreach (DB.Segment deck in DMControl.LayerControl.Instance.WorkingLayers)
            {
                DM.Models.Partition part = DMControl.PartitionControl.FromID(deck.BlockID);
                if (part == null)
                    continue;
                AddLayer(part, new DM.Models.Elevation(deck.DesignZ));
            }
        }
        const float LOWEST = 560.1f; // RU3
        const float HIGHEST = 821.5f; // RD1

        static EagleEye me = new EagleEye();

        public static EagleEye Me
        {
            get { return EagleEye.me; }
            set { EagleEye.me = value; }
        }

        RectangleF boundary = new RectangleF();
        RectangleF scrBoundary = new RectangleF();
        List<Models.Polygon> pls = new List<DM.Models.Polygon>();
        List<Models.Partition> partitions = new List<DM.Models.Partition>();
        List<Models.Elevation> elevations = new List<DM.Models.Elevation>();
        static string[] partitionNames = new string[] {
            "EJ",
            "RU1",
            "RU2",
            "RU4",
            "RU3",
            "FU",
            "EU",
            "ED",
            "FD",
            "RD3",
            "RD1",
            "RD2"
            };
        private static int Index(string name)
        {
            for (int i = 0; i < partitionNames.Length; i++ )
            {
                if (name.Equals(partitionNames[i], StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            return -1;
        }
        private float ElevationToY(float e)
        {
            float l = LOWEST;
            float h = HIGHEST;
            float x = e;
            float percent = (x - l) / (h - l);
            float y = scrBoundary.Height * (1-percent);
            return y;
        }
        private float YToElevation(float y)
        {
            y -= offset.Y;
            y /= scrBoundary.Height;
            y = 1 - y;
            y *= (HIGHEST - LOWEST);
            y += LOWEST;
            return y;
        }
        public void AddLayer(Models.Partition p, Models.Elevation e)
        {
            partitions.Add(p);
            elevations.Add(e);
        }
        Color color = Color.Black;
        PointF offset;
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);


            if (this.FormBorderStyle == FormBorderStyle.None)
            {
                StringFormat sf5 = new StringFormat();
                sf5.Alignment = StringAlignment.Center;
                sf5.LineAlignment = StringAlignment.Near;
                using (Font ft1 = new Font(Font.FontFamily, 32, FontStyle.Bold))
                //using (Brush b = new SolidBrush(Color.FromArgb(0xFF, Color.White)), b1 = new SolidBrush(Color.FromArgb(0xFF, Color.Black)))
                {
                    //string str = "糯   扎   渡   大   坝   填   筑   质   量   GPS   监   控   系   统";
                    string str = "糯   扎   渡   水   电   站" + "\n 大   坝   填   筑   质   量   GPS   监   控   系   统";
                    g.DrawString(str, ft1, Brushes.Black, this.ClientRectangle, sf5);
                    //g.DrawString("大 坝 填 筑 质 量 GPS 监 控 系 统", ft, b, this.ClientRectangle, sf);
                    //Utils.Graph.OutGlow.DrawOutglowText(g, str, ft, this.ClientRectangle, sf, b, b1);
                    //GraphicsPath gp = new GraphicsPath();
                    //gp.AddString(str, Font.FontFamily, (int)FontStyle.Bold, 32, this.ClientRectangle, sf5);
                    //g.FillPath(b, gp);
                    //using (Pen p = new Pen(Color.FromArgb(0xFF, Color.Black)))
                    //    g.DrawPath(p, gp);
                }
            }

            float dx = 0;
            float dy = (this.ClientRectangle.Height - scrBoundary.Height)/2;
            offset = new PointF(dx, dy);
            g.TranslateTransform(dx, dy);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            foreach (Models.Polygon pl in pls)
            {
                pl.Draw(g);
            }
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            #region - 画高低线 -
            float lowy = ElevationToY(LOWEST);
            float hiy = ElevationToY(HIGHEST);
            PointF plow1 = new PointF(this.ClientRectangle.Left, this.ClientRectangle.Top+lowy);
            PointF plow2 = new PointF(this.ClientRectangle.Right, plow1.Y);
            PointF phi1 = new PointF(plow1.X, this.ClientRectangle.Top + hiy);
            PointF phi2 = new PointF(this.ClientRectangle.Right, this.ClientRectangle.Top + hiy);
            g.DrawLine(Pens.Black, plow1, plow2);
            g.DrawLine(Pens.Black, phi1, phi2);
            RectangleF rc1 = new RectangleF(plow1, new SizeF(this.ClientRectangle.Width, 20));
            RectangleF rc2 = new RectangleF(phi1.X, phi1.Y - 20, this.ClientRectangle.Width, 20);
            StringFormat sf1 = new StringFormat();
            StringFormat sf2 = new StringFormat();
            sf1.Alignment = StringAlignment.Near;
            sf2.Alignment = StringAlignment.Far;
            sf1.LineAlignment = StringAlignment.Far;
            sf2.LineAlignment = StringAlignment.Far;
            g.DrawString("最低："+LOWEST.ToString()+"米", Font, Brushes.Black, rc1, sf1);
            g.DrawString("最高"+HIGHEST.ToString() + "米", Font, Brushes.Black, rc2, sf2);
            #endregion

            #region - 画正在工作的仓面 -
            for (int i = 0; i < partitions.Count; i++ )
            {
                int idx = Index(partitions[i].Name);
                if (idx == -1)
                    continue;
                float y = ElevationToY((float)elevations[i].Height);
                PointF p1 = new PointF(ClientRectangle.Left, ClientRectangle.Top + y);
                PointF p2 = new PointF(ClientRectangle.Right, ClientRectangle.Top + y);
                Models.Polygon pl = pls[idx];
                Region rg = pl.SetDrawClip(g);
                using (Pen p = new Pen(color, 2))
                    g.DrawLine(p, p1, p2);
                g.Clip = rg;
            }

//             if (current == -1)
//                 return;
//             Models.Polygon plCurrent = pls[current];
//             Region old = plCurrent.SetDrawClip(g);
            PointF pp1 = new PointF(ClientRectangle.Left, cursor.Y - offset.Y);
            PointF pp2 = new PointF(ClientRectangle.Right, pp1.Y);
            if (pp1.Y <= 0 || pp1.Y > scrBoundary.Height || !this.Focused)
                return;

            using (Pen p = new Pen(Color.White, 3))
                g.DrawLine(p, pp1, pp2);
            using(Pen p = new Pen(Color.Black))
            {
                p.DashStyle = DashStyle.Dash;
                p.DashPattern = new float[] { 6, 6 };
                g.DrawLine(p, pp1, pp2);
            }
            RectangleF rc = new RectangleF(pp1.X, pp2.Y-25, ClientRectangle.Width, 20);
            StringFormat sf =new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Far;
            // strNorth.AddString("北", font.FontFamily, (int)font.Style, font.SizeInPoints, rc ,sf);
            string s = "该处高程="+YToElevation(cursor.Y).ToString("0.00")+"米";
            RectangleF rc3 = rc;

            Font ft = new Font(Font, FontStyle.Bold);
            Utils.Graph.OutGlow.DrawOutglowText(g, s, ft, rc3, sf, Brushes.Black, Brushes.WhiteSmoke);
//             for (int i = -1; i <= 1; i++ )
//             {
//                 for (int j = -1; j <= 1; j++ )
//                 {
//                     rc3 = rc;
//                     rc3.Offset(i, j);
//                     g.DrawString(s, ft, Brushes.White, rc3, sf);
//                 }
//             }
            //g.DrawPath(Pens.Black, gpstr);
//            g.DrawString(s, ft, Brushes.Black, rc, sf);
//             g.Clip = old;

            #endregion
        }
        private void CheckFullscr(bool full)
        {
            if (full && this.WindowState == FormWindowState.Maximized)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.ControlBox = false;
                this.Text = null;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.Text = "鹰眼";
                this.ControlBox = true;
            }
        }
        private void EagleEye_Resize(object sender, EventArgs e)
        {
            if (boundary.Width == 0)
                return;
            CheckFullscr(true);

            int dx = this.Width - ClientRectangle.Width;
            int dy = this.Height - ClientRectangle.Height;
            int w = ClientRectangle.Width;
            int h = ClientRectangle.Height;
            w = h * 3;
            w += dx;
            h += dy;
            //this.Size = new Size(w, h);

            float zoom = (float)ClientRectangle.Width/ boundary.Width;
            Geo.DMMatrix mtx = new DM.Geo.DMMatrix();
            mtx.zoom = zoom;
            System.Diagnostics.Debug.Print("{0}/{1}={2}", ClientRectangle.Width, boundary.Width, mtx.zoom.ToString());
            mtx.degrees = 180;
            scrBoundary = new RectangleF();
            foreach (Models.Polygon pl in pls)
            {
                pl.CreateScreen(mtx);
                if (scrBoundary.Width == 0)
                    scrBoundary = pl.ScreenBoundary.RF;
                else
                    scrBoundary = RectangleF.Union(scrBoundary, pl.ScreenBoundary.RF);
            }

           Geo.Coord c = new DM.Geo.Coord(scrBoundary.Location);
           foreach (Models.Polygon pl in pls)
            {
                pl.OffsetScreen(c.Negative());
            }
            scrBoundary.Offset(-scrBoundary.Left, -scrBoundary.Top);
            Refresh();
        }

        private void EagleEye_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }
        Timer tm = new Timer();
        private void OnTick(object sender, EventArgs e)
        {
            if (color == Color.Black)
                color = Color.White;
            else
                color = Color.Black;
            Invalidate();
        }
        private void EagleEye_Load(object sender, EventArgs e)
        {
            EagleEye_Resize(null, null);
            tm.Interval = 500;
            tm.Tick += OnTick;
            tm.Start();

            Rectangle rc = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            this.Location = new Point(rc.Right-this.Width-230, rc.Bottom-this.Height-30);

            OnWorkingLayersChange(null, null);

            this.WindowState = FormWindowState.Maximized;
            CheckFullscr(true);
        }
        private string CurrentPartition { get { if (current == -1) return null; else return partitionNames[current]; } }
        private void EagleEye_MouseClick(object sender, MouseEventArgs e)
        {
            string partition = CurrentPartition;
            if (partition == null)
                return;
            //double elevation = YToElevation(cursor.Y);
            DM.Models.Partition part = DMControl.PartitionControl.FromName(partition);
            if( part == null )
                return;
            foreach (DB.Segment deck in DMControl.LayerControl.Instance.WorkingLayers)
            {
                if( deck.BlockID == part.ID )
                {
                    string confirm = string.Format("目前分区{0}正在工作的层高程为{1}米，现在打开吗？", part.Name, deck.DesignZ);
                    if( Utils.MB.OKCancelQ(confirm))
                    {
                        ToolsWindow.I.OpenLayer(partition, deck.DesignZ);
                        CheckFullscr(false);
                    }
                    return;
                }
            }
            Utils.MB.OK("抱歉，该分区未发现正在工作的仓面。");
        }
        private void CheckFocus()
        {
            if (this.Focused == false)
                this.Focus();
        }
        int current = -1;
        Point cursor;
        private void EagleEye_MouseMove(object sender, MouseEventArgs e)
        {
            CheckFocus();
            //*tpp.SetToolTip(this, tpp.GetToolTip(this));*/
            if (cursor.Equals(e.Location))
                return;
            cursor = e.Location;

            float dy = (this.ClientRectangle.Height - scrBoundary.Height) / 2;
            PointF pt = e.Location;
            pt.Y -= dy;
            int i = 0;
            bool found = false;
            for (i = 0; i < pls.Count; i++)
            {
                if( pls[i].IsScreenVisible(new DM.Geo.Coord(pt) ))
                {
                    if( i != current )
                    {
                        //tpp.SetToolTip(this, null);
                        //System.Diagnostics.Debug.Print(partitionNames[i]);
                        pls[i].FillColor = Color.YellowGreen;
                        tpp.SetToolTip(this, partitionNames[i]);
                        if( current != -1 )
                            pls[current].FillColor = colors[current];
                        current = i;
                    }
                    //System.Diagnostics.Debug.Print(partitionNames[i]);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                if (current != -1)
                    pls[current].FillColor = colors[current];
                tpp.SetToolTip(this, null);
                current = -1;
            }
            Refresh();
        }

        private void EagleEye_KeyDown(object sender, KeyEventArgs e)
        {
            Main.MainWindow.ProcessKeys(sender, e);
        }

        private void EagleEye_Deactivate(object sender, EventArgs e)
        {
            this.Opacity = 0.5;
        }

        private void EagleEye_Activated(object sender, EventArgs e)
        {
            this.Opacity = 1;
        }

        private void EagleEye_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            CheckFullscr(false);
        }


    }
}
