using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SISPRA.DAO
{
    public class MasterDAO
    {
        public List<dynamic> getAllMenu(String role)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT *
                        ";
                    
                    var data = conn.Query<dynamic>(query).ToList();

                    return data;
                }
                catch (Exception ex)
                {
                    return new List<dynamic>();
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public List<dynamic> getAllUnit()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT *
                        FROM siatmax.MST_UNIT";
                    var data = conn.Query<dynamic>(query).ToList();

                    return data;
                }
                catch (Exception ex)
                {
                    return new List<dynamic>();
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public List<dynamic> getAllTahunAnggaran()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT *
                        FROM sikeu.TBL_TAHUN_ANGGARAN";
                    var data = conn.Query<dynamic>(query).ToList();

                    return data;
                }
                catch (Exception ex)
                {
                    return new List<dynamic>();
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }
    }
}
