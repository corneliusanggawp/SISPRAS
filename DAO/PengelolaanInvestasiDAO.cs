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
        public DBOutput getRencanaPengadaanAset(string IDUnitUser, Array IDRoleUser)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT		sikeu.TBL_RKA.ID_RKA, sikeu.TBL_TAHUN_ANGGARAN.TAHUN_ANGGARAN, siatmax.MST_UNIT.NAMA_UNIT, sikeu.TBL_MATA_ANGGARAN.PROGRAM_KEGIATAN, sikeu.TBL_RKA.NAMA_PROGRAM
                        FROM		sikeu.TBL_RKA 
                        INNER JOIN	sikeu.TBL_TAHUN_ANGGARAN ON sikeu.TBL_RKA.ID_TAHUN_ANGGARAN = sikeu.TBL_TAHUN_ANGGARAN.ID_TAHUN_ANGGARAN
                        INNER JOIN	siatmax.MST_UNIT ON sikeu.TBL_RKA.ID_UNIT = siatmax.MST_UNIT.ID_UNIT 
                        INNER JOIN	sikeu.TBL_MATA_ANGGARAN ON sikeu.TBL_RKA.ID_MT_ANGGARAN = sikeu.TBL_MATA_ANGGARAN.ID_MT_ANGGARAN
                        WHERE sikeu.TBL_TAHUN_ANGGARAN.IS_CURRENT = 1
                    ";


                    if(Array.IndexOf(IDRoleUser, "9") != -1 && Array.IndexOf(IDRoleUser, "13") != -1 && Array.IndexOf(IDRoleUser, "14") != -1 || IDUnitUser != "0")
                    {
                        query += @" AND siatmax.MST_UNIT.ID_UNIT = @IDUnitUser";
                    }

                    var data    = conn.Query<dynamic>(query, new { IDUnitUser = IDUnitUser }).ToList();

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

        public DBOutput getDetailRencanaKhususInvestasi(int IDRKA)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT		sikeu.DTL_RKA.ID_DTL_RKA, sispras.TBL_PENCAIRAN_INVESTASI.ID_PENCAIRAN_INVESTASI, sispras.TBL_PENCAIRAN_INVESTASI.STATUS_APPROVAL, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.ID_DETAIL_PENCAIRAN_INVESTASI, sikeu.DTL_RKA.NAMA_KEGIATAN, sikeu.DTL_RKA.BULAN, sikeu.DTL_RKA.VOLUME, sikeu.DTL_RKA.SATUAN, sikeu.DTL_RKA.HARGA_SATUAN, sikeu.DTL_RKA.SUBTOTAL
                        FROM		sispras.TBL_PENCAIRAN_INVESTASI
                        RIGHT JOIN	sispras.TBL_DETAIL_PENCAIRAN_INVESTASI ON sispras.TBL_PENCAIRAN_INVESTASI.ID_PENCAIRAN_INVESTASI = sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.ID_PENCAIRAN_INVESTASI
                        RIGHT JOIN	sikeu.DTL_RKA ON sispras.TBL_PENCAIRAN_INVESTASI.ID_DTL_RKA = sikeu.DTL_RKA.ID_DTL_RKA
                        WHERE		(sikeu.DTL_RKA.ID_RKA = @IDRKA)
                    ";

                    var data = conn.Query<dynamic>(query, new { IDRKA = IDRKA }).ToList();

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

        public DBOutput getDetailRencanaPengadaanAset(int IDDetailPencairanInvestasi)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT TBL_DETAIL_PENCAIRAN_INVESTASI.*
                        FROM sispras.TBL_DETAIL_PENCAIRAN_INVESTASI TBL_DETAIL_PENCAIRAN_INVESTASI
                        WHERE (TBL_DETAIL_PENCAIRAN_INVESTASI.ID_DETAIL_PENCAIRAN_INVESTASI = @IDDetailPencairanInvestasi)
                    ";

                    var data = conn.QueryFirstOrDefault<dynamic>(query, new { IDDetailPencairanInvestasi = IDDetailPencairanInvestasi });

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

        public DBOutput addPencairanInvestasi(PencairanInvestasi obj)
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
                            end
                    ";

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

        public DBOutput addDetailPencairanInvestasi(DetailPencairanInvestasi obj)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        INSERT INTO sispras.TBL_DETAIL_PENCAIRAN_INVESTASI (ID_PENCAIRAN_INVESTASI, ID_KATEGORI, ID_REF_SK, SATUAN, HARGA_SATUAN, JUMLAH, IS_PO, SPESIFIKASI, NAMA_PENGADAAN, MERK)
                        VALUES (@IDPencairanInvestasi, @IDKategori, @IDRefSK, @satuan, @hargaSatuan, @jumlah, @isPO, @spesifikasi, @namaPengadaan, @merk)
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

        public DBOutput updateDetailPencairanInvestasi(DetailPencairanInvestasi obj)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        UPDATE sispras.TBL_DETAIL_PENCAIRAN_INVESTASI 
                        SET ID_PENCAIRAN_INVESTASI = @IDPencairanInvestasi, ID_KATEGORI = @IDKategori, ID_REF_SK = @IDRefSK, SATUAN = @satuan, HARGA_SATUAN = @hargaSatuan, JUMLAH = @jumlah, IS_PO = @isPO, SPESIFIKASI = @spesifikasi, NAMA_PENGADAAN = @namaPengadaan, MERK = @merk
                        WHERE ID_DETAIL_PENCAIRAN_INVESTASI = @IDDetailPencairanInvestasi

                        UPDATE sispras.TBL_PENCAIRAN_INVESTASI
                        SET TOTAL_PENCAIRAN = @totalPencairan
                        WHERE ID_PENCAIRAN_INVESTASI = @IDPencairanInvestasi
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

        public DBOutput getPencairanInvestasiApproval()
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT      sispras.TBL_PENCAIRAN_INVESTASI.*, sikeu.TBL_TAHUN_ANGGARAN.TAHUN_ANGGARAN, siatmax.MST_UNIT.NAMA_UNIT
                        FROM        sispras.TBL_PENCAIRAN_INVESTASI
                        INNER JOIN	sikeu.TBL_TAHUN_ANGGARAN ON sispras.TBL_PENCAIRAN_INVESTASI.ID_TAHUN_ANGGARAN = sikeu.TBL_TAHUN_ANGGARAN.ID_TAHUN_ANGGARAN
                        INNER JOIN	siatmax.MST_UNIT ON sispras.TBL_PENCAIRAN_INVESTASI.ID_UNIT = siatmax.MST_UNIT.ID_UNIT
                        WHERE		(sispras.TBL_PENCAIRAN_INVESTASI.STATUS_APPROVAL = 0)
                        AND			(sikeu.TBL_TAHUN_ANGGARAN.IS_CURRENT = 1)
                        AND NOT     (sispras.TBL_PENCAIRAN_INVESTASI.TOTAL_PENCAIRAN = 0)
                        AND EXISTS  (SELECT * FROM sispras.TBL_DETAIL_PENCAIRAN_INVESTASI WHERE sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.ID_PENCAIRAN_INVESTASI = sispras.TBL_PENCAIRAN_INVESTASI.ID_PENCAIRAN_INVESTASI)
                        ORDER BY    sispras.TBL_PENCAIRAN_INVESTASI.INSERT_DATE DESC
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

        public DBOutput getDetailPencairanInvestasiApproval(int IDPencairanInvestasi)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT      sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.ID_DETAIL_PENCAIRAN_INVESTASI, sispras.REF_KATEGORI.DESKRIPSI AS KATEGORI, sispras.REF_SUB_KATEGORI.DESKRIPSI AS SUB_KATEGORI, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.NAMA_PENGADAAN, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.MERK, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.SATUAN, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.SPESIFIKASI, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.HARGA_SATUAN, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.JUMLAH
                        FROM        sispras.TBL_DETAIL_PENCAIRAN_INVESTASI
                        INNER JOIN  sispras.REF_KATEGORI ON sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.ID_KATEGORI = sispras.REF_KATEGORI.ID_KATEGORI
                        INNER JOIN  sispras.REF_SUB_KATEGORI ON sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.ID_REF_SK = sispras.REF_SUB_KATEGORI.ID_REF_SK
                        WHERE       (sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.ID_PENCAIRAN_INVESTASI = @IDPencairanInvestasi)";

                    var data = conn.Query<dynamic>(query, new { IDPencairanInvestasi = IDPencairanInvestasi }).ToList();

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

        public DBOutput approvePencairanInvestasi(int IDPencairanInvestasi)
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
                         WHERE ID_PENCAIRAN_INVESTASI = @IDPencairanInvestasi";

                    output.data = conn.Execute(query, new { IDPencairanInvestasi = IDPencairanInvestasi });
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

        public DBOutput getRekapPengadaanInvestasi(string IDUnitUser, Array IDRoleUser)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT	sikeu.TBL_TAHUN_ANGGARAN.TAHUN_ANGGARAN, sispras.TBL_PENCAIRAN_INVESTASI.BULAN_PENGADAAN, sispras.TBL_PENCAIRAN_INVESTASI.ID_UNIT, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.NAMA_PENGADAAN, sispras.REF_KATEGORI.DESKRIPSI AS KATEGORI, sispras.REF_SUB_KATEGORI.DESKRIPSI AS SUB_KATEGORI, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.SPESIFIKASI, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.MERK, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.JUMLAH, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.SATUAN, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.HARGA_SATUAN, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.IS_PO, sikeu.TBL_TAHUN_ANGGARAN.IS_CURRENT
                        FROM	sispras.TBL_DETAIL_PENCAIRAN_INVESTASI
                        INNER JOIN sispras.REF_KATEGORI ON sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.ID_KATEGORI = sispras.REF_KATEGORI.ID_KATEGORI
                        INNER JOIN sispras.REF_SUB_KATEGORI ON sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.ID_REF_SK = sispras.REF_SUB_KATEGORI.ID_REF_SK
                        INNER JOIN sispras.TBL_PENCAIRAN_INVESTASI ON sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.ID_PENCAIRAN_INVESTASI = sispras.TBL_PENCAIRAN_INVESTASI.ID_PENCAIRAN_INVESTASI
                        INNER JOIN sikeu.TBL_TAHUN_ANGGARAN ON sispras.TBL_PENCAIRAN_INVESTASI.ID_TAHUN_ANGGARAN = sikeu.TBL_TAHUN_ANGGARAN.ID_TAHUN_ANGGARAN
                        INNER JOIN siatmax.MST_UNIT ON sispras.TBL_PENCAIRAN_INVESTASI.ID_UNIT = siatmax.MST_UNIT.ID_UNIT
                        WHERE	(sikeu.TBL_TAHUN_ANGGARAN.IS_CURRENT = 1)
                    ";

                    if (Array.IndexOf(IDRoleUser, "9") != -1 && Array.IndexOf(IDRoleUser, "13") != -1 && Array.IndexOf(IDRoleUser, "14") != -1 || IDUnitUser != "0")
                    {
                        query += @" AND (sispras.TBL_PENCAIRAN_INVESTASI.ID_UNIT = @IDUnitUser)";
                    }

                    query += @" ORDER BY sispras.TBL_PENCAIRAN_INVESTASI.INSERT_DATE DESC";

                    var data = conn.Query<dynamic>(query, new { IDUnitUser = IDUnitUser }).ToList();

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

        public DBOutput getDetailPencairanInvestasiPO()
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT	sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.*, sispras.TBL_PENCAIRAN_INVESTASI.BULAN_PENGADAAN, sikeu.TBL_TAHUN_ANGGARAN.TAHUN_ANGGARAN
                        FROM	sispras.TBL_DETAIL_PENCAIRAN_INVESTASI
                        INNER JOIN	sispras.TBL_PENCAIRAN_INVESTASI ON sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.ID_PENCAIRAN_INVESTASI = sispras.TBL_PENCAIRAN_INVESTASI.ID_PENCAIRAN_INVESTASI
                        INNER JOIN	sikeu.TBL_TAHUN_ANGGARAN ON sispras.TBL_PENCAIRAN_INVESTASI.ID_TAHUN_ANGGARAN = sikeu.TBL_TAHUN_ANGGARAN.ID_TAHUN_ANGGARAN
                        WHERE   (sispras.TBL_PENCAIRAN_INVESTASI.STATUS_APPROVAL = 1)
                        AND     (sikeu.TBL_TAHUN_ANGGARAN.IS_CURRENT = 1)
                        AND     (sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.IS_PO = 0)
                        ORDER BY sispras.TBL_PENCAIRAN_INVESTASI.INSERT_DATE DESC
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

        public DBOutput addPurchaseOrderInvestasi(PurchaseOrderInvestasi obj)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        if not exists(select ID_PO_INVESTASI from sispras.TBL_PO_INVESTASI where NO_PO = @nomorPO)
	                        begin
                                INSERT INTO sispras.TBL_PO_INVESTASI (NO_PO, TGL_PO, TOTAL_PO_TANPA_PAJAK, PAJAK, TOTAL_DENGAN_PAJAK, USER_ID, IP_ADDRESS, INSERT_DATE, ID_SUPPLIER, NAMA_PO)
                                OUTPUT INSERTED.ID_PO_INVESTASI
                                VALUES (@nomorPO, @tanggalPO, @totalTanpaPajak, @pajak, @totalDenganPajak, @userID, @IPAddress, @insertDate, @IDSupplier, @namaPO)
                            end
                        else
                            begin
                                SELECT  ID_PO_INVESTASI
                                FROM    sispras.TBL_PO_INVESTASI
                                WHERE   (NO_PO = @nomorPO)
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

        public DBOutput addDetailPurchaseOrder(int IDPurchaseOrderInvestasi, int IDDetailPencairanInvestasi)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        INSERT INTO sispras.TBL_DETAIL_PO_INVESTASI (ID_PO_INVESTASI, MERK, SATUAN, NAMA_BARANG, SPESIFIKASI, HARGA_SATUAN, JUMLAH, ID_UNIT, ID_DETAIL_PENCAIRAN_INVESTASI)
                        SELECT      @IDPurchaseOrderInvestasi, DPI.MERK, DPI.SATUAN, DPI.NAMA_PENGADAAN, DPI.SPESIFIKASI, DPI.HARGA_SATUAN, DPI.JUMLAH, PI.ID_UNIT, DPI.ID_DETAIL_PENCAIRAN_INVESTASI
                        FROM        sispras.TBL_DETAIL_PENCAIRAN_INVESTASI DPI
                        INNER JOIN  sispras.TBL_PENCAIRAN_INVESTASI PI ON DPI.ID_PENCAIRAN_INVESTASI = PI.ID_PENCAIRAN_INVESTASI
                        WHERE       (DPI.ID_DETAIL_PENCAIRAN_INVESTASI = @IDDetailPencairanInvestasi)";

                    output.data = conn.Execute(query, new { IDPurchaseOrderInvestasi = IDPurchaseOrderInvestasi, IDDetailPencairanInvestasi = IDDetailPencairanInvestasi });

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

        public DBOutput approvePurchaseOrder(int IDDetailPencairanInvestasi)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                         UPDATE sispras.TBL_DETAIL_PENCAIRAN_INVESTASI 
                         SET IS_PO = 1
                         WHERE ID_DETAIL_PENCAIRAN_INVESTASI = @IDDetailPencairanInvestasi";

                    output.data = conn.Execute(query, new { IDDetailPencairanInvestasi = IDDetailPencairanInvestasi });
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

        public dynamic getSpesifikasiDetailPencairanInvestasi(int IDDetailPencairanInvestasi)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT SPESIFIKASI
                        FROM sispras.TBL_DETAIL_PENCAIRAN_INVESTASI
                        WHERE ID_DETAIL_PENCAIRAN_INVESTASI = @IDDetailPencairanInvestasi
                    ";

                    var data = conn.QuerySingleOrDefault<dynamic>(query, new { IDDetailPencairanInvestasi = IDDetailPencairanInvestasi });

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