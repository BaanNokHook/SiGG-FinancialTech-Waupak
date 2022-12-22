function Setstatus(datatext) {
    $('#statusdata').val(datatext);

    if (datatext == "Approve") {
        $("#newdata").attr("style", "display: none");
        $("#approve").attr("style", "display: show");
        $("#unapprove").attr("style", "display: none");
        $("#unactivedata").attr("style", "display: none");

    }
    else if (datatext == "UnApprove") {
        $("#newdata").attr("style", "display: none");
        $("#approve").attr("style", "display: none");
        $("#unapprove").attr("style", "display: show");
        $("#unactivedata").attr("style", "display: none");
    }
    else if (datatext == "UnActive") {
        $("#newdata").attr("style", "display: none");
        $("#approve").attr("style", "display: none");
        $("#unapprove").attr("style", "display: none");
        $("#unactivedata").attr("style", "display: show");
    }
}
function expandpanel(panelname) {
    if (panelname == "detail") {
        $("#issuer-detail-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#issuer-detail-form").attr("aria-expanded", "true");
        $("#issuer-detail-form").removeAttr("style");
        $("#issuer-detail-icon").attr("aria-expanded", "true");
        $("#issuer-detail-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#issuer-detail-icon").attr("class", "box-header big-head expand-able");
        $("#detail-li").attr("class", "active");
        $("#address-li").removeAttr("class");
        $("#taxandother-li").removeAttr("class");
        $("#banklist-li").removeAttr("class");
        $("#identify-li").removeAttr("class");
        $("#rating-li").removeAttr("class");
        $("#margin-li").removeAttr("class");
    }
    else if (panelname == "address") {
        $("#issuer-address-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#issuer-address-form").attr("aria-expanded", "true");
        $("#issuer-address-form").removeAttr("style");
        $("#issuer-address-icon").attr("aria-expanded", "true");
        $("#issuer-address-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#issuer-address-icon").attr("class", "box-header big-head expand-able");

        $("#detail-li").removeAttr("class");
        $("#address-li").attr("class", "active");
        $("#taxandother-li").removeAttr("class");
        $("#banklist-li").removeAttr("class");
        $("#identify-li").removeAttr("class");
        $("#rating-li").removeAttr("class");
        $("#margin-li").removeAttr("class");
    }
    else if (panelname == "taxandother") {
        $("#issuer-taxandother-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#issuer-taxandother-form").attr("aria-expanded", "true");
        $("#issuer-taxandother-form").removeAttr("style");
        $("#issuer-taxandother-icon").attr("aria-expanded", "true");
        $("#issuer-taxandother-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#issuer-taxandother-icon").attr("class", "box-header big-head expand-able");
        $("#detail-li").removeAttr("class");
        $("#address-li").removeAttr("class");
        $("#taxandother-li").attr("class", "active");
        $("#banklist-li").removeAttr("class");
        $("#identify-li").removeAttr("class");
        $("#rating-li").removeAttr("class");
        $("#margin-li").removeAttr("class");
    }
    else if (panelname == "banklist") {
        $("#issuer-banklist-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#issuer-banklist-form").attr("aria-expanded", "true");
        $("#issuer-banklist-form").removeAttr("style");
        $("#issuer-banklist-icon").attr("aria-expanded", "true");
        $("#issuer-banklist-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#issuer-banklist-icon").attr("class", "box-header big-head expand-able");

        $("#detail-li").removeAttr("class");
        $("#address-li").removeAttr("class");
        $("#taxandother-li").removeAttr("class");
        $("#banklist-li").attr("class", "active");
        $("#identify-li").removeAttr("class");
        $("#rating-li").removeAttr("class");
        $("#margin-li").removeAttr("class");
    }
    else if (panelname == "identify") {
        $("#issuer-identify-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#issuer-identify-form").attr("aria-expanded", "true");
        $("#issuer-identify-form").removeAttr("style");
        $("#issuer-identify-icon").attr("aria-expanded", "true");
        $("#issuer-identify-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#issuer-identify-icon").attr("class", "box-header big-head expand-able");

        $("#detail-li").removeAttr("class");
        $("#address-li").removeAttr("class");
        $("#taxandother-li").removeAttr("class");
        $("#banklist-li").removeAttr("class");
        $("#identify-li").attr("class", "active");
        $("#rating-li").removeAttr("class");
        $("#margin-li").removeAttr("class");
    }
    else if (panelname == "rating") {
        $("#issuer-rating-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#issuer-rating-form").attr("aria-expanded", "true");
        $("#issuer-rating-form").removeAttr("style");
        $("#issuer-rating-icon").attr("aria-expanded", "true");
        $("#issuer-rating-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#issuer-rating-icon").attr("class", "box-header big-head expand-able");

        $("#detail-li").removeAttr("class");
        $("#address-li").removeAttr("class");
        $("#taxandother-li").removeAttr("class");
        $("#banklist-li").removeAttr("class");
        $("#identify-li").removeAttr("class");
        $("#rating-li").attr("class", "active");
        $("#margin-li").removeAttr("class");
    }
}

