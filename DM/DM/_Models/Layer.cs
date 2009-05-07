using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using DM.Geo;
using System.IO;
using System.Xml.Serialization;

namespace DM.Models
{
    /// <summary>
    /// 层

    /// </summary>
    public class Layer: IDisposable
    {
        public event EventHandler OnMouseEnter;
        public event EventHandler OnMouseLeave;
        public event EventHandler OnMouseEnterDeck;
        public event EventHandler OnMouseLeaveDeck;

        public void Dispose()
        {
            DeckControl.Dispose();
            GC.SuppressFinalize(this);
        }
        public Layer()
        {
            Init();
        }
        private void Init()
        {
            dkcontrol = new DM.DMControl.DeckControl(this);
            mtx.boundary = new DMRectangle();
            mtx.boundary.Left = mtx.boundary.Top = float.MaxValue;
            mtx.boundary.Right = mtx.boundary.Bottom = float.MinValue;
            OnMouseEnter += Dummy;
            OnMouseLeave += Dummy;
            OnMouseEnterDeck += Dummy;
            OnMouseLeaveDeck += Dummy;
            mtx.zoom = 1;
        }
        private void Dummy(object sender, EventArgs e) {}
        public Layer(string _fullpath)
        {
            fullpath = _fullpath;
            Init();
        }
//         public Layer(List<Coord> vtx, string _fullpath) 
//         {
//             fullpath = _fullpath;
//             Init();
//             AddLayer(vtx);
//         }
        Partition part = null;
        Elevation ele;
        Views.LayerView owner = null;

        public Views.LayerView Owner
        {
            get { return owner; }
            set { owner = value; }
        }
        
        public Partition Partition { get { return part; } set { part = value; dkcontrol.Owner = this; } }
        public Elevation Elevation { get { return ele; } set { ele = value; dkcontrol.Owner = this; } }
        DMControl.DeckControl dkcontrol = new DM.DMControl.DeckControl(null);
        public DMControl.DeckControl DeckControl
        {
            get { return dkcontrol; }
//             set { dkcontrol = value; }
        }
        
        public static string Format(Partition p, Elevation e)
        {
            return string.Format("{0}-{1}米", p.Name, e.Height);
        }
        public override string ToString()
        {
            if (Partition == null || Elevation == null) return "NULL";
            return Format(this.Partition, this.Elevation);
        }
        [XmlIgnore]
        public string Name { get { return this.ToString(); } }
        public bool IsEqual(Layer l) { return IsEqual(l.Name); }
        public bool IsEqual(string n) { return Name.Equals(n, StringComparison.OrdinalIgnoreCase); }
        //////////////////////////////////////////////////////////////////////////////////
        List<Polygon> layers = new List<Polygon>();
        /*DeckCollection dkcontrol = new DeckCollection();*/
        string fullpath;
        public string FullPath { get { return fullpath; } set { fullpath = value; } }

        bool isMultiLayer = false;
        public bool IsMultiLayer { get { return isMultiLayer; } }

        public void Clear()
        {
            layers.Clear();
            dkcontrol.Decks.Clear();
            isMultiLayer = false;
        }
        public bool IsEmtpy()
        {
            return layers.Count == 0;
        }
        private void FilterIdentical(ref List<Coord> lst)
        {
            List<Coord> newlst = new List<Coord>();
            newlst.Capacity = lst.Count;
            for (int i = 0; i < lst.Count-1; i++ )
            {
                if( lst[i].IsEqual(lst[i+1]))
                {
                    // 这里千万不能用Remove(object)
                    // 否则会删错对象，期望删除第71，却删除了第一个

                    // 估计和Coord.Equals有关？（未重写）
                    lst.RemoveAt(i);
                    i--;
                }
            }
        }
        private void AddPolygon(List<Coord> batch, Partition p, Elevation e)
        {
            Polygon pl = new Polygon(batch);
            pl.Partition = p;
            pl.Elevation = e;
            Color line, fill;
            p.PredefinedColor(out line, out fill);
            pl.LineColor = line;
            pl.FillColor = fill;
            layers.Add(pl);

        }
        /// <summary>
        /// 仅添加数据，刷新图形，需要执行ConstructGraphics
        /// </summary>
        /// <param name="vertex"></param>
        public void AddLayer(List<Coord> vertex, Partition p, Elevation e, bool loadDB)
        {
            if (vertex == null)
                return;
            FilterIdentical(ref vertex);
            if (vertex.Count <=4 )  // 最少4点才能成为一个多边形，首尾点相同
                return;
            //layers.Clear();

            if (layers.Count != 0)
                isMultiLayer = true;
            else
            {
                // 第一次添加层信息
                // 尝试从数据库读取改层的仓面信息

                if (dkcontrol.Decks.Count == 0)
                {
                    dkcontrol.Owner = this;
                    if( loadDB )
                        dkcontrol.LoadDB(null);
                }
            }
//             mtx.boundary = new DMRectangle();
//             mtx.boundary.Left = mtx.boundary.Top = float.MaxValue;
//             mtx.boundary.Right = mtx.boundary.Bottom = float.MinValue;

            List<Coord> batch = new List<Coord>();
            bool first = true;
            for (int i = 0; i < vertex.Count; i++ )
            {
                Coord pt = vertex[i];

                // 计算边框
                mtx.boundary.Left = Math.Min(mtx.boundary.Left, pt.X);
                mtx.boundary.Right = Math.Max(mtx.boundary.Right, pt.X);
                mtx.boundary.Top = Math.Min(mtx.boundary.Top, pt.Y);
                mtx.boundary.Bottom = Math.Max(mtx.boundary.Bottom, pt.Y);

                batch.Add(pt);
                if (first)
                {
                    first = false;
                    continue;
                }
                // 如果发现和第一个点相同的点，说明图形封闭

                // 添加前面已经统计的点，并开启新统计
                if( pt.IsEqual(batch.First())  )
                {
                    AddPolygon(batch, p, e);
                    batch = new List<Coord>();
                    first = true;
                }
            }
            if (batch.Count != 0)
            {
                AddPolygon(batch, p, e);
            }

            mtx.at = mtx.boundary.LeftTop;//Transform.DamUtils.CenterPoint(mtx.boundary);
        }

