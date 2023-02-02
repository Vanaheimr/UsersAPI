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

    public class UpdateUserGroupResult : AResult<UserGroup>
    {

        public UserGroup UserGroup
            => Object;


        public UpdateUserGroupResult(UserGroup         UserGroup,
                                     EventTracking_Id  EventTrackingId,
                                     Boolean           IsSuccess,
                                     String            Argument          = null,
                                     I18NString        ErrorDescription  = null)

            : base(UserGroup,
                   EventTrackingId,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        { }


        public static UpdateUserGroupResult Success(UserGroup         UserGroup,
                                                    EventTracking_Id  EventTrackingId)

            => new UpdateUserGroupResult(UserGroup,
                                         EventTrackingId,
                                         true,
                                         null,
                                         null);


        public static UpdateUserGroupResult ArgumentError(UserGroup         UserGroup,
                                                          EventTracking_Id  EventTrackingId,
                                                          String            Argument,
                                                          String            Description)

            => new UpdateUserGroupResult(UserGroup,
                                         EventTrackingId,
                                         false,
                                         Argument,
                                         I18NString.Create(Languages.en,
                                                           Description));

        public static UpdateUserGroupResult ArgumentError(UserGroup         UserGroup,
                                                          EventTracking_Id  EventTrackingId,
                                                          String            Argument,
                                                          I18NString        Description)

            => new UpdateUserGroupResult(UserGroup,
                                         EventTrackingId,
                                         false,
                                         Argument,
                                         Description);


        public static UpdateUserGroupResult Failed(UserGroup         UserGroup,
                                                   EventTracking_Id  EventTrackingId,
                                                   String            Description)

            => new UpdateUserGroupResult(UserGroup,
                                         EventTrackingId,
                                         false,
                                         null,
                                         I18NString.Create(Languages.en,
                                                           Description));

        public static UpdateUserGroupResult Failed(UserGroup         UserGroup,
                                                   EventTracking_Id  EventTrackingId,
                                                   I18NString        Description)

            => new UpdateUserGroupResult(UserGroup,
                                         EventTrackingId,
                                         false,
                                         null,
                                         Description);

        public static UpdateUserGroupResult Failed(UserGroup         UserGroup,
                                                   EventTracking_Id  EventTrackingId,
                                                   Exception         Exception)

            => new UpdateUserGroupResult(UserGroup,
                                         EventTrackingId,
                                         false,
                                         null,
                                         I18NString.Create(Languages.en,
                                                           Exception.Message));

    }

}
