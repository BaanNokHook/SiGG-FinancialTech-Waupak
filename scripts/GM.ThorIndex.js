$.ajaxSetup({
    cache: false
});

$(document).ready(function () {

    $("#NavBar").html($('#NavThorIndex').val());

    $("#ddl_instrument_code").click(function () {
        var txt_search = $('#txt_instrument_code');
        var data = { text: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_instrument_code').keyup(function () {
        var data = { text: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    }

    var momentDate = moment($("#BusinessDate").text(), "DD/MM/YYYY");

    $('#FormSearch_asof_date_from').data("DateTimePicker").date(momentDate);
    $('#FormSearch_asof_date_from').val(momentDate.format("DD/MM/YYYY"));

    $('#FormSearch_asof_date_to').data("DateTimePicker").date(momentDate);
    $('#FormSearch_asof_date_to').val(momentDate.format("DD/MM/YYYY"));

    GM.ThorIndex = {};
    GM.ThorIndex.Table = $('#x-table-data').DataTable({
        dom: 'Bfrtip',
        select: false,
        searching: true,
        scrollY: '80vh',
        scrollX: true,
        order: [
            [2, "asc"]
        ],
        buttons: [],
        processing: true,
        serverSide: true,
        ajax: {
            "url": "/ThorIndex/Search",
            "type": "POST",
            "error": function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR);
                console.log(textStatus);
                console.log(errorThrown);
            }
        },
        columnDefs:
            [
                { targets: 0, data: "RowNumber", orderable: false },
                { targets: 1, data: "instrument_code" },
                {
                    targets: 2, data: "next_business_date", render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data
                    }
                },
                {
                    targets: 3, data: "event_date", render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data
                    }
                },
                {
                    targets: 4, data: "thor_date", render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data
                    }
                },
                { targets: 5, data: "thor_rate" },
                { targets: 6, data: "ai_index_eod_accum" },
                { targets: 7, data: "compound_type" },
                { targets: 8, data: "day_count" },
                { targets: 9, data: "is_holiday", orderable: false },
                { targets: 10, data: "instrument_id", visible: false },
                { targets: 11, data: "asof_date_from", visible: false },
                { targets: 12, data: "asof_date_to", visible: false }
            ],
        fixedColumns: {
            leftColumns: 1,
            rightColumns: 3

        },
        fnPreDrawCallback: function () {
            $('#x-table-data').DataTable().columns(10).search($('#FormSearch_instrument_id').val());
            $('#x-table-data').DataTable().columns(11).search($('#FormSearch_asof_date_from').val());
            $('#x-table-data').DataTable().columns(12).search($('#FormSearch_asof_date_to').val());
        }

    });

    GM.ThorIndex.Search = function () {
        $('#x-table-data').DataTable().columns(10).search($('#FormSearch_instrument_id').val());
        $('#x-table-data').DataTable().columns(11).search($('#FormSearch_asof_date_from').val());
        $('#x-table-data').DataTable().columns(12).search($('#FormSearch_asof_date_to').val());
        GM.ThorIndex.Table.draw();
    };

    GM.ThorIndex.Export = function (btn) {
        swal({
            title: "Do you want to Export?",
            text: "",
            html: true,
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes",
            cancelButtonText: "No",
            closeOnConfirm: true,
            closeOnCancel: true
        },
            function (isConfirm) {
                if (isConfirm) {

                    $('.spinner').css('display', 'block');

                    $.post('/ThorIndex/ExportData', { asof_date_from: $('#FormSearch_asof_date_from').val(), asof_date_to: $('#FormSearch_asof_date_to').val(), instrument_id: $('#FormSearch_instrument_id').val() })
                        .done(function (response) {
                            if (response.errorMessage === '') {
                                window.location.href = '/ThorIndex/Download?filename=' + response.fileName;
                                $('.spinner').css('display', 'none');
                                setTimeout(function () {

                                    swal({
                                        title: "Complete",
                                        text: "Gen Excel Successfully ",
                                        type: "success",
                                        showCancelButton: false,
                                        confirmButtonClass: "btn-success",
                                        confirmButtonText: "Ok",
                                        closeOnConfirm: true
                                    }
                                    );
                                }, 100);
                            } else {
                                setTimeout(function () {
                                    swal("Warning", response.errorMessage, "warning");
                                }, 100);
                                $('.spinner').css('display', 'none');
                            }
                        });
                }
            }
        )
    };

    $("#search-form").on('submit', function (e) {
        e.preventDefault();
        GM.Message.Clear();
        GM.ThorIndex.Search();
    });

    $("#search-form").on('reset', function (e) {
        GM.Defer(function () {
            $("#ddl_instrument_code").find(".selected-data").text("Select...");
            $("#instrument_code").val(null);
            $("#instrument_code").text(null);
            $("#instrument_id").val(null);
            $("#instrument_id").text(null);
            $('#FormSearch_asof_date_from').data("DateTimePicker").date(null);
            $('#FormSearch_asof_date_to').data("DateTimePicker").date(null);
            GM.Message.Clear();
            GM.ThorIndex.Search();
        }, 100);
    });

    $("#btnSync").on("click", function () {
        swal({
            title: "Comfirm Sync ThorIndex?",
            text: "",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes",
            cancelButtonText: "No",
            closeOnConfirm: true,
            closeOnCancel: true
        },
            function (isConfirm) {
                if (isConfirm) {
                    $('.spinner').css('display', 'block');

                    var data = { asof_date: $("#BusinessDate").text() };

                    $.ajax({
                        url: "/Admin/InterfaceThorIndex",
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        cache: false,
                        dataType: 'JSON',
                        traditional: true,
                        data: JSON.stringify(data),
                        success: function (res) {
                            $('.spinner').css('display', 'none');
                            if (res.Status == "Success") {
                                swal("Success!", res.Message, "success");
                            }
                            else if (res.Status == "Error") {
                                swal("Fail", res.Message, "error");
                            }

                        },
                        error: function (jqXhr, textStatus) {
                            $('.spinner').css('display', 'none');
                            if (textStatus === "error") {
                                var objJson = jQuery.parseJSON(jqXhr.responseText);

                                if (Object.prototype.toString.call(objJson) === '[object Array]' &&
                                    objJson.length == 0) {
                                    // Array is empty
                                    // Do Something
                                } else {
                                    var errorMsg = jqXhr.statusText + " " + objJson.Message;
                                    swal("Fail", errorMsg, "error");
                                }
                            }
                        }
                    });
                }
            }
        );
    });
});


