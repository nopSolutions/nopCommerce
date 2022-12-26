var homePage = document.documentElement;
var bodyPage = document.getElementsByTagName("body")[0];
var sidePanel = $(".categories-in-side-panel");
var sidebar = $(".header-menu");
var header;
var content;
var subContent = $(".header-menu .sublist-wrap");
var selectCategory;
var nextButton = $(".header-menu .plus-button");
var backButton;
var closeButton = $(".header-menu .close-menu");

var index = 0;
var heightArray = new Array();
var imageArray = new Array('Default.png', 'HomeButton.png', 'LocationsButton.png', 'ClearanceTagButton.png', 'SaleAdButton.png', 'HawthStore.png', 'HawthWeeklyAD.png', 'HawthHome.png', 'BackButtonABCclearance.png', 'BackButtonHAWclearance.png', 'CreditIcon.png');
var element;
var elementArray = new Array();
var categoryArray = new Array();
categoryArray[0] = "SHOP ALL CATEGORIES";

$(document).ready(function () {
    var el = sidebar.find(".mega-menu-responsive");
    $(el[0]).addClass("mobile-sidebar-category first");
    $(el[1]).addClass("mobile-sidebar-category second");
    var str = '<div class="mobile-sidebar-title"><div class="back-button"> < Back </div><div id="select_category_item">' + categoryArray[0] + '</div></div>';
    sidebar.append(str);

    header = $(".header-menu .mobile-sidebar-category.first");
    content = $(".header-menu .mobile-sidebar-category.second");
    selectCategory = $(".header-menu #select_category_item");
    backButton = $(".mobile-sidebar-title .back-button");

    effectBack();

    heightArray[index] = getHeight($(".mobile-sidebar-category.second > li"), "F");

    content.height(heightArray[index]);

    nextButton.click(function () {
        heightArray[index + 1] = getHeight($(this).next().find(".sublist:eq(0)>li"), "O");
        if (heightArray[index] > heightArray[index + 1]) {
            $(this).next().css("height", heightArray[index]);
            content.height(heightArray[index]);
        } else {
            $(this).next().css("height", heightArray[index + 1]);
            content.height(heightArray[index + 1]);
        }

        backButton.removeClass('hidden');
        selectCategory.text($(this).parent().find('a span').first().text());

        categoryArray[index + 1] = $(this).parent().find('a span').first().text();
        elementArray[index] = this;
        index++;
        sidebar.animate({ scrollTop: 0 }, 500);
    });


    backButton.click(function () {
        if (sidebar.find('.active').length > 0) {
            index--;

            if (heightArray[index] > heightArray[index - 1]) {
                $(elementArray[index]).next().css("height", heightArray[index]);
                content.height(heightArray[index]);
            } else {
                $(elementArray[index]).next().css("height", heightArray[index - 1]);
                content.height(heightArray[index - 1]);
            }

            selectCategory.text(categoryArray[index]);

            if (sidebar.find('.active').length == 1) {
                content.height(heightArray[0]);
                sidebar.find('.sublist-wrap').removeClass('active');
            } else {
                $(elementArray[index]).next().removeClass('active');
            }
        }
        effectBack();
    });

    menuSetting();

    removeNonLeafLinks();

    $(".link-sub").click(function () {
        $(this).next().click();
    });

    closeButton.click(function () {
        subContent.removeClass("active");
    });
});

function toggleMobileMenu() {
    const isMobileMenuOpen = $(".categories-in-side-panel.open").length === 1;
    if (isMobileMenuOpen) {
        closeMobileMenu();
    } else {
        openMobileMenu();
    }
}

function openMobileMenu() {
    homePage.classList.add("scrollYRemove");
    bodyPage.classList.add("scrollYRemove");
    sidePanel.addClass("open");
    index = 0;
    heightArray[index] = getHeight($(".mobile-sidebar-category.second > li"), "F");
    var overlayCanvas = document.getElementsByClassName("overlayOffCanvas")[0];
    overlayCanvas.classList.add("show");
    overlayCanvas.style.display = "block";
    selectCategory.text(categoryArray[index]);
    backButton.addClass('hidden');
    sidebar.animate({ scrollTop: 0 }, 500);
    $('.overlayOffCanvas.show').on({ 'touchend': function () { closeButton.click(); } });
    sidebar.on({ 'touchstart': function () { $("html,body").scrollTop(50); } });
    sidebar.on({ 'touchmove': function () { $("html,body").scrollTop(50); } });
    sidebar.on({ 'touchend': function () { $("html,body").scrollTop(50); } });
}

function closeMobileMenu() {
    homePage.classList.remove("scrollYRemove");
    bodyPage.classList.remove("scrollYRemove");
    sidePanel.removeClass("open");
    var overlayCanvas = document.getElementsByClassName("overlayOffCanvas")[0];
    overlayCanvas.classList.remove("show");
    overlayCanvas.style.display = "none";
}

