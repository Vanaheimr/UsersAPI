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
                                                              String         URL,
                                                              String         BasicAuth_Login     = null,
                                                              String         BasicAuth_Password  = null)

            => UsersAPI.RegisterNotification(User,
                                             new HTTPSNotification(URL,
                                                                   BasicAuth_Login,
                                                                   BasicAuth_Password),
                                             (a, b) => a.URL                == b.URL &&
                                                       a.BasicAuth_Login    == b.BasicAuth_Login &&
                                                       a.BasicAuth_Password == b.BasicAuth_Password);

        public static Notifications RegisterHTTPSNotification(this UsersAPI  UsersAPI,
                                                              User           User,
                                                              String         URL,
                                                              String         BasicAuth_Login     = null,
                                                              String         BasicAuth_Password  = null)

            => UsersAPI.RegisterNotification(User.Id,
                                             new HTTPSNotification(URL,
                                                                   BasicAuth_Login,
                                                                   BasicAuth_Password),
                                             (a, b) => a.URL                == b.URL &&
                                                       a.BasicAuth_Login    == b.BasicAuth_Login &&
                                                       a.BasicAuth_Password == b.BasicAuth_Password);


        public static Notifications RegisterHTTPSNotification(this UsersAPI    UsersAPI,
                                                              User_Id          User,
                                                              Notification_Id  NotificationId,
                                                              String           URL,
                                                              String           BasicAuth_Login     = null,
                                                              String           BasicAuth_Password  = null)

            => UsersAPI.RegisterNotification(User,
                                             NotificationId,
                                             new HTTPSNotification(URL,
                                                                   BasicAuth_Login,
                                                                   BasicAuth_Password),
                                             (a, b) => a.URL                == b.URL &&
                                                       a.BasicAuth_Login    == b.BasicAuth_Login &&
                                                       a.BasicAuth_Password == b.BasicAuth_Password);

        public static Notifications RegisterHTTPSNotification(this UsersAPI    UsersAPI,
                                                              User             User,
                                                              Notification_Id  NotificationId,
                                                              String           URL,
                                                              String           BasicAuth_Login     = null,
                                                              String           BasicAuth_Password  = null)

            => UsersAPI.RegisterNotification(User.Id,
                                             NotificationId,
                                             new HTTPSNotification(URL,
                                                                   BasicAuth_Login,
                                                                   BasicAuth_Password),
                                             (a, b) => a.URL                == b.URL &&
                                                       a.BasicAuth_Login    == b.BasicAuth_Login &&
                                                       a.BasicAuth_Password == b.BasicAuth_Password);




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
                                                                String         URL,
                                                                String         BasicAuth_Login     = null,
                                                                String         BasicAuth_Password  = null)

            => UsersAPI.UnregisterNotification<HTTPSNotification>(User,
                                                                  a => a.URL                == URL &&
                                                                       a.BasicAuth_Login    == BasicAuth_Login &&
                                                                       a.BasicAuth_Password == BasicAuth_Password);


        public static Notifications UnregisterHTTPSNotification(this UsersAPI  UsersAPI,
                                                                User_Id        User,
                                                                String         URL,
                                                                String         BasicAuth_Login     = null,
                                                                String         BasicAuth_Password  = null)

            => UsersAPI.UnregisterNotification<HTTPSNotification>(User,
                                                                  a => a.URL                == URL &&
                                                                       a.BasicAuth_Login    == BasicAuth_Login &&
                                                                       a.BasicAuth_Password == BasicAuth_Password);


        public static Notifications UnregisterHTTPSNotification(this UsersAPI    UsersAPI,
                                                                User             User,
                                                                Notification_Id  NotificationId,
                                                                String           URL,
                                                                String           BasicAuth_Login     = null,
                                                                String           BasicAuth_Password  = null)

            => UsersAPI.UnregisterNotification<HTTPSNotification>(User,
                                                                  NotificationId,
                                                                  a => a.URL                == URL &&
                                                                       a.BasicAuth_Login    == BasicAuth_Login &&
                                                                       a.BasicAuth_Password == BasicAuth_Password);

        public static Notifications UnregisterHTTPSNotification(this UsersAPI    UsersAPI,
                                                                User_Id          User,
                                                                Notification_Id  NotificationId,
                                                                String           URL,
                                                                String           BasicAuth_Login     = null,
                                                                String           BasicAuth_Password  = null)

            => UsersAPI.UnregisterNotification<HTTPSNotification>(User,
                                                                  NotificationId,
                                                                  a => a.URL                == URL &&
                                                                       a.BasicAuth_Login    == BasicAuth_Login &&
                                                                       a.BasicAuth_Password == BasicAuth_Password);

    }


    public class HTTPSNotification : ANotificationType
    {

        public String URL                  { get; }
        public String BasicAuth_Login      { get; }
        public String BasicAuth_Password   { get; }


        public HTTPSNotification(String URL,
                                 String BasicAuth_Login     = null,
                                 String BasicAuth_Password  = null)
        {

            this.URL                 = URL;
            this.BasicAuth_Login     = BasicAuth_Login;
            this.BasicAuth_Password  = BasicAuth_Password;

        }


        public static HTTPSNotification Parse(JObject JSON)
        {

            if (JSON["type"]?.Value<String>() != typeof(HTTPSNotification).Name)
                throw new ArgumentException();

            return new HTTPSNotification(
                       JSON["URL"]?.Value<String>(),
                       JSON["basicAuth"]?["login"]?.   Value<String>(),
                       JSON["basicAuth"]?["password"]?.Value<String>()
                   );

        }

        public override JObject GetAsJSON(JObject JSON)
        {

            JSON.Add(new JProperty("type",  GetType().Name));
            JSON.Add(new JProperty("URL",   URL));

            if (BasicAuth_Login.IsNotNullOrEmpty() &&
                BasicAuth_Password.IsNotNullOrEmpty())
            {
                JSON.Add(new JProperty("basicAuth",
                             new JObject(
                                 new JProperty("login",    BasicAuth_Login),
                                 new JProperty("password", BasicAuth_Password)
                             )
                        ));
            }

            return JSON;

        }

    }

}
