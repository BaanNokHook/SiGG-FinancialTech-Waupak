var InternalPL = (function ($) {
    var table = "";

    var budate = $("#BusinessDate").text();

    var alertMsg = {
        Id: $('#alert-msg'),
        clearMessage: function () {
            this.Id.html();
        },
        setSuccess: function (msg) {
            var html = "<div class='alert alert-success alert-dismissible' role='alert'> " +
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

    var asOfDateFrom = {
        Id: $('#asof_date_from'),
        init: function () {
            this.bindEventHandler();
        },
        bindEventHandler: function () {
            //business date
            this.Id.text(budate);
            this.Id.val(budate);
        },
        getValue: function () {
            return this.Id.val();
        },
        validate: function () {
            if (this.getValue().length) {
                var date_from = setformatdateyyyymmdd(asOfDateFrom.getValue());
                var date_to = setformatdateyyyymmdd(asOfDateTo.getValue());
                var bDate = setformatdateyyyymmdd(budate);
                if (date_from > date_to) {
                    swal("Failed!", "As Of Date From more than Date To", "error");
                    return false;
                }
                else if (bDate > date_from) {
                    swal("Failed!", "As Of Date From more than Business Date", "error");
                    return false;
                }
                return true;
            } else {
                swal("Failed!", "Require As Of Date From", "error");
                return false;
            }
        }
    };

    var asOfDateTo = {
        Id: $('#asof_date_to'),
        init: function () {
            this.bindEventHandler();
        },
        bindEventHandler: function () {
            //business date
            this.Id.text(budate);
            this.Id.val(budate);
        },
        getValue: function () {
            return this.Id.val();
        }
    };

    var dataTable = {
        Id: $('#dataTable'),
        init: function () {

        },
        clearData: function () {
            if ($.fn.dataTable.isDataTable('#dataTable')) {
                table.destroy();
                dataTable.Id.empty();
            }
        },
        reload: function (data) {
            if (data.Columns.length) {
                table = dataTable.Id.DataTable({
                    scrollY: '80vh',
                    scrollX: true,
                    processing: true,
                    "data": data.Data,
                    "columns": data.Columns,
                    paging: true
                });
            }
        }
    };

    return {
        init: function (urlService) {
            asOfDateFrom.init();
            asOfDateTo.init();
            this.search(urlService);
        },
        search: function (urlService) {
            dataTable.clearData();
            $('.spinner').css('display', 'block');

            var postData = {
                asof_date_from: asOfDateFrom.getValue(),
                asof_date_to: asOfDateTo.getValue()
            };

            $.ajax({
                url: urlService,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'JSON',
                traditional: true,
                data: JSON.stringify(postData),
                success: function (data) {
                    $('.spinner').css('display', 'none');

                    if (data.Status == "Success") {
                        alertMsg.setSuccess(data.Message);
                        dataTable.reload(data);
                    }
                    else if (data.Status == "Error") {
                        alertMsg.setError(data.Message);
                    }
                },
                error: function (jqXhr, textStatus) {
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
                                function (e) {

                                },
                                {
                                    ok: "OK",
                                    classname: "custom-class"
                                });
                        }
                    }
                }
            });
        },
        reRun: function (urlService) {
            $('.spinner').css('display', 'block');

            //validate
            if (!asOfDateFrom.validate()) {
                $('.spinner').css('display', 'none');
                return false;
            }

            var postData = {
                asof_date_from: asOfDateFrom.getValue(),
                asof_date_to: asOfDateTo.getValue()
            };

            $.ajax({
                type: "POST",
                url: urlService,
                contentType: 'application/json; charset=utf-8',
                cache: false,
                dataType: 'JSON',
                traditional: true,
                data: JSON.stringify(postData),
                success: function (d) {
                    $('.spinner').css('display', 'none');
                    if (d.Success) {
                        swal("Success!", "ReRun Success.", "success");
                    } else {
                        swal("Failed!", "Error : " + d.Message, "error");
                    }
                },
                error: function (d) {
                    $('.spinner').css('display', 'none');
                }
            });
        }
    };
})(jQuery);