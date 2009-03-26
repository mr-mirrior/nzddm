using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
namespace DM.DB.datamap
{
    class DataMapManager
    {
        	//传入当地坐标

        public static int grid = 5;//"大网格"的边长

     public static float WIDTH = 0.5f;//每个正方形网格的宽度/单位米
    public static int SCREEN_ONEMETER = 10;//每米的像素数,每个网格的大小刚好是SCREEN_ONEMETER的一半
    public static double sin = 0.5509670120356448784912018921605;
    public static double cos = 0.83452702271916488948079272306091;
    static int map_left = 80;//主图形左侧
    static int map_bottom = 200;//主图形下侧
    static int map_right = 80;//右侧
    static int map_top = 160;//上侧

    public static Bitmap draw(int blockid, float designz, int segmentid)
    {
//从数据库中读出本仓面,和本仓面上一层的所有仓面信息.
        Segment segment = DAO.getInstance().getSegment(blockid, designz, segmentid);
        if (segment == null)
            return null;
        //得到上一层的所有仓面信息
        float lastDesignz = DAO.getInstance().getLastDesignZ(blockid, designz);        
        List<Segment> segments = SegmentDAO.getInstance().getSegments(blockid, lastDesignz);
        //将上一层所有仓面的数据图读出来
        for (int ii = 0; ii < segments.Count; ii++)
        {
            Segment tsegment = segments[ii];
            byte[] datamap = DAO.getInstance().getDatamap(blockid, lastDesignz, segment.SegmentID);
            if (datamap == null)
                return null;
            tsegment.Datamap = datamap;
        }
        String vertex = segment.Vertext;//大地坐标的vertex
        byte[] bytes = DAO.getInstance().getDatamap(blockid, designz, segmentid);//本仓面的数据图
        if (bytes==null)
        {
            return null;
        }
        DataMap dm = new DataMap(bytes);

//通过vertex得到大坝坐标系的边界点
        List<Point> dam_points = getSegmentVertex_DAM(vertex);
//大把坐标系下的外切矩阵的原点,把屏幕坐标转换成大坝坐标会用到
        Point dam_origin = getOrigin(dam_points.ToArray());
//转换成相对这组坐标的原点的“新点”也相当于屏幕坐标
        List<Point> screen_points = getRelatively(dam_points);
//取得大坝坐标系边界点的外切矩阵
        int left_index = getLeftIndex(screen_points);
        int right_index = getRightIndex(screen_points);
        int top_index = getTopIndex(screen_points);
        int bottom_index = getBottomIndex(screen_points);
        //外切矩形四个顶点
        Point left_top = new Point(screen_points[left_index].X, screen_points[top_index].Y);
        Point left_bottom = new Point(screen_points[left_index].X, screen_points[bottom_index].Y);
        Point right_top = new Point(screen_points[right_index].X, screen_points[top_index].Y);
        Point right_bottom = new Point(screen_points[right_index].X, screen_points[bottom_index].Y);
        
        //大坝坐标下本仓面外切矩形的宽度
        int segment_dam_width = screen_points[right_index].X - screen_points[left_index].X;
        //大坝坐标下本仓面外切矩形的高度
        int segment_dam_height = screen_points[bottom_index].Y - screen_points[top_index].Y;
        //定义出图像大小
        Bitmap map = new Bitmap(segment_dam_width +map_left+map_right,segment_dam_height +map_top+map_bottom);
        Graphics g = Graphics.FromImage(map);
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        g.Clear(Color.White);
        Font f = new Font("微软雅黑",8f);

//取得最大的厚度.最小的厚度.这部分比较耗费时间
        int x = dm.getWidth();
        int y = dm.getHeight();
        Coordinate c = getOriginOfCoordinate(vertex);
        double c_x = c.getX();//当前x
        double c_y = c.getY();//当前y
        float max = float.MinValue;
        float min = float.MaxValue;
        Pixel lastp;
        float difference;
        float sum = 0f;
        Pixel p = null;
        int i = 0;//行号
        int m = 0;//列号
        List<float> difference_s = new List<float>();
        for (i = 0; i < y; i++)
        {
            c_y += WIDTH;//y值加
            c_x = c.getX();//置为原点x值
            for (m = 0; m < x; m++)
            {
                c_x += WIDTH;//x值加
                p = dm.getPixel(m, i);
                if (p.getRollcount() != 255 && p.getRollthickness() != 0)//是否为仓面上的点
                {
                    lastp = DataMapManager.getPixel("" + c_x, "" + c_y, segments);//得到上一点的数据
                    if (lastp != null && lastp.getRollthickness() != 0 && lastp.getRollcount() != 255)
                    {//
                        difference = p.getRollthickness() - lastp.getRollthickness();
                        if (max < difference) { max = difference; };
                        if (min > difference) { min = difference; };
                        sum += difference;
                        difference_s.Add(difference);
                    }
                }
            }
        }
//计算方差和均值
        double average_difference = sum / difference_s.Count;//平均厚度
        double deviation = 0f;//方差
        double temp = 0;
        for (int temp_index = 0; temp_index < difference_s.Count; temp_index++)
        {
            temp += Math.Pow((difference_s[temp_index] - average_difference), 2);
        }
        deviation = temp / difference_s.Count;
        double standard_deviation = Math.Sqrt(deviation);
//初始化
        int hengxian = (left_bottom.Y - left_top.Y) / (grid * SCREEN_ONEMETER)+1;
        int shuxian = (right_bottom.X - left_bottom.X) / (grid * SCREEN_ONEMETER)+1;
        double[,] sum_grid = new double[hengxian,shuxian];//存储大网格的厚度和
        int[,] count_grid = new int[hengxian, shuxian];//存储每个大网格包含的可用小网格数
//最关键的部分.填形状
        Color color = Color.Black;
        float red;
        float blue;
        float green;

        //for (int thisy = left_top.Y; thisy <= left_bottom.Y;thisy+= (SCREEN_ONEMETER / 2) )
        for (int thisy = left_bottom.Y; thisy >= left_top.Y; thisy -= (SCREEN_ONEMETER / 2))//从屏幕坐标的最大y开始画
        {
            for (int thisx = left_top.X; thisx <= right_top.X; thisx += (SCREEN_ONEMETER / 2))
            {

//第一种实现方式,先判断该屏幕坐标点在不在屏幕坐标系下的仓面内.如果在,转换成大地坐标来找.
                
                //将屏幕坐标转换为大坝坐标
                Point screen_point = new Point(thisx, thisy);
                if (inThisSegment(screen_point,screen_points.ToArray()))
                {
                    //屏幕转大坝
                    Point dam_point = screenToDam(dam_origin, screen_point);
                    //大坝转大地
                    Coordinate earth_point = damToEarth(dam_point);
                    //取得大地坐标在数据图中的行列
                    Point row_column = getRowColumn_Earth(vertex, earth_point);
                    //取得该行列上的数据
                    Pixel pixel = dm.getPixel(row_column.X, row_column.Y);
                    if (pixel.getRollcount() != 255 && pixel.getRollthickness() != 0)//是否为仓面上的点
                    {
                        Pixel lastpixel = getPixel("" + earth_point.getX(), "" + earth_point.getY(), segments);
                        if (lastpixel != null && lastpixel.getRollthickness() != 0 && lastpixel.getRollcount() != 255)
                        {//
                            difference = pixel.getRollthickness() - lastpixel.getRollthickness();

                            int zong = (screen_point.X - left_bottom.X) / (grid * SCREEN_ONEMETER);//所在的列
                            int heng = (right_bottom.Y - screen_point.Y ) / (grid * SCREEN_ONEMETER);

                            sum_grid[heng,zong] += difference;
                            count_grid[heng,zong] += 1;

                            //根据不同厚度值.体现不同的颜色
                            red = (difference - min) / (max - min) * 255;
                            blue = (difference - min) / (max - min) * 255;
                            green = (difference - min) / (max - min) * 255;
                            color = Color.FromArgb((int)red, (int)green, (int)blue);
                            color = g.GetNearestColor(color);
                        }
                        else
                        {
                            color = Color.White;
                        }
                    }
                    else
                    {
                        color = Color.White;
                    }
                    g.FillRectangle(new SolidBrush(color), thisx+2, thisy, SCREEN_ONEMETER / 2, SCREEN_ONEMETER / 2);
                }
                
//(未测试)  第二种实现方式,直接将屏幕坐标点,转换成大地坐标.直接去找.
                /*Point dam_point = screenToDam(dam_origin,screen_point);
                Coordinate earth_point = damToEarth(dam_point);
                //先判断这点是不是在仓面里面
                if (inThisSegment(vertex, "" + earth_point.getX(), "" + earth_point.getY()))
                {
                    Point row_column = getRowColumn_Earth(vertex, earth_point);
                    Pixel pixel = dm.getPixel(row_column.X, row_column.Y);

                    if (pixel.getRollcount() != 255 && pixel.getRollthickness() != 0)//是否为仓面上的点
                    {
                        Pixel lastpixel = getPixel("" + earth_point.getX(), "" + earth_point.getY(), segments);
                        if (lastpixel != null && lastpixel.getRollthickness() != 0 && lastpixel.getRollcount() != 255)
                        {//
                            difference = pixel.getRollthickness() - lastpixel.getRollthickness();
                            //根据不同厚度值.体现不同的颜色
                            red = (difference - min) / (max - min) * 255;
                            blue = (difference - min) / (max - min) * 255;
                            green = (difference - min) / (max - min) * 255;
                            color = Color.FromArgb((int)red, (int)green, (int)blue); ;
                        }
                        else
                        {
                            color = Color.White;
                        }
                    }
                    else
                    {
                        color = Color.White;
                    }
                }
                else
                {
                    color = Color.Black;
                }
                g.FillRectangle(new SolidBrush(color),thisx, thisy, SCREEN_ONEMETER/2, SCREEN_ONEMETER/2);
*/
            }
        }
//大坝边界线
        g.DrawLines(Pens.Green, screen_points.ToArray());
//纵轴
        Pen pen = new Pen(Color.Black, 2);
//        pen.DashStyle = DashStyle.Dash;
        pen.StartCap = LineCap.Flat;
        pen.EndCap = LineCap.ArrowAnchor;

        int jiachang = 20;
        int putongjiachang = 10;
        g.DrawLine(pen, left_bottom, new Point(left_top.X,left_top.Y-jiachang));        
//横轴
        g.DrawLine(pen, left_bottom, new Point(right_bottom.X+jiachang,right_bottom.Y));

        pen = new Pen(Color.Black, 1);
        pen.DashStyle = DashStyle.Dash;

//横线们       
        for (i = 0; i < hengxian;i++ )
        {
            Point start = new Point(left_bottom.X, left_bottom.Y - i * grid * SCREEN_ONEMETER);
            Point end = new Point(right_bottom.X+putongjiachang, right_bottom.Y - i * grid * SCREEN_ONEMETER);
            g.DrawLine(pen, start, end);
        }
//纵线们       
        for (i = 0; i < shuxian; i++)
        {
            Point start = new Point(left_bottom.X + i * grid * SCREEN_ONEMETER, left_bottom.Y);
            Point end = new Point(left_top.X + i * grid * SCREEN_ONEMETER, left_top.Y-putongjiachang);
            g.DrawLine(pen, start, end);
        }

        HatchBrush hb = new HatchBrush(HatchStyle.SmallConfetti, Color.Black, Color.Black);

//数值们
        for (m = 0; m < hengxian; m++)
        {
            int y_m = left_bottom.Y - (m+1) * (SCREEN_ONEMETER * grid);
            for (int n  = 0; n< shuxian; n++)
            {                
                int x_n = left_bottom.X + n * (SCREEN_ONEMETER * grid) ;
                int count = count_grid[m, n];
                if (count!=0)
                {
                    double average = sum_grid[m, n] / count_grid[m, n];
                    System.Globalization.NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo();
                    nfi.NumberDecimalDigits = 4;
                    g.DrawString(average.ToString("N", nfi) + "m", f, new SolidBrush(Color.Red), new Point(x_n, y_m));
                }
            }
        }
//标题
        Font titlefont = new Font("微软雅黑", 24f);
        Point titlep = new Point(10, 10);
        String title = "碾压厚度图形报告";
        String sub_title = "分区   "+segment.BlockID+"     仓面名称   "+segment.SegmentName+"      高程   "+segment.DesignZ+"m";
        g.DrawString(title, titlefont, hb, titlep);
        g.DrawLine(Pens.Black,new Point(left_top.X, titlep.Y + titlefont.Height + 3), new Point(right_top.X, titlep.Y + titlefont.Height + 3));
        g.DrawString(sub_title, f,hb, new Point(left_top.X, titlep.Y + titlefont.Height + 5));
        String startd = segment.StartDate.ToString();
        String endd = segment.EndDate.ToString();
        g.DrawString("开始时间:" + startd, f, hb, new Point(right_top.X-30, titlep.Y + titlefont.Height + 5+ f.Height));
        g.DrawString("结束时间:" + endd, f, hb, new Point(right_top.X - 30, titlep.Y + titlefont.Height + 5 + f.Height * 2));
//原点,平均值，标准差
        Point left_bottom_dam = screenToDam(dam_origin,left_bottom);//右下角点的大把坐标
        Point yuandian = new Point(left_bottom.X-20, left_bottom.Y + 2);
        g.DrawString("("+left_bottom_dam.X+","+left_bottom_dam.Y+")     厚度均值:"+average_difference+"m    厚度标准差:"+standard_deviation, f,hb, yuandian);
//坝
        Point right_bottom_dam = screenToDam(dam_origin, right_bottom);
        Point youxia = new Point(yuandian.X + jiachang + 20 + right_bottom.X - left_bottom.X , right_bottom.Y - 10);
        g.DrawString("坝(m)",f,hb,youxia);
        g.DrawString(right_bottom_dam.X.ToString(), f, hb, new Point(yuandian.X+jiachang + 10 + right_bottom.X - left_bottom.X, yuandian.Y));
//轴
        Point left_top_dam = screenToDam(dam_origin, left_top);
        Point zuoshang = new Point(left_top.X - 5, left_top.Y - jiachang - 20);
        g.DrawString("轴(m)", f, hb, zuoshang);
        g.DrawString("("+left_top_dam.X+","+left_top_dam.Y+")", f, hb, new Point(yuandian.X-40 , left_top.Y-jiachang-10));
//图例
             
            //三角指示
            int sanjiao =  25;
            int sanjiao_height = 15;
            PointF[] points = new PointF[3];
            points[0] = new PointF(left_bottom.X+sanjiao,left_bottom.Y+sanjiao+sanjiao_height);//左下
            points[1] = new PointF(right_bottom.X - sanjiao, right_bottom.Y + sanjiao);//右上
            points[2] = new PointF(right_bottom.X - sanjiao, right_bottom.Y + sanjiao+sanjiao_height);//右下
            g.FillPolygon(Brushes.Gray, points);
            g.DrawPolygon(Pens.Black, points);
            //矩形渐变            
            PointF juxingjianbian = new PointF(points[0].X, points[0].Y + 10);
            float juxingjianbian_width = points[2].X-points[0].X;
            float juxingjianbian_height = 15f;
            RectangleF r= new RectangleF(juxingjianbian,new SizeF(juxingjianbian_width , juxingjianbian_height));
            Brush b = new LinearGradientBrush(r, Color.Black, Color.White, 0f);
            g.DrawRectangle(Pens.Black,Rectangle.Round(r));
            g.FillRectangle(b,r);
            //文字
                //最小值
            g.DrawString("最小值:"+min.ToString()+"m", f, new SolidBrush(Color.Black), new PointF(juxingjianbian.X,juxingjianbian.Y+25));
                //最大值
            g.DrawString("最大值:"+max.ToString()+"m", f, new SolidBrush(Color.Black), new PointF(juxingjianbian.X+juxingjianbian_width, juxingjianbian.Y + 25));
                //中间值
            g.DrawString((min+(max-min)/2).ToString() + "m", f, new SolidBrush(Color.Black), new PointF(juxingjianbian.X + juxingjianbian_width/2, juxingjianbian.Y + 25));
        
        //垃圾回收尚未进行,需要看书了解.
        return map;
//存图
       /* try
        {
           // map.Save("c:/test.gif", ImageFormat.Gif);
           // map.Save("c:/test.png", ImageFormat.Png);
           // map.Save("c:/test.jpg", ImageFormat.Jpeg);

             Console.WriteLine("导出数据成功！");
        }
        catch (Exception e)
        {
            Console.WriteLine("导出数据失败！");
            //e.printStackTrace();
        }*/

    }

