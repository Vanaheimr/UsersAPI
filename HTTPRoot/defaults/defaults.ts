
var HTTPCookieId: string = "UsersAPI";
var CurrentlyHighlightedMenuItem     = "";
var CurrentlyHighlightedSubmenuItem  = "";

interface IUserProfile {
    id:               string;
    name:             string;
    email:            string;
    description:      Object;
    privacyLevel:     string;
    isAuthenticated:  boolean;
    isDisabled:       boolean;
    //signatures:       Array<string>,
    hash:             string;
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





interface DivElementAction {
    (DoWith: HTMLDivElement): void;
}

class View {

    myself:           HTMLDivElement;
    HeadlineDiv:      HTMLDivElement;
    ContentDiv:       HTMLDivElement;
    CurrentGroupDiv:  HTMLDivElement;
    ButtonDiv:        HTMLDivElement;

    constructor(Parent:        HTMLDivElement,
                Prefix:        string,
                HeadlineText:  string) {

        this.myself = Parent.appendChild(document.createElement('div')) as HTMLDivElement;
        this.myself.id = Prefix + "View";

        this.HeadlineDiv = this.myself.appendChild(document.createElement('div')) as HTMLDivElement;
        this.HeadlineDiv.id        = "headline";
        this.HeadlineDiv.innerText = HeadlineText;

        this.ContentDiv  = this.myself.appendChild(document.createElement('div')) as HTMLDivElement;
        this.ContentDiv.id         = "content";

        this.ButtonDiv   = this.myself.appendChild(document.createElement('div')) as HTMLDivElement;
        this.ButtonDiv.id          = "buttons";

        this.CurrentGroupDiv = this.ContentDiv;

    }

    CreateGroup(Prefix:  string,
                Visible: boolean = true) {

        var GroupDiv = this.ContentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
        GroupDiv.id = Prefix + "Group";
        GroupDiv.className = "rowGroup";

        if (!Visible)
            GroupDiv.style.display = "none";

        this.CurrentGroupDiv = GroupDiv;
        return GroupDiv;

    }

    CreateRow(Prefix: string,
        KeyText: string,
        greeter: DivElementAction = null) {

        return this.CreateRow2(
            Prefix,
            KeyText,
            false,
            greeter);

    };

    CreateRow2(
        Prefix: string,
        KeyText: string,
        keyTop: boolean = false,
        greeter: DivElementAction = null) {


        var DivRow = this.CurrentGroupDiv.appendChild(document.createElement('div')) as HTMLDivElement;
        DivRow.id = Prefix + "Row";
        DivRow.className = "row";

        var DivKey = DivRow.appendChild(document.createElement('div')) as HTMLDivElement;
        DivKey.id = Prefix + "Key";
        DivKey.className = "key" + (keyTop ? " keyTop" : "");
        DivKey.innerText = KeyText;

        var DivValue = DivRow.appendChild(document.createElement('div')) as HTMLDivElement;
        DivValue.id = Prefix + "Value";
        DivValue.className = "value";

        if (greeter != null)
            greeter(DivValue);

        return DivValue;

    }

    CreateButton(Prefix: string,
                 Name:   string) {

        var ButtonDiv = this.ButtonDiv.appendChild(document.createElement('button')) as HTMLButtonElement;
        ButtonDiv.id = Prefix + "Button";
        ButtonDiv.className = "button";
        ButtonDiv.innerHTML = Name;

    }

    CancelButton(Prefix: string,
                 Name:   string) {

        var aa = this;

        var ButtonDiv = this.ButtonDiv.appendChild(document.createElement('button')) as HTMLButtonElement;
        ButtonDiv.id = Prefix + "Button";
        ButtonDiv.className = "button";
        ButtonDiv.innerText = Name;
        ButtonDiv.onclick = function (this: HTMLElement, ev: MouseEvent) {

            aa.myself.remove();

        };

    }


}
