var _isUpdate = "False";
var _isDelete = "False";
var _urlIndex;
var _urlCreate;
var _urlEdit;
var _urlGetDetail;
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
    var nStr = value.toString().replace(/,/g, '');
    var x = nStr.split('.');
    var x1 = x[0];
    var x2 = '00';

    if (x.length > 1) {
        x2 = x[1];

        var currentDigit = x[1].length;
        if (currentDigit < 2) {
            for (var i = currentDigit; i < 2; i++) {
                x2 += '0';
            }
        }
    }

    x1 = x1.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return x1 + '.' + x2;
};

var budate = $("#BusinessDate").text();

var formatmmddyyyydate = budate.split("/");
formatmmddyyyydate = formatmmddyyyydate[1] + "/" + formatmmddyyyydate[0] + "/" + formatmmddyyyydate[2];
var b_date = new Date(formatmmddyyyydate);

var tableResult = {
    Id: $('#x-table-data'),
    init: function () {
        this.Id.DataTable({
            dom: 'Bfrtip',
            searching: false,
            scrollY: '50vh',
            scrollX: true,
            order: [
                [0, "asc"]
            ],
            buttons: [],
            processing: true,
            data: [],
            paging: false,
            columnDefs:
                [
                    {
                        targets: 0, data: "RowNumber", orderable: false,
                        render: function(data, type, row, meta) {
                            return meta.row + 1;
                        }
                    },
                    { targets: 1, data: "account_no", orderable: false },
                    { targets: 2, data: "account_name", orderable: false },
                    { targets: 3, data: "dr_cr", orderable: false },
                    {
                        targets: 4, data: "amount", orderable: false,
                        className: 'dt-body-right',
                        render: function (data, type, row, meta) {
                            var html = '';
                            if (data != null) {
                                html = formatDecimal(data,2);
                            } else {
                                html = Number("0").toFixed(2);
                            }
                            return html;
                        }
                    },
                    {
                        targets: 5, data: "cost_center", orderable: false
                    },
                    {
                        targets: 6, data: "note", orderable: false
                    },
                    {
                        targets: 7, orderable: false,
                        data: "RowNumber",
                        className: "dt-body-center",
                        width: 60,
                        render: function (data, type, row, meta) {
                            var html = '';
                            if (_isUpdate == "True") {
                                html += '<button type="button" class="btn btn-default btn-round" onclick="GlAdjustForm.modalEdit(this)"><i class="feather-icon icon-edit"></i></button>';
                            }
                            else {
                                html += '<button type="button" class="btn btn-default btn-round" disabled><i class="feather-icon icon-edit"></i></button>';
                            }

                            if (_isDelete == "True") {
                                html += '<button type="button" class="btn btn-delete btn-round" onclick="GlAdjustForm.removeRow(this)"><i class="feather-icon icon-trash-2"></i></button>';
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
            }
        });
    },
    draw: function () {
        this.Id.DataTable().draw();
    },
    addRow: function (data) {
        this.Id.DataTable().row.add(data);
        this.draw();
    },
    updateRow: function (rowIndex, data) {
        this.Id.DataTable().row(rowIndex).data(data);

        this.Id.DataTable().column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });

        this.draw();
    },
    removeRow: function (obj) {
        this.getRow(obj).remove();

        this.Id.DataTable().column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });

        this.draw();
    },
    getRow: function(obj) {
       return this.Id.DataTable().row($(obj).parents('tr'));
    },
    getData: function() {
        return this.Id.DataTable().data();
    }
};

var adjustNum = {
    Id: $('#adjust_num'),
    enabled: function () {
        $('#divAdjustNum').removeAttr('style');
    },
    getValue: function() {
        return $.trim(this.Id.val());
    }
};

