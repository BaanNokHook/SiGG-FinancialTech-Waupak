var ImportData = (function ($) {
    var arrExt = [];
    var formId = $('#import-form');

    var importTo = {
        Id: $('#ddl_ImportTo'),
        init: function () {
            this.bindEventHandler();
        },
        bindEventHandler: function () {
        },
        getValue: function () {
            return $('#ImportTo').val();
        },
        validate: function () {
            var errorId = $("#import_to_error");
            if (this.getValue().length) {
                errorId.text("");
                this.Id.removeClass("input-validation-error");
                return true;
            } else {
                errorId.text("Import To field is required.");
                this.Id.addClass("input-validation-error");
                return false;
            }
        }
    };

    var importDate = {
        Id: $('#txt_import_date'),
        getValue: function () {
            return $.trim(this.Id.val());
        },
        validate: function () {
            var errorId = $("#import_date_error");
            if (this.getValue().length) {
                errorId.text("");
                this.Id.removeClass("input-validation-error");
                return true;
            } else {
                errorId.text("Date field is required.");
                this.Id.addClass("input-validation-error");
                return false;
            }
        }
    };

    var txtUploadFile = {
        Id: $('#txtUploadFile'),
        init: function () {
        },
        getValue: function () {
            return this.Id.val();
        },
        validate: function () {
            var errorId = $("#upload_file_error");

            var fileName = this.getValue();
            //var to = importTo.getValue();

            if (!fileName.length) {
                errorId.text("File field is required.");
                this.Id.addClass("input-validation-error");
                return false;
            }

            var ext = fileName.substring(fileName.lastIndexOf('.') + 1);
            if (arrExt.length) {
                if ($.inArray(ext, arrExt) == '-1') {
                    var extStr = arrExt.toString();
                    errorId.text("Please select '" + extStr + "' only.");
                    this.Id.addClass("input-validation-error");
                    return false;
                }
            }

            errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        }
    };

    var validateSubmit = function () {
        var isValidate = true;

        if (!txtUploadFile.validate()) {
            isValidate = false;
        }

        if (!importDate.validate()) {
            isValidate = false;
        }

        //if (!importTo.validate()) {
        //    isValidate = false;
        //}

        return isValidate;
    };

    return {
        init: function (settings) {
            arrExt = settings.arrExt;

            importTo.init();

            $('#btnImport').on('click', function (e) {
                e.preventDefault();
                $('.spinner').css('display', 'block');
                if (!validateSubmit()) {
                    $('.spinner').css('display', 'none');
                    return;
                }

                setTimeout(function () {
                    $.ajax({
                        url: "/ImportExportData/CheckRemark",
                        type: "GET",
                        dataType: "JSON",
                        data: {
                            asofDate: importDate.getValue()
                        },
                        //async: false,
                        success: function(result) {
                            if (result.Status == "Success") {
                                if (result.IsCheck) {
                                    setTimeout(function () {
                                        swal({
                                            title: "Selected date has reviewed, Require confirm import again?",
                                            type: "warning",
                                            showCancelButton: true,
                                            confirmButtonClass: "btn-danger",
                                            confirmButtonText: "Yes",
                                            cancelButtonText: "No",
                                            closeOnConfirm: true,
                                            closeOnCancel: true
                                        },
                                        function(isConfirm) {
                                            if (isConfirm) {
                                                formId.submit();
                                            } else {
                                                $('.spinner').css('display', 'none');
                                            }
                                        });
                                    },
                                    100);
                                } else {
                                    formId.submit();
                                }
                            } else {
                                setTimeout(function() {
                                    swal("Fail", result.Message, "error");
                                    $('.spinner').css('display', 'none'); // Close Loading
                                },
                                100);
                            }
                        }
                    });
                }, 100);
            });
        }
    };
})(jQuery);