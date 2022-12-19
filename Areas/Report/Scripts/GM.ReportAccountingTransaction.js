
$(window).on('load', function() {
    GM.Utility.GetBusinessDate("trade_date_from_string");
    GM.Utility.GetBusinessDate("trade_date_to_string");
});

$(document).ready(function() {

    //#region CounterParty Dropdown
    $("#ddl_counterparty").click(function () {
        var txt_search = $("#txt_counterparty");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_counterparty").keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //#endregion

    //#region EventType Dropdown
    $("#ddl_event_type").click(function () {
        var txt_search = $("#txt_event_type");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_event_type").keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //#endregion

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

    //Portfolio Dropdown
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
    //End Portfolio Dropdown

    //Account Group Dropdown
    $("#ddl_accountgroup").click(function() {
        var txt_search = $("#txt_accountgroup");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_accountgroup").keyup(function() {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Account Group Dropdown

    //Account Code Dropdown
    $("#ddl_accountcode").click(function() {
        var txt_search = $("#txt_accountcode");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_accountcode").keyup(function() {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Account Code Dropdown

    //Event Status Dropdown
    $("#ddl_accountevent").click(function() {
        var txt_search = $("#txt_accountevent");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_accountevent").keyup(function() {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Event Status Dropdown

});