﻿/*
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

using org.GraphDefined.Vanaheimr.Hermod.Distributed;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.OpenData.Users;

#endregion

namespace org.GraphDefined.OpenData.Notifications
{

    /// <summary>
    /// A notification message.
    /// </summary>
    public class NotificationMessage : ADistributedEntity<NotificationMessage_Id>,
                                       IEntityClass<NotificationMessage>
    {


        #region Data

        /// <summary>
        /// The JSON-LD context of the object.
        /// </summary>
        public const String JSONLDContext  = "https://opendata.social/contexts/UsersAPI+json/notificationMessage";

        #endregion

        #region Properties

        /// <summary>
        /// The timestamp of the notification message.
        /// </summary>
        [Mandatory]
        public DateTime                   Timestamp       { get; }

        /// <summary>
        /// The message type of the notification message.
        /// </summary>
        [Mandatory]
        public NotificationMessageType    Type            { get; }

        /// <summary>
        /// The data of the notification message.
        /// </summary>
        [Mandatory]
        public JObject                    Data            { get; }

        /// <summary>
        /// The owners of the notification message.
        /// </summary>
        [Mandatory]
        public IEnumerable<Organization>  Owners          { get; }

        /// <summary>
        /// The privacy level of the notification message.
        /// </summary>
        [Mandatory]
        public PrivacyLevel               PrivacyLevel    { get; }

        /// <summary>
        /// Optional cryptographic signatures of the notification message.
        /// </summary>
        [Mandatory]
        public IEnumerable<String>        Signatures      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new notification message.
        /// </summary>
        /// <param name="Id">The unique identification of the notification message.</param>
        /// <param name="Timestamp">The timestamp of the notification message.</param>
        /// <param name="Type">The message type of the notification message.</param>
        /// <param name="Data">The data of the notification message.</param>
        /// <param name="Owners">The owners of the notification message.</param>
        /// <param name="PrivacyLevel">The privacy level of the notification message.</param>
        /// <param name="Signatures">Optional cryptographic signatures of the notification message.</param>
        public NotificationMessage(DateTime                   Timestamp,
                                   NotificationMessageType    Type,
                                   JObject                    Data,
                                   IEnumerable<Organization>  Owners,
                                   PrivacyLevel?              PrivacyLevel  = null,
                                   IEnumerable<String>        Signatures    = null)

            : this(NotificationMessage_Id.Random(),
                   Timestamp,
                   Type,
                   Data,
                   Owners,
                   PrivacyLevel,
                   Signatures)

        { }


        /// <summary>
        /// Create a new notification message.
        /// </summary>
        /// <param name="Id">The unique identification of the notification message.</param>
        /// <param name="Timestamp">The timestamp of the notification message.</param>
        /// <param name="Type">The message type of the notification message.</param>
        /// <param name="Data">The data of the notification message.</param>
        /// <param name="Owners">The owners of the notification message.</param>
        /// <param name="PrivacyLevel">The privacy level of the notification message.</param>
        /// <param name="Signatures">Optional cryptographic signatures of the notification message.</param>
        public NotificationMessage(NotificationMessage_Id     Id,
                                   DateTime                   Timestamp,
                                   NotificationMessageType    Type,
                                   JObject                    Data,
                                   IEnumerable<Organization>  Owners,
                                   PrivacyLevel?              PrivacyLevel  = null,
                                   IEnumerable<String>        Signatures    = null)

            : base(Id,
                   "")

        {

            this.Timestamp     = Timestamp;
            this.Type          = Type;
            this.Data          = Data;
            this.Owners        = Owners == null ? new Organization[0] : Owners.Where(owner => owner != null);
            this.PrivacyLevel  = PrivacyLevel ?? OpenData.PrivacyLevel.Private;
            this.Signatures    = Signatures   ?? new String[0];

            CalcHash();

        }

        #endregion


        #region ToJSON(IncludeCryptoHash = true)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeCryptoHash">Include the hash value of this object.</param>
        public override JObject ToJSON(Boolean Embedded           = true,
                                       Boolean IncludeCryptoHash  = true)

            => JSONObject.Create(

                   new JProperty("@id",        Id.ToString()),

                   !Embedded
                       ? new JProperty("@context",   JSONLDContext)
                       : null,

                   new JProperty("timestamp",  Timestamp.ToIso8601()),
                   new JProperty("type",       Type.ToString()),
                   new JProperty("data",       Data),
                   new JProperty("ownerIds",   new JArray(Owners.Select(owner => owner.Id.ToString()))),

                   Signatures.SafeAny()
                       ? new JProperty("signatures",  new JArray(Signatures))
                       : null,

                   IncludeCryptoHash
                       ? new JProperty("hash",        CurrentCryptoHash)
                       : null

               );

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out NotificationMessage, out ErrorResponse)

        public static Boolean TryParseJSON(JObject                              JSONObject,
                                           Func<Organization_Id, Organization>  OrganizationProvider,
                                           out NotificationMessage              NotificationMessage,
                                           out String                           ErrorResponse,
                                           NotificationMessage_Id?              NotificationMessageIdURI = null)
        {

            if (OrganizationProvider == null)
            {
                NotificationMessage = null;
                ErrorResponse       = "The given owner/organization provider must not be null!";
                return false;
            }

            try
            {

                NotificationMessage = null;

                #region Parse NotificationMessageId   [optional]

                // Verify that a given NotificationMessage identification
                //   is at least valid.
                if (!JSONObject.ParseOptionalN("@id",
                                               "NotificationMessage identification",
                                               NotificationMessage_Id.TryParse,
                                               out NotificationMessage_Id? NotificationMessageIdBody,
                                               out ErrorResponse))
                {
                    return false;
                }

                if (!NotificationMessageIdURI.HasValue && !NotificationMessageIdBody.HasValue)
                {
                    ErrorResponse = "The NotificationMessage identification is missing!";
                    return false;
                }

                if (NotificationMessageIdURI.HasValue && NotificationMessageIdBody.HasValue && NotificationMessageIdURI.Value != NotificationMessageIdBody.Value)
                {
                    ErrorResponse = "The optional NotificationMessage identification given within the JSON body does not match the one given in the URI!";
                    return false;
                }

                #endregion

                #region Parse Context                 [mandatory]

                if (!JSONObject.ParseMandatory("@context",
                                               "JSON-LinkedData context information",
                                               out String Context,
                                               out ErrorResponse))
                {
                    ErrorResponse = @"The JSON-LD ""@context"" information is missing!";
                    return false;
                }

                if (Context != JSONLDContext)
                {
                    ErrorResponse = @"The given JSON-LD ""@context"" information '" + Context + "' is not supported!";
                    return false;
                }

                #endregion

                #region Parse Timestamp               [mandatory]

                if (!JSONObject.ParseMandatory("timestamp",
                                               "timestamp",
                                               out DateTime Timestamp,
                                               out ErrorResponse))
                {
                     return false;
                }

                #endregion

                #region Parse Type                    [mandatory]

                if (!JSONObject.ParseMandatory("type",
                                               "notification message type",
                                               NotificationMessageType.Parse,
                                               out NotificationMessageType Type,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse PrivacyLevel            [mandatory]

                if (!JSONObject.ParseMandatory("privacyLevel",
                                               "privacy level",
                                               out PrivacyLevel PrivacyLevel,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Data                    [mandatory]

                if (!JSONObject.ParseMandatory("name",
                                               "NotificationMessagename",
                                               out JObject Data,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Owners                  [mandatory]

                if (!JSONObject.ParseMandatory("ownerIds",
                                               "owner identifications",
                                               out JArray OwnersJSON,
                                               out ErrorResponse))
                {
                    return false;
                }

                var Owners = new List<Organization>();

                foreach (var ownerJSON in OwnersJSON)
                {

                    if (!Organization_Id.TryParse(ownerJSON.Value<String>(), out Organization_Id OwnerId))
                    {
                        ErrorResponse = "Invalid owner identification '" + OwnerId + "'!";
                        return false;
                    }

                    var Owner = OrganizationProvider(OwnerId);
                    if (Owner == null)
                    {
                        ErrorResponse = "Unknown owner '" + OwnerId + "'!";
                        return false;
                    }

                    Owners.Add(Owner);

                }

                if (Owners.Count == 0)
                {
                    ErrorResponse = "Invalid owner identifications!";
                    return false;
                }

                #endregion

                #region Parse Signatures              [optional]

                if (!JSONObject.ParseOptional("signatures",
                                              "Signatures",
                                              out JArray Signatures,
                                              out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse CryptoHash              [optional]

                var CryptoHash    = JSONObject.GetOptional("cryptoHash");

                #endregion


                NotificationMessage = new NotificationMessage(NotificationMessageIdBody ?? NotificationMessageIdURI.Value,
                                                              Timestamp,
                                                              Type,
                                                              Data,
                                                              Owners,
                                                              PrivacyLevel,
                                                              Signatures.Select(jtoken => jtoken.Value<String>()));

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse  = e.Message;
                NotificationMessage  = null;
                return false;
            }

        }

        #endregion


        #region Operator overloading

        #region Operator == (NotificationMessageId1, NotificationMessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotificationMessageId1">A NotificationMessage identification.</param>
        /// <param name="NotificationMessageId2">Another NotificationMessage identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (NotificationMessage NotificationMessageId1, NotificationMessage NotificationMessageId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(NotificationMessageId1, NotificationMessageId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) NotificationMessageId1 == null) || ((Object) NotificationMessageId2 == null))
                return false;

            return NotificationMessageId1.Equals(NotificationMessageId2);

        }

        #endregion

        #region Operator != (NotificationMessageId1, NotificationMessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotificationMessageId1">A NotificationMessage identification.</param>
        /// <param name="NotificationMessageId2">Another NotificationMessage identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (NotificationMessage NotificationMessageId1, NotificationMessage NotificationMessageId2)
            => !(NotificationMessageId1 == NotificationMessageId2);

        #endregion

        #region Operator <  (NotificationMessageId1, NotificationMessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotificationMessageId1">A NotificationMessage identification.</param>
        /// <param name="NotificationMessageId2">Another NotificationMessage identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (NotificationMessage NotificationMessageId1, NotificationMessage NotificationMessageId2)
        {

            if ((Object) NotificationMessageId1 == null)
                throw new ArgumentNullException(nameof(NotificationMessageId1), "The given NotificationMessageId1 must not be null!");

            return NotificationMessageId1.CompareTo(NotificationMessageId2) < 0;

        }

        #endregion

        #region Operator <= (NotificationMessageId1, NotificationMessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotificationMessageId1">A NotificationMessage identification.</param>
        /// <param name="NotificationMessageId2">Another NotificationMessage identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (NotificationMessage NotificationMessageId1, NotificationMessage NotificationMessageId2)
            => !(NotificationMessageId1 > NotificationMessageId2);

        #endregion

        #region Operator >  (NotificationMessageId1, NotificationMessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotificationMessageId1">A NotificationMessage identification.</param>
        /// <param name="NotificationMessageId2">Another NotificationMessage identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (NotificationMessage NotificationMessageId1, NotificationMessage NotificationMessageId2)
        {

            if ((Object) NotificationMessageId1 == null)
                throw new ArgumentNullException(nameof(NotificationMessageId1), "The given NotificationMessageId1 must not be null!");

            return NotificationMessageId1.CompareTo(NotificationMessageId2) > 0;

        }

        #endregion

        #region Operator >= (NotificationMessageId1, NotificationMessageId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotificationMessageId1">A NotificationMessage identification.</param>
        /// <param name="NotificationMessageId2">Another NotificationMessage identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (NotificationMessage NotificationMessageId1, NotificationMessage NotificationMessageId2)
            => !(NotificationMessageId1 < NotificationMessageId2);

        #endregion

        #endregion

        #region IComparable<NotificationMessage> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            var NotificationMessage = Object as NotificationMessage;
            if ((Object) NotificationMessage == null)
                throw new ArgumentException("The given object is not an NotificationMessage!");

            return CompareTo(NotificationMessage);

        }

        #endregion

        #region CompareTo(NotificationMessage)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NotificationMessage">An NotificationMessage object to compare with.</param>
        public Int32 CompareTo(NotificationMessage NotificationMessage)
        {

            if ((Object) NotificationMessage == null)
                throw new ArgumentNullException("The given NotificationMessage must not be null!");

            return Id.CompareTo(NotificationMessage.Id);

        }

        #endregion

        #endregion

        #region IEquatable<NotificationMessage> Members

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

            if (!(Object is NotificationMessage NotificationMessage))
                return false;

            return Equals(NotificationMessage);

        }

        #endregion

        #region Equals(NotificationMessage)

        /// <summary>
        /// Compares two NotificationMessages for equality.
        /// </summary>
        /// <param name="NotificationMessage">An NotificationMessage to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(NotificationMessage NotificationMessage)
        {

            if ((Object) NotificationMessage == null)
                return false;

            return Id.Equals(NotificationMessage.Id);

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