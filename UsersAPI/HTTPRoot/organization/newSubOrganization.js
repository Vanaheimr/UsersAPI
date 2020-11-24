function StartNewSubOrganization() {
    const pathElements = window.location.pathname.split("/");
    const organizationId = pathElements[pathElements.length - 2];
    const organizationMenuDiv = document.getElementById("organizationMenu");
    const links = organizationMenuDiv.querySelectorAll("a");
    for (let i = 0; i < links.length; i++) {
        if (links[i].href.indexOf("00000000") > 0) {
            links[i].href = links[i].href.replace("00000000", organizationId);
        }
    }
    const newSubOrganizationDiv = document.getElementById("newSubOrganization");
    const headlineDiv = newSubOrganizationDiv.querySelector("#headline");
    const dataDiv = newSubOrganizationDiv.querySelector('#data');
    const name = dataDiv.querySelector('#name');
    const description = dataDiv.querySelector('#description');
    const website = dataDiv.querySelector('#website');
    const email = dataDiv.querySelector('#email');
    const telephone = dataDiv.querySelector('#telephone');
    const responseDiv = document.getElementById("response");
    const lowerButtonsDiv = newSubOrganizationDiv.querySelector('#lowerButtons');
    const saveButton = lowerButtonsDiv.querySelector("#saveButton");
    name.onchange = () => { ToogleSaveButton(); };
    name.onkeyup = () => { ToogleSaveButton(); };
    saveButton.onclick = () => { SaveData(); };
    function ToogleSaveButton() {
        let isValid = false;
        if (name.value.trim() !== "" && name.value.trim().length > 4)
            isValid = true;
        saveButton.disabled = !isValid;
        if (isValid)
            responseDiv.innerHTML = "";
        return isValid;
    }
    function SaveData() {
        const newChildOrganizationJSON = {
            "@context": "https://opendata.social/contexts/UsersAPI/organization",
            "parentOrganization": organizationId,
            "name": { "en": name.value.trim() }
        };
        if (description.value.trim() !== "")
            newChildOrganizationJSON["description"] = { "en": description.value.trim() };
        if (website.value.trim() !== "")
            newChildOrganizationJSON["website"] = website.value.trim();
        if (email.value.trim() !== "")
            newChildOrganizationJSON["email"] = email.value.trim();
        if (telephone.value.trim() !== "")
            newChildOrganizationJSON["telephone"] = telephone.value.trim();
        HTTPAdd("/organizations", newChildOrganizationJSON, (statusCode, status, response) => {
            try {
                const responseJSON = JSON.parse(response);
                responseDiv.style.display = "block";
                responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully created this new sub-organization.</div>";
                // Redirect to updated service ticket after 2 sec!
                setTimeout(function () {
                    if (responseJSON["@id"] != null)
                        window.location.href = "../" + responseJSON["@id"];
                }, 2000);
            }
            catch (exception) {
                responseDiv.style.display = "block";
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing the service ticket might have failed!<br />" +
                    "Exception: " + exception +
                    "</div>";
            }
        }, (statusCode, status, response) => {
            responseDiv.style.display = "block";
            try {
                const responseJSON = JSON.parse(response);
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Creating the new sub-organization failed!<br />" +
                    (responseJSON.description != null
                        ? responseJSON.description
                        : "HTTP Error " + statusCode + " - " + status) +
                    "</div>";
            }
            catch (exception) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing the sub-organization failed!<br />" +
                    "Exception: " + exception +
                    "</div>";
            }
        });
    }
    HTTPGet("/organizations/" + organizationId + "?showMgt&expand=subOrganizations", (status, response) => {
        try {
            organizationJSON = ParseJSON_LD(response);
            headlineDiv.querySelector("#name #language").innerText = firstKey(organizationJSON.name);
            headlineDiv.querySelector("#name #I18NText").innerText = firstValue(organizationJSON.name);
            if (organizationJSON.description != null && firstValue(organizationJSON.description) != null) {
                headlineDiv.querySelector("#description").style.display = "block";
                headlineDiv.querySelector("#description #language").innerText = firstKey(organizationJSON.description);
                headlineDiv.querySelector("#description #I18NText").innerText = firstValue(organizationJSON.description);
            }
        }
        catch (exception) {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch organization data from server!<br />" +
                "Exception: " + exception +
                "</div>";
        }
    }, (statusCode, status, response) => {
        try {
            const responseJSON = JSON.parse(response);
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch organization data from server!<br />" +
                (responseJSON.description != null
                    ? responseJSON.description
                    : "HTTP Error " + statusCode + " - " + status) +
                "</div>";
        }
        catch (exception) {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch organization data from server!<br />" +
                "Exception: " + exception +
                "</div>";
        }
    });
}
//# sourceMappingURL=newSubOrganization.js.map