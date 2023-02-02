/*
 * Copyright (c) 2014-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

    public class AddOrganizationIfNotExistsResult : AResult<Organization>
    {

        public Organization  Organization
            => Object;

        public Organization     ParentOrganization   { get; internal set; }

        public AddedOrIgnored?  AddedOrIgnored       { get; internal set; }


        public AddOrganizationIfNotExistsResult(Organization      Organization,
                                                EventTracking_Id  EventTrackingId,
                                                Boolean           IsSuccess,
                                                String            Argument             = null,
                                                I18NString        ErrorDescription     = null,
                                                Organization      ParentOrganization   = null,
                                                AddedOrIgnored?   AddedOrIgnored       = null)

            : base(Organization,
                   EventTrackingId,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        {

            this.ParentOrganization  = ParentOrganization;
            this.AddedOrIgnored      = AddedOrIgnored;

        }


        public static AddOrganizationIfNotExistsResult Success(Organization      Organization,
                                                               AddedOrIgnored    AddedOrIgnored,
                                                               EventTracking_Id  EventTrackingId,
                                                               Organization      ParentOrganization  = null)

            => new AddOrganizationIfNotExistsResult(Organization,
                                                    EventTrackingId,
                                                    true,
                                                    null,
                                                    null,
                                                    ParentOrganization,
                                                    AddedOrIgnored);


        public static AddOrganizationIfNotExistsResult ArgumentError(Organization      Organization,
                                                                     EventTracking_Id  EventTrackingId,
                                                                     String            Argument,
                                                                     String            Description)

            => new AddOrganizationIfNotExistsResult(Organization,
                                                    EventTrackingId,
                                                    false,
                                                    Argument,
                                                    I18NString.Create(Languages.en,
                                                                      Description));

        public static AddOrganizationIfNotExistsResult ArgumentError(Organization      Organization,
                                                                     EventTracking_Id  EventTrackingId,
                                                                     String            Argument,
                                                                     I18NString        Description)

            => new AddOrganizationIfNotExistsResult(Organization,
                                                    EventTrackingId,
                                                    false,
                                                    Argument,
                                                    Description);


        public static AddOrganizationIfNotExistsResult Failed(Organization      Organization,
                                                              EventTracking_Id  EventTrackingId,
                                                              String            Description,
                                                              Organization      ParentOrganization  = null)

            => new AddOrganizationIfNotExistsResult(Organization,
                                                    EventTrackingId,
                                                    false,
                                                    null,
                                                    I18NString.Create(Languages.en,
                                                                      Description),
                                                    ParentOrganization);

        public static AddOrganizationIfNotExistsResult Failed(Organization      Organization,
                                                              EventTracking_Id  EventTrackingId,
                                                              I18NString        Description,
                                                              Organization      ParentOrganization  = null)

            => new AddOrganizationIfNotExistsResult(Organization,
                                                    EventTrackingId,
                                                    false,
                                                    null,
                                                    Description,
                                                    ParentOrganization);

        public static AddOrganizationIfNotExistsResult Failed(Organization      Organization,
                                                              EventTracking_Id  EventTrackingId,
                                                              Exception         Exception,
                                                              Organization      ParentOrganization  = null)

            => new AddOrganizationIfNotExistsResult(Organization,
                                                    EventTrackingId,
                                                    false,
                                                    null,
                                                    I18NString.Create(Languages.en,
                                                                      Exception.Message),
                                                    ParentOrganization);

    }

}
