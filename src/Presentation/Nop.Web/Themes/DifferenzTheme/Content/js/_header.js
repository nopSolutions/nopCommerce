var langArray = [];
$(".vodiapicker option").each((function () {
	var a = $(this).attr("data-thumbnail"),
		n = this.innerText,
		e = ' <li><img src="' + a + '" alt="" value="' + $(this).val() + '"/><span>' + n + "</span></li>";
	langArray.push(e)
})), $("#lang-none").html(langArray), $(".btn-select").html(langArray[0]), $(".btn-select").attr("value", "en"), $("#lang-none li").click((function () {
	var a = $(this).find("img").attr("src"),
		n = $(this).find("img").attr("value"),
		e = '<li><img src="' + a + '" alt="" /><span>' + this.innerText + "</span></li>";
	$(".btn-select").html(e), $(".btn-select").attr("value", n), $(".lang-all").toggle(), window.location.href = n
})), $(".btn-select").click((function () {
	$(".lang-all").toggle()
}));
var sessionLang = localStorage.getItem("lang");
if (sessionLang) {
	var langIndex = langArray.indexOf(sessionLang);
	$(".btn-select").html(langArray[langIndex]), $(".btn-select").attr("value", sessionLang)
} else langIndex = langArray.indexOf("ch"), console.log(langIndex), $(".btn-select").html(langArray[langIndex]);
$(document).ready((function () {
	$(".header").on("mouseenter", "#topcartlink", (function () {
		$("#flyout-cart").addClass("active")
	})), $(".header").on("mouseleave", "#topcartlink", (function () {
		$("#flyout-cart").removeClass("active")
	})), $(".header").on("mouseenter", "#flyout-cart", (function () {
		$("#flyout-cart").addClass("active")
	})), $(".header").on("mouseleave", "#flyout-cart", (function () {
		$("#flyout-cart").removeClass("active")
	})), $(".owl-carousel2").owlCarousel({
		loop: !0,
		margin: 20,
		nav: !0,
		responsive: {
			0: {
				items: 1
			},
			500: {
				items: 2
			},
			991: {
				items: 3
			},
			1200: {
				items: 4
			}
		}
	})

}));

