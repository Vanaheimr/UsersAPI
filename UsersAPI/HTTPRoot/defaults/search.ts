///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />

function StartSearch(RessourceURI:     string,
                     RessourcePrefix:  string,
                     ViewDelegate:     any,
                     NoURIupdate?:     boolean,
                     EditDesigner?:    any) {


    function ToggleViewsDiv23(this: HTMLElement, ev: MouseEvent) : any
    {

        // Return control to <a href=...</a>
        if ((ev.target as HTMLElement).tagName.toLowerCase() == "i")
            return true;

        var views = this.getElementsByClassName("views");
        if (views != null && views.length > 0)
        {

            var viewsDiv = views[0] as HTMLDivElement;

            if (viewsDiv.style.display == "block" && (ev.target as HTMLElement).id != this.id)
                return true;

        }


        var div = this as HTMLDivElement;

        if (div != null)
            ToggleViewsDiv3(div);

        return false;

    }

    //function ToggleViewsDiv2(element: string)
    //{

    //    var elementDiv = document.getElementById(element) as HTMLDivElement;

    //    if (elementDiv != null)
    //        ToggleViewsDiv3(elementDiv);

    //}

    function ToggleViewsDiv3(element: HTMLDivElement)
    {

        var views   = element.getElementsByClassName("views");
        //var buttons = element.getElementsByClassName("buttons");

        if (views != null && views.length > 0) {// && buttons != null && buttons.length > 0) {

            var viewsDiv    = views  [0] as HTMLDivElement;
            //var buttonsDiv  = buttons[0] as HTMLDivElement;
            //var expand      = buttonsDiv.querySelector("#expand")   as HTMLElement;
            //var collapse    = buttonsDiv.querySelector("#collapse") as HTMLElement;

            if (viewsDiv.style.display == "block")
            {
                viewsDiv.style.display = "none";
                //expand.  style.display = "inline-block";
                //collapse.style.display = "none";
            }

            else
            {
                viewsDiv.style.display = "block";
                //expand.  style.display = "none";
                //collapse.style.display = "inline-block";
            }

        }

    }


    var list                 = {};
    var ConnectionColors     = {};
    var StreamFilterPattern  = <HTMLInputElement> document.getElementById('filter');

    StreamFilterPattern.onchange = function () {

        var AllLogLines = (<HTMLDivElement[]> <any> document.getElementById('DataDiv').getElementsByClassName('Item'));

        for (var i = 0; i < AllLogLines.length; i++) {
            if (AllLogLines[i].innerHTML.indexOf(StreamFilterPattern.value) > -1)
                AllLogLines[i].style.display = 'block';
            else
                AllLogLines[i].style.display = 'none';
        }

    }

    HTTPGet(RessourceURI,

            (HTTPStatus, ResponseText) => {

                let DataDiv = document.getElementById('DataDiv') as HTMLDivElement;
                let lists   = JSON.parse(ResponseText);

                for (var i = 0, len = lists.length; i < len; i++) {

                    let element = lists[i];
                    let id      = element["@id"];

                    list[RessourcePrefix + "_" + id] = element;

                    let elementDiv = DataDiv.appendChild(document.createElement('div'));
                    elementDiv.id        = RessourcePrefix + "_" + id;
                    elementDiv.className = "Item";
                    elementDiv.onclick   = ToggleViewsDiv23;

                    let IdDiv = elementDiv.appendChild(document.createElement('div'));
                    IdDiv.className = "Id";
                    IdDiv.innerHTML = ViewDelegate(element); // Id or info text!

                    //let editDiv = elementDiv.appendChild(document.createElement('div'));
                    //editDiv.className = "buttons";
                    //editDiv.innerHTML = "<a href=\"javascript:ToggleViewsDiv2('" + RessourcePrefix + "_" + id + "')\" id=\"expand\"   title=\"expand\"  ><i class=\"fas fa-expand\"      ></i></a>" +
                    //                    "<a href=\"javascript:ToggleViewsDiv2('" + RessourcePrefix + "_" + id + "')\" id=\"collapse\" title=\"collapse\"><i class=\"fas fa-compress\"    ></i></a>";

                    // Show description... or the name...
                    elementDiv.appendChild(CreateI18NDiv(element.description != undefined
                        ? element.description
                        : element.name,
                        "Description"));

                    let viewsDiv = elementDiv.appendChild(document.createElement('div'));
                    viewsDiv.className = "views";
                    viewsDiv.appendChild(PrintProperties("rawView", element, 0, "rawView"));

                    //if (EditDesigner != null)
                    //    EditDesigner(viewsDiv, element);

                }

            },

            (HTTPStatus, StatusText, ResponseText) => {

            });

}



