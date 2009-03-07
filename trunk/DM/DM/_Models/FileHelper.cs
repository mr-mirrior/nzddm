using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DM.Geo;

namespace DM.Models
{
    public static class FileHelper
    {
//         public static void WriteTracking(List<Coord3D> lst)
//         {
//             FileStream fs = new FileStream("TrackingExp.txt", FileMode.OpenOrCreate);
//             StreamWriter sw = new StreamWriter(fs);
//             for (int i = 0; i < lst.Count; i++ )
//             {
//                 Coord3D c3d = lst[i];
//                 Coord c = c3d.Plane;
// 
//                 sw.WriteLine("{0:0.000},{1:0.000},{2:0.000}",);
//             }
//             sw.Close();
//             fs.Close();
//         }
        public static List<Coord3D> ReadTracking(string file)
        {
            List<Coord3D> tracking = new List<Coord3D>();
            try
            {
                FileStream fs = new FileStream(file, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string line = sr.ReadLine();
                double ZBase = double.PositiveInfinity;
                while (line != null)
                {
                    if (line.Length != 0)
                    {
                        string[] s = line.Trim().Split(new char[] { ',' });
                        if (s.Length != 3)
                        {
                            //MessageBox.Show("轨迹读取异常，请检查xyz.txt");
                            line = sr.ReadLine();
                            continue;
                        }
                        tracking.Add(new Coord3D(Convert.ToSingle(s[0]), -Convert.ToSingle(s[1]), Convert.ToSingle(s[2])));
                        if (ZBase > tracking.Last().Z)
                            ZBase = tracking.Last().Z;
                        //float xyz = Convert.ToSingle(s[3]);
                    }
                    line = sr.ReadLine();
                }
                sr.Close();
                fs.Close();

                for (int i = 0; i < tracking.Count; i++)
                {
                    tracking[i] = new Coord3D(tracking[i].Plane, tracking[i].Z - ZBase);
                }
                return tracking;
            }
            catch
            {
                return null;
            }
        }
        public static List<Coord> ReadLayer(string fullpath)
        {
            try
            {
                List<Coord> pts = new List<Coord>();
                FileStream fs = new FileStream(fullpath, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string line = sr.ReadLine();
                while (line != null)
                {
                    if (line.Length != 0)
                    {
                        string[] s = line.Trim().Split(new char[] { ',' });
                        pts.Add(new Coord(Convert.ToSingle(s[0]), -Convert.ToSingle(s[1])));
                    }
                    line = sr.ReadLine();
                }
                fs.Close();

                return pts;
            }
            catch (FileNotFoundException)
            {
                Utils.MB.Error(fullpath+" not found!");
            }
            return null;
        }
    }
}
