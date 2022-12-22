$.ajaxSetup({
    cache: false
});

function ReleaseMessageConfirm(trans_no, message_type) {
    setTimeout(
        function () {
            swal({
                title: "Comfirm Process?",
                text: "",
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
                        $('.spinner').css('display', 'block');
                        $.ajax({
                            url: "/RPConfirmation/ReleaseMessageAmendConfirm",
                            type: "POST",
                            dataType: "JSON",
                            data: {
                                trans_no: trans_no,
                                message_type: message_type
                            },
                            success: function (res) {
                                if (res.success) {
                                    setTimeout(
                                        function () {
                                            $('.spinner').css('display', 'none');
                                            swal("Complete", "Generate Message Successfully", "success");
                                        }, 100);

                                    setTimeout(
                                        function () {
                                            GM.MessageConfirm.Search();
                                        }, 500);
                                } else {
                                    console.log(res.Message);
                                    setTimeout(
                                        function () {
                                            $('.spinner').css('display', 'none');
                                            swal("Fail", res.Message, "error");
                                        }, 100);
                                }
                            },
                            error: function (res) {
                                setTimeout(
                                    function () {
                                        $('.spinner').css('display', 'none');
                                        swal("Fail", res.Message, "error");
                                    }, 100);
                                console.log(res);
                            }
                        });
                    }
                }
            );
        }, 100);
}

