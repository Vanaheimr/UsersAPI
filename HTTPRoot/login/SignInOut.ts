
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

    function VerifyPassword() : boolean {

        var ResponseText = HTTPAuth("/users/" + _login.value,
                                    "APIKey",
                                    {
                                        "login":    _login.value,
                                        "password": _password.value
                                    },
                                    null,
                                    null);

        if (ResponseText == "") {
            responseDiv.style.display = 'block';
            responseDiv.innerHTML     = "<i class='fas fa-exclamation-triangle  fa-2x menuicons'></i> Login failed!";
            return false;
        }

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

        return true;

    }

    let loginform    = document.getElementById("loginform") as HTMLFormElement;
    let _login       = document.getElementById("_login")    as HTMLButtonElement;
    let _password    = document.getElementById("_password") as HTMLInputElement;
    let responseDiv  = document.getElementById("response")  as HTMLDivElement;
    let saveButton   = document.getElementById("button")    as HTMLButtonElement;

    loginform.onsubmit = function (this: HTMLElement, ev: Event) {
        return VerifyPassword();
    }

    checkNotSignedIn();

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
