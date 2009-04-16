using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace DM
{
    public delegate void InvokeDelegate();
    static class Program
    {
        static volatile bool exiting = false;
        public static bool ISTJU = false;//是否天津大学专用

        public static bool Exiting
        {
            get { return Program.exiting; }
            set { Program.exiting = value; DMControl.GPSServer.Stop(); }
        }

        static void SetWorkingDirectory()
        {
            // exe文件所在目录
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));
        }

        static void Init()
        {
//             Utils.MB.OKI("DM.DB.DBconfig.getInstance().init();");
//             Utils.MB.OKI("DMControl.PartitionControl.Init();");
            DMControl.PartitionControl.Init();
//             Utils.MB.OKI("DMControl.VehicleControl.ReadVehicleInfo();");
            DMControl.VehicleControl.ReadVehicleInfo();
//             Utils.MB.OKI("DMControl.VehicleControl.LoadCarDistribute();");
            DMControl.VehicleControl.LoadCarDistribute();
            DMControl.LayerControl.Instance.LoadWorkingLayer();
//             Utils.MB.OKI("DMControl.WarningControl.Init();");
            DMControl.WarningControl.Init();
            DM.DB.DBconfig.getInstance();
            dlg.Finished = true;
            // 开始接受GPS线程
            DMControl.GPSServer.StartReceiveGPSData(DM.DB.DBconfig.getInstance().Damserver, 6666);
            System.Diagnostics.Debug.Print("Init finished");
            if (!System.IO.File.Exists(Models.Config.CONFIG_FILE))
                Models.Config.Save();
            Models.Config.I = DM.Utils.Xml.XMLUtil<Models.Config>.LoadXml(Models.Config.CONFIG_FILE);
        }

        /// <summary>
        /// 应用程序的主入口点。
        static Forms.Waiting dlg = null;
        /// </summary>
        [STAThread]
        static void Main()
        {
//             Console.WriteLine(DM.DB.UserDAO.getInstance().login("123sa", "123456123"));
//             return;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Forms.CoordsInput ci = new DM.Forms.CoordsInput();
            //ci.ShowDialog();
            //return;

            SetWorkingDirectory();

            if (!DM.DB.DBconfig.getInstance().init())
            {
                Utils.MB.Error("数据库配置无法读取，请检查db.config文件！");
                return;
            }


#if !DEBUG
            dlg = new DM.Forms.Waiting();
            dlg.Start(null, "请稍候，正在读取数据库……", Init, 2000);

            Forms.DMLogin ldlg = new DM.Forms.DMLogin();
            ldlg.ShowDialog();
            if (ldlg.DialogResult==DialogResult.OK)
            {
                Utils.MB.OKI("登录成功！");

                Application.Run(new Forms.Main());
            }
#else

            dlg = new DM.Forms.Waiting();
            dlg.Start(null, "请稍候，正在读取数据库……", Init, 2000);
#endif


//             DM.Forms.OverBlock ob = new DM.Forms.OverBlock();
//             List<DM.DB.Segment> lst = DM.DB.SegmentDAO.getInstance().findBeneathSegments(13, 586.5);
//             ob.PartitionList = lst;
//             ob.DeckInfo = "测试";
//             ob.BlockName = "测试";
//             if (ob.ShowDialog() == DialogResult.OK)
//             {
//                 DM.Models.Deck dk = new DM.Models.Deck();
//                 dk.DeckInfo.DesignZ = ob.Elevation;
//                 dk.DeckInfo.BlockID = 13;
//                 dk.ID = ob.DeckID;
//                 double avr = dk.AverageHeightFromDataMap();
//             }
//             return;

#if DEBUG
             Application.Run(new Forms.Main());
#endif
           
        }
    }
}
