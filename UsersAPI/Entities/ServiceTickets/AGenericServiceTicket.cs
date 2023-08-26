///*
// * Copyright (c) 2014-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
// * This file is part of UsersAPI <https://www.github.com/Vanaheimr/UsersAPI>
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using System;
//using System.Linq;
//using System.Collections.Generic;

//using org.GraphDefined.Vanaheimr.Aegir;
//using org.GraphDefined.Vanaheimr.Illias;
//using org.GraphDefined.Vanaheimr.Hermod;
//using org.GraphDefined.Vanaheimr.Hermod.HTTP;
//

//#endregion

//namespace social.OpenData.UsersAPI
//{

//    /// <summary>
//    /// A generic abstract service ticket.
//    /// </summary>
//    public abstract class AGenericServiceTicket : AEntity<ServiceTicket_Id>,
//                                                  IEntityClass<AGenericServiceTicket>
//    {

//        #region Data


//        #endregion

//        #region Properties


//        #endregion

//        #region Constructor(s)

//        /// <summary>
//        /// Create a new service ticket.
//        /// </summary>
//        /// <param name="Id">The unique identification of the service ticket.</param>
//        /// <param name="Title">The title of the service ticket (10-200 characters).</param>
//        /// <param name="Author">The initial author of this service ticket (if known).</param>
//        /// <param name="Affected">Affected devices or services by this service ticket.</param>
//        /// <param name="Location">The location of the problem or broken device.</param>
//        /// <param name="GeoLocation">The geographical location of the problem or broken device.</param>
//        /// <param name="ProblemDescriptions">An enumeration of well-defined problem descriptions.</param>
//        /// <param name="StatusIndicators">An enumeration of problem indicators.</param>
//        /// <param name="AdditionalInfo">A multi-language description of the service ticket.</param>
//        /// <param name="AttachedFiles">An enumeration of URLs to files attached to this service ticket.</param>
//        /// <param name="ChangeSets">An enumeration of service ticket change sets.</param>
//        /// 
//        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
//        public AGenericServiceTicket(ServiceTicket_Id                             Id,
//                                     I18NString                                   Title,
//                                     User                                         Author                = null,
//                                     Affected                                     Affected              = null,
//                                     I18NString                                   Location              = null,
//                                     GeoCoordinate?                               GeoLocation           = null,
//                                     IEnumerable<Tag>                             ProblemDescriptions   = null,
//                                     IEnumerable<Tag>                             StatusIndicators      = null,
//                                     I18NString                                   AdditionalInfo        = null,
//                                     IEnumerable<HTTPPath>                        AttachedFiles         = null,
//                                     IEnumerable<AServiceTicketChangeSet>           History               = null,

//                                     String                                       DataSource            = null)

//            : base(Id,
//                   DataSource)

//        {



//        }

//        #endregion


//        #region (private)  UpdateMyself     (NewServiceTicket)

//        private AGenericServiceTicket<TId, TStatus> UpdateMyself(AGenericServiceTicket<TId, TStatus> NewServiceTicket)
//        {

//            //foreach (var pairing in _Pairings.Where(pairing => pairing.ServiceTicket.Id == Id))
//            //    pairing.ServiceTicket = NewServiceTicket;

//            return NewServiceTicket;

//        }

//        #endregion

//        #region (internal) ChangeStatus     (NewStatus)

//        ///// <summary>
//        ///// Change the status of this service ticket.
//        ///// </summary>
//        ///// <param name="NewStatus">The new status.</param>
//        //internal ServiceTicket ChangeStatus(ServiceTicketStatusTypes NewStatus)
//        //{

//        //    if (NewStatus != Status.Value)
//        //        return UpdateMyself(new ServiceTicket(Id,
//        //                                              Title,
//        //                                              Author,
//        //                                              _StatusSchedule.Insert(NewStatus),
//        //                                              Priority,
//        //                                              Affected,
//        //                                              Location,
//        //                                              GeoLocation,
//        //                                              ProblemDescriptions,
//        //                                              StatusIndicators,
//        //                                              AdditionalInfo,
//        //                                              AttachedFiles,
//        //                                              History,

