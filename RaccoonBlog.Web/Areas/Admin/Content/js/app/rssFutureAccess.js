(function () {
    $('.datepicker input').datetimepicker();
    $('.future-rss .linkbox')
        .click(function () {
            this.setSelectionRange(0, this.value.length);
        });

    var copyBtnSel = '.copy-link-btn';
    var $copyBtn = $(copyBtnSel);
    var animClass = 'blink-green';

    var clipboard = new Clipboard(copyBtnSel);
    clipboard.on('success',
        function(e) {
            $copyBtn.removeClass(animClass);
            $copyBtn[0].offsetWidth = $copyBtn[0].offsetWidth; // causes reflow, allows restarting animation
            $copyBtn.addClass(animClass);

            var curText = $copyBtn.html();
            $copyBtn.html('<i class="glyphicon glyphicon-clip"></i>&nbsp;Copied to clipboard');
            setTimeout(function() {
                $copyBtn.html(curText);
            }, 900);
        })
        .on('error', function(err) {
            console.error(err);
            alert('Could not copy to clipboard. Please do it manually.');
        });
}(window));