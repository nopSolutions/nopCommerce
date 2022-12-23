(function ($, ssCore, ssEx) {

    window.themeSettings = {
        themeBreakpoint: 1024,
        isAccordionMenu: false
    }

    // Switch device master class name between desktop and mobile devices

    function recognizeMobileDevices() {
        if (ssCore.isMobileDevice) {
            $('body').addClass('mobile-device');
        }
        else {
            $('body').addClass('desktop-device');
        }
    }

    /* Sets the top offset of header links and selectors, search dropdown, and flyout cart.
       Should be invoked on load, on scroll, and on resize. */

    function setHeaderNavigationDropdownsPosition() {
        var responsiveNavWrapper = $('.responsive-nav-wrapper');
        var desktopNavWrapper = $('.desktop-nav-wrapper');
		var headerStoreTheme = $('.header-storetheme');
		var adminHeaderBar = $('.admin-header-links');
		
		var topOffsetMobile = $('.master-header-wrapper').outerHeight();
		
		if (adminHeaderBar.length > 0) {
			topOffsetMobile = topOffsetMobile + adminHeaderBar.outerHeight();
		}
		if (headerStoreTheme.length > 0)
		{
        	topOffsetMobile = topOffsetMobile + headerStoreTheme.outerHeight();
		}
		if (responsiveNavWrapper.hasClass('stick')) {
            topOffsetMobile = responsiveNavWrapper.outerHeight();
        }
		
        var topOffsetDesktop = desktopNavWrapper.offset().top - $(window).scrollTop();

        if (ssCore.getViewPort().width <= window.themeSettings.themeBreakpoint) {
            $('.store-search-box, .header-options').css('top', topOffsetMobile);
        }
        else {
            $('.store-search-box').css('top', topOffsetDesktop + desktopNavWrapper.outerHeight());
        }
    }

    // recalculate the top offset depending on whether the navigation has sticked or not
    $(document).on("navigationHasSticked", function () {
        setHeaderNavigationDropdownsPosition(false);
    });

    function handleHeaderNavigationDropdownsPosition() {
        $('.search-wrap').on('click', function () {
            setHeaderNavigationDropdownsPosition();
        });
        $('.personal-button').on('click', function () {
            setHeaderNavigationDropdownsPosition();
        });
        $(window).on('resize', setHeaderNavigationDropdownsPosition);
    }

    /* Handles the toggle states (open/close) of header links and selectors, search dropdown, and flyout cart +
       their interactions with the mobile menu and the screen overlay element, and with the page scroll. */

    function handleHeaderNavigationDropdownsToggle() {
        var menuButton = $('.menu-title');
        var searchButton = $('.search-wrap');
        var accountButton = $('.personal-button');
        var cartButton = $('.shopping-cart-link');

        var menuDropdown = $('.header-menu');
        var searchDropdown = $('.store-search-box');
        var optionsDropdown = $('.header-options');
        var overlay = $('.overlayOffCanvas');

        menuButton.on('click', function () {
            overlay.css('z-index', 1060);
            searchDropdown.removeClass('active');
            optionsDropdown.removeClass('active');
            $('.flyout-cart').removeClass('active');
        });

        searchButton.on('click', function () {
            overlay.css('z-index', 1030);
            searchDropdown.toggleClass('active');
            if (!searchDropdown.hasClass('active')) {
                overlay.removeClass('show').fadeOut();
                $('html, body').removeClass('scrollYRemove');
            }
            optionsDropdown.removeClass('active');
            $('.flyout-cart').removeClass('active');
            menuDropdown.removeClass('open');
        });

        accountButton.on('click', function () {
            overlay.css('z-index', 1030);
            optionsDropdown.toggleClass('active');
            if (!optionsDropdown.hasClass('active')) {
                overlay.removeClass('show').fadeOut();
                $('html, body').removeClass('scrollYRemove');
            }
            searchDropdown.removeClass('active');
            $('.flyout-cart').removeClass('active');
            menuDropdown.removeClass('open');
        });

        cartButton.on('click', function () {
            $('.flyout-cart').toggleClass('active');
            if (ssCore.getViewPort().width > window.themeSettings.themeBreakpoint) {
                overlay.addClass('show').fadeIn();
                $('html, body').addClass('scrollYRemove');
                if (!$('.flyout-cart').hasClass('active')) {
                    overlay.removeClass('show').fadeOut();
                    $('html, body').removeClass('scrollYRemove');
                }
                searchDropdown.removeClass('active');
                optionsDropdown.removeClass('active');
                menuDropdown.removeClass('open');
            }
        });

        overlay.on('click', function () {
            searchDropdown.removeClass('active');
            optionsDropdown.removeClass('active');
            $('.flyout-cart').removeClass('active');
        });

        $('.store-search-box .close').on('click', function () {
            searchDropdown.removeClass('active');
            overlay.removeClass('show').fadeOut();
            $('html, body').removeClass('scrollYRemove');
        });

        $(document).on('themeBreakpointPassed7Spikes', function (e) {
            /* close any open elements and reset overlay and page scroll on themeBreakpoint pass */
            searchDropdown.removeClass('active');
            optionsDropdown.removeClass('active');
            $('.flyout-cart').removeClass('active');
            menuDropdown.removeClass('open');
            overlay.removeClass('show').fadeOut();
            $('html, body').removeClass('scrollYRemove');
        });
    }

    /* Reverses the default z-index order of top level menu items to prevent the overlapping bug when hovering on desktop resolutions.
       On mobile resolutions the z-index is released (auto). */

    $(document).on('themeBreakpointPassed7Spikes', function (e) {
        if (!e.isMobileResolution) {
            $($('.header-menu > ul > li').get().reverse()).css('z-index', function (index) {
                return index + 1;
            });
        }
        else {
            $('.header-menu > ul > li').css('z-index', 'auto');
        }
    });

    // Handles the shoping cart link behavior. Should be a link on mobile resolutions and a toggle button on desktop resolutions.

    function handleHeaderShoppingCartLink() {
        $('.shopping-cart-link a').on('click', function (event) {
            if (ssCore.getViewPort().width > window.themeSettings.themeBreakpoint) {
                event.preventDefault();
            }
            else {
                location.href = $(this).attr('href');
            }
        });
    }

    // Implements Flyout Cart scrolling, it's triggered when too many items are added to the cart

    function handleFlyoutCartScrolling() {

        var windowHeight = ssCore.getViewPort().height;
        var miniShoppingCart = $('.mini-shopping-cart');

        if (miniShoppingCart.length === 0) {
            return;
        }

        var miniShoppingCartItems = miniShoppingCart.children('.items');
        var miniShoppingCartOffsetTop = miniShoppingCart.offset().top - $(window).scrollTop();
        var miniShoppingCartHeight = miniShoppingCart.outerHeight();
        var miniShoppingCartItemsHeight = miniShoppingCartItems.outerHeight();
        var newItemsHeight = (windowHeight - miniShoppingCartOffsetTop - (miniShoppingCartHeight - miniShoppingCartItemsHeight) - 5);

        miniShoppingCartItems.css('max-height', newItemsHeight + 'px');
        miniShoppingCartItems.perfectScrollbar({
            swipePropagation: false,
            wheelSpeed: 1,
            suppressScrollX: true
        });
    }
    function handleFlyoutCartScroll() {

        $(window).on('resize orientationchange', function () {
            handleFlyoutCartScrolling();
        });

        $('.responsive-nav-wrapper').on('click', '.shopping-cart-link', function () {
            handleFlyoutCartScrolling();
        });
    }

    // Handles the side category navigation sublists toggleability (subcategory lists, sub subcategory list, etc).

    function handleBlockNavigationSublists() {
        $('.block .sublist').siblings().addClass('with-subcategories');
        $('.block .with-subcategories').on('click', function (event) {
            event.preventDefault();
            $(this).toggleClass('opened').siblings('.sublist').slideToggle();
        });
    }

    /* Handles the product item box button tooltips (desktop only). As "to be centered" abslolute elements of a different width,
       their horizontal offset should be applied dynamically based on width calculation. Will work in RTL too. */

    $(document).on('themeBreakpointPassed7Spikes', function (e) {
        if (!e.isMobileResolution) {
            $(document).on('mouseenter', '.item-box .buttons.desktop button, .quick-view-button', function () {
                var tooltipWIdth = $(this).children().outerWidth();
                $(this).children().css('margin-left', tooltipWIdth / 2 * -1);
            });
        }
    });

    // Implements custom "Product Quantity" functionality (on Product page and in Cart tables).

    function incrementQuantityValue(event) {
        event.preventDefault();
        event.stopPropagation();
        var input = $(this).siblings('.qty-input');
        var value = parseInt(input.val());
        if (isNaN(value)) {
            input.val(1);
            return;
        }
        value++;
        input.val(value);

        //http://stackoverflow.com/a/17110709/6744066
        input.trigger('input'); //input event trigger required by ROPC
        input.trigger('change'); //change event trigger required by IE11
    }
    function decrementQuantityValue(event) {
        event.preventDefault();
        event.stopPropagation();
        var input = $(this).siblings('.qty-input');
        var value = parseInt(input.val());
        if (isNaN(value)) {
            input.val(1);
            return;
        }
        if (value <= 1) {
            return;
        }
        value--;
        input.val(value);

        //http://stackoverflow.com/a/17110709/6744066
        input.trigger('input'); //input event trigger required by ROPC
        input.trigger('change'); //change event trigger required by IE11
    }
    function handlePurchaseQuantityValue() {
        $(document).on('click', '.add-to-cart .increase, .cart .increase', incrementQuantityValue);
        $(document).on('click', '.add-to-cart .decrease, .cart .decrease', decrementQuantityValue);
    }

    // Implements custom rating functionality (Product Reviews page).

    /*function handleProductReviewRatingIcons() {
            $(document).on('click', '.write-review .review-rating input[type="radio"]', function () {
                $('input:not(:checked)').parent().removeClass("checked");
                $('input:checked').parent().addClass("checked").prevAll().addClass("checked");
            });
    }*/

    // Toogle appearance for shopping cart collaterals content.

    function handleCartCollateralsContentToggle() {
        $('.cart-collaterals .title').on('click', function () {
            $(this).siblings('.inner-wrapper').slideToggle();
        });
    }

    // Prevents a conflict between shopping-cart page buttons and cross-sells buttons.

    function preventCrossSellsButtonsConflict() {
        $('.cross-sells .item-box button').removeClass('button-2');
    }
    $(window).on('nopAjaxCartButtonsAddedEvent', function () {
        $('.cross-sells .item-box button').removeClass('button-2');
    });

    // Handles the RichBlog active post toggleability on home page.

    function handleRichBlogActivePostToggle() {
        $('.rich-blog-homepage .post-details').first().addClass('active');
        $('.rich-blog-homepage .post-details').on('click', function () {
            $(this).addClass('active').siblings().removeClass('active');
        });
    }

    // Handles the behaviour of RichBlog's "display" element on home page in case there are both posts with and without pictures.

    function handleRichBlogDisplayHeight() {
        if ($('.rich-blog-homepage').length == 0) {
            return;
        }

        var calculatePostHeight = function () {
            var pictureUrl = $('.post-details.active').attr('data-pictureUrl');

            if (pictureUrl.length == 0) {
                $('.post-primary .post-info').css('position', 'static');
            }
        };

        calculatePostHeight();

        $(document).on('richBlogHomePageImageChanged', function () {
            calculatePostHeight();
        });
    }

    // Rearranges the RichBlog posts footer layout (no other way to acheive the predesigned layout).

    function rearrangeRichBlogPostFooter() {
        if ($('.blog-page').length > 0) {
            $('.blog-posts .post').each(function () {
                $(this).find('.category-list').detach().appendTo($(this).find('.blog-links'));
            });
        }
        else if ($('.blogpost-page').length > 0) {
            $('.post-date').detach().insertBefore('.post-body');
            $('.category-list').detach().appendTo('.blog-links');
        }
    }

    /* Sets SmartCategoriesCollections category image height, equalizes height between category image and product grid.
   Not necessary to limit this script to desktop resolutions since SPC category images are always hidden on mobile */

    $(document).on('newProductsAddedToPageEvent', function () {
        var equalizeHeights = function () {
            $('.spc-categories').each(function () {
                var bodyHeight = $(this).find('.spc-body').outerHeight();
                $(this).find('.category-picture').height(bodyHeight);
            });
        };
        equalizeHeights();
        $(window).on('resize', function () {
            equalizeHeights();
        });
    });

    // Handles Air Theme Variant 2 header buttons and header menu interactions

    $(document).on('themeBreakpointPassed7Spikes', function (e) {
        var searchButton = $('.air-theme.variant-2 .search-wrap span');
        var personalButton = $('.air-theme.variant-2 .personal-button');
        var cartButton = $('.air-theme.variant-2 .shopping-cart-link');

        var searchCloseButton = $('.air-theme.variant-2 .store-search-box .close');

        var headerMenuParent = $('.air-theme.variant-2 .header-menu-parent');
        var overlayCanvas = $('.air-theme.variant-2 .overlayOffCanvas');

        if (!e.isMobileResolution) {
            searchButton.on('click', function () {
                if (!$('.store-search-box').hasClass('active')) {
                    headerMenuParent.addClass('hidden');
                }
                else {
                    headerMenuParent.removeClass('hidden');
                }
            });
            personalButton.on('click', function () {
                if ($('.header-options').hasClass('active')) {
                    headerMenuParent.addClass('hidden');
                }
                else {
                    headerMenuParent.removeClass('hidden');
                }
            });
            cartButton.on('click', function () {
                if ($('.flyout-cart').hasClass('active')) {
                    headerMenuParent.addClass('hidden');
                }
                else {
                    headerMenuParent.removeClass('hidden');
                }
            });

            overlayCanvas.on('click', function () {
                headerMenuParent.removeClass('hidden');
            });
            searchCloseButton.on('click', function () {
                headerMenuParent.removeClass('hidden');
            });
        }
        headerMenuParent.removeClass('hidden');
    });

    // Handles Earth Theme Variant 2 desktop header menu (which is a mobile menu used on desktop)

    $(document).on('themeBreakpointPassed7Spikes', function (e) {
        if (!e.isMobileResolution) {
            /* prevents the core script to add "active" class to menu sublists on hover (mouseenter).
               It's a mobile design used on desktop in this variant so it should be active on click only */
            $('.earth-theme.variant-2 .header-menu li').off('mouseenter mouseleave');
        }
    });
	
	// Equalizes form-fields height on GDPR page
	
	function equalizeFrameHeight() {
		var target = $('.gdpr-tools-page .form-fields');
		var frameHeight = 0;
		target.each(function () {
			if($(this).height() > frameHeight) {
				frameHeight = $(this).height();
			}
		});
		target.height(frameHeight);
	}
       
    function changeImageSrcOnHover() {
        var imageHovered = $(this).find('.picture a img');
        var firstImageUrl = imageHovered.attr('src');
        var dataSecondImageUrl = imageHovered.attr('data-second-image');

        if (dataSecondImageUrl) {
            imageHovered.attr('src', dataSecondImageUrl);
            imageHovered.attr('data-second-image', firstImageUrl);
        }
    }

    // THEME VARIABLES & FUNCTION INVOCATIONS

    $(document).ready(function () {

        var responsiveAppSettings = {
            themeBreakpoint: window.themeSettings.themeBreakpoint,
            isEnabled: true,
            isSearchBoxDetachable: false,
            isHeaderLinksWrapperDetachable: false,
            doesDesktopHeaderMenuStick: true,
            doesScrollAfterFiltration: false,
            doesSublistHasIndent: true,
            displayGoToTop: true,
            hasStickyNav: true,
            lazyLoadProductBoxImages: true,
            selectors: {
                menuTitle: ".menu-title",
                headerMenu: ".header-menu",
                closeMenu: ".close-menu span",
                sublist: ".header-menu .sublist",
                overlayOffCanvas: ".overlayOffCanvas",
                withSubcategories: ".with-subcategories",
                filtersContainer: ".nopAjaxFilters7Spikes",
                filtersOpener: ".filters-button span",
                searchBoxOpener: ".search-wrap span",
                searchBox: ".store-search-box",
                searchBoxBefore: ".header-logo",
                navWrapper: ".responsive-nav-wrapper",
                navWrapperParent: ".responsive-nav-wrapper-parent",
                headerLinksOpener: ".personal-button span",
                headerLinksWrapper: ".header-options",
                headerMenuDesktopStickElement: ".desktop-nav-wrapper",
                headerMenuDesktopStickParentElement: ".master-header-wrapper",
                shoppingCartLink: ".shopping-cart-link"
            }
        };

        recognizeMobileDevices();

        handleHeaderNavigationDropdownsPosition();
        handleHeaderNavigationDropdownsToggle();
        handleHeaderShoppingCartLink();
        handleFlyoutCartScroll();
        handleBlockNavigationSublists();
        handlePurchaseQuantityValue();
        //handleProductReviewRatingIcons();
        handleCartCollateralsContentToggle();
        preventCrossSellsButtonsConflict();
        handleRichBlogActivePostToggle();
        handleRichBlogDisplayHeight();
        rearrangeRichBlogPostFooter();
        equalizeFrameHeight();

        // Add Images Rollover Effect
        $('.page').on('mouseenter', '.item-box', changeImageSrcOnHover).on('mouseleave', '.item-box', changeImageSrcOnHover);

        ssEx.initResponsiveTheme(responsiveAppSettings);
    });

    $(window).on('load', function () {
        $('.master-loading-overlay').hide();
    });

})(jQuery, sevenSpikesCore, sevenSpikesEx);