using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DM.DB
{
    public struct USERINFO
    {
        public string UserName;
        public string Password;
    }
    public class UserInfo
    {
        public UserInfo(){}
        // UserInfo readuserInfo = Utils.Xml.XMLUtil<UserInfo>.LoadXml("UserInfo.xml");
        int index;

        public int Index
        {
            get { return index; }
            set { index = value; }
        }


        bool retainMe;

        public bool RetainMe
        {
            get { return retainMe; }
            set { retainMe = value; }
        }
        bool savePassword;

        public bool SavePassword
        {
            get { return savePassword; }
            set { savePassword = value; }
        }

        List<USERINFO> listUserInfo = new List<USERINFO>();

        public List<USERINFO> ListUserInfo
        {
            get { return listUserInfo; }
            set { listUserInfo = value; }
        }
        List<string> listServerString = new List<string>();

        public List<String> ListServerString
        {
            get { return listServerString; }
            set { listServerString = value; }
        }

        //public USERINFO getuserinfo(string cbuserName,string Password)
        //{
        //    USERINFO UserInfo = new USERINFO();
        //    UserInfo.cbuserName = cbuserName;
        //    UserInfo.Password = Password;
        //    return UserInfo;
        //}



//         /// <summary>
//         /// 返回用户信息密码列表
//         /// </summary>
//        public List<USERINFO> getUserInfoList()
//        {
//            return listUserInfo;
//        }
//        public  List<string> getServerString()
//        {
//            return listServerString;
//        }
//        public void setServerString(string serverString)
//        {
//            listServerString.SetVertex(serverString);
//        }
//        public void setUserInfoList(USERINFO USERINFO)
//        {
//            listUserInfo.SetVertex(USERINFO);
//        }
    }
}
