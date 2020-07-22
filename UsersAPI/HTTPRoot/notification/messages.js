///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
function StartUserNotificationMessages() {
    //checkSignedIn(true);
    var responseDiv = document.getElementById("response");
    HTTPGet("notifications", function (status, response) {
        try {
            var notificationMessages = ParseJSON_LD(response);
            if (notificationMessages === null)
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch notification messages from server!</div>";
            else {
                var notificationMessagesDiv = document.getElementById('notificationMessages');
                var messages = notificationMessagesDiv.querySelector('#messages');
                if (messages !== null && notificationMessages.length > 0) {
                    for (var _i = 0, notificationMessages_1 = notificationMessages; _i < notificationMessages_1.length; _i++) {
                        var notificationMessage = notificationMessages_1[_i];
                        var notificationMessageDiv = messages.appendChild(document.createElement('a'));
                        notificationMessageDiv.className = "searchResult notificationMessage";
                        notificationMessageDiv.href = "notificationMessages/" + notificationMessage["@id"];
                        var typeDiv = notificationMessageDiv.appendChild(document.createElement('div'));
                        typeDiv.className = "type";
                        typeDiv.innerHTML = notificationMessage.type;
                        if (notificationMessage.data !== undefined) {
                            var dataDiv = notificationMessageDiv.appendChild(document.createElement('div'));
                            dataDiv.className = "data";
                            dataDiv.innerText = JSON.stringify(notificationMessage.data);
                        }
                    }
                }
            }
        }
        catch (e) {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch notification data from server!</div>";
        }
    }, function (HTTPStatus, status, response) {
        responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch notification data from server!</div>";
    });
}
//# sourceMappingURL=messages.js.map