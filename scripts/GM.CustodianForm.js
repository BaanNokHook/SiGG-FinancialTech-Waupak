var custodianCode = {
    Id: $('#custodian_code'),
    getValue: function () {
        return $.trim(this.Id.val());
    },
    validate: function () {
        var errorId = $("#custodian_code_error");
        if (this.getValue().length) {
            errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        }
        else {
            errorId.text("Custodian Code field is required.");
            this.Id.addClass("input-validation-error");
            return false;
        }
    }
};

var custodianShortname = {
    Id: $('#custodian_shortname'),
    getValue: function () {
        return $.trim(this.Id.val());
    },
    validate: function () {
        var errorId = $("#custodian_shortname_error");
        if (this.getValue().length) {
            errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        }
        else {
            errorId.text("Custodian Short Name field is required.");
            this.Id.addClass("input-validation-error");
            return false;
        }
    }
};

var custodianName = {
    Id: $('#custodian_name'),
    getValue: function () {
        return $.trim(this.Id.val());
    },
    validate: function () {
        var errorId = $("#custodian_name_error");
        if (this.getValue().length) {
            errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        }
        else {
            errorId.text("Custodian Name field is required.");
            this.Id.addClass("input-validation-error");
            return false;
        }
    }
};

var province = {
    Id: $('#ddl_province'),
    init: function () {
        this.bindEventHandler();
    },
    bindEventHandler: function () {
        var txt_search = $('#txt_province');
        this.Id.click(function () {
            var data = { datastr: null };
            GM.Utility.DDLAutoComplete(txt_search, data, null);
            txt_search.val("");
        });

        $("#ul_province").on("click", ".searchterm", function (event) {
            district.clearValue();

            if (province.getValue().length > 0) {
                district.enabled();
            }
            else {
                district.disabled();
            }

            subDistrict.clearValue();
            subDistrict.disabled();

            postcode.setValue(null);
        });

        txt_search.keyup(function () {

            //if (this.value.length > 0) {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
            //}
            //else if (this.value.length == 0) {
            //    var data = { datastr: null };
            //    GM.Utility.DDLAutoComplete(this, data, null);
            //}
        });

    },
    disabled: function () {
        this.Id.attr("disabled", "disabled");
    },
    getValue: function () {
        return $.trim($('#province_id').val());
    },
    validate: function () {
        var errorId = $("#province_error");
        if (this.getValue().length) {
            errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        }
        else {
            errorId.text("Source Type field is required.");
            this.Id.addClass("input-validation-error");
            return false;
        }
    }
};

