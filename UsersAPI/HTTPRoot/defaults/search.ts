/////<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
/////<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/defaults/defaults.ts" />

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



interface SearchFilter {
    (): string;
}

interface SearchStartUp<TMetadata> {
    (json: TMetadata): void;
}

interface SearchListView<TSearchResult> {
    (searchResult:    TSearchResult,
        searchResultDiv: HTMLAnchorElement): void;
}

interface SearchTableView<TSearchResult> {
    (searchResult: Array<TSearchResult>,
        searchResultDiv: HTMLDivElement): void;
}

interface SearchResult2Link<TSearchResult> {
    (searchResult: TSearchResult): string;
}

interface SearchContext {
    (context: any): void;
}

enum searchResultsMode
{
    listView,
    tableView
}


function StartSearch<TSearchResult>(requestURL:   string,
                                    nameOfItem:   string,
                                    nameOfItems:  string,
                                    doListView:   SearchListView<TSearchResult>,
                                    doTableView:  SearchTableView<TSearchResult>,
                                    linkPrefix?:  SearchResult2Link<TSearchResult>,
                                    startView?:   searchResultsMode,
                                    context?:     SearchContext) {

    return StartSearch2<any, TSearchResult>(requestURL,
                                            () => "",
                                            () => { },
                                            nameOfItem,
                                            nameOfItems,
                                            doListView,
                                            doTableView,
                                            linkPrefix,
                                            startView,
                                            context);

}

