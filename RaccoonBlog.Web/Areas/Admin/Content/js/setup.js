// JavaScript Document
$(document).ready(function(e) {
	
	
});

var adjustSize = function(){
	if ($(window).width() > 768) {
	
	}
	else {
		
	}

	
}

window.addEventListener('orientationchange',adjustSize, false);

window.onresize = function(event) {
    adjustSize();
}


$(document).ready(function(e) {
	adjustSize();
});

