///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
function StartEditUserNotifications() {
    const pathElements = window.location.pathname.split("/");
    const notificationId = pathElements.length > 2 ? pathElements[pathElements.length - 1] : "";
    let NotificationTypeSelect = null;
    let notification = {};
    const newNotification_Context = "https://opendata.social/contexts/UsersAPI/newNotification";
    const DashboardNotification_Context = "https://opendata.social/contexts/UsersAPI/DashboardNotification";
    const TelegramNotification_Context = "https://opendata.social/contexts/UsersAPI/TelegramNotification";
    const TelegramGroupNotification_Context = "https://opendata.social/contexts/UsersAPI/TelegramGroupNotification";
    const SMSNotification_Context = "https://opendata.social/contexts/UsersAPI/SMSNotification";
    const HTTPSNotification_Context = "https://opendata.social/contexts/UsersAPI/HTTPSNotification";
    const EMailNotification_Context = "https://opendata.social/contexts/UsersAPI/EMailNotification";
    const responseDiv = document.getElementById("response");
    checkSignedIn(true);
    HTTPGet(notificationId != ""
        ? "/users/" + SignInUser + "/notifications/" + notificationId
        : "/newNotification", (status, response) => {
        try {
            notification = JSON.parse(response);
            if (notification["@context"] == null)
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch notification data from server!</div>";
            else
                AddOrEditNotificationView(notification);
        }
        catch (e) {
            responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch notification data from server!</div>";
        }
    }, (HTTPStatus, status, response) => {
        responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch notification data from server!</div>";
    });
    function AddOrEditNotificationView(existingNotification) {
        function SaveOrDeleteNotification(Delete) {
            const messageTypes = [];
            const messageTypesList = document.getElementsByClassName('messageType');
            for (let i = 0; i < messageTypesList.length; i++) {
                if (messageTypesList[i].children[0].classList.contains("on"))
                    messageTypes.push(messageTypesList[i].id);
            }
            let newNotificationJSON = {};
            const DescriptionValue = document.getElementById('DescriptionValue').children[0].value;
            switch (existingNotification != null && existingNotification["@context"] != newNotification_Context
                ? existingNotification["@context"]
                : NotificationTypeSelect.selectedOptions[0].innerText) {
                case "Dashboard Notification":
                case DashboardNotification_Context:
                    newNotificationJSON = {
                        "@context": DashboardNotification_Context,
                        "name": document.getElementById('DashboardValue').children[0].value
                    };
                    break;
                case "Telegram Notification":
                case TelegramNotification_Context:
                    newNotificationJSON = {
                        "@context": TelegramNotification_Context,
                        "username": document.getElementById('TelegramValue').children[0].value
                    };
                    break;
                case "Telegram Group Notification":
                case TelegramGroupNotification_Context:
                    newNotificationJSON = {
                        "@context": TelegramGroupNotification_Context,
                        "groupName": document.getElementById('TelegramValue').children[0].value
                    };
                    break;
                case "SMS Notification":
                case SMSNotification_Context:
                    newNotificationJSON = {
                        "@context": SMSNotification_Context,
                        "phoneNumber": document.getElementById('SMSValue').children[0].value
                    };
                    break;
                case "HTTPS Notification":
                case HTTPSNotification_Context:
                    newNotificationJSON = {
                        "@context": HTTPSNotification_Context,
                        "URL": document.getElementById('URLValue').children[0].value
                    };
                    const APIKey = document.getElementById('APIKeyValue').children[0].value;
                    if (APIKey != null && APIKey != "")
                        newNotificationJSON["APIKey"] = APIKey;
                    const basicAuthLogin = document.getElementById('BasicAuthLoginValue').children[0].value;
                    const basicAuthPassword = document.getElementById('BasicAuthPasswordValue').children[0].value;
                    if (basicAuthLogin != null && basicAuthLogin != "" && basicAuthPassword != null && basicAuthPassword != "") {
                        newNotificationJSON["basicAuth"] = {};
                        newNotificationJSON["basicAuth"]["login"] = basicAuthLogin;
                        newNotificationJSON["basicAuth"]["password"] = basicAuthPassword;
                    }
                    break;
                case "E-Mail Notification":
                case EMailNotification_Context:
                    newNotificationJSON = {
                        "@context": EMailNotification_Context,
                        "email": {
                            "ownerName": document.getElementById('NameValue').children[0].value,
                            "address": document.getElementById('EMailValue').children[0].value
                        }
                    };
                    break;
            }
            if (messageTypes.length > 0)
                newNotificationJSON["messageTypes"] = messageTypes;
            if (DescriptionValue != null && DescriptionValue != "")
                newNotificationJSON["description"] = DescriptionValue;
            HTTP(Delete ? "DELETE" : "SET", "/users/" + SignInUser + "/notifications", [newNotificationJSON], (status, response) => {
                try {
                    //const responseJSON = JSON.parse(ResponseText);
                    responseDiv.innerHTML = Delete
                        ? "<div class=\"HTTP OK\">Successfully removed notification.</div>"
                        : "<div class=\"HTTP OK\">Successfully stored updated notification.</div>";
                    setTimeout(function () {
                        //let redirectURL = document.location.href.substring(0, document.location.href.lastIndexOf("/"));
                        //redirectURL     = redirectURL.substring(0, redirectURL.lastIndexOf("/"));
                        document.location.href = "/notificationSettings";
                    }, 2000);
                }
                catch (e) {
                    responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing notification data failed!</div>";
                }
            }, (HTTPStatus, status, response) => {
                let responseJSON = { "description": "HTTP Error " + HTTPStatus + " - " + status + "!" };
                if (response != null && response != "") {
                    try {
                        responseJSON = JSON.parse(response);
                    }
                    catch (_a) { }
                }
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing notification data failed!<br />" + (responseJSON.description != null ? responseJSON.description : "") + "</div>";
            });
        }
        const AddOrEditNotificationDiv = document.getElementById('AddOrEditNotification');
        AddOrEditNotificationDiv.querySelector("#headline #title").innerHTML = (existingNotification == null || existingNotification["@context"] === newNotification_Context) ? "Create a new notification" : "Edit notification";
        const RemoveButton = AddOrEditNotificationDiv.querySelector("#removeNotificationButton");
        const AddNotificationsView = new View(AddOrEditNotificationDiv.querySelector("#notification"));
        const NotificationValue = (existingNotification == null || existingNotification["@context"] === newNotification_Context)
            ? AddNotificationsView.CreateRow("NotificationType", "Notification type")
            : null;
        const DescriptionValue = AddNotificationsView.CreateRow("Description", "Description", _ => _.innerHTML = "<input placeholder=\"Some description for you to remember why you created this notification...\" />");
        const DashboardGroup = AddNotificationsView.CreateGroup("Dashboard", false);
        const DashboardNameValue = AddNotificationsView.CreateRow("Dashboard", "Dashboard Name", _ => _.innerHTML = "<input placeholder=\"The name of the dashboard...\"/>");
        const TelegramGroup = AddNotificationsView.CreateGroup("Telegram", false);
        const TelegramUsernameValue = AddNotificationsView.CreateRow("Telegram", "Telegram Username", _ => _.innerHTML = "<input placeholder=\"Your telegram username...\"/>");
        const TelegramGroupGroup = AddNotificationsView.CreateGroup("TelegramGroup", false);
        const TelegramGroupNameValue = AddNotificationsView.CreateRow("TelegramGroup", "Telegram Group Name", _ => _.innerHTML = "<input placeholder=\"Your telegram group name...\"/>");
        const SMSGroup = AddNotificationsView.CreateGroup("SMS", false);
        const PhoneNumberValue = AddNotificationsView.CreateRow("SMS", "Phone number", _ => _.innerHTML = "<input placeholder=\"Your phone number...\"/>");
        if (notification.user != null && notification.user.phoneNumber != null && notification.user.phoneNumber != "")
            PhoneNumberValue.children[0].value = notification.user.phoneNumber;
        const HTTPSGroup = AddNotificationsView.CreateGroup("HTTPS", false);
        const URLValue = AddNotificationsView.CreateRow("URL", "HTTPS URL", _ => _.innerHTML = "<input placeholder=\"Your HTTPS URL...\" />");
        const APIKeyValue = AddNotificationsView.CreateRow("APIKey", "API Key", _ => _.innerHTML = "<input placeholder=\"An optional API key...\" />");
        const BasicAuthLoginValue = AddNotificationsView.CreateRow("BasicAuthLogin", "Login", _ => _.innerHTML = "<input placeholder=\"An optional login for HTTPS basic authentication...\" />");
        const BasicAuthPasswordValue = AddNotificationsView.CreateRow("BasicAuthPassword", "Password", _ => _.innerHTML = "<input placeholder=\"An optional password for HTTPS basic authentication...\" />");
        const EMailGroup = AddNotificationsView.CreateGroup("EMail", false);
        const NameValue = AddNotificationsView.CreateRow("Name", "Your name", _ => _.innerHTML = "<input placeholder=\"Your name...\" value=\"" + notification.user.name + "\" />");
        const EMailValue = AddNotificationsView.CreateRow("EMail", "E-Mail address", _ => _.innerHTML = "<input placeholder=\"Your e-mail address...\" value=\"" + notification.user.email + "\" />");
        const SignedMailsValue = AddNotificationsView.CreateRow("SignMails", "Sign E-Mails", _ => _.innerHTML = "<select><option>yes</option><option>no</option></select>");
        const PublicKeyValue = AddNotificationsView.CreateRow2("EncryptMails", "Encrypt E-Mails", true, _ => _.innerHTML = "<textarea placeholder=\"You can provide an optional GPG/PGP public key to receive encrypted e-mails...\" />");
        const SaveButton = AddOrEditNotificationDiv.querySelector("#saveNotificationButton");
        SaveButton.style.display = "block";
        SaveButton.disabled = false;
        SaveButton.onclick = () => {
            SaveOrDeleteNotification(false);
        };
        if (existingNotification != null && existingNotification["@context"] !== newNotification_Context) {
            RemoveButton.style.display = "block";
            RemoveButton.disabled = false;
            RemoveButton.onclick = () => {
                SaveOrDeleteNotification(true);
            };
        }
        AddNotificationsView.ResetGroup();
        const MessageTypesDiv = AddNotificationsView.CreateRow2("MessageType", "Notification messages", true).appendChild(document.createElement('div'));
        MessageTypesDiv.className = "messageTypeGroups";
        for (const notificationGroup of notification.notificationGroups) {
            const MessageTypeGroupDiv = MessageTypesDiv.appendChild(document.createElement('div'));
            MessageTypeGroupDiv.className = "messageTypeGroup";
            MessageTypeGroupDiv.onclick = () => {
                const subDiv = MessageTypeGroupDiv.lastChild;
                subDiv.style.display = subDiv.style.display === "none"
                    ? "block"
                    : "none";
            };
            const MessageTypeGroupHeadlineDiv = MessageTypeGroupDiv.appendChild(document.createElement('div'));
            MessageTypeGroupHeadlineDiv.className = "headline";
            const MessageTypeGroupTitleDiv = MessageTypeGroupHeadlineDiv.appendChild(document.createElement('div'));
            MessageTypeGroupTitleDiv.className = "title";
            MessageTypeGroupTitleDiv.innerHTML = firstValue(notificationGroup.title);
            const MessageTypeGroupDescriptionDiv = MessageTypeGroupHeadlineDiv.appendChild(document.createElement('div'));
            MessageTypeGroupDescriptionDiv.className = "description";
            MessageTypeGroupDescriptionDiv.innerHTML = firstValue(notificationGroup.description);
            const MessageTypeGroupMessagesDiv = MessageTypeGroupDiv.appendChild(document.createElement('div'));
            MessageTypeGroupMessagesDiv.className = "messageTypes";
            MessageTypeGroupMessagesDiv.style.display = "none";
            for (const MessageType of notificationGroup.notifications) {
                const MessageTypeOption = MessageTypeGroupMessagesDiv.appendChild(document.createElement('div'));
                MessageTypeOption.id = MessageType.messages[0];
                MessageTypeOption.dataset.messageType = MessageType.messages[0];
                MessageTypeOption.className = "messageType";
                MessageTypeOption.innerHTML = "<i class=\"fas fa-check\"></i>";
                const MessageTypeOptionText = MessageTypeOption.appendChild(document.createElement('div'));
                MessageTypeOptionText.className = "text";
                const MessageTypeOptionTitle = MessageTypeOptionText.appendChild(document.createElement('div'));
                MessageTypeOptionTitle.className = "title";
                MessageTypeOptionTitle.innerHTML = firstValue(MessageType.title);
                const MessageTypeOptionDescription = MessageTypeOptionText.appendChild(document.createElement('div'));
                MessageTypeOptionDescription.className = "description";
                MessageTypeOptionDescription.innerHTML = firstValue(MessageType.description);
                MessageTypeOption.onclick = (ev) => {
                    // Text...
                    if (MessageTypeOption.classList.contains("on"))
                        MessageTypeOption.classList.remove("on");
                    else
                        MessageTypeOption.classList.add("on");
                    // Green check item...
                    if (MessageTypeOption.children[0].classList.contains("on"))
                        MessageTypeOption.children[0].classList.remove("on");
                    else
                        MessageTypeOption.children[0].classList.add("on");
                    ev.stopPropagation();
                };
                if (existingNotification != null &&
                    existingNotification.messageTypes != null &&
                    existingNotification.messageTypes.includes(MessageType.messages[0])) {
                    MessageTypeOption.classList.add("on");
                    MessageTypeOption.children[0].classList.add("on");
                }
            }
        }
        if (existingNotification == null || existingNotification["@context"] === newNotification_Context) {
            NotificationTypeSelect = NotificationValue.appendChild(document.createElement('select'));
            NotificationTypeSelect.id = "NotificationSelect";
            const NotificationTypes = [
                "Telegram Notification",
                //"Telegram Group Notification",
                "SMS Notification",
                "HTTPS Notification",
                "E-Mail Notification"
            ];
            for (const NotificationType in NotificationTypes) {
                const NotificationTypeOption = NotificationTypeSelect.appendChild(document.createElement('option'));
                NotificationTypeOption.text = NotificationTypes[NotificationType];
            }
            NotificationTypeSelect.onchange = () => {
                //DashboardGroup.style.display      = "none";
                TelegramGroup.style.display = "none";
                //TelegramGroupGroup.style.display  = "none";
                SMSGroup.style.display = "none";
                HTTPSGroup.style.display = "none";
                EMailGroup.style.display = "none";
                switch (NotificationTypeSelect.selectedIndex) {
                    //case 0: DashboardGroup.style.display      = "table-row-group"; break;
                    case 0:
                        TelegramGroup.style.display = "table-row-group";
                        break;
                    //case 2: TelegramGroupGroup.style.display  = "table-row-group"; break;
                    case 1:
                        SMSGroup.style.display = "table-row-group";
                        break;
                    case 2:
                        HTTPSGroup.style.display = "table-row-group";
                        break;
                    case 3:
                        EMailGroup.style.display = "table-row-group";
                        break;
                }
            };
            NotificationTypeSelect.selectedIndex = 0;
            TelegramGroup.style.display = "table-row-group";
        }
        else {
            DashboardGroup.style.direction = "none";
            TelegramGroup.style.direction = "none";
            TelegramGroupGroup.style.direction = "none";
            SMSGroup.style.display = "none";
            HTTPSGroup.style.display = "none";
            EMailGroup.style.display = "none";
            if (existingNotification.description != null && existingNotification.description != "")
                DescriptionValue.children[0].value = existingNotification.description;
            switch (existingNotification["@context"]) {
                case DashboardNotification_Context:
                    AddOrEditNotificationDiv.querySelector("#headline #title").innerHTML = "Edit Dashboard Notification";
                    DashboardGroup.style.display = "table-row-group";
                    DashboardNameValue.children[0].value = existingNotification["name"];
                    break;
                case TelegramNotification_Context:
                    AddOrEditNotificationDiv.querySelector("#headline #title").innerHTML = "Edit Telegram Notification";
                    TelegramGroup.style.display = "table-row-group";
                    TelegramUsernameValue.children[0].value = existingNotification["username"];
                    break;
                case TelegramGroupNotification_Context:
                    AddOrEditNotificationDiv.querySelector("#headline #title").innerHTML = "Edit Telegram Group Notification";
                    TelegramGroupGroup.style.display = "table-row-group";
                    TelegramGroupNameValue.children[0].value = existingNotification["groupName"];
                    break;
                case SMSNotification_Context:
                    AddOrEditNotificationDiv.querySelector("#headline #title").innerHTML = "Edit SMS Notification";
                    SMSGroup.style.display = "table-row-group";
                    PhoneNumberValue.children[0].value = existingNotification["phoneNumber"];
                    break;
                case HTTPSNotification_Context:
                    AddOrEditNotificationDiv.querySelector("#headline #title").innerHTML = "Edit HTTPS Notification";
                    HTTPSGroup.style.display = "table-row-group";
                    URLValue.children[0].value = existingNotification["URL"];
                    if (existingNotification["APIKey"] != null && existingNotification["APIKey"] != "")
                        APIKeyValue.children[0].value = existingNotification["APIKey"];
                    if (existingNotification["basicAuth"] != null) {
                        if (existingNotification["basicAuth"]["login"] != null && existingNotification["basicAuth"]["login"] != "")
                            BasicAuthLoginValue.children[0].value = existingNotification["basicAuth"]["login"];
                        if (existingNotification["basicAuth"]["password"] != null && existingNotification["basicAuth"]["password"] != "")
                            BasicAuthPasswordValue.children[0].value = existingNotification["basicAuth"]["password"];
                    }
                    break;
                case EMailNotification_Context:
                    AddOrEditNotificationDiv.querySelector("#headline #title").innerHTML = "Edit E-Mail Notification";
                    EMailGroup.style.display = "table-row-group";
                    NameValue.children[0].value = existingNotification["email"]["ownerName"];
                    EMailValue.children[0].value = existingNotification["email"]["address"];
                    break;
            }
        }
    }
}
//# sourceMappingURL=editNotification.js.map