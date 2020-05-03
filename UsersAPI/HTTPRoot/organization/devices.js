function StartOrganizationDevices() {
    function AnyChangesMade() {
        //// name
        //if (UserProfileJSON.name != username.value) {
        //    if (username.value == "") {
        //        responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a username!</div>";
        //        return false;
        //    }
        //    return true;
        //}
        //// email
        //if (UserProfileJSON.email != eMailAddress.value) {
        //    if (eMailAddress.value == "" || eMailAddress.value.length < 6 || eMailAddress.value.indexOf("@") < 1 || eMailAddress.value.indexOf(".") < 4) {
        //        responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid e-mail address!</div>";
        //        return false;
        //    }
        //    return true;
        //}
        //// telephone
        //if (UserProfileJSON.telephone != telephone.value) {
        //    if (telephone.value != "" && telephone.value.length < 6) {
        //        responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid telephone number!</div>";
        //        return false;
        //    }
        //    return true;
        //}
        //// mobilePhone
        //if (UserProfileJSON.mobilePhone != mobilePhone.value) {
        //    if (mobilePhone.value != "" && mobilePhone.value.length < 6) {
        //        responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid mobile phone number!</div>";
        //        return false;
        //    }
        //    return true;
        //}
        //// description
        //let latestDescription  = UserProfileJSON.description != null ? firstValue(UserProfileJSON.description) : "";
        //let newDescription     = descriptionText.value;
        //if (latestDescription != newDescription)
        //    return true;
        //// publicKeyRing
        //if ((UserProfileJSON.publicKeyRing != null ? UserProfileJSON.publicKeyRing : "") != publicKeyRing.value)
        //    return true;
        return false;
    }
    function ToogleSaveButton() {
        var changesDetected = AnyChangesMade();
        //saveButton.disabled = !changesDetected;
        if (changesDetected)
            responseDiv.innerHTML = "";
        return changesDetected;
    }
    function SaveData() {
        //// name
        //if (UserProfileJSON.name != username.value)
        //    UserProfileJSON.name = username.value;
        //// email
        //if (UserProfileJSON.email != eMailAddress.value)
        //    UserProfileJSON.email = eMailAddress.value;
        //// telephone
        //if (UserProfileJSON.telephone != telephone.value)
        //    UserProfileJSON.telephone = telephone.value;
        //// mobilePhone
        //if (UserProfileJSON.mobilePhone != mobilePhone.value)
        //    UserProfileJSON.mobilePhone = mobilePhone.value;
        //// description
        //let latestDescription  = UserProfileJSON.description != null ? firstValue(UserProfileJSON.description) : "";
        //let newDescription     = descriptionText.value;
        //if (latestDescription != newDescription) {
        //    if (newDescription != "") {
        //        if (UserProfileJSON.description == null)
        //            UserProfileJSON.description = new Object();
        //        UserProfileJSON.description["eng"] = newDescription;
        //    }
        //    else {
        //        delete UserProfileJSON.description;
        //    }
        //}
        //// publicKeyRing
        //if ((UserProfileJSON.publicKeyRing != null ? UserProfileJSON.publicKeyRing : "") != publicKeyRing.value)
        //    UserProfileJSON.publicKeyRing = publicKeyRing.value;
        //if (UserProfileJSON.telephone   == "")
        //    delete (UserProfileJSON.telephone);
        //if (UserProfileJSON.mobilePhone == "")
        //    delete (UserProfileJSON.mobilePhone);
        //HTTPSet("/users/" + UserProfileJSON["@id"],
        //        UserProfileJSON,
        //        (HTTPStatus, ResponseText) => {
        //            var responseJSON = JSON.parse(ResponseText);
        //            responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully stored updated user profile data.</div>";
        //            saveButton.disabled = !AnyChangesMade();
        //        },
        //        (HTTPStatus, StatusText, ResponseText) => {
        //            var responseJSON = ResponseText != "" ? JSON.parse(ResponseText) : {};
        //            var info         = responseJSON.description != null ? "<br />" + responseJSON.description : "";
        //            responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing user profile data failed!" + info + "</div>";
        //        });
    }
    function AddProperty(parentDiv, className, key, content) {
        var propertyDiv = parentDiv.appendChild(document.createElement('div'));
        propertyDiv.className = "property " + className;
        var keyDiv = propertyDiv.appendChild(document.createElement('div'));
        keyDiv.className = "key";
        keyDiv.innerHTML = key;
        var valueDiv = propertyDiv.appendChild(document.createElement('div'));
        valueDiv.className = "value";
        valueDiv.innerHTML = content;
        return propertyDiv;
    }
    var pathElements = window.location.pathname.split("/");
    var organizationId = pathElements[pathElements.length - 2];
    var organizationMenuDiv = document.getElementById("organizationMenu");
    var links = organizationMenuDiv.querySelectorAll("a");
    for (var i = 0; i < links.length; i++) {
        if (links[i].href.indexOf("00000000") > 0) {
            links[i].href = links[i].href.replace("00000000", organizationId);
        }
    }
    var organizationDiv = document.getElementById("organization");
    var headlineDiv = organizationDiv.querySelector('.headline');
    var dataDiv = organizationDiv.querySelector('#data');
    var responseDiv = document.getElementById("response");
    //let saveButton           = document.    getElementById("saveButton")           as HTMLButtonElement;
    //name.onchange        = function (this: HTMLElement, ev: Event) {
    //    ToogleSaveButton();
    //}
    //name.onkeyup         = function (this: HTMLElement, ev: Event) {
    //    ToogleSaveButton();
    //}
    //description.onchange = function (this: HTMLElement, ev: Event) {
    //    ToogleSaveButton();
    //}
    //description.onkeyup  = function (this: HTMLElement, ev: Event) {
    //    ToogleSaveButton();
    //}
    //saveButton.onclick       = function (this: HTMLElement, ev: Event) {
    //    SaveData();
    //}
    var User = SignInUser;
    if (User == "ahzf" ||
        User == "lars" ||
        User == "hamzacardilink") {
        // e.g. show some buttons...
    }
    HTTPGet("/organizations/" + organizationId + "/devices?expand=defibrillators,communicators,simcards", function (HTTPStatus, ResponseText) {
        try {
            var allDevicesJSON = JSON.parse(ResponseText);
            headlineDiv.querySelector("#name #language").innerText = firstKey(allDevicesJSON["name"]);
            headlineDiv.querySelector("#name #I18NText").innerText = firstValue(allDevicesJSON["name"]);
            if (allDevicesJSON["description"]) {
                headlineDiv.querySelector("#description").style.display = "block";
                headlineDiv.querySelector("#description #language").innerText = firstKey(allDevicesJSON["description"]);
                headlineDiv.querySelector("#description #I18NText").innerText = firstValue(allDevicesJSON["description"]);
            }
            var defibrillators = allDevicesJSON["defibrillators"];
            if (defibrillators) {
                var defibrillatorsDiv = dataDiv.appendChild(document.createElement('div'));
                defibrillatorsDiv.className = "deviceType";
                var defibrillatorHeadlineDiv = defibrillatorsDiv.appendChild(document.createElement('div'));
                defibrillatorHeadlineDiv.className = "deviceHeadline";
                defibrillatorHeadlineDiv.innerHTML = "Defibrillators";
                var defibrillatorGroupDiv = defibrillatorsDiv.appendChild(document.createElement('div'));
                defibrillatorGroupDiv.className = "deviceGroup";
                var _loop_1 = function (defibrillator) {
                    defibrillatorDiv = defibrillatorGroupDiv.appendChild(document.createElement('div'));
                    defibrillatorDiv.className = "device";
                    defibrillatorDiv.onclick = function (mouseEvent) { return window.location.href = "/defibrillators/" + defibrillator["@id"]; };
                    // show description or id
                    var descriptionOrIdDiv = defibrillatorDiv.appendChild(document.createElement('div'));
                    descriptionOrIdDiv.className = "I18N descriptionOrId";
                    descriptionOrIdDiv.innerHTML = defibrillator.description != null && firstValue(defibrillator.description) != ""
                        ? ShowI18N(defibrillator.description)
                        : "<p>" + defibrillator["@id"] + "</p>";
                    var propertiesDiv = defibrillatorDiv.appendChild(document.createElement('div'));
                    propertiesDiv.className = "properties";
                    AddProperty(propertiesDiv, "model", "Model", defibrillator.hardware != null
                        ? defibrillator.hardware.model
                        : "unknown");
                    AddProperty(propertiesDiv, "adminStatus", "(Admin-)Status", firstValue(defibrillator.adminStatus) + " / " + firstValue(defibrillator.status));
                    if (defibrillator.dailySelfTests != null || defibrillator.monthlySelfTests != null) {
                        AddProperty(propertiesDiv, "selfTest", "Last self test", firstKey(defibrillator.monthlySelfTests) > firstKey(defibrillator.dailySelfTests)
                            ? "MONTHLY " + firstKey(defibrillator.monthlySelfTests)
                            : "DAILY " + firstKey(defibrillator.dailySelfTests));
                    }
                };
                var defibrillatorDiv;
                for (var _i = 0, defibrillators_1 = defibrillators; _i < defibrillators_1.length; _i++) {
                    var defibrillator = defibrillators_1[_i];
                    _loop_1(defibrillator);
                }
            }
            var communicators = allDevicesJSON["communicators"];
            if (communicators) {
                var communicatorsDiv = dataDiv.appendChild(document.createElement('div'));
                communicatorsDiv.className = "deviceType";
                var communicatorHeadlineDiv = communicatorsDiv.appendChild(document.createElement('div'));
                communicatorHeadlineDiv.className = "deviceHeadline";
                communicatorHeadlineDiv.innerHTML = "Communicators";
                var communicatorGroupDiv = communicatorsDiv.appendChild(document.createElement('div'));
                communicatorGroupDiv.className = "deviceGroup";
                var _loop_2 = function (communicator) {
                    communicatorDiv = communicatorGroupDiv.appendChild(document.createElement('div'));
                    communicatorDiv.className = "device";
                    communicatorDiv.onclick = function (mouseEvent) { return window.location.href = "/communicators/" + communicator["@id"]; };
                    // show description or id
                    var descriptionOrIdDiv = communicatorDiv.appendChild(document.createElement('div'));
                    descriptionOrIdDiv.className = "I18N descriptionOrId";
                    descriptionOrIdDiv.innerHTML = communicator.description != null && firstValue(communicator.description) != ""
                        ? ShowI18N(communicator.description)
                        : "<p>" + communicator["@id"] + "</p>";
                    var propertiesDiv = communicatorDiv.appendChild(document.createElement('div'));
                    propertiesDiv.className = "properties";
                    AddProperty(propertiesDiv, "model", "Model", communicator.hardware != null
                        ? communicator.hardware.model
                        : "unknown");
                    AddProperty(propertiesDiv, "adminStatus", "(Admin-)Status", firstValue(communicator.adminStatus) + " / " + firstValue(communicator.status));
                    if (communicator.dailySelfTests != null || communicator.monthlySelfTests != null) {
                        AddProperty(propertiesDiv, "selfTest", "Last self test", firstKey(communicator.monthlySelfTests) > firstKey(communicator.dailySelfTests)
                            ? "MONTHLY " + firstKey(communicator.monthlySelfTests)
                            : "DAILY " + firstKey(communicator.dailySelfTests));
                    }
                };
                var communicatorDiv;
                for (var _a = 0, communicators_1 = communicators; _a < communicators_1.length; _a++) {
                    var communicator = communicators_1[_a];
                    _loop_2(communicator);
                }
            }
            var simcards = allDevicesJSON["simcards"];
            if (simcards) {
                var simcardsDiv = dataDiv.appendChild(document.createElement('div'));
                simcardsDiv.innerHTML = "Simcards";
                for (var _b = 0, simcards_1 = simcards; _b < simcards_1.length; _b++) {
                    var simcard = simcards_1[_b];
                    var simcardDiv = simcardsDiv.appendChild(document.createElement('div'));
                    simcardDiv.className = "simcard";
                    simcardDiv.innerHTML = simcard["@id"];
                }
            }
            //UpdateI18N(nameDiv,        OrganizationJSON.name);
            //UpdateI18N(descriptionDiv, OrganizationJSON.description);
            //if (OrganizationJSON.parent[0] != "NoOwner")
            //    parentDiv.innerHTML  = "<a href=\"" + OrganizationJSON.parent[0] + "\">"+ OrganizationJSON.parent[0] + "</a>";
            //else
            //    parentDiv.parentElement.style.display = "none";
        }
        catch (exception) {
        }
    }, function (HTTPStatus, StatusText, ResponseText) {
    });
}
//# sourceMappingURL=devices.js.map