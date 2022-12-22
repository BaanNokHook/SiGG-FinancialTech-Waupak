$(document).ready(function () {

    $('.radio input[id=FormSearch_active_flag]').change(function() {
        var current = $(this).val();
        var radioyes = $("[id=FormSearch_active_flag][value=true]");
        var radiono = $("[id=FormSearch_active_flag][value=false]");
        if (current == "true") {
            radioyes.attr('ischeck', 'true');
            radiono.attr('ischeck', 'false');
            radioyes.attr("checked", "checked");
            radiono.removeAttr("checked");
        } else {
            radioyes.attr('ischeck', 'false');
            radiono.attr('ischeck', 'true');
            radiono.attr("checked", "checked");
            radioyes.removeAttr("checked");
        }
    });

    $('.radio input[id=FormAction_active_flag]').change(function() {
        var current = $(this).val();
        var radioyes = $("[id=FormAction_active_flag][value=true]");
        var radiono = $("[id=FormAction_active_flag][value=false]");
        if (current == "true") {
            radioyes.attr('ischeck', 'true');
            radiono.attr('ischeck', 'false');
            radioyes.attr("checked", "checked");
            radiono.removeAttr("checked");
        } else {
            radioyes.attr('ischeck', 'false');
            radiono.attr('ischeck', 'true');
            radiono.attr("checked", "checked");
            radioyes.removeAttr("checked");
        }
    });

    //Override Function
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    }

    GM.Currency.Table.SelectAll = function (check) {

        if ($(check)[0].checked) {
            GM.Currency.Table.rows().select();
        }
        else {
            GM.Currency.Table.rows().deselect();
        }
    }

    GM.Currency.Form.Search = function () {

        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            //console.log(key)
            switch (key) {
                case "cur": GM.Currency.Table.columns(1).search($(this).val()); break;
                case "cur_code": GM.Currency.Table.columns(2).search($(this).val()); break;
                case "cur_desc": GM.Currency.Table.columns(3).search($(this).val()); break;
                case "active_flag":
                    if ($(this).attr("ischeck") == "true") {
                        GM.Currency.Table.columns(4).search($(this).val());
                    }
                    break;
            }
        });

        GM.Currency.Table.draw();
    };
    GM.Currency.Form.Initial = function () {
        $("#action-form")[0].reset();

        var FormAction_cur = $("#FormAction_cur");
        var FormAction_cur_code = $('#FormAction_cur_code');
        var FormAction_cur_desc = $('#FormAction_cur_desc');

        $("#cur_error").text("");
        $("#cur_code_error").text("");
        $("#cur_desc_error").text("");

        $('#action-form :submit').removeAttr('disabled');

        FormAction_cur.removeClass("input-validation-error");
        FormAction_cur_code.removeClass("input-validation-error");
        FormAction_cur_desc.removeClass("input-validation-error");

        FormAction_cur.attr("oncopy", "return false;");
        FormAction_cur.attr("onpaste", "return false;");

        FormAction_cur_code.attr("oncopy", "return false;");
        FormAction_cur_code.attr("onpaste", "return false;");
    }

    GM.Currency.Form.DataBinding = function (p) {
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
        var FormAction_cur = $("#FormAction_cur");
        var FormAction_cur_code = $('#FormAction_cur_code');
        var FormAction_cur_desc = $('#FormAction_cur_desc');

        $("#cur_error").text("");
        $("#cur_code_error").text("");
        $("#cur_desc_error").text("");

        FormAction_cur.removeClass("input-validation-error");
        FormAction_cur_code.removeClass("input-validation-error");
        FormAction_cur_desc.removeClass("input-validation-error");

        if (FormAction_cur.val().trim() == "") {
            $("#cur_error").text("The Curency field is required.");
            FormAction_cur.addClass("input-validation-error");
            isValid = false;
        } else if (FormAction_cur.val().trim().length != 3) {
            $("#cur_error").text("The Curency field is required 3 digits");
            FormAction_cur.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_cur_code.val().trim().trim() == "") {
            $("#cur_code_error").text("The Curency Code field is required.");
            FormAction_cur_code.addClass("input-validation-error");
            isValid = false;
        } else if (FormAction_cur_code.val().trim().length != 2) {
            $("#cur_code_error").text("The Curency Code field is required 2 digits");
            FormAction_cur_code.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_cur_desc.val().trim() == "") {
            $("#cur_desc_error").text("The Curency Desc field is required.");
            FormAction_cur_desc.addClass("input-validation-error");
            isValid = false;
        }

        if (isValid) {

            var dataToPost = $(this).serialize();
            var action = $(this).attr('action');
            var isdelete = $(this).attr('isdelete');
            var key = $(this).attr('keyvalue');
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
                                GM.Currency.Table.draw();
                                GM.Currency.Form.Search();
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
            else {
                swal({
                    title: "Are you sure?",
                    text: "You will not be able to recover this imaginary file!",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonClass: "btn-danger",
                    confirmButtonText: "Yes, delete it!",
                    cancelButtonText: "No, cancel plx!",
                    closeOnConfirm: true,
                    closeOnCancel: true
                },
                    function (isConfirm) {
                        if (isConfirm) {
                            $('.spinner').css('display', 'block');
                            var data = { cur: key };
                            //console.log("Show form delete -> local");
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
                                        $('#action-form-modal').modal('hide');
                                        GM.Message.Clear();
                                        GM.Currency.Table.draw();
                                        GM.Currency.Form.Search();
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
        }
    });
    $("#search-form").on('submit', function (e) {
        e.preventDefault();

        GM.Message.Clear();
        GM.Currency.Form.Search();
    });

    $("#search-form").on('reset', function (e) {
        GM.Defer(function() {
                //reset input
                var activeFlagId = $('input:radio[name = "FormSearch.active_flag"]');
                if (activeFlagId.filter(':checked').val() == "false") {
                    activeFlagId.filter('[value="true"]').prop("checked", true).attr("checked", "checked")
                        .attr('ischeck', 'true');
                    activeFlagId.filter('[value="false"]').prop("checked", false).removeAttr("checked")
                        .attr('ischeck', 'false');
                }

                GM.Message.Clear();
                GM.Currency.Form.Search();
            },
            100);
    });

    $('#btnAdd').on("click", function () {
        GM.Currency.Form(this);
    });

    $("#FormAction_cur").on("keypress", function (evt) {
        if ((evt.key >= "a" && evt.key <= "z") || (evt.key >= "A" && evt.key <= "Z")
            || (evt.key >= "0" && evt.key <= "9") || evt.key == "-" || evt.key == "_") {
        } else {
            evt.preventDefault();
        }
    });

    $("#FormAction_cur_code").on("keypress", function (evt) {
        if ((evt.key >= "a" && evt.key <= "z") || (evt.key >= "A" && evt.key <= "Z")
            || (evt.key >= "0" && evt.key <= "9") || evt.key == "-" || evt.key == "_") {
        } else {
            evt.preventDefault();
        }
    });
});