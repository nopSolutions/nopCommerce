/*
** nopCommerce ajax cart implementation
*/


var AjaxCart = {
    loadWaiting: false,
    usepopupnotifications: false,
    topcartselector: '',
    topwishlistselector: '',
    flyoutcartselector: '',
    localized_data: false,

    init: function (usepopupnotifications, topcartselector, topwishlistselector, flyoutcartselector, localized_data) {
        this.loadWaiting = false;
        this.usepopupnotifications = usepopupnotifications;
        this.topcartselector = topcartselector;
        this.topwishlistselector = topwishlistselector;
        this.flyoutcartselector = flyoutcartselector;
        this.localized_data = localized_data;
    },

    setLoadWaiting: function (display) {
        displayAjaxLoading(display);
        this.loadWaiting = display;
  },

    //move a shopping cart item to the custom wishlist
    moveToCustomWishlist: function (urlmove, itemId, wishlistId) {
        if (this.loadWaiting !== false) {
            return;
        }
        this.setLoadWaiting(true);

        var postData = {
          shoppingCartItemId: itemId,
          customWishlistId: wishlistId
        };
        addAntiForgeryToken(postData);

        this.send_ajax(urlmove, postData);
    },

    //create custom wishlist
    createCustomWishlist: function (urlcreate, wishlistName, productId) {
      if (this.loadWaiting !== false) {
        return;
      }
      this.setLoadWaiting(true);

      var postData = {
        name: wishlistName,
        productId: productId
      };
      addAntiForgeryToken(postData);

      this.send_ajax(urlcreate, postData);
    },

    //delete custom wishlist
    deleteCustomWishlist: function (urldelete) {
        if (this.loadWaiting !== false) {
          return;
        }
        this.setLoadWaiting(true);

        var postData = {};
        addAntiForgeryToken(postData);

        this.send_ajax(urldelete, postData);
    },

    //move a product to the custom wishlist from the default wishlist
    moveproducttowishlist: function (urlmove, wishlistid) {
        if (this.loadWaiting !== false) {
            return;
        }
        this.setLoadWaiting(true);
        var postData = {
            wishlistId: wishlistid 
        };
        addAntiForgeryToken(postData);

        this.send_ajax(urlmove, postData);
    },

    //add a product to the cart/wishlist from the catalog pages
    addproducttocart_catalog: function (urladd) {
        if (this.loadWaiting !== false) {
            return;
        }
        this.setLoadWaiting(true);

        var postData = {};
        addAntiForgeryToken(postData);

        this.send_ajax(urladd, postData);
    },

    //add a product to the cart/wishlist from the product details page
    addproducttocart_details: function (urladd, formselector) {
        if (this.loadWaiting !== false) {
            return;
        }
        this.setLoadWaiting(true);

        this.send_ajax(urladd, $(formselector).serialize());
    },

    //add a product to compare list
    addproducttocomparelist: function (urladd) {
        if (this.loadWaiting !== false) {
            return;
        }
        this.setLoadWaiting(true);

        var postData = {};
        addAntiForgeryToken(postData);

        this.send_ajax(urladd, postData);
    },

    send_ajax: function (requestUrl, postData) {
      $.ajax({
        cache: false,
        url: requestUrl,
        type: "POST",
        data: postData,
        success: this.success_process,
        complete: this.resetLoadWaiting,
        error: this.ajaxFailure
      });
    },

    success_process: function (response) {
        if (response.updatetopcartsectionhtml) {
            $(AjaxCart.topcartselector).html(response.updatetopcartsectionhtml);
        }
        if (response.updatetopwishlistsectionhtml) {
            $(AjaxCart.topwishlistselector).html(response.updatetopwishlistsectionhtml);
        }
        if (response.updateflyoutcartsectionhtml) {
            $(AjaxCart.flyoutcartselector).replaceWith(response.updateflyoutcartsectionhtml);
        }
        if (response.message) {
            //display notification
            if (response.success === true) {
                //success
                if (AjaxCart.usepopupnotifications === true) {
                    displayPopupNotification(response.message, 'success', true);
                }
                else {
                    //specify timeout for success messages
                    displayBarNotification(response.message, 'success', 3500);
                }
            }
            else {
                //error
                if (AjaxCart.usepopupnotifications === true) {
                    displayPopupNotification(response.message, 'error', true);
                }
                else {
                    //no timeout for errors
                    displayBarNotification(response.message, 'error', 0);
                }
            }
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
      alert(this.localized_data.AjaxCartFailure);
    }
};