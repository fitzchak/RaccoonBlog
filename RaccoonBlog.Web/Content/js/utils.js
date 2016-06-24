(function(window) {
    window.utils = {};

    window.utils.randomString = function randomString(num) {
        if (typeof num !== 'number') {
            throw new Error("String length argument " + num + " is not a number.");
        }

        var text = "";
        var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        for (var i = 0; i < num; i++)
            text += possible.charAt(Math.floor(Math.random() * possible.length));

        return text;
    }

    var cookies = window.utils.cookies = {};

    cookies.create = function createCookie(name, value, days) {
        if (!days) {
            days = 365;
        }

        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();

        document.cookie = encodeURIComponent(name) + "=" + encodeURIComponent(value) + expires + "; path=/";
    };

    cookies.read = function readCookie(name) {
        var nameEQ = encodeURIComponent(name) + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) === ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) === 0) return decodeURIComponent(c.substring(nameEQ.length, c.length));
        }
        return null;
    };

    cookies.erase = function erase(name) {
        createCookie(name, "", -1);
    };

}(window))