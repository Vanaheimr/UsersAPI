
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

        if (UserProfileJSON.homepage == "")
            delete (UserProfileJSON.homepage);


        HTTPSet("/users/" + UserProfileJSON["@id"],
                UserProfileJSON,

                (HTTPStatus, ResponseText) => {
                    var responseJSON = JSON.parse(ResponseText);
                    responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully stored updated user profile data.</div>";
                    saveButton.disabled = !AnyChangesMade();
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

    const profileInfos       = document.    getElementById('profileInfos')      as HTMLDivElement;
    const login              = profileInfos.querySelector ('#login')            as HTMLInputElement;
    const username           = profileInfos.querySelector ('#username')         as HTMLInputElement;
    const eMailAddress       = profileInfos.querySelector ('#eMailAddress')     as HTMLInputElement;
    const telephone          = profileInfos.querySelector ('#telephone')        as HTMLInputElement;
    const mobilePhone        = profileInfos.querySelector ('#mobilePhone')      as HTMLInputElement;
    const homepage           = profileInfos.querySelector ('#homepage')         as HTMLInputElement;
    const description        = profileInfos.querySelector ('#userDescription')  as HTMLDivElement;
    const descriptionText    = profileInfos.querySelector ('#description')      as HTMLTextAreaElement;
    //const publicKeyRing      = profileinfos.querySelector ('#publicKeyRing')    as HTMLTextAreaElement;

    const responseDiv        = document.    getElementById("response")          as HTMLDivElement;

    const lowerButtonsDiv    = profileInfos.querySelector   ('#lowerButtons')   as HTMLDivElement;
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

    homepage.onchange        = () => { ToogleSaveButton(); }
    homepage.onkeyup         = () => { ToogleSaveButton(); }

    descriptionText.onchange = () => { ToogleSaveButton(); }
    descriptionText.onkeyup  = () => { ToogleSaveButton(); }

    //publicKeyRing.onchange   = () => { ToogleSaveButton(); }
    //publicKeyRing.onkeyup    = () => { ToogleSaveButton(); }

    saveButton.onclick       = () => { SaveData(); }


    HTTPGet("/users/" + SignInUser,

            (status, response) => {

                UserProfileJSON     = ParseJSON_LD<IUserProfile>(response);

                username.value      = UserProfileJSON.name;
                eMailAddress.value  = UserProfileJSON.email;
                telephone.value     = UserProfileJSON.telephone   != null ? UserProfileJSON.telephone   : "";
                mobilePhone.value   = UserProfileJSON.mobilePhone != null ? UserProfileJSON.mobilePhone : "";
                homepage.value      = UserProfileJSON.homepage    != null ? UserProfileJSON.homepage    : "";
                UpdateI18N(description, UserProfileJSON.description);

                //if (UserProfileJSON.publicKeyRing != null)
                //    publicKeyRing.value  = UserProfileJSON.publicKeyRing;

                //if (UserProfileJSON.publicKeyRing == null)
                //    UserProfileJSON.publicKeyRing = "";

            },

            (statusCode, status, response) => {

            });

}
