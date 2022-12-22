
function FormatDecimal(Num, point) {
    var format = Number(parseFloat(Num).toFixed(point)).toLocaleString('en', {
        minimumFractionDigits: point
    });
    return format;
}

function ddlChangePayment(id, value) {
    console.log('myData[id].margin_type.toUpperCase() : ' + myData[id].margin_type.toUpperCase());
    var data = {
        paymentmethod: $('#ddl_payment_' + id).val(),
        transdealtype: myData[id].margin_type.toUpperCase().indexOf('PAY') === 0 ? 'PAY' : myData[id].margin_type.toUpperCase().indexOf('RECEIVE') === 0 ? 'REV' : '',
        cur: $('#FormSearch_cur').val() === 'THB' ? $('#FormSearch_cur').val() : 'FCY'
    };
    myData[id].payment_method = $('#ddl_payment_' + id).val() === '' ? null : $('#ddl_payment_' + id).val();
    GM.Utility.DDLStandard('ddl_mt_code_' + id, data, "\\RPCallMargin\\FillMTCode", value);
}

function ddlChangeMTCode(id) {
    myData[id].mt_code = $('#ddl_mt_code_' + id).val();
}

function ddlChangeChannel(id, value, cur) {
    var data = { datastr: cur };
    GM.Utility.DDLStandard('ddl_channel_' + id, data, "\\RPReleaseMessage\\FillSwiftChannel", value);
}

function SetChannel(id) {
    myData[id].swift_channel = $('#ddl_channel_' + id).val();
}

function ReleaseMessage(listData) {

    $('.spinner').css('display', 'block'); // Open Loading
    $.ajax({
        url: "/RPCallMargin/ReleaseMessage",
        type: "GET",
        dataType: "JSON",
        data: { data: JSON.stringify(listData) },
        success: function (res) {
            console.log('res.returnCode :' + res.returnCode);
            if (res.returnCode === true) {
                swal("Complete", "Release Message Successfully", "success");
            } else {
                console.log(res.Message);
                swal("Fail", res.Message, "error");
            }
            GM.RPReleaseCallMargin.Search();
            $('.spinner').css('display', 'none');
        },
        error: function (error) {
            console.log(error);
            $('.spinner').css('display', 'none');
        },
        statusCode: {
            401: function () {
                window.location.href = "/RPReleaseMessage";
            }
        }
    });
}

function ReleaseMessageCyberPay(listData) {

    $('.spinner').css('display', 'block'); // Open Loading
    $.ajax({
        url: "/RPCallMargin/ReleaseMessageCyberPay",
        type: "GET",
        dataType: "JSON",
        data: { data: JSON.stringify(listData) },
        success: function (res) {
            console.log('res.returnCode :' + res.returnCode);
            if (res.returnCode === true) {
                swal("Complete", "Release Message Successfully", "success");
            } else {
                console.log(res.Message);
                swal("Fail", res.Message, "error");
            }
            GM.RPReleaseCallMargin.Search();
            $('.spinner').css('display', 'none');
        },
        error: function (error) {
            console.log(error);
            $('.spinner').css('display', 'none');
        },
        statusCode: {
            401: function () {
                window.location.href = "/RPReleaseMessage";
            }
        }
    });
}

function GetMessage(data) {

    $('.spinner').css('display', 'block'); // Open Loading
    $.ajax({
        url: "/RPReleaseMessage/GetReleaseCallMargin",
        type: "GET",
        dataType: "JSON",
        data: { data: JSON.stringify(data) },
        success: function (res) {
            console.log('res.returnCode :' + res.returnCode);
            $('.spinner').css('display', 'none');
            if (res.Success === true) {
                $("#check_releasemt").modal('toggle');
                var body = $("#modal_release_mt").find("tbody");
                var html = "";
                //var isin_code = "";
                body.html("");
                $.each(res.Data.RPReleaseMessageResultModel, function (i, resdata) {
                    if (resdata.mt_message !== '' && resdata.mt_message !== null) {
                        html = "<tr><td>" + $('<div/>').text(resdata.mt_message).html() + "</td></tr>";
                    } else {
                        html = "<tr><td>&nbsp;</td></tr>";
                    }
                    body.append(html);
                });
            } else {
                console.log(res.Message);
                swal("Fail", res.Message, "error");
            }
        },
        error: function (error) {
            console.log(error);
            $('.spinner').css('display', 'none');
        },
        statusCode: {
            401: function () {
                window.location.href = "/RPReleaseMessage";
            }
        }
    });
}