var district = {
    Id: $('#ddl_district'),
    init: function () {
        this.bindEventHandler();
    },
    bindEventHandler: function () {
        var txt_search = $('#txt_district');
        this.Id.click(function () {
            if (province.getValue().length > 0) {
                var data = { dataint: province.getValue(), datastr: null };
                GM.Utility.DDLAutoComplete(txt_search, data, "post_code");
                txt_search.val("");

                subDistrict.enabled();
            }
        });

        $("#ul_district").on("click", ".searchterm", function (event) {
            subDistrict.clearValue();
            if (district.getValue().length > 0) {
                subDistrict.enabled();
            }
            else {
                subDistrict.disabled();
                postcode.setValue(null);
            }
        });

        txt_search.keyup(function () {
            var data = { dataint: province.getValue(), datastr: this.value };
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
        $('#district_name').val(text);
    },
    setValue: function (value) {
        $('#district_id').val(value);
    },
    getValue: function () {
        return $.trim($('#district_id').val());
    },
    validate: function () {
        var errorId = $("#district_error");
        if (this.getValue().length) {
            errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        }
        else {
            errorId.text("Source Type field is required.");
            this.Id.addClass("input-validation-error");
            return false;
        }
    }
};

var subDistrict = {
    Id: $('#ddl_sub_district'),
    init: function () {
        this.bindEventHandler();
    },
    bindEventHandler: function () {
        var txt_search = $('#txt_sub_district');
        this.Id.click(function () {
            if (district.getValue().length > 0) {
                var data = { dataint: district.getValue(), datastr: null };
                GM.Utility.DDLAutoComplete(txt_search, data, null);
                txt_search.val("");
            }
        });

        txt_search.keyup(function () {

            //if (this.value.length > 0) {
            var data = { dataint: province.getValue(), datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
            //}
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
        this.Id.attr('disabled', 'disabled');
    },
    setText: function (text) {
        $('#sub_district_name').val(text);
    },
    setValue: function (value) {
        $('#sub_district_id').val(value);
    },
    getValue: function () {
        return $.trim($('#sub_district_id').val());
    },
    validate: function () {
        var errorId = $("#sub_district_error");
        if (this.getValue().length) {
            errorId.text("");
            this.Id.removeClass("input-validation-error");
            return true;
        }
        else {
            errorId.text("Source Type field is required.");
            this.Id.addClass("input-validation-error");
            return false;
        }
    }
};

var postcode = {
    Id: $('#post_code'),
    enabled: function () {
        this.Id.removeAttr('disabled');
    },
    disabled: function () {
        this.Id.attr('readonly', 'readonly');
    },
    setValue: function (value) {
        this.Id.val(value);
    }
};

var detailForm = {
    Id: $('#custodian-detail'),
    init: function() {
        this.Id.click(function() {
            var expand = $("div#custodian-detail-icon").attr("aria-expanded");
            if (expand == "true") {
                $("#custodian-detail-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
            } else {
                $("#custodian-detail-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
            }
        });
    }
};

var addressForm = {
    Id: $('#custodian-address'),
    init: function() {
        this.Id.click(function() {
            var expand = $("div#custodian-address-icon").attr("aria-expanded");
            if (expand == "true") {
                $("#custodian-address-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
            } else {
                $("#custodian-address-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
            }
        });
    }
};

var CustodianForm = (function ($) {
    var formId = $('#action-form');
    var initForm = function (urlIndex) {
        province.init();
        district.init();
        subDistrict.init();
        formId.on('submit', function (e) {
            e.preventDefault();
            if (validateSubmit()) {

                var dataToPost = $(this).serialize();
                console.log(dataToPost);
                var action = $(this).attr('action');
                $.post(action, dataToPost).done(function (response, status, jqxhr) {
                    // this is the "success" callback
                    GM.Unmask();

                    if (response.Success) {
                        window.location.href = urlIndex;
                    }
                    else {
                        swal("Error", response.Message, "error");
                    }
                }).fail(function (jqxhr, status, error) {
                });

            }
            return;
        });
    };
    var validateSubmit = function() {
        var isValidate = true;

        if (!custodianCode.validate()) {
            isValidate = false;
        }
        if (!custodianShortname.validate()) {
            isValidate = false;
        }
        if (!custodianName.validate()) {
            isValidate = false;
        }
        return isValidate;
    };
    var expandPanel = function(panelname) {
        if (panelname == "custodian-detail") {
            $("#custodian-detail-form").removeAttr("style");

            $("#custodian-detail-form").attr("class", "form-container form-horizontal have-head collapse in");
            $("#custodian-detail-form").attr("aria-expanded", "true");

            $("#custodian-detail-icon").attr("aria-expanded", "true");
            $("#custodian-detail-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
            $("#custodian-detail-icon").attr("class", "box-header big-head expand-able");

            $("#detail-li").attr("class", "active");
            $("#address-li").removeAttr("class");
        } else if (panelname == "custodian-address") {
            $("#custodian-address-form").removeAttr("style");

            $("#custodian-address-form").attr("class", "form-container form-horizontal have-head collapse in");
            $("#custodian-address-form").attr("aria-expanded", "true");

            $("#custodian-address-icon").attr("aria-expanded", "true");
            $("#custodian-address-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
            $("#custodian-address-icon").attr("class", "box-header big-head expand-able");

            $("#detail-li").removeAttr("class");
            $("#address-li").attr("class", "active");
        }
    };

    return {
        expandPanel: function(panelName) {
            expandPanel(panelName);
        },
        initAdd: function(urlIndex) {
            initForm(urlIndex);
            //disabled
            district.disabled();
            subDistrict.disabled();
            postcode.disabled();

            detailForm.init();
            addressForm.init();
        },
        initEdit: function(urlIndex) {
            initForm(urlIndex);
            //disabled
            if (district.getValue().length == 0) {
                district.disabled();
            }

            if (subDistrict.getValue().length == 0) {
                subDistrict.disabled();
            }

            postcode.disabled();

            detailForm.init();
            addressForm.init();
        },
        initView: function() {
            //disabled
            custodianCode.Id.attr('disabled', 'disabled');
            custodianShortname.Id.attr('disabled', 'disabled');
            custodianName.Id.attr('disabled', 'disabled');
            $('#swift_code').attr('disabled', 'disabled');
            $('#sa_acc_no').attr('disabled', 'disabled');
            $('#ca_acc_no').attr('disabled', 'disabled');
            $('#contact').attr('disabled', 'disabled');
            $('#address1').attr('disabled', 'disabled');
            $('#address2').attr('disabled', 'disabled');
            $('#address3').attr('disabled', 'disabled');
            $('#address4').attr('disabled', 'disabled');
            $('#post_code').attr('disabled', 'disabled');
            $('#tel_no').attr('disabled', 'disabled');
            $('#fax_no').attr('disabled', 'disabled');
            province.disabled();
            district.disabled();
            subDistrict.disabled();

            detailForm.init();
            addressForm.init();
        }
    };
})(jQuery);
