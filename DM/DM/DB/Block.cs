using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DM.DB
{
    public class Block
    {
        private int blockID;

        public override string ToString()
        {
            return string.Format("ID={0}, Code={2}, Name={1}", BlockID, BlockName, BlockCode);
        }
        public Block(){}
        public Block(int id, string code, string name) { BlockID = id; BlockCode = code; BlockName = name; }
        public int BlockID
        {
            get { return blockID; }
            set { blockID = value; }
        }
        private string blockCode;

        public string BlockCode
        {
            get { return blockCode; }
            set { blockCode = value; }
        }
        private string blockName;

        public string BlockName
        {
            get { return blockName; }
            set { blockName = value; }
        }
        
//         public string getBlockID()
//         {
//             return blockID;
//         }
//         public void setBlockID(string blockID)
//         {
//             this.blockID = blockID;
//         }
//         public string getBlockCode()
//         {
//             return blockCode;
//         }
//         public void setBlockCode(string blockCode)
//         {
//             this.blockCode = blockCode;
//         }
//         public string getBlockName()
//         {
//             return blockName;
//         }
//         public void setBlockName(string blockName)
//         {
//             this.blockName = blockName;
//         }
    }
}
