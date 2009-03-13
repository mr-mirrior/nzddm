using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DM.DB
{
    public class LibrateInfo
    {
        int carID;

        public int CarID
        {
            get { return carID; }
            set { carID = value; }
        }

       int state;

       public int State
        {
            get { return state; }
            set { state = value; }
        }

       DateTime dt;

       public DateTime Dt
       {
           get { return dt; }
           set { dt = value; }
       }
    }
}
