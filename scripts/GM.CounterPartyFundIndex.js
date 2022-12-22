//Event Handler
//Handle form submission event
$(document).ready(function () {
    //Override Function
    GM.System.Sidebar.Handler = function() {
        $('#x-table-data').DataTable().columns.adjust();
    };

    GM.CounterPartyFund.Table.SelectAll = function (check) {

        if ($(check)[0].checked) {
            GM.CounterPartyFund.Table.rows().select();
        }
        else {
            GM.CounterPartyFund.Table.rows().deselect();
        }

    };
    GM.CounterPartyFund.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            //console.log(key)
            switch (key) {
                case "counter_party_code": GM.CounterPartyFund.Table.columns(1).search($(this).val()); break;
                case "fund_code": GM.CounterPartyFund.Table.columns(2).search($(this).val()); break;
            }
        });

        GM.CounterPartyFund.Table.draw();
    };

    GM.CounterPartyFund.Form = {};
    GM.CounterPartyFund.Form.Initial = function() {
        $("#action-form")[0].reset();
    };

    GM.CounterPartyFund.Form.DataBinding = function (p) {
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
        GM.CounterPartyFund.Search();
    });

    $("#search-form").on('reset', function (e) {
        GM.Defer(function() {
                GM.Message.Clear();
                GM.CounterPartyFund.Search();
            },
            100);
    });
});