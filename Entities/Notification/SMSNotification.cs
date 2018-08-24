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

#endregion

namespace org.GraphDefined.OpenData.Notifications
{

    public static class SMSNotificationExtentions
    {

        #region AddSMSNotification(this UsersAPI, User,                    Phonenumber, TextTemplate = null)

        public static Task AddSMSNotification(this UsersAPI  UsersAPI,
                                              User           User,
                                              PhoneNumber    Phonenumber,
                                              String         TextTemplate  = null)

            => UsersAPI.AddNotification(User,
                                        new SMSNotification(Phonenumber,
                                                            TextTemplate));

        #endregion

        #region AddSMSNotification(this UsersAPI, UserId,                  Phonenumber, TextTemplate = null)

        public static Task AddSMSNotification(this UsersAPI  UsersAPI,
                                              User_Id        UserId,
                                              PhoneNumber    Phonenumber,
                                              String         TextTemplate  = null)

            => UsersAPI.AddNotification(UserId,
                                        new SMSNotification(Phonenumber,
                                                            TextTemplate));

        #endregion

        #region AddSMSNotification(this UsersAPI, User,   NotificationId,  Phonenumber, TextTemplate = null)

        public static Task AddSMSNotification(this UsersAPI            UsersAPI,
                                              User                     User,
                                              NotificationMessageType  NotificationId,
                                              PhoneNumber              Phonenumber,
                                              String                   TextTemplate  = null)

            => UsersAPI.AddNotification(User,
                                        new SMSNotification(Phonenumber,
                                                            TextTemplate),
                                        NotificationId);

        #endregion

        #region AddSMSNotification(this UsersAPI, UserId, NotificationId,  Phonenumber, TextTemplate = null)

        public static Task AddSMSNotification(this UsersAPI            UsersAPI,
                                              User_Id                  UserId,
                                              NotificationMessageType  NotificationId,
                                              PhoneNumber              Phonenumber,
                                              String                   TextTemplate  = null)

            => UsersAPI.AddNotification(UserId,
                                        new SMSNotification(Phonenumber,
                                                            TextTemplate),
                                        NotificationId);

        #endregion

        #region AddSMSNotification(this UsersAPI, User,   NotificationIds, Phonenumber, TextTemplate = null)

        public static Task AddSMSNotification(this UsersAPI                         UsersAPI,
                                              User                                  User,
                                              IEnumerable<NotificationMessageType>  NotificationIds,
                                              PhoneNumber                           Phonenumber,
                                              String                                TextTemplate  = null)

            => UsersAPI.AddNotification(User,
                                        new SMSNotification(Phonenumber,
                                                            TextTemplate),
                                        NotificationIds);

        #endregion

        #region AddSMSNotification(this UsersAPI, UserId, NotificationIds, Phonenumber, TextTemplate = null)

        public static Task AddSMSNotification(this UsersAPI                         UsersAPI,
                                              User_Id                               UserId,
                                              IEnumerable<NotificationMessageType>  NotificationIds,
                                              PhoneNumber                           Phonenumber,
                                              String                                TextTemplate  = null)

            => UsersAPI.AddNotification(UserId,
                                        new SMSNotification(Phonenumber,
                                                            TextTemplate),
                                        NotificationIds);

        #endregion


        #region GetSMSNotifications(this UsersAPI, User,   NotificationMessageType = null)

        public static IEnumerable<SMSNotification> GetSMSNotifications(this UsersAPI             UsersAPI,
                                                                       User                      User,
                                                                       NotificationMessageType?  NotificationMessageType = null)


            => UsersAPI.GetNotificationsOf<SMSNotification>(User,
                                                            NotificationMessageType);

        #endregion

        #region GetSMSNotifications(this UsersAPI, UserId, NotificationMessageType = null)

        public static IEnumerable<SMSNotification> GetSMSNotifications(this UsersAPI             UsersAPI,
                                                                       User_Id                   UserId,
                                                                       NotificationMessageType?  NotificationMessageType = null)


            => UsersAPI.GetNotificationsOf<SMSNotification>(UserId,
                                                            NotificationMessageType);

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
        //                                                      NotificationMessageType  NotificationId,
        //                                                      PhoneNumber     Phonenumber)

        //    => UsersAPI.UnregisterNotification<SMSNotification>(User,
        //                                                        NotificationId,
        //                                                        a => a.Phonenumber == Phonenumber);

        //public static Notifications UnregisterSMSNotification(this UsersAPI    UsersAPI,
        //                                                      User_Id          User,
        //                                                      NotificationMessageType  NotificationId,
        //                                                      PhoneNumber     Phonenumber)

        //    => UsersAPI.UnregisterNotification<SMSNotification>(User,
        //                                                        NotificationId,
        //                                                        a => a.Phonenumber == Phonenumber);

    }

    public class SMSNotification : ANotification,
                                   IEquatable <SMSNotification>,
                                   IComparable<SMSNotification>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of the object.
        /// </summary>
        public const String JSONLDContext = "https://opendata.social/contexts/UsersAPI+json/SMSNotification";

        #endregion

        #region Properties

        public PhoneNumber PhoneNumber    { get; }
        public String      TextTemplate   { get; }

        #endregion

        public SMSNotification(PhoneNumber                           Phonenumber,
                               String                                TextTemplate              = null,
                               IEnumerable<NotificationMessageType>  NotificationMessageTypes  = null)

            : base(NotificationMessageTypes)

        {

            this.PhoneNumber   = Phonenumber;
            this.TextTemplate  = TextTemplate;

        }

        public static SMSNotification Parse(JObject JSON)
        {

            if (JSON["type"]?.Value<String>() != typeof(SMSNotification).Name)
                throw new ArgumentException();

            return new SMSNotification(PhoneNumber.Parse(JSON["phoneNumber"]?.Value<String>()),
                                       JSON["textTemplate"]?.Value<String>(),
                                       (JSON["messageTypes"] as JArray)?.SafeSelect(element => NotificationMessageType.Parse(element.Value<String>())));

        }



        public override JObject ToJSON(Boolean Embedded = false)

            => JSONObject.Create(

                   !Embedded
                       ? new JProperty("@context", JSONLDContext)
                       : null,

                   new JProperty("type",           GetType().Name),
                   new JProperty("phoneNumber",    PhoneNumber.ToString()),

                   TextTemplate.IsNotNullOrEmpty()
                       ? new JProperty("textTemplate",  TextTemplate)
                       : null,

                   NotificationMessageTypes.SafeAny()
                       ? new JProperty("messageTypes",  new JArray(NotificationMessageTypes.Select(msgType => msgType.ToString())))
                       : null

               );



        #region OptionalEquals(EMailNotification)

        public override Boolean OptionalEquals(ANotification other)

            => other is SMSNotification smsNotification &&
               this.OptionalEquals(smsNotification);

        public Boolean OptionalEquals(SMSNotification other)

            => PhoneNumber.  Equals(other.PhoneNumber) &&
               TextTemplate?.Equals(other.TextTemplate) == true;

        #endregion


        #region SortKey

        public override String SortKey

            => String.Concat(nameof(EMailNotification),
                             PhoneNumber,
                             TextTemplate);

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
