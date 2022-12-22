var focusInput = function(input) {
    var center = $(window).height() / 2;
    var top = $(input).offset().top;
    if (top > center) {
        $(window).scrollTop(top - center);
    }
    input.addClass("input-validation-error");
    //input.focus();
};

var sourceType = {
    Id: $('#ddl_source_type'),
    init: function () {
        this.bindEventHandler();
    },
    bindEventHandler: function () {
        var txt_search = $('#txt_source_type');
        var ul_rp_source = $('#ul_rp_source');
        
        this.Id.click(function () {
            var data = { datastr: null };
            GM.Utility.DDLAutoCompleteSet4Value(txt_search, data, "source_type_desc", "source_type_desc", null);
            txt_search.val("");
        });

        txt_search.keyup(function () {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoCompleteSet4Value(txt_search, data, "source_type_desc", "source_type_desc", null);
        });

        ul_rp_source.on("click", ".searchterm", function (event) {
            exchangeType.clearValue();

            if (sourceType.getValue().length > 0) {
                exchangeType.enabled();
            } else {
                exchangeType.disabled();
            }

            if (sourceType.getValue().length) {
                var tmpSourceTypeDesc = '( ' + $('#source_type_desc').val() + ' )';
                $("#source_type_desc").text(tmpSourceTypeDesc);
            } else {
                $("#source_type_desc").text('');
            }
        });
    },
    getValue: function () {
        return $.trim($('#source_type').val());
    },
    validate: function () {
        var errorId = $("#source_type_error");
        if (this.getValue().length) {
            errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        } else {
            errorId.text("Source Type field is required.");
            this.Id.addClass("input-validation-error");
            focusInput(this.Id);
            return false;
        }
    }
};

var exchangeType = {
    Id: $('#ddl_exchange_type'),
    init: function () {
        this.bindEventHandler();
    },
    bindEventHandler: function () {
        var txt_search = $('#txt_exchange_type');
        this.Id.click(function () {
            var data = { datastr: sourceType.getValue() };
            GM.Utility.DDLAutoComplete(txt_search, data, null);
            txt_search.val("");
        });

        txt_search.keyup(function () {

            var data = { datastr: sourceType.getValue() };
            GM.Utility.DDLAutoComplete(this, data, null);

        });
    },
    clearValue: function () {
        this.Id.find(".selected-data").text("Select...");
        this.setValue(null);
        this.setText(null);
    },
    enabled: function () {
        this.Id.removeAttr('disabled');
    },
    disabled: function () {
        this.Id.attr("disabled", "disabled");
    },
    setText: function (text) {
        $('#exchange_desc').val(text);
    },
    setValue: function (value) {
        $('#exchange_type').val(value);
    },
    getValue: function () {
        return $.trim($('#exchange_type').val());
    },
    validate: function () {
        var errorId = $("#exchange_type_error");
        if (this.getValue().length) {
            errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        } else {
            errorId.text("Exchange Type field is required.");
            this.Id.addClass("input-validation-error");
            focusInput(this.Id);
            return false;
        }
    }
};

var asofDate = {
    Id: $('#txt_asof_date'),
    getValue: function () {
        return $.trim(this.Id.val());
    },
    validate: function () {
        var errorId = $("#asof_date_error");
        if (this.getValue().length) {
            errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        } else {
            errorId.text("Asof Date field is required.");
            this.Id.addClass("input-validation-error");
            focusInput(this.Id);
            return false;
        }
    }
};

var exchangeRate = {
    Id: $('#exch_rate'),
    init: function () {

        //this.Id.keyup(function(evt) {
        //    if (this.value > 999.999999) {
        //        this.value = 999.999999;
        //    }
        //});
    },
    getValue: function () {
        return $.trim(this.Id.val());
    },
    validate: function () {
        var errorId = $("#exch_rate_error");

        this.Id.removeClass("input-validation-error");

        var isCur1 = cur1.validate();
        var isCur2 = cur2.validate();

        if (sourceType.getValue() == "KTB" && !this.getValue().length) {
            errorId.text("Rate field is required.");
            this.Id.addClass("input-validation-error");
            focusInput(this.Id);
            return false;
        }
        else if (!isCur1) {
            errorId.text("Currency field is required.");
            return false;
        }
        else if (!isCur2) {
            errorId.text("Currency field is required.");
            return false;
        }
        else {
            errorId.text("");
            return true;
        }
    }
};

var cur1 = {
    Id: $('#ddl_cur1'),
    init: function () {
        this.bindEventHandler();
    },
    bindEventHandler: function () {
        var txt_search = $('#txt_cur1');
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
        return $.trim($('#cur1').val());
    },
    validate: function () {
        var errorId = $("#exch_rate_error");
        if (this.getValue().length) {
            //errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        } else {
            //errorId.text("Currency field is required.");
            this.Id.addClass("input-validation-error");
            focusInput(this.Id);
            return false;
        }
    }
};

var cur2 = {
    Id: $('#ddl_cur2'),
    init: function () {
        this.bindEventHandler();
    },
    bindEventHandler: function () {
        var txt_search = $('#txt_cur2');
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
        return $.trim($('#cur2').val());
    },
    validate: function () {
        var errorId = $("#exch_rate_error");
        if (this.getValue().length) {
            //errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        } else {
            //errorId.text("Currency field is required.");
            this.Id.addClass("input-validation-error");
            focusInput(this.Id);
            return false;
        }
    }
};

