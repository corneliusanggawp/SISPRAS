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
        public DBOutput getRencanaPengadaanAset(string id_unit, Array id_role)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT sikeu.TBL_RKA.ID_RPKA, sikeu.TBL_TAHUN_ANGGARAN.TAHUN_ANGGARAN, siatmax.MST_UNIT.NAMA_UNIT, sikeu.TBL_MATA_ANGGARAN.PROGRAM_KEGIATAN, sikeu.TBL_RKA.NAMA_PROGRAM, siatmax.MST_UNIT.MST_ID_UNIT, sikeu.TBL_RKA.ID_RKA
                        FROM sikeu.TBL_RPKA 
                        INNER JOIN sikeu.TBL_RKA ON sikeu.TBL_RPKA.ID_RPKA = sikeu.TBL_RKA.ID_RPKA 
                        INNER JOIN sikeu.TBL_MATA_ANGGARAN ON sikeu.TBL_RKA.ID_MT_ANGGARAN = sikeu.TBL_MATA_ANGGARAN.ID_MT_ANGGARAN
                        INNER JOIN sikeu.REF_PROGRAM ON sikeu.TBL_MATA_ANGGARAN.ID_REF_PROGRAM = sikeu.REF_PROGRAM.ID_REF_PROGRAM 
                        INNER JOIN sikeu.TBL_TAHUN_ANGGARAN ON sikeu.TBL_RKA.ID_TAHUN_ANGGARAN = sikeu.TBL_TAHUN_ANGGARAN.ID_TAHUN_ANGGARAN
                        INNER JOIN siatmax.MST_UNIT ON sikeu.TBL_RPKA.ID_UNIT = siatmax.MST_UNIT.ID_UNIT
                        WHERE siatmax.MST_UNIT.ID_UNIT = 246";

                    if(Array.IndexOf(id_role, "9") != -1 && Array.IndexOf(id_role, "13") != -1 && Array.IndexOf(id_role, "14") != -1 && id_unit != "0")
                    {
                        query += @" AND siatmax.MST_UNIT.MST_ID_UNIT = @id_unit";
                    }

                    var data = conn.Query<dynamic>(query, new { id_unit = id_unit }).ToList();

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
                        SELECT TBL_PENCAIRAN_INVESTASI.ID_PENCAIRAN_INVESTASI, TBL_DETAIL_PENCAIRAN_INVESTASI.ID_DETAIL_PENCAIRAN_INVESTASI , DTL_RKA.ID_DTL_RKA, DTL_RKA.ID_RKA, DTL_RKA.NAMA_KEGIATAN, DTL_RKA.BULAN, DTL_RKA.VOLUME, DTL_RKA.SATUAN, DTL_RKA.HARGA_SATUAN, DTL_RKA.SUBTOTAL, TBL_PENCAIRAN_INVESTASI.ID_DTL_RKA, DTL_RKA.ID_DTL_RKA AS DTL_RKA
                        FROM sikeu.DTL_RKA DTL_RKA 
                        LEFT JOIN sispras.TBL_PENCAIRAN_INVESTASI TBL_PENCAIRAN_INVESTASI ON DTL_RKA.ID_DTL_RKA = TBL_PENCAIRAN_INVESTASI.ID_DTL_RKA
	                    LEFT JOIN sispras.TBL_DETAIL_PENCAIRAN_INVESTASI TBL_DETAIL_PENCAIRAN_INVESTASI ON TBL_PENCAIRAN_INVESTASI.ID_PENCAIRAN_INVESTASI = TBL_DETAIL_PENCAIRAN_INVESTASI.ID_PENCAIRAN_INVESTASI 
                        WHERE (DTL_RKA.ID_RKA = @id)";

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

        public DBOutput getDetailRencanaPengadaanAset(int IDDetailRKA)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT DPI.*
                        FROM sispras.TBL_DETAIL_PENCAIRAN_INVESTASI DPI
                        INNER JOIN sispras.TBL_PENCAIRAN_INVESTASI TPI ON DPI.ID_PENCAIRAN_INVESTASI = TPI.ID_PENCAIRAN_INVESTASI
                        INNER JOIN sispras.REF_KATEGORI RK ON DPI.ID_KATEGORI = RK.ID_KATEGORI 
                        INNER JOIN sispras.REF_SUB_KATEGORI RSK ON DPI.ID_REF_SK = RSK.ID_REF_SK
                        WHERE TPI.ID_DTL_RKA = @IDDetailRKA";

                    var data = conn.QueryFirstOrDefault<dynamic>(query, new { IDDetailRKA = IDDetailRKA });

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

        public DBOutput getIDPencairanInvestasi(int IDDetailRKA)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT ID_PENCAIRAN_INVESTASI
                        FROM sispras.TBL_PENCAIRAN_INVESTASI
                        WHERE ID_DTL_RKA = @IDDetailRKA";

                    var data = conn.Query<dynamic>(query, new { IDDetailRKA = IDDetailRKA }).ToList();

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

        public DBOutput addPencairanInvestasi(RencanaPengadaanAset obj)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        if not exists(select ID_PENCAIRAN_INVESTASI from sispras.TBL_PENCAIRAN_INVESTASI where ID_DTL_RKA = @IDDetailRKA)
	                        begin
                                INSERT INTO sispras.TBL_PENCAIRAN_INVESTASI (ID_TAHUN_ANGGARAN, ID_UNIT, BULAN_PENGADAAN, TGL_PENCAIRAN, TOTAL_PENCAIRAN, INSERT_DATE, IP_ADDRESS, USER_ID, STATUS_APPROVAL, ID_DTL_RKA)
                                OUTPUT INSERTED.ID_PENCAIRAN_INVESTASI
                                SELECT      sikeu.TBL_RKA.ID_TAHUN_ANGGARAN, sikeu.TBL_RKA.ID_UNIT, sikeu.DTL_RKA.BULAN, @tanggalPencairan AS TGL_PENCAIRAN, @totalPencairan AS TOTAL_PENCAIRAN, @insertDate AS INSERT_DATE, @IPAddress AS IP_ADDRESS, @userID AS USER_ID, @statusApproval AS STATUS_APPROVAL, sikeu.DTL_RKA.ID_DTL_RKA
                                FROM        sikeu.TBL_RKA
                                INNER JOIN  sikeu.DTL_RKA ON sikeu.TBL_RKA.ID_RKA = sikeu.DTL_RKA.ID_RKA
                                WHERE       (sikeu.DTL_RKA.ID_DTL_RKA = @IDDetailRKA)
                            end
                        else
                            begin
                                SELECT ID_PENCAIRAN_INVESTASI
                                FROM sispras.TBL_PENCAIRAN_INVESTASI
                                WHERE ID_DTL_RKA = @IDDetailRKA
                            end";

                    var data = conn.QueryFirstOrDefault<int>(query, obj);

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

        public DBOutput addDetailPencairanInvestasi(DetailRencanaPengadaanAset obj)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        if not exists(select ID_DETAIL_PENCAIRAN_INVESTASI from sispras.TBL_DETAIL_PENCAIRAN_INVESTASI where ID_PENCAIRAN_INVESTASI = @IDPencairanInvestasi)
	                        begin
                                INSERT INTO sispras.TBL_DETAIL_PENCAIRAN_INVESTASI (ID_PENCAIRAN_INVESTASI, ID_KATEGORI, ID_REF_SK, SATUAN, HARGA_SATUAN, JUMLAH, IS_PO, SPESIFIKASI, NAMA_PENGADAAN, MERK)
                                VALUES (@IDPencairanInvestasi, @IDKategori, @IDRefSK, @satuan, @hargaSatuan, @jumlah, @isPO, @spesifikasi, @namaPengadaan, @merk)
                            end
                        else
                            begin
                                UPDATE sispras.TBL_DETAIL_PENCAIRAN_INVESTASI 
                                SET ID_PENCAIRAN_INVESTASI = @IDPencairanInvestasi, ID_KATEGORI = @IDKategori, ID_REF_SK = @IDRefSK, SATUAN = @satuan, HARGA_SATUAN = @hargaSatuan, JUMLAH = @jumlah, IS_PO = @isPO, SPESIFIKASI = @spesifikasi, NAMA_PENGADAAN = @namaPengadaan, MERK = @merk
                                WHERE ID_DETAIL_PENCAIRAN_INVESTASI = (
                                    SELECT ID_DETAIL_PENCAIRAN_INVESTASI
                                    FROM sispras.TBL_DETAIL_PENCAIRAN_INVESTASI TDPI
                                    WHERE ID_PENCAIRAN_INVESTASI = @IDPencairanInvestasi
                                )          
                            end
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

        public DBOutput getKategori(int IDRefSK)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT ID_KATEGORI
                        FROM sispras.REF_SUB_KATEGORI
                        WHERE ID_REF_SK = @IDRefSK";

                    var data = conn.Query<int>(query, new { IDRefSK = IDRefSK });

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

        public DBOutput getPencairanInvestasi(string id_unit, Array id_role)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT      TPI.*, TTA.TAHUN_ANGGARAN, MU.NAMA_UNIT
                        FROM        sispras.TBL_PENCAIRAN_INVESTASI TPI
                        INNER JOIN	sikeu.TBL_TAHUN_ANGGARAN TTA ON TPI.ID_TAHUN_ANGGARAN = TTA.ID_TAHUN_ANGGARAN
                        INNER JOIN	siatmax.MST_UNIT MU ON TPI.ID_UNIT = MU.ID_UNIT
                        WHERE		TPI.STATUS_APPROVAL = 0
                        ORDER BY    TPI.INSERT_DATE DESC";

                    if (Array.IndexOf(id_role, "9") != -1 && Array.IndexOf(id_role, "13") != -1 && Array.IndexOf(id_role, "14") != -1 && id_unit != "0")
                    {
                        query += @" AND siatmax.MST_UNIT.MST_ID_UNIT = @id_unit";
                    }

                    var data = conn.Query<dynamic>(query, new { id_unit = id_unit }).ToList();

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

        public DBOutput getDetailPencairanInvestasi(int id)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT      sispras.REF_KATEGORI.DESKRIPSI AS KATEGORI, sispras.REF_SUB_KATEGORI.DESKRIPSI AS SUB_KATEGORI, DPI.NAMA_PENGADAAN, DPI.MERK, DPI.SATUAN, DPI.SPESIFIKASI, DPI.HARGA_SATUAN, DPI.JUMLAH
                        FROM        sispras.TBL_DETAIL_PENCAIRAN_INVESTASI DPI
                        INNER JOIN  sispras.REF_KATEGORI ON DPI.ID_KATEGORI = sispras.REF_KATEGORI.ID_KATEGORI
                        INNER JOIN  sispras.REF_SUB_KATEGORI ON DPI.ID_REF_SK = sispras.REF_SUB_KATEGORI.ID_REF_SK
                        WHERE       (DPI.ID_PENCAIRAN_INVESTASI = @id)";

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

        public DBOutput approvePencairanInvestasi(int id)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                         UPDATE sispras.TBL_PENCAIRAN_INVESTASI 
                         SET STATUS_APPROVAL = 1
                         WHERE ID_PENCAIRAN_INVESTASI = @id";

                    output.data = conn.Execute(query, new { id = id });
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

        public DBOutput getRekapPengadaanInvestasi(string id_unit, Array id_role)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT DPI.*, RSK.DESKRIPSI AS SUB_KATEGORI, PI.ID_UNIT
                        FROM sispras.TBL_DETAIL_PENCAIRAN_INVESTASI DPI
                        INNER JOIN sispras.REF_SUB_KATEGORI RSK ON DPI.ID_REF_SK = RSK.ID_REF_SK
                        INNER JOIN sispras.TBL_PENCAIRAN_INVESTASI PI ON DPI.ID_PENCAIRAN_INVESTASI = PI.ID_PENCAIRAN_INVESTASI
                        WHERE PI.ID_UNIT = 246
                        ORDER BY PI.INSERT_DATE DESC";

                    //if (Array.IndexOf(id_role, "9") != -1 && Array.IndexOf(id_role, "13") != -1 && Array.IndexOf(id_role, "14") != -1 && id_unit != "0")
                    //{
                    //    query += @" AND siatmax.MST_UNIT.MST_ID_UNIT = @id_unit";
                    //}

                    var data = conn.Query<dynamic>(query, new { id_unit = id_unit }).ToList();

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

        public DBOutput getDetailPencairanInvestasi()
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                    SELECT	sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.*
                    FROM	sispras.TBL_DETAIL_PENCAIRAN_INVESTASI
                    INNER JOIN	sispras.TBL_PENCAIRAN_INVESTASI ON sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.ID_PENCAIRAN_INVESTASI = sispras.TBL_PENCAIRAN_INVESTASI.ID_PENCAIRAN_INVESTASI
                    WHERE (sispras.TBL_PENCAIRAN_INVESTASI.STATUS_APPROVAL = 1)
                    ORDER BY sispras.TBL_PENCAIRAN_INVESTASI.INSERT_DATE DESC";

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

        public List<dynamic> getAllSupplier()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT  ID_SUPPLIER, NAMA_SUPPLIER
                        FROM    sispras.MST_SUPPLIER";

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