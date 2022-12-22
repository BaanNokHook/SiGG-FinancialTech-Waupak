
function roundTo(num, digit) {
    return +(Math.round(parseFloat(num).toFixed(digit + 1) + ("e+" + digit)) + ("e-" + digit));
}

function FormatDecimal(num, digit) {
    var format = Number(parseFloat(roundTo(num, digit)).toFixed(digit)).toLocaleString('en', {
        minimumFractionDigits: digit
    });
    return format;
}

$(window).on('load', function() {

    $("#lbl_interest_amount_adj").val(FormatDecimal($("#interest_amount_adj").val(), 2));
    $('#lbl_interest_amount_adj').text(FormatDecimal($("#interest_amount_adj").val(), 2) + " Baht");

    $("#Port_AFS_interest_amount_adj").val(FormatDecimal($("#Port_AFS_interest_amount_adj").val(),2));
    $("#Port_HTM_interest_amount_adj").val(FormatDecimal($("#Port_HTM_interest_amount_adj").val(),2));
    $("#Port_TRD_interest_amount_adj").val(FormatDecimal($("#Port_TRD_interest_amount_adj").val(),2));
    $("#Port_MEMO_BNK_interest_amount_adj").val(FormatDecimal($("#Port_MEMO_BNK_interest_amount_adj").val(),2));
    $("#Port_MEMO_TRD_interest_amount_adj").val(FormatDecimal($("#Port_MEMO_TRD_interest_amount_adj").val(),2));

    $("#Port_AFS_wht_int_amount_adj").val(FormatDecimal($("#Port_AFS_wht_int_amount_adj").val(),2));
    $("#Port_HTM_wht_int_amount_adj").val(FormatDecimal($("#Port_HTM_wht_int_amount_adj").val(),2));
    $("#Port_TRD_wht_int_amount_adj").val(FormatDecimal($("#Port_TRD_wht_int_amount_adj").val(),2));
    $("#Port_MEMO_BNK_wht_int_amount_adj").val(FormatDecimal($("#Port_MEMO_BNK_wht_int_amount_adj").val(),2));
    $("#Port_MEMO_TRD_wht_int_amount_adj").val(FormatDecimal($("#Port_MEMO_TRD_wht_int_amount_adj").val(),2));
});


$(document).ready(function () {

    $("#ddl_counterparty_payment").click(function () {
        var txt_search = $('#txt_counterparty_payment');
        var trans_deal_type = $('#trans_deal_type').val() === 'LD' ? 'PAY' : 'REV';
        var data = { transdealtype: trans_deal_type };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_counterparty_payment').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    $("#ul_counterparty_payment").on("click", ".searchterm", function (event) {
        $("#ddl_trans_mt_code").find(".selected-value").val(null);
        $("#ddl_trans_mt_code").find(".selected-data").text("Select...");
        $("#mt_code").val(null);
        $("#mt_code").text(null);
    });

    $("#ddl_trans_mt_code").click(function () {
        var txt_search = $('#txt_trans_mt_code');
        var payment_method = $("#payment_method").val();
        var trans_deal_type = $('#trans_deal_type').val() === 'LD' ? 'PAY' : 'REV';
        var data = { paymentmethod: payment_method, transdealtype: trans_deal_type, cur: $("#cur").val() };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_trans_mt_code').keyup(function () {
        var payment_method = $("#payment_method").val();
        var trans_deal_type = $('#trans_deal_type').val() === 'LD' ? 'PAY' : 'REV';
        var data = { paymentmethod: payment_method, transdealtype: trans_deal_type, cur: $("#cur").val() };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    var isUpdate = "@IsUpdate";
    if (isUpdate == "False") {
        $('#ddl_counterparty_payment').attr('disabled', 'disabled');
        $('#ddl_trans_mt_code').attr('disabled', 'disabled');

        $('#Port_AFS_interest_amount_adj').attr('disabled', 'disabled');
        $('#Port_AFS_wht_int_amount_adj').attr('disabled', 'disabled');

        $('#Port_HTM_interest_amount_adj').attr('disabled', 'disabled');
        $('#Port_HTM_wht_int_amount_adj').attr('disabled', 'disabled');

        $('#Port_TRD_interest_amount_adj').attr('disabled', 'disabled');
        $('#Port_TRD_wht_int_amount_adj').attr('disabled', 'disabled');

        $('#Port_MEMO_BNK_interest_amount_adj').attr('disabled', 'disabled');
        $('#Port_MEMO_BNK_wht_int_amount_adj').attr('disabled', 'disabled');

        $('#Port_MEMO_TRD_interest_amount_adj').attr('disabled', 'disabled');
        $('#Port_MEMO_TRD_wht_int_amount_adj').attr('disabled', 'disabled');

        $('#remark').attr('disabled', 'disabled');

        $('#btnSave').attr('disabled', 'disabled');
    }


    $('#btnClear').click(function(e) {
        window.location.href = "/RPCoupon/Index";
    });

    $('#btnSave').click(function(e) {
        var isValid = true;
        var text = "";
        $("#payment_method_error").text("");

        var payment_method = $('#payment_method');
        if (payment_method.val().trim() == "") {
            text += " The Payment Methode is required.";
            payment_method.click();
        }

        var mt_code = $('#mt_code');
        if (mt_code.val().trim() == "") {
            text += " The MT Code is required.";
            if (payment_method.val() != "") {
                mt_code.click();
            }
        }

        if (text != "") {
            $("#payment_method_error").text(text);
            isValid = false;
        }

        if (isValid) {
            swal({
                title: "Comfirm Approve?",
                text: "",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-danger",
                confirmButtonText: "Yes",
                cancelButtonText: "No",
                closeOnConfirm: true,
                closeOnCancel: true
            },
                function(isConfirm) {
                    if (isConfirm) {
                        GM.Message.Clear();
                        $('.spinner').css('display', 'block');
                        var dataToPost = $("#action-form").serialize();
                        $.post("Save", dataToPost)
                            .done(function(response) {
                                GM.Unmask();
                                console.log(response);
                                $('.spinner').css('display', 'none');
                                if (response[0].Message == "") {
                                    setTimeout(function() {
                                        swal({
                                            title: "Complete",
                                            text: "Saved Successfully",
                                            type: "success",
                                            showCancelButton: false,
                                            confirmButtonClass: "btn-danger",
                                            confirmButtonText: "Yes",
                                            cancelButtonText: "No",
                                            closeOnConfirm: true,
                                            closeOnCancel: true
                                        },
                                            function(isConfirm) {
                                                $('.spinner').css('display', 'block');
                                                window.location.href = "/RPCoupon/Index";
                                            });
                                    }, 100);
                                }
                                else {
                                    setTimeout(function() {
                                        swal({
                                            title: "Fail",
                                            text: response[0].Message,
                                            type: "error",
                                            showCancelButton: false,
                                            confirmButtonClass: "btn-danger",
                                            confirmButtonText: "Yes",
                                            cancelButtonText: "No",
                                            closeOnConfirm: true,
                                            closeOnCancel: true
                                        },
                                            function(isConfirm) {
                                                $('.spinner').css('display', 'none');
                                            });
                                    }, 100);
                                }
                            })
                    }
                });
        }
    });
});