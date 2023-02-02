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

    public class AddUserResult : AResult<User>
    {

        public User User
            => Object;

        public Organization  Organization   { get; internal set; }


        public AddUserResult(User              User,
                             EventTracking_Id  EventTrackingId,
                             Boolean           IsSuccess,
                             String            Argument           = null,
                             I18NString        ErrorDescription   = null,
                             Organization      Organization       = null)

            : base(User,
                   EventTrackingId,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        {

            this.Organization = Organization;

        }


        public static AddUserResult Success(User              User,
                                            EventTracking_Id  EventTrackingId,
                                            Organization      Organization = null)

            => new AddUserResult(User,
                                 EventTrackingId,
                                 true,
                                 null,
                                 null,
                                 Organization);


        public static AddUserResult ArgumentError(User              User,
                                                  EventTracking_Id  EventTrackingId,
                                                  String            Argument,
                                                  String            Description)

            => new AddUserResult(User,
                                 EventTrackingId,
                                 false,
                                 Argument,
                                 I18NString.Create(Languages.en,
                                                   Description));

        public static AddUserResult ArgumentError(User              User,
                                                  EventTracking_Id  EventTrackingId,
                                                  String            Argument,
                                                  I18NString        Description)

            => new AddUserResult(User,
                                 EventTrackingId,
                                 false,
                                 Argument,
                                 Description);


        public static AddUserResult Failed(User              User,
                                           EventTracking_Id  EventTrackingId,
                                           String            Description,
                                           Organization      Organization  = null)

            => new AddUserResult(User,
                                 EventTrackingId,
                                 false,
                                 null,
                                 I18NString.Create(Languages.en,
                                                   Description),
                                 Organization);

        public static AddUserResult Failed(User              User,
                                           EventTracking_Id  EventTrackingId,
                                           I18NString        Description,
                                           Organization      Organization  = null)

            => new AddUserResult(User,
                                 EventTrackingId,
                                 false,
                                 null,
                                 Description,
                                 Organization);

        public static AddUserResult Failed(User              User,
                                           EventTracking_Id  EventTrackingId,
                                           Exception         Exception,
                                           Organization      Organization  = null)

            => new AddUserResult(User,
                                 EventTrackingId,
                                 false,
                                 null,
                                 I18NString.Create(Languages.en,
                                                   Exception.Message),
                                 Organization);

    }

}
