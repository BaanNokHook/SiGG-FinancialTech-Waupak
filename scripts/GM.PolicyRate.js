
function numberOnlyAndDot(obj) {
    obj.value = obj.value
        .replace(/[^\d.]/g, '')             // numbers and decimals only
        .replace(/(^[\d]{3})[\d]/g, '$1')   // not more than 3 digits at the beginning
        .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
        .replace(/(\.[\d]{8})./g, '$1');    // not more than 6 digits after decimal
}
function auto8digit  (obj) {
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
$(document).ready(function () {

    //Function Search =============================================  

    //Binding ddl_cur
    $("#ddl_cur").click(function () {
        var txt_search = $('#txt_cur');
        var data = { cur: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_cur').keyup(function () {
        //if (this.value.length > 0) {
        var data = { cur: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
        //else if (this.value.length == 0) {
        //    var data = { datastr: null };
        //    GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });

    //Binding Div Detail
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    }
    GM.PolicyRate.Form.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            switch (key) {
                case "policy_date": GM.PolicyRate.Table.columns(1).search($(this).val()); break;
                case "cur": GM.PolicyRate.Table.columns(2).search($(this).val()); break;
                case "cost_of_fund_date": GM.PolicyRate.Table.columns(3).search($(this).val()); break;
            }
        });
        GM.PolicyRate.Table.draw();
    };
    GM.PolicyRate.Form.DataBinding = function (p) {
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
        GM.PolicyRate.Form.Search();
    });
    $("#search-form").on('reset', function (e) {

        $("#ddl_cur").find(".selected-data").text("Select...");
        $("#ddl_cur").find(".selected-value").text("");

        GM.Defer(function () {
            GM.Message.Clear();
            GM.PolicyRate.Form.Search();
        },
            100);
    });
    $("#btnAdd").on("click", function () {
        GM.PolicyRate.Form(this);
    });

    //Function Add ==============================================
    //Binding ddl_user
  

    //Binding ddl_cur
    $("#ddl_cur_action").click(function () {
        var txt_search = $('#txt_cur_action');
        var data = { cur: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_cur_action').keyup(function () {
        //if (this.value.length > 0) {
        var data = { cur: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
        //else if (this.value.length == 0) {
        //    var data = { datastr: null };
        //    GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });

    GM.PolicyRate.Form.Initial = function () {
        $("#action-form")[0].reset();

        var FormAction_cost_of_fund_date = $("#FormAction_cost_of_fund_date");
        var FormAction_cost_of_fund_rate = $("#FormAction_cost_of_fund_rate");
        var FormAction_policy_date = $("#FormAction_policy_date");
        var ddl_cur_action = $("#ddl_cur_action").find(".selected-value");

        $("#policy_date_error").text("");
        FormAction_policy_date.removeClass("input-validation-error");

        $("#cost_of_fund_date_error").text("");
        FormAction_cost_of_fund_date.removeClass("input-validation-error");

        $("#cost_of_fund_rate_error").text("");
        FormAction_cost_of_fund_rate.removeClass("input-validation-error");

        ddl_cur_action.removeClass("input-validation-error");
        $("#cur_error").text("");

    }

    GM.PolicyRate.Form.ClearError = function () {

        var FormAction_cost_of_fund_date = $("#FormAction_cost_of_fund_date");
        var FormAction_cost_of_fund_rate = $("#FormAction_cost_of_fund_rate");
        var FormAction_policy_date = $("#FormAction_policy_date");
        var ddl_cur_action = $("#ddl_cur_action").find(".selected-value");

        $("#policy_date_error").text("");
        FormAction_policy_date.removeClass("input-validation-error");

        $("#cost_of_fund_date_error").text("");
        FormAction_cost_of_fund_date.removeClass("input-validation-error");

        $("#cost_of_fund_rate_error").text("");
        FormAction_cost_of_fund_rate.removeClass("input-validation-error");

        ddl_cur_action.removeClass("input-validation-error");
        $("#cur_error").text("");

    } 

    $("#action-form").on('submit', function (e) {

        GM.Message.Clear();
        e.preventDefault(); // prevent the form's normal submission

        GM.PolicyRate.Form.ClearError();

        var isValid = true;
        var FormAction_policy_date = $("#FormAction_policy_date");
        var ddl_cur_action = $("#ddl_cur_action").find(".selected-value");
        var FormAction_cost_of_fund_date = $("#FormAction_cost_of_fund_date");
        var FormAction_cost_of_fund_rate = $("#FormAction_cost_of_fund_rate");
        
        if (FormAction_policy_date.val().trim() == "") {
            $("#policy_date_error").text("The Policy Date field is required.");
            FormAction_policy_date.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_cost_of_fund_date.val().trim() == "") {
            $("#cost_of_fund_date_error").text("The Cost Of Fund Date field is required.");
            FormAction_cost_of_fund_date.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_cost_of_fund_rate.val().trim() == "") {
            $("#cost_of_fund_rate_error").text("The Cost Of Fund Rate field is required.");
            FormAction_cost_of_fund_rate.addClass("input-validation-error");
            isValid = false;
        }

        if (ddl_cur_action.val().trim() == "") {
            $("#cur_error").text("The Cur field is required.");
            ddl_cur_action.addClass("input-validation-error");
            isValid = false;
        }

        if (isValid) {
            var dataToPost = $(this).serialize();
           // var action = $(this).attr('action');
            var action = $(this).attr('action');
            var isdelete = $(this).attr('isdelete');
            var key = $(this).attr('keyvalue');
            var key2 = $(this).attr('keyvalue2');
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
                                GM.PolicyRate.Table.draw();
                                GM.PolicyRate.Form.Search();
                            }, 500);
                        }
                        else {
                            if (response.RefCode == -53) {
                                swal("Warning!", "Warning : " + response.Message, "warning");
                                $('#action-form-modal').modal('hide');
                                GM.PolicyRate.Table.draw();
                                GM.PolicyRate.Form.Search();
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
                            var data = { policy_date: key, cur: key2 };
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
                                            GM.PolicyRate.Table.draw();
                                            GM.PolicyRate.Form.Search();     
                                        }, 500);
                                                                          
                                    } else {
                                        // DoSomethingElse()
                                        swal("Deleted!", "Error : " + d.responseText, "error");
                                        $('#action-form-modal').modal('hide');
                                        GM.PolicyRate.Table.draw();
                                        GM.PolicyRate.Form.Search();
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