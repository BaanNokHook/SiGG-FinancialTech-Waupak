$(document).ready(function () {
    $("#NavBar").html($('#NavStockReconcile').val());

    var budate = $("#BusinessDate").text();

    var formatmmddyyyydate = budate.split("/");
    formatmmddyyyydate = formatmmddyyyydate[1] + "/" + formatmmddyyyydate[0] + "/" + formatmmddyyyydate[2];
    var business_date = new Date(formatmmddyyyydate);

    console.log("business_date : " + business_date);
    console.log("business_date : " + business_date.addDays(-1));

    $('#FormSearch_as_of_date').data("DateTimePicker").date(business_date.addDays(-1));

    $("#ddl_instrument_code").click(function () {
        var txt_search = $("#txt_instrument_code");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_instrument_code").keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    };

    GM.StockReconcile = {};
    GM.StockReconcile.Table = $('#x-table-data').DataTable({
        dom: 'Bfrtip',
        select: false,
        searching: true,
        paging: false,
        scrollY: '45vh',
        scrollX: true,
        order: [
            [1, "asc"]
        ],
        buttons:
            [
            ],
        processing: true,
        serverSide: true,
        ajax: {
            "url": "/StockReconcile/Search",
            "type": "POST",
            "statusCode": {
                401: function () {
                    window.location.href = "/StockReconcile";
                }
            },
            "error":
                function (error) {
                    console.log(error);
                }
        },
        columnDefs:
            [
                { targets: 0, data: "RowNumber", orderable: false },
                { targets: 1, data: "instrument_code" },
                {
                    targets: 2, data: "afs_unit",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data !== null) {
                            if (data < 0) {
                                html = '<span style="color: red;"> (' + FormatDecimalValidate(data * -1, 2) + ')</span> ';
                            }
                            else {
                                html = FormatDecimalValidate(data, 2);
                            }
                        }
                        return html;
                    }
                },
                {
                    targets: 3, data: "htm_unit",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data !== null) {
                            if (data < 0) {
                                html = '<span style="color: red;"> (' + FormatDecimalValidate(data * -1, 2) + ')</span> ';
                            }
                            else {
                                html = FormatDecimalValidate(data, 2);
                            }
                        }
                        return html;
                    }
                },
                {
                    targets: 4, data: "memo_bnk_unit",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data !== null) {
                            if (data < 0) {
                                html = '<span style="color: red;"> (' + FormatDecimalValidate(data * -1, 2) + ')</span> ';
                            }
                            else {
                                html = FormatDecimalValidate(data, 2);
                            }
                        }
                        return html;
                    }
                },
                {
                    targets: 5, data: "memo_trd_unit",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data !== null) {
                            if (data < 0) {
                                html = '<span style="color: red;"> (' + FormatDecimalValidate(data * -1, 2) + ')</span> ';
                            }
                            else {
                                html = FormatDecimalValidate(data, 2);
                            }
                        }
                        return html;
                    }
                },
                {
                    targets: 6, data: "total_unit",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data !== null) {
                            if (data < 0) {
                                html = '<span style="color: red;"> (' + FormatDecimalValidate(data * -1, 2) + ')</span> ';
                            }
                            else {
                                html = FormatDecimalValidate(data, 2);
                            }
                        }
                        return html;
                    }
                },
                {
                    targets: 7, data: "outstanding_unit",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data !== null) {
                            if (data < 0) {
                                html = '<span style="color: red;"> (' + FormatDecimalValidate(data * -1, 2) + ')</span> ';
                            }
                            else {
                                html = FormatDecimalValidate(data, 2);
                            }
                        }
                        return html;
                    }
                },
                {
                    targets: 8, data: "obligate_unit",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data !== null) {
                            if (data < 0) {
                                html = '<span style="color: red;"> (' + FormatDecimalValidate(data * -1, 2) + ')</span> ';
                            }
                            else {
                                html = FormatDecimalValidate(data, 2);
                            }
                        }
                        return html;
                    }
                },
                {
                    targets: 9, data: "diff_unit",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data !== null) {
                            if (data < 0) {
                                html = '<span style="color: red;"> (' + FormatDecimalValidate(data * -1, 2) + ')</span> ';
                            }
                            else {
                                html = FormatDecimalValidate(data, 2);
                            }
                        }
                        return html;
                    }
                },
                {
                    targets: 10, data: "remark", orderable: false
                },
                {
                    targets: 11, data: "instrument_id",
                    className: 'dt-body-center',
                    searchable: false,
                    orderable: false,
                    width: 10,
                    render: function (data, type, row, meta) {
                        var html = '<button class="btn btn-default btn-round" key="' + row.instrument_code + '" asof_date="' + moment(row.as_of_date).format('DD/MM/YYYY') + '" form-mode="edit" onclick="GM.StockReconcile.Form(this)" ><i class="feather-icon icon-edit"></i></button>';
                        return html;
                    }
                },
                { targets: 12, data: "as_of_date", "visible": false },
                { targets: 13, data: "instrument_id", "visible": false },
                { targets: 14, data: "import_id", "visible": false }
            ],
        fixedColumns: {
            leftColumns: 1,
            rightColumns: 4
        },
        footerCallback: function (row, data, start, end, display) {
            var api = this.api();
            //Remove the formatting to get integer data for summation
            var intVal = function (i) {
                return typeof i === 'string' ?
                    i.replace(/[\$,]/g, '') * 1 :
                    typeof i === 'number' ?
                        i : 0;
            };

            // computing column Total of the complete result 

            var afs = api
                .column(2)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            var htm = api
                .column(3)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            var memo_bnk = api
                .column(4)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            var memo_trd = api
                .column(5)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            var total = api
                .column(6)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            var out = api
                .column(7)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            var obl = api
                .column(8)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            var dif = api
                .column(9)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);

            $(api.column(2).footer()).attr('style', 'text-align: right');
            $(api.column(3).footer()).attr('style', 'text-align: right');
            $(api.column(4).footer()).attr('style', 'text-align: right');
            $(api.column(5).footer()).attr('style', 'text-align: right');
            $(api.column(6).footer()).attr('style', 'text-align: right');
            $(api.column(7).footer()).attr('style', 'text-align: right');
            $(api.column(8).footer()).attr('style', 'text-align: right');
            $(api.column(9).footer()).attr('style', 'text-align: right');
            $(api.column(1).footer()).html('Total');
            var html;
            if (afs < 0) {
                html = '<span style="color: red;"> (' + FormatDecimalValidate(data * -1, 2) + ')</span> ';
            }
            else {
                html = FormatDecimalValidate(data, 2);
            }

            $(api.column(2).footer()).html(afs < 0 ? '<span style="color: red;"> (' + FormatDecimalValidate(afs * -1, 2) + ')</span> ' : FormatDecimalValidate(afs, 2));
            $(api.column(3).footer()).html(htm < 0 ? '<span style="color: red;"> (' + FormatDecimalValidate(htm * -1, 2) + ')</span> ' : FormatDecimalValidate(htm, 2));
            $(api.column(4).footer()).html(memo_bnk < 0 ? '<span style="color: red;"> (' + FormatDecimalValidate(memo_bnk * -1, 2) + ')</span> ' : FormatDecimalValidate(memo_bnk, 2));
            $(api.column(5).footer()).html(memo_trd < 0 ? '<span style="color: red;"> (' + FormatDecimalValidate(memo_trd * -1, 2) + ')</span> ' : FormatDecimalValidate(memo_trd, 2));
            $(api.column(6).footer()).html(total < 0 ? '<span style="color: red;"> (' + FormatDecimalValidate(total * -1, 2) + ')</span> ' : FormatDecimalValidate(total, 2));
            $(api.column(7).footer()).html(out < 0 ? '<span style="color: red;"> (' + FormatDecimalValidate(out * -1, 2) + ')</span> ' : FormatDecimalValidate(out, 2));
            $(api.column(8).footer()).html(obl < 0 ? '<span style="color: red;"> (' + FormatDecimalValidate(obl * -1, 2) + ')</span> ' : FormatDecimalValidate(obl, 2));
            $(api.column(9).footer()).html(dif < 0 ? '<span style="color: red;"> (' + FormatDecimalValidate(dif * -1, 2) + ')</span> ' : FormatDecimalValidate(dif, 2));
        },
        fnPreDrawCallback: function () {
            $('#x-table-data').DataTable().columns(12).search($('#FormSearch_as_of_date').val());
        }
    });

    GM.StockReconcile.Search = function () {
        try {
            var asof_date = $("#FormSearch_as_of_date").val();
            var instrument_id = $("#FormSearch_instrument_id").val();

            GM.StockReconcile.Table.columns(12).search(asof_date);
            GM.StockReconcile.Table.columns(13).search(instrument_id);
            GM.StockReconcile.Table.draw();
        } catch (err) {
            console.log(err.message);
        }
    };

    GM.StockReconcile.Form = function (btn) {
        GM.StockReconcile.Form.Clear();
        var key = $(btn).attr("key");
        var asof_date = $(btn).attr("asof_date");

        console.log('asof_date : ' + asof_date);

        var data = {
            instrument_code: key,
            as_of_date: asof_date
        };

        $.ajax({
            url: "/StockReconcile/GetStockReconcile",
            type: "GET",
            dataType: "JSON",
            data: data,
            success: function (res) {
                if (res.Success && res.Data !== null && res.Data.StockReconcileListResultModel.length > 0) {
                    $('#modal_instrument_code').text(res.Data.StockReconcileListResultModel[0].instrument_code);
                    $('#modal_afs').text(FormatDecimalValidate(res.Data.StockReconcileListResultModel[0].afs_unit, 2));
                    $('#modal_htm').text(FormatDecimalValidate(res.Data.StockReconcileListResultModel[0].htm_unit, 2));
                    $('#modal_memo_bnk').text(FormatDecimalValidate(res.Data.StockReconcileListResultModel[0].memo_bnk_unit, 2));
                    $('#modal_memo_trd').text(FormatDecimalValidate(res.Data.StockReconcileListResultModel[0].memo_trd_unit, 2));
                    $('#modal_total').text(FormatDecimalValidate(res.Data.StockReconcileListResultModel[0].total_unit, 2));
                    $('#modal_out').text(FormatDecimalValidate(res.Data.StockReconcileListResultModel[0].outstanding_unit, 2));
                    $('#modal_obligate').text(FormatDecimalValidate(res.Data.StockReconcileListResultModel[0].obligate_unit, 2));
                    $('#modal_dif').text(FormatDecimalValidate(res.Data.StockReconcileListResultModel[0].diff_unit, 2));
                    $('#modal_remark').val(res.Data.StockReconcileListResultModel[0].remark);

                    $('#modal_import_id').val(res.Data.StockReconcileListResultModel[0].import_id);
                    $('#modal_instrument_id').val(res.Data.StockReconcileListResultModel[0].instrument_id);
                    $('#modal_as_of_date').val(res.Data.StockReconcileListResultModel[0].as_of_date);

                    $('#action-form-modal').modal('show');
                }
            }
        });
    };

    GM.StockReconcile.Form.Clear = function () {

        $('#modal_instrument_code').text(null);
        $('#modal_afs').text(null);
        $('#modal_htm').text(null);
        $('#modal_memo_bnk').text(null);
        $('#modal_memo_trd').text(null);
        $('#modal_total').text(null);
        $('#modal_out').text(null);
        $('#modal_obligate').text(null);
        $('#modal_dif').text(null);
        $('#modal_remark').val(null);

        $('#modal_import_id').val(null);
        $('#modal_instrument_id').val(null);
        $('#modal_as_of_date').val(null);
    };


    GM.StockReconcile.Submit = function () {
        console.log($('#modal_remark').val());

        swal({
            title: "Comfirm Save?",
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

                    var data = {
                        instrument_code: $('#modal_instrument_code').text(),
                        as_of_date: $('#modal_as_of_date').val(),
                        afs_unit: $('#modal_afs').text(),
                        htm_unit: $('#modal_htm').text(),
                        memo_bnk_unit: $('#modal_memo_bnk').text(),
                        memo_trd_unit: $('#modal_memo_trd').text(),
                        total_unit: $('#modal_total').text(),
                        outstanding_unit: $('#modal_out').text(),
                        obligate_unit: $('#modal_obligate').text(),
                        diff_unit: $('#modal_dif').text(),
                        import_id: $('#modal_import_id').val(),
                        instrument_id: $('#modal_instrument_id').val(),
                        remark: $('#modal_remark').val()
                    };


                    $('.spinner').css('display', 'block'); // Open Loading

                    $.ajax({
                        url: "/StockReconcile/Save",
                        type: "POST",
                        dataType: "JSON",
                        data: data,
                        success: function (res) {
                            setTimeout(
                                function () {
                                    swal({
                                        title: "Complete",
                                        text: "Save Successfully",
                                        type: "success",
                                        html: true,
                                        showCancelButton: false,
                                        confirmButtonClass: "btn-success",
                                        confirmButtonText: "Ok",
                                        closeOnConfirm: true
                                    },
                                        function (isConfirm) {
                                            $('.spinner').css('display', 'none'); // Close Loading
                                            $('#action-form-modal').modal('hide');
                                            GM.StockReconcile.Search();
                                        });
                                }, 100);
                            
                        }
                    });

                }
            }
        );
    };

    $('#btnExcel').click(function () {
        $('#FormDownload_instrument_id').val($('#FormSearch_instrument_id').val());
        $('#FormDownload_asofdate').val($('#FormSearch_as_of_date').val());

        $('#download-form').submit();
    });

});

