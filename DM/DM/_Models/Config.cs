﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace DM.Models
{
    public class Config
    {
        #region - 初始化 -
        public const string CONFIG_FILE = "Config.xml";
        public static Config I=new Config();
        public static void Save() { DM.Utils.Xml.XMLUtil<Config>.SaveXml(CONFIG_FILE, I); }
        #endregion

        public string WARNING = "该配置为专业人员设置，如果你不知道这些值的含义，请不要修改！否则可能会对系统造成灾难性的影响！";
        // 基础筛选
        public double BASE_FILTER_THRES = 0.2; // m, 相邻点距离
        public double BASE_FILTER_METERS = 5; // m, 相邻点距离
        public double BASE_FILTER_SECONDS = 3;  // sec, 相邻点时间间隔
        public string BASE_STATUS = "0";// 状态种类
        public int LIBRATE_Secends = 3000;//多少毫秒之内的算做连续振动不合格
        // 高程筛选
        public double ELEV_FILTER_SPEED = 1.5; // %
        public double ELEV_FILTER_ELEV_LOWER = 0.5; // %
        public double ELEV_FILTER_ELVE_UPPER = 1.5; // %
        public double ELEV_FILTER_SECONDS = 5; // sec

        public int REFRESH_TIME = 500; // milliseconds
        public int OVERTHICKNESS_DISTANCE = 1; // 米

        public bool IS_OVERSPEED_VALID =true;
        public bool IS_LIBRATE_VALID = true;//振动不合格是否记录遍数

        public bool GEN_NOLIBRATE_VALID = false;//是否可用 常规振动 选项

        //public byte[] bbbbbbbbb = File.ReadAllBytes(@"C:\Users\Administrator\Pictures\1440.jpg");
    }
}
