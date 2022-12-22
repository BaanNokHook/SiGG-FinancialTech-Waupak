
function FormatDecimalDigit(num, digit) {
    var format = Number(parseFloat(num).toFixed(0)).toLocaleString('en', {
        minimumFractionDigits: digit
    });
    return format;
}

function FormatDecimal(value, point) {
	const f = Math.pow(10, point);
	return (Math.floor(value * f) / f).toFixed(point);
}

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
        $("#security-detail-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#security-detail-form").attr("aria-expanded", "true");
        $("#security-detail-form").removeAttr("style");
        $("#security-detail-icon").attr("aria-expanded", "true");
        $("#security-detail-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#security-detail-icon").attr("class", "box-header big-head expand-able");
        $("#detail-li").attr("class", "active");
        $("#definition-li").removeAttr("class");
    }
    else if (panelname == "definition") {
        $("#security-definition-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#security-definition-form").attr("aria-expanded", "true");
        $("#security-definition-form").removeAttr("style");
        $("#security-definition-icon").attr("aria-expanded", "true");
        $("#security-definition-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#security-definition-icon").attr("class", "box-header big-head expand-able");
        $("#detail-li").removeAttr("class");
        $("#definition-li").attr("class", "active");

    }
    else if (panelname == "event") {
        $("#security-event-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#security-event-form").attr("aria-expanded", "true");
        $("#security-event-form").removeAttr("style");
        $("#security-event-icon").attr("aria-expanded", "true");
        $("#security-event-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#security-event-icon").attr("class", "box-header big-head expand-able");
        $("#detail-li").removeAttr("class");
        $("#definition-li").removeAttr("class");
        $("#event-li").attr("class", "active");
        adjusttable();
    }
    else if (panelname == "banklist") {
        $("#counterparty-banklist-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#counterparty-banklist-form").attr("aria-expanded", "true");
        $("#counterparty-banklist-form").removeAttr("style");
        $("#counterparty-banklist-icon").attr("aria-expanded", "true");
        $("#counterparty-banklist-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#counterparty-banklist-icon").attr("class", "box-header big-head expand-able");

        $("#detail-li").removeAttr("class");
        $("#address-li").removeAttr("class");
        $("#taxandother-li").removeAttr("class");
        $("#banklist-li").attr("class", "active");
        $("#identify-li").removeAttr("class");
        $("#rating-li").removeAttr("class");
        $("#margin-li").removeAttr("class");
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
        $("#banklist-li").removeAttr("class");
        $("#identify-li").attr("class", "active");
        $("#rating-li").removeAttr("class");
        $("#margin-li").removeAttr("class");
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
        $("#banklist-li").removeAttr("class");
        $("#identify-li").removeAttr("class");
        $("#rating-li").attr("class", "active");
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
        $("#banklist-li").removeAttr("class");
        $("#identify-li").removeAttr("class");
        $("#rating-li").removeAttr("class");
        $("#margin-li").attr("class", "active");
    }
}

function GetEditCashFlow() {
    if ($("#instrument_id").val() != null) {
        GM.Security.CashFlow.Table.columns(22).search($("#instrument_id").val());
        //GM.Security.CashFlow.Table.draw();
    }
}

function GenarateIssuerRatingTable(data) {

    GM.IssuerRating = {};
    GM.IssuerRating.Table = $("#x-table-data-issuerrating2");
    GM.IssuerRating.Table.find("tbody tr").remove();
    if (data.length > 0) {
        $("#issuer_nodata").attr("style", "display: none");
        GM.IssuerRating.Table.attr("style", "display: show");
        $.each(data, function (index, value) {

            var row = $('<tr></tr>');

            var ColAgency = $('<td>' + value.agency_name + '<input name="IssuerRatingList[' + index + '].agency_code" Isdropdown="false" type="hidden" value="' + value.agency_code + '"></td>');
            var ColTerm = $('<td>' + value.short_long_term_text + '<input name="IssuerRatingList[' + index + '].short_long_term" Isdropdown="false" type="hidden" value="' + value.short_long_term + '"></td>');
            var ColLocal = $('<td>' + value.local_rating_text + '<input name="IssuerRatingList[' + index + '].local_rating" Isdropdown="false" type="hidden" value="' + value.local_rating + '"></td>');
            var ColForeign = $('<td>' + value.foreign_rating_text + '<input name="IssuerRatingList[' + index + '].foreign_rating" Isdropdown="false" type="hidden" value="' + value.foreign_rating + '"></td>');

            row.append(ColAgency);
            row.append(ColTerm);
            row.append(ColLocal);
            row.append(ColForeign);

            GM.IssuerRating.Table.append(row);
            GM.IssuerRating.Table.find(".empty-data").parent().remove();

        });
    }
    else {
        $("#issuer_nodata").attr("style", "display: show");
        GM.IssuerRating.Table.attr("style", "display: none");
    }
}

function adjusttable() {
    setTimeout(
        function () {
            $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();
        },
        200);
}

