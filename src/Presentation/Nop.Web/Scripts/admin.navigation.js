var Admin = Admin || {};

Admin.Navigation = (function () {
    var buildMap = function () {
        var map = {};
        $('.sidebar-menu a.menu-item-link').each(function () {
            var linkTitle = $(this).children('.menu-item-title').text();
            var href = $(this).attr('href');
            map[href] = { root: linkTitle, node: null, title: linkTitle, url: href };
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
