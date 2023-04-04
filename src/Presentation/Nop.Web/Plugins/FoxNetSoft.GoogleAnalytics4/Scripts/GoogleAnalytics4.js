var GoogleGA4Helper = {
    enableDebug: false,
    currency: 'USD',
    Init: function (cc, debug) {
        this.currency = cc;
        this.enableDebug = debug;
    },

    _getProductId: function (object) {
        var productid = $(object).data("productid");
        if (productid > 0) {
            return productid;
        }
        var script = $(object).attr("onclick");
        if (script && script.length > 0) {
            //productid = parseInt(script.replace(/[^0-9]/g, ''));
            var numn = script.match(/\d+/g);
            if (numn !== null) {
                productid = numn.map(Number)[0];
                if (productid > 0) {
                    return productid;
                }
            }
        }
        return 0;
    },
    Initialization: function () {
        GoogleGA4Helper.ReloadEvents();

        $(document).bind("nopAjaxCartProductAddedToCartEvent", function (e) {
            GoogleGA4Helper.AddToCart('add_to_cart', e.productId, e.quantity);
            e.stopPropagation();
        });
    },
    CheckExistEvent: function (obj) {
        return $(obj).data('googleanalytics4') == 1;
        
        /*
        try {
            var events = $._data(obj.get(0), 'events').click;
            if (typeof events === "undefined") {
                return false;
            }
            return true;
        }
        catch (err) {
            return false;
        }*/
    },
    ReloadEvents: function () {
        //productClick
        $('.product-item[data-productid]').each(function () {
            var productid = $(this).data("productid");
            if (productid > 0) {
                if (GoogleGA4Helper.CheckExistEvent($(this).find('a'))) {
                    return;
                }
                $(this).find('a').data('googleanalytics4', 1);
                $(this).find('a').click({ productid: productid }, function (event) {
                    event.preventDefault();
                    var productid = event.data.productid;
                    var url = $(this).attr('href');
                    //var product = GA4_productImpressions.find(x => x.productId === productid);
                    var result = $.grep(GA4_productImpressions, function (e) { return e.productId == productid; });
                    if (result.length > 0) {
                        GoogleGA4Helper.ProductClick([result[0]], url);
                        return;
                    }
                    window.location.href = url;
                });
            }
        });
        //addToCart
        $('.add-to-cart-button, .product-box-add-to-cart-button').each(function () {
            var productid = GoogleGA4Helper._getProductId(this);
            if (productid > 0) {
                if (GoogleGA4Helper.CheckExistEvent($(this))) {
                    return;
                }
                $(this).data('googleanalytics4', 1);
                $(this).click({ productid: productid }, function (event) {
                    event.preventDefault();
                    GoogleGA4Helper.AddToCart('add_to_cart', event.data.productid);
                });
            }
        });
        //addToWishlist
        $('.add-to-wishlist-button').each(function () {
            var productid = GoogleGA4Helper._getProductId(this);
            if (productid > 0) {
                if (GoogleGA4Helper.CheckExistEvent($(this))) {
                    return;
                }
                $(this).data('googleanalytics4', 1);
                $(this).click({ productid: productid }, function (event) {
                    event.preventDefault();
                    GoogleGA4Helper.AddToCart('add_to_wishlist', event.data.productid);
                });
            }
        });
        //add-to-compare-list-button
        /*$('.add-to-compare-list-button').each(function () {
            var productid = GoogleGA4Helper._getProductId(this);
            if (productid > 0) {
                if (GoogleGA4Helper.CheckExistEvent($(this))) {
                    return;
                }
                $(this).data('googleanalytics4', 1);
                $(this).click({ productid: productid }, function (event) {
                    event.preventDefault();
                    GoogleGA4Helper.AddToCart('ga4_addToCompare', event.data.productid);
                });
            }
        });
        */
        //remove from the cart/wishlist
        $('input[name="updatecart"]').click(function () {
            $('input[name="removefromcart"]:checked').each(function () {
                var cartitemid = $(this).val();
                if (cartitemid > 0) {
                    GoogleGA4Helper.RemoveFromCart(cartitemid);
                }
            });
        });

        //copy from wishlist to cart
        $('input[name="addtocartbutton"]').click(function () {
            $('input[name="addtocart"]:checked').each(function () {
                var cartitemid = $(this).val();
                if (cartitemid > 0) {
                    var products = $.grep(GA4_wishlistproducts, function (e) { return e.cartItemId === cartitemid; });
                    if (products.length > 0) {
                        var quantity = $('input[name="itemquantity' + cartitemid + '"]').val();
                        GoogleGA4Helper.AddToCart('add_to_cart', products[0].productId, quantity);
                    }
                }
            });
        });

    },
    ProductClick: function (products, url) {
        this.WriteConsole({ 'ProductClick': products });
        dataLayer.push({
            'event': 'select_item',
            'ecommerce': {
                'item_list_name': products[0].list,
                'items': products
            },
            'eventCallback': function () {
                document.location = url;
            },
            'eventTimeout': 2000
        });
        /* if customer uses blocker*/
        if (typeof google_tag_manager == "undefined") {
            document.location = url;
        }
    },
    PurchaseForADblock: function (orderid) {
        //window.addEventListener('load', function () {
            //customer blocks Tag Manager
        if (!window.google_tag_manager) {
                var img = document.createElement('img');
                img.setAttribute('style', 'display:none;');
                img.src = '/GoogleAnalytics4/Purchase?orderid=' + orderid;
                document.body.appendChild(img);  
            };
        //}, false);
    },
    AddToCart: function (eventname, productid, quantity) {
        var products = $.grep(GA4_productDetails, function (e) { return e.productId == productid; });
        if (products.length == 0) {
            products = $.grep(GA4_productImpressions, function (e) { return e.productId == productid; });
        }
        if (products.length == 0) {
            products = $.grep(GA4_wishlistproducts, function (e) { return e.productId == productid; });
        }
        if (products.length > 0) {
            if (quantity == undefined)
                quantity = $('#product_enteredQuantity_' + productid).val();
            if (quantity == undefined)
                quantity = 1;
            if (eventname == undefined)
                eventname = 'addToCart';
            products[0].quantity = quantity;
            this.WriteConsole({ 'AddToCart': [eventname, products[0]] });

            dataLayer.push({
                'event': eventname,
                'ecommerce': {
                    'currency': this.currency,
                    'value': quantity,
                    'items': products
                }
            });
        }
    },
    RemoveFromCart: function (cartitemid, quantity) {
        var eventName = 'removeFromCart';
        var products = $.grep(GA4_cartproducts, function (e) { return e.cartItemId == cartitemid; });
        if (products.length == 0) {
            eventName = 'removeFromWishList';
            products = $.grep(GA4_wishlistproducts, function (e) { return e.cartItemId == cartitemid; });
        }
        if (products.length > 0) {
            if (quantity == undefined)
                quantity = $('input[name="itemquantity' + cartitemid + '"]').val();
            if (quantity == undefined)
                quantity = 1;
            products[0].quantity = quantity;
            this.WriteConsole({ 'RemoveFromCart': products });
            dataLayer.push({
                'event': eventName,
                'ecommerce': {
                    'currency': this.currency,
                    'value': quantity,
                    'items': products
                }
            });
        }
    },
    WriteConsole: function (obj) {
        if (this.enableDebug) {
            str = JSON.stringify(obj);
            //str = JSON.stringify(obj, null, 4); // (Optional) beautiful indented output.
            console.log(str); // Logs output to dev tools console.
        }
    }
};
