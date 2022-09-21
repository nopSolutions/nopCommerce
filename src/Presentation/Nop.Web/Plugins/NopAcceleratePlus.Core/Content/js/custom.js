function Customfunction() {

    //To Resolve the  QuickView Buttons Isuse
    //$.getScript("/Plugins/SevenSpikes.Nop.Plugins.QuickView/Scripts/QuickView.min.js", function () {
    //    // do something here after script has loaded
    //});

    //To Resolve the  Cloud Zoom Issue
    //$.getScript("/Plugins/SevenSpikes.Core/Scripts/cloudzoom.core.min.js", function () {
    //    // do something here after script has loaded
    //});

    //To Resolve the ProductRibbon Issue
    //$.getScript("/Plugins/SevenSpikes.Core/Scripts/sevenspikes.core.min.js", function () {
    //    // do something here after script has loaded
    //});
    //$.getScript("/Plugins/SevenSpikes.Nop.Plugins.ProductRibbons/Scripts/ProductRibbons.min.js", function () {
    //    // do something here after script has loaded
    //});

    //To Resolve the AjaxCart Issue
    //$.getScript("/Plugins/SevenSpikes.Nop.Plugins.AjaxCart/Scripts/AjaxCart.min.js", function () {
    //    // do something here after script has loaded
    //});
}

//1 If you are using the NopTemplate Theme please put this code inside condition:
//$(this).siblings('.filtersGroupPanel').slideToggle('slow');
//$('.title').show();

//2 If you are not using any nop Template theme, then comment this code
//var isAjax = $("#nopAccplus-facetdata").attr("data-ajaxrequest");
//if (screen.width >= 1025 || isAjax == "True") {}

$(document).on("click", ".filter-block .title", toggleFacet);

function toggleFacet() {
    //var isAjax = $("#nopAccplus-facetdata").attr("data-ajaxrequest");
    //var MobileViewScreenSize = parseInt($("#dimerce-facetdata").attr("data-mobileviewscreensize"));
    //if ($(window).width() >= MobileViewScreenSize || isAjax == "True") {
    //}
    $(this).siblings('.filtersGroupPanel').slideToggle('slow');
    $('.title').show();
    $(this).find('.acc_icon_expand').toggleClass('rotate');
}
