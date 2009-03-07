using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DM.DB
{
    public enum Compare_Type{
        YEAR,
        MONTH,
        DAY,
        HOUR,
        MINUTE,
        SECOND
    }
    class DateUtil
    {
        //得到间隔的compareType数
        public static Int32 dateDiff(Compare_Type compareType, System.DateTime dtstart, System.DateTime dtend)
        {
            int diff = 0;


            switch (compareType)
            {
                case Compare_Type.YEAR:
                    dtstart = new DateTime(dtstart.Year, 1, 1);
                    dtend = new DateTime(dtend.Year, 1, 1);
                    break;
                case Compare_Type.MONTH:
                    dtstart = new DateTime(dtstart.Year, dtstart.Month, 1);
                    dtend = new DateTime(dtend.Year, dtend.Month, 1);
                    break;
                case Compare_Type.DAY:
                    dtstart = new DateTime(dtstart.Year, dtstart.Month, 1);
                    dtend = new DateTime(dtend.Year, dtend.Month, 1);
                    break;
                case Compare_Type.HOUR:
                    dtstart = new DateTime(dtstart.Year, dtstart.Month,dtstart.Day,dtstart.Hour,0,0);
                    dtend = new DateTime(dtend.Year, dtend.Month, dtend.Day,dtend.Hour,0,0);
                    break;
                case Compare_Type.MINUTE:
                    dtstart = new DateTime(dtstart.Year, dtstart.Month, dtstart.Day, dtstart.Hour, dtstart.Minute, 0);
                    dtend = new DateTime(dtend.Year, dtend.Month, dtend.Day,dtend.Hour, dtend.Minute, 0);
                    break;              
            }

            System.TimeSpan TS = new System.TimeSpan(dtend.Ticks - dtstart.Ticks);
            switch (compareType)
            {
                case Compare_Type.YEAR:
                    diff = Convert.ToInt32(TS.TotalDays / 365);
                    break;
                case Compare_Type.MONTH:
                    diff = Convert.ToInt32((TS.TotalDays / 365) * 12);
                    break;
                case Compare_Type.DAY:
                    diff = Convert.ToInt32(TS.TotalDays);
                    break;
                case Compare_Type.HOUR:
                    diff = Convert.ToInt32(TS.TotalHours);
                    break;
                case Compare_Type.MINUTE:
                    diff = Convert.ToInt32(TS.TotalMinutes);
                    break;
                case Compare_Type.SECOND:
                    diff = Convert.ToInt32(TS.TotalSeconds);
                    break;
            }
            return diff;
        }
           
        //将两个时间按照Compare_Type分隔,返回DateTime数组
        public static DateTime[] getDateTimes(Compare_Type compareType,DateTime dtstart,DateTime dtend){

            
            Int32 monthNumber = DateUtil.dateDiff(Compare_Type.MONTH, dtstart, dtend);
            DateTime[] datetimes = new DateTime[monthNumber + 1];
            for (int i = 0; i <= monthNumber; i++)
            {
                datetimes[i] = dtstart;
                switch (compareType)
                {
                    case Compare_Type.YEAR:
                        dtstart = dtstart.AddYears(1);
                        break;
                    case Compare_Type.MONTH:
                        dtstart = dtstart.AddMonths(1);
                        break;
                    case Compare_Type.DAY:
                        dtstart = dtstart.AddDays(1);
                        break;
                    case Compare_Type.HOUR:
                        dtstart = dtstart.AddHours(1);
                        break;
                    case Compare_Type.MINUTE:
                        dtstart = dtstart.AddMinutes(1);
                        break;
                    case Compare_Type.SECOND:
                        dtstart = dtstart.AddSeconds(1);
                        break;
                }
                //Console.WriteLine(dtstart.ToString());
            }
            datetimes[monthNumber] = dtend;
            return datetimes;
        }
    }
}
