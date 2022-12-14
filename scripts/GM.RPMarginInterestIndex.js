//Event Handler
//Handle form submission event
$(document).ready(function () {
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


    //Override Function
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    }

    GM.RPMarginInterest.Table.SelectAll = function (check) {

        if ($(check)[0].checked) {
            GM.RPMarginInterest.Table.rows().select();
        }
        else {
            GM.RPMarginInterest.Table.rows().deselect();
        }
    };

    GM.RPMarginInterest.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            //console.log(key)
            switch (key) {
                case "asof_date": GM.RPMarginInterest.Table.columns(1).search($(this).val()); break;
                case "counter_party_code": GM.RPMarginInterest.Table.columns(3).search($(this).val()); break;
                case "cur": GM.RPMarginInterest.Table.columns(5).search($(this).val()); break;
                case "trans_no": GM.RPMarginInterest.Table.columns(2).search($(this).val()); break;
            }
        });
        GM.RPMarginInterest.Table.draw();
    };

    GM.RPMarginInterest.Form = {};
    GM.RPMarginInterest.Form.Initial = function () {
        $("#action-form")[0].reset();
    }

    GM.RPMarginInterest.Form.DataBinding = function (p) {
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


    $("#search-form").on('submit', function (e) {
        e.preventDefault()
        GM.Message.Clear();
        GM.RPMarginInterest.Search();
    });

    $("#search-form").on('reset', function (e) {
        GM.Defer(function () {
            $('#ddl_cur').find(".selected-data").text("Select...");
            $('#FormSearch_cur').val(null);
            GM.Message.Clear();
            GM.RPMarginInterest.Search();
        }, 100)
    });

});