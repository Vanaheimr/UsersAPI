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

    public enum Group2GroupEdgeTypes
    {
        IsSubgroup
    }

    public class Group2GroupEdge : MiniEdge<UserGroup, Group2GroupEdgeTypes, UserGroup>
    {

        /// <summary>
        /// Create a new miniedge.
        /// </summary>
        /// <param name="GroupA">The source of the edge.</param>
        /// <param name="EdgeLabel">The label of the edge.</param>
        /// <param name="GroupB">The target of the edge</param>
        /// <param name="PrivacyLevel">The level of privacy of this edge.</param>
        /// <param name="Created">The creation timestamp of the miniedge.</param>
        public Group2GroupEdge(UserGroup             GroupA,
                               Group2GroupEdgeTypes  EdgeLabel,
                               UserGroup             GroupB,
                               PrivacyLevel          PrivacyLevel  = PrivacyLevel.Private,
                               DateTime?             Created       = null)

            : base(GroupA ?? throw new ArgumentNullException(nameof(GroupA), "The given group must not be null!"),
                   EdgeLabel,
                   GroupB ?? throw new ArgumentNullException(nameof(GroupB), "The given group must not be null!"),
                   PrivacyLevel,
                   Created)

        { }

    }

}
