$(document).ready(function () {

    $('.radio input[id=view_flag]').change(function () {
        var current = $(this).val();
        var radioyes = $("[id=view_flag][value=true]");
        var radiono = $("[id=view_flag][value=false]");

        var radioCreate = $('[name=create_flag]');
        var radioUpdate = $('[name=update_flag]');
        var radioDelete = $('[name=delete_flag]');

        if (current == "true") {
            radioyes.attr('ischeck', 'true');
            radiono.attr('ischeck', 'false');
            radioyes.attr("checked", "checked");
            radiono.removeAttr("checked");

            radioCreate.removeAttr('disabled');
            radioUpdate.removeAttr('disabled');
            radioDelete.removeAttr('disabled');
        }
        else {
            radioyes.attr('ischeck', 'false');
            radiono.attr('ischeck', 'true');
            radiono.attr("checked", "checked");
            radioyes.removeAttr("checked");

            radioCreate.attr('readonly', 'readonly');
            radioUpdate.attr('readonly', 'readonly');
            radioDelete.attr('readonly', 'readonly');

            radioCreate.filter('[value=false]').prop('checked', true);
            radioUpdate.filter('[value=false]').prop('checked', true);
            radioDelete.filter('[value=false]').prop('checked', true);
        }
    });

    $("#ddl_screen_action").click(function () {
        var txt_search = $('#txt_screen_action');
        var role_id = $('#Txt_role_id').val();

        var data = { dataint: role_id, datastr: null };

        GM.Utility.DDLAutoComplete(txt_search, data, null);

        txt_search.val("");
    });
    $('#txt_screen_action').keyup(function () {
        var role_id = $('#Txt_role_id').val();
        var data = { dataint: role_id, datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Override Function
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    };

    GM.RoleAndScreen.Table.SelectAll = function (check) {

        if ($(check)[0].checked) {
            GM.RoleAndScreen.Table.rows().select();
        }
        else {
            GM.RoleAndScreen.Table.rows().deselect();
        }
    };

    GM.RoleAndScreen.Form.Search = function () {

        //$('#search-form :input').each(function () {
        //    var input = $(this);
        //    var key = input[0].name;
        //    //var key = input[0].name.split('.')[1];
        //    //console.log(key)
        //    switch (key) {
        //        case "screen_name": GM.RoleAndScreen.Table.columns(1).search($(this).val()); break;
        //            break;
        //    }
        //});

        GM.RoleAndScreen.Table.draw();
    };
    GM.RoleAndScreen.Form.Initial = function () {
        $("#action-form")[0].reset();

        $('#action-form :submit').removeAttr('disabled');
    };

    GM.RoleAndScreen.Form.DataBinding = function (p) {
        $('#' + p.form + ' :input').each(function () {
            var input = $(this);
            var inputid = input[0].id;
            var key = input[0].name;
            //var key = input[0].name.split('.')[1];
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
                            GM.RoleAndScreen.Table.draw();
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
        GM.RoleAndScreen.Form.Search();
    });

    $("#search-form").on('reset', function (e) {
        GM.Defer(function () {
            GM.Message.Clear();
            GM.RoleAndScreen.Form.Search();
        },
            100);
    });

    $('#btnAdd').on("click", function () {
        GM.RoleAndScreen.Form(this);
    });

});