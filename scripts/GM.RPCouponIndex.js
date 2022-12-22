
function FormatDecimal(Num, point) {
    var format = Number(parseFloat(Num).toFixed(point)).toLocaleString('en', {
        minimumFractionDigits: point
    });
    return format;
}

$(window).on('load', function () {

    var budate = $("#BusinessDate").text();

    var formatmmddyyyydate = budate.split("/");
    //var formatyyyyMMdd = formatmmddyyyydate[2] + "-" + formatmmddyyyydate[1] + "-" + formatmmddyyyydate[0];
    formatmmddyyyydate = formatmmddyyyydate[1] + "/" + formatmmddyyyydate[0] + "/" + formatmmddyyyydate[2];
    var payment_date = new Date(formatmmddyyyydate);

    //$('#FormSearch_payment_date').data("DateTimePicker").date(payment_date);

    $('#FormSearch_payment_date').val(budate);

    //setTimeout(
    //    function () {
    //        GM.RPCoupon.Search();
    //    },
    //    500);
});

$(document).ready(function () {

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

    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    };

    GM.RPCoupon = {};
    GM.RPCoupon.Table = $('#x-table-data').DataTable({
        dom: 'Bfrtip',
        select: false,
        searching: true,
        scrollY: '80vh',
        scrollX: true,
        order: [
            [1, "desc"]
        ],
        buttons:
            [

            ],
        processing: true,
        serverSide: true,
        ajax: {
            "url": "/RPCoupon/Search",
            "type": "POST",
            "statusCode": {
                401: function () {
                    window.location.href = "/RPCoupon";
                }
            },
            "error":
                function (error) {
                    console.log(error);
                    //window.location.href = "/RPCoupon";
                }
        },
        columnDefs:
            [
                { targets: 0, orderable: false, data: "RowNumber", width: 30 },
                { targets: 1, data: "instrument_code" },
                {
                    targets: 2, data: "payment_date",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                {
                    targets: 3, data: "event_date",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                { targets: 4, data: "counter_party_code" },
                { targets: 5, data: "fund_code" },
                { targets: 6, data: "cur" },
                {
                    targets: 7, data: "ending_par",
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
                    targets: 8, data: "coupon_rate",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            //html = FormatDecimal(data, 2);
                            html = data;
                        }
                        return html;
                    }
                },
                {
                    targets: 9, data: "unit",
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
                    targets: 10, data: "face_value",
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
                    targets: 11,
                    data: "instrument_code",
                    className: "dt-body-center",
                    width: 60,
                    render: function (data, type, row, meta) {
                        var html = '';
                        console.log(" moment(row.payment_date) : " + moment(row.payment_date));
                        console.log(" moment(row.payment_date).format(DD / MM / YYYY) : " + moment(row.payment_date).format("DD/MM/YYYY"));
                        //html += '<td></td>';
                        html += '<td><button class="btn btn-default btn-round" style="text-align:center;" form-mode="edit" onclick="location.href=\'/RPCoupon/Edit?instrument_code='
                            + row.instrument_code + '&instrument_id=' + row.instrument_id + '&counter_party_code=' + row.counter_party_code + '&counter_party_id=' + row.counter_party_id + '&fund_code=' + row.fund_code
                            + '&cur=' + row.cur + '&trans_deal_type=' + row.trans_deal_type + '&payment_date=' + moment(row.payment_date).format("DD/MM/YYYY")
                            + '\'" ><i class="feather-icon icon-edit"></i></button></td>';
                        html += '<td></td>';
                        return html;
                    }
                },
                { targets: 12, data: "counter_party_id", "visible": false },
                { targets: 13, data: "instrument_id", "visible": false }

            ],
        fixedColumns: {
            leftColumns: 1,
            rightColumns: 3
        }
    });

    $("#action-form").on('submit', function (e) {

        GM.Message.Clear();
        e.preventDefault(); // prevent the form's normal submission

        var dataToPost = $(this).serialize();
        var action = $(this).attr('action');

        GM.Mask('#action-form-modal .modal-content');
        $.post(action, dataToPost)

            .done(function (response, status, jqxhr) {
                GM.Unmask();
                if (response.Success) {
                    GM.Message.Success('.modal-body', response.Message);
                    GM.Defer(function () {
                        $('#action-form-modal').modal('hide');
                        GM.RPCoupon.Table.draw();
                    }, 500);
                }
                else {
                    GM.Message.Error('.modal-body', response.Message);
                }
            })
            .fail(function (jqxhr, status, error) {
                console.log(error);
            });
    });

    GM.RPCoupon.Table.SelectAll = function (check) {

        if ($(check)[0].checked) {
            GM.RPCoupon.Table.rows().select();
        }
        else {
            GM.RPCoupon.Table.rows().deselect();
        }
    };

    GM.RPCoupon.Search = function () {
        var FormSearch_payment_date = $("#FormSearch_payment_date").val();
        var FormSearch_instrument_code = $("#FormSearch_instrument_code").val();
        var FormSearch_counter_party_id = $("#FormSearch_counter_party_id").val();
        var FormSearch_fund_code = $("#FormSearch_fund_code").val();
        var FormSearch_instrument_id = $("#FormSearch_instrument_id").val();

        console.log("FormSearch_payment_date : " + FormSearch_payment_date);
        console.log("FormSearch_instrument_code : " + FormSearch_instrument_code);
        console.log("FormSearch_counter_party_id : " + FormSearch_counter_party_id);
        console.log("FormSearch_fund_code : " + FormSearch_fund_code);
        console.log("FormSearch_instrument_id : " + FormSearch_instrument_id);

        if (FormSearch_instrument_code !== '') {
            GM.RPCoupon.Table.columns(1).search(FormSearch_instrument_code);
        }

        if (FormSearch_payment_date !== '') {
            GM.RPCoupon.Table.columns(2).search(FormSearch_payment_date);
        }

        if (FormSearch_counter_party_id !== '') {
            GM.RPCoupon.Table.columns(12).search(FormSearch_counter_party_id);
        }

        if (FormSearch_instrument_id !== '') {
            GM.RPCoupon.Table.columns(13).search(FormSearch_instrument_id);
        }

        if (FormSearch_fund_code !== '') {
            GM.RPCoupon.Table.columns(5).search(FormSearch_fund_code);
        }

        GM.RPCoupon.Table.draw();
    };

    GM.RPCoupon.Form = {};
    GM.RPCoupon.Form.Initial = function () {
        $("#action-form")[0].reset();
    };

    GM.RPCoupon.Form.DataBinding = function (p) {
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
        GM.RPCoupon.Search();
    });

    $("#search-form").on('reset', function (e) {
        $('.spinner').css('display', 'block'); // Open Loading
        location.href = window.location.href;
    });

});