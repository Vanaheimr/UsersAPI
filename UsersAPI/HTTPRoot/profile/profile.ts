
function StartProfile() {

    function AnyChangesMade(): boolean {

        // name
        if (UserProfileJSON.name != username.value) {

            if (username.value == "") {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a username!</div>";
                return false;
            }

            return true;

        }

        // email
        if (UserProfileJSON.email != eMailAddress.value) {

            if (eMailAddress.value == "" || eMailAddress.value.length < 6 || eMailAddress.value.indexOf("@") < 1 || eMailAddress.value.indexOf(".") < 4) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid e-mail address!</div>";
                return false;
            }

            return true;

        }

        // telephone
        if (UserProfileJSON.telephone != telephone.value) {

            if (telephone.value != "" && telephone.value.length < 6) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid telephone number!</div>";
                return false;
            }

            return true;

        }

        // mobilePhone
        if (UserProfileJSON.mobilePhone != mobilePhone.value) {

            if (mobilePhone.value != "" && mobilePhone.value.length < 6) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid mobile phone number!</div>";
                return false;
            }

            return true;

        }

        // telegram
        if (UserProfileJSON.telegram != telegram.value) {

            //ToDo: Parse as number!
            if (telegram.value != "" && telegram.value.length < 4) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid telegram chat identification!</div>";
                return false;
            }

            return true;

        }

        // homepage
        if (UserProfileJSON.homepage != homepage.value) {

            if (homepage.value != "" && homepage.value.length < 4) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Users must have a valid homepage/Internet URL!</div>";
                return false;
            }

            return true;

        }

        // description
        let latestDescription  = UserProfileJSON.description != null ? firstValue(UserProfileJSON.description) : "";
        let newDescription     = descriptionText.value;

        if (latestDescription != newDescription)
            return true;

        //// publicKeyRing
        //if ((UserProfileJSON.publicKeyRing != null ? UserProfileJSON.publicKeyRing : "") != publicKeyRing.value)
        //    return true;


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

        // mobilePhone
        if (UserProfileJSON.mobilePhone != mobilePhone.value)
            UserProfileJSON.mobilePhone  = mobilePhone.value;

        // mobilePhone
        if (UserProfileJSON.telegram    != telegram.value)
            UserProfileJSON.telegram     = telegram.value;

        // homepage
        if (UserProfileJSON.homepage    != homepage.value)
            UserProfileJSON.homepage     = homepage.value;

        // description
        let latestDescription            = UserProfileJSON.description != null ? firstValue(UserProfileJSON.description) : "";
        let newDescription               = descriptionText.value;

        if (latestDescription != newDescription) {

            if (newDescription != "") {

                if (UserProfileJSON.description == null)
                    UserProfileJSON.description = new Object();

                UserProfileJSON.description["eng"] = newDescription;
            }

            else {
                delete UserProfileJSON.description;
            }

        }

        //// publicKeyRing
        //if ((UserProfileJSON.publicKeyRing != null ? UserProfileJSON.publicKeyRing : "") != publicKeyRing.value)
        //    UserProfileJSON.publicKeyRing = publicKeyRing.value;

        if (UserProfileJSON.telephone   == "")
            delete (UserProfileJSON.telephone);

        if (UserProfileJSON.mobilePhone == "")
            delete (UserProfileJSON.mobilePhone);

        if (UserProfileJSON.telegram == "")
            delete (UserProfileJSON.telegram);

        if (UserProfileJSON.homepage == "")
            delete (UserProfileJSON.homepage);


        HTTPSet("/users/" + UserProfileJSON["@id"],
                UserProfileJSON,

                (HTTPStatus, ResponseText) => {
                    try
                    {
                        var responseJSON = JSON.parse(ResponseText);
                        responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully stored updated user profile data.</div>";
                        saveButton.disabled = !AnyChangesMade();
                    }
                    catch (exception)
                    {
                        responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing user profile data failed:<br />" + exception + "</div>";
                    }
                },

                (HTTPStatus, StatusText, ResponseText) => {

                    var responseJSON = { "description": "HTTP Error " + HTTPStatus + " - " + StatusText + "!" };

                    if (ResponseText != null && ResponseText != "") {
                        try
                        {
                            responseJSON = JSON.parse(ResponseText);
                        }
                        catch { }
                    }

                    responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing user profile data failed!" +
                                                (responseJSON.description != null ? "<br />" + responseJSON.description : "") +
                                            "</div>";

                });

    }

    const userProfile        = document.   getElementById('userProfile')       as HTMLDivElement;

    const data               = userProfile.querySelector ('#data')             as HTMLInputElement;

    const login              = data.       querySelector ('#login')            as HTMLInputElement;
    const username           = data.       querySelector ('#username')         as HTMLInputElement;
    const eMailAddress       = data.       querySelector ('#eMailAddress')     as HTMLInputElement;
    const telephone          = data.       querySelector ('#telephone')        as HTMLInputElement;
    const mobilePhone        = data.       querySelector ('#mobilePhone')      as HTMLInputElement;
    const telegram           = data.       querySelector ('#telegram')         as HTMLInputElement;
    const homepage           = data.       querySelector ('#homepage')         as HTMLInputElement;
    const description        = data.       querySelector ('#userDescription')  as HTMLDivElement;
    const descriptionText    = data.       querySelector ('#description')      as HTMLTextAreaElement;
    //const publicKeyRing      = data.       querySelector ('#publicKeyRing')    as HTMLTextAreaElement;

    const responseDiv        = document.    getElementById("response")          as HTMLDivElement;

    const lowerButtonsDiv    = userProfile.querySelector   ('#lowerButtons')   as HTMLDivElement;
    const saveButton         = lowerButtonsDiv.querySelector("#saveButton")     as HTMLButtonElement;

    checkSignedIn(false);
    login.value              = SignInUser;
    username.value           = Username;
    eMailAddress.value       = UserEMail;

    username.onchange        = () => { ToogleSaveButton(); }
    username.onkeyup         = () => { ToogleSaveButton(); }

    eMailAddress.onchange    = () => { ToogleSaveButton(); }
    eMailAddress.onkeyup     = () => { ToogleSaveButton(); }

    telephone.onchange       = () => { ToogleSaveButton(); }
    telephone.onkeyup        = () => { ToogleSaveButton(); }

    mobilePhone.onchange     = () => { ToogleSaveButton(); }
    mobilePhone.onkeyup      = () => { ToogleSaveButton(); }

    telegram.onchange        = () => { ToogleSaveButton(); }
    telegram.onkeyup         = () => { ToogleSaveButton(); }

    homepage.onchange        = () => { ToogleSaveButton(); }
    homepage.onkeyup         = () => { ToogleSaveButton(); }

    descriptionText.onchange = () => { ToogleSaveButton(); }
    descriptionText.onkeyup  = () => { ToogleSaveButton(); }

    //publicKeyRing.onchange   = () => { ToogleSaveButton(); }
    //publicKeyRing.onkeyup    = () => { ToogleSaveButton(); }

    saveButton.onclick       = () => { SaveData(); }


    HTTPGet("/users/" + SignInUser,

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

                    //if (UserProfileJSON.publicKeyRing != null)
                    //    publicKeyRing.value  = UserProfileJSON.publicKeyRing;

                    //if (UserProfileJSON.publicKeyRing == null)
                    //    UserProfileJSON.publicKeyRing = "";

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
