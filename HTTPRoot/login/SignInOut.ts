
var SignInUser    = "";
var Username      = "";
var UserEMail     = "";
var isAdmin       = false;

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


interface CookieEater {
    (cookie: String): void;
}


function GetCookie(CookieName: String): String {

    var CookieMatch = document.cookie.match('(^|;) ?' + CookieName + '=([^;]*)(;|$)');

    if (CookieMatch == null)
        return null;

    return CookieMatch[2];

}

function WithCookie(CookieName: String,
                    OnSucess:   CookieEater,
                    OnFailure) {

    var Cookie = GetCookie(CookieName);

    if (Cookie == null) {
        OnFailure();
        return null;
    }

    else if (OnSucess != undefined)
        OnSucess(Cookie);

}

function DeleteCookie(CookieName: String, Path) {
    DeleteCookie2(CookieName, "/");
}

function DeleteCookie2(CookieName: String, Path) {

    var CookieDateTime = new Date();
    CookieDateTime.setTime(CookieDateTime.getTime() - 86400000); // 1 day

    if (Path == undefined)
        Path = "/";

    document.cookie = CookieName += "=; expires=" + CookieDateTime.toUTCString() + "; Path=" + Path;

}


function VerifyLogin() {

    function ToogleSaveButton() {

        var validInput = _login.value.length >= 3 && _password.value.length > 5;

        loginInput.disabled = !validInput;

        if (validInput && loginInput.classList.contains("error"))
            loginInput.classList.remove("error");

        else if (!validInput && !loginInput.classList.contains("error"))
            loginInput.classList.add("error");

    }

    function VerifyPassword() : boolean {

        var ResponseText = HTTPAuth("/users/" + _login.value,
                                    "APIKey",
                                    {
                                        "login":    _login.value,
                                        "password": _password.value
                                    });

        if (ResponseText == "") {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> Could not login!";
            return false;
        }

        try {

            var responseJSON = JSON.parse(ResponseText);

            if (responseJSON.username == null || responseJSON.email == null) {

                if (responseJSON.error != null) {
                    responseDiv.style.display = 'block';
                    responseDiv.innerHTML     = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> " + responseJSON.error;
                }

                if (responseJSON.description != null) {
                    responseDiv.style.display = 'block';
                    responseDiv.innerHTML     = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> " + responseJSON.description;
                }

                else {
                    responseDiv.style.display = 'block';
                    responseDiv.innerHTML     = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> Login failed!";
                }

                return false;

            }

        }
        catch (e)
        {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> Could not login!";
            return false;
        }

        return true;

    }

    let loginform    = document.getElementById("loginform")   as HTMLFormElement;
    let _login       = document.getElementById("_login")      as HTMLInputElement;
    let _realm       = document.getElementById("_realm")      as HTMLInputElement;
    let _password    = document.getElementById("_password")   as HTMLInputElement;
    let responseDiv  = document.getElementById("response")    as HTMLDivElement;
    let loginButton  = document.getElementById("loginButton") as HTMLDivElement;
    let loginInput   = document.getElementById("loginInput")  as HTMLInputElement;

    _login.onchange    = function (this: HTMLElement, ev: Event) {
        ToogleSaveButton();
    }

    _login.onkeyup     = function (this: HTMLElement, ev: Event) {
        ToogleSaveButton();
    }

    _password.onchange = function (this: HTMLElement, ev: Event) {
        ToogleSaveButton();
    }

    _password.onkeyup  = function (this: HTMLElement, ev: Event) {
        ToogleSaveButton();
    }

    loginform.onsubmit = function (this: HTMLElement, ev: Event) {
        return VerifyPassword();
    }

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

    function ResetPassword() : boolean {

        responseDiv.style.display = 'block';
        responseDiv.innerHTML = '<i class="fa fa-spinner faa-spin animated"></i> Verifying your login... please wait!';

        HTTPSet("/resetPassword",
                "APIKey",
                {
                    "id":  _id.value
                },

                (HTTPStatus, ResponseText) => {

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

                },

                (HTTPStatus, StatusText, ResponseText) => {
                    responseDiv.style.display = 'block';
                    responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> Resetting your password failed!";
                    responseDiv.classList.remove("responseOk");
                    responseDiv.classList.add("responseError");
                });

        return false;

    }

    let loginform            = document.getElementById("loginform")           as HTMLFormElement;
    let _id                  = document.getElementById("_id")                 as HTMLInputElement;
    let _realm               = document.getElementById("_realm")              as HTMLInputElement;
    let responseDiv          = document.getElementById("response")            as HTMLDivElement;
    let resetPasswordButton  = document.getElementById("resetPasswordButton") as HTMLDivElement;
    let resetPasswordInput   = document.getElementById("resetPasswordInput")  as HTMLInputElement;

    _id.onchange = function (this: HTMLElement, ev: Event) {
        ToogleSaveButton();
    }

    _id.onkeyup = function (this: HTMLElement, ev: Event) {
        ToogleSaveButton();
    }

    loginform.onsubmit = function (this: HTMLElement, ev: Event) {
        return ResetPassword();
    }

    checkNotSignedIn();
    ToogleSaveButton();

}


