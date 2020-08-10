/////<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
/////<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/defaults/defaults.ts" />
function AddProperty(parentDiv, className, key, content) {
    let propertyDiv = parentDiv.appendChild(document.createElement('div'));
    propertyDiv.className = "property " + className;
    let keyDiv = propertyDiv.appendChild(document.createElement('div'));
    keyDiv.className = "key";
    keyDiv.innerHTML = key;
    let valueDiv = propertyDiv.appendChild(document.createElement('div'));
    valueDiv.className = "value";
    valueDiv.innerHTML = content;
    return propertyDiv;
}
var searchResultsMode;
(function (searchResultsMode) {
    searchResultsMode[searchResultsMode["listView"] = 0] = "listView";
    searchResultsMode[searchResultsMode["tableView"] = 1] = "tableView";
})(searchResultsMode || (searchResultsMode = {}));
function StartSearch(requestURL, nameOfItem, nameOfItems, doListView, doTableView, linkPrefix, startView, context) {
    return StartSearch2(requestURL, () => "", () => { }, nameOfItem, nameOfItems, doListView, doTableView, linkPrefix, startView, context);
}
function StartSearch2(requestURL, searchFilters, doStartUp, nameOfItem, nameOfItems, doListView, doTableView, linkPrefix, startView, context) {
    requestURL = requestURL.indexOf('?') === -1
        ? requestURL + '?'
        : requestURL.endsWith('&')
            ? requestURL
            : requestURL + '&';
    let firstSearch = true;
    let skip = 0;
    let take = 10;
    let currentDateFrom = null;
    let currentDateTo = null;
    let viewMode = startView !== null ? startView : searchResultsMode.listView;
    const context__ = { Search: Search };
    const datepicker = new DatePicker();
    const controlsDiv = document.getElementById("controls");
    const patternFilter = controlsDiv.querySelector("#patternFilterInput");
    const takeSelect = controlsDiv.querySelector("#takeSelect");
    const searchButton = controlsDiv.querySelector("#searchButton");
    const leftButton = controlsDiv.querySelector("#leftButton");
    const rightButton = controlsDiv.querySelector("#rightButton");
    const dateFilters = controlsDiv.querySelector("#dateFilters");
    const dateFrom = dateFilters === null || dateFilters === void 0 ? void 0 : dateFilters.querySelector("#dateFromText");
    const dateTo = dateFilters === null || dateFilters === void 0 ? void 0 : dateFilters.querySelector("#dateToText");
    const listViewButton = controlsDiv.querySelector("#listView");
    const tableViewButton = controlsDiv.querySelector("#tableView");
    const messageDiv = document.getElementById('message');
    const localSearchMessageDiv = document.getElementById('localSearchMessage');
    const searchResultsDiv = document.getElementById('searchResults');
    const downLoadButton = document.getElementById("downLoadButton");
    function Search(deletePreviousResults, resetSkip, whenDone) {
        if (resetSkip)
            skip = 0;
        // handle local searches
        if (patternFilter.value[0] === '#') {
            if (whenDone !== null)
                whenDone();
            return;
        }
        // To avoid multiple clicks while waiting for the results from a slow server
        leftButton.disabled = true;
        rightButton.disabled = true;
        const filters = (patternFilter.value !== "" ? "&match=" + encodeURI(patternFilter.value) : "") +
            (typeof searchFilters !== 'undefined' && searchFilters ? searchFilters() : "") +
            (currentDateFrom != null && currentDateFrom !== "" ? "&from=" + currentDateFrom : "") +
            (currentDateTo != null && currentDateTo !== "" ? "&to=" + currentDateTo : "");
        if (downLoadButton != null)
            downLoadButton.href = requestURL.replace("?", ".csv?") + "withMetadata&download" + filters;
        HTTPGet(requestURL + "withMetadata" + filters + "&skip=" + skip + "&take=" + take +
            (context__["statusFilter"] !== undefined ? context__["statusFilter"] : ""), // + "&expand=members",
        (status, response) => {
            try {
                const JSONresponse = ParseJSON_LD(response);
                const searchResults = JSONresponse[nameOfItems];
                const numberOfResults = searchResults.length;
                const totalNumberOfResults = JSONresponse.filteredCount;
                if (deletePreviousResults || numberOfResults > 0)
                    searchResultsDiv.innerHTML = "";
                if (firstSearch && typeof doStartUp !== 'undefined' && doStartUp) {
                    doStartUp(JSONresponse);
                    firstSearch = false;
                }
                switch (viewMode) {
                    case searchResultsMode.tableView:
                        try {
                            doTableView(searchResults, searchResultsDiv);
                        }
                        catch (exception) {
                            console.debug("Exception in search table view: " + exception);
                        }
                        break;
                    // List view
                    default:
                        for (const searchResult of searchResults) {
                            const searchResultDiv = searchResultsDiv.appendChild(document.createElement('a'));
                            searchResultDiv.id = nameOfItem + "_" + searchResult["@id"];
                            searchResultDiv.className = "searchResult";
                            if (typeof linkPrefix !== 'undefined' && linkPrefix)
                                searchResultDiv.href = linkPrefix(searchResult) + nameOfItems + "/" + searchResult["@id"];
                            try {
                                doListView(searchResult, searchResultDiv);
                            }
                            catch (exception) {
                                console.debug("Exception in search list view: " + exception);
                            }
                        }
                }
                messageDiv.innerHTML = searchResults.length > 0
                    ? "showing results " + (skip + 1) + " - " + (skip + Math.min(searchResults.length, take)) +
                        " of " + totalNumberOfResults
                    : "no matching " + nameOfItems + " found";
                if (skip > 0)
                    leftButton.disabled = false;
                if (skip + take < totalNumberOfResults)
                    rightButton.disabled = false;
            }
            catch (exception) {
                messageDiv.innerHTML = exception;
            }
            if (typeof whenDone !== 'undefined' && whenDone)
                whenDone();
        }, (HTTPStatus, status, response) => {
            messageDiv.innerHTML = "Server error: " + HTTPStatus + " " + status + "<br />" + response;
            if (typeof whenDone !== 'undefined' && whenDone)
                whenDone();
        });
    }
    if (patternFilter !== null) {
        patternFilter.onchange = () => {
            if (patternFilter.value[0] !== '#') {
                skip = 0;
            }
        };
        patternFilter.onkeyup = (ev) => {
            if (patternFilter.value[0] !== '#') {
                if (ev.keyCode === 13)
                    Search(true);
            }
            // Client-side searches...
            else {
                const pattern = patternFilter.value.substring(1);
                const logLines = Array.from(document.getElementById('searchResults').getElementsByClassName('searchResult'));
                let numberOfMatches = 0;
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
        };
    }
    take = parseInt(takeSelect.options[takeSelect.selectedIndex].value);
    takeSelect.onchange = () => {
        take = parseInt(takeSelect.options[takeSelect.selectedIndex].value);
        Search(true);
    };
    searchButton.onclick = () => {
        Search(true);
    };
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
    };
    rightButton.disabled = true;
    rightButton.onclick = () => {
        leftButton.classList.add("busy");
        rightButton.classList.add("busy", "busyActive");
        skip += take;
        Search(false, false, () => {
            leftButton.classList.remove("busy");
            rightButton.classList.remove("busy", "busyActive");
        });
    };
    document.onkeydown = (ev) => {
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
    };
    dateFrom.onclick = () => {
        datepicker.show(dateFrom, currentDateFrom, function (newDate) {
            dateFrom.value = parseUTCDate(newDate);
            currentDateFrom = newDate;
            Search(true, true);
        });
    };
    dateTo.onclick = () => {
        datepicker.show(dateTo, currentDateTo, function (newDate) {
            dateTo.value = parseUTCDate(newDate);
            currentDateTo = newDate;
            Search(true, true);
        });
    };
    if (listViewButton !== null) {
        listViewButton.onclick = () => {
            viewMode = searchResultsMode.listView;
            Search(true);
        };
    }
    if (tableViewButton !== null) {
        tableViewButton.onclick = () => {
            viewMode = searchResultsMode.tableView;
            Search(true);
        };
    }
    if (context)
        context(context__);
    Search(true);
    return context__;
}
//# sourceMappingURL=search.js.map