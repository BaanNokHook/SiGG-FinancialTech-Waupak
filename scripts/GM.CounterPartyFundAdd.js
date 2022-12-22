function Setstatus(datatext) {
    $('#statusdata').val(datatext);
    $("#ddlstatuscounterparty").find(".selected-data").text(datatext);

    //if (datatext == "Approve") {
    //    $("#newdata").attr("style", "display: none");
    //    $("#active").attr("style", "display: none");
    //    $("#approve").attr("style", "display: show");
    //    $("#unapprove").attr("style", "display: none");
    //    $("#unactivedata").attr("style", "display: none");

    //}
    //else if (datatext == "UnApprove") {
    //    $("#newdata").attr("style", "display: none");
    //    $("#active").attr("style", "display: none");
    //    $("#approve").attr("style", "display: none");
    //    $("#unapprove").attr("style", "display: show");
    //    $("#unactivedata").attr("style", "display: none");
    //}
    //else if (datatext == "UnActive") {
    //    $("#newdata").attr("style", "display: none");        
    //    $("#active").attr("style", "display: none");
    //    $("#approve").attr("style", "display: none");
    //    $("#unapprove").attr("style", "display: none");
    //    $("#unactivedata").attr("style", "display: show");
    //}
    //else if (datatext == "Active") {
    //    $("#newdata").attr("style", "display: none");
    //    $("#active").attr("style", "display: show");
    //    $("#approve").attr("style", "display: none");
    //    $("#unapprove").attr("style", "display: none");
    //    $("#unactivedata").attr("style", "display: none");
    //}
}

