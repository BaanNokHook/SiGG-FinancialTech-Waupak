var _isUpdate = "False";
var _isDelete = "False";
var _urlCreate;
var _urlEdit;
var _urlGetEdit;
var _urlDelete;

var focusInput = function (input) {
    var center = $(window).height() / 2;
    var top = $(input).offset().top;
    if (top > center) {
        $(window).scrollTop(top - center);
    }
    input.addClass("input-validation-error");
    //input.focus();
};

var formatDecimal = function (value, point) {
    const f = Math.pow(10, point);
    return (Math.floor(value * f) / f).toFixed(point);
};

//#region Search Form

var securityType = {
    Id: $("#ddl_security_type"),
    init: function () {
        this.bindEventHandler();
    },
    bindEventHandler: function () {
        var txt_search = $('#txt_security_type');
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
        this.setValue(null);

    },
    enabled: function () {
        this.Id.removeAttr('disabled');
    },
    disabled: function () {
        this.Id.attr("disabled", "disabled");
    },
    setText: function (text) {
        this.Id.find(".selected-data").text(text);
    },
    setValue: function (value) {
        $('#FormSearch_SECURITYTYPE_ID').val(value);
    },
    getValue: function () {
        return $.trim($('#FormSearch_SECURITYTYPE_ID').val());
    }

};

var couponType = {
    Id: $('#ddl_coupon_type'),
    init: function () {
        this.bindEventHandler();
    },
    bindEventHandler: function () {
        var txt_search = $('#txt_coupon_type');
        this.Id.click(function () {
            var data = { datastr: null };
            GM.Utility.DDLAutoComplete(txt_search, data, null);
            txt_search.val("");
        });

        txt_search.keyup(function () {

            var data = { datastr: couponType.getValue() };
            GM.Utility.DDLAutoComplete(this, data, null);

        });
    },
    clearValue: function () {
        this.Id.find(".selected-data").text("Select...");
        this.setValue(null);
    },
    enabled: function () {
        this.Id.removeAttr('disabled');
    },
    disabled: function () {
        this.Id.attr("disabled", "disabled");
    },
    setText: function (text) {
        this.Id.find(".selected-data").text(text);
    },
    setValue: function (value) {
        $('#FormSearch_COUPONTYPE_ID').val(value);
    },
    getValue: function () {
        return $.trim($('#FormSearch_COUPONTYPE_ID').val());
    }
};

var description = {
    Id: $('#FormSearch_DESCRIPTION'),
    clearValue: function () {
        this.setValue(null);
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
    }
};

