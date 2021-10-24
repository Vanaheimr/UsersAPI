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

    public class RemoveUserFromUserGroupResult : AResult<User, UserGroup>
    {

        public User                      User
            => Object1;

        public User2UserGroupEdgeLabel?  EdgeLabel    { get; }

        public UserGroup                 UserGroup
            => Object2;


        public RemoveUserFromUserGroupResult(User                      User,
                                             User2UserGroupEdgeLabel?  EdgeLabel,
                                             UserGroup                 UserGroup,
                                             EventTracking_Id          EventTrackingId,
                                             Boolean                   IsSuccess,
                                             String                    Argument           = null,
                                             I18NString                ErrorDescription   = null)

            : base(User,
                   UserGroup,
                   EventTrackingId,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        {

            this.EdgeLabel = EdgeLabel;

        }


        public static RemoveUserFromUserGroupResult Success(User              User,
                                                            UserGroup         UserGroup,
                                                            EventTracking_Id  EventTrackingId)

            => new RemoveUserFromUserGroupResult(User,
                                                 null,
                                                 UserGroup,
                                                 EventTrackingId,
                                                 true);

        public static RemoveUserFromUserGroupResult Success(User                     User,
                                                            User2UserGroupEdgeLabel  EdgeLabel,
                                                            UserGroup                UserGroup,
                                                            EventTracking_Id         EventTrackingId)

            => new RemoveUserFromUserGroupResult(User,
                                                 EdgeLabel,
                                                 UserGroup,
                                                 EventTrackingId,
                                                 true);


        public static RemoveUserFromUserGroupResult ArgumentError(User              User,
                                                                  UserGroup         UserGroup,
                                                                  EventTracking_Id  EventTrackingId,
                                                                  String            Argument,
                                                                  String            Description)

            => new RemoveUserFromUserGroupResult(User,
                                                 null,
                                                 UserGroup,
                                                 EventTrackingId,
                                                 false,
                                                 Argument,
                                                 I18NString.Create(Languages.en,
                                                                   Description));

        public static RemoveUserFromUserGroupResult ArgumentError(User                     User,
                                                                  User2UserGroupEdgeLabel  EdgeLabel,
                                                                  UserGroup                UserGroup,
                                                                  EventTracking_Id         EventTrackingId,
                                                                  String                   Argument,
                                                                  String                   Description)

            => new RemoveUserFromUserGroupResult(User,
                                                 EdgeLabel,
                                                 UserGroup,
                                                 EventTrackingId,
                                                 false,
                                                 Argument,
                                                 I18NString.Create(Languages.en,
                                                                   Description));

        public static RemoveUserFromUserGroupResult ArgumentError(User              User,
                                                                  UserGroup         UserGroup,
                                                                  EventTracking_Id  EventTrackingId,
                                                                  String            Argument,
                                                                  I18NString        Description)

            => new RemoveUserFromUserGroupResult(User,
                                                 null,
                                                 UserGroup,
                                                 EventTrackingId,
                                                 false,
                                                 Argument,
                                                 Description);

        public static RemoveUserFromUserGroupResult ArgumentError(User                     User,
                                                                  User2UserGroupEdgeLabel  EdgeLabel,
                                                                  UserGroup                UserGroup,
                                                                  EventTracking_Id         EventTrackingId,
                                                                  String                   Argument,
                                                                  I18NString               Description)

            => new RemoveUserFromUserGroupResult(User,
                                                 EdgeLabel,
                                                 UserGroup,
                                                 EventTrackingId,
                                                 false,
                                                 Argument,
                                                 Description);


        public static RemoveUserFromUserGroupResult Failed(User              User,
                                                           UserGroup         UserGroup,
                                                           EventTracking_Id  EventTrackingId,
                                                           String            Description)

            => new RemoveUserFromUserGroupResult(User,
                                                 null,
                                                 UserGroup,
                                                 EventTrackingId,
                                                 false,
                                                 null,
                                                 I18NString.Create(Languages.en,
                                                                   Description));

        public static RemoveUserFromUserGroupResult Failed(User                     User,
                                                           User2UserGroupEdgeLabel  EdgeLabel,
                                                           UserGroup                UserGroup,
                                                           EventTracking_Id         EventTrackingId,
                                                           String                   Description)

            => new RemoveUserFromUserGroupResult(User,
                                                 EdgeLabel,
                                                 UserGroup,
                                                 EventTrackingId,
                                                 false,
                                                 null,
                                                 I18NString.Create(Languages.en,
                                                                   Description));

        public static RemoveUserFromUserGroupResult Failed(User              User,
                                                           UserGroup         UserGroup,
                                                           EventTracking_Id  EventTrackingId,
                                                           I18NString        Description)

            => new RemoveUserFromUserGroupResult(User,
                                                 null,
                                                 UserGroup,
                                                 EventTrackingId,
                                                 false,
                                                 null,
                                                 Description);

        public static RemoveUserFromUserGroupResult Failed(User                     User,
                                                           User2UserGroupEdgeLabel  EdgeLabel,
                                                           UserGroup                UserGroup,
                                                           EventTracking_Id         EventTrackingId,
                                                           I18NString               Description)

            => new RemoveUserFromUserGroupResult(User,
                                                 EdgeLabel,
                                                 UserGroup,
                                                 EventTrackingId,
                                                 false,
                                                 null,
                                                 Description);

        public static RemoveUserFromUserGroupResult Failed(User              User,
                                                           UserGroup         UserGroup,
                                                           EventTracking_Id  EventTrackingId,
                                                           Exception         Exception)

            => new RemoveUserFromUserGroupResult(User,
                                                 null,
                                                 UserGroup,
                                                 EventTrackingId,
                                                 false,
                                                 null,
                                                 I18NString.Create(Languages.en,
                                                                   Exception.Message));

        public static RemoveUserFromUserGroupResult Failed(User                     User,
                                                           User2UserGroupEdgeLabel  EdgeLabel,
                                                           UserGroup                UserGroup,
                                                           EventTracking_Id         EventTrackingId,
                                                           Exception                Exception)

            => new RemoveUserFromUserGroupResult(User,
                                                 EdgeLabel,
                                                 UserGroup,
                                                 EventTrackingId,
                                                 false,
                                                 null,
                                                 I18NString.Create(Languages.en,
                                                                   Exception.Message));

    }

}
