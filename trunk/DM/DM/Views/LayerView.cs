#define OPTIMIZED_PAINTING
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using DM.Geo;
using System.Runtime.InteropServices;
using DM.Models;

namespace DM.Views
{
    public partial class LayerView : UserControl, IMessageFilter, IDisposable
    {
        public LayerView()
        {
            InitializeComponent();
        }
        public new void Dispose() { layer.Dispose();  this.Dispose(true); GC.SuppressFinalize(this); }

        public void Init()
        {
            layer.Owner = this;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.DoubleBuffered = true;
            if (IsPreview)
                return;
            this.AutoScroll = false;
            Operation = Operations.SCROLL_FREE;
            magnify = new Cursor(GetType(),"MAGNIFY.CUR");
            zoomer.OnZoomChange += OnZoomChange;
            compass.OnCCW += OnCCW;
            compass.OnCW += OnCW;

            landscape.Visible = false;
            landscape.Show(this);
            Rectangle rc = this.ClientRectangle;
            rc = this.RectangleToScreen(rc);
            landscape.Location = new Point(rc.Right - landscape.Width, rc.Bottom-landscape.Height);

//             Parent.Controls.SetVertex(hscr);
//             Parent.Controls.SetVertex(vscr);
// 
//             hscr.Dock = DockStyle.Bottom;
//             vscr.Dock = DockStyle.Right;
// 
//             hscr.Scroll += LayerView_Scroll;
//             vscr.Scroll += LayerView_Scroll;

            landscape.LayerView = this;
            landscape.Visible = false;

#if OPTIMIZED_PAINTING
            tmPaint.Interval = DM.Models.Config.I.REFRESH_TIME;
            tmPaint.Tick -= OnTickPaint;
            tmPaint.Tick += OnTickPaint;
            tmPaint.Start();
#endif
        }

        #region - 滚动相关 -
        Point myScrollPos = new Point(0, 0);
        Size myScrollSize = new Size();
        Rectangle myDisplayRectangle = new Rectangle();
        public int MyScrollX
        {
            get { return myScrollPos.X; }
            set
            {
//                 if (ClientRectangle.Width > myScrollSize.Width)
//                     return;

                int x = value;
                x = Math.Max(ClientRectangle.Width - myScrollSize.Width, x);
                x = Math.Min(0, x);
                myScrollPos = new Point(x, myScrollPos.Y);

                hscr.Value = -MyScrollX;
                IsDirty = true;
            }
        }
        public int MyScrollY
        {
            get { return myScrollPos.Y; }
            set
            {
//                 if (ClientRectangle.Height > myScrollSize.Height)
//                     return;

                int y = value;
                y = Math.Max(ClientRectangle.Height - myScrollSize.Height, y);
                y = Math.Min(0, y);
                myScrollPos = new Point(myScrollPos.X, y);

                if( -y >= vscr.Minimum && -y < vscr.Maximum )
                    vscr.Value = -y;
                IsDirty = true;
            }
        }

        protected override void OnScroll(ScrollEventArgs se)
        {

        }
        private void LayerView_Scroll(object sender, ScrollEventArgs e)
        {
            System.Diagnostics.Debug.Print(e.Type.ToString());
            //             if( e.Type == ScrollEventType.Last && e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            //             {
            //                 MyScrollX = 100;
            //                 return;
            //             }
            //             if (e.Type == ScrollEventType.Last && e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            //             {
            //                 MyScrollY = vscr.Maximum-1;
            //                 return;
            //             } 
            int delta = e.NewValue;
            //             if (e.Type == ScrollEventType.LargeDecrement || e.Type == ScrollEventType.LargeIncrement ||
            //                 e.Type == ScrollEventType.SmallDecrement || e.Type == ScrollEventType.SmallIncrement)
            //             {
            //                 if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            //                     delta = MyScrollX;
            //                 else
            //                     delta = MyScrollY;
            //             }

            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                MyScrollX = -delta;
            }
            else
            {
                MyScrollY = -delta;
            }
            MyRefresh();
        }
        public void CheckScrollVisible()
        {
            if (ClientRectangle.Width <= myDisplayRectangle.Width)
            {
                hscr.Visible = true;
            }
            else
                hscr.Visible = false;
            if (ClientRectangle.Height <= myDisplayRectangle.Height)
            {
                vscr.Visible = true;
            }
            else
                vscr.Visible = false;

            if (hscr.Visible || vscr.Visible)
                landscape.Visible = true;
            else
                landscape.Visible = false;

            //btnLandscape.Visible = false;
        }
        public void OnClosedLandscape()
        {
            //btnLandscape.Visible = true;
        }
        private void SetAutoScroll()
        {
            try
            {
                if (IsPreview)
                    return;
                Size sz = layer.VisibleSize;
                sz.Width += PAD_HORZ * 2;
                sz.Height += PAD_VERT * 2;
                myScrollSize = sz;
                //             MyScrollX = 0;
                //             MyScrollY = 0;
                myDisplayRectangle = new Rectangle(0, 0, sz.Width, sz.Height);
                //             AutoScrollMinSize = sz;
                CheckScrollVisible();

                if (hscr.Visible)
                {
                    hscr.Minimum = 0;
                    hscr.Maximum = myDisplayRectangle.Width - ClientRectangle.Width + PAD_HORZ;
                    hscr.SmallChange = hscr.Maximum / 100;
                    hscr.LargeChange = hscr.Maximum / 10;
                    hscr.Maximum += hscr.LargeChange;
                    hscr.Value = 0;
                }
                if (vscr.Visible)
                {
                    vscr.Minimum = 0;
                    vscr.Maximum = myDisplayRectangle.Height - ClientRectangle.Height + PAD_VERT;
                    vscr.SmallChange = vscr.Maximum / 100;
                    vscr.LargeChange = vscr.Maximum / 10;
                    vscr.Maximum += vscr.LargeChange;
                    vscr.Value = 0;
                }

                landscape.UpdateSize(myDisplayRectangle);
                landscape.UpdateLocation();
            }
            catch
            {

            }

            //Refresh();
            //this.AutoScrollMinSize = this.Size;
            //this.AutoScrollMargin = new Size(PAD_HORZ, PAD_VERT);
        }
        private PointF DeScrollPoint(PointF pt)
        {
            //             pt.X += AutoScrollPosition.X;
            //             pt.Y += AutoScrollPosition.Y;
            pt.X += MyScrollX+PAD_HORZ;
            pt.Y += MyScrollY+PAD_VERT;
            return pt;
        }
        private PointF ScrollPoint(PointF pt)
        {
            //             pt.X -= AutoScrollPosition.X;
            //             pt.Y -= AutoScrollPosition.Y;
            pt.X -= MyScrollX+PAD_HORZ;
            pt.Y -= MyScrollY+PAD_VERT;
            return pt;
        }
        #endregion

        #region - 变量 -
        HScrollBar hscr = new HScrollBar();
        VScrollBar vscr = new VScrollBar();

        Forms.Landscape landscape = new DM.Forms.Landscape();


        const int PAD_HORZ = 5;
        const int PAD_VERT = 5;
        const double ZOOM_MIN = 1;
        const double ZOOM_MAX = 100;
        const int ZOOM_STEPS = 20;
        double ZOOM_STEP_FACTOR = Math.Pow(ZOOM_MAX / ZOOM_MIN, (double)1 / ZOOM_STEPS);
        
        Models.Layer layer = new DM.Models.Layer("");
        public Models.Layer Layer { get { return layer; } }

