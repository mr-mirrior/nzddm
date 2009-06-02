using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DM.DB;
using DM.Models;

namespace DM.DMControl
{
    public static class PartitionControl
    {
        static Dictionary<string, Partition> keyName;
        static Dictionary<int, Partition> keyID;

        /*
        10	EU	心墙上部
        11	ED	心墙下部
        12	RU1	上游粗堆石Ⅰ区

        13	RU2	上游粗堆石Ⅱ区

        14	RU3	上游细堆石区
        15	RD1	下游粗堆石Ⅰ区

        16	RD2	下游粗堆石Ⅱ区

        17	RD3	下游细堆石区
        18	FU	上游反滤区

        19	FD	下游反滤区

        29	RU4	上游粗堆石调节区
        35	EJ	接触粘土区

        31	test1	碾压测试区

        32	LeftTop	左岸坝顶试验区

        36 yingdi 业主营地门前
        37 sywy 上游围堰
         */
        public static Partition NewPartition(string name) { return FromName(name); }
        private static Partition Translate(Block blk)
        {
            return new Partition(blk.BlockID, blk.BlockCode, blk.BlockName);
        }
        public static void Init()
        {
            BlockDAO dao = new BlockDAO();
            List<Block> partitions = dao.getBlocks();

            if( partitions == null )
            {
                partitions = new List<Block>();
                partitions.Add(new Block(10, "EU", "心墙上部"));
                partitions.Add(new Block(11, "ED", "心墙下部"));
                partitions.Add(new Block(12, "RU1", "上游粗堆石Ⅰ区"));
                partitions.Add(new Block(13, "RU2", "上游粗堆石Ⅱ区"));
                partitions.Add(new Block(14, "RU3", "上游细堆石区"));
                partitions.Add(new Block(15, "RD1", "下游粗堆石Ⅰ区"));
                partitions.Add(new Block(16, "RD2", "下游粗堆石Ⅱ区"));
                partitions.Add(new Block(17, "RD3", "下游细堆石区"));
                partitions.Add(new Block(18, "FU", "上游反滤区"));
                partitions.Add(new Block(19, "FD", "下游反滤区"));
                partitions.Add(new Block(29, "RU4", "上游粗堆石调节区"));
                //partitions.Add(new Block(35, "EJ", "接触粘土区"));
                //partitions.Add(new Block(32, "LeftTop", "左岸坝顶试验区")); 
                //partitions.Add(new Block(36, "yingdi", "业主营地门前")); 
                //partitions.Add(new Block(37, "sywy", "上游围堰"));
            }

            keyID = new Dictionary<int, Partition>();
            keyName = new Dictionary<string, Partition>();
            foreach (DB.Block blk in partitions)
            {
                keyName.Add(blk.BlockCode, Translate(blk));
                keyID.Add(blk.BlockID, Translate(blk));
            }

        }
        // name = "EU"/"FD/"RU3"...
        public static Partition FromID(int i)
        {
            try
            {
                return keyID[i];
            }
            catch
            {
                return default(Partition);
            }
        }
        // ID=10/12...
        public static Partition FromName(string n)
        {
            try
            {
                return keyName[n];
            }
            catch
            {
                return default(Partition);
            }
        }
    }
}
