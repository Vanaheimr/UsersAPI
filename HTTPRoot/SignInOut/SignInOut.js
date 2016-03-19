//@ sourceURL=/shared/UsersAPI/SignInOut.js

var SignInUser   = "";
var HTTPCookieId = "OpenDataSocial";

function toggleLoginPanelSmall() {

    var SignInPanel = document.querySelector('#SignInPanel');

    // Hide...
    if (SignInPanel.style.display == "block")
        SignInPanel.style.display = "none";

    // ...or show and focus login input field
    else
    {
        SignInPanel.style.display = "block";
        SignInPanel.querySelector('#username').focus();
    }

}

function showLoginPanelSmall_Realm() {
    ShowElement("#SignInPanelRealm", "block");
    HideElement("#SignInPanelShowRealmButton");
}

function hideLoginPanelSmall_Realm() {

    HideElement("#SignInPanelRealm");
    ShowElement("#SignInPanelShowRealmButton");

    document.querySelector('#signinpanelsmall').
             querySelector('#realm').
             value = "";

}

function sign_in() {

    var SignInPanel  = document.   querySelector('#SignInPanel');
    var Realm        = SignInPanel.querySelector('#realm').value;
    var SignInErrors = SignInPanel.querySelector('#errors');

    var SubmitData = {
        "password":  SignInPanel.
                         querySelector('#password').
                         value,
    };

    if (Realm !== "")
        SubmitData["realm"] = Realm;

    SignInErrors.style.display = "none";
    SignInErrors.innerText     = "";

    SendJSON("/users/" + SignInPanel.querySelector('#username').value,
             "AUTH",
             SubmitData,

        function (HTTPStatus, ResponseText) {

            var JSONResponse = JSON.parse(ResponseText);

            HideElement('#SignInPanel');
            HideElement('#SignInLink');
            ShowElement('#SignOutLink');

            var UsernameDiv = ShowElement('#usercontrols').
                              querySelector("#username").
                              innerText = JSONResponse.username;

        },

        function (HTTPStatus, StatusText, ResponseText) {

            SignInErrors.style.display  = "block";
            SignInErrors.innerText      = JSON.parse(ResponseText).description;

        });

}

function checkSignedIn() {

    var SocialOpenDataCookie = GetCookie(HTTPCookieId, function (cookie) {

        HideElement('#SignInPanel');
        HideElement('#SignInLink');
        ShowElement('#SignOutLink');

        var UsernameDiv = ShowElement('#usercontrols').querySelector("#username");

        // Crumbs are base64 encoded!
        var crumbs = cookie.split(":").forEach(function (crumbs) {

            if (crumbs.startsWith("username"))
                SignInUser            = atob(crumbs.split("=")[1]);

            if (crumbs.startsWith("name"))
                UsernameDiv.innerText = atob(crumbs.split("=")[1]);

        });

    });

}

function sign_out() {

    HideElement("#usercontrols");
    ShowElement('#SignInLink');
    HideElement('#SignOutLink');
    DeleteCookie(HTTPCookieId);

}
