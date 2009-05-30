#define EXPERIMENT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using DM.Geo;
using DM.DMControl;

namespace DM.Models
{
    // 车辆GPS轨迹
    public class TrackGPS : ICloneable, IDisposable
    {
        public TrackGPS(Vehicle v) { owner = v; origTP.Capacity = 1000000; }
        public TrackGPS() { origTP.Capacity = 1000000; }

        public object Clone()
        {
            TrackGPS gps = (TrackGPS)this.MemberwiseClone();
            gps.origTP = new List<Coord3D>(origTP);
            gps.screenSeg = new List<List<Coord3D>>(screenSeg);
            for (int i = 0; i < screenSeg.Count; i++ )
            {
                gps.screenSeg[i] = new List<Coord3D>(screenSeg[i]);
            }
            return gps;
        }
        public void Dispose()
        {
            lock(sync)
            {
                gpLibrated.Clear();
                gpOverspeed.Clear();
                gpTracking.Clear();
                gpALLNOTracking.Clear();
                gpBand.Clear();
                libratedTracking.Clear();
                libratedOKTracking.Clear();
                screenSegLibrated.Clear();
                //hasReadCar.Clear();
                //alltimes.Clear();
            }
            screenSeg.Clear();
            screenSegFiltered.Clear();
            filteredSeg.Clear();
            filteredTP.Clear();
            segmentedTP.Clear();
            origTP.Clear();
            GC.SuppressFinalize(this);
        }
        // TP means TrackingPoints for short
        List<Coord3D> origTP = new List<Coord3D>();  // 原始轨迹点，未经任何筛选，施工坐标

        public List<Coord3D> TrackPoints
        {
            get { return origTP; }
            set { origTP = value; }
        }
        List<Coord3D> filteredTP = new List<Coord3D>(); // 经过筛选的轨迹点，施工坐标
        List<List<Coord3D>> segmentedTP = new List<List<Coord3D>>();   // 分段的坐标，施工坐标，未段内筛选

        List<List<Coord3D>> filteredSeg = new List<List<Coord3D>>();    // 经过段内筛选的分段坐标，施工坐标
        public static DateTime SetTime = DateTime.MinValue;
        List<List<Coord3D>> screenSeg = new List<List<Coord3D>>();     // 经过2次筛选的屏幕坐标
        List<List<Coord3D>> screenSegFiltered = new List<List<Coord3D>>();     // 经过2次筛选的屏幕坐标
        List<List<Coord3D>> screenSegLibrated = new List<List<Coord3D>>(); //经过筛选振动的屏幕坐标

        bool inCurve = true;
        public bool InCurve { get { return inCurve; } set { inCurve = value; } }
        Color cl = Color.Navy;
        public Color Color { get { return cl; } set { cl = value; } }

        public int Count { get { return origTP.Count; } }
        // 归属关系
        Vehicle owner = null;
        public Vehicle Owner { get { return owner; } set { owner = value; } }
        public override string ToString()
        {
            return string.Format("Orig={0}, Segment={1}, Filtered={2}", origTP.Count, segmentedTP.Count, filteredSeg.Count);
        }

