$(document).ready(function () {

    //Function Search ==============================================

    //Binding ddl

    //Binding TransState
    $("#ddl_TransState").click(function () {
        var txt_search = $('#txt_TransState');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_TransState').keyup(function () {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Binding ddl_role
    $("#ddl_TransStatus").click(function () {
        var txt_search = $('#txt_TransStatus');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_TransStatus').keyup(function () {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Binding Div Detail
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    }
    GM.RPDealSummary.Form = {};
    GM.RPDealSummary.Form.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            switch (key) {
                case "repo_deal_type_code": GM.RPDealSummary.Table.columns(3).search($(this).val()); break;
                case "trans_deal_type": GM.RPDealSummary.Table.columns(4).search($(this).val()); break;
                case "trans_type": GM.RPDealSummary.Table.columns(5).search($(this).val()); break;
                case "port": GM.RPDealSummary.Table.columns(6).search($(this).val()); break;
                case "purpose": GM.RPDealSummary.Table.columns(7).search($(this).val()); break;
                case "counter_party_code": GM.RPDealSummary.Table.columns(8).search($(this).val()); break;
                case "maturity_date": GM.RPDealSummary.Table.columns(11).search($(this).val()); break;
                case "cur": GM.RPDealSummary.Table.columns(15).search($(this).val()); break;
                case "trans_status": GM.RPDealSummary.Table.columns(16).search($(this).val()); break;
                case "trans_state": GM.RPDealSummary.Table.columns(17).search($(this).val()); break;
                case "from_trans_no": GM.RPDealSummary.Table.columns(19).search($(this).val()); break;
                case "to_trans_no": GM.RPDealSummary.Table.columns(20).search($(this).val()); break;
                case "from_trade_date": GM.RPDealSummary.Table.columns(21).search($(this).val()); break;
                case "to_trade_date": GM.RPDealSummary.Table.columns(22).search($(this).val()); break;
                case "from_settlement_date": GM.RPDealSummary.Table.columns(23).search($(this).val()); break;
                case "to_settlement_date": GM.RPDealSummary.Table.columns(24).search($(this).val()); break;
                case "from_maturity_date": GM.RPDealSummary.Table.columns(25).search($(this).val()); break;
                case "to_maturity_date": GM.RPDealSummary.Table.columns(26).search($(this).val()); break;
            }
        });

        GM.RPDealSummary.Table.draw();
    };

    GM.RPDealSummary.Form.Initial = function () {
        $("#action-form")[0].reset();
    };
    GM.RPDealSummary.Form.DataBinding = function (p) {
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
        GM.RPDealSummary.Form.Search();
    });
    $("#search-form").on('reset', function (e) {
        GM.Defer(function() {
            $('#ddl_trans_deal_type').find(".selected-data").text("Select...");
            $('#FormSearch_trans_deal_type').val(null);
            $('#FormSearch_trans_deal_type_name').val(null);

            $('#ddl_trans_type').find(".selected-data").text("Select...");
            $('#FormSearch_trans_type').val(null);
            $('#FormSearch_trans_type_name').val(null);

            $('#ddl_TransStatus').find(".selected-data").text("Select...");
            $('#FormSearch_trans_status').val(null);
            $('#FormSearch_status_desc').val(null);

            $('#ddl_TransState').find(".selected-data").text("Select...");
            $('#FormSearch_trans_state').val(null);
            $('#FormSearch_state_desc').val(null);

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

            GM.Message.Clear();
            GM.RPDealSummary.Form.Search();
        },
        100);
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