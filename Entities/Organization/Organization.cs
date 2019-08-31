/*
 * Copyright (c) 2014-2019, Achim 'ahzf' Friedland <achim@graphdefined.org>
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

    public delegate Boolean OrganizationProviderDelegate(Organization_Id OrganizationId, out Organization Organization);

    public delegate JObject OrganizationToJSONDelegate(Organization  Organization,
                                                       Boolean       Embedded                 = false,
                                                       InfoStatus    ExpandMembers            = InfoStatus.ShowIdOnly,
                                                       InfoStatus    ExpandParents            = InfoStatus.ShowIdOnly,
                                                       InfoStatus    ExpandSubOrganizations   = InfoStatus.ShowIdOnly,
                                                       InfoStatus    ExpandTags               = InfoStatus.ShowIdOnly,
                                                       Boolean       IncludeCryptoHash        = true);


    /// <summary>
    /// Extention methods for organizations.
    /// </summary>
    public static class OrganizationExtentions
    {

        #region ToJSON(this Organizations, Skip = null, Take = null, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of organizations.
        /// </summary>
        /// <param name="Organizations">An enumeration of organizations.</param>
        /// <param name="Skip">The optional number of organizations to skip.</param>
        /// <param name="Take">The optional number of organizations to return.</param>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        public static JArray ToJSON(this IEnumerable<Organization>  Organizations,
                                    UInt64?                         Skip                     = null,
                                    UInt64?                         Take                     = null,
                                    Boolean                         Embedded                 = false,
                                    InfoStatus                      ExpandMembers            = InfoStatus.ShowIdOnly,
                                    InfoStatus                      ExpandParents            = InfoStatus.ShowIdOnly,
                                    InfoStatus                      ExpandSubOrganizations   = InfoStatus.ShowIdOnly,
                                    InfoStatus                      ExpandTags               = InfoStatus.ShowIdOnly,
                                    OrganizationToJSONDelegate      OrganizationToJSON       = null,
                                    Boolean                         IncludeCryptoHash        = true)


            => Organizations?.Any() != true

                   ? new JArray()

                   : new JArray(Organizations.
                                    Where     (dataSet =>  dataSet != null).
                                    OrderBy   (dataSet => dataSet.Id).
                                    SkipTakeFilter(Skip, Take).
                                    SafeSelect(organization => OrganizationToJSON != null
                                                                    ? OrganizationToJSON (organization,
                                                                                          Embedded,
                                                                                          ExpandMembers,
                                                                                          ExpandParents,
                                                                                          ExpandSubOrganizations,
                                                                                          ExpandTags,
                                                                                          IncludeCryptoHash)

                                                                    : organization.ToJSON(Embedded,
                                                                                          ExpandMembers,
                                                                                          ExpandParents,
                                                                                          ExpandSubOrganizations,
                                                                                          ExpandTags,
                                                                                          IncludeCryptoHash)));

        #endregion

    }

    /// <summary>
    /// An Open Data organization.
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

        #endregion

        #region Properties

        #region API

        private UsersAPI _API;

        /// <summary>
        /// The UsersAPI of this organization.
        /// </summary>
        internal UsersAPI API
        {

            get
            {
                return _API;
            }

            set
            {

                if (_API != null)
                    throw new ArgumentException("Illegal attempt to change the API of this organization!");

                _API = value ?? throw new ArgumentException("Illegal attempt to delete the API reference of this organization!");

            }

        }

        #endregion


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
        /// The website of the organization.
        /// </summary>
        [Optional]
        public String               Website              { get; }

        /// <summary>
        /// The primary E-Mail address of the organization.
        /// </summary>
        [Optional]
        public EMailAddress         EMail                { get; }

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
        /// An collection of multi-language tags and their relevance.
        /// </summary>
        [Optional]
        public Tags                 Tags                 { get; }

        /// <summary>
        /// The privacy of this organization.
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
        /// Create a new Open Data organization.
        /// </summary>
        /// <param name="Id">The unique identification of the organization.</param>
        /// <param name="Name">The offical (multi-language) name of the organization.</param>
        /// <param name="Description">An optional (multi-language) description of the organization.</param>
        /// <param name="Website">The website of the organization.</param>
        /// <param name="EMail">The primary e-mail of the organisation.</param>
        /// <param name="Telephone">An optional telephone number of the organisation.</param>
        /// <param name="GeoLocation">An optional geographical location of the organisation.</param>
        /// <param name="Address">An optional address of the organisation.</param>
        /// <param name="PrivacyLevel">Whether the organization will be shown in organization listings, or not.</param>
        /// <param name="IsDisabled">The organization is disabled.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        public Organization(Organization_Id                                                                    Id,
                            I18NString                                                                         Name                                = null,
                            I18NString                                                                         Description                         = null,
                            String                                                                             Website                             = null,
                            EMailAddress                                                                       EMail                               = null,
                            PhoneNumber?                                                                       Telephone                           = null,
                            GeoCoordinate?                                                                     GeoLocation                         = null,
                            Address                                                                            Address                             = null,
                            Func<Tags.Builder, Tags>                                                           Tags                                = null,
                            PrivacyLevel                                                                       PrivacyLevel                        = OpenData.PrivacyLevel.World,
                            Boolean                                                                            IsDisabled                          = false,
                            String                                                                             DataSource                          = "",

                            IEnumerable<MiniEdge<User, User2OrganizationEdges, Organization>>                  User2OrganizationInEdges            = null,
                            IEnumerable<MiniEdge<Organization, Organization2OrganizationEdges, Organization>>  Organization2OrganizationInEdges    = null,
                            IEnumerable<MiniEdge<Organization, Organization2OrganizationEdges, Organization>>  Organization2OrganizationOutEdges   = null)

            : base(Id,
                   DataSource)

        {

            this.Name           = Name         ?? new I18NString();
            this.Description    = Description  ?? new I18NString();
            this.Website        = Website;
            this.EMail          = EMail;
            this.Address        = Address;
            this.Telephone      = Telephone;
            this.GeoLocation    = GeoLocation;
            this.Address        = Address;
            var _TagsBuilder = new Tags.Builder();
            this.Tags           = Tags != null ? Tags(_TagsBuilder) : _TagsBuilder;
            this.PrivacyLevel   = PrivacyLevel;
            this.IsDisabled     = IsDisabled;

            // Init edges
            this._User2Organization_InEdges           = User2OrganizationInEdges.         IsNeitherNullNorEmpty() ? new List<MiniEdge<User, User2OrganizationEdges, Organization>>                (User2OrganizationInEdges)          : new List<MiniEdge<User, User2OrganizationEdges, Organization>>();
            this._Organization2Organization_InEdges   = Organization2OrganizationInEdges. IsNeitherNullNorEmpty() ? new List<MiniEdge<Organization, Organization2OrganizationEdges, Organization>>(Organization2OrganizationInEdges)  : new List<MiniEdge<Organization, Organization2OrganizationEdges, Organization>>();
            this._Organization2Organization_OutEdges  = Organization2OrganizationOutEdges.IsNeitherNullNorEmpty() ? new List<MiniEdge<Organization, Organization2OrganizationEdges, Organization>>(Organization2OrganizationOutEdges) : new List<MiniEdge<Organization, Organization2OrganizationEdges, Organization>>();

            CalcHash();

        }

        #endregion


        #region User          -> Organization edges

        protected readonly List<MiniEdge<User, User2OrganizationEdges, Organization>> _User2Organization_InEdges;

        public IEnumerable<MiniEdge<User, User2OrganizationEdges, Organization>> User2OrganizationEdges
            => _User2Organization_InEdges;


        #region LinkUser(Edge)

        public MiniEdge<User, User2OrganizationEdges, Organization>

            LinkUser(MiniEdge<User, User2OrganizationEdges, Organization> Edge)

            => _User2Organization_InEdges.AddAndReturnElement(Edge);

        #endregion

        #region LinkUser(Source, EdgeLabel, PrivacyLevel = PrivacyLevel.World)

        public MiniEdge<User, User2OrganizationEdges, Organization>

            LinkUser(User                    Source,
                     User2OrganizationEdges  EdgeLabel,
                     PrivacyLevel            PrivacyLevel = PrivacyLevel.World)

            => _User2Organization_InEdges.
                   AddAndReturnElement(new MiniEdge<User, User2OrganizationEdges, Organization>(Source,
                                                                                                EdgeLabel,
                                                                                                this,
                                                                                                PrivacyLevel));

        #endregion


        #region User2OrganizationInEdges     (User)

        /// <summary>
        /// The edge labels of all (incoming) edges between the given user and this organization.
        /// </summary>
        public IEnumerable<MiniEdge<User, User2OrganizationEdges, Organization>> User2OrganizationInEdges(User User)

            => _User2Organization_InEdges.
                   Where(edge => edge.Source == User);

        #endregion

        #region User2OrganizationInEdgeLabels(User)

        /// <summary>
        /// The edge labels of all (incoming) edges between the given user and this organization.
        /// </summary>
        public IEnumerable<User2OrganizationEdges> User2OrganizationInEdgeLabels(User User)

            => _User2Organization_InEdges.
                   Where (edge => edge.Source == User).
                   Select(edge => edge.EdgeLabel);

        #endregion

        public IEnumerable<MiniEdge<User, User2OrganizationEdges, Organization>>

            Add(IEnumerable<MiniEdge<User, User2OrganizationEdges, Organization>> Edges)

                => _User2Organization_InEdges.AddAndReturnList(Edges);


        #region UnlinkUser(EdgeLabel, User)

        public void UnlinkUser(User2OrganizationEdges  EdgeLabel,
                               User                    User)
        {

            var edges = _User2Organization_InEdges.
                            Where(edge => edge.EdgeLabel == EdgeLabel &&
                                          edge.Source    == User).
                            ToArray();

            foreach (var edge in edges)
                _User2Organization_InEdges.Remove(edge);

        }

        #endregion

        #endregion

        #region Organization <-> Organization edges

        protected readonly List<MiniEdge<Organization, Organization2OrganizationEdges, Organization>> _Organization2Organization_InEdges;

        public IEnumerable<MiniEdge<Organization, Organization2OrganizationEdges, Organization>> Organization2OrganizationInEdges
            => _Organization2Organization_InEdges;

        #region AddInEdge (Edge)

        public MiniEdge<Organization, Organization2OrganizationEdges, Organization>

            AddInEdge(MiniEdge<Organization, Organization2OrganizationEdges, Organization> Edge)

            => _Organization2Organization_InEdges.AddAndReturnElement(Edge);

        #endregion

        #region AddInEdge (EdgeLabel, SourceOrganization, PrivacyLevel = PrivacyLevel.World)

        public MiniEdge<Organization, Organization2OrganizationEdges, Organization>

            AddInEdge (Organization2OrganizationEdges  EdgeLabel,
                       Organization                    SourceOrganization,
                       PrivacyLevel                    PrivacyLevel = PrivacyLevel.World)

            => _Organization2Organization_InEdges. AddAndReturnElement(new MiniEdge<Organization, Organization2OrganizationEdges, Organization>(SourceOrganization,
                                                                                                                                                EdgeLabel,
                                                                                                                                                this,
                                                                                                                                                PrivacyLevel));

        #endregion

        public IEnumerable<MiniEdge<Organization, Organization2OrganizationEdges, Organization>>

            AddInEdges(IEnumerable<MiniEdge<Organization, Organization2OrganizationEdges, Organization>> Edges)

                => _Organization2Organization_InEdges.AddAndReturnList(Edges);

        #region RemoveInEdges (EdgeLabel, SourceOrganization)

        public void RemoveInEdges(Organization2OrganizationEdges EdgeLabel,
                                  Organization SourceOrganization)
        {

            var edges = _Organization2Organization_OutEdges.
                            Where(edge => edge.EdgeLabel == EdgeLabel &&
                                          edge.Source == SourceOrganization).
                            ToArray();

            foreach (var edge in edges)
                _Organization2Organization_InEdges.Remove(edge);

        }

        #endregion



        protected readonly List<MiniEdge<Organization, Organization2OrganizationEdges, Organization>> _Organization2Organization_OutEdges;

        public IEnumerable<MiniEdge<Organization, Organization2OrganizationEdges, Organization>> Organization2OrganizationOutEdges
            => _Organization2Organization_OutEdges;

        #region AddOutEdge(Edge)

        public MiniEdge<Organization, Organization2OrganizationEdges, Organization>

            AddOutEdge(MiniEdge<Organization, Organization2OrganizationEdges, Organization> Edge)

            => _Organization2Organization_OutEdges.AddAndReturnElement(Edge);

        #endregion

        #region AddOutEdge(EdgeLabel, TargetOrganization, PrivacyLevel = PrivacyLevel.World)

        public MiniEdge<Organization, Organization2OrganizationEdges, Organization>

            AddOutEdge(Organization2OrganizationEdges  EdgeLabel,
                       Organization                    TargetOrganization,
                       PrivacyLevel                    PrivacyLevel = PrivacyLevel.World)

            => _Organization2Organization_OutEdges.AddAndReturnElement(new MiniEdge<Organization, Organization2OrganizationEdges, Organization>(this,
                                                                                                                                                EdgeLabel,
                                                                                                                                                TargetOrganization,
                                                                                                                                                PrivacyLevel));

        #endregion

        public IEnumerable<MiniEdge<Organization, Organization2OrganizationEdges, Organization>>

            AddOutEdges(IEnumerable<MiniEdge<Organization, Organization2OrganizationEdges, Organization>> Edges)

                => _Organization2Organization_OutEdges.AddAndReturnList(Edges);

        #region RemoveOutEdges(EdgeLabel, TargetOrganization)

        public void RemoveOutEdges(Organization2OrganizationEdges  EdgeLabel,
                                   Organization                    TargetOrganization)
        {

            var edges = _Organization2Organization_OutEdges.
                            Where(edge => edge.EdgeLabel == EdgeLabel &&
                                          edge.Target    == TargetOrganization).
                            ToArray();

            foreach (var edge in edges)
                _Organization2Organization_OutEdges.Remove(edge);

        }

        #endregion

        #endregion


        #region ToJSON(Embedded = false, IncludeCryptoHash = false)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        /// <param name="IncludeCryptoHash">Include the crypto hash value of this object.</param>
        public override JObject ToJSON(Boolean  Embedded           = false,
                                       Boolean  IncludeCryptoHash  = false)

            => ToJSON(Embedded:            false,
                      ExpandParents:       InfoStatus.ShowIdOnly,
                      ExpandSubOrganizations:        InfoStatus.ShowIdOnly,
                      ExpandTags:          InfoStatus.ShowIdOnly,
                      IncludeCryptoHash:   true);


        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        /// <param name="IncludeCryptoHash">Include the crypto hash value of this object.</param>
        public JObject ToJSON(Boolean     Embedded                = false,
                              InfoStatus  ExpandMembers           = InfoStatus.ShowIdOnly,
                              InfoStatus  ExpandParents           = InfoStatus.ShowIdOnly,
                              InfoStatus  ExpandSubOrganizations  = InfoStatus.ShowIdOnly,
                              InfoStatus  ExpandTags              = InfoStatus.ShowIdOnly,
                              Boolean     IncludeCryptoHash       = true)

            => JSONObject.Create(

                   new JProperty("@id",                 Id.             ToString()),

                   !Embedded
                       ? new JProperty("@context",      JSONLDContext)
                       : null,

                   new JProperty("name",                Name.           ToJSON()),

                   Description.IsNeitherNullNorEmpty()
                       ? new JProperty("description",   Description.    ToJSON())
                       : null,

                   EMail != null
                       ? new JProperty("email",         EMail.          ToString())
                       : null,

                   // PublicKeyRing

                   Telephone.HasValue
                       ? new JProperty("telephone",     Telephone.Value.ToString())
                       : null,

                   GeoLocation?.ToJSON("geoLocation"),
                   Address?.    ToJSON("address"),

                   Tags.Any()
                       ? new JProperty("tags", Tags.ToJSON(ExpandTags))
                       : null,

                   PrivacyLevel.ToJSON(),


                   new JProperty("parents",             Organization2OrganizationOutEdges.
                                                            Where     (edge => edge.EdgeLabel == Organization2OrganizationEdges.IsChildOf).
                                                            SafeSelect(edge => ExpandParents.Switch(edge,
                                                                                                    _edge => _edge.Target.Id.ToString(),
                                                                                                    _edge => _edge.Target.ToJSON()))),

                   Organization2OrganizationInEdges.SafeAny(edge => edge.EdgeLabel == Organization2OrganizationEdges.IsChildOf)
                       ? new JProperty("childs",        Organization2OrganizationInEdges.
                                                            Where     (edge => edge.EdgeLabel == Organization2OrganizationEdges.IsChildOf).
                                                            SafeSelect(edge => ExpandSubOrganizations.Switch(edge,
                                                                                                   _edge => _edge.Source.Id.ToString(),
                                                                                                   _edge => _edge.Source.ToJSON())))
                       : null,

                   Admins.SafeAny()
                       ? new JProperty("admins",        Admins.
                                                            SafeSelect(user => ExpandMembers.Switch(user,
                                                                                                   _user => _user.Id.ToString(),
                                                                                                   _user => _user.ToJSON())))
                       : null,

                   Members.SafeAny()
                       ? new JProperty("members",       Members.
                                                            SafeSelect(user => ExpandMembers.Switch(user,
                                                                                                   _user => _user.Id.ToString(),
                                                                                                   _user => _user.ToJSON())))
                       : null,


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
                if (JSONObject.ParseOptionalStruct("@id",
                                                   "organization identification",
                                                   Organization_Id.TryParse,
                                                   out Organization_Id? OrganizationIdBody,
                                                   out ErrorResponse))
                {

                    if (ErrorResponse != null)
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

                if (JSONObject.ParseOptional("description",
                                             "description",
                                             out I18NString Description,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Website          [optional]

                if (JSONObject.ParseOptional("website",
                                             out String Website,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse E-Mail           [optional]

                if (JSONObject.ParseOptionalStruct("email",
                                                   "e-mail address",
                                                   SimpleEMailAddress.TryParse,
                                                   out SimpleEMailAddress? EMail,
                                                   out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Telephone        [optional]

                if (JSONObject.ParseOptional("telephone",
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

                if (JSONObject.ParseOptionalStruct("geoLocation",
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
                                             Vanaheimr.Illias.Address.TryParse,
                                             out Address Address,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                var Tags = new Tags();

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
                                                Website,
                                                EMail,
                                                Telephone,
                                                GeoLocation,
                                                Address,
                                                _ => Tags,
                                                PrivacyLevel ?? OpenData.PrivacyLevel.World,
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


        #region CopyAllEdgesTo(NewOrganization)

        public void CopyAllEdgesTo(Organization NewOrganization)
        {

            if (_User2Organization_InEdges.Any() && !NewOrganization._User2Organization_InEdges.Any())
            {

                NewOrganization.Add(_User2Organization_InEdges);

                foreach (var edge in NewOrganization._User2Organization_InEdges)
                    edge.Target = NewOrganization;

            }

            if (_Organization2Organization_InEdges.Any() && !NewOrganization._Organization2Organization_InEdges.Any())
            {

                NewOrganization.AddInEdges(_Organization2Organization_InEdges);

                foreach (var edge in NewOrganization._Organization2Organization_InEdges)
                    edge.Target = NewOrganization;

            }

            if (_Organization2Organization_OutEdges.Any() && !NewOrganization._Organization2Organization_OutEdges.Any())
            {

                NewOrganization.AddOutEdges(_Organization2Organization_OutEdges);

                foreach (var edge in NewOrganization._Organization2Organization_OutEdges)
                    edge.Source = NewOrganization;

            }

        }

        #endregion


        #region (private) _GetAllParents(ref Parents)

        private void _GetAllParents(ref HashSet<Organization> Parents)
        {

            var parents = _Organization2Organization_OutEdges.Where(edge => edge.Source == this && edge.EdgeLabel == Organization2OrganizationEdges.IsChildOf).Select(edge => edge.Target).ToArray();

            foreach (var parent in parents)
            {
                // Detect loops!
                if (Parents.Add(parent))
                    parent._GetAllParents(ref Parents);
            }

        }

        #endregion

        #region GetAllParents(Filter = null)

        public IEnumerable<Organization> GetAllParents(Func<Organization, Boolean> Include = null)
        {

            var parents = new HashSet<Organization>();
            _GetAllParents(ref parents);

            return Include != null
                       ? parents.Where(Include)
                       : parents;

        }

        #endregion


        public IEnumerable<User> Admins
            => _User2Organization_InEdges.Where(_ => _.EdgeLabel == Users.User2OrganizationEdges.IsAdmin). SafeSelect(edge => edge.Source).ToArray();

        public IEnumerable<User> Members
            => _User2Organization_InEdges.Where(_ => _.EdgeLabel == Users.User2OrganizationEdges.IsMember).SafeSelect(edge => edge.Source).ToArray();

        public IEnumerable<Organization> Parents
            => _Organization2Organization_OutEdges.Where(edge => edge.Source == this && edge.EdgeLabel == Organization2OrganizationEdges.IsChildOf).Select(edge => edge.Target).ToArray();

        /// <summary>
        /// A relationship between two organizations where the first includes the second, e.g., as a subsidiary. See also: the more specific 'department' property.
        /// </summary>
        public IEnumerable<Organization> SubOrganizations
            => _Organization2Organization_InEdges.Where(edge => edge.Source == this && edge.EdgeLabel == Organization2OrganizationEdges.IsChildOf).Select(edge => edge.Source).ToArray();


        #region Operator overloading

        #region Operator == (OrganizationId1, OrganizationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OrganizationId1">A organization identification.</param>
        /// <param name="OrganizationId2">Another organization identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Organization OrganizationId1, Organization OrganizationId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(OrganizationId1, OrganizationId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) OrganizationId1 == null) || ((Object) OrganizationId2 == null))
                return false;

            return OrganizationId1.Equals(OrganizationId2);

        }

        #endregion

        #region Operator != (OrganizationId1, OrganizationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OrganizationId1">A organization identification.</param>
        /// <param name="OrganizationId2">Another organization identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Organization OrganizationId1, Organization OrganizationId2)
            => !(OrganizationId1 == OrganizationId2);

        #endregion

        #region Operator <  (OrganizationId1, OrganizationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OrganizationId1">A organization identification.</param>
        /// <param name="OrganizationId2">Another organization identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Organization OrganizationId1, Organization OrganizationId2)
        {

            if ((Object) OrganizationId1 == null)
                throw new ArgumentNullException(nameof(OrganizationId1), "The given OrganizationId1 must not be null!");

            return OrganizationId1.CompareTo(OrganizationId2) < 0;

        }

        #endregion

        #region Operator <= (OrganizationId1, OrganizationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OrganizationId1">A organization identification.</param>
        /// <param name="OrganizationId2">Another organization identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Organization OrganizationId1, Organization OrganizationId2)
            => !(OrganizationId1 > OrganizationId2);

        #endregion

        #region Operator >  (OrganizationId1, OrganizationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OrganizationId1">A organization identification.</param>
        /// <param name="OrganizationId2">Another organization identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Organization OrganizationId1, Organization OrganizationId2)
        {

            if ((Object) OrganizationId1 == null)
                throw new ArgumentNullException(nameof(OrganizationId1), "The given OrganizationId1 must not be null!");

            return OrganizationId1.CompareTo(OrganizationId2) > 0;

        }

        #endregion

        #region Operator >= (OrganizationId1, OrganizationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OrganizationId1">A organization identification.</param>
        /// <param name="OrganizationId2">Another organization identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Organization OrganizationId1, Organization OrganizationId2)
            => !(OrganizationId1 < OrganizationId2);

        #endregion

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

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
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
                           Description,
                           Website,
                           EMail,
                           Telephone,
                           GeoLocation,
                           Address,
                           _ => Tags,
                           PrivacyLevel,
                           IsDisabled,
                           DataSource,

                           _User2Organization_InEdges,
                           _Organization2Organization_InEdges,
                           _Organization2Organization_OutEdges);

        #endregion

        #region (class) Builder

        /// <summary>
        /// An Open Data organization builder.
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
            /// The website of the organization.
            /// </summary>
            [Optional]
            public String               Website              { get; set; }

            /// <summary>
            /// The primary E-Mail address of the organization.
            /// </summary>
            [Optional]
            public EMailAddress         EMail                { get; set; }

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
            /// An collection of multi-language tags and their relevance.
            /// </summary>
            [Optional]
            public Tags                 Tags                 { get; set; }

            /// <summary>
            /// The privacy of this organization.
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

            #region Edges

            protected readonly List<MiniEdge<User, User2OrganizationEdges, Organization>> _User2Organization_InEdges;

            public IEnumerable<MiniEdge<User, User2OrganizationEdges, Organization>> User2OrganizationEdges
                => _User2Organization_InEdges;


            protected readonly List<MiniEdge<Organization, Organization2UserEdges, User>> _Organization2UserEdges;

            public IEnumerable<MiniEdge<Organization, Organization2UserEdges, User>> Organization2UserEdges
                => _Organization2UserEdges;


            protected readonly List<MiniEdge<Organization, Organization2OrganizationEdges, Organization>> _Organization2Organization_InEdges;

            public IEnumerable<MiniEdge<Organization, Organization2OrganizationEdges, Organization>> Organization2OrganizationInEdges
                => _Organization2Organization_InEdges;


            protected readonly List<MiniEdge<Organization, Organization2OrganizationEdges, Organization>> _Organization2Organization_OutEdges;

            public IEnumerable<MiniEdge<Organization, Organization2OrganizationEdges, Organization>> Organization2OrganizationOutEdges
                => _Organization2Organization_OutEdges;

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new organization builder.
            /// </summary>
            /// <param name="Id">The unique identification of the organization.</param>
            /// <param name="Name">The offical (multi-language) name of the organization.</param>
            /// <param name="Description">An optional (multi-language) description of the organization.</param>
            /// <param name="Website">The website of the organization.</param>
            /// <param name="EMail">The primary e-mail of the organisation.</param>
            /// <param name="Telephone">An optional telephone number of the organisation.</param>
            /// <param name="GeoLocation">An optional geographical location of the organisation.</param>
            /// <param name="Address">An optional address of the organisation.</param>
            /// <param name="PrivacyLevel">Whether the organization will be shown in organization listings, or not.</param>
            /// <param name="IsDisabled">The organization is disabled.</param>
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            public Builder(Organization_Id                                                                    Id,
                           I18NString                                                                         Name                                = null,
                           I18NString                                                                         Description                         = null,
                           String                                                                             Website                             = null,
                           EMailAddress                                                                       EMail                               = null,
                           PhoneNumber?                                                                       Telephone                           = null,
                           GeoCoordinate?                                                                     GeoLocation                         = null,
                           Address                                                                            Address                             = null,
                           Func<Tags.Builder, Tags>                                                           Tags                                = null,
                           PrivacyLevel                                                                       PrivacyLevel                        = OpenData.PrivacyLevel.Private,
                           Boolean                                                                            IsDisabled                          = false,
                           String                                                                             DataSource                          = "",

                           IEnumerable<MiniEdge<User, User2OrganizationEdges, Organization>>                  User2OrganizationInEdges            = null,
                           IEnumerable<MiniEdge<Organization, Organization2OrganizationEdges, Organization>>  Organization2OrganizationInEdges    = null,
                           IEnumerable<MiniEdge<Organization, Organization2OrganizationEdges, Organization>>  Organization2OrganizationOutEdges   = null)
            {

                this.Id              = Id;
                this.Name            = Name        ?? new I18NString();
                this.Description     = Description ?? new I18NString();
                this.Website         = Website;
                this.EMail           = EMail;
                this.Address         = Address;
                this.Telephone       = Telephone;
                this.GeoLocation     = GeoLocation;
                this.Address         = Address;
                var _TagsBuilder = new Tags.Builder();
                this.Tags            = Tags != null ? Tags(_TagsBuilder) : _TagsBuilder;
                this.PrivacyLevel    = PrivacyLevel;
                this.IsDisabled      = IsDisabled;
                this.DataSource      = DataSource;

                // Init edges
                this._User2Organization_InEdges           = User2OrganizationInEdges.           IsNeitherNullNorEmpty() ? new List<MiniEdge<User, User2OrganizationEdges, Organization>>                (User2OrganizationInEdges)            : new List<MiniEdge<User, User2OrganizationEdges, Organization>>();
                this._Organization2Organization_InEdges   = Organization2OrganizationInEdges. IsNeitherNullNorEmpty() ? new List<MiniEdge<Organization, Organization2OrganizationEdges, Organization>>(Organization2OrganizationInEdges)  : new List<MiniEdge<Organization, Organization2OrganizationEdges, Organization>>();
                this._Organization2Organization_OutEdges  = Organization2OrganizationOutEdges.IsNeitherNullNorEmpty() ? new List<MiniEdge<Organization, Organization2OrganizationEdges, Organization>>(Organization2OrganizationOutEdges) : new List<MiniEdge<Organization, Organization2OrganizationEdges, Organization>>();

            }

            #endregion


            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the organization.
            /// </summary>
            /// <param name="Builder">An organization builder.</param>
            public static implicit operator Organization(Builder Builder)

                => Builder.ToImmutable;


            /// <summary>
            /// Return an immutable version of the organization.
            /// </summary>
            public Organization ToImmutable

                => new Organization(Id,
                                    Name,
                                    Description,
                                    Website,
                                    EMail,
                                    Telephone,
                                    GeoLocation,
                                    Address,
                                    _ => Tags,
                                    PrivacyLevel,
                                    IsDisabled,
                                    DataSource,

                                    _User2Organization_InEdges,
                                    _Organization2Organization_InEdges,
                                    _Organization2Organization_OutEdges);

            #endregion

        }

        #endregion

    }

}
