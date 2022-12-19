

$(window).on('load', function () {
    var action = "/SystemInformation/GetBusinessDate";
    $.ajax({
        url: action,
        type: "GET",
        dataType: "JSON",
        data: null,
        success: function (res) {
            if (!res.Error) {
                var business = new Date(res.Data);
                console.log("business : " + business);
                $("#asofdate_from_string").data("DateTimePicker").date(business);
                $("#asofdate_to_string").data("DateTimePicker").date(business);
            } else {
                swal("Warning", res.Error_detail, "warning");
            }
        }
    });
});

$(document).ready(function () {

    $("#ddl_repodealtype").click(function () {
        var txt_search = $("#txt_repodealtype");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_repodealtype").keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    $("#ddl_port").click(function () {
        var txt_search = $("#txt_port");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_port").keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    $("#ddl_currency").click(function () {
        var txt_search = $("#txt_currency");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_currency").keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    $("#ddl_counterparty").click(function () {
        var txt_search = $("#txt_counterparty");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#txt_counterparty").keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    $("#asofdate_from_string").on("dp.change",
        function (e) {
            if (e.date) {
                var date = moment(e.date).format("DD/MM/YYYY");
                if ($("#asofdate_to_string").val() != "") {
                    var date_from = setformatdateyyyymmdd(moment(e.date).format("DD/MM/YYYY"));
                    var date_to = setformatdateyyyymmdd($("#asofdate_to_string").val());

                    var month_form = moment(date, "DD/MM/YYYY").months();
                    var month_to = moment($("#asofdate_to_string").val(), "DD/MM/YYYY").months();

                    console.log("month_to : " + month_to);
                    console.log("month_form : " + month_form);

                    if (month_to !== month_form) {
                        $("#asofdate_to_string").text(date);
                        $("#asofdate_to_string").val(date);
                    }
                    else if (date_from > date_to) {
                        $("#asofdate_to_string").text(date);
                        $("#asofdate_to_string").val(date);
                        $("#asofdate_from_string").text(date);
                        $("#asofdate_from_string").val(date);
                    } else {
                        $("#asofdate_from_string").text(date);
                        $("#asofdate_from_string").val(date);
                    }
                } else {
                    $("#asofdate_from_string").text(date);
                    $("#asofdate_from_string").val(date);
                    $("#asofdate_to_string").text(date);
                    $("#asofdate_to_string").val(date);
                }
            }
        }
    );

    $("#asofdate_to_string").on("dp.change",
        //function (e) {
        //    if (e.date) {
        //        var date = moment(e.date).format("DD/MM/YYYY");
        //        var date_to = setformatdateyyyymmdd(moment(e.date).format("DD/MM/YYYY"));
        //        var date_from = setformatdateyyyymmdd($("#asofdate_from_string").text());
        //        var month_to = moment($("#asofdate_to_string").val(), "DD/MM/YYYY").months();
        //        var month_form = moment($("#asofdate_from_string").val(), "DD/MM/YYYY").months();

        //        console.log("month_to : " + month_to);
        //        console.log("month_form : " + month_form);

        //        if (month_to !== month_form) {
        //            setTimeout(function () {
        //                swal("Warning", "PL report can only select as monthly basis", "warning");
        //            }, 50);

        //            $("#asofdate_to_string").text($("#asofdate_from_string").text());
        //            $("#asofdate_to_string").val($("#asofdate_from_string").val());
        //        } else {
        //            if (date_to < date_from) {
        //                $("#asofdate_from_string").text(date);
        //                $("#asofdate_from_string").val(date);
        //            }
        //            $("#asofdate_to_string").text(date);
        //            $("#asofdate_to_string").val(date);
        //        }
        //    }
        //}
        function (e) {
            var date1 = new Date(setformatdatemmddyyyy($("#asofdate_from_string").val()));
            var date2 = new Date(moment(e.date).format("MM/DD/YYYY"));
            var limitDate = new Date(date1.getFullYear(), date1.getMonth() + 4, 1);
            var maxdate = new Date(date1.getFullYear(), date1.getMonth() + 4, 1);
            maxdate.setDate(maxdate.getDate() - 1);
            max_date = moment(maxdate).format("DD/MM/YYYY");
            if (date2 >= limitDate) {
                swal("Warning", "Can not select more then 4 month", "warning");
                $("#asofdate_to_string").text(max_date);
                $("#asofdate_to_string").val(max_date);
                return;
            }
        });

    $("form").on("submit",
        function (e) {
            if (!$("#asofdate_from_string").val().length) {
                e.preventDefault();
                $("#asofdate_from_string_error").text("As Of Date field is required.");
                $("#asofdate_from_string").addClass("input-validation-error");
            } else {
                $("#asofdate_from_string_error").text("");
                $("#asofdate_from_string").removeClass("input-validation-error");
            }

            if (!$("#asofdate_to_string").val().length) {
                e.preventDefault();
                $("#asofdate_to_string_error").text("As Of Date field is required.");
                $("#asofdate_to_string").addClass("input-validation-error");
            } else {
                $("#asofdate_to_string_error").text("");
                $("#asofdate_to_string").removeClass("input-validation-error");
            }

            setTimeout(function () {
                $('.spinner').css('display', 'block'); // Open Loading
            }, 50);

            if ($('#excel_category').val() !== 'M') {
                setTimeout(function () {
                    $('.spinner').css('display', 'none'); // Open Loading
                }, 1000);
            } else {
                setTimeout(function () {
                    $('.spinner').css('display', 'none'); // Open Loading
                }, 1000);
            }

            return;
        });
});