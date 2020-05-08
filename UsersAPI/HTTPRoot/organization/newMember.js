function StartNewMember() {
    var newUserJSON = {
        "@id": "",
        "@context": "https://opendata.social/contexts/UsersAPI+json/user",
        "username": null,
        "email": "",
        "telephone": "",
        "mobilephone": "",
        "homepage": ""
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
    var Username = dataDiv.querySelector('#username');
    var email = dataDiv.querySelector('#email');
    var language = dataDiv.querySelector('#language');
    var telephone = dataDiv.querySelector('#telephone');
    var mobilephone = dataDiv.querySelector('#mobilephone');
    var homepage = dataDiv.querySelector('#homepage');
    var description = dataDiv.querySelector('#description');
    var loginError = login.parentElement.querySelector('.validationError');
    var UsernameError = Username.parentElement.querySelector('.validationError');
    var emailError = email.parentElement.querySelector('.validationError');
    var telephoneError = telephone.parentElement.querySelector('.validationError');
    var mobilephoneError = mobilephone.parentElement.querySelector('.validationError');
    var homepageError = homepage.parentElement.querySelector('.validationError');
    var responseDiv = document.getElementById("response");
    var saveButton = document.getElementById("saveButton");
    login.oninput = function () { VerifyLogin(); };
    Username.oninput = function () { VerifyName(); };
    email.oninput = function () { VerifyEMail(); };
    telephone.oninput = function () { VerifyTelephone(); };
    mobilephone.oninput = function () { VerifyMobilephone(); };
    homepage.oninput = function () { VerifyHomepage(); };
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
        var name = Username.value;
        if (name == "") {
            saveButton.disabled = true;
            newUserJSON.username = "";
        }
        else if (/^(.{4,})$/.test(name) == false) {
            saveButton.disabled = true;
            newUserJSON.username = "";
            Username.classList.add("error");
            UsernameError.innerText = "Invalid user name!";
            UsernameError.style.display = "flex";
        }
        else {
            Username.classList.remove("error");
            UsernameError.style.display = "none";
            newUserJSON.username = name;
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
    function VerifyTelephone() {
        var Telephone = telephone.value.trim();
        telephone.value = Telephone;
        if (Telephone == "") {
            telephone.classList.remove("error");
            telephoneError.style.display = "none";
            newUserJSON.telephone = "";
        }
        else if (/^(\+?[0-9\ \-\/]{5,30})$/.test(Telephone) == false) {
            saveButton.disabled = true;
            newUserJSON.telephone = null;
            telephone.classList.add("error");
            telephoneError.innerText = "Invalid telephone number!";
            telephoneError.style.display = "flex";
        }
        else {
            telephone.classList.remove("error");
            telephoneError.style.display = "none";
            newUserJSON.telephone = Telephone;
            VerifyAll();
        }
    }
    function VerifyMobilephone() {
        var Mobilephone = mobilephone.value.trim();
        mobilephone.value = Mobilephone;
        if (Mobilephone == "") {
            mobilephone.classList.remove("error");
            mobilephoneError.style.display = "none";
            newUserJSON.mobilephone = "";
        }
        else if (/^(\+?[0-9\ \-\/]{5,30})$/.test(Mobilephone) == false) {
            saveButton.disabled = true;
            newUserJSON.mobilephone = null;
            mobilephone.classList.add("error");
            mobilephoneError.innerText = "Invalid mobile phone number!";
            mobilephoneError.style.display = "flex";
        }
        else {
            mobilephone.classList.remove("error");
            mobilephoneError.style.display = "none";
            newUserJSON.mobilephone = Mobilephone;
            VerifyAll();
        }
    }
    function VerifyHomepage() {
        var Homepage = homepage.value.trim();
        homepage.value = Homepage;
        if (Homepage == "") {
            newUserJSON.homepage = "";
        }
        else if (/^(http:\/\/|https:\/\/)(\S{2,}\.\S{2,})$/.test(Homepage) == false) {
            saveButton.disabled = true;
            newUserJSON.homepage = null;
            homepage.classList.add("error");
            homepageError.innerText = "Invalid homepage URL!";
            homepageError.style.display = "flex";
        }
        else {
            homepage.classList.remove("error");
            homepageError.style.display = "none";
            newUserJSON.homepage = Homepage;
            VerifyAll();
        }
    }
    function VerifyAll() {
        if (newUserJSON["@id"] != null &&
            newUserJSON.username != "" &&
            newUserJSON.email != "" &&
            newUserJSON.telephone != null &&
            newUserJSON.mobilephone != null &&
            newUserJSON.homepage != null) {
            saveButton.disabled = false;
            responseDiv.innerHTML = "";
        }
    }
    function SaveData() {
        if (newUserJSON["@id"] == "")
            delete newUserJSON["@id"];
        newUserJSON["language"] = language.selectedOptions[0].value;
        if (newUserJSON.telephone == "")
            delete newUserJSON.telephone;
        if (newUserJSON.mobilephone == "")
            delete newUserJSON.mobilephone;
        if (newUserJSON.homepage == "")
            delete newUserJSON.homepage;
        if (description.value.trim() !== "")
            newUserJSON["description"] = { "eng": description.value.trim() };
        newUserJSON["accessRights"] = [{
                "organizationId": organizationId,
                "accessRight": accessRights.selectedOptions[0].value.toLowerCase()
            }],
            HTTPAdd("/users", newUserJSON, function (statusCode, status, response) {
                responseDiv.style.display = "block";
                responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully created this new member.</div>";
                try {
                    var responseJSON = JSON.parse(response);
                    var userId_1 = responseJSON["@id"];
                    // Redirect to updated organization members view after 2 sec!
                    if (userId_1 != null && userId_1 != "") {
                        setTimeout(function () {
                            window.location.href = "../../users/" + userId_1;
                        }, 2000);
                    }
                }
                catch (exception) {
                    console.debug("Could not parse 'new member'-JSON result: " + exception);
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