        //volatile bool singlePointAdded = false;
        public void AddOnePoint(Coord3D pt, double offScrX, double offScrY)
        {
            lock(adding)
            {
                if (!DMControl.TrackGPSControl.IsValid(origTP, pt, owner.Owner.DeckInfo, owner.Info))
                    return;
                origTP.Add(pt);
                //singlePointAdded = true;
            }
            SetTracking(origTP, offScrX, offScrY);
        }
        object adding = new object();
        public void CreateScreen()
        {
            if (origTP.Count == 0)
                return;
            //SetTracking(origTP, 0, 0);
            lock (adding)
                CreatePath(0, 0);
        }
        List<GraphicsPath> gpTracking = new List<GraphicsPath>();
        List<GraphicsPath> gpALLNOTracking = new List<GraphicsPath>();
        List<GraphicsPath> gpBand = new List<GraphicsPath>();
        List<bool> gpOverspeed = new List<bool>();
        List<bool> gpLibrated = new List<bool>();
        List<GraphicsPath> libratedTracking = new List<GraphicsPath>();
        List<GraphicsPath> libratedOKTracking = new List<GraphicsPath>();
        public static List<DB.CarDistribute> hasReadCar = new List<DB.CarDistribute>(); //已经读过库的车
        public static List<List<Timeslice>> alltimes = new List<List<Timeslice>>();
        public void SetTracking(List<Coord3D> pts, double offScrX, double offScrY)
        {
            lock (adding)
            {
                /////////////////////////////////feiying 09.3.21 修改出数据图错误问题。待测试
                if (pts.Count == 0)
                    return;
                origTP = new List<Coord3D>(pts);
                filteredTP = new List<Coord3D>(origTP);
                PreFilter(ref filteredTP);
                Segment();
                FilterSegments();
                //FilterVibrant();
                lock(sync)
                    CreatePath(offScrX, offScrY);
            }
        }
        bool IsFlexPoint(int idx, List<Coord3D> lst)
        {
            if (idx == 0 || idx == (lst.Count - 1))
                return true;
            return false;
        }
        private double SegmentLength(List<Coord3D> lst)
        {
            if (lst == null || lst.Count <= 1)
                return 0;
            double sum = 0;
            for(int i=0; i<lst.Count-1; i++)
            {
                Vector v = new Vector(lst[i].Plane, lst[i+1].Plane);
                sum += v.Length();
            }
            return sum;
        }
//         private double SegmentAngle(List<Coord3D> s1, List<Coord3D> s2)
//         {
//             if (s1.Count <= 1 || s2.Count <= 2)
//                 return 0;
//             Vector v1 = new Vector(s1[s1.Count - 2].Plane, s1.Last().Plane);
//             Vector v2 = new Vector(s2[1].Plane, s2[2].Plane);
// 
//             return v1.DeltaAngleTo(v2);
//         }
//         private bool SameDirection(List<Coord3D> s1, List<Coord3D> s2)
//         {
//             return 90 <= Math.Abs(SegmentAngle(s1, s2));
//         }
        private double VibrantAngle1(List<Coord3D> s1, List<Coord3D> s2)
        {
            if (s1.Count <= 1 || s2.Count <= 2)
                return double.MaxValue;
            Vector v1 = new Vector(s1[s1.Count-2].Plane, s1[s1.Count-1].Plane);
            Vector v2 = new Vector(s2[1].Plane, s2[2].Plane);
            double angle = v1.DeltaAngleTo(v2);
            angle = Math.Abs(angle);
            return angle;
        }
        private double VibrantAngle2(List<Coord3D> s1, List<Coord3D> s2)
        {
            if (s1.Count <= 2 || s2.Count <= 1)
                return double.MaxValue;
            Vector v1 = new Vector(s1[s1.Count-3].Plane, s1[s1.Count-2].Plane);
            Vector v2 = new Vector(s2[0].Plane, s2[1].Plane);
            double angle = v1.DeltaAngleTo(v2);
            angle = Math.Abs(angle);
            return angle;
        }
        private void FilterVibrant()
        {
            for (int i = 1; i < filteredSeg.Count-1; i++ )
            {
                List<Coord3D> s1 = filteredSeg[i-1];
                List<Coord3D> s2 = filteredSeg[i];
                List<Coord3D> s3 = filteredSeg[i + 1];
                if (s2.Count != 2)
                    continue;

                double l = SegmentLength(s2);
                if (l > 3)
                    continue;

//                 System.Diagnostics.Debug.Print("Vibrant {0}", s2[1].Plane.ToString());
//                 if (s2[1].Plane.ToString() == "{X=49870.21, Y=-8401.77}")
//                 {
// 
//                 }

                Vector v = new Vector(s1[s1.Count - 2], s1[s1.Count - 1]);
                double angle = v.Angle();
                //System.Diagnostics.Debug.Print("angle={0:0.00}", angle);
                if( angle < 180 )
                {
                    // 往下

                    if (s3.Count <= 2)
                    {
                        s2[1] = new Coord3D(s2[1], 1);
                        continue;
                    }
                    Vector v2 = new Vector(s3[1], s3[2]);
                    double a2 = v.DeltaAngleTo(v2);
                    double a3 = Math.Abs(a2 - 180);
                    if ( a2> 30 && a3 > 30 )
                        continue;
                    s2[1] = new Coord3D(s2[1], 1);
                }
                else
                {
                    // 往上

                    if (s1.Count <= 2)
                    {
                        s2[0] = new Coord3D(s2[0], 2);
                        continue;
                    }
                    Vector v1 = new Vector(s1[s1.Count - 3], s1[s1.Count - 2]);
                    Vector v2 = new Vector(s3[0], s3[1]);
                    double a2 = v1.DeltaAngleTo(v2);
                    double a3 = Math.Abs(a2 - 180);
                    if (a2 > 30 && a3> 30)
                        continue;
                    s2[0] = new Coord3D(s2[0], 2);
                }
            }
        }
        // 删除距离过近的点（包括相邻坐标相同的点）
        const double THRESHOLD = .2;
        public static void PreFilter(ref List<Coord3D> lst)
        {
            if (lst.Count == 0)
                return;
            List<Coord3D> newlst = new List<Coord3D>();
            newlst.Capacity = lst.Count;
            newlst.Add(lst.First());
            double distance = 0;
            for (int i = 1; i < lst.Count - 1; i++)
            {
                Vector v = new Vector(newlst.Last().Plane, lst[i].Plane);
                //distance += v.Length();
                distance = v.Length();
                if ( distance >= DM.Models.Config.I.BASE_FILTER_THRES)
                {
                    newlst.Add(lst[i]);
                    distance = 0;
                    //                     lst.RemoveAt(i);
                    //                     i--;
                }
            }
            lst = newlst;
        }
        // 按方向选段
        private void Segment()
        {
//             const int START = 40;
//             const int END = 717;
            segmentedTP.Clear();
            if (filteredTP.Count < 2)
                return;
            List<Coord3D> single = new List<Coord3D>(new Coord3D[]{filteredTP[0], filteredTP[1]});
            int count = 0;
            for (int i = 1; i < filteredTP.Count-1; i++ )
            {
                Vector v = new Vector(filteredTP[i - 1].Plane, filteredTP[i].Plane);
                Coord3D pt = filteredTP[i + 1];

                // 丢包点筛选

                bool newseg = isDrawingElevation;
                if( newseg )
                {
                    TimeSpan ts = filteredTP[i].When - filteredTP[i - 1].When;
                    if (ts.TotalSeconds > Config.I.ELEV_FILTER_SECONDS)
                    {
                        newseg = true;
                        if (single.Count != 0)
                            single.RemoveAt(single.Count - 1);
                        count++;
                    }
                    else
                        newseg = false;
                }

                if ( (v.DotProductTo(pt.Plane) >= 0) || newseg)
                {
                    // 新段
                    segmentedTP.Add(single);
                    single = new List<Coord3D>();
                    single.Add(filteredTP[i]);
                }
                single.Add(pt);
            }
            if (single.Count != 0)
                segmentedTP.Add(single);

            if(isDrawingElevation)
            {
                System.Diagnostics.Debug.Print("丢包点: {0}，总共{1}", count, filteredTP.Count);
            }
            //System.Diagnostics.Debug.Print("found segments: {0}", segmentedTP.Count);
        }
        // 段内筛选
        private void FilterSegments()
        {
            if (segmentedTP.Count == 0)
                return;
            filteredSeg = new List<List<Coord3D>>(segmentedTP);
            for (int i = 0; i < segmentedTP.Count; i++ )
            {
                filteredSeg[i] = new List<Coord3D>(segmentedTP[i]);
            }
            const double THRES_HEADTAIL = 0.6;
            foreach (List<Coord3D> lst in filteredSeg)
            {
                if (lst.Count <= 3)
                    continue;

                for (int i = 0; i < lst.Count - 1; i++)
                {
                    Coord3D h1 = lst[i];
                    Coord3D h2 = lst[i + 1];
                    Vector header = new Vector(h1.Plane, h2.Plane);
                    if (header.Length() < THRES_HEADTAIL)
                    {
                        if (i != (lst.Count - 2)) // 是否末尾
                        {
                            lst.RemoveAt(i+1);
                            i--;
                        }
                        else
                        {
                            //TRACE("found to end of list {0}", i);
                            lst.RemoveAt(i);
                        }
                    }

                    if (lst.Count <= 2)
                        break;
                }
            }
        }
        DMRectangle scrBoundary = new DMRectangle();
        private void ResetBoundary()
        {
            scrBoundary.Left = scrBoundary.Top = float.MaxValue;
            scrBoundary.Right = scrBoundary.Bottom = float.MinValue;
        }
        private void FilterBoundary(Coord c)
        {
            scrBoundary.Left = Math.Min(scrBoundary.Left, c.X);
            scrBoundary.Top = Math.Min(scrBoundary.Top, c.Y);
            scrBoundary.Right = Math.Max(scrBoundary.Right, c.X);
            scrBoundary.Bottom = Math.Max(scrBoundary.Bottom, c.Y);
        }

