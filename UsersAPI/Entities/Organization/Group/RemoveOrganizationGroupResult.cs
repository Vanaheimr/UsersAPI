/*
 * Copyright (c) 2014-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of UsersAPI <https://www.github.com/Vanaheimr/UsersAPI>
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

    public class RemoveOrganizationGroupResult : AResult<OrganizationGroup>
    {

        private RemoveOrganizationGroupResult(OrganizationGroup  OrganizationGroup,
                                              EventTracking_Id   EventTrackingId,
                                              Boolean            IsSuccess,
                                              String             Argument           = null,
                                              I18NString         ErrorDescription   = null)

            : base(OrganizationGroup,
                   EventTrackingId,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        { }


        public static RemoveOrganizationGroupResult Success(OrganizationGroup  OrganizationGroup,
                                                            EventTracking_Id   EventTrackingId)

            => new RemoveOrganizationGroupResult(OrganizationGroup,
                                                 EventTrackingId,
                                                 true);


        public static RemoveOrganizationGroupResult ArgumentError(OrganizationGroup  OrganizationGroup,
                                                                  EventTracking_Id   EventTrackingId,
                                                                  String             Argument,
                                                                  String             Description)

            => new RemoveOrganizationGroupResult(OrganizationGroup,
                                                 EventTrackingId,
                                                 false,
                                                 Argument,
                                                 I18NString.Create(Languages.en,
                                                                   Description));

        public static RemoveOrganizationGroupResult ArgumentError(OrganizationGroup  OrganizationGroup,
                                                                  EventTracking_Id   EventTrackingId,
                                                                  String             Argument,
                                                                  I18NString         Description)

            => new RemoveOrganizationGroupResult(OrganizationGroup,
                                                 EventTrackingId,
                                                 false,
                                                 Argument,
                                                 Description);


        public static RemoveOrganizationGroupResult Failed(OrganizationGroup  OrganizationGroup,
                                                           EventTracking_Id   EventTrackingId,
                                                           String             Description)

            => new RemoveOrganizationGroupResult(OrganizationGroup,
                                                 EventTrackingId,
                                                 false,
                                                 null,
                                                 I18NString.Create(Languages.en,
                                                                   Description));

        public static RemoveOrganizationGroupResult Failed(OrganizationGroup  OrganizationGroup,
                                                           EventTracking_Id   EventTrackingId,
                                                           I18NString         Description)

            => new RemoveOrganizationGroupResult(OrganizationGroup,
                                                 EventTrackingId,
                                                 false,
                                                 null,
                                                 Description);

        public static RemoveOrganizationGroupResult Failed(OrganizationGroup  OrganizationGroup,
                                                           EventTracking_Id   EventTrackingId,
                                                           Exception          Exception)

            => new RemoveOrganizationGroupResult(OrganizationGroup,
                                                 EventTrackingId,
                                                 false,
                                                 null,
                                                 I18NString.Create(Languages.en,
                                                                   Exception.Message));

    }

}
