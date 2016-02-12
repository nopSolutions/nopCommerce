var Admin = Admin || {};

Admin.Navigation = (function () {
    var buildMap = function () {
        var map = {};
        map['/Admin'] = { root: 'nopCommerce Admin', node: null, title: 'Dashboard', url: '/Admin' }

        $('.header-menu > ul > li').each(function () {

            if ($(this).children('ul').length != 0) {

                var rootTitle = $(this).clone().children().remove().end().text();

                $(this).children('ul').first().children('li').each(function () {

                    if ($(this).children('a').length > 0) {

                        var linkTitle = $(this).children('a').first().text();

                        var href = $(this).children('a').first().attr('href');

                        map[href] = { root: rootTitle, node: null, title: linkTitle, url: href};

                    } else {

                        var nodeTitle = $(this).clone().children().remove().end().text();

                        $(this).children('ul').first().children('li').each(function () {

                            var linkTitle = $(this).children('a').first().text();

                            var href = $(this).children('a').first().attr('href');


                            map[href] = { root: rootTitle, node: nodeTitle, title: linkTitle, url: href };

                        });
                    }
                });
            }
        });

        return map;
    };

    var map;
   
    var init = function () {
        map = buildMap();
    };
    var events = {};
    return {
        enumerate: function (callback) {
            for (var url in map) {
                var node = map[url];
                callback.call(node, node);
            }
        },
        open: function (url) {
            if (events['open']) {
                var event = $.Event('open', { url: url });
                events['open'].fire(event);
                if (event.isDefaultPrevented())
                    return;
            }
            window.location.href = url;
        },
        
        initOnce: function () {
            if (!map)
                init();
        },
        init: init
    };
})();
