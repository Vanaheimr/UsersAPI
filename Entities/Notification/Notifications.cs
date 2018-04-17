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

#endregion

namespace org.GraphDefined.OpenData.Users
{

    public class Notifications
    {

        public class Multiplexer
        {

            private List<ANotificationType> _NotificationTypes;
            public IEnumerable<ANotificationType> NotificationTypes
                => _NotificationTypes;

            public Dictionary<Notification_Id, List<ANotificationType>>  NotificationIds     { get; }

            public Multiplexer()
            {
                this._NotificationTypes  = new List<ANotificationType>();
                this.NotificationIds     = new Dictionary<Notification_Id, List<ANotificationType>>();
            }


            public Multiplexer Add(ANotificationType NotificationType)
            {
                _NotificationTypes.Add(NotificationType);
                return this;
            }

            public Multiplexer Remove(ANotificationType NotificationType)
            {
                _NotificationTypes.Remove(NotificationType);
                return this;
            }

        }

        #region Data

        private readonly Dictionary<User_Id, Multiplexer> NotificationLookup;

        #endregion

        #region Events

        public delegate void OnNotificationDelegate(DateTime Timestamp, User_Id User, Notification_Id? NotificationId, ANotificationType Notification);

        public event OnNotificationDelegate OnAdded;

        public event OnNotificationDelegate OnRemoved;

        #endregion

        #region Constructor(s)

        public Notifications()
        {

            this.NotificationLookup = new Dictionary<User_Id, Multiplexer>();

        }

        #endregion


        #region Add(User,   NotificationType, EqualityComparer)

        public Notifications Add<T>(User                 User,
                                    T                    NotificationType,
                                    Func<T, T, Boolean>  EqualityComparer)

            where T : ANotificationType

        {

            if (User == null)
                throw new ArgumentNullException(nameof(User), "The given user must not be null!");

            return Add(User.Id,
                       NotificationType,
                       EqualityComparer);

        }

        #endregion

        #region Add(UserId, NotificationType, EqualityComparer)

        public Notifications Add<T>(User_Id              UserId,
                                    T                    NotificationType,
                                    Func<T, T, Boolean>  EqualityComparer)

            where T : ANotificationType

        {

            lock (NotificationLookup)
            {

                if (!NotificationLookup.TryGetValue(UserId, out Multiplexer Multiplexer))
                {
                    Multiplexer = new Multiplexer();
                    NotificationLookup.Add(UserId, Multiplexer);
                }

                var Found = false;

                foreach (var notificationtype in Multiplexer.NotificationTypes.ToArray())
                {
                    if (notificationtype is T mailnotification &&
                        EqualityComparer(mailnotification, NotificationType))
                    {
                        Found = true;
                    }
                }

                if (!Found)
                {

                    Multiplexer.Add(NotificationType);

                    OnAdded?.Invoke(DateTime.UtcNow,
                                    UserId,
                                    null,
                                    NotificationType);

                }

                return this;

            }

        }

        #endregion

        #region Add(User,   NotificationId, NotificationType, EqualityComparer)

        public Notifications Add<T>(User                 User,
                                    Notification_Id      NotificationId,
                                    T                    NotificationType,
                                    Func<T, T, Boolean>  EqualityComparer)

            where T : ANotificationType

        {

            if (User == null)
                throw new ArgumentNullException(nameof(User), "The given user must not be null!");

            return Add(User.Id,
                       NotificationId,
                       NotificationType,
                       EqualityComparer);

        }

        #endregion

        #region Add(UserId, NotificationId, NotificationType, EqualityComparer)

        public Notifications Add<T>(User_Id              User,
                                    Notification_Id      NotificationId,
                                    T                    NotificationType,
                                    Func<T, T, Boolean>  EqualityComparer)

            where T : ANotificationType

        {

            lock (NotificationLookup)
            {

                if (!NotificationLookup.TryGetValue(User, out Multiplexer Multiplexer))
                {
                    Multiplexer = new Multiplexer();
                    NotificationLookup.Add(User, Multiplexer);
                }

                if (!Multiplexer.NotificationIds.TryGetValue(NotificationId, out List<ANotificationType> NotificationTypes))
                {
                    NotificationTypes = new List<ANotificationType>();
                    Multiplexer.NotificationIds.Add(NotificationId, NotificationTypes);
                }

                var Found = false;

                foreach (var notificationtype in NotificationTypes.ToArray())
                {
                    if (notificationtype is T mailnotification &&
                        EqualityComparer(mailnotification, NotificationType))
                    {
                        Found = true;
                    }
                }

                if (!Found)
                {

                    NotificationTypes.Add(NotificationType);

                    OnAdded?.Invoke(DateTime.UtcNow,
                                    User,
                                    NotificationId,
                                    NotificationType);

                }

                return this;

            }

        }

