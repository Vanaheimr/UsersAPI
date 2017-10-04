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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Styx.Arrows;
using org.GraphDefined.Vanaheimr.Hermod.Distributed;

#endregion

namespace org.GraphDefined.OpenData.Users
{

    /// <summary>
    /// A organization.
    /// </summary>
    public class Organization : ADistributedEntity<Organization_Id>,
                                IEntityClass<Organization>
    {

        #region Data

        /// <summary>
        /// The default max size of the aggregated user organizations status history.
        /// </summary>
        public const UInt16 DefaultOrganizationStatusHistorySize = 50;

        private readonly ReactiveSet<MiniEdge<User,         User2OrganizationEdges,         Organization>>  _User2OrganizationEdges;

        public IEnumerable<MiniEdge<User, User2OrganizationEdges, Organization>> User2OrganizationEdges
            => _User2OrganizationEdges;


        private readonly ReactiveSet<MiniEdge<Organization, Organization2UserEdges,         User>>          _Organization2UserEdges;

        public IEnumerable<MiniEdge<Organization, Organization2UserEdges, User>> Organization2UserEdges
            => _Organization2UserEdges;


        private readonly ReactiveSet<MiniEdge<Organization, Organization2OrganizationEdges, Organization>>  _Organization2OrganizationInEdges;

        public IEnumerable<MiniEdge<Organization, Organization2OrganizationEdges, Organization>> Organization2OrganizationInEdges
            => _Organization2OrganizationInEdges;


        private readonly ReactiveSet<MiniEdge<Organization, Organization2OrganizationEdges, Organization>> _Organization2OrganizationOutEdges;

        public IEnumerable<MiniEdge<Organization, Organization2OrganizationEdges, Organization>> Organization2OrganizationOutEdges
            => _Organization2OrganizationOutEdges;

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
        /// The user will be shown in organization listings.
        /// </summary>
        [Mandatory]
        public Boolean     IsPublic      { get; }

        /// <summary>
        /// The user will be shown in organization listings.
        /// </summary>
        [Mandatory]
        public Boolean     IsDisabled    { get; }

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
        /// <param name="IsPublic">The organization will be shown in user listings.</param>
        /// <param name="IsDisabled">The organization is disabled.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        public Organization(Organization_Id  Id,
                            I18NString       Name          = null,
                            I18NString       Description   = null,
                            Boolean          IsPublic      = true,
                            Boolean          IsDisabled    = false,
                            String           DataSource    = "")

            : base(Id,
                   DataSource)

        {

            #region Init properties

            this.Name          = Name        ?? new I18NString();
            this.Description   = Description ?? new I18NString();

            this.IsPublic      = IsPublic;
            this.IsDisabled    = IsDisabled;

            #endregion

            #region Init edges

            this._User2OrganizationEdges             = new ReactiveSet<MiniEdge<User,         User2OrganizationEdges,         Organization>>();
            this._Organization2UserEdges             = new ReactiveSet<MiniEdge<Organization, Organization2UserEdges,         User>>();
            this._Organization2OrganizationInEdges   = new ReactiveSet<MiniEdge<Organization, Organization2OrganizationEdges, Organization>>();
            this._Organization2OrganizationOutEdges  = new ReactiveSet<MiniEdge<Organization, Organization2OrganizationEdges, Organization>>();

            #endregion

            CalcHash();

        }

        #endregion


        #region User         -> Organization edges

        public MiniEdge<User, User2OrganizationEdges, Organization>

            AddIncomingEdge(User                    Source,
                            User2OrganizationEdges  EdgeLabel,
                            PrivacyLevel            PrivacyLevel = PrivacyLevel.Private)

            => _User2OrganizationEdges.AddAndReturn(new MiniEdge<User, User2OrganizationEdges, Organization>(Source,
                                                                                                             EdgeLabel,
                                                                                                             this,
                                                                                                             PrivacyLevel));

        public MiniEdge<User, User2OrganizationEdges, Organization>

            AddIncomingEdge(MiniEdge<User, User2OrganizationEdges, Organization> Edge)

            => _User2OrganizationEdges.AddAndReturn(Edge);


        public MiniEdge<Organization, Organization2OrganizationEdges, Organization>