//        //                                              PrivacyLevel,
//        //                                              DataLicenses,
//        //                                              DataSource,

//        //                                              _StatusSchedule.MaxStatusHistorySize));

//        //    return this;

//        //}


//        ///// <summary>
//        ///// Change the status of this service ticket.
//        ///// </summary>
//        ///// <param name="NewStatus">The new status.</param>
//        //public ServiceTicket ChangeStatus(Timestamped<TStatus> NewStatus)
//        //{

//        //    if (NewStatus != Status.Value)
//        //        return UpdateMyself(new ServiceTicket(Id,
//        //                                              Title,
//        //                                              Author,
//        //                                              _StatusSchedule.Insert(NewStatus),
//        //                                              Priority,
//        //                                              Affected,
//        //                                              Location,
//        //                                              GeoLocation,
//        //                                              ProblemDescriptions,
//        //                                              StatusIndicators,
//        //                                              AdditionalInfo,
//        //                                              AttachedFiles,
//        //                                              History,

//        //                                              PrivacyLevel,
//        //                                              DataLicenses,
//        //                                              DataSource,

//        //                                              _StatusSchedule.     MaxStatusHistorySize));

//        //    return this;

//        //}

//        #endregion

//        #region CopyAllEdgesTo(Target)

//        public void CopyAllEdgesTo(AGenericServiceTicket<TId, TStatus> Target)
//        {


//        }

//        #endregion


//        #region Operator overloading

//        #region Operator == (ServiceTicket1, ServiceTicket2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="ServiceTicket1">A service ticket identification.</param>
//        /// <param name="ServiceTicket2">Another service ticket identification.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator == (AGenericServiceTicket<TId, TStatus> ServiceTicket1, AGenericServiceTicket<TId, TStatus> ServiceTicket2)
//        {

//            // If both are null, or both are same instance, return true.
//            if (Object.ReferenceEquals(ServiceTicket1, ServiceTicket2))
//                return true;

//            // If one is null, but not both, return false.
//            if (((Object) ServiceTicket1 == null) || ((Object) ServiceTicket2 == null))
//                return false;

//            return ServiceTicket1.Equals(ServiceTicket2);

//        }

//        #endregion

//        #region Operator != (ServiceTicket1, ServiceTicket2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="ServiceTicket1">A service ticket identification.</param>
//        /// <param name="ServiceTicket2">Another service ticket identification.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator != (AGenericServiceTicket<TId, TStatus> ServiceTicket1, AGenericServiceTicket<TId, TStatus> ServiceTicket2)
//            => !(ServiceTicket1 == ServiceTicket2);

//        #endregion

//        #region Operator <  (ServiceTicket1, ServiceTicket2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="ServiceTicket1">A service ticket identification.</param>
//        /// <param name="ServiceTicket2">Another service ticket identification.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator < (AGenericServiceTicket<TId, TStatus> ServiceTicket1, AGenericServiceTicket<TId, TStatus> ServiceTicket2)
//        {

//            if ((Object) ServiceTicket1 == null)
//                throw new ArgumentNullException(nameof(ServiceTicket1), "The given ServiceTicket1 must not be null!");

//            return ServiceTicket1.CompareTo(ServiceTicket2) < 0;

//        }

//        #endregion

//        #region Operator <= (ServiceTicket1, ServiceTicket2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="ServiceTicket1">A service ticket identification.</param>
//        /// <param name="ServiceTicket2">Another service ticket identification.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator <= (AGenericServiceTicket<TId, TStatus> ServiceTicket1, AGenericServiceTicket<TId, TStatus> ServiceTicket2)
//            => !(ServiceTicket1 > ServiceTicket2);

//        #endregion

//        #region Operator >  (ServiceTicket1, ServiceTicket2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="ServiceTicket1">A service ticket identification.</param>
//        /// <param name="ServiceTicket2">Another service ticket identification.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator > (AGenericServiceTicket<TId, TStatus> ServiceTicket1, AGenericServiceTicket<TId, TStatus> ServiceTicket2)
//        {

//            if ((Object) ServiceTicket1 == null)
//                throw new ArgumentNullException(nameof(ServiceTicket1), "The given ServiceTicket1 must not be null!");

