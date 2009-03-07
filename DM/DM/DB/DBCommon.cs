using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
namespace DM.DB
{
    class DBCommon
    {
        //返回数据库当前时间
        public static DateTime getDate(){
            DateTime thisDate = DateTime.MinValue;
            SqlConnection connection = null;
            SqlDataReader reader = null;
            string sqlTxt = "select getDate() as thisDate";
            try
            {
                connection = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(connection, sqlTxt);
                if(reader.Read())
                {
                    thisDate = Convert.ToDateTime(reader["thisDate"]);
                }

            }
            catch (Exception exp)
            {
                DebugUtil.log(exp);                
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(connection);
            }

            return thisDate;
        }

    }
}
