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

    /// <summary>
    /// A store for all notifications.
    /// </summary>
    public class NotificationStore
    {

        #region Data

        private readonly List<ANotification> _NotificationTypes;

        public IEnumerable<ANotification> NotificationTypes
            => _NotificationTypes;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new notification store.
        /// </summary>
        public NotificationStore()
        {
            this._NotificationTypes  = new List<ANotification>();
        }

        #endregion


        #region Add(NotificationType,                           OnUpdate = null)

        public T Add<T>(T          NotificationType,
                        Action<T>  OnUpdate  = null)

            where T : ANotification

        {

            lock (_NotificationTypes)
            {

                var notification = _NotificationTypes.OfType<T>().FirstOrDefault(typeT => typeT.Equals(NotificationType));

                if (notification == null)
                {
                    _NotificationTypes.Add(NotificationType);
                    notification = NotificationType;
                    OnUpdate?.Invoke(notification);
                }

                else
                {
                    // When reloaded from disc: Merge notifications.
                    var Updated = false;

                    // Some optional parameters are different...
                    if (!NotificationType.OptionalEquals(notification))
                    {
                        _NotificationTypes.Remove(notification);
                        _NotificationTypes.Add   (NotificationType);
                    }

                    else
                    {

                        foreach (var notificationMessageType in NotificationType.NotificationMessageTypes)
                        {
                            if (!notification.Contains(notificationMessageType))
                            {
                                notification.Add(notificationMessageType,
                                                 () => Updated = true);
                            }
                        }

                        if (Updated)
                            OnUpdate?.Invoke(notification);

                    }

                }

                return notification;

            }

        }

        #endregion

        #region Add(NotificationType, NotificationMessageType,  OnUpdate = null)

        public T Add<T>(T                        NotificationType,
                        NotificationMessageType  NotificationMessageType,
                        Action<T>                OnUpdate  = null)

            where T : ANotification

        {

            lock (_NotificationTypes)
            {

                var notification = _NotificationTypes.OfType<T>().FirstOrDefault(typeT => typeT.Equals(NotificationType));

                if (notification == null)
                {
                    _NotificationTypes.Add(NotificationType);
                    notification = NotificationType;
                }

                notification.Add(NotificationMessageType,
                                 () => OnUpdate?.Invoke(notification));

                return notification;

            }

        }

        #endregion

        #region Add(NotificationType, NotificationMessageTypes, OnUpdate = null)

        public T Add<T>(T                                     NotificationType,
                        IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                        Action<T>                             OnUpdate  = null)

            where T : ANotification

        {

            lock (_NotificationTypes)
            {

                var notification = _NotificationTypes.OfType<T>().FirstOrDefault(typeT => typeT.Equals(NotificationType));

                if (notification == null)
                {
                    _NotificationTypes.Add(NotificationType);
                    notification = NotificationType;
                }

                notification.Add(NotificationMessageTypes,
                                 () => OnUpdate?.Invoke(notification));

                return notification;

            }

        }

        #endregion


        #region GetNotifications  (NotificationMessageType = null)

        public IEnumerable<ANotification> GetNotifications(NotificationMessageType?  NotificationMessageType = null)
        {

            lock (_NotificationTypes)
            {

                var results = NotificationMessageType.HasValue
                                  ? _NotificationTypes.Where(typeT => typeT.Contains(NotificationMessageType.Value)).ToArray()
                                  : _NotificationTypes.ToArray();

                // When no specialized notification was found... return a general notification!
                return results.Length > 0
                           ? results
                           : _NotificationTypes.Where(typeT => typeT.Count == 0);

            }

        }

        #endregion

        #region GetNotificationsOf(NotificationMessageType = null)

        public IEnumerable<T> GetNotificationsOf<T>(NotificationMessageType?  NotificationMessageType = null)

            where T : ANotification

        {

            lock (_NotificationTypes)
            {

                var results = NotificationMessageType.HasValue
                                  ? _NotificationTypes.OfType<T>().Where(typeT => typeT.Contains(NotificationMessageType.Value)).ToArray()
                                  : _NotificationTypes.OfType<T>().ToArray();

                // When no specialized notification was found... return a general notification!
                return results.Length > 0
                           ? results
                           : _NotificationTypes.OfType<T>().Where(typeT => typeT.Count == 0);

            }

        }

        #endregion

        #region GetNotifications  (NotificationMessageTypeFilter)

        public IEnumerable<ANotification> GetNotifications(Func<NotificationMessageType, Boolean>  NotificationMessageTypeFilter)
        {

            lock (_NotificationTypes)
            {

                if (NotificationMessageTypeFilter == null)
                    NotificationMessageTypeFilter = (_ => true);

                var results = _NotificationTypes.Where(typeT => typeT.Any(NotificationMessageTypeFilter)).ToArray();

                // When no specialized notification was found... return a general notification!
                return results.Length > 0
                           ? results
                           : _NotificationTypes.Where(typeT => typeT.Count == 0);

            }

        }

        #endregion

        #region GetNotificationsOf(NotificationMessageTypeFilter)

        public IEnumerable<T> GetNotificationsOf<T>(Func<NotificationMessageType, Boolean> NotificationMessageTypeFilter)

            where T : ANotification

        {

            lock (_NotificationTypes)
            {

                if (NotificationMessageTypeFilter == null)
                    NotificationMessageTypeFilter = (_ => true);

                var results = _NotificationTypes.OfType<T>().Where(typeT => typeT.Any(NotificationMessageTypeFilter)).ToArray();

                // When no specialized notification was found... return a general notification!
                return results.Length > 0
                           ? results
                           : _NotificationTypes.OfType<T>().Where(typeT => typeT.Count == 0);

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
            => new JArray(_NotificationTypes.SafeSelect(_ => _.ToJSON(false)));


    }

}
