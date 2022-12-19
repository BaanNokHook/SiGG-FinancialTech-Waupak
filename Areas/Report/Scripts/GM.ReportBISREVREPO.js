
$(window).on('load', function() {
    var action = "/OutstandingReport/GetBusinessDate";
    GM.Utility.GetBusinessDate("asofdate_string", action);

    var defultCur = "THB";

    var defultExecuteType = "Query";

    $("#ddl_cur").find(".selected-data").text(defultCur);
    $("#ddl_cur").find(".selected-value").val(defultCur);


    $("#ddl_execute_type").find(".selected-data").text(defultExecuteType);
    $("#ddl_execute_type").find(".selected-value").val(defultExecuteType);
});

$("#ddl_cur").click(function() {
    var txt_search = $("#txt_cur");
    var data = { datastr: null };
    GM.Utility.DDLAutoComplete(txt_search, data, null, false);
    txt_search.val("");
});

$("#txt_cur").keyup(function() {
    var data = { datastr: this.value };
    GM.Utility.DDLAutoComplete(this, data, null, false);
});

$("#ddl_execute_type").click(function() {
    var txt_search = $("#txt_exe");
    var data = { datastr: null };
    GM.Utility.DDLAutoComplete(txt_search, data, null, false);
});

$("#txt_exe").keyup(function() {
    var data = { datastr: this.value };
    GM.Utility.DDLAutoComplete(this, data, null, false);
});