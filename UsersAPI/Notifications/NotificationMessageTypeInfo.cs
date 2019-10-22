/*
 * Copyright (c) 2014-2019, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Aegir;

#endregion

namespace social.OpenData.UsersAPI.Notifications
{

    public enum NotificationVisibility
    {
        System,
        Admins,
        Customers,
        Guests
    }


    public class NotificationMessageGroup
    {

        private List<NotificationMessageTypeInfo> _Notifications;


        public NotificationMessageGroupId                Id               { get; }

        public String                                    Text             { get; }

        public NotificationVisibility                    Visibility       { get; }

        public I18NString                                Description      { get; }

        public IEnumerable<NotificationMessageTypeInfo>  Notifications
            => _Notifications;

        public NotificationMessageGroup(NotificationMessageGroupId                Id,
                                        String                                    Text,
                                        NotificationVisibility                    Visibility,
                                        I18NString                                Description,
                                        IEnumerable<NotificationMessageTypeInfo>  Notifications = null)
        {

            this.Id              = Id;
            this.Text            = Text;
            this.Visibility      = Visibility;
            this.Description     = Description;
            this._Notifications  = new List<NotificationMessageTypeInfo>();

            if (Notifications.SafeAny())
                Notifications.ForEach(notification => _Notifications.Add(notification));

        }


        public NotificationMessageTypeInfo Add(NotificationMessageTypeInfo NotificationMessageTypeInfo)
        {

            if (NotificationMessageTypeInfo == null)
                throw new ArgumentNullException(nameof(NotificationMessageTypeInfo), "The given NotificationMessageTypeInfo must not be null!");

            return _Notifications.AddAndReturnElement(NotificationMessageTypeInfo);

        }


        public JObject ToJSON()

            => JSONObject.Create(

                   new JProperty("@id",          Id.ToString()),
                   new JProperty("text",         Text),
                   new JProperty("visibility",   Visibility.ToString().ToLower()),
                   new JProperty("description",  Description.ToJSON()),

                   Notifications.SafeAny()
                       ? new JProperty("notifications",  new JArray(Notifications.Select(info => info.ToJSON())))
                       : null

               );

    }


    public class NotificationMessageTypeInfo
    {

        public NotificationMessageType  Id             { get; }

        public String                   Text           { get; }

        public NotificationVisibility   Visibility     { get; }

        public I18NString               Description    { get; }

        public NotificationMessageTypeInfo(NotificationMessageType  Id,
                                           String                   Text,
                                           NotificationVisibility   Visibility,
                                           I18NString               Description)
        {

            this.Id           = Id;
            this.Text         = Text;
            this.Visibility   = Visibility;
            this.Description  = Description;

        }

        public JObject ToJSON()

            => JSONObject.Create(
                   new JProperty("@id",          Id.ToString()),
                   new JProperty("text",         Text),
                   new JProperty("visibility",   Visibility.ToString().ToLower()),
                   new JProperty("description",  Description.ToJSON())
               );

    }

}
