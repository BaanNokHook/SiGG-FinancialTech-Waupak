$(document).ready(function () {

    let data = {
        payment_method: $("#payment_method").val(),
        trans_deal_type: $("#Txt_trans_deal_type").val(),
        cur: $('#cur').val(),
        repo_deal_type: $('#Txt_repo_deal_type').val()
    };

    $.ajax({
        url: "/RPDealMaturity/FillTransMtCode",
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
            $("#ddl_trans_mt_code").find(".selected-value").val(value);

        }
    });


    $('#remark-deal').click(function (e) {
        var expand = $("div#remark-deal-icon").attr("aria-expanded");
        if (expand == "true") {
            $("#remark-deal-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#remark-deal-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }

    });

    //Binding DDL Payment Method
    $("#ddl_counterparty_payment").click(function () {
        var txt_search = $('#txt_counterparty_payment');
        var counterparty_id = $("#counter_party_id").val();
        var data = { counterpartyid: counterparty_id };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#ul_counterparty_payment").on("click", ".searchterm", function (event) {
        $("#ddl_trans_mt_code").find(".selected-data").text("Select...");
        $('#trans_mt_code').val('');
    });

    //Binding DDL trans_mt_code
    $("#ddl_trans_mt_code").click(function () {

        var txt_search = $('#txt_trans_mt_code');
        txt_search.val("");

        let data = {
            payment_method: $("#payment_method").val(),
            trans_deal_type: $("#Txt_trans_deal_type").val(),
            cur: $('#cur').val(),
            repo_deal_type: $('#Txt_repo_deal_type').val()
        };


        GM.Utility.DDLAutoComplete(txt_search, data, null);
    });

    //Binding : DDL MGP Method
    $("#ddl_margins_payment_method").click(function () {
        var txt_search = $('#txt_margins_payment_method');
        var data = { payment_method: '' };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    //Binding : DDL margins_mt_code
    $("#ddl_margins_mt_code").click(function () {

        var txt_search = $('#txt_margins_mt_code');
        txt_search.val("");
        var payment_method = $('#margins_payment_method').val();
        var trans_deal_type = $('#Txt_trans_deal_type').val();
        var data = { payment_method: payment_method, trans_deal_type: trans_deal_type };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
    });

    //Binding Div Detail
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    };
    GM.RPDealMaturity.Form = {};
    GM.RPDealMaturity.Form.DataBinding = function (p) {
        $('#' + p.form + ' :input').each(function () {
            var input = $(this);
            var inputid = input[0].id;
            var key = input[0].name.split('.')[1];
            var inputtype = input.attr("type");
            var inputvalue = input.attr("value");
            if (p.data[key] != null) {
                if (inputtype == "radio") {
                    var inputyes = $("[id=" + inputid + "][value=true]");
                    var inputno = $("[id=" + inputid + "][value=false]");
                    if (p.data[key]) {
                        inputno.removeAttr('checked');
                        inputno.attr('ischeck', 'false');
                        inputyes.attr('ischeck', 'true');
                        inputyes.attr('checked', 'checked');
                        inputyes.prop('checked', true);
                    }
                    else {
                        inputyes.removeAttr('checked');
                        inputyes.attr('ischeck', 'false');
                        inputno.attr('ischeck', 'true');
                        inputno.attr('checked', 'checked');
                        inputno.prop('checked', true);
                        //}
                    }
                }
                else {
                    $(this).val(p.data[key] + '');
                }

            }
            console.log('(' + key + ') => ' + p.data[key]);
        });
        $('#' + p.form + ' span').each(function () {
            var input = $(this);
            var key = input.attr("name");
            if (typeof key != "undefined") {
                if (p.data[key] != "" && p.data[key] != null) {
                    $(this)[0].innerHTML = p.data[key];
                }
            }
            console.log('(' + key + ') => ' + p.data[key]);
        });
    };

    //Function : Radio repo_deal_type =====================================
    var Lab_Private = $('#Lab_repo_deal_type_Pri');
    var Radio_Private = $('#repo_deal_type_Pri');
    var Lab_Bilateral = $('#Lab_repo_deal_type_Bil');
    var Radio_Bilateral = $('#repo_deal_type_Bil');

    if ($('#Txt_repo_deal_type').val() == "PRP") {

        Lab_Private.removeClass("radio-inline");
        Lab_Private.addClass("radio-inline checked");
        Radio_Private.prop('checked', true);

        Lab_Bilateral.removeClass("radio-inline checked");
        Lab_Bilateral.addClass("radio-inline");
        Radio_Bilateral.prop('checked', false);
    }
    else {

        Lab_Bilateral.removeClass("radio-inline");
        Lab_Bilateral.addClass("radio-inline checked");
        Radio_Bilateral.prop('checked', true);

        Lab_Private.removeClass("radio-inline checked");
        Lab_Private.addClass("radio-inline");
        Radio_Private.prop('checked', false);
    }

    //Function : Radio interest_type =====================================
    var Lab_interest_type_Fixed = $('#Lab_interest_type_Fixed');
    var Radio_interest_type_Fixed = $('#Radio_interest_type_Fixed');
    var Lab_interest_type_Float = $('#Lab_interest_type_Float');
    var Radio_interest_type_Float = $('#Radio_interest_type_Float');

    if ($('#Txt_interest_type').val() == "FIXED") {

        Lab_interest_type_Fixed.removeClass("radio-inline");
        Lab_interest_type_Fixed.addClass("radio-inline checked");
        Radio_interest_type_Fixed.prop('checked', true);

        Lab_interest_type_Float.removeClass("radio-inline checked");
        Lab_interest_type_Float.addClass("radio-inline");
        Radio_interest_type_Float.prop('checked', false);
    }
    else {

        Lab_interest_type_Float.removeClass("radio-inline");
        Lab_interest_type_Float.addClass("radio-inline checked");
        Radio_interest_type_Float.prop('checked', true);

        Lab_interest_type_Fixed.removeClass("radio-inline checked");
        Lab_interest_type_Fixed.addClass("radio-inline");
        Radio_interest_type_Fixed.prop('checked', false);
    }

    //Function : Calculate interest Total Int =====================================
    //var interest_rate = $('#Txt_interest_rate').val();
    //var interest_spread = $('#Txt_interest_spread').val();
    //var interest_total_int = 0;

    //if (!isNaN(interest_rate) && interest_rate.length != 0) {
    //    interest_total_int += parseFloat(interest_rate);
    //    $('#Txt_interest_rate').val(parseFloat(interest_rate).toFixed(6));
    //}
    //else {
    //    interest_total_int += 0;
    //}

    //if (!isNaN(interest_spread) && interest_spread.length != 0) {
    //    interest_total_int += parseFloat(interest_spread);
    //    $('#Txt_interest_spread').val(parseFloat(interest_spread).toFixed(6));
    //}
    //else {
    //    interest_total_int += 0;
    //}

   // $('#Txt_interest_total_int').val(interest_total_int.toFixed(6));

    //Function : Radio cost_type =================================================
    var Lab_cost_type_Fixed = $('#Lab_cost_type_Fixed');
    var Radio_cost_type_Fixed = $('#Radio_cost_type_Fixed');
    var Lab_cost_type_Float = $('#Lab_cost_type_Float');
    var Radio_cost_type_Float = $('#Radio_cost_type_Float');

    if ($('#Txt_cost_type').val() == "FIXED") {

        Lab_cost_type_Fixed.removeClass("radio-inline");
        Lab_cost_type_Fixed.addClass("radio-inline checked");
        Radio_cost_type_Fixed.prop('checked', true);

        Lab_cost_type_Float.removeClass("radio-inline checked");
        Lab_cost_type_Float.addClass("radio-inline");
        Radio_cost_type_Float.prop('checked', false);
    }
    else {

        Lab_cost_type_Float.removeClass("radio-inline");
        Lab_cost_type_Float.addClass("radio-inline checked");
        Radio_cost_type_Float.prop('checked', true);

        Lab_cost_type_Fixed.removeClass("radio-inline checked");
        Lab_cost_type_Fixed.addClass("radio-inline");
        Radio_cost_type_Fixed.prop('checked', false);
    }

    //Function : Counter Party Func ============================================
    var counter_party_fun = $('#Txt_counter_party_fun').val();
    if (counter_party_fun != undefined && counter_party_fun.length != 0) {
        $('#Sp_counter_party_fun').html(counter_party_fun);
    }
    else {
        $('#Sp_counter_party_fun').html("None");
    }


    var txt_append_name = $('#txt_append_name').val();
    if (txt_append_name != undefined && txt_append_name.length != 0) {
        $('#Sp_append_name').html(txt_append_name);
    }
    else {
        $('#Sp_append_name').html("None");
    }

    var txt_formula_name = $('#txt_formula_name').val();
    if (txt_formula_name != undefined && txt_formula_name.length != 0) {
        $('#Sp_formula_name').html(txt_formula_name);
    }
    else {
        $('#Sp_formula_name').html("None");
    }

    //Function : basis_code_name ============================================
    var basis_code_name = $('#Txt_basis_code_name').val();
    if (basis_code_name != undefined && basis_code_name.length != 0) {
        $('#Sp_basis_code_name').html(basis_code_name);
    }
    else {
        $('#Sp_basis_code_name').html("None");
    }

    //Function : trans_deal_type_name ============================================
    var trans_deal_type_name = $('#Txt_trans_deal_type_name').val();
    if (trans_deal_type_name != undefined && trans_deal_type_name.length != 0) {
        $('#Sp_trans_deal_type_name').html(trans_deal_type_name);
    }
    else {
        $('#Sp_trans_deal_type_name').html("None");
    }

    //Function : cur_pair1 ============================================
    var cur_pair1 = $('#Txt_cur_pair1').val();
    if (cur_pair1 != undefined && cur_pair1.length != 0) {
        $('#Sp_cur_pair1').html(cur_pair1);
    }
    else {
        $('#Sp_cur_pair1').html("None");
    }

    //Function : cur_pair2 ============================================
    var cur_pair2 = $('#Txt_cur_pair2').val();
    if (cur_pair2 != undefined && cur_pair2.length != 0) {
        $('#Sp_cur_pair2').html(cur_pair2);
    }
    else {
        $('#Sp_cur_pair2').html("None");
    }

    //Function : margins_payment_method ============================================
    var margins_payment_method = $('#Txt_margins_payment_method').val();
    var margins_mt_code = $('#Txt_margins_mt_code').val();

    if (margins_payment_method != undefined && margins_payment_method.length != 0) {
        $('#Sp_margins_payment_method').html(margins_payment_method);
    }
    else {
        $('#Sp_margins_payment_method').html("None");
    }

    if (margins_mt_code != undefined && margins_mt_code.length != 0) {
        $('#Sp_margins_mt_code').html(margins_mt_code);
    }
    else {
        $('#Sp_margins_mt_code').html("None");
    }

    //Function : Check interest Period ============================================
    GM.RPDealMaturity.Interest = {};
    GM.RPDealMaturity.Interest.Form = $("#check-interest");

    $("#btn_checkinterest").on('click', function () {
        GM.RPDealMaturity.Interest.Form.modal('toggle');
        var data = { transno: $(Txt_trans_no).val() };
        $.ajax({
            type: "GET",
            url: "/RPDealEntry/GetRPTransCheckInterest",
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: data,
            success: function (res) {
                var body = $("#modal-interest").find("tbody");
                var html = "";
                var totalamount = 0;
                body.html("");
                $.each(res, function (i, resdata) {
                    html = "<tr><td>" + resdata.period + "</td><td>" + resdata.day_period + "</td><td>" + resdata.coupon_rate + "</td><td>" + resdata.coupon_spread +
                        "</td><td>" + resdata.interest_total_rate + "</td><td>" + resdata.interest_amount + "</td></tr>";
                    totalamount = parseFloat(resdata.interest_amount) + parseFloat(totalamount);
                });

                $("#lbl_totalinterest_check").text(FormatDecimal(totalamount.toFixed(2)));
                body.append(html);

            },
            error: function (res) {
                // TODO: Show error
                GM.Message.Clear();
            }
        });

        $.ajax({
            type: "GET",
            url: "/RPDealEntry/GetRPTransCheckCostOfFund",
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: data,
            success: function (res) {
                var body = $("#modal-cost-fund").find("tbody");
                var html = "";
                body.html("");
                var totalamount = 0;
                $.each(res, function (i, resdata) {
                    html = "<tr><td>" + resdata.period + "</td><td>" + resdata.day_period + "</td><td>" + resdata.costoffund_rate + "</td><td>" + resdata.costoffund_spread +
                        "</td><td>" + resdata.costoffund_total_rate + "</td><td>" + resdata.costoffund_amount + "</td></tr>";
                    totalamount = parseFloat(resdata.costoffund_amount) + parseFloat(totalamount);
                });
                $("#lbl_totalcostoffund_check").text(FormatDecimal(totalamount.toFixed(2)));
                body.append(html);
            },
            error: function (res) {
                // TODO: Show error
                GM.Message.Clear();
            }
        });
        $("#lbl_selltementdate_check").text($("#settlement_date").val());
        $("#lbl_matdate_check").text($("#maturity_date").val());
        $("#lbl_purchase_check").text($("#purchase_price").val());
        $("#lbl_repurchase_check").text($("#repurchase_price").val());
    });

});