function StartSearch2<TMetadata extends TMetadataDefaults, TSearchResult>(requestURL:     string,
                                                                          searchFilters:  SearchFilter,
                                                                          doStartUp:      SearchStartUp<TMetadata>,
                                                                          nameOfItem:     string,
                                                                          nameOfItems:    string,
                                                                          doListView:     SearchListView<TSearchResult>,
                                                                          doTableView:    SearchTableView<TSearchResult>,
                                                                          linkPrefix?:    SearchResult2Link<TSearchResult>,
                                                                          startView?:     searchResultsMode,
                                                                          context?:       SearchContext) {

    requestURL = requestURL.indexOf('?') === -1
                    ? requestURL + '?'
                    : requestURL.endsWith('&')
                          ? requestURL
                          : requestURL + '&';

    let   firstSearch              = true;
    let   skip                     = 0;
    let   take                     = 10;
    let   currentDateFrom:string   = null;
    let   currentDateTo:string     = null;
    let   viewMode                 = startView !== null ? startView : searchResultsMode.listView;
    const context__                = { Search: Search };
    let   numberOfResults          = 0;
    let   filteredNumberOfResults  = 0;
    let   totalNumberOfResults     = 0;

    const controlsDiv              = document.    getElementById("controls")              as HTMLDivElement;
    const patternFilter            = controlsDiv. querySelector ("#patternFilterInput")   as HTMLInputElement;
    const takeSelect               = controlsDiv. querySelector ("#takeSelect")           as HTMLSelectElement;
    const searchButton             = controlsDiv. querySelector ("#searchButton")         as HTMLButtonElement;
    const leftButton               = controlsDiv. querySelector ("#leftButton")           as HTMLButtonElement;
    const rightButton              = controlsDiv. querySelector ("#rightButton")          as HTMLButtonElement;

    const dateFilters              = controlsDiv. querySelector ("#dateFilters")          as HTMLDivElement;
    const dateFrom                 = dateFilters?.querySelector ("#dateFromText")         as HTMLInputElement;
    const dateTo                   = dateFilters?.querySelector ("#dateToText")           as HTMLInputElement;
    const datepicker               = dateFilters != null ? new DatePicker() : null;

    const listViewButton           = controlsDiv. querySelector ("#listView")             as HTMLButtonElement;
    const tableViewButton          = controlsDiv. querySelector ("#tableView")            as HTMLButtonElement;

    const messageDiv               = document.    getElementById('message')               as HTMLDivElement;
    const localSearchMessageDiv    = document.    getElementById('localSearchMessage')    as HTMLDivElement;

    const resultsBox               = document.    getElementById('resultsBox')            as HTMLDivElement;
    const searchResultsDiv         = resultsBox.  querySelector ("#" + nameOfItems)       as HTMLDivElement;

    const downLoadButton           = document.    getElementById("downLoadButton")        as HTMLAnchorElement;


    function Search(deletePreviousResults:  boolean,
                    resetSkip?:             boolean,
                    whenDone?:              any)
    {

        if (resetSkip)
            skip = 0;

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

        const filters         = (patternFilter.value !== ""                             ? "&match="   + encodeURI(patternFilter.value) : "") +
                                (typeof searchFilters !== 'undefined' && searchFilters  ? searchFilters() : "") +
                                (currentDateFrom     !=  null && currentDateFrom !== "" ? "&from="    + currentDateFrom                : "") +
                                (currentDateTo       !=  null && currentDateTo   !== "" ? "&to="      + currentDateTo                  : "");

        if (downLoadButton != null)
            downLoadButton.href = requestURL.replace("?", ".csv?") + "withMetadata&download" + filters;

        HTTPGet(requestURL + "withMetadata" + filters + "&skip=" + skip + "&take=" + take +
                                  (context__["statusFilter"] !== undefined ? context__["statusFilter"] : ""),// + "&expand=members",

                (status, response) => {

                    try
                    {

                        const JSONresponse             = ParseJSON_LD<TMetadata>(response);
                        const searchResults            = JSONresponse[nameOfItems] as Array<TSearchResult>;
                              numberOfResults          = searchResults.length;
                              filteredNumberOfResults  = JSONresponse.filteredCount as number;
                              totalNumberOfResults     = JSONresponse.totalCount    as number;

                        if (deletePreviousResults || numberOfResults > 0)
                            searchResultsDiv.innerHTML = "";

                        if (firstSearch && typeof doStartUp !== 'undefined' && doStartUp) {
                            doStartUp(JSONresponse);
                            firstSearch = false;
                        }

                        switch (viewMode)
                        {

                            case searchResultsMode.tableView:
                                try
                                {
                                    doTableView(searchResults, searchResultsDiv);
                                }
                                catch (exception)
                                {
                                    console.debug("Exception in search table view: " + exception);
                                }
                                break;

                            // List view
                            default:
                                for (const searchResult of searchResults) {

                                    const searchResultDiv      = searchResultsDiv.appendChild(document.createElement('a')) as HTMLAnchorElement;
                                    searchResultDiv.id         = nameOfItem + "_" + searchResult["@id"];
                                    searchResultDiv.className  = "searchResult " + nameOfItem;

                                    if (typeof linkPrefix !== 'undefined' && linkPrefix)
                                        searchResultDiv.href   = linkPrefix(searchResult) + nameOfItems + "/" + searchResult["@id"];

                                    try
                                    {
                                        doListView(searchResult, searchResultDiv);
                                    }
                                    catch (exception)
                                    {
                                        console.debug("Exception in search list view: " + exception);
                                    }

                                }

                        }

                        messageDiv.innerHTML = searchResults.length > 0
                                                   ? "showing results " + (skip + 1) + " - " + (skip + Math.min(searchResults.length, take)) +
                                                         " of " + filteredNumberOfResults
                                                   : "no matching " + nameOfItems + " found";

                        if (skip > 0)
                            leftButton.disabled  = false;

                        if (skip + take < filteredNumberOfResults)
                            rightButton.disabled = false;

                    }
                    catch (exception)
                    {
                        messageDiv.innerHTML = exception;
                    }

                    if (typeof whenDone !== 'undefined' && whenDone)
                        whenDone();

                },

                (HTTPStatus, status, response) => {

                    messageDiv.innerHTML = "Server error: " + HTTPStatus + " " + status + "<br />" + response;

                    if (typeof whenDone !== 'undefined' && whenDone)
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

        Search(true, false, () => {
            leftButton.classList.remove("busy", "busyActive");
            rightButton.classList.remove("busy");
        });

    }

    rightButton.disabled = true;
    rightButton.onclick = () => {

        leftButton.classList.add("busy");
        rightButton.classList.add("busy", "busyActive");

        skip += take;

        Search(false, false, () => {
            leftButton.classList.remove("busy");
            rightButton.classList.remove("busy", "busyActive");
        });

    }

    document.onkeydown = (ev: KeyboardEvent) => {

        // left arrow
        if (ev.keyCode === 37 || ev.keyCode === 38) {
            if (leftButton.disabled === false)
                leftButton.click();
        }

        // right arrow
        else if (ev.keyCode === 39 || ev.keyCode === 40) {
            if (rightButton.disabled === false)
                rightButton.click();
        }

        // pos1
        else if (ev.keyCode === 36) {
            // Will set skip = 0!
            Search(true, true);
        }

        // end
        else if (ev.keyCode === 35) {
            skip = Math.trunc(filteredNumberOfResults / take) * take;
            Search(true, false);
        }

    }

    if (dateFrom != null) {
        dateFrom.onclick = () => {
            datepicker.show(dateFrom,
                currentDateFrom,
                function (newDate) {
                    dateFrom.value = parseUTCDate(newDate);
                    currentDateFrom = newDate;
                    Search(true, true);
                });
        }
    }

    if (dateTo != null) {
        dateTo.onclick = () => {
            datepicker.show(dateTo,
                currentDateTo,
                function (newDate) {
                    dateTo.value = parseUTCDate(newDate);
                    currentDateTo = newDate;
                    Search(true, true);
                });
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
