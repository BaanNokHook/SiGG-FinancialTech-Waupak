var _isUpdate = "False";
var _isDelete = "False";
var _urlCreate;
var _urlEdit;
var _urlGetEdit;
var _urlDelete;
var _urlSearch;

var budate = $("#BusinessDate").text();

var formatmmddyyyydate = budate.split("/");
formatmmddyyyydate = formatmmddyyyydate[1] + "/" + formatmmddyyyydate[0] + "/" + formatmmddyyyydate[2];
var b_date = new Date(formatmmddyyyydate);

var adjustNum = {
    Id: $('#FormSearch_adjust_num'),
    clearValue: function () {
        this.Id.val(null);
    },
    getValue: function () {
        return $.trim(this.Id.val());
    }
};

var transNo = {
    Id: $('#FormSearch_trans_no'),
    clearValue: function () {
        this.Id.val(null);
    },
    getValue: function () {
        return $.trim(this.Id.val());
    }
};

var cur = {
    Id: $('#ddl_cur'),
    init: function () {
        this.bindEventHandler();
    },
    bindEventHandler: function () {
        var txt_search = $('#txt_cur');
        this.Id.click(function () {
            var data = { datastr: null };
            GM.Utility.DDLAutoComplete(txt_search, data, null);
            txt_search.val("");
        });

        txt_search.keyup(function () {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        });
    },
    clearValue: function () {
        this.Id.find(".selected-data").text("Select...");
        this.Id.val(null);
        $('#FormSearch_cur').val(null);
    },
    getValue: function () {
        return $.trim($('#FormSearch_cur').val());
    }
};

var transPort = {
    Id: $('#ddl_trans_port'),
    init: function () {
        this.bindEventHandler();
    },
    bindEventHandler: function () {
        var txt_search = $('#txt_trans_port');
        this.Id.click(function () {
            var data = { datastr: null };
            GM.Utility.DDLAutoComplete(txt_search, data, null);
            txt_search.val("");
        });

        txt_search.keyup(function () {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        });
    },
    clearValue: function () {
        this.Id.find(".selected-data").text("Select...");
        this.Id.val(null);
        $('#FormSearch_trans_port').val(null);
    },
    getValue: function () {
        return $.trim($('#FormSearch_trans_port').val());
    }
};

var postingDate = {
    Id: $('#FormSearch_posting_date'),
    init: function() {
        this.setValue(b_date);
    },
    getValue: function () {
        return $.trim(this.Id.val());
    },
    setValue: function(value) {
        this.Id.data("DateTimePicker").date(value);
    }
};

var tableResult = {
    Id: $('#x-table-data'),
    init: function () {
        this.Id.DataTable({
            dom: 'Bfrtip',
            select: false,
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
                "url": _urlSearch,
                "type": "POST"
            },
            columnDefs:
                [
                    { targets: 0, data: "RowNumber", orderable: false },
                    { targets: 1, data: "adjust_num", orderable: true },
                    {
                        targets: 2, data: "posting_date", orderable: true,
                        render: function (data, type, row, meta) {
                            if (data != null) {
                                return moment(data).format('DD/MM/YYYY');
                            }
                            return data;
                        }
                    },
                    {
                        targets: 3, data: "value_date", orderable: true,
                        render: function (data, type, row, meta) {
                            if (data != null) {
                                return moment(data).format('DD/MM/YYYY');
                            }
                            return data;
                        }
                    },
                    { targets: 4, data: "trans_port", orderable: false },
                    { targets: 5, data: "trans_no", orderable: false },
                    { targets: 6, data: "cur", orderable: false },
                    { targets: 7, data: "item_count", orderable: false },
                    {
                        targets: 8, orderable: false,
                        data: "RowNumber",
                        className: "dt-body-center",
                        width: 60,
                        render: function (data, type, row, meta) {
                            var html = '';
                            if (_isUpdate == "True") {
                                html += '<button class="btn btn-default btn-round" onclick="location.href=\'' + _urlEdit + '?adjust_num=' + row.adjust_num +'\'"><i class="feather-icon icon-edit"></i></button>';
                            }
                            else {
                                html += '<button class="btn btn-default btn-round" onclick="location.href=\'' + _urlEdit + '?adjust_num=' + row.adjust_num +'\'"><i class="feather-icon icon-edit"></i></button>';
                            }

                            if (_isDelete == "True") {
                                html += '<button class="btn btn-delete  btn-round" onclick="GlAdjustIndex.delete(' + row.adjust_num + ')"><i class="feather-icon icon-trash-2"></i></button>';
                            }
                            else {
                                html += '<button class="btn btn-delete  btn-round" disabled><i class="feather-icon icon-trash-2"></i></button>';
                            }

                            return html;
                        }
                    }
                ],
            fixedColumns: {
                leftColumns: 1,
                rightColumns: 1
            },
            fnPreDrawCallback: function () {
                tableResult.Id.DataTable().columns(2).search(postingDate.getValue());
            }
        });
    },
    draw: function () {
        tableResult.Id.DataTable().columns(1).search(adjustNum.getValue());
        tableResult.Id.DataTable().columns(2).search(postingDate.getValue());
        tableResult.Id.DataTable().columns(4).search(transPort.getValue());
        tableResult.Id.DataTable().columns(5).search(transNo.getValue());
        tableResult.Id.DataTable().columns(6).search(cur.getValue());
        tableResult.Id.DataTable().draw();
    }
};

var searchForm = {
    Id: $('#search-form'),
    init: function () {
        this.Id.on('submit', function (e) {
            e.preventDefault();
            tableResult.draw();
        });

        this.Id.on('reset', function (e) {
            e.preventDefault();
            adjustNum.clearValue();
            transNo.clearValue();
            transPort.clearValue();
            cur.clearValue();
            postingDate.setValue(b_date);
            tableResult.draw();
        });
    }
};

var GlAdjustIndex = (function($) {
    return {
        init: function (settings) {
            _isUpdate = settings.isUpdate;
            _isDelete = settings.isDelete;
            _urlSearch = settings.urlSearch;
            _urlEdit = settings.urlEdit;
            _urlDelete = settings.urlDelete;
            cur.init();
            transPort.init();
            postingDate.init();
            tableResult.init();
            searchForm.init();
        },
        delete: function (adjustNum) {

            swal({
                title: "Comfirm Delete?",
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
                    $.ajax({
                        type: "POST",
                        url: _urlDelete,
                        content: "application/json; charset=utf-8",
                        dataType: "json",
                        data: {
                            adjust_num: adjustNum
                        },
                        success: function (d) {
                            $('.spinner').css('display', 'none');
                            if (d.Success) {
                                tableResult.draw();
                            } else {
                                swal("Failed!", "Error : " + d.Message, "error");
                            }
                        },
                        error: function (d) {
                            $('.spinner').css('display', 'none');
                        }
                    });
                }
            });
        }
    };
})(jQuery);