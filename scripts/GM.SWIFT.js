var SWIFT = (function ($) {

    var formId = $('#SWIFT-form');

    var activePath = {
        Id: $('#ddl_active_path'),
        init: function () {
            this.bindEventHandler();
        },
        bindEventHandler: function () {
            $('#ul_active_path').on("click", ".searchterm", function (event) {
                //importDesc.clearText();
                //importDesc.setText(activePath.getValue());
            });
        },
        getValue: function () {
            return $('#ActivePath').val();
        },
        validate: function () {
            var errorId = $("#active_path_error");
            if (this.getValue().length) {
                errorId.text("");
                this.Id.removeClass("input-validation-error");
                return true;
            } else {
                errorId.text("Active Path field is required.");
                this.Id.addClass("input-validation-error");
                return false;
            }
        }
    };

    var fileName = {
        Id: $('#file_name'),
        getValue: function () {
            return $.trim($('#file_name').val());
        }
    };

    var result = {
        Id: $('#result'),
        setValue: function (value) {
            this.Id.val(value);
        }
    };

    return {
        init: function (url) {
            activePath.init();

            $('#btnSftp').on('click', function (e) {
                e.preventDefault();
                $('.spinner').css('display', 'block');

                var dataToPost = {
                    file_name: fileName.getValue(),
                    active_path: activePath.getValue()
                };

                $.ajax({
                    type: "POST",
                    url: url,
                    content: "application/json; charset=utf-8",
                    dataType: "json",
                    data: dataToPost,
                    success: function (d) {
                        $('.spinner').css('display', 'none');
                        if (d.Success) {
                            result.setValue(d.Result);

                        } else {
                            swal("Failed!", "Error : " + d.Message, "error");
                        }
                    },
                    error: function (d) {
                    }
                });
            });

            $('#btnback').on('click', function (e) {
                e.preventDefault();
                alert('btnback');
            });
        }
    };
})(jQuery);