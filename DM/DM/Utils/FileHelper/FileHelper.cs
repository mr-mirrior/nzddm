using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DM.Utils.FileHelper
{
    public static class FileHelper
    {
        public static int[] ReaderLibratedIDS()
        {
            int[] carIDs;
            string str= DM.Models.Config.I.SHOWLIBRATED_CARIDS;
            string[] strIDs = str.Trim().Split(',');

            carIDs = new int[strIDs.Length];
            try
            {
                for (int i = 0; i < carIDs.Length; i++)
                {
                    carIDs[i] = Convert.ToInt32(strIDs[i]);
                }
            }
            catch (System.Exception e)
            {
                Utils.MB.Error("读取配置文件车辆击震力信息出错！");
            }
           

            return carIDs;
        }
    }
}
