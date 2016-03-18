
// IE11 workaround, as there 'EventTarget' is unknown!
var _EventTarget = typeof EventTarget !== 'undefined' ? EventTarget.prototype : Element.prototype;

// Allow to bind multiple events at once
if (_EventTarget.addEventListeners !== 'function') {
    _EventTarget.addEventListeners = function (events, callback) {
        var target = this;
        events.split(" ").forEach(function (event) {
            target.addEventListener(event, callback, false);
        });
    };
}

var InputCheckTimeout;
var InputCheckDelay = 1000; // 1 second

// Allow to bind multiple events at once
if (_EventTarget.addEventListeners2 !== 'function') {
    _EventTarget.addEventListeners2 = function (keyVerification, checkResult) {

        var target = this;

        target.onkeypress = function (e) {

            // Firefox seems to require special-key handling...
            // Google Chrome does not send events for cursor keys for the '.onkeypress' event!
            if (e.charCode == 0 && // Additional check for Google Chrome
              ((e.keyCode >= 37 && e.keyCode <= 40) || // cursors
                e.keyCode == 8 || // backspace
                e.keyCode == 9 || // tab
                e.keyCode == 36 || // home
                e.keyCode == 35 || // end
                e.keyCode == 46))                      // del
                return true;

            return keyVerification(e.keyCode, e.charCode);

        };

        "keyup cut paste blur".split(" ").forEach(
            function (event) {
                target.addEventListener(event,
                                        function (e) {

                                            if (InputCheckTimeout)
                                                clearTimeout(InputCheckTimeout);

                                            if (e.type == "blur")
                                                checkResult(e);

                                            else
                                                InputCheckTimeout = setTimeout(function () {
                                                    checkResult(e);
                                                }, InputCheckDelay);

                                        },
                                        false);
            });

    };
}

function HideDiv(DivName) {

    var div = document.querySelector(DivName);
    div.style.display = "none";

    return div;

}

function ShowDiv(DivName, displaymode) {

    if (displaymode == undefined)
        displaymode = "inline-block";

    var div = document.querySelector(DivName);
    div.style.display = displaymode;

    return div;

}
