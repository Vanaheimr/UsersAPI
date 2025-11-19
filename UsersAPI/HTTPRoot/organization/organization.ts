
function StartOrganization() {

    function AnyChangesMade(): boolean {

        // name
        if ((organizationJSON.name !== undefined ? firstValue(organizationJSON.name) : "") !== name.value) {

            if (name.value == "" && name.value.length < 4) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Organizations must have a valid name!</div>";
                return false;
            }

            return true;

        }


        // description
        if ((organizationJSON.description !== undefined ? firstValue(organizationJSON.description) : "") !== description.value) {

            if (description.value != "" && description.value.length < 4) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Organizations must have a valid description!</div>";
                return false;
            }

            return true;

        }


        // website
        if ((organizationJSON.website !== undefined ? organizationJSON.website : "") !== website.value) {

            if (website.value != "" && website.value.length < 4) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Organizations must have a valid website/Internet URL!</div>";
                return false;
            }

            return true;

        }


        // email
        if ((organizationJSON.email !== undefined ? organizationJSON.email : "") !== email.value) {

            if (email.value != "" && (email.value.length < 6 || email.value.indexOf("@") < 1 || email.value.indexOf(".") < 4)) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Organizations must have a valid e-mail address!</div>";
                return false;
            }

            return true;

        }


        // telephone
        if ((organizationJSON.telephone !== undefined ? organizationJSON.telephone : "") !== telephone.value) {

            if (telephone.value != "" && telephone.value.length < 6) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Organizations must have a valid telephone number!</div>";
                return false;
            }

            return true;

        }


        responseDiv.innerHTML = "";
        return false;

    }

    function ToogleSaveButton(): boolean {

        var changesDetected = AnyChangesMade();

        saveButton.disabled = !changesDetected;

        if (changesDetected)
            responseDiv.innerHTML = "";

        return changesDetected;

    }

    function SaveData() {

        saveButton.disabled          = true;
        organizationJSON["@context"] = "https://opendata.social/contexts/UsersAPI/organization";

        // name
        if ((organizationJSON.name !== undefined ? firstValue(organizationJSON.name) : "") !== name.value) {

            if (name.value != "")
                organizationJSON.name = { "en": name.value };

            else
                delete organizationJSON.name;

        }


        // description
        if ((organizationJSON.description !== undefined ? firstValue(organizationJSON.description) : "") !== description.value) {

            if (description.value != "")
                organizationJSON.description = { "en": description.value };

            else
                delete organizationJSON.description;

        }


        // website
        if ((organizationJSON.website !== undefined ? organizationJSON.website : "") !== website.value)
            organizationJSON.website = website.value;

        if (organizationJSON.website === "")
            delete organizationJSON.website;


        // email
        if ((organizationJSON.email !== undefined ? organizationJSON.email : "") !== email.value)
            organizationJSON.email = email.value;

        if (organizationJSON.email === "")
            delete organizationJSON.email;


        // telephone
        if ((organizationJSON.telephone !== undefined ? organizationJSON.telephone : "") !== telephone.value)
            organizationJSON.telephone = telephone.value;

        if (organizationJSON.telephone === "")
            delete organizationJSON.telephone;


        HTTPSet("/organizations/" + organizationJSON["@id"],
                organizationJSON,

            (HTTPStatus, ResponseText) => {

                try
                {

                    const responseJSON  = ResponseText             != ""   ? JSON.parse(ResponseText) : {};
                    //const info          = responseJSON.description !== null ? "<br />" + firstValue(responseJSON.description) : "";

                    //saveButton.disabled   = !AnyChangesMade();
                    responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully stored updated organization data.</div>";

                    // Redirect after 2 seconds!
                    setTimeout(function () {
                        window.location.href = organizationJSON["@id"];
                    }, 2000);

                }
                catch (exception)
                {
                    saveButton.disabled = !AnyChangesMade();
                    responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing organization data failed!<br />" + exception + "</div>";
                }

            },

            (HTTPStatus, StatusText, ResponseText) => {

                try
                {

                    const responseJSON  = ResponseText             != ""   ? JSON.parse(ResponseText) : {};
                    const info          = responseJSON.description !== null ? "<br />" + responseJSON.description : "";

                    responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing organization data failed!<br />" + info + "</div>";

                }
                catch (exception)
                {

                }

            });

    }

    const pathElements               = window.location.pathname.split("/");
    const organizationId             = pathElements[pathElements.length - 1];

    FixMenuLinks("organizationMenu", organizationId);

    const organizationDiv            = document.          getElementById("organization")                   as HTMLDivElement;
    const headlineDiv                = organizationDiv.   querySelector ("#headline")                      as HTMLDivElement;

    const upperButtonsDiv            = organizationDiv.   querySelector ('#upperButtons')                  as HTMLDivElement;
    const deleteOrganizationButton   = upperButtonsDiv.   querySelector ('#deleteOrganizationButton')      as HTMLButtonElement;

    const dataDiv                    = organizationDiv.   querySelector ('#data')                          as HTMLDivElement;
    const name                       = dataDiv.           querySelector ('#name')                          as HTMLInputElement;
    const description                = dataDiv.           querySelector ('#description')                   as HTMLTextAreaElement;
    const website                    = dataDiv.           querySelector ('#website')                       as HTMLInputElement;
    const email                      = dataDiv.           querySelector ('#email')                         as HTMLInputElement;
    const telephone                  = dataDiv.           querySelector ('#telephone')                     as HTMLInputElement;
    const parentsDiv                 = dataDiv.           querySelector ('#parentsDiv')                    as HTMLDivElement;
    const subOrganizationsDiv        = dataDiv.           querySelector ('#subOrganizationsDiv')           as HTMLDivElement;

    const responseDiv                = document.          getElementById("response")                       as HTMLDivElement;

    const lowerButtonsDiv            = organizationDiv.   querySelector ('#lowerButtons')                  as HTMLDivElement;
    const saveButton                 = lowerButtonsDiv.   querySelector ("#saveButton")                    as HTMLButtonElement;

    const confirmToDeleteDiv         = document.          getElementById("confirmToDeleteOrganization")    as HTMLDivElement;
    const yes                        = confirmToDeleteDiv.querySelector ('#yes')                           as HTMLButtonElement;
    const no                         = confirmToDeleteDiv.querySelector ('#no')                            as HTMLButtonElement;

    const deletionFailedDiv          = document.          getElementById("deletionFailed")                 as HTMLDivElement;
    const deletionFailedDescription  = deletionFailedDiv. querySelector ('#description')                   as HTMLDivElement;
    const ok                         = deletionFailedDiv. querySelector ('#ok')                            as HTMLButtonElement;

    ok.onclick                       = () => { deletionFailedDiv.style.display  = "none"; }

    name.oninput                     = () => { ToogleSaveButton(); }
    description.oninput              = () => { ToogleSaveButton(); }
    description.onkeyup              = () => { ToogleSaveButton(); }
    website.oninput                  = () => { ToogleSaveButton(); }
    email.oninput                    = () => { ToogleSaveButton(); }
    telephone.oninput                = () => { ToogleSaveButton(); }

    saveButton.onclick               = () => { SaveData(); }


    HTTPGet("/organizations/" + organizationId + "?showMgt&expand=parents,subOrganizations",

            (status, response) => {

                try
                {

                    organizationJSON    = ParseJSON_LD<IOrganization>(response);

                    (headlineDiv.querySelector("#name #language")        as HTMLDivElement).innerText = firstKey  (organizationJSON.name);
                    (headlineDiv.querySelector("#name #I18NText")        as HTMLDivElement).innerText = firstValue(organizationJSON.name);

                    if (organizationJSON.description) {

                        (headlineDiv.querySelector("#description") as HTMLDivElement).style.display = "block";
                        (headlineDiv.querySelector("#description #language") as HTMLDivElement).innerText = firstKey  (organizationJSON.description);
                        (headlineDiv.querySelector("#description #I18NText") as HTMLDivElement).innerText = firstValue(organizationJSON.description);

                        UpdateI18N(description.parentElement as HTMLDivElement, organizationJSON.description);

                    }

                    // data
                    UpdateI18N(name.parentElement        as HTMLDivElement, organizationJSON.name);

                    website.value    = organizationJSON.website   !== undefined ? organizationJSON.website   : "";// "<a href=\"https://" + OrganizationJSON.website + "\">" + OrganizationJSON.website + "</a>";
                    email.value      = organizationJSON.email     !== undefined ? organizationJSON.email     : ""; // "<a href=\"mailto:" + OrganizationJSON.email + "\">" + OrganizationJSON.email + "</a>";
                    telephone.value  = organizationJSON.telephone !== undefined ? organizationJSON.telephone : ""; // "<a href=\"tel:" + OrganizationJSON.telephone.replace(/^+/g, "00").replace(/[^0-9]/g, '') + "\">" + OrganizationJSON.telephone + "</a>";

                    if (organizationJSON.parents.length > 0)
                    {

                        if ((typeof organizationJSON.parents[0] == "string" && organizationJSON.parents[0]        === "NoOwner") ||
                            (typeof organizationJSON.parents[0] == "object" && organizationJSON.parents[0]["@id"] === "NoOwner"))
                        {
                            parentsDiv.parentElement.style.display = "none";
                        }

                        else
                        {
                            for (const parent of organizationJSON.parents) {

                                const subOrganizationDiv = parentsDiv.appendChild(document.createElement('button')) as HTMLButtonElement;
                                subOrganizationDiv.className  = "organization";
                                subOrganizationDiv.innerHTML  = typeof parent == 'object'
                                                                    ? firstValue(parent["name"])
                                                                    : parent;
                                subOrganizationDiv.onclick    = () => typeof parent == 'object'
                                                                    ? window.location.href = parent["@id"]
                                                                    : window.location.href = parent;

                            }
                        }

                    }


                    if (organizationJSON.subOrganizations)
                    {
                        for (const subOrganization of organizationJSON.subOrganizations) {

                            const subOrganizationDiv = subOrganizationsDiv.appendChild(document.createElement('button')) as HTMLButtonElement;
                            subOrganizationDiv.className  = "organization";
                            subOrganizationDiv.innerHTML  = typeof subOrganization == 'object'
                                                                ? firstValue(subOrganization["name"])
                                                                : subOrganization;
                            subOrganizationDiv.onclick    = () => typeof subOrganization == 'object'
                                                                ? window.location.href = subOrganization["@id"]
                                                                : window.location.href = subOrganization;

                        }
                    }

                    if (organizationJSON.youCanCreateChildOrganizations) {
                        const subOrganizationDiv = subOrganizationsDiv.appendChild(document.createElement('button')) as HTMLButtonElement;
                        subOrganizationDiv.className = "organization";
                        subOrganizationDiv.innerHTML = "<i class=\"fas fa-plus\"></i>";
                        subOrganizationDiv.onclick   = () => window.location.href = organizationId + "/newSubOrganization";
                    }

                    if (organizationJSON.youCanCreateChildOrganizations) {
                        deleteOrganizationButton.disabled = false;
                        deleteOrganizationButton.onclick = () => {

                            confirmToDeleteDiv.style.display = "block";

                            yes.onclick = () => {

                                HTTPDelete("/organizations/" + organizationId,

                                           // Ok!
                                           (status, response) => {
                                               try
                                               {

                                                   confirmToDeleteDiv.style.display = "none";
                                                   responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully deleted this organization.</div>";

                                                   // Redirect after 2 seconds!
                                                   setTimeout(function () {
                                                       window.location.href = typeof organizationJSON.parents[0] === 'string'
                                                                                   ? organizationJSON.parents[0]
                                                                                   : organizationJSON.parents[0]["@id"];
                                                   }, 2000);

                                               }
                                               catch (exception) {
                                                   responseDiv.innerHTML = "<div class=\"HTTP Error\">Deleting this organization failed!</div>";
                                               }
                                           },

                                           // Failed dependencies, e.g. organization still has members!
                                           (status, response) => {
                                               try
                                               {
                                                   const responseJSON = response !== "" ? JSON.parse(response) : {};
                                                   confirmToDeleteDiv.style.display = "none";
                                                   deletionFailedDiv.style.display  = "block";
                                                   deletionFailedDescription.innerHTML = responseJSON.errorDescription.en;
                                               }
                                               catch (exception)
                                               {
                                                   responseDiv.innerHTML = "<div class=\"HTTP Error\">Deleting this organization failed!</div>";
                                               }
                                           },

                                           // Some error occurred!
                                           (statusCode, status, response) => {
                                               try
                                               {

                                                   confirmToDeleteDiv.style.display = "none";

                                                   const responseJSON = response !== "" ? JSON.parse(response) : {};
                                                   const info         = responseJSON.description !== null ? "<br />" + responseJSON.description : "";
                                                   responseDiv.innerHTML = "<div class=\"HTTP Error\">Deleting this organization failed!<br />" + info + "</div>";

                                               }
                                               catch (exception) {
                                                   responseDiv.innerHTML = "<div class=\"HTTP Error\">Deleting this organization failed!</div>";
                                               }
                                           });

                            };

                            no.onclick = () => {

                                confirmToDeleteDiv.style.display = "none";

                            };

                        };
                    }

                }
                catch (exception)
                {

                }

            },

            (statusCode, status, response) => {

            });

}
