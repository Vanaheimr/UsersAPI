/*
 * Copyright (c) 2014-2017, Achim 'ahzf' Friedland <achim@graphdefined.org>
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

namespace org.GraphDefined.OpenData.Users
{

    /// <summary>
    /// The unique identification of a message.
    /// </summary>
    public struct Message_Id : IId,
                               IEquatable<Message_Id>,
                               IComparable<Message_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        //ToDo: Replace with better randomness!
        private static readonly Random _Random = new Random(DateTime.UtcNow.Millisecond);

        #endregion

        #region Properties

        /// <summary>
        /// The length of the message identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new message identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the message identification.</param>
        private Message_Id(String String)
        {
            InternalId = String;
        }

        #endregion


        #region (static) New

        /// <summary>
        /// Create a new random message identification.
        /// </summary>
        public static Message_Id New
            => Parse(Guid.NewGuid().ToString().Replace("-", ""));

        #endregion

        #region (static) Parse(Text)

        /// <summary>
        /// Parse the given string as a message identification.
        /// </summary>
        /// <param name="Text">A text representation of a message identification.</param>
        public static Message_Id Parse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a message identification must not be null or empty!");

            #endregion

            return new Message_Id(Text);

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a message identification.
        /// </summary>
        /// <param name="Text">A text representation of a message identification.</param>
        public static Message_Id? TryParse(String Text)
        {

            Message_Id _MessageId;

            if (TryParse(Text, out _MessageId))
                return _MessageId;

            return new Message_Id?();

        }

        #endregion

        #region (static) TryParse(Text, out MessageId)

        /// <summary>
        /// Try to parse the given string as a message identification.
        /// </summary>
        /// <param name="Text">A text representation of a message identification.</param>
        /// <param name="MessageId">The parsed message identification.</param>
        public static Boolean TryParse(String Text, out Message_Id MessageId)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a message identification must not be null or empty!");

            #endregion

            try
            {
                MessageId = new Message_Id(Text);
                return true;
            }
            catch (Exception)
            {
                MessageId = default(Message_Id);
                return false;
            }

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone a message identification.
        /// </summary>

        public Message_Id Clone
            => new Message_Id(new String(InternalId.ToCharArray()));

        #endregion


        #region Operator overloading

        #region Operator == (MessageId1, MessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageId1">A message identification.</param>
        /// <param name="MessageId2">Another message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Message_Id MessageId1, Message_Id MessageId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(MessageId1, MessageId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) MessageId1 == null) || ((Object) MessageId2 == null))
                return false;

            return MessageId1.Equals(MessageId2);

        }

        #endregion

        #region Operator != (MessageId1, MessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageId1">A message identification.</param>
        /// <param name="MessageId2">Another message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Message_Id MessageId1, Message_Id MessageId2)
            => !(MessageId1 == MessageId2);

        #endregion

        #region Operator <  (MessageId1, MessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageId1">A message identification.</param>
        /// <param name="MessageId2">Another message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Message_Id MessageId1, Message_Id MessageId2)
        {

            if ((Object) MessageId1 == null)
                throw new ArgumentNullException(nameof(MessageId1), "The given MessageId1 must not be null!");

            return MessageId1.CompareTo(MessageId2) < 0;

        }

        #endregion

        #region Operator <= (MessageId1, MessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageId1">A message identification.</param>
        /// <param name="MessageId2">Another message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Message_Id MessageId1, Message_Id MessageId2)
            => !(MessageId1 > MessageId2);

        #endregion

        #region Operator >  (MessageId1, MessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageId1">A message identification.</param>
        /// <param name="MessageId2">Another message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Message_Id MessageId1, Message_Id MessageId2)
        {

            if ((Object) MessageId1 == null)
                throw new ArgumentNullException(nameof(MessageId1), "The given MessageId1 must not be null!");

            return MessageId1.CompareTo(MessageId2) > 0;

        }

        #endregion

        #region Operator >= (MessageId1, MessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageId1">A message identification.</param>
        /// <param name="MessageId2">Another message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Message_Id MessageId1, Message_Id MessageId2)
            => !(MessageId1 < MessageId2);

        #endregion

        #endregion

        #region IComparable<MessageId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is Message_Id))
                throw new ArgumentException("The given object is not a message identification!",
                                            nameof(Object));

            return CompareTo((Message_Id) Object);

        }

        #endregion

        #region CompareTo(MessageId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageId">An object to compare with.</param>
        public Int32 CompareTo(Message_Id MessageId)
        {

            if ((Object) MessageId == null)
                throw new ArgumentNullException(nameof(MessageId),  "The given message identification must not be null!");

            return String.Compare(InternalId, MessageId.InternalId, StringComparison.Ordinal);

        }

        #endregion

        #endregion

        #region IEquatable<MessageId> Members

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

            if (!(Object is Message_Id))
                return false;

            return Equals((Message_Id) Object);

        }

        #endregion

        #region Equals(MessageId)

        /// <summary>
        /// Compares two message identifications for equality.
        /// </summary>
        /// <param name="MessageId">An message identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Message_Id MessageId)
        {

            if ((Object) MessageId == null)
                return false;

            return InternalId.Equals(MessageId.InternalId);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => InternalId.GetHashCode();

        #endregion

        #region ToString()

        /// <summary>
        /// Return a string represtentation of this object.
        /// </summary>
        public override String ToString()
            => InternalId;

        #endregion

    }

}
