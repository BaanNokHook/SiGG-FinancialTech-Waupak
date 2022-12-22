$(document).ready(function () {
    //Floating Index Dropdown
    $("#ddl_floatingindex").click(function () {
        var txt_search = $('#txt_floatingindex');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_floatingindex').keyup(function () {
        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Floating Index Dropdown

    //Currency Dropdown
    $("#ddl_currency").click(function () {
        var txt_search = $('#txt_currency');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_currency').keyup(function () {
        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Currency Dropdown


    //Function Search ==============================================
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    }
    GM.FloatingIndex.Form = {};
    GM.FloatingIndex.Form.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            switch (key) {
                case "floating_index_code": GM.FloatingIndex.Table.columns(2).search($(this).val()); break;
                case "cur": GM.FloatingIndex.Table.columns(3).search($(this).val()); break;
            }
        });

        GM.FloatingIndex.Table.draw();
    };

    GM.FloatingIndex.Form.Initial = function () {
        $("#action-form")[0].reset();
    };
    GM.FloatingIndex.Form.DataBinding = function (p) {
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
        e.preventDefault()
        GM.Message.Clear();
        GM.FloatingIndex.Form.Search();
    });
    $("#search-form").on('reset', function (e) {
        GM.Defer(function () {
            GM.Message.Clear();
            GM.FloatingIndex.Form.Search();
        }, 100)
    });
});

function text_OnKeyPress_NumberOnlyAndDotAndM(obj) {
    var keyAble = false;
    try {
        key = window.event.keyCode;
        var val = obj.value;
        if (val.indexOf('.') !== -1) {
            if (key === 46) {
                keyAble = false;
            } else if (key > 47 && key < 58) {
                keyAble = true; // if so, do nothing
            }
            else if (key === 77 || key === 109) {
              
                if (val.indexOf('m') !== -1) {
                    keyAble = false;
                } else if (val.indexOf('M') !== -1) {
                    keyAble = false;
                } else {
                    keyAble = true;
                }
            }
        }
        else if ((key === 46) || (key > 47 && key < 58)) {
            keyAble = true; // if so, do nothing
        } else if (key === 77 || key === 109) {
            if (val.indexOf('m') !== -1) {
                keyAble = false;
            } else if (val.indexOf('M') !== -1) {
                keyAble = false;
            } else {
                keyAble = true;
            }
        } else {
            keyAble = false;
        }
    } catch (err) {
        throw new Error(" error in sub text_OnKeyPress_NumberOnlyAndDotAndM function() cause = " + err.message);
    }
    return keyAble;
}