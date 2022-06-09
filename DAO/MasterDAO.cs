using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SISPRA.DAO
{
    public class MasterDAO
    {
        public List<dynamic> getAllMenu(Array id_role)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT      ID_SI_MENU, ID_SISTEM_INFORMASI, DESKRIPSI, ISACTIVE, LINK, NO_URUT, ICON
                        FROM        siatmax.TBL_SI_MENU
                        WHERE       ID_SISTEM_INFORMASI = 2 AND ISACTIVE = 1 ";

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

        public List<dynamic> getAllSubMenu(Array id_role)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    int arrayIndex = 0;

                    string query = @"
                        SELECT      siatmax.TBL_ROLE_SUBMENU.ID_SI_SUBMENU, siatmax.TBL_ROLE_SUBMENU.ID_ROLE, siatmax.TBL_SI_SUBMENU.ID_SI_MENU, siatmax.TBL_SI_SUBMENU.DESKRIPSI, siatmax.TBL_SI_SUBMENU.ISACTIVE, siatmax.TBL_SI_SUBMENU.LINK
                        FROM        siatmax.TBL_ROLE_SUBMENU 
                        INNER JOIN  siatmax.TBL_SI_SUBMENU ON siatmax.TBL_ROLE_SUBMENU.ID_SI_SUBMENU = siatmax.TBL_SI_SUBMENU.ID_SI_SUBMENU
                        INNER JOIN  siatmax.TBL_SI_MENU ON siatmax.TBL_SI_SUBMENU.ID_SI_MENU = siatmax.TBL_SI_MENU.ID_SI_MENU
                        WHERE       siatmax.TBL_SI_MENU.ID_SISTEM_INFORMASI = 2 AND siatmax.TBL_SI_SUBMENU.ISACTIVE = 1";

                    if (Array.IndexOf(id_role, "9") != -1)
                    {
                        query += @"AND siatmax.TBL_ROLE_SUBMENU.ID_ROLE = 9";
                    }
                    else
                    {
                        if(id_role.Length > 1)
                        {
                            foreach (var id in id_role)
                            {
                                arrayIndex++;
                                if (arrayIndex > 1)
                                {
                                    query += @" OR ";
                                }
                                else
                                {
                                    query += @" AND (";
                                }

                                query += @" siatmax.TBL_ROLE_SUBMENU.ID_ROLE = " + String.Join("", id);
                            }
                            query += @")";
                        }
                        else
                        {
                            foreach (var id in id_role)
                            {
                                query += @" AND siatmax.TBL_ROLE_SUBMENU.ID_ROLE = " + String.Join("", id);
                            }
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