        string fullPath;
        bool isDown = false;
        PointF origDownPos;
        PointF downPos;
        PointF cursorPos;
        Point moveOffset = new Point(PAD_HORZ, PAD_VERT);
        Cursor magnify;
        bool isRectSelecting = false;
        private bool IsRectSelecting { get { return isRectSelecting; } 
            set 
            {
                bool was = isRectSelecting;
                isRectSelecting = value;
                MyRefresh();
                if( was && !value )
                {
                    DMRectangle rc = new DMRectangle();
                    rc.LeftTop = new Coord(cursorPos);
                    rc.RightBottom = new Coord(origDownPos);
                    rc.Normalize();
                    RestoreRect(ref rc);
                    Coord p1 = rc.LeftTop;
                    Coord p2 = rc.RightTop;
                    Coord p3 = rc.RightBottom;
                    Coord p4 = rc.LeftBottom;
                    List<Coord> lc = new List<Coord>(new Coord[] { p1, p4, p3, p2, p1 });
                    DeckClip(lc);
                }
            } 
        }
        private void CancelRectSelecting() { isRectSelecting = false; MyRefresh(); }
        bool isPolySelecting = false;
        public bool isDeckInput = false;
        public bool IsPolySelecting
        {
            get { return isPolySelecting; }
            set
            {
                if( isPolySelecting && !value )
                {
                    // 确认选择
                    DeckClip(deckSelectPolygon);
                    deckSelectPolygon.Clear();
                    UnlockCursor();
                    RestoreCursor();
                }
                if (isDeckInput)
                {
                    DeckClip(deckSelectPolygon);
                    deckSelectPolygon.Clear();
                    //UnlockCursor();
                    //RestoreCursor();
                    isDeckInput = false;
                }
                isPolySelecting = value;
            }
        }
        // 多边形选择
        private void CancelPolySelecting() 
        {
            // 每次取消最后一个选择点

            // 如果列表中只剩下一个点，或者根本没开始，就完全取消选择状态

            if (deckSelectPolygon.Count <=1 )
            {
                isPolySelecting = false;
                deckSelectPolygon.Clear();
                MyRefresh();
                return;
            }
            deckSelectPolygon.RemoveAt(deckSelectPolygon.Count-1);
            MyRefresh();
        }
        #endregion

        //////////////////////////////////////////////////////////////////////////////////
        #region - 属性 -
        // 是否反走样绘图

        bool smooth = true;
        [DefaultValue(true)]
        public bool Smooth { get { return smooth; } set { smooth = value; MyRefresh(); } }
        // 放大率

        double zoom = 10;
        public double Zoom { get { return zoom; } 
            set 
            {
                if (AlwaysFitScreen)
                    return;
//                 if (zoom == value)
//                     return;
                zoom = value;
                if (zoom < ZOOM_MIN)
                    zoom = ZOOM_MIN;
                if (zoom > ZOOM_MAX)
                    zoom = ZOOM_MAX;
            } 
        }
        double rotateDegrees = 0;
        public double RotateDegrees { get { return rotateDegrees; } set { value %= 360; rotateDegrees = value; } }
        bool alwaysFitScreen = false;
        public bool AlwaysFitScreen { get { return alwaysFitScreen; } set { alwaysFitScreen = value; } }
        public void FitScreenOnce()
        {
            //Zoom = layer.FitScreenZoom;
            MyScrollX = 0;
            MyScrollY = 0;
            AlwaysFitScreen = true;
            UpdateGraphics();
            zoom = layer.Zoom;
            AlwaysFitScreen = false;
            layer.AlwaysFitScreen = false;
        }
        Operations currentOp = Operations.OBSERVE;
        public string FullPath { get { return fullPath; } set { fullPath = value; } }
        bool lockCursor = false;
        private void LockCursor() { lockCursor = true; }
        private void UnlockCursor() { lockCursor = false; }
        private void RestoreCursor()
        {
            if (lockCursor)
                return;
            if (Cursor != OperationCursor())
                Cursor = OperationCursor();
        }
        private Cursor OperationCursor()
        {
            return OperationsToCursor.Cursor(currentOp, magnify);
        }
        public Operations Operation { get { return currentOp; } 
            set 
            {
                bool cancel = false;
                OnOperationChange(currentOp, value, ref cancel);
                if (cancel)
                    return;

                currentOp = value;
                //System.Diagnostics.Debug.Print("Operation: {0}", value.ToString());
                Cursor = OperationCursor();
            } 
        }
        bool isPreview = false;
        public bool IsPreview { get { return isPreview; } set { isPreview = value; } }
        public float MouseMoveDeltaVert { get { float y = cursorPos.Y - downPos.Y; downPos = cursorPos; return y; } }
        //public float MouseMoveDeltaHorz { get { float x = cursorPos.X - downPos.X; downPos = cursorPos; return x; } }
        public Point MouseMoveDelta { get { float x = cursorPos.X - downPos.X; float y = cursorPos.Y - downPos.Y; downPos = cursorPos; return new Point((int)x, (int)y); } }
        #endregion

        //////////////////////////////////////////////////////////////////////////////////
        #region - 画图部分 -
        internal volatile bool dirty = true;

