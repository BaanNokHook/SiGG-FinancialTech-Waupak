

$(document).ready(function () {
    //Operation DDl
    $("#ddl_operation").click(function () {
        var txt_search = $('#txt_operation');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_operation').keyup(function () {

        //if (this.value.length > 0) {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        //}
        //else if (this.value.length == 0) {
        //    var data = { datastr: null };
        //    GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Operation DDl

    //Operation DDl Action From
    $("#ddl_operation_action").click(function () {
        var txt_search = $('#txt_operation_action');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_operation_action').keyup(function () {

        //if (this.value.length > 0) {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        //}
        //else if (this.value.length == 0) {
        //    var data = { datastr: null };
        //    GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Operation DDl Action From

    //Parent Screen Action From   

    $("#ddl_parent_screen_action").click(function () {
        var txt_search = $('#txt_parent_screen_action');
        txt_search.val("");
        var operation_id = $('#FormAction_operation_id').val();
        var data = { operation_id: operation_id};

        GM.Utility.DDLAutoComplete(txt_search, data, null);
    });

    $('#txt_parent_screen_action').keyup(function () {
        //if (this.value.length > 0) {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Parent Screen Action From

    //Override Function
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    }
    GM.Screen.Table.SelectAll = function (check) {
        if ($(check)[0].checked) {
            GM.Screen.Table.rows().select();
        }
        else {
            GM.Screen.Table.rows().deselect();
        }
    }
    GM.Screen.Form.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];           
            switch (key) {
                case "screen_id": GM.Screen.Table.columns(0).search($(this).val()); break;
                case "screen_name": GM.Screen.Table.columns(1).search($(this).val()); break;
                case "controller": GM.Screen.Table.columns(2).search($(this).val()); break;
                case "action": GM.Screen.Table.columns(3).search($(this).val()); break;
                case "operation_id": GM.Screen.Table.columns(4).search($(this).val()); break;
                case "operation_name": GM.Screen.Table.columns(5).search($(this).val()); break;
                case "parent_screen_id": GM.Screen.Table.columns(6).search($(this).val()); break;
                //case "parent_screen_id": GM.Screen.Table.columns(8).search($(this).val()); break;
                case "icon": GM.Screen.Table.columns(7).search($(this).val()); break;
                case "active_flag":   
                    if ($(this).attr("ischeck") == "true") {
                        GM.Screen.Table.columns(8).search($(this).val()); 
                    }
                    break;
            }
        });

        GM.Screen.Table.draw();
    };
    GM.Screen.Form.Initial = function () {
        $("#action-form")[0].reset();
    }

    GM.Screen.Form.DataBinding = function (p) {
       
        $('#' + p.form + ' :input').each(function () {
            var input = $(this);
            var inputid = input[0].id;
            var key = input[0].name.split('.')[1];
            var inputtype = input.attr("type");
            var inputvalue = input.attr("value");
            if (p.data[key] != null) {
                if (inputtype == "radio") {
                    var inputyes = $("[id=" + inputid +"][value=true]");
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
    $("#action-form").on('submit', function (e) {

        GM.Message.Clear();

        e.preventDefault(); // prevent the form's normal submission

        var dataToPost = $(this).serialize();
        console.log(dataToPost);
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
                            GM.Screen.Table.draw();
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
        $("#ddl_operation_action").find(".selected-data").text("Select...");
        $("#ddl_parent_screen_action").find(".selected-data").text("Select...");
        $("#ddl_operation").find(".selected-data").text("Select...");
        $("#operation_name").val(null);
        $("#operation_name").text(null);
        $("#operation_id").val(null);
        $("#operation_id").text(null);
    });
    $("#search-form").on('submit', function (e) {

        e.preventDefault();

        GM.Message.Clear();
        GM.Screen.Form.Search();
    });

    $("#search-form").on('reset', function (e) {
        GM.Defer(function() {
                GM.Message.Clear();
                GM.Screen.Form.Search();
            },
            100);
        $("#ddl_operation").find(".selected-data").text("Select...");
        $("#operation_name").val(null);
        $("#operation_name").text(null);
        $("#operation_id").val(null);
        $("#operation_id").text(null);
    });

    $('#btnAdd').on("click", function () {
        GM.Screen.Form(this);
    });


    //$(".ddl-operation").on('change', function () {
    //    var form = $(this).attr('group-form');
    //    var ddl = $('#' + form + ' .ddl-parrent');
    //    var panel = '#' + form + '-modal';

    //    if (form === 'action-form') {
    //        panel = '#' + form + '-modal  .modal-content';
    //    }

    //    ddl.find('option').remove();

    //    $('<option/>', { value: "", text: "- Please select -" }).appendTo(ddl);

    //    GM.Mask(panel);

    //    $.get("/screen/getparentwithoperation", { operation: $(this).val() })
    //        .done(function (datas, status, jqxhr) {

    //            GM.Defer(function () {

    //                $.each(datas, function (index, data) {
    //                    $('<option/>', { value: data.Value, text: data.Text }).appendTo(ddl);
    //                });

    //                GM.Unmask();

    //            }, 500);

    //        })
    //        .fail(function (jqxhr, status, error) {
    //            // this is the ""error"" callback
    //            //console.log("fail");
    //        });
    //});

});