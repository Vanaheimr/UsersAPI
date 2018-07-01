
var HTTPCookieId: string = "UsersAPI";
var CurrentlyHighlightedMenuItem     = "";
var CurrentlyHighlightedSubmenuItem  = "";

let UserProfileJSON: IUserProfile;

interface IUserProfile {
    id:               string;
    name:             string;
    email:            string;
    mobilePhone:      string;
    description:      Object;
    publicKeyRing:    string;
    privacyLevel:     string;
    isAuthenticated:  boolean;
    isDisabled:       boolean;
    //signatures:       Array<string>,
    hash:             string;
}

interface IAddress {
    city:             any;
    street:           string;
    houseNumber:      string;
    floorLevel:       string;
    postalCode:       string;
    country:          string;
    comment:          any;
}

// #region MenuHighlight(name, NoURIupdate?)

function MenuHighlight(name: string, NoURIupdate?: boolean) {

    if (CurrentlyHighlightedMenuItem != "") {

        var OldItem = <HTMLDivElement> document.getElementById('Item' + CurrentlyHighlightedMenuItem);
        if (OldItem != null)
            OldItem.classList.remove('active');

        var OldMenu = <HTMLDivElement> document.getElementById('Menu' + CurrentlyHighlightedMenuItem);
        if (OldMenu != null)
            OldMenu.style.display = "none";

    }

    var NewItem = <HTMLDivElement> document.getElementById('Item' + name);
    if (NewItem != null)
        NewItem.classList.add('active');

    var NewMenu = <HTMLDivElement> document.getElementById('Menu' + name);
    if (NewMenu != null)
        NewMenu.style.display = "block";

    CurrentlyHighlightedMenuItem = name;

    if (history && history.pushState && !NoURIupdate) {
        history.pushState(null, null, '/' + name.toLowerCase() + '/index.shtml');
    }

}

// #endregion

// #region SubmenuHighlight(name, subname, NoURIupdate?)

function SubmenuHighlight(name: string, subname: string, NoURIupdate?: boolean) {

    MenuHighlight(name, true);

    if (CurrentlyHighlightedSubmenuItem != "") {

        var OldItem = <HTMLDivElement> document.getElementById('Item' + CurrentlyHighlightedSubmenuItem);
        if (OldItem != null)
            OldItem.classList.remove('active');

    }

    var NewItem = <HTMLDivElement> document.getElementById('Item' + name + "/" + subname);
    if (NewItem != null)
        NewItem.classList.add('active');

    CurrentlyHighlightedSubmenuItem = name + "/" + subname;

    //if (history && history.pushState && !NoURIupdate) {
    //    history.pushState(null, null, '/' + name.toLowerCase() + '/index.shtml');
    //}

}

// #endregion

// #region SendJSON(HTTPVerb, URI, APIKey, Data, OnSuccess, OnError)

function SendJSON(HTTPVerb, URI, APIKey, Data, OnSuccess, OnError) {

    var ajax = new XMLHttpRequest();
    ajax.open(HTTPVerb, URI, true); // , user, password);
    ajax.setRequestHeader("Accept",       "application/json; charset=UTF-8");
    ajax.setRequestHeader("Content-Type", "application/json; charset=UTF-8");

    if (APIKey != null)
        ajax.setRequestHeader("APIKey", APIKey);

    ajax.onreadystatechange = function () {

        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {

            // Ok
            if (this.status >= 100 && this.status < 300) {

                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));

                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess(this.status, ajax.responseText);

            }

            else
                if (OnError && typeof OnError === 'function')
                    OnError(this.status, this.statusText, ajax.responseText);

        }

    }

    if (Data != null)
        ajax.send(JSON.stringify(Data));
    else
        ajax.send();

}

// #endregion



function ParseJSON_LD<T>(Text:    string,
                         Context: string = null): T {

    var JObject = JSON.parse(Text);

    JObject["id"] = JObject["@id"];

    return JObject as T;

}

function firstKey(obj) {
    for (var a in obj) return a;
}

function firstValue(obj) {
    for (var a in obj) return obj[a];
}

