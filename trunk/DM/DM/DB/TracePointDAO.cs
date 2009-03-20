using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace DM.DB
{
    class TracePointDAO
    {

        private static TracePointDAO myInstance = null;

        public static TracePointDAO getInstance()
        {
            if (myInstance == null)
            {
                myInstance = new TracePointDAO();
            }
            return myInstance;
        }

        private /*const*/ string statusLimit = " and status not in ("+Models.Config.I.BASE_STATUS+") order by dttrace";
        //private const String statusLimit = " and status not in (0)";
        //private const String statusLimit = " and status not in (0,1)";
        /// <summary>
        /// 插入一条gps点
        /// </summary>
        /// <param name="p">按照数据库字段顺序</param>
        /// <returns></returns>
        public bool InsertOneTP(params string[] p)
        {
            try
            {
                string sqltxt = "insert ZTracePoint" + DBCommon.getDate().Year.ToString() + DBCommon.getDate().Month.ToString("00") + " values(" + p[0] + ",'" + p[1] + "','" + p[2] + "','" + p[3] + "'," + p[4] + ",'" + p[5] + "'," + p[6] + ")";
                int i = DBConnection.executeUpdate(sqltxt);
                if (i == 1)
                    return true;
                else
                    return false;
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
                return false;
            }
        }

        /// <summary>
        /// 插入一条击震力信息
        /// </summary>
        /// <param name="p">按照数据库字段顺序</param>
        /// <returns></returns>
        public bool InsertOneOsense(params string[] p)
        {
            try
            {
                string sqltxt = "insert SenseOrgan values(" + p[0] + "," + p[1] + ",'" + p[2]  + "')";
                int i = DBConnection.executeUpdate(sqltxt);
                if (i == 1)
                    return true;
                else
                    return false;
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
                return false;
            }
            finally
            {
                //DBConnection.closeDataReader(reader);
                //                 DBConnection.closeSqlConnection(conn);
            }
        }

        //DBAgent.AddNewRecord(TableName, "CarID, X,Y,Z,V,DTTrace,Status", data.CarID.ToString(),
        //                data.Longitude.ToString(),
        //                data.Latitude.ToString(),
        //                data.Altitude.ToString(),
        //                ((int)((data.Speed) * 100)).ToString(),
        //                "'" + DTTrace + "'",
        //                data.WorkFlag.ToString());

        //取得某辆车最新的一点

//         public TracePoint getTracePoint(Int32 carid){
//             SqlConnection conn = null;
//             SqlDataReader reader = null;
//             try
//             {
//                 TracePoint tracePoint = new TracePoint();
//                 conn = DBConnection.getSqlConnection();
//                 string tablename = getThisTableName();
//                 reader = DBConnection.executeQuery(conn, "SELECT     TOP (1) CarID, X, Y, Z, V, DTTrace FROM "+tablename+" WHERE (CarID = "+carid+") "+statusLimit+" ORDER BY DTTrace DESC");
//                 if (reader.Read())
//                 {
//                     tracePoint.Carid = Convert.ToInt32(reader["carid"]);
//                     tracePoint.X = Convert.ToDouble((reader["x"]));
//                     tracePoint.Y = Convert.ToDouble((reader["y"]));
//                     tracePoint.Z = Convert.ToDouble((reader["z"]));
//                     tracePoint.V = Convert.ToInt32((reader["v"]));
//                     tracePoint.Dttrace = Convert.ToDateTime(reader["dttrace"]);
//                 }
//                 else
//                 {
//                     return null;
//                 }               
//                 return tracePoint;
//             }
//             catch (System.Exception e)
//             {
//                 DebugUtil.log(e);
//                 return null;
//             }finally{
//                 DBConnection.closeDataReader(reader);
//                 DBConnection.closeSqlConnection(conn);
//             }
//         }

        //得到当前正在使用的表名称
        public string getThisTableName(){
            return getTableNameByDateTime(DateTime.Now);
        }

        public string getTableNameByDateTime(DateTime datetime){
            return "ztracepoint" + string.Format("{0:yyyyMM}", datetime);
        }

        //半路来看正在施工的仓面



        //取得某一个仓面的List<TracePoint>.可能有多辆车,可能跨月。

        // 按不同车 分列表

        public List<List<TracePoint>> getHistoryTracePoints(Int32 blockid,Double designz,Int32 segmentid){
            //得到所有在此舱面工作过的车辆

            List<List<TracePoint>> tracepointLists = new List<List<TracePoint>>();
            //当前仓面上的车辆
            List<CarDistribute> carDistributes = null;
            
            try
            {
                carDistributes = CarDistributeDAO.getInstance().getCarDistributeInThisSegment_all(blockid, designz, segmentid);
            }
            catch (System.Exception e)
            {
                throw e;
                //return null;
            }
            
            foreach (CarDistribute cd in carDistributes){
                tracepointLists.Add(getTracePointList(cd.Carid,cd.DTStart,cd.DTEnd));
            }

            return tracepointLists;
        }

        public List<TracePoint> getTracePointList(Int32 carid, DateTime dtstart,DateTime dtend){
            List<TracePoint> tracePoints = new List<TracePoint>();            
            if(dtstart != DateTime.MinValue){

                if(dtend==DateTime.MinValue){
                    dtend = DateTime.Now;
                }                    
                    
                DateTime[] datetimes = DateUtil.getDateTimes(Compare_Type.MONTH,dtstart,dtend);
                if(datetimes.Length==1){
                    string tablename = string.Format("{0:yyyyMM}", datetimes[0]);
                    String sqltxt = "select * from " + getTableNameByDateTime(datetimes[0]) + " where carid=" + carid + " and dttrace between '" + dtstart.ToString() + "' and '" + dtend.ToString() + "'" + statusLimit;
                    getTracePointList(tracePoints,sqltxt);
                }else {
                    for(int i=0;i<datetimes.Length;i++){
                        if(i==0){
                            //dttrace >= datetimes[0]
                            string tablename = string.Format("{0:yyyyMM}", datetimes[0]);
                            String sqltxt = "select * from " + getTableNameByDateTime(datetimes[0]) + " where carid=" + carid + " and dttrace >= '" + dtstart.ToString() + "'" + statusLimit;
                            getTracePointList(tracePoints,sqltxt);
                        }else if(i==datetimes.Length-1){
                            //dttrace <= dtend
                            string tablename = string.Format("{0:yyyyMM}", datetimes[datetimes.Length-1]);
                            String sqltxt = "select * from " + getTableNameByDateTime(datetimes[datetimes.Length - 1]) + " where carid=" + carid + " and dttrace <= '" + dtend.ToString() + "'" + statusLimit;
                            getTracePointList(tracePoints,sqltxt);
                        }else{
                            //select * from table
                            string tablename = string.Format("{0:yyyyMM}", datetimes[i]);
                            String sqltxt = "select * from " + getTableNameByDateTime(datetimes[i]) + " where carid=" + carid + statusLimit;
                            getTracePointList(tracePoints,sqltxt);
                        }
                    }
                }                
            }            

            return tracePoints;

        }


        public List<TracePoint> getTracePointList(List<TracePoint> tracepoints ,String sqlTxt)
        {            
            SqlConnection connection = null;
            SqlDataReader reader = null;           
            try
            {
                connection = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(connection, sqlTxt);
                TracePoint tracePoint = null;
                while (reader.Read())
                {
                    tracePoint = new TracePoint();
                    tracePoint.Carid = Convert.ToInt32(reader["carid"]);
                    tracePoint.X = Convert.ToDouble((reader["x"]));
                    tracePoint.Y = Convert.ToDouble((reader["y"]));
                    tracePoint.Z = Convert.ToDouble((reader["z"]));
                    tracePoint.V = Convert.ToInt32((reader["v"]));
                    tracePoint.Dttrace = Convert.ToDateTime(reader["dttrace"]);
                    //Console.WriteLine(tracePoint.X);
                    tracepoints.Add(tracePoint);                
                }
                return tracepoints;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                return null;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(connection);
            }
        }
        
    }
}
