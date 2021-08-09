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

    public class RemoveUserResult : AResult<User>
    {

        private RemoveUserResult(User        User,
                                 Boolean     IsSuccess,
                                 String      Argument           = null,
                                 I18NString  ErrorDescription   = null)

            : base(User,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        { }


        public static RemoveUserResult Success(User User)

            => new RemoveUserResult(User,
                                    true);


        public static RemoveUserResult ArgumentError(User    User,
                                                     String  Argument,
                                                     String  Description)

            => new RemoveUserResult(User,
                                    false,
                                    Argument,
                                    I18NString.Create(Languages.en,
                                                      Description));

        public static RemoveUserResult ArgumentError(User        User,
                                                     String      Argument,
                                                     I18NString  Description)

            => new RemoveUserResult(User,
                                    false,
                                    Argument,
                                    Description);


        public static RemoveUserResult Failed(User    User,
                                              String  Description)

            => new RemoveUserResult(User,
                                    false,
                                    null,
                                    I18NString.Create(Languages.en,
                                                      Description));

        public static RemoveUserResult Failed(User        User,
                                              I18NString  Description)

            => new RemoveUserResult(User,
                                    false,
                                    null,
                                    Description);

        public static RemoveUserResult Failed(User       User,
                                              Exception  Exception)

            => new RemoveUserResult(User,
                                    false,
                                    null,
                                    I18NString.Create(Languages.en,
                                                      Exception.Message));

    }

}
