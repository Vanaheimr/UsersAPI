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

    public class RemoveUserGroupResult : AResult<UserGroup>
    {

        private RemoveUserGroupResult(UserGroup   UserGroup,
                                      Boolean     IsSuccess,
                                      String      Argument           = null,
                                      I18NString  ErrorDescription   = null)

            : base(UserGroup,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        { }


        public static RemoveUserGroupResult Success(UserGroup UserGroup)

            => new RemoveUserGroupResult(UserGroup,
                                         true);


        public static RemoveUserGroupResult ArgumentError(UserGroup  UserGroup,
                                                          String     Argument,
                                                          String     Description)

            => new RemoveUserGroupResult(UserGroup,
                                         false,
                                         Argument,
                                         I18NString.Create(Languages.en,
                                                           Description));

        public static RemoveUserGroupResult ArgumentError(UserGroup   UserGroup,
                                                          String      Argument,
                                                          I18NString  Description)

            => new RemoveUserGroupResult(UserGroup,
                                         false,
                                         Argument,
                                         Description);


        public static RemoveUserGroupResult Failed(UserGroup  UserGroup,
                                                   String     Description)

            => new RemoveUserGroupResult(UserGroup,
                                         false,
                                         null,
                                         I18NString.Create(Languages.en,
                                                           Description));

        public static RemoveUserGroupResult Failed(UserGroup   UserGroup,
                                                   I18NString  Description)

            => new RemoveUserGroupResult(UserGroup,
                                         false,
                                         null,
                                         Description);

        public static RemoveUserGroupResult Failed(UserGroup  UserGroup,
                                                   Exception  Exception)

            => new RemoveUserGroupResult(UserGroup,
                                         false,
                                         null,
                                         I18NString.Create(Languages.en,
                                                           Exception.Message));

    }

}
