GM.System.Sidebar.Handler = function () {
    $('#security-guarantor-table').DataTable().columns.adjust();
    $('#security-rating-table').DataTable().columns.adjust();
    $('#security-event-table').DataTable().columns.adjust();
    $('#security-addtion-table').DataTable().columns.adjust();
}

//Security
GM.Security = {};
GM.Security.Confirm = function (header, message) {
    var Form = $('#confirm-delete');


    Form.find(".modal-header").html(header);
    if (message) {
        Form.find(".modal-text").html(message);
    }
    else { Form.find(".modal-text").html(""); }
    Form.modal('toggle');
};


GM.Security.Save = function (form) {
    //GM.Security.Guarantor.AddToForm(form);
    //GM.Security.Rating.AddToForm(form);
    //GM.Security.Addtion.AddToForm(form)
};

//Guarantor
GM.Security.Guarantor = {};
GM.Security.Guarantor.Table = $("#tbGuarantor");
GM.Security.Guarantor.Table.RowSelected = {};
GM.Security.Guarantor.Table.RowEmpty = function () {
    var row = $("<tr></tr>");
    var col = $('<td class="long-data text-center empty-data" style="height:50px;" colspan="3"> No data.</td>');
    row.append(col);
    GM.Security.Guarantor.Table.append(row);
}
GM.Security.Guarantor.Table.RowAny = function(code, rowid) {

    var rows = GM.Security.Guarantor.Table.find('tr');
    var IsExits = false;

    rows.each(function(index, row) {

        var rowindex = $(row).data("id");
        var inputs = $(row).find('input,select,textarea');

        if (inputs.length > 0) {

            $.each(inputs,
                function() {

                    var names = this.name.split('.');

                    if (names[1] == 'guarantor_code' && this.value == code) {

                        if (rowid != rowindex) {
                            IsExits = true;
                            return false;
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
            GM.Security.Guarantor.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('+ Add Guarantor');
            break;

        case 'update':

            GM.Security.Guarantor.Table.RowSelected = $(btn).parent().parent();
            GM.Security.Guarantor.Form.find(".modal-title").html("Update");
            GM.Security.Guarantor.Form.Initial();

            GM.Security.Guarantor.Form.ButtonSave.data("action", "update");
            GM.Security.Guarantor.Form.ButtonSave.removeClass('btn-delete').addClass('btn-primary').text('Save Guarantor');
            break;

        case 'delete':

            GM.Security.Guarantor.Table.RowSelected = $(btn).parent().parent();
            GM.Security.Guarantor.Form.find(".modal-title").html("Delete");
            GM.Security.Guarantor.Form.Initial();

            GM.Security.Guarantor.Form.ButtonSave.data("action", "delete");
            GM.Security.Guarantor.Form.ButtonSave.removeClass('btn-primary').addClass('btn-delete').text('Confirm Delete');
            break;

        default:
            break;
    }

    GM.Security.Guarantor.Form.modal('toggle');
};
GM.Security.Guarantor.Form.Valid = function () {

    var IsValid = true;

    var guarantor_code = $('#guarantor_code').val();
    var guarantor_percent = $('#guarantor_percent').val();

    if (guarantor_code == "") {
        $('#guarantor_code').css('border-color', "red");
        IsValid = false;
    }

    if (guarantor_percent == "") {
        $('#guarantor_percent').css('border-color', "red");
        IsValid = false;
    }
    else if (!$.isNumeric(guarantor_percent)) {
        $('#guarantor_percent').css('border-color', "red");
        IsValid = false;
    }

    return IsValid;

}
GM.Security.Guarantor.Form.Save = function(btn) {

    var action = $(btn).data("action");


    if (!GM.Security.Guarantor.Form.Valid()) {
        return false;
    }

    switch (action) {
    case 'create':

        var guarantor_code = $('#guarantor_code').val();
        var guarantor_name = $("#guarantor_code option:selected").text();
        var guarantor_percent = $('#guarantor_percent').val();
        var rowindex = GM.Security.Guarantor.Table.find("tr:last").data("id");


        if (!GM.Security.Guarantor.Table.RowAny(guarantor_code)) {

            if (rowindex != undefined) {
                rowindex = parseInt(rowindex) + 1;
            } else {
                rowindex = 0;
            }

            var row = $('<tr data-id="' + rowindex + '" ></tr>');
            var ColGuarantor = $('<td class="long-data">' +
                guarantor_name +
                '<input name="Guarantors[' +
                rowindex +
                '].guarantor_code" type="hidden" value="' +
                guarantor_code +
                '"><input name="Guarantors[' +
                rowindex +
                '].guarantor_name" type="hidden" value="' +
                guarantor_name +
                '"></td>');
            var ColPercentage = $('<td>' +
                guarantor_percent +
                '<input name="Guarantors[' +
                rowindex +
                '].guarantor_percent" type="hidden" value="' +
                guarantor_percent +
                '"></td>');
            var ColAction = $('<td class="action">' +
                '<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                guarantor_code +
                '" data-action="update" onclick="GM.Security.Guarantor.Form.Show(this)" >' +
                '<i class="feather-icon icon-edit"></i>' +
                '</button > ' +
                '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                guarantor_code +
                '" data-action="delete" onclick="GM.Security.Guarantor.Form.Show(this)" >' +
                '<i class="feather-icon icon-trash-2"></i>' +
                '</button>' +
                '</td>');

            row.append(ColGuarantor);
            row.append(ColPercentage);
            row.append(ColAction);

            GM.Security.Guarantor.Table.append(row);
            GM.Security.Guarantor.Table.find(".empty-data").parent().remove();

            swal("Good job!", "Successfully saved", "success");
        } else {
            swal("Error", "The data guarantor already exists.", "error");
        }

        break;

    case 'update':

        var guarantor_code = $('#guarantor_code').val();
        var guarantor_name = $("#guarantor_code option:selected").text();
        var guarantor_percent = $('#guarantor_percent').val();
        var row = GM.Security.Guarantor.Table.RowSelected;
        var rowindex = row.data("id");

        if (!GM.Security.Guarantor.Table.RowAny(guarantor_code, rowindex)) {

            var colGuarantor = row.children("td:nth-child(1)");
            var colPercentage = row.children("td:nth-child(2)");
            var colAction = row.children("td:nth-child(3)");

            colGuarantor.html(guarantor_name +
                '<input name= "Guarantors[' +
                rowindex +
                '].guarantor_code" type= "hidden" value= "' +
                guarantor_code +
                '" ><input name= "Guarantors[' +
                rowindex +
                '].guarantor_name" type= "hidden" value= "' +
                guarantor_name +
                '" >');
            colPercentage.html(guarantor_percent +
                '<input name="Guarantors[' +
                rowindex +
                '].guarantor_percent" type="hidden" value="' +
                guarantor_percent +
                '">');
            colAction.html('<button class="btn btn-default btn-round icon-only" type="button" data-id="' +
                guarantor_code +
                '" data-action="update" onclick="GM.Security.Guarantor.Form.Show(this)" >' +
                '<i class="feather-icon icon-edit"></i>' +
                '</button > ' +
                '<button class="btn btn-delete btn-round icon-only" type="button" data-id="' +
                guarantor_code +
                '" data-action="delete" onclick="GM.Security.Guarantor.Form.Show(this)" >' +
                '<i class="feather-icon icon-trash-2"></i>' +
                '</button>');

            swal("Good job!", "Successfully saved", "success");
        } else {
            swal("Error", "The data guarantor already exists.", "error");
        }
        break;

    case 'delete':

        GM.Security.Guarantor.Table.RowSelected.remove();

        //renew index of input
        var rows = GM.Security.Guarantor.Table.find('tr');
        var rowindex = 0;

        rows.each(function(index, row) {

            var inputs = $(row).find('input,select,textarea');

            if (inputs.length > 0) {

                $.each(inputs,
                    function() {

                        var names = this.name.split('.');
                        var inputname = 'Guarantors[' + rowindex + '].' + names[1];

                        //update new input name
                        $(this).attr('name', inputname);

                    });

                rowindex++;
            }

        });

        if (rows.length === 1) {
            GM.Security.Guarantor.Table.RowEmpty(table);
        }

        break;

    }

    GM.Security.Guarantor.Table.RowSelected = {};

    GM.Security.Guarantor.Form.modal('toggle');
};
GM.Security.Guarantor.Form.Initial = function () {
    if (GM.Security.Guarantor.Table.RowSelected) {


        var inputs = $(GM.Security.Guarantor.Table.RowSelected).find('input,select,textarea');

        if (inputs.length > 0) {
            $.each(inputs, function () {
                var names = this.name.split('.');
                console.log('$(\'#' + names[1] + '\').val(' + this.value + ');');
                $('#' + names[1]).val(this.value);
            });
        }

    }
};
GM.Security.Guarantor.Form.Reset = function () {
    $('#guarantor_code').val("");
    $('#guarantor_percent').val("");

    $('#guarantor_code').css('border-color', "");
    $('#guarantor_percent').css('border-color', "");
};

//Rating
GM.Security.Rating = {};
GM.Security.Rating.Table = $("#table-rating");
GM.Security.Rating.Table.RowSelected = {};
GM.Security.Rating.Table.RowEmpty = function() {
    var row = $("<tr></tr>");
    var col = $('<td class="long-data text-center empty-data" style="height:50px;" colspan="6"> No data.</td>');
    row.append(col);
    GM.Security.Guarantor.Table.append(row);
};
GM.Security.Rating.Table.RowAny = function (code, rowid) {

    var rows = GM.Security.Rating.Table.find('tr');
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
                        return false;
                    }
                }

            });
        }

        if (IsExits) {
            return false;
        }

    });

    return IsExits;
}
GM.Security.Rating.Form = $("#modal-form-rating");
GM.Security.Rating.Form.ButtonSave = $('#btnRatingSave');
GM.Security.Rating.Form.Show = function (btn) {
    var action = $(btn).data("action");
    var button = $('#btnSaveRating');

    switch (action) {
        case 'create':
            GM.Security.Rating.Form.find(".modal-title").html("Create Bond Rating");
            button.data("action", "create");
            button.removeClass('btn-delete').addClass('btn-primary').text('+ Add Bond Rating');
            break;

        case 'update':
            GM.Security.Rating.Form.find(".modal-title").html("Update Bond Rating");
            button.data("action", "update");
            button.removeClass('btn-delete').addClass('btn-primary').text('Save Bond Rating');
            break;

        case 'delete':
            GM.Security.Rating.Form.find(".modal-title").html("Delete Bond Rating");
            button.data("action", "delete");
            button.removeClass('btn-primary').addClass('btn-delete').text('Confirm Delete');
            break;
    }

    GM.Security.Rating.Form.modal('toggle');
};
GM.Security.Rating.Form.Valid = function() {};
GM.Security.Rating.Form.Save = function (btn) {
    var action = $(btn).data("action");
    var button = $('#btnSaveRating');

    switch (action) {
        case 'create': break;
        case 'update': break;
        case 'delete': break;
    }

    GM.Security.Rating.Form.modal('toggle');
}
GM.Security.Rating.Form.Initial = function () { }
GM.Security.Rating.Form.Reset = function () {
    $('#guarantor_code').val("");
    $('#guarantor_percent').val("");

    $('#guarantor_code').css('border-color', "");
    $('#guarantor_percent').css('border-color', "");
};


