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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace social.OpenData.UsersAPI
{

    public class AddAPIKeyIfNotExistsResult : AResult<APIKey>
    {

        public APIKey APIKey
            => Object;

        public Organization     Organization      { get; internal set; }

        public AddedOrIgnored?  AddedOrIgnored    { get; internal set; }


        public AddAPIKeyIfNotExistsResult(APIKey            APIKey,
                                          EventTracking_Id  EventTrackingId,
                                          Boolean           IsSuccess,
                                          String            Argument           = null,
                                          I18NString        ErrorDescription   = null,
                                          Organization      Organization       = null,
                                          AddedOrIgnored?   AddedOrIgnored     = null)

            : base(APIKey,
                   EventTrackingId,
                   IsSuccess,
                   Argument,
                   ErrorDescription)

        {

            this.Organization    = Organization;
            this.AddedOrIgnored  = AddedOrIgnored;

        }


        public static AddAPIKeyIfNotExistsResult Success(APIKey            APIKey,
                                                         AddedOrIgnored    AddedOrIgnored,
                                                         EventTracking_Id  EventTrackingId,
                                                         Organization      Organization = null)

            => new AddAPIKeyIfNotExistsResult(APIKey,
                                              EventTrackingId,
                                              true,
                                              null,
                                              null,
                                              Organization,
                                              AddedOrIgnored);


        public static AddAPIKeyIfNotExistsResult ArgumentError(APIKey            APIKey,
                                                               EventTracking_Id  EventTrackingId,
                                                               String            Argument,
                                                               String            Description)

            => new AddAPIKeyIfNotExistsResult(APIKey,
                                              EventTrackingId,
                                              false,
                                              Argument,
                                              I18NString.Create(Languages.en,
                                                                Description));

        public static AddAPIKeyIfNotExistsResult ArgumentError(APIKey            APIKey,
                                                               EventTracking_Id  EventTrackingId,
                                                               String            Argument,
                                                               I18NString        Description)

            => new AddAPIKeyIfNotExistsResult(APIKey,
                                              EventTrackingId,
                                              false,
                                              Argument,
                                              Description);


        public static AddAPIKeyIfNotExistsResult Failed(APIKey            APIKey,
                                                        EventTracking_Id  EventTrackingId,
                                                        String            Description,
                                                        Organization      Organization = null)

            => new AddAPIKeyIfNotExistsResult(APIKey,
                                              EventTrackingId,
                                              false,
                                              null,
                                              I18NString.Create(Languages.en,
                                                                Description),
                                              Organization);

        public static AddAPIKeyIfNotExistsResult Failed(APIKey            APIKey,
                                                        EventTracking_Id  EventTrackingId,
                                                        I18NString        Description,
                                                        Organization      Organization = null)

            => new AddAPIKeyIfNotExistsResult(APIKey,
                                              EventTrackingId,
                                              false,
                                              null,
                                              Description,
                                              Organization);

        public static AddAPIKeyIfNotExistsResult Failed(APIKey            APIKey,
                                                        EventTracking_Id  EventTrackingId,
                                                        Exception         Exception,
                                                        Organization      Organization = null)

            => new AddAPIKeyIfNotExistsResult(APIKey,
                                              EventTrackingId,
                                              false,
                                              null,
                                              I18NString.Create(Languages.en,
                                                                Exception.Message),
                                              Organization);

    }

}