        //////////////////////////////////////////////////////////////////////////////////
        #region 画图相关
        bool alwaysFitScreen = false;

        DMMatrix mtx = new DMMatrix();

        public DMMatrix DMMatrix
        {
            get { return mtx; }
            set { mtx = value; }
        }
        [XmlIgnore]
        public double Zoom { get { return mtx.zoom; } set { if (AlwaysFitScreen) return; mtx.zoom = value; } }
        double fitScreenZoom = 1.0;
        [XmlIgnore]
        public double FitScreenZoom { get { return fitScreenZoom; } }
        [XmlIgnore]
        public double RotateDegree { get { return mtx.degrees; } set { mtx.degrees = value; } }
        [XmlIgnore]
        public Coord RotateAt { get { return mtx.at; } set { mtx.at = value; } }
        [XmlIgnore]
        public bool AlwaysFitScreen { get { return alwaysFitScreen; } set { alwaysFitScreen = value; } }
        DMRectangle screenBoundary = new DMRectangle();
        [XmlIgnore]
        public Size VisibleSize { get { return new Size((int)screenBoundary.Width, (int)screenBoundary.Height); } }
        Rectangle canvas;
//         public void CreateScreen()
//         {
//             CreateScreen(canvas);
//         }
        public bool RectContains(Coord pt)
        {
            return mtx.boundary.Contains(pt.PF);
        }
        public void CreateScreen()
        {
            CreateScreen(canvas);
        }
        // 之前必须保证AddLayer已经调用
        public void CreateScreen(Rectangle rcClient)
        {
            canvas = rcClient;
            ResetBoundary();
            foreach(Polygon pl in layers)
            {
                pl.Token = true;
                pl.CreateScreen(mtx);
                FilterBoundary(pl.ScreenBoundary);
            }
            mtx.offset = new Coord(0, 0);
            CheckFitScreen();
            CheckVisible();
            if (AlwaysFitScreen)
                DoFitScreen();

            for (int i = 0; i < dkcontrol.Decks.Count; i++ )
            {
                Polygon pl = dkcontrol.Decks[i].Polygon;
                CreateDeckScreen(ref pl);
                foreach(Vehicle v in dkcontrol.Decks[i].VehicleControl.Vehicles)
                {
                    v.TrackGPSControl.Tracking.CreateScreen();
                }
                foreach (NotRolling nr in dkcontrol.Decks[i].NRs)
                {
                    nr.CreateScreen();
                }
            }
        }
        private void ResetBoundary()
        {
            screenBoundary.Left = screenBoundary.Top = float.MaxValue;
            screenBoundary.Right = screenBoundary.Bottom = float.MinValue;
        }
        private void FilterBoundary(DMRectangle rc)
        {
            // 计算变换后的边框
            screenBoundary.Left = Math.Min(screenBoundary.Left, rc.Left);
            screenBoundary.Top = Math.Min(screenBoundary.Top, rc.Top);
            screenBoundary.Right = Math.Max(screenBoundary.Right, rc.Right);
            screenBoundary.Bottom = Math.Max(screenBoundary.Bottom, rc.Bottom);
        }
        private void CheckVisible()
        {
//             if (screenBoundary.Left >= 0 && screenBoundary.Top >= 0)
//                 return;
            Coord offset = new Coord(0, 0);

            offset.X = -screenBoundary.Left;
            offset.Y = -screenBoundary.Top;

            if (screenBoundary.Width < canvas.Width)
            {
                offset.X += (canvas.Width - screenBoundary.Width) / 2;
            }
            if (screenBoundary.Height < canvas.Height)
                offset.Y += (canvas.Height - screenBoundary.Height) / 2;

            mtx.offset = offset;
            ResetBoundary();
            foreach (Polygon pl in layers)
            {
                pl.OffsetGraphics(mtx.offset);
                FilterBoundary(pl.ScreenBoundary);
            }
        }
        private void CheckFitScreen()
        {
//             if (!AlwaysFitScreen)
//                 return;

            double cwidth = canvas.Width;
            double cheight = canvas.Height;
            double swidth = screenBoundary.Width;
            double sheight = screenBoundary.Height;
            if (swidth == 0 || sheight == 0)
                return;
            double xzoom = cwidth / swidth;
            double yzoom = cheight / sheight;
            double zoom = Math.Min(xzoom, yzoom);
            fitScreenZoom = mtx.zoom * zoom;

            //System.Diagnostics.Debug.Print("Canvas {2} ScreenBoundary {0}, Zoom {1:0.000}", screenBoundary.ToString(), fitScreenZoom, canvas.ToString());
            //System.Diagnostics.Debug.Print("");
        }
        public void DoFitScreen()
        {
            ResetBoundary();
            mtx.zoom = FitScreenZoom;
            foreach (Polygon pl in layers)
            {
                pl.CreateScreen(mtx);
                FilterBoundary(pl.ScreenBoundary);
            }
            CheckVisible();
        }
        Deck focusedDeck = null;
        Deck currentDeck = null;
        [XmlIgnore]
        public Deck FocusedDeck { get { return focusedDeck; } }
        [XmlIgnore]
        public Deck CurrentDeck { get { if (currentDeck != null) return currentDeck; return VisibleDeck; } }

