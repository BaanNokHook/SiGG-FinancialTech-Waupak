//initial  GM Object
var GM = {};
GM.System = {};
GM.System.Sidebar = {};
GM.View = {};
GM.View.Tables = [];
GM.Form = {};

GM.Time = function () {
    return new Date().getTime();
};

//message
GM.Message = {
    Error: function (divName, message) {
        $(divName).prepend('<div class="alert alert-danger alert-dismissible" role="alert">' +
            '<button type= "button" class="close" data-dismiss="alert" aria-label="Close" >' +
            '<i class="feather-icon icon-x"></i>' +
            '</button>' +
            '<div class="ritle">' +
            '<h4>' +
            '<strong>Request Failed!!</strong>' +
            '</h4>' +
            '</div>' +
            '<div class="content">' + message + '</div>' +
            '</div>');

        return this;
    },

    Success: function (divName, message) {
        $(divName).prepend('<div class="alert alert-success alert-dismissible" role="alert">' +
            '<button type= "button" class="close" data-dismiss="alert" aria-label="Close" >' +
            '<i class="feather-icon icon-x"></i>' +
            '</button>' +
            '<div class="ritle">' +
            '<h4>' +
            '<strong>Request Completed</strong>' +
            '</h4>' +
            '</div>' +
            '<div class="content">' + message + '</div>' +
            '</div>');
    },

    Clear: function () {
        $(".alert").remove();
    }
};

GM.Mask = function (divName) {
    //$(divName).prepend('<div class="loader" >Loading...</div>');


    var container = $(divName),
        loader = $('<div class="loader" ></div>'),
        overlay = $('<div></div>'),
        mainover = $('<div class="x-loadmask" ></div>');



    //if (overlayExist === 0) {
    //    container.css('position', 'relative');
    //    overlay.css({
    //        position: 'absolute',
    //        top: 0,
    //        left: 0,
    //        height: container.outerHeight(),
    //        width: container.outerWidth(),
    //        opacity: 0.8,
    //        zIndex: 5000,
    //        backgroundColor: '#000000',
    //        textAlign: 'center',
    //        color: '#ffffff'
    //    }).addClass('loader');

    overlay.css({
        position: 'absolute',

        left: -17,
        zIndex: 5000,
        opacity: 0.3,
        backgroundColor: '#e5e5e5',
        height: container.outerHeight(),
        width: container.outerWidth(),
    });

    loader.css({
        zIndex: 5001,
        top: ((overlay.outerHeight() / 2) - 100),
        left: (overlay.outerWidth() / 2),
        position: 'absolute'
    });


    mainover.css({
        position: 'absolute',

        top: 0

    });
    mainover.append(loader);
    mainover.append(overlay);

    container.append(mainover);
}

GM.Unmask = function () {
    $(".x-loadmask").remove();
};

GM.Defer = function (fn, _interval) {
    setTimeout(function () {
        fn();
    }, _interval);
};

// Treefilter Plugin
$(function () {
    // var tree = new treefilter($(".menu .item-set"), { searcher: $("input#menu-search") });
    GM.TreeMenu = new treefilter($(".menu"), { searcher: $("input#menu-search"), expanded: true });
});

////Make Bootstrap button do select
//$(".dropdown-form .dropdown li a").click(function () {
//  $(this).parents(".dropdown").find('.selected-data').html($(this).text());
//  $(this).parents(".dropdown").find('.selected-data').val($(this).data('value'));
//});
//$(".option .dropdown li a").click(function () {
//  $(this).parents(".dropdown").find('.selected-data').html($(this).text());
//  $(this).parents(".dropdown").find('.selected-data').val($(this).data('value'));
//});

/*See-more See-less for form*/
$('#user-master').click(function () {
    $(this).parents(".box-content").find('.advance-form').slideToggle(0);
    //$table.floatThead("reflow");
    if ($('#user-master i').hasClass('icon-chevron-up')) {
        $(this).html('Show more option <i class="feather-icon icon-chevron-down"></i>');
    } else {
        $(this).html('Show less option <i class="feather-icon icon-chevron-up"></i>');
    }
});

// Toggle Side bar
$('.sidebar-toggle-xs .btn').click(function () {


    if ($(".main-sidebar").hasClass('hide-sidebar')) {
        $.cookie("main-sidebar", "0");
    }
    else {
        $.cookie("main-sidebar", "1");
    }
    try {
        setTimeout(
            function () {
                $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();
            },
            300);
    } catch (e) {
        console.log(e.message);
    }


    $('.working-section').toggleClass('hide-sidebar');
    $('.main-sidebar').toggleClass('hide-sidebar');
    $('.top-bar .navbar-default').toggleClass('full-width');

    if (GM.System.Sidebar.Handler) {
        setTimeout(GM.System.Sidebar.Handler, 300);
    }

});

//$table.floatThead("reflow");

/*Dropdown table setting (Prevent close when click)*/
$('.dropdown-menu.checkbox-menu').click(function (e) {
    e.stopPropagation();
});