    public static Pixel getPixel(String x, String y, List<Segment> segments)
    {
        //确定所属仓面
        //System.out.println(blockid+" "+designz);		 
        for (int i = 0; i < segments.Count; i++)
        {
            Segment segment = segments[i];
            String vertex = segment.Vertext;
            //System.out.println(vertex+" "+x+" "+y);
            //byte[] bytes1 = getBytes(blockid,designz,segment.getSegmentID());
            //DataMap.test(bytes1);
            if (inThisSegment(vertex, x, y))
            {
                //System.out.println("在此舱面内");
                //根据原点坐标来计算出他对应数据图中的哪行哪列
                float x_ = float.Parse(x);
                float y_ = float.Parse(y);
                Coordinate origin = getOriginOfCoordinate(vertex);
                int m = (int)((x_ - origin.getX()) / WIDTH);
                int n = (int)((y_ - origin.getY()) / WIDTH);
                if (m > 0 && n > 0)
                {
                    byte[] bytes = segment.Datamap;
                    Pixel p = (new DataMap(bytes)).getPixel(m, n);
                    p.setSegmentid(segment.SegmentID);
                    return p;
                }
                else
                {
                    return null;
                }
            }
        }
        return null;
    }

    //判断x,y是不是在一个多边形里
    private static bool inThisSegment(String vertex, String x, String y)
    {
        String[] points = vertex.Split(';');
        //int[] x_points = new int[points.Length]; 
        //int[] y_points = new int[points.Length];
        Point[] point = new Point[points.Length - 1];//因为最后有个分号
        double getX = 100 * double.Parse(x);
        double getY = 100 * Double.Parse(y);
        for (int i = 0; i < points.Length - 1; i++)
        {
            String[] xy = points[i].Split(',');
            float thisx = float.Parse(xy[0]);
            float thisy = float.Parse(xy[1]);
            int xint = (int)(thisx * 100);
            int yint = (int)(thisy * (-100));
            point[i] = new Point(xint, yint);
        }

        System.Drawing.Drawing2D.GraphicsPath myGraphicsPath = new System.Drawing.Drawing2D.GraphicsPath();
        myGraphicsPath.Reset();
        myGraphicsPath.AddPolygon(point);
        Point p = new Point((int)((float.Parse(x)) * 100), (int)((float.Parse(y)) * (100)));
        return myGraphicsPath.IsVisible(p);
    }