GM.Security.Event = {};
GM.Security.Event.Table = $('#security-event-table').DataTable({
    paging: true,
    scrollX: true,
    columnDefs: [

        { data: 'round_no', targets: 0 },
        {
            data: 'event_date', targets: 1,
            render: function (data, type, row, meta) {
                if (data != null) {
                    return moment(data).format('DD/MM/YYYY');
                }
                return data;
            }
        },
        {
            data: 'payment_date', targets: 2,
            render: function (data, type, row, meta) {
                if (data != null) {
                    return moment(data).format('DD/MM/YYYY');
                }
                return data;
            }
        },
        {
            data: 'xi_date', targets: 3,
            render: function (data, type, row, meta) {
                if (data != null) {
                    return moment(data).format('DD/MM/YYYY');
                }
                return data;
            }
        },
        {
            data: 'coupon_date', targets: 4,
            render: function (data, type, row, meta) {
                if (data != null) {
                    return moment(data).format('DD/MM/YYYY');
                }
                return data;
            }
        },
        {
            data: 'next_coupon_date', targets: 5,
            render: function (data, type, row, meta) {
                if (data != null) {
                    return moment(data).format('DD/MM/YYYY');
                }
                return data;
            }
        },
        { data: 'begining_par', targets: 6 },
        { data: 'ending_par', targets: 7 },
        { data: 'coupon_rate', targets: 8 },
        { data: 'interest', targets: 9 },
        { data: 'principal', targets: 10 },
        { data: 'total_payment', targets: 11 },
        { data: 'event_type', targets: 12 },
        { data: 'coupon_type', targets: 13 },
        { data: 'coupon_floating_index_code', targets: 14 },
        { data: 'coupon_spread', targets: 15 },
        { data: 'redemption_percent', targets: 16 },
        { data: 'complete_flag', targets: 17 },
        { data: 'complete_flag', targets: 18 }
    ]
});
GM.Security.Event.Generate = function () {
    var data = {};
    //not pass parameter to store
    data.check_merge_flag = $("input[name='check_merge_flag']:checked").val();

    //parameter 
    data.instrument_id_temp = $('#instrument_id_temp').val();
    data.include_issue_date_flag = $("input[name='include_issue_date_flag']:checked").val();
    data.issue_date = $('#issue_date').val();
    data.maturity_date = $('#maturity_date').val();
    data.coupon_date_rule = $('#coupon_date_rule').val();
    data.coupon_payment_date_rule = $('#coupon_payment_date_rule').val();
    data.coupon_day = $('#coupon_day').val();
    data.coupon_month = $('#coupon_month').val();
    data.coupon_freq = $('#coupon_freq').val();
    data.coupon_freq_time = $('#coupon_freq_time').val();
    data.coupon_type = $('#coupon_type').val();
    data.coupon_rate = $('#coupon_rate').val();
    data.floating_index_code = $('#floating_index_code').val();
    data.cur = $('#cur').val();
    data.spread = $('#spread').val();
    data.redemption_percent = $('#redemption_percent').val();
    data.payment_freq = "";
    data.check_mat_coupon_flag = $("input[name='check_mat_coupon_flag']:checked").val();
    data.int_cf_flag = $("input[name='int_cf_flag']:checked").val();
    data.xi_day = $('#xi_day').val();
    data.xi_day_unit = $('#xi_day_unit').val();
    data.begining_par = $('#begining_par').val();
    data.ending_par = $('#ending_par').val();
    data.year_basis = $('#year_basis').val();
    data.create_date = "";
    data.create_by = "";
    data.update_date = "";
    data.update_by = "";

    GM.Security.Event.Table.clear();
    var action = $('#security-event-table').attr('data-action');
    $.ajax({
        url: action,
        type: "get", //send it through get method
        data: data,
        success: function (Response) {
            //Do Something

            console.log("event:", Response);

            if (Response.Success) {
                var rows = Response.Data.Events;

                if (rows.length > 0) {
                    GM.Security.Event.Table.row.add(rows[0]).draw(true);
                }
                else {
                    GM.Security.Event.Table.draw();
                }
            }

        },
        error: function (xhr, status, error) {
            //Do Something to handle error
            if (xhr.status === 401) {
                //window.location.href = xhr.Data.LogOnUrl;
                return;
            }
        }
    });
};