// Toggle Data table expand
$("td.view-more .btn").click(function () {
    $(this).closest("tr").next("tr").toggleClass("tr-show");
    $(this).toggleClass("active");
    $(this).find(".feather-icon").toggleClass("icon-plus icon-minus");
});

/*
var $table = $('#old-table');
$table.floatThead({
  top: 54
});

$('.alert').on('closed.bs.alert', function (e) {
  $table.floatThead("reflow");
});
*/



//for date picker
//$('.date').datetimepicker({  format: 'DD/MM/YYYY'}).on('dp.change', function (e) {

//});

//$('.datetime').datetimepicker({ format: 'DD/MM/YYYY HH:mm:ss' }).on('dp.change', function (e) {

//});

// Date Time picker
$(function () {
    $('.date-time-picker').datetimepicker({
        //sideBySide: true,
        format: "DD/MM/YYYY",
        //debug: true,
        //showTodayButton: true,
        //keepOpen: true,
        //showClear: true,
        icons: {
            time: 'feather-icon icon-clock',
            date: 'feather-icon icon-calendar',
            up: 'feather-icon icon-chevron-up',
            down: 'feather-icon icon-chevron-down',
            previous: 'feather-icon icon-arrow-left',
            next: 'feather-icon icon-arrow-right',
            today: 'feather-icon icon-refresh-ccw',
            clear: 'feather-icon icon-trash-2',
            close: 'fetaher-icon icon-x'
        }
    });
});

// Date Time picker
$(function () {
    $('.date-time-picker-weekend').datetimepicker({
        //sideBySide: true,
        format: "DD/MM/YYYY",
        //debug: true,
        //showTodayButton: true,
        //keepOpen: true,
        //showClear: true,
        daysOfWeekDisabled: [0, 6],
        useCurrent: false,
        //maxDate: datenow,
        icons: {
            time: 'feather-icon icon-clock',
            date: 'feather-icon icon-calendar',
            up: 'feather-icon icon-chevron-up',
            down: 'feather-icon icon-chevron-down',
            previous: 'feather-icon icon-arrow-left',
            next: 'feather-icon icon-arrow-right',
            today: 'feather-icon icon-refresh-ccw',
            clear: 'feather-icon icon-trash-2',
            close: 'fetaher-icon icon-x'
        }
    }).on("dp.show",
        function () {
            //$('td.day[data-day="09/27/2018"]').addClass('today');
            //$(this).data('DateTimePicker').date("01/01/1980");
        });
});
// Date picker
$(function () {
    $('.date-picker').datetimepicker({
        //sideBySide: true,
        format: "DD/MM/YYYY",
        //debug: true,
        //showTodayButton: true,
        //keepOpen: true,
        //showClear: true,
        icons: {
            time: 'feather-icon icon-clock',
            date: 'feather-icon icon-calendar',
            up: 'feather-icon icon-chevron-up',
            down: 'feather-icon icon-chevron-down',
            previous: 'feather-icon icon-arrow-left',
            next: 'feather-icon icon-arrow-right',
            today: 'feather-icon icon-refresh-ccw',
            clear: 'feather-icon icon-trash-2',
            close: 'fetaher-icon icon-x'
        }
    });
});
// Date&Month Only
$(function () {
    $('.date-month-picker').datetimepicker({
        //sideBySide: true,
        format: "DD/MM",
        //debug: true,
        //showTodayButton: false,
        //keepOpen: true,
        //showClear: true,
        viewMode: 'months',
        //minViewMode: 'months',
        //maxViewMode: 'months',
        //startView: 'months',
        icons: {
            time: 'feather-icon icon-clock',
            date: 'feather-icon icon-calendar',
            up: 'feather-icon icon-chevron-up',
            down: 'feather-icon icon-chevron-down',
            previous: 'feather-icon icon-arrow-left',
            next: 'feather-icon icon-arrow-right',
            today: 'feather-icon icon-refresh-ccw',
            clear: 'feather-icon icon-trash-2',
            close: 'fetaher-icon icon-x'
        }
    });
});

$(".date-month-picker").on("dp.show", function (e) {
    $(e.target).data("DateTimePicker").viewMode("months");
});

$('.show-popover').webuiPopover();

$(".icon-calendar").click(function () {

    var index = $(".icon-calendar").index(this);
    var item = $(".icon-calendar").get(index);
    var doc = item.parentNode.parentNode;
    var notes = null;
    for (var i = 0; i < doc.childNodes.length; i++) {
        if (doc.childNodes[i].className == "form-control date-time-picker") {
            notes = doc.childNodes[i];
            break;
        }
    }
    if (notes != null) {
        notes.focus();
    }
});

$(".icon-calendar-weekend").click(function () {

    //var index = $(".weekend").index(this);
    //var item = $(".date-time-picker-weekend").get(index - 1);
    //item.focus();

    var index = $(".icon-calendar-weekend").index(this);
    var item = $(".icon-calendar-weekend").get(index);
    var doc = item.parentNode.parentNode;
    var notes = null;
    for (var i = 0; i < doc.childNodes.length; i++) {
        if (doc.childNodes[i].className == "form-control date-time-picker-weekend") {
            notes = doc.childNodes[i];
            break;
        }
    }
    if (notes != null) {
        notes.focus();
    }
});

