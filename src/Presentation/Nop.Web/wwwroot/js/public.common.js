/*
** nopCommerce custom js functions
*/



function OpenWindow(query, w, h, scroll) {
    var l = (screen.width - w) / 2;
    var t = (screen.height - h) / 2;

    winprops = 'resizable=0, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l + 'w';
    if (scroll) winprops += ',scrollbars=1';
    var f = window.open(query, "_blank", winprops);
}

function setLocation(url) {
    window.location.href = url;
}

function displayAjaxLoading(display) {
    if (display) {
        $('.ajax-loading-block-window').show();
    }
    else {
        $('.ajax-loading-block-window').hide('slow');
    }
}

function displayPopupNotification(message, messagetype, modal) {
    //types: success, error, warning
    var container;
    if (messagetype == 'success') {
        //success
        container = $('#dialog-notifications-success');
    }
    else if (messagetype == 'error') {
        //error
        container = $('#dialog-notifications-error');
    }
    else if (messagetype == 'warning') {
        //warning
        container = $('#dialog-notifications-warning');
    }
    else {
        //other
        container = $('#dialog-notifications-success');
    }

    //we do not encode displayed message
    var htmlcode = '';
    if ((typeof message) == 'string') {
        htmlcode = '<p>' + message + '</p>';
    } else {
        for (var i = 0; i < message.length; i++) {
            htmlcode = htmlcode + '<p>' + message[i] + '</p>';
        }
    }

    container.html(htmlcode);

    var isModal = (modal ? true : false);
    container.dialog({
        modal: isModal,
        width: 350
    });
}
function displayJoinedPopupNotifications(notes) {
    if (Object.keys(notes).length === 0) return;

    var container = $('#dialog-notifications-success');
    var htmlcode = document.createElement('div');

    for (var note in notes) {
        if (notes.hasOwnProperty(note)) {
            var messages = notes[note];

            for (var i = 0; i < messages.length; ++i) {
                var elem = document.createElement("div");
                elem.innerHTML = messages[i];
                elem.classList.add('popup-notification');
                elem.classList.add(note);

                htmlcode.append(elem);
            }
        }
    }

    container.html(htmlcode);
    container.dialog({
        width: 350,
        modal: true
    });
}
function displayPopupContentFromUrl(url, title, modal, width) {
    var isModal = (modal ? true : false);
    var targetWidth = (width ? width : 550);
    var maxHeight = $(window).height() - 20;

    $('<div></div>').load(url)
        .dialog({
            modal: isModal,
            width: targetWidth,
            maxHeight: maxHeight,
            title: title,
            close: function(event, ui) {
                $(this).dialog('destroy').remove();
            }
        });
}

function displayBarNotification(message, messagetype, timeout) {
    var notificationTimeout;

    var messages = typeof message === 'string' ? [message] : message;
    if (messages.length === 0)
        return;

    //types: success, error, warning
    var cssclass = ['success', 'error', 'warning'].indexOf(messagetype) !== -1 ? messagetype : 'success';

    //remove previous CSS classes and notifications
    $('#bar-notification')
      .removeClass('success')
      .removeClass('error')
      .removeClass('warning');
    $('.bar-notification').remove();

    //add new notifications
    var htmlcode = document.createElement('div');

    //IE11 Does not support miltiple parameters for the add() & remove() methods
    htmlcode.classList.add('bar-notification', cssclass);
    htmlcode.classList.add(cssclass);

    //add close button for notification
    var close = document.createElement('span');
    close.classList.add('close');
    close.setAttribute('title', document.getElementById('bar-notification').dataset.close);

    for (var i = 0; i < messages.length; i++) {
        var content = document.createElement('p');
        content.classList.add('content');
        content.innerHTML = messages[i];

      htmlcode.appendChild(content);
    }
    
    htmlcode.appendChild(close);

    $('#bar-notification')
      .append(htmlcode);

    $(htmlcode)
        .fadeIn('slow')
        .on('mouseenter', function() {
            clearTimeout(notificationTimeout);
        });

    //callback for notification removing
    var removeNoteItem = function () {
        $(htmlcode).remove();
    };

    $(close).on('click', function () {
        $(htmlcode).fadeOut('slow', removeNoteItem);
    });

    //timeout (if set)
    if (timeout > 0) {
        notificationTimeout = setTimeout(function () {
            $(htmlcode).fadeOut('slow', removeNoteItem);
        }, timeout);
    }
}

function htmlEncode(value) {
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}


// CSRF (XSRF) security
function addAntiForgeryToken(data) {
    //if the object is undefined, create a new one.
    if (!data) {
        data = {};
    }
    //add token
    var tokenInput = $('input[name=__RequestVerificationToken]');
    if (tokenInput.length) {
        data.__RequestVerificationToken = tokenInput.val();
    }
    return data;
};

//customization
function changeWishlistStatus(productId, messagetype) {

    console.log(messagetype);

    if (messagetype === 'sendInterest') {
        var itemSelector = $('#product-interest-' + productId).children();

        console.log(itemSelector);

        itemSelector.first().removeClass('icon-color');
        itemSelector.first().addClass('icon-selected-color');
        itemSelector.last().text('Interest Sent');

        itemSelector.last().removeAttr("href");
        itemSelector.last().removeAttr("onclick");
        return;
    }

    var productSelector = $('#product-' + productId).children();

    if (messagetype === 'addToWishList') {

        console.log(productSelector);

        productSelector.first().removeClass('icon-color');
        productSelector.first().addClass('icon-selected-color');
        productSelector.last().text('Shortlisted');
    }
    else {
        //other
        console.log('Other Any Message Type:' + messagetype);
        productSelector.first().removeClass('icon-selected-color');
        productSelector.first().addClass('icon-color');
        productSelector.last().text('Shortlist');
    }
}

function displayPopupNotificationwithhtml(htmlcode) {

    var windowOptions = {
        actions: ["Close"],
        //width: "500px",
        //minWidth: "200px",
        //size: "auto",
        //position: {
        //    top: 100, // or "100px"
        //    //left: "20%"
        //},
        height: "100px",
        width: "20%",
        title: "View Mobile Number",
        visible: false,
        modal: true
    };

    $("#kendoWindow").kendoWindow(windowOptions);
    $("#kendoWindow").html(htmlcode);
    $("#kendoWindow").data("kendoWindow").center().open();

    //$('<div id="divSubscribePopup"></div>').html(htmlcode)
    //    .dialog({
    //        modal: true,
    //        title: 'View Mobile Number',
    //        position: { my: 'top', at: 'top-150' },
    //        close: function (event, ui) {
    //            $(this).dialog('destroy').remove();
    //        }
    //    });
}

//customization
function displayPopupWithHtml(htmlcode) {
    customerTypePopUp = $('#customerTypePopUp');
    customerTypePopUp.dialog({
        modal: true,
        width: 350,
        position: { my: "center top+100", at: "center top+100", of: window }
    });
}

//customization
function updateCustomerProfileType(profileTypeId, url) {

    console.log('updateCustomerProfileType method calling');

    $.ajax({
        url: url,
        type: 'GET',
        data: {
            'customerProfileTypeId': profileTypeId
        },
        dataType: 'json',
        success: function (data) {
            if (data) {
                window.location.reload(true)
            }
        },
        error: function (request, error) {
            console.log(error);
        }
    });

}