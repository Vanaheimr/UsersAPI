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
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.OpenData.Notifications
{

    public class NotificationStore
    {

        #region Data

        private readonly List<ANotification> _NotificationTypes;

        public IEnumerable<ANotification> NotificationTypes
            => _NotificationTypes;

        #endregion

        #region Events

        public delegate void OnNotificationDelegate(DateTime                      Timestamp,
                                                    IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                                    ANotification             Notification);

        public event OnNotificationDelegate OnAdded;

        public event OnNotificationDelegate OnRemoved;

        #endregion

        #region Constructor(s)

        public NotificationStore()
        {
            this._NotificationTypes  = new List<ANotification>();
        }

        #endregion


        #region Add(NotificationType)

        public NotificationStore Add<T>(T NotificationType)

            where T : ANotification

        {

            lock (_NotificationTypes)
            {

                var notification = _NotificationTypes.OfType<T>().FirstOrDefault(typeT => typeT.Equals(NotificationType));

                if (notification == null)
                {

                    _NotificationTypes.Add(NotificationType);

                    OnAdded?.Invoke(DateTime.UtcNow,
                                    null,
                                    NotificationType);

                }

                return this;

            }

        }

        #endregion

        #region Add(NotificationType, NotificationMessageType)

        public NotificationStore Add<T>(T                        NotificationType,
                                        NotificationMessageType  NotificationMessageType)

            where T : ANotification

        {

            lock (_NotificationTypes)
            {

                var notification = _NotificationTypes.OfType<T>().FirstOrDefault(typeT => typeT.Equals(NotificationType));

                if (notification == null)
                {

                    _NotificationTypes.Add(NotificationType);

                    notification = NotificationType;

                    OnAdded?.Invoke(DateTime.UtcNow,
                                    null,
                                    NotificationType);

                }

                notification.Add(NotificationMessageType);

                return this;

            }

        }

        #endregion

        #region Add(NotificationType, NotificationMessageTypes)

        public NotificationStore Add<T>(T                                     NotificationType,
                                        IEnumerable<NotificationMessageType>  NotificationMessageTypes)

            where T : ANotification

        {

            lock (_NotificationTypes)
            {

                var notification = _NotificationTypes.OfType<T>().FirstOrDefault(typeT => typeT.Equals(NotificationType));

                if (notification == null)
                {

                    _NotificationTypes.Add(NotificationType);

                    notification = NotificationType;

                    OnAdded?.Invoke(DateTime.UtcNow,
                                    null,
                                    NotificationType);

                }

                notification.Add(NotificationMessageTypes);

                return this;

            }

        }

        #endregion


        #region GetNotifications  (NotificationMessageType = null)

        public IEnumerable<ANotification> GetNotifications(NotificationMessageType?  NotificationMessageType = null)
        {

            lock (_NotificationTypes)
            {

                return NotificationMessageType.HasValue
                           ? _NotificationTypes.Where(typeT => typeT.Contains(NotificationMessageType.Value))
                           : _NotificationTypes;

            }

        }

        #endregion

        #region GetNotificationsOf(NotificationMessageType = null)

        public IEnumerable<T> GetNotificationsOf<T>(NotificationMessageType?  NotificationMessageType = null)

            where T : ANotification

        {

            lock (_NotificationTypes)
            {

                return NotificationMessageType.HasValue
                           ? _NotificationTypes.OfType<T>().Where(typeT => typeT.Contains(NotificationMessageType.Value))
                           : _NotificationTypes.OfType<T>();

            }

        }

        #endregion


        //#region Remove(                          EqualityComparer)

        //public Notifications Remove<T>(Func<T, Boolean>  EqualityComparer)

        //    where T : ANotificationType

        //{

        //    lock (NotificationTypes)
        //    {

        //            foreach (var notificationtype in NotificationTypes.ToArray())
        //            {
        //                if (notificationtype is T mailnotification &&
        //                    EqualityComparer(mailnotification))
        //                {

        //                    Remove(notificationtype);

        //                    OnRemoved?.Invoke(DateTime.UtcNow,
        //                                      null,
        //                                      notificationtype);

        //                }
        //            }

        //    }

        //    return this;

        //}

        //#endregion

        //#region Remove(NotificationMessageType,  EqualityComparer)

        //public Notifications Remove<T>(NotificationMessageType   NotificationMessageType)

        //    where T : ANotificationType

        //{

        //    lock (NotificationMessageTypes)
        //    {

        //        if (NotificationMessageTypes.TryGetValue(NotificationMessageType,  out List<ANotificationType>  NotificationTypes))
        //        {

        //            foreach (var notificationtype in NotificationTypes.ToArray())
        //            {
        //                if (notificationtype is T mailnotification &&
        //                    EqualityComparer(mailnotification))
        //                {

        //                    NotificationTypes.Remove(notificationtype);

        //                    //OnRemoved?.Invoke(DateTime.UtcNow,
        //                    //                  NotificationMessageType,
        //                    //                  notificationtype);

        //                }
        //            }

        //        }

        //    }

        //    return this;

        //}

        //#endregion

        //#region Remove(NotificationMessageTypes, EqualityComparer)

        //public Notifications Remove<T>(IEnumerable<NotificationMessageType>  NotificationMessageType,
        //                               Func<T, Boolean>              EqualityComparer)

        //    where T : ANotificationType

        //{

        //    lock (NotificationMessageTypes)
        //    {

        //        if (NotificationMessageTypes.TryGetValue(NotificationMessageType,  out List<ANotificationType>  NotificationTypes))
        //        {

        //            foreach (var notificationtype in NotificationTypes.ToArray())
        //            {
        //                if (notificationtype is T mailnotification &&
        //                    EqualityComparer(mailnotification))
        //                {

        //                    NotificationTypes.Remove(notificationtype);

        //                    //OnRemoved?.Invoke(DateTime.UtcNow,
        //                    //                  NotificationMessageType,
        //                    //                  notificationtype);

        //                }
        //            }

        //        }

        //    }

        //    return this;

        //}

        //#endregion


        public JArray ToJSON()
            => new JArray(_NotificationTypes.SafeSelect(_ => _.ToJSON()));


    }

}
