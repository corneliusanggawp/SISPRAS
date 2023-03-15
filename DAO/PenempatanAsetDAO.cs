using Dapper;
using SISPRAS.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SISPRAS.DAO
{
    public class PenempatanAsetDAO
    {
        public DBOutput getRekapDetailTerimaAset()
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT  ID_DETAIL_TERIMA, NAMA_ASET, MERK, SATUAN, JUMLAH
                        FROM    sispras.TBL_DETAIL_TERIMA_ASET
                        WHERE   (IS_PROCESSED = 0)
                        AND		JUMLAH > 0
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

        public DBOutput addAset(Aset obj)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        DECLARE @i AS INT = 0

                        WHILE @i < @jumlah
                        BEGIN
                            INSERT INTO sispras.MST_ASET(ID_REF_SK_SIASET, ID_KATEGORI, KODE_ASET_SIASET, ID_UNIT, ID_REF_STATUS_KEPEMILIKAN, ID_MST_RUANG, NAMA_BARANG, MERK, HARGA_BELI, SPESIFIKASI, STATUS, NO_DOKUMEN, NOMOR_GARANSI, ID_REF_GOL_AKTIVA, TGL_DITERIMA)
                            VALUES (@IDRefSK, @IDKategori, (SELECT REPLACE((CAST(YEAR(@tanggalDiterima) as VARCHAR(10))+ '-' + REF_SUB_KATEGORI.KODE_BARANG + '-' + REF_SUB_KATEGORI.KODE_JENIS_BARANG + '-' + CAST((SELECT COUNT(MST_ASET.ID_ASSET) + 1 FROM sispras.MST_ASET MST_ASET WHERE YEAR(MST_ASET.TGL_DITERIMA) = YEAR(@tanggalDiterima)) AS VARCHAR(10))), ' ', '') FROM sispras.REF_SUB_KATEGORI REF_SUB_KATEGORI WHERE REF_SUB_KATEGORI.ID_REF_SK = @IDRefSK), @IDUnit, @IDRefStatusKepemilikan, @IDMSTRuang, @namaBarang, @merk, @hargaBeli, @spesifikasi, @status, @nomorDokumen, @nomorGaransi, @IDRefGolonganAktiva, @tanggalDiterima)
	                        SET @i = @i + 1
                        END
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

        public DBOutput updateAset(int IDAset, string nomorDokumen, string nomorGaransi, decimal hargaBeli)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        UPDATE sispras.MST_ASET
                        SET NO_DOKUMEN = @nomorDokumen, HARGA_BELI = @hargaBeli, NOMOR_GARANSI = @nomorGaransi
                        WHERE (ID_ASSET = @IDAset)
                    ";

                    output.data = conn.Execute(query, new { IDAset = IDAset, nomorDokumen = nomorDokumen, nomorGaransi = nomorGaransi, hargaBeli = hargaBeli });

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

        public DBOutput generateAset(int IDDetailTerimaAset, int IDRefGolonganAktiva, int IDRefStatusKepemilikan, string nomorDokumen, string nomorGaransi, string status, string tanggalDiterima)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        DECLARE @jumlahAset AS INT = (SELECT JUMLAH FROM sispras.TBL_DETAIL_TERIMA_ASET WHERE ID_DETAIL_TERIMA = @IDDetailTerimaAset)
                        DECLARE @i AS INT = 0				

                        WHILE @i < @jumlahAset
                        BEGIN
	                        INSERT INTO sispras.MST_ASET(ID_REF_SK_SIASET, ID_KATEGORI, KODE_ASET_SIASET, ID_UNIT, ID_REF_STATUS_KEPEMILIKAN, ID_MST_RUANG, NAMA_BARANG, MERK, HARGA_BELI, SPESIFIKASI, STATUS, KONDISI_BARANG, NO_DOKUMEN, NOMOR_GARANSI, ID_REF_GOL_AKTIVA, TGL_DITERIMA)
                            SELECT ID_REF_SK, ID_KATEGORI, (SELECT REPLACE((CAST(YEAR(@tanggalDiterima) as VARCHAR(10))+ '-' + REF_SUB_KATEGORI.KODE_BARANG + '-' + REF_SUB_KATEGORI.KODE_JENIS_BARANG + '-' + CAST((SELECT COUNT(MST_ASET.ID_ASSET) + 1 FROM sispras.MST_ASET MST_ASET WHERE YEAR(MST_ASET.TGL_DITERIMA) = YEAR(@tanggalDiterima)) AS VARCHAR(10))), ' ', '') FROM sispras.REF_SUB_KATEGORI REF_SUB_KATEGORI WHERE REF_SUB_KATEGORI.ID_REF_SK = 184), ID_UNIT, @IDRefStatusKepemilikan, 3086, sispras.TBL_DETAIL_TERIMA_ASET.NAMA_ASET, sispras.TBL_DETAIL_TERIMA_ASET.MERK, sispras.TBL_DETAIL_TERIMA_ASET.HARGA_SATUAN, sispras.TBL_DETAIL_TERIMA_ASET.SPESIFIKASI, @status, 'Baik Digunakan', @nomorDokumen, @nomorGaransi, @IDRefGolonganAktiva, @tanggalDiterima
                            FROM        sispras.TBL_DETAIL_TERIMA_ASET
                            INNER JOIN	sispras.TBL_TERIMA_ASET ON sispras.TBL_DETAIL_TERIMA_ASET.ID_TERIMA_ASET = sispras.TBL_TERIMA_ASET.ID_TERIMA_ASET
                            INNER JOIN	sispras.TBL_DETAIL_PO_INVESTASI ON sispras.TBL_DETAIL_TERIMA_ASET.ID_DETAIL_PO_INVESTASI = sispras.TBL_DETAIL_PO_INVESTASI.ID_DETAIL_PO_INVESTASI
                            INNER JOIN	sispras.TBL_DETAIL_PENCAIRAN_INVESTASI ON sispras.TBL_DETAIL_PO_INVESTASI.ID_DETAIL_PENCAIRAN_INVESTASI = sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.ID_DETAIL_PENCAIRAN_INVESTASI
                            WHERE (sispras.TBL_DETAIL_TERIMA_ASET.ID_DETAIL_TERIMA = @IDDetailTerimaAset)

	                        SET @i = @i + 1
                        END

                        UPDATE sispras.TBL_DETAIL_TERIMA_ASET
                        SET IS_PROCESSED = 1
                        WHERE (ID_DETAIL_TERIMA = @IDDetailTerimaAset)
                    ";

                    output.data = conn.QueryFirstOrDefault<int>(query, new { IDDetailTerimaAset = IDDetailTerimaAset, IDRefGolonganAktiva = IDRefGolonganAktiva, IDRefStatusKepemilikan = IDRefStatusKepemilikan, nomorDokumen = nomorDokumen, nomorGaransi = nomorGaransi, status = status, tanggalDiterima = tanggalDiterima });

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

        public DBOutput getAllMasterAset()
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT sispras.MST_ASET.ID_ASSET, sispras.MST_ASET.KODE_ASET_SIASET AS KODE_ASET, sispras.MST_ASET.NAMA_BARANG, sispras.MST_ASET.MERK, sispras.REF_SUB_KATEGORI.DESKRIPSI AS SUB_KATEGORI, sispras.MST_ASET.TGL_DITERIMA, sispras.REF_GOLONGAN_AKTIVA.DESKRIPSI AS AKTIVA, sispras.MST_ASET.HARGA_BELI, sispras.MST_ASET.SPESIFIKASI, sispras.MST_ASET.KONDISI_BARANG, sispras.MST_ASET.NO_DOKUMEN, sispras.MST_ASET.ID_REF_SK_SIASET, sispras.MST_ASET.ID_MST_RUANG, sispras.MST_ASET.ID_UNIT
                        FROM sispras.MST_ASET 
                        INNER JOIN sispras.REF_SUB_KATEGORI ON sispras.MST_ASET.ID_REF_SK_SIASET = sispras.REF_SUB_KATEGORI.ID_REF_SK
                        INNER JOIN sispras.REF_GOLONGAN_AKTIVA ON sispras.MST_ASET.ID_REF_GOL_AKTIVA = sispras.REF_GOLONGAN_AKTIVA.ID_REF_GOL_AKTIVA
                        ORDER BY sispras.MST_ASET.KODE_ASET_SIASET DESC
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

        public DBOutput getDetailAset(int IDAset)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT NO_DOKUMEN, HARGA_BELI, NOMOR_GARANSI
                        FROM   sispras.MST_ASET
                        WHERE (ID_ASSET = @IDAset)
                    ";

                    output.data = conn.QueryFirstOrDefault<dynamic>(query, new { IDAset = IDAset });
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

        public List<dynamic> getAllGolonganAktiva()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT  *
                        FROM    sispras.REF_GOLONGAN_AKTIVA
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

        public List<dynamic> getAllStatusKepemilikan()
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT  *
                        FROM    sispras.REF_STATUS_KEPEMILIKAN
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

        public List<dynamic> getRuangan(string IDUnit)
        {
            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT ID_MST_RUANG, NAMA_RUANG
                        FROM sispras.MST_RUANG_BANGUNAN
                        WHERE ID_UNIT = @IDUnit
                    ";
                    var data = conn.Query<dynamic>(query, new { IDUnit = IDUnit }).ToList();

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
