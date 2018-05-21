
var SignInUser = "";

function HideElement(DivName) {

    var div = document.querySelector(DivName);
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
                           (<HTMLElement> document.querySelector('#username')).innerText = atob(crumb.split("=")[1]);

                       if (crumb.indexOf("isAdmin") >= 0) {
                           ShowElement('#admin');
                           ShowElement('.admin');
                       }

                   });

               },

               () => {

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
                   location.href = "/login";
               }

    );

    checkSignedIn(RedirectUnkownUsers);

}

function checkNotSignedIn() {

    WithCookie(HTTPCookieId,

               cookie => {
                   location.href = "/index.html";
               },

               () => {
               }

    );

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
