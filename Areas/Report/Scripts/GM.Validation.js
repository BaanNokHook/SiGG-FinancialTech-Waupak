$(function() {
    var input = $(".input-validation-error:first");
    if (input) {
        input.focus();
        input.click();
    }
});

$(document).ready(function() {

    $("#trade_date_from_string").on("dp.change",
        function(e) {
            if (e.date) {
                var date = moment(e.date).format("DD/MM/YYYY");
                if ($("#trade_date_to_string").text() != "") {
                    var date_from = setformatdateyyyymmdd(moment(e.date).format("DD/MM/YYYY"));
                    var date_to = setformatdateyyyymmdd($("#trade_date_to_string").text());
                    if (date_from > date_to) {
                        $("#trade_date_to_string").text(date);
                        $("#trade_date_to_string").val(date);
                        $("#trade_date_from_string").text(date);
                        $("#trade_date_from_string").val(date);
                    } else {
                        $("#trade_date_from_string").text(date);
                        $("#trade_date_from_string").val(date);
                    }
                } else {
                    $("#trade_date_from_string").text(date);
                    $("#trade_date_from_string").val(date);
                    $("#trade_date_to_string").text(date);
                    $("#trade_date_to_string").val(date);
                }
            }
        });

    $("#trade_date_to_string").on("dp.change",
        function(e) {
            if (e.date) {
                var date = moment(e.date).format("DD/MM/YYYY");
                $("#trade_date_to_string").text(date);
                $("#trade_date_to_string").val(date);
            }
        });

    $("#settlement_date_from_string").on("dp.change",
        function(e) {
            if (e.date) {
                var date = moment(e.date).format("DD/MM/YYYY");
                if ($("#settlement_date_to_string").text() != "") {
                    var date_from = setformatdateyyyymmdd(moment(e.date).format("DD/MM/YYYY"));
                    var date_to = setformatdateyyyymmdd($("#settlement_date_to_string").text());
                    if (date_from > date_to) {
                        $("#settlement_date_to_string").text(date);
                        $("#settlement_date_to_string").val(date);
                        $("#settlement_date_from_string").text(date);
                        $("#settlement_date_from_string").val(date);
                    } else {

                        $("#settlement_date_from_string").text(date);
                        $("#settlement_date_from_string").val(date);
                    }
                } else {
                    $("#settlement_date_from_string").text(date);
                    $("#settlement_date_from_string").val(date);
                    $("#settlement_date_to_string").text(date);
                    $("#settlement_date_to_string").val(date);
                }
            }
        });

    $("#settlement_date_to_string").on("dp.change",
        function(e) {
            if (e.date) {
                var date = moment(e.date).format("DD/MM/YYYY");
                $("#settlement_date_to_string").text(date);
                $("#settlement_date_to_string").val(date);
            }
        });

    $("#maturity_date_from_string").on("dp.change",
        function(e) {
            if (e.date) {
                var date = moment(e.date).format("DD/MM/YYYY");
                if ($("#maturity_date_to_string").text() != "") {
                    var date_from = setformatdateyyyymmdd(moment(e.date).format("DD/MM/YYYY"));
                    var date_to = setformatdateyyyymmdd($("#maturity_date_to_string").text());
                    if (date_from > date_to) {
                        $("#maturity_date_to_string").text(date);
                        $("#maturity_date_to_string").val(date);
                        $("#maturity_date_from_string").text(date);
                        $("#maturity_date_from_string").val(date);
                    } else {

                        $("#maturity_date_from_string").text(date);
                        $("#maturity_date_from_string").val(date);
                    }
                } else {
                    $("#maturity_date_from_string").text(date);
                    $("#maturity_date_from_string").val(date);
                    $("#maturity_date_to_string").text(date);
                    $("#maturity_date_to_string").val(date);
                }
            }
        });

    $("#maturity_date_to_string").on("dp.change",
        function(e) {
            if (e.date) {
                var date = moment(e.date).format("DD/MM/YYYY");
                $("#maturity_date_to_string").text(date);
                $("#maturity_date_to_string").val(date);
            }
        });

    $("#asofdate_from_string").on("dp.change",
        function(e) {
            if (e.date) {
                var date = moment(e.date).format("DD/MM/YYYY");
                if ($("#asofdate_to_string").text() != "") {
                    var date_from = setformatdateyyyymmdd(moment(e.date).format("DD/MM/YYYY"));
                    var date_to = setformatdateyyyymmdd($("#asofdate_to_string").text());
                    if (date_from > date_to) {
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
        });

    $("#asofdate_to_string").on("dp.change",
        function(e) {
            if (e.date) {
                var date = moment(e.date).format("DD/MM/YYYY");
                $("#asofdate_to_string").text(date);
                $("#asofdate_to_string").val(date);
            }
        });

    $("#call_date_from_string").on("dp.change",
        function(e) {
            if (e.date) {
                var date = moment(e.date).format("DD/MM/YYYY");
                if ($("#call_date_to_string").text() != "") {
                    var date_from = setformatdateyyyymmdd(moment(e.date).format("DD/MM/YYYY"));
                    var date_to = setformatdateyyyymmdd($("#call_date_to_string").text());
                    if (date_from > date_to) {
                        $("#call_date_to_string").text(date);
                        $("#call_date_to_string").val(date);
                        $("#call_date_from_string").text(date);
                        $("#call_date_from_string").val(date);
                    } else {

                        $("#call_date_from_string").text(date);
                        $("#call_date_from_string").val(date);
                    }
                } else {
                    $("#call_date_from_string").text(date);
                    $("#call_date_from_string").val(date);
                    $("#call_date_to_string").text(date);
                    $("#call_date_to_string").val(date);
                }
            }
        });

    $("#call_date_to_string").on("dp.change",
        function(e) {
            if (e.date) {
                var date = moment(e.date).format("DD/MM/YYYY");
                $("#call_date_to_string").text(date);
                $("#call_date_to_string").val(date);
            }
        });

    $("#from_date_string").on("dp.change",
        function(e) {
            if (e.date) {
                var date = moment(e.date).format("DD/MM/YYYY");
                if ($("#to_date_string").text() != "") {
                    var date_from = setformatdateyyyymmdd(moment(e.date).format("DD/MM/YYYY"));
                    var date_to = setformatdateyyyymmdd($("#to_date_string").text());
                    if (date_from > date_to) {
                        $("#to_date_string").text(date);
                        $("#to_date_string").val(date);
                        $("#from_date_string").text(date);
                        $("#from_date_string").val(date);
                    } else {

                        $("#from_date_string").text(date);
                        $("#from_date_string").val(date);
                    }
                } else {
                    $("#from_date_string").text(date);
                    $("#from_date_string").val(date);
                    $("#to_date_string").text(date);
                    $("#to_date_string").val(date);
                }
            }
        });

    $("#to_date_string").on("dp.change",
        function(e) {
            if (e.date) {
                var date = moment(e.date).format("DD/MM/YYYY");
                $("#to_date_string").text(date);
                $("#to_date_string").val(date);
            }
        });

    $("#start_date_string").on("dp.change",
        function(e) {
            if (e.date) {
                var date = moment(e.date).format("DD/MM/YYYY");
                if ($("#expire_date_string").text() != "") {
                    var date_from = setformatdateyyyymmdd(moment(e.date).format("DD/MM/YYYY"));
                    var date_to = setformatdateyyyymmdd($("#expire_date_string").text());
                    if (date_from > date_to) {
                        $("#expire_date_string").text(date);
                        $("#expire_date_string").val(date);
                        $("#start_date_string").text(date);
                        $("#start_date_string").val(date);
                    } else {

                        $("#start_date_string").text(date);
                        $("#start_date_string").val(date);
                    }
                } else {
                    $("#start_date_string").text(date);
                    $("#start_date_string").val(date);
                    $("#expire_date_string").text(date);
                    $("#expire_date_string").val(date);
                }
            }
        });

    $("#expire_date_string").on("dp.change",
        function(e) {
            if (e.date) {
                var date = moment(e.date).format("DD/MM/YYYY");
                $("#expire_date_string").text(date);
                $("#expire_date_string").val(date);
            }
        });

});