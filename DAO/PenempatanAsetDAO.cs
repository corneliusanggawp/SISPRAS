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

        public DBOutput generateAset(int IDDetailTerimaAset)
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
	                        INSERT INTO sispras.MST_ASET(ID_REF_SK, ID_KATEGORI, KODE_ASET, ID_UNIT, ID_REF_STATUS_KEPEMILIKAN, ID_MST_RUANG, NAMA_BARANG, MERK, HARGA_BELI, SPESIFIKASI, STATUS, KONDISI_BARANG, NO_DOKUMEN, NOMOR_GARANSI, ID_REF_GOL_AKTIVA, TGL_DITERIMA)
	                        SELECT      ID_REF_SK, ID_KATEGORI, NULL, ID_UNIT, 1, NULL, NAMA_BARANG, sispras.TBL_DETAIL_TERIMA_ASET.MERK, sispras.TBL_DETAIL_TERIMA_ASET.HARGA_SATUAN, NULL, 'Ada', 'Baik digunakan', NULL, NULL, NULL, TGL_TERIMA
	                        FROM        sispras.TBL_DETAIL_TERIMA_ASET
	                        INNER JOIN	sispras.TBL_TERIMA_ASET ON sispras.TBL_DETAIL_TERIMA_ASET.ID_TERIMA_ASET = sispras.TBL_TERIMA_ASET.ID_TERIMA_ASET
	                        INNER JOIN	sispras.TBL_DETAIL_PO_INVESTASI ON sispras.TBL_DETAIL_TERIMA_ASET.ID_DETAIL_PO_INVESTASI = sispras.TBL_DETAIL_PO_INVESTASI.ID_DETAIL_PO_INVESTASI
	                        INNER JOIN	sispras.TBL_DETAIL_PENCAIRAN_INVESTASI ON sispras.TBL_DETAIL_PO_INVESTASI.ID_DETAIL_PENCAIRAN_INVESTASI = sispras.TBL_DETAIL_PENCAIRAN_INVESTASI.ID_DETAIL_PENCAIRAN_INVESTASI
	                        WHERE       (sispras.TBL_DETAIL_TERIMA_ASET.ID_DETAIL_TERIMA = @IDDetailTerimaAset)
	
	                        SET @i = @i + 1
                        END

                        UPDATE sispras.TBL_DETAIL_TERIMA_ASET
                        SET IS_PROCESSED = 1
                        WHERE (ID_DETAIL_TERIMA = @IDDetailTerimaAset)
                    ";

                    output.data = conn.QueryFirstOrDefault<int>(query, new { IDDetailTerimaAset = IDDetailTerimaAset });

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
