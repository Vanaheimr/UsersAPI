function StartOrganizationGeoLocation() {
    let geoMarker;
    let _latitude;
    let _longitude;
    function DrawGeoMarker(centerMap) {
        if (latitude.value.trim() !== "" &&
            longitude.value.trim() !== "" &&
            !isNaN(_longitude) &&
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
            if (geoMarker !== null) {
                geoMarker.remove();
                geoMarker = null;
            }
        }
    }
    function AnyChangesMade() {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l;
        // address
        if ((((_a = organizationJSON.address) === null || _a === void 0 ? void 0 : _a.street) !== undefined ? organizationJSON.address.street : "") !== street.value)
            return true;
        if ((((_b = organizationJSON.address) === null || _b === void 0 ? void 0 : _b.houseNumber) !== undefined ? organizationJSON.address.houseNumber : "") !== houseNumber.value)
            return true;
        if ((((_c = organizationJSON.address) === null || _c === void 0 ? void 0 : _c.floorLevel) !== undefined ? organizationJSON.address.floorLevel : "") !== floorLevel.value)
            return true;
        if ((((_d = organizationJSON.address) === null || _d === void 0 ? void 0 : _d.postalCode) !== undefined ? organizationJSON.address.postalCode : "") !== postalCode.value)
            return true;
        if ((((_e = organizationJSON.address) === null || _e === void 0 ? void 0 : _e.city) !== undefined ? firstValue(organizationJSON.address.city) : "") !== city.value)
            return true;
        if ((((_f = organizationJSON.address) === null || _f === void 0 ? void 0 : _f.country) !== undefined ? organizationJSON.address.country : "") !== country.value)
            return true;
        if ((((_g = organizationJSON.address) === null || _g === void 0 ? void 0 : _g.comment) !== undefined ? firstValue(organizationJSON.address.comment) : "") !== comment.value)
            return true;
        // geo location
        if ((latitude.value === "" && ((_h = organizationJSON.geoLocation) === null || _h === void 0 ? void 0 : _h.lat) !== undefined) ||
            (latitude.value !== "" && !isNaN(_latitude) && _latitude !== ((_j = organizationJSON.geoLocation) === null || _j === void 0 ? void 0 : _j.lat))) {
            return true;
        }
        if ((longitude.value === "" && ((_k = organizationJSON.geoLocation) === null || _k === void 0 ? void 0 : _k.lng) !== undefined) ||
            (longitude.value !== "" && !isNaN(_longitude) && _longitude !== ((_l = organizationJSON.geoLocation) === null || _l === void 0 ? void 0 : _l.lng))) {
            return true;
        }
        responseDiv.innerHTML = "";
        return false;
    }
    function ToogleSaveButton() {
        var changesDetected = AnyChangesMade();
        saveButton.disabled = !changesDetected;
        if (changesDetected)
            responseDiv.innerHTML = "";
        return changesDetected;
    }
    function SaveData() {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t;
        // Only modify a copy of the original data
        let updatedOrganizationJSON = JSON.parse(JSON.stringify(organizationJSON));
        //#region Address
        // street
        if ((((_a = updatedOrganizationJSON.address) === null || _a === void 0 ? void 0 : _a.street) !== undefined ? updatedOrganizationJSON.address.street : "") !== street.value) {
            if (updatedOrganizationJSON.address === undefined)
                updatedOrganizationJSON.address = {};
            updatedOrganizationJSON.address.street = street.value.trim();
        }
        if (((_b = updatedOrganizationJSON.address) === null || _b === void 0 ? void 0 : _b.street) === "")
            delete updatedOrganizationJSON.address.street;
        // house number
        if ((((_c = updatedOrganizationJSON.address) === null || _c === void 0 ? void 0 : _c.houseNumber) !== undefined ? updatedOrganizationJSON.address.houseNumber : "") !== houseNumber.value) {
            if (updatedOrganizationJSON.address === undefined)
                updatedOrganizationJSON.address = {};
            updatedOrganizationJSON.address.houseNumber = houseNumber.value.trim();
        }
        if (((_d = updatedOrganizationJSON.address) === null || _d === void 0 ? void 0 : _d.houseNumber) === "")
            delete updatedOrganizationJSON.address.houseNumber;
        // floor level
        if ((((_e = updatedOrganizationJSON.address) === null || _e === void 0 ? void 0 : _e.floorLevel) !== undefined ? updatedOrganizationJSON.address.floorLevel : "") !== floorLevel.value) {
            if (updatedOrganizationJSON.address === undefined)
                updatedOrganizationJSON.address = {};
            updatedOrganizationJSON.address.floorLevel = floorLevel.value.trim();
        }
        if (((_f = updatedOrganizationJSON.address) === null || _f === void 0 ? void 0 : _f.floorLevel) === "")
            delete updatedOrganizationJSON.address.floorLevel;
        // postal code
        if ((((_g = updatedOrganizationJSON.address) === null || _g === void 0 ? void 0 : _g.postalCode) !== undefined ? updatedOrganizationJSON.address.postalCode : "") !== postalCode.value) {
            if (updatedOrganizationJSON.address === undefined)
                updatedOrganizationJSON.address = {};
            updatedOrganizationJSON.address.postalCode = postalCode.value.trim();
        }
        if (((_h = updatedOrganizationJSON.address) === null || _h === void 0 ? void 0 : _h.postalCode) === "")
            delete updatedOrganizationJSON.address.postalCode;
        // city
        if ((((_j = updatedOrganizationJSON.address) === null || _j === void 0 ? void 0 : _j.city) !== undefined ? updatedOrganizationJSON.address.city : "") !== city.value) {
            if (updatedOrganizationJSON.address === undefined)
                updatedOrganizationJSON.address = {};
            updatedOrganizationJSON.address.city = { "en": city.value.trim() };
        }
        if (((_m = (_l = (_k = updatedOrganizationJSON.address) === null || _k === void 0 ? void 0 : _k.city) === null || _l === void 0 ? void 0 : _l.en) === null || _m === void 0 ? void 0 : _m.toString()) === "")
            delete updatedOrganizationJSON.address.city;
        // country
        if ((((_o = updatedOrganizationJSON.address) === null || _o === void 0 ? void 0 : _o.country) !== undefined ? updatedOrganizationJSON.address.country : "") !== country.value) {
            if (updatedOrganizationJSON.address === undefined)
                updatedOrganizationJSON.address = {};
            updatedOrganizationJSON.address.country = country.value.trim();
        }
        if (((_p = updatedOrganizationJSON.address) === null || _p === void 0 ? void 0 : _p.country) === "")
            delete updatedOrganizationJSON.address.country;
        // comment
        if ((((_q = updatedOrganizationJSON.address) === null || _q === void 0 ? void 0 : _q.comment) !== undefined ? updatedOrganizationJSON.address.comment : "") !== comment.value) {
            if (updatedOrganizationJSON.address === undefined)
                updatedOrganizationJSON.address = {};
            updatedOrganizationJSON.address.comment = { "en": comment.value.trim() };
        }
        if (((_t = (_s = (_r = updatedOrganizationJSON.address) === null || _r === void 0 ? void 0 : _r.comment) === null || _s === void 0 ? void 0 : _s.en) === null || _t === void 0 ? void 0 : _t.toString()) === "")
            delete updatedOrganizationJSON.address.comment;
        // address
        if (updatedOrganizationJSON.address !== undefined &&
            (Object.keys(updatedOrganizationJSON.address).length === 0 ||
                (Object.keys(updatedOrganizationJSON.address).length === 1 && Object.keys(updatedOrganizationJSON.address)[0] === "@context"))) {
            delete updatedOrganizationJSON.address;
        }
        //#endregion
        //#region Geo location
        // Latitude
        if ((updatedOrganizationJSON.geoLocation !== undefined && updatedOrganizationJSON.geoLocation.lat !== undefined ? updatedOrganizationJSON.geoLocation.lat : "") !== latitude.value) {
            if (updatedOrganizationJSON.geoLocation === undefined)
                updatedOrganizationJSON.geoLocation = {};
            updatedOrganizationJSON.geoLocation.lat = Number.parseFloat(latitude.value);
        }
        // Longitude
        if ((updatedOrganizationJSON.geoLocation !== undefined && updatedOrganizationJSON.geoLocation.lng !== undefined ? updatedOrganizationJSON.geoLocation.lng : "") !== longitude.value) {
            if (updatedOrganizationJSON.geoLocation === undefined)
                updatedOrganizationJSON.geoLocation = {};
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
        HTTPSet("/organizations/" + updatedOrganizationJSON["@id"], updatedOrganizationJSON, (status, response) => {
            try {
                if (response !== "")
                    organizationJSON = JSON.parse(response);
                saveButton.disabled = !AnyChangesMade();
                responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully stored updated organization address data.</div>";
            }
            catch (exception) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">An exception occured while storing updated organization address data:<br />" + exception + "</div>";
            }
        }, (statusCode, status, response) => {
            try {
                var responseJSON = response !== "" ? JSON.parse(response) : { "description": "Received an empty response from the remote API!" };
                var info = responseJSON.description !== null ? "<br />" + responseJSON.description : "";
                responseDiv.innerHTML = "<div class=\"HTTP Error\">An error occured while storing updated organization address data: " + info + "</div>";
            }
            catch (exception) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">An exception occured while storing updated organization address data:<br />" + exception + "</div>";
            }
        });
    }
    let pathElements = window.location.pathname.split("/");
    let organizationId = pathElements[pathElements.length - 2];
    FixMenuLinks("organizationMenu", organizationId);
    const organizationDiv = document.getElementById("organization");
    const headlineDiv = organizationDiv.querySelector("#headline");
    const dataDiv = organizationDiv.querySelector('#data');
    const street = dataDiv.querySelector('#street');
    const houseNumber = dataDiv.querySelector('#houseNumber');
    const floorLevel = dataDiv.querySelector('#floorLevel');
    const postalCode = dataDiv.querySelector('#postalCode');
    const city = dataDiv.querySelector('#city');
    const country = dataDiv.querySelector('#country');
    const comment = dataDiv.querySelector("#comment");
    const latitude = dataDiv.querySelector('#latitude');
    const longitude = dataDiv.querySelector('#longitude');
    const streetError = street.parentElement.querySelector('.validationError');
    const houseNumberError = houseNumber.parentElement.querySelector('.validationError');
    const floorLevelError = floorLevel.parentElement.querySelector('.validationError');
    const postalCodeError = postalCode.parentElement.querySelector('.validationError');
    const cityError = city.parentElement.querySelector('.validationError');
    const countryError = country.parentElement.querySelector('.validationError');
    const commentError = comment.parentElement.querySelector('.validationError');
    const latitudeError = latitude.parentElement.querySelector('.validationError');
    const longitudeError = longitude.parentElement.querySelector('.validationError');
    const responseDiv = document.getElementById("response");
    const lowerButtonsDiv = organizationDiv.querySelector('#lowerButtons');
    const saveButton = lowerButtonsDiv.querySelector("#saveButton");
    //#region Input validation
    street.oninput = () => {
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
    };
    houseNumber.oninput = () => { ToogleSaveButton(); };
    floorLevel.oninput = () => { ToogleSaveButton(); };
    postalCode.oninput = () => {
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
    };
    city.oninput = () => {
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
    };
    country.oninput = () => {
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
    };
    comment.oninput = () => { ToogleSaveButton(); };
    latitude.oninput = () => {
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
    };
    longitude.oninput = () => {
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
    };
    saveButton.onclick = () => { SaveData(); };
    //#endregion
    //#region Init map
    mapboxgl.accessToken = 'pk.eyJ1IjoiYWh6ZiIsImEiOiJja2dja2s0emkwYTJ0MnlsZXQ0Y2VybmR3In0.ILJ6YtkyVXRVzjgdP-iQ2w';
    map = new mapboxgl.Map({
        container: 'map',
        style: 'mapbox://styles/mapbox/streets-v11',
    });
    map.addControl(
    // @ts-ignore
    new MapboxGeocoder({
        accessToken: mapboxgl.accessToken,
        mapboxgl: mapboxgl
    }));
    map.addControl(new mapboxgl.GeolocateControl({
        positionOptions: {
            enableHighAccuracy: true
        },
        trackUserLocation: true
    }));
    map.addControl(new mapboxgl.FullscreenControl());
    function switchLayer(layer) {
        map.setStyle('mapbox://styles/mapbox/' + layer.target.id);
    }
    var inputs = document.getElementById('menu').getElementsByTagName('input');
    for (let i = 0; i < inputs.length; i++)
        inputs[i].onclick = switchLayer;
    map.on('click', function (e) {
        _latitude = e.lngLat.lat;
        _longitude = e.lngLat.lng;
        latitude.value = e.lngLat.lat;
        longitude.value = e.lngLat.lng;
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
    HTTPGet("/organizations/" + organizationId, (status, response) => {
        var _a, _b, _c, _d;
        try {
            organizationJSON = ParseJSON_LD(response);
            headlineDiv.querySelector("#name #language").innerText = firstKey(organizationJSON.name);
            headlineDiv.querySelector("#name #I18NText").innerText = firstValue(organizationJSON.name);
            if (organizationJSON.description) {
                headlineDiv.querySelector("#description").style.display = "block";
                headlineDiv.querySelector("#description #language").innerText = firstKey(organizationJSON.description);
                headlineDiv.querySelector("#description #I18NText").innerText = firstValue(organizationJSON.description);
            }
            if (organizationJSON.address !== undefined) {
                street.value = organizationJSON.address.street !== undefined ? organizationJSON.address.street : "";
                houseNumber.value = organizationJSON.address.houseNumber !== undefined ? organizationJSON.address.houseNumber : "";
                floorLevel.value = organizationJSON.address.floorLevel !== undefined ? organizationJSON.address.floorLevel : "";
                postalCode.value = organizationJSON.address.postalCode !== undefined ? organizationJSON.address.postalCode : "";
                city.value = ((_a = organizationJSON.address.city) === null || _a === void 0 ? void 0 : _a.en) !== undefined ? organizationJSON.address.city.en : "";
                country.value = organizationJSON.address.country !== undefined ? organizationJSON.address.country : "";
                comment.value = ((_b = organizationJSON.address.comment) === null || _b === void 0 ? void 0 : _b.en) !== undefined ? organizationJSON.address.comment.en : "";
            }
            if (((_c = organizationJSON.geoLocation) === null || _c === void 0 ? void 0 : _c.lat) !== undefined &&
                ((_d = organizationJSON.geoLocation) === null || _d === void 0 ? void 0 : _d.lng) !== undefined) {
                _latitude = organizationJSON.geoLocation.lat;
                _longitude = organizationJSON.geoLocation.lng;
                latitude.value = organizationJSON.geoLocation.lat.toString();
                longitude.value = organizationJSON.geoLocation.lng.toString();
                map.setCenter([_longitude, _latitude]);
            }
        }
        catch (exception) {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">An error occured while getting organization address data from the remote API:<br />" + exception + "</div>";
        }
    }, (statusCode, status, response) => {
        try {
            var responseJSON = response !== "" ? JSON.parse(response) : { "description": "Received an empty response from the remote API!" };
            var info = responseJSON.description !== null ? "<br />" + responseJSON.description : "";
            responseDiv.innerHTML = "<div class=\"HTTP Error\">An error occured while getting organization address data from the remote API: " + info + "</div>";
        }
        catch (exception) {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">An exception occured while getting organization address data from the remote API:<br />" + exception + "</div>";
        }
    });
}
//# sourceMappingURL=address.js.map