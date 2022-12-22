var DWH = (function ($) {
    var table = "";

    var alertMsg = {
        Id: $('#alert-msg'),
        clearMessage: function () {
            this.Id.html();
        },
        setSuccess: function (msg) {
            var html = "<div class='alert alert-success alert-dismissible' role='alert'> " +
                "<button type='button' class='close' data-dismiss='alert' aria-label='Close'> " +
                "<span aria-hidden='true'>&times;</span></button>" + msg +
                "</div>";
            this.Id.html(html);
        },
        setError: function (msg) {
            var html = "<div class='alert alert-danger alert-dismissible' role='alert'> " +
                "<button type='button' class='close' data-dismiss='alert' aria-label='Close'> " +
                "<span aria-hidden='true'>&times;</span></button>" + msg +
                "</div>";
            this.Id.html(html);
        }
    };

    var fileType = {
        Id: $('#ddl_file_type'),
        init: function () {
            this.bindEventHandler();
        },
        bindEventHandler: function () {
            var txt_search = $('#txt_file_type');
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
        getValue: function () {
            return $.trim($('#file_type').val());
        },
        validate: function () {
            var errorId = $("#file_type_error");
            if (this.getValue().length) {
                errorId.text("");
                this.Id.removeClass("input-validation-error");
                return true;
            } else {
                errorId.text("File Type field is required.");
                this.Id.addClass("input-validation-error");
                return false;
            }
        }
    };

    var dataTable = {
        Id: $('#dataTable'),
        init: function () {

        },
        clearData: function () {
            if ($.fn.dataTable.isDataTable('#dataTable')) {
                table.destroy();
                dataTable.Id.empty();
            }
        },
        reload: function (data) {
            if (data.Columns.length) {
                table = dataTable.Id.DataTable({
                    scrollY: '80vh',
                    scrollX: true,
                    processing: true,
                    "data": data.Data,
                    "columns": data.Columns,
                    paging: true
                });
            }
        }
    };

    return {
        init: function (urlService) {
            fileType.init();
        },
        search: function (urlService) {

            if (!fileType.validate()) {
                return;
            }

            dataTable.clearData();
            $('.spinner').css('display', 'block');

            var postData = {
                file_type: fileType.getValue(),
                start: 1,
                length: 10
            };

            $.ajax({
                url: urlService,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'JSON',
                traditional: true,
                data: JSON.stringify(postData),
                success: function (data) {
                    $('.spinner').css('display', 'none');

                    if (data.Status == "Success") {
                        alertMsg.setSuccess(data.Message);
                        dataTable.reload(data);
                    }
                    else if (data.Status == "Error") {
                        alertMsg.setError(data.Message);
                    }
                },
                error: function (jqXhr, textStatus) {
                    $('.spinner').css('display', 'none');

                    if (textStatus === "error") {
                        var objJson = jQuery.parseJSON(jqXhr.responseText);

                        if (Object.prototype.toString.call(objJson) === '[object Array]' &&
                            objJson.length == 0) {
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
                }
            });
        },
        exportExcel: function (urlService) {
            if (!fileType.validate()) {
                return;
            }

            var formId = $('#search-form');
            formId.attr('action', urlService);
            formId.submit();
        }
    };

})(jQuery);