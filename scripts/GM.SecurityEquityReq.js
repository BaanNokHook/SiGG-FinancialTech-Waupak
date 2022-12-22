var SecurityEquityReq = (function ($) {
    var formId = $("#action-form");

    var focusInput = function (input) {
        input.addClass("input-validation-error");
        //input.focus();
    };

    var budate = $("#BusinessDate").text();

    var formatmmddyyyydate = budate.split("/");
    formatmmddyyyydate = formatmmddyyyydate[1] + "/" + formatmmddyyyydate[0] + "/" + formatmmddyyyydate[2];
    var b_date = new Date(formatmmddyyyydate);

    //#region Modal Slide
    var asofDate = {
        Id: $('#FormAction_asof_date'),
        errorId: $("#asofdate_error"),
        init: function () {
            this.setValue(b_date);
            this.bindEventHandler();
        },
        bindEventHandler: function () {
        },
        getValue: function () {
            return $.trim(this.Id.val());
        },
        setValue: function (value) {
            this.Id.data("DateTimePicker").date(value);
        },
        disabled: function () {
            this.Id.attr('disabled', 'disabled');
        },
        validate: function () {
            if (!this.getValue().length) {
                this.errorId.text("AsofDate field is required.");
                this.Id.addClass("input-validation-error");
                focusInput(this.Id);
                return false;
            } else {
                this.errorId.text("");
                this.Id.removeClass("input-validation-error");
                return true;
            }
        },
        clearValue: function () {
            this.setValue(b_date);
            this.bindEventHandler();
        }
    };

    var equityCode = {
        Id: $('#FormAction_code'),
        errorId: $("#equity_code_error"),
        clearValue: function () {
            this.setValue(null);
            this.errorId.text("");
            this.Id.removeClass("input-validation-error");
        },
        enabled: function () {
            this.Id.removeAttr('disabled');
        },
        disabled: function () {
            this.Id.attr("disabled", "disabled");
        },
        setValue: function (value) {
            this.Id.val(value);
        },
        getValue: function () {
            return $.trim(this.Id.val());
        },
        validate: function () {
            if (!this.getValue().length) {
                this.errorId.text("Code field is required.");
                this.Id.addClass("input-validation-error");
                focusInput(this.Id);
                return false;
            } else {
                this.errorId.text("");
                this.Id.removeClass("input-validation-error");
                return true;
            }
        },
        clearValue: function () {
            this.setValue("");
        }
    };

    var btnReq = {
        Id: $('#btnReq'),
        init: function () {
            this.Id.on('click', function (e) {
                e.preventDefault();
                if (!modalSlide.validateForm()) {
                    return;
                }

                swal({
                    title: "Do you want to requset equity code " + equityCode.getValue() + "?",
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
                                url: formId.attr('action'),
                                content: "application/json; charset=utf-8",
                                dataType: "json",
                                data: {
                                    asof_date: asofDate.getValue(),
                                    code: equityCode.getValue()
                                },
                                success: function (d) {
                                    $('.spinner').css('display', 'none');
                                    if (d.Success && d.RefCode == 0) {
                                        setTimeout(function () {
                                            swal({
                                                title: "Complete",
                                                text: "Successfully",
                                                type: "success",
                                                showCancelButton: false,
                                                confirmButtonClass: "btn-success",
                                                confirmButtonText: "Yes",
                                                cancelButtonText: "No",
                                                closeOnConfirm: true,
                                                closeOnCancel: true
                                            },
                                                function (isConfirm) {
                                                    if (isConfirm) {
                                                        GM.Security.Table.draw();
                                                        modalSlide.close();
                                                    }
                                                }
                                            );
                                        }, 100);
                                    } else if (d.Success && d.RefCode == 1) {
                                        $('.spinner').css('display', 'none');
                                        setTimeout(function () {
                                            swal({
                                                title: "Warning",
                                                text: equityCode.getValue() + " Not Found From EQUITY",
                                                type: "warning",
                                                showCancelButton: false,
                                                confirmButtonClass: "btn-success",
                                                confirmButtonText: "Yes",
                                                cancelButtonText: "No",
                                                closeOnConfirm: true,
                                                closeOnCancel: true
                                            },
                                                function () {
                                                    GM.Security.Table.draw();
                                                }
                                            );
                                        }, 100);
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
            });
        },
        enabled: function () {
            this.Id.removeAttr('disabled');
        },
        disabled: function () {
            this.Id.attr("disabled", "disabled");
        }
    };

    var modalSlide = {
        Id: $('#action-form-modal'),
        initForm: function () {
            this.setTitle('Requset Equity Symbol');
            this.clearForm();
            btnReq.init();

            asofDate.init();
            this.open();
        },
        setTitle: function (text) {
            this.Id.find('#modalTitle').text(text);
        },
        open: function () {
            this.Id.modal('show');
        },
        close: function () {
            this.Id.modal('hide');
        },
        clearForm: function () {
            btnReq.Id.unbind("click");
            asofDate.clearValue();
            equityCode.clearValue();
        },
        validateForm: function () {
            var isValidate = true;
            if (!equityCode.validate()) {
                isValidate = false;
            }
            if (!asofDate.validate()) {
                isValidate = false;
            }
            return isValidate;
        }
    };

    //#endregion Modal Slide

    var btnAddEquity = {
        Id: $('#btnAddEquity'),
        init: function () {
            this.Id.on("click", function () {
                modalSlide.initForm();
            });
        },
        enabled: function () {
            this.Id.removeAttr('disabled');
        },
        disabled: function () {
            this.Id.attr("disabled", "disabled");
        }
    };
    return {
        initForm: function () {
            btnAddEquity.init();
        }
    };
})(jQuery);