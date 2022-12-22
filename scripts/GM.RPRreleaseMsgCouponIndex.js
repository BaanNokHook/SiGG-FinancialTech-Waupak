
function FormatDecimal(Num, point) {
    var format = Number(parseFloat(Num).toFixed(point)).toLocaleString('en', {
        minimumFractionDigits: point
    });
    return format;
}

function ReleaseMessage() {
    var rowData = $('#x-table-data').DataTable().rows($('#x-table-data .filter-ck:checked').closest('tr')).data();
    var transcNoList = [];
    for (var i = 0; i < rowData.length; i++) {
        console.log(rowData[i].trans_cno);
        transcNoList.push(rowData[i].trans_cno);
    }

    if (transcNoList.length > 0) {
        window.location.href = "/RPReleaseMessage/SelectCoupon?id=" + JSON.stringify(transcNoList);
    }
    else {
        swal("Warning", "Please Select [Trans cNo] To RPReleaseMessage", "error");
    }
}


function GetReleaseMT(trans_no, cur, mt_code) {
    $("#check_releasemt").modal('toggle');

    var data = {
        trans_no: trans_no,
        from_page: 'COUPON',
        event_type: 'CNP',
        trans_deal_type: mt_code === 'MT202' ? 'PAY' : 'REV',
        payment_method: 'SWIFT',
        trans_mt_code: mt_code,
        cur: cur
    };

    $.ajax({
        type: "GET",
        url: "/RPReleaseMessage/GetReleaseMT",
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: data,
        success: function (res) {

            var body = $("#modal_release_mt").find("tbody");
            var html = "";
            var totalamount = 0;
            var isin_code = "";
            body.html("");
            $.each(res, function (i, resdata) {

                if (isin_code != resdata.isin_code) {
                    html = "<tr><td><b> ISIN Code:" + $('<div/>').text(resdata.isin_code).html() + "<b></td></tr>";
                    body.append(html);
                }
                if (resdata.mt_message !== '' && resdata.mt_message !== null) {
                    html = "<tr><td>" + $('<div/>').text(resdata.mt_message).html() + "</td></tr>";
                } else {
                    html = "<tr><td>&nbsp;</td></tr>";
                }
                body.append(html);
                isin_code = resdata.isin_code;

            });
        },
        error: function (res) {
            // TODO: Show error
            GM.Message.Clear();
            console.log(res.Message);
        }
    });
}


