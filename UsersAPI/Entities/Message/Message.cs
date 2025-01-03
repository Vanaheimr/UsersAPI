/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// A message.
    /// </summary>
    public class Message : AEntity<Message_Id,
                                   Message>
    {

        #region Data

        /// <summary>
        /// The default max size of the aggregated messages status history.
        /// </summary>
        public const UInt16 DefaultMessageStatusHistorySize = 50;

        /// <summary>
        /// The default JSON-LD context of messages.
        /// </summary>
        public new readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://opendata.social/contexts/UsersAPI/message");

        #endregion

        #region Properties

        #region API

        private Object _API;

        /// <summary>
        /// The API of this object.
        /// </summary>
        public Object API
        {

            get
            {
                return _API;
            }

            set
            {

                if (_API != null)
                    throw new ArgumentException("Illegal attempt to change the API!");

                _API = value ?? throw new ArgumentException("Illegal attempt to delete the API!");

            }

        }

        #endregion

        /// <summary>
        /// The sender of the message.
        /// </summary>
        public User_Id                    Sender            { get; }

        /// <summary>
        /// The receivers of the message.
        /// </summary>
        public IEnumerable<User_Id>       Receivers         { get; }

        /// <summary>
        /// The (multi-language) subject of the message.
        /// </summary>
        [Mandatory]
        public I18NString                 Subject           { get; }

        /// <summary>
        /// An optional (multi-language) text of the message.
        /// </summary>
        [Mandatory]
        public I18NString                 Text              { get; }

        /// <summary>
        /// The message is a reply to another message.
        /// </summary>
        public Message_Id?                InReplyTo         { get; }

        /// <summary>
        /// An enumeration of attached files.
        /// </summary>
        [Optional]
        public IEnumerable<AttachedFile>  AttachedFiles     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new message.
        /// </summary>
        /// <param name="Id">The unique identification of the message.</param>
        /// <param name="Sender">The sender of the message.</param>
        /// <param name="Receivers">The receivers of the message.</param>
        /// <param name="Subject">The (multi-language) subject of the message.</param>
        /// <param name="Text">An optional (multi-language) text of the message.</param>
        /// <param name="InReplyTo">The message is a reply to another message.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        public Message(Message_Id                  Id,
                       User_Id                     Sender,
                       IEnumerable<User_Id>        Receivers,
                       I18NString                  Subject,
                       I18NString                  Text,
                       Message_Id?                 InReplyTo       = null,

                       JObject?                    CustomData      = default,
                       IEnumerable<AttachedFile>?  AttachedFiles   = default,
                       JSONLDContext?              JSONLDContext   = default,
                       String?                     DataSource      = default,
                       DateTime?                   LastChange      = default)

            : base(Id,
                   JSONLDContext ?? DefaultJSONLDContext,
                   null,
                   null,
                   null,
                   CustomData,
                   null,
                   LastChange,
                   DataSource)

        {

            #region Initial checks

            if (!Receivers.Any())
                throw new ArgumentNullException(nameof(Receivers), "The enumeration of message receivers must not be null or empty!");

            #endregion

            this.Sender         = Sender;
            this.Receivers      = Receivers;
            this.Subject        = Subject       ?? new I18NString();
            this.Text           = Text          ?? new I18NString();
            this.InReplyTo      = InReplyTo;
            this.AttachedFiles  = AttachedFiles ?? new AttachedFile[0];

        }

        #endregion


        #region ToJSON(IncludeHash = true)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data structure is embedded into another data structure.</param>
        /// <param name="IncludeCryptoHash">Include the crypto hash value of this object.</param>
        public override JObject ToJSON(Boolean Embedded = false)

            => JSONObject.Create(
                   new JProperty("@id",         Id.     ToString()),
                   new JProperty("sender",      Sender. ToString()),
                   new JProperty("subject",     Subject.ToJSON()),
                   new JProperty("text",        Text.   ToJSON())
               );

        #endregion


        #region CopyAllLinkedDataFrom(OldMessage)

        public override void CopyAllLinkedDataFromBase(Message OldMessage)
        {

        }

        #endregion


        #region IComparable<Message> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public override Int32 CompareTo(Object Object)
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
        public override Int32 CompareTo(Message Message)
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
        public override Boolean Equals(Message Message)
        {

            if ((Object) Message == null)
                return false;

            return Id.Equals(Message.Id);

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

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => Id.ToString();

        #endregion


        #region ToBuilder(NewMessageId = null)

        /// <summary>
        /// Return a builder for this message.
        /// </summary>
        /// <param name="NewMessageId">An optional new message identification.</param>
        public Builder ToBuilder(Message_Id? NewMessageId = null)

            => new Builder(NewMessageId ?? Id,
                           Sender,
                           Receivers,
                           Subject,
                           Text,
                           InReplyTo,

                           CustomData,
                           AttachedFiles,
                           JSONLDContext,
                           DataSource,
                           LastChangeDate);

        #endregion

        #region (class) Builder

        /// <summary>
        /// An message builder.
        /// </summary>
        public new class Builder : AEntity<Message_Id,
                                           Message>.Builder
        {

            #region Properties

            /// <summary>
            /// The sender of the message.
            /// </summary>
            public User_Id                Sender            { get; set; }

            /// <summary>
            /// The receivers of the message.
            /// </summary>
            public HashSet<User_Id>       Receivers         { get; }

            /// <summary>
            /// The (multi-language) subject of the message.
            /// </summary>
            [Mandatory]
            public I18NString             Subject           { get; set; }

            /// <summary>
            /// An optional (multi-language) text of the message.
            /// </summary>
            [Mandatory]
            public I18NString             Text              { get; set; }

            /// <summary>
            /// The message is a reply to another message.
            /// </summary>
            public Message_Id?            InReplyTo         { get; set; }

            /// <summary>
            /// An enumeration of attached files.
            /// </summary>
            [Optional]
            public HashSet<AttachedFile>  AttachedFiles     { get; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new message builder.
            /// </summary>
            public Builder(Message_Id                  Id,
                           User_Id                     Sender,
                           IEnumerable<User_Id>        Receivers,
                           I18NString                  Subject,
                           I18NString                  Text,
                           Message_Id?                 InReplyTo       = null,

                           JObject?                    CustomData      = default,
                           IEnumerable<AttachedFile>?  AttachedFiles   = default,
                           JSONLDContext?              JSONLDContext   = default,
                           String?                     DataSource      = default,
                           DateTime?                   LastChange      = default)

                : base(Id,
                       JSONLDContext ?? DefaultJSONLDContext,
                       LastChange,
                       null,
                       CustomData,
                       null,
                       DataSource)

            {

                this.Sender         = Sender;
                this.Receivers      = AttachedFiles != null ?  new HashSet<User_Id>     (Receivers)     : new HashSet<User_Id>();
                this.Subject        = Subject               ?? new I18NString();
                this.Text           = Text                  ?? new I18NString();
                this.InReplyTo      = InReplyTo;
                this.AttachedFiles  = AttachedFiles != null ?  new HashSet<AttachedFile>(AttachedFiles) : new HashSet<AttachedFile>();

            }

            #endregion


            #region CopyAllLinkedDataFrom(OldMessage)

            public override void CopyAllLinkedDataFromBase(Message OldMessage)
            {

            }

            #endregion


            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the message.
            /// </summary>
            /// <param name="Builder">A message builder.</param>
            public static implicit operator Message(Builder Builder)

                => Builder?.ToImmutable;


            /// <summary>
            /// Return an immutable version of the message.
            /// </summary>
            public Message ToImmutable

                => new Message(Id,
                               Sender,
                               Receivers,
                               Subject,
                               Text,
                               InReplyTo,

                               CustomData,
                               AttachedFiles,
                               JSONLDContext,
                               DataSource,
                               LastChangeDate);

            #endregion


            #region Operator overloading

            #region Operator == (BuilderId1, BuilderId2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="BuilderId1">A message builder.</param>
            /// <param name="BuilderId2">Another message builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator == (Builder BuilderId1, Builder BuilderId2)
            {

                // If both are null, or both are same instance, return true.
                if (Object.ReferenceEquals(BuilderId1, BuilderId2))
                    return true;

                // If one is null, but not both, return false.
                if ((BuilderId1 is null) || (BuilderId2 is null))
                    return false;

                return BuilderId1.Equals(BuilderId2);

            }

            #endregion

            #region Operator != (BuilderId1, BuilderId2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="BuilderId1">A message builder.</param>
            /// <param name="BuilderId2">Another message builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator != (Builder BuilderId1, Builder BuilderId2)
                => !(BuilderId1 == BuilderId2);

            #endregion

            #region Operator <  (BuilderId1, BuilderId2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="BuilderId1">A message builder.</param>
            /// <param name="BuilderId2">Another message builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator < (Builder BuilderId1, Builder BuilderId2)
            {

                if (BuilderId1 is null)
                    throw new ArgumentNullException(nameof(BuilderId1), "The given BuilderId1 must not be null!");

                return BuilderId1.CompareTo(BuilderId2) < 0;

            }

            #endregion

            #region Operator <= (BuilderId1, BuilderId2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="BuilderId1">A message builder.</param>
            /// <param name="BuilderId2">Another message builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator <= (Builder BuilderId1, Builder BuilderId2)
                => !(BuilderId1 > BuilderId2);

            #endregion

            #region Operator >  (BuilderId1, BuilderId2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="BuilderId1">A message builder.</param>
            /// <param name="BuilderId2">Another message builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator > (Builder BuilderId1, Builder BuilderId2)
            {

                if (BuilderId1 is null)
                    throw new ArgumentNullException(nameof(BuilderId1), "The given BuilderId1 must not be null!");

                return BuilderId1.CompareTo(BuilderId2) > 0;

            }

            #endregion

            #region Operator >= (BuilderId1, BuilderId2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="BuilderId1">A message builder.</param>
            /// <param name="BuilderId2">Another message builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator >= (Builder BuilderId1, Builder BuilderId2)
                => !(BuilderId1 < BuilderId2);

            #endregion

            #endregion

            #region IComparable<Builder> Members

            #region CompareTo(Object)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Object">An object to compare with.</param>
            public override Int32 CompareTo(Object Object)

                => Object is Builder Builder
                       ? CompareTo(Builder)
                       : throw new ArgumentException("The given object is not an message!");


            #endregion

            #region CompareTo(Builder)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Builder">An message object to compare with.</param>
            public Int32 CompareTo(Builder Builder)

                => Builder is null
                       ? throw new ArgumentNullException(nameof(Builder), "The given message must not be null!")
                       : Id.CompareTo(Builder.Id);

            #endregion

            #endregion

            #region IEquatable<Builder> Members

            #region Equals(Object)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Object">An object to compare with.</param>
            /// <returns>true|false</returns>
            public override Boolean Equals(Object Object)

                => Object is Builder Builder &&
                      Equals(Builder);

            #endregion

            #region Equals(Builder)

            /// <summary>
            /// Compares two messages for equality.
            /// </summary>
            /// <param name="Builder">An message to compare with.</param>
            /// <returns>True if both match; False otherwise.</returns>
            public Boolean Equals(Builder Builder)

                => Builder is Builder &&
                       Id.Equals(Builder.Id);

            #endregion

            #endregion

            #region (override) GetHashCode()

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

            public override bool Equals(Message? other)
            {
                throw new NotImplementedException();
            }

            public override int CompareTo(Message? other)
            {
                throw new NotImplementedException();
            }

            #endregion

        }

        #endregion

    }

}
