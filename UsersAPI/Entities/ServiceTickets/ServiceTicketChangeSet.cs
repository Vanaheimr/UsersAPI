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
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using social.OpenData.UsersAPI;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using System.Resources;

#endregion

namespace social.OpenData.UsersAPI
{

    public delegate Boolean ServiceTicketChangeSetProviderDelegate(ServiceTicketChangeSet_Id ServiceTicketChangeSetId, out ServiceTicketChangeSet ServiceTicketChangeSet);

    public delegate JObject ServiceTicketChangeSetToJSONDelegate(ServiceTicketChangeSet  ServiceTicketChangeSet,
                                                                    InfoStatus              ExpandDataLicenses     = InfoStatus.ShowIdOnly,
                                                                    InfoStatus              ExpandOwnerId          = InfoStatus.ShowIdOnly,
                                                                    InfoStatus              ExpandCommunicatorId   = InfoStatus.ShowIdOnly,
                                                                    Boolean                 IncludeCryptoHash      = true);


    /// <summary>
    /// Extention methods for the service ticket change set.
    /// </summary>
    public static partial class ServiceTicketChangeSetExtentions
    {

        #region ToJSON(this ServiceTicketChangeSets, Skip = null, Take = null, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of service ticket change sets.
        /// </summary>
        /// <param name="ServiceTicketChangeSets">An enumeration of service ticket change sets.</param>
        /// <param name="Skip">The optional number of service ticket change sets to skip.</param>
        /// <param name="Take">The optional number of service ticket change sets to return.</param>
        public static JArray ToJSON(this IEnumerable<ServiceTicketChangeSet>  ServiceTicketChangeSets,
                                    UInt64?                                   Skip                           = null,
                                    UInt64?                                   Take                           = null,
                                    InfoStatus                                ExpandDataLicenses             = InfoStatus.ShowIdOnly,
                                    InfoStatus                                ExpandOwnerId                  = InfoStatus.ShowIdOnly,
                                    InfoStatus                                ExpandCommunicatorId           = InfoStatus.ShowIdOnly,
                                    ServiceTicketChangeSetToJSONDelegate      ServiceTicketChangeSetToJSON   = null,
                                    Boolean                                   IncludeCryptoHash              = true)


            => ServiceTicketChangeSets?.Any() != true

                    ? new JArray()

                    : new JArray(ServiceTicketChangeSets.
                                    Where(history => history != null).
                                    SkipTakeFilter(Skip, Take).
                                    SafeSelect(serviceTicketChangeSet => ServiceTicketChangeSetToJSON != null
                                                                             ? ServiceTicketChangeSetToJSON (serviceTicketChangeSet,
                                                                                                             ExpandDataLicenses,
                                                                                                             ExpandOwnerId,
                                                                                                             ExpandCommunicatorId,
                                                                                                             IncludeCryptoHash)

                                                                             : serviceTicketChangeSet.ToJSON(ExpandDataLicenses,
                                                                                                             ExpandOwnerId,
                                                                                                             ExpandCommunicatorId,
                                                                                                             IncludeCryptoHash)));

        #endregion

    }

    /// <summary>
    /// A service ticket change set.
    /// </summary>
    public class ServiceTicketChangeSet : AEntity<ServiceTicketChangeSet_Id,
                                                  ServiceTicketChangeSet>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public new const String JSONLDContextText = "https://opendata.social/contexts/UsersAPI/serviceTicket/changeSet";

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public new readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse(JSONLDContextText);

        #endregion

        #region Properties

        /// <summary>
        /// The timestamp of this service ticket change set.
        /// </summary>
        public DateTime                                      Timestamp                   { get; }

        /// <summary>
        /// The author of this service ticket change set.
        /// </summary>
        public User                                          Author                      { get; }

        /// <summary>
        /// The status of the service ticket.
        /// </summary>
        public ServiceTicketStatusTypes?                     Status                      { get; }

        /// <summary>
        /// The title of the service ticket (10-200 characters).
        /// </summary>
        public I18NString                                    Title                       { get; }

        /// <summary>
        /// Affected devices or services by this service ticket.
        /// </summary>
        public Affected                                      Affected                    { get; internal set; }

        /// <summary>
        /// The priority of the service ticket.
        /// </summary>
        public ServiceTicketPriorities?                      Priority                    { get; }

        /// <summary>
        /// The location of the problem or broken device.
        /// </summary>
        public I18NString                                    Location                    { get; }

        /// <summary>
        /// The geographical location of the problem or broken device.
        /// </summary>
        public GeoCoordinate?                                GeoLocation                 { get; }

        /// <summary>
        /// An enumeration of well-defined problem descriptions.
        /// </summary>
        public IEnumerable<ProblemDescriptionI18N>           ProblemDescriptions         { get; }

