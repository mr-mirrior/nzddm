using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace DM.DB
{
    class BlockDAO
    {
        public List<Block> getBlocks(){
            SqlConnection conn = null;
            SqlDataReader reader = null;
            try
            {
                conn = DBConnection.getSqlConnection();
                reader = DBConnection.executeQuery(conn, "select * from block");
                List<Block> blocks = new List<Block>();
                while (reader.Read())
                {
                    Block block = new Block();
                    block.BlockCode = (reader["blockcode"].ToString());
                    block.BlockID = ((int)reader["blockid"]);
                    block.BlockName = ((reader["blockname"].ToString()));
                    blocks.Add(block);
                }
                return blocks;
            }
            catch (System.Exception e)
            {
                DebugUtil.log(e);
                return null;
            }
            finally
            {
                DBConnection.closeDataReader(reader);
                DBConnection.closeSqlConnection(conn);            
            }                        
        }
    }
}
