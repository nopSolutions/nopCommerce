(function($) {
    //Attach this new method to jQuery

    $.fn.extend({

        //This is where you write your plugin's name
        champ: function(options) {
            //Iterate over the current set of matched elements

            var defaults = {
                selector: "tab_wrapper",
                plugin_type: "tab",
                side: "",
                //responsive: "true",
                active_tab: "1",
                controllers: "false",
                ajax: "false",
                multiple_tabs: "false",
                show_ajax_content_in_tab: "false",
                content_path: "false"
            }


            var obj = $.extend(defaults, options);

            var li_rel = 1,
                div_rel = 1;

            return this.each(function() {

                var plugin_type = obj.plugin_type;
                var side = obj.side;
                var active_tab = obj.active_tab;
                var controllers = obj.controllers;
                //var responsive = obj.responsive;
                var multiple_tabs = obj.multiple_tabs;
                var ajax = obj.ajax;
                var show_ajax_content_in_tab = obj.show_ajax_content_in_tab;
                var content_path = obj.content_path;

                var tab_content_selector = $(this).find(" > div > div.tab_content");
                var tab_selector = $(this).find(" >ul li");
                var parent = $(this);
                var controller_parent = $(".controller").closest(".tab_wrapper");
                var con_siblings;


                if (side != "") {
                    parent.addClass(side + "_side");
                }

                if (controllers == "true") {
                    parent.addClass("withControls");
                    parent.append("<div class='controller'><span class='previous'>previous</span><span class='next'>next</span></div>");
                }

                if (plugin_type == "accordion") {
                    //alert(34);
                    parent.addClass("accordion");
                    parent.removeClass(side + "_side");
                    parent.removeClass("withControls");
                    $(".controller", parent).remove();
                }

                if (ajax == "true") {
                    $.ajax({
                        url: content_path,
                        success: function(result) {
                            $(" .tab_content.tab_" + show_ajax_content_in_tab, parent).html(result);
                        }
                    });

                    $(document).ajaxError(function(event, request, settings) {
                        $(" .tab_content.tab_" + show_ajax_content_in_tab, parent).prepend("<h4 class='error'>Error requesting page " + settings.url + "</h2>");
                    });
                }


                function hide_controls(parent) {
                    if (parent.find(" >ul li:eq(0)").hasClass("active")) {
                        $(".controller .previous", parent).hide();
                    } else {
                        $(".controller .previous", parent).show();
                    }

                    if (parent.find(" >ul li").last().hasClass("active")) {
                        $(".controller .next", parent).hide();
                    } else {
                        $(".controller .next", parent).show();
                    }
                }

                $(".controller .previous", $(this)).click(function() {
                    con_siblings = $(this).closest(".controller");
                    con_siblings.siblings("ul").find("li.active").prev().trigger("click");
                    hide_controls(controller_parent);

                });

                $(".controller .next", $(this)).click(function() {
                    con_siblings = $(this).closest(".controller");
                    con_siblings.siblings("ul").find("li.active").next().trigger("click");
                    hide_controls(controller_parent);

                });

                $(this).find(" >ul li").removeClass("active");
                $(this).find(" > div > div.tab_content").removeClass("active");

                if (active_tab == "") {
                    $(this).find(" >ul li:eq(0)").addClass("active").show(); //set active tab on load   
                    $(this).find(" > div > div.tab_content:eq(0)").addClass("active").show(); //set active tab on load   
                    hide_controls(parent);

                } else {
                    $(this).find(" >ul li:eq(" + (active_tab - 1) + ")").addClass("active").show(); //set active tab on load
                    $(this).find(" > div > div.tab_content:eq(" + (active_tab - 1) + ")").addClass("active").show(); //set active tab on load
                    hide_controls(parent);
                }

                tab_content_selector.first().addClass("first");
                tab_content_selector.last().addClass("last");


                // add class to content div
                tab_content_selector.each(function() {
                    var tab_count = $(this).parents(".tab_wrapper").length;
                    var add_relation = "tab_" + tab_count + "_" + div_rel;
                    //var add_relation = "tab_" + div_rel;
                    $(this).addClass(add_relation);
                    $(this).attr("title", add_relation);
                    div_rel++;
                });


                if (multiple_tabs == "true") {
                    var get_parent = $(this).closest(".tab_wrapper");
                    var active_tab_text = $(this).find(" >ul li:eq(0)").text();
                    get_parent.addClass("show-as-dropdown");
                    get_parent.prepend("<div class='active_tab'><span class='text'>" + active_tab_text + "</span><span class='arrow'></span></div>");
                }

                $(".active_tab").click(function() {

                    $(this).next().stop(true, true).slideToggle();
                });

                // add relation attr to li and generate accordion header for mobile

                //if (responsive == "true") {
                tab_selector.each(function() {
                    var tab_count = $(this).parents(".tab_wrapper").length;
                    var add_relation = "tab_" + tab_count + "_" + li_rel;

                    var accordian_header = $(this).text();
                    //var add_relation = "tab_" + li_rel;
                    var get_parent = $(this).closest(".tab_wrapper");
                    $(this).attr("rel", add_relation);

                    var current_tab_class = $(this).attr("class");

                    tab_content_selector.each(function() {
                        if ($(this).hasClass(add_relation)) {
                            get_parent.find(" > div > div.tab_content." + add_relation).before("<div title='" + add_relation + "' class='accordian_header " + add_relation + ' ' + current_tab_class + "'>" + accordian_header + "<span class='arrow'></span></div>");
                        }
                    });
                    li_rel++;
                });
                // }


                // on click of accordion header slideUp/SlideDown respective content
                $(".accordian_header").click(function() {
                    var clicked_header = $(this).attr("title");
                    var content_status = $(this).next(".tab_content").css("display");
                    var get_closest_parent = $(this).closest(".tab_wrapper");

                    if (content_status == "none") {
                        get_closest_parent.find(">.content_wrapper >.accordian_header").removeClass("active");
                        $(this).addClass("active");
                        get_closest_parent.find(">ul >li").removeClass("active");
                        get_closest_parent.find(">ul >li[rel='" + clicked_header + "']").addClass("active");

                        tab_content_selector.removeClass("active").stop(true, true).slideUp();
                        get_closest_parent.find(" > div > div.tab_content." + clicked_header).addClass("active").stop(true, true).slideDown();
                    } else {
                        get_closest_parent.find(">.content_wrapper >.accordian_header").removeClass("active");
                        $(this).removeClass("active");
                        get_closest_parent.find(">ul >li").removeClass("active");
                        get_closest_parent.find(" > div > div.tab_content." + clicked_header).removeClass("active").stop(true, true).slideUp();
                    }
                });

                // on click of tab hide/show respective content
                tab_selector.click(function() {

                    var clicked_tab = $(this).attr("rel");
                    var get_new_closest_parent = $(this).closest(".tab_wrapper");
                    var get_closest_tab_list = $(this).closest(".tab_list");
                    get_closest_tab_list.next(".content_wrapper").find(" >.accordian_header").removeClass("active");

                    get_new_closest_parent.find(".accordian_header." + clicked_tab).addClass("active");

                    tab_content_selector.removeClass("active").hide();
                    get_new_closest_parent.find(" > div > div.tab_content." + clicked_tab).addClass("active").show();
                    tab_selector.removeClass("active");
                    $(this).addClass("active");
                    hide_controls(get_new_closest_parent);
                    var winWidth = $(window).width();

                    if (multiple_tabs == "true") {
                        if ($(this).parent(".tab_list").parent(".show-as-dropdown")) {
                            var selected_tab_text = $(this).text();
                            $(".active_tab .text").text(selected_tab_text);
                        }
                        if (winWidth <= 768) {

                            $(this).closest(".tab_list").stop(true, true).slideUp();
                        }
                    }
                });

            });
        }
    });

})(jQuery);
