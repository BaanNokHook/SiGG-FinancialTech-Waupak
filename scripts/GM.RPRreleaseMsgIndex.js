$.ajaxSetup({
    cache: false
});

function ReleaseMessage() {
    var rowData = $('#x-table-data').DataTable().rows($('#x-table-data .filter-ck:checked').closest('tr')).data();
    var transNoList = [];
    for (var i = 0; i < rowData.length; i++) {
        transNoList.push(rowData[i].trans_no);
    }

    if (transNoList.length > 0) {
        window.location.href = "/RPReleaseMessage/Select?id=" + JSON.stringify(transNoList) + "&evnet=" + $('#FormSearch_event_type').val();
    }
    else {
        swal("Warning", "Please Select [Trans No] To RPReleaseMessage", "error");
    }
}

function GetReleaseMT(trans_no, from_page, trans_deal_type_name, payment_method, trans_mt_code, cur) {
    $("#check_releasemt").modal('toggle');

    var trans_deal_type;
    if (trans_deal_type_name == "Borrowing") {
        trans_deal_type = 'BR';
    }
    else if (trans_deal_type_name == "Lending") {
        trans_deal_type = 'LD';
    }

    var data = {
        trans_no: trans_no,
        from_page: from_page,
        event_type: 'TRANS',
        trans_deal_type: trans_deal_type,
        payment_method: payment_method,
        trans_mt_code: trans_mt_code,
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
        }
    });
}

