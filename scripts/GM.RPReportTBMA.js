function ExportExcel(type) {
    if (type == 'Export') {
        var rowData = $('#x-table-data').DataTable().rows($('#x-table-data .filter-ck:checked').closest('tr')).data();
        var transNoList = [];
        for (var i = 0; i < rowData.length; i++) {
            //console.log(rowData[i].trans_no);
            transNoList.push(rowData[i].trans_no);
        }

        if (transNoList.length > 0) {
            var dataToPost = { id: JSON.stringify(transNoList) };
            //$.post('/RPReportTBMA/GenExcel', dataToPost)
            //    .done(function (response) {
            //       window.location.href =  '/RPReportTBMA/Download?filename=' + response.fileName;
            //    });

            $('#downloadId').val(transNoList);
            $('#downloadExportType').val('Export');
            $('#download-form').submit();
            GM.RPReportTBMA.Form.Search();
        }
        else {
            swal("Warning", "Please Select [Trans No] To Report TBMA", "error");
        }
    }
    else if (type == 'ExportAll') {
        $('#downloadId').val('');
        $('#downloadFromDate').val($('#FormSearch_from_trade_date').val());
        $('#downloadToDate').val($('#FormSearch_to_trade_date').val());
        $('#downloadTransDealType').val($('#FormSearch_trans_deal_type').val());
        $('#downloadCounterPartyCode').val($('#FormSearch_counter_party_code').val());
        $('#downloadPort').val($('#FormSearch_port').val());
        $('#downloadExportType').val('ExportAll');
        $('#download-form').submit();
        GM.RPReportTBMA.Form.Search();
    }
}

