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

    public class RemoveOrganizationResult : AResult<Organization>
    {

        private RemoveOrganizationResult(Organization  Organization,
                                         Boolean       IsSuccess,
                                         String        Argument           = null,
                                         I18NString    ErrorDescription   = null)

            : base(Organization,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        { }


        public static RemoveOrganizationResult Success(Organization Organization)

            => new RemoveOrganizationResult(Organization,
                                            true);


        public static RemoveOrganizationResult ArgumentError(Organization  Organization,
                                                             String        Argument,
                                                             String        Description)

            => new RemoveOrganizationResult(Organization,
                                            false,
                                            Argument,
                                            I18NString.Create(Languages.en,
                                                              Description));

        public static RemoveOrganizationResult ArgumentError(Organization  Organization,
                                                             String        Argument,
                                                             I18NString    Description)

            => new RemoveOrganizationResult(Organization,
                                            false,
                                            Argument,
                                            Description);


        public static RemoveOrganizationResult Failed(Organization  Organization,
                                                      String        Description)

            => new RemoveOrganizationResult(Organization,
                                            false,
                                            null,
                                            I18NString.Create(Languages.en,
                                                              Description));

        public static RemoveOrganizationResult Failed(Organization  Organization,
                                                      I18NString    Description)

            => new RemoveOrganizationResult(Organization,
                                            false,
                                            null,
                                            Description);

        public static RemoveOrganizationResult Failed(Organization  Organization,
                                                      Exception     Exception)

            => new RemoveOrganizationResult(Organization,
                                            false,
                                            null,
                                            I18NString.Create(Languages.en,
                                                              Exception.Message));

    }

}
