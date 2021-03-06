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
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Styx.Arrows;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Mail;

using social.OpenData.UsersAPI.Notifications;
using social.OpenData.UsersAPI;
using org.GraphDefined.Vanaheimr.Hermod.JSON;

#endregion

namespace social.OpenData.UsersAPI
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
    /// An organization.
    /// </summary>
    public class Organization : AEntity<Organization_Id,
                                        Organization>
    {

        #region Data

        /// <summary>
        /// The default max size of the aggregated user organizations status history.
        /// </summary>
        public const UInt16 DefaultOrganizationStatusHistorySize = 50;

        /// <summary>
        /// The default JSON-LD context of organizations.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://opendata.social/contexts/UsersAPI/organization");

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

                if (_API == value)
                    return;

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
        public I18NString                 Name                 { get; }

        /// <summary>
        /// The optional (multi-language) description of the organization.
        /// </summary>
        [Optional]
        public I18NString                 Description          { get; }

        /// <summary>
        /// The website of the organization.
        /// </summary>
        [Optional]
        public String                     Website              { get; }

        /// <summary>
        /// The primary E-Mail address of the organization.
        /// </summary>
        [Optional]
        public EMailAddress               EMail                { get; }

        /// <summary>
        /// The telephone number of the organization.
        /// </summary>
        [Optional]
        public PhoneNumber?               Telephone            { get; }

        /// <summary>
        /// The optional address of the organization.
        /// </summary>
        [Optional]
        public Address                    Address              { get; }

        /// <summary>
        /// The geographical location of this organization.
        /// </summary>
        public GeoCoordinate?             GeoLocation          { get; }

        /// <summary>
        /// An collection of multi-language tags and their relevance.
        /// </summary>
        [Optional]
        public Tags                       Tags                 { get; }

        /// <summary>
        /// The user will be shown in organization listings.
        /// </summary>
        [Mandatory]
        public Boolean                    IsDisabled           { get; }

        /// <summary>
        /// An enumeration of attached files.
        /// </summary>
        [Optional]
        public IEnumerable<AttachedFile>  AttachedFiles        { get; }

        #endregion

        #region Edges

        #region User          -> Organization edges

        protected readonly List<User2OrganizationEdge> _User2Organization_InEdges;

        public IEnumerable<User2OrganizationEdge> User2OrganizationEdges
            => _User2Organization_InEdges;


        #region LinkUser(Edge)

        public User2OrganizationEdge

            LinkUser(User2OrganizationEdge Edge)

            => _User2Organization_InEdges.AddAndReturnElement(Edge);

        #endregion

        #region LinkUser(Source, EdgeLabel, PrivacyLevel = PrivacyLevel.World)

        public User2OrganizationEdge

            LinkUser(User                    Source,
                     User2OrganizationEdgeTypes  EdgeLabel,
                     PrivacyLevel            PrivacyLevel = PrivacyLevel.World)

            => _User2Organization_InEdges.
                   AddAndReturnElement(new User2OrganizationEdge(Source,
                                                                                                EdgeLabel,
                                                                                                this,
                                                                                                PrivacyLevel));

        #endregion


        #region User2OrganizationInEdges     (User)

        /// <summary>
        /// The edge labels of all (incoming) edges between the given user and this organization.
        /// </summary>
        public IEnumerable<User2OrganizationEdge> User2OrganizationInEdges(User User)

            => _User2Organization_InEdges.
                   Where(edge => edge.Source == User);

        #endregion

        #region User2OrganizationInEdgeLabels(User)

        /// <summary>
        /// The edge labels of all (incoming) edges between the given user and this organization.
        /// </summary>
        public IEnumerable<User2OrganizationEdgeTypes> User2OrganizationInEdgeLabels(User User)

            => _User2Organization_InEdges.
                   Where (edge => edge.Source == User).
                   Select(edge => edge.EdgeLabel);

        #endregion

        public IEnumerable<User2OrganizationEdge>

            Add(IEnumerable<User2OrganizationEdge> Edges)

                => _User2Organization_InEdges.AddAndReturnList(Edges);


        #region UnlinkUser(EdgeLabel, User)

        public void UnlinkUser(User2OrganizationEdgeTypes  EdgeLabel,
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

        public Boolean RemoveInEdge(User2OrganizationEdge Edge)
            => _User2Organization_InEdges.Remove(Edge);

        #endregion

        #region Organization <-> Organization edges

        protected readonly List<Organization2OrganizationEdge> _Organization2Organization_InEdges;

        public IEnumerable<Organization2OrganizationEdge> Organization2OrganizationInEdges
            => _Organization2Organization_InEdges;

        #region AddInEdge (Edge)

        public Organization2OrganizationEdge

            AddInEdge(Organization2OrganizationEdge Edge)

            => _Organization2Organization_InEdges.AddAndReturnElement(Edge);

        #endregion

        #region AddInEdge (EdgeLabel, SourceOrganization, PrivacyLevel = PrivacyLevel.World)

        public Organization2OrganizationEdge

            AddInEdge (Organization2OrganizationEdgeTypes  EdgeLabel,
                       Organization                    SourceOrganization,
                       PrivacyLevel                    PrivacyLevel = PrivacyLevel.World)

            => _Organization2Organization_InEdges. AddAndReturnElement(new Organization2OrganizationEdge(SourceOrganization,
                                                                                                                                                EdgeLabel,
                                                                                                                                                this,
                                                                                                                                                PrivacyLevel));

        #endregion

        public IEnumerable<Organization2OrganizationEdge>

            AddInEdges(IEnumerable<Organization2OrganizationEdge> Edges)

                => _Organization2Organization_InEdges.AddAndReturnList(Edges);

        #region RemoveInEdges(EdgeLabel, TargetOrganization)

        public Boolean RemoveInEdge(Organization2OrganizationEdge Edge)
            => _Organization2Organization_InEdges.Remove(Edge);

        #endregion

        #region RemoveInEdges (EdgeLabel, SourceOrganization)

        public void RemoveInEdges(Organization2OrganizationEdgeTypes EdgeLabel,
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



        protected readonly List<Organization2OrganizationEdge> _Organization2Organization_OutEdges;

        public IEnumerable<Organization2OrganizationEdge> Organization2OrganizationOutEdges
            => _Organization2Organization_OutEdges;

        #region AddOutEdge(Edge)

        public Organization2OrganizationEdge

            AddOutEdge(Organization2OrganizationEdge Edge)

            => _Organization2Organization_OutEdges.AddAndReturnElement(Edge);

        #endregion

        #region AddOutEdge(EdgeLabel, TargetOrganization, PrivacyLevel = PrivacyLevel.World)

        public Organization2OrganizationEdge

            AddOutEdge(Organization2OrganizationEdgeTypes  EdgeLabel,
                       Organization                    TargetOrganization,
                       PrivacyLevel                    PrivacyLevel = PrivacyLevel.World)

            => _Organization2Organization_OutEdges.AddAndReturnElement(new Organization2OrganizationEdge(this,
                                                                                                                                                EdgeLabel,
                                                                                                                                                TargetOrganization,
                                                                                                                                                PrivacyLevel));

        #endregion

        public IEnumerable<Organization2OrganizationEdge>

            AddOutEdges(IEnumerable<Organization2OrganizationEdge> Edges)

                => _Organization2Organization_OutEdges.AddAndReturnList(Edges);

        #region RemoveOutEdges(EdgeLabel, TargetOrganization)

        public Boolean RemoveOutEdge(Organization2OrganizationEdge Edge)
            => _Organization2Organization_OutEdges.Remove(Edge);

        #endregion

        #region RemoveOutEdges(EdgeLabel, TargetOrganization)

        public void RemoveOutEdges(Organization2OrganizationEdgeTypes  EdgeLabel,
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

        #endregion

        #region Events

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new organization.
        /// </summary>
        /// <param name="Id">The unique identification of the organization.</param>
        /// 
        /// <param name="Name">The offical (multi-language) name of the organization.</param>
        /// <param name="Description">An optional (multi-language) description of the organization.</param>
        /// <param name="Website">The website of the organization.</param>
        /// <param name="EMail">The primary e-mail of the organisation.</param>
        /// <param name="Telephone">An optional telephone number of the organisation.</param>
        /// <param name="Address">An optional address of the organisation.</param>
        /// <param name="GeoLocation">An optional geographical location of the organisation.</param>
        /// <param name="IsDisabled">The organization is disabled.</param>
        /// 
        /// <param name="CustomData">Custom data to be stored with this organization.</param>
        /// <param name="AttachedFiles">Optional files attached to this organization.</param>
        /// <param name="JSONLDContext">The JSON-LD context of this organization.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        /// <param name="LastChange">The timestamp of the last changes within this organization. Can e.g. be used as a HTTP ETag.</param>
        public Organization(Organization_Id                             Id,
                            I18NString                                  Name                                = null,
                            I18NString                                  Description                         = null,
                            String                                      Website                             = null,
                            EMailAddress                                EMail                               = null,
                            PhoneNumber?                                Telephone                           = null,
                            Address                                     Address                             = null,
                            GeoCoordinate?                              GeoLocation                         = null,
                            Func<Tags.Builder, Tags>                    Tags                                = null,
                            Boolean                                     IsDisabled                          = false,

                            IEnumerable<ANotification>                  Notifications                       = null,

                            IEnumerable<User2OrganizationEdge>          User2OrganizationInEdges            = null,
                            IEnumerable<Organization2OrganizationEdge>  Organization2OrganizationInEdges    = null,
                            IEnumerable<Organization2OrganizationEdge>  Organization2OrganizationOutEdges   = null,

                            JObject                                     CustomData                          = default,
                            IEnumerable<AttachedFile>                   AttachedFiles                       = default,
                            JSONLDContext?                              JSONLDContext                       = default,
                            String                                      DataSource                          = default,
                            DateTime?                                   LastChange                          = default)

            : base(Id,
                   JSONLDContext ?? DefaultJSONLDContext,
                   CustomData,
                   DataSource,
                   LastChange)

        {

            this.Name                                 = Name         ?? new I18NString();
            this.Description                          = Description  ?? new I18NString();
            this.Website                              = Website;
            this.EMail                                = EMail;
            this.Telephone                            = Telephone;
            this.Address                              = Address;
            this.GeoLocation                          = GeoLocation;
            var _TagsBuilder                          = new Tags.Builder();
            this.Tags                                 = Tags != null ? Tags(_TagsBuilder) : _TagsBuilder;
            this.IsDisabled                           = IsDisabled;
            this.AttachedFiles                        = AttachedFiles ?? new AttachedFile[0];

            this._Notifications                       = new NotificationStore();

            if (Notifications.SafeAny())
                _Notifications.Add(Notifications);

            // Init edges
            this._User2Organization_InEdges           = User2OrganizationInEdges.         IsNeitherNullNorEmpty() ? new List<User2OrganizationEdge>                (User2OrganizationInEdges)          : new List<User2OrganizationEdge>();
            this._Organization2Organization_InEdges   = Organization2OrganizationInEdges. IsNeitherNullNorEmpty() ? new List<Organization2OrganizationEdge>(Organization2OrganizationInEdges)  : new List<Organization2OrganizationEdge>();
            this._Organization2Organization_OutEdges  = Organization2OrganizationOutEdges.IsNeitherNullNorEmpty() ? new List<Organization2OrganizationEdge>(Organization2OrganizationOutEdges) : new List<Organization2OrganizationEdge>();

            CalcHash();

        }

        #endregion


        #region Notifications

        private readonly NotificationStore _Notifications;

        #region (internal) AddNotification(Notification,                           OnUpdate = null)

        internal T AddNotification<T>(T          Notification,
                                      Action<T>  OnUpdate  = null)

            where T : ANotification

            => _Notifications.Add(Notification,
                                  OnUpdate);

        #endregion

        #region (internal) AddNotification(Notification, NotificationMessageType,  OnUpdate = null)

        internal T AddNotification<T>(T                        Notification,
                                      NotificationMessageType  NotificationMessageType,
                                      Action<T>                OnUpdate  = null)

            where T : ANotification

            => _Notifications.Add(Notification,
                                  NotificationMessageType,
                                  OnUpdate);

        #endregion

        #region (internal) AddNotification(Notification, NotificationMessageTypes, OnUpdate = null)

        internal T AddNotification<T>(T                                     Notification,
                                      IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                      Action<T>                             OnUpdate  = null)

            where T : ANotification

            => _Notifications.Add(Notification,
                                  NotificationMessageTypes,
                                  OnUpdate);

        #endregion


        #region GetNotifications  (NotificationMessageType = null)

        public IEnumerable<ANotification> GetNotifications(NotificationMessageType?  NotificationMessageType = null)
        {
            lock (_Notifications)
            {
                return _Notifications.GetNotifications(NotificationMessageType);
            }
        }

        #endregion

        #region GetNotificationsOf(params NotificationMessageTypes)

        public IEnumerable<T> GetNotificationsOf<T>(params NotificationMessageType[] NotificationMessageTypes)

            where T : ANotification

        {

            lock (_Notifications)
            {
                return _Notifications.GetNotificationsOf<T>(NotificationMessageTypes);
            }

        }

        #endregion

        #region GetNotifications  (NotificationMessageTypeFilter)

        public IEnumerable<ANotification> GetNotifications(Func<NotificationMessageType, Boolean> NotificationMessageTypeFilter)
        {
            lock (_Notifications)
            {
                return _Notifications.GetNotifications(NotificationMessageTypeFilter);
            }
        }

        #endregion

        #region GetNotificationsOf(NotificationMessageTypeFilter)

        public IEnumerable<T> GetNotificationsOf<T>(Func<NotificationMessageType, Boolean> NotificationMessageTypeFilter)

            where T : ANotification

        {

            lock (_Notifications)
            {
                return _Notifications.GetNotificationsOf<T>(NotificationMessageTypeFilter);
            }

        }

        #endregion


        #region GetNotificationInfos()

        public JObject GetNotificationInfos()

            => JSONObject.Create(new JProperty("user", JSONObject.Create(

                                     new JProperty("name",               EMail.OwnerName),
                                     new JProperty("email",              EMail.Address.ToString())

                                     //MobilePhone.HasValue
                                     //    ? new JProperty("phoneNumber",  MobilePhone.Value.ToString())
                                     //    : null

                                 )),
                                 new JProperty("notifications",  _Notifications.ToJSON()));

        #endregion


        #region (internal) RemoveNotification(NotificationType,                           OnRemoval = null)

        internal T RemoveNotification<T>(T          NotificationType,
                                         Action<T>  OnRemoval  = null)

            where T : ANotification

            => _Notifications.Remove(NotificationType,
                                     OnRemoval);

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

            => ToJSON(Embedded:                false,
                      ExpandParents:           InfoStatus.ShowIdOnly,
                      ExpandSubOrganizations:  InfoStatus.ShowIdOnly,
                      ExpandTags:              InfoStatus.ShowIdOnly,
                      IncludeCryptoHash:       true);


        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        /// <param name="IncludeCryptoHash">Include the crypto hash value of this object.</param>
        public JObject ToJSON(Boolean                                        Embedded                       = false,
                              InfoStatus                                     ExpandMembers                  = InfoStatus.ShowIdOnly,
                              InfoStatus                                     ExpandParents                  = InfoStatus.ShowIdOnly,
                              InfoStatus                                     ExpandSubOrganizations         = InfoStatus.ShowIdOnly,
                              InfoStatus                                     ExpandTags                     = InfoStatus.ShowIdOnly,
                              Boolean                                        IncludeLastChange              = true,
                              Boolean                                        IncludeCryptoHash              = true,
                              CustomJObjectSerializerDelegate<Organization>  CustomOrganizationSerializer   = null)

        {

            var JSON = base.ToJSON(Embedded,
                                   IncludeLastChange,
                                   IncludeCryptoHash,
                                   null,
                                   new JProperty[] {

                                       new JProperty("name",                    Name.           ToJSON()),

                                       Description.IsNeitherNullNorEmpty()
                                           ? new JProperty("description",       Description.    ToJSON())
                                           : null,

                                       Website.IsNeitherNullNorEmpty()
                                           ? new JProperty("website",           Website)
                                           : null,

                                       EMail != null
                                           ? new JProperty("email",             EMail.Address.  ToString())
                                           : null,

                                       Telephone.HasValue
                                           ? new JProperty("telephone",         Telephone.Value.ToString())
                                           : null,

                                       Address?.    ToJSON("address"),
                                       GeoLocation?.ToJSON("geoLocation"),

                                       Tags.Any()
                                           ? new JProperty("tags",              Tags.ToJSON(ExpandTags))
                                           : null,


                                       new JProperty("parents",                 Organization2OrganizationOutEdges.
                                                                                    Where     (edge => edge.EdgeLabel == Organization2OrganizationEdgeTypes.IsChildOf).
                                                                                    SafeSelect(edge => ExpandParents.Switch(edge,
                                                                                                                            _edge => _edge.Target.Id.ToString(),
                                                                                                                            _edge => _edge.Target.ToJSON()))),

                                       Organization2OrganizationInEdges.SafeAny(edge => edge.EdgeLabel == Organization2OrganizationEdgeTypes.IsChildOf)
                                           ? new JProperty("subOrganizations",  Organization2OrganizationInEdges.
                                                                                    Where     (edge => edge.EdgeLabel == Organization2OrganizationEdgeTypes.IsChildOf).
                                                                                    SafeSelect(edge => ExpandSubOrganizations.Switch(edge,
                                                                                                                           _edge => _edge.Source.Id.ToString(),
                                                                                                                           _edge => _edge.Source.ToJSON())))
                                           : null,

                                       Admins.SafeAny()
                                           ? new JProperty("admins",            Admins.
                                                                                    SafeSelect(user => ExpandMembers.Switch(user,
                                                                                                                           _user => _user.Id.ToString(),
                                                                                                                           _user => _user.ToJSON())))
                                           : null,

                                       Members.SafeAny()
                                           ? new JProperty("members",           Members.
                                                                                    SafeSelect(user => ExpandMembers.Switch(user,
                                                                                                                           _user => _user.Id.ToString(),
                                                                                                                           _user => _user.ToJSON())))
                                           : null,

                                       Guests.SafeAny()
                                           ? new JProperty("guests",            Guests.
                                                                                    SafeSelect(user => ExpandMembers.Switch(user,
                                                                                                                           _user => _user.Id.ToString(),
                                                                                                                           _user => _user.ToJSON())))
                                           : null,


                                       new JProperty("isDisabled",              IsDisabled)

                                    });


            return CustomOrganizationSerializer != null
                       ? CustomOrganizationSerializer(this, JSON)
                       : JSON;

        }

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out Organization, out ErrorResponse)

        public static Boolean TryParseJSON(JObject           JSONObject,
                                           out Organization  Organization,
                                           out String        ErrorResponse,
                                           Organization_Id?  OrganizationIdURL = null)
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

                if (!OrganizationIdURL.HasValue && !OrganizationIdBody.HasValue)
                {
                    ErrorResponse = "The organization identification is missing!";
                    return false;
                }

                if (OrganizationIdURL.HasValue && OrganizationIdBody.HasValue && OrganizationIdURL.Value != OrganizationIdBody.Value)
                {
                    ErrorResponse = "The optional organization identification given within the JSON body does not match the one given in the URI!";
                    return false;
                }

                #endregion

                #region Parse Context          [mandatory]

                if (!JSONObject.ParseMandatory("@context",
                                               "JSON-LinkedData context information",
                                               JSONLDContext.TryParse,
                                               out JSONLDContext Context,
                                               out ErrorResponse))
                {
                    ErrorResponse = @"The JSON-LD ""@context"" information is missing!";
                    return false;
                }

                if (Context != DefaultJSONLDContext && Context != OrganizationInfo.DefaultJSONLDContext && Context != OrganizationInfo2.DefaultJSONLDContext)
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
                                             "website",
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

                if (JSONObject.ParseOptionalJSON("address",
                                                 "address",
                                                 org.GraphDefined.Vanaheimr.Illias.Address.TryParse,
                                                 out Address Address,
                                                 out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                var Tags = new Tags();

                #region Parse PrivacyLevel     [optional]

                if (JSONObject.ParseOptionalEnum("privacyLevel",
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


                Organization = new Organization(OrganizationIdBody ?? OrganizationIdURL.Value,

                                                Name,
                                                Description,
                                                Website,
                                                EMail,
                                                Telephone,
                                                Address,
                                                GeoLocation,
                                                _ => Tags,
                                                IsDisabled ?? false);

                                          //      CustomData,
                                          //      AttachedFiles,
                                          //      JSONLDContext,
                                          //      DataSource,
                                          //      LastChange);

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


        #region CopyAllLinkedDataFrom(OldOrganization)

        public override void CopyAllLinkedDataFrom(Organization OldOrganization)
        {

            if (OldOrganization._User2Organization_InEdges.Any() && !_User2Organization_InEdges.Any())
            {

                Add(OldOrganization._User2Organization_InEdges);

                foreach (var edge in _User2Organization_InEdges)
                    edge.Target = this;

            }

            if (OldOrganization._Organization2Organization_InEdges.Any() && !_Organization2Organization_InEdges.Any())
            {

                AddInEdges(OldOrganization._Organization2Organization_InEdges);

                foreach (var edge in _Organization2Organization_InEdges)
                    edge.Target = this;

            }

            if (OldOrganization._Organization2Organization_OutEdges.Any() && !_Organization2Organization_OutEdges.Any())
            {

                AddOutEdges(OldOrganization._Organization2Organization_OutEdges);

                foreach (var edge in _Organization2Organization_OutEdges)
                    edge.Source = this;

            }

            if (OldOrganization._Notifications.SafeAny() && !_Notifications.SafeAny())
                _Notifications.Add(OldOrganization._Notifications);

        }

        #endregion


        #region (private) _GetAllParents(ref Parents)

        private void _GetAllParents(ref HashSet<Organization> Parents)
        {

            var parents = _Organization2Organization_OutEdges.Where(edge => edge.Source == this && edge.EdgeLabel == Organization2OrganizationEdgeTypes.IsChildOf).Select(edge => edge.Target).ToArray();

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

        #region GetMeAndAllMyParents(Filter = null)

        public IEnumerable<Organization> GetMeAndAllMyParents(Func<Organization, Boolean> Include = null)
        {

            var parentsAndMe = new HashSet<Organization>();
            parentsAndMe.Add(this);
            _GetAllParents(ref parentsAndMe);

            return Include != null
                       ? parentsAndMe.Where(Include)
                       : parentsAndMe;

        }

        #endregion


        public IEnumerable<User> Admins
            => _User2Organization_InEdges.Where(_ => _.EdgeLabel == User2OrganizationEdgeTypes.IsAdmin).
                                          SafeSelect(edge => edge.Source).
                                          Distinct();

        public IEnumerable<User> Members
            => _User2Organization_InEdges.Where(_ => _.EdgeLabel == User2OrganizationEdgeTypes.IsMember).
                                          SafeSelect(edge => edge.Source).
                                          Distinct();

        public IEnumerable<User> Guests
            => _User2Organization_InEdges.Where(_ => _.EdgeLabel == User2OrganizationEdgeTypes.IsGuest).
                                          SafeSelect(edge => edge.Source).
                                          Distinct();

        public IEnumerable<User> Users
            => _User2Organization_InEdges.Where(_ => _.EdgeLabel == User2OrganizationEdgeTypes.IsAdmin ||
                                                     _.EdgeLabel == User2OrganizationEdgeTypes.IsMember).
                                          SafeSelect(edge => edge.Source).
                                          Distinct();

        public IEnumerable<Organization> Parents
            => _Organization2Organization_OutEdges.Where(edge => edge.Source == this && edge.EdgeLabel == Organization2OrganizationEdgeTypes.IsChildOf).Select(edge => edge.Target).ToArray();

        /// <summary>
        /// A relationship between two organizations where the first includes the second, e.g., as a subsidiary. See also: the more specific 'department' property.
        /// </summary>
        public IEnumerable<Organization> SubOrganizations
            => _Organization2Organization_InEdges.Where(edge => edge.Source == this && edge.EdgeLabel == Organization2OrganizationEdgeTypes.IsChildOf).Select(edge => edge.Source).ToArray();


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
        public override Int32 CompareTo(Object Object)

            => Object is Organization Organization
                   ? CompareTo(Organization)
                   : throw new ArgumentException("The given object is not an organization!", nameof(Object));

        #endregion

        #region CompareTo(Organization)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Organization">An organization object to compare with.</param>
        public override Int32 CompareTo(Organization Organization)

            => Organization is null
                   ? throw new ArgumentNullException(nameof(Organization), "The given organization must not be null!")
                   : Id.CompareTo(Organization.Id);

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

            => Object is Organization Organization &&
                  Equals(Organization);

        #endregion

        #region Equals(Organization)

        /// <summary>
        /// Compares two organizations for equality.
        /// </summary>
        /// <param name="Organization">An organization to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(Organization Organization)

            => Organization is Organization &&
                   Id.Equals(Organization.Id);

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
                           Address,
                           GeoLocation,
                           _ => Tags,
                           IsDisabled,

                           _Notifications,

                           _User2Organization_InEdges,
                           _Organization2Organization_InEdges,
                           _Organization2Organization_OutEdges,

                           CustomData,
                           AttachedFiles,
                           JSONLDContext,
                           DataSource,
                           LastChange);

        #endregion

        #region (class) Builder

        /// <summary>
        /// An organization builder.
        /// </summary>
        public new class Builder : AEntity<Organization_Id,
                                           Organization>.Builder
        {

            #region Properties

            /// <summary>
            /// The offical (multi-language) name of the organization.
            /// </summary>
            [Mandatory]
            public I18NString             Name                 { get; set; }

            /// <summary>
            /// The optional (multi-language) description of the organization.
            /// </summary>
            [Optional]
            public I18NString             Description          { get; set; }

            /// <summary>
            /// The website of the organization.
            /// </summary>
            [Optional]
            public String                 Website              { get; set; }

            /// <summary>
            /// The primary E-Mail address of the organization.
            /// </summary>
            [Optional]
            public EMailAddress           EMail                { get; set; }

            /// <summary>
            /// The telephone number of the organization.
            /// </summary>
            [Optional]
            public PhoneNumber?           Telephone            { get; set; }

            /// <summary>
            /// The optional address of the organization.
            /// </summary>
            [Optional]
            public Address                Address              { get; set; }

            /// <summary>
            /// The geographical location of this organization.
            /// </summary>
            public GeoCoordinate?         GeoLocation          { get; set; }

            /// <summary>
            /// An collection of multi-language tags and their relevance.
            /// </summary>
            [Optional]
            public Tags                   Tags                 { get; set; }

            /// <summary>
            /// The user will be shown in organization listings.
            /// </summary>
            [Mandatory]
            public Boolean                IsDisabled           { get; set; }

            /// <summary>
            /// An enumeration of attached files.
            /// </summary>
            [Optional]
            public HashSet<AttachedFile>  AttachedFiles        { get; }

            #endregion

            #region Edges

            #region User          -> Organization edges

            protected readonly List<User2OrganizationEdge> _User2Organization_InEdges;

            public IEnumerable<User2OrganizationEdge> User2OrganizationEdges
                => _User2Organization_InEdges;


            #region LinkUser(Edge)

            public User2OrganizationEdge

                LinkUser(User2OrganizationEdge Edge)

                => _User2Organization_InEdges.AddAndReturnElement(Edge);

            #endregion

            #region LinkUser(Source, EdgeLabel, PrivacyLevel = PrivacyLevel.World)

            public User2OrganizationEdge

                LinkUser(User                    Source,
                         User2OrganizationEdgeTypes  EdgeLabel,
                         PrivacyLevel            PrivacyLevel = PrivacyLevel.World)

                => _User2Organization_InEdges.
                       AddAndReturnElement(new User2OrganizationEdge(Source,
                                                                                                    EdgeLabel,
                                                                                                    this,
                                                                                                    PrivacyLevel));

            #endregion


            #region User2OrganizationInEdges     (User)

            /// <summary>
            /// The edge labels of all (incoming) edges between the given user and this organization.
            /// </summary>
            public IEnumerable<User2OrganizationEdge> User2OrganizationInEdges(User User)

                => _User2Organization_InEdges.
                       Where(edge => edge.Source == User);

            #endregion

            #region User2OrganizationInEdgeLabels(User)

            /// <summary>
            /// The edge labels of all (incoming) edges between the given user and this organization.
            /// </summary>
            public IEnumerable<User2OrganizationEdgeTypes> User2OrganizationInEdgeLabels(User User)

                => _User2Organization_InEdges.
                       Where (edge => edge.Source == User).
                       Select(edge => edge.EdgeLabel);

            #endregion

            public IEnumerable<User2OrganizationEdge>

                Add(IEnumerable<User2OrganizationEdge> Edges)

                    => _User2Organization_InEdges.AddAndReturnList(Edges);


            #region UnlinkUser(EdgeLabel, User)

            public void UnlinkUser(User2OrganizationEdgeTypes  EdgeLabel,
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

            public Boolean RemoveInEdge(User2OrganizationEdge Edge)
                => _User2Organization_InEdges.Remove(Edge);

            #endregion

            #region Organization <-> Organization edges

            protected readonly List<Organization2OrganizationEdge> _Organization2Organization_InEdges;

            public IEnumerable<Organization2OrganizationEdge> Organization2OrganizationInEdges
                => _Organization2Organization_InEdges;

            #region AddInEdge (Edge)

            public Organization2OrganizationEdge

                AddInEdge(Organization2OrganizationEdge Edge)

                => _Organization2Organization_InEdges.AddAndReturnElement(Edge);

            #endregion

            #region AddInEdge (EdgeLabel, SourceOrganization, PrivacyLevel = PrivacyLevel.World)

            public Organization2OrganizationEdge

                AddInEdge (Organization2OrganizationEdgeTypes  EdgeLabel,
                           Organization                    SourceOrganization,
                           PrivacyLevel                    PrivacyLevel = PrivacyLevel.World)

                => _Organization2Organization_InEdges. AddAndReturnElement(new Organization2OrganizationEdge(SourceOrganization,
                                                                                                                                                    EdgeLabel,
                                                                                                                                                    this,
                                                                                                                                                    PrivacyLevel));

            #endregion

            public IEnumerable<Organization2OrganizationEdge>

                AddInEdges(IEnumerable<Organization2OrganizationEdge> Edges)

                    => _Organization2Organization_InEdges.AddAndReturnList(Edges);

            #region RemoveInEdges(EdgeLabel, TargetOrganization)

            public Boolean RemoveInEdge(Organization2OrganizationEdge Edge)
                => _Organization2Organization_InEdges.Remove(Edge);

            #endregion

            #region RemoveInEdges (EdgeLabel, SourceOrganization)

            public void RemoveInEdges(Organization2OrganizationEdgeTypes EdgeLabel,
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



            protected readonly List<Organization2OrganizationEdge> _Organization2Organization_OutEdges;

            public IEnumerable<Organization2OrganizationEdge> Organization2OrganizationOutEdges
                => _Organization2Organization_OutEdges;

            #region AddOutEdge(Edge)

            public Organization2OrganizationEdge

                AddOutEdge(Organization2OrganizationEdge Edge)

                => _Organization2Organization_OutEdges.AddAndReturnElement(Edge);

            #endregion

            #region AddOutEdge(EdgeLabel, TargetOrganization, PrivacyLevel = PrivacyLevel.World)

            public Organization2OrganizationEdge

                AddOutEdge(Organization2OrganizationEdgeTypes  EdgeLabel,
                           Organization                    TargetOrganization,
                           PrivacyLevel                    PrivacyLevel = PrivacyLevel.World)

                => _Organization2Organization_OutEdges.AddAndReturnElement(new Organization2OrganizationEdge(this,
                                                                                                                                                    EdgeLabel,
                                                                                                                                                    TargetOrganization,
                                                                                                                                                    PrivacyLevel));

            #endregion

            public IEnumerable<Organization2OrganizationEdge>

                AddOutEdges(IEnumerable<Organization2OrganizationEdge> Edges)

                    => _Organization2Organization_OutEdges.AddAndReturnList(Edges);

            #region RemoveOutEdges(EdgeLabel, TargetOrganization)

            public Boolean RemoveOutEdge(Organization2OrganizationEdge Edge)
                => _Organization2Organization_OutEdges.Remove(Edge);

            #endregion

            #region RemoveOutEdges(EdgeLabel, TargetOrganization)

            public void RemoveOutEdges(Organization2OrganizationEdgeTypes  EdgeLabel,
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
            /// <param name="IsDisabled">The organization is disabled.</param>
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            public Builder(Organization_Id                             Id,
                           I18NString                                  Name                                = null,
                           I18NString                                  Description                         = null,
                           String                                      Website                             = null,
                           EMailAddress                                EMail                               = null,
                           PhoneNumber?                                Telephone                           = null,
                           Address                                     Address                             = null,
                           GeoCoordinate?                              GeoLocation                         = null,
                           Func<Tags.Builder, Tags>                    Tags                                = null,
                           Boolean                                     IsDisabled                          = false,

                           IEnumerable<ANotification>                  Notifications                       = null,

                           IEnumerable<User2OrganizationEdge>          User2OrganizationInEdges            = null,
                           IEnumerable<Organization2OrganizationEdge>  Organization2OrganizationInEdges    = null,
                           IEnumerable<Organization2OrganizationEdge>  Organization2OrganizationOutEdges   = null,

                           JObject                                     CustomData                          = default,
                           IEnumerable<AttachedFile>                   AttachedFiles                       = default,
                           JSONLDContext?                              JSONLDContext                       = default,
                           String                                      DataSource                          = default,
                           DateTime?                                   LastChange                          = default)

                : base(Id,
                       JSONLDContext ?? DefaultJSONLDContext,
                       CustomData,
                       DataSource,
                       LastChange)

            {

                this.Name                                 = Name        ?? new I18NString();
                this.Description                          = Description ?? new I18NString();
                this.Website                              = Website;
                this.EMail                                = EMail;
                this.Telephone                            = Telephone;
                this.Address                              = Address;
                this.GeoLocation                          = GeoLocation;
                var _TagsBuilder                          = new Tags.Builder();
                this.Tags                                 = Tags != null ? Tags(_TagsBuilder) : _TagsBuilder;
                this.IsDisabled                           = IsDisabled;
                this.AttachedFiles                        = AttachedFiles.SafeAny() ? new HashSet<AttachedFile>(AttachedFiles) : new HashSet<AttachedFile>();

                this._Notifications                       = new NotificationStore();

                if (Notifications.SafeAny())
                    _Notifications.Add(Notifications);

                // Init edges
                this._User2Organization_InEdges           = User2OrganizationInEdges.         IsNeitherNullNorEmpty() ? new List<User2OrganizationEdge>                (User2OrganizationInEdges)          : new List<User2OrganizationEdge>();
                this._Organization2Organization_InEdges   = Organization2OrganizationInEdges. IsNeitherNullNorEmpty() ? new List<Organization2OrganizationEdge>(Organization2OrganizationInEdges)  : new List<Organization2OrganizationEdge>();
                this._Organization2Organization_OutEdges  = Organization2OrganizationOutEdges.IsNeitherNullNorEmpty() ? new List<Organization2OrganizationEdge>(Organization2OrganizationOutEdges) : new List<Organization2OrganizationEdge>();

            }

            #endregion


            #region Notifications

            private readonly NotificationStore _Notifications;

            #region (internal) AddNotification(Notification,                           OnUpdate = null)

            internal T AddNotification<T>(T          Notification,
                                          Action<T>  OnUpdate  = null)

                where T : ANotification

                => _Notifications.Add(Notification,
                                      OnUpdate);

            #endregion

            #region (internal) AddNotification(Notification, NotificationMessageType,  OnUpdate = null)

            internal T AddNotification<T>(T                        Notification,
                                          NotificationMessageType  NotificationMessageType,
                                          Action<T>                OnUpdate  = null)

                where T : ANotification

                => _Notifications.Add(Notification,
                                      NotificationMessageType,
                                      OnUpdate);

            #endregion

            #region (internal) AddNotification(Notification, NotificationMessageTypes, OnUpdate = null)

            internal T AddNotification<T>(T                                     Notification,
                                          IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                          Action<T>                             OnUpdate  = null)

                where T : ANotification

                => _Notifications.Add(Notification,
                                      NotificationMessageTypes,
                                      OnUpdate);

            #endregion


            #region GetNotifications  (NotificationMessageType = null)

            public IEnumerable<ANotification> GetNotifications(NotificationMessageType?  NotificationMessageType = null)
            {
                lock (_Notifications)
                {
                    return _Notifications.GetNotifications(NotificationMessageType);
                }
            }

            #endregion

            #region GetNotificationsOf(params NotificationMessageTypes)

            public IEnumerable<T> GetNotificationsOf<T>(params NotificationMessageType[] NotificationMessageTypes)

                where T : ANotification

            {

                lock (_Notifications)
                {
                    return _Notifications.GetNotificationsOf<T>(NotificationMessageTypes);
                }

            }

            #endregion

            #region GetNotifications  (NotificationMessageTypeFilter)

            public IEnumerable<ANotification> GetNotifications(Func<NotificationMessageType, Boolean> NotificationMessageTypeFilter)
            {
                lock (_Notifications)
                {
                    return _Notifications.GetNotifications(NotificationMessageTypeFilter);
                }
            }

            #endregion

            #region GetNotificationsOf(NotificationMessageTypeFilter)

            public IEnumerable<T> GetNotificationsOf<T>(Func<NotificationMessageType, Boolean> NotificationMessageTypeFilter)

                where T : ANotification

            {

                lock (_Notifications)
                {
                    return _Notifications.GetNotificationsOf<T>(NotificationMessageTypeFilter);
                }

            }

            #endregion


            #region GetNotificationInfos()

            public JObject GetNotificationInfos()

                => JSONObject.Create(new JProperty("user", JSONObject.Create(

                                         new JProperty("name",               EMail.OwnerName),
                                         new JProperty("email",              EMail.Address.ToString())

                                         //MobilePhone.HasValue
                                         //    ? new JProperty("phoneNumber",  MobilePhone.Value.ToString())
                                         //    : null

                                     )),
                                     new JProperty("notifications",  _Notifications.ToJSON()));

            #endregion


            #region (internal) RemoveNotification(NotificationType,                           OnRemoval = null)

            internal T RemoveNotification<T>(T          NotificationType,
                                             Action<T>  OnRemoval  = null)

                where T : ANotification

                => _Notifications.Remove(NotificationType,
                                         OnRemoval);

            #endregion

            #endregion


            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the organization.
            /// </summary>
            /// <param name="Builder">An organization builder.</param>
            public static implicit operator Organization(Builder Builder)

                => Builder?.ToImmutable;


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
                                    Address,
                                    GeoLocation,
                                    _ => Tags,
                                    IsDisabled,

                                    _Notifications,

                                    _User2Organization_InEdges,
                                    _Organization2Organization_InEdges,
                                    _Organization2Organization_OutEdges,

                                    CustomData,
                                    AttachedFiles,
                                    JSONLDContext,
                                    DataSource,
                                    LastChange);

            #endregion


            #region CopyAllLinkedDataFrom(OldOrganization)

            public override void CopyAllLinkedDataFrom(Organization OldOrganization)
            {

                if (OldOrganization._User2Organization_InEdges.Any() && !_User2Organization_InEdges.Any())
                {

                    Add(OldOrganization._User2Organization_InEdges);

                    foreach (var edge in _User2Organization_InEdges)
                        edge.Target = this;

                }

                if (OldOrganization._Organization2Organization_InEdges.Any() && !_Organization2Organization_InEdges.Any())
                {

                    AddInEdges(OldOrganization._Organization2Organization_InEdges);

                    foreach (var edge in _Organization2Organization_InEdges)
                        edge.Target = this;

                }

                if (OldOrganization._Organization2Organization_OutEdges.Any() && !_Organization2Organization_OutEdges.Any())
                {

                    AddOutEdges(OldOrganization._Organization2Organization_OutEdges);

                    foreach (var edge in _Organization2Organization_OutEdges)
                        edge.Source = this;

                }

                if (OldOrganization._Notifications.SafeAny() && !_Notifications.SafeAny())
                    _Notifications.Add(OldOrganization._Notifications);

            }

            #endregion


            #region Operator overloading

            #region Operator == (BuilderId1, BuilderId2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="BuilderId1">A organization builder.</param>
            /// <param name="BuilderId2">Another organization builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator == (Builder BuilderId1, Builder BuilderId2)
            {

                // If both are null, or both are same instance, return true.
                if (Object.ReferenceEquals(BuilderId1, BuilderId2))
                    return true;

                // If one is null, but not both, return false.
                if ((BuilderId1 is null) || (BuilderId2 is null))
                    return false;

                return BuilderId1.Equals(BuilderId2);

            }

            #endregion

            #region Operator != (BuilderId1, BuilderId2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="BuilderId1">A organization builder.</param>
            /// <param name="BuilderId2">Another organization builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator != (Builder BuilderId1, Builder BuilderId2)
                => !(BuilderId1 == BuilderId2);

            #endregion

            #region Operator <  (BuilderId1, BuilderId2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="BuilderId1">A organization builder.</param>
            /// <param name="BuilderId2">Another organization builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator < (Builder BuilderId1, Builder BuilderId2)
            {

                if (BuilderId1 is null)
                    throw new ArgumentNullException(nameof(BuilderId1), "The given BuilderId1 must not be null!");

                return BuilderId1.CompareTo(BuilderId2) < 0;

            }

            #endregion

            #region Operator <= (BuilderId1, BuilderId2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="BuilderId1">A organization builder.</param>
            /// <param name="BuilderId2">Another organization builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator <= (Builder BuilderId1, Builder BuilderId2)
                => !(BuilderId1 > BuilderId2);

            #endregion

            #region Operator >  (BuilderId1, BuilderId2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="BuilderId1">A organization builder.</param>
            /// <param name="BuilderId2">Another organization builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator > (Builder BuilderId1, Builder BuilderId2)
            {

                if (BuilderId1 is null)
                    throw new ArgumentNullException(nameof(BuilderId1), "The given BuilderId1 must not be null!");

                return BuilderId1.CompareTo(BuilderId2) > 0;

            }

            #endregion

            #region Operator >= (BuilderId1, BuilderId2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="BuilderId1">A organization builder.</param>
            /// <param name="BuilderId2">Another organization builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator >= (Builder BuilderId1, Builder BuilderId2)
                => !(BuilderId1 < BuilderId2);

            #endregion

            #endregion

            #region IComparable<Builder> Members

            #region CompareTo(Object)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Object">An object to compare with.</param>
            public override Int32 CompareTo(Object Object)

                => Object is Builder Builder
                       ? CompareTo(Builder)
                       : throw new ArgumentException("The given object is not an organization!");

            #endregion

            #region CompareTo(Builder)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Builder">An organization object to compare with.</param>
            public Int32 CompareTo(Builder Builder)

                => Builder is null
                       ? throw new ArgumentNullException(nameof(Builder), "The given organization must not be null!")
                       : Id.CompareTo(Builder.Id);

            #endregion

            #endregion

            #region IEquatable<Builder> Members

            #region Equals(Object)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Object">An object to compare with.</param>
            /// <returns>true|false</returns>
            public override Boolean Equals(Object Object)

                => Object is Builder Builder &&
                      Equals(Builder);

            #endregion

            #region Equals(Builder)

            /// <summary>
            /// Compares two organizations for equality.
            /// </summary>
            /// <param name="Builder">An organization to compare with.</param>
            /// <returns>True if both match; False otherwise.</returns>
            public Boolean Equals(Builder Builder)

                => Builder is Builder &&
                       Id.Equals(Builder.Id);

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

        }

        #endregion

    }

}
