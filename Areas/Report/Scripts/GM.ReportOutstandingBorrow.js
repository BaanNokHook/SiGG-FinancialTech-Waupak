$(window).on('load', function() {
    var action = "/DailyTransactionReport/GetBusinessDate";
    var id = "asofdate_from_string";
    GM.Utility.GetBusinessDate(id, action);
    GM.Utility.GetBusinessDate("asofdate_to_string", action);
});

$(document).ready(function() {

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
});