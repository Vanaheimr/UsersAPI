function StartOrganizationMembers() {
    function ImpersonateUser(newUserId) {
        HTTPImpersonate("/users/" + newUserId, (HTTPStatus, ResponseText) => {
            window.location.reload(true);
        }, (HTTPStatus, StatusText, ResponseText) => {
            alert("Not allowed!");
        });
    }
    function ShowUser(userDiv, member) {
        const memberDiv = userDiv.appendChild(document.createElement('div'));
        memberDiv.className = "member";
        if (typeof member == 'string') {
            memberDiv.onclick = () => window.location.href = "../../users/" + member;
            memberDiv.innerHTML = member;
        }
        else {
            memberDiv.onclick = () => window.location.href = "../../users/" + member["@id"];
            const frameDiv = memberDiv.appendChild(document.createElement('div'));
            frameDiv.className = "frame";
            const imageDiv = frameDiv.appendChild(document.createElement('div'));
            imageDiv.className = "avatar";
            imageDiv.innerHTML = "<img src=\"/shared/UsersAPI/images/dummy-user-gray-150x150.png\" >";
            const infoDiv = frameDiv.appendChild(document.createElement('div'));
            infoDiv.className = "info";
            const nameDiv = infoDiv.appendChild(document.createElement('div'));
            nameDiv.className = "name";
            nameDiv.innerHTML = member.name;
            if (member.description) {
                const descriptionDiv = infoDiv.appendChild(document.createElement('div'));
                descriptionDiv.className = "description";
                descriptionDiv.innerHTML = ShowI18N(member.description);
            }
            const emailDiv = infoDiv.appendChild(document.createElement('div'));
            emailDiv.className = "email";
            emailDiv.innerHTML = "<i class=\"fas fa-envelope\"></i> <a href=\"mailto:" + member.email + "\">" + member.email + "</a>";
            // Authenticated
            // Accpeted EULA
            const toolsDiv = frameDiv.appendChild(document.createElement('div'));
            toolsDiv.className = "tools";
            //            toolsDiv.innerHTML = "tools";
            const impersonateUserButton = toolsDiv.appendChild(document.createElement('button'));
            impersonateUserButton.className = "impersonateUser";
            impersonateUserButton.title = "Impersonate this user!";
            impersonateUserButton.innerHTML = '<i class="fas fa-theater-masks"></i> Impersonate</i>';
            impersonateUserButton.onclick = () => {
                ImpersonateUser(member["@id"]);
            };
        }
    }
    const pathElements = window.location.pathname.split("/");
    const organizationId = pathElements[pathElements.length - 2];
    FixMenuLinks("organizationMenu", organizationId);
    const organizationDiv = document.getElementById("organization");
    const headlineDiv = organizationDiv.querySelector("#headline");
    const addNewMemberButton = organizationDiv.querySelector('#addNewMemberButton');
    const adminsDiv = organizationDiv.querySelector('#adminsDiv');
    const membersDiv = organizationDiv.querySelector('#membersDiv');
    const guestsDiv = organizationDiv.querySelector('#guestsDiv');
    const responseDiv = document.getElementById("response");
    HTTPGet("/organizations/" + organizationId + "?showMgt&expand=members", (status, response) => {
        var _a, _b, _c;
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
                for (const admin of organizationJSON.admins)
                    ShowUser(adminsDiv, admin);
            if (((_b = organizationJSON.members) === null || _b === void 0 ? void 0 : _b.length) > 0)
                for (const member of organizationJSON.members)
                    ShowUser(membersDiv, member);
            if (((_c = organizationJSON.guests) === null || _c === void 0 ? void 0 : _c.length) > 0)
                for (const guest of organizationJSON.guests)
                    ShowUser(guestsDiv, guest);
            if (organizationJSON.youCanAddMembers) {
                addNewMemberButton.disabled = false;
                addNewMemberButton.onclick = () => window.location.href = "newMember";
            }
        }
        catch (exception) {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch organization members from the remote API!<br />" +
                "Exception: " + exception +
                "</div>";
        }
    }, (statusCode, status, response) => {
        try {
            var responseJSON = response !== "" ? JSON.parse(response) : { "description": "Received an empty response from the remote API!" };
            var info = responseJSON.description !== null ? "<br />" + responseJSON.description : "";
            responseDiv.innerHTML = "<div class=\"HTTP Error\">An error occured while getting organization members from the remote API: " + info + "</div>";
        }
        catch (exception) {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">An exception occured while getting organization members from the remote API:<br />" + exception + "</div>";
        }
    });
}
//# sourceMappingURL=members.js.map