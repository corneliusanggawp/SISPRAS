using Dapper;
using SISPRAS.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;


namespace SISPRAS.DAO
{
    public class PengelolaanInvestasiDAO
    {
        public DBOutput getRencanaPengadaanAset(string IDUnitUser, string IDRoleUser)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT	sikeu.TBL_RKA.ID_RKA, sikeu.TBL_TAHUN_ANGGARAN.TAHUN_ANGGARAN, siatmax.MST_UNIT.MST_ID_UNIT, siatmax.MST_UNIT.NAMA_UNIT, sikeu.TBL_MATA_ANGGARAN.PROGRAM_KEGIATAN, sikeu.TBL_RPKA.NAMA_PROGRAM
                        FROM	sikeu.TBL_RPKA
                        INNER JOIN	siatmax.MST_UNIT ON sikeu.TBL_RPKA.ID_UNIT = siatmax.MST_UNIT.ID_UNIT
                        INNER JOIN	sikeu.TBL_TAHUN_ANGGARAN ON sikeu.TBL_RPKA.ID_TAHUN_ANGGARAN = sikeu.TBL_TAHUN_ANGGARAN.ID_TAHUN_ANGGARAN 
                        INNER JOIN	sikeu.TBL_RKA ON sikeu.TBL_RPKA.ID_RPKA = sikeu.TBL_RKA.ID_RPKA
                        INNER JOIN	sikeu.TBL_MATA_ANGGARAN ON sikeu.TBL_RKA.ID_MT_ANGGARAN = sikeu.TBL_MATA_ANGGARAN.ID_MT_ANGGARAN
                        INNER JOIN	sikeu.REF_PROGRAM ON sikeu.TBL_MATA_ANGGARAN.ID_REF_PROGRAM = sikeu.REF_PROGRAM.ID_REF_PROGRAM
                        WHERE       (sikeu.TBL_TAHUN_ANGGARAN.IS_CURRENT = 1) AND (sikeu.REF_PROGRAM.ID_REF_PROGRAM = 4)
                    ";

                    if (IDRoleUser != "9")
                    {
                        query += @" AND (siatmax.MST_UNIT.MST_ID_UNIT = @IDUnitUser)";
                    }

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
                        SELECT ID_KATEGORI, ID_REF_SK, SATUAN, HARGA_SATUAN, JUMLAH, IS_PO, SPESIFIKASI, NAMA_PENGADAAN, MERK
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
                                SELECT      sikeu.TBL_RPKA.ID_TAHUN_ANGGARAN, sikeu.TBL_RPKA.ID_UNIT, sikeu.DTL_RKA.BULAN, @tanggalPencairan AS TGL_PENCAIRAN, @totalPencairan AS TOTAL_PENCAIRAN, @insertDate AS INSERT_DATE, @IPAddress AS IP_ADDRESS, @userID AS USER_ID, @statusApproval AS STATUS_APPROVAL, sikeu.DTL_RKA.ID_DTL_RKA
                                FROM        sikeu.TBL_RPKA
                                INNER JOIN	sikeu.TBL_RKA ON sikeu.TBL_RPKA.ID_RPKA = sikeu.TBL_RKA.ID_RPKA
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
                        SET ID_KATEGORI = @IDKategori, ID_REF_SK = @IDRefSK, SATUAN = @satuan, HARGA_SATUAN = @hargaSatuan, JUMLAH = @jumlah, IS_PO = @isPO, SPESIFIKASI = @spesifikasi, NAMA_PENGADAAN = @namaPengadaan, MERK = @merk
                        WHERE ID_DETAIL_PENCAIRAN_INVESTASI = @IDDetailPencairanInvestasi

                        UPDATE sispras.TBL_PENCAIRAN_INVESTASI
                        SET sispras.TBL_PENCAIRAN_INVESTASI.TOTAL_PENCAIRAN = @totalPencairan
                        FROM sispras.TBL_PENCAIRAN_INVESTASI 
                        INNER JOIN sispras.TBL_DETAIL_PENCAIRAN_INVESTASI ON sispras.TBL_PENCAIRAN_INVESTASI.ID_PENCAIRAN_INVESTASI = sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.ID_PENCAIRAN_INVESTASI
                        WHERE (sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.ID_DETAIL_PENCAIRAN_INVESTASI = @IDDetailPencairanInvestasi)
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

