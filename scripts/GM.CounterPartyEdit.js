function Setstatus(datatext) {
    $('#statusdata').val(datatext);
    $("#ddlstatuscounterparty").find(".selected-data").text(datatext);
}

function expandpanel(panelname) {
    if (panelname == "detail") {
        $("#counterparty-detail-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#counterparty-detail-form").attr("aria-expanded", "true");
        $("#counterparty-detail-form").removeAttr("style");
        $("#counterparty-detail-icon").attr("aria-expanded", "true");
        $("#counterparty-detail-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#counterparty-detail-icon").attr("class", "box-header big-head expand-able");
        $("#detail-li").attr("class", "active");
        $("#address-li").removeAttr("class");
        $("#taxandother-li").removeAttr("class");
        $("#payment-li").removeAttr("class");
        $("#identify-li").removeAttr("class");
        $("#rating-li").removeAttr("class");
        $("#haircut-li").removeAttr("class");
        $("#margin-li").removeAttr("class");
        $("#exchange-li").removeAttr("class");
    }
    else if (panelname == "address") {
        $("#counterparty-address-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#counterparty-address-form").attr("aria-expanded", "true");
        $("#counterparty-address-form").removeAttr("style");
        $("#counterparty-address-icon").attr("aria-expanded", "true");
        $("#counterparty-address-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#counterparty-address-icon").attr("class", "box-header big-head expand-able");

        $("#detail-li").removeAttr("class");
        $("#address-li").attr("class", "active");
        $("#taxandother-li").removeAttr("class");
        $("#payment-li").removeAttr("class");
        $("#identify-li").removeAttr("class");
        $("#rating-li").removeAttr("class");
        $("#haircut-li").removeAttr("class");
        $("#margin-li").removeAttr("class");
        $("#exchange-li").removeAttr("class");
    }
    else if (panelname == "taxandother") {
        $("#counterparty-taxandother-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#counterparty-taxandother-form").attr("aria-expanded", "true");
        $("#counterparty-taxandother-form").removeAttr("style");
        $("#counterparty-taxandother-icon").attr("aria-expanded", "true");
        $("#counterparty-taxandother-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#counterparty-taxandother-icon").attr("class", "box-header big-head expand-able");
        $("#detail-li").removeAttr("class");
        $("#address-li").removeAttr("class");
        $("#taxandother-li").attr("class", "active");
        $("#payment-li").removeAttr("class");
        $("#identify-li").removeAttr("class");
        $("#rating-li").removeAttr("class");
        $("#haircut-li").removeAttr("class");
        $("#margin-li").removeAttr("class");
        $("#exchange-li").removeAttr("class");
    }
    else if (panelname == "payment") {
        $("#counterparty-payment-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#counterparty-payment-form").attr("aria-expanded", "true");
        $("#counterparty-payment-form").removeAttr("style");
        $("#counterparty-payment-icon").attr("aria-expanded", "true");
        $("#counterparty-payment-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#counterparty-payment-icon").attr("class", "box-header big-head expand-able");

        $("#detail-li").removeAttr("class");
        $("#address-li").removeAttr("class");
        $("#taxandother-li").removeAttr("class");
        $("#payment-li").attr("class", "active");
        $("#identify-li").removeAttr("class");
        $("#rating-li").removeAttr("class");
        $("#haircut-li").removeAttr("class");
        $("#margin-li").removeAttr("class");
        $("#exchange-li").removeAttr("class");
    }
    else if (panelname == "identify") {
        $("#counterparty-identify-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#counterparty-identify-form").attr("aria-expanded", "true");
        $("#counterparty-identify-form").removeAttr("style");
        $("#counterparty-identify-icon").attr("aria-expanded", "true");
        $("#counterparty-identify-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#counterparty-identify-icon").attr("class", "box-header big-head expand-able");

        $("#detail-li").removeAttr("class");
        $("#address-li").removeAttr("class");
        $("#taxandother-li").removeAttr("class");
        $("#payment-li").removeAttr("class");
        $("#identify-li").attr("class", "active");
        $("#rating-li").removeAttr("class");
        $("#haircut-li").removeAttr("class");
        $("#margin-li").removeAttr("class");
        $("#exchange-li").removeAttr("class");
    }
    else if (panelname == "rating") {
        $("#counterparty-rating-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#counterparty-rating-form").attr("aria-expanded", "true");
        $("#counterparty-rating-form").removeAttr("style");
        $("#counterparty-rating-icon").attr("aria-expanded", "true");
        $("#counterparty-rating-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#counterparty-rating-icon").attr("class", "box-header big-head expand-able");

        $("#detail-li").removeAttr("class");
        $("#address-li").removeAttr("class");
        $("#taxandother-li").removeAttr("class");
        $("#payment-li").removeAttr("class");
        $("#identify-li").removeAttr("class");
        $("#rating-li").attr("class", "active");
        $("#haircut-li").removeAttr("class");
        $("#margin-li").removeAttr("class");
        $("#exchange-li").removeAttr("class");
    }
    else if (panelname == "haircut") {
        $("#counterparty-haircut-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#counterparty-haircut-form").attr("aria-expanded", "true");
        $("#counterparty-haircut-form").removeAttr("style");
        $("#counterparty-haircut-icon").attr("aria-expanded", "true");
        $("#counterparty-haircut-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#counterparty-haircut-icon").attr("class", "box-header big-head expand-able");

        $("#detail-li").removeAttr("class");
        $("#address-li").removeAttr("class");
        $("#taxandother-li").removeAttr("class");
        $("#payment-li").removeAttr("class");
        $("#identify-li").removeAttr("class");
        $("#haircut-li").attr("class", "active");
        $("#margin-li").removeAttr("class");
        $("#exchange-li").removeAttr("class");
    }
    else if (panelname == "exchange") {
        $("#counterparty-exchange-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#counterparty-exchange-form").attr("aria-expanded", "true");
        $("#counterparty-exchange-form").removeAttr("style");
        $("#counterparty-exchange-icon").attr("aria-expanded", "true");
        $("#counterparty-exchange-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#counterparty-exchange-icon").attr("class", "box-header big-head expand-able");

        $("#detail-li").removeAttr("class");
        $("#address-li").removeAttr("class");
        $("#taxandother-li").removeAttr("class");
        $("#payment-li").removeAttr("class");
        $("#identify-li").removeAttr("class");
        $("#haircut-li").removeAttr("class");
        $("#exchange-li").attr("class", "active");
        $("#margin-li").removeAttr("class");
    }
    else if (panelname == "margin") {
        $("#counterparty-margin-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#counterparty-margin-form").attr("aria-expanded", "true");
        $("#counterparty-margin-form").removeAttr("style");
        $("#counterparty-margin-icon").attr("aria-expanded", "true");
        $("#counterparty-margin-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#counterparty-margin-icon").attr("class", "box-header big-head expand-able");

        $("#detail-li").removeAttr("class");
        $("#address-li").removeAttr("class");
        $("#taxandother-li").removeAttr("class");
        $("#payment-li").removeAttr("class");
        $("#identify-li").removeAttr("class");
        $("#rating-li").removeAttr("class");
        $("#haircut-li").removeAttr("class");
        $("#margin-li").attr("class", "active");
        $("#exchange-li").removeAttr("class");
    }
}