var downloadPDF = function (asofdate, counter_party_id) {
    var url = "/RPReleaseMessage/DownloadPDF" + "?asofdate=" + asofdate + "&counter_party_id=" + counter_party_id;
    window.open(url, '_blank');
    return;
};

$(window).on('load', function () {

    var defultCur = "THB";

    $("#ddl_cur").find(".selected-data").text(defultCur);
    $("#ddl_cur").find(".selected-value").val(defultCur);

    var budate = $("#BusinessDate").text();

    var formatmmddyyyydate = budate.split("/");
    formatmmddyyyydate = formatmmddyyyydate[1] + "/" + formatmmddyyyydate[0] + "/" + formatmmddyyyydate[2];
    var callMarginDate = new Date(formatmmddyyyydate);

    //$('#FormSearch_from_call_date').data("DateTimePicker").date(callMarginDate);
    //$('#FormSearch_to_call_date').data("DateTimePicker").date(callMarginDate);

    $('#FormSearch_from_call_date').val(budate);
    $('#FormSearch_to_call_date').val(budate);

    setTimeout(
        function () {
            GM.RPReleaseCallMargin.Search();
        },
        1000);

});

$(document).ready(function () {

    //Binding : DDL
    $("#ddl_event_type").click(function () {
        var txt_search = $('#txt_event_type');
        var data = { event_type: '' };
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });

    $("#ul_event_type").on("click", ".searchterm", function (event) {
        var eventType = $("#ddl_event_type").find(".selected-value").val();
        console.log('eventType : ' + eventType);
        if (eventType === "Coupon") {
            window.location.href = "/RPReleaseMessage/IndexCoupon";
        } else if (eventType === 'Settlement' || eventType === 'Maturity') {
            window.location.href = "/RPReleaseMessage/Index?search=" + eventType;
        } else if (eventType === 'Net-Settlement') {
            window.location.href = "/RPReleaseMessage/IndexNetSettlement?search=" + eventType;
        } else if (eventType === 'Interest Margin') {
            window.location.href = "/RPReleaseMessage/IndexInterestMargin";
        }
    });

    $("#ddl_cur").click(function () {
        var txt_search = $('#txt_cur');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });

    $('#txt_cur').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null, false);
    });

    $("#ul_cur").on("click", ".searchterm", function (event) {
        GM.RPReleaseCallMargin.Search();
    });

    $("#ddl_counterparty").click(function () {
        var txt_search = $('#txt_counterparty');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_counterparty').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    };

    GM.RPReleaseCallMargin = {};

    GM.RPReleaseCallMargin.Table = $('#x-table-data').DataTable({
        drawCallback: function (row, data, index) {
            var api = this.api();
            myData = api.rows({ page: 'current' }).data();
            var x = document.getElementsByName("ddlPayment");
            for (var i = 0; i < myData.length; i++) {
                if (myData[i].cur !== 'THB' && myData[i].margin_type === 'Receive') {
                    console.log("Notice");
                } else {

                    var dataSearch = {
                        transdealtype: myData[i].margin_type.toUpperCase().indexOf('PAY') === 0 ? 'PAY' : myData[i].margin_type.toUpperCase().indexOf('RECEIVE') === 0 ? 'REV' : '',
                        cur: $('#FormSearch_cur').val() === 'THB' ? $('#FormSearch_cur').val() : 'FCY'
                    };

                    if ($('#ddl_payment_' + i).val() === "") {
                        GM.Utility.DDLStandard('ddl_payment_' + i, dataSearch, "\\RPCallMargin\\FillPaymentMethod", null);
                    }
                }
            }
        },
        dom: 'Bfrtip',
        searching: true,
        scrollY: '80vh',
        scrollX: true,
        order: [
            [1, "asc"]
        ],
        buttons: [
            'copy', 'csv', 'excel', 'pdf'
        ],
        processing: true,
        serverSide: true,
        ajax: {
            "url": "/RPReleaseMessage/SearchCallMargin",
            "type": "POST",
            "statusCode": {
                401: function () {
                    window.location.href = "/RPReleaseMessage";
                }
            },
            "error":
                function (error) {
                    console.log(error);
                    window.location.href = "/RPReleaseMessage";
                }
        },
        columnDefs:
            [
                {
                    targets: 0, data: "RowNumber", orderable: false, width: 10
                },
                {
                    targets: 1, data: "asof_date",
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = moment(data).format('DD/MM/YYYY');
                        }
                        return '<label id=asof_date_' + meta.row + ' style="font-weight:normal" value="' + html + '" >' + html + '</label>';
                    }

                },
                {
                    targets: 2, data: "call_date",
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = moment(data).format('DD/MM/YYYY');
                        }
                        return '<label id=call_date_' + meta.row + ' style="font-weight:normal" value="' + html + '" >' + html + '</label>';
                    }

                },
                {
                    targets: 3, data: "counter_party_name",
                    orderable: false,
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return '<label id=counter_party_name_' + meta.row + ' style="font-weight:normal;  text-overflow: ellipsis;overflow: hidden;width: 225px;white-space: nowrap;" value="' + row.counter_party_name + '" title="' + row.counter_party_name + '">' + row.counter_party_name + '</label>';
                        }
                        return data;
                    }
                },
                {
                    targets: 4, data: "margin_amt",
                    className: 'dt-body-right',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 4);
                        } else {
                            html = Number("0").toFixed(4);
                        }
                        return '<label id=margin_amt_' + meta.row + ' style="font-weight:normal" value="' + html + '" >' + html + '</label>';
                    }
                },
                {
                    targets: 5, data: "int_cash_margin",
                    className: 'dt-body-right',
                    orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 4);
                        } else {
                            html = Number("0").toFixed(4);
                        }
                        return '<label id=int_cash_margin_' + meta.row + ' style="font-weight:normal" value="' + html + '" >' + html + '</label>';
                    }
                },
                {
                    targets: 6, data: "payment_method",
                    className: 'dt-body-center',
                    searchable: false,
                    orderable: false,
                    render: function (data, type, row, meta) {

                        var html = "";
                        if (row.cur !== 'THB' && row.margin_type === 'Receive') {
                            html = "Notiec";
                        } else {
                            var html = '<input class="selected-data hidden" id="MarginList[' + meta.row + '].payment_method" name="MarginList[' + meta.row + '].payment_method" type="text" >' +
                                '<input class="selected-data hidden" id="MarginList[' + meta.row + '].payment_method_name" name="MarginList[' + meta.row + '].payment_method_name" type="text" >' +
                                '<select name="ddlPayment" id="ddl_payment_' + meta.row + '" style="height:23px;width:80px;" name="MarginList[' + meta.row + '].payment_method" onchange="ddlChangePayment(\'' + meta.row + '\');" >' +
                                ' <option value="">- Please select -</option>';
                            if (row.payment_method !== null) {
                                html += ' <option selected="selected" value="' + row.payment_method + '">' + row.payment_method + '</option>';
                            }

                            html += '</select > '
                                + '<input class="selected-data hidden" id="MarginList[' + meta.row + '].mt_code" name="MarginList[' + meta.row + '].mt_code" type="text" >' +
                                '<input class="selected-data hidden" id="MarginList[' + meta.row + '].mt_code_name" name="MarginList[' + meta.row + '].mt_code_name" type="text" >' +
                                '<select id="ddl_mt_code_' + meta.row + '" style="height:23px;width:80px;" name="MarginList[' + meta.row + '].mt_code" onchange="ddlChangeMTCode(\'' + meta.row + '\');" > ' +
                                ' <option value="">- Please select -</option>';

                            setTimeout(function () {
                                if (row.mt_code !== null) {
                                    console.log("row.mt_code : " + row.mt_code);
                                    ddlChangePayment(meta.row, row.mt_code);
                                } else {
                                    console.log("row.mt_code Else : " + row.mt_code);
                                }
                            }, 200);

                            html += '</select>';

                            html += '<input class="selected-data hidden" id="MarginList[' + meta.row + '].swift_channel" name="MarginList[' + meta.row + '].swift_channel" type="text" >' +
                                '<input class="selected-data hidden" id="MarginList[' + meta.row + '].swift_channel_desc" name="MarginList[' + meta.row + '].swift_channel_desc" type="text" >' +
                                '<select ' + (row.margin_type.toUpperCase().indexOf('PAY') !== -1 ? "" : " disabled ") +
                                ' id="ddl_channel_' + meta.row + '" style = "height:23px;width:150px;" name = "MarginList[' + meta.row + '].swift_channel" onchange = "SetChannel(\'' + meta.row + '\');" > ' +
                                ' <option value="">- Please select -</option>';
                        }

                        setTimeout(function () {
                            if (row.swift_channel !== null) {
                                console.log("row.swift_channel : " + row.swift_channel);
                                ddlChangeChannel(meta.row, row.swift_channel, row.cur);
                            } else if (row.margin_type.toUpperCase().indexOf('PAY') !== -1) {
                                console.log("row.swift_channel : SMD ");
                                ddlChangeChannel(meta.row, 'SMD', row.cur);
                            }
                            else {
                                console.log("row.swift_channel Else : " + row.swift_channel);
                            }
                        }, 200);

                        html += '</select>';

                        return html;
                    }
                },
                {
                    targets: 7, data: "swift_status",
                    searchable: false,
                    orderable: false,
                    className: "dt-body-center",
                    visible: true,
                    render: function (data, type, row, meta) {
                        return data;
                    }
                },
                {
                    targets: 8, data: "action",
                    className: 'dt-body-center',
                    searchable: false,
                    orderable: false,
                    width: 30,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (row.cur !== 'THB' && row.margin_type === 'Receive') {
                            html += '<button title="Print Notice" class="btn btn-default btn-round" key="' + row.counter_party_id + '" call_date="' + moment(row.call_date).format('DD/MM/YYYY') + '" asof_date="' + moment(row.asof_date).format('DD/MM/YYYY') + '" form-mode="printMessage" id="printMessage_' + meta.row + '" data-action="printMessage"  onclick="GM.RPReleaseCallMargin.Action(this)" ><i class="feather-icon icon-printer"></i></button>';
                        } else {
                            html += '<button title="Release Message" class="btn btn-default btn-round" key="' + row.counter_party_id + '" call_date="' + moment(row.call_date).format('DD/MM/YYYY') + '" asof_date="' + moment(row.asof_date).format('DD/MM/YYYY') + '" form-mode="genMessage" id="genMessage_' + meta.row + '" data-action="genMessage"  onclick="GM.RPReleaseCallMargin.Action(this)" ><i class="feather-icon icon-file-plus"></i></button>';
                            html += '<button title="View Message" class="btn btn-default btn-round" key="' + row.counter_party_id + '" call_date="' + moment(row.call_date).format('DD/MM/YYYY') + '" asof_date="' + moment(row.asof_date).format('DD/MM/YYYY') + '" form-mode="viewMessage" id="viewMessage_' + meta.row + '" data-action="viewMessage"  onclick="GM.RPReleaseCallMargin.Action(this)" ><i class="feather-icon icon-message-square"></i></button>';
                        }
                        return html;
                    }
                },
                { targets: 9, data: "from_as_of_date", visible: false },
                { targets: 10, data: "to_as_of_date", visible: false },
                { targets: 11, data: "from_call_date", visible: false },
                { targets: 12, data: "to_call_date", visible: false },
                { targets: 13, data: "counter_party_id", "visible": false },
                { targets: 14, data: "cur", "visible": false },
                { targets: 15, data: "margin_type", "visible": false },
                { targets: 16, data: "brp_contract_no", "visible": false },
                { targets: 17, data: "swift_channel", "visible": false }
            ],
        fixedColumns: {
            leftColumns: 1,
            rightColumns: 10
        },
        createdRow: function (row, data, dataIndex) {
            if (data.release_printed === "Y") {
                $('td', row).css('color', '#00B0FF');
            }
        }
    });

    GM.RPReleaseCallMargin.Search = function () {
        try {
            var FormSearch_from_as_of_date = $("#FormSearch_from_as_of_date").val();
            var FormSearch_to_as_of_date = $("#FormSearch_to_as_of_date").val();
            var FormSearch_from_call_date = $("#FormSearch_from_call_date").val();
            var FormSearch_to_call_date = $("#FormSearch_to_call_date").val();
            var FormSearch_cur = $('#FormSearch_cur').val();
            var FormSearch_counter_party_id = $('#FormSearch_counter_party_id').val();

            if (FormSearch_from_as_of_date !== '') {
                GM.RPReleaseCallMargin.Table.columns(9).search(FormSearch_from_as_of_date);
            }

            if (FormSearch_to_as_of_date !== '') {
                GM.RPReleaseCallMargin.Table.columns(10).search(FormSearch_to_as_of_date);
            }

            if (FormSearch_from_call_date !== '') {
                GM.RPReleaseCallMargin.Table.columns(11).search(FormSearch_from_call_date);
            }
            if (FormSearch_to_call_date !== '') {
                GM.RPReleaseCallMargin.Table.columns(12).search(FormSearch_to_call_date);
            }

            GM.RPReleaseCallMargin.Table.columns(13).search(FormSearch_counter_party_id);
            GM.RPReleaseCallMargin.Table.columns(14).search(FormSearch_cur);

            GM.RPReleaseCallMargin.Table.draw();
        } catch (err) {
            console.log(err.message);
        }
    };

    GM.RPReleaseCallMargin.Action = function (btn) {
        var mode = $(btn).attr("form-mode");
        GM.Message.Clear();
        if (mode) {
            var key = $(btn).attr("key");
            var call_date = $(btn).attr("call_date");
            var asof_date = $(btn).attr("asof_date");
            var FormSearch_cur = $('#FormSearch_cur').val();

            var rowData = $('#x-table-data').DataTable().rows().data();

            switch (mode) {
                case "printMessage":
                    {
                        downloadPDF(asof_date, key);
                    } break;
                case "viewMessage":
                    var idViewMessage = $(btn).attr("id").split('viewMessage_')[1];

                    if (rowData[idViewMessage].payment_method === null || rowData[idViewMessage].mt_code === null) {
                        swal("Warning", "Please select Payment Method", "error");
                        $('#ddl_payment_' + (rowData[idViewMessage].RowNumber - 1)).focus();
                    } else {
                        var data =
                        {
                            counter_party_id: rowData[idViewMessage].counter_party_id,
                            call_date: moment(rowData[idViewMessage].call_date).format('YYYY-MM-DD'),
                            payment_method: rowData[idViewMessage].payment_method,
                            mt_code: rowData[idViewMessage].mt_code,
                            cur: $('#FormSearch_cur').val(),
                            row: rowData[idViewMessage].RowNumber,
                            counter_party_name: rowData[idViewMessage].counter_party_name,
                            brp_contract_no: rowData[idViewMessage].brp_contract_no
                        };
                        GetMessage(data);
                    }
                    break;
                case "genMessage":
                    var idGenMessage = $(btn).attr("id").split('genMessage_')[1];

                    if (rowData[idGenMessage].payment_method === null || rowData[idGenMessage].mt_code === null) {
                        swal("Warning", "Please select Payment Method", "error");
                        $('#ddl_payment_' + (rowData[idGenMessage].RowNumber - 1)).focus();
                    } else {

                        var msg = "Comfirm ReleaseMessage?";
                        if (rowData[idGenMessage].release_printed == 'Y') {
                            msg = "Message already released, do you want to release again?";
                        }

                        swal({
                            title: msg,
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

                                    var listData = [];
                                    var row =
                                    {
                                        counter_party_id: rowData[idGenMessage].counter_party_id,
                                        call_date: moment(rowData[idGenMessage].call_date).format('YYYY-MM-DD'),
                                        payment_method: rowData[idGenMessage].payment_method,
                                        mt_code: rowData[idGenMessage].mt_code,
                                        cur: $('#FormSearch_cur').val(),
                                        row: rowData[idGenMessage].RowNumber,
                                        counter_party_name: rowData[idGenMessage].counter_party_name,
                                        brp_contract_no: rowData[idGenMessage].brp_contract_no,
                                        swift_channel: rowData[idGenMessage].swift_channel
                                    };
                                    listData.push(row);

                                    $('.spinner').css('display', 'block'); // Open Loading
                                    if (rowData[idGenMessage].mt_code.indexOf('202') == -1) {
                                        ReleaseMessage(listData);
                                    } else {
                                        $.post("/RPCallMargin/CheckSettlementStatus", {
                                            counter_party_id: rowData[idGenMessage].counter_party_id,
                                            call_date: moment(rowData[idGenMessage].call_date).format('DD/MM/YYYY'),
                                            payment_method: rowData[idGenMessage].payment_method,
                                            mt_code: rowData[idGenMessage].mt_code,
                                            cur: $('#FormSearch_cur').val(),
                                            row: rowData[idGenMessage].RowNumber,
                                            counter_party_name: rowData[idGenMessage].counter_party_name,
                                            brp_contract_no: rowData[idGenMessage].brp_contract_no,
                                            swift_channel: rowData[idGenMessage].swift_channel,
                                            event_type: "MARGIN",
                                            eom_int_flag: "N"
                                        })
                                            .done(function (result) {
                                                if (result.Success) {
                                                    if (result.Data.ResponseBody[0].SettlementStatus === localStorage.getItem('SettlementStatus.Complete')) {
                                                        setTimeout(function () {
                                                            swal("Fail", localStorage.getItem('CyberPay.ErrorMsg.Complete'), "error");
                                                        }, 100);
                                                        $('.spinner').css('display', 'none');
                                                    } else {
                                                        if ($('#ddl_channel_' + idGenMessage).val() === "SMD" || $('#ddl_channel_' + idGenMessage).val() == "") {
                                                            ReleaseMessage(listData);
                                                        } else {
                                                            ReleaseMessageCyberPay(listData);
                                                        }
                                                    }
                                                }
                                                else {
                                                    setTimeout(function () {
                                                        swal("Fail", result.Message, "error");
                                                    }, 100);
                                                    $('.spinner').css('display', 'none'); // Close Loading
                                                }
                                            });
                                    }
                                }
                            });
                    }
                    break;
            }
        }
    };

    GM.RPReleaseCallMargin.Sync = function (btn) {
        $('.spinner').css('display', 'block'); // Open Loading

        $.ajax({
            type: "POST",
            url: "/RPCallMargin/SyncTransaction",
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: {
                from_call_date: $('#FormSearch_from_call_date').val(),
                to_call_date: $('#FormSearch_to_call_date').val(),
                from_as_of_date: $('#FormSearch_from_as_of_date').val(),
                to_as_of_date: $('#FormSearch_to_as_of_date').val(),
                cur: $('#FormSearch_cur').val(),
                counter_party_id: $('#FormSearch_counter_party_id').val(),
                isSyncCyberPay: true
            },
            success: function (res) {
                if (res.Success) {
                    setTimeout(function () {
                        swal({
                            title: "Complete",
                            text: "Sync Data Cyber Pay Success",
                            type: "success",
                            showCancelButton: false,
                            confirmButtonClass: "btn-success",
                            confirmButtonText: "Ok",
                            closeOnConfirm: true,
                        },
                            function () {
                                $('.spinner').css('display', 'none'); // Close Loading
                                GM.RPReleaseCallMargin.Search();
                            }
                        );
                    }, 100);
                } else {
                    setTimeout(function () {
                        swal({
                            title: "Warning",
                            text: res.Message,
                            type: "error",
                            showCancelButton: false,
                            confirmButtonClass: "btn-success",
                            confirmButtonText: "Ok",
                            closeOnConfirm: true,
                        },
                            function () {
                                $('.spinner').css('display', 'none'); // Close Loading
                            }
                        );
                    }, 100);
                }

            },
            error: function (res) {
                setTimeout(function () {
                    swal({
                        title: "Warning",
                        text: res.Message,
                        type: "error",
                        showCancelButton: false,
                        confirmButtonClass: "btn-success",
                        confirmButtonText: "Ok",
                        closeOnConfirm: true,
                    },
                        function () {
                            $('.spinner').css('display', 'none'); // Close Loading
                        }
                    );
                }, 100);

                $('.spinner').css('display', 'none'); // Close Loading
            }
        });
    };

    $("#search-form").on('submit', function (e) {
        e.preventDefault();
        GM.RPReleaseCallMargin.Search();
    });

    $("#search-form").on('reset', function (e) {
        $('.spinner').css('display', 'block'); // Open Loading
        location.href = window.location.href;
    });
});