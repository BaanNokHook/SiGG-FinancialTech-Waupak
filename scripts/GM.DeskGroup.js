

$(document).ready(function () {
    //Override Function
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    }

    GM.DeskGroup.Table.SelectAll = function (check) {

        if ($(check)[0].checked) {
            GM.DeskGroup.Table.rows().select();
        }
        else {
            GM.DeskGroup.Table.rows().deselect();
        }
    }
    GM.DeskGroup.Form.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
           
            switch (key) {
                case "desk_group_id": GM.DeskGroup.Table.columns(0).search($(this).val()); break;
                case "desk_group_name": GM.DeskGroup.Table.columns(1).search($(this).val()); break;
                case "active_flag":
                    if ($(this).attr("ischeck") == "true") {
                        GM.DeskGroup.Table.columns(2).search($(this).val());                       
                    }
                    break;
            }

        });
        GM.DeskGroup.Table.draw();
    };

    GM.DeskGroup.Form.Initial = function () {
        $("#action-form")[0].reset();

        var FormAction_desk_group_name = $("#FormAction_desk_group_name");

        $("#desk_group_name_error").text("");

        $('#action-form :submit').removeAttr('disabled');

        FormAction_desk_group_name.removeClass("input-validation-error");
    }    

    GM.DeskGroup.Form.DataBinding = function (p) {
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
        var FormAction_desk_group_name = $("#FormAction_desk_group_name");

        $("#desk_group_name_error").text("");

        FormAction_desk_group_name.removeClass("input-validation-error");

        if (FormAction_desk_group_name.val().trim() == "") {
            $("#desk_group_name_error").text("The Desk Group Name field is required.");
            FormAction_desk_group_name.addClass("input-validation-error");
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
                                GM.DeskGroup.Table.draw();
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
        GM.DeskGroup.Form.Search();
    });

    $("#search-form").on('reset', function (e) {
        var radioyes = $("[id=FormSearch_active_flag][value=true]");
        var radiono = $("[id=FormSearch_active_flag][value=false]");
        radioyes.attr('ischeck', 'true');
        radioyes.prop('checked', true);
        radioyes.attr('checked', 'checked');
        radiono.removeAttr('checked');
        radiono.attr('ischeck', 'false');
        GM.Defer(function() {
                GM.Message.Clear();
                GM.DeskGroup.Form.Search();
            },
            100);

    });

    $('#btnAdd').on("click", function () {
        GM.DeskGroup.Form(this);
    });
});