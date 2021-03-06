﻿/*
 * Copyright (c) 2014-2021, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using System.Collections;

#endregion

namespace social.OpenData.UsersAPI.Notifications
{

    /// <summary>
    /// A store for all notifications.
    /// </summary>
    public class NotificationStore : IEnumerable<ANotification>
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

                // Create a new notification...
                if (notification == null)
                {
                    _NotificationTypes.Add(NotificationType);
                    notification = NotificationType;
                    OnUpdate?.Invoke(notification);
                }

                else
                {
                    // When reloaded from disc: Merge notifications.
                    //var Updated = false;

                    // Some optional parameters are different...
                    if (!NotificationType.OptionalEquals(notification))
                    {

                        _NotificationTypes.Remove(notification);
                        _NotificationTypes.Add   (NotificationType);

                        OnUpdate?.Invoke(NotificationType);

                    }

                    //else
                    //{

                    //    foreach (var notificationMessageType in NotificationType.NotificationMessageTypes)
                    //    {

                    //        notification.Clear();

                    //        if (!notification.Contains(notificationMessageType))
                    //        {
                    //            notification.Add(notificationMessageType,
                    //                             () => Updated = true);
                    //        }
                    //    }

                    //}

                    //if (Updated)
                    //    OnUpdate?.Invoke(notification);

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

        #region Add(Notifications)

        public void Add<T>(IEnumerable<T>  Notifications)
            where T : ANotification
        {

            lock (_NotificationTypes)
            {
                _NotificationTypes.AddRange(Notifications);
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

        #region GetNotificationsOf(params NotificationMessageTypes)

        public IEnumerable<T> GetNotificationsOf<T>(params NotificationMessageType[]  NotificationMessageTypes)

            where T : ANotification

        {

            lock (_NotificationTypes)
            {

                var results = NotificationMessageTypes != null && NotificationMessageTypes.Length > 0
                                  ? _NotificationTypes.OfType<T>().Where(typeT => typeT.Contains(NotificationMessageTypes)).ToArray()
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


        #region Remove(NotificationType, OnRemoval = null)

        public T Remove<T>(T          NotificationType,
                           Action<T>  OnRemoval  = null)

            where T : ANotification

        {

            lock (_NotificationTypes)
            {

                foreach (var notification in _NotificationTypes.OfType<T>().Where(typeT => typeT.Equals(NotificationType)).ToArray())
                {
                    _NotificationTypes.Remove(notification);
                    OnRemoval?.Invoke(notification);
                }

                return null;

            }

        }

        #endregion


        public JArray ToJSON()

            => new JArray(_NotificationTypes.
                              OrderBy   (notification => notification.SortKey).
                              SafeSelect(notification => notification.ToJSON(false)));


        public JObject ToJSON(UInt32 Number)

            => _NotificationTypes.
                  OrderBy(notification => notification.SortKey).
                   Skip(Number - 1).
                   FirstOrDefault()?.
                   ToJSON(false)

                ?? new JObject();

        public IEnumerator<ANotification> GetEnumerator()
        {
            lock (_NotificationTypes)
            {
                return new List<ANotification>(_NotificationTypes).GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (_NotificationTypes)
            {
                return new List<ANotification>(_NotificationTypes).GetEnumerator();
            }
        }

    }

}
