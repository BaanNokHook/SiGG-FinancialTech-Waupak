Date.prototype.addDays = function (days) {
    var date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return date;
};

$(function () {
    var input = $('.input-validation-error:first');
    if (input) {
        input.focus();
        input.click();
    }
});

function FormatDecimalValidate(Num, point) {
    var format = Number(parseFloat(Num).toFixed(point)).toLocaleString('en', {
        minimumFractionDigits: point
    });
    return format;
}

function OnKeyPress_NumberOnlyAndDot(obj) {
    var keyAble = false;
    try {
        key = window.event.keyCode;
        var beforeVal = obj.value;
        if (beforeVal.indexOf(".") !== -1) {
            if (key === 46) {
                keyAble = false;
            } else if (key > 47 && key < 58) {
                keyAble = true; // if so, do nothing
            }
            else {
                keyAble = false;
            }
        }
        else if ((key === 46) || (key > 47 && key < 58)) {
            keyAble = true; // if so, do nothing
        } else {
            keyAble = false;
        }
    } catch (err) {
        throw new Error(" error in sub text_OnKeyPress_NumberOnlyAndDot function() cause = " + err.message);
    }
    return keyAble;
}

function OnKeyPress_NumberOnlyAndDotAndM(obj) {
    var keyAble = false;
    try {
        key = window.event.keyCode;
        var val = obj.value;
        if (val.indexOf(".") !== -1) {
            if (key === 46) {
                keyAble = false;
            } else if (key > 47 && key < 58) {
                keyAble = true; // if so, do nothing
            }
            else if ((key === 77) || (key === 109)) {
                
                if (val.indexOf('m') !== -1) {
                    keyAble = false;
                } else if (val.indexOf('M') !== -1) {
                    keyAble = false;
                } else {
                    keyAble = true;
                }
            }
        }
        else if ((key === 46) || (key > 47 && key < 58)) {
            keyAble = true; // if so, do nothing
        } else if ((key === 77) || (key === 109)) {
            if (val.indexOf('m') !== -1) {
                keyAble = false;
            } else if (val.indexOf('M') !== -1) {
                keyAble = false;
            } else {
                keyAble = true;
            }
        } else {
            keyAble = false;
        }
    } catch (err) {
        throw new Error(" error in sub text_OnKeyPress_NumberOnlyAndDotAndM function() cause = " + err.message);
    }
    return keyAble;
}

function OnKeyPress_NumberOnlyAndDotAndMinus(obj) {

    var keyAble = false;
    try {
        key = window.event.keyCode;
        var beforeVal = obj.value;
        if (beforeVal.indexOf(".") !== -1) {
            if (key === 46) {
                keyAble = false;
            } else if (key > 47 && key < 58) {
                keyAble = true; // if so, do nothing
            }
            else if (key === 45) {
                if (!beforeVal.indexOf("-") !== -1 && obj.selectionStart === 0) {
                    keyAble = true;
                } else {
                    keyAble = false;
                }
            }
            else {
                keyAble = false;
            }
        }
        else if ((key === 46) || (key > 47 && key < 58)) {
            keyAble = true; // if so, do nothing
        } else if (key === 45) {
            if (!beforeVal.indexOf("-") !== -1 && obj.selectionStart === 0) {
                keyAble = true;
            } else {
                keyAble = false;
            }
        }
        else {
            keyAble = false;
        }
    } catch (err) {
        throw new Error(" error in sub text_OnKeyPress_NumberOnlyAndDotAndMinus function() cause = " + err.message);
    }
    return keyAble;
}

function OnChange_FormatNumber(name, point) {
    var text = $('input[name="' + name + '"]').val().replace(/,/g, '');
    if (text !== "") {
        $('input[name="' + name + '"]').val(FormatDecimalValidate(text, point));
    }
}