        #endregion


        #region GetNotifications(User)

        public Multiplexer GetNotifications(User User)
        {

            if (User == null)
                throw new ArgumentNullException(nameof(User), "The given user must not be null!");

            return GetNotifications(User.Id);

        }

        #endregion

        #region GetNotifications(UserId)

        public Multiplexer GetNotifications(User_Id UserId)
        {

            lock (NotificationLookup)
            {

                if (NotificationLookup.TryGetValue(UserId, out Multiplexer Multiplexer))
                    return Multiplexer;

                return null;

            }

        }

        #endregion

        #region GetNotifications(User,   NotificationId)

        public IEnumerable<T> GetNotifications<T>(User             User,
                                                  Notification_Id  NotificationId)

            where T : ANotificationType

        {

            if (User == null)
                throw new ArgumentNullException(nameof(User), "The given user must not be null!");

            return GetNotifications<T>(User.Id,
                                       NotificationId);

        }

        #endregion

        #region GetNotifications(UserId, NotificationId)

        public IEnumerable<T> GetNotifications<T>(User_Id          UserId,
                                                  Notification_Id  NotificationId)

            where T : ANotificationType

        {

            lock (NotificationLookup)
            {

                if (NotificationLookup.TryGetValue(UserId, out Multiplexer Multiplexer))
                {

                    if (Multiplexer.NotificationIds.TryGetValue(NotificationId, out List<ANotificationType>  NotificationTypes))
                        return NotificationTypes.
                                   Where(notificationtype => notificationtype is T).
                                   Cast<T>();

                    else
                        return Multiplexer.NotificationTypes.
                                   Where(notificationtype => notificationtype is T).
                                   Cast<T>();

                }

            }

            return new T[0];

        }

        #endregion


        #region Remove(User,   EqualityComparer)

        public Notifications Remove<T>(User              User,
                                       Func<T, Boolean>  EqualityComparer)

            where T : ANotificationType

        {

            if (User == null)
                throw new ArgumentNullException(nameof(User), "The given user must not be null!");

            return Remove(User.Id,
                          EqualityComparer);

        }

        #endregion

        #region Remove(UserId, EqualityComparer)

        public Notifications Remove<T>(User_Id           UserId,
                                       Func<T, Boolean>  EqualityComparer)

            where T : ANotificationType

        {

            lock (NotificationLookup)
            {

                if (NotificationLookup.TryGetValue(UserId, out Multiplexer Multiplexer))
                {

                    foreach (var notificationtype in Multiplexer.NotificationTypes.ToArray())
                    {
                        if (notificationtype is T mailnotification &&
                            EqualityComparer(mailnotification))
                        {

                            Multiplexer.Remove(notificationtype);

                            OnRemoved?.Invoke(DateTime.UtcNow,
                                              UserId,
                                              null,
                                              notificationtype);

                        }
                    }

                }

            }

            return this;

        }

        #endregion

        #region Remove(User,   NotificationId, EqualityComparer)

        public Notifications Remove<T>(User              User,
                                       Notification_Id   NotificationId,
                                       Func<T, Boolean>  EqualityComparer)

            where T : ANotificationType

        {

            if (User == null)
                throw new ArgumentNullException(nameof(User), "The given user must not be null!");

            return Remove(User.Id,
                          NotificationId,
                          EqualityComparer);

        }

        #endregion

        #region Remove(UserId, NotificationId, EqualityComparer)

        public Notifications Remove<T>(User_Id           UserId,
                                       Notification_Id   NotificationId,
                                       Func<T, Boolean>  EqualityComparer)

            where T : ANotificationType

        {

            lock (NotificationLookup)
            {

                if (NotificationLookup.         TryGetValue(UserId,           out Multiplexer              Multiplexer) &&
                    Multiplexer.NotificationIds.TryGetValue(NotificationId, out List<ANotificationType>  NotificationTypes))
                {

                    foreach (var notificationtype in NotificationTypes.ToArray())
                    {
                        if (notificationtype is T mailnotification &&
                            EqualityComparer(mailnotification))
                        {

                            NotificationTypes.Remove(notificationtype);

                            OnRemoved?.Invoke(DateTime.UtcNow,
                                              UserId,
                                              NotificationId,
                                              notificationtype);

                        }
                    }

                }

            }

            return this;

        }

        #endregion


    }

}
