function StartNewSubOrganization() {
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
    var name = dataDiv.querySelector('#name');
    var description = dataDiv.querySelector('#description');
    var website = dataDiv.querySelector('#website');
    var email = dataDiv.querySelector('#email');
    var telephone = dataDiv.querySelector('#telephone');
    var responseDiv = document.getElementById("response");
    var saveButton = document.getElementById("saveButton");
    function ToogleSaveButton() {
        var isValid = false;
        if (name.value.trim() !== "" && name.value.trim().length > 4)
            isValid = true;
        saveButton.disabled = !isValid;
        if (isValid)
            responseDiv.innerHTML = "";
        return isValid;
    }
    function SaveData() {
        var newChildOrganizationJSON = {
            "@context": "https://opendata.social/contexts/UsersAPI+json/organization",
            "parentOrganization": organizationId,
            "name": { "eng": name.value.trim() }
        };
        if (description.value.trim() !== "")
            newChildOrganizationJSON["description"] = { "eng": description.value.trim() };
        if (website.value.trim() !== "")
            newChildOrganizationJSON["website"] = website.value.trim();
        if (email.value.trim() !== "")
            newChildOrganizationJSON["email"] = email.value.trim();
        if (telephone.value.trim() !== "")
            newChildOrganizationJSON["telephone"] = telephone.value.trim();
        HTTPAdd("/organizations", newChildOrganizationJSON, function (statusCode, status, response) {
            try {
                var responseJSON_1 = JSON.parse(response);
                responseDiv.style.display = "block";
                responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully created this new sub-organization.</div>";
                // Redirect to updated service ticket after 2 sec!
                setTimeout(function () {
                    if (responseJSON_1["@id"] != null)
                        window.location.href = "../" + responseJSON_1["@id"];
                }, 2000);
            }
            catch (exception) {
                responseDiv.style.display = "block";
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing the service ticket might have failed!<br />" +
                    "Exception: " + exception +
                    "</div>";
            }
        }, function (statusCode, status, response) {
            responseDiv.style.display = "block";
            try {
                var responseJSON = JSON.parse(response);
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
    name.onchange = function () {
        ToogleSaveButton();
    };
    name.onkeyup = function () {
        ToogleSaveButton();
    };
    saveButton.onclick = function () {
        SaveData();
    };
    HTTPGet("/organizations/" + organizationId + "?showMgt&expand=subOrganizations", function (status, response) {
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
    }, function (statusCode, status, response) {
        try {
            var responseJSON = JSON.parse(response);
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