

function GetJS(URI, OnSuccess, OnError)
{

    var ajax = new XMLHttpRequest();
    ajax.open("GET", URI, true);
    ajax.setRequestHeader("Accept",       "application/javascript; charset=UTF-8");
    ajax.setRequestHeader("Content-Type", "application/javascript; charset=UTF-8");

    ajax.onreadystatechange = function() {

        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {

            // Ok
            if (this.status == 200) {

                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));

                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess(ajax.responseText);

            }

            else
                if (OnError && typeof OnError === 'function')
                    OnError(this.status, this.statusText);

        }

    }

    ajax.send("{ \"username\": \"ahzf\" }");

}

function GetJSON(URI, OnSuccess, OnError) {

    var ajax = new XMLHttpRequest();
    ajax.open("GET", URI, true);
    ajax.setRequestHeader("Accept",       "application/json; charset=UTF-8");
    ajax.setRequestHeader("Content-Type", "application/json; charset=UTF-8");

    ajax.onreadystatechange = function () {

        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {

            // Ok
            if (this.status == 200) {

                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));

                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess(ajax.responseText);

            }

            else
                if (OnError && typeof OnError === 'function')
                    OnError(this.status, this.statusText);

        }

    }

    ajax.send("{ \"username\": \"ahzf\" }");

}

function SendJSON(HTTPVerb, URI, OnSuccess, OnError) {

    var ajax = new XMLHttpRequest();
    ajax.open(HTTPVerb, URI, true);
    ajax.setRequestHeader("Accept",       "application/json; charset=UTF-8");
    ajax.setRequestHeader("Content-Type", "application/json; charset=UTF-8");

    ajax.onreadystatechange = function () {

        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {

            // Ok
            if (this.status >= 100 && this.status < 300) {

                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));

                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess(this.status, ajax.responseText);

            }

            else
                if (OnError && typeof OnError === 'function')
                    OnError(this.status, this.statusText, ajax.responseText);

        }

    }

    ajax.send();

}

function SendJSON(HTTPVerb, URI, Data, OnSuccess, OnError) {

    var ajax = new XMLHttpRequest();
    ajax.open(HTTPVerb, URI, true);
    ajax.setRequestHeader("Accept",       "application/json; charset=UTF-8");
    ajax.setRequestHeader("Content-Type", "application/json; charset=UTF-8");

    ajax.onreadystatechange = function () {

        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {

            // Ok
            if (this.status >= 100 && this.status < 300) {

                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));

                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess(this.status, ajax.responseText);

            }

            else
                if (OnError && typeof OnError === 'function')
                    OnError(this.status, this.statusText, ajax.responseText);

        }

    }

    // send the data as JSON
    ajax.send(JSON.stringify(Data));

  //  ajax.send("{ \"username\": \"ahzf\" }");

}

function GetHTML4App(URI, panelId, ChildDiv1, ChildDiv, OnSuccess, OnError) {

    if (panelId == undefined)
        panelId = 'panel1';

    var _panel = document.getElementById(panelId);
    if (_panel == null) {
        console.log("Could not find panel '" + panelId + "'!")
        return;
    }

    var _ChildDiv1 = _panel.querySelector(ChildDiv1);
    if (_ChildDiv1 == null) {
        console.log("Could not find child node '" + ChildDiv1 + "' of panel '" + panelId + "'!")
    }

    var _ChildDiv = _ChildDiv1.querySelector(ChildDiv);
    if (_ChildDiv == null) {
        console.log("Could not find child node '" + ChildDiv + "' of panel '" + panelId + "'!")
    }

    var ajax = new XMLHttpRequest();
    ajax.open("GET", URI, true);
    ajax.setRequestHeader("Accept",       "text/html; charset=UTF-8");
    ajax.setRequestHeader("Content-Type", "text/html; charset=UTF-8");

    ajax.onreadystatechange = function () {

        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {

            // Ok
            if (this.status == 200) {

                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));

                _ChildDiv.innerHTML = ajax.responseText;

                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess();

            }

            else
                if (OnError && typeof OnError === 'function')
                    OnError(this.status, this.statusText);

        }

    }

    ajax.send("{ \"username\": \"ahzf\" }");

}

function GetHTML2(URI, OnSuccess, OnError) {

    var _InfoboxDiv = document.querySelector('.Infobox');
    if (_InfoboxDiv != null)
        _InfoboxDiv.parentNode.removeChild(_InfoboxDiv);

    var ajax = new XMLHttpRequest();
    ajax.open("GET", URI, true);
    ajax.setRequestHeader("Accept",       "text/html; charset=UTF-8");
    ajax.setRequestHeader("Content-Type", "text/html; charset=UTF-8");

    ajax.onreadystatechange = function () {

        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {

            // Ok
            if (this.status == 200) {

                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));

                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess(ajax.responseText);

            }

            else
                if (OnError && typeof OnError === 'function')
                    OnError(this.status, this.statusText);

        }

    }

    ajax.send("{ \"username\": \"ahzf\" }");

}

function GetCookie(CookieName, Delegate) {

    var results = document.cookie.match('(^|;) ?' + CookieName + '=([^;]*)(;|$)');

    if (results == null)
        return null;

    if (Delegate == undefined)
        return results[2];

    Delegate(results[2]);

}

function DeleteCookie(CookieName, Path) {

    var CookieDateTime = new Date();
    CookieDateTime.setTime(CookieDateTime.getTime() - 86400000); // 1 day

    if (Path == undefined)
        Path = "/";

    document.cookie = CookieName += "=; expires=" + CookieDateTime.toGMTString() + "; Path=" + Path;

}