/*
 * Copyright (c) 2014-2016, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;
using org.GraphDefined.Vanaheimr.BouncyCastle;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;

#endregion

namespace org.GraphDefined.OpenData
{

    /// <summary>
    /// A library for managing users within and HTTP API or website.
    /// </summary>
    public class UsersAPI
    {

        #region Data

        /// <summary>
        /// Internal non-cryptographic random number generator.
        /// </summary>
        protected static readonly Random                              _Random                        = new Random();

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public  const             String                              DefaultHTTPServerName          = "GraphDefined Users API HTTP Service v0.8";

        /// <summary>
        /// The default HTTP server port.
        /// </summary>
        public  static readonly   IPPort                              DefaultHTTPServerPort          = new IPPort(2002);

        /// <summary>
        /// The HTTP root for embedded ressources.
        /// </summary>
        public const              String                              HTTPRoot                       = "org.GraphDefined.OpenData.UsersAPI.HTTPRoot.";

        /// <summary>
        /// The default service name.
        /// </summary>
        public  const             String                              DefaultServiceName             = "GraphDefined Users API";

        public  const             Byte                                DefaultMinUserNameLenght       = 4;
        public  const             Byte                                DefaultMinRealmLenght          = 2;
        public  const             Byte                                DefaultMinPasswordLenght       = 8;

        public  static readonly   TimeSpan                            DefaultSignInSessionLifetime   = TimeSpan.FromDays(30);


        private readonly          Dictionary<User_Id, LoginPassword>  _LoginPasswords;
        private readonly          List<VerificationToken>             _VerificationTokens;


        protected readonly        Func<String, Stream>                RessourcesProvider;


        /// <summary>
        /// Default logfile name.
        /// </summary>
        public  const             String                              DefaultLogfileName             = "UsersAPI.log";

        public const String SignUpContext     = "";
        public const String SignInOutContext  = "";
        public const String HTTPCookieId      = "OpenDataSocial";
        public const String HTTPCookieDomain  = "";

        #endregion

        #region Events

        #region RequestLog

        /// <summary>
        /// An event called whenever a request came in.
        /// </summary>
        public event RequestLogHandler RequestLog
        {

            add
            {
                HTTPServer.RequestLog += value;
            }

            remove
            {
                HTTPServer.RequestLog -= value;
            }

        }

        #endregion

        #region AccessLog

        /// <summary>
        /// An event called whenever a request could successfully be processed.
        /// </summary>
        public event AccessLogHandler AccessLog
        {

            add
            {
                HTTPServer.AccessLog += value;
            }

            remove
            {
                HTTPServer.AccessLog -= value;
            }

        }

        #endregion

        #region ErrorLog

        /// <summary>
        /// An event called whenever a request resulted in an error.
        /// </summary>
        public event ErrorLogHandler ErrorLog
        {

            add
            {
                HTTPServer.ErrorLog += value;
            }

            remove
            {
                HTTPServer.ErrorLog -= value;
            }

        }

        #endregion
        #endregion

        #region Properties

        /// <summary>
        /// The HTTP server of the API.
        /// </summary>
        public HTTPServer  HTTPServer   { get; }

        /// <summary>
        /// The HTTP hostname for all URIs within this API.
        /// </summary>
        public String      Hostname     { get; }

        /// <summary>
        /// The URI prefix of this HTTP API.
        /// </summary>
        public String      URIPrefix    { get; }


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

        #region APIAdminEMails

        private readonly EMailAddressList _APIAdminEMails;

        /// <summary>
        /// The E-Mail Addresses of the service admins.
        /// </summary>
        public EMailAddressList APIAdminEMails
        {
            get
            {
                return _APIAdminEMails;
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
        /// The collection of users.
        /// </summary>
        public Dictionary<User_Id, User> Users
        {
            get
            {
                return _Users;
            }
        }

        #endregion

        #region LogoImage

        protected readonly String _LogoImage;

        /// <summary>
        /// The logo of the website.
        /// </summary>
        public String LogoImage
        {
            get
            {
                return _LogoImage;
            }
        }

        #endregion

        #region DefaultLanguage

        private readonly Languages _DefaultLanguage;

        /// <summary>
        /// The default language of the API.
        /// </summary>
        public Languages DefaultLanguage
        {
            get
            {
                return _DefaultLanguage;
            }
        }

        #endregion

        #region NewUserSignUpEMailCreator

        /// <summary>
        /// A delegate for sending a sign-up e-mail to a new user.
        /// </summary>
        /// <param name="Login"></param>
        /// <param name="EMail"></param>
        /// <param name="Language"></param>
        /// <param name="VerificationToken"></param>
        public delegate EMail NewUserSignUpEMailCreatorDelegate(User_Id            Login,
                                                                EMailAddress       EMail,
                                                                Languages          Language,
                                                                VerificationToken  VerificationToken);

        private readonly NewUserSignUpEMailCreatorDelegate _NewUserSignUpEMailCreator;

        /// <summary>
        /// A delegate for sending a sign-up e-mail to a new user.
        /// </summary>
        public NewUserSignUpEMailCreatorDelegate NewUserSignUpEMailCreator
        {
            get
            {
                return _NewUserSignUpEMailCreator;
            }
        }

        #endregion

        #region NewUserWelcomeEMailCreator

        /// <summary>
        /// A delegate for sending a welcome e-mail to a new user.
        /// </summary>
        /// <param name="Login"></param>
        /// <param name="EMail"></param>
        /// <param name="Language"></param>
        public delegate EMail NewUserWelcomeEMailCreatorDelegate(User_Id       Login,
                                                                 EMailAddress  EMail,
                                                                 Languages     Language);

        private readonly NewUserWelcomeEMailCreatorDelegate _NewUserWelcomeEMailCreator;

        /// <summary>
        /// A delegate for sending a welcome e-mail to a new user.
        /// </summary>
        public NewUserWelcomeEMailCreatorDelegate NewUserWelcomeEMailCreator
        {
            get
            {
                return _NewUserWelcomeEMailCreator;
            }
        }

        #endregion

        #region ResetPasswordEMailCreator

        /// <summary>
        /// A delegate for sending a reset password e-mail to a user.
        /// </summary>
        /// <param name="Login"></param>
        /// <param name="EMail"></param>
        /// <param name="Language"></param>
        public delegate EMail ResetPasswordEMailCreatorDelegate(User_Id       Login,
                                                                EMailAddress  EMail,
                                                                String        Language);

        private readonly ResetPasswordEMailCreatorDelegate _ResetPasswordEMailCreator;

        /// <summary>
        /// A delegate for sending a reset password e-mail to a user.
        /// </summary>
        public ResetPasswordEMailCreatorDelegate ResetPasswordEMailCreator
        {
            get
            {
                return _ResetPasswordEMailCreator;
            }
        }

        #endregion

        #region SignInSessionLifetime

        private readonly TimeSpan _SignInSessionLifetime;

        public TimeSpan SignInSessionLifetime
        {
            get
            {
                return _SignInSessionLifetime;
            }
        }

        #endregion

        #region MinUserNameLenght

        private readonly Byte _MinUserNameLenght;

        /// <summary>
        /// The minimal user name length.
        /// </summary>
        public Byte MinUserNameLenght
        {
            get
            {
                return _MinUserNameLenght;
            }
        }

        #endregion

        #region MinRealmLenght

        private readonly Byte _MinRealmLenght;

        /// <summary>
        /// The minimal realm length.
        /// </summary>
        public Byte MinRealmLenght
        {
            get
            {
                return _MinRealmLenght;
            }
        }

        #endregion

        #region MinPasswordLenght

        private readonly Byte _MinPasswordLenght;

        /// <summary>
        /// The minimal password length.
        /// </summary>
        public Byte MinPasswordLenght
        {
            get
            {
                return _MinPasswordLenght;
            }
        }

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
        /// <param name="HTTPServerName">The default HTTP servername, used whenever no HTTP Host-header had been given.</param>
        /// <param name="HTTPServerPort">A TCP port to listen on.</param>
        /// <param name="HTTPHostname">The HTTP hostname for all URIs within this API.</param>
        /// <param name="URIPrefix">A common prefix for all URIs.</param>
        /// 
        /// <param name="ServiceName">The name of the service.</param>
        /// <param name="APIEMailAddress">An e-mail address for this API.</param>
        /// <param name="APIPublicKeyRing">A GPG public key for this API.</param>
        /// <param name="APISecretKeyRing">A GPG secret key for this API.</param>
        /// <param name="APIPassphrase">A GPG passphrase for this API.</param>
        /// <param name="APIAdminEMails">A list of admin e-mail addresses.</param>
        /// <param name="APISMTPClient">A SMTP client for sending e-mails.</param>
        /// 
        /// <param name="DefaultLanguage">The default language of the API.</param>
        /// <param name="LogoImage">The logo of the website.</param>
        /// <param name="NewUserSignUpEMailCreator">A delegate for sending a sign-up e-mail to a new user.</param>
        /// <param name="NewUserWelcomeEMailCreator">A delegate for sending a welcome e-mail to a new user.</param>
        /// <param name="ResetPasswordEMailCreator">A delegate for sending a reset password e-mail to a user.</param>
        /// <param name="MinUserNameLenght">The minimal user name length.</param>
        /// <param name="MinRealmLenght">The minimal realm length.</param>
        /// <param name="MinPasswordLenght">The minimal password length.</param>
        /// <param name="SignInSessionLifetime">The sign-in session lifetime.</param>
        /// 
        /// <param name="RessourcesProvider">A </param>
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
        /// <param name="DNSClient"></param>
        /// <param name="LogfileName"></param>
        public UsersAPI(String                              HTTPServerName                    = DefaultHTTPServerName,
                        IPPort                              HTTPServerPort                    = null,
                        String                              HTTPHostname                      = null,
                        String                              URIPrefix                         = "/",

                        String                              ServiceName                       = DefaultServiceName,
                        EMailAddress                        APIEMailAddress                   = null,
                        PgpPublicKeyRing                    APIPublicKeyRing                  = null,
                        PgpSecretKeyRing                    APISecretKeyRing                  = null,
                        String                              APIPassphrase                     = null,
                        EMailAddressList                    APIAdminEMails                    = null,
                        SMTPClient                          APISMTPClient                     = null,

                        Languages                           DefaultLanguage                   = Languages.eng,
                        String                              LogoImage                         = null,
                        NewUserSignUpEMailCreatorDelegate   NewUserSignUpEMailCreator         = null,
                        NewUserWelcomeEMailCreatorDelegate  NewUserWelcomeEMailCreator        = null,
                        ResetPasswordEMailCreatorDelegate   ResetPasswordEMailCreator         = null,
                        Byte                                MinUserNameLenght                 = DefaultMinUserNameLenght,
                        Byte                                MinRealmLenght                    = DefaultMinRealmLenght,
                        Byte                                MinPasswordLenght                 = DefaultMinPasswordLenght,
                        TimeSpan?                           SignInSessionLifetime             = null,

                        Func<String, Stream>                RessourcesProvider                = null,

                        String                              ServerThreadName                  = null,
                        ThreadPriority                      ServerThreadPriority              = ThreadPriority.AboveNormal,
                        Boolean                             ServerThreadIsBackground          = true,
                        ConnectionIdBuilder                 ConnectionIdBuilder               = null,
                        ConnectionThreadsNameBuilder        ConnectionThreadsNameBuilder      = null,
                        ConnectionThreadsPriorityBuilder    ConnectionThreadsPriorityBuilder  = null,
                        Boolean                             ConnectionThreadsAreBackground    = true,
                        TimeSpan?                           ConnectionTimeout                 = null,
                        UInt32                              MaxClientConnections              = TCPServer.__DefaultMaxClientConnections,

                        String                              LogfileName                       = DefaultLogfileName,
                        DNSClient                           DNSClient                         = null,
                        Boolean                             Autostart                         = false)

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
                   HTTPHostname,
                   URIPrefix,

                   ServiceName,
                   APIEMailAddress,
                   APIPublicKeyRing,
                   APISecretKeyRing,
                   APIPassphrase,
                   APIAdminEMails,
                   APISMTPClient,

                   DefaultLanguage,
                   LogoImage,
                   NewUserSignUpEMailCreator,
                   NewUserWelcomeEMailCreator,
                   ResetPasswordEMailCreator,
                   MinUserNameLenght,
                   MinRealmLenght,
                   MinPasswordLenght,
                   SignInSessionLifetime,

                   RessourcesProvider,

                   LogfileName)

        {

            if (Autostart)
                HTTPServer.Start();

        }

        #endregion

        #region (protected) UsersAPI(HTTPServer, URIPrefix = "/", ...)

        /// <summary>
        /// Attach this Open Data HTTP API to the given HTTP server.
        /// </summary>
        /// <param name="HTTPServer">An existing HTTP server.</param>
        /// <param name="HTTPHostname">The HTTP hostname for all URIs within this API.</param>
        /// <param name="URIPrefix">A common prefix for all URIs.</param>
        /// 
        /// <param name="ServiceName">The name of the service.</param>
        /// <param name="APIEMailAddress">An e-mail address for this API.</param>
        /// <param name="APIPublicKeyRing">A GPG public key for this API.</param>
        /// <param name="APISecretKeyRing">A GPG secret key for this API.</param>
        /// <param name="APIPassphrase">A GPG passphrase for this API.</param>
        /// <param name="APIAdminEMails">A list of admin e-mail addresses.</param>
        /// <param name="APISMTPClient">A SMTP client for sending e-mails.</param>
        /// 
        /// <param name="DefaultLanguage">The default language of the API.</param>
        /// <param name="LogoImage">The logo of the website.</param>
        /// <param name="NewUserSignUpEMailCreator">A delegate for sending a sign-up e-mail to a new user.</param>
        /// <param name="NewUserWelcomeEMailCreator">A delegate for sending a welcome e-mail to a new user.</param>
        /// <param name="ResetPasswordEMailCreator">A delegate for sending a reset password e-mail to a user.</param>
        /// <param name="MinUserNameLenght">The minimal user name length.</param>
        /// <param name="MinRealmLenght">The minimal realm length.</param>
        /// <param name="MinPasswordLenght">The minimal password length.</param>
        /// <param name="SignInSessionLifetime">The sign-in session lifetime.</param>
        /// 
        /// <param name="RessourcesProvider"></param>
        /// 
        /// <param name="LogfileName"></param>
        protected UsersAPI(HTTPServer                          HTTPServer,
                           String                              HTTPHostname                 = null,
                           String                              URIPrefix                    = "/",

                           String                              ServiceName                  = DefaultServiceName,
                           EMailAddress                        APIEMailAddress              = null,
                           PgpPublicKeyRing                    APIPublicKeyRing             = null,
                           PgpSecretKeyRing                    APISecretKeyRing             = null,
                           String                              APIPassphrase                = null,
                           EMailAddressList                    APIAdminEMails               = null,
                           SMTPClient                          APISMTPClient                = null,

                           Languages                           DefaultLanguage              = Languages.eng,
                           String                              LogoImage                    = null,
                           NewUserSignUpEMailCreatorDelegate   NewUserSignUpEMailCreator    = null,
                           NewUserWelcomeEMailCreatorDelegate  NewUserWelcomeEMailCreator   = null,
                           ResetPasswordEMailCreatorDelegate   ResetPasswordEMailCreator    = null,
                           Byte                                MinUserNameLenght            = DefaultMinUserNameLenght,
                           Byte                                MinRealmLenght               = DefaultMinRealmLenght,
                           Byte                                MinPasswordLenght            = DefaultMinPasswordLenght,
                           TimeSpan?                           SignInSessionLifetime        = null,

                           Func<String, Stream>                RessourcesProvider           = null,

                           String                              LogfileName                  = DefaultLogfileName)

        {

            #region Initial checks

            if (HTTPServer == null)
                throw new ArgumentNullException(nameof(HTTPServer), "HTTPServer!");

            if (NewUserSignUpEMailCreator  == null)
                throw new ArgumentNullException(nameof(NewUserSignUpEMailCreator),   "NewUserSignUpEMailCreator!");

            if (NewUserWelcomeEMailCreator == null)
                throw new ArgumentNullException(nameof(NewUserWelcomeEMailCreator),  "NewUserWelcomeEMailCreator!");

            if (ResetPasswordEMailCreator  == null)
                throw new ArgumentNullException(nameof(ResetPasswordEMailCreator),   "ResetPasswordEMailCreator!");

            #endregion

            #region Init data

            this.HTTPServer                  = HTTPServer;
            this.Hostname                    = HTTPHostname.IsNotNullOrEmpty() ? HTTPHostname : "*";
            this.URIPrefix                   = URIPrefix.   IsNotNullOrEmpty() ? URIPrefix    : "/";

            if (!this.URIPrefix.StartsWith("/", StringComparison.Ordinal))
                this.URIPrefix = "/" + this.URIPrefix;

            this._ServiceName                 = ServiceName. IsNotNullOrEmpty() ? ServiceName  : "UsersAPI";
            this._APIEMailAddress             = APIEMailAddress;
            this._APIPublicKeyRing            = APIPublicKeyRing;
            this._APISecretKeyRing            = APISecretKeyRing;
            this._APIPassphrase               = APIPassphrase;
            this._APIAdminEMails              = APIAdminEMails;
            this._APISMTPClient               = APISMTPClient;

            this._DefaultLanguage             = DefaultLanguage;
            this._LogoImage                   = LogoImage;
            this._NewUserSignUpEMailCreator   = NewUserSignUpEMailCreator;
            this._NewUserWelcomeEMailCreator  = NewUserWelcomeEMailCreator;
            this._ResetPasswordEMailCreator   = ResetPasswordEMailCreator;
            this._MinUserNameLenght           = MinUserNameLenght;
            this._MinRealmLenght              = MinRealmLenght;
            this._MinPasswordLenght           = MinPasswordLenght;
            this._SignInSessionLifetime       = SignInSessionLifetime.HasValue ? SignInSessionLifetime.Value : DefaultSignInSessionLifetime;

            this.RessourcesProvider           = RessourcesProvider;

            this._Users                       = new Dictionary<User_Id,      User>();
            this._Groups                      = new Dictionary<UserGroup_Id, UserGroup>();
            this._Messages                    = new Dictionary<Message_Id,   Message>();

            this._LoginPasswords              = new Dictionary<User_Id,      LoginPassword>();
            this._VerificationTokens          = new List<VerificationToken>();

            this._DNSClient                   = HTTPServer.DNSClient;

            #endregion

            RegisterURITemplates();

        }

        #endregion

        #endregion


        #region (static) AttachToHTTPAPI(HTTPServer, URIPrefix = "/", ...)

        /// <summary>
        /// Attach this Open Data HTTP API to the given HTTP server.
        /// </summary>
        /// <param name="HTTPServer">An existing HTTP server.</param>
        /// <param name="HTTPHostname">The HTTP hostname for all URIs within this API.</param>
        /// <param name="URIPrefix">A common prefix for all URIs.</param>
        /// 
        /// <param name="ServiceName">The name of the service.</param>
        /// <param name="APIEMailAddress">An e-mail address for this API.</param>
        /// <param name="APIPublicKeyRing">A GPG public key for this API.</param>
        /// <param name="APISecretKeyRing">A GPG secret key for this API.</param>
        /// <param name="APIPassphrase">A GPG passphrase for this API.</param>
        /// <param name="APIAdminEMails">A list of admin e-mail addresses.</param>
        /// <param name="APISMTPClient">A SMTP client for sending e-mails.</param>
        /// 
        /// <param name="DefaultLanguage">The default language of the API.</param>
        /// <param name="LogoImage">The logo of the website.</param>
        /// <param name="NewUserSignUpEMailCreator">A delegate for sending a sign-up e-mail to a new user.</param>
        /// <param name="NewUserWelcomeEMailCreator">A delegate for sending a welcome e-mail to a new user.</param>
        /// <param name="ResetPasswordEMailCreator">A delegate for sending a reset password e-mail to a user.</param>
        /// <param name="MinUserNameLenght">The minimal user name length.</param>
        /// <param name="MinRealmLenght">The minimal realm length.</param>
        /// <param name="MinPasswordLenght">The minimal password length.</param>
        /// <param name="SignInSessionLifetime">The sign-in session lifetime.</param>
        /// 
        /// <param name="RessourcesProvider"></param>
        /// 
        /// <param name="LogfileName"></param>
        public static UsersAPI AttachToHTTPAPI(HTTPServer                          HTTPServer,
                                               String                              HTTPHostname                 = "*",
                                               String                              URIPrefix                    = "/",

                                               String                              ServiceName                  = DefaultServiceName,
                                               EMailAddress                        APIEMailAddress              = null,
                                               PgpPublicKeyRing                    APIPublicKeyRing             = null,
                                               PgpSecretKeyRing                    APISecretKeyRing             = null,
                                               String                              APIPassphrase                = null,
                                               EMailAddressList                    APIAdminEMails               = null,
                                               SMTPClient                          APISMTPClient                = null,

                                               Languages                           DefaultLanguage              = Languages.eng,
                                               String                              LogoImage                    = null,
                                               NewUserSignUpEMailCreatorDelegate   NewUserSignUpEMailCreator    = null,
                                               NewUserWelcomeEMailCreatorDelegate  NewUserWelcomeEMailCreator   = null,
                                               ResetPasswordEMailCreatorDelegate   ResetPasswordEMailCreator    = null,
                                               Byte                                MinUserNameLenght            = DefaultMinUserNameLenght,
                                               Byte                                MinRealmLenght               = DefaultMinRealmLenght,
                                               Byte                                MinPasswordLenght            = DefaultMinPasswordLenght,
                                               TimeSpan?                           SignInSessionLifetime        = null,

                                               Func<String, Stream>                RessourcesProvider           = null,

                                               String                              LogfileName                  = DefaultLogfileName)


            => new UsersAPI(HTTPServer,
                            HTTPHostname,
                            URIPrefix,

                            ServiceName,
                            APIEMailAddress,
                            APIPublicKeyRing,
                            APISecretKeyRing,
                            APIPassphrase,
                            APIAdminEMails,
                            APISMTPClient,

                            DefaultLanguage,
                            LogoImage,
                            NewUserSignUpEMailCreator,
                            NewUserWelcomeEMailCreator,
                            ResetPasswordEMailCreator,
                            MinUserNameLenght,
                            MinRealmLenght,
                            MinPasswordLenght,
                            SignInSessionLifetime,

                            RessourcesProvider,

                            LogfileName);

        #endregion

        #region (private) RegisterURITemplates()

        private void RegisterURITemplates()
        {

            #region /shared/UsersAPI

            HTTPServer.RegisterResourcesFolder(HTTPHostname.Any, "/shared/UsersAPI", HTTPRoot.Substring(0, HTTPRoot.Length - 1));

            #endregion


            #region GET         ~/signup

            #region HTML_UTF8

            // -------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/signup
            // -------------------------------------------------------------
            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          new String[] { URIPrefix + "signup" },
                                          HTTPContentType.HTML_UTF8,
                                          HTTPDelegate: async Request => {

                                              var _MemoryStream1 = new MemoryStream();
                                              __GetRessources("template.html").SeekAndCopyTo(_MemoryStream1, 0);
                                              var Template = _MemoryStream1.ToArray().ToUTF8String();

                                              var _MemoryStream2 = new MemoryStream();
                                              typeof(UsersAPI).Assembly.GetManifestResourceStream(HTTPRoot + "SignUp.SignUp-" + DefaultLanguage.ToString() + ".html").SeekAndCopyTo(_MemoryStream2, 0);
                                              var HTML     = Template.Replace("<%= content %>",   _MemoryStream2.ToArray().ToUTF8String());

                                              if (LogoImage != null)
                                                  HTML = HTML.Replace("<%= logoimage %>", String.Concat(@"<img src=""", LogoImage, @""" /> "));

                                              return new HTTPResponseBuilder(Request) {
                                                  HTTPStatusCode  = HTTPStatusCode.OK,
                                                  ContentType     = HTTPContentType.HTML_UTF8,
                                                  Content         = HTML.ToUTF8Bytes(),
                                                  Connection      = "close"
                                              };

                                          });


            #endregion

            #endregion

            #region GET         ~/verificationtokens/{VerificationToken}

            #region HTML_UTF8

            // ----------------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/verificationtokens/0vu04w2hgf0w2h4bv08w
            // ----------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          "/verificationtokens/{VerificationToken}",
                                          HTTPContentType.HTML_UTF8,
                                          HTTPDelegate: async Request => {

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
                                                      Server = HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.HTML_UTF8,
                                                      Content = ("VerificationToken not found!").ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              User _User = null;
                                              if (!_Users.TryGetValue(VerificationToken.Login, out _User))
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.NotFound,
                                                      Server = HTTPServer.DefaultServerName,
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

                                                  var NewUserMail = NewUserWelcomeEMailCreatorLocal(Login:     VerificationToken.Login,
                                                                                                    EMail:     _User.EMail,
                                                                                                    Language:  DefaultLanguage);

                                                  var MailResultTask = APISMTPClient.Send(NewUserMail);

                                                  if (MailResultTask.Wait(60000))
                                                      MailSentResult = MailResultTask.Result;

                                              }

                                              if (MailSentResult == MailSentStatus.ok)
                                              {

                                                  #region Send Admin-Mail...

                                                  var AdminMail = new TextEMailBuilder() {
                                                      From = APIEMailAddress,
                                                      To = APIAdminEMails,
                                                      Subject = "New user activated: " + _User.Login.ToString() + " at " + DateTime.Now.ToString(),
                                                      Text = "New user activated: " + _User.Login.ToString() + " at " + DateTime.Now.ToString(),
                                                      Passphrase = APIPassphrase
                                                  };

                                                  var MailResultTask = APISMTPClient.Send(AdminMail).Wait(30000);

                                                  #endregion

                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.Created,
                                                      Server = HTTPServer.DefaultServerName,
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
                                                  Server = HTTPServer.DefaultServerName,
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

            #region GET         ~/lostpassword

            #region HTML_UTF8

            // -------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/lostpassword
            // -------------------------------------------------------------------
            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          new String[] { URIPrefix + "lostpassword" },
                                          HTTPContentType.HTML_UTF8,
                                          HTTPDelegate: async Request => {

                                              var _MemoryStream1 = new MemoryStream();
                                              __GetRessources("template.html").SeekAndCopyTo(_MemoryStream1, 0);
                                              var Template = _MemoryStream1.ToArray().ToUTF8String();

                                              var _MemoryStream2 = new MemoryStream();
                                              typeof(UsersAPI).Assembly.GetManifestResourceStream(HTTPRoot + "SignInOut.LostPassword-" + DefaultLanguage.ToString() + ".html").SeekAndCopyTo(_MemoryStream2, 0);
                                              var HTML     = Template.Replace("<%= content %>", _MemoryStream2.ToArray().ToUTF8String());

                                              return new HTTPResponseBuilder(Request) {
                                                  HTTPStatusCode  = HTTPStatusCode.OK,
                                                  ContentType     = HTTPContentType.HTML_UTF8,
                                                  Content         = HTML.ToUTF8Bytes(),
                                                  Connection      = "close"
                                              };

                                          });


            #endregion

            #endregion


            #region GET         ~/users

            #region HTML_UTF8

            // -----------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/users
            // -----------------------------------------------------------
            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          new String[] { URIPrefix + "/users",
                                                         URIPrefix + "/users/" },
                                          HTTPContentType.HTML_UTF8,
                                          HTTPDelegate: async Request => {

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

            HTTPServer.ITEMS_GET(UriTemplate: "/users",
                                  Dictionary: _Users,
                                  Filter: user => user.IsPublic,
                                  ToJSONDelegate: JSON.ToJSON);

            #endregion

            #endregion

            #region DEAUTH      ~/users

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.DEAUTH,
                                          "/users",
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: Request =>

                                              Task.FromResult(
                                                  new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode  = HTTPStatusCode.OK,
                                                      CacheControl    = "private",
                                                      SetCookie       = HTTPCookieId + "=; Expires=" + DateTime.Now.ToRfc1123() +
                                                                            (HTTPCookieDomain.IsNotNullOrEmpty()
                                                                                ? "; Domain=" + HTTPCookieDomain
                                                                                : "") +
                                                                            "; Path=/",
                                                      Connection      = "close"
                                                  }.AsImmutable()));

            #endregion

            #region ADD         ~/users/{UserId}

            #region JSON_UTF8

            // -------------------------------------------------------------------------------
            // curl -v -X ADD -H "Accept: application/json" http://127.0.0.1:2100/users/ahzf
            // -------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.ADD,
                                          "/users/{UserId}",
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: async Request => {

                                              #region Check JSON body...

                                              HTTPResponse _HTTPResponse = null;
                                              JSONWrapper NewUserData = null;
                                              if (!Request.TryParseJObjectRequestBody(out NewUserData, out _HTTPResponse))
                                                  return _HTTPResponse;

                                              if (!NewUserData.HasProperties)
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = HTTPServer.DefaultServerName,
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
                                                      Server = HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                             new JProperty("@context",     SignUpContext),
                                                                             new JProperty("property",     "name"),
                                                                             new JProperty("description",  "The name is invalid!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              #endregion

                                              #region Verify login/name

                                              if (_Login.Length < MinUserNameLenght)
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                             new JProperty("@context",     SignUpContext),
                                                                             new JProperty("property",     "name"),
                                                                             new JProperty("description",  "The name is too short!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              #endregion

                                              #region Verify email

                                              if (!NewUserData.ContainsKey("email"))
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                            new JProperty("@context",     SignUpContext),
                                                                            new JProperty("property",     "email"),
                                                                            new JProperty("description",  "Missing \"email\" property!")
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
                                                      Server = HTTPServer.DefaultServerName,
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
                                                      Server = HTTPServer.DefaultServerName,
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

                                              PgpPublicKeyRing PublicKeyRing = null;

                                              if (NewUserData.ContainsKey("GPGPublicKeyRing") &&
                                                  !OpenPGP.TryReadPublicKeyRing(NewUserData.GetString("GPGPublicKeyRing"), out PublicKeyRing))
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                             new JProperty("@context",     SignUpContext),
                                                                             new JProperty("property",     "gpgpublickeyring"),
                                                                             new JProperty("description",  "Invalid \"gpgpublickeyring\" property value!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              #endregion

                                              #region Verify password

                                              if (!NewUserData.ContainsKey("password"))
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                             new JProperty("@context",     SignUpContext),
                                                                             new JProperty("description",  "Missing \"password\" property!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              if (NewUserData.GetString("password").Length < MinPasswordLenght)
                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                      Server = HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                             new JProperty("@context",     SignUpContext),
                                                                             new JProperty("property",     "name"),
                                                                             new JProperty("description",  "The password is too short!")
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
                                                      Server = HTTPServer.DefaultServerName,
                                                      ContentType = HTTPContentType.JSON_UTF8,
                                                      Content = new JObject(
                                                                             new JProperty("@context",     SignUpContext),
                                                                             new JProperty("property",     "name"),
                                                                             new JProperty("description",  "The name is already in use!")
                                                                        ).ToString().ToUTF8Bytes(),
                                                      CacheControl = "public",
                                                      Connection = "close"
                                                  };

                                              #endregion

                                              var NewUser = CreateUser(Username:          _Login,
                                                                       Password:          NewUserData.GetString("password"),
                                                                       EMail:             SimpleEMailAddress.Parse(NewUserData.GetString("email")),
                                                                       GPGPublicKeyRing:  NewUserData.GetString("gpgpublickeyring"));

                                              var VerificationToken = _VerificationTokens.AddAndReturnElement(new VerificationToken(Seed: _Login.ToString() + NewUserData.GetString("password") + NewUserData.GetString("email"),
                                                                                                              UserId: _Login));

                                              #endregion

                                              #region Send New-User-Verification-E-Mail

                                              var MailSentResult = MailSentStatus.failed;

                                              var NewUserSignUpEMailCreatorLocal = NewUserSignUpEMailCreator;
                                              if (NewUserSignUpEMailCreatorLocal != null)
                                              {

                                                  var NewUserMail = NewUserSignUpEMailCreatorLocal(Login:              _Login,
                                                                                                   EMail:              new EMailAddress(matches.Groups[0].Value, null, PublicKeyRing),
                                                                                                   Language:           DefaultLanguage,
                                                                                                   VerificationToken:  VerificationToken);

                                                  var MailResultTask = APISMTPClient.Send(NewUserMail);

                                                  if (MailResultTask.Wait(60000))
                                                      MailSentResult = MailResultTask.Result;

                                              }

                                              if (MailSentResult == MailSentStatus.ok)
                                              {

                                                  #region Send Admin-Mail...

                                                  var AdminMail = new TextEMailBuilder() {
                                                      From = APIEMailAddress,
                                                      To = APIAdminEMails,
                                                      Subject = "New user registered: " + _Login + " <" + matches.Groups[0].Value + "> at " + DateTime.Now.ToString(),
                                                      Text = "New user registered: " + _Login + " <" + matches.Groups[0].Value + "> at " + DateTime.Now.ToString(),
                                                      Passphrase = APIPassphrase
                                                  };

                                                  var MailResultTask = APISMTPClient.Send(AdminMail).Wait(30000);

                                                  #endregion

                                                  return new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode = HTTPStatusCode.Created,
                                                      Server = HTTPServer.DefaultServerName,
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
                                                  Server = HTTPServer.DefaultServerName,
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

            HTTPServer.ITEM_EXISTS<User_Id, User>(UriTemplate: "/users/{UserId}",
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

            HTTPServer.ITEM_GET<User_Id, User>(UriTemplate: "/users/{UserId}",
                                                ParseIdDelegate: User_Id.TryParse,
                                                ParseIdError: Text => "Invalid user identification '" + Text + "'!",
                                                TryGetItemDelegate: _Users.TryGetValue,
                                                ItemFilterDelegate: user => user.IsPublic,
                                                TryGetItemError: userId => "Unknown user '" + userId + "'!",
                                                ToJSONDelegate: JSON.ToJSON);

            #endregion

            #endregion

            #region AUTH        ~/users/{UserId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.AUTH,
                                          "/users/{UserId}",
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: Request => {

                                              #region Check JSON body...

                                              //var Body = HTTPRequest.ParseJSONRequestBody();
                                              //if (Body.HasErrors)
                                              //    return Body.Error;

                                              HTTPResponse _HTTPResponse = null;
                                              JSONWrapper LoginData = null;
                                              if (!Request.TryParseJObjectRequestBody(out LoginData, out _HTTPResponse))
                                                  return Task.FromResult(_HTTPResponse);


                                              if (!LoginData.HasProperties)
                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                          Server          = HTTPServer.DefaultServerName,
                                                          ContentType     = HTTPContentType.JSON_UTF8,
                                                          Content         = new JObject(
                                                                                new JProperty("@context",     SignInOutContext),
                                                                                new JProperty("statuscode",   400),
                                                                                new JProperty("description",  "Invalid JSON!")
                                                                           ).ToUTF8Bytes(),
                                                          CacheControl    = "public",
                                                          Connection      = "close"
                                                      }.AsImmutable());

                                              #endregion

                                              // The login is taken from the URI, not from the JSON!
                                              LoginData.SetProperty("username", Request.ParsedURIParameters[0]);

                                              #region Verify username

                                              if (LoginData.GetString("username").Length < MinUserNameLenght)
                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                          Server = HTTPServer.DefaultServerName,
                                                          ContentType = HTTPContentType.JSON_UTF8,
                                                          Content = new JObject(
                                                                                 new JProperty("@context",     SignInOutContext),
                                                                                 new JProperty("statuscode",   400),
                                                                                 new JProperty("property",     "username"),
                                                                                 new JProperty("description",  "The login is too short!")
                                                                            ).ToString().ToUTF8Bytes(),
                                                          CacheControl = "public",
                                                          Connection = "close"
                                                      }.AsImmutable());

                                              #endregion

                                              #region Verify realm

                                              if (LoginData.ContainsKey("realm") &&
                                                  LoginData.GetString("realm").IsNotNullOrEmpty() &&
                                                  LoginData.GetString("realm").Length < MinRealmLenght)
                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                          Server = HTTPServer.DefaultServerName,
                                                          ContentType = HTTPContentType.JSON_UTF8,
                                                          Content = new JObject(
                                                                                new JProperty("@context",     SignInOutContext),
                                                                                new JProperty("statuscode",   400),
                                                                                new JProperty("property",     "realm"),
                                                                                new JProperty("description",  "The realm is too short!")
                                                                            ).ToString().ToUTF8Bytes(),
                                                          CacheControl = "public",
                                                          Connection = "close"
                                                      }.AsImmutable());

                                              #endregion

                                              #region Verify password

                                              if (!LoginData.ContainsKey("password"))
                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                          Server = HTTPServer.DefaultServerName,
                                                          ContentType = HTTPContentType.JSON_UTF8,
                                                          Content = new JObject(
                                                                                 new JProperty("@context",     SignInOutContext),
                                                                                 new JProperty("statuscode",   400),
                                                                                 new JProperty("description",  "Missing \"password\" property!")
                                                                            ).ToString().ToUTF8Bytes(),
                                                          CacheControl = "public",
                                                          Connection = "close"
                                                      }.AsImmutable());

                                              if (LoginData.GetString("password").Length < MinPasswordLenght)
                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode = HTTPStatusCode.BadRequest,
                                                          Server = HTTPServer.DefaultServerName,
                                                          ContentType = HTTPContentType.JSON_UTF8,
                                                          Content = new JObject(
                                                                                 new JProperty("@context",     SignInOutContext),
                                                                                 new JProperty("statuscode",   400),
                                                                                 new JProperty("property",     "name"),
                                                                                 new JProperty("description",  "The password is too short!")
                                                                            ).ToString().ToUTF8Bytes(),
                                                          CacheControl = "public",
                                                          Connection = "close"
                                                      }.AsImmutable());

                                              #endregion

                                              #region Check login and password

                                              User_Id       _UserId;
                                              LoginPassword _LoginPassword  = null;
                                              User          _User           = null;

                                              if (!User_Id.TryParse(LoginData.GetString("username"), out _UserId) ||
                                                  !_LoginPasswords.TryGetValue(_UserId, out _LoginPassword) ||
                                                  !_Users.TryGetValue(_UserId, out _User))

                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode = HTTPStatusCode.NotFound,
                                                          Server = HTTPServer.DefaultServerName,
                                                          ContentType = HTTPContentType.JSON_UTF8,
                                                          Content = new JObject(
                                                                                 new JProperty("@context",     SignInOutContext),
                                                                                 new JProperty("property",     "username"),
                                                                                 new JProperty("description",  "Unknown user!")
                                                                            ).ToString().ToUTF8Bytes(),
                                                          CacheControl = "public",
                                                          Connection = "close"
                                                      }.AsImmutable());


                                              if (!_LoginPassword.CheckPassword(LoginData.GetString("password")))

                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode = HTTPStatusCode.Unauthorized,
                                                          Server = HTTPServer.DefaultServerName,
                                                          ContentType = HTTPContentType.JSON_UTF8,
                                                          Content = new JObject(
                                                                                 new JProperty("@context",     SignInOutContext),
                                                                                 new JProperty("property",     "username"),
                                                                                 new JProperty("description",  "Invalid username or password!")
                                                                            ).ToString().ToUTF8Bytes(),
                                                          //SetCookie       = "SocialOpenData=" +
                                                          //                       "; Expires=" + DateTime.Now.AddMinutes(-5).ToRfc1123() +
                                                          //                          "; Path=/",
                                                          CacheControl = "public",
                                                          Connection = "close"
                                                      }.AsImmutable());

                                              #endregion


                                              var SHA256Hash = new SHA256Managed();
                                              var SecurityToken = SHA256Hash.ComputeHash((Guid.NewGuid().ToString() + _LoginPassword.Login + _LoginPassword.Realm).ToUTF8Bytes()).ToHexString();

                                              return Task.FromResult(
                                                  new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode  = HTTPStatusCode.Created,
                                                      ContentType     = HTTPContentType.TEXT_UTF8,
                                                      Content         = new JObject(
                                                                            new JProperty("@context",  SignInOutContext),
                                                                            new JProperty("username",  _LoginPassword.Login.ToString()),
                                                                            new JProperty("name",      _User.Name)
                                                                        ).ToUTF8Bytes(),
                                                      CacheControl    = "private",
                                                      SetCookie       = HTTPCookieId + "=username=" + _LoginPassword.Login.ToString().ToBase64() +
                                                                                       ":name=" + _User.Name.ToBase64() +
                                                                              ":securitytoken=" + SecurityToken +
                                                                                   "; Expires=" + DateTime.Now.Add(_SignInSessionLifetime).ToRfc1123() +
                                                                                    (HTTPCookieDomain.IsNotNullOrEmpty()
                                                                                        ? "; Domain=" + HTTPCookieDomain
                                                                                        : "") +
                                                                                      "; Path=/",
                                                      // secure;"
                                                      Connection = "close"
                                                  }.AsImmutable());

                                          });

            #endregion

            #region DEAUTH      ~/users/{UserId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.DEAUTH,
                                          "/users/{UserId}",
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: Request =>

                                              Task.FromResult(
                                                  new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode  = HTTPStatusCode.OK,
                                                      CacheControl    = "private",
                                                      SetCookie       = HTTPCookieId + "=; Expires=" + DateTime.Now.ToRfc1123() +
                                                                            (HTTPCookieDomain.IsNotNullOrEmpty()
                                                                                ? "; Domain=" + HTTPCookieDomain
                                                                                : "") +
                                                                            "; Path=/",
                                                      Connection      = "close"
                                                  }.AsImmutable()));

            #endregion

            #region GET         ~/users/{UserId}/profilephoto

            HTTPServer.RegisterFilesystemFile(HTTPHostname.Any,
                                               "/users/{UserId}/profilephoto",
                                               URIParams => "LocalHTTPRoot/data/Users/" + URIParams[0] + ".png",
                                               DefaultFile: "HTTPRoot/images/defaults/DefaultUser.png");

            #endregion


            #region GET         ~/groups

            #region HTML_UTF8

            // -----------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/groups
            // -----------------------------------------------------------
            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          new String[] { URIPrefix + "/groups",
                                                         URIPrefix + "/groups/" },
                                          HTTPContentType.HTML_UTF8,
                                          HTTPDelegate: async Request => {

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

            HTTPServer.ITEMS_GET(UriTemplate: "/groups",
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

            HTTPServer.ITEM_EXISTS<UserGroup_Id, UserGroup>(UriTemplate: "/groups/{GroupId}",
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

            HTTPServer.ITEM_GET<UserGroup_Id, UserGroup>(UriTemplate: "/groups/{GroupId}",
                                                          ParseIdDelegate: UserGroup_Id.TryParse,
                                                          ParseIdError: Text => "Invalid group identification '" + Text + "'!",
                                                          TryGetItemDelegate: _Groups.TryGetValue,
                                                          ItemFilterDelegate: group => group.IsPublic,
                                                          TryGetItemError: groupId => "Unknown group '" + groupId + "'!",
                                                          ToJSONDelegate: JSON.ToJSON);

            #endregion

            #endregion

        }

        #endregion


        #region (protected) __GetRessources(Ressource)

        protected Stream __GetRessources(String Ressource)
        {

            var DataStream = this.GetType().Assembly.GetManifestResourceStream("org.GraphDefined.UserAPI.HTTPRoot" + Ressource);

            if (DataStream == null)
                DataStream = RessourcesProvider(Ressource);

            return DataStream;

        }

        #endregion



        #region CreateUser(Username, Password, Name = null, EMail = null, GPGPublicKeyRing = null, Description = null, AuthenticateUser = false, HideUser = false)

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="Username">The unique identification of the user.</param>
        /// <param name="Password">The password of the user.</param>
        /// <param name="Name">The offical (multi-language) name of the user.</param>
        /// <param name="EMail">The primary e-mail of the user.</param>
        /// <param name="GPGPublicKeyRing">The PGP/GPG public keyring of the user.</param>
        /// <param name="Description">An optional (multi-language) description of the user.</param>
        /// <param name="AuthenticateUser"></param>
        /// <param name="HideUser"></param>
        public User CreateUser(User_Id             Username,
                               String              Password,
                               String              Name              = null,
                               SimpleEMailAddress  EMail             = null,
                               String              GPGPublicKeyRing  = null,
                               I18NString          Description       = null,
                               Boolean             AuthenticateUser  = false,
                               Boolean             HideUser          = false)
        {

            var User = new User(Username, Name, EMail, GPGPublicKeyRing, Description);

            if (AuthenticateUser)
                User.IsAuthenticated = true;

            if (HideUser)
                User.IsHidden        = true;

            SetPassword(Username, Password != null ? Password : "");

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

        // Create Mailinglist


        #region Start()

        public void Start()
        {

            lock (HTTPServer)
            {

                if (!HTTPServer.IsStarted)
                    HTTPServer.Start();

                //SendStarted(this, DateTime.Now);

            }

        }

        #endregion

        #region Shutdown(Message = null, Wait = true)

        public void Shutdown(String Message = null, Boolean Wait = true)
        {

            lock (HTTPServer)
            {

                HTTPServer.Shutdown(Message, Wait);
                //SendCompleted(this, DateTime.Now, Message);

            }

        }

        #endregion

        #region Dispose()

        public void Dispose()
        {

            lock (HTTPServer)
            {
                HTTPServer.Dispose();
            }

        }

        #endregion


    }

}
