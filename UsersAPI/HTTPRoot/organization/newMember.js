function StartNewMember() {
    let newUserJSON = {
        "@id": "",
        "@context": "https://opendata.social/contexts/UsersAPI/user",
        "username": null,
        "email": "",
        "telephone": "",
        "mobilephone": "",
        "homepage": ""
    };
    const pathElements = window.location.pathname.split("/");
    const organizationId = pathElements[pathElements.length - 2];
    const organizationMenuDiv = document.getElementById("organizationMenu");
    const links = organizationMenuDiv.querySelectorAll("a");
    for (let i = 0; i < links.length; i++) {
        if (links[i].href.indexOf("00000000") > 0) {
            links[i].href = links[i].href.replace("00000000", organizationId);
        }
    }
    const newMemberDiv = document.getElementById("newMember");
    const headlineDiv = newMemberDiv.querySelector("#headline");
    const dataDiv = newMemberDiv.querySelector('#data');
    const accessRights = dataDiv.querySelector('#accessRights');
    const login = dataDiv.querySelector('#login');
    const username = dataDiv.querySelector('#username');
    const eMail = dataDiv.querySelector('#eMail');
    const language = dataDiv.querySelector('#language');
    const telephone = dataDiv.querySelector('#telephone');
    const mobilePhone = dataDiv.querySelector('#mobilePhone');
    const homepage = dataDiv.querySelector('#homepage');
    const description = dataDiv.querySelector('#description');
    const loginError = login.parentElement.querySelector('.validationError');
    const usernameError = username.parentElement.querySelector('.validationError');
    const eMailError = eMail.parentElement.querySelector('.validationError');
    const telephoneError = telephone.parentElement.querySelector('.validationError');
    const mobilePhoneError = mobilePhone.parentElement.querySelector('.validationError');
    const homepageError = homepage.parentElement.querySelector('.validationError');
    const responseDiv = document.getElementById("response");
    const lowerButtonsDiv = newMemberDiv.querySelector('#lowerButtons');
    const saveButton = lowerButtonsDiv.querySelector("#saveButton");
    login.oninput = () => { VerifyLogin(); };
    username.oninput = () => { VerifyName(); };
    eMail.oninput = () => { VerifyEMail(); };
    telephone.oninput = () => { VerifyTelephone(); };
    mobilePhone.oninput = () => { VerifyMobilePhone(); };
    homepage.oninput = () => { VerifyHomepage(); };
    saveButton.onclick = () => { SaveData(); };
    function VerifyLogin() {
        const UserId = login.value.toLowerCase().trim();
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
            (status, response) => {
                newUserJSON["@id"] = null;
                login.classList.add("error");
                loginError.innerText = "This user identification already exists!";
                loginError.style.display = "flex";
                saveButton.disabled = true;
            }, 
            // HTTP Error => Maybe good!
            (HTTPStatus, status, response) => {
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
        const name = username.value;
        if (name == "") {
            saveButton.disabled = true;
            newUserJSON.username = "";
        }
        else if (/^(.{4,})$/.test(name) == false) {
            saveButton.disabled = true;
            newUserJSON.username = "";
            username.classList.add("error");
            usernameError.innerText = "Invalid user name!";
            usernameError.style.display = "flex";
        }
        else {
            username.classList.remove("error");
            usernameError.style.display = "none";
            newUserJSON.username = name;
            VerifyAll();
        }
    }
    function VerifyEMail() {
        const EMail = eMail.value.trim();
        eMail.value = EMail;
        if (EMail == "") {
            saveButton.disabled = true;
            newUserJSON.email = "";
        }
        else if (/^(\S{1,}@\S{2,}\.\S{2,})$/.test(EMail) == false) {
            saveButton.disabled = true;
            newUserJSON.email = "";
            eMail.classList.add("error");
            eMailError.innerText = "Invalid e-mail address!";
            eMailError.style.display = "flex";
        }
        else {
            eMail.classList.remove("error");
            eMailError.style.display = "none";
            newUserJSON.email = EMail;
            VerifyAll();
        }
    }
    function VerifyTelephone() {
        const Telephone = telephone.value.trim();
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
    function VerifyMobilePhone() {
        const MobilePhone = mobilePhone.value.trim();
        mobilePhone.value = MobilePhone;
        if (MobilePhone == "") {
            mobilePhone.classList.remove("error");
            mobilePhoneError.style.display = "none";
            newUserJSON.mobilephone = "";
        }
        else if (/^(\+?[0-9\ \-\/]{5,30})$/.test(MobilePhone) == false) {
            saveButton.disabled = true;
            newUserJSON.mobilephone = null;
            mobilePhone.classList.add("error");
            mobilePhoneError.innerText = "Invalid mobile phone number!";
            mobilePhoneError.style.display = "flex";
        }
        else {
            mobilePhone.classList.remove("error");
            mobilePhoneError.style.display = "none";
            newUserJSON.mobilephone = MobilePhone;
            VerifyAll();
        }
    }
    function VerifyHomepage() {
        const Homepage = homepage.value.trim();
        homepage.value = Homepage;
        if (Homepage == "") {
            homepage.classList.remove("error");
            homepageError.style.display = "none";
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
            newUserJSON["description"] = { "en": description.value.trim() };
        newUserJSON["accessRights"] = [{
                "organizationId": organizationId,
                "accessRight": accessRights.selectedOptions[0].value.toLowerCase()
            }],
            HTTPAdd("/users", newUserJSON, (statusCode, status, response) => {
                responseDiv.style.display = "block";
                responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully created this new member.</div>";
                try {
                    const responseJSON = JSON.parse(response);
                    const userId = responseJSON["@id"];
                    // Redirect to updated organization members view after 2 sec!
                    if (userId != null && userId != "") {
                        setTimeout(function () {
                            window.location.href = "../../users/" + userId;
                        }, 2000);
                    }
                }
                catch (exception) {
                    console.debug("Could not parse 'new member'-JSON result: " + exception);
                }
            }, (statusCode, status, response) => {
                responseDiv.style.display = "block";
                try {
                    const responseJSON = JSON.parse(response);
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
    HTTPGet("/organizations/" + organizationId, (status, response) => {
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
//# sourceMappingURL=newMember.js.map