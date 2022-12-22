$.ajaxSetup({
    cache: false
});

function numberOnlyAndDot(obj) {
    obj.value = obj.value
        .replace(/[^\d.]/g, '')             // numbers and decimals only
        .replace(/(^[\d]{3})[\d]/g, '$1')   // not more than 3 digits at the beginning
        .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
        .replace(/(\.[\d]{8})./g, '$1');    // not more than 6 digits after decimal
}
function auto8digit(obj) {
    if (obj.selectionStart == 1 && obj.value == ".") {
        obj.value = 0.0
        const f = Math.pow(10, 8);
        return obj.value = (Math.floor(obj.value * f) / f).toFixed(8);
    }
    else {
        const f = Math.pow(10, 8);
        return obj.value = (Math.floor(obj.value * f) / f).toFixed(8);
    }
    return "";
}

function auto8digitval(values) {
    if (values != "") {
        const f = Math.pow(10, 8);
        return values = (Math.floor(values * f) / f).toFixed(8);
    }
    return "";
}

function ClearError()
{
    var FormAction_asof_date = $("#FormAction_asof_date");
    var FormAction_curve_id = $("#FormAction_curve_id");
    var ddl_cur_action = $("#ddl_cur_action").find(".selected-value");
    var FormAction_index_type = $("#FormAction_index_type");

    $("#asof_date_error").text("");
    FormAction_asof_date.removeClass("input-validation-error");

    $("#curve_id_error").text("");
    FormAction_curve_id.removeClass("input-validation-error");

    ddl_cur_action.removeClass("input-validation-error");
    $("#cur_error").text("");

    $("#index_type_error").text("");
    FormAction_index_type.removeClass("input-validation-error");
}