        /// <summary>
        /// An enumeration of problem indicators.
        /// </summary>
        public IEnumerable<Tag>                              StatusIndicators            { get; }

        /// <summary>
        /// An enumeration of reactions.
        /// </summary>
        public IEnumerable<Tag>                              Reactions                   { get; }

        /// <summary>
        /// Additional multi-language information related to this service ticket.
        /// </summary>
        public I18NString                                    AdditionalInfo              { get; }

        /// <summary>
        /// An enumeration of URLs to files attached to this service ticket change set.
        /// </summary>
        public IEnumerable<AttachedFile>                     AttachedFiles               { get; }

        /// <summary>
        /// References to other service tickets.
        /// </summary>
        public IEnumerable<ServiceTicketReference>           TicketReferences            { get; }


        /// <summary>
        /// A multi-language comment.
        /// </summary>
        public I18NString                                    Comment                     { get; }

        /// <summary>
        /// This service ticket change set is a reply to the given history entry.
        /// </summary>
        public ServiceTicketChangeSet_Id?                    InReplyTo                   { get; }

        /// <summary>
        /// References to other service ticket change sets.
        /// </summary>
        public IEnumerable<ServiceTicketChangeSetReference>  CommentReferences           { get; }


        /// <summary>
        /// Optional data licsenses for publishing this data.
        /// </summary>
        public IEnumerable<DataLicense>                      DataLicenses                { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new service ticket change set.
        /// </summary>
        /// <param name="Id">The unique identification of this service ticket change set.</param>
        /// <param name="JSONLDContext">The JSON-LD context of this user group.</param>
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
        /// <param name="Reactions">An enumeration of reactions.</param>
        /// <param name="AdditionalInfo">A multi-language description of the service ticket.</param>
        /// <param name="AttachedFiles">An enumeration of URLs to files attached to this service ticket.</param>
        /// <param name="TicketReferences">References to other service tickets.</param>
        /// <param name="DataLicenses">Optional data licsenses for publishing this data.</param>
        /// 
        /// <param name="Comment">An optional multi-language comment.</param>
        /// <param name="InReplyTo">This service ticket change set is a reply to the given history entry.</param>
        /// <param name="CommentReferences">References to other service ticket change sets.</param>
        /// 
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        public ServiceTicketChangeSet(ServiceTicketChangeSet_Id?                    Id                    = null,
                                      JSONLDContext?                                JSONLDContext         = default,

                                      DateTime?                                     Timestamp             = null,
                                      User                                          Author                = null,
                                      ServiceTicketStatusTypes?                     Status                = null,
                                      I18NString                                    Title                 = null,
                                      Affected                                      Affected              = null,
                                      ServiceTicketPriorities?                      Priority              = null,
                                      I18NString                                    Location              = null,
                                      GeoCoordinate?                                GeoLocation           = null,
                                      IEnumerable<ProblemDescriptionI18N>           ProblemDescriptions   = null,
                                      IEnumerable<Tag>                              StatusIndicators      = null,
                                      IEnumerable<Tag>                              Reactions             = null,
                                      I18NString                                    AdditionalInfo        = null,
                                      JObject                                       CustomData            = default,
                                      IEnumerable<AttachedFile>                     AttachedFiles         = null,
                                      IEnumerable<ServiceTicketReference>           TicketReferences      = null,
                                      IEnumerable<DataLicense>                      DataLicenses          = null,

                                      I18NString                                    Comment               = null,
                                      ServiceTicketChangeSet_Id?                    InReplyTo             = null,
                                      IEnumerable<ServiceTicketChangeSetReference>  CommentReferences     = null,

                                      String                                        DataSource            = null)

            : base(Id            ?? ServiceTicketChangeSet_Id.Random(),
                   JSONLDContext ?? DefaultJSONLDContext,
                   CustomData,
                   DataSource)

        {

            this.Timestamp            = Timestamp           ?? DateTime.UtcNow;
            this.Author               = Author              ?? throw new ArgumentNullException(nameof(Author), "The given author must not be null!");
            this.Status               = Status;
            this.Title                = Title;
            this.Affected             = Affected;
            this.Priority             = Priority;
            this.Location             = Location;
            this.GeoLocation          = GeoLocation;
            this.ProblemDescriptions  = ProblemDescriptions != null ? ProblemDescriptions.Distinct() : Array.Empty<ProblemDescriptionI18N>();
            this.StatusIndicators     = StatusIndicators    != null ? StatusIndicators.   Distinct() : Array.Empty<Tag>();
            this.Reactions            = Reactions           ?? Array.Empty<Tag>();
            this.AdditionalInfo       = AdditionalInfo;
            this.AttachedFiles        = AttachedFiles       != null ? AttachedFiles.      Distinct() : Array.Empty<AttachedFile>();
            this.TicketReferences     = TicketReferences    != null ? TicketReferences.   Distinct() : Array.Empty<ServiceTicketReference>();
            this.DataLicenses         = DataLicenses        != null ? DataLicenses.       Distinct() : Array.Empty<DataLicense>();

            this.Comment              = Comment;
            this.InReplyTo            = InReplyTo;
            this.CommentReferences    = CommentReferences   != null ? CommentReferences.  Distinct() : Array.Empty<ServiceTicketChangeSetReference>();

        }

        #endregion


        #region ToJSON(...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        /// <param name="IncludeCryptoHash">Include the crypto hash value of this object.</param>
        public override JObject ToJSON(Boolean Embedded           = false,
                                       Boolean IncludeCryptoHash  = false)

            => ToJSON(ExpandAuthorId:        InfoStatus.ShowIdOnly,
                      ExpandReactions:       InfoStatus.ShowIdOnly,
                      ExpandDataLicenses:    InfoStatus.ShowIdOnly,
                      IncludeCryptoHash:     true);


        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeCryptoHash">Whether to include the cryptograhical hash value of this object.</param>
        public JObject ToJSON(InfoStatus       ExpandAuthorId       = InfoStatus.ShowIdOnly,
                              InfoStatus       ExpandReactions      = InfoStatus.ShowIdOnly,
                              InfoStatus       ExpandDataLicenses   = InfoStatus.ShowIdOnly,
                              Boolean          IncludeCryptoHash    = true,
                              Action<JObject>  Configurator         = null)
        {

            var JSON = JSONObject.Create(

                Id.ToJSON("@id"),

                new JProperty("@context", JSONLDContext.ToString()),

                new JProperty("timestamp",                  Timestamp.ToIso8601()),

                Author != null
                    ? ExpandAuthorId.Switch(
                          () => new JProperty("author",     new JObject(
                                                                new JProperty("@id",   Author.Id.ToString()),
                                                                new JProperty("name",  Author.Name)
                                                            )),
                          () => new JProperty("author",     Author.ToJSON()))
                    : null,

                Status.HasValue
                    ? new JProperty("status",               Status.Value.ToString())
                    : null,

                Title.IsNeitherNullNorEmpty()
                    ? new JProperty("title",                Title.ToJSON())
                    : null,

                Affected != null && !Affected.IsEmpty
                    ? new JProperty("affected",             Affected.ToJSON())
                    : null,

                Priority.HasValue
                    ? new JProperty("priority",             Priority.Value.ToString().ToLower())
                    : null,

                Location.IsNeitherNullNorEmpty()
                    ? Location.ToJSON("location")
                    : null,

                GeoLocation.HasValue
                    ? GeoLocation.Value.ToJSON("geoLocation")
                    : null,

                ProblemDescriptions.IsNeitherNullNorEmpty()
                       ? new JProperty("problemDescriptions",  new JArray(ProblemDescriptions.Select(problemDescription => problemDescription.ToJSON())))
                       : null,

                StatusIndicators.SafeAny()
                    ? new JProperty("statusIndicators",        new JArray(StatusIndicators.   Select(statusIndicator    => statusIndicator.Id.ToString())))
                    : null,

                Reactions.SafeAny()
                    ? ExpandReactions.Switch(
                          () => new JProperty("reactionIds",   new JArray(Reactions.          Select(tag                => tag.Id.ToString()))),
                          () => new JProperty("reactions",     new JArray(Reactions.          Select(tag                => tag.ToJSON()))))
                    : null,

                AdditionalInfo.IsNeitherNullNorEmpty()
                    ? AdditionalInfo.ToJSON("additionalInfo")
                    : null,

                AttachedFiles.SafeAny()
                    ? new JProperty("attachedFiles",           new JArray(AttachedFiles.      Select(attachedFile       => attachedFile.ToString())))
                    : null,

                TicketReferences.SafeAny()
                    ? new JProperty("ticketReferences",        new JArray(TicketReferences.   Select(references         => references.ToString())))
                    : null,



                Comment.IsNeitherNullNorEmpty()
                    ? Comment.ToJSON("comment")
                    : null,

                InReplyTo.HasValue
                    ? InReplyTo.Value.ToJSON("inReplyTo")
                    : null,

                CommentReferences.SafeAny()
                    ? new JProperty("commentReferences",       new JArray(CommentReferences.  Select(references         => references.ToString())))
                    : null,


                DataLicenses.SafeAny()
                    ? ExpandDataLicenses.Switch(
                          () => new JProperty("dataLicenseIds",  new JArray(DataLicenses.     Select(dataLicense        => dataLicense.Id.ToString()))),
                          () => new JProperty("dataLicenses",    DataLicenses.ToJSON()))
                    : null


                //IncludeCryptoHash
                //    ? new JProperty("cryptoHash",          CurrentCryptoHash)
                //    : null

            );

            Configurator?.Invoke(JSON);

            return JSON;

        }

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out ServiceTicketChangeSet, out ErrorResponse)

