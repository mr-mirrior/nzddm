using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace DM.Models
{
    public enum DrawingComponent
    {
        NONE = 0,
        SKELETON = 1,
        BAND = 2,
        OVERSPEED = 4,
        VEHICLE = 8,
        ARROWS = 16,
        ALL = SKELETON/*|BAND*/| OVERSPEED | VEHICLE/*|ARROWS*/
    }
    /// <summary>
    /// 仓面
    /// </summary>

    public class Deck : IDisposable
    {
        bool isVisible = false;
        double thickness = 0;
        const float baseValue = 19592.93f;
        string orignCoordString;
        Font ftScale;
        Font ft;
        Font ftString;
        Geo.Coord DamOrignCoord;
        List<NotRolling> nrs = new List<NotRolling>();

        public List<NotRolling> NRs
        {
            get { return nrs; }
            set { nrs = value; }
        }

        public DateTime DateStart
        {
            get { return deckInfo.StartDate; }
            set { deckInfo.StartDate = value; }
        }

        public DateTime DateEnd
        {
            get { return DeckInfo.EndDate; }
            set { deckInfo.EndDate = value; }
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; ResetStyles(); }
        }
        public static Deck thisdk;
        public static Deck I { get { return thisdk; } }
        // 厚度监控，-1表示不监控
        public double Thickness { get { return thickness; } set { thickness = value; } }

        #region - 数据库相关 -
        Layer owner = null;
        //         private Partition partition;
        //         private Elevation elevation;
        public Deck(DB.Segment seg) { this.DeckInfo = seg; Init(); }
        public Partition Partition
        {
            get { return DMControl.PartitionControl.FromID(deckInfo.BlockID); }
            set { deckInfo.BlockID = value.ID; UpdateName(); }
        }
        public Elevation Elevation { get { return new Elevation(deckInfo.DesignZ); } set { deckInfo.DesignZ = value.Height; UpdateName(); deckInfo.StartZ = value.Height; deckInfo.SpreadZ = value.Height + 2; } }
        public string Name { get { return deckInfo.SegmentName; } set { deckInfo.SegmentName = value; } }
        public int ID { get { return deckInfo.SegmentID; } set { deckInfo.SegmentID = value; UpdateName(); } }
        public DB.SegmentWorkState State { get { return deckInfo.WorkState; } set { deckInfo.WorkState = value; } }
        private DB.Segment deckInfo = new DM.DB.Segment();
        public DB.Segment DeckInfo { get { return deckInfo; } set { deckInfo = value; ParsePolygon(); } }
        public DB.SegmentWorkState WorkState { get { return DeckInfo.WorkState; } }
        public bool IsWorking { get { return WorkState == DM.DB.SegmentWorkState.WORK; } }
        public void UpdateName()
        {
            //             if (Name == null || Name.Length == 0)
            //             {
            //                 Name = this.ToString();
            //             }
            if( Partition.ID != 0 )
                deckInfo.BlockName = Partition.Description; 
        }
        // 仓面顶点坐标格式化字符串，准备入库

        public string VertexString()
        {
            string s = "";
            foreach (Geo.Coord c in Polygon.Vertex)
            {
                s += string.Format("{0:0.00},{1:0.00};", c.X, c.Y);
            }
            return s;
        }
        public string Status
        {
            get
            {
                string status = "【状态】";
                if (IsVisible)
                    status += "可见，";
                else
                    status += "不可见，";
                switch (DeckInfo.WorkState)
                {
                    case DM.DB.SegmentWorkState.WAIT:
                        status += "尚未开仓";
                        break;
                    case DM.DB.SegmentWorkState.END:
                        status += "已经关仓";
                        break;
                    case DM.DB.SegmentWorkState.WORK:
                        status += "已开仓，正在施工";
                        break;
                    default:
                        return "";
                }
                return status;
            }
        }
        // 从数据库读出仓面顶点坐标
        private void ParsePolygon()
        {
            vertex = new Polygon();
            string s = DeckInfo.Vertext;
            // x1,y1;x2,y2;...
            string[] coord = s.Split(new char[] { ';' });
            if (coord.Length == 0)
                return;
            List<Geo.Coord> lst = new List<DM.Geo.Coord>();
            foreach (string c in coord)
            {
                if (c == null || c.Length == 0)
                    continue;
                string[] d = c.Split(new char[] { ',' });
                if (d.Length == 2)
                {
                    lst.Add(new DM.Geo.Coord(double.Parse(d[0]), double.Parse(d[1])));
                }
            }

            vertex.SetVertex(lst);
        }
        #endregion

        // 安排了多个车辆

        DMControl.VehicleControl vCtrl = new DM.DMControl.VehicleControl();
        public DMControl.VehicleControl VehicleControl { get { return vCtrl; } set { vCtrl = value; } }

        //         public Deck(DB.Segment seg)
        //         {
        //             this.Partition = DMControl.PartitionControl.FromID(seg.BlockID);
        //             this.Elevation = new Elevation(seg.DesignZ);
        //             this.ID = seg.SegmentID;
        //             ParsePolygon(seg.Vertext);
        //         }
        public Deck() { InitDefault(); }
        private void InitDefault()
        {
            deckInfo.DesignRollCount = 0;
            deckInfo.ErrorParam = 10;
            deckInfo.MaxSpeed = 0; // km/hr
        }
        private void Init()
        {
            vCtrl.Owner = this;
        }
        public void Dispose()
        {
            this.VehicleControl.Dispose();
            GC.SuppressFinalize(this);
        }
        //string name;
        [XmlIgnore]
        public Layer Owner { get { return owner; } set { owner = value; if (owner.Partition != null) Partition = value.Partition; if (owner.Elevation != null) Elevation = value.Elevation; } }
        public bool IsEqual(Deck dk)
        {
            return this.Partition.Equals(dk.Partition) && this.Elevation.Height == dk.Elevation.Height && this.ID == dk.ID;
        }

        private void InitColor()
        {
            //cl = owner.Partition.FillColor;
            //vertex.Fill.Color = cl;
            //Partition.PredefinedColor(out lineColor, out fillColor);
            ResetStyles();
        }
        public override string ToString()
        {
            return string.Format("仓面{2} {0}-{1}米", Partition.Name, Elevation.Height, ID + 1);
        }
        Color fillColor = Color.Silver;
        Color lineColor = Color.Black;
        [XmlIgnore]
        public Color FillColor { get { return fillColor; } set { if (!IsVisible) fillColor = value; } }
        [XmlIgnore]
        public Color LineColor { get { return lineColor; } set { lineColor = value; } }

        public void ResetStyles()
        {
            vertex.AntiAlias = true;
            vertex.FillColor = Color.FromArgb(0x80, fillColor);
            vertex.LineColor = lineColor;
            vertex.LineColor = Color.FromArgb(0xFF, Color.Black);
            vertex.LineWidth = 2f;
            vertex.LineDashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            vertex.LineDashPattern = new float[] { 6, 4 };

            if (this.DeckInfo.IsFinished())
            {
                vertex.LineColor = Color.DimGray;
                vertex.LineDashStyle = DashStyle.Solid;
            }
            if (this.DeckInfo.IsWorking())
            {
                vertex.LineColor = Color.BlueViolet;
                vertex.LineDashStyle = DashStyle.Solid;
            }
            if (this.IsVisible)
                vertex.FillColor = Color.FromArgb(0xa0, Color.White);
        }
        Polygon vertex = new Polygon();
        public Polygon Polygon
        {
            get { return vertex; }
            set
            {
                vertex = value;
                ResetStyles();
                deckInfo.Vertext = VertexString();
            }
        }
        public bool IsScreenVisible(PointF scrPoint)
        {
            foreach (NotRolling nr in nrs)
            {
                if (nr.IsScreenVisible(scrPoint))
                    return false;
            }
            return Polygon.IsScreenVisible(new Geo.Coord(scrPoint));
        }
        public bool RectContains(Geo.Coord pt)
        {
            return Polygon.Boundary.Contains(pt.PF);
        }
        DrawingComponent drawingComponent = DrawingComponent.ALL;
        public DrawingComponent DrawingComponent { get { return drawingComponent; } set { drawingComponent = value; } }
        public void ShowDrawingComponent(DrawingComponent dc)
        {
            drawingComponent |= dc;
        }
        public void HideDrawComponent(DrawingComponent dc)
        {
            drawingComponent &= ~dc;
        }
        public void ShowDrawingComponent(DrawingComponent dc, bool show)
        {
            if (show)
                ShowDrawingComponent(dc);
            else
                HideDrawComponent(dc);
        }
        public bool IsDrawing(DrawingComponent dc) { return 0 != (drawingComponent & dc); }
        public void Draw(Graphics g, bool frameonly, Font ft)
        {
            this.ft = ft;
            this.Polygon.Draw(g);
            foreach (NotRolling nr in nrs)
            {
                nr.Draw(g, ft, true);
            }

            if (!IsVisible)
                return;
            if (frameonly && IsDrawing(DrawingComponent.SKELETON))
            {
                foreach (Vehicle v in vCtrl.Vehicles)
                {
                    v.Draw(g, frameonly);
                }
                return;
            }
            foreach (Vehicle v in vCtrl.Vehicles)
            {
                if (IsDrawing(DrawingComponent.OVERSPEED))
                    v.TrackGPSControl.Tracking.DrawOverSpeed = true;
                else
                    v.TrackGPSControl.Tracking.DrawOverSpeed = false;

                //v.TrackGPSControl.Tracking.Draw(g, true);
                if (IsDrawing(DrawingComponent.SKELETON))
                    v.TrackGPSControl.Tracking.DrawSkeleton(g, IsDrawing(DrawingComponent.ARROWS));
                if (IsDrawing(DrawingComponent.BAND))
                    v.Draw(g, false);
            }
            if (IsDrawing(DrawingComponent.VEHICLE))
                foreach (Vehicle v in vCtrl.Vehicles)
                {
                    v.TrackGPSControl.Tracking.DrawAnchor(g);
                }
        }
        /// <summary>
        /// 绘制碾压编数效果图，放大率等数据取屏幕当前设置值
        /// </summary>
        /// <param name="areas">返回不同碾压编数的总和点（面积）</param>
        /// <returns>返回图形，用完后必须bmp.Dispose();</returns>
        public Bitmap CreateRollCountImage(out int[] areas)
        {
            Layer layer = this.owner;
            Polygon pl = this.Polygon;
            Bitmap learning = new Bitmap(layersColor.Length * 2, 1, PixelFormat.Format32bppPArgb);
            Graphics gLearning = Graphics.FromImage(learning);
            gLearning.Clear(layersColor[0]);
            for (int i = 0; i < learning.Width; i++)
            {
                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddLine(learning.Width, 0, i + 1, 0);
                    using (Pen p = new Pen(Color.FromArgb(0x20, Color.Navy)))
                        gLearning.DrawPath(p, gp);
                }
            }
            int[] colorDict = new int[learning.Width];
            for (int i = 0; i < learning.Width; i++)
            {
                colorDict[i] = learning.GetPixel(i, 0).ToArgb();
            }
            //learning.Save(@"C:\learning.png", System.Drawing.Imaging.ImageFormat.Png);


            Bitmap bmp = new Bitmap((int)Math.Ceiling(pl.ScreenBoundary.Width), (int)Math.Ceiling(pl.ScreenBoundary.Height), PixelFormat.Format32bppPArgb);
            Graphics g = Graphics.FromImage(bmp);

            g.Clear(Color.White);
            // g.DrawImage(learning, 0, 0);
            g.TranslateTransform((float)-pl.ScreenBoundary.Left, (float)-pl.ScreenBoundary.Top);
            pl.AntiAlias = false;
            pl.LineColor = Color.Transparent;
            pl.FillColor = layersColor[0];
            pl.Draw(g);
            pl.SetDrawClip(g);

            for (int k = 0; k < VehicleControl.Vehicles.Count; k++)
            {
                Vehicle v = VehicleControl.Vehicles[k];
                if (v.Info == null)
                    continue;
                TrackGPS gps = v.TrackGPSControl.Tracking;
                Color oldcl = gps.Color;
                gps.Color = Color.Navy;
                gps.Draw(g, false);
                gps.Color = oldcl;
            }

            for (int i = 0; i < nrs.Count; i++)
            {
                bool aa = nrs[i].Vertex.AntiAlias;
                nrs[i].Vertex.AntiAlias = false;
                Color c1 = nrs[i].Vertex.LineColor;
                Color c2 = nrs[i].Vertex.FillColor;
                nrs[i].Vertex.LineColor =
                    nrs[i].Vertex.FillColor = learning.GetPixel(this.DeckInfo.DesignRollCount, 0);//layersColor[this.DeckInfo.DesignRollCount];//////////这里应该是改为蓝色的遍数
                nrs[i].Draw(g, ft, false);
                nrs[i].Vertex.LineColor = c1;
                nrs[i].Vertex.FillColor = c2;
                nrs[i].Vertex.AntiAlias = aa;
            }

            gLearning.Dispose();
            learning.Dispose();
            //bmp.Save(@"C:\debug.png", System.Drawing.Imaging.ImageFormat.Png);

            Bitmap output = (Bitmap)bmp.Clone();
            bmp.Dispose();

            BitmapData bd = output.LockBits(new Rectangle(0, 0, output.Width, output.Height), ImageLockMode.ReadWrite, output.PixelFormat);
            areas = new int[layersColor.Length];

            //             FileStream fs = null;
            //             BinaryWriter sw = null;
            //             if (datamap)
            //             {
            //                 fs = new FileStream(@"C:\Data.map", FileMode.OpenOrCreate);
            //                 sw = new BinaryWriter(fs);
            //                 sw.Write((short)output.Width);
            //                 sw.Write((short)output.Height);
            //             }

            unsafe
            {
                int* pp = (int*)bd.Scan0.ToPointer();
                pp = (int*)bd.Scan0.ToPointer();
                for (int i = 0; i < bd.Height; i++)
                {
                    for (int j = 0; j < bd.Width; j++)
                    {
                        Int32 cl = *(pp + j);
                        //                         if (cl != Color.White.ToArgb())
                        //                             System.Diagnostics.Debugger.Break();
                        int cl_idx = -1; //color_dict.IndexOf(cl);
                        for (int k = 0; k < colorDict.Length; k++) { if (colorDict[k] == cl) { cl_idx = k; break; } }
                        if (cl_idx == 0)
                        {
                            *(pp + j) = 0;
                            areas[cl_idx]++;
                            continue;
                        }
                        else if (cl_idx == -1) // 碾压超过限度的
                        {
                            if (cl == -1) // 该点不在仓面内
                            {
                                *(pp + j) = 0;
                                //                                 if (datamap)
                                //                                 {
                                //                                     sw.Write((byte)0xff);
                                //                                     sw.Write(569.2f);
                                //                                 }
                                //areas[0]++;
                                continue;
                            }
                            else
                                cl_idx = colorDict.Length - 1;
                        }

                        cl_idx = Math.Min(cl_idx, layersColor.Length - 1);
                        int last_idx;
                        last_idx = Math.Min(cl_idx, this.DeckInfo.DesignRollCount);

                        areas[cl_idx]++;
                        if(isDatamap)
                            *(pp + j) = layersColor[cl_idx].ToArgb();
                        else
                            *(pp + j) = layersColor[last_idx].ToArgb();
                        //                         if (datamap)
                        //                         {
                        //                             sw.Write((byte)cl_idx);
                        //                             sw.Write(569.2f);
                        //                         }
                    }
                    pp += bd.Stride / 4;
                }
            }
            //填充数据库总面积和碾压遍数百分比字段
            double[] areasPercents = Deck.AreaRatio(areas, this);
            string percentages, one;
            percentages = string.Empty;
            for (int i = 0; i < 15; i++)
            {
                if (i < areasPercents.Length)
                    one = areasPercents[i].ToString("0.0000");
                else
                    one = ((double)0).ToString("0.0000");
                if (i != 14)
                    percentages = percentages + one + ",";
                else
                    percentages += one;
            }
            DB.SegmentDAO.getInstance().UpdateSegmentAreaAndRollingPercentages(Partition.ID, Elevation.Height, ID, Polygon.ActualArea, percentages);
            //// 超过部分统一
            if (!isDatamap)
            {
                for (int idx = this.DeckInfo.DesignRollCount + 1; idx < areas.Length; idx++)
                {
                    areas[this.DeckInfo.DesignRollCount] += areas[idx];
                    areas[idx] = 0;
                }
            }
            //*/
            //             if (datamap)
            //             {
            //                 sw.Close();
            //                 fs.Close();
            //             }
            output.UnlockBits(bd);
            g.Dispose();

            g = Graphics.FromImage(output);
            g.TranslateTransform((float)-pl.ScreenBoundary.Left, (float)-pl.ScreenBoundary.Top);
            pl.SetDrawClip(g);
            Font fft;
            try
            {
                fft = new Font(new FontFamily("微软雅黑"), 3 * (float)owner.ScreenSize(0.356), GraphicsUnit.Pixel);
            }
            catch
            {
                fft = new Font(new FontFamily("宋体"), 3 * (float)owner.ScreenSize(0.356), GraphicsUnit.Pixel);
            }
            
            for (int i = 0; i < nrs.Count; i++)
            {
//                 bool aa = nrs[i].Vertex.AntiAlias;
//                 nrs[i].Vertex.AntiAlias = false;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Color c1 = nrs[i].Vertex.LineColor;
                Color c2 = nrs[i].Vertex.FillColor;
                nrs[i].Vertex.LineColor = Color.Black;
                nrs[i].Vertex.FillColor = Color.White;
                nrs[i].Draw(g, fft, true);
                nrs[i].Vertex.LineColor = c1;
                nrs[i].Vertex.FillColor = c2;
//                 nrs[i].Vertex.AntiAlias = aa;
            }

            //output.Save(@"C:\debug.png", System.Drawing.Imaging.ImageFormat.Png);
            
            return output;

        }
        Color[] layersColor = new Color[] {
                            Color.LightYellow,
                            Color.Thistle,
                            Color.Pink,
                            Color.Gray,
                            Color.Orange,
                            Color.CornflowerBlue,
                            Color.Cyan,
                            Color.Chocolate,
                            Color.Green,
                            Color.Red,
                            Color.Purple,
                            Color.Blue,
                            Color.SlateGray,
                            Color.Indigo,
                            Color.Aqua
                        };
        public bool isDatamap = false;
        public unsafe byte[] CreateDatamap()
        {
            /*
             * 数据图格式：
             * int32 width in pixel
             * int32 height in pixel
             * byte rollcount0
             * float height0 (4 bytes)
             * byte rollcount1
             * float height1 (4 bytes)
             * ...
             */
            isDatamap = true;
            Layer layer = owner;
            double oldZoom = layer.Zoom;
            double oldRotate = layer.RotateDegree;

            layer.Zoom = 10;
            layer.RotateDegree = 0;
            layer.CreateScreen();

            double lo, hi;
            int[] areas = null;
            Bitmap roll = CreateRollCountImage(out areas);
            Bitmap elev = ElevationImage(out lo, out hi);

            //roll.Save(@"C:\pngroll.png");
            //roll.Save(@"C:\roll.png", System.Drawing.Imaging.ImageFormat.Png);
            //elev.Save(@"C:\elev.png", System.Drawing.Imaging.ImageFormat.Png);
            isDatamap = false;
            if (roll == null || elev == null )//|| roll.Width != elev.Width || roll.Height != elev.Height)
            {
                return null;
            }
            int width =Math.Min(roll.Width,elev.Width); //roll.Width;
            int height = Math.Min(roll.Height, elev.Height); //roll.Height;

            BitmapData dataRoll, dataElev;
            Rectangle rc = new Rectangle(0, 0, width, height);
            dataRoll = roll.LockBits(rc, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            dataElev = elev.LockBits(rc, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] datamap = new byte[4+4+5*width*height]; // width, height, 5,5,5,5...
            fixed(byte* orig = datamap)
            {
                int* intp = (int*)orig;
                *intp = width;
                intp++;
                *intp = height;
                byte* p = (orig + 8);
                byte* rp = (byte*)dataRoll.Scan0;
                byte* ep = (byte*)dataElev.Scan0;
                for (int i = 0; i < height; i++ )
                {
                    for (int j = 0; j < width; j++ )
                    {
                        // 碾压编数
                        int count = -1;
                        int color = *((int*)(rp+j*4));
                        if( (color&0xFF000000) != 0 )   // 透明？
                        {
                            for (int k = 0; k < layersColor.Length; k++)
                            {
                                if (color == layersColor[k].ToArgb())
                                {
                                    count = k;
                                    //DM.DB.DebugUtil.fileLog(k.ToString());
                                    break;
                                }
                            }
                        }
                        *p++ = (byte)count;
                        
                        // 高程
                        float elevation = 0;
                        color = *((int*)(ep + j * 4));
                        if((color&0xFF000000)!=0)
                        {
                            color &= 0xFF; // 灰度图RGB相同，这里取蓝色分量
                            elevation = (float)( ((float)color / 255) * this.DeckInfo.DesignDepth + this.DeckInfo.StartZ );
                        }
                        *((float*)p) = elevation;
                        p += sizeof(float);
                        //if (count != -1 && elevation == 0)
                        //    elevation = 1;
                    }
                    rp += dataRoll.Stride;
                    ep += dataElev.Stride;
                }
            }
            roll.UnlockBits(dataRoll);
            elev.UnlockBits(dataElev);

            roll.Dispose();
            elev.Dispose();
            layer.Zoom = oldZoom;
            layer.RotateDegree = oldRotate;
            layer.CreateScreen();

            return datamap;
        }

        public string rolladdres = string.Empty;
        public static double[] AreaRatio(int[] areas, Deck dk)
        {
            if (areas == null || dk == null)
                return null;
            int okcount =/*dk.DeckInfo.DesignRollCount;*/areas.Length;
            double[] area_ratio = new double[okcount + 1];
            double total = 0;
            for (int i = 0; i < areas.Length; i++)
            {
                total += areas[i];
            }
            if (total == 0)
                return null;
            for (int i = 0; i < okcount; i++)
            {
                area_ratio[i] = areas[i] / total;
            }
            for (int i = okcount; i < areas.Length; i++)
            {
                area_ratio[okcount] += areas[i] / total;
            }
            return area_ratio;
        }
        public bool CreateRollCountReport(double zoom, bool datamap)
        {
            Layer layer = owner;
            double oldZoom = layer.Zoom;
            double oldRotate = layer.RotateDegree;

#if !DEBUG
            if (WorkState== DM.DB.SegmentWorkState.WAIT)//this.IsWorking||或结束关仓后
            {
                Utils.MB.Warning("该仓面没有开仓，无法生成图形报告。请再试一次。");
                return false;
            }
#endif
            
            //try
            {
                Brush[] bs = new SolidBrush[layersColor.Length];
                for (int i = 0; i < bs.Length; i++)
                {
                    bs[i] = new SolidBrush(layersColor[i]);
                }
                const double SHANGYOU = -146.568450927734;
                if (!datamap)
                {
                    layer.RotateDegree = SHANGYOU;
                }
                else
                {
                    layer.Zoom = 2;
                    layer.RotateDegree = 0;
                }

                layer.CreateScreen();
                Polygon pl = this.Polygon;
                if (pl.ScreenBoundary.Width > 5000 || pl.ScreenBoundary.Height > 5000)
                {
                    layer.RotateDegree = oldRotate;
                    layer.Zoom = oldZoom;
                    layer.CreateScreen();
                    Utils.MB.Warning("放大率过大，请缩小图形后再试一次");
                    return false;
                }
                DirectoryInfo di = new DirectoryInfo(@"C:\OUTPUT\"+this.DeckInfo.SegmentName);
                if (!di.Exists)
                {
                    di.Create();
                }
                thisdk = this;
                if (pl.ScreenBoundary.Width == 0 || pl.ScreenBoundary.Height == 0)
                    return false;

                int[] areas;
                Bitmap output = CreateRollCountImage(out areas);

                DB.datamap.DAO.getInstance().updateRollBitMap(deckInfo.BlockID, deckInfo.DesignZ, deckInfo.SegmentID, DB.datamap.DAO.getInstance().ToByte(output));
#if DEBUG
                output.Save(@"C:\OUTPUT\" + this.Partition.Name + this.Elevation.Height.ToString("0.0") + this.ID.ToString() + "OrignRoll.png", System.Drawing.Imaging.ImageFormat.Png);
#endif

                //output.Save("C:\\1.png");
                areaScale = new double[areas.Length];

                float factor;
                factor = GetMultipleFactor(layer, 3.56);


                //求原点坐标
                Geo.Coord screenOriginCoord = pl.ScreenBoundary.LeftBottom;
                Geo.Coord earthOriginCoord = layer.ScreenToDam(screenOriginCoord.PF);
                DamOrignCoord = earthOriginCoord.ToDamAxisCoord();
                orignCoordString = "(" + DamOrignCoord.X.ToString("0.00") + ", " + DamOrignCoord.Y.ToString("0.00") + ")";

                float offsetx;
                float offsety;
                float offset;

                offsetx = output.Width * 1.2f;
                offsety = output.Width / 6 * 0.5f * 0.5f * 7f + output.Height; //output.Height + 120 * (int)Math.Ceiling(factor);
                offset = (offsetx - output.Width) / 2;


                Bitmap newBmp = new Bitmap((int)offsetx, (int)offsety);//output.Height + 120 * (int)Math.Ceiling(factor));//60,80                
                Graphics newG = Graphics.FromImage(newBmp);

                float newH = output.Width / 6 * 0.5f * 0.5f;

                newG.Clear(Color.White);

                newG.SmoothingMode = SmoothingMode.AntiAlias;
                newG.InterpolationMode = InterpolationMode.HighQualityBicubic;
                newG.TranslateTransform((float)-pl.ScreenBoundary.Left + (offsetx - output.Width) / 2, (float)-pl.ScreenBoundary.Top + newH);
                this.ResetStyles();
                pl.Draw(newG);
                newG.ResetTransform();
                newG.DrawImageUnscaled(output, (int)offset, (int)newH);

                Pen newPen = new Pen(Brushes.Black, 1);
                newPen.CustomEndCap = new AdjustableArrowCap(2, 8, true);
                ft = new Font("微软雅黑", 7.5f * factor, GraphicsUnit.Pixel);
                ftScale = new Font("微软雅黑", 5.5f * factor);

                ftString = new Font("微软雅黑", 7.5f * factor, FontStyle.Bold, GraphicsUnit.Pixel);


                float multiple = output.Width / 6;
                float w0 = multiple * 0.5f;
                float fa = 10f;

                SizeF s = newG.MeasureString("100.00%", ftScale);

                while (s.Width > (multiple - w0) * 0.9f)
                {
                    if (fa * factor < 0)
                        return false;

                    ftScale = new Font("微软雅黑", fa * factor);
                    s = newG.MeasureString("100.99%", ftScale);
                    fa = fa - 0.1f;
                }
                s = newG.MeasureString("未碾压", ft);
                fa = 10f;
                while (s.Width > (multiple - w0) * 0.9f)
                {
                    ft = new Font("微软雅黑", fa * factor);
                    ftString = new Font("微软雅黑", fa * factor);
                    s = newG.MeasureString("未碾压", ftString);
                    fa = fa - 0.1f;
                }

                //横轴
                newG.DrawLine(newPen, new PointF(offset - 4, (float)this.Polygon.ScreenBoundary.Height + newH), new PointF((float)this.Polygon.ScreenBoundary.Width + offset, (float)this.Polygon.ScreenBoundary.Height + newH));
                newG.DrawString("坝(m)", ftScale, Brushes.Black, (float)this.Polygon.ScreenBoundary.Width + offset * 1f, (float)this.Polygon.ScreenBoundary.Height + newH);

                //纵轴
                newG.DrawLine(newPen, new PointF(offset, (float)this.Polygon.ScreenBoundary.Height + newH + 4), new PointF(offset, 2 * factor));
                //newG.DrawString("轴(m)", ftScale, Brushes.Black, offset * 0.9f, 0 * factor);

                //原点坐标
                newG.DrawString(orignCoordString, ftScale, Brushes.Black, offset * 0.8f, (float)this.Polygon.ScreenBoundary.Height + newH + 2);
                newPen.Dispose();

                int okcount = this.DeckInfo.DesignRollCount;
                double[] area_ratio = AreaRatio(areas, this);

                for (int i = 0; i < 6; i++)
                {
                    if (i > okcount)
                        continue;
                    newG.FillRectangle(bs[i], offset + i * multiple + w0 - w0 / 2.2f, output.Height + newH*0.8f + output.Width / 6 * 0.5f * 0.5f * 2, w0 / 2.2f, w0 / 2.2f);
                    newG.DrawRectangle(Pens.Black, offset + i * multiple + w0 - w0 / 2.2f, output.Height + newH * 0.8f + output.Width / 6 * 0.5f * 0.5f * 2, w0 / 2.2f, w0 / 2.2f);
                    if (i == 0)
                    {
                        newG.DrawString("未碾压", ftString, /*bs[i]*/Brushes.Black, offset * 1.05f + w0, output.Height + newH * 0.8f + output.Width / 6 * 0.5f * 0.5f * 2);
                        newG.DrawString(area_ratio[i].ToString("0.00%"), ftScale, /*bs[i]*/Brushes.Black, offset * 1.05f + w0, output.Height + newH * 0.8f + output.Width / 6 * 0.5f * 0.5f * 2 + w0 / 3.5f);
                        continue;
                    }
                    newG.DrawString(i.ToString() + "遍", ftString, /*bs[i]*/Brushes.Black, offset * 1.05f + ((i + 1) * multiple - w0), output.Height + newH * 0.8f + output.Width / 6 * 0.5f * 0.5f * 2);
                    newG.DrawString(area_ratio[i].ToString("0.00%"), ftScale, /*bs[i]*/Brushes.Black, offset * 1.05f + ((i + 1) * multiple - w0), output.Height + newH * 0.8f + output.Width / 6 * 0.5f * 0.5f * 2 + w0 / 3.5f);
                }

                for (int i = 0; i < 6; i++)
                {
                    if (i+6 > okcount)
                        continue;
                    newG.FillRectangle(bs[6 + i], offset + i * multiple + w0 - w0 / 2.2f, output.Height + newH * 0.8f + output.Width / 6 * 0.5f * 0.5f * 3.5f, w0 / 2.2f, w0 / 2.2f);
                    newG.DrawRectangle(Pens.Black, offset + i * multiple + w0 - w0 / 2.2f, output.Height + newH * 0.8f + output.Width / 6 * 0.5f * 0.5f * 3.5f, w0 / 2.2f, w0 / 2.2f);
//                     if ((i + 6) == 11)
//                     {
//                         newG.DrawString("11遍及以上", ftString, /*bs[6 + i]*/Brushes.Black, offset * 1.05f + ((i + 1) * multiple - w0), output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f);
//                         newG.DrawString(GetAreaScale(areas, i + 6).ToString("0.00%"), ftScale, /*bs[6 + i]*/Brushes.Black, offset * 1.05f + ((i + 1) * multiple - w0), output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f + w0 / 3.5f);
//                         continue;
//                     }
                    newG.DrawString((6 + i).ToString() + "遍" + (((i + 6) == okcount) ? "及以上" : ""), ftString, /*bs[6 + i]*/Brushes.Black, offset * 1.05f + ((i + 1) * multiple - w0), output.Height + newH * 0.8f + output.Width / 6 * 0.5f * 0.5f * 3.5f);
                    newG.DrawString(area_ratio[6 + i].ToString("0.00%"), ftScale, /*bs[6 + i]*/Brushes.Black, offset * 1.05f + ((i + 1) * multiple - w0), output.Height + newH * 0.8f + output.Width / 6 * 0.5f * 0.5f * 3.5f + w0 / 3.5f);
                }

                //备注
                RectangleF remarkPf = new RectangleF(offset - 4, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f + w0 / 3.5f + s.Height, newBmp.Width - 2*(offset - 4), newBmp.Height - (output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f + w0 / 3.5f) - s.Height);
                StringFormat remarkSf = new StringFormat();
                remarkSf.LineAlignment = StringAlignment.Near;
                remarkSf.Alignment = StringAlignment.Near;
                string remark = DB.SegmentDAO.getInstance().ReadSegmentRemark(this.DeckInfo.BlockID, this.DeckInfo.DesignZ, this.DeckInfo.SegmentID);
                if (remark != String.Empty)
                {
                    remark = "(" + remark + ")";
                    s = newG.MeasureString(remark, ftString);
                    float height = s.Height;
                    fa = 10f;
                    while (s.Width > (newBmp.Width - 2 * (offset - 4)) || s.Height >= height)
                    {
                        ftString = new Font("微软雅黑", fa * factor);
                        s = newG.MeasureString(remark, ftString);
                        fa = fa - 0.1f;
                    }

                }
                newG.DrawString(remark, ftString, Brushes.Black, remarkPf, remarkSf);

                //刻度
                float meterPrePoint = GetMultipleFactor(layer, 50);
                PointF pf;
                PointF pf5;
                PointF pfWord;
                Pen p2p = new Pen(Brushes.Black, 2);
                RectangleF rf;
                PointF pf1;
                SizeF sz;
                StringFormat sf = new StringFormat(); ;
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;


                newG.SmoothingMode = SmoothingMode.None;
                //横轴刻度
                for (float i = 1; i < (float)(pl.ScreenBoundary.Width - 10) / meterPrePoint; i++)//this.Polygon.ScreenBoundary.Width-40
                {
                    pf = new PointF(offset + i * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH - 6);
                    pf5 = new PointF(offset + i * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH);
                    if (i % 5 == 0)
                    {
                        pf = new PointF(offset + i * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH - 10);
                        pf5 = new PointF(offset + i * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH);
                        pfWord = new PointF(offset - 8 + i * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH + 5);
                        newG.DrawLine(Pens.Black, pf, pf5);

                        pf1 = new PointF(offset + (i - 1) * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH);
                        sz = new SizeF(2 * meterPrePoint, offset * 0.4f);
                        rf = new RectangleF(pf1, sz);
                        newG.DrawString((DamOrignCoord.X + i * 5).ToString("0"), ftScale, Brushes.Black, rf, sf);
                        continue;
                    }
                    newG.DrawLine(Pens.Gray, pf, pf5);
                }
                //纵轴刻度

                for (float i = 1; i < (float)(pl.ScreenBoundary.Height - 2) / meterPrePoint; i++)//this.Polygon.ScreenBoundary.Width-40
                {
                    pf = new PointF(offset, -i * meterPrePoint + (float)this.Polygon.ScreenBoundary.Height + newH);
                    pf5 = new PointF(offset + 5, -i * meterPrePoint + (float)this.Polygon.ScreenBoundary.Height + newH);
                    if (i % 5 == 0)
                    {
                        pf = new PointF(offset, -i * meterPrePoint + (float)this.Polygon.ScreenBoundary.Height + newH);
                        pf5 = new PointF(offset + 10, -i * meterPrePoint + (float)this.Polygon.ScreenBoundary.Height + newH);
                        newG.DrawLine(Pens.Black, pf, pf5);
                        pf1 = new PointF(offset * 0.2f, -(i + 1) * meterPrePoint + (float)pl.ScreenBoundary.Height + newH);
                        sz = new SizeF(offset * 0.8f, 2 * meterPrePoint);
                        rf = new RectangleF(pf1, sz);

                        newG.DrawString((DamOrignCoord.Y + i * 5).ToString("0"), ftScale, Brushes.Black, rf, sf);

                        continue;
                    }
                    newG.DrawLine(Pens.Gray, pf, pf5);
                }
                newG.SmoothingMode = SmoothingMode.AntiAlias;
                //输出放大倍数和面积
                RectangleF thisPf = new RectangleF(0, offset * 0.1f, newBmp.Width - offset * 0.1f, newBmp.Height);
                StringFormat thisSf = new StringFormat();
                thisSf.Alignment = StringAlignment.Far;


                string dateStartString = "开始：" + this.DateStart.Year.ToString("00-") + this.DateStart.Month.ToString("00-") + this.DateStart.Day.ToString("00 ")
                    + this.DateStart.Hour.ToString("00:") + this.DateStart.Minute.ToString("00:") + this.DateStart.Second.ToString("00");
                string dateEndString = "结束：" + this.DateEnd.Year.ToString("00-") + this.DateEnd.Month.ToString("00-") + this.DateEnd.Day.ToString("00 ")
                    + this.DateEnd.Hour.ToString("00:") + this.DateEnd.Minute.ToString("00:") + this.DateEnd.Second.ToString("00");
                if (this.State == DM.DB.SegmentWorkState.WORK)
                    dateEndString = "结束：" + "尚未收仓";
                string dateNow = DB.DBCommon.getDate().Year.ToString("00-") + DB.DBCommon.getDate().Month.ToString("00-") + DB.DBCommon.getDate().Day.ToString("00 ")
                    + DB.DBCommon.getDate().Hour.ToString("00:") + DB.DBCommon.getDate().Minute.ToString("00:") + DB.DBCommon.getDate().Second.ToString("00");

                //
                float topBlank = newBmp.Height * 0.1f;
                Font ftTime = new Font("微软雅黑", 7.5f * factor, GraphicsUnit.Pixel);
                s = newG.MeasureString("出图时间", ftTime);
                fa = 10f;
                while (s.Height > topBlank * 0.2f)
                {
                    ftTime = new Font("微软雅黑", fa * factor);
                    s = newG.MeasureString("出图时间", ftTime);
                    fa = fa - 0.1f;
                }

                Bitmap bitMp = new Bitmap((int)newBmp.Width, (int)(newBmp.Height + topBlank));
                Graphics endG = Graphics.FromImage(bitMp);
                thisPf = new RectangleF(0, 0, bitMp.Width * 0.98f, bitMp.Height);
                thisSf.LineAlignment = StringAlignment.Far;
                thisSf.Alignment = StringAlignment.Far;
                fa = 50f;
                Font ftWord = new Font("微软雅黑", ft.Size, GraphicsUnit.Pixel);
                s = newG.MeasureString("出图时间：" + dateNow, ftWord);
                while (s.Height*2 > topBlank * 0.29f)
                {
                    fa = fa - 0.1f;
                    ftWord = new Font("微软雅黑", fa * factor);
                    s = newG.MeasureString("出图时间：" + dateNow, ftWord);
                }
                endG.Clear(Color.White);
                endG.DrawImageUnscaled(newBmp, 0, (int)topBlank);
                //endG.DrawLine(Pens.Black, offset * 0.6f, topBlank * 0.8f, newBmp.Width - offset * 0.6f, topBlank * 0.8f);
                endG.DrawString("出图时间：" + dateNow, ftWord, Brushes.Black, thisPf, thisSf);
                Font ftTitle = new Font("微软雅黑", 15 * factor);

                thisPf = new RectangleF(0, topBlank * 0.7f, bitMp.Width * 0.98f, topBlank);
                thisSf.LineAlignment = StringAlignment.Near;
                thisSf.Alignment = StringAlignment.Far;
                endG.DrawString(dateStartString, ftWord, Brushes.Black, thisPf, thisSf);
                thisPf = new RectangleF(0, topBlank * 0.7f + s.Height, bitMp.Width * 0.98f, topBlank);
                endG.DrawString(dateEndString, ftWord, Brushes.Black, thisPf, thisSf);
                //输出分区，高程，名称，时间
                string allString = this.Partition.Name + "分区   " + this.deckInfo.DesignZ.ToString()+"米高程   " + this.deckInfo.SegmentName + "仓面" + pl.ActualArea.ToString("（0.00 米²）");
                fa = 20f;
                ftTime = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString(allString, ftTime);
                while (s.Height > topBlank * 0.29f || s.Width > (bitMp.Width * 0.98f - newG.MeasureString(dateEndString, ftWord).Width - offset * 3))
                {
                    fa = fa - 0.1f;
                    ftTime = new Font("微软雅黑", fa * factor);
                    s = newG.MeasureString(allString, ftTime);
                }
                endG.DrawString(allString, ftTime, Brushes.Black, offset * 2, topBlank * 0.71f);

                thisPf = new RectangleF(0f, topBlank * 0.25f, bitMp.Width, topBlank * 0.4f);
                thisSf.LineAlignment = StringAlignment.Center;
                thisSf.Alignment = StringAlignment.Center;

                endG.DrawLine(Pens.Black, offset * 0.6f, topBlank * 0.7f, newBmp.Width - offset * 0.6f, topBlank * 0.7f);


                s = newG.MeasureString("碾压遍数图形报告", ftTitle);
                fa = 10f;
                while (s.Height > topBlank * 0.4f||s.Width > newBmp.Width)
                {
                    ftTitle = new Font("微软雅黑", fa * factor);
                    s = newG.MeasureString("碾压遍数图形报告", ftTitle);
                    fa = fa - 0.1f;
                }
                s = newG.MeasureString("轴(m)", ftScale);



                endG.DrawString("轴(m)", ftScale, Brushes.Black, offset * 0.9f, topBlank - s.Height * 0.9f + 2 * factor);
                endG.DrawString("碾压遍数图形报告", ftTitle, Brushes.Black, thisPf, thisSf);

                if (!datamap)
                {
                    rolladdres = @"C:\OUTPUT\" + this.DeckInfo.SegmentName.Trim() + @"\" + this.Partition.Name + this.Elevation.Height.ToString("0.0") + this.ID.ToString() + "roll.png";
                   
#if DEBUG
                    bitMp.Save(@"C:\OUTPUT\" + this.Partition.Name + this.Elevation.Height.ToString("0.0") + this.ID.ToString() + "roll.png");
#else
                    DirectoryInfo dd = new DirectoryInfo(@"C:\OUTPUT\" + this.DeckInfo.SegmentName);
                    if (!dd.Exists)
                    {
                        dd.Create();
                    }
                    bitMp.Save(rolladdres);
#endif
                    rolladdres = this.Partition.Name + this.Elevation.Height.ToString("0.0") + this.ID.ToString() + "roll.png";
                }
                output.Dispose();
                endG.Dispose();
                newG.Dispose();
            }

            if (State == DM.DB.SegmentWorkState.END)
            {
                Bitmap bmp = CreateElevationImage();
                if (bmp != null)
                    bmp.Dispose();

                this.DrawPathMap();
            }
                layer.RotateDegree = oldRotate;
                layer.Zoom = oldZoom;
                layer.CreateScreen();
            

            return true;
        }
        public Bitmap CreateElevationImage()
        {
            Polygon pl = this.Polygon;
            double lo, hi;
            Bitmap bmp = ElevationImage(out lo, out hi);
            // 1、决定车辆轨迹时间上的先后次序
            // 2、计算相对高度
            // 3、渐变画图
            if (lo < 100 || hi < 100 || lo == double.MaxValue || hi == double.MinValue )
                return null;
            double delta = hi - lo;
            Graphics g = Graphics.FromImage(bmp);
            
//             g.TranslateTransform((float)-pl.ScreenBoundary.Left, (float)-pl.ScreenBoundary.Top);
// // 
//             g.SmoothingMode = SmoothingMode.AntiAlias;
//             g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //foreach (Vehicle v in this.VehicleControl.Vehicles)
            //{
            //    v.TrackGPSControl.Tracking.DrawElevation(g, lo, hi);
            //}
            //g.Clear(Color.Transparent);
//             pl.Line.Color = deckFrameColor;
//             pl.Fill.Codlor = Color.Transparent;
//             pl.Draw(g);
            //pl.SetDrawClip(g);
            DirectoryInfo di = new DirectoryInfo(@"C:\OUTPUT");
            if (!di.Exists)
            {
                di.Create();
            }
            DB.datamap.DAO.getInstance().updateElevationBitMap(deckInfo.BlockID, deckInfo.DesignZ, deckInfo.SegmentID, DB.datamap.DAO.getInstance().ToByte(bmp), lo.ToString("0.00") + "," + hi.ToString("0.00"));
#if DEBUG
            bmp.Save(@"C:\OUTPUT\"+this.Partition.Name + this.Elevation.Height.ToString("0.0") + this.ID.ToString() +"OrignElevation.png", System.Drawing.Imaging.ImageFormat.Png);
#endif

            #region - 画图 -
            Layer layer = owner;
            float factor;
            factor = GetMultipleFactor(layer, 3.56);
            float offsetx;
            float offsety;
            float offset;

            offsetx = bmp.Width * 1.2f;
            offsety = bmp.Width / 6 * 0.5f * 0.5f * 7f + bmp.Height;
            offset = (offsetx - bmp.Width) / 2;




            Bitmap newBmp = new Bitmap((int)offsetx, (int)offsety);
            Graphics newG = Graphics.FromImage(newBmp);

            float newH = bmp.Width / 6 * 0.5f * 0.5f;

            newG.Clear(Color.White);

            newG.SmoothingMode = SmoothingMode.AntiAlias;
            newG.InterpolationMode = InterpolationMode.HighQualityBicubic;
            newG.TranslateTransform((float)-pl.ScreenBoundary.Left + (offsetx - bmp.Width) / 2, (float)-pl.ScreenBoundary.Top + newH);
            this.ResetStyles();
            //pl.Draw(newG);
            //pl.SetDrawClip(newG);
            newG.ResetTransform();
            newG.DrawImageUnscaled(bmp, (int)offset, (int)newH);
            newG.ResetClip();


            Pen newPen = new Pen(Brushes.Black, 1);
            newPen.CustomEndCap = new AdjustableArrowCap(2, 8, true);

            //横轴
            newG.DrawLine(newPen, new PointF(offset - 4, (float)this.Polygon.ScreenBoundary.Height + newH), new PointF((float)this.Polygon.ScreenBoundary.Width + offset, (float)this.Polygon.ScreenBoundary.Height + newH));
            newG.DrawString("坝(m)", ftScale, Brushes.Black, (float)this.Polygon.ScreenBoundary.Width + offset * 1f, (float)this.Polygon.ScreenBoundary.Height + newH);

            //纵轴
            newG.DrawLine(newPen, new PointF(offset, (float)this.Polygon.ScreenBoundary.Height + newH + 4), new PointF(offset, 2 * factor));
            //newG.DrawString("轴(m)", ftScale, Brushes.Black, offset * 0.9f, 0 * factor);

            //原点坐标
            newG.DrawString(orignCoordString, ftScale, Brushes.Black, offset * 0.8f, (float)this.Polygon.ScreenBoundary.Height + newH + 2);

            //刻度
            float meterPrePoint = GetMultipleFactor(layer, 50);
            PointF pf;
            PointF pf5;
            PointF pfWord;
            Pen p2p = new Pen(Brushes.Black, 2);
            RectangleF rf;
            PointF pf1;
            SizeF sz;
            StringFormat sf = new StringFormat(); ;
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            PointF pf6;
            Pen p6;
            List<float> allX = new List<float>();
            List<float> allY = new List<float>();

            Color smallFore = Color.Gray;
            Color smallBack = Color.White;
            Color bigFore = Color.Black;
            Color bigBack = Color.White;
            //横轴刻度
            for (float i = 0; i < (float)(pl.ScreenBoundary.Width - 10) / meterPrePoint; i++)//this.Polygon.ScreenBoundary.Width-40
            {
                allX.Add(offset + i * meterPrePoint);
                if (i == 0)
                {
                    continue;
                }
                pf = new PointF(offset + i * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH - 6);
                pf5 = new PointF(offset + i * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH);
                if (i % 5 == 0)
                {
                    pf = new PointF(offset + i * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH);
                    pf5 = new PointF(offset + i * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH);
                    pfWord = new PointF(offset - 8 + i * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH + 5);
                    //newG.DrawLine(Pens.Black, pf, pf5);

                    pf6 = new PointF(offset + i * meterPrePoint, 4 * factor);
                    p6 = new Pen(bigBack, 2);
                    newG.DrawLine(p6, pf, pf6);
                    p6 = new Pen(bigFore, 1);
                    newG.DrawLine(p6, pf, pf6);

                    pf1 = new PointF(offset + (i - 1) * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH);
                    sz = new SizeF(2 * meterPrePoint, offset * 0.4f);
                    rf = new RectangleF(pf1, sz);
                    newG.DrawString((DamOrignCoord.X + i * 5).ToString("0"), ftScale, Brushes.Black, rf, sf);
                    continue;
                }
                //newG.DrawLine(Pens.Gray, pf, pf5);
                pf6 = new PointF(offset + i * meterPrePoint, 4 * factor);
                p6 = new Pen(smallBack, 2);
                newG.DrawLine(Pens.Gray, pf, pf5);
                newG.DrawLine(p6, pf, pf6);
                p6 = new Pen(smallFore, 1);
                newG.DrawLine(p6, pf, pf6);
            }
            //纵轴刻度

            for (float i = 0; i < (float)(pl.ScreenBoundary.Height - 10) / meterPrePoint; i++)//this.Polygon.ScreenBoundary.Width-40
            {
                allY.Add(-i * meterPrePoint + (float)this.Polygon.ScreenBoundary.Height + newH);
                if (i == 0)
                {
                    continue;
                }
                pf = new PointF(offset, -i * meterPrePoint + (float)this.Polygon.ScreenBoundary.Height + newH);
                pf5 = new PointF(offset + 5, -i * meterPrePoint + (float)this.Polygon.ScreenBoundary.Height + newH);
                if (i % 5 == 0)
                {
                    pf = new PointF(offset, -i * meterPrePoint + (float)this.Polygon.ScreenBoundary.Height + newH);
                    pf5 = new PointF(offset + 10, -i * meterPrePoint + (float)this.Polygon.ScreenBoundary.Height + newH);
                    //newG.DrawLine(Pens.Black, pf, pf5);

                    pf6 = new PointF(newBmp.Width - offset, -i * meterPrePoint + (float)this.Polygon.ScreenBoundary.Height + newH);
                    p6 = new Pen(bigBack, 2);
                    newG.DrawLine(Pens.Gray, pf, pf5);
                    newG.DrawLine(p6, pf5, pf6);
                    p6 = new Pen(bigFore, 1);
                    newG.DrawLine(p6, pf, pf6);

                    pf1 = new PointF(offset * 0.2f, -(i + 1) * meterPrePoint + (float)pl.ScreenBoundary.Height + newH);
                    sz = new SizeF(offset * 0.8f, 2 * meterPrePoint);
                    rf = new RectangleF(pf1, sz);
                    newG.DrawString((DamOrignCoord.Y + i * 5).ToString("0"), ftScale, Brushes.Black, rf, sf);
                    continue;
                }
                //newG.DrawLine(Pens.Gray, pf, pf5);
                pf6 = new PointF(newBmp.Width - offset, -i * meterPrePoint + (float)this.Polygon.ScreenBoundary.Height + newH);
                p6 = new Pen(smallBack, 2);
                //newG.DrawLine(Pens.Gray, pf, pf5);
                newG.DrawLine(p6, pf5, pf6);
                p6 = new Pen(smallFore, 1);
                newG.DrawLine(p6, pf, pf6);
            }

            //偏移量
            double offsetX = offset;
            double offsetY = newH;
            PointF focus = new PointF();
            float va;
            double elevationValue;
            RectangleF recValue;
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Far;


            Font ftValue = new Font("微软雅黑", ft.Size);
            SizeF ss = newG.MeasureString("1000.1", ftValue);
            float faV = 10f;
            while (ss.Width > meterPrePoint / 2)
            {
                faV = faV - 0.1f;
                ftValue = new Font("微软雅黑", faV * factor);
                ss = newG.MeasureString("1000.1", ftValue);
            }

            //获取所有焦点
            for (int i = 0; i < allX.Count; i++)
            {
                for (int j = 0; j < allY.Count; j++)
                {
                    focus = new PointF(allX[i], allY[j]);
                    Color cl = bmp.GetPixel((int)(GetOrigin(focus, offsetX, offsetY).X), (int)(GetOrigin(focus, offsetX, offsetY).Y));
                    va = ((float)cl.B) / 255;
                    elevationValue = va * (hi - lo) + lo;
                    recValue = new RectangleF(allX[i], 0, offset, allY[j]);
                    if (cl.A == 0)
                    {
                        continue;
                    }
                    //newG.DrawString(elevationValue.ToString("0.0"), ftValue, Brushes.White, recValue, sf);
                    Utils.Graph.OutGlow.DrawOutglowText(newG, elevationValue.ToString("0.00"), ftValue, recValue, sf, Brushes.Black, Brushes.White);
                }
            }

            GraphicsState gs = newG.Save();
            newG.ResetTransform();
            newG.TranslateTransform((float)-pl.ScreenBoundary.Left + (offsetx - bmp.Width) / 2, (float)-pl.ScreenBoundary.Top + newH);
            pl.LineColor = Color.OrangeRed;
            pl.FillColor = Color.Transparent;
            pl.Draw(newG);
            newG.Restore(gs);

            RectangleF thisPf = new RectangleF(0, offset * 0.1f, newBmp.Width - offset * 0.1f, newBmp.Height);
            StringFormat thisSf = new StringFormat();


            string dateStartString = "开始：" + this.DateStart.Year.ToString("00-") + this.DateStart.Month.ToString("00-") + this.DateStart.Day.ToString("00 ")
                   + this.DateStart.Hour.ToString("00:") + this.DateStart.Minute.ToString("00:") + this.DateStart.Second.ToString("00");
            string dateEndString = "结束：" + this.DateEnd.Year.ToString("00-") + this.DateEnd.Month.ToString("00-") + this.DateEnd.Day.ToString("00 ")
                + this.DateEnd.Hour.ToString("00:") + this.DateEnd.Minute.ToString("00:") + this.DateEnd.Second.ToString("00");
            if (this.State == DM.DB.SegmentWorkState.WORK)
                dateEndString = "结束：" + "尚未收仓";
            string dateNow = DB.DBCommon.getDate().Year.ToString("00-") + DB.DBCommon.getDate().Month.ToString("00-") + DB.DBCommon.getDate().Day.ToString("00 ")
                + DB.DBCommon.getDate().Hour.ToString("00:") + DB.DBCommon.getDate().Minute.ToString("00:") + DB.DBCommon.getDate().Second.ToString("00");

            //画渐变条
            newPen = new Pen(Brushes.Black, 1);
            newPen.CustomEndCap = new AdjustableArrowCap(2, 8, true);
            RectangleF f = new RectangleF(offset, bmp.Height + offset * 1.5f, bmp.Width, offset / 2);
            Brush b = new LinearGradientBrush(f, Color.White, Color.Black, 0f);

            newG.FillRectangle(b, offset, bmp.Height + offset * 1.4f, bmp.Width, offset / 3);
            newG.DrawRectangle(Pens.Black, offset, bmp.Height + offset * 1.4f, bmp.Width, offset / 3);
            
            Pen rcPen = new Pen(Brushes.White, 2);
            float rcWidth = bmp.Width / 5;
            thisSf.Alignment = StringAlignment.Center;
            thisSf.LineAlignment = StringAlignment.Center;
            double add = (hi - lo) / 5;
            //刻度，数值
            for (int i = 0; i < 6; i++)
            {
                thisPf = new RectangleF(offset + (i - 1) * rcWidth, bmp.Height + offset * 1.45f + offset * 0.2f, 2 * rcWidth, offset / 2);
                if (i==0||i==5)
                {
                    newG.DrawLine(rcPen, offset + i * rcWidth, bmp.Height + offset * 1.4f, offset + i * rcWidth, bmp.Height + offset * 1.4f + offset * 0.7f);
                    newG.DrawLine(Pens.Black, offset + i * rcWidth, bmp.Height + offset * 1.4f, offset + i * rcWidth, bmp.Height + offset * 1.4f + offset * 0.7f);
                    thisPf = new RectangleF(offset + (i - 1) * rcWidth, bmp.Height + offset * 1.4f + offset * 0.6f, 2 * rcWidth, offset / 2);
                }
                newG.DrawLine(rcPen, offset + i * rcWidth, bmp.Height + offset * 1.4f, offset + i * rcWidth, bmp.Height + offset * 1.4f + offset * 0.4f);
                newG.DrawLine(Pens.Black, offset + i * rcWidth, bmp.Height + offset * 1.4f, offset + i * rcWidth, bmp.Height + offset * 1.4f + offset * 0.4f);
                newG.DrawString((hi - i * add).ToString("0.00"), ft, Brushes.Black, thisPf, thisSf);
            }
            SizeF s = newG.MeasureString((hi - lo).ToString("0.00"), ft);
            newG.DrawLine(newPen, offset + 2.5f * rcWidth-s.Width, bmp.Height + offset * 1.4f + offset / 3+s.Height, offset, bmp.Height + offset * 1.4f + offset / 3+s.Height);
            newG.DrawLine(newPen, offset + 2.5f * rcWidth+s.Width, bmp.Height + offset * 1.4f + offset / 3 + s.Height, offset + 5 * rcWidth, bmp.Height + offset * 1.4f + offset / 3+s.Height);
            thisPf = new RectangleF(offset + 2 * rcWidth, bmp.Height + offset * 1.4f + offset / 3 + s.Height/2,rcWidth,s.Height);
            newG.DrawString((hi - lo).ToString("0.00"),ft,Brushes.Black,thisPf,thisSf);
            //三角指示
            PointF[] points = new PointF[3];
            points[0] = new PointF(offset, bmp.Height + offset);
            points[1] = new PointF(offset, bmp.Height + offset * 1.3f);
            points[2] = new PointF(offset + bmp.Width, bmp.Height + offset * 1.3f);

            newG.FillPolygon(Brushes.Gray, points);
            newG.DrawPolygon(Pens.Black, points);
            //输出分区，高程，名称，时间
            float topBlank = newBmp.Height * 0.1f;
            Bitmap bitMp = new Bitmap((int)newBmp.Width, (int)(newBmp.Height + topBlank));
            Graphics endG = Graphics.FromImage(bitMp);
            endG.Clear(Color.White);
            endG.DrawImageUnscaled(newBmp, 0, (int)topBlank);
            Font ftTitle = new Font("微软雅黑", 15 * factor);
            Font ftTime = new Font("微软雅黑", 7.5f * factor, GraphicsUnit.Pixel);

            float fa = 50f;
            Font ftWord = new Font("微软雅黑", ft.Size, GraphicsUnit.Pixel);
            s = newG.MeasureString("出图时间：" + dateNow, ftWord);
            while (s.Height * 2 > topBlank * 0.29f)
            {
                fa = fa - 0.1f;
                ftWord = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString("出图时间：" + dateNow, ftWord);
            }

            //输出分区，高程，名称，时间
            string allString = this.Partition.Name + "分区   " + this.deckInfo.DesignZ.ToString() + "米高程   " + this.deckInfo.SegmentName + "仓面" + pl.ActualArea.ToString("（0.00 米²）");
            //endG.DrawString(allString, ftWord, Brushes.Black, offset * 2, topBlank * 0.75f + s.Height * 0.8f);
            fa = 50f;
            ftTime = new Font("微软雅黑", fa * factor);
            s = newG.MeasureString(allString, ftTime);
            while (s.Height > topBlank * 0.29f || s.Width > (bitMp.Width * 0.98f - newG.MeasureString(dateEndString, ftWord).Width - offset * 3))
            {
                fa = fa - 0.1f;
                ftTime = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString(allString, ftTime);
            }
            endG.DrawString(allString, ftTime, Brushes.Black, offset * 2, topBlank * 0.71f);
            //endG.DrawString(allString, ftWord, Brushes.Black, offset * 2, topBlank * 0.75f + s.Height * 0.8f);
            //endG.DrawString(this.deckInfo.DesignZ.ToString("0米高程"), ftWord, Brushes.Black, offset * 3f, topBlank * 0.75f + s.Height * 0.8f);
            //endG.DrawString(this.deckInfo.SegmentName + "仓面"  +pl.ActualArea.ToString("（0.00 m²）"), ftWord, Brushes.Black, offset * 4f, topBlank * 0.75f + s.Height * 0.8f);

            thisPf = new RectangleF(0, 0, bitMp.Width * 0.98f, bitMp.Height);
            thisSf.LineAlignment = StringAlignment.Far;
            thisSf.Alignment = StringAlignment.Far;
            endG.DrawString("出图时间：" + dateNow, ftWord, Brushes.Black, thisPf, thisSf);


            //endG.DrawLine(Pens.Black, offset * 0.6f, topBlank * 0.8f, newBmp.Width - offset * 0.6f, topBlank * 0.8f);
            thisPf = new RectangleF(0, topBlank * 0.7f, bitMp.Width * 0.98f, topBlank);
            thisSf.LineAlignment = StringAlignment.Near;
            thisSf.Alignment = StringAlignment.Far;
            s = newG.MeasureString("出图时间：" + dateNow, ftWord);
            endG.DrawString(dateStartString, ftWord, Brushes.Black, thisPf, thisSf);
            thisPf = new RectangleF(0, topBlank * 0.7f + s.Height, bitMp.Width * 0.98f, topBlank);
            endG.DrawString(dateEndString, ftWord, Brushes.Black, thisPf, thisSf);

            thisPf = new RectangleF(0f, topBlank * 0.25f, bitMp.Width, topBlank * 0.4f);
            thisSf.LineAlignment = StringAlignment.Center;
            thisSf.Alignment = StringAlignment.Center;
            endG.DrawLine(Pens.Black, offset * 0.6f, topBlank * 0.7f, newBmp.Width - offset * 0.6f, topBlank * 0.7f);


            s = newG.MeasureString("碾压遍数图形报告", ftTitle);
            fa = 10f;
            while (s.Height > topBlank * 0.4f||s.Width > newBmp.Width)
            {
                ftTitle = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString("碾压遍数图形报告", ftTitle);
                fa = fa - 0.1f;
            }

            s = endG.MeasureString("轴", ftScale);
            endG.DrawString("轴(m)", ftScale, Brushes.Black, offset * 0.9f, topBlank - s.Height * 0.9f + 2 * factor);
            endG.DrawString("碾压高程图形报告", ftTitle, Brushes.Black, thisPf, thisSf);

            //ElevationImage();
            #endregion
            //#if DEBUG

            string address = @"C:\OUTPUT\" + this.DeckInfo.SegmentName + @"\" + this.Partition.Name + this.Elevation.Height.ToString("0.0")+this.ID.ToString() + "elevation.png";
#if DEBUG
            bitMp.Save(@"C:\OUTPUT\" + this.Partition.Name + this.Elevation.Height.ToString("0.0") + this.ID.ToString() + "elevation+.png");
//#else
            //            bitMp.Save(address, System.Drawing.Imaging.ImageFormat.Png);
#endif
            //#else
            //            bitMp.Save(@"C:\output_elevation.png", System.Drawing.Imaging.ImageFormat.Png);
            //#endif
            g.Dispose();
            endG.Dispose();
            newG.Dispose();
            newPen.Dispose();
            return bmp;
        }

        public Bitmap ElevationImage(out double lo, out double hi)
        {
            Polygon pl = this.Polygon;
            Bitmap bmp = new Bitmap((int)pl.ScreenBoundary.Width + 1, (int)pl.ScreenBoundary.Height + 1);

            foreach(Vehicle v in this.VehicleControl.Vehicles)
            {
                v.TrackGPSControl.Tracking.FilterForOutput();
            }

            // 1、决定车辆轨迹时间上的先后次序
            // 2、计算相对高度
            // 3、渐变画图
            this.VehicleControl.MaxMin(out lo, out hi);
            if (lo < 100 || hi < 100 || lo == double.MaxValue || hi == double.MinValue)
                return null;
            //double delta = hi - lo;
            Graphics g = Graphics.FromImage(bmp);
            //g.Clear(Color.Transparent);
            g.TranslateTransform((float)-pl.ScreenBoundary.Left, (float)-pl.ScreenBoundary.Top);
            //pl.Line.Color = Color.Transparent;
            //pl.Fill.Color = Color.Transparent;
            //pl.Draw(g);
            pl.SetDrawClip(g);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            foreach (Vehicle v in this.VehicleControl.Vehicles)
            {
                v.TrackGPSControl.Tracking.DrawElevation(g, lo, hi);
            }
            foreach (Vehicle v in this.VehicleControl.Vehicles)
            {
                v.TrackGPSControl.Tracking.Reset();
            }
//#if DEBUG
//            bmp.Save(@"C:\OUTPUT\"+this.DeckInfo.SegmentName+@"\elevation.png", System.Drawing.Imaging.ImageFormat.Png);
//#endif
            return bmp;
        }
        public void DrawPathMap()
        {
            TrackGPS gps;
            Polygon pl = this.Polygon;
            Bitmap output = new Bitmap((int)Math.Ceiling(pl.ScreenBoundary.Width), (int)Math.Ceiling(pl.ScreenBoundary.Height), PixelFormat.Format32bppPArgb);
            Graphics g = Graphics.FromImage(output);

            g.Clear(Color.White);
            // g.DrawImage(learning, 0, 0);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TranslateTransform((float)-pl.ScreenBoundary.Left, (float)-pl.ScreenBoundary.Top);
            pl.AntiAlias = false;
            pl.LineColor = Color.Gray;
            pl.FillColor = Color.White;
            pl.Draw(g);
            pl.SetDrawClip(g);

            foreach (Vehicle v in vCtrl.Vehicles)
            {
                gps = v.TrackGPSControl.Tracking;
                //gps.Color = kColor[i];
                v.TrackGPSControl.Tracking.DrawSkeleton(g, false);
            }
            DirectoryInfo di = new DirectoryInfo(@"C:\OUTPUT");
            if (!di.Exists)
            {
                di.Create();
            }

            Layer layer = owner;
            float factor;
            factor = GetMultipleFactor(layer, 3.56);


            //求原点坐标
            Geo.Coord screenOriginCoord = pl.ScreenBoundary.LeftBottom;
            Geo.Coord earthOriginCoord = layer.ScreenToDam(screenOriginCoord.PF);
            DamOrignCoord = earthOriginCoord.ToDamAxisCoord();
            orignCoordString = "(" + DamOrignCoord.X.ToString("0.00") + ", " + DamOrignCoord.Y.ToString("0.00") + ")";

            float offsetx;
            float offsety;
            float offset;

            offsetx = output.Width * 1.2f;
            offsety = output.Width / 6 * 0.5f * 0.5f * 7f + output.Height; //output.Height + 120 * (int)Math.Ceiling(factor);
            offset = (offsetx - output.Width) / 2;


            Bitmap newBmp = new Bitmap((int)offsetx, (int)offsety);//output.Height + 120 * (int)Math.Ceiling(factor));//60,80                
            Graphics newG = Graphics.FromImage(newBmp);

            float newH = output.Width / 6 * 0.5f * 0.5f;

            newG.Clear(Color.White);

            newG.SmoothingMode = SmoothingMode.AntiAlias;
            newG.InterpolationMode = InterpolationMode.HighQualityBicubic;
            newG.TranslateTransform((float)-pl.ScreenBoundary.Left + (offsetx - output.Width) / 2, (float)-pl.ScreenBoundary.Top + newH);
            this.ResetStyles();
            pl.Draw(newG);
            newG.ResetTransform();
            newG.DrawImageUnscaled(output, (int)offset, (int)newH);

            Pen newPen = new Pen(Brushes.Black, 1);
            newPen.CustomEndCap = new AdjustableArrowCap(2, 8, true);
            ft = new Font("微软雅黑", 7.5f * factor, GraphicsUnit.Pixel);
            ftScale = new Font("微软雅黑", 5.5f * factor);

            ftString = new Font("微软雅黑", 7.5f * factor, FontStyle.Bold, GraphicsUnit.Pixel);


            float multiple = output.Width / 6;
            float w0 = multiple * 0.5f;
            float fa = 10f;

            SizeF s = newG.MeasureString("100.00%", ftScale);

            while (s.Width > (multiple - w0) * 0.9f)
            {
                if (fa * factor < 0)
                    return;

                ftScale = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString("100.99%", ftScale);
                fa = fa - 0.1f;
            }
            s = newG.MeasureString("未碾压", ft);
            fa = 10f;
            while (s.Width > (multiple - w0) * 0.9f)
            {
                ft = new Font("微软雅黑", fa * factor);
                ftString = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString("未碾压", ftString);
                fa = fa - 0.1f;
            }

            //横轴
            newG.DrawLine(newPen, new PointF(offset - 4, (float)this.Polygon.ScreenBoundary.Height + newH), new PointF((float)this.Polygon.ScreenBoundary.Width + offset, (float)this.Polygon.ScreenBoundary.Height + newH));
            newG.DrawString("坝(m)", ftScale, Brushes.Black, (float)this.Polygon.ScreenBoundary.Width + offset * 1f, (float)this.Polygon.ScreenBoundary.Height + newH);

            //纵轴
            newG.DrawLine(newPen, new PointF(offset, (float)this.Polygon.ScreenBoundary.Height + newH + 4), new PointF(offset, 2 * factor));
            //newG.DrawString("轴(m)", ftScale, Brushes.Black, offset * 0.9f, 0 * factor);

            //原点坐标
            newG.DrawString(orignCoordString, ftScale, Brushes.Black, offset * 0.8f, (float)this.Polygon.ScreenBoundary.Height + newH + 2);
            newPen.Dispose();



            //刻度
            float meterPrePoint = GetMultipleFactor(layer, 50);
            PointF pf;
            PointF pf5;
            PointF pfWord;
            Pen p2p = new Pen(Brushes.Black, 2);
            RectangleF rf;
            PointF pf1;
            SizeF sz;
            StringFormat sf = new StringFormat(); ;
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;


            newG.SmoothingMode = SmoothingMode.None;
            //横轴刻度
            for (float i = 1; i < (float)(pl.ScreenBoundary.Width - 10) / meterPrePoint; i++)//this.Polygon.ScreenBoundary.Width-40
            {
                pf = new PointF(offset + i * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH - 6);
                pf5 = new PointF(offset + i * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH);
                if (i % 5 == 0)
                {
                    pf = new PointF(offset + i * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH - 10);
                    pf5 = new PointF(offset + i * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH);
                    pfWord = new PointF(offset - 8 + i * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH + 5);
                    newG.DrawLine(Pens.Black, pf, pf5);

                    pf1 = new PointF(offset + (i - 1) * meterPrePoint, (float)this.Polygon.ScreenBoundary.Height + newH);
                    sz = new SizeF(2 * meterPrePoint, offset * 0.4f);
                    rf = new RectangleF(pf1, sz);
                    newG.DrawString((DamOrignCoord.X + i * 5).ToString("0"), ftScale, Brushes.Black, rf, sf);
                    continue;
                }
                newG.DrawLine(Pens.Gray, pf, pf5);
            }
            //纵轴刻度

            for (float i = 1; i < (float)(pl.ScreenBoundary.Height - 10) / meterPrePoint; i++)//this.Polygon.ScreenBoundary.Width-40
            {
                pf = new PointF(offset, -i * meterPrePoint + (float)this.Polygon.ScreenBoundary.Height + newH);
                pf5 = new PointF(offset + 5, -i * meterPrePoint + (float)this.Polygon.ScreenBoundary.Height + newH);
                if (i % 5 == 0)
                {
                    pf = new PointF(offset, -i * meterPrePoint + (float)this.Polygon.ScreenBoundary.Height + newH);
                    pf5 = new PointF(offset + 10, -i * meterPrePoint + (float)this.Polygon.ScreenBoundary.Height + newH);
                    newG.DrawLine(Pens.Black, pf, pf5);
                    pf1 = new PointF(offset * 0.2f, -(i + 1) * meterPrePoint + (float)pl.ScreenBoundary.Height + newH);
                    sz = new SizeF(offset * 0.8f, 2 * meterPrePoint);
                    rf = new RectangleF(pf1, sz);

                    newG.DrawString((DamOrignCoord.Y + i * 5).ToString("0"), ftScale, Brushes.Black, rf, sf);

                    continue;
                }
                newG.DrawLine(Pens.Gray, pf, pf5);
                //newG.DrawString("一", ftK, Brushes.Black, pf);
            }
            newG.SmoothingMode = SmoothingMode.AntiAlias;


            //分析车辆
            List<string> vehicleName = new List<string>();
            List<Color> vehicleColor = new List<Color>();
            bool has = false;
            if (vCtrl.Vehicles.Count <= 0)
            {
                return;
            }
            vehicleName.Add(vCtrl.Vehicles[0].Info.CarName);
            vehicleColor.Add(vCtrl.Vehicles[0].TrackGPSControl.Tracking.Color);
            foreach (Vehicle v in vCtrl.Vehicles)
            {
                has = false;
                for (int i = 0; i < vehicleName.Count; i++)
                {
                    if (v.Info.CarName.Equals(vehicleName[i]))
                    {
                        has = true;
                        break;
                    }
                }
                if (!has)
                {
                    vehicleName.Add(v.Info.CarName);
                    vehicleColor.Add(v.TrackGPSControl.Tracking.Color);
                }
            }
            //for (int i = 0; i < 7; i++)
            //{
            //    vehicleName.SetVertex("三号碾压机");
            //    vehicleColor.SetVertex(Color.AliceBlue);
            //}
            s = g.MeasureString("三号碾压机", ftString);
            fa = 10f;
            while (s.Width > (multiple - w0 - 2))
            {
                ftString = new Font("微软雅黑", fa * factor);
                s = g.MeasureString("三号碾压机", ftString);
                fa = fa - 0.1f;
            }
            float cutline = (newBmp.Width - offset * 1.05f + w0 * 0.3f - s.Width * 2.6f) / 9;
            //图例
            newG.FillRectangle(Brushes.Black, offset, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 2, w0 *0.3f+ 2, w0 / 6f + 2);
            newG.FillRectangle(Brushes.Yellow, offset + 1, 1 + output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 2, w0*0.3f, w0 / 6f);
            newG.DrawString("超速", ftString, Brushes.Black, offset * 1.05f + w0*0.3f, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 2);
            //振动不合格
            newG.FillRectangle(Brushes.Black, offset, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f, w0 * 0.3f + 2, w0 / 6f + 2);
            newG.FillRectangle(Brushes.Red, offset + 1, 1 + output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f, w0 * 0.3f, w0 / 6f);
            newG.DrawString("振动不合格", ftString, Brushes.Black, offset * 1.05f + w0 * 0.3f, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f);
            Brush bs;
            s=g.MeasureString("超速",ftString);
            for (int i = 0; i < vehicleName.Count && i < 8; i++)
            {
                bs = new SolidBrush(vehicleColor[i]);
                newG.FillRectangle(Brushes.Black, offset * 1.05f + w0 * 0.3f + s.Width * 2.6f + i * cutline, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 2, w0 * 0.2f + 2, w0 / 6f + 2);
                newG.FillRectangle(bs, offset * 1.05f + w0 * 0.3f + s.Width * 2.6f + 1 + i * cutline, 1 + output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 2, w0 * 0.2f, w0 / 6f);
                newG.DrawString(vehicleName[i], ftString, Brushes.Black, offset * 1.05f + w0 * 0.3f + s.Width * 2.6f + w0 * 0.2f + 2 + i * cutline, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 2);
            }
            if (vehicleName.Count > 8)
            {
                for (int i = 0; i < vehicleName.Count-8; i++)
                {//output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f    output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f
                    bs = new SolidBrush(vehicleColor[i+8]);
                    newG.FillRectangle(Brushes.Black, offset * 1.05f + w0 * 0.3f + s.Width * 2.6f + i * cutline, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f, w0 * 0.2f + 2, w0 / 6f + 2);
                    newG.FillRectangle(bs, offset * 1.05f + w0 * 0.3f + s.Width*2.6f + 1 + i * cutline, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f, w0 * 0.2f, w0 / 6f);
                    newG.DrawString(vehicleName[i + 8], ftString, Brushes.Black, offset * 1.05f + w0 * 0.3f + s.Width * 2.6f + w0 * 0.2f + 2 + i * cutline, output.Height + newH + output.Width / 6 * 0.5f * 0.5f * 3.5f);
                }
            }

            //输出放大倍数和面积
            RectangleF thisPf = new RectangleF(0, offset * 0.1f, newBmp.Width - offset * 0.1f, newBmp.Height);
            StringFormat thisSf = new StringFormat();
            thisSf.Alignment = StringAlignment.Far;


            string dateStartString = "开始：" + this.DateStart.Year.ToString("00-") + this.DateStart.Month.ToString("00-") + this.DateStart.Day.ToString("00 ")
                  + this.DateStart.Hour.ToString("00:") + this.DateStart.Minute.ToString("00:") + this.DateStart.Second.ToString("00");
            string dateEndString = "结束：" + this.DateEnd.Year.ToString("00-") + this.DateEnd.Month.ToString("00-") + this.DateEnd.Day.ToString("00 ")
                + this.DateEnd.Hour.ToString("00:") + this.DateEnd.Minute.ToString("00:") + this.DateEnd.Second.ToString("00");
            if (this.State == DM.DB.SegmentWorkState.WORK)
                dateEndString = "结束：" + "尚未收仓";
            string dateNow = DB.DBCommon.getDate().Year.ToString("00-") + DB.DBCommon.getDate().Month.ToString("00-") + DB.DBCommon.getDate().Day.ToString("00 ")
                + DB.DBCommon.getDate().Hour.ToString("00:") + DB.DBCommon.getDate().Minute.ToString("00:") + DB.DBCommon.getDate().Second.ToString("00");

            float topBlank = newBmp.Height * 0.1f;

            Font ftTime = new Font("微软雅黑", 7.5f * factor, GraphicsUnit.Pixel);
            s = newG.MeasureString("出图时间", ftTime);
            fa = 10f;
            while (s.Height > topBlank * 0.2f)
            {
                ftTime = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString("出图时间", ftTime);
                fa = fa - 0.1f;
            }

            Bitmap bitMp = new Bitmap((int)newBmp.Width, (int)(newBmp.Height + topBlank));
            Graphics endG = Graphics.FromImage(bitMp);
            thisPf = new RectangleF(0, 0, bitMp.Width * 0.98f, bitMp.Height);
            thisSf.LineAlignment = StringAlignment.Far;
            thisSf.Alignment = StringAlignment.Far;
            endG.Clear(Color.White);
            endG.DrawImageUnscaled(newBmp, 0, (int)topBlank);
            //endG.DrawRectangle(Pens.Black, 0, 0, bitMp.Width, topBlank*0.8f);
            fa = 50f;
            Font ftWord = new Font("微软雅黑", ft.Size, GraphicsUnit.Pixel);
            s = newG.MeasureString("出图时间：" + dateNow, ftWord);
            while (s.Height * 2 > topBlank * 0.29f)
            {
                fa = fa - 0.1f;
                ftWord = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString("出图时间：" + dateNow, ftWord);
            }

            endG.DrawString("出图时间：" + dateNow, ftWord, Brushes.Black, thisPf, thisSf);
            Font ftTitle = new Font("微软雅黑", 15 * factor);

            thisPf = new RectangleF(0, topBlank * 0.7f, bitMp.Width * 0.98f, topBlank);
            thisSf.LineAlignment = StringAlignment.Near;
            thisSf.Alignment = StringAlignment.Far;
            endG.DrawString(dateStartString, ftWord, Brushes.Black, thisPf, thisSf);
            thisPf = new RectangleF(0, topBlank * 0.7f + s.Height, bitMp.Width * 0.98f, topBlank);
            endG.DrawString(dateEndString, ftWord, Brushes.Black, thisPf, thisSf);
            //输出分区，高程，名称，时间


            string allString = this.Partition.Name + "分区   " + this.deckInfo.DesignZ.ToString() + "米高程   " + this.deckInfo.SegmentName + "仓面" + pl.ActualArea.ToString("（0.00 米²）");
            fa = 50f;
            ftTime = new Font("微软雅黑", fa * factor);
            s = newG.MeasureString(allString, ftTime);
            while (s.Height > topBlank * 0.29f || s.Width > (bitMp.Width * 0.98f - newG.MeasureString(dateEndString, ftWord).Width - offset * 3))
            {
                fa = fa - 0.1f;
                ftTime = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString(allString, ftTime);
            }
            endG.DrawString(allString, ftTime, Brushes.Black, offset * 2, topBlank * 0.71f);
            //endG.DrawString(allString, ftWord, Brushes.Black, offset * 2, topBlank * 0.75f + s.Height * 0.8f);
            //endG.DrawString(this.deckInfo.DesignZ.ToString("0米高程"), ftWord, Brushes.Black, offset * 3f, topBlank * 0.75f + s.Height * 0.8f);
            //endG.DrawString(this.deckInfo.SegmentName + "仓面" + pl.ActualArea.ToString("（0.00 m²）"), ftWord, Brushes.Black, offset * 4f, topBlank * 0.75f + s.Height * 0.8f);

            thisPf = new RectangleF(0f, topBlank * 0.25f, bitMp.Width, topBlank * 0.4f);
            thisSf.LineAlignment = StringAlignment.Center;
            thisSf.Alignment = StringAlignment.Center;

            endG.DrawLine(Pens.Black, offset * 0.6f, topBlank * 0.7f, newBmp.Width - offset * 0.6f, topBlank * 0.7f);

            s = newG.MeasureString("碾压轨迹图形报告", ftTitle);
            fa = 10f;
            while (s.Height > topBlank * 0.4f||s.Width > newBmp.Width)
            {
                ftTitle = new Font("微软雅黑", fa * factor);
                s = newG.MeasureString("碾压轨迹图形报告", ftTitle);
                fa = fa - 0.1f;
            }
  

            s = endG.MeasureString("轴", ftScale);
            endG.DrawString("轴(m)", ftScale, Brushes.Black, offset * 0.9f, topBlank - s.Height * 0.9f + 2 * factor);
            endG.DrawString("碾压轨迹图形报告", ftTitle, Brushes.Black, thisPf, thisSf);
            string address = @"C:\OUTPUT\" + this.DeckInfo.SegmentName.Trim() + @"\" + this.Partition.Name+this.Elevation.Height.ToString("0.0")+this.ID.ToString()+ "tracing.png";

#if DEBUG
            bitMp.Save(@"C:\OUTPUT\" + this.Partition.Name + this.Elevation.Height.ToString("0.0") + this.ID.ToString() + "tracing.png");
#else
            bitMp.Save(address, System.Drawing.Imaging.ImageFormat.Png);
#endif

            output.Dispose();
            g.Dispose();
            endG.Dispose();
            newG.Dispose();
        }
        public int RollCount(PointF pt)
        {
            if (!this.Polygon.IsScreenVisible(new Geo.Coord(pt)))
                return 0;
            return VehicleControl.RollCount(pt);
        }
        public float GetMultipleFactor(Layer layer, double multiple)
        {
            return (float)layer.ScreenSize(0.1 * multiple);//*this.Polygon.ActualArea/baseValue
        }

        //返回相应面积比
        double[] areaScale = null;
        public double GetAreaScale(int[] areas, int index)
        {
            if (areaScale == null || areaScale.Length <= index)
                return 0;

            //被压的总面积
            double totalArea = 0;
            for (int i = 0; i < 12; i++)
            {
                totalArea += areas[i];
            }
            if (totalArea == 0)
            {
                return 0;
            }
            areaScale[index] = areas[index] / totalArea;
            return areaScale[index];
        }
        //返回原始图中点的坐标
        public PointF GetOrigin(PointF p, double offsetX, double offsetY)
        {
            PointF origP = new PointF();
            origP.X = (float)(p.X - offsetX);
            origP.Y = (float)(p.Y - offsetY);
            return origP;
        }

        public unsafe double AverageHeightFromDataMap()
        {
            double h = 0;
            int count = 0;
            DB.SegmentDAO dao = DB.SegmentDAO.getInstance();
            byte[] datamap = dao.getDatamap(this.DeckInfo.BlockID, this.DeckInfo.DesignZ, this.DeckInfo.SegmentID);
            if (datamap == null)
                return -1;

            /*
             * 数据图格式：
             * int32 width in pixel
             * int32 height in pixel
             * byte rollcount0
             * float height0 (4 bytes)
             * byte rollcount1
             * float height1 (4 bytes)
             * ...
             */
            int width = 0;
            int height = 0;
            fixed(byte *p = datamap)
            {
                // 取长宽
                width = *(int*)p;
                height = *(int*)(p + 4);
                if (width < 0 || width > 99999 ||
                    height < 0 || height > 99999)
                    return -1;
                if ((width * height * 5 + 8) != datamap.Length)
                    return -1;

                // 取碾压变数、高程
                byte* pp = p + 8;
                for (int i = 0; i < height; i++ )
                {
                    for (int j = 0; j < width; j++ )
                    {
                        int rollcount = pp[0];
                        float elev = *(float*)(pp + 1);
                        if( elev != 0 )
                        {
                            h += elev;
                            count++;
                        }
                        pp += 5;
                    }
                }
            }
            if (count <= 0)
                return -1;

            return h/count;
        }

        public void CheckOverThickness(DM.Geo.Coord3D c3d)
        {
            if( Thickness == -1 )
                return;
            if (!this.RectContains(c3d.Plane))
                return;

            double distance = c3d.Z - (Thickness + (1+deckInfo.ErrorParam/100)*deckInfo.DesignDepth);
            if(distance > 0)
            {
                DM.Geo.Coord c = c3d.Plane.ToDamAxisCoord();
                string position = string.Format("{{{0:0.00},{1:0.00}}}", c.X, c.Y);
                string warning = string.Format("碾压超厚告警！分区 {0}，高程 {1}米，仓面 {2}，超厚 {3:0.00}米，桩号 {4}",
                    this.Partition.Description,
                    this.Elevation.Height,
                    this.Name,
                    distance,
                    position
                    );

                //DMControl.WarningControl.SendMessage(DM.DMControl.WarningType.OVERTHICKNESS, this.deckInfo.BlockID, warning);
            }
        }

        public void UpdateNRFromDB()
        {
            nrs = NotRolling.ReadFromDB(this);
        }
    }
}
