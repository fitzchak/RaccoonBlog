
// Common
String.prototype.startsWith = function (str) {
    return this.substr(0, str.length) === str;
};
String.prototype.endsWith = function (str) {
    return this.indexOf(str, this.length - str.length) !== -1;
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
    
    Raccoon.Util.Markdown = {};
    Raccoon.Util.Markdown.settings = {
        onShiftEnter: { keepDefault: false, replaceWith: '<br />\n' },
        onCtrlEnter: { keepDefault: false, openWith: '\n<p>', closeWith: '</p>' },
        onTab: { keepDefault: false, replaceWith: '    ' },
        markupSet: [
            { name: 'Bold', key: 'B', openWith: '(!(<strong>|!|<b>)!)', closeWith: '(!(</strong>|!|</b>)!)' },
            { name: 'Italic', key: 'I', openWith: '(!(<em>|!|<i>)!)', closeWith: '(!(</em>|!|</i>)!)' },
            { name: 'Stroke through', key: 'S', openWith: '<del>', closeWith: '</del>' },
            { separator: '---------------' },
            { name: 'Picture', key: 'P', replaceWith: '<img src="[![Source:!:http://]!]" alt="[![Alternative text]!]" />' },
            { name: 'Link', key: 'L', openWith: '<a href="[![Link:!:http://]!]"(!( title="[![Title]!]")!)>', closeWith: '</a>', placeHolder: 'Your text to link...' },
            { separator: '---------------' },
            { name: 'Clean', className: 'clean', replaceWith: function(markitup) { return markitup.selection.replace( /<(.*?)>/g , "") } },
            { name: 'Preview', className: 'preview', call: 'preview' }
        ]
    };
    Raccoon.Util.Markdown.convert = function (content) {
        if (!Raccoon.Util.Markdown.showdownConverter)
            Raccoon.Util.Markdown.showdownConverter = new Showdown.converter();
        
        var lines = content.split(/\r\n|\r|\n/);
        var c = '';
        $.each(lines, function(n, elem) {
            if (!elem.endsWith('  '))
                elem += '  ';
            c += elem + '\r\n';
        });
        return Raccoon.Util.Markdown.showdownConverter.makeHtml(c);
    };

    Raccoon.Util.Views = {};
    Raccoon.Util.Views.setMessage = function setMessage(message, cssClass) {
        var m = $('#message').html(message).removeClass();
        if (cssClass) m.addClass(cssClass);
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
                body: Raccoon.Util.Markdown.convert($('article#postComment textarea[name$="Body"]').val()),
                createdAt: now.f("MM/dd/yyyy HH:mm")
            };
            $('#commentTemplate').tmpl(comment).appendTo('section.comments').show('medium');
        };
        
        var $preview = null;
        $('textarea[name$="Body"]').keyup(function () {
            if ($preview == null) {
                insertComment();
                $preview = $('.livecomment .comment-body');
            }
            $preview.html(Raccoon.Util.Markdown.convert($(this).val()));
        });
        var $email = null;
        $('input[name$="Email"]').keyup(function () {
            if ($email == null) {
                insertComment();
                $email = $('.livecomment .avatar img');
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
	                Raccoon.Util.Views.setMessage('An error occurred while posting your comment', 'fail');
	            });
	            jqxhr.success(function (data, textStatus, jqXHR) {
	                if (!data.success) {
	                    Raccoon.Util.Views.setMessage('An error occurred while posting your comment: ' + data.message, 'fail');
	                    if ($('#recaptcha_widget_div').length > 0) Recaptcha.reload();
	                }
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