        private bool IsDirty
        {
            get { return dirty; }
            set { dirty = value; }
            //get { while (isPainting) System.Threading.Thread.Sleep(1); return dirty; }
            //set { while (isPainting) System.Threading.Thread.Sleep(1); dirty = value; /*if (dirty) Refresh();*/ }
        }
        private volatile bool isPainting = false;
        public void MyRefresh()
        {
            if( IsPreview )
            {
                this.Refresh();
            }
            else
            {
                lock (updateLock)
                    IsDirty = true;
            }
// #if OPTIMIZED_PAINTING
//             IsDirty = true;
// #else
//             base.Refresh();
// #endif
        }
//         private void AdjustScroll(Graphics g)
//         {
//             g.TranslateTransform(this.AutoScrollPosition.X + moveOffset.X, this.AutoScrollPosition.Y + moveOffset.Y);
//         }
//         private void CreatePic()
//         {
//             if (pic != null)
//                 pic.Dispose();
//             pic = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height);
//         }
        private void AdjustScroll(Graphics g)
        {
            g.TranslateTransform(MyScrollX + moveOffset.X, MyScrollY+ moveOffset.Y);
            //g.TranslateTransform(myScrollPos.X + moveOffset.X, ClientSize.Height+myScrollPos.Y - moveOffset.Y);
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
//             if (!dirty)
//                 return;
// 
            lock(updateLock)
            {
                isPainting = true;

                if (!this.Visible)
                {
                    isPainting = false;
                    return;
                }

                Graphics g = e.Graphics;
                Graphics lg = landscape.GetGraphics();
                //g.TranslateTransform(0, ClientSize.Height);
                g.FillRectangle(Brushes.White, this.DisplayRectangle);
                AdjustScroll(g);
                //             g.ScaleTransform(1, -1);

                if (layer.IsEmtpy())
                {
                    isPainting = false;
                    return;
                }
                if (Smooth && !isMoving && !isZooming)
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                }

                //             Rectangle rc = this.DisplayRectangle;
                //             Region rg = new Region(rc);
                //             rg.Exclude(compass.Boundary);
                //             rg.Exclude(scaler.Boundary);
                //             rg.Exclude(zoomer.Boundary);
                //             g.Clip = rg;

                // 画层
                layer.Draw(g, RestoreCoord(cursorPos), true, isMoving || isZooming, Font);
                if (lg != null) layer.Draw(lg, RestoreCoord(cursorPos), false, isMoving || isZooming, Font);

                if (!isPreview)
                {
                    DrawRectSelect(g);
                    DrawPolySelect(g);
                    //                 DrawExperiment(g);
                    //                 if (lg != null) DrawExperiment(lg);
                }

                g.ResetTransform();
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                DrawDebugInfo(g);
                if (!IsPreview)
                {
                    // 显示其他信息
                    DrawCompass(g);
                    DrawZoom(g);
                    DrawScale(g);
                }

                if (!IsPreview)
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Near;
                    using (Font ft = new Font(Font.FontFamily, 32, FontStyle.Bold))
                    using (Brush b = new SolidBrush(Color.FromArgb(0xFF, Color.White)), b1 = new SolidBrush(Color.FromArgb(0xFF, Color.Black)))
                    {
                        //string str = "糯   扎   渡   大   坝   填   筑   质   量   GPS   监   控   系   统";
                        string str = "糯   扎   渡   水   电   站" + "\n大   坝   填   筑   质   量   GPS   监   控   系   统";
                        //g.DrawString("大 坝 填 筑 质 量 GPS 监 控 系 统", ft, b, this.ClientRectangle, sf);
                        //Utils.Graph.OutGlow.DrawOutglowText(g, str, ft, this.ClientRectangle, sf, b, b1);
                        GraphicsPath gp = new GraphicsPath();
                        gp.AddString(str, Font.FontFamily, (int)FontStyle.Bold, 32, this.ClientRectangle, sf);
                        g.FillPath(b, gp);
                        using(Pen p = new Pen(Color.FromArgb(0xFF, Color.Black)))
                            g.DrawPath(p, gp);
                    }

                }
                landscape.ReleaseGraphics(lg, myScrollPos, ClientRectangle);
                dirty = false;
                isPainting = false;
            }
        }
        // 画各个元素

        RectSelector selector = new RectSelector();
        private void DrawRectSelect(Graphics g)
        {
            if (Operation != Operations.DECK_RECT /*|| !IsRectSelecting*/)
                return;
            if( !IsRectSelecting )
            {
                DrawAxisCood(g);
                return;
            }
            DMRectangle rc = new DMRectangle();
            PointF down = new PointF(downPos.X - moveOffset.X, downPos.Y - moveOffset.Y);
            PointF cursor = new PointF(cursorPos.X-moveOffset.X, cursorPos.Y - moveOffset.Y);
            rc.LeftTop = new Coord(down);
            rc.RightBottom = new Coord(cursor);
            if (rc.Width == 0 || rc.Height==0)
                return;
            selector.DrawSelector(g, rc, Font, DamAxisCursor(), cursor, this);
        }
        private void DrawAxisCood(Graphics g)
        {
            RectangleF rc = new RectangleF(cursorPos.X, cursorPos.Y - 20, 1000, 20);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Far;
            Utils.Graph.OutGlow.DrawOutglowText(g, DamAxisCursor().ToString(), Font, rc, sf, Brushes.Black, Brushes.White);
        }
        private void DrawPolySelect(Graphics g)
        {
            if (Operation != Operations.DECK_POLYGON/* || !IsPolySelecting*/)
                return;
//             if (deckSelectPolygon.Count == 0)
//                 return;
            List<Coord> lc = new List<Coord>(deckSelectPolygon);
            Coord cursor = RestoreCoord(cursorPos);
            lc.Add(cursor);
            Models.Polygon pl = new DM.Models.Polygon();
            pl.NeedClose = false;
            pl.SetScreenVertex(lc);
//             pl.OffsetScreen((new Coord(moveOffset)).Negative());
            PolySelector.DrawPolySelect(g, pl, this.Font, DamAxisCursor(), cursorPos);
            if (IsPolySelectingClosed(cursor))
            {
                LockCursor();
                Cursor = Cursors.Hand;
            }
            else
            {
                UnlockCursor();
                RestoreCursor();
            }
        }
        private void CheckDeckFocus()
        {
            // 改到画仓面的同时进行，提高效率

//             Coord cursor = RestoreCoord(cursorPos);
//             layer.CheckDeckFocus(cursor);
        }
        Scaler scaler = new Scaler();
        private void DrawScale(Graphics g)
        {
            scaler.DrawScaler(g, this.ClientRectangle, zoom, Font);
        }
        Zoomer zoomer = new Zoomer();
        private void DrawZoom(Graphics g)
        {
            Rectangle rc = this.ClientRectangle;
            double zm = layer.Zoom;
            zoomer.DrawScale(g, rc, Font, zm, ZOOM_MIN, ZOOM_MAX, ZOOM_STEP_FACTOR, ZOOM_STEPS);
        }
        Compass compass = new Compass();
        private void DrawCompass(Graphics g)
        {
            if (IsPreview)
                return;
            compass.DrawCompass(g, this.ClientRectangle, RotateDegrees, Font);
        }
        private PointF DePadding(PointF pt)
        {
            return new PointF(pt.X + PAD_HORZ, pt.Y + PAD_VERT);
        }
        public Coord DamAxisCursor(PointF cur)
        {
            Coord dampt = ScreenToDam(DePadding(cur));
            Coord damaxis = dampt.ToDamAxisCoord();
            return damaxis;
        }
        public Coord DamAxisCursor()
        {
            Coord dampt = ScreenToDam(DePadding(cursorPos));
            Coord damaxis = dampt.ToDamAxisCoord();
            return damaxis;
        }
        private void DrawDebugInfo(Graphics g)
        {
            RectangleF rc = this.ClientRectangle;
            rc.Location = new PointF(rc.Location.X, rc.Location.Y);
            /*rc.Height = (int)(rc.Height*.95);*/

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Far;
            sf.LineAlignment = StringAlignment.Far;
            PointF pt = MyCoordSys(cursorPos);
            //pt.Y = -pt.Y;
            int count = layer.RollCount(cursorPos);
            Coord dampt = ScreenToDam(DePadding(cursorPos));
            Coord damaxis = dampt.ToDamAxisCoord();
            dampt.Y = -dampt.Y;
            PointF scrPt = pt;
            // PointF(pt.X, ClientSize.Height - pt.Y - PAD_VERT*2);
            scrPt = new PointF(pt.X, myDisplayRectangle.Height - cursorPos.Y - PAD_VERT * 2);
            string dbg = string.Format("层:{0}, 大地{1}, 施工{2}",
                this.ToString(), /*layer.Zoom, RotateDegrees, scrPt.ToString(), */dampt.ToString(), damaxis.ToString());
            Utils.Graph.OutGlow.DrawOutglowText(g, dbg, Font, rc, sf, Brushes.Black, Brushes.WhiteSmoke);

            if (IsPreview || Operation != Operations.ROOL_COUNT)
                return;
            rc = zoomer.Boundary;
            rc.Offset(0, rc.Height+50);
            int emSize = count*3+12;
            emSize = Math.Min(emSize, 80);
            string strCount = string.Format("碾压{0}遍", count);
            if (count == 0)
                strCount = "未碾压";
            Font ft = new Font(Font.FontFamily, emSize, count==0?FontStyle.Regular:FontStyle.Bold, GraphicsUnit.Pixel);
            SizeF size = g.MeasureString(strCount, ft);
            rc.Location = new PointF(0, rc.Location.Y);
            rc.Height = (int)size.Height+1;
            rc.Width = 130;//(int)size.Width + 1;
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Near;
            sf.FormatFlags |= StringFormatFlags.NoClip | StringFormatFlags.NoWrap;
            sf.Trimming = StringTrimming.None;
            Color cl = Color.OrangeRed;
            int goodCount = 8;
            Models.Deck deck = layer.VisibleDeck;
            if (deck != null) goodCount = deck.DeckInfo.DesignRollCount;
            if (count >= goodCount)
                cl = Color.OliveDrab;
            rc.Location = DeScrollPoint(cursorPos);
            //rc.Offset(-PAD_HORZ, -PAD_VERT);

            GraphicsPath gp = new GraphicsPath();
            gp.AddString(strCount, Font.FontFamily, (int)(count==0?FontStyle.Regular:FontStyle.Bold), emSize, rc, sf);
            //g.ExcludeClip(new Region(gp));

            //using(Brush b = new SolidBrush(Color.FromArgb(0x0, cl)), b1 = new SolidBrush(Color.FromArgb(0xa0, Color.WhiteSmoke)))
            //    Utils.Graph.OutGlow.DrawOutglowText(g, strCount, ft, rc, sf, b, b1);
            using (Pen p1 = new Pen(Color.FromArgb(0x0, cl)), p2 = new Pen(Color.FromArgb(0x40, Color.White)))
                Utils.Graph.OutGlow.DrawOutglowPath(g, gp, p1, p2);
            using (Brush p1 = new SolidBrush(cl), p2 = new SolidBrush(Color.Transparent))
                Utils.Graph.OutGlow.FillOutglowPath(g, gp, p1, p2);

            // 坐标信息
            sf.LineAlignment = StringAlignment.Far;
            rc = new RectangleF(rc.Left, rc.Top - 20, rc.Width, 20);
            rc.Offset(5, 0);
            Utils.Graph.OutGlow.DrawOutglowText(g, damaxis.ToString(), Font, rc, sf, Brushes.Black, Brushes.WhiteSmoke);
            //g.DrawString(dampt.ToString(), Font, Brushes.Black, rc, sf);
        }
        #endregion

        //////////////////////////////////////////////////////////////////////////////////
        #region - 业务逻辑处理 -
        
        private void DeckClip(List<Coord> shape)
        {
            // rc: 未经处理的屏幕坐标，经过滚动处理
            //System.Diagnostics.Debug.Print(rc.ToString());
            Models.Polygon pl = new DM.Models.Polygon(shape);
            layer.CutBy(pl);
        }

        private void OnZoomChange(object sender, EventArgs e)
        {
            Zoom = zoomer.ZoomValue;
            UpdateGraphics();
        }
        private void RestoreRect(ref DMRectangle rc)
        {
            rc.Offset(-moveOffset.X, -moveOffset.Y);
        }
        private Geo.Coord RestoreCoord(PointF pt)
        {
            return RestoreCoord(new Geo.Coord(pt));
        }
        private Geo.Coord RestoreCoord(Coord c)
        {
            return c.Offset(-moveOffset.X, -moveOffset.Y);
        }
        public PointF DamToScreen(Geo.Coord pt)
        {
            PointF c = layer.DamToScreen(pt);
            c.X += moveOffset.X;
            c.Y += moveOffset.Y;
            return c;
        }
        private Geo.Coord ScreenToDam(PointF pt)
        {
            pt.X -= moveOffset.X;
            pt.Y -= moveOffset.Y;
            return layer.ScreenToDam(pt);
        }

        public void Clear()
        {
            layer.Clear();
        }
        public bool IsEqual(string s) { return FullPath.Equals(s, StringComparison.OrdinalIgnoreCase); }
        public bool IsEqual(LayerView lv) { return IsEqual(lv.FullPath); }
        //Models.Deck experiment;
        // 打开层

        public bool OpenLayer(Models.PartitionDirectory p, Models.ElevationFile e)
        {
            if (p == null || e == null)
                return false;

//             layer = new DM.Models.Layer();
            if( layer.Partition ==null || IsPreview)
            {
                layer.Partition = p.Partition;
                layer.Elevation = e.Elevation;
                layer.FullPath = e.FullName;
                SetTitle();
            }

            fullPath = e.FullName;
            if (!ReadFile(p.Partition, e.Elevation))
                return false;

            if( !IsPreview )
            {
                layer.OnMouseEnter += OnLayerEnter;
                layer.OnMouseLeave += OnLayerLeave;
                layer.OnMouseEnterDeck += OnDeckEnter;
                layer.OnMouseLeaveDeck += OnDeckLeave;
            }
            UpdateGraphics();
            return true;
        }
        public override string ToString()
        {
            if (layer == null)
                return "null";
            return layer.ToString();
        }
        private void SetTitle()
        {
            if (layer == null) return;
            this.Text = this.ToString();
        }
        public bool ReadFile(Models.Partition p, Models.Elevation e)
        {
            if (layer == null)
                return false;
            List<Coord> lst = Models.FileHelper.ReadLayer(fullPath);
            if (lst == null)
                return false;
            layer.AddLayer(lst, p, e, !IsPreview);

            return true;
        }
        List<Coord3D> tracking;
        Timer timerTracking = new Timer();
        int trackingCount = 0;
        private void OnTickTracking(object sender, EventArgs e)
        {
            if( trackingCount >= tracking.Count )
            {
                timerTracking.Stop();
                trackingCount = 0;
                return;
            }
            Models.Deck dk = layer.DeckControl.Decks[0];
            Models.Vehicle veh = dk.VehicleControl.Vehicles[0];
            veh.TrackGPSControl.Tracking.AddOnePoint(tracking[trackingCount], 
                0/*-165*/ /*+moveOffset.X*//* - this.layer.DeckControl.Decks[0].Polygon.ScreenBoundary.Left*/,
                0/*-1090*/ /*+moveOffset.Y*//* - this.layer.DeckControl.Decks[0].Polygon.ScreenBoundary.Top*/);
            //System.Diagnostics.Debug.Print("track points = {0}", experiment.VehicleControl.Vehicles[0].TrackGPS.Count);
            trackingCount++;
            MyRefresh();
        }
        private void ExperimentAtOnce(double zm)
        {
            if (layer.DeckControl.Decks.Count == 0)
                return;

            Models.Deck dk = layer.DeckControl.Decks[0];
            //dk.CreateRollCountReport(zm);
            //Refresh();
        }
        private void CreateExperiment(double zm)
        {
            // EXPERIMENT
            if (!IsPreview && this.layer.DeckControl.Decks.Count != 0)
            {
            }
            else
                return;
//             if (layer.Zoom == zm)
//                 return;

            Models.Deck dk = layer.DeckControl.Decks[0];
            dk.VehicleControl.Clear();

            if (tracking == null)
                tracking = Models.FileHelper.ReadTracking("TrackingExp.txt");
            //Models.FileHelper.WriteTracking(tracking);

            //                 const int START = 40;
            //                 const int END = 714;
            //                 tracking.RemoveRange(END, tracking.Count - END);
            //                 tracking.RemoveRange(0, START);
            Coord origin = dk.Polygon.Boundary.Center;
            Models.TrackGPS.PreFilter(ref tracking);
            Models.Vehicle v = new DM.Models.Vehicle(dk);
            dk.VehicleControl.AddVehicle(v);
            //Models.Vehicle veh = dk.VehicleControl.Vehicles[0];
            Models.TrackGPS t = new DM.Models.TrackGPS(v);
            v.TrackGPSControl.Tracking = t;
            v.WheelWidth = 1f;
            List<Coord3D> trackingAnother = new List<Coord3D>(tracking);
            Models.TrackGPS.SetOrigin(ref trackingAnother, origin);
            t.SetTracking(trackingAnother, 0, 0);
            v.ID = 100;

            origin = origin.Offset(5, 2);
            List<Coord3D> trackingYetAnother = new List<Coord3D>(tracking);
            Models.TrackGPS.SetOrigin(ref trackingYetAnother, origin);
            Models.Vehicle v2 = new DM.Models.Vehicle(dk);
            dk.VehicleControl.AddVehicle(v2);
            Models.TrackGPS t2 = new DM.Models.TrackGPS(v2);
            v2.TrackGPSControl.Tracking = t2;
            t2.SetTracking(trackingYetAnother, 0, 0);
            t2.Color = Color.OrangeRed;
            v2.ID = 101;

            Models.Vehicle vInstant = new DM.Models.Vehicle(dk);
            dk.VehicleControl.AddVehicle(vInstant);
            Models.TrackGPS tInstant = new DM.Models.TrackGPS(vInstant);
            vInstant.TrackGPSControl.Tracking = tInstant;
            tInstant.Color = Color.Black;
            vInstant.ID = 3;
            vInstant.ListenGPS();

            trackingCount = 0;
            timerTracking.Interval = 200;
            timerTracking.Tick -= OnTickTracking;
            timerTracking.Tick += OnTickTracking;
            //timerTracking.Start();
            ExperimentAtOnce(zm);
            // EXPERIMENT

        }
        Timer tmPaint = new Timer();
        private void OnTickPaint(object sender, EventArgs e)
        {
            if( IsDirty )
                this.Refresh();
        }
        private delegate void MyPaint();
        public void RequestPaint()
        {
            if (IsPreview)
                this.Refresh();
            else
                MyRefresh();
            
            //IsDirty = true;
            //Refresh();
//             try
//             {
//                 this.BeginInvoke(new MyPaint(this.Refresh));
//             }
//             catch
//             {
// 
//             }
        }
