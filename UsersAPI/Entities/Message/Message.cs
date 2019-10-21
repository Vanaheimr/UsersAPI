/*
 * Copyright (c) 2014-2019, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.Distributed;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// An Open Data message.
    /// </summary>
    public class Message : ADistributedEntity<Message_Id>,
                           IEquatable<Message>,
                           IComparable<Message>
    {

        #region Data

        /// <summary>
        /// The default max size of the aggregated user groups status history.
        /// </summary>
        public const UInt16 DefaultMessageStatusHistorySize = 50;

        #endregion

        #region Properties

        /// <summary>
        /// The sender of the message.
        /// </summary>
        public User_Id               Sender      { get; }

        /// <summary>
        /// The receivers of the message.
        /// </summary>
        public IEnumerable<User_Id>  Receivers   { get; }

        /// <summary>
        /// The (multi-language) subject of the message.
        /// </summary>
        [Mandatory]
        public I18NString            Subject     { get; }

        /// <summary>
        /// An optional (multi-language) text of the message.
        /// </summary>
        [Mandatory]
        public I18NString            Text        { get; }

        /// <summary>
        /// The message is a reply to another message.
        /// </summary>
        public Message_Id?           InReplyTo   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new user group.
        /// </summary>
        /// <param name="Id">The unique identification of the user group.</param>
        /// <param name="Sender">The sender of the message.</param>
        /// <param name="Receivers">The receivers of the message.</param>
        /// <param name="Subject">The (multi-language) subject of the message.</param>
        /// <param name="Text">An optional (multi-language) text of the message.</param>
        /// <param name="InReplyTo">The message is a reply to another message.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        public Message(Message_Id            Id,
                       User_Id               Sender,
                       IEnumerable<User_Id>  Receivers,
                       I18NString            Subject,
                       I18NString            Text,
                       Message_Id?           InReplyTo   = null,
                       String                DataSource  = "")

            : base(Id,
                   DataSource)

        {

            #region Initial checks

            if (Receivers == null || !Receivers.Any())
                throw new ArgumentNullException(nameof(Receivers), "The enumeration of message receivers must not be null or empty!");

            #endregion

            #region Init data and properties

            this.Sender     = Sender;
            this.Receivers  = Receivers;
            this.Subject    = Subject ?? new I18NString();
            this.Text       = Text    ?? new I18NString();
            this.InReplyTo  = InReplyTo;

            #endregion

        }

        #endregion


        #region ToJSON(IncludeHash = true)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        /// <param name="IncludeCryptoHash">Include the crypto hash value of this object.</param>
        public override JObject ToJSON(Boolean Embedded           = false,
                                       Boolean IncludeCryptoHash  = false)

            => JSONObject.Create(

                   new JProperty("@id",         Id.     ToString()),
                   new JProperty("sender",      Sender. ToString()),
                   new JProperty("subject",     Subject.ToJSON()),
                   new JProperty("text",        Text.   ToJSON()),

                   IncludeCryptoHash
                       ? new JProperty("cryptoHash", CurrentCryptoHash)
                       : null

               );

        #endregion


        #region IComparable<Message> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            var Message = Object as Message;
            if ((Object) Message == null)
                throw new ArgumentException("The given object is not an message!");

            return CompareTo(Message);

        }

        #endregion

        #region CompareTo(Message)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Message">An message object to compare with.</param>
        public Int32 CompareTo(Message Message)
        {

            if ((Object) Message == null)
                throw new ArgumentNullException(nameof(Message), "The given message must not be null!");

            return Id.CompareTo(Message.Id);

        }

        #endregion

        #endregion

        #region IEquatable<Message> Members

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

            var Message = Object as Message;
            if ((Object) Message == null)
                return false;

            return Equals(Message);

        }

        #endregion

        #region Equals(Message)

        /// <summary>
        /// Compares two messages for equality.
        /// </summary>
        /// <param name="Message">An message to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Message Message)
        {

            if ((Object) Message == null)
                return false;

            return Id.Equals(Message.Id);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => Id.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => Id.ToString();

        #endregion

    }

}
