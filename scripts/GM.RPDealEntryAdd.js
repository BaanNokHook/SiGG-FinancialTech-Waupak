
$.ajaxSetup({
    cache: false
});

(function ($) {
    $.fn.currencyFormat = function () {
        this.each(function (i) {
            $(this).change(function (e) {
                if (isNaN(parseFloat(this.value.replace(/,/g, '')))) return;
                this.value = parseFloat(this.value.replace(/,/g, '')).toFixed(17);
            });
        });
        return this;
    };
})(jQuery);

function isNumeric(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

$(function () {
    var interestamount = $('#interest_amount').val() === "" ? 0.00 : parseFloat($('#interest_amount').val().replace(/,/g, ''));
    var whtamount = $('#withholding_amount').val() === "" ? 0.00 : parseFloat($('#withholding_amount').val().replace(/,/g, ''));
    var repurchaseprice = $('#repurchase_price').val() === "" ? 0.00 : parseFloat($('#repurchase_price').val().replace(/,/g, ''));
    var purchase_price = $('#purchase_price').val() === "" ? 0.00 : parseFloat($('#purchase_price').val().replace(/,/g, ''));
    var exch_rate = $('#exch_rate').val() === "" ? 1.00000000 : parseFloat($('#exch_rate').val().replace(/,/g, ''));
    var wht_tax = $('#wht_tax').val() === "" ? 0.00000000 : parseFloat($('#wht_tax').val().replace(/,/g, ''));
    var absorb = $("#lbl_wht_tax").find(".selected-text").text();
    if (repurchaseprice !== "") {
        $("#lbl_repurchase_price").text(FormatDecimal(repurchaseprice, 2));
        $("#repurchase_price").val(FormatDecimal(repurchaseprice, 2)).text(FormatDecimal(repurchaseprice, 2));
    }

    if (whtamount !== "") {
        $("#withholding_amount").val(FormatDecimal(whtamount, 2));
    }

    if (whtamount !== "" && absorb.indexOf('Absorb') !== -1) {
        $("#withholding_amount_text").text(FormatDecimal(whtamount, 2));
        tmp_abosrb = false;
    } else {
        tmp_abosrb = true;
        $("#withholding_amount_text").text("");
    }

    if (interestamount !== "") {
        $("#lbl_interest_amount").text(FormatDecimal(interestamount, 2));
        $("#interest_amount").val(FormatDecimal(interestamount, 2)).text(FormatDecimal(interestamount, 2));
    }

    if (exch_rate !== "") {
        $("#exch_rate").val(FormatDecimal(exch_rate, 8));
    }

    if (purchase_price != "") {
        $("#purchase_price").val(FormatDecimal(purchase_price, 2)).text(FormatDecimal(purchase_price, 2));
    }

    if (wht_tax !== "") {
        //$("#lbl_wht_tax").find(".selected-data").text(FormatDecimal(wht_tax));
        //$("#lbl_wht_tax").find(".selected-data").val(FormatDecimal(wht_tax));
        //$("#wht_tax").val(FormatDecimal(wht_tax)).text(FormatDecimal(wht_tax));
    }

    if ($('#cost_of_fund').val() !== "") {
        tmp_cost_of_fund = FormatDecimal($('#cost_of_fund').val(), 8);
    }

    if ($('#interest_rate').val() !== "") {
        tmp_interest_rate = FormatDecimal($('#interest_rate').val(), 8);
    }

    if ($('#interest_floating_index_code').val() !== '') {
        $("#interest_spread").removeAttr("readonly");
        $("#interest_spread").parent().find("span").removeAttr("readonly");
    }

    if ($('#cost_floating_index_code').val() !== '') {
        $("#cost_spread").removeAttr("readonly");
        $("#cost_spread").parent().find("span").removeAttr("readonly");
    }

});

var tmp_cost_of_fund = null;
var tmp_interest_rate = null;
var tmp_abosrb = false;
var tmpCostOfFund;
var ispostback = false;
var oldExchRate = 1;
var old_settlement_date = "";
var old_maturity_date = "";

$(window).on('load', function () {
    var trans_no = $("#trans_no").val();
    window.flagUpdate = true;
    if (trans_no.trim() === "") {

        setTimeout(function () {
            $('.spinner').css('display', 'block'); // Open Loading
        }, 50);

        $("#ddl_instrument").find(".selected-data").val("Lending");
        $("#ddl_instrument").find(".selected-data").text("Lending");
        $("#ddl_instrument").find(".selected-value").val("LD");
        $("#trans_deal_type").val("LD");

        if ($("#port").val() === 'TRADING') {
            var data = {
                userid: $('#userid').text(),
                port: $('#port').val(),
                instrument: checkinstrument()
            };
            var isNotFind = true;

            ClearCounterparty();

            $("#bilateral_contract_no").attr("readonly", "true");
            $("#bilateral_contract_no").val("");
            $("#bilateral_contract_no").text("");

            //FillDesk
            $.ajax({
                url: "/RPDealEntry/FillDesk",
                type: "GET",
                dataType: "JSON",
                data: data,
                success: function (res) {

                    for (var i = 0, len = res.length; i < len; i++) {
                        if (res[i].Text == "PRIV_REPO") {
                            $("#ddl_desk").find(".selected-data").val(res[i].Text);
                            $("#ddl_desk").find(".selected-data").text(res[i].Text);
                            $("#ddl_desk").find(".selected-value").val(res[i].Value);
                            isNotFind = false;
                            break;
                        }
                    }

                    if (isNotFind) {
                        $("#ddl_desk").find(".selected-data").val(res[0].Text);
                        $("#ddl_desk").find(".selected-data").text(res[0].Text);
                        $("#ddl_desk").find(".selected-value").val(res[0].Value);
                    }
                }
            });

            data = {
                datastr: "FINP"
            };

            //FillPurpose
            $.ajax({
                url: "/RPDealEntry/FillPurpose",
                type: "GET",
                dataType: "JSON",
                data: data,
                success: function (res) {
                    $("#ddl_purpose").find(".selected-data").val(res[0].Text);
                    $("#ddl_purpose").find(".selected-data").text(res[0].Text);
                    $("#ddl_purpose").find(".selected-data").attr("data-toggle", "tooltip");
                    $("#ddl_purpose").find(".selected-data").attr("title", res[0].Text);
                    $("#ddl_purpose").find(".selected-value").val(res[0].Value);
                }
            });

            setTimeout(function () {
                $("#withholding_amount").val("0.00");
                $("#withholding_amount_text").text("0.00");
            }, 500);
        }
        else {
            GetCounterParty();
        }

        var budate = $("#BusinessDate").text();

        var formatmmddyyyydate = budate.split("/");
        formatmmddyyyydate = formatmmddyyyydate[1] + "/" + formatmmddyyyydate[0] + "/" + formatmmddyyyydate[2];
        var tradedate = new Date(formatmmddyyyydate);
        var settledate = new Date(formatmmddyyyydate);
        var matdate = new Date(formatmmddyyyydate);
        matdate.setDate(matdate.getDate() + 1);

        var checksetdate = $('#trade_date').val();
        if (checksetdate === null || checksetdate === "") {
            $('#trade_date').data("DateTimePicker").date(tradedate);
            $('#settlement_date').data("DateTimePicker").date(settledate);
            $('#maturity_date').data("DateTimePicker").date(matdate);
        }

        var intrate = $("#interest_rate").val();
        var intspread = $("#interest_spread").val();
        var costrate = $("#cost_of_fund").val();
        var costspread = $("#cost_spread").val();

        var exch_rate = $('#exch_rate').val();

        if (parseInt(intrate.replace(/,/g, '')) === 0 && parseInt(intspread.replace(/,/g, '')) === 0) {
            clearinterest();
        }

        if (parseInt(costrate.replace(/,/g, '')) === 0 && parseInt(costspread.replace(/,/g, '')) === 0) {
            clearcost();
        }

        if (exch_rate !== "") {
            $("#exch_rate").val(FormatDecimal(exch_rate, 8));
            $("#exch_rate").text(FormatDecimal(exch_rate, 8));
        }

        $("#ddl_rp_source").find(".selected-data").text("TBMA");
        $('#rp_source_text').val("TBMA");
        $('#rp_source_value').val("TBMA");

        window.checkrpsource = $("#rp_source_value").val();

        CheckHolidayDate();
        GetRPpricedate();

        if ($("#cost_of_fund") !== null) {
            tmpCostOfFund = $("#cost_of_fund").val();
        } else {
            tmpCostOfFund = 0.00000000;
        }

        setTimeout(
            function () {
                $('.spinner').css('display', 'none'); // Close Loading
            }, 1000);
    } else {
        GetAbsorb($('#counter_party_id').val(), $("#lbl_wht_tax").find(".selected-data").val());
    }
});

function adjusttable() {
    setTimeout(
        function () {
            $.fn.dataTable.tables({
                visible: true,
                api: true
            }).columns.adjust();
        },
        200);
}

function roundTo(num, digit) {
    return +(Math.round(parseFloat(num).toFixed(digit + 1) + ("e+" + digit)) + ("e-" + digit));
}

function FormatDecimal(num, digit) {
    var format = Number(parseFloat(roundTo(num, digit)).toFixed(digit)).toLocaleString('en', {
        minimumFractionDigits: digit
    });
    return format;
}

function FormatDecimal16(num) {
    var format = Number(parseFloat(roundTo(num, 6)).toFixed(6)).toLocaleString('en', {
        minimumFractionDigits: 8
    });
    return format;
}

function ClearCounterparty() {
    $("#ddl_counterparty").find(".selected-data").text("Select...");
    $("#counter_party_id").val(null);
    $("#counter_party_name").text(null);

    $("#lbl_swift_code").find("p").text("-");
    $("#swift_code").val("-");
    $("#swift_code").text("-");

    $("#ddl_counterparty_fund").find(".selected-data").text("Select...");
    $("#counter_party_fund_id").val(null);
    $("#counter_party_fund_name").text(null);

    $("#ddl_formula").find(".selected-data").text("Select...");
    $("#formula").val(null);
    $("#formula_name").text(null);

    $("#lbl_wht_tax").find("p").text("0.00000000 %");
    $("#wht_tax").val(null);
    $("#wht_tax").text(null);

    $("#ddl_counterparty_payment").find(".selected-data").text("Select...");
    $("#payment_method").val(null);
    $("#payment_method").text(null);

    $("#ddl_trans_mt_code").find(".selected-data").text("Select...");
    $("#trans_mt_code").val(null);
    $("#trans_mt_code").text(null);

    $("#lbl_threshold").find("p").text("-");
    $("#threshold").val(null);
    $("#threshold").text(null);

}

function ClearCounterpartyDetail() {
    $("#lbl_swift_code").find("p").text("-");
    $("#swift_code").val("-");
    $("#swift_code").text("-");

    $("#ddl_counterparty_fund").find(".selected-data").text("Select...");
    $("#counter_party_fund_id").val(null);
    $("#counter_party_fund_name").text(null);

    $("#ddl_formula").find(".selected-data").text("Select...");
    $("#formula").val(null);
    $("#formula_name").text(null);

    $("#lbl_wht_tax").find("p").text("0.00000000 %");
    $("#wht_tax").val(null);
    $("#wht_tax").text(null);

    $("#ddl_counterparty_payment").find(".selected-data").text("Select...");
    $("#payment_method").val(null);
    $("#payment_method").text(null);

    $("#ddl_trans_mt_code").find(".selected-data").text("Select...");
    $("#trans_mt_code").val(null);
    $("#trans_mt_code").text(null);

    $("#lbl_threshold").find("p").text("-");
    $("#threshold").val(null);
    $("#threshold").text(null);
}

function clearinterest() {
    $("#interest_rate").val("");
    $("#interest_rate").text("");
    $("#interest_rate").removeClass("input-validation-error");

    $("#ddl_intrate").find(".selected-data").text("Select...");
    $("#ddl_intrate").find(".selected-data").val("");
    $("#ddl_intrate").find(".selected-value").val("");

    $("#interest_spread").val("");
    $("#interest_spread").text("");
    $("#interest_spread").removeClass("input-validation-error");

    $("#interest_total").val("");
    $("#interest_total").text("");

    $("#interest_rate_error").text("");
    $("#interest_floating_index_code_error").text("");
    $("#interest_spread_error").text("");
}

function clearcost() {
    $("#cost_of_fund").val("");
    $("#cost_of_fund").text("");
    $("#cost_of_fund").removeClass("input-validation-error");

    $("#ddl_costoffund").find(".selected-data").text("Select...");
    $("#ddl_costoffund").find(".selected-data").val("");
    $("#ddl_costoffund").find(".selected-value").val("");

    $("#cost_spread").val("");
    $("#cost_spread").text("");
    $("#cost_spread").removeClass("input-validation-error");

    $("#cost_total").val("");
    $("#cost_total").text("");

    $("#cost_of_fund_error").text("");
    $("#cost_floating_index_code_error").text("");
    $("#cost_spread_error").text("");
}


function SumInterestRate() {
    if (getinteresttype() === 'FIXED' && $("#interest_rate").val() === "") {
        return;
    }
    var valrate;
    var valspread = ($("#interest_spread").val() == "" || $("#interest_spread").val() == "-") ? parseFloat("0.00000000") : $("#interest_spread").val().replace(/,/g, '');
    var totalint;
    if (getinteresttype() === 'FLOAT') {
        valrate = $("#ddl_intrate").find(".selected-value").val() == "" ? parseFloat("0.00000000") : $("#ddl_intrate").find(".selected-value").val().replace(/,/g, '');
    } else {
        valrate = $("#interest_rate").val() == "" ? parseFloat("0.00000000") : $("#interest_rate").val().replace(/,/g, '');
    }

    totalint = parseFloat(valrate) + parseFloat(valspread);

    $("#interest_rate").val(FormatDecimal(valrate, 8));
    $("#interest_total").val(FormatDecimal(totalint, 8));

    CallPrice();

    setTimeout(function () {
        $('#x-table-data tbody tr').each(function (i, row) {
            var checkrow = $('#x-table-data tbody tr').children().eq(4).length;
            if (checkrow > 0) {
                CalBond('ColateralList[' + i + '].CASH_AMOUNT', false);
            }
        });
    }, 100);
}

function SumCostOfFund() {
    if (getcosttype() === 'FIXED' && $("#cost_of_fund").val() === "") {
        return;
    }

    var valrate;
    var valspread = ($("#cost_spread").val() == "" || $("#cost_spread").val() == "-") ? parseFloat("0.00000000") : $("#cost_spread").val().replace(/,/g, '');
    if (getcosttype() == 'FLOAT') {
        valrate = $("#ddl_costoffund").find(".selected-value").val() == "" ? parseFloat("0.00000000") : $("#ddl_costoffund").find(".selected-value").val().replace(/,/g, '');
    } else {
        valrate = ($("#cost_of_fund").val() == "" || $("#cost_of_fund").val() == "-") ? parseFloat("0.00000000") : $("#cost_of_fund").val().replace(/,/g, '');
    }

    var totalcost = parseFloat(valrate) + parseFloat(valspread);

    $("#cost_of_fund").val(FormatDecimal(valrate, 8));
    $("#cost_total").val(FormatDecimal(totalcost, 8));
}

function ValidateInput() {

    var IsValid = true;

    var desk_book_id = $('#desk_book_id');
    $('#ddl_desk').removeClass("input-validation-error");
    if (desk_book_id.val() == '') {
        $("#desk_book_id_error").text("The Desk Book field is required.");
        $('#ddl_desk').addClass("input-validation-error");
        IsValid = false;
    } else {
        $("#desk_book_id_error").text("");
    }

    var trader_id = $('#trader_id');
    $('#ddl_dealer').removeClass("input-validation-error");
    if (trader_id.val() == '') {
        $("#trader_id_error").text("The Trader Id field is required.");
        $('#ddl_dealer').addClass("input-validation-error");
        if (IsValid) {
            Focus(trader_id);
        }
        IsValid = false;
    } else {
        $("#trader_id_error").text("");
    }

    var purchase_price = $('#purchase_price');
    purchase_price.removeClass("input-validation-error");
    if (purchase_price.val() == '') {
        $("#purchase_price_error").text("The Purchase Price field is required.");
        if (IsValid) {
            Focus(purchase_price);
        }
        IsValid = false;
        purchase_price.addClass("input-validation-error");
    } else if (parseFloat(purchase_price.val()) <= 0) {
        $("#purchase_price_error").text("Purchase Price more than 0");
        if (IsValid) {
            Focus(purchase_price);
        }
        IsValid = false;
        purchase_price.addClass("input-validation-error");
    } else {
        $("#purchase_price_error").text("");
    }

    if (checkinstrument() !== 'PRP') {
        var bilateral_contract_no = $('#bilateral_contract_no');
        bilateral_contract_no.removeClass("input-validation-error");
        if (bilateral_contract_no.val() == '') {
            $("#bilateral_contract_no_error").text("The Bilat. Contract field is required.");
            if (IsValid) {
                Focus(bilateral_contract_no);
            }
            IsValid = false;
            bilateral_contract_no.addClass("input-validation-error");
        } else {
            $("#bilateral_contract_no_error").text("");
        }
    } else {
        $("#bilateral_contract_no_error").text("");
    }

    var trade_date = $('#trade_date');
    $('#trade_date').removeClass("input-validation-error");
    if (trade_date.val() == '') {
        $("#trade_date_error").text("The Trade Date field is required.");
        $('#trade_date').addClass("input-validation-error");
        IsValid = false;
    } else {
        $("#trade_date_error").text("");
    }

    var settlement_date = $('#settlement_date');
    $('#settlement_date').removeClass("input-validation-error");
    if (settlement_date.val() == '') {
        $("#settlement_date_error").text("The Settlement Date field is required.");
        $('#settlement_date').addClass("input-validation-error");
        IsValid = false;
    } else {
        $("#settlement_date_error").text("");
    }

    var purpose = $('#purpose');
    $('#ddl_purpose').removeClass("input-validation-error");
    if (purpose.val() === '') {
        $("#purpose_error").text("The Purpose field is required.");
        $('#ddl_purpose').addClass("input-validation-error");
        if (IsValid) {
            Focus(purpose);
        }
        IsValid = false;
    } else {
        $("#purpose_error").text("");
    }

    var basis_code = $('#basis_code');
    $('#ddl_basiscode').removeClass("input-validation-error");
    if (basis_code.val() === '') {
        $("#basis_code_error").text("The Basis Code field is required.");
        $('#ddl_basiscode').addClass("input-validation-error");
        if (IsValid) {
            Focus(basis_code);
        }
        IsValid = false;
    } else {
        $("#basis_code_error").text("");
    }

    var counter_party_id = $('#counter_party_id');
    $('#ddl_counterparty').removeClass("input-validation-error");
    if (counter_party_id.val() === '') {
        $("#counter_party_id_error").text("The Counter Party field is required.");
        $('#ddl_counterparty').addClass("input-validation-error");
        if (IsValid) {
            Focus(counter_party_id);
        }
        IsValid = false;
    } else {
        $("#counter_party_id_error").text("");
    }

    $("#interest_rate_error").text("");
    $("#interest_floating_index_code_error").text("");
    $("#interest_spread_error").text("");

    if (getinteresttype() === 'FIXED') {
        var interest_rate = $('#interest_rate');
        interest_rate.removeClass("input-validation-error");
        if (interest_rate.val() === "") {
            $("#interest_rate_error").text("The Repo Int. Rate field is required.");
            interest_rate.addClass("input-validation-error");
            if (IsValid) {
                Focus(interest_rate);
            }
            IsValid = false;
        }
    } else {
        var interest_floating_index_code = $('#interest_floating_index_code');
        $('#ddl_intrate').removeClass("input-validation-error");
        if (interest_floating_index_code.val() === "") {
            $("#interest_floating_index_code_error").text("The Repo Int. Rate field is required.");
            $('#ddl_intrate').addClass("input-validation-error");
            if (IsValid) {
                Focus(interest_floating_index_code);
            }
            IsValid = false;
        }

        var interest_spread = $('#interest_spread');
        interest_spread.removeClass("input-validation-error");
        if (interest_spread.val() == "") {
            $("#interest_spread_error").text("The Int. Spread field is required.");
            interest_spread.addClass("input-validation-error");
            if (IsValid) {
                Focus(interest_spread);
            }
            IsValid = false;
        }
    }

    $("#cost_of_fund_error").text("");
    $("#cost_floating_index_code_error").text("");
    $("#cost_spread_error").text("");

    if (getcosttype() == 'FIXED') {
        var cost_of_fund = $('#cost_of_fund');
        cost_of_fund.removeClass("input-validation-error");
        if (cost_of_fund.val() == "") {
            $("#cost_of_fund_error").text("The Cost of Fund field is required.");
            cost_of_fund.addClass("input-validation-error");
            if (IsValid) {
                Focus(cost_of_fund);
            }
            IsValid = false;
        }
    } else {
        var cost_floating_index_code = $('#cost_floating_index_code');
        $('#ddl_costoffund').removeClass("input-validation-error");
        if (cost_floating_index_code.val() == "") {
            $("#cost_floating_index_code_error").text("The Cost of Fund field is required.");
            $('#ddl_costoffund').addClass("input-validation-error");
            if (IsValid) {
                Focus(cost_floating_index_code);
            }
            IsValid = false;
        }

        var cost_spread = $('#cost_spread');
        cost_spread.removeClass("input-validation-error");
        if (cost_spread.val() == "") {
            $("#cost_spread_error").text("The Cost Spread field is required.");
            cost_spread.addClass("input-validation-error");
            if (IsValid) {
                Focus(cost_spread);
            }
            IsValid = false;
        }
    }

    var ref_trans_no = $('#ref_trans_no');
    ref_trans_no.removeClass("input-validation-error");
    if ($("#net_settement_flag").val() == 'true' && $("#ref_trans_no").val() == "") {
        $("#ref_trans_no_error").text("Reference No. field is required.");
        ref_trans_no.addClass("input-validation-error");
        if (IsValid) {
            Focus(ref_trans_no);
        }
        IsValid = false;
    }


    if (IsValid) {
        $('#x-table-data tbody tr').each(function (i, row) {
            var port = $('input[name="ColateralList[' + i + '].port"]').val();
            if (port == null) {
                swal("Warning", "ColateralList is required.", "warning");
                IsValid = false;
                return;
            }
        });
    }

    if (IsValid) {
        $('#x-table-data tbody tr').each(function (i, row) {
            var clean_price = $('input[name="ColateralList[' + i + '].clean_price"]').val();
            var status = $('input[name="ColateralList[' + i + '].status"]').val();
            if (status !== "delete" && (clean_price === "" || clean_price === null)) {
                swal("Warning", "Clean Price in Bond Collateral is required.", "warning");
                IsValid = false;
                return;
            }
        });
    }

    if (IsValid) {
        $('#x-table-data tbody tr').each(function (i, row) {
            var dirty_price = $('input[name="ColateralList[' + i + '].dirty_price"]').val();
            var status = $('input[name="ColateralList[' + i + '].status"]').val();
            if (status !== "delete" && (dirty_price === "" || dirty_price === null)) {
                swal("Warning", "Dirty Price in Bond Collateral is required.", "warning");
                IsValid = false;
                return;
            }
        });
    }

    if (IsValid) {
        $('#x-table-data tbody tr').each(function (i, row) {
            var port = $('input[name="ColateralList[' + i + '].port"]').val();
            var status = $('input[name="ColateralList[' + i + '].status"]').val();
            if (status !== "delete" && (port === "" || port === null)) {
                swal("Warning", "Port in Bond Collateral is required.", "warning");
                IsValid = false;
                return;
            }
        });
    }

    if (IsValid) {
        $('#x-table-data tbody tr').each(function (i, row) {
            var amount = $('input[name="ColateralList[' + i + '].cash_amount"]').val();
            var status = $('input[name="ColateralList[' + i + '].status"]').val();
            if (status !== "delete" & parseFloat(amount.replace(/,/g, '')) <= 0.00) {
                swal("Warning", "Cash Amount Not Zero In Bond", "warning");
                IsValid = false;
                return;
            }
        });
    }

    if (IsValid) {
        console.log('CompareDealPrice Start');
        var exch_rate = $('#exch_rate').val() === "" ? 0.00 : parseFloat($('#exch_rate').val().replace(/,/g, ''));
        var sumcash_amt = $('#Sum_cash_amount').html() == "" ? 0.00 : parseFloat($('#Sum_cash_amount').html().replace(/,/g, ''));
        var purchase_amt = purchase_price.val() == "" ? 0.00 : parseFloat(purchase_price.val().replace(/,/g, ''));
        //if (FormatDecimal(sumcash_amt, 2) != FormatDecimal(purchase_amt * exch_rate, 2) && $("#ismanual_cal").val().toUpperCase() == "FALSE") {

        let resultCompareDealPrice = CompareDealPrice(FormatDecimal(purchase_amt * exch_rate, 2), $("#cur_pair2").val(), FormatDecimal(sumcash_amt, 2), $("#cur_pair2").val());
        console.log("resultCompareDealPrice : " + resultCompareDealPrice);
        if (resultCompareDealPrice === 'N' && $("#ismanual_cal").val().toUpperCase() == "FALSE") {
            IsValid = false;
            swal("Fail", "Purchase Price is not equal to Cash Amount of Bond Collateral", "error");
        }

        console.log('CompareDealPrice END');
    }

    var isAmend = $('#isAmend').val();
    var remark_id = $('#remark_id');
    if (IsValid && isAmend === 'True' && remark_id.val() == '') {
        $("#remark-deal-form").removeClass("form-container form-horizontal have-head collapse");
        $("#remark-deal-form").addClass("form-container form-horizontal have-head collapse in");
        $("#remark_id_error").text("The Remark field is required for Amend.");
        remark_id.addClass("input-validation-error");
        swal("Fail", "The Remark field is required for Amend.", "error");
        Focus(remark_id);
        IsValid = false;
    } else {
        $("#remark_id_error").text("");
        remark_id.removeClass("input-validation-error");
    }

    var remark_desc = $('#remark_desc');
    var deal_remark = $('#deal_remark');
    if (IsValid && (remark_desc.val().toUpperCase() == "OTHER" || remark_desc.val().toUpperCase() == "CANCEL") && deal_remark.val() == '') {
        $("#remark-deal-form").removeClass("form-container form-horizontal have-head collapse");
        $("#remark-deal-form").addClass("form-container form-horizontal have-head collapse in");
        $("#deal_remark_error").text("The Deal Remark field is required for Amend.");
        deal_remark.addClass("input-validation-error");
        swal("Fail", "The Deal Remark field is required for Amend.", "error");
        Focus(remark_desc);
        IsValid = false;
    } else {
        $("#deal_remark_error").text("");
        deal_remark.removeClass("input-validation-error");
    }
    return IsValid;
}

function Focus(input) {
    var center = $(window).height() / 2;
    var top = $(input).offset().top;
    if (top > center) {
        $(window).scrollTop(top - center);
    }
    input.focus();
    input.click();
}

function FillFloatRate() {

    var data = {
        cur: $("#cur").val(),
        date: $("#settlement_date").val()
    };

    $.ajax({
        url: "/RPDealEntry/FillFloatRate",
        type: "GET",
        dataType: "JSON",
        data: data,
        success: function (res) {

            let text = 'Select...';
            let value = 0;

            for (var i = 0; i < res.length; i++) {
                if (res[i].Text === 'POLICY RATE') {
                    text = res[i].Text;
                    value = res[i].Value;
                }
            }

            $("#ddl_intrate").find(".selected-data").val(text);
            $("#ddl_intrate").find(".selected-data").text(text);
            $("#ddl_intrate").find(".selected-value").val(value);


            $("#ddl_costoffund").find(".selected-data").val(text);
            $("#ddl_costoffund").find(".selected-data").text(text);
            $("#ddl_costoffund").find(".selected-value").val(value);

            SumInterestRate();
            SumCostOfFund();

        }
    });
}

function text_OnKeyPress_NumberOnlyAndDot(obj) {
    var keyAble = false;
    try {
        key = window.event.keyCode;
        var beforeVal = obj.value;
        if (beforeVal.indexOf('.') !== -1) {
            if (key === 46) {
                keyAble = false;
            } else if (key > 47 && key < 58) {
                keyAble = true; // if so, do nothing
            } else {
                keyAble = false;
            }
        } else if (key === 46 || (key > 47 && key < 58)) {
            keyAble = true; // if so, do nothing
        } else {
            keyAble = false;
        }
    } catch (err) {
        throw new Error(" error in sub text_OnKeyPress_NumberOnlyAndDot function() cause = " + err.message);
    }
    return keyAble;
}

function text_OnKeyPress_NumberOnlyAndDotAndM(obj, name) {
    var keyAble = false;
    try {
        key = window.event.keyCode;
        var val = obj.value;
        if (val.indexOf('.') !== -1) {
            if (key === 46) {
                keyAble = false;
            } else if (key > 47 && key < 58) {
                keyAble = true; // if so, do nothing
            } else if (key === 77 || key === 109) {
                if (val.indexOf('m') !== -1) {
                    keyAble = false;
                } else if (val.indexOf('M') !== -1) {
                    keyAble = false;
                } else {
                    keyAble = true;
                }
            }
        } else if (key === 46 || (key > 47 && key < 58)) {
            keyAble = true; // if so, do nothing
        } else if (key === 77 || key === 109) {
            if (val.indexOf('m') !== -1) {
                keyAble = false;
            } else if (val.indexOf('M') !== -1) {
                keyAble = false;
            } else {
                keyAble = true;
            }
        } else {
            keyAble = false;
        }
    } catch (err) {
        throw new Error(" error in sub text_OnKeyPress_NumberOnlyAndDotAndM function() cause = " + err.message);
    }

    if (name !== undefined) {
        lastCol = name;
    }

    return keyAble;
}

function text_OnKeyPress_period(obj) {
    var keyAble = false;
    try {
        key = window.event.keyCode;
        var beforeVal = obj.value;
        if ((key > 47 && key < 58)) {
            keyAble = true; // if so, do nothing
        } else if ((key === 77 || key === 87 || key === 89 || key === 109 || key === 119 || key === 121)) {
            var val = obj.value;
            if (val.indexOf('m') !== -1 || val.indexOf('M') !== -1 ||
                val.indexOf('W') !== -1 || val.indexOf('w') !== -1 ||
                val.indexOf('Y') !== -1 || val.indexOf('y') !== -1) {
                keyAble = false;
            } else {
                keyAble = true;
            }
        } else {
            keyAble = false;
        }
    } catch (err) {
        throw new Error(" error in sub text_OnKeyPress_NumberOnlyAndDotAndM function() cause = " + err.message);
    }
    return keyAble;
}

var lastCol;

function text_OnKeyPress_NumberOnlyAndDotAndMinus(obj, name) {
    var keyAble = false;
    try {
        key = window.event.keyCode;
        var beforeVal = obj.value;
        if (beforeVal.indexOf(".") !== -1) {
            if (key === 46) {
                keyAble = true;
            } else if (key > 47 && key < 58) {
                keyAble = true; // if so, do nothing
            } else if (key === 45) {
                if (!beforeVal.indexOf("-") !== -1 && obj.selectionStart === 0) {
                    keyAble = true;
                } else {
                    keyAble = false;
                }
            } else {
                keyAble = false;
            }
        } else if (key === 46 || (key > 47 && key < 58)) {
            keyAble = true; // if so, do nothing
        } else if (key === 45) {
            if (!beforeVal.indexOf("-") !== -1 && obj.selectionStart === 0) {
                keyAble = true;
            } else {
                keyAble = false;
            }
        } else {
            keyAble = false;
        }
    } catch (err) {
        throw new Error(" error in sub text_OnKeyPress_NumberOnlyAndDotAndMinus function() cause = " + err.message);
    }

    if (name !== undefined) {
        lastCol = name;
    }
    return keyAble;
}

function checkAvalibleAmount(valPrice) {

    var _1M = 1000000;
    var num = 0;
    var cal = 0;
    if (valPrice.indexOf('m') !== -1) {
        num = valPrice.substring(0, valPrice.indexOf('m'));
        cal = num * _1M;
    } else if (valPrice.indexOf('M') !== -1) {
        num = valPrice.substring(0, valPrice.indexOf('M'));
        cal = num * _1M;
    } else {
        num = valPrice;
        cal = num;
    }
    return cal;
}

function CallPrice() {
    if ($("#deal_period").val() !== "") {
        var data = {
            purchaseprice: $("#purchase_price").val(),
            period: $("#deal_period").val(),
            wht: $("#wht_tax").val(),
            yearbasis: $("#basis_code").val(),
            totalint: $("#interest_total").val(),
            counter_party_id: $("#counter_party_id").val(),
            cur: $("#cur").val(),
            repo_deal_type: getinstrumenttypeselect(),
            instrumenttype: $("#trans_deal_type").val()
        };
        $.ajax({
            url: "/RPDealEntry/GetCallPrice",
            type: "GET",
            dataType: "JSON",
            data: data,
            async: false,
            success: function (res) {
                if (res.length > 0) {
                    if (res[0].message === null) {
                        var repurchase_price = FormatDecimal(res[0].repurchase_price, 2);
                        var interest_amount = FormatDecimal(res[0].interest_amount, 2);
                        var withholding_amount = FormatDecimal(res[0].withholding_amount, 2);

                        $("#lbl_repurchase_price").text(repurchase_price);
                        $("#lbl_interest_amount").text(interest_amount);
                        if (tmp_abosrb === false) {
                            $("#withholding_amount_text").text(withholding_amount);
                        } else {
                            $("#withholding_amount_text").text("");
                        }
                        $("#repurchase_price").val(repurchase_price).text(repurchase_price);
                        $("#interest_amount").val(interest_amount).text(interest_amount);
                        $("#withholding_amount").val(withholding_amount);
                    } else {
                        swal("Warning", res[0].message, "warning");
                    }
                }
            }
        });
    }
}

function Getcalenda(btn) {
    if (!btn.readOnly) {
        $(btn).datepicker({
            dateFormat: 'dd/mm/yy'
        });
        //$("#purchase_price").focus();
        $(btn).focusout();
        $(btn).focus();
    }
}

function checkinstrument() {
    var instrument;
    var radioprp = $("[id=repo_deal_type][value=PRP]");
    var radiobrp = $("[id=repo_deal_type][value=BRP]");
    if (radioprp.attr("ischeck") == "true") {
        instrument = "PRP";
    } else {
        instrument = "BRP";
    }

    return instrument;
}

function SumCashAmountBond(exch_rate, sumAmt) {
    var sumCashAmountBond = 0;
    if (sumAmt) {
        sumCashAmountBond = parseFloat(sumAmt.replace(/,/g, '')) / exch_rate;
    } else {
        $('#x-table-data tbody tr').each(function (i, row) {
            var status = $('input[name="ColateralList[' + i + '].status"]').val();
            if (status !== "delete") {
                var amount = $('input[name="ColateralList[' + i + '].cash_amount"]').val();
                if (amount != null) {
                    sumCashAmountBond += parseFloat(amount.replace(/,/g, '')) / exch_rate;
                }
            }
        });
    }
    return sumCashAmountBond;
}

function SumInterestBond() {
    var sum = 0;
    $('#x-table-data tbody tr').each(function (i, row) {
        var status = $('input[name="ColateralList[' + i + '].status"]').val();
        if (status !== "delete") {
            var amount = $('input[name="ColateralList[' + i + '].interest_amount"]').val();
            if (amount != null && amount !== '') {
                sum += parseFloat(amount.replace(/,/g, ''));
            }
        }
    });
    return sum;
}

function SumWhtBond() {
    var sum = 0;
    $('#x-table-data tbody tr').each(function (i, row) {
        var status = $('input[name="ColateralList[' + i + '].status"]').val();
        if (status !== "delete") {
            var amount = $('input[name="ColateralList[' + i + '].wht_amount"]').val();
            if (amount != null && amount !== '') {
                sum += parseFloat(amount.replace(/,/g, ''));
            }
        }
    });
    return sum;
}

function checkpurchaseprice() {
    var purchase_price = $('#purchase_price').val() == "" ? 0.00 : parseFloat($('#purchase_price').val().replace(/,/g, ''));

    var table = $('#x-table-data').DataTable();
    var lastrow = table.data().count();
    if (lastrow > 0) {
        var adjustamountlastrow = 0;
        var sumcash_amount = $('#Sum_cash_amount').html() == "" ? 0.00 : parseFloat($('#Sum_cash_amount').html().replace(/,/g, ''));
        if (parseFloat(sumcash_amount) < parseFloat(purchase_price)) {
            adjustamountlastrow = parseFloat(purchase_price) - parseFloat(sumcash_amount);
        }
        var namefield = 'ColateralList[' + (lastrow - 1) + '].CASH_AMOUNT';
        var cashamountlastrow = parseFloat(table.cell(lastrow - 1, 15).data()) + parseFloat(adjustamountlastrow);
        table.cell(lastrow - 1, 15).data(cashamountlastrow);

        CalBondForPurchasePrice(namefield, false);
    }
}

function checknumber(currentname, callfunction) {
    if (!document.getElementsByName(currentname)[0].readOnly) {
        if (callfunction) {
            var id = currentname.split("[")[1].split("]")[0];
            var rpsource = $("#rp_source_value").val();
            var rppricedate = $("#rp_price_date_value").val();
            var formula = $("#formula").val();
            var interestratetotal = $("#interest_total").val();
            var period = $("#deal_period").val();
            var yearbasis = $("#basis_code").val();
            var wht = $("#wht_tax").val();

            var interestAmt = $("#interest_amount").val();
            var withholding_amount = $("#withholding_amount").val();

            var exch_rate = $("#exch_rate").val() === "" ? 0 : parseFloat($("#exch_rate").val().replace(/,/g, ''));
            var purchase_price = $("#purchase_price").val() === "" ? 0 : parseFloat($("#purchase_price").val().replace(/,/g, ''));

            // ColateralList
            var instrumentcode = $('input[name="ColateralList[' + id + '].instrument_id"]').val();
            var parunit = $('input[name="ColateralList[' + id + '].par_unit"]').val();
            var ytm = $('input[name="ColateralList[' + id + '].ytm"]').val();
            var dm = $('input[name="ColateralList[' + id + '].dm"]').val() === "" ? null : $('input[name="ColateralList[' + id + '].dm"]').val().replace(/,/g, '');
            var cleanprice = $('input[name="ColateralList[' + id + '].clean_price"]').val() === "" ? null : $('input[name="ColateralList[' + id + '].clean_price"]').val().replace(/,/g, '');
            var dirtyprice = $('input[name="ColateralList[' + id + '].dirty_price"]').val() === "" ? null : $('input[name="ColateralList[' + id + '].dirty_price"]').val().replace(/,/g, '');
            var hc = $('input[name="ColateralList[' + id + '].haircut"]').val();
            var vm = $('input[name="ColateralList[' + id + '].variation"]').val() === "" ? null : $('input[name="ColateralList[' + id + '].variation"]').val().replace(/,/g, '');
            var parval = $('input[name="ColateralList[' + id + '].par"]').val();
            var secmarketval = $('input[name="ColateralList[' + id + '].market_value"]').val();
            var unit = $('input[name="ColateralList[' + id + '].unit"]').val() === "" ? 0 : parseFloat($('input[name="ColateralList[' + id + '].unit"]').val().replace(/,/g, ''));
            var cashamount = $('input[name="ColateralList[' + id + '].cash_amount"]').val() === "" ? 0 : parseFloat($('input[name="ColateralList[' + id + '].cash_amount"]').val().replace(/,/g, ''));
            var interest_amount = $('input[name="ColateralList[' + id + '].interest_amount"]').val() === "" ? 0 : parseFloat($('input[name="ColateralList[' + id + '].interest_amount"]').val().replace(/,/g, ''));
            var wht_amount = $('input[name="ColateralList[' + id + '].wht_amount"]').val() === "" ? 0 : parseFloat($('input[name="ColateralList[' + id + '].wht_amount"]').val().replace(/,/g, ''));

            var trigger = currentname.split(".")[1];
            var settlementdate = $("#settlement_date").val();
            var counterparty_id = $("#counter_party_id").val();
            var cur = $("#cur_pair1").val();

            var table = $('#x-table-data').DataTable();
            var lastrow = table.data().count() - 1;

            var isLastRecord = lastrow > 0 && id == lastrow && (SumCashAmountBond(exch_rate) >= purchase_price || cashamount > 0) ? true : false;
            if (isLastRecord) {
                wht_amount = parseFloat(withholding_amount.replace(/,/g, '')) - (parseFloat(FormatDecimal(SumWhtBond(), 2).replace(/,/g, '')) - parseFloat(wht_amount));
                interest_amount = parseFloat(interestAmt.replace(/,/g, '')) - (parseFloat(FormatDecimal(SumInterestBond(), 2).replace(/,/g, '')) - parseFloat(interest_amount));

                wht_amount = wht_amount < 0 ? 0 : wht_amount;
                interest_amount = interest_amount < 0 ? 0 : interest_amount;

                if (wht_amount === 0 && interest_amount === 0) {
                    isLastRecord = false;
                }
            }

            var data = {
                rpsource: rpsource,
                rppricedate: rppricedate,
                period: period,
                wht: wht,
                yearbasis: yearbasis,
                totalint: interestratetotal,
                instrumentcode: instrumentcode,
                parunit: parunit,
                ytm: ytm,
                dm: dm,
                cleanprice: cleanprice,
                dirtyprice: dirtyprice,
                hc: hc,
                vm: vm,
                unit: unit,
                parval: parval,
                secmarketval: secmarketval,
                cashamount: cashamount,
                interest_amount: interest_amount,
                wht_amount: wht_amount,
                trigger: trigger,
                formula: formula,
                settlementdate: settlementdate,
                counterpartyid: counterparty_id,
                exch_rate: exch_rate,
                cur: cur,
                sessionName: $('#SessionName').val(),
                isLastRecord: isLastRecord,
                specialCaseId: $('#special_case_id').val()
            };
            $.ajax({
                url: "/RPDealEntry/GetCheckCallFromText",
                type: "GET",
                dataType: "JSON",
                data: data,
                success: function (res) {
                    if (res.length > 0) {
                        $('input[name="ColateralList[' + id + '].ytm"]').val(FormatDecimal(res[0].ytm, 6));
                        $('input[name="ColateralList[' + id + '].haircut"]').val(FormatDecimal(res[0].haircut, 2));
                        $('input[name="ColateralList[' + id + '].unit"]').val(FormatDecimal(res[0].unit, 0));
                        $('input[name="ColateralList[' + id + '].par"]').val(FormatDecimal(res[0].par, 2));
                        $('input[name="ColateralList[' + id + '].market_value"]').val(FormatDecimal(res[0].market_value, 2));
                        $('input[name="ColateralList[' + id + '].cash_amount"]').val(FormatDecimal(res[0].cash_amount, 2));

                        if (currentname.indexOf('par') === -1 && parseFloat(res[0].dirty_price_after_hc) >= parseFloat(res[0].cash_amount)) {
                            $('input[name="ColateralList[' + id + '].dirty_price_after_hc"]').val(FormatDecimal(res[0].dirty_price_after_hc, 2));
                        }

                        $('input[name="ColateralList[' + id + '].trigger"]').val(res[0].trigger);

                        if (res[0].wht_amount !== null) {
                            $('input[name="ColateralList[' + id + '].wht_amount"]').val(FormatDecimal(res[0].wht_amount, 2));
                        }
                        if (res[0].interest_amount !== null) {
                            $('input[name="ColateralList[' + id + '].interest_amount"]').val(FormatDecimal(res[0].interest_amount, 2));
                        }
                        if (res[0].terminate_amount !== null) {
                            $('input[name="ColateralList[' + id + '].temination_value"]').val(FormatDecimal(res[0].terminate_amount, 2));
                        }

                        if (res[0].variation !== null) {
                            $('input[name="ColateralList[' + id + '].variation"]').val(FormatDecimal(res[0].variation, 2));
                        }

                        if (res[0].dm !== null) {
                            $('input[name="ColateralList[' + id + '].dm"]').val(FormatDecimal16(dm));
                        }

                        if (res[0].dirty_price !== null) {
                            $('input[name="ColateralList[' + id + '].dirty_price"]').val(FormatDecimal(res[0].dirty_price, 6));
                        }
                        if (res[0].clean_price !== null) {
                            $('input[name="ColateralList[' + id + '].clean_price"]').val(FormatDecimal(res[0].clean_price, 6));
                        }

                        $("#Sum_cash_amount").html(FormatDecimal(res[0].sum_cash_amount, 2));
                        $("#Sum_interest_amount").html(FormatDecimal(res[0].sum_interest_amount, 2));
                        $("#Sum_wht_amount").html(FormatDecimal(res[0].sum_wht_amount, 2));
                        $("#Sum_temination_value").html(FormatDecimal(res[0].sum_temination_value, 2));

                        sumCashAmountBond = SumCashAmountBond(exch_rate, FormatDecimal(res[0].sum_cash_amount, 2));

                        if ($("#ismanual_cal").val().toUpperCase() == "FALSE" && purchase_price != sumCashAmountBond) {
                            $("#purchase_price").val(FormatDecimal(sumCashAmountBond, 2));

                            CallPrice();
                            if (currentname != null && currentname !== undefined) {
                                setTimeout(
                                    function () {
                                        $('#x-table-data tbody tr').each(function (i, row) {
                                            var checkrow = $('#x-table-data tbody tr').children().eq(4).length;
                                            if (checkrow > 0) {
                                                CalBond(currentname, true);
                                            }
                                        });
                                    },
                                    1000);
                            }
                        }

                        if (id < lastrow) {
                            var cashamountNextBond = $('input[name="ColateralList[' + lastrow + '].cash_amount"]').val() === "" ? 0 : parseFloat($('input[name="ColateralList[' + lastrow + '].cash_amount"]').val().replace(/,/g, ''));

                            if (purchase_price > sumCashAmountBond) {

                                var diff = purchase_price - sumCashAmountBond;
                                cashamountNextBond = cashamountNextBond + diff;

                                CalculateBond('ColateralList[' + lastrow + '].CASH_AMOUNT', false, cashamountNextBond);
                                setTimeout(function () {
                                    var wht_adj = parseFloat(withholding_amount.replace(/,/g, '')) - parseFloat(FormatDecimal(SumWhtBond(), 2).replace(/,/g, ''));
                                    var interest_adj = parseFloat(interestAmt.replace(/,/g, '')) - parseFloat(FormatDecimal(SumInterestBond(), 2).replace(/,/g, ''));
                                    if (wht_adj !== 0 || interest_adj !== 0) {
                                        CalculateBond('ColateralList[' + lastrow + '].CASH_AMOUNT', false, cashamountNextBond);
                                    }
                                }, 2000);
                            }
                        }
                    }
                }
            });
        }
    }
    return true;
}

function SetPostBack(btn, row) {
    setTimeout(
        function () {
            var port = $(btn).val();
            var instrument_id = $('input[name="ColateralList[' + row + '].instrument_id"]').val();
            var data = {
                instrument_id: instrument_id,
                port: port,
                sessionName: $('#SessionName').val()
            };

            $.ajax({
                url: "/RPDealEntry/SetPortColl",
                type: "POST",
                dataType: "JSON",
                data: data
            });
        }, 100);
}

function SetPostBackDefult(port, row) {
    setTimeout(
        function () {
            var instrument_id = $('input[name="ColateralList[' + row + '].instrument_id"]').val();
            var data = {
                instrument_id: instrument_id,
                port: port,
                sessionName: $('#SessionName').val()
            };

            $.ajax({
                url: "/RPDealEntry/SetPortColl",
                type: "POST",
                dataType: "JSON",
                data: data
            });
        }, 100);
}

function CalBond(currentname, changePurchasePrice) {
    var id = currentname.split("[")[1].split("]")[0];
    var cashamount = $('input[name="ColateralList[' + id + '].cash_amount"]').val() === "" ? 0 : parseFloat($('input[name="ColateralList[' + id + '].cash_amount"]').val().replace(/,/g, '')); // in record blur
    var exch_rate = $("#exch_rate").val() === "" ? 0 : parseFloat($("#exch_rate").val().replace(/,/g, ''));

    if (oldExchRate != exch_rate) {

        console.log("oldExchRate : " + oldExchRate);
        console.log("exch_rate : " + exch_rate);
        cashamount = (cashamount / oldExchRate) * exch_rate;
        console.log("cashamount : " + cashamount);
    }

    CalculateBond(currentname, changePurchasePrice, cashamount, oldExchRate);

    oldExchRate = exch_rate;
}

function CalBondForPurchasePrice(currentname, changePurchasePrice) {
    var id = currentname.split("[")[1].split("]")[0];
    var cashamount = $('input[name="ColateralList[' + id + '].cash_amount"]').val() === "" ? 0 : parseFloat($('input[name="ColateralList[' + id + '].cash_amount"]').val().replace(/,/g, '')); // in record blur
    var purchase_price = $("#purchase_price").val() === "" ? 0 : parseFloat($("#purchase_price").val().replace(/,/g, ''));
    var exch_rate = $("#exch_rate").val() === "" ? 0 : parseFloat($("#exch_rate").val().replace(/,/g, ''));
    var sumCashAmountBond = SumCashAmountBond(exch_rate);

    if (id !== '0') {
        if (purchase_price < sumCashAmountBond) {
            cashamount = ((cashamount / exch_rate) - (sumCashAmountBond - purchase_price)) * exch_rate;
            if (cashamount < 0) {
                cashamount = 0;
            }
        } else if (purchase_price > sumCashAmountBond) {
            cashamount = ((cashamount / exch_rate) + (purchase_price - sumCashAmountBond)) * exch_rate;
        }
    } else {
        cashamount = purchase_price * exch_rate;
    }

    CalculateBond(currentname, changePurchasePrice, cashamount, exch_rate);
}

function CalculateBond(currentname, changePurchasePrice, cashamount, oldExchRate) {
    var id = currentname.split("[")[1].split("]")[0];
    var rpsource = $("#rp_source_value").val();
    var rppricedate = $("#rp_price_date_value").val();
    var formula = $("#formula").val();
    var interestratetotal = $("#interest_total").val();
    var period = $("#deal_period").val();
    var yearbasis = $("#basis_code").val();
    var wht = $("#wht_tax").val();
    var interestAmt = $("#interest_amount").val();
    var withholding_amount = $("#withholding_amount").val();

    var instrumentcode = $('input[name="ColateralList[' + id + '].instrument_id"]').val();
    var parunit = $('input[name="ColateralList[' + id + '].par_unit"]').val();
    var ytm = $('input[name="ColateralList[' + id + '].ytm"]').val();
    var dm = $('input[name="ColateralList[' + id + '].dm"]').val() === "" ? null : $('input[name="ColateralList[' + id + '].dm"]').val().replace(/,/g, '');
    var cleanprice = $('input[name="ColateralList[' + id + '].clean_price"]').val() === "" ? null : $('input[name="ColateralList[' + id + '].clean_price"]').val().replace(/,/g, '');
    var dirtyprice = $('input[name="ColateralList[' + id + '].dirty_price"]').val() === "" ? null : $('input[name="ColateralList[' + id + '].dirty_price"]').val().replace(/,/g, '');
    var hc = $('input[name="ColateralList[' + id + '].haircut"]').val();
    var vm = $('input[name="ColateralList[' + id + '].variation"]').val() === "" ? null : $('input[name="ColateralList[' + id + '].variation"]').val().replace(/,/g, '');
    var parval = $('input[name="ColateralList[' + id + '].par"]').val();
    var secmarketval = $('input[name="ColateralList[' + id + '].market_value"]').val();
    var unit = $('input[name="ColateralList[' + id + '].unit"]').val() === "" || $('input[name="ColateralList[' + id + '].unit"]').val() == null ? 0 : parseFloat($('input[name="ColateralList[' + id + '].unit"]').val().replace(/,/g, ''));
    var interest_amount = $('input[name="ColateralList[' + id + '].interest_amount"]').val() === "" || $('input[name="ColateralList[' + id + '].interest_amount"]').val() == null ? 0 : parseFloat($('input[name="ColateralList[' + id + '].interest_amount"]').val().replace(/,/g, ''));
    var wht_amount = $('input[name="ColateralList[' + id + '].wht_amount"]').val() === "" || $('input[name="ColateralList[' + id + '].wht_amount"]').val() == null ? 0 : parseFloat($('input[name="ColateralList[' + id + '].wht_amount"]').val().replace(/,/g, ''));

    var exch_rate = $("#exch_rate").val() === "" ? 0 : parseFloat($("#exch_rate").val().replace(/,/g, ''));
    var purchase_price = $("#purchase_price").val() === "" ? 0 : parseFloat($("#purchase_price").val().replace(/,/g, ''));
    var cur = $("#cur_pair1").val();

    var trigger = currentname.split(".")[1];
    var settlementdate = $("#settlement_date").val();
    var counterparty_id = $("#counter_party_id").val();

    var table = $('#x-table-data').DataTable();
    var lastrow = table.data().count() - 1;

    console.log("oldExchRate");
    console.log(oldExchRate);
    var tmpExchRate;
    if (oldExchRate !== null && oldExchRate !== undefined) {
        tmpExchRate = oldExchRate;
    } else {
        tmpExchRate = exch_rate;
    }

    console.log("cashamount : " + cashamount);
    var isLastRecord = lastrow > 0 && id == lastrow && (SumCashAmountBond(tmpExchRate) >= purchase_price || cashamount > 0) ? true : false;

    if (isLastRecord) {
        wht_amount = parseFloat(withholding_amount.replace(/,/g, '')) - (parseFloat(FormatDecimal(SumWhtBond(), 2).replace(/,/g, '')) - parseFloat(wht_amount));
        interest_amount = parseFloat(interestAmt.replace(/,/g, '')) - (parseFloat(FormatDecimal(SumInterestBond(), 2).replace(/,/g, '')) - parseFloat(interest_amount));

        wht_amount = wht_amount < 0 ? 0 : wht_amount;
        interest_amount = interest_amount < 0 ? 0 : interest_amount;

        if (wht_amount === 0 && interest_amount === 0) {
            isLastRecord = false;
        }
    }

    var data = {
        rpsource: rpsource,
        rppricedate: rppricedate,
        period: period,
        wht: wht,
        yearbasis: yearbasis,
        totalint: interestratetotal,
        instrumentcode: instrumentcode,
        parunit: parunit,
        ytm: ytm,
        dm: dm,
        cleanprice: cleanprice,
        dirtyprice: dirtyprice,
        hc: hc,
        vm: vm,
        unit: unit,
        parval: parval,
        secmarketval: secmarketval,
        cashamount: cashamount,
        trigger: trigger,
        formula: formula,
        settlementdate: settlementdate,
        counterpartyid: counterparty_id,
        cur: cur,
        sessionName: $('#SessionName').val(),
        interest_amount: interest_amount > 0 ? interest_amount : 0,
        wht_amount: wht_amount > 0 ? wht_amount : 0,
        isLastRecord: isLastRecord,
        specialCaseId: $('#special_case_id').val()
    };
    $.ajax({
        url: "/RPDealEntry/GetCheckCallFromText",
        type: "GET",
        dataType: "JSON",
        data: data,
        async: false,
        success: function (res) {
            if (res.length > 0) {
                $('input[name="ColateralList[' + id + '].ytm"]').val(FormatDecimal(res[0].ytm, 6));
                $('input[name="ColateralList[' + id + '].haircut"]').val(FormatDecimal(res[0].haircut, 2));
                $('input[name="ColateralList[' + id + '].unit"]').val(FormatDecimal(res[0].unit, 0));
                $('input[name="ColateralList[' + id + '].par"]').val(FormatDecimal(res[0].par, 2));
                $('input[name="ColateralList[' + id + '].market_value"]').val(FormatDecimal(res[0].market_value, 2));
                console.log("res[0].cash_amount : " + res[0].cash_amount);
                $('input[name="ColateralList[' + id + '].cash_amount"]').val(FormatDecimal(res[0].cash_amount, 2));

                if (currentname.indexOf('par') === -1 && res[0].dirty_price_after_hc >= res[0].cash_amount) {
                    $('input[name="ColateralList[' + id + '].dirty_price_after_hc"]').val(FormatDecimal(res[0].dirty_price_after_hc, 2));
                }

                if (res[0].wht_amount != null) {
                    $('input[name="ColateralList[' + id + '].wht_amount"]').val(FormatDecimal(res[0].wht_amount, 2));
                }
                if (res[0].interest_amount != null) {
                    $('input[name="ColateralList[' + id + '].interest_amount"]').val(FormatDecimal(res[0].interest_amount, 2));
                }
                if (res[0].terminate_amount != null) {
                    $('input[name="ColateralList[' + id + '].temination_value"]').val(FormatDecimal(res[0].terminate_amount, 2));
                }

                if (res[0].variation !== null) {
                    $('input[name="ColateralList[' + id + '].variation"]').val(FormatDecimal(res[0].variation, 2));
                }

                if (res[0].dm !== null) {
                    $('input[name="ColateralList[' + id + '].dm"]').val(FormatDecimal16(res[0].dm));
                }

                if (res[0].dirty_price !== null) {
                    $('input[name="ColateralList[' + id + '].dirty_price"]').val(FormatDecimal(res[0].dirty_price, 6));
                }
                if (res[0].clean_price !== null) {
                    $('input[name="ColateralList[' + id + '].clean_price"]').val(FormatDecimal(res[0].clean_price, 6));
                }

                $("#Sum_cash_amount").html(FormatDecimal(res[0].sum_cash_amount, 2));
                $("#Sum_interest_amount").html(FormatDecimal(res[0].sum_interest_amount, 2));
                $("#Sum_wht_amount").html(FormatDecimal(res[0].sum_wht_amount, 2));
                $("#Sum_temination_value").html(FormatDecimal(res[0].sum_temination_value, 2));
                sumCashAmountBond = SumCashAmountBond(exch_rate);
                if ($("#ismanual_cal").val().toUpperCase() == "FALSE" && changePurchasePrice === true) {
                    $("#purchase_price").val(FormatDecimal(sumCashAmountBond, 2));
                    CallPrice();
                }
                lastCol = null;
            }
        }
    });

    //if (!window.checkcommiteditcol) {
    //    adjusttable();
    //}
}

function CheckHolidayDate() {
    setTimeout(
        function () {
            var data = {
                tradedate: $("#trade_date").val(),
                settlementdate: $("#settlement_date").val(),
                maturitydate: $("#maturity_date").val(),
                period: $("#deal_period").val(),
                cur: $("#cur").val(),
                instrumenttype: $("#trans_deal_type").val(),
                inttype: getinteresttype(),
                transtype: $("#trans_type").val()
            };
            $.ajax({
                url: "/RPDealEntry/GetRPDealCheckDate",
                type: "GET",
                dataType: "JSON",
                data: data,
                async: false,
                success: function (res) {
                    var tradedate = moment(res[0].trade_date).format('DD/MM/YYYY');
                    var settlementdate = moment(res[0].settlement_date).format('DD/MM/YYYY');
                    if (res[0].maturity_date !== null) {
                        var maturitydate = moment(res[0].maturity_date).format('DD/MM/YYYY');
                        $("#maturity_date").val(maturitydate);
                        $("#maturity_date").text(maturitydate);
                        $("#deal_period").val(res[0].deal_period);
                    }


                    $("#trade_date").val(tradedate);
                    $("#settlement_date").val(settlementdate);


                    $("#trade_date").text(tradedate);
                    $("#settlement_date").text(settlementdate);

                    $("#business_period").val(res[0].business_period);


                    if ((res[0].business_period == 1) || (res[0].business_period == 0 && $("#deal_period").val() != "")) {
                        //Overnight
                        $("#ddl_trans_type").find(".selected-data").text("Overnight");
                        $("#ddl_trans_type").find(".selected-value").val("Overnight");
                    } else if (res[0].business_period >= 2) {
                        //Term
                        $("#ddl_trans_type").find(".selected-data").text("Term");
                        $("#ddl_trans_type").find(".selected-value").val("Term");
                    }

                    if (checkinstrument() !== 'PRP') {
                        getbilatcontractno();
                    }

                    CallPrice();

                    var table = $('#x-table-data').DataTable();
                    var lastrow = table.data().count();

                    if (lastrow > 0) {
                        var cashAmountBond = $('input[name="ColateralList[' + (lastrow - 1) + '].cash_amount"]').val() === ""
                            || $('input[name="ColateralList[' + (lastrow - 1) + '].cash_amount"]').val() == null
                            ? 0 : parseFloat($('input[name="ColateralList[' + (lastrow - 1) + '].cash_amount"]').val().replace(/,/g, ''));
                        var calCashamount = true;
                        CalculateBond('ColateralList[' + (lastrow - 1) + '].CASH_AMOUNT', calCashamount, cashAmountBond);
                    }
                }
            });
        },
        300);
}

function CheckPeriod(tenor, tenor_type) {
    setTimeout(
        function () {
            try {
                var data = {
                    settlementdate: $("#settlement_date").val(),
                    period: tenor,
                    cur: $("#cur").val(),
                    transtype: $("#trans_type").val(),
                    tenor_type: tenor_type
                };
                $.ajax({
                    url: "/RPDealEntry/GetPeriod",
                    type: "GET",
                    dataType: "JSON",
                    data: data,
                    success: function (res) {

                        var maturitydate = moment(res[0].maturity_date).format('DD/MM/YYYY');
                        $("#maturity_date").val(maturitydate);
                        $("#deal_period").val(res[0].deal_period);

                        if (res[0].holiday_flag === true && res[0].message !== null) {
                            swal("Warning", res[0].message, "warning");
                        }

                        CheckHolidayDate();
                    }
                });
            } catch (err) {
                console.log("Error : " + err);
            }
        },
        300);
}

function setformatdateyyyymmdd(date) {
    if (date != "") {
        date = date.split('/');
        date = date[2] + "" + date[1] + "" + date[0];
    } else {
        date = 0;
    }
    return date;
}

function SetMatDateAndPeriod() {
    console.log("deal_period " + $("#deal_period").val());
    if ($("#deal_period").val() != "") {
        if ($("#settlement_date").val() != "") {
            var settledate = $("#settlement_date").val();
            var formatmmddyyyydate = settledate.split("/");
            formatmmddyyyydate = formatmmddyyyydate[1] + "/" + formatmmddyyyydate[0] + "/" + formatmmddyyyydate[2];
            settledate = new Date(formatmmddyyyydate);
            settledate.setDate(settledate.getDate() + parseInt($("#deal_period").val().replace(/,/g, '')));

            $("#maturity_date").text(moment(settledate).format('DD/MM/YYYY'));
            $("#maturity_date").val(moment(settledate).format('DD/MM/YYYY'));

            CheckHolidayDate();
        }
    }
}

function getbilatcontractno() {
    //$('.spinner').css('display', 'block'); // Open Loading
    setTimeout(
        function () {
            var data = {
                repo_deal_type: getinstrumenttypeselect(),
                instrumenttype: $("#trans_deal_type").val(),
                inttype: getinteresttype(),
                transtype: $("#trans_type").val(),
                period: $("#deal_period").val(),
                tradedate: $("#trade_date").val(),
                sessionName: $('#SessionName').val()
            };
            $.ajax({
                url: "/RPDealEntry/GetBilatContractFromAjax",
                type: "GET",
                dataType: "JSON",
                data: data,
                async: false,
                success: function (res) {
                    if (checkinstrument() !== 'PRP' && res.length > 0) {
                        var bilateralconno = res[0].bilateral_contract_no;
                        $("#bilateral_contract_no").val(bilateralconno);
                        $("#bilateral_contract_no").text(bilateralconno);
                        $("#bilateral_contract_no").removeAttr("readonly");

                        updateColIsinCodeBOTN(bilateralconno);

                    }

                    //$('.spinner').css('display', 'none'); // Close Loading
                }
            });
        }, 500);
}

function getinstrumenttypeselect() {
    var instrument;
    var radioprp = $("[id=repo_deal_type][value=PRP]");
    var radiobrp = $("[id=repo_deal_type][value=BRP]");
    if (radioprp.attr("ischeck") == "true") {
        instrument = "PRP";
    } else {
        instrument = "BRP";
    }
    return instrument;
}

function getinteresttype() {
    var inttype;
    var radioprp = $("[id=interest_type][value=FIXED]");

    if (radioprp.attr("ischeck") == "true") {
        inttype = "FIXED";
    } else {
        inttype = "FLOAT";
    }

    return inttype;
}

function getcosttype() {
    var inttype;
    var radioprp = $("[id=cost_type][value=FIXED]");
    if (radioprp.attr("ischeck") == "true") {
        inttype = "FIXED";
    } else {
        inttype = "FLOAT";
    }

    return inttype;
}

function GetRPpricedate() {
    setTimeout(
        function () {
            var txt_search = $('#txt_rp_pricedate');
            var price_source = $("#rp_source_value").val();
            var tradedate = $("#trade_date").val();
            var settlementdate = $("#settlement_date").val();
            var cur = $("#cur_pair2").val(); // currency at exchange rate

            if (price_source != "" && price_source != null) {
                tradedate = tradedate.split("/")[2] + "" + tradedate.split("/")[1] + "" + tradedate.split("/")[0];
                settlementdate = settlementdate.split("/")[2] + "" + settlementdate.split("/")[1] + "" + settlementdate.split("/")[0];

                var data = {
                    pricesource: price_source,
                    tradedate: tradedate,
                    settlementdate: settlementdate,
                    curr: cur
                };
                $.ajax({
                    url: "/RPDealEntry/GetDDLMarketPrice",
                    type: "GET",
                    dataType: "JSON",
                    data: data,
                    async: false,
                    success: function (res) {
                        var text = "None";
                        var value = null;
                        if (res.length > 0) {
                            text = res[0].Text;
                            value = res[0].Value;
                        }

                        if (IsColleteralAdded()) {

                            var cur_rp_price_val = $("#ddl_rp_pricedate").find(".selected-data").val();
                            if (cur_rp_price_val === "") {
                                cur_rp_price_val = $("#rp_price_date_value").val();
                            }
                            var cur_settlement_date = $("#settlement_date").val();
                            var _deal_period = $("#deal_period").val();

                            if (cur_rp_price_val !== value) {

                                if (old_settlement_date !== "" && old_settlement_date !== cur_settlement_date) {

                                    // warning message RP Price has change
                                    swal("Warning", "Settlement date does not match with Bond Price concept.", "warning");

                                    // Reset Value To Before Action Change Date //
                                    if (_deal_period !== "") {
                                        var newmat = new Date(moment(old_settlement_date, "DD/MM/YYYY")); //new Date(old_settlement_date);
                                        newmat.setDate(newmat.getDate() + parseInt(_deal_period.replace(/,/g, '')));
                                        $("#maturity_date").text(moment(newmat).format('DD/MM/YYYY'));
                                        $("#maturity_date").val(moment(newmat).format('DD/MM/YYYY'));
                                    }

                                    $("#settlement_date").text(old_settlement_date);
                                    $("#settlement_date").val(old_settlement_date);


                                    setTimeout(function () {
                                        FillFloatRate();
                                    }, 1000);

                                }
                            }
                        } else {
                            $("#ddl_rp_pricedate").find(".selected-data").text(text);
                            $("#ddl_rp_pricedate").find(".selected-data").val(value);
                            $("#ddl_rp_pricedate").find(".selected-data").attr("data-toggle", "tooltip");
                            $("#ddl_rp_pricedate").find(".selected-data").attr("title", text);
                            $("#ddl_rp_pricedate").find(".selected-value").val(value);
                        }
                    }
                });
            }
        }, 300);
}

function GetPriceSourceOtherTBMA() {
    $.ajax({
        url: "/RPDealEntry/GetDDLPriceSource",
        type: "GET",
        dataType: "JSON",
        success: function (res) {
            var text = "None";
            var value = null;
            for (var i = 0; i < res.length; i++) {
                if (res[i].Text !== "TBMA") {
                    text = res[i].Text;
                    value = res[i].Value;
                    break;
                }
            }

            $("#rp_source_value").val(value);
            $("#rp_source_text").val(text);
            $("#ddl_rp_source").find(".selected-data").text(text);
            $("#ddl_rp_source").find(".selected-data").val(value);
            $("#ddl_rp_source").find(".selected-data").attr("data-toggle", "tooltip");
            $("#ddl_rp_source").find(".selected-data").attr("title", text);
            $("#ddl_rp_source").find(".selected-value").val(value);

            GetRPpricedate();
        }
    });
}

function GetCounterParty() {
    var transdealtype = $("#trans_deal_type").val();
    data = {
        datastr: null,
        instrumentcode: "BRP",
        dealtype: transdealtype
    };

    $.ajax({
        url: "/RPDealEntry/FillCounterparty",
        type: "GET",
        dataType: "JSON",
        data: data,
        async: false,
        success: function (res) {
            if (res.length > 0) {
                $("#ddl_counterparty").find(".selected-data").val(res[0].Text);
                $("#ddl_counterparty").find(".selected-data").text(res[0].Text);
                $("#ddl_counterparty").find(".selected-value").val(res[0].Value);

                $("#counter_party_id").val(res[0].Value);

                $("#lbl_wht_tax").find(".selected-data").val(FormatDecimal(res[0].Value2, 8));

                var swift_code = res[0].Value3 === "" ? "-" : res[0].Value3;

                $("#lbl_swift_code").find(".selected-data").text(swift_code);
                $("#lbl_swift_code").find(".selected-data").val(swift_code);

                var threshold = res[0].Value4 === "" ? "-" : parseFloat(res[0].Value4);

                $("#lbl_threshold").find(".selected-data").text(threshold);
                $("#lbl_threshold").find(".selected-data").val(threshold);

                GetAbsorb(res[0].Value, res[0].Value2);
                GetCounterPartyFund();
                GetFormula();
                GetPaymentMethod();
            }
        }
    });
}

function GetCounterPartyFund() {
    if ($("#counter_party_id").val() != "") {
        var data = {
            counterpartyid: $("#counter_party_id").val()
        };
        $.ajax({
            url: "/RPDealEntry/FillCounterPartyFund",
            type: "GET",
            dataType: "JSON",
            data: data,
            async: false,
            success: function (res) {
                var text = "None";
                var value = null;
                if (res.length > 0) {
                    text = res[0].Text;
                    value = res[0].Value;
                }

                $("#counter_party_fund_id").val(value);
                $("#counter_party_fund_name").val(text);
                $("#ddl_counterparty_fund").find(".selected-data").text(text);
                $("#ddl_counterparty_fund").find(".selected-data").val(value);
                $("#ddl_counterparty_fund").find(".selected-data").attr("data-toggle", "tooltip");
                $("#ddl_counterparty_fund").find(".selected-data").attr("title", text);

                $("#ddl_counterparty_fund").find(".selected-value").val(value);

            }
        });
    }
}

function GetFormula() {
    var cur_pair2 = $("#cur_pair1").val();
    var counterpartyid = $("#counter_party_id").val();
    var txt_search = $('#txt_formula');
    if (counterpartyid != "" && counterpartyid != null) {
        var data = {
            curpair2: cur_pair2,
            counterpartyid: counterpartyid
        };
        $.ajax({
            url: "/RPDealEntry/FillFormula",
            type: "GET",
            dataType: "JSON",
            data: data,
            success: function (res) {
                var text = "None";
                var value = null;
                if (res.length > 0) {
                    text = res[0].Text;
                    value = res[0].Value;
                }

                $("#formula").val(value);
                $("#formula_name").val(text);
                $("#ddl_formula").find(".selected-data").text(text);
                $("#ddl_formula").find(".selected-data").val(value);
                $("#ddl_formula").find(".selected-data").attr("data-toggle", "tooltip");
                $("#ddl_formula").find(".selected-data").attr("title", text);
                $("#ddl_formula").find(".selected-value").val(value);
            }
        });
    }
}

function GetPaymentMethod() {
    var counterparty_id = $("#counter_party_id").val();
    if (counterparty_id != "" && counterparty_id != null) {
        var payment_flag = null;
        if ($("#trans_deal_type").val() === localStorage.getItem('TRANS_DEAL_TYPE_BORROWING')) {
            payment_flag = 1; //sell
        } else {
            payment_flag = 2; //buy
        }

        var data = {
            counterpartyid: counterparty_id,
            payment_flag: payment_flag
        };
        $.ajax({
            url: "/RPDealEntry/FillCounterPartyPayment",
            type: "GET",
            dataType: "JSON",
            data: data,
            success: function (res) {
                var text = "None";
                var value = null;
                if (res.length > 0) {
                    text = res[0].Text;
                    value = res[0].Value;
                }

                $("#payment_method").val(value);
                $("#ddl_counterparty_payment").find(".selected-data").text(text);
                $("#ddl_counterparty_payment").find(".selected-data").val(text);
                $("#ddl_counterparty_payment").find(".selected-data").attr("data-toggle", "tooltip");
                $("#ddl_counterparty_payment").find(".selected-data").attr("title", text);
                $("#ddl_counterparty_payment").find(".selected-value").val(value);

                var trans_deal_type = $("#trans_deal_type").val();
                var payment_method = value;
                var cur = $("#cur").val();
                var data = {
                    paymentmethod: payment_method,
                    transdealtype: trans_deal_type,
                    cur: cur
                };

                $.ajax({
                    url: "/RPDealEntry/FillMTCode",
                    type: "GET",
                    dataType: "JSON",
                    data: data,
                    success: function (res) {
                        var text = "None";
                        var value = null;
                        if (res.length > 0) {
                            text = res[0].Text;
                            value = res[0].Value;
                        }

                        $("#trans_mt_code").val(value);
                        $("#trans_mt_code").val(text);
                        $("#ddl_trans_mt_code").find(".selected-data").text(text);
                        $("#ddl_trans_mt_code").find(".selected-data").val(value);
                        $("#ddl_trans_mt_code").find(".selected-data").attr("data-toggle", "tooltip");
                        $("#ddl_trans_mt_code").find(".selected-data").attr("title", text);
                        $("#ddl_trans_mt_code").find(".selected-value").val(value);

                    }
                });
            }
        });
    }

}

function GetAbsorb(counter_party_id, value) {
    $.ajax({
        url: "/RPDealEntry/GetAbsorb",
        type: "POST",
        dataType: "JSON",
        data: {
            counter_party_id: counter_party_id
        },
        success: function (respons) {
            if (respons.returnCode === 0) {
                $("#absorb").val(FormatDecimal(value, 8) + ' ' + respons.absorb);
                $("#lbl_wht_tax").find(".selected-text").text(FormatDecimal(value, 8) + ' ' + respons.absorb);

                if (respons.absorb.indexOf('Absorb') === -1) {
                    tmp_abosrb = false;
                    $("#withholding_amount_text").text($("#withholding_amount").val());
                } else {
                    tmp_abosrb = true;
                    $("#withholding_amount_text").text("");
                }
            } else {
                console.log(respons.Message);
            }
        }
    });
}

function PreparePriceSource() {
    if (getinstrumenttypeselect() === "BRP" && $('#cur_pair2').val() != "THB") {
        ClearCounterparty();
    } else {
        if ($('#cur_pair2').val() !== "THB") {
            GetPriceSourceOtherTBMA();
        } else {
            $("#rp_source_value").val("TBMA");
            $("#rp_source_text").val("TBMA");
            $("#ddl_rp_source").find(".selected-data").text("TBMA");
            $("#ddl_rp_source").find(".selected-data").val("TBMA");
            $("#ddl_rp_source").find(".selected-value").val("TBMA");
            GetRPpricedate();
        }
    }
}

function CompareDealPrice(price1, cur1, price2, cur2) {
    console.log('CompareDealPrice Process 1');
    var isEqual = 'N';
    $.ajax({
        url: "/RPDealEntry/CompareDealPrice",
        type: "POST",
        dataType: "JSON",
        async: false,
        data: {
            price1: price1,
            cur1: cur1,
            price2: price2,
            cur2: cur2
        },
        success: function (respons) {
            console.log('CompareDealPrice Process 2');
            if (respons.returnCode === 0) {
               isEqual = respons.isEqual;
            } else {
                console.log(respons.Message);
                return false;
            }

        }
    });
    return isEqual;
}

$(document).ready(function () {

    $("#p_plus").click(function () {
        var value = $("#deal_period").val();
        if (value === "") {
            value = 1;
        } else {
            value = parseInt(value.replace(/,/g, '')) + 1;
        }
        $("#deal_period").val(value);

        if (value === 1) {
            //Overnight
            $("#ddl_trans_type").find(".selected-data").text("Overnight");
            $("#ddl_trans_type").find(".selected-value").val("Overnight");
        } else if (value >= 2) {
            //Term
            $("#ddl_trans_type").find(".selected-data").text("Term");
            $("#ddl_trans_type").find(".selected-value").val("Term");
        }

        SetMatDateAndPeriod();

        if (checkinstrument() !== 'PRP') {
            getbilatcontractno();
        }
    });

    $("#p_minus").click(function () {
        var value = $("#deal_period").val();
        if (value == 1 || value == "" || value == "0") {
            value = "";
        } else {
            value = parseInt(value.replace(/,/g, '')) - 1;
        }
        $("#deal_period").val(value);
        if (value != "") {
            value = parseInt(value);
            if (value == 1) {
                //Overnight
                $("#ddl_trans_type").find(".selected-data").text("Overnight");
                $("#ddl_trans_type").find(".selected-value").val("Overnight");
            } else if (value >= 2) {
                //Term
                $("#ddl_trans_type").find(".selected-data").text("Term");
                $("#ddl_trans_type").find(".selected-value").val("Term");
            }
            SetMatDateAndPeriod();

        } else {
            //Term
            $("#ddl_trans_type").find(".selected-data").text("At Call");
            $("#ddl_trans_type").find(".selected-value").val("At Call");

            $("#maturity_date").text("");
            $("#maturity_date").val("");
            $("#deal_period").text("");
            $("#deal_period").val("");
        }

        if (checkinstrument() != 'PRP') {
            getbilatcontractno();
        }
    });

    $('#remark-deal').click(function (e) {
        var expand = $("div#remark-deal-icon").attr("aria-expanded");
        if (expand == "true") {
            $("#remark-deal-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        } else {
            $("#remark-deal-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }

    });

    //$('#trade_date').on('dp.update',
    //    function(e) {
    //        //$('#trade_date').data("DateTimePicker").disabledDates([moment("05/14/2019")]);
    //});

    $('#trade_date').on('dp.change', function (e) {

        if ($('#trade_date').val() !== '') {
            var date = moment(e.date).format('DD/MM/YYYY');
            var tradedate = moment(e.date).format('YYYYMMDD');

            var businessdate = setformatdateyyyymmdd($("#BusinessDate").text());
            var settlementdate = setformatdateyyyymmdd($("#settlement_date").text());

            if ($('#trade_date').text() != date) {
                var olddate = $('#trade_date').text();
                //$('#trade_date').text(date);
                if (tradedate <= businessdate) {
                    //if (settlementdate == 0) {
                    //    $("#settlement_date").text(date);
                    //    $("#settlement_date").val(date);
                    //}
                    //CheckHolidayDate();
                    setTimeout(function () {
                        var data = {
                            date: date,
                            cur: $("#cur").val()
                        };

                        var message = 'The selected trade date is not the current date, please click OK to continue.';
                        $.ajax({
                            url: "/RPDealEntry/CheckHoliday",
                            type: "POST",
                            dataType: "JSON",
                            data: data,
                            async: false,
                            success: function (res) {

                                if (res == true) {
                                    message = date + ' is the holiday, please select new business day.';
                                }

                                setTimeout(function () {
                                    swal({
                                        title: "Warning",
                                        text: message,
                                        type: "warning",
                                        showCancelButton: false,
                                        confirmButtonClass: "btn-success",
                                        confirmButtonText: "Ok",
                                        closeOnConfirm: true
                                    },
                                        function () {

                                            if (settlementdate == 0) {
                                                $("#settlement_date").text(date);
                                                $("#settlement_date").val(date);
                                            }
                                            CheckHolidayDate();

                                        });
                                }, 200);
                            }
                        });
                    }, 200);
                } else {
                    $("#trade_date").text(olddate);
                    $("#trade_date").val(olddate);
                    swal("Warning", "Trade Date Can't more than Business Date", "warning");
                }
            }
        } else {
            $("#trade_date").text("");
            $("#trade_date").val(null);
        }
    });

    $('#settlement_date').on('focusin', function () {
        old_settlement_date = $(this).val();
    });

    $('#settlement_date').on('dp.change', function (e) {

        if ($('#settlement_date').val() !== '') {

            var date = moment(e.date).format('DD/MM/YYYY');
            var settledate = moment(e.date).format('YYYYMMDD');
            var tradedate = setformatdateyyyymmdd($("#trade_date").text());


            if ($('#settlement_date').text() != date) {
                var olddate = $('#settlement_date').text();
                $('#settlement_date').text(date);
                if (settledate >= tradedate) {
                    old_maturity_date = $("#maturity_date").text();
                    if ($("#deal_period").val() !== "") {
                        var newmat = new Date(moment(e.date).format('MM/DD/YYYY'));
                        newmat.setDate(newmat.getDate() + parseInt($("#deal_period").val().replace(/,/g, '')));

                        $("#maturity_date").text(moment(newmat).format('DD/MM/YYYY'));
                        $("#maturity_date").val(moment(newmat).format('DD/MM/YYYY'));
                    }

                    CheckHolidayDate();
                    GetRPpricedate();

                    FillFloatRate();

                    if (checkinstrument() !== 'PRP') {
                        getbilatcontractno();
                    }
                } else {
                    $("#settlement_date").text(olddate);
                    $("#settlement_date").val(olddate);
                    swal("Warning", "Settlement Date Can't less than Trade Date", "warning");
                }
            }
        }
        else {
            $("#settlement_date").text("");
            $("#settlement_date").val(null);
        }
    });

    $('#maturity_date').on('dp.change', function (e) {
        if ($('#maturity_date').val() !== '') {
            var date = moment(e.date).format('DD/MM/YYYY');
            var matdate = moment(e.date).format('YYYYMMDD');
            var settlementdate = setformatdateyyyymmdd($("#settlement_date").text());

            if ($('#maturity_date').text() !== date) {
                var olddate = $('#maturity_date').text();
                $('#maturity_date').text(date);
                if (matdate > settlementdate) {

                    CheckHolidayDate();
                    if (checkinstrument() !== 'PRP') {
                        getbilatcontractno();
                    }
                } else {
                    $("#maturity_date").text(olddate);
                    $("#maturity_date").val(olddate);
                    swal("Warning", "Maturity Date Can't less than Settlement Date", "warning");
                }
            }
        } else {
            $("#maturity_date").text("");
            $("#maturity_date").val(null);
        }
    });

    $("#deal_period").change(function () {
        if (!document.getElementById("deal_period").readOnly) {
            var word = ["W", "w", "M", "m", "Y", "y"];
            var lastChar = null;
            var indexWord = null;
            var str = $("#deal_period").val();
            if (str.indexOf('W') !== -1 || str.indexOf('w') !== -1 ||
                str.indexOf('M') !== -1 || str.indexOf('m') !== -1 ||
                str.indexOf('Y') !== -1 || str.indexOf('y') !== -1) {
                for (var i = 0; i < word.length; i++) {
                    if (str.indexOf(word[i]) !== -1) {
                        indexWord = str.indexOf(word[i]);
                        lastChar = str[indexWord];
                        break;
                    }
                }
                if (indexWord === 0) {
                    $("#deal_period").val("1" + lastChar);
                    deal_period = 1;
                } else {
                    $("#deal_period").val(str.substring(0, indexWord + 1));
                    deal_period = str.substring(0, indexWord);
                }
            }

            var period = parseInt($("#deal_period").val());
            if (period < 1) {
                $("#deal_period").val("");
            }

            if ($("#deal_period").val() !== "") {
                if (lastChar !== null) {
                    CheckPeriod(deal_period, lastChar);
                } else {
                    SetMatDateAndPeriod();
                }

            } else {
                $("#ddl_trans_type").find(".selected-data").text("At Call");
                $("#ddl_trans_type").find(".selected-value").val("At Call");
                $("#maturity_date").text("");
                $("#maturity_date").val("");
                if (checkinstrument() !== 'PRP') {
                    getbilatcontractno();
                }
            }
        }
    });

    function ExchRateChange() {
        if ($('#exch_rate').val() !== "") {

            console.log("ExchRateChange");

            var exch_rate = FormatDecimal($('#exch_rate').val().replace(/,/g, ''), 8);

            //$("#exch_rate").val(exch_rate);
            //$("#exch_rate").text(exch_rate);

            $('#x-table-data tbody tr').each(function (i, row) {
                var checkrow = $('#x-table-data tbody tr').children().eq(4).length;
                if (checkrow > 0) {
                    CalBond('ColateralList[' + i + '].CASH_AMOUNT', false);
                } else {
                    oldExchRate = exch_rate;
                }
            });

        } else {
            swal("Warning", "Exchange Rate field is required.", "warning");
            $("#exch_rate").val(oldExchRate);
            $("#exch_rate").text(oldExchRate);
        }
    }

    $("#exch_rate").change(function () {
        ExchRateChange();
    });

    $("#interest_rate").on("change focusout", function () {

        if (!document.getElementById("interest_rate").readOnly && $('#interest_rate').val() !== "") {

            var spread = $('#interest_spread').val() == "" ? parseFloat("0.00000000") : parseFloat($('#interest_spread').val().replace(/,/g, ''));
            var interestrate = parseFloat($('#interest_rate').val().replace(/,/g, ''));

            if (isNaN(interestrate)) {
                swal("Warning", "Interest rate invalid format , please click OK to continue and reset to 0 ", "warning");
                interestrate = 0.00000000;
                $('#interest_spread').val('0.00000000');
            }

            if (interestrate < 0) {
                swal("Warning", "Negative Interest rate, please click OK to continue", "warning");
            }

            if (interestrate > 100) {
                swal("Warning", "Interest rate is more than 100 , please click OK to continue", "warning");
            }

            $('#interest_rate').val(FormatDecimal(interestrate, 8));
            $("#interest_rate").text(FormatDecimal(interestrate, 8));

            var totalrate = parseFloat(interestrate) + parseFloat(spread);

            $("#interest_total").val(FormatDecimal(totalrate, 8));
            $("#interest_total").text(FormatDecimal(totalrate, 8));

            CallPrice();

            $('#x-table-data tbody tr').each(function (i, row) {
                var checkrow = $('#x-table-data tbody tr').children().eq(4).length;
                if (checkrow > 0) {
                    CalBond('ColateralList[' + i + '].CASH_AMOUNT', false);
                }
            });

        } else {
            $("#interest_total").val(null);
            $("#interest_total").text("");
        }
    });

    $("#interest_spread").on("change focusout", function () {

        if (!document.getElementById("interest_spread").readOnly) {
            var interestrate = 0;
            var spread = 0;

            interestrate = $('#interest_rate').val() == "" ? parseFloat("0.00000000") : parseFloat($('#interest_rate').val().replace(/,/g, ''));

            if (isNaN(interestrate)) {
                swal("Warning", "Interest Rate invalid format and reset to 0 ", "warning");
                interestrate = 0.00000000;
                $('#interest_rate').val('0.00000000');
            }

            if ($('#interest_spread').val() != "") {

                spread = $('#interest_spread').val() == "" ? FormatDecimal("0.00000000", 8) : parseFloat($('#interest_spread').val().replace(/,/g, ''));

                if (isNaN(spread)) {
                    swal("Warning", "Interest spread invalid format , please click OK to continue and reset to 0 ", "warning");
                    spread = 0.00000000;
                    $('#interest_spread').val('0.00000000');
                }

                if (spread < 0.00000000) {
                    swal("Warning", "Negative Interest spread , please click OK to continue", "warning");
                }

                if (spread > 100.00000000) {
                    swal("Warning", "Interest spread is more than 100 , please click OK to continue", "warning");
                }

                $('#interest_spread').val(FormatDecimal(spread, 8));
                $("#interest_spread").text(FormatDecimal(spread, 8));

            }

            var totalrate = parseFloat(interestrate) + parseFloat(spread);

            $("#interest_total").val(FormatDecimal(totalrate, 8));
            $("#interest_total").text(FormatDecimal(totalrate, 8));

            CallPrice();

            $('#x-table-data tbody tr').each(function (i, row) {
                var checkrow = $('#x-table-data tbody tr').children().eq(4).length;
                if (checkrow > 0) {
                    CalBond('ColateralList[' + i + '].CASH_AMOUNT', false);
                }
            });
        }
    });

    $("#cost_of_fund").on("change focusout", function () {

        if (!document.getElementById("cost_of_fund").readOnly && $('#cost_of_fund').val() !== "") {

            var costoffund = parseFloat($('#cost_of_fund').val().replace(/,/g, ''));
            var spread = $('#cost_spread').val() === "" ? parseFloat("0.00000000") : parseFloat($('#cost_spread').val().replace(/,/g, ''));

            if (isNaN(costoffund)) {
                swal("Warning", "Cost of Fund invalid format , please click OK to continue and reset to 0  ", "warning");
                costoffund = 0.00000000;
                $('#cost_of_fund').val('0.00000000');
            }

            if (costoffund < 0.00000000) {
                swal("Warning", "Negative Cost of Fund , please click OK to continue", "warning");
            }

            if (costoffund > 100.00000000) {
                swal("Warning", "Cost of Fund is more than 100 , please click OK to continue", "warning");
            }

            $('#cost_of_fund').val(FormatDecimal(costoffund, 8));
            $("#cost_of_fund").text(FormatDecimal(costoffund, 8));

            var totalrate = parseFloat(costoffund) + parseFloat(spread);
            $('#cost_total').val(FormatDecimal(totalrate, 8));
            $("#cost_total").text(FormatDecimal(totalrate, 8));

        } else {
            $('#cost_total').val(null);
            $("#cost_total").text("");
        }
    });

    $("#cost_spread").on("change focusout", function () {

        if (!document.getElementById("cost_spread").readOnly) {
            var costoffund = 0;
            var spread = 0;

            costoffund = $('#cost_of_fund').val() == "" ? parseFloat("0.00000000") : parseFloat($('#cost_of_fund').val().replace(/,/g, ''));

            if (isNaN(costoffund)) {
                costoffund = 0.00000000;
                $('#cost_of_fund').val('0.00000000');
            }

            if ($('#cost_spread').val() !== "") {

                spread = $('#cost_spread').val() == "" ? FormatDecimal("0.00000000", 8) : parseFloat($('#cost_spread').val().replace(/,/g, ''));

                if (isNaN(spread)) {
                    swal("Warning", "Cost Spread invalid format , please click OK to continue and reset to 0 ", "warning");
                    spread = 0.00000000;
                    $('#cost_spread').val('0.00000000');
                }

                if (spread < 0.00000000) {
                    swal("Warning", "Negative Cost Spread , please click OK to continue", "warning");
                }

                if (spread > 100.00000000) {
                    swal("Warning", "Cost Spread is more than 100 , please click OK to continue", "warning");
                }

                $('#cost_spread').val(FormatDecimal(spread, 8));
                $("#cost_spread").text(FormatDecimal(spread, 8));

            }

            var totalrate = parseFloat(costoffund) + parseFloat(spread);

            $("#cost_total").val(FormatDecimal(totalrate, 8));
            $("#cost_total").text(FormatDecimal(totalrate, 8));

            $("#lbl_cost_total").find(".costotal").val(FormatDecimal(totalrate, 8));
            $("#lbl_cost_total").find(".costotal").text(FormatDecimal(totalrate, 8));

        }
    });

    $("#purchase_price").change(function () {
        if (!document.getElementById("purchase_price").readOnly && $('#purchase_price').val() != "") {

            var purchasetext = $('#purchase_price').val().replace(/,/g, '');
            var purchase = checkAvalibleAmount(purchasetext);

            if (purchasetext !== "" && isNumeric(purchase)) {
                $('#purchase_price').val(FormatDecimal(purchase, 2));
                if (purchase > 9999999999999) {
                    swal("warning", "Length of Purchase Price is over, System will auto set ", "warning");
                    purchase = FormatDecimal("9999999999999", 2);
                    $('#purchase_price').val(purchase);
                }

                CallPrice();

                if ($("#ismanual_cal").val().toUpperCase() == "FALSE") {
                    checkpurchaseprice();
                }
            } else {
                $('#purchase_price').val("");
            }
        }
    });

    $("#bilateral_contract_no").change(function () {

        if (!document.getElementById("bilateral_contract_no").readOnly && $('#bilateral_contract_no').val() != "") {
            var bilateralconno = $('#bilateral_contract_no').val();
            updateColIsinCodeBOTN(bilateralconno);
        }
    });


    // Fixed issue event change trigger for IE //

    $("#interest_rate").keyup(function () {

        if ($('#interest_rate').val() === "") {
            $("#interest_total").val(null);
            $("#interest_total").text("");
        }
    });

    $("#cost_of_fund").keyup(function () {

        if ($('#cost_of_fund').val() === "") {
            $("#cost_total").val(null);
            $("#cost_total").text("");
        }
    });


    //Start Append
    $("#ddl_append").click(function () {
        var txt_search = $('#txt_append');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null, true, "No");
        txt_search.val("");
    });

    $('#txt_append').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null, false);
    });
    //End Append

    //RP Price Date Dropdown
    $("#ddl_rp_pricedate").click(function () {
        var txt_search = $('#txt_rp_pricedate');
        var price_source = $("#rp_source_value").val();
        var tradedate = $("#trade_date").val();
        var settlementdate = $("#settlement_date").val();
        var cur = $("#cur_pair2").val(); // currency at exchange rate
        tradedate = tradedate.split("/")[2] + "" + tradedate.split("/")[1] + "" + tradedate.split("/")[0];
        settlementdate = settlementdate.split("/")[2] + "" + settlementdate.split("/")[1] + "" + settlementdate.split("/")[0];

        var data = {
            pricesource: price_source,
            tradedate: tradedate,
            settlementdate: settlementdate,
            curr: cur
        };

        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    //$("#ul_rp_pricedate").on("click", ".searchterm", function (event) {
    //    if ($("#rp_price_date_value").val() != "") {
    //        $("#ddl_rp_pricedate").prop('disabled', true);
    //    }
    //});

    $('#txt_rp_pricedate').keyup(function () {
        var data = {
            datastr: this.value
        };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End RP Price Date Dropdown

    //Formula
    //RP Source Dropdown
    $("#ddl_formula").click(function () {
        var txt_search = $('#txt_formula');
        var cur_pair2 = $("#cur_pair1").val();
        var counterpartyid = $("#counter_party_id").val();
        var data = {
            curpair2: cur_pair2,
            counterpartyid: counterpartyid
        };
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });

    $("#ul_formula").on("click", ".searchterm", function (event) {
        $('#x-table-data tbody tr').each(function (i, row) {
            var checkrow = $('#x-table-data tbody tr').children().eq(4).length;
            if (checkrow > 0) {
                CalBond('ColateralList[' + i + '].formula', false);
            }
        });
    });
    //End Formula

    //RP Source Dropdown
    $("#ddl_rp_source").click(function () {
        var txt_search = $('#txt_rp_source');
        var data = {
            datastr: null
        };
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });

    var checkrpsource;
    $("#ul_rp_source").on("click", ".searchterm", function (event) {

        var currentrpsource = $("#rp_source_value").val();

        if (window.checkrpsource != currentrpsource) {
            $("#ddl_rp_pricedate").find(".selected-data").text("Select...");
            $("#ddl_rp_pricedate").find(".selected-data").val("");
            $("#ddl_rp_pricedate").find(".selected-value").val("");
            $("#ddl_rp_pricedate").prop('disabled', false);
        }

        window.checkrpsource = $("#rp_source_value").val();

        GetRPpricedate();
    });

    $('#txt_rp_source').keyup(function () {
        var data = {
            datastr: this.value
        };
        GM.Utility.DDLAutoComplete(this, data, null, false);
    });
    //End RP Source Dropdown

    //Title Name Dropdown
    $("#ddl_port").click(function () {
        var txt_search = $('#txt_port');
        var data = {
            datastr: null
        };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_port').keyup(function () {
        var data = {
            datastr: this.value
        };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Title Name Dropdown

    //Desk Group Dropdown
    $("#ddl_desk").click(function () {
        var instrument = checkinstrument();
        var txt_search = $('#txt_desk');
        var userid = $('#userid').text();
        var port = $('#port').val();

        var data = {
            userid: userid,
            port: port,
            instrument: instrument
        };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_desk').keyup(function () {
        var data = {
            datastr: this.value
        };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Desk Group Dropdown

    //Dealer Dropdown
    $("#ddl_dealer").click(function () {
        var txt_search = $('#txt_dealer');
        var data = {
            datastr: null,
            port: $("#port").val()
        };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_dealer').keyup(function () {
        var data = {
            datastr: this.value,
            port: $("#port").val()
        };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Dealer Dropdown

    //Instrument Dropdown
    $("#ddl_instrument").click(function () {
        var txt_search = $('#txt_instrument');
        var data = {
            datastr: null
        };
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });

    $("#ul_instrument").on("click", ".searchterm", function (event) {

        if ($('#counter_party_id').val() !== "") {
            var transdealtype = $("#ddl_instrument").find(".selected-value").val();
            var instrument = getinstrumenttypeselect();
            var counterpartyname = $('#counter_party_id').val();
            var data = {
                datastr: counterpartyname,
                instrumentcode: instrument,
                dealtype: transdealtype,
                byId: true
            };
            $.ajax({
                url: "/RPDealEntry/FillCounterparty",
                type: "GET",
                dataType: "JSON",
                data: data,
                success: function (res) {
                    if (res.length > 0) {
                        $("#lbl_wht_tax").find(".selected-data").val(FormatDecimal(res[0].Value2, 8));
                        if (res[0].Value3 == "") {
                            swal({
                                title: "Warning",
                                text: "This Counter Party is no signed GMRA yet,<br> Not allow for doing a transaction !",
                                html: true,
                                type: "warning"
                            });

                            ClearCounterparty();
                            return;
                        }

                        var threshold = res[0].Value4 === "" ? "-" : parseFloat(res[0].Value4);

                        $("#lbl_threshold").find(".selected-data").text(threshold);
                        $("#lbl_threshold").find(".selected-data").val(threshold);

                        GetAbsorb(counterpartyname, res[0].Value2);
                        CallPrice();

                        if (checkinstrument() !== 'PRP') {
                            getbilatcontractno();
                        }

                        GetCounterPartyFund();
                        GetFormula();
                        GetPaymentMethod();
                        setTimeout(
                            function () {
                                $('#x-table-data tbody tr').each(function (i, row) {
                                    var checkrow = $('#x-table-data tbody tr').children().eq(4).length;
                                    if (checkrow > 0) {
                                        CalBond('ColateralList[' + i + '].CASH_AMOUNT', false);
                                    }
                                });
                            }, 500);
                    }
                }
            });
        }
    });

    $('#txt_instrument').keyup(function () {
        var data = {
            datastr: this.value
        };
        GM.Utility.DDLAutoComplete(this, data, null, false);
    });
    //End Instrument Dropdown

    var trans_type_select;
    //Tran Type Dropdown
    $("#ddl_trans_type").click(function () {
        var txt_search = $('#txt_trans_type');
        var data = {
            datastr: null
        };
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });

    $("#ul_trans_type").on("click", ".searchterm", function (event) {
        if (trans_type_select != $("#trans_type").val()) {
            var settledate = $("#settlement_date").text();
            var formatmmddyyyydate = settledate.split("/");
            formatmmddyyyydate = formatmmddyyyydate[1] + "/" + formatmmddyyyydate[0] + "/" + formatmmddyyyydate[2];
            settledate = new Date(formatmmddyyyydate);
            trans_type_select = $("#trans_type").val();
            if (trans_type_select.trim() == "Overnight") {
                settledate.setDate(settledate.getDate() + 1);
                $("#maturity_date").text(moment(settledate).format('DD/MM/YYYY'));
                $("#maturity_date").val(moment(settledate).format('DD/MM/YYYY'));
                $("#deal_period").text("1");
                $("#deal_period").val("1");
                CheckHolidayDate();
            } else if (trans_type_select == "At Call") {
                $("#maturity_date").text("");
                $("#maturity_date").val(null);
                $("#deal_period").text("");
                $("#deal_period").val("");
            } else if (trans_type_select == "Term" && $("#deal_period").val() < 2) {
                settledate.setDate(settledate.getDate() + 2);
                $("#maturity_date").text(moment(settledate).format('DD/MM/YYYY'));
                $("#maturity_date").val(moment(settledate).format('DD/MM/YYYY'));

                $("#deal_period").text("2");
                $("#deal_period").val("2");
                CheckHolidayDate();
            }

            CallPrice();
            if (checkinstrument() != 'PRP') {
                getbilatcontractno();
            }
        }
    });

    $('#txt_trans_type').keyup(function () {
        var data = {
            datastr: this.value
        };
        GM.Utility.DDLAutoComplete(this, data, null, false);
    });
    //End Tran Type Dropdown

    //Purpose Dropdown
    //var checkpurpose;
    $("#purpose").click(function () {
        var txt_search = $('#txt_purpose');
        var data = {
            datastr: null
        };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#ddl_purpose").click(function () {
        var txt_search = $('#txt_purpose');
        var data = {
            datastr: null
        };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_purpose').keyup(function () {
        var data = {
            datastr: this.value
        };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Purpose Dropdown

    //Trans Int Term Dropdown
    $("#ddl_transinterm").click(function () {
        var txt_search = $('#txt_transinterm');
        var data = {
            datastr: null
        };
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });

    $('#txt_transinterm').keyup(function () {
        var data = {
            datastr: this.value
        };
        GM.Utility.DDLAutoComplete(this, data, null, false);
    });
    //End Trans Int Term Dropdown

    //Cur Dropdown
    var checkcur;
    $("#ddl_cur").click(function () {
        var txt_search = $('#txt_cur');
        var data = {
            datastr: null
        };
        checkcur = $('#cur').val();
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });

    $("#ul_cur").on("click", ".searchterm", function (event) {
        if (checkcur !== $('#cur').val()) {
            $("#ddl_cur_pair1").find(".selected-data").text($('#cur').val());
            $("#cur_pair1").val($('#cur').val());
            CallPrice();

            $("#ddl_intrate").find(".selected-data").text("Select...");
            $("#ddl_intrate").find(".selected-data").val("");
            $("#ddl_intrate").find(".selected-value").val("");

            $("#ddl_costoffund").find(".selected-data").text("Select...");
            $("#ddl_costoffund").find(".selected-data").val("");
            $("#ddl_costoffund").find(".selected-value").val("");

            SumInterestRate();
            SumCostOfFund();

            SetMatDateAndPeriod();

            if (checkinstrument() !== 'PRP') {
                getbilatcontractno();
            }

            if ($('#cur_pair1').val() === $('#cur_pair2').val()) {
                $('#exch_rate').val("1.00000000");
                ExchRateChange();
            }
        }

        if ($('#cur').val() === "THB") {
            $("#ddl_basiscode").find(".selected-data").text("ACT/365");
            $('#basis_code_name').val("ACT/365");
            $('#basis_code').val(2);

            $("#ddl_transinterm").find(".selected-data").text("At Maturity");
            $('#trans_in_term_id_name').val("At Maturity");
            $('#trans_in_term_id').val(1);

            $("#ddl_rp_source").find(".selected-data").text("TBMA");
            $('#rp_source_text').val("TBMA");
            $('#rp_source_value').val("TBMA");

            window.checkrpsource = $("#rp_source_value").val();

            GetRPpricedate();

            if (getcosttype() === 'FIXED') {
                if (tmpCostOfFund == null) {
                    tmpCostOfFund = 0;
                }
                $('#cost_of_fund').val(FormatDecimal(tmpCostOfFund, 8));
                $('#cost_total').val(FormatDecimal(tmpCostOfFund, 8));
            }
        } else if ($('#cur').val() == "USD") {
            $("#ddl_basiscode").find(".selected-data").text("ACT/360");
            $('#basis_code_name').val("ACT/360");
            $('#basis_code').val(1);
        }

        GetFormula();
        GetPaymentMethod();
    });

    $('#txt_cur').keyup(function () {
        var data = {
            datastr: this.value
        };
        GM.Utility.DDLAutoComplete(this, data, null, false);
    });
    //End Cur Dropdown

    //Cur Dropdown
    var checkcurpair1;
    $("#ddl_cur_pair1").click(function () {
        var txt_search = $('#txt_cur_pair1');
        var data = {
            datastr: null
        };
        checkcurpair1 = $('#cur_pair1').val();
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });

    $("#ul_cur_pair1").on("click", ".searchterm", function (event) {

        $("#ddl_cur").find(".selected-data").text($('#cur_pair1').val());
        $("#cur").val($('#cur_pair1').val());
        if (checkcurpair1 !== $('#cur_pair1').val()) {
            CallPrice();

            $("#ddl_intrate").find(".selected-data").text("Select...");
            $("#ddl_intrate").find(".selected-data").val("");
            $("#ddl_intrate").find(".selected-value").val("");

            if ($('#cur_pair1').val() === $('#cur_pair2').val()) {
                $('#exch_rate').val("1.00000000");
                ExchRateChange();
            }
        }

        if ($('#cur_pair1').val() === "THB") {
            $("#ddl_basiscode").find(".selected-data").text("ACT/365");
            $('#basis_code_name').val("ACT/365");
            $('#basis_code').val(2);

            $("#ddl_transinterm").find(".selected-data").text("At Maturity");
            $('#trans_in_term_id_name').val("At Maturity");
            $('#trans_in_term_id').val(1);

            if (getcosttype() == 'FIXED') {
                $('#cost_of_fund').val(FormatDecimal(tmpCostOfFund, 8));
                $('#cost_total').val(FormatDecimal(tmpCostOfFund, 8));
            }

        } else if ($('#cur_pair1').val() == "USD") {
            $("#ddl_basiscode").find(".selected-data").text("ACT/360");
            $('#basis_code_name').val("ACT/360");
            $('#basis_code').val(1);
        }

        GetFormula();
        GetPaymentMethod();
    });

    $('#txt_cur_pair1').keyup(function () {
        var data = {
            datastr: this.value
        };
        GM.Utility.DDLAutoComplete(this, data, null, false);
    });
    //End Cur Dropdown

    //Cur Dropdown
    var checkcurpair2;
    $("#ddl_cur_pair2").click(function () {
        var txt_search = $('#txt_cur_pair2');
        var data = {
            datastr: null
        };
        checkcurpair2 = $('#cur_pair2').val();
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });


    $("#ul_cur_pair2").on("click", ".searchterm", function (event) {

        if (checkcurpair2 !== $('#cur_pair2').val()) {
            $('#x-table-data tbody tr').each(function (i, row) {
                var currentname = document.getElementsByName("ColateralList[" + i + "].cur");
                if (currentname !== null && currentname.length > 0) {
                    var cur = $('input[name="ColateralList[' + i + '].cur"]');
                    if (cur !== null && cur !== $('#cur_pair2').val() && $('input[name="ColateralList[' + i + '].status"]').val() !== "delete") {
                        swal("Warning", "Currency Not Equal Currency in Bond", "warning");
                        $('#cur_pair2').val(checkcurpair2);
                        $("#ddl_cur_pair2").find(".selected-data").text(checkcurpair2);
                        $("#ddl_cur_pair2").find(".selected-data").val(checkcurpair2);
                        $("#ddl_cur_pair2").find(".selected-value").val(checkcurpair2);
                        return;
                    }
                }
            });


            PreparePriceSource();

            $("#ddl_instrument_code").find(".selected-data").text("Select...");
            $("#instrument_code").val(null);
        }
    });

    $('#txt_cur_pair2').keyup(function () {
        var data = {
            datastr: this.value
        };
        GM.Utility.DDLAutoComplete(this, data, null, false);
    });
    //End Cur Dropdown

    //Counterparty Dropdown
    var checkcounterparty;
    $("#ddl_counterparty").click(function () {
        var txt_search = $('#txt_counterparty');
        var instrument = getinstrumenttypeselect();
        var transdealtype = $("#trans_deal_type").val();
        checkcounterparty = $('#counter_party_id').val();

        if (instrument === "BRP" && $('#cur_pair2').val() !== "THB") {
            GM.Utility.DDLNoData(txt_search);
        } else {
            var data = {
                datastr: null,
                instrumentcode: instrument,
                dealtype: transdealtype
            };
            GM.Utility.DDLAutoCompleteSet4Value(txt_search, data, "lbl_wht_tax", "lbl_swift_code", "lbl_threshold");
        }
        txt_search.val("");
    });

    $("#ul_counterparty").on("click", ".searchterm", function (event) {
        if (checkcounterparty !== $('#counter_party_id').val()) {

            checkcounterparty = $('#counter_party_id').val();
            ClearCounterpartyDetail();

            if ($('#counter_party_id').val() !== "") {

                var transdealtype = $("#ddl_instrument").find(".selected-value").val();
                var instrument = getinstrumenttypeselect();
                var counterpartyname = $('#counter_party_id').val();
                var data = {
                    datastr: counterpartyname,
                    instrumentcode: instrument,
                    dealtype: transdealtype,
                    byId: true
                };
                $.ajax({
                    url: "/RPDealEntry/FillCounterparty",
                    type: "GET",
                    dataType: "JSON",
                    data: data,
                    success: function (res) {

                        if (res.length > 0) {

                            $("#lbl_wht_tax").find(".selected-data").val(FormatDecimal(res[0].Value2, 8));
                            if (res[0].Value3 == "") {

                                swal({
                                    title: "Warning",
                                    text: "This Counter Party is no signed GMRA yet,<br> Not allow for doing a transaction !",
                                    html: true,
                                    type: "warning"
                                });
                                ClearCounterparty();
                                return;
                            }

                            var threshold = res[0].Value4 === "" ? "-" : parseFloat(res[0].Value4);

                            $("#lbl_threshold").find(".selected-data").text(threshold);
                            $("#lbl_threshold").find(".selected-data").val(threshold);

                            GetAbsorb(counterpartyname, res[0].Value2);
                            CallPrice();

                            if (checkinstrument() != 'PRP') {
                                getbilatcontractno();
                            }

                            GetCounterPartyFund();
                            GetFormula();
                            GetPaymentMethod();
                            setTimeout(
                                function () {
                                    $('#x-table-data tbody tr').each(function (i, row) {
                                        var checkrow = $('#x-table-data tbody tr').children().eq(4).length;
                                        if (checkrow > 0) {
                                            CalBond('ColateralList[' + i + '].CASH_AMOUNT', false);
                                        }
                                    });
                                }, 500);
                        }
                    }
                });
            } else {
                $("#withholding_amount_text").text("0.00");
            }
        }
    });

    $('#txt_counterparty').keyup(function () {
        var instrument = getinstrumenttypeselect();
        var transdealtype = $("#trans_deal_type").val();
        var data = {
            datastr: this.value,
            instrumentcode: instrument,
            dealtype: transdealtype
        };
        GM.Utility.DDLAutoCompleteSet4Value(this, data, "lbl_wht_tax", "lbl_swift_code", "lbl_threshold");
    });
    //End Counterparty Dropdown

    //CounterpartyFund Dropdown
    $("#ddl_counterparty_fund").click(function () {
        var txt_search = $('#txt_counterparty_fund');
        var counterparty_id = $("#counter_party_id").val();

        if (getinstrumenttypeselect() === "BRP" && $('#cur_pair2').val() !== "THB") {
            GM.Utility.DDLNoData(txt_search);
        } else {
            var data = {
                counterpartyid: counterparty_id
            };
            GM.Utility.DDLAutoComplete(txt_search, data, null);
        }
        txt_search.val("");
    });
    //End CounterpartyFund Dropdown

    //CounterpartyPayment Dropdown
    $("#ddl_counterparty_payment").click(function () {

        var txt_search = $('#txt_counterparty_payment');
        var counterparty_id = $("#counter_party_id").val();
        if (counterparty_id !== '') {
            var payment_flag = null;
            if ($("#trans_deal_type").val() === localStorage.getItem('TRANS_DEAL_TYPE_BORROWING')) {
                payment_flag = 1; //sell
            } else {
                payment_flag = 2; //buy
            }

            var data = {
                counterpartyid: counterparty_id,
                payment_flag: payment_flag
            };
            GM.Utility.DDLAutoComplete(txt_search, data, null);
        } else {
            GM.Utility.DDLNoData(txt_search);
        }

        txt_search.val("");
    });

    $("#ul_counterparty_payment").on("click", ".searchterm", function (event) {
        $("#ddl_trans_mt_code").find(".selected-data").text("Select...");


    });

    //End CounterpartyPayment Dropdown

    //Margin Payment Dropdown
    $("#ddl_margin_payment").click(function () {
        var txt_search = $('#txt_margin_payment');
        var data = {
            datastr: null
        };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    //End Margin Payment Dropdown

    //InstrumentCode Dropdown
    var checkinstrumentcode;
    $("#ddl_instrument_code").click(function () {
        var txt_search = $('#txt_instrument_code');
        var cur_pair2 = $('#cur_pair2').val();
        checkinstrumentcode = $('#instrument_code').val();
        var repo_deal_type = $('.radio input[id=repo_deal_type]:checked').val();
        var data = {
            instrument: null,
            cur_pair2: cur_pair2,
            repo_deal_type: repo_deal_type
        };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#ul_instrument_code").on("click", ".searchterm", function (event) {
        if (checkinstrumentcode !== $('#instrument_code').val()) {
            $("#ddl_isin_code").find(".selected-data").text("Select...");
            $("#isin_code").val(null);
            $("#isin_code_name").text(null);
            $("#isin_code_name").val(null);
        }
    });

    $('#txt_instrument_code').keyup(function (event) {

        var repo_deal_type = $('.radio input[id=repo_deal_type]:checked').val();
        var cur_pair2 = $('#cur_pair2').val();
        var data = {
            instrument: this.value,
            cur_pair2: cur_pair2,
            repo_deal_type: repo_deal_type
        };

        // Enter key //
        if (event.keyCode === 13) {
            event.preventDefault();
            // check data from ddl list
            if ($('#ul_instrument_code li').size() > 1) {
                $('#ul_instrument_code').find('li').find('a')[1].click();
            } else {
                console.log("no result data.");
            }
        } else {
            GM.Utility.DDLAutoComplete(this, data, null);
        }
    });

    //$('#txt_instrument_code').on("keyup",function (event) {

    //});

    //End InstrumentCode Dropdown

    //isincode Dropdown
    var checkisincode;
    $("#ddl_isin_code").click(function () {
        var txt_search = $('#txt_isin_code');
        checkisincode = $('#isin_code').val();
        var cur = $('#cur_pair2').val();
        var data = {
            isincode: null,
            instrumentcode: $('#instrument_code').val(),
            cur: cur
        };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#ul_isin_code").on("click", ".searchterm", function (event) {

        if (checkisincode != $('#isin_code').val()) {
            $("#ddl_instrument_code").find(".selected-data").text("Select...");
            $("#instrument_code").val(null);
            $("#instrument_code_name").text(null);
        }
    });

    $('#txt_isin_code').keyup(function () {
        var cur = $('#cur_pair2').val();
        var data = {
            isincode: this.value,
            instrumentcode: $('#instrument_code').val(),
            cur: cur
        };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End isincode Dropdown

    //Basis Code Dropdown
    $("#ddl_basiscode").click(function () {
        var txt_search = $('#txt_basiscode');
        var data = {
            datastr: null
        };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#ul_basiscode").on("click", ".searchterm", function (event) {
        CallPrice();
    });

    $('#txt_basiscode').keyup(function () {

        //if (this.value.length > 0) {
        var data = {
            datastr: this.value
        };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Basis Code Dropdown

    //Remark Dropdown
    $("#ddl_remark").click(function () {
        var txt_search = $('#txt_remark');
        var data = {
            datastr: null
        };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_remark').keyup(function () {

        //if (this.value.length > 0) {
        var data = {
            datastr: this.value
        };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Remark Dropdown

    //Float Rate Int Dropdown
    $("#ddl_intrate").click(function () {
        var txt_search = $('#txt_intrate');
        var cur = $("#cur").val();
        var data = {
            cur: cur
        };

        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#ul_intrate").on("click", ".searchterm", function (event) {
        SumInterestRate();
    });


    //End loat Rate Int Dropdown

    //Float Rate Int Dropdown
    $("#ddl_costoffund").click(function () {
        var txt_search = $('#txt_costoffund');
        var cur = $("#cur").val();
        var data = {
            cur: cur
        };

        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#ul_costoffund").on("click", ".searchterm", function (event) {
        SumCostOfFund();
    });

    //End loat Rate Int Dropdown

    //Trans MT Code Dropdown
    $("#ddl_trans_mt_code").click(function () {
        var txt_search = $('#txt_trans_mt_code');
        var trans_deal_type = $("#trans_deal_type").val();
        var payment_method = $("#payment_method").val();
        var cur = $("#cur").val();
        var data = {
            paymentmethod: payment_method,
            transdealtype: trans_deal_type,
            cur: cur
        };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_trans_mt_code').keyup(function () {
        var trans_deal_type = $("#trans_deal_type").val();
        var payment_method = $("#payment_method").val();
        var cur = $("#cur").val();
        var data = {
            paymentmethod: payment_method,
            transdealtype: trans_deal_type,
            cur: cur
        };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //End Trans MT Code Dropdown

    //margins_mt_code Dropdown
    $("#ddl_margins_mt_code").click(function () {
        var txt_search = $('#txt_margins_mt_code');
        var trans_deal_type = $("#trans_deal_type").val();
        var payment_method = $("#margins_payment_method").val();
        var data = {
            paymentmethod: payment_method,
            transdealtype: trans_deal_type
        };

        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_margins_mt_code').keyup(function () {
        var trans_deal_type = $("#trans_deal_type").val();
        var payment_method = $("#margins_payment_method").val();
        var data = {
            paymentmethod: payment_method,
            transdealtype: trans_deal_type
        };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //End margins_mt_code Dropdown

    //SpecialCase Dropdown
    $("#ddl_SpecialCase").click(function () {
        var txt_search = $('#txt_SpecialCase');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_SpecialCase').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    $("#ul_SpecialCase").on("click", ".searchterm", function (event) {
        setTimeout(
            function () {
                $('#x-table-data tbody tr').each(function (i, row) {
                    var checkrow = $('#x-table-data tbody tr').children().eq(4).length;
                    if (checkrow > 0) {

                        CalBond('ColateralList[' + i + '].CASH_AMOUNT', false);
                    }
                });
            }, 300);
    });
    //End SpecialCase Dropdown

    $('.radio input[id=repo_deal_type]').change(function () {
        setTimeout(function () {
            $('.spinner').css('display', 'block'); // Open Loading
        }, 50);

        var current = $(this).val();
        var radioprp = $("[id=repo_deal_type][value=PRP]");
        var radiobrp = $("[id=repo_deal_type][value=BRP]");

        if (current === "PRP" && $('#x-table-data').DataTable().rows().data().length > 1) {
            setTimeout(
                function () {
                    swal("Warning", "Private Repo is not support Multi Colleteral", "warning");
                    $('.spinner').css('display', 'none'); // Close Loading
                },
                100);
            radioprp.attr('ischeck', 'false');
            radioprp.removeAttr("checked");
            radioprp.closest('label').removeClass('checked');

            radiobrp.attr('ischeck', 'true');
            radiobrp.attr("checked", "checked");
            radiobrp.closest('label').addClass('checked');
        } else {
            $("#ddl_desk").find(".selected-data").text("Select...");
            $("#desk_book_id").val(null);
            $("#desk_book_name").text(null);

            ClearCounterparty();

            var instrument;
            var userid;
            var port;
            var data;
            var isNotFind;
            var isValidateBornTrans = true;

            console.log("current : " + current);

            if (current === "PRP") {
                $('#x-table-data tbody tr').each(function (i, row) {
                    var instrument_code = $('input[name="ColateralList[' + i + '].instrument_code"]').val();
                    if (instrument_code !== undefined && instrument_code.indexOf("BOTN") !== -1) {
                        swal("Warning", "Instrument: Private Repo Not Match Instrument Code : BOTN ", "warning");
                        isValidateBornTrans = false;
                    }
                });

                if (!isValidateBornTrans) { // reset checked radio instrument //
                    radioprp.attr('ischeck', 'false');
                    radioprp.removeAttr("checked");
                    radioprp.closest('label').removeClass('checked');

                    radiobrp.attr('ischeck', 'true');
                    radiobrp.attr("checked", "checked");
                    radiobrp.closest('label').addClass('checked');
                }
                else {

                    radioprp.attr('ischeck', 'true');
                    radiobrp.attr('ischeck', 'false');
                    radioprp.attr("checked", "checked");
                    radiobrp.removeAttr("checked");

                    $("#bilateral_contract_no").attr("readonly", "true");
                    $("#bilateral_contract_no").val("");
                    $("#bilateral_contract_no").text("");

                    $("#ddl_instrument_code").find(".selected-data").text("Select...");
                    $("#instrument_code").val(null);

                    instrument = checkinstrument();
                    userid = $('#userid').text();
                    port = $('#port').val();
                    data = {
                        userid: userid,
                        port: port,
                        instrument: instrument
                    };
                    isNotFind = true;

                    $.ajax({
                        url: "/RPDealEntry/FillDesk",
                        type: "GET",
                        dataType: "JSON",
                        data: data,
                        success: function (res) {

                            for (var i = 0, len = res.length; i < len; i++) {
                                if (res[i].Text == "PRIV_REPO") {
                                    $("#ddl_desk").find(".selected-data").val(res[i].Text);
                                    $("#ddl_desk").find(".selected-data").text(res[i].Text);
                                    $("#ddl_desk").find(".selected-value").val(res[i].Value);
                                    isNotFind = false;
                                    break;
                                }
                            }

                            if (isNotFind) {
                                $("#ddl_desk").find(".selected-data").val(res[0].Text);
                                $("#ddl_desk").find(".selected-data").text(res[0].Text);
                                $("#ddl_desk").find(".selected-value").val(res[0].Value);
                            }
                        }
                    });

                    data = {
                        datastr: "FINP"
                    };
                    $.ajax({
                        url: "/RPDealEntry/FillPurpose",
                        type: "GET",
                        dataType: "JSON",
                        data: data,
                        success: function (res) {
                            $("#ddl_purpose").find(".selected-data").val(res[0].Text);
                            $("#ddl_purpose").find(".selected-data").text(res[0].Text);
                            $("#ddl_purpose").find(".selected-data").attr("data-toggle", "tooltip");
                            $("#ddl_purpose").find(".selected-data").attr("title", res[0].Text);
                            $("#ddl_purpose").find(".selected-value").val(res[0].Value);
                        }
                    });

                    $("#withholding_amount_text").text("0.00");
                }
                setTimeout(function () {
                    $('.spinner').css('display', 'none'); // Close Loading
                }, 1000);
            }
            else {
                radioprp.attr('ischeck', 'false');
                radiobrp.attr('ischeck', 'true');
                radiobrp.attr("checked", "checked");
                radioprp.removeAttr("checked");

                instrument = checkinstrument();
                userid = $('#userid').text();
                port = $('#port').val();
                data = {
                    userid: userid,
                    port: port,
                    instrument: instrument
                };
                isNotFind = true;
                $.ajax({
                    url: "/RPDealEntry/FillDesk",
                    type: "GET",
                    dataType: "JSON",
                    data: data,
                    success: function (res) {
                        for (var i = 0, len = res.length; i < len; i++) {
                            if (res[i].Text == "BRP2") {
                                $("#ddl_desk").find(".selected-data").val(res[i].Text);
                                $("#ddl_desk").find(".selected-data").text(res[i].Text);
                                $("#ddl_desk").find(".selected-value").val(res[i].Value);
                                isNotFind = false;
                                break;
                            }
                        }

                        if (isNotFind) {
                            $("#ddl_desk").find(".selected-data").val(res[0].Text);
                            $("#ddl_desk").find(".selected-data").text(res[0].Text);
                            $("#ddl_desk").find(".selected-value").val(res[0].Value);
                            $("#ddl_desk").find(".selected-data").attr("data-toggle", "tooltip");
                            $("#ddl_desk").find(".selected-data").attr("title", res[0].Text);
                        }
                    }
                });

                data = {
                    datastr: "FINB"
                };
                $.ajax({
                    url: "/RPDealEntry/FillPurpose",
                    type: "GET",
                    dataType: "JSON",
                    data: data,
                    success: function (res) {
                        $("#ddl_purpose").find(".selected-data").val(res[0].Text);
                        $("#ddl_purpose").find(".selected-data").text(res[0].Text);
                        $("#ddl_purpose").find(".selected-value").val(res[0].Value);
                    }
                });

                if ($('#cur_pair2').val() === "THB") {

                    var transdealtype = $("#trans_deal_type").val();
                    data = {
                        datastr: null,
                        instrumentcode: "BRP",
                        dealtype: transdealtype
                    };

                    $.ajax({
                        url: "/RPDealEntry/FillCounterparty",
                        type: "GET",
                        dataType: "JSON",
                        data: data,
                        success: function (res) {
                            if (res.length > 0) {
                                $("#ddl_counterparty").find(".selected-data").val(res[0].Text);
                                $("#ddl_counterparty").find(".selected-data").text(res[0].Text);
                                $("#ddl_counterparty").find(".selected-value").val(res[0].Value);

                                $("#counter_party_id").val(res[0].Value);

                                $("#lbl_wht_tax").find(".selected-data").val(FormatDecimal(res[0].Value2, 8));

                                var swift_code = res[0].Value3 === "" ? "-" : res[0].Value3;

                                $("#lbl_swift_code").find(".selected-data").text(swift_code);
                                $("#lbl_swift_code").find(".selected-data").val(swift_code);

                                var threshold = res[0].Value4 === "" ? "-" : parseFloat(res[0].Value4);

                                $("#lbl_threshold").find(".selected-data").text(threshold);
                                $("#lbl_threshold").find(".selected-data").val(threshold);

                                GetAbsorb(res[0].Value, res[0].Value2);
                                GetCounterPartyFund();
                                GetFormula();
                                GetPaymentMethod();

                                CallPrice();

                                setTimeout(
                                    function () {
                                        $('#x-table-data tbody tr').each(function (i, row) {
                                            var checkrow = $('#x-table-data tbody tr').children().eq(4).length;
                                            if (checkrow > 0) {
                                                CalBond('ColateralList[' + i + '].CASH_AMOUNT', false);
                                            }
                                        });
                                    }, 500);
                            }
                        }
                    });
                } else {

                    ClearCounterparty();
                }

                getbilatcontractno();

                setTimeout(function () {
                    $('.spinner').css('display', 'none'); // Close Loading
                }, 500);
            }

            PreparePriceSource();
        }
    });

    $('.radio input[id=interest_type]').change(function () {
        var current = $(this).val();
        var radiofix = $("[id=interest_type][value=FIXED]");
        var radioflo = $("[id=interest_type][value=FLOAT]");
        clearinterest();
        if (current == "FIXED") {
            radiofix.attr('ischeck', 'true');
            radioflo.attr('ischeck', 'false');
            radiofix.attr("checked", "checked");
            $("#intratetext").attr("class", "input-group no-btn");
            $("#intrateddl").attr("class", "dropdown hidden");
            radioflo.removeAttr("checked");
            $("#interest_rate").removeAttr("readonly");
            $("#interest_rate").parent().find("span").removeAttr("readonly");
            $("#interest_spread").attr("readonly", "true");
            $("#interest_spread").parent().find("span").attr("readonly", "true");

            if (tmp_interest_rate !== null) {
                $("#interest_rate").val(tmp_interest_rate);
                $("#interest_total").val(tmp_interest_rate);
            }
        } else {
            $("#intrateddl").attr("class", "dropdown");
            $("#intratetext").attr("class", "input-group no-btn hidden");
            radiofix.attr('ischeck', 'false');
            radioflo.attr('ischeck', 'true');
            radioflo.attr("checked", "checked");
            radiofix.removeAttr("checked");
            $("#interest_rate").attr("readonly", "true");
            $("#interest_rate").parent().find("span").attr("readonly", "true");
            $("#interest_spread").removeAttr("readonly");
            $("#interest_spread").parent().find("span").removeAttr("readonly");

            $("#interest_spread").val('0.00000000');
            $("#interest_total").val('0.00000000');


            FillFloatRate();

        }

        CallPrice();
        if (checkinstrument() !== 'PRP') {
            getbilatcontractno();
        }
    });

    $('.radio input[id=cost_type]').change(function () {
        var current = $(this).val();
        var radiofix = $("[id=cost_type][value=FIXED]");
        var radioflo = $("[id=cost_type][value=FLOAT]");
        clearcost();
        if (current == "FIXED") {
            radiofix.attr('ischeck', 'true');
            radioflo.attr('ischeck', 'false');
            radiofix.attr("checked", "checked");
            radioflo.removeAttr("checked");
            $("#costoffundtext").attr("class", "input-group no-btn");
            $("#costoffundddl").attr("class", "dropdown hidden");
            $("#cost_of_fund").removeAttr("readonly");
            $("#cost_of_fund").parent().find("span").removeAttr("readonly");
            $("#cost_spread").attr("readonly", "true");
            $("#cost_spread").parent().find("span").attr("readonly", "true");
            if (tmp_cost_of_fund) {
                $("#cost_of_fund").val(tmp_cost_of_fund);
                $("#cost_total").val(tmp_cost_of_fund);
            }
        } else {
            $("#costoffundddl").attr("class", "dropdown");
            $("#costoffundtext").attr("class", "input-group no-btn hidden");
            radiofix.attr('ischeck', 'false');
            radioflo.attr('ischeck', 'true');
            radioflo.attr("checked", "checked");
            radiofix.removeAttr("checked");
            $("#cost_of_fund").attr("readonly", "true");
            $("#cost_of_fund").parent().find("span").attr("readonly", "true");
            $("#cost_spread").removeAttr("readonly");
            $("#cost_spread").parent().find("span").removeAttr("readonly");

            $("#cost_spread").val('0.00000000');
            $("#cost_total").val('0.00000000');

            var data = {
                cur: $("#cur").val(),
                date: $("#settlement_date").val()
            };


            $.ajax({
                url: "/RPDealEntry/FillFloatRate",
                type: "GET",
                dataType: "JSON",
                data: data,
                success: function (res) {

                    let text = 'Select...';
                    let value = 0;

                    for (var i = 0; i < res.length; i++) {
                        if (res[i].Text === 'POLICY RATE') {
                            text = res[i].Text;
                            value = res[i].Value;
                        }
                    }

                    $("#ddl_costoffund").find(".selected-data").val(text);
                    $("#ddl_costoffund").find(".selected-data").text(text);
                    $("#ddl_costoffund").find(".selected-value").val(value);

                    SumCostOfFund();

                }
            });

        }
    });

    GM.RPDealEntry = {};
    GM.RPDealEntry.Submit = function (btn) {

        if (ValidateInput()) {
            //var limit;
            var messageHC = "";
            //$('#x-table-data tbody tr').each(function (i, row) {
            //    var haircut = $('input[name="ColateralList[' + i + '].haircut"]').val();
            //    var status = $('input[name="ColateralList[' + i + '].status"]').val();
            //    if (status !== "delete" & parseFloat(haircut.replace(/,/g, '')) <= 0.00) {
            //        messageHC = '<li style="text-align:left; color:red;">The amount of Haircut should not be zero.</li>';
            //        return false;
            //    }
            //});

            if (messageHC !== "") {
                setTimeout(function () {
                    swal({
                        title: " Please confirm to continue?",
                        text: messageHC,
                        html: true,
                        type: "warning",
                        showCancelButton: true,
                        confirmButtonClass: "btn-danger",
                        confirmButtonText: "Yes",
                        cancelButtonText: "No",
                        closeOnConfirm: true,
                        closeOnCancel: true
                    },
                        function (isConfirm) {
                            if (isConfirm) {
                                $('.spinner').css('display', 'block'); // Open Loading
                                var dataToPost = $("#search-form").serialize();
                                $.post("CheckLimit", dataToPost)
                                    .done(function (response) {
                                        //if ($('#repo_deal_type').val() === 'BRP' || $('#port_name').val() === 'TRADING') {
                                        //    response.RefCode = 0;
                                        //}

                                        if (response.RefCode !== -1) {
                                            if (response.RefCode === 0) {
                                                messageHC = "";
                                            } else {
                                                messageHC = '<li style="text-align:left; color:red;">' + response.Message + '</li>';
                                            }
                                            setTimeout(function () {
                                                $('.spinner').css('display', 'none'); // Close Loading
                                                swal({
                                                    title: "Comfirm Save?",
                                                    text: messageHC,
                                                    html: true,
                                                    type: "warning",
                                                    showCancelButton: true,
                                                    confirmButtonClass: "btn-danger",
                                                    confirmButtonText: "Yes",
                                                    cancelButtonText: "No",
                                                    closeOnConfirm: true,
                                                    closeOnCancel: true
                                                },
                                                    function (isConfirm) {
                                                        if (isConfirm) {
                                                            $("#isover_limit").val(response.RefCode !== 1 ? false : true);

                                                            //$(":disabled", $('#search-form')).removeAttr("disabled");

                                                            $('.spinner').css('display', 'block'); // Open Loading

                                                            GM.Message.Clear();
                                                            var dataToPost = $("#search-form").serialize();
                                                            $.post("Add", dataToPost)
                                                                .done(function (response) {

                                                                    GM.Unmask();
                                                                    //console.log('$("#isover_limit").val : ' + $("#isover_limit").val());
                                                                    if (response.Success === true) {
                                                                        if ($('#port_name').val() === 'TRADING' && $("#isover_limit").val() === 'false') {

                                                                            $.ajax({
                                                                                url: "/RPDealVerify/Approve_Trans",
                                                                                type: "POST",
                                                                                dataType: "JSON",
                                                                                data: response.Data.RPTransResultModel[0],
                                                                                success: function (res) {
                                                                                    if (res[0].Message === "") {
                                                                                        setTimeout(
                                                                                            function () {
                                                                                                swal({
                                                                                                    title: "Complete",
                                                                                                    text: "Save Successfully <br><p style='font-size:90%;color:#0000e6;'>( Trans No : " + response.Data.RPTransResultModel[0].trans_no + " )</p>",
                                                                                                    type: "success",
                                                                                                    html: true,
                                                                                                    showCancelButton: false,
                                                                                                    confirmButtonClass: "btn-success",
                                                                                                    confirmButtonText: "Ok",
                                                                                                    closeOnConfirm: true
                                                                                                },
                                                                                                    function (isConfirm) {
                                                                                                        $('.spinner').css('display', 'block'); // Open Loading
                                                                                                        window.location.href = "/RPDealEntry/Index";
                                                                                                    });
                                                                                            }, 100);
                                                                                    }
                                                                                    else {

                                                                                        setTimeout(
                                                                                            function () {
                                                                                                swal({
                                                                                                    title: "Fail",
                                                                                                    text: res[0].Message,
                                                                                                    type: "error",
                                                                                                    showCancelButton: false,
                                                                                                    confirmButtonClass: "btn-success",
                                                                                                    confirmButtonText: "Ok",
                                                                                                    closeOnConfirm: true
                                                                                                });
                                                                                                $('.spinner').css('display', 'none'); // Close Loading
                                                                                            },
                                                                                            100);
                                                                                    }
                                                                                }
                                                                            });
                                                                        } else {
                                                                            setTimeout(
                                                                                function () {
                                                                                    swal({
                                                                                        title: "Complete",
                                                                                        text: "Save Successfully <br><p style='font-size:90%;color:#0000e6;'>( Trans No : " + response.Data.RPTransResultModel[0].trans_no + " )</p>",
                                                                                        type: "success",
                                                                                        html: true,
                                                                                        showCancelButton: false,
                                                                                        confirmButtonClass: "btn-success",
                                                                                        confirmButtonText: "Ok",
                                                                                        closeOnConfirm: true
                                                                                    },
                                                                                        function (isConfirm) {
                                                                                            $('.spinner').css('display', 'block'); // Open Loading
                                                                                            window.location.href = "/RPDealEntry/Index";
                                                                                        });
                                                                                }, 100);
                                                                        }
                                                                    } else {

                                                                        setTimeout(
                                                                            function () {
                                                                                swal({
                                                                                    title: "Fail",
                                                                                    text: response.Message,
                                                                                    type: "error",
                                                                                    showCancelButton: false,
                                                                                    confirmButtonClass: "btn-success",
                                                                                    confirmButtonText: "Ok",
                                                                                    closeOnConfirm: true
                                                                                });
                                                                                $('.spinner').css('display', 'none'); // Close Loading
                                                                            },
                                                                            100);
                                                                    }
                                                                });
                                                        } else {
                                                            GM.Message.Clear();
                                                        }
                                                    });
                                            }, 200);
                                        } else {
                                            setTimeout(
                                                function () {
                                                    swal("Fail", response.Message, "error");
                                                    $('.spinner').css('display', 'none'); // Close Loading
                                                }, 100);
                                        }
                                    });
                            }
                        });
                }, 100);
            } else {
                //$('.spinner').css('display', 'block'); // Open Loading
                var dataToPost = $("#search-form").serialize();
                $.post("CheckLimit", dataToPost)
                    .done(function (response) {
                        $('.spinner').css('display', 'none'); // Close Loading
                        if (response.RefCode !== -1) {
                            if (response.RefCode === 0) {
                                messageHC = "";
                            } else {
                                messageHC = '<li style="text-align:left; color:red;">' + response.Message + '</li>';
                            }
                            setTimeout(function () {
                                swal({
                                    title: "Comfirm Save?",
                                    text: messageHC,
                                    html: true,
                                    type: "warning",
                                    showCancelButton: true,
                                    confirmButtonClass: "btn-danger",
                                    confirmButtonText: "Yes",
                                    cancelButtonText: "No",
                                    closeOnConfirm: true,
                                    closeOnCancel: true
                                },
                                    function (isConfirm) {
                                        if (isConfirm) {
                                            $("#isover_limit").val(response.RefCode !== 1 ? false : true);

                                            $('.spinner').css('display', 'block'); // Open Loading
                                            var dataToPost = $("#search-form").serialize();
                                            $.post("Add", dataToPost)
                                                .done(function (response) {
                                                    GM.Unmask();
                                                    if (response.Success === true) {
                                                        if ($('#port_name').val() === 'TRADING' && $("#isover_limit").val() === 'false') {

                                                            $.ajax({
                                                                url: "/RPDealVerify/Approve_Trans",
                                                                type: "POST",
                                                                dataType: "JSON",
                                                                data: response.Data.RPTransResultModel[0],
                                                                success: function (res) {
                                                                    if (res[0].Message === "") {
                                                                        setTimeout(
                                                                            function () {
                                                                                swal({
                                                                                    title: "Complete",
                                                                                    text: "Save Successfully <br><p style='font-size:90%;color:#0000e6;'>( Trans No : " + response.Data.RPTransResultModel[0].trans_no + " )</p>",
                                                                                    type: "success",
                                                                                    html: true,
                                                                                    showCancelButton: false,
                                                                                    confirmButtonClass: "btn-success",
                                                                                    confirmButtonText: "Ok",
                                                                                    closeOnConfirm: true
                                                                                },
                                                                                    function (isConfirm) {
                                                                                        $('.spinner').css('display', 'block'); // Open Loading
                                                                                        window.location.href = "/RPDealEntry/Index";
                                                                                    });
                                                                            }, 100);
                                                                    }
                                                                    else {

                                                                        setTimeout(
                                                                            function () {
                                                                                swal({
                                                                                    title: "Fail",
                                                                                    text: res[0].Message,
                                                                                    type: "error",
                                                                                    showCancelButton: false,
                                                                                    confirmButtonClass: "btn-success",
                                                                                    confirmButtonText: "Ok",
                                                                                    closeOnConfirm: true
                                                                                });
                                                                                $('.spinner').css('display', 'none'); // Close Loading
                                                                            },
                                                                            100);
                                                                    }
                                                                }
                                                            });
                                                        } else {
                                                            setTimeout(
                                                                function () {
                                                                    swal({
                                                                        title: "Complete",
                                                                        text: "Save Successfully <br><p style='font-size:90%;color:#0000e6;'>( Trans No : " + response.Data.RPTransResultModel[0].trans_no + " )</p>",
                                                                        type: "success",
                                                                        html: true,
                                                                        showCancelButton: false,
                                                                        confirmButtonClass: "btn-success",
                                                                        confirmButtonText: "Ok",
                                                                        closeOnConfirm: true
                                                                    },
                                                                        function (isConfirm) {
                                                                            $('.spinner').css('display', 'block'); // Open Loading
                                                                            window.location.href = "/RPDealEntry/Index";
                                                                        });
                                                                }, 100);
                                                        }
                                                    } else {
                                                        setTimeout(
                                                            function () {
                                                                swal({
                                                                    title: "Fail",
                                                                    text: response.Message,
                                                                    type: "error",
                                                                    showCancelButton: false,
                                                                    confirmButtonClass: "btn-success",
                                                                    confirmButtonText: "Ok",
                                                                    closeOnConfirm: true
                                                                });
                                                                $('.spinner').css('display', 'none'); // Close Loading
                                                            },
                                                            100);

                                                    }
                                                });
                                        }
                                    });
                            }, 200);
                        } else {
                            setTimeout(function () {
                                swal("Fail", response.Message, "error");
                                $('.spinner').css('display', 'none'); // Close Loading
                            }, 100);
                        }
                    });
            }
        }
    };

    GM.RPDealEntry.Clear = function () {
        $('.spinner').css('display', 'block');
        window.location.reload(true);
    };
    GM.RPDealEntry.Coll = {};
    GM.RPDealEntry.Coll.Table = $('#x-table-data').DataTable({
        createdRow: function (row, data, index) {
            $('td', row).parent().attr("data-id", index);

            if (data.status === "delete") {
                $('td', row).parent().attr("style", "display:none");
            } else if (data.status === "duplicate") {
                $('#x-table-data').DataTable().row(row).remove();
                swal("Warning", "Duplicate Instrument code,please add new instrument code", "warning");
            } else if (data.status === "NotHave") {
                $('#x-table-data').DataTable().row(row).remove();
                swal("Warning", "Not Have Instrument code,please add new instrument code", "warning");
            } else {
                var action = $('td', row).parent().find("select").data("action");
                var id = $('td', row).parent().find("select").attr("id");

                if (data.port) {
                    GM.RPDealEntry.Coll.ClickCell(action, id, data.port);
                } else {
                    GM.RPDealEntry.Coll.ClickCell(action, id, null);
                }
            }
        },
        dom: 'Bfrtip',
        searching: true,
        scrollY: '21vh',
        scrollX: true,
        bInfo: false,
        bPaginate: false,
        order: [
            [1, "desc"]
        ],
        buttons: [],
        processing: true,
        serverSide: true,
        ordering: false,
        ajax: {
            "url": "/RPDealEntry/AddColl",
            "type": "POST",
            "data": {
                "trans_no": $('#trans_no').val(),
                "sessionName": $('#SessionName').val(),
                "isCopy": $('#isCopy').val()
            }
        },
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        columnDefs: [{
            targets: 0,
            data: "RowNumber",
            orderable: false,
            render: function (data, type, row, meta) {
                return data;
            }
        },
        {
            targets: 1,
            data: "trans_no",
            visible: false,
            render: function (data, type, row, meta) {
                return '<input name="ColateralList[' + meta.row + '].trans_no" Isdropdown="false"  type="text" value="' + data + '">';
            }
        },
        {
            targets: 2,
            data: "instrument_code",
            render: function (data, type, row, meta) {
                var html = '<input name="ColateralList[' + meta.row + '].instrument_code" style="background-color:lightGray;width:130px;" readonly="readonly" Isdropdown="false"  type="text" value="' + data + '">' +
                    '<input name="ColateralList[' + meta.row + '].instrument_id" Isdropdown="false"  type="hidden" value="' + row.instrument_id + '">' +
                    '<input name="ColateralList[' + meta.row + '].colateral_id" Isdropdown="false"  type="hidden" value="' + row.colateral_id + '">' +
                    '<input name="ColateralList[' + meta.row + '].status" Isdropdown="false"  type="hidden" value="' + row.status + '">';
                return html;
            }
        },
        {
            targets: 3,
            data: "isin_code",
            render: function (data, type, row, meta) {
                return '<input name="ColateralList[' + meta.row + '].isin_code" style="background-color:lightGray;width:120px;" readonly="readonly" Isdropdown="false"  type="text" value="' + data + '">';
            }
        },
        {
            targets: 4,
            data: "port",
            render: function (data, type, row, meta) {
                var html = '';
                html = '<input class="selected-data hidden" id="ColateralList[' + meta.row + '].port" name="ColateralList[' + meta.row + '].port" type="text" value="" >' +
                    '<input class="selected-data hidden" id="ColateralList[' + meta.row + '].port_name" name="ColateralList[' + meta.row + '].port_name" type="text" value="" >' +
                    '<select id="ddl_port_colraterral_' + meta.row + '" name="ColateralList[' + meta.row + '].port" data-action="\\RPDealEntry\\FillPortTrans" onchange="Setvalue(this);SetPostBack(this,' + meta.row + ');" style="height: 23px;width: 100px;">' +
                    '<option value="">--- Select ----</option>' +
                    '</select>';
                return html;
            }
        },
        {
            targets: 5,
            data: "repo_deal_type",
            visible: false,
            render: function (data, type, row, meta) {
                return '<input name="ColateralList[' + meta.row + '].repo_deal_type" Isdropdown="false"  type="text" value="' + data + '">';
            }
        },
        {
            targets: 6,
            data: "cur",
            render: function (data, type, row, meta) {
                return '<input name="ColateralList[' + meta.row + '].cur" readonly="readonly" style="background-color:lightGray;width:40px;" Isdropdown="false"  type="text" value="' + data + '">';
            }
        },
        {
            targets: 7,
            data: "ytm",
            render: function (data, type, row, meta) {
                var value = '0.000000';
                var currentname = 'ColateralList[' + meta.row + '].ytm';
                if (data != null) {
                    value = FormatDecimal(data, 6);
                }
                return '<input onkeypress = "return text_OnKeyPress_NumberOnlyAndDotAndMinus(this,\'' + currentname + '\');" name="ColateralList[' + meta.row + '].ytm" onchange="checknumber(\'' + currentname + '\',' + true + ')" style="width:100px;"  type="text" value="' + value + '">';
            }
        },
        {
            targets: 8,
            data: "clean_price",
            render: function (data, type, row, meta) {
                var value = '';
                var currentname = 'ColateralList[' + meta.row + '].clean_price';
                if (data != null) {
                    value = FormatDecimal(data, 6);
                }
                return '<input onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this,\'' + currentname + '\');" name="' + currentname + '" style="width:100px;"  onchange="checknumber(\'' + currentname + '\',' + true + ')"  Isdropdown="false"  type="text" value="' + value + '">';
            }
        },
        {
            targets: 9,
            data: "dirty_price",
            render: function (data, type, row, meta) {
                var value = '';
                var currentname = 'ColateralList[' + meta.row + '].dirty_price';
                if (data != null) {
                    value = FormatDecimal(data, 6);
                }
                return '<input onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this,\'' + currentname + '\');" name="' + currentname + '" onchange="checknumber(\'' + currentname + '\',' + true + ')" style="width:100px;" Isdropdown="false"  type="text" value="' + value + '">';
            }
        },
        {
            targets: 10,
            data: "haircut",
            render: function (data, type, row, meta) {
                var value = '0.00';
                var currentname = 'ColateralList[' + meta.row + '].haircut';
                if (data != null) {
                    value = FormatDecimal(data, 2);
                }
                return '<input onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this,\'' + currentname + '\');" name="' + currentname + '" onchange="checknumber(\'' + currentname + '\',' + true + ')" style="width:50px;" Isdropdown="false"  type="text" value="' + value + '">';
            }
        },
        {
            targets: 11,
            data: "unit",
            render: function (data, type, row, meta) {
                var value = '0';
                var currentname = 'ColateralList[' + meta.row + '].unit';
                if (data != null) {
                    value = FormatDecimal(data, 0);
                }
                return '<input onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this,\'' + currentname + '\');" name="' + currentname + '" onchange="checknumber(\'' + currentname + '\',' + true + ')" Isdropdown="false" style="width:100px;" type="text" value="' + value + '">';
            }
        },
        {
            targets: 12,
            data: "par",
            render: function (data, type, row, meta) {
                var value = '0.00';
                var currentname = 'ColateralList[' + meta.row + '].par';

                if (data != null) {
                    value = FormatDecimal(data, 2);
                }

                return '<input onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this,\'' + currentname + '\');" name="' + currentname + '" onchange="checknumber(\'' + currentname + '\',' + true + ')" Isdropdown="false" style="width:120px;" type="text" value="' + value + '">';
            }
        },
        {
            targets: 13,
            data: "market_value",
            render: function (data, type, row, meta) {
                var value = '0.00';
                var currentname = 'ColateralList[' + meta.row + '].market_value';
                if (data != null) {
                    value = FormatDecimal(data, 2);
                }
                return '<input onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this,\'' + currentname + '\');" name="' + currentname + '" onchange="checknumber(\'' + currentname + '\',' + true + ')" Isdropdown="false" style="width:120px;" type="text" value="' + value + '">';
            }
        },
        {
            targets: 14,
            data: "cash_amount",
            render: function (data, type, row, meta) {
                var value = '0.00';
                var currentname = 'ColateralList[' + meta.row + '].cash_amount';
                if (data != null) {
                    value = FormatDecimal(data, 2);
                }
                return '<input onkeypress = "return text_OnKeyPress_NumberOnlyAndDotAndM(this,\'' + currentname + '\');" name="' + currentname + '" onchange="checknumber(\'' + currentname + '\',' + true + ')" Isdropdown="false" style="width:120px;" type="text" value="' + value + '">';
            }
        },
        {
            targets: 15,
            data: "dirty_price_after_hc",
            render: function (data, type, row, meta) {
                var value = '0.00';
                var currentname = 'ColateralList[' + meta.row + '].dirty_price_after_hc';
                if (data != null) {
                    value = FormatDecimal(data, 2);
                }
                return '<input name="' + currentname + '" readonly="readonly" Isdropdown="false" style="background-color:lightGray;width:120px;" type="text" value="' + value + '">';
            }
        },
        {
            targets: 16,
            data: "interest_amount",
            render: function (data, type, row, meta) {
                var value = '';
                var currentname = 'ColateralList[' + meta.row + '].interest_amount';
                if (data != null) {
                    value = FormatDecimal(data, 2);
                }
                return '<input name="' + currentname + '" readonly="readonly" Isdropdown="false" style="background-color:lightGray;width:120px;" type="text" value="' + value + '">';
            }
        },
        {
            targets: 17,
            data: "wht_amount",
            render: function (data, type, row, meta) {
                var value = '';
                var currentname = 'ColateralList[' + meta.row + '].wht_amount';
                if (data != null) {
                    value = FormatDecimal(data, 2);
                }
                return '<input name="' + currentname + '" readonly="readonly" Isdropdown="false" style="background-color:lightGray;width:120px;" type="text" value="' + value + '">';
            }
        },
        {
            targets: 18,
            data: "temination_value",
            render: function (data, type, row, meta) {
                var value = '';
                var currentname = 'ColateralList[' + meta.row + '].temination_value';
                if (data != null) {
                    value = FormatDecimal(data, 2);
                }
                return '<input name="' + currentname + '" readonly="readonly" Isdropdown="false" style="background-color:lightGray;width:120px;"  type="text" value="' + value + '">';
            }
        },
        {
            targets: 19,
            data: "maturity_date",
            render: function (data, type, row, meta) {
                var value = '';
                var currentname = 'ColateralList[' + meta.row + '].maturity_date';
                if (data != null) {
                    value = moment(data).format('DD/MM/YYYY');
                }
                return '<input name="' + currentname + '" readonly="readonly" style="background-color:lightGray;width:80px;" Isdropdown="false" type="text"' + (value !== '' ? ' value="' + value + '">' : '>');
            }
        },
        {
            targets: 20,
            data: "coupon_rate",
            render: function (data, type, row, meta) {
                var value = '0.000000';
                var currentname = 'ColateralList[' + meta.row + '].coupon_rate';
                if (data != null) {
                    value = FormatDecimal(data, 6);
                }
                return '<input name="' + currentname + '" readonly="readonly" Isdropdown="false" style="background-color:lightGray;width:100px;"  type="text" value="' + value + '">';
            }
        },
        {
            targets: 21,
            data: "par_unit",
            render: function (data, type, row, meta) {
                var value = '';
                var currentname = 'ColateralList[' + meta.row + '].par_unit';
                if (data != null) {
                    value = FormatDecimal(data, 2);
                }
                return '<input name="' + currentname + '" readonly="readonly" Isdropdown="false" style="background-color:lightGray;width:100px;" type="text" value="' + value + '">';
            }
        },
        {
            targets: 22,
            data: "variation",
            render: function (data, type, row, meta) {
                var value = '';
                var currentname = 'ColateralList[' + meta.row + '].variation';
                if (data != null) {
                    value = FormatDecimal(data, 2);
                }
                return '<input onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this,\'' + currentname + '\');" name="' + currentname + '" onchange="checknumber(\'' + currentname + '\',' + true + ')" Isdropdown="false" style="width:100px;"  type="text" value="' + value + '">';
            }
        },
        {
            targets: 23,
            data: "dm",
            render: function (data, type, row, meta) {
                var value = '';
                var currentname = 'ColateralList[' + meta.row + '].dm';
                if (data != null) {
                    value = FormatDecimal16(data);
                }
                return '<input onkeypress="return text_OnKeyPress_NumberOnlyAndDotAndMinus(this,\'' + currentname + '\');" name="ColateralList[' + meta.row + '].dm" Isdropdown="false" onchange="checknumber(\'' + currentname + '\',' + true + ')" style="width:100px;"  type="text" value="' + value + '">';
            }
        },
        {
            targets: 24,
            data: "trade_date",
            "visible": false,
            render: function (data, type, row, meta) {
                var value = '';
                var currentname = 'ColateralList[' + meta.row + '].trade_date';
                if (data != null) {
                    value = moment(data).format('DD/MM/YYYY');
                }
                return '<input name="' + currentname + '" readonly="readonly" style="background-color:lightGray;width:80px;" Isdropdown="false" type="text"' + (value !== '' ? ' value="' + value + '">' : '>');
            }
        },
        {
            targets: 25,
            data: "settlement_date",
            "visible": false,
            render: function (data, type, row, meta) {
                var value = '';
                var currentname = 'ColateralList[' + meta.row + '].settlement_date';
                if (data != null) {
                    value = moment(data).format('DD/MM/YYYY');
                }
                return '<input name="' + currentname + '" readonly="readonly" style="background-color:lightGray;width:80px;" Isdropdown="false" type="text"' + (value !== '' ? ' value="' + value + '">' : '>');
            }
        },
        {
            targets: 26,
            data: "status",
            "visible": false
        },
        {
            targets: 27,
            data: "instrument_id",
            "visible": false
        },
        {
            targets: 28,
            data: "datafrom",
            "visible": false,
            render: function (data, type, row, meta) {
                return '<input name="ColateralList[' + meta.row + '].datafrom" Isdropdown="false"  type="text" value="' + data + '">';
            }
        },
        {
            targets: 29,
            data: "rp_price_date",
            "visible": false
        },
        {
            targets: 30,
            data: "rp_source",
            "visible": false
        },
        {
            targets: 31,
            data: "purchase_price",
            "visible": false
        },
        {
            targets: 32,
            data: "counter_party_id",
            "visible": false
        },
        {
            targets: 33,
            data: "interest_total",
            "visible": false
        },
        {
            targets: 34,
            data: "deal_period",
            "visible": false
        },
        {
            targets: 35,
            data: "basis_code",
            "visible": false
        },
        {
            targets: 36,
            data: "wht_tax",
            "visible": false
        },
        {
            targets: 37,
            data: "formula",
            "visible": false
        },
        {
            targets: 38,
            data: "trigger",
            "visible": false,
            render: function (data, type, row, meta) {
                return '<input name="ColateralList[' + meta.row + '].trigger" Isdropdown="false"  type="text" value="' + data + '">';
            }
        },
        {
            targets: 39,
            data: "action",
            className: 'dt-body-center',
            width: 20,
            orderable: false,
            render: function (data, type, row, meta) {
                var html = '<button type="button"  style="text-align:center;" class="btn btn-delete btn-round" key="' + row.instrument_id + '" form-mode="delete" id="delete_' + meta.row + '" datafrom="' + row.datafrom + '" data-action="delete"  onclick="GM.RPDealEntry.Coll.Action(\'delete\',' + meta.row + ')" ><i class="feather-icon icon-trash-2"></i></button>';
                return html;
            }
        },
        {
            targets: 40,
            data: "port_name",
            "visible": false
        },
        {
            targets: 41,
            data: "special_case_id",
            "visible": false
        }
        ],
        footerCallback: function (row, data, start, end, display) {
            //var api = this.api();

            if (data.length > 0) {
                if (data[data.length - 1].message != "") {
                    swal(data[data.length - 1].message, "", "warning");
                    data[data.length - 1].message = "";
                }
            }

            var Sum_cash_amount = 0;
            for (var i = 0; i < data.length; i++) {
                if (data[i].status != "delete") {
                    Sum_cash_amount += data[i].cash_amount;
                }
            }

            var purchase_price = $('#purchase_price').val() === "" ? 0.00 : parseFloat($('#purchase_price').val().replace(/,/g, ''));
            var exch_rate = $('#exch_rate').val() === "" ? 0.00 : parseFloat($('#exch_rate').val().replace(/,/g, ''));

            if ($('#page_name').val() === 'editpage') {
                if (((parseFloat(Sum_cash_amount) / parseFloat(exch_rate)) > parseFloat(purchase_price)
                    && (parseFloat(purchase_price) * parseFloat(exch_rate)) >= parseFloat(Sum_cash_amount))
                    && $("#ismanual_cal").val().toUpperCase() == "FALSE") {
                    $('#purchase_price').val(FormatDecimal(parseFloat((parseFloat(Sum_cash_amount) / parseFloat(exch_rate))).toFixed(2), 2));
                }
            } else {
                if ((parseFloat(Sum_cash_amount) / parseFloat(exch_rate)) > parseFloat(purchase_price)
                    && $("#ismanual_cal").val().toUpperCase() == "FALSE") {
                    $('#purchase_price').val(FormatDecimal(parseFloat((parseFloat(Sum_cash_amount) / parseFloat(exch_rate))).toFixed(2), 2));
                }
            }

            $("#Sum_cash_amount").html(FormatDecimal(parseFloat(Sum_cash_amount), 2));

            var Sum_interest_amount = 0;
            for (var i = 0; i < data.length; i++) {
                if (data[i].status != "delete" && data[i].interest_amount != null) {
                    Sum_interest_amount += data[i].interest_amount;
                }
            }
            $("#Sum_interest_amount").html(FormatDecimal(parseFloat(Sum_interest_amount), 2));

            var Sum_wht_amount = 0;
            for (var i = 0; i < data.length; i++) {
                if (data[i].status != "delete" && data[i].wht_amount != null) {
                    Sum_wht_amount += data[i].wht_amount;
                }
            }
            $("#Sum_wht_amount").html(FormatDecimal(parseFloat(Sum_wht_amount), 2));

            var Sum_temination_value = 0;
            for (var i = 0; i < data.length; i++) {
                if (data[i].status != "delete" && data[i].temination_value != null) {
                    Sum_temination_value += data[i].temination_value;
                }
            }
            $("#Sum_temination_value").html(FormatDecimal(parseFloat(Sum_temination_value), 2));

            setTimeout(function () {
                var rowNumber = 0;
                for (var i = 0; i < data.length; i++) {
                    rowNumber = data[i].RowNumber;
                }

                if (rowNumber - 1 > 0) {
                    var lastrow = rowNumber - 1;
                    var cashamountNextBond = data[lastrow].cash_amount;
                    Sum_cash_amount = roundTo(Sum_cash_amount, 2);
                    if (purchase_price > Sum_cash_amount) {
                        var diff = purchase_price - Sum_cash_amount;
                        if (window.flagUpdate) {
                            cashamountNextBond = (cashamountNextBond - data[lastrow].cash_amount) + diff;
                            CalculateBond('ColateralList[' + lastrow + '].CASH_AMOUNT', false, Math.abs(cashamountNextBond));
                        } else {
                            window.flagUpdate = true;
                        }
                    }
                }
            }, 500);
        },
        drawCallback: function (settings) {
            updateColIsinCodeBOTN($("#bilateral_contract_no").val());
        },
        fixedColumns: {
            leftColumns: 0,
            rightColumns: 3
        }
    });

    GM.RPDealEntry.Coll.Search = function () {
        var rpsource = $("#rp_source_value").val();
        var rppricedate = $("#rp_price_date_value").val();
        var purchaseprice = $('#purchase_price').val() === "" ? 0.00 : parseFloat($('#purchase_price').val().replace(/,/g, ''));
        var exch_rate = $("#exch_rate").val() === "" ? 0.00 : parseFloat($('#exch_rate').val().replace(/,/g, ''));
        purchaseprice = purchaseprice * exch_rate;
        var instrument = $("#instrument_code").val();
        var counterpartyid = $("#counter_party_id").val();
        var settledate = $("#settlement_date").val();
        if (settledate === '') {
            swal("Warning", "Please input settlement date.", "warning");
            return false;
        }
        var maturity_date = $("#maturity_date").val();
        if (maturity_date === '') {
            maturity_date = settledate;
        }
        var interest_rate = $("#interest_total").val();
        var period = $("#deal_period").val();
        var yearbasis = $("#basis_code").val();
        var wht = $("#wht_tax").val(); // wht at couterparty
        var sumcash_amt = $('#Sum_cash_amount').html() === "" ? 0.00 : parseFloat($('#Sum_cash_amount').html().replace(/,/g, ''));
        var formula = $("#formula").val(); // สูตรที่ใช้ในการคำนวน
        var cur = $("#cur_pair1").val();
        var port_name = $("#port").val();
        var special_case_id = $('#special_case_id').val()
        purchaseprice = parseFloat(purchaseprice) - parseFloat(sumcash_amt);

        if (instrument === "") {
            instrument = $("#isin_code").val();
        }

        if (getinstrumenttypeselect() === "PRP" && $('#x-table-data').DataTable().rows().data().length > 0) {
            swal("Warning", "Private Repo is not support Multi Colleteral", "warning");
        } else {
            if (instrument !== "" && rpsource !== "") {
                GM.RPDealEntry.Coll.Table.columns(37).search(formula);
                //for checkcollateral
                GM.RPDealEntry.Coll.Table.columns(33).search(interest_rate);
                GM.RPDealEntry.Coll.Table.columns(34).search(period);
                GM.RPDealEntry.Coll.Table.columns(35).search(yearbasis);
                GM.RPDealEntry.Coll.Table.columns(36).search(wht);

                //for add collateral
                GM.RPDealEntry.Coll.Table.columns(30).search(rpsource);
                GM.RPDealEntry.Coll.Table.columns(29).search(rppricedate);
                GM.RPDealEntry.Coll.Table.columns(31).search(purchaseprice);
                GM.RPDealEntry.Coll.Table.columns(2).search(instrument);
                GM.RPDealEntry.Coll.Table.columns(32).search(counterpartyid);
                GM.RPDealEntry.Coll.Table.columns(25).search(settledate);
                GM.RPDealEntry.Coll.Table.columns(19).search(maturity_date);

                GM.RPDealEntry.Coll.Table.columns(6).search(cur);

                GM.RPDealEntry.Coll.Table.columns(40).search(port_name);
                GM.RPDealEntry.Coll.Table.columns(41).search(special_case_id);

                GM.RPDealEntry.Coll.Table.columns(26).search("add");
                GM.RPDealEntry.Coll.Table.columns(28).search("ui");

                GM.RPDealEntry.Coll.Table.draw();

                $("#ddl_rp_pricedate").prop('disabled', true);
                $("#ddl_rp_source").prop('disabled', true);

            } else {
                swal("Warning", "Repo Source or Instrument is not select", "warning");
            }
        }
    };

    GM.RPDealEntry.Coll.Action = function (action, id) {
        if (action === "delete") {
            swal({
                title: "Do you want to delete ?",
                text: "",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-danger",
                confirmButtonText: "Yes",
                cancelButtonText: "No",
                closeOnConfirm: true,
                closeOnCancel: true
            },
                function (isConfirm) {
                    if (isConfirm) {
                        var tr = $("#delete_" + id).parent().parent();
                        var rowindex = id;
                        var datafrom = $("#delete_" + id).attr("datafrom");

                        GM.RPDealEntry.Coll.Table.RowSelected = tr;
                        var instrumentcode = $('input[name="ColateralList[' + rowindex + '].instrument_code"]').val();
                        var isin = $('input[name="ColateralList[' + rowindex + '].isin_code"]').val();

                        GM.RPDealEntry.Coll.Table.columns(2).search(instrumentcode);
                        GM.RPDealEntry.Coll.Table.columns(3).search(isin);
                        GM.RPDealEntry.Coll.Table.columns(26).search("delete");
                        GM.RPDealEntry.Coll.Table.columns(28).search(datafrom);
                        GM.RPDealEntry.Coll.Table.draw();
                        GM.Message.Clear();
                        //window.checkcommiteditcol = false;

                        //TODO
                        setTimeout(function () {
                            var purchase_price = $('#purchase_price').val() === "" ? 0.00 : parseFloat($('#purchase_price').val().replace(/,/g, ''));

                            var countRow = 0;
                            $('#x-table-data tbody tr').each(function (i, row) {
                                var status = $('input[name="ColateralList[' + i + '].status"]').val();
                                if (status !== "delete") {
                                    countRow = countRow + 1;
                                }
                            });

                            if (countRow === 1) {
                                $("#ddl_rp_pricedate").prop('disabled', false);
                                $("#ddl_rp_source").prop('disabled', false);
                            }

                            var cash_amountLastRow = $('input[name="ColateralList[' + (countRow - 1) + '].cash_amount"]').val() == "" || $('input[name="ColateralList[' + (countRow - 1) + '].cash_amount"]').val() == null ? 0 : $('input[name="ColateralList[' + (countRow - 1) + '].cash_amount"]').val().replace(/,/g, '');
                            var exch_rate = $('#exch_rate').val().replace(/,/g, '');

                            var cash_amount = 0;
                            cash_amountLastRow = parseFloat(cash_amountLastRow) / parseFloat(exch_rate);

                            $('#x-table-data tbody tr').each(function (i, row) {
                                var status = $('input[name="ColateralList[' + i + '].status"]').val();
                                if (status !== "delete") {
                                    var amount = $('input[name="ColateralList[' + i + '].cash_amount"]').val();
                                    if (amount != null) {
                                        cash_amount += parseFloat(amount.replace(/,/g, '')) / exch_rate;
                                    }
                                }
                            });

                            var sum_cash = parseFloat(cash_amountLastRow) + (purchase_price - cash_amount); // + parseFloat(cash_amount);

                            if (sum_cash >= purchase_price) {
                                sum_cash = purchase_price;
                            }

                            if (countRow > 0) {
                                setTimeout(function () {
                                    CalculateBond('ColateralList[' + (countRow - 1) + '].CASH_AMOUNT', false, sum_cash);
                                }, 500);
                            }
                            window.flagUpdate = false;
                        }, 1000);
                    }
                });
            adjusttable();
        }
    };

    GM.RPDealEntry.Coll.ClickCell = function (action, id, defaultvalue) {
        var data = {
            port: $("#port").val()
        };
        var row = id[id.length - 1];

        if (defaultvalue != null) {
            GM.Utility.DDLStandard(id, data, action, defaultvalue, "--- Select ----");
        }
        else if ($("#port").val() == 'BANKING' && $("#trans_deal_type").val() == 'LD') {
            $.ajax({
                url: "/RPDealEntry/FillPortTrans",
                type: "GET",
                dataType: "JSON",
                data: data,
                success: function (res) {
                    var setValue = true;
                    for (var i = 0; i < res.length; i++) {
                        if (res[i].Text.toUpperCase() === 'MEMO-BNK') {
                            GM.Utility.DDLStandard(id, data, action, res[i].Text, "--- Select ----");
                            setValue = false;
                            SetPostBackDefult(res[i].Text, row);
                        }
                    }

                    if (setValue) {
                        GM.Utility.DDLStandard(id, data, action, "", "--- Select ----");
                    }
                }
            });
        } else {
            GM.Utility.DDLStandard(id, data, action, "", "--- Select ----");
        }
    };

    GM.RPDealEntry.CopyDeal = function () {

    };

    //End Col Logic

    $("#btn_addcol").on('click', function () {

        GM.RPDealEntry.Coll.Search();
        $("#ddl_instrument_code").find(".selected-data").text("Select...");
        $("#instrument_code").val(null);

        $("#ddl_isin_code").find(".selected-data").text("Select...");
        $("#isin_code").val(null);

        checkcommiteditcol = false;

    });

    GM.RPDealEntry.Interest = {};
    GM.RPDealEntry.Interest.Form = $("#check-interest");

    $("#search-form").on('submit', function (e) {

        return false;
    });

    $("#ismanual_cal_Value").change(function () {
        $("#ismanual_cal_Value").val(this.checked);
        $("#ismanual_cal").val(this.checked);
    });

    numberOnlyAndDot = function (obj) {
        obj.value = obj.value
            .replace(/[^\d.]/g, '') // numbers and decimals only
            .replace(/(^[\d]{3})[\d]/g, '$1') // not more than 3 digits at the beginning
            .replace(/(\..*)\./g, '$1') // decimal can't exist more than once
            .replace(/(\--*)\-/g, '$1') // decimal can't exist more than once
            .replace(/(\.[\d]{8})./g, '$1'); // not more than 6 digits after decimal
    };

    numberOnlyAndDotAndMinute = function (obj) {
        obj.value = obj.value
            .replace(/[^\d.-]/g, '') // numbers and decimals and dot and minute only
            .replace(/(^[\d]{6})[\d]/g, '$1') // not more than 3 digits at the beginning
            .replace(/(^-[\d]{6})[\d]/g, '$1') // not more than 3 digits at the beginning
            .replace(/(\--*)\-/g, '$1') // decimal can't exist more than once
            .replace(/(\..*)\./g, '$1') // decimal can't exist more than once
            .replace(/(\.[\d]{8})./g, '$1'); // not more than 8 digits after decimal
    };

    auto8digit = function (obj) {
        if (obj.value.length) {
            var nStr = obj.value;
            if (nStr === 'NaN')
                nStr = '0';
            var x = nStr.split('.');
            var x1 = x[0];
            var x2 = '00000000';

            if (x.length > 1) {
                x2 = x[1];

                var currentDigit = x[1].length;
                if (currentDigit < 6) {
                    for (var i = currentDigit; i < 6; i++) {
                        x2 += '0';
                    }
                }
            }

            if (x1 > 999) {
                x1 = 999;
            }

            x1 = x1.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            return obj.value = x1 + '.' + x2;
        }
        return "";
    };

    updateColIsinCodeBOTN = function (bilateralconno) {

        // find colatoral in list for type BOTN to update isin_code from Bilat.Contract => BOTN-KTB-FIX-A1D-/19 

        $('#x-table-data tbody tr').each(function (i, row) {
            var instrument_code = $('input[name="ColateralList[' + i + '].instrument_code"]').val();
            if (instrument_code !== undefined && instrument_code.indexOf("BOTN") !== -1) {

                var isin_code = $('input[name="ColateralList[' + i + '].isin_code"]').val();
                if (isin_code !== null && isin_code !== undefined && isin_code !== '') {
                    if (isin_code.indexOf("-") !== -1)
                        isin_code = isin_code.split('-')[0] + '-' + bilateralconno;
                    else
                        isin_code += bilateralconno;
                } else {
                    isin_code += bilateralconno;
                }

                $('input[name="ColateralList[' + i + '].isin_code"]').val(isin_code);
            }
        });

    };

    IsColleteralAdded = function () {

        var _rowcol = $('#x-table-data tbody tr').children().length;
        if (_rowcol == 1)
            return false;
        else
            return true;
    };

    $("#btn_checkinterest").on('click', function () {
        GM.RPDealEntry.Interest.Form.modal('toggle');
        var data = { transno: '' };
        $.ajax({
            type: "GET",
            url: "/RPDealEntry/GetRPTransCheckCostOfFund",
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: data,
            success: function (res) {
                var body = $("#modal-interest").find("tbody");
                var html = "";
                var totalamount = 0;
                body.html("");
                $.each(res, function (i, resdata) {
                    html = "<tr><td>" + resdata.period + "</td><td>"
                        + resdata.day_period + "</td><td>"
                        + resdata.reference + "</td><td>"
                        + resdata.ref_rate + "</td><td>"
                        + resdata.costoffund_spread + "</td><td>"
                        + resdata.costoffund_total_rate + "</td><td>"
                        + resdata.interest_amount + "</td></tr>";
                    totalamount += parseFloat(resdata.interest_amount);
                });

                $("#lbl_totalinterest_check").text(FormatDecimal(totalamount.toFixed(2)));
                body.append(html);

            },
            error: function (res) {
                // TODO: Show error
                GM.Message.Clear();
            }
        });

        //$.ajax({
        //    type: "GET",
        //    url: "/RPDealEntry/GetRPTransCheckCostOfFund",
        //    content: "application/json; charset=utf-8",
        //    dataType: "json",
        //    data: data,
        //    success: function (res) {
        //        var body = $("#modal-cost-fund").find("tbody");
        //        var html = "";
        //        body.html("");
        //        var totalamount = 0;
        //        $.each(res, function (i, resdata) {
        //            html = "<tr><td>" + resdata.period + "</td><td>" + resdata.day_period + "</td><td>" + resdata.costoffund_rate + "</td><td>" + resdata.costoffund_spread +
        //                "</td><td>" + resdata.costoffund_total_rate + "</td><td>" + resdata.costoffund_amount + "</td></tr>";
        //            totalamount = parseFloat(resdata.costoffund_amount) + parseFloat(totalamount);
        //        });
        //        $("#lbl_totalcostoffund_check").text(FormatDecimal(totalamount.toFixed(2)));
        //        body.append(html);
        //    },
        //    error: function (res) {
        //        // TODO: Show error
        //        GM.Message.Clear();
        //    }
        //});

        $("#lbl_selltementdate_check").text($("#settlement_date").val());
        $("#lbl_matdate_check").text($("#maturity_date").val());
        $("#lbl_purchase_check").text($("#purchase_price").val());
        $("#lbl_repurchase_check").text($("#repurchase_price").val());
    });

    $("#net_settement_flag_Value").change(function () {
        $("#net_settement_flag_Value").val(this.checked);
        $("#net_settement_flag").val(this.checked);
        if (this.checked == true) {
            $("#ref_trans_no").removeAttr("readonly");
            $("#ref_trans_no").parent().find("span").removeAttr("readonly");
            $("#ref_trans_no").parent().find("span").attr("style", "cursor: pointer");

        } else {
            $("#ref_trans_no").attr("readonly", "true");
            $("#ref_trans_no").parent().find("span").attr("readonly", "true");
            $("#ref_trans_no").parent().find("span").removeAttr("style");
        }
    });

    GM.RPDealEntry.PopUpRefNo = {};
    GM.RPDealEntry.PopUpRefNo.Form = $("#modalRefTransNo");

    $(".btnPopupRefNo").click(function () {
        if ($("#net_settement_flag").val() == 'true') {
            GM.RPDealEntry.PopUpRefNo.Form.modal('toggle');
            $("#lbl_counter_party_name").text($('#counter_party_name').val());

            var tradedate = $("#trade_date").val();
            var settlementdate = $("#settlement_date").val();
            var maturity_date = $("#maturity_date").val();

            var data = {
                trans_deal_type: $('#trans_deal_type').val(),
                counter_party_id: $('#counter_party_id').val(),
                counter_party_code: '',
                trade_date: tradedate,
                settlement_date: settlementdate,
                maturity_date: maturity_date,
                cur_pair1: $("#cur_pair1").val(),
                cur_pair2: $("#cur_pair2").val(),
                counter_party_fund_id: $("#counter_party_fund_id").val(),
                trans_no: ''
            };
            $.ajax({
                type: "POST",
                url: "/RPDealEntry/GetPopupRefno",
                content: "application/json; charset=utf-8",
                dataType: "json",
                data: data,
                success: function (res) {
                    if (res.returnCode === 0) {
                        var body = $("#tableRefTransNo").find("tbody");
                        var html = "";
                        body.html("");
                        $.each(res.data, function (i, resdata) {
                            html += "<tr class='clickable-row' data-id='" + resdata.trans_no + "' style='cursor: pointer'><td>"
                                + resdata.RowNumber + "</td><td>"
                                + resdata.trans_no + "</td><td>"
                                + resdata.trans_deal_type + "</td><td>"
                                + moment(resdata.trade_date).format('DD/MM/YYYY') + "</td><td>"
                                + moment(resdata.settlement_date).format('DD/MM/YYYY') + "</td><td>"
                                + moment(resdata.maturity_date).format('DD/MM/YYYY') + "</td><td>"
                                + resdata.deal_period + "</td><td>"
                                + resdata.purchase_price + "</td><td>"
                                + resdata.repurchase_price + "</td><td>"
                                + resdata.cur + "</td></tr>";
                        });
                        body.append(html);
                    }
                },
                error: function (res) {
                    GM.Message.Clear();
                }
            });
        }
    });
});

$(document).on('click', '.clickable-row', function (e) {
    try {
        console.log('$(this).data("id") : ' + $(this).data("id"));
        $('#ref_trans_no').val($(this).data("id"));
        GM.RPDealEntry.PopUpRefNo.Form.modal('hide');
    }
    catch (err) { alert(err); }
});