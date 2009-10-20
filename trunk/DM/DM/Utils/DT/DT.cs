using System;
using System.Globalization;

namespace DM.Utils
{
    public static class DT
    {
        public static readonly TimeSpan OneHour = new TimeSpan(1, 0, 0);
        public static readonly TimeSpan OneDay = new TimeSpan(1, 0, 0, 0);
        public static readonly TimeSpan OneWeek = new TimeSpan(7, 0, 0, 0);
        public static readonly TimeSpan OneTick = new TimeSpan(0, 0, 0, 0, 1);
        public static bool IsValid(DateTime t)
        {
            if (t.Year == DateTime.MinValue.Year ||
                t.Year == DateTime.MaxValue.Year)
                return false;

            return true;
        }
        public static bool IsStrictlyValid(DateTime t)
        {
            if (!IsValid(t))
                return false;
            if (t.Year < 1980 /*No computer yet*/ ||
                t.Year > 4999 /*Can I live old enough to see this?*/)
                return false;
            return true;
        }
        public static TimeSpan Days(int days)
        {
            return new TimeSpan(days, 0, 0, 0);
        }
        public static TimeSpan Hours(int hours)
        {
            return new TimeSpan(hours, 0, 0);
        }
        public static int WeekDistance(DateTime d1, DateTime d2)
        {
            if (d1 > d2) { DateTime tmp = d1; d1 = d2; d2 = tmp; }
            double days = (d2 - d1).TotalDays;
            int week = (int)Math.Floor(days / 7);
            if (d1.DayOfWeek > d2.DayOfWeek)
                week++;
            return week;
        }
        const int WEEKS_IN_YEAR = 53;
        public static int NormalWeek(int week)
        {
            // 1 based week
            while (week <= 0)
                week += WEEKS_IN_YEAR;
            if (week <= WEEKS_IN_YEAR)
                return week;
            week %= WEEKS_IN_YEAR;
            return week;
        }
        public static int WhichWeekOfYear(DateTime t)
        {
            int weekday = (int)t.DayOfWeek;
            DateTime firstday = new DateTime(t.Year, 1, 1);
            int firstweek = 1;
            if (firstday.DayOfWeek != DayOfWeek.Sunday)
                firstweek = 53;
            int weeks = (t.DayOfYear - firstday.DayOfYear) / 7;
            return NormalWeek(firstweek + weeks);
        }

        public static int NormalMonth(int month)
        {
            while (month <= 0)
                month += 12;
            if (month <= 12)
                return month;
            month %= 12;
            return month;
        }
        public static DateTime LastMonth(DateTime m)
        {
            int year = m.Year;
            int month = m.Month;
            month--;
            if (month == 0)
            {
                year--;
                month = 12;
            }
            return new DateTime(year, month, 1,
                m.Hour,
                m.Minute,
                m.Second,
                m.Kind);
        }
        // 00:00:00.000
        public static DateTime DateFloor(DateTime d)
        {
            return new DateTime(d.Year, d.Month, d.Day, 0, 0, 0, d.Kind);
        }
        // 23:59:59.999
        public static DateTime DateCeiling(DateTime d)
        {
            if (d == DateTime.MaxValue)
                return DateTime.MaxValue;
            //             DateTime max = DateTime.MaxValue;
            //             DateTime x = new DateTime(d.Year, d.Month, d.Day,
            //                 max.Hour,
            //                 max.Minute,
            //                 max.Second
            //                 );
            DateTime x = DateFloor(d + OneDay);
            x -= OneTick;
            return x;
        }
        public static DateTime NextDay(DateTime t)
        {
            return t + OneDay;
        }
        public static DateTime NextDayFloor(DateTime t)
        {
            return DateFloor(t) + OneDay;
        }
        public static DateTime HourFloor(DateTime h)
        {
            return new DateTime(h.Year, h.Month, h.Day, h.Hour, 0, 0, h.Kind);
        }
        public static DateTime HourCeiling(DateTime h)
        {
            if (h == DateTime.MaxValue)
                return DateTime.MaxValue;

            h += new TimeSpan(1, 0, 0);
            return HourFloor(h);
        }

        public static DateTime NextMonth(DateTime d)
        {
            int year = d.Year;
            int month = d.Month;
            month++;
            if (month == 13)
            {
                month = 1;
                year++;
            }
            return new DateTime(year, month, 1,
                d.Hour,
                d.Minute,
                d.Second,
                d.Kind);
        }
        public static DateTime PrevMonth(DateTime d)
        {
            if (d == DateTime.MinValue)
                return DateTime.MinValue;
            int year = d.Year;
            int month = d.Month;
            month--;
            if (month == 0)
            {
                month = 12;
                year--;
            }
            return new DateTime(year, month, 1,
                d.Hour,
                d.Minute,
                d.Second,
                d.Kind);
        }
        public static int DaysInMonth(DateTime d)
        {
            return DateTime.DaysInMonth(d.Year, d.Month);
        }
        public static DateTime MonthFloor(DateTime d)
        {
            return new DateTime(d.Year, d.Month, 1,
                0,
                0,
                0,
                d.Kind);
        }
        public static DateTime MonthCeiling(DateTime d)
        {
            int days = DaysInMonth(d);
            DateTime max = DateTime.MaxValue;
            DateTime dt = new DateTime(d.Year, d.Month, days,
                max.Hour,
                max.Minute,
                max.Second,
                max.Millisecond,
                d.Kind
                );
            return dt;
        }
        public static bool SameMonth(DateTime d1, DateTime d2)
        {
            return d1.Year == d2.Year && d1.Month == d2.Month;
        }
        public static bool SameDay(DateTime d1, DateTime d2)
        {
            if (!SameMonth(d1, d2))
                return false;
            return d1.Day == d2.Day;
        }
        public static bool SameWeek(DateTime d1, DateTime d2)
        {
            if (d1.Year == d2.Year)
            {
                int week1 = WhichWeekOfYear(d1);
                int week2 = WhichWeekOfYear(d2);
                return week1 == week2;
            }
            return false;
        }
        public static DateTime WeekFloor(DateTime d)
        {
            if (d == DateTime.MinValue)
                return DateTime.MinValue;
            d = DateFloor(d);
            int day = (int)d.DayOfWeek;
            return d - new TimeSpan(day, 0, 0, 0);
        }
        public static DateTime NextWeek(DateTime d)
        {
            if (d == DateTime.MaxValue)
                return DateTime.MaxValue;

            return d + new TimeSpan(7, 0, 0, 0);
        }
        public static int MonthDistance(DateTime d1, DateTime d2)
        {
            if (d1 > d2)
            {
                DateTime d3 = d1;
                d1 = d2;
                d2 = d3;
            }

            int count = 0;
            count = (d2.Year - d1.Year) * 12;
            count += (d2.Month - d1.Month);
            return count;
        }
        public static DateTime BeforeMonths(DateTime t, int m)
        {
            if (!IsStrictlyValid(t))
                return DateTime.MinValue;
            for (int i = 0; i < m; i++)
            {
                t = PrevMonth(t);
            }
            return t;
        }
        public static DateTime AfterMonths(DateTime t, int m)
        {
            if (!IsStrictlyValid(t))
                return DateTime.MaxValue;
            for (int i = 0; i < m; i++)
            {
                t = NextMonth(t);
            }
            return t;
        }
    }
}