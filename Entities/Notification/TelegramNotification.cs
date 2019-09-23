/*
 * Copyright (c) 2014-2019, Achim 'ahzf' Friedland <achim@graphdefined.org>
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

#endregion

namespace org.GraphDefined.OpenData.Notifications
{

    public static class TelegramNotificationExtentions
    {

        #region AddTelegramNotification(this UsersAPI, User,                             TelegramUsername, TextTemplate = null)

        public static Task AddTelegramNotification(this UsersAPI  UsersAPI,
                                              User           User,
                                              String         TelegramUsername,
                                              String         TextTemplate  = null)

            => UsersAPI.AddNotification(User,
                                        new TelegramNotification(TelegramUsername,
                                                            TextTemplate));

        #endregion

        #region AddTelegramNotification(this UsersAPI, UserId,                           TelegramUsername, TextTemplate = null)

        public static Task AddTelegramNotification(this UsersAPI  UsersAPI,
                                              User_Id        UserId,
                                              String         TelegramUsername,
                                              String         TextTemplate  = null)

            => UsersAPI.AddNotification(UserId,
                                        new TelegramNotification(TelegramUsername,
                                                            TextTemplate));

        #endregion

        #region AddTelegramNotification(this UsersAPI, User,   NotificationMessageType,  TelegramUsername, TextTemplate = null)

        public static Task AddTelegramNotification(this UsersAPI            UsersAPI,
                                              User                     User,
                                              NotificationMessageType  NotificationMessageType,
                                              String                   TelegramUsername,
                                              String                   TextTemplate  = null)

            => UsersAPI.AddNotification(User,
                                        new TelegramNotification(TelegramUsername,
                                                            TextTemplate),
                                        NotificationMessageType);

        #endregion

        #region AddTelegramNotification(this UsersAPI, UserId, NotificationMessageType,  TelegramUsername, TextTemplate = null)

        public static Task AddTelegramNotification(this UsersAPI            UsersAPI,
                                              User_Id                  UserId,
                                              NotificationMessageType  NotificationMessageType,
                                              String                   TelegramUsername,
                                              String                   TextTemplate  = null)

            => UsersAPI.AddNotification(UserId,
                                        new TelegramNotification(TelegramUsername,
                                                            TextTemplate),
                                        NotificationMessageType);

        #endregion

        #region AddTelegramNotification(this UsersAPI, User,   NotificationMessageTypes, TelegramUsername, TextTemplate = null)

        public static Task AddTelegramNotification(this UsersAPI                         UsersAPI,
                                              User                                  User,
                                              IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                              String                                TelegramUsername,
                                              String                                TextTemplate  = null)

            => UsersAPI.AddNotification(User,
                                        new TelegramNotification(TelegramUsername,
                                                            TextTemplate),
                                        NotificationMessageTypes);

        #endregion

        #region AddTelegramNotification(this UsersAPI, UserId, NotificationMessageTypes, TelegramUsername, TextTemplate = null)

        public static Task AddTelegramNotification(this UsersAPI                         UsersAPI,
                                              User_Id                               UserId,
                                              IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                              String                                TelegramUsername,
                                              String                                TextTemplate  = null)

            => UsersAPI.AddNotification(UserId,
                                        new TelegramNotification(TelegramUsername,
                                                            TextTemplate),
                                        NotificationMessageTypes);

        #endregion


        #region GetTelegramNotifications(this UsersAPI, User,           params NotificationMessageTypes)

        public static IEnumerable<TelegramNotification> GetTelegramNotifications(this UsersAPI                     UsersAPI,
                                                                                 User                              User,
                                                                                 params NotificationMessageType[]  NotificationMessageTypes)


            => UsersAPI.GetNotificationsOf<TelegramNotification>(User,
                                                            NotificationMessageTypes);

        #endregion

        #region GetTelegramNotifications(this UsersAPI, UserId,         params NotificationMessageTypes)

        public static IEnumerable<TelegramNotification> GetTelegramNotifications(this UsersAPI                     UsersAPI,
                                                                                 User_Id                           UserId,
                                                                                 params NotificationMessageType[]  NotificationMessageTypes)


            => UsersAPI.GetNotificationsOf<TelegramNotification>(UserId,
                                                                 NotificationMessageTypes);

        #endregion

        #region GetTelegramNotifications(this UsersAPI, Organization,   params NotificationMessageTypes)

        public static IEnumerable<TelegramNotification> GetTelegramNotifications(this UsersAPI                     UsersAPI,
                                                                                 Organization                      Organization,
                                                                                 params NotificationMessageType[]  NotificationMessageTypes)


            => UsersAPI.GetNotificationsOf<TelegramNotification>(Organization,
                                                            NotificationMessageTypes);

        #endregion

        #region GetTelegramNotifications(this UsersAPI, OrganizationId, params NotificationMessageTypes)

        public static IEnumerable<TelegramNotification> GetTelegramNotifications(this UsersAPI                     UsersAPI,
                                                                                 Organization_Id                   OrganizationId,
                                                                                 params NotificationMessageType[]  NotificationMessageTypes)


            => UsersAPI.GetNotificationsOf<TelegramNotification>(OrganizationId,
                                                                 NotificationMessageTypes);

        #endregion


        //public static Notifications UnregisterTelegramNotification(this UsersAPI  UsersAPI,
        //                                                      User           User,
        //                                                      TelegramUsername   TelegramUsername)

        //    => UsersAPI.UnregisterNotification<TelegramNotification>(User,
        //                                                        a => a.TelegramUsername == TelegramUsername);

        //public static Notifications UnregisterTelegramNotification(this UsersAPI  UsersAPI,
        //                                                      User_Id        User,
        //                                                      TelegramUsername   TelegramUsername)

        //    => UsersAPI.UnregisterNotification<TelegramNotification>(User,
        //                                                        a => a.TelegramUsername == TelegramUsername);


        //public static Notifications UnregisterTelegramNotification(this UsersAPI    UsersAPI,
        //                                                      User             User,
        //                                                      NotificationMessageType  NotificationMessageType,
        //                                                      TelegramUsername     TelegramUsername)

        //    => UsersAPI.UnregisterNotification<TelegramNotification>(User,
        //                                                        NotificationMessageType,
        //                                                        a => a.TelegramUsername == TelegramUsername);

        //public static Notifications UnregisterTelegramNotification(this UsersAPI    UsersAPI,
        //                                                      User_Id          User,
        //                                                      NotificationMessageType  NotificationMessageType,
        //                                                      TelegramUsername     TelegramUsername)

        //    => UsersAPI.UnregisterNotification<TelegramNotification>(User,
        //                                                        NotificationMessageType,
        //                                                        a => a.TelegramUsername == TelegramUsername);

    }

    public class TelegramNotification : ANotification,
                                        IEquatable <TelegramNotification>,
                                        IComparable<TelegramNotification>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of the object.
        /// </summary>
        public const String JSONLDContext = "https://opendata.social/contexts/UsersAPI+json/TelegramNotification";

        #endregion

        #region Properties

        public String  TelegramUsername    { get; }
        public String  TextTemplate        { get; }

        #endregion

        #region Constructor(s)

        public TelegramNotification(String                                TelegramUsername,
                                    String                                TextTemplate              = null,
                                    IEnumerable<NotificationMessageType>  NotificationMessageTypes  = null,
                                    String                                Description               = null)

            : base(NotificationMessageTypes,
                   Description,
                   String.Concat(nameof(TelegramNotification),
                                 TelegramUsername,
                                 TextTemplate))

        {

            this.TelegramUsername  = TelegramUsername;
            this.TextTemplate      = TextTemplate;

        }

        #endregion


        #region Parse   (JSON)

        public static TelegramNotification Parse(JObject JSON)
        {

            if (TryParse(JSON, out TelegramNotification Notification))
                return Notification;

            return null;

        }

        #endregion

        #region TryParse(JSON, out Notification)

        public static Boolean TryParse(JObject JSON, out TelegramNotification Notification)
        {

            var TelegramUsername = JSON["telegramUsername"]?.Value<String>();

            if (JSON["@context"]?.Value<String>() == JSONLDContext && TelegramUsername.IsNeitherNullNorEmpty())
            {

                Notification = new TelegramNotification(TelegramUsername,
                                                        JSON["textTemplate"]?.Value<String>(),
                                                       (JSON["messageTypes"] as JArray)?.SafeSelect(element => NotificationMessageType.Parse(element.Value<String>())),
                                                        JSON["description" ]?.Value<String>());

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
                       ? new JProperty("@context",      JSONLDContext)
                       : null,

                   new JProperty("telegramUsername",    TelegramUsername),

                   TextTemplate.IsNotNullOrEmpty()
                       ? new JProperty("textTemplate",  TextTemplate)
                       : null,

                   NotificationMessageTypes.SafeAny()
                       ? new JProperty("messageTypes",  new JArray(NotificationMessageTypes.Select(msgType => msgType.ToString())))
                       : null,

                   Description.IsNotNullOrEmpty()
                       ? new JProperty("description",   Description)
                       : null

               );

        #endregion


        #region OptionalEquals(EMailNotification)

        public override Boolean OptionalEquals(ANotification other)

            => other is TelegramNotification TelegramNotification &&
               this.OptionalEquals(TelegramNotification);

        public Boolean OptionalEquals(TelegramNotification other)

            => TelegramUsername.  Equals(other.TelegramUsername)         &&

               String.Equals(TextTemplate, other.TextTemplate) &&

               _NotificationMessageTypes.SetEquals(other._NotificationMessageTypes);

        #endregion


        #region IComparable<TelegramNotification> Members

        #region CompareTo(ANotification)

        public override Int32 CompareTo(ANotification other)
            => SortKey.CompareTo(other.SortKey);

        #endregion

        #region CompareTo(TelegramNotification)

        public Int32 CompareTo(TelegramNotification other)
            => TelegramUsername.CompareTo(other.TelegramUsername);

        #endregion

        #endregion

        #region IEquatable<TelegramNotification> Members

        #region Equals(ANotification)

        public override Boolean Equals(ANotification other)
            => SortKey.Equals(other.SortKey);

        #endregion

        #region Equals(TelegramNotification)

        public Boolean Equals(TelegramNotification other)
            => TelegramUsername.Equals(other.TelegramUsername);

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
            => String.Concat(nameof(TelegramNotification), ": ", TelegramUsername.ToString());

        #endregion

    }

}
