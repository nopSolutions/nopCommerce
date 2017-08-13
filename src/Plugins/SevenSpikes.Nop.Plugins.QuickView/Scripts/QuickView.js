(function ($, ss) {

    var quickViewButtonClassName = "quick-view-button";
    var quickViewButtonClass = "." + quickViewButtonClassName;

    function closeQuickViewWindow() {
        var kwindow = $(".quickViewWindow").data("kendoWindow");

        if (kwindow == undefined || kwindow.content() === "") {
            return;
        }

        kwindow.close();
    }

    function showQuickViewWindow() {
        var kwindow = $(".quickViewWindow").data("kendoWindow");
        var ajaxBusyElement = $("<div class='k-loading-mask'><div class='k-loading-image'/><div class='k-loading-color'/></div>").width("700px").height("360px");
        kwindow.content(ajaxBusyElement);
        kwindow.center();
        kwindow.open();
    }

    function showQuickView(quickViewWindowData) {

        var kwindow = $(".quickViewWindow").data("kendoWindow");
        kwindow.content(quickViewWindowData);
        //$(".quickViewWindow .product-essential").css('opacity', "0").animate({ opacity: 1 }, {duration: 800});
    }

    function adjustPictureOnProductAttributeValueChange() {
        var quickViewAdjustPicture = $('.quickViewAdjustPictureOnProductAttributeValueChange');

        if (quickViewAdjustPicture.length > 0) {
            $(document).on("quickView.product_attributes_changed", function (e) {

                if (e.changedData && e.changedData.pictureDefaultSizeUrl) {
                    var pictureDefaultSizeUrl = e.changedData.pictureDefaultSizeUrl;

                    if (quickViewAdjustPicture.attr('data-isCloudZoomAvailable') === 'true') {
                        pictureDefaultSizeUrl = pictureDefaultSizeUrl.substring(1, pictureDefaultSizeUrl.length - 9);

                        $('.quickViewWindow .cloudzoom-gallery[href*="' + pictureDefaultSizeUrl + '"]').click();
                    } else {
                        $('.quickViewWindow .gallery .picture img').attr('src', pictureDefaultSizeUrl);
                    }
                }
            });
        }
    }

    function initRentalInfo() {
        var quickViewRentalInfo = $('.quickViewRentalInfo');

        if (quickViewRentalInfo.length > 0) {
            var datePickerFormat = quickViewRentalInfo.attr('data-datePickerFormat');
            var startDateControlId = '#' + quickViewRentalInfo.attr('data-startDateControlId');
            var endDateControlId = '#' + quickViewRentalInfo.attr('data-endDateControlId');

            $(startDateControlId).datepicker({ dateFormat: datePickerFormat });
            $(endDateControlId).datepicker({ dateFormat: datePickerFormat });
        }
    }

    //In the quick view form the product attributes are named quickView_product_attribute instead of product_attribute,
    //so the add to cart functionality was not working when the AjaxCart plugin is disabled.
    $(document).on('nopQuickViewDataShownEvent', function () {
        var productId = $('#add-to-cart-details').attr('data-productid');
        var addToCartButtonSelector = '.quickViewWindow #add-to-cart-button-' + productId;

        //In the case when the AjaxCart is Enabled it is not necessary to do anything.
        if ($(addToCartButtonSelector).hasClass('nopAjaxCartProductVariantAddToCartButton')) {
            return;
        }

        $(addToCartButtonSelector).prop('onclick', null).off('click');

        $(addToCartButtonSelector).on('click', function () {
            var addToCartUrl = $('#add-to-cart-details').attr('data-url');

            var formSerialization = $('.quickViewWindow #product-details-form').serialize();
            formSerialization = formSerialization.replace(new RegExp('quickView_product_attribute', 'g'), 'product_attribute');

            $.ajax({
                cache: false,
                url: addToCartUrl,
                data: formSerialization,
                type: 'post',
                success: AjaxCart.success_process,
                complete: AjaxCart.resetLoadWaiting,
                error: AjaxCart.ajaxFailure
            });
        });
    });

    function retrieveQuickViewData(productId) {

        var quickViewDataUrl = $(".quickViewData").attr("data-retrieveQuickViewUrl");

        if (quickViewDataUrl == undefined) {
            return;
        }

        var getQuickViewDataParameters = { "productID": productId };

        $.ajax({
            beforeSend: function () {
                showQuickViewWindow();
            },
            cache: false,
            type: "POST",
            data: $.toJSON(getQuickViewDataParameters),
            contentType: "application/json; charset=utf-8",
            url: quickViewDataUrl
        }).done(function (data) {

            showQuickView(data);

            var icons = {
                header: "ui-icon-circle-arrow-e",
                activeHeader: "ui-icon-circle-arrow-s"
            };

            var panelsHeightStyle = $(".quickViewData").attr("data-accordionpanelsheightstyle");

            // Call the jQuery-UI accordion in the Quick View window
            $("#accordion").accordion({
                collapsible: true,
                icons: icons,
                heightStyle: panelsHeightStyle
            });

            var qvOffset = $(".quickView").offset();
            var qvHeight = $(".quickView").height();

            if ($(window).height() < qvHeight) {
                $(".quickView").css("top", $(document).scrollTop() + 10);
            }
            else {
                $(".quickView").css("top", (qvOffset.top - (qvHeight / 4)));
            }

            adjustPictureOnProductAttributeValueChange();
            initRentalInfo();

            $.event.trigger({ type: "nopQuickViewDataShownEvent", productId: productId });

        }).fail(function () {
            alert("Loading the page failed.");
            closeQuickViewWindow();
        });
    }

    function addQuickViewButtons(productSelector, quickViewParentSelector) {
        var quickViewButtonText = $(".quickViewData").attr("data-quickViewButtonText");
        var quickViewButtonTitle = $(".quickViewData").attr("data-quickViewButtonTitle");

        if (quickViewButtonText == undefined) {
            quickViewButtonText = "Quick View";
        }

        var quickViewParent = $(quickViewParentSelector);

        $(quickViewParent).each(function () {
            //var parentPositionProperty = $(this).css('position');
            //if (parentPositionProperty === 'static' || parentPositionProperty === 'initial') {
            //    $(this).css("position", "relative");
            //}
            if ($(this).find(quickViewButtonClass).length <= 0) {
                $(this).prepend('<div class="' + quickViewButtonClassName + '"><a title="' + quickViewButtonTitle + '">' + quickViewButtonText + '</a></div>');
            }
        });

        $(quickViewButtonClass).off('click.quickview').on('click.quickview', function () {
            var productId = $(this).parents(productSelector).first().attr("data-productid");
            if (typeof productId == 'undefined') {
                return;
            }
            retrieveQuickViewData(productId);
        });

        $.event.trigger({ type: "nopQuickViewButtonsAddedCompleteEvent" });
    }

    function getIsQuickViewPopupDraggable() {
        var isQuickViewPopupDraggable = $(".quickViewData").attr("data-isQuickViewPopupDraggable");

        if (isQuickViewPopupDraggable != undefined) {
            if (isQuickViewPopupDraggable === "True") {
                return true;
            }
        }

        return false;
    }

    function getIsQuickViewPopupOverlayEnabled() {
        var isQuickViewPopupDraggable = $(".quickViewData").attr("data-enableQuickViewPopupOverlay");

        if (isQuickViewPopupDraggable != undefined) {
            if (isQuickViewPopupDraggable === "True") {
                return true;
            }
        }

        return false;
    }

    function addQuickViewWindowDialog() {
        var bodySelector = $("body");

        if (bodySelector.length > 0) {
            var quickViewModalDialog = "<div class=\"quickViewWindow\"></div>";
            bodySelector.prepend(quickViewModalDialog);

            var isQuickViewWindowDraggable = getIsQuickViewPopupDraggable();
            var isQuickViewWindowOverlayEnabled = getIsQuickViewPopupOverlayEnabled();

            var winObject = $(".quickViewWindow").kendoWindow({
                minHeight: "330px",
                minWidth: "660px",
                draggable: isQuickViewWindowDraggable,
                resizable: false,
                modal: isQuickViewWindowOverlayEnabled,
                actions: ["Close"],
                animation: false,
                visible: false,
                close: function () {
                    $(".quickViewWindow").html($("").html());
                }
            }).data("kendoWindow");
            winObject.wrapper.addClass("quickView");

            $(document).on("click", ".k-overlay", function () {
                winObject.close();
            });
        }
    }

    function getProductSelector() {
        var quickViewDataUrl = $(".quickViewData").attr("data-productselector");

        if (quickViewDataUrl != undefined) {
            return quickViewDataUrl;
        }

        return "";
    }

    function getQuickViewButtonParentSelector(productSelector) {
        var buttonParent = $(".quickViewData").attr("data-productselectorchild");

        if (typeof buttonParent != 'undefined' && buttonParent !== productSelector && $(productSelector + ' ' + buttonParent).length > 0) {
            return productSelector + ' ' + buttonParent;
        }

        return productSelector;
    }

    $(document).ready(function () {
        if (ss.isMobile() || (!ss.isMobile() && ss.getViewPort().width <= 768)) {
            return; // we do not want QuickView on mobile devices or on small screens
        }

        var productSelector = getProductSelector();
        if (productSelector === '') {
            return;
        }

        var quickViewButtonParentSelector = getQuickViewButtonParentSelector(productSelector);

        addQuickViewButtons(productSelector, quickViewButtonParentSelector);

        $(document).on("nopAjaxCartProductAddedToCartEvent nopAjaxCartProductAddedToWishlistEvent", function () {
            closeQuickViewWindow();
        });

        $(document).on("nopAjaxFiltersFiltrationCompleteEvent newProductsAddedToPageEvent", function () {
            quickViewButtonParentSelector = getQuickViewButtonParentSelector(productSelector);
            addQuickViewButtons(productSelector, quickViewButtonParentSelector);
        });

        addQuickViewWindowDialog();
    });

})(jQuery, sevenSpikes);