        bool isFirst = true;
        bool ISCOMMAND = false;

        private void CreatePath(double offScrX, double offScrY)
        {
            if (owner.Owner.DeckInfo.LibrateState == 3)
                ISCOMMAND = true;
            isFirst = true;
//             lock(sync)
//             {
                if (filteredSeg.Count == 0)
                    return;
                libratedTracking.Clear();
                gpALLNOTracking.Clear();
                gpTracking.Clear();
                gpBand.Clear();
                gpOverspeed.Clear();
                screenSegFiltered.Clear();
                gpLibrated.Clear();
                libratedOKTracking.Clear();
                screenSegLibrated.Clear();
                //hasReadCar.Clear();
                //alltimes.Clear();

                screenSeg = new List<List<Coord3D>>(filteredSeg);
                for (int i = 0; i < filteredSeg.Count; i++)
                {
                    screenSeg[i] = new List<Coord3D>(filteredSeg[i]);
                }
                Layer ownerLayer = owner.Owner.Owner;
        
                for (int i = 0; i < screenSeg.Count; i++)
                {
                    for (int j = 0; j < screenSeg[i].Count; j++)
                    {
                        Coord c = new Coord(ownerLayer.DamToScreen(screenSeg[i][j].Plane));
                        c = c.Offset(offScrX, offScrY);
                        screenSeg[i][j] = new Coord3D(c.X, c.Y, screenSeg[i][j].Z, screenSeg[i][j].V, screenSeg[i][j].tag1, filteredSeg[i][j].When);
                        //FilterBoundary(c);
                    }
                }
                RectangleF rc = new RectangleF();
               //筛选击震力不合格点  feiying 09.3.19
                List<Timeslice> times=new List<Timeslice>();
                int carindex=0;

               //一个车只读一次库  
                foreach (DB.CarDistribute id in hasReadCar)
                {
                    if(id.DTStart.Equals(owner.Assignment.DTStart)&&id.Carid==owner.Assignment.Carid)
                    {
                        isFirst = false;
                        break;
                    }
                    carindex++;
                }
                
                if (isFirst)
                {
                    times = FiterLibrated();
                    hasReadCar.Add(owner.Assignment);
                    alltimes.Add(times);
                    carindex = -1;
                }

                if (carindex != -1)
                    times = alltimes[carindex];


                
                foreach (List<Coord3D> lst in screenSeg)
                {
                    // 筛选超速点
                    //int count = 0;
                    List<Coord3D> onelist = new List<Coord3D>();
                    List<List<Coord3D>> lstoflst = new List<List<Coord3D>>();
                    List<Coord3D> libratedNO = new List<Coord3D>();
                    List<List<Coord3D>> libratedNOlst = new List<List<Coord3D>>();//存放所有振动不合格段
                    List<Coord3D> libratedOK = new List<Coord3D>();//存放所有筛选过后振动合格的点
                    List<List<Coord3D>> libratedOKlst = new List<List<Coord3D>>();//存放所有筛选过后振动合格的点
                    
                    bool overspeeding = (lst[0].V >= owner.Owner.DeckInfo.MaxSpeed);
                    onelist.Add(lst[0]);
                    lstoflst.Add(onelist);

                    int index = GetCarIDIndex(owner.ID);
                    bool isRight = VehicleControl.carLibratedStates[index] == owner.Owner.DeckInfo.LibrateState || VehicleControl.carLibratedStates[index] == -1;
                    bool isbreak = false;
                    bool hasNOlibrated = false;//第一个不合格list开关量
                    //bool BFWHEN = false;//实时和数据库点交替开关
                    
                    DateTime when = lst[0].When;
                    //if (when < SetTime)
                    //{
                        if (times.Count > 0)
                        {
                            for (int j = 0; j < times.Count; j++)
                            {
                                if (when < times[j].DtEnd && when > times[j].DtStart)
                                {
                                    //libratedNO.Add(lst[0]);
                                    hasNOlibrated = true;
                                    isbreak = true;
                                    break;
                                }
                            }
                            if (!isbreak)
                            {
                                libratedOK = new List<Coord3D>();
                                libratedOK.Add(lst[0]);
                                libratedOKlst.Add(libratedOK);
                            }
                            else
                            {
                                libratedNO = new List<Coord3D>();
                                libratedNO.Add(lst[0]);
                                libratedNOlst.Add(libratedNO);
                            }
                        }
                        else
                        {
                            libratedNO = new List<Coord3D>();
                            libratedOK.Add(lst[0]);
                            libratedOKlst.Add(libratedOK);
                        }
                    //}
                    //else
                    //{
                    //    hasNOlibrated = false;
                    //    if (true)//!ISCOMMAND&&VehicleControl.carLibratedStates[index] == owner.Owner.DeckInfo.LibrateState||VehicleControl.carLibratedStates[index]==-1)
                    //    {
                    //        isRight = true;
                    //        libratedOK = new List<Coord3D>();
                    //        libratedOK.Add(lst[0]);
                    //        libratedOKlst.Add(libratedOK);
                    //    }
                    //    else if (ISCOMMAND && VehicleControl.carLibratedStates[index] == 2 || VehicleControl.carLibratedStates[index] == 1 || VehicleControl.carLibratedStates[index] == -1)
                    //    {
                    //        isRight = true;
                    //        libratedOK = new List<Coord3D>();
                    //        libratedOK.Add(lst[0]);
                    //        libratedOKlst.Add(libratedOK);
                    //    }
                    //    else
                    //    {
                    //        isRight = false;
                    //        libratedNO = new List<Coord3D>();
                    //        libratedNO.Add(lst[0]);
                    //        libratedNOlst.Add(libratedNO);
                    //    }
                    //}


                    isbreak = false;
                    Coord3D previous = lst[0];

                    for (int i = 1; i < lst.Count; i++)
                    {
                        when=lst[i].When;
                        isbreak = false; 
                        //if ( when< SetTime /*|| VehicleControl.carLibratedTimes[index].Equals(DateTime.MinValue)*/)
                        //{
                            if (times.Count > 0)
                            {
                                for (int j = 0; j < times.Count; j++)
                                {
                                    if (when< times[j].DtEnd && when > times[j].DtStart)
                                    {
                                        if (when - lst[i - 1].When < TimeSpan.FromSeconds(Config.I.LIBRATE_Secends))
                                        {
                                            libratedNO.Add(lst[i]);
                                            isbreak = true;
                                            //hasNOlibrated = true;
                                            break;
                                        }
                                        else
                                        {
                                            libratedNOlst.Add(libratedNO);
                                            libratedNO = new List<Coord3D>();
                                            libratedNO.Add(lst[i]);
                                            libratedNOlst.Add(libratedNO);
                                            isbreak = true;
                                            hasNOlibrated = true;
                                            break;
                                        }
                                    }
                                }
                                if (!isbreak)
                                {
                                    libratedOK.Add(lst[i]);
                                }
                            }
                            else
                            {
                                libratedOK.Add(lst[i]);
                            }
                        //}
                        //else
                        //{
                        //    hasNOlibrated = false;
                        //    if (true)//!ISCOMMAND&&VehicleControl.carLibratedStates[index] == owner.Owner.DeckInfo.LibrateState||VehicleControl.carLibratedStates[index] == -1)
                        //    {
                        //        if (isRight)
                        //            libratedOK.Add(lst[i]);
                        //        else
                        //        {
                        //            libratedOK = new List<Coord3D>();
                        //            libratedOK.Add(lst[i]);
                        //            libratedOKlst.Add(libratedOK);
                        //        }
                        //        isRight = true;
                        //    }
                        //    else if (ISCOMMAND && VehicleControl.carLibratedStates[index] == 2 || VehicleControl.carLibratedStates[index] == 1 || VehicleControl.carLibratedStates[index] == -1)
                        //    {
                        //        if (isRight)
                        //            libratedOK.Add(lst[i]);
                        //        else
                        //        {
                        //            libratedOK = new List<Coord3D>();
                        //            libratedOK.Add(lst[i]);
                        //            libratedOKlst.Add(libratedOK);
                        //        }
                        //        isRight = true;
                        //    }
                        //    else
                        //    {
                        //        if (!isRight)
                        //            libratedNO.Add(lst[i]);
                        //        else
                        //        {
                        //            libratedNO = new List<Coord3D>();
                        //            libratedNO.Add(lst[i]);
                        //            libratedNOlst.Add(libratedNO);
                        //        }
                        //        isRight = false;
                        //    }
                        //}
                        

                        if (lst[i].V >= owner.Owner.DeckInfo.MaxSpeed)
                        {
                            if (!overspeeding)
                            {
                                onelist = new List<Coord3D>();
                                onelist.Add(previous);
                                lstoflst.Add(onelist);
                                //System.Diagnostics.Debug.Print("未超速->超速");
                                //                             gpOverspeed.SetVertex(true);
                            }
                            overspeeding = true;
                        }
                        else
                        {
                            if (overspeeding)
                            {
                                onelist = new List<Coord3D>();
                                onelist.Add(previous);
                                lstoflst.Add(onelist);
                                //System.Diagnostics.Debug.Print("超速->未超速");
                                //                             gpOverspeed.SetVertex(false);
                            }
                            overspeeding = false;
                        }
                        onelist.Add(lst[i]);
                        previous = lst[i];
                    }
                    if (hasNOlibrated && (libratedNOlst.Count == 0))
                    {
                        libratedNOlst.Add(libratedNO);
                    }
                    //System.Diagnostics.Debug.Print("舍弃超速点{0}个", count);
                    using (Pen p = WidthPen(Color.Black))
                        for (int i = 0; i < lstoflst.Count; i++)
                        {
                            if (lstoflst[i].Count < 2)
                                continue;
                            GraphicsPath gp = new GraphicsPath();
                            PointF[] plane = Geo.DamUtils.Translate(lstoflst[i]);
                            //                 if (inCurve)
                            //                     gp.AddCurve(plane);
                            //                 else
                            screenSegFiltered.Add(lstoflst[i]);
                            gp.AddLines(plane);
                            rc = RectangleF.Union(rc, gp.GetBounds(new Matrix(), p));
                            gpTracking.Add(gp);
                            gpOverspeed.Add(lstoflst[i].Last().V >= owner.Owner.DeckInfo.MaxSpeed);
                        }
                    using (Pen p = WidthPen(Color.Black))
                        for (int i = 0; i < libratedOKlst.Count; i++)
                        {
                            if (libratedOKlst[i].Count < 2)
                                continue;
                            PointF[] pf = Geo.DamUtils.Translate(libratedOKlst[i]);
                            GraphicsPath path = new GraphicsPath();
                            path.AddLines(pf);
                            rc = RectangleF.Union(rc, path.GetBounds(new Matrix(), p));
                            screenSegLibrated.Add(libratedOKlst[i]);
                            libratedOKTracking.Add(path);
                        }
                    using (Pen p = WidthPen(Color.Black))
                        for (int i = 0; i < libratedNOlst.Count; i++)
                        {
                            if (libratedNOlst[i].Count < 2)
                                continue;
                            PointF[] pf = Geo.DamUtils.Translate(libratedNOlst[i]);
                            GraphicsPath path = new GraphicsPath();
                            path.AddLines(pf);
                            rc = RectangleF.Union(rc, path.GetBounds(new Matrix(), p));
                            libratedTracking.Add(path);
                        }
                   
                    //取出振动合格的点列表放入screenSegLibrated
                }
               
                //// John, 2009-1-19
                //if (Config.I.IS_OVERSPEED_VALID)
                //{
                //    foreach (List<Coord3D> elem in screenSeg)
                //    {
                //        GraphicsPath gp = new GraphicsPath();
                //        PointF[] lines = Geo.DamUtils.Translate(elem);
                //        gp.AddLines(lines);
                //        gpBand.Add(gp);
                //    }
                //}
                //else
                //{
                //    for (int i = 0; i < gpTracking.Count; i++ )
                //    {
                //        GraphicsPath gp = gpTracking[i];
                //        if( !gpOverspeed[i] )
                //            gpBand.Add(gp.Clone() as GraphicsPath);
                //    }
                //}
               //////////////////////////////////////////////////////////////////feiying
                if (Config.I.IS_LIBRATE_VALID && Config.I.IS_OVERSPEED_VALID)
                {
                    foreach (List<Coord3D> elem in screenSeg)
                    {
                        GraphicsPath gp = new GraphicsPath();
                        PointF[] lines = Geo.DamUtils.Translate(elem);
                        gp.AddLines(lines);
                        gpBand.Add(gp);
                    }
                }
                else if (Config.I.IS_LIBRATE_VALID && !Config.I.IS_OVERSPEED_VALID)//只击震力算做遍数超速不算做遍数
                {
                    for (int i = 0; i < gpTracking.Count; i++)
                    {
                        GraphicsPath gp = gpTracking[i];
                        if (!gpOverspeed[i])
                            gpBand.Add(gp.Clone() as GraphicsPath);
                    }
                }
                else if (!Config.I.IS_LIBRATE_VALID && Config.I.IS_OVERSPEED_VALID)//只超速算做遍数击震力不算做遍数
                {
                    for (int i = 0; i < libratedOKTracking.Count; i++)
                    {
                        GraphicsPath gp = libratedOKTracking[i];
                        gpBand.Add(gp.Clone() as GraphicsPath);
                    }
                }
                else                                                                //全部不计入遍数
                {
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
 /////////////////////从振动合格的点中筛选超速点、//////////////////
                    foreach (List<Coord3D> lst in screenSegLibrated)
                    {
                        // 筛选超速点
                        //int count = 0;
                        List<Coord3D> onelist = new List<Coord3D>();
                        List<List<Coord3D>> lstoflst = new List<List<Coord3D>>();

                        bool overspeeding = (lst[0].V >= owner.Owner.DeckInfo.MaxSpeed);
                        onelist.Add(lst[0]);
                        lstoflst.Add(onelist);

                        Coord3D previous = lst[0];

                        for (int i = 1; i < lst.Count; i++)
                        {
                            if (lst[i].V >= owner.Owner.DeckInfo.MaxSpeed)
                            {
                                if (!overspeeding)
                                {
                                    onelist = new List<Coord3D>();
                                    onelist.Add(previous);
                                    lstoflst.Add(onelist);
                                    //System.Diagnostics.Debug.Print("未超速->超速");
                                    //                             gpOverspeed.SetVertex(true);
                                }
                                overspeeding = true;
                            }
                            else
                            {
                                if (overspeeding)
                                {
                                    onelist = new List<Coord3D>();
                                    onelist.Add(previous);
                                    lstoflst.Add(onelist);
                                    //System.Diagnostics.Debug.Print("超速->未超速");
                                    //                             gpOverspeed.SetVertex(false);
                                }
                                overspeeding = false;
                            }
                            onelist.Add(lst[i]);
                            previous = lst[i];
                        }

                        using (Pen p = WidthPen(Color.Black))
                            for (int i = 0; i < lstoflst.Count; i++)
                            {
                                if (lstoflst[i].Count < 2)
                                    continue;
                                GraphicsPath gp = new GraphicsPath();
                                PointF[] plane = Geo.DamUtils.Translate(lstoflst[i]);
                                //                 if (inCurve)
                                //                     gp.AddCurve(plane);
                                //                 else
                                gp.AddLines(plane);
                                rc = RectangleF.Union(rc, gp.GetBounds(new Matrix(), p));
                                gpALLNOTracking.Add(gp);
                                gpOverspeed.Add(lstoflst[i].Last().V >= owner.Owner.DeckInfo.MaxSpeed);
                            }
                    }
                    for (int i = 0; i < gpALLNOTracking.Count; i++)
                    {
                        GraphicsPath gp = gpALLNOTracking[i];
                        if (!gpOverspeed[i])
                            gpBand.Add(gp.Clone() as GraphicsPath);
                    }
                }
               

                // John, 2009-1-19

                scrBoundary = new DMRectangle(rc);
//             }
                //System.Diagnostics.Debug.Print(libratedTracking.Count.ToString());
        }

