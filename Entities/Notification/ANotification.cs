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
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.OpenData.Notifications
{

    /// <summary>
    /// An abstract notification.
    /// </summary>
    public abstract class ANotification : IEnumerable<NotificationMessageType>,
                                          IEquatable <ANotification>,
                                          IComparable<ANotification>,
                                          IComparable
    {

        #region Properties

        private readonly HashSet<NotificationMessageType> _NotificationMessageTypes;

        public IEnumerable<NotificationMessageType> NotificationMessageTypes
            => _NotificationMessageTypes;


        public Int32 Count
            => _NotificationMessageTypes.Count;


        public abstract String SortKey { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create an new abstract notification.
        /// </summary>
        protected ANotification()
        {
            this._NotificationMessageTypes = new HashSet<NotificationMessageType>();
        }

        #endregion


        #region Add     (NotificationMessageType)

        public void Add(NotificationMessageType NotificationMessageType)
        {
            lock (_NotificationMessageTypes)
            {
                _NotificationMessageTypes.Add(NotificationMessageType);
            }
        }

        #endregion

        #region Add     (NotificationMessageTypes)

        public void Add(IEnumerable<NotificationMessageType> NotificationMessageTypes)
        {
            lock (_NotificationMessageTypes)
            {
                foreach (var NotificationMessageType in NotificationMessageTypes)
                    _NotificationMessageTypes.Add(NotificationMessageType);
            }
        }

        #endregion

        #region Contains(NotificationMessageType)

        public Boolean Contains(NotificationMessageType NotificationMessageType)
        {
            lock (_NotificationMessageTypes)
            {
                return _NotificationMessageTypes.Contains(NotificationMessageType);
            }
        }

        #endregion

        #region IEnumerable<NotificationMessageType> Members

        public IEnumerator<NotificationMessageType> GetEnumerator()
            => _NotificationMessageTypes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _NotificationMessageTypes.GetEnumerator();

        #endregion


        public abstract JObject ToJSON();


        #region IComparable<ANotification> Members

        public abstract Int32 CompareTo(ANotification other);

        public Int32 CompareTo(Object obj)
            => 0;

        #endregion

        #region IEquatable<ANotification> Members

        public abstract Boolean Equals(ANotification other);

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
            => SortKey;

        #endregion

    }

}
