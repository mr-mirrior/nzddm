using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DM.Models;

namespace DM.DMControl
{
    // 车辆控制，隶属于一某仓面

    public class VehicleControl: IDisposable
    {
        #region - 静态 -
        static List<DB.CarInfo> vehiclesInfo = new List<DM.DB.CarInfo>();
        //最后一条击震力数据的字典 feiying 09.3.20
        public static int[] carIDs ;
        public static int[] carLibratedStates;
        public static DateTime[] carLibratedTimes;
        public static void ReadVehicleInfo()
        {
            DB.CarInfoDAO dao = DB.CarInfoDAO.getInstance();
            vehiclesInfo = dao.getAllCarInfo();

            carIDs=new int[vehiclesInfo.Count];
            carLibratedStates= new int[carIDs.Length];
            carLibratedTimes = new DateTime[carIDs.Length];
            for (int i = 0; i < vehiclesInfo.Count;i++ )
            {
                carIDs[i] = vehiclesInfo[i].CarID;
                carLibratedStates[i] = -1;
                carLibratedTimes[i] =DateTime.MinValue;
            }
        }
        public static DB.CarInfo FindVechicle(int id)
        {
            foreach (DB.CarInfo v in vehiclesInfo)
            {
                if (v.CarID == id)
                    return v;
            }
            return null;
        }
        static List<DB.CarDistribute> vehiclesWorking = new List<DM.DB.CarDistribute>();
        public static void LoadCarDistribute()
        {
            vehiclesWorking.Clear();
            try
            {
                DB.CarDistributeDAO dao = DB.CarDistributeDAO.getInstance();
                vehiclesWorking = dao.getInusedCarDistributes();
            }
            catch
            {
                Utils.MB.Warning("数据库访问错误！");
            }
        }
        public static DB.CarDistribute FindVehicleInUse(int carid)
        {
            // test
            if( carid == 993 )
            {
                DB.CarDistribute cd = new DM.DB.CarDistribute();
                cd.Carid = 3;
                cd.Segmentid = 0;
                cd.Blockid = 13;
                cd.DesignZ = 615;
                cd.DTStart = DateTime.MinValue;
                cd.DTEnd = DateTime.MinValue;
                cd.Status = DM.DB.CarDistribute_Status.ENDWORK;
                return cd;
            }
            //test


            foreach (DB.CarDistribute cd in vehiclesWorking)
            {
                if (cd.Carid == carid)
                    return cd;
            }
            return null;
        }
        #endregion

        /*
        None：	不振
        High：	高频低振
        Low：	低频高振
        Normal：震动	//此值只适用于只有两种状态的碾压机
         */
        public enum SenseOrganState { None = 0, High = 1, Low = 2, Normal = 3 };

        List<Vehicle> vehicles = new List<Vehicle>();
        Deck owner = null;
        public void Dispose()
        {
            foreach (Vehicle v in vehicles)
            {
                v.Dispose();
            }
            GC.SuppressFinalize(this);
        }
        public Deck Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        public List<Vehicle> Vehicles
        {
            get { return vehicles; }
//             set { vehicles = value; }
        }

        public VehicleControl(){}

        public void AssignVehicle(Deck deck)
        {
            owner = deck;
            Partition p = DMControl.PartitionControl.FromName(deck.Partition.Name);
            if (p == null)
                return;
            Forms.AssignVehicle dlg = new DM.Forms.AssignVehicle();
            dlg.Deck = deck.DeckInfo;
            dlg.BlockName = DMControl.PartitionControl.FromID(deck.DeckInfo.BlockID).Name;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //this.DeleteAll();
            }
            LoadDB();
        }