function SetPassword() {

    let minPasswordLength  = 5;
    let use2FactorAuth     = false;

    function ToogleSaveButton() {

        if (securityToken1.value != "" &&
            securityToken1.value.length != 40) {

            responseDiv.style.display = 'block';
            responseDiv.innerHTML     = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> The first security token is invalid!</div>";
            responseDiv.classList.remove("responseOk");
            responseDiv.classList.add   ("responseError");

        }

        else if (securityToken2.value != "" &&
            securityToken2.value.length != 11) {

            responseDiv.style.display = 'block';
            responseDiv.innerHTML     = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> The second security token is invalid!</div>";
            responseDiv.classList.remove("responseOk");
            responseDiv.classList.add   ("responseError");

        }

        else if (newPassword1.value != "" &&
            newPassword1.value.length <= minPasswordLength) {

            responseDiv.style.display = 'block';
            responseDiv.innerHTML     = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> The new password is too short!</div>";
            responseDiv.classList.remove("responseOk");
            responseDiv.classList.add   ("responseError");

        }

        else if (newPassword2.value != "" &&
            newPassword2.value.length <= minPasswordLength) {

            responseDiv.style.display = 'block';
            responseDiv.innerHTML     = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> The new password2 is too short!</div>";
            responseDiv.classList.remove("responseOk");
            responseDiv.classList.add   ("responseError");

        }

        else if (newPassword1.value != newPassword2.value) {

            responseDiv.style.display = 'block';
            responseDiv.innerHTML     = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> The given passwords do not match!</div>";
            responseDiv.classList.remove("responseOk");
            responseDiv.classList.add("responseError");

        }

        else {
            responseDiv.style.display = 'none';
            responseDiv.innerHTML     = "";
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

    function SetPassword() : boolean {

        responseDiv.style.display = 'block';
        responseDiv.innerHTML = '<i class="fa fa-spinner faa-spin animated"></i> Verifying your request... please wait!';
        responseDiv.classList.remove("responseError");
        responseDiv.classList.remove("responseOk");

        HTTPSet("/setPassword",
                "APIKey",
                {
                    "securityToken1":  securityToken1.value,
                    "securityToken2":  securityToken2.value,
                    "newPassword":     newPassword1.value
                },

                (HTTPStatus, ResponseText) => {

                    try {

                        var responseJSON = JSON.parse(ResponseText);

                        if (responseJSON.numberOfAccountsFound != null) {
                            responseDiv.style.display = 'block';
                            responseDiv.innerHTML     = "<i class='fas fa-user-check  fa-2x menuicons'></i> Succssfully (re-)setted your password!";
                            responseDiv.classList.remove("responseError");
                            responseDiv.classList.add   ("responseOk");
                            return;
                        }

                    }
                    catch (e)
                    { }

                    responseDiv.style.display = 'block';
                    responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> Setting your password failed!";
                    responseDiv.classList.remove("responseOk");
                    responseDiv.classList.add("responseError");

                },

                (HTTPStatus, StatusText, ResponseText) => {
                    responseDiv.style.display = 'block';
                    responseDiv.innerHTML = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> Setting your password failed!";
                    responseDiv.classList.remove("responseOk");
                    responseDiv.classList.add("responseError");
                });

        return false;

    }

    let loginform          = document.getElementById("loginform")         as HTMLFormElement;
    let securityToken1     = document.getElementById("securityToken1")    as HTMLInputElement;
    let securityToken2     = document.getElementById("securityToken2")    as HTMLInputElement;
    let newPassword1       = document.getElementById("newPassword1")      as HTMLInputElement;
    let newPassword2       = document.getElementById("newPassword2")      as HTMLInputElement;
    let responseDiv        = document.getElementById("response")          as HTMLDivElement;
    let setPasswordButton  = document.getElementById("setPasswordButton") as HTMLDivElement;
    let setPasswordInput   = document.getElementById("setPasswordInput")  as HTMLInputElement;

    if (window.location.search.length > 1) {

        var elements = window.location.search.substring(1).trim().split("&");

        securityToken1.value = elements[0].trim();

        if (elements[1] == "2factor") {
            use2FactorAuth = true;
            securityToken2.parentElement.parentElement.style.display = 'block';
        }

    }

    securityToken1.onchange = function (this: HTMLElement, ev: Event) {
        ToogleSaveButton();
    }

    securityToken1.onkeyup = function (this: HTMLElement, ev: Event) {
        ToogleSaveButton();
    }


    securityToken2.onchange = function (this: HTMLElement, ev: Event) {
        ToogleSaveButton();
    }

    securityToken2.onkeyup = function (this: HTMLElement, ev: Event) {
        ToogleSaveButton();
    }


    newPassword1.onchange = function (this: HTMLElement, ev: Event) {
        ToogleSaveButton();
    }

    newPassword1.onkeyup = function (this: HTMLElement, ev: Event) {
        ToogleSaveButton();
    }


    newPassword2.onchange = function (this: HTMLElement, ev: Event) {
        ToogleSaveButton();
    }

    newPassword2.onkeyup = function (this: HTMLElement, ev: Event) {
        ToogleSaveButton();
    }

    loginform.onsubmit = function (this: HTMLElement, ev: Event) {
        return SetPassword();
    }

    checkNotSignedIn();
    ToogleSaveButton();

}



function SignIn() {

    var SignInPanel  = document.querySelector('#login');
    var Username     = (<HTMLInputElement> SignInPanel.querySelector('#_login')).     value;
    var Realm        = (<HTMLInputElement> SignInPanel.querySelector('#_realm')).     value;
    var Password     = (<HTMLInputElement> SignInPanel.querySelector('#_password')).  value;
    var RememberMe   = (<HTMLInputElement> SignInPanel.querySelector('#_rememberme')).checked;

    var SignInErrors = <HTMLElement> SignInPanel.querySelector('#errors');
    SignInErrors.style.display = "none";
    SignInErrors.innerText     = "";

    SendJSON("AUTH",
             "/users/" + Username,
             "",
             {
                 "realm":      Realm,
                 "password":   Password,
                 "rememberme": RememberMe
             },

             function (HTTPStatus, ResponseText) {
                 //(<HTMLFormElement> document.querySelector('#loginform')).submit();
                 location.href = "/";
             },

             function (HTTPStatus, StatusText, ResponseText) {

                 SignInErrors.style.display = "block";
                 SignInErrors.innerText = JSON.parse(ResponseText).description;

             });

}

function checkSignedIn(RedirectUnkownUsers: Boolean) {

    WithCookie(HTTPCookieId,

               cookie => {

                   // Crumbs are base64 encoded!
                   cookie.split(":").forEach(crumb => {

                       if (crumb.indexOf("login")    >= 0)
                           SignInUser = atob(crumb.split("=")[1]);

                       if (crumb.indexOf("username") >= 0)
                           Username  = atob(crumb.split("=")[1]);

                       if (crumb.indexOf("email") >= 0)
                           UserEMail   = atob(crumb.split("=")[1]);

                       if (crumb.indexOf("isAdmin") >= 0) {
                           isAdmin = true;
                           ShowElement('#admin');
                           ShowElement('.admin');
                       }

                   });

                   (document.querySelector('#username') as HTMLDivElement).innerText = Username;

                   ShowElement('#username');
                   ShowElement('.username');

                   ShowElement('#profile');
                   ShowElement('.profile');

                   ShowElement('#SignOut');
                   ShowElement('.SignOut');

                   HideElement('#SignIn');
                   HideElement('.SignIn');

               },

               () => {

                   HideElement('#SignOut');
                   HideElement('.SignOut');

                   HideElement('#profile');
                   HideElement('.profile');

                   ShowElement('#SignIn');
                   ShowElement('.SignIn');

                   var usernameDiv = <HTMLElement> document.querySelector('#username');

                   if (usernameDiv != null)
                       usernameDiv.innerText = "anonymous";

                   if (RedirectUnkownUsers)
                       location.href = "/login";

               }

    );

}

function checkAdminSignedIn(RedirectUnkownUsers: Boolean) {

    WithCookie(HTTPCookieId,

               cookie => {

                   ShowElement('#admin');
                   ShowElement('.admin');

                   if (cookie.indexOf(":isAdmin") < 0)
                       location.href = "/";

               },

               () => {

                   if (RedirectUnkownUsers)
                       location.href = "/login";

               }

    );

    checkSignedIn(RedirectUnkownUsers);

}

function checkNotSignedIn() {

    if (HTTPCookieId != null && HTTPCookieId != "") {

        WithCookie(HTTPCookieId,

                   cookie => {
                       location.href = "/index.html";
                   },

                   () => {
                   }

            );

    }

}

function GetLogin() : string {

    var Cookie = GetCookie(HTTPCookieId);
    var Login  = "";

    if (Cookie != null) {

        // Crumbs are base64 encoded!
        Cookie.split(":").forEach(crumb => {

            if (crumb.indexOf("login") >= 0)
                Login = atob(crumb.split("=")[1]);

        });

    }

    return Login;

}

function GetUsername(): string {

    var Cookie    = GetCookie( HTTPCookieId);
    var Username  = "";

    if (Cookie != null) {

        // Crumbs are base64 encoded!
        Cookie.split(":").forEach(crumb => {

            if (crumb.indexOf("username") >= 0)
                Username = atob(crumb.split("=")[1]);

        });

    }

    return Username;

}

function GetEMail(): string {

    var Cookie    = GetCookie( HTTPCookieId);
    var Username  = "";

    if (Cookie != null) {

        // Crumbs are base64 encoded!
        Cookie.split(":").forEach(crumb => {

            if (crumb.indexOf("email") >= 0)
                Username = atob(crumb.split("=")[1]);

        });

    }

    return Username;

}

function SignOut() {

    SendJSON("DEAUTH",
                           "/users",
                           "",
                           "",

                           function (HTTPStatus, ResponseText) {
                           },

                           function (HTTPStatus, StatusText, ResponseText) {
                           });

    DeleteCookie(HTTPCookieId, "/");

    location.href = "/login";

}
