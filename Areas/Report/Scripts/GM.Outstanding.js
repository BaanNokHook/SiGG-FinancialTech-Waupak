
$(window).on('load', function () {
    var action = "/OutstandingReport/GetBusinessDate";
    GM.Utility.GetBusinessDate("asofdate_string", action);
});

$(document).ready(function () {

    //Instrument type Dropdown
    $("#ddl_instrument_type").click(function () {
        var txt_search = $("#txt_instrument_type");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_instrument_type").keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Instrument type Dropdown


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

    //Port Dropdown
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
    //End Port Dropdown

    //Repodealtype Dropdown
    $("#ddl_repodealtype").click(function () {
        var txt_search = $('#txt_repodealtype');
        var data = {
            datastr: null
        };
        GM.Utility.DDLAutoComplete(txt_search, data, null, true);
        txt_search.val("");
    });

    $("#txt_repodealtype").keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null, true);
    });
    //End Repodealtype Dropdown

    $("#ddl_instrument").click(function () {
        var txt_search = $('#txt_instrument');
        var data = {
            datastr: null
        };
        GM.Utility.DDLAutoComplete(txt_search, data, null, true);
        txt_search.val("");
    });

    $('#txt_instrument').keyup(function () {
        var data = {
            datastr: this.value
        };
        GM.Utility.DDLAutoComplete(this, data, null, true);
    });

    $("form").on("submit",
        function (e) {
            if (!$("#asofdate_string").val().length) {
                e.preventDefault();
                $("#asofdate_string_error").text("As Of Date field is required.");
                $("#asofdate_string").addClass("input-validation-error");
            } else {
                $("#asofdate_string_error").text("");
                $("#asofdate_string").removeClass("input-validation-error");
            }
            return;
        });
});