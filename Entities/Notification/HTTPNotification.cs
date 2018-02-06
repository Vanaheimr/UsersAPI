/*
 * Copyright (c) 2014-2018, Achim 'ahzf' Friedland <achim@graphdefined.org>
 * This file is part of OpenDataAPI <http://www.github.com/GraphDefined/OpenDataAPI>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Usings

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.OpenData.Users
{

    public static class HTTPNotificationExtentions
    {

        public static Notifications RegisterHTTPNotification(this UsersAPI  UsersAPI,
                                                              User_Id        User,
                                                              EMailAddress   EMailAddress,
                                                              String         Subject = null)

            => UsersAPI.RegisterNotification(User,
                                             new HTTPNotification(EMailAddress, Subject),
                                             (a, b) => a.EMailAddress == b.EMailAddress);

        public static Notifications RegisterHTTPNotification(this UsersAPI  UsersAPI,
                                                              User           User,
                                                              EMailAddress   EMailAddress,
                                                              String         Subject = null)

            => UsersAPI.RegisterNotification(User.Id,
                                             new HTTPNotification(EMailAddress, Subject),
                                             (a, b) => a.EMailAddress == b.EMailAddress);


        public static Notifications RegisterHTTPNotification(this UsersAPI    UsersAPI,
                                                              User_Id          User,
                                                              Notification_Id  NotificationId,
                                                              EMailAddress     EMailAddress,
                                                              String           Subject = null)

            => UsersAPI.RegisterNotification(User,
                                             NotificationId,
                                             new HTTPNotification(EMailAddress, Subject),
                                             (a, b) => a.EMailAddress == b.EMailAddress);

        public static Notifications RegisterHTTPNotification(this UsersAPI    UsersAPI,
                                                              User             User,
                                                              Notification_Id  NotificationId,
                                                              EMailAddress     EMailAddress,
                                                              String           Subject = null)

            => UsersAPI.RegisterNotification(User.Id,
                                             NotificationId,
                                             new HTTPNotification(EMailAddress, Subject),
                                             (a, b) => a.EMailAddress == b.EMailAddress);




        public static IEnumerable<HTTPNotification> GetHTTPNotifications(this UsersAPI    UsersAPI,
                                                                           User             User,
                                                                           Notification_Id  NotificationId)


            => UsersAPI.GetNotifications<HTTPNotification>(User,
                                                            NotificationId);

        public static IEnumerable<HTTPNotification> GetHTTPNotifications(this UsersAPI    UsersAPI,
                                                                           User_Id          User,
                                                                           Notification_Id  NotificationId)


            => UsersAPI.GetNotifications<HTTPNotification>(User,
                                                            NotificationId);




        public static Notifications UnregisterHTTPNotification(this UsersAPI  UsersAPI,
                                                                User           User,
                                                                EMailAddress   EMailAddress)

            => UsersAPI.UnregisterNotification<HTTPNotification>(User,
                                                                  a => a.EMailAddress == EMailAddress);

        public static Notifications UnregisterHTTPNotification(this UsersAPI  UsersAPI,
                                                                User_Id        User,
                                                                EMailAddress   EMailAddress)

            => UsersAPI.UnregisterNotification<HTTPNotification>(User,
                                                                  a => a.EMailAddress == EMailAddress);


        public static Notifications UnregisterHTTPNotification(this UsersAPI    UsersAPI,
                                                                User             User,
                                                                Notification_Id  NotificationId,
                                                                EMailAddress     EMailAddress)

            => UsersAPI.UnregisterNotification<HTTPNotification>(User,
                                                                  NotificationId,
                                                                  a => a.EMailAddress == EMailAddress);

        public static Notifications UnregisterHTTPNotification(this UsersAPI    UsersAPI,
                                                                User_Id          User,
                                                                Notification_Id  NotificationId,
                                                                EMailAddress     EMailAddress)

            => UsersAPI.UnregisterNotification<HTTPNotification>(User,
                                                                  NotificationId,
                                                                  a => a.EMailAddress == EMailAddress);

    }


    public class HTTPNotification : ANotificationType
    {

        public EMailAddress  EMailAddress   { get; }
        public String        Subject        { get; }


        public HTTPNotification(EMailAddress  EMailAddress,
                                 String        Subject = null)
        {

            this.EMailAddress  = EMailAddress;
            this.Subject       = Subject;

        }


        public static HTTPNotification Parse(JObject JSON)
        {

            if (JSON["type"]?.Value<String>() != typeof(HTTPNotification).Name)
                throw new ArgumentException();

            return new HTTPNotification(EMailAddress.Parse(JSON["email"]?.Value<String>()),
                                         JSON["subject"]?.Value<String>());

        }

        protected override JObject GetAsJSON(JObject JSON)
        {

            JSON.Add(new JProperty("type",   GetType().Name));
            JSON.Add(new JProperty("email",  EMailAddress.ToString()));

            if (Subject.IsNotNullOrEmpty())
                JSON.Add(new JProperty("subject", Subject));

            return JSON;

        }

    }

}
