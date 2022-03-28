(function ($, ssCore, ssEx) {

    window.themeSettings = {
        themeBreakpoint: 1024,
        isAccordionMenu: false
    };

    function handleReponsiveHeaderLinks() {
        var pageTitle = $('.side-2 .page-title');
        var adminHeaderLinks = $('.admin-header-links');
        var attributeDt = $('.attributes dt');
        var paymentDetailsLabel = $('.payment-details label');
        var labelMaxWidth = 0;
        var methodNameMaxWidth = 0;

        $(document).on('themeBreakpointPassed7Spikes', function (e) {
            var searchBox = $('.search-box.store-search-box');
            var headerLinks = $('.header-links');
            var headerSelectors = $('.header-selectors');
            var shoppingCart = $('.flyout-cart-wrapper');

            searchBox.removeAttr('style');
            headerLinks.removeAttr('style');
            headerSelectors.removeAttr('style');
            shoppingCart.removeAttr('style');

            if (e.isMobileResolution) {
                // mobile
                searchBox.detach().appendTo('.responsive-nav-wrapper .search-wrap');
                headerLinks.detach().appendTo('.responsive-nav-wrapper #header-links-opener');
                if (headerSelectors != undefined && headerSelectors.length > 0) {
                    headerSelectors.detach().appendTo('.responsive-nav-wrapper #header-selectors-opener');
                }
                else {
                    // hide the mobile "Preferences" menu icon if no languages, currencies or tax display type available.
                    $('#header-selectors-opener').hide();
                }

                shoppingCart.detach().appendTo('.responsive-nav-wrapper .shopping-cart-link');
                adminHeaderLinks.detach().prependTo('.master-wrapper-page');
                pageTitle.detach();

                attributeDt.css('width', '');
            }
            else {
                setTimeout(function () {
                    // Attribute labels equalize widths
                    labelMaxWidth = 0;
                    attributeDt.each(function () {
                        labelMaxWidth = Math.max(labelMaxWidth, $(this).outerWidth());
                    });

                    attributeDt.css('width', labelMaxWidth + 'px');
                }, 100);

                if (e.isInitialLoad === false) {
                    // desktop
                    searchBox.detach().appendTo('.header-lower .center');
                    headerLinks.detach().appendTo('.header-links-wrapper');
                    if (headerSelectors != undefined && headerSelectors.length > 0) {
                        headerSelectors.detach().appendTo('.header-selectors-wrapper');
                    }
                    else {
                        $('#header-selectors-opener').show();
                    }
                    shoppingCart.detach().appendTo('.header-middle .center');
                    adminHeaderLinks.detach().prependTo('.header-upper .center');
                    $('.side-2').prepend(pageTitle);
                }
            }

            // Payment labels equalize widths
            paymentDetailsLabel.each(function () {
                methodNameMaxWidth = Math.max(methodNameMaxWidth, $(this).outerWidth());
            });

            paymentDetailsLabel.css('width', methodNameMaxWidth + 'px');
        });

        var responsiveHeaderLinkOpeners = $('.responsive-nav-wrapper').find('.shopping-cart-link > span, #header-links-opener > span, #header-selectors-opener > span, .search-wrap > span');

        $('.responsive-nav-wrapper .menu-title, .responsive-nav-wrapper .filters-button, .overlayOffCanvas').on('click', function () {
            responsiveHeaderLinkOpeners.next().slideUp();
            setTimeout(function () {
                $('.admin-header-links, .responsive-nav-wrapper-parent').css('z-index', '');
                $('.header').css({ 'position': '', 'z-index': '' });
            }, 400);
        });

        responsiveHeaderLinkOpeners.on('click', function () {
            var nextElement = $(this).next();

            responsiveHeaderLinkOpeners.next().not(nextElement).slideUp();

            if (nextElement.is(':visible')) {
                nextElement.slideUp('slow');
                $('.overlayOffCanvas').fadeOut(function () {
                    $(this).removeClass('show');
                    $('.admin-header-links, .responsive-nav-wrapper-parent').css('z-index', '');
                    if ($('.mobile-sticky-logo').length > 0) {
                        $('.header').css({ 'position': '', 'z-index': '' });
                    }
                });
            } else {
                nextElement.slideDown('slow');
                $('.admin-header-links, .responsive-nav-wrapper-parent').css('z-index', '1070');
                $('.overlayOffCanvas').addClass('show').fadeIn();
                if ($('.mobile-sticky-logo').length > 0) {
                    $('.header').css({ 'position': 'relative', 'z-index': '1070' });
                }
            }
        });
    }

    function handleFooterBlocksCollapse() {
        $(".footer-block .title").click(function (e) {
            if (ssCore.getViewPort().width <= window.themeSettings.themeBreakpoint) {
                $(this).siblings(".list").slideToggle("slow");
            }
            else {
                e.preventDefault();
            }
        });
    }

    function handleHeaderMenuCategories() {
        if ($('.header-menu.categories-in-side-panel').length === 0) {
            return;
        }

        var sideCategoriesList = $('.category-navigation-list-wrapper > .category-navigation-list');

        var topMenuCategoryItems = $('.top-menu > li.root-category-items, ' +
            '.mega-menu > li.root-category-items, ' +
            '.mega-menu > li.category-menu-item > .sublist-wrap > .sublist > li.root-category-items');

        var topMenuCategoryItemsCloned = topMenuCategoryItems.clone(true);

        topMenuCategoryItemsCloned.removeClass('root-category-items').appendTo(sideCategoriesList);

        if ($('.two-columns-area.slider-right-column').length > 0) {
            sideCategoriesList.css('display', 'block');
        }
        else {
            $('.category-navigation-title').on('click', function () {
                sideCategoriesList.slideToggle();
            });
        }
    }

    function handleCustomerBlocksTitle() {
        $('body').on('click', '.customer-blocks .title', function () {
            if (!$(this).hasClass('active')) {
                $('.customer-blocks .title').toggleClass('active');
            }
            if (!$(this).siblings().hasClass("show")) {
                $('.customer-blocks .title').siblings('.inner-wrapper').toggleClass('show');
            }
        });
    }

    function handleOrderSummaryAccordion() {
        $('.shopping-cart-page .accordion-tab-title').on('click', function () {
            $(this).siblings('.accordion-tab-content').slideToggle().closest('.accordion-tab').toggleClass('active')
                .siblings('.accordion-tab').removeClass('active').find('.accordion-tab-content').slideUp();
        });

        if ($('.shopping-cart-page .shipping-results').length > 0) {
            $('.shopping-cart-page .accordion-tab.estimate-shipping .accordion-tab-title').trigger('click');
        }

        if ($('.shopping-cart-page .coupon-box').find('.message-success, .message-failure').length > 0) {
            $('.shopping-cart-page .accordion-tab.coupon-codes .accordion-tab-title').trigger('click');
        }

        if ($('.shopping-cart-page .giftcard-box').find('.message-success, .message-failure').length > 0) {
            $('.shopping-cart-page .accordion-tab.gift-cards .accordion-tab-title').trigger('click');
        }
    }

    function updateLoginPopup(data) {
        var dataObj = $(data);

        var resources = dataObj.find('#login-modal-window-static-resources').children();
        var cssFiles = resources.filter('link');
        var jsFiles = resources.filter('script');

        cssFiles.each(function () {
            var that = $(this);

            if ($('head link[href="' + that.attr('href') + '"]').length === 0) {
                $('head').append(that);
            }
        });

        jsFiles.each(function () {
            var that = $(this);

            if ($('head script[src="' + that.attr('src') + '"]').length === 0) {
                $('head').append(that);
            }
        });

        dataObj.find('#login-modal-window-static-resources').remove();

        $('#login-modal-window-overlay').fadeIn();
        $('#login-modal-window').html(dataObj).closest('.login-modal-window-wrapper').fadeIn();
    }

    function handleLoginModalPopup() {
        $('.header-links .ico-login.modal-login').on('click', function (e) {
            e.preventDefault();

            var that = $(this);

            $('body').css('overflow', 'hidden').on('keydown.loginModalWindow', function (e) {
                // If the pressed key is ESC then close the modal popup
                if (e.which === 27) {
                    $('.login-modal-window-wrapper > .close').trigger('click');
                }
            });

            if ($('.login-modal-window-wrapper').length === 0) {
                $('body').append('<div id="login-modal-window-overlay"></div><div class="login-modal-window-wrapper"><span class="close"></span><div id="login-modal-window"></div></div>');
            } else {
                $('#login-modal-window-overlay, .login-modal-window-wrapper').fadeIn();
                return;
            }

            $.ajax({
                url: that.attr('data-loginUrl'),
                type: 'GET',
                //dataType: 'html',
                data: {
                    'isPopup': true
                }
            }).done(updateLoginPopup).fail(function () {
                $('body').css('overflow', '');
                window.location.href = that.attr('data-loginUrl');
            });
        });

        $('body').on('click', '#login-modal-window-overlay, .login-modal-window-wrapper > .close', function () {
            $('#login-modal-window-overlay, .login-modal-window-wrapper').fadeOut();
            $('body').css('overflow', '').off('keydown.loginModalWindow');
        });

        $('body').on('click', '.login-modal-window-wrapper .login-button', function (e) {
            e.preventDefault();

            var form = $(this).closest('form');

            var data = form.serialize();

            $.ajax({
                url: form.attr('action') + "?isPopup=true",
                type: form.attr('method'),
                //dataType: 'html',
                data: data
            }).done(function (data) {
                var dataObj = $(data);

                if (dataObj.find('#login-modal-window-static-resources').length > 0) {
                    updateLoginPopup(data);
                } else {
                    window.location.reload();
                }
            });
        });
    }

    function handleHomePageRichBlogCarousel(numberOfItemsToRotate) {
        var richBlogCarousel = $('.rich-blog-homepage .blog-posts');

        if (richBlogCarousel.length === 0) {
            return;
        }

        var isInitialized = false;

        var blogPosts = richBlogCarousel.children('.blog-post');

        if (blogPosts.length > 1) {

            var lazyLoadRichBlogCarousel = function () {

                if (ssCore.isInViewPort(richBlogCarousel) && !isInitialized) {

                    var numberOfDesktopItems = Math.min(numberOfItemsToRotate, blogPosts.length);

                    richBlogCarousel.slick({
                        rtl: $('body').hasClass('rtl'),
                        autoplay: true,
                        infinite: true,
                        pauseOnHover: true,
                        arrows: true,
                        dots: false,
                        slidesToScroll: 1,
                        slidesToShow: numberOfDesktopItems,
                        responsive: [
                            {
                                breakpoint: 1025,
                                settings: {
                                    slidesToShow: 1
                                }
                            }
                        ]
                    });

                    isInitialized = true;

                    $(window).off('scroll.SevenSpikesPavilionRichBlogCarousel');
                    $(window).off('resize.SevenSpikesPavilionRichBlogCarousel');
                }
            }

            lazyLoadRichBlogCarousel();
            $(window).on('scroll.SevenSpikesPavilionRichBlogCarousel', lazyLoadRichBlogCarousel);
            $(window).on('resize.SevenSpikesPavilionRichBlogCarousel', lazyLoadRichBlogCarousel);
        }
    }

    function handleHomePageBestsellersCarousel() {
        var bestsellersCarousel = $('.product-grid.bestsellers .item-grid');

        if (bestsellersCarousel.length === 0) {
            return;
        }

        var isInitialized = false;

        if (bestsellersCarousel.find('.item-box').length > 1) {

            var lazyLoadBestSellersCarousel = function () {

			    if (ssCore.isInViewPort(bestsellersCarousel) && !isInitialized) {

                    bestsellersCarousel.slick({
                        rtl: $('body').hasClass('rtl'),
                        autoplay: true,
                        infinite: true,
                        pauseOnHover: true,
                        arrows: true,
                        dots: false,
                        slidesToScroll: 1,
                        slidesToShow: 2
                    });

                    isInitialized = true;

                    $(window).off('scroll.SevenSpikesPavilionBestSellerCarousel');
                    $(window).off('resize.SevenSpikesPavilionBestSellerCarousel');
                }
            }
        }

        lazyLoadBestSellersCarousel();
        $(window).on('scroll.SevenSpikesPavilionBestSellerCarousel', lazyLoadBestSellersCarousel);
        $(window).on('resize.SevenSpikesPavilionBestSellerCarousel', lazyLoadBestSellersCarousel);
    }

    function handleRichBlogSearch() {
        var blogSearchBox = $('.rich-blog-body .blog-search-box');

        if (blogSearchBox.length === 0) {
            return;
        }

        blogSearchBox.on('mouseenter', function () {
            $(this).addClass('active');
        });

        $(document).on('click', function (e) {
            if ($(e.target).closest('.blog-search-box, .blog-instant-search').length === 0) {
                $('.rich-blog-body .blog-search-box').removeClass('active');
            }
        });
    }

    function handleInstantSearchHiding() {
        $('.header').on('mouseenter', '.flyout-cart-wrapper', function () {
            if ($(this).find('.item').length > 0) {
                $('.instantSearch, .blog-instant-search').hide();
            }
        });

        $('.category-navigation-list > li, .root-category-items').on('mouseenter', function () {
            if ($(this).find('.sublist-wrap').length > 0) {
                $('.instantSearch, .blog-instant-search').hide();
            }
        });
    }

    function handleFlyoutCartScrolling(isInitialLoad) {
        if (isInitialLoad) {
            $('.flyout-cart-wrapper .flyout-cart').css({ 'opacity': '0', 'display': 'block' });
        }

        var windowHeight = ssCore.getViewPort().height;
        var miniShoppingCart = $('.mini-shopping-cart');

        if (miniShoppingCart.length === 0) {
            return;
        }

        var miniShoppingCartItems = miniShoppingCart.children('.items');
        var miniShoppingCartOffsetTop = miniShoppingCart.offset().top - $(window).scrollTop();
        var miniShoppingCartHeight = miniShoppingCart.outerHeight();
        var miniShoppingCartItemsHeight = miniShoppingCartItems.outerHeight();
        var newItemsHeight = (windowHeight - miniShoppingCartOffsetTop - (miniShoppingCartHeight - miniShoppingCartItemsHeight) - 10);

        miniShoppingCartItems.css('max-height', newItemsHeight + 'px');
        miniShoppingCartItems.perfectScrollbar({
            swipePropagation: false,
            wheelSpeed: 1,
            suppressScrollX: true
        });

        if (isInitialLoad) {
            $('.flyout-cart-wrapper .flyout-cart').css({ 'display': '', 'opacity': '' });
        }
    }

    function handleFlyoutCartScroll() {
        handleFlyoutCartScrolling(true);

        $(window).on('resize orientationchange', function () {
            setTimeout(function () {
                handleFlyoutCartScrolling(true);
            }, 200);
        });

        $('.header').on('mouseenter', '.flyout-cart-wrapper', function () {
            if (ssCore.getViewPort().width > window.themeSettings.themeBreakpoint) {
                setTimeout(handleFlyoutCartScrolling, 200);
            }
        });

        $('.responsive-nav-wrapper').on('click', '.shopping-cart-link', function () {
            setTimeout(handleFlyoutCartScrolling, 800);
        });
    }

    function handleAddToCartOverlayFromQuickView() {

        $('body').on("click", ".quickViewWindow .add-to-cart-button", function () {

            $('.ajax-loading-block-window').hide();
            $('.nopAjaxCartPanelAjaxBusy').hide();

        });
    }

    function hideProductCollateralIfEmpry() {

        // we should hide the product-collateral if it is empty or all its children are hidden in case of quick tabs are enabled.
        var productCollateral = $('.product-details-page .product-collateral');
        var productCollateralChildren = productCollateral.children();
        if (productCollateralChildren.length == 0) {
            productCollateral.hide();
            return;
        }

        var areAllProductCollateralHidden = true;
        for (i = 0; i < productCollateralChildren.length; i++) {
            if ($(productCollateralChildren[i]).is(':visible') == true) {
                areAllProductCollateralHidden = false;
                break;
            }
        }

        if (areAllProductCollateralHidden == true) {
            productCollateral.hide();
        }
    }

    function handleHeaderSelectorsHoverState() {
        // this is a workaround for bug in Firefox and IE

        if (ssCore.isMobile() || ssCore.getViewPort().width <= window.themeSettings.themeBreakpoint) {
            return;
        }

        $('.header-selectors').on('mouseenter', function () {
            $(this).addClass('active');
        }).on('mouseleave', function () {
            if (!$(this).find('select').is(':focus')) {
                $('.header-selectors').removeClass('active');
            }
        }).on('click', function (e) {
            e.stopPropagation();
        });

        var removeHeaderSelectorsClass = function () {
            $('.header-selectors').removeClass('active');
        };

        $(document).on('click', removeHeaderSelectorsClass);
        $('.header-links-wrapper, .header-middle, .header-lower').on('mouseenter', removeHeaderSelectorsClass);
    }

    function handleStickyFlyoutHeight() {
        $('.sticky-flyout > li > .sublist-wrap > .sublist').css({ 'min-height': ($('.sticky-flyout').height() + 'px') });
    }

    function handleShippingAddressForm() {
        var checkboxElement = $('.shipping-address-page #PickUpInStore');
        var targetElements = $('.new-shipping-address .title, .edit-address');
        var continueButton = $('.shipping-address-page .buttons');

        if (checkboxElement.is(':checked')) {
            targetElements.hide();
            continueButton.addClass('centered');
        }
        else {
            targetElements.show();
        }
        // the above code handles onload behavior, below is onclick behavior
        $(checkboxElement).on('click', function () {
            targetElements.toggle();
            continueButton.toggleClass('centered');
        });
    }

    $(document).ready(function () {

        var responsiveAppSettings = {
            themeBreakpoint: window.themeSettings.themeBreakpoint,
            isEnabled: true,
            isSearchBoxDetachable: false,
            isHeaderLinksWrapperDetachable: false,
            doesDesktopHeaderMenuStick: false,
            doesScrollAfterFiltration: true,
            doesSublistHasIndent: true,
            displayGoToTop: true,
            hasStickyNav: true,
            selectors: {
                menuTitle: ".menu-title",
                headerMenu: ".header-menu",
                closeMenu: ".close-menu",
                sublist: ".header-menu .sublist",
                overlayOffCanvas: ".overlayOffCanvas",
                withSubcategories: ".with-subcategories",
                filtersContainer: ".nopAjaxFilters7Spikes",
                filtersOpener: ".filters-button span",
                searchBoxOpener: "",
                searchBox: "",
                searchBoxBefore: "",
                navWrapper: ".responsive-nav-wrapper",
                navWrapperParent: ".responsive-nav-wrapper-parent",
                headerLinksOpener: "",
                headerLinksWrapper: "",
                headerLinksWrapperMobileInsertAfter: "",
                headerLinksWrapperDesktopPrependTo: "",
                shoppingCartLink: ".shopping-cart-link"
            }
        };

        handleReponsiveHeaderLinks();
        handleFooterBlocksCollapse();
        handleHeaderMenuCategories();
        handleCustomerBlocksTitle();
        handleOrderSummaryAccordion();
        handleLoginModalPopup();
        handleRichBlogSearch();
        handleInstantSearchHiding();
        handleFlyoutCartScroll();
        handleAddToCartOverlayFromQuickView();
        hideProductCollateralIfEmpry();
        handleHeaderSelectorsHoverState();
        handleStickyFlyoutHeight();
        handleShippingAddressForm();

        var homePageBestsellersGrid = $('.product-grid.bestsellers');

        if (homePageBestsellersGrid.find('.item-box').length < 1) {
            homePageBestsellersGrid.remove();
            $('.rich-blog-homepage').addClass('full-width');
            handleHomePageRichBlogCarousel(3);
        } else {
            handleHomePageBestsellersCarousel();
            handleHomePageRichBlogCarousel(1);
        }

        $(document).on('nopAjaxCartMiniProductDetailsViewShown nopQuickViewDataShownEvent', function () {
            var attributeDt = $('.attributes dt');
            var labelMaxWidth = 0;

            attributeDt.each(function () {
                labelMaxWidth = Math.max(labelMaxWidth, $(this).outerWidth());
            });

            attributeDt.css('width', labelMaxWidth + 'px');

            attributeDt.each(function () {
                var attributesLineWidth = $(this).outerWidth(true) + $(this).next().outerWidth(true);
                if (attributesLineWidth > 280) {
                    $('.attributes dd').css({ 'float': 'none', 'clear': 'both' });
                    return false;
                }
            });
        });

        ssEx.initResponsiveTheme(responsiveAppSettings);
    });
})(jQuery, sevenSpikesCore, sevenSpikesEx);


//override global ajax loader
function displayAjaxLoading(display) {
    if (display) {
        $('.ajax-loading-block-window').show();
    }
    else {
        $('.ajax-loading-block-window').fadeOut();
    }
}