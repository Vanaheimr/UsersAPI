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

namespace org.GraphDefined.OpenData.Users
{

    /// <summary>
    /// Extention method for the Users API.
    /// </summary>
    public static class UsersAPIExtentions
    {

        #region ParseUserId(this HTTPRequest, UsersAPI, out UserId,           out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the user identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="UsersAPI">The Users API.</param>
        /// <param name="UserId">The parsed unique user identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseUserId(this HTTPRequest  HTTPRequest,
                                          UsersAPI          UsersAPI,
                                          out User_Id?      UserId,
                                          out HTTPResponse  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest == null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (UsersAPI    == null)
                throw new ArgumentNullException(nameof(UsersAPI),     "The given Users API must not be null!");

            #endregion

            UserId        = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURIParameters.Length < 1)
            {

                HTTPResponse = new HTTPResponseBuilder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                };

                return false;

            }

            UserId = User_Id.TryParse(HTTPRequest.ParsedURIParameters[0], "");

            if (!UserId.HasValue)
            {

                HTTPResponse = new HTTPResponseBuilder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid UserId!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseUser  (this HTTPRequest, UsersAPI, out UserId, out User, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the user identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="UsersAPI">The Users API.</param>
        /// <param name="UserId">The parsed unique user identification.</param>
        /// <param name="User">The resolved user.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseUser(this HTTPRequest  HTTPRequest,
                                        UsersAPI          UsersAPI,
                                        out User_Id?      UserId,
                                        out User          User,
                                        out HTTPResponse  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest == null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (UsersAPI    == null)
                throw new ArgumentNullException(nameof(UsersAPI),     "The given Users API must not be null!");

            #endregion

            UserId        = null;
            User          = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURIParameters.Length < 1) {

                HTTPResponse = new HTTPResponseBuilder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                };

                return false;

            }

            UserId = User_Id.TryParse(HTTPRequest.ParsedURIParameters[0], "");

            if (!UserId.HasValue) {

                HTTPResponse = new HTTPResponseBuilder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid UserId!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            if (!UsersAPI.TryGetUser(UserId.Value, out User)) {

                HTTPResponse = new HTTPResponseBuilder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown UserId!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

    }


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
        public  static readonly   IPPort                              DefaultHTTPServerPort          = IPPort.Parse(2002);

        /// <summary>
        /// The HTTP root for embedded ressources.
        /// </summary>
        public const              String                              HTTPRoot                       = "org.GraphDefined.OpenData.UsersAPI.HTTPRoot.";

        /// <summary>
        /// The default service name.
        /// </summary>
        public  const             String                              DefaultServiceName             = "GraphDefined Users API";

        /// <summary>
        /// The default language of the API.
        /// </summary>
        public  const             Languages                           DefaultLanguage                = Languages.eng;

        public  const             Byte                                DefaultMinUserNameLenght       = 4;
        public  const             Byte                                DefaultMinRealmLenght          = 2;
        public  const             Byte                                DefaultMinPasswordLenght       = 8;

        public  static readonly   TimeSpan                            DefaultSignInSessionLifetime   = TimeSpan.FromDays(30);


        private readonly          Dictionary<User_Id, LoginPassword>  _LoginPasswords;
        private readonly          List<VerificationToken>             _VerificationTokens;

        /// <summary>
        /// Default logfile name.
        /// </summary>
        public  const             String                              DefaultLogfileName             = "UsersAPI.log";

        public  const             String                              SignUpContext                  = "";
        public  const             String                              SignInOutContext               = "";

        /// <summary>
        /// The name of the default HTTP cookie.
        /// </summary>
        public  static readonly   HTTPCookieName                      DefaultCookieName              = HTTPCookieName.Parse("UsersAPI");

        public  const             String                              HTTPCookieDomain               = "";

        public  const             String                              DefaultUsersAPIFile            = "UsersAPI_Users.db";
        public  const             String                              DefaultPasswordFile            = "UsersAPI_Passwords.db";
        public  const             String                              SecurityTokenCookieKey         = "securitytoken";
        public  const             String                              DefaultSecurityTokenFile       = "UsersAPI_SecurityTokens.db";
        //public  const             String                              DefaultGroupDBFile             = "UsersAPI_Groups.db";
        //public  const             String                              DefaultUser2GroupDBFile        = "UsersAPI_User2Group.db";
        public  const             String                              AdminGroupName                 = "Admins";


        protected readonly Dictionary<SecurityToken_Id, SecurityToken> SecurityTokens;

        public static readonly Regex JSONWhitespaceRegEx = new Regex(@"(\s)+", RegexOptions.IgnorePatternWhitespace);

        protected readonly Notifications _Notifications;

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
        public HTTPServer    HTTPServer   { get; }

        /// <summary>
        /// The HTTP hostname for all URIs within this API.
        /// </summary>
        public HTTPHostname  Hostname     { get; }

        /// <summary>
        /// The URI prefix of this HTTP API.
        /// </summary>
        public HTTPURI       URIPrefix    { get; }


        #region ServiceName

        /// <summary>
        /// The name of the Open Data API service.
        /// </summary>
        public String ServiceName { get; }

        #endregion

        #region APIEMailAddress

        /// <summary>
        /// A sender e-mail address for the Open Data API.
        /// </summary>
        public EMailAddress APIEMailAddress { get; }

        #endregion

        #region APIPublicKeyRing

        /// <summary>
        /// The PGP/GPG public key ring of the Open Data API.
        /// </summary>
        public PgpPublicKeyRing APIPublicKeyRing { get; }

        #endregion

        #region APISecretKeyRing

        /// <summary>
        /// The PGP/GPG secret key ring of the Open Data API.
        /// </summary>
        public PgpSecretKeyRing APISecretKeyRing { get; }

        #endregion

        #region APIPassphrase

        /// <summary>
        /// The passphrase of the PGP/GPG secret key of the Open Data API.
        /// </summary>
        public String APIPassphrase { get; }

        #endregion

        #region APIAdminEMails

        /// <summary>
        /// The E-Mail Addresses of the service admins.
        /// </summary>
        public EMailAddressList APIAdminEMails { get; }

        #endregion

        #region APISMTPClient

        /// <summary>
        /// A SMTP client to be used by the Open Data API.
        /// </summary>
        public SMTPClient APISMTPClient { get; }

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


        public Group Admins { get; }


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

        #region CookieName

        public HTTPCookieName CookieName { get; }

        #endregion

        #region Language

        /// <summary>
        /// The main language of this API.
        /// </summary>
        public Languages Language { get; }

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

        /// <summary>
        /// A delegate for sending a sign-up e-mail to a new user.
        /// </summary>
        public NewUserSignUpEMailCreatorDelegate NewUserSignUpEMailCreator { get; }

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

        /// <summary>
        /// A delegate for sending a welcome e-mail to a new user.
        /// </summary>
        public NewUserWelcomeEMailCreatorDelegate NewUserWelcomeEMailCreator { get; }

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

        /// <summary>
        /// A delegate for sending a reset password e-mail to a user.
        /// </summary>
        public ResetPasswordEMailCreatorDelegate ResetPasswordEMailCreator { get; }

        #endregion

        #region SignInSessionLifetime

        public TimeSpan SignInSessionLifetime { get; }

        #endregion

        #region MinUserNameLenght

        /// <summary>
        /// The minimal user name length.
        /// </summary>
        public Byte MinLoginLenght { get; }

        #endregion

        #region MinRealmLenght

        /// <summary>
        /// The minimal realm length.
        /// </summary>
        public Byte MinRealmLenght { get; }

        #endregion

        #region MinPasswordLenght

        /// <summary>
        /// The minimal password length.
        /// </summary>
        public Byte MinPasswordLenght { get; }

        #endregion


        /// <summary>
        /// The current hash value of the API.
        /// </summary>
        public String CurrentDatabaseHashValue { get; private set; }

        public String SystemId { get; }

        /// <summary>
        /// Disable the log file.
        /// </summary>
        public Boolean DisableLogfile { get; }

        /// <summary>
        /// Disable external notifications.
        /// </summary>
        public Boolean DisableNotifications { get; }

        /// <summary>
        /// The logfile of this API.
        /// </summary>
        public String LogfileName { get; }

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
        /// <param name="ServerCertificateSelector">An optional delegate to select a SSL/TLS server certificate.</param>
        /// <param name="ClientCertificateValidator">An optional delegate to verify the SSL/TLS client certificate used for authentication.</param>
        /// <param name="ClientCertificateSelector">An optional delegate to select the SSL/TLS client certificate used for authentication.</param>
        /// <param name="AllowedTLSProtocols">The SSL/TLS protocol(s) allowed for this connection.</param>
        /// 
        /// <param name="ServiceName">The name of the service.</param>
        /// <param name="APIEMailAddress">An e-mail address for this API.</param>
        /// <param name="APIPublicKeyRing">A GPG public key for this API.</param>
        /// <param name="APISecretKeyRing">A GPG secret key for this API.</param>
        /// <param name="APIPassphrase">A GPG passphrase for this API.</param>
        /// <param name="APIAdminEMails">A list of admin e-mail addresses.</param>
        /// <param name="APISMTPClient">A SMTP client for sending e-mails.</param>
        /// 
        /// <param name="CookieName">The name of the HTTP Cookie for authentication.</param>
        /// <param name="Language">The main language of the API.</param>
        /// <param name="LogoImage">The logo of the website.</param>
        /// <param name="NewUserSignUpEMailCreator">A delegate for sending a sign-up e-mail to a new user.</param>
        /// <param name="NewUserWelcomeEMailCreator">A delegate for sending a welcome e-mail to a new user.</param>
        /// <param name="ResetPasswordEMailCreator">A delegate for sending a reset password e-mail to a user.</param>
        /// <param name="MinUserNameLenght">The minimal user name length.</param>
        /// <param name="MinRealmLenght">The minimal realm length.</param>
        /// <param name="MinPasswordLenght">The minimal password length.</param>
        /// <param name="SignInSessionLifetime">The sign-in session lifetime.</param>
        /// 
        /// <param name="SkipURITemplates">Skip URI templates.</param>
        /// <param name="DisableNotifications">Disable external notifications.</param>
        /// <param name="DisableLogfile">Disable the log file.</param>
        /// <param name="LogfileName">The name of the logfile for this API.</param>
        /// <param name="DNSClient">The DNS client of the API.</param>
        /// <param name="Autostart">Whether to start the API automatically.</param>
        public UsersAPI(String                               HTTPServerName                     = DefaultHTTPServerName,
                        IPPort?                              HTTPServerPort                     = null,
                        HTTPHostname?                        HTTPHostname                       = null,
                        HTTPURI?                             URIPrefix                          = null,

                        ServerCertificateSelectorDelegate    ServerCertificateSelector          = null,
                        RemoteCertificateValidationCallback  ClientCertificateValidator         = null,
                        LocalCertificateSelectionCallback    ClientCertificateSelector          = null,
                        SslProtocols                         AllowedTLSProtocols                = SslProtocols.Tls12,

                        String                               ServiceName                        = DefaultServiceName,
                        EMailAddress                         APIEMailAddress                    = null,
                        PgpPublicKeyRing                     APIPublicKeyRing                   = null,
                        PgpSecretKeyRing                     APISecretKeyRing                   = null,
                        String                               APIPassphrase                      = null,
                        EMailAddressList                     APIAdminEMails                     = null,
                        SMTPClient                           APISMTPClient                      = null,

                        HTTPCookieName?                      CookieName                         = null,
                        Languages                            Language                           = DefaultLanguage,
                        String                               LogoImage                          = null,
                        NewUserSignUpEMailCreatorDelegate    NewUserSignUpEMailCreator          = null,
                        NewUserWelcomeEMailCreatorDelegate   NewUserWelcomeEMailCreator         = null,
                        ResetPasswordEMailCreatorDelegate    ResetPasswordEMailCreator          = null,
                        Byte                                 MinUserNameLenght                  = DefaultMinUserNameLenght,
                        Byte                                 MinRealmLenght                     = DefaultMinRealmLenght,
                        Byte                                 MinPasswordLenght                  = DefaultMinPasswordLenght,
                        TimeSpan?                            SignInSessionLifetime              = null,

                        String                               ServerThreadName                   = null,
                        ThreadPriority                       ServerThreadPriority               = ThreadPriority.AboveNormal,
                        Boolean                              ServerThreadIsBackground           = true,
                        ConnectionIdBuilder                  ConnectionIdBuilder                = null,
                        ConnectionThreadsNameBuilder         ConnectionThreadsNameBuilder       = null,
                        ConnectionThreadsPriorityBuilder     ConnectionThreadsPriorityBuilder   = null,
                        Boolean                              ConnectionThreadsAreBackground     = true,
                        TimeSpan?                            ConnectionTimeout                  = null,
                        UInt32                               MaxClientConnections               = TCPServer.__DefaultMaxClientConnections,

                        Boolean                              SkipURITemplates                   = false,
                        Boolean                              DisableNotifications               = false,
                        Boolean                              DisableLogfile                     = false,
                        String                               LogfileName                        = DefaultLogfileName,
                        DNSClient                            DNSClient                          = null,
                        Boolean                              Autostart                          = false)

            : this(new HTTPServer(TCPPort:                           HTTPServerPort ?? DefaultHTTPServerPort,
                                  DefaultServerName:                 HTTPServerName,

                                  ServerCertificateSelector:         ServerCertificateSelector,
                                  ClientCertificateValidator:        ClientCertificateValidator,
                                  ClientCertificateSelector:         ClientCertificateSelector,
                                  AllowedTLSProtocols:               AllowedTLSProtocols,

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

                   CookieName,
                   Language,
                   LogoImage,
                   NewUserSignUpEMailCreator,
                   NewUserWelcomeEMailCreator,
                   ResetPasswordEMailCreator,
                   MinUserNameLenght,
                   MinRealmLenght,
                   MinPasswordLenght,
                   SignInSessionLifetime,

                   SkipURITemplates,
                   DisableNotifications,
                   DisableLogfile,
                   LogfileName)

        {

            // Everything is done in the other constructor!

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
        /// <param name="CookieName">The name of the HTTP Cookie for authentication.</param>
        /// <param name="Language">The main language of the API.</param>
        /// <param name="LogoImage">The logo of the website.</param>
        /// <param name="NewUserSignUpEMailCreator">A delegate for sending a sign-up e-mail to a new user.</param>
        /// <param name="NewUserWelcomeEMailCreator">A delegate for sending a welcome e-mail to a new user.</param>
        /// <param name="ResetPasswordEMailCreator">A delegate for sending a reset password e-mail to a user.</param>
        /// <param name="MinUserNameLenght">The minimal user name length.</param>
        /// <param name="MinRealmLenght">The minimal realm length.</param>
        /// <param name="MinPasswordLenght">The minimal password length.</param>
        /// <param name="SignInSessionLifetime">The sign-in session lifetime.</param>
        /// 
        /// <param name="SkipURITemplates">Skip URI templates.</param>
        /// <param name="DisableNotifications">Disable external notifications.</param>
        /// <param name="DisableLogfile">Disable the log file.</param>
        /// <param name="LogfileName">The name of the logfile for this API.</param>
        protected UsersAPI(HTTPServer                          HTTPServer,
                           HTTPHostname?                       HTTPHostname                 = null,
                           HTTPURI?                            URIPrefix                    = null,

                           String                              ServiceName                  = DefaultServiceName,
                           EMailAddress                        APIEMailAddress              = null,
                           PgpPublicKeyRing                    APIPublicKeyRing             = null,
                           PgpSecretKeyRing                    APISecretKeyRing             = null,
                           String                              APIPassphrase                = null,
                           EMailAddressList                    APIAdminEMails               = null,
                           SMTPClient                          APISMTPClient                = null,

                           HTTPCookieName?                     CookieName                   = null,
                           Languages                           Language                     = DefaultLanguage,
                           String                              LogoImage                    = null,
                           NewUserSignUpEMailCreatorDelegate   NewUserSignUpEMailCreator    = null,
                           NewUserWelcomeEMailCreatorDelegate  NewUserWelcomeEMailCreator   = null,
                           ResetPasswordEMailCreatorDelegate   ResetPasswordEMailCreator    = null,
                           Byte                                MinUserNameLenght            = DefaultMinUserNameLenght,
                           Byte                                MinRealmLenght               = DefaultMinRealmLenght,
                           Byte                                MinPasswordLenght            = DefaultMinPasswordLenght,
                           TimeSpan?                           SignInSessionLifetime        = null,

                           Boolean                             SkipURITemplates             = false,
                           Boolean                             DisableNotifications         = false,
                           Boolean                             DisableLogfile               = false,
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

            this.HTTPServer                   = HTTPServer;
            this.Hostname                     = HTTPHostname ?? Vanaheimr.Hermod.HTTP.HTTPHostname.Any;
            this.URIPrefix                    = URIPrefix    ?? HTTPURI.Parse("/");

            this.ServiceName                  = ServiceName. IsNotNullOrEmpty() ? ServiceName  : "UsersAPI";
            this.APIEMailAddress              = APIEMailAddress;
            this.APIPublicKeyRing             = APIPublicKeyRing;
            this.APISecretKeyRing             = APISecretKeyRing;
            this.APIPassphrase                = APIPassphrase;
            this.APIAdminEMails               = APIAdminEMails;
            this.APISMTPClient                = APISMTPClient;

            this.CookieName                   = CookieName ?? DefaultCookieName;
            this.Language                     = Language;
            this._LogoImage                   = LogoImage;
            this.NewUserSignUpEMailCreator    = NewUserSignUpEMailCreator;
            this.NewUserWelcomeEMailCreator   = NewUserWelcomeEMailCreator;
            this.ResetPasswordEMailCreator    = ResetPasswordEMailCreator;
            this.MinLoginLenght               = MinUserNameLenght;
            this.MinRealmLenght               = MinRealmLenght;
            this.MinPasswordLenght            = MinPasswordLenght;
            this.SignInSessionLifetime        = SignInSessionLifetime ?? DefaultSignInSessionLifetime;

            this._DataLicenses                = new Dictionary<DataLicense_Id,  DataLicense>();
            this._Users                       = new Dictionary<User_Id,         User>();
            this._Groups                      = new Dictionary<Group_Id,        Group>();
            this._Organizations               = new Dictionary<Organization_Id, Organization>();
            this._Messages                    = new Dictionary<Message_Id,      Message>();

            this._LoginPasswords              = new Dictionary<User_Id,         LoginPassword>();
            this._VerificationTokens          = new List<VerificationToken>();

            this._DNSClient                   = HTTPServer.DNSClient;
            this.SystemId                     = Environment.MachineName.Replace("/", "") + "/" + HTTPServer.DefaultHTTPServerPort;
            this.SecurityTokens               = new Dictionary<SecurityToken_Id, SecurityToken>();

            this.DisableNotifications         = DisableNotifications;
            this.DisableLogfile               = DisableLogfile;
            this.LogfileName                  = LogfileName ?? DefaultLogfileName;

            this._APIKeys                     = new Dictionary<APIKey, APIKeyInfo>();
            this._Notifications               = new Notifications();

            #endregion

            this.Admins  = CreateGroupIfNotExists(Group_Id.Parse(AdminGroupName),
                                                  I18NString.Create(Languages.eng, AdminGroupName),
                                                  I18NString.Create(Languages.eng, "All admins of this API."));

            #region Reflect data licenses

            foreach (var dataLicense in typeof(DataLicense).GetFields(System.Reflection.BindingFlags.Public |
                                                                      System.Reflection.BindingFlags.Static).
                                                            Where (fieldinfo => fieldinfo.ReflectedType == typeof(DataLicense)).
                                                            Select(fieldinfo => fieldinfo.GetValue(DataLicense.None)).
                                                            Cast<DataLicense>())
            {

                _DataLicenses.Add(dataLicense.Id,
                                  dataLicense);

            }

            #endregion

            ReadDatabaseFiles();

            _Notifications.OnAdded   += (Timestamp, User, NotificationId, NotificationType) => WriteToLogfile("AddNotification",    NotificationType.ToJSON(User, NotificationId));
            _Notifications.OnRemoved += (Timestamp, User, NotificationId, NotificationType) => WriteToLogfile("RemoveNotification", NotificationType.ToJSON(User, NotificationId));

            if (!SkipURITemplates)
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
        /// <param name="CookieName">The name of the HTTP Cookie for authentication.</param>
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
        /// <param name="SkipURITemplates">Skip URI templates.</param>
        /// <param name="DisableNotifications">Disable external notifications.</param>
        /// <param name="DisableLogfile">Disable the log file.</param>
        /// <param name="LogfileName">The name of the logfile for this API.</param>
        public static UsersAPI AttachToHTTPAPI(HTTPServer                          HTTPServer,
                                               HTTPHostname?                       HTTPHostname                 = null,
                                               HTTPURI?                            URIPrefix                    = null,

                                               String                              ServiceName                  = DefaultServiceName,
                                               EMailAddress                        APIEMailAddress              = null,
                                               PgpPublicKeyRing                    APIPublicKeyRing             = null,
                                               PgpSecretKeyRing                    APISecretKeyRing             = null,
                                               String                              APIPassphrase                = null,
                                               EMailAddressList                    APIAdminEMails               = null,
                                               SMTPClient                          APISMTPClient                = null,

                                               HTTPCookieName?                     CookieName                   = null,
                                               Languages                           DefaultLanguage              = Languages.eng,
                                               String                              LogoImage                    = null,
                                               NewUserSignUpEMailCreatorDelegate   NewUserSignUpEMailCreator    = null,
                                               NewUserWelcomeEMailCreatorDelegate  NewUserWelcomeEMailCreator   = null,
                                               ResetPasswordEMailCreatorDelegate   ResetPasswordEMailCreator    = null,
                                               Byte                                MinUserNameLenght            = DefaultMinUserNameLenght,
                                               Byte                                MinRealmLenght               = DefaultMinRealmLenght,
                                               Byte                                MinPasswordLenght            = DefaultMinPasswordLenght,
                                               TimeSpan?                           SignInSessionLifetime        = null,

                                               Boolean                             SkipURITemplates             = false,
                                               Boolean                             DisableNotifications         = false,
                                               Boolean                             DisableLogfile               = false,
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

                            CookieName,
                            DefaultLanguage,
                            LogoImage,
                            NewUserSignUpEMailCreator,
                            NewUserWelcomeEMailCreator,
                            ResetPasswordEMailCreator,
                            MinUserNameLenght,
                            MinRealmLenght,
                            MinPasswordLenght,
                            SignInSessionLifetime,

                            SkipURITemplates,
                            DisableNotifications,
                            DisableLogfile,
                            LogfileName);

        #endregion

        #region (private) RegisterURITemplates()

        private void RegisterURITemplates()
        {

            #region /shared/UsersAPI

            HTTPServer.RegisterResourcesFolder(HTTPHostname.Any, URIPrefix + "/shared/UsersAPI", HTTPRoot.Substring(0, HTTPRoot.Length - 1));

            #endregion


            #region GET         ~/signup

            #region HTML_UTF8

            // -------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/signup
            // -------------------------------------------------------------
            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          new HTTPURI[] { URIPrefix + "signup" },
                                          HTTPContentType.HTML_UTF8,
                                          HTTPDelegate: async Request => {

                                              var _MemoryStream1 = new MemoryStream();
                                              GetUsersAPIRessource("template.html").SeekAndCopyTo(_MemoryStream1, 0);
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

                                          }, AllowReplacement: URIReplacement.Allow);


            #endregion

            #endregion

            #region GET         ~/verificationtokens/{VerificationToken}

            #region HTML_UTF8

            // ----------------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/verificationtokens/0vu04w2hgf0w2h4bv08w
            // ----------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         HTTPURI.Parse("/verificationtokens/{VerificationToken}"),
                                         HTTPContentType.HTML_UTF8,
                                         HTTPDelegate: Request => {

                                              VerificationToken VerificationToken = null;

                                              foreach (var _VerificationToken in _VerificationTokens)
                                                  if (_VerificationToken.ToString() == Request.ParsedURIParameters[0])
                                                  {
                                                      VerificationToken = _VerificationToken;
                                                      break;
                                                  }

                                              if (VerificationToken == null)
                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode  = HTTPStatusCode.NotFound,
                                                          Server          = HTTPServer.DefaultServerName,
                                                          ContentType     = HTTPContentType.HTML_UTF8,
                                                          Content         = ("VerificationToken not found!").ToUTF8Bytes(),
                                                          CacheControl    = "public",
                                                          Connection      = "close"
                                                      }.AsImmutable);

                                              if (!_Users.TryGetValue(VerificationToken.Login, out User _User))
                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode  = HTTPStatusCode.NotFound,
                                                          Server          = HTTPServer.DefaultServerName,
                                                          ContentType     = HTTPContentType.HTML_UTF8,
                                                          Content         = ("Login not found!").ToUTF8Bytes(),
                                                          CacheControl    = "public",
                                                          Connection      = "close"
                                                      }.AsImmutable);

                                              _VerificationTokens.Remove(VerificationToken);

                                              lock (_Users)
                                              {

                                                  var UserBuilder = _User.ToBuilder();
                                                  UserBuilder.IsAuthenticated = true;
                                                  var AuthenticatedUser = UserBuilder.Build();

                                                  _Users.Remove(_User.Id);
                                                  _Users.Add(AuthenticatedUser.Id, AuthenticatedUser);

                                              }

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
                                                      From        = APIEMailAddress,
                                                      To          = APIAdminEMails,
                                                      Subject     = "New user activated: " + _User.Id.ToString() + " at " + DateTime.UtcNow.ToString(),
                                                      Text        = "New user activated: " + _User.Id.ToString() + " at " + DateTime.UtcNow.ToString(),
                                                      Passphrase  = APIPassphrase
                                                  };

                                                  var MailResultTask = APISMTPClient.Send(AdminMail).Wait(30000);

                                                  #endregion

                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode  = HTTPStatusCode.Created,
                                                          Server          = HTTPServer.DefaultServerName,
                                                          ContentType     = HTTPContentType.JSON_UTF8,
                                                          Content         = new JObject(
                                                                                new JProperty("@context", ""),
                                                                                new JProperty("@id",   _User.Id.   ToString()),
                                                                                new JProperty("email", _User.EMail.ToString())
                                                                            ).ToString().ToUTF8Bytes(),
                                                          CacheControl    = "public",
                                                          //Expires         = "Mon, 25 Jun 2015 21:31:12 GMT",
                                                          Connection      = "close"
                                                      }.AsImmutable);

                                              }

                                              #endregion

                                              return Task.FromResult(
                                                  new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode  = HTTPStatusCode.OK,
                                                      Server          = HTTPServer.DefaultServerName,
                                                      ContentType     = HTTPContentType.HTML_UTF8,
                                                      Content         = ("Account '" + VerificationToken.Login.ToString() + "' activated!").ToUTF8Bytes(),
                                                      CacheControl    = "public",
                                                      ETag            = "1",
                                                      //Expires         = "Mon, 25 Jun 2015 21:31:12 GMT",
                                                      Connection      = "close"
                                                  }.AsImmutable);

                                          }, AllowReplacement: URIReplacement.Allow);

            #endregion

            #endregion

            #region POST        ~/login

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.POST,
                                         HTTPURI.Parse("/login"),
                                         HTTPContentType.XWWWFormUrlEncoded,
                                         HTTPDelegate: Request => {

                                              //Note: Add LoginRequest event!

                                              #region Check UTF8 text body...

                                              if (!Request.TryParseUTF8StringRequestBody(HTTPContentType.XWWWFormUrlEncoded,
                                                                                         out String       LoginText,
                                                                                         out HTTPResponse _HTTPResponse,
                                                                                         AllowEmptyHTTPBody: false))
                                              {
                                                  return Task.FromResult(_HTTPResponse);
                                              }

                                              #endregion

                                              var LoginData = LoginText.DoubleSplit('&', '=');

                                              #region Verify the login

                                              if (!LoginData.TryGetValue("login", out String Login) ||
                                                   Login.    IsNullOrEmpty())
                                              {

                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                          Server          = HTTPServer.DefaultServerName,
                                                          ContentType     = HTTPContentType.JSON_UTF8,
                                                          Content         = new JObject(
                                                                                new JProperty("@context",     SignInOutContext),
                                                                                new JProperty("statuscode",   400),
                                                                                new JProperty("property",     "login"),
                                                                                new JProperty("description",  "The login must not be empty!")
                                                                            ).ToString().ToUTF8Bytes(),
                                                          CacheControl     = "private",
                                                          Connection       = "close"
                                                      }.AsImmutable);

                                              }

                                              Login = HTTPTools.URLDecode(Login);

                                              if (Login.Length < MinLoginLenght)
                                              {

                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                          Server          = HTTPServer.DefaultServerName,
                                                          ContentType     = HTTPContentType.JSON_UTF8,
                                                          Content         = new JObject(
                                                                                new JProperty("@context",     SignInOutContext),
                                                                                new JProperty("statuscode",   400),
                                                                                new JProperty("property",     "login"),
                                                                                new JProperty("description",  "The login is too short!")
                                                                            ).ToString().ToUTF8Bytes(),
                                                          CacheControl    = "private",
                                                          Connection      = "close"
                                                      }.AsImmutable);

                                              }

                                              #endregion

                                              #region Verify the realm

                                              LoginData.TryGetValue("realm", out String Realm);

                                              if (Realm.IsNotNullOrEmpty())
                                                  Realm = HTTPTools.URLDecode(Realm);

                                              #endregion

                                              #region Verify the password

                                              if (!LoginData.TryGetValue("password", out String Password) ||
                                                   Password. IsNullOrEmpty())
                                              {

                                                 return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                          Server          = HTTPServer.DefaultServerName,
                                                          ContentType     = HTTPContentType.JSON_UTF8,
                                                          Content         = new JObject(
                                                                                new JProperty("@context",     SignInOutContext),
                                                                                new JProperty("statuscode",   400),
                                                                                new JProperty("property",     "password"),
                                                                                new JProperty("description",  "The password must not be empty!")
                                                                           ).ToString().ToUTF8Bytes(),
                                                          CacheControl    = "private",
                                                          Connection      = "close"
                                                      }.AsImmutable);

                                              }

                                              Password = HTTPTools.URLDecode(Password);

                                              if (Password.Length < MinPasswordLenght)
                                              {

                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                          Server          = HTTPServer.DefaultServerName,
                                                          ContentType     = HTTPContentType.JSON_UTF8,
                                                          Content         = new JObject(
                                                                                new JProperty("@context",     SignInOutContext),
                                                                                new JProperty("statuscode",   400),
                                                                                new JProperty("property",     "password"),
                                                                                new JProperty("description",  "The password is too short!")
                                                                           ).ToString().ToUTF8Bytes(),
                                                          CacheControl    = "private",
                                                          Connection      = "close"
                                                      }.AsImmutable);

                                              }

                                              #endregion

                                              #region Get RedirectURI

                                              LoginData.TryGetValue("RedirectURI", out String RedirectURI);

                                              if (RedirectURI.IsNotNullOrEmpty())
                                                 RedirectURI = HTTPTools.URLDecode(RedirectURI);

                                              else
                                                 RedirectURI = "/";

                                              #endregion

                                              #region Check login and password

                                              if (!(Realm.IsNotNullOrEmpty()
                                                        ? User_Id.TryParse(Login, Realm, out User_Id       _UserId)
                                                        : User_Id.TryParse(Login,        out               _UserId)) ||
                                                  !_LoginPasswords.TryGetValue(_UserId,  out LoginPassword _LoginPassword) ||
                                                  !_Users.         TryGetValue(_UserId,  out User          _User))
                                              {

                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode  = HTTPStatusCode.NotFound,
                                                          Server          = HTTPServer.DefaultServerName,
                                                          ContentType     = HTTPContentType.JSON_UTF8,
                                                          Content         = new JObject(
                                                                                new JProperty("@context",     SignInOutContext),
                                                                                new JProperty("property",     "login"),
                                                                                new JProperty("description",  "Unknown login!")
                                                                            ).ToString().ToUTF8Bytes(),
                                                          CacheControl    = "private",
                                                          Connection      = "close"
                                                      }.AsImmutable);

                                              }

                                              if (!_LoginPassword.VerifyPassword(Password))
                                              {

                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode  = HTTPStatusCode.Unauthorized,
                                                          Server          = HTTPServer.DefaultServerName,
                                                          ContentType     = HTTPContentType.JSON_UTF8,
                                                          Content         = new JObject(
                                                                                new JProperty("@context",     SignInOutContext),
                                                                                new JProperty("property",     "password"),
                                                                                new JProperty("description",  "Invalid password!")
                                                                            ).ToString().ToUTF8Bytes(),
                                                          CacheControl    = "private",
                                                          Connection      = "close"
                                                      }.AsImmutable);

                                              }

                                              #endregion


                                              #region Register security token

                                              var SHA256Hash     = new SHA256Managed();
                                              var SecurityToken  = SecurityToken_Id.Parse(SHA256Hash.ComputeHash(
                                                                                              String.Concat(Guid.NewGuid().ToString(),
                                                                                                            _LoginPassword.Login).
                                                                                              ToUTF8Bytes()
                                                                                          ).ToHexString());

                                              var Expires        = DateTime.UtcNow.Add(SignInSessionLifetime);

                                              lock (SecurityTokens)
                                              {

                                                  SecurityTokens.Add(SecurityToken,
                                                                     new SecurityToken(_LoginPassword.Login,
                                                                                             Expires));

                                                  File.AppendAllText(DefaultSecurityTokenFile,
                                                                     SecurityToken + ";" + _LoginPassword.Login + ";" + Expires.ToIso8601() + Environment.NewLine);

                                              }

                                              #endregion


                                              //Note: Add LoginResponse event!

                                              return Task.FromResult(
                                                  new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode  = HTTPStatusCode.Created,
                                                      ContentType     = HTTPContentType.HTML_UTF8,
                                                      Content         = String.Concat(
                                                                            "<!DOCTYPE html>", Environment.NewLine,
                                                                            @"<html><head><meta http-equiv=""refresh"" content=""0; url=" + RedirectURI + @""" /></head></html>",
                                                                            Environment.NewLine
                                                                        ).ToUTF8Bytes(),
                                                      CacheControl    = "private",
                                                      SetCookie       = CookieName + "=login="    + _LoginPassword.Login.ToString().ToBase64() +
                                                                                  ":username=" + _User.Name.ToBase64() +
                                                                                (IsAdmin(_User) ? ":isAdmin" : "") +
                                                                             ":securitytoken=" + SecurityToken +
                                                                                  "; Expires=" + Expires.ToRfc1123() +
                                                                                   (HTTPCookieDomain.IsNotNullOrEmpty()
                                                                                       ? "; Domain=" + HTTPCookieDomain
                                                                                       : "") +
                                                                                     "; Path=/",
                                                      // _gitlab_session=653i45j69051238907520q1350275575; path=/; secure; HttpOnly
                                                      Connection      = "close",
                                                      X_FrameOptions  = "DENY"
                                                  }.AsImmutable);

                                          });

            #endregion

            #region GET         ~/lostpassword

            #region HTML_UTF8

            // -------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/lostpassword
            // -------------------------------------------------------------------
            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          new HTTPURI[] { URIPrefix + "lostpassword" },
                                          HTTPContentType.HTML_UTF8,
                                          HTTPDelegate: async Request => {

                                              var _MemoryStream1 = new MemoryStream();
                                              GetUsersAPIRessource("template.html").SeekAndCopyTo(_MemoryStream1, 0);
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

                                          }, AllowReplacement: URIReplacement.Allow);


            #endregion

            #endregion


            #region GET         ~/users

            // -------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/users
            // -------------------------------------------------------------------
            HTTPServer.ITEMS_GET(UriTemplate: URIPrefix + "/users",
                                 Dictionary: _Users,
                                 Filter: user => user.PrivacyLevel == PrivacyLevel.World,
                                 ToJSONDelegate: JSON_IO.ToJSON);

            #endregion

            #region DEAUTH      ~/users

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.DEAUTH,
                                          HTTPURI.Parse("/users"),
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: Request =>

                                              Task.FromResult(
                                                  new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode  = HTTPStatusCode.OK,
                                                      CacheControl    = "private",
                                                      SetCookie       = CookieName + "=; Expires=" + DateTime.UtcNow.ToRfc1123() +
                                                                            (HTTPCookieDomain.IsNotNullOrEmpty()
                                                                                ? "; Domain=" + HTTPCookieDomain
                                                                                : "") +
                                                                            "; Path=/",
                                                      Connection      = "close"
                                                  }.AsImmutable));

            #endregion

            #region ADD         ~/users/{UserId}

            // -------------------------------------------------------------------------------
            // curl -v -X ADD -H "Accept: application/json" http://127.0.0.1:2100/users/ahzf
            // -------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.ADD,
                                          HTTPURI.Parse("/users/{UserId}"),
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: async Request => {

                                              #region Check JSON body...

                                              if (!Request.TryParseJObjectRequestBody(out JObject NewUserData, out HTTPResponse _HTTPResponse))
                                                  return _HTTPResponse;

                                              if (!NewUserData.HasValues)
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

                                              User_Id _Login;

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

                                              if (_Login.Length < MinLoginLenght)
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

                                              if (NewUserData["email"] == null)
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
                                              var matches = Regex.Match(NewUserData.GetOptional("email").Trim(), @"([^\@]+)\@([^\@]+\.[^\@]{2,})");

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

                                              if (NewUserData["GPGPublicKeyRing"] != null &&
                                                  !OpenPGP.TryReadPublicKeyRing(NewUserData["GPGPublicKeyRing"].Value<String>(), out PublicKeyRing))
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

                                              if (!NewUserData.Contains("password"))
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

                                              var NewUser = CreateUser(Id:             _Login,
                                                                       Password:       Password.Parse(NewUserData.GetString("password")),
                                                                       EMail:          SimpleEMailAddress.Parse(NewUserData.GetString("email")),
                                                                       PublicKeyRing:  NewUserData.GetString("gpgpublickeyring"));

                                              var VerificationToken = _VerificationTokens.AddAndReturnElement(new VerificationToken(Seed: _Login.ToString() + NewUserData.GetString("password") + NewUserData.GetString("email"),
                                                                                                              UserId: _Login));

                                              #endregion

                                              #region Send New-User-Verification-E-Mail

                                              var MailSentResult = MailSentStatus.failed;

                                              var NewUserSignUpEMailCreatorLocal = NewUserSignUpEMailCreator;
                                              if (NewUserSignUpEMailCreatorLocal != null)
                                              {

                                                  var NewUserMail = NewUserSignUpEMailCreatorLocal(Login:              _Login,
                                                                                                   EMail:              new EMailAddress(OwnerName:                 _Login.ToString(),
                                                                                                                                        SimpleEMailAddressString:  matches.Groups[0].Value,
                                                                                                                                        SecretKeyRing:             null,
                                                                                                                                        PublicKeyRing:             PublicKeyRing),
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
                                                      From        = APIEMailAddress,
                                                      To          = APIAdminEMails,
                                                      Subject     = "New user registered: " + _Login + " <" + matches.Groups[0].Value + "> at " + DateTime.UtcNow.ToString(),
                                                      Text        = "New user registered: " + _Login + " <" + matches.Groups[0].Value + "> at " + DateTime.UtcNow.ToString(),
                                                      Passphrase  = APIPassphrase
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

            #region EXISTS      ~/users/{UserId}

            // ---------------------------------------------------------------------------------
            // curl -v -X EXITS -H "Accept: application/json" http://127.0.0.1:2100/users/ahzf
            // ---------------------------------------------------------------------------------
            HTTPServer.ITEM_EXISTS<User_Id, User>(UriTemplate: URIPrefix + "/users/{UserId}",
                                                  ParseIdDelegate: User_Id.TryParse,
                                                  ParseIdError: Text => "Invalid user identification '" + Text + "'!",
                                                  TryGetItemDelegate: _Users.TryGetValue,
                                                  ItemFilterDelegate: user => user.PrivacyLevel == PrivacyLevel.World,
                                                  TryGetItemError: userId => "Unknown user '" + userId + "'!");

            #endregion

            #region GET         ~/users/{UserId}

            // ------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/users/ahzf
            // ------------------------------------------------------------------------
            HTTPServer.ITEM_GET<User_Id, User>(UriTemplate:         URIPrefix + "/users/{UserId}",
                                               ParseIdDelegate:     User_Id.TryParse,
                                               ParseIdError:        Text => "Invalid user identification '" + Text + "'!",
                                               TryGetItemDelegate:  _Users.TryGetValue,
                                               ItemFilterDelegate:  user   => user.PrivacyLevel == PrivacyLevel.World,
                                               TryGetItemError:     userId => "Unknown user '" + userId + "'!",
                                               ToJSONDelegate:      user   => user.ToJSON(IncludeCryptoHash: true));

            //HTTPServer.AddMethodCallback(Hostname,
            //                             HTTPMethod.GET,
            //                             URIPrefix + "users/{UserId}",
            //                             HTTPContentType.JSON_UTF8,
            //                             HTTPDelegate: Request => {

            //                                 #region Get HTTP user and its organizations

            //                                 // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
            //                                 TryGetHTTPUser(Request,
            //                                                out User                       HTTPUser,
            //                                                out IEnumerable<Organization>  HTTPOrganizations,
            //                                                Recursive: true);

            //                                 #endregion

            //                                 #region Check DefibrillatorId URI parameter

            //                                 if (!Request.ParseUser(this,
            //                                                        out User_Id?      DefibrillatorId,
            //                                                        out User          Defibrillator,
            //                                                        out HTTPResponse  HTTPResponse))
            //                                 {
            //                                     return Task.FromResult(HTTPResponse);
            //                                 }

            //                                 #endregion

            //                                 if (Organizations.Contains(Defibrillator.Owner) ||
            //                                     Admins.InEdges(HTTPUser).Any(edgelabel => edgelabel == User2GroupEdges.IsAdmin))
            //                                 {

            //                                     return Task.FromResult(
            //                                         new HTTPResponseBuilder(Request) {
            //                                                          HTTPStatusCode             = HTTPStatusCode.OK,
            //                                                          Server                     = HTTPServer.DefaultServerName,
            //                                                          Date                       = DateTime.UtcNow,
            //                                                          AccessControlAllowOrigin   = "*",
            //                                                          AccessControlAllowMethods  = "GET, SET",
            //                                                          AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
            //                                                          ETag                       = "1",
            //                                                          ContentType                = HTTPContentType.HTML_UTF8,
            //                                                          Content                    = Template.Replace("<%= content %>",   _MemoryStream2.ToArray().ToUTF8String()).
            //                                                                                                Replace("<%= logoimage %>", String.Concat(@"<img src=""", LogoImage, @""" /> ")).
            //                                                                                                ToUTF8Bytes(),
            //                                                          Connection                 = "close"
            //                                                      }.AsImmutable);


            //                                 }

            //                                 else return Task.FromResult(
            //                                                  new HTTPResponseBuilder(Request) {
            //                                                            HTTPStatusCode             = HTTPStatusCode.Unauthorized,
            //                                                            Server                     = HTTPServer.DefaultServerName,
            //                                                            Date                       = DateTime.UtcNow,
            //                                                            AccessControlAllowOrigin   = "*",
            //                                                            AccessControlAllowMethods  = "GET, SET",
            //                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
            //                                                            Connection                 = "close"
            //                                                        }.AsImmutable);

            //                             });


            #region Get HTTP user and its organizations

            // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
            //if (!TryGetHTTPUser(Request,
            //                    out User                       HTTPUser,
            //                    out IEnumerable<Organization>  HTTPOrganizations,
            //                    out HTTPResponse               Response,
            //                    Recursive: true))
            //{
            //    return Task.FromResult(Response);
            //}

            #endregion


            #endregion

            #region SET         ~/users/{UserId}

            // ---------------------------------------------------------------------------------------------
            // curl -v -X SET \
            //      -H "Accept:       application/json; charset=utf-8" \
            //      -H "Content-Type: application/json; charset=utf-8" \
            //      -d "{ \
            //              \"@id\" :             \"214080158\", \
            //              \"@context\" :        \"https://cardi-link.cloud/contexts/cardidb+json/user\", \
            //              \"description\" :     { \"deu\" : \"Test AED in Erlangen Raum Yavin 4\" },\
            //              \"dataLicenseIds\" :  [ \"ODbL\" ],\
            //              \"ownerId\" :         \"CardiLink\", \
            //              \"address\" :         { \
            //                                      \"country\" :      \"Germany\",
            //                                      \"postalCode\" :   \"91052\",
            //                                      \"city\" :         { \"deu\": \"Erlangen\" },
            //                                      \"street\" :       \"Henkestraße\",
            //                                      \"houseNumber\" :  \"91\",
            //                                      \"floorLevel\" :   \"1\"
            //                                    }, \
            //              \"geoLocation\" :     { \"lat\": 49.594760, \"lng\": 11.019356 }, \
            //              \"privacyLevel\" :    \"Public\" \
            //          }" \
            //      http://127.0.0.1:2000/users/214080158
            // ---------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.SET,
                                         URIPrefix + "/users/{UserId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             SetUserRequest(Request);

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out User                       HTTPUser,
                                                                 out IEnumerable<Organization>  HTTPOrganizations,
                                                                 out HTTPResponse               Response,
                                                                 RequireReadWriteAccess:        true,
                                                                 Recursive:                     true))
                                             {
                                                 return SetUserResponse(Response);
                                             }

                                             #endregion

                                             #region Check UserId URI parameter

                                             if (!Request.ParseUserId(this,
                                                                      out User_Id?      UserIdURI,
                                                                      out HTTPResponse  HTTPResponse))
                                             {
                                                 return SetUserResponse(HTTPResponse);
                                             }

                                             #endregion

                                             #region Parse JSON

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out HTTPResponse))
                                                 return SetUserResponse(HTTPResponse);

                                             if (!User.TryParseJSON(JSONObj,
                                                                    out User    _User,
                                                                    out String  ErrorResponse,
                                                                    UserIdURI))
                                             {

                                                 return SetUserResponse(
                                                            new HTTPResponseBuilder(Request) {
                                                                HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                                Server                     = HTTPServer.DefaultServerName,
                                                                Date                       = DateTime.UtcNow,
                                                                AccessControlAllowOrigin   = "*",
                                                                AccessControlAllowMethods  = "GET, SET",
                                                                AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                ETag                       = "1",
                                                                ContentType                = HTTPContentType.JSON_UTF8,
                                                                Content                    = JSONObject.Create(
                                                                                                 new JProperty("description",  ErrorResponse)
                                                                                             ).ToUTF8Bytes()
                                                            }.AsImmutable);

                                             }

                                             #endregion


                                             // Has the current HTTP user the required
                                             // access rights to update?
                                             if (HTTPUser.Id != _User.Id)
                                                 return SetUserResponse(
                                                        new HTTPResponseBuilder(Request) {
                                                            HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                                            Server                      = HTTPServer.DefaultServerName,
                                                            Date                        = DateTime.UtcNow,
                                                            AccessControlAllowOrigin    = "*",
                                                            AccessControlAllowMethods   = "GET, SET, CHOWN",
                                                            AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                            Connection                  = "close"
                                                        }.AsImmutable);


                                             //AddOrUpdate(_User);


                                             return SetUserResponse(
                                                        new HTTPResponseBuilder(Request) {
                                                            HTTPStatusCode              = HTTPStatusCode.OK,
                                                            Server                      = HTTPServer.DefaultServerName,
                                                            Date                        = DateTime.UtcNow,
                                                            AccessControlAllowOrigin    = "*",
                                                            AccessControlAllowMethods   = "GET, SET",
                                                            AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                            ETag                        = _User.CurrentCryptoHash,
                                                            ContentType                 = HTTPContentType.JSON_UTF8,
                                                            Content                     = _User.ToJSON().ToUTF8Bytes(),
                                                            Connection                  = "close"
                                                        }.AsImmutable);

                                         });

            #endregion

            #region AUTH        ~/users/{UserId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.AUTH,
                                          HTTPURI.Parse("/users/{UserId}"),
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: Request => {

                                              #region Check JSON body...

                                              //var Body = HTTPRequest.ParseJSONRequestBody();
                                              //if (Body.HasErrors)
                                              //    return Body.Error;

                                              if (!Request.TryParseJObjectRequestBody(out JObject LoginData, out HTTPResponse _HTTPResponse))
                                                  return Task.FromResult(_HTTPResponse);


                                              if (!LoginData.HasValues)
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
                                                          CacheControl    = "private",
                                                          Connection      = "close"
                                                      }.AsImmutable);

                                              #endregion

                                              // The login is taken from the URI, not from the JSON!
                                              LoginData["username"] = Request.ParsedURIParameters[0];

                                              #region Verify username

                                              if (LoginData.GetString("username").Length < MinLoginLenght)
                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                          Server          = HTTPServer.DefaultServerName,
                                                          ContentType     = HTTPContentType.JSON_UTF8,
                                                          Content         = new JObject(
                                                                                new JProperty("@context",     SignInOutContext),
                                                                                new JProperty("statuscode",   400),
                                                                                new JProperty("property",     "username"),
                                                                                new JProperty("description",  "The login is too short!")
                                                                            ).ToString().ToUTF8Bytes(),
                                                          CacheControl    = "private",
                                                          Connection      = "close"
                                                      }.AsImmutable);

                                              #endregion

                                              #region Verify realm

                                              if (LoginData.Contains("realm") &&
                                                  LoginData.GetString("realm").IsNotNullOrEmpty() &&
                                                  LoginData.GetString("realm").Length < MinRealmLenght)
                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                          Server          = HTTPServer.DefaultServerName,
                                                          ContentType     = HTTPContentType.JSON_UTF8,
                                                          Content         = new JObject(
                                                                                new JProperty("@context",     SignInOutContext),
                                                                                new JProperty("statuscode",   400),
                                                                                new JProperty("property",     "realm"),
                                                                                new JProperty("description",  "The realm is too short!")
                                                                            ).ToString().ToUTF8Bytes(),
                                                          CacheControl    = "private",
                                                          Connection      = "close"
                                                      }.AsImmutable);

                                              #endregion

                                              #region Verify password

                                              if (!LoginData.Contains("password"))
                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                          Server          = HTTPServer.DefaultServerName,
                                                          ContentType     = HTTPContentType.JSON_UTF8,
                                                          Content         = new JObject(
                                                                                new JProperty("@context",     SignInOutContext),
                                                                                new JProperty("statuscode",   400),
                                                                                new JProperty("property",     "password"),
                                                                                new JProperty("description",  "Missing \"password\" property!")
                                                                           ).ToString().ToUTF8Bytes(),
                                                          CacheControl    = "private",
                                                          Connection      = "close"
                                                      }.AsImmutable);

                                              if (LoginData.GetString("password").Length < MinPasswordLenght)
                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                          Server          = HTTPServer.DefaultServerName,
                                                          ContentType     = HTTPContentType.JSON_UTF8,
                                                          Content         = new JObject(
                                                                                new JProperty("@context",     SignInOutContext),
                                                                                new JProperty("statuscode",   400),
                                                                                new JProperty("property",     "password"),
                                                                                new JProperty("description",  "The password is too short!")
                                                                           ).ToString().ToUTF8Bytes(),
                                                          CacheControl    = "private",
                                                          Connection      = "close"
                                                      }.AsImmutable);

                                              #endregion

                                              #region Check login and password

                                              String _Realm = LoginData.GetString("realm");

                                              if (!(_Realm.IsNotNullOrEmpty()
                                                     ? User_Id.TryParse(LoginData.GetString("username"), _Realm, out User_Id _UserId)
                                                     : User_Id.TryParse(LoginData.GetString("username"),         out         _UserId)) ||
                                                  !_LoginPasswords.TryGetValue(_UserId, out LoginPassword _LoginPassword)              ||
                                                  !_Users.         TryGetValue(_UserId, out User          _User))
                                              {

                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode  = HTTPStatusCode.NotFound,
                                                          Server          = HTTPServer.DefaultServerName,
                                                          ContentType     = HTTPContentType.JSON_UTF8,
                                                          Content         = new JObject(
                                                                                new JProperty("@context",     SignInOutContext),
                                                                                new JProperty("property",     "username"),
                                                                                new JProperty("description",  "Unknown user!")
                                                                            ).ToString().ToUTF8Bytes(),
                                                          CacheControl    = "private",
                                                          Connection      = "close"
                                                      }.AsImmutable);

                                              }

                                              if (!_LoginPassword.VerifyPassword(LoginData.GetString("password")))
                                              {

                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode  = HTTPStatusCode.Unauthorized,
                                                          Server          = HTTPServer.DefaultServerName,
                                                          ContentType     = HTTPContentType.JSON_UTF8,
                                                          Content         = new JObject(
                                                                                new JProperty("@context",     SignInOutContext),
                                                                                new JProperty("property",     "username"),
                                                                                new JProperty("description",  "Invalid username or password!")
                                                                            ).ToString().ToUTF8Bytes(),
                                                          CacheControl    = "private",
                                                          Connection      = "close"
                                                      }.AsImmutable);

                                              }

                                              #endregion


                                              var SHA256Hash    = new SHA256Managed();
                                              var SecurityToken = SHA256Hash.ComputeHash((Guid.NewGuid().ToString() + _LoginPassword.Login).ToUTF8Bytes()).ToHexString();

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
                                                      SetCookie       = CookieName + "=login="    + _LoginPassword.Login.ToString().ToBase64() +
                                                                                  ":username=" + _User.Name.ToBase64() +
                                                                                (IsAdmin(_User) ? ":isAdmin" : "") +
                                                                             ":securitytoken=" + SecurityToken +
                                                                                  "; Expires=" + DateTime.UtcNow.Add(SignInSessionLifetime).ToRfc1123() +
                                                                                   (HTTPCookieDomain.IsNotNullOrEmpty()
                                                                                       ? "; Domain=" + HTTPCookieDomain
                                                                                       : "") +
                                                                                     "; Path=/",
                                                      // secure;"
                                                      Connection = "close"
                                                  }.AsImmutable);

                                          });

            #endregion

            #region DEAUTH      ~/users/{UserId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.DEAUTH,
                                          HTTPURI.Parse("/users/{UserId}"),
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: Request =>

                                              Task.FromResult(
                                                  new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode  = HTTPStatusCode.OK,
                                                      CacheControl    = "private",
                                                      SetCookie       = CookieName + "=; Expires=" + DateTime.UtcNow.ToRfc1123() +
                                                                            (HTTPCookieDomain.IsNotNullOrEmpty()
                                                                                ? "; Domain=" + HTTPCookieDomain
                                                                                : "") +
                                                                            "; Path=/",
                                                      Connection      = "close"
                                                  }.AsImmutable));

            #endregion

            #region GET         ~/users/{UserId}/profilephoto

            HTTPServer.RegisterFilesystemFile(HTTPHostname.Any,
                                              URIPrefix + "/users/{UserId}/profilephoto",
                                              URIParams => "LocalHTTPRoot/data/Users/" + URIParams[0] + ".png",
                                              DefaultFile: "HTTPRoot/images/defaults/DefaultUser.png");

            #endregion

            #region GET         ~/users/{UserId}/notifications

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/users/ahzf/notifications
            // --------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.GET,
                                         URIPrefix + "/users/{UserId}/notifications",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out User                       HTTPUser,
                                                                 out IEnumerable<Organization>  HTTPOrganizations,
                                                                 out HTTPResponse               Response,
                                                                 Recursive: true))
                                             {
                                                 return Task.FromResult(Response);
                                             }

                                             #endregion

                                             var __Notifications = _Notifications.GetNotifications(HTTPUser);
                                             //var __Types         = __Notifications.NotificationTypes;


                                             return Task.FromResult(__Notifications != null

                                                                        ? new HTTPResponseBuilder(Request) {
                                                                                  HTTPStatusCode             = HTTPStatusCode.OK,
                                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                                  Date                       = DateTime.UtcNow,
                                                                                  AccessControlAllowOrigin   = "*",
                                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                                  AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                                  ETag                       = "1",
                                                                                  ContentType                = HTTPContentType.JSON_UTF8,
                                                                                  Content                    = JSONObject.Create(

                                                                                                                   new JProperty("general",  JSONArray.Create(
                                                                                                                       __Notifications.NotificationTypes.Select(_ => _.GetAsJSON(new JObject())).ToArray()
                                                                                                                   )),

                                                                                                                   new JProperty("messages", JSONArray.Create(

                                                                                                                       __Notifications.NotificationIds.Select(_ => new JObject(
                                                                                                                                                                       new JProperty("message",        _.Key.ToString()),
                                                                                                                                                                       new JProperty("notifications",  new JArray(
                                                                                                                                                                           _.Value.Select(__ => __.GetAsJSON(new JObject()))
                                                                                                                                                                       ))
                                                                                                                                                                   )).ToArray()

                                                                                                                   ))

                                                                                                               ).ToUTF8Bytes(),
                                                                                  Connection                 = "close"
                                                                              }.AsImmutable

                                                                        : new HTTPResponseBuilder(Request) {
                                                                                  HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                                  Date                       = DateTime.UtcNow,
                                                                                  AccessControlAllowOrigin   = "*",
                                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                                  AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                                  Connection                 = "close"
                                                                              }.AsImmutable);



            });

            #endregion


            #region GET         ~/groups

            // ------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/groups
            // ------------------------------------------------------------------
            HTTPServer.ITEMS_GET(UriTemplate: URIPrefix + "/groups",
                                 Dictionary: _Groups,
                                 Filter: group => group.PrivacyLevel == PrivacyLevel.World,
                                 ToJSONDelegate: JSON_IO.ToJSON);

            #endregion

            #region EXISTS      ~/groups/{GroupId}

            // -------------------------------------------------------------------------------------------
            // curl -v -X EXITS -H "Accept: application/json" http://127.0.0.1:2100/groups/OK-Lab%20Jena
            // -------------------------------------------------------------------------------------------
            HTTPServer.ITEM_EXISTS<Group_Id, Group>(UriTemplate: URIPrefix + "/groups/{GroupId}",
                                                    ParseIdDelegate: Group_Id.TryParse,
                                                    ParseIdError: Text => "Invalid group identification '" + Text + "'!",
                                                    TryGetItemDelegate: _Groups.TryGetValue,
                                                    ItemFilterDelegate: group => group.PrivacyLevel == PrivacyLevel.World,
                                                    TryGetItemError: groupId => "Unknown group '" + groupId + "'!");

            #endregion

            #region GET         ~/groups/{GroupId}

            // ----------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/groups/OK-Lab%20Jena
            // ----------------------------------------------------------------------------------
            HTTPServer.ITEM_GET<Group_Id, Group>(UriTemplate:         URIPrefix + "/groups/{GroupId}",
                                                 ParseIdDelegate:     Group_Id.TryParse,
                                                 ParseIdError:        Text => "Invalid group identification '" + Text + "'!",
                                                 TryGetItemDelegate:  _Groups.TryGetValue,
                                                 ItemFilterDelegate:  group => group.PrivacyLevel == PrivacyLevel.World,
                                                 TryGetItemError:     groupId => "Unknown group '" + groupId + "'!",
                                                 ToJSONDelegate:      _ => _.ToJSON());

            #endregion


            #region GET         ~/orgs

            // ------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/orgs
            // ------------------------------------------------------------------
            HTTPServer.ITEMS_GET(UriTemplate:     URIPrefix + "/orgs",
                                 Dictionary:      _Organizations,
                                 Filter:          org => org.PrivacyLevel == PrivacyLevel.World,
                                 ToJSONDelegate:  JSON_IO.ToJSON);

            #endregion

            #region EXISTS      ~/orgs/{OrgId}

            // ------------------------------------------------------------------------------------
            // curl -v -X EXITS -H "Accept: application/json" http://127.0.0.1:2100/orgs/Stadtrat
            // ------------------------------------------------------------------------------------
            HTTPServer.ITEM_EXISTS<Organization_Id, Organization>(UriTemplate:               URIPrefix + "/orgs/{OrgId}",
                                                                  ParseIdDelegate:           Organization_Id.TryParse,
                                                                  ParseIdError:              Text  => "Invalid organization identification '" + Text + "'!",
                                                                  TryGetItemDelegate:        _Organizations.TryGetValue,
                                                                  ItemFilterDelegate:        org   => org.PrivacyLevel == PrivacyLevel.World,
                                                                  TryGetItemError:           orgId => "Unknown organization '" + orgId + "'!");

            #endregion

            #region GET         ~/orgs/{OrgId}

            // ---------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/orgs/Stadtrat
            // ---------------------------------------------------------------------------
            HTTPServer.ITEM_GET<Organization_Id, Organization>(UriTemplate:          URIPrefix + "/orgs/{OrgId}",
                                                               ParseIdDelegate:     Organization_Id.TryParse,
                                                               ParseIdError:        Text  => "Invalid organization identification '" + Text + "'!",
                                                               TryGetItemDelegate:  _Organizations.TryGetValue,
                                                               ItemFilterDelegate:  org   => org.PrivacyLevel == PrivacyLevel.World,
                                                               TryGetItemError:     orgId => "Unknown organization '" + orgId + "'!",
                                                               ToJSONDelegate:      _ => _.ToJSON());

            #endregion

        }

        #endregion

        #region (private) ReadDatabaseFiles()

        private void ReadDatabaseFiles()
        {

            #region Read DefaultUsersAPIFile

            try
            {

                if (File.Exists(DefaultUsersAPIFile))
                {

                    JObject JSONLine;
                    String  JSONCommand;
                    JObject JSONObject;

                    File.ReadLines(DefaultUsersAPIFile).ForEachCounted((line, linenumber) => {

                        if (line.IsNeitherNullNorEmpty() &&
                           !line.StartsWith("#")         &&
                           !line.StartsWith("//"))
                        {

                            try
                            {

                                JSONLine                  = JObject.Parse(line);
                                JSONCommand               = (JSONLine.First as JProperty)?.Name;
                                JSONObject                = (JSONLine.First as JProperty)?.Value as JObject;
                                CurrentDatabaseHashValue  =  JSONLine["HashValue"]?.Value<String>();

                                if (JSONCommand.IsNotNullOrEmpty() && JSONObject != null)
                                    ProcessCommand(JSONCommand, JSONObject);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(@"Could not read database file """ + DefaultUsersAPIFile + @""" line " + linenumber + ": " + e.Message);
                            }

                        }

                    });

                }

            }
            catch (Exception e)
            {
                DebugX.LogT("ReadStoredData() failed: " + e.Message);
            }

            #endregion

            #region Read Password file...

            if (File.Exists(DefaultPasswordFile))
            {

                String[] elements;

                File.ReadLines(DefaultPasswordFile).ForEachCounted((line, linenumber) => {

                    try
                    {

                        elements = line.Split(new Char[] { ':' }, StringSplitOptions.None);

                        if (elements.Length == 3)
                        {

                            var Login = User_Id.Parse(elements[0]);

                            if (elements[1].IsNotNullOrEmpty() &&
                                elements[2].IsNotNullOrEmpty())
                            {

                                if (!_LoginPasswords.ContainsKey(Login))
                                    _LoginPasswords.Add(Login,
                                                        new LoginPassword(Login,
                                                                          Password.ParseHash(elements[1],
                                                                                             elements[2])));

                                else
                                    _LoginPasswords[Login] = new LoginPassword(Login,
                                                                               Password.ParseHash(elements[1],
                                                                                                  elements[2]));

                            }

                        }

                        else
                            DebugX.Log(@"Could not read password file """ + DefaultPasswordFile + @""" line " + linenumber);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(@"Could not read password file """ + DefaultPasswordFile + @""" line " + linenumber + ": " + e.Message);
                    }

                });

            }

            #endregion

            #region Read SecurityToken file...

            if (File.Exists(DefaultSecurityTokenFile))
            {

                lock (SecurityTokens)
                {

                    try
                    {

                        File.ReadLines(DefaultSecurityTokenFile).ForEachCounted((line, linenumber) => {

                            try
                            {

                                var Tokens         = line.Split(new Char[] { ';' }, StringSplitOptions.None);

                                var SecurityToken  = SecurityToken_Id.Parse(Tokens[0]);
                                var Login          = User_Id.         Parse(Tokens[1]);
                                var Expires        = DateTime.        Parse(Tokens[2]);

                                if (!SecurityTokens.ContainsKey(SecurityToken) &&
                                    _LoginPasswords.ContainsKey(Login) &&
                                    Expires > DateTime.UtcNow)
                                {
                                    SecurityTokens.Add(SecurityToken,
                                                       new SecurityToken(Login, Expires));
                                }

                            }
                            catch (Exception e)
                            {
                                DebugX.Log(@"Could not read security token file """ + DefaultSecurityTokenFile + @""" line " + linenumber + ": " + e.Message);
                            }

                        });

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(@"Could not read security token file """ + DefaultSecurityTokenFile + @""": " + e.Message);
                    }

                    // Write filtered (no invalid users, no expired tokens) tokens back to file...
                    File.WriteAllLines(DefaultSecurityTokenFile,
                                       SecurityTokens.Select(token => token.Key + ";" + token.Value.UserId + ";" + token.Value.Expires.ToIso8601()));

                }

            }

            #endregion

        }

        #endregion

        //ToDo: Receive Network Database Commands

        #region (private) ProcessCommand(Command, Parameters)

        private void ProcessCommand(String   Command,
                                    JObject  JSONObject)
        {

            switch (Command)
            {

                #region CreateUser

                case "CreateUser":

                    if (User.TryParseJSON(JSONObject,
                                          out User    _User,
                                          out String  ErrorResponse))
                    {
                        _Users.AddAndReturnValue(_User.Id, _User);
                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", ErrorResponse));

                    break;

                #endregion

                #region CreateGroup

                case "CreateGroup":

                    if (Group.TryParseJSON(JSONObject,
                                           out Group  _Group,
                                           out ErrorResponse) &&
                        _Group.Id != Admins.Id)
                    {
                        _Groups.AddAndReturnValue(_Group.Id, _Group);
                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", ErrorResponse));

                    break;

                #endregion

                #region CreateOrganization

                case "CreateOrganization":

                    if (Organization.TryParseJSON(JSONObject,
                                                  out Organization  _Organization,
                                                  out ErrorResponse))
                    {
                        _Organizations.AddAndReturnValue(_Organization.Id, _Organization);
                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", ErrorResponse));

                    break;

                #endregion

                #region LinkOrganizations

                case "LinkOrganizations":

                    var O2O_OrganizationOut  = _Organizations[Organization_Id.Parse(JSONObject["organizationOut"].Value<String>())];
                    var O2O_OrganizationIn   = _Organizations[Organization_Id.Parse(JSONObject["organizationIn" ].Value<String>())];
                    var O2O_EdgeLabel        = (Organization2OrganizationEdges) Enum.Parse(typeof(Organization2OrganizationEdges), JSONObject["edge"].   Value<String>());
                    var O2O_Privacy          = JSONObject.ParseMandatory_PrivacyLevel();

                    if (!O2O_OrganizationOut.Organization2OrganizationOutEdges.Any(edge => edge.EdgeLabel == O2O_EdgeLabel && edge.Target == O2O_OrganizationIn))
                        O2O_OrganizationOut.AddOutEdge(O2O_EdgeLabel, O2O_OrganizationIn,  O2O_Privacy);

                    if (!O2O_OrganizationIn. Organization2OrganizationInEdges. Any(edge => edge.EdgeLabel == O2O_EdgeLabel && edge.Source == O2O_OrganizationOut))
                        O2O_OrganizationIn. AddInEdge (O2O_EdgeLabel, O2O_OrganizationOut, O2O_Privacy);

                    break;

                #endregion

                #region AddUserToGroup

                case "AddUserToGroup":

                    var U2G_User     = _Users [User_Id. Parse(JSONObject["user" ].Value<String>())];
                    var U2G_Group    = _Groups[Group_Id.Parse(JSONObject["group"].Value<String>())];
                    var U2G_Edge     = (User2GroupEdges) Enum.Parse(typeof(User2GroupEdges), JSONObject["edge"].        Value<String>());
                    var U2G_Privacy  = JSONObject.ParseMandatory_PrivacyLevel();

                    if (!U2G_User.OutEdges(U2G_Group).Any(edge => edge == U2G_Edge))
                        U2G_User.AddOutgoingEdge(U2G_Edge, U2G_Group, U2G_Privacy);

                    if (!U2G_Group.Edges(U2G_Group).Any(edge => edge == U2G_Edge))
                        U2G_Group.AddIncomingEdge(U2G_User, U2G_Edge, U2G_Privacy);

                    break;

                #endregion

                #region AddUserToOrganization

                case "AddUserToOrganization":

                    var U2O_User          = _Users        [User_Id.        Parse(JSONObject["user" ].Value<String>())];
                    var U2O_Organization  = _Organizations[Organization_Id.Parse(JSONObject["organization"].Value<String>())];
                    var U2O_Edge          = (User2OrganizationEdges) Enum.Parse(typeof(User2OrganizationEdges), JSONObject["edge"].        Value<String>());
                    var U2O_Privacy       = JSONObject.ParseMandatory_PrivacyLevel();

                    if (!U2O_User.Edges(U2O_Organization).Any(edgelabel => edgelabel == U2O_Edge))
                        U2O_User.AddOutgoingEdge(U2O_Edge, U2O_Organization, U2O_Privacy);

                    if (!U2O_Organization.InEdges(U2O_Organization).Any(edgelabel => edgelabel == U2O_Edge))
                        U2O_Organization.AddIncomingEdge(U2O_User, U2O_Edge, U2O_Privacy);

                    break;

                #endregion

                #region AddNotification

                case "AddNotification":

                    if (JSONObject["userId"]?.Value<String>().IsNotNullOrEmpty() == true &&
                        JSONObject["type"  ]?.Value<String>().IsNotNullOrEmpty() == true)
                    {

                        var UserId  = User_Id.Parse(JSONObject["userId"]?.Value<String>());

                        if (JSONObject["notificationId"]?.Value<String>().IsNotNullOrEmpty() == true)
                            switch (JSONObject["type"]?.Value<String>())
                            {

                                case "EMailNotification":
                                    RegisterNotification(UserId,
                                                         Notification_Id.Parse(JSONObject["notificationId"]?.Value<String>()),
                                                         EMailNotification.Parse(JSONObject),
                                                         (a, b) => a.EMailAddress == b.EMailAddress);
                                    break;

                                case "SMSNotification":
                                    RegisterNotification(UserId,
                                                         Notification_Id.Parse(JSONObject["notificationId"]?.Value<String>()),
                                                         SMSNotification.Parse(JSONObject),
                                                         (a, b) => a.Phonenumber == b.Phonenumber);
                                    break;

                                case "HTTPSNotification":
                                    RegisterNotification(UserId,
                                                         Notification_Id.Parse(JSONObject["notificationId"]?.Value<String>()),
                                                         HTTPSNotification.Parse(JSONObject),
                                                         (a, b) => a.URL == b.URL);
                                    break;

                            }

                        else
                            switch (JSONObject["type"]?.Value<String>())
                            {

                                case "EMailNotification":
                                    RegisterNotification(UserId,
                                                         EMailNotification.Parse(JSONObject),
                                                         (a, b) => a.EMailAddress == b.EMailAddress);
                                    break;

                                case "SMSNotification":
                                    RegisterNotification(UserId,
                                                         SMSNotification.Parse(JSONObject),
                                                         (a, b) => a.Phonenumber == b.Phonenumber);
                                    break;

                                case "HTTPSNotification":
                                    RegisterNotification(UserId,
                                                         HTTPSNotification.Parse(JSONObject),
                                                         (a, b) => a.URL == b.URL);
                                    break;

                            }

                    }

                    break;

                #endregion

                #region RemoveNotification

                case "RemoveNotification":

                    if (JSONObject["userId"]?.Value<String>().IsNotNullOrEmpty() == true &&
                        JSONObject["type"  ]?.Value<String>().IsNotNullOrEmpty() == true)
                    {

                        var UserId  = User_Id.Parse(JSONObject["userId"]?.Value<String>());

                        if (JSONObject["notificationId"]?.Value<String>().IsNotNullOrEmpty() == true)
                            switch (JSONObject["type"]?.Value<String>())
                            {

                                case "EMailNotification":
                                    UnregisterNotification<EMailNotification>(UserId,
                                                                              Notification_Id.Parse(JSONObject["notificationId"]?.Value<String>()),
                                                                              a => a.EMailAddress == EMailNotification.Parse(JSONObject).EMailAddress);
                                    break;

                                case "SMSNotification":
                                    UnregisterNotification<SMSNotification>  (UserId,
                                                                              Notification_Id.Parse(JSONObject["notificationId"]?.Value<String>()),
                                                                              a => a.Phonenumber  == SMSNotification.  Parse(JSONObject).Phonenumber);
                                    break;

                            }

                        else
                            switch (JSONObject["type"]?.Value<String>())
                            {

                                case "EMailNotification":
                                    UnregisterNotification<EMailNotification>(UserId,
                                                                              a => a.EMailAddress == EMailNotification.Parse(JSONObject).EMailAddress);
                                    break;

                                case "SMSNotification":
                                    UnregisterNotification<SMSNotification>  (UserId,
                                                                              a => a.Phonenumber  == SMSNotification.  Parse(JSONObject).Phonenumber);
                                    break;

                            }


                    }

                    break;

                #endregion

            }

        }

        #endregion

        #region (protected) GetUsersAPIRessource(Ressource)

        /// <summary>
        /// Get an embedded ressource of the UsersAPI.
        /// </summary>
        /// <param name="Ressource">The path and name of the ressource to load.</param>
        protected Stream GetUsersAPIRessource(String Ressource)
            => GetType().Assembly.GetManifestResourceStream(HTTPRoot + Ressource);

        #endregion


        #region (protected) TryGetSecurityTokenFromCookie(Request)

        protected SecurityToken_Id? TryGetSecurityTokenFromCookie(HTTPRequest Request)
        {

            if (Request.Cookies == null)
                return null;

            if (Request. Cookies.TryGet  (CookieName,             out HTTPCookie       Cookie) &&
                         Cookie. TryGet  (SecurityTokenCookieKey, out String           Value)  &&
                SecurityToken_Id.TryParse(Value,                  out SecurityToken_Id SecurityTokenId))
            {
                return SecurityTokenId;
            }

            return null;

        }

        #endregion

        #region (protected) TryGetSecurityTokenFromCookie(Request, SecurityTokenId)

        protected Boolean TryGetSecurityTokenFromCookie(HTTPRequest Request, out SecurityToken_Id SecurityTokenId)
        {

            if (Request.Cookies  != null &&
                Request.Cookies. TryGet  (CookieName,             out HTTPCookie  Cookie) &&
                        Cookie.  TryGet  (SecurityTokenCookieKey, out String      Value)  &&
                SecurityToken_Id.TryParse(Value,                  out             SecurityTokenId))
            {
                return true;
            }

            SecurityTokenId = default(SecurityToken_Id);
            return false;

        }

        #endregion

        #region (protected) TryGetHTTPUser(Request, out User)

        protected Boolean TryGetHTTPUser(HTTPRequest Request, out User User)
        {

            #region Get user from cookie...

            if (Request.Cookies != null                                                                         &&
                Request. Cookies.TryGet     (CookieName,             out HTTPCookie        Cookie)              &&
                         Cookie. TryGet     (SecurityTokenCookieKey, out String            Value)               &&
                SecurityToken_Id.TryParse   (Value,                  out SecurityToken_Id  SecurityTokenId)     &&
                SecurityTokens.  TryGetValue(SecurityTokenId,        out SecurityToken     SecurityInformation) &&
                DateTime.UtcNow < SecurityInformation.Expires                                                   &&
                TryGetUser(SecurityInformation.UserId, out User))
            {
                return true;
            }

            #endregion

            #region Get user from Basic-Auth...

            if (Request.Authorization?.HTTPCredentialType == HTTPAuthenticationTypes.Basic &&
                User_Id.TryParse(Request.Authorization.Username, out User_Id UserId)       &&
                TryGetUser                 (UserId, out User)                              &&
                _LoginPasswords.TryGetValue(UserId, out LoginPassword Password)            &&
                Password.VerifyPassword(Request.Authorization.Password))
            {
                return true;
            }

            #endregion

            #region Get user from API Key...

            if (Request.X_API_Key.HasValue &&
                TryGetAPIKeyInfo(Request.X_API_Key.Value, out APIKeyInfo apiKeyInfo) &&
                !apiKeyInfo.IsDisabled &&
                DateTime.UtcNow < apiKeyInfo.Expires)
            {
                User = apiKeyInfo.User;
                return true;
            }

            #endregion

            User = null;
            return false;

        }

        #endregion

        #region (protected) TryGetHTTPUser(Request, User, Organizations, Response, RequireReadWriteAccess = false, Recursive = false)

        protected Boolean TryGetHTTPUser(HTTPRequest                    Request,
                                         out User                       User,
                                         out IEnumerable<Organization>  Organizations,
                                         out HTTPResponse               Response,
                                         Boolean                        RequireReadWriteAccess  = false,
                                         Boolean                        Recursive               = false)
        {

            if (!TryGetHTTPUser(Request, out User))
            {

                if (Request.RemoteSocket.IPAddress.IsIPv4 &&
                    Request.RemoteSocket.IPAddress.IsLocalhost)
                {
                    User           = Admins.User2GroupInEdges(edgelabel => edgelabel == User2GroupEdges.IsAdmin).FirstOrDefault()?.Source;
                    Organizations  = User.Organizations(RequireReadWriteAccess, Recursive);
                    Response       = null;
                    return true;
                }

                Organizations  = null;
                Response       = new HTTPResponseBuilder(Request) {
                                     HTTPStatusCode  = HTTPStatusCode.Unauthorized,
                                     Location        = URIPrefix + "/login",
                                     Date            = DateTime.Now,
                                     Server          = HTTPServer.DefaultServerName,
                                     CacheControl    = "private, max-age=0, no-cache",
                                     Connection      = "close"
                                 };

                return false;

            }

            Organizations = User?.Organizations(RequireReadWriteAccess, Recursive);
            Response      = null;
            return true;

        }

        #endregion

        #region (protected) TryGetHTTPUser(Request, User, Organizations,           RequireReadWriteAccess = false, Recursive = false)

        protected void TryGetHTTPUser(HTTPRequest                    Request,
                                      out User                       User,
                                      out IEnumerable<Organization>  Organizations,
                                      Boolean                        RequireReadWriteAccess  = false,
                                      Boolean                        Recursive               = false)
        {

            if (!TryGetHTTPUser(Request, out User))
            {

                if (Request.RemoteSocket.IPAddress.IsIPv4 &&
                    Request.RemoteSocket.IPAddress.IsLocalhost)
                {
                    User           = Admins.User2GroupInEdges(edgelabel => edgelabel == User2GroupEdges.IsAdmin).FirstOrDefault()?.Source;
                    Organizations  = User.Organizations(RequireReadWriteAccess, Recursive);
                    return;
                }

                Organizations  = null;
                return;

            }

            Organizations = User?.Organizations(RequireReadWriteAccess, Recursive);

        }

        #endregion

        #region (protected internal) SetUserRequest (Request)

        /// <summary>
        /// An event sent whenever set user request was received.
        /// </summary>
        public event RequestLogHandler OnSetUserRequest;

        protected internal HTTPRequest SetUserRequest(HTTPRequest Request)
        {

            OnSetUserRequest?.Invoke(Request.Timestamp,
                                              HTTPServer,
                                              Request);

            return Request;

        }

        #endregion

        #region (protected internal) SetUserResponse(Response)

        /// <summary>
        /// An event sent whenever a response on a set user request was sent.
        /// </summary>
        public event AccessLogHandler OnSetUserResponse;

        protected internal HTTPResponse SetUserResponse(HTTPResponse Response)
        {

            OnSetUserResponse?.Invoke(Response.Timestamp,
                                               HTTPServer,
                                               Response.HTTPRequest,
                                               Response);

            return Response;

        }

        #endregion



        #region (private) WriteToLogfile(Command, JSONData)

        private void WriteToLogfile(String   Command,
                                    JObject  JSONData)
        {

            if (!DisableLogfile)
            {
                lock (DefaultUsersAPIFile)
                {

                    WriteToLogfile(DefaultUsersAPIFile,
                                   Command,
                                   JSONData);

                }
            }

        }

        #endregion

        #region WriteToLogfile(Logfilename, Command, JSONData)

        public void WriteToLogfile(String   Logfilename,
                                   String   Command,
                                   JObject  JSONData)
        {

            if (!DisableLogfile)
            {
                lock (DefaultUsersAPIFile)
                {

                    var _JObject   = new JObject(
                                         new JProperty(Command,       JSONData),
                                         new JProperty("Writer",      SystemId),
                                         new JProperty("Timestamp",   DateTime.UtcNow.ToIso8601()),
                                         new JProperty("Nonce",       Guid.NewGuid().ToString().Replace("-", "")),
                                         new JProperty("ParentHash",  CurrentDatabaseHashValue)
                                     );

                    var SHA256                = new SHA256Managed();
                    CurrentDatabaseHashValue  = SHA256.ComputeHash(Encoding.Unicode.GetBytes(JSONWhitespaceRegEx.Replace(_JObject.ToString(), " "))).
                                                       Select(value => String.Format("{0:x2}", value)).
                                                       Aggregate();

                    _JObject.Add(new JProperty("HashValue", CurrentDatabaseHashValue));

                    var retry = true;

                    do
                    {

                        try
                        {

                            File.AppendAllText(Logfilename,
                                               JSONWhitespaceRegEx.Replace(_JObject.ToString(), " ") +
                                               Environment.NewLine);

                        }
                        catch (IOException ioEx)
                        {
                            retry = false;
                        }
                        catch (Exception e)
                        {
                            retry = false;
                        }

                    } while(retry);

                }
            }

        }

        #endregion

        #region WriteToCustomLogfile(Logfilename, Lock, Data)

        public void WriteToCustomLogfile(String  Logfilename,
                                         Object  Lock,
                                         String  Data)
        {

            if (!DisableLogfile)
            {
                lock (Lock)
                {

                    var retry = true;

                    do
                    {

                        try
                        {

                            File.AppendAllText(Logfilename,
                                               Data +
                                               Environment.NewLine);

                        }
                        catch (IOException ioEx)
                        {
                            retry = false;
                        }
                        catch (Exception e)
                        {
                            retry = false;
                        }

                    } while(retry);

                }
            }

        }

        #endregion

        #region WriteToLogfileAndNotify(Command, JSONData)

        public void WriteToLogfileAndNotify(String   Command,
                                            JObject  JSONData)
        {

            WriteToLogfile  (DefaultUsersAPIFile,
                             Command,
                             JSONData);

            SendNotification(Command,
                             JSONData);

        }

        #endregion

        #region WriteToLogfileAndNotify(Logfilename, Command, JSONData)

        public void WriteToLogfileAndNotify(String   Logfilename,
                                            String   Command,
                                            JObject  JSONData)
        {

            WriteToLogfile  (Logfilename,
                             Command,
                             JSONData);

            SendNotification(Command,
                             JSONData);

        }

        #endregion


        private ConcurrentBag<JObject> _NotificationMessages = new ConcurrentBag<JObject>();

        public IEnumerable<JObject> NotificationMessages
            => _NotificationMessages;

        public void SendNotification(String   Command,
                                     JObject  JSONData)
        {

            if (!DisableNotifications)
            {
                lock (_NotificationMessages)
                {

                    var _JObject   = new JObject(
                                         new JProperty(Command,       JSONData),
                                         new JProperty("writer",      SystemId),
                                         new JProperty("timestamp",   DateTime.UtcNow.ToIso8601()),
                                         new JProperty("nonce",       Guid.NewGuid().ToString().Replace("-", "")),
                                         new JProperty("parentHash",  CurrentDatabaseHashValue)
                                     );

                    var SHA256                = new SHA256Managed();
                    CurrentDatabaseHashValue  = SHA256.ComputeHash(Encoding.Unicode.GetBytes(JSONWhitespaceRegEx.Replace(_JObject.ToString(), " "))).
                                                       Select(value => String.Format("{0:x2}", value)).
                                                       Aggregate();

                    _JObject.Add(new JProperty("HashValue", CurrentDatabaseHashValue));


                    _NotificationMessages.Add(_JObject);

                }
            }

        }


        #region DataLicenses

        #region DataLicenses

        protected readonly Dictionary<DataLicense_Id, DataLicense> _DataLicenses;

        public IEnumerable<DataLicense> DataLicenses
            => _DataLicenses.Values;

        #endregion

        #region CreateDataLicense           (Id, Description, URIs)

        /// <summary>
        /// Create a new data license.
        /// </summary>
        /// <param name="Id">The unique identification of the data license.</param>
        /// <param name="Description">The description of the data license.</param>
        /// <param name="URIs">Optional URIs for more information.</param>
        public DataLicense CreateDataLicense(DataLicense_Id   Id,
                                             String           Description,
                                             params String[]  URIs)
        {

            lock (_DataLicenses)
            {

                if (_DataLicenses.ContainsKey(Id))
                    throw new ArgumentException("The given data license already exists!", nameof(Id));


                var DataLicense = new DataLicense(Id,
                                                  Description,
                                                  URIs);

                WriteToLogfile("CreateDataLicense", DataLicense.ToJSON());

                return _DataLicenses.AddAndReturnValue(DataLicense.Id, DataLicense);

            }

        }

        #endregion

        #region CreateDataLicenseIfNotExists(Id, Description, URIs)

        /// <summary>
        /// Create a new data license.
        /// </summary>
        /// <param name="Id">The unique identification of the data license.</param>
        /// <param name="Description">The description of the data license.</param>
        /// <param name="URIs">Optional URIs for more information.</param>
        public DataLicense CreateDataLicenseIfNotExists(DataLicense_Id   Id,
                                                        String           Description,
                                                        params String[]  URIs)
        {

            lock (_DataLicenses)
            {

                if (_DataLicenses.ContainsKey(Id))
                    return _DataLicenses[Id];

                return CreateDataLicense(Id,
                                         Description,
                                         URIs);

            }

        }

        #endregion


        #region GetDataLicense   (DataLicenseId)

        /// <summary>
        /// Get the data license having the given unique identification.
        /// </summary>
        /// <param name="DataLicenseId">The unique identification of the data license.</param>
        public DataLicense GetDataLicense(DataLicense_Id  DataLicenseId)
        {

            lock (_DataLicenses)
            {

                if (_DataLicenses.TryGetValue(DataLicenseId, out DataLicense DataLicense))
                    return DataLicense;

                return null;

            }

        }

        #endregion

        #region TryGetDataLicense(DataLicenseId, out DataLicense)

        /// <summary>
        /// Try to get the data license having the given unique identification.
        /// </summary>
        /// <param name="DataLicenseId">The unique identification of the data license.</param>
        /// <param name="DataLicense">The data license.</param>
        public Boolean TryGetDataLicense(DataLicense_Id   DataLicenseId,
                                         out DataLicense  DataLicense)
        {

            lock (_DataLicenses)
            {
                return _DataLicenses.TryGetValue(DataLicenseId, out DataLicense);
            }

        }

        #endregion

        #endregion

        #region Users

        #region Users

        protected readonly Dictionary<User_Id, User> _Users;

        public IEnumerable<User> Users
            => _Users.Values;

        #endregion

        #region CreateUser           (Id, EMail, Password, Name = null, PublicKeyRing = null, Telephone = null, Description = null, IsPublic = true, IsDisabled = false, IsAuthenticated = false)

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="Id">The unique identification of the user.</param>
        /// <param name="EMail">The primary e-mail of the user.</param>
        /// <param name="Password">The password of the user.</param>
        /// <param name="Name">An offical (multi-language) name of the user.</param>
        /// <param name="PublicKeyRing">An optional PGP/GPG public keyring of the user.</param>
        /// <param name="Telephone">An optional telephone number of the user.</param>
        /// <param name="Description">An optional (multi-language) description of the user.</param>
        /// <param name="GeoLocation">An optional geographical location of the user.</param>
        /// <param name="Address">An optional address of the user.</param>
        /// <param name="PrivacyLevel">Whether the user will be shown in user listings, or not.</param>
        /// <param name="IsAuthenticated">The user will not be shown in user listings, as its primary e-mail address is not yet authenticated.</param>
        /// <param name="IsDisabled">The user will be shown in user listings.</param>
        public User CreateUser(User_Id             Id,
                               SimpleEMailAddress  EMail,
                               Password            Password,
                               String              Name              = null,
                               String              PublicKeyRing     = null,
                               PhoneNumber?        Telephone         = null,
                               I18NString          Description       = null,
                               GeoCoordinate?      GeoLocation       = null,
                               Address             Address           = null,
                               PrivacyLevel        PrivacyLevel      = PrivacyLevel.World,
                               Boolean             IsAuthenticated   = false,
                               Boolean             IsDisabled        = false)
        {

            lock (_Users)
            {

                if (_Users.ContainsKey(Id))
                    throw new ArgumentException("The given username already exists!", nameof(Id));


                var User = new User(Id,
                                    EMail,
                                    Name,
                                    PublicKeyRing,
                                    Telephone,
                                    Description,
                                    GeoLocation,
                                    Address,
                                    PrivacyLevel,
                                    IsAuthenticated,
                                    IsDisabled);

                WriteToLogfileAndNotify("CreateUser",
                                        User.ToJSON());

                SetPassword(Id, Password);

                return _Users.AddAndReturnValue(User.Id, User);

            }

        }

        #endregion

        #region CreateUserIfNotExists(Id, EMail, Password, Name = null, PublicKeyRing = null, Telephone = null, Description = null, IsPublic = true, IsDisabled = false, IsAuthenticated = false)

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="Id">The unique identification of the user.</param>
        /// <param name="EMail">The primary e-mail of the user.</param>
        /// <param name="Password">The password of the user.</param>
        /// <param name="Name">An offical (multi-language) name of the user.</param>
        /// <param name="PublicKeyRing">An optional PGP/GPG public keyring of the user.</param>
        /// <param name="Telephone">An optional telephone number of the user.</param>
        /// <param name="Description">An optional (multi-language) description of the user.</param>
        /// <param name="GeoLocation">An optional geographical location of the user.</param>
        /// <param name="Address">An optional address of the user.</param>
        /// <param name="PrivacyLevel">Whether the user will be shown in user listings, or not.</param>
        /// <param name="IsAuthenticated">The user will not be shown in user listings, as its primary e-mail address is not yet authenticated.</param>
        /// <param name="IsDisabled">The user will be shown in user listings.</param>
        public User CreateUserIfNotExists(User_Id             Id,
                                          SimpleEMailAddress  EMail,
                                          Password            Password,
                                          String              Name              = null,
                                          String              PublicKeyRing     = null,
                                          PhoneNumber?        Telephone         = null,
                                          I18NString          Description       = null,
                                          GeoCoordinate?      GeoLocation       = null,
                                          Address             Address           = null,
                                          PrivacyLevel        PrivacyLevel      = PrivacyLevel.World,
                                          Boolean             IsAuthenticated   = false,
                                          Boolean             IsDisabled        = false)
        {

            lock (_Users)
            {

                if (_Users.ContainsKey(Id))
                    return _Users[Id];

                return CreateUser(Id,
                                  EMail,
                                  Password,
                                  Name,
                                  PublicKeyRing,
                                  Telephone,
                                  Description,
                                  GeoLocation,
                                  Address,
                                  PrivacyLevel,
                                  IsAuthenticated,
                                  IsDisabled);

            }

        }

        #endregion

        #region SetPassword(Login, Password)

        public void SetPassword(User_Id   Login,
                                Password  Password)
        {

            lock (_Users)
            {

                if (!_LoginPasswords.ContainsKey(Login))
                    _LoginPasswords.Add(Login, new LoginPassword(Login, Password));
                else
                    _LoginPasswords[Login]   = new LoginPassword(Login, Password);

                File.AppendAllText(DefaultPasswordFile,
                                   String.Concat(Login,
                                                 ":",
                                                 Password.Salt.UnsecureString(),
                                                 ":",
                                                 Password.UnsecureString,
                                                 Environment.NewLine));

            }

        }

        #endregion

        #region VerifyPassword(Login, Password)

        public Boolean VerifyPassword(User_Id  Login,
                                      String   Password)
        {

            lock (_Users)
            {

                return _LoginPasswords.TryGetValue(Login, out LoginPassword LoginPassword) &&
                        LoginPassword.VerifyPassword(Password);

            }

        }

        #endregion

        #region GetUser   (UserId)

        /// <summary>
        /// Get the user having the given unique identification.
        /// </summary>
        /// <param name="UserId">The unique identification of the user.</param>
        public User GetUser(User_Id  UserId)
        {

            lock (_Users)
            {

                if (_Users.TryGetValue(UserId, out User User))
                    return User;

                return null;

            }

        }

        #endregion

        #region UserExists(UserId)

        /// <summary>
        /// Get the user having the given unique identification.
        /// </summary>
        /// <param name="UserId">The unique identification of the user.</param>
        public Boolean UserExists(User_Id  UserId)
        {

            lock (_Users)
            {
                return _Users.ContainsKey(UserId);
            }

        }

        #endregion

        #region TryGetUser(UserId, out User)

        /// <summary>
        /// Try to get the user having the given unique identification.
        /// </summary>
        /// <param name="UserId">The unique identification of the user.</param>
        /// <param name="User">The user.</param>
        public Boolean TryGetUser(User_Id   UserId,
                                  out User  User)
        {

            lock (_Users)
            {
                return _Users.TryGetValue(UserId, out User);
            }

        }

        #endregion

        #endregion

        #region API Keys

        #region API Keys

        protected readonly Dictionary<APIKey, APIKeyInfo> _APIKeys;

        public IEnumerable<APIKeyInfo> APIKeys
            => _APIKeys.Values;

        #endregion

        #region GetAPIKeyInfo   (APIKey)

        /// <summary>
        /// Get the APIKeyInfo for the given API key.
        /// </summary>
        /// <param name="APIKeyInfoId">The unique APIKey.</param>
        public APIKeyInfo GetAPIKeyInfo(APIKey  APIKeyInfoId)
        {

            lock (_APIKeys)
            {

                if (_APIKeys.TryGetValue(APIKeyInfoId, out APIKeyInfo APIKeyInfo))
                    return APIKeyInfo;

                return null;

            }

        }

        #endregion

        #region TryGetAPIKeyInfo(APIKey, out APIKeyInfo)

        /// <summary>
        /// Try to get the APIKeyInfo having the given unique identification.
        /// </summary>
        /// <param name="APIKey">The unique API key.</param>
        /// <param name="APIKeyInfo">The APIKeyInfo.</param>
        public Boolean TryGetAPIKeyInfo(APIKey          APIKey,
                                        out APIKeyInfo  APIKeyInfo)
        {

            lock (_APIKeys)
            {
                return _APIKeys.TryGetValue(APIKey, out APIKeyInfo);
            }

        }

        #endregion

        #endregion

        #region Groups

        #region CreateGroup           (Id, Name = null, Description = null)

        public Group CreateGroup(Group_Id   Id,
                                 I18NString Name         = null,
                                 I18NString Description  = null)
        {

            lock (_Groups)
            {

                if (_Groups.ContainsKey(Id))
                    throw new ArgumentException("The given group identification already exists!", nameof(Id));


                var Group = new Group(Id,
                                      Name,
                                      Description);

                if (Group.Id.ToString() != AdminGroupName)
                    WriteToLogfile("CreateGroup", Group.ToJSON());

                return _Groups.AddAndReturnValue(Group.Id, Group);

            }

        }

        #endregion

        #region CreateGroupIfNotExists(Id, Name = null, Description = null)

        public Group CreateGroupIfNotExists(Group_Id    Id,
                                            I18NString  Name         = null,
                                            I18NString  Description  = null)
        {

            lock (_Groups)
            {

                if (_Groups.ContainsKey(Id))
                    return _Groups[Id];

                return CreateGroup(Id,
                                   Name,
                                   Description);

            }

        }

        #endregion

        #region Groups

        protected readonly Dictionary<Group_Id, Group> _Groups;

        public IEnumerable<Group> Groups
            => _Groups.Values;

        #endregion

        #region GetGroup   (GroupId)

        /// <summary>
        /// Get the group having the given unique identification.
        /// </summary>
        /// <param name="GroupId">The unique identification of the group.</param>
        public Group GetGroup(Group_Id  GroupId)
        {

            lock (_Groups)
            {

                if (_Groups.TryGetValue(GroupId, out Group Group))
                    return Group;

                return null;

            }

        }

        #endregion

        #region TryGetGroup(GroupId, out Group)

        /// <summary>
        /// Try to get the group having the given unique identification.
        /// </summary>
        /// <param name="GroupId">The unique identification of the group.</param>
        /// <param name="Group">The group.</param>
        public Boolean TryGetGroup(Group_Id   GroupId,
                                          out Group  Group)
        {

            lock (_Groups)
            {
                return _Groups.TryGetValue(GroupId, out Group);
            }

        }

        #endregion

        #region AddToGroup(User, Edge, Group, PrivacyLevel = World)

        public Boolean AddToGroup(User             User,
                                  User2GroupEdges  Edge,
                                  Group            Group,
                                  PrivacyLevel     PrivacyLevel = PrivacyLevel.World)
        {

            if (!User.OutEdges(Group).Any(edge => edge == Edge))
            {

                User.AddOutgoingEdge(Edge, Group, PrivacyLevel);

                if (!Group.Edges(Group).Any(edge => edge == Edge))
                    Group.AddIncomingEdge(User, Edge,  PrivacyLevel);

                WriteToLogfile("AddUserToGroup",
                               new JObject(
                                   new JProperty("user",     User.Id.ToString()),
                                   new JProperty("edge",     Edge.   ToString()),
                                   new JProperty("group",    Group.  ToString()),
                                   PrivacyLevel.ToJSON()
                               ));

                return true;

            }

            return false;

        }

        #endregion


        #region IsAdmin(User)

        /// <summary>
        /// Check if the given user is an API admin.
        /// </summary>
        /// <param name="User">A user.</param>
        public Boolean IsAdmin(User User)

            => User.Groups(User2GroupEdges.IsAdmin).
                    Contains(Admins);

        #endregion

        #region IsAdmin(UserId)

        /// <summary>
        /// Check if the given user is an API admin.
        /// </summary>
        /// <param name="UserId">A user identification.</param>
        public Boolean IsAdmin(User_Id UserId)

            => TryGetUser(UserId, out User User) &&
               IsAdmin(User);

        #endregion

        #endregion

        #region Organizations

        #region CreateOrganization           (Id, Name = null, Description = null, ParentOrganization = null)

        public Organization CreateOrganization(Organization_Id      Id,
                                               I18NString           Name                = null,
                                               I18NString           Description         = null,
                                               SimpleEMailAddress?  EMail               = null,
                                               String               PublicKeyRing       = null,
                                               PhoneNumber?         Telephone           = null,
                                               GeoCoordinate?       GeoLocation         = null,
                                               Address              Address             = null,
                                               PrivacyLevel         PrivacyLevel        = PrivacyLevel.World,
                                               Boolean              IsDisabled          = false,
                                               String               DataSource          = "",
                                               Organization         ParentOrganization  = null)
        {

            lock (_Organizations)
            {

                if (_Organizations.ContainsKey(Id))
                    throw new ArgumentException("The given organization identification already exists!", nameof(Id));


                var Organization = new Organization(Id,
                                                    Name,
                                                    Description,
                                                    EMail,
                                                    PublicKeyRing,
                                                    Telephone,
                                                    GeoLocation,
                                                    Address,
                                                    PrivacyLevel,
                                                    IsDisabled,
                                                    DataSource);

                WriteToLogfileAndNotify("CreateOrganization",
                                        Organization.ToJSON());

                var NewOrg = _Organizations.AddAndReturnValue(Organization.Id, Organization);

                if (ParentOrganization != null)
                    LinkOrganizations(NewOrg, Organization2OrganizationEdges.IsChildOf, ParentOrganization);

                return NewOrg;

            }

        }

        #endregion

        #region CreateOrganizationIfNotExists(Id, Name = null, Description = null, ParentOrganization = null)

        public Organization CreateOrganizationIfNotExists(Organization_Id      Id,
                                                          I18NString           Name                = null,
                                                          I18NString           Description         = null,
                                                          SimpleEMailAddress?  EMail               = null,
                                                          String               PublicKeyRing       = null,
                                                          PhoneNumber?         Telephone           = null,
                                                          GeoCoordinate?       GeoLocation         = null,
                                                          Address              Address             = null,
                                                          PrivacyLevel         PrivacyLevel        = PrivacyLevel.World,
                                                          Boolean              IsDisabled          = false,
                                                          String               DataSource          = "",
                                                          Organization         ParentOrganization  = null)
        {

            lock (_Organizations)
            {

                if (_Organizations.ContainsKey(Id))
                    return _Organizations[Id];

                return CreateOrganization(Id,
                                          Name,
                                          Description,
                                          EMail,
                                          PublicKeyRing,
                                          Telephone,
                                          GeoLocation,
                                          Address,
                                          PrivacyLevel,
                                          IsDisabled,
                                          DataSource,
                                          ParentOrganization);

            }

        }

        #endregion

        #region Organizations

        protected readonly Dictionary<Organization_Id, Organization> _Organizations;

        public IEnumerable<Organization> Organizations
            => _Organizations.Values;

        #endregion

        #region GetOrganization   (OrganizationId)

        /// <summary>
        /// Get the organization having the given unique identification.
        /// </summary>
        /// <param name="OrganizationId">The unique identification of the organization.</param>
        public Organization GetOrganization(Organization_Id  OrganizationId)
        {

            lock (_Organizations)
            {

                if (_Organizations.TryGetValue(OrganizationId, out Organization Organization))
                    return Organization;

                return null;

            }

        }

        #endregion

        #region TryGetOrganization(OrganizationId, out Organization)

        /// <summary>
        /// Try to get the organization having the given unique identification.
        /// </summary>
        /// <param name="OrganizationId">The unique identification of the organization.</param>
        /// <param name="Organization">The organization.</param>
        public Boolean TryGetOrganization(Organization_Id   OrganizationId,
                                          out Organization  Organization)
        {

            lock (_Organizations)
            {
                return _Organizations.TryGetValue(OrganizationId, out Organization);
            }

        }

        #endregion


        #region AddToOrganization(User, Edge, Organization, PrivacyLevel = World)

        public Boolean AddToOrganization(User                    User,
                                         User2OrganizationEdges  Edge,
                                         Organization            Organization,
                                         PrivacyLevel            PrivacyLevel = PrivacyLevel.World)
        {

            if (!User.Edges(Organization).Any(edge => edge == Edge))
            {

                User.AddOutgoingEdge(Edge, Organization, PrivacyLevel);

                if (!Organization.InEdges(Organization).Any(edgelabel => edgelabel == Edge))
                    Organization.AddIncomingEdge(User, Edge, PrivacyLevel);

                WriteToLogfileAndNotify("AddUserToOrganization",
                                        new JObject(
                                            new JProperty("user",          User.        Id.ToString()),
                                            new JProperty("edge",          Edge.           ToString()),
                                            new JProperty("organization",  Organization.Id.ToString()),
                                            PrivacyLevel.ToJSON()
                                        ));

                return true;

            }

            return false;

        }

        #endregion

        #region LinkOrganizations(OrganizationOut, EdgeLabel, OrganizationIn, Privacy = Public)

        public Boolean LinkOrganizations(Organization                    OrganizationOut,
                                         Organization2OrganizationEdges  EdgeLabel,
                                         Organization                    OrganizationIn,
                                         PrivacyLevel                    Privacy = PrivacyLevel.World)
        {

            if (!OrganizationOut.
                    Organization2OrganizationOutEdges.
                    Where(edge => edge.Target    == OrganizationIn).
                    Any  (edge => edge.EdgeLabel == EdgeLabel))
            {

                OrganizationOut.AddOutEdge(EdgeLabel, OrganizationIn, Privacy);

                if (!OrganizationIn.
                        Organization2OrganizationInEdges.
                        Where(edge => edge.Source    == OrganizationOut).
                        Any  (edge => edge.EdgeLabel == EdgeLabel))
                {
                    OrganizationIn.AddIncomingEdge(OrganizationOut, EdgeLabel, Privacy);
                }

                WriteToLogfileAndNotify("LinkOrganizations",
                                        new JObject(
                                            new JProperty("organizationOut", OrganizationOut.Id.ToString()),
                                            new JProperty("edge",            EdgeLabel.         ToString()),
                                            new JProperty("organizationIn",  OrganizationIn. Id.ToString()),
                                            Privacy.ToJSON()
                                        ));

                return true;

            }

            return false;

        }

        #endregion

        #endregion

        #region Messages

        #region CreateMessage(Id, Sender, Receivers, Headline = null, Text = null)

        public Message CreateMessage(User_Id               Sender,
                                     IEnumerable<User_Id>  Receivers,
                                     I18NString            Subject,
                                     I18NString            Text,
                                     Message_Id?           Id  = null)
        {

            var Message = new Message(Id.HasValue
                                          ? Id.Value
                                          : Message_Id.New,
                                      Sender,
                                      Receivers,
                                      Subject,
                                      Text);

            return _Messages.AddAndReturnValue(Message.Id, Message);

        }

        #endregion

        #region Messages

        protected readonly Dictionary<Message_Id, Message> _Messages;

        /// <summary>
        /// A collection of message.
        /// </summary>
        public IEnumerable<Message> Messages
            => _Messages.Values;

        #endregion

        // Create Mailinglist

        #endregion

        #region Notifications

        public Notifications RegisterNotification<T>(User                 User,
                                                     T                    NotificationType,
                                                     Func<T, T, Boolean>  EqualityComparer)

            where T : ANotificationType

            => _Notifications.Add(User,
                                  NotificationType,
                                  EqualityComparer);

        public Notifications RegisterNotification<T>(User_Id              User,
                                                     T                    NotificationType,
                                                     Func<T, T, Boolean>  EqualityComparer)

            where T : ANotificationType

            => _Notifications.Add(User,
                                  NotificationType,
                                  EqualityComparer);


        public Notifications RegisterNotification<T>(User                 User,
                                                     Notification_Id      NotificationId,
                                                     T                    NotificationType,
                                                     Func<T, T, Boolean>  EqualityComparer)

            where T : ANotificationType

            => _Notifications.Add(User,
                                  NotificationId,
                                  NotificationType,
                                  EqualityComparer);

        public Notifications RegisterNotification<T>(User_Id              User,
                                                     Notification_Id      NotificationId,
                                                     T                    NotificationType,
                                                     Func<T, T, Boolean>  EqualityComparer)

            where T : ANotificationType

            => _Notifications.Add(User,
                                  NotificationId,
                                  NotificationType,
                                  EqualityComparer);



        public IEnumerable<T> GetNotifications<T>(User             User,
                                                  Notification_Id  NotificationId)

            where T : ANotificationType

            => _Notifications.GetNotifications<T>(User,
                                                  NotificationId);

        public IEnumerable<T> GetNotifications<T>(User_Id          User,
                                                  Notification_Id  NotificationId)

            where T : ANotificationType

            => _Notifications.GetNotifications<T>(User,
                                                  NotificationId);



        public Notifications UnregisterNotification<T>(User              User,
                                                       Func<T, Boolean>  EqualityComparer)

            where T : ANotificationType

            => _Notifications.Remove(User,
                                     EqualityComparer);

        public Notifications UnregisterNotification<T>(User_Id           User,
                                                       Func<T, Boolean>  EqualityComparer)

            where T : ANotificationType

            => _Notifications.Remove(User,
                                     EqualityComparer);


        public Notifications UnregisterNotification<T>(User              User,
                                                       Notification_Id   NotificationId,
                                                       Func<T, Boolean>  EqualityComparer)

            where T : ANotificationType

            => _Notifications.Remove(User,
                                     NotificationId,
                                     EqualityComparer);

        public Notifications UnregisterNotification<T>(User_Id           User,
                                                       Notification_Id   NotificationId,
                                                       Func<T, Boolean>  EqualityComparer)

            where T : ANotificationType

            => _Notifications.Remove(User,
                                     NotificationId,
                                     EqualityComparer);

        #endregion


        #region Start()

        public void Start()
        {

            lock (HTTPServer)
            {

                if (!HTTPServer.IsStarted)
                    HTTPServer.Start();

                //SendStarted(this, DateTime.UtcNow);

            }

        }

        #endregion

        #region Shutdown(Message = null, Wait = true)

        public void Shutdown(String Message = null, Boolean Wait = true)
        {

            lock (HTTPServer)
            {

                HTTPServer.Shutdown(Message, Wait);
                //SendCompleted(this, DateTime.UtcNow, Message);

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
