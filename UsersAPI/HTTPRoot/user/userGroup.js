function StartUserGroup() {
    function AnyChangesMade() {
        // name
        if ((UserProfileJSON.name !== undefined ? UserProfileJSON.name : "") !== name.value) {
            if (name.value == "") {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a username!</div>";
                return false;
            }
            return true;
        }
        // description
        if ((UserProfileJSON.description !== undefined ? firstValue(UserProfileJSON.description) : "") !== description.value) {
            return true;
        }
        return false;
    }
    function ToogleSaveButton() {
        var changesDetected = AnyChangesMade();
        saveButton.disabled = !changesDetected;
        if (changesDetected)
            responseDiv.innerHTML = "";
        return changesDetected;
    }
    function SaveData() {
        // name
        if ((UserProfileJSON.name !== undefined ? UserProfileJSON.name : "") !== name.value)
            UserProfileJSON.name = name.value;
        // description
        if ((UserProfileJSON.description !== undefined ? firstValue(UserProfileJSON.description) : "") !== description.value)
            UserProfileJSON.description = { "en": description.value };
        if (description.value === "")
            delete (UserProfileJSON.description);
        HTTPSet("/userGroups/" + UserGroupJSON["@id"], UserGroupJSON, (status, response) => {
            try {
                const responseJSON = JSON.parse(response);
                responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully stored updated user profile data.</div>";
                saveButton.disabled = !AnyChangesMade();
            }
            catch (exception) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing user profile data failed:<br />" + exception + "</div>";
            }
        }, (statusCode, status, response) => {
            let responseJSON = { "description": "HTTP Error " + statusCode + " - " + status + "!" };
            if (response != null && response != "") {
                try {
                    responseJSON = JSON.parse(response);
                }
                catch (_a) { }
            }
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing user profile data failed!" +
                (responseJSON.description != null ? "<br />" + responseJSON.description : "") +
                "</div>";
        });
    }
    const pathElements = window.location.pathname.split("/");
    const userGroupId = pathElements[pathElements.length - 1];
    const userGroupDiv = document.getElementById('userGroup');
    const userGroupIdDiv = userGroupDiv.querySelector("#id");
    //userGroupIdDiv.innerText = userGroupId;
    const data = userGroupDiv.querySelector('#data');
    //const groupId          = data.           querySelector ('#login')                          as HTMLInputElement;
    const name = data.querySelector('#name');
    const description = data.querySelector('#description');
    const nameError = name.parentElement.querySelector('.validationError');
    const responseDiv = document.getElementById("response");
    const lowerButtonsDiv = userGroupDiv.querySelector('#lowerButtons');
    const saveButton = lowerButtonsDiv.querySelector("#saveButton");
    //groupId.value          = userGroupId;
    function VerifyName() {
        const nameText = name.value;
        if (/^(.{4,})$/.test(nameText) == false) {
            saveButton.disabled = true;
            name.classList.add("error");
            nameError.innerText = "Invalid user group name!";
            nameError.style.display = "flex";
        }
        else {
            name.classList.remove("error");
            nameError.style.display = "none";
            ToogleSaveButton();
        }
    }
    HTTPGet("/userGroups/" + userGroupId + "?showMetadata", (status, response) => {
        try {
            UserGroupJSON = ParseJSON_LD(response);
            name.value = firstValue(UserGroupJSON.name);
            UpdateI18NTextArea(description, UserGroupJSON.description);
            if (UserGroupJSON["youCanEdit"]) {
                name.readOnly = false;
                name.oninput = () => { VerifyName(); };
                description.oninput = () => { ToogleSaveButton(); };
                description.readOnly = false;
                description.oninput = () => { ToogleSaveButton(); };
                saveButton.style.display = "block";
                saveButton.onclick = () => {
                    SaveData();
                };
            }
        }
        catch (exception) {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch user group data from the remote API!<br />" +
                "Exception: " + exception +
                "</div>";
        }
    }, (statusCode, status, response) => {
        try {
            var responseJSON = response !== "" ? JSON.parse(response) : { "description": "Received an empty response from the remote API!" };
            var info = responseJSON.description !== null ? "<br />" + responseJSON.description : "";
            responseDiv.innerHTML = "<div class=\"HTTP Error\">An error occured while getting user group data from the remote API: " + info + "</div>";
        }
        catch (exception) {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">An exception occured while getting user group data from the remote API:<br />" + exception + "</div>";
        }
    });
}
//# sourceMappingURL=userGroup.js.map