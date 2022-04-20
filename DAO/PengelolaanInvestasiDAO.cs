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
        public DBOutput getRencanaPengadaanAset(string id_unit, string unit)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        SELECT sikeu.TBL_RKA.ID_RPKA, sikeu.TBL_RKA.ID_TAHUN_ANGGARAN, siatmax.MST_UNIT.NAMA_UNIT, sikeu.TBL_MATA_ANGGARAN.PROGRAM_KEGIATAN, sikeu.TBL_RKA.NAMA_PROGRAM, siatmax.MST_UNIT.MST_ID_UNIT, sikeu.TBL_RKA.ID_RKA
                        FROM sikeu.TBL_RPKA 
                        INNER JOIN sikeu.TBL_RKA ON sikeu.TBL_RPKA.ID_RPKA = sikeu.TBL_RKA.ID_RPKA 
                        INNER JOIN sikeu.TBL_MATA_ANGGARAN ON sikeu.TBL_RKA.ID_MT_ANGGARAN = sikeu.TBL_MATA_ANGGARAN.ID_MT_ANGGARAN
                        INNER JOIN sikeu.REF_PROGRAM ON sikeu.TBL_MATA_ANGGARAN.ID_REF_PROGRAM = sikeu.REF_PROGRAM.ID_REF_PROGRAM 
                        INNER JOIN siatmax.MST_UNIT ON sikeu.TBL_RPKA.ID_UNIT = siatmax.MST_UNIT.ID_UNIT
                        WHERE (sikeu.TBL_RKA.ID_TAHUN_ANGGARAN = 2013) AND (siatmax.MST_UNIT.MST_ID_UNIT = 14)";

                    if(unit != "KPSP")
                    {
                        query += @" WHERE ID_UNIT = @id_unit";
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
                        SELECT TBL_PENCAIRAN_INVESTASI.ID_PENCAIRAN_INVESTASI, TBL_DETAIL_PENCAIRAN_INVESTASI.ID_DETAIL_PENCAIRAN_INVESTASI , DTL_RKA.ID_DTL_RKA, DTL_RKA.ID_RKA, DTL_RKA.NAMA_KEGIATAN, DTL_RKA.BULAN, DTL_RKA.VOLUME, DTL_RKA.SATUAN, DTL_RKA.HARGA_SATUAN, DTL_RKA.SUBTOTAL
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

        public DBOutput getDetailRencanaPengadaanAset(int IDPencairanInvestasi, int IDetailPencarianInvestasi)
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
                        WHERE TPI.ID_PENCAIRAN_INVESTASI = @IDPencairanInvestasi AND DPI.ID_DETAIL_PENCAIRAN_INVESTASI = @IDetailPencarianInvestasi";

                    var data = conn.QueryFirstOrDefault<dynamic>(query, new { IDPencairanInvestasi = IDPencairanInvestasi, IDetailPencarianInvestasi = IDetailPencarianInvestasi });

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
                        INSERT INTO sispras.TBL_PENCAIRAN_INVESTASI (ID_PENCAIRAN_INVESTASI, ID_TAHUN_ANGGARAN, ID_UNIT, BULAN_PENGADAAN, TGL_PENCAIRAN, TOTAL_PENCAIRAN, INSERT_DATE, IP_ADDRESS, USER_ID, STATUS_APPROVAL, ID_DTL_RKA)
                        VALUES (@IDPencairanInvestasi, @IDTahunAnggaran, @IDUnit, @bulanPengadaan, @tanggalPencairan, @totalPencairan, @insertDate, @IPAddress, @userID, @statusApproval, @IDDetailRKA )";

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

        public DBOutput addDetailPencairanInvestasi(DetailRencanaPengadaanAset obj)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        INSERT INTO sispras.TBL_DETAIL_PENCAIRAN_INVESTASI (ID_PENCAIRAN_INVESTASI, ID_DETAIL_PENCAIRAN_INVESTASI,  ID_KATEGORI, ID_REF_SK, SATUAN, HARGA_SATUAN, JUMLAH, IMAGE_BARANG, IS_PO, SPESIFIKASI, NAMA_PENGADAAN, MERK)
                        VALUES (@IDPencairanInvestasi, @IDDetailPencairanInvestasi, @IDKategori, @IDRefSK, @satuan, @hargaSatuan, @jumlah, @imageBarang, @isPO, @spesifikasi, @namaPengadaan, @merk)";

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

        public DBOutput updateDetailPencairanInvestasi (DetailRencanaPengadaanAset obj)
        {
            DBOutput output = new DBOutput();
            output.status = true;

            using (SqlConnection conn = new SqlConnection(DBConnection.db_sispras))
            {
                try
                {
                    string query = @"
                        UPDATE sispras.TBL_DETAIL_PENCAIRAN_INVESTASI 
                        SET ID_PENCAIRAN_INVESTASI = @IDPencairanInvestasi, ID_DETAIL_PENCAIRAN_INVESTASI = @IDDetailPencairanInvestasi,  ID_KATEGORI = @IDKategori, ID_REF_SK = @IDRefSK, SATUAN = @satuan, HARGA_SATUAN = @hargaSatuan, JUMLAH = @jumlah, IMAGE_BARANG = @imageBarang, IS_PO = @isPO, SPESIFIKASI = @spesifikasi, NAMA_PENGADAAN = @namaPengadaan, MERK = @merk
                        WHERE ID_PENCAIRAN_INVESTASI = @IDPencairanInvestasi AND ID_DETAIL_PENCAIRAN_INVESTASI = @IDDetailPencairanInvestasi";

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

                    var data = conn.QueryFirstOrDefault<dynamic>(query, new { IDRefSK = IDRefSK });

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
    }
}