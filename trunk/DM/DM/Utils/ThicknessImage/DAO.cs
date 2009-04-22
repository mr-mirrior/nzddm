using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace DM.DB.datamap
{
    class DAO
    {
        private DAO() { }

        private static DAO myInstance = null;

        public static DAO getInstance()
        {
            if (myInstance == null)
            {
                myInstance = new DAO();
            }
            return myInstance;
        }

        public static bool updateElevations(int blockid, double designz, int segmentid, string elevations)
        {

            string sqlTxt = "update segment set elevations = '" + elevations +
                "' where blockid=" + blockid + " and segmentid=" + segmentid +
                " and designz=" + designz;
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

        //得到当前高程的上一个高程
        public double getLastDesignZ(int blockid, double designz, int segmentid, String dtend)
        {
            List<Segment> segments = new List<Segment>();
            SqlConnection connection = null;
            SqlDataReader reader = null;
            String getdesignDepth = "(select designdepth from segment where blockid=" + blockid + " and segmentid=" + segmentid + " and designz=" + designz + ")*3";
            String sqlTxt = "select top 1 * from segment where blockid=" + blockid + " and designz < '" + (designz.ToString()) + "'and designz>(" + designz + "-" + getdesignDepth + ") and dtend <'" + dtend + "'and workstate=2 order by dtend desc";
            try
            {
                connection = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(connection, sqlTxt);
                if (reader.Read())
                {
                    return Double.Parse(reader["designz"].ToString());
                }
                else
                {
                    return -1;
                }
            }
            catch
            {
                return -1;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(connection);
            }
        }

        public List<Segment> getSegments(Int32 blockID, Double designZ)
        {
            List<Segment> segments = new List<Segment>();
            SqlConnection connection = null;
            SqlDataReader reader = null;
            string sqlTxt = "select * from segment where (blockid=" + blockID + ") and (designZ=" + designZ + ") and workstate=2 ";
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
                    if (reader["SenseOrganState"] != DBNull.Value)
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

        public List<Segment> getSegments(int blockid, double begin_designz, double end_designz)
        {
            List<Segment> segments = new List<Segment>();
            Segment segment = null;
            SqlConnection connection = null;
            SqlDataReader reader = null;
            string sqlTxt = "select * from segment where blockid=" + blockid + " and designz between " + begin_designz + " and " + end_designz + "and workstate=2 order by designz desc,segmentid desc";
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

        public Segment getSegment(Int32 blockID, double designZ, Int32 segmentid)
        {

            Segment segment = null;
            SqlConnection connection = null;
            SqlDataReader reader = null;
            string sqlTxt = "select * from segment where (blockid=" + blockID + ") and (designZ=" + designZ +
                ") and (segmentid=" + segmentid + ")";
            try
            {
                connection = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(connection, sqlTxt);
                if (reader.Read())
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

                }
                return segment;

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
            //segment.BlockName=
            segment.StartDate = (DateTime)(startdate);
            segment.EndDate = (enddate);
            segment.SegmentName = (segmentname);
            segment.StartZ = startZ;
            segment.POP = (double)reader["POP"];
            segment.DesignDepth = (double)reader["DESIGNDEPTH"];
            segment.Vertext = vertex;

            return segment;
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
    }
}
