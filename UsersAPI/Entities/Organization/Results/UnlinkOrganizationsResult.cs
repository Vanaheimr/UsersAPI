﻿/*
 * Copyright (c) 2014-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of OrganizationOutsAPI <https://www.github.com/Vanaheimr/OrganizationOutsAPI>
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace social.OpenData.UsersAPI
{

    public class UnlinkOrganizationsResult : AResult<Organization, Organization>
    {

        public Organization                        OrganizationOut
            => Object1;

        public Organization2OrganizationEdgeLabel  EdgeLabel       { get; }

        public Organization                        OrganizationIn
            => Object2;


        public UnlinkOrganizationsResult(Organization                        OrganizationOut,
                                         Organization2OrganizationEdgeLabel  EdgeLabel,
                                         Organization                        OrganizationIn,
                                         EventTracking_Id                    EventTrackingId,
                                         Boolean                             IsSuccess,
                                         String                              Argument           = null,
                                         I18NString                          ErrorDescription   = null)

            : base(OrganizationOut,
                   OrganizationIn,
                   EventTrackingId,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        {

            this.EdgeLabel = EdgeLabel;

        }


        public static UnlinkOrganizationsResult Success(Organization                        OrganizationOut,
                                                        Organization2OrganizationEdgeLabel  EdgeLabel,
                                                        Organization                        OrganizationIn,
                                                        EventTracking_Id                    EventTrackingId)

            => new UnlinkOrganizationsResult(OrganizationOut,
                                             EdgeLabel,
                                             OrganizationIn,
                                             EventTrackingId,
                                             true);


        public static UnlinkOrganizationsResult ArgumentError(Organization                        OrganizationOut,
                                                              Organization2OrganizationEdgeLabel  EdgeLabel,
                                                              Organization                        OrganizationIn,
                                                              EventTracking_Id                    EventTrackingId,
                                                              String                              Argument,
                                                              String                              Description)

            => new UnlinkOrganizationsResult(OrganizationOut,
                                             EdgeLabel,
                                             OrganizationIn,
                                             EventTrackingId,
                                             false,
                                             Argument,
                                             I18NString.Create(Languages.en,
                                                               Description));

        public static UnlinkOrganizationsResult ArgumentError(Organization                        OrganizationOut,
                                                              Organization2OrganizationEdgeLabel  EdgeLabel,
                                                              Organization                        OrganizationIn,
                                                              EventTracking_Id                    EventTrackingId,
                                                              String                              Argument,
                                                              I18NString                          Description)

            => new UnlinkOrganizationsResult(OrganizationOut,
                                             EdgeLabel,
                                             OrganizationIn,
                                             EventTrackingId,
                                             false,
                                             Argument,
                                             Description);


        public static UnlinkOrganizationsResult Failed(Organization                        OrganizationOut,
                                                       Organization2OrganizationEdgeLabel  EdgeLabel,
                                                       Organization                        OrganizationIn,
                                                       EventTracking_Id                    EventTrackingId,
                                                       String                              Description)

            => new UnlinkOrganizationsResult(OrganizationOut,
                                             EdgeLabel,
                                             OrganizationIn,
                                             EventTrackingId,
                                             false,
                                             null,
                                             I18NString.Create(Languages.en,
                                                               Description));

        public static UnlinkOrganizationsResult Failed(Organization                        OrganizationOut,
                                                       Organization2OrganizationEdgeLabel  EdgeLabel,
                                                       Organization                        OrganizationIn,
                                                       EventTracking_Id                    EventTrackingId,
                                                       I18NString                          Description)

            => new UnlinkOrganizationsResult(OrganizationOut,
                                             EdgeLabel,
                                             OrganizationIn,
                                             EventTrackingId,
                                             false,
                                             null,
                                             Description);

        public static UnlinkOrganizationsResult Failed(Organization                        OrganizationOut,
                                                       Organization2OrganizationEdgeLabel  EdgeLabel,
                                                       Organization                        OrganizationIn,
                                                       EventTracking_Id                    EventTrackingId,
                                                       Exception                           Exception)

            => new UnlinkOrganizationsResult(OrganizationOut,
                                             EdgeLabel,
                                             OrganizationIn,
                                             EventTrackingId,
                                             false,
                                             null,
                                             I18NString.Create(Languages.en,
                                                               Exception.Message));

    }

}
