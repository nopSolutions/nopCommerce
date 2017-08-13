///// <reference path="..\..\..\..\Scripts\jquery-1.10.2.js" />

(function() {
    $(document).ready(function () {
                
        var THEME_BREAKPOINT = 1000;

        // equal height element 
        $(window).on('resize, load', function () {
            
            if (sevenSpikes.getViewPort().width > 768) {

                var targetBlog = $(".rich-blog-homepage .blog-post");

                if (targetBlog.length == 2) {
                    equalBlogPostsHeight(targetBlog);
                } else {
                    $(targetBlog).css('width', '100%');
                }

                if (targetBlog.length == 3) {
                    $('.blog-post').addClass('threeInRow');
                }
                if (targetBlog.length > 1) {
                    equalBlogPostsHeight(targetBlog);
                }
            }
        });

 
         // parallax
        if ($('.home-page-wrapper .rich-blog-homepage').length > 0) {
            showVerticalParallaxForBlogPosts(THEME_BREAKPOINT);
        }  
    });

    // same height container 
    function equalBlogPostsHeight(group) {
        var tallest = 0;
        group.each(function () {
            var thisHeight = $(this).height();
            if (thisHeight > tallest) {
                tallest = thisHeight;
            }
        });
        group.height(tallest);
    }

    // vertical parallax
    function showVerticalParallaxForBlogPosts(breakPoint) {

        var blog = $('.rich-blog-homepage').offset().top;
        var blogHeight = $('.rich-blog-homepage').outerHeight();

        $(window).on('scroll', function () {
            if (sevenSpikes.getViewPort().width >= breakPoint) {
                var winTop = $(this).scrollTop();

                if (winTop > blog - blogHeight - 300) {
                    var animatedBg = $(".rich-blog-homepage");
                    var offsetMinusWinHeight = animatedBg.offset().top - winTop;
                    var start = 0;
                    var index = -0.9;
                    animatedBg.css({
                        'background-position': '50%' + (start + offsetMinusWinHeight) * index + 'px'
                    });

                    if (winTop > blogHeight) {
                        var target = $(".rich-blog-homepage .blog-posts");
                        var index1 = (target.offset().top) - ($(window).scrollTop() + 250);
                        var currentPos = 0;

                        target.css({
                            'position': 'relative',
                            'top': currentPos + index1 / 1.8 + 'px'
                        });
                    }
                    //both can be in one if condition (winTop > news - newsHeight)
                    if (winTop > blog - blogHeight) {
                        var newsCarousel = $('.news-carousel');
                        var startAnimation = (newsCarousel.offset().top) - (winTop + 800);
                        //newsCarousel.css('margin-top', newsCarousel / 2 + 'px');
                        newsCarousel.css({
                            'position': 'relative',
                            'top': startAnimation / 2.4 + 'px'
                        });
                    }
                }
            }
        });
    }

})();