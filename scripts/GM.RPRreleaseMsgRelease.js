
function autoMTCode() {
    let data = {
        payment_method: $("#payment_method").val(),
        trans_deal_type: $("#Txt_trans_deal_type").val(),
        cur: $('#cur').val(),
        repo_deal_type: $('#Txt_repo_deal_type').val()
    };

    $.ajax({
        url: "/RPDealSettlement/FillTransMtCode",
        type: "GET",
        dataType: "JSON",
        data: data,
        success: function (res) {
            var text = "None";
            var value = null;
            if (res.length <= 0) {
                $("#trans_mt_code").val(value);
                $("#trans_mt_code").val(text);
                $("#ddl_trans_mt_code").find(".selected-data").text(text);
                $("#ddl_trans_mt_code").find(".selected-data").val(value);
                $("#ddl_trans_mt_code").find(".selected-value").val(value);
            }
        }
    });
}


function CheckPaymentMethod() {
    $('.spinner').css('display', 'block'); // Open Loading

    var v_from_page;
    var v_payment_method = $("#payment_method").val();
    var v_mt_code = $("#trans_mt_code").val();
    var event = 'TRANS';

    if ($("#Hid_trans_state").val() == "BO-Settlement"
        || $("#Hid_trans_state").val() == "Bo-RejectMaturity"
        || $("#Hid_trans_state").val() == "FO-RejectApprove") {
        v_from_page = 'Settlement';
    }
    else if ($("#Hid_trans_state").val() == "BO-Maturity") {
        v_from_page = 'Maturity';
    }

    setTimeout(
        function () {
            var DataChkPayment = {
                from_page: v_from_page,
                event_type: event,
                trans_deal_type: $("#Txt_trans_deal_type").val(),
                payment_method: v_payment_method,
                mt_code: v_mt_code,
                repo_deal_type: $('#Txt_repo_deal_type').val()
            };

            $.ajax({
                url: "/RPReleaseMessage/CheckPaymentMethodFromAjax",
                type: "GET",
                dataType: "JSON",
                data: DataChkPayment,
                success: function (res) {
                    if (res.length > 0 && res[0].is_payment == false) {
                        $('#Btn_Approve').prop('disabled', true);
                    }
                    else {
                        $('#Btn_Approve').prop('disabled', false);
                    }
                }
            });
        }, 500);


    $('.spinner').css('display', 'none'); // Close Loading
}

function roundTo8(num) {
    return +(Math.round(parseFloat(num).toFixed(9) + "e+8") + "e-8");
}

function roundTo2(num) {
    return +(Math.round(parseFloat(num).toFixed(3) + "e+2") + "e-2");
}

function roundTo16(num) {
    return +(Math.round(parseFloat(num).toFixed(17) + "e+16") + "e-16");
}

function roundTo6(num) {
    return +(Math.round(parseFloat(num).toFixed(7) + "e+6") + "e-6");
}

function FormatDecimal16(num) {
    var format = Number(parseFloat(roundTo6(num)).toFixed(6)).toLocaleString('en', {
        minimumFractionDigits: 8
    });
    return format;
}

function FormatDecimal(Num) {
    var format = Number(parseFloat(roundTo8(Num)).toFixed(8)).toLocaleString('en', {
        minimumFractionDigits: 8
    });
    return format;
}

function FormatDecimal2(Num) {
    var format = Number(parseFloat(roundTo2(Num)).toFixed(2)).toLocaleString('en', {
        minimumFractionDigits: 2
    });
    return format;
}

function FormatDecimal6(Num) {
    var format = Number(parseFloat(roundTo6(Num)).toFixed(6)).toLocaleString('en', {
        minimumFractionDigits: 6
    });
    return format;
}

function FormatDecimal0(Num) {
    var format = Number(parseFloat(Num).toFixed(0)).toLocaleString('en', {
        minimumFractionDigits: 0
    });
    return format;
}