//            return ServiceTicket1.CompareTo(ServiceTicket2) > 0;

//        }

//        #endregion

//        #region Operator >= (ServiceTicket1, ServiceTicket2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="ServiceTicket1">A service ticket identification.</param>
//        /// <param name="ServiceTicket2">Another service ticket identification.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator >= (AGenericServiceTicket<TId, TStatus> ServiceTicket1, AGenericServiceTicket<TId, TStatus> ServiceTicket2)
//            => !(ServiceTicket1 < ServiceTicket2);

//        #endregion

//        #endregion

//        #region IComparable<ServiceTicket> Members

//        #region CompareTo(Object)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="Object">An object to compare with.</param>
//        public new Int32 CompareTo(Object Object)
//        {

//            if (Object == null)
//                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

//            if (!(Object is AGenericServiceTicket<TId, TStatus> ServiceTicket))
//                throw new ArgumentException("The given object is not a service ticket!");

//            return CompareTo(ServiceTicket);

//        }

//        #endregion

//        #region CompareTo(ServiceTicket)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="ServiceTicket">An service ticket object to compare with.</param>
//        public Int32 CompareTo(AGenericServiceTicket<TId, TStatus> ServiceTicket)
//        {

//            if (ServiceTicket is null)
//                throw new ArgumentNullException(nameof(ServiceTicket), "The given service ticket must not be null!");

//            return Id.CompareTo(ServiceTicket.Id);

//        }

//        #endregion

//        #endregion

//        #region IEquatable<ServiceTicket> Members

//        #region Equals(Object)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="Object">An object to compare with.</param>
//        /// <returns>true|false</returns>
//        public override Boolean Equals(Object Object)
//        {

//            if (Object == null)
//                return false;

//            if (!(Object is AGenericServiceTicket<TId, TStatus> ServiceTicket))
//                throw new ArgumentException("The given object is not a service ticket!");

//            return Equals(ServiceTicket);

//        }

//        #endregion

//        #region Equals(ServiceTicket)

//        /// <summary>
//        /// Compares two service tickets for equality.
//        /// </summary>
//        /// <param name="ServiceTicket">An service ticket to compare with.</param>
//        /// <returns>True if both match; False otherwise.</returns>
//        public Boolean Equals(AGenericServiceTicket<TId, TStatus> ServiceTicket)
//        {

//            if (ServiceTicket is null)
//                return false;

//            return Id.Equals(ServiceTicket.Id);

//        }

//        #endregion

//        #endregion

//        #region (override) GetHashCode()

//        /// <summary>
//        /// Get the hashcode of this object.
//        /// </summary>
//        public override Int32 GetHashCode()
//            => Id.GetHashCode();

//        #endregion


//        #region (class) Builder

//        /// <summary>
//        /// A service ticket builder.
//        /// </summary>
//        public abstract class ABuilder
//        {

//            #region Properties

//            /// <summary>
//            /// The service ticket identification.
//            /// </summary>
//            public ServiceTicket_Id                                       Id                         { get; set; }

//            /// <summary>
//            /// The title of the service ticket (10-200 characters).
//            /// </summary>
//            public I18NString                                             Title                      { get; set; }

//            /// <summary>
//            /// The initial author of this service ticket (if known).
//            /// </summary>
//            public User                                                   Author                     { get; set; }

//            /// <summary>
//            /// Related service tickets.
//            /// </summary>
//            public Affected.Builder                                       Affected                   { get; }

//            /// <summary>
//            /// The location of the problem.
//            /// </summary>
//            public I18NString                                             ProblemLocation            { get; set; }

//            /// <summary>
//            /// The geographical location of the problem.
//            /// </summary>
//            public GeoCoordinate?                                         GeoLocation                { get; set; }

//            /// <summary>
//            /// An enumeration of well-defined problem descriptions.
//            /// </summary>
//            public HashSet<Tag>                                           ProblemDescriptions        { get; set; }

//            /// <summary>
//            /// An enumeration of problem indicators.
//            /// </summary>
//            public HashSet<Tag>                                           ProblemIndicators          { get; set; }