$(document).ready(function () {

    $("#NavBar").html($('#NavThorRate').val());

    $("#ddl_cur").click(function () {
        var txt_search = $('#txt_cur');
        var data = { cur: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_cur').keyup(function () {
        var data = { cur: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    }

    $("#ddl_cur").find(".selected-data").text("THB");
    $("#ddl_cur").find(".selected-value").text("THB");
    $('#FormSearch_ccy').val("THB")

    var momentDate = moment($("#BusinessDate").text(), "DD/MM/YYYY");

    $('#FormSearch_asof_date_from').data("DateTimePicker").date(momentDate);
    $('#FormSearch_asof_date_from').val(momentDate.format("DD/MM/YYYY"));

    $('#FormSearch_asof_date_to').data("DateTimePicker").date(momentDate);
    $('#FormSearch_asof_date_to').val(momentDate.format("DD/MM/YYYY"));

    GM.ThorRate = {};
    GM.ThorRate.Table = $('#x-table-data').DataTable({
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
            "url": "/ThorRate/Search",
            "type": "POST",
            "error": function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR);
                console.log(textStatus);
                console.log(errorThrown);
            }
        },
        columnDefs:
            [
                { targets: 0, data: "RowNumber", orderable: false},
                {
                    targets: 1, data: "asof_date",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data
                    }
                },
                { targets: 2, data: "curve_id" },
                { targets: 3, data: "ccy" },
                { targets: 4, data: "index_type" },
                { targets: 5, data: "tenor" },
                {
                    targets: 6, data: "rate",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return auto8digitval(data);
                        }
                        return data
                    }
                },
                {
                    targets: 7, data: "spread",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return FormatDecimalValidate(data,8);
                        }
                        return data
                    }
                },
                {
                    targets: 8, orderable: false, data: "asof_date", className: "dt-body-center", width: 60, render: function (data, type, row, meta) {
                        var html = '';

                        if (isUpdate == "True") {
                            html += '<button class="btn btn-default btn-round" asof_date="' + moment(row.asof_date).format('DD/MM/YYYY') + '" curve_id="' + row.curve_id + '" ccy="' + row.ccy + '" index_type="' + row.index_type + '" form-mode="edit" onclick="GM.ThorRate.Form(this)"><i class="feather-icon icon-edit"></i></button>';
                        }
                        else {
                            html += '<button class="btn btn-default btn-round" asof_date="' + moment(row.asof_date).format('DD/MM/YYYY') + '" curve_id="' + row.curve_id + '" ccy="' + row.ccy + '" index_type="' + row.index_type + '" form-mode="view" onclick="GM.ThorRate.Form(this)"><i class="feather-icon icon-edit"></i></button>';
                        }

                        if (isDelete == "True") {
                            html += '<button class="btn btn-delete  btn-round" asof_date="' + moment(row.asof_date).format('DD/MM/YYYY') + '" curve_id="' + row.curve_id + '" ccy="' + row.ccy + '" index_type="' + row.index_type + '" form-mode="delete" onclick="GM.ThorRate.Form(this)"><i class="feather-icon icon-trash-2"></i></button>';
                        }
                        else {
                            html += '<button class="btn btn-delete  btn-round" asof_date="' + moment(row.asof_date).format('DD/MM/YYYY') + '" curve_id="' + row.curve_id + '" ccy="' + row.ccy + '" index_type="' + row.index_type + '" form-mode="delete" onclick="GM.ThorRate.Form(this)" disabled><i class="feather-icon icon-trash-2"></i></button>';
                        }

                        return html;
                    }
                },
                { targets: 9, data: "asof_date_from", visible: false },
                { targets: 10, data: "asof_date_to", visible: false }
            ],
        fixedColumns: {
            leftColumns: 1,
            rightColumns: 3
        },
        fnPreDrawCallback: function () {
            $('#x-table-data').DataTable().columns(9).search($('#FormSearch_asof_date_from').val());
            $('#x-table-data').DataTable().columns(10).search($('#FormSearch_asof_date_to').val());
            $('#x-table-data').DataTable().columns(3).search($('#FormSearch_ccy').val());
        }

    });

    GM.ThorRate.Form = function (btn) {
        var mode = $(btn).attr("form-mode");
        GM.Message.Clear();
        GM.ThorRate.Form.Initial();

        if (mode) {
            var asof_date = $(btn).attr("asof_date");
            var curve_id = $(btn).attr("curve_id");
            var ccy = $(btn).attr("ccy");
            var index_type = $(btn).attr("index_type");

            switch (mode) {
                case "add":
                    // Step 1 : Set title
                    $(".modal-title").text("Create ThorRate");
                    // Step 2 : Set Button
                    $("#action-form :submit").removeClass('btn-delete').addClass('btn-primary').text('Create');
                    $("#action-form :submit").removeAttr("onclick");
                    // Step 3 : Set Action
                    $("#action-form").attr("action", "/ThorRate/Create");
                    //// Step 4 : Set Input

                    $("#FormAction_asof_date").attr('readonly', false);
                    $("#FormAction_asof_date").val("");
                    $("#FormAction_curve_id").attr('readonly', false);
                    $("#FormAction_curve_id").val("");
                    $('#ddl_cur_action').removeAttr('disabled');
                    $("#ddl_cur_action").find(".selected-data").text("Select...");
                    $("#ddl_cur_action").find(".selected-value").text("");
                    $("#FormAction_index_type").attr('readonly', false);
                    $("#FormAction_index_type").val("");
                    $("#FormAction_tenor").attr('readonly', false);
                    $("#FormAction_tenor").val("");
                    $("#FormAction_rate").attr('readonly', false);
                    $("#FormAction_rate").val("");
                    $("#FormAction_spread").attr('readonly', false);
                    $("#FormAction_spread").val("");
                    $("#action-form").attr("isdelete", "false");
                    // Step 5 : Show modal
                    $('#action-form-modal').modal('show');

                    break;
                case "edit":
                    // Step 0 : Select Data
                    GM.ThorRate.Get({
                        asof_date: asof_date,
                        curve_id: curve_id,
                        ccy: ccy,
                        index_type: index_type,
                        form: 'action-form',
                        handler: function (response, status, jqxhr) {
                        }
                    });
                    // Step 1 : Set title
                    $(".modal-title").text("Edit ThorRate");
                    // Step 2 : Set Button
                    $("#action-form :submit").removeAttr("onclick");
                    $("#action-form :submit").removeClass('btn-delete').addClass('btn-primary').text('Update');
                    // Step 3 : Set Action
                    $("#action-form").attr("action", "/ThorRate/Edit");
                    // Step 4 : Set Input
                    $("#FormAction_tenor").attr('readonly', false);
                    $("#FormAction_rate").attr('readonly', false);
                    $("#FormAction_spread").attr('readonly', false);

                    $("#FormAction_asof_date").attr('readonly', true);
                    $("#FormAction_curve_id").attr('readonly', true);
                    $('#ddl_cur_action').attr('disabled', 'disabled');
                    $("#FormAction_index_type").attr('readonly', true);
                    $("#action-form").attr("isdelete", "false");

                    // Step 5 : Show modal
                    //$('#action-form-modal').modal('show');

                    break;
                case "delete":
                    // Step 0 : Select Data
                    GM.ThorRate.Get({
                        asof_date: asof_date,
                        curve_id: curve_id,
                        ccy: ccy,
                        index_type: index_type,
                        form: 'action-form',
                        handler: function (response, status, jqxhr) {
                        }
                    });
                    // Step 1 : Set title
                    $(".modal-title").text("Are you sure you want to delete this?");
                    // Step 2 : Set Button
                    $("#action-form :submit").removeClass('btn-primary').addClass('btn-delete').text('Delete');
                    $("#action-form").attr("isdelete", "true");
                    $("#action-form").attr("action", "/ThorRate/Delete");
                    $("#action-form").attr("asof_date", asof_date);
                    $("#action-form").attr("curve_id", curve_id);
                    $("#action-form").attr("ccy", ccy);
                    $("#action-form").attr("index_type", index_type);

                    $("#FormAction_asof_date").attr('readonly', true);
                    $("#FormAction_curve_id").attr('readonly', true);
                    $('#ddl_cur_action').attr('disabled', 'disabled');
                    $("#FormAction_index_type").attr('readonly', true);
                    $("#FormAction_tenor").attr('readonly', true);
                    $("#FormAction_rate").attr('readonly', true);
                    $("#FormAction_spread").attr('readonly', true);
                    // Step 4 : Show modal
                    //$('#action-form-modal').modal('show');

                    break;
                case "view":
                    // Step 0 : Select Data
                    GM.ThorRate.Get({
                        asof_date: asof_date,
                        curve_id: curve_id,
                        ccy: ccy,
                        index_type: index_type,
                        form: 'action-form',
                        handler: function (response, status, jqxhr) {
                        }
                    });
                    // Step 1 : Set title
                    $(".modal-title").text("ThorRate");
                    // Step 2 : Set Button
                    $("#action-form :submit").removeClass('btn-delete').addClass('btn-primary').text('Update');
                    $("#action-form :submit").attr('disabled', "disabled");
                    $("#action-form").attr("isdelete", "false");
                    // Step 3 : Set Input

                    // Step 4 : Show modal

                    break;
            }
        }
    };

    GM.ThorRate.Get = function (op) {
        $('.spinner').css('display', 'block');
        $.get("/ThorRate/Edit", { asof_date: op.asof_date, curve_id: op.curve_id, ccy: op.ccy, index_type: op.index_type, t: GM.Time })
            .done(function (response, status, jqxhr) {
                // this is the "success" callback
                if (response.RowNumber == null) {
                    swal("Warning", "Record have been remove!", "warning");
                    GM.ThorRate.Form.Search();
                    op.handler(response);
                    $('.spinner').css('display', 'none');
                }
                else {
                    
                    GM.ThorRate.Form.DataBinding({ form: op.form, data: response });
                    op.handler(response);
                    $('.spinner').css('display', 'none');
                }

            })
            .fail(function (jqxhr, status, error) {
                $('.spinner').css('display', 'none');
            });
    };

    GM.ThorRate.Form.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            switch (key) {
                case "asof_date_from": GM.ThorRate.Table.columns(9).search($(this).val()); break;
                case "asof_date_to": GM.ThorRate.Table.columns(10).search($(this).val()); break;
                case "ccy": GM.ThorRate.Table.columns(3).search($(this).val()); break;
            }
        });
        GM.ThorRate.Table.draw();
    };

    GM.ThorRate.Form.DataBinding = function (p) {
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
        var rateNum = auto8digitval($("#FormAction_rate").val());
        $("#FormAction_rate").val(rateNum);

        var spreadNum = auto8digitval($("#FormAction_spread").val());
        $("#FormAction_spread").val(spreadNum);
        $('#action-form-modal').modal('show');
    };

    GM.ThorRate.Export = function (btn) {
        swal({
            title: "Do you want to Export?",
            text: "",
            html: true,
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

                    $.post('/ThorRate/ExportData', { asof_date_from: $('#FormSearch_asof_date_from').val(), asof_date_to: $('#FormSearch_asof_date_to').val(), ccy: $('#FormSearch_ccy').val() })
                        .done(function (response) {
                            if (response.errorMessage === '') {
                                window.location.href = '/ThorRate/Download?filename=' + response.fileName;
                                $('.spinner').css('display', 'none');
                                setTimeout(function () {

                                    swal({
                                        title: "Complete",
                                        text: "Gen Excel Successfully ",
                                        type: "success",
                                        showCancelButton: false,
                                        confirmButtonClass: "btn-success",
                                        confirmButtonText: "Ok",
                                        closeOnConfirm: true
                                    }
                                    );
                                }, 100);
                            } else {
                                setTimeout(function () {
                                    swal("Warning", response.errorMessage, "warning");
                                }, 100);
                                $('.spinner').css('display', 'none');
                            }
                        });
                }
            }
        )
    };

    $("#search-form").on('submit', function (e) {
        e.preventDefault();
        GM.Message.Clear();
        GM.ThorRate.Form.Search();
    });

    $("#search-form").on('reset', function (e) {

        $("#ddl_cur").find(".selected-data").text("THB");
        $("#ddl_cur").find(".selected-value").text("THB");

        GM.Defer(function () {
            GM.Message.Clear();
            GM.ThorRate.Form.Search();
        },
            100);
    });
    $("#btnAdd").on("click", function () {
        GM.ThorRate.Form(this);
    });

    //Binding ddl_cur
    $("#ddl_cur_action").click(function () {
        var txt_search = $('#txt_cur_action');
        var data = { cur: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_cur_action').keyup(function () {
        var data = { cur: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    GM.ThorRate.Form.Initial = function () {
        $("#action-form")[0].reset();
        ClearError();
    }

    GM.ThorRate.Form.ClearError = function () {
        ClearError();
    }

    $("#action-form").on('submit', function (e) {

        GM.Message.Clear();
        e.preventDefault(); // prevent the form's normal submission

        GM.ThorRate.Form.ClearError();

        var isValid = true;
        var FormAction_asof_date = $("#FormAction_asof_date");
        var FormAction_curve_id = $("#FormAction_curve_id");
        var ddl_cur_action = $("#ddl_cur_action").find(".selected-value");
        var FormAction_index_type = $("#FormAction_index_type");

        if (FormAction_asof_date.val().trim() == "") {
            $("#asof_date_error").text("The AsOfDate field is required.");
            FormAction_asof_date.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_curve_id.val().trim() == "") {
            $("#curve_id_error").text("The Curve field is required.");
            FormAction_curve_id.addClass("input-validation-error");
            isValid = false;
        }

        if (ddl_cur_action.val().trim() == "") {
            $("#cur_error").text("The Cur field is required.");
            ddl_cur_action.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_index_type.val().trim() == "") {
            $("#index_type_error").text("The Cost Of Fund Rate field is required.");
            FormAction_index_type.addClass("input-validation-error");
            isValid = false;
        }

        if (isValid) {
            var dataToPost = $(this).serialize();
            // var action = $(this).attr('action');
            var action = $(this).attr('action');
            var isdelete = $(this).attr('isdelete');
            var asof_date = $(this).attr('asof_date');
            var curve_id = $(this).attr('curve_id');
            var ccy = $(this).attr('ccy');
            var index_type = $(this).attr('index_type');

            if (isdelete == "false") {
                GM.Mask('#action-form-modal .modal-content');
                $.post(action, dataToPost)

                    .done(function (response, status, jqxhr) {
                        // this is the "success" callback
                        GM.Unmask();

                        if (response.Success) {
                            GM.Message.Success('.modal-body', response.Message);
                            GM.Defer(function () {
                                $('#action-form-modal').modal('hide');
                                GM.ThorRate.Table.draw();
                                GM.ThorRate.Form.Search();
                            }, 500);
                        }
                        else {
                            if (response.RefCode == -53) {
                                swal("Warning!", "Warning : " + response.Message, "warning");
                                $('#action-form-modal').modal('hide');
                                GM.ThorRate.Table.draw();
                                GM.ThorRate.Form.Search();
                            }
                            else {
                                GM.Message.Error('.modal-body', response.Message);
                            }
                        }
                    })

                    .fail(function (jqxhr, status, error) {
                       
                    });
            }
            else {
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
                            var data = {
                                asof_date: asof_date,
                                curve_id: curve_id,
                                ccy: ccy,
                                index_type: index_type
                            };
                            console.log("Show form delete -> local");
                            GM.Defer(function () {
                                $.ajax({
                                    type: "POST",
                                    url: action,
                                    content: "application/json; charset=utf-8",
                                    dataType: "json",
                                    data: data,
                                    success: function (d) {
                                        $('.spinner').css('display', 'none');
                                        if (d.success) {
                                            swal("Deleted!", "Delete Success.", "success");
                                            GM.Defer(function () {
                                                $('#action-form-modal').modal('hide');
                                                GM.Message.Clear();
                                                GM.ThorRate.Table.draw();
                                                GM.ThorRate.Form.Search();
                                            }, 500);

                                        } else {
                                            // DoSomethingElse()
                                            swal("Deleted!", "Error : " + d.responseText, "error");
                                            $('#action-form-modal').modal('hide');
                                            GM.ThorRate.Table.draw();
                                            GM.ThorRate.Form.Search();
                                        }
                                    },
                                    error: function (d) {
                                        $('.spinner').css('display', 'none');
                                        // TODO: Show error
                                        GM.Message.Clear();
                                    }
                                });
                            }, 500);
                        } else {
                            GM.Message.Clear();
                            swal("Cancelled", "Your imaginary file is safe :)", "error");
                        }
                    });
            }
        }

    });
});