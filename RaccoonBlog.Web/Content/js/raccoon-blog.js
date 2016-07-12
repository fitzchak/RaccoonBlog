
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
    Raccoon.Util.Views = {};

    Raccoon.Util.Views.setMessage = function setMessage(message, cssClass) {
        var m = $('#message').html(message).removeClass();
        if (cssClass) m.addClass(cssClass);
        m.css('visibility', 'visible');
    };
    
    window.Raccoon = Raccoon;
})(window);
