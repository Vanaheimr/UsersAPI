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
var BoxView = (function () {
    function BoxView(Parent, Prefix, HeadlineText) {
        this.myself = Parent.appendChild(document.createElement('div'));
        this.myself.id = Prefix + "View";
        this.HeadlineDiv = this.myself.appendChild(document.createElement('div'));
        this.HeadlineDiv.id = "headline";
        this.HeadlineDiv.innerText = HeadlineText;
        this.ContentDiv = this.myself.appendChild(document.createElement('div'));
        this.ContentDiv.id = "content";
        this.ButtonDiv = this.myself.appendChild(document.createElement('div'));
        this.ButtonDiv.id = "buttons";
        this.CurrentGroupDiv = this.ContentDiv;
    }
    BoxView.prototype.CreateGroup = function (Prefix, Visible) {
        if (Visible === void 0) { Visible = true; }
        var GroupDiv = this.ContentDiv.appendChild(document.createElement('div'));
        GroupDiv.id = Prefix + "Group";
        GroupDiv.className = "rowGroup";
        if (!Visible)
            GroupDiv.style.display = "none";
        this.CurrentGroupDiv = GroupDiv;
        return GroupDiv;
    };
    BoxView.prototype.CreateRow = function (Prefix, KeyText, greeter) {
        if (greeter === void 0) { greeter = null; }
        return this.CreateRow2(Prefix, KeyText, false, greeter);
    };
    ;
    BoxView.prototype.CreateRow2 = function (Prefix, KeyText, keyTop, greeter) {
        if (keyTop === void 0) { keyTop = false; }
        if (greeter === void 0) { greeter = null; }
        var DivRow = this.CurrentGroupDiv.appendChild(document.createElement('div'));
        DivRow.id = Prefix + "Row";
        DivRow.className = "row";
        var DivKey = DivRow.appendChild(document.createElement('div'));
        DivKey.id = Prefix + "Key";
        DivKey.className = "key" + (keyTop ? " keyTop" : "");
        DivKey.innerText = KeyText;
        var DivValue = DivRow.appendChild(document.createElement('div'));
        DivValue.id = Prefix + "Value";
        DivValue.className = "value";
        if (greeter != null)
            greeter(DivValue);
        return DivValue;
    };
    BoxView.prototype.CreateButton = function (Prefix, Name) {
        var ButtonDiv = this.ButtonDiv.appendChild(document.createElement('button'));
        ButtonDiv.id = Prefix + "Button";
        ButtonDiv.className = "button";
        ButtonDiv.innerHTML = Name;
    };
    BoxView.prototype.CancelButton = function (Prefix, Name) {
        var aa = this;
        var ButtonDiv = this.ButtonDiv.appendChild(document.createElement('button'));
        ButtonDiv.id = Prefix + "Button";
        ButtonDiv.className = "button";
        ButtonDiv.innerText = Name;
        ButtonDiv.onclick = function (ev) {
            aa.myself.remove();
        };
    };
    return BoxView;
}());
//# sourceMappingURL=defaults.js.map