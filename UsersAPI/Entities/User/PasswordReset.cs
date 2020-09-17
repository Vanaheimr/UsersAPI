/*
 * Copyright (c) 2014-2020, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// Information for resetting user passwords.
    /// </summary>
    public class PasswordReset
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public const String JSONLDContext  = "https://opendata.social/contexts/UsersAPI+json/passwordReset";

        #endregion

        #region Properties

        /// <summary>
        /// The creation timestamp of the password-reset-information.
        /// </summary>
        public DateTime              Timestamp        { get; }

        /// <summary>
        /// An enumeration of valid user identifications.
        /// </summary>
        public IEnumerable<User_Id>  UserIds          { get; }

        /// <summary>
        /// A security token to authorize the password reset.
        /// </summary>
        public SecurityToken_Id      SecurityToken1   { get; }

        /// <summary>
        /// An optional second security token to authorize the password reset.
        /// </summary>
        public SecurityToken_Id?     SecurityToken2   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new information object for resetting user passwords.
        /// </summary>
        /// <param name="UserIds">An enumeration of valid user identifications.</param>
        /// <param name="SecurityToken1">A security token to authorize the password reset.</param>
        /// <param name="SecurityToken2">An optional second security token to authorize the password reset.</param>
        public PasswordReset(IEnumerable<User_Id>  UserIds,
                             SecurityToken_Id      SecurityToken1,
                             SecurityToken_Id?     SecurityToken2)

            : this(DateTime.UtcNow,
                   UserIds,
                   SecurityToken1,
                   SecurityToken2)

        { }


        /// <summary>
        /// Create a new information object for resetting user passwords.
        /// </summary>
        /// <param name="Timestamp">The creation timestamp of the password-reset-information.</param>
        /// <param name="UserIds">An enumeration of valid user identifications.</param>
        /// <param name="SecurityToken1">A security token to authorize the password reset.</param>
        /// <param name="SecurityToken2">An optional second security token to authorize the password reset.</param>
        public PasswordReset(DateTime              Timestamp,
                             IEnumerable<User_Id>  UserIds,
                             SecurityToken_Id      SecurityToken1,
                             SecurityToken_Id?     SecurityToken2)
        {

            if (UserIds == null || !UserIds.Any())
                throw new ArgumentNullException(nameof(UserIds), "The given enumeration of user identifications must not be null or empty!");

            this.Timestamp       = Timestamp;
            this.UserIds         = UserIds;
            this.SecurityToken1  = SecurityToken1;
            this.SecurityToken2  = SecurityToken2;

        }

        #endregion


        #region ToJSON(Embedded = true)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        public JObject ToJSON(Boolean Embedded = true)

            => JSONObject.Create(

                   Embedded
                       ? null
                       : new JProperty("@context",  JSONLDContext.ToString()),

                   new JProperty("timestamp",       Timestamp.ToIso8601()),
                   new JProperty("userIds",         new JArray(UserIds.Select(user => user.ToString()))),
                   new JProperty("securityToken1",  SecurityToken1.ToString()),

                   SecurityToken2.HasValue
                       ? new JProperty("securityToken2",  SecurityToken2.ToString())
                       : null

               );

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out Communicator, out ErrorResponse, IgnoreContextMismatches = true)

        public static Boolean TryParseJSON(JObject              JSONObject,
                                           out PasswordReset    PasswordReset,
                                           out String           ErrorResponse,
                                           Boolean              IgnoreContextMismatches = true)

        {

            try
            {

                PasswordReset = null;

                if (JSONObject == null)
                {
                    ErrorResponse = "The given JSON object must not be null!";
                    return false;
                }

                #region Parse Context          [mandatory]

                if (!IgnoreContextMismatches)
                {

                    if (!JSONObject.ParseMandatoryText("@context",
                                                       "JSON-LD context",
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

                }

                #endregion

                #region Parse Timestamp        [mandatory]

                if (!JSONObject.ParseMandatory("timestamp",
                                               "timestamp",
                                               out DateTime Timestamp,
                                               out          ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse UserIds          [mandatory]

                if (!JSONObject.ParseMandatory("userIds",
                                               "user identifications",
                                               out JArray UserIdArray,
                                               out ErrorResponse))
                {
                    return false;
                }

                var UserIds = new User_Id[0];

                try
                {
                    UserIds = UserIdArray.Select(jsonvalue => User_Id.Parse(jsonvalue.Value<String>())).ToArray();
                }
                catch (Exception e)
                {
                    ErrorResponse = "The given array of users '" + UserIdArray + "' is invalid!";
                    return false;
                }

                if (UserIds.Length == 0)
                {
                    ErrorResponse = "The given array of users '" + UserIdArray + "' is invalid!";
                    return false;
                }

                #endregion

                #region Parse SecurityToken1   [mandatory]

                if (!JSONObject.ParseMandatory("securityToken1",
                                               "security token #1",
                                               SecurityToken_Id.TryParse,
                                               out SecurityToken_Id SecurityToken1,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse SecurityToken2   [optional]

                if (JSONObject.ParseOptionalStruct("securityToken2",
                                                   "security token #2",
                                                   SecurityToken_Id.TryParse,
                                                   out SecurityToken_Id? SecurityToken2,
                                                   out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                PasswordReset = new PasswordReset(Timestamp,
                                                  UserIds,
                                                  SecurityToken1,
                                                  SecurityToken2);

                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = e.Message;
                PasswordReset = null;
                return false;
            }

        }

        #endregion


    }

}