function AddProperty(parentDiv:  HTMLDivElement,
                        className:  string,
                        key:        string,
                        content:    string): HTMLDivElement
{

    let propertyDiv  = parentDiv.appendChild(document.createElement('div'));
    propertyDiv.className  = "property " + className;

    let keyDiv = propertyDiv.appendChild(document.createElement('div'));
    keyDiv.className    = "key";
    keyDiv.innerHTML    = key;

    let valueDiv = propertyDiv.appendChild(document.createElement('div'));
    valueDiv.className  = "value";
    valueDiv.innerHTML  = content;

    return propertyDiv;

}

function StartSearch2<TSearch>(requestURL:        string,
                               item:              string,
                               items:             string,
                               doListView:        SearchListView<TSearch>,
                               doTableView:       SearchTableView<TSearch>,
                               noListViewLinks?:  boolean,
                               startView?:        string,
                               NoURIupdate?:      boolean,
                               context?:          SearchContext) {

    let viewMode          = startView != null ? startView : "listView";
    let context__         = { Search: Search };

    function Search(deletePreviousResults:  boolean,
                    whenDone?:              any)
    {

        // ignore local searches
        if (filterInput.value[0] == '#')
        {

            if (whenDone != null)
                whenDone();

            return;

        }

        leftButton.disabled   = true;
        rightButton.disabled  = true;

        var filterPattern     = filterInput.value != "" ? "include=" + encodeURI(filterInput.value) + "&" : "";
            take              = parseInt(takeSelect.options[takeSelect.selectedIndex].value);


        HTTPGet(requestURL + "?" + filterPattern + "take=" + take + "&skip=" + skip + (context__["statusFilter"] != null ? context__["statusFilter"] : ""),// + "&expand=members",
                (HTTPStatus, ResponseText) => {

                    try
                    {

                        let searchResults = JSON.parse(ResponseText);

                        numberOfResults = searchResults.length;

                        // delete previous search results...
                        if (deletePreviousResults || searchResults.length > 0)
                            searchResultsDiv.innerHTML = "";

                        switch (viewMode)
                        {

                            case "tableView":
                                try
                                {
                                    doTableView(searchResults, searchResultsDiv);
                                }
                                catch (exception)
                                { }
                                break;

                            default:
                                for (var i = 0, len = searchResults.length; i < len; i++) {

                                    let searchResult           = searchResults[i];
                                    let searchResultId         = searchResult["@id"];

                                    let searchResultDiv        = searchResultsDiv.appendChild(document.createElement('a')) as HTMLAnchorElement;
                                    searchResultDiv.id         = item + "_" + searchResultId;
                                    searchResultDiv.className  = "searchResult";

                                    if (noListViewLinks == false)
                                        searchResultDiv.href   = "/" + items + "/" + searchResult["@id"];

                                    try
                                    {
                                        doListView(searchResult, searchResultDiv);
                                    }
                                    catch (exception)
                                    { }

                                }

                        }

                        if (skip <= 0)
                            skip = 0;

                        messageDiv.innerHTML = searchResults.length > 0
                                                   ? "showing results " + (skip + 1) + " - " + (skip + Math.min(searchResults.length, take))
                                                   : "no matching " + items + " found";

                        if (skip > 0)
                            leftButton.disabled = false;

                        // a little edge case, whenever 'total number of results' % take == 0
                        if (searchResults.length == take)
                            rightButton.disabled = false;

                    }
                    catch (exception)
                    {
                        messageDiv.innerHTML = exception;
                    }

                    if (whenDone != null)
                        whenDone();

                },

                (HTTPStatus, StatusText, ResponseText) => {

                    messageDiv.innerHTML = "Server error: " + HTTPStatus + " " + StatusText + "<br />" + ResponseText;

                    if (whenDone != null)
                        whenDone();

                });

    }

    var skip              = 0;
    var take              = 10;
    var numberOfResults   = 0;

    let controlsDiv       = document.   getElementById("controls")       as HTMLDivElement;
    let filterInput       = controlsDiv.querySelector ("#filter")        as HTMLInputElement;
    let takeSelect        = controlsDiv.querySelector ("#takeSelect")    as HTMLSelectElement;
    let searchButton      = controlsDiv.querySelector ("#searchButton")  as HTMLButtonElement;
    let leftButton        = controlsDiv.querySelector ("#leftButton")    as HTMLButtonElement;
    let rightButton       = controlsDiv.querySelector ("#rightButton")   as HTMLButtonElement;

    let listViewButton    = document.   getElementById("listView")       as HTMLDivElement;
    let tableViewButton   = document.   getElementById("tableView")      as HTMLDivElement;

    let messageDiv        = document.   getElementById('message')        as HTMLDivElement;
    let searchResultsDiv  = document.   getElementById('searchResults')  as HTMLDivElement;

    filterInput.onchange = (ev: Event) => {
        if (filterInput.value[0] != '#')
            skip = 0;
    }

    filterInput.onkeyup = (ev: KeyboardEvent) => {

        if (filterInput.value[0] != '#') {
            if (ev.keyCode === 13)
                Search(true);
        }

        else
        {

            // Client-side searches...
            if (filterInput.value[0] == '#')
            {

                let pattern          = filterInput.value.substring(1);
                let AllLogLines      = (<HTMLDivElement[]> <any> document.getElementById('searchResults').getElementsByClassName('searchResult'));
                let numberOfMatches  = 0;

                for (let i = 0; i < AllLogLines.length; i++)
                {

                    if (AllLogLines[i].innerHTML.indexOf(pattern) > -1)
                    {
                        AllLogLines[i].style.display = 'block';
                        numberOfMatches++;
                    }

                    else
                        AllLogLines[i].style.display = 'none';

                }

                messageDiv.innerHTML = numberOfResults > 0
                                           ? "showing results " + (skip + 1) + " - " + (skip + Math.min(numberOfResults, take)) + " (" + numberOfMatches + " local matches)"
                                           : "no matching " + items + " found";

            }

        }

    }

    takeSelect.onchange = (ev: MouseEvent) => {
        Search(true);
    }

    searchButton.onclick = (ev: MouseEvent) => {
        Search(true);
    }

    leftButton.disabled = true;
    leftButton.onclick = (ev: MouseEvent) => {

        leftButton.classList.add("busy", "busyActive");
        rightButton.classList.add("busy");

        skip -= parseInt(takeSelect.options[takeSelect.selectedIndex].value);

        Search(true, () => {
            leftButton.classList.remove("busy", "busyActive");
            rightButton.classList.remove("busy");
        });

    }

    rightButton.onclick = (ev: MouseEvent) => {

        leftButton.classList.add("busy");
        rightButton.classList.add("busy", "busyActive");

        skip += parseInt(takeSelect.options[takeSelect.selectedIndex].value);

        Search(false, () => {
            leftButton.classList.remove("busy");
            rightButton.classList.remove("busy", "busyActive");
        });

    }

    if (listViewButton != null) {
        listViewButton.onclick = (ev: MouseEvent) => {
            viewMode = "listView";
            Search(true);
        }
    }

    if (tableViewButton != null) {
        tableViewButton.onclick = (ev: MouseEvent) => {
            viewMode = "tableView";
            Search(true);
        }
    }

    if (context)
        context(context__);

    Search(true);

    return context__;

}
