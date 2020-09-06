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
using org.GraphDefined.Vanaheimr.Styx.Arrows;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Distributed;

#endregion

namespace social.OpenData.UsersAPI
{

    public delegate Boolean AServiceTicketProviderDelegate(ServiceTicket_Id ServiceTicketId, out AServiceTicket ServiceTicket);

    public delegate JObject AServiceTicketToJSONDelegate(AServiceTicket                                     ServiceTicket,
                                                         Boolean                                            Embedded                = false,
                                                         UInt16?                                            MaxStatus               = null,
                                                         Func<DateTime, ServiceTicketStatusTypes, Boolean>  IncludeStatus           = null,
                                                         InfoStatus                                         ExpandDataLicenses      = InfoStatus.ShowIdOnly,
                                                         InfoStatus                                         ExpandOwnerId           = InfoStatus.ShowIdOnly,
                                                         Boolean                                            IncludeComments         = true,
                                                         Boolean                                            IncludeCryptoHash       = true);


    /// <summary>
    /// Extention methods for the service ticket.
    /// </summary>
    public static partial class ServiceTicketExtentions
    {

        #region ToJSON(this ServiceTickets, Skip = null, Take = null, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of service tickets.
        /// </summary>
        /// <param name="ServiceTickets">An enumeration of service tickets.</param>
        /// <param name="Skip">The optional number of service tickets to skip.</param>
        /// <param name="Take">The optional number of service tickets to return.</param>
        /// <param name="Embedded">Whether this data is embedded into another data structure, e.g. into a service ticket.</param>
        public static JArray ToJSON(this IEnumerable<AServiceTicket>                   ServiceTickets,
                                    UInt64?                                            Skip                    = null,
                                    UInt64?                                            Take                    = null,
                                    Boolean                                            Embedded                = false,
                                    Func<          ServiceTicketStatusTypes, Boolean>  IncludeWithStatus       = null,
                                    UInt16?                                            MaxStatus               = null,
                                    Func<DateTime, ServiceTicketStatusTypes, Boolean>  IncludeStatus           = null,
                                    Func<          ServiceTicketPriorities,  Boolean>  IncludeWithPriorities   = null,
                                    InfoStatus                                         ExpandDataLicenses      = InfoStatus.ShowIdOnly,
                                    InfoStatus                                         ExpandAuthorId          = InfoStatus.ShowIdOnly,
                                    Boolean                                            IncludeComments         = true,
                                    AServiceTicketToJSONDelegate                       ServiceTicketToJSON     = null,
                                    Boolean                                            IncludeCryptoHash       = true)


            => ServiceTickets?.Any() != true

                   ? new JArray()

                   : new JArray(ServiceTickets.
                                    Where(serviceticket => serviceticket          != null                                                       &&
                                                           (IncludeStatus         == null || IncludeWithStatus    (serviceticket.Status.Value)) &&
                                                           (IncludeWithPriorities == null || IncludeWithPriorities(serviceticket.Priority))).
                                    //OrderBy(serviceticket => serviceticket.Id).
                                    SkipTakeFilter(Skip, Take).
                                    SafeSelect(serviceticket => ServiceTicketToJSON != null
                                                                    ? ServiceTicketToJSON (serviceticket,
                                                                                           Embedded,
                                                                                           MaxStatus,
                                                                                           IncludeStatus,
                                                                                           ExpandDataLicenses,
                                                                                           ExpandAuthorId,
                                                                                           IncludeComments,
                                                                                           IncludeCryptoHash)

                                                                    : serviceticket.ToJSON(Embedded,
                                                                                           MaxStatus,
                                                                                           IncludeStatus,
                                                                                           ExpandDataLicenses,
                                                                                           ExpandAuthorId,
                                                                                           IncludeComments,
                                                                                           IncludeCryptoHash)));

        #endregion

    }