            AddIncomingEdge(Organization                    Organization,
                            Organization2OrganizationEdges  EdgeLabel,
                            PrivacyLevel                    PrivacyLevel = PrivacyLevel.Private)

            => _Organization2OrganizationInEdges.AddAndReturn(new MiniEdge<Organization, Organization2OrganizationEdges, Organization>(Organization,
                                                                                                                                       EdgeLabel,
                                                                                                                                       this,
                                                                                                                                       PrivacyLevel));


        #region Edges(Organization)

        /// <summary>
        /// All organizations this user belongs to,
        /// filtered by the given edge label.
        /// </summary>
        public IEnumerable<User2OrganizationEdges> InEdges(Organization Organization)
            => _User2OrganizationEdges.
                   Where (edge => edge.Target == Organization).
                   Select(edge => edge.EdgeLabel);

        #endregion

        #endregion

        #region Organization -> User         edges

        public MiniEdge<Organization, Organization2UserEdges, User>

            AddOutgoingEdge(Organization2UserEdges  EdgeLabel,
                            User                    Target,
                            PrivacyLevel            PrivacyLevel = PrivacyLevel.Private)

            => _Organization2UserEdges.AddAndReturn(new MiniEdge<Organization, Organization2UserEdges, User>(this,
                                                                                                             EdgeLabel,
                                                                                                             Target,
                                                                                                             PrivacyLevel));

        public MiniEdge<Organization, Organization2UserEdges, User>

            AddIncomingEdge(MiniEdge<Organization, Organization2UserEdges, User> Edge)

            => _Organization2UserEdges.AddAndReturn(Edge);


        #region Edges(User)

        /// <summary>
        /// All organizations this user belongs to,
        /// filtered by the given edge label.
        /// </summary>
        public IEnumerable<Organization2UserEdges> InEdges(User User)
            => _Organization2UserEdges.
                   Where (edge => edge.Target == User).
                   Select(edge => edge.EdgeLabel);

        #endregion

        #endregion

        #region Organization -> Organization edges

        public MiniEdge<Organization, Organization2OrganizationEdges, Organization>

            AddInEdge(Organization2OrganizationEdges  EdgeLabel,
                      Organization                    Target,
                      PrivacyLevel                    PrivacyLevel = PrivacyLevel.Private)

            => _Organization2OrganizationInEdges.AddAndReturn(new MiniEdge<Organization, Organization2OrganizationEdges, Organization>(this,
                                                                                                                                       EdgeLabel,
                                                                                                                                       Target,
                                                                                                                                       PrivacyLevel));

        public MiniEdge<Organization, Organization2OrganizationEdges, Organization>

            AddOutEdge(Organization2OrganizationEdges  EdgeLabel,
                       Organization                    Target,
                       PrivacyLevel                    PrivacyLevel = PrivacyLevel.Private)

            => _Organization2OrganizationOutEdges.AddAndReturn(new MiniEdge<Organization, Organization2OrganizationEdges, Organization>(this,
                                                                                                                                        EdgeLabel,
                                                                                                                                        Target,
                                                                                                                                        PrivacyLevel));


        public MiniEdge<Organization, Organization2OrganizationEdges, Organization>

            AddInEdge(MiniEdge<Organization, Organization2OrganizationEdges, Organization> Edge)

            => _Organization2OrganizationInEdges.AddAndReturn(Edge);

        public MiniEdge<Organization, Organization2OrganizationEdges, Organization>

            AddOutEdge(MiniEdge<Organization, Organization2OrganizationEdges, Organization> Edge)

            => _Organization2OrganizationOutEdges.AddAndReturn(Edge);


        //#region Edges(Organization)

        ///// <summary>
        ///// All organizations this user belongs to,
        ///// filtered by the given edge label.
        ///// </summary>
        //public IEnumerable<User2OrganizationEdges> Edges(Organization Organization)
        //    => _User2OrganizationEdges.
        //           Where (edge => edge.Target == Organization).
        //           Select(edge => edge.EdgeLabel);

        //#endregion

        #endregion


        #region IComparable<Organization> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            var EVSE_Operator = Object as Organization;
            if ((Object) EVSE_Operator == null)
                throw new ArgumentException("The given object is not an organization!");