        public DBOutput getRekapPengadaanInvestasi(string IDUnitUser, string IDRoleUser)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT	sikeu.TBL_TAHUN_ANGGARAN.TAHUN_ANGGARAN, sispras.TBL_PENCAIRAN_INVESTASI.BULAN_PENGADAAN, siatmax.MST_UNIT.MST_ID_UNIT, siatmax.MST_UNIT.NAMA_UNIT, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.NAMA_PENGADAAN, sispras.REF_KATEGORI.DESKRIPSI AS KATEGORI, sispras.REF_SUB_KATEGORI.DESKRIPSI AS SUB_KATEGORI, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.SPESIFIKASI, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.MERK, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.JUMLAH, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.SATUAN, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.HARGA_SATUAN, sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.IS_PO, sispras.TBL_PENCAIRAN_INVESTASI.STATUS_APPROVAL
                        FROM	sispras.TBL_DETAIL_PENCAIRAN_INVESTASI
                        INNER JOIN sispras.REF_KATEGORI ON sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.ID_KATEGORI = sispras.REF_KATEGORI.ID_KATEGORI
                        INNER JOIN sispras.REF_SUB_KATEGORI ON sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.ID_REF_SK = sispras.REF_SUB_KATEGORI.ID_REF_SK
                        INNER JOIN sispras.TBL_PENCAIRAN_INVESTASI ON sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.ID_PENCAIRAN_INVESTASI = sispras.TBL_PENCAIRAN_INVESTASI.ID_PENCAIRAN_INVESTASI
                        INNER JOIN sikeu.TBL_TAHUN_ANGGARAN ON sispras.TBL_PENCAIRAN_INVESTASI.ID_TAHUN_ANGGARAN = sikeu.TBL_TAHUN_ANGGARAN.ID_TAHUN_ANGGARAN
                        INNER JOIN siatmax.MST_UNIT ON sispras.TBL_PENCAIRAN_INVESTASI.ID_UNIT = siatmax.MST_UNIT.ID_UNIT
                        WHERE	(sikeu.TBL_TAHUN_ANGGARAN.IS_CURRENT = 1)
                    ";

