/*
 * Copyright (c) 2014-2021, Achim Friedland <achim.friedland@graphdefined.com>
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

    public class AddUserToOrganizationResult : AResult<User, Organization>
    {

        public User                        User
            => Object1;

        public User2OrganizationEdgeTypes  EdgeLabel       { get; }

        public Organization                Organization
            => Object2;


        public AddUserToOrganizationResult(User                        User,
                                           User2OrganizationEdgeTypes  EdgeLabel,
                                           Organization                Organization,
                                           EventTracking_Id            EventTrackingId,
                                           Boolean                     IsSuccess,
                                           String                      Argument           = null,
                                           I18NString                  ErrorDescription   = null)

            : base(User,
                   Organization,
                   EventTrackingId,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        {

            this.EdgeLabel = EdgeLabel;

        }


        public static AddUserToOrganizationResult Success(User                        User,
                                                          User2OrganizationEdgeTypes  EdgeLabel,
                                                          Organization                Organization,
                                                          EventTracking_Id            EventTrackingId)

            => new AddUserToOrganizationResult(User,
                                               EdgeLabel,
                                               Organization,
                                               EventTrackingId,
                                               true);


        public static AddUserToOrganizationResult ArgumentError(User                        User,
                                                                User2OrganizationEdgeTypes  EdgeLabel,
                                                                Organization                Organization,
                                                                EventTracking_Id            EventTrackingId,
                                                                String                      Argument,
                                                                String                      Description)

            => new AddUserToOrganizationResult(User,
                                               EdgeLabel,
                                               Organization,
                                               EventTrackingId,
                                               false,
                                               Argument,
                                               I18NString.Create(Languages.en,
                                                                 Description));

        public static AddUserToOrganizationResult ArgumentError(User                        User,
                                                                User2OrganizationEdgeTypes  EdgeLabel,
                                                                Organization                Organization,
                                                                EventTracking_Id            EventTrackingId,
                                                                String                      Argument,
                                                                I18NString                  Description)

            => new AddUserToOrganizationResult(User,
                                               EdgeLabel,
                                               Organization,
                                               EventTrackingId,
                                               false,
                                               Argument,
                                               Description);


        public static AddUserToOrganizationResult Failed(User                        User,
                                                         User2OrganizationEdgeTypes  EdgeLabel,
                                                         Organization                Organization,
                                                         EventTracking_Id            EventTrackingId,
                                                         String                      Description)

            => new AddUserToOrganizationResult(User,
                                               EdgeLabel,
                                               Organization,
                                               EventTrackingId,
                                               false,
                                               null,
                                               I18NString.Create(Languages.en,
                                                                 Description));

        public static AddUserToOrganizationResult Failed(User                        User,
                                                         User2OrganizationEdgeTypes  EdgeLabel,
                                                         Organization                Organization,
                                                         EventTracking_Id            EventTrackingId,
                                                         I18NString                  Description)

            => new AddUserToOrganizationResult(User,
                                               EdgeLabel,
                                               Organization,
                                               EventTrackingId,
                                               false,
                                               null,
                                               Description);

        public static AddUserToOrganizationResult Failed(User                        User,
                                                         User2OrganizationEdgeTypes  EdgeLabel,
                                                         Organization                Organization,
                                                         EventTracking_Id            EventTrackingId,
                                                         Exception                   Exception)

            => new AddUserToOrganizationResult(User,
                                               EdgeLabel,
                                               Organization,
                                               EventTrackingId,
                                               false,
                                               null,
                                               I18NString.Create(Languages.en,
                                                                 Exception.Message));

    }

}
