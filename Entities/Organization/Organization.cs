/*
 * Copyright (c) 2014-2018, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Aegir;

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

        /// <summary>
        /// The JSON-LD context of the object.
        /// </summary>
        public const String JSONLDContext = "https://opendata.social/contexts/UsersAPI+json/organization";


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
        public I18NString           Name                 { get; }

        /// <summary>
        /// The optional (multi-language) description of the organization.
        /// </summary>
        [Optional]
        public I18NString           Description          { get; }

        /// <summary>
        /// The primary E-Mail address of the organization.
        /// </summary>
        [Optional]
        public SimpleEMailAddress?  EMail                { get; }

        /// <summary>
        /// The PGP/GPG public keyring of the organization.
        /// </summary>
        [Optional]
        public String               PublicKeyRing        { get; }

        /// <summary>
        /// The telephone number of the organization.
        /// </summary>
        [Optional]
        public PhoneNumber?         Telephone            { get; }

        /// <summary>
        /// The geographical location of this organization.
        /// </summary>
        public GeoCoordinate?       GeoLocation          { get; }

        /// <summary>
        /// The optional address of the organization.
        /// </summary>
        [Optional]
        public Address              Address              { get; }

        /// <summary>
        /// Whether the organization will be shown in organization listings, or not.
        /// </summary>
        [Mandatory]
        public PrivacyLevel         PrivacyLevel         { get; }

        /// <summary>
        /// The user will be shown in organization listings.
        /// </summary>
        [Mandatory]
        public Boolean              IsDisabled           { get; }

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
        /// <param name="EMail">The primary e-mail of the organisation.</param>
        /// <param name="PublicKeyRing">An optional PGP/GPG public keyring of the organisation.</param>
        /// <param name="Telephone">An optional telephone number of the organisation.</param>
        /// <param name="GeoLocation">An optional geographical location of the organisation.</param>
        /// <param name="Address">An optional address of the organisation.</param>
        /// <param name="PrivacyLevel">Whether the organization will be shown in organization listings, or not.</param>
        /// <param name="IsDisabled">The organization is disabled.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        public Organization(Organization_Id      Id,
                            I18NString           Name            = null,
                            I18NString           Description     = null,
                            SimpleEMailAddress?  EMail           = null,
                            String               PublicKeyRing   = null,
                            PhoneNumber?         Telephone       = null,
                            GeoCoordinate?       GeoLocation     = null,
                            Address              Address         = null,
                            PrivacyLevel?        PrivacyLevel    = null,
                            Boolean              IsDisabled      = false,
                            String               DataSource      = "")

            : base(Id,
                   DataSource)

        {

            #region Init properties

            this.Name           = Name         ?? new I18NString();
            this.Description    = Description  ?? new I18NString();
            this.EMail          = EMail;
            this.Address        = Address;
            this.PublicKeyRing  = PublicKeyRing;
            this.Telephone      = Telephone;
            this.GeoLocation    = GeoLocation;
            this.Address        = Address;
            this.PrivacyLevel   = PrivacyLevel ?? Users.PrivacyLevel.World;
            this.IsDisabled     = IsDisabled;

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


        #region ToJSON(IncludeCryptoHash = true)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeCryptoHash">Include the crypto hash value of this object.</param>
        public override JObject ToJSON(Boolean IncludeCryptoHash = true)

            => JSONObject.Create(

                   new JProperty("@id",                 Id.             ToString()),
                   new JProperty("@context",            JSONLDContext),
                   new JProperty("name",                Name.           ToJSON()),

                   Description.IsNeitherNullNorEmpty()
                       ? new JProperty("description",   Description.    ToJSON())
                       : null,

                   EMail.HasValue
                       ? new JProperty("email",         EMail.Value.    ToString())
                       : null,

                   // PublicKeyRing

                   Telephone.HasValue
                       ? new JProperty("telephone",     Telephone.Value.ToString())
                       : null,

                   GeoLocation?.ToJSON("geoLocation"),
                   Address?.    ToJSON("address"),
                   PrivacyLevel.ToJSON(),

                   new JProperty("isDisabled",          IsDisabled),

                   IncludeCryptoHash
                       ? new JProperty("cryptoHash",    CurrentCryptoHash)
                       : null

               );

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out Organization, out ErrorResponse)

        public static Boolean TryParseJSON(JObject           JSONObject,
                                           out Organization  Organization,
                                           out String        ErrorResponse,
                                           Organization_Id?  OrganizationIdURI = null)
        {

            try
            {

                Organization = null;

                #region Parse OrganizationId   [optional]

                // Verify that a given organization identification
                //   is at least valid.
                if (!JSONObject.ParseOptionalN("@id",
                                               "organization identification",
                                               Organization_Id.TryParse,
                                               out Organization_Id? OrganizationIdBody,
                                               out ErrorResponse))
                {
                    return false;
                }

                if (!OrganizationIdURI.HasValue && !OrganizationIdBody.HasValue)
                {
                    ErrorResponse = "The organization identification is missing!";
                    return false;
                }

                if (OrganizationIdURI.HasValue && OrganizationIdBody.HasValue && OrganizationIdURI.Value != OrganizationIdBody.Value)
                {
                    ErrorResponse = "The optional organization identification given within the JSON body does not match the one given in the URI!";
                    return false;
                }

                #endregion

                #region Parse Context          [mandatory]

                if (!JSONObject.ParseMandatory("@context",
                                               "JSON-LinkedData context information",
                                               out String Context,
                                               out ErrorResponse))
                {
                    ErrorResponse = @"The JSON-LD ""@context"" information is missing!";
                    return false;
                }

                if (Context != JSONLDContext)
                {
                    ErrorResponse = @"The given JSON-LD ""@context"" information '" + Context + "' is not supported!";
                    return false;
                }

                #endregion

                #region Parse Name             [mandatory]

                if (!JSONObject.ParseMandatory("name",
                                               "name",
                                               out I18NString Name,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Description      [optional]

                if (!JSONObject.ParseOptional("description",
                                              "description",
                                              out I18NString Description,
                                              out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse E-Mail           [optional]

                if (!JSONObject.ParseOptionalN("email",
                                               "e-mail address",
                                               SimpleEMailAddress.TryParse,
                                               out SimpleEMailAddress? EMail,
                                               out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse PublicKey        [optional]

                if (!JSONObject.ParseOptional("publicKey",
                                              out String PublicKey))
                {
                    return false;
                }

                #endregion

                #region Parse Telephone        [optional]

                if (!JSONObject.ParseOptional("telephone",
                                              "phone number",
                                              PhoneNumber.TryParse,
                                              out PhoneNumber? Telephone,
                                              out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse GeoLocation      [optional]

                if (!JSONObject.ParseOptionalN("geoLocation",
                                               "geo location",
                                               GeoCoordinate.TryParseJSON,
                                               out GeoCoordinate? GeoLocation,
                                               out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Address          [optional]

                if (JSONObject.ParseOptional("address",
                                             "address",
                                             Vanaheimr.Illias.Address.Parse,
                                             out Address Address,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse PrivacyLevel     [optional]

                if (JSONObject.ParseOptional("privacyLevel",
                                             "privacy level",
                                             out PrivacyLevel? PrivacyLevel,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                var IsDisabled       = JSONObject["isDisabled"]?.     Value<Boolean>();

                #region Get   DataSource       [optional]

                var DataSource = JSONObject.GetOptional("dataSource");

                #endregion

                #region Parse CryptoHash       [optional]

                var CryptoHash    = JSONObject.GetOptional("cryptoHash");

                #endregion


                Organization = new Organization(OrganizationIdBody ?? OrganizationIdURI.Value,
                                                Name,
                                                Description,
                                                EMail,
                                                PublicKey,
                                                Telephone,
                                                GeoLocation,
                                                Address,
                                                PrivacyLevel,
                                                IsDisabled ?? false,
                                                DataSource);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse  = e.Message;
                Organization  = null;
                return false;
            }

        }

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

            var Organization = Object as Organization;
            if ((Object) Organization == null)
                throw new ArgumentException("The given object is not an organization!");

            return CompareTo(Organization);

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
            /// The unique identification of the organization.
            /// </summary>
            public Organization_Id      Id                   { get; set; }

            /// <summary>
            /// The offical (multi-language) name of the organization.
            /// </summary>
            [Mandatory]
            public I18NString           Name                 { get; set; }

            /// <summary>
            /// The optional (multi-language) description of the organization.
            /// </summary>
            [Optional]
            public I18NString           Description          { get; set; }

            /// <summary>
            /// The primary E-Mail address of the organization.
            /// </summary>
            [Mandatory]
            public SimpleEMailAddress?  EMail                { get; set; }

            /// <summary>
            /// The PGP/GPG public keyring of the organization.
            /// </summary>
            [Optional]
            public String               PublicKeyRing        { get; set; }

            /// <summary>
            /// The telephone number of the organization.
            /// </summary>
            [Optional]
            public PhoneNumber?         Telephone            { get; set; }

            /// <summary>
            /// The geographical location of this organization.
            /// </summary>
            public GeoCoordinate?       GeoLocation          { get; set; }

            /// <summary>
            /// The optional address of the organization.
            /// </summary>
            [Optional]
            public Address              Address              { get; set; }

            /// <summary>
            /// Whether the organization will be shown in organization listings, or not.
            /// </summary>
            [Mandatory]
            public PrivacyLevel         PrivacyLevel         { get; set; }

            /// <summary>
            /// The user will be shown in organization listings.
            /// </summary>
            [Mandatory]
            public Boolean              IsDisabled           { get; set; }


            /// <summary>
            /// The source of this information, e.g. an automatic importer.
            /// </summary>
            [Optional]
            public String               DataSource           { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new organization builder.
            /// </summary>
            /// <param name="Id">The unique identification of the organization.</param>
            /// <param name="Name">The offical (multi-language) name of the organization.</param>
            /// <param name="Description">An optional (multi-language) description of the organization.</param>
            /// <param name="EMail">The primary e-mail of the organisation.</param>
            /// <param name="PublicKeyRing">An optional PGP/GPG public keyring of the organisation.</param>
            /// <param name="Telephone">An optional telephone number of the organisation.</param>
            /// <param name="GeoLocation">An optional geographical location of the organisation.</param>
            /// <param name="Address">An optional address of the organisation.</param>
            /// <param name="PrivacyLevel">Whether the organization will be shown in organization listings, or not.</param>
            /// <param name="IsDisabled">The organization is disabled.</param>
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            public Builder(Organization_Id      Id,
                           I18NString           Name            = null,
                           I18NString           Description     = null,
                           SimpleEMailAddress?  EMail           = null,
                           String               PublicKeyRing   = null,
                           PhoneNumber?         Telephone       = null,
                           GeoCoordinate?       GeoLocation     = null,
                           Address              Address         = null,
                           PrivacyLevel         PrivacyLevel    = PrivacyLevel.World,
                           Boolean              IsDisabled      = false,
                           String               DataSource      = "")
            {

                #region Init properties

                this.Id             = Id;
                this.Name           = Name        ?? new I18NString();
                this.Description    = Description ?? new I18NString();
                this.EMail          = EMail;
                this.Address        = Address;
                this.PublicKeyRing  = PublicKeyRing;
                this.Telephone      = Telephone;
                this.GeoLocation    = GeoLocation;
                this.Address        = Address;
                this.PrivacyLevel   = PrivacyLevel;
                this.IsDisabled     = IsDisabled;
                this.DataSource     = DataSource;

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
                                    EMail,
                                    PublicKeyRing,
                                    Telephone,
                                    GeoLocation,
                                    Address,
                                    PrivacyLevel,
                                    IsDisabled,
                                    DataSource);

            #endregion

        }

        #endregion

    }

}