function GetMessageConfirm(trans_no, message_tpye) {
    $("#check_releasemt").modal('toggle');

    var data = {
        trans_no: trans_no,
        message_type: message_tpye
    };

    $.ajax({
        type: "GET",
        url: "/RPConfirmation/GetMessageConfirmAmendDeal",
        content: "application/json; charset=utf-8",
        dataType: "json",
        data: data,
        success: function (res) {

            var body = $("#modal_release_mt").find("tbody");
            var html = "";
            var totalamount = 0;
            var isin_code = "";
            body.html("");
            $.each(res, function (i, resdata) {

                if (isin_code !== resdata.isin_code) {
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
        },
        error: function (res) {
            console.log(res);
        }
    });
}

$(document).ready(function () {

    $("#ddl_event_type").click(function () {
        var txt_search = $('#txt_event_type');
        var data = { event_type: '' };
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });

    $("#ul_event_type").on("click", function (event) {
        var EventType = $("#ddl_event_type").find(".selected-value").val();
        if (EventType === "Confirmation") {
            window.location.href = "/RPConfirmation/Index";
        }
        else if (EventType === 'AmendConfirmation') {
            window.location.href = "/RPConfirmation/AmendConfirm";
        } else if (EventType === 'EarlyConfirmation') {
            window.location.href = "/RPConfirmation/EarlyTermination";
        }
        else if (EventType === 'AmendDeal') {
            //window.location.href = "/RPConfirmation/AmendDeal";
        }
    });

    $("#ddl_message_type").click(function () {
        var txt_search = $('#txt_message_type');
        var data = { event_type: '' };
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });

    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    };

    GM.MessageConfirm = {};
    GM.MessageConfirm.Table = $('#x-table-data').DataTable({
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
            "url": "/RPConfirmation/SearchAmendDeal",
            "type": "POST",
            "statusCode": {
                401: function () {
                    window.location.href = "/RPConfirmation";
                }
            },
            "error":
                function (error) {
                    console.log(error);
                    window.location.href = "/RPConfirmation";
                }
        },
        columnDefs:
            [
                {
                    targets: 0, data: "RowNumber", orderable: false,
                    className: 'dt-body-center',
                    render: function (data, type, row, meta) {
                        return data;
                    }
                },
                {
                    targets: 1, data: "message_type", orderable: false,
                    className: 'dt-body-center',
                    render: function (data, type, row, meta) {
                        if (data === 'CONFIRM') {
                            return 'NEW';
                        } else {
                            return data;
                        }

                    }
                },
                { targets: 2, data: "trans_no" },
                { targets: 3, data: "repo_deal_type" },
                { targets: 4, data: "trans_deal_type_name" },
                { targets: 5, data: "counter_party_name" },
                { targets: 6, data: "cur" },
                {
                    targets: 7, data: "trade_date",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                {
                    targets: 8, data: "settlement_date",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                {
                    targets: 9, data: "maturity_date",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                {
                    targets: 10,
                    orderable: false,
                    data: "trans_no",
                    width: 60,
                    className: "dt-body-center",
                    render: function (data, type, row, meta) {
                        var html = '';
                        html += '<button title="Process" class="btn btn-default btn-round" form-mode="edit" onclick="ReleaseMessageConfirm(' + "'" + row.trans_no + "'" + ",'" + row.message_type + "'" + ')" ><i class="feather-icon icon-file-plus"></i></button>';
                        html += '<button title="Message" class="btn btn-default btn-round" form-mode="viewmt" onclick="GetMessageConfirm(' + "'" + row.trans_no + "'" + ",'" + row.message_type + "'" + ')" ><i class="feather-icon icon-message-square"></i></button>';
                        return html;
                    }
                },
                { targets: 11, data: "policy_date", visible: false },
                { targets: 12, data: "from_trans_no", visible: false },
                { targets: 13, data: "to_trans_no", visible: false },
                { targets: 14, data: "from_trade_date", visible: false },
                { targets: 15, data: "to_trade_date", visible: false },
                { targets: 16, data: "from_settlement_date", visible: false },
                { targets: 17, data: "to_settlement_date", visible: false },
                { targets: 18, data: "from_maturity_date", visible: false },
                { targets: 19, data: "to_maturity_date", visible: false }

            ],
        fixedColumns: {
            leftColumns: 1,
            rightColumns: 10
        },
        createdRow: function (row, data, dataIndex) {
            if (data.release_printed === "Y") {
                $('td', row).css('color', '#00B0FF');
            }
        }
    });


    GM.MessageConfirm.Search = function () {
        var from_trans_no = $("#FormSearch_from_trans_no").val();
        var to_trans_no = $("#FormSearch_to_trans_no").val();
        var policy_date = $("#FormSearch_policy_date").val();

        if (policy_date !== '') {
            GM.MessageConfirm.Table.columns(11).search(policy_date);
        } else {
            GM.MessageConfirm.Table.columns(11).search(null);
        }

        if (from_trans_no !== '') {
            GM.MessageConfirm.Table.columns(12).search(from_trans_no);
        } else {
            GM.MessageConfirm.Table.columns(12).search(null);
        }

        if (to_trans_no !== '') {
            GM.MessageConfirm.Table.columns(13).search(to_trans_no);
        } else {
            GM.MessageConfirm.Table.columns(13).search(null);
        }

        var from_trade_date = $("#FormSearch_from_trade_date").val();
        var to_trade_date = $("#FormSearch_to_trade_date").val();

        if (from_trade_date !== '') {
            GM.MessageConfirm.Table.columns(14).search(from_trade_date);
        } else {
            GM.MessageConfirm.Table.columns(14).search(null);
        }

        if (to_trade_date !== '') {
            GM.MessageConfirm.Table.columns(15).search(to_trade_date);
        } else {
            GM.MessageConfirm.Table.columns(15).search(null);
        }

        var from_settlement_date = $("#FormSearch_from_settlement_date").val();
        var to_settlement_date = $("#FormSearch_to_settlement_date").val();

        if (from_settlement_date !== '') {
            GM.MessageConfirm.Table.columns(16).search(from_settlement_date);
        } else {
            GM.MessageConfirm.Table.columns(16).search(null);
        }

        if (to_settlement_date !== '') {
            GM.MessageConfirm.Table.columns(17).search(to_settlement_date);
        } else {
            GM.MessageConfirm.Table.columns(17).search(null);
        }

        var from_maturity_date = $("#FormSearch_from_maturity_date").val();
        var to_maturity_date = $("#FormSearch_to_maturity_date").val();

        if (from_maturity_date !== '') {
            GM.MessageConfirm.Table.columns(18).search(from_maturity_date);
        } else {
            GM.MessageConfirm.Table.columns(18).search(null);
        }

        if (to_maturity_date !== '') {
            GM.MessageConfirm.Table.columns(19).search(to_maturity_date);
        } else {
            GM.MessageConfirm.Table.columns(19).search(null);
        }

        var message_type = $("#FormSearch_message_type").val();
        if (message_type !== '') {
            GM.MessageConfirm.Table.columns(1).search(message_type);
        } else {
            GM.MessageConfirm.Table.columns(1).search(null);
        }

        GM.MessageConfirm.Table.draw();
    };

    $("#search-form").on('submit', function (e) {
        e.preventDefault();
        GM.MessageConfirm.Search();
    });

    $("#search-form").on('reset', function (e) {
        $('.spinner').css('display', 'block'); // Open Loading
        location.href = window.location.href;
    });
});