    //判断x,y是不是在一个多边形里
    private static bool inThisSegment(Point p, Point[] vertex)
    {
        
        System.Drawing.Drawing2D.GraphicsPath myGraphicsPath = new System.Drawing.Drawing2D.GraphicsPath();
        myGraphicsPath.Reset();
        myGraphicsPath.AddPolygon(vertex);
        return myGraphicsPath.IsVisible(p);
    }

    //得到原点坐标,因为vertex中的x坐标为正数，y坐标为负数.所以取vertex中的最小x，和绝对值最小y
    static Coordinate getOriginOfCoordinate(String vertex)
    {
        String[] points = vertex.Split(';');
        float x = float.MaxValue;
        float y = float.MaxValue;
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].Equals(""))
            {
                break;
            }
            String[] xy = points[i].Split(',');
            float thisx = float.Parse(xy[0]);
            float thisy = float.Parse(xy[1]) * (-1);
            if (x > thisx)
            {
                x = thisx;
            }
            if (y > thisy)
            {
                y = thisy;
            }
        }
        Coordinate c = new Coordinate(x, y);
        return c;
    }
    //传入大地坐标的vertex,和大地坐标的一个点.返回这个点在大地坐标中的行号列号点
    static Point getRowColumn_Earth(String vertex,Coordinate earth_point)
    {
        Coordinate o = getOriginOfCoordinate(vertex);
        double x = (earth_point.getX()-o.getX()) / WIDTH;
        double y = (earth_point.getY()-o.getY()) / WIDTH;
        return new Point((int)x, (int)y);
    }

    //得到这些点外切矩形的原点
    static Point getOrigin(Point[] points)
    {
        float x = float.MaxValue;
        float y = float.MaxValue;
        for (int i = 0; i < points.Length; i++)
        {
            Point p = points[i];
            int thisx = p.X;
            int thisy = p.Y;
            if (x > thisx)
            {
                x = thisx;
            }
            if (y > thisy)
            {
                y = thisy;
            }
        }
        return new Point((int)x, (int)y);
    }
    //大坝坐标转成屏幕坐标
    static List<Point> getRelatively(List<Point> points)
    {
        List<Point> newPoints = new List<Point>();
        Point origin = getOrigin(points.ToArray());
        for (int i = 0; i < points.Count; i++)
        {
            newPoints.Add(damToScreen(origin, points[i]));
        }
        return newPoints;
    }
    //大坝坐标转屏幕坐标   origin为大坝坐标外切矩形原点
    static Point damToScreen(Point origin,Point dam_point){
        int thisx = (dam_point.X - origin.X) * SCREEN_ONEMETER + map_left;
        int thisy = (dam_point.Y - origin.Y) * SCREEN_ONEMETER + map_top;
        return new Point(thisx, thisy );
    }

    static Point screenToDam(Point origin,Point screen_point){
        int thisx = (screen_point.X - map_left) / SCREEN_ONEMETER + origin.X;
        int thisy = (screen_point.Y - map_top) / SCREEN_ONEMETER + origin.Y;
        return new Point(thisx, thisy);
    }
    
    //通过大地坐标系的vertex字符串得到大坝坐标下的边界点值
    static List<Point> getSegmentVertex_DAM(String vertex)
    {
        List<Point> cs = new List<Point>();
        Coordinate o = getOriginOfCoordinate(vertex);//得到原点坐标
        String[] points = vertex.Split(';');
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].Equals(""))
            {
                break;
            }
            String[] xy = points[i].Split(',');
            double thisx = float.Parse(xy[0]);
            double thisy = float.Parse(xy[1]) * (-1);
            Coordinate c = earthToDam(new Coordinate(thisx, thisy));

            cs.Add(new Point((int)c.getX(),(int)c.getY()));
        }
        return cs;
    }
    //将大地坐标转换成大坝坐标
    static Coordinate earthToDam(Coordinate p)
    {
        double x0 = -cos * p.getX() - sin * p.getY() + 46557.7811830799932563179112397188;
        double y0 = +sin * p.getX() - cos * p.getY() - 20616.2311146461071871455578251375;
        return new Coordinate(x0, y0);

    }
    //将大把坐标转换成大地坐标
    static Coordinate damToEarth(Point p)
    {
        double x = -cos * p.X + sin * p.Y + 50212.59;
        double y = -sin * p.X - cos * p.Y + 8447;
        return new Coordinate(x, y);
    }
    //得到这一堆点最左的点在这个数组中的下标,最小的X
    static int getLeftIndex(List<Point> cs)
    {
        int i = -1;
        double minx = double.MaxValue;
        foreach (Point c in cs)
        {
            if (minx > c.X)
            {
                minx = c.X;
            }
        }
        i = 0;
        foreach (Point c in cs)
        {
            if (minx == c.X)
            {
                break;
            }
            i++;
        }
        return i;
    }
    //最大的X
    static int getRightIndex(List<Point> cs)
    {
        int i = -1;
        double maxx = double.MinValue;
        foreach (Point c in cs)
        {
            if (maxx < c.X)
            {
                maxx = c.X;
            }
        }
        i = 0;
        foreach (Point c in cs)
        {
            if (maxx == c.X)
            {
                break;
            }
            i++;
        }
        return i;
    }
    //最大的Y
    static int getBottomIndex(List<Point> cs)
    {
        int i = -1;
        double maxy = double.MinValue;
        foreach (Point c in cs)
        {
            if (maxy < c.Y)
            {
                maxy = c.Y;
            }
        }
        i = 0;
        foreach (Point c in cs)
        {
            if (maxy == c.Y)
            {
                break;
            }
            i++;
        }
        return i;
    }
    //最小的Y
    static int getTopIndex(List<Point> cs)
    {
        int i = -1;
        double miny = double.MaxValue;
        foreach (Point c in cs)
        {
            if (miny > c.Y)
            {
                miny = c.Y;
            }
        }
        i = 0;
        foreach (Point c in cs)
        {
            if (miny == c.Y)
            {
                break;
            }
            i++;
        }
        return i;
    }

    }
}
