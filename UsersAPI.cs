/*
 * Copyright (c) 2014-2015, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using System.Security;
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
using org.GraphDefined.Vanaheimr.Hermod.Services.CSV;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.UDP;
using org.GraphDefined.Vanaheimr.BouncyCastle;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;

#endregion

namespace org.GraphDefined.UsersAPI
{

    /// <summary>
    /// A library for managing users within and HTTP API or website.
    /// </summary>
    public class UsersAPI
    {

        #region Data

        private static readonly Random          _Random                         = new Random();

        private const           String          DefaultHTTPServerName           = "Users API HTTP Service v0.8";
        private static readonly IPPort          DefaultHTTPServerPort           = new IPPort(2002);

        private readonly Func<String, Stream> _GetRessources;

        public  const           LogLevel        MinLogLevel_Console             = LogLevel.INFO;
        public  const           LogLevel        MinLogLevel_LogFile             = LogLevel.INFO;
        public  const           String          DefaultLogfileName              = "UsersAPI.log";

        public const            Byte            MinUserNameLenght       = 4;
        public const            Byte            MinRealmLenght          = 2;
        public const            Byte            MinPasswordLenght       = 8;

        public readonly         TimeSpan        SignInSessionLifetime   = TimeSpan.FromDays(30);

        private readonly        Dictionary<User_Id, LoginPassword>      _LoginPasswords;
        private readonly        List<VerificationToken>                 _VerificationTokens;

        #endregion

        #region Properties

        #region HTTPServer

        protected readonly HTTPServer _HTTPServer;

        /// <summary>
        /// The HTTP server of the Open Data API.
        /// </summary>
        public HTTPServer HTTPServer
        {
            get
            {
                return _HTTPServer;
            }
        }

        #endregion

        #region URIPrefix

        private readonly String _URIPrefix;

        public String URIPrefix
        {
            get
            {
                return _URIPrefix;
            }
        }

        #endregion


        #region ServiceName

        private readonly String _ServiceName;

        /// <summary>
        /// The name of the Open Data API service.
        /// </summary>
        public String ServiceName
        {
            get
            {
                return _ServiceName;
            }
        }

        #endregion

        #region APIEMailAddress

        private readonly EMailAddress _APIEMailAddress;

        /// <summary>
        /// A sender e-mail address for the Open Data API.
        /// </summary>
        public EMailAddress APIEMailAddress
        {
            get
            {
                return _APIEMailAddress;
            }
        }

        #endregion

        #region APIPublicKeyRing

        private readonly PgpPublicKeyRing _APIPublicKeyRing;

        /// <summary>
        /// The PGP/GPG public key ring of the Open Data API.
        /// </summary>
        public PgpPublicKeyRing APIPublicKeyRing
        {
            get
            {
                return _APIPublicKeyRing;
            }
        }

        #endregion

        #region APISecretKeyRing

        private readonly PgpSecretKeyRing _APISecretKeyRing;

        /// <summary>
        /// The PGP/GPG secret key ring of the Open Data API.
        /// </summary>
        public PgpSecretKeyRing APISecretKeyRing
        {
            get
            {
                return _APISecretKeyRing;
            }
        }

        #endregion

        #region APIPassphrase

        private readonly String _APIPassphrase;

        /// <summary>
        /// The passphrase of the PGP/GPG secret key of the Open Data API.
        /// </summary>
        public String APIPassphrase
        {
            get
            {
                return _APIPassphrase;
            }
        }

        #endregion

        #region APIAdminEMail

        private readonly EMailAddressList _APIAdminEMail;

        /// <summary>
        /// The E-Mail Addresses of the service admins.
        /// </summary>
        public EMailAddressList APIAdminEMail
        {
            get
            {
                return _APIAdminEMail;
            }
        }

        #endregion

        #region APISMTPClient

        private readonly SMTPClient _APISMTPClient;

        /// <summary>
        /// A SMTP client to be used by the Open Data API.
        /// </summary>
        public SMTPClient APISMTPClient
        {
            get
            {
                return _APISMTPClient;
            }
        }

        #endregion

        #region DNSClient

        protected readonly DNSClient _DNSClient = null;

        /// <summary>
        /// The DNS resolver to use.
        /// </summary>
        public DNSClient DNSClient
        {
            get
            {
                return _DNSClient;
            }
        }

        #endregion


        #region Users

        protected readonly Dictionary<User_Id, User> _Users;

        /// <summary>
        /// A collection of users.
        /// </summary>
        public Dictionary<User_Id, User> Users
        {
            get
            {
                return _Users;
            }
        }

        #endregion

        #region NewUserSignUpEMailCreator

        public delegate EMail NewUserSignUpEMailCreatorDelegate(User_Id Login, EMailAddress EMail, String Language, VerificationToken VerificationToken);

        /// <summary>
        /// A delegate to create a "New User Sign Up"-mail.
        /// </summary>
        public NewUserSignUpEMailCreatorDelegate NewUserSignUpEMailCreator { get; set; }

        #endregion

        #region NewUserWelcomeEMailCreator

        public delegate EMail NewUserWelcomeEMailCreatorDelegate(User_Id Login, EMailAddress EMail, String Language);

        /// <summary>
        /// A delegate to create a "New User Welcome"-mail.
        /// </summary>
        public NewUserWelcomeEMailCreatorDelegate NewUserWelcomeEMailCreator { get; set; }

        #endregion

        #region ResetPasswordEMailCreator

        public delegate EMail ResetPasswordEMailCreatorDelegate(User_Id Login, SimpleEMailAddress EMail, String Language);

        /// <summary>
        /// A delegate to create a "Reset Password"-mail.
        /// </summary>
        public ResetPasswordEMailCreatorDelegate ResetPasswordEMailCreator { get; set; }

        #endregion


        #region UserGroups

        protected readonly Dictionary<UserGroup_Id, UserGroup> _Groups;

        /// <summary>
        /// A collection of user groups.
        /// </summary>
        public Dictionary<UserGroup_Id, UserGroup> Groups
        {
            get
            {
                return _Groups;
            }
        }

        #endregion

        #region Messages

        protected readonly Dictionary<Message_Id, Message> _Messages;

        /// <summary>
        /// A collection of message.
        /// </summary>
        public Dictionary<Message_Id, Message> Message
        {
            get
            {
                return _Messages;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        #region UsersAPI(HTTPServerName, ...)

        /// <summary>
        /// Create a new HTTP server and attach this Open Data HTTP API to it.
        /// </summary>
        /// <param name="HTTPServerName"></param>
        /// <param name="HTTPServerPort"></param>
        /// <param name="URIPrefix"></param>
        /// 
        /// <param name="ServerThreadName"></param>
        /// <param name="ServerThreadPriority"></param>
        /// <param name="ServerThreadIsBackground"></param>
        /// <param name="ConnectionIdBuilder"></param>
        /// <param name="ConnectionThreadsNameBuilder"></param>
        /// <param name="ConnectionThreadsPriorityBuilder"></param>
        /// <param name="ConnectionThreadsAreBackground"></param>
        /// <param name="ConnectionTimeout"></param>
        /// <param name="MaxClientConnections"></param>
        /// 
        /// <param name="ServiceName"></param>
        /// <param name="APIEMailAddress"></param>
        /// <param name="APIPublicKeyRing"></param>
        /// <param name="APISecretKeyRing"></param>
        /// <param name="APIPassphrase"></param>
        /// <param name="APIAdminEMail"></param>
        /// <param name="APISMTPClient"></param>
        /// <param name="GetRessources"></param>
        /// 
        /// <param name="DNSClient"></param>
        /// <param name="LogfileName"></param>
        public UsersAPI(String                            HTTPServerName                    = DefaultHTTPServerName,
                        IPPort                            HTTPServerPort                    = null,
                        String                            URIPrefix                         = "/",

                        String                            ServerThreadName                  = null,
                        ThreadPriority                    ServerThreadPriority              = ThreadPriority.AboveNormal,
                        Boolean                           ServerThreadIsBackground          = true,
                        ConnectionIdBuilder               ConnectionIdBuilder               = null,
                        ConnectionThreadsNameBuilder      ConnectionThreadsNameBuilder      = null,
                        ConnectionThreadsPriorityBuilder  ConnectionThreadsPriorityBuilder  = null,
                        Boolean                           ConnectionThreadsAreBackground    = true,
                        TimeSpan?                         ConnectionTimeout                 = null,
                        UInt32                            MaxClientConnections              = TCPServer.__DefaultMaxClientConnections,

                        String                            ServiceName                       = "Open Data API",
                        EMailAddress                      APIEMailAddress                   = null,
                        PgpPublicKeyRing                  APIPublicKeyRing                  = null,
                        PgpSecretKeyRing                  APISecretKeyRing                  = null,
                        String                            APIPassphrase                     = null,
                        EMailAddressList                  APIAdminEMail                     = null,
                        SMTPClient                        APISMTPClient                     = null,
                        Func<String, Stream>              GetRessources                     = null,

                        DNSClient             DNSClient                = null,
                        String                LogfileName              = DefaultLogfileName)

            : this(new HTTPServer(TCPPort:                           HTTPServerPort != null ? HTTPServerPort : DefaultHTTPServerPort,
                                  DefaultServerName:                 HTTPServerName,
                                  ServerThreadName:                  ServerThreadName,
                                  ServerThreadPriority:              ServerThreadPriority,
                                  ServerThreadIsBackground:          ServerThreadIsBackground,
                                  ConnectionIdBuilder:               ConnectionIdBuilder,
                                  ConnectionThreadsNameBuilder:      ConnectionThreadsNameBuilder,
                                  ConnectionThreadsPriorityBuilder:  ConnectionThreadsPriorityBuilder,
                                  ConnectionThreadsAreBackground:    ConnectionThreadsAreBackground,
                                  ConnectionTimeout:                 ConnectionTimeout,
                                  MaxClientConnections:              MaxClientConnections,
                                  DNSClient:                         DNSClient,
                                  Autostart:                         false),
                   URIPrefix,

                   ServiceName,
                   APIEMailAddress,
                   APIPublicKeyRing,
                   APISecretKeyRing,
                   APIPassphrase,
                   APIAdminEMail,
                   APISMTPClient,
                   GetRessources,

                   DNSClient,
                   LogfileName)

        {

            #region Configure HTTP Server events

            // Server events...
            _HTTPServer.OnStarted           += (Sender,    Timestamp, Message)                                    => Console.WriteLine("[" + Timestamp + "] '" + (Sender as HTTPServer).DefaultServerName + "' started on port(s) " + (Sender as HTTPServer).Select(tcpserver => tcpserver.Port).AggregateWith(", ") + (Message.IsNotNullOrEmpty() ? "; msg: '" + Message + "'..." : ""));
            //_HTTPServer.OnNewConnection     += (TCPServer, Timestamp, RemoteSocket, ConnectionId, TCPConnection)  => Console.WriteLine("[" + Timestamp + "] New TCP/HTTP connection from " + TCPConnection.RemoteSocket.ToString());
            _HTTPServer.OnExceptionOccured  += (Sender,    Timestamp, Exception)                                  => Console.WriteLine("[" + Timestamp + "] HTTP exception occured: '" + Exception.Message + "'");
            //_HTTPServer.OnConnectionClosed  += (Sender,    Timestamp, RemoteSocket, ConnectionId, ClosedBy)       => Console.WriteLine("[" + Timestamp + "] TCP/HTTP connection from " + RemoteSocket.ToString() + " closed by " + ClosedBy.ToString().ToLower() + "!");
            _HTTPServer.OnCompleted         += (Sender,    Timestamp, Message)                                    => Console.WriteLine("[" + Timestamp + "] '" + (Sender as HTTPServer).DefaultServerName + "' shutdown" + (Message.IsNotNullOrEmpty() ? "; msg: '" + Message + "'..." : "..."));

            // HTTP events...
            //_HTTPServer.RequestLog          += (Sender, Timestamp, Request)            => Console.WriteLine("[" + Timestamp + "] " + Request.HTTPMethod + " " + Request.URI);
            //_HTTPServer.AccessLog           += (Sender, Timestamp, Request, Response)  => Console.WriteLine("[" + Timestamp + "] " + Request.HTTPMethod + " " + Request.URI + " => " + Response.HTTPStatusCode.SimpleString);


            _HTTPServer.AccessLog += (HTTPServer, ServerTimestamp, Request, Response) => {

                Console.WriteLine("[" + ServerTimestamp.ToString() + "] " +
                                  (Request.X_Forwarded_For != null
                                     ? Request.X_Forwarded_For + "(" +  Request.RemoteSocket + ") - "
                                     : Request.RemoteSocket + " - ") +
                                  Request.HTTPMethod   + " " +
                                  Request.URI          + " " +
                                  Response.HTTPStatusCode + " " +
                                  Response.ContentLength + " bytes");

            };

            _HTTPServer.ErrorLog += (HTTPServer, ServerTimestamp, Request, Response, Error, LastException) => {

                var _error            = (Error         == null) ? "" : Error;
                var _exceptionMessage = (LastException == null) ? "" : Environment.NewLine + LastException.Message;

                Console.Write("[" + ServerTimestamp.ToString() + "] " +
                              (Request.X_Forwarded_For != null
                                  ? Request.X_Forwarded_For + "(" + Request.RemoteSocket + ") - "
                                  : Request.RemoteSocket + " - ") +
                              Request.HTTPMethod + " " +
                              Request.URI        + " => ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[" + Response.HTTPStatusCode + "] ");
                Console.ResetColor();
                Console.WriteLine((_error.           IsNotNullOrEmpty() ? _error  + "/"     : "" ) +
                                  (_exceptionMessage.IsNotNullOrEmpty() ? _exceptionMessage : ""));

            };

            #endregion

        }

        #endregion

        #region (protected) UsersAPI(HTTPServer, URIPrefix = "/", ...)

        /// <summary>
        /// Attach this Open Data HTTP API to the given HTTP server.
        /// </summary>
        /// <param name="HTTPServer"></param>
        /// <param name="URIPrefix"></param>
        /// 
        /// <param name="ServiceName"></param>
        /// <param name="APIEMailAddress"></param>
        /// <param name="APIPublicKeyRing"></param>
        /// <param name="APISecretKeyRing"></param>
        /// <param name="APIPassphrase"></param>
        /// <param name="APIAdminEMail"></param>
        /// <param name="APISMTPClient"></param>
        /// <param name="GetRessources"></param>
        /// 
        /// <param name="DNSClient"></param>
        /// <param name="LogfileName"></param>
        protected UsersAPI(HTTPServer HTTPServer,
                           String URIPrefix = "/",

                           String ServiceName = "Open Data API",
                           EMailAddress APIEMailAddress = null,
                           PgpPublicKeyRing APIPublicKeyRing = null,
                           PgpSecretKeyRing APISecretKeyRing = null,
                           String APIPassphrase = null,
                           EMailAddressList APIAdminEMail = null,
                           SMTPClient APISMTPClient = null,
                           Func<String, Stream> GetRessources = null,

                           DNSClient DNSClient = null,
                           String LogfileName = DefaultLogfileName)

        {

            #region Init data

            this._HTTPServer = HTTPServer;
            this._URIPrefix = URIPrefix;
            this._GetRessources = GetRessources;

            this._ServiceName = ServiceName;
            this._APIEMailAddress = APIEMailAddress;
            this._APIPublicKeyRing = APIPublicKeyRing;
            this._APISecretKeyRing = APISecretKeyRing;
            this._APIPassphrase = APIPassphrase;
            this._APIAdminEMail = APIAdminEMail;
            this._APISMTPClient = APISMTPClient;

            this._Users     = new Dictionary<User_Id, User>();
            this._Groups    = new Dictionary<UserGroup_Id, UserGroup>();
            this._Messages  = new Dictionary<Message_Id, Message>();

            this._LoginPasswords = new Dictionary<User_Id, LoginPassword>();
            this._VerificationTokens = new List<VerificationToken>();

            this._DNSClient = (DNSClient != null) ? DNSClient : new DNSClient(SearchForIPv6DNSServers: false);

            #endregion

            #region Register HTTP URI-templates

            #region /shared

            _HTTPServer.RegisterResourcesFolder("/shared", "org.GraphDefined.OpenData.API.HTTPRoot");

            #endregion

            #region /raw

            _HTTPServer.AddMethodCallback(HTTPMethod.GET,
                                          "/raw",
                                          HTTPContentType.HTML_UTF8,
                                          Request => {

                                              return new HTTPResponseBuilder(Request) {
                                                  HTTPStatusCode = HTTPStatusCode.OK,
                                                  ContentType = HTTPContentType.TEXT_UTF8,
                                                  Content = Request.RawHTTPHeader.ToString().ToUTF8Bytes(),
                                                  CacheControl = "private",
                                                  //Expires         = "Mon, 25 Jun 2015 21:31:12 GMT",
                                                  Connection = "close"
                                              };

                                          });

            #endregion


            #region GET         ~/signup

            #region HTML_UTF8

            // -------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/signup
            // -------------------------------------------------------------
            _HTTPServer.Redirect(HTTPMethod.GET,
                                 "/signup",
                                 HTTPContentType.HTML_UTF8,
                                 "/index.html");

            #endregion

            #endregion

            #region GET         ~/verificationtokens/{VerificationToken}

            #region HTML_UTF8

            // ----------------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/verificationtokens/0vu04w2hgf0w2h4bv08w
            // ----------------------------------------------------------------------------------------------
            _HTTPServer.AddMethodCallback(HTTPMethod.GET,
                                          "/verificationtokens/{VerificationToken}",
                                          HTTPContentType.HTML_UTF8,
                                          Request => {

                                              VerificationToken VerificationToken = null;

                                              foreach (var _VerificationToken in _VerificationTokens)
                                                  if (_VerificationToken.ToString() == Request.ParsedURIParameters[0])
                                                  {
                                                      VerificationToken = _VerificationToken;
                                                      break;
                                                  }

                                              if (VerificationToken == null)
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.NotFound,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.HTML_UTF8,
                                                      Content = ("VerificationToken not found!").ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              User _User = null;
                                              if (!_Users.TryGetValue(VerificationToken.Login, out _User))
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.NotFound,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.HTML_UTF8,
                                                      Content = ("Login not found!").ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              _VerificationTokens.Remove(VerificationToken);

                                              _User.IsAuthenticated = true;

                                              #region Send New-User-Welcome-E-Mail

                                              var MailSentResult = MailSentStatus.failed;

                                              var NewUserWelcomeEMailCreatorLocal = NewUserWelcomeEMailCreator;
                                              if (NewUserWelcomeEMailCreatorLocal != null)
                                              {

                                                  var NewUserMail = NewUserWelcomeEMailCreatorLocal(Login: VerificationToken.Login,
                                                                                                       EMail: _User.EMail,
                                                                                                       Language: "de");

                                                  var MailResultTask = APISMTPClient.Send(NewUserMail);

                                                  if (MailResultTask.Wait(60000))
                                                      MailSentResult = MailResultTask.Result;

                                              }

                                              if (MailSentResult == MailSentStatus.ok)
                                              {

                                                  #region Send Admin-Mail...

                                                  var AdminMail = new TextEMailBuilder() {
                                                      From = APIEMailAddress,
                                                      To = APIAdminEMail,
                                                      Subject = "New user activated: " + _User.Login.ToString() + " at " + DateTime.Now.ToString(),
                                                      Text = "New user activated: " + _User.Login.ToString() + " at " + DateTime.Now.ToString(),
                                                      Passphrase = APIPassphrase
                                                  };

                                                  var MailResultTask = APISMTPClient.Send(AdminMail).Wait(30000);

                                                  #endregion

                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.Created,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                               new JProperty("@context", ""),
                                                                               new JProperty("@id", _User.Login.ToString()),
                                                                               new JProperty("email", _User.EMail.ToString())
                                                                          ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      //Expires         = "Mon, 25 Jun 2015 21:31:12 GMT",
                                                      Connection = "close"
                                                  };

                                              }

                                              #endregion

                                              return new HTTPResponseBuilder(Request) {
                                                  HTTPStatusCode = HTTPStatusCode.OK,
                                                  Server = _HTTPServer.DefaultServerName,
                                                  ContentType = HTTPContentType.HTML_UTF8,
                                                  Content = ("Account '" + VerificationToken.Login.ToString() + "' activated!").ToUTF8Bytes(),
                                                  CacheControl = "public",
                                                  ETag = "1",
                                                  //Expires         = "Mon, 25 Jun 2015 21:31:12 GMT",
                                                  Connection = "close"
                                              };

                                          });

            #endregion

            #endregion


            #region GET         ~/users

            #region HTML_UTF8

            // -----------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/users
            // -----------------------------------------------------------
            _HTTPServer.AddMethodCallback(HTTPMethod.GET,
                                          new String[] { _URIPrefix + "/users",
                                                         _URIPrefix + "/users/" },
                                          HTTPContentType.HTML_UTF8,
                                          HTTPDelegate: Request => {

                                              var _MemoryStream = new MemoryStream();
                                              __GetRessources("_header.html").SeekAndCopyTo(_MemoryStream, 3);
                                              __GetRessources("users.index.html").SeekAndCopyTo(_MemoryStream, 3);
                                              __GetRessources("_footer.html").SeekAndCopyTo(_MemoryStream, 3);

                                              return new HTTPResponseBuilder(Request) {
                                                  HTTPStatusCode = HTTPStatusCode.OK,
                                                  ContentType = HTTPContentType.HTML_UTF8,
                                                  Content = _MemoryStream.ToArray(),
                                                  Connection = "close"
                                              };

                                          });

            #endregion

            #region JSON_UTF8

            // -------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/users
            // -------------------------------------------------------------------

            _HTTPServer.ITEMS_GET(UriTemplate: "/users",
                                  Dictionary: _Users,
                                  Filter: user => user.IsPublic,
                                  ToJSONDelegate: JSON.ToJSON);

            #endregion

            #endregion

            #region ADD         ~/users/{UserId}

            #region JSON_UTF8

            // -------------------------------------------------------------------------------
            // curl -v -X ADD -H "Accept: application/json" http://127.0.0.1:2100/users/ahzf
            // -------------------------------------------------------------------------------
            _HTTPServer.AddMethodCallback(HTTPMethod.ADD,
                                          "/users/{UserId}",
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: Request => {

                                              #region Check JSON body...

                                              HTTPResponse _HTTPResponse = null;
                                              JSONWrapper NewUserData = null;
                                              if (!Request.TryParseJObjectRequestBody(out NewUserData, out _HTTPResponse))
                                                  return _HTTPResponse;

                                              if (!NewUserData.HasProperties)
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                             new JProperty("@context", ""),
                                                                             new JProperty("statuscode", 400),
                                                                             new JProperty("description", "Invalid JSON!")
                                                                        ).ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              #endregion

                                              //var NewUserData = Body.Properties().ToDictionary(prop => prop.Name. ToLower(). Trim(),
                                              //                                                 prop => prop.Value.ToString().Trim());

                                              #region The 'login/name' is taken from the URI, not from the JSON!

                                              User_Id _Login = null;

                                              if (!User_Id.TryParse(Request.ParsedURIParameters[0], out _Login))
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                             new JProperty("@context", ""),
                                                                             new JProperty("property", "name"),
                                                                             new JProperty("description", "The name is invalid!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              #endregion

                                              #region Verify login/name

                                              if (_Login.Length < MinUserNameLenght)
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                             new JProperty("@context", ""),
                                                                             new JProperty("property", "name"),
                                                                             new JProperty("description", "The name is too short!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              #endregion

                                              #region Verify email

                                              if (!NewUserData.ContainsKey("email"))
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                            new JProperty("@context", ""),
                                                                            new JProperty("property", "email"),
                                                                            new JProperty("description", "Missing \"email\" property!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              //ToDo: See rfc5322 for more complex regular expression!
                                              //      [a-z0-9!#$%&'*+/=?^_`{|}~-]+
                                              // (?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*
                                              //                 @
                                              // (?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+
                                              //    [a-z0-9](?:[a-z0-9-]*[a-z0-9])?
                                              var matches = Regex.Match(NewUserData.GetString("email").Trim(), @"([^\@]+)\@([^\@]+\.[^\@]{2,})");

                                              if (!matches.Success)
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                            new JProperty("@context", ""),
                                                                            new JProperty("property", "email"),
                                                                            new JProperty("description", "Invalid \"email\" property!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              //ToDo: CNAMEs are also okay according to rfc5321#section-5
                                              var MailServerA = DNSClient.Query<A>(matches.Groups[2].Value);
                                              var MailServerAAAA = DNSClient.Query<AAAA>(matches.Groups[2].Value);
                                              var MailServerMX = DNSClient.Query<MX>(matches.Groups[2].Value);

                                              Task.WaitAll(MailServerA, MailServerAAAA, MailServerMX);

                                              if (MailServerA.Result.Count() == 0 &
                                                  MailServerAAAA.Result.Count() == 0 &
                                                  MailServerMX.Result.Count() == 0)
                                              {
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                            new JProperty("@context", ""),
                                                                            new JProperty("property", "email"),
                                                                            new JProperty("description", "Unknown or invalid domain name!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };
                                              }

                                              EMailAddress EMail = null;
                                              // if (!EMailAddress.)

                                              #endregion

                                              #region Verify GPG public key

                                              if (!NewUserData.ContainsKey("gpgpublickeyring"))
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                             new JProperty("@context", ""),
                                                                             new JProperty("property", "gpgpublickeyring"),
                                                                             new JProperty("description", "Missing \"gpgpublickeyring\" property!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              PgpPublicKeyRing PublicKeyRing = null;

                                              if (NewUserData.ContainsKey("gpgpublickeyring") &&
                                                  !OpenPGP.TryReadPublicKeyRing(NewUserData.GetString("gpgpublickeyring"), out PublicKeyRing))
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                             new JProperty("@context", ""),
                                                                             new JProperty("property", "gpgpublickeyring"),
                                                                             new JProperty("description", "Invalid \"gpgpublickeyring\" property value!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              #endregion

                                              #region Verify password

                                              if (!NewUserData.ContainsKey("password"))
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                             new JProperty("@context", ""),
                                                                             new JProperty("description", "Missing \"password\" property!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              if (NewUserData.GetString("password").Length < MinPasswordLenght)
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                             new JProperty("@context", ""),
                                                                             new JProperty("property", "name"),
                                                                             new JProperty("description", "The password is too short!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              #endregion


                                              //ToDo: Not yet 100% thread-safe!
                                              #region Add new user

                                              #region Verify new user does not exist

                                              if (_LoginPasswords.ContainsKey(_Login))

                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.Conflict,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                             new JProperty("@context", ""),
                                                                             new JProperty("property", "name"),
                                                                             new JProperty("description", "The name is already in use!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              #endregion

                                              var NewUser = CreateUser(Login: _Login,
                                                                       Password: NewUserData.GetString("password"),
                                                                       EMail: SimpleEMailAddress.Parse(NewUserData.GetString("email")),
                                                                       GPGPublicKeyRing: NewUserData.GetString("gpgpublickeyring"));

                                              var VerificationToken = new VerificationToken(Seed: _Login.ToString() + NewUserData.GetString("password") + NewUserData.GetString("email"),
                                                                                            UserId: _Login);

                                              _VerificationTokens.Add(VerificationToken);

                                              #endregion

                                              #region Send New-User-Verification-E-Mail

                                              var MailSentResult = MailSentStatus.failed;

                                              var NewUserSignUpEMailCreatorLocal = NewUserSignUpEMailCreator;
                                              if (NewUserSignUpEMailCreatorLocal != null)
                                              {

                                                  var NewUserMail = NewUserSignUpEMailCreatorLocal(Login: _Login,
                                                                                                      EMail: new EMailAddress(matches.Groups[0].Value, null, PublicKeyRing),
                                                                                                      Language: "de",
                                                                                                      VerificationToken: VerificationToken);

                                                  var MailResultTask = APISMTPClient.Send(NewUserMail);

                                                  if (MailResultTask.Wait(60000))
                                                      MailSentResult = MailResultTask.Result;

                                              }

                                              if (MailSentResult == MailSentStatus.ok)
                                              {

                                                  #region Send Admin-Mail...

                                                  var AdminMail = new TextEMailBuilder() {
                                                      From = APIEMailAddress,
                                                      To = APIAdminEMail,
                                                      Subject = "New user registered: " + _Login + " <" + matches.Groups[0].Value + "> at " + DateTime.Now.ToString(),
                                                      Text = "New user registered: " + _Login + " <" + matches.Groups[0].Value + "> at " + DateTime.Now.ToString(),
                                                      Passphrase = APIPassphrase
                                                  };

                                                  var MailResultTask = APISMTPClient.Send(AdminMail).Wait(30000);

                                                  #endregion

                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.Created,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                               new JProperty("@context", ""),
                                                                               new JProperty("@id", _Login.ToString()),
                                                                               new JProperty("email", NewUser.EMail.ToString())
                                                                          ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      //Expires         = "Mon, 25 Jun 2015 21:31:12 GMT",
                                                      Connection = "close"
                                                  };

                                              }

                                              #endregion

                                              return new HTTPResponseBuilder(Request) {
                                                  HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                  Server = _HTTPServer.DefaultServerName,
                                                  ContentType = HTTPContentType.JSON_UTF8,
                                                  Content = new JObject(
                                                                         new JProperty("@context", ""),
                                                                         new JProperty("description", "Something wicked happened!")
                                                                    ).ToString().ToUTF8Bytes(),
                                                  CacheControl = "public",
                                                  Connection = "close"
                                              };

                                          });

            #endregion

            #endregion

            #region EXISTS      ~/users/{UserId}

            #region JSON_UTF8

            // ---------------------------------------------------------------------------------
            // curl -v -X EXITS -H "Accept: application/json" http://127.0.0.1:2100/users/ahzf
            // ---------------------------------------------------------------------------------

            _HTTPServer.ITEM_EXISTS<User_Id, User>(UriTemplate: "/users/{UserId}",
                                                   ParseIdDelegate: User_Id.TryParse,
                                                   ParseIdError: Text => "Invalid user identification '" + Text + "'!",
                                                   TryGetItemDelegate: _Users.TryGetValue,
                                                   ItemFilterDelegate: user => user.IsPublic,
                                                   TryGetItemError: userId => "Unknown user '" + userId + "'!");

            #endregion

            #endregion

            #region GET         ~/users/{UserId}

            #region JSON_UTF8

            // ------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/users/ahzf
            // ------------------------------------------------------------------------

            _HTTPServer.ITEM_GET<User_Id, User>(UriTemplate: "/users/{UserId}",
                                                ParseIdDelegate: User_Id.TryParse,
                                                ParseIdError: Text => "Invalid user identification '" + Text + "'!",
                                                TryGetItemDelegate: _Users.TryGetValue,
                                                ItemFilterDelegate: user => user.IsPublic,
                                                TryGetItemError: userId => "Unknown user '" + userId + "'!",
                                                ToJSONDelegate: JSON.ToJSON);

            #endregion

            #endregion

            #region AUTH        ~/users/{UserId}

            _HTTPServer.AddMethodCallback(HTTPMethod.AUTH,
                                          "/users/{UserId}",
                                          HTTPContentType.JSON_UTF8,
                                          Request => {

                                              #region Check JSON body...

                                              //var Body = HTTPRequest.ParseJSONRequestBody();
                                              //if (Body.HasErrors)
                                              //    return Body.Error;

                                              HTTPResponse _HTTPResponse = null;
                                              JSONWrapper LoginData = null;
                                              if (!Request.TryParseJObjectRequestBody(out LoginData, out _HTTPResponse))
                                                  return _HTTPResponse;


                                              if (!LoginData.HasProperties)
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                             new JProperty("@context", ""),
                                                                             new JProperty("statuscode", 400),
                                                                             new JProperty("description", "Invalid JSON!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              #endregion

                                              //  var LoginData = Body.Properties().ToDictionary(prop => prop.Name. ToLower(). Trim(),
                                              //                                                 prop => prop.Value.ToString().Trim());
                                              //var LoginData = Body;

                                              // The login is taken from the URI, not from the JSON!
                                              LoginData.SetProperty("login", Request.ParsedURIParameters[0]);

                                              #region Verify login

                                              if (LoginData.GetString("login").Length < MinUserNameLenght)
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                             new JProperty("@context", ""),
                                                                             new JProperty("statuscode", 400),
                                                                             new JProperty("property", "login"),
                                                                             new JProperty("description", "The login is too short!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              #endregion

                                              #region Verify realm

                                              if (LoginData.ContainsKey("realm") &&
                                                  LoginData.GetString("realm").Length < MinRealmLenght)
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                            new JProperty("@context", ""),
                                                                            new JProperty("statuscode", 400),
                                                                            new JProperty("property", "realm"),
                                                                            new JProperty("description", "The realm is too short!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              #endregion

                                              #region Verify password

                                              if (!LoginData.ContainsKey("password"))
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                             new JProperty("@context", ""),
                                                                             new JProperty("statuscode", 400),
                                                                             new JProperty("description", "Missing \"password\" property!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              if (LoginData.GetString("password").Length < MinPasswordLenght)
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                             new JProperty("@context", ""),
                                                                             new JProperty("statuscode", 400),
                                                                             new JProperty("property", "name"),
                                                                             new JProperty("description", "The password is too short!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              #endregion

                                              #region Check login and password

                                              User_Id _UserId;
                                              LoginPassword _LoginPassword = null;
                                              User _User = null;

                                              if (!User_Id.TryParse(LoginData.GetString("login"), out _UserId) ||
                                                  !_LoginPasswords.TryGetValue(_UserId, out _LoginPassword) ||
                                                  !_Users.TryGetValue(_UserId, out _User))

                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.NotFound,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                             new JProperty("@context", ""),
                                                                             new JProperty("property", "login"),
                                                                             new JProperty("description", "The login does not exist!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };


                                              if (_LoginPassword.CheckPassword(LoginData.GetString("password")))

                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.Unauthorized,
                                                      Server = _HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                             new JProperty("@context", ""),
                                                                             new JProperty("property", "login"),
                                                                             new JProperty("description", "Invalid login or password!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      //SetCookie       = "SocialOpenData=" +
                                                      //                       "; Expires=" + DateTime.Now.AddMinutes(-5).ToRfc1123() +
                                                      //                          "; Path=/",
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              #endregion


                                              var SHA256Hash = new SHA256Managed();
                                              var SecurityToken = SHA256Hash.ComputeHash((Guid.NewGuid().ToString() + _LoginPassword.Login + _LoginPassword.Realm).ToUTF8Bytes()).ToHexString();

                                              return new HTTPResponseBuilder(Request) {
                                                  HTTPStatusCode = HTTPStatusCode.Created,
                                                  ContentType = HTTPContentType.TEXT_UTF8,
                                                  Content = new JObject(
                                                                             new JProperty("@context", ""),
                                                                             new JProperty("login", _LoginPassword.Login.ToString()),
                                                                             new JProperty("username", _User.Name)
                                                                        ).ToString().ToUTF8Bytes(),
                                                  CacheControl = "private",
                                                  SetCookie = "SocialOpenData=login=" + _LoginPassword.Login.ToString().ToBase64() +
                                                                               ":username=" + _User.Name.ToBase64() +
                                                                          ":securitytoken=" + SecurityToken +
                                                                               "; Expires=" + DateTime.Now.Add(SignInSessionLifetime).ToRfc1123() +
                                                                                  "; Path=/",
                                                  // Domain=.offenes-jena.de;
                                                  // secure;"
                                                  Connection = "close"
                                              };

                                          });

            #endregion

            #region DEAUTH      ~/users/{UserId}

            _HTTPServer.AddMethodCallback(HTTPMethod.DEAUTH,
                                          "/users/{UserId}",
                                          HTTPContentType.JSON_UTF8,
                                          Request => {

                                              return new HTTPResponseBuilder(Request) {
                                                  HTTPStatusCode = HTTPStatusCode.OK,
                                                  ContentType = HTTPContentType.TEXT_UTF8,
                                                  Content = ("ok").ToUTF8Bytes(),
                                                  CacheControl = "private",
                                                  //Expires         = "Mon, 25 Jun 2015 21:31:12 GMT",
                                                  SetCookie = "SocialOpenData=; Expires=Wed, 24 Nov 2016 09:44:55 GMT; Path=/", // Domain=.xing.com;
                                                  Connection = "close"
                                              };

                                          });

            #endregion

            #region GET         ~/users/{UserId}/profilephoto

            _HTTPServer.RegisterFilesystemFile("/users/{UserId}/profilephoto",
                                               URIParams => "LocalHTTPRoot/data/Users/" + URIParams[0] + ".png",
                                               DefaultFile: "HTTPRoot/images/defaults/DefaultUser.png");

            #endregion


            #region GET         ~/groups

            #region HTML_UTF8

            // -----------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/groups
            // -----------------------------------------------------------
            _HTTPServer.AddMethodCallback(HTTPMethod.GET,
                                          new String[] { _URIPrefix + "/groups",
                                                         _URIPrefix + "/groups/" },
                                          HTTPContentType.HTML_UTF8,
                                          HTTPDelegate: Request => {

                                              var _MemoryStream = new MemoryStream();
                                              __GetRessources("_header.html").SeekAndCopyTo(_MemoryStream, 3);
                                              __GetRessources("groups.index.html").SeekAndCopyTo(_MemoryStream, 3);
                                              __GetRessources("_footer.html").SeekAndCopyTo(_MemoryStream, 3);

                                              return new HTTPResponseBuilder(Request) {
                                                  HTTPStatusCode = HTTPStatusCode.OK,
                                                  ContentType = HTTPContentType.HTML_UTF8,
                                                  Content = _MemoryStream.ToArray(),
                                                  Connection = "close"
                                              };

                                          });

            #endregion

            #region JSON_UTF8

            // ------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/groups
            // ------------------------------------------------------------------

            _HTTPServer.ITEMS_GET(UriTemplate: "/groups",
                                  Dictionary: _Groups,
                                  Filter: group => group.IsPublic,
                                  ToJSONDelegate: JSON.ToJSON);

            #endregion

            #endregion

            #region EXISTS      ~/groups/{GroupId}

            #region JSON_UTF8

            // -------------------------------------------------------------------------------------------
            // curl -v -X EXITS -H "Accept: application/json" http://127.0.0.1:2100/groups/OK-Lab%20Jena
            // -------------------------------------------------------------------------------------------

            _HTTPServer.ITEM_EXISTS<UserGroup_Id, UserGroup>(UriTemplate: "/groups/{GroupId}",
                                                             ParseIdDelegate: UserGroup_Id.TryParse,
                                                             ParseIdError: Text => "Invalid group identification '" + Text + "'!",
                                                             TryGetItemDelegate: _Groups.TryGetValue,
                                                             ItemFilterDelegate: group => group.IsPublic,
                                                             TryGetItemError: groupId => "Unknown group '" + groupId + "'!");

            #endregion

            #endregion

            #region GET         ~/groups/{GroupId}

            #region JSON_UTF8

            // ----------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/groups/OK-Lab%20Jena
            // ----------------------------------------------------------------------------------

            _HTTPServer.ITEM_GET<UserGroup_Id, UserGroup>(UriTemplate: "/groups/{GroupId}",
                                                          ParseIdDelegate: UserGroup_Id.TryParse,
                                                          ParseIdError: Text => "Invalid group identification '" + Text + "'!",
                                                          TryGetItemDelegate: _Groups.TryGetValue,
                                                          ItemFilterDelegate: group => group.IsPublic,
                                                          TryGetItemError: groupId => "Unknown group '" + groupId + "'!",
                                                          ToJSONDelegate: JSON.ToJSON);

            #endregion

            #endregion

            #endregion

        }

        #endregion

        #endregion


        #region (static) AttachToHTTPAPI(HTTPServer, URIPrefix = "/", ...)

        /// <summary>
        /// Attach this Open Data HTTP API to the given HTTP server.
        /// </summary>
        /// <param name="HTTPServer"></param>
        /// <param name="URIPrefix"></param>
        /// <param name="ServiceName"></param>
        /// <param name="APIEMailAddress"></param>
        /// <param name="APIPublicKeyRing"></param>
        /// <param name="APISecretKeyRing"></param>
        /// <param name="APIPassphrase"></param>
        /// <param name="APIAdminEMail"></param>
        /// <param name="APISMTPClient"></param>
        /// <param name="GetRessources"></param>
        /// 
        /// <param name="DNSClient"></param>
        /// <param name="LogfileName"></param>
        public static UsersAPI AttachToHTTPAPI(HTTPServer HTTPServer,
                                               String URIPrefix = "/",

                                               String ServiceName = "Users API",
                                               EMailAddress APIEMailAddress = null,
                                               PgpPublicKeyRing APIPublicKeyRing = null,
                                               PgpSecretKeyRing APISecretKeyRing = null,
                                               String APIPassphrase = null,
                                               EMailAddressList APIAdminEMail = null,
                                               SMTPClient APISMTPClient = null,
                                               Func<String, Stream> GetRessources = null,

                                               DNSClient DNSClient = null,
                                               String LogfileName = DefaultLogfileName)

        {

            return new UsersAPI(HTTPServer,
                                URIPrefix,

                                ServiceName,
                                APIEMailAddress,
                                APIPublicKeyRing,
                                APISecretKeyRing,
                                APIPassphrase,
                                APIAdminEMail,
                                APISMTPClient,
                                GetRessources,

                                DNSClient,
                                LogfileName);

        }

        #endregion

        #region (private) __GetRessources(Ressource)

        private Stream __GetRessources(String Ressource)
        {

            var DataStream = this.GetType().Assembly.GetManifestResourceStream("org.GraphDefined.UserAPI.HTTPRoot" + Ressource);

            if (DataStream == null)
                DataStream = _GetRessources(Ressource);

            return DataStream;

        }

        #endregion




        #region CreateUser(Login, Password, Name = null, EMail = null, GPGPublicKeyRing = null, Description = null, AuthenticateUser = false, HideUser = false)

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="Login">The unique identification of the user.</param>
        /// <param name="Password">The password of the user.</param>
        /// <param name="Name">The offical (multi-language) name of the user.</param>
        /// <param name="EMail">The primary e-mail of the user.</param>
        /// <param name="GPGPublicKeyRing">The PGP/GPG public keyring of the user.</param>
        /// <param name="Description">An optional (multi-language) description of the user.</param>
        /// <param name="AuthenticateUser"></param>
        /// <param name="HideUser"></param>
        public User CreateUser(User_Id             Login,
                               String              Password,
                               String              Name              = null,
                               SimpleEMailAddress  EMail             = null,
                               String              GPGPublicKeyRing  = null,
                               I18NString          Description       = null,
                               Boolean             AuthenticateUser  = false,
                               Boolean             HideUser          = false)
        {

            var User = new User(Login, Name, EMail, GPGPublicKeyRing, Description);

            if (AuthenticateUser)
                User.IsAuthenticated = true;

            if (HideUser)
                User.IsHidden        = true;

            SetPassword(Login, Password != null ? Password : "");

            return _Users.AddAndReturnValue(User.Id, User);

        }

        #endregion

        #region SetPassword(Login, Password, Realm = null)

        public void SetPassword(User_Id Login,
                                String  Password,
                                String  Realm  = null)
        {

            _LoginPasswords.Add(Login, new LoginPassword(Login, Password, Realm = null));

        }

        public void SetPassword(User_Id       Login,
                                SecureString  Password,
                                String        Realm  = null)
        {

            _LoginPasswords.Add(Login, new LoginPassword(Login, Password, Realm = null));

        }

        #endregion

        #region CreateGroup(Id, Name = null, Description = null)

        public UserGroup CreateGroup(UserGroup_Id  Id,
                                     I18NString    Name         = null,
                                     I18NString    Description  = null)
        {

            var Group = new UserGroup(Id, Name, Description);

            return _Groups.AddAndReturnValue(Group.Id, Group);

        }

        #endregion

        #region CreateMessage(Id, Headline = null, Text = null)

        public Message CreateMessage(Message_Id  Id,
                                     I18NString  Headline  = null,
                                     I18NString  Text      = null)
        {

            var Message = new Message(Id, Headline, Text);

            return _Messages.AddAndReturnValue(Message.Id, Message);

        }

        #endregion


    }

}
