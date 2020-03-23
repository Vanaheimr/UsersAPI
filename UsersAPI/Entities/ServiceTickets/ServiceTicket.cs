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

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// A service ticket.
    /// </summary>
    public class ServiceTicket : AServiceTicket
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        private const String _JSONLDContext = "https://opendata.social/contexts/UsersAPI+json/serviceTicket";

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new service ticket.
        /// </summary>
        /// <param name="Id">The unique identification of the service ticket.</param>
        /// <param name="ChangeSets">An enumeration of service ticket change sets.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        public ServiceTicket(ServiceTicket_Id                      Id,
                             IEnumerable<AServiceTicketChangeSet>  ChangeSets,
                             String                                DataSource     = null)

            : base(Id,
                   ChangeSets,
                   DataSource)

        { }

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
        public ServiceTicket(ServiceTicket_Id                     Id,

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

            : base(Id,

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
                   DataLicenses,

                   DataSource)

        { }

        #endregion


        #region ToJSON(...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        /// <param name="IncludeCryptoHash">Include the crypto hash value of this object.</param>
        public override JObject ToJSON(Boolean Embedded           = false,
                                       Boolean IncludeCryptoHash  = false)

            => ToJSON(Embedded:               false,
                      MaxStatus:              null,
                      IncludeStatus:          null,
                      ExpandDataLicenses:     InfoStatus.ShowIdOnly,
                      ExpandAuthorId:         InfoStatus.ShowIdOnly,
                      IncludeComments:        true,
                      IncludeCryptoHash:      true);


        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure, e.g. into a service ticket.</param>
        /// <param name="IncludeCryptoHash">Include the cryptograhical hash value of this object.</param>
        public JObject ToJSON(Boolean                                            Embedded                = false,
                              UInt16?                                            MaxStatus               = null,
                              Func<DateTime, ServiceTicketStatusTypes, Boolean>  IncludeStatus           = null,
                              InfoStatus                                         ExpandDataLicenses      = InfoStatus.ShowIdOnly,
                              InfoStatus                                         ExpandAuthorId          = InfoStatus.ShowIdOnly,
                              Boolean                                            IncludeComments         = true,
                              Boolean                                            IncludeCryptoHash       = true)

            => base.ToJSON(Embedded,
                           MaxStatus,
                           IncludeStatus,
                           ExpandDataLicenses,
                           ExpandAuthorId,
                           IncludeComments,
                           IncludeCryptoHash,
                           json => {

                               if (Embedded)
                                   json["@context"] = JSONLDContext;

                           });

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
        /// <param name="VerifyContext">Verify the JSON-LD context.</param>
        /// <param name="ServiceTicketIdURI">The optional service ticket identification, e.g. from the HTTP URI.</param>
        /// <param name="OverwriteAuthor">Overwrite the author of the service ticket, if given.</param>
        public static Boolean TryParseJSON(JObject                         JSONObject,
                                           AServiceTicketProviderDelegate  ServiceTicketProvider,
                                           UserProviderDelegate            UserProvider,
                                           OrganizationProviderDelegate    OrganizationProvider,
                                           out ServiceTicket               ServiceTicket,
                                           out String                      ErrorResponse,
                                           String                          VerifyContext        = _JSONLDContext,
                                           ServiceTicket_Id?               ServiceTicketIdURI   = null,
                                           OverwriteUserDelegate           OverwriteAuthor      = null)
        {

            try
            {

                ServiceTicket = null;

                if (JSONObject?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Context                    [mandatory if requested]

                if (VerifyContext != null)
                {

                    if (!JSONObject.ParseMandatory("@context",
                                                   "JSON-LD context",
                                                   out String Context,
                                                   out ErrorResponse))
                    {
                        ErrorResponse = @"The JSON-LD ""@context"" information is missing!";
                        return false;
                    }

                    if (Context != VerifyContext && Context != _JSONLDContext)
                    {
                        ErrorResponse = @"The given JSON-LD ""@context"" information '" + Context + "' is not supported!";
                        return false;
                    }

                }

                #endregion


                #region Parse ServiceTicketId            [optional]

                // Verify that a given service ticket identification
                //   is at least valid.
                if (JSONObject.ParseOptionalStruct("@id",
                                                   "service ticket identification",
                                                   ServiceTicket_Id.TryParse,
                                                   out ServiceTicket_Id? ServiceTicketIdBody,
                                                   out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                if (!ServiceTicketIdURI.HasValue && !ServiceTicketIdBody.HasValue)
                {
                    ErrorResponse = "The service ticket identification is missing!";
                    return false;
                }

                if (ServiceTicketIdURI.HasValue && ServiceTicketIdBody.HasValue && ServiceTicketIdURI.Value != ServiceTicketIdBody.Value)
                {
                    ErrorResponse = "The optional service ticket identification given within the JSON body does not match the one given in the URI!";
                    return false;
                }

                #endregion

                // Timestamp?

                #region Parse Author                     [optional]

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

                // Status?

                #region Parse Title                      [mandatory]

                if (JSONObject.ParseMandatory("title",
                                              "title",
                                              out I18NString Title,
                                              out ErrorResponse))
                {

                    if (ErrorResponse != null)
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

                #region Parse Priority                   [optional]

                if (JSONObject.ParseOptionalEnum("priority",
                                                 "priority",
                                                 out ServiceTicketPriorities? Priority,
                                                 out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse PrivacyLevel               [optional]

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

                #region Parse Location                   [optional]

                if (JSONObject.ParseOptional("location",
                                             "location",
                                             out I18NString Location,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse GeoLocation                [optional]

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

                #region Parse Problem descriptions       [optional]

                if (!JSONObject.ParseOptionalI18NHashSet("problemDescriptions",
                                                         "problem descriptions",
                                                         ProblemDescriptionI18N.TryParse,
                                                         out HashSet<ProblemDescriptionI18N> ProblemDescriptions,
                                                         out ErrorResponse))
                {

                    if (ErrorResponse != null)
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

                // Reactions

                #region Parse AdditionalInfo             [optional]

                if (JSONObject.ParseOptional("additionalInfo",
                                             "additional information",
                                             out I18NString AdditionalInfo,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse AttachedFiles              [optional]

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

                // TicketReferences

                #region Parse DataLicenseIds             [optional]

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


                #region Parse History                    [optional]

                var History = new List<AServiceTicketChangeSet>();

                if (JSONObject.ParseOptional("history",
                                             "service ticket history",
                                             out JArray HistoryJSON,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                    if (HistoryJSON != null)
                    {
                        foreach (var history in HistoryJSON)
                        {

                            if (history is JObject historyJSON)
                            {

                                var context = historyJSON.GetString("@context");

                                AServiceTicketChangeSet serviceTicketHistoryEntry = null;

                                switch (context)
                                {

                                    case ServiceTicketChangeSet.JSONLDContext:
                                        if (!ServiceTicketChangeSet.TryParseJSON(historyJSON,
                                                                               ServiceTicketProvider,
                                                                               UserProvider,
                                                                               OrganizationProvider,
                                                                               out ServiceTicketChangeSet serviceTicketHistory,
                                                                               out ErrorResponse))
                                        {
                                            return false;
                                        }

                                        serviceTicketHistoryEntry = serviceTicketHistory;
                                        break;

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

                                }

                                History.Add(serviceTicketHistoryEntry);

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


                ServiceTicket = History.Count > 0

                                    ? new ServiceTicket(ServiceTicketIdBody ?? ServiceTicketIdURI.Value,
                                                        History,
                                                        DataSource)

                                    : new ServiceTicket(Id:                     ServiceTicketIdBody ?? ServiceTicketIdURI.Value,

                                                        Timestamp:              DateTime.UtcNow,
                                                        Author:                 Author,
                                                        Status:                 ServiceTicketStatusTypes.New,
                                                        Title:                  Title,
                                                        Affected:               Affected,
                                                        Priority:               Priority,
                                                        PrivacyLevel:           PrivacyLevel,
                                                        Location:               Location,
                                                        GeoLocation:            GeoLocation,
                                                        ProblemDescriptions:    ProblemDescriptions,
                                                        StatusIndicators:       StatusIndicators,
                                                        Reactions:              null,
                                                        AdditionalInfo:         AdditionalInfo,
                                                        AttachedFiles:          AttachedFiles,
                                                        TicketReferences:       null,
                                                        DataLicenses:           DataLicenses,

                                                        DataSource:             DataSource);


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


        #region (private)  UpdateMyself     (NewServiceTicket)

        private ServiceTicket UpdateMyself(ServiceTicket NewServiceTicket)
        {

            //foreach (var pairing in _Pairings.Where(pairing => pairing.ServiceTicket.Id == Id))
            //    pairing.ServiceTicket = NewServiceTicket;

            return NewServiceTicket;

        }

        #endregion


        #region CopyAllEdgesTo(Target)

        public void CopyAllEdgesTo(ServiceTicket Target)
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
        public static Boolean operator == (ServiceTicket ServiceTicket1, ServiceTicket ServiceTicket2)
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
        public static Boolean operator != (ServiceTicket ServiceTicket1, ServiceTicket ServiceTicket2)
            => !(ServiceTicket1 == ServiceTicket2);

        #endregion

        #region Operator <  (ServiceTicket1, ServiceTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicket1">A service ticket identification.</param>
        /// <param name="ServiceTicket2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ServiceTicket ServiceTicket1, ServiceTicket ServiceTicket2)
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
        public static Boolean operator <= (ServiceTicket ServiceTicket1, ServiceTicket ServiceTicket2)
            => !(ServiceTicket1 > ServiceTicket2);

        #endregion

        #region Operator >  (ServiceTicket1, ServiceTicket2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicket1">A service ticket identification.</param>
        /// <param name="ServiceTicket2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ServiceTicket ServiceTicket1, ServiceTicket ServiceTicket2)
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
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is ServiceTicket ServiceTicket))
                throw new ArgumentException("The given object is not a service ticket!");

            return CompareTo(ServiceTicket);

        }

        #endregion

        #region CompareTo(ServiceTicket)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicket">An service ticket object to compare with.</param>
        public Int32 CompareTo(ServiceTicket ServiceTicket)
        {

            if ((Object) ServiceTicket == null)
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

            var ServiceTicket = Object as ServiceTicket;
            if ((Object) ServiceTicket == null)
                return false;

            return Equals(ServiceTicket);

        }

        #endregion

        #region Equals(ServiceTicket)

        /// <summary>
        /// Compares two service tickets for equality.
        /// </summary>
        /// <param name="ServiceTicket">An service ticket to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ServiceTicket ServiceTicket)
        {

            if ((Object) ServiceTicket == null)
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


        #region ToBuilder(NewServiceTicketId = null)

        /// <summary>
        /// Return a builder for this service ticket.
        /// </summary>
        /// <param name="NewServiceTicketId">An optional new service ticket identification.</param>
        public Builder ToBuilder(ServiceTicket_Id? NewServiceTicketId = null)

            => ChangeSets.Any()

                   ? new Builder(Id,
                                 ChangeSets,
                                 DataSource)

                   : new Builder(NewServiceTicketId ?? Id,

                                 ServiceTicketChangeSet_Id.Random(),
                                 Status.Timestamp,
                                 Author,
                                 Status.Value,
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
                                 DataLicenses,

                                 DataSource);

        #endregion

        #region (class) Builder

        /// <summary>
        /// An service ticket builder.
        /// </summary>
        public class Builder : ABuilder
        {

            #region Constructor(s)

            /// <summary>
            /// Create a new service ticket builder.
            /// </summary>
            /// <param name="Id">The unique identification of the service ticket.</param>
            /// <param name="ChangeSets">An enumeration of service ticket change sets.</param>
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            public Builder(ServiceTicket_Id                      Id,
                           IEnumerable<AServiceTicketChangeSet>  ChangeSets,
                           String                                DataSource  = null)

            : base(Id,
                   ChangeSets,
                   DataSource)

            { }

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
            public Builder(ServiceTicket_Id                     Id,

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

                : base(Id,

                       ServiceTicketChangeSetId,
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
                       DataLicenses,

                       DataSource)

            { }

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
                                           DataSource)

                       : new ServiceTicket(Id,

                                           Status?.Timestamp,
                                           Author,
                                           Status?.Value,
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
                                           DataLicenses,

                                           DataSource);

            #endregion

        }

        #endregion

    }

}
