var AdminDashboard = (function ($) {

    var alertMsg = {
        Id: $('#alert-msg'),
        clearMessage: function() {
            this.Id.html();
        },
        setSuccess: function (msg) {
            var html = "<div class='alert alert-success alert-dismissible' role='alert'> "+
                "<button type='button' class='close' data-dismiss='alert' aria-label='Close'> " +
                "<span aria-hidden='true'>&times;</span></button>" + msg +
                "</div>";
            this.Id.html(html);
        },
        setError: function (msg) {
            var html = "<div class='alert alert-danger alert-dismissible' role='alert'> " +
                "<button type='button' class='close' data-dismiss='alert' aria-label='Close'> " +
                "<span aria-hidden='true'>&times;</span></button>" + msg +
                "</div>";
            this.Id.html(html);
        }
    };

    var asOfDate = {
        Id: $('#asof_date'),
        init: function() {
            this.bindEventHandler();
        },
        bindEventHandler: function () {
            var businessDate = $('#BusinessDate').text();

            this.Id.text(businessDate);
            this.Id.val(businessDate);

            //this.Id.text(moment().format('DD/MM/YYYY'));
            //this.Id.val(moment().format('DD/MM/YYYY'));
        },
        getValue: function() {
            return this.Id.val();
        }
        
    };

    var requestRerun = function (urlService, param) {
        alertMsg.clearMessage();
        $('.spinner').css('display', 'block');

        $.ajax({
            url: urlService,
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            cache: false,
            dataType: 'JSON',
            traditional: true,
                data: JSON.stringify(param),
            success: function(data) {
                $('.spinner').css('display', 'none');

                if (data.Status == "Success") {
                    alertMsg.setSuccess(data.Message);
                }
                else if (data.Status == "Error") {
                    alertMsg.setError(data.Message);
                }

            },
            error: function(jqXhr, textStatus) {
                $('.spinner').css('display', 'none');

                if (textStatus === "error") {
                    var objJson = jQuery.parseJSON(jqXhr.responseText);

                    if (Object.prototype.toString.call(objJson) === '[object Array]' &&
                        objJson.length == 0) {
                        // Array is empty
                        // Do Something
                    } else {
                        var errorMsg = jqXhr.statusText + " " + objJson.Message;
                        alert("An error occurred, " + errorMsg,
                            function(e) {

                            },
                            {
                                ok: "OK",
                                classname: "custom-class"
                            });
                    }
                }
            }
        });
    };

    return {
        init: function() {
            asOfDate.init();
        },
        requestRerun: function (urlService) {
            swal({
                title: "Are you sure?",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-danger",
                confirmButtonText: "Yes",
                cancelButtonText: "No",
                closeOnConfirm: true,
                closeOnCancel: true
            }, function (isConfirm) {
                if (isConfirm) {
                    var param = {
                        asof_date: asOfDate.getValue()
                    };

                    requestRerun(urlService, param);
                }
            });
        }
    };
})(jQuery);