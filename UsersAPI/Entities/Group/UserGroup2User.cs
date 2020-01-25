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
using social.OpenData.UsersAPI;

#endregion

namespace social.OpenData.UsersAPI
{

    [Flags]
    public enum Group2UserEdgeTypes
    {
        IsAdmin,
        IsMember,
        IsFollower
    }


    public class Group2UserEdge : MiniEdge<Group, Group2UserEdgeTypes, User>
    {

        /// <summary>
        /// Create a new miniedge.
        /// </summary>
        /// <param name="Group">The source of the edge.</param>
        /// <param name="EdgeLabel">The label of the edge.</param>
        /// <param name="User">The target of the edge</param>
        /// <param name="PrivacyLevel">The level of privacy of this edge.</param>
        /// <param name="Created">The creation timestamp of the miniedge.</param>
        public Group2UserEdge(Group                Group,
                              Group2UserEdgeTypes  EdgeLabel,
                              User                 User,
                              PrivacyLevel         PrivacyLevel  = PrivacyLevel.Private,
                              DateTime?            Created       = null)

            : base(Group ?? throw new ArgumentNullException(nameof(Group),  "The given group must not be null!"),
                   EdgeLabel,
                   User  ?? throw new ArgumentNullException(nameof(User),   "The given user must not be null!"),
                   PrivacyLevel,
                   Created)

        { }

    }

}
