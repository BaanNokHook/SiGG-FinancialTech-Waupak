


var RPConfirmationIndex = (function ($) {

    var urlGetSignName = '';
    var urlUpdateSignName = '';
    var urlDownloadPDF = '';
    var urlReleaseMessage = '';
    var urlReleaseMessage298 = '';
    var urlGetReleaseMT = '';
    var arrCheckboxList = [];

    var printConfirmBo1By = {
        Id: $('#ddl_print_confirm_bo1_by'),
        init: function () {
            this.bindEventHandler();
        },
        bindEventHandler: function () {
            var txt_search = $('#txt_print_confirm_bo1_by');
            this.Id.click(function () {
                var data = { datastr: null };
                GM.Utility.DDLAutoCompleteSet4Value(txt_search, data, "sign_name_1", "position_name_1", null);
                txt_search.val("");
            });

            txt_search.keyup(function () {
                var data = { datastr: this.value };
                GM.Utility.DDLAutoCompleteSet4Value(this, data, "sign_name_1", "position_name_1", null);
            });

            $("#ul_print_confirm_bo1_by").on("click", ".searchterm", function (event) {
                if (!printConfirmBo1By.getValue().length) {
                    sign_name_1.setValue("");
                    position_name_1.setValue("");
                }
            });
        },
        clearValue: function () {
            this.Id.find(".selected-data").text("Select...");
            this.setValue();
            this.setText();
        },
        setText: function (text) {
            if (text != null && text.length) {
                $("span[name='print_confirm_bo1_by']").text(text);
                $("#print_confirm_bo1_by_name").val(text);
            }
        },
        setValue: function (value) {
            $('#print_confirm_bo1_by').val(value);
        },
        getValue: function () {
            return $.trim($('#print_confirm_bo1_by').val());
        },
        validate: function () {
            var errorId = $("#print_confirm_bo1_by_error");
            if (this.getValue().length) {
                errorId.text("");
                this.Id.removeClass("input-validation-error");
                return true;
            } else {
                errorId.text("Exchange Rate field is required.");
                this.Id.addClass("input-validation-error");
                return false;
            }
        }
    };

    var sign_name_1 = {
        Id: $('#sign_name_1'),
        setValue: function (value) {
            this.Id.val(value);
        },
        getValue: function () {
            return this.Id.val();
        }
    };

    var position_name_1 = {
        Id: $('#position_name_1'),
        setValue: function (value) {
            this.Id.val(value);
        }
    };

    var printConfirmBo2By = {
        Id: $('#ddl_print_confirm_bo2_by'),
        init: function () {
            this.bindEventHandler();
        },
        bindEventHandler: function () {
            var txt_search = $('#txt_print_confirm_bo2_by');
            this.Id.click(function () {
                var data = { datastr: null };
                GM.Utility.DDLAutoCompleteSet4Value(txt_search, data, "sign_name_2", "position_name_2", null);
                txt_search.val("");
            });

            txt_search.keyup(function () {
                var data = { datastr: this.value };
                GM.Utility.DDLAutoCompleteSet4Value(this, data, "sign_name_2", "position_name_2", null);
            });

            $("#ul_print_confirm_bo2_by").on("click", ".searchterm", function (event) {
                if (!printConfirmBo2By.getValue().length) {
                    sign_name_2.setValue("");
                    position_name_2.setValue("");
                }
            });
        },
        clearValue: function () {
            this.Id.find(".selected-data").text("Select...");
            this.setValue();
            this.setText();
        },
        setText: function (text) {
            if (text != null && text.length) {
                $("span[name='print_confirm_bo2_by']").text(text);
                $("#print_confirm_bo2_by_name").val(text);
            }
        },
        setValue: function (value) {
            $('#print_confirm_bo2_by').val(value);
        },
        getValue: function () {
            return $.trim($('#print_confirm_bo2_by').val());
        },
        validate: function () {
            var errorId = $("#print_confirm_bo2_by_error");
            if (this.getValue().length) {
                errorId.text("");
                this.Id.removeClass("input-validation-error");
                return true;
            } else {
                errorId.text("Exchange Rate field is required.");
                this.Id.addClass("input-validation-error");
                return false;
            }
        }
    };

    var sign_name_2 = {
        Id: $('#sign_name_2'),
        setValue: function (value) {
            this.Id.val(value);
        },
        getValue: function () {
            return this.Id.val();
        }
    };

    var position_name_2 = {
        Id: $('#position_name_2'),
        setValue: function (value) {
            this.Id.val(value);
        }
    };

    var setAnnexChkTransNoItems = function (value) {
        arrCheckboxList.push(value);
    };

    var getAnnexChkTransNoItems = function () {
        return arrCheckboxList;
    };

    var clearAnnexChkTransNoItems = function () {
        arrCheckboxList = [];
    };

    var getMtChkTransNoItems = function () {
        var arrCheckboxList = [];
        var checkboxList = $("input[id^='Mt_chkTransNo']:checked");
        if (checkboxList.length) {
            $.each(checkboxList,
                function () {
                    var checkbox = $(this);

                    // Set Item to Array
                    arrCheckboxList.push(checkbox.val());
                });
        }

        return arrCheckboxList;
    };

    var getMtChkTransNoItems298 = function () {
        var arrCheckboxList = [];
        var checkboxList = $("input[id^='mt298_chkTransNo']:checked");
        if (checkboxList.length) {
            $.each(checkboxList,
                function () {
                    var checkbox = $(this);

                    // Set Item to Array
                    arrCheckboxList.push(checkbox.val());
                });
        }

        return arrCheckboxList;
    };

    var downloadPDF = function (arr_trans_no) {
        var url = urlDownloadPDF + "?trans_no=" + arr_trans_no.join(',') + "&print_confirm_bo1_by=" + sign_name_1.getValue() + "&print_confirm_bo2_by=" + sign_name_2.getValue();
        window.open(url, '_blank');
        return;
    };

    var btnDownloadPDF = {
        Id: $('#btnDownloadPDF'),
        init: function () {
            this.bindEventHandler();
        },
        bindEventHandler: function () {
            this.Id.click(function () {
                $('.spinner').css('display', 'block');
                clearAnnexChkTransNoItems();

                var checkboxList = $("input[id^='Annex_chkTransNo']:checked");
                if (checkboxList.length) {
                    $.each(checkboxList,
                        function () {
                            var checkbox = $(this);
                            setAnnexChkTransNoItems(checkbox.val());
                        });

                    modalSignName.open(getAnnexChkTransNoItems());
                } else {
                    swal("Warning", "Please Select [Trans No] To Download", "error");
                    $('.spinner').css('display', 'none');
                }
            });
        }
    };

    var btnPrint = {
        Id: $('#btnPrint'),
        init: function () {
            this.bindEventHandler();
        },
        bindEventHandler: function () {
            this.Id.click(function () {
                var arr_trans_no = getAnnexChkTransNoItems();
                if (arr_trans_no.length) {
                    downloadPDF(arr_trans_no);
                    GM.RPDealSettlement.Form.Search();
                }
            });
        }
    };

    var btnReleaseMsg = {
        Id: $('#btnReleaseMsg'),
        init: function () {
            this.bindEventHandler();
        },
        bindEventHandler: function () {
            this.Id.click(function () {
                var arr_trans_no = getMtChkTransNoItems();
                if (arr_trans_no.length) {
                    swal({
                        title: "Comfirm Release Message?",
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
                                $('.spinner').css('display', 'block');

                                var postData = {
                                    arr_trans_no: arr_trans_no
                                };

                                $.ajax({
                                    url: urlReleaseMessage,
                                    type: 'POST',
                                    contentType: 'application/json; charset=utf-8',
                                    cache: false,
                                    dataType: 'JSON',
                                    traditional: true,
                                    data: JSON.stringify(postData),
                                    success: function (data) {
                                        $('.spinner').css('display', 'none');
                                        if (data.success) {
                                            swal("Successed", "Release Message Success.", "success");
                                            GM.Defer(function () {
                                                GM.RPDealSettlement.Form.Search();
                                            }, 500);

                                        } else {
                                            swal("Failed!", "Error : " + data.Message, "error");
                                        }

                                    },
                                    error: function (jqXhr, textStatus) {
                                        $('.spinner').css('display', 'none');
                                        if (textStatus === "error") {
                                            var objJson = jQuery.parseJSON(jqXhr.responseText);

                                            if (Object.prototype.toString.call(objJson) === '[object Array]' && objJson.length == 0) {
                                                // Array is empty
                                                // Do Something
                                            } else {
                                                var errorMsg = jqXhr.statusText + " " + objJson.Message;
                                                alert("An error occurred, " + errorMsg,
                                                    function (e) {

                                                    },
                                                    {
                                                        ok: "OK",
                                                        classname: "custom-class"
                                                    });
                                            }
                                        }
                                        window.location.href = "/RPConfirmation";
                                    },
                                    statusCode: {
                                        401: function () {
                                            window.location.href = "/RPConfirmation";
                                        }
                                    }
                                });
                            }
                        });
                } else {
                    swal("Warning", "Please Select [Trans No] To Release Message", "error");
                    $('.spinner').css('display', 'none');
                }
            });
        }
    };

    var btnReleaseMsg298 = {
        Id: $('#btnReleaseMsg298'),
        init: function () {
            this.bindEventHandler();
        },
        bindEventHandler: function () {
            this.Id.click(function () {
                var arr_trans_no = getMtChkTransNoItems298();
                if (arr_trans_no.length) {
                    swal({
                        title: "Comfirm Release Message?",
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
                                $('.spinner').css('display', 'block');

                                var postData = {
                                    arr_trans_no: arr_trans_no
                                };

                                $.ajax({
                                    url: urlReleaseMessage298,
                                    type: 'POST',
                                    contentType: 'application/json; charset=utf-8',
                                    cache: false,
                                    dataType: 'JSON',
                                    traditional: true,
                                    data: JSON.stringify(postData),
                                    success: function (data) {
                                        $('.spinner').css('display', 'none');
                                        if (data.success) {
                                            swal("Successed", "Release Message Success.", "success");
                                            GM.Defer(function () {
                                                GM.RPDealSettlement.Form.Search();
                                            }, 500);

                                        } else {
                                            swal("Failed!", "Error : " + data.Message, "error");
                                        }

                                    },
                                    error: function (jqXhr, textStatus) {
                                        $('.spinner').css('display', 'none');
                                        if (textStatus === "error") {
                                            var objJson = jQuery.parseJSON(jqXhr.responseText);

                                            if (Object.prototype.toString.call(objJson) === '[object Array]' && objJson.length == 0) {
                                                // Array is empty
                                                // Do Something
                                            } else {
                                                var errorMsg = jqXhr.statusText + " " + objJson.Message;
                                                alert("An error occurred, " + errorMsg,
                                                    function (e) {

                                                    },
                                                    {
                                                        ok: "OK",
                                                        classname: "custom-class"
                                                    });
                                            }
                                        }
                                        window.location.href = "/RPConfirmation";
                                    },
                                    statusCode: {
                                        401: function () {
                                            window.location.href = "/RPConfirmation";
                                        }
                                    }
                                });
                            }
                        });
                } else {
                    swal("Warning", "Please Select [Trans No] To Release Message 298", "error");
                    $('.spinner').css('display', 'none');
                }
            });
        }
    };

    var modalSignName = {
        Id: $('#modal-sign-name'),
        init: function () {
            printConfirmBo1By.init();
            printConfirmBo2By.init();
            btnPrint.init();
        },
        open: function (arr_trans_no) {
            //clear data
            printConfirmBo1By.clearValue();
            sign_name_1.setValue('');
            position_name_1.setValue('');

            printConfirmBo2By.clearValue();
            sign_name_2.setValue('');
            position_name_2.setValue('');

            var postData = {
                arr_trans_no: arr_trans_no
            };

            $.ajax({
                url: urlGetSignName,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'JSON',
                traditional: true,
                data: JSON.stringify(postData),
                success: function (data) {
                    $('.spinner').css('display', 'none');

                    //set value
                    if (data.print_confirm_bo1_by != null && data.print_confirm_bo1_by.length) {
                        printConfirmBo1By.setText(data.sign_name_1);
                        printConfirmBo1By.setValue(data.sign_name_1);
                        sign_name_1.setValue(data.print_confirm_bo1_by);
                        position_name_1.setValue(data.position_name_1);
                    }

                    if (data.print_confirm_bo2_by != null && data.print_confirm_bo2_by.length) {
                        printConfirmBo2By.setText(data.sign_name_2);
                        printConfirmBo2By.setValue(data.sign_name_2);
                        sign_name_2.setValue(data.print_confirm_bo1_by);
                        position_name_2.setValue(data.position_name_2);
                    }

                    modalSignName.Id.modal('toggle');
                },
                error: function (jqXhr, textStatus) {
                    $('.spinner').css('display', 'none');
                    if (textStatus === "error") {
                        var objJson = jQuery.parseJSON(jqXhr.responseText);

                        if (Object.prototype.toString.call(objJson) === '[object Array]' && objJson.length == 0) {
                            // Array is empty
                            // Do Something
                        } else {
                            var errorMsg = jqXhr.statusText + " " + objJson.Message;
                            alert("An error occurred, " + errorMsg,
                                function (e) {

                                },
                                {
                                    ok: "OK",
                                    classname: "custom-class"
                                });
                        }
                    }
                    window.location.href = "/RPConfirmation";
                },
                statusCode: {
                    401: function () {
                        window.location.href = "/RPConfirmation";
                    }
                }
            });
        }
    };

    var modalReleaseMsg = {
        Id: $('#modal-release-msg'),
        init: function () {
        },
        open: function (id, mt_code) {
            $('.spinner').css('display', 'block');

            var post = {
                trans_no: id,
                event_type: 'CONFIRM',
                mt_code: mt_code
            };

            $.ajax({
                type: "GET",
                url: urlGetReleaseMT,
                content: "application/json; charset=utf-8",
                dataType: "json",
                data: post,
                success: function (res) {
                    $('.spinner').css('display', 'none');

                    var body = $("#modal_release_mt").find("tbody");
                    var html = "";
                    var totalamount = 0;
                    var isin_code = "";
                    body.html("");
                    $.each(res, function (i, resdata) {

                        if (isin_code != resdata.isin_code) {
                            html = "<tr><td><b> ISIN Code:" + $('<div/>').text(resdata.isin_code).html() + "<b></td></tr>";
                            body.append(html);
                        }
                        if (resdata.mt_message !== '' && resdata.mt_message !== null) {
                            html = "<tr><td>" + $('<div/>').text(resdata.mt_message).html() + "</td></tr>";
                        } else {
                            html = "<tr><td>&nbsp;</td></tr>";
                        }
                        body.append(html);
                        isin_code = resdata.isin_code;

                    });

                    modalReleaseMsg.Id.modal('toggle');
                },
                error: function (jqXhr, textStatus) {
                    $('.spinner').css('display', 'none');
                    if (textStatus === "error") {
                        var objJson = jQuery.parseJSON(jqXhr.responseText);

                        if (Object.prototype.toString.call(objJson) === '[object Array]' && objJson.length == 0) {
                            // Array is empty
                            // Do Something
                        } else {
                            var errorMsg = jqXhr.statusText + " " + objJson.Message;
                            alert("An error occurred, " + errorMsg,
                                function (e) {

                                },
                                {
                                    ok: "OK",
                                    classname: "custom-class"
                                });
                        }
                    }
                    window.location.href = "/RPConfirmation";
                },
                statusCode: {
                    401: function () {
                        window.location.href = "/RPConfirmation";
                    }
                }
            });
        }
    };

    return {
        initIndex: function () {
            btnReleaseMsg.init();
            btnReleaseMsg298.init();
            btnDownloadPDF.init();
            modalSignName.init();
        },
        setUrlGetSignName: function (url) {
            urlGetSignName = url;
        },
        setUrlUpdateSignName: function (url) {
            urlUpdateSignName = url;
        },
        setUrlDownloadPDF: function (url) {
            urlDownloadPDF = url;
        },
        setUrlReleaseMessage: function (url) {
            urlReleaseMessage = url;
        },
        setUrlReleaseMessage298: function (url) {
            urlReleaseMessage298 = url;
        },
        setUrlGetReleaseMT: function (url) {
            urlGetReleaseMT = url;
        },
        ReleaseMsgOpen: function (id,mt_code) {
            modalReleaseMsg.open(id, mt_code);
        },
        SignNameOpen: function (id) {
            clearAnnexChkTransNoItems();
            setAnnexChkTransNoItems(id);
            modalSignName.open(getAnnexChkTransNoItems());
        }
    };
})(jQuery);

