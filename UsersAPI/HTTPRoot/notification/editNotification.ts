///<reference path="../libs/date.format.ts" />
///<reference path="../defaults/views.ts" />

function StartEditUserNotifications() {

    const pathElements                              = window.location.pathname.split("/");
    const notificationId                            = pathElements[pathElements.length - 1];
    let   NotificationTypeSelect:HTMLSelectElement  = null;

    let   notification:any                          = {}
    const newNotification_Context                   = "https://opendata.social/contexts/UsersAPI+json/newNotification";
    const DashboardNotification_Context             = "https://opendata.social/contexts/UsersAPI+json/DashboardNotification";
    const TelegramNotification_Context              = "https://opendata.social/contexts/UsersAPI+json/TelegramNotification";
    const TelegramGroupNotification_Context         = "https://opendata.social/contexts/UsersAPI+json/TelegramGroupNotification";
    const SMSNotification_Context                   = "https://opendata.social/contexts/UsersAPI+json/SMSNotification";
    const HTTPSNotification_Context                 = "https://opendata.social/contexts/UsersAPI+json/HTTPSNotification";
    const EMailNotification_Context                 = "https://opendata.social/contexts/UsersAPI+json/EMailNotification";


    checkSignedIn(true);

    HTTPGet("/users/" + SignInUser + "/notifications/" + notificationId,

            (HTTPStatus, ResponseText) => {

                try {

                    notification = JSON.parse(ResponseText);

                    if (notification["@context"] == null)
                        responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch notification data from server!</div>";

                    else
                        AddOrEditNotificationView(notification);

                }
                catch (e)
                {
                    responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch notification data from server!</div>";
                }

            },

            (HTTPStatus, StatusText, ResponseText) => {
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch notification data from server!</div>";
            });


    function AddOrEditNotificationView(existingNotification: any) {

        function SaveOrDeleteNotification(Delete: boolean) {

            let messageTypes           = [];
            let messageTypesList       = document.getElementsByClassName('messageType');
            for (var i = 0; i < messageTypesList.length; i++) {

                if (messageTypesList[i].children[0].classList.contains("on"))
                    messageTypes.push(messageTypesList[i].id);

            }

            let newNotificationJSON    = {};
            let DescriptionValue       = (document.getElementById('DescriptionValue').children[0] as HTMLInputElement).value;

            switch (existingNotification != null && existingNotification["@context"] != newNotification_Context
                        ? existingNotification["@context"]
                        : NotificationTypeSelect.selectedOptions[0].innerText)
            {

                case "Dashboard Notification":
                case DashboardNotification_Context:
                    newNotificationJSON = {
                        "@context":       DashboardNotification_Context,
                        "name":           (document.getElementById('DashboardValue').children[0] as HTMLInputElement).value
                    }

                    break;

                case "Telegram Notification":
                case TelegramNotification_Context:
                    newNotificationJSON = {
                        "@context": TelegramNotification_Context,
                        "username": (document.getElementById('TelegramValue').children[0] as HTMLInputElement).value
                    }

                    break;

                case "Telegram Group Notification":
                case TelegramGroupNotification_Context:
                    newNotificationJSON = {
                        "@context":       TelegramGroupNotification_Context,
                        "groupName":      (document.getElementById('TelegramValue').children[0] as HTMLInputElement).value
                    }

                    break;

                case "SMS Notification":
                case SMSNotification_Context:
                    newNotificationJSON = {
                        "@context":       SMSNotification_Context,
                        "phoneNumber":    (document.getElementById('SMSValue').children[0] as HTMLInputElement).value
                    }

                    break;

                case "HTTPS Notification":
                case HTTPSNotification_Context:
                    newNotificationJSON = {
                        "@context":       HTTPSNotification_Context,
                        "URL":            (document.getElementById('URLValue').children[0] as HTMLInputElement).value
                    }

                    let APIKey = (document.getElementById('APIKeyValue').children[0] as HTMLInputElement).value;
                    if (APIKey != null && APIKey != "")
                        newNotificationJSON["APIKey"] = APIKey;

                    let basicAuthLogin    = (document.getElementById('BasicAuthLoginValue').children[0]    as HTMLInputElement).value;
                    let basicAuthPassword = (document.getElementById('BasicAuthPasswordValue').children[0] as HTMLInputElement).value;

                    if (basicAuthLogin != null && basicAuthLogin != "" && basicAuthPassword != null && basicAuthPassword != "") {
                        newNotificationJSON["basicAuth"]              = {};
                        newNotificationJSON["basicAuth"]["login"]     = basicAuthLogin;
                        newNotificationJSON["basicAuth"]["password"]  = basicAuthPassword;
                    }

                    break;

                case "E-Mail Notification":
                case EMailNotification_Context:
                    newNotificationJSON = {
                        "@context":       EMailNotification_Context,
                        "email": {
                            "ownerName":  (document.getElementById('NameValue').children[0]        as HTMLInputElement).value,
                            "address":    (document.getElementById('EMailValue').children[0]       as HTMLInputElement).value
                        }
                    }

                    break;

            }

            if (messageTypes.length > 0)
                newNotificationJSON["messageTypes"] = messageTypes;

            if (DescriptionValue != null && DescriptionValue != "")
                newNotificationJSON["description"] = DescriptionValue;


            HTTP(Delete ? "DELETE" : "SET",
                 "/users/" + SignInUser + "/notifications",
                 [ newNotificationJSON ],

                    (HTTPStatus, ResponseText) => {

                        try {

                            //var responseJSON = JSON.parse(ResponseText);
                            responseDiv.innerHTML = "<div class=\"HTTP OK\">Successfully stored updated notification data.</div>";

                            setTimeout(function () {

                                let redirectURL = document.location.href.substring(0, document.location.href.lastIndexOf("/"));
                                redirectURL     = redirectURL.substring(0, redirectURL.lastIndexOf("/"));

                                document.location.href = redirectURL + "/notifications";

                            }, 2000);

                        }
                        catch (e) {
                            responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing notification data failed!</div>";
                        }

                    },

                    (HTTPStatus, StatusText, ResponseText) => {

                        var responseJSON = { "description": "HTTP Error " + HTTPStatus + " - " + StatusText + "!" };

                        if (ResponseText != null && ResponseText != "") {
                            try
                            {
                                responseJSON = JSON.parse(ResponseText);
                            }
                            catch { }
                        }

                        responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing notification data failed!<br />" + (responseJSON.description != null ? responseJSON.description : "") + "</div>";

                    }

            );

        }

        var AddOrEditNotificationDiv = document.getElementById('AddOrEditNotification')  as HTMLDivElement;

        AddOrEditNotificationDiv.querySelector("#headline #title").innerHTML = (existingNotification == null || existingNotification["@context"] === newNotification_Context) ? "Create a new notification" : "Edit notification";

        var AddNotificationsView    = new View(AddOrEditNotificationDiv.querySelector("#notification"));

        var NotificationValue       = (existingNotification == null || existingNotification["@context"] === newNotification_Context)
                                          ? AddNotificationsView.CreateRow("NotificationType", "Notification type")
                                          : null;

        var DescriptionValue        = AddNotificationsView.CreateRow   ("Description",
                                                                        "Description", _ => _.innerHTML = "<input placeholder=\"Some description for you to remember why you created this notification...\" />");


        var DashboardGroup          = AddNotificationsView.CreateGroup ("Dashboard",         false);
        var DashboardNameValue      = AddNotificationsView.CreateRow   ("Dashboard",         "Dashboard Name",        _ => _.innerHTML = "<input placeholder=\"The name of the dashboard...\"/>");

        var TelegramGroup           = AddNotificationsView.CreateGroup ("Telegram",          false);
        var TelegramUsernameValue   = AddNotificationsView.CreateRow   ("Telegram",          "Telegram Username",     _ => _.innerHTML = "<input placeholder=\"Your telegram username...\"/>");

        var TelegramGroupGroup      = AddNotificationsView.CreateGroup ("TelegramGroup",     false);
        var TelegramGroupNameValue  = AddNotificationsView.CreateRow   ("TelegramGroup",     "Telegram Group Name",   _ => _.innerHTML = "<input placeholder=\"Your telegram group name...\"/>");

        var SMSGroup                = AddNotificationsView.CreateGroup ("SMS",               false);
        var PhoneNumberValue        = AddNotificationsView.CreateRow   ("SMS",               "Phone number",          _ => _.innerHTML = "<input placeholder=\"Your phone number...\"/>");
        if (notification.user != null && notification.user.phoneNumber != null && notification.user.phoneNumber != "")
            (PhoneNumberValue.children[0] as HTMLInputElement).value = notification.user.phoneNumber;

        var HTTPSGroup              = AddNotificationsView.CreateGroup ("HTTPS",             false);
        var URLValue                = AddNotificationsView.CreateRow   ("URL",               "HTTPS URL",             _ => _.innerHTML = "<input placeholder=\"Your HTTPS URL...\" />");
        var APIKeyValue             = AddNotificationsView.CreateRow   ("APIKey",            "API Key",               _ => _.innerHTML = "<input placeholder=\"An optional API key...\" />");
        var BasicAuthLoginValue     = AddNotificationsView.CreateRow   ("BasicAuthLogin",    "Login",                 _ => _.innerHTML = "<input placeholder=\"An optional login for HTTPS basic authentication...\" />");
        var BasicAuthPasswordValue  = AddNotificationsView.CreateRow   ("BasicAuthPassword", "Password",              _ => _.innerHTML = "<input placeholder=\"An optional password for HTTPS basic authentication...\" />");

        var EMailGroup              = AddNotificationsView.CreateGroup ("EMail",             false);
        var NameValue               = AddNotificationsView.CreateRow   ("Name",              "Your name",             _ => _.innerHTML = "<input placeholder=\"Your name...\" value=\"" + notification.user.name + "\" />");
        var EMailValue              = AddNotificationsView.CreateRow   ("EMail",             "E-Mail address",        _ => _.innerHTML = "<input placeholder=\"Your e-mail address...\" value=\"" + notification.user.email + "\" />");
        var SignedMailsValue        = AddNotificationsView.CreateRow   ("SignMails",         "Sign E-Mails",          _ => _.innerHTML = "<select><option>yes</option><option>no</option></select>");
        var PublicKeyValue          = AddNotificationsView.CreateRow2  ("EncryptMails",      "Encrypt E-Mails", true, _ => _.innerHTML = "<textarea placeholder=\"You can provide an optional GPG/PGP public key to receive encrypted e-mails...\" />");


        var SaveButton              = AddOrEditNotificationDiv.querySelector("#SaveButton")   as HTMLButtonElement;
        var RemoveButton            = AddOrEditNotificationDiv.querySelector("#RemoveButton") as HTMLButtonElement;

        SaveButton.style.display = "block";
        SaveButton.disabled = false;

        SaveButton.onclick = (ev: MouseEvent) => {
            SaveOrDeleteNotification(false);
        };

        if (existingNotification != null && existingNotification["@context"] != newNotification_Context) {

            RemoveButton.style.display = "block";
            RemoveButton.disabled = false;

            RemoveButton.onclick = (ev: MouseEvent) => {
                SaveOrDeleteNotification(true);
            };

        }

        AddNotificationsView.ResetGroup();

        let MessageTypesDiv   = AddNotificationsView.CreateRow2("MessageType", "Notification messages", true).appendChild(document.createElement('div')) as HTMLDivElement;
        MessageTypesDiv.className = "messageTypeGroups";

        for (let notificationGroup of notification.notificationGroups) {

            let MessageTypeGroupDiv = MessageTypesDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            MessageTypeGroupDiv.className = "messageTypeGroup";
            MessageTypeGroupDiv.onclick = () => {

                let subDiv = MessageTypeGroupDiv.lastChild as HTMLDivElement;
                subDiv.style.display = subDiv.style.display == "none"
                                           ? "block"
                                           : "none";

            };

            let MessageTypeGroupHeadlineDiv = MessageTypeGroupDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            MessageTypeGroupHeadlineDiv.className = "headline";

            let MessageTypeGroupTitleDiv = MessageTypeGroupHeadlineDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            MessageTypeGroupTitleDiv.className = "title";
            MessageTypeGroupTitleDiv.innerHTML = firstValue(notificationGroup.title);

            let MessageTypeGroupDescriptionDiv = MessageTypeGroupHeadlineDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            MessageTypeGroupDescriptionDiv.className = "description";
            MessageTypeGroupDescriptionDiv.innerHTML = firstValue(notificationGroup.description);


            let MessageTypeGroupMessagesDiv = MessageTypeGroupDiv.appendChild(document.createElement('div')) as HTMLDivElement;
            MessageTypeGroupMessagesDiv.className = "messageTypes";
            MessageTypeGroupMessagesDiv.style.display = "none";

            for (let MessageType of notificationGroup.notifications) {

                let MessageTypeOption = MessageTypeGroupMessagesDiv.appendChild(document.createElement('div')) as HTMLDivElement;
                MessageTypeOption.id        = MessageType.messages[0];
                MessageTypeOption.dataset.messageType = MessageType.messages[0];
                MessageTypeOption.className = "messageType";
                MessageTypeOption.innerHTML = "<i class=\"fas fa-check\"></i>";

                let MessageTypeOptionText = MessageTypeOption.appendChild(document.createElement('div')) as HTMLDivElement;
                MessageTypeOptionText.className = "text";

                let MessageTypeOptionTitle       = MessageTypeOptionText.appendChild(document.createElement('div')) as HTMLDivElement;
                MessageTypeOptionTitle.className = "title";
                MessageTypeOptionTitle.innerHTML = firstValue(MessageType.title);

                let MessageTypeOptionDescription = MessageTypeOptionText.appendChild(document.createElement('div')) as HTMLDivElement;
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

                }

                if (existingNotification != null &&
                    existingNotification.messageTypes != null &&
                    existingNotification.messageTypes.includes(MessageType.messages[0])) {
                    MessageTypeOption.classList.add("on");
                    MessageTypeOption.children[0].classList.add("on");
                }

            }

        }


        if (existingNotification == null || existingNotification["@context"] === newNotification_Context) {

            NotificationTypeSelect = NotificationValue.appendChild(document.createElement('select')) as HTMLSelectElement;
            NotificationTypeSelect.id = "NotificationSelect";

            let NotificationTypes      = [ "Dashboard Notification",
                                           "Telegram Notification",
                                           "Telegram Group Notification",
                                           "SMS Notification",
                                           "HTTPS Notification",
                                           "E-Mail Notification" ];

            for (let NotificationType in NotificationTypes) {
                let NotificationTypeOption  = NotificationTypeSelect.appendChild(document.createElement('option')) as HTMLOptionElement;
                NotificationTypeOption.text = NotificationTypes[NotificationType];
            }

            NotificationTypeSelect.onchange = () => {

                DashboardGroup.style.display      = "none";
                TelegramGroup.style.display       = "none";
                TelegramGroupGroup.style.display  = "none";
                SMSGroup.style.display            = "none";
                HTTPSGroup.style.display          = "none";
                EMailGroup.style.display          = "none";

                switch (NotificationTypeSelect.selectedIndex) {
                    case 0: DashboardGroup.style.display      = "table-row-group"; break;
                    case 1: TelegramGroup.style.display       = "table-row-group"; break;
                    case 2: TelegramGroupGroup.style.display  = "table-row-group"; break;
                    case 3: SMSGroup.style.display            = "table-row-group"; break;
                    case 4: HTTPSGroup.style.display          = "table-row-group"; break;
                    case 5: EMailGroup.style.display          = "table-row-group"; break;
                }

            };

            NotificationTypeSelect.selectedIndex = 0;
            TelegramGroup.style.display = "table-row-group";

        }

        else
        {

            DashboardGroup.style.direction      = "none";
            TelegramGroup.style.direction       = "none";
            TelegramGroupGroup.style.direction  = "none";
            SMSGroup.style.display              = "none";
            HTTPSGroup.style.display            = "none";
            EMailGroup.style.display            = "none";

            if (existingNotification.description != null && existingNotification.description != "")
                (DescriptionValue.children[0] as HTMLInputElement).value = existingNotification.description;

            switch (existingNotification["@context"]) {

                case DashboardNotification_Context:
                    AddOrEditNotificationDiv.querySelector("#headline #title").innerHTML = "Edit Dashboard Notification";
                    DashboardGroup.style.display = "table-row-group";
                    (DashboardNameValue.children[0] as HTMLInputElement).value = existingNotification["name"];
                    break;

                case TelegramNotification_Context:
                    AddOrEditNotificationDiv.querySelector("#headline #title").innerHTML = "Edit Telegram Notification";
                    TelegramGroup.style.display = "table-row-group";
                    (TelegramUsernameValue.children[0] as HTMLInputElement).value = existingNotification["username"];
                    break;

                case TelegramGroupNotification_Context:
                    AddOrEditNotificationDiv.querySelector("#headline #title").innerHTML = "Edit Telegram Group Notification";
                    TelegramGroupGroup.style.display = "table-row-group";
                    (TelegramGroupNameValue.children[0] as HTMLInputElement).value = existingNotification["groupName"];
                    break;

                case SMSNotification_Context:
                    AddOrEditNotificationDiv.querySelector("#headline #title").innerHTML = "Edit SMS Notification";
                    SMSGroup.style.display = "table-row-group";
                    (PhoneNumberValue.children[0] as HTMLInputElement).value = existingNotification["phoneNumber"];
                    break;

                case HTTPSNotification_Context:
                    AddOrEditNotificationDiv.querySelector("#headline #title").innerHTML = "Edit HTTPS Notification";
                    HTTPSGroup.style.display = "table-row-group";
                    (URLValue.children[0] as HTMLInputElement).value = existingNotification["URL"];

                    if (existingNotification["APIKey"] != null && existingNotification["APIKey"] != "")
                        (APIKeyValue.children[0]            as HTMLInputElement).value = existingNotification["APIKey"];

                    if (existingNotification["basicAuth"] != null) {

                        if (existingNotification["basicAuth"]["login"]    != null && existingNotification["basicAuth"]["login"]    != "")
                            (BasicAuthLoginValue.children[0] as HTMLInputElement).value = existingNotification["basicAuth"]["login"];

                        if (existingNotification["basicAuth"]["password"] != null && existingNotification["basicAuth"]["password"] != "")
                            (BasicAuthPasswordValue.children[0] as HTMLInputElement).value = existingNotification["basicAuth"]["password"];

                    }
                    break;

                case EMailNotification_Context:
                    AddOrEditNotificationDiv.querySelector("#headline #title").innerHTML = "Edit E-Mail Notification";
                    EMailGroup.style.display = "table-row-group";
                    (NameValue.children[0]  as HTMLInputElement).value = existingNotification["email"]["ownerName"];
                    (EMailValue.children[0] as HTMLInputElement).value = existingNotification["email"]["address"];
                    break;

            }

        }

    }


    let responseDiv             = document.getElementById("response")                as HTMLDivElement;

}