            return CompareTo(EVSE_Operator);

        }

        #endregion

        #region CompareTo(Organization)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Organization">An organization object to compare with.</param>
        public Int32 CompareTo(Organization Organization)
        {

            if ((Object) Organization == null)
                throw new ArgumentNullException(nameof(Organization), "The given organization must not be null!");

            return Id.CompareTo(Organization.Id);

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

            var Organization = Object as Organization;
            if ((Object) Organization == null)
                return false;

            return Equals(Organization);

        }

        #endregion

        #region Equals(Organization)

        /// <summary>
        /// Compares two organizations for equality.
        /// </summary>
        /// <param name="Organization">An organization to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Organization Organization)
        {

            if ((Object) Organization == null)
                return false;

            return Id.Equals(Organization.Id);

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

        #region ToJSON(IncludeHash = true)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeHash">Include the hash value of this object.</param>
        public override JObject ToJSON(Boolean IncludeHash = true)

            => JSONObject.Create(

                   new JProperty("@id",          Id.         ToString()),
                   new JProperty("name",         Name.       ToJSON()),
                   new JProperty("description",  Description.ToJSON()),
                   new JProperty("isPublic",     IsPublic),
                   new JProperty("isDisabled",   IsDisabled),

                   IncludeHash
                       ? new JProperty("Hash",   CurrentCryptoHash)
                       : null

               );

        #endregion


        #region ToBuilder(NewOrganizationId = null)

        /// <summary>
        /// Return a builder for this organization.
        /// </summary>
        /// <param name="NewOrganizationId">An optional new organization identification.</param>
        public Builder ToBuilder(Organization_Id? NewOrganizationId = null)

            => new Builder(NewOrganizationId ?? Id,
                           Name,
                           Description);

        #endregion

        #region (class) Builder

        /// <summary>
        /// An organization builder.
        /// </summary>
        public class Builder
        {

            #region Properties

            /// <summary>
            /// The organization identification.
            /// </summary>
            public Organization_Id  Id                   { get; set; }

            /// <summary>
            /// The offical public name of the organization.
            /// </summary>
            [Optional]
            public I18NString       Name                 { get; set; }

            /// <summary>
            /// An optional (multi-language) description of the organization.
            /// </summary>
            [Optional]
            public I18NString       Description          { get; set; }

            /// <summary>
            /// The organization will be shown in organization listings.
            /// </summary>
            [Mandatory]
            public Boolean          IsPublic             { get; set; }

            /// <summary>
            /// The organization is disabled.
            /// </summary>
            [Mandatory]
            public Boolean          IsDisabled           { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new organization builder.
            /// </summary>
            /// <param name="Id">The unique identification of the organization.</param>
            /// <param name="Name">An offical (multi-language) name of the organization.</param>
            /// <param name="Description">An optional (multi-language) description of the organization.</param>
            /// <param name="IsPublic">The organization will be shown in organization listings.</param>
            /// <param name="IsDisabled">The organization is disabled.</param>
            public Builder(Organization_Id  Id,
                           I18NString       Name          = null,
                           I18NString       Description   = null,
                           Boolean          IsPublic      = true,
                           Boolean          IsDisabled    = false)
            {

                #region Init properties

                this.Id           = Id;
                this.Name         = Name        ?? new I18NString();
                this.Description  = Description ?? new I18NString();

                this.IsPublic     = IsPublic;
                this.IsDisabled   = IsDisabled;

                #endregion

                #region Init edges

                //this._User2UserEdges          = new ReactiveSet<MiniEdge<User, User2UserEdges,         User>>();
                //this._User2GroupEdges         = new ReactiveSet<MiniEdge<User, User2GroupEdges,        Group>>();
                //this._User2OrganizationEdges  = new ReactiveSet<MiniEdge<User, User2OrganizationEdges, Organization>>();

                #endregion

            }

            #endregion


            #region Build()

            /// <summary>
            /// Return an immutable version of the organization.
            /// </summary>
            public Organization Build()

                => new Organization(Id,
                                    Name,
                                    Description,

                                    IsPublic,
                                    IsDisabled);

            #endregion

        }

        #endregion

    }

}