$(document).ready(function () {

    $('#issuer-detail').click(function (e) {
        var expand = $("div#issuer-detail-icon").attr("aria-expanded");
        if (expand == "true") {
            $("#issuer-detail-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#issuer-detail-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }

    });

    $('#issuer-address').click(function (e) {
        var expand = $("div#issuer-address-icon").attr("aria-expanded");
        if (expand == "true") {

            $("#issuer-address-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#issuer-address-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }

    });

    $('#issuer-taxandother').click(function (e) {
        var expand = $("div#issuer-taxandother-icon").attr("aria-expanded");
        if (expand == "true") {

            $("#issuer-taxandother-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#issuer-taxandother-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }

    });

    $('#issuer-identify').click(function (e) {
        var expand = $("div#issuer-identify-icon").attr("aria-expanded");
        if (expand == "true") {

            $("#issuer-identify-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#issuer-identify-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }

    });

    $('#issuer-rating').click(function (e) {
        var expand = $("div#issuer-rating-icon").attr("aria-expanded");
        if (expand == "true") {

            $("#issuer-rating-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#issuer-rating-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }

    });

    //Binding DDL IssuerType
    $("#ddl_IssuerType_Add").click(function () {
        var txt_search = $('#txt_IssuerType_Add');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_IssuerType_Add').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Binding DDL IssuerGroup
    $("#ddl_IssuerGroup_Add").click(function () {
        var txt_search = $('#txt_IssuerGroup_Add');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_IssuerGroup_Add').keyup(function () {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Binding DDL TitleName
    $("#ddl_TitleName_Add").click(function () {
        var txt_search = $('#txt_TitleName_Add');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_TitleName_Add').keyup(function () {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
    });

    //Binding DDL Province
    var checkprovince_id;
    $("#ddl_province_add").click(function () {
        var txt_search = $('#txt_search_province_add');
        var data = { datastr: null };
        checkprovince_id = $('#province_id').val();
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $("#ul_province_add").on("click", ".searchterm", function (event) {

        if (checkprovince_id != $('#province_id').val()) {
            $("#ddl_district_add").find(".selected-data").text("Select...");
            $("#ddl_sub_district_add").find(".selected-data").text("Select...");
            $("#district_id").val(null);
            $("#sub_district_id").val(null);
            $("#district_name").text(null);
            $("#sub_district_name").text(null);
            $("#district_name").val(null);
            $("#sub_district_name").val(null);
        }
    });
    $('#txt_search_province_add').keyup(function () {

        //if (this.value.length > 0) {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Province

    //Binding DDL District
    var check_district;
    $("#ddl_district_add").click(function () {
        check_district = $('#district_id').val();
        var txt_search = $('#txt_search_district_add');
        txt_search.val("");
        var province_id = $('#province_id').val();
        var data = { dataint: province_id, datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, "zipcode");
    });
    $("#ul_district_add").on("click", ".searchterm", function (event) {
        if (check_district != $('#district_id').val()) {
            $("#ddl_sub_district_add").find(".selected-data").text("Select...");
            $("#sub_district_id").val(null);
            $("#sub_district_name").text(null);
            $("#sub_district_name").val(null);
        }
    });
    $('#txt_search_district_add').keyup(function () {
        var province_id = $('#province_id').val();
        //if (this.value.length > 0) {
            var data = { dataint: province_id, datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, "zipcode");
        //}
    });
    //District

    //Binding DDL Sub District
    $('#txt_search_sub_district_add').keyup(function () {
        var province_id = $('#district_id').val();
        //if (this.value.length > 0) {
            var data = { dataint: province_id, datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    $("#ddl_sub_district_add").click(function () {
        var txt_search = $('#txt_search_sub_district_add');
        txt_search.val("");
        var district_id = $('#district_id').val();
        var data = { dataint: district_id, datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
    });
    //End SubDistrict

    //Binding DDL Country
    $("#ddl_country_add").click(function () {
        var txt_search = $('#txt_search_country_add');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_search_country_add').keyup(function () {

        //if (this.value.length > 0) {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });

    //Binding DDL ktbisic_code
    $("#ddl_ktbisic_code_add").click(function () {
        var txt_search = $('#txt_ktbisic_code_add');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_ktbisic_code_add').keyup(function () {
        //if (this.value.length > 0) {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });

    //Binding DDL custodian
    $("#ddl_custodian_id_add").click(function () {
        var txt_search = $('#txt_custodian_id_add');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_custodian_id_add').keyup(function () {
        //if (this.value.length > 0) {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });

    //Binding DDL unique
    $("#ddl_unique").click(function () {
        var txt_search = $('#txt_unique');
        txt_search.val("");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, "ddl_identify_type");
    });
    $("#ul_uniqueid").on("click", ".searchterm", function (event) {
        if ($('#unique_id').val() == "") {
            $("#ddl_identify_type").find(".selected-data").text("Select...");
            $("#identify_type").val(null);
            $("#identify_type_text").text(null);
            $("#identify_type_text").val(null);
        }
    });
    $('#txt_unique').keyup(function () {
        //if (this.value.length > 0) {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });

    //Binding DDL identify_type
    $("#ddl_identify_type").click(function () {
        var txt_search = $('#txt_identify_type');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, "ddl_unique");
        txt_search.val("");
    });
    $("#ul_identify").on("click", ".searchterm", function (event) {
        if ($('#identify_type').val() == "") {
            $("#ddl_unique").find(".selected-data").text("Select...");
            $("#unique_id").val(null);
            $("#unique_name").text(null);
            $("#unique_name").val(null);
        }
    });
    $('#txt_identify_type').keyup(function () {

        //if (this.value.length > 0) {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });

    //Binding DDL agency
    $("#ddl_agency").click(function () {
        var txt_search = $('#txt_agency');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $("#ul_agency").on("click", ".searchterm", function (event) {
        //if ($('#identify_type').val() == "") {
        $("#ddl_localrating").find(".selected-data").text("Select...");
        $("#local_rating").val(null);
        $("#local_rating_text").text(null);
        $("#local_rating_text").val(null);

        $("#ddl_foreignrating").find(".selected-data").text("Select...");
        $("#foreign_rating").val(null);
        $("#foreign_rating_text").text(null);
        $("#foreign_rating_text").val(null);
        //}
    });
    $('#txt_agency').keyup(function () {

        //if (this.value.length > 0) {
            var data = { datastr: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });

    //Binding DDL Local Rating
    $("#ddl_localrating").click(function () {
        // check_district = $('#district_id').val();
        var txt_search = $('#txt_localrating');
        var agency_code = $('#RatingRightModal_agency_code').val();
        var short_long_term = $('#RatingRightModal_short_long_term').val();
        var data = { agencycode: agency_code, shortlongterm: short_long_term };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_localrating').keyup(function () {
        var agency_code = $('#RatingRightModal_agency_code').val();
        var short_long_term = $('#RatingRightModal_short_long_term').val();
        //if (this.value.length > 0) {
            var data = { agencycode: agency_code, shortlongterm: short_long_term, datatext: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });

    //Binding DDL Foreign Rating
    $("#ddl_foreignrating").click(function () {
        // check_district = $('#district_id').val();
        var txt_search = $('#txt_foreignrating');
        var agency_code = $('#RatingRightModal_agency_code').val();
        var short_long_term = $('#RatingRightModal_short_long_term').val();
        var data = { agencycode: agency_code, shortlongterm: short_long_term };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    $('#txt_foreignrating').keyup(function () {
        var agency_code = $('#RatingRightModal_agency_code').val();
        var short_long_term = $('#RatingRightModal_short_long_term').val();
        //if (this.value.length > 0) {
            var data = { agencycode: agency_code, shortlongterm: short_long_term, datatext: this.value };
            GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });


    $('.radio input[id=verify_flag_fo]').change(function() {
        var current = $(this).val();
        var radioyes = $("[id=verify_flag_fo][value=true]");
        var radiono = $("[id=verify_flag_fo][value=false]");
        if (current == "true") {
            radioyes.attr('ischeck', 'true');
            radiono.attr('ischeck', 'false');
            radioyes.attr("checked", "checked");
            radiono.removeAttr("checked");
        } else {
            radioyes.attr('ischeck', 'false');
            radiono.attr('ischeck', 'true');
            radiono.attr("checked", "checked");
            radioyes.removeAttr("checked");
        }
    });

    $('.radio input[id=netting_flag]').change(function() {
        var current = $(this).val();
        var radioyes = $("[id=netting_flag][value=true]");
        var radiono = $("[id=netting_flag][value=false]");
        if (current == "true") {
            radioyes.attr('ischeck', 'true');
            radiono.attr('ischeck', 'false');
            radioyes.attr("checked", "checked");
            radiono.removeAttr("checked");
        } else {
            radioyes.attr('ischeck', 'false');
            radiono.attr('ischeck', 'true');
            radiono.attr("checked", "checked");
            radioyes.removeAttr("checked");
        }
    });

    $('.radio input[id=verify_flag_bo]').change(function() {
        var current = $(this).val();
        var radioyes = $("[id=verify_flag_bo][value=true]");
        var radiono = $("[id=verify_flag_bo][value=false]");
        if (current == "true") {
            radioyes.attr('ischeck', 'true');
            radiono.attr('ischeck', 'false');
            radioyes.attr("checked", "checked");
            radiono.removeAttr("checked");
        } else {
            radioyes.attr('ischeck', 'false');
            radiono.attr('ischeck', 'true');
            radiono.attr("checked", "checked");
            radioyes.removeAttr("checked");
        }
    });

    $('.radio input[id=default_flag]').change(function() {
        var current = $(this).val();
        var radioyes = $("[id=default_flag][value=true]");
        var radiono = $("[id=default_flag][value=false]");
        if (current == "true") {
            radioyes.attr('ischeck', 'true');
            radiono.attr('ischeck', 'false');
            radioyes.attr("checked", "checked");
            radiono.removeAttr("checked");
        } else {
            radioyes.attr('ischeck', 'false');
            radiono.attr('ischeck', 'true');
            radiono.attr("checked", "checked");
            radioyes.removeAttr("checked");
        }
    });

    $('.radio input[id=active_flag]').change(function() {
        var current = $(this).val();
        var radioyes = $("[id=active_flag][value=true]");
        var radiono = $("[id=active_flag][value=false]");
        if (current == "true") {
            radioyes.attr('ischeck', 'true');
            radiono.attr('ischeck', 'false');
            radioyes.attr("checked", "checked");
            radiono.removeAttr("checked");
        } else {
            radioyes.attr('ischeck', 'false');
            radiono.attr('ischeck', 'true');
            radiono.attr("checked", "checked");
            radioyes.removeAttr("checked");
        }
    });

    $('.radio input[id=custodian_flag]').change(function() {
        var current = $(this).val();
        var radioyes = $("[id=custodian_flag][value=true]");
        var radiono = $("[id=custodian_flag][value=false]");
        if (current == "true") {
            radioyes.attr('ischeck', 'true');
            radiono.attr('ischeck', 'false');
            radioyes.attr("checked", "checked");
            radiono.removeAttr("checked");
            $("#ddl_custodian_id_add").removeAttr('disabled');
        } else {
            radioyes.attr('ischeck', 'false');
            radiono.attr('ischeck', 'true');
            radiono.attr("checked", "checked");
            radioyes.removeAttr("checked");
            $("#ddl_custodian_id_add").attr('disabled', 'disabled');

        }
    });

    $('.radio input[id=borrow_only_flag]').change(function() {
        var current = $(this).val();
        var radioyes = $("[id=borrow_only_flag][value=true]");
        var radiono = $("[id=borrow_only_flag][value=false]");
        if (current == "true") {
            radioyes.attr('ischeck', 'true');
            radiono.attr('ischeck', 'false');
            radioyes.attr("checked", "checked");
            radiono.removeAttr("checked");
        } else {
            radioyes.attr('ischeck', 'false');
            radiono.attr('ischeck', 'true');
            radiono.attr("checked", "checked");
            radioyes.removeAttr("checked");
        }
    });


    // Model Identif
    GM.Issuer = {};
    $('#btnIdentifyCreate').on('click', function () {
        GM.Issuer.Identify.Form.Show(this);
    });
    $('#btnIdentifySave').on('click', function () {
        GM.Issuer.Identify.Form.Save(this);
    });

    GM.Issuer.Identify = {};
    GM.Issuer.Identify.Table = $("#tbIdentify");
    GM.Issuer.Identify.Table.RowSelected = {};
    GM.Issuer.Identify.Table.RowEmpty = function() {
        var row = $("<tr></tr>");
        var col = $('<td class="long-data text-center empty-data" style="height:50px;" colspan="3"> No data.</td>');
        row.append(col);
        GM.Issuer.Identify.Table.append(row);
    };
    GM.Issuer.Identify.Table.RowAny = function(code, code2, rowid) {

        var rows = GM.Issuer.Identify.Table.find('tr');
        var IsExits = false;
        rows.each(function(index, row) {

            var rowindex = $(row).data("id");
            var inputs = $(row).find('input,select,textarea');

            if (inputs.length > 0) {

                $.each(inputs,
                    function() {

                        var names = this.name.split('.');

                        if (names[1] == 'unique_id' && this.value == code) {

                            if (rowid != rowindex) {
                                IsExits = true;
                                //return false
                            }
                        }
                    });
            }
            if (IsExits) {
                return false;
            }
        });

        return IsExits;
    };
    GM.Issuer.Identify.Form = $("#modal-form-identify");
    GM.Issuer.Identify.Form.ButtonSave = $('#btnIdentifySave');
    GM.Issuer.Identify.Form.Show = function (btn) {

        var action = $(btn).data("action");
        var button = $('#btnIdentifySave');

        GM.Issuer.Identify.Form.Reset();

        $('#ddl_unique').removeAttr('disabled');
        $('#ddl_identify_type').removeAttr('disabled');
        $('#IdentifyRightModal_identify_no').removeAttr('disabled');
        $('#IdentifyRightModal_juris_reg_date').removeAttr('disabled');
        $('#IdentifyRightModal_reg_bus_ename').removeAttr('disabled');
        $('#IdentifyRightModal_reg_bus_tname').removeAttr('disabled');

        switch (action) {
            case 'create':
                GM.Issuer.Identify.Form.find(".modal-title").html("Create");
                GM.Issuer.Identify.Form.ButtonSave.data("action", "create");
                GM.Issuer.Identify.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('+ Add Identify');
                break;

            case 'update':
                GM.Issuer.Identify.Table.RowSelected = $(btn).parent().parent();
                GM.Issuer.Identify.Form.find(".modal-title").html("Update");
                GM.Issuer.Identify.Form.Initial();
                GM.Issuer.Identify.Form.ButtonSave.data("action", "update");
                GM.Issuer.Identify.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('Save Identify');
                break;

            case 'delete':
                $('#ddl_unique').attr('disabled', 'disabled');
                $('#ddl_identify_type').attr('disabled', 'disabled');
                $('#IdentifyRightModal_identify_no').attr('disabled', 'disabled');
                $('#IdentifyRightModal_juris_reg_date').attr('disabled', 'disabled');
                $('#IdentifyRightModal_reg_bus_ename').attr('disabled', 'disabled');
                $('#IdentifyRightModal_reg_bus_tname').attr('disabled', 'disabled');
                GM.Issuer.Identify.Table.RowSelected = $(btn).parent().parent();
                GM.Issuer.Identify.Form.find(".modal-title").html("Delete");
                GM.Issuer.Identify.Form.Initial();
                GM.Issuer.Identify.Form.ButtonSave.data("action", "delete");
                GM.Issuer.Identify.Form.ButtonSave.removeClass('btn-primary').addClass('btn-delete').text('Confirm Delete');
                break;

            default:
                break;
        }

        GM.Issuer.Identify.Form.modal('toggle');
    };
    GM.Issuer.Identify.Form.Valid = function() {

        var IsValid = true;

        //var guarantor_code = $('#guarantor_code').val();
        //var guarantor_percent = $('#guarantor_percent').val();

        //if (guarantor_code == "") {
        //    $('#guarantor_code').css('border-color', "red");
        //    IsValid = false;
        //}

        //if (guarantor_percent == "") {
        //    $('#guarantor_percent').css('border-color', "red");
        //    IsValid = false;
        //}
        //else if (!$.isNumeric(guarantor_percent)) {
        //    $('#guarantor_percent').css('border-color', "red");
        //    IsValid = false;
        //}

        return IsValid;

    };
    GM.Issuer.Identify.Form.Save = function(btn) {

        var action = $(btn).data("action");

        //  if (!GM.Issuer.Identify.Form.Valid()) {
        //      return false;
        //  }

        var IsInValid = false;
        GM.Message.Clear();
        $("#unique_id_error").text("");
        $("#identify_type_error").text("");
        $("#identify_no_error").text("");

        var unique_text = $('#IdentifyRightModal_unique_name').text();
        var unique_val = $("#IdentifyRightModal_unique_id").val();
        var identify_type_text = $('#IdentifyRightModal_identify_type_text').text();
        var identify_type_val = $("#IdentifyRightModal_identify_type").val();
        var identify_no = $('#IdentifyRightModal_identify_no').val();
        var juris_reg_date = $("#IdentifyRightModal_juris_reg_date").val();
        var reg_bus_ename = $('#IdentifyRightModal_reg_bus_ename').val();
        var reg_bus_tname = $('#IdentifyRightModal_reg_bus_tname').val();

        if (unique_val == "") {
            $("#unique_id_error").text("The Unique field is required.");
            IsInValid = true;
        }

        if (identify_type_val == "") {
            $("#identify_type_error").text("The Identify Type field is required.");
            IsInValid = true;
        }

        if (identify_no == "") {
            $("#identify_no_error").text("The Identify No field is required.");
            IsInValid = true;
        }

        if (IsInValid) {
            return;
        }

        switch (action) {
        case 'create':
            GM.Issuer.Identify.Table.attr("style", "display: show");

            var rowindex = GM.Issuer.Identify.Table.find("tr:last").data("id");

            if (!GM.Issuer.Identify.Table.RowAny(unique_text)) {
                if (rowindex != undefined) {
                    rowindex = parseInt(rowindex) + 1;
                } else {
                    rowindex = 0;
                }
                var row = $('<tr data-id="' + rowindex + '" ></tr>');
                var ColUniqueID = $('<td class="long-data">' +
                    unique_text +
                    '<input name="Identify[' +
                    rowindex +
                    '].unique_id" type="hidden" Isdropdown="true" ' +
                    'textfield="IdentifyRightModal.unique_name" textvalue="' +
                    unique_text +
                    '" ' +
                    'valuefield="IdentifyRightModal.unique_id" value="' +
                    unique_val +
                    '">' +
                    '<input name="Identify[' +
                    rowindex +
                    '].unique_name" type="hidden" Isdropdown="true" ' +
                    'textfield="IdentifyRightModal.unique_name" textvalue="' +
                    unique_text +
                    '" ' +
                    'valuefield="IdentifyRightModal.unique_id" value="' +
                    unique_text +
                    '">' +
                    '</td>');

                var ColIdentifyType = $('<td class="long-data">' +
                    identify_type_text +
                    '<input name="Identify[' +
                    rowindex +
                    '].identify_type" type="hidden" Isdropdown="true" ' +
                    'textfield="IdentifyRightModal.identify_type_text" textvalue="' +
                    identify_type_text +
                    '" ' +
                    'valuefield="IdentifyRightModal.identify_type" value="' +
                    identify_type_val +
                    '">' +
                    '<input name="Identify[' +
                    rowindex +
                    '].identify_type_text" type="hidden" Isdropdown="true" ' +
                    'textfield="IdentifyRightModal.identify_type_text" textvalue="' +
                    identify_type_text +
                    '" ' +
                    'valuefield="IdentifyRightModal.identify_type" value="' +
                    identify_type_text +
                    '">' +
                    '</td>');


                var ColIdentifyNo = $('<td>' +
                    identify_no +
                    '<input name="Identify[' +
                    rowindex +
                    '].identify_no" Isdropdown="false" type="hidden" value="' +
                    identify_no +
                    '"></td>');
                var ColJurisRegDate = $('<td>' +
                    juris_reg_date +
                    '<input name="Identify[' +
                    rowindex +
                    '].juris_reg_date" Isdropdown="false"  type="hidden" value="' +
                    juris_reg_date +
                    '"></td>');
                var ColRegBusEname = $('<td>' +
                    reg_bus_ename +
                    '<input name="Identify[' +
                    rowindex +
                    '].reg_bus_ename" Isdropdown="false"  type="hidden" value="' +
                    reg_bus_ename +
                    '"></td>');
                var ColRegBustname = $('<td>' +
                    reg_bus_tname +
                    '<input name="Identify[' +
                    rowindex +
                    '].reg_bus_tname" Isdropdown="false"  type="hidden" value="' +
                    reg_bus_tname +
                    '"></td>');
                var ColAction = $('<td class="action">' +
                    '<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                    unique_text +
                    '" data-action="update" onclick="GM.Issuer.Identify.Form.Show(this)" >' +
                    '<i class="feather-icon icon-edit"></i>' +
                    '</button > ' +
                    '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                    unique_text +
                    '" data-action="delete" onclick="GM.Issuer.Identify.Form.Show(this)" >' +
                    '<i class="feather-icon icon-trash-2"></i>' +
                    '</button>' +
                    '</td>');

                row.append(ColUniqueID);
                row.append(ColIdentifyType);
                row.append(ColIdentifyNo);
                row.append(ColJurisRegDate);
                row.append(ColRegBusEname);
                row.append(ColRegBustname);
                row.append(ColAction);

                GM.Issuer.Identify.Table.append(row);
                GM.Issuer.Identify.Table.find(".empty-data").parent().remove();

                //swal("Good job!", "Successfully saved", "success");
            } else {
                //swal("Error", "The data guarantor already exists.", "error");
                GM.Message.Error('.modal-body', "The Data already exists.");
                return;
            }

            break;

        case 'update':
            var unique_text = $('#IdentifyRightModal_unique_name').text();
            var unique_val = $("#IdentifyRightModal_unique_id").val();
            var identify_type_text = $('#IdentifyRightModal_identify_type_text').text();
            var identify_type_val = $("#IdentifyRightModal_identify_type").val();
            var identify_no = $('#IdentifyRightModal_identify_no').val();
            var juris_reg_date = $("#IdentifyRightModal_juris_reg_date").val();
            var reg_bus_ename = $('#IdentifyRightModal_reg_bus_ename').val();
            var reg_bus_tname = $('#IdentifyRightModal_reg_bus_tname').val();
            var rowindex = GM.Issuer.Identify.Table.find("tr:last").data("id");

            var row = GM.Issuer.Identify.Table.RowSelected;
            var rowindex = row.data("id");

            if (!GM.Issuer.Identify.Table.RowAny(unique_val, null, rowindex)) {
                // if (1 == 1) {
                var ColUniqueID = row.children("td:nth-child(1)");
                var ColIdentifyType = row.children("td:nth-child(2)");
                var ColIdentifyNo = row.children("td:nth-child(3)");
                var ColJurisRegDate = row.children("td:nth-child(4)");
                var ColRegBusEname = row.children("td:nth-child(5)");
                var ColRegBusTname = row.children("td:nth-child(6)");
                var ColAction = row.children("td:nth-child(7)");

                ColUniqueID.html(unique_text +
                    '<input name="Identify[' +
                    rowindex +
                    '].unique_id" type="hidden" Isdropdown="true" ' +
                    'textfield="IdentifyRightModal.unique_name" textvalue="' +
                    unique_text +
                    '" ' +
                    'valuefield="IdentifyRightModal.unique_id" value="' +
                    unique_val +
                    '">' +
                    '<input name="Identify[' +
                    rowindex +
                    '].unique_name" type="hidden" Isdropdown="true" ' +
                    'textfield="IdentifyRightModal.unique_name" textvalue="' +
                    unique_text +
                    '" ' +
                    'valuefield="IdentifyRightModal.unique_id" value="' +
                    unique_text +
                    '">');
                ColIdentifyType.html(identify_type_text +
                    '<input name="Identify[' +
                    rowindex +
                    '].identify_type" type="hidden" Isdropdown="true" ' +
                    'textfield="IdentifyRightModal.identify_type_text" textvalue="' +
                    identify_type_text +
                    '" ' +
                    'valuefield="IdentifyRightModal.identify_type" value="' +
                    identify_type_val +
                    '">' +
                    '<input name="Identify[' +
                    rowindex +
                    '].identify_type_text" type="hidden" Isdropdown="true" ' +
                    'textfield="IdentifyRightModal.identify_type_text" textvalue="' +
                    identify_type_text +
                    '" ' +
                    'valuefield="IdentifyRightModal.identify_type" value="' +
                    identify_type_text +
                    '">');

                ColIdentifyNo.html(identify_no +
                    '<input name="Identify[' +
                    rowindex +
                    '].identify_no" Isdropdown="false" type="hidden" value="' +
                    identify_no +
                    '">');
                ColJurisRegDate.html(juris_reg_date +
                    '<input name="Identify[' +
                    rowindex +
                    '].juris_reg_date" Isdropdown="false"  type="hidden" value="' +
                    juris_reg_date +
                    '">');
                ColRegBusEname.html(reg_bus_ename +
                    '<input name="Identify[' +
                    rowindex +
                    '].reg_bus_ename" Isdropdown="false"  type="hidden" value="' +
                    reg_bus_ename +
                    '">');
                ColRegBusTname.html(reg_bus_tname +
                    '<input name="Identify[' +
                    rowindex +
                    '].reg_bus_tname" Isdropdown="false"  type="hidden" value="' +
                    reg_bus_tname +
                    '">');

                ColAction.html('<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                    unique_text +
                    '" data-action="update" onclick="GM.Issuer.Identify.Form.Show(this)" >' +
                    '<i class="feather-icon icon-edit"></i>' +
                    '</button > ' +
                    '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                    unique_text +
                    '" data-action="delete" onclick="GM.Issuer.Identify.Form.Show(this)" >' +
                    '<i class="feather-icon icon-trash-2"></i>' +
                    '</button>');

                //swal("Good job!", "Successfully saved", "success");
            } else {
                //swal("Error", "The data guarantor already exists.", "error");
                GM.Message.Error('.modal-body', "The Data already exists.");
                return;
            }
            break;

        case 'delete':

            var rowselect = GM.Issuer.Identify.Table.RowSelected;
            var rowfrom = rowselect.data("action");
            var rowid = rowselect.data("id");
            if (rowfrom != "fromdatabase") {
                GM.Issuer.Identify.Table.RowSelected.remove();
            } else {
                rowselect.attr("style", "display: none");
                var inputs = $(rowselect).find('input,select,textarea');
                if (inputs.length > 0) {
                    $.each(inputs,
                        function() {
                            var names = this.name.split('.');
                            //update new input name                                
                            if (names[1] == "rowstatus") {
                                var inputname = 'Identify[' + rowid + '].' + names[1];
                                $(this).attr('name', inputname);
                                $(this).attr("value", "delete");
                                $(this).attr("type", "hidden");
                                $(this).text("delete");
                            }
                        });
                }
            }
            //renew index of input
            var rows = GM.Issuer.Identify.Table.find('tr');
            var rowindex = 0;
            var checkrow = 0;
            rows.each(function(index, row) {

                var inputs = $(row).find('input,select,textarea');

                if (inputs.length > 0) {

                    $.each(inputs,
                        function() {

                            var names = this.name.split('.');
                            var inputname = 'Identify[' + rowindex + '].' + names[1];
                            //update new input name
                            $(this).attr('name', inputname);

                            if (names[1] == "rowstatus" && $(this).val() != "delete") {
                                checkrow++;
                            }

                        });

                    rowindex++;
                }

            });

            if (checkrow == 0) {
                var row = $('<tr data-id="' + 0 + '" ></tr>');
                var rowEmpty =
                    '<td class="long-data text-center empty-data" style="height: 50px; " colspan="7"> No data.</td>';
                row.append(rowEmpty);
                GM.Issuer.Identify.Table.append(row);
                //GM.Issuer.Identify.Table.RowEmpty()
            }

            break;

        }

        GM.Issuer.Identify.Table.RowSelected = {};

        GM.Issuer.Identify.Form.modal('toggle');
    };
    GM.Issuer.Identify.Form.Initial = function () {
        if (GM.Issuer.Identify.Table.RowSelected) {
            var inputs = $(GM.Issuer.Identify.Table.RowSelected).find('input,select,textarea');

            if (inputs.length > 0) {
                $.each(inputs, function () {
                    var names = this.name.split('.');
                    if (this.attributes.isdropdown.value == 'true') {
                        var filedtextid = this.attributes.textfield.value.split('.');
                        $("#IdentifyForm span[name='" + filedtextid[1] + "']").text(this.attributes.textvalue.value);
                        $("#IdentifyForm input[name='" + this.attributes.textfield.value + "']").val(this.attributes.textvalue.value);
                        $("#IdentifyForm input[name='" + this.attributes.textfield.value + "']").text(this.attributes.textvalue.value);
                        $("#IdentifyForm input[name='" + this.attributes.valuefield.value + "']").val(this.value);
                    } else {
                        //console.log('$(\'#' + names[1] + '\').val(' + this.value + ');');
                        //$('#' + names[1]).val(this.value);   
                        $("#IdentifyForm input[name='IdentifyRightModal." + names[1] + "']").val(this.value);
                        $("#IdentifyForm input[name='IdentifyRightModal." + names[1] + "']").text(this.value);
                    }
                });
            }

        }
    };
    GM.Issuer.Identify.Form.Reset = function () {

        $("#ddl_unique").find(".selected-data").text("Select...");
        $('#IdentifyRightModal_unique_name').text(null);
        $("#IdentifyRightModal_unique_id").val(null);
        $("#ddl_identify_type").find(".selected-data").text("Select...");
        $('#IdentifyRightModal_identify_type_text').text(null);
        $("#IdentifyRightModal_identify_type").val(null);
        $('#IdentifyRightModal_identify_no').val(null);
        $('#IdentifyRightModal_juris_reg_date').val(null);
        $('#IdentifyRightModal_reg_bus_ename').val(null);
        $('#IdentifyRightModal_reg_bus_tname').val(null);

        GM.Message.Clear();
        $("#unique_id_error").text("");
        $("#identify_type_error").text("");
        $("#identify_no_error").text("");

        $('#IdentifyRightModal_identify_no').removeClass("input-validation-error");
    };

    // Model Rating
    $('#btnRatingCreate').on('click', function () {
        GM.Issuer.Rating.Form.Show(this);
    });
    $('#btnRatingSave').on('click', function () {
        GM.Issuer.Rating.Form.Save(this);
    });

    GM.Issuer.Rating = {};
    GM.Issuer.Rating.Table = $("#tbRating");
    GM.Issuer.Rating.Table.RowSelected = {};
    GM.Issuer.Rating.Table.RowEmpty = function() {
        var row = $("<tr></tr>");
        var col = $('<td class="long-data text-center empty-data" style="height:50px;" colspan="3"> No data.</td>');
        row.append(col);
        GM.Issuer.Rating.Table.append(row);
    };
    GM.Issuer.Rating.Table.RowAny = function(code, code2, rowid) {

        var rows = GM.Issuer.Rating.Table.find('tr');
        var IsExits = false;
        rows.each(function(index, row) {

            var rowindex = $(row).data("id");
            var inputs = $(row).find('input,select,textarea');

            if (inputs.length > 0) {

                $.each(inputs,
                    function() {

                        var names = this.name.split('.');

                        if (names[1] == 'agency_code' && this.value == code) {

                            if (rowid != rowindex) {
                                IsExits = true;
                                //return false
                            }
                        }
                    });
            }
            if (IsExits) {
                return false;
            }
        });

        return IsExits;
    };
    GM.Issuer.Rating.Form = $("#modal-form-rating");
    GM.Issuer.Rating.Form.ButtonSave = $('#btnRatingSave');
    GM.Issuer.Rating.Form.Show = function (btn) {

        var action = $(btn).data("action");
        var button = $('#btnRatingSave');

        GM.Issuer.Rating.Form.Reset();

        $('#ddl_agency').removeAttr('disabled');
        $('#ddl_term').removeAttr('disabled');
        $('#ddl_localrating').removeAttr('disabled');
        $('#ddl_foreignrating').removeAttr('disabled');

        switch (action) {
            case 'create':
                GM.Issuer.Rating.Form.find(".modal-title").html("Create");
                GM.Issuer.Rating.Form.ButtonSave.data("action", "create");
                GM.Issuer.Rating.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('+ Add Rating');
                break;

            case 'update':

                GM.Issuer.Rating.Table.RowSelected = $(btn).parent().parent();
                GM.Issuer.Rating.Form.find(".modal-title").html("Update");
                GM.Issuer.Rating.Form.Initial();

                GM.Issuer.Rating.Form.ButtonSave.data("action", "update");
                GM.Issuer.Rating.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('Save Rating');
                break;

            case 'delete':

                $('#ddl_agency').attr('disabled', 'disabled');
                $('#ddl_term').attr('disabled', 'disabled');
                $('#ddl_localrating').attr('disabled', 'disabled');
                $('#ddl_foreignrating').attr('disabled', 'disabled');

                GM.Issuer.Rating.Table.RowSelected = $(btn).parent().parent();
                GM.Issuer.Rating.Form.find(".modal-title").html("Delete");
                GM.Issuer.Rating.Form.Initial();

                GM.Issuer.Rating.Form.ButtonSave.data("action", "delete");
                GM.Issuer.Rating.Form.ButtonSave.removeClass('btn-primary').addClass('btn-delete').text('Confirm Delete');
                break;

            default:
                break;
        }

        GM.Issuer.Rating.Form.modal('toggle');
    };
    GM.Issuer.Rating.Form.Valid = function() {

        var IsValid = true;

        //var guarantor_code = $('#guarantor_code').val();
        //var guarantor_percent = $('#guarantor_percent').val();

        //if (guarantor_code == "") {
        //    $('#guarantor_code').css('border-color', "red");
        //    IsValid = false;
        //}

        //if (guarantor_percent == "") {
        //    $('#guarantor_percent').css('border-color', "red");
        //    IsValid = false;
        //}
        //else if (!$.isNumeric(guarantor_percent)) {
        //    $('#guarantor_percent').css('border-color', "red");
        //    IsValid = false;
        //}

        return IsValid;

    };

    GM.Issuer.Rating.Form.Save = function(btn) {

        var action = $(btn).data("action");

        //  if (!GM.Issuer.Identify.Form.Valid()) {
        //      return false;
        //  }

        var IsInValid = true;
        GM.Message.Clear();
        $("#agency_code_error").text("");
        $("#short_long_term_error").text("");
        $("#local_rating_error").text("");
        $("#foreign_rating_error").text("");

        var agency_text = $('#RatingRightModal_agency_name').text();
        var agency_val = $("#RatingRightModal_agency_code").val();
        var short_long_term_text = $('#RatingRightModal_short_long_term_text').text();
        var short_long_term_val = $("#RatingRightModal_short_long_term").val();
        var local_rating_text = $('#RatingRightModal_local_rating_text').text();
        var local_rating_val = $("#RatingRightModal_local_rating").val();
        var foreign_rating_text = $('#RatingRightModal_foreign_rating_text').text();
        var foreign_rating_val = $("#RatingRightModal_foreign_rating").val();

        if (agency_val == "") {
            $("#agency_code_error").text("The Agency field is required.");
            IsInValid = false;
        }

        if (short_long_term_val == "") {
            $("#short_long_term_error").text("The Term field is required.");
            IsInValid = false;
        }

        if (local_rating_val == "") {
            $("#local_rating_error").text("The Local Rating field is required.");
            IsInValid = false;
        }

        if (foreign_rating_val == "") {
            $("#foreign_rating_error").text("The Foreign Rating field is required.");
            IsInValid = false;
        }

        if (IsInValid) {
            switch (action) {
            case 'create':
                GM.Issuer.Rating.Table.attr("style", "display: show");

                var rowindex = GM.Issuer.Rating.Table.find("tr:last").data("id");

                if (!GM.Issuer.Rating.Table.RowAny(agency_val)) {
                    if (rowindex != undefined) {
                        rowindex = parseInt(rowindex) + 1;
                    } else {
                        rowindex = 0;
                    }
                    var row = $('<tr data-id="' + rowindex + '" ></tr>');

                    var ColAgencyCode = $('<td class="long-data">' +
                        agency_text +
                        '<input name="Rating[' +
                        rowindex +
                        '].agency_code" type="hidden" Isdropdown="true" ' +
                        'textfield="RatingRightModal.agency_name" textvalue="' +
                        agency_text +
                        '" ' +
                        'valuefield="RatingRightModal.agency_code" value="' +
                        agency_val +
                        '">' +
                        '<input name="Rating[' +
                        rowindex +
                        '].agency_name" type="hidden" Isdropdown="true" ' +
                        'textfield="RatingRightModal.agency_name" textvalue="' +
                        agency_text +
                        '" ' +
                        'valuefield="RatingRightModal.agency_code" value="' +
                        agency_text +
                        '">' +
                        '</td >');

                    var ColShortLongTerm = $('<td class="long-data">' +
                        short_long_term_text +
                        '<input name="Rating[' +
                        rowindex +
                        '].short_long_term" type="hidden" Isdropdown="true" ' +
                        'textfield="RatingRightModal.short_long_term_text" textvalue="' +
                        short_long_term_text +
                        '" ' +
                        'valuefield="RatingRightModal.short_long_term" value="' +
                        short_long_term_val +
                        '">' +
                        '<input name="Rating[' +
                        rowindex +
                        '].short_long_term_text" type="hidden" Isdropdown="true" ' +
                        'textfield="RatingRightModal.short_long_term_text" textvalue="' +
                        short_long_term_text +
                        '" ' +
                        'valuefield="RatingRightModal.short_long_term" value="' +
                        short_long_term_text +
                        '">' +
                        '</td > ');

                    var ColLocalRating = $('<td class="long-data">' +
                        local_rating_text +
                        '<input name="Rating[' +
                        rowindex +
                        '].local_rating" type="hidden" Isdropdown="true" ' +
                        'textfield="RatingRightModal.local_rating_text" textvalue="' +
                        local_rating_text +
                        '" ' +
                        'valuefield="RatingRightModal.local_rating" value="' +
                        local_rating_val +
                        '">' +
                        '<input name="Rating[' +
                        rowindex +
                        '].local_rating_text" type="hidden" Isdropdown="true" ' +
                        'textfield="RatingRightModal.local_rating_text" textvalue="' +
                        local_rating_text +
                        '" ' +
                        'valuefield="RatingRightModal.local_rating" value="' +
                        local_rating_text +
                        '">' +
                        '</td>');

                    var ColForeignRating = $('<td class="long-data">' +
                        foreign_rating_text +
                        '<input name="Rating[' +
                        rowindex +
                        '].foreign_rating" type="hidden" Isdropdown="true" ' +
                        'textfield="RatingRightModal.foreign_rating_text" textvalue="' +
                        foreign_rating_text +
                        '" ' +
                        'valuefield="RatingRightModal.foreign_rating" value="' +
                        foreign_rating_val +
                        '">' +
                        '<input name="Rating[' +
                        rowindex +
                        '].foreign_rating_text" type="hidden" Isdropdown="true" ' +
                        'textfield="RatingRightModal.foreign_rating_text" textvalue="' +
                        foreign_rating_text +
                        '" ' +
                        'valuefield="RatingRightModal.foreign_rating" value="' +
                        foreign_rating_text +
                        '">' +
                        '</td > ');

                    var ColAction = $('<td class="action">' +
                        '<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                        agency_val +
                        '" data-action="update" onclick="GM.Issuer.Rating.Form.Show(this)" >' +
                        '<i class="feather-icon icon-edit"></i>' +
                        '</button > ' +
                        '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                        agency_val +
                        '" data-action="delete" onclick="GM.Issuer.Rating.Form.Show(this)" >' +
                        '<i class="feather-icon icon-trash-2"></i>' +
                        '</button>' +
                        '</td>');

                    row.append(ColAgencyCode);
                    row.append(ColShortLongTerm);
                    row.append(ColLocalRating);
                    row.append(ColForeignRating);
                    row.append(ColAction);

                    GM.Issuer.Rating.Table.append(row);
                    GM.Issuer.Rating.Table.find(".empty-data").parent().remove();

                    // swal("Good job!", "Successfully saved", "success");
                } else {
                    //swal("Error", "The data guarantor already exists.", "error");
                    GM.Message.Error('.modal-body', "The Data already exists.");
                    return;
                }

                break;

            case 'update':

                var rowindex = GM.Issuer.Rating.Table.find("tr:last").data("id");

                var row = GM.Issuer.Rating.Table.RowSelected;
                var rowindex = row.data("id");

                if (!GM.Issuer.Rating.Table.RowAny(agency_val, null, rowindex)) {
                    // if (1 == 1) {
                    var ColAgencyCode = row.children("td:nth-child(1)");
                    var ColShortLongTerm = row.children("td:nth-child(2)");
                    var ColLocalRating = row.children("td:nth-child(3)");
                    var ColForeignRating = row.children("td:nth-child(4)");
                    var ColAction = row.children("td:nth-child(5)");

                    ColAgencyCode.html(agency_text +
                        '<input name="Rating[' +
                        rowindex +
                        '].agency_code" type="hidden" Isdropdown="true" ' +
                        'textfield="RatingRightModal.agency_name" textvalue="' +
                        agency_text +
                        '" ' +
                        'valuefield="RatingRightModal.agency_code" value="' +
                        agency_val +
                        '">' +
                        '<input name="Rating[' +
                        rowindex +
                        '].agency_name" type="hidden" Isdropdown="true" ' +
                        'textfield="RatingRightModal.agency_name" textvalue="' +
                        agency_text +
                        '" ' +
                        'valuefield="RatingRightModal.agency_code" value="' +
                        agency_text +
                        '">');

                    ColShortLongTerm.html(short_long_term_text +
                        '<input name="Rating[' +
                        rowindex +
                        '].short_long_term" type="hidden" Isdropdown="true" ' +
                        'textfield="RatingRightModal.short_long_term_text" textvalue="' +
                        short_long_term_text +
                        '" ' +
                        'valuefield="RatingRightModal.short_long_term" value="' +
                        short_long_term_val +
                        '">' +
                        '<input name="Rating[' +
                        rowindex +
                        '].short_long_term_text" type="hidden" Isdropdown="true" ' +
                        'textfield="RatingRightModal.short_long_term_text" textvalue="' +
                        short_long_term_text +
                        '" ' +
                        'valuefield="RatingRightModal.short_long_term" value="' +
                        short_long_term_text +
                        '">');

                    ColLocalRating.html(local_rating_text +
                        '<input name="Rating[' +
                        rowindex +
                        '].local_rating" type="hidden" Isdropdown="true" ' +
                        'textfield="RatingRightModal.local_rating_text" textvalue="' +
                        local_rating_text +
                        '" ' +
                        'valuefield="RatingRightModal.local_rating" value="' +
                        local_rating_val +
                        '">' +
                        '<input name="Rating[' +
                        rowindex +
                        '].local_rating_text" type="hidden" Isdropdown="true" ' +
                        'textfield="RatingRightModal.local_rating_text" textvalue="' +
                        local_rating_text +
                        '" ' +
                        'valuefield="RatingRightModal.local_rating" value="' +
                        local_rating_text +
                        '">');

                    ColForeignRating.html(foreign_rating_text +
                        '<input name="Rating[' +
                        rowindex +
                        '].foreign_rating" type="hidden" Isdropdown="true" ' +
                        'textfield="RatingRightModal.foreign_rating_text" textvalue="' +
                        foreign_rating_text +
                        '" ' +
                        'valuefield="RatingRightModal.foreign_rating" value="' +
                        foreign_rating_val +
                        '">' +
                        '<input name="Rating[' +
                        rowindex +
                        '].foreign_rating_text" type="hidden" Isdropdown="true" ' +
                        'textfield="RatingRightModal.foreign_rating_text" textvalue="' +
                        foreign_rating_text +
                        '" ' +
                        'valuefield="RatingRightModal.foreign_rating" value="' +
                        foreign_rating_text +
                        '">');

                    ColAction.html('<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                        agency_val +
                        '" data-action="update" onclick="GM.Issuer.Rating.Form.Show(this)" >' +
                        '<i class="feather-icon icon-edit"></i>' +
                        '</button > ' +
                        '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                        agency_val +
                        '" data-action="delete" onclick="GM.Issuer.Rating.Form.Show(this)" >' +
                        '<i class="feather-icon icon-trash-2"></i>' +
                        '</button>');

                    //swal("Good job!", "Successfully saved", "success");
                } else {
                    //swal("Error", "The data guarantor already exists.", "error");
                    GM.Message.Error('.modal-body', "The Data already exists.");
                    return;
                }
                break;

            case 'delete':

                GM.Issuer.Rating.Table.RowSelected.remove();

                //renew index of input
                var rows = GM.Issuer.Rating.Table.find('tr');
                var rowindex = 0;

                rows.each(function(index, row) {

                    var inputs = $(row).find('input,select,textarea');

                    if (inputs.length > 0) {

                        $.each(inputs,
                            function() {

                                var names = this.name.split('.');
                                var inputname = 'Rating[' + rowindex + '].' + names[1];

                                //update new input name
                                $(this).attr('name', inputname);

                            });

                        rowindex++;
                    }
                });
                if (rowindex === 0) {
                    var row = $('<tr data-id="' + 0 + '" ></tr>');
                    var rowEmpty =
                        '<td class="long-data text-center empty-data" style="height: 50px; " colspan="7"> No data.</td>';
                    row.append(rowEmpty);
                    GM.Issuer.Rating.Table.append(row);
                    //GM.Issuer.Rating.Table.attr("style", "display: none");
                    //GM.Issuer.Rating.Table.RowEmpty(table)
                }

                break;

            }

            GM.Issuer.Rating.Table.RowSelected = {};

            GM.Issuer.Rating.Form.modal('toggle');
        }
    };
    GM.Issuer.Rating.Form.Initial = function () {
        if (GM.Issuer.Rating.Table.RowSelected) {
            var inputs = $(GM.Issuer.Rating.Table.RowSelected).find('input,select,textarea');

            if (inputs.length > 0) {
                $.each(inputs, function () {
                    var names = this.name.split('.');
                    if (this.attributes.isdropdown.value == 'true') {
                        var filedtextid = this.attributes.textfield.value.split('.');
                        $("#RatingForm span[name='" + filedtextid[1] + "']").text(this.attributes.textvalue.value);
                        $("#RatingForm input[name='" + this.attributes.textfield.value + "']").val(this.attributes.textvalue.value);
                        $("#RatingForm input[name='" + this.attributes.textfield.value + "']").text(this.attributes.textvalue.value);
                        $("#RatingForm input[name='" + this.attributes.valuefield.value + "']").val(this.value);
                    } else {
                        //console.log('$(\'#' + names[1] + '\').val(' + this.value + ');');
                        //$('#' + names[1]).val(this.value);   
                        $("#RatingForm input[name='RatingRightModal." + names[1] + "']").val(this.value);
                        $("#RatingForm input[name='RatingRightModal." + names[1] + "']").text(this.value);
                    }
                });
            }

        }
    };
    GM.Issuer.Rating.Form.Reset = function () {

        $("#ddl_agency").find(".selected-data").text("Select...");
        $('#RatingRightModal_agency_name').text(null);
        $("#RatingRightModal_agency_code").val(null);

        $("#ddl_term").find(".selected-data").text("Select...");
        $('#RatingRightModal_short_long_term_text').text(null);
        $("#RatingRightModal_short_long_term").val(null);

        $("#ddl_localrating").find(".selected-data").text("Select...");
        $('#RatingRightModal_local_rating_text').text(null);
        $("#RatingRightModal_local_rating").val(null);

        $("#ddl_foreignrating").find(".selected-data").text("Select...");
        $('#RatingRightModal_foreign_rating_text').text(null);
        $("#RatingRightModal_foreign_rating").val(null);

        $("#agency_code_error").text("");
        $("#short_long_term_error").text("");
        $("#local_rating_error").text("");
        $("#foreign_rating_error").text("");
        GM.Message.Clear();
    };

    $('form').on('submit', function (e) {
        //alert("test");
        if ($("#issuer_code").val().trim() == "" || $("#issuer_name").val().trim() == "" ||
            $("#issuer_thainame").val().trim() == "" || $("#issuer_type_code").val().trim() == "") {

        } else if ($("#bot_def_code").val().trim() == "" || $("#ktbisic_code").val().trim() == "") {
            $("#issuer-taxandother-form").removeClass("in");
            $("#issuer-taxandother-form").addClass("in");
            document.getElementById("issuer-taxandother-icon").setAttribute("aria-expanded", true);
        } else {
            GM.Issuer.Save(this);
        }
    });

    $("#open_date").on("dp.change", function (e) {
        $('#close_date').data("DateTimePicker").minDate(e.date);
    });

    $("#close_date").on("dp.change", function (e) {
        $('#open_date').data("DateTimePicker").maxDate(e.date);
    });
});