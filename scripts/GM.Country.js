$(document).ready(function () {

    $('.radio input[id=FormSearch_active_flag]').change(function () {
        var current = $(this).val();
        var radioyes = $("[id=FormSearch_active_flag][value=true]");
        var radiono = $("[id=FormSearch_active_flag][value=false]");
        if (current == "true") {
            radioyes.attr('ischeck', 'true');
            radiono.attr('ischeck', 'false');
            radioyes.attr("checked", "checked");
            radiono.removeAttr("checked");
        }
        else {
            radioyes.attr('ischeck', 'false');
            radiono.attr('ischeck', 'true');
            radiono.attr("checked", "checked");
            radioyes.removeAttr("checked");
        }
    });

    $('.radio input[id=FormAction_active_flag]').change(function () {
        var current = $(this).val();
        var radioyes = $("[id=FormAction_active_flag][value=true]");
        var radiono = $("[id=FormAction_active_flag][value=false]");
        if (current == "true") {
            radioyes.attr('ischeck', 'true');
            radiono.attr('ischeck', 'false');
            radioyes.attr("checked", "checked");
            radiono.removeAttr("checked");
        }
        else {
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

    GM.Country.Table.SelectAll = function (check) {

        if ($(check)[0].checked) {
            GM.Country.Table.rows().select();
        }
        else {
            GM.Country.Table.rows().deselect();
        }
    }

    GM.Country.Form.Search = function () {

        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            //console.log(key)
            switch (key) {
                case "country_id": GM.Country.Table.columns(1).search($(this).val()); break;
                case "country_code": GM.Country.Table.columns(2).search($(this).val()); break;
                case "country_desc": GM.Country.Table.columns(3).search($(this).val()); break;
                case "domicile_code": GM.Country.Table.columns(4).search($(this).val()); break;
                case "domicile_desc": GM.Country.Table.columns(5).search($(this).val()); break;
                case "active_flag":
                    if ($(this).attr("ischeck") == "true") {
                        GM.Country.Table.columns(6).search($(this).val());
                    }
                    break;
            }
        });

        GM.Country.Table.draw();
    };
    GM.Country.Form.Initial = function() {
        $("#action-form")[0].reset();

        var FormAction_domicile_code = $("#FormAction_domicile_code");
        var FormAction_country_code = $('#FormAction_country_code');
        var FormAction_country_desc = $('#FormAction_country_desc');

        $("#domicile_code_error").text("");
        $("#country_code_error").text("");
        $("#country_desc_error").text("");

        $('#action-form :submit').removeAttr('disabled');

        FormAction_domicile_code.removeClass("input-validation-error");
        FormAction_country_code.removeClass("input-validation-error");
        FormAction_country_desc.removeClass("input-validation-error");
    };

    GM.Country.Form.DataBinding = function (p) {
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

        var FormAction_domicile_code = $("#FormAction_domicile_code");
        var FormAction_country_code = $('#FormAction_country_code');
        var FormAction_country_desc = $('#FormAction_country_desc');

        $("#domicile_code_error").text("");
        $("#country_code_error").text("");
        $("#country_desc_error").text("");

        FormAction_domicile_code.removeClass("input-validation-error");
        FormAction_country_code.removeClass("input-validation-error");
        FormAction_country_desc.removeClass("input-validation-error");

        if (FormAction_domicile_code.val().trim() == "") {
            $("#domicile_code_error").text("The Domicile Code field is required.");
            FormAction_domicile_code.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_country_code.val().trim() == "") {
            $("#country_code_error").text("The Country Code field is required.");
            FormAction_country_code.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_country_desc.val().trim() == "") {
            $("#country_desc_error").text("The Country Desc field is required.");
            FormAction_country_desc.addClass("input-validation-error");
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
                                GM.Country.Table.draw();
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
        GM.Country.Form.Search();
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
                GM.Country.Form.Search();
            },
            100);
    });

    $('#btnAdd').on("click", function () {
        GM.Country.Form(this);
    });

    $("#FormAction_country_code").on("keypress", function (evt) {
        if ((evt.key >= "a" && evt.key <= "z") || (evt.key >= "A" && evt.key <= "Z")
            || (evt.key >= "0" && evt.key <= "9")) {
        } else {
            evt.preventDefault();
        }
    });

});