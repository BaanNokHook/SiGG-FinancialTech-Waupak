
$(document).ready(function () {
    GM.Utility.GetBusinessDate("asofdate_string");
    //Trans status Dropdown
    $("#ddl_trans_status").click(function() {
        var txt_search = $("#txt_trans_status");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    //End Trans status Dropdown

    //cust type Dropdown
    $("#ddl_cust_type").click(function() {
        var txt_search = $("#txt_cust_type");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    //End cust type Dropdown

    //report type Dropdown
    $("#ddl_report_type").click(function() {
        var txt_search = $("#txt_report_type");
        var data = { datastr: null };
        GM.Utility.DDLAutoComplete(txt_search, data, null);
        txt_search.val("");
    });

    //End report type Dropdown

});

formreset = function () {
    GM.Message.Clear();

    $("#ddl_trans_status").find(".selected-data").text("Select...");
    $("#trans_status").val(null);
    $("#trans_status").text(null);
    $("#trans_status_name").val(null);
    $("#trans_status_name").text(null);

    $("#ddl_cust_type").find(".selected-data").text("Select...");
    $("#cust_type").val(null);
    $("#cust_type").text(null);
    $("#cust_type_name").val(null);
    $("#cust_type_name").text(null);

    $("#ddl_report_type").find(".selected-data").text("Select...");
    $("#report_type").val(null);
    $("#report_type").text(null);
    $("#report_type_name").val(null);
    $("#report_type_name").text(null);

    
    $("#trans_no").val("");

    // set default date  
    GM.Utility.GetBusinessDate("asofdate_string");
};