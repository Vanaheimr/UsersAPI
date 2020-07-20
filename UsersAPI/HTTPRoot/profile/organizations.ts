///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />

function ShowOrganizations() {

    var myOrgs = [];

    function ImpersonateUser(newUserId) {

        HTTPImpersonate("/users/" + newUserId,

                        (HTTPStatus, ResponseText) => {
                            window.location.reload(true);
                        },

                        (HTTPStatus, StatusText, ResponseText) => {
                            alert("Not allowed!");
                        });

    }

    function PrintOrganization(organization, orgDiv) {

        function PrintUser(user, userDiv, className, printId, deleteable) {

            let wrapper = userDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            wrapper.className = className;

            let nameDiv = wrapper.appendChild(document.createElement('div')) as HTMLDivElement;
            nameDiv.id = "name";
            nameDiv.innerHTML = user.name;

            let emailDiv = wrapper.appendChild(document.createElement('div')) as HTMLDivElement;
            emailDiv.id = "email";
            emailDiv.innerHTML = "&lt;" + user.email + "&gt;";

            if (printId) {

                let isGuest = user["accessLevel"] == "readOnly"
                                  ? ", guest"
                                  : "";

                let idDiv = wrapper.appendChild(document.createElement('div')) as HTMLDivElement;
                idDiv.id        = "id";
                idDiv.innerHTML = "(" + user["@id"] + isGuest + ")";

            }

            let impersonateUserButton = wrapper.appendChild(document.createElement('button')) as HTMLButtonElement;
            impersonateUserButton.className = "impersonateUser";
            impersonateUserButton.title     = "Impersonate this user!"
            impersonateUserButton.innerHTML = '<i class="fas fa-user-astronaut"></i>';
            impersonateUserButton.onclick = function (this: HTMLElement, ev: MouseEvent) {
                ImpersonateUser(user["@id"]);
            }

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
                "@id":          "",
                "@context":     "https://opendata.social/contexts/UsersAPI+json/user",
                "name":         "",
                "email":        ""
            };

            var validUserId = false;

            CreateNewUserButton.style.display = 'none';

            const newUserDiv = membersDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserDiv.className = "newUser";

            const newUserData = newUserDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserData.className = "data";


            //- Id -------------------------------------------------------------
            const newUserIdRow = newUserData.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserIdRow.className = "row";

            const newUserIdKey = newUserIdRow.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserIdKey.className = "key";
            newUserIdKey.innerHTML = "Login";

            const newUserIdValue = newUserIdRow.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserIdValue.className = "value";

            const newUserIdInput = newUserIdValue.appendChild(document.createElement('input')) as HTMLInputElement;
            newUserIdInput.className = "value";
            newUserIdInput.placeholder = "The login (unqiue identification) of the new user...";

            const newUserIdError = newUserIdValue.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserIdError.className = "validationError";

            newUserIdInput.onchange = () => {
                newUserIdInput.value = newUserIdInput.value.toLowerCase();
                VerifyNewUserId();
            }

            newUserIdInput.onkeyup = () => {
                newUserIdInput.value = newUserIdInput.value.toLowerCase();
                VerifyNewUserId();
            }


            //- Name -------------------------------------------------------------
            let newUserNameRow = newUserData.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserNameRow.className = "row";

            let newUserNameKey = newUserNameRow.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserNameKey.className = "key";
            newUserNameKey.innerHTML = "Name";

            let newUserNameValue = newUserNameRow.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserNameValue.className = "value";

            let newUserNameInput = newUserNameValue.appendChild(document.createElement('input')) as HTMLInputElement;
            newUserNameInput.className = "value";
            newUserNameInput.placeholder = "The real name of the new user...";


            //- EMail -------------------------------------------------------------
            let newUserEMailRow = newUserData.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserEMailRow.className     = "row";

            let newUserEMailKey = newUserEMailRow.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserEMailKey.className     = "key";
            newUserEMailKey.innerHTML     = "E-Mail";

            let newUserEMailValue = newUserEMailRow.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserEMailValue.className   = "value";

            let newUserEMailInput = newUserEMailValue.appendChild(document.createElement('input')) as HTMLInputElement;
            newUserEMailInput.className   = "value";
            newUserEMailInput.placeholder = "The e-mail address of the new user...";


            //- MobilePhone -------------------------------------------------------------
            let newUserMobilePhoneRow = newUserData.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserMobilePhoneRow.className     = "row";

            let newUserMobilePhoneKey = newUserMobilePhoneRow.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserMobilePhoneKey.className     = "key";
            newUserMobilePhoneKey.innerHTML     = "Mobile phone";

            let newUserMobilePhoneValue = newUserMobilePhoneRow.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserMobilePhoneValue.className   = "value";

            let newUserMobilePhoneInput = newUserMobilePhoneValue.appendChild(document.createElement('input')) as HTMLInputElement;
            newUserMobilePhoneInput.className   = "value";
            newUserMobilePhoneInput.placeholder = "The optional mobile phone number (SMS) of the new user...";


            //- Description -------------------------------------------------------------
            let newUserDescriptionRow = newUserData.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserDescriptionRow.className     = "row";

            let newUserDescriptionKey = newUserDescriptionRow.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserDescriptionKey.className     = "key";
            newUserDescriptionKey.innerHTML     = "Description";

            let newUserDescriptionValue = newUserDescriptionRow.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserDescriptionValue.className   = "value";

            let newUserDescriptionInput = newUserDescriptionValue.appendChild(document.createElement('input')) as HTMLInputElement;
            newUserDescriptionInput.className   = "value";
            newUserDescriptionInput.placeholder = "A description of the new user...";


            //- AccessLevel --------------------------------------------------------
            let newUserAccessLevelRow = newUserData.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserAccessLevelRow.className    = "row";

            let newUserAccessLevelKey = newUserAccessLevelRow.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserAccessLevelKey.className    = "key";
            newUserAccessLevelKey.innerHTML    = "AccessLevel";

            let newUserAccessLevelValue = newUserAccessLevelRow.appendChild(document.createElement('div')) as HTMLDivElement;
            newUserAccessLevelValue.className  = "value";

            let newUserAccessLevelSelect = newUserAccessLevelValue.appendChild(document.createElement('select')) as HTMLSelectElement;
            newUserAccessLevelSelect.className = "value";

            let newUserAccessLevelGuest = newUserAccessLevelSelect.appendChild(document.createElement('option')) as HTMLOptionElement;
            newUserAccessLevelGuest.value      = "guest";
            newUserAccessLevelGuest.innerText  = "Guest";

            let newUserAccessLevelMember = newUserAccessLevelSelect.appendChild(document.createElement('option')) as HTMLOptionElement;
            newUserAccessLevelMember.value     = "member";
            newUserAccessLevelMember.innerText = "Member";
            newUserAccessLevelMember.selected  = true;

            let newUserAccessLevelAdmin = newUserAccessLevelSelect.appendChild(document.createElement('option')) as HTMLOptionElement;
            newUserAccessLevelAdmin.value      = "admin";
            newUserAccessLevelAdmin.innerText  = "Admin";


            //- Response ---------------------------------------------------------
            let responseDiv = newUserDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            responseDiv.className = "response";


            //- Store ------------------------------------------------------------
            let storeNewUserButton = newUserDiv.appendChild(document.createElement('button')) as HTMLButtonElement;
            storeNewUserButton.className = "storeNewUserButton";
            storeNewUserButton.innerHTML = '<i class="fas fa-save"></i> Store new user';
            storeNewUserButton.disabled = true

            storeNewUserButton.onclick = function (this: HTMLElement, ev: MouseEvent) {

                var newUserId           = newUserIdInput.value.trim();
                var newUserName         = newUserNameInput.value.trim();
                var newUserEMail        = newUserEMailInput.value.trim();
                var newUserMobilePhone  = newUserMobilePhoneInput.value.trim();
                var newUserDescription  = newUserDescriptionInput.value.trim();
                var newUserAccessLevel  = newUserAccessLevelSelect.selectedOptions[0].value.trim();

                var newUserJSON = {
                    "@id":           newUserId,
                    "@context":      "https://opendata.social/contexts/UsersAPI+json/user",
                    "name":          newUserName,
                    "email":         newUserEMail,
                    "organization":  organization["@id"],
                    "accessLevel":   newUserAccessLevel,
                    "verified":      false
                };

                if (newUserMobilePhone != "")
                    newUserJSON["mobilePhone"] = newUserMobilePhone;

                if (newUserDescription != "")
                    newUserJSON["description"] = { "eng": newUserDescription };


                HTTPAdd("/users/" + newUserId,
                        newUserJSON,

                        (statusCode, status, response) => {

                            PrintUser(newUserJSON,
                                      newUserAccessLevel == "admin"
                                          ? this.parentElement.parentElement.parentElement.parentElement.children[0].children[1].children[0]
                                          : this.parentElement.parentElement.children[0],
                                      newUserAccessLevel,  // css classname!
                                      newUserAccessLevel != "admin",
                                      newUserAccessLevel != "admin");

                            CreateNewUserButton.style.display = 'block';
                            newUserDiv.remove();

                        },

                        (statusCode, status, response) => {

                            var responseJSON = { "description": "HTTP Error " + statusCode + " - " + status + "!" };

                            if (response != null && response != "") {
                                try
                                {
                                    responseJSON = JSON.parse(response);
                                }
                                catch { }
                            }

                            responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing new user data failed!" + (responseJSON.description != null ? responseJSON.description : "") + "</div>";

                        });

            };


            //- Cancel -----------------------------------------------------------
            let cancelNewUserButton = newUserDiv.appendChild(document.createElement('button')) as HTMLButtonElement;
            cancelNewUserButton.className = "cancelNewUserButton";
            cancelNewUserButton.innerText = "Cancel";

            cancelNewUserButton.onclick = function (this: HTMLElement, ev: MouseEvent) {
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
                    newUserIdError.innerText     = "Invalid user identification!";
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
                                newUserIdError.innerText     = "This user identification already exists!";
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
                "@id":          "",
                "@context":     "https://opendata.social/contexts/UsersAPI+json/organization",
                "parent":       organization["@id"],
                "name":         { "eng": "" }
            };

            var validOrganizationId = false;

            AddChildButton.style.display = 'none';

            let newChildOrganizationDiv = orgDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            newChildOrganizationDiv.className = "newChildOrganization";


            let newChildOrganizationData = newChildOrganizationDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            newChildOrganizationData.className = "data";

            //- Id -------------------------------------------------------------
            let newChildOrganizationIdRow = newChildOrganizationData.appendChild(document.createElement('div')) as HTMLDivElement;
            newChildOrganizationIdRow.className = "row";

            let newChildOrganizationIdKey = newChildOrganizationIdRow.appendChild(document.createElement('div')) as HTMLDivElement;
            newChildOrganizationIdKey.className = "key";
            newChildOrganizationIdKey.innerHTML = "Id";

            let newChildOrganizationIdValue = newChildOrganizationIdRow.appendChild(document.createElement('div')) as HTMLDivElement;
            newChildOrganizationIdValue.className = "value";

            let newChildOrganizationIdInput = newChildOrganizationIdValue.appendChild(document.createElement('input')) as HTMLInputElement;
            newChildOrganizationIdInput.className = "value";
            newChildOrganizationIdInput.placeholder = "The unique identification of the new child organization...";

            let newChildOrganizationIdError = newChildOrganizationIdValue.appendChild(document.createElement('div')) as HTMLDivElement;
            newChildOrganizationIdError.className = "validationError";

            newChildOrganizationIdInput.onchange = function (this: HTMLElement, ev: Event) {
                newChildOrganizationIdInput.value = newChildOrganizationIdInput.value.toLowerCase();
                VerifyNewOrganizationId();
            }

            newChildOrganizationIdInput.onkeyup = function (this: HTMLElement, ev: Event) {
                newChildOrganizationIdInput.value = newChildOrganizationIdInput.value.toLowerCase();
                VerifyNewOrganizationId();
            }


            //- Name -------------------------------------------------------------
            let newChildOrganizationNameRow = newChildOrganizationData.appendChild(document.createElement('div')) as HTMLDivElement;
            newChildOrganizationNameRow.className = "row";

            let newChildOrganizationNameKey = newChildOrganizationNameRow.appendChild(document.createElement('div')) as HTMLDivElement;
            newChildOrganizationNameKey.className = "key";
            newChildOrganizationNameKey.innerHTML = "Name";

            let newChildOrganizationNameValue = newChildOrganizationNameRow.appendChild(document.createElement('div')) as HTMLDivElement;
            newChildOrganizationNameValue.className = "value";

            let newChildOrganizationNameInput = newChildOrganizationNameValue.appendChild(document.createElement('input')) as HTMLInputElement;
            newChildOrganizationNameInput.className = "value";
            newChildOrganizationNameInput.placeholder = "The name of the new child organization...";


            //- Description -------------------------------------------------------------
            let newChildOrganizationDescriptionRow = newChildOrganizationData.appendChild(document.createElement('div')) as HTMLDivElement;
            newChildOrganizationDescriptionRow.className = "row";

            let newChildOrganizationDescriptionKey = newChildOrganizationDescriptionRow.appendChild(document.createElement('div')) as HTMLDivElement;
            newChildOrganizationDescriptionKey.className = "key";
            newChildOrganizationDescriptionKey.innerHTML = "Description";

            let newChildOrganizationDescriptionValue = newChildOrganizationDescriptionRow.appendChild(document.createElement('div')) as HTMLDivElement;
            newChildOrganizationDescriptionValue.className = "value";

            let newChildOrganizationDescriptionInput = newChildOrganizationDescriptionValue.appendChild(document.createElement('input')) as HTMLInputElement;
            newChildOrganizationDescriptionInput.className = "value";
            newChildOrganizationDescriptionInput.placeholder = "An optional description of the new child organization...";


            //- Response ---------------------------------------------------------
            let responseDiv = newChildOrganizationDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            responseDiv.className = "response";


            //- Store ------------------------------------------------------------
            let storeNewChildButton = newChildOrganizationDiv.appendChild(document.createElement('button')) as HTMLButtonElement;
            storeNewChildButton.className = "storeChildButton";
            storeNewChildButton.innerHTML = '<i class="fas fa-save"></i> Store new child organization';
            storeNewChildButton.disabled = true

            storeNewChildButton.onclick = function (this: HTMLElement, ev: MouseEvent) {

                var newChildOrganizationId           = newChildOrganizationIdInput.value.trim();
                var newChildOrganizationName         = newChildOrganizationNameInput.value.trim();
                var newChildOrganizationDescription  = newChildOrganizationDescriptionInput.value.trim();

                var newChildOrganizationJSON = {
                    "@id":                             newChildOrganizationId,
                    "@context":                        "https://opendata.social/contexts/UsersAPI+json/organization",
                    "parentOrganization":              organization["@id"],
                    "name":                            { "eng": newChildOrganizationName != "" ? newChildOrganizationName : newChildOrganizationId },
                    "admins": [{
                        "@id":       SignInUser,
                        "@context":  "https://opendata.social/contexts/UsersAPI+json/user",
                        "name":      Username,
                        "email":     UserEMail
                    }],
                    "members":                         [],
                    "youAreMember":                    true,
                    "youCanAddMembers":                true,
                    "youCanCreateChildOrganizations":  true,
                    "childs":                          []
                };

                if (newChildOrganizationDescription != "") {
                    newChildOrganizationJSON["description"] = { "eng": newChildOrganizationDescription };
                }


                HTTPAdd("/organizations/" + newChildOrganizationId,
                        newChildOrganizationJSON,

                        (statusCode, status, response) => {

                            PrintOrganization(newChildOrganizationJSON,
                                              this.parentElement.parentElement.parentElement.children[2]);

                            AddChildButton.style.display = 'block';
                            newChildOrganizationDiv.remove();

                        },

                        (statusCode, status, response) => {
                            var responseJSON = response != "" ? JSON.parse(response) : {};
                            var info         = responseJSON.description != null ? "<br />" + responseJSON.description : "";
                            responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing new child organization data failed!" + info + "</div>";
                        });

            };



            //- Cancel -----------------------------------------------------------
            let cancelNewChildButton = newChildOrganizationDiv.appendChild(document.createElement('button')) as HTMLButtonElement;
            cancelNewChildButton.className = "cancelChildButton";
            cancelNewChildButton.innerText = "Cancel";

            cancelNewChildButton.onclick = function (this: HTMLElement, ev: MouseEvent) {
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
                    newChildOrganizationIdError.innerText     = "Invalid organization identification!";
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
                                newChildOrganizationIdError.innerText     = "This organization identification already exists!";
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

        let organizationDiv        = orgDiv.appendChild(document.createElement('div')) as HTMLDivElement;
        organizationDiv.className  = "organization";

        organization.youAreMember
            ? organizationDiv.className += " Member"
            : organizationDiv.className += " notMember";

        let nameDiv = organizationDiv.appendChild(document.createElement('div')) as HTMLDivElement;
        nameDiv.className = "name";
        nameDiv.innerHTML = firstValue(organization.name) + " <a href=\"/organizations/" + organization["@id"] + "\" style=\"text-decoration: none\"><i class=\"fas fa-arrow-right\"></i></a>";

        nameDiv.onclick = function (this: HTMLElement, ev: MouseEvent) {

            let propertiesDiv = this.parentElement.querySelector('.properties') as HTMLDivElement;

            propertiesDiv.style.display == '' || propertiesDiv.style.display == 'none'
                ? propertiesDiv.style.display = 'table'
                : propertiesDiv.style.display = 'none';

            if (organization.youCanCreateChildOrganizations) {

                let addChildDiv = this.parentElement.children[this.parentElement.childElementCount - 1] as HTMLDivElement;

                addChildDiv.style.display == '' || addChildDiv.style.display == 'none'
                    ? addChildDiv.style.display = 'block'
                    : addChildDiv.style.display = 'none';

            }

        };

        let propertiesDiv = organizationDiv.appendChild(document.createElement('div')) as HTMLDivElement;
        propertiesDiv.className = "properties";

        // Show admins...
        if (organization.admins != null && organization.admins.length > 0) {

            let adminsDiv = propertiesDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            adminsDiv.className = "admins";

            let infoDiv = adminsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            infoDiv.className = "info";
            infoDiv.innerText = "Admins";

            let wrapperDiv = adminsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            wrapperDiv.className = "wrapper";

            let usersDiv = wrapperDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            usersDiv.className = "users";

            for (var admin of organization.admins)
                PrintUser(admin, usersDiv, "admin", false, false);

        }

        // Show members... only when you are also a member!
        if (organization.youAreMember) {

            myOrgs.push(orgDiv);

            let membersDiv = propertiesDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            membersDiv.id = "members";

            let infoDiv = membersDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            infoDiv.id = "info";
            infoDiv.innerText = "Members";

            let wrapperDiv = membersDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            wrapperDiv.id = "wrapper";

            let usersDiv = wrapperDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            usersDiv.className = "users";

            if (organization.members != null && organization.members.length > 0) {
                for (var member of organization.members)
                    PrintUser(member, usersDiv, "member", true, true);
            }

            if (organization.youCanAddMembers) {

                let AddUserDiv = wrapperDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                AddUserDiv.className = "addUser";

                //let AddExistingUserButton = AddUserDiv.appendChild(document.createElement('button')) as HTMLButtonElement;
                //AddExistingUserButton.className = "addExistingUserButton";
                //AddExistingUserButton.innerHTML = '<i class="fas fa-link"></i><i class="fas fa-user"></i> Add existing user';

                let CreateNewUserButton = AddUserDiv.appendChild(document.createElement('button')) as HTMLButtonElement;
                CreateNewUserButton.className = "createNewUserButton";
                CreateNewUserButton.innerHTML = '<i class="fas fa-plus"></i><i class="fas fa-user"></i> Create new user';

                CreateNewUserButton.onclick = function (this: HTMLElement, ev: MouseEvent) {
                    CreateNewUser(organization, CreateNewUserButton.parentElement.parentElement, CreateNewUserButton)
                }

            }

        }


        // Show child orgranizations...
        let childsDiv = organizationDiv.appendChild(document.createElement('div')) as HTMLDivElement;
        childsDiv.className = "childs";

        for (var child of organization._childs)
            PrintOrganization(child, childsDiv);

        // Create a new child orgranization...
        if (organization.youCanCreateChildOrganizations) {

            let CreateChildOrganizationDiv = organizationDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            CreateChildOrganizationDiv.className = "createChildOrganization";

            let CreateChildOrganizationButton = CreateChildOrganizationDiv.appendChild(document.createElement('button')) as HTMLButtonElement;
            CreateChildOrganizationButton.className = "createChildOrganizationButton";
            CreateChildOrganizationButton.innerHTML = '<i class="fas fa-plus"></i><i class="fas fa-building"></i> Create child organization';

            CreateChildOrganizationButton.onclick = function (this: HTMLElement, ev: MouseEvent) {
                AddChildOrganization(organization, CreateChildOrganizationDiv, CreateChildOrganizationButton);
            };

        }

    }


    checkSignedIn(true);
    const organizationsDiv = document.getElementById('organizations') as HTMLDivElement;


    HTTPGet("/users/" + SignInUser + "/organizations",

            (status, response) => {

                //let user          = SignInUser;
                const organizations = JSON.parse(response);
                organizationsDiv.innerHTML = "";

                for (const organization of organizations)
                    PrintOrganization(organization, organizationsDiv);

                // If there is only a single organization: Open it!
                if (myOrgs.length === 1) {

                    (myOrgs[0].querySelector('.properties') as HTMLDivElement).style.display = 'table';

                    (myOrgs[0].children[0].children[myOrgs[0].children[0].childElementCount - 1] as HTMLDivElement).style.display = 'block';

                }

            },

            (statusCode, status, response) => {
                organizationsDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch your organizations data!</div>";
            });

}
