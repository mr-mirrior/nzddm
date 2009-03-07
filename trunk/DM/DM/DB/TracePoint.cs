using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DM.DB
{
    class TracePoint
    {
        private Int32 carid;

        public Int32 Carid
        {
            get { return carid; }
            set { carid = value; }
        }

        private Double x;

        public Double X
        {
            get { return x; }
            set { x = value; }
        }

        private Double y;

        public Double Y
        {
            get { return y; }
            set { y = value; }
        }

        private Double z;

        public Double Z
        {
            get { return z; }
            set { z = value; }
        }

        private Int32 v;

        public Int32 V
        {
            get { return v; }
            set { v = value; }
        }

        private DateTime dttrace;

        public DateTime Dttrace
        {
            get { return dttrace; }
            set { dttrace = value; }
        }        
        
    }
    
}
