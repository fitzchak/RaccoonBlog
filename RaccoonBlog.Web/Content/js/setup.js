(function (window) {

    var bodyEl = window.document.body,
        openbtn = $('#open-button'),
        enlargebtn = $('#enlarge-button'),
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

    var cookies = utils.cookies;
    var markdown = getMarkdownConverter();

    init();

    function init() {
        initArchiveMenu();
        initEvents();

        window.onresize = function () { adjustSize(); };

        initMorePostsInSeries();

        $('.comments textarea').keydown(handleTabsInComment);

        enableCommentsPreview();
        adjustSize();
        showSidebarOnFirstVisit();
        setPreferredView();
        makeTablesResponsive();
    }

    function initEvents() {
        openbtn.click(toggleMenu);
        enlargebtn.click(toggleSidebar);
        showgrid.click(toggleGrid);
        showstack.click(toggleStack);
        openTags.click(toggleTags);
        openArch.click(toggleArch);
        openSeries.click(toggleSeries);
    }

    function enableCommentsPreview() {
        $('#commentPreview').click(function () {
            $('.comments textarea').blur(updateCommentPreview);
            updateCommentPreview();
        });
    }

    function makeTablesResponsive() {
        $('.articleContent>table').addClass('table').wrap("<div class='table-responsive'></div>");
    }

    function initMorePostsInSeries() {
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
    }

    function initArchiveMenu() {
        $('#archive > ul > li > a').click(function (e) {
            e.preventDefault();
            if ($(this).parent().hasClass('open')) {
                $(this).parent().removeClass('open');
            } else {
                $('#archive .open').removeClass('open');
                $(this).parent().addClass('open');
            }
        });
    }

    function toggleSidebar() {
        $('.container').toggleClass('hideSidebar');
    }

    function toggleMenu() {
        if (isOpen) {
            $(bodyEl).removeClass('show-menu');
        } else {
            $(bodyEl).addClass('show-menu');
        }

        isOpen = !isOpen;
    }

    function toggleStack() {
        if (!isGrid)
            return;

        $(bodyEl).removeClass('show-grid');
        $(showgrid).removeClass('active');
        $(showstack).addClass('active');
        isGrid = false;
        cookies.create('view', 'stack');
        adjustSize();
    }

    function toggleGrid() {
        if (isGrid)
            return;

        $(bodyEl).addClass('show-grid');
        $(showgrid).addClass('active');
        $(showstack).removeClass('active');
        isGrid = true;
        cookies.create('view', 'grid');
        adjustSize();
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

    function adjustSize() {
        if ($(window).width() < 992) {
            $('.rightSide').css('min-height', 0);
        } else {
            $('.rightSide').css('min-height', $('.centerCol').height() + 70);
        }
    }

    function showSidebarOnFirstVisit() {
        var visitCookieVaule = cookies.read('visitStatus');
        if (visitCookieVaule != 'visited') {
            var showCondition = $(window).width() > $(window).height();
            if (showCondition) {
                toggleSidebar();
            }
            cookies.create('visitStatus', 'visited');
        }
    }

    function setPreferredView() {
        var viewCookieValue = cookies.read('view');
        if (viewCookieValue == 'stack') {
            $(bodyEl).removeClass('show-grid');
            $(showgrid).removeClass('active');
            $(showstack).addClass('active');
            isGrid = false;
            cookies.create('view', 'stack');
            adjustSize();

        } else {
            $(bodyEl).addClass('show-grid');
            $(showgrid).addClass('active');
            $(showstack).removeClass('active');
            isGrid = true;
            cookies.create('view', 'grid');
            adjustSize();
        }
    }

    function handleTabsInComment(ev) {
        if (ev.keyCode !== 9)
            return;

        var start = this.selectionStart;
        var end = this.selectionEnd;

        var $this = $(this);
        var value = $this.val();

        $this.val(value.substring(0, start)
            + "\t"
            + value.substring(end));

        this.selectionStart = this.selectionEnd = start + 1;

        ev.preventDefault();
    }

    function updateCommentPreview() {

        if (!$('form:last').valid())
            return;

        var rawBody = $('#Input_Body').val();
        var html = markdown.Transform(rawBody);
        $('.comment.preview .comment-body').html(html);

        var source = 'http://www.gravatar.com/avatar.php?gravatar_id=' +
            utils.randomString(50) + '&size=50&default=identicon';

        var mdate = moment();
        var currentTime = mdate.format("DD MMM YYYY<br/>HH:mm");

        $('.comment.preview time').html(currentTime);
        $('.comment.preview .avatar img').attr("src", source);
        $('.comment.preview .postedBy a').html($('#Input_Name').val());
        $('.comment.preview').addClass('active');
    }

    function getMarkdownConverter() {
        var markdown = new MarkdownDeep.Markdown();
        markdown.ExtraMode = true;
        markdown.SafeMode = true;
        markdown.NoFollowLinks = true;
        markdown.NewWindowForExternalLinks = true;
        markdown.MarkdownInHtml = false;

        return {
            Transform: function (body) {
                return markdown.Transform.call(markdown, supportGithubPreCodeBlocks(body));
            }
        };

        function supportGithubPreCodeBlocks(body) {
            return body.replace(/^```+\s*$/gm, '~~~');
        }
    }

})(window);

