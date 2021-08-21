/*
 * Copyright (c) 2014-2021, Achim Friedland <achim.friedland@graphdefined.com>
 * This file is part of UsersAPI <https://www.github.com/Vanaheimr/UsersAPI>
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.Mail;

#endregion

namespace social.OpenData.UsersAPI.Notifications
{

    /// <summary>
    /// Extention methods for e-mail notifications.
    /// </summary>
    public static class EMailNotificationExtentions
    {

        #region AddEMailNotification(this UsersAPI, User,                             EMailAddress = null, Subject = null, SubjectPrefix = null)

        public static Task AddEMailNotification(this UsersAPI     UsersAPI,
                                                User              User,
                                                EMailAddress      EMailAddress      = null,
                                                String            Subject           = null,
                                                String            SubjectPrefix     = null,
                                                EventTracking_Id  EventTrackingId   = null,
                                                User_Id?          CurrentUserId     = null)

            => UsersAPI.AddNotification(User,
                                        new EMailNotification(EMailAddress ?? User.EMail,
                                                              Subject,
                                                              SubjectPrefix),
                                        EventTrackingId,
                                        CurrentUserId);

        #endregion

        #region AddEMailNotification(this UsersAPI, UserId,                           EMailAddress,        Subject = null, SubjectPrefix = null)

        public static Task AddEMailNotification(this UsersAPI     UsersAPI,
                                                User_Id           UserId,
                                                EMailAddress      EMailAddress,
                                                String            Subject           = null,
                                                String            SubjectPrefix     = null,
                                                EventTracking_Id  EventTrackingId   = null,
                                                User_Id?          CurrentUserId     = null)

            => UsersAPI.AddNotification(UserId,
                                        new EMailNotification(EMailAddress,
                                                              Subject,
                                                              SubjectPrefix),
                                        EventTrackingId,
                                        CurrentUserId);

        #endregion

        #region AddEMailNotification(this UsersAPI, User,   NotificationMessageType,  EMailAddress = null, Subject = null, SubjectPrefix = null)

        public static Task AddEMailNotification(this UsersAPI            UsersAPI,
                                                User                     User,
                                                NotificationMessageType  NotificationMessageType,
                                                EMailAddress             EMailAddress      = null,
                                                String                   Subject           = null,
                                                String                   SubjectPrefix     = null,
                                                EventTracking_Id         EventTrackingId   = null,
                                                User_Id?                 CurrentUserId     = null)

            => UsersAPI.AddNotification(User,
                                        new EMailNotification(EMailAddress ?? User.EMail,
                                                              Subject,
                                                              SubjectPrefix),
                                        NotificationMessageType,
                                        EventTrackingId,
                                        CurrentUserId);

        #endregion

        #region AddEMailNotification(this UsersAPI, UserId, NotificationMessageType,  EMailAddress,        Subject = null, SubjectPrefix = null)

        public static Task AddEMailNotification(this UsersAPI            UsersAPI,
                                                User_Id                  UserId,
                                                NotificationMessageType  NotificationMessageType,
                                                EMailAddress             EMailAddress,
                                                String                   Subject           = null,
                                                String                   SubjectPrefix     = null,
                                                EventTracking_Id         EventTrackingId   = null,
                                                User_Id?                 CurrentUserId     = null)

            => UsersAPI.AddNotification(UserId,
                                        new EMailNotification(EMailAddress,
                                                              Subject,
                                                              SubjectPrefix),
                                        NotificationMessageType,
                                        EventTrackingId,
                                        CurrentUserId);

        #endregion

        #region AddEMailNotification(this UsersAPI, User,   NotificationMessageTypes, EMailAddress = null, Subject = null, SubjectPrefix = null)

        public static Task AddEMailNotification(this UsersAPI                         UsersAPI,
                                                User                                  User,
                                                IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                                EMailAddress                          EMailAddress      = null,
                                                String                                Subject           = null,
                                                String                                SubjectPrefix     = null,
                                                EventTracking_Id                      EventTrackingId   = null,
                                                User_Id?                              CurrentUserId     = null)

            => UsersAPI.AddNotification(User,
                                        new EMailNotification(EMailAddress ?? User.EMail,
                                                              Subject,
                                                              SubjectPrefix),
                                        NotificationMessageTypes,
                                        EventTrackingId,
                                        CurrentUserId);

        #endregion

        #region AddEMailNotification(this UsersAPI, UserId, NotificationMessageTypes, EMailAddress,        Subject = null, SubjectPrefix = null)

        public static Task AddEMailNotification(this UsersAPI                         UsersAPI,
                                                User_Id                               UserId,
                                                IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                                EMailAddress                          EMailAddress,
                                                String                                Subject           = null,
                                                String                                SubjectPrefix     = null,
                                                EventTracking_Id                      EventTrackingId   = null,
                                                User_Id?                              CurrentUserId     = null)

            => UsersAPI.AddNotification(UserId,
                                        new EMailNotification(EMailAddress,
                                                              Subject,
                                                              SubjectPrefix),
                                        NotificationMessageTypes,
                                        EventTrackingId,
                                        CurrentUserId);

        #endregion


        #region GetEMailNotifications(this UsersAPI, User,           params NotificationMessageTypes)

        public static IEnumerable<EMailNotification> GetEMailNotifications(this UsersAPI                     UsersAPI,
                                                                           User                              User,
                                                                           params NotificationMessageType[]  NotificationMessageTypes)


            => UsersAPI.GetNotificationsOf<EMailNotification>(User,
                                                              NotificationMessageTypes);

        #endregion

        #region GetEMailNotifications(this UsersAPI, UserId,         params NotificationMessageTypes)

        public static IEnumerable<EMailNotification> GetEMailNotifications(this UsersAPI                     UsersAPI,
                                                                           User_Id                           UserId,
                                                                           params NotificationMessageType[]  NotificationMessageTypes)


            => UsersAPI.GetNotificationsOf<EMailNotification>(UserId,
                                                              NotificationMessageTypes);

        #endregion

        #region GetEMailNotifications(this UsersAPI, Organization,   params NotificationMessageTypes)

        public static IEnumerable<EMailNotification> GetEMailNotifications(this UsersAPI                     UsersAPI,
                                                                           Organization                      Organization,
                                                                           params NotificationMessageType[]  NotificationMessageTypes)


            => UsersAPI.GetNotificationsOf<EMailNotification>(Organization,
                                                              NotificationMessageTypes);

        #endregion

        #region GetEMailNotifications(this UsersAPI, OrganizationId, params NotificationMessageTypes)

        public static IEnumerable<EMailNotification> GetEMailNotifications(this UsersAPI                     UsersAPI,
                                                                           Organization_Id                   OrganizationId,
                                                                           params NotificationMessageType[]  NotificationMessageTypes)


            => UsersAPI.GetNotificationsOf<EMailNotification>(OrganizationId,
                                                              NotificationMessageTypes);

        #endregion


        //public static Notifications UnregisterEMailNotification(this UsersAPI  UsersAPI,
        //                                                        User           User,
        //                                                        EMailAddress   EMailAddress)

        //    => UsersAPI.UnregisterNotification<EMailNotification>(User,
        //                                                          a => a.EMailAddress == EMailAddress);

        //public static Notifications UnregisterEMailNotification(this UsersAPI  UsersAPI,
        //                                                        User_Id        User,
        //                                                        EMailAddress   EMailAddress)

        //    => UsersAPI.UnregisterNotification<EMailNotification>(User,
        //                                                          a => a.EMailAddress == EMailAddress);


        //public static Notifications UnregisterEMailNotification(this UsersAPI    UsersAPI,
        //                                                        User             User,
        //                                                        NotificationMessageType  NotificationMessageType,
        //                                                        EMailAddress     EMailAddress)

        //    => UsersAPI.UnregisterNotification<EMailNotification>(User,
        //                                                          NotificationMessageType,
        //                                                          a => a.EMailAddress == EMailAddress);

        //public static Notifications UnregisterEMailNotification(this UsersAPI    UsersAPI,
        //                                                        User_Id          User,
        //                                                        NotificationMessageType  NotificationMessageType,
        //                                                        EMailAddress     EMailAddress)

        //    => UsersAPI.UnregisterNotification<EMailNotification>(User,
        //                                                          NotificationMessageType,
        //                                                          a => a.EMailAddress == EMailAddress);

    }


    /// <summary>
    /// An E-Mail notification.
    /// </summary>
    public class EMailNotification : ANotification,
                                     IEquatable <EMailNotification>,
                                     IComparable<EMailNotification>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public const String JSONLDContext = "https://opendata.social/contexts/UsersAPI/EMailNotification";

        #endregion

        #region Properties

        /// <summary>
        /// The e-mail address of the receiver of this notification.
        /// </summary>
        public EMailAddress  EMailAddress     { get; }

        /// <summary>
        /// An optional customer-specific subject of all e-mails to send.
        /// </summary>
        public String        Subject          { get; }

        /// <summary>
        /// An optional prefix added to the standard subject of the e-mail notification.
        /// </summary>
        public String        SubjectPrefix    { get; }

        /// <summary>
        /// An optional 'List-Id" e-mail header within all e-mails to send.
        /// </summary>
        public String        ListId           { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new e-mail notification.
        /// </summary>
        /// <param name="EMailAddress">The e-mail address of the receiver of this notification.</param>
        /// <param name="Subject">An optional customer-specific subject of all e-mails to send.</param>
        /// <param name="SubjectPrefix">An optional prefix added to the standard subject of all e-mails to send.</param>
        /// <param name="ListId">An optional 'List-Id" e-mail header within all e-mails to send.</param>
        /// <param name="NotificationMessageTypes">An optional enumeration of notification message types.</param>
        /// <param name="Description">Some description to remember why this notification was created.</param>
        public EMailNotification(EMailAddress                          EMailAddress,
                                 String                                Subject                    = null,
                                 String                                SubjectPrefix              = null,
                                 String                                ListId                     = null,
                                 IEnumerable<NotificationMessageType>  NotificationMessageTypes   = null,
                                 String                                Description                = null)

            : base(NotificationMessageTypes,
                   Description,
                   String.Concat(nameof(EMailNotification),
                                 EMailAddress,
                                 Subject,
                                 SubjectPrefix,
                                 ListId))

        {

            this.EMailAddress   = EMailAddress;
            this.Subject        = Subject;
            this.SubjectPrefix  = SubjectPrefix;
            this.ListId         = ListId;

        }

        #endregion


        #region Parse   (JSON)

        public static EMailNotification Parse(JObject JSON)
        {

            if (TryParse(JSON, out EMailNotification Notification))
                return Notification;

            return null;

        }

        #endregion

        #region TryParse(JSON, out Notification)

        public static Boolean TryParse(JObject JSON, out EMailNotification Notification)
        {

            if (JSON["@context"]?.Value<String>() == JSONLDContext &&
                JSON["email"] is JObject EMailJSON &&
                EMailAddress.TryParseJSON(EMailJSON,
                                          out EMailAddress EMail,
                                          out String       ErrorResponse,
                                          true))
            {

                Notification = new EMailNotification(EMail,
                                                     JSON["subject"      ]?.Value<String>(),
                                                     JSON["subjectPrefix"]?.Value<String>(),
                                                     JSON["listId"       ]?.Value<String>(),
                                                     (JSON["messageTypes"] as JArray)?.SafeSelect(element => NotificationMessageType.Parse(element.Value<String>())),
                                                     JSON["description"  ]?.Value<String>());

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
                       ? new JProperty("@context",       JSONLDContext.ToString())
                       : null,

                   new JProperty("email",                EMailAddress.ToJSON(Embedded: true)),

                   Subject.IsNotNullOrEmpty()
                       ? new JProperty("subject",        Subject)
                       : null,

                   SubjectPrefix.IsNotNullOrEmpty()
                       ? new JProperty("subjectPrefix",  SubjectPrefix)
                       : null,

                   ListId.IsNotNullOrEmpty()
                       ? new JProperty("listId",         ListId)
                       : null,

                   NotificationMessageTypes.SafeAny()
                       ? new JProperty("messageTypes",   new JArray(NotificationMessageTypes.Select(msgType => msgType.ToString())))
                       : null,

                   Description.IsNotNullOrEmpty()
                       ? new JProperty("description",    Description)
                       : null

               );

        #endregion


        #region OptionalEquals(EMailNotification)

        public override Boolean OptionalEquals(ANotification other)

            => other is EMailNotification eMailNotification &&
               this.OptionalEquals(eMailNotification);

        public Boolean OptionalEquals(EMailNotification other)

            => EMailAddress.Equals(other.EMailAddress)           &&

               String.Equals(Subject,       other.Subject)       &&
               String.Equals(SubjectPrefix, other.SubjectPrefix) &&
               String.Equals(ListId,        other.ListId)        &&

               String.Equals(Description,   other.Description)   &&

               _NotificationMessageTypes.SetEquals(other._NotificationMessageTypes);

        #endregion


        #region IComparable<EMailNotification> Members

        #region CompareTo(ANotification)

        public override Int32 CompareTo(ANotification other)
            => SortKey.CompareTo(other.SortKey);

        #endregion

        #region CompareTo(EMailNotification)

        public Int32 CompareTo(EMailNotification other)
            => EMailAddress.CompareTo(other.EMailAddress);

        #endregion

        #endregion

        #region IEquatable<EMailNotification> Members

        #region Equals(ANotification)

        public override Boolean Equals(ANotification other)
            => SortKey.Equals(other.SortKey);

        #endregion

        #region Equals(EMailNotification)

        public Boolean Equals(EMailNotification other)
            => EMailAddress.Equals(other.EMailAddress);

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
            => String.Concat(nameof(EMailNotification), ": ", EMailAddress.ToString());

        #endregion

    }

}
