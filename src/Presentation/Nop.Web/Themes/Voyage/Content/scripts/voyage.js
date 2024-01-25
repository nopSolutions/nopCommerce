(function ($) {
    $(document).ready(function () {

        var dependencies = [
            {
                module: "header",
                dependencies: ["attachDetach", "overlay", "perfectScrollbar"]
            },
            {
                module: "menu",
                dependencies: ["perfectScrollbar"]
            }
        ];

        var themeSettings = {

            // global settings

            flyoutCart: {
                flyoutCartSelector: '#flyout-cart',
                removeItemSelector: '#flyout-cart .remove-btn'
            },
            productQuantity: {
                quantityInputSelector: '.qty-input',
                incrementSelectors: '.increase',
                decrementSelectors: '.decrease'
            },
            attachDetach: {
                blocks: [
                    {
                        elementToAttach: ".blogpost-page .post-footer",
                        content: ".richblog-pages-body .blogpost-page .tags",
                        insertAction: "prependTo"
                    }
                ]
            },

            // responsive settings

            responsive: [
                {
                    breakpoint: 1261,
                    settings: {
                        header: {
                            activeClass: 'active',
                            modules: [
                                {
                                    opener: '.responsive-nav-wrapper .menu-button',
                                    closer: '.close-menu span',
                                    content: '.header-menu',
                                    scrollbar: false,
                                    overlay: true,
                                    disablePageScroll: true
                                },
                                {
                                    opener: '.responsive-nav-wrapper .personal-button',
                                    closer: '.close-links span',
                                    content: '.header-links',
                                    overlay: true,
                                    disablePageScroll: true
                                },
                                {
                                    opener: '.responsive-nav-wrapper .search-button',
                                    closer: '.close-search span',
                                    content: '.store-search-box',
                                    overlay: true,
                                    disablePageScroll: true
                                },
                                {
                                    opener: '.filters-button',
                                    closer: '.close-filters',
                                    content: '.nopAjaxFilters7Spikes',
                                    scrollbar: false,
                                    overlay: true,
                                    disablePageScroll: true
                                }
                            ]
                        },
                        overlay: {
                            overlayElementSelector: '.overlayOffCanvas',
                            overlayClass: 'active',
                            noPageScrollClass: 'scrollYRemove'
                        },
                        toggle: {
                            blocks: [
                                {
                                    opener: '.block:not(.filter-block) .title',
                                    content: '.block:not(.filter-block) .listbox',
                                    activeClassOpener: 'opened',
                                    animation: {
                                        type: 'slide',
                                        speed: 'slow'
                                    }
                                },
                                {
                                    opener: '.footer-block .title',
                                    content: '.footer-block .list',
                                    activeClassOpener: 'opened',
                                    animation: {
                                        type: 'slide',
                                        speed: 'slow'
                                    }
                                },
                                {
                                    opener: '.deals .title',
                                    content: '.deals .content',
                                    activeClassOpener: 'opened',
                                    animation: {
                                        type: 'slide',
                                        speed: 'slow'
                                    }
                                }
                            ]
                        },
                        attachDetach: {
                            blocks: [
                                {
                                    elementToAttach: ".account-page .page-body",
                                    content: ".block-account-navigation",
                                    insertAction: "prependTo"
                                },
                                {
                                    elementToAttach: ".blogpost-page .post-footer",
                                    content: ".richblog-pages-body .blogpost-page .tags",
                                    insertAction: "prependTo"
                                }
                            ]
                        },
                        filters: {
                            closePanelAfterFiltrationDataAttribute: 'closefilterspanelafterfiltrationinmobile'
                        }
                    }

                    // define more breakpoints here if necessary.
                    // all responsive settings defined for any breakpoint should be present also in every other breakpoint otherwise they will not work.
                }
            ]
        };

        var theme = new window.sevenSpikesTheme(themeSettings, dependencies, false);

        theme.init();

        // invoke custom functions

        handleHeaderUpper();
        setHeaderSelectSizes();
        handleMobileNavigation();
        updateMobileCartQuantity();
        closePopupOnOverlayClick();
        handleCategoryNavigation();
        handleCatalogSorting();
        handleProductOverview();
        applyAdditionalClasses();
        handleErrorNotifications();
        handleNewAddressForm();
        applyCheckoutClasses();
        handleCheckoutMethods();
        setFiltersButtonPosition();
        setSearchCriteriaSize();
        handleRichBlogContent();
        handleSCCsubcategories();





        //   C U S T O M  F U N C T I O N S



        // Set up additional class names based on number and type of elements in upper header

        function handleHeaderUpper() {

            var targetElement = $('.header-upper');

            if (targetElement.children().length == 2) {
                targetElement.addClass('two');
            }
            if (targetElement.children().length == 3) {
                targetElement.addClass('three');
            }
            $('.header-selectors').children().addClass('selector');

            if ($('.impersonate').length > 0) {
                $('.header').addClass('impersonated session');
            }
            if ($('.html-category-page, .html-manufacturer-page, .html-vendor-page, .html-product-details-page, .html-topic-page').length > 0) {
                $('.header').addClass('extended-admin');
            }
        }



        // Use character number to calculate width of select elements based on the seleced <option>. "Width" can't be used for this as <option>s width is always 100% of the longest one

        function setHeaderSelectSizes() {

            $('.header-selectors select').each(function () {

                var sizeOnLoad = $(this).children('option:selected').text().length * 10; // approx 10px per character (uppercase)

                $(this).on('change', function () {

                    var sizeOnChange = $(this).children('option:selected').text().length * 10; // approx 10px per character (uppercase)

                    $(this).css('max-width', sizeOnChange);
                }).css('max-width', sizeOnLoad);
            });
        }



        // Set up class names when the navigation panel sticks at the top of the screen, and when the page is scrolled up 

        function handleMobileNavigation() {

            var targetElement = $('.responsive-nav-wrapper');
            var demoStrip = $('.header-storetheme');
            var lastScrollTop = 0;

            var queryElement = document.querySelector('.responsive-nav-wrapper');
            var observer = new IntersectionObserver(
                ([e]) => e.target.classList.toggle('pinned', e.intersectionRatio < 1),
                { threshold: [1] }
            );
            observer.observe(queryElement); // detect when sticky element sticks

            if ($(window).outerWidth() >= themeSettings['responsive'][0]['breakpoint']) {
                setTimeout(function () {
                    targetElement.removeClass('pinned');
                }, 100); // set delay, or the class will be added again
            }
            $(document).on('themeBreakpoint', function (event, settings, breakpoint) {
                if (breakpoint !== null) {
                    targetElement.addClass('pinned up'); // mobile
                }
                else {
                    targetElement.removeClass('pinned up'); // desktop
                }
            });
            $(window).on('scroll', function () {

                var scrollTop = $(this).scrollTop();
                
                if (scrollTop > lastScrollTop) {
                    targetElement.removeClass('up demo'); // scroll down
                }
                else {
                    targetElement.addClass('up'); // scroll up

                    if (demoStrip.length > 0) {
                        targetElement.addClass('demo'); // nop-templates demo strip
                    }
                }
                lastScrollTop = scrollTop;
            });
        }



        // Updates the "mobile" quantity marker when a product is removed from the flyout cart on desktop. It uses the "desktop" marker which is updated by the core script

        function updateMobileCartQuantity() {

            $(document).on('removeItemFlyoutCart', function () {

                var productsCountElement = $('.cart-link .cart-qty'); // desktop
                var mobileQtyElement = $('.cart-button .cart-qty'); // mobile

                if (productsCountElement.length && mobileQtyElement.length) {

                    var regex = /\d+/;
                    var productsCountString = productsCountElement.text();
                    var productsCount = productsCountString.match(regex) || [0];

                    mobileQtyElement.text(productsCount[0]);
                }
            });
        }



        // Workaround for closing "ui-dialog" popups from their screen overlay (as the overlay is a pseudo element, it can't be targeted with JS)

        function closePopupOnOverlayClick() {

            $(document).mousedown(function (e) {
                // detect left click ("1" = left click, "2" = middle click, "3" = right click)
                if (e.which == 1) {
                    // "stopPropagation" will deactivate the "Close" button, so "target" is used instead
                    if (!$(e.target).closest('.ui-dialog-titlebar, .ui-dialog-content').length > 0) {
                        $('.ui-dialog-titlebar-close').click();
                    }
                }
            });
        }



        // Toggle expand/collapse functionality for subcategories inside Category Navigation panel

        function handleCategoryNavigation() {

            var targetElement = $('.block .sublist').parent();

            targetElement.each(function () {

                if ($(this).hasClass('active')) {
                    $(this).addClass('accordion expanded');
                }
                else {
                    $(this).toggleClass('accordion collapsed');
                }

                $(this).find('a').on('click', function (e) {
                    e.stopPropagation();
                });

                $(this).on('click', function (e) {
                    e.stopPropagation();
                    if ($(this).hasClass('active')) {
                        $(this).toggleClass('expanded').toggleClass('collapsed');
                    }
                    else {
                        $(this).toggleClass('collapsed').toggleClass('expanded');
                    }
                    $(this).children('ul').children('li').slideToggle();
                });
            });
        }



        // Use character number to calculate width of select elements based on the seleced <option>. "Width" can't be used for this as <option>s width is always 100% of the longest one

        function handleCatalogSorting() {

            $('.product-selectors select').each(function () {

                var sizeOnLoad = $(this).children('option:selected').text().length * 8; // approx 8px per character

                $(this).on('change', function () {

                    var sizeOnChange = $(this).children('option:selected').text().length * 8; // approx 8px per character

                    $(this).css('max-width', sizeOnChange);
                }).css('max-width', sizeOnLoad);
            });

            $(document).on('ajaxFiltersSortOptionsChanged', function (e) {

                var element = e.element;
                var sizeOnReplace = element.children('option:selected').text().length * 8; // approx 8px per character

                element.css('max-width', sizeOnReplace);
            });
        }



        // Set up additional class names on Product Details pages. Set up down-scrolling for associated products

        function handleProductOverview() {

            var targetElement = $('.stock-delivery-wrapper');
            if (!targetElement.children().length > 0) {
                targetElement.addClass('empty');
            }

            $('.add-to-cart-panel .qty-wrapper').focusin(function () {
                $(this).addClass('focus');
            }).focusout(function () {
                $(this).removeClass('focus')
            });

            $('.attributes .qty-box').siblings('select').addClass('qty-select');

            $('.variants-scroll-button').on('click', function () {
                $('html, body').animate({
                    scrollTop: $('.product-variant-list').offset().top - 30
                }, 1000);
            });
        }



        // Set up additional class names on various Customer pages, Shopping Cart page, Search page, ForumEdit page, and Profile page

        function applyAdditionalClasses() {

            $('.registration-page, .login-page, .password-recovery-page, .account-page, .return-request-page, .user-agreement-page').find('.button-1').addClass('account-button');
            $('.gdpr-tools-page form').addClass('gdpr-form');

            if ($('.reward-points-page .pager li').length == 1) {
                $('.reward-points-page .pager').addClass('empty');
            }
            $('.order-progress .active-step:last').addClass('current-step');

            if ($('.cart tbody tr').length % 2 != 0) {
                $('.cart').addClass('odd-number');
            }
            $('.search-page').find('#advs, #isc').prop('checked', true);

            if ($('.forum-subject').length > 0) {
                $('.forum-subject').parent('.inputs').hide();
            }
            $('.profile-page .quote').prev().addClass('hide');
            $('.private-messages col:last-child').addClass('date').removeAttr('width');
        }



        // Scroll page to Discount of GiftCard notification text whenever it is present on the Shopping Cart page

        function handleErrorNotifications() {

            var notifications = $('.deals .message-failure, .deals .message-success, .addon-buttons .message-error');

            if (notifications.length > 0) {

                var additionalOffset = ($('.header-storetheme').length > 0) ? "90" : "30"; // check for nop-templates demo strip

                notifications.parent().siblings('.title').click(); // expand Discount or GiftCard panels which are collapsed by default on mobile resolutions

                $('html, body').animate({
                    scrollTop: notifications.offset().top - additionalOffset
                }, 1000);
            }
        }



        // Toggle and Scroll functionality for "New Address / Edit Address" form on Checkout pages

        function handleNewAddressForm() {

            var triggerElement = $('.add-address-button');
            var targetElement = $('.new-address-wrapper');
            var numberOfAddresses = $('.address-item').length;

            if (numberOfAddresses % 2 == 0) {
                $('.address-item.last').prev().addClass('unflex');
            }
            triggerElement.on('click', function () {
                targetElement.slideToggle('slow').toggleClass('active');
                $('html, body').animate({
                    scrollTop: targetElement.offset().top - 60
                }, 1000);
            });
            $('.edit-address-button').on('click', function () {
                if (!targetElement.hasClass('active')) {
                    triggerElement.click();
                }
            });
        }



        // Set up additional class names on Checkout pages. To be invoked also on OPC step load

        function applyCheckoutClasses() {

            $('.shipping-method-page .pickup-in-store').addClass('additional');
            $('.method-list .payment-description').toggleClass('payment-description method-description');

            if ($('.payment-info td').length > 1) {
                $('.payment-info table').addClass('form').find('tr').addClass('inputs');
            }
            if ($('.info-list').parent().siblings().length > 2) {
                $('.info-list').parent().parent().addClass('full');
            }
        }



        // Detect currently active shipping/payment method and apply custom behavior to the active element's DOM

        function handleCheckoutMethods() {

            var targetElement = $('.method-list li');

            targetElement.each(function () {
                if ($(this).find('input').is(':checked')) {
                    $(this).addClass('selected');
                }
            });
            targetElement.on('click', function () {
                $(this).find('input').prop('checked', true);
                $(this).addClass('selected').siblings('').removeClass('selected');
            });
        }



        // Trigger additional event on OPC step load. Use the event to apply script to OPC (html markup is not loaded until step load)

        if (window.Checkout) { // check if OPC is present

            var setStepResponse = Checkout.setStepResponse;
            Checkout.setStepResponse = function (response) {
                setStepResponse(response);
                $(document).trigger("opcStepLoad");
            };
            $(document).on("opcStepLoad", function () {
                applyCheckoutClasses();
                handleCheckoutMethods();
            });
        }
        // Re-apply additional checkout class names on ROPC panel load and on Payment Method refresh (otherwise they won't work in ROPC)
        $(document).on('nopOnePageCheckoutPanelsLoadedEvent', function () {
            applyCheckoutClasses();
        });
        $(document).on('SevenSpikesROPCPaymentMethodsRefresh', function () {
            applyCheckoutClasses();
        });



        // Set mobile Ajax Filters opening button's position according to the distance between the page-title element and the top of the screen

        function setFiltersButtonPosition() {

            var filtersButton = $('.filters-button');
            
            if (filtersButton.length > 0) {

                var targetElement = $('.page-title');

                var setPosition = function () {
                    var targetOffset = targetElement.offset().top + parseInt(targetElement.css('padding-top'));
                    filtersButton.css('top', targetOffset);
                }
                setPosition();
                $(window).on('resize', setPosition);
            }
        }



        // Use character number to calculate width of select elements based on the seleced <option>. "Width" can't be used for this as <option>s width is always 100% of the longest one

        function setSearchCriteriaSize() {

            var targetElement = $('.search-box-select')
            var sizeOnLoad = targetElement.children('option:selected').text().length * 8; // approx 8px per character

            targetElement.on('change', function () {

                var sizeOnChange = targetElement.children('option:selected').text().length * 8; // approx 8px per character

                targetElement.css('width', sizeOnChange);
            }).css('width', sizeOnLoad);

        }



        // Set additional functionalities for Rich Blog (first post, post height, date container, comments link, navigation dropdowns)

        function handleRichBlogContent() {

            $('.richblog-widget .post').first().addClass('first');

            var richBlogPost = $('.richblog-pages-body .post');
            richBlogPost.first().addClass('first');

            function setMinimumHeight() {

                richBlogPost.each(function () {
                    var targetHeight = $(this).find('.rich-blog-image').height();
                    $(this).css('min-height', targetHeight);
                });
            }
            setMinimumHeight();
            $(window).on('resize', setMinimumHeight);

            richBlogPost.find('.post-date').wrap('<div class="wrapper"></div>');

            richBlogPost.find('.read-comments').each(function () {
                var targetElement = $(this).closest('.post').find('.post-date');
                $(this).detach().insertAfter(targetElement);
            });

            $('.richblog-pages-body .block').not('.blog-search-box').on('click', function () {

                if ($(window).outerWidth() >= themeSettings['responsive'][0]['breakpoint']) {

                    $(this).siblings().find('.active').slideUp().removeClass('active');
                    $(this).find('.listbox').slideToggle().toggleClass('active');
                }
            });
        }



        // Set additional class name for Smart Category Collections that have some subcategories listed

        function handleSCCsubcategories() {

            $(document).on('newProductsAddedToPageEvent', function () {

                $('.spc-categories').each(function () {
                    if ($(this).find('.category-sublist').length > 0) {
                        $(this).find('.category-info').addClass('full');
                    }
                });
            });
        }



        // FOOTABLE.js initialization ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        if (typeof $('body').footable == 'function') {
            $('.recurring-payments .data-table, .downloadable-products-page .data-table, .reward-points-history .data-table, .checkout-page .cart, .order-details-page .data-table, .return-request-page .data-table, .forum-table, .private-messages-page .data-table').footable();
        }
        if ($('.checkout-page').length > 0) {
            $(document).ajaxSuccess(function () {
                if ($('.order-summary-content .cart').length > 0) {
                    $('.order-summary-content .cart').footable();
                }
            });
        }

    }); // document.ready

    // workaround for wrong ui-tabs-nav integration on Product Details pages in Extended Gallery mode, and on Grouped Product pages. To be invoked outside document.ready

    $(document).on('quickTabsCreated', function () {
        $('.productTabs-tab ul').removeClass('ui-tabs-nav');
    });

    // Bypass perfect-scrollbar.js, no other way to disable it as it's invoked by the Core script. Should be removed from Head.cshtml first.
    $.fn.perfectScrollbar = function () { return };

})(jQuery);
