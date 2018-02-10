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

        public static Notifications RegisterHTTPSNotification(this UsersAPI  UsersAPI,
                                                              User_Id        User,
                                                              String         URL)

            => UsersAPI.RegisterNotification(User,
                                             new HTTPSNotification(URL),
                                             (a, b) => a.URL == b.URL);

        public static Notifications RegisterHTTPSNotification(this UsersAPI  UsersAPI,
                                                              User           User,
                                                              String         URL)

            => UsersAPI.RegisterNotification(User.Id,
                                             new HTTPSNotification(URL),
                                             (a, b) => a.URL == b.URL);


        public static Notifications RegisterHTTPSNotification(this UsersAPI    UsersAPI,
                                                              User_Id          User,
                                                              Notification_Id  NotificationId,
                                                              String           URL)

            => UsersAPI.RegisterNotification(User,
                                             NotificationId,
                                             new HTTPSNotification(URL),
                                             (a, b) => a.URL == b.URL);

        public static Notifications RegisterHTTPSNotification(this UsersAPI    UsersAPI,
                                                              User             User,
                                                              Notification_Id  NotificationId,
                                                              String           URL)

            => UsersAPI.RegisterNotification(User.Id,
                                             NotificationId,
                                             new HTTPSNotification(URL),
                                             (a, b) => a.URL == b.URL);




        public static IEnumerable<HTTPSNotification> GetHTTPNotifications(this UsersAPI    UsersAPI,
                                                                          User             User,
                                                                          Notification_Id  NotificationId)


            => UsersAPI.GetNotifications<HTTPSNotification>(User,
                                                            NotificationId);

        public static IEnumerable<HTTPSNotification> GetHTTPNotifications(this UsersAPI    UsersAPI,
                                                                           User_Id          User,
                                                                           Notification_Id  NotificationId)


            => UsersAPI.GetNotifications<HTTPSNotification>(User,
                                                            NotificationId);




        public static Notifications UnregisterHTTPSNotification(this UsersAPI  UsersAPI,
                                                                User           User,
                                                                String         URL)

            => UsersAPI.UnregisterNotification<HTTPSNotification>(User,
                                                                  a => a.URL == URL);

        public static Notifications UnregisterHTTPSNotification(this UsersAPI  UsersAPI,
                                                                User_Id        User,
                                                                String         URL)

            => UsersAPI.UnregisterNotification<HTTPSNotification>(User,
                                                                  a => a.URL == URL);


        public static Notifications UnregisterHTTPSNotification(this UsersAPI    UsersAPI,
                                                                User             User,
                                                                Notification_Id  NotificationId,
                                                                String           URL)

            => UsersAPI.UnregisterNotification<HTTPSNotification>(User,
                                                                  NotificationId,
                                                                  a => a.URL == URL);

        public static Notifications UnregisterHTTPSNotification(this UsersAPI    UsersAPI,
                                                                User_Id          User,
                                                                Notification_Id  NotificationId,
                                                                String           URL)

            => UsersAPI.UnregisterNotification<HTTPSNotification>(User,
                                                                  NotificationId,
                                                                  a => a.URL == URL);

    }


    public class HTTPSNotification : ANotificationType
    {

        public String URL { get; }


        public HTTPSNotification(String URL)
        {
            this.URL = URL;
        }


        public static HTTPSNotification Parse(JObject JSON)
        {

            if (JSON["type"]?.Value<String>() != typeof(HTTPSNotification).Name)
                throw new ArgumentException();

            return new HTTPSNotification(JSON["URL"]?.Value<String>());

        }

        public override JObject GetAsJSON(JObject JSON)
        {

            JSON.Add(new JProperty("type",  GetType().Name));
            JSON.Add(new JProperty("URL",   URL));

            return JSON;

        }

    }

}
