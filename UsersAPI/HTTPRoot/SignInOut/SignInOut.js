////@ sourceURL=/shared/UsersAPI/SignInOut.js

//var HTTPCookieId  = "OpenDataSocial";
//var SignInUser    = "";
//var Username      = "";
//var UserEMail     = "";

//function ToggleLoginPanel() {

//    var SignInPanel = document.querySelector('#SignInPanel');

//    // Hide...
//    if (SignInPanel.style.display == "block")
//        SignInPanel.style.display = "none";

//    // ...or show and focus login input field
//    else
//    {
//        SignInPanel.style.display = "block";
//        SignInPanel.querySelector('#username').focus();
//    }

//}

//function showLoginPanelSmall_Realm() {
//    ShowElement("#SignInPanelRealm", "block");
//    HideElement("#SignInPanelShowRealmButton");
//}

//function hideLoginPanelSmall_Realm() {

//    HideElement("#SignInPanelRealm");
//    ShowElement("#SignInPanelShowRealmButton");

//    document.querySelector('#SignInPanel').
//             querySelector('#realm').
//             value = "";

//}

//function SignIn() {

//    var SignInPanel  = document.   querySelector('#SignInPanel');
//    var Realm        = SignInPanel.querySelector('#realm').value;
//    var SignInErrors = SignInPanel.querySelector('#errors');

//    var SubmitData = {
//        "password":  SignInPanel.
//                         querySelector('#password').
//                         value,
//    };

//    if (Realm !== "")
//        SubmitData["realm"] = Realm;

//    SignInErrors.style.display = "none";
//    SignInErrors.innerText     = "";

//    SendJSON("AUTH",
//             "/users/" + SignInPanel.querySelector('#username').value,
//             SubmitData,

//        function (HTTPStatus, ResponseText) {

//            var JSONResponse = JSON.parse(ResponseText);

//            HideElement('#SignInPanel');
//            HideElement('#SignInLink');
//            ShowElement('#SignOutLink');

//            var UsernameDiv = ShowElement('#UserControls').
//                              querySelector("#username").
//                              innerText = JSONResponse.username;

//        },

//        function (HTTPStatus, StatusText, ResponseText) {

//            SignInErrors.style.display  = "block";
//            SignInErrors.innerText      = JSON.parse(ResponseText).description;

//        });

//}

//function checkSignedIn() {

//    var SocialOpenDataCookie = GetCookie(HTTPCookieId, function (cookie) {

//        HideElement('#SignInPanel');
//        HideElement('#SignInLink');
//        ShowElement('#SignOutLink');

//        // Crumbs are base64 encoded!
//        var crumbs = cookie.split(":").forEach(function (crumbs) {

//            if (crumbs.startsWith("username"))
//                SignInUser  = atob(crumbs.split("=")[1]);

//            if (crumbs.startsWith("name"))
//                Username    = atob(crumbs.split("=")[1]);

//            if (crumbs.startsWith("email"))
//                UserEMail   = atob(crumbs.split("=")[1]);

//        });

//        ShowElement('#UserControls').querySelector("#username").innerText = Username;

//    });

//}

//function SignOut() {

//    HideElement("#UserControls");
//    ShowElement('#SignInLink');
//    HideElement('#SignOutLink');

//    SendJSON("DEAUTH",
//             "/users",

//        function (HTTPStatus, ResponseText) {
//        },

//        function (HTTPStatus, StatusText, ResponseText) {
//        });

//    DeleteCookie(HTTPCookieId);

//}
