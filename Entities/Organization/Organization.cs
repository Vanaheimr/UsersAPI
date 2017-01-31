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
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Illias.Votes;
using org.GraphDefined.Vanaheimr.Styx.Arrows;
using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.OpenData
{

    /// <summary>
    /// An Open Data organization.
    /// </summary>
    public class Organization : AEntity<Organization_Id>,
                                IEquatable<Organization>,
                                IComparable<Organization>,
                                IComparable
    {

        #region Data

        /// <summary>
        /// The default max size of the aggregated user organizations status history.
        /// </summary>
        public const UInt16 DefaultOrganizationStatusHistorySize = 50;

        private readonly ReactiveSet<MiniEdge<User,         User2OrganizationEdges, Organization>>  _User2OrganizationEdges;
        private readonly ReactiveSet<MiniEdge<Organization, Organization2UserEdges, User>>          _Organization2UserEdges;

        #endregion

        #region Properties

        /// <summary>
        /// The offical (multi-language) name of the organization.
        /// </summary>
        [Mandatory]
        public I18NString  Name          { get; }

        /// <summary>
        /// An optional (multi-language) description of the organization.
        /// </summary>
        [Optional]
        public I18NString  Description   { get; }

        /// <summary>
        /// The organization will not be shown in user listings.
        /// </summary>
        [Mandatory]
        public Boolean     IsHidden      { get; }

        #endregion

        #region Events

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new organization.
        /// </summary>
        /// <param name="Id">The unique identification of the organization.</param>
        /// <param name="Name">The offical (multi-language) name of the organization.</param>
        /// <param name="Description">An optional (multi-language) description of the organization.</param>
        public Organization(Organization_Id  Id,
                            I18NString       Name          = null,
                            I18NString       Description   = null)

            : base(Id)

        {

            #region Init properties

            this.Name         = Name        ?? new I18NString();
            this.Description  = Description ?? new I18NString();

            #endregion

            #region Init edges

            this._User2OrganizationEdges    = new ReactiveSet<MiniEdge<User, User2OrganizationEdges, Organization>>();
            this._Organization2UserEdges    = new ReactiveSet<MiniEdge<Organization, Organization2UserEdges, User>>();

            #endregion

        }

        #endregion



        #region Groups()

        /// <summary>
        /// All organizations this user belongs to.
        /// </summary>
        public IEnumerable<Organization> Groups()
            => _User2OrganizationEdges.
                   Select(edge => edge.Target);

        #endregion

        #region Groups(EdgeFilter)

        /// <summary>
        /// All organizations this user belongs to,
        /// filtered by the given edge label.
        /// </summary>
        public IEnumerable<Organization> Groups(User2OrganizationEdges EdgeFilter)
            => _User2OrganizationEdges.
                   Where (edge => edge.EdgeLabel == EdgeFilter).
                   Select(edge => edge.Target);

        #endregion

        #region Edges(Group)

        /// <summary>
        /// All organizations this user belongs to,
        /// filtered by the given edge label.
        /// </summary>
        public IEnumerable<User2OrganizationEdges> Edges(Organization Organization)
            => _User2OrganizationEdges.
                   Where (edge => edge.Target == Organization).
                   Select(edge => edge.EdgeLabel);

        #endregion


        public MiniEdge<User, User2OrganizationEdges, Organization>

            AddIncomingEdge(User            Source,
                            User2OrganizationEdges EdgeLabel,
                            PrivacyLevel    PrivacyLevel = PrivacyLevel.Private)

        {
            return _User2OrganizationEdges.AddAndReturn(new MiniEdge<User, User2OrganizationEdges, Organization>(Source, EdgeLabel, this, PrivacyLevel));
        }

        public MiniEdge<User, User2OrganizationEdges, Organization>

            AddIncomingEdge(MiniEdge<User, User2OrganizationEdges, Organization> Edge)

        {
            return _User2OrganizationEdges.AddAndReturn(Edge);
        }

        public MiniEdge<Organization, Organization2UserEdges, User>

            AddOutgoingEdge(Organization2UserEdges EdgeLabel,
                            User            Target,
                            PrivacyLevel    PrivacyLevel = PrivacyLevel.Private)

        {

            return _Organization2UserEdges.AddAndReturn(new MiniEdge<Organization, Organization2UserEdges, User>(this, EdgeLabel, Target, PrivacyLevel));

        }


        #region IComparable<Organization> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an user organization.
            var EVSE_Operator = Object as Organization;
            if ((Object) EVSE_Operator == null)
                throw new ArgumentException("The given object is not an user organization!");

            return CompareTo(EVSE_Operator);

        }

        #endregion

        #region CompareTo(Operator)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Operator">An user organizations object to compare with.</param>
        public Int32 CompareTo(Organization Operator)
        {

            if ((Object) Operator == null)
                throw new ArgumentNullException("The given user organizations must not be null!");

            return Id.CompareTo(Operator.Id);

        }

        #endregion

        #endregion

        #region IEquatable<Organization> Members

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

            // Check if the given object is an user organization.
            var EVSE_Operator = Object as Organization;
            if ((Object) EVSE_Operator == null)
                return false;

            return this.Equals(EVSE_Operator);

        }

        #endregion

        #region Equals(Organization)

        /// <summary>
        /// Compares two user organizations for equality.
        /// </summary>
        /// <param name="Operator">An user organizations to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Organization Operator)
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
            => Id.GetHashCode();

        #endregion

        #region ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()
            => Id.ToString();

        #endregion

        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON()

            => new JObject(
                   new JProperty("@id",          Id.ToString()),
                   new JProperty("name",         Name),
                   new JProperty("description",  Description)
               );

        #endregion

    }

}
