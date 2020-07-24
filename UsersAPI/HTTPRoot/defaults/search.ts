///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />

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

enum searchResultsMode
{
    listView,
    tableView
}

function StartSearch<TSearch>(requestURL:        string,
                              nameOfItem:        string,
                              nameOfItems:       string,
                              doListView:        SearchListView<TSearch>,
                              doTableView:       SearchTableView<TSearch>,
                              noListViewLinks?:  boolean,
                              startView?:        searchResultsMode,
                              context?:          SearchContext) {

    requestURL = requestURL.indexOf('?') === -1
                    ? requestURL + '?'
                    : requestURL.endsWith('&')
                          ? requestURL
                          : requestURL + '&';

    let   skip                   = 0;
    let   take                   = 10;
    let   viewMode               = startView !== null ? startView : searchResultsMode.listView;
    const context__              = { Search: Search };

    const controlsDiv            = document.   getElementById("controls")              as HTMLDivElement;
    const patternFilter          = controlsDiv.querySelector ("#patternFilterInput")   as HTMLInputElement;
    const takeSelect             = controlsDiv.querySelector ("#takeSelect")           as HTMLSelectElement;
    const searchButton           = controlsDiv.querySelector ("#searchButton")         as HTMLButtonElement;
    const leftButton             = controlsDiv.querySelector ("#leftButton")           as HTMLButtonElement;
    const rightButton            = controlsDiv.querySelector ("#rightButton")          as HTMLButtonElement;

    const listViewButton         = controlsDiv.querySelector ("#listView")             as HTMLButtonElement;
    const tableViewButton        = controlsDiv.querySelector ("#tableView")            as HTMLButtonElement;

    const messageDiv             = document.   getElementById('message')               as HTMLDivElement;
    const localSearchMessageDiv  = document.   getElementById('localSearchMessage')    as HTMLDivElement;
    const searchResultsDiv       = document.   getElementById('searchResults')         as HTMLDivElement;


    function Search(deletePreviousResults:  boolean,
                    whenDone?:              any)
    {

        // handle local searches
        if (patternFilter.value[0] === '#')
        {

            if (whenDone !== null)
                whenDone();

            return;

        }

        // To avoid multiple clicks while waiting for the results from a slow server
        leftButton.disabled   = true;
        rightButton.disabled  = true;

        const filterPattern   = patternFilter.value !== "" ? "include=" + encodeURI(patternFilter.value) + "&" : "";

        HTTPGet(requestURL + "withMetadata&" + filterPattern + "take=" + take + "&skip=" + skip +
                                  (context__["statusFilter"] !== undefined ? context__["statusFilter"] : ""),// + "&expand=members",

                (status, response) => {

                    try
                    {

                        const JSONresponse          = JSON.parse(response);
                        const searchResults         = JSONresponse[nameOfItems] as Array<TSearch>;
                        const numberOfResults       = searchResults.length;
                        const totalNumberOfResults  = JSONresponse.filteredCount as number;

                        if (deletePreviousResults || numberOfResults > 0)
                            searchResultsDiv.innerHTML = "";

                        switch (viewMode)
                        {

                            case searchResultsMode.tableView:
                                try
                                {
                                    doTableView(searchResults, searchResultsDiv);
                                }
                                catch (exception)
                                { }
                                break;

                            default:
                                for (const searchResult of searchResults) {

                                    const searchResultDiv      = searchResultsDiv.appendChild(document.createElement('a')) as HTMLAnchorElement;
                                    searchResultDiv.id         = nameOfItem + "_" + searchResult["@id"];
                                    searchResultDiv.className  = "searchResult";

                                    if (noListViewLinks === false)
                                        searchResultDiv.href   = "/" + nameOfItems + "/" + searchResult["@id"];

                                    try
                                    {
                                        doListView(searchResult, searchResultDiv);
                                    }
                                    catch (exception)
                                    { }

                                }

                        }

                        messageDiv.innerHTML = searchResults.length > 0
                                                   ? "showing results " + (skip + 1) + " - " + (skip + Math.min(searchResults.length, take)) +
                                                         " of " + totalNumberOfResults
                                                   : "no matching " + nameOfItems + " found";

                        if (skip > 0)
                            leftButton.disabled  = false;

                        if (skip + take < totalNumberOfResults)
                            rightButton.disabled = false;

                    }
                    catch (exception)
                    {
                        messageDiv.innerHTML = exception;
                    }

                    if (whenDone !== null)
                        whenDone();

                },

                (HTTPStatus, status, response) => {

                    messageDiv.innerHTML = "Server error: " + HTTPStatus + " " + status + "<br />" + response;

                    if (whenDone !== null)
                        whenDone();

                });

    }


    if (patternFilter !== null)
    {

        patternFilter.onchange = () => {
            if (patternFilter.value[0] !== '#') {
                skip = 0;

            }
        }

        patternFilter.onkeyup = (ev: KeyboardEvent) => {

            if (patternFilter.value[0] !== '#') {
                if (ev.keyCode === 13)
                    Search(true);
            }

            // Client-side searches...
            else
            {

                const pattern          = patternFilter.value.substring(1);
                const logLines         = Array.from(document.getElementById('searchResults').getElementsByClassName('searchResult')) as HTMLDivElement[];
                let   numberOfMatches  = 0;

                for (const logLine of logLines) {

                    if (logLine.innerHTML.indexOf(pattern) > -1) {
                        logLine.style.display = 'block';
                        numberOfMatches++;
                    }

                    else
                        logLine.style.display = 'none';

                }

                if (localSearchMessageDiv !== null) {

                    localSearchMessageDiv.innerHTML = numberOfMatches > 0
                                                          ? numberOfMatches + " local matches"
                                                          : "no matching " + nameOfItems + " found";

                }

            }

        }

    }

    take = parseInt(takeSelect.options[takeSelect.selectedIndex].value);
    takeSelect.onchange = () => {
        take = parseInt(takeSelect.options[takeSelect.selectedIndex].value);
        Search(true);
    }

    searchButton.onclick = () => {
        Search(true);
    }

    leftButton.disabled = true;
    leftButton.onclick = () => {

        leftButton.classList.add("busy", "busyActive");
        rightButton.classList.add("busy");

        skip -= take;

        if (skip < 0)
            skip = 0;

        Search(true, () => {
            leftButton.classList.remove("busy", "busyActive");
            rightButton.classList.remove("busy");
        });

    }

    rightButton.disabled = true;
    rightButton.onclick = () => {

        leftButton.classList.add("busy");
        rightButton.classList.add("busy", "busyActive");

        skip += take;

        Search(false, () => {
            leftButton.classList.remove("busy");
            rightButton.classList.remove("busy", "busyActive");
        });

    }

    document.onkeydown = (ev: KeyboardEvent) => {

        // left arrow
        if (ev.keyCode === 37) {
            if (leftButton.disabled === false)
                leftButton.click();
        }

        // right arrow
        else if (ev.keyCode === 39) {
            if (rightButton.disabled === false)
                rightButton.click();
        }

    }

    if (listViewButton !== null) {
        listViewButton.onclick = () => {
            viewMode = searchResultsMode.listView;
            Search(true);
        }
    }

    if (tableViewButton !== null) {
        tableViewButton.onclick = () => {
            viewMode = searchResultsMode.tableView;
            Search(true);
        }
    }


    if (context)
        context(context__);

    Search(true);

    return context__;

}
