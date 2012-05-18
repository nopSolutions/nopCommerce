/*
** nopCommerce ajax cart implementation
*/


var AjaxCart = {
    loadWaiting: false,
    urladd: '',
    topcartselector: '',
    flyoutcartselector: '',

    init: function (urladd, topcartselector, flyoutcartselector) {
        this.loadWaiting = false;
        this.urladd = urladd;
        this.topcartselector = topcartselector;
        this.flyoutcartselector = flyoutcartselector;
    },

    setLoadWaiting: function (display) {
        displayAjaxLoading(display);
        this.loadWaiting = display;
    },

    addtocart: function (productId) {
        if (this.loadWaiting != false) {
            alert('return');
            return;
        }
        this.setLoadWaiting(true);

        $.ajax({
            url: this.urladd,
            data: { "productId": productId },
            type: 'post',
            success: this.successprocess,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        });
    },

    successprocess: function (response) {
        if (response.updatetopcartsectionhtml) {
            $(AjaxCart.topcartselector).html(response.updatetopcartsectionhtml);
        }
        if (response.updateflyoutcartsectionhtml) {
            $(AjaxCart.flyoutcartselector).replaceWith(response.updateflyoutcartsectionhtml);
        } 
        if (response.message) {
            var messageType;
            if (response.success == true) {
                messageType = 'success';
            }
            else {
                messageType = 'error';
            }

            displayNotification(response.message, messageType, true);

            return false;
        }
        if (response.redirect) {
            location.href = response.redirect;
            return true;
        }
        return false;
    },

    resetLoadWaiting: function () {
        AjaxCart.setLoadWaiting(false);
    },

    ajaxFailure: function () {
        alert('Failed to add the product to the cart. Please refresh the page and try one more time.');
    }
};