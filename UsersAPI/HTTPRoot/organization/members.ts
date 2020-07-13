
function StartOrganizationMembers() {

    function AnyChangesMade(): boolean {

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

    function ToogleSaveButton(): boolean {

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

        HTTPImpersonate("/users/" + newUserId,

            (HTTPStatus, ResponseText) => {
                window.location.reload(true);
            },

            (HTTPStatus, StatusText, ResponseText) => {
                alert("Not allowed!");
            });

    }

    function ShowUser(userDiv:  HTMLDivElement,
                      member:   string | IUserProfile)
    {

        const memberDiv = userDiv.appendChild(document.createElement('div')) as HTMLDivElement;
        memberDiv.className = "member";

        if (typeof member == 'string')
        {
            memberDiv.onclick = () => window.location.href = "../../users/" + member;
            memberDiv.innerHTML = member;
        }

        else {

            memberDiv.onclick = () => window.location.href = "../../users/" + member["@id"];

            const frameDiv = memberDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            frameDiv.className = "frame";

            const imageDiv = frameDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            imageDiv.className = "avatar";
            imageDiv.innerHTML = "<img src=\"/shared/UsersAPI/images/dummy-user-gray-150x150.png\" >";


            const infoDiv = frameDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            infoDiv.className = "info";

            const nameDiv = infoDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            nameDiv.className = "name";
            nameDiv.innerHTML = member.name;

            if (member.description) {
                const descriptionDiv = infoDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                descriptionDiv.className = "description";
                descriptionDiv.innerHTML = ShowI18N(member.description);
            }

            const emailDiv = infoDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            emailDiv.className = "email";
            emailDiv.innerHTML = "<i class=\"fas fa-envelope\"></i> <a href=\"mailto:" + member.email + "\">" + member.email + "</a>";

            // Authenticated

            // Accpeted EULA


            const toolsDiv = frameDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            toolsDiv.className = "tools";
//            toolsDiv.innerHTML = "tools";


            const impersonateUserButton = toolsDiv.appendChild(document.createElement('button')) as HTMLButtonElement;
            impersonateUserButton.className = "impersonateUser";
            impersonateUserButton.title     = "Impersonate this user!"
            impersonateUserButton.innerHTML = '<i class="fas fa-theater-masks"></i> Impersonate</i>';
            impersonateUserButton.onclick = () => {
                ImpersonateUser(member["@id"]);
            }

        }

    }


    const pathElements         = window.location.pathname.split("/");
    const organizationId       = pathElements[pathElements.length - 2];

    const organizationMenuDiv  = document.getElementById("organizationMenu") as HTMLDivElement;
    const links                = organizationMenuDiv.querySelectorAll("a");

    for (let i = 0; i < links.length; i++) {

        if (links[i].href.indexOf("00000000") > 0) {
            links[i].href = links[i].href.replace("00000000", organizationId);
        }

    }

    const organizationDiv      = document.getElementById("organization")                as HTMLDivElement;
    const headlineDiv          = organizationDiv.querySelector ("#headline")            as HTMLDivElement;

    const addNewMemberButton   = organizationDiv.querySelector ('#addNewMemberButton')  as HTMLButtonElement;

    const nameDiv              = organizationDiv.querySelector ('#nameDiv')             as HTMLDivElement;
    const name                 = organizationDiv.querySelector ('#name')                as HTMLInputElement;
    const adminsDiv            = organizationDiv.querySelector ('#adminsDiv')           as HTMLDivElement;
    const membersDiv           = organizationDiv.querySelector ('#membersDiv')          as HTMLDivElement;

    const responseDiv          = document.    getElementById("response")                as HTMLDivElement;

    name.onchange              = () => { ToogleSaveButton(); }
    name.onkeyup               = () => { ToogleSaveButton(); }


    var aa = SignInUser;

    HTTPGet("/organizations/" + organizationId + "?showMgt&expand=members",

            (status, response) => {

                try
                {

                    organizationJSON    = ParseJSON_LD<IOrganization>(response);

                    (headlineDiv.querySelector("#name #language")        as HTMLDivElement).innerText = firstKey  (organizationJSON.name);
                    (headlineDiv.querySelector("#name #I18NText")        as HTMLDivElement).innerText = firstValue(organizationJSON.name);

                    if (organizationJSON.description) {
                        (headlineDiv.querySelector("#description")           as HTMLDivElement).style.display = "block";
                        (headlineDiv.querySelector("#description #language") as HTMLDivElement).innerText = firstKey  (organizationJSON.description);
                        (headlineDiv.querySelector("#description #I18NText") as HTMLDivElement).innerText = firstValue(organizationJSON.description);
                    }

                    if (organizationJSON.admins?.length > 0)
                        for (const member of organizationJSON.admins)
                            ShowUser(adminsDiv, member);

                    if (organizationJSON.members?.length > 0)
                        for (const member of organizationJSON.members)
                            ShowUser(membersDiv, member);

                    if (organizationJSON.youCanAddMembers) {
                        addNewMemberButton.disabled = false;
                        addNewMemberButton.onclick = () => window.location.href = "newMember";
                    }

                }
                catch (exception)
                {
                    responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch organization members data from server!<br />" +
                                                "Exception: " + exception +
                                            "</div>";
                }

            },

            (statusCode, status, response) => {

                try
                {

                    const responseJSON     = JSON.parse(response);
                    responseDiv.innerHTML  = "<div class=\"HTTP Error\">Could not fetch organization members data from server!<br />" +
                                                 (responseJSON.description != null
                                                      ? responseJSON.description
                                                      : "HTTP Error " + statusCode + " - " + status) +
                                             "</div>";

                }
                catch (exception)
                {
                    responseDiv.innerHTML  = "<div class=\"HTTP Error\">Could not fetch organization members data from server!<br />" +
                                                 "Exception: " + exception +
                                             "</div>";
                }

            });

}
