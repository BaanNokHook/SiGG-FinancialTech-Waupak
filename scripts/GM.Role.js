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

    GM.Role.Table.SelectAll = function (check) {

        if ($(check)[0].checked) {
            GM.Role.Table.rows().select();
        }
        else {
            GM.Role.Table.rows().deselect();
        }
    }

    GM.Role.Form.Search = function () {

        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            //console.log(key)
            switch (key) {
                case "role_id": GM.Role.Table.columns(1).search($(this).val()); break;
                case "role_code": GM.Role.Table.columns(2).search($(this).val()); break;
                case "role_name": GM.Role.Table.columns(3).search($(this).val()); break;
                case "active_flag":
                    if ($(this).attr("ischeck") == "true") {
                        GM.Role.Table.columns(4).search($(this).val());
                    }
                    break;
            }
        });

        GM.Role.Table.draw();
    };
    GM.Role.Form.Initial = function() {
        $("#action-form")[0].reset();

        $('#action-form :submit').removeAttr('disabled');
    };

    GM.Role.Form.DataBinding = function (p) {
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
                            GM.Role.Table.draw();
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
    });
    $("#search-form").on('submit', function (e) {

        e.preventDefault();

        GM.Message.Clear();
        GM.Role.Form.Search();
    });

    $("#search-form").on('reset', function (e) {
        GM.Defer(function() {
                GM.Message.Clear();
                GM.Role.Form.Search();
            },
            100);
    });

    $('#btnAdd').on("click", function () {
        GM.Role.Form(this);
    });

});