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

function getE(name) {
    //Obsolete since nopCommerce 2.60. But still here for backwards compatibility (in case of some plugin developers used it in their plugins or customized solutions)
    if (document.getElementById)
        var elem = document.getElementById(name);
    else if (document.all)
        var elem = document.all[name];
    else if (document.layers)
        var elem = document.layers[name];
    return elem;
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



$(document).ready(function () {
    //Setup the ajax indicator
    //TODO We'll not be able to load ajax-loading.gif when site is run in virtual directory. Fix it by creating CSS style
    $('body').append('<div id="ajaxBusy"><p><img src="/administration/content/images/ajax-loading.gif"></p></div>');
    $('#ajaxBusy').css({
        display: "none",
        margin: "0px",
        paddingLeft: "0px",
        paddingRight: "0px",
        paddingTop: "0px",
        paddingBottom: "0px",
        position: "absolute",
        right: "3px",
        top: "3px",
        width: "auto"
    });
});
// Ajax activity indicator bound to ajax start/stop document events
$(document).ajaxStart(function () {
    $('#ajaxBusy').show();
}).ajaxStop(function () {
    $('#ajaxBusy').hide();
});