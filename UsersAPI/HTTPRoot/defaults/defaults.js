var HTTPCookieId = "UsersAPI";
var APIKey = null;
var CurrentlyHighlightedMenuItem = "";
var CurrentlyHighlightedSubmenuItem = "";
var UserProfileJSON;
var OrganizationJSON;
// #region MenuHighlight(name, NoURIupdate?)
function MenuHighlight(name, NoURIupdate) {
    if (CurrentlyHighlightedMenuItem != "") {
        var OldItem = document.getElementById('Item' + CurrentlyHighlightedMenuItem);
        if (OldItem != null)
            OldItem.classList.remove('active');
        var OldMenu = document.getElementById('Menu' + CurrentlyHighlightedMenuItem);
        if (OldMenu != null)
            OldMenu.style.display = "none";
    }
    var NewItem = document.getElementById('Item' + name);
    if (NewItem != null)
        NewItem.classList.add('active');
    var NewMenu = document.getElementById('Menu' + name);
    if (NewMenu != null)
        NewMenu.style.display = "block";
    CurrentlyHighlightedMenuItem = name;
    if (history && history.pushState && !NoURIupdate) {
        history.pushState(null, null, '/' + name.toLowerCase() + '/index.shtml');
    }
}
// #endregion
// #region SubmenuHighlight(name, subname, NoURIupdate?)
function SubmenuHighlight(name, subname, NoURIupdate) {
    MenuHighlight(name, true);
    if (CurrentlyHighlightedSubmenuItem != "") {
        var OldItem = document.getElementById('Item' + CurrentlyHighlightedSubmenuItem);
        if (OldItem != null)
            OldItem.classList.remove('active');
    }
    var NewItem = document.getElementById('Item' + name + "/" + subname);
    if (NewItem != null)
        NewItem.classList.add('active');
    CurrentlyHighlightedSubmenuItem = name + "/" + subname;
    //if (history && history.pushState && !NoURIupdate) {
    //    history.pushState(null, null, '/' + name.toLowerCase() + '/index.shtml');
    //}
}
// #endregion
// #region SendJSON(HTTPVerb, URI, APIKey, Data, OnSuccess, OnError)
function SendJSON(HTTPVerb, URI, Data, OnSuccess, OnError) {
    var ajax = new XMLHttpRequest();
    ajax.open(HTTPVerb, URI, true); // , user, password);
    ajax.setRequestHeader("Accept", "application/json; charset=UTF-8");
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
            else if (OnError && typeof OnError === 'function')
                OnError(this.status, this.statusText, ajax.responseText);
        }
    };
    if (Data != null)
        ajax.send(JSON.stringify(Data));
    else
        ajax.send();
}
// #endregion
function ParseJSON_LD(Text, Context) {
    if (Context === void 0) { Context = null; }
    var JObject = JSON.parse(Text);
    JObject["id"] = JObject["@id"];
    return JObject;
}
function firstKey(obj) {
    for (var a in obj)
        return a;
}
function firstValue(obj) {
    for (var a in obj)
        return obj[a];
}
function UpdateI18N(Div, JSON) {
    if (Div != null &&
        JSON != null &&
        firstValue(JSON) != null) {
        var opt = document.createElement('option');
        opt.value = firstKey(JSON);
        opt.innerHTML = firstKey(JSON);
        opt.selected = true;
        Div.querySelector("select").appendChild(opt);
        Div.querySelector("textarea").value = firstValue(JSON);
    }
}
// #region HTTPSet(Method, RessourceURI, APIKey, Data, OnSuccess, OnError)
function HTTP(Method, RessourceURI, Data, OnSuccess, OnError) {
    // #region Make HTTP call
    var ajax = new XMLHttpRequest();
    ajax.open(Method, RessourceURI, true);
    ajax.setRequestHeader("Accept", "application/json; charset=UTF-8");
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
            else if (OnError && typeof OnError === 'function')
                OnError(this.status, this.statusText, ajax.responseText);
        }
    };
    if (Data != null)
        ajax.send(JSON.stringify(Data));
    else
        ajax.send();
    // #endregion
}
// #endregion
// #region HTTPGet(RessourceURI, OnSuccess, OnError)
function HTTPGet(RessourceURI, OnSuccess, OnError) {
    // #region Make HTTP call
    var ajax = new XMLHttpRequest();
    ajax.open("GET", RessourceURI, true);
    ajax.setRequestHeader("Accept", "application/json; charset=UTF-8");
    ajax.setRequestHeader("X-Portal", "true");
    //ajax.setRequestHeader("Content-Type", "application/json; charset=UTF-8");
    //if (APIKey != null)
    //    ajax.setRequestHeader("APIKey", APIKey);
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
            else if (OnError && typeof OnError === 'function')
                OnError(this.status, this.statusText, ajax.responseText);
        }
    };
    ajax.send();
    // #endregion
}
// #endregion
// #region HTTPCount(RessourceURI, Data, OnSuccess, OnError)
function HTTPCount(RessourceURI, Data, OnSuccess, OnError) {
    // #region Make HTTP call
    var ajax = new XMLHttpRequest();
    ajax.open("COUNT", RessourceURI, true); // , user, password);
    ajax.setRequestHeader("Accept", "application/json; charset=UTF-8");
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
            else if (OnError && typeof OnError === 'function')
                OnError(this.status, this.statusText, ajax.responseText);
        }
    };
    if (Data != null)
        ajax.send(JSON.stringify(Data));
    else
        ajax.send();
    // #endregion
}
// #endregion
// #region Exists (RessourceURI, OnSuccess, OnError)
function Exists(RessourceURI, OnSuccess, OnError) {
    // #region Make HTTP call
    var ajax = new XMLHttpRequest();
    ajax.open("GET", RessourceURI, true); // , user, password);
    ajax.setRequestHeader("Accept", "application/json; charset=UTF-8");
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
            else if (OnError && typeof OnError === 'function')
                OnError(this.status, this.statusText);
        }
    };
    ajax.send();
    // #endregion
}
// #endregion
// #region HTTPSet(RessourceURI, Data, OnSuccess, OnError)
function HTTPSet(RessourceURI, Data, OnSuccess, OnError) {
    // #region Make HTTP call
    var ajax = new XMLHttpRequest();
    ajax.open("SET", RessourceURI, true);
    ajax.setRequestHeader("Accept", "application/json; charset=UTF-8");
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
            else if (OnError && typeof OnError === 'function')
                OnError(this.status, this.statusText, ajax.responseText);
        }
    };
    if (Data != null)
        ajax.send(JSON.stringify(Data));
    else
        ajax.send();
    // #endregion
}
// #endregion
// #region HTTPAdd(RessourceURI, Data, OnSuccess, OnError)
function HTTPAdd(RessourceURI, Data, OnSuccess, OnError) {
    // #region Make HTTP call
    var ajax = new XMLHttpRequest();
    ajax.open("ADD", RessourceURI, true); // , user, password);
    ajax.setRequestHeader("Accept", "application/json; charset=UTF-8");
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
            else if (OnError && typeof OnError === 'function')
                OnError(this.status, this.statusText, ajax.responseText);
        }
    };
    if (Data != null)
        ajax.send(JSON.stringify(Data));
    else
        ajax.send();
    // #endregion
}
// #endregion
// #region HTTPAddIfNotExists(RessourceURI, Data, OnSuccess, OnError)
function HTTPAddIfNotExists(RessourceURI, Data, OnSuccess, OnError) {
    // #region Make HTTP call
    var ajax = new XMLHttpRequest();
    ajax.open("ADDIFNOTEXISTS", RessourceURI, true); // , user, password);
    ajax.setRequestHeader("Accept", "application/json; charset=UTF-8");
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
            else if (OnError && typeof OnError === 'function')
                OnError(this.status, this.statusText, ajax.responseText);
        }
    };
    if (Data != null)
        ajax.send(JSON.stringify(Data));
    else
        ajax.send();
    // #endregion
}
// #endregion
// #region HTTPDelete(RessourceURI, OnSuccess, OnError)
function HTTPDelete(RessourceURI, OnSuccess, OnError) {
    // #region Make HTTP call
    var ajax = new XMLHttpRequest();
    ajax.open("DELETE", RessourceURI, true); // , user, password);
    ajax.setRequestHeader("Accept", "application/json; charset=UTF-8");
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
            else if (OnError && typeof OnError === 'function')
                OnError(this.status, this.statusText, ajax.responseText);
        }
    };
    ajax.send();
    // #endregion
}
// #endregion
// #region HTTPChown(RessourceURI, Data, OnSuccess, OnError)
function HTTPChown(RessourceURI, Data, OnSuccess, OnError) {
    // #region Make HTTP call
    var ajax = new XMLHttpRequest();
    ajax.open("CHOWN", RessourceURI, true); // , user, password);
    ajax.setRequestHeader("Accept", "application/json; charset=UTF-8");
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
            else if (OnError && typeof OnError === 'function')
                OnError(this.status, this.statusText, ajax.responseText);
        }
    };
    if (Data != null)
        ajax.send(JSON.stringify(Data));
    else
        ajax.send();
    // #endregion
}
// #endregion
// #region HTTPCheck(RessourceURI, Data, OnSuccess, OnError)
function HTTPCheck(RessourceURI, Data, OnSuccess, OnError) {
    var ajax = new XMLHttpRequest();
    ajax.open("CHECK", RessourceURI, true); // , user, password);
    ajax.setRequestHeader("Accept", "application/json; charset=UTF-8");
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
            else if (OnError && typeof OnError === 'function')
                OnError(this.status, this.statusText, ajax.responseText);
        }
    };
    if (Data != null)
        ajax.send(JSON.stringify(Data));
    else
        ajax.send();
}
// #endregion
// #region HTTPAuth(RessourceURI, Data)
function HTTPAuth(RessourceURI, Data) {
    var ajax = new XMLHttpRequest();
    ajax.open("AUTH", RessourceURI, false); // NOT ASYNC!
    ajax.setRequestHeader("Accept", "application/json; charset=UTF-8");
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
// #region HTTPImpersonate(RessourceURI, OnSuccess, OnError)
function HTTPImpersonate(RessourceURI, OnSuccess, OnError) {
    var ajax = new XMLHttpRequest();
    ajax.open("IMPERSONATE", RessourceURI, true); // NOT ASYNC!
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
            else if (OnError && typeof OnError === 'function')
                OnError(this.status, this.statusText, ajax.responseText);
        }
    };
    ajax.send();
}
// #endregion
// #region HTTPDepersonate(RessourceURI, OnSuccess, OnError)
function HTTPDepersonate(RessourceURI, OnSuccess, OnError) {
    var ajax = new XMLHttpRequest();
    ajax.open("DEPERSONATE", RessourceURI, true); // NOT ASYNC!
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
            else if (OnError && typeof OnError === 'function')
                OnError(this.status, this.statusText, ajax.responseText);
        }
    };
    ajax.send();
}
// #endregion
// #region HTTPSet__SYNCED(RessourceURI, Data)
function HTTPSet__SYNCED(RessourceURI, Data) {
    var ajax = new XMLHttpRequest();
    ajax.open("SET", RessourceURI, false); // NOT ASYNC!
    ajax.setRequestHeader("Accept", "application/json; charset=UTF-8");
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
function PrintArray(array, recursionLevel, CSSClassNames) {
    var listDiv = document.createElement('div');
    listDiv.className = "List" + (CSSClassNames ? " " + CSSClassNames : "");
    for (var number in array) {
        var item = array[number];
        if (typeof item === "string" || typeof item === "number") {
            var propertyValueDiv = listDiv.appendChild(document.createElement('div'));
            if (typeof item === "string")
                propertyValueDiv.innerHTML = '<p>' + item + '</p>';
            else
                propertyValueDiv.innerHTML = '<p>' + item.toString() + '</p>';
        }
        else if (item instanceof Array)
            listDiv.appendChild(PrintArray(item, recursionLevel));
        else
            listDiv.appendChild(PrintProperties(null, item, recursionLevel, "PropertyValue ListItem"));
    }
    return listDiv;
}
// #endregion
// #region PrintProperties(id, properties, recursionLevel, CSSClassNames?)
function PrintProperties(id, properties, recursionLevel, CSSClassNames) {
    var propertiesDiv = document.createElement('div');
    if (id != undefined)
        propertiesDiv.id = id;
    propertiesDiv.className = "Properties" + (CSSClassNames ? " " + CSSClassNames : "");
    for (var propertyKey in properties) {
        // #region Omit @id and @context at the top-level...
        if ((propertyKey == "@id" && recursionLevel == 0) ||
            propertyKey == "@context") {
            continue;
        }
        // #endregion
        var propertyValue = properties[propertyKey];
        var propertyDiv = propertiesDiv.appendChild(document.createElement('div'));
        propertyDiv.className = "Property";
        var propertyKeyDiv = propertyDiv.appendChild(document.createElement('div'));
        propertyKeyDiv.className = "PropertyKey";
        propertyKeyDiv.innerText = propertyKey;
        if (typeof propertyValue === "string" ||
            typeof propertyValue === "number" ||
            typeof propertyValue === "boolean" ||
            propertyValue instanceof HTMLDivElement) {
            var propertyValueDiv = propertyDiv.appendChild(document.createElement('div'));
            propertyValueDiv.className = "PropertyValue";
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
            propertyDiv.appendChild(PrintProperties(null, propertyValue, recursionLevel + 1, "PropertyValue"));
    }
    return propertiesDiv;
}
// #endregion
// #region ShowI18N(I18NString)
function ShowI18N(I18NString) {
    var I18NDiv = document.createElement('div');
    for (var I18NKey in I18NString) {
        var propertyKeyDiv = I18NDiv.appendChild(document.createElement('div'));
        propertyKeyDiv.className = "I18NLanguage";
        propertyKeyDiv.innerText = I18NKey;
        var propertyValueDiv = I18NDiv.appendChild(document.createElement('div'));
        propertyValueDiv.className = "I18NValue";
        propertyValueDiv.innerText = I18NString[I18NKey];
    }
    return I18NDiv.innerHTML;
}
// #endregion
// #region CreateI18NPrefixDiv(Prefix, I18NString, CSSClassNames?)
function CreateI18NPrefixDiv(Prefix, I18NString, CSSClassNames) {
    var I18NDiv = document.createElement('div');
    I18NDiv.className = "I18N" + (CSSClassNames ? " " + CSSClassNames : "");
    for (var I18NKey in I18NString) {
        var propertyKeyDiv = I18NDiv.appendChild(document.createElement('div'));
        propertyKeyDiv.className = "I18NLanguage";
        propertyKeyDiv.innerHTML = "<p>" + I18NKey + "</p>";
        var propertyValueDiv = I18NDiv.appendChild(document.createElement('div'));
        propertyValueDiv.className = "I18NValue";
        propertyValueDiv.innerHTML = Prefix + I18NString[I18NKey];
    }
    return I18NDiv;
}
// #endregion
// #region CreateI18NDiv(I18NString, CSSClassNames?)
function CreateI18NDiv(I18NString, CSSClassNames) {
    var I18NDiv = document.createElement('div');
    I18NDiv.className = "I18N" + (CSSClassNames ? " " + CSSClassNames : "");
    for (var I18NKey in I18NString) {
        var propertyKeyDiv = I18NDiv.appendChild(document.createElement('div'));
        propertyKeyDiv.className = "I18NLanguage";
        propertyKeyDiv.innerHTML = "<p>" + I18NKey + "</p>";
        var propertyValueDiv = I18NDiv.appendChild(document.createElement('div'));
        propertyValueDiv.className = "I18NValue";
        propertyValueDiv.innerHTML = I18NString[I18NKey];
    }
    return I18NDiv;
}
// #endregion
// #region CreateDiv(Content, CSSClassNames?)
function CreateDiv(Content, CSSClassNames) {
    var newDiv = document.createElement('div');
    newDiv.className = (CSSClassNames ? CSSClassNames : "");
    newDiv.innerHTML = Content;
    return newDiv;
}
// #endregion
var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
function parseLocalDate(DateString) {
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
function parseUTCDateWithDayOfWeek(UTCString) {
    if (UTCString === "")
        return "";
    moment.locale(window.navigator.language);
    return moment.utc(UTCString).local().format('ddd ll');
}
function parseUTCDateWithDayOfWeekShort(UTCString) {
    if (UTCString === "")
        return "";
    moment.locale(window.navigator.language);
    return moment.utc(UTCString).local().format('dd, LLL');
}
function parseUTCDate(UTCString) {
    if (UTCString === "")
        return "";
    moment.locale(window.navigator.language);
    return moment.utc(UTCString).local().format('ll');
}
function parseUTCTimestamp(UTCString) {
    if (UTCString === "")
        return "";
    moment.locale(window.navigator.language);
    return moment.utc(UTCString).local().format('LLLL');
}
//# sourceMappingURL=defaults.js.map