        //时间片结构体
        public struct Timeslice
        {
            DateTime dtStart;

            public DateTime DtStart
            {
                get { return dtStart; }
                set { dtStart = value; }
            }
            DateTime dtEnd;

            public DateTime DtEnd
            {
                get { return dtEnd; }
                set { dtEnd = value; }
            }
            int libratedState;

            public int LibratedState
            {
                get { return libratedState; }
                set { libratedState = value; }
            }

        }
        //获得参数车辆id的索引
        int GetCarIDIndex(int carid)
        {
            int k = 0;
            foreach (int id in VehicleControl.carIDs)
            {
                if (id == carid)
                    break;
                k++;
            }
            return k;
        }

        /// <summary>
        /// 筛选击震力时间段
        /// </summary>
        private List<Timeslice> FiterLibrated()
        {
            DateTime end;
            //不合格时间段集合
            List<Timeslice> times = new List<Timeslice>();
            Timeslice time = new Timeslice();
            if (this.owner.Owner.WorkState == DM.DB.SegmentWorkState.WAIT)
                return null;
            else if (this.owner.Assignment.DTEnd==DateTime.MinValue)
                end=DB.DBCommon.getDate();
            else 
                end=this.owner.Assignment.DTEnd;
            List<DB.LibrateInfo> lstLInfos = DB.LibrateInfoDAO.Instance.getLibrateInfosOfthisCar(this.owner.ID, this.owner.Assignment.DTStart, end);

            for (int i = 0; i < lstLInfos.Count;i++ )
            {
                if (!ISCOMMAND&&lstLInfos[i].State!=this.owner.Owner.DeckInfo.LibrateState)
                {
                    time.LibratedState = lstLInfos[i].State;
                    time = new Timeslice();
                    time.DtStart = lstLInfos[i].Dt;
                    if ((i + 1) < lstLInfos.Count)
                        time.DtEnd = lstLInfos[i + 1].Dt;
                    else
                    {
                        if (this.owner.Assignment.DTEnd == DateTime.MinValue)
                            time.DtEnd = DB.DBCommon.getDate();
                        else
                            time.DtEnd = this.owner.Assignment.DTEnd;
                    }
                    times.Add(time);
                }
                else if (ISCOMMAND && lstLInfos[i].State == 0 || lstLInfos[i].State == 3)
                {
                    time.LibratedState = lstLInfos[i].State;
                    time = new Timeslice();
                    time.DtStart = lstLInfos[i].Dt;
                    if ((i + 1) < lstLInfos.Count)
                        time.DtEnd = lstLInfos[i + 1].Dt;
                    else
                    {
                        if (this.owner.Assignment.DTEnd == DateTime.MinValue)
                            time.DtEnd = DB.DBCommon.getDate();
                        else
                            time.DtEnd = this.owner.Assignment.DTEnd;
                    }
                    times.Add(time);
                }
            }


            List<Timeslice> Timeslices = new List<Timeslice>();
            if (times.Count < 2)////////////////筛选连接不合格的时间片
                return times;
            else
            {
                for (int i = 0; i < times.Count; )
                {
                    Timeslice thistime = times[i];
                    if (i == times.Count - 1) { Timeslices.Add(thistime); break; } 
                    for (int j = i + 1; j < times.Count; j++)
                    {
                        if (times[j].DtStart == thistime.DtEnd)
                        {
                            thistime.DtEnd = times[j].DtEnd;
                            if (j == times.Count - 1)
                            {
                                Timeslices.Add(thistime);
                                i = j;
                            }
                        }
                        else
                        {
                            Timeslices.Add(thistime);
                            i = j;
                            break;
                        }
                    }
                }
                return Timeslices;
            }
        }
        private float WidthPen()
        {
            return (float)Owner.Owner.Owner.ScreenSize(owner.WheelWidth);
        }
        private Pen WidthPen(Color cl)
        {
            return new Pen(cl, WidthPen());
        }
        object sync = new object();
        public void Draw(Graphics g, bool frameonly)
        {
            lock(sync)
            {

                if (frameonly)
                {
                    for (int i = 0; i < gpTracking.Count; i++)
                    {
                        g.DrawPath(Pens.Black, gpTracking[i]);
                    }
                    return;
                }
                Layer ownerLayer = owner.Owner.Owner;
                //double screenMeters = ownerLayer.ScreenSize(owner.WheelWidth);
                using (Pen p = WidthPen(Color.FromArgb(0x20, cl)))
                {
                    p.LineJoin = LineJoin.Round;
                    // 画碾压带
//                     for (int i = 0; i < gpTracking.Count; i++)
//                     {
//                         if (!gpOverspeed[i])
//                             g.DrawPath(p, gpTracking[i]);
//                     }
                    for (int i = 0; i < gpBand.Count; i++)
                    {
                        g.DrawPath(p, gpBand[i]);
                    }
                }
            }
            //DrawAnchor(g);
        }
        private void DrawToken(Graphics g, Coord3D c, Color cl1, Color cl2, Color cl3)
        {
            float radius = (float)owner.Owner.Owner.ScreenSize(0.1);
            RectangleF rc = new RectangleF(c.Plane.XF - radius, c.Plane.YF - radius, radius * 2, radius * 2);
            using(Brush b = new SolidBrush(cl1))
            {
                g.FillEllipse(b, rc);
            }
            using(Pen p = new Pen(cl2, radius*.3f))
                g.DrawEllipse(p, rc);
            rc.Inflate(-radius * .6f, -radius * .6f);
            using (Pen p = new Pen(cl3, radius*.3f))
            {
                g.DrawLine(p, new PointF(rc.Left, rc.Top), new PointF(rc.Right, rc.Bottom));
                g.DrawLine(p, new PointF(rc.Left, rc.Bottom), new PointF(rc.Right, rc.Top));
            }
        }
        private void DrawPointOK(Graphics g, Coord3D c)
        {
            DrawToken(g, c, Color.Gray, Color.Black, Color.Transparent);
        }
        bool drawOverspeed = true;
        public bool DrawOverSpeed { get { return drawOverspeed; } set { drawOverspeed = value; } }
        public void DrawSkeleton(Graphics g, bool drawingArrows)
        {
            lock(sync)
            {
//                 foreach (List<Coord3D> lst in screenSeg)
//                 {
//                     foreach (Coord3D c in lst)
//                     {
//                         if (c.tag1 == 1)
//                         {
//                             g.FillEllipse(Brushes.PaleVioletRed, c.Plane.XF - 5, c.Plane.YF - 5, 10, 10);
//                         }
//                         else if (c.tag1 == 2)
//                         {
//                             g.FillEllipse(Brushes.Navy, c.Plane.XF - 5, c.Plane.YF - 5, 10, 10);
//                         }
//                     }
//                 }

                float scrSize = (float)owner.Owner.Owner.ScreenSize(0.05);
                scrSize = Math.Max(scrSize, 0.1f);
                scrSize = Math.Min(scrSize, 0.8f);
                float size = 1;// (float)owner.Owner.Owner.ScreenSize(.15f);
                using (Pen p = new Pen(Color.FromArgb(0xFF, this.Color), size),
                    p1 = new Pen(Color.Yellow, 1.8f),
                    p2 = new Pen(Color.Black, 2.7f))
                    for (int i = 0; i < gpTracking.Count; i++)
                    {
                        //p.EndCap = LineCap.ArrowAnchor;
                        if (g.SmoothingMode == SmoothingMode.AntiAlias && drawingArrows)
                            p.CustomEndCap = new AdjustableArrowCap(scrSize * 3, scrSize * 12, true);

                        if (gpOverspeed[i])
                        {
                            if (drawOverspeed)
                            {
                                g.DrawPath(p2, gpTracking[i]);
                                g.DrawPath(p1, gpTracking[i]);
                            }
                        }
                        else
                            g.DrawPath(p, gpTracking[i]);
                    }
                ///////////////////////////////////////////////////////////feiying 09.3.22
                using (Pen p1 = new Pen(Color.Red, size))
                foreach (GraphicsPath path in libratedTracking)
                {
                    g.DrawPath(p1,path);
                }
            }
        }
        public void DrawAnchor(Graphics g)
        {
            lock(sync)
            {
                // 画最后轨迹点箭头
                //             if (!anchor)
                //             {
                //                 isPainting = false;
                //                 return;
                //             }
                if (screenSeg.Count == 0 || filteredSeg.Count==0)
                {
                    return;
                }
                List<Coord3D> lst = screenSeg.Last(); // 最后一段
                List<Coord3D> filteredLst = filteredSeg.Last();
                if (lst.Count < 2 || filteredLst.Count < 2)
                {
                    return;
                }
                Coord c1 = lst[lst.Count - 2].Plane;  // 倒数第2点

                Coord c2 = lst[lst.Count - 1].Plane;  // 最后1点

                using (Pen p = new Pen(Color.Black, (float)owner.Owner.Owner.ScreenSize(0.5)))
                {
                    p.EndCap = LineCap.ArrowAnchor;
                    g.DrawLine(p, c1.PF, c2.PF);
                }
                //DrawAnchor(g, lst[lst.Count - 1], filteredSeg.Last().Last());

                Coord3D lastptScr = screenSeg.Last().Last();
                Coord3D lastpt = filteredSeg.Last().Last();
                Coord cp = lastptScr.Plane;
                RectangleF rc = new RectangleF((float)cp.X, (float)cp.Y, 1000, 500);
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Near;
                sf.FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.NoWrap;
                rc.Offset(10, 3);
                //写实时点的击震力状态。feiying 09.3.20
                
                //lstLInfos = DB.LibrateInfoDAO.Instance.getLastLibrateInfosOfthisCar(this.owner.ID, this.owner.Assignment.DTStart, end);
                //DateTime dtnow=DB.DBCommon.getDate();
                int i=GetCarIDIndex(owner.ID);
                DateTime dtnow =DB.DBCommon.getDate();
                string libratedstring=string.Empty;
                if (VehicleControl.carLibratedStates[i] == -1 || !owner.Assignment.IsWorking()/*||this.owner.Assignment.DTEnd < lstLInfos.Last().Dt*/)
                    libratedstring = string.Empty;
                else if (dtnow > VehicleControl.carLibratedTimes[i] && (dtnow - VehicleControl.carLibratedTimes[i]) > TimeSpan.FromSeconds(240))
                    libratedstring = string.Empty;
                else 
                    libratedstring = "（"+Forms.Warning.GetLibratedString(VehicleControl.carLibratedStates[i])+"）";

                string strInfo = string.Format("{0}", Owner.Info.CarName+libratedstring, lastpt.Plane.ToString());
                string strVelocity = string.Format("{0:0.00} km/h", lastpt.V);

                FontStyle fs = FontStyle.Regular;
                float emsize = .8f;
                float emsize1 = (float)owner.Owner.Owner.ScreenSize(1);
                Color cl = Color.Navy;
                float maxspeed = (float)owner.Owner.DeckInfo.MaxSpeed;
                if (lastpt.V >= maxspeed)
                {
                    fs = FontStyle.Bold;
                    emsize *= 1 + ((float)lastpt.V - maxspeed) / maxspeed;
                    emsize = Math.Min(emsize, 48);
                    cl = Color.OrangeRed;
                }
                emsize = (float)owner.Owner.Owner.ScreenSize(emsize);
                Font orig = new Font(owner.Owner.Owner.Owner.Font.FontFamily, emsize1, FontStyle.Bold);
                Font ft = new Font(orig.FontFamily, emsize, fs);
                SizeF size1 = g.MeasureString(strInfo, orig);
                SizeF size2 = g.MeasureString(strVelocity, ft);
                rc.Width = Math.Max(size1.Width, size2.Width);
                float height1 = size1.Height;
                float height2 = size2.Height;
                RectangleF rc1 = rc;
                rc1.Offset(0, height1);
                //             orig = new Font(orig, FontStyle.Bold);
                using (Brush b1 = new SolidBrush(Color.FromArgb(0xff, this.Color)), b2 = new SolidBrush(Color.FromArgb(0x50, Color.WhiteSmoke)))
                    Utils.Graph.OutGlow.DrawOutglowText(g, strInfo, orig, rc, sf, b1, b2);

                if (owner.Assignment.IsWorking())
                    using (Brush b1 = new SolidBrush(Color.FromArgb(0xff, cl)), b2 = new SolidBrush(Color.FromArgb(0x50, Color.WhiteSmoke)))
                        Utils.Graph.OutGlow.DrawOutglowText(g, strVelocity, ft, rc1, sf, b1, b2);
            }

        }
        public void SetOrigin(Coord neworig)
        {
            for (int i = 0; i < this.screenSeg.Count; i++ )
            {
                List<Coord3D> s = this.screenSeg[i];
                SetOrigin(ref s, neworig);
                this.screenSeg[i] = s;
            }
        }
        // 重置原点
        public static void SetOrigin(ref List<Coord3D> lst, Coord neworig)
        {
            Coord origin = new Coord(double.MaxValue, double.MaxValue);
            for (int i = 0; i < lst.Count; i++)
            {
                Coord3D c3d = lst[i];
                Coord c = c3d.Plane;
                if (origin.X > c.X)
                    origin.X = c.X;
                if (origin.Y > c.Y)
                    origin.Y = c.Y;
            }

            for (int i = 0; i < lst.Count; i++)
            {
                Coord3D c3d = lst[i];
                Coord c = c3d.Plane;
                c = c.Offset(origin.Negative());
                c = c.Offset(neworig);
                lst[i] = new Coord3D(c, c3d.Z);
            }
        }
        public int RollCount(PointF scrPoint)
        {
            lock(sync)
            {
                if (!this.scrBoundary.Contains(scrPoint))
                    return 0;
                int count = 0;
                using (Pen p = WidthPen(Color.Black))
                    //for (int i = 0; i < gpTracking.Count; i++)
                    for (int i = 0; i < gpBand.Count; i++)
                    {
                        if (/*!gpOverspeed[i] &&*/ gpBand[i].IsOutlineVisible(scrPoint, p))
                            count++;
                    }
                //using (Pen p = WidthPen(Color.Black))
                //    for (int i = 0; i < libratedTracking.Count; i++)
                //    {
                //        if (/*!gpOverspeed[i] &&*/ libratedTracking[i].IsOutlineVisible(scrPoint, p))
                //            count++;
                //    }
                return count;
            }
        }
        public void MaxMin(out double lo, out double hi)
        {
            lo = -1;
            hi = -1;
            if (this.filteredTP.Count == 0)
                return;
            double max = filteredTP[0].Z;
            double min = max;
            foreach (Coord3D c in filteredTP)
            {
                if (owner.Owner.RectContains(c.Plane))
                {
                    max = Math.Max(max, c.Z);
                    min = Math.Min(min, c.Z);
                }
            }
            lo = min;
            hi = max;
        }
        bool isDrawingElevation = false;
        List<Coord3D> old;
        public void FilterForOutput()
        {
            List<Coord3D> lst = new List<Coord3D>();
            DB.Segment dkinfo = owner.Owner.DeckInfo;

            double lo = dkinfo.StartZ + dkinfo.DesignDepth * Config.I.ELEV_FILTER_ELEV_LOWER;
            double hi = dkinfo.StartZ + dkinfo.DesignDepth * Config.I.ELEV_FILTER_ELVE_UPPER;
            double speedmax = dkinfo.MaxSpeed*Config.I.ELEV_FILTER_SPEED;

            for (int i = 0; i < origTP.Count; i++ )
            {
                double z = origTP[i].Z;
                double v = origTP[i].V;
                if (z >= lo && z <= hi && (v<speedmax))
                    lst.Add(origTP[i]);
            }

            old = origTP;
            isDrawingElevation = true;
            SetTracking(lst, 0, 0);
            isDrawingElevation = false;
            //origTP = old;
        }
        public void Reset()
        {
            SetTracking(old, 0, 0);
        }

