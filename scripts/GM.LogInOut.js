var LogInOut = (function ($) {

    var download = function (svcId) {

    };

    var createDate = {
        Id: $('#FormSearch_create_date'),
        init: function () {
            this.setValue($("#BusinessDate").text());
        },
        getValue: function () {
            return $.trim(this.Id.val());
        },
        setValue: function (value) {
            this.Id.val(value);
        }
    };

    var dataTable = {
        Id: $('#x-table-data'),
        init: function (url) {
            this.Id.DataTable({
                dom: 'Bfrtip',
                select: false,
                searching: true,
                scrollY: '80vh',
                scrollX: true,
                order: [
                    [1, "desc"]
                ],
                buttons: [],
                processing: true,
                serverSide: true,
                ajax: {
                    "url": url,
                    "type": "POST",
                    "error": function (jqXHR, textStatus, errorThrown) {
                        console.log(jqXHR);
                        console.log(textStatus);
                        console.log(errorThrown);
                    }
                },
                columnDefs: [
                    { targets: 0, data: "RowNumber", orderable: false },
                    { targets: 1, data: "svc_id" },
                    { targets: 2, data: "guid" },
                    {
                        targets: 3,
                        data: "svc_id",
                        className: "dt-body-center",
                        width: 30,
                        render: function (data, type, row, meta) {
                            var html = '';
                            html += '<button class="btn btn-default btn-round" onclick="LogInOut.download(\'' + row.svc_id + '\',\'req\')"><i class="feather-icon icon-download"></i></button>';
                            return html;
                        }
                    },
                    {
                        targets: 4,
                        data: "svc_id",
                        className: "dt-body-center",
                        width: 30,
                        render: function (data, type, row, meta) {
                            var html = '';
                            html += '<button class="btn btn-default btn-round" onclick="LogInOut.download(\'' + row.svc_id + '\',\'res\')"><i class="feather-icon icon-download"></i></button>';
                            return html;
                        }
                    },
                    { targets: 5, data: "svc_type" },
                    { targets: 6, data: "module_name" },
                    { targets: 7, data: "action_name" },
                    { targets: 8, data: "ref_id" },
                    { targets: 9, data: "status" },
                    { targets: 10, data: "status_desc" },
                    {
                        targets: 11, data: "create_date",
                        render: function (data, type, row, meta) {
                            if (data != null) {
                                return moment(data).format('DD/MM/YYYY HH:mm:ss');
                            }
                            return data;
                        }
                    },
                    { targets: 12, data: "create_by" }
                ],
                fixedColumns: {
                    leftColumns: 1
                },
                fnPreDrawCallback: function () {
                    dataTable.Id.DataTable().columns(11).search(createDate.getValue());
                }
            });
        }
    };

    var searchForm = {
        Id: $('#search-form'),
        init: function () {
            this.bindEventHandler();
        },
        bindEventHandler: function () {
            this.Id.submit(function (e) {
                e.preventDefault();

                $('#search-form :input').each(function () {
                    var input = $(this);
                    var key = input[0].name.split('.')[1];
                    switch (key) {
                        case "module_name": dataTable.Id.DataTable().columns(6).search($(this).val()); break;
                        case "action_name": dataTable.Id.DataTable().columns(7).search($(this).val()); break;
                        case "status": dataTable.Id.DataTable().columns(9).search($(this).val()); break;
                        case "create_date": dataTable.Id.DataTable().columns(11).search($(this).val()); break;
                    }
                });

                dataTable.Id.DataTable().draw();
            });
        }
    };

    return {
        init: function (url) {
            createDate.init();
            searchForm.init();
            dataTable.init(url.LogInOutList);
        },
        download: function (svcId, type) {
            var formId = $('#download-form');

            $("#svcId").val(svcId);

            $("#type").val(type);
            formId.submit();
        }
    };
})(jQuery);