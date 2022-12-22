$(document).ready(function () {

    //Function Search ==============================================

    //Binding ddl_role
    $("#ddl_IssuerType").click(function () {
        var txt_search = $('#txt_IssuerType');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_IssuerType').keyup(function () {

        //if (this.value.length > 0) {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        //}
        //else if (this.value.length == 0) {
        //    var data = { datastr: null };
        //    GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });

    //Binding Div Detail
    GM.System.Sidebar.Handler = function() {
        $('#x-table-data').DataTable().columns.adjust();
    };
    GM.Issuer.Form = {};
    GM.Issuer.Form.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            switch (key) {
                case "issuer_code": GM.Issuer.Table.columns(3).search($(this).val()); break;
                case "issuer_name": GM.Issuer.Table.columns(4).search($(this).val()); break;
                case "issuer_shortname": GM.Issuer.Table.columns(5).search($(this).val()); break;
                case "issuer_thainame": GM.Issuer.Table.columns(6).search($(this).val()); break;
                case "issuer_type_desc": GM.Issuer.Table.columns(7).search($(this).val()); break;
                case "tel_no": GM.Issuer.Table.columns(10).search($(this).val()); break;
            }
        });

        GM.Issuer.Table.draw();
    };

    GM.Issuer.Form.Initial = function () {
        $("#action-form")[0].reset();
    };
    GM.Issuer.Form.DataBinding = function (p) {
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
        GM.Issuer.Form.Search();
    });
    $("#search-form").on('reset', function (e) {
        GM.Defer(function() {
                $('#ddl_IssuerType').find(".selected-data").text("Select...");
                $('#FormSearch_issuer_type_desc').val(null);
                $('#FormSearch_issuer_type_code').val(null);

                GM.Message.Clear();
                GM.Issuer.Form.Search();
            },
            100);
    });
});