
function auto2digit(obj) {
    if (obj.value.length) {
        var nStr = obj.value;
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
        return obj.value = x1 + '.' + x2;
    }
    return obj.value = '0.00';
}

$(document).ready(function () {

    $("#NavBar").html($('#NavRPMarginInterestFCY').val());

    var budate = $("#BusinessDate").text();

    var formatmmddyyyydate = budate.split("/");
    formatmmddyyyydate = formatmmddyyyydate[1] + "/" + formatmmddyyyydate[0] + "/" + formatmmddyyyydate[2];
    var eomDate = new Date(formatmmddyyyydate);

    $('#FormSearch_eom_date_from').data("DateTimePicker").date(eomDate);
    $('#FormSearch_eom_date_to').data("DateTimePicker").date(eomDate);

    $("#ddl_counterparty").click(function () {
        var txt_search = $('#txt_counterparty');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_counterparty').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });


    $("#ddl_cur").click(function () {
        var txt_search = $('#txt_cur');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_cur').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });


    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    }

    GM.RPMarginInterestFCY = {};
    GM.RPMarginInterestFCY.Table = $('#x-table-data').DataTable({
        dom: 'Bfrtip',
        select: false,
        searching: true,
        scrollY: '80vh',
        scrollX: true,
        order: [
            [1, "desc"]
        ],
        buttons:
            [

            ],
        processing: true,
        serverSide: true,
        ajax: {
            "url": "/RPMarginInterestFCY/Search",
            "type": "POST",
            "error": function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR);
                console.log(textStatus);
                console.log(errorThrown);
            }
        },
        columnDefs:
            [
                { targets: 0, orderable: false, data: "RowNumber", width: 30, },
                {
                    targets: 1, data: "eom_date",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data
                    }
                },
                { targets: 2, data: "counter_party_code" },
                { targets: 3, data: "cur", className: 'dt-body-center' },
                { targets: 4, data: "margin_status", className: 'dt-body-center' },
                { targets: 5, data: "rec_pay_status", className: 'dt-body-center' },
                {
                    targets: 6, data: "position_margin", className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimalValidate(data, 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 7, data: "total_int_rec", className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimalValidate(data, 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 8, data: "total_int_pay", className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimalValidate(data, 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 9, data: "int_rec_tax", className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimalValidate(data, 2);
                        }
                        return html;
                    }
                },
                {
                    targets: 10,
                    data: "counter_party_id",
                    className: "dt-body-center",
                    width: 30,
                    orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        html += '<td></td>';
                        html += '<button class="btn btn-default btn-round" counter_party_id="' + row.counter_party_id
                            + '" counter_party_code="' + row.counter_party_code
                            + '" eom_date="' + moment(row.eom_date).format('DD/MM/YYYY')
                            + '" cur="' + row.cur
                            + '" margin_status="' + row.margin_status
                            + '" rec_pay_status="' + row.rec_pay_status
                            + '" position_margin="' + row.position_margin
                            + '" total_int_rec="' + row.total_int_rec
                            + '" total_int_pay="' + row.total_int_pay
                            + '" int_rec_tax="' + row.int_rec_tax
                            + '" int_rec_pay_date="' + moment(row.int_rec_pay_date).format('DD/MM/YYYY')
                        html += '" form-mode="edit" onclick="GM.RPMarginInterestFCY.Interest.Get(this)"><i class="feather-icon icon-edit"></i></button>';
                        html += '<td></td>';
                        return html;
                    }
                },
                { targets: 11, data: "counter_party_id", "visible": false },
                { targets: 12, data: "eom_date_from", "visible": false },
                { targets: 13, data: "eom_date_to", "visible": false },
                { targets: 14, data: "int_rec_pay_date", "visible": false }
            ],
        fixedColumns: {
            leftColumns: 1,
            rightColumns: 5
        },
        fnPreDrawCallback: function () {
            $('#x-table-data').DataTable().columns(12).search($("#FormSearch_eom_date_from").val());
            $('#x-table-data').DataTable().columns(13).search($("#FormSearch_eom_date_to").val());
        }
    });

    GM.RPMarginInterestFCY.Search = function () {
        try {
            var eom_date_from = $("#FormSearch_eom_date_from").val();
            var eom_date_to = $("#FormSearch_eom_date_to").val();
            var cur = $('#FormSearch_cur').val();
            var counter_party_id = $('#FormSearch_counter_party_id').val();
            var margin_status = $("#FormSearch_margin_status").val();
            var rec_pay_status = $("#FormSearch_rec_pay_status").val();

            GM.RPMarginInterestFCY.Table.columns(3).search(cur);
            GM.RPMarginInterestFCY.Table.columns(4).search(margin_status);
            GM.RPMarginInterestFCY.Table.columns(5).search(rec_pay_status);
            GM.RPMarginInterestFCY.Table.columns(11).search(counter_party_id);
            GM.RPMarginInterestFCY.Table.columns(12).search(eom_date_from);
            GM.RPMarginInterestFCY.Table.columns(13).search(eom_date_to);
            GM.RPMarginInterestFCY.Table.draw();
        } catch (err) {
            console.log(err.message);
        }
    };

    $("#search-form").on('submit', function (e) {
        e.preventDefault()
        GM.Message.Clear();
        GM.RPMarginInterestFCY.Search();
    });

    $("#search-form").on('reset', function (e) {
        GM.Defer(function () {
            $('#ddl_cur').find(".selected-data").text("Select...");
            $('#FormSearch_cur').val(null);
            GM.Message.Clear();
            GM.RPMarginInterestFCY.Search();
        }, 100)
    });

    GM.RPMarginInterestFCY.Interest = {};
    GM.RPMarginInterestFCY.Interest.Form = $("#AdjustInterestModal");

    GM.RPMarginInterestFCY.Interest.Get = function (btn) {
        GM.RPMarginInterestFCY.Interest.Form.modal('toggle');

        var counter_party_id = $(btn).attr("counter_party_id");
        var counter_party_code = $(btn).attr("counter_party_code");
        var eom_date = $(btn).attr("eom_date");

        var cur = $(btn).attr("cur");
        var margin_status = $(btn).attr("margin_status");
        var rec_pay_status = $(btn).attr("rec_pay_status");
        var position_margin = $(btn).attr("position_margin");
        var total_int_rec = $(btn).attr("total_int_rec");
        var total_int_pay = $(btn).attr("total_int_pay");
        var int_rec_tax = $(btn).attr("int_rec_tax");
        var int_rec_pay_date = $(btn).attr("int_rec_pay_date");

        $('#lbl_eom_date').text(eom_date);
        $('#lbl_cur').text(cur);
        $('#lbl_counter_party_code').text(counter_party_code);
        $('#lbl_position_margin').text(position_margin);
        $('#lbl_margin_status').text(margin_status);
        $('#lbl_rec_pay_status').text(rec_pay_status);

        $('#lbl_counter_party_id').val(counter_party_id);
        if (int_rec_pay_date != 'Invalid date') {
            $('#FormAction_payment_date').val(int_rec_pay_date);
        } else {
            $('#FormAction_payment_date').data("DateTimePicker").date(null);
        }

        $('#FormAction_total_int_rec').val(FormatDecimalValidate(total_int_rec, 2));
        $('#FormAction_total_int_pay').val(FormatDecimalValidate(total_int_pay, 2));
        $('#FormAction_int_rec_tax').val(FormatDecimalValidate(int_rec_tax, 2));

        //if (rec_pay_status === 'Receive') {
        //    $('#FormAction_total_int_pay').prop('disabled', true);
        //    $('#FormAction_int_rec_tax').prop('disabled', true);

        //    $('#FormAction_total_int_rec').prop('disabled', false);
        //} else {
        //    $('#FormAction_total_int_rec').prop('disabled', true);

        //    $('#FormAction_total_int_pay').prop('disabled', false);
        //    $('#FormAction_int_rec_tax').prop('disabled', false);
        //}
    }

    $('#btnSave').click(function (e) {
        var isValid = true;
        $("#payment_date_error").text("");

        var payment_date = $('#FormAction_payment_date');
        if (payment_date.val().trim() == "") {
            payment_date.focus();
            $("#payment_date_error").text("The Payment Date is required.");
            isValid = false;
        }

        if (isValid) {
            swal({
                title: "Comfirm Save ?",
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
                        GM.Message.Clear();
                        $('.spinner').css('display', 'block');

                        var formatmmddyyyydate = $("#FormAction_payment_date").val().split("/");
                        var payment_date = formatmmddyyyydate[2] + "-" + formatmmddyyyydate[1] + "-" + formatmmddyyyydate[0];
                        formatmmddyyyydate = $('#lbl_eom_date').text().split("/");
                        var eom_date = formatmmddyyyydate[2] + "-" + formatmmddyyyydate[1] + "-" + formatmmddyyyydate[0];


                        $('.spinner').css('display', 'block');
                        $.ajax({
                            url: "/RPMarginInterestFCY/Save",
                            type: "POST",
                            dataType: "JSON",
                            data: {
                                eom_date: $('#lbl_eom_date').text(),
                                counter_party_id: $('#lbl_counter_party_id').val(),
                                cur: $('#lbl_cur').text(),
                                int_rec_pay_date: $("#FormAction_payment_date").val(),
                                total_int_rec: $('#FormAction_total_int_rec').val(),
                                total_int_pay: $('#FormAction_total_int_pay').val(),
                                int_rec_tax: $('#FormAction_int_rec_tax').val(),
                                margin_status: $('#lbl_margin_status').text() === 'Interest' ? 'int' : 'close',
                                rec_pay_status: $('#lbl_rec_pay_status').text().toLowerCase()
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
                                                    GM.RPMarginInterestFCY.Interest.Form.modal('hide');
                                                    GM.RPMarginInterestFCY.Search();
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
        }
    });

    numberOnlyAndDotAndMinute = function (obj) {
        obj.value = obj.value
            .replace(/[^\d.-]/g, '') // numbers and decimals and dot and minute only
            .replace(/(^[\d]{6})[\d]/g, '$1') // not more than 3 digits at the beginning
            .replace(/(^-[\d]{6})[\d]/g, '$1') // not more than 3 digits at the beginning
            .replace(/(\--*)\-/g, '$1') // decimal can't exist more than once
            .replace(/(\..*)\./g, '$1') // decimal can't exist more than once
            .replace(/(\.[\d]{8})./g, '$1'); // not more than 8 digits after decimal
    };

    $('#FormAction_payment_date').on('dp.change', function (e) {
        if ($('#FormAction_payment_date').val() !== "") {
            var data = {
                eom_date: $('#lbl_eom_date').text(),
                payment_date: $('#FormAction_payment_date').val(),
                counter_party_id: $("#lbl_counter_party_id").val(),
                cur: $("#lbl_cur").text()
            }

            console.log("$('#FormAction_payment_date').val() : " + $('#FormAction_payment_date').val());

            $.post('/RPMarginInterestFCY/CheckPaymentDateInterestMargin', data)
                .done(function (res) {
                    if (res.Message !== '') {
                        setTimeout(
                            function () {
                                swal({
                                    title: "Fail",
                                    text: res.Message,
                                    type: "error",
                                    showCancelButton: false,
                                    confirmButtonClass: "btn-success",
                                    confirmButtonText: "Ok",
                                    closeOnConfirm: true
                                });
                            },
                            200);
                        $('#FormAction_payment_date').data("DateTimePicker").date(null);
                    }
                });
        }
    });
});