function expandpanel(panelname) {
    if (panelname == "detail") {
        $("#counterpartyfund-detail-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#counterpartyfund-detail-form").attr("aria-expanded", "true");
        $("#counterpartyfund-detail-form").removeAttr("style");
        $("#counterpartyfund-detail-icon").attr("aria-expanded", "true");
        $("#counterpartyfund-detail-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#counterpartyfund-detail-icon").attr("class", "box-header big-head expand-able");
        $("#detail-li").attr("class", "active");
        $("#identify-li").removeAttr("class");
        $("#payment-li").removeAttr("class");
        $("#margin-li").removeAttr("class");
    }
    else if (panelname == "identify") {
        $("#counterpartyfund-identify-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#counterpartyfund-identify-form").attr("aria-expanded", "true");
        $("#counterpartyfund-identify-form").removeAttr("style");
        $("#counterpartyfund-identify-icon").attr("aria-expanded", "true");
        $("#counterpartyfund-identify-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#counterpartyfund-identify-icon").attr("class", "box-header big-head expand-able");
        $("#identify-li").attr("class", "active");
        $("#detail-li").removeAttr("class");
        $("#payment-li").removeAttr("class");
        $("#margin-li").removeAttr("class");
    }
    else if (panelname == "payment") {
        $("#counterpartyfund-payment-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#counterpartyfund-payment-form").attr("aria-expanded", "true");
        $("#counterpartyfund-payment-form").removeAttr("style");
        $("#counterpartyfund-payment-icon").attr("aria-expanded", "true");
        $("#counterpartyfund-payment-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#counterpartyfund-payment-icon").attr("class", "box-header big-head expand-able");
        $("#payment-li").attr("class", "active");
        $("#detail-li").removeAttr("class");
        $("#identify-li").removeAttr("class");
        $("#margin-li").removeAttr("class");
    }
    else if (panelname == "margin") {
        $("#counterpartyfund-margin-form").attr("class", "form-container form-horizontal have-head collapse in");
        $("#counterpartyfund-margin-form").attr("aria-expanded", "true");
        $("#counterpartyfund-margin-form").removeAttr("style");
        $("#counterpartyfund-margin-icon").attr("aria-expanded", "true");
        $("#counterpartyfund-margin-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        $("#counterpartyfund-margin-icon").attr("class", "box-header big-head expand-able");
        $("#margin-li").attr("class", "active");
        $("#detail-li").removeAttr("class");
        $("#identify-li").removeAttr("class");
        $("#payment-li").removeAttr("class");
    }
}

function CallInsumentName(insumentID) {
    var data = {
        insumentID: insumentID
    };
    $.ajax({
        url: "/CounterPartyFund/GetInsumentName",
        type: "GET",
        dataType: "JSON",
        data: data,
        success: function (res) {
            if (res.length > 0) {
                $('#counter_party_name').text(res[0].counter_party_name);
                $('#counter_party_name').value = res[0].counter_party_name;
                $('#counter_party_name').val(res[0].counter_party_name);
            } else {
                $('#counter_party_name').text("");
                $('#counter_party_name').value = "";
                $('#counter_party_name').val("");
            }

        }
    });
    //End Counter Party
}

$(document).ready(function () {

    $('#counterpartyfund-detail').click(function (e) {
        var expand = $("div#counterpartyfund-detail-icon").attr("aria-expanded");
        if (expand == "true") {
            $("#counterpartyfund-detail-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#counterpartyfund-detail-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }

    });

    $('#counterpartyfund-margin').click(function (e) {
        var expand = $("div#counterpartyfund-margin-icon").attr("aria-expanded");
        if (expand == "true") {

            $("#counterpartyfund-margin-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#counterpartyfund-margin-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }

    });

    $('#counterpartyfund-payment').click(function (e) {
        var expand = $("div#counterpartyfund-payment-icon").attr("aria-expanded");
        //alert(expand);
        if (expand == "true") {

            $("#counterpartyfund-payment-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#counterpartyfund-payment-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }

    });

    $('#counterpartyfund-identify').click(function (e) {
        var expand = $("div#counterpartyfund-identify-icon").attr("aria-expanded");
        if (expand == "true") {

            $("#counterpartyfund-identify-icon").find("div.title").find("i").attr("class", "feather-icon icon-plus");
        }
        else {
            $("#counterpartyfund-identify-icon").find("div.title").find("i").attr("class", "feather-icon icon-minus");
        }
    });


    //Title Name
    $("#ddl_title").click(function () {
        var txt_search = $('#txt_title');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_title').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });

    $("#ul_title").on("click", ".searchterm", function (event) {
        if ($("#ddl_title").find(".selected-data").text() == "อื่นๆอื่นๆ") {
            $("#title_name2").removeAttr("readonly");
        } else {
            $("#title_name2").attr("readonly", "readonly");
        }
        $("#title_name2").text(null);
        $("#title_name2").val(null);
    });


    //End Title Name

    //Counter Party
    $("#ddl_counterparty").click(function () {
        var txt_search = $('#txt_counterparty');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_counterparty').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    $("#ul_counterparty").on("click", ".searchterm", function (event) {
        CallInsumentName($("#ddl_counterparty").find(".selected-value").val());
    });

    //custodian
    $("#ddl_custodian").click(function () {
        var txt_search = $('#txt_custodian');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_custodian').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End custodian

    //fund type
    $("#ddl_fund_type").click(function () {
        var txt_search = $('#txt_fund_type');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_fund_type').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End fund type

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

    //account_type_banklist
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
    //End account_type_banklist

    //bank_name_banklist
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
    //End bank_name_banklist

    //payment_method
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

    GM.CounterpartyFund = {};
    GM.CounterpartyFund.Save = function (form) {
    };

    //Start Payment
    GM.CounterpartyFund.Payment = {};
    GM.CounterpartyFund.Payment.Table = $("#tbPayment");
    GM.CounterpartyFund.Payment.Table.RowSelected = {};
    GM.CounterpartyFund.Payment.Table.RowEmpty = function () {
        var row = $("<tr></tr>");
        var col = $('<td class="long-data text-center empty-data" style="height:50px;" colspan="3"> No data.</td>');
        row.append(col);
        GM.CounterpartyFund.Payment.Table.append(row);
    };
    GM.CounterpartyFund.Payment.Table.RowAny = function (code, code2, rowid) {

        var rows = GM.CounterpartyFund.Payment.Table.find('tr');
        var IsExits = false;
        var IsExits2 = false;
        var Isdupplicate = false;
        rows.each(function (index, row) {

            var rowindex = $(row).data("id");
            var inputs = $(row).find('input,select,textarea');
            if (inputs.length > 0) {

                $.each(inputs,
                    function () {

                        var names = this.name.split('.');
                        if (names[1] == 'nosto_vosto_code' && this.value == code) {

                            if (rowid != rowindex) {
                                IsExits = true;
                                //return false
                            }
                        }

                        if (names[1] == 'payment_method' && this.value == code2) {

                            if (rowid != rowindex) {
                                IsExits2 = true;
                                //return false
                            }
                        }

                    });
            }
            //if (typeof code2 != 'undefined') {
            if (IsExits && IsExits2) {
                Isdupplicate = true;
                return false;
            }
            //}
            //else {
            //    if (IsExits) {
            //        Isdupplicate = true;
            //        return false;
            //    }
            //}
        });

        return Isdupplicate;
    };
    GM.CounterpartyFund.Payment.Form = $("#modal-form-payment");
    GM.CounterpartyFund.Payment.Form.ButtonSave = $('#btnPaymentSave');
    GM.CounterpartyFund.Payment.Form.Show = function (btn) {

        var action = $(btn).data("action");
        var button = $('#btnPaymentSave');

        GM.CounterpartyFund.Payment.Form.Reset();

        switch (action) {
            case 'create':
                GM.CounterpartyFund.Payment.Form.find(".modal-title").html("Create");
                GM.CounterpartyFund.Payment.Form.ButtonSave.data("action", "create");
                GM.CounterpartyFund.Payment.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('+ Add Bank List');

                $('#ddl_account_type_banklist').removeAttr('disabled');
                $('#ddl_bank_name_banklist').removeAttr('disabled');
                $('#ddl_payment_method_banklist').removeAttr('disabled');

                $('#PaymentRightModal_account_number').removeAttr('disabled');
                $('#PaymentRightModal_ca_no').removeAttr('disabled');
                $('#PaymentRightModal_sa_no').removeAttr('disabled');
                $('#PaymentRightModal_participant_id').removeAttr('disabled');
                break;

            case 'update':

                GM.CounterpartyFund.Payment.Table.RowSelected = $(btn).parent().parent();
                GM.CounterpartyFund.Payment.Form.find(".modal-title").html("Update");
                GM.CounterpartyFund.Payment.Form.Initial();

                $("#ddl_account_type_banklist").attr('disabled', 'disabled');
                $("#ddl_payment_method_banklist").attr('disabled', 'disabled');


                $("#ddl_bank_name_banklist").removeAttr('disabled');
                $('#PaymentRightModal_account_number').removeAttr('disabled');

                $('#PaymentRightModal_ca_no').removeAttr('disabled');
                $('#PaymentRightModal_sa_no').removeAttr('disabled');
                $('#PaymentRightModal_participant_id').removeAttr('disabled');

                GM.CounterpartyFund.Payment.Form.ButtonSave.data("action", "update");
                GM.CounterpartyFund.Payment.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('Save Bank List');
                break;

            case 'delete':

                GM.CounterpartyFund.Payment.Table.RowSelected = $(btn).parent().parent();
                GM.CounterpartyFund.Payment.Form.find(".modal-title").html("Delete");
                GM.CounterpartyFund.Payment.Form.Initial();

                $("#ddl_account_type_banklist").attr('disabled', 'disabled');
                $("#ddl_payment_method_banklist").attr('disabled', 'disabled');
                $("#ddl_bank_name_banklist").attr('disabled', 'disabled');

                $('#PaymentRightModal_account_number').attr('disabled', 'disabled');
                $('#PaymentRightModal_ca_no').attr('disabled', 'disabled');
                $('#PaymentRightModal_sa_no').attr('disabled', 'disabled');
                $('#PaymentRightModal_participant_id').attr('disabled', 'disabled');

                GM.CounterpartyFund.Payment.Form.ButtonSave.data("action", "delete");
                GM.CounterpartyFund.Payment.Form.ButtonSave.removeClass('btn-primary').addClass('btn-delete').text('Confirm Delete');
                break;

            default:
                break;
        }

        GM.CounterpartyFund.Payment.Form.modal('toggle');
    };
    GM.CounterpartyFund.Payment.Form.Valid = function () {

        var PaymentRightModal_nosto_vosto_code = $('#PaymentRightModal_nosto_vosto_code');
        var PaymentRightModal_payment_method = $('#PaymentRightModal_payment_method');
        //var PaymentRightModal_bank_code = $('#PaymentRightModal_bank_code');
        //var PaymentRightModal_account_number = $('#PaymentRightModal_account_number');

        if (PaymentRightModal_nosto_vosto_code.val().trim() == "") {
            return false;
        }
        if (PaymentRightModal_payment_method.val().trim() == "") {
            return false;
        }
        //if (PaymentRightModal_bank_code.val().trim() == "") {
        //    return false;
        //}
        //if (PaymentRightModal_account_number.val().trim() == "") {
        //    return false;
        //}
        return true;

    };
    GM.CounterpartyFund.Payment.Form.Save = function (btn) {

        var action = $(btn).data("action");

        GM.Message.Clear();
        var IsInValid = GM.CounterpartyFund.Payment.Form.Valid();

        if (IsInValid) {
            switch (action) {
                case 'create':
                    GM.CounterpartyFund.Payment.Table.attr("style", "display: show");
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
                    var rowindex = GM.CounterpartyFund.Payment.Table.find("tr:last").data("id");

                    if (!GM.CounterpartyFund.Payment.Table.RowAny(nosto_vosto_code_val, payment_method_val)) {
                        if (rowindex != undefined) {
                            rowindex = parseInt(rowindex) + 1;
                        } else {
                            rowindex = 0;
                        }
                        var row = $('<tr data-id="' + rowindex + '" ></tr>');
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
                            nosto_vosto_code_text +
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
                            bank_name_text +
                            '">' +
                            '</td>');

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
                            payment_method_text +
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

                        var ColRowStatus = $('<td><input name="Payment[' +
                            rowindex +
                            '].rowstatus" Isdropdown="false"  type="hidden" value="create"></td>');

                        var ColAction = $('<td class="action">' +
                            '<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                            nosto_vosto_code_text +
                            '" data-action="update" onclick="GM.CounterpartyFund.Payment.Form.Show(this)" >' +
                            '<i class="feather-icon icon-edit"></i>' +
                            '</button > ' +
                            '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                            nosto_vosto_code_text +
                            '" data-action="delete" onclick="GM.CounterpartyFund.Payment.Form.Show(this)" >' +
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
                        row.append(ColRowStatus);
                        row.append(ColAction);

                        GM.CounterpartyFund.Payment.Table.append(row);
                        GM.CounterpartyFund.Payment.Table.find(".empty-data").parent().remove();
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
                    var rowindex = GM.CounterpartyFund.Payment.Table.find("tr:last").data("id");

                    var row = GM.CounterpartyFund.Payment.Table.RowSelected;
                    var rowindex = row.data("id");
                    var rowfrom = row.data("action");
                    var textstatus = "create";
                    if (rowfrom == "fromdatabase") {
                        textstatus = "update";
                    }


                    if (!GM.CounterpartyFund.Payment.Table.RowAny(nosto_vosto_code_val, payment_method_val, rowindex)) {
                        // if (1 == 1) {
                        var ColAccountType = row.children("td:nth-child(1)");
                        var ColBankName = row.children("td:nth-child(2)");
                        var ColPaymentMathod = row.children("td:nth-child(3)");
                        var ColAccountNo = row.children("td:nth-child(4)");
                        var ColCaNo = row.children("td:nth-child(5)");
                        var ColSaNo = row.children("td:nth-child(6)");
                        var ColParticipantID = row.children("td:nth-child(7)");
                        var ColRowStatus = row.children("td:nth-child(8)");
                        var ColAction = row.children("td:nth-child(9)");

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
                            bank_name_text +
                            '">');

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
                            payment_method_text +
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

                        ColRowStatus.html('<input name="Payment[' +
                            rowindex +
                            '].rowstatus" Isdropdown="false"  type="hidden" value="' +
                            textstatus +
                            '">');

                        ColAction.html('<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                            nosto_vosto_code_text +
                            '" data-action="update" onclick="GM.CounterpartyFund.Payment.Form.Show(this)" >' +
                            '<i class="feather-icon icon-edit"></i>' +
                            '</button > ' +
                            '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                            nosto_vosto_code_text +
                            '" data-action="delete" onclick="GM.CounterpartyFund.Payment.Form.Show(this)" >' +
                            '<i class="feather-icon icon-trash-2"></i>' +
                            '</button>');
                    } else {
                        GM.Message.Error('.modal-body', "The Data already exists.");
                        return;
                    }
                    break;

                case 'delete':
                    var rowselect = GM.CounterpartyFund.Payment.Table.RowSelected;
                    var rowfrom = rowselect.data("action");
                    var rowid = rowselect.data("id");
                    if (rowfrom != "fromdatabase") {
                        GM.CounterpartyFund.Payment.Table.RowSelected.remove();
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
                    var rows = GM.CounterpartyFund.Payment.Table.find('tr');
                    var rowindex = 0;
                    var checkrow = 0;

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
                        GM.CounterpartyFund.Payment.Table.append(row);
                        //GM.CounterpartyFund.Payment.Table.attr("style", "display: none");
                    }

                    break;

            }

            GM.CounterpartyFund.Payment.Table.RowSelected = {};

            GM.CounterpartyFund.Payment.Form.modal('toggle');
        }
    };
    GM.CounterpartyFund.Payment.Form.Initial = function () {
        if (GM.CounterpartyFund.Payment.Table.RowSelected) {
            var inputs = $(GM.CounterpartyFund.Payment.Table.RowSelected).find('input,select,textarea');

            if (inputs.length > 0) {
                $.each(inputs, function () {
                    var names = this.name.split('.');
                    if (this.attributes.isdropdown.value == 'true') {
                        var filedtextid = this.attributes.textfield.value.split('.');
                        //$("#PaymentForm span[name='" + filedtextid[1] + "']").text(this.attributes.textvalue.value);
                        //$("#PaymentForm input[name='" + this.attributes.textfield.value + "']").val(this.attributes.textvalue.value);
                        //$("#PaymentForm input[name='" + this.attributes.textfield.value + "']").text(this.attributes.textvalue.value);
                        //$("#PaymentForm input[name='" + this.attributes.valuefield.value + "']").val(this.value);

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
    GM.CounterpartyFund.Payment.Form.Reset = function () {
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
        GM.CounterpartyFund.Payment.Form.Show(this);
    });

    $('#btnPaymentSave').on('click', function () {
        GM.CounterpartyFund.Payment.Form.Save(this);
    });
    //End Payment

    //Start Identify
    GM.CounterpartyFund.Identify = {};
    GM.CounterpartyFund.Identify.Table = $("#tbIdentify");
    GM.CounterpartyFund.Identify.Table.RowSelected = {};
    GM.CounterpartyFund.Identify.Table.RowEmpty = function () {
        var row = $("<tr></tr>");
        var col = $('<td class="long-data text-center empty-data" style="height:50px;" colspan="3"> No data.</td>');
        row.append(col);
        GM.CounterpartyFund.Identify.Table.append(row);
    };
    GM.CounterpartyFund.Identify.Table.RowAny = function (code, code2, rowid) {

        var rows = GM.CounterpartyFund.Identify.Table.find('tr');
        var IsExits = false;
        rows.each(function (index, row) {

            var rowindex = $(row).data("id");
            var inputs = $(row).find('input,select,textarea');

            if (inputs.length > 0) {

                $.each(inputs,
                    function () {

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
    GM.CounterpartyFund.Identify.Form = $("#modal-form-identify");
    GM.CounterpartyFund.Identify.Form.ButtonSave = $('#btnIdentifySave');
    GM.CounterpartyFund.Identify.Form.Show = function (btn) {

        var action = $(btn).data("action");
        var button = $('#btnIdentifySave');

        GM.CounterpartyFund.Identify.Form.Reset();

        switch (action) {
            case 'create':
                GM.CounterpartyFund.Identify.Form.find(".modal-title").html("Create");
                GM.CounterpartyFund.Identify.Form.ButtonSave.data("action", "create");
                GM.CounterpartyFund.Identify.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('+ Add Identify');

                $("#ddl_unique").removeAttr('disabled');
                $("#ddl_identify_type").removeAttr('disabled');

                $('#IdentifyRightModal_identify_no').removeAttr('disabled');
                $('#IdentifyRightModal_juris_reg_date').removeAttr('disabled');
                $('#IdentifyRightModal_reg_bus_ename').removeAttr('disabled');
                $('#IdentifyRightModal_reg_bus_tname').removeAttr('disabled');

                break;

            case 'update':

                GM.CounterpartyFund.Identify.Table.RowSelected = $(btn).parent().parent();
                GM.CounterpartyFund.Identify.Form.find(".modal-title").html("Update");
                GM.CounterpartyFund.Identify.Form.Initial();


                $("#ddl_unique").attr('disabled', 'disabled');
                $("#ddl_identify_type").attr('disabled', 'disabled');

                $('#IdentifyRightModal_identify_no').attr('disabled', 'disabled');

                $('#IdentifyRightModal_juris_reg_date').removeAttr('disabled');
                $('#IdentifyRightModal_reg_bus_ename').removeAttr('disabled');
                $('#IdentifyRightModal_reg_bus_tname').removeAttr('disabled');

                GM.CounterpartyFund.Identify.Form.ButtonSave.data("action", "update");
                GM.CounterpartyFund.Identify.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('Save Identify');
                break;

            case 'delete':

                GM.CounterpartyFund.Identify.Table.RowSelected = $(btn).parent().parent();
                GM.CounterpartyFund.Identify.Form.find(".modal-title").html("Delete");
                GM.CounterpartyFund.Identify.Form.Initial();

                $("#ddl_unique").attr('disabled', 'disabled');
                $("#ddl_identify_type").attr('disabled', 'disabled');

                $('#IdentifyRightModal_identify_no').attr('disabled', 'disabled');
                $('#IdentifyRightModal_juris_reg_date').attr('disabled', 'disabled');
                $('#IdentifyRightModal_reg_bus_ename').attr('disabled', 'disabled');
                $('#IdentifyRightModal_reg_bus_tname').attr('disabled', 'disabled');

                GM.CounterpartyFund.Identify.Form.ButtonSave.data("action", "delete");
                GM.CounterpartyFund.Identify.Form.ButtonSave.removeClass('btn-primary').addClass('btn-delete').text('Confirm Delete');
                break;

            default:
                break;
        }

        GM.CounterpartyFund.Identify.Form.modal('toggle');
    };
    GM.CounterpartyFund.Identify.Form.Valid = function () {

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
    GM.CounterpartyFund.Identify.Form.Save = function (btn) {

        var action = $(btn).data("action");
        GM.Message.Clear();
        var isValid = GM.CounterpartyFund.Identify.Form.Valid();
        if (isValid) {

            switch (action) {
                case 'create':
                    GM.CounterpartyFund.Identify.Table.attr("style", "display: show");
                    var unique_text = $('#IdentifyRightModal_unique_name').text();
                    var unique_val = $("#IdentifyRightModal_unique_id").val();
                    var identify_type_text = $('#IdentifyRightModal_identify_type_text').text();
                    var identify_type_val = $("#IdentifyRightModal_identify_type").val();
                    var identify_no = $('#IdentifyRightModal_identify_no').val();
                    var juris_reg_date = $("#IdentifyRightModal_juris_reg_date").val();
                    var reg_bus_ename = $('#IdentifyRightModal_reg_bus_ename').val();
                    var reg_bus_tname = $('#IdentifyRightModal_reg_bus_tname').val();
                    var rowindex = GM.CounterpartyFund.Identify.Table.find("tr:last").data("id");

                    if (!GM.CounterpartyFund.Identify.Table.RowAny(unique_val)) {
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
                        var ColRowStatus = $('<td><input name="Identify[' +
                            rowindex +
                            '].rowstatus" Isdropdown="false"  type="hidden" value="create"></td>');

                        var ColAction = $('<td class="action">' +
                            '<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                            unique_text +
                            '" data-action="update" onclick="GM.CounterpartyFund.Identify.Form.Show(this)" >' +
                            '<i class="feather-icon icon-edit"></i>' +
                            '</button > ' +
                            '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                            unique_text +
                            '" data-action="delete" onclick="GM.CounterpartyFund.Identify.Form.Show(this)" >' +
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

                        GM.CounterpartyFund.Identify.Table.append(row);
                        GM.CounterpartyFund.Identify.Table.find(".empty-data").parent().remove();
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
                    var rowindex = GM.CounterpartyFund.Identify.Table.find("tr:last").data("id");

                    var row = GM.CounterpartyFund.Identify.Table.RowSelected;
                    var rowindex = row.data("id");

                    GM.CounterpartyFund.Identify.Table.attr("style", "display: show");

                    var row = GM.CounterpartyFund.Identify.Table.RowSelected;
                    var rowindex = row.data("id");
                    var rowfrom = row.data("action");
                    var textstatus = "create";
                    if (rowfrom == "fromdatabase") {
                        textstatus = "update";
                    }

                    if (!GM.CounterpartyFund.Identify.Table.RowAny(unique_val, null, rowindex)) {
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
                            '" data-action="update" onclick="GM.CounterpartyFund.Identify.Form.Show(this)" >' +
                            '<i class="feather-icon icon-edit"></i>' +
                            '</button > ' +
                            '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                            unique_text +
                            '" data-action="delete" onclick="GM.CounterpartyFund.Identify.Form.Show(this)" >' +
                            '<i class="feather-icon icon-trash-2"></i>' +
                            '</button>');
                    } else {
                        GM.Message.Error('.modal-body', "The Data already exists.");
                        return;
                    }

                    break;

                case 'delete':

                    var rowselect = GM.CounterpartyFund.Identify.Table.RowSelected;
                    var rowfrom = rowselect.data("action");
                    var rowid = rowselect.data("id");
                    if (rowfrom != "fromdatabase") {
                        GM.CounterpartyFund.Identify.Table.RowSelected.remove();
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
                    var rows = GM.CounterpartyFund.Identify.Table.find('tr');
                    var rowindex = 0;
                    var checkrow = 0;
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
                        GM.CounterpartyFund.Identify.Table.append(row);
                        // GM.CounterpartyFund.Identify.Table.attr("style", "display: none");
                    }

                    break;
            }

            GM.CounterpartyFund.Identify.Table.RowSelected = {};

            GM.CounterpartyFund.Identify.Form.modal('toggle');
        }
    };
    GM.CounterpartyFund.Identify.Form.Initial = function () {
        if (GM.CounterpartyFund.Identify.Table.RowSelected) {
            var inputs = $(GM.CounterpartyFund.Identify.Table.RowSelected).find('input,select,textarea');

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
    GM.CounterpartyFund.Identify.Form.Reset = function () {

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
        GM.CounterpartyFund.Identify.Form.Show(this);
    });

    $('#btnIdentifySave').on('click', function () {
        GM.CounterpartyFund.Identify.Form.Save(this);
    });
    //End Identify


    $('form').on('submit', function (e) {
    });

    $('form').on('reset', function (e) {
        $('.spinner').css('display', 'block'); // Open Loading
        location.href = window.location.href;
    });
});