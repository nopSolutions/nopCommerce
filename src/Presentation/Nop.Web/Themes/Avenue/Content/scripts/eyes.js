

// ===============
// eyes in logo
// ===============

//This is a pen based off of Codewoofy's eyes follow mouse. It is just cleaned up, face removed, and then made to be a little more cartoony. https://codepen.io/Codewoofy/pen/VeBJEP

var eyesInit = false;

document.querySelector('body').onmousemove = function(e) {
	// console.log(e, eyesInit);
	// console.log(document.body.clientWidth);
	var eyesContainer = document.querySelector(".eyes");
	var eyes = document.querySelectorAll(".eye");

	if (!eyesInit) {
		eyesContainer.classList.add('animation');
		if (document.body.clientWidth < 992) {
			eyesContainer.classList.add('animation-mobile');
		} else {
			eyesContainer.classList.add('animation');
		}
		eyesInit = true;
	}
	[].forEach.call(eyes, function(eye) {


		// do whatever
		var rect = eye.getBoundingClientRect();
		var x = (rect.left) + (eye.offsetWidth / 2);
		var y = (rect.top) + (eye.offsetHeight / 2);
		var rad = Math.atan2(e.pageX - x, e.pageY - y);
		var rot = (rad * (180 / Math.PI) * -1) + 180;

		eye.style.transform = 'rotate(' + rot + 'deg)'
	});
};
