var Admin = Admin || {};

Admin.Navigation = (function () {
    var buildMap = function () {
        var map = {};
        
        var linkElements = $("a.menu-item-link");

        linkElements.each(function () {
            var parents = $(this).parentsUntil(".sidebar-menu");
            var href;
            var title;
            var parent;
            var grandParent;
            switch (parents.length) {
                // items in level one
                case 1:
                    {
                        href = $(parents).find("a").attr("href");
                        title = $(parents).find("a").find("span").html();
                        map[href] = { title: title, link: href, parent: null, grandParent: null };

                        break;
                    }
                    // items in level two, these items have parent but have not grand parent
                case 3:
                    {
                        href = $(parents).eq(0).find("a").attr("href");
                        title = $(parents).eq(0).find("a").find("span").html();
                        parent = $(parents).eq(2).find("a").find("span").html();
                        map[href] = { title: title, link: href, parent: parent, grandParent: null };

                        break;
                    }
                    // items in level three, these items have both parent and grand parent
                case 5:
                    {
                        href = $(parents).eq(0).find("a").attr("href");
                        title = $(parents).eq(0).find("a").find("span").html();
                        parent = $(parents).eq(2).find("a").find("span").html();
                        grandParent = $(parents).eq(4).find("a").find("span").html();
                        map[href] = { title: title, link: href, parent: parent, grandParent: grandParent };
                        break;
                    }
                default: break;
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
            if (events["open"]) {
                var event = $.Event("open", { url: url });
                events["open"].fire(event);
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
