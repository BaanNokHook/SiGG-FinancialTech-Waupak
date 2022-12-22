$(document).ready(function () {

    //Function Search ==============================================

    //Binding ddl_user
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

    //Binding ddl_deskgroup
    $("#ddl_deskgroup").click(function () {
        var txt_search = $('#txt_deskgroup');
        var data = { desk_group_name: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_deskgroup').keyup(function () {
        var data = { desk_group_name: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Binding ddl_cur
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

    //Binding Div Detail
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    }
    GM.TraderLimit.Form.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            switch (key) {
                case "user_id": GM.TraderLimit.Table.columns(1).search($(this).val()); break;
                case "cur": GM.TraderLimit.Table.columns(3).search($(this).val()); break;
                case "active_flag":
                    if ($(this).attr("ischeck") == "true") {
                        GM.TraderLimit.Table.columns(13).search($(this).val());
                    }
                    break;
                case "desk_group_id": GM.TraderLimit.Table.columns(14).search($(this).val()); break;
            }
        });
        GM.TraderLimit.Table.draw();
    };
    GM.TraderLimit.Form.DataBinding = function (p) {
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
                else if (key == 'limit_amount' || key == 'threshold_amount' || key == 'threshold_percent') {
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
        GM.TraderLimit.Form.Search();
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
        $("#ddl_deskgroup").find(".selected-data").text("Select...");
        $("#ddl_deskgroup").find(".selected-value").text("");
        $("#ddl_cur").find(".selected-data").text("Select...");
        $("#ddl_cur").find(".selected-value").text("");

        GM.Defer(function () {
            GM.Message.Clear();
            GM.TraderLimit.Form.Search();
        },
            100);
    });
    $("#btnAdd").on("click", function () {
        GM.TraderLimit.Form(this);
    });

    //Function Add ==============================================
    //Binding ddl_user
    $("#ddl_user_action").click(function () {
        var txt_search = $('#txt_user_action');
        var data = { user_id: null };
        GM.Utility.DDLAutoCompleteSet4Value(txt_search, data, "Hid_desk_group_id", "Txt_desk_group_name", null);
        txt_search.val("");
    });
    $('#txt_user_action').keyup(function () {
        var data = { user_id: this.value };
        GM.Utility.DDLAutoCompleteSet4Value(this, data, "Hid_desk_group_id", "Txt_desk_group_name", null);
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

    GM.TraderLimit.Form.Initial = function () {
        $("#action-form")[0].reset();

        var ddl_user_action = $("#ddl_user_action").find(".selected-value");
        var FormAction_limit_amount = $("#FormAction_limit_amount");
        var FormAction_threshold_amount = $("#FormAction_threshold_amount");
        var FormAction_threshold_percent = $("#FormAction_threshold_percent");
        var ddl_cur_action = $("#ddl_cur_action").find(".selected-value");
        var FormAction_start_date = $("#FormAction_start_date");
        var FormAction_expire_date = $("#FormAction_expire_date");

        $("#user_id_error").text("");
        ddl_user_action.removeClass("input-validation-error");
        $("#limit_amount_error").text("");
        FormAction_limit_amount.removeClass("input-validation-error");
        $("#threshold_amount_error").text("");
        FormAction_threshold_amount.removeClass("input-validation-error");
        $("#threshold_percent_error").text("");
        FormAction_threshold_percent.removeClass("input-validation-error");
        $("#cur_error").text("");
        ddl_cur_action.removeClass("input-validation-error");
        $("#start_date_error").text("");
        FormAction_start_date.removeClass("input-validation-error");
        $("#expire_date_error").text("");
        FormAction_expire_date.removeClass("input-validation-error");
    };

    $("#action-form").on('submit', function (e) {

        GM.Message.Clear();
        e.preventDefault(); // prevent the form's normal submission

        var isValid = true;

        var ddl_user_action = $("#ddl_user_action").find(".selected-value");
        var FormAction_limit_amount = $("#FormAction_limit_amount");
        var FormAction_threshold_amount = $("#FormAction_threshold_amount");
        var FormAction_threshold_percent = $("#FormAction_threshold_percent");
        var ddl_cur_action = $("#ddl_cur_action").find(".selected-value");
        var FormAction_start_date = $("#FormAction_start_date");
        var FormAction_expire_date = $("#FormAction_expire_date");

        if (ddl_user_action.val().trim() == "") {
            $("#user_id_error").text("The User Id field is required.");
            ddl_user_action.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_limit_amount.val().trim() == "") {
            $("#limit_amount_error").text("The Limit Amount field is required.");
            FormAction_limit_amount.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_threshold_amount.val().trim() == "") {
            $("#threshold_amount_error").text("The Threshold Amount field is required.");
            FormAction_threshold_amount.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_threshold_percent.val().trim() == "") {
            $("#threshold_percent_error").text("The Threshold Percent field is required.");
            FormAction_threshold_percent.addClass("input-validation-error");
            isValid = false;
        }

        if (ddl_cur_action.val().trim() == "") {
            $("#cur_error").text("The Cur field is required.");
            ddl_cur_action.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_start_date.val().trim() == "") {
            $("#start_date_error").text("The Start Date field is required.");
            FormAction_start_date.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_expire_date.val().trim() == "") {
            $("#expire_date_error").text("The Expire Date field is required.");
            FormAction_expire_date.addClass("input-validation-error");
            isValid = false;
        }

        if (isValid) {
            var dataToPost = $(this).serialize();
            var action = $(this).attr('action');
            if (action != "notpost") {
                GM.Mask('#action-form-modal .modal-content');
                $.post(action, dataToPost)

                    .done(function (response, status, jqxhr) {
                        // this is the "success" callback
                        GM.Unmask();

                        if (response.Success) {
                            GM.Message.Success('.modal-body', response.Message);
                            GM.Defer(function () {
                                $('#action-form-modal').modal('hide');
                                GM.TraderLimit.Table.draw();
                                GM.TraderLimit.Form.Search();
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

    $("#FormAction_limit_amount").blur(function () {
        Cal_Percent("amount");
    });

    $("#FormAction_threshold_amount").blur(function () {
        Cal_Percent("amount");
    });
    $("#FormAction_threshold_percent").blur(function () {
        Cal_Percent("percent");
    });

    function FormatDecimal(Num, point) {
        var format = Number(parseFloat(Num).toFixed(point)).toLocaleString('en', {
            minimumFractionDigits: point
        });
        return format;
    }

    function Cal_Percent(type) {

        var limit_amount = $("#FormAction_limit_amount").val();
        var threshold_amount = $("#FormAction_threshold_amount").val();
        var threshold_percent = $("#FormAction_threshold_percent").val();
        if (limit_amount !== '' && threshold_amount !== '') {
            var v_limit_amount;
            var v_threshold_amount;
            var v_threshold_percent;

            if (type == "amount") {

                v_limit_amount = limit_amount.replace(/[^\d.]/g, '');
                v_threshold_amount = threshold_amount.replace(/[^\d.]/g, '');

                if (v_limit_amount == 0) {
                    swal("Warning", "Limit Amount Divide By Zero", "error");
                }
                else {
                    v_threshold_percent = (v_threshold_amount * 100) / v_limit_amount;
                    $("#FormAction_threshold_percent").val(FormatDecimal(v_threshold_percent, 2));
                }
            }
            else if (type == "percent") {

                v_limit_amount = parseFloat(limit_amount.replace(/,/g, ''));
                v_threshold_percent = parseFloat(threshold_percent.replace(/,/g, ''));
                v_threshold_amount = v_threshold_percent * v_limit_amount / 100;
                $("#FormAction_threshold_amount").val(FormatDecimal(v_threshold_amount, 2));
            }
        }
    }
});