$(document).ready(function () {
    $("#NavBar").html($('#NavRPConfirmation').val());

    var budate = $("#BusinessDate").text();

    var formatmmddyyyydate = budate.split("/");
    formatmmddyyyydate = formatmmddyyyydate[1] + "/" + formatmmddyyyydate[0] + "/" + formatmmddyyyydate[2];
    var business_date = new Date(formatmmddyyyydate);

    $('#FormSearch_from_trade_date').data("DateTimePicker").date(business_date);
    $('#FormSearch_to_trade_date').data("DateTimePicker").date(business_date);

    GM.RPDealSettlement = {};
    GM.RPDealSettlement.Form = {};
    GM.RPDealSettlement.Form.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            switch (key) {
                case "repo_deal_type_code": GM.RPDealSettlement.Table.columns(5).search($(this).val()); break;
                case "trans_deal_type": GM.RPDealSettlement.Table.columns(6).search($(this).val()); break;
                case "counter_party_code": GM.RPDealSettlement.Table.columns(10).search($(this).val()); break;
                case "from_trade_date": GM.RPDealSettlement.Table.columns(23).search($(this).val()); break;
                case "to_trade_date": GM.RPDealSettlement.Table.columns(24).search($(this).val()); break;
            }
        });

        GM.RPDealSettlement.Table.draw();
    };

    GM.RPDealSettlement.Table = $('#x-table-data').DataTable({
        dom: 'Bfrtip',
        searching: true,
        scrollY: '80vh',
        scrollX: true,
        order: [
            [4, "asc"]
        ],
        buttons: [],
        processing: true,
        serverSide: true,
        ajax: {
            "url": "/RPConfirmation/Search",
            "type": "POST",
            "error": function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR);
                console.log(textStatus);
                console.log(errorThrown);
            }
        },
        columnDefs:
            [
                { targets: 0, data: "RowNumber", className: 'dt-body-center', orderable: false },
                {
                    targets: 1,
                    data: "trans_no",
                    searchable: false,
                    orderable: false,
                    visible: true,
                    className: 'dt-body-center',
                    render: function (data, type, row) {
                        if (row.print_type === 'MT518') {
                            return '<input type="checkbox"' + ' id="Mt_chkTransNo' + data + '" value="' + data + '" class="filter-ck" />';
                        } else {
                            return '<input type="checkbox"' + ' id="Mt_chkTransNo' + data + '" value="' + data + '" class="filter-ck" disabled />';
                        }
                    }
                },
                {
                    targets: 2,
                    data: "trans_no",
                    searchable: false,
                    orderable: false,
                    visible: true,
                    className: 'dt-body-center',
                    render: function (data, type, row) {
                        if (row.print_type === 'ANNEXII' || row.print_type === 'MT298') {
                            return '<input type="checkbox"' + ' id="Annex_chkTransNo' + data + '" value="' + data + '" class="filter-ck" />';
                        } else {
                            return '<input type="checkbox"' + ' id="Annex_chkTransNo' + data + '" value="' + data + '" class="filter-ck" disabled />';
                        }
                    }
                },
                {
                    targets: 3,
                    data: "trans_no",
                    searchable: false,
                    orderable: false,
                    visible: true,
                    className: 'dt-body-center',
                    render: function (data, type, row) {
                        if (row.print_type === 'MT298') {
                            return '<input type="checkbox"' + ' id="mt298_chkTransNo' + data + '" value="' + data + '" class="filter-ck" />';
                        } else {
                            return '<input type="checkbox"' + ' id="mt298_chkTransNo' + data + '" value="' + data + '" class="filter-ck" disabled />';
                        }
                    }
                },
                { targets: 4, data: "trans_no" },
                { targets: 5, data: "repo_deal_type" },
                { targets: 6, data: "trans_deal_type_name" },
                { targets: 7, data: "trans_type" },
                { targets: 8, data: "port" },
                { targets: 9, data: "purpose" },
                { targets: 10, data: "counter_party_name" },
                {
                    targets: 11, data: "trade_date",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                {
                    targets: 12, data: "settlement_date",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                {
                    targets: 13, data: "maturity_date",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                { targets: 14, data: "deal_period" },
                {
                    targets: 15, data: "purchase_price",
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 0);
                        }
                        return html;
                    }
                },
                {
                    targets: 16, data: "repurchase_price",
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        }
                        return html;
                    }
                },
                { targets: 17, data: "cur" },
                { targets: 18, data: "trans_status" },
                { targets: 19, data: "trans_state" },
                { targets: 20, data: "trader_id" },
                { targets: 21, data: "from_trans_no", visible: false },
                { targets: 22, data: "to_trans_no", visible: false },
                { targets: 23, data: "from_trade_date", visible: false },
                { targets: 24, data: "to_trade_date", visible: false },
                { targets: 25, data: "from_settlement_date", visible: false },
                { targets: 26, data: "to_settlement_date", visible: false },
                { targets: 27, data: "from_maturity_date", visible: false },
                { targets: 28, data: "to_maturity_date", visible: false },
                {
                    targets: 29,
                    orderable: false,
                    data: "trans_no",
                    className: "dt-body-center",
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (row.print_type === 'MT518') {

                            html += '<button class="btn btn-default btn-round" form-mode="viewmt" onclick="RPConfirmationIndex.ReleaseMsgOpen(\'' + row.trans_no + '\',\'MT518\')" ><i class="feather-icon icon-message-square"></i></button>';

                        }else if (row.print_type === 'MT298') {

                            html += '<button class="btn btn-default btn-round" form-mode="viewmt" onclick="RPConfirmationIndex.ReleaseMsgOpen(\'' + row.trans_no + '\',\'MT298\')" ><i class="feather-icon icon-message-square"></i></button>';
                            html += '<button class="btn btn-default btn-round" form-mode="viewmt" onclick="RPConfirmationIndex.SignNameOpen(\'' + row.trans_no + '\')" ><i class="feather-icon icon-printer"></i></button>';
                        }else if (row.print_type === 'ANNEXII') {

                            html += '<button class="btn btn-default btn-round" form-mode="viewmt" onclick="RPConfirmationIndex.SignNameOpen(\'' + row.trans_no + '\')" ><i class="feather-icon icon-printer"></i></button>';
                        }
                        return html;
                    }
                }
            ],
        fixedColumns: {
            leftColumns: 4,
            rightColumns: 2
        },
        fnPreDrawCallback: function () {
            $('#x-table-data').DataTable().columns(23).search($('#FormSearch_from_trade_date').val());
            $('#x-table-data').DataTable().columns(24).search($('#FormSearch_to_trade_date').val());
        },
        createdRow: function (row, data, dataIndex) {
            if (data.release_printed == "Y") {
                $('td', row).css('color', '#00B0FF');
            }
        }
    });

    //Function : Checkbox
    $('#x-table-data').on('click', 'tr', function () {

        GM.RPDealSettlement.Table = $('#x-table-data').DataTable();
        var data = GM.RPDealSettlement.Table.row(this).data();

        if (typeof data != 'undefined') {
            if ($(this).find('.filter-ck').prop('checked') == true) {  //update the cell data with the checkbox state
                var id = 'chk_' + data.trans_no;
                var inputyes = $("[id=" + id + "]");
                inputyes.removeAttr('checked');
                inputyes.attr('checked', 'checked');
                inputyes.prop('checked', true);
            } else {
                var id = 'chk_' + data.trans_no;
                var inputno = $("[id=" + id + "]");
                inputno.removeAttr('checked');
                inputno.prop('checked', false);
            }
        }
    });
    //Function : Checkbox All
    $('#x-table-data_wrapper').on("click", '#CheckAll', function () {
        if ($(this).prop('checked') == true) {
            $('.filter-ck').prop('checked', true);
        }
        else {
            $('.filter-ck').prop('checked', false);
        }
    });
    //Function : Btn Settlement
    GM.RPDealSettlement.Settlement = function (btn) {
        var mode = $(btn).attr("form-mode");
        GM.Message.Clear();
        Settlement();
    };


});
//End document.ready

