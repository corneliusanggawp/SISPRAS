using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SISPRAS.DAO
{
    public class MasterDAO
    {
        public List<dynamic> getAllMenu(string IDRole)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT DISTINCT siatmax.TBL_SI_MENU.ID_SI_MENU, siatmax.TBL_SI_MENU.ID_SISTEM_INFORMASI, siatmax.TBL_SI_MENU.DESKRIPSI, siatmax.TBL_SI_MENU.ISACTIVE, siatmax.TBL_SI_MENU.LINK, siatmax.TBL_SI_MENU.NO_URUT, siatmax.TBL_SI_MENU.ICON
                        FROM        siatmax.TBL_ROLE_SUBMENU 
                        INNER JOIN  siatmax.TBL_SI_SUBMENU ON siatmax.TBL_ROLE_SUBMENU.ID_SI_SUBMENU = siatmax.TBL_SI_SUBMENU.ID_SI_SUBMENU
                        INNER JOIN  siatmax.TBL_SI_MENU ON siatmax.TBL_SI_SUBMENU.ID_SI_MENU = siatmax.TBL_SI_MENU.ID_SI_MENU
                        WHERE       siatmax.TBL_SI_MENU.ID_SISTEM_INFORMASI = 2 AND siatmax.TBL_SI_SUBMENU.ISACTIVE = 1 AND siatmax.TBL_SI_MENU.ISACTIVE = 1
                        AND siatmax.TBL_ROLE_SUBMENU.ID_ROLE = @IDRole
                    ";

                    var data = conn.Query<dynamic>(query, new { IDRole = IDRole }).ToList();

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

        public List<dynamic> getAllSubMenu(string IDRole)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT      siatmax.TBL_ROLE_SUBMENU.ID_SI_SUBMENU, siatmax.TBL_ROLE_SUBMENU.ID_ROLE, siatmax.TBL_SI_SUBMENU.ID_SI_MENU, siatmax.TBL_SI_SUBMENU.DESKRIPSI, siatmax.TBL_SI_SUBMENU.ISACTIVE, siatmax.TBL_SI_SUBMENU.LINK
                        FROM        siatmax.TBL_ROLE_SUBMENU 
                        INNER JOIN  siatmax.TBL_SI_SUBMENU ON siatmax.TBL_ROLE_SUBMENU.ID_SI_SUBMENU = siatmax.TBL_SI_SUBMENU.ID_SI_SUBMENU
                        INNER JOIN  siatmax.TBL_SI_MENU ON siatmax.TBL_SI_SUBMENU.ID_SI_MENU = siatmax.TBL_SI_MENU.ID_SI_MENU
                        WHERE       siatmax.TBL_SI_MENU.ID_SISTEM_INFORMASI = 2 AND siatmax.TBL_SI_SUBMENU.ISACTIVE = 1 AND siatmax.TBL_SI_MENU.ISACTIVE = 1
                        AND siatmax.TBL_ROLE_SUBMENU.ID_ROLE = @IDRole
                        ORDER BY siatmax.TBL_SI_SUBMENU.NO_URUT ASC
                    ";

                    var data = conn.Query<dynamic>(query, new { IDRole = IDRole }).ToList();

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

        public List<dynamic> getAllUnit(string IDUnitUser, string IDRoleUser)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT ID_UNIT, NAMA_UNIT
                        FROM siatmax.MST_UNIT
                        WHERE EXISTS(
	                        SELECT DISTINCT MST_ID_UNIT
	                        FROM siatmax.MST_UNIT
                        )
                    ";

                    if (IDRoleUser != "9")
                    {
                        query += @" AND (ID_UNIT = @IDUnitUser)";
                    }
                    else
                    {
                        query += @" AND (MST_ID_UNIT = 285)";
                    }

                    var data = conn.Query<dynamic>(query, new { IDUnitUser = IDUnitUser }).ToList();

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

        public List<dynamic> getAllSubUnit(string IDUnitUser, string IDRoleUser)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                            SELECT ID_UNIT, NAMA_UNIT
                            FROM siatmax.MST_UNIT
                            WHERE EXISTS(
	                            SELECT DISTINCT MST_ID_UNIT
	                            FROM siatmax.MST_UNIT
                            )
                            AND (MST_ID_UNIT = @IDUnitUser)
                    ";

                    var data = conn.Query<dynamic>(query, new { IDUnitUser = IDUnitUser }).ToList();

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
                        FROM sikeu.TBL_TAHUN_ANGGARAN
                        WHERE IS_CURRENT = 1
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

        public List<dynamic> getAllProvinsi()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT *
                        FROM siatmax.REF_PROPINSI
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

        public List<dynamic> getKabKodya(int IDProvinsi)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT *
                        FROM siatmax.REF_KAB_KODYA
                        WHERE ID_REF_PROPINSI = @IDProvinsi
                    ";

                    var data = conn.Query<dynamic>(query, new { IDProvinsi = IDProvinsi }).ToList();

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

        public List<dynamic> getAllKategori()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT *
                        FROM sispras.REF_KATEGORI";

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

        public List<dynamic> getAllSubKategori()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT *
                        FROM sispras.REF_SUB_KATEGORI";
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


        public List<dynamic> getSubKategori(string IDKategori)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT *
                        FROM sispras.REF_SUB_KATEGORI
                        WHERE ID_KATEGORI = @IDKategori
                    ";
                    var data = conn.Query<dynamic>(query, new { IDKategori = IDKategori }).ToList();

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

        public List<dynamic> getAllRuangan()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT *
                        FROM sispras.MST_RUANG_BANGUNAN
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
    }
}
