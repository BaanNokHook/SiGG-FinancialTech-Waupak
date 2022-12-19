$(window).on('load', function() {
    var action = "/DailyTransactionReport/GetBusinessDate";
    GM.Utility.GetBusinessDate("call_date_from_string", action);
    GM.Utility.GetBusinessDate("call_date_to_string", action);
});
$(document).ready(function() {

    //CounterParty Dropdown
    $("#ddl_counterparty").click(function(e) {
        var txt_search = $("#txt_counterparty");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_counterparty").keyup(function() {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    $("#ul_counterparty").on("click", ".searchterm", function (event) {
        $("#ddl_counterparty_fund").find(".selected-data").text("Select...");
        $("#counter_party_fund_id").val(null);
        $("#counter_party_fund_name").text(null);
    });

    //End CounterParty Dropdown

    //CounterpartyFund Dropdown
    $("#ddl_counterparty_fund").click(function () {
        var txt_search = $('#txt_counterparty_fund');
        var counterparty_id = $("#counterparty_code").val();
        if (counterparty_id.length) {
            var data = {
                datastr: counterparty_id
            };
            GM.Utility.DDLAutoComplete(txt_search, data, null);
        } else {
            GM.Utility.DDLNoData(txt_search);
        }
        
        txt_search.val("");
    });
    //End CounterpartyFund Dropdown

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

    $("#ul_repodealtype").on("click",
        ".searchterm",
        function() {
            if ($("#repo_deal_type").val() === "BRP") {
                var data = {
                    datastr: $("#txt_counterparty").val(),
                    type: "BRP"
                };
                $.ajax({
                    url: "/ReportOutstandingCashMargin/FillCounterParty",
                    type: "GET",
                    dataType: "JSON",
                    data: data,
                    success: function(res) {
                        var text = "None";
                        var value = null;
                        if (res.length > 0) {
                            text = res[0].Text;
                            value = res[0].Value;
                        }

                        $("#ddl_counterparty").find(".selected-data").text(text);
                        $("#ddl_counterparty").find(".selected-data").val(value);
                        $("#ddl_counterparty").find(".selected-data").attr("data-toggle", "tooltip");
                        $("#ddl_counterparty").find(".selected-data").attr("title", text);
                        $("#ddl_counterparty").find(".selected-value").val(value);
                    }
                });
            } else {
                $("#ddl_counterparty").find(".selected-data").text("Select...");
                $("#ddl_counterparty").find(".selected-data").val("");
                $("#ddl_counterparty").find(".selected-value").val("");
            }

        });


    //End Repodealtype Dropdown
});