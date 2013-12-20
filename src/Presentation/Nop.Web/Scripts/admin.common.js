var currentModelId = "";
function closeModalWindow() {
    $('#' + currentModelId).data('tWindow').close();
}
function openModalWindow(modalId) {
    currentModelId = modalId;
    $('#' + modalId).data('tWindow').center().open();
}


function setLocation(url) {
    window.location.href = url;
}

function OpenWindow(query, w, h, scroll) {
    var l = (screen.width - w) / 2;
    var t = (screen.height - h) / 2;

    winprops = 'resizable=1, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l + 'w';
    if (scroll) winprops += ',scrollbars=1';
    var f = window.open(query, "_blank", winprops);
}

function showThrobber(message) {
    $('.throbber-header').html(message);
    window.setTimeout(function () {
        $(".throbber").show();
    }, 1000);
}

$(document).ready(function () {
    $('.multi-store-override-option').each(function (k, v) {
        checkOverridenStoreValue(v, $(v).attr('data-for-input-selector'));
    });
});

function checkAllOverridenStoreValue(item) {
    $('.multi-store-override-option').each(function (k, v) {
        $(v).attr('checked', item.checked);
        checkOverridenStoreValue(v, $(v).attr('data-for-input-selector'));
    });
}

function checkOverridenStoreValue(obj, selector) {
    var elementsArray = selector.split(",");
    if (!$(obj).is(':checked')) {
        $(selector).attr('disabled', true);
        //Telerik elements are enabled/disabled some other way
        $.each(elementsArray, function(key, value) {
            var telerikElement = $(value).data("tTextBox");
            if (telerikElement !== undefined && telerikElement !== null) {
                telerikElement.disable();
            }
        }); 
    }
    else {
        $(selector).removeAttr('disabled');
        //Telerik elements are enabled/disabled some other way
        $.each(elementsArray, function(key, value) {
            var telerikElement = $(value).data("tTextBox");
            if (telerikElement !== undefined && telerikElement !== null) {
                telerikElement.enable();
            }
        });
    };
}

function telerik_on_tab_select(e) {
    //we use this function to store selected tab index into HML input
    //this way we can persist selected tab between HTTP requests
    $("#selected-tab-index").val($(e.item).index());
}

function display_kendoui_grid_error(e) {
    if (e.errors) {
        if ((typeof e.errors) == 'string') {
            //single error
            alert('single error');
            //display the message
            alert(e.errors);
        } else {
            //array of errors
            //source: http://docs.kendoui.com/getting-started/using-kendo-with/aspnet-mvc/helpers/grid/faq#how-do-i-display-model-state-errors?
            var message = "The following errors have occurred:";
            //create a message containing all errors.
            $.each(e.errors, function (key, value) {
                if (value.errors) {
                    message += "\n";
                    message += value.errors.join("\n");
                }
            });
            //display the message
            alert(message);
        }
    } else {
        alert('Error happened');
    }
}


// Ajax activity indicator bound to ajax start/stop document events
$(document).ajaxStart(function () {
    $('#ajaxBusy').show();
}).ajaxStop(function () {
    $('#ajaxBusy').hide();
});