
// Sign In and Sign Out...

var SignInUser = "";

function toggleLoginPanelSmall() {

    var SignInPanel = document.querySelector('#signinpanelsmall');

    // Hide...
    if (SignInPanel.style.display == "block")
        SignInPanel.style.display = "none";

    // ...or show and focus login input field
    else
    {
        SignInPanel.style.display = "block";
        SignInPanel.querySelector('#login').focus();
    }

}

function showLoginPanelSmall_Realm() {
    ShowDiv("#signinpanelsmall_realm", "block");
    HideDiv("#signinpanelsmall_showrealmbutton");
}

function hideLoginPanelSmall_Realm() {

    HideDiv("#signinpanelsmall_realm");
    ShowDiv("#signinpanelsmall_showrealmbutton");

    document.querySelector('#signinpanelsmall').
             querySelector('#realm').
             value = "";

}

function sign_in() {

    var SignInPanel  = document.querySelector('#signinpanelsmall');
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

    SendJSON("/users/" + SignInPanel.querySelector('#login').value,
             "AUTH",
             SubmitData,

        function (HTTPStatus, ResponseText) {

            var JSONResponse = JSON.parse(ResponseText);

            HideDiv('#anonymouscontrols');
            HideDiv('#signinpanelsmall');

            var UsernameDiv = ShowDiv('#usercontrols').
                              querySelector("#username").
                              innerText = JSONResponse.username;

        },

        function (HTTPStatus, StatusText, ResponseText) {

            SignInErrors.style.display  = "block";
            SignInErrors.innerText      = JSON.parse(ResponseText).description;

        });

}

function checkSignedIn() {

    var SocialOpenDataCookie = GetCookie("SocialOpenData", function (cookie) {

        HideDiv('#anonymouscontrols');
        HideDiv('#signinpanelsmall');

        var UsernameDiv = ShowDiv('#usercontrols').querySelector("#username");

        // Crumbs are base64 encoded!
        var crumbs = cookie.split(":").forEach(function (crumbs) {

            if (crumbs.startsWith("login"))
                SignInUser            = atob(crumbs.split("=")[1]);

            if (crumbs.startsWith("username"))
                UsernameDiv.innerText = atob(crumbs.split("=")[1]);

        });

    });

}

function sign_out() {

    ShowDiv("#anonymouscontrols");
    HideDiv("#usercontrols");
    DeleteCookie("SocialOpenData");

}
