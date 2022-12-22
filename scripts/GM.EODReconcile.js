$.ajaxSetup({
    cache: false
});

function checkValue(str, max) {
    if (str.charAt(0) !== '0' || str === '00') {
        var num = parseInt(str);
        if (isNaN(num) || num <= 0 || num > max) num = 1;
        str = num > parseInt(max.toString().charAt(0)) && num.toString().length === 1 ? '0' + num : num.toString();
    }
    return str;
}

function Confirm() {

    setTimeout(function () {

        swal({
            title: "Comfirm Save?",
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

                    var formatmmddyyyydate = $("#asof_date").val().split("/");
                    var formatyyyyMMdd = formatmmddyyyydate[2] + "-" + formatmmddyyyydate[1] + "-" + formatmmddyyyydate[0];


                    $('.spinner').css('display', 'block');
                    $.ajax({
                        url: "/EODReconcile/Save",
                        type: "POST",
                        dataType: "JSON",
                        data: {
                            TRANS_EOD_NO: $('#TRANS_EOD_NO').val(),
                            ASOF_DATE: formatyyyyMMdd,
                            BILATERAL_TRADE_TOTAL: $('#BILATERAL_TRADE_TOTAL').val(),
                            BILATERAL_TRADE_VERIFY: $('#BILATERAL_TRADE_VERIFY').val(),
                            BILATERAL_TRADE_PENDING: $('#BILATERAL_TRADE_PENDING').val(),
                            BILATERAL_TRADE_REMARK: $('#BILATERAL_TRADE_REMARK').val(),
                            BILATERAL_SATTLEMENT_DVP_TOTAL: $('#BILATERAL_SATTLEMENT_DVP_TOTAL').val(),
                            BILATERAL_SATTLEMENT_DVP_PTI: $('#BILATERAL_SATTLEMENT_DVP_PTI').val(),
                            BILATERAL_SATTLEMENT_DVP_PENDING: $('#BILATERAL_SATTLEMENT_DVP_PENDING').val(),
                            BILATERAL_SATTLEMENT_DVP_REMARK: $('#BILATERAL_SATTLEMENT_DVP_REMARK').val(),
                            BILATERAL_SATTLEMENT_RVP_TOTAL: $('#BILATERAL_SATTLEMENT_RVP_TOTAL').val(),
                            BILATERAL_SATTLEMENT_RVP_PTI: $('#BILATERAL_SATTLEMENT_RVP_PTI').val(),
                            BILATERAL_SATTLEMENT_RVP_PENDING: $('#BILATERAL_SATTLEMENT_RVP_PENDING').val(),
                            BILATERAL_SATTLEMENT_RVP_REMARK: $('#BILATERAL_SATTLEMENT_RVP_REMARK').val(),
                            BILATERAL_SATTLEMENT_MT202_TOTAL: $('#BILATERAL_SATTLEMENT_MT202_TOTAL').val(),
                            BILATERAL_SATTLEMENT_MT202_BAHTNET: $('#BILATERAL_SATTLEMENT_MT202_BAHTNET').val(),
                            BILATERAL_SATTLEMENT_MT202_PENDING: $('#BILATERAL_SATTLEMENT_MT202_PENDING').val(),
                            BILATERAL_SATTLEMENT_MT202_REMARK: $('#BILATERAL_SATTLEMENT_MT202_REMARK').val(),
                            PRIVATE_TRADE_TOTAL: $('#PRIVATE_TRADE_TOTAL').val(),
                            PRIVATE_TRADE_VERIFY: $('#PRIVATE_TRADE_VERIFY').val(),
                            PRIVATE_TRADE_PENDING: $('#PRIVATE_TRADE_PENDING').val(),
                            PRIVATE_TRADE_REMARK: $('#PRIVATE_TRADE_REMARK').val(),
                            PRIVATE_SATTLEMENT_DVP_TOTAL: $('#PRIVATE_SATTLEMENT_DVP_TOTAL').val(),
                            PRIVATE_SATTLEMENT_DVP_PTI: $('#PRIVATE_SATTLEMENT_DVP_PTI').val(),
                            PRIVATE_SATTLEMENT_DVP_PENDING: $('#PRIVATE_SATTLEMENT_DVP_PENDING').val(),
                            PRIVATE_SATTLEMENT_DVP_REMARK: $('#PRIVATE_SATTLEMENT_DVP_REMARK').val(),
                            PRIVATE_SATTLEMENT_RVP_TOTAL: $('#PRIVATE_SATTLEMENT_RVP_TOTAL').val(),
                            PRIVATE_SATTLEMENT_RVP_PTI: $('#PRIVATE_SATTLEMENT_RVP_PTI').val(),
                            PRIVATE_SATTLEMENT_RVP_PENDING: $('#PRIVATE_SATTLEMENT_RVP_PENDING').val(),
                            PRIVATE_SATTLEMENT_RVP_REMARK: $('#PRIVATE_SATTLEMENT_RVP_REMARK').val(),

                            PRIVATE_SATTLEMENT_DF_TOTAL: $('#PRIVATE_SATTLEMENT_DF_TOTAL').val(),
                            PRIVATE_SATTLEMENT_DF_PTI: $('#PRIVATE_SATTLEMENT_DF_PTI').val(),
                            PRIVATE_SATTLEMENT_DF_PENDING: $('#PRIVATE_SATTLEMENT_DF_PENDING').val(),
                            PRIVATE_SATTLEMENT_DF_REMARK: $('#PRIVATE_SATTLEMENT_DF_REMARK').val(),
                            PRIVATE_SATTLEMENT_RF_TOTAL: $('#PRIVATE_SATTLEMENT_RF_TOTAL').val(),
                            PRIVATE_SATTLEMENT_RF_PTI: $('#PRIVATE_SATTLEMENT_RF_PTI').val(),
                            PRIVATE_SATTLEMENT_RF_PENDING: $('#PRIVATE_SATTLEMENT_RF_PENDING').val(),
                            PRIVATE_SATTLEMENT_RF_REMARK: $('#PRIVATE_SATTLEMENT_RF_REMARK').val(),

                            PRIVATE_SATTLEMENT_MT103_TOTAL: $('#PRIVATE_SATTLEMENT_MT103_TOTAL').val(),
                            PRIVATE_SATTLEMENT_MT103_BAHTNET: $('#PRIVATE_SATTLEMENT_MT103_BAHTNET').val(),
                            PRIVATE_SATTLEMENT_MT103_PENDING: $('#PRIVATE_SATTLEMENT_MT103_PENDING').val(),
                            PRIVATE_SATTLEMENT_MT103_REMARK: $('#PRIVATE_SATTLEMENT_MT103_REMARK').val(),

                            BILATERAL_MARGIN_PAY_TOTAL: $('#BILATERAL_MARGIN_PAY_TOTAL').val(),
                            BILATERAL_MARGIN_PAY_BAHTNET: $('#BILATERAL_MARGIN_PAY_BAHTNET').val(),
                            BILATERAL_MARGIN_PAY_PENDING: $('#BILATERAL_MARGIN_PAY_PENDING').val(),
                            BILATERAL_MARGIN_PAY_REMARK: $('#BILATERAL_MARGIN_PAY_REMARK').val(),
                            BILATERAL_MARGIN_RECEIVE_TOTAL: $('#BILATERAL_MARGIN_RECEIVE_TOTAL').val(),
                            BILATERAL_MARGIN_RECEIVE_MANUAL: $('#BILATERAL_MARGIN_RECEIVE_MANUAL').val(),
                            BILATERAL_MARGIN_RECEIVE_PENDING: $('#BILATERAL_MARGIN_RECEIVE_PENDING').val(),
                            BILATERAL_MARGIN_RECEIVE_REMARK: $('#BILATERAL_MARGIN_RECEIVE_REMARK').val(),
                            PRIVATE_MARGIN_PAY_TOTAL: $('#PRIVATE_MARGIN_PAY_TOTAL').val(),
                            PRIVATE_MARGIN_PAY_BAHTNET: $('#PRIVATE_MARGIN_PAY_BAHTNET').val(),
                            PRIVATE_MARGIN_PAY_PENDING: $('#PRIVATE_MARGIN_PAY_PENDING').val(),
                            PRIVATE_MARGIN_PAY_REMARK: $('#PRIVATE_MARGIN_PAY_REMARK').val(),
                            PRIVATE_MARGIN_RECEIVE_TOTAL: $('#PRIVATE_MARGIN_RECEIVE_TOTAL').val(),
                            PRIVATE_MARGIN_RECEIVE_MANUAL: $('#PRIVATE_MARGIN_RECEIVE_MANUAL').val(),
                            PRIVATE_MARGIN_RECEIVE_PENDING: $('#PRIVATE_MARGIN_RECEIVE_PENDING').val(),
                            PRIVATE_MARGIN_RECEIVE_REMARK: $('#PRIVATE_MARGIN_RECEIVE_REMARK').val()
                        },
                        //async: false,
                        success: function (result) {
                            if (result.Success) {
                                setTimeout(
                                    function () {
                                        swal({
                                            title: "Complete",
                                            text: "Save Successfully ",
                                            type: "success",
                                            showCancelButton: false,
                                            confirmButtonClass: "btn-success",
                                            confirmButtonText: "Ok",
                                            closeOnConfirm: true
                                        },
                                            function () {
                                                $('.spinner').css('display', 'none'); // Close Loading
                                                Search();
                                            });
                                    }, 100);
                            } else {
                                setTimeout(function () {
                                    swal("Fail", result.Message, "error");
                                    $('.spinner').css('display', 'none'); // Close Loading
                                }, 100);
                            }
                        }
                    }
                    );
                } else {
                    $('.spinner').css('display', 'none'); // Close Loading
                }
            });
    }, 100);
}

