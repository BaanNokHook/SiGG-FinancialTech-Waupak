$(window).on('load', function() {
    var action = "/DailyTransactionReport/GetBusinessDate";
    GM.Utility.GetBusinessDate("from_date_string", action);
    GM.Utility.GetBusinessDate("to_date_string", action);
});

$(document).ready(function() {
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

    $(".radio input[id=type_date]").change(function() {
        var current = $(this).val();
        var radioAsDate = $("[id=type_date][value=AS_DATE]");
        var radioCallDate = $("[id=type_date][value=CALL_DATE]");
        if (current === "AS_DATE") {
            radioAsDate.attr("ischeck", "true");
            radioCallDate.attr("ischeck", "false");
            radioAsDate.attr("checked", "checked");
            radioCallDate.removeAttr("checked");
        } else {
            radioAsDate.attr("ischeck", "false");
            radioCallDate.attr("ischeck", "true");
            radioCallDate.attr("checked", "checked");
            radioAsDate.removeAttr("checked");
        }
    });

    $(".radio input[id=type_report]").change(function() {
        var current = $(this).val();
        var radioSum = $("[id=type_report][value=SUM]");
        var radioDetail = $("[id=type_report][value=DETAIL]");
        if (current === "SUM") {
            radioSum.attr("ischeck", "true");
            radioDetail.attr("ischeck", "false");
            radioSum.attr("checked", "checked");
            radioDetail.removeAttr("checked");
        } else {
            radioSum.attr("ischeck", "false");
            radioDetail.attr("ischeck", "true");
            radioDetail.attr("checked", "checked");
            radioSum.removeAttr("checked");
        }
    });

    $("#from_date_string").on("dp.change",
        function(e) {
            if (e.date) {
                var date = moment(e.date).format("DD/MM/YYYY");
                if ($("#to_date_string").text() != "") {
                    var date_from = setformatdateyyyymmdd(moment(e.date).format("DD/MM/YYYY"));
                    var date_to = setformatdateyyyymmdd($("#to_date_string").text());
                    if (date_from > date_to) {
                        $("#to_date_string").text(date);
                        $("#to_date_string").val(date);
                        $("#from_date_string").text(date);
                        $("#from_date_string").val(date);
                    } else {

                        $("#from_date_string").text(date);
                        $("#from_date_string").val(date);
                    }
                } else {
                    $("#from_date_string").text(date);
                    $("#from_date_string").val(date);
                }
            }
        });

    $("#to_date_string").on("dp.change",
        function(e) {
            if (e.date) {
                var date_from;
                var date = moment(e.date).format("DD/MM/YYYY");

                if ($("#from_date_string").text() == "") {
                    // swal("Warning", "Please select As Of Date From Before", "warning");
                    $("#to_date_string").text(date);
                    $("#to_date_string").val(date);
                    $("#from_date_string").text(date);
                    $("#from_date_string").val(date);
                } else {
                    date_from = setformatdateyyyymmdd($("#from_date_string").text());
                    var date_to = setformatdateyyyymmdd(moment(e.date).format("DD/MM/YYYY"));
                    if (date_from > date_to) {
                        //swal("Warning", "Date To Can not less than Date From", "warning");
                        // $('#asofdate_to_string').text($('#asofdate_from_string').text());
                        // $('#asofdate_to_string').val($('#asofdate_from_string').text());
                        $("#to_date_string").text(date);
                        $("#to_date_string").val(date);
                        $("#from_date_string").text(date);
                        $("#from_date_string").val(date);
                    } else {
                        $("#to_date_string").text(date);
                        $("#to_date_string").val(date);
                    }
                }

            }
        });

    $("form").on("submit",
        function(e) {
            $("#counterparty_error").text("");
            $("#ddl_counterparty").removeClass("input-validation-error");

            if ($('input:radio[name="type_report"]').filter(":checked").val() == "DETAIL") {
                var counterparty_id = $("#counterparty_id");
                if (counterparty_id.val().trim() == "") {
                    e.preventDefault();
                    $("#counterparty_error").text("Counter Party field is required.");
                    $("#ddl_counterparty").addClass("input-validation-error");
                }
            }
        });
});