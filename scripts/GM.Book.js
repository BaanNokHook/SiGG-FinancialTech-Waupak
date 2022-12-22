$(document).ready(function () {
    //Override Function
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    }

    GM.Book.Table.SelectAll = function (check) {

        if ($(check)[0].checked) {
            GM.Book.Table.rows().select();
        }
        else {
            GM.Book.Table.rows().deselect();
        }
    }

    GM.Book.Form.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];

            switch (key) {
                case "book_id": GM.Book.Table.columns(0).search($(this).val()); break;
                case "book_name_en": GM.Book.Table.columns(1).search($(this).val()); break;
                case "book_name_th": GM.Book.Table.columns(2).search($(this).val()); break;
                case "port": GM.Book.Table.columns(3).search($(this).val()); break;
                case "user_id": GM.Book.Table.columns(4).search($(this).val()); break;
                case "repo_deal_type": GM.Book.Table.columns(5).search($(this).val()); break;
                case "active_flag":
                    if ($(this).attr("ischeck") == "true") {
                        GM.Book.Table.columns(6).search($(this).val());
                    }
                    break;
            }

        });
        GM.Book.Table.draw();
    };

    GM.Book.Form.Initial = function () {
        $("#action-form")[0].reset();

        var FormAction_book_name_en = $("#FormAction_book_name_en");
        var FormAction_book_name_th = $('#FormAction_book_name_th');

        $("#book_name_en_error").text("");
        $("#book_name_th_error").text("");
        $("#port_error").text("");
        $('#action-form :submit').removeAttr('disabled');

        FormAction_book_name_en.removeClass("input-validation-error");
        FormAction_book_name_th.removeClass("input-validation-error");
        $('#ddl_port').removeClass("input-validation-error");

        $("#ddl_port").find(".selected-data").text("Select...");
    }

    GM.Book.Form.DataBinding = function (p) {
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
                    var inputyesRepo = $("[id=" + inputid + "][value=PRP]");
                    var inputnoRepo = $("[id=" + inputid + "][value=BRP]");

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

                    if (p.data[key] == "PRP") {
                        inputnoRepo.removeAttr('checked');
                        inputnoRepo.attr('ischeck', 'false');
                        inputyesRepo.attr('ischeck', 'true');
                        inputyesRepo.attr('checked', 'checked');
                        inputyesRepo.prop('checked', true);
                    } else {
                        inputyesRepo.removeAttr('checked');
                        inputyesRepo.attr('ischeck', 'false');
                        inputnoRepo.attr('ischeck', 'true');
                        inputnoRepo.attr('checked', 'checked');
                        inputnoRepo.prop('checked', true);
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
        var FormAction_book_name_en = $("#FormAction_book_name_en");
        FormAction_book_name_en.value = FormAction_book_name_en.val().trim();
        var FormAction_book_name_th = $("#FormAction_book_name_th");
        var FormAction_port = $('#FormAction_port');

        $("#book_name_en_error").text("");
        $("#book_name_th_error").text("");
        $("#port_error").text("");

        FormAction_book_name_en.removeClass("input-validation-error");
        FormAction_book_name_th.removeClass("input-validation-error");
        $('#ddl_port').removeClass("input-validation-error");

        if (FormAction_book_name_en.val().trim() == "") {
            $("#book_name_en_error").text("The Book Name (Eng) field is required.");
            FormAction_book_name_en.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_book_name_th.val().trim() == "") {
            $("#book_name_th_error").text("The Book Name (Th) field is required.");
            FormAction_book_name_th.addClass("input-validation-error");
            isValid = false;
        }

        if (FormAction_port.val().trim() == "") {
            $("#port_error").text("The Port field is required.");
            $('#ddl_port').addClass("input-validation-error");
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
                                GM.Book.Form.Search();
                                GM.Book.Table.draw();
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
        GM.Book.Form.Search();
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
                GM.Book.Form.Search();
            },
            100);

    });

    $('#btnAdd').on("click", function () {
        GM.Book.Form(this);
    });

    //Binding ddl_title
    $("#ddl_port").click(function () {
        var txt_search = $('#txt_port');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_port').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
});