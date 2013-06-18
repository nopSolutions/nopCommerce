/*
** nopCommerce custom js functions for mobile devices
*/


function setLocation(url) {
    window.location.href = url;
}

function displayStandardAlertNotification(message) {
    var alertText = '';
    if ((typeof message) == 'string') {
        alertText = message;
    } else {
        for (var i = 0; i < message.length; i++) {
            alertText = alertText + '\\n' + message[i];
        }
    }

    alert(alertText);
}