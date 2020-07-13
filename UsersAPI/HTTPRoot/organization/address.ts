
function StartOrganizationGeoLocation() {

    let geoPosition: any;


    function DrawGeoPosition() {

        if (geoPosition != null) {
            map.removeLayer(geoPosition);
            geoPosition = null;
        }

        if (latitude.value != "" && longitude.value != "")
            geoPosition = leaflet.marker([latitude.value, longitude.value]).addTo(map);


        if (geoPosition != null)
            map.panTo([latitude.value, longitude.value]);

    }

    function AnyChangesMade(): boolean {

        // street
        if ((organizationJSON.address !== undefined && organizationJSON.address.street !== undefined ? organizationJSON.address.street : "") !== street.value) {

            if (street.value != "" && street.value.length < 3) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Organizations must have a valid street!</div>";
                return false;
            }

            return true;

        }

        // house number
        if ((organizationJSON.address !== undefined && organizationJSON.address.houseNumber !== undefined ? organizationJSON.address.houseNumber : "") !== houseNumber.value) {
            return true;
        }

        // floor level
        if ((organizationJSON.address !== undefined && organizationJSON.address.floorLevel !== undefined ? organizationJSON.address.floorLevel : "") !== floorLevel.value) {
            return true;
        }

        // postal code
        if ((organizationJSON.address !== undefined && organizationJSON.address.postalCode !== undefined ? organizationJSON.address.postalCode : "") !== postalCode.value) {

            if (postalCode.value != "" && postalCode.value.length < 4) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Organizations must have a valid postal code!</div>";
                return false;
            }

            return true;

        }

        // city
        if ((organizationJSON.address !== undefined && organizationJSON.address.city !== undefined ? firstValue(organizationJSON.address.city) : "") !== city.value) {

            if (city.value != "" && city.value.length < 2) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Organizations must have a valid city!</div>";
                return false;
            }

            return true;

        }

        // country
        if ((organizationJSON.address !== undefined && organizationJSON.address.country !== undefined ? organizationJSON.address.country : "") !== country.value) {

            if (country.value != "" && country.value.length < 2) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Organizations must have a valid country!</div>";
                return false;
            }

            return true;

        }


        // geo location latitude
        if ((organizationJSON.geoLocation !== undefined && organizationJSON.geoLocation.lat !== undefined ? organizationJSON.geoLocation.lat : "") !== latitude.value) {

            if (latitude.value != "" && latitude.value.length < 6) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Organizations must have a valid latitude geo location!</div>";
                return false;
            }

            return true;

        }

        // geo location longitude
        if ((organizationJSON.geoLocation !== undefined && organizationJSON.geoLocation.lng !== undefined ? organizationJSON.geoLocation.lng : "") !== longitude.value) {

            if (longitude.value != "" && longitude.value.length < 6) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Organizations must have a valid longitude geo location!</div>";
                return false;
            }

            return true;

        }


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

        //// name
        //if ((OrganizationJSON.name !== undefined ? firstValue(OrganizationJSON.name) : "") !== name.value) {

        //    if (name.value != "") {

        //        if (OrganizationJSON.name == null)
        //            OrganizationJSON.name = new Object();

        //        OrganizationJSON.name["eng"] = name.value;

        //    }

        //    else
        //        delete OrganizationJSON.name;

        //}


        //// description
        //if ((OrganizationJSON.description !== undefined ? firstValue(OrganizationJSON.description) : "") !== description.value) {

        //    if (description.value != "") {

        //        if (OrganizationJSON.description == null)
        //            OrganizationJSON.description = new Object();

        //        OrganizationJSON.description["eng"] = description.value;

        //    }

        //    else
        //        delete OrganizationJSON.description;

        //}


        //// website
        //if ((OrganizationJSON.website !== undefined ? OrganizationJSON.website : "") !== website.value)
        //    OrganizationJSON.website = website.value;

        //if (OrganizationJSON.website === "")
        //    delete OrganizationJSON.website;


        //// email
        //if ((OrganizationJSON.email !== undefined ? OrganizationJSON.email : "") !== email.value)
        //    OrganizationJSON.email = email.value;

        //if (OrganizationJSON.email === "")
        //    delete OrganizationJSON.email;


        //// telephone
        //if ((OrganizationJSON.telephone !== undefined ? OrganizationJSON.telephone : "") !== telephone.value)
        //    OrganizationJSON.telephone = telephone.value;

        //if (OrganizationJSON.telephone === "")
        //    delete OrganizationJSON.telephone;


        //// address
        //if ((OrganizationJSON.address !== undefined ? OrganizationJSON.address : "") !== address.value)
        //    OrganizationJSON.address = address.value;

        //if (OrganizationJSON.address === "")
        //    delete OrganizationJSON.address;


        // geo location
        //if ((OrganizationJSON.geoLocation !== undefined && OrganizationJSON.geoLocation.lat !== undefined ? OrganizationJSON.geoLocation.lat : "") !== latitude.value)
        //    OrganizationJSON.geoLocation.lat = Number.parseFloat(latitude.value);

        //if (OrganizationJSON.geoLocation.lat === "")
        //    delete OrganizationJSON.geoLocation;


        HTTPSet("/organizations/" + organizationJSON["@id"],
                organizationJSON,

            (HTTPStatus, ResponseText) => {

                try
                {

                    var responseJSON  = ResponseText             != ""   ? JSON.parse(ResponseText) : {};
                    var info          = responseJSON.description != null ? "<br />" + responseJSON.description : "";

                    saveButton.disabled   = !AnyChangesMade();
                    responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully stored updated organization data." + info + "</div>";

                }
                catch (exception)
                {
                    saveButton.disabled = !AnyChangesMade();
                    responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully stored updated organization data.</div>";
                }

            },

            (HTTPStatus, StatusText, ResponseText) => {

                try
                {

                    var responseJSON  = ResponseText             != ""   ? JSON.parse(ResponseText) : {};
                    var info          = responseJSON.description != null ? "<br />" + responseJSON.description : "";

                    responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing organization data failed!" + info + "</div>";

                }
                catch (exception)
                {

                }

            });

    }

    let pathElements              = window.location.pathname.split("/");
    let organizationId            = pathElements[pathElements.length - 2];

    let organizationMenuDiv       = document.getElementById("organizationMenu") as HTMLDivElement;
    let links                     = organizationMenuDiv.querySelectorAll("a");

    for (var i = 0; i < links.length; i++) {

        if (links[i].href.indexOf("00000000") > 0) {
            links[i].href = links[i].href.replace("00000000", organizationId);
        }

    }

    const organizationDiv  = document.       getElementById("organization")   as HTMLDivElement;
    const headlineDiv      = organizationDiv.querySelector ("#headline")      as HTMLDivElement;
    const dataDiv          = organizationDiv.querySelector ('#data')          as HTMLDivElement;

    const street           = dataDiv.        querySelector ('#street')        as HTMLInputElement;
    const houseNumber      = dataDiv.        querySelector ('#houseNumber')   as HTMLInputElement;
    const floorLevel       = dataDiv.        querySelector ('#floorLevel')    as HTMLInputElement;
    const postalCode       = dataDiv.        querySelector ('#postalCode')    as HTMLInputElement;
    const city             = dataDiv.        querySelector ('#city')          as HTMLInputElement;
    const country          = dataDiv.        querySelector ('#country')       as HTMLInputElement;
    const comment          = dataDiv.        querySelector ("#comment")       as HTMLInputElement;

    const latitude         = dataDiv.        querySelector ('#latitude')      as HTMLInputElement;
    const longitude        = dataDiv.        querySelector ('#longitude')     as HTMLInputElement;

    const responseDiv      = document.       getElementById("response")       as HTMLDivElement;

    const lowerButtonsDiv  = organizationDiv.querySelector ('#lowerButtons')  as HTMLDivElement;
    const saveButton       = lowerButtonsDiv.querySelector ("#saveButton")    as HTMLButtonElement;

    street.oninput         = () => { ToogleSaveButton(); }
    houseNumber.oninput    = () => { ToogleSaveButton(); }
    floorLevel.oninput     = () => { ToogleSaveButton(); }
    postalCode.oninput     = () => { ToogleSaveButton(); }
    city.oninput           = () => { ToogleSaveButton(); }
    country.oninput        = () => { ToogleSaveButton(); }

    latitude.oninput       = () => { DrawGeoPosition(); ToogleSaveButton(); }
    longitude.oninput      = () => { DrawGeoPosition(); ToogleSaveButton(); }

    saveButton.onclick     = () => { SaveData(); }


    map.on('click', function (e) {

        const coordinate   = e.latlng.wrap();
        latitude. value  = coordinate.lat;
        longitude.value  = coordinate.lng;

        DrawGeoPosition();
        ToogleSaveButton();

    });


    HTTPGet("/organizations/" + organizationId + "?showMgt&expand=parents,subOrganizations",

            (status, response) => {

                try
                {

                    organizationJSON    = ParseJSON_LD<IOrganization>(response);

                    (headlineDiv.querySelector("#name #language")        as HTMLDivElement).innerText = firstKey  (organizationJSON.name);
                    (headlineDiv.querySelector("#name #I18NText")        as HTMLDivElement).innerText = firstValue(organizationJSON.name);

                    if (organizationJSON.description) {
                        (headlineDiv.querySelector("#description")           as HTMLDivElement).style.display = "block";
                        (headlineDiv.querySelector("#description #language") as HTMLDivElement).innerText = firstKey  (organizationJSON.description);
                        (headlineDiv.querySelector("#description #I18NText") as HTMLDivElement).innerText = firstValue(organizationJSON.description);
                    }

                    // data
                    if (organizationJSON.address !== undefined) {
                        street.value       = organizationJSON.address.street      !== undefined ? organizationJSON.address.street             : "";
                        houseNumber.value  = organizationJSON.address.houseNumber !== undefined ? organizationJSON.address.houseNumber        : "";
                        floorLevel.value   = organizationJSON.address.floorLevel  !== undefined ? organizationJSON.address.floorLevel         : "";
                        postalCode.value   = organizationJSON.address.postalCode  !== undefined ? organizationJSON.address.postalCode         : "";
                        city.value         = organizationJSON.address.city        !== undefined ? firstValue(organizationJSON.address.city)   : "";
                        country.value      = organizationJSON.address.country     !== undefined ? organizationJSON.address.country            : "";
                    }

                    if (organizationJSON.geoLocation !== undefined) {
                        latitude.value     = organizationJSON.geoLocation.lat     !== undefined ? organizationJSON.geoLocation.lat.toString() : "";
                        longitude.value    = organizationJSON.geoLocation.lng     !== undefined ? organizationJSON.geoLocation.lng.toString() : "";
                    }

                }
                catch (exception)
                {
                    responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch organization data from server!</div>";
                }

            },

            (statusCode, status, response) => {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch organization data from server!</div>";
            });

}