function UpdateI18NDescription(DescriptionDiv: HTMLDivElement,
                               JSON:           Object) {

    if (firstValue(JSON["description"]) != null)
    {

        var opt = document.createElement('option') as HTMLOptionElement;
        opt.value     = firstKey(JSON["description"]);
        opt.innerHTML = firstKey(JSON["description"]);
        opt.selected  = true;
        (DescriptionDiv.querySelector("#language")    as HTMLSelectElement).appendChild(opt);

        (DescriptionDiv.querySelector("#description") as HTMLTextAreaElement).value = firstValue(JSON["description"]);

    }

}


// #region HTTPGet(RessourceURI, APIKey, Data, OnSuccess, OnError)

function HTTPGet(RessourceURI: string,
                 APIKey:       string,
                 Data,
                 OnSuccess,
                 OnError) {

    // #region Make HTTP call

    let ajax = new XMLHttpRequest();
    ajax.open("GET", RessourceURI, true); // , user, password);
    ajax.setRequestHeader("Accept",       "application/json; charset=UTF-8");
    //ajax.setRequestHeader("Content-Type", "application/json; charset=UTF-8");

    if (APIKey != null)
        ajax.setRequestHeader("APIKey", APIKey);

    ajax.onreadystatechange = function () {

        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {

            // Ok
            if (this.status >= 100 && this.status < 300) {

                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));

                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess(this.status, ajax.responseText);

            }

            else
                if (OnError && typeof OnError === 'function')
                    OnError(this.status, this.statusText, ajax.responseText);

        }

    }

    if (Data != null)
        ajax.send(JSON.stringify(Data));
    else
        ajax.send();

    // #endregion

}

// #endregion

// #region HTTPCount(RessourceURI, APIKey, Data, OnSuccess, OnError)

function HTTPCount(RessourceURI: string,
                   APIKey:       string,
                   Data,
                   OnSuccess,
                   OnError) {

    // #region Make HTTP call

    let ajax = new XMLHttpRequest();
    ajax.open("COUNT", RessourceURI, true); // , user, password);
    ajax.setRequestHeader("Accept",       "application/json; charset=UTF-8");
    //ajax.setRequestHeader("Content-Type", "application/json; charset=UTF-8");

    if (APIKey != null)
        ajax.setRequestHeader("APIKey", APIKey);

    ajax.onreadystatechange = function () {

        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {

            // Ok
            if (this.status >= 100 && this.status < 300) {

                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));

                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess(this.status, ajax.responseText);

            }

            else
                if (OnError && typeof OnError === 'function')
                    OnError(this.status, this.statusText, ajax.responseText);

        }

    }

    if (Data != null)
        ajax.send(JSON.stringify(Data));
    else
        ajax.send();

    // #endregion

}

// #endregion

// #region Exists (RessourceURI, APIKey, OnSuccess, OnError)

function Exists(RessourceURI: string,
                APIKey:       string,
                OnSuccess,
                OnError) {

    // #region Make HTTP call

    let ajax = new XMLHttpRequest();
    ajax.open("GET", RessourceURI, true); // , user, password);
    ajax.setRequestHeader("Accept",       "application/json; charset=UTF-8");
    //ajax.setRequestHeader("Content-Type", "application/json; charset=UTF-8");

    if (APIKey != null)
        ajax.setRequestHeader("APIKey", APIKey);

    ajax.onreadystatechange = function () {

        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {

            // Ok
            if (this.status >= 100 && this.status < 300) {

                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));

                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess(this.status);

            }

            else
                if (OnError && typeof OnError === 'function')
                    OnError(this.status, this.statusText);

        }

    }

    ajax.send();

    // #endregion

}

// #endregion

// #region HTTPSet(RessourceURI, APIKey, Data, OnSuccess, OnError)

