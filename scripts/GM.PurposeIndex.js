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

//#region Search Form

var purpose = {
    Id: $('#FormSearch_purpose'),
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

var description = {
    Id: $('#FormSearch_description'),
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
                    { targets: 0, data: "RowNumber", width: 60, orderable: false },
                    { targets: 1, data: "purpose", width: 100, orderable: true },
                    { targets: 2, data: "description", orderable: true },
                    {
                        targets: 3, data: "display_flag", width: 60, orderable: false,
                        render: function (data, type, row, meta) {
                            if (row.display_flag == '1') {
                                return 'Yes';
                            } else {
                                return 'No';
                            }
                        }
                    },
                    {
                        targets: 4, orderable: false,
                        data: "RowNumber",
                        className: "dt-body-center",
                        width: 60,
                        render: function (data, type, row, meta) {
                            var html = '';
                            if (_isUpdate == "True") {
                                html += '<button class="btn btn-default btn-round" key="' + row.purpose + '" form-mode="edit"   onclick="PurposeIndex.modalEdit(\'' + row.purpose + '\')"><i class="feather-icon icon-edit"></i></button>';
                            }
                            else {
                                html += '<button class="btn btn-default btn-round" key="' + row.purpose + '" form-mode="view"   onclick="PurposeIndex.modelView(\'' + row.purpose + '\')"><i class="feather-icon icon-edit"></i></button>';
                            }

                            if (_isDelete == "True") {
                                html += '<button class="btn btn-delete  btn-round" key="' + row.purpose + '" form-mode="delete" onclick="PurposeIndex.modelDelete(\'' + row.purpose + '\')"><i class="feather-icon icon-trash-2"></i></button>';
                            }
                            else {
                                html += '<button class="btn btn-delete  btn-round" key="' + row.purpose + '" form-mode="delete"  disabled><i class="feather-icon icon-trash-2"></i></button>';
                            }

                            return html;
                        }
                    }
                ],
            fixedColumns: {
                leftColumns: 1,
                rightColumns: 1
            }
        });
    },
    draw: function () {
        tableResult.Id.DataTable().columns(1).search(purpose.getValue());
        tableResult.Id.DataTable().columns(2).search(description.getValue());
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
            purpose.clearValue();
            description.clearValue();
            tableResult.draw();
        });
    }
};

//#endregion

//#region Modal Slide

var formAction_purpose = {
    Id: $('#FormAction_purpose'),
    errorId: $("#purpose_error"),
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
            this.errorId.text("Purpose Code field is required.");
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

var formAction_description = {
    Id: $('#FormAction_description'),
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

var formAction_displayFlag = {
    Id: $('input:radio[id="FormAction_display_flag"]'),
    init: function () {
        //this.Id.change(function() {
        //    if (formAction_displayFlag.getValue() == '1') {

        //    }
        //});
    },
    clearValue: function () {
        this.Id.filter('[value="1"]').prop("checked", true).attr("checked", "checked");
        this.Id.filter('[value="0"]').prop("checked", false).removeAttr("checked");
    },
    enabled: function () {
        this.Id.removeAttr('disabled');
    },
    disabled: function () {
        this.Id.attr('disabled', 'disabled');
    },
    setValue: function (value) {
        if (value == '1') {
            this.Id.filter('[value="1"]').prop("checked", true).attr("checked", "checked");
            this.Id.filter('[value="0"]').prop("checked", false).removeAttr("checked");
        } else {
            this.Id.filter('[value="0"]').prop("checked", true).attr("checked", "checked");
            this.Id.filter('[value="1"]').prop("checked", false).removeAttr("checked");
        }
    },
    getValue: function () {
        return this.Id.filter(':checked').val();
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
        this.setTitle('Add Purpose');
        this.clearForm();
        btnSave.init('Create', _urlCreate);
        btnCancel.init();

        this.open();
    },
    initEdit: function (purpose) {
        this.setTitle('Edit Purpose');
        this.clearForm();
        btnSave.init('Update', _urlEdit);
        btnCancel.init();

        formAction_purpose.disabled();
        formAction.setForm(purpose);

        this.open();
    },
    initView: function (purpose) {
        this.setTitle('View Purpose');
        this.clearForm();
        btnSave.init('Update', _urlEdit);
        btnSave.disabled();
        btnCancel.init();

        formAction_purpose.disabled();
        formAction_description.disabled();
        formAction_displayFlag.disabled();
        formAction.setForm(purpose);

        this.open();
    },
    initDelete: function (purpose) {
        this.setTitle('Delete Purpose');
        this.clearForm();
        btnSave.init('Delete', _urlDelete);
        btnCancel.init();

        formAction_purpose.disabled();
        formAction_description.disabled();
        formAction_displayFlag.disabled();
        formAction.setForm(purpose);

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
        formAction_purpose.clearValue();
        formAction_purpose.enabled();
        formAction_description.clearValue();
        formAction_description.enabled();
        formAction_displayFlag.clearValue();
        formAction_displayFlag.enabled();
    },
    validateForm: function () {
        var isValidate = true;

        if (!formAction_description.validate()) {
            isValidate = false;
        }

        if (!formAction_purpose.validate()) {
            isValidate = false;
        }

        return isValidate;
    }
};

var formAction = {
    Id: $('#action-form'),
    setForm: function (purpose) {
        var dataToPost = {
            purpose: purpose
        };

        $.ajax({
            type: "POST",
            url: _urlGetEdit,
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: dataToPost,
            success: function (d) {
                if (d.Success) {
                    if (d.Data.PurposeResultModel.length > 0) {
                        var data = d.Data.PurposeResultModel[0];

                        formAction_purpose.setValue(data.purpose);
                        formAction_description.setValue(data.description);
                        formAction_displayFlag.setValue(data.display_flag);
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
            purpose: formAction_purpose.getValue(),
            description: formAction_description.getValue(),
            display_flag: formAction_displayFlag.getValue()
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
            purpose: formAction_purpose.getValue()
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

var PurposeIndex = (function ($) {
    return {
        init: function (settings) {
            _isUpdate = settings.isUpdate;
            _isDelete = settings.isDelete;
            _urlCreate = settings.urlCreate;
            _urlEdit = settings.urlEdit;
            _urlGetEdit = settings.urlGetEdit;
            _urlDelete = settings.urlDelete;
            tableResult.init(settings.urlIndex);

            searchForm.init();

            btnAdd.init();
        },
        modalEdit: function (purpose) {
            modalSlide.initEdit(purpose);
        },
        modelView: function (purpose) {
            modalSlide.initView(purpose);
        },
        modelDelete: function (purpose) {
            modalSlide.initDelete(purpose);
        }
    };

})(jQuery);
