var HTTPCookieId = "UsersAPI";
var CurrentlyHighlightedMenuItem = "";
var CurrentlyHighlightedSubmenuItem = "";
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
function SendJSON(HTTPVerb, URI, APIKey, Data, OnSuccess, OnError) {
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
function UpdateI18NDescription(DescriptionDiv, JSON) {
    if (firstValue(JSON["description"]) != null) {
        var opt = document.createElement('option');
        opt.value = firstKey(JSON["description"]);
        opt.innerHTML = firstKey(JSON["description"]);
        opt.selected = true;
        DescriptionDiv.querySelector("#language").appendChild(opt);
        DescriptionDiv.querySelector("#description").value = firstValue(JSON["description"]);
    }
}
// #region HTTPGet(RessourceURI, APIKey, Data, OnSuccess, OnError)
function HTTPGet(RessourceURI, APIKey, Data, OnSuccess, OnError) {
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
// #region HTTPCount(RessourceURI, APIKey, Data, OnSuccess, OnError)
function HTTPCount(RessourceURI, APIKey, Data, OnSuccess, OnError) {
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
// #region Exists (RessourceURI, APIKey, OnSuccess, OnError)
function Exists(RessourceURI, APIKey, OnSuccess, OnError) {
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
// #region HTTPSet(RessourceURI, APIKey, Data, OnSuccess, OnError)
function HTTPSet(RessourceURI, APIKey, Data, OnSuccess, OnError) {
    // #region Make HTTP call
    var ajax = new XMLHttpRequest();
    ajax.open("SET", RessourceURI, true); // , user, password);
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
// #region HTTPAdd(RessourceURI, APIKey, Data, OnSuccess, OnError)
function HTTPAdd(RessourceURI, APIKey, Data, OnSuccess, OnError) {
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
// #region HTTPAddIfNotExists(RessourceURI, APIKey, Data, OnSuccess, OnError)
function HTTPAddIfNotExists(RessourceURI, APIKey, Data, OnSuccess, OnError) {
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
// #region HTTPChown(RessourceURI, APIKey, Data, OnSuccess, OnError)
function HTTPChown(RessourceURI, APIKey, Data, OnSuccess, OnError) {
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
// #region HTTPCheck(RessourceURI, APIKey, Data, OnSuccess, OnError)
function HTTPCheck(RessourceURI, APIKey, Data, OnSuccess, OnError) {
    // #region Make HTTP call
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
    // #endregion
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
// #region GetI18N(I18NString, CSSClassNames?)
function GetI18N(I18NString, CSSClassNames) {
    var I18NDiv = document.createElement('div');
    I18NDiv.className = "I18N" + (CSSClassNames ? " " + CSSClassNames : "");
    for (var I18NKey in I18NString) {
        var propertyKeyDiv = I18NDiv.appendChild(document.createElement('div'));
        propertyKeyDiv.className = "I18NLanguage";
        propertyKeyDiv.innerText = I18NKey;
        var propertyValueDiv = I18NDiv.appendChild(document.createElement('div'));
        propertyValueDiv.className = "I18NValue";
        propertyValueDiv.innerText = I18NString[I18NKey];
    }
    return I18NDiv;
}
// #endregion
//# sourceMappingURL=defaults.js.map