                    if (IDRoleUser != "9")
                    {
                        query += @" AND (siatmax.MST_UNIT.MST_ID_UNIT = @IDUnitUser)";
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

        public DBOutput getApprovalRequestPencairanInvestasi()
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT      ID_PENCAIRAN_INVESTASI, TAHUN_ANGGARAN, BULAN_PENGADAAN, MST_ID_UNIT, NAMA_UNIT, INSERT_DATE, TOTAL_PENCAIRAN
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

        public DBOutput getApprovalRequestDetailPencairanInvestasi(int IDPencairanInvestasi)
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

        public DBOutput getPurchaseOrderRequestPencairanInvestasi()
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT ID_DETAIL_PENCAIRAN_INVESTASI, TAHUN_ANGGARAN, BULAN_PENGADAAN, ID_KATEGORI, ID_REF_SK, NAMA_PENGADAAN, MERK, HARGA_SATUAN, JUMLAH, (JUMLAH * HARGA_SATUAN) AS TOTAL_PENGADAAN
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

        public dynamic getLastPurchaseOrderNumber()
        {

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT COUNT(ID_PO_INVESTASI) AS NO_PO
                        FROM	sispras.TBL_PO_INVESTASI
                        WHERE	MONTH(TGL_PO) = DATEPART(month, GETDATE())
                        AND IS_REVISI = 0
                    ";

                    var data = conn.QueryFirstOrDefault<int>(query);

                    return data+1;
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

        public dynamic getTotalPurchaseOrder(string[] IDDetailPencairanInvestasi)
        {

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT SUM(JUMLAH*HARGA_SATUAN) AS TOTAL_PURCHASE_ORDER
                        FROM sispras.TBL_DETAIL_PENCAIRAN_INVESTASI
                        WHERE ID_DETAIL_PENCAIRAN_INVESTASI IN @IDDetailPencairanInvestasi
                    ";

                    var data = conn.QueryFirstOrDefault<decimal>(query, new { IDDetailPencairanInvestasi = IDDetailPencairanInvestasi });

                    return data;
                }
                catch (Exception ex)
                {
                    return 0;
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
                                INSERT INTO sispras.TBL_PO_INVESTASI (NO_PO, TGL_PO, TOTAL_PO_TANPA_PAJAK, PAJAK, TOTAL_DENGAN_PAJAK, USER_ID, IP_ADDRESS, INSERT_DATE, ID_SUPPLIER, NAMA_PO, IS_LUNAS, IS_REVISI, DISCOUNT)
                                OUTPUT INSERTED.ID_PO_INVESTASI
                                VALUES (@nomorPO, @tanggalPO, @totalTanpaPajak, @pajak, @totalDenganPajak, @userID, @IPAddress, @insertDate, @IDSupplier, @namaPO, @isLunas, @isRevisi, @diskon)
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

        public DBOutput addDetailPurchaseOrderInvestasi(int IDPurchaseOrderInvestasi, string IDDetailPencairanInvestasi)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        INSERT INTO sispras.TBL_DETAIL_PO_INVESTASI (ID_PO_INVESTASI, MERK, SATUAN, NAMA_BARANG, SPESIFIKASI, HARGA_SATUAN, JUMLAH, BARANG_DATANG, ID_UNIT, ID_DETAIL_PENCAIRAN_INVESTASI)
                        SELECT      @IDPurchaseOrderInvestasi, DPI.MERK, DPI.SATUAN, DPI.NAMA_PENGADAAN, DPI.SPESIFIKASI, DPI.HARGA_SATUAN, DPI.JUMLAH, 0 , PI.ID_UNIT, DPI.ID_DETAIL_PENCAIRAN_INVESTASI
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

        public DBOutput approvePurchaseOrderInvestasi(string IDDetailPencairanInvestasi)
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
                         WHERE ID_DETAIL_PENCAIRAN_INVESTASI = @IDDetailPencairanInvestasi
                    ";

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

        public List<dynamic> getListNomorPO(string nomorPO)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT A.ID_PO_INVESTASI, A.NO_PO, (
	                        case when charindex('-', A.NO_PO) = 0 then A.NO_PO 
	                        else left(A.NO_PO, charindex('-', A.NO_PO) - 1) end) + '-REV' + CAST(
		                        (SELECT COUNT(*) + 1
		                        FROM sispras.TBL_PO_INVESTASI B
		                        WHERE B.NO_PO LIKE 
			                        case when charindex('-', A.NO_PO) = 0 then A.NO_PO 
			                        else left(A.NO_PO, charindex('-', A.NO_PO) - 1) end + '%'
		                        AND B.IS_REVISI = 1) AS VARCHAR(10)) AS NEW_NO_PO,
	                        ROUND(((((A.TOTAL_DENGAN_PAJAK + A.DISCOUNT) - (A.TOTAL_PO_TANPA_PAJAK - A.DISCOUNT))/(A.TOTAL_DENGAN_PAJAK + A.DISCOUNT)) * 100), 0)AS PAJAK,
	                        A.TGL_PO, A.TOTAL_PO_TANPA_PAJAK, A.DISCOUNT, A.ID_SUPPLIER
                        FROM sispras.TBL_PO_INVESTASI A
                        LEFT JOIN sispras.TBL_TERIMA_ASET C ON A.ID_PO_INVESTASI = C.ID_PO_INVESTASI
                        WHERE A.NO_PO LIKE @nomorPO + '%'
                        AND A.IS_REVISI = 0
                        AND IS_LUNAS = 0
                        AND C.ID_TERIMA_ASET IS NULL
                    ";

                    var data = conn.Query<dynamic>(query, new { nomorPO = nomorPO }).ToList();

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

        public string getNomorPORevisi(int IDPurchaseOrderInvestasi)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT (
	                        case when charindex('-', A.NO_PO) = 0 
	                        then A.NO_PO
	                        else left(A.NO_PO, charindex('-', A.NO_PO) - 1) end
	                        + '-REV' + CAST((
		                        SELECT COUNT(*) + 1
		                        FROM sispras.TBL_PO_INVESTASI B
		                        WHERE B.NO_PO LIKE (
										case when charindex('-', A.NO_PO) = 0 
										then A.NO_PO
										else left(A.NO_PO, charindex('-', A.NO_PO) - 1) end
								    )+ '%'
		                        AND B.IS_REVISI = 1
	                        ) AS VARCHAR)
                        )
                        FROM sispras.TBL_PO_INVESTASI A
                        WHERE ID_PO_INVESTASI = @IDPurchaseOrderInvestasi
                    ";

                    var data = conn.QueryFirstOrDefault<string>(query, new { IDPurchaseOrderInvestasi = IDPurchaseOrderInvestasi });

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

        public DBOutput getDetailPurchaseOrder(string IDPurchaseOrderInvestasi)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT  NAMA_BARANG, MERK, SATUAN, SPESIFIKASI, HARGA_SATUAN, JUMLAH, (HARGA_SATUAN*JUMLAH) AS TOTAL, ID_DETAIL_PO_INVESTASI
                        FROM	sispras.TBL_DETAIL_PO_INVESTASI
                        INNER JOIN	sispras.TBL_PO_INVESTASI ON sispras.TBL_DETAIL_PO_INVESTASI.ID_PO_INVESTASI = sispras.TBL_PO_INVESTASI.ID_PO_INVESTASI
                        LEFT JOIN sispras.TBL_TERIMA_ASET ON sispras.TBL_PO_INVESTASI.ID_PO_INVESTASI = sispras.TBL_TERIMA_ASET.ID_PO_INVESTASI
                        WHERE	sispras.TBL_PO_INVESTASI.ID_PO_INVESTASI = @IDPurchaseOrderInvestasi 
                        AND sispras.TBL_PO_INVESTASI.IS_REVISI = 0
                        AND sispras.TBL_TERIMA_ASET.ID_TERIMA_ASET IS NULL
                    ";

                    var data = conn.Query<dynamic>(query, new { IDPurchaseOrderInvestasi = IDPurchaseOrderInvestasi }).ToList();

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

        public DBOutput addRevisiPurchaseOrderInvestasi(PurchaseOrderInvestasi obj)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        INSERT INTO sispras.TBL_PO_INVESTASI (NO_PO, TGL_PO, TOTAL_PO_TANPA_PAJAK, PAJAK, TOTAL_DENGAN_PAJAK, USER_ID, IP_ADDRESS, INSERT_DATE, ID_SUPPLIER, NAMA_PO, IS_LUNAS, IS_REVISI, DISCOUNT)
                        OUTPUT INSERTED.ID_PO_INVESTASI
                        VALUES (@nomorPO, @tanggalPO, @totalTanpaPajak, @pajak, @totalDenganPajak, @userID, @IPAddress, @insertDate, @IDSupplier, @namaPO, @isLunas, @isRevisi, @diskon)
                    
                        UPDATE sispras.TBL_PO_INVESTASI
                        SET IS_REVISI = 1
                        WHERE ID_PO_INVESTASI = @IDPurchaseOrderInvestasi               
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

        public DBOutput addRevisiDetailPurchaseOrderInvestasi(DetailPurchaseOrderInvestasi obj)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        INSERT INTO sispras.TBL_DETAIL_PO_INVESTASI (ID_PO_INVESTASI, MERK, SATUAN, NAMA_BARANG, SPESIFIKASI, HARGA_SATUAN, JUMLAH, BARANG_DATANG, ID_UNIT, ID_DETAIL_PENCAIRAN_INVESTASI)
                        SELECT @IDPurchaseOrderInvestasi, @merk, @satuan, @namaBarang, @spesifikasi, @hargaSatuan, @jumlah, 0, ID_UNIT, ID_DETAIL_PENCAIRAN_INVESTASI
                        FROM	sispras.TBL_DETAIL_PO_INVESTASI
                        WHERE	sispras.TBL_DETAIL_PO_INVESTASI.ID_DETAIL_PO_INVESTASI = @IDDetailPurchaseOrderInvestasi
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

        public DBOutput getRekapPurchaseOrderInvestasi()
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT      sispras.TBL_PO_INVESTASI.ID_PO_INVESTASI, sispras.TBL_PO_INVESTASI.NO_PO, sispras.TBL_PO_INVESTASI.TGL_PO, sispras.TBL_PO_INVESTASI.TOTAL_PO_TANPA_PAJAK, ROUND(((((sispras.TBL_PO_INVESTASI.TOTAL_DENGAN_PAJAK + sispras.TBL_PO_INVESTASI.DISCOUNT) - (sispras.TBL_PO_INVESTASI.TOTAL_PO_TANPA_PAJAK - sispras.TBL_PO_INVESTASI.DISCOUNT))/(sispras.TBL_PO_INVESTASI.TOTAL_DENGAN_PAJAK + sispras.TBL_PO_INVESTASI.DISCOUNT)) * 100), 0)AS PAJAK, sispras.TBL_PO_INVESTASI.TOTAL_DENGAN_PAJAK, sispras.MST_SUPPLIER.NAMA_SUPPLIER, sispras.TBL_PO_INVESTASI.IS_LUNAS, sispras.TBL_PO_INVESTASI.IS_REVISI, sispras.TBL_PO_INVESTASI.DISCOUNT
                        FROM        sispras.TBL_PO_INVESTASI
                        INNER JOIN  sispras.MST_SUPPLIER ON sispras.TBL_PO_INVESTASI.ID_SUPPLIER = sispras.MST_SUPPLIER.ID_SUPPLIER
                        WHERE sispras.TBL_PO_INVESTASI.IS_REVISI = 0
                    ";

                    output.data = conn.Query<dynamic>(query).ToList();

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

        public DBOutput getPurchaseOrderInvestasi(int IDPurchaseOrderInvestasi)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT	sispras.TBL_PO_INVESTASI.*, sispras.MST_SUPPLIER.*, ROUND(((((sispras.TBL_PO_INVESTASI.TOTAL_DENGAN_PAJAK + sispras.TBL_PO_INVESTASI.DISCOUNT) - (sispras.TBL_PO_INVESTASI.TOTAL_PO_TANPA_PAJAK - sispras.TBL_PO_INVESTASI.DISCOUNT))/(sispras.TBL_PO_INVESTASI.TOTAL_DENGAN_PAJAK + sispras.TBL_PO_INVESTASI.DISCOUNT)) * 100), 0)AS PAJAK_PERCENT,siatmax.REF_PROPINSI.DESKRIPSI AS PROVINSI, siatmax.REF_KAB_KODYA.DESKRIPSI AS KABUPATEN
                        FROM	sispras.TBL_PO_INVESTASI
                        INNER JOIN sispras.MST_SUPPLIER ON sispras.TBL_PO_INVESTASI.ID_SUPPLIER = sispras.MST_SUPPLIER.ID_SUPPLIER
                        INNER JOIN  siatmax.REF_PROPINSI ON sispras.MST_SUPPLIER.ID_REF_PROPINSI = siatmax.REF_PROPINSI.ID_REF_PROPINSI
                        INNER JOIN  siatmax.REF_KAB_KODYA ON sispras.MST_SUPPLIER.ID_REF_KAB_KODYA = siatmax.REF_KAB_KODYA.ID_REF_KAB_KODYA
                        WHERE   (sispras.TBL_PO_INVESTASI.ID_PO_INVESTASI = @IDPurchaseOrderInvestasi)
                    ";

                    var data = conn.QueryFirstOrDefault<dynamic>(query, new { IDPurchaseOrderInvestasi = IDPurchaseOrderInvestasi });

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

        public DBOutput getDetailPurchaseOrderInvestasi(int IDPurchaseOrderInvestasi)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT	*
                        FROM	sispras.TBL_DETAIL_PO_INVESTASI
                        WHERE   (ID_PO_INVESTASI = @IDPurchaseOrderInvestasi)
                    ";

                    var data = conn.Query<dynamic>(query, new { IDPurchaseOrderInvestasi = IDPurchaseOrderInvestasi }).ToList();

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

        public DBOutput getRekapRevisiPurchaseOrderInvestasi(string nomorPO)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT      sispras.TBL_PO_INVESTASI.NO_PO, sispras.TBL_PO_INVESTASI.TGL_PO, sispras.TBL_PO_INVESTASI.TOTAL_PO_TANPA_PAJAK, ROUND(((((sispras.TBL_PO_INVESTASI.TOTAL_DENGAN_PAJAK + sispras.TBL_PO_INVESTASI.DISCOUNT) - (sispras.TBL_PO_INVESTASI.TOTAL_PO_TANPA_PAJAK - sispras.TBL_PO_INVESTASI.DISCOUNT))/(sispras.TBL_PO_INVESTASI.TOTAL_DENGAN_PAJAK + sispras.TBL_PO_INVESTASI.DISCOUNT)) * 100), 0)AS PAJAK, sispras.TBL_PO_INVESTASI.TOTAL_DENGAN_PAJAK, sispras.MST_SUPPLIER.NAMA_SUPPLIER, sispras.TBL_PO_INVESTASI.IS_LUNAS, sispras.TBL_PO_INVESTASI.IS_REVISI, sispras.TBL_PO_INVESTASI.DISCOUNT
                        FROM        sispras.TBL_PO_INVESTASI
                        INNER JOIN  sispras.MST_SUPPLIER ON sispras.TBL_PO_INVESTASI.ID_SUPPLIER = sispras.MST_SUPPLIER.ID_SUPPLIER
                        WHERE		sispras.TBL_PO_INVESTASI.NO_PO LIKE
	                        case when charindex('-', @nomorPO) = 0 
	                        then @nomorPO
	                        else left(@nomorPO, charindex('-', @nomorPO) - 1) end + '%'
                        AND		sispras.TBL_PO_INVESTASI.IS_REVISI = 1
                    ";

                    output.data = conn.Query<dynamic>(query, new { nomorPO = nomorPO }).ToList();

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

        public List<dynamic> getListNomorPOTerimaBarang(string nomorPO)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT	sispras.TBL_PO_INVESTASI.ID_PO_INVESTASI, sispras.TBL_PO_INVESTASI.NO_PO, sispras.TBL_TERIMA_ASET.NO_INVOICE, sispras.TBL_TERIMA_ASET.TGL_TERIMA
                        FROM	sispras.TBL_PO_INVESTASI
                        LEFT JOIN sispras.TBL_TERIMA_ASET ON sispras.TBL_PO_INVESTASI.ID_PO_INVESTASI = sispras.TBL_TERIMA_ASET.ID_PO_INVESTASI
                        WHERE sispras.TBL_PO_INVESTASI.NO_PO LIKE @nomorPO + '%'
                        AND (sispras.TBL_PO_INVESTASI.IS_REVISI = 0)
                    ";

                    var data = conn.Query<dynamic>(query, new { nomorPO = nomorPO }).ToList();

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

        public DBOutput getDetailTerimaBarang(string IDPurchaseOrderInvestasi)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        if not exists(SELECT ID_TERIMA_ASET FROM sispras.TBL_TERIMA_ASET WHERE ID_PO_INVESTASI = @IDPurchaseOrderInvestasi)
	                        begin
		                        SELECT NAMA_BARANG AS NAMA_ASET, MERK, SATUAN, HARGA_SATUAN, JUMLAH, JUMLAH AS JUMLAH_MAX, (JUMLAH * HARGA_SATUAN) AS TOTAL, ID_DETAIL_PO_INVESTASI
		                        FROM sispras.TBL_DETAIL_PO_INVESTASI
		                        WHERE ID_PO_INVESTASI = @IDPurchaseOrderInvestasi
	                        end
                        else
	                        begin
                                SELECT	sispras.TBL_DETAIL_TERIMA_ASET.NAMA_ASET, sispras.TBL_DETAIL_TERIMA_ASET.MERK, sispras.TBL_DETAIL_TERIMA_ASET.SATUAN, sispras.TBL_DETAIL_TERIMA_ASET.HARGA_SATUAN, sispras.TBL_DETAIL_TERIMA_ASET.JUMLAH, sispras.TBL_DETAIL_PO_INVESTASI.JUMLAH AS JUMLAH_MAX,(sispras.TBL_DETAIL_TERIMA_ASET.JUMLAH * sispras.TBL_DETAIL_TERIMA_ASET.HARGA_SATUAN) AS TOTAL, sispras.TBL_DETAIL_TERIMA_ASET.ID_DETAIL_PO_INVESTASI
                                FROM	sispras.TBL_DETAIL_TERIMA_ASET
                                LEFT JOIN sispras.TBL_TERIMA_ASET ON sispras.TBL_DETAIL_TERIMA_ASET.ID_TERIMA_ASET = sispras.TBL_TERIMA_ASET.ID_TERIMA_ASET
                                LEFT JOIN sispras.TBL_DETAIL_PO_INVESTASI ON sispras.TBL_DETAIL_TERIMA_ASET.ID_DETAIL_PO_INVESTASI = sispras.TBL_DETAIL_PO_INVESTASI.ID_DETAIL_PO_INVESTASI
                                WHERE	sispras.TBL_TERIMA_ASET.ID_PO_INVESTASI = @IDPurchaseOrderInvestasi
                                AND sispras.TBL_DETAIL_TERIMA_ASET.JUMLAH < sispras.TBL_DETAIL_PO_INVESTASI.JUMLAH
	                        end
                    ";

                    var data = conn.Query<dynamic>(query, new { IDPurchaseOrderInvestasi = IDPurchaseOrderInvestasi }).ToList();

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

        public DBOutput addTerimaAset(TerimaAset obj)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        if not exists(select ID_TERIMA_ASET from sispras.TBL_TERIMA_ASET where ID_PO_INVESTASI = @IDPurchaseOrderInvestasi)
	                        begin
                                INSERT INTO sispras.TBL_TERIMA_ASET (ID_PO_INVESTASI, TGL_TERIMA, NO_INVOICE, TOTAL_INVOICE, JUMLAH_ITEM, USER_ID, INSERT_DATE, IP_ADDRESS, DOKUMEN_INVOICE)
                                OUTPUT INSERTED.ID_TERIMA_ASET
                                VALUES (@IDPurchaseOrderInvestasi, @tanggalTerima, @nomorInvoice, 0, 0, @userID, @insertDate, @IPAddress, @DokumenInvoiceByte)
                            end
                        else
                            begin
                                UPDATE sispras.TBL_TERIMA_ASET
                                SET NO_INVOICE = @nomorInvoice, TGL_TERIMA = @insertDate, INSERT_DATE = @tanggalTerima, DOKUMEN_INVOICE = @DokumenInvoiceByte
                                OUTPUT INSERTED.ID_TERIMA_ASET
                                WHERE  (NO_INVOICE = @nomorInvoice)
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

        public DBOutput addDetailTerimaAset(DetailTerimaAset obj)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        if not exists(select ID_DETAIL_TERIMA from sispras.TBL_DETAIL_TERIMA_ASET where ID_DETAIL_PO_INVESTASI = @IDDetailPurchaseOrderInvestasi)
	                        begin
                                INSERT INTO sispras.TBL_DETAIL_TERIMA_ASET(ID_TERIMA_ASET, MERK, SATUAN, NAMA_ASET, SPESIFIKASI, HARGA_SATUAN, JUMLAH, IS_PROCESSED, ID_DETAIL_PO_INVESTASI)
                                SELECT		@IDTerimaAset, MERK, SATUAN, NAMA_BARANG, SPESIFIKASI, HARGA_SATUAN, @jumlah, 0, ID_DETAIL_PO_INVESTASI
                                FROM        sispras.TBL_DETAIL_PO_INVESTASI
                                WHERE       (ID_DETAIL_PO_INVESTASI = @IDDetailPurchaseOrderInvestasi)
                            end
                        else
                            begin
                                UPDATE sispras.TBL_DETAIL_TERIMA_ASET
                                SET JUMLAH = @jumlah
                                WHERE  (ID_DETAIL_PO_INVESTASI = @IDDetailPurchaseOrderInvestasi)
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

        public DBOutput updateTerimaAset(int IDTerimaAset)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        UPDATE sispras.TBL_TERIMA_ASET
                        SET TOTAL_INVOICE = (SELECT	SUM(JUMLAH*HARGA_SATUAN) FROM sispras.TBL_DETAIL_TERIMA_ASET WHERE (ID_TERIMA_ASET = @IDTerimaAset)), JUMLAH_ITEM = (SELECT COUNT(*) FROM sispras.TBL_DETAIL_TERIMA_ASET WHERE (ID_TERIMA_ASET = @IDTerimaAset))
                        WHERE  (ID_TERIMA_ASET = @IDTerimaAset)
                    ";

                    output.data = conn.QueryFirstOrDefault<int>(query, new { IDTerimaAset = IDTerimaAset });

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

        public DBOutput getRekapPenerimaanBarangInvestasi()
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT  ID_TERIMA_ASET, ID_PO_INVESTASI, TGL_TERIMA, NO_INVOICE, TOTAL_INVOICE, JUMLAH_ITEM, DOKUMEN_INVOICE
                        FROM    sispras.TBL_TERIMA_ASET
                    ";

                    output.data = conn.Query<dynamic>(query).ToList();

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

        public byte[] getDokumenInvoice(int IDTerimaAset)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT DOKUMEN_INVOICE
                        FROM sispras.TBL_TERIMA_ASET
                        WHERE ID_TERIMA_ASET = @IDTerimaAset
                    ";

                    var data = conn.QueryFirstOrDefault<byte[]>(query, new { IDTerimaAset = IDTerimaAset });

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