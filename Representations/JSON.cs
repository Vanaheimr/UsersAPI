/*
 * Copyright (c) 2014-2015, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace org.GraphDefined.UsersAPI
{

    /// <summary>
    /// JSON content representation.
    /// </summary>
    public static class JSON
    {

        //#region ToUTF8Bytes(JObject)

        //public static Byte[] ToUTF8Bytes(this JObject JObject)
        //{
        //    return JObject.ToString().ToUTF8Bytes();
        //}

        //#endregion

        //#region ToUTF8Bytes(JArray)

        //public static Byte[] ToUTF8Bytes(this JArray JArray)
        //{
        //    return JArray.ToString().ToUTF8Bytes();
        //}

        //#endregion


        #region ErrorMessage(Message)

        public static JObject ErrorMessage(String Message)
        {

            return new JObject(new JProperty("@context",     "http://api.opendata.social/context/errors"),
                               new JProperty("description",  Message));

        }

        #endregion

        #region ToJSON(User)

        private static JObject AsJSON(this User User)
        {

            return new JObject(new JProperty("@id",          User.Id.ToString()),
                               new JProperty("name",         User.Name),
                               new JProperty("inedges",      new JObject(
                                   new JProperty("IsFollowedBy",     new JArray(User.FollowsUsers.SafeSelect(u => u.Id.ToString())))
                               )),
                               new JProperty("outedges",     new JObject(
                                   new JProperty("follows",          new JArray(User.FollowsUsers.SafeSelect(u => u.Id.ToString())))
                               ))
                              );

        }

        public static JObject ToJSON(this User User)
        {

            return new JObject(new JProperty("@context",  "http://api.opendata.social/context/user"),
                               new JProperty("user",      User.AsJSON()));

        }

        #endregion

        #region ToJSON(Users)

        public static JObject ToJSON(this IEnumerable<User> Users)
        {

            return new JObject(new JProperty("@context",  "http://api.opendata.social/context/users"),
                               new JProperty("users",     new JArray(Users.SafeSelect(user => user.AsJSON()))));

        }

        #endregion

        #region ToJSON(Group)

        private static JObject AsJSON(this UserGroup Group)
        {

            return new JObject(new JProperty("@id",          Group.Id.ToString()),
                               new JProperty("name",         Group.Name.ToJSON())
                               //new JProperty("inedges",      new JObject(
                               //    new JProperty("IsFollowedBy",     new JArray(Group.FollowsGroups.SafeSelect(u => u.Id.ToString())))
                               //)),
                               //new JProperty("outedges",     new JObject(
                               //    new JProperty("follows",          new JArray(Group.FollowsGroups.SafeSelect(u => u.Id.ToString())))
                               //))
                              );

        }

        public static JObject ToJSON(this UserGroup Group)
        {

            return new JObject(new JProperty("@context",  "http://api.opendata.social/context/group"),
                               new JProperty("group",     Group.AsJSON()));

        }

        #endregion

        #region ToJSON(Groups)

        public static JObject ToJSON(this IEnumerable<UserGroup> Groups)
        {

            return new JObject(new JProperty("@context",  "http://api.opendata.social/context/groups"),
                               new JProperty("groups",    new JArray(Groups.SafeSelect(group => group.AsJSON()))));

        }

        #endregion

    }

}
