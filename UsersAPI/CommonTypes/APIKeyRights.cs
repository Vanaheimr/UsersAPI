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

using Newtonsoft.Json.Linq;
using org.GraphDefined.Vanaheimr.Illias;
using System;

namespace social.OpenData.UsersAPI
{

        /// <summary>
    /// JSON content representation.
    /// </summary>
    public static class APIKeyRightsExtentions
    {

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

    /// <summary>
    /// Access rights for API keys.
    /// </summary>
    public enum APIKeyRights
    {

        /// <summary>
        /// Read-only access.
        /// </summary>
        ReadOnly,

        /// <summary>
        /// Read & write access.
        /// </summary>
        ReadWrite,

        /// <summary>
        /// Read & write access and can create new API keys.
        /// </summary>
        ReadWriteNew

    }

}