$(document).ready(function () {

    $("#ddl_trans_deal_type").click(function () {
        var txt_search = $('#txt_trans_deal_type');
        var data = { trans_deal_type: '' };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#ddl_event_type").click(function () {
        var txt_search = $('#txt_event_type');
        var data = { event_type: '' };
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });

    $("#ddl_payment_method").click(function () {
        var txt_search = $('#txt_payment_method');
        var data = { payment_method: '' };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#ddl_trans_mt_code").click(function () {

        var txt_search = $('#txt_trans_mt_code');
        txt_search.val("");
        var payment_method = $('#FormSearch_payment_method').val();
        var trans_deal_type = $('#FormSearch_trans_deal_type').val();
        var event_type = $('#FormSearch_event_type').val();

        if (payment_method !== "") {
            //swal("Warning", "MT Code require data from [Trans Deal Type],[Payment Method],[Event Type]", "warning");
            var data = { payment_method: payment_method, trans_deal_type: trans_deal_type, event_type: event_type };
            GM.Utility.DDLAutoComplete(txt_search, data, null);
        }

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

    $("#ddl_cur").click(function () {
        var txt_search = $('#txt_cur');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_cur').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null, false);
    });

    $("#ddl_channel").click(function () {
        var txt_search = $('#txt_channel');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_channel').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null, false);
    });

    $("#NavBar").html($('#NavRPReleaseMessage').val());

    $('#show-more-less').click(function () {
        $(this).parents(".box-content").find('.advance-form').slideToggle(0);
        if ($('#show-more-less i').hasClass('icon-chevron-up')) {
            $(this).html('Show less option <i class="feather-icon icon-chevron-down"></i>');
        } else {
            $(this).html('Show more option <i class="feather-icon icon-chevron-up"></i>');
        }
    });

    var budate = $("#BusinessDate").text();

    var formatmmddyyyydate = budate.split("/");
    formatmmddyyyydate = formatmmddyyyydate[1] + "/" + formatmmddyyyydate[0] + "/" + formatmmddyyyydate[2];
    var business_date = new Date(formatmmddyyyydate);

    if ($('#FormSearch_event_type').val() === 'Maturity') {
        $('#FormSearch_from_maturity_date').data("DateTimePicker").date(business_date);
        $('#FormSearch_to_maturity_date').data("DateTimePicker").date(business_date);
    } else {
        $('#FormSearch_from_settlement_date').data("DateTimePicker").date(business_date);
        $('#FormSearch_to_settlement_date').data("DateTimePicker").date(business_date);
    }

    GM.RPReleaseMessage = {};
    GM.RPReleaseMessage.Table = $('#x-table-data').DataTable({
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
            "url": "/RPReleaseMessage/Search",
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
                        return '<input type="checkbox"' + ' id="chk_' + data + '" class="filter-ck" />';
                    }
                },
                { targets: 1, data: "RowNumber", orderable: false },
                { targets: 2, data: "trans_no" },
                { targets: 3, data: "repo_deal_type" },
                { targets: 4, data: "trans_deal_type_name" },
                { targets: 5, data: "event_type" },
                { targets: 6, data: "port" },
                { targets: 7, data: "counter_party_code" },
                { targets: 8, data: "cur" },
                { targets: 9, data: "payment_method" },
                { targets: 10, data: "trans_mt_code" },
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
                    targets: 12, data: "settlement_date",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                {
                    targets: 13, data: "maturity_date",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                {
                    targets: 14, data: "swift_channel",
                    searchable: false,
                    orderable: false,
                    className: "dt-body-center",
                    visible: true,
                    render: function (data, type, row, meta) {
                        return data;
                    }
                },
                {
                    targets: 15, data: "swift_status",
                    searchable: false,
                    orderable: false,
                    className: "dt-body-center",
                    visible: true,
                    render: function (data, type, row, meta) {
                        return data;
                    }
                },
                { targets: 16, data: "from_trans_no", visible: false },
                { targets: 17, data: "to_trans_no", visible: false },
                { targets: 18, data: "from_trade_date", visible: false },
                { targets: 19, data: "to_trade_date", visible: false },
                { targets: 20, data: "from_settlement_date", visible: false },
                { targets: 21, data: "to_settlement_date", visible: false },
                { targets: 22, data: "from_maturity_date", visible: false },
                { targets: 23, data: "to_maturity_date", visible: false },
                { targets: 24, data: "counter_party_name", visible: false },
                {
                    targets: 25,
                    orderable: false,
                    data: "trans_no",
                    className: "dt-body-center",
                    render: function (data, type, row, meta) {
                        var html = '';
                        html += '<button class="btn btn-default btn-round" form-mode="edit" onclick="location.href=\'/RPReleaseMessage/Select?id=[' + row.trans_no + ']&evnet=' + row.event_type + ' \'" ><i class="feather-icon icon-edit"></i></button>';
                        html += '<button class="btn btn-default btn-round" form-mode="viewmt" onclick="GetReleaseMT(' + "'" + row.trans_no + "'" + ',' + "'" + row.event_type + "'" + ',' + "'" + row.trans_deal_type_name + "'" + ',' + "'" + row.payment_method + "'" + ',' + "'" + row.trans_mt_code + "'" + ',' + "'" + row.cur + "'" + ')" ><i class="feather-icon icon-message-square"></i></button>';
                        return html;
                    }
                }
            ],
        fixedColumns: {
            leftColumns: 1,
            rightColumns: 12
        },
        fnPreDrawCallback: function () {
            $('#x-table-data').DataTable().columns(5).search($('#FormSearch_event_type').val());
            $('#x-table-data').DataTable().columns(20).search($('#FormSearch_from_settlement_date').val());
            $('#x-table-data').DataTable().columns(21).search($('#FormSearch_to_settlement_date').val());
            $('#x-table-data').DataTable().columns(22).search($('#FormSearch_from_maturity_date').val());
            $('#x-table-data').DataTable().columns(23).search($('#FormSearch_to_maturity_date').val());
        },
        createdRow: function (row, data, dataIndex) {
            if (data.release_printed == "Y") {
                $('td', row).css('color', '#00B0FF');
            }
        }
    });

    //Function : Checkbox
    $('#x-table-data').on('click', 'tr', function () {

        GM.RPReleaseMessage.Table = $('#x-table-data').DataTable();
        var data = GM.RPReleaseMessage.Table.row(this).data();

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

    //Function : Btn ReleaseMessage
    GM.RPReleaseMessage.ReleaseMessage = function (btn) {
        var mode = $(btn).attr("form-mode");
        GM.Message.Clear();
        ReleaseMessage();
    };

    //Function : DDl Event Type
    $("#ul_event_type").on("click", function (event) {
        var EventType = $("#ddl_event_type").find(".selected-value").val();
        if (EventType == "Coupon") {
            window.location.href = "/RPReleaseMessage/IndexCoupon";
        }
        else if (EventType === 'Margin') {
            window.location.href = "/RPReleaseMessage/IndexCallMargin";
        }
        else if (EventType === 'Net-Settlement') {
            window.location.href = "/RPReleaseMessage/IndexNetSettlement?search=Net-Settlement";
        } else if (EventType === 'Interest Margin') {
            window.location.href = "/RPReleaseMessage/IndexInterestMargin";
        }
        else {
            if (EventType === 'Maturity') {
                $('#FormSearch_from_maturity_date').data("DateTimePicker").date(business_date);
                $('#FormSearch_to_maturity_date').data("DateTimePicker").date(business_date);
                $('#FormSearch_from_settlement_date').data("DateTimePicker").date(null);
                $('#FormSearch_to_settlement_date').data("DateTimePicker").date(null);
            }
            else if (EventType === 'Settlement') {
                $('#FormSearch_from_maturity_date').data("DateTimePicker").date(null);
                $('#FormSearch_to_maturity_date').data("DateTimePicker").date(null);
                $('#FormSearch_from_settlement_date').data("DateTimePicker").date(business_date);
                $('#FormSearch_to_settlement_date').data("DateTimePicker").date(business_date);
            }

            GM.RPReleaseMessage.Form.Search();
        }
    });

    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    };

    GM.RPReleaseMessage.Form = {};
    GM.RPReleaseMessage.Form.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            switch (key) {
                case "event_type": GM.RPReleaseMessage.Table.columns(5).search($(this).val()); break;
                case "counter_party_code": GM.RPReleaseMessage.Table.columns(7).search($(this).val()); break;
                case "payment_method": GM.RPReleaseMessage.Table.columns(9).search($(this).val()); break;
                case "trans_mt_code": GM.RPReleaseMessage.Table.columns(10).search($(this).val()); break;
                case "from_trans_no": GM.RPReleaseMessage.Table.columns(16).search($(this).val()); break;
                case "to_trans_no": GM.RPReleaseMessage.Table.columns(17).search($(this).val()); break;
                case "from_trade_date": GM.RPReleaseMessage.Table.columns(18).search($(this).val()); break;
                case "to_trade_date": GM.RPReleaseMessage.Table.columns(19).search($(this).val()); break;
                case "from_settlement_date": GM.RPReleaseMessage.Table.columns(20).search($(this).val()); break;
                case "to_settlement_date": GM.RPReleaseMessage.Table.columns(21).search($(this).val()); break;
                case "from_maturity_date": GM.RPReleaseMessage.Table.columns(22).search($(this).val()); break;
                case "to_maturity_date": GM.RPReleaseMessage.Table.columns(23).search($(this).val()); break;
                case "counter_party_name": GM.RPReleaseMessage.Table.columns(24).search($(this).val()); break;
                case "cur": GM.RPReleaseMessage.Table.columns(8).search($(this).val()); break;
                case "swift_channel": GM.RPReleaseMessage.Table.columns(14).search($(this).val()); break;
            }
        });

        GM.RPReleaseMessage.Table.draw();
    };

    GM.RPReleaseMessage.Form.Initial = function () {
        $("#action-form")[0].reset();
    };
    GM.RPReleaseMessage.Form.DataBinding = function (p) {
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

    GM.RPReleaseMessage.Sync = function (btn) {
        $('.spinner').css('display', 'block'); // Open Loading

        $.ajax({
            type: "POST",
            url: "/RPReleaseMessage/SyncTransaction",
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: {
                event_type: $('#FormSearch_event_type').val(),
                counter_party_code: $('#FormSearch_counter_party_code').val(),
                payment_method: "SWIFT",
                trans_mt_code: $('#FormSearch_trans_mt_code').val(),
                from_trans_no: $('#FormSearch_from_trans_no').val(),
                to_trans_no: $('#FormSearch_to_trans_no').val(),
                from_trade_date: $('#FormSearch_from_trade_date').val(),
                to_trade_date: $('#FormSearch_to_trade_date').val(),
                from_settlement_date: $('#FormSearch_from_settlement_date').val(),
                to_settlement_date: $('#FormSearch_to_settlement_date').val(),
                from_maturity_date: $('#FormSearch_from_maturity_date').val(),
                to_maturity_date: $('#FormSearch_to_maturity_date').val(),
                counter_party_name: $('#FormSearch_counter_party_name').val(),
                cur: $('#FormSearch_cur').val(),
                swift_channel: $('#FormSearch_swift_channel').val(),
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
                                GM.RPReleaseMessage.Form.Search();
                            }
                        );
                    }, 100);
                } else
                {
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

                swal("Warning", "Please Select [Trans No] To RPReleaseMessage", "error");

                $('.spinner').css('display', 'none'); // Close Loading
            }
        });
    };

    $("#search-form").on('submit', function (e) {
        e.preventDefault();
        GM.Message.Clear();

        GM.RPReleaseMessage.Form.Search();
    });

    $("#search-form").on('reset', function (e) {

        $('.spinner').css('display', 'block'); // Open Loading
        location.href = window.location.href;
    });
});