///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />

function StartUserNotificationSettings() {

    let   notificationGroups                 = [];
    let   userInfos                          = {}
    const DashboardNotification_Context      = "https://opendata.social/contexts/UsersAPI+json/DashboardNotification";
    const TelegramNotification_Context       = "https://opendata.social/contexts/UsersAPI+json/TelegramNotification";
    const TelegramGroupNotification_Context  = "https://opendata.social/contexts/UsersAPI+json/TelegramGroupNotification";
    const SMSNotification_Context            = "https://opendata.social/contexts/UsersAPI+json/SMSNotification";
    const HTTPSNotification_Context          = "https://opendata.social/contexts/UsersAPI+json/HTTPSNotification";
    const EMailNotification_Context          = "https://opendata.social/contexts/UsersAPI+json/EMailNotification";
    let   notificationsCounter               = 1;

    function ShowNotification(parentDiv: HTMLDivElement, JSON: any)
    {

        function ShowDashboardNotification(parentDiv: HTMLAnchorElement, JSON: Object) {

            var typeDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            typeDiv.className = "type"
            typeDiv.innerText = "Dashboard Notification";

            var valueDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            valueDiv.className = "infos"
            valueDiv.innerHTML = "<i class=\"fas fa-chart-line\"></i> " + JSON["name"];

            if (JSON["description"] != null) {
                var descriptionDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                descriptionDiv.className = "description";
                descriptionDiv.innerText = JSON["description"];
            }

        }

        function ShowTelegramNotification(parentDiv: HTMLAnchorElement, JSON: Object) {

            var typeDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            typeDiv.className = "type"
            typeDiv.innerText = "Telegram Notification";

            var valueDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            valueDiv.className = "infos"
            valueDiv.innerHTML = "<i class=\"fab fa-telegram-plane\"></i> " + JSON["username"];

            if (JSON["description"] != null) {
                var descriptionDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                descriptionDiv.className = "description";
                descriptionDiv.innerText = JSON["description"];
            }

        }

        function ShowTelegramGroupNotification(parentDiv: HTMLAnchorElement, JSON: Object) {

            var typeDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            typeDiv.className = "type"
            typeDiv.innerText = "Telegram Notification";

            var valueDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            valueDiv.className = "infos"
            valueDiv.innerHTML = "<i class=\"fab fa-telegram-plane\"></i> " + JSON["groupName"];

            if (JSON["description"] != null) {
                var descriptionDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                descriptionDiv.className = "description";
                descriptionDiv.innerText = JSON["description"];
            }

        }

        function ShowSMSNotification(parentDiv: HTMLAnchorElement, JSON: Object) {

            var typeDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            typeDiv.className = "type"
            typeDiv.innerText = "SMS Notification";

            var valueDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            valueDiv.className = "infos"
            valueDiv.innerHTML = "<i class=\"fas fa-mobile-alt\"></i> " + JSON["phoneNumber"];

            if (JSON["description"] != null) {
                var descriptionDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                descriptionDiv.className = "description";
                descriptionDiv.innerText = JSON["description"];
            }

        }

        function ShowHTTPSNotification(parentDiv: HTMLAnchorElement, JSON: Object) {

            var typeDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            typeDiv.className = "type"
            typeDiv.innerText = "HTTPS Notification";

            var valueDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            valueDiv.className = "infos";
            valueDiv.innerHTML = "<i class=\"fas fa-globe\"></i> " + JSON["URL"];

            if (JSON["APIKey"] != null && JSON["APIKey"] != "") {
                var APIKeyDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                APIKeyDiv.className = "parameter";
                APIKeyDiv.innerText = "APIKey: " + JSON["APIKey"];
            }

            if (JSON["basicAuth"] != null && JSON["basicAuth"] != "") {
                var basicAuthDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                basicAuthDiv.className = "parameter";
                basicAuthDiv.innerHTML = "Basic Authentication '" + JSON["basicAuth"]["login"] + "' / '" + JSON["basicAuth"]["password"];
            }

            if (JSON["description"] != null && JSON["description"] != "") {
                var descriptionDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                descriptionDiv.className = "description";
                descriptionDiv.innerText = "Description: " + JSON["description"];
            }

        }

        function ShowEMailNotification(parentDiv: HTMLAnchorElement, JSON: Object) {

            // {
            //     "type": "EMailNotification",
            //     "email": {
            //         "ownerName": "Achim Friedland (CardiLogs)",
            //         "address":   "cardilogs@graphdefined.com"
            //     },
            //     "messageTypes": [
            //         "DAILY self tests received",
            //         "MONTHLY self tests received"
            //     ]
            // }

            var typeDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            typeDiv.className = "type"
            typeDiv.innerText = "E-Mail Notification";

            var valueDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            valueDiv.className = "infos"
            valueDiv.innerHTML = "<i class=\"far fa-envelope\"></i> " + JSON["email"]["ownerName"] + " &lt;" + JSON["email"]["address"] + "&gt;";

            if (JSON["description"] != null) {
                var descriptionDiv = parentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                descriptionDiv.className = "description";
                descriptionDiv.innerText = JSON["description"];
            }

        }


        var valueDiv = parentDiv.appendChild(document.createElement('a')) as HTMLAnchorElement;
        valueDiv.className = "notification";
        valueDiv.href      = "/notifications/" + notificationsCounter++;

        switch (JSON["@context"]) {

            case DashboardNotification_Context:
                ShowDashboardNotification(valueDiv, JSON)
                break;

            case TelegramNotification_Context:
                ShowTelegramNotification(valueDiv, JSON)
                break;

            case TelegramGroupNotification_Context:
                ShowTelegramGroupNotification(valueDiv, JSON)
                break;

            case SMSNotification_Context:
                ShowSMSNotification(valueDiv, JSON)
                break;

            case HTTPSNotification_Context:
                ShowHTTPSNotification(valueDiv, JSON)
                break;

            case EMailNotification_Context:
                ShowEMailNotification(valueDiv, JSON)
                break;

        }

        if (JSON.messageTypes != null && valueDiv != null) {

            var messageTypesDiv = valueDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            messageTypesDiv.className = "messageTypes";

            for (var messageType of JSON.messageTypes) {
                var messageTypeDiv = messageTypesDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                messageTypeDiv.className = "messageType";
                messageTypeDiv.innerHTML = messageType;
            }

        }

    }


    checkSignedIn(true);

    HTTPGet("/users/" + SignInUser + "/notifications",

            (status, response) => {

                try
                {

                    const responseJSON  = JSON.parse(response);

                    notificationGroups  = responseJSON.notificationGroups;
                    userInfos           = responseJSON.user;

                    if (notificationGroups == null || userInfos == null || responseJSON.notifications == null)
                        responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch notification data from server!</div>";

                    else
                    {

                        const showNotificationsDiv = document.getElementById('showNotifications') as HTMLDivElement;
                        showNotificationsDiv.innerText = "";

                        if (showNotificationsDiv != null && responseJSON.notifications.length > 0) {
                            for (let i = 0, len = responseJSON.notifications.length; i < len; i++)
                                ShowNotification(showNotificationsDiv, responseJSON.notifications[i]);
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



    const responseDiv            = document.getElementById("response")               as HTMLDivElement;
    const newNotificationButton  = document.getElementById("newNotificationButton")  as HTMLButtonElement;

    newNotificationButton.onclick = () => {
        const redirectURL = document.location.href.substring(0, document.location.href.lastIndexOf("/"));
        document.location.href = redirectURL + "/newNotification";
    }

}
