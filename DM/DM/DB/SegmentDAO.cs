using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using DM.Models;

namespace DM.DB
{
    //仓面工作状态

    public enum SegmentWorkState
    {
        WAIT = 0,//等待状态

        WORK = 1,//工作状态

        END = 2//结束状态

    }
    //结束仓面结果
    public enum EndSegmengResult
    {
        THIS_LAYER_END = 1,//当前层结束

        ONLY_SEGMENT_END,//只有仓面被结束

        END_ERROR//结束失败
    }
    //操作仓面,包含车辆车辆操作
    public enum SegmentVehicleResult
    {
        SUCCESS = 1,//成功
        SEGMENT_FAIL,//仓面操作失败
        CARS_FAIL //车辆操作失败
    }
    public enum UpdateSegmentResult
    {
        NO_SEGMENT = 1,//没有仓面信息
        SUCCESS,//成功
        FAIL_BUT_SEGMENT_DELETED,//失败,但是删除了仓面信息

        FAIL//失败
    }

    public class SegmentDAO
    {
        private SegmentDAO() { }

        private static SegmentDAO myInstance = null;

        public static SegmentDAO getInstance()
        {
            if (myInstance == null)
            {
                myInstance = new SegmentDAO();
            }
            return myInstance;
        }


        //!!!!待修改
        /// <summary>
        /// 更新数据库仓面面积和碾压遍数百分比字段
        /// </summary>
        public bool UpdateSegmentAreaAndRollingPercentages(Int32 blockid, Double designz, Int32 segmentid, double area, string perent)
        {
            string sqlTxt = "update segment set SegmentArea = '" + area + "',RollingPercentages='" + perent + "'  where blockid = " + blockid + " and designz=" + designz + " and segmentid=" + segmentid;
            try
            {
                return (DBConnection.executeUpdate(sqlTxt) == 1);
            }
            catch (System.Data.SqlClient.SqlException E)
            {
                DebugUtil.log(E);
                return false;
            }
        }
        /// <summary>
        /// 读取指定舱面的备注信息
        /// </summary>
        public string ReadSegmentRemark(int blockid, Double designz, Int32 segmentid)
        {
           string sqlTxt = "select remark from Segment"+
               "  where blockid = " + blockid +
               " and designz=" + designz +
               " and segmentid=" + segmentid;
            try
            {
               SqlDataReader dr=DBConnection.executeQuery(DBConnection.getSqlConnection(),sqlTxt);
                while(dr.Read())
                {
                    if (dr["Remark"]==DBNull.Value)
                    {
                        return string.Empty;
                    }
                    return dr["Remark"].ToString();
                }
            }
            catch (System.Data.SqlClient.SqlException E)
            {
                DebugUtil.log(E);
                return string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// 添加不碾压区域的坐标和备注字
        /// </summary>
        public bool UpdateSegmentNotRolling(Int32 blockid, Double designz, Int32 segmentid, string coord, string mark, string areas)
        {
            string sqlTxt, co, mk;
            mk = co = null;
//             sqlTxt = "select NotRolling,CommentNR from segment" + " where blockid = " + blockid + " and designz=" + designz + " and segmentid=" + segmentid;
//             SqlDataReader dr=DBConnection.executeQuery(DBConnection.getSqlConnection(),sqlTxt);
//             while(dr.Read())
//             {
//                 if (dr["NotRolling"].ToString().Equals(string.Empty) && dr["CommentNR"].ToString().Equals(string.Empty))
//                 {
//                     co = coord;
//                     mk = mark;
//                 }
//                 else
//                 {
//                     co = dr["NotRolling"].ToString() + "|" + coord;
//                     mk = dr["CommentNR"].ToString() + "|" + mark;
//                 }
//             }
            co = coord;
            mk = mark;
            sqlTxt = "update segment set NotRolling = '" + co+ 
                "',CommentNR='" + mk + 
                "',AreaNR='" + areas +
                "'  where blockid = " + blockid + 
                " and designz=" + designz + 
                " and segmentid=" + segmentid;
            try
            {
                return (DBConnection.executeUpdate(sqlTxt) == 1);
            }
            catch (System.Data.SqlClient.SqlException E)
            {
                DebugUtil.log(E);
                return false;
            }
        }


        /// <summary>
        /// 读取不碾压区域的坐标和备注字(填充输入deck NotRolling和备注属性)
        /// </summary>
        public bool ReadSegmentNotRolling(Int32 blockid, Double designz, Int32 segmentid, out string vtx, out string cmt)
        {
            string sqlTxt;
            vtx = "";
            cmt = "";
            try
            {
                sqlTxt = "select NotRolling,CommentNR from segment" + " where blockid = " + blockid + " and designz=" + designz + " and segmentid=" + segmentid;
                SqlDataReader dr = DBConnection.executeQuery(DBConnection.getSqlConnection(), sqlTxt);
                while (dr.Read())
                {
                    vtx = dr["NotRolling"].ToString();
                    cmt = dr["CommentNR"].ToString();
                }

            }
            catch (System.Data.SqlClient.SqlException E)
            {
                DebugUtil.log(E);
                return false;
            }

            return true;
        }

        //更新指定仓面的pop值
        public bool setSegmentPOP(Int32 blockid, Double designz, Int32 segmentid, double pop)
        {
            string sqlTxt = "update segment set pop  = '" + pop + "'  where blockid = " + blockid + " and designz=" + designz + " and segmentid=" + segmentid;
            try
            {
                return (DBConnection.executeUpdate(sqlTxt) == 1);
            }
            catch (System.Data.SqlClient.SqlException E)
            {
                DebugUtil.log(E);
                return false;
            }
        }

        //可能有几个字段不必更新的

        public UpdateSegmentResult updateSegments(List<Segment> segments, Int32 blockid, Double designz)
        {
            //先得删除当前层的所有仓面
            if (segments == null /*|| segments.Count < 1*/)
            {
                return UpdateSegmentResult.NO_SEGMENT;
            }
            String sqlTxt = "select * from segment where blockid = " + blockid + " and designz=" + designz;
            SqlDataReader dr = DBConnection.executeQuery(DBConnection.getSqlConnection(),sqlTxt);
            sqlTxt = "delete from segment where blockid = " + blockid + " and designz=" + designz;
            int updateCount = DBConnection.executeUpdate(sqlTxt);
            //if(updateCount>0){
            foreach (Segment segment in segments)
            {
                Int32 segmentID = segment.SegmentID;
                SegmentWorkState workState = segment.WorkState;
                Int32 blockID = segment.BlockID;
                Double designZ = segment.DesignZ;
                string vertext = segment.Vertext;
                DateTime startDate = segment.StartDate;
                DateTime endDate = segment.EndDate;
                Double maxSpeed = segment.MaxSpeed;
                Int32 designRollCount = segment.DesignRollCount;
                Double errorParam = segment.ErrorParam;
                Double spreadZ = segment.SpreadZ;
                Double designDepth = segment.DesignDepth;
                //add in 2008-12-1
                Double pop = segment.POP;
                //string remark = segment.Remark;
                string notrolling = segment.NotRollingstring;
                string notrollingremark = segment.CommentNRstring;
                string segmentName = segment.SegmentName;
                Double startZ = segment.StartZ;
                int librateState = segment.LibrateState;
                string startDateStr = "'" + startDate.ToString() + "'";
                string endDateStr = "'" + endDate.ToString() + "'";
                if (startDate.Equals(DateTime.MinValue))
                {
                    startDateStr = "NULL";
                }
                if (endDate.Equals(DateTime.MinValue))
                {
                    endDateStr = "NULL";
                }
                sqlTxt = string.Format("insert into segment  (SegmentID, WorkState, BlockID, DesignZ, Vertex, DTStart, DTEnd, MaxSpeed, DesignRollCount, ErrorParam, SpreadZ, DesignDepth, SegmentName,StartZ,pop,SenseOrganState,NotRolling,CommentNR) values(" +
                    "{0},{1},{2},'{3}','{4}',{5},{6},'{7}','{8}',{9},'{10}','{11}','{12}','{13}','{14}',{15},'{16}','{17}'"
                    + ")", segmentID, (int)workState, blockID, designZ, vertext, startDateStr, endDateStr, maxSpeed, designRollCount, errorParam, spreadZ, designDepth, segmentName, startZ, pop,librateState,notrolling,notrollingremark);
                if (DBConnection.executeUpdate(sqlTxt) == 1)
                {
                    continue;
                }
                else
                {
                    while (dr.Read())
                    {
                        sqlTxt = string.Format("insert into segment  (SegmentID, WorkState, BlockID, DesignZ, Vertex, DTStart, DTEnd, MaxSpeed, DesignRollCount, ErrorParam, SpreadZ, DesignDepth, SegmentName,StartZ,pop,SenseOrganState,NotRolling,CommentNR) values(" +
                    "{0},{1},{2},'{3}','{4}',{5},{6},'{7}','{8}',{9},'{10}','{11}','{12}','{13}','{14}',{15},'{16}','{17}'"
                    + ")", (int)dr["SegmentID"], (int)dr["WorkState"], (int)dr["BlockID"], (float)dr["DesignZ"], (string)dr["Vertex"], (DateTime)dr["DTStart"], (DateTime)dr["DTEnd"], dr["MaxSpeed"], (float)dr["DesignRollCount"], (float)dr["ErrorParam"], (float)dr["SpreadZ"], (float)dr["DesignDepth"], (string)dr["SegmentName"], (float)dr["StartZ"], (float)dr["POP"], (int)dr["SenseOrganState"], (string)dr["NotRolling"], (string)dr["CommentNR"]);
                        DBConnection.executeUpdate(sqlTxt);
                    }
                    return UpdateSegmentResult.FAIL;
                }
            }
            return UpdateSegmentResult.SUCCESS;
            //}else{
            //    return UpdateSegment_Result.FAIL;
            //}
        }

        /// <summary>
        /// 得到某分区的某层的全部舱面
        public List<Segment> getAllSegments()
        {
            List<Segment> segments = new List<Segment>();
            SqlConnection connection = null;
            SqlDataReader reader = null;
            string sqlTxt = "select blockid,designz,segmentid from segment";
            try
            {
                connection = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(connection, sqlTxt);
                while (reader.Read())
                {
                    if (reader["SegmentID"].Equals(DBNull.Value) || reader["BlockID"].Equals(DBNull.Value) || reader["DesignZ"].Equals(DBNull.Value))
                        continue;
                    Segment segment = new Segment();
                    segment.BlockID = (int)reader["BlockID"];
                    segment.SegmentID = Convert.ToInt32(reader["SegmentID"]);
                    segment.DesignZ = Convert.ToDouble(reader["DesignZ"]);
                    segments.Add(segment);
                }
                return segments;
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

        /// </summary>
        /// <param name="blockID"></param>
        /// <param name="designZ"></param>
        /// <returns>
        /// 
        /// </returns>
        public List<Segment> getSegments(Int32 blockID, Double designZ)
        {
            List<Segment> segments = new List<Segment>();
            SqlConnection connection = null;
            SqlDataReader reader = null;
            string sqlTxt = "select * from segment where (blockid=" + blockID + ") and (designZ=" + designZ + ")";
            try
            {
                connection = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(connection, sqlTxt);
                while (reader.Read())
                {
                    SegmentWorkState workState = (SegmentWorkState)Convert.ToInt32(reader["workState"]);
                    Int32 segmentid = Convert.ToInt32(reader["segmentid"]);
                    string vertex = reader["vertex"].ToString();
                    DateTime enddate = DateTime.MinValue;
                    DateTime startdate = DateTime.MinValue;
                    if (!reader["dtend"].Equals(DBNull.Value))
                    {
                        enddate = Convert.ToDateTime(reader["dtend"]);
                    }
                    if (!reader["dtstart"].Equals(DBNull.Value))
                    {
                        startdate = Convert.ToDateTime(reader["dtstart"]);
                    }
                    string remark = reader["remark"].ToString();
                    string segmentname = reader["segmentname"].ToString();
                    Double startZ = Convert.ToDouble(reader["startz"]);
                    Double maxSpeed = Convert.ToDouble(reader["maxspeed"]);
                    Int32 designRollCount = Convert.ToInt32(reader["designRollCount"]);
                    Double errorParam = Convert.ToDouble(reader["errorParam"]);
                    Segment segment = new Segment();
                    segment.MaxSpeed = maxSpeed;
                    segment.DesignRollCount = designRollCount;
                    segment.ErrorParam = errorParam;
                    segment.Remark = (remark);
                    segment.BlockID = (blockID);
                    segment.SegmentID = (segmentid);
                    segment.WorkState = (DB.SegmentWorkState)(workState);
                    segment.DesignZ = (designZ);
                    segment.Vertext = vertex;
                    segment.StartDate = (startdate);
                    segment.EndDate = (enddate);
                    segment.SegmentName = (segmentname);
                    segment.StartZ = startZ;
                    segment.DesignDepth = (double)reader["DESIGNDEPTH"];
                    segment.POP = (double)reader["POP"];
                    if (reader["SenseOrganState"]!=DBNull.Value)
                    segment.LibrateState = (int)reader["SenseOrganState"];
                    if (reader["NotRolling"] != DBNull.Value)
                        segment.NotRollingstring = (string)reader["NotRolling"];
                    if (reader["CommentNR"] != DBNull.Value)
                        segment.CommentNRstring = (string)reader["CommentNR"];
                    segments.Add(segment);
                }
                return segments;
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

        private Segment readSegment(SqlDataReader reader)
        {
            Segment segment = new Segment();
            SegmentWorkState workState = (SegmentWorkState)Convert.ToInt32(reader["workState"]);
            //Int32 segmentid = Convert.ToInt32(reader["segmentid"]);
            string vertex = reader["vertex"].ToString();
            DateTime enddate = DateTime.MinValue;
            DateTime startdate = DateTime.MinValue;
            if (!reader["dtend"].Equals(DBNull.Value))
            {
                enddate = Convert.ToDateTime(reader["dtend"]);
            }
            if (!reader["dtstart"].Equals(DBNull.Value))
            {
                startdate = Convert.ToDateTime(reader["dtstart"]);
            }
            string remark = reader["remark"].ToString();
            string segmentname = reader["segmentname"].ToString();
            Double startZ = Convert.ToDouble(reader["startz"]);
            Double maxSpeed = Convert.ToDouble(reader["maxspeed"]);
            Int32 designRollCount = Convert.ToInt32(reader["designRollCount"]);
            Double errorParam = Convert.ToDouble(reader["errorParam"]);
            segment = new Segment();
            segment.MaxSpeed = maxSpeed;
            segment.DesignRollCount = designRollCount;
            segment.ErrorParam = errorParam;
            segment.Remark = (remark);
            segment.BlockID = (int)(reader["BLOCKID"]);
            segment.SegmentID = (int)(reader["SEGMENTID"]);
            segment.WorkState = (DB.SegmentWorkState)(workState);
            segment.DesignZ = (double)(reader["DESIGNZ"]);
            segment.Vertext = vertex;
            segment.StartDate = (startdate);
            segment.EndDate = (enddate);
            segment.SegmentName = (segmentname);
            if (reader["NotRolling"]!=DBNull.Value)
                segment.NotRollingstring = (string)reader["NotRolling"];
            if (reader["CommentNR"] != DBNull.Value)
                segment.CommentNRstring = (string)reader["CommentNR"];
            segment.StartZ = startZ;
            segment.POP = (double)reader["POP"];
            segment.DesignDepth = (double)reader["DESIGNDEPTH"];
            if (reader["SenseOrganState"] != DBNull.Value)
            segment.LibrateState = (int)reader["SenseOrganState"];
            return segment;
        }
        public List<Segment> getSegment(Int32 blockID, Double designZ, Int32 segmentid)
        {
            List<Segment> segments = new List<Segment>();
            Segment segment = null;
            SqlConnection connection = null;
            SqlDataReader reader = null;
            string sqlTxt = "select * from segment where (blockid=" + blockID + ") and (designZ=" + designZ + 
                ") and (segmentid=" + segmentid + ")";
            try
            {
                connection = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(connection, sqlTxt);
                while (reader.Read())
                {
//                     SegmentWorkState workState = (SegmentWorkState)Convert.ToInt32(reader["workState"]);
//                     //Int32 segmentid = Convert.ToInt32(reader["segmentid"]);
//                     string vertex = reader["vertex"].ToString();
//                     DateTime enddate = DateTime.MinValue;
//                     DateTime startdate = DateTime.MinValue;
//                     if (!reader["dtend"].Equals(DBNull.Value))
//                     {
//                         enddate = Convert.ToDateTime(reader["dtend"]);
//                     }
//                     if (!reader["dtstart"].Equals(DBNull.Value))
//                     {
//                         startdate = Convert.ToDateTime(reader["dtstart"]);
//                     }
//                     string remark = reader["remark"].ToString();
//                     string segmentname = reader["segmentname"].ToString();
//                     Double startZ = Convert.ToDouble(reader["startz"]);
//                     Double maxSpeed = Convert.ToDouble(reader["maxspeed"]);
//                     Int32 designRollCount = Convert.ToInt32(reader["designRollCount"]);
//                     Double errorParam = Convert.ToDouble(reader["errorParam"]);
//                     segment = new Segment();
//                     segment.MaxSpeed = maxSpeed;
//                     segment.DesignRollCount = designRollCount;
//                     segment.ErrorParam = errorParam;
//                     segment.Remark = (remark);
//                     segment.BlockID = (blockID);
//                     segment.SegmentID = (segmentid);
//                     segment.WorkState = (DB.SegmentWorkState)(workState);
//                     segment.DesignZ = (designZ);
//                     segment.Vertext = vertex;
//                     segment.StartDate = (startdate);
//                     segment.EndDate = (enddate);
//                     segment.SegmentName = (segmentname);
//                     segment.StartZ = startZ;
//                     segment.POP = (double)reader["POP"];
//                     segment.DesignDepth = (double)reader["DESIGNDEPTH"];
                    segment = readSegment(reader);
                    segments.Add(segment);
                }
                return segments;
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

        // 启动某分区下的某工作层下的全部舱面

        public Boolean startAllSegments(Int32 blockid, Double designz)
        {
            string sqlTxt = "update segment set workstate=" + (int)SegmentWorkState.WORK + 
                ",dtstart=getdate() where blockid=" + blockid + " and designz=" + designz +
                " and wrokstate =" + (int)SegmentWorkState.WAIT;
            try
            {
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
        }

        // 启动某分区下的某工作层下的舱面,设置仓面状态和启动时间
        // 启动某分区下的某工作层下的舱面,设置仓面状态和启动时间
        public Boolean startThisSegment(Int32 blockid, Double designZ, Int32 segmentid, SegmentWorkState state)
        {
            string sqlTxt = "update segment set workstate=" + (int)SegmentWorkState.WORK +
                ",dtstart=getdate() where blockid=" + blockid + " and segmentid=" + segmentid +
                " and designz=" + designZ + " and workstate<>" + (int)SegmentWorkState.WORK;

            if (state == SegmentWorkState.END)
            {
                sqlTxt = "update segment set workstate=" + (int)SegmentWorkState.WORK +
                ",dtend=null where blockid=" + blockid + " and segmentid=" + segmentid +
                " and designz=" + designZ + " and workstate<>" + (int)SegmentWorkState.WORK;
            }

            try
            {
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

        }
        //重新启动当前仓面,更新workstate,设置dtstart为当前时间,dtend为空
        public Boolean reStartThisSegment(Int32 blockid, Double designZ, Int32 segmentid)
        {

            string sqlTxt = "update segment set workstate=" + (int)SegmentWorkState.WORK + 
                ",dtstart=getdate(),dtend=null where blockid=" + blockid + " and segmentid=" + segmentid + 
                " and designz=" + designZ + " and (workstate=" + (int)SegmentWorkState.END + ")";
            try
            {
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

        }


//         //结束当前层所有工作舱面,设置workstate,和dtend
//         protected Boolean endAllWorkSegments(Int32 blockid, Double designz)
//         {
//             //wrokstate <> WORKING		避免重复启动.
//             string sqlTxt = "update segment set workstate=" + (int)SegmentWorkState.END + 
//                 ",dtend=getdate() where blockid=" + blockid + " and designz=" + designz + 
//                 " and wrokstate = " + (int)SegmentWorkState.WORK;
//             try
//             {
//                 int updateCount = DBConnection.executeUpdate(sqlTxt);
//                 if (updateCount <= 0)
//                 {
//                     return false;
//                 }
//                 return true;
//             }
//             catch (Exception exp)
//             {
//                 DebugUtil.log(exp);
//                 return false;
//             }
// 
//         }

        // 结束某分区下的某工作层下的正在工作的舱面.
        //如果该舱面是最后一个未被结束的舱面,则结束该层.
        public EndSegmengResult endThisSegment(Int32 blockid, Double designZ, Int32 segmentid)
        {
            SqlConnection connection = null;
            SqlDataReader reader = null;
            string sqlTxt = "update segment set workstate=" + (int)SegmentWorkState.END + 
                ",dtend=getdate() where blockid=" + blockid + " and segmentid=" + segmentid +
                " and designZ=" + designZ /*+ " and workstate=" + (int)SegmentWorkState.WORK*/;
            try
            {
                int updateCount = DBConnection.executeUpdate(sqlTxt);
                if (updateCount <= 0)
                {
                    return EndSegmengResult.END_ERROR;
                }

                return EndSegmengResult.ONLY_SEGMENT_END;

                // 查看当前处于非结束状态的segment的数量

//                 connection = DBConnection.getSqlConnection();
//                 sqlTxt = "select * from segment where (workstate=" + (int)SegmentWorkState.WAIT +
//                     " or workstate=" + (int)SegmentWorkState.WORK + ") and blockid=" + blockid + " and designz=" + designZ;
//                 reader = DBConnection.executeQuery(connection, sqlTxt);
//                 if (reader.Read())
//                 {// 若无,结束本层
//                     sqlTxt = "update worklayer set workstate=" + (int)SegmentWorkState.END +
//                         ",DTEnd=getDate() where blockid=" + blockid /*+ " and workstate<>" + (int)SegmentWorkState.END*/+ 
//                         " and designz=" + designZ /*+ " and DTEnd is null"*/;
//                     try
//                     {
//                         updateCount = DBConnection.executeUpdate(sqlTxt);
//                         if (updateCount <= 0)
//                         {
//                             return EndSegmengResult.END_ERROR;
//                         }
//                     }
//                     catch (Exception exp)
//                     {
//                         DebugUtil.log(exp);
//                         return EndSegmengResult.END_ERROR;
//                     }
//                     return EndSegmengResult.THIS_LAYER_END;
//                 }
//                 else
//                 {
//                     return EndSegmengResult.ONLY_SEGMENT_END;
//                 }

            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);
                return EndSegmengResult.END_ERROR;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(connection);
            }
        }

        /// 包含车辆操作的启动仓面
        public SegmentVehicleResult startSegment(Int32 blockid, Double designZ, Int32 segmentid, Double maxSpeed, SegmentWorkState state)
        {
            //更新舱面状态.
            if (startThisSegment(blockid, designZ, segmentid, state))
            {
                //分配车辆.
                CarDistribute cd = new CarDistribute();
                cd.Blockid = (blockid);
                cd.DesignZ = (designZ);
                cd.Segmentid = (segmentid);
                Segment deck=getSegment( blockid,  designZ,  segmentid)[0];
                if (CarDistributeDAO.getInstance().startCars(cd, maxSpeed, deck.LibrateState, deck.DesignZ))
                {

                    return SegmentVehicleResult.SUCCESS;
                }


                else
                {
                    return SegmentVehicleResult.CARS_FAIL;
                }
            }
            return SegmentVehicleResult.SEGMENT_FAIL;
        }
        ////包含车辆操作的重新启动仓面

        //public SegmentVehicleResult reStartSegment(Int32 blockid, Double designZ, Int32 segmentid)
        //{
        //    //更新舱面状态.			
        //    if (reStartThisSegment(blockid, designZ, segmentid))
        //    {
        //        //分配车辆.
        //        CarDistribute cd = new CarDistribute();
        //        cd.Blockid = (blockid);
        //        cd.DesignZ = (designZ);
        //        cd.Segmentid = (segmentid);
        //        if ( CarDistributeDAO.getInstance().startCars(cd))
        //        {
        //            return SegmentVehicleResult.SUCCESS;
        //        }
        //        else
        //        {
        //            return SegmentVehicleResult.CARS_FAIL;
        //        }
        //    }
        //    return SegmentVehicleResult.SEGMENT_FAIL;
        //}
        //包含车辆操作的结束仓面

        public SegmentVehicleResult endSegment(Int32 blockid, Double designZ, Int32 segmentid)
        {
            //结束本仓面全部车辆.		
            CarDistribute cd = new CarDistribute();
            cd.Blockid = (blockid);
            cd.DesignZ = (designZ);
            cd.Segmentid = (segmentid);

            if (CarDistributeDAO.getInstance().endCars(cd) >= 0)
            {//成功结束了车辆

                if (endThisSegment(blockid, designZ, segmentid) != EndSegmengResult.END_ERROR)
                {
                    return SegmentVehicleResult.SUCCESS;
                }
                else
                {
                    return SegmentVehicleResult.SEGMENT_FAIL;
                }
            }
            return SegmentVehicleResult.CARS_FAIL;
        }

        public int updateDatamap(Int32 blockid, Double designz, Int32 segmentid, byte[] datamap)
        {
            string sqlTxt = "update segment set datamap  = @datamap  where blockid = " + blockid + " and designz=" + designz + " and segmentid=" + segmentid;

            SqlConnection conn = null;
            SqlCommand cmd = null;
            try
            {
                conn = DBConnection.getSqlConnection();
                cmd = new SqlCommand(sqlTxt, conn);
                SqlParameter sqlImage = cmd.Parameters.Add("@datamap ", SqlDbType.Image);
                sqlImage.Value = datamap;
                conn.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows;
            }
            catch (System.Data.SqlClient.SqlException E)
            {
                DebugUtil.log(E);
                return -1;
            }
            finally
            {
                DBConnection.closeSqlConnection(conn);
            }

        }
        public byte[] getDatamap(int blockid, double designz, int segmentid)
        {
            string sqltext = string.Format(
                "select DATAMAP from SEGMENT where (BLOCKID ={0}) and (DESIGNZ={1}) and (SEGMENTID={2})",
                blockid, designz, segmentid);
            SqlConnection connection = null;
            SqlDataReader reader = null;
            try
            {
                connection = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(connection, sqltext);
                if (reader != null)
                {
                    if (reader.Read())
                        return (byte[])reader["DATAMAP"];
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        public List<Segment> findBeneathSegments(int blockid, double designz)
        {
            List<Segment> lst = new List<Segment>();
            string sqltext = string.Format(
    "select * from SEGMENT where (BLOCKID ={0}) and (DESIGNZ<{1}) and (DATAMAP IS NOT NULL)",
    blockid, designz);
            SqlConnection connection = null;
            SqlDataReader reader = null;
            try
            {
                connection = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(connection, sqltext);
                while(reader.Read())
                {
                    Segment seg = new Segment();
                    lst.Add(readSegment(reader));
                }
            }
            catch
            {
                return lst;
            }

            return lst;
        }
    }
}
