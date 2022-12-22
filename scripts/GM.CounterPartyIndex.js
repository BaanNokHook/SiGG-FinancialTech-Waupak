//Event Handler
//Handle form submission event
$(document).ready(function () {
    //Override Function
    GM.System.Sidebar.Handler = function() {
        $('#x-table-data').DataTable().columns.adjust();
    };

    GM.CounterParty.Table.SelectAll = function (check) {

        if ($(check)[0].checked) {
            GM.CounterParty.Table.rows().select();
        }
        else {
            GM.CounterParty.Table.rows().deselect();
        }

    };
    GM.CounterParty.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            //console.log(key)
            switch (key) {
                case "counter_party_code": GM.CounterParty.Table.columns(2).search($(this).val()); break;
                case "counter_party_shortname": GM.CounterParty.Table.columns(3).search($(this).val()); break;
                case "counter_party_name": GM.CounterParty.Table.columns(4).search($(this).val()); break;
                case "counter_party_thainame": GM.CounterParty.Table.columns(5).search($(this).val()); break;
                case "tel_no": GM.CounterParty.Table.columns(7).search($(this).val()); break;
                case "counter_party_type_code": GM.CounterParty.Table.columns(8).search($(this).val()); break;
            }
        });

        GM.CounterParty.Table.draw();
    };

    GM.CounterParty.Form = {};
    GM.CounterParty.Form.Initial = function() {
        $("#action-form")[0].reset();
    };

    GM.CounterParty.Form.DataBinding = function (p) {
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

    //$('input[name=custodian_flag]').click(function () {
    //    if ($(this).attr('value') == "Y") {
    //        $("#custodian_id").prop("disabled", false);
    //    }
    //    else {
    //        $("#custodian_id").prop("disabled", true);
    //    }
    //});

    //$('input[name=custodian_flag]').click(function () {
    //    if ($(this).attr('value') == "Y") {
    //        $("#custodian_id").prop("disabled", false);
    //    }
    //    else {
    //        $("#custodian_id").prop("disabled", true);
    //    }
    //});

    $("#search-form").on('submit', function (e) {
        e.preventDefault();
        GM.Message.Clear();
        GM.CounterParty.Search();
    });

    $("#search-form").on('reset', function (e) {
        GM.Defer(function() {
                GM.Message.Clear();
                GM.CounterParty.Search();
            },
            100);
    });

    //$("#province_id").change(function () {
    //    var province_id = $('#province_id').val();
    //    var district_id = $('#district_id');
    //    if (province_id == "") {
    //        $("#district_id").html("");
    //        $("#sub_district_id").html("");
    //        $("#district_id").append($('<option></option>').val("").html("- Please select -"));
    //        $("#sub_district_id").append($('<option></option>').val("").html("- Please select -"));
    //    }
    //    else {
    //        var data = { dataint: province_id, datastr: null };
    //        GM.Utility.DDLAutoComplete(district_id, data);
    //    }
    //});

    //$("#district_id").change(function () {
    //    var district_id = $('#district_id').val();
    //    var sub_district = $("#sub_district_id");
    //    if (district_id == "") {
    //        $("#sub_district_id").html("");
    //        $("#sub_district_id").append($('<option></option>').val("").html("- Please select -"));
    //    }
    //    else {
    //        GM.Utility.DDLStandard(sub_district, district_id);
    //    }
    //});


});