$(document).ready(function () {
    var budate = $("#BusinessDate").text();

    var formatmmddyyyydate = budate.split("/");
    formatmmddyyyydate = formatmmddyyyydate[1] + "/" + formatmmddyyyydate[0] + "/" + formatmmddyyyydate[2];
    var b_date = new Date(formatmmddyyyydate);

    if ($('#FormSearch_from_trade_date').length) {
        $('#FormSearch_from_trade_date').data("DateTimePicker").date(b_date);
    }

    if ($('#FormSearch_to_trade_date').length) {
        $('#FormSearch_to_trade_date').data("DateTimePicker").date(b_date);
    }

    $("#FormSearch_from_trade_date").on("dp.change", function (e) {
        if (e.date) {

            var to = $('#FormSearch_to_trade_date').data("DateTimePicker").date();
            var diff = to.diff(e.date, 'days');
            if (diff < 0) {
                $('#FormSearch_to_trade_date').data("DateTimePicker").date(e.date);
            }
            else if (diff > 365) {
                swal("Warning", "Date range is over 1 year. Please try select date again.", "warning");
                $('#FormSearch_to_trade_date').data("DateTimePicker").date(e.date.add(1, 'years'));
            }
        } else {
            //$('#FormSearch_from_trade_date').data("DateTimePicker").date(b_date);
            //$('#FormSearch_to_trade_date').data("DateTimePicker").date(b_date);
        }
    });

    $("#FormSearch_to_trade_date").on("dp.change", function (e) {
        if (e.date) {

            var from = $('#FormSearch_from_trade_date').data("DateTimePicker").date();
            var diff = e.date.diff(from, 'days');
            if (diff < 0) {
                $('#FormSearch_from_trade_date').data("DateTimePicker").date(e.date);
            }
            else if (diff > 365) {
                swal("Warning", "Date range is over 1 year. Please try select date again.", "warning");
                $('#FormSearch_from_trade_date').data("DateTimePicker").date(e.date.subtract(1, 'years'));
            }
        } else {
            //$('#FormSearch_from_trade_date').data("DateTimePicker").date(b_date);
            //$('#FormSearch_to_trade_date').data("DateTimePicker").date(b_date);
        }
    });

    //Function Search ==============================================

    //Binding ddl
    //Function : Btn Report TBMA
    //Binding Div Detail
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    };

    GM.RPReportTBMA = {};
    GM.RPReportTBMA.Table = $('#x-table-data').DataTable({
        dom: 'Bfrtip',
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
            "url": "/RPReportTBMA/Search",
            "type": "POST",
            "error": function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR);
                console.log(textStatus);
                console.log(errorThrown);
            }
        },
        columnDefs:
            [
                {
                    targets: 0,
                    data: "trans_no",
                    searchable: false,
                    orderable: false,
                    className: 'dt-body-center',
                    render: function (data, type, row) {
                        console.log(row.port);
                        return '<input type="checkbox"' + ' id="chk_' + data + '" class="filter-ck" />';
                    }
                },
                { targets: 1, data: "RowNumber", orderable: false },
                { targets: 2, data: "trans_no" },
                {
                    targets: 3, data: "report_status",
                    render: function (data, type, row) {
                        if (data && row.report_status.toLowerCase() === "success") {
                            return '<label style="color:#008000;font-weight:normal">' + row.report_status + '</label>';
                        }
                        else if (data && row.report_status.toLowerCase() === "unsend") {
                            return '<label style="color:#FF0000;font-weight:normal">' + row.report_status + '</label>';
                        }
                        return data;
                    }
                },
                { targets: 4, data: "round_no", orderable: false },
                { targets: 5, data: "trans_deal_type_name" },
                { targets: 6, data: "port" },
                { targets: 7, data: "trader_id" },
                { targets: 8, data: "trader_engname" },
                { targets: 9, data: "purpose", orderable: false },
                { targets: 10, data: "trade_time", orderable: false },
                {
                    targets: 11, data: "trade_date",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                {
                    targets: 12, data: "settlement_date", visible: false,
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                { targets: 13, data: "buysell_type", orderable: false },
                { targets: 14, data: "counter_party", orderable: false },
                { targets: 15, data: "term", orderable: false },
                { targets: 16, data: "rate", orderable: false },
                { targets: 17, data: "from_trade_date", visible: false },
                { targets: 18, data: "to_trade_date", visible: false }
            ],
        fixedColumns: {
            leftColumns: 1
        },
        fnPreDrawCallback: function () {
            $('#x-table-data').DataTable().columns(17).search($('#FormSearch_from_trade_date').val());
            $('#x-table-data').DataTable().columns(18).search($('#FormSearch_to_trade_date').val());
        }
    });

    //Function : Checkbox
    $('#x-table-data').on('click', 'tr', function () {

        GM.RPReportTBMA.Table = $('#x-table-data').DataTable();
        var data = GM.RPReportTBMA.Table.row(this).data();

        if (typeof data != 'undefined') {
            if ($(this).find('.filter-ck').prop('checked') == true) {  //update the cell data with the checkbox state
                var id = 'chk_' + data.trans_no;
                var inputyes = $("[id=" + id + "]");
                inputyes.removeAttr('checked');
                inputyes.attr('checked', 'checked');
                inputyes.prop('checked', true);
            } else {
                var id = 'chk_' + data.trans_no;
                var inputno = $("[id=" + id + "]");
                inputno.removeAttr('checked');
                inputno.prop('checked', false);
            }
        }
    });
    //Function : Checkbox All
    $('#x-table-data_wrapper').on("click", '#CheckAll', function () {
        if ($(this).prop('checked') == true) {
            $('.filter-ck').prop('checked', true);
        }
        else {
            $('.filter-ck').prop('checked', false);
        }
    });
    //Function : Btn Report TBMA
    GM.RPReportTBMA.Report = function (btn) {
        var mode = $(btn).attr("form-mode");
        GM.Message.Clear();
        Report();
    };

    GM.RPReportTBMA.Form = {};

    GM.RPReportTBMA.Form.GenExcel = function (type) {
        ExportExcel(type);
    };

    GM.RPReportTBMA.Form.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            switch (key) {
                case "trans_deal_type": GM.RPReportTBMA.Table.columns(5).search($(this).val()); break;
                case "port": GM.RPReportTBMA.Table.columns(6).search($(this).val()); break;
                case "counter_party_code": GM.RPReportTBMA.Table.columns(14).search($(this).val()); break;
                case "from_trade_date": GM.RPReportTBMA.Table.columns(17).search($(this).val()); break;
                case "to_trade_date": GM.RPReportTBMA.Table.columns(18).search($(this).val()); break;
            }
        });

        GM.RPReportTBMA.Table.draw();
    };

    GM.RPReportTBMA.Form.Initial = function () {
        $("#action-form")[0].reset();
    };
    GM.RPReportTBMA.Form.DataBinding = function (p) {
        $('#' + p.form + ' :input').each(function () {
            var input = $(this);
            var inputid = input[0].id;
            var key = input[0].name.split('.')[1];
            var inputtype = input.attr("type");
            var inputvalue = input.attr("value");
            if (p.data[key] != null) {
                if (inputtype == "radio") {
                    var inputyes = $("[id=" + inputid + "][value=true]");
                    var inputno = $("[id=" + inputid + "][value=false]");
                    if (p.data[key]) {
                        inputno.removeAttr('checked');
                        inputno.attr('ischeck', 'false');
                        inputyes.attr('ischeck', 'true');
                        inputyes.attr('checked', 'checked');
                        inputyes.prop('checked', true);
                    }
                    else {
                        inputyes.removeAttr('checked');
                        inputyes.attr('ischeck', 'false');
                        inputno.attr('ischeck', 'true');
                        inputno.attr('checked', 'checked');
                        inputno.prop('checked', true);
                        //}
                    }
                }
                else {
                    $(this).val(p.data[key] + '');
                }

            }
            console.log('(' + key + ') => ' + p.data[key]);
        });
        $('#' + p.form + ' span').each(function () {
            var input = $(this);
            var key = input.attr("name");
            if (typeof key != "undefined") {
                if (p.data[key] != "" && p.data[key] != null) {
                    $(this)[0].innerHTML = p.data[key];
                }
            }
            console.log('(' + key + ') => ' + p.data[key]);
        });
    };

    $("#search-form").on('submit', function (e) {
        e.preventDefault();
        GM.Message.Clear();
        GM.RPReportTBMA.Form.Search();
    });
    $("#search-form").on('reset', function (e) {
        GM.Defer(function () {
            $('#ddl_trans_deal_type').find(".selected-data").text("Select...");
            $('#FormSearch_trans_deal_type').val(null);
            $('#FormSearch_trans_deal_type_name').val(null);

            $('#ddl_trans_type').find(".selected-data").text("Select...");
            $('#FormSearch_trans_type').val(null);
            $('#FormSearch_trans_type_name').val(null);

            $('#ddl_counterparty').find(".selected-data").text("Select...");
            $('#FormSearch_counter_party_code').val(null);
            $('#FormSearch_counter_party_name').val(null);
            $('#ddl_repo_deal_type').find(".selected-data").text("Select...");
            $('#FormSearch_repo_deal_type').val(null);
            $('#FormSearch_repo_deal_type_code').val(null);
            $('#ddl_port').find(".selected-data").text("Select...");
            $('#FormSearch_port').val(null);
            $('#FormSearch_port_name').val(null);
            $('#ddl_purpose').find(".selected-data").text("Select...");
            $('#FormSearch_purpose').val(null);
            $('#FormSearch_purpose_name').val(null);
            $('#ddl_cur').find(".selected-data").text("Select...");
            $('#FormSearch_cur').val(null);

            $('#FormSearch_from_trade_date').data("DateTimePicker").date(b_date);
            $('#FormSearch_to_trade_date').data("DateTimePicker").date(b_date);

            GM.Message.Clear();
            GM.RPReportTBMA.Form.Search();
        }, 100);
    });

    //Tran Type Dropdown
    $("#ddl_trans_type").click(function () {
        var txt_search = $('#txt_trans_type');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_trans_type').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //Tran Deal Type Dropdown
    $("#ddl_trans_deal_type").click(function () {
        var txt_search = $('#txt_trans_deal_type');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_trans_deal_type').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //counterparty Dropdown
    $("#ddl_counterparty").click(function () {
        var txt_search = $('#txt_counterparty');
        var data = { datastr: null };
        GM.Utility.DDLAutoCompleteSet4Value(txt_search, data, null, null, null);
        txt_search.val("");
    });
    $('#txt_counterparty').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });

    //purpose Dropdown
    $("#ddl_purpose").click(function () {
        var txt_search = $('#txt_purpose');
        var data = { datastr: null };
        GM.Utility.DDLAutoCompleteSet4Value(txt_search, data, null, null, null);
        txt_search.val("");
    });
    $('#txt_purpose').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });

    //port Dropdown
    $("#ddl_port").click(function () {
        var txt_search = $('#txt_port');
        var data = { datastr: null };
        GM.Utility.DDLAutoCompleteSet4Value(txt_search, data, null, null, null);
        txt_search.val("");
    });
    $('#txt_port').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });

    //cur Dropdown
    $("#ddl_cur").click(function () {
        var txt_search = $('#txt_cur');
        var data = { datastr: null };
        GM.Utility.DDLAutoCompleteSet4Value(txt_search, data, null, null, null);
        txt_search.val("");
    });
    $('#txt_cur').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });

});