var tableResult = {
    Id: $('#x-table-data'),
    init: function (urlSearch) {
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
                "url": urlSearch,
                "type": "POST",
                "error": function (jqXHR, textStatus, errorThrown) {
                    console.log(jqXHR);
                    console.log(textStatus);
                    console.log(errorThrown);
                }
            },
            columnDefs:
                [
                    { targets: 0, data: "RowNumber", orderable: false },
                    { targets: 1, data: "SECURITYTYPE", orderable: true },
                    { targets: 2, data: "COUPONTYPE", orderable: true },
                    {
                        targets: 3, data: "HAIRCUTMARGIN", orderable: false,
                        className: 'dt-body-right',
                        render: function (data, type, row, meta) {
                            var html = '';
                            if (data != null) {
                                html = formatDecimal(data, 6);
                            } else {
                                html = Number("0").toFixed(6);
                            }
                            return html;
                        }
                    },
                    {
                        targets: 4, data: "VARIATIONMARGIN", orderable: false,
                        className: 'dt-body-right',
                        render: function (data, type, row, meta) {
                            var html = '';
                            if (data != null) {
                                html = formatDecimal(data, 6);
                            } else {
                                html = Number("0").toFixed(6);
                            }
                            return html;
                        }
                    },
                    {
                        targets: 5, data: "YearStart", orderable: false,
                        className: 'dt-body-right',
                        render: function (data, type, row, meta) {
                            var html = '';
                            if (data != null) {
                                html = formatDecimal(data, 0);
                            }
                            else {
                                html = Number("0").toFixed(0);
                            }
                            return html;
                        }
                    },
                    {
                        targets: 6, data: "YearEnd", orderable: false,
                        className: 'dt-body-right',
                        render: function (data, type, row, meta) {
                            var html = '';
                            if (data != null) {
                                html = formatDecimal(data, 0);
                            } else {
                                html = Number("0").toFixed(0);
                            }
                            return html;
                        }
                    },
                    { targets: 7, data: "DESCRIPTION", orderable: true },
                    { targets: 8, data: "DESCRIPTION2", orderable: false },
                    {
                        targets: 9, orderable: false,
                        data: "RowNumber",
                        className: "dt-body-center",
                        width: 60,
                        render: function (data, type, row, meta) {
                            var html = '';
                            if (_isUpdate == "True") {
                                html += '<button class="btn btn-default btn-round" key="' + row.ID + '" form-mode="edit"   onclick="InitialMarginIndex.modalEdit(' + row.ID + ')"><i class="feather-icon icon-edit"></i></button>';
                            }
                            else {
                                html += '<button class="btn btn-default btn-round" key="' + row.ID + '" form-mode="view"   onclick="InitialMarginIndex.modelView(' + row.ID + ')"><i class="feather-icon icon-edit"></i></button>';
                            }

                            if (_isDelete == "True") {
                                html += '<button class="btn btn-delete  btn-round" key="' + row.ID + '" form-mode="delete" onclick="InitialMarginIndex.modelDelete(' + row.ID + ')"><i class="feather-icon icon-trash-2"></i></button>';
                            }
                            else {
                                html += '<button class="btn btn-delete  btn-round" key="' + row.ID + '" form-mode="delete"  disabled><i class="feather-icon icon-trash-2"></i></button>';
                            }

                            return html;
                        }
                    },
                    { targets: 10, data: "SECURITYTYPE_ID", visible: false },
                    { targets: 11, data: "COUPONTYPE_ID", visible: false }
                ],
            fixedColumns: {
                leftColumns: 1,
                rightColumns: 1
            }
        });
    },
    draw: function () {
        tableResult.Id.DataTable().columns(10).search(securityType.getValue());
        tableResult.Id.DataTable().columns(11).search(couponType.getValue());
        tableResult.Id.DataTable().columns(7).search(description.getValue());

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
            couponType.clearValue();
            securityType.clearValue();
            tableResult.draw();
        });
    }
};

//#endregion

//#region Modal Slide

var formAction_id = {
    Id: $('#FormAction_ID'),
    setValue: function (value) {
        this.Id.val(value);
    },
    getValue: function () {
        return this.Id.val();
    }
};

var formAction_couponType = {
    Id: $('#ddl_FormAction_coupon_type'),
    errorId: $("#coupon_type_error"),
    init: function () {
        this.bindEventHandler();
    },
    bindEventHandler: function () {
        var txt_search = $('#txt_FormAction_coupon_type');
        this.Id.click(function () {
            var data = { datastr: null };
            GM.Utility.DDLAutoComplete(txt_search, data, null);
            txt_search.val("");
        });

        txt_search.keyup(function () {

            var data = { datastr: couponType.getValue() };
            GM.Utility.DDLAutoComplete(this, data, null);

        });
    },
    clearValue: function () {
        this.setText("Select...");
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
    setText: function (text) {
        this.Id.find("span[name='COUPONTYPE']").text(text);
    },
    setValue: function (value) {
        $('#FormAction_COUPONTYPE_ID').val(value);
    },
    getValue: function () {
        return $.trim($('#FormAction_COUPONTYPE_ID').val());
    },
    validate: function () {
        if (!this.getValue().length) {
            this.errorId.text("Coupon Type field is required.");
            this.Id.addClass("input-validation-error");
            focusInput(this.Id);
            return false;
        } else {
            this.errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        }
    }

};

var formAction_securityType = {
    Id: $("#ddl_FormAction_security_type"),
    errorId: $("#security_type_error"),
    init: function () {
        this.bindEventHandler();
    },
    bindEventHandler: function () {
        var txt_search = $('#txt_FormAction_security_type');
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
        this.setText("Select...");
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
    setText: function (text) {
        this.Id.find("span[name='SECURITYTYPE']").text(text);
    },
    setValue: function (value) {
        $('#FormAction_SECURITYTYPE_ID').val(value);
    },
    getValue: function () {
        return $.trim($('#FormAction_SECURITYTYPE_ID').val());
    },
    validate: function () {
        if (!this.getValue().length) {
            this.errorId.text("Security Type field is required.");
            this.Id.addClass("input-validation-error");
            focusInput(this.Id);
            return false;
        } else {
            this.errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        }
    }
};

var formAction_haircutMargin = {
    Id: $('#FormAction_HAIRCUTMARGIN'),
    errorId: $("#haircutMargin_error"),
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
            this.errorId.text("Haircut field is required.");
            this.Id.addClass("input-validation-error");
            focusInput(this.Id);
            return false;
        } else {
            this.errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        }
    }
};