        public Deck VisibleDeck { get { return DeckControl.GetVisibleDeck(); } }

        // 主界面每次MouseMove都会调用Refresh
        // 所以每次OnPaint都会到这里

        Polygon lastHoverLayer = null;
        public void Draw(Graphics g, Coord scrCursor, bool autoCenter, bool frameonly, Font ft)
        {
            if (layers.Count == 0)
                return;

            Polygon thisHoverLayer = null;

            if (!autoCenter)
            {
                PointF offset = this.screenBoundary.LeftTop.PF;
                g.TranslateTransform(-offset.X, -offset.Y, MatrixOrder.Prepend);
            }
            foreach (Polygon pl in layers)
            {

                pl.Draw(g);
                if (pl.IsScreenVisible(scrCursor))
                {
                    thisHoverLayer = pl;
                }
                else
                {
                    if (lastHoverLayer == pl)
                        OnMouseLeave.Invoke(pl, null);
                }
            }

            // 倒序查找，保证最晚的仓面优先被选中
            Deck lastSelect = FocusedDeck;
            focusedDeck = null;
            for (int i = dkcontrol.Decks.Count - 1; i >= 0; i--)
            {
                Deck deck = dkcontrol.Decks[i];
                Polygon pl = deck.Polygon;
                if (pl.IsScreenVisible(scrCursor) && focusedDeck==null)
                {
                    deck.Polygon.LineDashStyle = DashStyle.Solid;
                    deck.Polygon.LineWidth = 4;
                    if (!deck.IsVisible)
                        deck.Polygon.FillColor = Color.FromArgb(0x80, Color.White);
                    focusedDeck = deck;
                    currentDeck = deck;
                }
                else
                {
                    deck.ResetStyles();
                    if (deck == lastSelect)
                        OnMouseLeaveDeck.Invoke(deck, null);
                }
            }

            // 顺序画仓面，保证最晚的仓面在最上

            foreach (Deck deck in dkcontrol.Decks)
            {
                deck.Draw(g, frameonly, ft);
            }

            // 层激活：thisHoverLayer != null
            // 仓激活：selectedDeck != null
            // 仓优先

            if (focusedDeck != null)
            {
                OnMouseLeave(lastHoverLayer, null);
                OnMouseEnterDeck(focusedDeck, null);
            }
            else
            {
                OnMouseEnter(thisHoverLayer, null);
            }
            lastHoverLayer = thisHoverLayer;


        }
        #endregion

