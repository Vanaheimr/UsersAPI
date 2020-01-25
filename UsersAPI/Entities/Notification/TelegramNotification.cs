/*
 * Copyright (c) 2014-2020, Achim 'ahzf' Friedland <achim@graphdefined.org>
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

#endregion

namespace social.OpenData.UsersAPI.Notifications
{

    /// <summary>
    /// Extention methods for Telegram notifications.
    /// </summary>
    public static class TelegramNotificationExtentions
    {

        #region AddTelegramNotification(this UsersAPI, User,                             Username, TextTemplate = null)

        public static Task AddTelegramNotification(this UsersAPI  UsersAPI,
                                                   User           User,
                                                   String         Username,
                                                   Int32?         ChatId         = null,
                                                   String         SharedSecret   = null,
                                                   String         TextTemplate   = null)

            => UsersAPI.AddNotification(User,
                                        new TelegramNotification(Username,
                                                                 ChatId,
                                                                 SharedSecret,
                                                                 TextTemplate));

        #endregion

        #region AddTelegramNotification(this UsersAPI, UserId,                           Username, TextTemplate = null)

        public static Task AddTelegramNotification(this UsersAPI  UsersAPI,
                                                   User_Id        UserId,
                                                   String         Username,
                                                   Int32?         ChatId         = null,
                                                   String         SharedSecret   = null,
                                                   String         TextTemplate   = null)

            => UsersAPI.AddNotification(UserId,
                                        new TelegramNotification(Username,
                                                                 ChatId,
                                                                 SharedSecret,
                                                                 TextTemplate));

        #endregion

        #region AddTelegramNotification(this UsersAPI, User,   NotificationMessageType,  Username, TextTemplate = null)

        public static Task AddTelegramNotification(this UsersAPI            UsersAPI,
                                                   User                     User,
                                                   NotificationMessageType  NotificationMessageType,
                                                   String                   Username,
                                                   Int32?                   ChatId         = null,
                                                   String                   SharedSecret   = null,
                                                   String                   TextTemplate   = null)

            => UsersAPI.AddNotification(User,
                                        new TelegramNotification(Username,
                                                                 ChatId,
                                                                 SharedSecret,
                                                                 TextTemplate),
                                        NotificationMessageType);

        #endregion

        #region AddTelegramNotification(this UsersAPI, UserId, NotificationMessageType,  Username, TextTemplate = null)

        public static Task AddTelegramNotification(this UsersAPI            UsersAPI,
                                                   User_Id                  UserId,
                                                   NotificationMessageType  NotificationMessageType,
                                                   String                   Username,
                                                   Int32?                   ChatId         = null,
                                                   String                   SharedSecret   = null,
                                                   String                   TextTemplate   = null)

            => UsersAPI.AddNotification(UserId,
                                        new TelegramNotification(Username,
                                                                 ChatId,
                                                                 SharedSecret,
                                                                 TextTemplate),
                                        NotificationMessageType);

        #endregion

        #region AddTelegramNotification(this UsersAPI, User,   NotificationMessageTypes, Username, TextTemplate = null)

        public static Task AddTelegramNotification(this UsersAPI                         UsersAPI,
                                                   User                                  User,
                                                   IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                                   String                                Username,
                                                   Int32?                                ChatId         = null,
                                                   String                                SharedSecret   = null,
                                                   String                                TextTemplate   = null)

            => UsersAPI.AddNotification(User,
                                        new TelegramNotification(Username,
                                                                 ChatId,
                                                                 SharedSecret,
                                                                 TextTemplate),
                                        NotificationMessageTypes);

        #endregion

        #region AddTelegramNotification(this UsersAPI, UserId, NotificationMessageTypes, Username, TextTemplate = null)

        public static Task AddTelegramNotification(this UsersAPI                         UsersAPI,
                                                   User_Id                               UserId,
                                                   IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                                   String                                Username,
                                                   Int32?                                ChatId         = null,
                                                   String                                SharedSecret   = null,
                                                   String                                TextTemplate   = null)

            => UsersAPI.AddNotification(UserId,
                                        new TelegramNotification(Username,
                                                                 ChatId,
                                                                 SharedSecret,
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
        //                                                      Username   Username)

        //    => UsersAPI.UnregisterNotification<TelegramNotification>(User,
        //                                                        a => a.Username == Username);

        //public static Notifications UnregisterTelegramNotification(this UsersAPI  UsersAPI,
        //                                                      User_Id        User,
        //                                                      Username   Username)

        //    => UsersAPI.UnregisterNotification<TelegramNotification>(User,
        //                                                        a => a.Username == Username);


        //public static Notifications UnregisterTelegramNotification(this UsersAPI    UsersAPI,
        //                                                      User             User,
        //                                                      NotificationMessageType  NotificationMessageType,
        //                                                      Username     Username)

        //    => UsersAPI.UnregisterNotification<TelegramNotification>(User,
        //                                                        NotificationMessageType,
        //                                                        a => a.Username == Username);

        //public static Notifications UnregisterTelegramNotification(this UsersAPI    UsersAPI,
        //                                                      User_Id          User,
        //                                                      NotificationMessageType  NotificationMessageType,
        //                                                      Username     Username)

        //    => UsersAPI.UnregisterNotification<TelegramNotification>(User,
        //                                                        NotificationMessageType,
        //                                                        a => a.Username == Username);

    }

    /// <summary>
    /// A Telegram notification.
    /// </summary>
    public class TelegramNotification : ANotification,
                                        IEquatable <TelegramNotification>,
                                        IComparable<TelegramNotification>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public const String JSONLDContext = "https://opendata.social/contexts/UsersAPI+json/TelegramNotification";

        #endregion

        #region Properties

        /// <summary>
        /// The Telegram user name of this Telegram notification.
        /// </summary>
        public String  Username        { get; }

        /// <summary>
        /// The Telegram chat identification of this Telegram notification.
        /// </summary>
        public Int32?  ChatId          { get; }

        /// <summary>
        /// The Telegram shared secret of this Telegram notification.
        /// </summary>
        public String  SharedSecret    { get; }

        /// <summary>
        /// An optional text template for the SMS notification.
        /// </summary>
        public String  TextTemplate    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Telegram notification.
        /// </summary>
        /// <param name="Username">The Telegram user name of this Telegram notification.</param>
        /// <param name="TextTemplate">An optional text template for the SMS notification.</param>
        /// <param name="ChatId">The Telegram chat identification of this Telegram notification.</param>
        /// <param name="SharedSecret">The Telegram shared secret of this Telegram notification.</param>
        /// <param name="NotificationMessageTypes">An optional enumeration of notification message types.</param>
        /// <param name="Description">Some description to remember why this notification was created.</param>
        public TelegramNotification(String                                Username,
                                    Int32?                                ChatId                     = null,
                                    String                                SharedSecret               = null,
                                    String                                TextTemplate               = null,
                                    IEnumerable<NotificationMessageType>  NotificationMessageTypes   = null,
                                    String                                Description                = null)

            : base(NotificationMessageTypes,
                   Description,
                   String.Concat(nameof(TelegramNotification),
                                 Username,
                                 ChatId ?? 0,
                                 SharedSecret,
                                 TextTemplate))

        {

            this.Username      = Username;
            this.ChatId        = ChatId;
            this.SharedSecret  = SharedSecret;
            this.TextTemplate  = TextTemplate;

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

            var Username = JSON["username"]?.Value<String>();

            if (JSON["@context"]?.Value<String>() == JSONLDContext && Username.IsNeitherNullNorEmpty())
            {

                Notification = new TelegramNotification(Username,
                                                        JSON["chatId"]?.      Value<Int32>(),
                                                        JSON["sharedSecret"]?.Value<String>(),
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

                   new JProperty("username",            Username),

                   ChatId.HasValue
                       ? new JProperty("chatId",        ChatId.Value)
                       : null,

                   SharedSecret.IsNotNullOrEmpty()
                       ? new JProperty("sharedSecret",  SharedSecret)
                       : null,

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


        #region OptionalEquals(TelegramNotification)

        public override Boolean OptionalEquals(ANotification other)

            => other is TelegramNotification TelegramNotification &&
               this.OptionalEquals(TelegramNotification);

        public Boolean OptionalEquals(TelegramNotification other)

            => String.Equals(Username,     other.Username)     &&
               String.Equals(Description,  other.Description)  &&
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
            => Username.CompareTo(other.Username);

        #endregion

        #endregion

        #region IEquatable<TelegramNotification> Members

        #region Equals(ANotification)

        public override Boolean Equals(ANotification other)
            => SortKey.Equals(other.SortKey);

        #endregion

        #region Equals(TelegramNotification)

        public Boolean Equals(TelegramNotification other)
            => Username.Equals(other.Username);

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
            => String.Concat(nameof(TelegramNotification), ": ", Username.ToString());

        #endregion

    }

}