$(".TypeNumberNotPoint").on("keypress", function (evt) {
    if (evt.which != 8 && evt.which != 0 && evt.which < 48 || evt.which > 57) {
        evt.preventDefault();
    }
});

$(".TypeNumberNotPoint").on("focusout", function (evt) {

    var index = $(".TypeNumberNotPoint").index(this);
    var item = $(".TypeNumberNotPoint").get(index);
    var partComma = item.value.split(',');
    if (item.value != "") {
        var n = "";
        for (var i = 0; i < item.value.split(',').length; i++) {
            n = n + partComma[i];
        }

        n = isNaN(n) ? 0 : (n > parseInt('2147483647') ? parseInt('2147483647') : n);

        var val = Math.round(Number(n) * 100) / 100;
        var parts = val.toString().split(".");
        var num = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, "");

        item.value = num;
    }
});

$(".TypeNumber").on("keypress", function (evt) {
    if (evt.which != 8 && evt.which != 0 && evt.which < 48 || evt.which > 57) {
        evt.preventDefault();
    }
});

$(".TypeNumber").on("focusout", function (evt) {

    var index = $(".TypeNumber").index(this);
    var item = $(".TypeNumber").get(index);
    var partComma = item.value.split(',');
    if (item.value != "") {
        var n = "";
        for (var i = 0; i < item.value.split(',').length; i++) {
            n = n + partComma[i];
        }

        n = isNaN(n) ? 0 : (n > parseInt('2147483647') ? parseInt('2147483647') : n);

        var val = Math.round(Number(n) * 100) / 100;
        var parts = val.toString().split(".");
        var num = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, "") + (parts[1] != null ? "." + ".00" : ".00");

        item.value = num;
    }
});

$(".TypeDecimal").on("keypress", function (evt) {
    var $field = $(this);
    var beforeVal = $field.val();
    if (beforeVal.indexOf('.') !== -1) {
        if (evt.which == 46) {
            evt.preventDefault();
        }
    }
    if (evt.which != 8 && evt.which != 0 && evt.which < 48 || evt.which > 57) {
        if (evt.which != 46) {
            evt.preventDefault();
        }
    }
});

$(".TypeDecimal").on("focusout", function (evt) {
    var index = $(".TypeDecimal").index(this);
    var item = $(".TypeDecimal").get(index);

    var partComma = item.value.split(',');
    var n = "";
    for (var i = 0; i < item.value.split(',').length; i++) {
        n = n + partComma[i];
    }
    n = isNaN(n) ? 0 : n;
    var val = Math.round(Number(n) * 100) / 100;
    var parts = val.toString().split(".");
    var num = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",") + (parts[1] != null ? "." + parts[1] : ".00");

    item.value = num;
});


$("label:contains('*')").each(function () {
    $(this).html($(this).html().replace("*", "<span class='required'>*</span>"));
});

$.fn.setValue = function (value) {

    var control = $(this)[0];

    switch (control.tagName) {
        case '_SELECT':
            if ($(this).find('option[value=' + value + ']').length === 0) {

                $('<option />', { value: value, text: value, "x-type": "tempdata" }).appendTo($(this));
            }
            $(this).val(value);

            break;

        default:
            $(this).val(value);
            break;
    }
};


var headerHeight = $("header").height();
// Select all links with hashes
$('a[href*="#"]')
    // Remove links that don't actually link to anything
    .not('[href="#"]')
    .not('[href="#0"]')
    .not('[data-toggle="tab"]')
    .click(function (event) {
        // On-page links
        if (
            location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') &&
            location.hostname == this.hostname
        ) {
            // Figure out element to scroll to
            var target = $(this.hash);
            target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
            // Does a scroll target exist?
            if (target.length) {
                // Only prevent default if animation is actually gonna happen
                event.preventDefault();
                $('html, body').animate({
                    scrollTop: target.offset().top - headerHeight - 20
                }, 500, function () { });
            }
        }
    });

$('input.btn-radio').click(function () {
    $(this).parent().addClass('checked').siblings('label').removeClass('checked');
});


$("form").submit(function () {
    $(':input').each(function () {
        if ($(this).attr('type') != 'file')
        $(this).val($.trim($(this).val()));
    });
    return true;
});

$("form").attr("autocomplete", "off");

$("form").on('load', function () {
/*$("form").load(function () {*/
    $('.TypeDecimal').each(function () {
        var index = $(".TypeDecimal").index(this);
        var item = $(".TypeDecimal").get(index);
        var partComma = item.value.split(',');
        var n = "";
        for (var i = 0; i < item.value.split(',').length; i++) {
            n = n + partComma[i];
        };
        var val = Math.round(Number(n) * 100) / 100;
        var parts = val.toString().split(".");
        var num = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",") + (parts[1] != null ? "." + parts[1] : ".00");

        item.value = num;
    });
    return true;
});