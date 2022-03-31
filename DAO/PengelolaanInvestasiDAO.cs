using Dapper;
using SISPRA.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;


namespace SISPRA.DAO
{
    public class PengelolaanInvestasiDAO
    {
        public DBOutput getRencanaPengadaanAset(string id_role, string role, string npp)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT ID_TAHUN_ANGGARAN, ID_UNIT, ID_REF_PROGRAM_RPKA, NAMA_PROGRAM
                        FROM sikeu.TBL_RPKA";

                    if(role != "KPSP")
                    {
                        //Filter Data By Role
                        query += @" WHERE ID_UNIT = @id_role";
                    }

                    var data = conn.Query<dynamic>(query, new { id_role = id_role }).ToList();

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

        public DBOutput getDetailRencanaKhususInvestasi(int id)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT 
                        FROM ";

                    var data = conn.Query<dynamic>(query, new { id = id }).ToList();

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

        public DBOutput getDetailRencanaPengadaanAset(int id)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT RK.DESKRIPSI AS Kategori, RSK.DESKRIPSI AS [Sub Kategori], DPI.NAMA_PENGADAAN, DPI.MERK, DPI.SATUAN, DPI.SPESIFIKASI, DPI.HARGA_SATUAN, DPI.JUMLAH
                        FROM sispras.TBL_DETAIL_PENCAIRAN_INVESTASI DPI
                        INNER JOIN sispras.TBL_PENCAIRAN_INVESTASI TPI ON DPI.ID_PENCAIRAN_INVESTASI = TPI.ID_PENCAIRAN_INVESTASI
                        INNER JOIN sispras.REF_KATEGORI RK ON DPI.ID_KATEGORI = RK.ID_KATEGORI 
                        INNER JOIN sispras.REF_SUB_KATEGORI RSK ON DPI.ID_REF_SK = RSK.ID_REF_SK
                        WHERE TPI.ID_PENCAIRAN_INVESTASI = @id";

                    var data = conn.Query<dynamic>(query, new { id = id }).ToList();

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

        public DBOutput updateDetailRencanaPengadaanAset(DetailRencanaPengadaanAset obj)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        UPDATE
                        SET
                        WHERE";

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
    }
}