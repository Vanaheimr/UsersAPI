function StartProfile() {
    function AnyChangesMade() {
        // name
        if (UserProfileJSON.name != username.value) {
            if (username.value == "") {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a username!</div>";
                return false;
            }
            return true;
        }
        // email
        if (UserProfileJSON.email != eMailAddress.value) {
            if (eMailAddress.value == "" || eMailAddress.value.length < 6 || eMailAddress.value.indexOf("@") < 1 || eMailAddress.value.indexOf(".") < 4) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid e-mail address!</div>";
                return false;
            }
            return true;
        }
        // telephone
        if (UserProfileJSON.telephone != telephone.value) {
            if (telephone.value != "" && telephone.value.length < 6) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid telephone number!</div>";
                return false;
            }
            return true;
        }
        // mobilePhone
        if (UserProfileJSON.mobilePhone != mobilePhone.value) {
            if (mobilePhone.value != "" && mobilePhone.value.length < 6) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid mobile phone number!</div>";
                return false;
            }
            return true;
        }
        // telegram
        if (UserProfileJSON.telegram != telegram.value) {
            //ToDo: Parse as number!
            if (telegram.value != "" && telegram.value.length < 4) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid telegram chat identification!</div>";
                return false;
            }
            return true;
        }
        // homepage
        if (UserProfileJSON.homepage != homepage.value) {
            if (homepage.value != "" && homepage.value.length < 4) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid homepage/Internet URL!</div>";
                return false;
            }
            return true;
        }
        // description
        var latestDescription = UserProfileJSON.description != null ? firstValue(UserProfileJSON.description) : "";
        var newDescription = descriptionText.value;
        if (latestDescription != newDescription)
            return true;
        //// publicKeyRing
        //if ((UserProfileJSON.publicKeyRing != null ? UserProfileJSON.publicKeyRing : "") != publicKeyRing.value)
        //    return true;
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
        if (UserProfileJSON.name != username.value)
            UserProfileJSON.name = username.value;
        // email
        if (UserProfileJSON.email != eMailAddress.value)
            UserProfileJSON.email = eMailAddress.value;
        // telephone
        if (UserProfileJSON.telephone != telephone.value)
            UserProfileJSON.telephone = telephone.value;
        // mobilePhone
        if (UserProfileJSON.mobilePhone != mobilePhone.value)
            UserProfileJSON.mobilePhone = mobilePhone.value;
        // mobilePhone
        if (UserProfileJSON.telegram != telegram.value)
            UserProfileJSON.telegram = telegram.value;
        // homepage
        if (UserProfileJSON.homepage != homepage.value)
            UserProfileJSON.homepage = homepage.value;
        // description
        var latestDescription = UserProfileJSON.description != null ? firstValue(UserProfileJSON.description) : "";
        var newDescription = descriptionText.value;
        if (latestDescription != newDescription) {
            if (newDescription != "") {
                if (UserProfileJSON.description == null)
                    UserProfileJSON.description = new Object();
                UserProfileJSON.description["eng"] = newDescription;
            }
            else {
                delete UserProfileJSON.description;
            }
        }
        //// publicKeyRing
        //if ((UserProfileJSON.publicKeyRing != null ? UserProfileJSON.publicKeyRing : "") != publicKeyRing.value)
        //    UserProfileJSON.publicKeyRing = publicKeyRing.value;
        if (UserProfileJSON.telephone == "")
            delete (UserProfileJSON.telephone);
        if (UserProfileJSON.mobilePhone == "")
            delete (UserProfileJSON.mobilePhone);
        if (UserProfileJSON.telegram == "")
            delete (UserProfileJSON.telegram);
        if (UserProfileJSON.homepage == "")
            delete (UserProfileJSON.homepage);
        HTTPSet("/users/" + UserProfileJSON["@id"], UserProfileJSON, function (HTTPStatus, ResponseText) {
            try {
                var responseJSON = JSON.parse(ResponseText);
                responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully stored updated user profile data.</div>";
                saveButton.disabled = !AnyChangesMade();
            }
            catch (exception) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing user profile data failed:<br />" + exception + "</div>";
            }
        }, function (HTTPStatus, StatusText, ResponseText) {
            var responseJSON = { "description": "HTTP Error " + HTTPStatus + " - " + StatusText + "!" };
            if (ResponseText != null && ResponseText != "") {
                try {
                    responseJSON = JSON.parse(ResponseText);
                }
                catch (_a) { }
            }
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing user profile data failed!" +
                (responseJSON.description != null ? "<br />" + responseJSON.description : "") +
                "</div>";
        });
    }
    var userProfile = document.getElementById('userProfile');
    var data = userProfile.querySelector('#data');
    var login = data.querySelector('#login');
    var username = data.querySelector('#username');
    var eMailAddress = data.querySelector('#eMailAddress');
    var telephone = data.querySelector('#telephone');
    var mobilePhone = data.querySelector('#mobilePhone');
    var telegram = data.querySelector('#telegram');
    var homepage = data.querySelector('#homepage');
    var description = data.querySelector('#userDescription');
    var descriptionText = data.querySelector('#description');
    //const publicKeyRing      = data.       querySelector ('#publicKeyRing')    as HTMLTextAreaElement;
    var responseDiv = document.getElementById("response");
    var lowerButtonsDiv = userProfile.querySelector('#lowerButtons');
    var saveButton = lowerButtonsDiv.querySelector("#saveButton");
    checkSignedIn(false);
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
    descriptionText.onchange = function () { ToogleSaveButton(); };
    descriptionText.onkeyup = function () { ToogleSaveButton(); };
    //publicKeyRing.onchange   = () => { ToogleSaveButton(); }
    //publicKeyRing.onkeyup    = () => { ToogleSaveButton(); }
    saveButton.onclick = function () { SaveData(); };
    HTTPGet("/users/" + SignInUser, function (status, response) {
        try {
            UserProfileJSON = ParseJSON_LD(response);
            username.value = UserProfileJSON.name;
            eMailAddress.value = UserProfileJSON.email;
            telephone.value = UserProfileJSON.telephone != null ? UserProfileJSON.telephone : "";
            mobilePhone.value = UserProfileJSON.mobilePhone != null ? UserProfileJSON.mobilePhone : "";
            telegram.value = UserProfileJSON.telegram != null ? UserProfileJSON.telegram : "";
            homepage.value = UserProfileJSON.homepage != null ? UserProfileJSON.homepage : "";
            UpdateI18N(description, UserProfileJSON.description);
            //if (UserProfileJSON.publicKeyRing != null)
            //    publicKeyRing.value  = UserProfileJSON.publicKeyRing;
            //if (UserProfileJSON.publicKeyRing == null)
            //    UserProfileJSON.publicKeyRing = "";
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