function HTTPSet(RessourceURI: string,
                 APIKey:       string,
                 Data,
                 OnSuccess,
                 OnError) {

    // #region Make HTTP call

    let ajax = new XMLHttpRequest();
    ajax.open("SET", RessourceURI, true); // , user, password);
    ajax.setRequestHeader("Accept",       "application/json; charset=UTF-8");
    ajax.setRequestHeader("Content-Type", "application/json; charset=UTF-8");

    if (APIKey != null)
        ajax.setRequestHeader("APIKey", APIKey);

    ajax.onreadystatechange = function () {

        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {

            // Ok
            if (this.status >= 100 && this.status < 300) {

                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));

                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess(this.status, ajax.responseText);

            }

            else
                if (OnError && typeof OnError === 'function')
                    OnError(this.status, this.statusText, ajax.responseText);

        }

    }

    if (Data != null)
        ajax.send(JSON.stringify(Data));
    else
        ajax.send();

    // #endregion

}

// #endregion

// #region HTTPAdd(RessourceURI, APIKey, Data, OnSuccess, OnError)

function HTTPAdd(RessourceURI: string,
                 APIKey:       string,
                 Data,
                 OnSuccess,
                 OnError) {

    // #region Make HTTP call

    let ajax = new XMLHttpRequest();
    ajax.open("ADD", RessourceURI, true); // , user, password);
    ajax.setRequestHeader("Accept",       "application/json; charset=UTF-8");
    ajax.setRequestHeader("Content-Type", "application/json; charset=UTF-8");

    if (APIKey != null)
        ajax.setRequestHeader("APIKey", APIKey);

    ajax.onreadystatechange = function () {

        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {

            // Ok
            if (this.status >= 100 && this.status < 300) {

                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));

                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess(this.status, ajax.responseText);

            }

            else
                if (OnError && typeof OnError === 'function')
                    OnError(this.status, this.statusText, ajax.responseText);

        }

    }

    if (Data != null)
        ajax.send(JSON.stringify(Data));
    else
        ajax.send();

    // #endregion

}

// #endregion

// #region HTTPAddIfNotExists(RessourceURI, APIKey, Data, OnSuccess, OnError)

function HTTPAddIfNotExists(RessourceURI: string,
                            APIKey:       string,
                            Data,
                            OnSuccess,
                            OnError) {

    // #region Make HTTP call

    let ajax = new XMLHttpRequest();
    ajax.open("ADDIFNOTEXISTS", RessourceURI, true); // , user, password);
    ajax.setRequestHeader("Accept",       "application/json; charset=UTF-8");
    ajax.setRequestHeader("Content-Type", "application/json; charset=UTF-8");

    if (APIKey != null)
        ajax.setRequestHeader("APIKey", APIKey);

    ajax.onreadystatechange = function () {

        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {

            // Ok
            if (this.status >= 100 && this.status < 300) {

                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));

                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess(this.status, ajax.responseText);

            }

            else
                if (OnError && typeof OnError === 'function')
                    OnError(this.status, this.statusText, ajax.responseText);

        }

    }

    if (Data != null)
        ajax.send(JSON.stringify(Data));
    else
        ajax.send();

    // #endregion

}

// #endregion

// #region HTTPChown(RessourceURI, APIKey, Data, OnSuccess, OnError)

function HTTPChown(RessourceURI: string,
                   APIKey:       string,
                   Data,
                   OnSuccess,
                   OnError) {

    // #region Make HTTP call

    let ajax = new XMLHttpRequest();
    ajax.open("CHOWN", RessourceURI, true); // , user, password);
    ajax.setRequestHeader("Accept",       "application/json; charset=UTF-8");
    ajax.setRequestHeader("Content-Type", "application/json; charset=UTF-8");

    if (APIKey != null)
        ajax.setRequestHeader("APIKey", APIKey);

    ajax.onreadystatechange = function () {

        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {

            // Ok
            if (this.status >= 100 && this.status < 300) {

                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));

                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess(this.status, ajax.responseText);

            }

            else
                if (OnError && typeof OnError === 'function')
                    OnError(this.status, this.statusText, ajax.responseText);

        }

    }

    if (Data != null)
        ajax.send(JSON.stringify(Data));
    else
        ajax.send();

    // #endregion

}

// #endregion

// #region HTTPCheck(RessourceURI, APIKey, Data, OnSuccess, OnError)

