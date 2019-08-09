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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.OpenData.Notifications
{

    /// <summary>
    /// The unique identification of a notification message.
    /// </summary>
    public struct NotificationMessage_Id : IId,
                                           IEquatable<NotificationMessage_Id>,
                                           IComparable<NotificationMessage_Id>

    {

        #region Data

        private static readonly Random _random = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String  InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the notification identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId?.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new notification identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the notification identification.</param>
        private NotificationMessage_Id(String  String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Random  (Size)

        /// <summary>
        /// Create a random notification message identification.
        /// </summary>
        /// <param name="Size">The expected size of the notification message identification.</param>
        public static NotificationMessage_Id Random(UInt16? Size = 64)

            => new NotificationMessage_Id(_random.RandomString(Size ?? 64));

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a notification identification.
        /// </summary>
        /// <param name="Text">A text representation of a notification identification.</param>
        public static NotificationMessage_Id Parse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a notification identification must not be null or empty!");

            #endregion

            return new NotificationMessage_Id(Text);

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a notification identification.
        /// </summary>
        /// <param name="Text">A text representation of a notification identification.</param>
        public static NotificationMessage_Id? TryParse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a notification identification must not be null or empty!");

            #endregion

            if (TryParse(Text, out NotificationMessage_Id _NotificationId))
                return _NotificationId;

            return new NotificationMessage_Id?();

        }

        #endregion

        #region (static) TryParse(Text, out NotificationId)

        /// <summary>
        /// Try to parse the given string as a notification identification.
        /// </summary>
        /// <param name="Text">A text representation of a notification identification.</param>
        /// <param name="NotificationId">The parsed notification identification.</param>
        public static Boolean TryParse(String Text, out NotificationMessage_Id NotificationId)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a notification identification must not be null or empty!");

            #endregion

            try
            {
                NotificationId = new NotificationMessage_Id(Text);
                return true;
            }
            catch (Exception)
            {
                NotificationId = default(NotificationMessage_Id);
                return false;
            }

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this notification identification.
        /// </summary>
        public NotificationMessage_Id Clone
            => new NotificationMessage_Id(new String(InternalId.ToCharArray()));

        #endregion


        #region Operator overloading

        #region Operator == (NotificationId1, NotificationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotificationId1">A notification identification.</param>
        /// <param name="NotificationId2">Another notification identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (NotificationMessage_Id NotificationId1, NotificationMessage_Id NotificationId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(NotificationId1, NotificationId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) NotificationId1 == null) || ((Object) NotificationId2 == null))
                return false;

            return NotificationId1.Equals(NotificationId2);

        }

        #endregion

        #region Operator != (NotificationId1, NotificationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotificationId1">A notification identification.</param>
        /// <param name="NotificationId2">Another notification identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (NotificationMessage_Id NotificationId1, NotificationMessage_Id NotificationId2)
            => !(NotificationId1 == NotificationId2);

        #endregion

        #region Operator <  (NotificationId1, NotificationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotificationId1">A notification identification.</param>
        /// <param name="NotificationId2">Another notification identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (NotificationMessage_Id NotificationId1, NotificationMessage_Id NotificationId2)
        {

            if ((Object) NotificationId1 == null)
                throw new ArgumentNullException(nameof(NotificationId1), "The given NotificationId1 must not be null!");

            return NotificationId1.CompareTo(NotificationId2) < 0;

        }

        #endregion

        #region Operator <= (NotificationId1, NotificationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotificationId1">A notification identification.</param>
        /// <param name="NotificationId2">Another notification identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (NotificationMessage_Id NotificationId1, NotificationMessage_Id NotificationId2)
            => !(NotificationId1 > NotificationId2);

        #endregion

        #region Operator >  (NotificationId1, NotificationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotificationId1">A notification identification.</param>
        /// <param name="NotificationId2">Another notification identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (NotificationMessage_Id NotificationId1, NotificationMessage_Id NotificationId2)
        {

            if ((Object) NotificationId1 == null)
                throw new ArgumentNullException(nameof(NotificationId1), "The given NotificationId1 must not be null!");

            return NotificationId1.CompareTo(NotificationId2) > 0;

        }

        #endregion

        #region Operator >= (NotificationId1, NotificationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotificationId1">A notification identification.</param>
        /// <param name="NotificationId2">Another notification identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (NotificationMessage_Id NotificationId1, NotificationMessage_Id NotificationId2)
            => !(NotificationId1 < NotificationId2);

        #endregion

        #endregion

        #region IComparable<NotificationId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is NotificationMessage_Id NotificationId))
                throw new ArgumentException("The given object is not a notification identification!");

            return CompareTo(NotificationId);

        }

        #endregion

        #region CompareTo(NotificationId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotificationId">An object to compare with.</param>
        public Int32 CompareTo(NotificationMessage_Id NotificationId)
        {

            if ((Object) NotificationId == null)
                throw new ArgumentNullException(nameof(NotificationId),  "The given notification identification must not be null!");

            return String.Compare(InternalId, NotificationId.InternalId, StringComparison.Ordinal);

        }

        #endregion

        #endregion

        #region IEquatable<NotificationId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            if (!(Object is NotificationMessage_Id NotificationId))
                return false;

            return Equals(NotificationId);

        }

        #endregion

        #region Equals(NotificationId)

        /// <summary>
        /// Compares two notification identifications for equality.
        /// </summary>
        /// <param name="NotificationId">An notification identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(NotificationMessage_Id NotificationId)
        {

            if ((Object) NotificationId == null)
                return false;

            return InternalId.Equals(NotificationId.InternalId, StringComparison.OrdinalIgnoreCase);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => InternalId.ToLower().GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => InternalId;

        #endregion

    }

}
