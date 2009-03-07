using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DM.DB
{
    public class User
    {
        string userID;
        string loginName;
        string userPassword;
        string userClass;
        string userName;

        public string getUserID()
        {
            return userID;
        }
        public void setUserID(string userID)
        {
            this.userID = userID;
        }
        public string getLoginName()
        {
            return loginName;
        }
        public void setLoginName(string loginName)
        {
            this.loginName = loginName;
        }
        public string getUserPassword()
        {
            return userPassword;
        }
        public void setUserPassword(string userPassword)
        {
            this.userPassword = userPassword;
        }
        public LoginResult getUserClass()
        {
            if (this.userClass.Equals("浏览"))
            {
                return LoginResult.VIEW;
            }
            else if (this.userClass.Equals("操作"))
            {
                return LoginResult.OPERATOR;
            }
            else if (this.userClass.Equals("管理员"))
            {
                return LoginResult.ADMIN;
            }
            else if (this.userClass.Equals("不报警"))
            {
                return LoginResult.DISWARNING;
            }
            else
            {
                return LoginResult.ERROR;
            }

        }
        public void setUserClass(string userClass)
        {
            this.userClass = userClass;
        }
        public string getUserName()
        {
            return userName;
        }
        public void setUserName(string userName)
        {
            this.userName = userName;
        }
    }
}
