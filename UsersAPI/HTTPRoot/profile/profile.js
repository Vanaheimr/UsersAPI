function StartProfile() {
    function AnyChangesMade() {
        // name
        if ((UserProfileJSON.name !== undefined ? UserProfileJSON.name : "") !== username.value) {
            if (username.value == "") {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a username!</div>";
                return false;
            }
            return true;
        }
        // email
        if ((UserProfileJSON.email !== undefined ? UserProfileJSON.email : "") !== eMailAddress.value) {
            if (eMailAddress.value == "" || eMailAddress.value.length < 6 || eMailAddress.value.indexOf("@") < 1 || eMailAddress.value.indexOf(".") < 4) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid e-mail address!</div>";
                return false;
            }
            return true;
        }
        // telephone
        if ((UserProfileJSON.telephone !== undefined ? UserProfileJSON.telephone : "") !== telephone.value) {
            if (telephone.value != "" && telephone.value.length < 6) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid telephone number!</div>";
                return false;
            }
            return true;
        }
        // mobilePhone
        if ((UserProfileJSON.mobilePhone !== undefined ? UserProfileJSON.mobilePhone : "") !== mobilePhone.value) {
            if (mobilePhone.value != "" && mobilePhone.value.length < 6) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid mobile phone number!</div>";
                return false;
            }
            return true;
        }
        // telegram
        if ((UserProfileJSON.telegram !== undefined ? UserProfileJSON.telegram : "") !== telegram.value) {
            if (telegram.value != "" && telegram.value.length < 4) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid telegram user name!</div>";
                return false;
            }
            return true;
        }
        // homepage
        if ((UserProfileJSON.homepage !== undefined ? UserProfileJSON.homepage : "") !== homepage.value) {
            if (homepage.value != "" && homepage.value.length < 4) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid homepage!</div>";
                return false;
            }
            return true;
        }
        // userLanguage
        if ((UserProfileJSON.language !== undefined ? UserProfileJSON.language : "") !== userLanguage.value) {
            if (userLanguage.value == "") {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid language setting!</div>";
                return false;
            }
            return true;
        }
        // description
        if ((UserProfileJSON.description !== undefined ? firstValue(UserProfileJSON.description) : "") !== descriptionText.value) {
            if (descriptionText.value != "" && descriptionText.value.length < 4) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid description!</div>";
                return false;
            }
            return true;
        }
        return false;
    }
    function ToogleSaveButton() {
        var changesDetected = AnyChangesMade();
        saveButton.disabled = !changesDetected;
        if (changesDetected)
            responseDiv.innerHTML = "";
        return changesDetected;
    }
    function SaveData() {
        // name
        if ((UserProfileJSON.name !== undefined ? UserProfileJSON.name : "") !== username.value)
            UserProfileJSON.name = username.value;
        // email
        if ((UserProfileJSON.email !== undefined ? UserProfileJSON.email : "") !== eMailAddress.value)
            UserProfileJSON.email = eMailAddress.value;
        // telephone
        if ((UserProfileJSON.telephone !== undefined ? UserProfileJSON.telephone : "") !== telephone.value)
            UserProfileJSON.telephone = telephone.value;
        // mobilePhone
        if ((UserProfileJSON.mobilePhone !== undefined ? UserProfileJSON.mobilePhone : "") !== mobilePhone.value)
            UserProfileJSON.mobilePhone = mobilePhone.value;
        // telegram
        if ((UserProfileJSON.telegram !== undefined ? UserProfileJSON.telegram : "") !== telegram.value)
            UserProfileJSON.telegram = telegram.value;
        // homepage
        if ((UserProfileJSON.homepage !== undefined ? UserProfileJSON.homepage : "") !== homepage.value)
            UserProfileJSON.homepage = homepage.value;
        // user language
        if ((UserProfileJSON.language !== undefined ? UserProfileJSON.language : "") !== userLanguage.selectedOptions[0].value)
            UserProfileJSON.language = userLanguage.selectedOptions[0].value;
        // description
        if ((UserProfileJSON.description !== undefined ? firstValue(UserProfileJSON.description) : "") !== descriptionText.value)
            UserProfileJSON.description = { "eng": firstValue(UserProfileJSON.description) };
        if (UserProfileJSON.telephone === "")
            delete (UserProfileJSON.telephone);
        if (UserProfileJSON.mobilePhone === "")
            delete (UserProfileJSON.mobilePhone);
        if (UserProfileJSON.telegram === "")
            delete (UserProfileJSON.telegram);
        if (UserProfileJSON.homepage === "")
            delete (UserProfileJSON.homepage);
        if (UserProfileJSON.language === "")
            delete (UserProfileJSON.language);
        if (descriptionText.value === "")
            delete (UserProfileJSON.description);
        HTTPSet("/users/" + UserProfileJSON["@id"], UserProfileJSON, function (status, response) {
            try {
                var responseJSON = JSON.parse(response);
                responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully stored updated user profile data.</div>";
                saveButton.disabled = !AnyChangesMade();
            }
            catch (exception) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing user profile data failed:<br />" + exception + "</div>";
            }
        }, function (statusCode, status, response) {
            var responseJSON = { "description": "HTTP Error " + statusCode + " - " + status + "!" };
            if (response != null && response != "") {
                try {
                    responseJSON = JSON.parse(response);
                }
                catch (_a) { }
            }
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing user profile data failed!" +
                (responseJSON.description != null ? "<br />" + responseJSON.description : "") +
                "</div>";
        });
    }
    checkSignedIn(false);
    var userProfile = document.getElementById('userProfile');
    var data = userProfile.querySelector('#data');
    var login = data.querySelector('#login');
    var username = data.querySelector('#username');
    var eMailAddress = data.querySelector('#eMailAddress');
    var telephone = data.querySelector('#telephone');
    var mobilePhone = data.querySelector('#mobilePhone');
    var telegram = data.querySelector('#telegram');
    var homepage = data.querySelector('#homepage');
    var userLanguage = data.querySelector('#userLanguage');
    var description = data.querySelector('#userDescription');
    var descriptionText = data.querySelector('#description');
    var responseDiv = document.getElementById("response");
    var lowerButtonsDiv = userProfile.querySelector('#lowerButtons');
    var saveButton = lowerButtonsDiv.querySelector("#saveButton");
    login.value = SignInUser;
    username.value = Username;
    eMailAddress.value = UserEMail;
    username.onchange = function () { ToogleSaveButton(); };
    username.onkeyup = function () { ToogleSaveButton(); };
    eMailAddress.onchange = function () { ToogleSaveButton(); };
    eMailAddress.onkeyup = function () { ToogleSaveButton(); };
    telephone.onchange = function () { ToogleSaveButton(); };
    telephone.onkeyup = function () { ToogleSaveButton(); };
    mobilePhone.onchange = function () { ToogleSaveButton(); };
    mobilePhone.onkeyup = function () { ToogleSaveButton(); };
    telegram.onchange = function () { ToogleSaveButton(); };
    telegram.onkeyup = function () { ToogleSaveButton(); };
    homepage.onchange = function () { ToogleSaveButton(); };
    homepage.onkeyup = function () { ToogleSaveButton(); };
    userLanguage.onchange = function () { ToogleSaveButton(); };
    descriptionText.onchange = function () { ToogleSaveButton(); };
    descriptionText.onkeyup = function () { ToogleSaveButton(); };
    saveButton.onclick = function () { SaveData(); };
    HTTPGet("/users/" + SignInUser, function (status, response) {
        var _a, _b, _c, _d;
        try {
            UserProfileJSON = ParseJSON_LD(response);
            username.value = UserProfileJSON.name;
            eMailAddress.value = UserProfileJSON.email;
            telephone.value = (_a = UserProfileJSON.telephone) !== null && _a !== void 0 ? _a : "";
            mobilePhone.value = (_b = UserProfileJSON.mobilePhone) !== null && _b !== void 0 ? _b : "";
            telegram.value = (_c = UserProfileJSON.telegram) !== null && _c !== void 0 ? _c : "";
            homepage.value = (_d = UserProfileJSON.homepage) !== null && _d !== void 0 ? _d : "";
            if (UserProfileJSON.language !== undefined)
                userLanguage.add(new Option(languageKey2Text(UserProfileJSON.language, UILanguage), UserProfileJSON.language, true, true));
            UpdateI18N(description, UserProfileJSON.description);
        }
        catch (exception) {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch user data from server:<br />" + exception + "</div>";
        }
    }, function (statusCode, status, response) {
        try {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch user data from server!</div>";
        }
        catch (exception) {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch user data from server:<br />" + exception + "</div>";
        }
    });
}
//# sourceMappingURL=profile.js.map