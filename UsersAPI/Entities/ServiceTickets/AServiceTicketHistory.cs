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

        #region Data

        /// <summary>
        /// The default max size of the data set status list.
        /// </summary>
        public const UInt16  DefaultMaxStatusListSize   = 15;

        #endregion

        #region Properties

        #region API

        private Object _API;

        /// <summary>
        /// The CardiCloudAPI of this service ticket history entry.
        /// </summary>
        internal Object API
        {

            get
            {
                return _API;
            }

            set
            {

                if (_API != null)
                    throw new ArgumentException("Illegal attempt to change the API of this service ticket history entry!");

                _API = value ?? throw new ArgumentException("Illegal attempt to delete the API reference of this service ticket history entry!");

            }

        }

        #endregion


        public ServiceTicket_Id           ServiceTicketId           { get; }

        public DateTime                   Timestamp                 { get; }

        /// <summary>
        /// The initial author of this service ticket history entry (if known).
        /// </summary>
        public User                       Author                    { get; }

        public ServiceTicketStatusTypes?  NewTicketStatus           { get; }

        /// <summary>
        /// An additional multi-language problem description.
        /// </summary>
        public I18NString                 Comment                   { get; }


        public ServiceTicketHistory_Id?   InReplyTo                 { get; }

        public IEnumerable<ServiceTicketReference>         TicketReferences    { get; }

        public IEnumerable<ServiceTicketHistoryReference>  CommentReferences   { get; }

        /// <summary>
        /// An enumeration of URLs to files attached to this service ticket history entry.
        /// </summary>
        public IEnumerable<HTTPPath>      AttachedFiles             { get; }

        /// <summary>
        /// An enumeration of reactions.
        /// </summary>
        public IEnumerable<Tag>           Reactions                 { get; }


        /// <summary>
        /// Whether the service ticket history entry will be shown in (public) listings.
        /// </summary>
        public PrivacyLevel               PrivacyLevel              { get; }

        /// <summary>
        /// Optional data licsenses for publishing this data.
        /// </summary>
        public IEnumerable<DataLicense>   DataLicenses              { get; }

        #endregion

        #region Events


        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new service ticket history entry.
        /// </summary>
        /// <param name="ServiceTicketId">The unique identification of the related service ticket.</param>
        /// <param name="Id">The unique identification of this service ticket history entry.</param>
        /// <param name="Timestamp">The timestamp of the creation of this service ticket history entry.</param>
        /// <param name="Author">The initial author of this service ticket history entry (if known).</param>
        /// <param name="NewTicketStatus">An optional new service ticket status caused by this service ticket history entry.</param>
        /// <param name="Comment">An optional multi-language comment.</param>
        /// 
        /// <param name="InReplyTo">This service ticket history entry is a reply to the given history entry.</param>
        /// <param name="TicketReferences"></param>
        /// <param name="CommentReferences"></param>
        /// <param name="AttachedFiles">An enumeration of URLs to files attached to this RMA.</param>
        /// <param name="Reactions"></param>
        /// 
        /// <param name="PrivacyLevel">Whether the service ticket history entry will be shown in (public) listings.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        /// <param name="DataLicenses">Optional data licsenses for publishing this data.</param>
        /// 
        /// <param name="MaxPoolStatusListSize">The maximum size of the status list.</param>
        public AServiceTicketHistory(ServiceTicket_Id                            ServiceTicketId,
                                     ServiceTicketHistory_Id?                    Id                      = null,
                                     DateTime?                                   Timestamp               = null,
                                     User                                        Author                  = null,
                                     ServiceTicketStatusTypes?                   NewTicketStatus         = null,
                                     I18NString                                  Comment                 = null,

                                     ServiceTicketHistory_Id?                    InReplyTo               = null,
                                     IEnumerable<ServiceTicketReference>         TicketReferences        = null,
                                     IEnumerable<ServiceTicketHistoryReference>  CommentReferences       = null,
                                     IEnumerable<HTTPPath>                       AttachedFiles           = null,
                                     IEnumerable<Tag>                            Reactions               = null,

                                     PrivacyLevel?                               PrivacyLevel            = null,
                                     IEnumerable<DataLicense>                    DataLicenses            = null,
                                     String                                      DataSource              = null,

                                     UInt16                                      MaxPoolStatusListSize   = DefaultMaxStatusListSize)

            : base(Id ?? ServiceTicketHistory_Id.Random(),
                   DataSource)

        {

            this.ServiceTicketId  = ServiceTicketId;
            this.Timestamp        = Timestamp     ?? DateTime.Now;
            this.Author           = Author        ?? throw new ArgumentNullException(nameof(Author), "The given author must not be null!");
            this.NewTicketStatus  = NewTicketStatus;
            this.Comment          = Comment       ?? I18NString.Empty;
            this.InReplyTo        = InReplyTo;
            this.AttachedFiles    = AttachedFiles ?? new HTTPPath[0];
            this.Reactions        = Reactions     ?? new Tag[0];

            this.PrivacyLevel     = PrivacyLevel  ?? social.OpenData.UsersAPI.PrivacyLevel.Private;
            this.DataLicenses     = DataLicenses  ?? new DataLicense[0];

            CalcHash();

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

                new JProperty("serviceTicketId",             ServiceTicketId.ToString()),

                new JProperty("timestamp",                   Timestamp.ToIso8601()),

                Author != null
                    ? ExpandAuthorId.Switch(
                          () => new JProperty("authorId",    Author.Id.ToString()),
                          () => new JProperty("author",      Author.ToJSON()))
                    : null,

                NewTicketStatus.HasValue
                    ? new JProperty("newTicketStatus",       NewTicketStatus.Value.ToString())
                    : null,

                Comment.IsNeitherNullNorEmpty()
                    ? Comment.ToJSON("comment")
                    : null,

                InReplyTo.HasValue
                    ? InReplyTo.Value.ToJSON("inReplyTo")
                    : null,

                TicketReferences.SafeAny()
                    ? new JProperty("ticketReferences", new JArray(TicketReferences.Select(references => references.ToString())))
                    : null,

                CommentReferences.SafeAny()
                    ? new JProperty("commentReferences", new JArray(CommentReferences.Select(references => references.ToString())))
                    : null,

                Reactions.SafeAny()
                    ? ExpandReactions.Switch(
                          () => new JProperty("reactionIds",  new JArray(Reactions.Select(tag => tag.Id.ToString()))),
                          () => new JProperty("reactions",    new JArray(Reactions.Select(tag => tag.ToJSON()))))
                    : null,

                AttachedFiles.SafeAny()
                    ? new JProperty("attachedFiles", new JArray(AttachedFiles.Select(uri => uri.ToString())))
                    : null,


                PrivacyLevel.ToJSON(),

                DataLicenses.SafeAny()
                    ? ExpandDataLicenses.Switch(
                          () => new JProperty("dataLicenseIds",  new JArray(DataLicenses.SafeSelect(license => license.Id.ToString()))),
                          () => new JProperty("dataLicenses",    DataLicenses.ToJSON()))
                    : null,


                DataSource.IsNeitherNullNorEmpty()
                    ? DataSource.ToJSON("dataSource")
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
        /// <param name="UserProvider">A delegate resolving users.</param>
        /// <param name="ServiceTicketHistory">The parsed service ticket history entry.</param>
        /// <param name="ErrorResponse">An error message.</param>
        /// <param name="ServiceTicketHistoryIdURI">The optional service ticket history entry identification, e.g. from the HTTP URI.</param>
        protected static Boolean _TryParseJSON(JObject                   JSONObject,
                                               UserProviderDelegate      UserProvider,
                                               out ServiceTicketHistory  ServiceTicketHistory,
                                               out String                ErrorResponse,
                                               ServiceTicketHistory_Id?  ServiceTicketHistoryIdURI = null)
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

                if (!JSONObject.ParseMandatory("serviceTicketId",
                                               "service ticket history entry identification",
                                               ServiceTicket_Id.TryParse,
                                               out ServiceTicket_Id ServiceTicketId,
                                               out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

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

                #region Parse newTicketStatus           [optional]

                if (JSONObject.ParseOptional("newTicketStatus",
                                             "new service ticket status",
                                             out ServiceTicketStatusTypes? newTicketStatus,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

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
                                                   ServiceTicketHistory_Id.TryParse,
                                                   out ServiceTicketHistory_Id? InReplyTo,
                                                   out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                var TicketReferences   = new ServiceTicketReference[0];
                var CommentReferences  = new ServiceTicketHistoryReference[0];
                var Reactions          = new Tag[0];

                #region Parse AttachedFiles             [optional]

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

                #region Get   DataSource                [optional]

                var DataSource = JSONObject.GetOptional("dataSource");

                #endregion

                #region Parse CryptoHash                [optional]

                var CryptoHash    = JSONObject.GetOptional("cryptoHash");

                #endregion


                ServiceTicketHistory = new ServiceTicketHistory(ServiceTicketId,
                                                                ServiceTicketHistoryIdBody ?? ServiceTicketHistoryIdURI.Value,
                                                                Timestamp,
                                                                Author,
                                                                newTicketStatus,
                                                                Comment,
                                                                InReplyTo,
                                                                TicketReferences,
                                                                CommentReferences,
                                                                AttachedFiles,
                                                                Reactions,

                                                                PrivacyLevel,
                                                                DataLicenses,
                                                                DataSource);

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

            => new Builder(ServiceTicketId,
                           NewServiceTicketHistoryId ?? Id,
                           Timestamp,
                           Author,
                           NewTicketStatus,
                           Comment,
                           InReplyTo,
                           TicketReferences,
                           CommentReferences,
                           Reactions,
                           AttachedFiles,

                           PrivacyLevel,
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

            public ServiceTicket_Id           ServiceTicketId           { get; set; }

            /// <summary>
            /// The service ticket history entry identification.
            /// </summary>
            public ServiceTicketHistory_Id    Id                        { get; set; }

            public DateTime                   Timestamp                 { get; set; }

            /// <summary>
            /// The initial author of this service ticket history entry (if known).
            /// </summary>
            public User                       Author                    { get; set; }

            public ServiceTicketStatusTypes?  NewTicketStatus           { get; set; }

            /// <summary>
            /// An additional multi-language problem description.
            /// </summary>
            public I18NString                 Comment                   { get; set; }

            public ServiceTicketHistory_Id?   InReplyTo                 { get; set; }

            public IEnumerable<ServiceTicketReference>         TicketReferences    { get; set; }

            public IEnumerable<ServiceTicketHistoryReference>  CommentReferences   { get; set; }

            /// <summary>
            /// An enumeration of reactions.
            /// </summary>
            public IEnumerable<Tag>           Reactions                 { get; set; }

            /// <summary>
            /// An enumeration of URLs to files attached to this service ticket history entry.
            /// </summary>
            public IEnumerable<HTTPPath>       AttachedFiles             { get; set; }


            /// <summary>
            /// Whether the service ticket history entry will be shown in (public) listings.
            /// </summary>
            public PrivacyLevel               PrivacyLevel              { get; set; }

            /// <summary>
            /// Optional data licsenses for publishing this data.
            /// </summary>
            public IEnumerable<DataLicense>   DataLicenses              { get; set; }


            /// <summary>
            /// The source of this information, e.g. an automatic importer.
            /// </summary>
            public String                     DataSource                { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new service ticket history entry builder.
            /// </summary>
            /// <param name="Id">The unique identification of the service ticket history entry.</param>
            /// <param name="Author">The initial author of this service ticket history entry (if known).</param>
            /// <param name="PrivacyLevel">Whether the service ticket history entry will be shown in (public) listings.</param>
            /// <param name="DataLicenses">Optional data licsenses for publishing this data.</param>
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            public Builder(ServiceTicket_Id                            ServiceTicketId,
                           ServiceTicketHistory_Id?                    Id                  = null,
                           DateTime?                                   Timestamp           = null,
                           User                                        Author              = null,
                           ServiceTicketStatusTypes?                   NewTicketStatus     = null,
                           I18NString                                  Comment             = null,
                           ServiceTicketHistory_Id?                    InReplyTo           = null,
                           IEnumerable<ServiceTicketReference>         TicketReferences    = null,
                           IEnumerable<ServiceTicketHistoryReference>  CommentReferences   = null,
                           IEnumerable<Tag>                            Reactions           = null,
                           IEnumerable<HTTPPath>                        AttachedFiles       = null,
                           PrivacyLevel?                               PrivacyLevel        = null,
                           IEnumerable<DataLicense>                    DataLicenses        = null,
                           String                                      DataSource          = null)
            {

                this.ServiceTicketId    = ServiceTicketId;
                this.Id                 = Id            ?? ServiceTicketHistory_Id.Random();
                this.Timestamp          = Timestamp     ?? DateTime.Now;
                this.Author             = Author        ?? throw new ArgumentNullException(nameof(Author), "The given author must not be null!");
                this.NewTicketStatus    = NewTicketStatus;
                this.Comment            = Comment       ?? I18NString.Empty;
                this.InReplyTo          = InReplyTo;
                this.TicketReferences   = TicketReferences;
                this.CommentReferences  = CommentReferences;
                this.Reactions          = Reactions     ?? new Tag[0];
                this.AttachedFiles      = AttachedFiles ?? new HTTPPath[0];

                this.PrivacyLevel       = PrivacyLevel  ?? social.OpenData.UsersAPI.PrivacyLevel.Private;
                this.DataLicenses       = DataLicenses  ?? new DataLicense[0];
                this.DataSource         = DataSource    ?? "";

            }

            #endregion

            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the service ticket history entry.
            /// </summary>
            public static implicit operator ServiceTicketHistory(Builder Builder)

                => Builder.ToImmutable;


            /// <summary>
            /// Return an immutable version of the service ticket history entry.
            /// </summary>
            public ServiceTicketHistory ToImmutable

                => new ServiceTicketHistory(ServiceTicketId,
                                            Id,
                                            Timestamp,
                                            Author,
                                            NewTicketStatus,
                                            Comment,
                                            InReplyTo,
                                            TicketReferences,
                                            CommentReferences,
                                            AttachedFiles,
                                            Reactions,

                                            PrivacyLevel,
                                            DataLicenses,
                                            DataSource);

            #endregion

        }

        #endregion

    }

}