function Search() {

    var momentDate = moment($('#asof_date').val(), "DD/MM/YYYY");

    setTimeout(function () {
        $('.spinner').css('display', 'block'); // Open Loading
    }, 50);

    $('#action-form :checkbox:enabled').prop('checked', false);

    $.ajax({
        url: "/EODReconcile/GET",
        type: "GET",
        dataType: "JSON",
        data: {
            asofDate: momentDate.format("YYYYMMDD")
        },
        async: false,
        success: function (result) {
            if (result.Success) {
                $('#TRANS_EOD_NO').val(result.Data.RPEodReconcileResultModel[0].TRANS_EOD_NO);
                $('#ASOF_DATE').val(result.Data.RPEodReconcileResultModel[0].ASOF_DATE);
                $('#BILATERAL_TRADE_TOTAL').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_TRADE_TOTAL);
                $('#BILATERAL_TRADE_VERIFY').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_TRADE_VERIFY);
                $('#BILATERAL_TRADE_PENDING').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_TRADE_PENDING);
                $('#BILATERAL_TRADE_REMARK').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_TRADE_REMARK);
                $('#BILATERAL_SATTLEMENT_DVP_TOTAL').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_SATTLEMENT_DVP_TOTAL);
                $('#BILATERAL_SATTLEMENT_DVP_PTI').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_SATTLEMENT_DVP_PTI);
                $('#BILATERAL_SATTLEMENT_DVP_PENDING').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_SATTLEMENT_DVP_PENDING);
                $('#BILATERAL_SATTLEMENT_DVP_REMARK').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_SATTLEMENT_DVP_REMARK);
                $('#BILATERAL_SATTLEMENT_RVP_TOTAL').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_SATTLEMENT_RVP_TOTAL);
                $('#BILATERAL_SATTLEMENT_RVP_PTI').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_SATTLEMENT_RVP_PTI);
                $('#BILATERAL_SATTLEMENT_RVP_PENDING').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_SATTLEMENT_RVP_PENDING);
                $('#BILATERAL_SATTLEMENT_RVP_REMARK').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_SATTLEMENT_RVP_REMARK);
                $('#BILATERAL_SATTLEMENT_MT202_TOTAL').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_SATTLEMENT_MT202_TOTAL);
                $('#BILATERAL_SATTLEMENT_MT202_BAHTNET').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_SATTLEMENT_MT202_BAHTNET);
                $('#BILATERAL_SATTLEMENT_MT202_PENDING').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_SATTLEMENT_MT202_PENDING);
                $('#BILATERAL_SATTLEMENT_MT202_REMARK').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_SATTLEMENT_MT202_REMARK);
                $('#PRIVATE_TRADE_TOTAL').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_TRADE_TOTAL);
                $('#PRIVATE_TRADE_VERIFY').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_TRADE_VERIFY);
                $('#PRIVATE_TRADE_PENDING').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_TRADE_PENDING);
                $('#PRIVATE_TRADE_REMARK').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_TRADE_REMARK);
                $('#PRIVATE_SATTLEMENT_DVP_TOTAL').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_SATTLEMENT_DVP_TOTAL);
                $('#PRIVATE_SATTLEMENT_DVP_PTI').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_SATTLEMENT_DVP_PTI);
                $('#PRIVATE_SATTLEMENT_DVP_PENDING').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_SATTLEMENT_DVP_PENDING);
                $('#PRIVATE_SATTLEMENT_DVP_REMARK').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_SATTLEMENT_DVP_REMARK);
                $('#PRIVATE_SATTLEMENT_RVP_TOTAL').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_SATTLEMENT_RVP_TOTAL);
                $('#PRIVATE_SATTLEMENT_RVP_PTI').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_SATTLEMENT_RVP_PTI);
                $('#PRIVATE_SATTLEMENT_RVP_PENDING').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_SATTLEMENT_RVP_PENDING);
                $('#PRIVATE_SATTLEMENT_RVP_REMARK').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_SATTLEMENT_RVP_REMARK);

                //Add for Net Settlement
                $('#PRIVATE_SATTLEMENT_DF_TOTAL').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_SATTLEMENT_DF_TOTAL);
                $('#PRIVATE_SATTLEMENT_DF_PTI').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_SATTLEMENT_DF_PTI);
                $('#PRIVATE_SATTLEMENT_DF_PENDING').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_SATTLEMENT_DF_PENDING);
                $('#PRIVATE_SATTLEMENT_DF_REMARK').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_SATTLEMENT_DF_REMARK);
                $('#PRIVATE_SATTLEMENT_RF_TOTAL').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_SATTLEMENT_RF_TOTAL);
                $('#PRIVATE_SATTLEMENT_RF_PTI').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_SATTLEMENT_RF_PTI);
                $('#PRIVATE_SATTLEMENT_RF_PENDING').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_SATTLEMENT_RF_PENDING);
                $('#PRIVATE_SATTLEMENT_RF_REMARK').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_SATTLEMENT_RF_REMARK);
                $('#PRIVATE_SATTLEMENT_MT103_TOTAL').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_SATTLEMENT_MT103_TOTAL);
                $('#PRIVATE_SATTLEMENT_MT103_BAHTNET').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_SATTLEMENT_MT103_BAHTNET);
                $('#PRIVATE_SATTLEMENT_MT103_PENDING').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_SATTLEMENT_MT103_PENDING);
                $('#PRIVATE_SATTLEMENT_MT103_REMARK').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_SATTLEMENT_MT103_REMARK);

                $('#BILATERAL_MARGIN_PAY_TOTAL').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_MARGIN_PAY_TOTAL);
                $('#BILATERAL_MARGIN_PAY_BAHTNET').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_MARGIN_PAY_BAHTNET);
                $('#BILATERAL_MARGIN_PAY_PENDING').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_MARGIN_PAY_PENDING);
                $('#BILATERAL_MARGIN_PAY_REMARK').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_MARGIN_PAY_REMARK);
                $('#BILATERAL_MARGIN_RECEIVE_TOTAL').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_MARGIN_RECEIVE_TOTAL);
                $('#BILATERAL_MARGIN_RECEIVE_MANUAL').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_MARGIN_RECEIVE_MANUAL);
                $('#BILATERAL_MARGIN_RECEIVE_PENDING').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_MARGIN_RECEIVE_PENDING);
                $('#BILATERAL_MARGIN_RECEIVE_REMARK').val(result.Data.RPEodReconcileResultModel[0].BILATERAL_MARGIN_RECEIVE_REMARK);
                $('#PRIVATE_MARGIN_PAY_TOTAL').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_MARGIN_PAY_TOTAL);
                $('#PRIVATE_MARGIN_PAY_BAHTNET').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_MARGIN_PAY_BAHTNET);
                $('#PRIVATE_MARGIN_PAY_PENDING').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_MARGIN_PAY_PENDING);
                $('#PRIVATE_MARGIN_PAY_REMARK').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_MARGIN_PAY_REMARK);
                $('#PRIVATE_MARGIN_RECEIVE_TOTAL').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_MARGIN_RECEIVE_TOTAL);
                $('#PRIVATE_MARGIN_RECEIVE_MANUAL').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_MARGIN_RECEIVE_MANUAL);
                $('#PRIVATE_MARGIN_RECEIVE_PENDING').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_MARGIN_RECEIVE_PENDING);
                $('#PRIVATE_MARGIN_RECEIVE_REMARK').val(result.Data.RPEodReconcileResultModel[0].PRIVATE_MARGIN_RECEIVE_REMARK);

                if (result.Data.RPEodReconcileResultModel[0].IS_SAVE) {
                    $('#action-form :input').each(function () {
                        var input = $(this);
                        if (input.attr("type") !== "button" && input.attr("type") !== "checkbox"
                            && input.attr("id") !== "asof_date") {
                            input.prop('disabled', true);
                        }
                    });
                    document.getElementById("btnConfirm").style.visibility = "hidden";

                } else {
                    $('#action-form :input').each(function () {
                        var input = $(this);
                        if (input.attr("id") !== undefined) {
                            var x = document.getElementById(input.attr("id"));
                            if (document.getElementById(input.attr("id")).nodeName === 'TEXTAREA' || input.attr("type") === "number") {
                                input.removeProp('disabled');
                            }
                        }
                    });
                    document.getElementById("btnConfirm").style.visibility = "visible";
                }

                setTimeout(function () {
                    $('.spinner').css('display', 'none'); // Open Loading
                }, 400);
            } else {
                console.log("ERROR : " + result.Message);
                setTimeout(function () {
                    $('.spinner').css('display', 'none'); // Open Loading
                }, 400);
            }
        }
    });
}

$(window).on('load', function () {

    //var momentDate = moment($("#BusinessDate").text(), "DD/MM/YYYY");

    //$('#asof_date').data("DateTimePicker").date(momentDate);

    $('#asof_date').val($("#BusinessDate").text());

    //$('#asof_date').val(momentDate.format("DD/MM/YYYY"));

    var date = document.getElementById('asof_date');

    date.addEventListener('input', function (e) {
        this.type = 'text';
        var input = this.value;
        if (/\D\/$/.test(input)) input = input.substr(0, input.length - 3);
        var values = input.split('/').map(function (v) {
            return v.replace(/\D/g, '');
        });

        if (values[0]) values[0] = checkValue(values[0], 31);
        if (values[1]) values[1] = checkValue(values[1], 12);
        var output = values.map(function (v, i) {
            return v.length === 2 && i < 2 ? v + '/' : v;
        });
        this.value = output.join('').substr(0, 14);
    });

    setTimeout(function () {
        Search();
    }, 100);


    $('.onoffswitch-label').click(function () {
        $(this).parent().toggleClass('onoffswitch-checked');
    });

});