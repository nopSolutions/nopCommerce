(function ($) {
    $(document).ready(function () {

        var dependencies = [
            {
                module: "header",
                dependencies: ["attachDetach", "overlay", "perfectScrollbar"]
            },
            {
                module: "menu",
                dependencies: ["overlay","perfectScrollbar"]
            }
        ];

        var themeSettings = {
			
			// global settings
			
			/*stickyNavigation: {
				stickyElement: '.header-lower-inner',
				stickyElementParent: '.header-lower',
				showStickyOnFirstReverseScroll: false
			},*/
			header: {
				activeClass: 'active',
                modules: [
					{
						opener: '.responsive-nav-wrapper .search-button .trigger',
						closer: '.responsive-nav-head .close-search',
						content: '.store-search-box'
					},
					{
						opener: '.responsive-nav-wrapper .cart-button .trigger',
						closer: '.responsive-nav-head .close-cart',
						content: '.flyout-cart', // use class here, not ID !
						scrollbar: '#flyout-cart .items',
						preventClicking: true
					}
				]
			},
			flyoutCart: {
			   	flyoutCartSelector: '#flyout-cart',
            	flyoutCartScrollbarSelector: '#flyout-cart .items',
                removeItemSelector: '#flyout-cart .remove'
            },
			attachDetach: {
				blocks: [
					{
						elementToAttach: ".nav-button.search-button",
						content: ".store-search-box"
					},
					{
						elementToAttach: ".nav-button.cart-button",
						content: ".flyout-cart" // use class here, not ID !
					},
					{
						elementToAttach: ".one-column .filtersPanel",
						content: ".one-column .filterBox ~ .product-sorting"
					},
					{
						elementToAttach: ".one-column .filtersPanel",
						content: ".one-column .filterBox ~ .product-page-size"
					},
					{
						elementToAttach: ".blogpost-page .post-relations",
						content: ".blogpost-page .tags",
						insertAction: "prependTo"
					}
				]
			},
			toggle: {
				blocks: [
					{
						opener: '.write-review-button',
						content: '.write-review .review-form',
						activeClassOpener: 'open',
						animation: {
							type: 'slide',
							speed: 'slow'
						}
					},
					{
                        opener: '.box .box-title',
                        content: '.box .box-wrapper',
                        activeClassOpener: 'open',
                        animation: {
                            type: 'slide',
                            speed: 'slow'
                        }
                    },
					{
                        opener: '.write-comment-button',
                        content: '.new-comment form',
                        activeClassOpener: 'open',
                        animation: {
                            type: 'slide',
                            speed: 'slow'
                        }
                    }
				]
			},
			equalizer: {
				blocks: [
					{
						selector: '.address-box'
					},
					{
						selector: '.info-list.equalize'
					},
					{
						selector: '.variant-overview'
					},
					{
						selector: '.item-box .product-title'
					},
					{
						selector: '.news-list-homepage .news-head'
					},
					{
						selector: '.news-list-homepage .news-body'
					},
					{
						selector: '.information-box .image'
					},
					{
						selector: '.information-box .title'
					},
					{
						selector: '.information-box .description'
					},
					{
						selector: '.rich-blog-homepage .post-title'
					}
				]
			},
			
			// responsive settings
			
			responsive: [
				{
                    breakpoint: 1201,
                    settings: {
						header: {
                            activeClass: 'active',
                            modules: [
								{
                                    opener: '.header-logo-wrapper .menu-button',
                                    closer: '.responsive-nav-head .close-menu',
                                    content: '.header-menu',
									scrollbar: '.header-menu > ul',
                                    overlay: true,
									disablePageScroll: true
                                },
                                {
                                    opener: '.responsive-nav-wrapper .search-button',
									closer: '.responsive-nav-head .close-search',
									content: '.store-search-box',
                                    overlay: true,
									disablePageScroll: true
                                },
								{
									opener: '.responsive-nav-wrapper .personal-button',
									closer: '.responsive-nav-head .close-links',
									content: '.profile-links',
									scrollbar: '.profile-links .responsive-nav-body',
									overlay: true,
									disablePageScroll: true
                                },
								{
									opener: '.responsive-nav-wrapper .cart-button',
									preventClicking: true,
									closer: '.responsive-nav-head .close-cart',
									content: '.flyout-cart', // use class here, not ID !
									scrollbar: '#flyout-cart .items',
									overlay: true,
									disablePageScroll: true
                                },
								{
                                    opener: '.filters-button',
									closer: '.filtersHead .close-filters',
                                    content: '.nopAjaxFilters7Spikes',
									elementToAttach: '.master-wrapper-page',
									scrollbar: '.nopAjaxFilters7Spikes .filtersPanel',
                                    overlay: true,
									disablePageScroll: true 
                                }
                            ]
                        },
						menu: {
							closeMenuSelector: '.responsive-nav-head .close-menu',
							backButtonSelector: '.back-button .text',
							sublistIndent: {
								enabled: false
							}
						},
						flyoutCart: {
						   flyoutCartSelector: '#flyout-cart',
						   flyoutCartScrollbarSelector: '#flyout-cart .items',
						   removeItemSelector: '#flyout-cart .remove'
						},
						overlay: {
							overlayElementSelector: '.overlayOffCanvas',
							overlayClass: 'active',
							noPageScrollClass: 'scrollYRemove'
						},
						attachDetach: {
							blocks: [
								{
									elementToAttach: ".header",
									content: ".store-search-box"
								},
								{
									elementToAttach: ".header",
									content: ".flyout-cart" // use class here, not ID !
								},
								{
									elementToAttach: ".one-column .product-selectors",
									content: ".one-column .filterBox ~ .product-sorting"
								},
								{
									elementToAttach: ".one-column .product-selectors",
									content: ".one-column .filterBox ~ .product-page-size"
								},
								{
									elementToAttach: ".blogpost-page .post-relations",
									content: ".blogpost-page .tags",
									insertAction: "prependTo"
								}
							]
						},
						toggle: {
                            blocks: [
                                {
                                    opener: '.block .title',
                                    content: '.block .listbox',
                                    activeClassOpener: 'open',
                                    animation: {
                                        type: 'slide',
                                        speed: 'slow'
                                    }
                                },
								{
                                    opener: '.footer-block .title',
                                    content: '.footer-block .list',
									activeClassOpener: 'open',
                                    animation: {
                                        type: 'slide',
                                        speed: 'slow'
                                    }
                                },
								{
									opener: '.write-review-button',
									content: '.write-review .review-form',
									activeClassOpener: 'open',
									animation: {
										type: 'slide',
										speed: 'slow'
									}
								},
								{
									opener: '.box .box-title',
									content: '.box .box-wrapper',
									activeClassOpener: 'open',
									animation: {
										type: 'slide',
										speed: 'slow'
									}
								},
								{
									opener: '.write-comment-button',
									content: '.new-comment form',
									activeClassOpener: 'open',
									animation: {
										type: 'slide',
										speed: 'slow'
									}
								}
                            ]
                        },
						equalizer: {
                            blocks: [
                                {
                                    selector: '.cart .product'
                                },
                                {
                                    selector: '.cart .subtotal'
                                },
								{
                                    selector: '.address-box'
                                },
								{
									selector: '.info-list.equalize'
								},
								{
									selector: '.item-box .product-title'
								},
								{
									selector: '.information-box .image'
								},
								{
									selector: '.information-box .title'
								},
								{
									selector: '.information-box .description'
								},
								{
									selector: '.rich-blog-homepage .post-title'
								}
                            ]
                        }
                    }
					
					// define more breakpoints here if necessary.
					// all responsive settings defined for any breakpoint should be present also in every other breakpoint otherwise they will not work.
                }
			]
        };

        var theme = new window.sevenSpikesTheme(themeSettings, dependencies, false);

        theme.init();




		
		//   C U S T O M  S E T T I N G S
		
		
		
		// Mobile nav panels' height is not 100% of the screen so dynamic height calculation is needed for the scroll pane ("calc" doesn't work).
		
		function setMobileScrollPanes() {
		
			var scrollPane1 = $('.header-menu > ul');
			var scrollPane2 = $('.profile-links .responsive-nav-body');
			var scrollPane3 = $('.nopAjaxFilters7Spikes .filtersPanel');
			var scrollPane4 = $('.mini-shopping-cart .items');
			
			function setPaneHeight() {
			
				setTimeout( function() {
					var parentHeight1 = scrollPane1.parent().height();
					var parentHeight2 = scrollPane2.parent().height();
					var parentHeight3 = scrollPane3.parent().height();
					var parentHeight4 = scrollPane4.closest('.flyout-cart').height();

					if ($(window).outerWidth() <= 1200) {
						scrollPane1.css('max-height', parentHeight1 - 60);
						scrollPane2.css('max-height', parentHeight2 - 60);
						scrollPane3.css('max-height', parentHeight3 - 60);
						scrollPane4.css('max-height', parentHeight4 - 242);
					}
					else {
						scrollPane1.removeAttr('style');
						scrollPane2.removeAttr('style');
						scrollPane3.removeAttr('style');
						scrollPane4.removeAttr('style');
					}

				}, 500); // delay time should match css transition delay time
			}
			setPaneHeight();
			$(window).on('resize', setPaneHeight);
		}
		setMobileScrollPanes();
		
		
		
		
		
		// This sets equal minimum height for menu dropdowns and flyouts for better appearance. JS "Equalizer" can't be used for this. 
		
		function equalizeMenuSublistHeight() {
		
			setTimeout(function() {
			
				$('.header-menu .sublist .sublist').each( function() {
				
					var parentHeight = $(this).parent('.sublist-wrap').innerHeight();
					
					if ($(window).outerWidth() > 1200) {
						$(this).css('minHeight', parentHeight);
					}
					else {
						$(this).css('minHeight', '0');
					}
				});
			}, 500); // delay time should match css transition delay time
		}
		equalizeMenuSublistHeight();
		$(window).on('resize', equalizeMenuSublistHeight);
		
		
		
		
		
		// This updates the inner and outer quantity marker when a product is removed from the flyout cart.
		
		function updateFlyoutCart() {

			$(document).on('removeItemFlyoutCart', function() {

				var productsCountElement = $('#flyout-cart .count');
				var cartQtyElement = $('.cart-qty');

				if (productsCountElement.length && cartQtyElement.length) {
				
					var regex = /\d+/;
					var productsCountString = productsCountElement.text();
					var productsCount = productsCountString.match(regex) || [0];
		
					cartQtyElement.text(productsCount[0]);
				}
			});
		}
		updateFlyoutCart();
		
		
		
		
		
		// This is reserving space for category side navigation links depending on how long the "number" element is (if present).
		
		function reserveSpaceForSideNavLinks() {
		
			var availableWidth = 0;
			
			$('.block-category-navigation a').each( function() {
			
				availableWidth = $(this).width() - $(this).children('.number').width() - 16;
				$(this).children('.name').css('max-width', Math.floor(availableWidth));
			});
		}
		reserveSpaceForSideNavLinks();
		
		
		
		
		
		// This is recalculating bottom margin for "center-1" and "center-2" containers when a product grid or list is present.
		
		function handleCenter1And2Margin() {
		
			var targetElements = $('.center-1, .center-2');
			var currentMargin = targetElements.css('margin-bottom').replace('px','');
			
			if ($('.pager').length > 0) {
				return;
			}
			else if ($('.item-grid').length > 0) {
				targetElements.css('margin-bottom', currentMargin - 30);
			}
		}
		handleCenter1And2Margin();
		$(document).on('themeBreakpoint', function(event, settings, breakpoint) {
			$('.center-1, .center-2').removeAttr('style'); // removes inline css so the margin is recalulated correctly
			handleCenter1And2Margin();
		});
		
		
		
		
		
		// On mobile resolutions the product title is absolutely positioned so some space should be reserved for it depending on its height.
		
		function reserveSpaceForProductTitle() {
		
			var targetElement = $('.master-wrapper-content');
			var productTitle = $('.product-details-page .product-name');
			
			function setPadding() {
				if (productTitle.length > 0 && $(window).outerWidth() <= 1200) {
					targetElement.css('padding-top', productTitle.height() + 60);
				}
				else {
					targetElement.removeAttr('style');
				}
			}
			setPadding();
			$(window).on('resize', setPadding);
		}
		reserveSpaceForProductTitle();
		
		
		
		
		
		// Scrolls the screen to "product-variant-list" section on Grouped Product pages.
		
		function scrollToProductVariants() {
		
			$('.variants-scroll-button').on('click', function() {
				$('html, body').animate({
					scrollTop: $('.product-variant-list').offset().top - 30
				}, 1000);
			});
        }
		scrollToProductVariants();
		
		
		
		
		
		// On submit, forms collapsed by default will be automatically expanded if there is any error/success notification.
		
		function handleFormsWithErrors(errors, opener) {
		
            if ($(errors).length > 0) {			
                $(opener).click();
				$('html, body').animate({
					scrollTop: $(opener).offset().top
				}, 1000);
            }
        }
		handleFormsWithErrors(".write-review .message-error, .write-review .field-validation-error", ".write-review-button");
        handleFormsWithErrors(".new-comment .message-error, .new-comment .field-validation-error", ".write-comment-button");
		handleFormsWithErrors(".box.deals .message-failure, .box.deals .message-success", ".box .box-title");
		
		$(document).on("quickTabsRefreshedTab", function() {
            handleFormsWithErrors(".write-review .message-error, .write-review .field-validation-error", ".write-review-button");
        });
		
		
		
		
		
		// This is used to set up an additional class name depending on if there are 1 or 2 email filds in the form.
		
		function handleEmailRegistrationFieldsWidth() {
		
			var emailField = $('.email-field');
			if (emailField.length > 1) {
				emailField.addClass('half-width');
			}
		}
		handleEmailRegistrationFieldsWidth();
		
		
		
		
		
		// This is used to set up an additional class name on address lists when only one address is present.
		
		function detectNumberOfAddresses() {
		
			if ($('.address-list .address-item').length == 1) {
				$('.address-list').addClass('one-address');
			}
		}
		detectNumberOfAddresses();
		
		
		
		
		
		// This equalizes left and right columns' height on "Account pages" on desktop resolutions.
		
		function equalizeAccountColumnsHeight() {
		
			var targetHeight = $('.customer-pages-body .side-2').height();
			
			if ($(window).outerWidth() > 1200) {
				$('.customer-pages-body .center-2').css('min-height', targetHeight - 12);
			}
		}
		equalizeAccountColumnsHeight();
		$(document).on('themeBreakpoint', function(event, settings, breakpoint) {
			// "style" attribute is removed by the "handleCenter1And2Margin" function so no need to remove it from here too
			equalizeAccountColumnsHeight();
		});
		
		
		
		
		
		// This activates the custom "Clear Shopping Cart" button on Shopping Cart page.
		
		function clearShoppingCart() {
		
            $('.cart-options .clear-cart-button').on('click', function(e) {
                e.preventDefault();
                $('.cart [name="removefromcart"]').attr('checked', 'checked');
                $('.cart-options .update-cart-button').click();
            });
        }
		clearShoppingCart();
		
		
		
		
		
		// A workaround for closing "ui-dialog" popups from their screen overlay (as the overlay is a pseudo element, it can't be targeted with JS).
		
		function closePopupOnOverlayClick() {
		
			$(document).mousedown( function(e) {
				// detect left click ("1" = left click, "2" = middle click, "3" = right click)
				if (e.which == 1) {
					// "stopPropagation" will deactivate the "Close" button, so "target" is used instead
					if ($(e.target).closest('.ui-dialog-titlebar, .ui-dialog-content').length > 0) {
						return;
					}
					else {
						$('.ui-dialog-titlebar-close').click();
					}
				}
			});
		}
		closePopupOnOverlayClick();
		
		
		
		
		
		// expand~collapse behavior for the "New Address" form panel. Core script's "toggle" functionality can't be used for this.
		
		function toggleNewAddressForm() {
		
			var newAddressForm = $('.new-billing-address, .new-shipping-address');
			
			$('.enter-address-button').on('click', function() {
			
				$('.add-new .address-box').toggleClass('active');
				newAddressForm.toggleClass('active').fadeToggle();
				
				if (newAddressForm.hasClass('active')) {
					$('html, body').animate({
						scrollTop: newAddressForm.offset().top - 30
					}, 1000);
				}
			});
		}
		toggleNewAddressForm();
		
		
		
		
		
		// This provides additional class names for filter blocks when the Ajax Filters are in "dropdown" mode.
		
		function handleFilterDropdownPanels() {
		
			if($('.filtersDropDownPanel').length > 0) {
			
				$('.filter-block').each( function() {
					if(!$(this).hasClass('selected-options') && !$(this).hasClass('priceRangeFilterPanel7Spikes')) {
						$(this).addClass('dropdown-block');
					}
				});
				$('.filtersPanel .block:last').addClass('last');
			}
		}
		handleFilterDropdownPanels();
		
		
		
		
		
		// Dynamic top offset for jCarousel navigation arrows depending on the carousel image container height.
		
		function handleCarouselNavigation() {

			var setCarouselArrowsOffset = function() {
			
				$('.nop-jcarousel').each( function() {
					var imageHeight = $(this).find('.picture').height(); // all "picture" elements will always have the same height so we don't need a loop
					$(this).find('button').css('top', imageHeight/2);
				});		
			};
			setCarouselArrowsOffset();
			
			var windowResizeEnd; // recalculate position on window resize end
			
			$(window).on('resize', function() {
				clearTimeout(windowResizeEnd);
				windowResizeEnd = setTimeout( function() {
					setCarouselArrowsOffset();
				}, 100);
			});
    	}
		$(document).on('newProductsAddedToPageEvent', handleCarouselNavigation);
		
		
		
		
		
		// This calculates Mega Menu's dropdowns size and position. Css can't be used due to the relative postion of the parent list items.
		
		function handleMegaMenuDropdownSizeAndPosition() {
		
			var megaMenu = $('.mega-menu');
			var megaMenuItem = $('.with-dropdown-in-grid');
			var megaMenuDropdown = $('.mega-menu .dropdown');
			
			function setSizeAndPosition() {
			
				if ($(window).outerWidth() > 1200) {
					megaMenuDropdown.css('width', megaMenu.outerWidth());
				}
				else {
					megaMenuDropdown.css('width', 'auto');
				}

				megaMenuItem.each( function() {
				
					var thisItem = $(this);
					var thisDropdown = $(this).children('.dropdown');
					var dropdownOffset = megaMenu.offset().left - thisItem.offset().left;
					
					if ($(window).outerWidth() > 1200) {
						thisDropdown.css('left', dropdownOffset);
					}
					else {
						thisDropdown.css('left', 'auto');
					}
				});
			}
			setSizeAndPosition();
			$(window).on('resize', setSizeAndPosition);
		}
		handleMegaMenuDropdownSizeAndPosition();
		
		
		
		
		
		// Default "Sale of the Day" navigation cannot be adapted to theme's design, so an emulation is used instead.
		
		function replaceDefaultSaleOfTheDayNavigation() {
		
			var targetArea = $('.sale-of-the-day-offer');
			
			targetArea.find('.new-prev').on('click', function() {
				targetArea.find('.owl-prev').click();
			});
			targetArea.find('.new-next').on('click', function() {
				targetArea.find('.owl-next').click();
			});
		}
		replaceDefaultSaleOfTheDayNavigation();
		
		
		
		
		
		// "Sale of the Day" main image has different markup location for mobile and desktop resolutions. Core script's "attach/detach" module can't be used. 
		
		function handleSaleOfTheDayImagePlacement() {
		
			$('.sale-item').each( function() {
			
				var thisBoxImage = $(this).find('.item-picture');
				var placeholder1 = $(this).find('.item-gallery');
				var placeholder2 = $(this).find('.picture-thumbs');
				
				function reAttachImage() {
					if ($(window).outerWidth() > 1200) {
						thisBoxImage.prependTo(placeholder1);
					}
					else {
						thisBoxImage.prependTo(placeholder2);
					}
				}
				reAttachImage();
				$(document).on('themeBreakpoint', function(event, settings, breakpoint) {
					reAttachImage();
				});
			});
		}
		handleSaleOfTheDayImagePlacement();
		
		
		
		
		
		// Dynamic height for SCC Variant-2 category info boxes. It's linked to the height of the first product item box.
		
		function setCategoryInfoBoxHeight() {
		
			$('.variant-2 .spc-categories').each( function() {

				var targetHeight = $(this).find('.product-grid.active .item-box .picture').height(); // same height for each picture so we don't need a loop
				
				$(this).find('.category-info').height(targetHeight - 40); // excluding padding
			});
		}
		$(document).on('newProductsAddedToPageEvent', setCategoryInfoBoxHeight);
		$(window).on('resize', setCategoryInfoBoxHeight);
		
		
		
		
		
		// Sample code for targeting mobile or desktop states of the site on master breakpoint pass. Do not delete.
		
		/*$(document).on('themeBreakpoint', function(event, settings, breakpoint) {

			if(breakpoint !== null) {
				alert('mobile'); // mobile screen width reached
			}
			else {
				alert('desktop'); // desktop screen width reached
			}
		});*/
		
		
		
		
		
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
	
	
	
	// Loading Overlay panel behavior. It is closed on page load OR after 8 seconds (in case the page is not fully loaded).
	
	$(window).on('load', function () {
		$('.master-loading-overlay').hide();
	});
	setTimeout(function() {
		$('.master-loading-overlay').hide();
	}, 8000);
	
	// Adding top level class names for IE10 and IE11. Conditional comments can't be used to add dedicated css (not working for versions > 9).
	
	if (navigator.appVersion.indexOf('MSIE 10') !== -1) {
	  $('html').addClass('ie10');
	}
	if (!!window.MSInputMethodContext && !!document.documentMode) {
	  $('html').addClass('ie11');
	}
	
})(jQuery);
