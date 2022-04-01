// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {

    $('#testing').on('click', function () {
        toastr.error('Lorem ipsum dolor sit amet, consetetur sadipscing elitr.')
    });

    $('#showModalDetailRencanaPengadaanAset').on('click', function () {
        $('#DetailRencanaKhususInvestasiModal').modal('hide');
        $('#DetailRencanaPengadaanAset').modal('show');
    });

    $('#showDetailRencanaKhususInvestasiModal').on('click', function () {
        var id = $(this).data('id');
        var tabelDetailRencanaKhususInvestasi;

        $.ajax({
            url: "/PengelolaanInvestasi/ajaxGetDetailRencanaKhususInvestasi",
            type: "POST",
            data: { "id": id },
            dataType: "json",
            success: function (data) {
                var data = result.data;

                for (var i = 0; i < data.length; i++) {
                    str += '<tr> <td>data[i].NAMA_KEGIATAN</td> <td>data[i].BULAN</td> <td>data[i].VOLUME</td> <td>data[i].SATUAN</td> <td>data[i].HARGA_SATUAN</td> <td>data[i].SUBTOTAL</td> <td> <button id="showModalDetailRencanaPengadaanAset" type="button" class="btn btn-primary btn-block btn-sm" data-id="data[i].ID_DTL_RKA"><i class="fa fa-edit"></i></button> </td> </tr>';
                }

                tabelDetailRencanaKhususInvestasi.destroy();
                $(".tbodyDetailRencanaKhususInvestasi").html(str);
            }
        })
    });



})