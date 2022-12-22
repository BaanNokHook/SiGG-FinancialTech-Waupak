var DMS = (function ($) {
    var table = "";

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

    var searchDate = {
        Id: $('#SearchDate'),
        init: function () {
            this.bindEventHandler();
        },
        bindEventHandler: function () {
            this.Id.text(moment().format('DD/MM/YYYY'));
            this.Id.val(moment().format('DD/MM/YYYY'));
        },
        getValue: function () {
            return this.Id.val();
        }
    };

    var dmsType = {
        Id: $('#DmsType'),
        init: function() {
            
        },
        getValue: function() {
            return this.Id.val();
        },
        setValue: function(value) {
            this.Id.val(value);
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
            searchDate.init();
            dmsType.setValue("FCP");
            this.search(urlService);
        },
        search: function (urlService) {
            dataTable.clearData();
            $('.spinner').css('display', 'block');

            var postData = {
                SearchDate: searchDate.getValue(),
                DmsType: dmsType.getValue()
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
        exportExcel: function (urlService) {
            var formId = $('#search-form');
            formId.attr('action', urlService);
            formId.submit();
        }
    };

})(jQuery);