$(document).ready(function () {
    $("#ddl_exchange_type").attr("disabled", "disabled");

    //Binding ddl_role
    $("#ddl_source_type").click(function () {
        var txt_search = $('#txt_source_type');
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_source_type').keyup(function () {
        var data = { datastr: this.value };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    $("#ul_source_type").on("click", ".searchterm", function (event) {
        $("#ddl_exchange_type").find(".selected-data").text("Select...");
        $("#FormSearch_exchange_type_code").val(null);
        $("#FormSearch_exchange_type").val(null);

        if ($("#FormSearch_source_type_code").val().length > 0) {
            $("#ddl_exchange_type").removeAttr('disabled');
        }
        else {
            $("#ddl_exchange_type").attr("disabled", "disabled");
        }
    });

    $("#ddl_exchange_type").click(function () {
        var txt_search = $('#txt_exchange_type');
        var data = { datastr: $("#FormSearch_source_type_code").val() };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    $('#txt_exchange_type').keyup(function () {
        var data = { datastr: $("#FormSearch_source_type_code").val() };
        GM.Utility.DDLAutoComplete(this, data, null);
    });

    GM.ExchangeRate.Form.Search = function () {
        $('#search-form :input').each(function () {
            var input = $(this);
            var key = input[0].name.split('.')[1];

            switch (key) {
                case "source_type_code": GM.ExchangeRate.Table.columns(1).search($(this).val()); break;
                case "exchange_type_code": GM.ExchangeRate.Table.columns(3).search($(this).val()); break;
                case "asof_date": GM.ExchangeRate.Table.columns(5).search($(this).val()); break;
            }

        });
        GM.ExchangeRate.Table.draw();
    };

    $("#search-form").on('submit', function (e) {

        e.preventDefault();
        GM.Message.Clear();
        GM.ExchangeRate.Form.Search();
    });

    $("#search-form").on('reset', function (e) {

        GM.Defer(function () {
            $('#ddl_source_type').find(".selected-data").text("Select...");
            $('#FormSearch_source_type_code').val(null);
            $('#FormSearch_source_type').val(null);

            $('#ddl_exchange_type').find(".selected-data").text("Select...");
            $('#FormSearch_exchange_type_code').val(null);
            $('#FormSearch_exchange_type').val(null);

            GM.Message.Clear();
            GM.ExchangeRate.Form.Search();
        }, 100);

    });
});