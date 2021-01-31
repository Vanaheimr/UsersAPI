/*
 * Copyright (c) 2014-2021, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using social.OpenData.UsersAPI;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace social.OpenData.UsersAPI.Notifications
{

    /// <summary>
    /// Extention methods for HTTPS Notifications.
    /// </summary>
    public static class HTTPSNotificationExtentions
    {

        #region AddHTTPSNotification(this UsersAPI, User,                             URL, Method, BasicAuth_Login = null, BasicAuth_Password = null, APIKey = null)

        public static Task AddHTTPSNotification(this UsersAPI  UsersAPI,
                                                User           User,
                                                String         URL,
                                                HTTPMethod?    Method              = null,
                                                IPPort?        TCPPort             = null,
                                                String         BasicAuth_Login     = null,
                                                String         BasicAuth_Password  = null,
                                                String         APIKey              = null)

            => UsersAPI.AddNotification(User,
                                        new HTTPSNotification(URL,
                                                              Method,
                                                              TCPPort,
                                                              BasicAuth_Login,
                                                              BasicAuth_Password,
                                                              APIKey));

        #endregion

        #region AddHTTPSNotification(this UsersAPI, UserId,                           URL, Method, BasicAuth_Login = null, BasicAuth_Password = null, APIKey = null)

        public static Task AddHTTPSNotification(this UsersAPI  UsersAPI,
                                                User_Id        UserId,
                                                String         URL,
                                                HTTPMethod?    Method              = null,
                                                IPPort?        TCPPort             = null,
                                                String         BasicAuth_Login     = null,
                                                String         BasicAuth_Password  = null,
                                                String         APIKey              = null)

            => UsersAPI.AddNotification(UserId,
                                        new HTTPSNotification(URL,
                                                              Method,
                                                              TCPPort,
                                                              BasicAuth_Login,
                                                              BasicAuth_Password,
                                                              APIKey));

        #endregion

        #region AddHTTPSNotification(this UsersAPI, User,   NotificationMessageType,  URL, Method, BasicAuth_Login = null, BasicAuth_Password = null, APIKey = null)

        public static Task AddHTTPSNotification(this UsersAPI            UsersAPI,
                                                User                     User,
                                                NotificationMessageType  NotificationMessageType,
                                                String                   URL,
                                                HTTPMethod?              Method              = null,
                                                IPPort?                  TCPPort             = null,
                                                String                   BasicAuth_Login     = null,
                                                String                   BasicAuth_Password  = null,
                                                String                   APIKey              = null)

            => UsersAPI.AddNotification(User,
                                        new HTTPSNotification(URL,
                                                              Method,
                                                              TCPPort,
                                                              BasicAuth_Login,
                                                              BasicAuth_Password,
                                                              APIKey),
                                        NotificationMessageType);

        #endregion

        #region AddHTTPSNotification(this UsersAPI, UserId, NotificationMessageType,  URL, Method, BasicAuth_Login = null, BasicAuth_Password = null, APIKey = null)

        public static Task AddHTTPSNotification(this UsersAPI            UsersAPI,
                                                User_Id                  UserId,
                                                NotificationMessageType  NotificationMessageType,
                                                String                   URL,
                                                HTTPMethod?              Method              = null,
                                                IPPort?                  TCPPort             = null,
                                                String                   BasicAuth_Login     = null,
                                                String                   BasicAuth_Password  = null,
                                                String                   APIKey              = null)

            => UsersAPI.AddNotification(UserId,
                                        new HTTPSNotification(URL,
                                                              Method,
                                                              TCPPort,
                                                              BasicAuth_Login,
                                                              BasicAuth_Password,
                                                              APIKey),
                                        NotificationMessageType);

        #endregion

        #region AddHTTPSNotification(this UsersAPI, User,   NotificationMessageTypes, URL, Method, BasicAuth_Login = null, BasicAuth_Password = null, APIKey = null)

        public static Task AddHTTPSNotification(this UsersAPI                         UsersAPI,
                                                User                                  User,
                                                IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                                String                                URL,
                                                HTTPMethod?                           Method              = null,
                                                IPPort?                               TCPPort             = null,
                                                String                                BasicAuth_Login     = null,
                                                String                                BasicAuth_Password  = null,
                                                String                                APIKey              = null)

            => UsersAPI.AddNotification(User,
                                        new HTTPSNotification(URL,
                                                              Method,
                                                              TCPPort,
                                                              BasicAuth_Login,
                                                              BasicAuth_Password,
                                                              APIKey),
                                        NotificationMessageTypes);

        #endregion

        #region AddHTTPSNotification(this UsersAPI, UserId, NotificationMessageTypes, URL, Method, BasicAuth_Login = null, BasicAuth_Password = null, APIKey = null)

        public static Task AddHTTPSNotification(this UsersAPI                         UsersAPI,
                                                User_Id                               UserId,
                                                IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                                String                                URL,
                                                HTTPMethod?                           Method              = null,
                                                IPPort?                               TCPPort             = null,
                                                String                                BasicAuth_Login     = null,
                                                String                                BasicAuth_Password  = null,
                                                String                                APIKey              = null)

            => UsersAPI.AddNotification(UserId,
                                        new HTTPSNotification(URL,
                                                              Method,
                                                              TCPPort,
                                                              BasicAuth_Login,
                                                              BasicAuth_Password,
                                                              APIKey),
                                        NotificationMessageTypes);

        #endregion


        #region GetHTTPSNotifications(this UsersAPI, User,           params NotificationMessageTypes)

        public static IEnumerable<HTTPSNotification> GetHTTPSNotifications(this UsersAPI                     UsersAPI,
                                                                           User                              User,
                                                                           params NotificationMessageType[]  NotificationMessageTypes)


            => UsersAPI.GetNotificationsOf<HTTPSNotification>(User,
                                                              NotificationMessageTypes);

        #endregion

        #region GetHTTPSNotifications(this UsersAPI, UserId,         params NotificationMessageTypes)

        public static IEnumerable<HTTPSNotification> GetHTTPSNotifications(this UsersAPI                     UsersAPI,
                                                                           User_Id                           UserId,
                                                                           params NotificationMessageType[]  NotificationMessageTypes)


            => UsersAPI.GetNotificationsOf<HTTPSNotification>(UserId,
                                                              NotificationMessageTypes);

        #endregion

        #region GetHTTPSNotifications(this UsersAPI, Organization,   params NotificationMessageTypes)

        public static IEnumerable<HTTPSNotification> GetHTTPSNotifications(this UsersAPI                     UsersAPI,
                                                                           Organization                      Organization,
                                                                           params NotificationMessageType[]  NotificationMessageTypes)


            => UsersAPI.GetNotificationsOf<HTTPSNotification>(Organization,
                                                              NotificationMessageTypes);

        #endregion

        #region GetHTTPSNotifications(this UsersAPI, OrganizationId, params NotificationMessageTypes)

        public static IEnumerable<HTTPSNotification> GetHTTPSNotifications(this UsersAPI                     UsersAPI,
                                                                           Organization_Id                   OrganizationId,
                                                                           params NotificationMessageType[]  NotificationMessageTypes)


            => UsersAPI.GetNotificationsOf<HTTPSNotification>(OrganizationId,
                                                              NotificationMessageTypes);

        #endregion


        //public static Notifications UnregisterHTTPSNotification(this UsersAPI  UsersAPI,
        //                                                        User           User,
        //                                                        String         URL,
        //                                                        String         BasicAuth_Login     = null,
        //                                                        String         BasicAuth_Password  = null)

        //    => UsersAPI.UnregisterNotification<HTTPSNotification>(User,
        //                                                          a => a.URL                == URL &&
        //                                                               a.BasicAuth_Login    == BasicAuth_Login &&
        //                                                               a.BasicAuth_Password == BasicAuth_Password);


        //public static Notifications UnregisterHTTPSNotification(this UsersAPI  UsersAPI,
        //                                                        User_Id        User,
        //                                                        String         URL,
        //                                                        String         BasicAuth_Login     = null,
        //                                                        String         BasicAuth_Password  = null)

        //    => UsersAPI.UnregisterNotification<HTTPSNotification>(User,
        //                                                          a => a.URL                == URL &&
        //                                                               a.BasicAuth_Login    == BasicAuth_Login &&
        //                                                               a.BasicAuth_Password == BasicAuth_Password);


        //public static Notifications UnregisterHTTPSNotification(this UsersAPI    UsersAPI,
        //                                                        User             User,
        //                                                        NotificationMessageType  NotificationMessageType,
        //                                                        String           URL,
        //                                                        String           BasicAuth_Login     = null,
        //                                                        String           BasicAuth_Password  = null)

        //    => UsersAPI.UnregisterNotification<HTTPSNotification>(User,
        //                                                          NotificationMessageType,
        //                                                          a => a.URL                == URL &&
        //                                                               a.BasicAuth_Login    == BasicAuth_Login &&
        //                                                               a.BasicAuth_Password == BasicAuth_Password);

        //public static Notifications UnregisterHTTPSNotification(this UsersAPI    UsersAPI,
        //                                                        User_Id          User,
        //                                                        NotificationMessageType  NotificationMessageType,
        //                                                        String           URL,
        //                                                        String           BasicAuth_Login     = null,
        //                                                        String           BasicAuth_Password  = null)

        //    => UsersAPI.UnregisterNotification<HTTPSNotification>(User,
        //                                                          NotificationMessageType,
        //                                                          a => a.URL                == URL &&
        //                                                               a.BasicAuth_Login    == BasicAuth_Login &&
        //                                                               a.BasicAuth_Password == BasicAuth_Password);

    }


    /// <summary>
    /// A HTTPS notification.
    /// </summary>
    public class HTTPSNotification : ANotification,
                                     IEquatable<HTTPSNotification>,
                                     IComparable<HTTPSNotification>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public const String JSONLDContext = "https://opendata.social/contexts/UsersAPI/HTTPSNotification";

        #endregion

        #region Properties

        /// <summary>
        /// The URL for of HTTPS notification.
        /// </summary>
        public String      URL                   { get; }

        /// <summary>
        /// The HTTP method of this HTTPS notification.
        /// </summary>
        public HTTPMethod  Method                { get; }

        /// <summary>
        /// The remote TCP port of this HTTPS notification.
        /// </summary>
        public IPPort      RemotePort            { get; }

        /// <summary>
        /// An optional HTTP Basic Auth login for the HTTPS notification.
        /// </summary>
        public String      BasicAuth_Login       { get; }

        /// <summary>
        /// An optional HTTP Basic Auth password for the HTTPS notification.
        /// </summary>
        public String      BasicAuth_Password    { get; }

        /// <summary>
        /// An optional HTTP API Key for the HTTPS notification.
        /// </summary>
        public String      APIKey                { get; }

        /// <summary>
        /// An optional HTTP request timeout.
        /// </summary>
        public TimeSpan?   RequestTimeout        { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new HTTPS notification.
        /// </summary>
        /// <param name="URL">The URL of this HTTPS notification.</param>
        /// <param name="Method">The HTTP method of this HTTPS notification.</param>
        /// <param name="RemotePort">The remote TCP port of this HTTPS notification.</param>
        /// <param name="BasicAuth_Login">An optional HTTP Basic Auth login for the HTTPS notification.</param>
        /// <param name="BasicAuth_Password">An optional HTTP Basic Auth password for the HTTPS notification.</param>
        /// <param name="APIKey">An optional HTTP API Key for the HTTPS notification.</param>
        /// <param name="RequestTimeout"></param>
        /// <param name="NotificationMessageTypes">An optional enumeration of notification message types.</param>
        /// <param name="Description">Some description to remember why this notification was created.</param>
        public HTTPSNotification(String                                URL,
                                 HTTPMethod?                           Method                     = null,
                                 IPPort?                               RemotePort                 = null,
                                 String                                BasicAuth_Login            = null,
                                 String                                BasicAuth_Password         = null,
                                 String                                APIKey                     = null,
                                 TimeSpan?                             RequestTimeout             = null,
                                 IEnumerable<NotificationMessageType>  NotificationMessageTypes   = null,
                                 String                                Description                = null)

            : base(NotificationMessageTypes,
                   Description,
                   String.Concat(nameof(HTTPSNotification),
                                 URL,
                                 Method     ?? HTTPMethod.POST,
                                 RemotePort ?? IPPort.HTTPS))

        {

            this.URL                 = URL;
            this.Method              = Method     ?? HTTPMethod.POST;
            this.RemotePort          = RemotePort ?? IPPort.HTTPS;
            this.BasicAuth_Login     = BasicAuth_Login;
            this.BasicAuth_Password  = BasicAuth_Password;
            this.APIKey              = APIKey;
            this.RequestTimeout      = RequestTimeout;

        }

        #endregion


        #region Parse   (JSON)

        public static HTTPSNotification Parse(JObject JSON)
        {

            if (TryParse(JSON, out HTTPSNotification Notification))
                return Notification;

            return null;

        }

        #endregion

        #region TryParse(JSON, out Notification)

        public static Boolean TryParse(JObject JSON, out HTTPSNotification Notification)
        {

            var url = JSON["URL"]?.Value<String>();

            if (JSON["@context"]?.Value<String>() == JSONLDContext &&
                url.IsNotNullOrEmpty())
            {

                Notification = new HTTPSNotification(JSON["URL"           ]?.Value<String>(),
                                                     JSON["method"        ] != null ? HTTPMethod.ParseString(JSON["method"].Value<String>()) : HTTPMethod.POST,
                                                     JSON["TCPPort"       ] != null ? IPPort.Parse(JSON["TCPPort"].Value<String>()) : IPPort.HTTPS,
                                                     JSON["basicAuth"     ]?["login"   ]?.Value<String>(),
                                                     JSON["basicAuth"     ]?["password"]?.Value<String>(),
                                                     JSON["APIKey"        ]?.Value<String>(),
                                                     JSON["RequestTimeout"] != null ? TimeSpan.FromSeconds((Double) JSON["RequestTimeout"]?.Value<Int32>()) : new TimeSpan?(),
                                                     (JSON["messageTypes" ] as JArray)?.SafeSelect(element => NotificationMessageType.Parse(element.Value<String>())),
                                                     JSON["description"   ]?.Value<String>());

                return true;

            }

            Notification = null;
            return false;

        }

        #endregion

        #region ToJSON(Embedded = false)

        public override JObject ToJSON(Boolean Embedded = false)

            => JSONObject.Create(

                   !Embedded
                       ? new JProperty("@context",          JSONLDContext.ToString())
                       : null,

                   new JProperty("method",                Method.ToString()),
                   new JProperty("URL",                   URL),
                   new JProperty("TCPPort",               RemotePort.ToUInt16()),

                   BasicAuth_Login.   IsNotNullOrEmpty() &&
                   BasicAuth_Password.IsNotNullOrEmpty()
                       ? new JProperty("basicAuth",
                             new JObject(
                                 new JProperty("login",     BasicAuth_Login),
                                 new JProperty("password",  BasicAuth_Password)
                             )
                         )
                       : null,

                   APIKey.IsNotNullOrEmpty()
                       ? new JProperty("APIKey",          APIKey)
                       : null,

                   RequestTimeout.HasValue
                       ? new JProperty("RequestTimeout",  RequestTimeout.Value.TotalSeconds)
                       : null,

                   NotificationMessageTypes.SafeAny()
                       ? new JProperty("messageTypes",    new JArray(NotificationMessageTypes.Select(msgType => msgType.ToString())))
                       : null,

                   Description.IsNotNullOrEmpty()
                       ? new JProperty("description",     Description)
                       : null

               );

        #endregion


        #region OptionalEquals(EMailNotification)

        public override Boolean OptionalEquals(ANotification other)

            => other is HTTPSNotification httpsNotification &&
               this.OptionalEquals(httpsNotification);

        public Boolean OptionalEquals(HTTPSNotification other)

            => Method.    Equals(other.Method)                             &&
               URL.       Equals(other.URL)                                &&
               RemotePort.Equals(other.RemotePort)                         &&

               String.Equals(BasicAuth_Login,    other.BasicAuth_Login)    &&
               String.Equals(BasicAuth_Password, other.BasicAuth_Password) &&
               String.Equals(APIKey,             other.APIKey)             &&

               String.Equals(Description,        other.Description)        &&

               _NotificationMessageTypes.SetEquals(other._NotificationMessageTypes);

        #endregion


        #region IComparable<HTTPSNotification> Members

        #region CompareTo(ANotification)

        public override Int32 CompareTo(ANotification other)
            => SortKey.CompareTo(other.SortKey);

        #endregion

        #region CompareTo(HTTPSNotification)

        public Int32 CompareTo(HTTPSNotification other)
        {

            var c = URL.CompareTo(other.URL);
            if (c != 0)
                return c;

            c = RemotePort.CompareTo(other.RemotePort);
            if (c != 0)
                return c;

            return Method.CompareTo(other.Method);

        }

        #endregion

        #endregion

        #region IEquatable<HTTPSNotification> Members

        #region Equals(ANotification)

        public override Boolean Equals(ANotification other)
            => SortKey.Equals(other.SortKey);

        #endregion

        #region Equals(HTTPSNotification)

        public Boolean Equals(HTTPSNotification other)

            => URL.    Equals(other.URL)     &&
               RemotePort.Equals(other.RemotePort) &&
               Method. Equals(other.Method);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => SortKey.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => String.Concat(nameof(HTTPSNotification), ": ", Method, " ", URL, ":", RemotePort);

        #endregion

    }

}
