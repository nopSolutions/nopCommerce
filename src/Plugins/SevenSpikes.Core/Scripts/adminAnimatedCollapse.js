$(document).ready(function () {
    $(".controlTitle a.toggleControl").each(function () {
        $(this).click(function (eventObject) {

            var currentElement = $(eventObject.currentTarget);

            if (currentElement.attr("class") == "toggleControl") {
                currentElement.attr("class", "toggleControl closed");

                animatedcollapse.toggle($(currentElement).parent().siblings(".adminGroupPanel").attr("id"));
            } else if (currentElement.attr("class") == "toggleControl closed") {
                currentElement.attr("class", "toggleControl");

                animatedcollapse.toggle($(currentElement).parent().siblings(".adminGroupPanel").attr("id"));
            }
        });

    });
});