$.ajaxSetup({
    cache: false
});

function ReleaseMessageEarly(trans_no) {
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
                            url: "/RPConfirmation/ReleaseMessageEarly",
                            type: "POST",
                            dataType: "JSON",
                            data: {
                                trans_no: trans_no,
                                message_type: 'CHANGE-EARLY'
                            },
                            success: function (res) {
                                if (res.success) {
                                    setTimeout(
                                        function () {
                                            $('.spinner').css('display', 'none');
                                            swal("Complete", "Generate Message Successfully", "success");
                                            GM.MessageEarly.Search();
                                        }, 100);
                                    setTimeout(
                                        function () {

                                        }, 300);
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

function GetMessageEarly(trans_no) {
    $("#check_releasemt").modal('toggle');

    var data = {
        trans_no: trans_no,
        event_type: 'CHANGE-EAR'
    };

    $.ajax({
        type: "GET",
        url: "/RPConfirmation/GetMessageEarly",
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
    $("#NavBar").html($('#NavRPReleaseMessage').val());

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
            // window.location.href = "/RPConfirmation/EarlyTermination";
        }
        else if (EventType === 'AmendDeal') {
            window.location.href = "/RPConfirmation/AmendDeal";
        }
    });

    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    };

    GM.MessageEarly = {};
    GM.MessageEarly.Table = $('#x-table-data').DataTable({
        dom: 'Bfrtip',
        searching: true,
        scrollY: '80vh',
        scrollX: true,
        order: [
            [1, "asc"]
        ],
        buttons: [],
        processing: true,
        serverSide: true,
        ajax: {
            "url": "/RPConfirmation/SearchEarlyTerminate",
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
                { targets: 0, data: "RowNumber", orderable: false },
                { targets: 1, data: "trans_no" },
                { targets: 2, data: "repo_deal_type" },
                { targets: 3, data: "trans_deal_type_name" },
                { targets: 4, data: "counter_party_name" },
                { targets: 5, data: "cur" },
                { targets: 6, data: "terminate_date", visible: false },
                { targets: 7, data: "from_trans_no", visible: false },
                { targets: 8, data: "to_trans_no", visible: false },
                {
                    targets: 9,
                    orderable: false,
                    data: "trans_no",
                    width: 60,
                    className: "dt-body-center",
                    render: function (data, type, row, meta) {
                        var html = '';
                        html += '<button title="Process" class="btn btn-default btn-round" form-mode="edit" onclick="ReleaseMessageEarly(' + "'" + row.trans_no + "'" + ')" ><i class="feather-icon icon-file-plus"></i></button>';
                        html += '<button title="Message" class="btn btn-default btn-round" form-mode="viewmt" onclick="GetMessageEarly(' + "'" + row.trans_no + "'" + ')" ><i class="feather-icon icon-message-square"></i></button>';
                        return html;
                    }
                }
            ],
        fixedColumns: {
            leftColumns: 1,
            rightColumns: 1
        },
        createdRow: function (row, data, dataIndex) {
            if (data.release_printed === "Y") {
                $('td', row).css('color', '#00B0FF');
            }
        }
    });


    GM.MessageEarly.Search = function () {
        var from_trans_no = $("#FormSearch_from_trans_no").val();
        var to_trans_no = $("#FormSearch_to_trans_no").val();

        if (from_trans_no !== '') {
            GM.MessageEarly.Table.columns(7).search(from_trans_no);
        } else {
            GM.MessageEarly.Table.columns(7).search(null);
        }

        if (to_trans_no !== '') {
            GM.MessageEarly.Table.columns(8).search(to_trans_no);
        } else {
            GM.MessageEarly.Table.columns(8).search(null);
        }

        GM.MessageEarly.Table.draw();
    };

    $("#search-form").on('submit', function (e) {
        e.preventDefault();
        GM.MessageEarly.Search();
    });

    $("#search-form").on('reset', function (e) {
        $('.spinner').css('display', 'block'); // Open Loading
        location.href = window.location.href;
    });
});