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

    public static class EMailNotificationExtentions
    {

        public static Notifications RegisterEMailNotification(this UsersAPI  UsersAPI,
                                                              User_Id        User,
                                                              EMailAddress   EMailAddress,
                                                              String         Subject = null)

            => UsersAPI.RegisterNotification(User,
                                             new EMailNotification(EMailAddress, Subject),
                                             (a, b) => a.EMailAddress == b.EMailAddress);

        public static Notifications RegisterEMailNotification(this UsersAPI  UsersAPI,
                                                              User           User,
                                                              EMailAddress   EMailAddress,
                                                              String         Subject = null)

            => UsersAPI.RegisterNotification(User.Id,
                                             new EMailNotification(EMailAddress, Subject),
                                             (a, b) => a.EMailAddress == b.EMailAddress);


        public static Notifications RegisterEMailNotification(this UsersAPI    UsersAPI,
                                                              User_Id          User,
                                                              Notification_Id  NotificationId,
                                                              EMailAddress     EMailAddress,
                                                              String           Subject = null)

            => UsersAPI.RegisterNotification(User,
                                             NotificationId,
                                             new EMailNotification(EMailAddress, Subject),
                                             (a, b) => a.EMailAddress == b.EMailAddress);

        public static Notifications RegisterEMailNotification(this UsersAPI    UsersAPI,
                                                              User             User,
                                                              Notification_Id  NotificationId,
                                                              EMailAddress     EMailAddress,
                                                              String           Subject = null)

            => UsersAPI.RegisterNotification(User.Id,
                                             NotificationId,
                                             new EMailNotification(EMailAddress, Subject),
                                             (a, b) => a.EMailAddress == b.EMailAddress);




        public static IEnumerable<EMailNotification> GetEMailNotifications(this UsersAPI    UsersAPI,
                                                                           User             User,
                                                                           Notification_Id  NotificationId)


            => UsersAPI.GetNotifications<EMailNotification>(User,
                                                            NotificationId);

        public static IEnumerable<EMailNotification> GetEMailNotifications(this UsersAPI    UsersAPI,
                                                                           User_Id          User,
                                                                           Notification_Id  NotificationId)


            => UsersAPI.GetNotifications<EMailNotification>(User,
                                                            NotificationId);




        public static Notifications UnregisterEMailNotification(this UsersAPI  UsersAPI,
                                                                User           User,
                                                                EMailAddress   EMailAddress)

            => UsersAPI.UnregisterNotification<EMailNotification>(User,
                                                                  a => a.EMailAddress == EMailAddress);

        public static Notifications UnregisterEMailNotification(this UsersAPI  UsersAPI,
                                                                User_Id        User,
                                                                EMailAddress   EMailAddress)

            => UsersAPI.UnregisterNotification<EMailNotification>(User,
                                                                  a => a.EMailAddress == EMailAddress);


        public static Notifications UnregisterEMailNotification(this UsersAPI    UsersAPI,
                                                                User             User,
                                                                Notification_Id  NotificationId,
                                                                EMailAddress     EMailAddress)

            => UsersAPI.UnregisterNotification<EMailNotification>(User,
                                                                  NotificationId,
                                                                  a => a.EMailAddress == EMailAddress);

        public static Notifications UnregisterEMailNotification(this UsersAPI    UsersAPI,
                                                                User_Id          User,
                                                                Notification_Id  NotificationId,
                                                                EMailAddress     EMailAddress)

            => UsersAPI.UnregisterNotification<EMailNotification>(User,
                                                                  NotificationId,
                                                                  a => a.EMailAddress == EMailAddress);

    }


    public class EMailNotification : ANotificationType
    {

        public EMailAddress  EMailAddress   { get; }
        public String        Subject        { get; }


        public EMailNotification(EMailAddress  EMailAddress,
                                 String        Subject = null)
        {

            this.EMailAddress  = EMailAddress;
            this.Subject       = Subject;

        }


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

        public override JObject GetAsJSON(JObject JSON)
        {

            JSON.Add(new JProperty("type",     GetType().Name));
            JSON.Add(new JProperty("username", EMailAddress.OwnerName.ToString()));
            JSON.Add(new JProperty("email",    EMailAddress.Address.  ToString()));

            if (Subject.IsNotNullOrEmpty())
                JSON.Add(new JProperty("subject", Subject));

            return JSON;

        }

    }

}
