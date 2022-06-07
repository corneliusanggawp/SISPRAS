using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SISPRA.DAO
{
    public class MasterDAO
    {
        public List<dynamic> getAllSubMenu(Array id_role)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    int first = 0;

                    string query = @"
                        SELECT      siatmax.TBL_ROLE_SUBMENU.ID_SI_SUBMENU, siatmax.TBL_ROLE_SUBMENU.ID_ROLE, siatmax.TBL_SI_SUBMENU.ID_SI_MENU, siatmax.TBL_SI_SUBMENU.DESKRIPSI, siatmax.TBL_SI_SUBMENU.ISACTIVE, siatmax.TBL_SI_SUBMENU.LINK
                        FROM        siatmax.TBL_ROLE_SUBMENU 
                        INNER JOIN  siatmax.TBL_SI_SUBMENU ON siatmax.TBL_ROLE_SUBMENU.ID_SI_SUBMENU = siatmax.TBL_SI_SUBMENU.ID_SI_SUBMENU
                        WHERE       siatmax.TBL_SI_SUBMENU.ISACTIVE = 1
                        AND";

                    if (Array.IndexOf(id_role, "9") != -1)
                    {
                        query += @"siatmax.TBL_ROLE_SUBMENU.ID_ROLE = 9";
                    }
                    else
                    {
                        foreach (var id in id_role)
                        {
                            first = 1;
                            query += @" AND siatmax.TBL_ROLE_SUBMENU.ID_ROLE " + String.Join("=", id);
                        }
                    }

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
