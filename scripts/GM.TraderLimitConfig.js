$(document).ready(function () {

    $("#NavBar").html($('#NavTraderLimitConfig').val());

    GM.TraderLimitConfig = {};
    GM.TraderLimitConfig.Table = $('#x-table-data').DataTable({
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
            "url": "/TraderLimitConfig/Search",
            "type": "POST",
            "error": function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR);
                console.log(textStatus);
                console.log(errorThrown);
            }
        },
        columnDefs:
            [
                { targets: 0, data: "RowNumber", className: 'dt-body-center', orderable: false },
                { targets: 1, data: "user_id", className: 'dt-body-center', },
                { targets: 2, data: "user_eng_name" },
                { targets: 3, data: "title_master_name_eng" },
                {
                    targets: 4, data: "govt_short_limit",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = Number("0").toFixed(2);
                        }
                        return html;
                    }
                },
                {
                    targets: 5, data: "govt_long_limit",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = Number("0").toFixed(2);
                        }
                        return html;
                    }
                },
                {
                    targets: 6, data: "corp_limit",
                    className: 'dt-body-right',
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            html = FormatDecimal(data, 2);
                        } else {
                            html = Number("0").toFixed(2);
                        }
                        return html;
                    }
                },
                {
                    targets: 7, data: "effective_date",
                    className: 'dt-body-center',
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                {
                    targets: 8, data: "active_flag",
                    className: 'dt-body-center',
                    render: function (data, type, row, meta) {
                        if (data == true) {
                            return 'Yes';
                        }
                        else {
                            return 'No';
                        }
                    }
                },
                {
                    targets: 9, orderable: false, data: "limit_id", className: "dt-body-center", width: 60, render: function (data, type, row, meta) {
                        var html = '';

                        if (isUpdate == "True") {
                            html += '<button class="btn btn-default btn-round" key="' + row.limit_id + '" keysecond="' + moment(row.effective_date).format('DD/MM/YYYY') + '" active_flag="' + row.active_flag + '" form-mode="edit" onclick="GM.TraderLimitConfig.Form(this)"><i class="feather-icon icon-edit"></i></button>';
                        }
                        else {
                            html += '<button class="btn btn-default btn-round" key="' + row.limit_id + '" keysecond="' + moment(row.effective_date).format('DD/MM/YYYY') + '" active_flag="' + row.active_flag + '" form-mode="view" onclick="GM.TraderLimitConfig.Form(this)"><i class="feather-icon icon-edit"></i></button>';
                        }

                        if (isDelete == "True") {
                            //html += '<button class="btn btn-delete  btn-round" key="' + row.limit_id + '" keysecond="' + moment(row.effective_date).format('DD/MM/YYYY') + '" active_flag="' + row.active_flag + '" form-mode="delete" onclick="GM.TraderLimitConfig.Form(this)"><i class="feather-icon icon-trash-2"></i></button>';
                        }
                        else {
                            //html += '<button class="btn btn-delete  btn-round" key="' + row.limit_id + '" keysecond="' + moment(row.effective_date).format('DD/MM/YYYY') + '" active_flag="' + row.active_flag + '" form-mode="delete" onclick="GM.TraderLimitConfig.Form(this)" disabled><i class="feather-icon icon-trash-2"></i></button>';
                        }

                        return html;
                    }
                },
                { targets: 10, data: "desk_group_id", visible: false },
                { targets: 11, data: "title_master_id", visible: false },
            ],
        fixedColumns: {
            leftColumns: 1,
            rightColumns: 3
        }

    });

    GM.TraderLimitConfig.Form = function (btn) {
        var mode = $(btn).attr("form-mode");
        GM.Message.Clear();
        GM.TraderLimitConfig.Form.Initial();
        var radioyes = $("[id=FormAction_active_flag][value=true]");
        var radiono = $("[id=FormAction_active_flag][value=false]");

        if (mode) {
            var key = $(btn).attr("key");
            var keysecond = $(btn).attr("keysecond");
            var active_flag = $(btn).attr("active_flag");
        
            switch (mode) {
                case "add":
                    // Step 1 : Set title
                    $(".modal-title").text("Trader Limit");
                    // Step 2 : Set Button
                    $("#action-form :submit").removeClass('btn-delete').addClass('btn-primary').text('Create');
                    $("#action-form :submit").removeAttr("onclick");
                    // Step 3 : Set Action
                    $("#action-form").attr("action", "/TraderLimitConfig/Create");
                    // Step 4 : Set Input
                    $("#ddl_user_action").removeAttr('disabled', 'disabled');
                    $("#ddl_user_action").find(".selected-data").text("Select...");
                    $("#ddl_user_action").find(".selected-value").text("");
                    $("#Txt_title_master_name").removeAttr('disabled', 'disabled');
                    $("#Txt_title_master_name").val("");
                    $("#FormAction_govt_short_limit").removeAttr('disabled', 'disabled');
                    $("#FormAction_govt_short_limit").val("");
                    $("#FormAction_govt_long_limit").removeAttr('disabled', 'disabled');
                    $("#FormAction_govt_long_limit").val("");
                    $("#FormAction_corp_limit").removeAttr('disabled', 'disabled');
                    $("#FormAction_corp_limit").val("");
                    $("#FormAction_effective_date").removeAttr('disabled', 'disabled');
                    $("#FormAction_effective_date").val("");
                    radioyes.removeAttr('disabled', 'disabled');
                    radiono.removeAttr('disabled', 'disabled');
                    radioyes.attr('ischeck', 'true');
                    radioyes.attr('checked', 'checked');
                    radioyes.prop('checked', true);
                    radiono.removeAttr('checked');
                    radiono.attr('ischeck', 'false');
                    // Step 5 : Show modal
                    $('#action-form-modal').modal('show');

                    break;
                case "edit":
                    // Step 0 : Select Data
                    GM.TraderLimitConfig.Get({
                        id: key,
                        id1: keysecond,
                        active_flag: active_flag,
                        form: 'action-form',
                        handler: function (response, status, jqxhr) {
                        }
                    });
                    // Step 1 : Set title
                    $(".modal-title").text("Trader Limit");
                    // Step 2 : Set Button
                    $("#action-form :submit").removeAttr("onclick");
                    $("#action-form :submit").removeClass('btn-delete').addClass('btn-primary').text('Update');
                    // Step 3 : Set Action
                    $("#action-form").attr("action", "/TraderLimitConfig/Edit");
                    // Step 4 : Set Input

                    $("#FormAction_govt_short_limit").removeAttr('disabled', 'disabled');
                    $("#FormAction_govt_long_limit").removeAttr('disabled', 'disabled');
                    $("#FormAction_corp_limit").removeAttr('disabled', 'disabled');
                    $("#FormAction_effective_date").removeAttr('disabled', 'disabled');
                    radioyes.removeAttr('disabled', 'disabled');
                    radiono.removeAttr('disabled', 'disable');

                    $("#Txt_title_master_name").attr('disabled', 'disabled');
                    $("#ddl_user_action").attr('disabled', 'disabled');

                    // Step 5 : Show modal
                    $('#action-form-modal').modal('show');

                    break;
                case "delete":
                    // Step 0 : Select Data
                    GM.TraderLimitConfig.Get({
                        id: key,
                        id1: keysecond,
                        active_flag: active_flag,
                        form: 'action-form',
                        handler: function (response, status, jqxhr) {
                        }
                    });
                    // Step 1 : Set title
                    $(".modal-title").text("Trader Limit");
                    // Step 2 : Set Button
                    $("#action-form :submit").removeClass('btn-primary').addClass('btn-delete').text('Delete');
                    $("#action-form :submit").attr("onclick", "Delete('" + key + "', '" + keysecond + "')");
                    $("#action-form").attr("action", "notpost");
                    // Step 3 : Set Input
                    $("#ddl_user_action").attr('disabled', 'disabled');
                    $("#Txt_title_master_name").attr('disabled', 'disabled');
                    $("#FormAction_govt_short_limit").attr('disabled', 'disabled');
                    $("#FormAction_govt_long_limit").attr('disabled', 'disabled');
                    $("#FormAction_corp_limit").attr('disabled', 'disabled');
                    $("#FormAction_effective_date").attr('disabled', 'disabled');
                    radioyes.attr('disabled', 'disabled');
                    radiono.attr('disabled', 'disable');
                    // Step 4 : Show modal
                    $('#action-form-modal').modal('show');

                    break;
                case "view":
                    // Step 0 : Select Data
                    GM.TraderLimitConfig.Get({
                        id: key,
                        id1: keysecond,
                        active_flag: active_flag,
                        form: 'action-form',
                        handler: function (response, status, jqxhr) {
                        }
                    });
                    // Step 1 : Set title
                    $(".modal-title").text("Trader Limit");
                    // Step 2 : Set Button
                    $("#action-form :submit").removeClass('btn-delete').addClass('btn-primary').text('Update');
                    $("#action-form :submit").attr('disabled', "disabled");
                    // Step 3 : Set Input
                    $("#ddl_user_action").attr('disabled', 'disabled');
                    $("#Txt_title_master_name").attr('disabled', 'disabled');
                    $("#FormAction_govt_short_limit").attr('disabled', 'disabled');
                    $("#FormAction_govt_long_limit").attr('disabled', 'disabled');
                    $("#FormAction_corp_limit").attr('disabled', 'disabled');
                    $("#FormAction_effective_date").attr('disabled', 'disabled');
                    radioyes.attr('disabled', 'disabled');
                    radiono.attr('disabled', 'disable');

                    // Step 4 : Show modal
                    $('#action-form-modal').modal('show');

                    break;
            }
        }
    };

    GM.TraderLimitConfig.Get = function (op) {
        $.get("/TraderLimitConfig/Edit", { id: op.id, id1: op.id1, active_flag: op.active_flag, t: GM.Time })
            .done(function (response, status, jqxhr) {
                // this is the "success" callback
                GM.TraderLimitConfig.Form.DataBinding({ form: op.form, data: response });
                op.handler(response);
            })
            .fail(function (jqxhr, status, error) {
            });
    };

    $('.radio input[id=FormSearch_active_flag]').change(function () {
        var current = $(this).val();
        var radioyes = $("[id=FormSearch_active_flag][value=true]");
        var radiono = $("[id=FormSearch_active_flag][value=false]");
        if (current == "true") {
            radioyes.attr('ischeck', 'true');
            radiono.attr('ischeck', 'false');
            radioyes.attr("checked", "checked");
            radiono.removeAttr("checked");
        }
        else {
            radioyes.attr('ischeck', 'false');
            radiono.attr('ischeck', 'true');
            radiono.attr("checked", "checked");
            radioyes.removeAttr("checked");
        }
    });

    $('.radio input[id=FormAction_active_flag]').change(function () {
        var current = $(this).val();
        var radioyes = $("[id=FormAction_active_flag][value=true]");
        var radiono = $("[id=FormAction_active_flag][value=false]");
        if (current == "true") {
            radioyes.attr('ischeck', 'true');
            radiono.attr('ischeck', 'false');
            radioyes.attr("checked", "checked");
            radiono.removeAttr("checked");
        }
        else {
            radioyes.attr('ischeck', 'false');
            radiono.attr('ischeck', 'true');
            radiono.attr("checked", "checked");
            radioyes.removeAttr("checked");
        }
    });

    $("#ddl_user").click(function () {
        var txt_search = $('#txt_user');
        var data = { user_id: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_user').keyup(function () {
        console.log(this.value);
        var data = { user_id: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    }

    GM.TraderLimitConfig.Form.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            switch (key) {
                case "user_id": GM.TraderLimitConfig.Table.columns(1).search($(this).val()); break;
                case "active_flag":
                    if ($(this).attr("ischeck") == "true") {
                        GM.TraderLimitConfig.Table.columns(8).search($(this).val());
                    }
                    break;
            }
        });
        GM.TraderLimitConfig.Table.draw();
    };

    GM.TraderLimitConfig.Form.DataBinding = function (p) {
        $('#' + p.form + ' :input').each(function () {
            var input = $(this);
            var inputid = input[0].id;
            var key = input[0].name.split('.')[1];
            var inputtype = input.attr("type");
            var inputvalue = input.attr("value");
             var typeoftextfilesd = input.attr("typefield");
            if (p.data[key] != null) {
                if (inputtype == "radio") {
                    var inputyes = $("[id=" + inputid + "][value=true]");
                    var inputno = $("[id=" + inputid + "][value=false]");
                    if (p.data[key]) {
                        inputno.removeAttr('checked');
                        inputno.attr('ischeck', 'false');
                        inputyes.attr('ischeck', 'true');
                        inputyes.attr('checked', 'checked');
                        inputyes.prop('checked', true);
                    }
                    else {
                        inputyes.removeAttr('checked');
                        inputyes.attr('ischeck', 'false');
                        inputno.attr('ischeck', 'true');
                        inputno.attr('checked', 'checked');
                        inputno.prop('checked', true);
                    }
                }
                else if (typeoftextfilesd == "date") {
                    var data = moment(p.data[key]).format('DD/MM/YYYY');
                    $(this).val(data);
                }
                else if (key == 'govt_short_limit' || key == 'govt_long_limit' || key == 'corp_limit') {
                    $(this).val(FormatDecimal(p.data[key], 2));
                }
                else {
                    $(this).val(p.data[key] + '');
                }
            }
            console.log('(' + key + ') => ' + p.data[key]);
        });

        $('#' + p.form + ' span').each(function () {
            var input = $(this);
            var key = input.attr("name");
            if (typeof key != "undefined") {
                if (p.data[key] != "" && p.data[key] != null) {
                    $(this)[0].innerHTML = p.data[key];
                }
            }
            console.log('(' + key + ') => ' + p.data[key]);
        });
    };

    $("#search-form").on('submit', function (e) {
        e.preventDefault();
        GM.Message.Clear();
        GM.TraderLimitConfig.Form.Search();
    });

    $("#search-form").on('reset', function (e) {

        var radioyes = $("[id=FormSearch_active_flag][value=true]");
        var radiono = $("[id=FormSearch_active_flag][value=false]");
        radioyes.attr('ischeck', 'true');
        radioyes.prop('checked', true);
        radioyes.attr('checked', 'checked');
        radiono.removeAttr('checked');
        radiono.attr('ischeck', 'false');

        $("#ddl_user").find(".selected-data").text("Select...");
        $("#ddl_user").find(".selected-value").text("");

        GM.Defer(function () {
            GM.Message.Clear();
            GM.TraderLimitConfig.Form.Search();
        },
            100);
    });

    $("#btnAdd").on("click", function () {
        GM.TraderLimitConfig.Form(this);
    });

    $("#ddl_user_action").click(function () {
        var txt_search = $('#txt_user_action');
        var data = { user_id: null };
        GM.Utility.DDLAutoCompleteSet4Value(txt_search, data, "Hid_title_master_id", "Txt_title_master_name", null);
        txt_search.val("");
    });

    $('#txt_user_action').keyup(function () {
        var data = { user_id: this.value };
        GM.Utility.DDLAutoCompleteSet4Value(this, data, "Hid_title_master_id", "Txt_title_master_name", null);
    });

    GM.TraderLimitConfig.Form.Initial = function () {
        $("#action-form")[0].reset();

        var ddl_user_action = $("#ddl_user_action").find(".selected-value");
        var FormAction_govt_short_limit = $("#FormAction_govt_short_limit");
        var FormAction_govt_long_limit = $("#FormAction_govt_long_limit");
        var FormAction_corp_limit = $("#FormAction_corp_limit");
        var FormAction_effective_date = $("#FormAction_effective_date");

        $("#user_id_error").text("");
        ddl_user_action.removeClass("input-validation-error");
        $("#govt_short_limit_error").text("");
        FormAction_govt_short_limit.removeClass("input-validation-error");
        $("#govt_long_limit_error").text("");
        FormAction_govt_long_limit.removeClass("input-validation-error");
        $("#corp_limit_error").text("");
        FormAction_corp_limit.removeClass("input-validation-error");
        $("#effective_date_error").text("");
        FormAction_effective_date.removeClass("input-validation-error");
    };

    $("#action-form").on('submit', function (e) {

        GM.Message.Clear();
        e.preventDefault();

        var isValid = true;

        var ddl_user_action = $("#ddl_user_action").find(".selected-value");
        var FormAction_govt_short_limit = $("#FormAction_govt_short_limit");
        var FormAction_govt_long_limit = $("#FormAction_govt_long_limit");
        var FormAction_corp_limit = $("#FormAction_corp_limit");
        var FormAction_effective_date = $("#FormAction_effective_date");

        if (ddl_user_action.val().trim() == "") {
            $("#user_id_error").text("The User Id field is required.");
            ddl_user_action.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_govt_short_limit.val().trim() == "") {
            $("#govt_short_limit_error").text("The Govt Short Limit field is required.");
            FormAction_govt_short_limit.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_govt_long_limit.val().trim() == "") {
            $("#govt_long_limit_error").text("The Govt Long Limit field is required.");
            FormAction_govt_long_limit.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_corp_limit.val().trim() == "") {
            $("#corp_limit_error").text("The Corp Limit field is required.");
            FormAction_corp_limit.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_effective_date.val().trim() == "") {
            $("#effective_date_error").text("The Effective Date field is required.");
            FormAction_effective_date.addClass("input-validation-error");
            isValid = false;
        }

        if (isValid) {
            var dataToPost = $(this).serialize();
            var action = $(this).attr('action');
            if (action != "notpost") {
                GM.Mask('#action-form-modal .modal-content');
                $.post(action, dataToPost)

                    .done(function (response, status, jqxhr) {
                        GM.Unmask();
                        if (response.Success) {
                            GM.Message.Success('.modal-body', response.Message);
                            GM.Defer(function () {
                                $('#action-form-modal').modal('hide');
                                GM.TraderLimitConfig.Table.draw();
                                GM.TraderLimitConfig.Form.Search();
                            }, 500);
                        }
                        else {
                            GM.Message.Error('.modal-body', response.Message);
                        }
                    })

                    .fail(function (jqxhr, status, error) {
                    });
            }
        }
    });
});


