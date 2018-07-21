var SignInUser = "";
var Username = "";
var UserEMail = "";
var Astronaut = "";
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
    var CookieDateTime = new Date();
    CookieDateTime.setTime(CookieDateTime.getTime() - 86400000); // 1 day
    if (Path == undefined)
        Path = "/";
    document.cookie = CookieName += "=; expires=" + CookieDateTime.toUTCString() + "; Path=" + Path;
}
function VerifyLogin() {
    var acceptsEULA = false;
    function ToogleSaveButton() {
        var validInput = _login.value.length >= 3 && _password.value.length > 5;
        loginInput.disabled = !validInput;
        if (validInput && loginInput.classList.contains("error"))
            loginInput.classList.remove("error");
        else if (!validInput && !loginInput.classList.contains("error"))
            loginInput.classList.add("error");
    }
    function VerifyPassword() {
        var ResponseText = HTTPAuth("/users/" + _login.value, "APIKey", {
            "login": _login.value,
            "password": _password.value,
            "acceptsEULA": acceptsEULA
        });
        if (ResponseText == "") {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> Could not login!";
            return false;
        }
        try {
            var responseJSON = JSON.parse(ResponseText);
            if (responseJSON.showEULA == true) {
                EULA.style.display = 'block';
                return false;
            }
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
        }
        catch (e) {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> Could not login!";
            return false;
        }
        return true;
    }
    var loginform = document.getElementById("loginform");
    var _login = document.getElementById("_login");
    var _realm = document.getElementById("_realm");
    var _password = document.getElementById("_password");
    var responseDiv = document.getElementById("response");
    var loginButton = document.getElementById("loginButton");
    var loginInput = document.getElementById("loginInput");
    var EULA = document.getElementById("EULA");
    var IAcceptDiv = document.getElementById("IAccept");
    var acceptEULAButton = document.getElementById("acceptEULAButton");
    _login.onchange = function (ev) {
        _login.value = _login.value.toLowerCase();
        ToogleSaveButton();
    };
    _login.onkeyup = function (ev) {
        _login.value = _login.value.toLowerCase();
        ToogleSaveButton();
    };
    _password.onchange = function (ev) {
        ToogleSaveButton();
    };
    _password.onkeyup = function (ev) {
        ToogleSaveButton();
    };
    loginform.onsubmit = function (ev) {
        return VerifyPassword();
    };
    IAcceptDiv.onclick = function (ev) {
        if (!IAcceptDiv.children[0].classList.contains("accepted")) {
            IAcceptDiv.children[0].classList.add("accepted");
            acceptEULAButton.classList.add("accepted");
            // Respect the login button... just for additional security!
            acceptEULAButton.disabled = loginInput.disabled;
        }
        else {
            IAcceptDiv.children[0].classList.remove("accepted");
            acceptEULAButton.classList.remove("accepted");
            acceptEULAButton.disabled = true;
        }
    };
    acceptEULAButton.onclick = function (ev) {
        acceptsEULA = true;
        loginform.submit();
    };
    checkNotSignedIn();
}
function LostPassword() {
    function ToogleSaveButton() {
        var validInput = _id.value.length > 3;
        resetPasswordInput.disabled = !validInput;
        if (validInput && resetPasswordInput.classList.contains("error"))
            resetPasswordInput.classList.remove("error");
        else if (!validInput && !resetPasswordInput.classList.contains("error"))
            resetPasswordInput.classList.add("error");
        responseDiv.style.display = 'none';
        responseDiv.innerHTML = "";
        responseDiv.classList.remove("responseError");
        responseDiv.classList.remove("responseOk");
    }
    function ResetPassword() {
        responseDiv.style.display = 'block';
        responseDiv.innerHTML = '<i class="fa fa-spinner faa-spin animated"></i> Verifying your login... please wait!';
        HTTPSet("/resetPassword", "APIKey", {
            "id": _id.value
        }, function (HTTPStatus, ResponseText) {
            try {
                var responseJSON = JSON.parse(ResponseText);
                if (responseJSON.numberOfAccountsFound != null) {
                    responseDiv.style.display = 'block';
                    responseDiv.innerHTML = "<i class='fas fa-user-check  fa-2x menuicons'></i> Found " + responseJSON.numberOfAccountsFound + " account(s). Please check your e-mails!";
                    responseDiv.classList.remove("responseError");
                    responseDiv.classList.add("responseOk");
                    return;
                }
            }
            catch (e) {
            }
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> Resetting your password failed!";
            responseDiv.classList.remove("responseOk");
            responseDiv.classList.add("responseError");
        }, function (HTTPStatus, StatusText, ResponseText) {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> Resetting your password failed!";
            responseDiv.classList.remove("responseOk");
            responseDiv.classList.add("responseError");
        });
        return false;
    }
    var loginform = document.getElementById("loginform");
    var _id = document.getElementById("_id");
    var _realm = document.getElementById("_realm");
    var responseDiv = document.getElementById("response");
    var resetPasswordButton = document.getElementById("resetPasswordButton");
    var resetPasswordInput = document.getElementById("resetPasswordInput");
    _id.onchange = function (ev) {
        _id.value = _id.value.toLowerCase();
        ToogleSaveButton();
    };
    _id.onkeyup = function (ev) {
        _id.value = _id.value.toLowerCase();
        ToogleSaveButton();
    };
    loginform.onsubmit = function (ev) {
        return ResetPassword();
    };
    checkNotSignedIn();
    ToogleSaveButton();
}
function SetPassword() {
    var minPasswordLength = 5;
    var use2FactorAuth = false;
    function ToogleSaveButton() {
        if (securityToken1.value != "" &&
            securityToken1.value.length != 40) {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> The first security token is invalid!</div>";
            responseDiv.classList.remove("responseOk");
            responseDiv.classList.add("responseError");
        }
        else if (securityToken2.value != "" &&
            securityToken2.value.length != 11) {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> The second security token is invalid!</div>";
            responseDiv.classList.remove("responseOk");
            responseDiv.classList.add("responseError");
        }
        else if (newPassword1.value != "" &&
            newPassword1.value.length <= minPasswordLength) {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> The new password is too short!</div>";
            responseDiv.classList.remove("responseOk");
            responseDiv.classList.add("responseError");
        }
        else if (newPassword2.value != "" &&
            newPassword2.value.length <= minPasswordLength) {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> The new password2 is too short!</div>";
            responseDiv.classList.remove("responseOk");
            responseDiv.classList.add("responseError");
        }
        else if (newPassword1.value != newPassword2.value) {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> The given passwords do not match!</div>";
            responseDiv.classList.remove("responseOk");
            responseDiv.classList.add("responseError");
        }
        else {
            responseDiv.style.display = 'none';
            responseDiv.innerHTML = "";
        }
        var validInput = responseDiv.innerHTML == "" &&
            securityToken1.value.length == 40 &&
            (!use2FactorAuth || securityToken2.value.length == 11) &&
            newPassword1.value != "" &&
            newPassword2.value != "";
        setPasswordInput.disabled = !validInput;
        if (validInput && setPasswordInput.classList.contains("error"))
            setPasswordInput.classList.remove("error");
        else if (!validInput && !setPasswordInput.classList.contains("error"))
            setPasswordInput.classList.add("error");
    }
    function SetPassword() {
        responseDiv.style.display = 'block';
        responseDiv.innerHTML = '<i class="fa fa-spinner faa-spin animated"></i> Verifying your request... please wait!';
        responseDiv.classList.remove("responseError");
        responseDiv.classList.remove("responseOk");
        var SetPasswordJSON = {
            "securityToken1": securityToken1.value,
            "newPassword": newPassword1.value
        };
        if (securityToken2.value != "")
            SetPasswordJSON["securityToken2"] = securityToken2.value;
        HTTPSet("/setPassword", "APIKey", SetPasswordJSON, function (HTTPStatus, ResponseText) {
            try {
                var responseJSON = JSON.parse(ResponseText);
                if (responseJSON.numberOfAccountsFound != null) {
                    responseDiv.style.display = 'block';
                    responseDiv.innerHTML = "<i class='fas fa-user-check  fa-2x menuicons'></i> Succssfully resetted your password!";
                    responseDiv.classList.remove("responseError");
                    responseDiv.classList.add("responseOk");
                    setPasswordInput.disabled = true;
                    setPasswordButton.style.display = 'none';
                    gotoLoginButton.style.display = 'block';
                    return;
                }
            }
            catch (e) { }
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> Setting your password failed!";
            responseDiv.classList.remove("responseOk");
            responseDiv.classList.add("responseError");
        }, function (HTTPStatus, StatusText, ResponseText) {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> Setting your password failed!";
            responseDiv.classList.remove("responseOk");
            responseDiv.classList.add("responseError");
        });
        return false;
    }
    var loginform = document.getElementById("loginform");
    var securityToken1 = document.getElementById("securityToken1");
    var securityToken2 = document.getElementById("securityToken2");
    var newPassword1 = document.getElementById("newPassword1");
    var newPassword2 = document.getElementById("newPassword2");
    var responseDiv = document.getElementById("response");
    var setPasswordButton = document.getElementById("setPasswordButton");
    var setPasswordInput = document.getElementById("setPasswordInput");
    var gotoLoginButton = document.getElementById("gotoLoginButton");
    var gotoLoginInput = document.getElementById("gotoLoginInput");
    if (window.location.search.length > 1) {
        var elements = window.location.search.substring(1).trim().split("&");
        securityToken1.value = elements[0].trim();
        if (elements[1] == "2factor") {
            use2FactorAuth = true;
            securityToken2.parentElement.parentElement.style.display = 'block';
        }
    }
    securityToken1.onchange = function (ev) {
        ToogleSaveButton();
    };
    securityToken1.onkeyup = function (ev) {
        ToogleSaveButton();
    };
    securityToken2.onchange = function (ev) {
        ToogleSaveButton();
    };
    securityToken2.onkeyup = function (ev) {
        ToogleSaveButton();
    };
    newPassword1.onchange = function (ev) {
        ToogleSaveButton();
    };
    newPassword1.onkeyup = function (ev) {
        ToogleSaveButton();
    };
    newPassword2.onchange = function (ev) {
        ToogleSaveButton();
    };
    newPassword2.onkeyup = function (ev) {
        ToogleSaveButton();
    };
    loginform.onsubmit = function (ev) {
        return SetPassword();
    };
    gotoLoginButton.onclick = function (ev) {
        window.location.href = "/login";
    };
    DeleteCookie(HTTPCookieId);
    ToogleSaveButton();
}
function SignIn() {
    var SignInPanel = document.querySelector('#login');
    var Username = SignInPanel.querySelector('#_login').value.toLowerCase();
    var Realm = SignInPanel.querySelector('#_realm').value.toLowerCase();
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
        isAdmin = false;
        Astronaut = "";
        // Crumbs are base64 encoded!
        cookie.split(":").forEach(function (crumb) {
            if (crumb.indexOf("login") >= 0)
                SignInUser = atob(crumb.split("=")[1]);
            if (crumb.indexOf("username") >= 0)
                Username = atob(crumb.split("=")[1]);
            if (crumb.indexOf("email") >= 0)
                UserEMail = atob(crumb.split("=")[1]);
            if (crumb.indexOf("astronaut") >= 0)
                Astronaut = atob(crumb.split("=")[1]);
            if (crumb.indexOf("isAdmin") >= 0) {
                isAdmin = true;
                ShowElement('#admin');
                ShowElement('.admin');
            }
        });
        document.querySelector('#username').innerText = Username;
        document.querySelector('#astronaut').innerText = Astronaut;
        if (Astronaut != "")
            ShowElement2('#astronautFrame', 'inline-block');
        if (window.matchMedia("(min-device-width : 376px)").matches) {
            ShowElement('#username');
            ShowElement('.username');
        }
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
    DeleteCookie(HTTPCookieId);
    location.href = "/login";
}
function Depersonate() {
    HTTPDepersonate("/users/" + SignInUser, function (HTTPStatus, ResponseText) {
        window.location.reload(true);
    }, function (HTTPStatus, StatusText, ResponseText) {
        alert("Not allowed!");
    });
}
//# sourceMappingURL=SignInOut.js.map