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
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace social.OpenData.UsersAPI.Notifications
{

    public class NotificationGroup
    {

        #region Properties

        public I18NString                                   Title            { get; }

        public I18NString                                   Description      { get; }

        public NotificationVisibility                       Visibility       { get; }

        public IEnumerable<NotificationMessageDescription>  Notifications    { get; }

        #endregion

        #region Constructor(s)

        public NotificationGroup(I18NString                                   Title,
                                 I18NString                                   Description,
                                 NotificationVisibility                       Visibility,
                                 IEnumerable<NotificationMessageDescription>  Notifications)
        {

            #region Initial checks

            if (Title.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Title),          "The given multi-language headline string must not be null or empty!");

            if (Description == null)
                throw new ArgumentNullException(nameof(Description),    "The given multi-language description string must not be null or empty!");

            if (Notifications.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Notifications),  "The given enumeration of notifications must not be null or empty!");

            #endregion

            this.Title          = Title;
            this.Description    = Description;
            this.Visibility     = Visibility;
            this.Notifications  = Notifications;

        }

        #endregion


        #region ToJSON()

        public JObject ToJSON()

            => JSONObject.Create(

                         new JProperty("title",          Title.ToJSON()),

                   Description.IsNeitherNullNorEmpty()
                       ? new JProperty("description",    Description.ToJSON())
                       : null,

                         new JProperty("visibility",     Visibility.ToString().ToLower()),

                   Notifications.SafeAny()
                       ? new JProperty("notifications",  new JArray(Notifications.Select(info => info.ToJSON())))
                       : null

               );

        #endregion

    }

}
