/*
** nopCommerce ajax cart implementation
*/


var AjaxCart = {
    loadWaiting: false,
    urladd: '',
    minicartselector: '',
    productcountselector: '',

    init: function (urladd, minicartselector, productcountselector) {
        this.loadWaiting = false;
        this.urladd = urladd;
        this.minicartselector = minicartselector;
        this.productcountselector = productcountselector;
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
        if (response.updateitemscountsectionhtml) {
            $(AjaxCart.productcountselector).html(response.updateitemscountsectionhtml);
        }
        if (response.updateminicartsectionhtml) {
            $(AjaxCart.minicartselector).html(response.updateminicartsectionhtml);
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