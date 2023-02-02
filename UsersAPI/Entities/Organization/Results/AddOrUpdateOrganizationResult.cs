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

    public class AddOrUpdateOrganizationResult : AResult<Organization>
    {

        public Organization Organization
            => Object;

        public Organization     ParentOrganization    { get; internal set; }

        public AddedOrUpdated?  AddedOrUpdated        { get; internal set; }


        public AddOrUpdateOrganizationResult(Organization      Organization,
                                             EventTracking_Id  EventTrackingId,
                                             Boolean           IsSuccess,
                                             String            Argument             = null,
                                             I18NString        ErrorDescription     = null,
                                             Organization      ParentOrganization   = null,
                                             AddedOrUpdated?   AddedOrUpdated       = null)

            : base(Organization,
                   EventTrackingId,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        {

            this.ParentOrganization  = ParentOrganization;
            this.AddedOrUpdated      = AddedOrUpdated;

        }


        public static AddOrUpdateOrganizationResult Success(Organization      Organization,
                                                            AddedOrUpdated    AddedOrUpdated,
                                                            EventTracking_Id  EventTrackingId,
                                                            Organization      ParentOrganization = null)

            => new AddOrUpdateOrganizationResult(Organization,
                                                 EventTrackingId,
                                                 true,
                                                 null,
                                                 null,
                                                 ParentOrganization,
                                                 AddedOrUpdated);


        public static AddOrUpdateOrganizationResult ArgumentError(Organization      Organization,
                                                                  EventTracking_Id  EventTrackingId,
                                                                  String            Argument,
                                                                  String            Description)

            => new AddOrUpdateOrganizationResult(Organization,
                                                 EventTrackingId,
                                                 false,
                                                 Argument,
                                                 I18NString.Create(Languages.en,
                                                                   Description));

        public static AddOrUpdateOrganizationResult ArgumentError(Organization      Organization,
                                                                  EventTracking_Id  EventTrackingId,
                                                                  String            Argument,
                                                                  I18NString        Description)

            => new AddOrUpdateOrganizationResult(Organization,
                                                 EventTrackingId,
                                                 false,
                                                 Argument,
                                                 Description);


        public static AddOrUpdateOrganizationResult Failed(Organization      Organization,
                                                           EventTracking_Id  EventTrackingId,
                                                           String            Description,
                                                           Organization      ParentOrganization  = null)

            => new AddOrUpdateOrganizationResult(Organization,
                                                 EventTrackingId,
                                                 false,
                                                 null,
                                                 I18NString.Create(Languages.en,
                                                                   Description),
                                                 ParentOrganization);

        public static AddOrUpdateOrganizationResult Failed(Organization      Organization,
                                                           EventTracking_Id  EventTrackingId,
                                                           I18NString        Description,
                                                           Organization      ParentOrganization  = null)

            => new AddOrUpdateOrganizationResult(Organization,
                                                 EventTrackingId,
                                                 false,
                                                 null,
                                                 Description,
                                                 ParentOrganization);

        public static AddOrUpdateOrganizationResult Failed(Organization      Organization,
                                                           EventTracking_Id  EventTrackingId,
                                                           Exception         Exception,
                                                           Organization      ParentOrganization  = null)

            => new AddOrUpdateOrganizationResult(Organization,
                                                 EventTrackingId,
                                                 false,
                                                 null,
                                                 I18NString.Create(Languages.en,
                                                                   Exception.Message),
                                                 ParentOrganization);

    }

}
