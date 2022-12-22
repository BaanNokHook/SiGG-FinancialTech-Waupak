$(document).ready(function () {

    var budate = $("#BusinessDate").text();
    var oldMaturityDate = $('#maturity_date').val();
    var oldDealPeriod = $('#deal_period').val();
    var oldRepurchasePrice = $('#repurchase_price').val();
    var oldInterestAmount = $('#interest_amount').val();
    var oldWithholdingAmount = $('#withholding_amount').val();

    var formatmmddyyyydate = budate.split("/");
    formatmmddyyyydate = formatmmddyyyydate[1] + "/" + formatmmddyyyydate[0] + "/" + formatmmddyyyydate[2];
    var business_date = new Date(formatmmddyyyydate);

    if ($('#terminate_date').val().length) {
        $('#terminate_date').text($('#terminate_date').val());
    }

    $('#remark-deal').click(function (e) {
        var expand = $("div#remark-deal-icon").attr("aria-expanded");
        if (expand == "true") {
            $("#remark-deal-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#remark-deal-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }

    });

    //Binding Div Detail
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    };
    GM.RPEarlyTermination.Form = {};
    GM.RPEarlyTermination.Form.DataBinding = function (p) {
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

    if ($('#Txt_repo_deal_type').val() == "Private Repo") {

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
    //    $('#Txt_interest_spread').val(parseFloat(interest_spread).toFixed(2));
    //}
    //else {
    //    interest_total_int += 0;
    //}

    //$('#Txt_interest_total_int').val(interest_total_int.toFixed(6));

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

    //Function : Fund Code ============================================
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

    //Function : payment_method ============================================
    var payment_method = $('#Txt_payment_method').val();
    var trans_mt_code = $('#Txt_trans_mt_code').val();
    if (payment_method != undefined && payment_method.length != 0) {
        $('#Sp_payment_method').html(payment_method);
    }
    else {
        $('#Sp_payment_method').html("None");
    }

    if (trans_mt_code != undefined && trans_mt_code.length != 0) {
        $('#Sp_trans_mt_code').html(trans_mt_code);
    }
    else {
        $('#Sp_trans_mt_code').html("None");
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
    GM.RPEarlyTermination.Interest = {};
    GM.RPEarlyTermination.Interest.Form = $("#check-interest");

    $("#btn_checkinterest").on('click', function () {
        GM.RPEarlyTermination.Interest.Form.modal('toggle');
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


    //Remark Dropdown
    $("#ddl_remark").click(function () {
        var txt_search = $('#txt_remark');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_remark').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Remark Dropdown

    $('#terminate_date').on('dp.hide', function (e) {
        
        if ($('#terminate_date').val().length) {
            var selectDate = moment(e.date).format('DD/MM/YYYY');
            var maturityDate = setformatdateyyyymmdd(oldMaturityDate);
            $('#terminate_date_error').text("");
            $('#terminate_date').removeClass("input-validation-error");

            var terminateDate = moment(e.date).format('YYYYMMDD');
            var businessDate = setformatdateyyyymmdd(budate);
            var settlementDate = setformatdateyyyymmdd($("#settlement_date").val());

            if ($('#terminate_date').text() !== selectDate) {
                $('.spinner').css('display', 'block');
                var oldDate = $('#terminate_date').text();
                $('#terminate_date').text(selectDate);
                if (terminateDate < businessDate) {
                    $('.spinner').css('display', 'none');
                    $("#terminate_date").text(oldDate);
                    $("#terminate_date").val(oldDate);
                    swal("Warning", "Terminate Date Can't less than Business Date", "warning");
                    return;
                }
                else if (terminateDate <= settlementDate) {
                    $('.spinner').css('display', 'none');
                    $("#terminate_date").text(oldDate);
                    $("#terminate_date").val(oldDate);
                    swal("Warning", "Terminate Date Can't less than Settlement Date", "warning");
                    return;
                }
                else if (terminateDate > maturityDate){
                    $('.spinner').css('display', 'none');
                    $("#terminate_date").text(oldDate);
                    $("#terminate_date").val(oldDate);
                    swal("Warning", "Terminate Date Can't more than Maturity Date", "warning");
                    return;
                }

                //validate date
                setTimeout(function () {
                    var data = {
                        date: selectDate,
                        cur: $("#cur").val()
                    };

                    $.ajax({
                        url: "/RPDealEntry/CheckHoliday",
                        type: "POST",
                        dataType: "JSON",
                        data: data,
                        async: false,
                        success: function (res) {

                            if (res == true) {
                                $('.spinner').css('display', 'none');
                                var message = selectDate + ' is the holiday, please select new day.';
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

                                        });
                                },
                                    200);
                                $("#terminate_date").text(oldDate);
                                $("#terminate_date").val(oldDate);
                            } else {
                                Calculate('Open');
                                $('.spinner').css('display', 'none');
                            }
                        }
                    });
                }, 200);

            }
        } else {
            $('.spinner').css('display', 'block');
            var status = $('#trans_status').val();
            if (status != "Terminate") {
                $('.spinner').css('display', 'none');
                $("#terminate_date").text("");
                $("#terminate_date").val(null);
                $('#fee_amount').val('0.00');
                $("#maturity_date").text(oldMaturityDate);
                $("#maturity_date").val(oldMaturityDate);
                $('#deal_period').val(oldDealPeriod);
                $('#repurchase_price').val(oldRepurchasePrice);
                $('#interest_amount').val(oldInterestAmount);
                $('#withholding_amount').val(oldWithholdingAmount);

                GM.RPEarlyTermination.Table.draw();
            }
            else
            {
                Calculate(status);
            }
            
            $('.spinner').css('display', 'none');
        }
    });

    GM.RPEarlyTermination.Submit = function (btn) {
        if (!ValidateForm()) {
            return;
        }

        swal({
            title: "Comfirm Submit?",
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
                SubmitForm();
            }
        });
    };

    GM.RPEarlyTermination.Reset = function (btn) {
        swal({
                title: "Comfirm Reset?",
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
                    Calculate('Terminate');
                }
            });
    };

    function ValidateForm() {
        var isValidate = true;
        var status = $('#trans_status').val();
        if (status != 'Terminate') {
            if ($('#terminate_date').val() == '') {
                isValidate = false;
                $('#terminate_date_error').text("Terminate Date field is required.");
                $('#terminate_date').addClass("input-validation-error");
                focusInput($('#terminate_date'));
            } else {
                $('#terminate_date_error').text("");
                $('#terminate_date').removeClass("input-validation-error");
            }
        }

        if ($('#fee_amount').val() == '') {
            isValidate = false;
            $('#fee_amount_error').text("Unwind Fees field is required.");
            $('#fee_amount').addClass("input-validation-error");
            focusInput($('#fee_amount'));
        }
        else if ($('#fee_amount').val() != '0.00' && $('#terminate_date').val() == '')
        {
            isValidate = false;
            $('#terminate_date_error').text("Terminate Date field is required.");
            $('#terminate_date').addClass("input-validation-error");
            focusInput($('#terminate_date'));

            $('#fee_amount_error').text("");
            $('#fee_amount').removeClass("input-validation-error");
        }
        else
        {
            $('#fee_amount_error').text("");
            $('#fee_amount').removeClass("input-validation-error");
        }

        return isValidate;
    }

    function SubmitForm() {
        $('.spinner').css('display', 'block');
        var collList = [];

        $.each(GM.RPEarlyTermination.Table.data(),
            function (i, v) {

                collList.push($.extend(true,
                    v,
                    {
                        colateral_id: v.colateral_id,
                        instrument_id: v.instrument_id,
                        maturity_date: moment(v.maturity_date).format('DD/MM/YYYY'),
                        cash_amount: v.cash_amount,
                        interest_amount: v.interest_amount,
                        wht_amount: v.wht_amount,
                        temination_value: v.temination_value
                    }
                ));
            });

        var dataToPost = {
            trans_no: $('#trans_no').val(),
            maturity_date: $('#maturity_date').val(),
            deal_period: $('#deal_period').val(),
            repurchase_price: $('#repurchase_price').val(),
            interest_amount: $('#interest_amount').val(),
            withholding_amount: $('#withholding_amount').val(),
            remark_id: $('#remark_id').val(),
            deal_remark: $('#deal_remark').val(),
            commentaries: $('#commentaries').val(),
            fee_amount: $('#fee_amount').val().replace(/,/g, ''),
            terminate_date: $('#terminate_date').val(),
            ColateralList: collList
        };

        var urlAction = $('#search-form').attr('action');
        $.ajax({
            type: "POST",
            url: urlAction,
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: dataToPost,
            success: function (d) {
                $('.spinner').css('display', 'none');
                if (!d.Message.length) {
                    if (d.trans_no.length) {
                        setTimeout(function () {
                            swal({
                                title: "Complete",
                                text: "Save Successfully",
                                type: "success",
                                showCancelButton: false,
                                confirmButtonClass: "btn-success",
                                confirmButtonText: "Yes",
                                cancelButtonText: "No",
                                closeOnConfirm: true,
                                closeOnCancel: true
                            },
                                function () {
                                    window.location.href = "/RPEarlyTermination/Detail?trans_no=" + d.trans_no;
                                }
                            );
                        },
                            100);
                    } else {
                        setTimeout(function () {
                            swal({
                                title: "Complete",
                                text: "Save Successfully",
                                type: "success",
                                showCancelButton: false,
                                confirmButtonClass: "btn-success",
                                confirmButtonText: "Yes",
                                cancelButtonText: "No",
                                closeOnConfirm: true,
                                closeOnCancel: true
                            },
                                function () {
                                    window.location.href = "/RPEarlyTermination";
                                }
                            );
                        }, 100);
                    }
                } else {
                    swal("Failed!", "Error : " + d.Message, "error");
                }
            },
            error: function (d) {
                $('.spinner').css('display', 'none');
            }
        });
    }

    function Calculate(status) {
        setTimeout(function () {
            var data = {
                trans_no: $("#trans_no").val(),
                terminate_date: $('#terminate_date').val(),
                asof_date: $("#BusinessDate").text()
            };

            var url;
            if (status == "Terminate") {
                url = '/RPEarlyTermination/Reset';
            } else {
                url = '/RPEarlyTermination/Calculate';
            }

            $.ajax({
                url: url,
                type: "POST",
                dataType: "JSON",
                data: data,
                async: false,
                success: function (res) {
                    if (res.success) {
                        //set form

                        if (res.data.terminate_date != null) {
                            var terminate_date = moment(res.data.terminate_date).format('DD/MM/YYYY');
                            $('#terminate_date').val(terminate_date);
                            $('#terminate_date').text(terminate_date);
                        } else {
                            $('#terminate_date').val('');
                            $('#terminate_date').text('');
                        }

                        $('#fee_amount').val(FormatDecimal2(res.data.fee_amount));
                        $('#deal_period').val(res.data.deal_period);

                        var maturity_date = moment(res.data.maturity_date).format('DD/MM/YYYY');
                        $('#maturity_date').val(maturity_date);
                        $('#maturity_date').text(maturity_date);
                        if (status == "Terminate") {
                            oldMaturityDate = maturity_date;
                        }

                        $('#repurchase_price').val(res.data.repurchase_price);
                        $('#interest_amount').val(res.data.interest_amount);
                        $('#withholding_amount').val(res.data.withholding_amount);


                        if (GM.RPEarlyTermination.Table.data().length && res.data.ColateralList.length) {
                            var rowCount = GM.RPEarlyTermination.Table.data().length;
                            for (var i = 0; i < rowCount; i++) {
                                if (res.data.ColateralList[i] == null) {
                                    continue;
                                }
                                GM.RPEarlyTermination.Table.row(i).data(res.data.ColateralList[i]);
                            }
                        }

                        var Sum_cash_amount = GM.RPEarlyTermination.Table.column(14)
                            .data()
                            .reduce(function (a, b) {
                                return a + b;
                            }, 0);
                        $("#Sum_cash_amount").html(FormatDecimal2(parseFloat(Sum_cash_amount).toFixed(2)));

                        var Sum_interest_amount = GM.RPEarlyTermination.Table.column(16)
                            .column(16)
                            .data()
                            .reduce(function (a, b) {
                                return a + b;
                            }, 0);
                        $("#Sum_interest_amount").html(FormatDecimal2(parseFloat(Sum_interest_amount).toFixed(2)));

                        var Sum_wht_amount = GM.RPEarlyTermination.Table.column(17)
                            .data()
                            .reduce(function (a, b) {
                                return a + b;
                            }, 0);
                        $("#Sum_wht_amount").html(FormatDecimal2(parseFloat(Sum_wht_amount).toFixed(2)));

                        var Sum_temination_value = GM.RPEarlyTermination.Table.column(18)
                            .data()
                            .reduce(function (a, b) {
                                return a + b;
                            }, 0);
                        $("#Sum_temination_value").html(FormatDecimal2(parseFloat(Sum_temination_value).toFixed(2)));
                    }
                    else {
                        swal("Failed!", "Error : " + res.Message, "error");
                    }
                }
            });
        }, 200);
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

    
});

function focusInput(input) {
    var center = $(window).height() / 2;
    var top = $(input).offset().top;
    if (top > center) {
        $(window).scrollTop(top - center);
    }
    input.addClass("input-validation-error");
    //input.focus();
};

function formatDecimal(value, point) {
    var nStr = value.toString().replace(/,/g, '');
    var x = nStr.split('.');
    var x1 = x[0];
    var x2 = '00';

    if (x.length > 1) {
        x2 = x[1];

        var currentDigit = x[1].length;
        if (currentDigit < 2) {
            for (var i = currentDigit; i < 2; i++) {
                x2 += '0';
            }
        }
    }

    x1 = x1.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return x1 + '.' + x2;
}

function numberOnlyAndDot(obj) {
    obj.value = obj.value
        .replace(/[^\d.]/g, '')             // numbers and decimals only
        .replace(/(^[\d]{18})[\d]/g, '$1')   // not more than 18 digits at the beginning
        .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
        .replace(/(\.[\d]{2})./g, '$1');    // not more than 2 digits after decimal
}

function auto2digit(obj) {
    if (obj.value.length) {
        return obj.value = formatDecimal(obj.value, 2);
    }
    return "";
}