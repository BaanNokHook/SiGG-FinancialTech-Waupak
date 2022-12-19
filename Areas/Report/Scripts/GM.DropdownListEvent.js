$(document).ready(function() {

    //#region :: Instrument type Dropdown ::
    $("#ddl_instrument_type").click(function() {
        var txt_search = $("#txt_instrument_type");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_instrument_type").keyup(function() {
        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //#endregion End Instrument type Dropdown

    //#region :: Instrument Dropdown ::
    //Instrument Dropdown
    $("#ddl_instrument_code").click(function() {
        var txt_search = $("#txt_instrument_code");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_instrument_code").keyup(function() {
        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Instrument Dropdown
    //#endregion

    //#region :: CounterParty Dropdown ::
    //CounterParty Dropdown
    $("#ddl_counterparty").click(function() {
        var txt_search = $("#txt_counterparty");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_counterparty").keyup(function() {
        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End CounterParty Dropdown
    //#endregion

    //#region :: Currency Dropdown ::
    //Currency Dropdown
    $("#ddl_currency").click(function() {
        var txt_search = $("#txt_currency");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_currency").keyup(function() {
        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Currency Dropdown
    //#endregion

    //#region :: Portfolio Dropdown ::
    //Portfolio Dropdown
    $("#ddl_port").click(function() {
        var txt_search = $("#txt_port");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_port").keyup(function() {
        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Currency Dropdown
    //#endregion

    //#region :: CounterParty Fund Dropdown ::
    //CounterParty Fund Dropdown
    $("#ddl_counterparty_fund").click(function() {
        var text_search = $("#txt_counterparty_fund");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(text_search, data, null);
        text_search.val("");
    });

    $("#txt_counterparty_fund").keyup(function() {
        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End CounterParty Dropdown
    //#endregion

    //#region :: Repodealtype Dropdown ::
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
    //#endregion

    //#region :: Account Group Dropdown ::
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
    //#endregion

    //#region :: Account Code Dropdown ::
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
    //#endregion

    //#region :: Event Status Dropdown ::
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
    //#endregion

    //#region ::  ::

    //#endregion


});