$.ajaxSetup({
    cache: false
});

(function ($) {
    $.fn.currencyFormat = function () {
        this.each(function (i) {
            $(this).change(function (e) {
                if (isNaN(parseFloat(this.value))) return;
                this.value = Number(this.value).toFixed(6);
            });
        });
        return this;
    };
    $.fn.dataTable.ext.errMode = 'none';
})(jQuery);

function adjusttable() {
    setTimeout(
        function () {
            $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();
        },
        200);
}

function setformatdateyyyymmdd(date) {
    date = date.split('/');
    date = date[2] + "-" + date[1] + "-" + date[0];
    return date;
}

function ddlClickPayment(id) {
    var data = { datastr: null };
    if ($('#ddl_payment_' + id).val() === "") {
        GM.Utility.DDLStandard('ddl_payment_' + id, data, "\\RPCallMargin\\FillPaymentMethod", null);
    }
}

function ddlChangePayment(id) {
    var data = { paymentmethod: $('#ddl_payment_' + id).val(), transdealtype: myData[id].margin_type.toUpperCase().indexOf('PAY') === 0 ? 'PAY' : myData[id].margin_type.toUpperCase().indexOf('RECEIVE') === 0 ? 'REV' : '', cur: $('#FormSearch_cur').val() };
    myData[id].payment_method = $('#ddl_payment_' + id).val() === '' ? null : $('#ddl_payment_' + id).val();
    GM.Utility.DDLStandard('ddl_mt_code_' + id, data, "\\RPCallMargin\\FillMTCode", null);
}

function ddlChangeMTCode(id) {
    myData[id].mt_code = $('#ddl_mt_code_' + id).val();
}

function FormatDecimal(Num, point) {
    var format = Number(parseFloat(Num).toFixed(point + 1)).toLocaleString('en', {
        minimumFractionDigits: point
    });
    return format;
}

function text_OnKeyPress_NumberOnlyAndDotAndMinus(obj) {

    var keyAble = false;
    try {
        key = window.event.keyCode;
        var beforeVal = obj.value;
        if (beforeVal.indexOf('.') !== -1) {
            if (key === 46) {
                keyAble = false;
            } else if (key > 47 && key < 58) {
                keyAble = true;
            }
            else if (key === 45) {
                if (!beforeVal.indexOf('-') !== -1 && obj.selectionStart === 0) {
                    keyAble = true;
                } else {
                    keyAble = false;
                }
            }
            else {
                keyAble = false;
            }
        }
        else if ((key === 46) || (key > 47 && key < 58)) {
            keyAble = true;
        } else if (key === 45) {
            if (!beforeVal.indexOf('-') !== -1 && obj.selectionStart === 0) {
                keyAble = true;
            } else {
                keyAble = false;
            }
        }
        else {
            keyAble = false;
        }
    } catch (err) {
        throw new Error(" error in sub text_OnKeyPress_NumberOnlyAndDotAndMinus function() cause = " + err.message);
    }
    return keyAble;
}

function setformatdatemmddyyyy(date) {
    if (date !== "") {
        date = date.split('/');
        date = date[1] + "/" + date[0] + "/" + date[2];
    }
    else {
        date = 0;
    }
    return date;
}

function GetNextBusinessDate(data) {
    $.ajax({
        url: "/RPCallMargin/GetNextBusinessDate",
        type: "POST",
        dataType: "JSON",
        data: data,
        success: function (res) {
            if (res.returnCode === 0) {
                var nextBusinessDate = new Date(res.NextBusinessDate);
                $('#FormSearch_call_date').data("DateTimePicker").date(nextBusinessDate);

                setTimeout(
                    function () {
                        CheckMarginIntRate();
                    },
                    500);

            } else {

                console.log(res.Message);
            }
        },
        error: function (error) {
            $('.spinner').css('display', 'none');
            console.log(error);
        },
        statusCode: {
            401: function () {
                window.location.href = "/RPCallMargin";
            }
        }
    });
}

function GetInterestRate(data) {
    $.ajax({
        url: "/RPCallMargin/GetInterestRate",
        type: "POST",
        dataType: "JSON",
        data: data,
        success: function (res) {
            var intRate = 0.00;
            if (res.returnCode === 0) {
                intRate = res.interRest;
            } else {
                console.log(res.Message);
            }

            $("#FormSearch_interes_rate").text(FormatDecimal(intRate, 2));
            $("#FormSearch_interes_rate").val(FormatDecimal(intRate, 2));
        },
        error: function (error) {
            console.log(error);
            $("#FormSearch_interes_rate").text(FormatDecimal(0, 2));
            $("#FormSearch_interes_rate").val(FormatDecimal(0, 2));
        },
        statusCode: {
            401: function () {
                window.location.href = "/RPCallMargin";
            }
        }
    });
}

function CheckMarginIntRate() {
    $.ajax({
        url: "/RPCallMargin/CheckMarginIntRate",
        type: "POST",
        dataType: "JSON",
        data: {
            call_date: $('#FormSearch_call_date').val(),
            asof_date: setformatdateyyyymmdd($('#FormSearch_asof_date').val()),
            cur: $('#FormSearch_cur').val()
        },
        success: function (res) {
            var margin_int_rate_holiday;
            var flag_holiday = 'N';
            if (res.Success) {
                flag_holiday = res.flag_holiday;
                //margin_int_rate_holiday = res.margin_int_rate_holiday;
            } else {
                console.log(res.Message);
            }

            if (flag_holiday === 'Y') {
                document.getElementById("divHoliday").style.display = "block";
                $('#FormSearch_margin_int_rate_holiday').val(FormatDecimal(res.margin_int_rate_holiday, 2));
                $('#FormSearch_flag_holiday').val(flag_holiday);
            } else {
                document.getElementById("divHoliday").style.display = "none";
            }
        },
        error: function (error) {
            console.log(error);
        },
        statusCode: {
            401: function () {
                window.location.href = "/RPCallMargin";
            }
        }
    });
}


function checkValue(val) {
    if (val !== '' && val !== '.' && val !== '-') {
        return true;
    } else {
        return false;
    }
}

function checknumber(currentname) {
    if (checkValue($('input[name="' + currentname + '"]').val())) {
        $('input[name="' + currentname + '"]').val(FormatDecimal($('input[name="' + currentname + '"]').val().replace(/,/g, ''), 2));
    } else {
        $('input[name="' + currentname + '"]').val("0.00");
    }
}

var downloadPDF = function (arr_trans_no) {
    var url = "/RPConfirmation/DownloadPDF" + "?trans_no=" + arr_trans_no.join(',');
    window.open(url, '_blank');
    return;
};


