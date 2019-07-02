$(document).ready(function () {
    $('.sale-of-the-day-offer .owl-carousel').not('.initialized').each(function () {
        var that = $(this);

        if (that.find('.product-element').length > 1) {
            that.owlCarousel({
                rtl: $('.sale-of-the-day-offer.support-rtl').length > 0,
                loop: true,
                margin: 0,
                nav: true,
                items: 1,
                //autoHeight: true,
                autoplay: false,
                autoplayTimeout: 5000,
                autoplayHoverPause: true,
                onInitialized: function() {
                    that.addClass('initialized');
                }
            });
        }
    });
});