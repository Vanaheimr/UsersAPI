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

    public static class SMSNotificationExtentions
    {

        public static Notifications RegisterSMSNotification(this UsersAPI  UsersAPI,
                                                            User_Id        User,
                                                            PhoneNumber    Phonenumber,
                                                            String         TextTemplate = null)

            => UsersAPI.RegisterNotification(User,
                                             new SMSNotification(Phonenumber, TextTemplate),
                                             (a, b) => a.Phonenumber == b.Phonenumber);

        public static Notifications RegisterSMSNotification(this UsersAPI  UsersAPI,
                                                            User           User,
                                                            PhoneNumber    Phonenumber,
                                                            String         TextTemplate = null)

            => UsersAPI.RegisterNotification(User.Id,
                                             new SMSNotification(Phonenumber, TextTemplate),
                                             (a, b) => a.Phonenumber == b.Phonenumber);


        public static Notifications RegisterSMSNotification(this UsersAPI    UsersAPI,
                                                            User_Id          User,
                                                            Notification_Id  NotificationId,
                                                            PhoneNumber      Phonenumber,
                                                            String           TextTemplate = null)

            => UsersAPI.RegisterNotification(User,
                                             NotificationId,
                                             new SMSNotification(Phonenumber, TextTemplate),
                                             (a, b) => a.Phonenumber == b.Phonenumber);

        public static Notifications RegisterSMSNotification(this UsersAPI    UsersAPI,
                                                            User             User,
                                                            Notification_Id  NotificationId,
                                                            PhoneNumber      Phonenumber,
                                                            String           TextTemplate = null)

            => UsersAPI.RegisterNotification(User.Id,
                                             NotificationId,
                                             new SMSNotification(Phonenumber, TextTemplate),
                                             (a, b) => a.Phonenumber == b.Phonenumber);




        public static IEnumerable<SMSNotification> GetSMSNotifications(this UsersAPI    UsersAPI,
                                                                       User             User,
                                                                       Notification_Id  NotificationId)


            => UsersAPI.GetNotifications<SMSNotification>(User,
                                                            NotificationId);

        public static IEnumerable<SMSNotification> GetSMSNotifications(this UsersAPI    UsersAPI,
                                                                       User_Id          User,
                                                                       Notification_Id  NotificationId)


            => UsersAPI.GetNotifications<SMSNotification>(User,
                                                          NotificationId);




        public static Notifications UnregisterSMSNotification(this UsersAPI  UsersAPI,
                                                              User           User,
                                                              PhoneNumber   Phonenumber)

            => UsersAPI.UnregisterNotification<SMSNotification>(User,
                                                                a => a.Phonenumber == Phonenumber);

        public static Notifications UnregisterSMSNotification(this UsersAPI  UsersAPI,
                                                              User_Id        User,
                                                              PhoneNumber   Phonenumber)

            => UsersAPI.UnregisterNotification<SMSNotification>(User,
                                                                a => a.Phonenumber == Phonenumber);


        public static Notifications UnregisterSMSNotification(this UsersAPI    UsersAPI,
                                                              User             User,
                                                              Notification_Id  NotificationId,
                                                              PhoneNumber     Phonenumber)

            => UsersAPI.UnregisterNotification<SMSNotification>(User,
                                                                NotificationId,
                                                                a => a.Phonenumber == Phonenumber);

        public static Notifications UnregisterSMSNotification(this UsersAPI    UsersAPI,
                                                              User_Id          User,
                                                              Notification_Id  NotificationId,
                                                              PhoneNumber     Phonenumber)

            => UsersAPI.UnregisterNotification<SMSNotification>(User,
                                                                NotificationId,
                                                                a => a.Phonenumber == Phonenumber);

    }

    public class SMSNotification : ANotificationType
    {

        public PhoneNumber Phonenumber    { get; }
        public String      TextTemplate   { get; }


        public SMSNotification(PhoneNumber Phonenumber,
                               String      TextTemplate = null)
        {

            this.Phonenumber   = Phonenumber;
            this.TextTemplate  = TextTemplate;

        }

        public static SMSNotification Parse(JObject JSON)
        {

            if (JSON["type"]?.Value<String>() != typeof(SMSNotification).Name)
                throw new ArgumentException();

            return new SMSNotification(PhoneNumber.Parse(JSON["phoneNumber"]?.Value<String>()),
                                       JSON["text"]?.Value<String>());

        }

        public override JObject GetAsJSON(JObject JSON)
        {

            JSON.Add(new JProperty("type",         GetType().Name));
            JSON.Add(new JProperty("phoneNumber",  Phonenumber.ToString()));

            if (TextTemplate.IsNotNullOrEmpty())
                JSON.Add(new JProperty("textTemplate",  TextTemplate));

            return JSON;

        }

    }

}
