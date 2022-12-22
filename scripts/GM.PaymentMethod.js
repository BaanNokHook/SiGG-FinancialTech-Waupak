$(document).ready(function () {

    $("#search-form").on('reset', function (e) {
        $('#ddl_PaymentFlag').find(".selected-data").text("Select...");
        $('#payment_flag_item').val(null);
        $('#payment_flag').val(null);
        GM.Message.Clear();
        GM.PaymentMethod.Form.Search();
    });

    //Binding ddl_role
    $("#ddl_PaymentFlag").click(function () {
        var txt_search = $('#txt_PaymentFlag');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_PaymentFlag').keyup(function () {

        //if (this.value.length > 0) {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        //}
        //else if (this.value.length == 0) {
        //    var data = { datastr: null };
        //    GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });

    $("#ddl_PaymentFlag_action").click(function () {
        var txt_search = $('#txt_PaymentFlag_action');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_PaymentFlag_action').keyup(function () {

        //if (this.value.length > 0) {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        //}
        //else if (this.value.length == 0) {
        //    var data = { datastr: null };
        //    GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });

    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    }

    GM.PaymentMethod.Form.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];

            switch (key) {
                case "payment_method": GM.PaymentMethod.Table.columns(1).search($(this).val()); break;
                case "payment_flag": GM.PaymentMethod.Table.columns(2).search($(this).val()); break;
                case "system_type": GM.PaymentMethod.Table.columns(4).search($(this).val()); break;
            }

        });
        GM.PaymentMethod.Table.draw();
    };

    GM.PaymentMethod.Form.Initial = function () {
        $("#action-form")[0].reset();

        var FormAction_payment_method = $("#FormAction_payment_method");
        var FormAction_system_type = $('#FormAction_system_type');
        var FormAction_payment_flag = $('#FormAction_payment_flag');
        $("#payment_method_error").text("");
        $("#system_type_error").text("");
        $("#payment_flag_error").text("");

        $('#action-form :submit').removeAttr('disabled');

        FormAction_payment_method.removeClass("input-validation-error");
        FormAction_system_type.removeClass("input-validation-error");
        FormAction_payment_flag.removeClass("input-validation-error");
    }  

    GM.PaymentMethod.Form.DataBinding = function (p) {
        $('#' + p.form + ' :input').each(function () {
            var input = $(this);
            var inputid = input[0].id;
            var key = input[0].name.split('.')[1];
            var inputtype = input.attr("type");
            var inputvalue = input.attr("value");
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

    $("#action-form").on('submit', function (e) {

        GM.Message.Clear();
        e.preventDefault(); // prevent the form's normal submission

        var isValid = true;

        var FormAction_payment_method = $("#FormAction_payment_method");
        var FormAction_system_type = $('#FormAction_system_type');
        var FormAction_payment_flag = $('#FormAction_payment_flag');

        $("#payment_method_error").text("");
        $("#system_type_error").text("");
        $("#payment_flag_error").text("");

        FormAction_payment_method.removeClass("input-validation-error");
        FormAction_system_type.removeClass("input-validation-error");
        FormAction_payment_flag.removeClass("input-validation-error");
  
        if (FormAction_payment_method.val().trim() == "") {
            $("#payment_method_error").text("The Payment Method field is required.");
            FormAction_payment_method.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_system_type.val().trim() == "") {
            $("#system_type_error").text("The System Type field is required.");
            FormAction_system_type.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_payment_flag.val().trim() == "") {
            $("#payment_flag_error").text("The Payment Flag field is required.");
            FormAction_payment_flag.addClass("input-validation-error");
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
                                GM.PaymentMethod.Table.draw();
                            }, 500);
                        }
                        else {
                            GM.Message.Error('.modal-body', response.Message);
                        }
                    })

                    .fail(function (jqxhr, status, error) {
                        // this is the ""error"" callback
                        //console.log("fail");
                    });
            }
        }
    });

    $("#search-form").on('submit', function (e) {

        e.preventDefault();
        GM.Message.Clear();
        GM.PaymentMethod.Form.Search();
    });

    $("#search-form").on('reset', function (e) {
        GM.Defer(function() {
                GM.Message.Clear();
                GM.PaymentMethod.Form.Search();
            },
            100);

    });

    $('#btnAdd').on("click", function () {
        GM.PaymentMethod.Form(this);
    });

});