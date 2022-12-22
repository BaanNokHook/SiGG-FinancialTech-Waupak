$(window).on('load', function () {
    var businessDate = $("#BusinessDate").text();
    var formatmmddyyyydate = businessDate.split("/");
    formatmmddyyyydate = formatmmddyyyydate[1] + "/" + formatmmddyyyydate[0] + "/" + formatmmddyyyydate[2];
    var tmpBusinessDate = new Date(formatmmddyyyydate);

    //$('#from_date').data("DateTimePicker").date(tmpBusinessDate);
    //$('#to_date').data("DateTimePicker").date(tmpBusinessDate);

    $('#from_date').val(businessDate);
    $('#to_date').val(businessDate);

    $("#ddl_cur").find(".selected-data").text("THB");
    $('#cur').val("THB");
});

function Validate() {
    var IsValid = true;
    var message = '';
    var from_date = $('#from_date');
    if (from_date.val() == '') {
        IsValid = false;
        message += '<li style="text-align:left; color:red;">The From Date field is required.</li>';
    }
    var to_date = $('#to_date');
    if (to_date.val() == '') {
        IsValid = false;
        message += '<li style="text-align:left; color:red;">The To Date field is required.</li>';
    }

    //var event_generate = $('#event_generate');
    //if (event_generate.val() == '') {
    //    IsValid = false;
    //    message += '<li style="text-align:left; color:red;">The Event field is required.</li>';
    //}

    if (message !== '') {
        swal({
            title: "warning",
            text: message,
            html: true,
            type: "warning",
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes",
            closeOnConfirm: true
        });
    }

    return IsValid;
}

function SetDDL(element, value) {
    $("#" + element).find(".selected-data").text(value);
    $("#" + element).find(".selected-data").val(value);
    $("#" + element).find(".selected-value").val(value);
}

$(document).ready(function () {

    $('form').on('reset', function (e) {
        $('.spinner').css('display', 'block'); // Open Loading
        location.href = window.location.href;
    });

    //$("#ddl_cur").click(function () {
    //    var txt_search = $('#txt_cur');
    //    var data = { datastr: null };
    //    GM.Utility.DDLAutoComplete(txt_search, data, null, false);
    //    txt_search.val("");
    //});

    //$('#txt_cur').keyup(function () {
    //    var data = { datastr: this.value };
    //    GM.Utility.DDLAutoComplete(this, data, null);
    //});

    $("#ddl_event").click(function () {
        var txt_search = $('#txt_event');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_event').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    GM.GLGenerate = {};
    GM.GLGenerate.Submit = function (btn) {
        if (Validate()) {
            swal({
                title: "Comfirm Generate?",
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
                        setTimeout(
                            function () {
                                try {
                                    $('.spinner').css('display', 'block'); // Open Loading
                                    var dataToPost = $("#search-form").serialize();
                                    $.post("GLGenerate/Generate", dataToPost)
                                        .done(function (response) {
                                            $('.spinner').css('display', 'none'); // Close Loading
                                            if (response.Success) {
                                                swal({
                                                    title: "Complete",
                                                    text: "Save Successfully ",
                                                    type: "success",
                                                    showCancelButton: false,
                                                    confirmButtonClass: "btn-success",
                                                    confirmButtonText: "Ok",
                                                    closeOnConfirm: true
                                                });
                                            } else {
                                                setTimeout(
                                                    function () {
                                                        swal("Fail", response.Message, "error");
                                                    }, 300);
                                                console.log(response);
                                            }
                                        });
                                } catch (e) {
                                    console.log('Error : ' + e.message);
                                    $('.spinner').css('display', 'none'); // Close Loading
                                }
                            }, 300);
                    }
                });
        }
    };





});