$(document).ready(function () {

    GM.Utility.GetBusinessDate("payment_date_string");

    //#region Instrument Dropdown

    //Instrument Dropdown
    $("#ddl_instrument_code").click(function () {
        var txt_search = $("#txt_instrument_code");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_instrument_code").keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Instrument Dropdown

    //#endregion

    //#region CounterParty Dropdown
    //CounterParty Dropdown
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
    //End CounterParty Dropdown

    //#endregion

    //#region Currency Dropdown
    //Currency Dropdown
    $("#ddl_currency").click(function () {
        var txt_search = $("#txt_currency");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_currency").keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Currency Dropdown

    //#endregion

});


formreset = function() {
    GM.Message.Clear();

    $("#ddl_counterparty").find(".selected-data").text("Select...");
    $("#counterparty_code").val(null);
    $("#counterparty_code").text(null);
    $("#counterparty_code_name").val(null);
    $("#counterparty_code_name").text(null);

    $("#ddl_instrument_code").find(".selected-data").text("Select...");
    $("#instrument_id").val(null);
    $("#instrument_id").text(null);
    $("#instrument_code_name").val(null);
    $("#instrument_code_name").text(null);

    $("#ddl_currency").find(".selected-data").text("Select...");
    $("#currency").val(null);
    $("#currency").text(null);

    $("#xi_date_string").val("");
    $("#payment_date_string").val("");

    // set default date  
    GM.Utility.GetBusinessDate("payment_date_string");
};
