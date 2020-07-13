///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
function ShowAPIKey(parentDiv, JSON) {
    var DescriptionDiv = parentDiv.appendChild(document.createElement('div'));
    DescriptionDiv.className = "row Description";
    var Description1Div = DescriptionDiv.appendChild(document.createElement('div'));
    Description1Div.className = "key";
    Description1Div.innerText = "Description";
    var Description2Div = DescriptionDiv.appendChild(document.createElement('div'));
    Description2Div.className = "value";
    Description2Div.innerText = JSON["description"]["eng"];
    var APIKeyDiv = parentDiv.appendChild(document.createElement('div'));
    APIKeyDiv.className = "row APIKey";
    var APIKeyDiv1 = APIKeyDiv.appendChild(document.createElement('div'));
    APIKeyDiv1.className = "key";
    APIKeyDiv1.innerText = "API key";
    var APIKeyDiv2 = APIKeyDiv.appendChild(document.createElement('div'));
    APIKeyDiv2.className = "value";
    APIKeyDiv2.innerText = JSON["@id"];
    var AccessRightsDiv = parentDiv.appendChild(document.createElement('div'));
    AccessRightsDiv.className = "row AccessRights";
    var AccessRightsDiv1 = AccessRightsDiv.appendChild(document.createElement('div'));
    AccessRightsDiv1.className = "key";
    AccessRightsDiv1.innerText = "AccessRights";
    var AccessRightsDiv2 = AccessRightsDiv.appendChild(document.createElement('div'));
    AccessRightsDiv2.className = "value";
    AccessRightsDiv2.innerText = JSON["accessRights"];
    var CreatedDiv = parentDiv.appendChild(document.createElement('div'));
    CreatedDiv.className = "row Created";
    var CreatedDiv1 = CreatedDiv.appendChild(document.createElement('div'));
    CreatedDiv1.className = "key";
    CreatedDiv1.innerText = "Created";
    var CreatedDiv2 = CreatedDiv.appendChild(document.createElement('div'));
    CreatedDiv2.className = "value";
    CreatedDiv2.innerText = JSON["created"];
    var NotBeforeDiv = parentDiv.appendChild(document.createElement('div'));
    NotBeforeDiv.className = "row NotBefore";
    var NotBeforeDiv1 = NotBeforeDiv.appendChild(document.createElement('div'));
    NotBeforeDiv1.className = "key";
    NotBeforeDiv1.innerText = "NotBefore";
    var NotBeforeDiv2 = NotBeforeDiv.appendChild(document.createElement('div'));
    NotBeforeDiv2.className = "value";
    NotBeforeDiv2.innerText = JSON["notBefore"] != null ? JSON["notBefore"] : "-";
    var NotAfterDiv = parentDiv.appendChild(document.createElement('div'));
    NotAfterDiv.className = "row NotAfter";
    var NotAfterDiv1 = NotAfterDiv.appendChild(document.createElement('div'));
    NotAfterDiv1.className = "key";
    NotAfterDiv1.innerText = "NotAfter";
    var NotAfterDiv2 = NotAfterDiv.appendChild(document.createElement('div'));
    NotAfterDiv2.className = "value";
    NotAfterDiv2.innerText = JSON["notAfter"] != null ? JSON["notAfter"] : "-";
    var IsDisabledDiv = parentDiv.appendChild(document.createElement('div'));
    IsDisabledDiv.className = "row IsDisabled";
    var IsDisabledDiv1 = IsDisabledDiv.appendChild(document.createElement('div'));
    IsDisabledDiv1.className = "key";
    IsDisabledDiv1.innerText = "Disabled";
    var IsDisabledDiv2 = IsDisabledDiv.appendChild(document.createElement('div'));
    IsDisabledDiv2.className = "value";
    IsDisabledDiv2.innerText = JSON["isDisabled"] != null ? JSON["isDisabled"] : false;
}
function StartAPIKeys() {
    checkSignedIn(true);
    HTTPGet("/users/" + SignInUser + "/APIKeys", function (status, response) {
        var multiplexer = JSON.parse(response);
        var user = SignInUser;
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
        var APIKeysDiv = document.getElementById('APIKeys');
        for (var i = 0, len = multiplexer.length; i < len; i++) {
            var APIKeyInfoDiv = APIKeysDiv.appendChild(document.createElement('div'));
            APIKeyInfoDiv.className = "APIKeyInfo";
            ShowAPIKey(APIKeyInfoDiv, multiplexer[i]);
        }
    }, function (statusCode, status, response) {
    });
}
//# sourceMappingURL=APIKeys.js.map