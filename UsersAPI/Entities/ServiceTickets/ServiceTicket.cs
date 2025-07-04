﻿/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Data;

using Newtonsoft.Json.Linq;

using social.OpenData.UsersAPI;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Styx.Arrows;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace social.OpenData.UsersAPI
{

    public delegate Boolean ServiceTicketProviderDelegate(ServiceTicket_Id ServiceTicketId, out ServiceTicket ServiceTicket);

    /// <summary>
    /// A service ticket.
    /// </summary>
    public class ServiceTicket : AEntity<ServiceTicket_Id,
                                         ServiceTicket>
    {

        #region Data

        /// <summary>
        /// ASCII unit/cell separator
        /// </summary>
        public const Char US = (Char) 0x1F;

        /// <summary>
        /// ASCII record/row separator
        /// </summary>
        public const Char RS = (Char) 0x1E;

        /// <summary>
        /// ASCII group separator
        /// </summary>
        public const Char GS = (Char) 0x1D;

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://opendata.social/contexts/UsersAPI/serviceTicket");

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        // public String                                   JSONLDContext           { get; }

        #region API

        private UsersAPI _API;

        /// <summary>
        /// The UsersAPI of this service ticket.
        /// </summary>
        public UsersAPI API
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
                    throw new ArgumentException("Illegal attempt to change the API of this service ticket!");

                _API = value ?? throw new ArgumentException("Illegal attempt to delete the API reference of this service ticket!");

            }

        }

        #endregion


        /// <summary>
        /// An enumeration of historichal changes to this ticket.
        /// </summary>
        public IEnumerable<ServiceTicketChangeSet>      ChangeSets              { get; }


        /// <summary>
        /// The timestamp when this ticket was created.
        /// </summary>
        public DateTime                                 Created                 { get; }

        /// <summary>
        /// The initial author of this service ticket.
        /// </summary>
        public IUser                                    Author                  { get; }

        /// <summary>
        /// The current status of the service ticket.
        /// </summary>
        public Timestamped<ServiceTicketStatusTypes>    Status                  { get; }

        /// <summary>
        /// The title of the service ticket (10-200 characters).
        /// </summary>
        public I18NString                               Title                   { get; }


        /// <summary>
        /// Affected devices or services by this service ticket.
        /// </summary>
        public Affected                                 Affected                { get; }

        /// <summary>
        /// Whether the service ticket will be shown in (public) listings.
        /// </summary>
        public ServiceTicketPriorities                  Priority                { get; }

        /// <summary>
        /// The location of the problem or broken device.
        /// </summary>
        public I18NString                               Location                { get; }

        /// <summary>
        /// The geographical location of the problem or broken device.
        /// </summary>
        public GeoCoordinate?                           GeoLocation             { get; }

        /// <summary>
        /// An enumeration of well-defined problem descriptions.
        /// </summary>
        public IEnumerable<ProblemDescriptionI18N>      ProblemDescriptions     { get; }

        /// <summary>
        /// An enumeration of status indicators.
        /// </summary>
        public IEnumerable<Tag>                         StatusIndicators        { get; }

        /// <summary>
        /// The first official response to a service ticket.
        /// </summary>
        public FirstResponse?                           FirstResponse           { get; }

        /// <summary>
        /// An enumeration of reactions.
        /// </summary>
        public IEnumerable<Tag>                         Reactions               { get; }

        /// <summary>
        /// Additional multi-language information related to this service ticket.
        /// </summary>
        public I18NString                               AdditionalInfo          { get; }

        /// <summary>
        /// An enumeration of URLs to files attached to this service ticket change set.
        /// </summary>
        public IEnumerable<AttachedFile>                AttachedFiles           { get; }

        /// <summary>
        /// References to other service tickets.
        /// </summary>
        public IEnumerable<ServiceTicketReference>      TicketReferences        { get; }

        /// <summary>
        /// An enumeration of usable data licenses for this service ticket.
        /// </summary>
        public IEnumerable<DataLicense>                 DataLicenses            { get; }

        #endregion

        #region Constructor(s)

        #region ServiceTicket(Id, ChangeSets, ...)

        /// <summary>
        /// Create a new service ticket.
        /// </summary>
        /// <param name="Id">The unique identification of the service ticket.</param>
        /// <param name="ChangeSets">An enumeration of service ticket change sets.</param>
        public ServiceTicket(ServiceTicket_Id                     Id,
                             IEnumerable<ServiceTicketChangeSet>  ChangeSets,
                             JSONLDContext?                       JSONLDContext   = default)

            : base(Id,
                   JSONLDContext ?? DefaultJSONLDContext)

        {

            if (!ChangeSets.SafeAny())
                throw new ArgumentNullException(nameof(ChangeSets),  "The enumeration of change sets (the service ticket history) of the service ticket must not be null!");

            this.ChangeSets      = ChangeSets.OrderByDescending(changeSet => changeSet.Timestamp).ToArray();

            Created              = this.ChangeSets.LastOrDefault()?.Timestamp
                                                    ?? Timestamp.Now;


            Author               = this.ChangeSets.LastOrDefault()?.Author;

            if (this.Author == null)
                throw new ArgumentNullException(nameof(Author),   "The author of the service ticket must not be null!");


            var latestStatus     = this.ChangeSets.Where(entry => entry.Status.HasValue).
                                                    FirstOrDefault();

            Status               = latestStatus != null
                                        ? new Timestamped<ServiceTicketStatusTypes>(latestStatus.Timestamp,
                                                                                    latestStatus.Status.Value)
                                        : new Timestamped<ServiceTicketStatusTypes>(this.Created,
                                                                                    ServiceTicketStatusTypes.New);


            Title                = this.ChangeSets.Where(entry => entry.Title.IsNotNullOrEmpty()).
                                                   FirstOrDefault()?.Title;

            if (this.Title.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Title),    "The title of the service ticket must not be null or empty!");


            Affected             = this.ChangeSets.Where(entry => !entry.Affected.IsEmpty()).
                                                   FirstOrDefault()?.Affected;

            Priority             = this.ChangeSets.Where(entry => entry.Priority.HasValue).
                                                   FirstOrDefault()?.Priority
                                                   ?? ServiceTicketPriorities.Normal;

            Location             = this.ChangeSets.Where(entry => entry.Location.IsNotNullOrEmpty()).
                                                   FirstOrDefault()?.Location;

            GeoLocation          = this.ChangeSets.Where(entry => entry.GeoLocation.HasValue).
                                                   FirstOrDefault()?.GeoLocation;

            ProblemDescriptions  = this.ChangeSets.Where(entry => entry.ProblemDescriptions.SafeAny()).
                                                   FirstOrDefault()?.ProblemDescriptions;

            StatusIndicators     = this.ChangeSets.Where(entry => entry.StatusIndicators.SafeAny()).
                                                   FirstOrDefault()?.StatusIndicators;

            FirstResponse        = this.ChangeSets.Where(entry => entry.FirstResponse.HasValue).
                                                   LastOrDefault()?.FirstResponse;

            Reactions            = this.ChangeSets.Where(entry => entry.Reactions.SafeAny()).
                                                   FirstOrDefault()?.Reactions;

            AdditionalInfo       = this.ChangeSets.Where(entry => entry.AdditionalInfo.IsNotNullOrEmpty()).
                                                   FirstOrDefault()?.AdditionalInfo;

            AttachedFiles        = this.ChangeSets.Where(entry => entry.AttachedFiles.SafeAny()).
                                                   FirstOrDefault()?.AttachedFiles;

            TicketReferences     = this.ChangeSets.Where(entry => entry.TicketReferences.SafeAny()).
                                                   FirstOrDefault()?.TicketReferences;

            DataLicenses         = this.ChangeSets.Where(entry => entry.DataLicenses.SafeAny()).
                                                   FirstOrDefault()?.DataLicenses;

        }

        #endregion

        #region ServiceTicket(Id, ...)

        /// <summary>
        /// Create a new service ticket.
        /// </summary>
        /// <param name="Id">The unique identification of the service ticket.</param>
        /// 
        /// <param name="Timestamp">The timestamp of the creation of this service ticket change set.</param>
        /// <param name="Author">The initial author of this service ticket change set (if known).</param>
        /// <param name="Status">An optional new service ticket status caused by this service ticket change set.</param>
        /// <param name="Title">The title of the service ticket (10-200 characters).</param>
        /// <param name="Affected">Affected devices or services by this service ticket.</param>
        /// <param name="Priority">The priority of the service ticket.</param>
        /// <param name="Location">The location of the problem or broken device.</param>
        /// <param name="GeoLocation">The geographical location of the problem or broken device.</param>
        /// <param name="ProblemDescriptions">An enumeration of well-defined problem descriptions.</param>
        /// <param name="StatusIndicators">An enumeration of problem indicators.</param>
        /// <param name="FirstResponse">The first official response to a service ticket.</param>
        /// <param name="Reactions">An enumeration of reactions.</param>
        /// <param name="AdditionalInfo">A multi-language description of the service ticket.</param>
        /// <param name="AttachedFiles">An enumeration of URLs to files attached to this service ticket.</param>
        /// <param name="TicketReferences">References to other service tickets.</param>
        /// <param name="DataLicenses">An enumeration of usable data licenses for this service ticket.</param>
        /// 
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        public ServiceTicket(ServiceTicket_Id?                     Id                    = null,
                             JSONLDContext?                        JSONLDContext         = null,

                             DateTime?                             Timestamp             = null,
                             IUser?                                Author                = null,
                             ServiceTicketStatusTypes?             Status                = null,
                             I18NString?                           Title                 = null,
                             Affected?                             Affected              = null,
                             ServiceTicketPriorities?              Priority              = null,
                             I18NString?                           Location              = null,
                             GeoCoordinate?                        GeoLocation           = null,
                             IEnumerable<ProblemDescriptionI18N>?  ProblemDescriptions   = null,
                             IEnumerable<Tag>?                     StatusIndicators      = null,
                             FirstResponse?                        FirstResponse         = null,
                             IEnumerable<Tag>?                     Reactions             = null,
                             I18NString?                           AdditionalInfo        = null,
                             JObject?                              CustomData            = null,
                             IEnumerable<AttachedFile>?            AttachedFiles         = null,
                             IEnumerable<ServiceTicketReference>?  TicketReferences      = null,
                             IEnumerable<DataLicense>?         DataLicenses          = null,

                             String?                               DataSource            = null)


            : this(Id ?? ServiceTicket_Id.Random(),
                   new List<ServiceTicketChangeSet>() {
                       new ServiceTicketChangeSet(
                           ServiceTicketChangeSet_Id.Random(),
                           ServiceTicketChangeSet.DefaultJSONLDContext,
                           Timestamp     ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                           Author,
                           Status        ?? ServiceTicketStatusTypes.New,
                           Title,
                           Affected,
                           Priority      ?? ServiceTicketPriorities.Normal,
                           Location,
                           GeoLocation,
                           ProblemDescriptions,
                           StatusIndicators,
                           FirstResponse,
                           Reactions,
                           AdditionalInfo,
                           CustomData,
                           AttachedFiles,
                           TicketReferences,
                           DataLicenses,

                           null, // Comment
                           null, // InReplyTo
                           null, // CommentReferences

                           DataSource
                       )
                   },
                   JSONLDContext ?? DefaultJSONLDContext)

        { }

        #endregion

        #endregion


        #region ToJSON(...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data structure is embedded into another data structure.</param>
        public override JObject ToJSON(Boolean Embedded = false)

            => ToJSON(Embedded:            false,
                      MaxStatus:           null,
                      ExpandDataLicenses:  InfoStatus.ShowIdOnly,
                      ExpandAuthorId:      InfoStatus.ShowIdOnly,
                      IncludeChangeSets:   true);


        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure, e.g. into a service ticket.</param>
        /// <param name="IncludeCryptoHash">Whether to include the cryptograhical hash value of this object.</param>
        public JObject ToJSON(Boolean                                          Embedded                        = false,
                              UInt16?                                          MaxStatus                       = null,
                              InfoStatus                                       ExpandDataLicenses              = InfoStatus.ShowIdOnly,
                              InfoStatus                                       ExpandAuthorId                  = InfoStatus.ShowIdOnly,
                              InfoStatus                                       ExpandReactions                 = InfoStatus.ShowIdOnly,
                              Boolean                                          IncludeChangeSets               = true,
                              CustomJObjectSerializerDelegate<ServiceTicket>?  CustomServiceTicketSerializer   = null,
                              CustomJObjectSerializerDelegate<FirstResponse>?  CustomFirstResponseSerializer   = null)

        {

            var Status         = ServiceTicketStatusTypes.New;
            var StatusHistory  = new List<Timestamped<ServiceTicketStatusTypes>>();

            foreach (var history in ChangeSets.Reverse())
            {

                if (history.Status.HasValue && Status != history.Status)
                    Status = history.Status.Value;

                StatusHistory.Add(new Timestamped<ServiceTicketStatusTypes>(history.Timestamp, Status));

            }


            var JSON = base.ToJSON(Embedded,
                                   false, // IncludeLastChange
                                   null,
                                   new JProperty?[] {

                                       new JProperty("title",                      Title.ToJSON()),

                                       new JProperty("status",                     new JObject((StatusHistory as IEnumerable<Timestamped<ServiceTicketStatusTypes>>).
                                                                                                   Reverse().
                                                                                                   Take   (MaxStatus).
                                                                                                   Select (timestamped => new JProperty(timestamped.Timestamp.ToISO8601(),
                                                                                                                                        timestamped.Value.    ToString()))
                                                                                              )),

                                       !Affected.IsEmpty()
                                           ? new JProperty("affected", Affected.ToJSON())
                                           : null,

                                       Author != null
                                           ? ExpandAuthorId.Switch(
                                                 () => new JProperty("author",  new JObject(
                                                                                    new JProperty("@id",   Author.Id.ToString()),
                                                                                    new JProperty("name",  Author.Name)
                                                                                )),
                                                 () => new JProperty("author",  Author.ToJSON(Embedded:            true,
                                                                                              ExpandOrganizations: InfoStatus.Hidden,
                                                                                              ExpandGroups:        InfoStatus.Hidden)))
                                           : null,

                                       new JProperty("priority", Priority.ToString().ToLower()),

                                       Location.IsNotNullOrEmpty()
                                           ? new JProperty("location",     Location.ToJSON())
                                           : null,

                                       GeoLocation.HasValue
                                           ? new JProperty("geoLocation",  GeoLocation.Value.ToJSON())
                                           : null,

                                       ProblemDescriptions.IsNeitherNullNorEmpty()
                                           ? new JProperty("problemDescriptions",  new JArray(ProblemDescriptions.  Select(problemDescription => problemDescription.ToJSON())))
                                           : null,

                                       StatusIndicators.SafeAny()
                                           ? new JProperty("statusIndicators",     new JArray(StatusIndicators.     Select(tag                => tag.Id.ToString())))
                                           : null,

                                       FirstResponse.HasValue
                                           ? new JProperty("firstResponse",        FirstResponse.Value.ToJSON(CustomFirstResponseSerializer))
                                           : null,

                                       Reactions.SafeAny()
                                           ? ExpandReactions.Switch(
                                                 () => new JProperty("reactionIds",   new JArray(Reactions.          Select(tag                => tag.Id.ToString()))),
                                                 () => new JProperty("reactions",     new JArray(Reactions.          Select(tag                => tag.ToJSON()))))
                                           : null,

                                       AdditionalInfo.IsNotNullOrEmpty()
                                           ? new JProperty("additionalInfo", AdditionalInfo.ToJSON())
                                           : null,

                                       AttachedFiles.SafeAny()
                                           ? new JProperty("attachedFiles",        new JArray(AttachedFiles.         Select(attachedFile       => attachedFile.ToString())))
                                           : null,

                                       TicketReferences.SafeAny()
                                           ? new JProperty("ticketReferences",     new JArray(TicketReferences.      Select(references         => references.ToString())))
                                           : null,

                                       IncludeChangeSets && ChangeSets.SafeAny()
                                           ? new JProperty("changeSets",           new JArray(ChangeSets.
                                                                                                  OrderByDescending(changeSet => changeSet.Timestamp).
                                                                                                  SafeSelect       (changeSet => changeSet.ToJSON(true))))
                                           : null,


                                       // MaxPoolStatusListSize

                                       DataLicenses.SafeAny()
                                           ? ExpandDataLicenses.Switch(
                                                 () => new JProperty("dataLicenseIds",  new JArray(DataLicenses.    Select(dataLicense        => dataLicense.Id.ToString()))),
                                                 () => new JProperty("dataLicenses",    DataLicenses.ToJSON()))
                                           : null

                                    });


            return CustomServiceTicketSerializer is not null
                       ? CustomServiceTicketSerializer(this, JSON)
                       : JSON;

        }

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out ServiceTicket, out ErrorResponse)

        /// <summary>
        /// Try to parse the given service ticket JSON.
        /// </summary>
        /// <param name="JSONObject">A JSON object.</param>
        /// <param name="UserProvider">A delegate resolving users.</param>
        /// <param name="OrganizationProvider">A delegate resolving organizations.</param>
        /// <param name="ServiceTicketProvider">A delegate resolving service tickets.</param>
        /// <param name="ServiceTicket">The parsed service ticket.</param>
        /// <param name="ErrorResponse">An error message.</param>
        /// <param name="ExpectedContext">Verify the JSON-LD context.</param>
        /// <param name="ServiceTicketIdURL">An optional service ticket identification, e.g. from the HTTP URL.</param>
        /// <param name="OverwriteAuthor">Overwrite the author of the service ticket, if given.</param>
        public static Boolean TryParseJSON(JObject                        JSONObject,
                                           ServiceTicketProviderDelegate  ServiceTicketProvider,
                                           UserProviderDelegate           UserProvider,
                                           OrganizationProviderDelegate   OrganizationProvider,
                                           out ServiceTicket?             ServiceTicket,
                                           out String?                    ErrorResponse,
                                           JSONLDContext?                 ExpectedContext            = default,
                                           Boolean                        IgnoreChangeSets           = false,
                                           JSONLDContext?                 ExpectedChangeSetContext   = default,
                                           ServiceTicket_Id?              ServiceTicketIdURL         = null,
                                           OverwriteUserDelegate?         OverwriteAuthor            = null)
        {

            try
            {

                ServiceTicket = null;

                if (JSONObject?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Context                    [mandatory]

                if (!JSONObject.ParseMandatory("@context",
                                                "JSON-LD context",
                                                JSONLDContext.TryParse,
                                                out JSONLDContext Context,
                                                out ErrorResponse))
                {
                    ErrorResponse = @"The JSON-LD ""@context"" information is missing!";
                    return false;
                }

                if (!ExpectedContext.HasValue)
                    ExpectedContext = DefaultJSONLDContext;

                if (Context != ExpectedContext.Value)
                {
                    ErrorResponse = @"The given JSON-LD ""@context"" information '" + Context + "' is not supported!";
                    return false;
                }

                #endregion


                #region Parse ServiceTicketId            [optional]

                // Verify that a given service ticket identification
                //   is at least valid.
                if (JSONObject.ParseOptional("@id",
                                             "service ticket identification",
                                             ServiceTicket_Id.TryParse,
                                             out ServiceTicket_Id? ServiceTicketIdBody,
                                             out ErrorResponse))
                {
                    if (ErrorResponse is not null)
                        return false;
                }

                if (!ServiceTicketIdURL.HasValue && !ServiceTicketIdBody.HasValue)
                {
                    ErrorResponse = "The service ticket identification is missing!";
                    return false;
                }

                if (ServiceTicketIdURL.HasValue && ServiceTicketIdBody.HasValue && ServiceTicketIdURL.Value != ServiceTicketIdBody.Value)
                {
                    ErrorResponse = "The optional service ticket identification given within the JSON body does not match the one given in the URI!";
                    return false;
                }

                #endregion

                // Timestamp?

                #region Parse Author                     [optional]

                IUser? Author = null;

                if (JSONObject["author"] is JObject authorJSON &&
                    authorJSON.ParseOptional("@id",
                                             "author identification",
                                             User_Id.TryParse,
                                             out User_Id? UserId,
                                             out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (UserId.HasValue &&
                       !UserProvider(UserId.Value, out Author))
                    {
                        ErrorResponse = "The given author '" + UserId + "' is unknown!";
                        return false;
                    }

                }

                if (OverwriteAuthor is not null && Author is not null)
                    Author = OverwriteAuthor(Author);

                #endregion

                // Status?

                #region Parse Title                      [mandatory]

                if (JSONObject.ParseMandatory("title",
                                              "title",
                                              out I18NString Title,
                                              out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse Affected                   [optional]

                Affected Affected = null;

                if (JSONObject.ParseOptional("affected",
                                             "affected things",
                                             out JObject AffectedJSON,
                                             out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (AffectedJSON != null)
                    {

                        try
                        {

                            var success = Affected.TryParseJSON(AffectedJSON,
                                                                ServiceTicketProvider,
                                                                UserProvider,
                                                                OrganizationProvider,
                                                                out Affected,
                                                                out ErrorResponse);

                            if (!success)
                                return false;

                        }
                        catch (Exception e)
                        {
                            return false;
                        }

                    }

                }

                #endregion

                #region Parse Priority                   [optional]

                if (JSONObject.ParseOptionalEnum("priority",
                                                 "priority",
                                                 out ServiceTicketPriorities? Priority,
                                                 out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse Location                   [optional]

                if (JSONObject.ParseOptional("location",
                                             "location",
                                             out I18NString Location,
                                             out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse GeoLocation                [optional]

                if (JSONObject.ParseOptionalJSON("geoLocation",
                                                 "geo location",
                                                 GeoCoordinate.TryParse,
                                                 out GeoCoordinate? GeoLocation,
                                                 out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse Problem descriptions       [optional]

                if (!JSONObject.ParseOptionalI18N("problemDescriptions",
                                                  "problem descriptions",
                                                  ProblemDescriptionI18N.TryParse,
                                                  out IEnumerable<ProblemDescriptionI18N> ProblemDescriptions,
                                                  out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse Status Indicators          [optional]

                var StatusIndicators = new HashSet<Tag>();

                if (JSONObject.ParseOptional("statusIndicators",
                                             "status indicators",
                                             out JArray statusIndicatorsJSON,
                                             out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (statusIndicatorsJSON != null)
                    {

                        var text = "";

                        foreach (var statusIndicator in statusIndicatorsJSON)
                        {

                            text = statusIndicator.Value<String>();

                            if (text.IsNotNullOrEmpty())
                            {
                                text = text.Trim();
                                if (text != "")
                                    StatusIndicators.Add(new Tag(Tag_Id.Parse(text)));
                            }

                        }

                    }

                }

                #endregion

                #region Parse FirstResponse              [optional]

                if (JSONObject.ParseOptionalJSON("firstResponse",
                                                 "first response",
                                                 OpenData.UsersAPI.FirstResponse.TryParse,
                                                 out FirstResponse? FirstResponse,
                                                 out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                // Reactions

                #region Parse AdditionalInfo             [optional]

                if (JSONObject.ParseOptional("additionalInfo",
                                             "additional information",
                                             out I18NString AdditionalInfo,
                                             out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse AttachedFiles              [optional]

                if (JSONObject.ParseOptionalHashSet("attachedFiles",
                                                    "attached files",
                                                    AttachedFile.TryParseJSON,
                                                    out HashSet<AttachedFile> AttachedFiles,
                                                    out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                // TicketReferences

                #region Parse DataLicenseIds             [optional]

                if (JSONObject.ParseOptional("dataLicenseIds",
                                             "data license identifications",
                                             DataLicense_Id.TryParse,
                                             out IEnumerable<DataLicense_Id> DataLicenseIds,
                                             out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                List<DataLicense> DataLicenses = null;

                if (DataLicenseIds?.Any() == true)
                {

                    DataLicenses = new List<DataLicense>();

                    foreach (var dataLicenseId in DataLicenseIds)
                    {

                        //if (!TryGetDataLicense(dataLicenseId, out DataLicense DataLicense))
                        //{

                        //    return SetCommunicatorResponse(
                        //               new HTTPResponseBuilder(Request) {
                        //                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                        //                   Server                     = HTTPServer.DefaultServerName,
                        //                   Date                       = Timestamp.Now,
                        //                   AccessControlAllowOrigin   = "*",
                        //                   AccessControlAllowMethods  = new[] { "GET", "SET" },
                        //                   AccessControlAllowHeaders  = [ "Content-Type", "Accept", "Authorization" ],
                        //                   ETag                       = "1",
                        //                   ContentType                = HTTPContentType.Application.JSON_UTF8,
                        //                   Content                    = JSONObject.Create(
                        //                                                    new JProperty("description", "The given data license '" + dataLicenseId + "' is unknown!")
                        //                                                ).ToUTF8Bytes(),
                        //                   Connection                 = ConnectionType.Close
                        //               }.AsImmutable());

                        //}

                        //DataLicenses.Add(DataLicense);

                    }

                }

                #endregion


                #region Parse ChangeSets                 [optional]

                var ChangeSets = new List<ServiceTicketChangeSet>();

                if (!IgnoreChangeSets &&
                    JSONObject.ParseOptional("changeSets",
                                             "service ticket change sets",
                                             out JArray ChangeSetsJSON,
                                             out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (ChangeSetsJSON != null)
                    {
                        foreach (var changeSet in ChangeSetsJSON)
                        {

                            if (changeSet is JObject changeSetJSON)
                            {

                                //var context = historyJSON.GetString("@context");

                                ServiceTicketChangeSet serviceTicketHistoryEntry = null;

                                //switch (context)
                                //{

                                //    case ServiceTicketChangeSet.DefaultJSONLDContext:
                                        if (!ServiceTicketChangeSet.TryParseJSON(changeSetJSON,
                                                                                 ServiceTicketProvider,
                                                                                 UserProvider,
                                                                                 OrganizationProvider,
                                                                                 out ServiceTicketChangeSet serviceTicketHistory,
                                                                                 out ErrorResponse,
                                                                                 ExpectedChangeSetContext ?? ServiceTicketChangeSet.DefaultJSONLDContext))
                                        {
                                            return false;
                                        }

                                        serviceTicketHistoryEntry = serviceTicketHistory;
                                        //break;

                                    //case RMA.JSONLDContext:
                                    //    if (!RMA.TryParseJSON(historyJSON,
                                    //                          UserProvider,
                                    //                          out RMA serviceTicketRMA,
                                    //                          out ErrorResponse))
                                    //    {
                                    //        return false;
                                    //    }

                                    //    serviceTicketHistoryEntry = serviceTicketRMA;
                                    //    break;

                                //}

                                ChangeSets.Add(serviceTicketHistoryEntry);

                            }

                            else
                                return false;

                        }
                    }

                }

                #endregion


                #region Get   DataSource                 [optional]

                var DataSource = JSONObject.GetOptional("dataSource");

                #endregion


                #region Parse CryptoHash                 [optional]

                var CryptoHash    = JSONObject.GetOptional("cryptoHash");

                #endregion


                ServiceTicket = ChangeSets.SafeAny()

                                    ? new ServiceTicket(
                                          ServiceTicketIdBody ?? ServiceTicketIdURL.Value,
                                          ChangeSets,
                                          Context
                                      )

                                    : new ServiceTicket(
                                          ServiceTicketIdBody ?? ServiceTicketIdURL.Value,
                                          Context,

                                          Timestamp.Now,
                                          Author,
                                          ServiceTicketStatusTypes.New,
                                          Title,
                                          Affected,
                                          Priority,
                                          Location,
                                          GeoLocation,
                                          ProblemDescriptions,
                                          StatusIndicators,
                                          FirstResponse,
                                          null, // Reactions
                                          AdditionalInfo,
                                          null, // CustomData
                                          AttachedFiles,
                                          null, // TicketReferences
                                          DataLicenses,

                                          DataSource
                                      );


                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse  = e.Message;
                ServiceTicket  = null;
                return false;
            }

        }

        #endregion

        #region ToCSV(IncludeMessageContext = false, UnitSeparator = US, ShortModelNamespaces = false)

        public virtual String ToCSV(Boolean IncludeMessageContext   = false,
                                    Char    UnitSeparator           = US,
                                    Boolean ShortModelNamespaces    = false)
        {

            var firstResponseSeconds  = FirstResponse.HasValue ? new Double?(Math.Round(FirstResponse.Value.ResponseTime.TotalSeconds, 0))    : new Double?();
            var firstResponseDays     = FirstResponse.HasValue ? Math.Floor(firstResponseSeconds.Value / 60 / (60 * 24))                      : -1;
            var firstResponseHours    = FirstResponse.HasValue ? Math.Floor(firstResponseSeconds.Value / 60 / 60) - (firstResponseDays  * 24) : -1;
            var firstResponseMinutes  = FirstResponse.HasValue ? Math.Floor(firstResponseSeconds.Value / 60)      - (firstResponseHours * 60) : -1;
            var firstResponse         = FirstResponse.HasValue
                                            ? (firstResponseDays                            > 0 ? firstResponseDays    + " days "   : "") +
                                              (firstResponseHours                           > 0 ? firstResponseHours   + " hours "  : "") +
                                              (firstResponseDays == 0 && firstResponseHours < 4 ? firstResponseMinutes + " minutes" : "")
                                            : "";

            var ageSeconds            = Math.Round(((ChangeSets.FirstOrDefault(changeSet => changeSet.Status == ServiceTicketStatusTypes.Closed)?.Timestamp ?? Timestamp.Now) - ChangeSets.Last().Timestamp).TotalSeconds, 0);
            var ageDays               = Math.Floor(ageSeconds / 60 / (60 * 24));
            var ageHours              = Math.Floor(ageSeconds / 60 / 60) - (ageDays  * 24);
            var ageMinutes            = Math.Floor(ageSeconds / 60)      - (ageHours * 60);
            var age                   = (ageDays                  > 0 ? ageDays    + " days "   : "") +
                                        (ageHours                 > 0 ? ageHours   + " hours "  : "") +
                                        (ageDays == 0 && ageHours < 4 ? ageMinutes + " minutes" : "");

            return String.Concat(Status.    Value.ToString(),                                                                           UnitSeparator,
                                 Status.Timestamp.ToISO8601(),                                                                          UnitSeparator,
                                 Title.           FirstText(),                                                                          UnitSeparator,
                                 ProblemDescriptions.Select(problemDescription => problemDescription.FirstText()).AggregateWith(". "),  UnitSeparator,
                                 //!Affected.IsEmpty()
                                 //    ? Affected.DefibrillatorDeviceInfos.Select(info => info.DeviceId.ToString()).Concat(
                                 //      Affected.CommunicatorDeviceInfos. Select(info => info.DeviceId.ToString())).
                                 //      AggregateWith(",")
                                 //    : "", UnitSeparator,
                                 //!Affected.IsEmpty()
                                 //    ? Affected.DefibrillatorDeviceInfos.Select(info => ShortModelNamespaces
                                 //                                                           ? info.Model.Substring(Math.Max(info.Model.LastIndexOf(".") + 1, 0))
                                 //                                                           : info.Model).AggregateWith(",")
                                 //    : "",                                                                                              UnitSeparator,
                                 Author.Name,                                                                                           UnitSeparator,
                                 AdditionalInfo.FirstText(),                                                                            UnitSeparator,
                                 firstResponse,                                                                                         UnitSeparator,
                                 age,                                                                                                   UnitSeparator,
                                 !Affected.IsEmpty()
                                     ? Affected.Organizations.Select(info => info.Message.Name.FirstText()).AggregateWith(",")
                                     : "",                                                                                              UnitSeparator,
                                 Priority.ToString(),                                                                                   UnitSeparator,
                                 Location.FirstText(),                                                                                  UnitSeparator,
                                 Id.ToString());

        }

        #endregion


        #region (private)  UpdateMyself     (NewServiceTicket)

        private ServiceTicket UpdateMyself(ServiceTicket NewServiceTicket)
        {

            //foreach (var pairing in _Pairings.Where(pairing => pairing.ServiceTicket.Id == Id))
            //    pairing.ServiceTicket = NewServiceTicket;

            return NewServiceTicket;

        }

        #endregion


        #region CopyAllLinkedDataFrom(OldServiceTicket)

        public override void CopyAllLinkedDataFromBase(ServiceTicket OldServiceTicket)
        {


        }

        #endregion


        #region Operator overloading

        #region Operator == (ServiceTicket1, ServiceTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicket1">A service ticket.</param>
        /// <param name="ServiceTicket2">Another service ticket.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ServiceTicket ServiceTicket1, ServiceTicket ServiceTicket2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ServiceTicket1, ServiceTicket2))
                return true;

            // If one is null, but not both, return false.
            if ((ServiceTicket1 is null) || (ServiceTicket2 is null))
                return false;

            return ServiceTicket1.Equals(ServiceTicket2);

        }

        #endregion

        #region Operator != (ServiceTicket1, ServiceTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicket1">A service ticket.</param>
        /// <param name="ServiceTicket2">Another service ticket.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ServiceTicket ServiceTicket1, ServiceTicket ServiceTicket2)
            => !(ServiceTicket1 == ServiceTicket2);

        #endregion

        #region Operator <  (ServiceTicket1, ServiceTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicket1">A service ticket.</param>
        /// <param name="ServiceTicket2">Another service ticket.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ServiceTicket ServiceTicket1, ServiceTicket ServiceTicket2)
        {

            if (ServiceTicket1 is null)
                throw new ArgumentNullException(nameof(ServiceTicket1), "The given ServiceTicket1 must not be null!");

            return ServiceTicket1.CompareTo(ServiceTicket2) < 0;

        }

        #endregion

        #region Operator <= (ServiceTicket1, ServiceTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicket1">A service ticket.</param>
        /// <param name="ServiceTicket2">Another service ticket.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ServiceTicket ServiceTicket1, ServiceTicket ServiceTicket2)
            => !(ServiceTicket1 > ServiceTicket2);

        #endregion

        #region Operator >  (ServiceTicket1, ServiceTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicket1">A service ticket.</param>
        /// <param name="ServiceTicket2">Another service ticket.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ServiceTicket ServiceTicket1, ServiceTicket ServiceTicket2)
        {

            if (ServiceTicket1 is null)
                throw new ArgumentNullException(nameof(ServiceTicket1), "The given ServiceTicket1 must not be null!");

            return ServiceTicket1.CompareTo(ServiceTicket2) > 0;

        }

        #endregion

        #region Operator >= (ServiceTicket1, ServiceTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicket1">A service ticket.</param>
        /// <param name="ServiceTicket2">Another service ticket.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ServiceTicket ServiceTicket1, ServiceTicket ServiceTicket2)
            => !(ServiceTicket1 < ServiceTicket2);

        #endregion

        #endregion

        #region IComparable<ServiceTicket> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public override Int32 CompareTo(Object Object)

            => Object is ServiceTicket ServiceTicket
                   ? CompareTo(ServiceTicket)
                   : throw new ArgumentException("The given object is not a service ticket!");

        #endregion

        #region CompareTo(ServiceTicket)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicket">An service ticket object to compare with.</param>
        public override Int32 CompareTo(ServiceTicket ServiceTicket)

            => ServiceTicket is null
                   ? throw new ArgumentNullException(nameof(ServiceTicket), "The given service ticket must not be null!")
                   : Id.CompareTo(ServiceTicket.Id);

        #endregion

        #endregion

        #region IEquatable<ServiceTicket> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is ServiceTicket ServiceTicket &&
                   Equals(ServiceTicket);

        #endregion

        #region Equals(ServiceTicket)

        /// <summary>
        /// Compares two service tickets for equality.
        /// </summary>
        /// <param name="ServiceTicket">An service ticket to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ServiceTicket ServiceTicket)

            => ServiceTicket is ServiceTicket &&
                   Id.Equals(ServiceTicket.Id);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Get the hash code of this object.
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


        #region ToBuilder(NewServiceTicketId = null)

        /// <summary>
        /// Return a builder for this service ticket.
        /// </summary>
        /// <param name="NewServiceTicketId">An optional new service ticket identification.</param>
        public Builder ToBuilder(ServiceTicket_Id? NewServiceTicketId = null)

            => ChangeSets.Any()

                   ? new Builder(Id,
                                 ChangeSets)

                   : new Builder(NewServiceTicketId ?? Id,
                                 JSONLDContext,

                                 ServiceTicketChangeSet_Id.Random(),
                                 Status.Timestamp,
                                 Author,
                                 Status.Value,
                                 Title,
                                 Affected,
                                 Priority,
                                 Location,
                                 GeoLocation,
                                 ProblemDescriptions,
                                 StatusIndicators,
                                 FirstResponse,
                                 Reactions,
                                 AdditionalInfo,
                                 CustomData,
                                 AttachedFiles,
                                 TicketReferences,
                                 DataLicenses,

                                 DataSource);

        #endregion

        #region (class) Builder

        /// <summary>
        /// An service ticket builder.
        /// </summary>
        public new class Builder : AEntity<ServiceTicket_Id,
                                           ServiceTicket>.Builder
        {

            #region Properties

            /// <summary>
            /// The unique identification of the service ticket.
            /// </summary>
            public ServiceTicket_Id                      Id              { get; set; }

            private readonly List<ServiceTicketChangeSet>  _ChangeSets;

            /// <summary>
            /// An enumeration of change sets.
            /// </summary>
            public IEnumerable<ServiceTicketChangeSet>  ChangeSets
                => _ChangeSets;

            /// <summary>
            /// An enumeration of usable data licenses for this service ticket.
            /// </summary>
            public HashSet<DataLicense>                  DataLicenses    { get; set; }

            /// <summary>
            /// The source of all this data, e.g. an automatic importer.
            /// </summary>
            public String                                DataSource      { get; set; }


            /// <summary>
            /// The initial author of this service ticket.
            /// </summary>
            public IUser Author
                => ChangeSets?.LastOrDefault()?.Author;

            /// <summary>
            /// The current status of the service ticket.
            /// </summary>
            public Timestamped<ServiceTicketStatusTypes>? Status
            {
                get
                {

                    var latestStatus = ChangeSets?.Where(entry => entry.Status.HasValue).
                                                FirstOrDefault();

                    return latestStatus != null
                               ? new Timestamped<ServiceTicketStatusTypes>(latestStatus.Timestamp,
                                                                           latestStatus.Status.Value)
                               : new Timestamped<ServiceTicketStatusTypes>?();

                }
            }

            /// <summary>
            /// The title of the service ticket (10-200 characters).
            /// </summary>
            public I18NString Title
                 => ChangeSets?.Where(entry => entry.Title.IsNotNullOrEmpty()).
                             FirstOrDefault()?.Title;

            /// <summary>
            /// Affected devices or services by this service ticket.
            /// </summary>
            public Affected Affected
                => ChangeSets?.Where(entry => entry.Affected != null).
                               FirstOrDefault()?.Affected;

            /// <summary>
            /// Whether the service ticket will be shown in (public) listings.
            /// </summary>
            public ServiceTicketPriorities Priority
                 => ChangeSets?.Where(entry => !entry.Priority.HasValue).
                                FirstOrDefault()?.Priority
                    ?? ServiceTicketPriorities.Normal;

            /// <summary>
            /// The location of the problem or broken device.
            /// </summary>
            public I18NString Location
                => ChangeSets?.Where(entry => entry.Location.IsNotNullOrEmpty()).
                               FirstOrDefault()?.Location;

            /// <summary>
            /// The geographical location of the problem or broken device.
            /// </summary>
            public GeoCoordinate? GeoLocation
                => ChangeSets?.Where(entry => !entry.GeoLocation.HasValue).
                               FirstOrDefault()?.GeoLocation;

            /// <summary>
            /// An enumeration of well-defined problem descriptions.
            /// </summary>
            public IEnumerable<ProblemDescriptionI18N> ProblemDescriptions
                => ChangeSets?.Where(entry => entry.ProblemDescriptions.SafeAny()).
                               FirstOrDefault()?.ProblemDescriptions;

            /// <summary>
            /// An enumeration of status indicators.
            /// </summary>
            public IEnumerable<Tag> StatusIndicators
                => ChangeSets?.Where(entry => entry.StatusIndicators.SafeAny()).
                               FirstOrDefault()?.StatusIndicators;

            /// <summary>
            /// The first official response to a service ticket.
            /// </summary>
            public FirstResponse? FirstResponse
                => ChangeSets?.Where(entry => entry.FirstResponse.HasValue).
                               LastOrDefault()?.FirstResponse;

            /// <summary>
            /// An enumeration of reactions.
            /// </summary>
            public IEnumerable<Tag> Reactions
                => ChangeSets?.Where(entry => entry.Reactions.SafeAny()).
                               FirstOrDefault()?.Reactions;

            /// <summary>
            /// Additional multi-language information related to this service ticket.
            /// </summary>
            public I18NString AdditionalInfo
                => ChangeSets?.Where(entry => entry.AdditionalInfo.IsNotNullOrEmpty()).
                            FirstOrDefault()?.AdditionalInfo;

            /// <summary>
            /// An enumeration of URLs to files attached to this service ticket change set.
            /// </summary>
            public IEnumerable<AttachedFile> AttachedFiles
                => ChangeSets?.Where(entry => entry.AttachedFiles.SafeAny()).
                            FirstOrDefault()?.AttachedFiles;

            /// <summary>
            /// References to other service tickets.
            /// </summary>
            public IEnumerable<ServiceTicketReference> TicketReferences
                => ChangeSets?.Where(entry => entry.TicketReferences.SafeAny()).
                            FirstOrDefault()?.TicketReferences;

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new service ticket builder.
            /// </summary>
            /// <param name="Id">The unique identification of the service ticket.</param>
            /// <param name="ChangeSets">An enumeration of service ticket change sets.</param>
            public Builder(ServiceTicket_Id                     Id,
                           IEnumerable<ServiceTicketChangeSet>  ChangeSets,
                           JSONLDContext?                       JSONLDContext = default)

                : base(Id,
                       JSONLDContext ?? DefaultJSONLDContext)

            {

                this.Id           = Id;
                this._ChangeSets  = ChangeSets != null ? new List<ServiceTicketChangeSet>(ChangeSets) : new List<ServiceTicketChangeSet>();

            }



            /// <summary>
            /// Create a new service ticket.
            /// </summary>
            /// <param name="Id">The unique identification of the service ticket.</param>
            /// 
            /// <param name="ServiceTicketChangeSetId">The unique identification of a service ticket change set.</param>
            /// <param name="Timestamp">The timestamp of the creation of this service ticket change set.</param>
            /// <param name="Author">The initial author of this service ticket change set (if known).</param>
            /// <param name="Status">An optional new service ticket status caused by this service ticket change set.</param>
            /// <param name="Title">The title of the service ticket (10-200 characters).</param>
            /// <param name="Affected">Affected devices or services by this service ticket.</param>
            /// <param name="Priority">The priority of the service ticket.</param>
            /// <param name="Location">The location of the problem or broken device.</param>
            /// <param name="GeoLocation">The geographical location of the problem or broken device.</param>
            /// <param name="ProblemDescriptions">An enumeration of well-defined problem descriptions.</param>
            /// <param name="StatusIndicators">An enumeration of problem indicators.</param>
            /// <param name="FirstResponse">The first official response to a service ticket.</param>
            /// <param name="Reactions">An enumeration of reactions.</param>
            /// <param name="AdditionalInfo">A multi-language description of the service ticket.</param>
            /// <param name="AttachedFiles">An enumeration of URLs to files attached to this service ticket.</param>
            /// <param name="TicketReferences">References to other service tickets.</param>
            /// <param name="DataLicenses">An enumeration of usable data licenses for this service ticket.</param>
            /// 
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            public Builder(ServiceTicket_Id?                     Id                         = null,
                           JSONLDContext?                        JSONLDContext              = null,

                           ServiceTicketChangeSet_Id?            ServiceTicketChangeSetId   = null,
                           DateTime?                             Timestamp                  = null,
                           IUser?                                Author                     = null,
                           ServiceTicketStatusTypes?             Status                     = null,
                           I18NString?                           Title                      = null,
                           Affected?                             Affected                   = null,
                           ServiceTicketPriorities?              Priority                   = null,
                           I18NString?                           Location                   = null,
                           GeoCoordinate?                        GeoLocation                = null,
                           IEnumerable<ProblemDescriptionI18N>?  ProblemDescriptions        = null,
                           IEnumerable<Tag>?                     StatusIndicators           = null,
                           FirstResponse?                        FirstResponse              = null,
                           IEnumerable<Tag>?                     Reactions                  = null,
                           I18NString?                           AdditionalInfo             = null,
                           JObject?                              CustomData                 = null,
                           IEnumerable<AttachedFile>?            AttachedFiles              = null,
                           IEnumerable<ServiceTicketReference>?  TicketReferences           = null,
                           IEnumerable<DataLicense>?             DataLicenses               = null,

                           String?                               DataSource                 = null)


                : this(Id ?? ServiceTicket_Id.Random(),
                       new List<ServiceTicketChangeSet>() {
                           new ServiceTicketChangeSet(
                               ServiceTicketChangeSetId ?? ServiceTicketChangeSet_Id.Random(),
                               JSONLDContext,

                               Timestamp                ?? org.GraphDefined.Vanaheimr.Illias.Timestamp.Now,
                               Author,
                               Status                   ?? ServiceTicketStatusTypes.New,
                               Title,
                               Affected,
                               Priority                 ?? ServiceTicketPriorities.Normal,
                               Location,
                               GeoLocation,
                               ProblemDescriptions,
                               StatusIndicators,
                               FirstResponse,
                               Reactions,
                               AdditionalInfo,
                               CustomData,
                               AttachedFiles,
                               TicketReferences,
                               DataLicenses,

                               null, // Comment
                               null, // InReplyTo
                               null, // CommentReferences

                               DataSource)
                       },
                       JSONLDContext)

            { }

            #endregion


            public void AppendChangeSet<TServiceTicketChangeSet>(TServiceTicketChangeSet ServiceTicketChangeSet)

                where TServiceTicketChangeSet : ServiceTicketChangeSet

            {

                this._ChangeSets.Add(ServiceTicketChangeSet);

            }



            #region CopyAllLinkedDataFrom(OldServiceTicket)

            public override void CopyAllLinkedDataFromBase(ServiceTicket OldServiceTicket)
            {


            }

            #endregion


            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the service ticket.
            /// </summary>
            public static implicit operator ServiceTicket(Builder Builder)

                => Builder?.ToImmutable;


            /// <summary>
            /// Return an immutable version of the service ticket.
            /// </summary>
            public ServiceTicket ToImmutable

                => ChangeSets.Any()

                       ? new ServiceTicket(Id,
                                           ChangeSets,
                                           JSONLDContext)

                       : new ServiceTicket(Id,
                                           JSONLDContext,

                                           Status?.Timestamp,
                                           Author,
                                           Status?.Value,
                                           Title,
                                           Affected,
                                           Priority,
                                           Location,
                                           GeoLocation,
                                           ProblemDescriptions,
                                           StatusIndicators,
                                           FirstResponse,
                                           Reactions,
                                           AdditionalInfo,
                                           CustomData,
                                           AttachedFiles,
                                           TicketReferences,
                                           DataLicenses,

                                           DataSource);

            #endregion


            #region Operator overloading

            #region Operator == (Builder1, Builder2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Builder1">A service ticket builder.</param>
            /// <param name="Builder2">Another service ticket builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator == (Builder Builder1, Builder Builder2)
            {

                // If both are null, or both are same instance, return true.
                if (Object.ReferenceEquals(Builder1, Builder2))
                    return true;

                // If one is null, but not both, return false.
                if ((Builder1 is null) || (Builder2 is null))
                    return false;

                return Builder1.Equals(Builder2);

            }

            #endregion

            #region Operator != (Builder1, Builder2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Builder1">A service ticket builder.</param>
            /// <param name="Builder2">Another service ticket builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator != (Builder Builder1, Builder Builder2)
                => !(Builder1 == Builder2);

            #endregion

            #region Operator <  (Builder1, Builder2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Builder1">A service ticket builder.</param>
            /// <param name="Builder2">Another service ticket builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator < (Builder Builder1, Builder Builder2)
            {

                if (Builder1 is null)
                    throw new ArgumentNullException(nameof(Builder1), "The given Builder1 must not be null!");

                return Builder1.CompareTo(Builder2) < 0;

            }

            #endregion

            #region Operator <= (Builder1, Builder2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Builder1">A service ticket builder.</param>
            /// <param name="Builder2">Another service ticket builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator <= (Builder Builder1, Builder Builder2)
                => !(Builder1 > Builder2);

            #endregion

            #region Operator >  (Builder1, Builder2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Builder1">A service ticket builder.</param>
            /// <param name="Builder2">Another service ticket builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator > (Builder Builder1, Builder Builder2)
            {

                if (Builder1 is null)
                    throw new ArgumentNullException(nameof(Builder1), "The given Builder1 must not be null!");

                return Builder1.CompareTo(Builder2) > 0;

            }

            #endregion

            #region Operator >= (Builder1, Builder2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Builder1">A service ticket builder.</param>
            /// <param name="Builder2">Another service ticket builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator >= (Builder Builder1, Builder Builder2)
                => !(Builder1 < Builder2);

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
                       : throw new ArgumentException("The given object is not a service ticket builder!");

            #endregion

            #region CompareTo(Builder)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Builder">An service ticket object to compare with.</param>
            public Int32 CompareTo(Builder Builder)

                => Builder is Builder
                       ? Id.CompareTo(Builder.Id)
                       : throw new ArgumentNullException(nameof(ServiceTicket), "The given service ticket builder must not be null!");

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

                => Object is ServiceTicket ServiceTicket &&
                       Equals(ServiceTicket);

            #endregion

            #region Equals(Builder)

            /// <summary>
            /// Compares two service tickets for equality.
            /// </summary>
            /// <param name="Builder">An service ticket to compare with.</param>
            /// <returns>True if both match; False otherwise.</returns>
            public Boolean Equals(Builder Builder)

                => Builder is Builder &&
                       Id.Equals(Builder.Id);

            #endregion

            #endregion

            #region (override) GetHashCode()

            /// <summary>
            /// Get the hash code of this object.
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

            public override bool Equals(ServiceTicket? other)
            {
                throw new NotImplementedException();
            }

            public override int CompareTo(ServiceTicket? other)
            {
                throw new NotImplementedException();
            }

            #endregion

        }

        #endregion

    }

}