        /// <summary>
        /// Try to parse the given service ticket change set JSON.
        /// </summary>
        /// <param name="JSONObject">A JSON object.</param>
        /// <param name="ServiceTicketProvider">A delegate resolving service tickets.</param>
        /// <param name="UserProvider">A delegate resolving users.</param>
        /// <param name="OrganizationProvider">A delegate resolving organizations.</param>
        /// <param name="ServiceTicketChangeSet">The parsed service ticket change set.</param>
        /// <param name="ErrorResponse">An error message.</param>
        /// <param name="ServiceTicketChangeSetIdURL">An optional service ticket change set identification, e.g. from the HTTP URL.</param>
        /// <param name="OverwriteAuthor">Overwrite the author of the service ticket, if given.</param>
        /// <param name="DataSource">The source of this data.</param>
        public static Boolean TryParseJSON(JObject                        JSONObject,
                                           ServiceTicketProviderDelegate  ServiceTicketProvider,
                                           UserProviderDelegate           UserProvider,
                                           OrganizationProviderDelegate   OrganizationProvider,
                                           out ServiceTicketChangeSet     ServiceTicketChangeSet,
                                           out String                     ErrorResponse,
                                           JSONLDContext?                 VerifyContext                 = null,
                                           ServiceTicketChangeSet_Id?     ServiceTicketChangeSetIdURL   = null,
                                           OverwriteUserDelegate          OverwriteAuthor               = null,
                                           String                         DataSource                    = null)
        {

            try
            {

                ServiceTicketChangeSet = null;

                if (JSONObject?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse ServiceTicketChangeSetId    [optional]

                // Verify that a given service ticket change set identification
                //   is at least valid.
                if (JSONObject.ParseOptionalStruct("@id",
                                                   "service ticket change set identification",
                                                   ServiceTicketChangeSet_Id.TryParse,
                                                   out ServiceTicketChangeSet_Id? ServiceTicketChangeSetIdBody,
                                                   out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                if (!ServiceTicketChangeSetIdURL.HasValue && !ServiceTicketChangeSetIdBody.HasValue)
                    ServiceTicketChangeSetIdBody = ServiceTicketChangeSet_Id.Random();

                if (ServiceTicketChangeSetIdURL.HasValue && ServiceTicketChangeSetIdBody.HasValue && ServiceTicketChangeSetIdURL.Value != ServiceTicketChangeSetIdBody.Value)
                {
                    ErrorResponse = "The optional service ticket change set identification given within the JSON body does not match the one given in the URI!";
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

                if (!VerifyContext.HasValue)
                    VerifyContext = DefaultJSONLDContext;

                if (Context != VerifyContext.Value)
                {
                    ErrorResponse = @"The given JSON-LD ""@context"" information '" + Context + "' is not supported!";
                    return false;
                }

                #endregion

                #region Parse Timestamp                   [mandatory]

                if (!JSONObject.ParseMandatory("timestamp",
                                               "timestamp",
                                               out DateTime Timestamp,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Author                      [optional]

                User Author = null;

                if (JSONObject["author"] is JObject authorJSON &&
                    authorJSON.ParseOptionalStruct("@id",
                                                   "author identification",
                                                   User_Id.TryParse,
                                                   out User_Id? UserId,
                                                   out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                    if (UserId.HasValue &&
                       !UserProvider(UserId.Value, out Author))
                    {
                        ErrorResponse = "The given author '" + UserId + "' is unknown!";
                        return false;
                    }

                }

                if (OverwriteAuthor != null)
                    Author = OverwriteAuthor(Author);

                #endregion

                #region Parse Status                    [optional]

                if (JSONObject.ParseOptional("status",
                                             "service ticket status",
                                             ServiceTicketStatusTypes.TryParse,
                                             out ServiceTicketStatusTypes? Status,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Title                     [optional]

                if (JSONObject.ParseOptional("title",
                                             "title",
                                             out I18NString Title,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Affected                  [optional]

                Affected Affected = null;

                if (JSONObject.ParseOptional("affected",
                                             "affected things",
                                             out JObject AffectedJSON,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
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

                #region Parse Priority                  [optional]

                if (JSONObject.ParseOptionalEnum("priority",
                                                 "priority",
                                                 out ServiceTicketPriorities? Priority,
                                                 out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Location                  [optional]

                if (JSONObject.ParseOptional("location",
                                             "location",
                                             out I18NString Location,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse GeoLocation               [optional]

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

                #region Parse Problem descriptions      [optional]

                if (!JSONObject.ParseOptionalI18N("problemDescriptions",
                                                  "problem descriptions",
                                                  ProblemDescriptionI18N.TryParse,
                                                  out IEnumerable<ProblemDescriptionI18N> ProblemDescriptions,
                                                  out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Status indicators         [optional]

                var StatusIndicators = new HashSet<Tag>();

                if (JSONObject.ParseOptional("statusIndicators",
                                             "status indicators",
                                             out JArray statusIndicatorsJSON,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
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

                var Reactions          = new Tag[0];

                #region Parse Additional info           [optional]

                if (JSONObject.ParseOptional("additionalInfo",
                                             "additional information",
                                             out I18NString AdditionalInfo,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Attached files            [optional]

                if (JSONObject.ParseOptionalHashSet("attachedFiles",
                                                    "attached files",
                                                    AttachedFile.TryParseJSON,
                                                    out HashSet<AttachedFile> AttachedFiles,
                                                    out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                var TicketReferences   = new ServiceTicketReference[0];

                #region Parse DataLicenseIds            [optional]

                if (JSONObject.ParseOptional("dataLicenseIds",
                                             "data license identifications",
                                             DataLicense_Id.TryParse,
                                             out IEnumerable<DataLicense_Id> DataLicenseIds,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
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
                        //                   Date                       = DateTime.UtcNow,
                        //                   AccessControlAllowOrigin   = "*",
                        //                   AccessControlAllowMethods  = "GET, SET",
                        //                   AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                        //                   ETag                       = "1",
                        //                   ContentType                = HTTPContentType.JSON_UTF8,
                        //                   Content                    = JSONObject.Create(
                        //                                                    new JProperty("description", "The given data license '" + dataLicenseId + "' is unknown!")
                        //                                                ).ToUTF8Bytes(),
                        //                   Connection                 = "close"
                        //               }.AsImmutable());

                        //}

                        //DataLicenses.Add(DataLicense);

                    }

                }

                #endregion


                #region Parse Comment                   [optional]

                if (JSONObject.ParseOptional("comment",
                                             "comment",
                                             out I18NString Comment,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse InReplyTo                 [optional]

                if (JSONObject.ParseOptionalStruct("inReplyTo",
                                                   "in reply to",
                                                   ServiceTicketChangeSet_Id.TryParse,
                                                   out ServiceTicketChangeSet_Id? InReplyTo,
                                                   out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                var CommentReferences  = new ServiceTicketChangeSetReference[0];


                #region Parse CryptoHash                [optional]

                var CryptoHash    = JSONObject.GetOptional("cryptoHash");

                #endregion


                ServiceTicketChangeSet = new ServiceTicketChangeSet(ServiceTicketChangeSetIdBody ?? ServiceTicketChangeSetIdURL.Value,
                                                                    Context,

                                                                    Timestamp,
                                                                    Author,
                                                                    Status,
                                                                    Title,
                                                                    Affected,
                                                                    Priority,
                                                                    Location,
                                                                    GeoLocation,
                                                                    ProblemDescriptions,
                                                                    StatusIndicators,
                                                                    Reactions,
                                                                    AdditionalInfo,
                                                                    null,
                                                                    AttachedFiles,
                                                                    TicketReferences,
                                                                    DataLicenses,

                                                                    Comment,
                                                                    InReplyTo,
                                                                    CommentReferences,

                                                                    DataSource);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse  = e.Message;
                ServiceTicketChangeSet  = null;
                return false;
            }

        }

        #endregion


        #region (private)  UpdateMyself     (NewServiceTicketChangeSet)

        private ServiceTicketChangeSet UpdateMyself(ServiceTicketChangeSet NewServiceTicketChangeSet)
        {

            //foreach (var pairing in _Pairings.Where(pairing => pairing.ServiceTicketChangeSet.Id == Id))
            //    pairing.ServiceTicketChangeSet = NewServiceTicketChangeSet;

            return NewServiceTicketChangeSet;

        }

        #endregion


        #region CopyAllEdgesTo(Target)

        public override void CopyAllEdgesTo(ServiceTicketChangeSet Target)
        {


        }

        #endregion


        #region Operator overloading

        #region Operator == (ServiceTicketChangeSet1, ServiceTicketChangeSet2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSet1">A service ticket change set identification.</param>
        /// <param name="ServiceTicketChangeSet2">Another service ticket change set identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ServiceTicketChangeSet ServiceTicketChangeSet1, ServiceTicketChangeSet ServiceTicketChangeSet2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ServiceTicketChangeSet1, ServiceTicketChangeSet2))
                return true;

            // If one is null, but not both, return false.
            if ((ServiceTicketChangeSet1 is null) || (ServiceTicketChangeSet2 is null))
                return false;

            return ServiceTicketChangeSet1.Equals(ServiceTicketChangeSet2);

        }

        #endregion

        #region Operator != (ServiceTicketChangeSet1, ServiceTicketChangeSet2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSet1">A service ticket change set identification.</param>
        /// <param name="ServiceTicketChangeSet2">Another service ticket change set identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ServiceTicketChangeSet ServiceTicketChangeSet1, ServiceTicketChangeSet ServiceTicketChangeSet2)
            => !(ServiceTicketChangeSet1 == ServiceTicketChangeSet2);

        #endregion

        #region Operator <  (ServiceTicketChangeSet1, ServiceTicketChangeSet2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSet1">A service ticket change set identification.</param>
        /// <param name="ServiceTicketChangeSet2">Another service ticket change set identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ServiceTicketChangeSet ServiceTicketChangeSet1, ServiceTicketChangeSet ServiceTicketChangeSet2)
        {

            if (ServiceTicketChangeSet1 is null)
                throw new ArgumentNullException(nameof(ServiceTicketChangeSet1), "The given ServiceTicketChangeSet1 must not be null!");

            return ServiceTicketChangeSet1.CompareTo(ServiceTicketChangeSet2) < 0;

        }

        #endregion

        #region Operator <= (ServiceTicketChangeSet1, ServiceTicketChangeSet2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSet1">A service ticket change set identification.</param>
        /// <param name="ServiceTicketChangeSet2">Another service ticket change set identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ServiceTicketChangeSet ServiceTicketChangeSet1, ServiceTicketChangeSet ServiceTicketChangeSet2)
            => !(ServiceTicketChangeSet1 > ServiceTicketChangeSet2);

        #endregion

        #region Operator >  (ServiceTicketChangeSet1, ServiceTicketChangeSet2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSet1">A service ticket change set identification.</param>
        /// <param name="ServiceTicketChangeSet2">Another service ticket change set identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ServiceTicketChangeSet ServiceTicketChangeSet1, ServiceTicketChangeSet ServiceTicketChangeSet2)
        {

            if (ServiceTicketChangeSet1 is null)
                throw new ArgumentNullException(nameof(ServiceTicketChangeSet1), "The given ServiceTicketChangeSet1 must not be null!");

            return ServiceTicketChangeSet1.CompareTo(ServiceTicketChangeSet2) > 0;

        }

        #endregion

        #region Operator >= (ServiceTicketChangeSet1, ServiceTicketChangeSet2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSet1">A service ticket change set identification.</param>
        /// <param name="ServiceTicketChangeSet2">Another service ticket change set identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ServiceTicketChangeSet ServiceTicketChangeSet1, ServiceTicketChangeSet ServiceTicketChangeSet2)
            => !(ServiceTicketChangeSet1 < ServiceTicketChangeSet2);

        #endregion

        #endregion

        #region IComparable<ServiceTicketChangeSet> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public override Int32 CompareTo(Object Object)

            => Object is ServiceTicketChangeSet ServiceTicketChangeSet
                   ? CompareTo(ServiceTicketChangeSet)
                   : throw new ArgumentException("The given object is not a service ticket change set!", nameof(Object));

        #endregion

        #region CompareTo(ServiceTicketChangeSet)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSet">An service ticket change set object to compare with.</param>
        public override Int32 CompareTo(ServiceTicketChangeSet ServiceTicketChangeSet)

            => ServiceTicketChangeSet is ServiceTicketChangeSet
                   ? Id.CompareTo(ServiceTicketChangeSet.Id)
                   : throw new ArgumentException("The given object is not a service ticket change set!", nameof(ServiceTicketChangeSet));

        #endregion

        #endregion

        #region IEquatable<ServiceTicketChangeSet> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is ServiceTicketChangeSet ServiceTicketChangeSet &&
                  Equals(ServiceTicketChangeSet);

        #endregion

        #region Equals(ServiceTicketChangeSet)

        /// <summary>
        /// Compares two service ticket change sets for equality.
        /// </summary>
        /// <param name="ServiceTicketChangeSet">An service ticket change set to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(ServiceTicketChangeSet ServiceTicketChangeSet)

            => ServiceTicketChangeSet is ServiceTicketChangeSet &&
                   Id.Equals(ServiceTicketChangeSet.Id);

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


        #region ToBuilder(NewServiceTicketChangeSetId = null)

        /// <summary>
        /// Return a builder for this service ticket change set.
        /// </summary>
        /// <param name="NewServiceTicketChangeSetId">An optional new service ticket change set identification.</param>
        public Builder ToBuilder(ServiceTicketChangeSet_Id? NewServiceTicketChangeSetId = null)

            => new Builder(NewServiceTicketChangeSetId ?? Id,
                           JSONLDContext,

                           Timestamp,
                           Author,
                           Status,
                           Title,
                           Affected,
                           Priority,
                           Location,
                           GeoLocation,
                           ProblemDescriptions,
                           StatusIndicators,
                           Reactions,
                           AdditionalInfo,
                           CustomData,
                           AttachedFiles,
                           TicketReferences,
                           DataLicenses,

                           Comment,
                           InReplyTo,
                           CommentReferences,

                           DataSource);

        #endregion

        #region (class) Builder

        /// <summary>
        /// An service ticket change set builder.
        /// </summary>
        public new class Builder : AEntity<ServiceTicketChangeSet_Id,
                                           ServiceTicketChangeSet>.Builder
        {

            #region Properties

            /// <summary>
            /// The timestamp of this service ticket change set.
            /// </summary>
            public DateTime                                      Timestamp                 { get; set; }

            /// <summary>
            /// The initial author of this service ticket change set (if known).
            /// </summary>
            public User                                          Author                    { get; set; }

            /// <summary>
            /// The status of the service ticket.
            /// </summary>
            public ServiceTicketStatusTypes?                     Status                    { get; set; }

            /// <summary>
            /// The title of the service ticket (10-200 characters).
            /// </summary>
            public I18NString                                    Title                     { get; set; }

            /// <summary>
            /// Affected devices or services by this service ticket.
            /// </summary>
            public Affected.Builder                              Affected                  { get; set; }

            /// <summary>
            /// The priority of the service ticket.
            /// </summary>
            public ServiceTicketPriorities?                      Priority                  { get; set; }

            /// <summary>
            /// The location of the problem or broken device.
            /// </summary>
            public I18NString                                    Location                  { get; set; }

            /// <summary>
            /// The geographical location of the problem or broken device.
            /// </summary>
            public GeoCoordinate?                                GeoLocation               { get; set; }

            /// <summary>
            /// An enumeration of well-defined problem descriptions.
            /// </summary>
            public IEnumerable<ProblemDescriptionI18N>           ProblemDescriptions       { get; set; }

            /// <summary>
            /// An enumeration of problem indicators.
            /// </summary>
            public IEnumerable<Tag>                              StatusIndicators          { get; set; }

            /// <summary>
            /// An enumeration of reactions.
            /// </summary>
            public IEnumerable<Tag>                              Reactions                 { get; set; }

            /// <summary>
            /// Additional multi-language information related to this service ticket.
            /// </summary>
            public I18NString                                    AdditionalInfo            { get; set; }

            /// <summary>
            /// An enumeration of URLs to files attached to this service ticket change set.
            /// </summary>
            public IEnumerable<AttachedFile>                     AttachedFiles             { get; set; }

            /// <summary>
            /// References to other service tickets.
            /// </summary>
            public IEnumerable<ServiceTicketReference>           TicketReferences          { get; set; }


            /// <summary>
            /// A multi-language comment.
            /// </summary>
            public I18NString                                    Comment                   { get; set; }

            /// <summary>
            /// This service ticket change set is a reply to the given history entry.
            /// </summary>
            public ServiceTicketChangeSet_Id?                    InReplyTo                 { get; set; }

            /// <summary>
            /// References to other service ticket change sets.
            /// </summary>
            public IEnumerable<ServiceTicketChangeSetReference>  CommentReferences         { get; set; }


            /// <summary>
            /// Optional data licsenses for publishing this data.
            /// </summary>
            public IEnumerable<DataLicense>                      DataLicenses              { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new service ticket change set builder.
            /// </summary>
            /// <param name="Id">The unique identification of this service ticket change set.</param>
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
            /// <param name="Reactions">An enumeration of reactions.</param>
            /// <param name="AdditionalInfo">A multi-language description of the service ticket.</param>
            /// <param name="AttachedFiles">An enumeration of URLs to files attached to this service ticket.</param>
            /// <param name="TicketReferences">References to other service tickets.</param>
            /// <param name="DataLicenses">Optional data licsenses for publishing this data.</param>
            /// 
            /// <param name="Comment">An optional multi-language comment.</param>
            /// <param name="InReplyTo">This service ticket change set is a reply to the given history entry.</param>
            /// <param name="CommentReferences">References to other service ticket change sets.</param>
            /// 
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            public Builder(ServiceTicketChangeSet_Id?                    Id                    = null,
                           JSONLDContext?                                JSONLDContext         = default,

                           DateTime?                                     Timestamp             = null,
                           User                                          Author                = null,
                           ServiceTicketStatusTypes?                     Status                = null,
                           I18NString                                    Title                 = null,
                           Affected                                      Affected              = null,
                           ServiceTicketPriorities?                      Priority              = null,
                           I18NString                                    Location              = null,
                           GeoCoordinate?                                GeoLocation           = null,
                           IEnumerable<ProblemDescriptionI18N>           ProblemDescriptions   = null,
                           IEnumerable<Tag>                              StatusIndicators      = null,
                           IEnumerable<Tag>                              Reactions             = null,
                           I18NString                                    AdditionalInfo        = null,
                           JObject                                       CustomData            = default,
                           IEnumerable<AttachedFile>                     AttachedFiles         = null,
                           IEnumerable<ServiceTicketReference>           TicketReferences      = null,
                           IEnumerable<DataLicense>                      DataLicenses          = null,

                           I18NString                                    Comment               = null,
                           ServiceTicketChangeSet_Id?                    InReplyTo             = null,
                           IEnumerable<ServiceTicketChangeSetReference>  CommentReferences     = null,

                           String                                        DataSource            = null)

                : base(Id            ?? ServiceTicketChangeSet_Id.Random(),
                       JSONLDContext ?? DefaultJSONLDContext,
                       CustomData,
                       DataSource)

            {

                this.Timestamp            = Timestamp     ?? DateTime.UtcNow;
                this.Author               = Author        ?? throw new ArgumentNullException(nameof(Author), "The given author must not be null!");
                this.Status               = Status;
                this.Title                = Title;
                this.Affected             = Affected            != null ? Affected.ToBuilder()           : new Affected.Builder();
                this.Priority             = Priority;
                this.Location             = Location;
                this.GeoLocation          = GeoLocation;
                this.ProblemDescriptions  = ProblemDescriptions != null ? ProblemDescriptions.Distinct() : Array.Empty<ProblemDescriptionI18N>();
                this.StatusIndicators     = StatusIndicators    != null ? StatusIndicators.   Distinct() : Array.Empty<Tag>();
                this.Reactions            = Reactions        ?? Array.Empty<Tag>();
                this.AdditionalInfo       = AdditionalInfo;
                this.CustomData           = CustomData;
                this.AttachedFiles        = AttachedFiles       != null ? AttachedFiles.      Distinct() : Array.Empty<AttachedFile>();
                this.TicketReferences     = TicketReferences    != null ? TicketReferences.   Distinct() : Array.Empty<ServiceTicketReference>();
                this.DataLicenses         = DataLicenses  ?? Array.Empty<DataLicense>();

                this.Comment              = Comment;
                this.InReplyTo            = InReplyTo;
                this.CommentReferences    = CommentReferences != null ? CommentReferences.Distinct() : Array.Empty<ServiceTicketChangeSetReference>();

                this.DataSource           = DataSource;

            }

            #endregion


            #region CopyAllEdgesTo(Target)

            public override void CopyAllEdgesTo(ServiceTicketChangeSet Target)
            {


            }

            #endregion


            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the service ticket change set.
            /// </summary>
            public static implicit operator ServiceTicketChangeSet(Builder Builder)

                => Builder?.ToImmutable;


            /// <summary>
            /// Return an immutable version of the service ticket change set.
            /// </summary>
            public ServiceTicketChangeSet ToImmutable

                => new ServiceTicketChangeSet(Id,
                                              JSONLDContext,

                                              Timestamp,
                                              Author,
                                              Status,
                                              Title,
                                              Affected.ToImmutable,
                                              Priority,
                                              Location,
                                              GeoLocation,
                                              ProblemDescriptions,
                                              StatusIndicators,
                                              Reactions,
                                              AdditionalInfo,
                                              CustomData,
                                              AttachedFiles,
                                              TicketReferences,
                                              DataLicenses,

                                              Comment,
                                              InReplyTo,
                                              CommentReferences,

                                              DataSource);

            #endregion


            #region Operator overloading

            #region Operator == (Builder1, Builder2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Builder1">A service ticket change set builder.</param>
            /// <param name="Builder2">Another service ticket change set builder.</param>
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
            /// <param name="Builder1">A service ticket change set builder.</param>
            /// <param name="Builder2">Another service ticket change set builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator != (Builder Builder1, Builder Builder2)
                => !(Builder1 == Builder2);

            #endregion

            #region Operator <  (Builder1, Builder2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Builder1">A service ticket change set builder.</param>
            /// <param name="Builder2">Another service ticket change set builder.</param>
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
            /// <param name="Builder1">A service ticket change set builder.</param>
            /// <param name="Builder2">Another service ticket change set builder.</param>
            /// <returns>true|false</returns>
            public static Boolean operator <= (Builder Builder1, Builder Builder2)
                => !(Builder1 > Builder2);

            #endregion

            #region Operator >  (Builder1, Builder2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Builder1">A service ticket change set builder.</param>
            /// <param name="Builder2">Another service ticket change set builder.</param>
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
            /// <param name="Builder1">A service ticket change set builder.</param>
            /// <param name="Builder2">Another service ticket change set builder.</param>
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
                       : throw new ArgumentException("The given object is not a service ticket change set!", nameof(Object));

            #endregion

            #region CompareTo(Builder)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Builder">An service ticket change set object to compare with.</param>
            public Int32 CompareTo(Builder Builder)

                => Builder is Builder
                       ? Id.CompareTo(Builder.Id)
                       : throw new ArgumentException("The given object is not a service ticket change set!", nameof(Builder));

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
            /// Compares two service ticket change sets for equality.
            /// </summary>
            /// <param name="Builder">An service ticket change set to compare with.</param>
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
