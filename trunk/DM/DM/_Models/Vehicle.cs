using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Xml.Serialization;
using DM.Models;
using DM.DMControl;
using System.Drawing;

namespace DM.Models
{
    /// <summary>
    /// 车辆（压路机）

    /// </summary>
    public class Vehicle : IDisposable
    {
        public Vehicle() { Init(); }
        public Vehicle(Deck o) { owner = o; Init(); }
        public Vehicle(DB.CarDistribute car)
        {
            Init();
            this.Assignment = car;
        }
        Deck owner = null;
        //TrackGPS trackGPS = null;
        TrackGPSControl trackCtrl = new TrackGPSControl();
        private void Init()
        {
            TrackGPSControl.Owner = this;
            info.ScrollWidth = 2.17;

//             DMControl.GPSReceiver.OnResponseData -= OnGPSData;
//             DMControl.GPSReceiver.OnResponseData += OnGPSData;
        }
        public void Dispose()
        {
            lock(disposing)
            {
                TurnOffGPS();
                trackCtrl.Dispose();
                GC.SuppressFinalize(this);
                isDisposed = true;
            }
        }
        public void TurnOffGPS()
        {
            DMControl.GPSServer.OnResponseData -= OnGPSData;
        }
        public void ListenGPS()
        {
            DMControl.GPSServer.OnResponseData -= OnGPSData;
            DMControl.GPSServer.OnResponseData += OnGPSData;
        }
        object disposing = new object();
        bool isDisposed = false;
        private void OnGPSData(object sender, DM.DMControl.GPSCoordEventArg e)
        {
            lock(disposing)
            {
                if (isDisposed)
                    return;
                if (e.msg != DM.DMControl.GPSMessage.GPSDATA)
                    return;
                if (e.gps.CarID != this.ID)
                    return;
                if (!dist.IsWorking())
                    return;

                //this.TrackGPSControl.CheckOverThickness(e.gps.Coord3D);

                //             if (!Owner.IsVisible)
                //                 return;
                //             DateTime start = dist.DTStart;
                //             DateTime end = dist.DTEnd;
                //             if (end == DateTime.MinValue)
                //                 end = DateTime.MaxValue;
                //             if (e.gps.Time < start || e.gps.Time > end)
                //                 return;

                // 这个点属于该车段
                if (owner.IsVisible)
                {
                    if (!owner.Owner.RectContains(e.gps.Coord3D.Plane))
                        return;
                    SoundControl.BeepIII();
                    
                    // John, 2009-1-20
                    if( TrackGPSControl.Tracking.Count != 0 )
                    {
                        // 判断超厚
                        // 先要看是否停在某个固定点
                        DM.Geo.Vector v = new DM.Geo.Vector(e.gps.Coord3D.Plane, TrackGPSControl.Tracking.TrackPoints.Last().Plane);
                        if (v.Length() > Config.I.OVERTHICKNESS_DISTANCE)
                            owner.CheckOverThickness(e.gps.Coord3D);
                    }

                    TrackGPSControl.Tracking.AddOnePoint(e.gps.Coord3D, 0, 0);
                    owner.Owner.Owner.RequestPaint();
                }
            }
        }
//         int id = 0;
//         string name = DateTime.Now.ToString("o", CultureInfo.InvariantCulture);
//         float wheelWidth = 2.17f; // width of wheel, in meters
//         float height = 2.0f; // height of this vehicle, in meters
        DB.CarDistribute dist = new DM.DB.CarDistribute();
        DB.CarInfo info = new DM.DB.CarInfo();

        public DB.CarDistribute Assignment
        {
            get { return dist; }
            set { dist = value; if (dist != null) info = DMControl.VehicleControl.FindVechicle(dist.Carid); }
        }
        public DB.CarInfo Info
        {
            get { return info; }
            set { info = value; }
        }
        private DB.CarInfo FindThis() { return DMControl.VehicleControl.FindVechicle(ID); }
        public int ID { get { return info.CarID; } set { info.CarID = value; } }
        //public string Name { get { return FindThis().CarName; } set { DMControl.VehicleControl.FindVechicle(ID).CarName = value; } }
        // 轮宽
        public float WheelWidth { get { return (float)info.ScrollWidth; } set { info.ScrollWidth = value; } }
        // 天线高度
        public float Height { get { return (float)info.GPSHeight; } }
        public TrackGPSControl TrackGPSControl { get { return trackCtrl; } set { trackCtrl= value; } }
        [XmlIgnore]
        public Deck Owner { get { return owner; } set { owner = value; } }
        public override string ToString()
        {
            return "Owner=" + owner.ToString() + ", Width="+WheelWidth.ToString("0.00")+", ID="+ID.ToString()+
                " End="+dist.DTEnd.ToString();
        }
        public int RollCount(PointF pt)
        {
            return TrackGPSControl.Tracking.RollCount(pt);
        }

        public void Draw(Graphics g, bool frameonly)
        {
            TrackGPSControl.Draw(g, frameonly);
        }
    }
}
