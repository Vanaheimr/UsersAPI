///<reference path="../libs/date.format.ts" />

function ShowAPIKey(parentDiv: HTMLDivElement, JSON: any) {

    const DescriptionDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    DescriptionDiv.className = "row Description";

    const Description1Div = DescriptionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    Description1Div.className = "key";
    Description1Div.innerText = "Description";

    const Description2Div = DescriptionDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    Description2Div.className = "value";
    Description2Div.innerText = JSON["description"]["en"];



    const APIKeyDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    APIKeyDiv.className = "row APIKey";

    const APIKeyDiv1 = APIKeyDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    APIKeyDiv1.className = "key";
    APIKeyDiv1.innerText = "API key";

    const APIKeyDiv2 = APIKeyDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    APIKeyDiv2.className = "value";
    APIKeyDiv2.innerText = JSON["@id"];



    const AccessRightsDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    AccessRightsDiv.className = "row AccessRights";

    const AccessRightsDiv1 = AccessRightsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    AccessRightsDiv1.className = "key";
    AccessRightsDiv1.innerText = "AccessRights";

    const AccessRightsDiv2 = AccessRightsDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    AccessRightsDiv2.className = "value";
    AccessRightsDiv2.innerText = JSON["accessRights"];



    const CreatedDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    CreatedDiv.className = "row Created";

    const CreatedDiv1 = CreatedDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    CreatedDiv1.className = "key";
    CreatedDiv1.innerText = "Created";

    const CreatedDiv2 = CreatedDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    CreatedDiv2.className = "value";
    CreatedDiv2.innerText = JSON["created"];



    const NotBeforeDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    NotBeforeDiv.className = "row NotBefore";

    const NotBeforeDiv1 = NotBeforeDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    NotBeforeDiv1.className = "key";
    NotBeforeDiv1.innerText = "NotBefore";

    const NotBeforeDiv2 = NotBeforeDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    NotBeforeDiv2.className = "value";
    NotBeforeDiv2.innerText = JSON["notBefore"] !== null ? JSON["notBefore"] : "-";


    const NotAfterDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    NotAfterDiv.className = "row NotAfter";

    const NotAfterDiv1 = NotAfterDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    NotAfterDiv1.className = "key";
    NotAfterDiv1.innerText = "NotAfter";

    const NotAfterDiv2 = NotAfterDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    NotAfterDiv2.className = "value";
    NotAfterDiv2.innerText = JSON["notAfter"] !== null ? JSON["notAfter"] : "-";



    const IsDisabledDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    IsDisabledDiv.className = "row IsDisabled";

    const IsDisabledDiv1 = IsDisabledDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    IsDisabledDiv1.className = "key";
    IsDisabledDiv1.innerText = "Disabled";

    const IsDisabledDiv2 = IsDisabledDiv.appendChild(document.createElement('div')) as HTMLDivElement;
    IsDisabledDiv2.className = "value";
    IsDisabledDiv2.innerText = JSON["isDisabled"] !== null ? JSON["isDisabled"] : false;

}

function StartAPIKeys() {

    checkSignedIn(true);

    const APIKeysDiv   = document.getElementById('APIKeys')    as HTMLDivElement;
    const responseDiv  = document.getElementById("response")   as HTMLDivElement;


    HTTPGet("/users/" + SignInUser + "/APIKeys",

            (status, response) => {

                try
                {

                    const apiKeys = JSON.parse(response) as any[];

                    // [
                    //     {
                    //         "@id":           "vdYS345t87xYtGGOEUMdQxpfdpU43526pjdYxhfGhtbjnbOfnIWOdfjYnOKEdGQf",
                    //         "@context":      "https://opendata.social/contexts/UsersAPI+json/APIKeyInfo",
                    //         "userId":        "test",
                    //         "description":   {
                    //             "eng":  "Test key"
                    //         },
                    //         "accessRights":  "readOnly",
                    //         "created":       "2018-03-13T11:41:58.228Z"
                    //     }
                    // ]

                    if (apiKeys.length == 0)
                        responseDiv.innerHTML = "No API keys found!";

                    else
                    {

                        for (const apiKey of apiKeys) {

                            const APIKeyInfoDiv = APIKeysDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                            APIKeyInfoDiv.className = "APIKeyInfo";

                            ShowAPIKey(APIKeyInfoDiv, apiKey);

                        }

                    }

                }
                catch (exception)
                {
                    responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch API keys from the remote API!<br />" +
                                                "Exception: " + exception +
                                            "</div>";
                }

            },

            (statusCode, status, response) => {

                try
                {

                    var responseJSON  = response                 !== ""   ? JSON.parse(response)                : { "description": "Received an empty response from the remote API!" };
                    var info          = responseJSON.description !== null ? "<br />" + responseJSON.description : "";

                    responseDiv.innerHTML = "<div class=\"HTTP Error\">An error occured while getting API keys from the remote API: " + info + "</div>";

                }
                catch (exception)
                {
                    responseDiv.innerHTML = "<div class=\"HTTP Error\">An exception occured while getting API keys from the remote API:<br />" + exception + "</div>";
                }

            });

}