function HTTPCheck(RessourceURI: string,
                   APIKey:       string,
                   Data,
                   OnSuccess,
                   OnError) {

    // #region Make HTTP call

    let ajax = new XMLHttpRequest();
    ajax.open("CHECK", RessourceURI, true); // , user, password);
    ajax.setRequestHeader("Accept",       "application/json; charset=UTF-8");
    ajax.setRequestHeader("Content-Type", "application/json; charset=UTF-8");

    if (APIKey != null)
        ajax.setRequestHeader("APIKey", APIKey);

    ajax.onreadystatechange = function () {

        // 0 UNSENT | 1 OPENED | 2 HEADERS_RECEIVED | 3 LOADING | 4 DONE
        if (this.readyState == 4) {

            // Ok
            if (this.status >= 100 && this.status < 300) {

                //alert(ajax.getAllResponseHeaders());
                //alert(ajax.getResponseHeader("Date"));
                //alert(ajax.getResponseHeader("Cache-control"));
                //alert(ajax.getResponseHeader("ETag"));

                if (OnSuccess && typeof OnSuccess === 'function')
                    OnSuccess(this.status, ajax.responseText);

            }

            else
                if (OnError && typeof OnError === 'function')
                    OnError(this.status, this.statusText, ajax.responseText);

        }

    }

    if (Data != null)
        ajax.send(JSON.stringify(Data));
    else
        ajax.send();

    // #endregion

}

// #endregion

// #region HTTPAuth(RessourceURI, APIKey, Data)

function HTTPAuth(RessourceURI: string,
                  APIKey:       string,
                  Data) : string {

    let ajax = new XMLHttpRequest();
    ajax.open("AUTH", RessourceURI, false); // NOT ASYNC!
    ajax.setRequestHeader("Accept",       "application/json; charset=UTF-8");
    ajax.setRequestHeader("Content-Type", "application/json; charset=UTF-8");

    if (APIKey != null)
        ajax.setRequestHeader("APIKey", APIKey);

    if (Data != null)
        ajax.send(JSON.stringify(Data));
    else
        ajax.send();

    return ajax.responseText;

}

// #endregion

// #region HTTPSet__SYNCED(RessourceURI, APIKey, Data)

function HTTPSet__SYNCED(RessourceURI: string,
                         APIKey:       string,
                         Data):        string {

    let ajax = new XMLHttpRequest();
    ajax.open("SET", RessourceURI, false); // NOT ASYNC!
    ajax.setRequestHeader("Accept",       "application/json; charset=UTF-8");
    ajax.setRequestHeader("Content-Type", "application/json; charset=UTF-8");

    if (APIKey != null)
        ajax.setRequestHeader("APIKey", APIKey);

    if (Data != null)
        ajax.send(JSON.stringify(Data));
    else
        ajax.send();


    return ajax.responseText;

}

// #endregion



// #region PrintArray(aArray, recursionLevel, CSSClassNames?)

function PrintArray(array:           any,
                    recursionLevel:  number,
                    CSSClassNames?:  string) : HTMLDivElement {

    let listDiv        = <HTMLDivElement> document.createElement('div');
    listDiv.className  = "List" + (CSSClassNames ? " " + CSSClassNames : "");

    for (var number in array) {

        let item = array[number];

        if (typeof item === "string" || typeof item === "number")
        {

            let propertyValueDiv = <HTMLDivElement> listDiv.appendChild(document.createElement('div'));

            if (typeof item === "string")
                propertyValueDiv.innerHTML = '<p>' + item + '</p>';
            else
                propertyValueDiv.innerHTML = '<p>' + item.toString() + '</p>';

        }

        else if (item instanceof Array)
            listDiv.appendChild(PrintArray(item, recursionLevel));

        else
            listDiv.appendChild(PrintProperties(null,
                                                item,
                                                recursionLevel,
                                                "PropertyValue ListItem"));

    }

    return listDiv;

}

// #endregion

// #region PrintProperties(id, properties, recursionLevel, CSSClassNames?)

