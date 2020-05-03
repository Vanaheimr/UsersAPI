function StartNewMember() {
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
    var accessRights = dataDiv.querySelector('#accessRights');
    var login = dataDiv.querySelector('#login');
    var name = dataDiv.querySelector('#name');
    var email = dataDiv.querySelector('#email');
    var telephone = dataDiv.querySelector('#telephone');
    var mobilephone = dataDiv.querySelector('#mobilephone');
    var homepage = dataDiv.querySelector('#homepage');
    var description = dataDiv.querySelector('#description');
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
        var newUserJSON = {
            "@context": "https://opendata.social/contexts/UsersAPI+json/user",
            //   "parentOrganization":  organizationId,
            "name": { "eng": name.value.trim() }
        };
        newUserJSON["accessRights"] = accessRights.selectedOptions[accessRights.selectedIndex].value;
        if (email.value.trim() !== "")
            newUserJSON["email"] = email.value.trim();
        if (telephone.value.trim() !== "")
            newUserJSON["telephone"] = telephone.value.trim();
        if (mobilephone.value.trim() !== "")
            newUserJSON["mobilephone"] = mobilephone.value.trim();
        if (homepage.value.trim() !== "")
            newUserJSON["homepage"] = homepage.value.trim();
        if (description.value.trim() !== "")
            newUserJSON["description"] = { "eng": description.value.trim() };
        HTTPAdd("/users", newUserJSON, function (statusCode, status, response) {
            try {
                var responseJSON_1 = JSON.parse(response);
                responseDiv.style.display = "block";
                responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully created this new member.</div>";
                // Redirect to updated organization members view after 2 sec!
                setTimeout(function () {
                    if (responseJSON_1["@id"] != null)
                        window.location.href = "../" + responseJSON_1["@id"];
                }, 2000);
            }
            catch (exception) {
                responseDiv.style.display = "block";
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing the new member failed!<br />" +
                    "Exception: " + exception +
                    "</div>";
            }
        }, function (statusCode, status, response) {
            responseDiv.style.display = "block";
            try {
                var responseJSON = JSON.parse(response);
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing the new member failed!<br />" +
                    (responseJSON.description != null
                        ? responseJSON.description
                        : "HTTP Error " + statusCode + " - " + status) +
                    "</div>";
            }
            catch (exception) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing the new member failed!<br />" +
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
//# sourceMappingURL=newMember.js.map