var transNo = {
    Id: $('#ddl_trans_no'),
    errorId: $('#trans_no_error'),
    init: function () {
        this.bindEventHandler();
    },
    bindEventHandler: function () {
        var txt_search = $('#txt_trans_no');
        this.Id.click(function () {
            var data = { trans_date: transDate.getValue(), trans_no: txt_search.val() };
            GM.Utility.DDLAutoComplete(txt_search, data, null);
            txt_search.val("");
        });

        txt_search.keyup(function () {
            var data = { trans_date: transDate.getValue(), trans_no: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        });
    },
    clearValue: function () {
        this.Id.find(".selected-data").text("Select...");
        this.Id.val(null);
        $('#trans_no').val(null);
    },
    getValue: function () {
        return $.trim($('#trans_no').val());
    },
    enabled: function () {
        this.Id.removeAttr('disabled');
    },
    disabled: function () {
        this.Id.attr("disabled", "disabled");
    },
    validate: function () {
        if (this.getValue().length) {
            this.errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        } else {
            this.errorId.text("Transaction No. field is required.");
            this.Id.addClass("input-validation-error");
            return false;
        }
    }
};

var counterParty = {
    Id: $('#ddl_counter_party'),
    errorId: $('#counter_party_error'),
    init: function () {
        this.bindEventHandler();
    },
    bindEventHandler: function () {
        var txt_search = $('#txt_counter_party');
        this.Id.click(function () {
            var data = { counter_party: txt_search.val() };
            GM.Utility.DDLAutoComplete(txt_search, data, null);
            txt_search.val("");
        });

        txt_search.keyup(function () {
            var data = { counter_party: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        });
    },
    clearValue: function () {
        this.Id.find(".selected-data").text("Select...");
        this.Id.val(null);
        $('#counter_party_id').val(null);
    },
    getValue: function () {
        return $.trim($('#counter_party_id').val());
    },
    enabled: function () {
        this.Id.removeAttr('disabled');
    },
    disabled: function () {
        this.Id.attr("disabled", "disabled");
    },
    validate: function () {
        if (this.getValue().length) {
            this.errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        } else {
            this.errorId.text("Counter Party field is required.");
            this.Id.addClass("input-validation-error");
            return false;
        }
    }
};

var adjustType = {
    Id: $('input:radio[id="adjust_type"]'),
    errorId: $('#adjust_type_error'),
    init: function () {
        this.bindEventHandler();
    },
    bindEventHandler: function () {
        this.Id.change(function () {
            transNo.clearValue();
            counterParty.clearValue();

            transNo.errorId.text("");
            transNo.Id.removeClass("input-validation-error");

            counterParty.errorId.text("");
            counterParty.Id.removeClass("input-validation-error");

            if (adjustType.getValue() == 'TRANS') {
                transNo.enabled();
                counterParty.disabled();
            }
            else if (adjustType.getValue() == 'CPTY') {
                transNo.disabled();
                counterParty.enabled();
            }
        });
    },
    clearValue: function () {
        this.Id.filter('[value="TRANS"]').prop("checked", true).attr("checked", "checked");
        this.Id.filter('[value="CPTY"]').prop("checked", false).removeAttr("checked");
    },
    enabled: function () {
        this.Id.removeAttr('disabled');
    },
    disabled: function () {
        this.Id.attr('disabled', 'disabled');
    },
    setValue: function (value) {
        if (value == 'TRANS') {
            this.Id.filter('[value="TRANS"]').prop("checked", true).attr("checked", "checked");
            this.Id.filter('[value="CPTY"]').prop("checked", false).removeAttr("checked");
        } else {
            this.Id.filter('[value="CPTY"]').prop("checked", true).attr("checked", "checked");
            this.Id.filter('[value="TRANS"]').prop("checked", false).removeAttr("checked");
        }
    },
    getValue: function () {
        return !this.Id.filter(':checked').length ? this.Id.filter(':checked') : this.Id.filter(':checked').val();
    },
    validate: function () {

        this.errorId.text("");
        this.Id.removeClass("input-validation-error");

        transNo.errorId.text("");
        transNo.Id.removeClass("input-validation-error");

        counterParty.errorId.text("");
        counterParty.Id.removeClass("input-validation-error");

        if (this.getValue() == 'CPTY') {
            if (!counterParty.validate()) {
                return false;
            } else {
                return true;
            }
        }
        else if (this.getValue() == 'TRANS') {
            if (!transNo.validate()) {
                return false;
            } else {
                return true;
            }
        }
        else {
            this.errorId.text("Adjust Type field is required.");
            this.Id.addClass("input-validation-error");
            return false;
        }
    }
};

var cur = {
    Id: $('#ddl_cur'),
    errorId: $('#cur_error'),
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
    },
    getValue: function () {
        return $.trim($('#cur').val());
    },
    disabled: function () {
        this.Id.attr('disabled', 'disabled');
    },
    validate: function () {
        if (this.getValue().length) {
            this.errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        } else {
            this.errorId.text("Adjust Currency field is required.");
            this.Id.addClass("input-validation-error");
            return false;
        }
    }
};

var transPort = {
    Id: $('#ddl_trans_port'),
    errorId: $('#trans_port_error'),
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
    },
    getValue: function () {
        return $.trim($('#trans_port').val());
    },
    disabled: function () {
        this.Id.attr('disabled', 'disabled');
    },
    validate: function () {
        if (this.getValue().length) {
            this.errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        } else {
            this.errorId.text("Port field is required.");
            this.Id.addClass("input-validation-error");
            return false;
        }
    }
};

