/*
 * Copyright (c) 2014-2021, Achim Friedland <achim.friedland@graphdefined.com>
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

using System;

#endregion

namespace social.OpenData.UsersAPI
{

    [Flags]
    public enum User2UserGroupEdgeTypes
    {
        IsRoot,
        IsAdmin_ReadOnly,
        IsAdmin,
        IsMember,
        IsGuest
    }

    public class User2UserGroupEdge : MiniEdge<User, User2UserGroupEdgeTypes, UserGroup>
    {

        /// <summary>
        /// Create a new miniedge.
        /// </summary>
        /// <param name="User">The source of the edge.</param>
        /// <param name="EdgeLabel">The label of the edge.</param>
        /// <param name="UserGroup">The target of the edge</param>
        /// <param name="PrivacyLevel">The level of privacy of this edge.</param>
        /// <param name="Created">The creation timestamp of the miniedge.</param>
        public User2UserGroupEdge(User                     User,
                                  User2UserGroupEdgeTypes  EdgeLabel,
                                  UserGroup                UserGroup,
                                  PrivacyLevel             PrivacyLevel  = PrivacyLevel.Private,
                                  DateTime?                Created       = null)

            : base(User      ?? throw new ArgumentNullException(nameof(User),       "The given user must not be null!"),
                   EdgeLabel,
                   UserGroup ?? throw new ArgumentNullException(nameof(UserGroup),  "The given user group must not be null!"),
                   PrivacyLevel,
                   Created)

        { }

    }

}
