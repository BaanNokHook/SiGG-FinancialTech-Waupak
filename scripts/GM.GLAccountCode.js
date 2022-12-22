
function numberOnlyAndDot(obj) {
    obj.value = obj.value
        .replace(/[^\d.]/g, '')             // numbers and decimals only
        .replace(/(^[\d]{3})[\d]/g, '$1')   // not more than 3 digits at the beginning
        .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
        .replace(/(\.[\d]{8})./g, '$1');    // not more than 6 digits after decimal
}
function auto8digit(obj) {
    if (obj.value.length) {
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
function text_OnKeyPress_NumberOnlyAndMinus(obj) {

    var keyAble = false;
    try {
        key = window.event.keyCode;
        var beforeVal = obj.value;
        if (key > 47 && key < 58) {
            keyAble = true; // if so, do nothing
        }
        else if (key == 45) {
            keyAble = true;
        }
        else {
            keyAble = false;
        }       
    } catch (err) {
        throw new Error(" error in sub text_OnKeyPress_NumberOnlyAndDotAndMinus function() cause = " + err.message);
    }
    return keyAble;
}
$(document).ready(function () {

    //Function Search =============================================  
    //Binding ddl_acct_port
    $("#ddl_acct_port").click(function () {
        var txt_search = $('#txt_acct_port');
        var data = { cur: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_acct_port').keyup(function () {
        var data = { cur: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Binding ddl_acct_port_action
    $("#ddl_acct_port_action").click(function () {
        var txt_search = $('#txt_acct_port_action');
        var data = { cur: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_acct_port_action').keyup(function () {
        var data = { cur: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Binding Div Detail
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    }
    GM.GLAccountCode.Form.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            switch (key) {
                case "account_num": GM.GLAccountCode.Table.columns(1).search($(this).val()); break;
                case "account_name": GM.GLAccountCode.Table.columns(2).search($(this).val()); break;
                case "acct_port": GM.GLAccountCode.Table.columns(3).search($(this).val()); break;
            }
        });
        GM.GLAccountCode.Table.draw();
    };
    GM.GLAccountCode.Form.DataBinding = function (p) {
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
        var datanum = auto8digitval($("#FormAction_cost_of_fund_rate").val());
        $("#FormAction_cost_of_fund_rate").val(datanum);
    };

    $("#search-form").on('submit', function (e) {
        e.preventDefault();
        GM.Message.Clear();
        GM.GLAccountCode.Form.Search();
    });
    $("#search-form").on('reset', function (e) {

        $("#ddl_acct_port").find(".selected-data").text("Select...");
        $("#ddl_acct_port").find(".selected-value").text("");

        GM.Defer(function () {
            GM.Message.Clear();
            GM.GLAccountCode.Form.Search();
        },
            100);
    });
    $("#btnAdd").on("click", function () {
        GM.GLAccountCode.Form(this);
    });

    //Function Add ==============================================   

    GM.GLAccountCode.Form.Initial = function () {
        $("#action-form")[0].reset();

        var FormAction_account_num = $("#FormAction_account_num");
        var FormAction_account_name = $("#FormAction_account_name");        
        var ddl_acct_port_action = $("#ddl_acct_port_action").find(".selected-value");
        var FormAction_exp_acct_num = $("#FormAction_exp_acct_num");
        var FormAction_acct_remark = $("#FormAction_acct_remark");

        $("#account_num_error").text("");
        FormAction_account_num.removeClass("input-validation-error");
    }

    GM.GLAccountCode.Form.ClearError = function () {

        var FormAction_account_num = $("#FormAction_account_num");

        $("#account_num_error").text("");
        FormAction_account_num.removeClass("input-validation-error");

    }

    $("#action-form").on('submit', function (e) {

        GM.Message.Clear();
        e.preventDefault(); // prevent the form's normal submission

        GM.GLAccountCode.Form.ClearError();

        var isValid = true;
        var FormAction_account_num = $("#FormAction_account_num");
        var FormAction_account_name = $("#FormAction_account_name");
        var ddl_acct_port_action = $("#ddl_acct_port_action").find(".selected-value");
        var FormAction_exp_acct_num = $("#FormAction_exp_acct_num");
        var FormAction_acct_remark = $("#FormAction_acct_remark");

        if (FormAction_account_num.val().trim() == "") {
            $("#account_num_error").text("The Account Number field is required.");
            FormAction_account_num.addClass("input-validation-error");
            isValid = false;
        }
        
        if (isValid) {
            var dataToPost = $(this).serialize();
            // var action = $(this).attr('action');
            var action = $(this).attr('action');
            var isdelete = $(this).attr('isdelete');
            var key = $(this).attr('keyvalue');
            // if (action != "notpost") {
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
                                GM.GLAccountCode.Table.draw();
                                GM.GLAccountCode.Form.Search();
                            }, 500);
                        }
                        else {
                            if (response.RefCode == -53) {
                                swal("Warning!", "Warning : " + response.Message, "warning");
                                $('#action-form-modal').modal('hide');
                                GM.GLAccountCode.Table.draw();
                                GM.GLAccountCode.Form.Search();
                            }
                            else {
                                GM.Message.Error('.modal-body', response.Message);
                            }
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
                    text: "Confirm Delete ?",
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
                            var data = { account_num: key };
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
                                                GM.GLAccountCode.Table.draw();
                                                GM.GLAccountCode.Form.Search();
                                            }, 500);

                                        } else {
                                            // DoSomethingElse()
                                            swal("Deleted!", "Error : " + d.responseText, "error");
                                            $('#action-form-modal').modal('hide');
                                            GM.GLAccountCode.Table.draw();
                                            GM.GLAccountCode.Form.Search();
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


    function FormatDecimal(Num) {
        return Num.toString().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,");
    }

});