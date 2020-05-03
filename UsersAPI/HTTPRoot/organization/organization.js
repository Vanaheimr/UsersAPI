function StartOrganization() {
    function AnyChangesMade() {
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
            if (email.value == "" || email.value.length < 6 || email.value.indexOf("@") < 1 || email.value.indexOf(".") < 4) {
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
    function ToogleSaveButton() {
        var changesDetected = AnyChangesMade();
        saveButton.disabled = !changesDetected;
        if (changesDetected)
            responseDiv.innerHTML = "";
        return changesDetected;
    }
    function SaveData() {
        saveButton.disabled = true;
        organizationJSON["@context"] = "https://opendata.social/contexts/UsersAPI+json/organization";
        // name
        if ((organizationJSON.name !== undefined ? firstValue(organizationJSON.name) : "") !== name.value) {
            if (name.value != "") {
                if (organizationJSON.name == null)
                    organizationJSON.name = new Object();
                organizationJSON.name["eng"] = name.value;
            }
            else
                delete organizationJSON.name;
        }
        // description
        if ((organizationJSON.description !== undefined ? firstValue(organizationJSON.description) : "") !== description.value) {
            if (description.value != "") {
                if (organizationJSON.description == null)
                    organizationJSON.description = new Object();
                organizationJSON.description["eng"] = description.value;
            }
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
        HTTPSet("/organizations/" + organizationJSON["@id"], organizationJSON, function (HTTPStatus, ResponseText) {
            try {
                var responseJSON = ResponseText != "" ? JSON.parse(ResponseText) : {};
                var info = responseJSON.description != null ? "<br />" + responseJSON.description : "";
                //saveButton.disabled   = !AnyChangesMade();
                responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully stored updated organization data." + info + "</div>";
                // Redirect after 2 seconds!
                setTimeout(function () {
                    window.location.href = organizationJSON["@id"];
                }, 2000);
            }
            catch (exception) {
                saveButton.disabled = !AnyChangesMade();
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing organization data failed!" + exception + "</div>";
            }
        }, function (HTTPStatus, StatusText, ResponseText) {
            try {
                var responseJSON = ResponseText != "" ? JSON.parse(ResponseText) : {};
                var info = responseJSON.description != null ? "<br />" + responseJSON.description : "";
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing organization data failed!" + info + "</div>";
            }
            catch (exception) {
            }
        });
    }
    var pathElements = window.location.pathname.split("/");
    var organizationId = pathElements[pathElements.length - 1];
    var organizationMenuDiv = document.getElementById("organizationMenu");
    var links = organizationMenuDiv.querySelectorAll("a");
    for (var i = 0; i < links.length; i++) {
        if (links[i].href.indexOf("00000000") > 0) {
            links[i].href = links[i].href.replace("00000000", organizationId);
        }
    }
    var organizationDiv = document.getElementById("organization");
    var headlineDiv = organizationDiv.querySelector("#headline");
    var dataDiv = organizationDiv.querySelector('#data');
    var name = dataDiv.querySelector('#name');
    var description = dataDiv.querySelector('#description');
    var website = dataDiv.querySelector('#website');
    var email = dataDiv.querySelector('#email');
    var telephone = dataDiv.querySelector('#telephone');
    var parentsDiv = dataDiv.querySelector('#parentsDiv');
    var subOrganizationsDiv = dataDiv.querySelector('#subOrganizationsDiv');
    var responseDiv = document.getElementById("response");
    var buttonsDiv = organizationDiv.querySelector('#buttons');
    var deleteOrganizationButton = buttonsDiv.querySelector('#delete');
    var confirmToDelete = document.getElementById("confirmToDeleteOrganization");
    var yes = confirmToDelete.querySelector('#yes');
    var no = confirmToDelete.querySelector('#no');
    var saveButton = document.getElementById("saveButton");
    name.oninput = function () {
        ToogleSaveButton();
    };
    description.oninput = function () {
        ToogleSaveButton();
    };
    description.onkeyup = function () {
        ToogleSaveButton();
    };
    website.oninput = function () {
        ToogleSaveButton();
    };
    email.oninput = function () {
        ToogleSaveButton();
    };
    telephone.oninput = function () {
        ToogleSaveButton();
    };
    saveButton.onclick = function () {
        SaveData();
    };
    HTTPGet("/organizations/" + organizationId + "?showMgt&expand=parents,subOrganizations", function (HTTPStatus, ResponseText) {
        try {
            organizationJSON = ParseJSON_LD(ResponseText);
            headlineDiv.querySelector("#name #language").innerText = firstKey(organizationJSON.name);
            headlineDiv.querySelector("#name #I18NText").innerText = firstValue(organizationJSON.name);
            if (organizationJSON.description) {
                headlineDiv.querySelector("#description").style.display = "block";
                headlineDiv.querySelector("#description #language").innerText = firstKey(organizationJSON.description);
                headlineDiv.querySelector("#description #I18NText").innerText = firstValue(organizationJSON.description);
            }
            // data
            UpdateI18N(name.parentElement, organizationJSON.name);
            UpdateI18N(description.parentElement, organizationJSON.description);
            website.value = organizationJSON.website !== undefined ? organizationJSON.website : ""; // "<a href=\"https://" + OrganizationJSON.website + "\">" + OrganizationJSON.website + "</a>";
            email.value = organizationJSON.email !== undefined ? organizationJSON.email : ""; // "<a href=\"mailto:" + OrganizationJSON.email + "\">" + OrganizationJSON.email + "</a>";
            telephone.value = organizationJSON.telephone !== undefined ? organizationJSON.telephone : ""; // "<a href=\"tel:" + OrganizationJSON.telephone.replace(/^+/g, "00").replace(/[^0-9]/g, '') + "\">" + OrganizationJSON.telephone + "</a>";
            if (organizationJSON.parents.length > 0) {
                if ((typeof organizationJSON.parents[0] == "string" && organizationJSON.parents[0] === "NoOwner") ||
                    (typeof organizationJSON.parents[0] == "object" && organizationJSON.parents[0]["@id"] === "NoOwner")) {
                    parentsDiv.parentElement.style.display = "none";
                }
                else {
                    var _loop_1 = function (parent_1) {
                        var subOrganizationDiv = parentsDiv.appendChild(document.createElement('button'));
                        subOrganizationDiv.className = "organization";
                        subOrganizationDiv.innerHTML = typeof parent_1 == 'object'
                            ? firstValue(parent_1["name"])
                            : parent_1;
                        subOrganizationDiv.onclick = function () { return typeof parent_1 == 'object'
                            ? window.location.href = parent_1["@id"]
                            : window.location.href = parent_1; };
                    };
                    for (var _i = 0, _a = organizationJSON.parents; _i < _a.length; _i++) {
                        var parent_1 = _a[_i];
                        _loop_1(parent_1);
                    }
                }
            }
            if (organizationJSON.subOrganizations) {
                var _loop_2 = function (subOrganization) {
                    var subOrganizationDiv = subOrganizationsDiv.appendChild(document.createElement('button'));
                    subOrganizationDiv.className = "organization";
                    subOrganizationDiv.innerHTML = typeof subOrganization == 'object'
                        ? firstValue(subOrganization["name"])
                        : subOrganization;
                    subOrganizationDiv.onclick = function () { return typeof subOrganization == 'object'
                        ? window.location.href = subOrganization["@id"]
                        : window.location.href = subOrganization; };
                };
                for (var _b = 0, _c = organizationJSON.subOrganizations; _b < _c.length; _b++) {
                    var subOrganization = _c[_b];
                    _loop_2(subOrganization);
                }
            }
            if (organizationJSON.youCanCreateChildOrganizations) {
                var subOrganizationDiv = subOrganizationsDiv.appendChild(document.createElement('button'));
                subOrganizationDiv.className = "organization";
                subOrganizationDiv.innerHTML = "<i class=\"fas fa-plus\"></i>";
                subOrganizationDiv.onclick = function () { return window.location.href = organizationId + "/newSubOrganization"; };
            }
            if (organizationJSON.youCanCreateChildOrganizations) {
                deleteOrganizationButton.disabled = false;
                deleteOrganizationButton.onclick = function () {
                    confirmToDelete.style.display = "flex";
                    yes.onclick = function () {
                        HTTPDelete("/organizations/" + organizationId, function (HTTPStatus, ResponseText) {
                            confirmToDelete.style.display = "none";
                            responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully deleted this organization.</div>";
                            // Redirect after 2 seconds!
                            setTimeout(function () {
                                window.location.href = typeof organizationJSON.parents[0] === 'string'
                                    ? organizationJSON.parents[0]
                                    : organizationJSON.parents[0]["@id"];
                            }, 2000);
                        }, function (HTTPStatus, StatusText, ResponseText) {
                            confirmToDelete.style.display = "none";
                            var responseJSON = ResponseText != "" ? JSON.parse(ResponseText) : {};
                            var info = responseJSON.description != null ? "<br />" + responseJSON.description : "";
                            responseDiv.innerHTML = "<div class=\"HTTP Error\">Deleting this organization failed!" + info + "</div>";
                        });
                    };
                    no.onclick = function () {
                        confirmToDelete.style.display = "none";
                    };
                };
            }
        }
        catch (exception) {
        }
    }, function (HTTPStatus, StatusText, ResponseText) {
    });
}
//# sourceMappingURL=organization.js.map