        //////////////////////////////////////////////////////////////////////////////////
        public PointF DamToScreen(Coord c)
        {
            Coord pt = new Coord(c);

            pt.X -= mtx.at.X;
            pt.Y -= mtx.at.Y;
            pt.X *= Zoom;
            pt.Y *= Zoom;
            pt = Geo.DamUtils.RotateDegree(pt, new Coord(0, 0), mtx.degrees);
            pt.X += mtx.offset.X;
            pt.Y += mtx.offset.Y;

            return pt.PF;
        }
        public double ScreenSize(double realSize)
        {
            return realSize * mtx.zoom;
        }
        public Coord ScreenToDam(PointF pt)
        {
            Coord c = new Coord(pt);

            c.X -= mtx.offset.X;
            c.Y -= mtx.offset.Y;
            c = Geo.DamUtils.RotateRadian(c, new Coord(0,0), Geo.DamUtils.Degree2Radian(-mtx.degrees));
            c.X /= Zoom;
            c.Y /= Zoom;
            c.X += mtx.at.X;
            c.Y += mtx.at.Y;

            return c;
        }
        private void CreateDeckScreen(ref Polygon dk)
        {
            dk.CreateScreen(mtx);
            dk.OffsetGraphics(mtx.offset);
        }
        public void ScreenToDam(ref List<Coord> pts)
        {
            for (int i = 0; i < pts.Count; i++)
            {
                pts[i] = ScreenToDam(pts[i].PF);
            }
        }
        //List<Polygon> cutby = new List<Polygon>();

        #region 仓面管理
        public void ModifyCurrentDeck()
        {
            if (VisibleDeck != null)
                dkcontrol.ModifyDeck(VisibleDeck);
        }
        public void AssignVehicle()
        {
            if (VisibleDeck != null)
                VisibleDeck.VehicleControl.AssignVehicle(VisibleDeck);
        }
        private void AddNewDeck(Models.Deck dk)
        {
            dkcontrol.AddDeck(dk);
        }
        public void CutBy(Polygon scrCut)
        {
            bool shownWarning = false;
            foreach (Polygon pl in layers)
            {
                // 目前只允许对当前层面进行分仓处理
                // 其他层面只做视觉参考

                Polygon scrDeck = pl.CutBy(scrCut);
                if (scrDeck != null && (!pl.Partition.Equals(this.Partition) || !pl.Elevation.Equals(this.Elevation)))
                {
                    if (!shownWarning)
                    {
                        Utils.MB.Warning("只能对当前有效层\"" + this.ToString() + "\"进行分仓，\n其他层只做视觉参考，无法进行分仓。\n" +
                            "若要对该分区进行分仓，请打开对应分区的层。");
                        shownWarning = true;
                    }
                    continue;
                }
                if (scrDeck != null)
                {
                    List<Coord> lc = scrDeck.Vertex;
                    ScreenToDam(ref lc);
                    scrDeck.Vertex = lc;
                    Deck dk = new Deck();
                    scrDeck.UpdateBoundary();
                    dk.Polygon = scrDeck;
                    dk.Partition = pl.Partition;
                    dk.Elevation = pl.Elevation;
                    dk.Owner = this;
                    AddNewDeck(dk);
                    //cutby.SetVertex(cut);
                }
                if(isDeckInput)
                {
                    Polygon DamDeck = pl.CutByOfEarth(scrCut);
                    if (DamDeck != null)
                    {
                        Deck dk = new Deck();
                        DamDeck.UpdateBoundary();
                        dk.Polygon = DamDeck;
                        dk.Partition = pl.Partition;
                        dk.Elevation = pl.Elevation;
                        dk.Owner = this;
                        AddNewDeck(dk);
                        isDeckInput = false;
                    }
                }
            }
            CreateScreen(canvas);
        }
        public bool isDeckInput = false;
        public void DeleteCurrentDeck()
        {
            if (VisibleDeck == null)
                return;
            dkcontrol.DeleteDeck(VisibleDeck);
            focusedDeck = null;
            currentDeck = null;
        }
        #endregion

        public int RollCount(PointF pt)
        {
            if (!this.screenBoundary.Contains(pt))
                return 0;
            return dkcontrol.RollCount(pt);
        }
        public void SetActiveDeck()
        {
            if (CurrentDeck == null)
                return;
            if (CurrentDeck.IsVisible)
                return;
            DeckControl.SetVisibleDeck(CurrentDeck);
        }
        public void HideActiveDeck()
        {
            if (CurrentDeck == null)
                return;
            if (!CurrentDeck.IsVisible)
                return;
            DeckControl.UnvisibleDeck(CurrentDeck);
        }
        public void CreateDataMap()
        {
            if (VisibleDeck == null)
                return;
            if (DeckControl.CreateDataMap(VisibleDeck))
            {
                //Utils.MB.OK("数据图更新成功！");
            }
            else
            {
                Utils.MB.OK("数据图更新失败！");
            }
        }
    }
}