function ValidateInput() {

    var payment_method = $('#payment_method');
    if (payment_method.val() == '') {
        $("#payment_method_error").text("The Payment Method field is required.");
        return false;
    }
    else {
        $("#payment_method_error").text("");
    }

    var trans_mt_code = $('#trans_mt_code');
    if (trans_mt_code.val() == '') {
        $("#payment_method_error").text("The Payment Method field is required.");
        return false;
    }
    else {
        $("#payment_method_error").text("");
    }

    var margins_payment_method = $('#margins_payment_method');
    if (margins_payment_method.val() == '') {
        $("#margins_payment_method_error").text("The MGP Method field is required.");
        return false;
    }
    else {
        $("#margins_payment_method_error").text("");
    }

    var margins_mt_code = $('#margins_mt_code');
    if (margins_mt_code.val() == '') {
        $("#margins_payment_method_error").text("The MGP Method field is required.");
        return false;
    }
    else {
        $("#margins_payment_method_error").text("");
    }

    return true;
}

$(document).ready(function () {

    $("#NavBar").html($('#NavRPReleaseMessage').val());

    //if ($('#trans_mt_code').val() === 'MT202') {
    //    $('#divSwiftChannel').css('display', 'block');
    //} else {
    //    $('#divSwiftChannel').css('display', 'none');
    //}

    if ($('#trans_mt_code').val().indexOf('202') == -1) {
        $('#divSwiftChannel').css('display', 'none');
    } else {
        $('#divSwiftChannel').css('display', 'block');
    }

    CheckPaymentMethod();

    //autoMTCode();

    $("#ddl_channel").click(function () {
        var txt_search = $('#txt_channel');
        var data = { datastr: $('#cur').val()};
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    //$('#txt_channel').keyup(function () {
    //    var data = { datastr: this.value };
    //    GM.Utility.DDLAutoComplete(this, data, null, false);
    //});

    $('#remark-deal').click(function (e) {
        var expand = $("div#remark-deal-icon").attr("aria-expanded");
        if (expand == "true") {
            $("#remark-deal-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        } else {
            $("#remark-deal-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }
    });

    var Hid_Previous = $('#Hid_Previous');
    if (Hid_Previous.val() == false) {
        $('#Btn_Previous').prop('disabled', true);
    }
    else {
        $('#Btn_Previous').prop('disabled', false);
    }

    GM.RPReleaseMessage = {};
    GM.RPReleaseMessage.Previous = function (btn) {
        $('.spinner').css('display', 'block'); // Open Loading
        var mode = $(btn).attr("form-mode");
        GM.Message.Clear();
        window.location.href = "/RPReleaseMessage/Previous?id=" + $('#Lab_trans_no').html();
    };

    var Hid_Next = $('#Hid_Next');
    if (Hid_Next.val() == false) {
        $('#Btn_Next').prop('disabled', true);
    }
    else {
        $('#Btn_Next').prop('disabled', false);
    }

    GM.RPReleaseMessage.Next = function (btn) {
        $('.spinner').css('display', 'block'); // Open Loading
        GM.Message.Clear();
        window.location.href = "/RPReleaseMessage/Next?id=" + $('#Lab_trans_no').html();
    };

    GM.RPReleaseMessage.Release = function (btn) {

        if ($("#trans_mt_code").val() === "" || $("#trans_mt_code").val() === null) {
            setTimeout(function () {
                swal("Fail", "The MT Code field is required.", "error");
                return false;
            }, 100);
        } else if ($("#trans_mt_code").val() == 'MT202' && ( $("#swift_channel").val() === "" || $("#swift_channel").val() === null)) {
            setTimeout(function () {
                swal("Fail", "The Swift Channel field is required.", "error");
                return false;
            }, 100);
        }
        else {

            var msg = "Comfirm ReleaseMessage?";
            if ($('#release_printed').val() == 'Y') {
                msg = "Message already released, do you want to release again?";
            }

            swal({
                title: msg,
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
                        $('.spinner').css('display', 'block'); // Open Loading
                        var dataToPost = $("#action-form").serialize();
                        if ($("#payment_method").val() != 'DVP/RVP') {
                            if ($('#trans_mt_code').val().indexOf('202') == -1) {
                                $.post("ReleaseMessage", dataToPost)
                                    .done(function (response) {
                                        $('.spinner').css('display', 'none'); // Close Loading
                                        GM.Unmask();
                                        console.log(response);
                                        console.log(response[0].Success);
                                        if (response[0].Success) {
                                            if (response[0].trans_no != "") {
                                                console.log("Release");
                                                setTimeout(function () {
                                                    swal({
                                                        title: "Complete",
                                                        text: "Release Message Success",
                                                        type: "success",
                                                        showCancelButton: false,
                                                        confirmButtonClass: "btn-success",
                                                        confirmButtonText: "Ok",
                                                        closeOnConfirm: true,
                                                    },
                                                        function () {
                                                            window.location.href = "/RPReleaseMessage/Release?trans_no=" + response[0].trans_no;
                                                        });
                                                }, 100);
                                            }
                                            else {
                                                console.log("Index");
                                                setTimeout(function () {
                                                    swal({
                                                        title: "Complete",
                                                        text: "Release Message Success",
                                                        type: "success",
                                                        showCancelButton: false,
                                                        confirmButtonClass: "btn-success",
                                                        confirmButtonText: "Ok",
                                                        closeOnConfirm: true,
                                                    },
                                                        function () {
                                                            window.location.href = "/RPReleaseMessage/Index?search=" + $('#event_type').val();
                                                        }
                                                    );
                                                }, 100);
                                            }
                                        }
                                        else {
                                            setTimeout(function () {
                                                swal("Fail", response[0].Message, "error");
                                            }, 100);
                                        }
                                    });
                            } else {
                                $.post("CheckSettlementStatus", dataToPost)
                                    .done(function (result) {
                                        if (result.Success) {
                                            if (result.Message === "" && result.Data != null && result.Data.ResponseBody != null && result.Data.ResponseBody.Count > 0
                                                && result.Data.ResponseBody[0].SettlementStatus === localStorage.getItem('SettlementStatus.Complete')) {
                                                setTimeout(function () {
                                                    swal("Fail", localStorage.getItem('CyberPay.ErrorMsg.Complete'), "error");
                                                }, 100);
                                                $('.spinner').css('display', 'none');
                                            } else {
                                                var url = $('#swift_channel').val() === "SMD" || $('#swift_channel').val() == "" ? "ReleaseMessage" : "ReleaseMessageCyberPay";
                                                $.post(url, dataToPost)
                                                    .done(function (response) {
                                                        $('.spinner').css('display', 'none'); // Close Loading
                                                        GM.Unmask();
                                                        console.log(response);
                                                        console.log(response[0].Success);
                                                        if (response[0].Success) {
                                                            if (response[0].trans_no != "") {
                                                                console.log("Release");
                                                                setTimeout(function () {
                                                                    swal({
                                                                        title: "Complete",
                                                                        text: "Release Message Success",
                                                                        type: "success",
                                                                        showCancelButton: false,
                                                                        confirmButtonClass: "btn-success",
                                                                        confirmButtonText: "Ok",
                                                                        closeOnConfirm: true,
                                                                    },
                                                                        function () {
                                                                            window.location.href = "/RPReleaseMessage/Release?trans_no=" + response[0].trans_no;
                                                                        });
                                                                }, 100);
                                                            }
                                                            else {
                                                                console.log("Index");
                                                                setTimeout(function () {
                                                                    swal({
                                                                        title: "Complete",
                                                                        text: "Release Message Success",
                                                                        type: "success",
                                                                        showCancelButton: false,
                                                                        confirmButtonClass: "btn-success",
                                                                        confirmButtonText: "Ok",
                                                                        closeOnConfirm: true,
                                                                    },
                                                                        function () {
                                                                            window.location.href = "/RPReleaseMessage/Index?search=" + $('#event_type').val();
                                                                        }
                                                                    );
                                                                }, 100);
                                                            }
                                                        }
                                                        else {
                                                            setTimeout(function () {
                                                                swal("Fail", response[0].Message, "error");
                                                            }, 100);
                                                        }
                                                    });
                                            }
                                        } else {
                                            setTimeout(function () {
                                                swal("Fail", result.Message, "error");
                                            }, 100);
                                            $('.spinner').css('display', 'none'); // Close Loading
                                        }
                                    });
                            }
                         
                        } else {

                            $.post("ReleaseMessage", dataToPost)
                                .done(function (response) {
                                    $('.spinner').css('display', 'none'); // Close Loading
                                    GM.Unmask();
                                    console.log(response);

                                    if (response[0].Message == "") {
                                        download(response[0].file_content, response[0].file_name, "text/xml");
                                        if (response[0].trans_no != "") {
                                            console.log("Release");
                                            setTimeout(function () {
                                                swal({
                                                    title: "Complete",
                                                    text: "Release Message Success",
                                                    type: "success",
                                                    showCancelButton: false,
                                                    confirmButtonClass: "btn-success",
                                                    confirmButtonText: "Ok",
                                                    closeOnConfirm: true,
                                                },
                                                    function (isConfirm) {
                                                        if (isConfirm) {
                                                            window.location.href = "/RPReleaseMessage/Release?trans_no=" + response[0].trans_no;
                                                        }
                                                    }
                                                );
                                            }, 100);
                                        }
                                        else {
                                            console.log("Index");
                                            setTimeout(function () {
                                                swal({
                                                    title: "Complete",
                                                    text: "Release Message Success",
                                                    type: "success",
                                                    showCancelButton: false,
                                                    confirmButtonClass: "btn-success",
                                                    confirmButtonText: "Ok",
                                                    closeOnConfirm: true,
                                                },
                                                    function (isConfirm) {
                                                        if (isConfirm) {
                                                            window.location.href = "/RPReleaseMessagePti/Index?search=" + ($('#Hid_trans_state').val() === 'BO-Maturity' ? 'Maturity' : 'Settlement');
                                                        }
                                                    }
                                                );
                                            }, 100);
                                        }
                                    }
                                    else {
                                        setTimeout(function () {
                                            swal("Fail", response.Message, "error");
                                        }, 100);
                                    }
                                });
                        }
                    }
                });
        }
    };

    GM.RPReleaseMessage.Table = $('#x-table-data').DataTable({
        dom: 'Bfrtip',
        searching: true,
        scrollY: '15vh',
        scrollX: true,
        order: [
            [1, "desc"]
        ],
        buttons: [],
        processing: true,
        serverSide: true,
        ajax: {
            "url": "/RPReleaseMessage/Search_Colateral",
            "type": "POST",
            "data": { "trans_no": $('#Lab_trans_no').html() }
        },
        columnDefs:
            [
                { targets: 0, data: "RowNumber", orderable: false },
                { targets: 1, data: "trans_no", visible: false },
                { targets: 2, data: "instrument_code" },
                { targets: 3, data: "isin_code" },
                { targets: 4, data: "port" },
                { targets: 5, data: "repo_deal_type", visible: false },
                { targets: 6, data: "cur" },
                {
                    targets: 7, data: "ytm",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return FormatDecimal6(data);
                        }
                        return FormatDecimal6(0);
                    }
                },
                {
                    targets: 8, data: "clean_price",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return FormatDecimal6(data);
                        }
                        return FormatDecimal6(0);
                    }
                },
                {
                    targets: 9, data: "dirty_price",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return FormatDecimal6(data);
                        }
                        return FormatDecimal6(0);
                    }
                },
                {
                    targets: 10, data: "haircut",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return FormatDecimal2(data);
                        }
                        return FormatDecimal2(0);
                    }
                },
                {
                    targets: 11, data: "unit",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return FormatDecimal0(data);
                        }
                        return FormatDecimal0(0);
                    }
                },
                {
                    targets: 12, data: "par",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return FormatDecimal2(data);
                        }
                        return FormatDecimal2(0);
                    }
                },
                {
                    targets: 13, data: "market_value",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return FormatDecimal2(data);
                        }
                        return FormatDecimal2(0);
                    }
                },
                {
                    targets: 14, data: "cash_amount",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return FormatDecimal2(data);
                        }
                        return FormatDecimal2(0);
                    }
                },
                {
                    targets: 15, data: "dirty_price_after_hc",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return FormatDecimal2(data);
                        }
                        return FormatDecimal2(0);
                    }
                },
                {
                    targets: 16, data: "interest_amount",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return FormatDecimal2(data);
                        }
                        return '';
                    }
                },
                {
                    targets: 17, data: "wht_amount",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return FormatDecimal2(data);
                        }
                        return '';
                    }
                },
                {
                    targets: 18, data: "temination_value",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return FormatDecimal2(data);
                        }
                        return '';
                    }
                },
                {
                    targets: 19, data: "maturity_date",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                {
                    targets: 20, data: "coupon_rate",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return FormatDecimal6(data);
                        }
                        return FormatDecimal6(0);
                    }
                },
                {
                    targets: 21, data: "par_unit",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return FormatDecimal2(data);
                        }
                        return FormatDecimal2(0);
                    }
                },
                {
                    targets: 22, data: "variation",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return FormatDecimal2(data);
                        }
                        return FormatDecimal2(0);
                    }
                },
                {
                    targets: 23, data: "dm",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return FormatDecimal16(data);
                        }
                        return '';
                    }
                },
                {
                    targets: 24, data: "trade_date", visible: false,
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                {
                    targets: 25, data: "settlement_date", visible: false,
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                }
            ],
        footerCallback: function (row, data, start, end, display) {
            var api = this.api();
            //Remove the formatting to get integer data for summation
            var intVal = function (i) {
                return typeof i === 'string' ?
                    i.replace(/[\$,]/g, '') * 1 :
                    typeof i === 'number' ?
                        i : 0;
            };

            //Total over all pages
            Sum_cash_amount = api
                .column(14)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);
            $("#Sum_cash_amount").html(FormatDecimal2(parseFloat(Sum_cash_amount).toFixed(2)));

            Sum_interest_amount = api
                .column(16)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);
            $("#Sum_interest_amount").html(FormatDecimal2(parseFloat(Sum_interest_amount).toFixed(2)));

            Sum_wht_amount = api
                .column(17)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);
            $("#Sum_wht_amount").html(FormatDecimal2(parseFloat(Sum_wht_amount).toFixed(2)));

            Sum_temination_value = api
                .column(18)
                .data()
                .reduce(function (a, b) {
                    return intVal(a) + intVal(b);
                }, 0);
            $("#Sum_temination_value").html(FormatDecimal2(parseFloat(Sum_temination_value).toFixed(2)));
        }
    });

    //Binding Div Detail
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    };
    GM.RPReleaseMessage.Form = {};
    GM.RPReleaseMessage.Form.DataBinding = function (p) {
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

    //Binding : DDL Payment Method
    $("#ddl_counterparty_payment").click(function () {
        var txt_search = $('#txt_counterparty_payment');
        var counterparty_id = $("#Hid_counter_party_id").val();
        var data = { counterpartyid: counterparty_id };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#ul_counterparty_payment").on("click", ".searchterm", function (event) {
        $("#ddl_trans_mt_code").find(".selected-data").text("Select...");
        CheckPaymentMethod();
    });

    //Binding : DDL trans_mt_code
    $("#ddl_trans_mt_code").click(function () {

        var txt_search = $('#txt_trans_mt_code');
        txt_search.val("");
        var data = {
            payment_method: $('#payment_method').val(),
            trans_deal_type: $('#Txt_trans_deal_type').val(),
            event_type: $('#event_type') != null ? $('#event_type').val().toUpperCase() : $('#event_type').val(),
            cur: $('#cur').val(),
            repo_deal_type: $('#Txt_repo_deal_type').val()
        };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
    });

    $("#ul_trans_mt_code").on("click", ".searchterm", function (event) {
        CheckPaymentMethod();

        if ($('#trans_mt_code').val().indexOf('202') == -1) {
            $('#divSwiftChannel').css('display', 'none');
        } else {
            $('#divSwiftChannel').css('display', 'block');
        }
    });

    //Function : Radio repo_deal_type =====================================
    var Lab_Private = $('#Lab_repo_deal_type_Pri');
    var Radio_Private = $('#repo_deal_type_Pri');
    var Lab_Bilateral = $('#Lab_repo_deal_type_Bil');
    var Radio_Bilateral = $('#repo_deal_type_Bil');

    if ($('#Txt_repo_deal_type').val() === "PRP") {

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

    if ($('#Txt_interest_type').val() === "FIXED") {

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
        $('#Sp_counter_party_fun').html("-");
    }

    //Function : basis_code_name ============================================
    var basis_code_name = $('#Txt_basis_code_name').val();
    if (basis_code_name != undefined && basis_code_name.length != 0) {
        $('#Sp_basis_code_name').html(basis_code_name);
    }
    else {
        $('#Sp_basis_code_name').html("-");
    }

    //Function : trans_deal_type_name ============================================
    var trans_deal_type_name = $('#Txt_trans_deal_type_name').val();
    if (trans_deal_type_name != undefined && trans_deal_type_name.length != 0) {
        $('#Sp_trans_deal_type_name').html(trans_deal_type_name);
    }
    else {
        $('#Sp_trans_deal_type_name').html("-");
    }

    //Function : Payment Method ============================================
    var payment_method = $('#Txt_payment_method').val();
    if (payment_method != undefined && payment_method.length != 0) {
        $('#Sp_payment_method').html(payment_method);
    }
    else {
        $('#Sp_payment_method').html("-");
    }

    var trans_mt_code = $('#Txt_trans_mt_code').val();
    if (trans_mt_code != undefined && trans_mt_code.length != 0) {
        $('#Sp_trans_mt_code').html(trans_mt_code);
    }
    else {
        $('#Sp_trans_mt_code').html("-");
    }

    //Function : MGP Method ============================================
    var margins_payment_method = $('#Txt_margins_payment_method').val();
    if (margins_payment_method != undefined && margins_payment_method.length != 0) {
        $('#Sp_margins_payment_method').html(margins_payment_method);
    }
    else {
        $('#Sp_margins_payment_method').html("-");
    }

    var margins_mt_code = $('#Txt_margins_mt_code').val();
    if (margins_mt_code != undefined && margins_mt_code.length != 0) {
        $('#Sp_margins_mt_code ').html(margins_mt_code);
    }
    else {
        $('#Sp_margins_mt_code').html("-");
    }

    var txt_append_name = $('#txt_append_name').val();
    if (txt_append_name != undefined && txt_append_name.length != 0) {
        $('#Sp_append_name').html(txt_append_name);
    }
    else {
        $('#Sp_append_name').html("-");
    }

    var txt_formula_name = $('#txt_formula_name').val();
    if (txt_formula_name != undefined && txt_formula_name.length != 0) {
        $('#Sp_formula_name').html(txt_formula_name);
    }
    else {
        $('#Sp_formula_name').html("-");
    }

    //Function : cur_pair1 ============================================
    var cur_pair1 = $('#Txt_cur_pair1').val();
    if (cur_pair1 != undefined && cur_pair1.length != 0) {
        $('#Sp_cur_pair1').html(cur_pair1);
    }
    else {
        $('#Sp_cur_pair1').html("-");
    }

    //Function : cur_pair2 ============================================
    var cur_pair2 = $('#Txt_cur_pair2').val();
    if (cur_pair2 != undefined && cur_pair2.length != 0) {
        $('#Sp_cur_pair2').html(cur_pair2);
    }
    else {
        $('#Sp_cur_pair2').html("-");
    }

    //Function : margins_payment_method ============================================
    var margins_payment_method = $('#Txt_margins_payment_method').val();
    var margins_mt_code = $('#Txt_margins_mt_code').val();

    if (margins_payment_method != undefined && margins_payment_method.length != 0) {
        $('#Sp_margins_payment_method').html(margins_payment_method);
    }
    else {
        $('#Sp_margins_payment_method').html("-");
    }

    if (margins_mt_code != undefined && margins_mt_code.length != 0) {
        $('#Sp_margins_mt_code').html(margins_mt_code);
    }
    else {
        $('#Sp_margins_mt_code').html("-");
    }

    //Function : Check interest Period ============================================
    GM.RPReleaseMessage.Interest = {};
    GM.RPReleaseMessage.Interest.Form = $("#check-interest");

    $("#btn_checkinterest").on('click', function () {
        GM.RPReleaseMessage.Interest.Form.modal('toggle');
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

