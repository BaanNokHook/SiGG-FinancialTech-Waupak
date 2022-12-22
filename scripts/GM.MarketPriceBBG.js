var Market = (function ($) {
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

    var cur = {
        Id: $('#ddl_cur'),
        init: function () {
            this.bindEventHandler();
        },
        bindEventHandler: function () {
            var txt_search = $('#txt_cur');
            this.Id.click(function () {
                var data = { datastr: null };
                GM.Utility.DDLAutoComplete(txt_search, data, null);
                txt_search.val("");
            });

            txt_search.keyup(function () {

                var data = { datastr: this.value };
                GM.Utility.DDLAutoComplete(this, data, null);

            });
        },
        getValue: function () {
            return $.trim($('#cur').val());
        }
    };

    var security_code = {
        Id: $('#security_code'),
        getValue: function () {
            return $.trim($('#security_code').val());
        },
        validate: function () {
            var errorId = $("#security_code_error");
            if (this.getValue().length) {
                errorId.text("");
                this.Id.removeClass("input-validation-error");
                return true;
            } else {
                errorId.text("Security Code field is required.");
                this.Id.addClass("input-validation-error");
                Focus(this.Id);
                return false;
            }
        }
    };

    var asofDate = {
        Id: $('#as_of_date'),
        init: function () {
            this.bindEventHandler();
        },
        bindEventHandler: function () {
            var businessDate = $('#BusinessDate').text();

            this.Id.text(businessDate);
            this.Id.val(businessDate);
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
            cur.init();
            asofDate.init();
        },
        Search: function (urlService) {

            dataTable.clearData();
            $('.spinner').css('display', 'block');

            var postData = {
                security_code: security_code.getValue(),
                cur: cur.getValue(),
                as_of_date: asofDate.getValue()
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
        Request: function (urlService, urlSearch) {
            if (!security_code.validate()) {
                return;
            }

            swal({
                title: "Please confirm to continue?",
                text: "",
                html: true,
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-danger",
                confirmButtonText: "Yes",
                cancelButtonText: "No",
                closeOnConfirm: true,
                closeOnCancel: true
            }, function (isConfirm) {
                if (isConfirm) {

                    $('.spinner').css('display', 'block');

                    var postData = {
                        security_code: security_code.getValue(),
                        cur: cur.getValue(),
                        as_of_date: asofDate.getValue()
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

                                //#region Search
                                dataTable.clearData();
                                $('.spinner').css('display', 'block');

                                var postData = {
                                    security_code: security_code.getValue(),
                                    cur: cur.getValue(),
                                    as_of_date: asofDate.getValue()
                                };

                                $.ajax({
                                    url: urlSearch,
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
                                //#endregion

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
                }
            });
        }
    };
})(jQuery);