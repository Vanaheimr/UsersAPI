
function StartOrganizationMembers() {

    function ImpersonateUser(newUserId) {

        HTTPImpersonate("/users/" + newUserId,

            (HTTPStatus, ResponseText) => {
                window.location.reload();
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


            const removeUserButton = toolsDiv.appendChild(document.createElement('button')) as HTMLButtonElement;
            removeUserButton.className = "removeUser";
            removeUserButton.title     = "Remove this user from the organization!"
            removeUserButton.innerHTML = '<i class="fas fa-trash-alt"></i> Remove</i>';
            removeUserButton.onclick = (event) => {

                event.stopPropagation();
                confirmToRemoveUserDiv.style.display = "block";

                yes.onclick = () => {

                    HTTPDelete("_all/" + member["@id"],

                                // Ok!
                                (status, response) => {
                                    try
                                    {

                                        confirmToRemoveUserDiv.style.display = "none";
                                        responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully removed this user from the organization.</div>";

                                        // Redirect after 2 seconds!
                                        setTimeout(function () {
                                            //window.location.href = window.location.href;
                                            location.reload();
                                        }, 2000);

                                    }
                                    catch (exception) {
                                        responseDiv.innerHTML = "<div class=\"HTTP Error\">Removing this user from the organization failed!</div>";
                                    }
                                },

                                // Failed dependencies, e.g. user still has attached data!
                                (status, response) => {
                                    try
                                    {
                                        const responseJSON = response !== "" ? JSON.parse(response) : {};
                                        confirmToRemoveUserDiv.style.display = "none";
                                        removalFailedDiv.style.display       = "block";
                                        removalFailedReason.innerHTML        = responseJSON.errorDescription.en;
                                    }
                                    catch (exception)
                                    {
                                        responseDiv.innerHTML = "<div class=\"HTTP Error\">Removing this user from the organization failed!</div>";
                                    }
                                },

                                // Some error occurred!
                                (statusCode, status, response) => {
                                    try
                                    {

                                        confirmToRemoveUserDiv.style.display = "none";

                                        const responseJSON    = response !== "" ? JSON.parse(response) : {};
                                        const info            = responseJSON.description !== null ? "<br />" + responseJSON.description : "";
                                        responseDiv.innerHTML = "<div class=\"HTTP Error\">Removing this user from the organization failed!<br />" + info + "</div>";

                                    }
                                    catch (exception) {
                                        responseDiv.innerHTML = "<div class=\"HTTP Error\">Removing this user from the organization failed!</div>";
                                    }
                                });

                };

                no.onclick = () => {

                    confirmToRemoveUserDiv.style.display = "none";

                };

            }

        }

    }


    const pathElements              = window.location.pathname.split("/");
    const organizationId            = pathElements[pathElements.length - 2];

    FixMenuLinks("organizationMenu", organizationId);

    const organizationDiv           = document.              getElementById("organization")          as HTMLDivElement;
    const headlineDiv               = organizationDiv.       querySelector ("#headline")             as HTMLDivElement;

    const addNewMemberButton        = organizationDiv.       querySelector ('#addNewMemberButton')   as HTMLButtonElement;

    const adminsDiv                 = organizationDiv.       querySelector ('#adminsDiv')            as HTMLDivElement;
    const membersDiv                = organizationDiv.       querySelector ('#membersDiv')           as HTMLDivElement;
    const guestsDiv                 = organizationDiv.       querySelector ('#guestsDiv')            as HTMLDivElement;

    const responseDiv               = document.              getElementById("response")              as HTMLDivElement;

    const confirmToRemoveUserDiv    = document.              getElementById("confirmToRemoveUser")   as HTMLDivElement;
    const yes                       = confirmToRemoveUserDiv.querySelector ('#yes')                  as HTMLButtonElement;
    const no                        = confirmToRemoveUserDiv.querySelector ('#no')                   as HTMLButtonElement;

    const removalFailedDiv          = document.              getElementById("removalFailed")         as HTMLDivElement;
    const removalFailedReason       = removalFailedDiv.      querySelector ('#reason')               as HTMLDivElement;
    const ok                        = removalFailedDiv.      querySelector ('#ok')                   as HTMLButtonElement;

    ok.onclick                      = () => { removalFailedDiv.style.display  = "none"; }


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
                        for (const admin of organizationJSON.admins)
                            ShowUser(adminsDiv, admin);

                    if (organizationJSON.members?.length > 0)
                        for (const member of organizationJSON.members)
                            ShowUser(membersDiv, member);

                    if (organizationJSON.guests?.length > 0)
                        for (const guest of organizationJSON.guests)
                            ShowUser(guestsDiv, guest);

                    if (organizationJSON.youCanAddMembers) {
                        addNewMemberButton.disabled = false;
                        addNewMemberButton.onclick = () => window.location.href = "newMember";
                    }

                }
                catch (exception)
                {
                    responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch organization members from the remote API!<br />" +
                                                "Exception: " + exception +
                                            "</div>";
                }

            },

            (statusCode, status, response) => {

                try
                {

                    var responseJSON  = response                 !== ""   ? JSON.parse(response)                : { "description": "Received an empty response from the remote API!" };
                    var info          = responseJSON.description !== null ? "<br />" + responseJSON.description : "";

                    responseDiv.innerHTML = "<div class=\"HTTP Error\">An error occurred while getting organization members from the remote API: " + info + "</div>";

                }
                catch (exception)
                {
                    responseDiv.innerHTML = "<div class=\"HTTP Error\">An exception occurred while getting organization members from the remote API:<br />" + exception + "</div>";
                }

            });

}