var formAction_variationMargin = {
    Id: $('#FormAction_VARIATIONMARGIN'),
    errorId: $("#variationMargin_error"),
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
            this.errorId.text("Var. Margin field is required.");
            this.Id.addClass("input-validation-error");
            focusInput(this.Id);
            return false;
        } else {
            this.errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        }
    }

};

var formAction_yearStart = {
    Id: $('#FormAction_YearStart'),
    errorId: $("#yearStart_error"),
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
            this.errorId.text("Year Start field is required.");
            this.Id.addClass("input-validation-error");
            focusInput(this.Id);
            return false;
        } else {

            if (this.getValue() > formAction_yearEnd.getValue()) {
                this.errorId.text("Year Start field is less than Year End.");
                this.Id.addClass("input-validation-error");
                focusInput(this.Id);
                return false;
            }

            this.errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        }

    }
};

var formAction_yearEnd = {
    Id: $('#FormAction_YearEnd'),
    errorId: $("#yearEnd_error"),
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
            this.errorId.text("Year End field is required.");
            this.Id.addClass("input-validation-error");
            focusInput(this.Id);
            return false;
        } else {

            if (this.getValue() < formAction_yearStart.getValue()) {
                this.errorId.text("Year End field is more than Year End.");
                this.Id.addClass("input-validation-error");
                focusInput(this.Id);
                return false;
            }

            this.errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        }
    }
};

var formAction_description = {
    Id: $('#FormAction_DESCRIPTION'),
    errorId: $("#description_error"),
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
            this.errorId.text("Description field is required.");
            this.Id.addClass("input-validation-error");
            focusInput(this.Id);
            return false;
        } else {
            this.errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        }
    }
};

var formAction_description2 = {
    Id: $('#FormAction_DESCRIPTION2'),
    errorId: $("#description2_error"),
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
            this.errorId.text("Description 2 field is required.");
            this.Id.addClass("input-validation-error");
            focusInput(this.Id);
            return false;
        } else {
            this.errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        }
    }
};

var btnSave = {
    Id: $('#btnSave'),
    init: function (text, action) {
        this.Id.on('click', function (e) {
            e.preventDefault();
            if (!modalSlide.validateForm()) {
                return;
            }

            if (text == 'Delete') {
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
                            var promise = formAction.delete();
                            promise.done(function (result) {

                                $('.spinner').css('display', 'none');
                                if (result.Success) {
                                    swal("Deleted!", "Delete Success.", "success");
                                    modalSlide.close();
                                    tableResult.draw();
                                } else {
                                    swal("Deleted!", "Error : " + result.responseText, "error");
                                }
                            }).fail(function (jqXhr, textStatus) {
                                $('.spinner').css('display', 'none');
                                alert("Reuest Failed: " + textStatus);
                            });
                        }
                    });
            }
            else {
                formAction.submit(action);
            }

        });
        btnSave.setText(text);
    },
    setText: function (text) {

        this.Id.text(text);
        if (text == 'Delete') {
            this.Id.removeClass('btn-primary');
            this.Id.addClass('btn-delete');
        } else {
            this.Id.removeClass('btn-delete');
            this.Id.addClass('btn-primary');
        }
    },
    enabled: function () {
        this.Id.removeAttr('disabled');
    },
    disabled: function () {
        this.Id.attr("disabled", "disabled");
    }
};

