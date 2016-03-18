//@ sourceURL=SignUp.js

function HideOverlay(target) {

    if (target == null)
        return;

    var element = target.parentNode.querySelector('.overlay');

    if (element != null)
        element.style.display = "none";

}

// Change a few things up and try submitting again.
function ShowError(target, text) {

    var element = target.parentNode.querySelector('.overlay');

    if (text == null)
    {
        element.innerHTML = "";
        element.className = "overlay overlayError";
    }

    else
    {
        element.innerHTML = text;
        element.className = "overlay overlayError overlayErrorText";
    }

    element.style.display = "inline-block";

    return false;

}

// Better check yourself, you're not looking too good.
function ShowWarning(target, text) {

    var element = target.parentNode.querySelector('.overlay');

    if (text == null)
    {
        element.innerHTML = "";
        element.className = "overlay overlayWarning";
    }

    else
    {
        element.innerHTML = text;
        element.className = "overlay overlayWarning overlayWarningText";
    }

    element.style.display = "inline-block";

    return true;

}

// This alert needs your attention, but it's not super important.
function ShowNotice(target, text) {

    var element = target.parentNode.querySelector('.overlay');

    if (text == null) {
        element.innerHTML = "";
        element.className = "overlay overlayNotice";
    }

    else {
        element.innerHTML = text;
        element.className = "overlay overlayNotice overlayNoticeText";
    }

    element.style.display = "inline-block";

    return true;

}

// Everything is 200 Ok!
function ShowOk(target, text) {

    var element = target.parentNode.querySelector('.overlay');

    if (text == null)
    {
        element.innerHTML = "";
        element.className = "overlay overlayOk";
    }

    else
    {
        element.innerHTML = text;
        element.className = "overlay overlayOk overlayOkText";
    }

    element.style.display = "inline-block";

    return true;

}



function SignUp_Verify_Name(event) {

    var target = event != null
                  ? event.target
                  : document.querySelector('#signupform').
                             querySelector('#name');

    var name = target.value;

    if (name.length < 4)
        return ShowError(target, "Der Name ist zu kurz!");

    if (!name.match(/^[a-zA-Z0-9-._]{4,}$/g))
        return ShowError(target, "Ungültiger Name!");

    SendJSON("/users/" + name,
             "EXISTS",
             null,

             function (HTTPStatus, ResponseText) {
                 alert("Gibt's schon! " + ResponseText);
             },

             function (HTTPStatus, StatusText, ResponseText) {
                 //alert("Error! " + StatusText + "\n" + ResponseText);
             });

    return ShowOk(target);

}

function SignUp_Verify_EMail(event) {

    var target = event != null
                  ? event.target
                  : document.querySelector('#signupform').querySelector('#email');

    var email = target.value;

    //ToDo: See rfc5322 for more complex regular expression!
    //      [a-z0-9!#$%&'*+/=?^_`{|}~-]+
    // (?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*
    //                 @
    // (?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+
    //    [a-z0-9](?:[a-z0-9-]*[a-z0-9])?
    if (!email.match(/^[a-z0-9]+[a-z0-9-._+%]*@[a-z0-9][a-z0-9-._]+\.[a-z]{2,}$/g))
        return ShowError(target, "Ungültige E-Mail Adresse!");

    return ShowOk(target);

}

function SignUp_Verify_GPGPublicKey(event) {

    var target = event != null
                  ? event.target
                  : document.querySelector('#signupform').querySelector('#gpgpublickeyring');

    var gpgpublickeyring = target.value;

    if (gpgpublickeyring === "") {
        HideOverlay(target);
        return true;
    }

    var publicKey = window.openpgp.key.readArmored(gpgpublickeyring);

    if (publicKey.err != undefined && publicKey.err.length >= 1)
        return ShowError(target, publicKey.err[0]);

    if (publicKey.keys.length == 0)
        return ShowError(target, "Ungültiger OpenGPG public key!");

    var UserId = publicKey.keys[0].users[0].userId.userid;

    var matches = UserId.match("(.*?)<(.*?)>");
    if (matches.length != 3)
        return ShowError(target, "Ungültige Nutzerkennung im OpenGPG public key!");

    var UserName      = matches[1];
    var UserNameForm = document.querySelector('#signupform').querySelector('#name');
    if (UserNameForm.value === "") {
        UserNameForm.value = UserName.replace(/[^a-zA-Z0-9-._]/g, "");
        SignUp_Verify_Name();
    }

    var EMailAddress = matches[2];
    var EMailForm = document.querySelector('#signupform').querySelector('#email');
    if (EMailForm.value === "") {
        EMailForm.value = EMailAddress.replace(/[^a-z0-9-._@]/g, "");
        SignUp_Verify_EMail();
    }

    return ShowOk(target, UserName + "&lt;" + EMailAddress + "&gt;<br />keyId: 0x" + publicKey.keys[0].primaryKey.keyid.toHex().toUpperCase());

}

