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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace social.OpenData.UsersAPI
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

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the message identificator.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId?.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new message identification based on the given string.
        /// </summary>
        private Message_Id(String Text)
        {
            InternalId = Text;
        }

        #endregion


        #region Parse   (Text)

        /// <summary>
        /// Parse the given string as a message identification.
        /// </summary>
        /// <param name="Text">A text representation of a message identification.</param>
        public static Message_Id Parse(String Text)
        {

            if (TryParse(Text, out Message_Id messageId))
                return messageId;

            throw new ArgumentException($"Invalid text representation of a message identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a message identification.
        /// </summary>
        /// <param name="Text">A text representation of a message identification.</param>
        public static Message_Id? TryParse(String Text)
        {

            if (TryParse(Text, out Message_Id messageId))
                return messageId;

            return null;

        }

        #endregion

        #region TryParse(Text, out MessageId)

        /// <summary>
        /// Try to parse the given string as a message identification.
        /// </summary>
        /// <param name="Text">A text representation of a message identification.</param>
        /// <param name="MessageId">The parsed message identification.</param>
        public static Boolean TryParse(String Text, out Message_Id MessageId)
        {

            Text = Text?.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    MessageId = new Message_Id(Text);
                    return true;
                }
                catch
                { }
            }

            MessageId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this message identification.
        /// </summary>
        public Message_Id Clone

            => new Message_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (MessageIdId1, MessageIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageIdId1">A message identification.</param>
        /// <param name="MessageIdId2">Another message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Message_Id MessageIdId1,
                                           Message_Id MessageIdId2)

            => MessageIdId1.Equals(MessageIdId2);

        #endregion

        #region Operator != (MessageIdId1, MessageIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageIdId1">A message identification.</param>
        /// <param name="MessageIdId2">Another message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Message_Id MessageIdId1,
                                           Message_Id MessageIdId2)

            => !MessageIdId1.Equals(MessageIdId2);

        #endregion

        #region Operator <  (MessageIdId1, MessageIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageIdId1">A message identification.</param>
        /// <param name="MessageIdId2">Another message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Message_Id MessageIdId1,
                                          Message_Id MessageIdId2)

            => MessageIdId1.CompareTo(MessageIdId2) < 0;

        #endregion

        #region Operator <= (MessageIdId1, MessageIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageIdId1">A message identification.</param>
        /// <param name="MessageIdId2">Another message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Message_Id MessageIdId1,
                                           Message_Id MessageIdId2)

            => MessageIdId1.CompareTo(MessageIdId2) <= 0;

        #endregion

        #region Operator >  (MessageIdId1, MessageIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageIdId1">A message identification.</param>
        /// <param name="MessageIdId2">Another message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Message_Id MessageIdId1,
                                          Message_Id MessageIdId2)

            => MessageIdId1.CompareTo(MessageIdId2) > 0;

        #endregion

        #region Operator >= (MessageIdId1, MessageIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageIdId1">A message identification.</param>
        /// <param name="MessageIdId2">Another message identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Message_Id MessageIdId1,
                                           Message_Id MessageIdId2)

            => MessageIdId1.CompareTo(MessageIdId2) >= 0;

        #endregion

        #endregion

        #region IComparable<MessageId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Message_Id messageId
                   ? CompareTo(messageId)
                   : throw new ArgumentException("The given object is not a message identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(MessageId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MessageId">An object to compare with.</param>
        public Int32 CompareTo(Message_Id MessageId)

            => String.Compare(InternalId,
                              MessageId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

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

            => Object is Message_Id messageId &&
                   Equals(messageId);

        #endregion

        #region Equals(MessageId)

        /// <summary>
        /// Compares two MessageIds for equality.
        /// </summary>
        /// <param name="MessageId">A message identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Message_Id MessageId)

            => String.Equals(InternalId,
                             MessageId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