var btnCancel = {
    Id: $('#btnCancel'),
    init: function () {
        this.Id.on("click", function () {

        });
    }
};

var btnAdd = {
    Id: $('#btnAdd'),
    init: function () {
        this.Id.on("click", function () {
            modalSlide.initAdd();
        });
    }
};

var modalSlide = {
    Id: $('#action-form-modal'),
    initAdd: function () {
        this.setTitle('Add Initial Margin');
        this.clearForm();
        btnSave.init('Create', _urlCreate);
        btnCancel.init();
        formAction_securityType.init();
        formAction_couponType.init();

        this.open();
    },
    initEdit: function (id) {
        this.setTitle('Edit Initial Margin');
        this.clearForm();
        btnSave.init('Update', _urlEdit);
        btnCancel.init();
        formAction_securityType.init();
        formAction_securityType.disabled();
        formAction_couponType.init();
        formAction_couponType.disabled();
        formAction.setForm(id);

        this.open();
    },
    initView: function (id) {
        this.setTitle('View Initial Margin');
        this.clearForm();
        btnSave.init('Update', _urlEdit);
        btnSave.disabled();
        btnCancel.init();

        formAction_securityType.init();
        formAction_securityType.disabled();
        formAction_couponType.init();
        formAction_couponType.disabled();
        formAction_haircutMargin.disabled();
        formAction_variationMargin.disabled();
        formAction_yearStart.disabled();
        formAction_yearEnd.disabled();
        formAction_description.disabled();
        formAction_description2.disabled();
        formAction.setForm(id);

        this.open();
    },
    initDelete: function (id) {
        this.setTitle('Delete Initial Margin');
        this.clearForm();
        btnSave.init('Delete', _urlDelete);
        btnCancel.init();

        formAction_securityType.init();
        formAction_securityType.disabled();
        formAction_couponType.init();
        formAction_couponType.disabled();
        formAction_haircutMargin.disabled();
        formAction_variationMargin.disabled();
        formAction_yearStart.disabled();
        formAction_yearEnd.disabled();
        formAction_description.disabled();
        formAction_description2.disabled();
        formAction.setForm(id);

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
        btnSave.Id.unbind("click");
        formAction_couponType.clearValue();
        formAction_couponType.enabled();
        formAction_securityType.clearValue();
        formAction_securityType.enabled();
        formAction_haircutMargin.clearValue();
        formAction_haircutMargin.enabled();
        formAction_variationMargin.clearValue();
        formAction_variationMargin.enabled();
        formAction_yearStart.clearValue();
        formAction_yearStart.enabled();
        formAction_yearEnd.clearValue();
        formAction_yearEnd.enabled();
        formAction_description.clearValue();
        formAction_description.enabled();
        formAction_description2.clearValue();
        formAction_description2.enabled();
    },
    validateForm: function () {
        var isValidate = true;

        if (!formAction_description2.validate()) {
            isValidate = false;
        }

        if (!formAction_description.validate()) {
            isValidate = false;
        }

        if (!formAction_yearEnd.validate()) {
            isValidate = false;
        }

        if (!formAction_yearStart.validate()) {
            isValidate = false;
        }

        if (!formAction_variationMargin.validate()) {
            isValidate = false;
        }

        if (!formAction_haircutMargin.validate()) {
            isValidate = false;
        }

        if (!formAction_securityType.validate()) {
            isValidate = false;
        }

        if (!formAction_couponType.validate()) {
            isValidate = false;
        }

        return isValidate;
    }
};