        public bool AddVehicle(Vehicle v)
        {
            vehicles.Add(v);
            return true;
        }
//         public bool DeleteVehicle(Vehicle v)
//         {
//             return true;
//         }
        public void DeleteAll()
        {
            DB.CarDistributeDAO dao = DB.CarDistributeDAO.getInstance();
            //int 在这里实现_删除某仓面全部安排车辆;
            foreach (Vehicle v in this.Vehicles)
            {
                dao.removeCar(v.Assignment);
            }
            this.Clear();
        }
        public void Clear()
        {
            foreach (Vehicle vk in Vehicles)
            {
                vk.Dispose();
            }
            vehicles.Clear();
        }
        public int RollCount(PointF pt)
        {
            int count = 0;
            foreach (Vehicle vk in vehicles)
            {
                count += vk.RollCount(pt);
            }
            return count;
        }
        private List<Vehicle> Translate(List<DB.CarDistribute> lst)
        {
            Color[] cls = new Color[]{
                Color.Blue,
                Color.DarkSlateGray,
                Color.Navy,
                Color.DarkTurquoise,
                Color.SlateBlue,
                Color.DarkOliveGreen,
                Color.Green,
                Color.YellowGreen,
                Color.DodgerBlue,
                Color.CornflowerBlue,
                Color.Olive,
                Color.Teal,
                Color.Goldenrod,
                Color.Indigo,
                Color.SteelBlue,
                Color.LimeGreen

            };

            List<Vehicle> vs = new List<Vehicle>();
            if (lst == null)
                return vs;
            foreach (DB.CarDistribute cd in lst)
            {
                int color = 0;
                for (int i = 0; i < VehicleControl.vehiclesInfo.Count; i++)
                {
                    if (VehicleControl.vehiclesInfo[i].CarID == cd.Carid)
                    {
                        color = i % 16;
                        break;
                    }
                }

                Vehicle vk = new Vehicle(cd);
                vk.Owner = this.Owner;
                vk.TrackGPSControl.LoadDB();
                vk.TrackGPSControl.Tracking.Color = cls[color];
                if (cd.IsWorking())
                    vk.ListenGPS();
                vs.Add(vk);
            }
            return vs;
        }
        //private List<Vehicle> Translate(List<DB.CarDistribute> lst)
        //{
        //    Color[] cls = new Color[]{
        //        Color.Navy, 
        //        Color.LightSeaGreen,
        //        Color.DarkGreen, 
        //        Color.Black, 
        //        Color.OrangeRed, 
        //        Color.Purple,
        //        Color.Goldenrod,
        //        Color.Yellow
        //    };

        //    List<Vehicle> vs = new List<Vehicle>();
        //    if (lst == null)
        //        return vs;
        //    foreach (DB.CarDistribute cd in lst)
        //    {
        //        int color = 0;
        //        for (int i = 0; i < VehicleControl.vehiclesInfo.Count; i++)
        //        {
        //            if (VehicleControl.vehiclesInfo[i].CarID == cd.Carid)
        //            {
        //                color = i;
        //                break;
        //            }
        //        }

        //        Vehicle vk = new Vehicle(cd);
        //        vk.Owner = this.Owner;
        //        vk.TrackGPSControl.LoadDB();
        //        vk.TrackGPSControl.Tracking.Color = cls[color];
        //        if (cd.IsWorking())
        //            vk.ListenGPS();
        //        vs.Add(vk);
        //    }
        //    return vs;
        //}
        private static int VechilePriority(Vehicle v1, Vehicle v2)
        {
            if (v1.Assignment.DTStart < v2.Assignment.DTStart)
                return -1;
            if (v1.Assignment.DTStart == v2.Assignment.DTStart)
                return 0;
            return 1;
        }
        private void Sort()
        {
            vehicles.Sort(VechilePriority);
        }

        public void MaxMin(out double lo, out double hi)
        {
            lo = -1;
            hi = -1;
            if (vehicles.Count == 0)
                return;
            double max = double.MinValue;// vehicles.First().TrackGPSControl.Tracking.Heighest();
            double min = double.MaxValue;
            foreach (Vehicle v in vehicles)
            {
                double l, h;
                v.TrackGPSControl.Tracking.MaxMin(out l, out h);
                if (h != -1)
                    max = Math.Max(max, h);
                if( l != -1 )
                    min = Math.Min(min, l);
            }
            hi = max;
            lo = min;
        }
        public void LoadDB()
        {
            this.Clear();

            //int 在这里实现_从数据库读取某仓面的车辆安排情况;
            DB.CarDistributeDAO dao = DB.CarDistributeDAO.getInstance();
            if (owner == null)
                return;
            List<DB.CarDistribute> lst = dao.getCarDistributeInThisSegment_all(owner.DeckInfo.BlockID, owner.DeckInfo.DesignZ, owner.ID);
            this.vehicles = Translate(lst);

            Sort();
        }
    }
}
