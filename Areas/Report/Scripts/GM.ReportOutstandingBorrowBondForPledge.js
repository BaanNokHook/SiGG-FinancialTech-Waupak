$(document).ready(function() {
    var action = "Report/ReportOutstandingBorrowBondForPledge/GetBusinessDate";
    var id = "asofdate_string";
    GM.Utility.GetBusinessDate(id, action);
});