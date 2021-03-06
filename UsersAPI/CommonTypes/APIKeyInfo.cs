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
using System.Text;
using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// An API key information.
    /// </summary>
    public class APIKeyInfo : IEquatable<APIKeyInfo>,
                              IComparable<APIKeyInfo>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public const String JSONLDContext  = "https://opendata.social/contexts/UsersAPI/APIKeyInfo";

        #endregion

        #region Properties

        /// <summary>
        /// The API key.
        /// </summary>
        public APIKey        APIKey              { get; }

        /// <summary>
        /// The related user.
        /// </summary>
        public User          User                { get; }

        /// <summary>
        /// An internationalized description of the API key.
        /// </summary>
        public I18NString    Description         { get; }

        /// <summary>
        /// The access rights of the API key.
        /// </summary>
        public APIKeyRights  AccessRights        { get; }

        /// <summary>
        /// The creation timestamp.
        /// </summary>
        public DateTime      Created             { get; }

        /// <summary>
        /// The API key is not valid before this optional timestamp.
        /// </summary>
        public DateTime?     NotBefore           { get; }

        /// <summary>
        /// The API key is not valid after this optional timestamp.
        /// </summary>
        public DateTime?     NotAfter            { get; }

        /// <summary>
        /// The API key is currently disabled.
        /// </summary>
        public Boolean       IsDisabled          { get; }


        /// <summary>
        /// The hash value of this object.
        /// </summary>
        public String        CurrentCryptoHash   { get; protected set; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new API key information.
        /// </summary>
        /// <param name="APIKey">The API key.</param>
        /// <param name="User">The related user.</param>
        /// <param name="Description">An optional internationalized description of the API key.</param>
        /// <param name="AccessRights">The access rights of the API key.</param>
        /// <param name="Created">The creation timestamp.</param>
        /// <param name="NotBefore">The API key is not valid before this optional timestamp.</param>
        /// <param name="NotAfter">The API key is not valid after this optional timestamp.</param>
        /// <param name="IsDisabled">The API key is currently disabled.</param>
        public APIKeyInfo(APIKey        APIKey,
                          User          User,
                          I18NString    Description    = null,
                          APIKeyRights  AccessRights   = APIKeyRights.ReadOnly,
                          DateTime?     Created        = null,
                          DateTime?     NotBefore      = null,
                          DateTime?     NotAfter       = null,
                          Boolean?      IsDisabled     = false)
        {

            this.APIKey        = APIKey;
            this.User          = User        ?? throw new ArgumentNullException(nameof(User), "The given user must not be null!");
            this.Description   = Description ?? I18NString.Empty;
            this.AccessRights  = AccessRights;
            this.Created       = Created     ?? DateTime.UtcNow;
            this.NotBefore     = NotBefore;
            this.NotAfter      = NotAfter;
            this.IsDisabled    = IsDisabled  ?? false;

        }

        #endregion


        #region ToJSON(IncludeCryptoHash)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeCryptoHash">Whether to include the cryptograhical hash value of this object.</param>
        public JObject ToJSON(Boolean IncludeCryptoHash)

            => JSONObject.Create(

                   new JProperty("@id",                 APIKey.        ToString()),
                   new JProperty("@context",            JSONLDContext. ToString()),

                   new JProperty("userId",              User.Id.       ToString()),
                   new JProperty("description",         Description.   ToJSON()),
                   new JProperty("accessRights",        AccessRights.  AsText()),
                   new JProperty("created",             Created.       ToIso8601()),

                   NotBefore.HasValue
                       ? new JProperty("notBefore",     NotAfter.Value.ToIso8601())
                       : null,

                   NotAfter.HasValue
                       ? new JProperty("notAfter",      NotAfter.Value.ToIso8601())
                       : null,

                   IsDisabled
                       ? new JProperty("isDisabled",    IsDisabled)
                       : null

               );

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out APIKeyInfo, out ErrorResponse)

        public static Boolean TryParseJSON(JObject               JSONObject,
                                           out APIKeyInfo        APIKeyInfo,
                                           UserProviderDelegate  UserProvider,
                                           out String            ErrorResponse,
                                           APIKey?               APIKeyURI = null)
        {

            try
            {

                APIKeyInfo = null;

                #region Parse APIKey           [optional]

                // Verify that a given API key is at least valid.
                if (!JSONObject.ParseOptionalStruct("@id",
                                                    "API key",
                                                    APIKey.TryParse,
                                                    out APIKey? APIKeyBody,
                                                    out ErrorResponse))
                {
                    return false;
                }

                if (!APIKeyURI.HasValue && !APIKeyBody.HasValue)
                {
                    ErrorResponse = "The API key is missing!";
                    return false;
                }

                if (APIKeyURI.HasValue && APIKeyBody.HasValue && APIKeyURI.Value != APIKeyBody.Value)
                {
                    ErrorResponse = "The optional API key given within the JSON body does not match the one given in the URI!";
                    return false;
                }

                #endregion

                #region Parse Context          [mandatory]

                if (!JSONObject.ParseMandatoryText("@context",
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

                #region Parse UserId           [mandatory]

                // Verify that a given user identification
                //   is at least valid.
                if (!JSONObject.ParseMandatory("userId",
                                               "user identification",
                                               User_Id.TryParse,
                                               out User_Id UserId,
                                               out ErrorResponse))
                {
                    return false;
                }

                if (!UserProvider(UserId, out User User))
                {
                    ErrorResponse = "The given user '" + UserId + "' is unknown!";
                    return false;
                }

                #endregion

                #region Parse Description      [optional]

                if (JSONObject.ParseOptional("description",
                                             "description",
                                             out I18NString Description,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse AccessRights     [mandatory]

                if (!JSONObject.ParseMandatoryEnum("accessRights",
                                                   "Access Rights",
                                                  // s => s.ParseMandatory_APIKeyRights(),
                                                   out APIKeyRights AccessRights,
                                                   out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Created          [mandatory]

                if (!JSONObject.ParseMandatory("created",
                                               "creation timestamp",
                                               out DateTime Created,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse NotBefore        [optional]

                if (JSONObject.ParseOptional("NotBefore",
                                             "'not-valid-before'-timestamp",
                                             out DateTime? NotBefore,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse NotAfter         [optional]

                if (JSONObject.ParseOptional("NotAfter",
                                             "'not-valid-after'-timestamp",
                                             out DateTime? NotAfter,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                var IsDisabled       = JSONObject["isDisabled"]?.     Value<Boolean>();

                #region Parse CryptoHash       [optional]

                var CryptoHash       = JSONObject.GetOptional("cryptoHash");

                #endregion


                APIKeyInfo = new APIKeyInfo(APIKeyBody ?? APIKeyURI.Value,
                                            User,
                                            Description,
                                            AccessRights,
                                            Created,
                                            NotBefore,
                                            NotAfter,
                                            IsDisabled);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse  = e.Message;
                APIKeyInfo     = null;
                return false;
            }

        }

        #endregion


        #region CalcHash()

        /// <summary>
        /// Calculate the hash value of this object.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "<Pending>")]
        public void CalcHash()
        {

            using (var SHA256 = new SHA256Managed())
            {

                CurrentCryptoHash = "json:sha256:" +
                                        SHA256.ComputeHash(Encoding.Unicode.GetBytes(
                                                               ToJSON  (IncludeCryptoHash: false).
                                                               ToString(Newtonsoft.Json.Formatting.None)
                                                           )).
                                               Select(value => String.Format("{0:x2}", value)).
                                               Aggregate();

            }

        }

        #endregion


        #region Operator overloading

        #region Operator == (APIKeyInfo1, APIKeyInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="APIKeyInfo1">An API key.</param>
        /// <param name="APIKeyInfo2">Another API key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (APIKeyInfo APIKeyInfo1, APIKeyInfo APIKeyInfo2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(APIKeyInfo1, APIKeyInfo2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) APIKeyInfo1 == null) || ((Object) APIKeyInfo2 == null))
                return false;

            return APIKeyInfo1.Equals(APIKeyInfo2);

        }

        #endregion

        #region Operator != (APIKeyInfo1, APIKeyInfo2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="APIKeyInfo1">An API key.</param>
        /// <param name="APIKeyInfo2">Another API key.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (APIKeyInfo APIKeyInfo1, APIKeyInfo APIKeyInfo2)
            => !(APIKeyInfo1 == APIKeyInfo2);

        #endregion

        #endregion

        #region IComparable<APIKeyInfo> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is APIKeyInfo))
                throw new ArgumentException("The given object is not an API key info!",
                                            nameof(Object));

            return CompareTo((APIKeyInfo) Object);

        }

        #endregion

        #region CompareTo(APIKeyInfo)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="APIKeyInfo">An object to compare with.</param>
        public Int32 CompareTo(APIKeyInfo APIKeyInfo)
        {

            if ((Object) APIKeyInfo == null)
                throw new ArgumentNullException(nameof(APIKeyInfo),  "The given API key info must not be null!");

            return APIKey.CompareTo(APIKeyInfo.APIKey);

        }

        #endregion

        #endregion

        #region IEquatable<APIKeyInfo> Members

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

            if (!(Object is APIKeyInfo))
                return false;

            return Equals((APIKeyInfo) Object);

        }

        #endregion

        #region Equals(APIKeyInfo)

        /// <summary>
        /// Compares two API keys informations for equality.
        /// </summary>
        /// <param name="APIKeyInfo">An API key information to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(APIKeyInfo APIKeyInfo)
        {

            if ((Object) APIKeyInfo == null)
                return false;

            return APIKey.Equals(APIKeyInfo.APIKey);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => APIKey.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat("'", APIKey, "' for ",
                             User.Id.ToString(), ", [",
                             AccessRights.ToString(),
                             NotAfter != null ? ", expires at " + NotAfter.Value.ToIso8601() : "",
                             IsDisabled ? ", disabled]" : "]");

        #endregion

    }

}
