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

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Distributed;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// A generic abstract service ticket.
    /// </summary>
    public abstract class AGenericServiceTicket<TId, TStatus> : ADistributedEntity<TId>,
                                                                IEntityClass<AGenericServiceTicket<TId, TStatus>>
        where TId     : struct, IId
        where TStatus : struct

    {

        #region Data

        /// <summary>
        /// The default max size of the data set status list.
        /// </summary>
        public const UInt16 DefaultMaxStatusListSize = 30;

        #endregion

        #region Properties

        #region API

        private Object _API;

        /// <summary>
        /// The CardiCloudAPI of this service ticket.
        /// </summary>
        public Object API
        {

            get
            {
                return _API;
            }

            set
            {

                if (_API != null)
                    throw new ArgumentException("Illegal attempt to change the API of this service ticket!");

                _API = value ?? throw new ArgumentException("Illegal attempt to delete the API reference of this service ticket!");

            }

        }

        #endregion

        /// <summary>
        /// The initial author of this service ticket (if known).
        /// </summary>
        public User                                       Author                      { get; }

        #region Status

        protected readonly StatusSchedule<TStatus> _StatusSchedule;

        /// <summary>
        /// The current status of the service ticket.
        /// </summary>
        public Timestamped<TStatus> Status
            => _StatusSchedule.CurrentStatus;

        /// <summary>
        /// The status schedule of the service ticket.
        /// </summary>
        public IEnumerable<Timestamped<TStatus>> StatusSchedule(UInt64? HistorySize = null)
        {

            if (HistorySize.HasValue)
                return _StatusSchedule.Take(HistorySize);

            return _StatusSchedule;

        }

        #endregion

        /// <summary>
        /// The title of the service ticket (10-200 characters).
        /// </summary>
        public I18NString                                 Title                       { get; }

        /// <summary>
        /// The priority of the service ticket.
        /// </summary>
        public ServiceTicketPriorities                    Priority                    { get; }

        /// <summary>
        /// Affected devices or services by this service ticket.
        /// </summary>
        public Affected                                   Affected                    { get; }

        /// <summary>
        /// The location of the problem or broken device.
        /// </summary>
        public I18NString                                 Location                    { get; }

        /// <summary>
        /// The geographical location of the problem or broken device.
        /// </summary>
        public GeoCoordinate?                             GeoLocation                 { get; }

        /// <summary>
        /// An enumeration of well-defined problem descriptions.
        /// </summary>
        public IEnumerable<Tag>                           ProblemDescriptions         { get; }

        /// <summary>
        /// An enumeration of problem indicators.
        /// </summary>
        public IEnumerable<Tag>                           StatusIndicators            { get; }

        /// <summary>
        /// Additional multi-language information related to this service ticket.
        /// </summary>
        public I18NString                                 AdditionalInfo              { get; }

        /// <summary>
        /// An enumeration of URLs to files attached to this service ticket.
        /// </summary>
        public IEnumerable<HTTPPath>                      AttachedFiles               { get; }

        /// <summary>
        /// An enumeration of comments.
        /// </summary>
        public IEnumerable<AServiceTicketHistory>         History                     { get; }


        /// <summary>
        /// Whether the service ticket will be shown in (public) listings.
        /// </summary>
        public PrivacyLevel                               PrivacyLevel                { get; }

        /// <summary>
        /// Optional data licsenses for publishing this data.
        /// </summary>
        public IEnumerable<DataLicense>                   DataLicenses                { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new generic abstract service ticket.
        /// </summary>
        /// <param name="Id">The unique identification of the service ticket.</param>
        /// <param name="Title">The title of the service ticket (10-200 characters).</param>
        /// <param name="Author">The initial author of this service ticket (if known).</param>
        /// <param name="InitialStatus">The initial status of the service ticket.</param>
        /// <param name="Priority">The priority of the service ticket.</param>
        /// <param name="Affected">Affected devices or services by this service ticket.</param>
        /// <param name="ProblemLocation">The location of the problem or broken device.</param>
        /// <param name="GeoLocation">The geographical location of the problem or broken device.</param>
        /// <param name="ProblemDescriptions">An enumeration of well-defined problem descriptions.</param>
        /// <param name="ProblemIndicators">An enumeration of problem indicators.</param>
        /// <param name="AdditionalInfo">A multi-language description of the service ticket.</param>
        /// <param name="AttachedFiles">An enumeration of URLs to files attached to this service ticket.</param>
        /// <param name="History">The service ticket history.</param>
        /// 
        /// <param name="PrivacyLevel">Whether the service ticket will be shown in (public) listings.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        /// <param name="DataLicenses">Optional data licsenses for publishing this data.</param>
        /// 
        /// <param name="MaxPoolStatusListSize">The maximum size of the status list.</param>
        public AGenericServiceTicket(TId                                 Id,
                                     TStatus                             InitialStatus,
                                     I18NString                          Title,
                                     User                                Author                  = null,
                                     ServiceTicketPriorities?            Priority                = null,
                                     Affected                            Affected                = null,
                                     I18NString                          ProblemLocation         = null,
                                     GeoCoordinate?                      GeoLocation             = null,
                                     IEnumerable<Tag>                    ProblemDescriptions     = null,
                                     IEnumerable<Tag>                    ProblemIndicators       = null,
                                     I18NString                          AdditionalInfo          = null,
                                     IEnumerable<HTTPPath>               AttachedFiles           = null,
                                     IEnumerable<AServiceTicketHistory>  History                 = null,

                                     PrivacyLevel?                       PrivacyLevel            = null,
                                     IEnumerable<DataLicense>            DataLicenses            = null,
                                     String                              DataSource              = null,

                                     UInt16                              MaxPoolStatusListSize   = DefaultMaxStatusListSize)

            : this(Id,
                   Title,
                   Author,
                   new StatusSchedule<TStatus>(InitialStatus, MaxPoolStatusListSize),
                   Priority,
                   Affected,
                   ProblemLocation,
                   GeoLocation,
                   ProblemDescriptions,
                   ProblemIndicators,
                   AdditionalInfo,
                   AttachedFiles,
                   History,

                   PrivacyLevel,
                   DataLicenses,
                   DataSource,

                   MaxPoolStatusListSize)

        { }


        /// <summary>
        /// Create a new service ticket.
        /// </summary>
        /// <param name="Id">The unique identification of the service ticket.</param>
        /// <param name="Title">The title of the service ticket (10-200 characters).</param>
        /// <param name="Author">The initial author of this service ticket (if known).</param>
        /// <param name="Status">The status of the service ticket.</param>
        /// <param name="Priority">The priority of the service ticket.</param>
        /// <param name="Affected">Affected devices or services by this service ticket.</param>
        /// <param name="Location">The location of the problem or broken device.</param>
        /// <param name="GeoLocation">The geographical location of the problem or broken device.</param>
        /// <param name="ProblemDescriptions">An enumeration of well-defined problem descriptions.</param>
        /// <param name="StatusIndicators">An enumeration of problem indicators.</param>
        /// <param name="AdditionalInfo">A multi-language description of the service ticket.</param>
        /// <param name="AttachedFiles">An enumeration of URLs to files attached to this service ticket.</param>
        /// <param name="History">The service ticket history.</param>
        /// 
        /// <param name="PrivacyLevel">Whether the service ticket will be shown in (public) listings.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        /// <param name="DataLicenses">Optional data licsenses for publishing this data.</param>
        /// 
        /// <param name="MaxPoolStatusListSize">The maximum size of the status list.</param>
        public AGenericServiceTicket(TId                                 Id,
                                     I18NString                          Title,
                                     User                                Author                  = null,
                                     IEnumerable<Timestamped<TStatus>>   Status                  = null,
                                     ServiceTicketPriorities?            Priority                = null,
                                     Affected                            Affected                = null,
                                     I18NString                          Location                = null,
                                     GeoCoordinate?                      GeoLocation             = null,
                                     IEnumerable<Tag>                    ProblemDescriptions     = null,
                                     IEnumerable<Tag>                    StatusIndicators        = null,
                                     I18NString                          AdditionalInfo          = null,
                                     IEnumerable<HTTPPath>               AttachedFiles           = null,
                                     IEnumerable<AServiceTicketHistory>  History                 = null,

                                     PrivacyLevel?                       PrivacyLevel            = null,
                                     IEnumerable<DataLicense>            DataLicenses            = null,
                                     String                              DataSource              = null,

                                     UInt16                              MaxPoolStatusListSize   = DefaultMaxStatusListSize)

            : base(Id,
                   DataSource)

        {

            if (Title.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Title), "The given title must not be null or empty!");

            this.Title                = Title;
            this.Author               = Author;

            if (Status?.Any() != true)
                Status = new Timestamped<TStatus>[] { default(TStatus) };

            this._StatusSchedule      = new StatusSchedule<TStatus>(Status, MaxPoolStatusListSize);

            this.Priority             = Priority            ?? ServiceTicketPriorities.Normal;
            this.Affected             = Affected            ?? new Affected();
            this.Location             = Location            ?? I18NString.Empty;
            this.GeoLocation          = GeoLocation;
            this.ProblemDescriptions  = ProblemDescriptions != null ? ProblemDescriptions.Distinct() : new Tag[0];
            this.StatusIndicators     = StatusIndicators    != null ? StatusIndicators.   Distinct() : new Tag[0];
            this.AdditionalInfo       = AdditionalInfo      ?? new I18NString();
            this.AttachedFiles        = AttachedFiles       != null ? AttachedFiles.      Distinct() : new HTTPPath[0];
            this.History              = History             ?? new AServiceTicketHistory[0];

            this.PrivacyLevel         = PrivacyLevel        ?? OpenData.UsersAPI.PrivacyLevel.Private;
            this.DataLicenses         = DataLicenses        ?? new DataLicense[0];

            CalcHash();

        }

        #endregion


        #region (private)  UpdateMyself     (NewServiceTicket)

        private AGenericServiceTicket<TId, TStatus> UpdateMyself(AGenericServiceTicket<TId, TStatus> NewServiceTicket)
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

        public void CopyAllEdgesTo(AGenericServiceTicket<TId, TStatus> Target)
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
        public static Boolean operator == (AGenericServiceTicket<TId, TStatus> ServiceTicket1, AGenericServiceTicket<TId, TStatus> ServiceTicket2)
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
        public static Boolean operator != (AGenericServiceTicket<TId, TStatus> ServiceTicket1, AGenericServiceTicket<TId, TStatus> ServiceTicket2)
            => !(ServiceTicket1 == ServiceTicket2);

        #endregion

        #region Operator <  (ServiceTicket1, ServiceTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicket1">A service ticket identification.</param>
        /// <param name="ServiceTicket2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (AGenericServiceTicket<TId, TStatus> ServiceTicket1, AGenericServiceTicket<TId, TStatus> ServiceTicket2)
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
        public static Boolean operator <= (AGenericServiceTicket<TId, TStatus> ServiceTicket1, AGenericServiceTicket<TId, TStatus> ServiceTicket2)
            => !(ServiceTicket1 > ServiceTicket2);

        #endregion

        #region Operator >  (ServiceTicket1, ServiceTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicket1">A service ticket identification.</param>
        /// <param name="ServiceTicket2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (AGenericServiceTicket<TId, TStatus> ServiceTicket1, AGenericServiceTicket<TId, TStatus> ServiceTicket2)
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
        public static Boolean operator >= (AGenericServiceTicket<TId, TStatus> ServiceTicket1, AGenericServiceTicket<TId, TStatus> ServiceTicket2)
            => !(ServiceTicket1 < ServiceTicket2);

        #endregion

        #endregion

        #region IComparable<ServiceTicket> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public new Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is AGenericServiceTicket<TId, TStatus> ServiceTicket))
                throw new ArgumentException("The given object is not a service ticket!");

            return CompareTo(ServiceTicket);

        }

        #endregion

        #region CompareTo(ServiceTicket)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicket">An service ticket object to compare with.</param>
        public Int32 CompareTo(AGenericServiceTicket<TId, TStatus> ServiceTicket)
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

            if (!(Object is AGenericServiceTicket<TId, TStatus> ServiceTicket))
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
        public Boolean Equals(AGenericServiceTicket<TId, TStatus> ServiceTicket)
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


        #region (class) Builder

        /// <summary>
        /// A service ticket builder.
        /// </summary>
        public abstract class ABuilder
        {

            #region Properties

            /// <summary>
            /// The service ticket identification.
            /// </summary>
            public TId                                                    Id                         { get; set; }

            /// <summary>
            /// The title of the service ticket (10-200 characters).
            /// </summary>
            public I18NString                                             Title                      { get; set; }

            /// <summary>
            /// The initial author of this service ticket (if known).
            /// </summary>
            public User                                                   Author                     { get; set; }

            /// <summary>
            /// The priority of the service ticket.
            /// </summary>
            public ServiceTicketPriorities                                Priority                   { get; set; }

            /// <summary>
            /// The current status of the service ticket.
            /// </summary>
            public Timestamped<TStatus>                                   Status
                => StatusSchedule.CurrentStatus;

            /// <summary>
            /// The status schedule of the service ticket.
            /// </summary>
            public StatusSchedule<TStatus>                                StatusSchedule             { get; set; }

            /// <summary>
            /// Related service tickets.
            /// </summary>
            public Affected.Builder                                       Affected                   { get; }

            /// <summary>
            /// The location of the problem.
            /// </summary>
            public I18NString                                             ProblemLocation            { get; set; }

            /// <summary>
            /// The geographical location of the problem.
            /// </summary>
            public GeoCoordinate?                                         GeoLocation                { get; set; }

            /// <summary>
            /// An enumeration of well-defined problem descriptions.
            /// </summary>
            public HashSet<Tag>                                           ProblemDescriptions        { get; set; }

            /// <summary>
            /// An enumeration of problem indicators.
            /// </summary>
            public HashSet<Tag>                                           ProblemIndicators          { get; set; }

            /// <summary>
            /// Additional multi-language information related to this service ticket  .
            /// </summary>
            public I18NString                                             AdditionalInfo             { get; set; }

            /// <summary>
            /// An enumeration of URLs to files attached to this service ticket.
            /// </summary>
            public HashSet<HTTPPath>                                      AttachedFiles              { get; set; }

            /// <summary>
            /// An enumeration of comments.
            /// </summary>
            public HashSet<AServiceTicketHistory>                         History                    { get; set; }


            /// <summary>
            /// Whether the service ticket will be shown in (public) listings.
            /// </summary>
            public PrivacyLevel                                           PrivacyLevel               { get; set; }

            /// <summary>
            /// Optional data licsenses for publishing this data.
            /// </summary>
            public IEnumerable<DataLicense>                               DataLicenses               { get; set; }

            /// <summary>
            /// The source of this information, e.g. an automatic importer.
            /// </summary>
            public String                                                 DataSource                 { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new service ticket builder.
            /// </summary>
            /// <param name="Id">The unique identification of the service ticket.</param>
            /// <param name="Title">The title of the service ticket (10-200 characters).</param>
            /// <param name="Author">The initial author of this service ticket (if known).</param>
            /// <param name="Status">The status of the service ticket.</param>
            /// <param name="Priority">The priority of the service ticket.</param>
            /// <param name="Affected">Affected devices or services by this service ticket.
            /// <param name="ProblemLocation">The location of the problem or broken device.</param>
            /// <param name="GeoLocation">The geographical location of the problem or broken device.</param>
            /// <param name="ProblemDescriptions">An enumeration of well-defined problem descriptions.</param>
            /// <param name="ProblemIndicators">An enumeration of problem indicators.</param>
            /// <param name="AdditionalInfo">A multi-language description of the service ticket.</param>
            /// <param name="AttachedFiles">An enumeration of URLs to files attached to this service ticket.</param>
            /// 
            /// <param name="PrivacyLevel">Whether the service ticket will be shown in (public) listings.</param>
            /// <param name="DataLicenses">Optional data licsenses for publishing this data.</param>
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            /// 
            /// <param name="MaxPoolStatusListSize">The maximum size of the status list.</param>
            public ABuilder(TId                                                       Id,
                            I18NString                                                Title,
                            User                                                      Author                    = null,
                            IEnumerable<Timestamped<TStatus>>                         Status                    = null,
                            ServiceTicketPriorities?                                  Priority                  = null,
                            Affected                                                  Affected                  = null,
                            I18NString                                                ProblemLocation           = null,
                            GeoCoordinate?                                            GeoLocation               = null,
                            IEnumerable<Tag>                                          ProblemDescriptions       = null,
                            IEnumerable<Tag>                                          ProblemIndicators         = null,
                            I18NString                                                AdditionalInfo            = null,
                            IEnumerable<HTTPPath>                                     AttachedFiles             = null,
                            IEnumerable<AServiceTicketHistory>                        History                   = null,

                            PrivacyLevel?                                             PrivacyLevel              = null,
                            IEnumerable<DataLicense>                                  DataLicenses              = null,
                            String                                                    DataSource                = null,

                            UInt16                                                    MaxPoolStatusListSize     = DefaultMaxStatusListSize)

            {

                if (Title.IsNullOrEmpty())
                    throw new ArgumentNullException(nameof(Title), "The given title must not be null or empty!");

                this.Id                       = Id;
                this.Title                    = Title;
                this.Author                   = Author;
                this.StatusSchedule           = new StatusSchedule<TStatus>(Status, MaxPoolStatusListSize);
                this.Priority                 = Priority                ?? ServiceTicketPriorities.Normal;
                this.Affected                 = Affected                != null ? Affected.ToBuilder() : new Affected.Builder();
                this.ProblemLocation          = ProblemLocation         ?? I18NString.Empty;
                this.GeoLocation              = GeoLocation;
                this.ProblemDescriptions      = ProblemDescriptions     != null ? new HashSet<Tag>(ProblemDescriptions)                          : new HashSet<Tag>();
                this.ProblemIndicators        = ProblemIndicators       != null ? new HashSet<Tag>(ProblemIndicators)                            : new HashSet<Tag>();
                this.AdditionalInfo           = AdditionalInfo          ?? new I18NString();
                this.AttachedFiles            = AttachedFiles           != null ? new HashSet<HTTPPath>(AttachedFiles)                           : new HashSet<HTTPPath>();
                this.History                  = History                 != null ? new HashSet<AServiceTicketHistory>(History)                    : new HashSet<AServiceTicketHistory>();

                this.PrivacyLevel             = PrivacyLevel            ?? social.OpenData.UsersAPI.PrivacyLevel.Private;
                this.DataLicenses             = DataLicenses            ?? new DataLicense[0];
                this.DataSource               = DataSource              ?? "";

            }

            #endregion

        }

        #endregion

    }

}
