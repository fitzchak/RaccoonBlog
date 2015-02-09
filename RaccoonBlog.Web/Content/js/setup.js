// JavaScript Document

(function () {

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
		openbtn = $('#open-button'),
		showgrid = $('#gridView'),
        showstack = $('#stackView'),
		openTags = $('#tags-button'),
		openArch = $('#archive-button'),
		openSeries = $('#seriesShow'),
		isOpen = false,
		isGrid = false,
		isTags = false,
		isArchive = false,
		isSeries = false;


	function init() {
		initEvents();
	}

	function initEvents() {
		openbtn.click(toggleMenu);
		showgrid.click(toggleGrid);
		showstack.click(toggleStack);
		openTags.click(toggleTags);
		openArch.click(toggleArch);
		openSeries.click(toggleSeries);
	}

	function toggleMenu() {
		if (isOpen) {
			$(bodyEl).removeClass('show-menu');

		}
		else {
			$(bodyEl).addClass('show-menu');
		}
		isOpen = !isOpen;
	}

	function toggleGrid() {
		if (!isGrid) {
			$(bodyEl).addClass('show-grid');
			$(showgrid).addClass('active');
			$(showstack).removeClass('active');
			isGrid = true;
			adjustSize();
		}
	}

	function toggleStack() {
		if (isGrid) {
			$(bodyEl).removeClass('show-grid');
			$(showgrid).removeClass('active');
			$(showstack).addClass('active');
			isGrid = false;
			adjustSize();
		}
	}

	function toggleSeries(e) {
		e.preventDefault();
		if (isSeries) {
			$('.postsInSeries').removeClass('open');

		}
		else {
			$('.postsInSeries').addClass('open');
		}
		isSeries = !isSeries;
	}

	function toggleTags() {
		if (isTags) {
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
		if (isArchive) {
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

var adjustSize = function () {
	if ($(window).width() < 992) {
		$('.rightSide').css('min-height', 0);
	}
	else {
		$('.rightSide').css('min-height', $('.centerCol').height() + 70);
	}
}

window.onresize = function (event) {
	adjustSize();
}

$(document).ready(function (e) {
	adjustSize();
});

