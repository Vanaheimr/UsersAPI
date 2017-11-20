/*
 * Copyright (c) 2014-2017, Achim 'ahzf' Friedland <achim@graphdefined.org>
 * This file is part of Open Data Graph API <http://www.github.com/GraphDefined/OpenDataAPI>
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
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace org.GraphDefined.OpenData.Users
{

    /// <summary>
    /// JSON content representation.
    /// </summary>
    public static class JSON_IO
    {

        #region ToJSON(Users)

        public static JObject ToJSON(this IEnumerable<User> Users)

            => JSONObject.Create(
                   new JProperty("@context",  "https://api.opendata.social/context/users"),
                   new JProperty("users",     new JArray(Users.SafeSelect(user => user.ToJSON())))
               );

        #endregion

        #region ToJSON(Groups)

        public static JObject ToJSON(this IEnumerable<Group> Groups)
        {

            return new JObject(new JProperty("@context",  "https://api.opendata.social/context/groups"),
                               new JProperty("groups",    new JArray(Groups.SafeSelect(group => group.ToJSON()))));

        }

        #endregion

        #region ToJSON(Organizations)

        public static JObject ToJSON(this IEnumerable<Organization> Organizations)
        {

            return new JObject(new JProperty("@context",  "http://api.opendata.social/context/organizations"),
                               new JProperty("orgs",      new JArray(Organizations.SafeSelect(organization => organization.ToJSON()))));

        }

        #endregion


        #region ToJSON(this Privacylevel)

        public static JProperty ToJSON(this PrivacyLevel Privacylevel)
        {

            switch (Privacylevel)
            {

                case PrivacyLevel.Internal:
                    return new JProperty("privacyLevel",  "internal");

                case PrivacyLevel.Friends:
                    return new JProperty("privacyLevel",  "friends");

                case PrivacyLevel.World:
                    return new JProperty("privacyLevel",  "world");

                default:
                    return new JProperty("privacyLevel",  "private");

            }

        }

        #endregion

        #region ParsePrivacyLevel(this Text)

        public static PrivacyLevel ParsePrivacyLevel(this String Text)
        {

            switch (Text)
            {

                case "internal":
                    return PrivacyLevel.Internal;

                case "friends":
                    return PrivacyLevel.Friends;

                case "world":
                    return PrivacyLevel.World;

                default:
                    return PrivacyLevel.Private;

            }

        }

        public static PrivacyLevel ParseMandatory_PrivacyLevel(this JObject JSON)
        {

            var Value = JSON["privacyLevel"]?.Value<String>();

            if (Value.IsNullOrEmpty())
                throw new Exception("Missing JSON property 'privacyLevel'!");

            switch (Value)
            {

                case "internal":
                    return PrivacyLevel.Internal;

                case "friends":
                    return PrivacyLevel.Friends;

                case "world":
                    return PrivacyLevel.World;

                case "privat":
                    return PrivacyLevel.Private;

                default:
                    throw new Exception("Invalid value '" + Value + "' for JSON property 'privacyLevel'!");

            }

        }

        public static Boolean TryParseMandatory_PrivacyLevel(this JObject      JSON,
                                                             out PrivacyLevel  PrivacyLevel,
                                                             out String        ErrorResponse)
        {

            var Value = JSON["privacyLevel"]?.Value<String>();

            if (Value.IsNullOrEmpty())
            {
                PrivacyLevel   = PrivacyLevel.Private;
                ErrorResponse  = "Missing JSON property 'privacyLevel'!";
                return false;
            }

            switch (Value)
            {

                case "internal":
                    PrivacyLevel   = PrivacyLevel.Internal;
                    ErrorResponse  = String.Empty;
                    return true;

                case "friends":
                    PrivacyLevel   = PrivacyLevel.Friends;
                    ErrorResponse  = String.Empty;
                    return true;

                case "world":
                    PrivacyLevel   = PrivacyLevel.World;
                    ErrorResponse  = String.Empty;
                    return true;

                case "privat":
                    PrivacyLevel   = PrivacyLevel.Private;
                    ErrorResponse  = String.Empty;
                    return true;

                default:
                    PrivacyLevel   = PrivacyLevel.Private;
                    ErrorResponse  = "Invalid value '" + Value + "' for JSON property 'privacyLevel'!";
                    return false;

            }

        }

        public static PrivacyLevel? ParseOptional_PrivacyLevel(this JObject JSON)
        {

            var Value = JSON["privacyLevel"]?.Value<String>();

            if (Value.IsNullOrEmpty())
                return new PrivacyLevel?();

            switch (Value)
            {

                case "internal":
                    return PrivacyLevel.Internal;

                case "friends":
                    return PrivacyLevel.Friends;

                case "world":
                    return PrivacyLevel.World;

                default:
                    return PrivacyLevel.Private;

            }

        }

        #endregion

        #region ToJSON(this APIKeyRights)

        public static JProperty ToJSON(this APIKeyRights APIKeyRights)
        {

            switch (APIKeyRights)
            {

                case APIKeyRights.ReadWrite:
                    return new JProperty("accessRights",  "readWrite");

                default:
                    return new JProperty("accessRights", "readOnly");

            }

        }

        #endregion

        #region ParseAPIKeyRights(this Text)

        public static APIKeyRights ParseAPIKeyRights(this String Text)
        {

            switch (Text)
            {

                case "readWrite":
                    return APIKeyRights.ReadWrite;

                default:
                    return APIKeyRights.ReadOnly;

            }

        }

        public static APIKeyRights ParseMandatory_APIKeyRights(this JObject JSON)

            => ParseMandatory_APIKeyRights(JSON["accessRights"]?.Value<String>());


        public static APIKeyRights ParseMandatory_APIKeyRights(this String Value)
        {

            if (Value.IsNullOrEmpty())
                throw new Exception("Invalid value!");

            switch (Value)
            {

                case "readWrite":
                    return APIKeyRights.ReadWrite;

                case "readOnly":
                    return APIKeyRights.ReadOnly;

                default:
                    throw new Exception("Invalid value '" + Value + "' for JSON property 'accessRights'!");

            }

        }

        public static Boolean TryParseMandatory_APIKeyRights(this JObject      JSON,
                                                             out APIKeyRights  APIKeyRights,
                                                             out String        ErrorResponse)
        {

            var Value = JSON["accessRights"]?.Value<String>();

            if (Value.IsNullOrEmpty())
            {
                APIKeyRights   = APIKeyRights.ReadOnly;
                ErrorResponse  = "Missing JSON property 'accessRights'!";
                return false;
            }

            switch (Value)
            {

                case "readWrite":
                    APIKeyRights   = APIKeyRights.ReadWrite;
                    ErrorResponse  = String.Empty;
                    return true;

                case "readOnly":
                    APIKeyRights   = APIKeyRights.ReadOnly;
                    ErrorResponse  = String.Empty;
                    return true;

                default:
                    APIKeyRights   = APIKeyRights.ReadOnly;
                    ErrorResponse  = "Invalid value '" + Value + "' for JSON property 'accessRights'!";
                    return false;

            }

        }

        public static APIKeyRights? ParseOptional_APIKeyRights(this JObject JSON)
        {

            var Value = JSON["accessRights"]?.Value<String>();

            if (Value.IsNullOrEmpty())
                return new APIKeyRights?();

            switch (Value)
            {

                case "readWrite":
                    return APIKeyRights.ReadWrite;

                default:
                    return APIKeyRights.ReadOnly;

            }

        }

        #endregion


    }

}
