let loader = {
	active: $(".loading").addClass("active"),
	inactive: $(".loading").removeClass("active")
};
$(document).ready((function () {
	loader.inactive;
	let e = {
		diffElement: function () {
//			$(".footer").children("div").last().append('<div class="Differnz-footer" style="display: block;">  <style>    .heart-icon svg {      height: 15px;      width: 15px    }  </style>  <span style="display: block;"> Designed with <span class="heart-icon" style="display: inline;">      <svg xmlns="http://www.w3.org/2000/svg" version="1.1" xmlns:xlink="http://www.w3.org/1999/xlink" xmlns:svgjs="http://svgjs.com/svgjs" width="512" height="512" x="0" y="0" viewBox="0 0 512 512" style="enable-background:new 0 0 512 512" xml:space="preserve">        <g>          <path fill="#ff343b" d="M449.28 121.43a115.2 115.2 0 0 0-137.89-35.75c-21.18 9.14-40.07 24.55-55.39 45-15.32-20.5-34.21-35.91-55.39-45a115.2 115.2 0 0 0-137.89 35.75c-16.5 21.62-25.22 48.64-25.22 78.13 0 42.44 25.31 89 75.22 138.44 40.67 40.27 88.73 73.25 113.75 89.32a54.78 54.78 0 0 0 59.06 0c25-16.07 73.08-49.05 113.75-89.32 49.91-49.42 75.22-96 75.22-138.44 0-29.49-8.72-56.51-25.22-78.13z" data-original="#f9595f"></path>        </g>      </svg>    </span> by <a href="https://www.differenzsystem.com/" style="display: inline;">Differenz System LLC</a>  </span></div>')
		},
		diffTimer: function () {
			setInterval((function () {
				$(".footer").find(".Differnz-footer").length <= 0 && e.diffElement()
			}), 6e4)
		}
	};
	e.diffElement(), e.diffTimer()
}));

$("form").submit((function (a) {
	loader.active;
}));
function displayAjaxLoading(display) {
	if (display) {
		loader.active;
	}
	else {
		loader.inactive;
	}
}