//            /// <summary>
//            /// Additional multi-language information related to this service ticket  .
//            /// </summary>
//            public I18NString                                             AdditionalInfo             { get; set; }

//            /// <summary>
//            /// An enumeration of URLs to files attached to this service ticket.
//            /// </summary>
//            public HashSet<HTTPPath>                                      AttachedFiles              { get; set; }

//            /// <summary>
//            /// An enumeration of comments.
//            /// </summary>
//            public HashSet<AServiceTicketChangeSet>                         History                    { get; set; }


//            /// <summary>
//            /// The source of this information, e.g. an automatic importer.
//            /// </summary>
//            public String                                                 DataSource                 { get; set; }

//            #endregion

//            #region Constructor(s)

//            /// <summary>
//            /// Create a new service ticket builder.
//            /// </summary>
//            /// <param name="Id">The unique identification of the service ticket.</param>
//            /// <param name="Title">The title of the service ticket (10-200 characters).</param>
//            /// <param name="Author">The initial author of this service ticket (if known).</param>
//            /// <param name="Status">The status of the service ticket.</param>
//            /// <param name="Priority">The priority of the service ticket.</param>
//            /// <param name="Affected">Affected devices or services by this service ticket.
//            /// <param name="ProblemLocation">The location of the problem or broken device.</param>
//            /// <param name="GeoLocation">The geographical location of the problem or broken device.</param>
//            /// <param name="ProblemDescriptions">An enumeration of well-defined problem descriptions.</param>
//            /// <param name="ProblemIndicators">An enumeration of problem indicators.</param>
//            /// <param name="AdditionalInfo">A multi-language description of the service ticket.</param>
//            /// <param name="AttachedFiles">An enumeration of URLs to files attached to this service ticket.</param>
//            /// 
//            /// <param name="PrivacyLevel">Whether the service ticket will be shown in (public) listings.</param>
//            /// <param name="DataLicenses">Optional data licsenses for publishing this data.</param>
//            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
//            /// 
//            /// <param name="MaxPoolStatusListSize">The maximum size of the status list.</param>
//            public ABuilder(ServiceTicket_Id                                          Id,
//                            I18NString                                                Title,
//                            User                                                      Author                    = null,
//                            Affected                                                  Affected                  = null,
//                            I18NString                                                ProblemLocation           = null,
//                            GeoCoordinate?                                            GeoLocation               = null,
//                            IEnumerable<Tag>                                          ProblemDescriptions       = null,
//                            IEnumerable<Tag>                                          ProblemIndicators         = null,
//                            I18NString                                                AdditionalInfo            = null,
//                            IEnumerable<HTTPPath>                                     AttachedFiles             = null,
//                            IEnumerable<AServiceTicketChangeSet>                        History                   = null,

//                            String                                                    DataSource                = null)

//            {

//                if (Title.IsNullOrEmpty())
//                    throw new ArgumentNullException(nameof(Title), "The given title must not be null or empty!");

//                this.Id                       = Id;
//                this.Title                    = Title;
//                this.Author                   = Author;
//                this.Affected                 = Affected                != null ? Affected.ToBuilder() : new Affected.Builder();
//                this.ProblemLocation          = ProblemLocation         ?? I18NString.Empty;
//                this.GeoLocation              = GeoLocation;
//                this.ProblemDescriptions      = ProblemDescriptions     != null ? new HashSet<Tag>(ProblemDescriptions)                          : new HashSet<Tag>();
//                this.ProblemIndicators        = ProblemIndicators       != null ? new HashSet<Tag>(ProblemIndicators)                            : new HashSet<Tag>();
//                this.AdditionalInfo           = AdditionalInfo          ?? new I18NString();
//                this.AttachedFiles            = AttachedFiles           != null ? new HashSet<HTTPPath>(AttachedFiles)                           : new HashSet<HTTPPath>();
//                this.History                  = History                 != null ? new HashSet<AServiceTicketChangeSet>(History)           : new HashSet<AServiceTicketChangeSet>();

//                this.DataSource               = DataSource              ?? "";

//            }

//            #endregion

//        }

//        #endregion

//    }

//}
