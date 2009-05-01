﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;

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
        public double getLastDesignZ(int blockid, double designz,int segmentid, String vertex)
        {
            List<Segment> segments = new List<Segment>(); 
            SqlConnection connection = null;
            SqlDataReader reader = null;
            List<double> last_designzs = new List<double>();
            List<String> last_vertexs = new List<string>();
            //dtend = "2009/4/23 17:00:00";
            String getdesignDepth = "(select designdepth from segment where blockid=" + blockid + " and segmentid=" + segmentid + " and designz=" + designz + ")";
            String sqlTxt = "select * from segment where blockid=" + blockid + " and (designz < " + (designz.ToString()) + "-" + getdesignDepth + "*0.5) and designz>(" + designz + "-" + getdesignDepth + "*1.25) and workstate=2 order by dtend desc";
            try
            {
                connection = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(connection, sqlTxt);
                while (reader.Read())
                {
                    double d = (Double.Parse(reader["designz"].ToString()));
                    string v = reader["vertex"].ToString();
                    last_designzs.Add(d);
                    last_vertexs.Add(v);
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

            if (last_designzs == null||last_designzs.Count==0)
            {
                return -1;
            }
            else if (last_designzs.Count == 1)
            {
                return last_designzs[0];
            }
            else
            {
                int count = last_designzs.Count;
                List<Point>[] points = new List<Point>[count];
                Color[] colors = new Color[count];
                int[] pixel_count = new int[count];
                int color_range = 255 / (count + 2);
                List<Point> all = new List<Point>();
                List<Point> up_Points = DataMapManager.getSegmentVertex_DAM(vertex);//上一层所有点
                all.AddRange(up_Points);
                for (int i = 0; i < count;i++ )
                {
                    int rgb = (i + 1) * color_range;
                    colors[i] = Color.FromArgb(rgb,rgb,rgb);//设定颜色
                    points[i] = DataMapManager.getSegmentVertex_DAM(last_vertexs[i]);
                    all.AddRange(points[i]);
                }
                Point origin = DataMapManager.getOrigin(all.ToArray());
                up_Points = DataMapManager.getRelatively(origin, up_Points);
                for (int i = 0; i < count; i++)
                {
                    points[i] = DataMapManager.getRelatively(origin, points[i]);
                }
                int left_index = DataMapManager.getLeftIndex(all);
                int right_index = DataMapManager.getRightIndex(all);
                int top_index = DataMapManager.getTopIndex(all);
                int bottom_index = DataMapManager.getBottomIndex(all);
                int map_width = all[right_index].X - all[left_index].X;
                int map_height = all[bottom_index].Y - all[top_index].Y;

                Bitmap[] bitmaps = new Bitmap[count];
                Graphics[] graphicses = new Graphics[count];

                for (int i = 0; i < count; i++)
                {
                    bitmaps[i] = new Bitmap(map_width, map_height);
                    graphicses[i] = Graphics.FromImage(bitmaps[i]);
                    graphicses[i].Clear(Color.White);
                }

                System.Drawing.Drawing2D.GraphicsPath up_GraphicsPath = new System.Drawing.Drawing2D.GraphicsPath();
                up_GraphicsPath.Reset();
                up_GraphicsPath.AddPolygon(up_Points.ToArray());

                for (int i = 0; i < count; i++)
                {
                    System.Drawing.Drawing2D.GraphicsPath this_GraphicsPath = new System.Drawing.Drawing2D.GraphicsPath();
                    this_GraphicsPath.Reset();
                    this_GraphicsPath.AddPolygon(points[i].ToArray());
                    System.Drawing.Region this_region = new Region(this_GraphicsPath);
                    graphicses[i].FillRegion(Brushes.Red, this_region);
                    //本region与主region的交集
                    this_region.Intersect(up_GraphicsPath);
                    graphicses[i].FillRegion(new SolidBrush(colors[i]), this_region);
                    graphicses[i].Flush();
                    //bitmaps[i].Save("c:/" + blockid + " " + designz + " " + segmentid +" -"+i+ ".png", ImageFormat.Png);
                }
                

              for (int i = 0; i < count; i++)
           {
  
                for (int x = 0; x < map_width; x++)
                {
                    for (int y = 0; y < map_height; y++)
                    {
                        Color c = bitmaps[i].GetPixel(x, y);
                        
                            if (c.A == colors[i].A && c.R == colors[i].R && c.G == colors[i].G)
                            {
                                pixel_count[i]++;
                                break;
                            }
                    }
                }
            }
                int max = 0;
                for (int i = 0; i < count;i++ )
                {
                    if (max<pixel_count[i])
                    {
                        max = pixel_count[i];
                    }
                }
                if (max != 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (max == pixel_count[i])
                        {
                            return last_designzs[i];
                        }
                    }
                }
                return -1;
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