function PrintProperties(id:              string,
                         properties:      any,
                         recursionLevel:  number,
                         CSSClassNames?:  string) : HTMLDivElement {

    let propertiesDiv        = <HTMLDivElement> document.createElement('div');

    if (id != undefined)
        propertiesDiv.id     = id;

    propertiesDiv.className  = "Properties" + (CSSClassNames ? " " + CSSClassNames : "");

    for (var propertyKey in properties) {

        // #region Omit @id and @context at the top-level...

        if ((propertyKey == "@id" && recursionLevel == 0) ||
             propertyKey == "@context")
        {
            continue;
        }

        // #endregion

        let propertyValue = properties[propertyKey];

        let propertyDiv             = <HTMLDivElement> propertiesDiv.appendChild(document.createElement('div'));
        propertyDiv.className       = "Property";

        let propertyKeyDiv          = <HTMLDivElement> propertyDiv.  appendChild(document.createElement('div'));
        propertyKeyDiv.className    = "PropertyKey";
        propertyKeyDiv.innerText    = propertyKey;

        if (typeof propertyValue === "string"  ||
            typeof propertyValue === "number"  ||
            typeof propertyValue === "boolean" ||
            propertyValue instanceof HTMLDivElement)
        {

            let propertyValueDiv        = <HTMLDivElement> propertyDiv.appendChild(document.createElement('div'));
            propertyValueDiv.className  = "PropertyValue";

            if (typeof propertyValue === "string")
                propertyValueDiv.innerHTML = '<p>' + propertyValue + '</p>';

            else if (typeof propertyValue === "number" || typeof propertyValue === "boolean")
                propertyValueDiv.innerHTML = '<p>' + propertyValue.toString() + '</p>';

            else if (propertyValue instanceof HTMLDivElement)
                propertyDiv.appendChild(propertyValueDiv).appendChild(propertyValue);

        }

        else if (propertyValue instanceof Array)
            propertyDiv.appendChild(PrintArray(propertyValue, recursionLevel + 1));

        else
            propertyDiv.appendChild(PrintProperties(null,
                                                    propertyValue,
                                                    recursionLevel + 1,
                                                    "PropertyValue"));

    }

    return propertiesDiv;

}

// #endregion

// #region GetI18N(I18NString, CSSClassNames?)

function GetI18N(I18NString: object, CSSClassNames?: string) : HTMLDivElement {

    let I18NDiv = <HTMLDivElement> document.createElement('div');
    I18NDiv.className = "I18N" + (CSSClassNames ? " " + CSSClassNames : "");

    for (var I18NKey in I18NString) {

        let propertyKeyDiv          = <HTMLDivElement> I18NDiv.appendChild(document.createElement('div'));
        propertyKeyDiv.className    = "I18NLanguage";
        propertyKeyDiv.innerText    = I18NKey;

        let propertyValueDiv        = <HTMLDivElement> I18NDiv.appendChild(document.createElement('div'));
        propertyValueDiv.className  = "I18NValue";
        propertyValueDiv.innerText  = I18NString[I18NKey];

    }

    return I18NDiv;

}

// #endregion

var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

declare var moment: any;

function parseLocalDate(DateString: string): string {

    console.log("initial locale: " + moment.locale());
    console.log("initial date format:= " + moment.localeData().longDateFormat('L'));

    //var locale = window.navigator.language.userLanguage || window.navigator.language;
    var locale = window.navigator.language;
    console.log("changing locale to: " + locale);
    moment.locale(locale);

    console.log("updated locale: " + moment.locale());
    console.log("updated date format = " + moment.localeData().longDateFormat('L'));

    var _date = moment(DateString, moment.localeData().longDateFormat('l'));

    if (_date.isValid())
        return _date.toISOString();

    _date = moment(DateString, moment.localeData().longDateFormat('L'));

    if (_date.isValid())
        return _date.toISOString();

    _date = moment(DateString, moment.localeData().longDateFormat('ll'));

    if (_date.isValid())
        return _date.toISOString();

    _date = moment(DateString, moment.localeData().longDateFormat('LL'));

    if (_date.isValid())
        return _date.toISOString();

}


function parseUTCDate(UTCString: string): string {

    moment.locale(window.navigator.language);

    return moment.utc(UTCString).local().format('ll');

}

function parseUTCTimestamp(UTCString: string): string {

    moment.locale(window.navigator.language);

    return moment.utc(UTCString).local().format('LLLL');

}