function Delete(limit_id, effective_date) {

    swal({
        title: "Are you sure?",
        text: "You will not be able to recover this imaginary file!",
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
                var data = { limit_id: limit_id, effective_date: effective_date };
                console.log("Show form delete -> local");
                $.ajax({
                    type: "POST",
                    url: "/TraderLimitConfig/Delete",
                    content: "application/json; charset=utf-8",
                    dataType: "json",
                    data: data,
                    success: function (d) {
                        $('.spinner').css('display', 'none');
                        if (d.success) {
                            setTimeout(function () {
                                swal("Deleted!", "Delete Success.", "success");
                            }, 100);
                            $('#action-form-modal').modal('hide');
                            GM.Message.Clear();
                            GM.TraderLimitConfig.Form.Search();
                            GM.TraderLimitConfig.Table.draw();
                        } else {
                            // DoSomethingElse()
                            swal("Deleted!", "Error : " + d.responseText, "error");
                        }
                    },
                    error: function (d) {
                        $('.spinner').css('display', 'none');
                        // TODO: Show error
                        GM.Message.Clear();
                    }
                });
            } else {
                GM.Message.Clear();
                swal("Cancelled", "Your imaginary file is safe :)", "error");
            }
        });
}

function FormatDecimal(Num, point) {
    var format = Number(parseFloat(Num).toFixed(point)).toLocaleString('en', {
        minimumFractionDigits: point
    });
    return format;
}

function numberOnlyAnd5_2Dot(obj) {
    obj.value = obj.value
        .replace(/[^\d.]/g, '')             // numbers and decimals only
        .replace(/(^[\d]{5})[\d]/g, '$1')   // not more than 5 digits at the beginning
        .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
        .replace(/(\.[\d]{2})./g, '$1');    // not more than 2 digits after decimal
}

function numberOnlyAnd18_2Dot(obj) {
    obj.value = obj.value
        .replace(/[^\d.]/g, '')             // numbers and decimals only
        .replace(/(^[\d]{18})[\d]/g, '$1')   // not more than 18 digits at the beginning
        .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
        .replace(/(\.[\d]{2})./g, '$1');    // not more than 2 digits after decimal
}

function auto2digit(obj) {
    if (obj.value.length) {
        var nStr = obj.value;
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
        return obj.value = x1 + '.' + x2;
    }
    return obj.value = '0.00';
}