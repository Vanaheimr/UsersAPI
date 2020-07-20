
function StartUser() {

    function AnyChangesMade(): boolean {

        // name
        if ((UserProfileJSON.name !== undefined ? UserProfileJSON.name : "") !== username.value) {

            if (username.value == "") {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a username!</div>";
                return false;
            }

            return true;

        }


        // email
        if ((UserProfileJSON.email !== undefined ? UserProfileJSON.email : "") !== eMailAddress.value) {

            if (eMailAddress.value == "" || eMailAddress.value.length < 6 || eMailAddress.value.indexOf("@") < 1 || eMailAddress.value.indexOf(".") < 4) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid e-mail address!</div>";
                return false;
            }

            return true;

        }


        // telephone
        if ((UserProfileJSON.telephone !== undefined ? UserProfileJSON.telephone : "") !== telephone.value) {

            if (telephone.value != "" && telephone.value.length < 6) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid telephone number!</div>";
                return false;
            }

            return true;

        }


        // mobilePhone
        if ((UserProfileJSON.mobilePhone !== undefined ? UserProfileJSON.mobilePhone : "") !== mobilePhone.value) {

            if (mobilePhone.value != "" && mobilePhone.value.length < 6) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid mobile phone number!</div>";
                return false;
            }

            return true;

        }


        // homepage
        if ((UserProfileJSON.homepage !== undefined ? UserProfileJSON.homepage : "") !== homepage.value) {

            if (homepage.value != "" && homepage.value.length < 4) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid homepage/Internet URL!</div>";
                return false;
            }

            return true;

        }


        // description
        if ((UserProfileJSON.description != null ? firstValue(UserProfileJSON.description) : "") !== descriptionText.value)
            return true;


        responseDiv.innerHTML = "";
        return false;

    }

    function ToogleSaveButton(): boolean {

        var changesDetected = AnyChangesMade();

        saveButton.disabled = !changesDetected;

        if (changesDetected)
            responseDiv.innerHTML = "";

        return changesDetected;

    }

    function SaveData() {

        // name
        if (UserProfileJSON.name        != username.value)
            UserProfileJSON.name         = username.value;


        // email
        if (UserProfileJSON.email       != eMailAddress.value)
            UserProfileJSON.email        = eMailAddress.value;


        // telephone
        if (UserProfileJSON.telephone   != telephone.value)
            UserProfileJSON.telephone    = telephone.value;

        if (UserProfileJSON.telephone == "")
            delete (UserProfileJSON.telephone);


        // mobilePhone
        if (UserProfileJSON.mobilePhone != mobilePhone.value)
            UserProfileJSON.mobilePhone  = mobilePhone.value;

        if (UserProfileJSON.mobilePhone == "")
            delete (UserProfileJSON.mobilePhone);


        // homepage
        if (UserProfileJSON.homepage    != homepage.value)
            UserProfileJSON.homepage     = homepage.value;

        if (UserProfileJSON.homepage == "")
            delete (UserProfileJSON.homepage);


        // description
        let latestDescription            = UserProfileJSON.description != null ? firstValue(UserProfileJSON.description) : "";
        let newDescription               = descriptionText.value;

        if (latestDescription != newDescription) {

            if (newDescription != "") {

                if (UserProfileJSON.description == null)
                    UserProfileJSON.description = new Object();

                UserProfileJSON.description["eng"] = newDescription;
            }

            else
                delete UserProfileJSON.description;

        }


        HTTPSet("/users/" + UserProfileJSON["@id"],
                UserProfileJSON,

                (HTTPStatus, ResponseText) => {
                    var responseJSON = JSON.parse(ResponseText);
                    responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully stored updated user profile data.</div>";
                    //saveButton.disabled = !AnyChangesMade();
                },

                (HTTPStatus, StatusText, ResponseText) => {

                    var responseJSON = { "description": "HTTP Error " + HTTPStatus + " - " + StatusText + "!" };

                    if (ResponseText != null && ResponseText != "") {
                        try {
                            responseJSON = JSON.parse(ResponseText);
                        }
                        catch { }
                    }

                    responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing user profile data failed!" + (responseJSON.description != null ? responseJSON.description : "") + "</div>";

                });

    }

    function ImpersonateUser(newUserId) {

        HTTPImpersonate("/users/" + newUserId,

            (status, response) => {
                window.location.reload(true);
            },

            (statusCode, status, response) => {
                alert("Not allowed!");
            });

    }


    const pathElements       = window.location.pathname.split("/");
    const userId             = pathElements[pathElements.length - 1];

    const userProfile        = document.   getElementById('userProfile')         as HTMLDivElement;

    const impersonateButton  = document.   getElementById("impersonateButton")   as HTMLButtonElement;

    const data               = userProfile.querySelector ('#data')               as HTMLInputElement;
    const login              = data.       querySelector ('#login')              as HTMLInputElement;
    const username           = data.       querySelector ('#username')           as HTMLInputElement;
    const eMailAddress       = data.       querySelector ('#eMailAddress')       as HTMLInputElement;
    const telephone          = data.       querySelector ('#telephone')          as HTMLInputElement;
    const mobilePhone        = data.       querySelector ('#mobilePhone')        as HTMLInputElement;
    const telegram           = data.       querySelector ('#telegram')           as HTMLInputElement;
    const homepage           = data.       querySelector ('#homepage')           as HTMLInputElement;
    const description        = data.       querySelector ('#userDescription')    as HTMLDivElement;
    const descriptionText    = data.       querySelector ('#description')        as HTMLTextAreaElement;

    const responseDiv        = document.   getElementById("response")            as HTMLDivElement;

    const saveButton         = document.   getElementById("saveButton")          as HTMLButtonElement;

    login.value              = userId;


    HTTPGet("/users/" + userId,

            (status, response) => {

                try
                {

                    UserProfileJSON     = ParseJSON_LD<IUserProfile>(response);

                    username.value      = UserProfileJSON.name;
                    eMailAddress.value  = UserProfileJSON.email;
                    telephone.value     = UserProfileJSON.telephone   != null ? UserProfileJSON.telephone   : "";
                    mobilePhone.value   = UserProfileJSON.mobilePhone != null ? UserProfileJSON.mobilePhone : "";
                    telegram.value      = UserProfileJSON.telegram    != null ? UserProfileJSON.telegram    : "";
                    homepage.value      = UserProfileJSON.homepage    != null ? UserProfileJSON.homepage    : "";
                    UpdateI18N(description, UserProfileJSON.description);

                    if (UserProfileJSON["youCanEdit"])
                    {

                        username.readOnly         = false;
                        username.onchange         = () => { ToogleSaveButton(); }
                        username.onkeyup          = () => { ToogleSaveButton(); }

                        eMailAddress.readOnly     = false;
                        eMailAddress.onchange     = () => { ToogleSaveButton(); }
                        eMailAddress.onkeyup      = () => { ToogleSaveButton(); }

                        telephone.readOnly        = false;
                        telephone.onchange        = () => { ToogleSaveButton(); }
                        telephone.onkeyup         = () => { ToogleSaveButton(); }

                        mobilePhone.readOnly      = false;
                        mobilePhone.onchange      = () => { ToogleSaveButton(); }
                        mobilePhone.onkeyup       = () => { ToogleSaveButton(); }

                        telegram.readOnly         = false;
                        telegram.onchange         = () => { ToogleSaveButton(); }
                        telegram.onkeyup          = () => { ToogleSaveButton(); }

                        homepage.readOnly         = false;
                        homepage.onchange         = () => { ToogleSaveButton(); }
                        homepage.onkeyup          = () => { ToogleSaveButton(); }

                        descriptionText.readOnly  = false;
                        descriptionText.onchange  = () => { ToogleSaveButton(); }
                        descriptionText.onkeyup   = () => { ToogleSaveButton(); }


                        saveButton.style.display  = "block";
                        saveButton.onclick        = () => {
                            SaveData();
                        }

                        impersonateButton.disabled       = false;
                        impersonateButton.style.display  = "block";
                        impersonateButton.onclick        = () => {
                            ImpersonateUser(userId);
                        }

                    }

                }
                catch (exception)
                {
                    responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch user data from server:<br />" + exception + "</div>";
                }

            },

            (statusCode, status, response) => {

                try
                {
                    responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch user data from server!</div>";
                }
                catch (exception) {
                    responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch user data from server:<br />" + exception + "</div>";
                }

            });

}
