$(window).on('load', function () {

    // uncomment below to use the ss scroll

    /*
    var element = $('.post-list').height();
    var elementWrapper = $('.post-list-wrap').height();

    var elementScroll = document.querySelector('.post-list-wrap');

    if (element > elementWrapper) {
        SimpleScrollbar.initEl(elementScroll);
    }
    */

    if (($('.social-feed').attr('data-ieoldversion') === 'True') && ($('.social-feed').length > 0)) {

        $('.social-feed').addClass("old-ie");

        var rtlvalue = $('.social-feed').attr("data-rtl") === 'True';

        $('.center-1 .old-ie .post-list, .center-2 .old-ie .post-list').masonry({
            itemselector: '.post-item',
            percentposition: true,
            originleft: !rtlvalue
        });
    }

});