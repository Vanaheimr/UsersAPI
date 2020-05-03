
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

            (HTTPStatus, ResponseText) => {
                window.location.reload(true);
            },

            (HTTPStatus, StatusText, ResponseText) => {
                alert("Not allowed!");
            });

    }


    const pathElements       = window.location.pathname.split("/");
    const userId             = pathElements[pathElements.length - 1];

    const profileinfos       = document.    getElementById('profileinfos')        as HTMLDivElement;
    const login              = profileinfos.querySelector ('#login')              as HTMLInputElement;
    const username           = profileinfos.querySelector ('#username')           as HTMLInputElement;
    const eMailAddress       = profileinfos.querySelector ('#eMailAddress')       as HTMLInputElement;
    const telephone          = profileinfos.querySelector ('#telephone')          as HTMLInputElement;
    const mobilePhone        = profileinfos.querySelector ('#mobilePhone')        as HTMLInputElement;
    const homepage           = profileinfos.querySelector ('#homepage')           as HTMLInputElement;
    const description        = profileinfos.querySelector ('#userDescription')    as HTMLDivElement;
    const descriptionText    = profileinfos.querySelector ('#description')        as HTMLTextAreaElement;

    const responseDiv        = document.    getElementById("response")            as HTMLDivElement;
    const impersonateButton  = document.    getElementById("impersonateButton")   as HTMLButtonElement;
    const saveButton         = document.    getElementById("saveButton")          as HTMLButtonElement;

    login.value              = userId;


    HTTPGet("/users/" + userId,

            (HTTPStatus, ResponseText) => {

                UserProfileJSON     = ParseJSON_LD<IUserProfile>(ResponseText);

                username.value      = UserProfileJSON.name;
                eMailAddress.value  = UserProfileJSON.email;
                telephone.value     = UserProfileJSON.telephone   != null ? UserProfileJSON.telephone   : "";
                mobilePhone.value   = UserProfileJSON.mobilePhone != null ? UserProfileJSON.mobilePhone : "";
                homepage.value      = UserProfileJSON.homepage    != null ? UserProfileJSON.homepage    : "";
                UpdateI18N(description, UserProfileJSON.description);

                if (UserProfileJSON["youCanEdit"])
                {

                    username.readOnly         = false;

                    username.onchange         = () => {
                        ToogleSaveButton();
                    }

                    username.onkeyup          = () => {
                        ToogleSaveButton();
                    }


                    eMailAddress.readOnly     = false;

                    eMailAddress.onchange     = () => {
                        ToogleSaveButton();
                    }

                    eMailAddress.onkeyup      = () => {
                        ToogleSaveButton();
                    }


                    telephone.readOnly        = false;

                    telephone.onchange        = () => {
                        ToogleSaveButton();
                    }

                    telephone.onkeyup         = () => {
                        ToogleSaveButton();
                    }


                    mobilePhone.readOnly      = false;

                    mobilePhone.onchange      = () => {
                        ToogleSaveButton();
                    }

                    mobilePhone.onkeyup       = () => {
                        ToogleSaveButton();
                    }


                    homepage.readOnly         = false;

                    homepage.onchange         = () => {
                        ToogleSaveButton();
                    }

                    homepage.onkeyup          = () => {
                        ToogleSaveButton();
                    }


                    descriptionText.readOnly  = false;

                    descriptionText.onchange  = () => {
                        ToogleSaveButton();
                    }

                    descriptionText.onkeyup   = () => {
                        ToogleSaveButton();
                    }


                    saveButton.style.display  = "block";
                    saveButton.onclick        = () => {
                        SaveData();
                    }


                    impersonateButton.disabled       = false;
                    impersonateButton.style.display  = "block";
                    impersonateButton.onclick        = function (this: HTMLElement, ev: Event) {
                        ImpersonateUser(userId);
                    }

                }

            },

            (HTTPStatus, StatusText, ResponseText) => {

            });

}
