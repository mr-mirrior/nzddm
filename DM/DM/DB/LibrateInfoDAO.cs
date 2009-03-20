using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace DM.DB
{
    public class LibrateInfoDAO
    {
        static LibrateInfoDAO dao = new LibrateInfoDAO();
        public static LibrateInfoDAO Instance { get { return dao; } }

        /// <summary>
        /// 获得此时间段内的所有车辆振动信息
        /// </summary>
        /// <param name="dtstart">开始时间</param>
        /// <param name="dtend">结束时间</param>
        /// <returns></returns>
        public List<LibrateInfo> getLibrateInfos(DateTime dtstart,DateTime dtend)
        {
            List<LibrateInfo> list = new List<LibrateInfo>();
            string sqltxt = "select * from SenseOrgan where DT between '" + dtstart + "' and '" + dtend + "' order by DT";
            SqlDataReader reader = DBConnection.executeQuery(DBConnection.getSqlConnection(), sqltxt);
            LibrateInfo info = new LibrateInfo();
            try
            {
                while (reader.Read())
                {
                    info.CarID = (int)reader["CarID"];
                    info.State = (int)reader["State"];
                    info.Dt = (DateTime)reader["DT"];

                    list.Add(info);
                }
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
                return null;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
            }
            return list;
        }
        /// <summary>
        /// 获得该车在该时间端内的车辆信息
        /// </summary>
        /// <param name="carid">车辆id</param>
        /// <param name="dtstart">开始时间 </param>
        /// <param name="dtend">结束时间</param>
        /// <returns></returns>
        public List<LibrateInfo> getLibrateInfosOfthisCar(int carid,DateTime dtstart,DateTime dtend)
        {
            List<LibrateInfo> list = new List<LibrateInfo>();
            string sqltxt = "select * from SenseOrgan where CarID=" + carid + "and DT between '" + dtstart + "' and '" + dtend + "' order by DT";
            SqlDataReader reader = DBConnection.executeQuery(DBConnection.getSqlConnection(), sqltxt);
            LibrateInfo info;
            try
            {
                while (reader.Read())
                {
                    info = new LibrateInfo();
                    info.CarID = (int)reader["CarID"];
                    info.State = (int)reader["State"];
                    info.Dt = (DateTime)reader["DT"];

                    list.Add(info);
                }
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
                return null;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
            }
            return list;
        }

        /// <summary>
        /// 获得该车在该时间端内的车辆信息
        /// </summary>
        /// <param name="carid">车辆id</param>
        /// <param name="dtstart">开始时间 </param>
        /// <param name="dtend">结束时间</param>
        /// <returns></returns>
        public List<LibrateInfo> getLastLibrateInfosOfthisCar(int carid, DateTime dtstart, DateTime dtend)
        {
            List<LibrateInfo> list = new List<LibrateInfo>();
            string sqltxt = "select top 1 * from SenseOrgan where CarID=" + carid + "and DT between '" + dtstart + "' and '" + dtend + "' order by DT desc";
            SqlDataReader reader = DBConnection.executeQuery(DBConnection.getSqlConnection(), sqltxt);
            LibrateInfo info;
            try
            {
                while (reader.Read())
                {
                    info = new LibrateInfo();
                    info.CarID = (int)reader["CarID"];
                    info.State = (int)reader["State"];
                    info.Dt = (DateTime)reader["DT"];

                    list.Add(info);
                }
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
                return null;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
            }
            return list;
        }
    }
}
