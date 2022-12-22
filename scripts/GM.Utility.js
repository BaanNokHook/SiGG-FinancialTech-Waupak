GM.Utility = {};

GM.Utility.Table = {
    Render: {
        Date: function (data, type, row, meta) {
            if (data != null) {
                return moment(data).format('DD/MM/YYYY');
            }
            return data;
        },

        DateTime: function (data, type, row, meta) {
            if (data != null) {
                return moment(data).format('DD/MM/YYYY HH:mm:ss');
            }
            return data;
        }
    }
};

//Start DDL
GM.Utility.DDLAutoComplete = function (ddl, data, txtid_for_setvalue, isSelect, text) {
    var action = $(ddl).data('action');
    var idtext = $(ddl).attr("id");
    var ul = $("#" + idtext).parent().parent().find("ul");
    var isSelected = isSelect == null ? true : isSelect;
    var defultText = text == null ? "Select..." : text;
    data.t = new Date().getTime();
    $.ajax({
        url: action,
        type: "GET",
        dataType: "JSON",
        data: data,
        beforeSend: function () {
            ul.html("").append('<li style="text-align: center;"><img src="/Content/img/ajax-loader.gif" /></li>');
        },
        success: function (res) {
            ul.html(""); // clear before appending new list
            //Add Select Data
            var li = $("<li class='searchterm'></li>");
            var ta = "";
            if (res.length) {
                if (isSelected) {
                    ta = $("<a  onclick='ClearDDL(\"" + idtext + "\",\"" + defultText + "\" )'></a>").val("").html(defultText);
                }
            } else {
                ta = $("<a></a>").val("").html("No Data");
            }
            li.append(ta);
            ul.append(li);
            $.each(res, function (i, resdata) {
                var li = $("<li class='searchterm'></li>");
                var ta = $("<a data-toggle='tooltip'  title='" + encodeHTML(resdata.Text) + "' onclick='AddTextForDDL(\"" + encodeHTML(resdata.Text) + "\",\"" + idtext + "\",\"" + encodeHTML(resdata.Value) + "\",\"" + txtid_for_setvalue + "\",\"" + encodeHTML(resdata.Value2) + "\")'></a>").val(encodeHTML(resdata.Value)).html(encodeHTML(resdata.Text));
                li.append(ta);
                ul.append(li);
            });
        },
        error: function (jqXhr, textStatus) {
            if (textStatus === "error") {
                var objJson = jQuery.parseJSON(jqXhr.responseText);

                if (Object.prototype.toString.call(objJson) === '[object Array]' && objJson.length == 0) {
                    // Array is empty
                    // Do Something
                } else {
                    var errorMsg = jqXhr.statusText + " " + objJson.Message;
                    alert("An error occurred, " + errorMsg, function (e) {

                    }, {
                            ok: "OK",
                            classname: "custom-class"
                        });
                }
            }
        }
    });
};

GM.Utility.DDLAutoCompleteSet4Value = function (ddl, data, txtid_for_setvalue2, txtid_for_setvalue3, txtid_for_setvalue4) {
    var action = $(ddl).data('action');
    var idtext = $(ddl).attr("id");
    var ul = $("#" + idtext).parent().parent().find("ul");
    data.t = new Date().getTime();
    $.ajax({
        url: action,
        type: "GET",
        dataType: "JSON",
        data: data,
        beforeSend: function () {
            ul.html("").append('<li style="text-align: center;"><img src="/Content/img/ajax-loader.gif" /></li>');
        },
        success: function (res) {
            ul.html(""); // clear before appending new list
            //Add Select Data
            var li = $("<li class='searchterm'></li>");
            var ta = "";
            if (res.length) {
                ta = $("<a  onclick='ClearDDL(\"" + idtext + "\")'></a>").val("").html("Select...");
            } else {
                ta = $("<a  onclick='ClearDDL(\"" + idtext + "\")'></a>").val("").html("No Data");
            }
            li.append(ta);
            ul.append(li);
            $.each(res, function (i, resdata) {
                var li = $("<li class='searchterm'></li>");
                var ta = $("<a data-toggle='tooltip' title='" + encodeHTML(resdata.Text) + "' onclick='AddTextForDDL4Value(\"" + encodeHTML(resdata.Text) + "\",\"" + idtext + "\",\"" + encodeHTML(resdata.Value) + "\",\"" + txtid_for_setvalue2 + "\",\"" + encodeHTML(resdata.Value2) + "\",\"" + txtid_for_setvalue3 + "\",\"" + encodeHTML(resdata.Value3) + "\",\"" + txtid_for_setvalue4 + "\",\"" + encodeHTML(resdata.Value4) + "\")'></a>").val(encodeHTML(resdata.Value)).html(encodeHTML(resdata.Text));
                li.append(ta);
                ul.append(li);
            });
        }
    });
};

