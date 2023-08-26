/*
 * Copyright (c) 2014-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

#endregion

namespace social.OpenData.UsersAPI
{

    public struct MessageHolder<TId, TMessage> : IEquatable<MessageHolder<TId, TMessage>>,
                                                 IComparable<MessageHolder<TId, TMessage>>,
                                                 IComparable

        where TId : IComparable

    {

        public TId                  Id         { get; }
        public TMessage             Message    { get; }
        public Func<TId, TMessage>  Lookup     { get; }

        public MessageHolder(TId                  Id,
                             TMessage             Message  = default,
                             Func<TId, TMessage>  Lookup   = null)
        {

            this.Id       = Id;
            this.Message  = Message;
            this.Lookup   = Lookup;// ?? throw new ArgumentNullException(nameof(Lookup), "The given message lookup must not be null!");

        }


        #region IComparable<MessageHolder> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is MessageHolder<TId, TMessage> MessageHolder))
                throw new ArgumentException("The given object is not a message holder!");

            return CompareTo(MessageHolder);

        }

        #endregion

        #region CompareTo(MessageHolder)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageHolder">A message holder object to compare with.</param>
        public Int32 CompareTo(MessageHolder<TId, TMessage> MessageHolder)
        {

            if ((Object) MessageHolder == null)
                throw new ArgumentNullException(nameof(ServiceTicket), "The given message holder must not be null!");

            return Id.CompareTo(MessageHolder.Id);

        }

        #endregion

        #endregion

        #region IEquatable<MessageHolder> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is null)
                return false;

            if (!(Object is MessageHolder<TId, TMessage> MessageHolder))
                return false;

            return Equals(MessageHolder);

        }

        #endregion

        #region Equals(MessageHolder)

        /// <summary>
        /// Compares two message holders for equality.
        /// </summary>
        /// <param name="MessageHolder">A message holder to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(MessageHolder<TId, TMessage> MessageHolder)
        {

            if ((Object) MessageHolder == null)
                return false;

            return Id.Equals(MessageHolder.Id);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => Id.GetHashCode();

        #endregion

    }

}
