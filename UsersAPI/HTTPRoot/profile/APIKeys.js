///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
function ShowAPIKey(parentDiv, JSON) {
    const DescriptionDiv = parentDiv.appendChild(document.createElement('div'));
    DescriptionDiv.className = "row Description";
    const Description1Div = DescriptionDiv.appendChild(document.createElement('div'));
    Description1Div.className = "key";
    Description1Div.innerText = "Description";
    const Description2Div = DescriptionDiv.appendChild(document.createElement('div'));
    Description2Div.className = "value";
    Description2Div.innerText = JSON["description"]["eng"];
    const APIKeyDiv = parentDiv.appendChild(document.createElement('div'));
    APIKeyDiv.className = "row APIKey";
    const APIKeyDiv1 = APIKeyDiv.appendChild(document.createElement('div'));
    APIKeyDiv1.className = "key";
    APIKeyDiv1.innerText = "API key";
    const APIKeyDiv2 = APIKeyDiv.appendChild(document.createElement('div'));
    APIKeyDiv2.className = "value";
    APIKeyDiv2.innerText = JSON["@id"];
    const AccessRightsDiv = parentDiv.appendChild(document.createElement('div'));
    AccessRightsDiv.className = "row AccessRights";
    const AccessRightsDiv1 = AccessRightsDiv.appendChild(document.createElement('div'));
    AccessRightsDiv1.className = "key";
    AccessRightsDiv1.innerText = "AccessRights";
    const AccessRightsDiv2 = AccessRightsDiv.appendChild(document.createElement('div'));
    AccessRightsDiv2.className = "value";
    AccessRightsDiv2.innerText = JSON["accessRights"];
    const CreatedDiv = parentDiv.appendChild(document.createElement('div'));
    CreatedDiv.className = "row Created";
    const CreatedDiv1 = CreatedDiv.appendChild(document.createElement('div'));
    CreatedDiv1.className = "key";
    CreatedDiv1.innerText = "Created";
    const CreatedDiv2 = CreatedDiv.appendChild(document.createElement('div'));
    CreatedDiv2.className = "value";
    CreatedDiv2.innerText = JSON["created"];
    const NotBeforeDiv = parentDiv.appendChild(document.createElement('div'));
    NotBeforeDiv.className = "row NotBefore";
    const NotBeforeDiv1 = NotBeforeDiv.appendChild(document.createElement('div'));
    NotBeforeDiv1.className = "key";
    NotBeforeDiv1.innerText = "NotBefore";
    const NotBeforeDiv2 = NotBeforeDiv.appendChild(document.createElement('div'));
    NotBeforeDiv2.className = "value";
    NotBeforeDiv2.innerText = JSON["notBefore"] != null ? JSON["notBefore"] : "-";
    const NotAfterDiv = parentDiv.appendChild(document.createElement('div'));
    NotAfterDiv.className = "row NotAfter";
    const NotAfterDiv1 = NotAfterDiv.appendChild(document.createElement('div'));
    NotAfterDiv1.className = "key";
    NotAfterDiv1.innerText = "NotAfter";
    const NotAfterDiv2 = NotAfterDiv.appendChild(document.createElement('div'));
    NotAfterDiv2.className = "value";
    NotAfterDiv2.innerText = JSON["notAfter"] != null ? JSON["notAfter"] : "-";
    const IsDisabledDiv = parentDiv.appendChild(document.createElement('div'));
    IsDisabledDiv.className = "row IsDisabled";
    const IsDisabledDiv1 = IsDisabledDiv.appendChild(document.createElement('div'));
    IsDisabledDiv1.className = "key";
    IsDisabledDiv1.innerText = "Disabled";
    const IsDisabledDiv2 = IsDisabledDiv.appendChild(document.createElement('div'));
    IsDisabledDiv2.className = "value";
    IsDisabledDiv2.innerText = JSON["isDisabled"] != null ? JSON["isDisabled"] : false;
}
function StartAPIKeys() {
    checkSignedIn(true);
    HTTPGet("/users/" + SignInUser + "/APIKeys", (status, response) => {
        const multiplexer = JSON.parse(response);
        const user = SignInUser;
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
        const APIKeysDiv = document.getElementById('APIKeys');
        for (let i = 0, len = multiplexer.length; i < len; i++) {
            const APIKeyInfoDiv = APIKeysDiv.appendChild(document.createElement('div'));
            APIKeyInfoDiv.className = "APIKeyInfo";
            ShowAPIKey(APIKeyInfoDiv, multiplexer[i]);
        }
    }, (statusCode, status, response) => {
    });
}
//# sourceMappingURL=APIKeys.js.map