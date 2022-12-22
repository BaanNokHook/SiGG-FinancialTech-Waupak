$.ajaxSetup({
    cache: false
});

$(document).ready(function () {

    $("#NavBar").html($('#NavActivity').val());

    var budate = $("#BusinessDate").text();

    var formatmmddyyyydate = budate.split("/");
    formatmmddyyyydate = formatmmddyyyydate[1] + "/" + formatmmddyyyydate[0] + "/" + formatmmddyyyydate[2];
    var business_date = new Date(formatmmddyyyydate);

    $('#date_from').data("DateTimePicker").date(business_date);
    $('#date_to').data("DateTimePicker").date(business_date);

    //Function : Binding Table
    GM.Activity = {};
    GM.Activity.Export = function (btn) {
        swal({
            title: "Do you want to Export Activity?",
            text: "",
            html: true,
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes",
            cancelButtonText: "No",
            closeOnConfirm: true,
            closeOnCancel: true
        },
            function (isConfirm) {
                if (isConfirm) {

                    $('.spinner').css('display', 'block');

                    $.post('/Activity/ExportActivity', { date_from: $('#date_from').val(), date_to: $('#date_to').val()})
                        .done(function (response) {
                            if (response.errorMessage === '') {
                                window.location.href = '/Activity/Download?filename=' + response.fileName;
                                $('.spinner').css('display', 'none');
                                setTimeout(function () {

                                    swal({
                                        title: "Complete",
                                        text: "Gen Excel Successfully ",
                                        type: "success",
                                        showCancelButton: false,
                                        confirmButtonClass: "btn-success",
                                        confirmButtonText: "Ok",
                                        closeOnConfirm: true
                                    }
                                    );
                                }, 100);
                            } else {
                                setTimeout(function () {
                                    swal("Warning", response.errorMessage, "warning");
                                }, 100);
                                $('.spinner').css('display', 'none');
                            }
                        });
                }
            }
        )
    };
});