GM.Utility.DDLNoData = function (ddl) {
    var idtext = $(ddl).attr("id");
    var ul = $("#" + idtext).parent().parent().find("ul");
    ul.html(""); // clear before appending new list
    //Add Select Data
    var li = $("<li class='searchterm'></li>");
    var ta = $("<a  onclick='ClearDDL(\"" + idtext + "\")'></a>").val("").html("No Data");
    li.append(ta);
    ul.append(li);
};

GM.Utility.DDLStandard = function (id, data, action, defaultvalue, defaultText) {
    var text = defaultText == null ? "- Please select -" : defaultText;
    var checkDefaultvalue = false;
    $.ajax({
        url: action,
        type: "GET",
        dataType: "JSON",
        data: data,
        success: function (res) {
            $("#" + id).html(""); // clear before appending new list
            $("#" + id).append($('<option></option>').val("").html(text));
            $.each(res, function (i, resdata) {
               
                checkDefaultvalue = (defaultvalue == resdata.Value) || checkDefaultvalue ? true : false;

                $("#" + id).append(
                    $('<option></option>').val(resdata.Value).html(resdata.Text));
            });
            if (defaultvalue != null && checkDefaultvalue) {
                $("#" + id).val(defaultvalue);
                Setvalue($("#" + id));
            }
        }
    });
};

GM.Utility.GetBusinessDate = function (id, action) {
    action = 'SystemInformation/GetBusinessDate';
    $.ajax({
        url: action,
        type: "GET",
        dataType: "JSON",
        data: null,
        success: function (res) {
            if (!res.Error) {
                var business = new Date(res.Data);
                $("#" + id).data("DateTimePicker").date(business);
            }
            else {
                swal("Warning", res.Error_detail, "warning");
            }
        }
    });
};

function AddTextForDDL(stringtext, idtxt, value, txtid_for_setvalue, value2) {
    var textvalue = $("#" + idtxt).parent().parent().parent().find("button").find(".selected-data");
    textvalue.text(stringtext);
    textvalue.val(stringtext);
    textvalue.attr("data-toggle", "tooltip");
    textvalue.attr("title", stringtext);
    $("#" + idtxt).parent().parent().parent().find("button").find(".selected-value").val(value);
    if (txtid_for_setvalue != null && txtid_for_setvalue != 'null') {
        if ($("#" + txtid_for_setvalue).attr("type") == "button") {
            $("#" + txtid_for_setvalue).find(".selected-data").text(value2);
            $("#" + txtid_for_setvalue).find(".selected-data").val(value2);
            $("#" + txtid_for_setvalue).find(".selected-value").val(value2);

        } else {
            $("#" + txtid_for_setvalue).val(value2);
            $("#" + idtxt).val("");
        }
    }
}

