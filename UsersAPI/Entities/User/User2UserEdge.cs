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
using social.OpenData.UsersAPI;

#endregion

namespace social.OpenData.UsersAPI
{

    public enum User2UserEdgeTypes
    {
        gemini,
        follows,
        IsFollowedBy
    }


    public class User2UserEdge : MiniEdge<User, User2UserEdgeTypes, User>
    {

        /// <summary>
        /// Create a new miniedge.
        /// </summary>
        /// <param name="UserA">The source of the edge.</param>
        /// <param name="EdgeLabel">The label of the edge.</param>
        /// <param name="UserB">The target of the edge</param>
        /// <param name="PrivacyLevel">The level of privacy of this edge.</param>
        /// <param name="Created">The creation timestamp of the miniedge.</param>
        public User2UserEdge(User                UserA,
                             User2UserEdgeTypes  EdgeLabel,
                             User                UserB,
                             PrivacyLevel        PrivacyLevel  = PrivacyLevel.Private,
                             DateTime?           Created       = null)

            : base(UserA ?? throw new ArgumentNullException(nameof(UserA), "The given user must not be null!"),
                   EdgeLabel,
                   UserB ?? throw new ArgumentNullException(nameof(UserB), "The given user must not be null!"),
                   PrivacyLevel,
                   Created)

        { }

    }

}
