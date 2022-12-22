//Event Handler
//Handle form submission event
$(document).ready(function () {
    //Override Function
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    };

    GM.Security.Table.SelectAll = function (check) {

        if ($(check)[0].checked) {
            GM.Security.Table.rows().select();
        }
        else {
            GM.Security.Table.rows().deselect();
        }

    };
    GM.Security.Search = function () {

        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            //console.log(key)
            switch (key) {
                case "instrument_code": GM.Security.Table.columns(1).search($(this).val()); break;
                case "ISIN_code": GM.Security.Table.columns(2).search($(this).val()); break;
                case "instrument_desc": GM.Security.Table.columns(3).search($(this).val()); break;
                case "instrumenttype": GM.Security.Table.columns(4).search($(this).val()); break;
                case "begining_par": GM.Security.Table.columns(5).search($(this).val()); break;
                case "xa_day": GM.Security.Table.columns(6).search($(this).val()); break;
                case "xi_day": GM.Security.Table.columns(7).search($(this).val()); break;
                case "issuer_name": GM.Security.Table.columns(8).search($(this).val()); break;
                case "issue_date": GM.Security.Table.columns(9).search($(this).val()); break;
                case "maturity_date": GM.Security.Table.columns(10).search($(this).val()); break;
                case "cur": GM.Security.Table.columns(11).search($(this).val()); break;
                case "redemption_method_id": GM.Security.Table.columns(12).search($(this).val()); break;
                case "redemp_method": GM.Security.Table.columns(13).search($(this).val()); break;
                case "coupon_rate": GM.Security.Table.columns(14).search($(this).val()); break;
                case "coupon_type": GM.Security.Table.columns(15).search($(this).val()); break;
                case "instrument_owner": GM.Security.Table.columns(16).search($(this).val()); break;
            }
        });
        GM.Security.Table.draw();
    };

    GM.Security.Form = {};
    GM.Security.Form.Initial = function () {
        $("#action-form")[0].reset();
    };

    GM.Security.Form.DataBinding = function (p) {
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
        e.preventDefault();
        GM.Message.Clear();
        GM.Security.Search();
    });

    $("#search-form").on('reset', function (e) {
        GM.Defer(function () {

            $('#ddl_instrument_code').find(".selected-data").text("Select...");
            $('#FormSearch_instrument_id').val(null);
            $('#FormSearch_instrument_code').val(null);

            $('#ddl_instrumenttype').find(".selected-data").text("Select...");
            $('#FormSearch_instrumenttype').val(null);
            $('#FormSearch_instrumenttype_text').val(null);

            $('#ddl_isin_code').find(".selected-data").text("Select...");
            $('#FormSearch_ISIN_code').val(null);
            $('#FormSearch_ISIN_code').val(null);

            $('#ddl_issuer').find(".selected-data").text("Select...");
            $('#FormSearch_issuer_id').val(null);
            $('#FormSearch_issuer_name').val(null);

            GM.Message.Clear();
            GM.Security.Search();
        }, 100);
    });

    $("#ddl_instrumenttype").click(function () {
        var txt_search = $('#txt_instrumenttype');
        var product_code = $('#product_code').val();

        var data = { instrumenttypename: null, productcode: product_code };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_instrumenttype').keyup(function () {
        var product_code = $('#product_code').val();
        var data = { instrumenttypename: this.value, productcode: product_code };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Issuer
    $("#ddl_issuer").click(function () {
        var txt_search = $('#txt_issuer');
        var data = { issuername: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_issuer').keyup(function () {
        var data = { issuername: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Issuer

    //InstrumentCode Dropdown
    var checkinstrumentcode;
    $("#ddl_instrument_code").click(function () {
        var txt_search = $('#txt_instrument_code');
        var data = { text: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_instrument_code').keyup(function () {
        var data = { text: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End InstrumentCode Dropdown

    //isincode Dropdown
    $("#ddl_isin_code").click(function () {
        var txt_search = $('#txt_isin_code');
        var data = { text: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_isin_code').keyup(function () {
        var data = { text: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End isincode Dropdown
});