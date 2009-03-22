using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace DM.DB
{
    public enum CarDistribute_Status
    {
        //定义车辆工作状态枚举
        FREE = 0,
        ASSIGNED = 1,
        ENDWORK = 2,
        WORK = 3
    }

    public class CarDistributeDAO
    {
        public const int NOMATCH = 0;//没有相对应的车辆
        public const int SUCCESS = 1;//成功
        public const int MATCHMORE = 2;//匹配太多车辆

        private CarDistributeDAO() { }
        private static CarDistributeDAO myInstance = null;

        public static CarDistributeDAO getInstance()
        {
            if (myInstance == null)
            {
                myInstance = new CarDistributeDAO();
            }
            return myInstance;
        }
        //结束该仓面上的车辆
         public int endCars(CarDistribute carDistribute)
        {
            String sqlTxt = "update cardistribute set DTEnd=getDate() where blockid=" + carDistribute.Blockid + " and designz=" + carDistribute.DesignZ + " and segmentid=" + carDistribute.Segmentid + " and DTEnd is null";
            try
            {
                int updateRowsNumber = DBConnection.executeUpdate(sqlTxt);           

                    //更新车辆状态
                sqlTxt = "update carinfo set blockid=0,maxspeed='0',segmentid=0,SenseOrganState=0,DesignZ='0' where carid in (select carid from cardistribute  where" +
                       " blockid=" + carDistribute.Blockid +
                       " and segmentid=" + carDistribute.Segmentid +
                       " and designz= " + carDistribute.DesignZ+" and  dtend is not null)";

                    DBConnection.executeUpdate(sqlTxt);

                    return updateRowsNumber;

            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                throw exp;
            }
        }

        //结束车辆
        //返回类型参照上面
         public int endCar(CarDistribute carDistribute)
         {
             String sqlTxt = "update cardistribute set DTEnd=getDate() where carid=" + carDistribute.Carid + " and blockid=" + carDistribute.Blockid + " and designz=" + carDistribute.DesignZ + " and segmentid=" + carDistribute.Segmentid + " and DTEnd is null";
             try
             {
                 int updateRowsNumber = DBConnection.executeUpdate(sqlTxt);
                 if (updateRowsNumber > 1)//更新了太多数据
                 {
                     return MATCHMORE;
                 }
                 else
                 {
                     //更新车辆状态
                     sqlTxt = "update carinfo set blockid=0,maxspeed='0',segmentid=0,SenseOrganState=0,DesignZ='0' where carid=" + carDistribute.Carid;

                     DBConnection.executeUpdate(sqlTxt);

                     return updateRowsNumber;
                 }
             }
             catch (Exception exp)
             {
                 DebugUtil.log(exp);
                 throw exp;
             }
         }

        /*
       
               //添加一些车辆
               public Boolean addCars(Int32 blockid,Double designZ,Int32 segmentid, List<Int32> carids) {		
                   CarDistribute cd = new CarDistribute();
                   cd.Blockid=(blockid);
                   cd.DesignZ=(designZ);		   
                   cd.Segmentid=(segmentid);				
                   //匹配车辆.
                   for(int i=0;i<carids.Count;i++){				
                       cd.Carid = carids[i];
                       try
                       {
                           startCar(cd);
                       }
                       catch (System.Exception e)
                       {
                           DebugUtil.log(e);
                           return false;
                       }
                   }
                   return true;		
               }	
         */
        //添加一部车辆
        //分配未使用的car给一个舱面
        //分配之前检查一下是否正在使用中.
        // lect count(*) from cardistribute where carid=? and DTEnd is not null.
        //insert cardistribute (blockid,designlayerid,segmentid,carid,DTStart) values(?,?,?,?,getDate());
         public Boolean startCar(CarDistribute carDistribute, Double maxSpeed, int librate,double designz)
         {

             //检查是否在使用中
             String sqlTxt = "select * from cardistribute where carid=" + carDistribute.Carid + " and ( dtstart is not null and DTEnd is null)";
             SqlConnection conn = null;
             SqlDataReader reader = null;
             try
             {
                 conn = DBConnection.getSqlConnection();
                 reader = DBConnection.executeQuery(conn, sqlTxt);
                 if (reader.Read())
                 {
                     return false;
                 }
                 DBConnection.closeDataReader(reader);
                 //分配车辆
                 sqlTxt = "insert cardistribute (carid,blockid,segmentid,designz,DTStart) values(" + carDistribute.Carid + "," + carDistribute.Blockid + "," + carDistribute.Segmentid + "," + carDistribute.DesignZ + "," + "getDate())";
                 int updateCount = DBConnection.executeUpdate(sqlTxt);

                 if (updateCount <= 0)
                 {
                     return false;
                 }


                 sqlTxt = "update carinfo set blockid=" + carDistribute.Blockid + ",maxspeed='" + maxSpeed + "',segmentid=" + carDistribute.Segmentid + ",SenseOrganState=" + librate + ",DesignZ='"+designz+"' where carid = " + carDistribute.Carid;

                 updateCount = DBConnection.executeUpdate(sqlTxt);

                 if (updateCount <= 0)
                 {
                     return false;
                 }

                 return true;
             }
             catch (Exception exp)
             {
                 DebugUtil.log(exp);
                 return false;

             }
             finally
             {
                 DBConnection.closeSqlConnection(conn);
             }
         }

        //预先分配一些车辆到某仓面
        public Boolean distributeCars(Int32 blockid, Double designZ, Int32 segmentid, List<Int32> carids)
        {
            CarDistribute cd = new CarDistribute();
            cd.Blockid = (blockid);
            cd.DesignZ = (designZ);
            cd.Segmentid = (segmentid);
            //匹配车辆.
            for (int i = 0; i < carids.Count; i++)
            {
                cd.Carid = carids[i];
                try
                {
                    distributeCar(cd);
                }
                catch (System.Exception e)
                {
                    DebugUtil.log(e);
                    return false;
                }
            }
            return true;
        }
        //预分配一部车辆
        public Boolean distributeCar(CarDistribute carDistribute)
        {
            //检查是否在使用中            
            SqlConnection conn = null;
            try
            {
                conn = DBConnection.getSqlConnection();
                //分配车辆
                String sqlTxt = "insert cardistribute (carid,blockid,segmentid,designz) values(" + carDistribute.Carid + "," + carDistribute.Blockid + "," + carDistribute.Segmentid + "," + carDistribute.DesignZ + ")";
                int updateCount = DBConnection.executeUpdate(sqlTxt);

                if (updateCount <= 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                return false;

            }
            finally
            {
                DBConnection.closeSqlConnection(conn);
            }
        }

        //删除某一仓面预定义的某辆车
        public Boolean removeCar(CarDistribute carDistribute)
        {
            //检查是否在使用中            
            SqlConnection conn = null;
            try
            {
                conn = DBConnection.getSqlConnection();
                //分配车辆
                string sqlTxt = "delete cardistribute where carid= " + carDistribute.Carid +
                    " and blockid=" + carDistribute.Blockid +
                    " and segmentid=" + carDistribute.Segmentid +
                    " and designz= " + carDistribute.DesignZ
                    /*+ "' and dtstart is null and dtend is null"*/;
                int updateCount = DBConnection.executeUpdate(sqlTxt);

                if (updateCount <= 0)
                {
                    return false;
                }

                //sqlTxt = "update carinfo set blockid=0,maxspeed='0',segmentid=0 where carid = " + carDistribute.Carid;

                //updateCount = DBConnection.executeUpdate(sqlTxt);

                if (updateCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                return false;

            }
            finally
            {
                DBConnection.closeSqlConnection(conn);
            }
        }



        //启动当前仓面上的车辆
        public Boolean startCars(CarDistribute carDistribute, Double maxspeed, int librate,double designz)
        {
            //检查是否在使用中            
            SqlConnection conn = null;
            try
            {
                conn = DBConnection.getSqlConnection();
                //分配车辆
                string sqlTxt = "update cardistribute set dtstart = getdate() where  blockid=" + carDistribute.Blockid +
                    " and segmentid=" + carDistribute.Segmentid +
                    " and designz= '" + carDistribute.DesignZ +
                    "' and  dtstart is null and dtend is null";


                int updateCount = DBConnection.executeUpdate(sqlTxt);

                if (updateCount <= 0)
                {
                    return false;
                }

                sqlTxt = "update carinfo set blockid=" + carDistribute.Blockid + ",maxspeed='" + maxspeed + "',segmentid=" + carDistribute.Segmentid + ",SenseOrganState=" + librate + ",DesignZ='"+designz+"' where carid in (select carid from cardistribute  where  blockid=" + carDistribute.Blockid +
                    " and segmentid=" + carDistribute.Segmentid +
                    " and designz= '" + carDistribute.DesignZ +
                    "' and  dtstart is not null and dtend is null)";

                updateCount = DBConnection.executeUpdate(sqlTxt);

                if (updateCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                return false;

            }
            finally
            {
                DBConnection.closeSqlConnection(conn);
            }
        }


        //取得当前舱面已经分配的car
        //select * from carinfo where DTEnd is null and DTStart is not null and blockid=? and designlayerid=? and segmentid=?;
        public List<CarDistribute> getCarInfosInThisSegment_Distributed(Int32 blockid, Double designZ, Int32 segmentid)
        {
            List<CarDistribute> carinfos = new List<CarDistribute>();
            SqlConnection conn = null;
            SqlDataReader reader = null;
            String sqlTxt = "select * from cardistribute where blockid=" + blockid +
                " and segmentid=" + segmentid +
                " and designZ=" + designZ;
            //List<CarInfo> all = CarInfoDAO.getInstance().getAllCarInfo();
            try
            {
                conn = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(conn, sqlTxt);
                while (reader.Read())
                {
                    CarDistribute dis = new CarDistribute();
                    dis.Carid = (int)reader["CarID"];
                    if (reader["DTStart"] == DBNull.Value)
                        dis.DTStart = DateTime.MinValue;
                    else
                        dis.DTStart = (DateTime)reader["DTStart"];

                    if (reader["DTEnd"] == DBNull.Value)
                        dis.DTEnd = DateTime.MinValue;
                    else
                        dis.DTEnd = (DateTime)reader["DTEnd"];

                    carinfos.Add(dis);
                }
                return carinfos;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                throw exp;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
            }
        }


        //取得当前舱面正在使用的car
        //select * from carinfo where DTEnd is null and DTStart is not null and blockid=? and designlayerid=? and segmentid=?;
        public List<CarInfo> getCarInfosInThisSegment_inuse(Int32 blockid, Double designZ, Int32 segmentid)
        {
            List<CarInfo> carinfos = new List<CarInfo>();
            SqlConnection conn = null;
            SqlDataReader reader = null;
            String sqlTxt = "select * from cardistribute where blockid=" + blockid + " and segmentid=" + segmentid + " and designZ=" + designZ + " and DTEnd is null and DTStart is not null";
            List<CarInfo> all = CarInfoDAO.getInstance().getAllCarInfo();
            try
            {
                conn = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(conn, sqlTxt);
                while (reader.Read())
                {
                    Int32 carID = (Convert.ToInt32(reader["carid"]));
                    CarInfo carinfo = CarInfoDAO.getInstance().getCarInfo(all, carID);
                    carinfos.Add(carinfo);
                }
                return carinfos;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                throw exp;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
            }
        }

        public CarDistribute_Status checkStatus(DateTime start, DateTime end)
        {
            if (!start.Equals(DateTime.MinValue) && end.Equals(DateTime.MinValue))
            {
                return CarDistribute_Status.WORK;
            }
            else if (!start.Equals(DateTime.MinValue) && !end.Equals(DateTime.MinValue))
            {
                return CarDistribute_Status.ENDWORK;
            }
            return CarDistribute_Status.ASSIGNED;
        }

        //取得某一个仓面所有分配过得车辆信息               
        public List<CarDistribute> getCarDistributeInThisSegment_all(Int32 blockid, Double designZ, Int32 segmentid)
        {
            List<CarDistribute> cardistributes = new List<CarDistribute>();
            SqlConnection conn = null;
            SqlDataReader reader = null;
            String sqlTxt = "select * from cardistribute where blockid=" + blockid + " and segmentid=" + segmentid + " and designZ=" + designZ;
            try
            {
                conn = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(conn, sqlTxt);
                while (reader.Read())
                {
                    Int32 carid = Convert.ToInt32(reader["carid"]);
                    //Int32 blockid = Convert.ToUInt32(reader["blockid"]);
                    //Int32 segmentid = Convert.ToUInt32(reader["segmentid"]);
                    DateTime dTEnd = DateTime.MinValue;
                    DateTime dTStart = DateTime.MinValue;
                    if (!reader["dtend"].Equals(DBNull.Value))
                    {
                        dTEnd = Convert.ToDateTime(reader["dtend"]);
                    }
                    if (!reader["dtstart"].Equals(DBNull.Value))
                    {
                        dTStart = Convert.ToDateTime(reader["dtstart"]);
                    }
                    CarDistribute_Status status = checkStatus(dTStart, dTEnd);
                    //Double designZ = Convert.ToDouble(reader["designz"]);
                    CarDistribute cardistribute = new CarDistribute();
                    cardistribute.Carid = (carid);
                    cardistribute.Blockid = (blockid);
                    cardistribute.DTEnd = dTEnd;
                    cardistribute.DTStart = dTStart;
                    cardistribute.DesignZ = (designZ);
                    cardistribute.Segmentid = (segmentid);
                    cardistribute.Status = status;
                    cardistributes.Add(cardistribute);
                }

                //                 foreach (CarDistribute cd in cardistributes)
                //                 {
                //                     foreach (CarDistribute cd2 in cardistributes)
                //                     {
                //                         if(cd.Carid==cd2.Carid){
                //                             if(cd2.Status>cd.Status){
                //                                 cardistributes.Remove(cd);
                //                             }
                //                             break;
                //                         }
                //                     }
                //                 }
                return cardistributes;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                throw exp;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
            }
        }

        //取得某一个仓面所有分配过得车辆信息                
        public List<CarDistribute> getCarDistributeInThisSegment_inuse(Int32 blockid, Double designZ, Int32 segmentid)
        {
            List<CarDistribute> cardistributes = new List<CarDistribute>();
            SqlConnection conn = null;
            SqlDataReader reader = null;
            String sqlTxt = "select * from cardistribute where blockid=" + blockid + " and segmentid=" + segmentid + " and designZ=" + designZ + " and DTEnd is null and DTStart is not null";
            try
            {
                conn = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(conn, sqlTxt);
                while (reader.Read())
                {
                    Int32 carid = Convert.ToInt32(reader["carid"]);
                    //Int32 blockid = Convert.ToUInt32(reader["blockid"]);
                    //Int32 segmentid = Convert.ToUInt32(reader["segmentid"]);
                    DateTime dTEnd = DateTime.MinValue;
                    DateTime dTStart = DateTime.MinValue;
                    if (!reader["dtend"].Equals(DBNull.Value))
                    {
                        dTEnd = Convert.ToDateTime(reader["dtend"]);
                    }
                    if (!reader["dtstart"].Equals(DBNull.Value))
                    {
                        dTStart = Convert.ToDateTime(reader["dtstart"]);
                    }
                    CarDistribute_Status status = checkStatus(dTEnd, dTStart);
                    //Double designZ = Convert.ToDouble(reader["designz"]);
                    CarDistribute cardistribute = new CarDistribute();
                    cardistribute.Carid = (carid);
                    cardistribute.Blockid = (blockid);
                    cardistribute.DTEnd = dTEnd;
                    cardistribute.DTStart = dTStart;
                    cardistribute.DesignZ = (designZ);
                    cardistribute.Segmentid = (segmentid);
                    cardistribute.Status = status;
                    cardistributes.Add(cardistribute);
                }
                return cardistributes;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                throw exp;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
            }
        }


        //得到全部正在使用中的car
        public List<CarInfo> getInusedCars()
        {
            List<CarInfo> carinfos = new List<CarInfo>();
            SqlConnection connection = null;
            SqlDataReader reader = null;
            List<CarInfo> all = CarInfoDAO.getInstance().getAllCarInfo();
            String sqlTxt = "select * from cardistribute where DTEnd is null and DTStart is not null";
            try
            {
                connection = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(connection, sqlTxt);
                while (reader.Read())
                {
                    Int32 carID = (Convert.ToInt32(reader["carid"]));
                    CarInfo carinfo = CarInfoDAO.getInstance().getCarInfo(all, carID);
                    carinfos.Add(carinfo);
                }
                return carinfos;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                throw exp;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(connection);
            }
        }

        //得到全部正在使用中的car
        public List<CarDistribute> getInusedCarDistributes()
        {
            List<CarDistribute> cardistributes = new List<CarDistribute>();
            SqlConnection conn = null;
            SqlDataReader reader = null;
            String sqlTxt = "select * from cardistribute where DTEnd is null and DTStart is not null";
            try
            {
                conn = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(conn, sqlTxt);
                while (reader.Read())
                {
                    Int32 carid = Convert.ToInt32(reader["carid"]);
                    Int32 blockid = Convert.ToInt32(reader["blockid"]);
                    Int32 segmentid = Convert.ToInt32(reader["segmentid"]);
                    Double designZ = Convert.ToDouble(reader["designz"]);
                    DateTime dTEnd = DateTime.MinValue;
                    DateTime dTStart = DateTime.MinValue;
                    if (!reader["dtend"].Equals(DBNull.Value))
                    {
                        dTEnd = Convert.ToDateTime(reader["dtend"]);
                    }
                    if (!reader["dtstart"].Equals(DBNull.Value))
                    {
                        dTStart = Convert.ToDateTime(reader["dtstart"]);
                    }
                    CarDistribute_Status status = checkStatus(dTEnd, dTStart);
                    //Double designZ = Convert.ToDouble(reader["designz"]);
                    CarDistribute cardistribute = new CarDistribute();
                    cardistribute.Carid = (carid);
                    cardistribute.Blockid = (blockid);
                    cardistribute.DTEnd = dTEnd;
                    cardistribute.DTStart = dTStart;
                    cardistribute.DesignZ = (designZ);
                    cardistribute.Segmentid = (segmentid);
                    cardistribute.Status = status;
                    cardistributes.Add(cardistribute);
                }
                return cardistributes;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                return null;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
            }
            
        }

        //         //取得所有结束工作的车辆,不包含从未投入过工作的.(因为从未投入工作的在carDistribute表中没有记录)
        //         public List<Int32> getUnusedCaridsWithoutNeverUsed()
        //         {
        //             List<Int32> carids = new List<Int32>();
        //             SqlConnection connection = null;
        //             SqlDataReader reader = null;
        //             //DTend is not null 和 DTstart is null 的均为 未使用.
        //             String sqlTxt = "select carid from cardistribute where (DTEnd is not null) or (DTStart is null) ";
        //             try
        //             {
        //                 connection = DBConnection.getSqlConnection();
        //                 reader = DBConnection.executeQuery(connection, sqlTxt);
        //                 while (reader.Read())
        //                 {
        //                     Int32 carid = Convert.ToInt32(reader["carid"]);
        //                     carids.SetVertex(carid);
        //                 }
        //                 return carids;
        //             }
        //             catch (Exception exp)
        //             {                
        //                 return null;
        //             }
        //             finally
        //             {
        //                 DBConnection.closeDataReader(reader);
        //                 DBConnection.closeSqlConnection(connection);
        //             }
        //         }

        //取得全部未使用的car()..
        //先得取出所有car信息,然后查询出正在使用中的car信息.在所有car信息中去掉正在使用中的car信息.	

        public List<CarInfo> getUnusedCars()
        {
            List<CarInfo> unusedCars = new List<CarInfo>();

            List<CarInfo> carinfos = null;
            List<CarInfo> inusedCars = null;
            try
            {
                carinfos = CarInfoDAO.getInstance().getAllCarInfo();
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
                throw e;
            }

            try
            {
                inusedCars = getInusedCars();
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
                throw e;
            }

            foreach (CarInfo carinfo in carinfos)
            {
                Boolean inused = false;
                foreach (CarInfo inusedcar in inusedCars)
                {
                    if (inusedcar.CarID == (carinfo.CarID))
                    {
                        inused = true;
                        break;
                    }
                }
                if (!inused)
                {
                    unusedCars.Add(carinfo);
                }
            }
            return unusedCars;
        }
        //取得某一个仓面所有分配过得车辆信息,不包含已经结束的
        public List<CarDistribute> getCarDistributeInThisSegment_all_except_end(Int32 blockid, Double designZ, Int32 segmentid)
        {
            List<CarDistribute> cardistributes = new List<CarDistribute>();
            SqlConnection conn = null;
            SqlDataReader reader = null;
            String sqlTxt = "select * from cardistribute where blockid=" + blockid + " and segmentid=" + segmentid + " and designZ=" + designZ + " and dtend is null and dtstart is null";
            try
            {
                conn = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(conn, sqlTxt);
                while (reader.Read())
                {
                    Int32 carid = Convert.ToInt32(reader["carid"]);
                    //Int32 blockid = Convert.ToUInt32(reader["blockid"]);
                    //Int32 segmentid = Convert.ToUInt32(reader["segmentid"]);
                    DateTime dTEnd = DateTime.MinValue;
                    DateTime dTStart = DateTime.MinValue;
                    if (!reader["dtend"].Equals(DBNull.Value))
                    {
                        dTEnd = Convert.ToDateTime(reader["dtend"]);
                    }
                    if (!reader["dtstart"].Equals(DBNull.Value))
                    {
                        dTStart = Convert.ToDateTime(reader["dtstart"]);
                    }
                    CarDistribute_Status status = checkStatus(dTStart, dTEnd);
                    //Double designZ = Convert.ToDouble(reader["designz"]);
                    CarDistribute cardistribute = new CarDistribute();
                    cardistribute.Carid = (carid);
                    cardistribute.Blockid = (blockid);
                    cardistribute.DTEnd = dTEnd;
                    cardistribute.DTStart = dTStart;
                    cardistribute.DesignZ = (designZ);
                    cardistribute.Segmentid = (segmentid);
                    cardistribute.Status = status;
                    cardistributes.Add(cardistribute);
                }

                //foreach (CarDistribute cd in cardistributes)
                //{
                //    foreach (CarDistribute cd2 in cardistributes)
                //    {
                //        if(cd.Carid==cd2.Carid){
                //            if(cd2.Status>cd.Status){
                //                cardistributes.Remove(cd);
                //            }
                //            break;
                //        }
                //    }
                //}
                return cardistributes;
            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                throw exp;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
            }
        }

        //得到与当前仓面完全没有关系的车辆
        public List<CarInfo> getCars_not_in_this_segment(Int32 blockid, Double designZ, Int32 segmentid)
        {
            return getOthers(getCarDistributeInThisSegment_all_except_end(blockid,designZ,segmentid));
        }


        //从全部的车辆中去除这些车辆
        public List<CarInfo> getOthers(List<CarDistribute> cds)
        {
            List<CarInfo> others = new List<CarInfo>();
            List<CarInfo> carinfos = null;

            try
            {
                carinfos = CarInfoDAO.getInstance().getAllCarInfo();
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
                throw e;
            }


            foreach (CarInfo carinfo in carinfos)
            {
                Boolean inused = false;
                foreach (CarDistribute cd in cds)
                {
                    if (cd.Carid == (carinfo.CarID))
                    {
                        inused = true;
                        break;
                    }
                }
                if (!inused)
                {
                    others.Add(carinfo);
                }
            }
            return others;
        }

        //从全部的车辆中去除这些车辆
        public List<CarInfo> getOthers(List<Int32> ids)
        {
            List<CarInfo> others = new List<CarInfo>();
            List<CarInfo> carinfos = null;

            try
            {
                carinfos = CarInfoDAO.getInstance().getAllCarInfo();
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
                throw e;
            }


            foreach (CarInfo carinfo in carinfos)
            {
                Boolean inused = false;
                foreach (Int32 id in ids)
                {
                    if (id == (carinfo.CarID))
                    {
                        inused = true;
                        break;
                    }
                }
                if (!inused)
                {
                    others.Add(carinfo);
                }
            }
            return others;
        }
    }
}
