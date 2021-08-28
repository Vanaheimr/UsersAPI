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

using social.OpenData.UsersAPI;

#endregion

namespace social.OpenData.UsersAPI.Notifications
{

    /// <summary>
    /// Extention methods for SMS notifications.
    /// </summary>
    public static class SMSNotificationExtentions
    {

        #region AddSMSNotification(this UsersAPI, User, NotificationMessageType,  Phonenumber = null, TextTemplate = null)

        public static Task AddSMSNotification(this UsersAPI            UsersAPI,
                                              User                     User,
                                              NotificationMessageType  NotificationMessageType,
                                              PhoneNumber?             PhoneNumber       = null,
                                              String                   TextTemplate      = null,
                                              EventTracking_Id         EventTrackingId   = null,
                                              User_Id?                 CurrentUserId     = null)
        {

            var phoneNumber = PhoneNumber ?? User.MobilePhone;

            if (!phoneNumber.HasValue || phoneNumber.Value.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(PhoneNumber), "The given mobile phone number must not be null or empty!");

            return UsersAPI.AddNotification(User,
                                            new SMSNotification(phoneNumber.Value,
                                                                TextTemplate),
                                            NotificationMessageType,
                                            EventTrackingId,
                                            CurrentUserId);

        }

        #endregion

        #region AddSMSNotification(this UsersAPI, User, NotificationMessageTypes, PhoneNumber = null, TextTemplate = null)

        public static Task AddSMSNotification(this UsersAPI                         UsersAPI,
                                              User                                  User,
                                              IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                              PhoneNumber?                          PhoneNumber       = null,
                                              String                                TextTemplate      = null,
                                              EventTracking_Id                      EventTrackingId   = null,
                                              User_Id?                              CurrentUserId     = null)
        {

            var phoneNumber = PhoneNumber ?? User.MobilePhone;

            if (!phoneNumber.HasValue || phoneNumber.Value.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(PhoneNumber), "The given mobile phone number must not be null or empty!");

            return UsersAPI.AddNotification(User,
                                            new SMSNotification(phoneNumber.Value,
                                                                TextTemplate),
                                            NotificationMessageTypes,
                                            EventTrackingId,
                                            CurrentUserId);

        }

        #endregion


        #region GetSMSNotifications(this UsersAPI, User,         params NotificationMessageTypes)

        public static IEnumerable<SMSNotification> GetSMSNotifications(this UsersAPI                     UsersAPI,
                                                                       User                              User,
                                                                       params NotificationMessageType[]  NotificationMessageTypes)


            => UsersAPI.GetNotificationsOf<SMSNotification>(User,
                                                            NotificationMessageTypes);

        #endregion

        #region GetSMSNotifications(this UsersAPI, Organization, params NotificationMessageTypes)

        public static IEnumerable<SMSNotification> GetSMSNotifications(this UsersAPI                     UsersAPI,
                                                                       Organization                      Organization,
                                                                       params NotificationMessageType[]  NotificationMessageTypes)


            => UsersAPI.GetNotificationsOf<SMSNotification>(Organization,
                                                            NotificationMessageTypes);

        #endregion

        #region GetSMSNotifications(this UsersAPI, UserGroup,    params NotificationMessageTypes)

        public static IEnumerable<SMSNotification> GetSMSNotifications(this UsersAPI                     UsersAPI,
                                                                       UserGroup                         UserGroup,
                                                                       params NotificationMessageType[]  NotificationMessageTypes)


            => UsersAPI.GetNotificationsOf<SMSNotification>(UserGroup,
                                                            NotificationMessageTypes);

        #endregion


        //public static Notifications UnregisterSMSNotification(this UsersAPI  UsersAPI,
        //                                                      User           User,
        //                                                      PhoneNumber   Phonenumber)

        //    => UsersAPI.UnregisterNotification<SMSNotification>(User,
        //                                                        a => a.Phonenumber == Phonenumber);

        //public static Notifications UnregisterSMSNotification(this UsersAPI  UsersAPI,
        //                                                      User_Id        User,
        //                                                      PhoneNumber   Phonenumber)

        //    => UsersAPI.UnregisterNotification<SMSNotification>(User,
        //                                                        a => a.Phonenumber == Phonenumber);