function ReleaseMessage(listData) {

    $('.spinner').css('display', 'block');
    var messageError = '';

    var isCheck = true;

    $.ajax({
        url: "/RPCallMargin/ReleaseMessage",
        type: "GET",
        dataType: "JSON",
        data: { data: JSON.stringify(listData) },
        success: function (res) {
            if (res.returnCode === true) {
                $('#status_' + (listData[0].row - 1)).text("Complete");
                document.getElementById("status_" + (listData[0].row - 1)).setAttribute("style", "font-weight:normal;width:50px;color:green;");
                swal("Complete", "Release Message Successfully", "success");
            } else {
                isCheck = false;
                console.log(res.Message);
                messageError = res.Message;
                $('#status_' + (listData[0].row - 1)).text("Fail");
                document.getElementById("status_" + (listData[0].row - 1)).setAttribute("style", "font-weight:normal;width:50px;color:red;");
            }

            if (isCheck === false && listData.length === 1) {
                swal(
                    {
                        title: "Fail",
                        text: messageError,
                        html: true,
                        type: "error",
                        showCancelButton: false,
                        confirmButtonClass: "btn-success",
                        confirmButtonText: "Ok",
                        closeOnConfirm: true
                    }
                );
            }

            $('.spinner').css('display', 'none');

        },
        error: function (error) {
            $('.spinner').css('display', 'none');
            console.log(error);
        },
        statusCode: {
            401: function () {
                window.location.href = "/RPCallMargin";
            }
        }
    });
}


function ClearStatus() {
    var rowData = $('#x-table-data').DataTable().rows().data();
    for (var i = 0; i < rowData.length; i++) {
        $('#status_' + i).text("");
    }
}

function GetMarginTrigger() {
    $('#client_margin').val(null);
    $.ajax({
        url: "/RPCallMargin/GetMarginTrigger",
        type: "POST",
        dataType: "JSON",
        data: { category: 'MARGIN', item_code: 'Trigger' },
        success: function (res) {
            var marginTrigger = 0.0;

            if (res.returnCode === 0) {
                marginTrigger = res.marginTrigger;
            } else {
                console.log(res.Message);
            }

            $('#textTrigger').text(FormatDecimal(marginTrigger, 2) + ' %');


        },
        error: function (error) {
            console.log(error);
            $('#textTrigger').text(FormatDecimal(0, 2) + ' %');
        },
        statusCode: {
            401: function () {
                window.location.href = "/RPCallMargin";
            }
        }
    });
}

function getFormAction_eom_int_flag() {
    var value;
    var radioprp = $("[id=FormAction_eom_int_flag][value=true]");
    var radiobrp = $("[id=FormAction_eom_int_flag][value=false]");
    if (radioprp.attr("ischeck") == "true") {
        value = "Y";
    } else {
        value = "N";
    }
    return value;
}

$(window).on('load', function () {

    var defultCur = "THB";

    $("#ddl_cur").find(".selected-data").text(defultCur);
    $("#ddl_cur").find(".selected-value").val(defultCur);

    var budate = $("#BusinessDate").text();

    var formatmmddyyyydate = budate.split("/");
    var formatyyyyMMdd = formatmmddyyyydate[2] + "-" + formatmmddyyyydate[1] + "-" + formatmmddyyyydate[0];
    formatmmddyyyydate = formatmmddyyyydate[1] + "/" + formatmmddyyyydate[0] + "/" + formatmmddyyyydate[2];
    var callMarginDate = new Date(formatmmddyyyydate);

    console.log("callMarginDate : " + callMarginDate);

    //$('#FormSearch_asof_date').data("DateTimePicker").date(callMarginDate);

    $('#FormSearch_asof_date').val(budate);

    var data = {
        date: formatyyyyMMdd,
        cur: defultCur
    };

    GetInterestRate(data);
    GetNextBusinessDate(data);

    setTimeout(
        function () {
            GM.RPCallMargin.Search();
        },
        1500);
});

var myData;
var ctpyID;
var AS_OF_DATE;

