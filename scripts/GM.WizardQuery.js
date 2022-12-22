var WizardQuery = (function ($) {
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

    var textQuery = {
        Id: $('#text-query'),
        getValue: function() {
            return this.Id.val();
        },
        validate: function () {
            if (this.getValue().toLowerCase().indexOf("insert") === 0) {
                return false;
            }
            if (this.getValue().toLowerCase().indexOf("update") === 0) {
                return false;
            }
            if (this.getValue().toLowerCase().indexOf("delete") === 0) {
                return false;
            }
            if (this.getValue().toLowerCase().indexOf("alter") === 0) {
                return false;
            }
            if (this.getValue().toLowerCase().indexOf("create") === 0) {
                return false;
            }
            return true;
        }
    };

    var textNonQuery = {
        Id: $('#text-nonquery'),
        getValue: function() {
            return this.Id.val();
        }
    };

    var tableResult = {
        Id: $('#tableResult'),
        init: function() {
            
        },
        clearData: function() {
            if ($.fn.dataTable.isDataTable('#tableResult')) {
                table.destroy();
                tableResult.Id.empty();
            }
        },
        reload: function (data) {
            if (data.Columns.length) {
                table = tableResult.Id.DataTable({
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
        executeQuery: function (urlService, type) {
            swal({
                title: "Are you sure?",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-danger",
                confirmButtonText: "Yes",
                cancelButtonText: "No",
                closeOnConfirm: true,
                closeOnCancel: true
            }, function (isConfirm) {
                if (isConfirm) {
                    alertMsg.clearMessage();
                    tableResult.clearData();
                    $('.spinner').css('display', 'block');

                    var postData;
                    if (type == 'query') {
                        if (!textQuery.validate()) {
                            $('.spinner').css('display', 'none');
                            alertMsg.setError("Don't have 'insert', 'update', 'delete', 'alter', 'create' in query.");
                            return;
                        }

                        postData = {
                            TextQuery: textQuery.getValue()
                        };
                    } else {
                        postData = {
                            TextQuery: textNonQuery.getValue()
                        };
                    }

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
                                tableResult.reload(data);
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
                }
            });
        },
        exportQuery: function (urlService) {
            alertMsg.clearMessage();
            if (!textQuery.validate()) {
                $('.spinner').css('display', 'none');
                alertMsg.setError("Don't have 'insert', 'update', 'delete', 'alter', 'create' in query.");
                return;
            }

            var formId = $('#queryForm');
            formId.attr('action', urlService);
            formId.submit();
        },
        exportQuerySQL: function(urlService) {
            
        }
    };
})(jQuery);