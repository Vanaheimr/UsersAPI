let SignInUser = "";
let Username = "";
let UserEMail = "";
let Astronaut = "";
let isAdmin = "";
const newsBannersCookieId = "newsBanners";
function HideElement(DivName) {
    const div = document.querySelector(DivName);
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
    const div = document.querySelector(DivName);
    if (div != null)
        div.style.display = displaymode;
    return div;
}
function GetCookie(CookieName) {
    const CookieMatch = document.cookie.match('(^|;) ?' + CookieName + '=([^;]*)(;|$)');
    if (CookieMatch == null)
        return null;
    return CookieMatch[2];
}
function WithCookie(CookieName, OnSucess, OnFailure) {
    const CookieMatch = document.cookie.match('(^|;) ?' + CookieName + '=([^;]*)(;|$)');
    if (CookieMatch == null)
        OnFailure();
    else if (OnSucess != undefined)
        OnSucess(CookieMatch[2]);
}
function DeleteCookie(CookieName, Path) {
    let CookieDateTime = new Date();
    CookieDateTime.setTime(CookieDateTime.getTime() - 86400000); // 1 day
    if (Path == undefined)
        Path = "/";
    document.cookie = CookieName += "=; expires=" + CookieDateTime.toUTCString() + "; Path=" + Path;
}
function VerifyLogin() {
    let acceptsEULA = false;
    const redirectInputHidden = document.getElementById("redirect");
    const loginform = document.getElementById("loginform");
    const _login = document.getElementById("_login");
    const _realm = document.getElementById("_realm");
    const _password = document.getElementById("_password");
    const responseDiv = document.getElementById("response");
    const loginButton = document.getElementById("loginButton");
    const loginInput = document.getElementById("loginInput");
    const EULA = document.getElementById("EULA");
    const IAcceptDiv = document.getElementById("IAccept");
    const acceptEULAButton = document.getElementById("acceptEULAButton");
    const AdditionalAuthenticationFactor = document.getElementById("AdditionalAuthenticationFactor");
    if (window.location.search.length > 1) {
        for (const element of window.location.search.substring(1).trim().split("&")) {
            if (element.startsWith("redirect=")) {
                redirectInputHidden.value = element.substring(9).trim();
            }
        }
    }
    _login.onchange = () => {
        _login.value = _login.value.toLowerCase();
        ToogleSaveButton();
    };
    _login.onkeyup = () => {
        _login.value = _login.value.toLowerCase();
        ToogleSaveButton();
    };
    _password.onchange = () => {
        ToogleSaveButton();
    };
    _password.onkeyup = () => {
        ToogleSaveButton();
    };
    loginform.onsubmit = () => {
        return VerifyPassword();
    };
    IAcceptDiv.onclick = () => {
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
    acceptEULAButton.onclick = () => {
        acceptsEULA = true;
        if (VerifyPassword())
            loginform.submit();
    };
    function ToogleSaveButton() {
        var validInput = _login.value.length >= 3 && _password.value.length > 5;
        loginInput.disabled = !validInput;
        if (validInput && loginInput.classList.contains("error"))
            loginInput.classList.remove("error");
        else if (!validInput && !loginInput.classList.contains("error"))
            loginInput.classList.add("error");
    }
    function VerifyPassword() {
        const ResponseText = HTTPAuth((URLPathPrefix !== null && URLPathPrefix !== void 0 ? URLPathPrefix : "") + "/users/" + _login.value, {
            "login": _login.value,
            "password": _password.value,
            "acceptsEULA": acceptsEULA
        });
        if (ResponseText === "") {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> Could not login!";
            return false;
        }
        try {
            const responseJSON = JSON.parse(ResponseText);
            if (responseJSON.showEULA == true) {
                EULA.style.display = 'block';
                return false;
            }
            if (responseJSON.additionalAuthenticationFactor == true) {
                AdditionalAuthenticationFactor.style.display = 'block';
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
            return true;
        }
        catch (e) {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> Could not login!";
            return false;
        }
    }
    //checkNotSignedIn();
}
function LostPassword() {
    const loginform = document.getElementById("loginform");
    const _id = document.getElementById("_id");
    const _realm = document.getElementById("_realm");
    const responseDiv = document.getElementById("response");
    const resetPasswordButton = document.getElementById("resetPasswordButton");
    const resetPasswordInput = document.getElementById("resetPasswordInput");
    _id.onchange = () => {
        _id.value = _id.value.toLowerCase();
        ToogleSaveButton();
    };
    _id.onkeyup = () => {
        _id.value = _id.value.toLowerCase();
        ToogleSaveButton();
    };
    loginform.onsubmit = function (ev) {
        return ResetPassword();
    };
    function ToogleSaveButton() {
        const validInput = _id.value.length > 3;
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
        HTTPSet((URLPathPrefix !== null && URLPathPrefix !== void 0 ? URLPathPrefix : "") + "/resetPassword", {
            "id": _id.value
        }, (HTTPStatus, ResponseText) => {
            try {
                const responseJSON = JSON.parse(ResponseText);
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
        }, (HTTPStatus, StatusText, ResponseText) => {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> Resetting your password failed!";
            responseDiv.classList.remove("responseOk");
            responseDiv.classList.add("responseError");
        });
        return false;
    }
    checkNotSignedIn();
    ToogleSaveButton();
}
function SetPassword() {
    const minPasswordLength = 5;
    let use2FactorAuth = false;
    const loginform = document.getElementById("loginform");
    const securityToken1 = document.getElementById("securityToken1");
    const securityToken2 = document.getElementById("securityToken2");
    const newPassword1 = document.getElementById("newPassword1");
    const newPassword2 = document.getElementById("newPassword2");
    const responseDiv = document.getElementById("response");
    const setPasswordButton = document.getElementById("setPasswordButton");
    const setPasswordInput = document.getElementById("setPasswordInput");
    const gotoLoginButton = document.getElementById("gotoLoginButton");
    const gotoLoginInput = document.getElementById("gotoLoginInput");
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
        let SetPasswordJSON = {
            "securityToken1": securityToken1.value,
            "newPassword": newPassword1.value
        };
        if (securityToken2.value != "")
            SetPasswordJSON["securityToken2"] = securityToken2.value;
        HTTPSet((URLPathPrefix !== null && URLPathPrefix !== void 0 ? URLPathPrefix : "") + "/setPassword", SetPasswordJSON, (HTTPStatus, ResponseText) => {
            try {
                var responseJSON = JSON.parse(ResponseText);
                if (responseJSON.numberOfAccountsFound != null) {
                    responseDiv.style.display = 'block';
                    responseDiv.innerHTML = "<i class='fas fa-user-check  fa-2x menuicons'></i> Succssfully resetted your password!";
                    responseDiv.classList.remove("responseError");
                    responseDiv.classList.add("responseOk");
                    setPasswordInput.disabled = true;
                    setPasswordButton.style.display = 'none';
                    gotoLoginInput.disabled = false;
                    gotoLoginButton.style.display = 'block';
                    return;
                }
            }
            catch (e) { }
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> Setting your password failed!";
            responseDiv.classList.remove("responseOk");
            responseDiv.classList.add("responseError");
        }, (HTTPStatus, StatusText, ResponseText) => {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> Setting your password failed!";
            responseDiv.classList.remove("responseOk");
            responseDiv.classList.add("responseError");
        });
        return false;
    }
    if (window.location.search.length > 1) {
        const elements = window.location.search.substring(1).trim().split("&");
        securityToken1.value = elements[0].trim();
        if (elements[1] == "2factor") {
            use2FactorAuth = true;
            securityToken2.parentElement.parentElement.style.display = 'block';
        }
    }
    securityToken1.onchange = () => {
        ToogleSaveButton();
    };
    securityToken1.onkeyup = () => {
        ToogleSaveButton();
    };
    securityToken2.onchange = () => {
        ToogleSaveButton();
    };
    securityToken2.onkeyup = () => {
        ToogleSaveButton();
    };
    newPassword1.onchange = () => {
        ToogleSaveButton();
    };
    newPassword1.onkeyup = () => {
        ToogleSaveButton();
    };
    newPassword2.onchange = () => {
        ToogleSaveButton();
    };
    newPassword2.onkeyup = () => {
        ToogleSaveButton();
    };
    loginform.onsubmit = () => {
        return SetPassword();
    };
    gotoLoginButton.onclick = () => {
        window.location.href = (URLPathPrefix !== null && URLPathPrefix !== void 0 ? URLPathPrefix : "") + "/login";
    };
    DeleteCookie(HTTPCookieId);
    ToogleSaveButton();
}
function SignIn() {
    const SignInPanel = document.querySelector('#login');
    const Username = SignInPanel.querySelector('#_login').value.toLowerCase();
    const Realm = SignInPanel.querySelector('#_realm').value.toLowerCase();
    const Password = SignInPanel.querySelector('#_password').value;
    const RememberMe = SignInPanel.querySelector('#_rememberme').checked;
    const SignInErrors = SignInPanel.querySelector('#errors');
    SignInErrors.style.display = "none";
    SignInErrors.innerText = "";
    SendJSON("AUTH", (URLPathPrefix !== null && URLPathPrefix !== void 0 ? URLPathPrefix : "") + "/users/" + Username, {
        "realm": Realm,
        "password": Password,
        "rememberme": RememberMe
    }, function (status, response) {
        //(<HTMLFormElement> document.querySelector('#loginform')).submit();
        location.href = URLPathPrefix != null && URLPathPrefix != "" ? URLPathPrefix : "/";
    }, function (HTTPStatus, status, response) {
        SignInErrors.style.display = "block";
        SignInErrors.innerText = JSON.parse(response).description;
    });
}
function checkSignedIn(RedirectUnkownUsers) {
    WithCookie(HTTPCookieId, cookie => {
        isAdmin = "false";
        Astronaut = "";
        // Crumbs are base64 encoded!
        cookie.split(":").forEach(crumb => {
            if (crumb.indexOf("login") >= 0)
                SignInUser = atob(crumb.split("=")[1]);
            if (crumb.indexOf("username") >= 0)
                Username = atob(crumb.split("=")[1]);
            if (crumb.indexOf("email") >= 0)
                UserEMail = atob(crumb.split("=")[1]);
            if (crumb.indexOf("astronaut") >= 0)
                Astronaut = atob(crumb.split("=")[1]);
            if (crumb.indexOf("isAdminRO") >= 0) {
                isAdmin = "readOnly";
                ShowElement('#admin');
                ShowElement('.admin');
            }
            if (crumb.indexOf("isAdminRW") >= 0) {
                isAdmin = "readWrite";
                ShowElement('#admin');
                ShowElement('.admin');
            }
            if (crumb.indexOf("language") >= 0) {
                UILanguage = atob(crumb.split("=")[1]);
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
    }, () => {
        HideElement('#SignOut');
        HideElement('.SignOut');
        HideElement('#profile');
        HideElement('.profile');
        ShowElement('#SignIn');
        ShowElement('.SignIn');
        const usernameDiv = document.querySelector('#username');
        if (usernameDiv != null)
            usernameDiv.innerText = "anonymous";
        if (RedirectUnkownUsers)
            location.href = (URLPathPrefix !== null && URLPathPrefix !== void 0 ? URLPathPrefix : "") + "/login";
    });
    WithCookie(newsBannersCookieId, cookie => checkNewsBanner(cookie.split(":")), () => checkNewsBanner([]));
}
function checkAdminSignedIn(RedirectUnkownUsers) {
    WithCookie(HTTPCookieId, cookie => {
        ShowElement('#admin');
        ShowElement('.admin');
        if (cookie.indexOf(":isAdmin") < 0)
            location.href = URLPathPrefix != null && URLPathPrefix != "" ? URLPathPrefix : "/";
    }, () => {
        if (RedirectUnkownUsers)
            location.href = (URLPathPrefix !== null && URLPathPrefix !== void 0 ? URLPathPrefix : "") + "/login";
    });
    checkSignedIn(RedirectUnkownUsers);
}
function checkNotSignedIn() {
    WithCookie(HTTPCookieId, () => {
        location.href = (URLPathPrefix !== null && URLPathPrefix !== void 0 ? URLPathPrefix : "") + "/index.shtml";
    }, () => { });
}
function SignOut() {
    SendJSON("DEAUTH", (URLPathPrefix !== null && URLPathPrefix !== void 0 ? URLPathPrefix : "") + "/users", null, function (HTTPStatus, ResponseText) {
    }, function (HTTPStatus, StatusText, ResponseText) {
    });
    DeleteCookie(HTTPCookieId);
    location.href = (URLPathPrefix !== null && URLPathPrefix !== void 0 ? URLPathPrefix : "") + "/login";
}
function Depersonate() {
    HTTPDepersonate((URLPathPrefix !== null && URLPathPrefix !== void 0 ? URLPathPrefix : "") + "/users/" + SignInUser, (status, response) => {
        window.location.reload(true);
    }, (status, statusText, response) => {
        alert("Not allowed!");
    });
}
function checkNewsBanner(knownNewsIds) {
    const newsFilter = knownNewsIds.length > 0
        ? "?match=" + knownNewsIds.map(knownNewsId => "!" + knownNewsId).join(",")
        : "";
    HTTPGet((URLPathPrefix !== null && URLPathPrefix !== void 0 ? URLPathPrefix : "") + "/newsBanners" + newsFilter, (status, response) => {
        var _a, _b;
        const newsBanners = ParseJSON_LD(response);
        const currentDate = new Date().getTime();
        if (Array.isArray(newsBanners)) {
            const knownNewsBannerIds = (_b = (_a = GetCookie(newsBannersCookieId)) === null || _a === void 0 ? void 0 : _a.split(",")) !== null && _b !== void 0 ? _b : [];
            for (const newsBanner of newsBanners) {
                var expires = new Date(newsBanner.endTimestamp);
                if (expires.getTime() > currentDate) {
                    // The expire date of the cookies...
                    expires.setDate(expires.getDate() + 1);
                    if (knownNewsBannerIds.indexOf(newsBanner["@id"]) < 0) {
                        const newsBannerDiv = document.getElementById("newsBanner");
                        newsBannerDiv.style.display = "flex";
                        const bannerTextDiv = newsBannerDiv.querySelector("#bannerText");
                        bannerTextDiv.innerHTML = newsBanner.text != undefined && newsBanner.text != null
                            ? firstValue(newsBanner.text)
                            : "No news found!";
                        const ignoreNewsButton = newsBannerDiv.querySelector("#ignoreNewsButton");
                        ignoreNewsButton.onclick = () => {
                            var _a, _b;
                            let updatedKnownNewsBannerIds = (_b = (_a = GetCookie(newsBannersCookieId)) === null || _a === void 0 ? void 0 : _a.split(",")) !== null && _b !== void 0 ? _b : [];
                            updatedKnownNewsBannerIds.push(newsBanner["@id"]);
                            document.cookie = newsBannersCookieId + '=' + updatedKnownNewsBannerIds.join(",") + '; expires=' + expires + '; path=/';
                            newsBannerDiv.style.display = "none";
                        };
                        const clickLinks = newsBannerDiv.querySelectorAll("a.clickLink");
                        if (clickLinks != undefined && clickLinks.length > 0) {
                            for (const clickLink of clickLinks) {
                                clickLink.onclick = () => {
                                    var _a, _b;
                                    let updatedKnownNewsBannerIds = (_b = (_a = GetCookie(newsBannersCookieId)) === null || _a === void 0 ? void 0 : _a.split(",")) !== null && _b !== void 0 ? _b : [];
                                    updatedKnownNewsBannerIds.push(newsBanner["@id"]);
                                    document.cookie = newsBannersCookieId + '=' + updatedKnownNewsBannerIds.join(",") + '; expires=' + expires + '; path=/';
                                };
                            }
                        }
                    }
                }
            }
        }
    }, (status, statusText, response) => { });
}
//# sourceMappingURL=SignInOut.js.map