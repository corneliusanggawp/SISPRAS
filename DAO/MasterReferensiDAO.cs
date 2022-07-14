using Dapper;
using SISPRAS.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SISPRAS.DAO
{
    public class MasterReferensiDAO
    {
        public DBOutput getSupplier()
        {
            DBOutput output = new DBOutput(); 
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT      sispras.MST_SUPPLIER.*, siatmax.REF_PROPINSI.DESKRIPSI AS PROVINSI, siatmax.REF_KAB_KODYA.DESKRIPSI AS KABUPATEN
                        FROM        sispras.MST_SUPPLIER 
                        INNER JOIN  siatmax.REF_PROPINSI ON sispras.MST_SUPPLIER.ID_REF_PROPINSI = siatmax.REF_PROPINSI.ID_REF_PROPINSI
                        INNER JOIN  siatmax.REF_KAB_KODYA ON sispras.MST_SUPPLIER.ID_REF_KAB_KODYA = siatmax.REF_KAB_KODYA.ID_REF_KAB_KODYA
                        AND siatmax.REF_PROPINSI.ID_REF_PROPINSI = siatmax.REF_KAB_KODYA.ID_REF_PROPINSI
                        ORDER BY    sispras.MST_SUPPLIER.ID_SUPPLIER ASC
                    ";

                    var data = conn.Query<dynamic>(query).ToList();

                    output.data = data;
                    return output;
                }
                catch (Exception ex)
                {
                    output.status = false;
                    output.pesan = ex.Message;
                    output.data = new List<string>();
                    return output;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public DBOutput getDetailSupplier(int IDSupplier)
        {
            DBOutput output = new DBOutput(); 
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT *
                        FROM    sispras.MST_SUPPLIER 
                        WHERE   ID_SUPPLIER = @IDSupplier
                    ";

                    var data = conn.QueryFirstOrDefault<dynamic>(query, new { IDSupplier = IDSupplier });

                    output.data = data;
                    return output;
                }
                catch (Exception ex)
                {
                    output.status = false;
                    output.pesan = ex.Message;
                    output.data = new List<string>();
                    return output;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public DBOutput addSupplier(Supplier obj)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        INSERT INTO sispras.MST_SUPPLIER (NAMA_SUPPLIER, ALAMAT, NEGARA, KONTAK_PERSON, NO_TELPON, NO_HP_KONTAK, NO_FAX, ID_REF_PROPINSI, ID_REF_KAB_KODYA)
                        VALUES (@namaSupplier, @alamat, @negara, @kontakPerson, @noTelp, @noHP, @noFax, @IDProvinsi, @IDKabKodya)
                    ";

                    output.data = conn.Execute(query, obj);
                    return output;
                }
                catch (Exception ex)
                {
                    output.status = false;
                    output.pesan = ex.Message;
                    output.data = new List<string>();
                    return output;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public DBOutput updateSupplier(Supplier obj)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        UPDATE sispras.MST_SUPPLIER
                        SET NAMA_SUPPLIER = @namaSupplier, ALAMAT = @alamat, NEGARA = @negara, KONTAK_PERSON = @kontakPerson, NO_TELPON = @noTelp, NO_HP_KONTAK = @noHP, NO_FAX = @noFax, ID_REF_PROPINSI = @IDProvinsi, ID_REF_KAB_KODYA =  @IDKabKodya
                        WHERE ID_SUPPLIER = @IDSupplier
                    ";

                    output.data = conn.Execute(query, obj);
                    return output;
                }
                catch (Exception ex)
                {
                    output.status = false;
                    output.pesan = ex.Message;
                    output.data = new List<string>();
                    return output;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public DBOutput deleteSupplier(int IDSupplier)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        DELETE
                        FROM sispras.MST_SUPPLIER
                        WHERE ID_SUPPLIER = @IDSupplier
                    ";

                    output.data = conn.Execute(query, new { IDSupplier = IDSupplier });
                    return output;
                }
                catch (Exception ex)
                {
                    output.status = false;
                    output.pesan = ex.Message;
                    output.data = new List<string>();
                    return output;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public DBOutput getKategori()
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT  ID_KATEGORI, DESKRIPSI
                        FROM    sispras.REF_KATEGORI
                        ORDER BY ID_KATEGORI ASC
                    ";

                    var data = conn.Query<dynamic>(query).ToList();

                    output.data = data;
                    return output;
                }
                catch (Exception ex)
                {
                    output.status = false;
                    output.pesan = ex.Message;
                    output.data = new List<string>();
                    return output;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public DBOutput getDetailKategori(int IDKategori)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT  *
                        FROM    sispras.REF_KATEGORI
                        WHERE ID_KATEGORI = @IDKategori
                    ";

                    var data = conn.QueryFirstOrDefault<dynamic>(query, new { IDKategori = IDKategori });

                    output.data = data;
                    return output;
                }
                catch (Exception ex)
                {
                    output.status = false;
                    output.pesan = ex.Message;
                    output.data = new List<string>();
                    return output;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public DBOutput addKategori(Kategori obj)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        INSERT INTO sispras.REF_KATEGORI (DESKRIPSI)
                        VALUES (@deskripsi)
                    ";

                    output.data = conn.Execute(query, obj);
                    return output;
                }
                catch (Exception ex)
                {
                    output.status = false;
                    output.pesan = ex.Message;
                    output.data = new List<string>();
                    return output;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public DBOutput updateKategori(Kategori obj)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        UPDATE sispras.REF_KATEGORI
                        SET DESKRIPSI = @deskripsi
                        WHERE ID_KATEGORI = @IDKategori
                    ";

                    output.data = conn.Execute(query, obj);
                    return output;
                }
                catch (Exception ex)
                {
                    output.status = false;
                    output.pesan = ex.Message;
                    output.data = new List<string>();
                    return output;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public DBOutput deleteKategori(int IDKategori)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        DELETE
                        FROM sispras.REF_KATEGORI
                        WHERE ID_KATEGORI = @IDKategori
                    ";

                    output.data = conn.Execute(query, new { IDKategori = IDKategori });
                    return output;
                }
                catch (Exception ex)
                {
                    output.status = false;
                    output.pesan = ex.Message;
                    output.data = new List<string>();
                    return output;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public DBOutput getSubKategori(int IDKategori)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT      sispras.REF_SUB_KATEGORI.ID_REF_SK, sispras.REF_KATEGORI.DESKRIPSI AS KATEGORI, sispras.REF_SUB_KATEGORI.DESKRIPSI, sispras.REF_SUB_KATEGORI.KODE_BARANG
                        FROM        sispras.REF_SUB_KATEGORI
                        INNER JOIN  sispras.REF_KATEGORI ON sispras.REF_SUB_KATEGORI.ID_KATEGORI = sispras.REF_KATEGORI.ID_KATEGORI
                        WHERE       (sispras.REF_SUB_KATEGORI.ID_KATEGORI = @IDKategori)
                    ";

                    var data = conn.Query<dynamic>(query, new { IDKategori = IDKategori }).ToList();

                    output.data = data;
                    return output;
                }
                catch (Exception ex)
                {
                    output.status = false;
                    output.pesan = ex.Message;
                    output.data = new List<string>();
                    return output;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public DBOutput updateSubKategori(SubKategori obj)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        UPDATE sispras.REF_SUB_KATEGORI
                        SET ID_KATEGORI = @IDKategori,DESKRIPSI = @deskripsi, KODE_BARANG = @kodeBarang
                        WHERE ID_REF_SK = @IDSubKategori
                    ";

                    output.data = conn.Execute(query, obj);
                    return output;
                }
                catch (Exception ex)
                {
                    output.status = false;
                    output.pesan = ex.Message;
                    output.data = new List<string>();
                    return output;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

        public DBOutput deleteSubKategori(int IDSubKategori)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        DELETE
                        FROM sispras.REF_SUB_KATEGORI
                        WHERE ID_REF_SK = @IDSubKategori
                    ";

                    output.data = conn.Execute(query, new { IDSubKategori = IDSubKategori });
                    return output;
                }
                catch (Exception ex)
                {
                    output.status = false;
                    output.pesan = ex.Message;
                    output.data = new List<string>();
                    return output;
                }
                finally
                {
                    conn.Dispose();
                }
            }
        }

    }
}