var transDate = {
    Id: $('#trans_date'),
    init: function () {
        this.setValue(b_date);
        this.bindEventHandler();
    },
    bindEventHandler: function () {
        this.Id.on('dp.change', function (e) {
            transNo.clearValue();
            counterParty.clearValue();
        });
    },
    getValue: function () {
        return $.trim(this.Id.val());
    },
    setValue: function (value) {
        this.Id.data("DateTimePicker").date(value);
    },
    disabled: function () {
        this.Id.attr('disabled', 'disabled');
    }
};

var postingDate = {
    Id: $('#posting_date'),
    init: function () {
        this.setValue(b_date);
        this.bindEventHandler();
    },
    bindEventHandler: function() {
    },
    getValue: function () {
        return $.trim(this.Id.val());
    },
    setValue: function (value) {
        this.Id.data("DateTimePicker").date(value);
    },
    disabled: function () {
        this.Id.attr('disabled', 'disabled');
    }
};

var valueDate = {
    Id: $('#value_date'),
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
    }
};

//#region Modal Slide
var amount = {
    Id: $('#amount'),
    errorId: $("#amount_error"),
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
            this.errorId.text("Amount field is required.");
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

var note = {
    Id: $('#note'),
    errorId: $("#note_error"),
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
            this.errorId.text("Note field is required.");
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

var accountNo = {
    Id: $('#ddl_account_no'),
    errorId: $("#account_no_error"),
    init: function () {
        this.bindEventHandler();
    },
    bindEventHandler: function () {
        var txt_search = $('#txt_account_no');
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
        this.Id.find("span[name='account_name']").text(text);
    },
    setValue: function (value) {
        $('#account_no').val(value);
    },
    getText: function() {
        return this.Id.find("span[name='account_name']").text();
    },
    getValue: function () {
        return $.trim($('#account_no').val());
    },
    validate: function () {
        if (!this.getValue().length) {
            this.errorId.text("Account No field is required.");
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

var drCr = {
    Id: $('#ddl_dr_cr'),
    errorId: $("#dr_cr_error"),
    init: function () {
        this.bindEventHandler();
    },
    bindEventHandler: function () {
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
        this.Id.find("span[name='dr_cr']").text(text);
    },
    setValue: function (value) {
        $('#dr_cr').val(value);
    },
    getValue: function () {
        return $.trim($('#dr_cr').val());
    },
    validate: function () {
        if (this.getValue().length) {
            this.errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        } else {
            this.errorId.text("DR/CR field is required.");
            this.Id.addClass("input-validation-error");
            return false;
        }
    }
};

var costCenter = {
    Id: $('#ddl_cost_center'),
    errorId: $("#cost_center_error"),
    init: function () {
        this.bindEventHandler();
    },
    bindEventHandler: function () {
        var txt_search = $('#txt_cost_center');
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
        this.Id.find("span[name='cost_center']").text(text);
    },
    setValue: function (value) {
        $('#cost_center').val(value);
    },
    getValue: function () {
        return $.trim($('#cost_center').val());
    },
    validate: function () {
        if (this.getValue().length) {
            this.errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        } else {
            this.errorId.text("Cost Center field is required.");
            this.Id.addClass("input-validation-error");
            return false;
        }
    }
};

var rowId = {
    Id: $('#rowId'),
    getValue: function() {
        return this.Id.val();
    },
    setValue: function(value) {
        this.Id.val(value);
    },
    clearValue: function () {
        this.setValue(null);
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

            var accountText = accountNo.getText().split(/:(.+)/);
            var accountName = accountText.length >= 2 ? accountText[1] : '';
            
            var data = {
                account_no: accountNo.getValue(),
                account_name: accountName,
                dr_cr: drCr.getValue(),
                amount: amount.getValue(),
                cost_center: costCenter.getValue(),
                note: note.getValue()
            };

            if (text == 'Add') {
                tableResult.addRow(data);
                modalSlide.close();
            }
            else if (text == 'Edit') {
                tableResult.updateRow(rowId.getValue(), data);
                modalSlide.close();
            }
        });
        btnSave.setText('Save');
    },
    setText: function(text) {
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

var btnAdd = {
    Id: $('#btnAdd'),
    init: function() {
        this.Id.on("click", function() {
            modalSlide.initAdd();
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
    initAdd: function() {
        this.setTitle('Add Record');
        this.clearForm();
        btnSave.init('Add', _urlCreate);

        accountNo.init();
        costCenter.init();
        drCr.init();
        this.open();
    },
    initEdit: function(obj) {
        this.setTitle('Edit Record');
        this.clearForm();
        btnSave.init('Edit', _urlEdit);
        this.setForm(obj);

        accountNo.init();
        costCenter.init();
        drCr.init();

        this.open();
    },
    initView: function(id) {

    },
    initDelete: function(id) {

    },
    setTitle: function (text) {
        this.Id.find('#modalTitle').text(text);
    },
    setForm: function(obj) {
        var row = tableResult.getRow(obj);
        if (row.length) {
            var data = row.data();
            accountNo.setValue(data.account_no);
            accountNo.setText(data.account_no + ':' + data.account_name);
            drCr.setValue(data.dr_cr);
            drCr.setText(data.dr_cr);
            costCenter.setValue(data.cost_center);
            costCenter.setText(data.cost_center);
            amount.setValue(formatDecimal(data.amount,2));
            note.setValue(data.note);
            rowId.setValue(row.index());
        }
    },
    open: function () {
        this.Id.modal('show');
    },
    close: function () {
        this.Id.modal('hide');
    },
    clearForm: function () {
        btnSave.Id.unbind("click");
        accountNo.clearValue();
        drCr.clearValue();
        amount.clearValue();
        costCenter.clearValue();
        note.clearValue();
        rowId.clearValue();
    },
    validateForm: function() {
        var isValidate = true;
        if (!note.validate()) {
            isValidate = false;
        }

        if (!costCenter.validate()) {
            isValidate = false;
        }

        if (!amount.validate()) {
            isValidate = false;
        }

        if (!drCr.validate()) {
            isValidate = false;
        }

        if (!accountNo.validate()) {
            isValidate = false;
        }

        return isValidate;
    }
};

//#endregion Modal Slide

var validateSubmit = function() {
    var isValidate = true;
    if (!cur.validate()) {
        isValidate = false;
    }

    if (!adjustType.validate()) {
        isValidate = false;
    }

    if (!transPort.validate()) {
        isValidate = false;
    }

    return isValidate;
};

var GlAdjustForm = (function ($) {
    var formId = $('#action-form');
    var initForm = function(action) {
        formId.on('submit', function(e) {
            e.preventDefault();

            var dataRecord = tableResult.getData();
            if (action == 'Add') {
                if (!validateSubmit()) {
                    return;
                }

                //validate record
                if (!dataRecord.length) {
                    swal("Warning!", "Record not found.", "warning");
                    return;
                }
            }
            else if (action == 'Edit') {
                //check record
                if (!dataRecord.length) {
                    swal({
                        title: "No record data, Do you want to delete Adjust No "+ adjustNum.getValue() + "?",
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
                                    adjust_num: adjustNum.getValue()
                                },
                                success: function (d) {
                                    $('.spinner').css('display', 'none');
                                    if (d.Success) {
                                        window.location.href = _urlIndex;
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
                    return;
                }
            }

            //validate balance
            var sumDr = 0;
            var sumCr = 0;
            var row = 1;
            var items = [];
            $.each(dataRecord,
                function(i, v) {

                    items.push($.extend(true,
                        v,
                        {
                            adjust_num: adjustNum.getValue(),
                            posting_date: postingDate.getValue(),
                            value_date: valueDate.getValue(),
                            adjust_type: adjustType.getValue(),
                            trans_no: transNo.getValue(),
                            counter_party_id: counterParty.getValue(),
                            cur: cur.getValue(),
                            trans_port: transPort.getValue(),
                            sub_seq_num: row
                        }
                    ));

                    if (v.dr_cr == 'DR') {
                        sumDr += parseFloat(v.amount.toString().replace(/,/g, ''));
                    }
                    if (v.dr_cr == 'CR') {
                        sumCr += parseFloat(v.amount.toString().replace(/,/g, ''));
                    }

                    row++;
                });

            if (sumDr != sumCr) {
                swal("Warning!",
                    "DR(" +
                    formatDecimal(sumDr, 2) +
                    ") and CR(" +
                    formatDecimal(sumCr, 2) +
                    ") are not balance, please adjust again.",
                    "warning");
                return false;
            }

            //process
            $('.spinner').css('display', 'block');
            var urlAction = $(this).attr('action');
            var dataToPost = {
                items
            };

            $.ajax({
                type: "POST",
                url: urlAction,
                content: "application/json; charset=utf-8",
                dataType: "json",
                data: dataToPost,
                success: function (d) {
                    $('.spinner').css('display', 'none');
                    if (d.Success) {
                        window.location.href = _urlIndex;
                    } else {
                        swal("Failed!", "Error : " + d.Message, "error");
                    }
                },
                error: function (d) {
                    $('.spinner').css('display', 'none');
                }
            });
        });
    };

    return {
        numberOnlyAndDot: function (obj) {
            obj.value = obj.value
                .replace(/[^\d.]/g, '')             // numbers and decimals only
                .replace(/(^[\d]{18})[\d]/g, '$1')   // not more than 18 digits at the beginning
                .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
                .replace(/(\.[\d]{2})./g, '$1');    // not more than 2 digits after decimal
        },
        auto2digit: function (obj) {
            if (obj.value.length) {
                return obj.value = formatDecimal(obj.value, 2);
            }
            return "";
        },
        initAdd: function (settings) {
            _urlIndex = settings.urlIndex;
            _isUpdate = settings.isUpdate;
            _isDelete = settings.isDelete;
            
            cur.init();
            transPort.init();
            transDate.init();
            postingDate.init();
            valueDate.init();
            transNo.init();
            counterParty.init();
            adjustType.init();
            transNo.disabled();
            counterParty.disabled();
            postingDate.disabled();
            tableResult.init();

            btnAdd.init();
            initForm('Add');
        },
        initEdit: function (settings) {
            _urlIndex = settings.urlIndex;
            _urlDelete = settings.urlDelete;
            _urlGetDetail = settings.urlGetDetail;
            _isUpdate = settings.isUpdate;
            _isDelete = settings.isDelete;

            adjustNum.enabled();
            transPort.disabled();
            transDate.disabled();
            postingDate.disabled();
            valueDate.disabled();
            cur.disabled();
            adjustType.disabled();
            transNo.disabled();
            counterParty.disabled();

            tableResult.init();
            tableResult.Id.DataTable().ajax.url( _urlGetDetail + '?adjust_num=' + adjustNum.getValue()).load();

            btnAdd.init();

            if (_isUpdate == 'False') {
                btnAdd.disabled();
            }

            initForm('Edit');
        },
        modalEdit: function (obj) {
            modalSlide.initEdit(obj);
        },
        modalView: function (obj) {
            modalSlide.initView(obj);
        },
        removeRow: function (obj) {

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
                    tableResult.removeRow(obj);
                }
            });
        }
    };
})(jQuery);