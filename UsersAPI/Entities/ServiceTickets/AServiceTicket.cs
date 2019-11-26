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
    public abstract class AServiceTicket : AGenericServiceTicket<ServiceTicket_Id, ServiceTicketStatusTypes>
    {

        #region Constructor(s)

        /// <summary>
        /// Create a new service ticket.
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
        public AServiceTicket(ServiceTicket_Id                    Id,
                              ServiceTicketStatusTypes            InitialStatus,
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
                   new StatusSchedule<ServiceTicketStatusTypes>(InitialStatus, MaxPoolStatusListSize),
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
        public AServiceTicket(ServiceTicket_Id                                    Id,
                              I18NString                                          Title,
                              User                                                Author                  = null,
                              IEnumerable<Timestamped<ServiceTicketStatusTypes>>  Status                  = null,
                              ServiceTicketPriorities?                            Priority                = null,
                              Affected                                            Affected                = null,
                              I18NString                                          Location                = null,
                              GeoCoordinate?                                      GeoLocation             = null,
                              IEnumerable<Tag>                                    ProblemDescriptions     = null,
                              IEnumerable<Tag>                                    StatusIndicators        = null,
                              I18NString                                          AdditionalInfo          = null,
                              IEnumerable<HTTPPath>                               AttachedFiles           = null,
                              IEnumerable<AServiceTicketHistory>                  History                 = null,

                              PrivacyLevel?                                       PrivacyLevel            = null,
                              IEnumerable<DataLicense>                            DataLicenses            = null,
                              String                                              DataSource              = null,

                              UInt16                                              MaxPoolStatusListSize   = DefaultMaxStatusListSize)

            : base(Id,
                   Title,
                   Author,
                   Status,
                   Priority,
                   Affected,
                   Location,
                   GeoLocation,
                   ProblemDescriptions,
                   StatusIndicators,
                   AdditionalInfo,
                   AttachedFiles,
                   History,

                   PrivacyLevel,
                   DataLicenses,
                   DataSource,

                   MaxPoolStatusListSize)

        {

        }

        #endregion


        #region (protected) ToJSON(...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure, e.g. into a service ticket.</param>
        /// <param name="IncludeCryptoHash">Include the cryptograhical hash value of this object.</param>
        public virtual JObject ToJSON(Boolean                                            Embedded                = false,
                                      UInt16?                                            MaxStatus               = null,
                                      Func<DateTime, ServiceTicketStatusTypes, Boolean>  IncludeStatus           = null,
                                      InfoStatus                                         ExpandDataLicenses      = InfoStatus.ShowIdOnly,
                                      InfoStatus                                         ExpandAuthorId          = InfoStatus.ShowIdOnly,
                                      Boolean                                            IncludeComments         = true,
                                      Boolean                                            IncludeCryptoHash       = true,
                                      Action<JObject>                                    Configurator            = null)

        {

            var JSON = JSONObject.Create(

                   Id.ToJSON("@id"),

                   //Embedded
                   //    ? null
                   //    : new JProperty("@context",             JSONLDContext),

                   new JProperty("title",                      Title.ToJSON()),

                   new JProperty("status",                     new JObject(_StatusSchedule.
                                                                               Where (kvp => IncludeStatus != null ? IncludeStatus(kvp.Timestamp, kvp.Value) : true).
                                                                               Take  (MaxStatus).
                                                                               Select(status => new JProperty(status.Timestamp.ToIso8601(),
                                                                                                              status.Value.    ToString().ToLower()))
                                                                          )),

                   !Affected.IsEmpty && Affected != null
                       ? new JProperty("affected", Affected.ToJSON())
                       : null,

                   Author != null
                       ? ExpandAuthorId.Switch(
                             () => new JProperty("author",     new JObject(
                                                                   new JProperty("id",   Author.Id.ToString()),
                                                                   new JProperty("name", Author.Name)
                                                               )),
                             () => new JProperty("author",     Author.ToJSON()))
                       : null,

                   new JProperty("priority", Priority.ToString().ToLower()),

                   Location.IsNeitherNullNorEmpty()
                       ? Location.ToJSON("location")
                       : null,

                   GeoLocation?.ToJSON("geoLocation"),

                   ProblemDescriptions.SafeAny()
                       ? new JProperty("problemDescriptions",  new JArray(ProblemDescriptions.SafeSelect(tag => tag.Id.ToString())))
                       : null,

                   StatusIndicators.SafeAny()
                       ? new JProperty("statusIndicators",     new JArray(StatusIndicators.SafeSelect(tag => tag.Id.ToString())))
                       : null,

                   AdditionalInfo.IsNeitherNullNorEmpty()
                       ? AdditionalInfo.ToJSON("additionalInfo")
                       : null,

                   AttachedFiles.SafeAny()
                       ? new JProperty("attachedFiles",        new JArray(AttachedFiles.SafeSelect(url     => url.ToString())))
                       : null,

                   IncludeComments && History.SafeAny()
                       ? new JProperty("history",              new JArray(History.
                                                                              OrderByDescending(history => history.Timestamp).
                                                                              SafeSelect       (history => history.ToJSON())))
                       : null,


                   // MaxPoolStatusListSize

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

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public new Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is AServiceTicket ServiceTicket))
                throw new ArgumentException("The given object is not a service ticket!");

            return CompareTo(ServiceTicket);

        }

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
        public new abstract class ABuilder : AGenericServiceTicket<ServiceTicket_Id, ServiceTicketStatusTypes>.ABuilder
        {

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
            public ABuilder(ServiceTicket_Id                                          Id,
                            I18NString                                                Title,
                            User                                                      Author                    = null,
                            IEnumerable<Timestamped<ServiceTicketStatusTypes>>        Status                    = null,
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

                : base(Id,
                       Title,
                       Author,
                       Status,
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

            {

            }

            #endregion

        }

        #endregion

    }

}
