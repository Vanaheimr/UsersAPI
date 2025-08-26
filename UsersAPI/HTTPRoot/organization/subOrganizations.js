function StartSubOrganizations() {
    function ShowSubOrganization(subOrganizationsDiv, subOrganization) {
        const subOrganizationDiv = subOrganizationsDiv.appendChild(document.createElement('div'));
        subOrganizationDiv.className = "subOrganization";
        if (typeof subOrganization == 'string') {
            subOrganizationDiv.onclick = () => window.location.href = "../../organizations/" + subOrganization;
            subOrganizationDiv.innerHTML = subOrganization;
        }
        else {
            subOrganizationDiv.onclick = () => window.location.href = "../../organizations/" + subOrganization["@id"];
            const frameDiv = subOrganizationDiv.appendChild(document.createElement('div'));
            frameDiv.className = "frame";
            const imageDiv = frameDiv.appendChild(document.createElement('div'));
            imageDiv.className = "avatar";
            imageDiv.innerHTML = "<img src=\"/shared/UsersAPI/images/building-2696768_640.png\" >";
            const infoDiv = frameDiv.appendChild(document.createElement('div'));
            infoDiv.className = "info";
            const nameDiv = infoDiv.appendChild(document.createElement('div'));
            nameDiv.className = "name";
            nameDiv.innerHTML = ShowI18N(subOrganization.name);
            if (subOrganization.description) {
                const descriptionDiv = infoDiv.appendChild(document.createElement('div'));
                descriptionDiv.className = "description";
                descriptionDiv.innerHTML = ShowI18N(subOrganization.description);
            }
            //let emailDiv = infoDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            //emailDiv.className = "email";
            //emailDiv.innerHTML = "<i class=\"fas fa-envelope\"></i> <a href=\"mailto:" + subOrganization.email + "\">" + subOrganization.email + "</a>";
            //const toolsDiv = frameDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            //toolsDiv.className = "tools";
            //toolsDiv.innerHTML = "tools";
        }
    }
    const pathElements = window.location.pathname.split("/");
    const organizationId = pathElements[pathElements.length - 2];
    FixMenuLinks("organizationMenu", organizationId);
    const organizationDiv = document.getElementById("organization");
    const headlineDiv = organizationDiv.querySelector("#headline");
    const upperButtonsDiv = organizationDiv.querySelector('#upperButtons');
    const newSubOrganizationButton = upperButtonsDiv.querySelector('#newSubOrganizationButton');
    const subOrganizationsDiv = organizationDiv.querySelector('#subOrganizations');
    const responseDiv = document.getElementById("response");
    HTTPGet("/organizations/" + organizationId + "?showMgt&expand=subOrganizations", (status, response) => {
        var _a;
        try {
            organizationJSON = ParseJSON_LD(response);
            headlineDiv.querySelector("#name #language").innerText = firstKey(organizationJSON.name);
            headlineDiv.querySelector("#name #I18NText").innerText = firstValue(organizationJSON.name);
            if (organizationJSON.description) {
                headlineDiv.querySelector("#description").style.display = "block";
                headlineDiv.querySelector("#description #language").innerText = firstKey(organizationJSON.description);
                headlineDiv.querySelector("#description #I18NText").innerText = firstValue(organizationJSON.description);
            }
            if (organizationJSON.youCanCreateChildOrganizations) {
                newSubOrganizationButton.disabled = false;
                newSubOrganizationButton.onclick = () => window.location.href = "newSubOrganization";
            }
            if (((_a = organizationJSON.subOrganizations) === null || _a === void 0 ? void 0 : _a.length) > 0) {
                //const rowDiv = dataDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                //rowDiv.className = "row";
                //const keyDiv = rowDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                //keyDiv.className  = "key keyTop";
                //keyDiv.innerHTML  = "Sub-Organizations";
                //const subOrganizationsDiv = rowDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                //subOrganizationsDiv.id        = "subOrganizations";
                //subOrganizationsDiv.className = "value memberGroup";
                for (const subOrganization of organizationJSON.subOrganizations)
                    ShowSubOrganization(subOrganizationsDiv, subOrganization);
            }
        }
        catch (exception) {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch sub-organizations data from server!<br />" +
                "Exception: " + exception +
                "</div>";
        }
    }, (statusCode, status, response) => {
        try {
            const responseJSON = JSON.parse(response);
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch sub-organizations data from server!<br />" +
                (responseJSON.description !== null
                    ? responseJSON.description
                    : "HTTP Error " + statusCode + " - " + status) +
                "</div>";
        }
        catch (exception) {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch sub-organizations data from server!<br />" +
                "Exception: " + exception +
                "</div>";
        }
    });
}
//# sourceMappingURL=subOrganizations.js.map