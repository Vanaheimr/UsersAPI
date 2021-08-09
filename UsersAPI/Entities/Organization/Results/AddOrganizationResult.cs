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

    public class AddOrganizationResult : AResult<Organization>
    {

        public Organization  Organization
            => Object;

        public Organization  ParentOrganization   { get; internal set; }


        public AddOrganizationResult(Organization  Organization,
                                     Boolean       IsSuccess,
                                     String        Argument             = null,
                                     I18NString    ErrorDescription     = null,
                                     Organization  ParentOrganization   = null)

            : base(Organization,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        {

            this.ParentOrganization = ParentOrganization;

        }


        public static AddOrganizationResult Success(Organization  Organization,
                                                    Organization  ParentOrganization  = null)

            => new AddOrganizationResult(Organization,
                                         true,
                                         null,
                                         null,
                                         ParentOrganization);


        public static AddOrganizationResult ArgumentError(Organization  Organization,
                                                          String        Argument,
                                                          String        Description)

            => new AddOrganizationResult(Organization,
                                         false,
                                         Argument,
                                         I18NString.Create(Languages.en,
                                                           Description));

        public static AddOrganizationResult ArgumentError(Organization  Organization,
                                                          String        Argument,
                                                          I18NString    Description)

            => new AddOrganizationResult(Organization,
                                         false,
                                         Argument,
                                         Description);


        public static AddOrganizationResult Failed(Organization  Organization,
                                                   String        Description,
                                                   Organization  ParentOrganization = null)

            => new AddOrganizationResult(Organization,
                                         false,
                                         null,
                                         I18NString.Create(Languages.en,
                                                           Description),
                                         ParentOrganization);

        public static AddOrganizationResult Failed(Organization  Organization,
                                                   I18NString    Description,
                                                   Organization  ParentOrganization = null)

            => new AddOrganizationResult(Organization,
                                         false,
                                         null,
                                         Description,
                                         ParentOrganization);

        public static AddOrganizationResult Failed(Organization  Organization,
                                                   Exception     Exception,
                                                   Organization  ParentOrganization = null)

            => new AddOrganizationResult(Organization,
                                         false,
                                         null,
                                         I18NString.Create(Languages.en,
                                                           Exception.Message),
                                         ParentOrganization);

    }

}
