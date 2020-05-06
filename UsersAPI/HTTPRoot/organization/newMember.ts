
function StartNewMember() {

    let newUserJSON = {
        "@id":      "",
        "@context": "https://opendata.social/contexts/UsersAPI+json/user",
        "name":     null,
        "email":    ""
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

    const newMemberDiv         = document.getElementById("newMember")      as HTMLDivElement;
    const headlineDiv          = newMemberDiv.querySelector("#headline")   as HTMLDivElement;
    const dataDiv              = newMemberDiv.querySelector('#data')       as HTMLDivElement;

    const accessRights         = dataDiv.querySelector('#accessRights')    as HTMLSelectElement;
    const login                = dataDiv.querySelector('#login')           as HTMLTextAreaElement;
    const loginError           = dataDiv.querySelector('#loginError')      as HTMLDivElement;
    const name                 = dataDiv.querySelector('#name')            as HTMLTextAreaElement;
    const nameError            = dataDiv.querySelector('#nameError')       as HTMLDivElement;
    const email                = dataDiv.querySelector('#email')           as HTMLInputElement;
    const emailError           = dataDiv.querySelector('#emailError')      as HTMLDivElement;
    const telephone            = dataDiv.querySelector('#telephone')       as HTMLInputElement;
    const mobilephone          = dataDiv.querySelector('#mobilephone')     as HTMLInputElement;
    const homepage             = dataDiv.querySelector('#homepage')        as HTMLInputElement;
    const description          = dataDiv.querySelector('#description')     as HTMLTextAreaElement;

    const responseDiv          = document.getElementById("response")       as HTMLDivElement;
    const saveButton           = document.getElementById("saveButton")     as HTMLButtonElement;

    login.onchange     = () => { VerifyLogin(); }
    login.onkeyup      = () => { VerifyLogin(); }
    name.onchange      = () => { VerifyName();  }
    name.onkeyup       = () => { VerifyName();  }
    email.onchange     = () => { VerifyEMail(); }
    email.onkeyup      = () => { VerifyEMail(); }
    saveButton.onclick = () => { SaveData();    }


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

        const UserName = name.value;

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

    function VerifyAll() {

        if (newUserJSON["@id"] != null &&
            newUserJSON.name   != null &&
            newUserJSON.email  != "")
        {
            saveButton.disabled = false;
            responseDiv.innerHTML = "";
        }

    }


    function SaveData() {

        if (newUserJSON["@id"] == "")
            delete newUserJSON["@id"];

        if (telephone.value.trim() !== "")
            newUserJSON["telephone"]    = telephone.  value.trim();

        if (mobilephone.value.trim() !== "")
            newUserJSON["mobilephone"]  = mobilephone.value.trim();

        if (homepage.value.trim() !== "")
            newUserJSON["homepage"]     = homepage.   value.trim();

        if (description.value.trim() !== "")
            newUserJSON["description"]  = { "eng": description.value.trim() };

        newUserJSON["organizations"] = [{
            "organizationId":  organizationId,
            "accessRights":    accessRights.selectedOptions[0].value.toLowerCase()
        }],


        HTTPAdd("/users",
                newUserJSON,

                (statusCode, status, response) => {

                    try
                    {

                        const responseJSON         = JSON.parse(response);
                        responseDiv.style.display  = "block";
                        responseDiv.innerHTML      = "<div class=\"HTTP OK\">Successfully created this new member.</div>";

                        // Redirect to updated organization members view after 2 sec!
                        setTimeout(function () {
                            if (responseJSON["@id"] != null)
                                window.location.href = "../" + responseJSON["@id"];
                        }, 2000);

                    }
                    catch (exception)
                    {
                        responseDiv.style.display  = "block";
                        responseDiv.innerHTML      = "<div class=\"HTTP Error\">Storing the new member failed!<br />" +
                                                         "Exception: " + exception +
                                                     "</div>";
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