function effectBack() {
    if (sidebar.find('.active').length == 0) {
        backButton.addClass('hidden');
    } else {
        backButton.removeClass('hidden');
    }
}

function menuSetting() {
    var menu_array = header.find('li');
    var len = menu_array.length;
    var width = 100 / len + '%';
    var path = '';
    var storeFlag = "abc";
    for (var i = 0; i < len; i++) {
        if ($(menu_array[i]).find("a[href*='hawthorne']").length == 1) {
            storeFlag = "haw";
            break;
        }
    }
    for (var i = 0; i < len; i++) {
        const isAbc = storeFlag == "abc";
        var str = $(menu_array[i]).find('span').text().trim();
        var img = "";
        if (str == "Home") {
            if (storeFlag == "abc") {
                path = 'url(/Plugins/Misc.AbcFrontend/Images/' + imageArray[1] + ')';
            } else {
                path = 'url(/Plugins/Misc.AbcFrontend/Images/' + imageArray[7] + ')';
            }
        } else if (str == "Home Page") {
            path = 'url(/Plugins/Misc.AbcFrontend/Images/' + imageArray[7] + ')';
        } else if (str == "Locations") {
            if (storeFlag == "abc") {
                path = 'url(/Plugins/Misc.AbcFrontend/Images/' + imageArray[2] + ')';
            } else {
                path = 'url(/Plugins/Misc.AbcFrontend/Images/' + imageArray[5] + ')';
            }
        } else if (str == "Clearance") {
            path = 'url(/Plugins/Misc.AbcFrontend/Images/' + imageArray[3] + ')';
        } else if (str == "Sale Ad") {
            if (storeFlag == "abc") {
                path = 'url(/Plugins/Misc.AbcFrontend/Images/' + imageArray[4] + ')';
            } else {
                path = 'url(/Plugins/Misc.AbcFrontend/Images/' + imageArray[6] + ')';
            }
        } else if (str == "Store Locations") {
            img = isAbc ? imageArray[2] : imageArray[5];
            path = 'url(/Plugins/Misc.AbcFrontend/Images/' + img + ')';
        } else if (str == "Financing") {
            img = imageArray[10];
            path = 'url(/Plugins/Misc.AbcFrontend/Images/' + img + ')';
        } else if (str == "Weekly Ad") {
            path = 'url(/Plugins/Misc.AbcFrontend/Images/' + imageArray[6] + ')';
        } else if (str.indexOf("Back") !== -1) {
            img = isAbc ? imageArray[8] : imageArray[9];
            path = 'url(/Plugins/Misc.AbcFrontend/Images/' + img + ')';
        } else {
            path = 'url(/Plugins/Misc.AbcFrontend/Images/' + imageArray[0] + ')';
        }
        $(menu_array[i]).find('a').css('background-image', path);
        $(menu_array[i]).css('width', width);
    }
    $(menu_array[len]).append("<div class='phone-line'></div>");
    $(menu_array[len]).find('span').css('line-height', '0px');

    hardcodeForMickeyShorr(menu_array);
}

// Hardcodes for Mickey Shorr since menu items don't match desired outcome.
function hardcodeForMickeyShorr(menu_array) {
    debugger;
    const isMickeyShorr = window.location.href.indexOf("mickeyshorr") > -1;
    if (!isMickeyShorr) { return; }

    $(menu_array[0]).css('width', "33.3%");
    $(menu_array[0]).find('a').css('background-image', "url('/Plugins/Misc.AbcFrontend/Images/LocationsButton.png')");
    $(menu_array[0]).find('a').attr("href", '/AllShops')
    $(menu_array[0]).find('a').find('span').text('Locations')
    $(menu_array[1]).css('width', "33.3%");
    $(menu_array[1]).find('a').css('background-image', "url('/Plugins/Misc.AbcFrontend/Images/HawthWeeklyAD.png')");
    $(menu_array[1]).find('a').attr("href", '/sale-ad-3')
    $(menu_array[1]).find('a').find('span').text('Sale Ad')
    $(menu_array[2]).css('width', "33.3%");
    $(menu_array[2]).find('a').css('background-image', "url('/Plugins/Misc.AbcFrontend/Images/CreditIcon.png')");
    $(menu_array[2]).find('a').attr("href", '/special-financing-options-2')
    $(menu_array[2]).find('a').find('span').text('Financing')
    menu_array.length = 3;
}

function removeNonLeafLinks() {
    var links = sidebar.find(".mobile-sidebar-category .has-sublist");
    for (var j = 0; j < links.length; j++) {
        $(links[j]).find("a:eq(0)").removeAttr("href");
        $(links[j]).find("a:eq(0)").addClass("link-sub");
    }
}

function getHeight(el, flag) {
    var h = 0;
    if (flag == "F") {
        for (var i = 0; i < el.length; i++) {
            h += $(el[i]).height();
        }
    } else if (flag == "O") {
        for (var i = 1; i < el.length; i++) {
            h += $(el[i]).height();
        }
    }
    return h;
}
