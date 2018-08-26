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
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.OpenData.Users;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace org.GraphDefined.OpenData.Notifications
{

    /// <summary>
    /// Extention methods for HTTPS Notifications.
    /// </summary>
    public static class HTTPSNotificationExtentions
    {

        #region AddHTTPSNotification(this UsersAPI, User,                    URL, Method, BasicAuth_Login = null, BasicAuth_Password = null, APIKey = null)

        public static Task AddHTTPSNotification(this UsersAPI  UsersAPI,
                                                User           User,
                                                String         URL,
                                                HTTPMethod?    Method              = null,
                                                IPPort?        TCPPort             = null,
                                                String         BasicAuth_Login     = null,
                                                String         BasicAuth_Password  = null,
                                                String         APIKey              = null)

            => UsersAPI.AddNotification(User,
                                        new HTTPSNotification(Method ?? HTTPMethod.POST,
                                                              URL,
                                                              TCPPort,
                                                              BasicAuth_Login,
                                                              BasicAuth_Password,
                                                              APIKey));

        #endregion

        #region AddHTTPSNotification(this UsersAPI, UserId,                  URL, Method, BasicAuth_Login = null, BasicAuth_Password = null, APIKey = null)

        public static Task AddHTTPSNotification(this UsersAPI  UsersAPI,
                                                User_Id        UserId,
                                                String         URL,
                                                HTTPMethod?    Method              = null,
                                                IPPort?        TCPPort             = null,
                                                String         BasicAuth_Login     = null,
                                                String         BasicAuth_Password  = null,
                                                String         APIKey              = null)

            => UsersAPI.AddNotification(UserId,
                                        new HTTPSNotification(Method ?? HTTPMethod.POST,
                                                              URL,
                                                              TCPPort,
                                                              BasicAuth_Login,
                                                              BasicAuth_Password,
                                                              APIKey));

        #endregion

        #region AddHTTPSNotification(this UsersAPI, User,   NotificationId,  URL, Method, BasicAuth_Login = null, BasicAuth_Password = null, APIKey = null)

        public static Task AddHTTPSNotification(this UsersAPI            UsersAPI,
                                                User                     User,
                                                NotificationMessageType  NotificationId,
                                                String                   URL,
                                                HTTPMethod?              Method              = null,
                                                IPPort?                  TCPPort             = null,
                                                String                   BasicAuth_Login     = null,
                                                String                   BasicAuth_Password  = null,
                                                String                   APIKey              = null)

            => UsersAPI.AddNotification(User,
                                        new HTTPSNotification(Method ?? HTTPMethod.POST,
                                                              URL,
                                                              TCPPort,
                                                              BasicAuth_Login,
                                                              BasicAuth_Password,
                                                              APIKey),
                                        NotificationId);

        #endregion

        #region AddHTTPSNotification(this UsersAPI, UserId, NotificationId,  URL, Method, BasicAuth_Login = null, BasicAuth_Password = null, APIKey = null)

        public static Task AddHTTPSNotification(this UsersAPI            UsersAPI,
                                                User_Id                  UserId,
                                                NotificationMessageType  NotificationId,
                                                String                   URL,
                                                HTTPMethod?              Method              = null,
                                                IPPort?                  TCPPort             = null,
                                                String                   BasicAuth_Login     = null,
                                                String                   BasicAuth_Password  = null,
                                                String                   APIKey              = null)

            => UsersAPI.AddNotification(UserId,
                                        new HTTPSNotification(Method ?? HTTPMethod.POST,
                                                              URL,
                                                              TCPPort,
                                                              BasicAuth_Login,
                                                              BasicAuth_Password,
                                                              APIKey),
                                        NotificationId);

        #endregion

        #region AddHTTPSNotification(this UsersAPI, User,   NotificationIds, URL, Method, BasicAuth_Login = null, BasicAuth_Password = null, APIKey = null)

        public static Task AddHTTPSNotification(this UsersAPI                         UsersAPI,
                                                User                                  User,
                                                IEnumerable<NotificationMessageType>  NotificationIds,
                                                String                                URL,
                                                HTTPMethod?                           Method              = null,
                                                IPPort?                               TCPPort             = null,
                                                String                                BasicAuth_Login     = null,
                                                String                                BasicAuth_Password  = null,
                                                String                                APIKey              = null)

            => UsersAPI.AddNotification(User,
                                        new HTTPSNotification(Method ?? HTTPMethod.POST,
                                                              URL,
                                                              TCPPort,
                                                              BasicAuth_Login,
                                                              BasicAuth_Password,
                                                              APIKey),
                                        NotificationIds);

        #endregion

        #region AddHTTPSNotification(this UsersAPI, UserId, NotificationIds, URL, Method, BasicAuth_Login = null, BasicAuth_Password = null, APIKey = null)

        public static Task AddHTTPSNotification(this UsersAPI                         UsersAPI,
                                                User_Id                               UserId,
                                                IEnumerable<NotificationMessageType>  NotificationIds,
                                                String                                URL,
                                                HTTPMethod?                           Method              = null,
                                                IPPort?                               TCPPort             = null,
                                                String                                BasicAuth_Login     = null,
                                                String                                BasicAuth_Password  = null,
                                                String                                APIKey              = null)

            => UsersAPI.AddNotification(UserId,
                                        new HTTPSNotification(Method ?? HTTPMethod.POST,
                                                              URL,
                                                              TCPPort,
                                                              BasicAuth_Login,
                                                              BasicAuth_Password,
                                                              APIKey),
                                        NotificationIds);

        #endregion


        #region GetHTTPSNotifications(this UsersAPI, User,   NotificationMessageType = null)

        public static IEnumerable<HTTPSNotification> GetHTTPSNotifications(this UsersAPI             UsersAPI,
                                                                           User                      User,
                                                                           NotificationMessageType?  NotificationMessageType = null)


            => UsersAPI.GetNotificationsOf<HTTPSNotification>(User,
                                                              NotificationMessageType);

        #endregion

        #region GetHTTPSNotifications(this UsersAPI, UserId, NotificationMessageType = null)

        public static IEnumerable<HTTPSNotification> GetHTTPSNotifications(this UsersAPI             UsersAPI,
                                                                           User_Id                   UserId,
                                                                           NotificationMessageType?  NotificationMessageType = null)


            => UsersAPI.GetNotificationsOf<HTTPSNotification>(UserId,
                                                              NotificationMessageType);

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
        //                                                        NotificationMessageType  NotificationId,
        //                                                        String           URL,
        //                                                        String           BasicAuth_Login     = null,
        //                                                        String           BasicAuth_Password  = null)

        //    => UsersAPI.UnregisterNotification<HTTPSNotification>(User,
        //                                                          NotificationId,
        //                                                          a => a.URL                == URL &&
        //                                                               a.BasicAuth_Login    == BasicAuth_Login &&
        //                                                               a.BasicAuth_Password == BasicAuth_Password);

        //public static Notifications UnregisterHTTPSNotification(this UsersAPI    UsersAPI,
        //                                                        User_Id          User,
        //                                                        NotificationMessageType  NotificationId,
        //                                                        String           URL,
        //                                                        String           BasicAuth_Login     = null,
        //                                                        String           BasicAuth_Password  = null)

        //    => UsersAPI.UnregisterNotification<HTTPSNotification>(User,
        //                                                          NotificationId,
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
        /// The JSON-LD context of the object.
        /// </summary>
        public const String JSONLDContext = "https://opendata.social/contexts/UsersAPI+json/HTTPSNotification";

        #endregion

        #region Properties

        public HTTPMethod Method               { get; }
        public String     URL                  { get; }
        public IPPort     TCPPort              { get; }
        public String     BasicAuth_Login      { get; }
        public String     BasicAuth_Password   { get; }
        public String     APIKey               { get; }


        public override String SortKey

            => String.Concat(nameof(EMailNotification),
                             URL,
                             Method,
                             TCPPort);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new  HTTPS notification.
        /// </summary>
        public HTTPSNotification(HTTPMethod                            Method,
                                 String                                URL,
                                 IPPort?                               TCPPort                   = null,
                                 String                                BasicAuth_Login           = null,
                                 String                                BasicAuth_Password        = null,
                                 String                                APIKey                    = null,
                                 IEnumerable<NotificationMessageType>  NotificationMessageTypes  = null)

            : base(NotificationMessageTypes)

        {

            this.Method              = Method;
            this.URL                 = URL;
            this.TCPPort             = TCPPort ?? IPPort.HTTPS;
            this.BasicAuth_Login     = BasicAuth_Login;
            this.BasicAuth_Password  = BasicAuth_Password;
            this.APIKey              = APIKey;

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

                Notification = new HTTPSNotification(JSON["method"] != null ? HTTPMethod.ParseString(JSON["method"].Value<String>()) : HTTPMethod.POST,
                                                     JSON["URL"]?.Value<String>(),
                                                     JSON["TCPPort"] != null ? IPPort.Parse(JSON["TCPPort"].Value<String>()) : IPPort.HTTPS,
                                                     JSON["basicAuth"]?["login"]?.   Value<String>(),
                                                     JSON["basicAuth"]?["password"]?.Value<String>(),
                                                     JSON["APIKey"]?.Value<String>(),
                                                     (JSON["messageTypes"] as JArray)?.SafeSelect(element => NotificationMessageType.Parse(element.Value<String>())));

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
                       ? new JProperty("@context", JSONLDContext)
                       : null,

                   new JProperty("method",   Method.ToString()),
                   new JProperty("URL",      URL),
                   new JProperty("TCPPort",  TCPPort.ToUInt16()),

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
                       ? new JProperty("APIKey",  APIKey)
                       : null,

                   NotificationMessageTypes.SafeAny()
                       ? new JProperty("messageTypes", new JArray(NotificationMessageTypes.Select(msgType => msgType.ToString())))
                       : null

               );

        #endregion


        #region OptionalEquals(EMailNotification)

        public override Boolean OptionalEquals(ANotification other)

            => other is HTTPSNotification httpsNotification &&
               this.OptionalEquals(httpsNotification);

        public Boolean OptionalEquals(HTTPSNotification other)

            => Method. Equals(other.Method)  &&
               URL.    Equals(other.URL)     &&
               TCPPort.Equals(other.TCPPort) &&

               BasicAuth_Login?.   Equals(other.BasicAuth_Login)    == true &&
               BasicAuth_Password?.Equals(other.BasicAuth_Password) == true &&
               APIKey?.            Equals(other.APIKey)             == true;

        #endregion


        #region IComparable<HTTPSNotification> Members

        #region CompareTo(ANotification)

        public override Int32 CompareTo(ANotification other)
            => SortKey.CompareTo(other.SortKey);

        #endregion

        #region CompareTo(HTTPSNotification)

        public Int32 CompareTo(HTTPSNotification other)
        {

            if (URL.CompareTo(other.URL) != 0)
                return URL.CompareTo(other.URL);

            return TCPPort.CompareTo(other.TCPPort);

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

            => Method. Equals(other.Method) &&
               URL.    Equals(other.URL)    &&
               TCPPort.Equals(other.URL);

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
            => String.Concat(nameof(HTTPSNotification), ": ", Method, " ", URL, ":", TCPPort);

        #endregion

    }

}
