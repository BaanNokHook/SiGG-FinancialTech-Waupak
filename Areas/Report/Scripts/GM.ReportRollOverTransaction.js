$(document).ready(function () {

    GM.Utility.GetBusinessDate("trade_date_from_string");
    GM.Utility.GetBusinessDate("trade_date_to_string");


    //#region Instrument Dropdown
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
    //#endregion

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

    //#region CounterParty Fund Dropdown
    $("#ddl_counterparty_fund").click(function () {
        var text_search = $("#txt_counterparty_fund");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(text_search, data, null);
        text_search.val("");
    });

    $("#txt_counterparty_fund").keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //#endregion

    //#region Currency Dropdown
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
    //#endregion

    //#region Port Dropdown
    $("#ddl_port").click(function () {
        var txt_search = $("#txt_port");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_port").keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //#endregion

    //#region Repo Deal Type  Dropdown
    $("#ddl_repodealtype").click(function () {
        var txt_search = $("#txt_repodealtype");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_repodealtype").keyup(function () {
        if (this.value.length > 0) {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        }
    });
    //#endregion

});

$("form").on("submit",
    function (e) {
        var nameElement = '';
        if (!$("#trade_date_from_string").val().length && !$("#trade_date_to_string").val().length) {
            nameElement = 'Trade Date';
        } else {
            return;
        }

        if (!$("#settlement_date_from_string").val().length && !$("#settlement_date_to_string").val().length) {
            nameElement = 'Settlement Date';
        } else {
            return;
        }

        if (!$("#maturity_date_from_string").val().length && !$("#maturity_date_to_string").val().length) {
            nameElement = 'Maturity Date';
        } else {
            return;
        }

        if (nameElement !== '') {
            e.preventDefault();
            swal("Warning","Date field is required.", "warning");
        } else {
            return;
        }
    });

formreset = function () {
    GM.Message.Clear();

    $("#ddl_instrument_code").find(".selected-data").text("Select...");
    $("#instrument_id").val(null);
    $("#instrument_id").text(null);
    $("#instrument_code_name").val(null);
    $("#instrument_code_name").text(null);

    $("#ddl_counterparty").find(".selected-data").text("Select...");
    $("#counterparty_code").val(null);
    $("#counterparty_code").text(null);
    $("#counterparty_code_name").val(null);
    $("#counterparty_code_name").text(null);


    $("#ddl_currency").find(".selected-data").text("Select...");
    $("#currency").val(null);
    $("#currency").text(null);

    $("#ddl_port").find(".selected-data").text("Select...");
    $("#port").val(null);
    $("#port").text(null);
    $("#port_name").val(null);
    $("#port_name").text(null);

    $("#ddl_repodealtype").find(".selected-data").text("Select...");
    $("#repo_deal_type").val(null);
    $("#repo_deal_type").text(null);
    $("#repo_deal_type_name").val(null);
    $("#repo_deal_type_name").text(null);


    $("#settlement_date_from_string").val("");
    $("#settlement_date_to_string").val("");

    $("#maturity_date_from").val("");
    $("#maturity_date_to_string").val("");

    $("#trans_no_from").val("");
    $("#trans_no_to").val("");

    // set default date  
    GM.Utility.GetBusinessDate("trade_date_from_string");
    GM.Utility.GetBusinessDate("trade_date_to_string");
};