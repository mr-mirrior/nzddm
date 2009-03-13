using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace DM.DB
{
    public class CarInfoDAO
    {
        private CarInfoDAO(){

        }
        private static CarInfoDAO myInstance = null;

        public static CarInfoDAO getInstance()
        {
            if (myInstance == null)
            {
                myInstance = new CarInfoDAO();
            }
            return myInstance;
        }

        //返回所有车辆信息

        public List<CarInfo> getAllCarInfo(){
            List<CarInfo> carinfos = new List<CarInfo>();
            SqlConnection conn = null;
            SqlDataReader reader = null;
            try
            {
                conn = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(conn, "select * from carinfo");
                while (reader.Read())
                {
                    CarInfo carinfo = new CarInfo();
                    carinfo.CarID = (Convert.ToInt32(reader["carid"]));
                    carinfo.CarName = (reader["carname"].ToString());
                    carinfo.DeviceID = (reader["deviceid"].ToString());
                    carinfo.GPSHeight = (Convert.ToDouble(reader["gpsheight"]));
                    carinfo.PhoneNumber = (reader["phonenumber"].ToString());
                    carinfo.ScrollWidth = (Convert.ToDouble(reader["scrollwidth"]));
                    //carinfo.LibrateState = (int)reader["SenseOrganState"];
                    carinfos.Add(carinfo);
                }                
                return carinfos;
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
                return null;
            }finally{
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
            }
        }

        public CarInfo getCarInfo(List<CarInfo> carinfos, Int32 carInfoID)
        {
            foreach (CarInfo car in carinfos)
            {
                if (car.CarID == carInfoID)
                {
                    return car;
                }
            }
            return null;
        }
// 
//         public List<CarInfo> getCarInfos(List<CarInfo> carinfos, List<Int32> carInfoIDs)
//         {
//             List<CarInfo> ret = new List<CarInfo>();
// 
//             foreach (Int32 carid in carInfoIDs)
//             {
//                 ret.SetVertex(getCarInfo(carinfos, carid));
//             }
//             return ret;
//         }

        public Int32 getCarNameByCarID(List<CarInfo> carinfos, string carname)
        {
            foreach (CarInfo car in carinfos)
            {
                if (car.CarName.Equals(carname))
                {
                    return car.CarID;
                }
            }

            return -1;
        }
    }
}
;