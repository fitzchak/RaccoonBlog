
// Common
String.prototype.startsWith = function (str) {
    return this.substr(0, str.length) === str;
};
String.prototype.isNullOrEmpty = function () {
    return this == false || this === '';
};

(function (window, undefined) {
    var Raccoon = {};
    Raccoon.Util = {};
    Raccoon.Util.Url = {};
    Raccoon.Util.Url.toAbsolute = function (url) {
        if (url.isNullOrEmpty()) return null;
        if (url.startsWith("http://") || url.startsWith("https://")) return url;
        return "http://" + url;
    };

    Raccoon.Util.Views = {};
    Raccoon.Util.Views.setMessage = function setMessage(message, cssClass) {
        var m = $('#message').html(message);
        if (cssClass) m.removeClass().addClass(cssClass);
        m.css('visibility', 'visible');
    };
    
    Raccoon.Views = {};
    Raccoon.Views.Details = function() {
        function insertComment() {
            if ($('.livecomment').length > 0) return;

            var now = new Date();
            var comment = {
                author: $('article#postComment input[name$="Name"]').val(),
                emailHash: $.md5($('article#postComment input[name$="Email"]').val()),
                url: $('article#postComment input[name$="Url"]').val(),
                body: converter.makeHtml($('article#postComment textarea[name$="Body"]').val()),
                createdAt: now.f("MM/dd/yyyy HH:mm")
            };
            $('#commentTemplate').tmpl(comment).appendTo('section.comments').show('medium');
        };
        
        var $preview = null;
        var converter = new Showdown.converter();
        $('textarea[name$="Body"]').keydown(function () {
            if ($preview == null) {
                insertComment();
                $preview = $('.livecomment .comment-body');
            }
            $preview.html(converter.makeHtml($(this).val()));
        });
        var $email = null;
        $('input[name$="Email"]').keyup(function () {
            if ($email == null) {
                insertComment();
                $email = $('.livecomment .avatar');
            }
            $email.attr('src', 'http://www.gravatar.com/avatar.php?gravatar_id=' + $.md5($(this).val()) + '&size=50&default=identicon');
        });
        var $name = null;
        var name = null;
        $('input[name$="Name"]').keyup(function () {
            if ($name == null) {
                insertComment();
                $name = $('.livecomment address a');
            }
            name = $(this).val();
            $name.text(name);
        });
        var $url = null;
        $('input[name$="Url"]').keyup(function () {
            if ($url == null) {
                insertComment();
                $url = $('.livecomment address a');
            }
            var url = Raccoon.Util.Url.toAbsolute($(this).val());
            if (url) {
                $url.attr('href', url);
                $url.attr('title', url);
            } else {
                $url.removeAttr('href');
                $url.attr('title', name);
            }
        });
        
        
        $('#postComment form').submit(function () {
	        var t = $(this);
	        if (t.valid()) {
	            var jqxhr = $.post(t.attr('action'), t.serializeArray(), null, 'json');
	            jqxhr.error(function (data, textStatus, jqXHR) {
	                Raccoon.Util.Views.setMessage('An error occurred while posing your comment', 'fail');
	            });
	            jqxhr.success(function (data, textStatus, jqXHR) {
	                if (!data.success) Raccoon.Util.Views.setMessage('An error occurred while posting your comment: ' + data.message, 'fail');
	                else {
	                    Raccoon.Util.Views.setMessage(data.message);
	                    $('article#postComment, article.markdown-preview-container').remove();
	                }
	            });
	        }
	        return false;
	    });
    };
    
    window.Raccoon = Raccoon;
})(window);