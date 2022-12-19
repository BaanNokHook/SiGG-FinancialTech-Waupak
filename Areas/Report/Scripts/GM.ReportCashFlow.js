function checkinstrument() {
    var instrument;
    var radioprp = $("[id=repo_deal_type][value=PRP]");
    var radiobrp = $("[id=repo_deal_type][value=BRP]");
    if (radioprp.attr("ischeck") == "true") {
        instrument = "PRP";
    } else {
        instrument = "BRP";
    }

    return instrument;
}

$(window).on('load', function () {
    var action = "/OutstandingReport/GetBusinessDate";
    GM.Utility.GetBusinessDate("asofdate_from_string", action);
    setTimeout(
        function () {

            GM.Utility.GetBusinessDate("asofdate_to_string", action);
        },
        200);

    $("#currency").val("THB");
});

var max_date;

$(document).ready(function () {

    //Repodealtype Dropdown
    $("#ddl_repodealtype").click(function () {
        var txt_search = $("#txt_repodealtype");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null, true, "Consolidate");
        txt_search.val("");
    });

    $("#txt_repodealtype").keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null, true, "Consolidate");
    });
    //End Repodealtype Dropdown

    //Port Dropdown
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
    //End Port Dropdown

    //Currency Dropdown
    $("#ddl_cur").click(function () {
        var txt_search = $("#txt_cur");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });

    $("#txt_cur").keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null, false);
    });
    //End Currency Dropdown

    $("#asofdate_from_string").on("dp.change",
        function (e) {

            var date = moment(e.date).format("DD/MM/YYYY");
            var date_from = new Date(moment(e.date).format("MM/DD/YYYY"));
            var date_to = new Date(setformatdatemmddyyyy($("#asofdate_to_string").val()));

            var limitDate = new Date(date_from.getFullYear(), date_from.getMonth() + 4, 1);
            var maxdate = new Date(date_from.getFullYear(), date_from.getMonth() + 4, 1);
            maxdate.setDate(maxdate.getDate() - 1);
            max_date = moment(maxdate).format("DD/MM/YYYY");

            if ($("#asofdate_to_string").val() !== "") {
                if (date_from > date_to) {
                    $("#asofdate_to_string").text(date);
                    $("#asofdate_to_string").val(date);
                    $("#asofdate_from_string").text(date);
                    $("#asofdate_from_string").val(date);
                    return;
                } else {
                    $("#asofdate_from_string").text(date);
                    $("#asofdate_from_string").val(date);

                    if (date_to >= limitDate) {
                        swal("Warning", "Can not select more then 4 month", "warning");
                        $("#asofdate_to_string").text(max_date);
                        $("#asofdate_to_string").val(max_date);
                        return;
                    }
                }
            } else {
                $("#asofdate_to_string").text(date);
                $("#asofdate_to_string").val(date);
                $("#asofdate_from_string").text(date);
                $("#asofdate_from_string").val(date);
            }
        });

    $("#asofdate_to_string").on("dp.change",
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
            if ($("#asofdate_to_string").val() === "") {
                $("#asofdate_max_string").val(max_date);
                $("#asofdate_max_string").text(max_date);
            }
        });
});