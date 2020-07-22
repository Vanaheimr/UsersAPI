///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />

function StartUserNotificationMessages() {

    //checkSignedIn(true);

    let responseDiv = document.getElementById("response") as HTMLDivElement;


    HTTPGet("notifications",

            (status, response) => {

                try
                {

                    const notificationMessages = ParseJSON_LD<Array<INotificationMessage>>(response);

                    if (notificationMessages === null)
                        responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch notification messages from server!</div>";

                    else
                    {

                        const notificationMessagesDiv  = document.               getElementById('notificationMessages')  as HTMLDivElement;
                        const messages                 = notificationMessagesDiv.querySelector ('#messages')             as HTMLDivElement;
 
                        if (messages !== null && notificationMessages.length > 0) {
                            for (const notificationMessage of notificationMessages) {

                                const notificationMessageDiv = messages.appendChild(document.createElement('a')) as HTMLAnchorElement;
                                notificationMessageDiv.className = "searchResult notificationMessage";
                                notificationMessageDiv.href      = "notificationMessages/" + notificationMessage["@id"];

                                const typeDiv = notificationMessageDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                typeDiv.className = "type"
                                typeDiv.innerHTML = notificationMessage.type;

                                if (notificationMessage.data !== undefined) {
                                    const dataDiv = notificationMessageDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                                    dataDiv.className = "data";
                                    dataDiv.innerText = JSON.stringify(notificationMessage.data);
                                }

                            }
                        }

                    }

                }
                catch (e)
                {
                    responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch notification data from server!</div>";
                }

            },

            (HTTPStatus, status, response) => {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch notification data from server!</div>";
            });

}
