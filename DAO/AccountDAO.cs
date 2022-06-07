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
                        SELECT simka.MST_KARYAWAN.NPP, simka.MST_KARYAWAN.NAMA, simka.MST_KARYAWAN.FILE_FOTO, simka.MST_KARYAWAN.PASSWORD, simka.MST_KARYAWAN.ID_UNIT
                        FROM simka.MST_KARYAWAN
                        INNER JOIN siatmax.TBL_USER_ROLE ON simka.MST_KARYAWAN.NPP = siatmax.TBL_USER_ROLE.NPP
                        WHERE USERNAME = @username
                        AND siatmax.TBL_USER_ROLE.ID_SISTEM_INFORMASI = 2
                        AND siatmax.TBL_USER_ROLE.IS_ACTIVE = 1";
                        
                    var param = new {username = username};
                    var data  = conn.QueryFirstOrDefault<dynamic>(query, param);

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
                    var data = conn.Query<dynamic>(query, param).ToList();

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
