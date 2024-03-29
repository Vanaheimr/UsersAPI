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
        if ((UserProfileJSON.email !== undefined ? UserProfileJSON.email : "") !== eMail.value) {
            if (eMail.value == "" || eMail.value.length < 6 || eMail.value.indexOf("@") < 1 || eMail.value.indexOf(".") < 1 || eMail.value.indexOf("@") > eMail.value.lastIndexOf(".")) {
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
        if ((UserProfileJSON.language !== undefined ? UserProfileJSON.language : "") !== language.value) {
            if (language.value == "") {
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
        if ((UserProfileJSON.email !== undefined ? UserProfileJSON.email : "") !== eMail.value)
            UserProfileJSON.email = eMail.value;
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
        if ((UserProfileJSON.language !== undefined ? UserProfileJSON.language : "") !== language.selectedOptions[0].value)
            UserProfileJSON.language = language.selectedOptions[0].value;
        // description
        if ((UserProfileJSON.description !== undefined ? firstValue(UserProfileJSON.description) : "") !== descriptionText.value)
            UserProfileJSON.description = { "en": descriptionText.value };
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
    checkSignedIn(false);
    const userProfile = document.getElementById('userProfile');
    const data = userProfile.querySelector('#data');
    const login = data.querySelector('#login');
    const username = data.querySelector('#username');
    const eMail = data.querySelector('#eMail');
    const language = data.querySelector('#language');
    const telephone = data.querySelector('#telephone');
    const mobilePhone = data.querySelector('#mobilePhone');
    const telegram = data.querySelector('#telegram');
    const homepage = data.querySelector('#homepage');
    const description = data.querySelector('#userDescription');
    const descriptionText = data.querySelector('#description');
    const usernameError = username.parentElement.querySelector('.validationError');
    const eMailError = eMail.parentElement.querySelector('.validationError');
    const telephoneError = telephone.parentElement.querySelector('.validationError');
    const mobilePhoneError = mobilePhone.parentElement.querySelector('.validationError');
    const homepageError = homepage.parentElement.querySelector('.validationError');
    const responseDiv = document.getElementById("response");
    const lowerButtonsDiv = userProfile.querySelector('#lowerButtons');
    const saveButton = lowerButtonsDiv.querySelector("#saveButton");
    login.value = SignInUser;
    username.value = Username;
    eMail.value = UserEMail;
    username.oninput = () => { VerifyName(); };
    eMail.oninput = () => { VerifyEMail(); };
    language.onchange = () => { ToogleSaveButton(); };
    telephone.oninput = () => { VerifyTelephone(); };
    mobilePhone.oninput = () => { VerifyMobilephone(); };
    telegram.oninput = () => { ToogleSaveButton(); };
    homepage.oninput = () => { VerifyHomepage(); };
    descriptionText.oninput = () => { ToogleSaveButton(); };
    saveButton.onclick = () => { SaveData(); };
    function VerifyName() {
        const name = username.value;
        if (/^(.{4,})$/.test(name) == false) {
            saveButton.disabled = true;
            username.classList.add("error");
            usernameError.innerText = "Invalid user name!";
            usernameError.style.display = "flex";
        }
        else {
            username.classList.remove("error");
            usernameError.style.display = "none";
            ToogleSaveButton();
        }
    }
    function VerifyEMail() {
        const EMail = eMail.value.trim();
        eMail.value = EMail;
        if (/^(\S{1,}@\S{2,}\.\S{2,})$/.test(EMail) == false) {
            saveButton.disabled = true;
            eMail.classList.add("error");
            eMailError.innerText = "Invalid e-mail address!";
            eMailError.style.display = "flex";
        }
        else {
            eMail.classList.remove("error");
            eMailError.style.display = "none";
            VerifyAll();
        }
    }
    function VerifyTelephone() {
        const Telephone = telephone.value.trim();
        telephone.value = Telephone;
        if (Telephone == "") {
            telephone.classList.remove("error");
            telephoneError.style.display = "none";
        }
        else if (/^(\+?[0-9\ \-\/]{5,30})$/.test(Telephone) == false) {
            saveButton.disabled = true;
            telephone.classList.add("error");
            telephoneError.innerText = "Invalid telephone number!";
            telephoneError.style.display = "flex";
        }
        else {
            telephone.classList.remove("error");
            telephoneError.style.display = "none";
            VerifyAll();
        }
    }
    function VerifyMobilephone() {
        const MobilePhone = mobilePhone.value.trim();
        mobilePhone.value = MobilePhone;
        if (MobilePhone == "") {
            mobilePhone.classList.remove("error");
            mobilePhoneError.style.display = "none";
        }
        else if (/^(\+?[0-9\ \-\/]{5,30})$/.test(MobilePhone) == false) {
            saveButton.disabled = true;
            mobilePhone.classList.add("error");
            mobilePhoneError.innerText = "Invalid mobile phone number!";
            mobilePhoneError.style.display = "flex";
        }
        else {
            mobilePhone.classList.remove("error");
            mobilePhoneError.style.display = "none";
            VerifyAll();
        }
    }
    function VerifyHomepage() {
        const Homepage = homepage.value.trim();
        homepage.value = Homepage;
        if (Homepage == "") {
            homepage.classList.remove("error");
            homepageError.style.display = "none";
        }
        else if (/^(http:\/\/|https:\/\/)(\S{2,}\.\S{2,})$/.test(Homepage) == false) {
            saveButton.disabled = true;
            homepage.classList.add("error");
            homepageError.innerText = "Invalid homepage URL!";
            homepageError.style.display = "flex";
        }
        else {
            homepage.classList.remove("error");
            homepageError.style.display = "none";
            VerifyAll();
        }
    }
    function VerifyAll() {
        //if (newUserJSON["@id"]      != null &&
        //    newUserJSON.username    != ""   &&
        //    newUserJSON.email       != ""   &&
        //    newUserJSON.telephone   != null &&
        //    newUserJSON.mobilephone != null &&
        //    newUserJSON.homepage    != null)
        //{
        //    saveButton.disabled    = false;
        //    responseDiv.innerHTML  = "";
        //}
        ToogleSaveButton();
    }
    HTTPGet("/users/" + SignInUser, (status, response) => {
        var _a, _b, _c, _d;
        try {
            UserProfileJSON = ParseJSON_LD(response);
            username.value = UserProfileJSON.name;
            eMail.value = UserProfileJSON.email;
            telephone.value = (_a = UserProfileJSON.telephone) !== null && _a !== void 0 ? _a : "";
            mobilePhone.value = (_b = UserProfileJSON.mobilePhone) !== null && _b !== void 0 ? _b : "";
            telegram.value = (_c = UserProfileJSON.telegram) !== null && _c !== void 0 ? _c : "";
            homepage.value = (_d = UserProfileJSON.homepage) !== null && _d !== void 0 ? _d : "";
            if (UserProfileJSON.language !== undefined)
                language.add(new Option(languageKey2Text(UserProfileJSON.language, UILanguage), UserProfileJSON.language, true, true));
            UpdateI18N(description, UserProfileJSON.description);
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
//# sourceMappingURL=profile.js.map