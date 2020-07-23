///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
function ShowOrganizations() {
    var myOrgs = [];
    function ImpersonateUser(newUserId) {
        HTTPImpersonate("/users/" + newUserId, (HTTPStatus, ResponseText) => {
            window.location.reload(true);
        }, (HTTPStatus, StatusText, ResponseText) => {
            alert("Not allowed!");
        });
    }
    function PrintOrganization(organization, orgDiv) {
        function PrintUser(user, userDiv, className, printId, deleteable) {
            let wrapper = userDiv.appendChild(document.createElement('div'));
            wrapper.className = className;
            let nameDiv = wrapper.appendChild(document.createElement('div'));
            nameDiv.id = "name";
            nameDiv.innerHTML = user.name;
            let emailDiv = wrapper.appendChild(document.createElement('div'));
            emailDiv.id = "email";
            emailDiv.innerHTML = "&lt;" + user.email + "&gt;";
            if (printId) {
                let isGuest = user["accessLevel"] == "readOnly"
                    ? ", guest"
                    : "";
                let idDiv = wrapper.appendChild(document.createElement('div'));
                idDiv.id = "id";
                idDiv.innerHTML = "(" + user["@id"] + isGuest + ")";
            }
            let impersonateUserButton = wrapper.appendChild(document.createElement('button'));
            impersonateUserButton.className = "impersonateUser";
            impersonateUserButton.title = "Impersonate this user!";
            impersonateUserButton.innerHTML = '<i class="fas fa-user-astronaut"></i>';
            impersonateUserButton.onclick = function (ev) {
                ImpersonateUser(user["@id"]);
            };
            //if (deleteable) {
            //    let removeUserButton = wrapper.appendChild(document.createElement('button')) as HTMLButtonElement;
            //    removeUserButton.className = "removeUser";
            //    removeUserButton.title     = "Remove this user!"
            //    removeUserButton.innerHTML = '<i class="fas fa-trash-alt"></i>';
            //}
        }
        // ------------------------------------------------------
        function CreateNewUser(organization, membersDiv, CreateNewUserButton) {
            let userJSON = {
                "@id": "",
                "@context": "https://opendata.social/contexts/usersAPI/user+json",
                "name": "",
                "email": ""
            };
            var validUserId = false;
            CreateNewUserButton.style.display = 'none';
            const newUserDiv = membersDiv.appendChild(document.createElement('div'));
            newUserDiv.className = "newUser";
            const newUserData = newUserDiv.appendChild(document.createElement('div'));
            newUserData.className = "data";
            //- Id -------------------------------------------------------------
            const newUserIdRow = newUserData.appendChild(document.createElement('div'));
            newUserIdRow.className = "row";
            const newUserIdKey = newUserIdRow.appendChild(document.createElement('div'));
            newUserIdKey.className = "key";
            newUserIdKey.innerHTML = "Login";
            const newUserIdValue = newUserIdRow.appendChild(document.createElement('div'));
            newUserIdValue.className = "value";
            const newUserIdInput = newUserIdValue.appendChild(document.createElement('input'));
            newUserIdInput.className = "value";
            newUserIdInput.placeholder = "The login (unqiue identification) of the new user...";
            const newUserIdError = newUserIdValue.appendChild(document.createElement('div'));
            newUserIdError.className = "validationError";
            newUserIdInput.onchange = () => {
                newUserIdInput.value = newUserIdInput.value.toLowerCase();
                VerifyNewUserId();
            };
            newUserIdInput.onkeyup = () => {
                newUserIdInput.value = newUserIdInput.value.toLowerCase();
                VerifyNewUserId();
            };
            //- Name -------------------------------------------------------------
            let newUserNameRow = newUserData.appendChild(document.createElement('div'));
            newUserNameRow.className = "row";
            let newUserNameKey = newUserNameRow.appendChild(document.createElement('div'));
            newUserNameKey.className = "key";
            newUserNameKey.innerHTML = "Name";
            let newUserNameValue = newUserNameRow.appendChild(document.createElement('div'));
            newUserNameValue.className = "value";
            let newUserNameInput = newUserNameValue.appendChild(document.createElement('input'));
            newUserNameInput.className = "value";
            newUserNameInput.placeholder = "The real name of the new user...";
            //- EMail -------------------------------------------------------------
            let newUserEMailRow = newUserData.appendChild(document.createElement('div'));
            newUserEMailRow.className = "row";
            let newUserEMailKey = newUserEMailRow.appendChild(document.createElement('div'));
            newUserEMailKey.className = "key";
            newUserEMailKey.innerHTML = "E-Mail";
            let newUserEMailValue = newUserEMailRow.appendChild(document.createElement('div'));
            newUserEMailValue.className = "value";
            let newUserEMailInput = newUserEMailValue.appendChild(document.createElement('input'));
            newUserEMailInput.className = "value";
            newUserEMailInput.placeholder = "The e-mail address of the new user...";
            //- MobilePhone -------------------------------------------------------------
            let newUserMobilePhoneRow = newUserData.appendChild(document.createElement('div'));
            newUserMobilePhoneRow.className = "row";
            let newUserMobilePhoneKey = newUserMobilePhoneRow.appendChild(document.createElement('div'));
            newUserMobilePhoneKey.className = "key";
            newUserMobilePhoneKey.innerHTML = "Mobile phone";
            let newUserMobilePhoneValue = newUserMobilePhoneRow.appendChild(document.createElement('div'));
            newUserMobilePhoneValue.className = "value";
            let newUserMobilePhoneInput = newUserMobilePhoneValue.appendChild(document.createElement('input'));
            newUserMobilePhoneInput.className = "value";
            newUserMobilePhoneInput.placeholder = "The optional mobile phone number (SMS) of the new user...";
            //- Description -------------------------------------------------------------
            let newUserDescriptionRow = newUserData.appendChild(document.createElement('div'));
            newUserDescriptionRow.className = "row";
            let newUserDescriptionKey = newUserDescriptionRow.appendChild(document.createElement('div'));
            newUserDescriptionKey.className = "key";
            newUserDescriptionKey.innerHTML = "Description";
            let newUserDescriptionValue = newUserDescriptionRow.appendChild(document.createElement('div'));
            newUserDescriptionValue.className = "value";
            let newUserDescriptionInput = newUserDescriptionValue.appendChild(document.createElement('input'));
            newUserDescriptionInput.className = "value";
            newUserDescriptionInput.placeholder = "A description of the new user...";
            //- AccessLevel --------------------------------------------------------
            let newUserAccessLevelRow = newUserData.appendChild(document.createElement('div'));
            newUserAccessLevelRow.className = "row";
            let newUserAccessLevelKey = newUserAccessLevelRow.appendChild(document.createElement('div'));
            newUserAccessLevelKey.className = "key";
            newUserAccessLevelKey.innerHTML = "AccessLevel";
            let newUserAccessLevelValue = newUserAccessLevelRow.appendChild(document.createElement('div'));
            newUserAccessLevelValue.className = "value";
            let newUserAccessLevelSelect = newUserAccessLevelValue.appendChild(document.createElement('select'));
            newUserAccessLevelSelect.className = "value";
            let newUserAccessLevelGuest = newUserAccessLevelSelect.appendChild(document.createElement('option'));
            newUserAccessLevelGuest.value = "guest";
            newUserAccessLevelGuest.innerText = "Guest";
            let newUserAccessLevelMember = newUserAccessLevelSelect.appendChild(document.createElement('option'));
            newUserAccessLevelMember.value = "member";
            newUserAccessLevelMember.innerText = "Member";
            newUserAccessLevelMember.selected = true;
            let newUserAccessLevelAdmin = newUserAccessLevelSelect.appendChild(document.createElement('option'));
            newUserAccessLevelAdmin.value = "admin";
            newUserAccessLevelAdmin.innerText = "Admin";
            //- Response ---------------------------------------------------------
            let responseDiv = newUserDiv.appendChild(document.createElement('div'));
            responseDiv.className = "response";
            //- Store ------------------------------------------------------------
            let storeNewUserButton = newUserDiv.appendChild(document.createElement('button'));
            storeNewUserButton.className = "storeNewUserButton";
            storeNewUserButton.innerHTML = '<i class="fas fa-save"></i> Store new user';
            storeNewUserButton.disabled = true;
            storeNewUserButton.onclick = function (ev) {
                var newUserId = newUserIdInput.value.trim();
                var newUserName = newUserNameInput.value.trim();
                var newUserEMail = newUserEMailInput.value.trim();
                var newUserMobilePhone = newUserMobilePhoneInput.value.trim();
                var newUserDescription = newUserDescriptionInput.value.trim();
                var newUserAccessLevel = newUserAccessLevelSelect.selectedOptions[0].value.trim();
                var newUserJSON = {
                    "@id": newUserId,
                    "@context": "https://opendata.social/contexts/usersAPI/user+json",
                    "name": newUserName,
                    "email": newUserEMail,
                    "organization": organization["@id"],
                    "accessLevel": newUserAccessLevel,
                    "verified": false
                };
                if (newUserMobilePhone != "")
                    newUserJSON["mobilePhone"] = newUserMobilePhone;
                if (newUserDescription != "")
                    newUserJSON["description"] = { "eng": newUserDescription };
                HTTPAdd("/users/" + newUserId, newUserJSON, (statusCode, status, response) => {
                    PrintUser(newUserJSON, newUserAccessLevel == "admin"
                        ? this.parentElement.parentElement.parentElement.parentElement.children[0].children[1].children[0]
                        : this.parentElement.parentElement.children[0], newUserAccessLevel, // css classname!
                    newUserAccessLevel != "admin", newUserAccessLevel != "admin");
                    CreateNewUserButton.style.display = 'block';
                    newUserDiv.remove();
                }, (statusCode, status, response) => {
                    var responseJSON = { "description": "HTTP Error " + statusCode + " - " + status + "!" };
                    if (response != null && response != "") {
                        try {
                            responseJSON = JSON.parse(response);
                        }
                        catch (_a) { }
                    }
                    responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing new user data failed!" + (responseJSON.description != null ? responseJSON.description : "") + "</div>";
                });
            };
            //- Cancel -----------------------------------------------------------
            let cancelNewUserButton = newUserDiv.appendChild(document.createElement('button'));
            cancelNewUserButton.className = "cancelNewUserButton";
            cancelNewUserButton.innerText = "Cancel";
            cancelNewUserButton.onclick = function (ev) {
                CreateNewUserButton.style.display = 'block';
                newUserDiv.remove();
            };
            function VerifyNewUserId() {
                let UserId = newUserIdInput.value;
                if (UserId == "")
                    storeNewUserButton.disabled = true;
                else if (/^([a-zA-Z0-9]{3,})$/.test(UserId) == false) {
                    storeNewUserButton.disabled = true;
                    newUserIdInput.classList.add("error");
                    newUserIdError.innerText = "Invalid user identification!";
                    newUserIdError.style.display = "flex";
                }
                else if (UserId != userJSON["@id"]) {
                    validUserId = false;
                    responseDiv.innerHTML = "";
                    HTTPGet("/users/" + UserId, 
                    // HTTP OK    => bad!
                    (HTTPStatus, ResponseText) => {
                        storeNewUserButton.disabled = true;
                        newUserIdInput.classList.add("error");
                        newUserIdError.innerText = "This user identification already exists!";
                        newUserIdError.style.display = "flex";
                    }, 
                    // HTTP Error => Maybe good!
                    (HTTPStatus, StatusText, ResponseText) => {
                        // HTTP Not Found => good!
                        if (HTTPStatus == 404) {
                            validUserId = true;
                            userJSON["@id"] = UserId;
                            newUserIdInput.classList.remove("error");
                            newUserIdError.style.display = "none";
                            storeNewUserButton.disabled = false;
                        }
                        // Any other status => bad!
                        else {
                            validUserId = false;
                            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not validate the given user identification!</div>";
                            storeNewUserButton.disabled = true;
                        }
                    });
                }
            }
        }
        // ------------------------------------------------------
        function AddChildOrganization(organization, orgDiv, AddChildButton) {
            var organizationJSON = {
                "@id": "",
                "@context": "https://opendata.social/contexts/UsersAPI+json/organization",
                "parent": organization["@id"],
                "name": { "eng": "" }
            };
            var validOrganizationId = false;
            AddChildButton.style.display = 'none';
            let newChildOrganizationDiv = orgDiv.appendChild(document.createElement('div'));
            newChildOrganizationDiv.className = "newChildOrganization";
            let newChildOrganizationData = newChildOrganizationDiv.appendChild(document.createElement('div'));
            newChildOrganizationData.className = "data";
            //- Id -------------------------------------------------------------
            let newChildOrganizationIdRow = newChildOrganizationData.appendChild(document.createElement('div'));
            newChildOrganizationIdRow.className = "row";
            let newChildOrganizationIdKey = newChildOrganizationIdRow.appendChild(document.createElement('div'));
            newChildOrganizationIdKey.className = "key";
            newChildOrganizationIdKey.innerHTML = "Id";
            let newChildOrganizationIdValue = newChildOrganizationIdRow.appendChild(document.createElement('div'));
            newChildOrganizationIdValue.className = "value";
            let newChildOrganizationIdInput = newChildOrganizationIdValue.appendChild(document.createElement('input'));
            newChildOrganizationIdInput.className = "value";
            newChildOrganizationIdInput.placeholder = "The unique identification of the new child organization...";
            let newChildOrganizationIdError = newChildOrganizationIdValue.appendChild(document.createElement('div'));
            newChildOrganizationIdError.className = "validationError";
            newChildOrganizationIdInput.onchange = function (ev) {
                newChildOrganizationIdInput.value = newChildOrganizationIdInput.value.toLowerCase();
                VerifyNewOrganizationId();
            };
            newChildOrganizationIdInput.onkeyup = function (ev) {
                newChildOrganizationIdInput.value = newChildOrganizationIdInput.value.toLowerCase();
                VerifyNewOrganizationId();
            };
            //- Name -------------------------------------------------------------
            let newChildOrganizationNameRow = newChildOrganizationData.appendChild(document.createElement('div'));
            newChildOrganizationNameRow.className = "row";
            let newChildOrganizationNameKey = newChildOrganizationNameRow.appendChild(document.createElement('div'));
            newChildOrganizationNameKey.className = "key";
            newChildOrganizationNameKey.innerHTML = "Name";
            let newChildOrganizationNameValue = newChildOrganizationNameRow.appendChild(document.createElement('div'));
            newChildOrganizationNameValue.className = "value";
            let newChildOrganizationNameInput = newChildOrganizationNameValue.appendChild(document.createElement('input'));
            newChildOrganizationNameInput.className = "value";
            newChildOrganizationNameInput.placeholder = "The name of the new child organization...";
            //- Description -------------------------------------------------------------
            let newChildOrganizationDescriptionRow = newChildOrganizationData.appendChild(document.createElement('div'));
            newChildOrganizationDescriptionRow.className = "row";
            let newChildOrganizationDescriptionKey = newChildOrganizationDescriptionRow.appendChild(document.createElement('div'));
            newChildOrganizationDescriptionKey.className = "key";
            newChildOrganizationDescriptionKey.innerHTML = "Description";
            let newChildOrganizationDescriptionValue = newChildOrganizationDescriptionRow.appendChild(document.createElement('div'));
            newChildOrganizationDescriptionValue.className = "value";
            let newChildOrganizationDescriptionInput = newChildOrganizationDescriptionValue.appendChild(document.createElement('input'));
            newChildOrganizationDescriptionInput.className = "value";
            newChildOrganizationDescriptionInput.placeholder = "An optional description of the new child organization...";
            //- Response ---------------------------------------------------------
            let responseDiv = newChildOrganizationDiv.appendChild(document.createElement('div'));
            responseDiv.className = "response";
            //- Store ------------------------------------------------------------
            let storeNewChildButton = newChildOrganizationDiv.appendChild(document.createElement('button'));
            storeNewChildButton.className = "storeChildButton";
            storeNewChildButton.innerHTML = '<i class="fas fa-save"></i> Store new child organization';
            storeNewChildButton.disabled = true;
            storeNewChildButton.onclick = function (ev) {
                var newChildOrganizationId = newChildOrganizationIdInput.value.trim();
                var newChildOrganizationName = newChildOrganizationNameInput.value.trim();
                var newChildOrganizationDescription = newChildOrganizationDescriptionInput.value.trim();
                var newChildOrganizationJSON = {
                    "@id": newChildOrganizationId,
                    "@context": "https://opendata.social/contexts/UsersAPI+json/organization",
                    "parentOrganization": organization["@id"],
                    "name": { "eng": newChildOrganizationName != "" ? newChildOrganizationName : newChildOrganizationId },
                    "admins": [{
                            "@id": SignInUser,
                            "@context": "https://opendata.social/contexts/usersAPI/user+json",
                            "name": Username,
                            "email": UserEMail
                        }],
                    "members": [],
                    "youAreMember": true,
                    "youCanAddMembers": true,
                    "youCanCreateChildOrganizations": true,
                    "childs": []
                };
                if (newChildOrganizationDescription != "") {
                    newChildOrganizationJSON["description"] = { "eng": newChildOrganizationDescription };
                }
                HTTPAdd("/organizations/" + newChildOrganizationId, newChildOrganizationJSON, (statusCode, status, response) => {
                    PrintOrganization(newChildOrganizationJSON, this.parentElement.parentElement.parentElement.children[2]);
                    AddChildButton.style.display = 'block';
                    newChildOrganizationDiv.remove();
                }, (statusCode, status, response) => {
                    var responseJSON = response != "" ? JSON.parse(response) : {};
                    var info = responseJSON.description != null ? "<br />" + responseJSON.description : "";
                    responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing new child organization data failed!" + info + "</div>";
                });
            };
            //- Cancel -----------------------------------------------------------
            let cancelNewChildButton = newChildOrganizationDiv.appendChild(document.createElement('button'));
            cancelNewChildButton.className = "cancelChildButton";
            cancelNewChildButton.innerText = "Cancel";
            cancelNewChildButton.onclick = function (ev) {
                AddChildButton.style.display = 'block';
                newChildOrganizationDiv.remove();
            };
            function VerifyNewOrganizationId() {
                let OrganizationId = newChildOrganizationIdInput.value;
                if (OrganizationId == "")
                    storeNewChildButton.disabled = true;
                else if (/^([a-zA-Z0-9]{5,})$/.test(OrganizationId) == false) {
                    storeNewChildButton.disabled = true;
                    newChildOrganizationIdInput.classList.add("error");
                    newChildOrganizationIdError.innerText = "Invalid organization identification!";
                    newChildOrganizationIdError.style.display = "flex";
                }
                else if (OrganizationId != organizationJSON["@id"]) {
                    validOrganizationId = false;
                    responseDiv.innerHTML = "";
                    HTTPGet("/organizations/" + OrganizationId, 
                    // HTTP OK    => bad!
                    (HTTPStatus, ResponseText) => {
                        storeNewChildButton.disabled = true;
                        newChildOrganizationIdInput.classList.add("error");
                        newChildOrganizationIdError.innerText = "This organization identification already exists!";
                        newChildOrganizationIdError.style.display = "flex";
                    }, 
                    // HTTP Error => Maybe good!
                    (HTTPStatus, StatusText, ResponseText) => {
                        // HTTP Not Found => good!
                        if (HTTPStatus == 404) {
                            validOrganizationId = true;
                            organizationJSON["@id"] = OrganizationId;
                            newChildOrganizationIdInput.classList.remove("error");
                            newChildOrganizationIdError.style.display = "none";
                            storeNewChildButton.disabled = false;
                        }
                        // Any other status => bad!
                        else {
                            validOrganizationId = false;
                            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not validate the given organization identification!</div>";
                            storeNewChildButton.disabled = true;
                        }
                    });
                }
            }
        }
        // ------------------------------------------------------
        let organizationDiv = orgDiv.appendChild(document.createElement('div'));
        organizationDiv.className = "organization";
        organization.youAreMember
            ? organizationDiv.className += " Member"
            : organizationDiv.className += " notMember";
        let nameDiv = organizationDiv.appendChild(document.createElement('div'));
        nameDiv.className = "name";
        nameDiv.innerHTML = firstValue(organization.name) + " <a href=\"/organizations/" + organization["@id"] + "\" style=\"text-decoration: none\"><i class=\"fas fa-arrow-right\"></i></a>";
        nameDiv.onclick = function (ev) {
            let propertiesDiv = this.parentElement.querySelector('.properties');
            propertiesDiv.style.display == '' || propertiesDiv.style.display == 'none'
                ? propertiesDiv.style.display = 'table'
                : propertiesDiv.style.display = 'none';
            if (organization.youCanCreateChildOrganizations) {
                let addChildDiv = this.parentElement.children[this.parentElement.childElementCount - 1];
                addChildDiv.style.display == '' || addChildDiv.style.display == 'none'
                    ? addChildDiv.style.display = 'block'
                    : addChildDiv.style.display = 'none';
            }
        };
        let propertiesDiv = organizationDiv.appendChild(document.createElement('div'));
        propertiesDiv.className = "properties";
        // Show admins...
        if (organization.admins != null && organization.admins.length > 0) {
            let adminsDiv = propertiesDiv.appendChild(document.createElement('div'));
            adminsDiv.className = "admins";
            let infoDiv = adminsDiv.appendChild(document.createElement('div'));
            infoDiv.className = "info";
            infoDiv.innerText = "Admins";
            let wrapperDiv = adminsDiv.appendChild(document.createElement('div'));
            wrapperDiv.className = "wrapper";
            let usersDiv = wrapperDiv.appendChild(document.createElement('div'));
            usersDiv.className = "users";
            for (var admin of organization.admins)
                PrintUser(admin, usersDiv, "admin", false, false);
        }
        // Show members... only when you are also a member!
        if (organization.youAreMember) {
            myOrgs.push(orgDiv);
            let membersDiv = propertiesDiv.appendChild(document.createElement('div'));
            membersDiv.id = "members";
            let infoDiv = membersDiv.appendChild(document.createElement('div'));
            infoDiv.id = "info";
            infoDiv.innerText = "Members";
            let wrapperDiv = membersDiv.appendChild(document.createElement('div'));
            wrapperDiv.id = "wrapper";
            let usersDiv = wrapperDiv.appendChild(document.createElement('div'));
            usersDiv.className = "users";
            if (organization.members != null && organization.members.length > 0) {
                for (var member of organization.members)
                    PrintUser(member, usersDiv, "member", true, true);
            }
            if (organization.youCanAddMembers) {
                let AddUserDiv = wrapperDiv.appendChild(document.createElement('div'));
                AddUserDiv.className = "addUser";
                //let AddExistingUserButton = AddUserDiv.appendChild(document.createElement('button')) as HTMLButtonElement;
                //AddExistingUserButton.className = "addExistingUserButton";
                //AddExistingUserButton.innerHTML = '<i class="fas fa-link"></i><i class="fas fa-user"></i> Add existing user';
                let CreateNewUserButton = AddUserDiv.appendChild(document.createElement('button'));
                CreateNewUserButton.className = "createNewUserButton";
                CreateNewUserButton.innerHTML = '<i class="fas fa-plus"></i><i class="fas fa-user"></i> Create new user';
                CreateNewUserButton.onclick = function (ev) {
                    CreateNewUser(organization, CreateNewUserButton.parentElement.parentElement, CreateNewUserButton);
                };
            }
        }
        // Show child orgranizations...
        let childsDiv = organizationDiv.appendChild(document.createElement('div'));
        childsDiv.className = "childs";
        for (var child of organization._childs)
            PrintOrganization(child, childsDiv);
        // Create a new child orgranization...
        if (organization.youCanCreateChildOrganizations) {
            let CreateChildOrganizationDiv = organizationDiv.appendChild(document.createElement('div'));
            CreateChildOrganizationDiv.className = "createChildOrganization";
            let CreateChildOrganizationButton = CreateChildOrganizationDiv.appendChild(document.createElement('button'));
            CreateChildOrganizationButton.className = "createChildOrganizationButton";
            CreateChildOrganizationButton.innerHTML = '<i class="fas fa-plus"></i><i class="fas fa-building"></i> Create child organization';
            CreateChildOrganizationButton.onclick = function (ev) {
                AddChildOrganization(organization, CreateChildOrganizationDiv, CreateChildOrganizationButton);
            };
        }
    }
    checkSignedIn(true);
    const organizationsDiv = document.getElementById('organizations');
    HTTPGet("/users/" + SignInUser + "/organizations", (status, response) => {
        //let user          = SignInUser;
        const organizations = JSON.parse(response);
        organizationsDiv.innerHTML = "";
        for (const organization of organizations)
            PrintOrganization(organization, organizationsDiv);
        // If there is only a single organization: Open it!
        if (myOrgs.length === 1) {
            myOrgs[0].querySelector('.properties').style.display = 'table';
            myOrgs[0].children[0].children[myOrgs[0].children[0].childElementCount - 1].style.display = 'block';
        }
    }, (statusCode, status, response) => {
        organizationsDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch your organizations data!</div>";
    });
}
//# sourceMappingURL=organizations.js.map