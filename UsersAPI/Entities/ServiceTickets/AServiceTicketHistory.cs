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
using org.GraphDefined.Vanaheimr.Hermod.Distributed;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// A service ticket history entry.
    /// </summary>
    public abstract class AServiceTicketHistory : ADistributedEntity<ServiceTicketHistory_Id>,
                                                  IEntityClass<ServiceTicketHistory>
    {

        #region Properties

        /// <summary>
        /// The timestamp of this service ticket history entry.
        /// </summary>
        public DateTime                                    Timestamp                   { get; }

        /// <summary>
        /// The author of this service ticket history entry.
        /// </summary>
        public User                                        Author                      { get; }

        /// <summary>
        /// The status of the service ticket.
        /// </summary>
        public ServiceTicketStatusTypes?                   Status                      { get; }

        /// <summary>
        /// The title of the service ticket (10-200 characters).
        /// </summary>
        public I18NString                                  Title                       { get; }

        /// <summary>
        /// Affected devices or services by this service ticket.
        /// </summary>
        public Affected                                    Affected                    { get; }

        /// <summary>
        /// The priority of the service ticket.
        /// </summary>
        public ServiceTicketPriorities?                    Priority                    { get; }

        /// <summary>
        /// Whether the service ticket history entry will be shown in (public) listings.
        /// </summary>
        public PrivacyLevel?                               PrivacyLevel                { get; }

        /// <summary>
        /// The location of the problem or broken device.
        /// </summary>
        public I18NString                                  Location                    { get; }

        /// <summary>
        /// The geographical location of the problem or broken device.
        /// </summary>
        public GeoCoordinate?                              GeoLocation                 { get; }

        /// <summary>
        /// An enumeration of well-defined problem descriptions.
        /// </summary>
        public IEnumerable<Tag>                            ProblemDescriptions         { get; }

        /// <summary>
        /// An enumeration of problem indicators.
        /// </summary>
        public IEnumerable<Tag>                            StatusIndicators            { get; }

        /// <summary>
        /// An enumeration of reactions.
        /// </summary>
        public IEnumerable<Tag>                            Reactions                   { get; }

        /// <summary>
        /// Additional multi-language information related to this service ticket.
        /// </summary>
        public I18NString                                  AdditionalInfo              { get; }

        /// <summary>
        /// An enumeration of URLs to files attached to this service ticket history entry.
        /// </summary>
        public IEnumerable<HTTPPath>                       AttachedFiles               { get; }

        /// <summary>
        /// References to other service tickets.
        /// </summary>
        public IEnumerable<ServiceTicketReference>         TicketReferences            { get; }


        /// <summary>
        /// A multi-language comment.
        /// </summary>
        public I18NString                                  Comment                     { get; }

        /// <summary>
        /// This service ticket history entry is a reply to the given history entry.
        /// </summary>
        public ServiceTicketHistory_Id?                    InReplyTo                   { get; }

        /// <summary>
        /// References to other service ticket comments.
        /// </summary>
        public IEnumerable<ServiceTicketHistoryReference>  CommentReferences           { get; }


        /// <summary>
        /// Optional data licsenses for publishing this data.
        /// </summary>
        public IEnumerable<DataLicense>                    DataLicenses                { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new service ticket history entry.
        /// </summary>
        /// <param name="Id">The unique identification of this service ticket history entry.</param>
        /// <param name="Timestamp">The timestamp of the creation of this service ticket history entry.</param>
        /// <param name="Author">The initial author of this service ticket history entry (if known).</param>
        /// <param name="Status">An optional new service ticket status caused by this service ticket history entry.</param>
        /// <param name="Title">The title of the service ticket (10-200 characters).</param>
        /// <param name="Affected">Affected devices or services by this service ticket.</param>
        /// <param name="Priority">The priority of the service ticket.</param>
        /// <param name="PrivacyLevel">Whether the service ticket history entry will be shown in (public) listings.</param>
        /// <param name="Location">The location of the problem or broken device.</param>
        /// <param name="GeoLocation">The geographical location of the problem or broken device.</param>
        /// <param name="ProblemDescriptions">An enumeration of well-defined problem descriptions.</param>
        /// <param name="StatusIndicators">An enumeration of problem indicators.</param>
        /// <param name="Reactions">An enumeration of reactions.</param>
        /// <param name="AdditionalInfo">A multi-language description of the service ticket.</param>
        /// <param name="AttachedFiles">An enumeration of URLs to files attached to this service ticket.</param>
        /// <param name="TicketReferences">References to other service tickets.</param>
        /// 
        /// <param name="Comment">An optional multi-language comment.</param>
        /// <param name="InReplyTo">This service ticket history entry is a reply to the given history entry.</param>
        /// <param name="CommentReferences">References to other service ticket comments.</param>
        /// 
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        /// <param name="DataLicenses">Optional data licsenses for publishing this data.</param>
        public AServiceTicketHistory(ServiceTicketHistory_Id?                    Id                    = null,
                                     DateTime?                                   Timestamp             = null,
                                     User                                        Author                = null,
                                     ServiceTicketStatusTypes?                   Status                = null,
                                     I18NString                                  Title                 = null,
                                     Affected                                    Affected              = null,
                                     ServiceTicketPriorities?                    Priority              = null,
                                     PrivacyLevel?                               PrivacyLevel          = null,
                                     I18NString                                  Location              = null,
                                     GeoCoordinate?                              GeoLocation           = null,
                                     IEnumerable<Tag>                            ProblemDescriptions   = null,
                                     IEnumerable<Tag>                            StatusIndicators      = null,
                                     IEnumerable<Tag>                            Reactions             = null,
                                     I18NString                                  AdditionalInfo        = null,
                                     IEnumerable<HTTPPath>                       AttachedFiles         = null,
                                     IEnumerable<ServiceTicketReference>         TicketReferences      = null,

                                     I18NString                                  Comment               = null,
                                     ServiceTicketHistory_Id?                    InReplyTo             = null,
                                     IEnumerable<ServiceTicketHistoryReference>  CommentReferences     = null,

                                     IEnumerable<DataLicense>                    DataLicenses          = null,
                                     String                                      DataSource            = null)

            : base(Id ?? ServiceTicketHistory_Id.Random(),
                   DataSource)

        {

            this.Timestamp            = Timestamp        ?? DateTime.UtcNow;
            this.Author               = Author           ?? throw new ArgumentNullException(nameof(Author), "The given author must not be null!");
            this.Status               = Status;
            this.Title                = Title;
            this.Affected             = Affected;
            this.Priority             = Priority;
            this.PrivacyLevel         = PrivacyLevel;
            this.Location             = Location;
            this.GeoLocation          = GeoLocation;
            this.ProblemDescriptions  = ProblemDescriptions != null ? ProblemDescriptions.Distinct() : Array.Empty<Tag>();
            this.StatusIndicators     = StatusIndicators    != null ? StatusIndicators.   Distinct() : Array.Empty<Tag>();
            this.Reactions            = Reactions        ?? Array.Empty<Tag>();
            this.AdditionalInfo       = AdditionalInfo;
            this.AttachedFiles        = AttachedFiles       != null ? AttachedFiles.      Distinct() : Array.Empty<HTTPPath>();
            this.TicketReferences     = TicketReferences    != null ? TicketReferences.   Distinct() : Array.Empty<ServiceTicketReference>();

            this.Comment              = Comment;
            this.InReplyTo            = InReplyTo;
            this.CommentReferences    = CommentReferences   != null ? CommentReferences.  Distinct() : Array.Empty<ServiceTicketHistoryReference>();

            this.DataLicenses         = DataLicenses     ?? Array.Empty<DataLicense>();

        }

        #endregion


        #region (protected) ToJSON(...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeCryptoHash">Include the cryptograhical hash value of this object.</param>
        protected JObject ToJSON(InfoStatus       ExpandAuthorId       = InfoStatus.ShowIdOnly,
                                 InfoStatus       ExpandReactions      = InfoStatus.ShowIdOnly,
                                 InfoStatus       ExpandDataLicenses   = InfoStatus.ShowIdOnly,
                                 Boolean          IncludeCryptoHash    = true,
                                 Action<JObject>  Configurator         = null)
        {

            var JSON = JSONObject.Create(

                Id.ToJSON("@id"),

                new JProperty("timestamp",                   Timestamp.ToIso8601()),

                Author != null
                    ? ExpandAuthorId.Switch(
                          () => new JProperty("author",     new JObject(
                                                                new JProperty("id",   Author.Id.ToString()),
                                                                new JProperty("name", Author.Name)
                                                            )),
                          () => new JProperty("author",     Author.ToJSON()))
                    : null,

                Status.HasValue
                    ? new JProperty("status",                Status.Value.ToString())
                    : null,

                Title.IsNeitherNullNorEmpty()
                    ? new JProperty("title",                 Title.ToJSON())
                    : null,

                Affected != null && !Affected.IsEmpty
                    ? new JProperty("affected",              Affected.ToJSON())
                    : null,

                Priority.HasValue
                    ? new JProperty("priority",              Priority.Value.ToString().ToLower())
                    : null,

                PrivacyLevel.HasValue
                    ? PrivacyLevel.Value.ToJSON()
                    : null,

                Location.IsNeitherNullNorEmpty()
                    ? Location.ToJSON("location")
                    : null,

                GeoLocation.HasValue
                    ? PrivacyLevel.Value.ToJSON("geoLocation")
                    : null,

                ProblemDescriptions.SafeAny()
                    ? new JProperty("problemDescriptions",  new JArray(ProblemDescriptions.SafeSelect(tag => tag.Id.ToString())))
                    : null,

                StatusIndicators.SafeAny()
                    ? new JProperty("statusIndicators",     new JArray(StatusIndicators.SafeSelect(tag => tag.Id.ToString())))
                    : null,

                Reactions.SafeAny()
                    ? ExpandReactions.Switch(
                          () => new JProperty("reactionIds",  new JArray(Reactions.Select(tag => tag.Id.ToString()))),
                          () => new JProperty("reactions",    new JArray(Reactions.Select(tag => tag.ToJSON()))))
                    : null,

                AdditionalInfo.IsNeitherNullNorEmpty()
                    ? AdditionalInfo.ToJSON("additionalInfo")
                    : null,

                AttachedFiles.SafeAny()
                    ? new JProperty("attachedFiles",          new JArray(AttachedFiles.Select(uri => uri.ToString())))
                    : null,

                TicketReferences.SafeAny()
                    ? new JProperty("ticketReferences", new JArray(TicketReferences.Select(references => references.ToString())))
                    : null,



                Comment.IsNeitherNullNorEmpty()
                    ? Comment.ToJSON("comment")
                    : null,

                InReplyTo.HasValue
                    ? InReplyTo.Value.ToJSON("inReplyTo")
                    : null,

                CommentReferences.SafeAny()
                    ? new JProperty("commentReferences", new JArray(CommentReferences.Select(references => references.ToString())))
                    : null,


                DataLicenses.SafeAny()
                    ? ExpandDataLicenses.Switch(
                          () => new JProperty("dataLicenseIds",  new JArray(DataLicenses.SafeSelect(license => license.Id.ToString()))),
                          () => new JProperty("dataLicenses",    DataLicenses.ToJSON()))
                    : null,


                IncludeCryptoHash
                    ? new JProperty("cryptoHash",          CurrentCryptoHash)
                    : null

            );

            Configurator?.Invoke(JSON);

            return JSON;

        }

        #endregion

        #region (protected static) _TryParseJSON(JSONObject, ..., out ServiceTicketHistory, out ErrorResponse)

        /// <summary>
        /// Try to parse the given service ticket history entry JSON.
        /// </summary>
        /// <param name="JSONObject">A JSON object.</param>
        /// <param name="ServiceTicketProvider">A delegate resolving service tickets.</param>
        /// <param name="UserProvider">A delegate resolving users.</param>
        /// <param name="OrganizationProvider">A delegate resolving organizations.</param>
        /// <param name="ServiceTicketHistory">The parsed service ticket history entry.</param>
        /// <param name="ErrorResponse">An error message.</param>
        /// <param name="ServiceTicketHistoryIdURI">The optional service ticket history entry identification, e.g. from the HTTP URI.</param>
        protected static Boolean _TryParseJSON(JObject                         JSONObject,
                                               AServiceTicketProviderDelegate  ServiceTicketProvider,
                                               UserProviderDelegate            UserProvider,
                                               OrganizationProviderDelegate    OrganizationProvider,
                                               out ServiceTicketHistory        ServiceTicketHistory,
                                               out String                      ErrorResponse,
                                               ServiceTicketHistory_Id?        ServiceTicketHistoryIdURI = null)
        {

            try
            {

                ServiceTicketHistory = null;

                if (JSONObject?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse ServiceTicketId           [mandatory]

                //if (!JSONObject.ParseMandatory("serviceTicketId",
                //                               "service ticket history entry identification",
                //                               ServiceTicket_Id.TryParse,
                //                               out ServiceTicket_Id ServiceTicketId,
                //                               out ErrorResponse))
                //{

                //    if (ErrorResponse != null)
                //        return false;

                //}

                #endregion

                #region Parse ServiceTicketHistoryId    [optional]

                // Verify that a given service ticket history entry identification
                //   is at least valid.
                if (JSONObject.ParseOptionalStruct("@id",
                                                   "service ticket history entry identification",
                                                   ServiceTicketHistory_Id.TryParse,
                                                   out ServiceTicketHistory_Id? ServiceTicketHistoryIdBody,
                                                   out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                if (!ServiceTicketHistoryIdURI.HasValue && !ServiceTicketHistoryIdBody.HasValue)
                    ServiceTicketHistoryIdBody = ServiceTicketHistory_Id.Random();

                if (ServiceTicketHistoryIdURI.HasValue && ServiceTicketHistoryIdBody.HasValue && ServiceTicketHistoryIdURI.Value != ServiceTicketHistoryIdBody.Value)
                {
                    ErrorResponse = "The optional service ticket history entry identification given within the JSON body does not match the one given in the URI!";
                    return false;
                }

                #endregion

                #region Parse Timestamp                 [mandatory]

                if (!JSONObject.ParseMandatory("timestamp",
                                               "timestamp",
                                               out DateTime Timestamp,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Author                    [optional]

                User Author = null;

                if (JSONObject.ParseOptionalStruct("authorId",
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

                #region Parse PrivacyLevel              [optional]

                if (JSONObject.ParseOptionalStruct("privacyLevel",
                                                   "privacy level",
                                                   social.OpenData.UsersAPI.JSON_IO.TryParsePrivacyLevel,
                                                   out PrivacyLevel? PrivacyLevel,
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

                var ProblemDescriptions = new HashSet<Tag>();

                if (JSONObject.ParseOptional("problemDescriptions",
                                             "problem descriptions",
                                             out JArray ProblemDescriptionsJSON,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                    if (ProblemDescriptionsJSON != null)
                    {

                        var text = "";

                        foreach (var problemDescription in ProblemDescriptionsJSON)
                        {

                            text = problemDescription.Value<String>();

                            if (text.IsNotNullOrEmpty())
                            {
                                text = text.Trim();
                                if (text != "")
                                    ProblemDescriptions.Add(new Tag(Tag_Id.Parse(text)));
                            }

                        }

                    }

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
                                                    HTTPPath.TryParse,
                                                    out HashSet<HTTPPath> AttachedFiles,
                                                    out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                var TicketReferences   = new ServiceTicketReference[0];


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
                                                   ServiceTicketHistory_Id.TryParse,
                                                   out ServiceTicketHistory_Id? InReplyTo,
                                                   out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                var CommentReferences  = new ServiceTicketHistoryReference[0];


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


                #region Parse CryptoHash                [optional]

                var CryptoHash    = JSONObject.GetOptional("cryptoHash");

                #endregion


                ServiceTicketHistory = new ServiceTicketHistory(ServiceTicketHistoryIdBody ?? ServiceTicketHistoryIdURI.Value,
                                                                Timestamp,
                                                                Author,
                                                                Status,
                                                                Title,
                                                                Affected,
                                                                Priority,
                                                                PrivacyLevel,
                                                                Location,
                                                                GeoLocation,
                                                                ProblemDescriptions,
                                                                StatusIndicators,
                                                                Reactions,
                                                                AdditionalInfo,
                                                                AttachedFiles,
                                                                TicketReferences,

                                                                Comment,
                                                                InReplyTo,
                                                                CommentReferences,

                                                                DataLicenses);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse  = e.Message;
                ServiceTicketHistory  = null;
                return false;
            }

        }

        #endregion


        #region (private)  UpdateMyself     (NewServiceTicketHistory)

        private ServiceTicketHistory UpdateMyself(ServiceTicketHistory NewServiceTicketHistory)
        {

            //foreach (var pairing in _Pairings.Where(pairing => pairing.ServiceTicketHistory.Id == Id))
            //    pairing.ServiceTicketHistory = NewServiceTicketHistory;

            return NewServiceTicketHistory;

        }

        #endregion


        #region CopyAllEdgesTo(Target)

        public void CopyAllEdgesTo(ServiceTicketHistory Target)
        {


        }

        #endregion


        #region Operator overloading

        #region Operator == (AServiceTicketHistory1, AServiceTicketHistory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AServiceTicketHistory1">A service ticket history entry identification.</param>
        /// <param name="AServiceTicketHistory2">Another service ticket history entry identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AServiceTicketHistory AServiceTicketHistory1, AServiceTicketHistory AServiceTicketHistory2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(AServiceTicketHistory1, AServiceTicketHistory2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) AServiceTicketHistory1 == null) || ((Object) AServiceTicketHistory2 == null))
                return false;

            return AServiceTicketHistory1.Equals(AServiceTicketHistory2);

        }

        #endregion

        #region Operator != (AServiceTicketHistory1, AServiceTicketHistory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AServiceTicketHistory1">A service ticket history entry identification.</param>
        /// <param name="AServiceTicketHistory2">Another service ticket history entry identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AServiceTicketHistory AServiceTicketHistory1, AServiceTicketHistory AServiceTicketHistory2)
            => !(AServiceTicketHistory1 == AServiceTicketHistory2);

        #endregion

        #region Operator <  (AServiceTicketHistory1, AServiceTicketHistory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AServiceTicketHistory1">A service ticket history entry identification.</param>
        /// <param name="AServiceTicketHistory2">Another service ticket history entry identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (AServiceTicketHistory AServiceTicketHistory1, AServiceTicketHistory AServiceTicketHistory2)
        {

            if ((Object) AServiceTicketHistory1 == null)
                throw new ArgumentNullException(nameof(AServiceTicketHistory1), "The given AServiceTicketHistory1 must not be null!");

            return AServiceTicketHistory1.CompareTo(AServiceTicketHistory2) < 0;

        }

        #endregion

        #region Operator <= (AServiceTicketHistory1, AServiceTicketHistory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AServiceTicketHistory1">A service ticket history entry identification.</param>
        /// <param name="AServiceTicketHistory2">Another service ticket history entry identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (AServiceTicketHistory AServiceTicketHistory1, AServiceTicketHistory AServiceTicketHistory2)
            => !(AServiceTicketHistory1 > AServiceTicketHistory2);

        #endregion

        #region Operator >  (AServiceTicketHistory1, AServiceTicketHistory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AServiceTicketHistory1">A service ticket history entry identification.</param>
        /// <param name="AServiceTicketHistory2">Another service ticket history entry identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (AServiceTicketHistory AServiceTicketHistory1, AServiceTicketHistory AServiceTicketHistory2)
        {

            if ((Object) AServiceTicketHistory1 == null)
                throw new ArgumentNullException(nameof(AServiceTicketHistory1), "The given AServiceTicketHistory1 must not be null!");

            return AServiceTicketHistory1.CompareTo(AServiceTicketHistory2) > 0;

        }

        #endregion

        #region Operator >= (AServiceTicketHistory1, AServiceTicketHistory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AServiceTicketHistory1">A service ticket history entry identification.</param>
        /// <param name="AServiceTicketHistory2">Another service ticket history entry identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (AServiceTicketHistory AServiceTicketHistory1, AServiceTicketHistory AServiceTicketHistory2)
            => !(AServiceTicketHistory1 < AServiceTicketHistory2);

        #endregion

        #endregion

        #region IComparable<ServiceTicketHistory> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is ServiceTicketHistory ServiceTicketHistory))
                throw new ArgumentException("The given object is not an service ticket history entry!");

            return CompareTo(ServiceTicketHistory);

        }

        #endregion

        #region CompareTo(ServiceTicketHistory)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketHistory">An service ticket history entry object to compare with.</param>
        public Int32 CompareTo(ServiceTicketHistory ServiceTicketHistory)
        {

            if ((Object) ServiceTicketHistory == null)
                throw new ArgumentNullException(nameof(ServiceTicketHistory), "The given service ticket history entry must not be null!");

            return Id.CompareTo(ServiceTicketHistory.Id);

        }

        #endregion

        #endregion

        #region IEquatable<ServiceTicketHistory> Members

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

            var ServiceTicketHistory = Object as ServiceTicketHistory;
            if ((Object) ServiceTicketHistory == null)
                return false;

            return Equals(ServiceTicketHistory);

        }

        #endregion

        #region Equals(ServiceTicketHistory)

        /// <summary>
        /// Compares two service ticket history entrys for equality.
        /// </summary>
        /// <param name="ServiceTicketHistory">An service ticket history entry to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ServiceTicketHistory ServiceTicketHistory)
        {

            if ((Object) ServiceTicketHistory == null)
                return false;

            return Id.Equals(ServiceTicketHistory.Id);

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


        #region ToBuilder(NewServiceTicketHistoryId = null)

        /// <summary>
        /// Return a builder for this service ticket history entry.
        /// </summary>
        /// <param name="NewServiceTicketHistoryId">An optional new service ticket history entry identification.</param>
        public Builder ToBuilder(ServiceTicketHistory_Id? NewServiceTicketHistoryId = null)

            => new Builder(NewServiceTicketHistoryId ?? Id,
                           Timestamp,
                           Author,
                           Status,
                           Title,
                           Affected,
                           Priority,
                           PrivacyLevel,
                           Location,
                           GeoLocation,
                           ProblemDescriptions,
                           StatusIndicators,
                           Reactions,
                           AdditionalInfo,
                           AttachedFiles,
                           TicketReferences,

                           Comment,
                           InReplyTo,
                           CommentReferences,

                           DataLicenses,
                           DataSource);

        #endregion

        #region (class) Builder

        /// <summary>
        /// An service ticket history entry builder.
        /// </summary>
        public class Builder
        {

            #region Properties

            /// <summary>
            /// The service ticket history entry identification.
            /// </summary>
            public ServiceTicketHistory_Id                     Id                        { get; set; }

            /// <summary>
            /// The timestamp of this service ticket history entry.
            /// </summary>
            public DateTime                                    Timestamp                 { get; set; }

            /// <summary>
            /// The initial author of this service ticket history entry (if known).
            /// </summary>
            public User                                        Author                    { get; set; }

            /// <summary>
            /// The status of the service ticket.
            /// </summary>
            public ServiceTicketStatusTypes?                   Status                    { get; set; }

            /// <summary>
            /// The title of the service ticket (10-200 characters).
            /// </summary>
            public I18NString                                  Title                     { get; set; }

            /// <summary>
            /// Affected devices or services by this service ticket.
            /// </summary>
            public Affected                                    Affected                  { get; set; }

            /// <summary>
            /// The priority of the service ticket.
            /// </summary>
            public ServiceTicketPriorities?                    Priority                  { get; set; }

            /// <summary>
            /// Whether the service ticket history entry will be shown in (public) listings.
            /// </summary>
            public PrivacyLevel?                               PrivacyLevel              { get; set; }

            /// <summary>
            /// The location of the problem or broken device.
            /// </summary>
            public I18NString                                  Location                  { get; set; }

            /// <summary>
            /// The geographical location of the problem or broken device.
            /// </summary>
            public GeoCoordinate?                              GeoLocation               { get; set; }

            /// <summary>
            /// An enumeration of well-defined problem descriptions.
            /// </summary>
            public IEnumerable<Tag>                            ProblemDescriptions       { get; set; }

            /// <summary>
            /// An enumeration of problem indicators.
            /// </summary>
            public IEnumerable<Tag>                            StatusIndicators          { get; set; }

            /// <summary>
            /// An enumeration of reactions.
            /// </summary>
            public IEnumerable<Tag>                            Reactions                 { get; set; }

            /// <summary>
            /// Additional multi-language information related to this service ticket.
            /// </summary>
            public I18NString                                  AdditionalInfo            { get; set; }

            /// <summary>
            /// An enumeration of URLs to files attached to this service ticket history entry.
            /// </summary>
            public IEnumerable<HTTPPath>                       AttachedFiles             { get; set; }

            /// <summary>
            /// References to other service tickets.
            /// </summary>
            public IEnumerable<ServiceTicketReference>         TicketReferences          { get; set; }


            /// <summary>
            /// A multi-language comment.
            /// </summary>
            public I18NString                                  Comment                   { get; set; }

            /// <summary>
            /// This service ticket history entry is a reply to the given history entry.
            /// </summary>
            public ServiceTicketHistory_Id?                    InReplyTo                 { get; set; }

            /// <summary>
            /// References to other service ticket comments.
            /// </summary>
            public IEnumerable<ServiceTicketHistoryReference>  CommentReferences         { get; set; }


            /// <summary>
            /// Optional data licsenses for publishing this data.
            /// </summary>
            public IEnumerable<DataLicense>                    DataLicenses              { get; set; }

            /// <summary>
            /// The source of this information, e.g. an automatic importer.
            /// </summary>
            public String                                      DataSource                { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new service ticket history entry builder.
            /// </summary>
            /// <param name="Id">The unique identification of this service ticket history entry.</param>
            /// <param name="Timestamp">The timestamp of the creation of this service ticket history entry.</param>
            /// <param name="Author">The initial author of this service ticket history entry (if known).</param>
            /// <param name="Status">An optional new service ticket status caused by this service ticket history entry.</param>
            /// <param name="Title">The title of the service ticket (10-200 characters).</param>
            /// <param name="Affected">Affected devices or services by this service ticket.</param>
            /// <param name="Priority">The priority of the service ticket.</param>
            /// <param name="PrivacyLevel">Whether the service ticket history entry will be shown in (public) listings.</param>
            /// <param name="Location">The location of the problem or broken device.</param>
            /// <param name="GeoLocation">The geographical location of the problem or broken device.</param>
            /// <param name="ProblemDescriptions">An enumeration of well-defined problem descriptions.</param>
            /// <param name="StatusIndicators">An enumeration of problem indicators.</param>
            /// <param name="Reactions">An enumeration of reactions.</param>
            /// <param name="AdditionalInfo">A multi-language description of the service ticket.</param>
            /// <param name="AttachedFiles">An enumeration of URLs to files attached to this service ticket.</param>
            /// <param name="TicketReferences">References to other service tickets.</param>
            /// 
            /// <param name="Comment">An optional multi-language comment.</param>
            /// <param name="InReplyTo">This service ticket history entry is a reply to the given history entry.</param>
            /// <param name="CommentReferences">References to other service ticket comments.</param>
            /// 
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            /// <param name="DataLicenses">Optional data licsenses for publishing this data.</param>
            public Builder(ServiceTicketHistory_Id?                    Id                    = null,
                           DateTime?                                   Timestamp             = null,
                           User                                        Author                = null,
                           ServiceTicketStatusTypes?                   Status                = null,
                           I18NString                                  Title                 = null,
                           Affected                                    Affected              = null,
                           ServiceTicketPriorities?                    Priority              = null,
                           PrivacyLevel?                               PrivacyLevel          = null,
                           I18NString                                  Location              = null,
                           GeoCoordinate?                              GeoLocation           = null,
                           IEnumerable<Tag>                            ProblemDescriptions   = null,
                           IEnumerable<Tag>                            StatusIndicators      = null,
                           IEnumerable<Tag>                            Reactions             = null,
                           I18NString                                  AdditionalInfo        = null,
                           IEnumerable<HTTPPath>                       AttachedFiles         = null,
                           IEnumerable<ServiceTicketReference>         TicketReferences      = null,

                           I18NString                                  Comment               = null,
                           ServiceTicketHistory_Id?                    InReplyTo             = null,
                           IEnumerable<ServiceTicketHistoryReference>  CommentReferences     = null,

                           IEnumerable<DataLicense>                    DataLicenses          = null,
                           String                                      DataSource            = null)
            {

                this.Id                   = Id            ?? ServiceTicketHistory_Id.Random();
                this.Timestamp            = Timestamp     ?? DateTime.UtcNow;
                this.Author               = Author        ?? throw new ArgumentNullException(nameof(Author), "The given author must not be null!");
                this.Status               = Status;
                this.Title                = Title;
                this.Affected             = Affected;
                this.Priority             = Priority;
                this.PrivacyLevel         = PrivacyLevel;
                this.Location             = Location;
                this.GeoLocation          = GeoLocation;
                this.ProblemDescriptions  = ProblemDescriptions != null ? ProblemDescriptions.Distinct() : Array.Empty<Tag>();
                this.StatusIndicators     = StatusIndicators    != null ? StatusIndicators.   Distinct() : Array.Empty<Tag>();
                this.Reactions            = Reactions        ?? Array.Empty<Tag>();
                this.AdditionalInfo       = AdditionalInfo;
                this.AttachedFiles        = AttachedFiles       != null ? AttachedFiles.      Distinct() : Array.Empty<HTTPPath>();
                this.TicketReferences     = TicketReferences    != null ? TicketReferences.   Distinct() : Array.Empty<ServiceTicketReference>();

                this.Comment              = Comment;
                this.InReplyTo            = InReplyTo;
                this.CommentReferences    = CommentReferences != null ? CommentReferences.Distinct() : Array.Empty<ServiceTicketHistoryReference>();

                this.DataLicenses         = DataLicenses  ?? Array.Empty<DataLicense>();
                this.DataSource           = DataSource;

            }

            #endregion

            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the service ticket history entry.
            /// </summary>
            public static implicit operator ServiceTicketHistory(Builder Builder)

                => Builder?.ToImmutable;


            /// <summary>
            /// Return an immutable version of the service ticket history entry.
            /// </summary>
            public ServiceTicketHistory ToImmutable

                => new ServiceTicketHistory(Id,
                                            Timestamp,
                                            Author,
                                            Status,
                                            Title,
                                            Affected,
                                            Priority,
                                            PrivacyLevel,
                                            Location,
                                            GeoLocation,
                                            ProblemDescriptions,
                                            StatusIndicators,
                                            Reactions,
                                            AdditionalInfo,
                                            AttachedFiles,
                                            TicketReferences,

                                            Comment,
                                            InReplyTo,
                                            CommentReferences,

                                            DataLicenses,
                                            DataSource);

            #endregion

        }

        #endregion

    }

}
