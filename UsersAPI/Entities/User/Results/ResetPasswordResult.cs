/*
 * Copyright (c) 2014-2022, Achim Friedland <achim.friedland@graphdefined.com>
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

    public class ResetPasswordResult : AResult<IEnumerable<User>>
    {

        public User User
            => Object.FirstOrDefault();

        public IEnumerable<User> Users
            => Object;

        public PasswordReset  PasswordReset    { get; internal set; }


        public ResetPasswordResult(User              User,
                                   EventTracking_Id  EventTrackingId,
                                   Boolean           IsSuccess,
                                   String            Argument           = null,
                                   I18NString        ErrorDescription   = null,
                                   PasswordReset     PasswordReset      = null)

            : base(new User[] { User },
                   EventTrackingId,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        {

            this.PasswordReset = PasswordReset;

        }

        public ResetPasswordResult(IEnumerable<User>  Users,
                                   EventTracking_Id   EventTrackingId,
                                   Boolean            IsSuccess,
                                   String             Argument           = null,
                                   I18NString         ErrorDescription   = null,
                                   PasswordReset      PasswordReset      = null)

            : base(Users,
                   EventTrackingId,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        {

            this.PasswordReset = PasswordReset;

        }


        public static ResetPasswordResult Success(User              User,
                                                  EventTracking_Id  EventTrackingId,
                                                  PasswordReset     PasswordReset  = null)

            => new ResetPasswordResult(User,
                                       EventTrackingId,
                                       true,
                                       null,
                                       null,
                                       PasswordReset);

        public static ResetPasswordResult Success(IEnumerable<User>  Users,
                                                  EventTracking_Id   EventTrackingId,
                                                  PasswordReset      PasswordReset  = null)

            => new ResetPasswordResult(Users,
                                       EventTrackingId,
                                       true,
                                       null,
                                       null,
                                       PasswordReset);


        public static ResetPasswordResult ArgumentError(User              User,
                                                        EventTracking_Id  EventTrackingId,
                                                        String            Argument,
                                                        String            Description)

            => new ResetPasswordResult(User,
                                       EventTrackingId,
                                       false,
                                       Argument,
                                       I18NString.Create(Languages.en,
                                                         Description));

        public static ResetPasswordResult ArgumentError(IEnumerable<User>  Users,
                                                        EventTracking_Id   EventTrackingId,
                                                        String             Argument,
                                                        String             Description)

            => new ResetPasswordResult(Users,
                                       EventTrackingId,
                                       false,
                                       Argument,
                                       I18NString.Create(Languages.en,
                                                         Description));

        public static ResetPasswordResult ArgumentError(User              User,
                                                        EventTracking_Id  EventTrackingId,
                                                        String            Argument,
                                                        I18NString        Description)

            => new ResetPasswordResult(User,
                                       EventTrackingId,
                                       false,
                                       Argument,
                                       Description);

        public static ResetPasswordResult ArgumentError(IEnumerable<User>  Users,
                                                        EventTracking_Id   EventTrackingId,
                                                        String             Argument,
                                                        I18NString         Description)

            => new ResetPasswordResult(Users,
                                       EventTrackingId,
                                       false,
                                       Argument,
                                       Description);


        public static ResetPasswordResult Failed(User              User,
                                                 EventTracking_Id  EventTrackingId,
                                                 String            Description,
                                                 PasswordReset     PasswordReset  = null)

            => new ResetPasswordResult(User,
                                       EventTrackingId,
                                       false,
                                       null,
                                       I18NString.Create(Languages.en,
                                                         Description),
                                       PasswordReset);

        public static ResetPasswordResult Failed(IEnumerable<User>  Users,
                                                 EventTracking_Id   EventTrackingId,
                                                 String             Description,
                                                 PasswordReset      PasswordReset  = null)

            => new ResetPasswordResult(Users,
                                       EventTrackingId,
                                       false,
                                       null,
                                       I18NString.Create(Languages.en,
                                                         Description),
                                       PasswordReset);

        public static ResetPasswordResult Failed(User              User,
                                                 EventTracking_Id  EventTrackingId,
                                                 I18NString        Description,
                                                 PasswordReset     PasswordReset  = null)

            => new ResetPasswordResult(User,
                                       EventTrackingId,
                                       false,
                                       null,
                                       Description,
                                       PasswordReset);

        public static ResetPasswordResult Failed(IEnumerable<User>  Users,
                                                 EventTracking_Id   EventTrackingId,
                                                 I18NString         Description,
                                                 PasswordReset      PasswordReset  = null)

            => new ResetPasswordResult(Users,
                                       EventTrackingId,
                                       false,
                                       null,
                                       Description,
                                       PasswordReset);

        public static ResetPasswordResult Failed(User              User,
                                                 EventTracking_Id  EventTrackingId,
                                                 Exception         Exception,
                                                 PasswordReset     PasswordReset  = null)

            => new ResetPasswordResult(User,
                                       EventTrackingId,
                                       false,
                                       null,
                                       I18NString.Create(Languages.en,
                                                         Exception.Message),
                                       PasswordReset);

        public static ResetPasswordResult Failed(IEnumerable<User>  Users,
                                                 EventTracking_Id   EventTrackingId,
                                                 Exception          Exception,
                                                 PasswordReset      PasswordReset  = null)

            => new ResetPasswordResult(Users,
                                       EventTrackingId,
                                       false,
                                       null,
                                       I18NString.Create(Languages.en,
                                                         Exception.Message),
                                       PasswordReset);

    }

}