function AddTextForDDL4Value(stringtext, idtxt, value, txtid_for_setvalue2, value2, txtid_for_setvalue3, value3, txtid_for_setvalue4, value4) {
    var textvalue = $("#" + idtxt).parent().parent().parent().find("button").find(".selected-data");
    textvalue.text(stringtext);
    textvalue.val(stringtext);
    textvalue.attr("data-toggle", "tooltip");
    textvalue.attr("title", stringtext);
    //$("#" + idtxt).parent().parent().parent().find("button").find(".selected-data").text(stringtext);
    $("#" + idtxt).parent().parent().parent().find("button").find(".selected-value").val(value);
    if (txtid_for_setvalue2 != null) {
        if ($("#" + txtid_for_setvalue2).attr("type") == "button") {
            $("#" + txtid_for_setvalue2).find(".selected-data").text(value2);
            $("#" + txtid_for_setvalue2).find(".selected-data").val(value2);
            $("#" + txtid_for_setvalue2).find(".selected-value").val(value2);

        }
        else if ($("#" + txtid_for_setvalue2).attr("type") == "PFor") {
            $("#" + txtid_for_setvalue2).find(".selected-data").text(value2);
            $("#" + txtid_for_setvalue2).find(".selected-data").val(value2);
        }
        else {
            $("#" + txtid_for_setvalue2).val(value2);
            $("#" + txtid_for_setvalue2).text(value2);
            $("#" + idtxt).val("");
        }
    }
    if (txtid_for_setvalue3 != null) {
        if ($("#" + txtid_for_setvalue3).attr("type") == "button") {
            $("#" + txtid_for_setvalue3).find(".selected-data").text(value3);
            $("#" + txtid_for_setvalue3).find(".selected-data").val(value3);
            $("#" + txtid_for_setvalue3).find(".selected-value").val(value3);

        }
        else if ($("#" + txtid_for_setvalue3).attr("type") == "PFor") {
            $("#" + txtid_for_setvalue3).find(".selected-data").text(value3);
            $("#" + txtid_for_setvalue3).find(".selected-data").val(value3);
        } else {
            $("#" + txtid_for_setvalue3).val(value3);
            $("#" + txtid_for_setvalue3).text(value3);
            $("#" + idtxt).val("");
        }
    }
    if (txtid_for_setvalue4 != null) {
        if ($("#" + txtid_for_setvalue4).attr("type") == "button") {
            $("#" + txtid_for_setvalue4).find(".selected-data").text(value4);
            $("#" + txtid_for_setvalue4).find(".selected-data").val(value4);
            $("#" + txtid_for_setvalue4).find(".selected-value").val(value4);

        }
        else if ($("#" + txtid_for_setvalue4).attr("type") == "PFor") {
            $("#" + txtid_for_setvalue4).find(".selected-data").text(value4);
            $("#" + txtid_for_setvalue4).find(".selected-data").val(value4);
        } else {
            $("#" + txtid_for_setvalue4).val(value4);
            $("#" + txtid_for_setvalue4).text(value4);
            $("#" + idtxt).val("");
        }
    }
}

function ClearDDL(idtxt, text) {
    var textvalue = $("#" + idtxt).parent().parent().parent().find("button").find(".selected-data");
    var textDefult = text == null ? "Select..." : text;
    textvalue.text(textDefult);
    textvalue.val("");
    $("#" + idtxt).parent().parent().parent().find("button").find(".selected-value").val("");
    $("#" + idtxt).val("");
}
//End DDL
function Setvalue(btn) {
    //alert($(btn).val() + "test :" + $(btn).text() );
    var value = $(btn).val();
    $(btn).prev().val(value).prev().val(value);
}

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

function setformatdatemmddyyyy(date) {
    if (date != "") {
        date = date.split('/');
        date = date[1] + "/" + date[0] + "/" + date[2];
    }
    else {
        date = 0;
    }
    return date;
}

function encodeHTML(s) {
    if (s != null) {
        return s.split('&').join('&amp;').split('<').join('&lt;').split('"').join('&quot;').split("'").join('&#39;');
    }
    return s;
}