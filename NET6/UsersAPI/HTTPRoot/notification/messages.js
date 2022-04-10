///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
function StartUserNotificationMessages() {
    //checkSignedIn(true);
    let responseDiv = document.getElementById("response");
    HTTPGet("notifications", (status, response) => {
        try {
            const notificationMessages = ParseJSON_LD(response);
            if (notificationMessages === null)
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch notification messages from server!</div>";
            else {
                const notificationMessagesDiv = document.getElementById('notificationMessages');
                const messages = notificationMessagesDiv.querySelector('#messages');
                if (messages !== null && notificationMessages.length > 0) {
                    for (const notificationMessage of notificationMessages) {
                        const notificationMessageDiv = messages.appendChild(document.createElement('a'));
                        notificationMessageDiv.className = "searchResult notificationMessage";
                        notificationMessageDiv.href = "notificationMessages/" + notificationMessage["@id"];
                        const typeDiv = notificationMessageDiv.appendChild(document.createElement('div'));
                        typeDiv.className = "type";
                        typeDiv.innerHTML = notificationMessage.type;
                        if (notificationMessage.data !== undefined) {
                            const dataDiv = notificationMessageDiv.appendChild(document.createElement('div'));
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
    }, (HTTPStatus, status, response) => {
        responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch notification data from server!</div>";
    });
}
//# sourceMappingURL=messages.js.map