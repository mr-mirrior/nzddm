using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DM.DB
{
    public class CarDistribute
    {
        Int32 carid;

        public Int32 Carid
        {
            get { return carid; }
            set { carid = value; }
        }
        Int32 blockid;

        public Int32 Blockid
        {
            get { return blockid; }
            set { blockid = value; }
        }
        Int32 segmentid;

        public Int32 Segmentid
        {
            get { return segmentid; }
            set { segmentid = value; }
        }
        DateTime dTStart;

        public DateTime DTStart
        {
            get { return dTStart; }
            set { dTStart = value; }
        }
        Double designZ;

        public Double DesignZ
        {
            get { return designZ; }
            set { designZ = value; }
        }
        DateTime dTEnd;

        public DateTime DTEnd
        {
            get { return dTEnd; }
            set { dTEnd = value; }
        }

        CarDistribute_Status status;

        public CarDistribute_Status Status
        {
            get { return status; }
            set { status = value; }
        }

        public bool IsFinished()
        {
            return DTEnd != DateTime.MinValue && DTStart!=DateTime.MinValue;
        }
        public bool IsWorking()
        {
            return DTEnd == DateTime.MinValue && DTStart != DateTime.MinValue;
        }
        public bool IsAssigned()
        {
            return DTEnd == DateTime.MinValue && DTStart == DateTime.MinValue;
        }
//         public Double getDesignZ()
//         {
//             return designZ;
//         }
//         public void setDesignZ(Double designZ)
//         {
//             this.designZ = designZ;
//         }
//         public Int32 getCarid()
//         {
//             return carid;
//         }
//         public void setCarid(Int32 carid)
//         {
//             this.carid = carid;
//         }
//         public Int32 getBlockid()
//         {
//             return blockid;
//         }
//         public void setBlockid(Int32 blockid)
//         {
//             this.blockid = blockid;
//         }
//         public Int32 getSegmentid()
//         {
//             return segmentid;
//         }
//         public void setSegmentid(Int32 segmentid)
//         {
//             this.segmentid = segmentid;
//         }
	
    }
}
