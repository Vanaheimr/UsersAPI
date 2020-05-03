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
        // homepage
        if ((UserProfileJSON.homepage !== undefined ? UserProfileJSON.homepage : "") !== homepage.value) {
            if (homepage.value != "" && homepage.value.length < 4) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid homepage/Internet URL!</div>";
                return false;
            }
            return true;
        }
        // description
        if ((UserProfileJSON.description != null ? firstValue(UserProfileJSON.description) : "") !== descriptionText.value)
            return true;
        responseDiv.innerHTML = "";
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
        if (UserProfileJSON.telephone == "")
            delete (UserProfileJSON.telephone);
        // mobilePhone
        if (UserProfileJSON.mobilePhone != mobilePhone.value)
            UserProfileJSON.mobilePhone = mobilePhone.value;
        if (UserProfileJSON.mobilePhone == "")
            delete (UserProfileJSON.mobilePhone);
        // homepage
        if (UserProfileJSON.homepage != homepage.value)
            UserProfileJSON.homepage = homepage.value;
        if (UserProfileJSON.homepage == "")
            delete (UserProfileJSON.homepage);
        // description
        var latestDescription = UserProfileJSON.description != null ? firstValue(UserProfileJSON.description) : "";
        var newDescription = descriptionText.value;
        if (latestDescription != newDescription) {
            if (newDescription != "") {
                if (UserProfileJSON.description == null)
                    UserProfileJSON.description = new Object();
                UserProfileJSON.description["eng"] = newDescription;
            }
            else
                delete UserProfileJSON.description;
        }
        HTTPSet("/users/" + UserProfileJSON["@id"], UserProfileJSON, function (HTTPStatus, ResponseText) {
            var responseJSON = JSON.parse(ResponseText);
            responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully stored updated user profile data.</div>";
            //saveButton.disabled = !AnyChangesMade();
        }, function (HTTPStatus, StatusText, ResponseText) {
            var responseJSON = { "description": "HTTP Error " + HTTPStatus + " - " + StatusText + "!" };
            if (ResponseText != null && ResponseText != "") {
                try {
                    responseJSON = JSON.parse(ResponseText);
                }
                catch (_a) { }
            }
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing user profile data failed!" + (responseJSON.description != null ? responseJSON.description : "") + "</div>";
        });
    }
    function ImpersonateUser(newUserId) {
        HTTPImpersonate("/users/" + newUserId, function (HTTPStatus, ResponseText) {
            window.location.reload(true);
        }, function (HTTPStatus, StatusText, ResponseText) {
            alert("Not allowed!");
        });
    }
    var pathElements = window.location.pathname.split("/");
    var userId = pathElements[pathElements.length - 1];
    var profileinfos = document.getElementById('profileinfos');
    var login = profileinfos.querySelector('#login');
    var username = profileinfos.querySelector('#username');
    var eMailAddress = profileinfos.querySelector('#eMailAddress');
    var telephone = profileinfos.querySelector('#telephone');
    var mobilePhone = profileinfos.querySelector('#mobilePhone');
    var homepage = profileinfos.querySelector('#homepage');
    var description = profileinfos.querySelector('#userDescription');
    var descriptionText = profileinfos.querySelector('#description');
    var responseDiv = document.getElementById("response");
    var impersonateButton = document.getElementById("impersonateButton");
    var saveButton = document.getElementById("saveButton");
    login.value = userId;
    HTTPGet("/users/" + userId, function (HTTPStatus, ResponseText) {
        UserProfileJSON = ParseJSON_LD(ResponseText);
        username.value = UserProfileJSON.name;
        eMailAddress.value = UserProfileJSON.email;
        telephone.value = UserProfileJSON.telephone != null ? UserProfileJSON.telephone : "";
        mobilePhone.value = UserProfileJSON.mobilePhone != null ? UserProfileJSON.mobilePhone : "";
        homepage.value = UserProfileJSON.homepage != null ? UserProfileJSON.homepage : "";
        UpdateI18N(description, UserProfileJSON.description);
        if (UserProfileJSON["youCanEdit"]) {
            username.readOnly = false;
            username.onchange = function () {
                ToogleSaveButton();
            };
            username.onkeyup = function () {
                ToogleSaveButton();
            };
            eMailAddress.readOnly = false;
            eMailAddress.onchange = function () {
                ToogleSaveButton();
            };
            eMailAddress.onkeyup = function () {
                ToogleSaveButton();
            };
            telephone.readOnly = false;
            telephone.onchange = function () {
                ToogleSaveButton();
            };
            telephone.onkeyup = function () {
                ToogleSaveButton();
            };
            mobilePhone.readOnly = false;
            mobilePhone.onchange = function () {
                ToogleSaveButton();
            };
            mobilePhone.onkeyup = function () {
                ToogleSaveButton();
            };
            homepage.readOnly = false;
            homepage.onchange = function () {
                ToogleSaveButton();
            };
            homepage.onkeyup = function () {
                ToogleSaveButton();
            };
            descriptionText.readOnly = false;
            descriptionText.onchange = function () {
                ToogleSaveButton();
            };
            descriptionText.onkeyup = function () {
                ToogleSaveButton();
            };
            saveButton.style.display = "block";
            saveButton.onclick = function () {
                SaveData();
            };
            impersonateButton.disabled = false;
            impersonateButton.style.display = "block";
            impersonateButton.onclick = function (ev) {
                ImpersonateUser(userId);
            };
        }
    }, function (HTTPStatus, StatusText, ResponseText) {
    });
}
//# sourceMappingURL=user.js.map