$(document).ready(function () {

    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
        $('#modal-detail-prp').DataTable().columns.adjust();
        $('#modal-detail-brp').DataTable().columns.adjust();
    };

    GM.RPCallMargin = {};
    GM.RPCallMargin.Table = $('#x-table-data').DataTable({
        //drawCallback: function (row, data, index) {
        //    var api = this.api();
        //    myData = api.rows({ page: 'current' }).data();
        //    var x = document.getElementsByName("ddlPayment");
        //    for (var i = 0; i < myData.length; i++) {
        //        var dataSearch = { transdealtype: myData[i].margin_type.toUpperCase().indexOf('PAY') === 0 ? 'PAY' : myData[i].margin_type.toUpperCase().indexOf('RECEIVE') === 0 ? 'REV' : '', cur: $('#FormSearch_cur').val() === 'THB' ? $('#FormSearch_cur').val() : 'FCY' };
        //        if ($('#' + x[i].id + '').val() === "") {
        //            GM.Utility.DDLStandard(x[i].id, dataSearch, "\\RPCallMargin\\FillPaymentMethod", null);
        //        }
        //    }
        //},
        dom: 'Bfrtip',
        searching: true,
        scrollY: '35vh',
        scrollX: true,
        order: [],
        buttons: [
            'copy', 'csv', 'excel', 'pdf'
        ],
        processing: true,
        serverSide: true,
        ajax: {
            "url": "/RPCallMargin/Search",
            "type": "POST",
            "statusCode": {
                401: function () {
                    window.location.href = "/RPCallMargin";
                }
            },
            "error":
                function (error) {
                    console.log(error);
                }
        }
        ,
        columnDefs:
            [
                {
                    targets: 0,
                    data: "RowNumber",
                    searchable: false,
                    orderable: false,
                    width: 15,
                    className: 'dt-body-center',
                    render: function (data, type, row, meta) {
                        if (row.isCall === 1) {
                            return '<input type="checkbox"' + ' id="chk_' + meta.row + '" class="filter-ck" />';
                        }
                    }
                },
                {
                    targets: 1, data: "margin_type",
                    searchable: false,
                    orderable: false,
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return '<label id=margin_type_' + meta.row + ' style="font-weight:normal" value="' + row.margin_type + '" >' + row.margin_type + '</label>';
                        }
                        return data;
                    }
                },
                {
                    targets: 2, data: "counter_party_name",
                    orderable: false,
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return '<label id=counter_party_name_' + meta.row + ' style="font-weight:normal;text-overflow: ellipsis;overflow: hidden;width: 225px;white-space: nowrap;" value="' + row.counter_party_name + '" title="' + row.counter_party_name + '">' + row.counter_party_name + '</label>';
                        }
                        return data;
                    }
                },
                {
                    targets: 3, data: "fund_engname",
                    orderable: false,
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return '<label id=fund_engname_' + meta.row + ' style="font-weight:normal;text-overflow: ellipsis;overflow: hidden;width: 225px;white-space: nowrap;" value="' + row.fund_engname + '" title="' + row.fund_engname + '">' + row.fund_engname + '</label>';
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
                            html = FormatDecimal(data, 2);
                        } else {
                            html = Number("0").toFixed(2);
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
                            html = FormatDecimal(data, 2);
                        } else {
                            html = Number("0").toFixed(2);
                        }
                        return '<label id=int_cash_margin_' + meta.row + ' style="font-weight:normal" value="' + html + '" >' + html + '</label>';
                    }
                },
                {
                    targets: 6, data: "totalExposure",
                    orderable: false,
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = Number("0").toFixed(2);
                        }
                        return '<label id=totalExposure_' + meta.row + ' style="font-weight:normal" value="' + html + '" >' + html + '</label>';
                    }
                },
                {
                    targets: 7, data: "action",
                    className: 'dt-body-center',
                    searchable: false,
                    orderable: false,
                    width: 30,
                    render: function (data, type, row, meta) {
                        //var html = '<button title="Adjust Margin" class="btn btn-default btn-round" key="' + row.counter_party_id + '" call_date="' + moment(row.call_date).format('DD/MM/YYYY') + '" asof_date="' + moment(row.asof_date).format('DD/MM/YYYY') + '" isCall="' + row.isCall + '" trade_type="' + row.trade_type + '" form-mode="adjust" id="adjust_' + meta.row + '" data-action="adjust"  onclick="GM.RPCallMargin.Action(this)" ' + (row.isCall === 0 ? 'disabled' : '') + ' ><i class="feather-icon icon-edit"></i></button>';
                        //html += '<button title="Release Message" class="btn btn-default btn-round" key="' + row.counter_party_id + '" call_date="' + moment(row.call_date).format('DD/MM/YYYY') + '" asof_date="' + moment(row.asof_date).format('DD/MM/YYYY') + '" form-mode="genMessage" id="genMessage_' + meta.row + '" data-action="genMessage"  onclick="GM.RPCallMargin.Action(this)" ' + (row.isCall === 0 ? 'disabled' : '') + ' ><i class="feather-icon icon-file-plus"></i></button>';

                        var html = '<button title="Adjust Margin" class="btn btn-default btn-round" key="' + row.counter_party_id + '" call_date="' + moment(row.call_date).format('DD/MM/YYYY') + '" asof_date="' + moment(row.asof_date).format('DD/MM/YYYY') + '" isCall="' + row.isCall + '" trade_type="' + row.trade_type + '" form-mode="adjust" id="adjust_' + meta.row + '" data-action="adjust"  onclick="GM.RPCallMargin.Action(this)" ><i class="feather-icon icon-edit"></i></button>';
                        //html += '<button title="Release Message" class="btn btn-default btn-round" key="' + row.counter_party_id + '" call_date="' + moment(row.call_date).format('DD/MM/YYYY') + '" asof_date="' + moment(row.asof_date).format('DD/MM/YYYY') + '" form-mode="genMessage" id="genMessage_' + meta.row + '" data-action="genMessage"  onclick="GM.RPCallMargin.Action(this)" ><i class="feather-icon icon-file-plus"></i></button>';

                        return html;
                    }
                },
                {
                    targets: 8, data: "call_date", "visible": false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = moment(data).format('DD/MM/YYYY');
                        }
                        return '<label id=call_date_' + meta.row + ' style="font-weight:normal" value="' + html + '" >' + html + '</label>';
                    }
                },
                { targets: 9, data: "interes_rate", "visible": false },
                { targets: 10, data: "cur", "visible": false },
                {
                    targets: 11, data: "asof_date", "visible": false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = moment(data).format('DD/MM/YYYY');
                        }
                        return '<label id=asof_date_' + meta.row + ' style="font-weight:normal" value="' + html + '" >' + html + '</label>';
                    }
                },
                { targets: 12, data: "counter_party_id", "visible": false }
            ],
        fixedColumns: {
            leftColumns: 1,
            rightColumns: 6
        }
    });

    GM.RPCallMargin.Search = function () {
        try {
            var FormSearch_call_date = $("#FormSearch_call_date").val();
            var FormSearch_interes_rate = $("#FormSearch_interes_rate").val();
            var FormSearch_cur = $('#FormSearch_cur').val();
            var FormSearch_counter_party_id = $('#FormSearch_counter_party_id').val();

            GM.RPCallMargin.Table.columns(8).search(FormSearch_call_date);
            GM.RPCallMargin.Table.columns(9).search(FormSearch_interes_rate);
            GM.RPCallMargin.Table.columns(10).search(FormSearch_cur);
            GM.RPCallMargin.Table.columns(12).search(FormSearch_counter_party_id);
            GM.RPCallMargin.Table.draw();
        } catch (err) {
            console.log(err.message);
        }
    };

    GM.RPCallMargin.Submit = function (btn) {

        setTimeout(function () {
            swal({
                title: "Please confirm to Process?",
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

                        var formatmmddyyyydate = $("#FormSearch_asof_date").val().split("/");
                        var formatyyyyMMdd = formatmmddyyyydate[2] + "-" + formatmmddyyyydate[1] + "-" + formatmmddyyyydate[0];

                        var data = {
                            tradeDate: formatyyyyMMdd,
                            cur: $('#FormSearch_cur').val(),
                            rate: $("#FormSearch_interes_rate").val(),
                            holiday_rate: $("#FormSearch_margin_int_rate_holiday").val()
                        };

                        console.log('$("#FormSearch_margin_int_rate_holiday").val() : ' + $("#FormSearch_margin_int_rate_holiday").val());

                        $.ajax({
                            url: "/RPCallMargin/Process",
                            type: "POST",
                            dataType: "JSON",
                            data: data,
                            success: function (res) {
                                if (res.Success === true) {
                                    $('.spinner').css('display', 'none');
                                    swal({
                                        title: "Complete",
                                        text: "Call Margin Successfully ",
                                        type: "success",
                                        showCancelButton: false,
                                        confirmButtonClass: "btn-success",
                                        confirmButtonText: "Ok",
                                        closeOnConfirm: true
                                    },
                                        function (isConfirm) {
                                            if (isConfirm) {
                                                GM.RPCallMargin.Search();
                                            }
                                        }
                                    );
                                }
                                else {
                                    setTimeout(
                                        function () {
                                            $('.spinner').css('display', 'none');
                                            swal({
                                                title: "Fail",
                                                text: res.Message,
                                                type: "error",
                                                showCancelButton: false,
                                                confirmButtonClass: "btn-success",
                                                confirmButtonText: "Ok",
                                                closeOnConfirm: true
                                            });
                                        },
                                        200);
                                }
                            },
                            error: function (d) {
                                $('.spinner').css('display', 'none');
                                console.log(d);
                            },
                            statusCode: {
                                401: function () {
                                    window.location.href = "/RPCallMargin";
                                }
                            }
                        });
                    }
                }
            );
        }, 100);
    };

    GM.RPCallMargin.Action = function (btn) {
        var mode = $(btn).attr("form-mode");
        GM.Message.Clear();
        if (mode) {
            var key = $(btn).attr("key");
            var call_date = $(btn).attr("call_date");
            var asof_date = $(btn).attr("asof_date");
            var cur = $('#FormSearch_cur').val();
            var isCall = $(btn).attr("isCall");
            var trade_type = $(btn).attr("trade_type");
            console.log("trade_type : " + trade_type);

            if (cur === 'THB') {
                document.getElementById("from-adjust-fcy").style.display = "none";
            } else {
                if (isCall == 0 && trade_type == 'PRP') {
                    document.getElementById("from-adjust-fcy").style.display = "block";



                } else {
                    document.getElementById("from-adjust-fcy").style.display = "none";
                }

                //var radioyes = $("[id=FormAction_eom_int_flag][value=true]");
                //var radiono = $("[id=FormAction_eom_int_flag][value=false]");

                //radiono.click();

                //radioyes.attr('ischeck', 'false');
                //radiono.attr('ischeck', 'true');
                //radiono.attr("checked", "checked");
                //radioyes.removeAttr("checked");

            }



            switch (mode) {
                case "adjust":
                    if (trade_type === 'PRP') {
                        setTimeout(
                            function () {
                                GM.RPCallMargin.Detail.Table.columns(23).search(call_date);
                                GM.RPCallMargin.Detail.Table.columns(2).search(cur);
                                GM.RPCallMargin.Detail.Table.columns(25).search(key);
                                GM.RPCallMargin.Detail.Table.columns(26).search(isCall);
                                ctpyID = key;
                                AS_OF_DATE = asof_date;
                                GM.RPCallMargin.Detail.Table.draw();
                                adjusttable();

                                GetMarginTrigger();
                            },
                            200);
                        $('#adjust-prp').modal('toggle');
                    } else {
                        setTimeout(
                            function () {
                                GM.RPCallMargin.DetailBRP.Table.columns(22).search(call_date);
                                GM.RPCallMargin.DetailBRP.Table.columns(1).search(cur);
                                GM.RPCallMargin.DetailBRP.Table.columns(24).search(key);
                                GM.RPCallMargin.DetailBRP.Table.columns(25).search(isCall);
                                ctpyID = key;
                                AS_OF_DATE = asof_date;
                                GM.RPCallMargin.DetailBRP.Table.draw();
                                adjusttable();
                            },
                            200);
                        $('#adjust-brp').modal('toggle');
                    }
                    $('#btnSubmitAdjust').add('disabled', 'disabled');
                    break;
                case "genMessage":
                    var id = $(btn).attr("id").split('genMessage_')[1];
                    var rowData = $('#x-table-data').DataTable().rows().data();
                    {
                        if (rowData[id].payment_method === null || rowData[id].mt_code === null) {
                            swal("Warning", "Please select Payment Method", "error");
                            $('#ddl_payment_' + (rowData[id].RowNumber - 1)).focus();
                            $('#status_' + (rowData[id].RowNumber - 1)).text("Fail");
                            document.getElementById("status_" + (rowData[id].RowNumber - 1)).setAttribute("style", "font-weight:normal;width:50px;color:red;");
                        } else {
                            $('#status_' + (rowData[id].RowNumber - 1)).text("");
                            console.log('rowData[i].call_date :' + moment(rowData[id].call_date).format('YYYY-MM-DD'));
                            var listData = [];
                            var row =
                            {
                                counter_party_id: rowData[id].counter_party_id,
                                call_date: moment(rowData[id].call_date).format('YYYY-MM-DD'),
                                payment_method: rowData[id].payment_method,
                                mt_code: rowData[id].mt_code,
                                cur: $('#FormSearch_cur').val(),
                                row: rowData[id].RowNumber,
                                counter_party_name: rowData[id].counter_party_name
                            };
                            listData.push(row);
                            ReleaseMessage(listData);
                        }
                    }
                    break;
            }
        }
    };

    GM.RPCallMargin.PrintAnnexII = function (btn) {
        $('.spinner').css('display', 'block');

        var formatmmddyyyydate = $("#FormSearch_call_date").val().split("/");
        var formatyyyyMMdd = formatmmddyyyydate[2] + "-" + formatmmddyyyydate[1] + "-" + formatmmddyyyydate[0];
        var counterPartyId = '';

        var rowData = $('#x-table-data').DataTable().rows($('#x-table-data .filter-ck:checked').closest('tr')).data();
        for (var i = 0; i < rowData.length; i++) {
            counterPartyId += rowData[i].counter_party_id + '|';
        }

        var data = {
            asOfDate: formatyyyyMMdd,
            counterPartyId: counterPartyId
        };

        $.ajax({
            url: "/RPCallMargin/MarginDetail",
            type: "POST",
            dataType: "JSON",
            data: data,
            success: function (res) {
                $('.spinner').css('display', 'none');
                if (res.Success === true) {

                    var listTransNo = [];
                    for (var i = 0; i < res.result.length; i++) {
                        listTransNo.push(res.result[i]);
                    }
                    if (listTransNo.length > 0) {
                        downloadPDF(listTransNo);
                    }
                }
                else {
                    setTimeout(
                        function () {
                            swal({
                                title: "Fail",
                                text: res.Message,
                                type: "error",
                                showCancelButton: false,
                                confirmButtonClass: "btn-success",
                                confirmButtonText: "Ok",
                                closeOnConfirm: true
                            });
                        },
                        200);
                }
            },
            error: function (error) {
                $('.spinner').css('display', 'none');
                console.log(error);
            },
            statusCode: {
                401: function () {
                    window.location.href = "/RPCallMargin";
                }
            }
        });
    };

    GM.RPCallMargin.GenPDF = function (btn) {
        $('.spinner').css('display', 'block');
        var rowData = $('#x-table-data').DataTable().rows($('#x-table-data .filter-ck:checked').closest('tr')).data();
        var listData = [];
        for (var i = 0; i < rowData.length; i++) {

            var row = {
                margin_type: rowData[i].margin_type,
                counter_party_name: rowData[i].counter_party_name,
                margin_amt: rowData[i].margin_amt,
                int_cash_margin: rowData[i].int_cash_margin,
                status: '',
                payment_method: ''
            };
            listData.push(row);
        }

        if (listData.length > 0) {
            var dataToPost = { data: JSON.stringify(listData) };
            $.post('/RPCallMargin/GenPDF', dataToPost)
                .done(function (response) {
                    window.location.href = '/RPCallMargin/DownloadPDF?filename=' + response.fileName;

                    $('.spinner').css('display', 'none');
                    swal({
                        title: "Complete",
                        text: "Gen PDF Successfully ",
                        type: "success",
                        showCancelButton: false,
                        confirmButtonClass: "btn-success",
                        confirmButtonText: "Ok",
                        closeOnConfirm: true
                    });
                });
        }
        else {
            $('.spinner').css('display', 'none');
        }
    };

    GM.RPCallMargin.GenExcel = function (btn) {
        $('.spinner').css('display', 'block');
        var rowData = $('#x-table-data').DataTable().rows($('#x-table-data .filter-ck:checked').closest('tr')).data();
        var listData = [];
        for (var i = 0; i < rowData.length; i++) {

            var row = {
                margin_type: rowData[i].margin_type,
                counter_party_name: rowData[i].counter_party_name,
                margin_amt: rowData[i].margin_amt,
                int_cash_margin: rowData[i].int_cash_margin,
                status: '',
                payment_method: ''
            };
            listData.push(row);
        }

        if (listData.length > 0) {
            var dataToPost = { data: JSON.stringify(listData) };
            $.post('/RPCallMargin/GenExcel', dataToPost)
                .done(function (response) {
                    window.location.href = '/RPCallMargin/Download?filename=' + response.fileName;
                    $('.spinner').css('display', 'none');
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
                });
        }
        else {
            $('.spinner').css('display', 'none');
        }
    };

    GM.RPCallMargin.ReleaseSwift = function (btn) {
        var rowData = $('#x-table-data').DataTable().rows($('#x-table-data .filter-ck:checked').closest('tr')).data();
        var valid = true;
        var listData = [];
        for (var i = 0; i < rowData.length; i++) {
            console.log('rowData[i].payment_method :' + rowData[i].payment_method);
            console.log('rowData[i].mt_code :' + rowData[i].mt_code);
            if (rowData[i].payment_method === null || rowData[i].mt_code === null) {
                $('#ddl_payment_' + (rowData[i].RowNumber - 1)).click();
                valid = false;
                $('#status_' + (rowData[i].RowNumber - 1)).text("Fail");
                document.getElementById("status_" + (rowData[i].RowNumber - 1)).setAttribute("style", "font-weight:normal;width:50px;color:red;");
            } else {
                console.log('rowData[i].call_date :' + moment(rowData[i].call_date).format('YYYY-MM-DD'));
                $('#status_' + (rowData[i].RowNumber - 1)).text("");
                var row =
                {
                    counter_party_id: rowData[i].counter_party_id,
                    call_date: moment(rowData[i].call_date).format('YYYY-MM-DD'),
                    payment_method: rowData[i].payment_method,
                    mt_code: rowData[i].mt_code,
                    cur: $('#FormSearch_cur').val(),
                    row: rowData[i].RowNumber,
                    counter_party_name: rowData[i].counter_party_name
                };
                listData.push(row);
            }
        }

        if (valid) {
            ReleaseMessage(listData);
        } else {
            swal("Warning", "Please select Payment Method", "error");
        }
    };

    GM.RPCallMargin.Detail = {};
    GM.RPCallMargin.Detail.Table = $('#modal-detail-prp').DataTable({
        dom: 'Bfrtip',
        select: false,
        searching: true,
        scrollY: '35vh',
        scrollX: true,
        bInfo: false,
        bPaginate: false,
        buttons: [],
        processing: true,
        serverSide: true,
        ordering: false,
        ajax: {
            "url": "/RPCallMargin/SearchPRP",
            "type": "POST",
            "statusCode": {
                401: function () {
                    window.location.href = "/RPCallMargin";
                }
            },
            "error":
                function (error) {
                    console.log(error);
                }
        },
        columnDefs:
            [
                {
                    targets: 0, data: "counter_party_code", orderable: false
                },
                {
                    targets: 1, data: "threshold", orderable: false,
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 2, data: "cur"
                },
                {
                    targets: 3, data: "exposure",
                    render: function (data, type, row, meta) {
                        var html = '';
                        var currentname = 'MarginList[' + meta.row + '].Exposure';
                        var disable = false;
                        if (row.event_seq === 1 || row.event_seq === 4 || row.event_seq === 5) {
                            disable = true;
                        }

                        if (row.isCall === 0) {
                            disable = true;
                        }

                        if (data != null) {
                            var valuedata = data;
                            if (disable) {
                                html = '<input onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);" style="background-color:lightGray;width:110px;" readonly="readonly" name="' + currentname + '" id="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="' + FormatDecimal(valuedata, 2) + '">';
                            } else {
                                html = '<input onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);" style="width:110px;" name="' + currentname + '" id="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="' + FormatDecimal(valuedata, 2) + '">';
                            }
                        }
                        else {
                            if (disable) {
                                html = '<input onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);" style="background-color:lightGray;width:110px;" readonly="readonly" name="' + currentname + '" id="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="0.00">';
                            } else {
                                html = '<input onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);" style="width:110px;" name="' + currentname + '" id="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="0.00">';
                            }
                        }
                        return html;
                    }
                },
                {
                    targets: 4, data: "ORG_TODAY_EXPOSURE",
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 5, data: "TOTAL_INT_REC",
                    render: function (data, type, row, meta) {
                        var html = '';
                        var currentname = 'MarginList[' + meta.row + '].TOTAL_INT_REC';
                        var valuedata = 0;
                        if (data != null) {
                            valuedata = data;
                        }

                        if (row.isCall === 0) {
                            html = '<input style="background-color:lightGray;width:110px;" readonly="readonly" onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);"  name="' + currentname + '" id="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="' + FormatDecimal(valuedata, 2) + '">';
                        }
                        else {
                            html = '<input style="width:110px;" onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);"  name="' + currentname + '" id="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="' + FormatDecimal(valuedata, 2) + '">';
                        }

                        return html;
                    }
                },
                {
                    targets: 6, data: "ORG_TRANS_MARGIN_INT_REC",
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 7, data: "INT_REC_TAX",
                    render: function (data, type, row, meta) {
                        var html = '';
                        var currentname = 'MarginList[' + meta.row + '].INT_REC_TAX';
                        if (data != null) {
                            var valuedata = data;
                            if (row.isCall === 0) {
                                html = '<input style="background-color:lightGray;width:110px;" readonly="readonly" onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);"  name="' + currentname + '" id="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="' + FormatDecimal(valuedata, 2) + '">';
                            }
                            else {
                                html = '<input style="width:110px;" onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);"  name="' + currentname + '" id="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="' + FormatDecimal(valuedata, 2) + '">';
                            }
                        }
                        else {
                            if (row.isCall === 0) {
                                html = '<input style="background-color:lightGray;width:110px;" readonly="readonly" onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);" name="' + currentname + '" id="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="0">';
                            } else {
                                html = '<input style="width:110px;" onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);" name="' + currentname + '" id="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="0">';
                            }
                        }
                        return html;
                    }
                },
                {
                    targets: 8, data: "ORG_TRANS_MARGIN_INT_TAX",
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 9, data: "TOTAL_INT_PAY",
                    render: function (data, type, row, meta) {
                        var html = '';
                        var currentname = 'MarginList[' + meta.row + '].TOTAL_INT_PAY';
                        if (data != null) {
                            var valuedata = data;
                            if (row.isCall === 0) {
                                html = '<input style="background-color:lightGray;width:110px;" readonly="readonly" onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);"  name="' + currentname + '" id="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="' + FormatDecimal(valuedata, 2) + '">';
                            }
                            else {
                                html = '<input style="width:110px;" onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);"  name="' + currentname + '" id="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="' + FormatDecimal(valuedata, 2) + '">';
                            }
                        }
                        else {
                            if (row.isCall === 0) {
                                html = '<input style="background-color:lightGray;width:110px;" readonly="readonly" onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);" name="' + currentname + '" id="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="0">';
                            } else {
                                html = '<input style="width:110px;" onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);" name="' + currentname + '" id="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="0">';
                            }
                        }
                        return html;
                    }
                },
                {
                    targets: 10, data: "ORG_TRANS_MARGIN_INT_PAY",
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 11, data: "prev_position_margin",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 12, data: "margin",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 13, data: "accrue_int_yesterday",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 14, data: "call_margin",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 15, data: "close_margin",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 16, data: "minimum_transfer",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 17, data: "margin_balance",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 18, data: "IntRateToday",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 19, data: "IntPerDay",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 20, data: "net_accrue_int_today",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 21, data: "net_accrue_int_yesterday",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 22, data: "remark"
                },
                { targets: 23, data: "call_date", "visible": false },
                { targets: 24, data: "asof_date", "visible": false },
                { targets: 25, data: "counter_party_id", "visible": false },
                { targets: 26, data: "isCall", "visible": false }
            ],
        footerCallback: function (row, data, start, end, display) {

            if (data.length > 0 && data[0].isCall === 0) {
                //$('#btnSubmitAdjust').addClass("hidden");
            } else {
                //$('#btnSubmitAdjust').removeClass("hidden");
            }

            if (data.length > 0 && data[0].eom_int_flag === 'Y') {
                var radioyes = $("[id=FormAction_eom_int_flag][value=true]");
                radioyes.click();
            } else if (data.length > 0 && data[0].eom_int_flag === 'N') {
                var radiono = $("[id=FormAction_eom_int_flag][value=false]");
                radiono.click();
            }
        },
        fixedColumns: {
            leftColumns: 1,
            rightColumns: 1
        }
    });

    GM.RPCallMargin.Detail.Submit = function (btn) {
        $('.spinner').css('display', 'block');

        var rowData = $('#modal-detail-prp').DataTable().rows().data();
        var listData = [];
        for (var i = 0; i < rowData.length; i++) {

            var row = {
                asof_date: setformatdateyyyymmdd(AS_OF_DATE),
                counter_party_id: ctpyID,
                Exposure: $('input[name="MarginList[' + i + '].Exposure"]').val(),
                TOTAL_INT_REC: $('input[name="MarginList[' + i + '].TOTAL_INT_REC"]').val(),
                TOTAL_INT_PAY: $('input[name="MarginList[' + i + '].TOTAL_INT_PAY"]').val(),
                INT_REC_TAX: $('input[name="MarginList[' + i + '].INT_REC_TAX"]').val(),
                cur: rowData[i].cur,
                client_margin: $('#client_margin').val() === '' ? null : $('#client_margin').val(),
                isCall: $('#client_margin').val() === '' ? 0 : 1,
                eom_int_flag: getFormAction_eom_int_flag()
            };
            listData.push(row);
        }

        var dataToPost = { data: JSON.stringify(listData) };
        console.log("dataToPost : " + dataToPost);
        $.post('/RPCallMargin/AdjustPRP', dataToPost)
            .done(function (res) {
                $('.spinner').css('display', 'none');
                if (res.Success === true) {
                    swal({
                        title: "Complete",
                        text: "Adjust Margin Successfully ",
                        type: "success",
                        showCancelButton: false,
                        confirmButtonClass: "btn-success",
                        confirmButtonText: "Ok",
                        closeOnConfirm: true
                    }
                    );
                    setTimeout(
                        function () {
                            GM.RPCallMargin.Search();
                        }, 100);
                }
                else {
                    setTimeout(
                        function () {
                            swal({
                                title: "Fail",
                                text: res.Message,
                                type: "error",
                                showCancelButton: false,
                                confirmButtonClass: "btn-success",
                                confirmButtonText: "Ok",
                                closeOnConfirm: true
                            }
                            );
                        },
                        200);
                }
            });
    };

    GM.RPCallMargin.Detail.Process = function (btn) {

        var data = {
            asof_date: setformatdateyyyymmdd(AS_OF_DATE),
            counter_party_id: ctpyID,
            cur: $("#FormSearch_cur").val(),
            client_margin: parseFloat($('#client_margin').val().replace(/,/g, ''))
        }

        var dataToPost = { data: JSON.stringify(data) };

        $.post('/RPCallMargin/CheckMarginTrigger', dataToPost)
            .done(function (res) {
                $('.spinner').css('display', 'none');
                if (res.Success === true) {

                    if (res.isTrigger === true) {

                        var Exposure = document.getElementById('MarginList[0].Exposure');
                        Exposure.style.removeProperty("background-color");
                        Exposure.removeAttribute("readonly");

                        var TOTAL_INT_REC = document.getElementById('MarginList[0].TOTAL_INT_REC');
                        TOTAL_INT_REC.style.removeProperty("background-color");
                        TOTAL_INT_REC.removeAttribute("readonly");

                        var INT_REC_TAX = document.getElementById('MarginList[0].INT_REC_TAX');
                        INT_REC_TAX.style.removeProperty("background-color");
                        INT_REC_TAX.removeAttribute("readonly");

                        var TOTAL_INT_PAY = document.getElementById('MarginList[0].TOTAL_INT_PAY');
                        TOTAL_INT_PAY.style.removeProperty("background-color");
                        TOTAL_INT_PAY.removeAttribute("readonly");

                        $('#btnSubmitAdjust').removeClass("hidden");
                    } else {


                        setTimeout(
                            function () {
                                swal({
                                    title: "Fail",
                                    text: "Client Margin is Over KTB Trigger",
                                    type: "error",
                                    showCancelButton: false,
                                    confirmButtonClass: "btn-success",
                                    confirmButtonText: "Ok",
                                    closeOnConfirm: true
                                }
                                );
                            },
                            200);
                    }
                }
                else {
                    $('.spinner').css('display', 'none');
                    setTimeout(
                        function () {
                            swal({
                                title: "Fail",
                                text: res.Message,
                                type: "error",
                                showCancelButton: false,
                                confirmButtonClass: "btn-success",
                                confirmButtonText: "Ok",
                                closeOnConfirm: true
                            }
                            );
                        },
                        200);
                }
            });


    };

    GM.RPCallMargin.DetailBRP = {};
    GM.RPCallMargin.DetailBRP.Table = $('#modal-detail-brp').DataTable({
        dom: 'Bfrtip',
        select: false,
        searching: true,
        scrollY: '35vh',
        scrollX: true,
        bInfo: false,
        bPaginate: false,
        buttons: [],
        processing: true,
        serverSide: true,
        ordering: false,
        ajax: {
            "url": "/RPCallMargin/SearchBRP",
            "type": "POST",
            "statusCode": {
                401: function () {
                    window.location.href = "/RPCallMargin";
                }
            },
            "error":
                function (error) {
                    console.log(error);
                }
        },
        columnDefs:
            [
                {
                    targets: 0, data: "contract_no", orderable: false
                },
                {
                    targets: 1, data: "cur", orderable: false
                },
                {
                    targets: 2, data: "exposure",
                    render: function (data, type, row, meta) {
                        var html = '';
                        var currentname = 'MarginList[' + meta.row + '].Exposure';
                        var disable = false;
                        console.log("row.event_seq : " + row.event_seq);
                        if (row.event_seq === 1 || row.event_seq === 3) {
                            disable = true;
                        }

                        if (row.isCall === 0) {
                            disable = true;
                        }

                        if (data != null) {
                            var valuedata = data;
                            if (disable) {
                                html = '<input style="background-color:lightGray;width:110px;" readonly="readonly" name="' + currentname + '" type="text" value="' + valuedata + '">';
                            } else {
                                html = '<input onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);" style="width:110px;" name="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="' + FormatDecimal(valuedata, 2) + '">';
                            }
                        }
                        else {
                            if (disable) {
                                html = '<input onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);" style="background-color:lightGray;width:110px;" readonly="readonly" name="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="0.00">';
                            } else {
                                html = '<input onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);" style="width:110px;" name="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="0.00">';
                            }
                        }
                        return html;
                    }
                },
                {
                    targets: 3, data: "ORG_TODAY_EXPOSURE",
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 4, data: "TRANS_MARGIN_INT_REC",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {

                        var html = '';
                        var currentname = 'MarginList[' + meta.row + '].Int_Recv_Disp';
                        if (data != null) {
                            var valuedata = data;
                            if (row.isCall === 0) {
                                html = FormatDecimal(data, 2);
                            }
                            else {
                                html = '<input style="width:110px;" onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);"  name="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="' + FormatDecimal(valuedata, 2) + '">';
                            }
                        }
                        else {
                            if (row.isCall === 0) {
                                html = FormatDecimal('0', 2);
                            } else {
                                html = '<input style="width:110px;" onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);" name="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="0">';
                            }
                        }
                        return html;
                    }
                },
                {
                    targets: 5, data: "ORG_TRANS_MARGIN_INT_REC",
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 6, data: "TRANS_MARGIN_INT_TAX",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        var currentname = 'MarginList[' + meta.row + '].Int_Tax_Disp';
                        if (data != null) {
                            var valuedata = data;
                            if (row.isCall === 0) {
                                html = FormatDecimal(data, 2);
                            }
                            else {
                                html = '<input style="width:110px;" onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);"  name="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="' + FormatDecimal(valuedata, 2) + '">';
                            }
                        }
                        else {
                            if (row.isCall === 0) {
                                html = FormatDecimal('0', 2);
                            } else {
                                html = '<input style="width:110px;" onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);" name="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="0">';
                            }
                        }
                        return html;
                    }
                },
                {
                    targets: 7, data: "ORG_TRANS_MARGIN_INT_TAX",
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 8, data: "TRANS_MARGIN_INT_PAY",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        var currentname = 'MarginList[' + meta.row + '].Int_Paid_Disp';
                        if (data != null) {
                            var valuedata = data;
                            if (row.isCall === 0) {
                                html = FormatDecimal(data, 2);
                            }
                            else {
                                html = '<input style="width:110px;" onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);"  name="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="' + FormatDecimal(valuedata, 2) + '">';
                            }
                        }
                        else {
                            if (row.isCall === 0) {
                                html = FormatDecimal('0', 2);
                            } else {
                                html = '<input style="width:110px;" onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this);" name="' + currentname + '" onchange="checknumber(\'' + currentname + '\')" type="text" value="0">';
                            }
                        }
                        return html;
                    }
                },
                {
                    targets: 9, data: "ORG_TRANS_MARGIN_INT_PAY",
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 10, data: "position_yesterday",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 11, data: "call_margin",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 12, data: "close_margin",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 13, data: "Margin_Disp",
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 14, data: "margin_balance",
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 15, data: "int_rate",
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 16, data: "IntPerDay",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 17, data: "IntRecToday",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 18, data: "IntRecYesterday",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 19, data: "IntPayToday",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 20, data: "IntPayYesterday",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = FormatDecimal('0', 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 21, data: "BRP_REMARK"
                },
                { targets: 22, data: "call_date", "visible": false },
                { targets: 23, data: "asof_date", "visible": false },
                { targets: 24, data: "counter_party_id", "visible": false },
                { targets: 25, data: "isCall", "visible": false },
                { targets: 26, data: "trans_no", "visible": false },
                { targets: 27, data: "bond_id", "visible": false }
            ],
        footerCallback: function (row, data, start, end, display) {
            if (data.length > 0) {
                if (data[0].isCall === 0) {
                    $('#btnSubmitAdjustBrp').addClass("hidden");
                } else {
                    $('#btnSubmitAdjustBrp').removeClass("hidden");
                }
                console.log("data[0].threshold : " + data[0].threshold);
                if (data[0].threshold !== '') {
                    $('#lbl_threshold').text(FormatDecimal(data[0].threshold, 2));
                }
            }

        },
        fixedColumns: {
            leftColumns: 1,
            rightColumns: 1
        }
    });
    GM.RPCallMargin.DetailBRP.Submit = function (btn) {
        $('.spinner').css('display', 'block');

        var rowData = $('#modal-detail-brp').DataTable().rows().data();
        var listData = [];
        console.log("ctpyID : " + ctpyID);
        for (var i = 0; i < rowData.length; i++) {
            var row = {
                asof_date: setformatdateyyyymmdd(AS_OF_DATE),
                counter_party_id: ctpyID,
                trans_no: rowData[i].trans_no,
                cur: rowData[i].cur,
                contract_no: rowData[i].contract_no,
                exposure: $('input[name="MarginList[' + i + '].Exposure"]').val(),
                Int_Recv_Disp: $('input[name="MarginList[' + i + '].Int_Recv_Disp"]').val(),
                Int_Tax_Disp: $('input[name="MarginList[' + i + '].Int_Tax_Disp"]').val(),
                Int_Paid_Disp: $('input[name="MarginList[' + i + '].Int_Paid_Disp"]').val(),
                bond_id: rowData[i].bond_id
            };
            listData.push(row);
        }
        var dataToPost = { data: JSON.stringify(listData) };
        console.log("dataToPost : " + dataToPost);
        $.post('/RPCallMargin/AdjustBRP', dataToPost)
            .done(function (res) {
                $('.spinner').css('display', 'none');
                if (res.Success === true) {
                    swal({
                        title: "Complete",
                        text: "Adjust Margin Successfully ",
                        type: "success",
                        showCancelButton: false,
                        confirmButtonClass: "btn-success",
                        confirmButtonText: "Ok",
                        closeOnConfirm: true
                    });
                    setTimeout(
                        function () {
                            GM.RPCallMargin.Search();
                        }, 100);
                }
                else {
                    setTimeout(
                        function () {
                            swal({
                                title: "Fail",
                                text: res.Message,
                                type: "error",
                                html: true,
                                showCancelButton: false,
                                confirmButtonClass: "btn-success",
                                confirmButtonText: "Ok",
                                closeOnConfirm: true
                            }
                            );
                        },
                        200);
                }
            });
    };

    numberOnlyAndDotAndMinute = function (obj) {
        obj.value = obj.value
            .replace(/[^\d.-]/g, '') // numbers and decimals and dot and minute only
            .replace(/(^[\d]{22})[\d]/g, '$1') // not more than 22 digits at the beginning
            .replace(/(^-[\d]{22})[\d]/g, '$1') // not more than 22 digits at the beginning
            .replace(/(\--*)\-/g, '$1') // decimal can't exist more than once
            .replace(/(\..*)\./g, '$1') // decimal can't exist more than once
            .replace(/(\.[\d]{2})./g, '$1'); // not more than 2 digits after decimal
    };

    $("#client_margin").on("change focusout", function () {
        if ($('#client_margin').val() !== '' && $('#client_margin').val() !== '-') {
            var client_margin = parseFloat($('#client_margin').val().replace(/,/g, ''));
            $('#client_margin').val(FormatDecimal(client_margin, 2));
        } else {
            $('#client_margin').val("");
        }
    });

    //Function : Checkbox
    $('#x-table-data').on('click', 'tr', function () {
        GM.RPCallMargin.Table = $('#x-table-data').DataTable();
        var data = GM.RPCallMargin.Table.row(this).data();

        if (typeof data !== 'undefined') {
            var id = 'chk_' + GM.RPCallMargin.Table.row(this).index();
            if ($(this).find('.filter-ck').prop('checked') === true) {
                var inputyes = $("[id=" + id + "]");
                inputyes.attr('checked', 'checked');
                inputyes.prop('checked', true);
            } else {
                var inputno = $("[id=" + id + "]");
                inputno.prop('checked', false);
            }
        }

        var isCheck = false;

        $('#x-table-data tbody tr').each(function (i, row) {

            if ($('#chk_' + i).prop('checked')) {
                isCheck = true;
                return;
            }
        });

        if (isCheck) {
            $('#btnPrint').removeAttr('disabled');
            $('#btnPDF').removeAttr('disabled');
            $('#btnExcel').removeAttr('disabled');
        } else {
            $('#btnPrint').attr('disabled', 'disabled');
            $('#btnPDF').attr('disabled', 'disabled');
            $('#btnExcel').attr('disabled', 'disabled');
        }

    });

    //Function : Checkbox All
    $('#x-table-data_wrapper').on("click", '#CheckAll', function () {
        if ($(this).prop('checked') === true) {

            $('.filter-ck').prop('checked', true);
            setTimeout(
                function () {
                    var rowData = $('#x-table-data').DataTable().rows($('#x-table-data .filter-ck:checked').closest('tr')).data();
                    if (rowData.length > 0) {
                        $('#btnPrint').removeAttr('disabled');
                        $('#btnPDF').removeAttr('disabled');
                        $('#btnExcel').removeAttr('disabled');
                    }
                },
                200);
        }
        else {
            $('.filter-ck').prop('checked', false);

            $('#btnPrint').attr('disabled', 'disabled');
            $('#btnPDF').attr('disabled', 'disabled');
            $('#btnExcel').attr('disabled', 'disabled');
        }
    });

    GM.RPCallMargin.Table.ClickCell = function (action, id, defaultvalue) {
        var data = { datastr: null };
        GM.Utility.DDLStandard(id, data, action, defaultvalue);
    };

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
        var data = {
            date: setformatdateyyyymmdd($('#FormSearch_asof_date').val()),
            cur: $('#FormSearch_cur').val()
        };

        GetInterestRate(data);
        GetNextBusinessDate(data);



        setTimeout(
            function () {
                GM.RPCallMargin.Search();
            },
            1000);
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

    $("#ddl_margin").click(function () {
        var txt_search = $('#txt_margin');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });

    $('#txt_margin').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null, false);
    });

    $("#btnSearch").click(function () {
        GM.RPCallMargin.Search();
    });

    $('#FormSearch_asof_date').on('dp.change', function (e) {
        var data = {
            date: setformatdateyyyymmdd($('#FormSearch_asof_date').val()),
            cur: $('#FormSearch_cur').val()
        };

        GetNextBusinessDate(data);
        GetInterestRate(data);

        //setTimeout(
        //    function () {
        //        CheckMarginIntRate();
        //    },
        //    500);

        setTimeout(
            function () {
                GM.RPCallMargin.Search();
            },
            1000);
    });

    $('.radio input[id=FormAction_eom_int_flag]').change(function () {
        var current = $(this).val();
        var radioyes = $("[id=FormAction_eom_int_flag][value=true]");
        var radiono = $("[id=FormAction_eom_int_flag][value=false]");
        if (current == "true") {
            radioyes.attr('ischeck', 'true');
            radiono.attr('ischeck', 'false');
            radioyes.attr("checked", "checked");
            radiono.removeAttr("checked");
        }
        else {
            radioyes.attr('ischeck', 'false');
            radiono.attr('ischeck', 'true');
            radiono.attr("checked", "checked");
            radioyes.removeAttr("checked");
        }
    })
});

