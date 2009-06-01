using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Xml.Serialization;

namespace DM.Models
{
    /// <summary>
    /// 分区
    /// </summary>
    public class Partition
    {
        private Partition() { }
        public Partition(int ID, string NAME, string DESCR) { id = ID; name = NAME; description = DESCR; }
        public Partition(string n) { id = 0; name = n; description = ""; }

        int id;
        public int ID { get { return id; } set { id = value; } }
        string name;
        public string Name { get { return name; } set { name = value; } }
        string description;
        public string Description { get { return description; } set { description = value; } }
        public static string GetName(int id)
        {
            switch (id)
            {
                case 10:
                    return "EU";
                case 11:
                    return "ED";
                case 12:
                    return "RU1";
                case 13:
                    return "RU2";
                case 14:
                    return "RU3";
                case 15:
                    return "RD1";
                case 16:
                    return "RD2";
                case 17:
                    return "RD3";
                case 18:
                    return "FU";
                case 19:
                    return "FD";
                case 29:
                    return "RU4";
                case 35:
                    return "EJ";
                default:
                    return "测试区";
            }
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Partition))
            {
                Partition p = (Partition)obj;
                return name.Equals(p.name, StringComparison.OrdinalIgnoreCase);
            }
            if (obj.GetType() == typeof(string))
            {
                return name.Equals(obj as string, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
        public bool IsEmpty() { return name == null; }
        /*
         10	EU	心墙上部	NULL	NULL
        11	ED	心墙下部	NULL	NULL
        12	RU1	上游粗堆石Ⅰ区	NULL	NULL
        13	RU2	上游粗堆石Ⅱ区	NULL	NULL
        14	RU3	上游细堆石区	NULL	NULL
        15	RD1	下游粗堆石Ⅰ区	NULL	NULL
        16	RD2	下游粗堆石Ⅱ区	NULL	NULL
        17	RD3	下游细堆石区	NULL	NULL
        18	FU	上游反滤区	NULL	NULL
        19	FD	下游反滤区	NULL	NULL
        29	RU4	上游粗堆石调节区	NULL	NULL
        31	test1	碾压测试区	NULL	NULL
        32	LeftTop	左岸坝顶试验区	NULL	NULL
        35	EJ	接触粘土区	NULL	NULL
        36 yingdi 业主营地门前 NULL NULL
         */
        //         public int ID
        //         {
        //             get
        //             {
        //                 switch (name)
        //                 {
        //                     case "EU": return 10;
        //                     case "ED": return 11;
        //                     case "RU1": return 12;
        //                     case "RU2": return 13;
        //                     case "RU3": return 14;
        //                     case "RD1": return 15;
        //                     case "RD2": return 16;
        //                     case "RD3": return 17;
        //                     case "FU": return 18;
        //                     case "FD": return 19;
        //                     case "RU4": return 29;
        //                     case "EJ": return 35;
        //                     default: return 0;
        //                 }
        //             }
        //         }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", id, name, description);
        }
        public void PredefinedColor(out Color line, out Color fill)
        {
            line = Color.Black;
            fill = Color.White;
            switch (Name)
            {
                case "EU":
                    fill = Color.FromArgb(0xb4, 0x3d, 0x00);
                    line = Color.Black;
                    break;
                case "FU":
                case "FD":
                case "ED":
                    fill = Color.FromArgb(0xeb, 0xda, 0xc6);
                    line = Color.BlueViolet;
                    break;
                case "RU1":
                case "RD1":
                    fill = Color.FromArgb(0x6e, 0x6e, 0x6e);
                    line = Color.Black;
                    break;
                case "RU2":
                case "RD2":
                    fill = Color.FromArgb(0x9d, 0x9b, 0x9c);
                    line = Color.Black;
                    break;
                default:
                    fill = Color.FromArgb(0xff, 0xff, 0xcc);
                    line = Color.BlueViolet;
                    break;
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////
    public class PartitionDirectory
    {
        public PartitionDirectory() { }
        public PartitionDirectory(string fullpath) { name = fullpath; heights.Search(fullpath); }
        string name = "";

        public string FullPath { get { return name; } set { name = value; } }
        public string Name { get { return Path.GetFileName(name); } }
        public string Description { get { return Partition.Description; } }
        public string Display { get { return Name + " " + Description; } }
        ElevationFiles heights = new ElevationFiles();
        public List<ElevationFile> Heights { get { return heights.Files; } set { heights.Files = value; } }

        public Partition Partition { get {return DMControl.PartitionControl.NewPartition(Name); } }
    }
    public class PartitionDirectories
    {
        const string ROOT = @"C:\DAMDATA";
        const string ALT_ROOT = "DAMDATA";
        List<PartitionDirectory> dirs = new List<PartitionDirectory>();
        public List<PartitionDirectory> Directories { get { return dirs; } set { dirs = value; } }
        //         string rt = ROOT;
        //         public string Root { get { return rt; } set { rt = value; } }
        public PartitionDirectories()
        {
        }
        private bool ReadFromCache()
        {
            PartitionDirectories pds = Utils.Xml.XMLUtil<PartitionDirectories>.LoadXml("Cache\\Partitions.xml");
            if (pds != null)
                this.dirs = pds.Directories;
            else
                return false;
            return true;
        }
        private void SavePartitionsCache()
        {
            Directory.CreateDirectory("Cache");
            Utils.Xml.XMLUtil<PartitionDirectories>.SaveXml("Cache\\Partitions.xml", this);
        }
        public bool Search(string root)
        {
            DirectoryInfo di = new DirectoryInfo("Cache");
            if (di.Exists)
            {
                if (ReadFromCache())
                    return true;
            }

            if (root == null)
                root = ROOT;
            di = new DirectoryInfo(root);
            if (!di.Exists)
            {
                di = new DirectoryInfo(ALT_ROOT);
                if (!di.Exists)
                    return false;
            }

            DirectoryInfo[] dis = di.GetDirectories();
            if (dis == null)
                return false;
            if (dis.Length == 0)
                return false;

            foreach (DirectoryInfo d in dis)
            {
                dirs.Add(new PartitionDirectory(d.FullName));
            }

            SavePartitionsCache();
            return true;
        }
    }
}
