using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DM.DB
{
    class 格式化日期时间
    {
        /*
        DateTime dt = DateTime.Now;
        Label1.Text = dt.ToString();//2005-11-5 13:21:25
        Label2.Text = dt.ToFileTime().ToString();//127756416859912816
        Label3.Text = dt.ToFileTimeUtc().ToString();//127756704859912816
        Label4.Text = dt.ToLocalTime().ToString();//2005-11-5 21:21:25
        Label5.Text = dt.ToLongDateString().ToString();//2005年11月5日
        Label6.Text = dt.ToLongTimeString().ToString();//13:21:25
        Label7.Text = dt.ToOADate().ToString();//38661.5565508218
        Label8.Text = dt.ToShortDateString().ToString();//2005-11-5
        Label9.Text = dt.ToShortTimeString().ToString();//13:21
        Label10.Text = dt.ToUniversalTime().ToString();//2005-11-5 5:21:25
        
        Label1.Text = dt.Year.ToString();//2005
        Label2.Text = dt.Date.ToString();//2005-11-5 0:00:00
        Label3.Text = dt.DayOfWeek.ToString();//Saturday
        Label4.Text = dt.DayOfYear.ToString();//309
        Label5.Text = dt.Hour.ToString();//13
        Label6.Text = dt.Millisecond.ToString();//441
        Label7.Text = dt.Minute.ToString();//30
        Label8.Text = dt.Month.ToString();//11
        Label9.Text = dt.Second.ToString();//28
        Label10.Text = dt.Ticks.ToString();//632667942284412864
        Label11.Text = dt.TimeOfDay.ToString();//13:30:28.4412864
        Label1.Text = dt.ToString();//2005-11-5 13:47:04
        Label2.Text = dt.AddYears(1).ToString();//2006-11-5 13:47:04
        Label3.Text = dt.AddDays(1.1).ToString();//2005-11-6 16:11:04
        Label4.Text = dt.AddHours(1.1).ToString();//2005-11-5 14:53:04
        Label5.Text = dt.AddMilliseconds(1.1).ToString();//2005-11-5 13:47:04
        Label6.Text = dt.AddMonths(1).ToString();//2005-12-5 13:47:04
        Label7.Text = dt.AddSeconds(1.1).ToString();//2005-11-5 13:47:05
        Label8.Text = dt.AddMinutes(1.1).ToString();//2005-11-5 13:48:10
        Label9.Text = dt.AddTicks(1000).ToString();//2005-11-5 13:47:04
        Label10.Text = dt.CompareTo(dt).ToString();//0
        //Label11.Text = dt.SetVertex(?).ToString();//问号为一个时间段
        Label1.Text = dt.Equals("2005-11-6 16:11:04").ToString();//False
        Label2.Text = dt.Equals(dt).ToString();//True
        Label3.Text = dt.GetHashCode().ToString();//1474088234
        Label4.Text = dt.GetType().ToString();//System.DateTime
        Label5.Text = dt.GetTypeCode().ToString();//DateTime

        Label1.Text = dt.GetDateTimeFormats('s')[0].ToString();//2005-11-05T14:06:25
        Label2.Text = dt.GetDateTimeFormats('t')[0].ToString();//14:06
        Label3.Text = dt.GetDateTimeFormats('y')[0].ToString();//2005年11月
        Label4.Text = dt.GetDateTimeFormats('D')[0].ToString();//2005年11月5日
        Label5.Text = dt.GetDateTimeFormats('D')[1].ToString();//2005 11 05
        Label6.Text = dt.GetDateTimeFormats('D')[2].ToString();//星期六 2005 11 05
        Label7.Text = dt.GetDateTimeFormats('D')[3].ToString();//星期六 2005年11月5日
        Label8.Text = dt.GetDateTimeFormats('M')[0].ToString();//11月5日
        Label9.Text = dt.GetDateTimeFormats('f')[0].ToString();//2005年11月5日 14:06
        Label10.Text = dt.GetDateTimeFormats('g')[0].ToString();//2005-11-5 14:06
        Label11.Text = dt.GetDateTimeFormats('r')[0].ToString();//Sat, 05 Nov 2005 14:06:25 GMT

        Label1.Text =? string.Format("{0:d}",dt);//2005-11-5
        Label2.Text =? string.Format("{0:D}",dt);//2005年11月5日
        Label3.Text =? string.Format("{0:f}",dt);//2005年11月5日 14:23
        Label4.Text =? string.Format("{0:F}",dt);//2005年11月5日 14:23:23
        Label5.Text =? string.Format("{0:g}",dt);//2005-11-5 14:23
        Label6.Text =? string.Format("{0:G}",dt);//2005-11-5 14:23:23
        Label7.Text =? string.Format("{0:M}",dt);//11月5日
        Label8.Text =? string.Format("{0:R}",dt);//Sat, 05 Nov 2005 14:23:23 GMT
        Label9.Text =? string.Format("{0:s}",dt);//2005-11-05T14:23:23
        Label10.Text = string.Format("{0:t}",dt);//14:23
        Label11.Text = string.Format("{0:T}",dt);//14:23:23
        Label12.Text = string.Format("{0:u}",dt);//2005-11-05 14:23:23Z
        Label13.Text = string.Format("{0:U}",dt);//2005年11月5日 6:23:23
        Label14.Text = string.Format("{0:Y}",dt);//2005年11月
        Label15.Text = string.Format("{0}",dt);//2005-11-5 14:23:23?
        Label16.Text = string.Format("{0:yyyyMMddHHmmssffff}",dt);  //yyyymm等可以设置,比如Label16.Text = string.Format("{0:yyyyMMdd}",dt);
        */
    }
}