        public void DrawElevation(Graphics g, double lo, double hi)
        {
            float delta = (float)(hi - lo);
            if (delta <= 0)
                return;

            for (int idx = 0; idx < screenSeg.Count; idx++ )
            {
//                 if (gpOverspeed[idx])
//                     continue;
                List<Coord3D> seg = screenSeg[idx];
                Vector test = new Vector(seg.First(), seg.Last());
                if (test.Length() < 0.01)
                    continue;

                float distance = 0;
                List<float> distances = new List<float>(); // 距离
                List<float> factors = new List<float>();    // 因子
                List<float> subdistances = new List<float>();    // 子距离, 0~1
                distances.Add(0);
                double f = (seg.First().Z - lo) / delta;
                f = Math.Max(0, f);
                f = Math.Min(1, f);
                factors.Add((float)f);
                for (int i = 1; i < seg.Count; i++)
                {
                    Vector v = new Vector(seg[i - 1], seg[i]);
                    float length = (float)v.Length();
                    distance += length;
                    distances.Add(distance);
                    f = (seg[i].Z - lo) / (delta);
                    f = Math.Max(0, f);
                    f = Math.Min(1, f); 
                    factors.Add((float)f);
                }
                List<Color> colors = new List<Color>();
                for (int i = 0; i < distances.Count; i++)
                {
                    float subdistance = distances[i] / distance;
                    int gray = (int)(factors[i]*255);
                    colors.Add(Color.FromArgb(gray,gray,gray));
                    subdistances.Add(subdistance);
                }

                GraphicsPath gp = new GraphicsPath();
                gp.AddLines(Geo.DamUtils.TranslatePoints(seg).ToArray());
                LinearGradientBrush br = new LinearGradientBrush(seg.First().Plane.PF, seg.Last().Plane.PF, Color.Black, Color.White);
                Blend bl = new Blend();
                bl.Factors = factors.ToArray();
                bl.Positions = subdistances.ToArray();
                br.Blend = bl;

                br.WrapMode = WrapMode.TileFlipXY;
                Pen p = new Pen(br);
                p.Width = WidthPen();
                g.DrawPath(p, gp);

                p.Dispose();
                br.Dispose();
                gp.Dispose();
//                 GraphicsPath gp = (GraphicsPath)gpTracking[idx].Clone();
//                 using (Pen p = WidthPen(Color.White))
//                 {
//                     gp.Widen(p);
//                     using (PathGradientBrush b = new PathGradientBrush(gp))
//                     {
//                         //b.CenterColor = Color.Black;
//                         ColorBlend cb = new ColorBlend();
//                         cb.Colors = colors.ToArray();
//                         cb.Positions = subdistances.ToArray();
//                         b.InterpolationColors = cb;
// 
//                         //b.WrapMode = WrapMode.TileFlipXY;
//                         g.FillPath(b, gp);
//                     }
//                     gp.Dispose();
//                 }
            }
        }
    }
}

