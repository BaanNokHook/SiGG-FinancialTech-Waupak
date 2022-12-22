$.ajaxSetup({
    cache: false
});

$(document).ready(function () {

    $("#NavBar").html($('#NavAccount').val());

    //Function : Binding Table
    GM.User = {};
    GM.User.Table = $('#x-table-data').DataTable({
        dom: 'Bfrtip',
        searching: true,
        scrollY: '80vh',
        scrollX: true,
        order: [
            [1, "asc"]
        ],
        buttons: [],
        processing: true,
        serverSide: true,
        ajax: {
            "url": "/Account/Search",
            "type": "POST",
            "error": function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR);
                console.log(textStatus);
                console.log(errorThrown);
            }
        },
        columnDefs:
            [
                { targets: 0, data: "RowNumber", orderable: false },
                { targets: 1, data: "user_id" },
                { targets: 2, data: "user_eng_name" },
                {
                    targets: 3, data: "ldap_user_flag", render: function (data, type, row, meta) {
                        if (data != null && data === true) { return 'Yes'; }
                        else { return 'No'; }
                    }
                },
                { targets: 4, data: "role_id", visible: false },
                { targets: 5, data: "role_name" },
                { targets: 6, data: "title_master_id", visible: false },
                { targets: 7, data: "title_master_name" },
                { targets: 8, data: "desk_group_id", visible: false },
                { targets: 9, data: "desk_group_name" },
                { targets: 10, data: "trader_id" },
                { targets: 11, data: "costcenter_code" },
                {
                    targets: 12, data: "user_id", className: "dt-body-center", width: 60, render: function (data, type, row, meta) {
                        var html = '';

                        if (isUpdate == "True") {
                            html += '<button class="btn btn-default btn-round" key="' + row.user_id + '" formaction="edit"   formtype="' + row.ldap_user_flag + '" onclick="GM.User.Form(this)"><i class="feather-icon icon-edit"></i></button>';
                        }
                        else {
                            html += '<button class="btn btn-default btn-round" key="' + row.user_id + '" formaction="view"   formtype="' + row.ldap_user_flag + '" onclick="GM.User.Form(this)"><i class="feather-icon icon-edit"></i></button>';
                        }

                        if (isDelete == "True") {
                            html += '<button class="btn btn-delete  btn-round" key="' + row.user_id + '" formaction="delete" formtype="' + row.ldap_user_flag + '" onclick="GM.User.Form(this)"><i class="feather-icon icon-trash-2"></i></button>';
                        }
                        else {
                            html += '<button class="btn btn-delete  btn-round" key="' + row.user_id + '" formaction="delete" formtype="' + row.ldap_user_flag + '" onclick="GM.User.Form(this)" disabled><i class="feather-icon icon-trash-2"></i></button>';
                        }

                        return html;
                    }
                }
            ],
        fixedColumns: {
            leftColumns: 1,
            rightColumns: 1
        },
        createdRow: function (row, data, dataIndex) {
            if (data.user_id == "gmrepo") {
                $('td', row).hide();
            }
        }
    });

    GM.User.Table.SelectAll = function (check) {

        if ($(check)[0].checked) {
            GM.User.Table.rows().select();
        } else {
            GM.User.Table.rows().deselect();
        }
    };

    //Function : Open Modal Add & Edit
    GM.User.Form = function (btn) {

        var formaction = $(btn).attr("formaction");
        GM.Message.Clear();
        GM.User.Form.Initial();

        if (formaction) {
            var key = $(btn).attr("key");
            var ldapform = $(btn).attr("formtype");
            switch (formaction) {
                case "add":
                    if (ldapform == "true") {
                        $(".modal-title").text("Add LDAP User");

                        $('#radio-ldap').show();
                        $('#btnLdapLookUp').prop('disabled', false);
                        $('#LdapForm_user_id').attr('readonly', false);
                        $('#LdapForm_user_eng_name').removeAttr('disabled', 'disabled');
                        $('#LdapForm_user_thai_name').removeAttr('disabled', 'disabled');
                        $('#ddl_role_ldap_action').removeAttr('disabled', 'disabled');
                        $('#ddl_title_ldap_action').removeAttr('disabled', 'disabled');
                        $('#ddl_deskgroup_ldap_action').removeAttr('disabled', 'disabled');
                        $('#ddl_traderid_ldap_action').removeAttr('disabled', 'disabled');
                        $('#LdapForm_desk_sub_group').removeAttr('disabled', 'disabled');

                        $('input:radio[name="LdapForm.ldap_user_flag"]').filter('[value="true"]').prop("checked", true).attr("checked", "checked").attr('ischeck', 'true');
                        $('input:radio[name="LdapForm.ldap_user_flag"]').filter('[value="false"]').prop("checked", false).removeAttr("checked").attr('ischeck', 'false');
                        $('input:radio[name="LdapForm.ldap_user_flag"]').attr('disabled', 'disabled');
                        $('#section-ldap-password').hide();

                        var radioyes = $("[id=LdapForm_active_flag][value=true]");
                        var radiono = $("[id=LdapForm_active_flag][value=false]");
                        radioyes.removeAttr('disabled', 'disabled');
                        radiono.removeAttr('disabled', 'disable');

                        $('#add-ldap-user-modal').modal('show');
                        $("#ldapform :submit").removeAttr("onclick");
                        $("#ldapform :submit").removeClass('btn-delete').addClass('btn-primary').text('Create');
                        $("#ldapform").attr("action", "/Account/Create");
                    }
                    else {
                        $(".modal-title").text("Add Local User");

                        $('#btnLocalLookUp').prop('disabled', true);
                        $('#LocalForm_user_id').removeAttr('disabled');
                        $('#LocalForm_password').removeAttr('disabled');
                        $('#LocalForm_user_eng_name').removeAttr('disabled');
                        $('#LocalForm_user_thai_name').removeAttr('disabled');
                        $('#ddl_role_action').removeAttr('disabled');
                        $('#ddl_title_action').removeAttr('disabled');
                        $('#ddl_deskgroup_action').removeAttr('disabled');
                        $('#ddl_traderid_action').removeAttr('disabled');
                        $('#LocalForm_desk_sub_group').removeAttr('disabled');
                        $('#LocalForm_active_flag').removeAttr('disabled');

                        $('input:radio[name="LocalForm.ldap_user_flag"]').filter('[value="false"]').prop("checked", true).attr("checked", "checked").attr('ischeck', 'true');
                        $('input:radio[name="LocalForm.ldap_user_flag"]').filter('[value="true"]').prop("checked", false).removeAttr("checked").attr('ischeck', 'false');
                        $('input:radio[name="LocalForm.ldap_user_flag"]').attr('disabled', 'disabled');
                        $('#section-local-password').show();

                        var radioyes = $("[id=LocalForm_active_flag][value=true]");
                        var radiono = $("[id=LocalForm_active_flag][value=false]");
                        radioyes.removeAttr('disabled');
                        radiono.removeAttr('disabled');
                        $('#LocalForm_user_id').attr('readonly', false);

                        $('#add-local-user-modal').modal('show');
                        $("#localform :submit").removeAttr("onclick");
                        $("#localform :submit").removeClass('btn-delete').addClass('btn-primary').text('Create');
                        $("#localform").attr("action", "/Account/Create");
                    }
                    break;

                case "edit":

                    if (ldapform == "true") {
                        GM.User.Form.GetUser({
                            user_id: key,
                            form: 'ldapform',
                            handler: function (response, status, jqxhr) {
                                //console.log("response:", response);
                            }
                        });

                        $(".modal-title").text("Edit LDAP User");

                        $('#radio-ldap').hide();
                        $('#btnLdapLookUp').prop('disabled', false);
                        $('#LdapForm_user_id').attr('readonly', true);
                        $('#LdapForm.ddl_user_id').removeAttr('required');
                        $('#LdapForm_user_eng_name').removeAttr('disabled', 'disabled');
                        $('#LdapForm_user_thai_name').removeAttr('disabled', 'disabled');
                        $('#ddl_role_ldap_action').removeAttr('disabled', 'disabled');
                        $('#ddl_title_ldap_action').removeAttr('disabled', 'disabled');
                        $('#ddl_deskgroup_ldap_action').removeAttr('disabled', 'disabled');
                        $('#ddl_traderid_ldap_action').removeAttr('disabled', 'disabled');
                        $('#LdapForm_desk_sub_group').removeAttr('disabled', 'disabled');

                        $('input:radio[name="LdapForm.ldap_user_flag"]').removeAttr('disabled', 'disabled');
                        $('#section-ldap-password').hide();

                        var radioyes = $("[id=LdapForm_active_flag][value=true]");
                        var radiono = $("[id=LdapForm_active_flag][value=false]");
                        radioyes.removeAttr('disabled', 'disabled');
                        radiono.removeAttr('disabled', 'disable');

                        $('#add-ldap-user-modal').modal('show');
                        $("#ldapform :submit").removeAttr("onclick");
                        $("#ldapform :submit").removeClass('btn-delete').addClass('btn-primary').text('Update');

                        if (isUpdate == "False") {
                            $('#ldapform :submit').attr('disabled', "disabled");
                        }

                        $("#ldapform").attr("action", "/Account/Edit");
                    }
                    else {
                        GM.User.Form.GetUser({
                            user_id: key,
                            form: 'localform',
                            handler: function (response, status, jqxhr) {
                                //console.log("response:", response);
                            }
                        });

                        $(".modal-title").text("Edit Local User");

                        $('#btnLocalLookUp').prop('disabled', true);
                        $('#LocalForm_user_id').removeAttr('disabled');
                        $('#LocalForm_password').removeAttr('disabled');
                        $('#LocalForm_user_eng_name').removeAttr('disabled');
                        $('#LocalForm_user_thai_name').removeAttr('disabled');
                        $('#ddl_role_action').removeAttr('disabled');
                        $('#ddl_title_action').removeAttr('disabled');
                        $('#ddl_deskgroup_action').removeAttr('disabled');
                        $('#ddl_traderid_action').removeAttr('disabled');
                        $('#LocalForm_desk_sub_group').removeAttr('disabled');
                        $('#LocalForm_active_flag').removeAttr('disabled');

                        $('input:radio[name="LocalForm.ldap_user_flag"]').removeAttr('disabled');
                        $('#section-local-password').show();

                        var radioyes = $("[id=LocalForm_active_flag][value=true]");
                        var radiono = $("[id=LocalForm_active_flag][value=false]");
                        radioyes.removeAttr('disabled');
                        radiono.removeAttr('disabled');
                        $('#LocalForm_user_id').attr('readonly', true);

                        $('#add-local-user-modal').modal('show');
                        $("#localform :submit").removeAttr("onclick");
                        $("#localform :submit").removeClass('btn-delete').addClass('btn-primary').text('Update');

                        if (isUpdate == "False") {
                            $('#localform :submit').attr('disabled', "disabled");
                        }

                        $("#localform").attr("action", "/Account/Edit");
                    }

                    break;

                case "delete":
                    $(".modal-title").text("Are you sure you want to delete this?");

                    if (ldapform == "true") {
                        GM.User.Form.GetUser({
                            user_id: key,
                            form: 'ldapform',
                            handler: function (response, status, jqxhr) {
                                //console.log("response:", response);
                            }
                        });

                        $('#radio-ldap').hide();
                        $('#btnLdapLookUp').prop('disabled', true);
                        $('#LdapForm_user_id').attr('readonly', true);
                        $('#LdapForm.ddl_user_id').removeAttr('required');

                        $('#LdapForm_user_eng_name').attr('disabled', 'disabled');
                        $('#LdapForm_user_thai_name').attr('disabled', 'disabled');
                        $('#ddl_role_ldap_action').attr('disabled', 'disabled');
                        $('#ddl_title_ldap_action').attr('disabled', 'disabled');
                        $('#ddl_deskgroup_ldap_action').attr('disabled', 'disabled');
                        $('#ddl_traderid_ldap_action').attr('disabled', 'disabled');
                        $('#LdapForm_desk_sub_group').attr('disabled', 'disabled');

                        $('input:radio[name="LdapForm.ldap_user_flag"]').attr('disabled', 'disabled');
                        $('#section-ldap-password').hide();

                        var radioyes = $("[id=LdapForm_active_flag][value=true]");
                        var radiono = $("[id=LdapForm_active_flag][value=false]");
                        radioyes.attr('disabled', 'disabled');
                        radiono.attr('disabled', 'disable');

                        $('#add-ldap-user-modal').modal('show');
                        $("#ldapform :submit").removeClass('btn-primary').addClass('btn-delete').text('Delete');
                        $("#ldapform :submit").attr("onclick", "DeleteLdap('" + key + "')");
                        $("#ldapform").attr("action", "notpost");
                    }
                    else {
                        GM.User.Form.GetUser({
                            user_id: key,
                            form: 'localform',
                            handler: function (response, status, jqxhr) {
                                //console.log("response:", response);
                            }
                        });

                        $('#btnLocalLookUp').prop('disabled', true);
                        $('#LocalForm_user_id').attr('disabled', 'disabled');
                        $('#LocalForm_password').attr('disabled', 'disabled');
                        $('#LocalForm_user_eng_name').attr('disabled', 'disabled');
                        $('#LocalForm_user_thai_name').attr('disabled', 'disabled');
                        $('#ddl_role_action').attr('disabled', 'disabled');
                        $('#ddl_title_action').attr('disabled', 'disabled');
                        $('#ddl_deskgroup_action').attr('disabled', 'disabled');
                        $('#ddl_traderid_action').attr('disabled', 'disabled');
                        $('#LocalForm_desk_sub_group').attr('disabled', 'disabled');

                        $('input:radio[name="LocalForm.ldap_user_flag"]').attr('disabled', 'disabled');
                        $('#section-local-password').show();

                        var radioyes = $("[id=LocalForm_active_flag][value=true]");
                        var radiono = $("[id=LocalForm_active_flag][value=false]");
                        radioyes.attr('disabled', 'disabled');
                        radiono.attr('disabled', 'disable');

                        $('#add-local-user-modal').modal('show');
                        $("#localform :submit").removeClass('btn-primary').addClass('btn-delete').text('Delete');
                        $("#localform :submit").attr("onclick", "Delete('" + key + "')");
                        $("#localform").attr("action", "notpost");
                    }

                    break;
                case "view":
                    if (ldapform == "true") {
                        GM.User.Form.GetUser({
                            user_id: key,
                            form: 'ldapform',
                            handler: function (response, status, jqxhr) {
                                //console.log("response:", response);
                            }
                        });

                        $('#radio-ldap').hide();
                        $('#btnLdapLookUp').prop('disabled', true);
                        $('#LdapForm_user_id').attr('readonly', true);
                        $('#LdapForm.ddl_user_id').removeAttr('required');

                        $('#LdapForm_user_eng_name').attr('disabled', 'disabled');
                        $('#LdapForm_user_thai_name').attr('disabled', 'disabled');
                        $('#ddl_role_ldap_action').attr('disabled', 'disabled');
                        $('#ddl_title_ldap_action').attr('disabled', 'disabled');
                        $('#ddl_deskgroup_ldap_action').attr('disabled', 'disabled');
                        $('#ddl_traderid_ldap_action').attr('disabled', 'disabled');
                        $('#LdapForm_desk_sub_group').attr('disabled', 'disabled');

                        $('input:radio[name="LadpForm.ldap_user_flag"]').attr('disabled', 'disabled');
                        $('#section-ldap-password').hide();

                        var radioyes = $("[id=LdapForm_active_flag][value=true]");
                        var radiono = $("[id=LdapForm_active_flag][value=false]");
                        radioyes.attr('disabled', 'disabled');
                        radiono.attr('disabled', 'disable');

                        $('#add-ldap-user-modal').modal('show');
                        $(".modal-title").text("View Account");
                        $("#ldapform :submit").removeClass('btn-delete').addClass('btn-primary').text('Update');
                        $("#ldapform :submit").attr('disabled', "disabled");
                        $("#ldapform :submit").removeAttr("onclick");
                    }
                    else {
                        GM.User.Form.GetUser({
                            user_id: key,
                            form: 'localform',
                            handler: function (response, status, jqxhr) {
                                //console.log("response:", response);
                            }
                        });

                        $('#btnLocalLookUp').prop('disabled', true);
                        $('#LocalForm_user_id').attr('disabled', 'disabled');
                        $('#LocalForm_password').attr('disabled', 'disabled');
                        $('#LocalForm_user_eng_name').attr('disabled', 'disabled');
                        $('#LocalForm_user_thai_name').attr('disabled', 'disabled');
                        $('#ddl_role_action').attr('disabled', 'disabled');
                        $('#ddl_title_action').attr('disabled', 'disabled');
                        $('#ddl_deskgroup_action').attr('disabled', 'disabled');
                        $('#ddl_traderid_action').attr('disabled', 'disabled');
                        $('#LocalForm_desk_sub_group').attr('disabled', 'disabled');

                        $('input:radio[name="LocalForm.ldap_user_flag"]').attr('disabled', 'disabled');
                        $('#section-local-password').show();

                        var radioyes = $("[id=LocalForm_active_flag][value=true]");
                        var radiono = $("[id=LocalForm_active_flag][value=false]");
                        radioyes.attr('disabled', 'disabled');
                        radiono.attr('disabled', 'disable');

                        $('#add-local-user-modal').modal('show');
                        $(".modal-title").text("View Account");
                        $("#localform :submit").removeClass('btn-delete').addClass('btn-primary').text('Update');
                        $("#localform :submit").attr('disabled', "disabled");
                        $("#localform :submit").removeAttr("onclick");
                    }
                    break;
            }
        }
    };

    //Function : Open ModalEdit
    GM.User.Form.GetUser = function (op) {
        $.get("/Account/Edit", { id: op.user_id, t: GM.Time })
            .done(function (response, status, jqxhr) {
                // this is the "success" callback
                GM.User.Form.DataBinding({ form: op.form, data: response });
                op.handler(response);
            })
            .fail(function (jqxhr, status, error) {
            });
    };

    GM.User.ExportUser = function (btn) {
        swal({
            title: "Do you want to Export User?",
            text: "",
            html: true,
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

                    $.post('/Account/ExportUser')
                        .done(function (response) {
                            if (response.errorMessage === '') {
                                window.location.href = '/Account/Download?filename=' + response.fileName;
                                $('.spinner').css('display', 'none');
                                setTimeout(function () {

                                    swal({
                                        title: "Complete",
                                        text: "Gen Excel Successfully ",
                                        type: "success",
                                        showCancelButton: false,
                                        confirmButtonClass: "btn-success",
                                        confirmButtonText: "Ok",
                                        closeOnConfirm: true
                                    }
                                    );
                                }, 100);
                            } else
                            {
                                setTimeout(function () {
                                    swal("Error", response.errorMessage, "error");
                                }, 100);
                                $('.spinner').css('display', 'none');
                            }
                         
                        });
                }
            }
        )
    };

    GM.System.Sidebar.Handler = function () {
        $('#x-table-account').DataTable().columns.adjust();
    };

    $('#btnLocalLookUp').on('click', function () {

        var userid = $('#LocalForm_user_id').val();

        if (userid) {
            GM.Mask('#add-local-user-modal .modal-content');
            $.get("/account/getldapuser", { userid: userid })
                .done(function (response, status, jqxhr) {
                    GM.Defer(function () {

                        if (response.Success) {
                            var user = response.Data;
                            var en = user.cn;
                            var th = user.thaipersonaltitle + ' ' + user.thaifirstname + ' ' + user.thailastname;

                            $('#LocalForm_user_eng_name').val(en);
                            $('#LocalForm_user_thai_name').val(th);
                        }
                        else {
                            $('#LocalForm_user_eng_name').val("");
                            $('#LocalForm_user_thai_name').val("");
                            $('#LocalForm_costcenter_code').val("");
                        }

                        GM.Unmask();

                    }, 500);
                })
                .fail(function (jqxhr, status, error) {
                    // this is the ""error"" callback
                    //console.log("fail");
                });
        }
        else {
            $('#LocalForm_user_id').focus();
        }
    });

    $('#btnLdapLookUp').on('click', function () {

        var userid = $('#LdapForm_user_id').val();

        if (userid) {
            GM.Mask('#add-ldap-user-modal .modal-content');
            $.get("/account/getldapuser", { userid: userid })
                .done(function (response, status, jqxhr) {
                    GM.Defer(function () {

                        if (response.Success) {
                            var user = response.Data;
                            var en = user.cn;
                            var th = user.thaipersonaltitle + ' ' + user.thaifirstname + ' ' + user.thailastname;
                            var costcenter = user.costcenter;

                            $('#LdapForm_user_eng_name').val(en);
                            $('#LdapForm_user_thai_name').val(th);
                            $('#LdapForm_costcenter_code').val(costcenter);
                        }
                        else {
                            $('#LdapForm_user_eng_name').val("");
                            $('#LdapForm_user_thai_name').val("");
                            $('#LdapForm_costcenter_code').val("");
                        }

                        GM.Unmask();

                    }, 500);
                })
                .fail(function (jqxhr, status, error) {
                    // this is the ""error"" callback
                    //console.log("fail");
                });
        }
        else {
            $('#LdapForm_user_id').focus();
        }
    });

    //Function Search ==============================================
    //Binding ddl_role
    $("#ddl_role").click(function () {
        var txt_search = $('#txt_role');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_role').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Binding ddl_title
    $("#ddl_title").click(function () {
        var txt_search = $('#txt_title');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_title').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Binding ddl_deskgroup
    $("#ddl_deskgroup").click(function () {
        var txt_search = $('#txt_deskgroup');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_deskgroup').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Binding ddl_traderid
    $("#ddl_traderid").click(function () {
        var txt_search = $('#txt_traderid');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_traderid').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Binding Div Detail
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    }
    GM.User.Form.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            switch (key) {
                case "user_id": GM.User.Table.columns(1).search($(this).val()); break;
                case "ldap_user_flag": GM.User.Table.columns(3).search($(this).val()); break;
                case "role_id": GM.User.Table.columns(4).search($(this).val()); break;
                case "title_master_id": GM.User.Table.columns(6).search($(this).val()); break;
                case "desk_group_id": GM.User.Table.columns(9).search($(this).val()); break;
                case "trader_id": GM.User.Table.columns(10).search($(this).val()); break;
            }
        });

        GM.User.Table.draw();
    };
    GM.User.Form.Initial = function () {
        $("#localform")[0].reset();
        $("#ldapform")[0].reset();
        $('#lookup_userid').show();
        $('#lookup_costcenter').hide();

        var LocalForm_user_id = $("#LocalForm_user_id");
        var LocalForm_password = $('#LocalForm_password');
        var LocalForm_user_eng_name = $("#LocalForm_user_eng_name");
        var LocalForm_user_thai_name = $('#LocalForm_user_thai_name');

        var LdapForm_user_id = $("#LdapForm_user_id");

        $("#localform_user_id_error").text("");
        $("#localform_password_error").text("");
        $("#localform_user_eng_name_error").text("");
        $("#localform_user_thai_name_error").text("");
        $("#localform_role_error").text("");
        $("#localform_desk_group_error").text("");

        $("#ldapform_user_id_error").text("");
        $("#ldapform_role_error").text("");
        $("#ldapform_desk_group_error").text("");

        $('#ldapform :submit').removeAttr('disabled');

        LocalForm_user_id.removeClass("input-validation-error");
        LocalForm_password.removeClass("input-validation-error");
        LocalForm_user_eng_name.removeClass("input-validation-error");
        LocalForm_user_thai_name.removeClass("input-validation-error");
        $('#ddl_role_action').removeClass("input-validation-error");
        $('#ddl_deskgroup_action').removeClass("input-validation-error");

        $('#localform :submit').removeAttr('disabled');

        LdapForm_user_id.removeClass("input-validation-error");
        $('#ddl_role_ldap_action').removeClass("input-validation-error");
        $('#ddl_deskgroup_ldap_action').removeClass("input-validation-error");


        $("#ddl_role_action").find(".selected-data").text("Select...");
        $("#ddl_title_action").find(".selected-data").text("Select...");
        $("#ddl_deskgroup_action").find(".selected-data").text("Select...");
        $("#ddl_traderid_action").find(".selected-data").text("Select...");

        $("#ddl_role_ldap_action").find(".selected-data").text("Select...");
        $("#ddl_title_ldap_action").find(".selected-data").text("Select...");
        $("#ddl_deskgroup_ldap_action").find(".selected-data").text("Select...");
        $("#ddl_traderid_ldap_action").find(".selected-data").text("Select...");
    };

    //Binding Div Detail
    GM.User.Form.DataBinding = function (p) {
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
                        //}
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

    $("#search-form").on('submit', function (e) {
        e.preventDefault();
        GM.Message.Clear();
        GM.User.Form.Search();
    });
    $("#search-form").on('reset', function (e) {
        GM.Defer(function () {
            $('#ddl_traderid').find(".selected-data").text("Select...");
            $('#SearchForm_trader_id').val(null);
            $('#SearchForm_trader_id_text').val(null);

            $('#ddl_role').find(".selected-data").text("Select...");
            $('#SearchForm_role_id').val(null);
            $('#SearchForm_role_name').val(null);

            $('#ddl_title').find(".selected-data").text("Select...");
            $('#SearchForm_title_master_id').val(null);
            $('#SearchForm_title_master_name').val(null);

            $('#ddl_deskgroup').find(".selected-data").text("Select...");
            $('#SearchForm_desk_group_id').val(null);
            $('#SearchForm_desk_group_name').val(null);

            GM.Message.Clear();
            GM.User.Form.Search();
        }, 100);
    });

    //Modal Local : Add & Edit ==============================================
    //Binding ddl_role_action
    $("#ddl_role_action").click(function () {
        var txt_search = $('#txt_role_action');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_role_action').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Binding ddl_title_action
    $("#ddl_title_action").click(function () {
        var txt_search = $('#txt_title_action');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_title_action').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Binding ddl_deskgroup_action
    $("#ddl_deskgroup_action").click(function () {
        var txt_search = $('#txt_deskgroup_action');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_deskgroup_action').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Binding ddl_traderid_action
    $("#ddl_traderid_action").click(function () {
        var txt_search = $('#txt_traderid_action');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_traderid_action').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Btn submit
    $("#localform").on('submit', function (e) {
        GM.Message.Clear();
        e.preventDefault(); // prevent the form's normal submission

        var isValid = true;
        var LocalForm_user_id = $("#LocalForm_user_id");
        var LocalForm_password = $('#LocalForm_password');
        var LocalForm_user_eng_name = $("#LocalForm_user_eng_name");
        var LocalForm_user_thai_name = $('#LocalForm_user_thai_name');
        var LocalForm_role_id = $('#LocalForm_role_id');
        var LocalForm_desk_group_id = $('#LocalForm_desk_group_id');

        $("#localform_user_id_error").text("");
        $("#localform_password_error").text("");
        $("#localform_user_eng_name_error").text("");
        $("#localform_user_thai_name_error").text("");
        $("#localform_role_error").text("");
        $("#localform_desk_group_error").text("");

        LocalForm_user_id.removeClass("input-validation-error");
        LocalForm_password.removeClass("input-validation-error");
        LocalForm_user_eng_name.removeClass("input-validation-error");
        LocalForm_user_thai_name.removeClass("input-validation-error");
        $('#ddl_role_action').removeClass("input-validation-error");
        $('#ddl_deskgroup_action').removeClass("input-validation-error");

        if (LocalForm_user_id.val().trim() == "") {
            $("#localform_user_id_error").text("The User ID field is required.");
            LocalForm_user_id.addClass("input-validation-error");
            isValid = false;
        }

        if (LocalForm_user_eng_name.val().trim() == "") {
            $("#localform_user_eng_name_error").text("The English Name field is required.");
            LocalForm_user_eng_name.addClass("input-validation-error");
            isValid = false;
        }

        if (LocalForm_user_thai_name.val().trim() == "") {
            $("#localform_user_thai_name_error").text("The Thai Name field is required.");
            LocalForm_user_thai_name.addClass("input-validation-error");
            isValid = false;
        }

        if (LocalForm_role_id.val().trim() == "") {
            $("#localform_role_error").text("The Role field is required.");
            $('#ddl_role_action').addClass("input-validation-error");
            isValid = false;
        }

        if ($('input:radio[name="LocalForm.ldap_user_flag"]').filter(':checked').val() == "false") {
            if ($('#LocalForm_password').val().trim() == "") {
                $('#LocalForm_password').addClass("input-validation-error");
                $('#localform_password_error').text("The Password field is required.");
                isValid = false;
            }
        }

        if (isValid) {
            var dataToPost = $(this).serialize() + '&LocalForm.ldap_user_flag=' + $('input:radio[name="LocalForm.ldap_user_flag"]').filter(':checked').val();
            console.log(dataToPost);
            var action = $(this).attr('action');
            if (action != "notpost") {

                GM.Mask('#add-local-user-modal .modal-content');
                $.post(action, dataToPost)
                    .done(function (response, status, jqxhr) {
                        // this is the "success" callback
                        GM.Unmask();

                        if (response.Success) {
                            GM.Message.Success('.modal-body', response.Message);
                            GM.Defer(function () {
                                $('#add-local-user-modal').modal('hide');
                                GM.User.Form.Search();
                                GM.User.Table.draw();
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

            $("#ddl_role_action").find(".selected-data").text("Select...");
            $("#ddl_traderrole_action").find(".selected-data").text("Select...");
            $("#ddl_deskgroup_action").find(".selected-data").text("Select...");
        }
    });

    //Modal Ldap : Add & Edit ==============================================
    //Binding ddl_role_ldap_action
    $("#ddl_role_ldap_action").click(function () {
        var txt_search = $('#txt_role_ldap_action');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_role_ldap_action').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Binding ddl_title_ldap_action
    $("#ddl_title_ldap_action").click(function () {
        var txt_search = $('#txt_traderrole_ldap_action');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_title_ldap_action').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Binding ddl_deskgroup_ldap_action
    $("#ddl_deskgroup_ldap_action").click(function () {
        var txt_search = $('#txt_deskgroup_ldap_action');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_deskgroup_ldap_action').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Binding ddl_traderid_ldap_action
    $("#ddl_traderid_ldap_action").click(function () {
        var txt_search = $('#txt_traderid_ldap_action');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_traderid_ldap_action').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    $("#ddl_title_ldap_action").click(function () {
        var txt_search = $('#txt_title_ldap_action');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_title_ldap_action').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Btn submit
    $("#ldapform").on('submit', function (e) {
        GM.Message.Clear();
        e.preventDefault(); // prevent the form's normal submission

        var isValid = true;
        var LdapForm_user_id = $("#LdapForm_user_id");
        var LdapForm_role_id = $('#LdapForm_role_id');
        var LdapForm_desk_group_id = $('#LdapForm_desk_group_id');

        $("#ldapform_user_id_error").text("");
        $("#ldapform_role_error").text("");
        $("#ldapform_desk_group_error").text("");

        LdapForm_user_id.removeClass("input-validation-error");
        $('#ddl_role_ldap_action').removeClass("input-validation-error");
        $('#ddl_deskgroup_ldap_action').removeClass("input-validation-error");

        if (LdapForm_user_id.val().trim() == "") {
            $("#ldapform_user_id_error").text("The User ID field is required.");
            LdapForm_user_id.addClass("input-validation-error");
            isValid = false;
        }

        if (LdapForm_role_id.val().trim() == "") {
            $("#ldapform_role_error").text("The Role field is required.");
            $('#ddl_role_ldap_action').addClass("input-validation-error");
            isValid = false;
        }

        if ($('input:radio[name="LdapForm.ldap_user_flag"]').filter(':checked').val() == "false") {
            if ($('#LdapForm_password').val().trim() == "") {
                $('#LdapForm_password').addClass("input-validation-error");
                $('#ldapform_password_error').text("The Password field is required.");
                isValid = false;
            }
        }

        if (isValid) {

            var dataToPost = $(this).serialize() + '&LdapForm.ldap_user_flag=' + $('input:radio[name="LdapForm.ldap_user_flag"]').filter(':checked').val();
            console.log(dataToPost);
            var action = $(this).attr('action');
            if (action != "notpost") {
                GM.Mask('#add-ldap-user-modal .modal-content');
                $.post(action, dataToPost)
                    .done(function (response, status, jqxhr) {
                        // this is the "success" callback
                        GM.Unmask();

                        if (response.Success) {
                            GM.Message.Success('.modal-body', response.Message);
                            GM.Defer(function () {
                                $('#add-ldap-user-modal').modal('hide');
                                GM.User.Table.draw();
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
            $("#ddl_role_ldap_action").find(".selected-data").text("Select...");
            $("#ddl_traderrole_ldap_action").find(".selected-data").text("Select...");
            $("#ddl_deskgroup_ldap_action").find(".selected-data").text("Select...");

        }
    });

    //radio
    $('input:radio[name="LocalForm.ldap_user_flag"]').change(function () {
        var obj = $('input:radio[name="LocalForm.ldap_user_flag"]');
        if (obj.filter(':checked').val() == "true") {
            $('#btnLocalLookUp').prop('disabled', false);
            $('#LocalForm_user_eng_name').attr('readonly', 'readonly');
            $('#LocalForm_user_thai_name').attr('readonly', 'readonly');
            obj.filter('[value="true"]').prop("checked", true).attr("checked", "checked").attr('ischeck', 'true');
            obj.filter('[value="false"]').prop("checked", false).removeAttr("checked").attr('ischeck', 'false');
            $('#LocalForm_password').val("");
            $('#section-local-password').hide();
        } else {
            $('#btnLocalLookUp').prop('disabled', true);
            $('#LocalForm_user_eng_name').removeAttr('readonly');
            $('#LocalForm_user_thai_name').removeAttr('readonly');
            obj.filter('[value="false"]').prop("checked", true).attr("checked", "checked").attr('ischeck', 'true');
            obj.filter('[value="true"]').prop("checked", false).removeAttr("checked").attr('ischeck', 'false');
            $('#section-local-password').show();
            $('#LocalForm_password').val("").focus();
        }
    });

    $('input:radio[name="LdapForm.ldap_user_flag"]').change(function () {
        var obj = $('input:radio[name="LdapForm.ldap_user_flag"]');
        if (obj.filter(':checked').val() == "true") {
            $('#btnLdapLookUp').prop('disabled', false);
            $('#LdapForm_user_eng_name').attr('readonly', 'readonly');
            $('#LdapForm_user_thai_name').attr('readonly', 'readonly');
            obj.filter('[value="true"]').prop("checked", true).attr("checked", "checked").attr('ischeck', 'true');
            obj.filter('[value="false"]').prop("checked", false).removeAttr("checked").attr('ischeck', 'false');
            $('#LdapForm_password').val("");
            $('#section-ldap-password').hide();
        } else {
            $('#btnLdapLookUp').prop('disabled', true);
            $('#LdapForm_user_eng_name').removeAttr('readonly');
            $('#LdapForm_user_thai_name').removeAttr('readonly');
            obj.filter('[value="false"]').prop("checked", true).attr("checked", "checked").attr('ischeck', 'true');
            obj.filter('[value="true"]').prop("checked", false).removeAttr("checked").attr('ischeck', 'false');
            $('#section-ldap-password').show();
            $('#LdapForm_password').val("").focus();
        }
    });
});

function Delete(user_id) {
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
                var data = { user_id: user_id };
                //console.log("Show form delete -> local");
                $.ajax({
                    type: "POST",
                    url: "/Account/Deletes",
                    content: "application/json; charset=utf-8",
                    dataType: "json",
                    data: data,
                    success: function (d) {
                        $('.spinner').css('display', 'none');
                        if (d.success) {
                            setTimeout(function () {
                                swal("Deleted!", "Delete Success.", "success");
                            }, 100);
                            GM.Message.Clear();
                            GM.Defer(function () {
                                $('#add-local-user-modal').modal('hide');
                                GM.User.Form.Search();
                                GM.User.Table.draw();
                            }, 500);

                        } else {
                            // DoSomethingElse()
                            swal("Deleted!", "Error : " + d.responseText, "error");
                        }
                    },
                    error: function (d) {
                        // TODO: Show error
                        $('.spinner').css('display', 'none');
                        GM.Message.Clear();
                    }
                });
            } else {
                GM.Message.Clear();
                swal("Cancelled", "Your imaginary file is safe :)", "error");
            }
        });
}

function DeleteLdap(user_id) {
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
                var data = { user_id: user_id };
                $.ajax({
                    type: "POST",
                    url: "/Account/Deletes",
                    content: "application/json; charset=utf-8",
                    dataType: "json",
                    data: data,
                    success: function (d) {
                        $('.spinner').css('display', 'none');
                        if (d.success) {
                            setTimeout(function () {
                                swal("Deleted!", "Delete Success.", "success");
                            }, 100);
                            $('#add-ldap-user-modal').modal('hide');
                            GM.Message.Clear();
                            GM.User.Form.Search();
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