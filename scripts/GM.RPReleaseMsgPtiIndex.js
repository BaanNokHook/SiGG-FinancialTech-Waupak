$.ajaxSetup({
    cache: false
});

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
        url: "/RPReleaseMessagePti/GetReleaseMT",
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: data,
        success: function (res) {

            var release_pti = "";

            var body = $("#modal_release_mt").find("tbody");
            var html = "";
            var totalamount = 0;
            var isin_code = "";
            body.html("");
            $.each(res, function (i, resdata) {

                //if (isin_code != resdata.isin_code) {
                //    html = "<tr><td><b> ISIN Code:" + $('<div/>').text(resdata.isin_code).html() + "<b></td></tr>";
                //    body.append(html);
                //}
                //html = "<tr><td>" + $('<div/>').text(resdata.mt_message).html() + "</td></tr>";
                //body.append(html);
                //isin_code = resdata.isin_code;

                release_pti += resdata.mt_message + "\n";
            });

            $("#txtarea_release_pti").val(release_pti);
        },
        error: function (res) {
            // TODO: Show error
            GM.Message.Clear();
        }
    });
}

$(window).on('load', function () {

    //Function Search ==============================================
    var BusinessDate = $('#BusinessDate').text();

    $('#FormSearch_from_settlement_date').val(BusinessDate);
    $('#FormSearch_to_settlement_date').val(BusinessDate);

    GM.RPReleaseMessagePti.Form.Search();
});

$(document).ready(function () {
    //Binding : DDL Trans Deal Type

    $("#ddl_trans_deal_type").click(function () {
        var txt_search = $('#txt_trans_deal_type');
        var data = { trans_deal_type: '' };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    //Binding : DDL Event Type
    $("#ddl_event_type").click(function () {
        var txt_search = $('#txt_event_type');
        var data = { event_type: '' };
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });

    //Binding : DDL Payment Method
    $("#ddl_payment_method").click(function () {
        var txt_search = $('#txt_payment_method');
        var data = { payment_method: '' };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    //Binding : DDL trans_mt_code
    $("#ddl_trans_mt_code").click(function () {

        var txt_search = $('#txt_trans_mt_code');
        txt_search.val("");
        var payment_method = $('#FormSearch_payment_method').val();
        var trans_deal_type = $('#FormSearch_trans_deal_type').val();
        var event_type = "TRANS";
        var data = { payment_method: payment_method, trans_deal_type: trans_deal_type, event_type: event_type };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
    });

    //Binding : DDL Counterparty
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



    //Binding Div Detail
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    };
    GM.RPReleaseMessagePti.Form = {};
    GM.RPReleaseMessagePti.Form.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            switch (key) {

                case "event_type": GM.RPReleaseMessagePti.Table.columns(5).search($(this).val()); break;
                case "counter_party_code": GM.RPReleaseMessagePti.Table.columns(7).search($(this).val()); break;
                case "payment_method": GM.RPReleaseMessagePti.Table.columns(9).search($(this).val()); break;
                case "trans_mt_code": GM.RPReleaseMessagePti.Table.columns(10).search($(this).val()); break;
                case "from_trans_no": GM.RPReleaseMessagePti.Table.columns(14).search($(this).val()); break;
                case "to_trans_no": GM.RPReleaseMessagePti.Table.columns(15).search($(this).val()); break;
                case "from_trade_date": GM.RPReleaseMessagePti.Table.columns(16).search($(this).val()); break;
                case "to_trade_date": GM.RPReleaseMessagePti.Table.columns(17).search($(this).val()); break;
                case "FormSearch_from_settlement_date": GM.RPReleaseMessagePti.Table.columns(18).search($(this).val()); break;
                case "FormSearch_to_settlement_date": GM.RPReleaseMessagePti.Table.columns(19).search($(this).val()); break;
                case "from_maturity_date": GM.RPReleaseMessagePti.Table.columns(20).search($(this).val()); break;
                case "to_maturity_date": GM.RPReleaseMessagePti.Table.columns(21).search($(this).val()); break;
                case "trans_deal_type": GM.RPReleaseMessagePti.Table.columns(22).search($(this).val()); break;
                case "counter_party_name": GM.RPReleaseMessagePti.Table.columns(23).search($(this).val()); break;
                case "trans_deal_type_name": GM.RPReleaseMessagePti.Table.columns(24).search($(this).val()); break;
            }
        });

        GM.RPReleaseMessagePti.Table.draw();
    };

    GM.RPReleaseMessagePti.Form.Initial = function () {
        $("#action-form")[0].reset();
    };
    GM.RPReleaseMessagePti.Form.DataBinding = function (p) {
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
        GM.RPReleaseMessagePti.Form.Search();
    });
    $("#search-form").on('reset', function (e) {
        $('.spinner').css('display', 'block'); // Open Loading
        location.href = window.location.href;

        //GM.Defer(function () {
        //    $('#ddl_trans_deal_type').find(".selected-data").text("Select...");
        //    $('#FormSearch_trans_deal_type').val(null);
        //    $('#FormSearch_trans_deal_type_name').val(null);

        //    $('#ddl_counterparty').find(".selected-data").text("Select...");
        //    $('#FormSearch_counter_party_code').val(null);
        //    $('#FormSearch_counter_party_name').val(null);

        //    $('#ddl_repo_deal_type').find(".selected-data").text("Select...");
        //    $('#FormSearch_repo_deal_type').val(null);
        //    $('#FormSearch_repo_deal_type_code').val(null);

        //    $('#ddl_event_type').find(".selected-data").text("Settlement");
        //    $('#FormSearch_event_type').val(null);
        //    $('#FormSearch_event_type').val(null);

        //    $('#ddl_payment_method').find(".selected-data").text("Select...");
        //    $('#FormSearch_payment_method').val(null);
        //    $('#FormSearch_payment_method').val(null);

        //    $('#ddl_trans_mt_code').find(".selected-data").text("Select...");
        //    $('#FormSearch_trans_mt_code').val(null);
        //    $('#FormSearch_trans_mt_code').val(null);

        //    GM.Message.Clear();
        //    GM.RPReleaseMessagePti.Form.Search();
        //}, 100);
    });
});