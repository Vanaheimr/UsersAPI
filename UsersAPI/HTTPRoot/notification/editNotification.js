///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
function StartEditUserNotifications() {
    var pathElements = window.location.pathname.split("/");
    var notificationId = pathElements.length > 2 ? pathElements[pathElements.length - 1] : "";
    var NotificationTypeSelect = null;
    var notification = {};
    var newNotification_Context = "https://opendata.social/contexts/UsersAPI+json/newNotification";
    var DashboardNotification_Context = "https://opendata.social/contexts/UsersAPI+json/DashboardNotification";
    var TelegramNotification_Context = "https://opendata.social/contexts/UsersAPI+json/TelegramNotification";
    var TelegramGroupNotification_Context = "https://opendata.social/contexts/UsersAPI+json/TelegramGroupNotification";
    var SMSNotification_Context = "https://opendata.social/contexts/UsersAPI+json/SMSNotification";
    var HTTPSNotification_Context = "https://opendata.social/contexts/UsersAPI+json/HTTPSNotification";
    var EMailNotification_Context = "https://opendata.social/contexts/UsersAPI+json/EMailNotification";
    var responseDiv = document.getElementById("response");
    checkSignedIn(true);
    HTTPGet(notificationId != ""
        ? "/users/" + SignInUser + "/notifications/" + notificationId
        : "/newNotification", function (status, response) {
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
    }, function (HTTPStatus, status, response) {
        responseDiv.innerHTML = "<div class=\"HTTP Error\">Could not fetch notification data from server!</div>";
    });
    function AddOrEditNotificationView(existingNotification) {
        function SaveOrDeleteNotification(Delete) {
            var messageTypes = [];
            var messageTypesList = document.getElementsByClassName('messageType');
            for (var i = 0; i < messageTypesList.length; i++) {
                if (messageTypesList[i].children[0].classList.contains("on"))
                    messageTypes.push(messageTypesList[i].id);
            }
            var newNotificationJSON = {};
            var DescriptionValue = document.getElementById('DescriptionValue').children[0].value;
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
                    var APIKey_1 = document.getElementById('APIKeyValue').children[0].value;
                    if (APIKey_1 != null && APIKey_1 != "")
                        newNotificationJSON["APIKey"] = APIKey_1;
                    var basicAuthLogin = document.getElementById('BasicAuthLoginValue').children[0].value;
                    var basicAuthPassword = document.getElementById('BasicAuthPasswordValue').children[0].value;
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
            HTTP(Delete ? "DELETE" : "SET", "/users/" + SignInUser + "/notifications", [newNotificationJSON], function (status, response) {
                try {
                    //const responseJSON = JSON.parse(ResponseText);
                    responseDiv.innerHTML = Delete
                        ? "<div class=\"HTTP OK\">Successfully removed notification.</div>"
                        : "<div class=\"HTTP OK\">Successfully stored updated notification.</div>";
                    setTimeout(function () {
                        var redirectURL = document.location.href.substring(0, document.location.href.lastIndexOf("/"));
                        redirectURL = redirectURL.substring(0, redirectURL.lastIndexOf("/"));
                        document.location.href = "/notifications";
                    }, 2000);
                }
                catch (e) {
                    responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing notification data failed!</div>";
                }
            }, function (HTTPStatus, status, response) {
                var responseJSON = { "description": "HTTP Error " + HTTPStatus + " - " + status + "!" };
                if (response != null && response != "") {
                    try {
                        responseJSON = JSON.parse(response);
                    }
                    catch (_a) { }
                }
                responseDiv.innerHTML = "<div class=\"HTTP Error\">Storing notification data failed!<br />" + (responseJSON.description != null ? responseJSON.description : "") + "</div>";
            });
        }
        var AddOrEditNotificationDiv = document.getElementById('AddOrEditNotification');
        AddOrEditNotificationDiv.querySelector("#headline #title").innerHTML = (existingNotification == null || existingNotification["@context"] === newNotification_Context) ? "Create a new notification" : "Edit notification";
        var AddNotificationsView = new View(AddOrEditNotificationDiv.querySelector("#notification"));
        var NotificationValue = (existingNotification == null || existingNotification["@context"] === newNotification_Context)
            ? AddNotificationsView.CreateRow("NotificationType", "Notification type")
            : null;
        var DescriptionValue = AddNotificationsView.CreateRow("Description", "Description", function (_) { return _.innerHTML = "<input placeholder=\"Some description for you to remember why you created this notification...\" />"; });
        var DashboardGroup = AddNotificationsView.CreateGroup("Dashboard", false);
        var DashboardNameValue = AddNotificationsView.CreateRow("Dashboard", "Dashboard Name", function (_) { return _.innerHTML = "<input placeholder=\"The name of the dashboard...\"/>"; });
        var TelegramGroup = AddNotificationsView.CreateGroup("Telegram", false);
        var TelegramUsernameValue = AddNotificationsView.CreateRow("Telegram", "Telegram Username", function (_) { return _.innerHTML = "<input placeholder=\"Your telegram username...\"/>"; });
        var TelegramGroupGroup = AddNotificationsView.CreateGroup("TelegramGroup", false);
        var TelegramGroupNameValue = AddNotificationsView.CreateRow("TelegramGroup", "Telegram Group Name", function (_) { return _.innerHTML = "<input placeholder=\"Your telegram group name...\"/>"; });
        var SMSGroup = AddNotificationsView.CreateGroup("SMS", false);
        var PhoneNumberValue = AddNotificationsView.CreateRow("SMS", "Phone number", function (_) { return _.innerHTML = "<input placeholder=\"Your phone number...\"/>"; });
        if (notification.user != null && notification.user.phoneNumber != null && notification.user.phoneNumber != "")
            PhoneNumberValue.children[0].value = notification.user.phoneNumber;
        var HTTPSGroup = AddNotificationsView.CreateGroup("HTTPS", false);
        var URLValue = AddNotificationsView.CreateRow("URL", "HTTPS URL", function (_) { return _.innerHTML = "<input placeholder=\"Your HTTPS URL...\" />"; });
        var APIKeyValue = AddNotificationsView.CreateRow("APIKey", "API Key", function (_) { return _.innerHTML = "<input placeholder=\"An optional API key...\" />"; });
        var BasicAuthLoginValue = AddNotificationsView.CreateRow("BasicAuthLogin", "Login", function (_) { return _.innerHTML = "<input placeholder=\"An optional login for HTTPS basic authentication...\" />"; });
        var BasicAuthPasswordValue = AddNotificationsView.CreateRow("BasicAuthPassword", "Password", function (_) { return _.innerHTML = "<input placeholder=\"An optional password for HTTPS basic authentication...\" />"; });
        var EMailGroup = AddNotificationsView.CreateGroup("EMail", false);
        var NameValue = AddNotificationsView.CreateRow("Name", "Your name", function (_) { return _.innerHTML = "<input placeholder=\"Your name...\" value=\"" + notification.user.name + "\" />"; });
        var EMailValue = AddNotificationsView.CreateRow("EMail", "E-Mail address", function (_) { return _.innerHTML = "<input placeholder=\"Your e-mail address...\" value=\"" + notification.user.email + "\" />"; });
        var SignedMailsValue = AddNotificationsView.CreateRow("SignMails", "Sign E-Mails", function (_) { return _.innerHTML = "<select><option>yes</option><option>no</option></select>"; });
        var PublicKeyValue = AddNotificationsView.CreateRow2("EncryptMails", "Encrypt E-Mails", true, function (_) { return _.innerHTML = "<textarea placeholder=\"You can provide an optional GPG/PGP public key to receive encrypted e-mails...\" />"; });
        var SaveButton = AddOrEditNotificationDiv.querySelector("#SaveButton");
        var RemoveButton = AddOrEditNotificationDiv.querySelector("#RemoveButton");
        SaveButton.style.display = "block";
        SaveButton.disabled = false;
        SaveButton.onclick = function () {
            SaveOrDeleteNotification(false);
        };
        if (existingNotification != null && existingNotification["@context"] != newNotification_Context) {
            RemoveButton.style.display = "block";
            RemoveButton.disabled = false;
            RemoveButton.onclick = function () {
                SaveOrDeleteNotification(true);
            };
        }
        AddNotificationsView.ResetGroup();
        var MessageTypesDiv = AddNotificationsView.CreateRow2("MessageType", "Notification messages", true).appendChild(document.createElement('div'));
        MessageTypesDiv.className = "messageTypeGroups";
        var _loop_1 = function (notificationGroup) {
            var MessageTypeGroupDiv = MessageTypesDiv.appendChild(document.createElement('div'));
            MessageTypeGroupDiv.className = "messageTypeGroup";
            MessageTypeGroupDiv.onclick = function (ev) {
                var subDiv = MessageTypeGroupDiv.lastChild;
                subDiv.style.display = subDiv.style.display == "none"
                    ? "block"
                    : "none";
            };
            var MessageTypeGroupHeadlineDiv = MessageTypeGroupDiv.appendChild(document.createElement('div'));
            MessageTypeGroupHeadlineDiv.className = "headline";
            var MessageTypeGroupTitleDiv = MessageTypeGroupHeadlineDiv.appendChild(document.createElement('div'));
            MessageTypeGroupTitleDiv.className = "title";
            MessageTypeGroupTitleDiv.innerHTML = firstValue(notificationGroup.title);
            var MessageTypeGroupDescriptionDiv = MessageTypeGroupHeadlineDiv.appendChild(document.createElement('div'));
            MessageTypeGroupDescriptionDiv.className = "description";
            MessageTypeGroupDescriptionDiv.innerHTML = firstValue(notificationGroup.description);
            var MessageTypeGroupMessagesDiv = MessageTypeGroupDiv.appendChild(document.createElement('div'));
            MessageTypeGroupMessagesDiv.className = "messageTypes";
            MessageTypeGroupMessagesDiv.style.display = "none";
            var _loop_2 = function (MessageType) {
                var MessageTypeOption = MessageTypeGroupMessagesDiv.appendChild(document.createElement('div'));
                MessageTypeOption.id = MessageType.messages[0];
                MessageTypeOption.dataset.messageType = MessageType.messages[0];
                MessageTypeOption.className = "messageType";
                MessageTypeOption.innerHTML = "<i class=\"fas fa-check\"></i>";
                var MessageTypeOptionText = MessageTypeOption.appendChild(document.createElement('div'));
                MessageTypeOptionText.className = "text";
                var MessageTypeOptionTitle = MessageTypeOptionText.appendChild(document.createElement('div'));
                MessageTypeOptionTitle.className = "title";
                MessageTypeOptionTitle.innerHTML = firstValue(MessageType.title);
                var MessageTypeOptionDescription = MessageTypeOptionText.appendChild(document.createElement('div'));
                MessageTypeOptionDescription.className = "description";
                MessageTypeOptionDescription.innerHTML = firstValue(MessageType.description);
                MessageTypeOption.onclick = function (ev) {
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
            };
            for (var _i = 0, _a = notificationGroup.notifications; _i < _a.length; _i++) {
                var MessageType = _a[_i];
                _loop_2(MessageType);
            }
        };
        for (var _i = 0, _a = notification.notificationGroups; _i < _a.length; _i++) {
            var notificationGroup = _a[_i];
            _loop_1(notificationGroup);
        }
        if (existingNotification == null || existingNotification["@context"] === newNotification_Context) {
            NotificationTypeSelect = NotificationValue.appendChild(document.createElement('select'));
            NotificationTypeSelect.id = "NotificationSelect";
            var NotificationTypes = ["Dashboard Notification",
                "Telegram Notification",
                "Telegram Group Notification",
                "SMS Notification",
                "HTTPS Notification",
                "E-Mail Notification"];
            for (var NotificationType in NotificationTypes) {
                var NotificationTypeOption = NotificationTypeSelect.appendChild(document.createElement('option'));
                NotificationTypeOption.text = NotificationTypes[NotificationType];
            }
            NotificationTypeSelect.onchange = function () {
                DashboardGroup.style.display = "none";
                TelegramGroup.style.display = "none";
                TelegramGroupGroup.style.display = "none";
                SMSGroup.style.display = "none";
                HTTPSGroup.style.display = "none";
                EMailGroup.style.display = "none";
                switch (NotificationTypeSelect.selectedIndex) {
                    case 0:
                        DashboardGroup.style.display = "table-row-group";
                        break;
                    case 1:
                        TelegramGroup.style.display = "table-row-group";
                        break;
                    case 2:
                        TelegramGroupGroup.style.display = "table-row-group";
                        break;
                    case 3:
                        SMSGroup.style.display = "table-row-group";
                        break;
                    case 4:
                        HTTPSGroup.style.display = "table-row-group";
                        break;
                    case 5:
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