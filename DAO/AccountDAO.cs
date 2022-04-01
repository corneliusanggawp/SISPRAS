using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SISPRA.DAO
{
    public class AccountDAO
    {
        public dynamic getUserData(string username)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT NAMA, NPP, PASSWORD
                        FROM simka.MST_KARYAWAN
                        WHERE USERNAME = @username";
                    var param = new {username = username};
                    var data  = conn.QuerySingleOrDefault<dynamic>(query, param);

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public dynamic getUserRole(string npp)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT siatmax.REF_ROLE.ID_ROLE, siatmax.REF_ROLE.DESKRIPSI
                        FROM siatmax.TBL_USER_ROLE
                        INNER JOIN siatmax.REF_ROLE ON siatmax.TBL_USER_ROLE.ID_ROLE = siatmax.REF_ROLE.ID_ROLE
                        WHERE NPP = @npp
                        AND siatmax.TBL_USER_ROLE.ID_SISTEM_INFORMASI = 2";
                    var param = new { npp = npp };
                    var data = conn.QuerySingleOrDefault<dynamic>(query, param);

                    return data;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }
    }
}
