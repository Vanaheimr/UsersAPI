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
using System.Linq;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace social.OpenData.UsersAPI
{

    public class ChangePasswordResult : AResult<IEnumerable<User>>
    {

        public User User
            => Object.FirstOrDefault();

        public IEnumerable<User> Users
            => Object;

        public ChangePasswordResult(User              User,
                                    EventTracking_Id  EventTrackingId,
                                    Boolean           IsSuccess,
                                    String            Argument           = null,
                                    I18NString        ErrorDescription   = null)

            : base(new User[] { User },
                   EventTrackingId,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        { }

        public ChangePasswordResult(IEnumerable<User>  Users,
                                    EventTracking_Id   EventTrackingId,
                                    Boolean            IsSuccess,
                                    String             Argument           = null,
                                    I18NString         ErrorDescription   = null)

            : base(Users,
                   EventTrackingId,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        { }


        public static ChangePasswordResult Success(User              User,
                                                   EventTracking_Id  EventTrackingId)

            => new ChangePasswordResult(User,
                                        EventTrackingId,
                                        true,
                                        null,
                                        null);
        public static ChangePasswordResult Success(IEnumerable<User>  Users,
                                                   EventTracking_Id   EventTrackingId)

            => new ChangePasswordResult(Users,
                                        EventTrackingId,
                                        true,
                                        null,
                                        null);


        public static ChangePasswordResult ArgumentError(User              User,
                                                         EventTracking_Id  EventTrackingId,
                                                         String            Argument,
                                                         String            Description)

            => new ChangePasswordResult(User,
                                        EventTrackingId,
                                        false,
                                        Argument,
                                        I18NString.Create(Languages.en,
                                                          Description));

        public static ChangePasswordResult ArgumentError(IEnumerable<User>  Users,
                                                         EventTracking_Id   EventTrackingId,
                                                         String             Argument,
                                                         String             Description)

            => new ChangePasswordResult(Users,
                                        EventTrackingId,
                                        false,
                                        Argument,
                                        I18NString.Create(Languages.en,
                                                          Description));

        public static ChangePasswordResult ArgumentError(User              User,
                                                         EventTracking_Id  EventTrackingId,
                                                         String            Argument,
                                                         I18NString        Description)

            => new ChangePasswordResult(User,
                                        EventTrackingId,
                                        false,
                                        Argument,
                                        Description);

        public static ChangePasswordResult ArgumentError(IEnumerable<User>  Users,
                                                         EventTracking_Id   EventTrackingId,
                                                         String             Argument,
                                                         I18NString         Description)

            => new ChangePasswordResult(Users,
                                        EventTrackingId,
                                        false,
                                        Argument,
                                        Description);


        public static ChangePasswordResult Failed(User              User,
                                                  EventTracking_Id  EventTrackingId,
                                                  String            Description)

            => new ChangePasswordResult(User,
                                        EventTrackingId,
                                        false,
                                        null,
                                        I18NString.Create(Languages.en,
                                                          Description));

        public static ChangePasswordResult Failed(IEnumerable<User>  Users,
                                                  EventTracking_Id   EventTrackingId,
                                                  String             Description)

            => new ChangePasswordResult(Users,
                                        EventTrackingId,
                                        false,
                                        null,
                                        I18NString.Create(Languages.en,
                                                          Description));

        public static ChangePasswordResult Failed(User              User,
                                                  EventTracking_Id  EventTrackingId,
                                                  I18NString        Description)

            => new ChangePasswordResult(User,
                                        EventTrackingId,
                                        false,
                                        null,
                                        Description);

        public static ChangePasswordResult Failed(IEnumerable<User>  Users,
                                                  EventTracking_Id   EventTrackingId,
                                                  I18NString         Description)

            => new ChangePasswordResult(Users,
                                        EventTrackingId,
                                        false,
                                        null,
                                        Description);

        public static ChangePasswordResult Failed(User              User,
                                                  EventTracking_Id  EventTrackingId,
                                                  Exception         Exception)

            => new ChangePasswordResult(User,
                                        EventTrackingId,
                                        false,
                                        null,
                                        I18NString.Create(Languages.en,
                                                          Exception.Message));

        public static ChangePasswordResult Failed(IEnumerable<User>  Users,
                                                  EventTracking_Id   EventTrackingId,
                                                  Exception          Exception)

            => new ChangePasswordResult(Users,
                                        EventTrackingId,
                                        false,
                                        null,
                                        I18NString.Create(Languages.en,
                                                          Exception.Message));

    }

}
