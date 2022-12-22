var SwiftManual = (function ($) {
    var _urlManual;
    var _urlGenFileName;

    var focusInput = function (input) {
        var center = $(window).height() / 2;
        var top = $(input).offset().top;
        if (top > center) {
            $(window).scrollTop(top - center);
        }
        input.addClass("input-validation-error");
        input.focus();
    };

    var mtCode = {
        Id: $('#ddl_mt_code'),
        init: function() {
            this.bindEventHandler();
        },
        bindEventHandler: function () {
            var txt_search = $('#txt_mt_code');
            this.Id.click(function() {
                var data = { datastr: null };
                GM.Utility.DDLAutoComplete(txt_search, data, null);
                txt_search.val("");
            });

            txt_search.keyup(function() {
                var data = { datastr: this.value };
                GM.Utility.DDLAutoComplete(this, data, null);

            });

            $('#ul_mt_code').on("click", ".searchterm", function (event) {
                fileName.setValue(null);
            });
        },
        getValue: function () {
            return $.trim($('#mt_code').val());
        },
        validate: function () {
            var errorId = $("#mt_code_error");
            if (this.getValue().length) {
                errorId.text("");
                this.Id.removeClass("input-validation-error");
                return true;
            } else {
                errorId.text("MT Code field is required.");
                this.Id.addClass("input-validation-error");
                focusInput(this.Id);
                return false;
            }
        }
    };

    var fileName = {
        Id: $('#file_name'),
        getValue: function () {
            return $.trim(this.Id.val());
        },
        setValue: function (value) {
            this.Id.val(value);
        },
        validate: function () {
            var errorId = $("#file_name_error");
            if (this.getValue().length) {
                errorId.text("");
                this.Id.removeClass("input-validation-error");
                return true;
            } else {
                errorId.text("File name field is required.");
                this.Id.addClass("input-validation-error");
                focusInput(this.Id);
                return false;
            }
        }
    };

    var resultText = {
        Id: $('#result'),
        getValue: function() {
            return $.trim(this.Id.val());
        },
        validate: function () {
            var errorId = $("#result_error");
            if (this.getValue().length) {
                errorId.text("");
                this.Id.removeClass("input-validation-error");
                return true;
            } else {
                errorId.text("Text field is required.");
                this.Id.addClass("input-validation-error");
                focusInput(this.Id);
                return false;
            }
        }
    };

    var validateSubmit = function() {
        var isValidate = true;
        if (!mtCode.validate()) {
            isValidate = false;
        }
        if (!resultText.validate()) {
            isValidate = false;
        }
        return isValidate;
    };

    var validateText = function () {
        var code = mtCode.getValue().replace('MT', '');
        var reg = new RegExp('^(.+)({)(.+)(' + code + ')(.+)(})');
        if (!reg.test(resultText.getValue())) {
            swal("Failed!", "MT Code selected not match Text file.", "error");
            return false;
        } else {
            return true;
        }
    };

    return {
        init: function (settings) {
            _urlManual = settings.urlManual;
            _urlGenFileName = settings.urlGenFileName;

            mtCode.init();

            $('#btnSftp').on('click', function(e) {
                e.preventDefault();

                if (!validateSubmit()) {
                    return;
                }

                if (!fileName.validate()) {
                    swal("Failed!", "Please press the button 'Gen File name'.", "error");
                    return;
                }

                if (!validateText()) {
                    return;
                }

                swal({
                    title: "Comfirm Send SFTP?",
                    text: "File name: " + fileName.getValue(),
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

                        var dataToPost = {
                            file_name: fileName.getValue(),
                            result: resultText.getValue()
                        };

                        $.ajax({
                            type: "POST",
                            url: _urlManual,
                            content: "application/json; charset=utf-8",
                            dataType: "json",
                            data: dataToPost,
                            success: function (d) {
                                $('.spinner').css('display', 'none');
                                if (d.Success) {
                                    setTimeout(function () {
                                        swal({
                                                title: "Complete",
                                                text: "Successfully",
                                                type: "success",
                                                showCancelButton: false,
                                                confirmButtonClass: "btn-danger",
                                                confirmButtonText: "Yes",
                                                cancelButtonText: "No",
                                                closeOnConfirm: true,
                                                closeOnCancel: true
                                            },
                                            function (isConfirm) {
                                                //window.location.href = _urlManual;
                                            });
                                    }, 100);
                                } else {
                                    swal("Failed!", "Error : " + d.Message, "error");
                                }
                            },
                            error: function (d) {
                            }
                        });
                    }
                });
            });

            $('#btnSave').on('click', function (e) {
                e.preventDefault();
                if (validateSubmit()) {
                    $('.spinner').css('display', 'block');
                    var dataToPost = {
                        mt_code: mtCode.getValue()
                    };

                    $.ajax({
                        type: "POST",
                        url: _urlGenFileName,
                        content: "application/json; charset=utf-8",
                        dataType: "json",
                        data: dataToPost,
                        success: function(d) {
                            $('.spinner').css('display', 'none');
                            if (d.Success) {
                                fileName.setValue(d.FileName);
                            } else {
                                swal("Failed!", "Error : " + d.Message, "error");
                            }
                        },
                        error: function(d) {
                        }
                    });
                }
                return;
            });
        }
    };
})(jQuery);