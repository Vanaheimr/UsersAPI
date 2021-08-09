/*
 * Copyright (c) 2014-2021, Achim 'ahzf' Friedland <achim@graphdefined.org>
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace social.OpenData.UsersAPI
{

    public class UpdateOrganizationResult : AResult<Organization>
    {

        public Organization Organization
            => Object;


        public UpdateOrganizationResult(Organization      Organization,
                                        EventTracking_Id  EventTrackingId,
                                        Boolean           IsSuccess,
                                        String            Argument          = null,
                                        I18NString        ErrorDescription  = null)

            : base(Organization,
                   EventTrackingId,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        { }


        public static UpdateOrganizationResult Success(Organization      Organization,
                                                       EventTracking_Id  EventTrackingId)

            => new UpdateOrganizationResult(Organization,
                                    EventTrackingId,
                                    true,
                                    null,
                                    null);


        public static UpdateOrganizationResult ArgumentError(Organization      Organization,
                                                             EventTracking_Id  EventTrackingId,
                                                             String            Argument,
                                                             String            Description)

            => new UpdateOrganizationResult(Organization,
                                            EventTrackingId,
                                            false,
                                            Argument,
                                            I18NString.Create(Languages.en,
                                                              Description));

        public static UpdateOrganizationResult ArgumentError(Organization      Organization,
                                                             EventTracking_Id  EventTrackingId,
                                                             String            Argument,
                                                             I18NString        Description)

            => new UpdateOrganizationResult(Organization,
                                            EventTrackingId,
                                            false,
                                            Argument,
                                            Description);


        public static UpdateOrganizationResult Failed(Organization      Organization,
                                                      EventTracking_Id  EventTrackingId,
                                                      String            Description)

            => new UpdateOrganizationResult(Organization,
                                            EventTrackingId,
                                            false,
                                            null,
                                            I18NString.Create(Languages.en,
                                                              Description));

        public static UpdateOrganizationResult Failed(Organization      Organization,
                                                      EventTracking_Id  EventTrackingId,
                                                      I18NString        Description)

            => new UpdateOrganizationResult(Organization,
                                            EventTrackingId,
                                            false,
                                            null,
                                            Description);

        public static UpdateOrganizationResult Failed(Organization      Organization,
                                                      EventTracking_Id  EventTrackingId,
                                                      Exception         Exception)

            => new UpdateOrganizationResult(Organization,
                                            EventTrackingId,
                                            false,
                                            null,
                                            I18NString.Create(Languages.en,
                                                              Exception.Message));

    }

}
