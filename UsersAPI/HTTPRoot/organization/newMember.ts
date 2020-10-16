
function StartNewMember() {

    let newUserJSON = {
        "@id":          "",
        "@context":     "https://opendata.social/contexts/UsersAPI/user",
        "username":      null,
        "email":        "",
        "telephone":    "",
        "mobilephone":  "",
        "homepage":     ""
    };

    const pathElements         = window.location.pathname.split("/");
    const organizationId       = pathElements[pathElements.length - 2];

    const organizationMenuDiv  = document.getElementById("organizationMenu")  as HTMLDivElement;
    const links                = organizationMenuDiv.querySelectorAll("a");
    for (let i = 0; i < links.length; i++) {

        if (links[i].href.indexOf("00000000") > 0) {
            links[i].href = links[i].href.replace("00000000", organizationId);
        }

    }

    const newMemberDiv      = document.getElementById("newMember")                          as HTMLDivElement;
    const headlineDiv       = newMemberDiv.querySelector("#headline")                       as HTMLDivElement;

    const dataDiv           = newMemberDiv.querySelector('#data')                           as HTMLDivElement;
    const accessRights      = dataDiv.querySelector('#accessRights')                        as HTMLSelectElement;
    const login             = dataDiv.querySelector('#login')                               as HTMLTextAreaElement;
    const Username          = dataDiv.querySelector('#username')                            as HTMLTextAreaElement;
    const email             = dataDiv.querySelector('#email')                               as HTMLInputElement;
    const language          = dataDiv.querySelector('#language')                            as HTMLSelectElement;
    const telephone         = dataDiv.querySelector('#telephone')                           as HTMLInputElement;
    const mobilephone       = dataDiv.querySelector('#mobilephone')                         as HTMLInputElement;
    const homepage          = dataDiv.querySelector('#homepage')                            as HTMLInputElement;
    const description       = dataDiv.querySelector('#description')                         as HTMLTextAreaElement;

    const loginError        = login.      parentElement.querySelector('.validationError')   as HTMLDivElement;
    const UsernameError     = Username.   parentElement.querySelector('.validationError')   as HTMLDivElement;
    const emailError        = email.      parentElement.querySelector('.validationError')   as HTMLDivElement;
    const telephoneError    = telephone.  parentElement.querySelector('.validationError')   as HTMLDivElement;
    const mobilephoneError  = mobilephone.parentElement.querySelector('.validationError')   as HTMLDivElement;
    const homepageError     = homepage.   parentElement.querySelector('.validationError')   as HTMLDivElement;

    const responseDiv       = document.getElementById("response")                           as HTMLDivElement;

    const lowerButtonsDiv   = newMemberDiv.querySelector   ('#lowerButtons')                as HTMLDivElement;
    const saveButton        = lowerButtonsDiv.querySelector("#saveButton")                  as HTMLButtonElement;

    login.oninput           = () => { VerifyLogin();       }
    Username.oninput        = () => { VerifyName();        }
    email.oninput           = () => { VerifyEMail();       }
    telephone.oninput       = () => { VerifyTelephone();   }
    mobilephone.oninput     = () => { VerifyMobilephone(); }
    homepage.oninput        = () => { VerifyHomepage();    }
    saveButton.onclick      = () => { SaveData();          }


    function VerifyLogin() {

        const UserId = login.value.toLowerCase().trim();
        login.value = UserId;

        if (UserId == "")
        {
            newUserJSON["@id"] = "";
            login.classList.remove("error");
            loginError.style.display = "none";
            VerifyAll();
        }

        else if (/^([a-zA-Z0-9]{3,})$/.test(UserId) == false) {
            saveButton.disabled = true;
            newUserJSON["@id"] = null;
            login.classList.add("error");
            loginError.innerText     = "Invalid user identification!";
            loginError.style.display = "flex";
        }

        else if (newUserJSON["@id"] != UserId) {

            responseDiv.innerHTML = "";

            HTTPGet("/users/" + UserId,

                    // HTTP OK    => bad!
                    (status, response) => {
                        newUserJSON["@id"] = null;
                        login.classList.add("error");
                        loginError.innerText     = "This user identification already exists!";
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

        const name = Username.value;

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

        const EMail = email.value.trim();
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
            telephoneError.innerText     = "Invalid telephone number!";
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

        const Mobilephone = mobilephone.value.trim();
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
            mobilephoneError.innerText     = "Invalid mobile phone number!";
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

        const Homepage = homepage.value.trim();
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

        if (newUserJSON["@id"]      != null &&
            newUserJSON.username    != ""   &&
            newUserJSON.email       != ""   &&
            newUserJSON.telephone   != null &&
            newUserJSON.mobilephone != null &&
            newUserJSON.homepage    != null)
        {
            saveButton.disabled    = false;
            responseDiv.innerHTML  = "";
        }

    }


    function SaveData() {

        if (newUserJSON["@id"] == "")
            delete newUserJSON["@id"];

        newUserJSON["language"]         = language.selectedOptions[0].value;

        if (newUserJSON.telephone   == "")
            delete newUserJSON.telephone;

        if (newUserJSON.mobilephone == "")
            delete newUserJSON.mobilephone;

        if (newUserJSON.homepage    == "")
            delete newUserJSON.homepage;

        if (description.value.trim() !== "")
            newUserJSON["description"] = { "eng": description.value.trim() };

        newUserJSON["accessRights"] = [{
            "organizationId":  organizationId,
            "accessRight":     accessRights.selectedOptions[0].value.toLowerCase()
        }],


        HTTPAdd("/users",
                newUserJSON,

                (statusCode, status, response) => {

                    responseDiv.style.display  = "block";
                    responseDiv.innerHTML      = "<div class=\"HTTP OK\">Successfully created this new member.</div>";

                    try
                    {

                        const responseJSON  = JSON.parse(response);
                        const userId        = responseJSON["@id"];

                        // Redirect to updated organization members view after 2 sec!
                        if (userId != null && userId != "") {
                            setTimeout(function () {
                                window.location.href = "../../users/" + userId;
                            }, 2000);
                        }

                    }
                    catch (exception)
                    {
                        console.debug("Could not parse 'new member'-JSON result: " + exception);
                    }

                },

                (statusCode, status, response) => {

                    responseDiv.style.display = "block";

                    try
                    {

                        const responseJSON     = JSON.parse(response);
                        responseDiv.innerHTML  = "<div class=\"HTTP Error\">Storing the new member failed!<br />" +
                                                     (responseJSON.description != null
                                                          ? responseJSON.description
                                                          : "HTTP Error " + statusCode + " - " + status) +
                                                 "</div>";

                    }
                    catch (exception)
                    {
                        responseDiv.innerHTML  = "<div class=\"HTTP Error\">Storing the new member failed!<br />" +
                                                     "Exception: " + exception +
                                                 "</div>";
                    }

                });

    }


    HTTPGet("/organizations/" + organizationId,

            (status, response) => {

                try
                {

                    organizationJSON    = ParseJSON_LD<IOrganization>(response);

                    (headlineDiv.querySelector("#name #language")        as HTMLDivElement).innerText = firstKey  (organizationJSON.name);
                    (headlineDiv.querySelector("#name #I18NText")        as HTMLDivElement).innerText = firstValue(organizationJSON.name);

                    if (organizationJSON.description != null && firstValue(organizationJSON.description) != null) {
                        (headlineDiv.querySelector("#description")           as HTMLDivElement).style.display = "block";
                        (headlineDiv.querySelector("#description #language") as HTMLDivElement).innerText = firstKey  (organizationJSON.description);
                        (headlineDiv.querySelector("#description #I18NText") as HTMLDivElement).innerText = firstValue(organizationJSON.description);
                    }

                }
                catch (exception)
                {
                    responseDiv.innerHTML  = "<div class=\"HTTP Error\">Could not fetch organization data from server!<br />" +
                                                 "Exception: " + exception +
                                             "</div>";
                }

            },

            (statusCode, status, response) => {

                try
                {

                    const responseJSON     = JSON.parse(response);
                    responseDiv.innerHTML  = "<div class=\"HTTP Error\">Could not fetch organization data from server!<br />" +
                                                 (responseJSON.description != null
                                                      ? responseJSON.description
                                                      : "HTTP Error " + statusCode + " - " + status) +
                                             "</div>";

                }
                catch (exception)
                {
                    responseDiv.innerHTML  = "<div class=\"HTTP Error\">Could not fetch organization data from server!<br />" +
                                                 "Exception: " + exception +
                                             "</div>";
                }

            });

}
