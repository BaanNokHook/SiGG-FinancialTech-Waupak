$(window).on('load', function() {
    var action = "/DailyTransactionReport/GetBusinessDate";
    var id = "asofdate_from_string";
    GM.Utility.GetBusinessDate(id, action);
    GM.Utility.GetBusinessDate("asofdate_to_string", action);
});

$(document).ready(function() {
    //Repodealtype Dropdown
    $("#ddl_obligatetype").click(function() {
        var txt_search = $("#txt_obligatetype");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_obligatetype").keyup(function() {
        if (this.value.length > 0) {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        }
    });
    //End Repodealtype Dropdown

    //Port Dropdown
    $("#ddl_port").click(function() {
        var txt_search = $("#txt_port");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_port").keyup(function() {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Port Dropdown

    //Instrument Dropdown
    $("#ddl_instrument_code").click(function() {
        var txt_search = $("#txt_instrument_code");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_instrument_code").keyup(function() {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Instrument Dropdown  

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