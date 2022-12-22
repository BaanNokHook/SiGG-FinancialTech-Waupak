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

var fname = {
    Id: $('#FormSearch_fname'),
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

var position = {
    Id: $('#FormSearch_position'),
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
                    { targets: 1, data: "fname", width: 300, orderable: true },
                    { targets: 2, data: "position", orderable: true },
                    {
                        targets: 3, data: "active_flag", width: 60, orderable: false,
                        render: function (data, type, row, meta) {
                            if (row.active_flag == '1') {
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
                                html += '<button class="btn btn-default btn-round" key="' + row.id + '" form-mode="edit"   onclick="UserSignNameIndex.modalEdit(\'' + row.id + '\')"><i class="feather-icon icon-edit"></i></button>';
                            }
                            else {
                                html += '<button class="btn btn-default btn-round" key="' + row.id + '" form-mode="view"   onclick="UserSignNameIndex.modelView(\'' + row.id + '\')"><i class="feather-icon icon-edit"></i></button>';
                            }

                            if (_isDelete == "True") {
                                html += '<button class="btn btn-delete  btn-round" key="' + row.id + '" form-mode="delete" onclick="UserSignNameIndex.modelDelete(\'' + row.id + '\')"><i class="feather-icon icon-trash-2"></i></button>';
                            }
                            else {
                                html += '<button class="btn btn-delete  btn-round" key="' + row.id + '" form-mode="delete"  disabled><i class="feather-icon icon-trash-2"></i></button>';
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
        tableResult.Id.DataTable().columns(1).search(fname.getValue());
        tableResult.Id.DataTable().columns(2).search(position.getValue());
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
            fname.clearValue();
            position.clearValue();
            tableResult.draw();
        });
    }
};

//#endregion

//#region Modal Slide

var formAction_id = {
    Id: $('#FormAction_id'),
    clearValue: function () {
        this.setValue(null);
    },
    setValue: function (value) {
        this.Id.val(value);
    },
    getValue: function () {
        return this.Id.val();
    }
};

var formAction_fname = {
    Id: $('#FormAction_fname'),
    errorId: $("#fname_error"),
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
            this.errorId.text("Full Name field is required.");
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

var formAction_position = {
    Id: $('#FormAction_position'),
    errorId: $("#position_error"),
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
            this.errorId.text("Position field is required.");
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

var formAction_activeFlag = {
    Id: $('input:radio[id="FormAction_active_flag"]'),
    init: function () {
        //this.Id.change(function() {
        //    if (formAction_displayFlag.getValue() == '1') {

        //    }
        //});
    },
    clearValue: function () {
        this.Id.filter('[value="true"]').prop("checked", true).attr("checked", "checked");
        this.Id.filter('[value="false"]').prop("checked", false).removeAttr("checked");
    },
    enabled: function () {
        this.Id.removeAttr('disabled');
    },
    disabled: function () {
        this.Id.attr('disabled', 'disabled');
    },
    setValue: function (value) {
        if (value == '1') {
            this.Id.filter('[value="true"]').prop("checked", true).attr("checked", "checked");
            this.Id.filter('[value="false"]').prop("checked", false).removeAttr("checked");
        } else {
            this.Id.filter('[value="false"]').prop("checked", true).attr("checked", "checked");
            this.Id.filter('[value="true"]').prop("checked", false).removeAttr("checked");
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
                                    setTimeout(function () {
                                        swal("Deleted!", "Delete Success.", "success");
                                        modalSlide.close();
                                        tableResult.draw();
                                    }, 100);
                                } else {
                                    setTimeout(function() {
                                        swal("Deleted!", "Error : " + result.Message, "error");
                                    }, 100);
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
        this.setTitle('Add User Sign Name');
        this.clearForm();
        btnSave.init('Create', _urlCreate);
        btnCancel.init();

        this.open();
    },
    initEdit: function (id) {
        this.setTitle('Edit User Sign Name');
        this.clearForm();
        btnSave.init('Update', _urlEdit);
        btnCancel.init();

        formAction.setForm(id);

        this.open();
    },
    initView: function (id) {
        this.setTitle('View User Sign Name');
        this.clearForm();
        btnSave.init('Update', _urlEdit);
        btnSave.disabled();
        btnCancel.init();

        formAction_fname.disabled();
        formAction_position.disabled();
        formAction_activeFlag.disabled();
        formAction.setForm(id);

        this.open();
    },
    initDelete: function (id) {
        this.setTitle('Delete User Sign Name');
        this.clearForm();
        btnSave.init('Delete', _urlDelete);
        btnCancel.init();

        formAction_fname.disabled();
        formAction_position.disabled();
        formAction_activeFlag.disabled();
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
        formAction_id.clearValue();
        formAction_fname.clearValue();
        formAction_fname.enabled();
        formAction_position.clearValue();
        formAction_position.enabled();
        formAction_activeFlag.clearValue();
        formAction_activeFlag.enabled();
    },
    validateForm: function () {
        var isValidate = true;

        if (!formAction_position.validate()) {
            isValidate = false;
        }

        if (!formAction_fname.validate()) {
            isValidate = false;
        }

        return isValidate;
    }
};

var formAction = {
    Id: $('#action-form'),
    setForm: function (id) {
        var dataToPost = {
            id: id
        };

        $.ajax({
            type: "POST",
            url: _urlGetEdit,
            content: "application/json; charset=utf-8",
            dataType: "json",
            data: dataToPost,
            success: function (d) {
                if (d.Success) {
                    if (d.Data.UserSignNameResultModel.length > 0) {
                        var data = d.Data.UserSignNameResultModel[0];
                        formAction_id.setValue(id);
                        formAction_fname.setValue(data.fname);
                        formAction_position.setValue(data.position);
                        formAction_activeFlag.setValue(data.active_flag);
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
            id: formAction_id.getValue(),
            fname: formAction_fname.getValue(),
            position: formAction_position.getValue(),
            active_flag: formAction_activeFlag.getValue()
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
            id: formAction_id.getValue()
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

var UserSignNameIndex = (function ($) {
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
