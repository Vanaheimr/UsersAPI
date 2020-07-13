function StartSubOrganizations() {
    function ShowSubOrganization(subOrganizationsDiv, subOrganization) {
        var subOrganizationDiv = subOrganizationsDiv.appendChild(document.createElement('div'));
        subOrganizationDiv.className = "subOrganization";
        if (typeof subOrganization == 'string') {
            subOrganizationDiv.onclick = function () { return window.location.href = "../../organizations/" + subOrganization; };
            subOrganizationDiv.innerHTML = subOrganization;
        }
        else {
            subOrganizationDiv.onclick = function () { return window.location.href = "../../organizations/" + subOrganization["@id"]; };
            var frameDiv = subOrganizationDiv.appendChild(document.createElement('div'));
            frameDiv.className = "frame";
            var imageDiv = frameDiv.appendChild(document.createElement('div'));
            imageDiv.className = "avatar";
            imageDiv.innerHTML = "<img src=\"/shared/UsersAPI/images/building-2696768_640.png\" >";
            var infoDiv = frameDiv.appendChild(document.createElement('div'));
            infoDiv.className = "info";
            var nameDiv = infoDiv.appendChild(document.createElement('div'));
            nameDiv.className = "name";
            nameDiv.innerHTML = ShowI18N(subOrganization.name);
            if (subOrganization.description) {
                var descriptionDiv = infoDiv.appendChild(document.createElement('div'));
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
    var headlineDiv = organizationDiv.querySelector("#headline");
    var upperButtonsDiv = organizationDiv.querySelector('#upperButtons');
    var newSubOrganizationButton = upperButtonsDiv.querySelector('#newSubOrganizationButton');
    var subOrganizationsDiv = organizationDiv.querySelector('#subOrganizations');
    var responseDiv = document.getElementById("response");
    HTTPGet("/organizations/" + organizationId + "?showMgt&expand=subOrganizations", function (status, response) {
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
                newSubOrganizationButton.onclick = function () { return window.location.href = "newSubOrganization"; };
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
                for (var _i = 0, _b = organizationJSON.subOrganizations; _i < _b.length; _i++) {
                    var subOrganization = _b[_i];
                    ShowSubOrganization(subOrganizationsDiv, subOrganization);
                }
            }
        }
        catch (exception) {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch sub-organizations data from server!<br />" +
                "Exception: " + exception +
                "</div>";
        }
    }, function (statusCode, status, response) {
        try {
            var responseJSON = JSON.parse(response);
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch sub-organizations data from server!<br />" +
                (responseJSON.description != null
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