var formAction = {
    Id: $('#action-form'),
    setForm: function (id) {
        var dataToPost = {
            Id: id
        };

        $.ajax({
            type: "POST",
            url: _urlGetEdit,
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: dataToPost,
            success: function (d) {
                if (d.Success) {
                    if (d.Data.InitialMarginResultModel.length > 0) {
                        var data = d.Data.InitialMarginResultModel[0];

                        formAction_id.setValue(data.ID);
                        formAction_couponType.setText(data.COUPONTYPE);
                        formAction_couponType.setValue(data.COUPONTYPE_ID);
                        formAction_securityType.setText(data.SECURITYTYPE);
                        formAction_securityType.setValue(data.SECURITYTYPE_ID);
                        formAction_haircutMargin.setValue(formatDecimal(data.HAIRCUTMARGIN, 6));
                        formAction_variationMargin.setValue(formatDecimal(data.VARIATIONMARGIN, 6));
                        formAction_yearStart.setValue(data.YearStart);
                        formAction_yearEnd.setValue(data.YearEnd);
                        formAction_description.setValue(data.DESCRIPTION);
                        formAction_description2.setValue(data.DESCRIPTION2);
                    }
                    else {
                        swal("Failed!", "Error : Data Not Found", "error");
                    }
                } else {
                    swal("Failed!", "Error : " + d.Message, "error");
                }
            },
            error: function (d) {
            }
        });


    },
    submit: function (action) {
        var dataToPost = {
            ID: formAction_id.getValue(),
            SECURITYTYPE_ID: formAction_securityType.getValue(),
            COUPONTYPE_ID: formAction_couponType.getValue(),
            HAIRCUTMARGIN: formAction_haircutMargin.getValue(),
            VARIATIONMARGIN: formAction_variationMargin.getValue(),
            YearStart: formAction_yearStart.getValue(),
            YearEnd: formAction_yearEnd.getValue(),
            DESCRIPTION: formAction_description.getValue(),
            DESCRIPTION2: formAction_description2.getValue()
        };

        $.ajax({
            type: "POST",
            url: action,
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: dataToPost,
            success: function (d) {
                if (d.Success) {
                    modalSlide.close();
                    tableResult.draw();
                } else {
                    swal("Failed!", "Error : " + d.Message, "error");
                }
            },
            error: function (d) {
            }
        });
    },
    delete: function () {

        var dataToPost = {
            ID: formAction_id.getValue()
        };

        return $.ajax({
            type: "POST",
            url: _urlDelete,
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: dataToPost
        });


    }
};

//#endregion

var InitialMarginIndex = (function ($) {
    return {
        numberOnly: function (obj) {
            obj.value = obj.value
                .replace(/[^\d.]/g, '')             // numbers and decimals only
        },
        numberOnlyAndDot: function (obj) {
            obj.value = obj.value
                .replace(/[^\d.]/g, '')             // numbers and decimals only
                .replace(/(^[\d]{3})[\d]/g, '$1')   // not more than 3 digits at the beginning
                .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
                .replace(/(\.[\d]{6})./g, '$1');    // not more than 6 digits after decimal
        },
        auto6digit: function (obj) {
            if (obj.value.length) {
                var nStr = obj.value;
                var x = nStr.split('.');
                var x1 = x[0];
                var x2 = '000000';

                if (x.length > 1) {
                    x2 = x[1];

                    var currentDigit = x[1].length;
                    if (currentDigit < 6) {
                        for (var i = currentDigit; i < 6; i++) {
                            x2 += '0';
                        }
                    }
                }

                if (x1 > 999) {
                    x1 = 999;
                }

                x1 = x1.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                return obj.value = x1 + '.' + x2;
            }
            return "";
        },
        init: function (settings) {
            _isUpdate = settings.isUpdate;
            _isDelete = settings.isDelete;
            _urlCreate = settings.urlCreate;
            _urlEdit = settings.urlEdit;
            _urlGetEdit = settings.urlGetEdit;
            _urlDelete = settings.urlDelete;
            tableResult.init(settings.urlIndex);

            securityType.init();
            couponType.init();
            searchForm.init();

            btnAdd.init();
        },
        modalEdit: function (id) {
            modalSlide.initEdit(id);
        },
        modelView: function (id) {
            modalSlide.initView(id);
        },
        modelDelete: function (id) {
            modalSlide.initDelete(id);
        }
    };

})(jQuery);
