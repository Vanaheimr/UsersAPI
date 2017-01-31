/*
 * Copyright (c) 2014-2017, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using org.GraphDefined.Vanaheimr.Styx.Arrows;
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace org.GraphDefined.OpenData
{

    /// <summary>
    /// An Open Data group.
    /// </summary>
    public class Group : AEntity<Group_Id>,
                         IEquatable<Group>,
                         IComparable<Group>,
                         IComparable
    {

        #region Data

        /// <summary>
        /// The default max size of the aggregated user groups status history.
        /// </summary>
        public const UInt16 DefaultGroupStatusHistorySize = 50;

        private readonly ReactiveSet<MiniEdge<User,      User2GroupEdges, Group>> _User2GroupEdges;
        private readonly ReactiveSet<MiniEdge<Group, Group2UserEdges, User>>      _Group2UserEdges;

        #endregion

        #region Properties

        /// <summary>
        /// The offical (multi-language) name of the group.
        /// </summary>
        [Mandatory]
        public I18NString  Name          { get; }

        /// <summary>
        /// An optional (multi-language) description of the group.
        /// </summary>
        [Optional]
        public I18NString  Description   { get; }

        /// <summary>
        /// The group will be shown in group listings.
        /// </summary>
        [Mandatory]
        public Boolean     IsPublic      { get; }

        #endregion

        #region Events

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new user group.
        /// </summary>
        /// <param name="Id">The unique identification of the user group.</param>
        /// <param name="Name">The offical (multi-language) name of the user group.</param>
        /// <param name="Description">An optional (multi-language) description of the user group.</param>
        internal Group(Group_Id    Id,
                       I18NString  Name          = null,
                       I18NString  Description   = null)

            : base(Id)

        {

            #region Initial checks

            if (Id == null)
                throw new ArgumentNullException(nameof(Id), "The group identification must not be null!");

            #endregion

            #region Init data and properties

            this.Name                      = Name        ?? new I18NString();
            this.Description               = Description ?? new I18NString();

            this._User2GroupEdges          = new ReactiveSet<MiniEdge<User, User2GroupEdges, Group>>();
            this._Group2UserEdges          = new ReactiveSet<MiniEdge<Group, Group2UserEdges, User>>();

            #endregion

        }

        #endregion



        #region Groups()

        /// <summary>
        /// All groups this user belongs to.
        /// </summary>
        public IEnumerable<Group> Groups()
            => _User2GroupEdges.
                   Select(edge => edge.Target);

        #endregion

        #region Groups(EdgeFilter)

        /// <summary>
        /// All groups this user belongs to,
        /// filtered by the given edge label.
        /// </summary>
        public IEnumerable<Group> Groups(User2GroupEdges EdgeFilter)
            => _User2GroupEdges.
                   Where (edge => edge.EdgeLabel == EdgeFilter).
                   Select(edge => edge.Target);

        #endregion

        #region Edges(Group)

        /// <summary>
        /// All groups this user belongs to,
        /// filtered by the given edge label.
        /// </summary>
        public IEnumerable<User2GroupEdges> Edges(Group Group)
            => _User2GroupEdges.
                   Where (edge => edge.Target == Group).
                   Select(edge => edge.EdgeLabel);

        #endregion


        public MiniEdge<User, User2GroupEdges, Group>

            AddIncomingEdge(User            Source,
                            User2GroupEdges EdgeLabel,
                            PrivacyLevel    PrivacyLevel = PrivacyLevel.Private)

        {
            return _User2GroupEdges.AddAndReturn(new MiniEdge<User, User2GroupEdges, Group>(Source, EdgeLabel, this, PrivacyLevel));
        }

        public MiniEdge<User, User2GroupEdges, Group>

            AddIncomingEdge(MiniEdge<User, User2GroupEdges, Group> Edge)

        {
            return this._User2GroupEdges.AddAndReturn(Edge);
        }

        public MiniEdge<Group, Group2UserEdges, User>

            AddOutgoingEdge(Group2UserEdges EdgeLabel,
                            User            Target,
                            PrivacyLevel    PrivacyLevel = PrivacyLevel.Private)

        {

            return _Group2UserEdges.AddAndReturn(new MiniEdge<Group, Group2UserEdges, User>(this, EdgeLabel, Target, PrivacyLevel));

        }


        #region IComparable<Group> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an user group.
            var EVSE_Operator = Object as Group;
            if ((Object) EVSE_Operator == null)
                throw new ArgumentException("The given object is not an user group!");

            return CompareTo(EVSE_Operator);

        }

        #endregion

        #region CompareTo(Operator)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Operator">An user groups object to compare with.</param>
        public Int32 CompareTo(Group Operator)
        {

            if ((Object) Operator == null)
                throw new ArgumentNullException("The given user groups must not be null!");

            return Id.CompareTo(Operator.Id);

        }

        #endregion

        #endregion

        #region IEquatable<Group> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            // Check if the given object is an user group.
            var EVSE_Operator = Object as Group;
            if ((Object) EVSE_Operator == null)
                return false;

            return this.Equals(EVSE_Operator);

        }

        #endregion

        #region Equals(Group)

        /// <summary>
        /// Compares two user groups for equality.
        /// </summary>
        /// <param name="Operator">An user groups to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Group Operator)
        {

            if ((Object) Operator == null)
                return false;

            return Id.Equals(Operator.Id);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion

        #region ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()
        {
            return Id.ToString();
        }

        #endregion

        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON()

            => new JObject(
                   new JProperty("id",           Id.         ToString()),
                   new JProperty("name",         Name.       ToJSON()),
                   new JProperty("description",  Description.ToJSON())
               );

        #endregion

    }

}
