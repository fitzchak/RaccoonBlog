// JavaScript Document

(function() {
	
	$('#archive > ul > li > a').click(function (e) {
		e.preventDefault();
        if ($(this).parent().hasClass('open')) {
            $(this).parent().removeClass('open');
        } else {
			$('#archive .open').removeClass('open');
            $(this).parent().addClass('open');
        }
    });

	var bodyEl = document.body,
		openbtn = document.getElementById( 'open-button' ),
		showgrid = document.getElementById( 'gridView' ),
		openTags = document.getElementById( 'tags-button' ),
		openArch = document.getElementById( 'archive-button' ),
		isOpen = false;
		isGrid = false;
		isTags = false;
		isArchive = false;

	function init() {
		initEvents();
	}

	function initEvents() {
		if (openbtn.addEventListener) {
			
    		openbtn.addEventListener("click", toggleMenu, false);
			showgrid.addEventListener("click", toggleGrid, false);
			openTags.addEventListener("click", toggleTags, false);
			openArch.addEventListener("click", toggleArch, false);
			
		}
		else {
			
 	  		openbtn.attachEvent("onclick", toggleMenu);
			showgrid.attachEvent("onclick", toggleGrid);
			openTags.attachEvent("onclick", toggleTags);
			openArch.attachEvent("onclick", toggleArch);
		}
		
	}

	function toggleMenu() {
		if( isOpen ) {
			$(bodyEl).removeClass('show-menu');
			
		}
		else {
			$(bodyEl).addClass('show-menu');
		}
		isOpen = !isOpen;
	}
	
	function toggleGrid() {

		if( isGrid ) {
			$(bodyEl).removeClass('show-grid');
			
		}
		else {
			$(bodyEl).addClass('show-grid');
		}
		isGrid = !isGrid;
		adjustSize();
	}
	
		function toggleTags() {

		if( isTags ) {
			$('#tags').removeClass('open');
			
		}
		else {
			$('#archive').removeClass('open');
			$('#tags').addClass('open');
		}
		isTags = !isTags;
		isArchive = false;
	}
	
		function toggleArch() {

		if( isArchive ) {
			$('#archive').removeClass('open');
			
		}
		else {
			$('#tags').removeClass('open');
			$('#archive').addClass('open');
		}
		isArchive = !isArchive;
		isTags = false;
	}

	init();

})();

var adjustSize = function(){
	if ($(window).width() < 992) {
		$('.rightSide').css('min-height',0);		
	}
	else {
		$('.rightSide').css('min-height',$('.centerCol').height()+70);		
	}

	
}

window.onresize = function(event) {
    adjustSize();
}

$(document).ready(function(e) {
    adjustSize();
});