$(window).on('load', function () {

    var budate = $("#BusinessDate").text();

    var formatmmddyyyydate = budate.split("/");
    formatmmddyyyydate = formatmmddyyyydate[1] + "/" + formatmmddyyyydate[0] + "/" + formatmmddyyyydate[2];
    var payment_date = new Date(formatmmddyyyydate);

    //$('#FormSearch_payment_date').data("DateTimePicker").date(payment_date);

    $('#FormSearch_payment_date').val(budate);

    setTimeout(
        function () {
            GM.RPCoupon.Search();
        },
        500);
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
        if (eventType === 'Margin') {
            window.location.href = "/RPReleaseMessage/IndexCallMargin";
        } else if (eventType === 'Interest Margin') {
            window.location.href = "/RPReleaseMessage/IndexInterestMargin";
        } else if (eventType === 'Settlement' || eventType === 'Maturity') {
            window.location.href = "/RPReleaseMessage/Index?search=" + eventType;
        } else if (eventType === 'Net-Settlement') {
            window.location.href = "/RPReleaseMessage/IndexNetSettlement?search=" + eventType;
        }
    });

    $("#ddl_instrument").click(function () {
        var txt_search = $('#txt_instrument');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_instrument').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
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

    $("#ddl_counterparty_fund").click(function () {
        var txt_search = $('#txt_counterparty_fund');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_counterparty_fund').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //End Binding : DDL

    //Binding Div Detail
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    };
    GM.RPCoupon = {};
    GM.RPCoupon.Search = function () {
        var FormSearch_payment_date = $("#FormSearch_payment_date").val();
        var FormSearch_instrument_code = $("#FormSearch_instrument_code").val();
        var FormSearch_counter_party_id = $("#FormSearch_counter_party_id").val();
        var FormSearch_fund_code = $("#FormSearch_fund_code").val();
        var FormSearch_instrument_id = $("#FormSearch_instrument_id").val();

        if (FormSearch_payment_date !== '') {
            GM.RPCoupon.Table.columns(4).search(FormSearch_payment_date);
        }

        if (FormSearch_instrument_id !== '') {
            GM.RPCoupon.Table.columns(15).search(FormSearch_instrument_id);
        }
        if (FormSearch_instrument_code !== '') {
            GM.RPCoupon.Table.columns(3).search(FormSearch_instrument_code);
        }

        if (FormSearch_counter_party_id !== '') {
            GM.RPCoupon.Table.columns(14).search(FormSearch_counter_party_id);
        }

        if (FormSearch_fund_code !== '') {
            GM.RPCoupon.Table.columns(7).search(FormSearch_fund_code);
        }

        GM.RPCoupon.Table.draw();
    };
    GM.RPCoupon.Initial = function () {
        $("#action-form")[0].reset();
    };
    GM.RPCoupon.DataBinding = function (p) {
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

    GM.RPCoupon.Table = $('#x-table-data').DataTable({
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
            "url": "/RPReleaseMessage/SearchCoupon",
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
                    targets: 0,
                    data: "trans_cno",
                    searchable: false,
                    orderable: false,
                    className: 'dt-body-center',
                    render: function (data, type, row) {
                        console.log(data);
                        return '<input type="checkbox"' + ' id="chk_' + data + '" class="filter-ck" />';
                    }
                },
                { targets: 1, orderable: false, data: "RowNumber", width: 30 },
                { targets: 2, data: "trans_cno" },
                { targets: 3, data: "instrument_code" },
                {
                    targets: 4, data: "payment_date",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                {
                    targets: 5, data: "event_date",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                { targets: 6, data: "counter_party_code" },
                { targets: 7, data: "fund_code" },
                { targets: 8, data: "cur" },
                {
                    targets: 9, data: "ending_par",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 10, data: "coupon_rate",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 11, data: "unit",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 12, data: "face_value",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 13,
                    orderable: false,
                    data: "trans_cno",
                    className: "dt-body-center",
                    width: 60,
                    render: function (data, type, row, meta) {
                        var html = '<button class="btn btn-default btn-round" form-mode="edit" onclick="location.href=\'/RPReleaseMessage/ReleaseCoupon?trans_cno=' + row.trans_cno + '\'" ><i class="feather-icon icon-edit"></i></button>';
                        html += '<button class="btn btn-default btn-round" form-mode="viewmt" onclick="GetReleaseMT(' + "'" + row.trans_cno + "'" + ',' + "'" + row.cur + "'" + ',' + "'" + row.mt_code + "'" + ')" ><i class="feather-icon icon-message-square"></i></button>';
                        return html;
                    }
                },
                { targets: 14, data: "counter_party_id", "visible": false },
                { targets: 15, data: "instrument_id", "visible": false },
                { targets: 16, data: "mt_code", "visible": false }
            ],
        fixedColumns: {
            leftColumns: 1,
            rightColumns: 4
        },
        createdRow: function (row, data, dataIndex) {
            if (data.release_printed == "Y") {
                $('td', row).css('color', '#00B0FF');
            }
        }
    });

    //Function : Checkbox
    $('#x-table-data').on('click', 'tr', function () {

        GM.RPCoupon.Table = $('#x-table-data').DataTable();
        var data = GM.RPCoupon.Table.row(this).data();

        if (typeof data != 'undefined') {
            if ($(this).find('.filter-ck').prop('checked') == true) {  //update the cell data with the checkbox state
                var id = 'chk_' + data.trans_cno;
                var inputyes = $("[id=" + id + "]");
                inputyes.removeAttr('checked');
                inputyes.attr('checked', 'checked');
                inputyes.prop('checked', true);
            } else {
                var id = 'chk_' + data.trans_cno;
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
    //Function : Btn ReleaseMessage
    GM.RPCoupon.ReleaseMessage = function (btn) {
        var mode = $(btn).attr("form-mode");
        GM.Message.Clear();
        ReleaseMessage();
    };
    $("#search-form").on('submit', function (e) {
        e.preventDefault();
        GM.Message.Clear();
        GM.RPCoupon.Search();
    });
    $("#search-form").on('reset', function (e) {
        $('.spinner').css('display', 'block'); // Open Loading
        location.href = window.location.href;
    });

});