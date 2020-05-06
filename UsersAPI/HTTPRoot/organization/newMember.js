function StartNewMember() {
    var newUserJSON = {
        "@id": "",
        "@context": "https://opendata.social/contexts/UsersAPI+json/user",
        "name": null,
        "email": ""
    };
    var pathElements = window.location.pathname.split("/");
    var organizationId = pathElements[pathElements.length - 2];
    var organizationMenuDiv = document.getElementById("organizationMenu");
    var links = organizationMenuDiv.querySelectorAll("a");
    for (var i = 0; i < links.length; i++) {
        if (links[i].href.indexOf("00000000") > 0) {
            links[i].href = links[i].href.replace("00000000", organizationId);
        }
    }
    var newMemberDiv = document.getElementById("newMember");
    var headlineDiv = newMemberDiv.querySelector("#headline");
    var dataDiv = newMemberDiv.querySelector('#data');
    var accessRights = dataDiv.querySelector('#accessRights');
    var login = dataDiv.querySelector('#login');
    var loginError = dataDiv.querySelector('#loginError');
    var name = dataDiv.querySelector('#name');
    var nameError = dataDiv.querySelector('#nameError');
    var email = dataDiv.querySelector('#email');
    var emailError = dataDiv.querySelector('#emailError');
    var telephone = dataDiv.querySelector('#telephone');
    var mobilephone = dataDiv.querySelector('#mobilephone');
    var homepage = dataDiv.querySelector('#homepage');
    var description = dataDiv.querySelector('#description');
    var responseDiv = document.getElementById("response");
    var saveButton = document.getElementById("saveButton");
    login.onchange = function () { VerifyLogin(); };
    login.onkeyup = function () { VerifyLogin(); };
    name.onchange = function () { VerifyName(); };
    name.onkeyup = function () { VerifyName(); };
    email.onchange = function () { VerifyEMail(); };
    email.onkeyup = function () { VerifyEMail(); };
    saveButton.onclick = function () { SaveData(); };
    function VerifyLogin() {
        var UserId = login.value.toLowerCase().trim();
        login.value = UserId;
        if (UserId == "") {
            newUserJSON["@id"] = "";
            login.classList.remove("error");
            loginError.style.display = "none";
            VerifyAll();
        }
        else if (/^([a-zA-Z0-9]{3,})$/.test(UserId) == false) {
            saveButton.disabled = true;
            newUserJSON["@id"] = null;
            login.classList.add("error");
            loginError.innerText = "Invalid user identification!";
            loginError.style.display = "flex";
        }
        else if (newUserJSON["@id"] != UserId) {
            responseDiv.innerHTML = "";
            HTTPGet("/users/" + UserId, 
            // HTTP OK    => bad!
            function (status, response) {
                newUserJSON["@id"] = null;
                login.classList.add("error");
                loginError.innerText = "This user identification already exists!";
                loginError.style.display = "flex";
                saveButton.disabled = true;
            }, 
            // HTTP Error => Maybe good!
            function (HTTPStatus, status, response) {
                // HTTP Not Found => good!
                if (HTTPStatus == 404) {
                    newUserJSON["@id"] = UserId;
                    login.classList.remove("error");
                    loginError.style.display = "none";
                    VerifyAll();
                }
                // Any other status => bad!
                else {
                    responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not validate the given user identification!</div>";
                    newUserJSON["@id"] = null;
                    saveButton.disabled = true;
                }
            });
        }
    }
    function VerifyName() {
        var UserName = name.value;
        if (UserName == "") {
            saveButton.disabled = true;
            newUserJSON.name = null;
        }
        else if (/^(.{4,})$/.test(UserName) == false) {
            saveButton.disabled = true;
            newUserJSON.name = null;
            name.classList.add("error");
            nameError.innerText = "Invalid user name!";
            nameError.style.display = "flex";
        }
        else {
            name.classList.remove("error");
            nameError.style.display = "none";
            newUserJSON.name = { "eng": UserName };
            VerifyAll();
        }
    }
    function VerifyEMail() {
        var EMail = email.value.trim();
        email.value = EMail;
        if (EMail == "") {
            saveButton.disabled = true;
            newUserJSON.email = "";
        }
        else if (/^(\S{1,}@\S{2,}\.\S{2,})$/.test(EMail) == false) {
            saveButton.disabled = true;
            newUserJSON.email = "";
            email.classList.add("error");
            emailError.innerText = "Invalid e-mail address!";
            emailError.style.display = "flex";
        }
        else {
            email.classList.remove("error");
            emailError.style.display = "none";
            newUserJSON.email = EMail;
            VerifyAll();
        }
    }
    function VerifyAll() {
        if (newUserJSON["@id"] != null &&
            newUserJSON.name != null &&
            newUserJSON.email != "") {
            saveButton.disabled = false;
            responseDiv.innerHTML = "";
        }
    }
    function SaveData() {
        if (newUserJSON["@id"] == "")
            delete newUserJSON["@id"];
        if (telephone.value.trim() !== "")
            newUserJSON["telephone"] = telephone.value.trim();
        if (mobilephone.value.trim() !== "")
            newUserJSON["mobilephone"] = mobilephone.value.trim();
        if (homepage.value.trim() !== "")
            newUserJSON["homepage"] = homepage.value.trim();
        if (description.value.trim() !== "")
            newUserJSON["description"] = { "eng": description.value.trim() };
        newUserJSON["organizations"] = [{
                "organizationId": organizationId,
                "accessRights": accessRights.selectedOptions[0].value.toLowerCase()
            }],
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
    HTTPGet("/organizations/" + organizationId, function (status, response) {
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