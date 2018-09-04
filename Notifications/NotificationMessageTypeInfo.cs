/*
 * Copyright (c) 2014-2018, Achim 'ahzf' Friedland <achim@graphdefined.org>
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Aegir;

#endregion

namespace org.GraphDefined.OpenData.Notifications
{

    public class NotificationMessageTypeInfo
    {

        public NotificationMessageType Id            { get; }
        public String                  Text          { get; }
        public I18NString              Description   { get; }

        public NotificationMessageTypeInfo(NotificationMessageType  Id,
                                           String                   Text,
                                           I18NString               Description)
        {

            this.Id           = Id;
            this.Text         = Text;
            this.Description  = Description;

        }

        public JObject ToJSON()

            => JSONObject.Create(
                   new JProperty("@id",          Id.ToString()),
                   new JProperty("text",         Text),
                   new JProperty("description",  Description.ToJSON())
               );

    }

}
