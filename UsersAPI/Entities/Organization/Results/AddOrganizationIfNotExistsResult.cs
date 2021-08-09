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

    public class AddOrganizationIfNotExistsResult : AResult<Organization>
    {

        public Organization  Organization
            => Object;

        public Organization  ParentOrganization   { get; internal set; }


        public AddOrganizationIfNotExistsResult(Organization  Organization,
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


        public static AddOrganizationIfNotExistsResult Success(Organization  Organization,
                                                               Organization  ParentOrganization  = null)

            => new AddOrganizationIfNotExistsResult(Organization,
                                                    true,
                                                    null,
                                                    null,
                                                    ParentOrganization);


        public static AddOrganizationIfNotExistsResult ArgumentError(Organization  Organization,
                                                                     String        Argument,
                                                                     String        Description)

            => new AddOrganizationIfNotExistsResult(Organization,
                                                    false,
                                                    Argument,
                                                    I18NString.Create(Languages.en,
                                                                      Description));

        public static AddOrganizationIfNotExistsResult ArgumentError(Organization  Organization,
                                                                     String        Argument,
                                                                     I18NString    Description)

            => new AddOrganizationIfNotExistsResult(Organization,
                                                    false,
                                                    Argument,
                                                    Description);


        public static AddOrganizationIfNotExistsResult Failed(Organization  Organization,
                                                              String        Description,
                                                              Organization  ParentOrganization  = null)

            => new AddOrganizationIfNotExistsResult(Organization,
                                                    false,
                                                    null,
                                                    I18NString.Create(Languages.en,
                                                                      Description),
                                                    ParentOrganization);

        public static AddOrganizationIfNotExistsResult Failed(Organization  Organization,
                                                              I18NString    Description,
                                                              Organization  ParentOrganization  = null)

            => new AddOrganizationIfNotExistsResult(Organization,
                                                    false,
                                                    null,
                                                    Description,
                                                    ParentOrganization);

        public static AddOrganizationIfNotExistsResult Failed(Organization  Organization,
                                                              Exception     Exception,
                                                              Organization  ParentOrganization  = null)

            => new AddOrganizationIfNotExistsResult(Organization,
                                                    false,
                                                    null,
                                                    I18NString.Create(Languages.en,
                                                                      Exception.Message),
                                                    ParentOrganization);

    }

}