$("#search-form").on('submit', function (e) {
    e.preventDefault();
    GM.Message.Clear();
    GM.RPDealSettlement.Form.Search();
});
$("#btnClearData").on('click', function (e) {
    window.location.href = "/RPConfirmation/Index";
});

//Tran Type Dropdown
$("#ddl_trans_type").click(function () {
    var txt_search = $('#txt_trans_type');
    var data = { datastr: null };
    GM.Utility.DDLAutoComplete(txt_search, data, null);
    txt_search.val("");
});
$('#txt_trans_type').keyup(function () {

    //if (this.value.length > 0) {
    var data = { datastr: this.value };
    GM.Utility.DDLAutoComplete(this, data, null);
    //}
});
//Tran Deal Type Dropdown
$("#ddl_trans_deal_type").click(function () {
    var txt_search = $('#txt_trans_deal_type');
    var data = { datastr: null };
    GM.Utility.DDLAutoComplete(txt_search, data, null);
    txt_search.val("");
});
$('#txt_trans_deal_type').keyup(function () {

    //if (this.value.length > 0) {
    var data = { datastr: this.value };
    GM.Utility.DDLAutoComplete(this, data, null);
    //}
});
//counterparty Dropdown
$("#ddl_counterparty").click(function () {
    var txt_search = $('#txt_counterparty');
    var data = { datastr: null };
    GM.Utility.DDLAutoCompleteSet4Value(txt_search, data, null, null, null);
    txt_search.val("");
});
$('#txt_counterparty').keyup(function () {

    //if (this.value.length > 0) {
    var data = { datastr: this.value };
    GM.Utility.DDLAutoComplete(this, data, null);
    //}
});

