using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace DM.DB
{   
   
    public enum LoginResult
    {
        INVALID_USER = -2,
        INVALID_PASSWORD,
    /*{"浏览","操作","管理员","不报警"};*/
        VIEW,OPERATOR,ADMIN,DISWARNING,
        ERROR
    }

    public class UserDAO
    {

        private static UserDAO myInstance = null;

        public static UserDAO getInstance()
        {
            if (myInstance == null)
            {
                myInstance = new UserDAO();
            }
            return myInstance;
        }

        public LoginResult login(string username, string password)
        {
            User user = null;
            
            try
            {
              user = getUser(username);
            }catch(System.Exception e){
                DebugUtil.log(e);
                return LoginResult.ERROR;
            }

            if (user == null)
            {
                return LoginResult.INVALID_USER;
            }
            else if (!password.Equals(user.getUserPassword()))
            {
                return LoginResult.INVALID_PASSWORD;
            }
            else
            {
                return (LoginResult)user.getUserClass();
            }
        }

        public User getUser(string username){
            try{
                User user = new User();
                SqlConnection conn = DBConnection.getSqlConnection();
                SqlDataReader reader = DBConnection.executeQuery(conn, "select userpassword,userclass from userlist where loginname='"+username+"'");               
                if (reader.Read()){
                    user.setUserPassword(reader["userpassword"].ToString());
                    user.setUserClass(reader["userclass"].ToString());
                }else{
                    return null;
                }
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);
                return user;
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
                throw e;
            }
        }


    }
}