GM.Security.Addtion = {};
GM.Security.Addtion.Table = $('#security-addtion-table').DataTable({
    paging: true,
    scrollX: true,
    columnDefs: [
        {
            targets: 0,
            data: 'new_instrument_code',
            render: function (data, type, row, meta) {
                return '<input class="form-control"  name="AdditionalCodes[' + meta.row + '].new_instrument_code" type="text"  value = ' + data + '  >';
            }
        },
        {
            targets: 1,
            data: 'update_date',
            render: function (data, type, row, meta) {
                return '<input class="form-control"  name="AdditionalCodes[' + meta.row + '].update_date" type="text"  value = ' + data + '  >';
            }
        },
        {
            targets: 2,
            render: function (data, type, row, meta) {
                return "";
            }
        }
    ]
});
GM.Security.Addtion.NewRow = function (row) {
    GM.Security.Addtion.Table.row.add(row).draw(true);
};
GM.Security.Addtion.AddToForm = function (form) {

    var params = GM.Security.Rating.Table.$('input,select,textarea').serializeArray();

    // Iterate over all form elements
    $.each(params, function () {
        // If element doesn't exist in DOM
        console.log(this.name);

        if (!$.contains(document, form[this.name])) {
            // Create a hidden element
            $(form).append(
                $('<input>')
                    .attr('type', 'hidden')
                    .attr('name', this.name)
                    .val(this.value)
            );
        }
    });
};


//Event Handler
//Handle form submission event
$('#btnGuarantorCreate').on('click', function () {
    GM.Security.Guarantor.Form.Show(this);
});

$('#btnGuarantorSave').on('click', function () {
    GM.Security.Guarantor.Form.Save(this);
});

$('#btnRatingCreate').on('click', function () {
    GM.Security.Rating.Form.Show(this);
});

$('#btnRatingSave').on('click', function () {
    GM.Security.Rating.Form.Save(this);
});

$('form').on('submit', function (e) {
    GM.Security.Save(this);
});

$('#btnGenearteEvent').on('click', function () {
    GM.Security.Event.Generate();
});

//for mockup template
$('#btnAddGuarantor').on('click', function () {
    GM.Security.Guarantor.NewRow({ Guarantor: new Date().getTime(), Percent: 0.0 });
});

$('#btnAddRating').on('click', function () {
    GM.Security.Rating.NewRow({
        agency_code: "",
        short_long_term: "",
        local_rating: "",
        foreign_rating: "",
        assess_date: ""
    });
});

$('#btnAddAdditionCode').on('click', function () {
    GM.Security.Addtion.NewRow({ new_instrument_code: "", update_date: new Date(), action: "" });
});