function SignUp_Verify_Password1(event) {

    var target = event != null
                  ? event.target
                  : document.querySelector('#signupform').
                             querySelector('#password1');

    var password1 = target.value;

    if (password1.length < 8)
        return ShowError(target, "Das Passwort ist zu kurz!");
    else {
        SignUp_Verify_Password2();
        return ShowOk(target);
    }

}

function SignUp_Verify_Password2(event) {

    var target = event != null
                  ? event.target
                  : document.querySelector('#signupform').querySelector('#password2');

    var password1 = document.querySelector('#signupform').querySelector('#password1').value;
    var password2 = target.value;

    if (password1 === password2)
        return ShowOk(target);
    else
        return ShowError(target, "Die Passwörter stimmen nicht überein!");

}


function VerifyAndSubmit(event) {

    // Will always check all input fields!
    if (SignUp_Verify_Name()         &
        SignUp_Verify_EMail()        &
        SignUp_Verify_GPGPublicKey() &
        SignUp_Verify_Password1()    &
        SignUp_Verify_Password2()) {

        var SignUpForm = document.querySelector('#signupform');

        var SubmitData = {
            "email":            SignUpForm.
                                    querySelector('#email').
                                    value,
            "gpgpublickeyring": SignUpForm.
                                    querySelector('#gpgpublickeyring').
                                    value,
            "password":         SignUpForm.
                                    querySelector('#password1').
                                    value,
        };

        SendJSON("/users/" + SignUpForm.querySelector('#name').value,
                 "ADD",
                 SubmitData,

            function (HTTPStatus, ResponseText) {

                alert("OK! " + ResponseText);

            },

            function (HTTPStatus, StatusText, ResponseText) {

                alert("Error! " + StatusText + "\n" + ResponseText);

            });

    }

}



// Delayed execution! As this should not run
// before rendering the HTML is ready!
setTimeout(function () { 

    document.
        querySelector('#signupform').
        querySelector('#name').
        addEventListeners2(function (keyCode, charCode) {

                               return ((charCode >= 48 && charCode <=  57) || // numbers
                                       (charCode >= 65 && charCode <=  90) || // big letters
                                       (charCode >= 97 && charCode <= 122) || // small letters
                                        charCode == 45                     || // -
                                        charCode == 46                     || // .
                                        charCode == 95                        // _
                                        //charCode >= 228 || // ä
                                        //charCode >= 246 || // ö
                                        //charCode >= 252 || // ü
                                        //charCode >= 196 || // Ä
                                        //charCode >= 214 || // Ö
                                        //charCode >= 220 || // Ü
                                        //charCode >= 223 || // ß
                                        //charCode >= 233    // é

                                      ) ? true : false

                           },
                           SignUp_Verify_Name);


    document.
        querySelector('#email').
        addEventListeners2(function (keyCode, charCode) {

                               return ((charCode >= 48 && charCode <=  57) || // numbers
                                       (charCode >= 97 && charCode <= 122) || // small letters
                                        charCode == 37                     || // %
                                        charCode == 43                     || // +
                                        charCode == 45                     || // -
                                        charCode == 46                     || // .
                                        charCode == 95                     || // _
                                        charCode == 64                        // @
                                      ) ? true : false

                           },
                           SignUp_Verify_EMail);

    // Pasting OpenPGP/GPG keys (any input!) will reduce the fontsize of the textarea
    document.
        querySelector('#signupform').
        querySelector('#gpgpublickeyring').
        addEventListeners2(function (keyCode, charCode) {
                               return true;
                           },
                           function (e) {
                               setTimeout(function () {
                                   e.target.style.fontSize = "50%";
                                   SignUp_Verify_GPGPublicKey(e);
                               }, 100);
                           });

    document.
        querySelector('#signupform').
        querySelector('#password1').
        addEventListeners2(function (keyCode, charCode) {
                               return true;
                           },
                           SignUp_Verify_Password1);

    document.
        querySelector('#signupform').
        querySelector('#password2').
        addEventListeners2(function (keyCode, charCode) {
                               return true;
                           },
                           SignUp_Verify_Password2);

    document.
        querySelector('#signupform').
        querySelector('#SubmitButton').
        addEventListener("click",
                         VerifyAndSubmit);

}, 1000);


