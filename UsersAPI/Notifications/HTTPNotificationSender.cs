﻿/*
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
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Security;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using Org.BouncyCastle.Bcpg.OpenPgp;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Styx.Arrows;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;
using org.GraphDefined.Vanaheimr.BouncyCastle;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Aegir;
using System.Collections.Concurrent;

#endregion

namespace social.OpenData.UsersAPI
{

    public class HTTPNotificationSender : ANotificationSender
    {

        #region Data

        public static readonly IPPort DefaultHTTPPort = IPPort.Parse(80);

        public static readonly Regex JSONWhitespaceRegEx = new Regex(@"(\s)+", RegexOptions.IgnorePatternWhitespace);

        #endregion

        #region Properties

        public HTTPHostname  Hostname   { get; }

        public IPPort        HTTPPort   { get; }

        /// <summary>
        /// The current hash value of the API.
        /// </summary>
        public String CurrentDatabaseHashValue { get; private set; }

        #endregion

        #region Constructor(s)

        public HTTPNotificationSender(UsersAPI          UsersAPI,
                                      HTTPHostname      Hostname,
                                      IPPort?           HTTPPort                   = null,
                                      TimeSpan?         SendNotificationsEvery     = null,
                                      Boolean           DisableSendNotifications   = false,
                                      PgpPublicKeyRing  PublicKeyRing              = null,
                                      PgpSecretKeyRing  SecretKeyRing              = null,
                                      DNSClient         DNSClient                  = null)

            : base(UsersAPI,
                   SendNotificationsEvery,
                   DisableSendNotifications,
                   PublicKeyRing,
                   SecretKeyRing,
                   DNSClient)

        {

            this.Hostname  = Hostname;
            this.HTTPPort  = HTTPPort ?? DefaultHTTPPort;

        }

        #endregion


        #region (timer) SendNotifications(State)

        public override async Task SendNotifications(IEnumerable<JObject> JSONData)
        {

            var _JSONData = JSONData.ToArray();

            if (_JSONData.Length > 0)
            {

                var JSON = new JObject(
                               new JProperty("messages",    new JArray(JSONData)),
                               new JProperty("writer",      UsersAPI.SystemId),
                               new JProperty("timestamp",   DateTime.UtcNow.ToIso8601()),
                               new JProperty("nonce",       Guid.NewGuid().ToString().Replace("-", "")),
                               new JProperty("parentHash",  CurrentDatabaseHashValue)
                           );

                var SHA256                = new SHA256Managed();
                CurrentDatabaseHashValue  = SHA256.ComputeHash(Encoding.Unicode.GetBytes(JSONWhitespaceRegEx.Replace(JSON.ToString(), " "))).
                                                   Select(value => String.Format("{0:x2}", value)).
                                                   Aggregate();

                JSON.Add(new JProperty("hashValue", CurrentDatabaseHashValue));

            }

        }

        #endregion

    }

}
