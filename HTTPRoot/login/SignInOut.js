var SignInUser = "";
var Username = "";
var UserEMail = "";
var isAdmin = false;
function HideElement(DivName) {
    var div = document.querySelector(DivName);
    if (div != null)
        div.style.display = "none";
    return div;
}
function ShowElement(DivName) {
    return ShowElement2(DivName, "flex");
}
function ShowElement2(DivName, displaymode) {
    if (displaymode == undefined)
        displaymode = "flex";
    var div = document.querySelector(DivName);
    if (div != null)
        div.style.display = displaymode;
    return div;
}
function GetCookie(CookieName) {
    var CookieMatch = document.cookie.match('(^|;) ?' + CookieName + '=([^;]*)(;|$)');
    if (CookieMatch == null)
        return null;
    return CookieMatch[2];
}
function WithCookie(CookieName, OnSucess, OnFailure) {
    var Cookie = GetCookie(CookieName);
    if (Cookie == null) {
        OnFailure();
        return null;
    }
    else if (OnSucess != undefined)
        OnSucess(Cookie);
}
function DeleteCookie(CookieName, Path) {
    DeleteCookie2(CookieName, "/");
}
function DeleteCookie2(CookieName, Path) {
    var CookieDateTime = new Date();
    CookieDateTime.setTime(CookieDateTime.getTime() - 86400000); // 1 day
    if (Path == undefined)
        Path = "/";
    document.cookie = CookieName += "=; expires=" + CookieDateTime.toUTCString() + "; Path=" + Path;
}
function VerifyLogin() {
    function VerifyPassword() {
        var ResponseText = HTTPAuth("/users/" + _login.value, "APIKey", {
            "login": _login.value,
            "password": _password.value
        }, null, null);
        if (ResponseText == "") {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> Login failed!";
            return false;
        }
        var responseJSON = JSON.parse(ResponseText);
        if (responseJSON.username == null || responseJSON.email == null) {
            if (responseJSON.error != null) {
                responseDiv.style.display = 'block';
                responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> " + responseJSON.error;
            }
            if (responseJSON.description != null) {
                responseDiv.style.display = 'block';
                responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> " + responseJSON.description;
            }
            else {
                responseDiv.style.display = 'block';
                responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> Login failed!";
            }
            return false;
        }
        return true;
    }
    var loginform = document.getElementById("loginform");
    var _login = document.getElementById("_login");
    var _password = document.getElementById("_password");
    var responseDiv = document.getElementById("response");
    var saveButton = document.getElementById("button");
    loginform.onsubmit = function (ev) {
        return VerifyPassword();
    };
    checkNotSignedIn();
}
function SignIn() {
    var SignInPanel = document.querySelector('#login');
    var Username = SignInPanel.querySelector('#_login').value;
    var Realm = SignInPanel.querySelector('#_realm').value;
    var Password = SignInPanel.querySelector('#_password').value;
    var RememberMe = SignInPanel.querySelector('#_rememberme').checked;
    var SignInErrors = SignInPanel.querySelector('#errors');
    SignInErrors.style.display = "none";
    SignInErrors.innerText = "";
    SendJSON("AUTH", "/users/" + Username, "", {
        "realm": Realm,
        "password": Password,
        "rememberme": RememberMe
    }, function (HTTPStatus, ResponseText) {
        //(<HTMLFormElement> document.querySelector('#loginform')).submit();
        location.href = "/";
    }, function (HTTPStatus, StatusText, ResponseText) {
        SignInErrors.style.display = "block";
        SignInErrors.innerText = JSON.parse(ResponseText).description;
    });
}
function checkSignedIn(RedirectUnkownUsers) {
    WithCookie(HTTPCookieId, function (cookie) {
        // Crumbs are base64 encoded!
        cookie.split(":").forEach(function (crumb) {
            if (crumb.indexOf("login") >= 0)
                SignInUser = atob(crumb.split("=")[1]);
            if (crumb.indexOf("username") >= 0)
                Username = atob(crumb.split("=")[1]);
            if (crumb.indexOf("email") >= 0)
                UserEMail = atob(crumb.split("=")[1]);
            if (crumb.indexOf("isAdmin") >= 0) {
                isAdmin = true;
                ShowElement('#admin');
                ShowElement('.admin');
            }
        });
        document.querySelector('#username').innerText = Username;
        ShowElement('#username');
        ShowElement('.username');
        ShowElement('#profile');
        ShowElement('.profile');
        ShowElement('#SignOut');
        ShowElement('.SignOut');
        HideElement('#SignIn');
        HideElement('.SignIn');
    }, function () {
        HideElement('#SignOut');
        HideElement('.SignOut');
        HideElement('#profile');
        HideElement('.profile');
        ShowElement('#SignIn');
        ShowElement('.SignIn');
        var usernameDiv = document.querySelector('#username');
        if (usernameDiv != null)
            usernameDiv.innerText = "anonymous";
        if (RedirectUnkownUsers)
            location.href = "/login";
    });
}
function checkAdminSignedIn(RedirectUnkownUsers) {
    WithCookie(HTTPCookieId, function (cookie) {
        ShowElement('#admin');
        ShowElement('.admin');
        if (cookie.indexOf(":isAdmin") < 0)
            location.href = "/";
    }, function () {
        if (RedirectUnkownUsers)
            location.href = "/login";
    });
    checkSignedIn(RedirectUnkownUsers);
}
function checkNotSignedIn() {
    if (HTTPCookieId != null && HTTPCookieId != "") {
        WithCookie(HTTPCookieId, function (cookie) {
            location.href = "/index.html";
        }, function () {
        });
    }
}
function GetLogin() {
    var Cookie = GetCookie(HTTPCookieId);
    var Login = "";
    if (Cookie != null) {
        // Crumbs are base64 encoded!
        Cookie.split(":").forEach(function (crumb) {
            if (crumb.indexOf("login") >= 0)
                Login = atob(crumb.split("=")[1]);
        });
    }
    return Login;
}
function GetUsername() {
    var Cookie = GetCookie(HTTPCookieId);
    var Username = "";
    if (Cookie != null) {
        // Crumbs are base64 encoded!
        Cookie.split(":").forEach(function (crumb) {
            if (crumb.indexOf("username") >= 0)
                Username = atob(crumb.split("=")[1]);
        });
    }
    return Username;
}
function GetEMail() {
    var Cookie = GetCookie(HTTPCookieId);
    var Username = "";
    if (Cookie != null) {
        // Crumbs are base64 encoded!
        Cookie.split(":").forEach(function (crumb) {
            if (crumb.indexOf("email") >= 0)
                Username = atob(crumb.split("=")[1]);
        });
    }
    return Username;
}
function SignOut() {
    SendJSON("DEAUTH", "/users", "", "", function (HTTPStatus, ResponseText) {
    }, function (HTTPStatus, StatusText, ResponseText) {
    });
    DeleteCookie(HTTPCookieId, "/");
    location.href = "/login";
}
//# sourceMappingURL=SignInOut.js.map