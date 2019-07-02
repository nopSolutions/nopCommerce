'use strict';

var Feeds = (function() {

    var FacebookModule = (function() {
        
        function init(appId, callback) {

            if (appId) {

                $.ajaxSetup({ cache: true });
                $.getScript('//connect.facebook.net/en_US/sdk.js', function() {

                    FB.init({
                        appId: appId,
                        version: 'v2.5' // or v2.0, v2.1, v2.2, v2.3
                    });

                    if (callback) {

                        callback();
                    }
                });
            }
        }

        return {
            init: init
        };

    })();

    var UtilsModule = (function() {
        
        function refreshParent() {

            window.opener.location.reload();
        }

        function popitup(url, windowName) {

            var newwindow = window.open(url, windowName, 'height=700,width=700');
            newwindow.onunload = refreshParent;

            if (window.focus) {

                newwindow.focus();
            }

            return false;
        }

        function disableAuthorizeOnChange(elementSelector, btnSelector, authorizedText, notAuthorizedText) {

            var elementsText = {};

            var btnOriginalClasses = ($(btnSelector).attr('class') || '').split(' ');

            $(elementSelector).each(function() {

                elementsText[$(this).val()] = true;
            });

            $(elementSelector).change(function(e) {

                var key = $(this).val();

                if (elementsText.hasOwnProperty(key)) elementsText[key] = true;
                else elementsText[e.currentTarget.defaultValue] = false;

                var shouldEnable = true;

                for (var prop in elementsText) {

                    shouldEnable &= elementsText[prop];
                }

                if (shouldEnable && $.inArray('not-authorized', btnOriginalClasses) < 0) {

                    $(btnSelector).prop('disabled', false).removeClass('not-authorized').addClass('already-authorized').text(authorizedText);
                }
                else {

                    $(btnSelector).prop('disabled', true).removeClass('already-authorized').addClass('not-authorized').text(notAuthorizedText);
                }
            });
        }

        return {
        
            popitup: popitup,
            disableAuthorizeOnChange: disableAuthorizeOnChange
        };

    })();

    return {

        Facebook: FacebookModule,
        Utils: UtilsModule
    };

})();