        //public static Notifications UnregisterSMSNotification(this UsersAPI    UsersAPI,
        //                                                      User             User,
        //                                                      NotificationMessageType  NotificationMessageType,
        //                                                      PhoneNumber     Phonenumber)

        //    => UsersAPI.UnregisterNotification<SMSNotification>(User,
        //                                                        NotificationMessageType,
        //                                                        a => a.Phonenumber == Phonenumber);

        //public static Notifications UnregisterSMSNotification(this UsersAPI    UsersAPI,
        //                                                      User_Id          User,
        //                                                      NotificationMessageType  NotificationMessageType,
        //                                                      PhoneNumber     Phonenumber)

        //    => UsersAPI.UnregisterNotification<SMSNotification>(User,
        //                                                        NotificationMessageType,
        //                                                        a => a.Phonenumber == Phonenumber);

    }

    /// <summary>
    /// A SMS notification.
    /// </summary>
    public class SMSNotification : ANotification,
                                   IEquatable <SMSNotification>,
                                   IComparable<SMSNotification>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public const String JSONLDContext = "https://opendata.social/contexts/UsersAPI/SMSNotification";

        #endregion

        #region Properties

        /// <summary>
        /// The phone number of this SMS notification.
        /// </summary>
        public PhoneNumber  PhoneNumber     { get; }

        /// <summary>
        /// An optional text template for the SMS notification.
        /// </summary>
        public String       TextTemplate    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SMS notification.
        /// </summary>
        /// <param name="PhoneNumber">The phone number of this SMS notification.</param>
        /// <param name="TextTemplate">An optional text template for the SMS notification.</param>
        /// <param name="NotificationMessageTypes">An optional enumeration of notification message types.</param>
        /// <param name="Description">Some description to remember why this notification was created.</param>
        public SMSNotification(PhoneNumber                           PhoneNumber,
                               String                                TextTemplate               = null,
                               IEnumerable<NotificationMessageType>  NotificationMessageTypes   = null,
                               String                                Description                = null)

            : base(NotificationMessageTypes,
                   Description,
                   String.Concat(nameof(EMailNotification),
                                 PhoneNumber,
                                 TextTemplate))

        {

            this.PhoneNumber   = PhoneNumber;
            this.TextTemplate  = TextTemplate;

        }

        #endregion


        #region Parse   (JSON)

        public static SMSNotification Parse(JObject JSON)
        {

            if (TryParse(JSON, out SMSNotification Notification))
                return Notification;

            return null;

        }

        #endregion

        #region TryParse(JSON, out Notification)

        public static Boolean TryParse(JObject JSON, out SMSNotification Notification)
        {

            if (JSON["@context"]?.Value<String>() == JSONLDContext &&
                PhoneNumber.TryParse(JSON["phoneNumber"]?.Value<String>(), out PhoneNumber phoneNumber))
            {

                Notification = new SMSNotification(PhoneNumber.Parse(JSON["phoneNumber"]?.Value<String>()),
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
                       ? new JProperty("@context",      JSONLDContext.ToString())
                       : null,

                   new JProperty("phoneNumber",         PhoneNumber.ToString()),

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

            => other is SMSNotification smsNotification &&
               this.OptionalEquals(smsNotification);

        public Boolean OptionalEquals(SMSNotification other)

            => String.Equals(PhoneNumber,  other.PhoneNumber)  &&
               String.Equals(Description,  other.Description)  &&
               String.Equals(TextTemplate, other.TextTemplate) &&

               _NotificationMessageTypes.SetEquals(other._NotificationMessageTypes);

        #endregion


        #region IComparable<SMSNotification> Members

        #region CompareTo(ANotification)

        public override Int32 CompareTo(ANotification other)
            => SortKey.CompareTo(other.SortKey);

        #endregion

        #region CompareTo(SMSNotification)

        public Int32 CompareTo(SMSNotification other)
            => PhoneNumber.CompareTo(other.PhoneNumber);

        #endregion

        #endregion

        #region IEquatable<SMSNotification> Members

        #region Equals(ANotification)

        public override Boolean Equals(ANotification other)
            => SortKey.Equals(other.SortKey);

        #endregion

        #region Equals(SMSNotification)

        public Boolean Equals(SMSNotification other)
            => PhoneNumber.Equals(other.PhoneNumber);

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
            => String.Concat(nameof(SMSNotification), ": ", PhoneNumber.ToString());

        #endregion

    }

}
