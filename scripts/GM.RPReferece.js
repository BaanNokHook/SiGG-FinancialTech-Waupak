$(document).ready(function () {

    function ExportExcel() {
        $('#FormDownload_instrument_code').val($('#FormSearch_instrument_code').val());
        $('#FormDownload_asof_date').val($('#FormSearch_asof_date').val());
        $('#download-form').submit();
    }

    function focusInput(input) {
        var center = $(window).height() / 2;
        var top = $(input).offset().top;
        if (top > center) {
            $(window).scrollTop(top - center);
        }
        input.addClass("input-validation-error");
        //input.focus();
    }

    function setFormTBMA() {
        $('#panel-BBG').attr('style', 'display:none');

        //set labal require
        $("label[for='ai']").append('<span class="required"> *</span>');
        $("label[for='gross_price']").append('<span class="required"> *</span>');
        $("label[for='clean_price']").append('<span class="required"> *</span>');
        $("label[for='modifiedduration']").append('<span class="required"> *</span>');
        $("label[for='convexity']").append('<span class="required"> *</span>');
    }

    function setFormBBG() {
        $('#panel-BBG').show();

        //set labal not require
        $("label[for='ai']").find('span').remove('span');
        $("label[for='gross_price']").find('span').remove('span');
        $("label[for='clean_price']").find('span').remove('span');
        $("label[for='modifiedduration']").find('span').remove('span');
        $("label[for='convexity']").find('span').remove('span');

        //remove error
        $("span[data-valmsg-for='ai']").text('');
        $('#ai').removeClass("input-validation-error");

        $("span[data-valmsg-for='gross_price']").text('');
        $('#gross_price').removeClass("input-validation-error");

        $("span[data-valmsg-for='clean_price']").text('');
        $('#clean_price').removeClass("input-validation-error");

        $("span[data-valmsg-for='modifiedduration']").text('');
        $('#modifiedduration').removeClass("input-validation-error");

        $("span[data-valmsg-for='convexity']").text('');
        $('#convexity').removeClass("input-validation-error");
    }

    function validateForm() {
        var isValidate = true;

        if ($('#txt_asof_date').val().length) {
            $("span[data-valmsg-for='asof_date']").text('');
            $('#txt_asof_date').removeClass("input-validation-error");
        } else {
            $("span[data-valmsg-for='asof_date']").text('Process Date field is required.');
            $('#txt_asof_date').addClass("input-validation-error");
            focusInput($('#txt_asof_date'));
            isValidate = false;
        }

        if ($('#instrument_id').val().length) {
            $("span[data-valmsg-for='instrument_id']").text('');
            $('#ddl_instrument_code').removeClass("input-validation-error");
        } else {
            $("span[data-valmsg-for='instrument_id']").text('Instrument Code field is required.');
            $('#ddl_instrument_code').addClass("input-validation-error");
            focusInput($('#ddl_instrument_code'));
            isValidate = false;
        }

        if ($('#marketdate_t').val().length) {
            $("span[data-valmsg-for='marketdate_t']").text('');
            $('#ddl_marketdate_t').removeClass("input-validation-error");
        } else {
            $("span[data-valmsg-for='marketdate_t']").text('Market Date (T+x) field is required.');
            $('#ddl_marketdate_t').addClass("input-validation-error");
            focusInput($('#ddl_marketdate_t'));
            isValidate = false;
        }

        if ($("#price_source").val() == 'TBMA') {
            if ($('#ai').val().length) {
                $("span[data-valmsg-for='ai']").text('');
                $('#ai').removeClass("input-validation-error");
            } else {
                $("span[data-valmsg-for='ai']").text('AI % field is required.');
                $('#ai').addClass("input-validation-error");
                focusInput($('#ai'));
                isValidate = false;
            }

            if ($('#gross_price').val().length) {
                $("span[data-valmsg-for='gross_price']").text('');
                $('#gross_price').removeClass("input-validation-error");
            } else {
                $("span[data-valmsg-for='gross_price']").text('Gross Price % field is required.');
                $('#gross_price').addClass("input-validation-error");
                focusInput($('#gross_price'));
                isValidate = false;
            }

            if ($('#clean_price').val().length) {
                $("span[data-valmsg-for='clean_price']").text('');
                $('#clean_price').removeClass("input-validation-error");
            } else {
                $("span[data-valmsg-for='clean_price']").text('Clean Price % field is required.');
                $('#clean_price').addClass("input-validation-error");
                focusInput($('#clean_price'));
                isValidate = false;
            }

            if ($('#modifiedduration').val().length) {
                $("span[data-valmsg-for='modifiedduration']").text('');
                $('#modifiedduration').removeClass("input-validation-error");
            } else {
                $("span[data-valmsg-for='modifiedduration']").text('Modified Duration field is required.');
                $('#modifiedduration').addClass("input-validation-error");
                focusInput($('#modifiedduration'));
                isValidate = false;
            }

            if ($('#convexity').val().length) {
                $("span[data-valmsg-for='convexity']").text('');
                $('#convexity').removeClass("input-validation-error");
            } else {
                $("span[data-valmsg-for='convexity']").text('Convexity field is required.');
                $('#convexity').addClass("input-validation-error");
                focusInput($('#convexity'));
                isValidate = false;
            }
        }

        return isValidate;
    }

    var budate = $("#BusinessDate").text();

    var formatmmddyyyydate = budate.split("/");
    formatmmddyyyydate = formatmmddyyyydate[1] + "/" + formatmmddyyyydate[0] + "/" + formatmmddyyyydate[2];
    var b_date = new Date(formatmmddyyyydate);

    if ($('#FormSearch_asof_date').length) {
        $('#FormSearch_asof_date').data("DateTimePicker").date(b_date);
    }

    //ini Search
    if ($("#FormSearch_price_source").val() == '') {
        $("#FormSearch_price_source").val('TBMA');
        $("#FormSearch_price_source").val('TBMA');
        $("#ddl_rp_source").find(".selected-data").text('TBMA');
        $("#ddl_rp_source").find(".selected-data").val('TBMA');
        $("#ddl_rp_source").find(".selected-value").val('TBMA');
    }

    //ini Add
    if ($("#price_source").val() == '') {
        $("#price_source").val('TBMA');
        $("#price_source").val('TBMA');
        $("#ddl_rp_source").find(".selected-data").text('TBMA');
        $("#ddl_rp_source").find(".selected-data").val('TBMA');
        $("#ddl_rp_source").find(".selected-value").val('TBMA');

        setFormTBMA();
    }
    else if ($("#price_source").val() == 'TBMA') {
        setFormTBMA();
    }
    else if ($("#price_source").val() == 'BBG') {
        setFormBBG();
    }

    //InstrumentCode Dropdown
    var checkinstrumentcode;
    $("#ddl_instrument_code").click(function () {
        var txt_search = $('#txt_instrument_code');
        var mat_date = "";
        checkinstrumentcode = $('#instrument_code').val();

        var data = { instrument: null };//, maturitydate: mat_date };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_instrument_code').keyup(function () {
        var mat_date = "";
        //if (this.value.length > 0) {
        var data = { instrument: this.value };//, maturitydate: mat_date  };
        GM.Utility.DDLAutoComplete(this, data, null);
        //}
    });
    //End InstrumentCode Dropdown

    //RP Source Dropdown
    $("#ddl_rp_source").click(function () {
        var txt_search = $('#txt_rp_source');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null, false);
        txt_search.val("");
    });

    $('#txt_rp_source').keyup(function () {

        //if (this.value.length > 0) {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null, false);
        //}
    });

    $('#ul_rp_source').click(function() {
        if ($('#price_source').val() == 'BBG') {
            setFormBBG();
        } else {
            setFormTBMA();
        }
    });

    //End RP Source Dropdown

    //Function Search ==============================================
    //Binding Div Detail
    GM.System.Sidebar.Handler = function () {
        $('#x-table-data').DataTable().columns.adjust();
    };

    GM.RPReferece = {};
    GM.RPReferece.Table = $('#x-table-data').DataTable({
        dom: 'Bfrtip',
        searching: true,
        scrollY: '80vh',
        scrollX: true,
        order: [
            [2, "desc"]
        ],
        buttons: [],
        processing: true,
        serverSide: true,
        ajax: {
            "url": "/RPReferece/Search",
            "type": "POST",
            "error": function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR);
                console.log(textStatus);
                console.log(errorThrown);
            }
        },
        select: {
            style: 'os',
            selector: 'td:first-child'
        },
        columnDefs:
            [
                { targets: 0, data: "RowNumber", orderable: false },
                { targets: 1, data: "price_source" },
                { targets: 2, data: "instrument_id", visible: false },
                {
                    targets: 3, data: "asof_date",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                { targets: 4, data: "instrument_code" },
                { targets: 5, data: "bondtype" },
                { targets: 6, data: "marketdate_t" },
                {
                    targets: 7, data: "maturity_date",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                {
                    targets: 8, data: "settlementdate",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                {
                    targets: 9, data: "processdate",
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                { targets: 10, data: "ai", orderable: false },
                { targets: 11, data: "clean_price", orderable: false },
                { targets: 12, data: "gross_price", orderable: false },
                { targets: 13, data: "avgbidding", orderable: false },
                { targets: 14, data: "ttm", orderable: false },
                { targets: 15, data: "govtinterpolatedyield", visible: false },
                {
                    targets: 16, data: "spreadquoteddate", orderable: false,
                    render: function (data, type, row, meta) {
                        if (data != null) {
                            return moment(data).format('DD/MM/YYYY');
                        }
                        return data;
                    }
                },
                { targets: 17, data: "spread", orderable: false },
                { targets: 18, data: "referenceyield", visible: false },
                { targets: 19, data: "modifiedduration", orderable: false },
                { targets: 20, data: "convexity", orderable: false },

                {
                    targets: 21,
                    data: "instrument_id", orderable: false,
                    className: "dt-body-center",
                    width: 60,
                    render: function (data, type, row, meta) {
                        var html = '';
                        html += '<button class="btn btn-default btn-round" form-mode="edit"   onclick="location.href=\'/RPReferece/Edit?price_source=' + row.price_source + '&asof_date=' + moment(row.asof_date).format('DD/MM/YYYY') + '&instrument_id=' + row.instrument_id + '&marketdate_t=' + row.marketdate_t + '\'" @(!IsView? "disabled":"") ><i class="feather-icon icon-edit"></i></button>';
                        html += '<button class="btn btn-delete  btn-round" key="' + row.price_source + '" key2="' + moment(row.asof_date).format('DD/MM/YYYY') + '" key3="' + row.instrument_id + '" key4="' + row.marketdate_t + '" form-mode="delete" onclick="GM.RPReferece.Delete(this)" @(!IsDelete? "disabled":"") ><i class="feather-icon icon-trash-2"></i></button>';
                        return html;
                    }
                }
            ],
        fixedColumns: {
            leftColumns: 1,
            rightColumns: 1
        },
        fnPreDrawCallback: function () {
            $('#x-table-data').DataTable().columns(1).search($('#FormSearch_price_source').val());
            $('#x-table-data').DataTable().columns(3).search($('#FormSearch_asof_date').val());
        }
    });

    GM.RPReferece.Delete = function (btn) {
        var mode = $(btn).attr("form-mode");
        GM.Message.Clear();
        if (mode) {
            var key = $(btn).attr("key");
            var key2 = $(btn).attr("key2");
            var key3 = $(btn).attr("key3");
            var key4 = $(btn).attr("key4");
            switch (mode) {
                case "delete":
                    Delete(key, key2, key3, key4);
                    break;
            }
        }
    };

    GM.RPReferece.Form = function (btn) {
        var mode = $(btn).attr("form-mode");
        GM.Message.Clear();
        if (mode) {
            //var key = $(btn).attr("key");
            //var key2 = $(btn).attr("key2");
            //switch (mode) {
            //    case "edit":
            //        alert("test");
            //}
        }
    };

    GM.RPReferece.Form.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];
            switch (key) {
                case "price_source": GM.RPReferece.Table.columns(1).search($(this).val()); break;
                case "asof_date": GM.RPReferece.Table.columns(3).search($(this).val()); break;
                case "instrument_code": GM.RPReferece.Table.columns(4).search($(this).val()); break;

            }
        });

        GM.RPReferece.Table.draw();
    };

    GM.RPReferece.Form.Initial = function () {
        $("#action-form")[0].reset();
    };
    GM.RPReferece.Form.DataBinding = function (p) {
        $('#' + p.form + ' :input').each(function () {
            var input = $(this);
            var inputid = input[0].id;
            var key = input[0].name.split('.')[1];
            var inputtype = input.attr("type");
            var inputvalue = input.attr("value");
            if (p.data[key] != null) {
                if (inputtype == "radio") {
                    var inputyes = $("[id=" + inputid + "][value=true]");
                    var inputno = $("[id=" + inputid + "][value=false]");
                    if (p.data[key]) {
                        inputno.removeAttr('checked');
                        inputno.attr('ischeck', 'false');
                        inputyes.attr('ischeck', 'true');
                        inputyes.attr('checked', 'checked');
                        inputyes.prop('checked', true);
                    }
                    else {
                        inputyes.removeAttr('checked');
                        inputyes.attr('ischeck', 'false');
                        inputno.attr('ischeck', 'true');
                        inputno.attr('checked', 'checked');
                        inputno.prop('checked', true);
                        //}
                    }
                }
                else {
                    $(this).val(p.data[key] + '');
                }

            }
            console.log('(' + key + ') => ' + p.data[key]);
        });
        $('#' + p.form + ' span').each(function () {
            var input = $(this);
            var key = input.attr("name");
            if (typeof key != "undefined") {
                if (p.data[key] != "" && p.data[key] != null) {
                    $(this)[0].innerHTML = p.data[key];
                }
            }
            console.log('(' + key + ') => ' + p.data[key]);
        });
    };
    GM.RPReferece.Form.GenExcel = function (btn) {

        ExportExcel();
    };
    $("#search-form").on('submit', function (e) {
        e.preventDefault();
        GM.Message.Clear();
        GM.RPReferece.Form.Search();
    });
    $("#search-form").on('reset', function (e) {
        GM.Defer(function () {
            $('#FormSearch_asof_date').data("DateTimePicker").date(b_date);

            $("#FormSearch_price_source").val('TBMA');
            $("#FormSearch_price_source").val('TBMA');
            $("#ddl_rp_source").find(".selected-data").text('TBMA');
            $("#ddl_rp_source").find(".selected-data").val('TBMA');
            $("#ddl_rp_source").find(".selected-value").val('TBMA');

            GM.Message.Clear();
            GM.RPReferece.Form.Search();
        }, 100);
    });

    $('#add-form').on('submit', function (e) {
        
        //validate
        if (!validateForm()) {
            e.preventDefault();
            return;
        }
    });

    $('#add-form').on('reset', function (e) {

        $('.spinner').css('display', 'block'); // Open Loading
        location.href = window.location.href;
    });
});