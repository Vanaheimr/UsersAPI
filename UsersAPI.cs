/*
 * Copyright (c) 2014-2017, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using System.Text;

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

        public  const             String                              SignUpContext                  = "";
        public  const             String                              SignInOutContext               = "";
        public  const             String                              DefaultCookieName              = "OpenDataSocial";
        public  const             String                              HTTPCookieDomain               = "";

        public  const             String                              DefaultUsersAPIFile            = "UsersAPI_Users.db";
        public  const             String                              DefaultPasswordFile            = "UsersAPI_Passwords.db";
        //public  const             String                              DefaultGroupDBFile             = "UsersAPI_Groups.db";
        //public  const             String                              DefaultUser2GroupDBFile        = "UsersAPI_User2Group.db";


        private static            Regex                               UserDB_RegEx                   = new Regex(@"(\s)+",
                                                                                                                 RegexOptions.IgnorePatternWhitespace);

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

        public String CookieName { get; }

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
        public Byte MinLoginLenght
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


        /// <summary>
        /// The current hash value of the API.
        /// </summary>
        public String CurrentHash { get; private set; }

        public String SystemId { get; }

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

                        String                              CookieName                        = DefaultCookieName,
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

                           String                              CookieName                   = DefaultCookieName,
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

            this.CookieName                   = CookieName.IsNotNullOrEmpty() ? CookieName : DefaultCookieName;
            this._DefaultLanguage             = DefaultLanguage;
            this._LogoImage                   = LogoImage;
            this.NewUserSignUpEMailCreator   = NewUserSignUpEMailCreator;
            this.NewUserWelcomeEMailCreator  = NewUserWelcomeEMailCreator;
            this.ResetPasswordEMailCreator   = ResetPasswordEMailCreator;
            this._MinUserNameLenght           = MinUserNameLenght;
            this._MinRealmLenght              = MinRealmLenght;
            this._MinPasswordLenght           = MinPasswordLenght;
            this._SignInSessionLifetime       = SignInSessionLifetime.HasValue ? SignInSessionLifetime.Value : DefaultSignInSessionLifetime;

            this.RessourcesProvider           = RessourcesProvider;

            this._Users                       = new Dictionary<User_Id,         User>();
            this._Groups                      = new Dictionary<Group_Id,        Group>();
            this._Organizations               = new Dictionary<Organization_Id, Organization>();
            this._Messages                    = new Dictionary<Message_Id,      Message>();

            this._LoginPasswords              = new Dictionary<User_Id,         LoginPassword>();
            this._VerificationTokens          = new List<VerificationToken>();

            this._DNSClient                   = HTTPServer.DNSClient;
            this.SystemId                     = Environment.MachineName.Replace("/", "") + "/" + HTTPServer.DefaultHTTPServerPort;

            #endregion

            RegisterURITemplates();

            #region Read UsersAPI file...

            if (File.Exists(DefaultUsersAPIFile))
            {

                File.ReadLines(DefaultUsersAPIFile).ForEachCounted((line, linenumber) => {

                    try
                    {

                        var JSONCommand     = JObject.Parse(line);
                        var JSONParameters  = (JSONCommand.First as JProperty).Value as JObject;
                        CurrentHash         = JSONCommand["HashValue"].Value<String>();

                        switch ((JSONCommand.First as JProperty).Name)
                        {

                            #region CreateUser

                            case "CreateUser":

                                var User = new User(User_Id.           Parse(JSONParameters["@id"            ].Value<String>(),
                                                                             JSONParameters["realm"          ].Value<String>()),
                                                    SimpleEMailAddress.Parse(JSONParameters["email"          ].Value<String>()),
                                                                             JSONParameters["name"           ].Value<String>(),
                                                                             JSONParameters["publickey"      ].Value<String>(),
                                                                             JSONParameters["telephone"      ].Value<String>(),
                                                                             JSONParameters.ParseI18NString("description"),
                                                                             JSONParameters["isPublic"       ].Value<Boolean>(),
                                                                             JSONParameters["isDisabled"     ].Value<Boolean>(),
                                                                             JSONParameters["isAuthenticated"].Value<Boolean>());

                                _Users.AddAndReturnValue(User.Id, User);

                                break;

                            #endregion

                            #region CreateGroup

                            case "CreateGroup":

                                var Group = new Group(Group_Id.Parse(JSONParameters["@id"].Value<String>()),
                                                      JSONParameters.ParseI18NString("name"),
                                                      JSONParameters.ParseI18NString("description"),
                                                      JSONParameters["isPublic"].   Value<Boolean>(),
                                                      JSONParameters["isDisabled"]. Value<Boolean>());

                                _Groups.AddAndReturnValue(Group.Id, Group);

                                break;

                            #endregion

                            #region CreateOrganization

                            case "CreateOrganization":

                                var Organization = new Organization(Organization_Id.Parse(JSONParameters["@id"].Value<String>()),
                                                                    JSONParameters.ParseI18NString("name"),
                                                                    JSONParameters.ParseI18NString("description"),
                                                                    JSONParameters["isPublic"].Value<Boolean>(),
                                                                    JSONParameters["isDisabled"].Value<Boolean>());

                                _Organizations.AddAndReturnValue(Organization.Id, Organization);

                                break;

                            #endregion

                            #region AddUserToGroup

                            case "AddUserToGroup":

                                var U2G_User     = _Users [User_Id. Parse(JSONParameters["user" ].Value<String>(),
                                                                          JSONParameters["realm"].Value<String>())];
                                var U2G_Group    = _Groups[Group_Id.Parse(JSONParameters["group"].Value<String>())];
                                var U2G_Edge     = (User2GroupEdges) Enum.Parse(typeof(User2GroupEdges), JSONParameters["edge"].   Value<String>());
                                var U2G_Privacy  = (PrivacyLevel)    Enum.Parse(typeof(PrivacyLevel),    JSONParameters["privacy"].Value<String>());

                                if (!U2G_User.Edges(U2G_Group).Any(edge => edge == U2G_Edge))
                                    U2G_User.AddOutgoingEdge(U2G_Edge, U2G_Group, U2G_Privacy);

                                if (!U2G_Group.Edges(U2G_Group).Any(edge => edge == U2G_Edge))
                                    U2G_Group.AddIncomingEdge(U2G_User, U2G_Edge, U2G_Privacy);

                                break;

                            #endregion

                            #region AddUserToOrganization

                            case "AddUserToOrganization":

                                var U2O_User          = _Users        [User_Id.        Parse(JSONParameters["user" ].Value<String>(),
                                                                                             JSONParameters["realm"].Value<String>())];
                                var U2O_Organization  = _Organizations[Organization_Id.Parse(JSONParameters["organization"].Value<String>())];
                                var U2O_Edge          = (User2OrganizationEdges) Enum.Parse(typeof(User2OrganizationEdges), JSONParameters["edge"].   Value<String>());
                                var U2O_Privacy       = (PrivacyLevel)           Enum.Parse(typeof(PrivacyLevel),           JSONParameters["privacy"].Value<String>());

                                if (!U2O_User.Edges(U2O_Organization).Any(edge => edge == U2O_Edge))
                                    U2O_User.AddOutgoingEdge(U2O_Edge, U2O_Organization, U2O_Privacy);

                                if (!U2O_Organization.Edges(U2O_Organization).Any(edge => edge == U2O_Edge))
                                    U2O_Organization.AddIncomingEdge(U2O_User, U2O_Edge, U2O_Privacy);

                                break;

                             #endregion

                        }

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(@"Could not read UserDB file """ + DefaultUsersAPIFile + @""" line " + linenumber + ": " + e.Message);
                    }

                });

            }

            this.Admins  = CreateGroupIfNotExists(Group_Id.Parse("Admins"),
                                                  I18NString.Create(Languages.eng, "Admins"),
                                                  I18NString.Create(Languages.eng, "All admins of the API."));

            #endregion

            #region Read Password file...

            if (File.Exists(DefaultPasswordFile))
            {

                File.ReadLines(DefaultPasswordFile).ForEachCounted((line, linenumber) => {

                    try
                    {

                        var LoginPassword  = line.Split(new Char[] { ':' }, StringSplitOptions.None);
                        var Login          = User_Id.Parse(LoginPassword[0]);

                        if (!_LoginPasswords.ContainsKey(Login))
                            _LoginPasswords.Add(Login,
                                                new LoginPassword(Login,
                                                                  LoginPassword[2].IsNotNullOrEmpty()
                                                                      ? LoginPassword[2]
                                                                      : null,
                                                                  LoginPassword[1]));

                        else
                            _LoginPasswords[Login] = new LoginPassword(Login,
                                                                       LoginPassword[2].IsNotNullOrEmpty()
                                                                           ? LoginPassword[2]
                                                                           : null,
                                                                       LoginPassword[1]);

                    }
                    catch (Exception e)
                    {
                        DebugX.Log(@"Could not read password file """ + DefaultPasswordFile + @""" line " + linenumber + ": " + e.Message);
                    }

                });

            }

            #endregion

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

                                               String                              CookieName                   = DefaultCookieName,
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

                            RessourcesProvider,

                            LogfileName);

        #endregion

        #region (private) RegisterURITemplates()

        private void RegisterURITemplates()
        {

            #region /shared/UsersAPI

            HTTPServer.RegisterResourcesFolder(HTTPHostname.Any, "/shared/UsersAPI", HTTPRoot.Substring(0, HTTPRoot.Length - 1));

            #endregion


            HTTPServer.AddFilter((server, request) => {

                #region Protect anything under / and /admin...

                if ((request.URI         == "/" ||
                     request.URI         == "/index.html" ||
                     request.URI.StartsWith("/admin", StringComparison.Ordinal))
                     &&
                    (request.Cookie      == null ||
                     request.Cookie.Name != CookieName))
                    // Unkown cookie?!
                {

                    return new HTTPResponseBuilder(request) {
                                   HTTPStatusCode  = HTTPStatusCode.TemporaryRedirect,
                                   Location        = "/login",
                                   Date            = DateTime.Now,
                                   Server          = HTTPServer.DefaultServerName,
                                   CacheControl    = "private, max-age=0, no-cache",
                                   Connection      = "close"
                               }.AsImmutable();

                }

                #endregion

                #region Disallow non-admins to access anything under /admin...

                if (request.URI.StartsWith("/admin", StringComparison.Ordinal) &&
                    request.Cookie      != null &&
                    request.Cookie.Name == CookieName &&
                   !request.Cookie.Crumbs.Contains(new KeyValuePair<String, String>("isAdmin", "")))
                    // Unkown cookie?!
                {

                    return new HTTPResponseBuilder(request) {
                                   HTTPStatusCode  = HTTPStatusCode.TemporaryRedirect,
                                   Location        = "/",
                                   Date            = DateTime.Now,
                                   Server          = HTTPServer.DefaultServerName,
                                   CacheControl    = "private, max-age=0, no-cache",
                                   Connection      = "close"
                               }.AsImmutable();

                }

                #endregion

                #region GET "/login...", but the user us already logged in...

                if ((request.URI == "/login"                    ||
                     request.URI == "/login/"                   ||
                     request.URI == "/login/index.html"         ||
                     request.URI == "/login/lost_password.html" ||
                     request.URI == "/login/set_password.html") &&
                     request.HTTPMethod == HTTPMethod.GET)
                {

                    if (request.Cookie      != null &&
                        request.Cookie.Name == CookieName)
                        // Unkown cookie?!
                    {

                        return new HTTPResponseBuilder(request) {
                                   HTTPStatusCode  = HTTPStatusCode.TemporaryRedirect,
                                   Location        = "/",
                                   Date            = DateTime.Now,
                                   Server          = HTTPServer.DefaultServerName,
                                   CacheControl    = "private, max-age=0, no-cache",
                                   Connection      = "close"
                               }.AsImmutable();

                    }

                }

                #endregion

                return null;

            });

            HTTPServer.Rewrite((server, request) => {

                #region /login

                if (request.URI == "/login" &&
                    request.HTTPMethod == HTTPMethod.GET)
                {

                    var NewRequest = new HTTPRequestBuilder(request);
                    NewRequest.URI = "/login/index.html";

                    return NewRequest;

                }

                #endregion

                #region /lost_password

                if (request.URI == "/lost_password" &&
                    request.HTTPMethod == HTTPMethod.GET)
                {

                    var NewRequest = new HTTPRequestBuilder(request);
                    NewRequest.URI = "/login/lost_password.html";

                    return NewRequest;

                }

                #endregion

                #region /set_password

                if (request.URI == "/set_password" &&
                    request.HTTPMethod == HTTPMethod.GET)
                {

                    var NewRequest = new HTTPRequestBuilder(request);
                    NewRequest.URI = "/login/set_password.html";

                    return NewRequest;

                }

                #endregion

                #region /admin

                if (request.URI == "/admin" &&
                    request.HTTPMethod == HTTPMethod.GET)
                {

                    var NewRequest = new HTTPRequestBuilder(request);
                    NewRequest.URI = "/admin/index.html";

                    return NewRequest;

                }

                #endregion

                return null;

            });

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
                                                      }.AsImmutable());

                                              User _User = null;
                                              if (!_Users.TryGetValue(VerificationToken.Login, out _User))
                                                  return Task.FromResult(
                                                      new HTTPResponseBuilder(Request) {
                                                          HTTPStatusCode  = HTTPStatusCode.NotFound,
                                                          Server          = HTTPServer.DefaultServerName,
                                                          ContentType     = HTTPContentType.HTML_UTF8,
                                                          Content         = ("Login not found!").ToUTF8Bytes(),
                                                          CacheControl    = "public",
                                                          Connection      = "close"
                                                      }.AsImmutable());

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
                                                      From = APIEMailAddress,
                                                      To = APIAdminEMails,
                                                      Subject = "New user activated: " + _User.Id.ToString() + " at " + DateTime.Now.ToString(),
                                                      Text = "New user activated: " + _User.Id.ToString() + " at " + DateTime.Now.ToString(),
                                                      Passphrase = APIPassphrase
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
                                                      }.AsImmutable());

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
                                                  }.AsImmutable());

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
                                              GetUsersAPIRessource("_header.html").SeekAndCopyTo(_MemoryStream, 3);
                                              GetUsersAPIRessource("users.index.html").SeekAndCopyTo(_MemoryStream, 3);
                                              GetUsersAPIRessource("_footer.html").SeekAndCopyTo(_MemoryStream, 3);

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
                                                      SetCookie       = CookieName + "=; Expires=" + DateTime.Now.ToRfc1123() +
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

                                              var NewUser = CreateUser(Id:          _Login,
                                                                       Password:          NewUserData.GetString("password"),
                                                                       EMail:             SimpleEMailAddress.Parse(NewUserData.GetString("email")),
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

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.POST,
                                         "/login",
                                         HTTPContentType.XWWWFormUrlEncoded,
                                         HTTPDelegate: Request => {

                                              //username=ahzf&password=sfdsdfsdf

                                              #region Check UTF8 text body...

                                              HTTPResponse _HTTPResponse  = null;
                                              String       LoginText      = null;
                                              if (!Request.TryParseUTF8StringRequestBody(HTTPContentType.XWWWFormUrlEncoded, out LoginText, out _HTTPResponse, AllowEmptyHTTPBody: false))
                                                  return Task.FromResult(_HTTPResponse);

                                              var LoginData = LoginText.DoubleSplit('&', '=');

                                              #endregion

                                              #region Verify the login

                                              String Login;

                                              if (!LoginData.TryGetValue("login", out Login) ||
                                                   Login.    IsNullOrEmpty())
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
                                                      }.AsImmutable());

                                              Login = HTTPTools.URLDecode(Login);

                                              if (Login.Length < MinLoginLenght)
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
                                                      }.AsImmutable());

                                              #endregion

                                              #region Verify the realm

                                              String Realm;

                                              LoginData.TryGetValue("realm", out Realm);

                                              if (Realm.IsNotNullOrEmpty())
                                                  Realm = HTTPTools.URLDecode(Realm);

                                              #endregion

                                              #region Verify the password

                                              String Password;

                                              if (!LoginData.TryGetValue("password", out Password) ||
                                                   Password. IsNullOrEmpty())
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
                                                      }.AsImmutable());

                                              Password = HTTPTools.URLDecode(Password);

                                              if (Password.Length < MinPasswordLenght)
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
                                                      }.AsImmutable());

                                              #endregion

                                              #region Check login and password

                                              User_Id       _UserId;
                                              LoginPassword _LoginPassword  = null;
                                              User          _User           = null;

                                              if (!User_Id.TryParse(Login, out _UserId) ||
                                                  !_LoginPasswords.TryGetValue(_UserId, out _LoginPassword) ||
                                                  !_Users.         TryGetValue(_UserId, out _User))

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
                                                      }.AsImmutable());


                                              if (!_LoginPassword.CheckPassword(Password))

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
                                                      }.AsImmutable());

                                              #endregion

                                              var SHA256Hash     = new SHA256Managed();
                                              var SecurityToken  = SHA256Hash.ComputeHash(
                                                                       String.Concat(
                                                                           Guid.NewGuid().ToString(),
                                                                           _LoginPassword.Login,
                                                                           _LoginPassword.Realm).
                                                                       ToUTF8Bytes()).
                                                                   ToHexString();

                                              return Task.FromResult(
                                                  new HTTPResponseBuilder(Request) {
                                                      HTTPStatusCode  = HTTPStatusCode.Created,
                                                      ContentType     = HTTPContentType.HTML_UTF8,
                                                      Content         = String.Concat(
                                                                            @"<!DOCTYPE html>",
                                                                            Environment.NewLine,
                                                                            @"<html><head><meta http-equiv=""refresh"" content=""0; url=/"" /></head></html>"
                                                                        ).ToUTF8Bytes(),
                                                      CacheControl    = "private",
                                                      SetCookie       = CookieName + "=login="    + _LoginPassword.Login.ToString().ToBase64() +
                                                                                  ":username=" + _User.Name.ToBase64() +
                                                                                (IsAdmin(_User) ? ":isAdmin" : "") +
                                                                             ":securitytoken=" + SecurityToken +
                                                                                  "; Expires=" + DateTime.Now.Add(_SignInSessionLifetime).ToRfc1123() +
                                                                                   (HTTPCookieDomain.IsNotNullOrEmpty()
                                                                                       ? "; Domain=" + HTTPCookieDomain
                                                                                       : "") +
                                                                                     "; Path=/",
                                                      // _gitlab_session=653i45j69051238907520q1350275575; path=/; secure; HttpOnly
                                                      Connection      = "close",
                                                      X_FrameOptions  = "DENY"
                                                  }.AsImmutable());

                                          });

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
                                                          CacheControl    = "private",
                                                          Connection      = "close"
                                                      }.AsImmutable());

                                              #endregion

                                              // The login is taken from the URI, not from the JSON!
                                              LoginData.SetProperty("username", Request.ParsedURIParameters[0]);

                                              #region Verify username

                                              if (LoginData.GetString("username").Length < MinLoginLenght)
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
                                                          CacheControl    = "private",
                                                          Connection      = "close"
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
                                                          CacheControl = "private",
                                                          Connection = "close"
                                                      }.AsImmutable());

                                              #endregion

                                              #region Verify password

                                              if (!LoginData.ContainsKey("password"))
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
                                                      }.AsImmutable());

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
                                                      }.AsImmutable());


                                              if (!_LoginPassword.CheckPassword(LoginData.GetString("password")))

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
                                                          //SetCookie       = "SocialOpenData=" +
                                                          //                       "; Expires=" + DateTime.Now.AddMinutes(-5).ToRfc1123() +
                                                          //                          "; Path=/",
                                                          CacheControl    = "private",
                                                          Connection      = "close"
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
                                                      SetCookie       = CookieName + "=login="    + _LoginPassword.Login.ToString().ToBase64() +
                                                                                  ":username=" + _User.Name.ToBase64() +
                                                                                (IsAdmin(_User) ? ":isAdmin" : "") +
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
                                                      SetCookie       = CookieName + "=; Expires=" + DateTime.Now.ToRfc1123() +
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
                                              GetUsersAPIRessource("_header.html").SeekAndCopyTo(_MemoryStream, 3);
                                              GetUsersAPIRessource("groups.index.html").SeekAndCopyTo(_MemoryStream, 3);
                                              GetUsersAPIRessource("_footer.html").SeekAndCopyTo(_MemoryStream, 3);

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

            HTTPServer.ITEM_EXISTS<Group_Id, Group>(UriTemplate: "/groups/{GroupId}",
                                                             ParseIdDelegate: Group_Id.TryParse,
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

            HTTPServer.ITEM_GET<Group_Id, Group>(UriTemplate: "/groups/{GroupId}",
                                                          ParseIdDelegate: Group_Id.TryParse,
                                                          ParseIdError: Text => "Invalid group identification '" + Text + "'!",
                                                          TryGetItemDelegate: _Groups.TryGetValue,
                                                          ItemFilterDelegate: group => group.IsPublic,
                                                          TryGetItemError: groupId => "Unknown group '" + groupId + "'!",
                                                          ToJSONDelegate: JSON.ToJSON);

            #endregion

            #endregion

        }

        #endregion


        #region (protected) GetUsersAPIRessource(Ressource)

        protected Stream GetUsersAPIRessource(String Ressource)
        {

            var DataStream = this.GetType().Assembly.GetManifestResourceStream(HTTPRoot + Ressource);

            if (DataStream == null)
                DataStream = RessourcesProvider(Ressource);

            return DataStream;

        }

        #endregion


        #region (private) WriteToLogfile(Command, JSON)

        private void WriteToLogfile(String   Command,
                                    JObject  JSON)
        {

            var _JObject   = new JObject(
                                 new JProperty(Command,       JSON),
                                 new JProperty("Writer",      SystemId),
                                 new JProperty("Timestamp",   DateTime.Now.ToIso8601()),
                                 new JProperty("Nonce",       Guid.NewGuid().ToString().Replace("-", "")),
                                 new JProperty("ParentHash",  CurrentHash)
                             );

            var SHA256     = new SHA256Managed();
            CurrentHash    = SHA256.ComputeHash(Encoding.Unicode.GetBytes(UserDB_RegEx.Replace(_JObject.ToString(), " "))).
                                    Select(value => String.Format("{0:x2}", value)).
                                    Aggregate();

            _JObject.Add(new JProperty("HashValue", CurrentHash));

            File.AppendAllText(DefaultUsersAPIFile,
                               UserDB_RegEx.Replace(_JObject.ToString(), " ") +
                               Environment.NewLine);

        }

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
        /// <param name="IsPublic">The user will be shown in user listings.</param>
        /// <param name="IsDisabled">The user will be shown in user listings.</param>
        /// <param name="IsAuthenticated">The user will not be shown in user listings, as its primary e-mail address is not yet authenticated.</param>
        public User CreateUser(User_Id             Id,
                               SimpleEMailAddress  EMail,
                               String              Password,
                               String              Name              = null,
                               String              PublicKeyRing     = null,
                               String              Telephone         = null,
                               I18NString          Description       = null,
                               Boolean             IsPublic          = true,
                               Boolean             IsDisabled        = false,
                               Boolean             IsAuthenticated   = false)
        {

            #region Initial checks

            if (Password.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Password), "The given password must not be null or empty!");

            #endregion

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
                                    IsPublic,
                                    IsDisabled,
                                    IsAuthenticated);

                WriteToLogfile("CreateUser", User.ToJSON());

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
        /// <param name="IsPublic">The user will be shown in user listings.</param>
        /// <param name="IsDisabled">The user will be shown in user listings.</param>
        /// <param name="IsAuthenticated">The user will not be shown in user listings, as its primary e-mail address is not yet authenticated.</param>
        public User CreateUserIfNotExists(User_Id             Id,
                                          SimpleEMailAddress  EMail,
                                          String              Password,
                                          String              Name              = null,
                                          String              PublicKeyRing     = null,
                                          String              Telephone         = null,
                                          I18NString          Description       = null,
                                          Boolean             IsPublic          = true,
                                          Boolean             IsDisabled        = false,
                                          Boolean             IsAuthenticated   = false)
        {

            #region Initial checks

            if (Password.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Password), "The given password must not be null or empty!");

            #endregion

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
                                  IsPublic,
                                  IsDisabled,
                                  IsAuthenticated);

            }

        }

        #endregion

        #region SetPassword(Login, Password, Realm = null)

        public void SetPassword(User_Id  Login,
                                String   Password,
                                String   Realm  = null)
        {

            lock (_Users)
            {

                if (!_LoginPasswords.ContainsKey(Login))
                    _LoginPasswords.Add(Login, new LoginPassword(Login, Password, Realm));
                else
                    _LoginPasswords[Login]   = new LoginPassword(Login, Password, Realm);

                File.AppendAllText(DefaultPasswordFile,
                                   String.Concat(Login,
                                                 ":",
                                                 Realm,
                                                 ":",
                                                 Password,
                                                 Environment.NewLine));

            }

        }

        public void SetPassword(User_Id       Login,
                                SecureString  Password,
                                String        Realm  = null)
        {

            lock (_Users)
            {

                if (!_LoginPasswords.ContainsKey(Login))
                    _LoginPasswords.Add(Login, new LoginPassword(Login, Password, Realm));
                else
                    _LoginPasswords[Login]   = new LoginPassword(Login, Password, Realm);

                File.AppendAllText(DefaultPasswordFile,
                   String.Concat(Login,
                                 ":",
                                 Realm,
                                 ":",
                                 Password,
                                 Environment.NewLine));

            }

        }

        #endregion

        #region Users

        protected readonly Dictionary<User_Id, User> _Users;

        public IEnumerable<User> Users
            => _Users.Values;

        #endregion


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

        #region AddToGroup(User, Edge, Group, Privacy = Public)

        public Boolean AddToGroup(User             User,
                                  User2GroupEdges  Edge,
                                  Group            Group,
                                  PrivacyLevel     Privacy = PrivacyLevel.Public)
        {

            if (!User.Edges(Group).Any(edge => edge == Edge))
            {

                User. AddOutgoingEdge(Edge, Group, Privacy);

                if (!Group.Edges(Group).Any(edge => edge == Edge))
                    Group.AddIncomingEdge(User, Edge,  Privacy);

                WriteToLogfile("AddUserToGroup",
                               new JObject(
                                   new JProperty("user",     User.Id.ToString()),
                                   new JProperty("realm",    User.Id.Realm),
                                   new JProperty("edge",     Edge.   ToString()),
                                   new JProperty("group",    Group.  ToString()),
                                   new JProperty("privacy",  Privacy.ToString())
                               ));

                return true;

            }

            return false;

        }

        #endregion

        #region IsAdmin(User)

        public Boolean IsAdmin(User User)

            => User.Groups(User2GroupEdges.IsAdmin).
                    Contains(Admins);

        #endregion


        #region CreateOrganization           (Id, Name = null, Description = null)

        public Organization CreateOrganization(Organization_Id  Id,
                                               I18NString       Name         = null,
                                               I18NString       Description  = null)
        {

            lock (_Organizations)
            {

                if (_Organizations.ContainsKey(Id))
                    throw new ArgumentException("The given organization identification already exists!", nameof(Id));


                var Organization = new Organization(Id,
                                                    Name,
                                                    Description);

                WriteToLogfile("CreateOrganization", Organization.ToJSON());

                return _Organizations.AddAndReturnValue(Organization.Id, Organization);

            }

        }

        #endregion

        #region CreateOrganizationIfNotExists(Id, Name = null, Description = null)

        public Organization CreateOrganizationIfNotExists(Organization_Id  Id,
                                                          I18NString       Name         = null,
                                                          I18NString       Description  = null)
        {

            lock (_Organizations)
            {

                if (_Organizations.ContainsKey(Id))
                    return _Organizations[Id];

                return CreateOrganization(Id,
                                          Name,
                                          Description);

            }

        }

        #endregion

        #region Organizations

        protected readonly Dictionary<Organization_Id, Organization> _Organizations;

        public IEnumerable<Organization> Organizations
            => _Organizations.Values;

        #endregion

        #region AddToOrganization(User, Edge, Organization, Privacy = Public)

        public Boolean AddToOrganization(User                    User,
                                         User2OrganizationEdges  Edge,
                                         Organization            Organization,
                                         PrivacyLevel            Privacy = PrivacyLevel.Public)
        {

            if (!User.Edges(Organization).Any(edge => edge == Edge))
            {

                User.AddOutgoingEdge(Edge, Organization, Privacy);

                if (!Organization.Edges(Organization).Any(edge => edge == Edge))
                    Organization.AddIncomingEdge(User, Edge,         Privacy);

                WriteToLogfile("AddUserToOrganization",
                               new JObject(
                                   new JProperty("user",          User.Id.     ToString()),
                                   new JProperty("realm",         User.Id.Realm),
                                   new JProperty("edge",          Edge.        ToString()),
                                   new JProperty("organization",  Organization.ToString()),
                                   new JProperty("privacy",       Privacy.     ToString())
                               ));

                return true;

            }

            return false;

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

        #region Messages

        protected readonly Dictionary<Message_Id, Message> _Messages;

        /// <summary>
        /// A collection of message.
        /// </summary>
        public IEnumerable<Message> Messages
            => _Messages.Values;

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
