/*
** nopCommerce custom js functions for mobile devices
*/


function displayAjaxLoading(display) {
    if (display) {
        $.mobile.showPageLoadingMsg();
    }
    else {
        $.mobile.hidePageLoadingMsg();
    }
}

function displayStandardAlertNotification(message) {
    var alertText = '';
    if ((typeof message) == 'string') {
        alertText = message;
    } else {
        for (var i = 0; i < message.length; i++) {
            alertText = alertText + message[i];
            if (i != message.length - 1) {
                alertText = alertText + '\r\n';
            }
        }
    }

    alert(alertText);
}