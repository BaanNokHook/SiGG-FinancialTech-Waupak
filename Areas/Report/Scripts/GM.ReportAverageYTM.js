
$(document).ready(function() {

    //Instrument type Dropdown
    $("#ddl_instrument_type").click(function() {
        var txt_search = $("#txt_instrument_type");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_instrument_type").keyup(function() {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Instrument type Dropdown

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
});