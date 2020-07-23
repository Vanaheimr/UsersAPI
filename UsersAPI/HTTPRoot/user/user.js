function StartUser() {
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
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid homepage/Internet URL!</div>";
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
        HTTPSet("/users/" + UserProfileJSON["@id"], UserProfileJSON, (status, response) => {
            try {
                const responseJSON = JSON.parse(response);
                responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully stored updated user profile data.</div>";
                saveButton.disabled = !AnyChangesMade();
            }
            catch (exception) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing user profile data failed:<br />" + exception + "</div>";
            }
        }, (statusCode, status, response) => {
            let responseJSON = { "description": "HTTP Error " + statusCode + " - " + status + "!" };
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
    const pathElements = window.location.pathname.split("/");
    const userId = pathElements[pathElements.length - 1];
    const userProfile = document.getElementById('userProfile');
    const impersonateButton = userProfile.querySelector("#impersonateButton");
    const data = userProfile.querySelector('#data');
    const login = data.querySelector('#login');
    const username = data.querySelector('#username');
    const eMailAddress = data.querySelector('#eMailAddress');
    const telephone = data.querySelector('#telephone');
    const mobilePhone = data.querySelector('#mobilePhone');
    const telegram = data.querySelector('#telegram');
    const homepage = data.querySelector('#homepage');
    const userLanguage = data.querySelector('#userLanguage');
    const description = data.querySelector('#userDescription');
    const descriptionText = data.querySelector('#description');
    const responseDiv = document.getElementById("response");
    const lowerButtonsDiv = userProfile.querySelector('#lowerButtons');
    const saveButton = lowerButtonsDiv.querySelector("#saveButton");
    login.value = userId;
    HTTPGet("/users/" + userId, (status, response) => {
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
            if (UserProfileJSON["youCanEdit"]) {
                impersonateButton.disabled = false;
                impersonateButton.style.display = "block";
                impersonateButton.onclick = () => {
                    ImpersonateUser(userId);
                };
                username.readOnly = false;
                username.onchange = () => { ToogleSaveButton(); };
                username.onkeyup = () => { ToogleSaveButton(); };
                eMailAddress.readOnly = false;
                eMailAddress.onchange = () => { ToogleSaveButton(); };
                eMailAddress.onkeyup = () => { ToogleSaveButton(); };
                telephone.readOnly = false;
                telephone.onchange = () => { ToogleSaveButton(); };
                telephone.onkeyup = () => { ToogleSaveButton(); };
                mobilePhone.readOnly = false;
                mobilePhone.onchange = () => { ToogleSaveButton(); };
                mobilePhone.onkeyup = () => { ToogleSaveButton(); };
                telegram.readOnly = false;
                telegram.onchange = () => { ToogleSaveButton(); };
                telegram.onkeyup = () => { ToogleSaveButton(); };
                homepage.readOnly = false;
                homepage.onchange = () => { ToogleSaveButton(); };
                homepage.onkeyup = () => { ToogleSaveButton(); };
                //userLanguage.readOnly            = false;
                userLanguage.onchange = () => { ToogleSaveButton(); };
                descriptionText.readOnly = false;
                descriptionText.onchange = () => { ToogleSaveButton(); };
                descriptionText.onkeyup = () => { ToogleSaveButton(); };
                saveButton.style.display = "block";
                saveButton.onclick = () => {
                    SaveData();
                };
            }
        }
        catch (exception) {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch user data from server:<br />" + exception + "</div>";
        }
    }, (statusCode, status, response) => {
        try {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch user data from server!</div>";
        }
        catch (exception) {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch user data from server:<br />" + exception + "</div>";
        }
    });
}
//# sourceMappingURL=user.js.map