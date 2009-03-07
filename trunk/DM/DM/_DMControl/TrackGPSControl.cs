using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DM.Models;
using System.Drawing;

namespace DM.DMControl
{
    public class TrackGPSControl : IDisposable
    {
        TrackGPS tracking = new TrackGPS();
        Vehicle owner;
        public Vehicle Owner
        {
            get { return owner; }
            set { owner = value; tracking.Owner = owner; }
        }

        public TrackGPS Tracking
        {
            get { return tracking; }
            set { tracking= value; }
        }
        public TrackGPSControl() { }
        public void Dispose() { tracking.Dispose(); GC.SuppressFinalize(this); }
        
        public static bool IsValid(List<Geo.Coord3D> clst, Geo.Coord3D c, DB.Segment di, DB.CarInfo ci)
        {
            //return true;
            if (clst.Count == 0)
                return true;
            if (clst.Count < 1)
                return true;
            c.Z -= ci.GPSHeight;

//             double elevMax = di.StartZ + di.DesignDepth * 1.5;
//             double elevMin = di.StartZ + di.DesignDepth * 0.5;
//             if (c.Z > elevMax || c.Z < elevMin)
//                 return false;
            TimeSpan ts = c.When - clst.Last().When;
            if (ts.TotalSeconds > Models.Config.I.BASE_FILTER_SECONDS)
                return true;

            Geo.Vector v = new DM.Geo.Vector(clst.Last(), c);
            if (v.Length() > Models.Config.I.BASE_FILTER_METERS)
                return false;
//            if (v.Length() > ts.TotalSeconds * 1)
//                return false;

            return true;
        }
        private List<Geo.Coord3D> Translate(List<DB.TracePoint> pts)
        {
            List<Geo.Coord3D> lst = new List<DM.Geo.Coord3D>();
            Geo.XYZ xyz;
            Geo.BLH blh;
            foreach (DB.TracePoint pt in pts)
            {
                blh = new DM.Geo.BLH(pt.Y, pt.X, pt.Z);
                xyz = Geo.Coord84To54.Convert(blh);
                Geo.Coord3D c3d = new DM.Geo.Coord3D(xyz.y, -xyz.x, xyz.z, (double)pt.V/100, 0);
                c3d.When = pt.Dttrace;
                c3d.Z -= owner.Info.GPSHeight;
                if( owner.Owner.Owner.RectContains(c3d.Plane) && IsValid(lst, c3d, owner.Owner.DeckInfo, owner.Info))
                    lst.Add(c3d);
            }

            return lst;
        }
        public void LoadDB()
        {
            try
            {
                List<DB.TracePoint> lst = DB.TracePointDAO.getInstance().getTracePointList(
                    owner.ID,
                    owner.Assignment.DTStart,
                    owner.Assignment.DTEnd);
                List<Geo.Coord3D> pts = Translate(lst);
                this.Tracking.SetTracking(pts, 0, 0);
            }
            catch
            {

            }
        }
        public void Draw(Graphics g, bool frameonly)
        {
            tracking.Draw(g, frameonly);
        }

        public void CheckOverThickness(Geo.Coord3D c)
        {
            DB.Segment deckinfo = owner.Owner.DeckInfo;
            double designdepth = deckinfo.DesignDepth;
            double error = deckinfo.ErrorParam/100;
            double actual = c.Z - deckinfo.StartZ;
            double hi = designdepth * (1 + error);
            
            if (actual > hi)
            {
                Geo.Coord damaxis = c.Plane.ToDamAxisCoord();
                string warning = string.Format("铺层超厚报警：限制：{0:0.00}*(1+{1:P})米, 实际：{2:0.00}米, 位置：{3}", 
                    designdepth, error, actual, damaxis.ToString());
                WarningControl.SendMessage(WarningType.OVERTHICKNESS, Owner.Owner.Partition.ID, warning);
                //System.Diagnostics.Debug.Print(warning);
            }
        }

    }
}
