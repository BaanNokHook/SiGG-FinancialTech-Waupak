
function setformatdateyyyymmdd(date) {
    if (date != "") {
        date = date.split('/');
        date = date[2] + "" + date[1] + "" + date[0];
    }
    else {
        date = 0;
    }
    return date;
}
$(document).ready(function () {

    $('#FormAction_HolidayStart').on('dp.change', function (e) {
        if (e.date) {
            var date = moment(e.date).format('DD/MM/YYYY');
            if ($('#FormAction_HolidayEnd').text() != "") {
                var date_from = setformatdateyyyymmdd(moment(e.date).format('DD/MM/YYYY'));
                var date_to = setformatdateyyyymmdd($('#FormAction_HolidayEnd').text());
                if (date_from > date_to) {
                    $('#FormAction_HolidayEnd').text(date);
                    $('#FormAction_HolidayEnd').val(date);
                    $('#FormAction_HolidayStart').text(date);
                    $('#FormAction_HolidayStart').val(date);
                }
                else {

                    $('#FormAction_HolidayStart').text(date);
                    $('#FormAction_HolidayStart').val(date);
                }
            }
            else {
                $('#FormAction_HolidayStart').text(date);
                $('#FormAction_HolidayStart').val(date);
            }
        }
    });

    $('#FormAction_HolidayEnd').on('dp.change', function (e) {
        if (e.date) {
            var date_from;
            var date = moment(e.date).format('DD/MM/YYYY');

            if ($('#FormAction_HolidayStart').text() == "") {
                // swal("Warning", "Please select As Of Date From Before", "warning");
                $('#FormAction_HolidayEnd').text(date);
                $('#FormAction_HolidayEnd').val(date);
                $('#FormAction_HolidayStart').text(date);
                $('#FormAction_HolidayStart').val(date);
            }
            else {
                date_from = setformatdateyyyymmdd($('#FormAction_HolidayStart').text());
                var date_to = setformatdateyyyymmdd(moment(e.date).format('DD/MM/YYYY'));
                if (date_from > date_to) {
                    //swal("Warning", "Date To Can not less than Date From", "warning");
                    // $('#asofdate_to_string').text($('#asofdate_from_string').text());
                    // $('#asofdate_to_string').val($('#asofdate_from_string').text());
                    $('#FormAction_HolidayEnd').text(date);
                    $('#FormAction_HolidayEnd').val(date);
                    $('#FormAction_HolidayStart').text(date);
                    $('#FormAction_HolidayStart').val(date);
                }
                else {
                    $('#FormAction_HolidayEnd').text(date);
                    $('#FormAction_HolidayEnd').val(date);
                }
            }

        }
    });

  
    //Cur Dropdown
    $("#ddl_cur").click(function () {
        var txt_search = $('#txt_cur');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_cur').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    GM.Holiday.Form.Initial = function() {
        $("#action-form")[0].reset();

        $('#action-form :submit').removeAttr('disabled');
    };

    $('#btnAdd1').on("click", function () {
        //GM.Holiday.Form(this);
    });

    //$('#btnAdd2').on("click", function() {
    //    GM.Holiday.Form(this);
    //});

    GM.Holiday.Form.DataBinding = function (p) {
        $('#' + p.form + ' :input').each(function () {
            var input = $(this);
            var inputid = input[0].id;
            var key = input[0].name.split('.')[1];
            var inputtype = input.attr("type");
            var inputvalue = input.attr("value");
            if (p.data[key] != null) {
                if (key == "holiday_date") {
                }
                else {
                    $(this).val(p.data[key] + '');
                }
            }
        });
        $('#' + p.form + ' span').each(function () {
            var input = $(this);
            var key = input.attr("name");
            if (typeof key != "undefined") {
                if (p.data[key] != "" && p.data[key] != null) {
                    $(this)[0].innerHTML = p.data[key];
                }
            }
        });
    };
});