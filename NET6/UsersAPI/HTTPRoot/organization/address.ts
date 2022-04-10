
function StartOrganizationGeoLocation() {

    let geoMarker:  any;
    let _latitude:  number;
    let _longitude: number;


    function DrawGeoMarker(centerMap: boolean) {

        if (latitude. value.trim() !== "" &&
            longitude.value.trim() !== "" &&
            !isNaN(_longitude)            &&
            !isNaN(_latitude)) {

            if (geoMarker == null)
                geoMarker = new mapboxgl.Marker().
                    setLngLat([_longitude, _latitude]).
                    addTo(map);

            geoMarker.setLngLat([_longitude, _latitude]);

            if (centerMap)
                map.setCenter([_longitude, _latitude]);

        }

        else {

            if (geoMarker != null) {
                geoMarker.remove();
                geoMarker = null;
            }

        }

    }


    function AnyChangesMade(): boolean {

        // address
        if ((organizationJSON.address?.street      !== undefined ? organizationJSON.address.street              : "") !== street.value)
            return true;

        if ((organizationJSON.address?.houseNumber !== undefined ? organizationJSON.address.houseNumber         : "") !== houseNumber.value)
            return true;

        if ((organizationJSON.address?.floorLevel  !== undefined ? organizationJSON.address.floorLevel          : "") !== floorLevel.value)
            return true;

        if ((organizationJSON.address?.postalCode  !== undefined ? organizationJSON.address.postalCode          : "") !== postalCode.value)
            return true;

        if ((organizationJSON.address?.city        !== undefined ? firstValue(organizationJSON.address.city)    : "") !== city.value)
            return true;

        if ((organizationJSON.address?.country     !== undefined ? organizationJSON.address.country             : "") !== country.value)
            return true;

        if ((organizationJSON.address?.comment     !== undefined ? firstValue(organizationJSON.address.comment) : "") !== comment.value)
            return true;


        // geo location
        if ((latitude.value  === "" && organizationJSON.geoLocation?.lat !== undefined) ||
            (latitude.value  !== "" && !isNaN(_latitude)  && _latitude  !== organizationJSON.geoLocation?.lat))
        {
            return true;
        }

        if ((longitude.value === "" && organizationJSON.geoLocation?.lng !== undefined) ||
            (longitude.value !== "" && !isNaN(_longitude) && _longitude !== organizationJSON.geoLocation?.lng))
        {
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

        // Only modify a copy of the original data
        let updatedOrganizationJSON = JSON.parse(JSON.stringify(organizationJSON));

        //#region Address

        // street
        if ((updatedOrganizationJSON.address?.street !== undefined ? updatedOrganizationJSON.address.street : "") !== street.value) {

            if (updatedOrganizationJSON.address === undefined)
                updatedOrganizationJSON.address = {} as IAddress;

            updatedOrganizationJSON.address.street = street.value.trim();

        }

        if (updatedOrganizationJSON.address?.street === "")
            delete updatedOrganizationJSON.address.street;


        // house number
        if ((updatedOrganizationJSON.address?.houseNumber !== undefined ? updatedOrganizationJSON.address.houseNumber : "") !== houseNumber.value) {

            if (updatedOrganizationJSON.address === undefined)
                updatedOrganizationJSON.address = {} as IAddress;

            updatedOrganizationJSON.address.houseNumber = houseNumber.value.trim();

        }

        if (updatedOrganizationJSON.address?.houseNumber === "")
            delete updatedOrganizationJSON.address.houseNumber;


        // floor level
        if ((updatedOrganizationJSON.address?.floorLevel !== undefined ? updatedOrganizationJSON.address.floorLevel : "") !== floorLevel.value) {

            if (updatedOrganizationJSON.address === undefined)
                updatedOrganizationJSON.address = {} as IAddress;

            updatedOrganizationJSON.address.floorLevel = floorLevel.value.trim();

        }

        if (updatedOrganizationJSON.address?.floorLevel === "")
            delete updatedOrganizationJSON.address.floorLevel;


        // postal code
        if ((updatedOrganizationJSON.address?.postalCode !== undefined ? updatedOrganizationJSON.address.postalCode : "") !== postalCode.value) {

            if (updatedOrganizationJSON.address === undefined)
                updatedOrganizationJSON.address = {} as IAddress;

            updatedOrganizationJSON.address.postalCode = postalCode.value.trim();

        }

        if (updatedOrganizationJSON.address?.postalCode === "")
            delete updatedOrganizationJSON.address.postalCode;


        // city
        if ((updatedOrganizationJSON.address?.city !== undefined ? updatedOrganizationJSON.address.city : "") !== city.value) {

            if (updatedOrganizationJSON.address === undefined)
                updatedOrganizationJSON.address = {} as IAddress;

            updatedOrganizationJSON.address.city = { "en": city.value.trim() };

        }

        if (updatedOrganizationJSON.address?.city?.en?.toString() === "")
            delete updatedOrganizationJSON.address.city;


        // country
        if ((updatedOrganizationJSON.address?.country !== undefined ? updatedOrganizationJSON.address.country : "") !== country.value) {

            if (updatedOrganizationJSON.address === undefined)
                updatedOrganizationJSON.address = {} as IAddress;

            updatedOrganizationJSON.address.country = country.value.trim();

        }

        if (updatedOrganizationJSON.address?.country === "")
            delete updatedOrganizationJSON.address.country;


        // comment
        if ((updatedOrganizationJSON.address?.comment !== undefined ? updatedOrganizationJSON.address.comment : "") !== comment.value) {

            if (updatedOrganizationJSON.address === undefined)
                updatedOrganizationJSON.address = {} as IAddress;

            updatedOrganizationJSON.address.comment = { "en": comment.value.trim() };

        }

        if (updatedOrganizationJSON.address?.comment?.en?.toString() === "")
            delete updatedOrganizationJSON.address.comment;


        // address
        if (updatedOrganizationJSON.address                     !== undefined &&
           (Object.keys(updatedOrganizationJSON.address).length === 0         ||
           (Object.keys(updatedOrganizationJSON.address).length === 1 && Object.keys(updatedOrganizationJSON.address)[0] === "@context")))
        {
            delete updatedOrganizationJSON.address;
        }

        //#endregion

        //#region Geo location

        // Latitude
        if ((updatedOrganizationJSON.geoLocation !== undefined && updatedOrganizationJSON.geoLocation.lat !== undefined ? updatedOrganizationJSON.geoLocation.lat : "") !== latitude.value) {

            if (updatedOrganizationJSON.geoLocation === undefined)
                updatedOrganizationJSON.geoLocation = {} as IGeoLocation;

            updatedOrganizationJSON.geoLocation.lat = Number.parseFloat(latitude.value);

        }

        // Longitude
        if ((updatedOrganizationJSON.geoLocation !== undefined && updatedOrganizationJSON.geoLocation.lng !== undefined ? updatedOrganizationJSON.geoLocation.lng : "") !== longitude.value) {

            if (updatedOrganizationJSON.geoLocation === undefined)
                updatedOrganizationJSON.geoLocation = {} as IGeoLocation;

            updatedOrganizationJSON.geoLocation.lng = Number.parseFloat(longitude.value);

        }

        // Remove geoLocation property if empty
        if (updatedOrganizationJSON.geoLocation !== undefined) {

            if (isNaN(updatedOrganizationJSON.geoLocation.lat))
                delete updatedOrganizationJSON.geoLocation.lat;

            if (isNaN(updatedOrganizationJSON.geoLocation.lng))
                delete updatedOrganizationJSON.geoLocation.lng;

            if (Object.keys(updatedOrganizationJSON.geoLocation).length === 0)
                delete updatedOrganizationJSON.geoLocation;

        }

        //#endregion


        saveButton.disabled = true;

        HTTPSet("/organizations/" + updatedOrganizationJSON["@id"],
                updatedOrganizationJSON,

                (status, response) => {

                    try
                    {

                        if (response !== "")
                            organizationJSON = JSON.parse(response);

                        saveButton.disabled   = !AnyChangesMade();
                        responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully stored updated organization address data.</div>";

                    }
                    catch (exception)
                    {
                        responseDiv.innerHTML = "<div class=\"HTTP Error\">An exception occured while storing updated organization address data:<br />" + exception + "</div>";
                    }

                },

                (statusCode, status, response) => {

                    try
                    {

                        var responseJSON  = response                 !== ""   ? JSON.parse(response)                : { "description": "Received an empty response from the remote API!" };
                        var info          = responseJSON.description !== null ? "<br />" + responseJSON.description : "";

                        responseDiv.innerHTML = "<div class=\"HTTP Error\">An error occured while storing updated organization address data: " + info + "</div>";

                    }
                    catch (exception)
                    {
                        responseDiv.innerHTML = "<div class=\"HTTP Error\">An exception occured while storing updated organization address data:<br />" + exception + "</div>";
                    }

                });

    }


    let pathElements        = window.location.pathname.split("/");
    let organizationId      = pathElements[pathElements.length - 2];

    FixMenuLinks("organizationMenu", organizationId);

    const organizationDiv   = document.       getElementById("organization")                    as HTMLDivElement;
    const headlineDiv       = organizationDiv.querySelector ("#headline")                       as HTMLDivElement;
    const dataDiv           = organizationDiv.querySelector ('#data')                           as HTMLDivElement;

    const street            = dataDiv.        querySelector ('#street')                         as HTMLInputElement;
    const houseNumber       = dataDiv.        querySelector ('#houseNumber')                    as HTMLInputElement;
    const floorLevel        = dataDiv.        querySelector ('#floorLevel')                     as HTMLInputElement;
    const postalCode        = dataDiv.        querySelector ('#postalCode')                     as HTMLInputElement;
    const city              = dataDiv.        querySelector ('#city')                           as HTMLInputElement;
    const country           = dataDiv.        querySelector ('#country')                        as HTMLInputElement;
    const comment           = dataDiv.        querySelector ("#comment")                        as HTMLTextAreaElement;

    const latitude          = dataDiv.        querySelector ('#latitude')                       as HTMLInputElement;
    const longitude         = dataDiv.        querySelector ('#longitude')                      as HTMLInputElement;

    const streetError       = street.         parentElement.querySelector('.validationError')   as HTMLDivElement;
    const houseNumberError  = houseNumber.    parentElement.querySelector('.validationError')   as HTMLDivElement;
    const floorLevelError   = floorLevel.     parentElement.querySelector('.validationError')   as HTMLDivElement;
    const postalCodeError   = postalCode.     parentElement.querySelector('.validationError')   as HTMLDivElement;
    const cityError         = city.           parentElement.querySelector('.validationError')   as HTMLDivElement;
    const countryError      = country.        parentElement.querySelector('.validationError')   as HTMLDivElement;
    const commentError      = comment.        parentElement.querySelector('.validationError')   as HTMLDivElement;

    const latitudeError     = latitude.       parentElement.querySelector('.validationError')   as HTMLDivElement;
    const longitudeError    = longitude.      parentElement.querySelector('.validationError')   as HTMLDivElement;

    const responseDiv       = document.       getElementById("response")                        as HTMLDivElement;

    const lowerButtonsDiv   = organizationDiv.querySelector ('#lowerButtons')                   as HTMLDivElement;
    const saveButton        = lowerButtonsDiv.querySelector ("#saveButton")                     as HTMLButtonElement;

    //#region Input validation

    street.oninput          = () => {

        if (country.value !== "" && country.value.length < 2) {
            saveButton.disabled = true;
            street.classList.add("error");
            streetError.innerHTML = "Invalid street name!";
            streetError.style.display = "flex";
            return false;
        }

        else {
            street.classList.remove("error");
            streetError.style.display = "none";
            ToogleSaveButton();
        }

    }

    houseNumber.oninput     = () => { ToogleSaveButton(); }

    floorLevel.oninput      = () => { ToogleSaveButton(); }

    postalCode.oninput      = () => {

        if (postalCode.value !== "" && postalCode.value.length < 4) {
            saveButton.disabled = true;
            postalCode.classList.add("error");
            postalCodeError.innerHTML = "Invalid postal code!";
            postalCodeError.style.display = "flex";
            return false;
        }

        else {
            postalCode.classList.remove("error");
            postalCodeError.style.display = "none";
            ToogleSaveButton();
        }

    }

    city.oninput            = () => {

        if (city.value !== "" && city.value.length < 2) {
            saveButton.disabled = true;
            city.classList.add("error");
            cityError.innerHTML = "Invalid city name!";
            cityError.style.display = "flex";
            return false;
        }

        else {
            city.classList.remove("error");
            cityError.style.display = "none";
            ToogleSaveButton();
        }

    }

    country.oninput         = () => {

        if (country.value !== "" && country.value.length < 2) {
            saveButton.disabled = true;
            country.classList.add("error");
            countryError.innerHTML = "Invalid country name or country code!";
            countryError.style.display = "flex";
            return false;
        }

        else {
            country.classList.remove("error");
            countryError.style.display = "none";
            ToogleSaveButton();
        }

    }

    comment.oninput         = () => { ToogleSaveButton(); }

    latitude.oninput        = () => {

        const newLatitudeString = latitude.value.trim();

        if (newLatitudeString === "") {
            latitude.classList.remove("error");
            latitudeError.style.display = "none";
            ToogleSaveButton();
        }

        else {

            const newLatitudeNumber = parseFloat(newLatitudeString);

            if (isNaN(newLatitudeNumber) || newLatitudeNumber < -90 || newLatitudeNumber > 90) {
                saveButton.disabled = true;
                latitude.classList.add("error");
                latitudeError.innerText = "Invalid latitude!";
                latitudeError.style.display = "flex";
            }
            else {
                latitude.classList.remove("error");
                latitudeError.style.display = "none";
                _latitude = newLatitudeNumber;
                ToogleSaveButton();
            }

        }

        // Might also remove it!
        DrawGeoMarker(true);

    }

    longitude.oninput       = () => {

        const newLongitudeString = longitude.value.trim();

        if (newLongitudeString === "") {
            longitude.classList.remove("error");
            longitudeError.style.display = "none";
            ToogleSaveButton();
        }

        else {

            const newLongitudeNumber = parseFloat(newLongitudeString);

            if (isNaN(newLongitudeNumber) || newLongitudeNumber < -180 || newLongitudeNumber > 180) {
                saveButton.disabled = true;
                longitude.classList.add("error");
                longitudeError.innerText = "Invalid longitude!";
                longitudeError.style.display = "flex";
            }
            else {
                longitude.classList.remove("error");
                longitudeError.style.display = "none";
                _longitude = newLongitudeNumber;
                ToogleSaveButton();
            }

        }

        // Might also remove it!
        DrawGeoMarker(true);

    }

    saveButton.onclick      = () => { SaveData(); }

    //#endregion

    //#region Init map

    mapboxgl.accessToken = 'pk.eyJ1IjoiYWh6ZiIsImEiOiJja2dja2s0emkwYTJ0MnlsZXQ0Y2VybmR3In0.ILJ6YtkyVXRVzjgdP-iQ2w';

    map = new mapboxgl.Map({
        container: 'map',
        style:     'mapbox://styles/mapbox/streets-v11',
    });

    map.addControl(
        // @ts-ignore
        new MapboxGeocoder({
            accessToken: mapboxgl.accessToken,
            mapboxgl: mapboxgl
        })
    );

    map.addControl(
        new mapboxgl.GeolocateControl({
            positionOptions: {
                enableHighAccuracy: true
            },
            trackUserLocation: true
        })
    );

    map.addControl(
        new mapboxgl.FullscreenControl()
    );

    function switchLayer(layer) {
        map.setStyle('mapbox://styles/mapbox/' + layer.target.id);
    }

    var inputs = document.getElementById('menu').getElementsByTagName('input');

    for (let i = 0; i < inputs.length; i++)
        inputs[i].onclick = switchLayer;


    map.on('click', function (e) {

        _latitude        = e.lngLat.lat;
        _longitude       = e.lngLat.lng;

        latitude. value  = e.lngLat.lat;
        longitude.value  = e.lngLat.lng;

        DrawGeoMarker(true);
        ToogleSaveButton();

    });

    map.on('load', function () {

        DrawGeoMarker(true);

        //if (organizationJSON.geoLocation?.lat !== undefined &&
        //    organizationJSON.geoLocation?.lng !== undefined) {

        //    map.flyTo({
        //        center: [organizationJSON.geoLocation.lng,
        //                 organizationJSON.geoLocation.lat],
        //        zoom:   14,
        //        speed: 1.8,
        //        curve:   1,
        //        easing(t) {
        //            return t;
        //        }
        //    });

        //}

    });

    //#endregion


    HTTPGet("/organizations/" + organizationId,

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

                    if (organizationJSON.address !== undefined) {
                        street.     value  = organizationJSON.address.street       !== undefined ? organizationJSON.address.street      : "";
                        houseNumber.value  = organizationJSON.address.houseNumber  !== undefined ? organizationJSON.address.houseNumber : "";
                        floorLevel. value  = organizationJSON.address.floorLevel   !== undefined ? organizationJSON.address.floorLevel  : "";
                        postalCode. value  = organizationJSON.address.postalCode   !== undefined ? organizationJSON.address.postalCode  : "";
                        city.       value  = organizationJSON.address.city?.en     !== undefined ? organizationJSON.address.city.en     : "";
                        country.    value  = organizationJSON.address.country      !== undefined ? organizationJSON.address.country     : "";
                        comment.    value  = organizationJSON.address.comment?.en  !== undefined ? organizationJSON.address.comment.en  : "";
                    }

                    if (organizationJSON.geoLocation?.lat !== undefined &&
                        organizationJSON.geoLocation?.lng !== undefined) {

                        _latitude          = organizationJSON.geoLocation.lat;
                        _longitude         = organizationJSON.geoLocation.lng;

                        latitude. value    = organizationJSON.geoLocation.lat.toString();
                        longitude.value    = organizationJSON.geoLocation.lng.toString();

                        map.setCenter([_longitude, _latitude]);

                    }

                }
                catch (exception)
                {
                    responseDiv.innerHTML = "<div class=\"HTTP Error\">An error occured while getting organization address data from the remote API:<br />" + exception + "</div>";
                }

            },

            (statusCode, status, response) => {

                try
                {

                    var responseJSON  = response                 !== ""   ? JSON.parse(response)                : { "description": "Received an empty response from the remote API!" };
                    var info          = responseJSON.description !== null ? "<br />" + responseJSON.description : "";

                    responseDiv.innerHTML = "<div class=\"HTTP Error\">An error occured while getting organization address data from the remote API: " + info + "</div>";

                }
                catch (exception)
                {
                    responseDiv.innerHTML = "<div class=\"HTTP Error\">An exception occured while getting organization address data from the remote API:<br />" + exception + "</div>";
                }

            });

}