var tenor = {
    Id: $('#ddl_tenor'),
    getValue: function () {
        return $.trim($('#tenor').val());
    },
    validate: function () {
        var errorId = $("#tenor_error");
        if (sourceType.getValue() == "SUMMIT" && !this.getValue().length) {
            errorId.text("Tenor field is required.");
            this.Id.addClass("input-validation-error");
            focusInput(this.Id);
            return false;
        } else {
            errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        }
    }
};

var rateAvg = {
    Id: $('#rate_avg'),
    getValue: function () {
        return $.trim($('#rate_avg').val());
    },
    validate: function () {
        var errorId = $("#rate_avg_error");
        if (sourceType.getValue() == "SUMMIT" && !this.getValue().length) {
            errorId.text("AVG Rate field is required.");
            this.Id.addClass("input-validation-error");
            focusInput(this.Id);
            return false;
        } else {
            errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        }
    }
};

var ExchangeRateForm = (function ($) {
    var formId = $('#action-form');
    var initForm = function (urlIndex) {
        sourceType.init();
        exchangeType.init();
        exchangeRate.init();
        cur1.init();
        cur2.init();
        formId.on('submit', function (e) {
            e.preventDefault();
            if (validateSubmit()) {

                var dataToPost = $(this).serialize();
                var action = $(this).attr('action');
                //$.post(action, dataToPost).done(function (response, status, jqxhr) {
                //    // this is the "success" callback
                //    GM.Unmask();

                //    if (response.Success) {
                //        window.location.href = urlIndex;
                //    }
                //    else {
                //        GM.Message.Error('.modal-body', response.Message);
                //    }
                //}).fail(function (jqxhr, status, error) {
                //    });

                $.ajax({
                    type: "POST",
                    url: action,
                    content: "application/json; charset=utf-8",
                    dataType: "json",
                    data: dataToPost,
                    success: function (d) {
                        if (d.Success) {
                            window.location.href = urlIndex;
                        } else {
                            swal("Failed!", "Error : " + d.Message, "error");
                        }
                    },
                    error: function (d) {
                    }
                });

            }
            return;
        });
    };
    var validateSubmit = function () {
        var isValidate = true;
        if (!rateAvg.validate()) {
            isValidate = false;
        }
        if (!tenor.validate()) {
            isValidate = false;
        }
        if (!exchangeRate.validate()) {
            isValidate = false;
        }
        //if (!cur2.validate()) {
        //    isValidate = false;
        //}
        //if (!cur1.validate()) {
        //    isValidate = false;
        //}
        if (!asofDate.validate()) {
            isValidate = false;
        }
        if (!exchangeType.validate()) {
            isValidate = false;
        }
        if (!sourceType.validate()) {
            isValidate = false;
        }
        return isValidate;
    };

    return {
        numberOnlyAndDot: function (obj) {
            obj.value = obj.value
                .replace(/[^\d.]/g, '')             // numbers and decimals only
                .replace(/(^[\d]{3})[\d]/g, '$1')   // not more than 3 digits at the beginning
                .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
                .replace(/(\.[\d]{6})./g, '$1');    // not more than 6 digits after decimal
        },
        auto6digit: function (obj) {
            if (obj.value.length) {
                var nStr = obj.value;
                var x = nStr.split('.');
                var x1 = x[0];
                var x2 = '000000';

                if (x.length > 1) {
                    x2 = x[1];

                    var currentDigit = x[1].length;
                    if (currentDigit < 6) {
                        for (var i = currentDigit; i < 6; i++) {
                            x2 += '0';
                        }
                    }
                }

                if (x1 > 999) {
                    x1 = 999;
                }

                x1 = x1.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                return obj.value = x1 + '.' + x2;
            }
            return "";
        },
        initAdd: function (urlIndex) {
            initForm(urlIndex);
            //disabled
            exchangeType.disabled();
        },
        initEdit: function (urlIndex) {
            initForm(urlIndex);
            //disabled
            sourceType.Id.attr('disabled', 'disabled');
            exchangeType.Id.attr('disabled', 'disabled');
            asofDate.Id.attr('readonly', true);
            cur1.Id.attr('disabled', 'disabled');
            cur2.Id.attr('disabled', 'disabled');
        },
        initView: function () {
            //disabled
            sourceType.Id.attr('disabled', 'disabled');
            exchangeType.Id.attr('disabled', 'disabled');
            asofDate.Id.attr('disabled', 'disabled');
            cur1.Id.attr('disabled', 'disabled');
            cur2.Id.attr('disabled', 'disabled');
            $('#exch_rate').attr('disabled', 'disabled');
            $('#MOS1').attr('disabled', 'disabled');
            $('#MOS2').attr('disabled', 'disabled');
            $('#MOS3').attr('disabled', 'disabled');
            $('#MOS6').attr('disabled', 'disabled');
            $('#MOS9').attr('disabled', 'disabled');
            $('#YR1').attr('disabled', 'disabled');
            $('#YR2').attr('disabled', 'disabled');
            $('#YR3').attr('disabled', 'disabled');
            $('#YR4').attr('disabled', 'disabled');
            $('#YR5').attr('disabled', 'disabled');
            $('#YR6').attr('disabled', 'disabled');
            $('#YR7').attr('disabled', 'disabled');
            $('#YR8').attr('disabled', 'disabled');
            $('#YR9').attr('disabled', 'disabled');
            $('#YR10').attr('disabled', 'disabled');
        }
    };
})(jQuery);