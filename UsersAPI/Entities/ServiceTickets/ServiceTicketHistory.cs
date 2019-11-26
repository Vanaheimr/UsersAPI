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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Styx.Arrows;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace social.OpenData.UsersAPI
{

    public delegate Boolean ServiceTicketHistoryProviderDelegate(ServiceTicketHistory_Id CommunicatorId, out ServiceTicketHistory ServiceTicketHistory);

    public delegate JObject ServiceTicketHistoryToJSONDelegate(ServiceTicketHistory  ServiceTicketHistory,
                                                               InfoStatus            ExpandDataLicenses     = InfoStatus.ShowIdOnly,
                                                               InfoStatus            ExpandOwnerId          = InfoStatus.ShowIdOnly,
                                                               InfoStatus            ExpandCommunicatorId   = InfoStatus.ShowIdOnly,
                                                               Boolean               IncludeCryptoHash      = true);


    /// <summary>
    /// Extention methods for the service ticket history entry.
    /// </summary>
    public static partial class ServiceTicketHistoryExtentions
    {

        #region ToJSON(this ServiceTicketHistorys, Skip = null, Take = null, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of service ticket history entrys.
        /// </summary>
        /// <param name="ServiceTicketHistorys">An enumeration of service ticket history entrys.</param>
        /// <param name="Skip">The optional number of service ticket history entrys to skip.</param>
        /// <param name="Take">The optional number of service ticket history entrys to return.</param>
        /// <param name="Embedded">Whether this data is embedded into another data structure, e.g. into a service ticket history entry.</param>
        public static JArray ToJSON(this IEnumerable<ServiceTicketHistory>  ServiceTicketHistorys,
                                    UInt64?                                 Skip                         = null,
                                    UInt64?                                 Take                         = null,
                                    InfoStatus                              ExpandDataLicenses           = InfoStatus.ShowIdOnly,
                                    InfoStatus                              ExpandOwnerId                = InfoStatus.ShowIdOnly,
                                    InfoStatus                              ExpandCommunicatorId         = InfoStatus.ShowIdOnly,
                                    ServiceTicketHistoryToJSONDelegate      ServiceTicketHistoryToJSON   = null,
                                    Boolean                                 IncludeCryptoHash            = true)


            => ServiceTicketHistorys?.Any() != true

                   ? new JArray()

                   : new JArray(ServiceTicketHistorys.
                                    Where(history => history != null).
                                    SkipTakeFilter(Skip, Take).
                                    SafeSelect(serviceticket => ServiceTicketHistoryToJSON != null
                                                                    ? ServiceTicketHistoryToJSON(serviceticket,
                                                                                                 ExpandDataLicenses,
                                                                                                 ExpandOwnerId,
                                                                                                 ExpandCommunicatorId,
                                                                                                 IncludeCryptoHash)

                                                                    : serviceticket.ToJSON(ExpandDataLicenses,
                                                                                           ExpandOwnerId,
                                                                                           ExpandCommunicatorId,
                                                                                           IncludeCryptoHash)));

        #endregion

    }


    /// <summary>
    /// A service ticket history entry.
    /// </summary>
    public class ServiceTicketHistory : AServiceTicketHistory,
                                        IEntityClass<ServiceTicketHistory>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of the object.
        /// </summary>
        public const String JSONLDContext = "https://cardi-link.cloud/contexts/cardidb+json/serviceTicketComment";

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
        public ServiceTicketHistory(ServiceTicket_Id                            ServiceTicketId,
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

            : base(ServiceTicketId,
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
                   DataSource)

        {
            CalcHash();
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
        /// <param name="IncludeCryptoHash">Include the cryptograhical hash value of this object.</param>
        public JObject ToJSON(InfoStatus  ExpandAuthorId      = InfoStatus.ShowIdOnly,
                              InfoStatus  ExpandReactions     = InfoStatus.ShowIdOnly,
                              InfoStatus  ExpandDataLicenses  = InfoStatus.ShowIdOnly,
                              Boolean     IncludeCryptoHash   = true)

            => base.ToJSON(ExpandAuthorId,
                           ExpandReactions,
                           ExpandDataLicenses,
                           IncludeCryptoHash,
                           json => {
                               json["@context"] = JSONLDContext;
                           });

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out ServiceTicketHistory, out ErrorResponse)

        /// <summary>
        /// Try to parse the given service ticket history entry JSON.
        /// </summary>
        /// <param name="JSONObject">A JSON object.</param>
        /// <param name="UserProvider">A delegate resolving users.</param>
        /// <param name="ServiceTicketHistory">The parsed service ticket history entry.</param>
        /// <param name="ErrorResponse">An error message.</param>
        /// <param name="ServiceTicketHistoryIdURI">The optional service ticket history entry identification, e.g. from the HTTP URI.</param>
        public static Boolean TryParseJSON(JObject                   JSONObject,
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

                #region Parse Context                   [mandatory]

                if (!JSONObject.ParseMandatory("@context",
                                               "JSON-LD context",
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

                return _TryParseJSON(JSONObject,
                                     UserProvider,
                                     out ServiceTicketHistory,
                                     out ErrorResponse,
                                     ServiceTicketHistoryIdURI);

            }
            catch (Exception e)
            {
                ErrorResponse         = e.Message;
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

        #region Operator == (ServiceTicketHistory1, ServiceTicketHistory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketHistory1">A service ticket history entry identification.</param>
        /// <param name="ServiceTicketHistory2">Another service ticket history entry identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ServiceTicketHistory ServiceTicketHistory1, ServiceTicketHistory ServiceTicketHistory2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ServiceTicketHistory1, ServiceTicketHistory2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ServiceTicketHistory1 == null) || ((Object) ServiceTicketHistory2 == null))
                return false;

            return ServiceTicketHistory1.Equals(ServiceTicketHistory2);

        }

        #endregion

        #region Operator != (ServiceTicketHistory1, ServiceTicketHistory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketHistory1">A service ticket history entry identification.</param>
        /// <param name="ServiceTicketHistory2">Another service ticket history entry identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ServiceTicketHistory ServiceTicketHistory1, ServiceTicketHistory ServiceTicketHistory2)
            => !(ServiceTicketHistory1 == ServiceTicketHistory2);

        #endregion

        #region Operator <  (ServiceTicketHistory1, ServiceTicketHistory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketHistory1">A service ticket history entry identification.</param>
        /// <param name="ServiceTicketHistory2">Another service ticket history entry identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ServiceTicketHistory ServiceTicketHistory1, ServiceTicketHistory ServiceTicketHistory2)
        {

            if ((Object) ServiceTicketHistory1 == null)
                throw new ArgumentNullException(nameof(ServiceTicketHistory1), "The given ServiceTicketHistory1 must not be null!");

            return ServiceTicketHistory1.CompareTo(ServiceTicketHistory2) < 0;

        }

        #endregion

        #region Operator <= (ServiceTicketHistory1, ServiceTicketHistory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketHistory1">A service ticket history entry identification.</param>
        /// <param name="ServiceTicketHistory2">Another service ticket history entry identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ServiceTicketHistory ServiceTicketHistory1, ServiceTicketHistory ServiceTicketHistory2)
            => !(ServiceTicketHistory1 > ServiceTicketHistory2);

        #endregion

        #region Operator >  (ServiceTicketHistory1, ServiceTicketHistory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketHistory1">A service ticket history entry identification.</param>
        /// <param name="ServiceTicketHistory2">Another service ticket history entry identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ServiceTicketHistory ServiceTicketHistory1, ServiceTicketHistory ServiceTicketHistory2)
        {

            if ((Object) ServiceTicketHistory1 == null)
                throw new ArgumentNullException(nameof(ServiceTicketHistory1), "The given ServiceTicketHistory1 must not be null!");

            return ServiceTicketHistory1.CompareTo(ServiceTicketHistory2) > 0;

        }

        #endregion

        #region Operator >= (ServiceTicketHistory1, ServiceTicketHistory2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketHistory1">A service ticket history entry identification.</param>
        /// <param name="ServiceTicketHistory2">Another service ticket history entry identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ServiceTicketHistory ServiceTicketHistory1, ServiceTicketHistory ServiceTicketHistory2)
            => !(ServiceTicketHistory1 < ServiceTicketHistory2);

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
        public new class Builder
        {

            #region Properties

            public ServiceTicket_Id                     ServiceTicketId           { get; set; }

            /// <summary>
            /// The service ticket history entry identification.
            /// </summary>
            public ServiceTicketHistory_Id              Id                        { get; set; }

            public DateTime                             Timestamp                 { get; set; }

            /// <summary>
            /// The initial author of this service ticket history entry (if known).
            /// </summary>
            public User                                 Author                    { get; set; }

            public ServiceTicketStatusTypes?            NewTicketStatus           { get; set; }

            /// <summary>
            /// An additional multi-language problem description.
            /// </summary>
            public I18NString                           Comment                   { get; set; }

            public ServiceTicketHistory_Id?             InReplyTo                 { get; set; }

            public List<ServiceTicketReference>         TicketReferences          { get; set; }

            public List<ServiceTicketHistoryReference>  CommentReferences         { get; set; }

            /// <summary>
            /// An enumeration of reactions.
            /// </summary>
            public IEnumerable<Tag>                     Reactions                 { get; set; }

            /// <summary>
            /// An enumeration of URLs to files attached to this service ticket history entry.
            /// </summary>
            public IEnumerable<HTTPPath>                 AttachedFiles             { get; set; }


            /// <summary>
            /// Whether the service ticket history entry will be shown in (public) listings.
            /// </summary>
            public PrivacyLevel                         PrivacyLevel              { get; set; }

            /// <summary>
            /// Optional data licsenses for publishing this data.
            /// </summary>
            public IEnumerable<DataLicense>             DataLicenses              { get; set; }


            /// <summary>
            /// The source of this information, e.g. an automatic importer.
            /// </summary>
            public String                               DataSource                { get; set; }

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
                this.Id                 = Id                ?? ServiceTicketHistory_Id.Random();
                this.Timestamp          = Timestamp         ?? DateTime.Now;
                this.Author             = Author            ?? throw new ArgumentNullException(nameof(Author), "The given author must not be null!");
                this.NewTicketStatus    = NewTicketStatus;
                this.Comment            = Comment           ?? I18NString.Empty;
                this.InReplyTo          = InReplyTo;
                this.TicketReferences   = TicketReferences  != null
                                              ? new List<ServiceTicketReference>(TicketReferences)
                                              : new List<ServiceTicketReference>();
                this.CommentReferences  = CommentReferences != null
                                              ? new List<ServiceTicketHistoryReference>(CommentReferences)
                                              : new List<ServiceTicketHistoryReference>();
                this.Reactions          = Reactions         ?? new Tag[0];
                this.AttachedFiles      = AttachedFiles     ?? new HTTPPath[0];

                this.PrivacyLevel       = PrivacyLevel      ?? social.OpenData.UsersAPI.PrivacyLevel.Private;
                this.DataLicenses       = DataLicenses      ?? new DataLicense[0];
                this.DataSource         = DataSource        ?? "";

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
