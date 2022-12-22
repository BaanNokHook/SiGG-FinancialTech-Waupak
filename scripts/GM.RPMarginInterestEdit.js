

function text_OnKeyPress_NumberOnlyAndDotAndMinus(obj) {
   
    var keyAble = false;
    try {
        key = window.event.keyCode;
        var beforeVal = obj.value;
        if (beforeVal.indexOf('.') !== -1) {
            if (key === 46) {
                keyAble = false;
            } else if (key > 47 && key < 58) {
                keyAble = true; // if so, do nothing
            }
            else if (key === 45) {
                if (!beforeVal.indexOf('-') !== -1 && obj.selectionStart === 0) {
                    keyAble = true;
                } else {
                    keyAble = false;
                }
            }
            else {
                keyAble = false;
            }
        }
        else if ((key === 46) || (key > 47 && key < 58)) {
            keyAble = true; // if so, do nothing
        } else if (key === 45) {
            if (!beforeVal.indexOf('-') !== -1 && obj.selectionStart === 0) {
                keyAble = true;
            } else {
                keyAble = false;
            }
        }
        else {
            keyAble = false;
        }
    } catch (err) {
        throw new Error(" error in sub text_OnKeyPress_NumberOnlyAndDotAndMinus function() cause = " + err.message);
    }
    return keyAble;
}

$(document).ready(function () {
    //CounterpartyPayment Dropdown
    $("#ddl_counterparty_payment").click(function () {
        var txt_search = $('#txt_counterparty_payment');
        var counterparty_id = $("#counter_party_id").val();
        var payment_flag = null;
        if ($("#trans_deal_type").val() === localStorage.getItem('TRANS_DEAL_TYPE_BORROWING')) {
            payment_flag = 1;//sell
        }
        else {
            payment_flag = 2;//buy
        }

        var data = { counterpartyid: counterparty_id, payment_flag: payment_flag };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#ul_counterparty_payment").on("click", ".searchterm", function (event) {
        $("#ddl_trans_mt_code").find(".selected-data").text("Select...");
    });

    //End CounterpartyPayment Dropdown


    //Trans MT Code Dropdown
    $("#ddl_trans_mt_code").click(function () {
        var txt_search = $('#txt_trans_mt_code');
        var trans_deal_type = $("#trans_deal_type").val();
        var payment_method = $("#payment_method").val();
        var data = { paymentmethod: payment_method, transdealtype: trans_deal_type };

        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_trans_mt_code').keyup(function () {

        //if (this.value.length > 0) {
        var trans_deal_type = $("#trans_deal_type").val();
        var payment_method = $("#payment_method").val();
        var data = { paymentmethod: payment_method, transdealtype: trans_deal_type };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });

    //End Trans MT Code Dropdown
});