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
    public static class JSON
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

    }

}
