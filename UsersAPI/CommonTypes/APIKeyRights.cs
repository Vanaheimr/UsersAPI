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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// JSON content representation.
    /// </summary>
    public static class APIKeyRightsExtentions
    {

        public static APIKeyRights Parse(String Text)

            => Text switch {
                "readWrite"    => APIKeyRights.ReadWrite,
                "readWriteNew" => APIKeyRights.ReadWriteNew,
                _              => APIKeyRights.ReadOnly,
            };

        public static String AsText(this APIKeyRights APIKeyRight)

            => APIKeyRight switch {
                APIKeyRights.ReadWrite    => "readWrite",
                APIKeyRights.ReadWriteNew => "readWriteNew",
                _                         => "readOnly",
            };

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
