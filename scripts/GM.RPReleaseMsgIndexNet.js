$.ajaxSetup({
    cache: false
});

var myData;

function ReleaseMessage() {
    var rowData = $('#x-table-data').DataTable().rows($('#x-table-data .filter-ck:checked').closest('tr')).data();
    var transNoList = [];
    for (var i = 0; i < rowData.length; i++) {
        transNoList.push(rowData[i].trans_no);
    }

    if (transNoList.length > 0) {

        window.location.href = "/RPReleaseMessage/Select?id=" + JSON.stringify(transNoList);
    }
    else {
        swal("Warning", "Please Select [Trans No] To RPReleaseMessage", "error");
    }
}

function GenReleaseMT(trans_no, from_page, trans_deal_type_name, cur, release_printed, id, settlement_date, counter_party_id) {

    if ($('#ddl_payment_' + id).val() === '' || $('#ddl_mt_code_' + id).val() === '') {
        swal("Warning", "Please select Payment Method", "error");
        $('#ddl_payment_' + id).focus();
    } else {
        var payment_method = $('#ddl_payment_' + id).val();
        var trans_mt_code = $('#ddl_mt_code_' + id).val();

        var trans_deal_type;
        if (trans_deal_type_name === "Borrowing") {
            trans_deal_type = 'BR';
        }
        else if (trans_deal_type_name === "Lending") {
            trans_deal_type = 'LD';
        }

        if ($('#ddl_payment_' + id).val() === 'SWIFT') {
            var msg = "Comfirm ReleaseMessage?";
            if (release_printed === 'Y') {
                msg = "Message already released, do you want to release again?";
            }

            var data = {
                trans_no: trans_no,
                event_type: 'TRANS',
                trans_deal_type: trans_deal_type,
                payment_method: payment_method,
                trans_mt_code: trans_mt_code,
                cur: cur,
                settlement_date: settlement_date,
                maturity_date: settlement_date,
                counter_party_id: counter_party_id,
                swift_channel: $('#ddl_channel_' + id).val()
            };

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
                        $.post("CheckSettlementStatus", data)
                            .done(function (result) {
                                if (result.Success) {
                                    if (result.Message === "" && result.Data != null && result.Data.ResponseBody != null && result.Data.ResponseBody.Count > 0
                                        && result.Data.ResponseBody[0].SettlementStatus === localStorage.getItem('SettlementStatus.Complete')) {
                                        setTimeout(function () {
                                            swal("Fail", localStorage.getItem('CyberPay.ErrorMsg.Complete'), "error");
                                        }, 100);
                                        $('.spinner').css('display', 'none');
                                    } else {
                                        var url = $('#ddl_channel_' + id).val() === "SMD" ? "ReleaseMessageNetSettle" : "ReleaseMessageNetSettleCyberPay";
                                        $.post(url, data)
                                            .done(function (response) {
                                                $('.spinner').css('display', 'none');
                                                GM.Unmask();
                                                console.log(response);
                                                if (response.Success) {
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
                                                                GM.RPReleaseMessage.Form.Search();
                                                            }
                                                        );
                                                    }, 100);
                                                }
                                                else {
                                                    setTimeout(function () {
                                                        swal("Fail", response.Message, "error");
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


                        $('.spinner').css('display', 'block');

                    }
                });
        } else {

            $.ajax({
                type: "GET",
                url: "/RPReleaseMessage/GetReleaseMTNetSettle",
                content: "application/json; charset=utf-8",
                dataType: "json",
                data: {
                    trans_no: trans_no,
                    from_page: from_page,
                    event_type: 'TRANS',
                    trans_deal_type: trans_deal_type,
                    payment_method: $('#ddl_payment_' + id).val(),
                    trans_mt_code: $('#ddl_mt_code_' + id).val(),
                    cur: cur
                },
                success: function (res) {
                    var release_pti = "";
                    $.each(res, function (i, resdata) {
                        release_pti += resdata.mt_message + "\n";
                    });

                    $("#txtarea_release_pti").val(release_pti);
                    console.log("$(#txtarea_release_pti).val : " + $("#txtarea_release_pti").val());
                    setTimeout(function () {
                        PopUpDownload();
                    }, 200);
                },
                error: function (res) {
                    GM.Message.Clear();
                }
            });
        }
    }
}

function GetReleaseMT(trans_no, from_page, trans_deal_type_name, payment_method, trans_mt_code, cur, id) {

    if ($('#ddl_payment_' + id).val() === '' || $('#ddl_mt_code_' + id).val() === '') {
        swal("Warning", "Please select Payment Method", "error");
        $('#ddl_payment_' + id).focus();
    } else {

        if ($('#ddl_payment_' + id).val() === 'SWIFT') {
            var body = $("#modal_release_mt").find("tbody");
            body.html("");
            $("#check_releasemt").modal('toggle');
        } else {
            $("#txtarea_release_pti").val("");
            $("#check_releasemt_pti").modal('toggle');
        }

        var trans_deal_type;
        if (trans_deal_type_name === "Borrowing") {
            trans_deal_type = 'BR';
        }
        else if (trans_deal_type_name === "Lending") {
            trans_deal_type = 'LD';
        }

        var data = {
            trans_no: trans_no,
            from_page: from_page,
            event_type: 'TRANS',
            trans_deal_type: trans_deal_type,
            payment_method: $('#ddl_payment_' + id).val(),
            trans_mt_code: $('#ddl_mt_code_' + id).val(),
            cur: cur
        };

        $.ajax({
            type: "GET",
            url: "/RPReleaseMessage/GetReleaseMTNetSettle",
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: data,
            success: function (res) {
                if ($('#ddl_payment_' + id).val() === 'SWIFT') {
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
                } else {
                    var release_pti = "";
                    $.each(res, function (i, resdata) {
                        release_pti += resdata.mt_message + "\n";
                    });

                    $("#txtarea_release_pti").val(release_pti);
                }
            },
            error: function (res) {
                GM.Message.Clear();
            }
        });
    }
}

function ddlChangePayment(id, value) {
    console.log('myData[id].print_type.toUpperCase() : ' + myData[id].print_type.toUpperCase());
    var data = {
        payment_method: $('#ddl_payment_' + id).val(),
        trans_deal_type: myData[id].print_type.toUpperCase(),
        cur: myData[id].cur,
        event_type: 'NET-SET',
        repo_deal_type: 'ALL'
    };
    myData[id].payment_method = $('#ddl_payment_' + id).val() === '' ? null : $('#ddl_payment_' + id).val();
    GM.Utility.DDLStandard('ddl_mt_code_' + id, data, "\\RPReleaseMessage\\FillTransMtCode", value);
}

function ddlChangeMTCode(id) {
    myData[id].mt_code = $('#ddl_mt_code_' + id).val();
}

function ddlChangeChannel(id, value) {
    var data = { datastr: "" };
    GM.Utility.DDLStandard('ddl_channel_' + id, data, "\\RPReleaseMessage\\FillSwiftChannel", value);
}

function SetChannel(id) {
    myData[id].swift_channel = $('#ddl_channel_' + id).val();
}

function PopUpDownload() {
    var strDatetime = moment().format('DDMMYYhhmmss');
    var file_content = $('#txtarea_release_pti').val();
    var file_name = 'DF_RFREPO' + strDatetime + '.xml';
    download(file_content, file_name, "text/xml");
}

$(document).ready(function () {

    $("#NavBar").html($('#NavRPReleaseMessage').val());


    $('#show-more-less').click(function () {
        $(this).parents(".box-content").find('.advance-form').slideToggle(0);
        //$table.floatThead("reflow");
        if ($('#show-more-less i').hasClass('icon-chevron-up')) {
            $(this).html('Show less option <i class="feather-icon icon-chevron-down"></i>');
        } else {
            $(this).html('Show more option <i class="feather-icon icon-chevron-up"></i>');
        }
    });

    var budate = $("#BusinessDate").text();

    var formatmmddyyyydate = budate.split("/");
    formatmmddyyyydate = formatmmddyyyydate[1] + "/" + formatmmddyyyydate[0] + "/" + formatmmddyyyydate[2];
    var business_date = new Date(formatmmddyyyydate);


    $('#FormSearch_from_settlement_date').data("DateTimePicker").date(business_date);
    $('#FormSearch_to_settlement_date').data("DateTimePicker").date(business_date);


    GM.RPReleaseMessage = {};
    GM.RPReleaseMessage.Table = $('#x-table-data').DataTable({
        drawCallback: function (row, data, index) {
            var api = this.api();
            myData = api.rows({ page: 'current' }).data();

            var x = document.getElementsByName("ddlPayment");
            for (var i = 0; i < myData.length; i++) {

                var dataSearch = {
                    transdealtype: 'PAY',
                    cur: myData[i].cur
                };

                if ($('#ddl_payment_' + i).val() === "") {
                    GM.Utility.DDLStandard('ddl_payment_' + i, dataSearch, "\\RPCallMargin\\FillPaymentMethod", null);
                }

            }
        },
        dom: 'Bfrtip',
        searching: true,
        scrollY: '80vh',
        scrollX: true,
        order: [
            [2, "asc"]
        ],
        buttons: [],
        processing: true,
        serverSide: true,
        ajax: {
            "url": "/RPReleaseMessage/SearchNetSettle",
            "type": "POST",
            "error": function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR);
                console.log(textStatus);
                console.log(errorThrown);
            }
        },
        columnDefs:
            [
                {
                    targets: 0,
                    data: "trans_no",
                    searchable: false,
                    orderable: false,
                    className: 'dt-body-center',
                    render: function (data, type, row) {
                        if (row.event_type === 'CASH') {
                            return '<input type="checkbox"' + ' id="chk_' + data + '_1" class="filter-ck" />';
                        } else {
                            return '<input type="checkbox"' + ' id="chk_' + data + '_2" class="filter-ck" />';
                        }
                    }
                },
                { targets: 1, data: "RowNumber", orderable: false },
                {
                    targets: 2, data: "trans_no",
                    render: function (data, type, row, meta) {
                        var html = data;
                        if (row.event_type === 'CASH') {
                            html += '-1';
                        } else {
                            html += '-2';
                        }
                        return html;
                    }
                },
                { targets: 3, data: "counter_party_code" },
                { targets: 4, data: "cur" },
                {
                    targets: 5, data: "payment_method",
                    className: 'dt-body-center',
                    searchable: false,
                    orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '<input class="selected-data hidden" id="NetSetList[' + meta.row + '].payment_method" name="NetSetList[' + meta.row + '].payment_method" type="text" >' +
                            '<input class="selected-data hidden" id="NetSetList[' + meta.row + '].payment_method_name" name="NetSetList[' + meta.row + '].payment_method_name" type="text" >' +
                            '<select name="ddlPayment" id="ddl_payment_' + meta.row + '" style="height:23px;width:100px;" name="NetSetList[' + meta.row + '].payment_method" onchange="ddlChangePayment(\'' + meta.row + '\');" >' +
                            ' <option value="">- Please select -</option>';

                        html += ' <option value="SWIFT" ' + (row.payment_method === 'SWIFT' ? 'selected="selected"' : '') + ' >SWIFT</option>';

                        if (row.event_type !== 'CASH') {
                            html += '<option value="DF/RF" ' + (row.payment_method === 'DF/RF' ? 'selected="selected"' : '') + '  >DF/RF</option>';
                        }
                        html += '</select > ';


                        html += '<input class="selected-data hidden" id="NetSetList[' + meta.row + '].trans_mt_code" name="NetSetList[' + meta.row + '].trans_mt_code" type="text" >' +
                            '<input class="selected-data hidden" id="NetSetList[' + meta.row + '].trans_mt_code_name" name="NetSetList[' + meta.row + '].trans_mt_code_name" type="text" >' +
                            '<select name="ddl_mt_code" id="ddl_mt_code_' + meta.row + '" style="height:23px;width:100px;" name="NetSetList[' + meta.row + '].trans_mt_code" onchange="ddlChangeMTCode(\'' + meta.row + '\');" >' +
                            ' <option value="">- Please select -</option>';

                        setTimeout(function () {
                            if (row.trans_mt_code !== null) {
                                console.log("row.trans_mt_code : " + row.trans_mt_code);
                                ddlChangePayment(meta.row, row.trans_mt_code);
                            } else {
                                console.log("row.trans_mt_code Else : " + row.trans_mt_code);
                            }
                        }, 50);

                        html += '</select>';

                        html += '<input class="selected-data hidden" id="MarginList[' + meta.row + '].swift_channel" name="MarginList[' + meta.row + '].swift_channel" type="text" >' +
                            '<input class="selected-data hidden" id="MarginList[' + meta.row + '].swift_channel_desc" name="MarginList[' + meta.row + '].swift_channel_desc" type="text" >' +
                            '<select ' + (row.print_type.toUpperCase().indexOf('CASH') !== -1 ? "" : " disabled ") +
                            ' id="ddl_channel_' + meta.row + '" style = "height:23px;width:100px;" name = "MarginList[' + meta.row + '].swift_channel" onchange = "SetChannel(\'' + meta.row + '\');" > ' +
                            ' <option value="">- Please select -</option>';

                        setTimeout(function () {
                            if (row.swift_channel !== null) {
                                console.log("row.swift_channel : " + row.swift_channel);
                                ddlChangeChannel(meta.row, row.swift_channel);
                            } else if (row.print_type.toUpperCase().indexOf('CASH') !== -1) {
                                console.log("row.swift_channel : SMD ");
                                ddlChangeChannel(meta.row, 'SMD');
                            }
                            else {
                                console.log("row.swift_channel Else : " + row.swift_channel);
                            }
                        }, 100);

                        html += '</select>';

                        return html;
                    }
                },
                {
                    targets: 6, data: "swift_status",
                    searchable: false,
                    orderable: false,
                    className: "dt-body-center",
                    visible: true,
                    render: function (data, type, row, meta) {
                        return data;
                    }
                },
                {
                    targets: 7,
                    orderable: false,
                    data: "trans_no",
                    className: "dt-body-center",
                    render: function (data, type, row, meta) {
                        var html = '';
                        html += '<button title="Release Message" class="btn btn-default btn-round" form-mode="viewmt" onclick="GenReleaseMT(' + "'" + row.trans_no + "'" + ',' + "'" + row.event_type + "'" + ',' + "'" + row.trans_deal_type_name + "'" + ',' + "'" + row.cur + "'" + ',' + "'" + row.release_printed + "'" + ',' + "'" + meta.row + "'" + ',' + "'" + row.settlement_date + "'" + ',' + "'" + row.counter_party_id + "'" + ')" ><i class="feather-icon icon-file-plus"></i></button>';
                        html += '<button title="View Message" class="btn btn-default btn-round" form-mode="viewmt" onclick="GetReleaseMT(' + "'" + row.trans_no + "'" + ',' + "'" + row.event_type + "'" + ',' + "'" + row.trans_deal_type_name + "'" + ',' + "'" + row.payment_method + "'" + ',' + "'" + row.trans_mt_code + "'" + ',' + "'" + row.cur + "'" + ',' + "'" + meta.row + "'" + ')" ><i class="feather-icon icon-message-square"></i></button>';
                        return html;
                    }
                },

                { targets: 8, data: "from_trans_no", visible: false },
                { targets: 9, data: "to_trans_no", visible: false },
                { targets: 10, data: "from_trade_date", visible: false },
                { targets: 11, data: "to_trade_date", visible: false },
                { targets: 12, data: "from_settlement_date", visible: false },
                { targets: 13, data: "to_settlement_date", visible: false },
                { targets: 14, data: "from_maturity_date", visible: false },
                { targets: 15, data: "to_maturity_date", visible: false },
                { targets: 16, data: "counter_party_name", visible: false },
                { targets: 17, data: "event_type", visible: false },
                { targets: 18, data: "print_type", visible: false },
            ],
        fixedColumns: {
            leftColumns: 1,
            rightColumns: 12
        },
        fnPreDrawCallback: function () {
            $('#x-table-data').DataTable().columns(17).search($('#FormSearch_event_type').val());
            $('#x-table-data').DataTable().columns(12).search($('#FormSearch_from_settlement_date').val());
            $('#x-table-data').DataTable().columns(13).search($('#FormSearch_to_settlement_date').val());
            $('#x-table-data').DataTable().columns(14).search($('#FormSearch_from_maturity_date').val());
            $('#x-table-data').DataTable().columns(15).search($('#FormSearch_to_maturity_date').val());
        },
        createdRow: function (row, data, dataIndex) {
            if (data.release_printed === "Y") {
                $('td', row).css('color', '#00B0FF');
            }
        }
    });

    //Function : Checkbox
    $('#x-table-data').on('click', 'tr', function () {

        GM.RPReleaseMessage.Table = $('#x-table-data').DataTable();
        var data = GM.RPReleaseMessage.Table.row(this).data();

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

    //Function : Btn ReleaseMessage
    GM.RPReleaseMessage.ReleaseMessage = function (btn) {
        GM.Message.Clear();
        ReleaseMessage();
    };

    //Function : DDl Event Type
    $("#ul_event_type").on("click", function (event) {
        var EventType = $("#ddl_event_type").find(".selected-value").val();
        if (EventType === "Coupon") {
            window.location.href = "/RPReleaseMessage/IndexCoupon";
        }
        else if (EventType === 'Margin') {
            window.location.href = "/RPReleaseMessage/IndexCallMargin";
        }
        else if (EventType === 'Interest Margin') {
            window.location.href = "/RPReleaseMessage/IndexInterestMargin";
        }
        else if (EventType === 'Settlement' || EventType === 'Maturity') {
            window.location.href = "/RPReleaseMessage/Index?search=" + EventType;
        }
        else {
            $('#FormSearch_from_maturity_date').data("DateTimePicker").date(null);
            $('#FormSearch_to_maturity_date').data("DateTimePicker").date(null);
            $('#FormSearch_from_settlement_date').data("DateTimePicker").date(business_date);
            $('#FormSearch_to_settlement_date').data("DateTimePicker").date(business_date);
            GM.RPReleaseMessage.Form.Search();
        }
    });

    //Binding : DDL Trans Deal Type

    $("#ddl_trans_deal_type").click(function () {
        var txt_search = $('#txt_trans_deal_type');
        var data = { trans_deal_type: '' };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    //Binding : DDL Event Type
    $("#ddl_event_type").click(function () {
        var txt_search = $('#txt_event_type');
        var data = { event_type: '' };
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });

    //Binding : DDL Payment Method
    $("#ddl_payment_method").click(function () {
        var txt_search = $('#txt_payment_method');
        var data = { payment_method: '' };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    //Binding : DDL trans_mt_code
    $("#ddl_trans_mt_code").click(function () {

        var txt_search = $('#txt_trans_mt_code');
        txt_search.val("");
        var payment_method = $('#FormSearch_payment_method').val();
        var trans_deal_type = $('#FormSearch_trans_deal_type').val();
        var event_type = "TRANS";
        var data = { payment_method: payment_method, trans_deal_type: trans_deal_type, event_type: event_type };
        if (payment_method === "" || trans_deal_type === "" || event_type === "") {
            swal("Warning", "MT Code require data from [Trans Deal Type],[Payment Method],[Event Type]", "warning");
        }
        GM.Utility.DDLAutoComplete(txt_search, data, null);
    });

    //Binding : DDL Counterparty
    $("#ddl_counterparty").click(function () {
        var txt_search = $('#txt_counterparty');
        var data = { datastr: null };
        GM.Utility.DDLAutoCompleteSet4Value(txt_search, data, null, null, null);
        txt_search.val("");
    });

    $('#txt_counterparty').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Binding Div Detail
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    };

    GM.RPReleaseMessage.Form = {};
    GM.RPReleaseMessage.Form.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            switch (key) {

                case "event_type": GM.RPReleaseMessage.Table.columns(17).search($(this).val()); break;
                case "counter_party_code": GM.RPReleaseMessage.Table.columns(3).search($(this).val()); break;
                case "payment_method": GM.RPReleaseMessage.Table.columns(5).search($(this).val()); break;
                case "trans_mt_code": GM.RPReleaseMessage.Table.columns(6).search($(this).val()); break;
                case "from_trans_no": GM.RPReleaseMessage.Table.columns(8).search($(this).val()); break;
                case "to_trans_no": GM.RPReleaseMessage.Table.columns(9).search($(this).val()); break;
                case "from_trade_date": GM.RPReleaseMessage.Table.columns(10).search($(this).val()); break;
                case "to_trade_date": GM.RPReleaseMessage.Table.columns(11).search($(this).val()); break;
                case "from_settlement_date": GM.RPReleaseMessage.Table.columns(12).search($(this).val()); break;
                case "to_settlement_date": GM.RPReleaseMessage.Table.columns(13).search($(this).val()); break;
                case "from_maturity_date": GM.RPReleaseMessage.Table.columns(14).search($(this).val()); break;
                case "to_maturity_date": GM.RPReleaseMessage.Table.columns(15).search($(this).val()); break;
                case "counter_party_name": GM.RPReleaseMessage.Table.columns(16).search($(this).val()); break;
            }
        });

        GM.RPReleaseMessage.Table.draw();
    };

    GM.RPReleaseMessage.Form.Initial = function () {
        $("#action-form")[0].reset();
    };
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


    GM.RPReleaseMessage.Sync = function (btn) {
        $('.spinner').css('display', 'block'); // Open Loading

        $.ajax({
            type: "POST",
            url: "/RPReleaseMessage/SyncTransactionNet",
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: {
                event_type: $('#FormSearch_event_type').val(),
                counter_party_code: $('#FormSearch_counter_party_code').val(),
                payment_method: "SWIFT",
                from_trade_date: $('#FormSearch_from_trade_date').val(),
                to_trade_date: $('#FormSearch_to_trade_date').val(),
                from_settlement_date: $('#FormSearch_from_settlement_date').val(),
                to_settlement_date: $('#FormSearch_to_settlement_date').val(),
                from_maturity_date: $('#FormSearch_from_maturity_date').val(),
                to_maturity_date: $('#FormSearch_to_maturity_date').val(),
                cur: $('#FormSearch_cur').val(),
                isSyncCyberPay: true
            },
            success: function (res) {
                if (res.Success) {
                    setTimeout(function () {
                        swal({
                            title: "Complete",
                            text: "Sync Data CyberPay Success",
                            type: "success",
                            showCancelButton: false,
                            confirmButtonClass: "btn-success",
                            confirmButtonText: "Ok",
                            closeOnConfirm: true,
                        },
                            function () {
                                $('.spinner').css('display', 'none'); // Close Loading
                            }
                        );
                    }, 100);
                } else {
                    setTimeout(function () {
                        swal({
                            title: "Warning",
                            text: res.Message,
                            type: "error",
                            showCancelButton: false,
                            confirmButtonClass: "btn-success",
                            confirmButtonText: "Ok",
                            closeOnConfirm: true,
                        },
                            function () {
                                $('.spinner').css('display', 'none'); // Close Loading
                            }
                        );
                    }, 100);
                }

            },
            error: function (res) {
                setTimeout(function () {
                    swal({
                        title: "Warning",
                        text: res.Message,
                        type: "error",
                        showCancelButton: false,
                        confirmButtonClass: "btn-success",
                        confirmButtonText: "Ok",
                        closeOnConfirm: true,
                    },
                        function () {
                            $('.spinner').css('display', 'none'); // Close Loading
                        }
                    );
                }, 100);

                $('.spinner').css('display', 'none'); // Close Loading
            }
        });
    };


    $("#search-form").on('submit', function (e) {
        e.preventDefault();
        GM.Message.Clear();

        GM.RPReleaseMessage.Form.Search();
    });

    $("#search-form").on('reset', function (e) {

        $('.spinner').css('display', 'block'); // Open Loading
        location.href = window.location.href;

    });
});