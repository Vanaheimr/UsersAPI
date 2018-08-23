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
using org.GraphDefined.Vanaheimr.Hermod.Mail;

#endregion

namespace org.GraphDefined.OpenData.Notifications
{

    /// <summary>
    /// Extention methods for e-mail notifications.
    /// </summary>
    public static class EMailNotificationExtentions
    {

        #region AddEMailNotification(this UsersAPI, User,                    EMailAddress, Subject = null, SubjectPrefix = null)

        public static Task<NotificationStore> AddEMailNotification(this UsersAPI  UsersAPI,
                                                                   User           User,
                                                                   EMailAddress   EMailAddress,
                                                                   String         Subject        = null,
                                                                   String         SubjectPrefix  = null)

            => UsersAPI.AddNotification(User,
                                        new EMailNotification(EMailAddress,
                                                              Subject,
                                                              SubjectPrefix));

        #endregion

        #region AddEMailNotification(this UsersAPI, UserId,                  EMailAddress, Subject = null, SubjectPrefix = null)

        public static Task<NotificationStore> AddEMailNotification(this UsersAPI  UsersAPI,
                                                                   User_Id        UserId,
                                                                   EMailAddress   EMailAddress,
                                                                   String         Subject        = null,
                                                                   String         SubjectPrefix  = null)

            => UsersAPI.AddNotification(UserId,
                                        new EMailNotification(EMailAddress,
                                                              Subject,
                                                              SubjectPrefix));

        #endregion

        #region AddEMailNotification(this UsersAPI, User,   NotificationId,  EMailAddress, Subject = null, SubjectPrefix = null)

        public static Task<NotificationStore> AddEMailNotification(this UsersAPI            UsersAPI,
                                                                   User                     User,
                                                                   NotificationMessageType  NotificationId,
                                                                   EMailAddress             EMailAddress,
                                                                   String                   Subject        = null,
                                                                   String                   SubjectPrefix  = null)

            => UsersAPI.AddNotification(User,
                                        new EMailNotification(EMailAddress,
                                                              Subject,
                                                              SubjectPrefix),
                                        NotificationId);

        #endregion

        #region AddEMailNotification(this UsersAPI, UserId, NotificationId,  EMailAddress, Subject = null, SubjectPrefix = null)

        public static Task<NotificationStore> AddEMailNotification(this UsersAPI            UsersAPI,
                                                                   User_Id                  UserId,
                                                                   NotificationMessageType  NotificationId,
                                                                   EMailAddress             EMailAddress,
                                                                   String                   Subject        = null,
                                                                   String                   SubjectPrefix  = null)

            => UsersAPI.AddNotification(UserId,
                                        new EMailNotification(EMailAddress,
                                                              Subject,
                                                              SubjectPrefix),
                                        NotificationId);

        #endregion

        #region AddEMailNotification(this UsersAPI, User,   NotificationIds, EMailAddress, Subject = null, SubjectPrefix = null)

        public static Task<NotificationStore> AddEMailNotification(this UsersAPI                         UsersAPI,
                                                                   User                                  User,
                                                                   IEnumerable<NotificationMessageType>  NotificationIds,
                                                                   EMailAddress                          EMailAddress,
                                                                   String                                Subject        = null,
                                                                   String                                SubjectPrefix  = null)

            => UsersAPI.AddNotification(User,
                                        new EMailNotification(EMailAddress,
                                                              Subject,
                                                              SubjectPrefix),
                                        NotificationIds);

        #endregion

        #region AddEMailNotification(this UsersAPI, UserId, NotificationIds, EMailAddress, Subject = null, SubjectPrefix = null)

        public static Task<NotificationStore> AddEMailNotification(this UsersAPI                         UsersAPI,
                                                                   User_Id                               UserId,
                                                                   IEnumerable<NotificationMessageType>  NotificationIds,
                                                                   EMailAddress                          EMailAddress,
                                                                   String                                Subject        = null,
                                                                   String                                SubjectPrefix  = null)

            => UsersAPI.AddNotification(UserId,
                                        new EMailNotification(EMailAddress,
                                                              Subject,
                                                              SubjectPrefix),
                                        NotificationIds);

        #endregion


        #region GetEMailNotifications(this UsersAPI, User,   NotificationMessageType = null)

        public static IEnumerable<EMailNotification> GetEMailNotifications(this UsersAPI             UsersAPI,
                                                                           User                      User,
                                                                           NotificationMessageType?  NotificationMessageType = null)


            => UsersAPI.GetNotificationsOf<EMailNotification>(User,
                                                              NotificationMessageType);

        #endregion

        #region GetEMailNotifications(this UsersAPI, UserId, NotificationMessageType = null)

        public static IEnumerable<EMailNotification> GetEMailNotifications(this UsersAPI             UsersAPI,
                                                                           User_Id                   UserId,
                                                                           NotificationMessageType?  NotificationMessageType = null)


            => UsersAPI.GetNotificationsOf<EMailNotification>(UserId,
                                                              NotificationMessageType);

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
        //                                                        NotificationMessageType  NotificationId,
        //                                                        EMailAddress     EMailAddress)

        //    => UsersAPI.UnregisterNotification<EMailNotification>(User,
        //                                                          NotificationId,
        //                                                          a => a.EMailAddress == EMailAddress);

        //public static Notifications UnregisterEMailNotification(this UsersAPI    UsersAPI,
        //                                                        User_Id          User,
        //                                                        NotificationMessageType  NotificationId,
        //                                                        EMailAddress     EMailAddress)

        //    => UsersAPI.UnregisterNotification<EMailNotification>(User,
        //                                                          NotificationId,
        //                                                          a => a.EMailAddress == EMailAddress);

    }


    /// <summary>
    /// An E-Mail notification.
    /// </summary>
    public class EMailNotification : ANotification,
                                     IEquatable <EMailNotification>,
                                     IComparable<EMailNotification>
    {

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
        public EMailNotification(EMailAddress  EMailAddress,
                                 String        Subject         = null,
                                 String        SubjectPrefix   = null,
                                 String        ListId          = null)
        {

            this.EMailAddress   = EMailAddress;
            this.Subject        = Subject;
            this.SubjectPrefix  = SubjectPrefix;
            this.ListId         = ListId;

        }

        #endregion


        public static EMailNotification Parse(JObject JSON)
        {

            if (JSON["type"]?.Value<String>() != typeof(EMailNotification).Name)
                throw new ArgumentException();

            return new EMailNotification(new EMailAddress(
                                             OwnerName:                 JSON["username"]?.Value<String>(),
                                             SimpleEMailAddressString:  JSON["email"]?.   Value<String>(),
                                             SecretKeyRing:             null,
                                             PublicKeyRing:             null
                                         ),
                                         JSON["subject"]?.Value<String>());

        }

        public override JObject ToJSON()

            => JSONObject.Create(

                   new JProperty("type",      GetType().Name),
                   new JProperty("email",     EMailAddress.ToJSON(Embedded: true)),

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
                       : null

               );


        #region SortKey

        public override String SortKey

            => String.Concat(nameof(EMailNotification),
                             EMailAddress,
                             Subject,
                             SubjectPrefix,
                             ListId);

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
