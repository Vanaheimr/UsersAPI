function StartOrganizationGeoLocation() {
    let geoPosition;
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
    function AnyChangesMade() {
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
            if (country.value !== "" && country.value.length < 2) {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Organizations must have a valid country!</div>";
                return false;
            }
            return true;
        }
        // comment
        if ((organizationJSON.address !== undefined && organizationJSON.address.comment !== undefined ? organizationJSON.address.comment : "") !== comment.value) {
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
    function ToogleSaveButton() {
        var changesDetected = AnyChangesMade();
        saveButton.disabled = !changesDetected;
        if (changesDetected)
            responseDiv.innerHTML = "";
        return changesDetected;
    }
    function SaveData() {
        var _a, _b, _c, _d, _e, _f, _g, _h, _j, _k, _l, _m, _o, _p, _q, _r, _s, _t;
        // street
        if ((((_a = organizationJSON.address) === null || _a === void 0 ? void 0 : _a.street) !== undefined ? organizationJSON.address.street : "") !== street.value) {
            if (organizationJSON.address === undefined)
                organizationJSON.address = {};
            organizationJSON.address.street = street.value;
        }
        if (((_b = organizationJSON.address) === null || _b === void 0 ? void 0 : _b.street) === "")
            delete organizationJSON.address.street;
        // house number
        if ((((_c = organizationJSON.address) === null || _c === void 0 ? void 0 : _c.houseNumber) !== undefined ? organizationJSON.address.houseNumber : "") !== houseNumber.value) {
            if (organizationJSON.address === undefined)
                organizationJSON.address = {};
            organizationJSON.address.houseNumber = houseNumber.value;
        }
        if (((_d = organizationJSON.address) === null || _d === void 0 ? void 0 : _d.houseNumber) === "")
            delete organizationJSON.address.houseNumber;
        // floor level
        if ((((_e = organizationJSON.address) === null || _e === void 0 ? void 0 : _e.floorLevel) !== undefined ? organizationJSON.address.floorLevel : "") !== floorLevel.value) {
            if (organizationJSON.address === undefined)
                organizationJSON.address = {};
            organizationJSON.address.floorLevel = floorLevel.value;
        }
        if (((_f = organizationJSON.address) === null || _f === void 0 ? void 0 : _f.floorLevel) === "")
            delete organizationJSON.address.floorLevel;
        // postal code
        if ((((_g = organizationJSON.address) === null || _g === void 0 ? void 0 : _g.postalCode) !== undefined ? organizationJSON.address.postalCode : "") !== postalCode.value) {
            if (organizationJSON.address === undefined)
                organizationJSON.address = {};
            organizationJSON.address.postalCode = postalCode.value;
        }
        if (((_h = organizationJSON.address) === null || _h === void 0 ? void 0 : _h.postalCode) === "")
            delete organizationJSON.address.postalCode;
        // city
        if ((((_j = organizationJSON.address) === null || _j === void 0 ? void 0 : _j.city) !== undefined ? organizationJSON.address.city : "") !== city.value) {
            if (organizationJSON.address === undefined)
                organizationJSON.address = {};
            organizationJSON.address.city = { "eng": city.value };
        }
        if (((_m = (_l = (_k = organizationJSON.address) === null || _k === void 0 ? void 0 : _k.city) === null || _l === void 0 ? void 0 : _l.eng) === null || _m === void 0 ? void 0 : _m.toString()) === "")
            delete organizationJSON.address.city;
        // country
        if ((((_o = organizationJSON.address) === null || _o === void 0 ? void 0 : _o.country) !== undefined ? organizationJSON.address.country : "") !== country.value) {
            if (organizationJSON.address === undefined)
                organizationJSON.address = {};
            organizationJSON.address.country = country.value;
        }
        if (((_p = organizationJSON.address) === null || _p === void 0 ? void 0 : _p.country) === "")
            delete organizationJSON.address.country;
        // comment
        if ((((_q = organizationJSON.address) === null || _q === void 0 ? void 0 : _q.comment) !== undefined ? organizationJSON.address.comment : "") !== comment.value) {
            if (organizationJSON.address === undefined)
                organizationJSON.address = {};
            organizationJSON.address.comment = { "eng": comment.value };
        }
        if (((_t = (_s = (_r = organizationJSON.address) === null || _r === void 0 ? void 0 : _r.comment) === null || _s === void 0 ? void 0 : _s.eng) === null || _t === void 0 ? void 0 : _t.toString()) === "")
            delete organizationJSON.address.comment;
        // address
        if (Object.keys(organizationJSON.address).length === 0 ||
            (Object.keys(organizationJSON.address).length === 1 && Object.keys(organizationJSON.address)[0] === "@context"))
            delete organizationJSON.address;
        // geo latitude
        if ((organizationJSON.geoLocation !== undefined && organizationJSON.geoLocation.lat !== undefined ? organizationJSON.geoLocation.lat : "") !== latitude.value) {
            if (organizationJSON.geoLocation === undefined)
                organizationJSON.geoLocation = {};
            organizationJSON.geoLocation.lat = Number.parseFloat(latitude.value);
        }
        // geo longitude
        if ((organizationJSON.geoLocation !== undefined && organizationJSON.geoLocation.lng !== undefined ? organizationJSON.geoLocation.lng : "") !== longitude.value) {
            if (organizationJSON.geoLocation === undefined)
                organizationJSON.geoLocation = {};
            organizationJSON.geoLocation.lng = Number.parseFloat(longitude.value);
        }
        // geo location
        if (organizationJSON.geoLocation !== undefined) {
            if (isNaN(organizationJSON.geoLocation.lat))
                delete organizationJSON.geoLocation.lat;
            if (isNaN(organizationJSON.geoLocation.lng))
                delete organizationJSON.geoLocation.lng;
            if (Object.keys(organizationJSON.geoLocation).length === 0)
                delete organizationJSON.geoLocation;
        }
        HTTPSet("/organizations/" + organizationJSON["@id"], organizationJSON, (HTTPStatus, ResponseText) => {
            try {
                var responseJSON = ResponseText != "" ? JSON.parse(ResponseText) : {};
                var info = responseJSON.description != null ? "<br />" + responseJSON.description : "";
                saveButton.disabled = !AnyChangesMade();
                responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully stored updated organization data." + info + "</div>";
            }
            catch (exception) {
                saveButton.disabled = !AnyChangesMade();
                responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully stored updated organization data.</div>";
            }
        }, (HTTPStatus, StatusText, ResponseText) => {
            try {
                var responseJSON = ResponseText != "" ? JSON.parse(ResponseText) : {};
                var info = responseJSON.description != null ? "<br />" + responseJSON.description : "";
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing organization data failed!" + info + "</div>";
            }
            catch (exception) {
            }
        });
    }
    let pathElements = window.location.pathname.split("/");
    let organizationId = pathElements[pathElements.length - 2];
    let organizationMenuDiv = document.getElementById("organizationMenu");
    let links = organizationMenuDiv.querySelectorAll("a");
    for (var i = 0; i < links.length; i++) {
        if (links[i].href.indexOf("00000000") > 0) {
            links[i].href = links[i].href.replace("00000000", organizationId);
        }
    }
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
    const responseDiv = document.getElementById("response");
    const lowerButtonsDiv = organizationDiv.querySelector('#lowerButtons');
    const saveButton = lowerButtonsDiv.querySelector("#saveButton");
    street.oninput = () => { ToogleSaveButton(); };
    houseNumber.oninput = () => { ToogleSaveButton(); };
    floorLevel.oninput = () => { ToogleSaveButton(); };
    postalCode.oninput = () => { ToogleSaveButton(); };
    city.oninput = () => { ToogleSaveButton(); };
    country.oninput = () => { ToogleSaveButton(); };
    comment.oninput = () => { ToogleSaveButton(); };
    latitude.oninput = () => { DrawGeoPosition(); ToogleSaveButton(); };
    longitude.oninput = () => { DrawGeoPosition(); ToogleSaveButton(); };
    saveButton.onclick = () => { SaveData(); };
    map.on('click', function (e) {
        const coordinate = e.latlng.wrap();
        latitude.value = coordinate.lat;
        longitude.value = coordinate.lng;
        DrawGeoPosition();
        ToogleSaveButton();
    });
    HTTPGet("/organizations/" + organizationId, (status, response) => {
        try {
            organizationJSON = ParseJSON_LD(response);
            headlineDiv.querySelector("#name #language").innerText = firstKey(organizationJSON.name);
            headlineDiv.querySelector("#name #I18NText").innerText = firstValue(organizationJSON.name);
            if (organizationJSON.description) {
                headlineDiv.querySelector("#description").style.display = "block";
                headlineDiv.querySelector("#description #language").innerText = firstKey(organizationJSON.description);
                headlineDiv.querySelector("#description #I18NText").innerText = firstValue(organizationJSON.description);
            }
            // data
            if (organizationJSON.address !== undefined) {
                street.value = organizationJSON.address.street !== undefined ? organizationJSON.address.street : "";
                houseNumber.value = organizationJSON.address.houseNumber !== undefined ? organizationJSON.address.houseNumber : "";
                floorLevel.value = organizationJSON.address.floorLevel !== undefined ? organizationJSON.address.floorLevel : "";
                postalCode.value = organizationJSON.address.postalCode !== undefined ? organizationJSON.address.postalCode : "";
                city.value = organizationJSON.address.city !== undefined ? firstValue(organizationJSON.address.city) : "";
                country.value = organizationJSON.address.country !== undefined ? organizationJSON.address.country : "";
                comment.value = organizationJSON.address.comment !== undefined ? organizationJSON.address.comment.eng : "";
            }
            if (organizationJSON.geoLocation !== undefined) {
                latitude.value = organizationJSON.geoLocation.lat !== undefined ? organizationJSON.geoLocation.lat.toString() : "";
                longitude.value = organizationJSON.geoLocation.lng !== undefined ? organizationJSON.geoLocation.lng.toString() : "";
            }
        }
        catch (exception) {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch organization data from server!</div>";
        }
    }, (statusCode, status, response) => {
        responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch organization data from server!</div>";
    });
}
//# sourceMappingURL=address.js.map