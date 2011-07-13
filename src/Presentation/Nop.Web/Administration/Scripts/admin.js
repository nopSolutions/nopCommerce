(function ($) {
    $.fn.nopBlock = function (message) {
        this.each(function () {
            $(this).block({ overlayCSS: {
                backgroundColor: '#fff'
            },
                css: {
                    border: 'none',
                    padding: '15px',
                    backgroundColor: '#fff',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: 1,
                    color: '#009FFF'
                },
                message: '<h1>' + message + '</h1>'
            });
        });
    };
})(jQuery);

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

$(document).ready(function () {
    //Setup the ajax indicator
    //TODO We'll not be able to load ajax-loading.gif when site is run in virtual directory. Fix it 
    //1. Move to .cshtml file and use @Url.Content there)
    //2. Or create css style
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