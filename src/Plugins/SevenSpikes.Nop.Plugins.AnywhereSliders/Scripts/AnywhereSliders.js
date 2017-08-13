(function ($) {

    $(document).ready(function () {
        // Nivo Slider
        $('.anywhere-sliders-nivo-slider').each(function () {
            var that = $(this);
            var numberOfImages = parseInt(that.attr('data-imagesCount'));
            var nivoSliderId = that.attr('data-sliderHtmlElementId');
            var imagesString = that.attr('data-imagesString');
            var effect = that.attr('data-effect');
            var slices = parseInt(that.attr('data-slices')) || 15;
            var boxCols = parseInt(that.attr('data-boxCols')) || 8;
            var boxRows = parseInt(that.attr('data-boxRows')) || 4;
            var animSpeed = parseInt(that.attr('data-animSpeed')) || 500;
            var pauseTime = parseInt(that.attr('data-pauseTime')) || 5000;
            var directionNav = that.attr('data-directionNav') === 'true';
            var controlNav = that.attr('data-controlNav') === 'true';
            var controlNavThumbs = that.attr('data-controlNavThumbs') === 'true';
            var pauseOnHover = that.attr('data-pauseOnHover') === 'true';
            var prevText = that.attr('data-prevText');
            var nextText = that.attr('data-nextText');

            if (isNaN(numberOfImages) || numberOfImages <= 0) {
                return;
            }

            if (numberOfImages > 1) {
                $(window).load(function () {
                    $('#' + nivoSliderId).html(imagesString).nivoSlider({
                        effect: effect, // Specify sets like: 'fold,fade,sliceDown'
                        slices: slices, // For slice animations
                        boxCols: boxCols, // For box animations
                        boxRows: boxRows, // For box animations
                        animSpeed: animSpeed, // Slide transition speed
                        pauseTime: pauseTime, // How long each slide will show
                        startSlide: 0, // Set starting Slide (0 index)
                        directionNav: directionNav, // Next & Prev navigation
                        controlNav: controlNav, // 1,2,3... navigation
                        controlNavThumbs: controlNavThumbs, // Use thumbnails for Control Nav
                        pauseOnHover: pauseOnHover, // Stop animation while hovering
                        manualAdvance: false, // Force manual transitions
                        prevText: prevText, // Prev directionNav text
                        nextText: nextText, // Next directionNav text
                        randomStart: false, // Start on a random slide
                        afterLoad: function () {
                            $(document).trigger({ type: 'nopAnywhereSlidersFinishedLoading', targetId: nivoSliderId });
                        }
                    });
                });
            } else {
                $(document).trigger({ type: 'nopAnywhereSlidersFinishedLoading', targetId: nivoSliderId });
            }
        });

        // Carousel 2D Slider
        $('.anywhere-sliders-carousel2d-slider').each(function () {
            var that = $(this);
            var nivoSliderId = that.attr('data-sliderHtmlElementId');
            var width = parseInt(that.attr('data-width')) || 400;
            var height = parseInt(that.attr('data-height')) || 300;
            var navigation = that.attr('data-navigation') === 'true';
            var delay = parseInt(that.attr('data-delay')) || 3000;
            var links = that.attr('data-links') === 'true';
            var hoverPause = that.attr('data-hoverPause') === 'true';

            $('#' + nivoSliderId).coinslider({
                width: width,
                height: height,
                navigation: navigation,
                delay: delay,
                effect: 'straight',
                links: links,
                hoverPause: hoverPause
            });

            $('#' + nivoSliderId).swipeEvents().bind('swipeLeft', function () {
                $('.cs-prev').trigger('click');
            }).bind('swipeRight', function () {
                $('.cs-next').trigger('click');
            });
        });

        // Carousel 3D Slider
        $('.anywhere-sliders-carousel3d-slider').each(function () {
            var that = $(this);
            var nivoSliderId = that.attr('data-sliderHtmlElementId');
            var yRadius = parseInt(that.attr('data-yRadius')) || 40;
            var xPos = parseInt(that.attr('data-xPos')) || 270;
            var yPos = parseInt(that.attr('data-yPos')) || 40;
            var speed = parseFloat(that.attr('data-speed')) || 0.05;
            var mouseWheel = parseFloat(that.attr('data-mouseWheel')) || 0.0;
            var autoRotateDelay = parseInt(that.attr('data-autoRotateDelay')) || 4000;
            var autoRotate = that.attr('data-autoRotate') === 'true';

            $('#' + nivoSliderId).CloudCarousel({
                reflHeight: 0,
                reflGap: 2,
                titleBox: $('#pj_carousel_title'),
                reflOpacity: 0,
                altBox: $('#pj_carousel_alt'),
                buttonLeft: $('#but1'),
                buttonRight: $('#but2'),
                yRadius: yRadius,
                xPos: xPos,
                yPos: yPos,
                speed: speed,
                mouseWheel: mouseWheel,
                autoRotateDelay: autoRotateDelay,
                autoRotate: autoRotate
            });

            $('#' + nivoSliderId).swipeEvents().bind('swipeLeft', function () {
                $('#but1').trigger('mouseup');
            }).bind('swipeRight', function () {
                $('#but2').trigger('mouseup');
            });
        });

    });

})(jQuery);