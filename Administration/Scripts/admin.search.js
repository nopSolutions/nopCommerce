var Admin = Admin || {};

Admin.Search = (function () {
    var itemTemplate = function (data) {
        var path = function () {
            if (data.grandParent) {
                return $("<p/>").text([data.grandParent, data.parent].join(" > ")).html();
            }
            else {
                return data.parent;
            }
        };
        var result = '<div id="user-selection">' +
            "<h5>" + data.title + "</h5>";
        if (path()) {
            result = result + path();
        }
        result = result + "</div>";
        return result;
    };

    var substringMatcher = function (enumerate) {
        var byRateAndTitle = function (a, b) {
            if (a.rate < b.rate)
                return 1;
            if (a.rate > b.rate)
                return -1;
            if (a.title < b.title)
                return -1;
            if (a.title > b.title)
                return 1;
            return 0;
        };

        return function findMatches(q, cb) {

            var matches = [];

            var substrRegex = new RegExp(q, "i");

            enumerate(function (item) {

                var rate = 0;
                var missKeyword = false;

                if (substrRegex.test(item.title)) {
                    rate += 10;
                }
                else if (item.node && substrRegex.test(item.node)) {
                    rate += 5;
                }
                else if (substrRegex.test(item.root)) {
                    rate += 1;
                } else {
                    missKeyword = true;
                }

                item.rate = rate;

                if (!missKeyword) {
                    matches.push(item);
                }
            });


            matches.sort(byRateAndTitle);
            cb(matches);
        };
    };

    return {
        init: function () {
            Admin.Navigation.initOnce();

            var $input = $(".admin-search-box");
            $input.blur(function (e) { e.preventDefault(); e.stopPropagation(); });
            $input.typeahead({ minLength: 2, highlight: true, hint: false },
            {
                name: "pages",
                displayKey: "name",
                templates: {
                    empty: [
                            '<div class="empty-message">',
                              "NO RESULTS",
                            "</div>"
                    ].join("\n"),
                    suggestion: itemTemplate
                },
                source: substringMatcher(Admin.Navigation.enumerate),
                limit: 10
            });

            var navigateTo = function (item) {
                $input.typeahead("val", "");
                Admin.Navigation.open(item.link);
            };

            $input.on("typeahead:selected", function (e, item) {
                navigateTo(item);
            });
        }
    };
})();
