using System.Xml.Serialization;
using System.Drawing;
using System.Runtime.InteropServices;
using System;

namespace DM.Geo
{
    /// <summary>
    /// 基本2D坐标点

    /// </summary>
    public struct Coord
    {
//         public Coord() { x = 0; y = 0; /*z = 0;*/ }
        public Coord(double xx, double yy) { x = xx; y = yy; /*z = 0;*/ }
        public Coord(Coord c) { x = c.x; y = c.y; }
        double x;
        double y;
//         double z;

        public double X { get { return x; } set { x = value; } }
        public double Y { get { return y; } set { y = value; } }
//         public double Z { get { return z; } set { z = value; } }

        [XmlIgnore] public float XF { get { return (float)x; } set { x = value; } }
        [XmlIgnore] public float YF { get { return (float)y; } set { y = value; } }
//         [XmlIgnore] public float ZF { get { return (float)z; } set { z = value; } }

        [XmlIgnore]
        public Point PT { get { return new Point((int)x, (int)y); } set { x = value.X; y = value.Y; /*z = 0;*/ } }
        [XmlIgnore]
        public PointF PF { get { return new PointF((float)x, (float)y); } set { x = value.X; y = value.Y; /*z = 0;*/ } }

        public Coord(Point pt) { x = pt.X; y = pt.Y; /*z = 0;*/ }
        public Coord(PointF pt) { x = pt.X; y = pt.Y; /*z = 0;*/ }
        public override string ToString()
        {
            return string.Format("{{X={0:0.00}, Y={1:0.00}}}", x, y/*, z*/);
        }
        /// <summary>
        /// 完全相等判断，包含Z坐标
        /// </summary>
        /// <param name="obj">比较对象</param>
        /// <returns>如果完全相等返回true，否则false</returns>
        public bool IsEqual(Coord c)
        {
            return c.x == x && c.y == y /*&& c.z == z*/;
        }
        public Coord Origin(Coord c)
        {
            double xx = x;
            double yy = y;
            xx -= c.x;
            yy -= c.y;
            return new Coord(xx,yy);
        }
        public Coord Negative()
        {
            return new Coord(-x, -y);
        }
        public Coord Offset(double x1, double y1)
        {
            double xx = x;
            double yy = y;
            xx += x1;
            yy += y1;
            return new Coord(xx, yy);
        }
        public Coord Offset(Coord c)
        {
            return Offset(c.x, c.y);
        }
        public Coord Scale(double zoom)
        {
            double xx = x;
            double yy = y;
            xx *= zoom;
            yy *= zoom;
            return new Coord(xx, yy);
        }

        public static int XGreater(Coord c1, Coord c2)
        {
            double delta = c1.x - c2.x;
            if (delta < 0) return -1;
            if (delta == 0) return 0;
            /*if (delta > 0) */return 1;
        }
        public static int YGreater(Coord c1, Coord c2)
        {
            double delta = c1.y - c2.y;
            if (delta < 0) return -1;
            if (delta == 0) return 0;
            /*if (delta > 0) */return 1;
        }
        public static Coord operator -(Coord c1, Coord c2)
        {
            return new Coord(c1.x-c2.x, c1.y-c2.y);
        }

        /*
        X0 = -COS *X - SIN *Y + 46557.7811830799932563179112397188
        Y0 =  SIN *X - COS *Y - 20616.2311146461071871455578251375
        式中，X、Y为大地坐标，X0、Y0为坝轴线坐标。


        反算公式：

        X = － COS *X0 ＋ SIN *Y0 ＋ 50212.59
        Y = － SIN *X0 － COS *Y0 ＋ 8447

        SIN=0.5509670120356448784912018921605
        COS=0.83452702271916488948079272306091
         */
        // 大地坐标 -> 坝轴坐标
        public Geo.Coord ToDamAxisCoord()
        {
            double SIN = 0.5509670120356448784912018921605;
            double COS = 0.83452702271916488948079272306091;

            Geo.Coord cod0 = new DM.Geo.Coord();
            cod0.X = (-COS * this.X + SIN * this.Y + 46557.7811830799932563179112397188);
            cod0.Y = (SIN * this.X + COS * this.Y - 20616.2311146461071871455578251375);

            return cod0;
        }
        // 坝轴坐标 -> 大地坐标
        public Geo.Coord ToEarthCoord()
        {
            double SIN = 0.5509670120356448784912018921605;
            double COS = 0.83452702271916488948079272306091;

            Geo.Coord c = new DM.Geo.Coord();
            c.X = (-COS * this.X + SIN * this.Y + 50212.59);
            c.Y = (-SIN * this.X - COS * this.Y + 8447);

            c.Y = -c.Y;
            return c;
        }

//         public static int ZGreater(Coord c1, Coord c2 )
//         {
//             double delta = c1.z - c2.z;
//             if (delta < 0) return -1;
//             if (delta == 0) return 0;
//             /*if (delta > 0) */return 1;
//         }
    }

    public struct Coord3D
    {
        public byte tag1;
        double v;
        public double V { get { return v; } set { v = value; } }
        Coord plane;
        public Coord Plane { get { return plane; } set { plane = value; } }
        double z;
        public double Z { get { return z; } set { z = value; } }
        public Coord3D(double xx, double yy, double zz)
        {
            when = DateTime.MinValue;
            tag1 = 0;

            v = 0;
            plane = new Coord(xx,yy);
            z = zz;
        }
        public Coord3D(Coord3D c, byte t1)
        {
            when = DateTime.MinValue;
            this.plane = c.Plane;
            this.z = c.z;
            this.v = c.v;
            this.tag1 = t1;
        }
        public Coord3D(double xx, double yy, double zz, double vv, byte t1)
        {
            when = DateTime.MinValue;
            tag1 = t1;

            plane = new Coord(xx, yy);
            z = zz;
            v = vv;
        }
        public Coord3D(double xx, double yy, double zz, double vv, byte t1,DateTime dt)
        {
            when = dt;
            tag1 = t1;

            plane = new Coord(xx, yy);
            z = zz;
            v = vv;
        }
        public Coord3D(Coord c, double zz)
        {
            when = DateTime.MinValue;
            tag1 = 0;

            v = 0;
            plane = c;
            z = zz;
        }
        public override string ToString()
        {
            return string.Format("{{X={0:0.00},Y={1:0.00},Z={2:0.00},V={3:0.00}}}",plane.X, plane.Y, z, v);
        }
        DateTime when;
        public DateTime When { get { return when; } set { when = value; } }
    }
}
