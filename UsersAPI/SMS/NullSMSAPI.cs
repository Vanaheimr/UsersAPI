/*
 * Copyright (c) 2010-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of Vanaheimr Hermod <https://www.github.com/Vanaheimr/Hermod>
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
using System.Text;
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;
using com.GraphDefined.SMSApi.API;
using com.GraphDefined.SMSApi.API.Action;
using System.IO;
using System.Collections.Specialized;

#endregion

namespace org.GraphDefined.Vanaheimr.Hermod.SMTP
{

    /// <summary>
    /// A SMTP client for NOT sending, but logging e-mails.
    /// </summary>
    public class NullSMSAPI : ISMSClient
    {

        public class SMS
        {

            public String               Text         { get; }

            public IEnumerable<String>  Receivers    { get; }


            public SMS(String               Text,
                       IEnumerable<String>  Receivers)
            {
                this.Text       = Text;
                this.Receivers  = Receivers;
            }


        }


        #region Data

        private static readonly Random                       _Random               = new Random();
        private static readonly SHA256CryptoServiceProvider  _SHAHasher            = new SHA256CryptoServiceProvider();
        private static readonly SemaphoreSlim                SMSSemaphore          = new SemaphoreSlim(1, 1);
        public  static readonly TimeSpan                     SemaphoreSlimTimeout  = TimeSpan.FromSeconds(30);

        #endregion

        #region Properties


        #endregion

        #region Events

        public event OnSendSMSAPIRequestDelegate   OnSendSMSAPIRequest;

        public event OnSendSMSAPIResponseDelegate  OnSendSMSAPIResponse;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new SMTP client for NOT sending, but logging e-mails.
        /// </summary>
        public NullSMSAPI()
        {

        }

        #endregion



        #region SMSs

        #region Data

        private readonly List<SMS> _SMSs = new List<SMS>();

        /// <summary>
        /// An enumeration of all sent SMS.
        /// </summary>
        public IEnumerable<SMS> SMSs
        {
            get
            {

                if (SMSSemaphore.Wait(SemaphoreSlimTimeout))
                {
                    try
                    {

                        return _SMSs.ToArray();

                    }
                    finally
                    {
                        try
                        {
                            SMSSemaphore.Release();
                        }
                        catch
                        { }
                    }
                }

                return new SMS[0];

            }
        }

        #endregion

        #endregion






        public SMSSend Send(String Text, String Receiver)
        {
            _SMSs.Add(new SMS(Text, new String[] { Receiver }));
            return null;
        }

        public SMSSend Send(String Text, String[] Receivers)
        {
            _SMSs.Add(new SMS(Text, Receivers));
            return null;
        }

        public Task<Stream> Execute(string Command, NameValueCollection Data, Stream File, RequestMethods HTTPMethod = RequestMethods.POST)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> Execute(string Command, NameValueCollection Data, Dictionary<string, Stream> Files = null, RequestMethods HTTPMethod = RequestMethods.POST)
        {
            throw new NotImplementedException();
        }


        #region Clear()

        public void Clear()
        {
            if (SMSSemaphore.Wait(TimeSpan.FromSeconds(60)))
            {
                try
                {

                    _SMSs.Clear();

                }
                catch (Exception e)
                {
                    DebugX.LogException(e);
                }
                finally
                {
                    SMSSemaphore.Release();
                }
            }
        }

        #endregion


        #region Dispose()

        public void Dispose()
        {

        }

        #endregion

    }

}