    /// <summary>
    /// A service ticket.
    /// </summary>
    public abstract class AServiceTicket : ADistributedEntity<ServiceTicket_Id>,
                                           IEntityClass<AServiceTicket>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        private const String _JSONLDContext = "https://opendata.social/contexts/UsersAPI+json/serviceTicket";

        #endregion

        #region Properties

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public virtual String JSONLDContext
            => _JSONLDContext;

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
        public IEnumerable<AServiceTicketChangeSet>     ChangeSets              { get; }


        /// <summary>
        /// The timestamp when this ticket was created.
        /// </summary>
        public DateTime                                 Created                 { get; }

        /// <summary>
        /// The initial author of this service ticket.
        /// </summary>
        public User                                     Author                  { get; }

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
        /// Whether the service ticket change set will be shown in (public) listings.
        /// </summary>
        public PrivacyLevel                             PrivacyLevel            { get; }

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
        public IEnumerable<HTTPPath>                    AttachedFiles           { get; }

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

        /// <summary>
        /// Create a new service ticket.
        /// </summary>
        /// <param name="Id">The unique identification of the service ticket.</param>
        /// <param name="ChangeSets">An enumeration of service ticket change sets.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        public AServiceTicket(ServiceTicket_Id                      Id,
                              IEnumerable<AServiceTicketChangeSet>  ChangeSets,
                              String                                DataSource  = null)

            : base(Id,
                   DataSource)

        {

            if (ChangeSets == null || !ChangeSets.Any())
                throw new ArgumentNullException(nameof(AServiceTicket.ChangeSets),  "The enumeration of change sets (the service ticket history) of the service ticket must not be null!");

            this.ChangeSets      = ChangeSets.OrderByDescending(changeSet => changeSet.Timestamp).ToArray();

            Created              = this.ChangeSets.LastOrDefault()?.Timestamp
                                                   ?? DateTime.UtcNow;


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


            Title                = this.ChangeSets.Where(entry => entry.Title.IsNeitherNullNorEmpty()).
                                                   FirstOrDefault()?.Title;

            if (this.Title.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Title),    "The title of the service ticket must not be null or empty!");


            Affected             = this.ChangeSets.Where(entry => entry.Affected            != null).
                                                   FirstOrDefault()?.Affected;

            Priority             = this.ChangeSets.Where(entry => entry.Priority.HasValue).
                                                   FirstOrDefault()?.Priority
                                                   ?? ServiceTicketPriorities.Normal;

            PrivacyLevel         = this.ChangeSets.Where(entry => entry.PrivacyLevel.HasValue).
                                                   FirstOrDefault()?.PrivacyLevel
                                                   ?? PrivacyLevel.Private;

            Location             = this.ChangeSets.Where(entry => entry.Location.IsNeitherNullNorEmpty()).
                                                   FirstOrDefault()?.Location;

            GeoLocation          = this.ChangeSets.Where(entry => entry.GeoLocation.HasValue).
                                                   FirstOrDefault()?.GeoLocation;

            ProblemDescriptions  = this.ChangeSets.Where(entry => entry.ProblemDescriptions.SafeAny()).
                                                   FirstOrDefault()?.ProblemDescriptions;

            StatusIndicators     = this.ChangeSets.Where(entry => entry.StatusIndicators.SafeAny()).
                                                   FirstOrDefault()?.StatusIndicators;

            Reactions            = this.ChangeSets.Where(entry => entry.Reactions.SafeAny()).
                                                   FirstOrDefault()?.Reactions;

            AdditionalInfo       = this.ChangeSets.Where(entry => entry.AdditionalInfo.IsNeitherNullNorEmpty()).
                                                   FirstOrDefault()?.AdditionalInfo;

            AttachedFiles        = this.ChangeSets.Where(entry => entry.AttachedFiles.SafeAny()).
                                                   FirstOrDefault()?.AttachedFiles;

            TicketReferences     = this.ChangeSets.Where(entry => entry.TicketReferences.SafeAny()).
                                                   FirstOrDefault()?.TicketReferences;

            DataLicenses         = this.ChangeSets.Where(entry => entry.DataLicenses.SafeAny()).
                                                   FirstOrDefault()?.DataLicenses;

        }

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
        /// <param name="PrivacyLevel">Whether the service ticket change set will be shown in (public) listings.</param>
        /// <param name="Location">The location of the problem or broken device.</param>
        /// <param name="GeoLocation">The geographical location of the problem or broken device.</param>
        /// <param name="ProblemDescriptions">An enumeration of well-defined problem descriptions.</param>
        /// <param name="StatusIndicators">An enumeration of problem indicators.</param>
        /// <param name="Reactions">An enumeration of reactions.</param>
        /// <param name="AdditionalInfo">A multi-language description of the service ticket.</param>
        /// <param name="AttachedFiles">An enumeration of URLs to files attached to this service ticket.</param>
        /// <param name="TicketReferences">References to other service tickets.</param>
        /// <param name="DataLicenses">An enumeration of usable data licenses for this service ticket.</param>
        /// 
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        public AServiceTicket(ServiceTicket_Id                     Id,

                              DateTime?                            Timestamp             = null,
                              User                                 Author                = null,
                              ServiceTicketStatusTypes?            Status                = null,
                              I18NString                           Title                 = null,
                              Affected                             Affected              = null,
                              ServiceTicketPriorities?             Priority              = null,
                              PrivacyLevel?                        PrivacyLevel          = null,
                              I18NString                           Location              = null,
                              GeoCoordinate?                       GeoLocation           = null,
                              IEnumerable<ProblemDescriptionI18N>  ProblemDescriptions   = null,
                              IEnumerable<Tag>                     StatusIndicators      = null,
                              IEnumerable<Tag>                     Reactions             = null,
                              I18NString                           AdditionalInfo        = null,
                              IEnumerable<HTTPPath>                AttachedFiles         = null,
                              IEnumerable<ServiceTicketReference>  TicketReferences      = null,
                              IEnumerable<DataLicense>             DataLicenses          = null,

                              String                               DataSource            = null)

            : this(Id,
                   new List<AServiceTicketChangeSet>() {
                       new ServiceTicketChangeSet(
                           Id:                   ServiceTicketChangeSet_Id.Random(),
                           Timestamp:            Timestamp    ?? DateTime.UtcNow,
                           Author:               Author,
                           Status:               Status       ?? ServiceTicketStatusTypes.New,
                           Title:                Title,
                           Affected:             Affected,
                           Priority:             Priority     ?? ServiceTicketPriorities.Normal,
                           PrivacyLevel:         PrivacyLevel ?? OpenData.UsersAPI.PrivacyLevel.Private,
                           Location:             Location,
                           GeoLocation:          GeoLocation,
                           ProblemDescriptions:  ProblemDescriptions,
                           StatusIndicators:     StatusIndicators,
                           Reactions:            Reactions,
                           AdditionalInfo:       AdditionalInfo,
                           AttachedFiles:        AttachedFiles,
                           TicketReferences:     TicketReferences,
                           DataLicenses:         DataLicenses,

                           Comment:              null,
                           InReplyTo:            null,
                           CommentReferences:    null,

                           DataSource:           DataSource)
                    },
                    DataSource)

        { }

        #endregion


        #region (protected) ToJSON(...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure, e.g. into a service ticket.</param>
        /// <param name="IncludeCryptoHash">Whether to include the cryptograhical hash value of this object.</param>
        public virtual JObject ToJSON(Boolean                                            Embedded                = false,
                                      UInt16?                                            MaxStatus               = null,
                                      Func<DateTime, ServiceTicketStatusTypes, Boolean>  IncludeStatus           = null,
                                      InfoStatus                                         ExpandDataLicenses      = InfoStatus.ShowIdOnly,
                                      InfoStatus                                         ExpandAuthorId          = InfoStatus.ShowIdOnly,
                                      Boolean                                            IncludeChangeSets         = true,
                                      Boolean                                            IncludeCryptoHash       = true,
                                      Action<JObject>                                    Configurator            = null)

        {

            var Status         = ServiceTicketStatusTypes.New;
            var StatusHistory  = new List<Timestamped<ServiceTicketStatusTypes>>();

            foreach (var history in ChangeSets.Reverse())
            {

                if (history.Status.HasValue && Status != history.Status)
                    Status = history.Status.Value;

                StatusHistory.Add(new Timestamped<ServiceTicketStatusTypes>(history.Timestamp, Status));

            }


            var JSON = JSONObject.Create(

                   Id.ToJSON("@id"),

                   Embedded
                       ? null
                       : new JProperty("@context",             JSONLDContext),

                   new JProperty("title",                      Title.ToJSON()),

                   new JProperty("status",                     new JObject((StatusHistory as IEnumerable<Timestamped<ServiceTicketStatusTypes>>).
                                                                               Reverse().
                                                                               Where (timestamped => IncludeStatus != null ? IncludeStatus(timestamped.Timestamp, timestamped.Value) : true).
                                                                               Take  (MaxStatus).
                                                                               Select(timestamped => new JProperty(timestamped.Timestamp.ToIso8601(),
                                                                                                                   timestamped.Value.    ToString()))
                                                                          )),

                   Affected != null && !Affected.IsEmpty
                       ? new JProperty("affected", Affected.ToJSON())
                       : null,

                   Author != null
                       ? ExpandAuthorId.Switch(
                             () => new JProperty("author",     new JObject(
                                                                   new JProperty("@id",   Author.Id.ToString()),
                                                                   new JProperty("name",  Author.Name)
                                                               )),
                             () => new JProperty("author",     Author.ToJSON()))
                       : null,

                   new JProperty("priority", Priority.ToString().ToLower()),

                   Location.IsNeitherNullNorEmpty()
                       ? Location.ToJSON("location")
                       : null,

                   GeoLocation?.ToJSON("geoLocation"),

                   ProblemDescriptions.IsNeitherNullNorEmpty()
                       ? new JProperty("problemDescriptions",  new JArray(ProblemDescriptions.  Select(problemDescription => problemDescription.ToJSON())))
                       : null,

                   StatusIndicators.SafeAny()
                       ? new JProperty("statusIndicators",     new JArray(StatusIndicators.     Select(tag                => tag.Id.ToString())))
                       : null,

                   AdditionalInfo.IsNeitherNullNorEmpty()
                       ? AdditionalInfo.ToJSON("additionalInfo")
                       : null,

                   AttachedFiles.SafeAny()
                       ? new JProperty("attachedFiles",        new JArray(AttachedFiles.        Select(attachedFile       => attachedFile.ToString())))
                       : null,

                   IncludeChangeSets && ChangeSets.SafeAny()
                       ? new JProperty("changeSets",           new JArray(ChangeSets.
                                                                              OrderByDescending(changeSet => changeSet.Timestamp).
                                                                              SafeSelect       (changeSet => changeSet.ToJSON())))
                       : null,


                   // MaxPoolStatusListSize

                   PrivacyLevel.ToJSON(),

                   DataLicenses.SafeAny()
                       ? ExpandDataLicenses.Switch(
                             () => new JProperty("dataLicenseIds",  new JArray(DataLicenses.    Select(dataLicense        => dataLicense.Id.ToString()))),
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


        #region (private)  UpdateMyself     (NewServiceTicket)

        private AServiceTicket UpdateMyself(AServiceTicket NewServiceTicket)
        {

            //foreach (var pairing in _Pairings.Where(pairing => pairing.ServiceTicket.Id == Id))
            //    pairing.ServiceTicket = NewServiceTicket;

            return NewServiceTicket;

        }

        #endregion

        #region (internal) ChangeStatus     (NewStatus)

        ///// <summary>
        ///// Change the status of this service ticket.
        ///// </summary>
        ///// <param name="NewStatus">The new status.</param>
        //internal ServiceTicket ChangeStatus(ServiceTicketStatusTypes NewStatus)
        //{

        //    if (NewStatus != Status.Value)
        //        return UpdateMyself(new ServiceTicket(Id,
        //                                              Title,
        //                                              Author,
        //                                              _StatusSchedule.Insert(NewStatus),
        //                                              Priority,
        //                                              Affected,
        //                                              Location,
        //                                              GeoLocation,
        //                                              ProblemDescriptions,
        //                                              StatusIndicators,
        //                                              AdditionalInfo,
        //                                              AttachedFiles,
        //                                              History,

        //                                              PrivacyLevel,
        //                                              DataLicenses,
        //                                              DataSource,

        //                                              _StatusSchedule.MaxStatusHistorySize));

        //    return this;

        //}


        ///// <summary>
        ///// Change the status of this service ticket.
        ///// </summary>
        ///// <param name="NewStatus">The new status.</param>
        //public ServiceTicket ChangeStatus(Timestamped<TStatus> NewStatus)
        //{

        //    if (NewStatus != Status.Value)
        //        return UpdateMyself(new ServiceTicket(Id,
        //                                              Title,
        //                                              Author,
        //                                              _StatusSchedule.Insert(NewStatus),
        //                                              Priority,
        //                                              Affected,
        //                                              Location,
        //                                              GeoLocation,
        //                                              ProblemDescriptions,
        //                                              StatusIndicators,
        //                                              AdditionalInfo,
        //                                              AttachedFiles,
        //                                              History,

        //                                              PrivacyLevel,
        //                                              DataLicenses,
        //                                              DataSource,

        //                                              _StatusSchedule.     MaxStatusHistorySize));

        //    return this;

        //}

        #endregion

        #region CopyAllEdgesTo(Target)

        public void CopyAllEdgesTo(AServiceTicket Target)
        {


        }

        #endregion


        #region Operator overloading

        #region Operator == (ServiceTicket1, ServiceTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicket1">A service ticket identification.</param>
        /// <param name="ServiceTicket2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AServiceTicket ServiceTicket1, AServiceTicket ServiceTicket2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ServiceTicket1, ServiceTicket2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ServiceTicket1 == null) || ((Object) ServiceTicket2 == null))
                return false;

            return ServiceTicket1.Equals(ServiceTicket2);

        }

        #endregion

        #region Operator != (ServiceTicket1, ServiceTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicket1">A service ticket identification.</param>
        /// <param name="ServiceTicket2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AServiceTicket ServiceTicket1, AServiceTicket ServiceTicket2)
            => !(ServiceTicket1 == ServiceTicket2);

        #endregion

        #region Operator <  (ServiceTicket1, ServiceTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicket1">A service ticket identification.</param>
        /// <param name="ServiceTicket2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (AServiceTicket ServiceTicket1, AServiceTicket ServiceTicket2)
        {

            if ((Object) ServiceTicket1 == null)
                throw new ArgumentNullException(nameof(ServiceTicket1), "The given ServiceTicket1 must not be null!");

            return ServiceTicket1.CompareTo(ServiceTicket2) < 0;

        }

        #endregion

        #region Operator <= (ServiceTicket1, ServiceTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicket1">A service ticket identification.</param>
        /// <param name="ServiceTicket2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (AServiceTicket ServiceTicket1, AServiceTicket ServiceTicket2)
            => !(ServiceTicket1 > ServiceTicket2);

        #endregion

        #region Operator >  (ServiceTicket1, ServiceTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicket1">A service ticket identification.</param>
        /// <param name="ServiceTicket2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (AServiceTicket ServiceTicket1, AServiceTicket ServiceTicket2)
        {

            if ((Object) ServiceTicket1 == null)
                throw new ArgumentNullException(nameof(ServiceTicket1), "The given ServiceTicket1 must not be null!");

            return ServiceTicket1.CompareTo(ServiceTicket2) > 0;

        }

        #endregion

        #region Operator >= (ServiceTicket1, ServiceTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicket1">A service ticket identification.</param>
        /// <param name="ServiceTicket2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (AServiceTicket ServiceTicket1, AServiceTicket ServiceTicket2)
            => !(ServiceTicket1 < ServiceTicket2);

        #endregion

        #endregion

        #region IComparable<ServiceTicket> Members

        #region CompareTo(Object)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="Object">An object to compare with.</param>
        //public new Int32 CompareTo(Object Object)
        //{

        //    if (Object == null)
        //        throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

        //    if (!(Object is AServiceTicket ServiceTicket))
        //        throw new ArgumentException("The given object is not a service ticket!");

        //    return CompareTo(ServiceTicket);

        //}

        #endregion

        #region CompareTo(ServiceTicket)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicket">An service ticket object to compare with.</param>
        public Int32 CompareTo(AServiceTicket ServiceTicket)
        {

            if (ServiceTicket is null)
                throw new ArgumentNullException(nameof(ServiceTicket), "The given service ticket must not be null!");

            return Id.CompareTo(ServiceTicket.Id);

        }

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
        {

            if (Object == null)
                return false;

            if (!(Object is AServiceTicket ServiceTicket))
                throw new ArgumentException("The given object is not a service ticket!");

            return Equals(ServiceTicket);

        }

        #endregion

        #region Equals(ServiceTicket)

        /// <summary>
        /// Compares two service tickets for equality.
        /// </summary>
        /// <param name="ServiceTicket">An service ticket to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(AServiceTicket ServiceTicket)
        {

            if (ServiceTicket is null)
                return false;

            return Id.Equals(ServiceTicket.Id);

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


        #region (class) Builder

        /// <summary>
        /// A service ticket builder.
        /// </summary>
        public abstract class ABuilder
        {

            #region Properties

            /// <summary>
            /// The unique identification of the service ticket.
            /// </summary>
            public ServiceTicket_Id                      Id              { get; set; }

            private readonly List<AServiceTicketChangeSet>  _ChangeSets;

            /// <summary>
            /// An enumeration of change sets.
            /// </summary>
            public IEnumerable<AServiceTicketChangeSet>  ChangeSets
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
            public User Author
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
                 => ChangeSets?.Where(entry => entry.Title.IsNeitherNullNorEmpty()).
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
            /// Whether the service ticket change set will be shown in (public) listings.
            /// </summary>
            public PrivacyLevel PrivacyLevel
                => ChangeSets?.Where(entry => !entry.PrivacyLevel.HasValue).
                            FirstOrDefault()?.PrivacyLevel
                   ?? PrivacyLevel.Private;

            /// <summary>
            /// The location of the problem or broken device.
            /// </summary>
            public I18NString Location
                => ChangeSets?.Where(entry => entry.Location.IsNeitherNullNorEmpty()).
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
            /// An enumeration of reactions.
            /// </summary>
            public IEnumerable<Tag> Reactions
                => ChangeSets?.Where(entry => entry.Reactions.SafeAny()).
                               FirstOrDefault()?.Reactions;

            /// <summary>
            /// Additional multi-language information related to this service ticket.
            /// </summary>
            public I18NString AdditionalInfo
                => ChangeSets?.Where(entry => entry.AdditionalInfo.IsNeitherNullNorEmpty()).
                            FirstOrDefault()?.AdditionalInfo;

            /// <summary>
            /// An enumeration of URLs to files attached to this service ticket change set.
            /// </summary>
            public IEnumerable<HTTPPath> AttachedFiles
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
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            public ABuilder(ServiceTicket_Id                      Id,
                            IEnumerable<AServiceTicketChangeSet>  ChangeSets,
                            String                                DataSource  = null)

            {

                this.Id           = Id;
                this._ChangeSets  = ChangeSets != null ? new List<AServiceTicketChangeSet>(ChangeSets) : new List<AServiceTicketChangeSet>();
                this.DataSource   = DataSource;

            }

            /// <summary>
            /// Create a new service ticket builder.
            /// </summary>
            /// <param name="Id">The unique identification of the service ticket.</param>
            /// 
            /// <param name="ServiceTicketChangeSetId">The unique identification of a service ticket change set.</param>
            /// <param name="Timestamp">The timestamp of the creation of this service ticket.</param>
            /// <param name="Author">The initial author of this service ticket (if known).</param>
            /// <param name="Status">An optional new service ticket status.</param>
            /// <param name="Title">The title of the service ticket (10-200 characters).</param>
            /// <param name="Affected">Affected devices or services by this service ticket.</param>
            /// <param name="Priority">The priority of the service ticket.</param>
            /// <param name="PrivacyLevel">Whether the service ticket will be shown in (public) listings.</param>
            /// <param name="Location">The location of the problem or broken device.</param>
            /// <param name="GeoLocation">The geographical location of the problem or broken device.</param>
            /// <param name="ProblemDescriptions">An enumeration of well-defined problem descriptions.</param>
            /// <param name="StatusIndicators">An enumeration of problem indicators.</param>
            /// <param name="Reactions">An enumeration of reactions.</param>
            /// <param name="AdditionalInfo">A multi-language description of the service ticket.</param>
            /// <param name="AttachedFiles">An enumeration of URLs to files attached to this service ticket.</param>
            /// <param name="TicketReferences">References to other service tickets.</param>
            /// <param name="DataLicenses">An enumeration of usable data licenses for this service ticket.</param>
            /// 
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            public ABuilder(ServiceTicket_Id                     Id,

                            ServiceTicketChangeSet_Id?           ServiceTicketChangeSetId   = null,
                            DateTime?                            Timestamp                  = null,
                            User                                 Author                     = null,
                            ServiceTicketStatusTypes?            Status                     = null,
                            I18NString                           Title                      = null,
                            Affected                             Affected                   = null,
                            ServiceTicketPriorities?             Priority                   = null,
                            PrivacyLevel?                        PrivacyLevel               = null,
                            I18NString                           Location                   = null,
                            GeoCoordinate?                       GeoLocation                = null,
                            IEnumerable<ProblemDescriptionI18N>  ProblemDescriptions        = null,
                            IEnumerable<Tag>                     StatusIndicators           = null,
                            IEnumerable<Tag>                     Reactions                  = null,
                            I18NString                           AdditionalInfo             = null,
                            IEnumerable<HTTPPath>                AttachedFiles              = null,
                            IEnumerable<ServiceTicketReference>  TicketReferences           = null,
                            IEnumerable<DataLicense>             DataLicenses               = null,

                            String                               DataSource                 = null)

                : this(Id,
                       new List<AServiceTicketChangeSet>() {
                           new ServiceTicketChangeSet(
                               Id:                   ServiceTicketChangeSetId ?? ServiceTicketChangeSet_Id.Random(),
                               Timestamp:            Timestamp                ?? DateTime.UtcNow,
                               Author:               Author,
                               Status:               Status                   ?? ServiceTicketStatusTypes.New,
                               Title:                Title,
                               Affected:             Affected,
                               Priority:             Priority                 ?? ServiceTicketPriorities.Normal,
                               PrivacyLevel:         PrivacyLevel             ?? OpenData.UsersAPI.PrivacyLevel.Private,
                               Location:             Location,
                               GeoLocation:          GeoLocation,
                               ProblemDescriptions:  ProblemDescriptions,
                               StatusIndicators:     StatusIndicators,
                               Reactions:            Reactions,
                               AdditionalInfo:       AdditionalInfo,
                               AttachedFiles:        AttachedFiles,
                               TicketReferences:     TicketReferences,
                               DataLicenses:         DataLicenses,

                               Comment:              null,
                               InReplyTo:            null,
                               CommentReferences:    null,

                               DataSource:           DataSource)
                       },
                       DataSource)

            { }

            #endregion


            public void AppendHistoryEntry(AServiceTicketChangeSet ServiceTicketChangeSet)
            {

                this._ChangeSets.Add(ServiceTicketChangeSet);

            }

        }

        #endregion

    }

}
