$(window).on('load', function() {
    var action = "/DailyTransactionReport/GetBusinessDate";

    GM.Utility.GetBusinessDate("call_date_from_string", action);
    GM.Utility.GetBusinessDate("call_date_to_string", action);
});
$(document).ready(function() {

    //CounterParty Dropdown
    $("#ddl_counterparty").click(function() {
        var txt_search = $("#txt_counterparty");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_counterparty").keyup(function() {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End CounterParty Dropdown

    //Currency Dropdown
    $("#ddl_currency").click(function() {
        var txt_search = $("#txt_currency");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_currency").keyup(function() {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Currency Dropdown

    //Repodealtype Dropdown
    $("#ddl_repodealtype").click(function() {
        var txt_search = $("#txt_repodealtype");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_repodealtype").keyup(function() {
        if (this.value.length > 0) {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        }
    });
    //End Repodealtype Dropdown
});