$(document).ready(function () {

    //Start Top Bar
    $('#security-detail').click(function (e) {
        var expand = $("div#security-detail-icon").attr("aria-expanded");
        if (expand == "true") {
            $("#security-detail-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#security-detail-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }

    });

    $('#security-definition').click(function (e) {
        var expand = $("div#security-definition-icon").attr("aria-expanded");
        adjusttable();
        if (expand == "true") {

            $("#security-definition-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#security-definition-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }

    });

    $('#security-event').click(function (e) {
        var expand = $("div#security-event-icon").attr("aria-expanded");
        if (expand == "true") {

            $("#security-event-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#security-event-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }
        adjusttable();
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

    $('#counterparty-banklist').click(function (e) {
        var expand = $("div#counterparty-banklist-icon").attr("aria-expanded");
        //alert(expand);
        if (expand == "true") {

            $("#counterparty-banklist-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#counterparty-banklist-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
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

    //End Top Bar

    //---------DropDownList

    //Product
    var checkproduct;
    $("#ddl_product_code").click(function () {
        var txt_search = $('#txt_product_code');
        var instrument_type = $('#instrumenttype').val();
        var data = { instrumenttype: instrument_type, productname: null };
        checkproduct = $('#product_code').val();

        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#ul_product_code").on("click", ".searchterm", function (event) {

        if (checkproduct != $('#product_code').val() && checkproduct != "") {
            $("#ddl_sub_product_code").find(".selected-data").text("Select...");
            $("#sub_product_code").val(null);
            $("#sub_product_code_text").text(null);

            $("#ddl_instrumenttype").find(".selected-data").text("Select...");
            $("#instrumenttype").val(null);
            $("#instrumenttype_text").text(null);

            $("#ddl_second_instrumenttype").find(".selected-data").text("Select...");
            $("#second_instrumenttype").val(null);
            $("#second_instrumenttypee_text").text(null);
        }
        else if ($('#product_code').val().trim() == "") {
            $("#ddl_sub_product_code").find(".selected-data").text("Select...");
            $("#sub_product_code").val(null);
            $("#sub_product_code_text").text(null);

            $("#ddl_instrumenttype").find(".selected-data").text("Select...");
            $("#instrumenttype").val(null);
            $("#instrumenttype_text").text(null);

            $("#ddl_second_instrumenttype").find(".selected-data").text("Select...");
            $("#second_instrumenttype").val(null);
            $("#second_instrumenttypee_text").text(null);
        }
    });

    $('#txt_product_code').keyup(function () {

        //if (this.value.length > 0) {
        var data = { instrumenttypename: null, productname: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Product

    //Sub Product
    $("#ddl_sub_product_code").click(function () {
        var txt_search = $('#txt_sub_product_code');
        var product_code = $('#product_code').val();
        var data = { productcode: product_code, subproduct: null };

        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_sub_product_code').keyup(function () {

        //if (this.value.length > 0) {
        var product_code = $('#product_code').val();
        var data = { productcode: product_code, subproduct: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Sub Product

    //Bond Type
    var checkbondtype;
    $("#ddl_instrumenttype").click(function () {

        var txt_search = $('#txt_instrumenttype');
        var product_code = $('#product_code').val();

        checkbondtype = $('#instrumenttype').val();
        var data = { instrumenttypename: null, productcode: product_code };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#ul_instrumenttype").on("click", ".searchterm", function (event) {

        if (checkbondtype != $('#instrumenttype').val() && checkbondtype != "") {
            $("#ddl_product_code").find(".selected-data").text("Select...");
            $("#product_code").val(null);
            $("#product_code_text").text(null);

            $("#ddl_sub_product_code").find(".selected-data").text("Select...");
            $("#sub_product_code").val(null);
            $("#sub_product_code_text").text(null);

            $("#ddl_second_instrumenttype").find(".selected-data").text("Select...");
            $("#second_instrumenttype").val(null);
            $("#second_instrumenttypee_text").text(null);
        }
        else if ($('#instrumenttype').val().trim() == "") {
            $("#ddl_product_code").find(".selected-data").text("Select...");
            $("#product_code").val(null);
            $("#product_code_text").text(null);

            $("#ddl_sub_product_code").find(".selected-data").text("Select...");
            $("#sub_product_code").val(null);
            $("#sub_product_code_text").text(null);

            $("#ddl_second_instrumenttype").find(".selected-data").text("Select...");
            $("#second_instrumenttype").val(null);
            $("#second_instrumenttypee_text").text(null);
        }
    });

    $('#txt_instrumenttype').keyup(function () {
        var product_code = $('#product_code').val();
        var data = { instrumenttypename: this.value, productcode: product_code };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Bond Type

    //Bond Sub Type
    $("#ddl_second_instrumenttype").click(function () {
        var txt_search = $('#txt_second_instrumenttype');
        var product_code = $('#product_code').val();
        var instrumenttype = $('#instrumenttype').val();

        var data = { bondsubtypedesc: null, productcode: product_code, instrumenttypename: instrumenttype };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_second_instrumenttype').keyup(function () {

        //if (this.value.length > 0) {
        var product_code = $('#product_code').val();
        var instrumenttype = $('#instrumenttype').val();

        var data = { bondsubtypedesc: this.value, productcode: product_code, instrumenttypename: instrumenttype };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End Bond Sub Type

    //xa unit
    $("#ddl_xadayunit").click(function () {
        var txt_search = $('#txt_xadayunit');

        var data = {};
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });
    //End xa unit

    //xi unit
    $("#ddl_xidayunit").click(function () {
        var txt_search = $('#txt_xidayunit');

        var data = {};
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });
    //End xi unit

    //owner
    $("#ddl_owner").click(function () {
        var txt_search = $('#txt_owner');

        var data = {};
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });
    //End owner

    //Issuer
    $("#ddl_issuer").click(function () {
        var txt_search = $('#txt_issuer');
        var data = { issuername: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#ul_issuer").on("click", ".searchterm", function (event) {
        GetIssuerRating($('#issuer_id').val());
        //GM.Security.IssuerRating.Table.columns(4).search($('#issuer_id').val());  
        //GM.Security.IssuerRating.Table.draw();
    });

    $('#txt_issuer').keyup(function () {
        var data = { issuername: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Issuer

    //Register
    $("#ddl_register").click(function () {
        var txt_search = $('#txt_register');
        var data = { registername: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_register').keyup(function () {
        var data = { registername: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Register

    //OptionType
    $("#ddl_optiontype").click(function () {
        var txt_search = $('#txt_optiontype');
        var data = { optionname: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_optiontype').keyup(function () {
        var data = { optionname: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End OptionType

    //SeniorityType
    $("#ddl_seniority").click(function () {
        var txt_search = $('#txt_seniority');
        var data = { senioritytypename: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_seniority').keyup(function () {
        var data = { senioritytypename: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End SeniorityType

    //market
    $("#ddl_market").click(function () {
        var txt_search = $('#txt_market');
        var data = { marketname: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_market').keyup(function () {
        var data = { marketname: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End market

    //cur
    $("#ddl_cur").click(function () {
        var txt_search = $('#txt_cur');
        var data = { curtext: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_cur').keyup(function () {
        var data = { curtext: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End cur

    //holiday cur
    $("#ddl_holidaycur").click(function () {
        var txt_search = $('#txt_holidaycur');
        var data = { curtext: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_holidaycur').keyup(function () {
        var data = { curtext: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End holiday cur

    //yearbasis
    $("#ddl_yearbasis").click(function () {
        var txt_search = $('#txt_yearbasis');
        var data = { yearbasistext: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_yearbasis').keyup(function () {
        var data = { yearbasistext: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End yearbasis

    //yearbasis
    $("#ddl_tbmalisted").click(function () {
        var txt_search = $('#txt_tbmalisted');
        var data = { tbmalisttext: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_tbmalisted').keyup(function () {
        var data = { tbmalisttext: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End yearbasis

    //yearbasis
    $("#ddl_guarantor").click(function () {
        var txt_search = $('#txt_guarantor');
        var data = { guarantorname: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_guarantor').keyup(function () {
        var data = { guarantorname: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End yearbasis

    //agency
    $("#ddl_agency").click(function () {
        var txt_search = $('#txt_agency');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $("#ul_agency").on("click", ".searchterm", function (event) {
        $("#ddl_localrating").find(".selected-data").text("Select...");
        $("#local_rating").val(null);
        $("#local_rating_text").text(null);
        $("#local_rating_text").val(null);

        $("#ddl_foreignrating").find(".selected-data").text("Select...");
        $("#foreign_rating").val(null);
        $("#foreign_rating_text").text(null);
        $("#foreign_rating_text").val(null);
    });

    $('#txt_agency').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End agency

    //Local Rating
    $("#ddl_localrating").click(function () {
        var txt_search = $('#txt_localrating');
        var agency_code = $('#BondRatingRightModal_agency_code').val();
        var short_long_term = $('#BondRatingRightModal_short_long_term').val();
        var data = { agencycode: agency_code, shortlongterm: short_long_term };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_localrating').keyup(function () {
        var agency_code = $('#BondRatingRightModal_agency_code').val();
        var short_long_term = $('#BondRatingRightModal_short_long_term').val();
        var data = { agencycode: agency_code, shortlongterm: short_long_term, datatext: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });
    //End Local Rating

    //Foreign Rating
    $("#ddl_foreignrating").click(function () {
        var txt_search = $('#txt_foreignrating');
        var agency_code = $('#BondRatingRightModal_agency_code').val();
        var short_long_term = $('#BondRatingRightModal_short_long_term').val();
        var data = { agencycode: agency_code, shortlongterm: short_long_term };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_foreignrating').keyup(function () {
        var agency_code = $('#BondRatingRightModal_agency_code').val();
        var short_long_term = $('#BondRatingRightModal_short_long_term').val();
        var data = { agencycode: agency_code, shortlongterm: short_long_term, datatext: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //End Foreign Rating

    $("#ddl_coupon_type").click(function () {
        var txt_search = $('#txt_coupon_type');
        var data = { text: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_coupon_type').keyup(function () {
        var data = { text: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    $("#ddl_coupon_freq").click(function () {
        var txt_search = $('#txt_coupon_freq');
        var data = { text: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_coupon_freq').keyup(function () {
        var data = { text: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    //--------- End DropDownList

    $('.radio input[id=tax_on_coupon_flag]').change(function () {
        var current = $(this).val();
        var radioyes = $("[id=tax_on_coupon_flag][value=true]");
        var radiono = $("[id=tax_on_coupon_flag][value=false]");
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

    $('.radio input[id=ex_coupon_flag]').change(function () {
        var current = $(this).val();
        var radioyes = $("[id=ex_coupon_flag][value=true]");
        var radiono = $("[id=ex_coupon_flag][value=false]");
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

    $('.radio input[id=verify_flag]').change(function () {
        var current = $(this).val();
        var radioyes = $("[id=verify_flag][value=true]");
        var radiono = $("[id=verify_flag][value=false]");
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

    GM.Security = {};
    GM.Security.Save = function (form) {
        // GM.Counterparty.Banklist.AddToForm(form);
        //GM.Security.Rating.AddToForm(form);
        //GM.Security.Addtion.AddToForm(form)
    };
    //GM.Security.IssuerRating = {};
    //GM.Security.IssuerRating.Table = $('#x-table-data-issuerrating').DataTable({
    //    dom: 'Bfrtip',
    //    select: false,
    //    searching: true,
    //    scrollY: '80vh',
    //    scrollX: true,
    //    order: [
    //        [1, "desc"]
    //    ],
    //    buttons:
    //    [
    //        //{
    //        //    text: 'Refresh',
    //        //    action: function (e, dt, node, config) {
    //        //        dt.ajax.reload();
    //        //    }
    //        //}
    //    ],
    //    processing: true,
    //    serverSide: true,
    //    ajax: {
    //        "url": "/Security/SearchIssuerRating2",
    //        "type": "POST"
    //    },

    //    select: {
    //        style: 'os',
    //        selector: 'td:first-child'
    //    },
    //    select: true,
    //    columnDefs:
    //    [
    //        { targets: 0, data: "agency_name" },
    //        { targets: 1, data: "short_long_term_text" },
    //        { targets: 2, data: "local_rating_text" },
    //        { targets: 3, data: "foreign_rating_text" },
    //        { targets: 4, data: "issuer_id", "visible": false }
    //    ],
    //    fixedColumns: {
    //        leftColumns: 1,
    //        rightColumns: 1
    //    },

    //});

    //Start Guarantor
    GM.Security.Guarantor = {};
    GM.Security.Guarantor.Table = $("#tbGuarantors");
    GM.Security.Guarantor.Table.RowSelected = {};
    GM.Security.Guarantor.Table.RowEmpty = function () {
        var row = $("<tr></tr>");
        var col = $('<td class="long-data text-center empty-data" style="height:50px;" colspan="3"> No data.</td>');
        row.append(col);
        GM.Security.Guarantor.Table.append(row);
    };

    GM.Security.Guarantor.Table.RowAny = function (code, code2, rowid) {

        var rows = GM.Security.Guarantor.Table.find('tr');
        var IsExits = false;
        rows.each(function (index, row) {

            var rowindex = $(row).data("id");
            var inputs = $(row).find('input,select,textarea');

            if (inputs.length > 0) {

                $.each(inputs, function () {

                    var names = this.name.split('.');

                    if (names[1] == 'guarantor_code' && this.value == code) {

                        if (rowid != rowindex) {
                            IsExits = true;
                        }
                    }

                    if (names[1] == 'rowstatus' && this.value === 'delete') {
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
    GM.Security.Guarantor.Form = $("#modal-form-guarantor");
    GM.Security.Guarantor.Form.ButtonSave = $('#btnGuarantorSave');
    GM.Security.Guarantor.Form.Show = function (btn) {

        var action = $(btn).data("action");
        var button = $('#btnGuarantorSave');

        GM.Security.Guarantor.Form.Reset();

        switch (action) {
            case 'create':
                GM.Security.Guarantor.Form.find(".modal-title").html("Create");
                GM.Security.Guarantor.Form.ButtonSave.data("action", "create");
                GM.Security.Guarantor.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('Add');
                $('#ddl_guarantor').removeAttr('disabled');
                $('#GuarantorsRightModal_guarantor_percent').removeAttr('disabled');
                break;

            case 'update':

                GM.Security.Guarantor.Table.RowSelected = $(btn).parent().parent();
                GM.Security.Guarantor.Form.find(".modal-title").html("Update");
                GM.Security.Guarantor.Form.Initial();

                GM.Security.Guarantor.Form.ButtonSave.data("action", "update");
                GM.Security.Guarantor.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('Update');

                $('#ddl_guarantor').attr('disabled', 'disabled');
                $('#GuarantorsRightModal_guarantor_percent').removeAttr('disabled');
                break;

            case 'delete':

                GM.Security.Guarantor.Table.RowSelected = $(btn).parent().parent();
                GM.Security.Guarantor.Form.find(".modal-title").html("Delete");
                GM.Security.Guarantor.Form.Initial();

                GM.Security.Guarantor.Form.ButtonSave.data("action", "delete");
                GM.Security.Guarantor.Form.ButtonSave.removeClass('btn-primary').addClass('btn-delete').text('Confirm Delete');

                $('#ddl_guarantor').attr('disabled', 'disabled');
                $('#GuarantorsRightModal_guarantor_percent').attr('disabled', 'disabled');
                break;

            default:
                break;
        }

        GM.Security.Guarantor.Form.modal('toggle');
    };
    GM.Security.Guarantor.Form.Valid = function () {

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

    GM.Security.Guarantor.Form.Save = function (btn) {

        var action = $(btn).data("action");
        //  if (!GM.Counterparty.Banklist.Form.Valid()) {
        //      return false;
        //  }
        var IsInValid = false;
        GM.Message.Clear();
        $("#guarantor_name_text_error").text("");
        $("#guarantor_percent_text_error").text("");
        switch (action) {
            case 'create':
                GM.Security.Guarantor.Table.attr("style", "display: show");

                var guarantor_code_text = $('#GuarantorsRightModal_guarantor_name').text();
                var guarantor_code_val = $("#GuarantorsRightModal_guarantor_code").val();
                var guarantorpercent = $('#GuarantorsRightModal_guarantor_percent').val();

                if (guarantor_code_val == "") {
                    $("#guarantor_name_text_error").text("The Guarantor Code field is required.");
                    IsInValid = true;
                }
                if (guarantorpercent == "") {
                    $("#guarantor_percent_text_error").text("The Guarantor Percent field is required.");
                    IsInValid = true;
                }

                if (IsInValid) {
                    break;
                }

                var rowindex = GM.Security.Guarantor.Table.find("tr:last").data("id");

                if (!GM.Security.Guarantor.Table.RowAny(guarantor_code_val)) {
                    if (rowindex != undefined) {
                        rowindex = parseInt(rowindex) + 1;
                    }
                    else { rowindex = 0; }

                    var row = $('<tr data-id="' + rowindex + '" data-action="fromui"></tr>');

                    console.log("rowindex : " + rowindex);

                    var ColGuarantors = $('<td class="long-data" style="width: 300px;">' + guarantor_code_text +
                        '<input name="Guarantors[' + rowindex + '].guarantor_code" type="hidden" Isdropdown="true" ' +
                        'textfield="GuarantorsRightModal.guarantor_name" textvalue= "' + guarantor_code_text + '"' +
                        'valuefield="GuarantorsRightModal.guarantor_code" value= "' + guarantor_code_val + '" >' +
                        '<input name="Guarantors[' + rowindex + '].guarantor_name" type="hidden" Isdropdown="true" ' +
                        'textfield="GuarantorsRightModal.guarantor_name" textvalue= "' + guarantor_code_text + '"' +
                        'valuefield="GuarantorsRightModal.guarantor_code" value= "' + guarantor_code_text + '" >' +
                        '</td > ');

                    var ColGuarantorPercent = $('<td>' + guarantorpercent + '<input name="Guarantors[' + rowindex + '].guarantor_percent" Isdropdown="false"  type="hidden" value="' + guarantorpercent + '"></td>');
                    var ColRowStatus = $('<td><input name="Guarantors[' + rowindex + '].rowstatus" Isdropdown="false"  type="hidden" value="create"></td>');
                    var ColAction = $('<td class="action">' +
                        '<button class="btn btn-default btn-round icon-only" type="button" data-id="' + guarantor_code_text + '" data-action="update" onclick="GM.Security.Guarantor.Form.Show(this)"  >' +
                        '<i class="feather-icon icon-edit"></i>' +
                        '</button > ' +
                        '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' + guarantor_code_text + '" data-action="delete" onclick="GM.Security.Guarantor.Form.Show(this)"  >' +
                        '<i class="feather-icon icon-trash-2"></i>' +
                        '</button>' +
                        '</td>');

                    row.append(ColGuarantors);
                    row.append(ColGuarantorPercent);
                    row.append(ColRowStatus);
                    row.append(ColAction);

                    GM.Security.Guarantor.Table.append(row);
                    GM.Security.Guarantor.Table.find(".empty-data").parent().remove();
                }
                else {
                    GM.Message.Error('.modal-body', "The Data already exists.");
                    IsInValid = true;
                }
                break;

            case 'update':

                var guarantor_code_text = $('#GuarantorsRightModal_guarantor_name').text();
                var guarantor_code_val = $("#GuarantorsRightModal_guarantor_code").val();
                var guarantorpercent = $('#GuarantorsRightModal_guarantor_percent').val();


                if (guarantor_code_val == "") {
                    $("#guarantor_name_text_error").text("The Guarantor Code field is required.");
                    IsInValid = true;
                }
                if (guarantorpercent == "") {
                    $("#guarantor_percent_text_error").text("The Guarantor Percent field is required.");
                    IsInValid = true;
                }

                if (IsInValid) {
                    break;
                }

                var rowindex = GM.Security.Guarantor.Table.find("tr:last").data("id");

                var row = GM.Security.Guarantor.Table.RowSelected;
                var rowindex = row.data("id");
                var rowfrom = row.data("action");
                var textstatus = "create";
                if (rowfrom == "fromdatabase") {
                    textstatus = "update";
                }

                if (!GM.Security.Guarantor.Table.RowAny(guarantor_code_val, null, rowindex)) {
                    // if (1 == 1) {
                    var ColGuarantors = row.children("td:nth-child(1)");
                    var ColGuarantorPercent = row.children("td:nth-child(2)");
                    var ColRowStatus = row.children("td:nth-child(3)");
                    var ColAction = row.children("td:nth-child(4)");

                    ColGuarantors.html(guarantor_code_text +
                        '<input name="Guarantors[' + rowindex + '].guarantor_code" type="hidden" Isdropdown="true" ' +
                        'textfield="GuarantorsRightModal.guarantor_name" textvalue="' + guarantor_code_text + '" ' +
                        'valuefield="GuarantorsRightModal.guarantor_code" value="' + guarantor_code_val + '">' +
                        '<input name="Guarantors[' + rowindex + '].guarantor_name" type="hidden" Isdropdown="true" ' +
                        'textfield="GuarantorsRightModal.guarantor_name" textvalue="' + guarantor_code_text + '" ' +
                        'valuefield="GuarantorsRightModal.guarantor_code" value="' + guarantor_code_text + '">');

                    ColGuarantorPercent.html(guarantorpercent + '<input name="Guarantors[' + rowindex + '].guarantor_percent" Isdropdown="false" type="hidden" value="' + guarantorpercent + '">');
                    ColRowStatus.html('<input name="Guarantors[' + rowindex + '].rowstatus" Isdropdown="false"  type="hidden" value="' + textstatus + '">');
                    var htmlAction = '<button class="btn btn-default btn-round icon-only" type="button" data-id="' + guarantor_code_text + '" data-action="update" onclick="GM.Security.Guarantor.Form.Show(this)" >' +
                        '<i class="feather-icon icon-edit"></i>' +
                        '</button > ' +
                        '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' + guarantor_code_text + '" data-action="delete" onclick="GM.Security.Guarantor.Form.Show(this)" >' +
                        '<i class="feather-icon icon-trash-2"></i>' +
                        '</button>';
                    ColAction.html(htmlAction);
                    //  swal("Good job!", "Successfully saved", "success");
                }

                else {
                    GM.Message.Error('.modal-body', "The data guarantor already exists.");
                    IsInValid = true;
                }
                break;

            case 'delete':
                var rowselect = GM.Security.Guarantor.Table.RowSelected;
                var rowfrom = rowselect.data("action");
                var rowid = rowselect.data("id");
                if (rowfrom != "fromdatabase") {
                    GM.Security.Guarantor.Table.RowSelected.remove();
                }
                else {
                    rowselect.attr("style", "display: none");
                    var inputs = $(rowselect).find('input,select,textarea');
                    if (inputs.length > 0) {
                        $.each(inputs, function () {
                            var names = this.name.split('.');
                            //update new input name                                
                            if (names[1] == "rowstatus") {
                                var inputname = 'Guarantors[' + rowid + '].' + names[1];
                                $(this).attr('name', inputname);
                                $(this).attr("value", "delete");
                                $(this).attr("type", "hidden");
                                $(this).text("delete");
                            }
                        });
                    }
                }
                //renew index of input
                var rows = GM.Security.Guarantor.Table.find('tr');
                var rowindex = 0;
                var checkrow = 0;

                rows.each(function (index, row) {

                    var inputs = $(row).find('input,select,textarea');

                    if (inputs.length > 0) {

                        $.each(inputs, function () {

                            var names = this.name.split('.');
                            var inputname = 'Guarantors[' + rowindex + '].' + names[1];

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
                    GM.Security.Guarantor.Table.RowEmpty();
                }
                break;
        }

        GM.Security.Guarantor.Table.RowSelected = {};
        if (IsInValid == false) {
            GM.Security.Guarantor.Form.modal('toggle');
        }
    };

    GM.Security.Guarantor.Form.Initial = function () {
        if (GM.Security.Guarantor.Table.RowSelected) {
            var inputs = $(GM.Security.Guarantor.Table.RowSelected).find('input,select,textarea');
            if (inputs.length > 0) {
                $.each(inputs, function () {
                    var names = this.name.split('.');
                    if (this.attributes.isdropdown.value == 'true') {
                        $("#ddl_guarantor").find(".selected-data").text(this.value);
                        //$("#ddl_guarantor").attr("style", "pointer-events: none");
                        $('#GuarantorsRightModal_' + names[1]).val(this.value);
                        $('#GuarantorsRightModal_' + names[1]).text(this.value);
                    } else {
                        $('#GuarantorsRightModal_' + names[1]).val(FormatDecimalDigit(this.value, 2));
                        $('#GuarantorsRightModal_' + names[1]).focus();
                    }
                });
            }
        }
    };

    GM.Security.Guarantor.Form.Reset = function () {
        $("#ddl_guarantor").find(".selected-data").text("Select...");
        $("#ddl_guarantor").removeAttr("style");
        $('#GuarantorsRightModal_guarantor_name').text(null);
        $("#GuarantorsRightModal_guarantor_code").val(null);
        $('#GuarantorsRightModal_guarantor_percent').val(null);
        $("#guarantor_percent_text_error").text("");
        $("#guarantor_name_text_error").text("");
        $('#GuarantorsRightModal_guarantor_percent').removeClass("input-validation-error");
        GM.Message.Clear();
    };

    $('#btnGuarantorsCreate').on('click', function () {
        GM.Security.Guarantor.Form.Show(this);
    });

    $('#btnGuarantorSave').on('click', function () {
        GM.Security.Guarantor.Form.Save(this);
    });
    //End Guarantor

    //Start Bond Rating
    GM.Security.BondRating = {};
    GM.Security.BondRating.Table = $("#tbBondRating");
    GM.Security.BondRating.Table.RowSelected = {};
    GM.Security.BondRating.Table.RowEmpty = function () {
        var row = $("<tr></tr>");
        var col = $('<td class="long-data text-center empty-data" style="height:50px;" colspan="7"> No data.</td>');
        row.append(col);
        GM.Security.BondRating.Table.append(row);
    }
    GM.Security.BondRating.Table.RowAny = function (code, code2, rowid) {

        var rows = GM.Security.BondRating.Table.find('tr');
        var IsExits = false;
        rows.each(function (index, row) {

            var rowindex = $(row).data("id");
            var inputs = $(row).find('input,select,textarea');

            if (inputs.length > 0) {

                $.each(inputs, function () {

                    var names = this.name.split('.');

                    if (names[1] == 'agency_code' && this.value == code) {

                        if (rowid != rowindex) {
                            IsExits = true;
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

    GM.Security.BondRating.Form = $("#modal-form-rating");
    GM.Security.BondRating.Form.ButtonSave = $('#btnBondRatingSave');
    GM.Security.BondRating.Form.Show = function (btn) {

        var action = $(btn).data("action");
        var button = $('#btnBondRatingSave');

        GM.Security.BondRating.Form.Reset();

        switch (action) {
            case 'create':
                GM.Security.BondRating.Form.find(".modal-title").html("Create");
                GM.Security.BondRating.Form.ButtonSave.data("action", "create");
                GM.Security.BondRating.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('+ Add Rating');
                $("#ddl_agency").removeAttr('disabled');
                break;

            case 'update':

                GM.Security.BondRating.Table.RowSelected = $(btn).parent().parent();
                GM.Security.BondRating.Form.find(".modal-title").html("Update");
                GM.Security.BondRating.Form.Initial();

                GM.Security.BondRating.Form.ButtonSave.data("action", "update");
                GM.Security.BondRating.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('Save Rating');
                break;

            case 'delete':

                GM.Security.BondRating.Table.RowSelected = $(btn).parent().parent();
                GM.Security.BondRating.Form.find(".modal-title").html("Delete");
                GM.Security.BondRating.Form.Initial();

                GM.Security.BondRating.Form.ButtonSave.data("action", "delete");
                GM.Security.BondRating.Form.ButtonSave.removeClass('btn-primary').addClass('btn-delete').text('Confirm Delete');
                break;

            default:
                break;
        }

        GM.Security.BondRating.Form.modal('toggle');
    };

    GM.Security.BondRating.Form.Valid = function () {

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
    GM.Security.BondRating.Form.Save = function (btn) {

        var action = $(btn).data("action");

        //  if (!GM.Security.BondRating.Form.Valid()) {
        //      return false;
        //  }

        var IsInValid = false;
        GM.Message.Clear();
        $("#agency_code_error").text("");
        $("#short_long_term_error").text("");
        $("#local_rating_error").text("");
        $("#foreign_rating_error").text("");
        $("#assess_date_error").text("");

        switch (action) {
            case 'create':
                GM.Security.BondRating.Table.attr("style", "display: show");
                var agency_text = $('#BondRatingRightModal_agency_name').text();
                var agency_val = $("#BondRatingRightModal_agency_code").val();
                var short_long_term_text = $('#BondRatingRightModal_short_long_term_text').text();
                var short_long_term_val = $("#BondRatingRightModal_short_long_term").val();
                var local_rating_text = $('#BondRatingRightModal_local_rating_text').text();
                var local_rating_val = $("#BondRatingRightModal_local_rating").val();
                var foreign_rating_text = $('#BondRatingRightModal_foreign_rating_text').text();
                var foreign_rating_val = $("#BondRatingRightModal_foreign_rating").val();
                var assess_date_val = $("#BondRatingRightModal_assess_date").val();

                if (agency_val == "") {
                    $("#agency_code_error").text("The Agency field is required.");
                    IsInValid = true;
                }
                if (short_long_term_val == "") {
                    $("#short_long_term_error").text("The Term field is required.");
                    IsInValid = true;
                }
                //if(local_rating_val == "")
                //{
                //    $("#local_rating_error").text("The Guarantor Percent field is required.");
                //    IsInValid = true;
                //}
                //if(foreign_rating_val == "")
                //{
                //    $("#foreign_rating_error").text("The Guarantor Percent field is required.");
                //    IsInValid = true;
                //}
                if (assess_date_val == "") {
                    $("#assess_date_error").text("The Assess Date field is required.");
                    IsInValid = true;
                }

                if (IsInValid) {
                    break;
                }

                var rowindex = GM.Security.BondRating.Table.find("tr:last").data("id");

                if (!GM.Security.BondRating.Table.RowAny(agency_val)) {
                    if (rowindex != undefined) {
                        rowindex = parseInt(rowindex) + 1;
                    } else {
                        rowindex = 0;
                    }
                    var row = $('<tr data-id="' + rowindex + '" data-action="fromui"></tr>');

                    var ColAgencyCode = $('<td class="long-data">' +
                        agency_text +
                        '<input name="BondRating[' +
                        rowindex +
                        '].agency_code" type="hidden" Isdropdown="true" ' +
                        'textfield="BondRatingRightModal.agency_name" textvalue="' +
                        agency_text +
                        '" ' +
                        'valuefield="BondRatingRightModal.agency_code" value="' +
                        agency_val +
                        '">' +
                        '<input name="BondRating[' +
                        rowindex +
                        '].agency_name" type="hidden" Isdropdown="true" ' +
                        'textfield="BondRatingRightModal.agency_name" textvalue="' +
                        agency_text +
                        '" ' +
                        'valuefield="BondRatingRightModal.agency_code" value="' +
                        agency_val +
                        '">' +
                        '</td >');

                    var ColShortLongTerm = $('<td class="long-data">' +
                        short_long_term_text +
                        '<input name="BondRating[' +
                        rowindex +
                        '].short_long_term" type="hidden" Isdropdown="true" ' +
                        'textfield="BondRatingRightModal.short_long_term_text" textvalue="' +
                        short_long_term_val +
                        '" ' +
                        'valuefield="BondRatingRightModal.short_long_term" value="' +
                        short_long_term_val +
                        '">' +
                        '<input name="BondRating[' +
                        rowindex +
                        '].short_long_term_text" type="hidden" Isdropdown="true" ' +
                        'textfield="BondRatingRightModal.short_long_term_text" textvalue="' +
                        short_long_term_val +
                        '" ' +
                        'valuefield="BondRatingRightModal.short_long_term" value="' +
                        short_long_term_val +
                        '">' +
                        '</td > ');

                    var ColLocalRating = $('<td class="long-data">' +
                        local_rating_text +
                        '<input name="BondRating[' +
                        rowindex +
                        '].local_rating" type="hidden" Isdropdown="true" ' +
                        'textfield="BondRatingRightModal.local_rating_text" textvalue="' +
                        local_rating_text +
                        '" ' +
                        'valuefield="BondRatingRightModal.local_rating" value="' +
                        local_rating_val +
                        '">' +
                        '<input name="BondRating[' +
                        rowindex +
                        '].local_rating_text" type="hidden" Isdropdown="true" ' +
                        'textfield="BondRatingRightModal.local_rating_text" textvalue="' +
                        local_rating_text +
                        '" ' +
                        'valuefield="BondRatingRightModal.local_rating" value="' +
                        local_rating_text +
                        '">' +
                        '</td>');

                    var ColForeignRating = $('<td class="long-data">' +
                        foreign_rating_text +
                        '<input name="BondRating[' +
                        rowindex +
                        '].foreign_rating" type="hidden" Isdropdown="true" ' +
                        'textfield="BondRatingRightModal.foreign_rating_text" textvalue="' +
                        foreign_rating_text +
                        '" ' +
                        'valuefield="BondRatingRightModal.foreign_rating" value="' +
                        foreign_rating_val +
                        '">' +
                        '<input name="BondRating[' +
                        rowindex +
                        '].foreign_rating_text" type="hidden" Isdropdown="true" ' +
                        'textfield="BondRatingRightModal.foreign_rating_text" textvalue="' +
                        foreign_rating_text +
                        '" ' +
                        'valuefield="BondRatingRightModal.foreign_rating" value="' +
                        foreign_rating_text +
                        '">' +
                        '</td > ');

                    var ColAssessDate = $('<td>' +
                        assess_date_val +
                        '<input name="BondRating[' +
                        rowindex +
                        '].assess_date" Isdropdown="false" type="hidden" value="' +
                        assess_date_val +
                        '"></td>');

                    var ColRowStatus = $('<td><input name="BondRating[' +
                        rowindex +
                        '].rowstatus" Isdropdown="false"  type="hidden" value="create"></td>');

                    var ColAction = '<td class="action">' +
                        '<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                        agency_val +
                        '" data-action="update" onclick="GM.Security.BondRating.Form.Show(this)" >' +
                        '<i class="feather-icon icon-edit"></i>' +
                        '</button > ' +
                        '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                        agency_val +
                        '" data-action="delete" onclick="GM.Security.BondRating.Form.Show(this)" >' +
                        '<i class="feather-icon icon-trash-2"></i>' +
                        '</button>' +
                        '</td>';

                    row.append(ColAgencyCode);
                    row.append(ColShortLongTerm);
                    row.append(ColLocalRating);
                    row.append(ColForeignRating);
                    row.append(ColAssessDate);
                    row.append(ColRowStatus);
                    row.append(ColAction);

                    GM.Security.BondRating.Table.append(row);
                    GM.Security.BondRating.Table.find(".empty-data").parent().remove();

                   
                } else {
                    GM.Message.Error('.modal-body', "The Data already exists.");
                    IsInValid = true;
                }

                break;

            case 'update':
                var agency_text = $('#BondRatingRightModal_agency_name').text();
                var agency_val = $("#BondRatingRightModal_agency_code").val();
                var short_long_term_text = $('#BondRatingRightModal_short_long_term_text').text();
                var short_long_term_val = $("#BondRatingRightModal_short_long_term").val();
                var local_rating_text = $('#BondRatingRightModal_local_rating_text').text();
                var local_rating_val = $("#BondRatingRightModal_local_rating").val();
                var foreign_rating_text = $('#BondRatingRightModal_foreign_rating_text').text();
                var foreign_rating_val = $("#BondRatingRightModal_foreign_rating").val();
                var assess_date_val = $("#BondRatingRightModal_assess_date").val();

                if (agency_val == "") {
                    $("#agency_code_error").text("The Agency field is required.");
                    IsInValid = true;
                }
                if (short_long_term_val == "") {
                    $("#short_long_term_error").text("The Term field is required.");
                    IsInValid = true;
                }
                
                if (assess_date_val == "") {
                    $("#assess_date_error").text("The Assess Date field is required.");
                    IsInValid = true;
                }

                if (IsInValid) {
                    break;
                }

                var rowindex = GM.Security.BondRating.Table.find("tr:last").data("id");

                var row = GM.Security.BondRating.Table.RowSelected;
                var rowindex = row.data("id");

                var rowfrom = row.data("action");
                var textstatus = "create";
                if (rowfrom == "fromdatabase") {
                    textstatus = "update";
                }

                if (!GM.Security.BondRating.Table.RowAny(agency_val, null, rowindex)) {
                   
                    var ColAgencyCode = row.children("td:nth-child(1)");
                    var ColShortLongTerm = row.children("td:nth-child(2)");
                    var ColLocalRating = row.children("td:nth-child(3)");
                    var ColForeignRating = row.children("td:nth-child(4)");
                    var ColAssessDate = row.children("td:nth-child(5)");
                    var ColRowStatus = row.children("td:nth-child(6)");
                    var ColAction = row.children("td:nth-child(7)");

                    ColAgencyCode.html(agency_text +
                        '<input name="BondRating[' +
                        rowindex +
                        '].agency_code" type="hidden" Isdropdown="true" ' +
                        'textfield="BondRatingRightModal.agency_name" textvalue="' +
                        agency_text +
                        '" ' +
                        'valuefield="BondRatingRightModal.agency_code" value="' +
                        agency_val +
                        '">' +
                        '<input name="BondRating[' +
                        rowindex +
                        '].agency_name" type="hidden" Isdropdown="true" ' +
                        'textfield="BondRatingRightModal.agency_name" textvalue="' +
                        agency_text +
                        '" ' +
                        'valuefield="BondRatingRightModal.agency_code" value="' +
                        agency_val +
                        '">');

                    ColShortLongTerm.html(short_long_term_text +
                        '<input name="BondRating[' +
                        rowindex +
                        '].short_long_term" type="hidden" Isdropdown="true" ' +
                        'textfield="BondRatingRightModal.short_long_term_text" textvalue="' +
                        short_long_term_val +
                        '" ' +
                        'valuefield="BondRatingRightModal.short_long_term" value="' +
                        short_long_term_val +
                        '">' +
                        '<input name="BondRating[' +
                        rowindex +
                        '].short_long_term_text" type="hidden" Isdropdown="true" ' +
                        'textfield="BondRatingRightModal.short_long_term_text" textvalue="' +
                        short_long_term_val +
                        '" ' +
                        'valuefield="BondRatingRightModal.short_long_term" value="' +
                        short_long_term_val +
                        '">');

                    ColLocalRating.html(local_rating_text +
                        '<input name="BondRating[' +
                        rowindex +
                        '].local_rating" type="hidden" Isdropdown="true" ' +
                        'textfield="BondRatingRightModal.local_rating_text" textvalue="' +
                        local_rating_text +
                        '" ' +
                        'valuefield="BondRatingRightModal.local_rating" value="' +
                        local_rating_val +
                        '">' +
                        '<input name="BondRating[' +
                        rowindex +
                        '].local_rating_text" type="hidden" Isdropdown="true" ' +
                        'textfield="BondRatingRightModal.local_rating_text" textvalue="' +
                        local_rating_text +
                        '" ' +
                        'valuefield="BondRatingRightModal.local_rating" value="' +
                        local_rating_text +
                        '">');

                    ColForeignRating.html(foreign_rating_text +
                        '<input name="BondRating[' +
                        rowindex +
                        '].foreign_rating" type="hidden" Isdropdown="true" ' +
                        'textfield="BondRatingRightModal.foreign_rating_text" textvalue="' +
                        foreign_rating_text +
                        '" ' +
                        'valuefield="BondRatingRightModal.foreign_rating" value="' +
                        foreign_rating_val +
                        '">' +
                        '<input name="BondRating[' +
                        rowindex +
                        '].foreign_rating_text" type="hidden" Isdropdown="true" ' +
                        'textfield="BondRatingRightModal.foreign_rating_text" textvalue="' +
                        foreign_rating_text +
                        '" ' +
                        'valuefield="BondRatingRightModal.foreign_rating" value="' +
                        foreign_rating_text +
                        '">');

                    ColAssessDate.html(assess_date_val +
                        '<input name="BondRating[' +
                        rowindex +
                        '].assess_date" Isdropdown="false"  type="hidden" value="' +
                        assess_date_val +
                        '">');

                    ColRowStatus.html('<input name="BondRating[' +
                        rowindex +
                        '].rowstatus" Isdropdown="false"  type="hidden" value="' +
                        textstatus +
                        '">');

                    var htmlAction = '<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                        agency_val +
                        '" data-action="update" onclick="GM.Security.BondRating.Form.Show(this)" >' +
                        '<i class="feather-icon icon-edit"></i>' +
                        '</button > ' +
                        '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                        agency_val +
                        '" data-action="delete" onclick="GM.Security.BondRating.Form.Show(this)" >' +
                        '<i class="feather-icon icon-trash-2"></i>' +
                        '</button>';
                    ColAction.html(htmlAction);
                   
                } else {
                    GM.Message.Error('.modal-body', "The Data already exists.");
                    IsInValid = true;
                }
                break;

            case 'delete':

                var rowselect = GM.Security.BondRating.Table.RowSelected;
                var rowfrom = rowselect.data("action");
                var rowid = rowselect.data("id");
                if (rowfrom != "fromdatabase") {
                    GM.Security.BondRating.Table.RowSelected.remove();
                } else {
                    rowselect.attr("style", "display: none");
                    var inputs = $(rowselect).find('input,select,textarea');
                    if (inputs.length > 0) {
                        $.each(inputs,
                            function () {
                                var names = this.name.split('.');
                                //update new input name                                
                                if (names[1] == "rowstatus") {
                                    var inputname = 'BondRating[' + rowid + '].' + names[1];
                                    $(this).attr('name', inputname);
                                    $(this).attr("value", "delete");
                                    $(this).attr("type", "hidden");
                                    $(this).text("delete");
                                }
                            });
                    }
                }
                //renew index of input
                var rows = GM.Security.BondRating.Table.find('tr');
                var rowindex = 0;
                var checkrow = 0;

                rows.each(function (index, row) {

                    var inputs = $(row).find('input,select,textarea');

                    if (inputs.length > 0) {

                        $.each(inputs,
                            function () {

                                var names = this.name.split('.');
                                var inputname = 'BondRating[' + rowindex + '].' + names[1];

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
                    //GM.Security.BondRating.Table.attr("style", "display: none");
                    GM.Security.BondRating.Table.RowEmpty();
                }

                break;

        }

        //GM.Security.BondRating.Table.RowSelected = {};
        if (IsInValid == false) {
            GM.Security.BondRating.Form.modal('toggle');
        }
    };
    GM.Security.BondRating.Form.Initial = function () {
        if (GM.Security.BondRating.Table.RowSelected) {
            var inputs = $(GM.Security.BondRating.Table.RowSelected).find('input,select,textarea');
            if (inputs.length > 0) {
                $.each(inputs, function () {
                    var names = this.name.split('.');
                    if (this.attributes.isdropdown.value == 'true') {
                        var filedtextid = this.attributes.textfield.value.split('.');
                        $("#BondRatingForm span[name='" + filedtextid[1] + "']").text(this.attributes.textvalue.value);
                        $("#BondRatingForm input[name='" + this.attributes.textfield.value + "']").val(this.attributes.textvalue.value);
                        $("#BondRatingForm input[name='" + this.attributes.textfield.value + "']").text(this.attributes.textvalue.value);
                        $("#BondRatingForm input[name='" + this.attributes.valuefield.value + "']").val(this.value);
                        //$("#BondRatingForm input[name='" + this.attributes.valuefield.value + "']").attr("style", "pointer-events: none");
                        switch (names[1]) {
                            case 'agency_code':
                                $("#ddl_agency").attr("disabled", "disabled");
                                break;
                            ////case 'short_long_term':
                            ////    $("#ddl_term").attr("style", "pointer-events: none");
                            ////    break;
                            ////case 'local_rating':
                            ////    $("#ddl_localrating").attr("style", "pointer-events: none");
                            ////    break;
                            ////case 'foreign_rating':
                            ////    $("#ddl_foreignrating").attr("style", "pointer-events: none");
                            ////    break;
                        }
                    } else {
                        //console.log('$(\'#' + names[1] + '\').val(' + this.value + ');');
                        //$('#' + names[1]).val(this.value);   
                        $("#BondRatingForm input[name='BondRatingRightModal." + names[1] + "']").val(this.value);
                        $("#BondRatingForm input[name='BondRatingRightModal." + names[1] + "']").text(this.value);
                    }
                });
            }
        }
    };
    GM.Security.BondRating.Form.Reset = function () {

        $("#ddl_agency").find(".selected-data").text("Select...");
        $("#ddl_agency").removeAttr("style");
        $('#BondRatingRightModal_agency_name').text(null);
        $("#BondRatingRightModal_agency_code").val(null);

        $("#ddl_term").find(".selected-data").text("Select...");
        $("#ddl_term").removeAttr("style");
        $('#BondRatingRightModal_short_long_term_text').text(null);
        $("#BondRatingRightModal_short_long_term").val(null);

        $("#ddl_localrating").find(".selected-data").text("Select...");
        $("#ddl_localrating").removeAttr("style");
        $('#BondRatingRightModal_local_rating_text').text(null);
        $("#BondRatingRightModal_local_rating").val(null);

        $("#ddl_foreignrating").find(".selected-data").text("Select...");
        $("#ddl_foreignrating").removeAttr("style");
        $('#BondRatingRightModal_foreign_rating_text').text(null);
        $("#BondRatingRightModal_foreign_rating").val(null);

        $('#BondRatingRightModal_assess_date').text(null);
        $("#BondRatingRightModal_assess_date").val(null);

        $("#agency_code_error").text("");
        $("#short_long_term_error").text("");
        $("#local_rating_error").text("");
        $("#foreign_rating_error").text("");
        $("#assess_date_error").text("");

        $('#BondRatingRightModal_assess_date').removeClass("input-validation-error");

        GM.Message.Clear();
    };

    $('#btnBondRatingCreate').on('click', function () {
        GM.Security.BondRating.Form.Show(this);
    });

    $('#btnBondRatingSave').on('click', function () {
        GM.Security.BondRating.Form.Save(this);
    });
    //End Bond Rating

    //Cash Flow Logic
    GM.Security.CashFlow = {};
    GM.Security.CashFlow.Table = $('#x-table-data-cashflow').DataTable({
        createdRow: function (row, data, index) {
            if (data["action"] == "delete") {
                $('td', row).parent().attr("style", "display:none");
            }
            $('td', row).parent().attr("data-id", index);
        },
        pageLength: 50,
        dom: 'Bfrtip',
        select: false,
        searching: true,
        scrollY: '80vh',
        scrollX: true,
        //order: [
        //    [1, "asc"]
        //],
        buttons:
            [
                //{
                //    text: 'Refresh',
                //    action: function (e, dt, node, config) {
                //        dt.ajax.reload();
                //    }
                //}
            ],
        processing: true,
        serverSide: true,
        ajax: {
            "url": "/Security/CreatCashFlow",
            "type": "POST",
            "data": {
                instrument_id: $('#instrument_id').val()
            }
        },
        //select: {
        //    style: 'os',
        //    selector: 'td:first-child'
        //},
        columnDefs:
            [
                {
                    targets: 0, data: "round_no", orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            var valuedata = data;
                            html = valuedata + '<input name="Events[' + meta.row + '].round_no" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                            return html;
                        }
                        return html;
                    }
                },
                {
                    targets: 1, data: "event_date", orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            var valuedata = moment(data).format('DD/MM/YYYY');
                            html = valuedata + '<input name="Events[' + meta.row + '].event_date" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                            return html;
                        }
                        return html;
                    }
                },
                {
                    targets: 2, data: "payment_date", orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            var valuedata = moment(data).format('DD/MM/YYYY');
                            html = valuedata + '<input name="Events[' + meta.row + '].payment_date" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                            return html;
                        }
                        return html;
                    }
                },
                {
                    targets: 3, data: "xi_date", orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            var valuedata = moment(data).format('DD/MM/YYYY');
                            html = valuedata + '<input name="Events[' + meta.row + '].xi_date" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                            return html;
                        }
                        return html;
                    }
                },
                {
                    targets: 4, data: "start_date",// coupon_date
                    orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            var valuedata = moment(data).format('DD/MM/YYYY');
                            html = valuedata + '<input name="Events[' + meta.row + '].start_date" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                            return html;
                        }
                        return html;
                    }
                },
                {
                    targets: 5, data: "end_date",//next_coupon_date
                    orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            var valuedata = moment(data).format('DD/MM/YYYY');
                            html = valuedata + '<input name="Events[' + meta.row + '].end_date" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                            return html;
                        }
                        return html;
                    }
                },
                {
                    targets: 6, data: "begining_par", orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            var valuedata = data;
                            html = valuedata + '<input name="Events[' + meta.row + '].begining_par" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                            return html;
                        }
                        return html;
                    }
                },
                {
                    targets: 7, data: "ending_par", orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            var valuedata = data;
                            html = valuedata + '<input name="Events[' + meta.row + '].ending_par" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                            return html;
                        }
                        return html;
                    }
                },
                {
                    targets: 8, data: "coupon_rate", orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            var valuedata = FormatDecimal(data, 6);
                            html = valuedata + '<input name="Events[' + meta.row + '].coupon_rate" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                            return html;
                        }
                        return html;
                    }
                },
                {
                    targets: 9, data: "interest", orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            var valuedata = FormatDecimal(data, 6);
                            html = valuedata + '<input name="Events[' + meta.row + '].interest" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                            return html;
                        }
                        return html;
                    }
                },
                {
                    targets: 10, data: "principal", orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            var valuedata = FormatDecimal(data, 6);
                            html = valuedata + '<input name="Events[' + meta.row + '].principal" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                            return html;
                        }
                        return html;
                    }
                },
                {
                    targets: 11, data: "total_payment", orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            var valuedata = FormatDecimal(data, 6);
                            html = valuedata + '<input name="Events[' + meta.row + '].total_payment" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                            return html;
                        }
                        return html;
                    }
                },
                {
                    targets: 12, data: "event_type", orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            var valuedata = data;
                            html = valuedata + '<input name="Events[' + meta.row + '].event_type" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                            return html;
                        }
                        return html;
                    }
                },
                {
                    targets: 13, data: "coupon_type", orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            var valuedata = data;
                            html = valuedata + '<input name="Events[' + meta.row + '].coupon_type" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                            return html;
                        }
                        return html;
                    }
                },
                {
                    targets: 14, data: "coupon_floating_index_code", orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            var valuedata = data;
                            html = valuedata + '<input name="Events[' + meta.row + '].coupon_floating_index_code" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                            return html;
                        }
                        return html;
                    }
                },
                {
                    targets: 15, data: "coupon_spread", orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            var valuedata = data;
                            html = valuedata + '<input name="Events[' + meta.row + '].coupon_spread" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                            return html;
                        }
                        return html;
                    }
                },
                {
                    targets: 16, data: "ai_amount", orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            var valuedata = data;
                            html = valuedata + '<input name="Events[' + meta.row + '].ai_amount" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                            return html;
                        }
                        return html;
                    }
                },
                {
                    targets: 17, data: "redemption_percent", orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            var valuedata = data;
                            html = valuedata + '<input name="Events[' + meta.row + '].redemption_percent" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                            return html;
                        }
                        return html;
                    }
                },
                {
                    targets: 18, data: "complete_flag", orderable: false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        var valuedata = data;
                        html = valuedata + '<input name="Events[' + meta.row + '].complete_flag" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                        return html;
                    }
                },
                {
                    targets: 19, data: "datafrom", "visible": false,
                    render: function (data, type, row, meta) {
                        var html = '';
                        if (data != null) {
                            var valuedata = data;
                            html = valuedata + '<input name="Events[' + meta.row + '].datafrom" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                        }
                        return html;
                    }
                },
                {
                    targets: 20, data: "rowno", "visible": false,
                    render: function (data, type, row, meta) {

                        var html = '<input name="Events[' + meta.row + '].rowno" Isdropdown="false"  type="hidden" value="' + meta.row + '">';
                        return html;
                    }
                },
                {
                    targets: 21, data: "action", "visible": false,
                    render: function (data, type, row, meta) {
                        var valuedata = data;
                        var html = '<input name="Events[' + meta.row + '].action" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                        return html;
                    }
                },
                {
                    targets: 22, data: "instrument_id", "visible": false,
                    render: function (data, type, row, meta) {
                        var valuedata = data;
                        var html = '<input name="Events[' + meta.row + '].instrument_id" Isdropdown="false"  type="hidden" value="' + valuedata + '">';
                        return html;
                    }
                }
                //{
                //    targets: 23, data: "round_no", className: "dt-body-center",
                //    width: 60,
                //    render: function (data, type, row, meta) {
                //        var html = '';

                //        if (isUpdate == "True") {
                //            html += '<button class="btn btn-default btn-round" key="' + row.event_date + '" form-mode="edit" id="update_' + meta.row + '" datafrom="' + row.datafrom + '"  data-action="update"  onclick="GM.Security.CashFlow.Form.Show(this)" ><i class="feather-icon icon-edit"></i></button>';
                //            html += '<button class="btn btn-delete  btn-round" key="' + row.event_date + '" form-mode="delete" id="delete_' + meta.row + '" datafrom="' + row.datafrom + '"    data-action="delete"  onclick="GM.Security.CashFlow.Form.Show(this)" ><i class="feather-icon icon-trash-2"></i></button>';
                //        }
                //        else {
                //            html += '<button class="btn btn-default btn-round" key="' + row.event_date + '" form-mode="edit" id="update_' + meta.row + '" datafrom="' + row.datafrom + '"  data-action="update"  onclick="GM.Security.CashFlow.Form.Show(this)" disabled ><i class="feather-icon icon-edit"></i></button>';
                //            html += '<button class="btn btn-delete  btn-round" key="' + row.event_date + '" form-mode="delete" id="delete_' + meta.row + '" datafrom="' + row.datafrom + '"    data-action="delete"  onclick="GM.Security.CashFlow.Form.Show(this)" disabled ><i class="feather-icon icon-trash-2"></i></button>';
                //        }


                //        return html;
                //    }
                //}
            ],
        fixedColumns: {
            leftColumns: 1,
            rightColumns: 1
        }
    });
    GM.Security.CashFlow.Form = $("#modal-form-cashflow");
    GM.Security.CashFlow.Form.ButtonSave = $('#btnCashFlowSave');
    GM.Security.CashFlow.Form.Show = function (btn) {

        var action = $(btn).data("action");
        var button = $('#btnCashFlowSave');
        GM.Security.CashFlow.Form.Reset();

        switch (action) {
            case 'create':
                $("#SecurityEventRightModal_action").val("create");
                GM.Security.CashFlow.Form.find(".modal-title").html("Create");
                GM.Security.CashFlow.Form.ButtonSave.data("action", "create");
                GM.Security.CashFlow.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('+ Add');
                break;

            case 'update':
                var id = $(btn).attr("id");
                var realbtn = $("#x-table-data-cashflow").find("button#" + id);
                var rowindex = id.replace("update_", "");
                $("#SecurityEventRightModal_action").val("update");
                GM.Security.CashFlow.Table.RowSelected = realbtn.parent().parent();
                GM.Security.CashFlow.Form.find(".modal-title").html("Update");
                GM.Security.CashFlow.Form.Initial();
                GM.Security.CashFlow.Form.ButtonSave.attr("rowindex", rowindex);
                GM.Security.CashFlow.Form.ButtonSave.data("action", "update");
                GM.Security.CashFlow.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('Add');
                break;

            case 'delete':
                var id = $(btn).attr("id");
                var realbtn = $("#x-table-data-cashflow").find("button#" + id);
                var rowindex = id.replace("delete_", "");
                $("#SecurityEventRightModal_action").val("delete");
                GM.Security.CashFlow.Table.RowSelected = realbtn.parent().parent();
                GM.Security.CashFlow.Form.find(".modal-title").html("Delete");
                GM.Security.CashFlow.Form.Initial();
                GM.Security.CashFlow.Form.ButtonSave.attr("rowindex", rowindex);
                GM.Security.CashFlow.Form.ButtonSave.data("action", "delete");
                GM.Security.CashFlow.Form.ButtonSave.removeClass('btn-primary').addClass('btn-delete').text('Confirm Delete');
                break;

            default:
                break;
        }

        GM.Security.CashFlow.Form.modal('toggle');
    };
    GM.Security.CashFlow.Form.Initial = function () {
        if (GM.Security.CashFlow.Table.RowSelected) {
            var inputs = $(GM.Security.CashFlow.Table.RowSelected).find('input,select,textarea');
            if (inputs.length > 0) {
                $.each(inputs, function () {
                    var names = this.name.split('.');
                    if (this.attributes.isdropdown.value == 'true') {
                        var filedtextid = this.attributes.textfield.value.split('.');
                        $("#CashFlowForm span[name='" + filedtextid[1] + "']").text(this.attributes.textvalue.value);
                        $("#CashFlowForm input[name='" + this.attributes.textfield.value + "']").val(this.attributes.textvalue.value);
                        $("#CashFlowForm input[name='" + this.attributes.textfield.value + "']").text(this.attributes.textvalue.value);
                        $("#CashFlowForm input[name='" + this.attributes.valuefield.value + "']").val(this.value);
                    } else {
                        //console.log('$(\'#' + names[1] + '\').val(' + this.value + ');');
                        //$('#' + names[1]).val(this.value);

                        if (names[1] == "start_date") {
                            if ($("#SecurityEventRightModal_start_date").val().length > 0) {
                                $("#SecurityEventRightModal_end_date").data("DateTimePicker").minDate(this.value);
                            }
                            $("#CashFlowForm input[name='SecurityEventRightModal." + names[1] + "']").val(this.value);
                            $("#CashFlowForm input[name='SecurityEventRightModal." + names[1] + "']").text(this.value);
                        }
                        else if (names[1] == "end_date") {
                            if ($("#SecurityEventRightModal_end_date").val().length > 0) {
                                $("#SecurityEventRightModal_start_date").data("DateTimePicker").maxDate(this.value);
                            }
                            $("#CashFlowForm input[name='SecurityEventRightModal." + names[1] + "']").val(this.value);
                            $("#CashFlowForm input[name='SecurityEventRightModal." + names[1] + "']").text(this.value);
                        }
                        else if (names[1] == "complete_flag") {
                            var activeFlagId = $('input:radio[name = "SecurityEventRightModal.complete_flag"]');
                            if (this.value == "false") {
                                activeFlagId.filter('[value="false"]').prop("checked", true).attr("checked", "checked").attr('ischeck', 'true');
                                activeFlagId.filter('[value="true"]').prop("checked", false).removeAttr("checked").attr('ischeck', 'false');
                            }
                        }
                        else {
                            $("#CashFlowForm input[name='SecurityEventRightModal." + names[1] + "']").val(this.value);
                            $("#CashFlowForm input[name='SecurityEventRightModal." + names[1] + "']").text(this.value);
                        }
                    }
                });
            }

        }
    };
    GM.Security.CashFlow.Form.Save = function (btn) {
        var action = $(btn).data("action");
        GM.Security.CashFlow.Table.columns(19).search("");
        var round_no = $("#SecurityEventRightModal_round_no").val();
        var coupontype = $("#SecurityEventRightModal_coupon_type").val();

        var IsInValid = true;
        var event_date = $("#SecurityEventRightModal_event_date").val();
        var payment_date = $("#SecurityEventRightModal_payment_date").val();
        var start_date = $("#SecurityEventRightModal_start_date").val();
        var end_date = $("#SecurityEventRightModal_end_date").val();

        var begining_par = $("#SecurityEventRightModal_begining_par").val();
        var ending_par = $("#SecurityEventRightModal_ending_par").val();
        var couponRate = $("#SecurityEventRightModal_coupon_rate").val();

        var event_type = $("#SecurityEventRightModal_event_type").val();
        var round_no = $("#SecurityEventRightModal_round_no").val();

        var complete_flag = $('input:radio[name = "SecurityEventRightModal.complete_flag"]').filter(':checked').val();
        GM.Message.Clear();

        $("#coupon_rate_text_error").text("");
        $("#begining_par_error").text("");
        $("#ending_par_error").text("");

        $("#event_date_error").text("");
        $("#payment_date_error").text("");
        $("#start_date_error").text("");
        $("#end_date_error").text("");

        $("#event_type_text_error").text("");
        $("#round_no_text_error").text("");

        if (couponRate.trim() == "") {
            $("#coupon_rate_text_error").text("The Coupon Rate field is required.");
            IsInValid = false;
        }
        if (begining_par.trim() == "") {
            $("#begining_par_error").text("The Begining Per field is required.");
            IsInValid = false;
        }
        if (ending_par.trim() == "") {
            $("#ending_par_error").text("The Ending Per field is required.");
            IsInValid = false;
        }

        if (event_date.trim() == "") {
            $("#event_date_error").text("The Event Date field is required.");
            IsInValid = false;
        }
        if (payment_date.trim() == "") {
            $("#payment_date_error").text("The Payment Date field is required.");
            IsInValid = false;
        }
        if (start_date.trim() == "") {
            $("#start_date_error").text("The Start Date field is required.");
            IsInValid = false;
        }
        if (end_date.trim() == "") {
            $("#end_date_error").text("The End Date field is required.");
            IsInValid = false;
        }

        if (event_type.trim() == "") {
            $("#event_type_text_error").text("The Event Type field is required.");
            IsInValid = false;
        }

        if (round_no.trim() == "") {
            $("#round_no_text_error").text("The Round No field is required.");
            IsInValid = false;
        }

        if (IsInValid) {
            var SecurityEventRightModal_action = $("#SecurityEventRightModal_action");
            SecurityEventRightModal_action.value = action;
            switch (action) {
                case 'create':
                    if (!GM.Security.CashFlow.Table.RowAny(parseInt(round_no), coupontype)) {
                        GM.Security.CashFlow.Table.columns(0).search(parseInt(round_no));
                        GM.Security.CashFlow.Table.columns(1).search($("#SecurityEventRightModal_event_date").val());
                        GM.Security.CashFlow.Table.columns(2).search($("#SecurityEventRightModal_payment_date").val());
                        GM.Security.CashFlow.Table.columns(3).search($("#SecurityEventRightModal_xi_date").val());
                        GM.Security.CashFlow.Table.columns(4).search($("#SecurityEventRightModal_start_date").val());
                        GM.Security.CashFlow.Table.columns(5).search($("#SecurityEventRightModal_end_date").val());
                        GM.Security.CashFlow.Table.columns(6).search($("#SecurityEventRightModal_begining_par").val());
                        GM.Security.CashFlow.Table.columns(7).search($("#SecurityEventRightModal_ending_par").val());
                        GM.Security.CashFlow.Table.columns(8).search($("#SecurityEventRightModal_coupon_rate").val());
                        GM.Security.CashFlow.Table.columns(9).search($("#SecurityEventRightModal_interest").val());
                        GM.Security.CashFlow.Table.columns(10).search($("#SecurityEventRightModal_principal").val());
                        GM.Security.CashFlow.Table.columns(11).search($("#SecurityEventRightModal_total_payment").val());
                        GM.Security.CashFlow.Table.columns(12).search($("#SecurityEventRightModal_event_type").val());
                        GM.Security.CashFlow.Table.columns(13).search(coupontype);
                        GM.Security.CashFlow.Table.columns(14).search($("#SecurityEventRightModal_coupon_floating_index_code").val());
                        GM.Security.CashFlow.Table.columns(15).search($("#SecurityEventRightModal_coupon_spread").val());
                        GM.Security.CashFlow.Table.columns(16).search($("#SecurityEventRightModal_ai_amount").val());
                        GM.Security.CashFlow.Table.columns(17).search($("#SecurityEventRightModal_redemption_percent").val());
                        GM.Security.CashFlow.Table.columns(18).search(complete_flag);
                        GM.Security.CashFlow.Table.columns(19).search("fromui");
                        GM.Security.CashFlow.Table.columns(20).search($("#SecurityEventRightModal_rowno").val());
                        GM.Security.CashFlow.Table.columns(21).search("create");
                        GM.Security.CashFlow.Table.columns(22).search("");
                        GM.Security.CashFlow.Table.draw();
                        adjusttable();
                        break;
                    }
                    else {
                        //swal("Error", "The data Dupplicate Key round_no , coupon type", "error");
                        GM.Message.Error('.modal-body', "The Data already exists.");
                        break;
                    }
                case 'update':
                    var rowindex = $(btn).attr("rowindex");
                    if (!GM.Security.CashFlow.Table.RowAny(round_no, coupontype, rowindex)) {
                        GM.Security.CashFlow.Table.columns(0).search(parseInt(round_no));
                        GM.Security.CashFlow.Table.columns(1).search($("#SecurityEventRightModal_event_date").val());
                        GM.Security.CashFlow.Table.columns(2).search($("#SecurityEventRightModal_payment_date").val());
                        GM.Security.CashFlow.Table.columns(3).search($("#SecurityEventRightModal_xi_date").val());
                        GM.Security.CashFlow.Table.columns(4).search($("#SecurityEventRightModal_start_date").val());
                        GM.Security.CashFlow.Table.columns(5).search($("#SecurityEventRightModal_end_date").val());
                        GM.Security.CashFlow.Table.columns(6).search($("#SecurityEventRightModal_begining_par").val());
                        GM.Security.CashFlow.Table.columns(7).search($("#SecurityEventRightModal_ending_par").val());
                        GM.Security.CashFlow.Table.columns(8).search($("#SecurityEventRightModal_coupon_rate").val());
                        GM.Security.CashFlow.Table.columns(9).search($("#SecurityEventRightModal_interest").val());
                        GM.Security.CashFlow.Table.columns(10).search($("#SecurityEventRightModal_principal").val());
                        GM.Security.CashFlow.Table.columns(11).search($("#SecurityEventRightModal_total_payment").val());
                        GM.Security.CashFlow.Table.columns(12).search($("#SecurityEventRightModal_event_type").val());
                        GM.Security.CashFlow.Table.columns(13).search($("#SecurityEventRightModal_coupon_type").val());
                        GM.Security.CashFlow.Table.columns(14).search($("#SecurityEventRightModal_coupon_floating_index_code").val());
                        GM.Security.CashFlow.Table.columns(15).search($("#SecurityEventRightModal_coupon_spread").val());
                        GM.Security.CashFlow.Table.columns(16).search($("#SecurityEventRightModal_ai_amount").val());
                        GM.Security.CashFlow.Table.columns(17).search($("#SecurityEventRightModal_redemption_percent").val());
                        GM.Security.CashFlow.Table.columns(18).search(complete_flag);
                        GM.Security.CashFlow.Table.columns(19).search($("#SecurityEventRightModal_datafrom").val());
                        GM.Security.CashFlow.Table.columns(20).search(rowindex);
                        GM.Security.CashFlow.Table.columns(21).search(action);
                        GM.Security.CashFlow.Table.columns(22).search("");
                        GM.Security.CashFlow.Table.draw();
                        break;
                    }
                    else {
                        //swal("Error", "The data Dupplicate Key round_no , coupon type", "error");
                        GM.Message.Error('.modal-body', "The Data already exists.");
                        break;
                    }
                case 'delete':
                    var rowindex = $(btn).attr("rowindex");
                    GM.Security.CashFlow.Table.columns(22).search("");
                    GM.Security.CashFlow.Table.columns(0).search($("#SecurityEventRightModal_round_no").val());
                    GM.Security.CashFlow.Table.columns(20).search(rowindex);
                    GM.Security.CashFlow.Table.columns(21).search(action);
                    GM.Security.CashFlow.Table.draw();
                    //var rowselect = GM.Security.CashFlow.Table.RowSelected;
                    //var rowfrom = rowselect.find("button").attr("datafrom");
                    //var rowid = rowselect.find("button").attr("id");
                    //rowid = rowid.substring(7, rowid.length);
                    //if (rowfrom != "fromdatabase") {
                    //    GM.Security.CashFlow.Table.RowSelected.remove();
                    //}
                    //else {
                    //    rowselect.attr("style", "display: none");
                    //    var inputs = $(rowselect).find('input,select,textarea');
                    //    if (inputs.length > 0) {
                    //        $.each(inputs, function () {
                    //            var names = this.name.split('.');
                    //            //update new input name                                
                    //            if (names[1] == "rowstatus") {
                    //                var inputname = 'Events[' + rowid + '].' + names[1];
                    //                $(this).attr('name', inputname);
                    //                $(this).attr("value", "delete");
                    //                $(this).attr("type", "hidden");
                    //                $(this).text("delete");
                    //            }
                    //        });
                    //    }
                    //}
                    ////renew index of input
                    //var rows = GM.Security.CashFlow.Table.find('tr');
                    //var rowindex = 0;
                    //var checkrow = 0;

                    //rows.each(function (index, row) {

                    //    var inputs = $(row).find('input,select,textarea');

                    //    if (inputs.length > 0) {

                    //        $.each(inputs, function () {

                    //            var names = this.name.split('.');
                    //            var inputname = 'Events[' + rowindex + '].' + names[1];

                    //            //update new input name
                    //            $(this).attr('name', inputname);
                    //            if (names[1] == "rowstatus" && $(this).val() != "delete") {
                    //                checkrow++;
                    //            }

                    //        });

                    //        rowindex++
                    //    }

                    //});

                    //if (checkrow == 0) {
                    //GM.Security.CashFlow.Table.attr("style", "display: none");
                    //    GM.Security.CashFlow.Table.RowEmpty()
                    //}
                    break;
            }
            GM.Security.CashFlow.Form.modal('toggle');
        }
    };

    GM.Security.CashFlow.Table.RowAny = function (code, code2, rowid) {

        var rows = $("#x-table-data-cashflow").find("tr");
        var IsExits = false;
        var IsExits2 = false;
        var Isdupplicate = false;
        rows.each(function (index, row) {

            var rowindex = $(row).data("id");
            var inputs = $(row).find('input,select,textarea');

            if (inputs.length > 0) {

                $.each(inputs, function () {

                    var names = this.name.split('.');

                    if (names[1] == 'round_no' && this.value == code) {

                        if (rowid != rowindex) {
                            IsExits = true;
                            //return false
                        }
                    }
                    if (names[1] == 'coupon_type' && this.value == code2) {

                        if (rowid != rowindex) {
                            IsExits2 = true;
                            //return false
                        }
                    }
                });
            }
            if (typeof code2 != 'undefined') {
                if (IsExits && IsExits2) {
                    Isdupplicate = true;
                    return false;
                }
            }
            else {
                if (IsExits) {
                    Isdupplicate = true;
                    return false;
                }
            }
        });

        return Isdupplicate;
    };

    GM.Security.CashFlow.Form.Reset = function () {
        $('#SecurityEventRightModal_round_no').text(null);
        $('#SecurityEventRightModal_round_no').val(null);
        $('#SecurityEventRightModal_event_date').text(null);
        $("#SecurityEventRightModal_event_date").val(null);
        $('#SecurityEventRightModal_payment_date').text(null);
        $("#SecurityEventRightModal_payment_date").val(null);
        $('#SecurityEventRightModal_xi_date').text(null);
        $("#SecurityEventRightModal_xi_date").val(null);
        $('#SecurityEventRightModal_start_date').text(null);
        $("#SecurityEventRightModal_start_date").val(null);
        $('#SecurityEventRightModal_end_date').text(null);
        $("#SecurityEventRightModal_end_date").val(null);
        $('#SecurityEventRightModal_coupon_date').text(null);
        $("#SecurityEventRightModal_coupon_date").val(null);
        $('#SecurityEventRightModal_next_coupon_date').text(null);
        $("#SecurityEventRightModal_next_coupon_date").val(null);
        $('#SecurityEventRightModal_begining_par').text(null);
        $("#SecurityEventRightModal_begining_par").val(null);
        $('#SecurityEventRightModal_ending_par').text(null);
        $("#SecurityEventRightModal_ending_par").val(null);
        $('#SecurityEventRightModal_coupon_rate').text(null);
        $("#SecurityEventRightModal_coupon_rate").val(null);
        $('#SecurityEventRightModal_interest').text(null);
        $("#SecurityEventRightModal_interest").val(null);
        $('#SecurityEventRightModal_principal').text(null);
        $("#SecurityEventRightModal_principal").val(null);
        $('#SecurityEventRightModal_total_payment').text(null);
        $("#SecurityEventRightModal_total_payment").val(null);
        $('#SecurityEventRightModal_event_type').text(null);
        $("#SecurityEventRightModal_event_type").val(null);
        $('#SecurityEventRightModal_coupon_type').text(null);
        $("#SecurityEventRightModal_coupon_type").val(null);
        $('#SecurityEventRightModal_coupon_floating_index_code').text(null);
        $("#SecurityEventRightModal_coupon_floating_index_code").val(null);
        $('#SecurityEventRightModal_coupon_spread').text(null);
        $("#SecurityEventRightModal_coupon_spread").val(null);
        $('#SecurityEventRightModal_ai_amount').text(null);
        $("#SecurityEventRightModal_ai_amount").val(null);
        $('#SecurityEventRightModal_redemption_percent').text(null);
        $("#SecurityEventRightModal_redemption_percent").val(null);
        //$('#SecurityEventRightModal_complete_flag').text(null);
        //$("#SecurityEventRightModal_complete_flag").val(null);

        $("#coupon_rate_text_error").text("");
        $('#SecurityEventRightModal_coupon_rate').removeClass("input-validation-error");

        $("#begining_par_error").text("");
        $('#SecurityEventRightModal_begining_par').removeClass("input-validation-error");

        $("#ending_par_error").text("");
        $('#SecurityEventRightModal_ending_par').removeClass("input-validation-error");


        $("#event_date_error").text("");
        $('#SecurityEventRightModal_event_date').removeClass("input-validation-error");
        $("#payment_date_error").text("");
        $('#SecurityEventRightModal_payment_date').removeClass("input-validation-error");
        $("#start_date_error").text("");
        $('#SecurityEventRightModal_start_date').removeClass("input-validation-error");
        $("#end_date_error").text("");
        $('#SecurityEventRightModal_end_date').removeClass("input-validation-error");

        $("#event_type_text_error").text("");
        $('#SecurityEventRightModal_event_type').removeClass("input-validation-error");
        $("#round_no_text_error").text("");
        $('#SecurityEventRightModal_round_no').removeClass("input-validation-error");

        $("#SecurityEventRightModal_start_date").data("DateTimePicker").minDate(false).maxDate(false);
        $("#SecurityEventRightModal_end_date").data("DateTimePicker").minDate(false).maxDate(false);

        var completeFlagId = $('input:radio[name = "SecurityEventRightModal.complete_flag"]');
        completeFlagId.filter('[value="true"]').prop("checked", true).attr("checked", "checked").attr('ischeck', 'true');
        completeFlagId.filter('[value="false"]').prop("checked", false).removeAttr("checked").attr('ischeck', 'false');

        GM.Message.Clear();
    };

    $("#btnCashFlowSave").on('click', function () {
        GM.Security.CashFlow.Form.Save(this);
    });

    $("#btnCashFlowCreate").on('click', function () {
        GM.Security.CashFlow.Form.Show(this);
    });
    //End Cash Flow Logic

    $('form').on('submit', function (e) {
        GM.Security.Save(this);
    });

    //Start for Edit Page
    GetIssuerRating($('#issuer_id').val());
    GetEditCashFlow();

    //set date Issue Date & Maturity Date
    if ($("#issue_date").val().length > 0) {
        $("#maturity_date").data("DateTimePicker").minDate($("#issue_date").val());
    }

    if ($("#maturity_date").val().length > 0) {
        $("#issue_date").data("DateTimePicker").maxDate($("#maturity_date").val());
    }

    $("#issue_date").on("dp.change", function (e) {
        $("#maturity_date").data("DateTimePicker").minDate(e.date);
    });

    $("#maturity_date").on("dp.change", function (e) {
        $("#issue_date").data("DateTimePicker").maxDate(e.date);
    });

    //set date start end
    $("#SecurityEventRightModal_start_date").on("dp.change", function (e) {
        $("#SecurityEventRightModal_end_date").data("DateTimePicker").minDate(e.date);
    });

    $("#SecurityEventRightModal_end_date").on("dp.change", function (e) {
        $("#SecurityEventRightModal_start_date").data("DateTimePicker").maxDate(e.date);
    });

    //set radio 
    $('input:radio[name = "SecurityEventRightModal.complete_flag"]').change(function (e) {
        var ele = $(this);
        if (ele.val() == "true") {
            $('input:radio[name = "SecurityEventRightModal.complete_flag"]').filter('[value="true"]').prop("checked", true).attr("checked", "checked").attr('ischeck', 'true');
            $('input:radio[name = "SecurityEventRightModal.complete_flag"]').filter('[value="false"]').prop("checked", false).removeAttr("checked").attr('ischeck', 'false');
        }
        else {
            $('input:radio[name = "SecurityEventRightModal.complete_flag"]').filter('[value="false"]').prop("checked", true).attr("checked", "checked").attr('ischeck', 'true');
            $('input:radio[name = "SecurityEventRightModal.complete_flag"]').filter('[value="true"]').prop("checked", false).removeAttr("checked").attr('ischeck', 'false');
        }
    });

    $("#GuarantorsRightModal_guarantor_percent").keyup(function (evt) {
        if (this.value > 100.00) {
            this.value = 100.00;
        }
    });

    $('form').on('reset', function (e) {
        $('.spinner').css('display', 'block'); // Open Loading
        location.href = window.location.href;
    });
    //End for Edit page

    $('#xa_day').on('focusout', function() {
        if (!this.value.length) {
            this.value = 0;
        }
    });

    $('#xi_day').on('focusout', function () {
        if (!this.value.length) {
            this.value = 0;
        }
    });
});