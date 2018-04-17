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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.OpenData.Users
{

    /// <summary>
    /// Extention methods for HTTPS Notifications.
    /// </summary>
    public static class HTTPSNotificationExtentions
    {

        #region RegisterHTTPSNotification(this UsersAPI, User,                   URL,         BasicAuth_Login = null, BasicAuth_Password = null)

        public static Notifications RegisterHTTPSNotification(this UsersAPI  UsersAPI,
                                                              User           User,
                                                              String         URL,
                                                              IPPort?        TCPPort             = null,
                                                              String         BasicAuth_Login     = null,
                                                              String         BasicAuth_Password  = null)
        {

            if (User == null)
                throw new ArgumentNullException(nameof(User), "The given user must not be null!");

            return UsersAPI.RegisterHTTPSNotification(User.Id,
                                                      URL,
                                                      TCPPort,
                                                      BasicAuth_Login,
                                                      BasicAuth_Password);

        }

        #endregion

        #region RegisterHTTPSNotification(this UsersAPI, UserId,                 URL,         BasicAuth_Login = null, BasicAuth_Password = null)

        public static Notifications RegisterHTTPSNotification(this UsersAPI  UsersAPI,
                                                              User_Id        UserId,
                                                              String         URL,
                                                              IPPort?        TCPPort             = null,
                                                              String         BasicAuth_Login     = null,
                                                              String         BasicAuth_Password  = null)

            => UsersAPI.RegisterNotification(UserId,
                                             new HTTPSNotification(HTTPMethod.POST,
                                                                   URL,
                                                                   TCPPort,
                                                                   BasicAuth_Login,
                                                                   BasicAuth_Password),
                                             (a, b) => a.URL                == b.URL &&
                                                       a.BasicAuth_Login    == b.BasicAuth_Login &&
                                                       a.BasicAuth_Password == b.BasicAuth_Password);

        #endregion

        #region RegisterHTTPSNotification(this UsersAPI, User,                   Method, URL, BasicAuth_Login = null, BasicAuth_Password = null)

        public static Notifications RegisterHTTPSNotification(this UsersAPI  UsersAPI,
                                                              User           User,
                                                              HTTPMethod     Method,
                                                              String         URL,
                                                              IPPort?        TCPPort             = null,
                                                              String         BasicAuth_Login     = null,
                                                              String         BasicAuth_Password  = null)
        {

            if (User == null)
                throw new ArgumentNullException(nameof(User), "The given user must not be null!");

            return UsersAPI.RegisterHTTPSNotification(User.Id,
                                                      Method,
                                                      URL,
                                                      TCPPort,
                                                      BasicAuth_Login,
                                                      BasicAuth_Password);

        }

        #endregion

        #region RegisterHTTPSNotification(this UsersAPI, UserId,                 Method, URL, BasicAuth_Login = null, BasicAuth_Password = null)

        public static Notifications RegisterHTTPSNotification(this UsersAPI  UsersAPI,
                                                              User_Id        UserId,
                                                              HTTPMethod     Method,
                                                              String         URL,
                                                              IPPort?        TCPPort             = null,
                                                              String         BasicAuth_Login     = null,
                                                              String         BasicAuth_Password  = null)

            => UsersAPI.RegisterNotification(UserId,
                                             new HTTPSNotification(Method,
                                                                   URL,
                                                                   TCPPort,
                                                                   BasicAuth_Login,
                                                                   BasicAuth_Password),
                                             (a, b) => a.URL                == b.URL &&
                                                       a.BasicAuth_Login    == b.BasicAuth_Login &&
                                                       a.BasicAuth_Password == b.BasicAuth_Password);

        #endregion


        #region RegisterHTTPSNotification(this UsersAPI, User,   NotificationId, URL,         BasicAuth_Login = null, BasicAuth_Password = null)

        public static Notifications RegisterHTTPSNotification(this UsersAPI    UsersAPI,
                                                              User             User,
                                                              Notification_Id  NotificationId,
                                                              String           URL,
                                                              IPPort?          TCPPort             = null,
                                                              String           BasicAuth_Login     = null,
                                                              String           BasicAuth_Password  = null)
        {

            if (User == null)
                throw new ArgumentNullException(nameof(User), "The given user must not be null!");

            return UsersAPI.RegisterHTTPSNotification(User.Id,
                                                      NotificationId,
                                                      URL,
                                                      TCPPort,
                                                      BasicAuth_Login,
                                                      BasicAuth_Password);

        }

        #endregion

        #region RegisterHTTPSNotification(this UsersAPI, UserId, NotificationId, URL,         BasicAuth_Login = null, BasicAuth_Password = null)

        public static Notifications RegisterHTTPSNotification(this UsersAPI    UsersAPI,
                                                              User_Id          UserId,
                                                              Notification_Id  NotificationId,
                                                              String           URL,
                                                              IPPort?          TCPPort             = null,
                                                              String           BasicAuth_Login     = null,
                                                              String           BasicAuth_Password  = null)

            => UsersAPI.RegisterNotification(UserId,
                                             NotificationId,
                                             new HTTPSNotification(HTTPMethod.POST,
                                                                   URL,
                                                                   TCPPort,
                                                                   BasicAuth_Login,
                                                                   BasicAuth_Password),
                                             (a, b) => a.URL                == b.URL &&
                                                       a.BasicAuth_Login    == b.BasicAuth_Login &&
                                                       a.BasicAuth_Password == b.BasicAuth_Password);

        #endregion

        #region RegisterHTTPSNotification(this UsersAPI, User,   NotificationId, Method, URL, BasicAuth_Login = null, BasicAuth_Password = null)

        public static Notifications RegisterHTTPSNotification(this UsersAPI    UsersAPI,
                                                              User             User,
                                                              Notification_Id  NotificationId,
                                                              HTTPMethod       Method,
                                                              String           URL,
                                                              IPPort?          TCPPort             = null,
                                                              String           BasicAuth_Login     = null,
                                                              String           BasicAuth_Password  = null)
        {

            if (User == null)
                throw new ArgumentNullException(nameof(User), "The given user must not be null!");

            return UsersAPI.RegisterHTTPSNotification(User.Id,
                                                      NotificationId,
                                                      Method,
                                                      URL,
                                                      TCPPort,
                                                      BasicAuth_Login,
                                                      BasicAuth_Password);

        }

        #endregion

        #region RegisterHTTPSNotification(this UsersAPI, UserId, NotificationId, Method, URL, BasicAuth_Login = null, BasicAuth_Password = null)

        public static Notifications RegisterHTTPSNotification(this UsersAPI    UsersAPI,
                                                              User_Id          UserId,
                                                              Notification_Id  NotificationId,
                                                              HTTPMethod       Method,
                                                              String           URL,
                                                              IPPort?          TCPPort             = null,
                                                              String           BasicAuth_Login     = null,
                                                              String           BasicAuth_Password  = null)

            => UsersAPI.RegisterNotification(UserId,
                                             NotificationId,
                                             new HTTPSNotification(Method,
                                                                   URL,
                                                                   TCPPort,
                                                                   BasicAuth_Login,
                                                                   BasicAuth_Password),
                                             (a, b) => a.URL                == b.URL &&
                                                       a.BasicAuth_Login    == b.BasicAuth_Login &&
                                                       a.BasicAuth_Password == b.BasicAuth_Password);

        #endregion


        public static IEnumerable<HTTPSNotification> GetHTTPSNotifications(this UsersAPI    UsersAPI,
                                                                           User             User,
                                                                           Notification_Id  NotificationId)


            => UsersAPI.GetNotifications<HTTPSNotification>(User,
                                                            NotificationId);

        public static IEnumerable<HTTPSNotification> GetHTTPSNotifications(this UsersAPI    UsersAPI,
                                                                           User_Id          User,
                                                                           Notification_Id  NotificationId)


            => UsersAPI.GetNotifications<HTTPSNotification>(User,
                                                            NotificationId);





        #region Upstream HTTP request...

        //var httpresult = await new HTTPSClient(Hostname,
        //                                       RemoteCertificateValidator,
        //                                       RemotePort:  HTTPPort,
        //                                       DNSClient:   DNSClient ?? this.DNSClient).

        //                           Execute(client => client.REMOTESTART(HTTPURI.Parse("/ps/rest/hubject/RNs/" + RoamingNetworkId + "/EVSEs/" + EVSEId.ToString().Replace("+", "")),

        //                                                                requestbuilder => {
        //                                                                    requestbuilder.Host         = VirtualHostname;
        //                                                                    requestbuilder.ContentType  = HTTPContentType.JSON_UTF8;
        //                                                                    requestbuilder.Content      = new JObject(
        //                                                                                                      new JProperty("@context",           "http://ld.graphdefined.org/wwcp/RemoteStartRequest"),
        //                                                                                                      new JProperty("@id",                SessionId.ToString()),
        //                                                                                                      new JProperty("ProviderId",         ProviderId.ToString()),
        //                                                                                                      new JProperty("eMAId",              eMAId.ToString())
        //                                                                                                      //new JProperty("ChargingProductId",  ChargingProductId != null ? ChargingProductId.ToString() : "")
        //                                                                                                  ).ToString().
        //                                                                                                    ToUTF8Bytes();
        //                                                                    requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
        //                                                                }),

        //                                   RequestLogDelegate:   OnRemoteStartRequest,
        //                                   ResponseLogDelegate:  OnRemoteStartResponse,
        //                                   CancellationToken:    CancellationToken,
        //                                   EventTrackingId:      EventTrackingId,
        //                                   RequestTimeout:       RequestTimeout ?? TimeSpan.FromSeconds(60)).

        //                           ConfigureAwait(false);

        #endregion












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

        public HTTPMethod Method               { get; }
        public String     URL                  { get; }
        public IPPort     TCPPort              { get; }
        public String     BasicAuth_Login      { get; }
        public String     BasicAuth_Password   { get; }


        public HTTPSNotification(HTTPMethod Method,
                                 String     URL,
                                 IPPort?    TCPPort             = null,
                                 String     BasicAuth_Login     = null,
                                 String     BasicAuth_Password  = null)
        {

            this.Method              = Method;
            this.URL                 = URL;
            this.TCPPort             = TCPPort ?? IPPort.HTTPS;
            this.BasicAuth_Login     = BasicAuth_Login;
            this.BasicAuth_Password  = BasicAuth_Password;

        }


        public static HTTPSNotification Parse(JObject JSON)
        {

            if (JSON["type"]?.Value<String>() != typeof(HTTPSNotification).Name)
                throw new ArgumentException();

            return new HTTPSNotification(
                       JSON["method"] != null ? HTTPMethod.ParseString(JSON["method"].Value<String>()) : HTTPMethod.POST,
                       JSON["URL"]?.Value<String>(),
                       JSON["TCPPort"] != null ? IPPort.Parse(JSON["TCPPort"].Value<String>()) : IPPort.HTTPS,
                       JSON["basicAuth"]?["login"]?.   Value<String>(),
                       JSON["basicAuth"]?["password"]?.Value<String>()
                   );

        }

        public override JObject GetAsJSON(JObject JSON)
        {

            JSON.Add(new JProperty("type",     GetType().Name));
            JSON.Add(new JProperty("method",   Method.ToString()));
            JSON.Add(new JProperty("URL",      URL));
            JSON.Add(new JProperty("TCPPort",  TCPPort.ToUInt16()));

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