//         private void DrawExperiment(Graphics g)
//         {
//             //             if (experiment == null)
//             //                 return;
//             if (layer.DeckControl.Decks.Count == 0)
//                 return;
//             foreach (Models.Vehicle v in /*experiment.VehicleControl.Vehicles*/layer.DeckControl.Decks[0].VehicleControl.Vehicles)
//             {
//                 v.TrackGPSControl.Tracking.Draw(g, true);
//             }
//         }
        object updateLock = new object();
        //int i = 0;
        public void UpdateGraphics()
        {
            //System.Diagnostics.Debug.Print(i++.ToString());
            //TrackGPS.WAITFINISHED =false;
            lock(updateLock)
            {
                double oldzoom = layer.Zoom;
                Point pt = myScrollPos;

                //             MyScrollX = 0;
                //             MyScrollY = 0;
                layer.Zoom = zoom;
                layer.AlwaysFitScreen = alwaysFitScreen;
                layer.RotateDegree = rotateDegrees;
                Rectangle rc = this.ClientRectangle;
                rc.Offset(moveOffset);
                rc.Offset(moveOffset);
                rc.Width -= 4 * 5;// moveOffset.X;
                rc.Height -= 4 * 5;// moveOffset.Y;
                layer.CreateScreen(rc);
                SetAutoScroll();

                //if( oldzoom != zoom )
                //CreateExperiment(zoom);

                MyScrollX = (int)(pt.X * zoom / oldzoom);
                MyScrollY = (int)(pt.Y * zoom / oldzoom);
                
            }
            RequestPaint();
            //TrackGPS.WAITFINISHED = true;
        }
        #region - 漫游 -
        private void RoamFree()
        {
            PointF d = DeScrollPoint(downPos);
            PointF c = DeScrollPoint(cursorPos);
            Point p = new Point((int)(d.X-c.X), (int)(d.Y-c.Y));
//             int x = AutoScrollPosition.X;
//             int y = AutoScrollPosition.Y;
            int x = MyScrollX;
            int y = MyScrollY;
            int dx = (int)(-p.X * 1);
            int dy = (int)(-p.Y * 1);
            //downPos = cursorPos;
            if (dx == 0 && dy == 0)
                return;
//             System.Diagnostics.Debug.Print("{0}, dx={1} dy={2}", p.ToString(), dx, dy);
//             AutoScrollPosition = new Point(dx-x, dy-y);
            MyScrollX += dx;
            MyScrollY += dy;
            //downPos = cursorPos;
            /*MyRefresh();*/
//             Rectangle rc = this.ClientRectangle;
//             rc.Offset(MyScrollX, MyScrollY);
//             this.Invalidate(rc);
            MyRefresh();
        }
        // d > 0 往上滚，否则往下滚
        // op = 0: 垂直滚动
        // op = 1: 水平滚动
        // op = 2: 全方位滚动

        private void RoamStepping(int op, int d) 
        {
//             if( !AutoScroll ) return;
            int x = MyScrollX;
            int y = MyScrollY;
//             int x = AutoScrollPosition.X;
//             int y = AutoScrollPosition.Y;
//             if (d > 0) d = 10;
//             else d = -10;
            if (op == 0)
                MyScrollY += d;
            else if (op == 1)
                MyScrollX += d;
            else
            {
                MyScrollX += d;
                MyScrollY += d;
            }
            //System.Diagnostics.Debug.Print("Scroll {0}", AutoScrollPosition.ToString());
            MyRefresh();
        }
        #endregion
        #region - 放大、旋转 -
        // d > 0 放大，否则缩小

        private void ZoomStepping(int deltaPercent) 
        {
            // ALT+SHIFT+滚轮：大规模放大、缩小

            // ALT+滚轮：微调放大、缩小

//             double delta = .5;
//             if (ModifierKeys == (Keys.Control|Keys.Shift))
//                 delta = 5;
            Zoom *= (double)(100+deltaPercent)/100;

            UpdateGraphics();
        }
        private void Rotating()
        {
            float delta = MouseMoveDeltaVert;
            RotateDegrees += delta;
            UpdateGraphics();
        }
        private void RotateStepping(double deltaDegrees)
        {
            RotateDegrees += deltaDegrees;
            UpdateGraphics();
        }
        public void ShowLandscape(bool show)
        {
            if (show == false)
                landscape.Visible = show;
            else
                CheckScrollVisible();
        }
        #endregion
        public void OnActiveTab()
        {
            ShowLandscape(true);
        }
        public void OnLostTab()
        {
            layer.HideActiveDeck();
            ShowLandscape(false);
        }
        private void CheckFocus()
        {
            if (!this.Focused)
            {
                this.Focus();
                if (IsPreview)
                    return;
                Forms.ToolsWindow.I.CurrentLayer = this;
                Forms.ToolsWindow.I.HidePreview();

                //Application.AddMessageFilter(this);
            }
        }
        private void UpdateMousePos()
        {
            /*this.Invalidate();*/
            MyRefresh();
        }
        #region - MouseOperation -
        private void MouseOperation(MouseEventArgs e)
        {
            switch (Operation)
            {
                case Operations.MOVE:
                    break;
                case Operations.ROTATE:
                    //Rotating();
                    if (e.Button == MouseButtons.Left)
                        RotateStepping(30);
                    if (e.Button == MouseButtons.Right)
                        RotateStepping(-30); 
                    break;
                case Operations.SCROLL_HORZ:
                    RoamStepping(1, e.Delta);
                    break;
                case Operations.SCROLL_VERT:
                    RoamStepping(0, e.Delta);
                    break;
                case Operations.SCROLL_ALL:
                    RoamStepping(2, e.Delta);
                    break;
                case Operations.ZOOM:
                    if( e.Button == MouseButtons.Left)
                    {
                        ZoomStepping(20);
                    }
                    if(e.Button == MouseButtons.Right)
                    {
                        ZoomStepping(-20);
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion
        #endregion


        #region - 事件处理 -
        private void SetTip(Control c, string caption, string title)
        {
            if (Operation == Operations.ROOL_COUNT)
                return;

            string orig = tpp.GetToolTip(c);
            if (orig.Equals(caption))
                return;
            tpp.SetToolTip(c, null);
            tpp.ToolTipTitle = title;
            tpp.SetToolTip(c, caption);
        }
        private void OnLayerEnter(object sender, EventArgs e)
        {
            if (sender == null)
            {
                tpp.SetToolTip(this, null);
                return;
            }
            if( sender is Models.Polygon)
            {
                Models.Polygon pl = (Models.Polygon)sender;
                Models.Partition p = pl.Partition;
                Models.Elevation el = pl.Elevation;
                SetTip(this, Models.Layer.Format(p, el) + " 面积："+pl.ActualArea.ToString("0.00")+"平方米", "层信息");
            }
        }
        private void OnLayerLeave(object sender, EventArgs e)
        {
            //tpp.SetToolTip(this, null);
        }
        private void OnDeckEnter(object sender, EventArgs e)
        {
            if (sender == null)
            {
                tpp.SetToolTip(this, null);
                return;
            }
            if( sender is Models.Deck)
            {
                Models.Deck dk = (Models.Deck)sender;
                string tip = string.Format("{0} \"{1}\" 面积：{2:0.00}平方米，{3}",
                    dk.ToString(), dk.Name, dk.Polygon.ActualArea, dk.Status);
                if(dk.DeckInfo.IsFinished() && dk.DeckInfo.POP !=-1 )
                {
                    tip += "，标准遍数百分比";
                    tip += dk.DeckInfo.POP.ToString("P02");
                }
                SetTip(this,
                    tip,
                    "仓面信息");
            }
        }
        private void OnDeckLeave(object sender, EventArgs e)
        {
            //tpp.SetToolTip(this, null);
        }

        public List<Coord> deckSelectPolygon = new List<Coord>();
        private void OnOperationChange(Operations old, Operations newop, ref bool cancel)
        {
            if( old != Operations.DECK_RECT && newop == Operations.DECK_RECT)
            {
                deckSelectPolygon.Clear();
            }
            if (newop == Operations.ROOL_COUNT)
                tpp.SetToolTip(this, null);
        }
        private void EndRectSelecting()
        {
            if (Operation != Operations.DECK_RECT)
                return;
            IsRectSelecting = !IsRectSelecting;
            if (IsRectSelecting)
            {
                origDownPos = downPos;
            }
            MyRefresh();
        }
        private bool IsPolySelectingClosed(Coord lastpoint)
        {
            if (deckSelectPolygon.Count <= 2) return false;
            Coord first = deckSelectPolygon.First();
            Coord delta = lastpoint - first;
            double x = Math.Abs(delta.X);
            double y = Math.Abs(delta.Y);
            //System.Diagnostics.Debug.Print("{0} - {1} = {2}", lastpoint.ToString(), first.ToString(), delta.ToString());
            return x < 10 && y < 10;
        }
        private void PolySelecting()
        {
            if (Operation != Operations.DECK_POLYGON)
                return;
            IsPolySelecting = true;
            Coord down = RestoreCoord(downPos);
            deckSelectPolygon.Add(down);
            if (deckSelectPolygon.Count <= 2)
            {
                MyRefresh();
                return;
            }

            Coord first = deckSelectPolygon.First();
            if (IsPolySelectingClosed(down))
            {
                deckSelectPolygon[deckSelectPolygon.Count-1] = first;
                IsPolySelecting = false;
            }
        }
        private void OnDeckMouseLeftDown(MouseEventArgs e)
        {
            EndRectSelecting();
            PolySelecting();
        }

        PointF MyCoordSys(PointF pt)
        {
            return new PointF(pt.X, ClientSize.Height - pt.Y - PAD_VERT*2);
        }

        Point MyCoordSys(Point pt)
        {
            return new Point(pt.X, ClientSize.Height - pt.Y - PAD_VERT*2);
        }

        #region - 内部处理 -
        bool isMoving = false;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point location = MyCoordSys(e.Location);

            cursorPos = ScrollPoint(e.Location);
            CheckFocus();
            //UpdateMousePos();
            if (isZooming)
            {
                zoomer.CalcZoomValue(e.Location);
                return;
            }
//             float x = Math.Abs(downPos.X - cursorPos.X);
//             float y = Math.Abs(downPos.Y - cursorPos.Y);
//             x += y;
            if (isDown/* && x > 5*/)
                isMoving = true;
            else
                isMoving = false;
            if (isDown && (Operation == Operations.SCROLL_FREE || Operation == Operations.ROOL_COUNT))
            {
                RoamFree();
                return;
            }
            if (compass.MoveOn(e.Location))
            {
                Cursor = Cursors.Hand;
                return;
            }
            if ((zoomer.MouseMove(e.Location) != SCALE_BUTTONS.NONE))
            {
                Cursor = Cursors.Hand;
                if( isDown )
                {
                    zoomer.HitTest(e.Location, true);
                }
                return;
            }
            
            CheckDeckFocus();
            RestoreCursor();
            MyRefresh();
        }
        private void OnCW(object sender, EventArgs e)
        {
            RotateDegrees += .1;
            UpdateGraphics();
        }
        private void OnCCW(object sender, EventArgs e)
        {
            RotateDegrees -= .1;
            UpdateGraphics();
        }
        bool isZooming = false;
        private void OnMouseLeftDown(MouseEventArgs e)
        {
            Point location = MyCoordSys(e.Location);
            if (isMenuDeckOn)
            {
                CloseDeckMenu();
                return;
            }
            isDown = true;
            downPos = cursorPos;

            if (compass.ClickOn(e.Location, true))
                return;
            SCALE_BUTTONS sb = zoomer.HitTest(e.Location, false);
            if ( sb != SCALE_BUTTONS.NONE)
            {
                if (sb == SCALE_BUTTONS.ZOOM_VALUE)
                {
                    isZooming = true;
                    zoomer.CalcZoomValue(e.Location);
                    OnZoomChange(null, null);
                }
                return;
            }
            if (Operation == Operations.DECK_RECT || Operation == Operations.DECK_POLYGON)
            {
                OnDeckMouseLeftDown(e);
                return;
            }
            MouseOperation(e);
        }
        private void OnMouseLeftUp(MouseEventArgs e)
        {
            Point location = MyCoordSys(e.Location);

            isDown = false;
            isZooming = false;
            if (compass.ClickOn(e.Location, false))
                return;

            if (isMoving)
                isMoving = false;
            else
            {
            }
//             RestoreOp();
        }
        private void OnMouseRightDown(MouseEventArgs e) 
        {
            Point location = MyCoordSys(e.Location);
            
            if (isMenuDeckOn)
                return;
            CancelRectSelecting();
            CancelPolySelecting();

        }
        private void OpenDeckMenu(Point where)
        {
            Models.Deck visible = layer.VisibleDeck;
            Models.Deck focus = layer.CurrentDeck;
            if (focus==null)
                return;
            Point pt = this.PointToScreen(where);
            isMenuDeckOn = true;
            miDeckName.Text = "\""+focus.Name+"\"";

            miActive.Checked = focus.IsVisible;
            miSkeleton.Checked = layer.CurrentDeck.IsDrawing(Models.DrawingComponent.SKELETON);
            miRollingCount.Checked = layer.CurrentDeck.IsDrawing(Models.DrawingComponent.BAND);
            miOverspeed.Checked = layer.CurrentDeck.IsDrawing(Models.DrawingComponent.OVERSPEED);
            miVehicleInfo.Checked = layer.CurrentDeck.IsDrawing(Models.DrawingComponent.VEHICLE);
            miArrows.Checked = layer.CurrentDeck.IsDrawing(Models.DrawingComponent.ARROWS);
            if (DM.DMControl.LoginControl.loginResult != DM.DB.LoginResult.ADMIN)
            {
                this.miDataMap.Enabled = false;
            }
            if (DM.DMControl.LoginControl.loginResult == DM.DB.LoginResult.VIEW)
            {
                this.miProperties.Enabled = false;
                this.miAssignment.Enabled = false;
                this.miStartDeck.Enabled = false;
                this.miEndDeck.Enabled = false;
                this.miDelete.Enabled = false;
                this.tmiNotRolling.Enabled = false;
            }

#if !DEBUG
            if (layer.CurrentDeck.WorkState != DM.DB.SegmentWorkState.WAIT)
            {
                this.tmiNotRolling.Enabled = false;
            }
#endif
            menuDeck.Show(pt);

            bool showStart = layer.CurrentDeck.IsVisible;
            bool showStop = layer.CurrentDeck.IsVisible;
            bool showPrandAs = layer.CurrentDeck.IsVisible;

            if( focus== null )
            {
                showStart = false;
                showStop = false;
            }
            else
            {
                showStart &= !layer.CurrentDeck.IsWorking;
                showStop &= layer.CurrentDeck.IsWorking;
            }

            if (DM.DMControl.LoginControl.loginResult != DM.DB.LoginResult.VIEW)
            {
                miStartDeck.Enabled = showStart;
                miEndDeck.Enabled = showStop;
                miProperties.Enabled = showPrandAs;
                miAssignment.Enabled = showPrandAs;
            }
        }
        private void CloseDeckMenu()
        {
            isMenuDeckOn = false;
            menuDeck.Hide();
        }
        bool isMenuDeckOn = false;
        private void OnMouseRightUp(MouseEventArgs e) 
        {
            if( Operation == Operations.ZOOM )
            {
                MouseOperation(e);
                return;
            }
            if( isMenuDeckOn )
            {
                CloseDeckMenu();
                return;
            }
            if (layer.FocusedDeck != null)
            {
                OpenDeckMenu(e.Location);
                //  菜单项处理在事件部分
                return;
            }
            MouseOperation(e);

        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            RoamStepping(0, e.Delta);
            MyRefresh();
        }
        private new bool OnKeyDown(KeyEventArgs e)
        {
            return ProcessKeys(e);
        }
        public bool ProcessKeys(KeyEventArgs e)
        {
            if( e.KeyCode == Keys.Escape )
            {
                if( isRectSelecting)
                {
                    // 取消选择
                    CancelPolySelecting();
                    CancelRectSelecting();
                    e.SuppressKeyPress = true;
                    return true;
                }
            }
            if( e.KeyCode == Keys.Space )
            {
            }
            /*
             * 左键 单击 切换可见

                开仓、关仓：CTRL+ALT+SHIFT+D
                删除仓面：delete

                轨迹：F5
                碾压编数：F6
                超速：F7
                碾压及：F8

                F：F9
                仓面信息：F10
                车辆安排：F11
            */
            switch (e.KeyCode)
            {
                case Keys.D:
                    if(e.Control && e.Shift && e.Alt)
                        if( layer.VisibleDeck != null )
                        {
                            Models.Deck dk = layer.VisibleDeck;
                            if (dk.IsWorking)
                                layer.DeckControl.Stop(dk);
                            else
                                layer.DeckControl.Start(dk);
                        }
                    return true;
                case Keys.F5:
                    miSkeleton_Click(null, null);
                    return true;
                case Keys.F6:
                    miRollingCount_Click(null, null);
                    return true;
                case Keys.F7:
                    miOverspeed_Click(null, null);
                    return true;
                case Keys.F8:
                    miVehicleInfo_Click(null, null);
                    return true;
                case Keys.F9:
                    tsReport_Click(null, null);
                    return true;
                case Keys.F10:
                    if(e.Control)
                    {
                        tmiNotRolling_Click(null, null);
                    }
                    else
                        miProperties_Click(null, null);
                    return true;
                case Keys.F11:
                    miVehicle_Click(null, null);
                    return true;
//                 case Keys.F12:
//                     return true;
            }

            return Forms.Main.MainWindow.ProcessKeys(this, e);
        }
        #endregion

        #region - 控件关联 -
        private void OnEnter(object sender, EventArgs e) { this.Focus();}
        private void LayerView_Leave(object sender, EventArgs e) { landscape.Visible = false; }

        private void OnKeyDown(object sender, KeyEventArgs e) 
        {
            if (OnKeyDown(e)) return;
            if( DM.Forms.Main.MainWindow.ProcessKeys(this, e)) return; 
        }
        private void OnKeyUp(object sender, KeyEventArgs e) { }
//        private void OnMouseMove(object sender, MouseEventArgs e) { cursorPos = ScrollPoint(e.Location);  OnMouseMove(e); }
        private void OnMouseEnter(object sender, EventArgs e) { }
        private void OnMouseLeave(object sender, EventArgs e) { }
        private void OnMouseDBClick(object sender, MouseEventArgs e)
        {
            if( e.Button == MouseButtons.Left )
            {
                isMoving = false;
                IsRectSelecting = false;
                IsPolySelecting = false;
                isDown = false;
                isMenuDeckOn = false;
                layer.SetActiveDeck();
                MyRefresh();
            }
        }
        private void OnMouseClick(object sender, MouseEventArgs e) { OnClick(e); }
        private void OnMouseRightClick(object sender, MouseEventArgs e) { }
        //protected override void OnMouseWheel(MouseEventArgs e) { OnMouseWheel(e); }
        private void OnMouseWheel(object sender, MouseEventArgs e) {}
        private void OnMouseDown(object sender, MouseEventArgs e) { if (e.Button == MouseButtons.Left) OnMouseLeftDown(e); if (e.Button == MouseButtons.Right) OnMouseRightDown(e); }
        private void OnMouseUp(object sender, MouseEventArgs e) { if (e.Button == MouseButtons.Left) OnMouseLeftUp(e); if (e.Button == MouseButtons.Right) OnMouseRightUp(e); }
        private void OnResize(object sender, EventArgs e) { }
        private void OnLoad(object sender, EventArgs e) { Init();  }
        private void LayerView_Enter(object sender, EventArgs e)
        {

        }
        #endregion

        public bool PreFilterMessage(ref Message msg)
        {
//             if (!this.Focused || isPreview)
//                 return false;
// 
//             const int WM_LBUTTONDOWN = 0x0201;
//             const int WM_LBUTTONUP = 0x0202;
//             const int WM_LBUTTONDBLCLK = 0x0203;
//             const int WM_RBUTTONDOWN = 0x0204;
//             const int WM_RBUTTONUP = 0x0205;
//             const int WM_RBUTTONDBLCLK = 0x0206;
//             const int WM_MBUTTONDOWN = 0x0207;
//             const int WM_MBUTTONUP = 0x0208;
//             const int WM_MBUTTONDBLCLK = 0x0209;
//             const int WM_KEYDOWN = 0x0100;
//             const int WM_KEYUP = 0x0101;
//             const int WM_MOUSEWHEEL = 0x020A;
//             const int WM_HSCROLL = 0x0114;
//             const int WM_VSCROLL = 0x0115;
//             //const int WM_CHAR = 0x0102;
//             if (msg.Msg == WM_MOUSEWHEEL)
//             {
//                 short delta = (short)(msg.WParam.ToInt32() >> 16);
//                 System.Diagnostics.Debug.Print("delta {0}", delta);
//                 RoamStepping(0, delta>0?1:-1);
//                 //Refresh();
//                 return true;
//             }
//             if (msg.Msg == WM_VSCROLL)
//                 System.Diagnostics.Debugger.Break();

/*            System.Diagnostics.Debug.Print("{0:X}", msg.Msg);*/
            //             Point pt = new Point(msg.LParam&0xFFFF, msg.LParam>>16);
//             Rectangle rc = this.RectangleToScreen(this.ClientRectangle);
//             rc.IsScreenVisible(pt);

            return false;
        }
        #endregion

        #region - 仓面菜单处理 -
        private void miProperties_Click(object sender, EventArgs e)
        {
            isMenuDeckOn = false;
//             if (layer.FocusedDeck == null)
//                 return;

            layer.ModifyCurrentDeck();
        }

        private void miVehicle_Click(object sender, EventArgs e)
        {
            isMenuDeckOn = false;
            layer.AssignVehicle();
        }

        private void miDelete_Click(object sender, EventArgs e)
        {
            isMenuDeckOn = false;
            layer.DeleteCurrentDeck();
        }

        private void miCancel_Click(object sender, EventArgs e)
        {
            isMenuDeckOn = false;
        }
        Forms.Waiting dlg = new DM.Forms.Waiting();
        private void ReportOK()
        {
            if (layer.VisibleDeck == null)
                return;
            if (layer.VisibleDeck.State == DM.DB.SegmentWorkState.END)
            {
                lock (updateLock)
                {
                    layer.CreateDataMap();
                }
            }
            bool result;
            lock (updateLock)
            {
                result = layer.VisibleDeck.CreateRollCountReport(zoom, false);
            }
            dlg.Finished = true;
            if (!result)
                return;
            System.IO.FileInfo fi = new System.IO.FileInfo(@"C:\output\"+DM.Models.Deck.I.DeckInfo.SegmentName+@"\"+Models.Deck.I.rolladdres);
            if ( result)
            {
 #if !DEBUG
                Utils.Sys.SysUtils.StartProgram(fi.FullName, null);
#else
                Utils.Sys.SysUtils.StartProgram(@"C:\output\" + Models.Deck.I.rolladdres, null);
#endif
            }

            //TrackGPS.WAITFINISHED = true;
        }
        private void tsReport_Click(object sender, EventArgs e)
        {
            isMenuDeckOn = false;
            if (layer.VisibleDeck == null)
                return;

            if(!Utils.MB.OKCancelQ("您确定生成图形报告吗？"))
            {
                return;
            }
            dlg.Dispose();
            dlg = new DM.Forms.Waiting();
            dlg.Start(this, "正在计算，请稍候……", ReportOK, 1000);
        }
        #endregion
        private void btnLandscape_Click(object sender, EventArgs e)
        {
            CheckScrollVisible();
        }

        private void miStartDeck_Click(object sender, EventArgs e)
        {
            isMenuDeckOn = false;
            if (layer.VisibleDeck != null)
                layer.DeckControl.Start(layer.VisibleDeck);
        }

        private void miEndDeck_Click(object sender, EventArgs e)
        {
            isMenuDeckOn = false;
            if (layer.VisibleDeck != null)
                layer.DeckControl.Stop(layer.VisibleDeck);
        }
        private void CheckMenu(ToolStripMenuItem mi, Models.DrawingComponent dc)
        {
            isMenuDeckOn = false;
            Models.Deck deck = layer.VisibleDeck;
            if (deck != null)
            {
                bool check = deck.IsDrawing(dc);
                deck.ShowDrawingComponent(dc, !check);
                Refresh();
                mi.Checked = !check;
            }
        }
        private void miSkeleton_Click(object sender, EventArgs e)
        {
            CheckMenu(miSkeleton, Models.DrawingComponent.SKELETON);
        }

        private void miOverspeed_Click(object sender, EventArgs e)
        {
            CheckMenu(miOverspeed, Models.DrawingComponent.OVERSPEED);
        }

        private void miRollingCount_Click(object sender, EventArgs e)
        {
            CheckMenu(miRollingCount, Models.DrawingComponent.BAND);
        }

        private void miActive_Click(object sender, EventArgs e)
        {
            isMenuDeckOn = false;
            //TrackGPS.WAITFINISHED = false;
            layer.SetActiveDeck();
        }

        private void miShowAll_Click(object sender, EventArgs e)
        {
            isMenuDeckOn = false;
            if (layer.VisibleDeck != null)
            {
                layer.VisibleDeck.ShowDrawingComponent(Models.DrawingComponent.ALL);
                Refresh();
                miSkeleton.Checked = miRollingCount.Checked = miVehicleInfo.Checked = miOverspeed.Checked = true;
            }
        }

        private void miVehicleInfo_Click(object sender, EventArgs e)
        {
            CheckMenu(miVehicleInfo, Models.DrawingComponent.VEHICLE);
        }

        private void miArrows_Click(object sender, EventArgs e)
        {
            CheckMenu(miArrows, Models.DrawingComponent.ARROWS);
        }

        private void miDataMap_Click(object sender, EventArgs e)
        {
            isMenuDeckOn = false;
            layer.CreateDataMap();
        }

        private void 测试TToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isMenuDeckOn = false;
            if(layer.VisibleDeck != null )
            {
                double x = layer.VisibleDeck.AverageHeightFromDataMap();
            }
        }

        private void tmiNotRolling_Click(object sender, EventArgs e)
        {
            isMenuDeckOn = false;
            if (layer.CurrentDeck == null)
                return;
            Forms.CoordsInput dlg = new DM.Forms.CoordsInput();
            dlg.Deck = layer.CurrentDeck.DeckInfo;
            DB.SegmentDAO dao = DB.SegmentDAO.getInstance();
            string vtx, cmt;
            if (!dao.ReadSegmentNotRolling(dlg.Deck.BlockID, dlg.Deck.DesignZ, dlg.Deck.SegmentID, out vtx, out cmt))
            {
                Utils.MB.Warning("数据库读取失败，请检查系统。");
                return;
            }
            string[] areas = vtx.Split('|');
            if(areas.Length<2)
            {
                string[] j = vtx.Trim().Split(';');
                vtx = string.Empty;
                for (int i = 0; i < j.Length - 2; i++)
                {
                    vtx += j[i].Trim() + ";";
                }
            }
            else
            {
                vtx = string.Empty;
                for(int k=0;k<areas.Length;k++)
                {
                    string[] j = areas[k].Trim().Split(';');
                    if (j.Length < 3)
                        continue;
                    for (int i = 0; i < j.Length - 2; i++)
                    {
                        vtx += j[i].Trim() + ";";
                    }
                    if (k != areas.Length-1)
                    vtx += "|";
                }
            }
            dlg.CoordsNoadd = vtx;
            dlg.Comments = cmt;
            //dlg.btnOK.Enabled = false;
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            layer.CurrentDeck.UpdateNRFromDB();
            layer.CreateScreen();
            MyRefresh();
        }

        private void miLookHistory_Click(object sender, EventArgs e)
        {
            layer.DeckControl.LookVehicleHistory(layer.VisibleDeck);
        }

        private void 生成压实厚度图TToolStripMenuItem_Click(object sender, EventArgs e)
        {
                dlg.Dispose();
                dlg = new DM.Forms.Waiting();
                dlg.Start(this, "正在计算，请稍候……", ReportThicknest, 1000);
        }

        private void ReportThicknest()
        {
            //Bitmap[] bp=DB.datamap.DataMapManager.draw(layer.VisibleDeck.DeckInfo.BlockID, layer.VisibleDeck.DeckInfo.DesignZ, layer.VisibleDeck.DeckInfo.SegmentID);
            Bitmap[] bp=DB.datamap.DataMapManager4.draw(layer.VisibleDeck.DeckInfo.BlockID, layer.VisibleDeck.DeckInfo.DesignZ, layer.VisibleDeck.DeckInfo.SegmentID);
            if (bp==null)
                Utils.MB.Warning("此仓面或者此仓面的下层仓面没有生成数据图，请确认这两个仓面都已在关仓状态出过图形报告！");
            else
            {
                Image image = (Image)bp[0];
                Image image2 = (Image)bp[1];
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(@"C:\OUTPUT\" + layer.VisibleDeck.DeckInfo.SegmentName);
                if (!di.Exists)
                {
                    di.Create();
                }
#if DEBUG
                image.Save(@"C:\OUTPUT\" + layer.VisibleDeck.Partition.Name + layer.VisibleDeck.Elevation.Height.ToString("0.0") + layer.VisibleDeck.ID.ToString() + "thickness.png");
                image2.Save(@"C:\OUTPUT\" + layer.VisibleDeck.Partition.Name + layer.VisibleDeck.Elevation.Height.ToString("0.0") + layer.VisibleDeck.ID.ToString() + "elevation.png");
#else
                image.Save(@"C:\OUTPUT\" + layer.VisibleDeck.DeckInfo.SegmentName.Trim() + @"\" + layer.VisibleDeck.Partition.Name + layer.VisibleDeck.Elevation.Height.ToString("0.0") + layer.VisibleDeck.ID.ToString() + "thickness.png");
                image2.Save(@"C:\OUTPUT\" + layer.VisibleDeck.DeckInfo.SegmentName.Trim() + @"\" + layer.VisibleDeck.Partition.Name + layer.VisibleDeck.Elevation.Height.ToString("0.0") + layer.VisibleDeck.ID.ToString() + "elevation.png");
#endif
                image.Dispose();
                image2.Dispose();
                System.IO.FileInfo fi = new System.IO.FileInfo(@"C:\OUTPUT\" + layer.VisibleDeck.DeckInfo.SegmentName.Trim() + @"\" + layer.VisibleDeck.Partition.Name + layer.VisibleDeck.Elevation.Height.ToString("0.0") + layer.VisibleDeck.ID.ToString() + "thickness.png");
                if (fi.Exists)
#if !DEBUG
                Utils.Sys.SysUtils.StartProgram(fi.FullName, null);
#else
                    Utils.Sys.SysUtils.StartProgram(@"C:\OUTPUT\" + layer.VisibleDeck.Partition.Name + layer.VisibleDeck.Elevation.Height.ToString("0.0") + layer.VisibleDeck.ID.ToString() + "thickness.png", null);
#endif
            }
            dlg.Finished = true;
        }
    }
}
