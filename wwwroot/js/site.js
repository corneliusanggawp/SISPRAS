// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {
    $("#example1").DataTable({
        "responsive": true, "lengthChange": false, "autoWidth": false,
        "buttons": ["copy", "csv", "excel", "pdf", "print", "colvis"]
    }).buttons().container().appendTo('#example1_wrapper .col-md-6:eq(0)');

    $('#example2').DataTable({
        "paging": true,
        "lengthChange": false,
        "searching": false,
        "ordering": true,
        "info": true,
        "autoWidth": false,
        "responsive": true,
    });

    $('.select2').select2()

    //Initialize Select2 Elements
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    })

    $('#testing').on('click', function () {
        toastr.error('Lorem ipsum dolor sit amet, consetetur sadipscing elitr.')
    });

    $('#showModalDetailRencanaPengadaanAset').on('click', function () {
        $('#DetailRencanaKhususInvestasiModal').modal('hide');
        $('#DetailRencanaPengadaanAset').modal('show');
    });

    $('#showDetailRencanaKhususInvestasiModal').on('click', function () {
        $.ajax({
            url: "PengelolaanInvestasi/ajaxGetDetailRencanaKhususInvestasi",
            type: "POST",
            data: { "id": id },
            dataType: "json",
            success: function (data) {
                var RencanaKhususInvestasiModal = result.data;
            }
        })
    });



})