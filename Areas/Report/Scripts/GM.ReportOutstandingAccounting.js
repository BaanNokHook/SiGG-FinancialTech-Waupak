
$(window).on('load', function () {
    $("#ddlTypeDate").find(".selected-data").val("Trade Date");
    $("#ddlTypeDate").find(".selected-data").text("Trade Date");
    $("#ddlTypeDate").find(".selected-value").val("T");
});

$("form").on("submit",
    function (e) {
        if (!$("#asofdate_string").val().length) {
            e.preventDefault();
            swal("Warning", "As of Date field is required.", "warning");
        } else {
            return;
        }
    }
);


$(document).ready(function () {

    GM.Utility.GetBusinessDate("asofdate_string");

    //Portfolio Dropdown
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
    //End Portfolio Dropdown

    //Account Code Dropdown
    $("#ddl_accountcode").click(function () {
        var txt_search = $("#txt_accountcode");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    //Account Code Dropdown
    $("#ddl_accountcode_to").click(function () {
        var txt_search = $("#txt_accountcode_to");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_accountcode").keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    $("#txt_accountcode_to").keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Account Code Dropdown

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

});