//Binding : DDL Event Type
$("#ddl_event_type").click(function () {
    var txt_search = $('#txt_event_type');
    var data = { event_type: '' };
    GM.Utility.DDLAutoComplete(txt_search, data, null, false);
    txt_search.val("");
});

$("#ul_event_type").on("click", function (event) {
    var EventType = $("#ddl_event_type").find(".selected-value").val();
    if (EventType == "Confirmation") {
       //window.location.href = "/RPConfirmation/Index";

    }
    else if (EventType === 'AmendConfirmation') {
        window.location.href = "/RPConfirmation/AmendConfirm";
    }
    else if (EventType === 'EarlyConfirmation') {
        window.location.href = "/RPConfirmation/EarlyTermination";
    }
    else if (EventType === 'AmendDeal') {
        window.location.href = "/RPConfirmation/AmendDeal";
    }
});

//Function : Settlement
function Settlement() {
    var rowData = $('#x-table-data').DataTable().rows($('#x-table-data .filter-ck:checked').closest('tr')).data();
    var transNoList = [];
    for (var i = 0; i < rowData.length; i++) {
        console.log(rowData[i].trans_no);
        transNoList.push(rowData[i].trans_no);
    }

    if (transNoList.length > 0) {

        window.location.href = "/RPConfirmation/Select?id=" + JSON.stringify(transNoList);
    }
    else {
        swal("Warning", "Please Select [Trans No] To Settlement", "error");
    }
}

function linktoreport(linkencode) {
    window.open(
        decodeURIComponent(linkencode),
        '_blank' // <- This is what makes it open in a new window.
    );
}

function FormatDecimal(value, point) {
    const f = Math.pow(10, point);
    var x = (Math.floor(value * f) / f).toFixed(point);
    return x.toString().replace(/\B(?=(?:\d{3})+(?!\d))/g, ",");
}