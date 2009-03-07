using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DM.DB
{
    public class Segment
    {
        Int32 segmentID;

        public Segment() { }
        public bool IsEqual(Segment s)
        {
            return IsEqual(s.BlockID, s.DesignZ, s.SegmentID);
        }
        public bool IsEqual(int partitionID, double elevation, int sID)
        {
            return partitionID == this.BlockID && elevation == this.DesignZ && sID == this.SegmentID;
        }

        public Int32 SegmentID
        {
            get { return segmentID; }
            set { segmentID = value; }
        }
        DB.SegmentWorkState workState;

        public DB.SegmentWorkState WorkState
        {
            get { return workState; }
            set { workState = value; }
        }
        Int32 blockID;

        public Int32 BlockID
        {
            get { return blockID; }
            set { blockID = value; }
        }
        string blockName;

        public string BlockName
        {
            get { return blockName; }
            set { blockName = value; }
        }


        Double designZ;

        public Double DesignZ
        {
            get { return designZ; }
            set { designZ = value; }
        }
        string vertext;

        public string Vertext
        {
            get { return vertext; }
            set { vertext = value; }
        }

        DateTime startDate;

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }
        DateTime endDate;

        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }
        Double maxSpeed;

        public Double MaxSpeed
        {
            get { return maxSpeed; }
            set { maxSpeed = value; }
        }
        Int32 designRollCount;

        public Int32 DesignRollCount
        {
            get { return designRollCount; }
            set { designRollCount = value; }
        }
        Double errorParam;

        public Double ErrorParam
        {
            get { return errorParam; }
            set { errorParam = value; }
        }
        Double spreadZ;

        public Double SpreadZ
        {
            get { return spreadZ; }
            set { spreadZ = value; }
        }
        Double designDepth;

        public Double DesignDepth
        {
            get { return designDepth; }
            set { designDepth = value; }
        }
        string remark;

        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }
        string segmentName;

        public string SegmentName
        {
            get { return segmentName; }
            set { segmentName = value; }
        }

        Double startZ;

        public Double StartZ
        {
            get { return startZ; }
            set { startZ = value; }
        }

        Double pop;  // percent of pass
		
		public Double POP
		{
			get{return pop;}
			set{pop = value;}
		}

        string notRollingstring;

        public string NotRollingstring
        {
            get { return notRollingstring; }
            set { notRollingstring = value; }
        }

        string commentNRstring;  

        public string CommentNRstring
        {
          get { return commentNRstring; }
          set { commentNRstring = value; }
        }

        public bool IsWorking()
        {
            return WorkState == SegmentWorkState.WORK;
        }
        public bool IsFinished()
        {
            return WorkState == SegmentWorkState.END;
        }
        //         public string getSegmentName()
        //         {
        //             return segmentName;
        //         }
        // 
        //         public void setSegmentName(string segmentName)
        //         {
        //             this.segmentName = segmentName;
        //         }
        // 
        //         public DateTime getDTEnd()
        //         {
        //             return endDate;
        //         }
        // 
        //         public DateTime getDTStart()
        //         {
        //             return startDate;
        //         }
        // 
        //         public Double getDesignZ()
        //         {
        //             return designZ;
        //         }
        //         public void setDesignZ(Double designZ)
        //         {
        //             this.designZ = designZ;
        //         }
        //         public Int32 getSegmentID()
        //         {
        //             return segmentID;
        //         }
        //         public void setSegmentID(Int32 segmentID)
        //         {
        //             this.segmentID = segmentID;
        //         }
        //         public Int32 getWorkState()
        //         {
        //             return workState;
        //         }
        //         public void setWorkState(Int32 workState)
        //         {
        //             this.workState = workState;
        //         }
        //         public Int32 getBlockID()
        //         {
        //             return blockID;
        //         }
        //         public void setBlockID(Int32 blockID)
        //         {
        //             this.blockID = blockID;
        //         }
        //         public Double getMaxSpeed()
        //         {
        //             return maxSpeed;
        //         }
        //         public void setMaxSpeed(Double maxSpeed)
        //         {
        //             this.maxSpeed = maxSpeed;
        //         }
        //         public Int32 getDesignRollCount()
        //         {
        //             return designRollCount;
        //         }
        //         public void setDesignRollCount(Int32 designRollCount)
        //         {
        //             this.designRollCount = designRollCount;
        //         }
        //         public Double getErrorParam()
        //         {
        //             return errorParam;
        //         }
        //         public void setErrorParam(Double errorParam)
        //         {
        //             this.errorParam = errorParam;
        //         }
        //         public Double getSpreadZ()
        //         {
        //             return spreadZ;
        //         }
        //         public void setSpreadZ(Double spreadZ)
        //         {
        //             this.spreadZ = spreadZ;
        //         }
        //         public Double getDesignDepth()
        //         {
        //             return designDepth;
        //         }
        //         public void setDesignDepth(Double designDepth)
        //         {
        //             this.designDepth = designDepth;
        //         }
        // 
        //         public DateTime getStartDate()
        //         {
        //             return startDate;
        //         }
        // 
        //         public void setStartDate(DateTime startDate)
        //         {
        //             this.startDate = startDate;
        //         }
        // 
        //         public DateTime getEndDate()
        //         {
        //             return endDate;
        //         }
        // 
        //         public void setEndDate(DateTime endDate)
        //         {
        //             this.endDate = endDate;
        //         }
        //         
        //         public string getRemark()
        //         {
        //             return remark;
        //         }
        // 
        //         public void setRemark(string remark)
        //         {
        //             this.remark = remark;
        //         }
    }
}
