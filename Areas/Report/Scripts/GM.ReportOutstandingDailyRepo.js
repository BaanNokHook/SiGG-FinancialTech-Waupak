$(window).on('load', function () {
    var action = "/OutstandingReport/GetBusinessDate";
    GM.Utility.GetBusinessDate("asofdate_from_string", action);
    GM.Utility.GetBusinessDate("asofdate_to_string", action);
    $("#currency").val("THB");
    $("#is_full_date").val('N');
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
    //End Currency Dropdown

    //#region Report Type Dropdown
    $("#ddl_reporttype").click(function () {
        var txt_search = $("#txt_reporttype");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_reporttype").keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //#endregion End Report Type Dropdown

    //Currency Dropdown
    $("#ddl_currency").click(function () {
        var txt_search = $("#txt_currency");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });

    $("#txt_currency").keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null, false);
    });
    //End Currency Dropdown

    $("#chcekFullDay").click(function () {
        if ($("#chcekFullDay").val() === 'N') {
            $("#chcekFullDay").val('Y');
            $("#is_full_date").val('Y');
        } else {
            $("#chcekFullDay").val('N');
            $("#is_full_date").val('N');
        }
    });

    $("form").on("submit",
        function (e) {

            if (!$("#asofdate_from_string").val().length && !$("#asofdate_to_string").val().length) {
                e.preventDefault();
                $("#asofdate_string_from_error").text("As Of Date From field is required.");
                $("#asofdate_from_string").addClass("input-validation-error");

                $("#asofdate_to_string_error").text("As Of Date To field is required.");
                $("#asofdate_to_string").addClass("input-validation-error");

            } else if (!$("#asofdate_from_string").val().length || !$("#asofdate_to_string").val().length) {
                e.preventDefault();
                if (!$("#asofdate_from_string").val().length) {
                    $("#asofdate_string_from_error").text("As Of Date From field is required.");
                    $("#asofdate_from_string").addClass("input-validation-error");
                } else {
                    $("#asofdate_string_from_error").text("");
                    $("#asofdate_from_string").removeClass("input-validation-error");
                }
                if (!$("#asofdate_to_string").val().length) {
                    $("#asofdate_to_string_error").text("As Of Date To field is required.");
                    $("#asofdate_to_string").addClass("input-validation-error");
                } else {
                    $("#asofdate_to_string_error").text("");
                    $("#asofdate_to_string").removeClass("input-validation-error");
                }
            } else {
                $("#asofdate_string_from_error").text("");
                $("#asofdate_from_string").removeClass("input-validation-error");

                $("#asofdate_to_string_error").text("");
                $("#asofdate_to_string").removeClass("input-validation-error");
            }

            if ($("#is_full_date").val() === 'Y' && $("#counterparty_code").val() === '') {
                e.preventDefault();
                swal("Warning", "When choosing take every day, please select counter party", "warning");
            } else {
                return;
            }
        });
});