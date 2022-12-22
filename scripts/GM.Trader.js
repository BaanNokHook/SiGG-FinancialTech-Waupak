

$(document).ready(function () {
    //Override Function
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    }


    GM.Trader.Table.SelectAll = function (check) {

        if ($(check)[0].checked) {
            GM.Trader.Table.rows().select();
        }
        else {
            GM.Trader.Table.rows().deselect();
        }
    }

    GM.Trader.Form.Search = function () {

        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            //console.log(key)
            switch (key) {
                case "trader_id": GM.Trader.Table.columns(0).search($(this).val()); break;
                case "trader_engname": GM.Trader.Table.columns(1).search($(this).val()); break;
                case "trader_thainame": GM.Trader.Table.columns(2).search($(this).val()); break;
                case "active_flag":
                    if ($(this).attr("ischeck") == "true") {
                        GM.Trader.Table.columns(3).search($(this).val());
                    }
                    break;
            }
        });

        GM.Trader.Table.draw();
    };

    GM.Trader.Form.Initial = function () {
        $("#action-form")[0].reset();

        var FormAction_trader_id = $("#FormAction_trader_id");
        var FormAction_trader_engname = $('#FormAction_trader_engname');
        var FormAction_trader_thainame = $('#FormAction_trader_thainame');

        $("#trader_id_error").text("");
        $("#trader_engname_error").text("");
        $("#trader_thainame_error").text("");

        $('#action-form :submit').removeAttr('disabled');

        FormAction_trader_id.removeClass("input-validation-error");
        FormAction_trader_engname.removeClass("input-validation-error");
        FormAction_trader_thainame.removeClass("input-validation-error");
    }

    GM.Trader.Form.DataBinding = function (p) {
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
        var FormAction_trader_id = $("#FormAction_trader_id");
        var FormAction_trader_engname = $('#FormAction_trader_engname');
        var FormAction_trader_thainame = $('#FormAction_trader_thainame');

        $("#trader_id_error").text("");
        $("#trader_engname_error").text("");
        $("#trader_thainame_error").text("");

        FormAction_trader_id.removeClass("input-validation-error");
        FormAction_trader_engname.removeClass("input-validation-error");
        FormAction_trader_thainame.removeClass("input-validation-error");

        if (FormAction_trader_id.val().trim() == "") {
            $("#trader_id_error").text("The Trader ID field is required.");
            FormAction_trader_id.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_trader_engname.val().trim() == "") {
            $("#trader_engname_error").text("The Trader Name (Eng) field is required.");
            FormAction_trader_engname.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_trader_thainame.val().trim() == "") {
            $("#trader_thainame_error").text("The Trader Name (Th) field is required.");
            FormAction_trader_thainame.addClass("input-validation-error");
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
                                GM.Trader.Table.draw();
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
        GM.Trader.Form.Search();
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
                GM.Trader.Form.Search();
            },
            100);
    });

    $('#btnAdd').on("click", function () {
        GM.Trader.Form(this);
    });
});