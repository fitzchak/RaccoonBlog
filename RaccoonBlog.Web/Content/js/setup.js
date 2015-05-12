// JavaScript Document


// COOKIES 
function createCookie(name, value, days) {
    var expires;

    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toGMTString();
    } else {
        expires = "";
    }
    document.cookie = encodeURIComponent(name) + "=" + encodeURIComponent(value) + expires + "; path=/";
}

function readCookie(name) {
    var nameEQ = encodeURIComponent(name) + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return decodeURIComponent(c.substring(nameEQ.length, c.length));
    }
    return null;
}

function eraseCookie(name) {
    createCookie(name, "", -1);
}

// VARS

var bodyEl = document.body,
    openbtn = $('#open-button'),
    showgrid = $('#gridView'),
    showstack = $('#stackView'),
    openTags = $('#tags-button'),
    openArch = $('#archive-button'),
    openSeries = $('#seriesShow'),
    isOpen = false,
    isGrid = true,
    isTags = false,
    isArchive = false,
    isSeries = false;



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
	    if ($(window).width() > 768) {
	        if ($('.container').hasClass('visited')) {
	            $('.container').removeClass('visited');
	        } 
	    } else {
	        if (isOpen) {
			    $(bodyEl).removeClass('show-menu');

		    }
		    else {
			    $(bodyEl).addClass('show-menu');
		    }
	        isOpen = !isOpen;
	    }
	}

	

	function toggleStack() {
		if (isGrid) {
			$(bodyEl).removeClass('show-grid');
			$(showgrid).removeClass('active');
			$(showstack).addClass('active');
			isGrid = false;
		    createCookie('view', 'stack');
			adjustSize();
		}
	}
    
	function toggleGrid() {
	    if (!isGrid) {
	        $(bodyEl).addClass('show-grid');
	        $(showgrid).addClass('active');
	        $(showstack).removeClass('active');
	        isGrid = true;
	        createCookie('view', 'grid');
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
    
    
    $('.postsInSeries .morePosts').click(function (event) {
        event.preventDefault();
        if ($('.postsInSeries').attr('data-state') != 'open') {
            $('.postsInSeries ol').addClass('open');
            $('.postsInSeries').attr('data-state', 'open');
            $(this).html('hide');
        } else {
            $('.postsInSeries ol').removeClass('open');
            $('.postsInSeries').attr('data-state', 'closed');
            $(this).html('show all');
        }


    });

    $('#commentPreview').click(function (event) {
        var text = "";
        var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        for (var i = 0; i < 50; i++)
            text += possible.charAt(Math.floor(Math.random() * possible.length));

        var source = 'http://www.gravatar.com/avatar.php?gravatar_id=' + text + '&size=50&default=identicon';
        

        var monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
        
            
        var today = new Date();
        var dd = today.getDate();
        var mm = monthNames[today.getMonth()];
        var yyyy = today.getFullYear();
        var posttime = today.getHours() +':'+today.getMinutes();

        if(dd<10) {
            dd='0'+dd
        } 

        if(mm<10) {
            mm='0'+mm
        } 

        var currentTime =  dd+' '+mm+' '+yyyy+'<br/>'+posttime;

        $('.comment.preview time').html(currentTime);
        
        $('.comment.preview .avatar img').attr("src", source);
        $('.comment.preview .comment-body').html($('#Input_Body').val());
        $('.comment.preview .postedBy a').html($('#Input_Name').val());
        $('.comment.preview').addClass('active');
    });

    adjustSize();
    
    var visitCookieVaule = readCookie('newVisit');
    if (visitCookieVaule == 'visited') {
        $('.container').addClass('visited');
    } else {
        createCookie('newVisit', 'visited');
    }

    var viewCookieValue = readCookie('view');
    if (viewCookieValue == 'stack') {
        $(bodyEl).removeClass('show-grid');
        $(showgrid).removeClass('active');
        $(showstack).addClass('active');
        isGrid = false;
        createCookie('view', 'stack');
        adjustSize();

    } else {
        $(bodyEl).addClass('show-grid');
        $(showgrid).addClass('active');
        $(showstack).removeClass('active');
        isGrid = true;
        createCookie('view', 'grid');
        adjustSize();
     
    }
        
});