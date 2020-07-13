function StartOrganizationMembers() {
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
    function ImpersonateUser(newUserId) {
        HTTPImpersonate("/users/" + newUserId, function (HTTPStatus, ResponseText) {
            window.location.reload(true);
        }, function (HTTPStatus, StatusText, ResponseText) {
            alert("Not allowed!");
        });
    }
    function ShowUser(userDiv, member) {
        var memberDiv = userDiv.appendChild(document.createElement('div'));
        memberDiv.className = "member";
        if (typeof member == 'string') {
            memberDiv.onclick = function () { return window.location.href = "../../users/" + member; };
            memberDiv.innerHTML = member;
        }
        else {
            memberDiv.onclick = function () { return window.location.href = "../../users/" + member["@id"]; };
            var frameDiv = memberDiv.appendChild(document.createElement('div'));
            frameDiv.className = "frame";
            var imageDiv = frameDiv.appendChild(document.createElement('div'));
            imageDiv.className = "avatar";
            imageDiv.innerHTML = "<img src=\"/shared/UsersAPI/images/dummy-user-gray-150x150.png\" >";
            var infoDiv = frameDiv.appendChild(document.createElement('div'));
            infoDiv.className = "info";
            var nameDiv_1 = infoDiv.appendChild(document.createElement('div'));
            nameDiv_1.className = "name";
            nameDiv_1.innerHTML = member.name;
            if (member.description) {
                var descriptionDiv = infoDiv.appendChild(document.createElement('div'));
                descriptionDiv.className = "description";
                descriptionDiv.innerHTML = ShowI18N(member.description);
            }
            var emailDiv = infoDiv.appendChild(document.createElement('div'));
            emailDiv.className = "email";
            emailDiv.innerHTML = "<i class=\"fas fa-envelope\"></i> <a href=\"mailto:" + member.email + "\">" + member.email + "</a>";
            // Authenticated
            // Accpeted EULA
            var toolsDiv = frameDiv.appendChild(document.createElement('div'));
            toolsDiv.className = "tools";
            //            toolsDiv.innerHTML = "tools";
            var impersonateUserButton = toolsDiv.appendChild(document.createElement('button'));
            impersonateUserButton.className = "impersonateUser";
            impersonateUserButton.title = "Impersonate this user!";
            impersonateUserButton.innerHTML = '<i class="fas fa-theater-masks"></i> Impersonate</i>';
            impersonateUserButton.onclick = function () {
                ImpersonateUser(member["@id"]);
            };
        }
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
    var headlineDiv = organizationDiv.querySelector("#headline");
    var addNewMemberButton = organizationDiv.querySelector('#addNewMemberButton');
    var nameDiv = organizationDiv.querySelector('#nameDiv');
    var name = organizationDiv.querySelector('#name');
    var adminsDiv = organizationDiv.querySelector('#adminsDiv');
    var membersDiv = organizationDiv.querySelector('#membersDiv');
    var responseDiv = document.getElementById("response");
    name.onchange = function () { ToogleSaveButton(); };
    name.onkeyup = function () { ToogleSaveButton(); };
    var aa = SignInUser;
    HTTPGet("/organizations/" + organizationId + "?showMgt&expand=members", function (status, response) {
        var _a, _b;
        try {
            organizationJSON = ParseJSON_LD(response);
            headlineDiv.querySelector("#name #language").innerText = firstKey(organizationJSON.name);
            headlineDiv.querySelector("#name #I18NText").innerText = firstValue(organizationJSON.name);
            if (organizationJSON.description) {
                headlineDiv.querySelector("#description").style.display = "block";
                headlineDiv.querySelector("#description #language").innerText = firstKey(organizationJSON.description);
                headlineDiv.querySelector("#description #I18NText").innerText = firstValue(organizationJSON.description);
            }
            if (((_a = organizationJSON.admins) === null || _a === void 0 ? void 0 : _a.length) > 0)
                for (var _i = 0, _c = organizationJSON.admins; _i < _c.length; _i++) {
                    var member = _c[_i];
                    ShowUser(adminsDiv, member);
                }
            if (((_b = organizationJSON.members) === null || _b === void 0 ? void 0 : _b.length) > 0)
                for (var _d = 0, _e = organizationJSON.members; _d < _e.length; _d++) {
                    var member = _e[_d];
                    ShowUser(membersDiv, member);
                }
            if (organizationJSON.youCanAddMembers) {
                addNewMemberButton.disabled = false;
                addNewMemberButton.onclick = function () { return window.location.href = "newMember"; };
            }
        }
        catch (exception) {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch organization members data from server!<br />" +
                "Exception: " + exception +
                "</div>";
        }
    }, function (statusCode, status, response) {
        try {
            var responseJSON = JSON.parse(response);
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch organization members data from server!<br />" +
                (responseJSON.description != null
                    ? responseJSON.description
                    : "HTTP Error " + statusCode + " - " + status) +
                "</div>";
        }
        catch (exception) {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch organization members data from server!<br />" +
                "Exception: " + exception +
                "</div>";
        }
    });
}
//# sourceMappingURL=members.js.map