$(document).ready(function () {

    $('#counterparty-detail').click(function (e) {
        var expand = $("div#counterparty-detail-icon").attr("aria-expanded");
        if (expand == "true") {
            $("#counterparty-detail-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#counterparty-detail-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }

    });

    $('#counterparty-address').click(function (e) {
        var expand = $("div#counterparty-address-icon").attr("aria-expanded");
        if (expand == "true") {

            $("#counterparty-address-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#counterparty-address-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }

    });

    $('#counterparty-taxandother').click(function (e) {
        var expand = $("div#counterparty-taxandother-icon").attr("aria-expanded");
        if (expand == "true") {

            $("#counterparty-taxandother-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#counterparty-taxandother-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }

    });

    $('#counterparty-margin').click(function (e) {
        var expand = $("div#counterparty-margin-icon").attr("aria-expanded");
        if (expand == "true") {

            $("#counterparty-margin-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#counterparty-margin-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }

    });

    $('#counterparty-payment').click(function (e) {
        var expand = $("div#counterparty-payment-icon").attr("aria-expanded");
        if (expand == "true") {

            $("#counterparty-payment-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#counterparty-payment-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }

    });

    $('#counterparty-identify').click(function (e) {
        var expand = $("div#counterparty-identify-icon").attr("aria-expanded");
        if (expand == "true") {

            $("#counterparty-identify-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#counterparty-identify-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }

    });

    $('#counterparty-rating').click(function (e) {
        var expand = $("div#counterparty-rating-icon").attr("aria-expanded");
        if (expand == "true") {

            $("#counterparty-rating-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#counterparty-rating-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }

    });

    $('#counterparty-haircut').click(function (e) {
        var expand = $("div#counterparty-haircut-icon").attr("aria-expanded");
        if (expand == "true") {
            $("#counterparty-haircut-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
            $("#counterparty-haircut-icon").attr("aria-expanded", "false");
        }
        else {
            $("#counterparty-haircut-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
            $("#counterparty-haircut-icon").attr("aria-expanded", "true");
        }

    });

    $('#counterparty-exchange').click(function (e) {
        var expand = $("div#counterparty-exchange-icon").attr("aria-expanded");
        if (expand == "true") {
            $("#counterparty-exchange-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
            $("#counterparty-exchange-icon").attr("aria-expanded", "false");
        }
        else {
            $("#counterparty-exchange-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
            $("#counterparty-exchange-icon").attr("aria-expanded", "true");
        }

    });

    //#region : Fund Type :
    //Fund Type
    $("#ddl_fundtype").click(function () {
	    var txt_search = $('#txt_fundtype');
	    var data = { datastr: null };
	    GM.Utility.DDLAutoComplete(txt_search, data, null, true);
	    txt_search.val("");
    });

    $('#txt_fundtype').keyup(function () {
	    var data = { datastr: this.value };
	    GM.Utility.DDLAutoComplete(this, data, null, true);
    });
    //End Fund Type
    //#endregion

    //Title Name
    $("#ddl_title_name_add").click(function () {
        var txt_search = $('#txt_title_name_add');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_title_name_add').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Title Name
    
    //Counter Party Type Code
    $("#ddl_counter_party_type_code_add").click(function () {
        var txt_search = $('#txt_counter_party_type_code_add');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_counter_party_type_code_add').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Counter Party Type Code

    //Counter Party Group
    $("#ddl_counter_party_group_id_add").click(function () {
        var txt_search = $('#txt_counter_party_group_id_add');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_counter_party_group_id_add').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Counter Party Group

    //Province
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

            $("#ddl_district_add").removeAttr("disabled");

            $("#ddl_sub_district_add").attr("disabled", "disabled");
            $("#zipcode").attr("readonly", "readonly");
            $("#zipcode").val(null);
        }
    });

    $('#txt_search_province_add').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Province

    //District
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

            $("#ddl_sub_district_add").removeAttr("disabled");
            $("#zipcode").removeAttr("readonly");
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

    //Sub District
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

    //Country
    var checkcountry_id;
    $("#ddl_country_add").click(function () {
        var txt_search = $('#txt_search_country_add');
        checkcountry_id = $('#country_id').val();
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#ul_country_add").on("click", ".searchterm", function (event) {

        if ($('#country_id').val() !== '65') {
            $("#ddl_province_add").attr("disabled", "disabled");
            $("#ddl_province_add").find(".selected-data").text("Select...");
            $("#ddl_district_add").attr("disabled", "disabled");
            $("#ddl_district_add").find(".selected-data").text("Select...");
            $("#ddl_sub_district_add").attr("disabled", "disabled");
            $("#ddl_sub_district_add").find(".selected-data").text("Select...");
            $("#zipcode").attr("readonly", "readonly");

            $("#province_id").val(null);
            $("#district_id").val(null);
            $("#sub_district_id").val(null);
            $("#district_name").text(null);
            $("#sub_district_name").text(null);
            $("#district_name").val(null);
            $("#sub_district_name").val(null);
            $("#zipcode").val(null);

        } else {
            $("#ddl_province_add").removeAttr("disabled"); $.ajax({
                url: "/CounterParty/GetHairCut",
                type: "GET",
                dataType: "JSON",
                success: function (res) {
                    if (res.message === "") {
                        var cur = res.cur;
                        var formula = res.formula;
                        var calType = res.calType;
                        var rowindex = GM.Counterparty.Haircut.Table.find("tr:last").data("id");

                        if (!GM.Counterparty.Haircut.Table.RowAnyCurOnly(cur)) {
                            if (rowindex !== undefined) {
                                rowindex = parseInt(rowindex) + 1;
                            }
                            else { rowindex = 0; }
                            var row = $('<tr data-id="' + rowindex + '" ></tr>');

                            var ColCur = $('<td class="long-data">' + cur +
                                '<input name="Haircut[' + rowindex + '].cur" type="hidden" Isdropdown="true" ' +
                                'textfield="HaircutRightModel.cur_name" textvalue="' + cur + '" ' +
                                'valuefield="HaircutRightModel.cur" value="' + cur + '">' +

                                '<input name="Haircut[' + rowindex + '].cur_name" type="hidden" Isdropdown="true" ' +
                                'textfield="HaircutRightModel.cur_name" textvalue="' + cur + '" ' +
                                'valuefield="HaircutRightModel.cur" value="' + cur + '">' +
                                '</td >');

                            var ColFormula = $('<td class="long-data">' + formula +
                                '<input name="Haircut[' + rowindex + '].formula" type="hidden" Isdropdown="true" ' +
                                'textfield="HaircutRightModel.formula_name" textvalue="' + formula + '" ' +
                                'valuefield="HaircutRightModel.formula" value="' + formula + '">' +
                                '<input name="Haircut[' + rowindex + '].short_long_term_text" type="hidden" Isdropdown="true" ' +
                                'textfield="HaircutRightModel.formula_name" textvalue="' + formula + '" ' +
                                'valuefield="HaircutRightModel.formula" value="' + formula + '">' +
                                '</td > ');

                            var ColCalculateType = $('<td class="long-data">' + calType +
                                '<input name="Haircut[' + rowindex + '].calculate_type" type="hidden" Isdropdown="true" ' +
                                'textfield="HaircutRightModel.calculate_type_name" textvalue="' + calType + '" ' +
                                'valuefield="HaircutRightModel.calculate_type" value="' + calType + '">' +
                                '<input name="Haircut[' + rowindex + '].local_rating_text" type="hidden" Isdropdown="true" ' +
                                'textfield="HaircutRightModel.calculate_type_name" textvalue="' + calType + '" ' +
                                'valuefield="HaircutRightModel.calculate_type" value="' + calType + '">' +
                                '</td>');

                            var ColRowStatus = $('<td><input name="Haircut[' + rowindex + '].rowstatus" Isdropdown="false"  type="hidden" value="create"></td>');

                            var ColAction = $('<td class="action">' +
                                '<button class="btn btn-default btn-round icon-only" type="button" data-id="' + cur + '" data-action="update" onclick="GM.Counterparty.Haircut.Form.Show(this)" >' +
                                '<i class="feather-icon icon-edit"></i>' +
                                '</button > ' +
                                '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' + cur + '" data-action="delete" onclick="GM.Counterparty.Haircut.Form.Show(this)" >' +
                                '<i class="feather-icon icon-trash-2"></i>' +
                                '</button>' +
                                '</td>');

                            row.append(ColCur);
                            row.append(ColFormula);
                            row.append(ColCalculateType);
                            row.append(ColRowStatus);
                            row.append(ColAction);

                            GM.Counterparty.Haircut.Table.append(row);
                            GM.Counterparty.Haircut.Table.find(".empty-data").parent().remove();
                        }

                    }
                    else {
                        console.log(res.message);
                    }
                }
            });

        }
    });

    $('#txt_search_country_add').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Country

    //Corp Type
    //$("#ddl_corp_type_add").click(function () {
    //    var txt_search = $('#txt_search_corp_type_add');
    //    var data = { datastr: null };
    //    GM.Utility.DDLAutoComplete(txt_search, data, null);
    //    txt_search.val("");
    //});

    //$('#txt_search_corp_type_add').keyup(function () {

    //    if (this.value.length > 0) {
    //        var data = { datastr: this.value };
    //        GM.Utility.DDLAutoComplete(this, data, null);
    //    }
    //});
    //End Corp Type

    //ktbisic_code
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
    //End ktbisic_code

    //custodian
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
    //End custodian

    //margin_type
    $("#ddl_margin_type_id_add").click(function () {
        var txt_search = $('#txt_margin_type_id_add');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_margin_type_id_add').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End margin_type

    //margin_in_type
    $("#ddl_margin_in_type_id_add").click(function () {
        var txt_search = $('#txt_margin_in_type_id_add');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_margin_in_type_id_add').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End margin_in_type

    //margin_in_term
    $("#ddl_margin_in_term_id_add").click(function () {
        var txt_search = $('#txt_margin_in_term_id_add');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_margin_in_term_id_add').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End margin_in_term

    //margin_in_term
    $("#ddl_account_type_banklist").click(function () {
        var txt_search = $('#txt_account_type_banklist');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_account_type_banklist').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End margin_in_term

    //margin_in_term
    $("#ddl_bank_name_banklist").click(function () {
        var txt_search = $('#txt_bank_name_banklist');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_bank_name_banklist').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End margin_in_term

    //payment_methodv
    $("#ddl_payment_method_banklist").click(function () {
        var txt_search = $('#txt_payment_method_banklist');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_payment_method_banklist').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End payment_method

    //txt_unique
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
    //End txt_unique


    //identify_type
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
    //End identify_type

    //agency
    $("#ddl_agency").click(function () {
        var txt_search = $('#txt_agency');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#ul_agency").on("click", ".searchterm", function (event) {
        //if ($('#identify_type').val() == "") {
        $("#ddl_localrating").find(".selected-data").text("Select...");
        $("#RatingRightModal_local_rating").val(null);
        $("#RatingRightModal_local_rating_text").text(null);
        $("#RatingRightModal_local_rating_text").val(null);

        $("#ddl_foreignrating").find(".selected-data").text("Select...");
        $("#RatingRightModal_foreign_rating").val(null);
        $("#RatingRightModal_foreign_rating_text").text(null);
        $("#RatingRightModal_foreign_rating_text").val(null);
        //}
    });

    $('#txt_agency').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End agency

    //Local Rating
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
    //End Local Rating

    //Foreign Rating
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
    //End Foreign Rating

    //Start Cur
    $("#ddl_cur").click(function () {
        var txt_search = $('#txt_cur');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_cur').keyup(function () {
        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Cur

    //Start formula
    $("#ddl_formula").click(function () {
        var txt_search = $('#txt_formula');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_formula').keyup(function () {
        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End formula

    //Start formula
    $("#ddl_calculate").click(function () {
        var txt_search = $('#txt_calculate');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_calculate').keyup(function () {
        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End formula

    //Start Cur Exchange
    $("#ddl_cur_exchange").click(function () {
        var txt_search = $('#txt_cur_exchange');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_cur_exchange').keyup(function () {
        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Cur

    $("#ddl_source_type").click(function () {
        var txt_search = $('#txt_source_type');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_source_type').keyup(function () {
        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });

    $("#ul_source_type").on("click", ".searchterm", function (event) {
        $("#ddl_exchange_type").find(".selected-data").text("Select...");
        $("#ExchangeRightModel_exchange_type_name").val(null);
        $("#ExchangeRightModel_exchange_type").val(null);

        if ($("#ExchangeRightModel_source_type").val().length > 0) {
            $("#ddl_exchange_type").removeAttr('disabled');
        }
        else {
            $("#ddl_exchange_type").attr("disabled", "disabled");
        }
    });

    //Start Cur Exchange
    $("#ddl_exchange_type").click(function () {
        var txt_search = $('#txt_exchange_type');
        var data = { datastr: $("#ExchangeRightModel_source_type").val() };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_exchange_type').keyup(function () {
        //if (this.value.length > 0) {
        var data = { datastr: $("#ExchangeRightModel_source_type").val() };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Cur

    $('#minimum_transfer').on("input", function () {
        var input = $('#minimum_transfer').val()
            .replace(/[^\d.]/g, '')             // numbers and decimals only
            .replace(/(^[\d]{24})[\d]/g, '$1')   // not more than 24 digits at the beginning
            .replace(/(\..*)\./g, '$1')         // decimal can't exist more than once
            .replace(/(\.[\d]{6})./g, '$1');    // not more than 6 digits after decimal
        $('#minimum_transfer').val(input);
    });

    $('#minimum_transfer').on("focusout", function () {
        var input = $('#minimum_transfer').val();
        if (input.length) {
            var nStr = input;
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

            x1 = x1.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            $('#minimum_transfer').val(x1 + '.' + x2);
        }
    });

    $('.radio input[id=verify_flag_fo]').change(function () {
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

    $('.radio input[id=netting_flag]').change(function () {
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

    $('.radio input[id=verify_flag_bo]').change(function () {
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

    $('.radio input[id=default_flag]').change(function () {
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

    $('.radio input[id=active_flag]').change(function () {
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

    $('.radio input[id=tax_examtion_flag]').change(function () {
        var current = $(this).val();
        var radioyes = $("[id=tax_examtion_flag][value=true]");
        var radiono = $("[id=tax_examtion_flag][value=false]");
        if (current == "true") {
            radioyes.attr('ischeck', 'true');
            radiono.attr('ischeck', 'false');
            radioyes.attr("checked", "checked");
            radiono.removeAttr("checked");

            $("#tax_examtion_amt").removeAttr('readonly');
            document.getElementById('tax_examtion_amt').value = '';
            document.getElementById('tax_examtion_amt').focus();

            $("#wht_tax_pay").attr('readonly', 'readonly');
            $("#wht_tax_rec").attr('readonly', 'readonly');
            document.getElementById('wht_tax_pay').value = '0.00';
            document.getElementById('wht_tax_rec').value = '0.00';
        } else {
            radioyes.attr('ischeck', 'false');
            radiono.attr('ischeck', 'true');
            radiono.attr("checked", "checked");
            radioyes.removeAttr("checked");

            $("#tax_examtion_amt").attr('readonly', 'readonly');
            document.getElementById('tax_examtion_amt').value = '0.00';

            $("#wht_tax_pay").removeAttr('readonly');
            $("#wht_tax_rec").removeAttr('readonly');
            document.getElementById('wht_tax_pay').value = '';
            document.getElementById('wht_tax_rec').value = '';
            document.getElementById('wht_tax_pay').focus();
        }
    });

    $('.radio input[id=custodian_flag]').change(function () {
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

    $('.radio input[id=borrow_only_flag]').change(function () {
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

    $('.radio input[id=except_margin_flag]').change(function () {
        var current = $(this).val();
        var radioyes = $("[id=except_margin_flag][value=true]");
        var radiono = $("[id=except_margin_flag][value=false]");
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

    $('.radio input[id=int_compound]').change(function () {
        var current = $(this).val();
        var radioyes = $("[id=int_compound][value=true]");
        var radiono = $("[id=int_compound][value=false]");
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

    $('.radio input[id=margin_method]').change(function () {
        var current = $(this).val();
        var radioAsset = $("[id=margin_method][value=ASSET]");
        var radioSec = $("[id=margin_method][value=SEC]");
        if (current == "ASSET") {
            radioAsset.attr('ischeck', 'true');
            radioSec.attr('ischeck', 'false');
            radioAsset.attr("checked", "checked");
            radioSec.removeAttr("checked");
        } else {
            radioAsset.attr('ischeck', 'false');
            radioSec.attr('ischeck', 'true');
            radioSec.attr("checked", "checked");
            radioAsset.removeAttr("checked");
        }
    });

    GM.Counterparty = {};

    //Start Payment
    GM.Counterparty.Payment = {};
    GM.Counterparty.Payment.Table = $("#tbPayment");
    GM.Counterparty.Payment.Table.RowSelected = {};
    GM.Counterparty.Payment.Table.RowEmpty = function () {
        var row = $("<tr></tr>");
        var col = $('<td class="long-data text-center empty-data" style="height:50px;" colspan="3"> No data.</td>');
        row.append(col);
        GM.Counterparty.Payment.Table.append(row);
    };
    GM.Counterparty.Payment.Table.RowAny = function (code, code2, rowid) {

        var rows = GM.Counterparty.Payment.Table.find('tr');
        var IsExits = false;
        var IsExits2 = false;
        var Isdupplicate = false;
        rows.each(function (index, row) {
            IsExits = false;
            IsExits2 = false;
            var rowindex = $(row).data("id");
            var inputs = $(row).find('input,select,textarea');
            if (inputs.length > 0) {
                $.each(inputs,
                    function () {
                        var names = this.name.split('.');
                        if (names[1] == 'nosto_vosto_code' && this.value == code) {

                            if (rowid != rowindex) {
                                IsExits = true;
                            }
                        }
                        if (names[1] == 'payment_method' && this.value == code2) {

                            if (rowid != rowindex) {
                                IsExits2 = true;
                            }
                        }

                        if (IsExits && IsExits2 && names[1] == 'rowstatus' && this.value == 'delete') {
                            IsExits = false;
                            IsExits2 = false;
                        }

                    });
            }
            if (IsExits && IsExits2) {
                Isdupplicate = true;
                return false;
            }
        });

        return Isdupplicate;
    };
    GM.Counterparty.Payment.Form = $("#modal-form-payment");
    GM.Counterparty.Payment.Form.ButtonSave = $('#btnPaymentSave');
    GM.Counterparty.Payment.Form.Show = function (btn) {

        var action = $(btn).data("action");
        var button = $('#btnPaymentSave');

        GM.Counterparty.Payment.Form.Reset();

        switch (action) {
            case 'create':
                GM.Counterparty.Payment.Form.find(".modal-title").html("Create");
                GM.Counterparty.Payment.Form.ButtonSave.data("action", "create");
                GM.Counterparty.Payment.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('OK');
                $('#ddl_account_type_banklist').removeAttr('disabled');
                $('#ddl_bank_name_banklist').removeAttr('disabled');
                $('#ddl_payment_method_banklist').removeAttr('disabled');
                $('#PaymentRightModal_account_number').removeAttr('disabled');
                $('#PaymentRightModal_ca_no').removeAttr('disabled');
                $('#PaymentRightModal_sa_no').removeAttr('disabled');
                $('#PaymentRightModal_participant_id').removeAttr('disabled');
                $('#PaymentRightModal_acc_clearstream').removeAttr('disabled');
                $('#PaymentRightModal_security_agent').removeAttr('disabled');
                $('#PaymentRightModal_intermediary').removeAttr('disabled');
                $('#PaymentRightModal_accWithInst').removeAttr('disabled');
                $('#PaymentRightModal_Beneficial').removeAttr('disabled');
                break;

            case 'update':

                GM.Counterparty.Payment.Table.RowSelected = $(btn).parent().parent();
                GM.Counterparty.Payment.Form.find(".modal-title").html("Update");
                GM.Counterparty.Payment.Form.Initial();
                //$('#ddl_account_type_banklist').removeAttr('disabled');
                //$('#ddl_payment_method_banklist').removeAttr('disabled');

                $('#ddl_bank_name_banklist').removeAttr('disabled');
                $('#PaymentRightModal_account_number').removeAttr('disabled');
                $('#PaymentRightModal_ca_no').removeAttr('disabled');
                $('#PaymentRightModal_sa_no').removeAttr('disabled');
                $('#PaymentRightModal_participant_id').removeAttr('disabled');
                $('#PaymentRightModal_acc_clearstream').removeAttr('disabled');
                $('#PaymentRightModal_security_agent').removeAttr('disabled');
                $('#PaymentRightModal_intermediary').removeAttr('disabled');
                $('#PaymentRightModal_accWithInst').removeAttr('disabled');
                $('#PaymentRightModal_Beneficial').removeAttr('disabled');

                if ($(btn).parent().parent().data("action") != "fromui") {
                    $('#ddl_account_type_banklist').attr('disabled', 'disabled');
                    $('#ddl_payment_method_banklist').attr('disabled', 'disabled');
                }

                GM.Counterparty.Payment.Form.ButtonSave.data("action", "update");
                GM.Counterparty.Payment.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('OK');
                break;

            case 'delete':

                GM.Counterparty.Payment.Table.RowSelected = $(btn).parent().parent();
                GM.Counterparty.Payment.Form.find(".modal-title").html("Delete");
                GM.Counterparty.Payment.Form.Initial();

                $('#ddl_account_type_banklist').attr('disabled', 'disabled');
                $('#ddl_bank_name_banklist').attr('disabled', 'disabled');
                $('#ddl_payment_method_banklist').attr('disabled', 'disabled');
                $('#PaymentRightModal_account_number').attr('disabled', 'disabled');
                $('#PaymentRightModal_ca_no').attr('disabled', 'disabled');
                $('#PaymentRightModal_sa_no').attr('disabled', 'disabled');
                $('#PaymentRightModal_participant_id').attr('disabled', 'disabled');
                $('#PaymentRightModal_acc_clearstream').attr('disabled', 'disabled');
                $('#PaymentRightModal_security_agent').attr('disabled', 'disabled');
                $('#PaymentRightModal_intermediary').attr('disabled', 'disabled');
                $('#PaymentRightModal_accWithInst').attr('disabled', 'disabled');
                $('#PaymentRightModal_Beneficial').attr('disabled', 'disabled');

                GM.Counterparty.Payment.Form.ButtonSave.data("action", "delete");
                GM.Counterparty.Payment.Form.ButtonSave.removeClass('btn-primary').addClass('btn-delete').text('Confirm Delete');
                break;

            default:
                break;
        }

        GM.Counterparty.Payment.Form.modal('toggle');
    };
    GM.Counterparty.Payment.Form.Valid = function () {

        var PaymentRightModal_nosto_vosto_code = $('#PaymentRightModal_nosto_vosto_code');
        var PaymentRightModal_payment_method = $('#PaymentRightModal_payment_method');

        if (PaymentRightModal_nosto_vosto_code.val().trim() === "") {
            return false;
        }
        if (PaymentRightModal_payment_method.val().trim() === "") {
            return false;
        }

        return true;
    };

    GM.Counterparty.Payment.Form.Save = function (btn) {

        var action = $(btn).data("action");

        var IsInValid = GM.Counterparty.Payment.Form.Valid();
        GM.Message.Clear();

        if (IsInValid) {
            switch (action) {
                case 'create':
                    GM.Counterparty.Payment.Table.attr("style", "display: show");
                    var nosto_vosto_code_text = $('#PaymentRightModal_nosto_vosto_name').text();
                    var nosto_vosto_code_val = $("#PaymentRightModal_nosto_vosto_code").val();
                    var bank_name_text = $('#PaymentRightModal_bank_name').text();
                    var bank_name_val = $("#PaymentRightModal_bank_code").val();
                    var payment_method_text = $('#PaymentRightModal_payment_method_name').text();
                    var payment_method_val = $("#PaymentRightModal_payment_method").val();
                    var accountno = $('#PaymentRightModal_account_number').val();
                    var cano = $('#PaymentRightModal_ca_no').val();
                    var sano = $('#PaymentRightModal_sa_no').val();
                    var participantid = $('#PaymentRightModal_participant_id').val();
                    var acc_clearstream = $('#PaymentRightModal_acc_clearstream').val();
                    var security_agent = $('#PaymentRightModal_security_agent').val();
                    var intermediary = $('#PaymentRightModal_intermediary').val();
                    var accWithInst = $('#PaymentRightModal_accWithInst').val();
                    var beneficial = $('#PaymentRightModal_Beneficial').val();
                    //var rowindex = GM.Counterparty.Payment.Table.find("tr:last").data("id");

                    var rowindex = GM.Counterparty.Payment.Table.length;
                    rowindex = rowindex == 1 ? 0 : rowindex;
                    var rowEdit = 0;
                    var rows = GM.Counterparty.Payment.Table.find('tr');
                    rows.each(function (index, row) {

                        var inputs = $(row).find('input,select,textarea');

                        if (inputs.length > 0) {

                            $.each(inputs,
                                function () {

                                    var names = this.name.split('.');
                                    var inputname = 'Payment[' + rowindex + '].' + names[1];

                                    //update new input name
                                    //$(this).attr('name', inputname);
                                    if (names[1] == "rowstatus" && ($(this).val() === "create" || $(this).val() === "update" || $(this).val() == "delete")) {
                                        rowEdit++;
                                    }

                                    if (names[1] == "rowstatus" && $(this).val() !== "create" && $(this).val() !== "update" && $(this).val() !== "delete") {
                                        rowindex++;
                                    }
                                });
                        }
                    });
                    rowindex = rowindex + rowEdit;
                    console.log("rowindex : " + rowindex);
                    if (!GM.Counterparty.Payment.Table.RowAny(nosto_vosto_code_val, payment_method_val)) {

                        if (rowindex != undefined) {
                            rowindex = parseInt(rowindex);
                        } else {
                            rowindex = 0;
                        }
                        var row = $('<tr data-id="' + rowindex + '" data-action="fromui"></tr>');
                        var ColAccountType = $('<td class="long-data">' +
                            nosto_vosto_code_text +
                            '<input name="Payment[' +
                            rowindex +
                            '].nosto_vosto_code" type="hidden" Isdropdown="true" ' +
                            'textfield="PaymentRightModal.nosto_vosto_name" textvalue= "' +
                            nosto_vosto_code_text +
                            '"' +
                            'valuefield="PaymentRightModal.nosto_vosto_code" value= "' +
                            nosto_vosto_code_val +
                            '" >' +
                            '<input name="Payment[' +
                            rowindex +
                            '].nosto_vosto_name" type="hidden" Isdropdown="true" ' +
                            'textfield="PaymentRightModal.nosto_vosto_name" textvalue= "' +
                            nosto_vosto_code_text +
                            '"' +
                            'valuefield="PaymentRightModal.nosto_vosto_code" value= "' +
                            nosto_vosto_code_val +
                            '" >' +
                            '</td > ');

                        var ColBankName = $('<td class="long-data">' +
                            bank_name_text +
                            '<input name="Payment[' +
                            rowindex +
                            '].bank_code" type="hidden" Isdropdown="true" ' +
                            'textfield="PaymentRightModal.bank_name" textvalue="' +
                            bank_name_text +
                            '" ' +
                            'valuefield="PaymentRightModal.bank_code" value="' +
                            bank_name_val +
                            '">' +
                            '<input name="Payment[' +
                            rowindex +
                            '].bank_name" type="hidden" Isdropdown="true" ' +
                            'textfield="PaymentRightModal.bank_name" textvalue="' +
                            bank_name_text +
                            '" ' +
                            'valuefield="PaymentRightModal.bank_code" value="' +
                            bank_name_val +
                            '">' +
                            '</td>');

                        if (bank_name_val == "") {
                            ColBankName = $('<td class="long-data">' +
                                '<input name="Payment[' +
                                rowindex +
                                '].bank_code" type="hidden" Isdropdown="true" ' +
                                'textfield="PaymentRightModal.bank_name" textvalue="' +
                                bank_name_text +
                                '" ' +
                                'valuefield="PaymentRightModal.bank_code" value="' +
                                bank_name_val +
                                '">' +
                                '<input name="Payment[' +
                                rowindex +
                                '].bank_name" type="hidden" Isdropdown="true" ' +
                                'textfield="PaymentRightModal.bank_name" textvalue="' +
                                bank_name_text +
                                '" ' +
                                'valuefield="PaymentRightModal.bank_code" value="' +
                                bank_name_val +
                                '">' +
                                '</td>');
                        }


                        var ColPaymentMathod = $('<td class="long-data">' +
                            payment_method_text +
                            '<input name="Payment[' +
                            rowindex +
                            '].payment_method" type="hidden" Isdropdown="true" ' +
                            'textfield="PaymentRightModal.payment_method_name" textvalue="' +
                            payment_method_text +
                            '" ' +
                            'valuefield="PaymentRightModal.payment_method" value="' +
                            payment_method_val +
                            '">' +
                            '<input name="Payment[' +
                            rowindex +
                            '].payment_method_name" type="hidden" Isdropdown="true" ' +
                            'textfield="PaymentRightModal.payment_method_name" textvalue="' +
                            payment_method_text +
                            '" ' +
                            'valuefield="PaymentRightModal.payment_method" value="' +
                            payment_method_val +
                            '">' +
                            '</td>');

                        var ColAccountNo = $('<td>' +
                            accountno +
                            '<input name="Payment[' +
                            rowindex +
                            '].account_number" Isdropdown="false" type="hidden" value="' +
                            accountno +
                            '"></td>');
                        var ColCaNo = $('<td>' +
                            cano +
                            '<input name="Payment[' +
                            rowindex +
                            '].ca_no" Isdropdown="false"  type="hidden" value="' +
                            cano +
                            '"></td>');
                        var ColSaNo = $('<td>' +
                            sano +
                            '<input name="Payment[' +
                            rowindex +
                            '].sa_no" Isdropdown="false"  type="hidden" value="' +
                            sano +
                            '"></td>');
                        var ColParticipantID = $('<td>' +
                            participantid +
                            '<input name="Payment[' +
                            rowindex +
                            '].participant_id" Isdropdown="false"  type="hidden" value="' +
                            participantid +
                            '"></td>');

                        var ColAccClearstream = $('<td>' + acc_clearstream + '<input name="Payment[' + rowindex + '].acc_clearstream" Isdropdown="false"  type="hidden" value="' + acc_clearstream + '"></td>');
                        var ColSecurityAgent = $('<td>' + security_agent + '<input name="Payment[' + rowindex + '].security_agent" Isdropdown="false"  type="hidden" value="' + security_agent + '"></td>');
                        var ColIntermediary = $('<td>' + intermediary + '<input name="Payment[' + rowindex + '].intermediary" Isdropdown="false" type="hidden" value="' + intermediary + '"></td>');
                        var ColAccWithInst = $('<td>' + accWithInst + '<input name="Payment[' + rowindex + '].accWithInst" Isdropdown="false" type="hidden" value="' + accWithInst + '"></td>');
                        var ColBeneficial = $('<td>' + beneficial + '<input name="Payment[' + rowindex + '].Beneficial" Isdropdown="false" type="hidden" value="' + beneficial + '"></td>');
                        var ColRowStatus = $('<input name="Payment[' +
                            rowindex +
                            '].rowstatus" Isdropdown="false"  type="hidden" value="create">');

                        var ColAction = $('<td class="action" style="min-width: 105px">' +
                            '<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                            nosto_vosto_code_text +
                            '" data-action="update" onclick="GM.Counterparty.Payment.Form.Show(this)" >' +
                            '<i class="feather-icon icon-edit"></i>' +
                            '</button > ' +
                            '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                            nosto_vosto_code_text +
                            '" data-action="delete" onclick="GM.Counterparty.Payment.Form.Show(this)" >' +
                            '<i class="feather-icon icon-trash-2"></i>' +
                            '</button>' +
                            '</td>');

                        row.append(ColAccountType);
                        row.append(ColBankName);
                        row.append(ColPaymentMathod);
                        row.append(ColAccountNo);
                        row.append(ColCaNo);
                        row.append(ColSaNo);
                        row.append(ColParticipantID);
                        row.append(ColAccClearstream);
                        row.append(ColSecurityAgent);
                        row.append(ColIntermediary);
                        row.append(ColAccWithInst);
                        row.append(ColBeneficial);
                        row.append(ColRowStatus);
                        row.append(ColAction);

                        GM.Counterparty.Payment.Table.append(row);
                        GM.Counterparty.Payment.Table.find(".empty-data").parent().remove();
                    } else {
                        GM.Message.Error('.modal-body', "The Data already exists.");
                        return;
                    }

                    break;

                case 'update':
                    var nosto_vosto_code_text = $('#PaymentRightModal_nosto_vosto_name').text();
                    var nosto_vosto_code_val = $("#PaymentRightModal_nosto_vosto_code").val();
                    var bank_name_text = $('#PaymentRightModal_bank_name').text();
                    var bank_name_val = $("#PaymentRightModal_bank_code").val();
                    var payment_method_text = $('#PaymentRightModal_payment_method_name').text();
                    var payment_method_val = $("#PaymentRightModal_payment_method").val();
                    var accountno = $('#PaymentRightModal_account_number').val();
                    var cano = $('#PaymentRightModal_ca_no').val();
                    var sano = $('#PaymentRightModal_sa_no').val();
                    var participantid = $('#PaymentRightModal_participant_id').val();
                    var acc_clearstream = $('#PaymentRightModal_acc_clearstream').val();
                    var security_agent = $('#PaymentRightModal_security_agent').val();
                    var intermediary = $('#PaymentRightModal_intermediary').val();
                    var accWithInst = $('#PaymentRightModal_accWithInst').val();
                    var beneficial = $('#PaymentRightModal_Beneficial').val();
                    var rowindex = GM.Counterparty.Payment.Table.find("tr:last").data("id");

                    GM.Counterparty.Payment.Table.attr("style", "display: show");

                    var row = GM.Counterparty.Payment.Table.RowSelected;
                    var rowindex = row.data("id");
                    var rowfrom = row.data("action");
                    var textstatus = "create";
                    if (rowfrom == "fromdatabase") {
                        textstatus = "update";
                    }

                    if (!GM.Counterparty.Payment.Table.RowAny(nosto_vosto_code_val, payment_method_val, rowindex)) {
                        // if (1 == 1) {
                        var ColAccountType = row.children("td:nth-child(1)");
                        var ColBankName = row.children("td:nth-child(2)");
                        var ColPaymentMathod = row.children("td:nth-child(3)");
                        var ColAccountNo = row.children("td:nth-child(4)");
                        var ColCaNo = row.children("td:nth-child(5)");
                        var ColSaNo = row.children("td:nth-child(6)");
                        var ColParticipantID = row.children("td:nth-child(7)");
                        var ColAccClearstream = row.children("td:nth-child(8)");
                        var ColSecurityAgent = row.children("td:nth-child(9)");
                        var ColIntermediary = row.children("td:nth-child(10)");
                        var ColAccWithInst = row.children("td:nth-child(11)");
                        var ColBeneficial = row.children("td:nth-child(12)");
                        //var ColRowStatus = row.children("td:nth-child(13)");
                        var ColAction = row.children("td:nth-child(13)");

                        ColAccountType.html(nosto_vosto_code_text +
                            '<input name="Payment[' +
                            rowindex +
                            '].nosto_vosto_code" type="hidden" Isdropdown="true" ' +
                            'textfield="PaymentRightModal.nosto_vosto_name" textvalue="' +
                            nosto_vosto_code_text +
                            '" ' +
                            'valuefield="PaymentRightModal.nosto_vosto_code" value="' +
                            nosto_vosto_code_val +
                            '">' +
                            '<input name="Payment[' +
                            rowindex +
                            '].nosto_vosto_name" type="hidden" Isdropdown="true" ' +
                            'textfield="PaymentRightModal.nosto_vosto_name" textvalue="' +
                            nosto_vosto_code_text +
                            '" ' +
                            'valuefield="PaymentRightModal.nosto_vosto_code" value="' +
                            nosto_vosto_code_text +
                            '">');

                        if (bank_name_val == "") {
                            ColBankName.html(
                                '<input name="Payment[' +
                                rowindex +
                                '].bank_code" type="hidden" Isdropdown="true"' +
                                'textfield="PaymentRightModal.bank_name" textvalue="' +
                                bank_name_text +
                                '"' +
                                'valuefield="PaymentRightModal.bank_code" value="' +
                                bank_name_val +
                                '">' +
                                '<input name="Payment[' +
                                rowindex +
                                '].bank_name" type="hidden" Isdropdown="true"' +
                                'textfield="PaymentRightModal.bank_name" textvalue="' +
                                bank_name_text +
                                '"' +
                                'valuefield="PaymentRightModal.bank_code" value="' +
                                bank_name_val +
                                '">');
                        } else {
                            ColBankName.html(bank_name_text +
                                '<input name="Payment[' +
                                rowindex +
                                '].bank_code" type="hidden" Isdropdown="true"' +
                                'textfield="PaymentRightModal.bank_name" textvalue="' +
                                bank_name_text +
                                '"' +
                                'valuefield="PaymentRightModal.bank_code" value="' +
                                bank_name_val +
                                '">' +
                                '<input name="Payment[' +
                                rowindex +
                                '].bank_name" type="hidden" Isdropdown="true"' +
                                'textfield="PaymentRightModal.bank_name" textvalue="' +
                                bank_name_text +
                                '"' +
                                'valuefield="PaymentRightModal.bank_code" value="' +
                                bank_name_val +
                                '">');
                        }

                        ColPaymentMathod.html(payment_method_text +
                            '<input name="Payment[' +
                            rowindex +
                            '].payment_method" type="hidden" Isdropdown="true"' +
                            'textfield="PaymentRightModal.payment_method_name" textvalue="' +
                            payment_method_text +
                            '" ' +
                            'valuefield="PaymentRightModal.payment_method" value="' +
                            payment_method_val +
                            '">' +
                            '<input name="Payment[' +
                            rowindex +
                            '].payment_method_name" type="hidden" Isdropdown="true"' +
                            'textfield="PaymentRightModal.payment_method_name" textvalue="' +
                            payment_method_text +
                            '" ' +
                            'valuefield="PaymentRightModal.payment_method" value="' +
                            payment_method_val +
                            '">');

                        ColAccountNo.html(accountno +
                            '<input name="Payment[' +
                            rowindex +
                            '].account_number" Isdropdown="false" type="hidden" value="' +
                            accountno +
                            '">');
                        ColCaNo.html(cano +
                            '<input name="Payment[' +
                            rowindex +
                            '].ca_no" Isdropdown="false"  type="hidden" value="' +
                            cano +
                            '">');
                        ColSaNo.html(sano +
                            '<input name="Payment[' +
                            rowindex +
                            '].sa_no" Isdropdown="false"  type="hidden" value="' +
                            sano +
                            '">');
                        ColParticipantID.html(participantid +
                            '<input name="Payment[' +
                            rowindex +
                            '].participant_id" Isdropdown="false"  type="hidden" value="' +
                            participantid +
                            '">');

                        ColAccClearstream.html(acc_clearstream + '<input name="Payment[' + rowindex + '].acc_clearstream" Isdropdown="false"  type="hidden" value="' + acc_clearstream + '">');
                        ColSecurityAgent.html(security_agent + '<input name="Payment[' + rowindex + '].security_agent" Isdropdown="false"  type="hidden" value="' + security_agent + '">');
                        ColIntermediary.html(intermediary + '<input name="Payment[' + rowindex + '].intermediary" Isdropdown="false" type="hidden" value="' + intermediary + '">');
                        ColAccWithInst.html(accWithInst + '<input name="Payment[' + rowindex + '].accWithInst" Isdropdown="false" type="hidden" value="' + accWithInst + '">');
                        ColBeneficial.html(beneficial + '<input name="Payment[' + rowindex + '].Beneficial" Isdropdown="false" type="hidden" value="' + beneficial + '">');

                        //ColRowStatus.html('<input name="Payment[' +
                        //    rowindex +
                        //    '].rowstatus" Isdropdown="false"  type="hidden" value="' +
                        //    textstatus +
                        //    '">');

                        $('input[name="Payment[' + rowindex + '].rowstatus"]').val(textstatus);

                        ColAction.html('<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                            bank_name_text +
                            '" data-action="update" onclick="GM.Counterparty.Payment.Form.Show(this)" >' +
                            '<i class="feather-icon icon-edit"></i>' +
                            '</button > ' +
                            '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                            bank_name_text +
                            '" data-action="delete" onclick="GM.Counterparty.Payment.Form.Show(this)" >' +
                            '<i class="feather-icon icon-trash-2"></i>' +
                            '</button>');
                    } else {
                        GM.Message.Error('.modal-body', "The Data already exists.");
                        return;
                    }
                    break;

                case 'delete':
                    var rowselect = GM.Counterparty.Payment.Table.RowSelected;
                    var rowfrom = rowselect.data("action");
                    var rowid = rowselect.data("id");
                    if (rowfrom != "fromdatabase") {
                        GM.Counterparty.Payment.Table.RowSelected.remove();
                    } else {
                        rowselect.attr("style", "display: none");
                        var inputs = $(rowselect).find('input,select,textarea');
                        if (inputs.length > 0) {
                            $.each(inputs,
                                function () {
                                    var names = this.name.split('.');
                                    //update new input name                                
                                    if (names[1] == "rowstatus") {
                                        var inputname = 'Payment[' + rowid + '].' + names[1];
                                        $(this).attr('name', inputname);
                                        $(this).attr("value", "delete");
                                        $(this).attr("type", "hidden");
                                        $(this).text("delete");
                                    }
                                });
                        }
                    }
                    //renew index of input

                    var rowindex = 0;
                    var checkrow = 0;
                    var rowDelete = -1;
                    var rows = GM.Counterparty.Payment.Table.find('tr');
                    rows.each(function (index, row) {

                        var inputs = $(row).find('input,select,textarea');

                        if (inputs.length > 0) {

                            $.each(inputs,
                                function () {

                                    var names = this.name.split('.');
                                    var inputname = 'Payment[' + rowindex + '].' + names[1];

                                    //update new input name
                                    $(this).attr('name', inputname);
                                    if (names[1] == "rowstatus" && $(this).val() != "delete") {
                                        checkrow++;
                                    } else if (names[1] == "rowstatus" && $(this).val() == "delete") {
                                        rowDelete++;
                                    }

                                });

                            rowindex++;
                        }
                    });

                    if (checkrow == 0) {
                        var row = null;
                        if (rowDelete == 0) {
                            row = $('<tr></tr>');
                        } else {
                            row = $('<tr data-id="' + rowDelete + '" ></tr>');
                        }
                        var rowEmpty =
                            '<td class="long-data text-center empty-data" style="height: 50px; " colspan="14"> No data.</td>';
                        row.append(rowEmpty);
                        GM.Counterparty.Payment.Table.append(row);
                    }

                    break;

            }

            GM.Counterparty.Payment.Table.RowSelected = {};
            GM.Counterparty.Payment.Form.modal('toggle');

        }
    };
    GM.Counterparty.Payment.Form.Initial = function () {
        if (GM.Counterparty.Payment.Table.RowSelected) {
            var inputs = $(GM.Counterparty.Payment.Table.RowSelected).find('input,select,textarea');

            if (inputs.length > 0) {
                $.each(inputs, function () {
                    var names = this.name.split('.');
                    if (names[1] == 'bank_name') {
                        return true;
                    }
                    if (this.attributes.isdropdown.value == 'true') {
                        var filedtextid = this.attributes.textfield.value.split('.');
                        if (this.value == "") {
                            $("#PaymentForm span[name='" + filedtextid[1] + "']").text("Select...");
                            $("#PaymentForm input[name='" + this.attributes.textfield.value + "']").val("Select...");
                            $("#PaymentForm input[name='" + this.attributes.textfield.value + "']").text("Select...");
                            $("#PaymentForm input[name='" + this.attributes.valuefield.value + "']").val(this.value);
                        } else {
                            $("#PaymentForm span[name='" + filedtextid[1] + "']").text(this.attributes.textvalue.value);
                            $("#PaymentForm input[name='" + this.attributes.textfield.value + "']").val(this.attributes.textvalue.value);
                            $("#PaymentForm input[name='" + this.attributes.textfield.value + "']").text(this.attributes.textvalue.value);
                            $("#PaymentForm input[name='" + this.attributes.valuefield.value + "']").val(this.value);
                        }

                    } else {
                        //console.log('$(\'#' + names[1] + '\').val(' + this.value + ');');
                        //$('#' + names[1]).val(this.value);   
                        $("#PaymentForm input[name='PaymentRightModal." + names[1] + "']").val(this.value);
                        $("#PaymentForm input[name='PaymentRightModal." + names[1] + "']").text(this.value);

                    }
                });
            }

        }
    };
    GM.Counterparty.Payment.Form.Reset = function () {
        $("#ddl_account_type_banklist").find(".selected-data").text("Select...");
        $('#PaymentRightModal_nosto_vosto_name').text(null);
        $("#PaymentRightModal_nosto_vosto_code").val(null);
        $("#ddl_bank_name_banklist").find(".selected-data").text("Select...");
        $('#PaymentRightModal_bank_name').text(null);
        $("#PaymentRightModal_bank_code").val(null);
        $("#ddl_payment_method_banklist").find(".selected-data").text("Select...");
        $('#PaymentRightModal_payment_method_name').text(null);
        $("#PaymentRightModal_payment_method").val(null);

        $('#PaymentRightModal_account_number').val(null);
        $('#PaymentRightModal_ca_no').val(null);
        $('#PaymentRightModal_sa_no').val(null);
        $('#PaymentRightModal_participant_id').val(null);
        $('#PaymentRightModal_acc_clearstream').val(null);
        $('#PaymentRightModal_security_agent').val(null);
        $('#PaymentRightModal_intermediary').val(null);
        $('#PaymentRightModal_accWithInst').val(null);
        $('#PaymentRightModal_Beneficial').val(null);
        //$('#guarantor_code').css('border-color', "");
        //$('#guarantor_percent').css('border-color', "");

        $("#nosto_vosto_code_error").text("");
        $("#payment_method_error").text("");
        $("#bank_code_error").text("");
        $("#account_number_error").text("");
        $('#PaymentRightModal_account_number').removeClass("input-validation-error");

        GM.Message.Clear();
    };

    $('#btnPaymentCreate').on('click', function () {
        GM.Counterparty.Payment.Form.Show(this);
    });

    $('#btnPaymentSave').on('click', function () {
        GM.Counterparty.Payment.Form.Save(this);
    });
    //End Payment

    //Start Identify
    GM.Counterparty.Identify = {};
    GM.Counterparty.Identify.Table = $("#tbIdentify");
    GM.Counterparty.Identify.Table.RowSelected = {};
    GM.Counterparty.Identify.Table.RowEmpty = function () {
        var row = $("<tr></tr>");
        var col = $('<td class="long-data text-center empty-data" style="height:50px;" colspan="3"> No data.</td>');
        row.append(col);
        GM.Counterparty.Identify.Table.append(row);
    };

    GM.Counterparty.Identify.Table.RowAny = function (code, code2, rowid) {

        var rows = GM.Counterparty.Identify.Table.find('tr');
        var IsExits = false;
        rows.each(function (index, row) {
            IsExits = false;
            var rowindex = $(row).data("id");
            var inputs = $(row).find('input,select,textarea');

            if (inputs.length > 0) {

                $.each(inputs,
                    function () {

                        var names = this.name.split('.');

                        if (names[1] == 'unique_id' && this.value == code) {

                            if (rowid != rowindex) {
                                IsExits = true;
                            }
                        }

                        if (IsExits && names[1] == 'rowstatus' && this.value == 'delete') {
                            IsExits = false;
                        }
                    });
            }
            if (IsExits) {
                return false;
            }
        });

        return IsExits;
    };
    GM.Counterparty.Identify.Form = $("#modal-form-identify");
    GM.Counterparty.Identify.Form.ButtonSave = $('#btnIdentifySave');
    GM.Counterparty.Identify.Form.Show = function (btn) {

        var action = $(btn).data("action");
        var button = $('#btnIdentifySave');

        GM.Counterparty.Identify.Form.Reset();

        switch (action) {
            case 'create':
                GM.Counterparty.Identify.Form.find(".modal-title").html("Create");
                GM.Counterparty.Identify.Form.ButtonSave.data("action", "create");
                GM.Counterparty.Identify.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('OK');
                $('#ddl_unique').removeAttr('disabled');
                $('#ddl_identify_type').removeAttr('disabled');
                $('#IdentifyRightModal_identify_no').removeAttr('disabled');
                $('#IdentifyRightModal_juris_reg_date').removeAttr('disabled');
                $('#IdentifyRightModal_reg_bus_ename').removeAttr('disabled');
                $('#IdentifyRightModal_reg_bus_tname').removeAttr('disabled');
                break;

            case 'update':

                GM.Counterparty.Identify.Table.RowSelected = $(btn).parent().parent();
                GM.Counterparty.Identify.Form.find(".modal-title").html("Update");
                GM.Counterparty.Identify.Form.Initial();

                $('#ddl_unique').removeAttr('disabled');
                $('#ddl_identify_type').removeAttr('disabled');
                $('#IdentifyRightModal_identify_no').removeAttr('disabled');
                $('#IdentifyRightModal_juris_reg_date').removeAttr('disabled');
                $('#IdentifyRightModal_reg_bus_ename').removeAttr('disabled');
                $('#IdentifyRightModal_reg_bus_tname').removeAttr('disabled');

                if ($(btn).parent().parent().data("action") != "fromui") {
                    $('#ddl_unique').attr('disabled', 'disabled');
                    $('#ddl_identify_type').attr('disabled', 'disabled');
                }

                GM.Counterparty.Identify.Form.ButtonSave.data("action", "update");
                GM.Counterparty.Identify.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('Save Identify');
                break;

            case 'delete':

                GM.Counterparty.Identify.Table.RowSelected = $(btn).parent().parent();
                GM.Counterparty.Identify.Form.find(".modal-title").html("Delete");
                GM.Counterparty.Identify.Form.Initial();

                $('#ddl_unique').attr('disabled', 'disabled');
                $('#ddl_identify_type').attr('disabled', 'disabled');
                $('#IdentifyRightModal_identify_no').attr('disabled', 'disabled');
                $('#IdentifyRightModal_juris_reg_date').attr('disabled', 'disabled');
                $('#IdentifyRightModal_reg_bus_ename').attr('disabled', 'disabled');
                $('#IdentifyRightModal_reg_bus_tname').attr('disabled', 'disabled');

                GM.Counterparty.Identify.Form.ButtonSave.data("action", "delete");
                GM.Counterparty.Identify.Form.ButtonSave.removeClass('btn-primary').addClass('btn-delete').text('Confirm Delete');
                break;

            default:
                break;
        }

        GM.Counterparty.Identify.Form.modal('toggle');
    };
    GM.Counterparty.Identify.Form.Valid = function () {

        var IdentifyRightModal_unique_id = $('#IdentifyRightModal_unique_id');
        var IdentifyRightModal_identify_type = $('#IdentifyRightModal_identify_type');
        var IdentifyRightModal_identify_no = $('#IdentifyRightModal_identify_no');

        if (IdentifyRightModal_unique_id.val().trim() == "") {
            return false;
        }
        if (IdentifyRightModal_identify_type.val().trim() == "") {
            return false;
        }
        if (IdentifyRightModal_identify_no.val().trim() == "") {
            return false;
        }

        return true;

    };
    GM.Counterparty.Identify.Form.Save = function (btn) {

        var action = $(btn).data("action");
        GM.Message.Clear();
        var isValid = GM.Counterparty.Identify.Form.Valid();
        if (isValid) {
            switch (action) {
                case 'create':
                    GM.Counterparty.Identify.Table.attr("style", "display: show");
                    var unique_text = $('#IdentifyRightModal_unique_name').text();
                    var unique_val = $("#IdentifyRightModal_unique_id").val();
                    var identify_type_text = $('#IdentifyRightModal_identify_type_text').text();
                    var identify_type_val = $("#IdentifyRightModal_identify_type").val();
                    var identify_no = $('#IdentifyRightModal_identify_no').val();
                    var juris_reg_date = $("#IdentifyRightModal_juris_reg_date").val();
                    var reg_bus_ename = $('#IdentifyRightModal_reg_bus_ename').val();
                    var reg_bus_tname = $('#IdentifyRightModal_reg_bus_tname').val();
                    //var rowindex = GM.Counterparty.Identify.Table.find("tr:last").data("id");
                    var rowindex = GM.Counterparty.Identify.Table.length;
                    rowindex = rowindex == 1 ? 0 : rowindex;
                    var rowEdit = 0;
                    var rows = GM.Counterparty.Identify.Table.find('tr');
                    rows.each(function (index, row) {
                        var inputs = $(row).find('input,select,textarea');
                        if (inputs.length > 0) {
                            $.each(inputs,
                                function () {
                                    var names = this.name.split('.');
                                    var inputname = 'Identify[' + rowindex + '].' + names[1];
                                    //update new input name
                                    //$(this).attr('name', inputname);
                                    if (names[1] == "rowstatus" && ($(this).val() === "create" || $(this).val() === "update" || $(this).val() == "delete")) {
                                        rowEdit++;
                                    }

                                    if (names[1] == "rowstatus" && $(this).val() !== "create" && $(this).val() !== "update" && $(this).val() !== "delete") {
                                        rowindex++;
                                    }
                                });
                        }
                    });
                    rowindex = rowindex + rowEdit;
                    console.log("rowindex : " + rowindex);
                    if (!GM.Counterparty.Identify.Table.RowAny(unique_val)) {
                        if (rowindex != undefined) {
                            rowindex = parseInt(rowindex);// + 1;
                        } else {
                            rowindex = 0;
                        }
                        var row = $('<tr data-id="' + rowindex + '" data-action="fromui"></tr>');
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

                        var ColRowStatus = $('<td><input name="Identify[' +
                            rowindex +
                            '].rowstatus" Isdropdown="false"  type="hidden" value="create"></td>');

                        var ColAction = $('<td class="action">' +
                            '<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                            unique_text +
                            '" data-action="update" onclick="GM.Counterparty.Identify.Form.Show(this)" >' +
                            '<i class="feather-icon icon-edit"></i>' +
                            '</button > ' +
                            '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                            unique_text +
                            '" data-action="delete" onclick="GM.Counterparty.Identify.Form.Show(this)" >' +
                            '<i class="feather-icon icon-trash-2"></i>' +
                            '</button>' +
                            '</td>');

                        row.append(ColUniqueID);
                        row.append(ColIdentifyType);
                        row.append(ColIdentifyNo);
                        row.append(ColJurisRegDate);
                        row.append(ColRegBusEname);
                        row.append(ColRegBustname);
                        row.append(ColRowStatus);
                        row.append(ColAction);

                        GM.Counterparty.Identify.Table.append(row);
                        GM.Counterparty.Identify.Table.find(".empty-data").parent().remove();
                    } else {
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
                    var rowindex = GM.Counterparty.Identify.Table.find("tr:last").data("id");

                    var row = GM.Counterparty.Identify.Table.RowSelected;
                    var rowindex = row.data("id");

                    GM.Counterparty.Identify.Table.attr("style", "display: show");

                    var row = GM.Counterparty.Identify.Table.RowSelected;
                    var rowindex = row.data("id");
                    var rowfrom = row.data("action");
                    var textstatus = "create";
                    if (rowfrom == "fromdatabase") {
                        textstatus = "update";
                    }

                    if (!GM.Counterparty.Identify.Table.RowAny(unique_val, null, rowindex)) {
                        // if (1 == 1) {
                        var ColUniqueID = row.children("td:nth-child(1)");
                        var ColIdentifyType = row.children("td:nth-child(2)");
                        var ColIdentifyNo = row.children("td:nth-child(3)");
                        var ColJurisRegDate = row.children("td:nth-child(4)");
                        var ColRegBusEname = row.children("td:nth-child(5)");
                        var ColRegBusTname = row.children("td:nth-child(6)");
                        var ColRowStatus = row.children("td:nth-child(7)");
                        var ColAction = row.children("td:nth-child(8)");

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

                        ColRowStatus.html('<input name="Identify[' +
                            rowindex +
                            '].rowstatus" Isdropdown="false"  type="hidden" value="' +
                            textstatus +
                            '">');

                        ColAction.html('<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                            unique_text +
                            '" data-action="update" onclick="GM.Counterparty.Identify.Form.Show(this)" >' +
                            '<i class="feather-icon icon-edit"></i>' +
                            '</button > ' +
                            '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                            unique_text +
                            '" data-action="delete" onclick="GM.Counterparty.Identify.Form.Show(this)" >' +
                            '<i class="feather-icon icon-trash-2"></i>' +
                            '</button>');
                    } else {
                        GM.Message.Error('.modal-body', "The Data already exists.");
                        return;
                    }
                    break;

                case 'delete':

                    var rowselect = GM.Counterparty.Identify.Table.RowSelected;
                    var rowfrom = rowselect.data("action");
                    var rowid = rowselect.data("id");
                    if (rowfrom != "fromdatabase") {
                        GM.Counterparty.Identify.Table.RowSelected.remove();
                    } else {
                        rowselect.attr("style", "display: none");
                        var inputs = $(rowselect).find('input,select,textarea');
                        if (inputs.length > 0) {
                            $.each(inputs,
                                function () {
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
                    var rows = GM.Counterparty.Identify.Table.find('tr');
                    var rowindex = 0;
                    var checkrow = 0;
                    var rowDelete = 0;
                    rows.each(function (index, row) {

                        var inputs = $(row).find('input,select,textarea');

                        if (inputs.length > 0) {

                            $.each(inputs,
                                function () {

                                    var names = this.name.split('.');
                                    var inputname = 'Identify[' + rowindex + '].' + names[1];
                                    //update new input name
                                    $(this).attr('name', inputname);

                                    if (names[1] == "rowstatus" && $(this).val() != "delete") {
                                        checkrow++;
                                    } else if (names[1] == "rowstatus" && $(this).val() == "delete") {
                                        rowDelete++;
                                    }

                                });

                            rowindex++;
                        }

                    });

                    if (checkrow == 0) {
                        var row = null;
                        if (rowDelete == 0) {
                            row = $('<tr></tr>');
                        } else {
                            row = $('<tr data-id="' + rowDelete + '" ></tr>');
                        }
                        var rowEmpty =
                            '<td class="long-data text-center empty-data" style="height: 50px; " colspan="7"> No data.</td>';
                        row.append(rowEmpty);
                        GM.Counterparty.Identify.Table.append(row);
                    }
                    break;
            }

            GM.Counterparty.Identify.Table.RowSelected = {};

            GM.Counterparty.Identify.Form.modal('toggle');
        }
    };
    GM.Counterparty.Identify.Form.Initial = function () {
        if (GM.Counterparty.Identify.Table.RowSelected) {
            var inputs = $(GM.Counterparty.Identify.Table.RowSelected).find('input,select,textarea');

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
    GM.Counterparty.Identify.Form.Reset = function () {

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

        $("#unique_id_error").text("");
        $("#identify_type_error").text("");
        $("#identify_no_error").text("");

        $('#IdentifyRightModal_identify_no').removeClass("input-validation-error");

        GM.Message.Clear();
    };

    $('#btnIdentifyCreate').on('click', function () {
        GM.Counterparty.Identify.Form.Show(this);
    });

    $('#btnIdentifySave').on('click', function () {
        GM.Counterparty.Identify.Form.Save(this);
    });
    //End Identify

    //#region Start Rating

    //Start Rating
    GM.Counterparty.Rating = {};
    GM.Counterparty.Rating.Table = $("#tbRating");
    GM.Counterparty.Rating.Table.RowSelected = {};
    GM.Counterparty.Rating.Table.RowEmpty = function () {
        var row = $("<tr></tr>");
        var col = $('<td class="long-data text-center empty-data" style="height:50px;" colspan="3"> No data.</td>');
        row.append(col);
        GM.Counterparty.Rating.Table.append(row);
    };
    GM.Counterparty.Rating.Table.RowAny = function (code, code2, rowid) {

        var rows = GM.Counterparty.Rating.Table.find('tr');
        var IsExits = false;
        rows.each(function (index, row) {
            IsExits = false;
            var rowindex = $(row).data("id");
            var inputs = $(row).find('input,select,textarea');

            if (inputs.length > 0) {

                $.each(inputs,
                    function () {

                        var names = this.name.split('.');

                        if (names[1] == 'agency_code' && this.value == code) {

                            if (rowid != rowindex) {
                                IsExits = true;
                            }
                        }

                        if (IsExits && names[1] == 'rowstatus' && this.value == 'delete') {
                            IsExits = false;
                        }
                    });
            }
            if (IsExits) {
                return false;
            }
        });

        return IsExits;
    };
    GM.Counterparty.Rating.Form = $("#modal-form-rating");
    GM.Counterparty.Rating.Form.ButtonSave = $('#btnRatingSave');
    GM.Counterparty.Rating.Form.Show = function (btn) {

        var action = $(btn).data("action");
        var button = $('#btnRatingSave');

        GM.Counterparty.Rating.Form.Reset();

        switch (action) {
            case 'create':
                GM.Counterparty.Rating.Form.find(".modal-title").html("Create");
                GM.Counterparty.Rating.Form.ButtonSave.data("action", "create");
                GM.Counterparty.Rating.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('OK');
                $('#ddl_agency').removeAttr('disabled');
                $('#ddl_term').removeAttr('disabled');
                $('#ddl_localrating').removeAttr('disabled');
                $('#ddl_foreignrating').removeAttr('disabled');

                break;

            case 'update':

                GM.Counterparty.Rating.Table.RowSelected = $(btn).parent().parent();
                GM.Counterparty.Rating.Form.find(".modal-title").html("Update");
                GM.Counterparty.Rating.Form.Initial();
                $('#ddl_agency').removeAttr('disabled');
                $('#ddl_term').removeAttr('disabled');
                $('#ddl_localrating').removeAttr('disabled');
                $('#ddl_foreignrating').removeAttr('disabled');

                if ($(btn).parent().parent().data("action") != "fromui") {
                    $('#ddl_agency').attr('disabled', 'disabled');
                }

                GM.Counterparty.Rating.Form.ButtonSave.data("action", "update");
                GM.Counterparty.Rating.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('OK');
                break;

            case 'delete':

                GM.Counterparty.Rating.Table.RowSelected = $(btn).parent().parent();
                GM.Counterparty.Rating.Form.find(".modal-title").html("Delete");
                GM.Counterparty.Rating.Form.Initial();

                $('#ddl_agency').attr('disabled', 'disabled');
                $('#ddl_term').attr('disabled', 'disabled');
                $('#ddl_localrating').attr('disabled', 'disabled');
                $('#ddl_foreignrating').attr('disabled', 'disabled');

                GM.Counterparty.Rating.Form.ButtonSave.data("action", "delete");
                GM.Counterparty.Rating.Form.ButtonSave.removeClass('btn-primary').addClass('btn-delete').text('Confirm Delete');
                break;

            default:
                break;
        }

        GM.Counterparty.Rating.Form.modal('toggle');
    };
    GM.Counterparty.Rating.Form.Valid = function () {

        var RatingRightModal_agency_code = $('#RatingRightModal_agency_code');
        var RatingRightModal_short_long_term = $('#RatingRightModal_short_long_term');
        var RatingRightModal_local_rating = $('#RatingRightModal_local_rating');
        var RatingRightModal_foreign_rating = $('#RatingRightModal_foreign_rating');

        if (RatingRightModal_agency_code.val().trim() == "") {
            return false;
        }

        if (RatingRightModal_short_long_term.val().trim() == "") {
            return false;
        }

        if (RatingRightModal_local_rating.val().trim() == "") {
            return false;
        }

        if (RatingRightModal_foreign_rating.val().trim() == "") {
            return false;
        }

        return true;

    };
    GM.Counterparty.Rating.Form.Save = function (btn) {

        var action = $(btn).data("action");
        GM.Message.Clear();
        var isValid = GM.Counterparty.Rating.Form.Valid();
        if (isValid) {
            switch (action) {
                case 'create':
                    GM.Counterparty.Rating.Table.attr("style", "display: show");
                    var agency_text = $('#RatingRightModal_agency_name').text();
                    var agency_val = $("#RatingRightModal_agency_code").val();
                    var short_long_term_text = $('#RatingRightModal_short_long_term_text').text();
                    var short_long_term_val = $("#RatingRightModal_short_long_term").val();
                    var local_rating_text = $('#RatingRightModal_local_rating_text').text();
                    var local_rating_val = $("#RatingRightModal_local_rating").val();
                    var foreign_rating_text = $('#RatingRightModal_foreign_rating_text').text();
                    var foreign_rating_val = $("#RatingRightModal_foreign_rating").val();

                    //var rowindex = GM.Counterparty.Rating.Table.find("tr:last").data("id");
                    var rowindex = GM.Counterparty.Rating.Table.length;
                    rowindex = rowindex == 1 ? 0 : rowindex;
                    var rowEdit = 0;
                    var rows = GM.Counterparty.Rating.Table.find('tr');
                    rows.each(function (index, row) {
                        var inputs = $(row).find('input,select,textarea');
                        if (inputs.length > 0) {
                            $.each(inputs,
                                function () {
                                    var names = this.name.split('.');
                                    var inputname = 'Rating[' + rowindex + '].' + names[1];
                                    //update new input name
                                    //$(this).attr('name', inputname);
                                    if (names[1] == "rowstatus" && ($(this).val() === "create" || $(this).val() === "update" || $(this).val() == "delete")) {
                                        rowEdit++;
                                    }
                                    if (names[1] == "rowstatus" && $(this).val() !== "create" && $(this).val() !== "update" && $(this).val() !== "delete") {
                                        rowindex++;
                                    }
                                });
                        }
                    });
                    rowindex = rowindex + rowEdit;
                    console.log("rowindex : " + rowindex);
                    if (!GM.Counterparty.Rating.Table.RowAny(agency_val)) {
                        if (rowindex != undefined) {
                            rowindex = parseInt(rowindex);// + 1;
                        } else {
                            rowindex = 0;
                        }
                        var row = $('<tr data-id="' + rowindex + '" data-action="fromui"></tr>');

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
                            agency_val +
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
                            short_long_term_val +
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
                            local_rating_val +
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
                            foreign_rating_val +
                            '">' +
                            '</td > ');

                        var ColRowStatus = $('<td><input name="Rating[' +
                            rowindex +
                            '].rowstatus" Isdropdown="false"  type="hidden" value="create"></td>');

                        var ColAction = $('<td class="action">' +
                            '<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                            agency_val +
                            '" data-action="update" onclick="GM.Counterparty.Rating.Form.Show(this)" >' +
                            '<i class="feather-icon icon-edit"></i>' +
                            '</button > ' +
                            '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                            agency_val +
                            '" data-action="delete" onclick="GM.Counterparty.Rating.Form.Show(this)" >' +
                            '<i class="feather-icon icon-trash-2"></i>' +
                            '</button>' +
                            '</td>');

                        row.append(ColAgencyCode);
                        row.append(ColShortLongTerm);
                        row.append(ColLocalRating);
                        row.append(ColForeignRating);
                        row.append(ColRowStatus);
                        row.append(ColAction);

                        GM.Counterparty.Rating.Table.append(row);
                        GM.Counterparty.Rating.Table.find(".empty-data").parent().remove();

                        //swal("Good job!", "Successfully saved", "success");
                    } else {
                        //swal("Error", "The data guarantor already exists.", "error");
                        GM.Message.Error('.modal-body', "The Data already exists.");
                        return;
                    }

                    break;

                case 'update':
                    var agency_text = $('#RatingRightModal_agency_name').text();
                    var agency_val = $("#RatingRightModal_agency_code").val();
                    var short_long_term_text = $('#RatingRightModal_short_long_term_text').text();
                    var short_long_term_val = $("#RatingRightModal_short_long_term").val();
                    var local_rating_text = $('#RatingRightModal_local_rating_text').text();
                    var local_rating_val = $("#RatingRightModal_local_rating").val();
                    var foreign_rating_text = $('#RatingRightModal_foreign_rating_text').text();
                    var foreign_rating_val = $("#RatingRightModal_foreign_rating").val();
                    var rowindex = GM.Counterparty.Rating.Table.find("tr:last").data("id");

                    GM.Counterparty.Rating.Table.attr("style", "display: show");

                    var row = GM.Counterparty.Rating.Table.RowSelected;
                    var rowindex = row.data("id");
                    var rowfrom = row.data("action");
                    var textstatus = "create";
                    if (rowfrom == "fromdatabase") {
                        textstatus = "update";
                    }

                    if (!GM.Counterparty.Rating.Table.RowAny(agency_val, null, rowindex)) {
                        // if (1 == 1) {
                        var ColAgencyCode = row.children("td:nth-child(1)");
                        var ColShortLongTerm = row.children("td:nth-child(2)");
                        var ColLocalRating = row.children("td:nth-child(3)");
                        var ColForeignRating = row.children("td:nth-child(4)");
                        var ColRowStatus = row.children("td:nth-child(5)");
                        var ColAction = row.children("td:nth-child(6)");

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
                            agency_val +
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
                            short_long_term_val +
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
                            local_rating_val +
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
                            foreign_rating_val +
                            '">');

                        ColRowStatus.html('<input name="Rating[' +
                            rowindex +
                            '].rowstatus" Isdropdown="false"  type="hidden" value="' +
                            textstatus +
                            '">');

                        ColAction.html('<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                            agency_val +
                            '" data-action="update" onclick="GM.Counterparty.Rating.Form.Show(this)" >' +
                            '<i class="feather-icon icon-edit"></i>' +
                            '</button > ' +
                            '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                            agency_val +
                            '" data-action="delete" onclick="GM.Counterparty.Rating.Form.Show(this)" >' +
                            '<i class="feather-icon icon-trash-2"></i>' +
                            '</button>');

                        // swal("Good job!", "Successfully saved", "success");
                    } else {
                        //swal("Error", "The data guarantor already exists.", "error");
                        GM.Message.Error('.modal-body', "The Data already exists.");
                        return;
                    }
                    break;

                case 'delete':

                    var rowselect = GM.Counterparty.Rating.Table.RowSelected;
                    var rowfrom = rowselect.data("action");
                    var rowid = rowselect.data("id");
                    if (rowfrom != "fromdatabase") {
                        GM.Counterparty.Rating.Table.RowSelected.remove();
                    } else {
                        rowselect.attr("style", "display: none");
                        var inputs = $(rowselect).find('input,select,textarea');
                        if (inputs.length > 0) {
                            $.each(inputs,
                                function () {
                                    var names = this.name.split('.');
                                    //update new input name                                
                                    if (names[1] == "rowstatus") {
                                        var inputname = 'Rating[' + rowid + '].' + names[1];
                                        $(this).attr('name', inputname);
                                        $(this).attr("value", "delete");
                                        $(this).attr("type", "hidden");
                                        $(this).text("delete");
                                    }
                                });
                        }
                    }
                    //renew index of input
                    var rows = GM.Counterparty.Rating.Table.find('tr');
                    var rowindex = 0;
                    var checkrow = 0;
                    var rowDelete = 0;
                    rows.each(function (index, row) {
                        var inputs = $(row).find('input,select,textarea');
                        if (inputs.length > 0) {
                            $.each(inputs,
                                function () {
                                    var names = this.name.split('.');
                                    var inputname = 'Rating[' + rowindex + '].' + names[1];
                                    //update new input name
                                    $(this).attr('name', inputname);

                                    if (names[1] == "rowstatus" && $(this).val() != "delete") {
                                        checkrow++;
                                    } else if (names[1] == "rowstatus" && $(this).val() == "delete") {
                                        rowDelete++;
                                    }

                                });
                            rowindex++;
                        }
                    });

                    if (checkrow == 0) {
                        var row = null;
                        if (rowDelete == 0) {
                            row = $('<tr></tr>');
                        } else {
                            row = $('<tr data-id="' + rowDelete + '" ></tr>');
                        }
                        var rowEmpty =
                            '<td class="long-data text-center empty-data" style="height: 50px; " colspan="7"> No data.</td>';
                        row.append(rowEmpty);
                        GM.Counterparty.Rating.Table.append(row);
                    }

                    break;

            }

            GM.Counterparty.Rating.Table.RowSelected = {};

            GM.Counterparty.Rating.Form.modal('toggle');
        }

    };
    GM.Counterparty.Rating.Form.Initial = function () {
        GM.Message.Clear();
        if (GM.Counterparty.Rating.Table.RowSelected) {
            var inputs = $(GM.Counterparty.Rating.Table.RowSelected).find('input,select,textarea');

            if (inputs.length > 0) {
                $.each(inputs, function () {
                    var names = this.name.split('.');
                    if (this.attributes.isdropdown.value === 'true') {
                        var filedtextid = this.attributes.textfield.value.split('.');
                        var elm_filedname = this.attributes.textfield.value.split('.')[0] + '.' + this.name.split('.')[1];
                        $("#RatingForm span[name='" + filedtextid[1] + "']").text(this.attributes.textvalue.value);
                        $("#RatingForm input[name='" + elm_filedname + "']").val(this.attributes.value.value);
                        $("#RatingForm input[name='" + elm_filedname + "']").text(this.attributes.textvalue.value);
                        //$("#RatingForm input[name='" + elm_filedname + "']").val(this.value);
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
    GM.Counterparty.Rating.Form.Reset = function () {

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

    $('#btnRatingCreate').on('click', function () {
        GM.Counterparty.Rating.Form.Show(this);
    });

    $('#btnRatingSave').on('click', function () {
        GM.Counterparty.Rating.Form.Save(this);
    });

    //#endregion

    //Start Haircut
    GM.Counterparty.Haircut = {};
    GM.Counterparty.Haircut.Table = $("#tbHaircut");
    GM.Counterparty.Haircut.Table.RowSelected = {};
    GM.Counterparty.Haircut.Table.RowEmpty = function () {
        var row = $("<tr></tr>");
        var col = $('<td class="long-data text-center empty-data" style="height:50px;" colspan="3"> No data.</td>');
        row.append(col);
        GM.Counterparty.Haircut.Table.append(row);
    };

    GM.Counterparty.Haircut.Table.RowAnyCurOnly = function (code, rowid) {

        var rows = GM.Counterparty.Haircut.Table.find('tr');
        var rowExitCur = 0;
        var IsExits = false;
        var Isdupplicate = false;
        rows.each(function (index, row) {

            var rowindex = $(row).data("id");
            var inputs = $(row).find('input,select,textarea');
            if (inputs.length > 0) {
                var status = inputs[6].value;
                $.each(inputs,
                    function () {

                        var names = this.name.split('.');

                        if (names[1] == 'cur' && this.value == code) {

                            if (rowid != rowindex && status != 'delete') {
                                Isdupplicate = true;
                                return false;
                            }
                        }
                    });
            }
        });

        return Isdupplicate;
    };

    GM.Counterparty.Haircut.Table.RowAny = function (code, code2, rowid) {

        var rows = GM.Counterparty.Haircut.Table.find('tr');
        var rowExitCur = 0;
        var IsExits = false;
        var IsExits2 = false;
        var Isdupplicate = false;
        rows.each(function (index, row) {
            var IsExits = false;
            var IsExits2 = false;
            var rowindex = $(row).data("id");
            var inputs = $(row).find('input,select,textarea');

            if (inputs.length > 0) {

                $.each(inputs,
                    function () {

                        var names = this.name.split('.');

                        if (names[1] == 'cur' && this.value == code) {

                            if (rowid != rowindex) {
                                IsExits = true;
                                rowExitCur = rowindex;
                            }
                        }

                        if (names[1] == 'formula' && IsExits && this.value == code2) {

                            if (rowid != rowindex && rowExitCur == rowindex) {
                                IsExits2 = true;
                            }
                        }

                        if (IsExits && IsExits2 && names[1] == 'rowstatus' && this.value == 'delete') {
                            IsExits = false;
                            IsExits2 = false;
                        }
                    });
            }
            if (IsExits && IsExits2) {
                Isdupplicate = true;
                return false;
            }
        });

        return Isdupplicate;
    };
    GM.Counterparty.Haircut.Form = $("#modal-form-haircut");
    GM.Counterparty.Haircut.Form.ButtonSave = $('#btnHaircutSave');
    GM.Counterparty.Haircut.Form.Show = function (btn) {

        var action = $(btn).data("action");
        var button = $('#btnHaircutSave');

        GM.Counterparty.Haircut.Form.Reset();

        switch (action) {
            case 'create':
                GM.Counterparty.Haircut.Form.find(".modal-title").html("Create");
                GM.Counterparty.Haircut.Form.ButtonSave.data("action", "create");
                GM.Counterparty.Haircut.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('+ Add Haircut');

                $("#ddl_cur").removeAttr("disabled");
                $("#ddl_formula").removeAttr("disabled");
                $("#ddl_calculate").removeAttr("disabled");

                break;

            case 'update':

                GM.Counterparty.Haircut.Table.RowSelected = $(btn).parent().parent();
                GM.Counterparty.Haircut.Form.find(".modal-title").html("Update");
                GM.Counterparty.Haircut.Form.Initial();

                $("#ddl_cur").attr("disabled", "disabled");
                $("#ddl_formula").attr("disabled", "disabled");
                $("#ddl_calculate").removeAttr("disabled");

                if ($(btn).parent().parent().data("action") != "fromui") {
                    $('#ddl_cur').attr('disabled', 'disabled');
                    $('#ddl_formula').attr('disabled', 'disabled');
                }

                GM.Counterparty.Haircut.Form.ButtonSave.data("action", "update");
                GM.Counterparty.Haircut.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('Save Haircut');
                break;

            case 'delete':

                GM.Counterparty.Haircut.Table.RowSelected = $(btn).parent().parent();
                GM.Counterparty.Haircut.Form.find(".modal-title").html("Delete");
                GM.Counterparty.Haircut.Form.Initial();

                $("#ddl_cur").attr("disabled", "disabled");
                $("#ddl_formula").attr("disabled", "disabled");
                $("#ddl_calculate").attr("disabled", "disabled");

                GM.Counterparty.Haircut.Form.ButtonSave.data("action", "delete");
                GM.Counterparty.Haircut.Form.ButtonSave.removeClass('btn-primary').addClass('btn-delete').text('Confirm Delete');
                break;

            default:
                break;
        }

        GM.Counterparty.Haircut.Form.modal('toggle');
    };
    GM.Counterparty.Haircut.Form.Valid = function () {

        var HaircutRightModel_cur = $('#HaircutRightModel_cur');
        var HaircutRightModel_formula = $('#HaircutRightModel_formula');
        var HaircutRightModel_calculate_type = $('#HaircutRightModel_calculate_type');

        if (HaircutRightModel_cur.val().trim() == "") {
            return false;
        }

        if (HaircutRightModel_formula.val().trim() == "") {
            return false;
        }

        if (HaircutRightModel_calculate_type.val().trim() == "") {
            return false;
        }

        return true;
    };

    GM.Counterparty.Haircut.Form.Save = function (btn) {

        var action = $(btn).data("action");
        GM.Message.Clear();
        var isValid = GM.Counterparty.Haircut.Form.Valid();
        if (isValid) {
            switch (action) {
                case 'create':
                    GM.Counterparty.Haircut.Table.attr("style", "display: show");
                    var cur_name = $('#HaircutRightModel_cur_name').text();
                    var cur = $("#HaircutRightModel_cur").val();
                    var formula_name = $('#HaircutRightModel_formula_name').text();
                    var formula = $("#HaircutRightModel_formula").val();
                    var calculate_type_name = $('#HaircutRightModel_calculate_type_name').text();
                    var calculate_type = $("#HaircutRightModel_calculate_type").val();

                    //var rowindex = GM.Counterparty.Haircut.Table.find("tr:last").data("id");
                    var rowindex = GM.Counterparty.Haircut.Table.length;
                    rowindex = rowindex == 1 ? 0 : rowindex;
                    var rowEdit = 0;
                    var rows = GM.Counterparty.Haircut.Table.find('tr');
                    rows.each(function (index, row) {
                        var inputs = $(row).find('input,select,textarea');
                        if (inputs.length > 0) {
                            $.each(inputs,
                                function () {
                                    var names = this.name.split('.');
                                    var inputname = 'Haircut[' + rowindex + '].' + names[1];
                                    //update new input name
                                    //$(this).attr('name', inputname);
                                    if (names[1] == "rowstatus" && ($(this).val() === "create" || $(this).val() === "update" || $(this).val() == "delete")) {
                                        rowEdit++;
                                    }
                                    if (names[1] == "rowstatus" && $(this).val() !== "create" && $(this).val() !== "update" && $(this).val() !== "delete") {
                                        rowindex++;
                                    }
                                });
                        }
                    });
                    rowindex = rowindex + rowEdit;
                    console.log("rowindex : " + rowindex);
                    if (!GM.Counterparty.Haircut.Table.RowAny(cur, formula)) {
                        if (rowindex != undefined) {
                            rowindex = parseInt(rowindex);// + 1;
                        } else {
                            rowindex = 0;
                        }
                        var row = $('<tr data-id="' + rowindex + '" ></tr>');

                        var ColCur = $('<td class="long-data">' +
                            cur_name +
                            '<input name="Haircut[' +
                            rowindex +
                            '].cur" type="hidden" Isdropdown="true" ' +
                            'textfield="HaircutRightModel.cur_name" textvalue="' +
                            cur_name +
                            '" ' +
                            'valuefield="HaircutRightModel.cur" value="' +
                            cur +
                            '">' +
                            '<input name="Haircut[' +
                            rowindex +
                            '].cur_name" type="hidden" Isdropdown="true" ' +
                            'textfield="HaircutRightModel.cur_name" textvalue="' +
                            cur_name +
                            '" ' +
                            'valuefield="HaircutRightModel.cur" value="' +
                            cur_name +
                            '">' +
                            '</td >');

                        var ColFormula = $('<td class="long-data">' +
                            formula_name +
                            '<input name="Haircut[' +
                            rowindex +
                            '].formula" type="hidden" Isdropdown="true" ' +
                            'textfield="HaircutRightModel.formula_name" textvalue="' +
                            formula_name +
                            '" ' +
                            'valuefield="HaircutRightModel.formula" value="' +
                            formula +
                            '">' +
                            '<input name="Haircut[' +
                            rowindex +
                            '].short_long_term_text" type="hidden" Isdropdown="true" ' +
                            'textfield="HaircutRightModel.formula_name" textvalue="' +
                            formula_name +
                            '" ' +
                            'valuefield="HaircutRightModel.formula" value="' +
                            formula_name +
                            '">' +
                            '</td > ');

                        var ColCalculateType = $('<td class="long-data">' +
                            calculate_type_name +
                            '<input name="Haircut[' +
                            rowindex +
                            '].calculate_type" type="hidden" Isdropdown="true" ' +
                            'textfield="HaircutRightModel.calculate_type_name" textvalue="' +
                            calculate_type_name +
                            '" ' +
                            'valuefield="HaircutRightModel.calculate_type" value="' +
                            calculate_type +
                            '">' +
                            '<input name="Haircut[' +
                            rowindex +
                            '].local_rating_text" type="hidden" Isdropdown="true" ' +
                            'textfield="HaircutRightModel.calculate_type_name" textvalue="' +
                            calculate_type_name +
                            '" ' +
                            'valuefield="HaircutRightModel.calculate_type" value="' +
                            calculate_type_name +
                            '">' +
                            '</td>');

                        var ColRowStatus = $('<td><input name="Haircut[' +
                            rowindex +
                            '].rowstatus" Isdropdown="false"  type="hidden" value="create"></td>');

                        var ColAction = $('<td class="action">' +
                            '<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                            cur +
                            '" data-action="update" onclick="GM.Counterparty.Haircut.Form.Show(this)" >' +
                            '<i class="feather-icon icon-edit"></i>' +
                            '</button > ' +
                            '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                            cur +
                            '" data-action="delete" onclick="GM.Counterparty.Haircut.Form.Show(this)" >' +
                            '<i class="feather-icon icon-trash-2"></i>' +
                            '</button>' +
                            '</td>');

                        row.append(ColCur);
                        row.append(ColFormula);
                        row.append(ColCalculateType);
                        row.append(ColRowStatus);
                        row.append(ColAction);

                        GM.Counterparty.Haircut.Table.append(row);
                        GM.Counterparty.Haircut.Table.find(".empty-data").parent().remove();
                    } else {
                        GM.Message.Error('.modal-body', "The Data already exists.");
                        return;
                    }
                    break;

                case 'update':
                    var cur_name = $('#HaircutRightModel_cur_name').text();
                    var cur = $("#HaircutRightModel_cur").val();
                    var formula_name = $('#HaircutRightModel_formula_name').text();
                    var formula = $("#HaircutRightModel_formula").val();
                    var calculate_type_name = $('#HaircutRightModel_calculate_type_name').text();
                    var calculate_type = $("#HaircutRightModel_calculate_type").val();

                    var rowindex = GM.Counterparty.Haircut.Table.find("tr:last").data("id");

                    GM.Counterparty.Haircut.Table.attr("style", "display: show");

                    var row = GM.Counterparty.Haircut.Table.RowSelected;
                    var rowindex = row.data("id");
                    var rowfrom = row.data("action");
                    var textstatus = "create";
                    if (rowfrom == "fromdatabase") {
                        textstatus = "update";
                    }

                    if (!GM.Counterparty.Haircut.Table.RowAny(cur, formula, rowindex)) {
                        // if (1 == 1) {
                        var ColCur = row.children("td:nth-child(1)");
                        var ColFormula = row.children("td:nth-child(2)");
                        var ColCalculateType = row.children("td:nth-child(3)");
                        var ColRowStatus = row.children("td:nth-child(4)");
                        var ColAction = row.children("td:nth-child(5)");

                        ColCur.html(cur_name +
                            '<input name="Haircut[' +
                            rowindex +
                            '].cur" type="hidden" Isdropdown="true" ' +
                            'textfield="HaircutRightModel.cur_name" textvalue="' +
                            cur_name +
                            '" ' +
                            'valuefield="HaircutRightModel.cur" value="' +
                            cur +
                            '">' +
                            '<input name="Haircut[' +
                            rowindex +
                            '].cur_name" type="hidden" Isdropdown="true" ' +
                            'textfield="HaircutRightModel.cur_name" textvalue="' +
                            cur_name +
                            '" ' +
                            'valuefield="HaircutRightModel.cur" value="' +
                            cur_name +
                            '">');

                        ColFormula.html(formula_name +
                            '<input name="Haircut[' +
                            rowindex +
                            '].formula" type="hidden" Isdropdown="true" ' +
                            'textfield="HaircutRightModel.formula_name" textvalue="' +
                            formula_name +
                            '" ' +
                            'valuefield="HaircutRightModel.formula" value="' +
                            formula +
                            '">' +
                            '<input name="Haircut[' +
                            rowindex +
                            '].short_long_term_text" type="hidden" Isdropdown="true" ' +
                            'textfield="HaircutRightModel.formula_name" textvalue="' +
                            formula_name +
                            '" ' +
                            'valuefield="HaircutRightModel.formula" value="' +
                            formula_name +
                            '">');

                        ColCalculateType.html(calculate_type_name +
                            '<input name="Haircut[' +
                            rowindex +
                            '].calculate_type" type="hidden" Isdropdown="true" ' +
                            'textfield="HaircutRightModel.calculate_type_name" textvalue="' +
                            calculate_type_name +
                            '" ' +
                            'valuefield="HaircutRightModel.calculate_type" value="' +
                            calculate_type +
                            '">' +
                            '<input name="Haircut[' +
                            rowindex +
                            '].local_rating_text" type="hidden" Isdropdown="true" ' +
                            'textfield="HaircutRightModel.calculate_type_name" textvalue="' +
                            calculate_type_name +
                            '" ' +
                            'valuefield="HaircutRightModel.calculate_type" value="' +
                            calculate_type_name +
                            '">');

                        ColRowStatus.html('<input name="Haircut[' +
                            rowindex +
                            '].rowstatus" Isdropdown="false"  type="hidden" value="' +
                            textstatus +
                            '">');

                        ColAction.html('<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                            cur +
                            '" data-action="update" onclick="GM.Counterparty.Haircut.Form.Show(this)" >' +
                            '<i class="feather-icon icon-edit"></i>' +
                            '</button > ' +
                            '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                            cur +
                            '" data-action="delete" onclick="GM.Counterparty.Haircut.Form.Show(this)" >' +
                            '<i class="feather-icon icon-trash-2"></i>' +
                            '</button>');
                    } else {
                        GM.Message.Error('.modal-body', "The Data already exists.");
                        return;
                    }
                    break;

                case 'delete':

                    var rowselect = GM.Counterparty.Haircut.Table.RowSelected;
                    var rowfrom = rowselect.data("action");
                    var rowid = rowselect.data("id");
                    if (rowfrom != "fromdatabase") {
                        GM.Counterparty.Haircut.Table.RowSelected.remove();
                    } else {
                        rowselect.attr("style", "display: none");
                        var inputs = $(rowselect).find('input,select,textarea');
                        if (inputs.length > 0) {
                            $.each(inputs,
                                function () {
                                    var names = this.name.split('.');
                                    //update new input name                                
                                    if (names[1] == "rowstatus") {
                                        var inputname = 'Haircut[' + rowid + '].' + names[1];
                                        $(this).attr('name', inputname);
                                        $(this).attr("value", "delete");
                                        $(this).attr("type", "hidden");
                                        $(this).text("delete");
                                    }
                                });
                        }
                    }
                    //renew index of input
                    var rows = GM.Counterparty.Haircut.Table.find('tr');
                    var rowindex = 0;
                    var checkrow = 0;
                    var rowDelete = 0;
                    rows.each(function (index, row) {
                        var inputs = $(row).find('input,select,textarea');
                        if (inputs.length > 0) {
                            $.each(inputs,
                                function () {
                                    var names = this.name.split('.');
                                    var inputname = 'Haircut[' + rowindex + '].' + names[1];
                                    //update new input name
                                    $(this).attr('name', inputname);

                                    if (names[1] == "rowstatus" && $(this).val() != "delete") {
                                        checkrow++;
                                    } else if (names[1] == "rowstatus" && $(this).val() == "delete") {
                                        rowDelete++;
                                    }

                                });
                            rowindex++;
                        }
                    });

                    if (checkrow == 0) {
                        var row = null;
                        if (rowDelete == 0) {
                            row = $('<tr></tr>');
                        } else {
                            row = $('<tr data-id="' + rowDelete + '" ></tr>');
                        }
                        var rowEmpty =
                            '<td class="long-data text-center empty-data" style="height: 50px; " colspan="7"> No data.</td>';
                        row.append(rowEmpty);
                        GM.Counterparty.Haircut.Table.append(row);
                    }
                    break;
            }

            GM.Counterparty.Haircut.Table.RowSelected = {};
            GM.Counterparty.Haircut.Form.modal('toggle');
        }
    };
    GM.Counterparty.Haircut.Form.Initial = function () {
        GM.Message.Clear();
        if (GM.Counterparty.Haircut.Table.RowSelected) {
            var inputs = $(GM.Counterparty.Haircut.Table.RowSelected).find('input,select,textarea');

            if (inputs.length > 0) {
                $.each(inputs, function () {
                    var names = this.name.split('.');
                    if (this.attributes.isdropdown.value == 'true') {
                        if (this.attributes.textvalue != null) {
                            var filedtextid = this.attributes.textfield.value.split('.');

                            $("#HaircutForm span[name='" + filedtextid[1] + "']").text(this.attributes.textvalue.value);
                            $("#HaircutForm input[name='" + this.attributes.textfield.value + "']").val(this.attributes.textvalue.value);
                            $("#HaircutForm input[name='" + this.attributes.textfield.value + "']").text(this.attributes.textvalue.value);
                            $("#HaircutForm input[name='" + this.attributes.valuefield.value + "']").val(this.value);

                            if (filedtextid[1] == "cur") {
                                $("#ddl_cur").find(".selected-data").text(this.value);
                                $("#ddl_cur").attr("disabled", "disabled");
                                $('#HaircutRightModel_cur_name').text(this.value);
                                $("#HaircutRightModel_cur").val(this.value);
                            } else if (filedtextid[1] == "formula") {
                                $("#ddl_formula").find(".selected-data").text(this.value);
                                $("#ddl_formula").attr("disabled", "disabled");
                                $('#HaircutRightModel_formula_name').text(this.value);
                                $("#HaircutRightModel_formula").val(this.value);
                            } else if (filedtextid[1] == "calculate_type") {
                                $("#ddl_calculate").find(".selected-data").text(this.value);
                                $("#ddl_calculate").attr("disabled", "disabled");
                                $('#HaircutRightModel_calculate_type_name').text(this.value);
                                $("#HaircutRightModel_calculate_type").val(this.value);
                            }
                        }

                    } else {
                        $("#HaircutForm input[name='HaircutRightModel." + names[1] + "']").val(this.value);
                        $("#HaircutForm input[name='HaircutRightModel." + names[1] + "']").text(this.value);
                    }
                });
            }

        }
    };
    GM.Counterparty.Haircut.Form.Reset = function () {

        $("#ddl_cur").find(".selected-data").text("Select...");
        $("#ddl_cur").removeAttr("disabled");
        $('#HaircutRightModel_cur_name').text(null);
        $("#HaircutRightModel_cur").val(null);

        $("#ddl_formula").find(".selected-data").text("Select...");
        $("#ddl_formula").removeAttr("disabled");
        $('#HaircutRightModel_formula_name').text(null);
        $("#HaircutRightModel_formula").val(null);

        $("#ddl_calculate").find(".selected-data").text("Select...");
        $("#ddl_calculate").removeAttr("disabled");
        $('#HaircutRightModel_calculate_type_name').text(null);
        $("#HaircutRightModel_calculate_type").val(null);

        $("#cur_error").text("");
        $("#formula_error").text("");
        $("#calculate_type_error").text("");

        GM.Message.Clear();
    };

    $('#btnHaircutCreate').on('click', function () {
        GM.Counterparty.Haircut.Form.Show(this);
    });

    $('#btnHaircutSave').on('click', function () {
        GM.Counterparty.Haircut.Form.Save(this);
    });

    //End HairCut

    //Start Exchange Rate

    GM.Counterparty.Exchange = {};
    GM.Counterparty.Exchange.Table = $("#tbExchange");
    GM.Counterparty.Exchange.Table.RowSelected = {};
    GM.Counterparty.Exchange.Table.RowEmpty = function () {
        var row = $("<tr></tr>");
        var col = $('<td class="long-data text-center empty-data" style="height:50px;" colspan="3"> No data.</td>');
        row.append(col);
        GM.Counterparty.Exchange.Table.append(row);
    };
    GM.Counterparty.Exchange.Table.RowAny = function (cur, rowid) {

        var rows = GM.Counterparty.Exchange.Table.find('tr');
        var IsExits = false;
        rows.each(function (index, row) {
            IsExits = false;
            var rowindex = $(row).data("id");
            var inputs = $(row).find('input,select,textarea');

            if (inputs.length > 0) {

                $.each(inputs,
                    function () {

                        var names = this.name.split('.');

                        if (names[1] === 'cur' && this.value === cur) {

                            if (rowid !== rowindex) {
                                IsExits = true;
                                //return false
                            }
                        }
                        //if (names[1] === 'source_type' && this.value === source_type) {

                        //    if (rowid !== rowindex) {
                        //        IsExits = true;
                        //        //return false
                        //    }
                        //}
                        //if (names[1] === 'exchange_type' && this.value === exchange_type) {

                        //    if (rowid !== rowindex) {
                        //        IsExits = true;
                        //        //return false
                        //    }
                        //}
                        if (IsExits && names[1] === 'rowstatus' && this.value === 'delete') {
                            IsExits = false;
                        }
                    });
            }
        });

        return IsExits;
    };
    GM.Counterparty.Exchange.Form = $("#modal-form-exchange");
    GM.Counterparty.Exchange.Form.ButtonSave = $('#btnExchangeSave');
    GM.Counterparty.Exchange.Form.Show = function (btn) {

        var action = $(btn).data("action");
        var button = $('#btnExchangeSave');

        GM.Counterparty.Exchange.Form.Reset();

        switch (action) {
            case 'create':
                GM.Counterparty.Exchange.Form.find(".modal-title").html("Create");
                GM.Counterparty.Exchange.Form.ButtonSave.data("action", "create");
                GM.Counterparty.Exchange.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('+ Add Exchange Rate');

                $("#ddl_cur_exchange").removeAttr("disabled");
                $("#ddl_source_type").removeAttr("disabled");
                $("#ddl_exchange_type").attr("disabled", "disabled");

                break;

            case 'update':

                GM.Counterparty.Exchange.Table.RowSelected = $(btn).parent().parent();
                GM.Counterparty.Exchange.Form.find(".modal-title").html("Update");
                GM.Counterparty.Exchange.Form.Initial();
                var rowselect = GM.Counterparty.Exchange.Table.RowSelected;
                var rowfrom = rowselect.data("action");

                if (rowfrom === "fromdatabase") {
                    $("#ddl_cur_exchange").attr("disabled", "disabled");
                }

                $("#ddl_source_type").removeAttr("disabled");
                $("#ddl_exchange_type").removeAttr("disabled");

                GM.Counterparty.Exchange.Form.ButtonSave.data("action", "update");
                GM.Counterparty.Exchange.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('Save Exchange Rate');
                break;

            case 'delete':

                GM.Counterparty.Exchange.Table.RowSelected = $(btn).parent().parent();
                GM.Counterparty.Exchange.Form.find(".modal-title").html("Delete");
                GM.Counterparty.Exchange.Form.Initial();

                GM.Counterparty.Exchange.Form.ButtonSave.data("action", "delete");
                GM.Counterparty.Exchange.Form.ButtonSave.removeClass('btn-primary').addClass('btn-delete').text('Confirm Delete');
                break;

            default:
                break;
        }

        GM.Counterparty.Exchange.Form.modal('toggle');
    };
    GM.Counterparty.Exchange.Form.Valid = function () {

        var ExchangeRightModel_cur = $('#ExchangeRightModel_cur');
        var ExchangeRightModel_source_type = $('#ExchangeRightModel_source_type');
        var ExchangeRightModel_exchange_type = $('#ExchangeRightModel_exchange_type');

        if (ExchangeRightModel_cur.val().trim() == "") {
            return false;
        }

        if (ExchangeRightModel_source_type.val().trim() == "") {
            return false;
        }

        if (ExchangeRightModel_exchange_type.val().trim() == "") {
            return false;
        }

        return true;
    };

    GM.Counterparty.Exchange.Form.Save = function (btn) {

        var action = $(btn).data("action");
        GM.Message.Clear();
        var isValid = GM.Counterparty.Exchange.Form.Valid();
        if (isValid) {

            var cur = $("#ExchangeRightModel_cur").val();
            var cur_name = $('#ExchangeRightModel_cur_name').text();

            var source_type = $("#ExchangeRightModel_source_type").val();
            var source_type_name = $('#ExchangeRightModel_source_type_name').text();

            var exchange_type = $("#ExchangeRightModel_exchange_type").val();
            var exchange_type_name = $('#ExchangeRightModel_exchange_type_name').text();

            switch (action) {
                case 'create':
                    GM.Counterparty.Exchange.Table.attr("style", "display: show");
                    var rowindex = GM.Counterparty.Exchange.Table.length;
                    rowindex = rowindex === 1 ? 0 : rowindex;
                    var rowEdit = 0;
                    var rows = GM.Counterparty.Exchange.Table.find('tr');
                    rows.each(function (index, row) {
                        var inputs = $(row).find('input,select,textarea');
                        if (inputs.length > 0) {
                            $.each(inputs,
                                function () {
                                    var names = this.name.split('.');
                                    var inputname = 'Exchange[' + rowindex + '].' + names[1];
                                    //update new input name
                                    //$(this).attr('name', inputname);
                                    if (names[1] == "rowstatus" && ($(this).val() === "create" || $(this).val() === "update" || $(this).val() == "delete")) {
                                        rowEdit++;
                                    }
                                    if (names[1] == "rowstatus" && $(this).val() !== "create" && $(this).val() !== "update" && $(this).val() !== "delete") {
                                        rowindex++;
                                    }
                                });
                        }
                    });
                    rowindex = rowindex + rowEdit;
                    console.log("rowindex : " + rowindex);
                    if (!GM.Counterparty.Exchange.Table.RowAny(cur)) {
                        if (rowindex !== undefined) {
                            rowindex = parseInt(rowindex);// + 1;
                        } else {
                            rowindex = 0;
                        }

                        var row = $('<tr data-id="' + rowindex + '" data-action="fromui"></tr>');

                        var ColCur = $('<td class="long-data">' +
                            cur_name +
                            '<input name="Exchange[' +
                            rowindex +
                            '].cur" type="hidden" Isdropdown="true" ' +
                            'textfield="ExchangeRightModel.cur_name" textvalue="' +
                            cur_name +
                            '" ' +
                            'valuefield="ExchangeRightModel.cur" value="' +
                            cur +
                            '">' +
                            '<input name="Exchange[' +
                            rowindex +
                            '].cur_name" type="hidden" Isdropdown="true" ' +
                            'textfield="ExchangeRightModel.cur_name" textvalue="' +
                            cur_name +
                            '" ' +
                            'valuefield="ExchangeRightModel.cur" value="' +
                            cur +
                            '">' +
                            '</td >');

                        var ColFormula = $('<td class="long-data">' +
                            source_type_name +
                            '<input name="Exchange[' +
                            rowindex +
                            '].source_type" type="hidden" Isdropdown="true" ' +
                            'textfield="ExchangeRightModel.source_type" textvalue="' +
                            source_type_name +
                            '" ' +
                            'valuefield="ExchangeRightModel.source_type" value="' +
                            source_type +
                            '">' +
                            '<input name="Exchange[' +
                            rowindex +
                            '].source_type_name" type="hidden" Isdropdown="true" ' +
                            'textfield="ExchangeRightModel.source_type_name" textvalue="' +
                            source_type_name +
                            '" ' +
                            'valuefield="ExchangeRightModel.source_type" value="' +
                            source_type_name +
                            '">' +
                            '</td > ');

                        var ColCalculateType = $('<td class="long-data">' +
                            exchange_type_name +
                            '<input name="Exchange[' +
                            rowindex +
                            '].exchange_type" type="hidden" Isdropdown="true" ' +
                            'textfield="ExchangeRightModel.exchange_type_name" textvalue="' +
                            exchange_type_name +
                            '" ' +
                            'valuefield="ExchangeRightModel.exchange_type" value="' +
                            exchange_type +
                            '">' +
                            '<input name="Exchange[' +
                            rowindex +
                            '].exchange_type_name" type="hidden" Isdropdown="true" ' +
                            'textfield="ExchangeRightModel.exchange_type_name" textvalue="' +
                            exchange_type_name +
                            '" ' +
                            'valuefield="ExchangeRightModel.exchange_type" value="' +
                            exchange_type_name +
                            '">' +
                            '</td>');

                        var ColRowStatus = $('<td><input name="Exchange[' +
                            rowindex +
                            '].rowstatus" Isdropdown="false"  type="hidden" value="create"></td>');

                        var ColAction = $('<td class="action">' +
                            '<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                            cur +
                            '" data-action="update" onclick="GM.Counterparty.Exchange.Form.Show(this)" >' +
                            '<i class="feather-icon icon-edit"></i>' +
                            '</button > ' +
                            '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                            cur +
                            '" data-action="delete" onclick="GM.Counterparty.Exchange.Form.Show(this)" >' +
                            '<i class="feather-icon icon-trash-2"></i>' +
                            '</button>' +
                            '</td>');

                        row.append(ColCur);
                        row.append(ColFormula);
                        row.append(ColCalculateType);
                        row.append(ColRowStatus);
                        row.append(ColAction);

                        GM.Counterparty.Exchange.Table.append(row);
                        GM.Counterparty.Exchange.Table.find(".empty-data").parent().remove();
                    } else {
                        GM.Message.Error('.modal-body', "The Data already exists.");
                        return;
                    }
                    break;

                case 'update':

                    // Change Logic To Delete Row Seleced And Create New Row From New Data
                    //#region :: New Step ::

                    var rowselect = GM.Counterparty.Exchange.Table.RowSelected;
                    var rowfrom = rowselect.data("action");
                    var rowid = rowselect.data("id");
                    var textstatus = "create";
                    if (rowfrom == "fromdatabase") {
                        textstatus = "update";
                    }

                    if (!GM.Counterparty.Exchange.Table.RowAny(cur, rowid)) {

                        if (rowfrom === "fromdatabase") {

                            //#region delete action

                            rowselect.attr("style", "display: none");
                            var inputs = $(rowselect).find('input,select,textarea');
                            if (inputs.length > 0) {
                                $.each(inputs,
                                    function () {
                                        var names = this.name.split('.');
                                        //update new input name                                
                                        if (names[1] === "rowstatus") {
                                            var inputname = 'Exchange[' + rowid + '].' + names[1];
                                            $(this).attr('name', inputname);
                                            $(this).attr("value", "delete");
                                            $(this).attr("type", "hidden");
                                            $(this).text("delete");
                                        }
                                    });
                            }

                            //renew index of input
                            rows = GM.Counterparty.Exchange.Table.find('tr');
                            rowindex = 0;
                            var checkrow = 0;
                            var rowDelete = 0;
                            rows.each(function (index, row) {
                                var inputs = $(row).find('input,select,textarea');
                                if (inputs.length > 0) {
                                    $.each(inputs,
                                        function () {
                                            var names = this.name.split('.');
                                            var inputname = 'Exchange[' + rowindex + '].' + names[1];
                                            //update new input name
                                            $(this).attr('name', inputname);

                                            if (names[1] === "rowstatus" && $(this).val() !== "delete") {
                                                checkrow++;
                                            } else if (names[1] === "rowstatus" && $(this).val() === "delete") {
                                                rowDelete++;
                                            }

                                        });
                                    rowindex++;
                                }
                            });

                            if (checkrow === 0) {
                                row = null;
                                if (rowDelete === 0) {
                                    row = $('<tr></tr>');
                                } else {
                                    row = $('<tr data-id="' + rowDelete + '" ></tr>');
                                }
                                var rowEmpty =
                                    '<td class="long-data text-center empty-data" style="height: 50px; " colspan="7"> No data.</td>';
                                row.append(rowEmpty);
                                GM.Counterparty.Exchange.Table.append(row);
                            }



                            //#endregion

                            //#region create action
                            GM.Counterparty.Exchange.Table.attr("style", "display: show");

                            rowindex = GM.Counterparty.Exchange.Table.length;
                            rowindex = rowindex === 1 ? 0 : rowindex;

                            rowEdit = 0;
                            rows = GM.Counterparty.Exchange.Table.find('tr');
                            rows.each(function (index, row) {
                                var inputs = $(row).find('input,select,textarea');
                                if (inputs.length > 0) {
                                    $.each(inputs,
                                        function () {
                                            var names = this.name.split('.');
                                            var inputname = 'Exchange[' + rowindex + '].' + names[1];
                                            //update new input name
                                            //$(this).attr('name', inputname);
                                            if (names[1] === "rowstatus" && ($(this).val() === "create" || $(this).val() === "update" || $(this).val() === "delete")) {
                                                rowEdit++;
                                            }
                                            if (names[1] === "rowstatus" && $(this).val() !== "create" && $(this).val() !== "update" && $(this).val() !== "delete") {
                                                rowindex++;
                                            }
                                        });
                                }
                            });

                            rowindex = rowindex + rowEdit;

                            console.log("rowindex : " + rowindex);

                            if (!GM.Counterparty.Exchange.Table.RowAny(cur)) {
                                if (rowindex !== undefined) {
                                    rowindex = parseInt(rowindex);// + 1;
                                } else {
                                    rowindex = 0;
                                }

                                row = $('<tr data-id="' + rowindex + '" data-action="fromdatabase"></tr>');

                                ColCur = $('<td class="long-data">' +
                                    cur_name +
                                    '<input name="Exchange[' +
                                    rowindex +
                                    '].cur" type="hidden" Isdropdown="true" ' +
                                    'textfield="ExchangeRightModel.cur_name" textvalue="' +
                                    cur_name +
                                    '" ' +
                                    'valuefield="ExchangeRightModel.cur" value="' +
                                    cur +
                                    '">' +
                                    '<input name="Exchange[' +
                                    rowindex +
                                    '].cur_name" type="hidden" Isdropdown="true" ' +
                                    'textfield="ExchangeRightModel.cur_name" textvalue="' +
                                    cur_name +
                                    '" ' +
                                    'valuefield="ExchangeRightModel.cur" value="' +
                                    cur +
                                    '">' +
                                    '</td >');

                                ColFormula = $('<td class="long-data">' +
                                    source_type_name +
                                    '<input name="Exchange[' +
                                    rowindex +
                                    '].source_type" type="hidden" Isdropdown="true" ' +
                                    'textfield="ExchangeRightModel.source_type" textvalue="' +
                                    source_type_name +
                                    '" ' +
                                    'valuefield="ExchangeRightModel.source_type" value="' +
                                    source_type +
                                    '">' +
                                    '<input name="Exchange[' +
                                    rowindex +
                                    '].source_type_name" type="hidden" Isdropdown="true" ' +
                                    'textfield="ExchangeRightModel.source_type_name" textvalue="' +
                                    source_type_name +
                                    '" ' +
                                    'valuefield="ExchangeRightModel.source_type" value="' +
                                    source_type_name +
                                    '">' +
                                    '</td > ');

                                ColCalculateType = $('<td class="long-data">' +
                                    exchange_type_name +
                                    '<input name="Exchange[' +
                                    rowindex +
                                    '].exchange_type" type="hidden" Isdropdown="true" ' +
                                    'textfield="ExchangeRightModel.exchange_type_name" textvalue="' +
                                    exchange_type_name +
                                    '" ' +
                                    'valuefield="ExchangeRightModel.exchange_type" value="' +
                                    exchange_type +
                                    '">' +
                                    '<input name="Exchange[' +
                                    rowindex +
                                    '].exchange_type_name" type="hidden" Isdropdown="true" ' +
                                    'textfield="ExchangeRightModel.exchange_type_name" textvalue="' +
                                    exchange_type_name +
                                    '" ' +
                                    'valuefield="ExchangeRightModel.exchange_type" value="' +
                                    exchange_type_name +
                                    '">' +
                                    '</td>');

                                ColRowStatus = $('<td><input name="Exchange[' +
                                    rowindex +
                                    '].rowstatus" Isdropdown="false"  type="hidden" value="create"></td>');

                                ColAction = $('<td class="action">' +
                                    '<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                                    cur +
                                    '" data-action="update" onclick="GM.Counterparty.Exchange.Form.Show(this)" >' +
                                    '<i class="feather-icon icon-edit"></i>' +
                                    '</button > ' +
                                    '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                                    cur +
                                    '" data-action="delete" onclick="GM.Counterparty.Exchange.Form.Show(this)" >' +
                                    '<i class="feather-icon icon-trash-2"></i>' +
                                    '</button>' +
                                    '</td>');

                                row.append(ColCur);
                                row.append(ColFormula);
                                row.append(ColCalculateType);
                                row.append(ColRowStatus);
                                row.append(ColAction);

                                //GM.Counterparty.Exchange.Table.append(row);
                                GM.Counterparty.Exchange.Table.find('tr[data-id="' + rowid + '"]').after(row);
                                GM.Counterparty.Exchange.Table.find(".empty-data").parent().remove();
                            } else {
                                GM.Message.Error('.modal-body', "The Data already exists.");
                                return;
                            }
                            //#endregion

                        }
                        else {

                            //#region update action
                            row = GM.Counterparty.Exchange.Table.RowSelected;
                            rowindex = row.data("id");

                            if (!GM.Counterparty.Exchange.Table.RowAny(cur, null, rowindex)) {

                                ColCur = row.children("td:nth-child(1)");
                                ColFormula = row.children("td:nth-child(2)");
                                ColCalculateType = row.children("td:nth-child(3)");
                                ColRowStatus = row.children("td:nth-child(4)");
                                ColAction = row.children("td:nth-child(5)");

                                ColCur.html(cur_name +
                                    '<input name="Exchange[' +
                                    rowindex +
                                    '].cur" type="hidden" Isdropdown="true" ' +
                                    'textfield="ExchangeRightModel.cur_name" textvalue="' +
                                    cur_name +
                                    '" ' +
                                    'valuefield="ExchangeRightModel.cur" value="' +
                                    cur +
                                    '">' +
                                    '<input name="Exchange[' +
                                    rowindex +
                                    '].cur_name" type="hidden" Isdropdown="true" ' +
                                    'textfield="ExchangeRightModel.cur_name" textvalue="' +
                                    cur_name +
                                    '" ' +
                                    'valuefield="ExchangeRightModel.cur" value="' +
                                    cur +
                                    '">');

                                ColFormula.html(source_type_name +
                                    '<input name="Exchange[' +
                                    rowindex +
                                    '].source_type" type="hidden" Isdropdown="true" ' +
                                    'textfield="ExchangeRightModel.source_type_name" textvalue="' +
                                    source_type_name +
                                    '" ' +
                                    'valuefield="ExchangeRightModel.source_type" value="' +
                                    source_type +
                                    '">' +

                                    '<input name="Exchange[' +
                                    rowindex +
                                    '].source_type_name" type="hidden" Isdropdown="true" ' +
                                    'textfield="ExchangeRightModel.source_type_name" textvalue="' +
                                    source_type_name +
                                    '" ' +
                                    'valuefield="ExchangeRightModel.source_type" value="' +
                                    source_type_name +
                                    '">');

                                ColCalculateType.html(exchange_type_name +
                                    '<input name="Exchange[' +
                                    rowindex +
                                    '].exchange_type" type="hidden" Isdropdown="true" ' +
                                    'textfield="ExchangeRightModel.exchange_type_name" textvalue="' +
                                    exchange_type_name +
                                    '" ' +
                                    'valuefield="ExchangeRightModel.exchange_type" value="' +
                                    exchange_type +
                                    '">' +
                                    '<input name="Exchange[' +
                                    rowindex +
                                    '].exchange_type_name" type="hidden" Isdropdown="true" ' +
                                    'textfield="ExchangeRightModel.exchange_type_name" textvalue="' +
                                    exchange_type_name +
                                    '" ' +
                                    'valuefield="ExchangeRightModel.exchange_type" value="' +
                                    exchange_type_name +
                                    '">');

                                ColRowStatus.html('<input name="Exchange[' +
                                    rowindex +
                                    '].rowstatus" Isdropdown="false"  type="hidden" value="' + textstatus+'">');

                                ColAction.html('<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                                    cur +
                                    '" data-action="update" onclick="GM.Counterparty.Exchange.Form.Show(this)" >' +
                                    '<i class="feather-icon icon-edit"></i>' +
                                    '</button > ' +
                                    '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                                    cur +
                                    '" data-action="delete" onclick="GM.Counterparty.Exchange.Form.Show(this)" >' +
                                    '<i class="feather-icon icon-trash-2"></i>' +
                                    '</button>');
                            } else {
                                GM.Message.Error('.modal-body', "The Data already exists.");
                                return;
                            }
                            //#endregion
                        }

                    }
                    else {
                        GM.Message.Error('.modal-body', "The Data already exists.");
                        return;
                    }
                    //#endregion

                    break;

                case 'delete':

                    var rowselect = GM.Counterparty.Exchange.Table.RowSelected;
                    var rowfrom = rowselect.data("action");
                    var rowid = rowselect.data("id");
                    if (rowfrom != "fromdatabase") {
                        GM.Counterparty.Exchange.Table.RowSelected.remove();
                    } else {
                        rowselect.attr("style", "display: none");
                        var inputs = $(rowselect).find('input,select,textarea');
                        if (inputs.length > 0) {
                            $.each(inputs,
                                function () {
                                    var names = this.name.split('.');
                                    //update new input name                                
                                    if (names[1] == "rowstatus") {
                                        var inputname = 'Exchange[' + rowid + '].' + names[1];
                                        $(this).attr('name', inputname);
                                        $(this).attr("value", "delete");
                                        $(this).attr("type", "hidden");
                                        $(this).text("delete");
                                    }
                                });
                        }
                    }
                    //renew index of input
                    var rows = GM.Counterparty.Exchange.Table.find('tr');
                    var rowindex = 0;
                    var checkrow = 0;
                    var rowDelete = 0;
                    rows.each(function (index, row) {
                        var inputs = $(row).find('input,select,textarea');
                        if (inputs.length > 0) {
                            $.each(inputs,
                                function () {
                                    var names = this.name.split('.');
                                    var inputname = 'Exchange[' + rowindex + '].' + names[1];
                                    //update new input name
                                    $(this).attr('name', inputname);

                                    if (names[1] == "rowstatus" && $(this).val() != "delete") {
                                        checkrow++;
                                    } else if (names[1] == "rowstatus" && $(this).val() == "delete") {
                                        rowDelete++;
                                    }

                                });
                            rowindex++;
                        }
                    });

                    if (checkrow == 0) {
                        var row = null;
                        if (rowDelete == 0) {
                            row = $('<tr></tr>');
                        } else {
                            row = $('<tr data-id="' + rowDelete + '" ></tr>');
                        }
                        var rowEmpty = '<td class="long-data text-center empty-data" style="height: 50px; " colspan="7"> No data.</td>';
                        row.append(rowEmpty);
                        GM.Counterparty.Exchange.Table.append(row);
                    }
                    break;
            }

            GM.Counterparty.Exchange.Table.RowSelected = {};
            GM.Counterparty.Exchange.Form.modal('toggle');
        }
    };
    GM.Counterparty.Exchange.Form.Initial = function () {
        GM.Message.Clear();
        if (GM.Counterparty.Exchange.Table.RowSelected) {
            var inputs = $(GM.Counterparty.Exchange.Table.RowSelected).find('input,select,textarea');

            if (inputs.length > 0) {
                $.each(inputs, function () {
                    var names = this.name.split('.');
                    if (this.attributes.isdropdown.value == 'true') {
                        if (this.attributes.textvalue != null) {
                            var filedtextid = this.attributes.textfield.value.split('.');

                            $("#ExchangeForm span[name='" + filedtextid[1] + "']").text(this.attributes.textvalue.value);
                            $("#ExchangeForm input[name='" + this.attributes.textfield.value + "']").val(this.attributes.textvalue.value);
                            $("#ExchangeForm input[name='" + this.attributes.textfield.value + "']").text(this.attributes.textvalue.value);
                            $("#ExchangeForm input[name='" + this.attributes.valuefield.value + "']").val(this.value);

                            if (filedtextid[1] == "cur") {
                                $("#ddl_cur_exchange").find(".selected-data").text(this.value);
                                $("#ddl_cur_exchange").attr("disabled", "disabled");
                                $('#ExchangeRightModel_cur_name').text(this.value);
                                $("#ExchangeRightModel_cur").val(this.value);
                            } else if (filedtextid[1] == "source_type") {
                                $("#ddl_source_type").find(".selected-data").text(this.value);
                                $("#ddl_source_type").attr("disabled", "disabled");
                                $('#ExchangeRightModel_source_type_name').text(this.value);
                                $("#ExchangeRightModel_source_type").val(this.value);
                            } else if (filedtextid[1] == "exchange_type") {
                                $("#ddl_exchange_type").find(".selected-data").text(this.value);
                                $("#ddl_exchange_type").attr("disabled", "disabled");
                                $('#ExchangeRightModel_exchange_type_name').text(this.value);
                                $("#ExchangeRightModel_exchange_type").val(this.value);
                            }
                        }

                    } else {
                        $("#ExchangeForm input[name='ExchangeRightModel." + names[1] + "']").val(this.value);
                        $("#ExchangeForm input[name='ExchangeRightModel." + names[1] + "']").text(this.value);
                    }
                });
            }

        }
    };
    GM.Counterparty.Exchange.Form.Reset = function () {

        $("#ddl_cur_exchange").find(".selected-data").text("Select...");
        $("#ddl_cur_exchange").removeAttr('disabled');
        $('#ExchangeRightModel_cur_name').text(null);
        $("#ExchangeRightModel_cur").val(null);

        $("#ddl_source_type").find(".selected-data").text("Select...");
        $("#ddl_source_type").removeAttr('disabled');
        $('#ExchangeRightModel_source_type_name').text(null);
        $("#ExchangeRightModel_source_type").val(null);

        $("#ddl_exchange_type").find(".selected-data").text("Select...");
        $("#ddl_exchange_type").removeAttr('disabled');
        $('#ExchangeRightModel_exchange_type_name').text(null);
        $("#ExchangeRightModel_exchange_type").val(null);

        $("#cur_exchange_error").text("");
        $("#source_type_error").text("");
        $("#exchange_type_error").text("");

        GM.Message.Clear();
    };

    $('#btnExchangeCreate').on('click', function () {
        GM.Counterparty.Exchange.Form.Show(this);
    });

    $('#btnExchangeSave').on('click', function () {
        GM.Counterparty.Exchange.Form.Save(this);
    });
    //End Exchange Rate


    $('#action-form').on('submit', function (e) {
    });

    function setformatdateyyyymmdd(date) {
        if (date != "") {
            date = date.split('/');
            date = date[2] + "" + date[1] + "" + date[0];
        }
        else {
            date = 0;
        }
        return date;
    }


    $('#open_date').on('dp.change', function (e) {

        var date = moment(e.date).format('DD/MM/YYYY');
        var open_date = moment(e.date).format('YYYYMMDD');
        var close_date = setformatdateyyyymmdd($("#close_date").val());

        if (close_date != 0 && close_date < open_date) {
            $("#open_date").text($("#close_date").val());
            $("#open_date").val($("#close_date").val());
            swal("Warning", "Open Date Can't more than Close Date", "warning");
        }
    });

    $('#close_date').on('dp.change', function (e) {

        var date = moment(e.date).format('DD/MM/YYYY');
        var close_date = moment(e.date).format('YYYYMMDD');
        var open_date = setformatdateyyyymmdd($("#open_date").val());

        if (close_date < open_date) {
            $("#close_date").text($("#open_date").val());
            $("#close_date").val($("#open_date").val());
            swal("Warning", "Close Date Can't less than Open Date", "warning");
        }
    });

    $('#signed_date').on('dp.change', function (e) {

        var signed_date = moment(e.date).format('YYYYMMDD');
        var close_date = setformatdateyyyymmdd($("#close_date").val());
        var open_date = setformatdateyyyymmdd($("#open_date").val());

        // comment by paweekorn : 18/4/2019
        //if (close_date !== 0 && close_date < signed_date) {
        //    $("#close_date").text(moment(e.date).format('DD/MM/YYYY'));
        //    $("#close_date").val(moment(e.date).format('DD/MM/YYYY'));
        //}
        
        //if (open_date != 0 && signed_date < open_date) {
        //    $("#open_date").text(moment(e.date).format('DD/MM/YYYY'));
        //    $("#open_date").val(moment(e.date).format('DD/MM/YYYY'));
        //}
    });

    $("#counter_party_code").on("keypress", function (evt) {
        if ((evt.key >= "a" && evt.key <= "z") || (evt.key >= "A" && evt.key <= "Z")
            || (evt.key >= "0" && evt.key <= "9") || evt.key === "-" || evt.key === "_") {
            ;
        } else {
            evt.preventDefault();
        }
    });
});