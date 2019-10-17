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

using Telegram.Bot;

using Org.BouncyCastle.Bcpg.OpenPgp;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Styx.Arrows;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Warden;

using org.GraphDefined.OpenData.Postings;
using org.GraphDefined.OpenData.Notifications;

using com.GraphDefined.SMSApi.API;
using com.GraphDefined.SMSApi.API.Response;

#endregion

namespace org.GraphDefined.OpenData.Users
{

    public delegate Single PasswordQualityCheckDelegate(String Password);


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

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
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

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
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

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                };

                return false;

            }

            UserId = User_Id.TryParse(HTTPRequest.ParsedURIParameters[0], "");

            if (!UserId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid UserId!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            if (!UsersAPI.TryGet(UserId.Value, out User)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
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


        #region ParseOrganizationId(this HTTPRequest, UsersAPI, out OrganizationId,                   out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the user identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="UsersAPI">The Organizations API.</param>
        /// <param name="OrganizationId">The parsed unique user identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseOrganizationId(this HTTPRequest      HTTPRequest,
                                                  UsersAPI              UsersAPI,
                                                  out Organization_Id?  OrganizationId,
                                                  out HTTPResponse      HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest == null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (UsersAPI    == null)
                throw new ArgumentNullException(nameof(UsersAPI),     "The given Organizations API must not be null!");

            #endregion

            OrganizationId        = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURIParameters.Length < 1)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                };

                return false;

            }

            OrganizationId = Organization_Id.TryParse(HTTPRequest.ParsedURIParameters[0]);

            if (!OrganizationId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid OrganizationId!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseOrganization  (this HTTPRequest, UsersAPI, out OrganizationId, out Organization, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the user identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="UsersAPI">The Organizations API.</param>
        /// <param name="OrganizationId">The parsed unique user identification.</param>
        /// <param name="Organization">The resolved user.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseOrganization(this HTTPRequest      HTTPRequest,
                                                UsersAPI              UsersAPI,
                                                out Organization_Id?  OrganizationId,
                                                out Organization      Organization,
                                                out HTTPResponse      HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest == null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (UsersAPI    == null)
                throw new ArgumentNullException(nameof(UsersAPI),     "The given Organizations API must not be null!");

            #endregion

            OrganizationId        = null;
            Organization          = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURIParameters.Length < 1) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    Connection      = "close"
                };

                return false;

            }

            OrganizationId = Organization_Id.TryParse(HTTPRequest.ParsedURIParameters[0]);

            if (!OrganizationId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid OrganizationId!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            if (!UsersAPI.TryGet(OrganizationId.Value, out Organization)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = DateTime.UtcNow,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown OrganizationId!"" }".ToUTF8Bytes(),
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
    public class UsersAPI : HTTPAPI
    {

        #region (class) UserContext

        public class UserContext : IDisposable
        {

            public User_Id  Current    { get; }
            public User_Id? Previous   { get; }


            public UserContext(User_Id UserId)
            {

                this.Previous                  = CurrentAsyncLocalUserId.Value;
                this.Current                   = UserId;
                CurrentAsyncLocalUserId.Value  = UserId;

            }

            public void Dispose()
            {
                CurrentAsyncLocalUserId.Value = Previous;
            }

        }

        #endregion

        #region Data

        private static readonly SemaphoreSlim  LogFileSemaphore        = new SemaphoreSlim(1, 1);
        //private static readonly SemaphoreSlim  NotificationsSemaphore  = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim  UsersSemaphore          = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim  OrganizationsSemaphore  = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim  GroupsSemaphore         = new SemaphoreSlim(1, 1);

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
        public  static readonly   PasswordQualityCheckDelegate        DefaultPasswordQualityCheck    = password => password.Length > 8 ? 1.0f : 0;

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
        public  const             String                              DefaultHTTPCookiesFile         = "UsersAPI_HTTPCookies.db";
        public  const             String                              DefaultPasswordResetsFile      = "UsersAPI_PasswordResets.db";
        //public  const             String                              DefaultGroupDBFile             = "UsersAPI_Groups.db";
        //public  const             String                              DefaultUser2GroupDBFile        = "UsersAPI_User2Group.db";
        public  const             String                              AdminGroupName                 = "Admins";


        protected readonly SMSAPI                                      _SMSAPI;

        protected readonly Dictionary<SecurityToken_Id, SecurityToken> HTTPCookies;
        protected readonly Dictionary<SecurityToken_Id, PasswordReset> PasswordResets;

        public static Organization NoOwner;

        //private readonly Queue<NotificationMessage> _NotificationMessages;

        //public IEnumerable<NotificationMessage> NotificationMessages
        //    => _NotificationMessages;

        #endregion

        #region Properties

        public HashSet<String>           DevMachines                { get; set; }

        public String                    LoggingPath                { get; }
        public String                    UsersAPIPath               { get; }
        public String                    HTTPSSEPath                { get; }
        public String                    HTTPRequestsPath           { get; }
        public String                    HTTPResponsesPath          { get; }
        public String                    NotificationsPath          { get; }
        public String                    MetricsPath                { get; }

        /// <summary>
        /// The current async local user identification to simplify API usage.
        /// </summary>
        protected internal static AsyncLocal<User_Id?> CurrentAsyncLocalUserId = new AsyncLocal<User_Id?>();

        public User                      Robot                      { get; }

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


        #region SMSAPI

        /// <summary>
        /// The (default) SMS sender name.
        /// </summary>
        public String                    SMSSenderName              { get; }

        /// <summary>
        /// A list of admin SMS phonenumbers.
        /// </summary>
        public IEnumerable<PhoneNumber>  APIAdminSMS                { get; }

        #endregion


        /// <summary>
        /// The Telegram API access token of the bot.
        /// </summary>
        public String                    TelegramBotToken           { get; }

        /// <summary>
        /// The Telegram API client.
        /// </summary>
        public TelegramBotClient         TelegramAPI                { get; }

        /// <summary>
        /// The Telegram user store.
        /// </summary>
        public TelegramStore             TelegramStore              { get; }


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
        public delegate EMail NewUserSignUpEMailCreatorDelegate(User_Id           UserId,
                                                                EMailAddress      EMailAddress,
                                                                String            Username,
                                                                SecurityToken_Id  SecurityToken,
                                                                Boolean           Use2FactorAuth,
                                                                String            DNSHostname,
                                                                Languages         Language);

        /// <summary>
        /// A delegate for sending a sign-up e-mail to a new user.
        /// </summary>
        public NewUserSignUpEMailCreatorDelegate NewUserSignUpEMailCreator { get; }

        #endregion

        #region NewUserWelcomeEMailCreator

        /// <summary>
        /// A delegate for sending a welcome e-mail to a new user.
        /// </summary>
        public delegate EMail NewUserWelcomeEMailCreatorDelegate(User_Id       UserId,
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
        public delegate EMail ResetPasswordEMailCreatorDelegate(User_Id           UserId,
                                                                EMailAddress      EMailAddress,
                                                                String            Username,
                                                                SecurityToken_Id  SecurityToken,
                                                                Boolean           Use2FactorAuth,
                                                                String            DNSHostname,
                                                                Languages         Language);

        /// <summary>
        /// A delegate for sending a reset password e-mail to a user.
        /// </summary>
        public ResetPasswordEMailCreatorDelegate ResetPasswordEMailCreator { get; }

        #endregion

        #region PasswordChangedEMailCreator

        /// <summary>
        /// A delegate for sending a password changed e-mail to a user.
        /// </summary>
        public delegate EMail PasswordChangedEMailCreatorDelegate(User_Id       UserId,
                                                                  EMailAddress  EMail,
                                                                  String        Username,
                                                                  String        DNSHostname,
                                                                  Languages     Language);

        /// <summary>
        /// A delegate for sending a password changed e-mail to a user.
        /// </summary>
        public PasswordChangedEMailCreatorDelegate PasswordChangedEMailCreator { get; }

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

        #region PasswordQualityCheck

        /// <summary>
        /// A delegate to ensure a minimal password quality.
        /// </summary>
        public PasswordQualityCheckDelegate PasswordQualityCheck { get; }

        #endregion


        /// <summary>
        /// The current hash value of the API.
        /// </summary>
        public String        CurrentDatabaseHashValue   { get; private set; }


        /// <summary>
        /// Disable the log file.
        /// </summary>
        public Boolean       DisableLogfile             { get; }

        /// <summary>
        /// Disable external notifications.
        /// </summary>
        public Boolean       DisableNotifications       { get; set; }

        /// <summary>
        /// The logfile of this API.
        /// </summary>
        public String        LogfileName                { get; }


        public Warden        Warden                     { get; }

        public List<BlogPosting> BlogPostings           { get; }


        #endregion

        #region Events

        #region (protected internal) AddUserRequest (Request)

        /// <summary>
        /// An event sent whenever add user request was received.
        /// </summary>
        public HTTPRequestLogEvent OnAddUserRequest = new HTTPRequestLogEvent();

        /// <summary>
        /// An event sent whenever add user request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task AddUserRequest(DateTime     Timestamp,
                                               HTTPAPI      API,
                                               HTTPRequest  Request)

            => OnAddUserRequest?.WhenAll(Timestamp,
                                         API ?? this,
                                         Request);

        #endregion

        #region (protected internal) AddUserResponse(Response)

        /// <summary>
        /// An event sent whenever a response on an add user request was sent.
        /// </summary>
        public HTTPResponseLogEvent OnAddUserResponse = new HTTPResponseLogEvent();

        /// <summary>
        /// An event sent whenever a response on an add user request was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task AddUserResponse(DateTime      Timestamp,
                                                HTTPAPI       API,
                                                HTTPRequest   Request,
                                                HTTPResponse  Response)

            => OnAddUserResponse?.WhenAll(Timestamp,
                                          API ?? this,
                                          Request,
                                          Response);

        #endregion


        #region (protected internal) SetUserRequest (Request)

        /// <summary>
        /// An event sent whenever set user request was received.
        /// </summary>
        public HTTPRequestLogEvent OnSetUserRequest = new HTTPRequestLogEvent();

        /// <summary>
        /// An event sent whenever set user request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task SetUserRequest(DateTime     Timestamp,
                                               HTTPAPI      API,
                                               HTTPRequest  Request)

            => OnSetUserRequest?.WhenAll(Timestamp,
                                         API ?? this,
                                         Request);

        #endregion

        #region (protected internal) SetUserResponse(Response)

        /// <summary>
        /// An event sent whenever a response on a set user request was sent.
        /// </summary>
        public HTTPResponseLogEvent OnSetUserResponse = new HTTPResponseLogEvent();

        /// <summary>
        /// An event sent whenever a response on a set user request was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task SetUserResponse(DateTime      Timestamp,
                                                HTTPAPI       API,
                                                HTTPRequest   Request,
                                                HTTPResponse  Response)

            => OnSetUserResponse?.WhenAll(Timestamp,
                                          API ?? this,
                                          Request,
                                          Response);

        #endregion


        #region (protected internal) ChangePasswordRequest (Request)

        /// <summary>
        /// An event sent whenever set user request was received.
        /// </summary>
        public HTTPRequestLogEvent OnChangePasswordRequest = new HTTPRequestLogEvent();

        /// <summary>
        /// An event sent whenever set user request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task ChangePasswordRequest(DateTime     Timestamp,
                                                      HTTPAPI      API,
                                                      HTTPRequest  Request)

            => OnChangePasswordRequest?.WhenAll(Timestamp,
                                                API ?? this,
                                                Request);

        #endregion

        #region (protected internal) ChangePasswordResponse(Response)

        /// <summary>
        /// An event sent whenever a response on a set user request was sent.
        /// </summary>
        public HTTPResponseLogEvent OnChangePasswordResponse = new HTTPResponseLogEvent();

        /// <summary>
        /// An event sent whenever a response on a set user request was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task ChangePasswordResponse(DateTime      Timestamp,
                                                       HTTPAPI       API,
                                                       HTTPRequest   Request,
                                                       HTTPResponse  Response)

            => OnChangePasswordResponse?.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request,
                                                 Response);

        #endregion


        #region (protected internal) SetUserNotificationsRequest    (Request)

        /// <summary>
        /// An event sent whenever set user notifications request was received.
        /// </summary>
        public HTTPRequestLogEvent OnSetUserNotificationsRequest = new HTTPRequestLogEvent();

        /// <summary>
        /// An event sent whenever set user notifications request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task SetUserNotificationsRequest(DateTime     Timestamp,
                                                            HTTPAPI      API,
                                                            HTTPRequest  Request)

            => OnSetUserNotificationsRequest?.WhenAll(Timestamp,
                                                      API ?? this,
                                                      Request);

        #endregion

        #region (protected internal) SetUserNotificationsResponse   (Response)

        /// <summary>
        /// An event sent whenever a response on a set user notifications request was sent.
        /// </summary>
        public HTTPResponseLogEvent OnSetUserNotificationsResponse = new HTTPResponseLogEvent();

        /// <summary>
        /// An event sent whenever a response on a set user notifications request was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task SetUserNotificationsResponse(DateTime      Timestamp,
                                                             HTTPAPI       API,
                                                             HTTPRequest   Request,
                                                             HTTPResponse  Response)

            => OnSetUserNotificationsResponse?.WhenAll(Timestamp,
                                                       API ?? this,
                                                       Request,
                                                       Response);

        #endregion

        #region (protected internal) DeleteUserNotificationsRequest (Request)

        /// <summary>
        /// An event sent whenever set user notifications request was received.
        /// </summary>
        public HTTPRequestLogEvent OnDeleteUserNotificationsRequest = new HTTPRequestLogEvent();

        /// <summary>
        /// An event sent whenever set user notifications request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task DeleteUserNotificationsRequest(DateTime     Timestamp,
                                                               HTTPAPI      API,
                                                               HTTPRequest  Request)

            => OnDeleteUserNotificationsRequest?.WhenAll(Timestamp,
                                                         API ?? this,
                                                         Request);

        #endregion

        #region (protected internal) DeleteUserNotificationsResponse(Response)

        /// <summary>
        /// An event sent whenever a response on a set user notifications request was sent.
        /// </summary>
        public HTTPResponseLogEvent OnDeleteUserNotificationsResponse = new HTTPResponseLogEvent();

        /// <summary>
        /// An event sent whenever a response on a set user notifications request was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task DeleteUserNotificationsResponse(DateTime      Timestamp,
                                                                HTTPAPI       API,
                                                                HTTPRequest   Request,
                                                                HTTPResponse  Response)

            => OnDeleteUserNotificationsResponse?.WhenAll(Timestamp,
                                                          API ?? this,
                                                          Request,
                                                          Response);

        #endregion


        #region (protected internal) ImpersonateUserRequest (Request)

        /// <summary>
        /// An event sent whenever an impersonate user request was received.
        /// </summary>
        public HTTPRequestLogEvent OnImpersonateUserRequest = new HTTPRequestLogEvent();

        /// <summary>
        /// An event sent whenever an impersonate user request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task ImpersonateUserRequest(DateTime     Timestamp,
                                                       HTTPAPI      API,
                                                       HTTPRequest  Request)

            => OnImpersonateUserRequest?.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request);

        #endregion

        #region (protected internal) ImpersonateUserResponse(Response)

        /// <summary>
        /// An event sent whenever a response on an impersonate user request was sent.
        /// </summary>
        public HTTPResponseLogEvent OnImpersonateUserResponse = new HTTPResponseLogEvent();

        /// <summary>
        /// An event sent whenever a response on an impersonate user request was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task ImpersonateUserResponse(DateTime      Timestamp,
                                                        HTTPAPI       API,
                                                        HTTPRequest   Request,
                                                        HTTPResponse  Response)

            => OnImpersonateUserResponse?.WhenAll(Timestamp,
                                                  API ?? this,
                                                  Request,
                                                  Response);

        #endregion


        #region (protected internal) AddOrganizationRequest (Request)

        /// <summary>
        /// An event sent whenever add organization request was received.
        /// </summary>
        public HTTPRequestLogEvent OnAddOrganizationRequest = new HTTPRequestLogEvent();

        /// <summary>
        /// An event sent whenever add organization request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task AddOrganizationRequest(DateTime     Timestamp,
                                                       HTTPAPI      API,
                                                       HTTPRequest  Request)

            => OnAddOrganizationRequest?.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request);

        #endregion

        #region (protected internal) AddOrganizationResponse(Response)

        /// <summary>
        /// An event sent whenever a response on an add organization request was sent.
        /// </summary>
        public HTTPResponseLogEvent OnAddOrganizationResponse = new HTTPResponseLogEvent();

        /// <summary>
        /// An event sent whenever a response on an add organization request was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task AddOrganizationResponse(DateTime      Timestamp,
                                                        HTTPAPI       API,
                                                        HTTPRequest   Request,
                                                        HTTPResponse  Response)

            => OnAddOrganizationResponse?.WhenAll(Timestamp,
                                                  API ?? this,
                                                  Request,
                                                  Response);

        #endregion


        #region (protected internal) SetOrganizationHTTPRequest (Request)

        /// <summary>
        /// An event sent whenever Set organization request was received.
        /// </summary>
        public HTTPRequestLogEvent OnSetOrganizationHTTPRequest = new HTTPRequestLogEvent();

        /// <summary>
        /// An event sent whenever Set organization request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task SetOrganizationHTTPRequest(DateTime     Timestamp,
                                                           HTTPAPI      API,
                                                           HTTPRequest  Request)

            => OnSetOrganizationHTTPRequest?.WhenAll(Timestamp,
                                                     API ?? this,
                                                     Request);

        #endregion

        #region (protected internal) SetOrganizationHTTPResponse(Response)

        /// <summary>
        /// An event sent whenever a response on an Set organization request was sent.
        /// </summary>
        public HTTPResponseLogEvent OnSetOrganizationHTTPResponse = new HTTPResponseLogEvent();

        /// <summary>
        /// An event sent whenever a response on an Set organization request was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task SetOrganizationHTTPResponse(DateTime      Timestamp,
                                                            HTTPAPI       API,
                                                            HTTPRequest   Request,
                                                            HTTPResponse  Response)

            => OnSetOrganizationHTTPResponse?.WhenAll(Timestamp,
                                                      API ?? this,
                                                      Request,
                                                      Response);

        #endregion


        #region (protected internal) DeleteOrganizationRequest (Request)

        /// <summary>
        /// An event sent whenever delete organization request was received.
        /// </summary>
        public HTTPRequestLogEvent OnDeleteOrganizationRequest = new HTTPRequestLogEvent();

        /// <summary>
        /// An event sent whenever delete organization request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task DeleteOrganizationRequest(DateTime     Timestamp,
                                                       HTTPAPI      API,
                                                       HTTPRequest  Request)

            => OnDeleteOrganizationRequest?.WhenAll(Timestamp,
                                                 API ?? this,
                                                 Request);

        #endregion

        #region (protected internal) DeleteOrganizationResponse(Response)

        /// <summary>
        /// An event sent whenever a response on a delete organization request was sent.
        /// </summary>
        public HTTPResponseLogEvent OnDeleteOrganizationResponse = new HTTPResponseLogEvent();

        /// <summary>
        /// An event sent whenever a response on a delete organization request was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task DeleteOrganizationResponse(DateTime      Timestamp,
                                                        HTTPAPI       API,
                                                        HTTPRequest   Request,
                                                        HTTPResponse  Response)

            => OnDeleteOrganizationResponse?.WhenAll(Timestamp,
                                                  API ?? this,
                                                  Request,
                                                  Response);

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
        /// <param name="ServiceName">The name of the service.</param>
        /// <param name="BaseURL">The base url of the service.</param>
        /// <param name="URLPathPrefix">A common prefix for all URLs.</param>
        /// 
        /// <param name="ServerCertificateSelector">An optional delegate to select a SSL/TLS server certificate.</param>
        /// <param name="ClientCertificateValidator">An optional delegate to verify the SSL/TLS client certificate used for authentication.</param>
        /// <param name="ClientCertificateSelector">An optional delegate to select the SSL/TLS client certificate used for authentication.</param>
        /// <param name="AllowedTLSProtocols">The SSL/TLS protocol(s) allowed for this connection.</param>
        /// 
        /// <param name="APIEMailAddress">An e-mail address for this API.</param>
        /// <param name="APIPassphrase">A GPG passphrase for this API.</param>
        /// <param name="APIAdminEMails">A list of admin e-mail addresses.</param>
        /// <param name="APISMTPClient">A SMTP client for sending e-mails.</param>
        /// 
        /// <param name="SMSAPICredentials">The credentials for the SMS API.</param>
        /// <param name="SMSSenderName">The (default) SMS sender name.</param>
        /// <param name="APIAdminSMS">A list of admin SMS phonenumbers.</param>
        /// 
        /// <param name="TelegramBotToken">The Telegram API access token of the bot.</param>
        /// 
        /// <param name="CookieName">The name of the HTTP Cookie for authentication.</param>
        /// <param name="Language">The main language of the API.</param>
        /// <param name="LogoImage">The logo of the website.</param>
        /// <param name="NewUserSignUpEMailCurrentUserId">A delegate for sending a sign-up e-mail to a new user.</param>
        /// <param name="NewUserWelcomeEMailCurrentUserId">A delegate for sending a welcome e-mail to a new user.</param>
        /// <param name="ResetPasswordEMailCurrentUserId">A delegate for sending a reset password e-mail to a user.</param>
        /// <param name="MinUserNameLenght">The minimal user name length.</param>
        /// <param name="MinRealmLenght">The minimal realm length.</param>
        /// <param name="PasswordQualityCheck">A delegate to ensure a minimal password quality.</param>
        /// <param name="SignInSessionLifetime">The sign-in session lifetime.</param>
        /// 
        /// <param name="ServerThreadName">The optional name of the TCP server thread.</param>
        /// <param name="ServerThreadPriority">The optional priority of the TCP server thread.</param>
        /// <param name="ServerThreadIsBackground">Whether the TCP server thread is a background thread or not.</param>
        /// <param name="ConnectionIdBuilder">An optional delegate to build a connection identification based on IP socket information.</param>
        /// <param name="ConnectionThreadsNameBuilder">An optional delegate to set the name of the TCP connection threads.</param>
        /// <param name="ConnectionThreadsPriorityBuilder">An optional delegate to set the priority of the TCP connection threads.</param>
        /// <param name="ConnectionThreadsAreBackground">Whether the TCP connection threads are background threads or not (default: yes).</param>
        /// <param name="ConnectionTimeout">The TCP client timeout for all incoming client connections in seconds (default: 30 sec).</param>
        /// <param name="MaxClientConnections">The maximum number of concurrent TCP client connections (default: 4096).</param>
        /// 
        /// <param name="SkipURLTemplates">Skip URL templates.</param>
        /// <param name="DisableNotifications">Disable external notifications.</param>
        /// <param name="DisableLogfile">Disable the log file.</param>
        /// <param name="LoggingPath">The path for all logfiles.</param>
        /// <param name="LogfileName">The name of the logfile for this API.</param>
        /// <param name="DNSClient">The DNS client of the API.</param>
        /// <param name="Autostart">Whether to start the API automatically.</param>
        public UsersAPI(String                               HTTPServerName                     = DefaultHTTPServerName,
                        IPPort?                              HTTPServerPort                     = null,
                        HTTPHostname?                        HTTPHostname                       = null,
                        String                               ServiceName                        = DefaultServiceName,
                        String                               BaseURL                            = "",
                        HTTPPath?                            URLPathPrefix                      = null,

                        ServerCertificateSelectorDelegate    ServerCertificateSelector          = null,
                        RemoteCertificateValidationCallback  ClientCertificateValidator         = null,
                        LocalCertificateSelectionCallback    ClientCertificateSelector          = null,
                        SslProtocols                         AllowedTLSProtocols                = SslProtocols.Tls12,

                        EMailAddress                         APIEMailAddress                    = null,
                        String                               APIPassphrase                      = null,
                        EMailAddressList                     APIAdminEMails                     = null,
                        SMTPClient                           APISMTPClient                      = null,

                        Credentials                          SMSAPICredentials                  = null,
                        String                               SMSSenderName                      = null,
                        IEnumerable<PhoneNumber>             APIAdminSMS                        = null,

                        String                               TelegramBotToken                   = null,

                        HTTPCookieName?                      CookieName                         = null,
                        Languages                            Language                           = DefaultLanguage,
                        String                               LogoImage                          = null,
                        NewUserSignUpEMailCreatorDelegate    NewUserSignUpEMailCreator          = null,
                        NewUserWelcomeEMailCreatorDelegate   NewUserWelcomeEMailCreator         = null,
                        ResetPasswordEMailCreatorDelegate    ResetPasswordEMailCreator          = null,
                        PasswordChangedEMailCreatorDelegate  PasswordChangedEMailCreator        = null,
                        Byte                                 MinUserNameLenght                  = DefaultMinUserNameLenght,
                        Byte                                 MinRealmLenght                     = DefaultMinRealmLenght,
                        PasswordQualityCheckDelegate         PasswordQualityCheck               = null,
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

                        Boolean                              SkipURLTemplates                   = false,
                        Boolean                              DisableNotifications               = false,
                        Boolean                              DisableLogfile                     = false,
                        String                               LoggingPath                        = null,
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
                   ServiceName,
                   BaseURL,
                   URLPathPrefix,

                   APIEMailAddress,
                   APIPassphrase,
                   APIAdminEMails,
                   APISMTPClient,

                   SMSAPICredentials,
                   SMSSenderName,
                   APIAdminSMS,

                   TelegramBotToken,

                   CookieName,
                   Language,
                   LogoImage,
                   NewUserSignUpEMailCreator,
                   NewUserWelcomeEMailCreator,
                   ResetPasswordEMailCreator,
                   PasswordChangedEMailCreator,
                   MinUserNameLenght,
                   MinRealmLenght,
                   PasswordQualityCheck,
                   SignInSessionLifetime,

                   SkipURLTemplates,
                   DisableNotifications,
                   DisableLogfile,
                   LoggingPath,
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
        /// <param name="ServiceName">The name of the service.</param>
        /// <param name="BaseURL">The base URL of the service.</param>
        /// <param name="URLPathPrefix">A common prefix for all URLs.</param>
        /// 
        /// <param name="APIEMailAddress">An e-mail address for this API.</param>
        /// <param name="APIPassphrase">A GPG passphrase for this API.</param>
        /// <param name="APIAdminEMails">A list of admin e-mail addresses.</param>
        /// <param name="APISMTPClient">A SMTP client for sending e-mails.</param>
        /// 
        /// <param name="SMSAPICredentials">The credentials for the SMS API.</param>
        /// <param name="SMSSenderName">The (default) SMS sender name.</param>
        /// <param name="APIAdminSMS">A list of admin SMS phonenumbers.</param>
        /// 
        /// <param name="TelegramBotToken">The Telegram API access token of the bot.</param>
        /// 
        /// <param name="CookieName">The name of the HTTP Cookie for authentication.</param>
        /// <param name="Language">The main language of the API.</param>
        /// <param name="LogoImage">The logo of the website.</param>
        /// <param name="NewUserSignUpEMailCurrentUserId">A delegate for sending a sign-up e-mail to a new user.</param>
        /// <param name="NewUserWelcomeEMailCurrentUserId">A delegate for sending a welcome e-mail to a new user.</param>
        /// <param name="ResetPasswordEMailCurrentUserId">A delegate for sending a reset password e-mail to a user.</param>
        /// <param name="MinUserNameLenght">The minimal user name length.</param>
        /// <param name="MinRealmLenght">The minimal realm length.</param>
        /// <param name="PasswordQualityCheck">A delegate to ensure a minimal password quality.</param>
        /// <param name="SignInSessionLifetime">The sign-in session lifetime.</param>
        /// 
        /// <param name="SkipURLTemplates">Skip URI templates.</param>
        /// <param name="DisableNotifications">Disable external notifications.</param>
        /// <param name="DisableLogfile">Disable the log file.</param>
        /// <param name="LogfileName">The name of the logfile for this API.</param>
        protected UsersAPI(HTTPServer                           HTTPServer,
                           HTTPHostname?                        HTTPHostname                  = null,
                           String                               ServiceName                   = DefaultServiceName,
                           String                               BaseURL                       = "",
                           HTTPPath?                            URLPathPrefix                 = null,

                           EMailAddress                         APIEMailAddress               = null,
                           String                               APIPassphrase                 = null,
                           EMailAddressList                     APIAdminEMails                = null,
                           SMTPClient                           APISMTPClient                 = null,

                           Credentials                          SMSAPICredentials             = null,
                           String                               SMSSenderName                 = null,
                           IEnumerable<PhoneNumber>             APIAdminSMS                   = null,

                           String                               TelegramBotToken              = null,

                           HTTPCookieName?                      CookieName                    = null,
                           Languages                            Language                      = DefaultLanguage,
                           String                               LogoImage                     = null,
                           NewUserSignUpEMailCreatorDelegate    NewUserSignUpEMailCreator     = null,
                           NewUserWelcomeEMailCreatorDelegate   NewUserWelcomeEMailCreator    = null,
                           ResetPasswordEMailCreatorDelegate    ResetPasswordEMailCreator     = null,
                           PasswordChangedEMailCreatorDelegate  PasswordChangedEMailCreator   = null,
                           Byte                                 MinUserNameLenght             = DefaultMinUserNameLenght,
                           Byte                                 MinRealmLenght                = DefaultMinRealmLenght,
                           PasswordQualityCheckDelegate         PasswordQualityCheck          = null,
                           TimeSpan?                            SignInSessionLifetime         = null,

                           Boolean                              SkipURLTemplates              = false,
                           Boolean                              DisableNotifications          = false,
                           Boolean                              DisableLogfile                = false,
                           String                               LoggingPath                   = null,
                           String                               LogfileName                   = DefaultLogfileName)

            : base(HTTPServer,
                   HTTPHostname,
                   ServiceName,
                   BaseURL,
                   URLPathPrefix)

        {

            #region Init data

            this.LoggingPath                  = LoggingPath ?? Directory.GetCurrentDirectory();

            if (!this.LoggingPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                this.LoggingPath += Path.DirectorySeparatorChar;

            this.DevMachines                  = new HashSet<String>();
            this.UsersAPIPath                 = this.LoggingPath + "UsersAPI"      + Path.DirectorySeparatorChar;
            this.HTTPRequestsPath             = this.LoggingPath + "HTTPRequests"  + Path.DirectorySeparatorChar;
            this.HTTPResponsesPath            = this.LoggingPath + "HTTPResponses" + Path.DirectorySeparatorChar;
            this.HTTPSSEPath                  = this.LoggingPath + "HTTPSSEs"      + Path.DirectorySeparatorChar;
            this.NotificationsPath            = this.LoggingPath + "Notifications" + Path.DirectorySeparatorChar;
            this.MetricsPath                  = this.LoggingPath + "Metrics"       + Path.DirectorySeparatorChar;

            if (!DisableLogfile)
            {
                Directory.CreateDirectory(this.LoggingPath);
                Directory.CreateDirectory(this.UsersAPIPath);
                Directory.CreateDirectory(this.HTTPRequestsPath);
                Directory.CreateDirectory(this.HTTPResponsesPath);
                Directory.CreateDirectory(this.HTTPSSEPath);
                Directory.CreateDirectory(this.NotificationsPath);
                Directory.CreateDirectory(this.MetricsPath);
            }

            this.Robot                        = new User(Id:               User_Id.Parse("robot"),
                                                         EMail:            APIEMailAddress.Address,
                                                         Name:             APIEMailAddress.OwnerName,
                                                         PublicKeyRing:    APIEMailAddress.PublicKeyRing,
                                                         SecretKeyRing:    APIEMailAddress.SecretKeyRing,
                                                         Description:      I18NString.Create(Languages.eng, "API robot"),
                                                         PrivacyLevel:     PrivacyLevel.World,
                                                         IsAuthenticated:  true);

            CurrentAsyncLocalUserId.Value     = Robot.Id;

            this.APIPassphrase                = APIPassphrase;
            this.APIAdminEMails               = APIAdminEMails;
            this.APISMTPClient                = APISMTPClient;

            this.CookieName                   = CookieName                 ?? DefaultCookieName;
            this.Language                     = Language;
            this._LogoImage                   = LogoImage;
            this.NewUserSignUpEMailCreator    = NewUserSignUpEMailCreator  ?? throw new ArgumentNullException(nameof(NewUserSignUpEMailCreator),   "NewUserSignUpEMailCreator!");
            this.NewUserWelcomeEMailCreator   = NewUserWelcomeEMailCreator ?? throw new ArgumentNullException(nameof(NewUserWelcomeEMailCreator),  "NewUserWelcomeEMailCreator!");
            this.ResetPasswordEMailCreator    = ResetPasswordEMailCreator  ?? throw new ArgumentNullException(nameof(ResetPasswordEMailCreator),   "ResetPasswordEMailCreator!");
            this.PasswordChangedEMailCreator  = PasswordChangedEMailCreator;
            this.MinLoginLenght               = MinUserNameLenght;
            this.MinRealmLenght               = MinRealmLenght;
            this.PasswordQualityCheck         = PasswordQualityCheck       ?? DefaultPasswordQualityCheck;
            this.SignInSessionLifetime        = SignInSessionLifetime      ?? DefaultSignInSessionLifetime;

            this._DataLicenses                = new Dictionary<DataLicense_Id,  DataLicense>();
            this._Users                       = new Dictionary<User_Id,         User>();
            this._Groups                      = new Dictionary<Group_Id,        Group>();
            this._Organizations               = new Dictionary<Organization_Id, Organization>();
            this._Messages                    = new Dictionary<Message_Id,      Message>();

            this._LoginPasswords              = new Dictionary<User_Id,         LoginPassword>();
            this._VerificationTokens          = new List<VerificationToken>();

            this._DNSClient                   = HTTPServer.DNSClient;

            this.HTTPCookies                  = new Dictionary<SecurityToken_Id, SecurityToken>();
            this.PasswordResets               = new Dictionary<SecurityToken_Id, PasswordReset>();

            this.DisableNotifications         = DisableNotifications;
            this.DisableLogfile               = DisableLogfile;
            this.LogfileName                  = this.UsersAPIPath + (LogfileName ?? DefaultLogfileName);

            this._APIKeys                     = new Dictionary<APIKey, APIKeyInfo>();

            //this._NotificationMessages        = new Queue<NotificationMessage>();

            #endregion

            this.Admins  = CreateGroupIfNotExists(Group_Id.Parse(AdminGroupName),
                                                  Robot.Id,
                                                  I18NString.Create(Languages.eng, AdminGroupName),
                                                  I18NString.Create(Languages.eng, "All admins of this API.")).Result;

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

            ReadDatabaseFiles().Wait();

            NoOwner = CreateOrganizationIfNotExists(Organization_Id.Parse("NoOwner"), CurrentUserId: Robot.Id).Result;

            if (SMSAPICredentials != null)
            {
                this._SMSAPI            = new SMSAPI(Credentials: SMSAPICredentials);
                this.SMSSenderName      = SMSSenderName;
                this.APIAdminSMS        = APIAdminSMS;
            }

            this.TelegramBotToken       = TelegramBotToken;

            if (this.TelegramBotToken.IsNeitherNullNorEmpty())
            {
                this.TelegramAPI        = new TelegramBotClient(this.TelegramBotToken);
                this.TelegramStore      = new TelegramStore    (this.TelegramAPI);
                this.TelegramAPI.OnMessage += TelegramStore.ReceiveTelegramMessage;
                this.TelegramAPI.OnMessage += ReceiveTelegramMessage;
                this.TelegramAPI.StartReceiving();
            }


            this.Warden = new Warden(InitialDelay: TimeSpan.FromMinutes(3));

            this.BlogPostings = new List<BlogPosting>();

            if (!SkipURLTemplates)
                RegisterURITemplates();

            DebugX.Log("UsersAPI started...");

        }

        #endregion

        #endregion


        #region (static) AttachToHTTPAPI(HTTPServer, URIPrefix = "/", ...)

        /// <summary>
        /// Attach this Open Data HTTP API to the given HTTP server.
        /// </summary>
        /// <param name="HTTPServer">An existing HTTP server.</param>
        /// <param name="HTTPHostname">The HTTP hostname for all URIs within this API.</param>
        /// <param name="ServiceName">The name of the service.</param>
        /// <param name="BaseURL">The base URL of the service.</param>
        /// <param name="URLPathPrefix">A common prefix for all URLs.</param>
        /// 
        /// <param name="APIEMailAddress">An e-mail address for this API.</param>
        /// <param name="APIPassphrase">A GPG passphrase for this API.</param>
        /// <param name="APIAdminEMails">A list of admin e-mail addresses.</param>
        /// <param name="APISMTPClient">A SMTP client for sending e-mails.</param>
        /// 
        /// <param name="SMSAPICredentials">The credentials for the SMS API.</param>
        /// <param name="SMSSenderName">The (default) SMS sender name.</param>
        /// <param name="APIAdminSMS">A list of admin SMS phonenumbers.</param>
        /// 
        /// <param name="CookieName">The name of the HTTP Cookie for authentication.</param>
        /// <param name="DefaultLanguage">The default language of the API.</param>
        /// <param name="LogoImage">The logo of the website.</param>
        /// <param name="NewUserSignUpEMailCurrentUserId">A delegate for sending a sign-up e-mail to a new user.</param>
        /// <param name="NewUserWelcomeEMailCurrentUserId">A delegate for sending a welcome e-mail to a new user.</param>
        /// <param name="ResetPasswordEMailCurrentUserId">A delegate for sending a reset password e-mail to a user.</param>
        /// <param name="MinUserNameLenght">The minimal user name length.</param>
        /// <param name="MinRealmLenght">The minimal realm length.</param>
        /// <param name="MinPasswordLenght">The minimal password length.</param>
        /// <param name="SignInSessionLifetime">The sign-in session lifetime.</param>
        /// 
        /// <param name="SkipURLTemplates">Skip URI templates.</param>
        /// <param name="DisableNotifications">Disable external notifications.</param>
        /// <param name="DisableLogfile">Disable the log file.</param>
        /// <param name="LogfileName">The name of the logfile for this API.</param>
        public static UsersAPI AttachToHTTPAPI(HTTPServer                           HTTPServer,
                                               HTTPHostname?                        HTTPHostname                  = null,
                                               String                               ServiceName                   = DefaultServiceName,
                                               String                               BaseURL                       = "",
                                               HTTPPath?                            URLPathPrefix                 = null,

                                               EMailAddress                         APIEMailAddress               = null,
                                               String                               APIPassphrase                 = null,
                                               EMailAddressList                     APIAdminEMails                = null,
                                               SMTPClient                           APISMTPClient                 = null,

                                               Credentials                          SMSAPICredentials             = null,
                                               String                               SMSSenderName                 = null,
                                               IEnumerable<PhoneNumber>             APIAdminSMS                   = null,

                                               String                               TelegramBotToken              = null,

                                               HTTPCookieName?                      CookieName                    = null,
                                               Languages                            DefaultLanguage               = Languages.eng,
                                               String                               LogoImage                     = null,
                                               NewUserSignUpEMailCreatorDelegate    NewUserSignUpEMailCreator     = null,
                                               NewUserWelcomeEMailCreatorDelegate   NewUserWelcomeEMailCreator    = null,
                                               ResetPasswordEMailCreatorDelegate    ResetPasswordEMailCreator     = null,
                                               PasswordChangedEMailCreatorDelegate  PasswordChangedEMailCreator   = null,
                                               Byte                                 MinUserNameLenght             = DefaultMinUserNameLenght,
                                               Byte                                 MinRealmLenght                = DefaultMinRealmLenght,
                                               PasswordQualityCheckDelegate         PasswordQualityCheck          = null,
                                               TimeSpan?                            SignInSessionLifetime         = null,

                                               Boolean                              SkipURLTemplates              = false,
                                               Boolean                              DisableNotifications          = false,
                                               Boolean                              DisableLogfile                = false,
                                               String                               LogfileName                   = DefaultLogfileName)


            => new UsersAPI(HTTPServer,
                            HTTPHostname,
                            ServiceName,
                            BaseURL,
                            URLPathPrefix,

                            APIEMailAddress,
                            APIPassphrase,
                            APIAdminEMails,
                            APISMTPClient,

                            SMSAPICredentials,
                            SMSSenderName,
                            APIAdminSMS,

                            TelegramBotToken,

                            CookieName,
                            DefaultLanguage,
                            LogoImage,
                            NewUserSignUpEMailCreator,
                            NewUserWelcomeEMailCreator,
                            ResetPasswordEMailCreator,
                            PasswordChangedEMailCreator,
                            MinUserNameLenght,
                            MinRealmLenght,
                            PasswordQualityCheck,
                            SignInSessionLifetime,

                            SkipURLTemplates,
                            DisableNotifications,
                            DisableLogfile,
                            LogfileName);

        #endregion

        #region (private) GetResourceStream[...](ResourceName)

        private Stream GetResourceStream(String ResourceName)
            => GetType().Assembly.GetManifestResourceStream(HTTPRoot + ResourceName);


        private MemoryStream GetResourceMemoryStream(String ResourceName)
        {

            var OutputStream    = new MemoryStream();
            var ResourceStream  = GetType().Assembly.GetManifestResourceStream(HTTPRoot + ResourceName);

            ResourceStream.CopyTo(OutputStream);
            OutputStream.  Seek(0, SeekOrigin.Begin);

            return OutputStream;

        }

        private String GetResourceString(String ResourceName)
        {

            var OutputStream    = new MemoryStream();
            var ResourceStream  = GetType().Assembly.GetManifestResourceStream(HTTPRoot + ResourceName);

            ResourceStream.Seek(0, SeekOrigin.Begin);
            ResourceStream.CopyTo(OutputStream);

            return OutputStream.ToArray().ToUTF8String();

        }

        #endregion

        #region (protected) MixWithHTMLTemplate(ResourceName)

        String HTMLTemplate = null;

        //protected String MixWithHTMLTemplate(String ResourceName)
        //{

        //    if (HTMLTemplate == null)
        //    {

        //        var OutputStream    = new MemoryStream();
        //        var TemplateStream  = GetType().Assembly.GetManifestResourceStream(HTTPRoot + "template.html");

        //        TemplateStream.Seek(0, SeekOrigin.Begin);
        //        TemplateStream.CopyTo(OutputStream);

        //        HTMLTemplate = OutputStream.ToArray().ToUTF8String();

        //    }

        //    var HTMLStream      = new MemoryStream();
        //    var ResourceStream  = GetType().Assembly.GetManifestResourceStream(HTTPRoot + ResourceName);

        //    if (ResourceStream != null)
        //    {
        //        ResourceStream.Seek(3, SeekOrigin.Begin);
        //        ResourceStream.CopyTo(HTMLStream);
        //    }

        //    return HTMLTemplate.Replace("<%= content %>", HTMLStream.ToArray().ToUTF8String());

        //}

        protected String MixWithHTMLTemplate(String HTTPRoot2, String ResourceName)
        {

            if (HTMLTemplate == null)
            {

                var OutputStream    = new MemoryStream();
                var TemplateStream  = GetType().Assembly.GetManifestResourceStream(HTTPRoot2 + "template.html");

                TemplateStream.Seek(0, SeekOrigin.Begin);
                TemplateStream.CopyTo(OutputStream);

                HTMLTemplate = OutputStream.ToArray().ToUTF8String();

            }

            var HTMLStream      = new MemoryStream();
            var ResourceStream  = GetType().Assembly.GetManifestResourceStream(HTTPRoot2 + ResourceName);

            if (ResourceStream != null)
            {
                ResourceStream.Seek(3, SeekOrigin.Begin);
                ResourceStream.CopyTo(HTMLStream);
            }

            return HTMLTemplate.Replace("<%= content %>", HTMLStream.ToArray().ToUTF8String());

        }

        #endregion


        #region (private) GetOrganizationSerializator(Request, User)

        private OrganizationToJSONDelegate GetOrganizationSerializator(HTTPRequest  Request,
                                                                       User         User)
        {

            switch (User?.Id.ToString())
            {

                //case __issapi:
                //    return ISSNotificationExtentions.ToISSJSON;

                default:
                    return (organization,
                            embedded,
                            expandMembers,
                            expandParents,
                            expandSubOrganizations,
                            expandTags,
                            includeCryptoHash)

                            => organization.ToJSON(embedded,
                                                   expandMembers,
                                                   expandParents,
                                                   expandSubOrganizations,
                                                   expandTags,
                                                   includeCryptoHash);

            }

        }

        #endregion

        #region (private) GetUserSerializator(Request, User)

        private UserToJSONDelegate GetUserSerializator(HTTPRequest  Request,
                                                       User         User)
        {

            switch (User?.Id.ToString())
            {

                //case __issapi:
                //    return ISSNotificationExtentions.ToISSJSON;

                default:
                    return (user,
                            embedded,
                            includeCryptoHash)

                            => user.ToJSON(embedded,
                                           includeCryptoHash);

            }

        }

        #endregion

        #region (private) GetBlogPostingSerializator (Request, User)

        private BlogPostingToJSONDelegate GetBlogPostingSerializator(HTTPRequest  Request,
                                                                     User         User)
        {

            switch (User?.Id.ToString())
            {

                //case __issapi:
                //    return ISSNotificationExtentions.ToISSJSON;

                default:
                    return (BlogPosting,
                            Embedded,
                            ExpandTags,
                            IncludeCryptoHash)

                            => BlogPosting.ToJSON(Embedded,
                                                  ExpandTags,
                                                  IncludeCryptoHash);

            }

        }

        #endregion


        #region (private) CheckImpersonate(currentOrg, Astronaut, AstronautFound, Member, VetoUsers)

        private Boolean? CheckImpersonate(Organization   currentOrg,
                                          User           Astronaut,
                                          Boolean        AstronautFound,
                                          User           Member,
                                          HashSet<User>  VetoUsers)
        {

            var currentUsers = new HashSet<User>(currentOrg.User2OrganizationEdges.Select(edge => edge.Source));

            AstronautFound |= currentUsers.Contains(Astronaut);

            if (!AstronautFound)
            {

                // Fail early!
                if (currentUsers.Contains(Member))
                    return false;

            }

            else if (currentUsers.Contains(Member))
            {

                // Astronaut and member are on the same level, e.g. both admin of the same organization!
                if (currentUsers.Contains(Astronaut))
                {
                    // Currently this is allowed!
                }

                return !VetoUsers.Contains(Member);

            }

            // Everyone found so far can no longer be impersonated!
            foreach (var currentUser in currentUsers)
                VetoUsers.Add(currentUser);



            var childResults = currentOrg.Organization2OrganizationInEdges.Where(edge => edge.EdgeLabel == Organization2OrganizationEdges.IsChildOf).
                                          Select(edge => CheckImpersonate(edge.Source, Astronaut, AstronautFound, Member, new HashSet<User>(VetoUsers))).ToArray();

            return childResults.Any(result => result == true);

        }

        #endregion

        #region CanImpersonate(Astronaut, Member)

        public Boolean CanImpersonate(User  Astronaut,
                                      User  Member)
        {

            if (Astronaut == Member)
                return false;

            // API admins can impersonate everyone! Except other API Admins!
            if (Admins.InEdges(Astronaut).Any())
                return !Admins.InEdges(Member).Any();

            // API admins can never be impersonated!
            if (Admins.InEdges(Member).Any())
                return false;

            // An astronaut must be at least an admin of some parent organization!
            if (!Astronaut.User2Organization_OutEdges.Any(edge => edge.EdgeLabel == User2OrganizationEdges.IsAdmin))
                return false;

            var VetoUsers             = new HashSet<User>();
            var AstronautFound        = false;
            var CurrentOrganizations  = new HashSet<Organization>(Organizations.Where(org => !org.Organization2OrganizationOutEdges.
                                                                                                  Any(edge => edge.EdgeLabel == Organization2OrganizationEdges.IsChildOf)));

            var childResults = CurrentOrganizations.Select(org => CheckImpersonate(org, Astronaut, AstronautFound, Member, VetoUsers)).ToArray();

            return childResults.Any(result => result == true);


            do
            {

                var NextOrgs  = new HashSet<Organization>(CurrentOrganizations.SelectMany(org => org.Organization2OrganizationInEdges.Where(edge => edge.EdgeLabel == Organization2OrganizationEdges.IsChildOf)).Select(edge => edge.Source));

                foreach (var currentOrg in NextOrgs)
                {

                    var currentUsers = new HashSet<User>(currentOrg.User2OrganizationEdges.Select(edge => edge.Source));

                    AstronautFound |= currentUsers.Contains(Astronaut);

                    if (!AstronautFound)
                    {

                        // Fail early!
                        if (currentUsers.Contains(Member))
                            return false;

                    }

                    else if (currentUsers.Contains(Member))
                    {

                        // Astronaut and member are on the same level, e.g. both admin of the same organization!
                        if (currentUsers.Contains(Astronaut))
                        {
                            // Currently this is allowed!
                        }

                        return !VetoUsers.Contains(Member);

                    }

                    // Everyone found so far can no longer be impersonated!
                    currentUsers.ForEach(user => VetoUsers.Add(user));

                }

                CurrentOrganizations.Clear();
                NextOrgs.ForEach(org => CurrentOrganizations.Add(org));

            }
            while (CurrentOrganizations.Count > 0);

            // The member was not found within the organizational hierarchy!
            return false;

        }

        #endregion


        #region SendSMS(Text, To, Sender = null)

        /// <summary>
        /// Send a SMS to the given phone number.
        /// </summary>
        /// <param name="Text">The text of the SMS.</param>
        /// <param name="To">The phone number of the recipient.</param>
        /// <param name="Sender">An optional sender name.</param>
        public virtual SMSAPIResponseStatus SendSMS(String  Text,
                                                    String  To,
                                                    String  Sender  = null)
        {

            if (_SMSAPI != null)
                return _SMSAPI.Send(Text,
                                    To).
                               SetSender(Sender ?? SMSSenderName).
                               Execute();

            return SMSAPIResponseStatus.Failed("No SMSAPI defined!");

        }

        /// <summary>
        /// Send a SMS to the given phone number.
        /// </summary>
        /// <param name="Text">The text of the SMS.</param>
        /// <param name="To">The phone numbers of the recipients.</param>
        /// <param name="Sender">An optional sender name.</param>
        public SMSAPIResponseStatus SendSMS(String    Text,
                                            String[]  To,
                                            String    Sender  = null)
        {

            if (_SMSAPI != null)
                return _SMSAPI.Send(Text,
                                    To).
                               SetSender(Sender ?? SMSSenderName).
                               Execute();

            return SMSAPIResponseStatus.Failed("No SMSAPI defined!");

        }

        #endregion


        private readonly List<NotificationMessageTypeInfo> _NotificationMessageTypeInfos = new List<NotificationMessageTypeInfo> ();

        public void Add(NotificationMessageTypeInfo NotificationMessageTypeInfo)
            => _NotificationMessageTypeInfos.Add(NotificationMessageTypeInfo);


        private JObject GetNotificationInfos(User User)
        {

            if (User == null)
                throw new ArgumentNullException(nameof(User), "The given user must not be null!");

            var notificationsJSON = User.GetNotificationInfos();

            notificationsJSON.AddFirst(new JProperty("messages", new JArray(
                                           _NotificationMessageTypeInfos.Select(notificationMessageTypeInfo => notificationMessageTypeInfo.ToJSON())
                                      )));

            return notificationsJSON;

        }



        #region (private) RegisterURITemplates()

        private void RegisterURITemplates()
        {

            #region /shared/UsersAPI

            HTTPServer.RegisterResourcesFolder(HTTPHostname.Any,
                                               URLPathPrefix + "shared/UsersAPI",
                                               HTTPRoot.Substring(0, HTTPRoot.Length - 1),
                                               typeof(UsersAPI).Assembly);

            #endregion


            #region GET         ~/signup

            #region HTML_UTF8

            // -------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/signup
            // -------------------------------------------------------------
            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.GET,
                                          new HTTPPath[] { URLPathPrefix + "signup" },
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

                                              return new HTTPResponse.Builder(Request) {
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
                                         HTTPPath.Parse("/verificationtokens/{VerificationToken}"),
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
                                                      new HTTPResponse.Builder(Request) {
                                                          HTTPStatusCode  = HTTPStatusCode.NotFound,
                                                          Server          = HTTPServer.DefaultServerName,
                                                          ContentType     = HTTPContentType.HTML_UTF8,
                                                          Content         = ("VerificationToken not found!").ToUTF8Bytes(),
                                                          CacheControl    = "public",
                                                          Connection      = "close"
                                                      }.AsImmutable);

                                              if (!_Users.TryGetValue(VerificationToken.Login, out User _User))
                                                  return Task.FromResult(
                                                      new HTTPResponse.Builder(Request) {
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
                                                  var AuthenticatedUser = UserBuilder.ToImmutable;

                                                  _Users.Remove(_User.Id);
                                                  _Users.Add(AuthenticatedUser.Id, AuthenticatedUser);

                                              }

                                              #region Send New-User-Welcome-E-Mail

                                              var MailSentResult = MailSentStatus.failed;

                                              var NewUserWelcomeEMailCreatorLocal = NewUserWelcomeEMailCreator;
                                              if (NewUserWelcomeEMailCreatorLocal != null)
                                              {

                                                  var NewUserMail = NewUserWelcomeEMailCreatorLocal(UserId:     VerificationToken.Login,
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
                                                      From        = Robot.EMail,
                                                      To          = APIAdminEMails,
                                                      Subject     = "New user activated: " + _User.Id.ToString() + " at " + DateTime.UtcNow.ToString(),
                                                      Text        = "New user activated: " + _User.Id.ToString() + " at " + DateTime.UtcNow.ToString(),
                                                      Passphrase  = APIPassphrase
                                                  };

                                                  var MailResultTask = APISMTPClient.Send(AdminMail).Wait(30000);

                                                  #endregion

                                                  return Task.FromResult(
                                                      new HTTPResponse.Builder(Request) {
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
                                                  new HTTPResponse.Builder(Request) {
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
                                         HTTPPath.Parse("/login"),
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
                                                      new HTTPResponse.Builder(Request) {
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
                                                      new HTTPResponse.Builder(Request) {
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
                                                      new HTTPResponse.Builder(Request) {
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

                                              if (PasswordQualityCheck(Password) < 1.0)
                                              {

                                                  return Task.FromResult(
                                                      new HTTPResponse.Builder(Request) {
                                                          HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                          Server          = HTTPServer.DefaultServerName,
                                                          ContentType     = HTTPContentType.JSON_UTF8,
                                                          Content         = new JObject(
                                                                                new JProperty("@context",     SignInOutContext),
                                                                                new JProperty("statuscode",   400),
                                                                                new JProperty("property",     "password"),
                                                                                new JProperty("description",  "The password does not match the password quality criteria!")
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

                                              #region Check login or e-mail address and password(s)

                                              var validUsers = new HashSet<User>();

                                              if ((Realm.IsNotNullOrEmpty()
                                                       ? User_Id.TryParse(Login, Realm, out User_Id       _UserId)
                                                       : User_Id.TryParse(Login,        out               _UserId))       &&
                                                  _Users.         TryGetValue(_UserId,  out User          _User)          &&
                                                  _LoginPasswords.TryGetValue(_UserId,  out LoginPassword _LoginPassword) &&
                                                  _LoginPassword.VerifyPassword(Password))
                                              {
                                                  validUsers.Add(_User);
                                              }

                                              if (validUsers.Count == 0)
                                              {
                                                  foreach (var user in _Users.Values)
                                                  {
                                                      if (String.Equals(Login, user.EMail.Address.ToString(), StringComparison.OrdinalIgnoreCase))
                                                      {
                                                          if (_LoginPasswords.TryGetValue(user.Id, out LoginPassword loginPassword) &&
                                                              loginPassword.VerifyPassword(Password))
                                                          {
                                                              validUsers.Add(user);
                                                          }
                                                      }
                                                  }
                                              }

                                              if (!validUsers.Any())
                                                  return Task.FromResult(
                                                      new HTTPResponse.Builder(Request) {
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

                                              if (validUsers.Count > 1)
                                                  return Task.FromResult(
                                                      new HTTPResponse.Builder(Request) {
                                                          HTTPStatusCode  = HTTPStatusCode.MultipleChoices,
                                                          Server          = HTTPServer.DefaultServerName,
                                                          ContentType     = HTTPContentType.JSON_UTF8,
                                                          Content         = new JObject(
                                                                                new JProperty("@context",     SignInOutContext),
                                                                                new JProperty("property",     "login"),
                                                                                new JProperty("description",  "Multiple matching user accounts found: Please use your login name!")
                                                                            ).ToString().ToUTF8Bytes(),
                                                          CacheControl    = "private",
                                                          Connection      = "close"
                                                      }.AsImmutable);

                                              #endregion


                                              #region Register security token

                                              var validUser        = validUsers.First();
                                              var SHA256Hash       = new SHA256Managed();
                                              var SecurityTokenId  = SecurityToken_Id.Parse(SHA256Hash.ComputeHash(
                                                                                                String.Concat(Guid.NewGuid().ToString(),
                                                                                                              validUser.Id).
                                                                                                ToUTF8Bytes()
                                                                                            ).ToHexString());

                                              var Expires          = DateTime.UtcNow.Add(SignInSessionLifetime);

                                              lock (HTTPCookies)
                                              {

                                                  HTTPCookies.Add(SecurityTokenId,
                                                                  new SecurityToken(validUser.Id,
                                                                                    Expires));

                                                  File.AppendAllText(this.UsersAPIPath + DefaultHTTPCookiesFile,
                                                                     SecurityTokenId + ";" + validUser.Id + ";" + Expires.ToIso8601() + Environment.NewLine);

                                              }

                                              #endregion


                                              //Note: Add LoginResponse event!

                                              return Task.FromResult(
                                                  new HTTPResponse.Builder(Request) {
                                                      HTTPStatusCode  = HTTPStatusCode.Created,
                                                      ContentType     = HTTPContentType.HTML_UTF8,
                                                      Content         = String.Concat(
                                                                            "<!DOCTYPE html>", Environment.NewLine,
                                                                            @"<html><head><meta http-equiv=""refresh"" content=""0; url=" + RedirectURI + @""" /></head></html>",
                                                                            Environment.NewLine
                                                                        ).ToUTF8Bytes(),
                                                      CacheControl    = "private",
                                                      SetCookie       = CookieName + "=login=" + validUser.Id.ToString().ToBase64() +
                                                                                  ":username=" + validUser.Name.ToBase64() +
                                                                                  ":email="    + validUser.EMail.Address.ToString().ToBase64() +
                                                                                (IsAdmin(validUser) == Access_Levels.ReadOnly  ? ":isAdminRO" : "") +
                                                                                (IsAdmin(validUser) == Access_Levels.ReadWrite ? ":isAdminRW" : "") +
                                                                             ":securitytoken=" + SecurityTokenId +
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

            #region GET         ~/lostPassword

            // -------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/lostPassword
            // -------------------------------------------------------------------
            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "lostpassword",
                                         HTTPContentType.HTML_UTF8,
                                         HTTPDelegate: Request =>

                                            Task.FromResult(
                                                new HTTPResponse.Builder(Request) {
                                                    HTTPStatusCode             = HTTPStatusCode.OK,
                                                    Server                     = HTTPServer.DefaultServerName,
                                                    Date                       = DateTime.UtcNow,
                                                    AccessControlAllowOrigin   = "*",
                                                    AccessControlAllowMethods  = "GET",
                                                    AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                    ContentType                = HTTPContentType.HTML_UTF8,
                                                    ContentStream              = GetResourceStream("SignInOut.LostPassword-" + DefaultLanguage.ToString() + ".html"),
                                                    Connection                 = "close"
                                                }.AsImmutable),

                                         AllowReplacement: URIReplacement.Allow);

            #endregion

            #region SET         ~/resetPassword

            // --------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/resetPassword
            // --------------------------------------------------------------------
            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.SET,
                                         URLPathPrefix + "resetPassword",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             #region Parse JSON

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out HTTPResponse HTTPResponse))
                                             {

                                                 // Slow down attackers!
                                                 Thread.Sleep(5000);

                                                 return HTTPResponse;

                                             }

                                             var UserIdOrEMailJSON = JSONObj["id"].Value<String>();

                                             if (UserIdOrEMailJSON != null)
                                                 UserIdOrEMailJSON = UserIdOrEMailJSON.Trim();

                                             if (UserIdOrEMailJSON.IsNullOrEmpty() || UserIdOrEMailJSON.Length < 4)
                                             {

                                                 // Slow down attackers!
                                                 Thread.Sleep(5000);

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = DateTime.UtcNow,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            Connection                 = "close"
                                                        };

                                             }

                                             #endregion

                                             #region Find user(s)...

                                             var Users = new HashSet<User>();

                                             if (User_Id.TryParse(UserIdOrEMailJSON, out User_Id UserId) &&
                                                 TryGet(UserId, out User User))
                                             {
                                                 Users.Add(User);
                                             }

                                             if (SimpleEMailAddress.TryParse(UserIdOrEMailJSON, out SimpleEMailAddress EMailAddress))
                                             {
                                                 foreach (var user in _Users.Values)
                                                 {
                                                     if (user.EMail.Address == EMailAddress)
                                                         Users.Add(user);
                                                 }
                                             }

                                             #endregion

                                             #region No users found!

                                             if (Users.Count == 0)
                                             {

                                                 // Slow down attackers!
                                                 Thread.Sleep(5000);

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = DateTime.UtcNow,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            Connection                 = "close"
                                                        };

                                             }

                                             #endregion


                                             var PasswordReset    = await ResetPassword(Users.Select(user => user.Id),
                                                                                        SecurityToken_Id.Random(40, _Random),
                                                                                        SecurityToken_Id.Parse(_Random.RandomString(5) + "-" + _Random.RandomString(5)));

                                             var MailSentResults  = new List<MailSentStatus>();
                                             var SMSSentResults   = new List<SMSAPIResponseStatus>();

                                             try
                                             {

                                                 foreach (var user in Users)
                                                 {

                                                     #region Send e-mail...

                                                     var MailResultTask = APISMTPClient.Send(ResetPasswordEMailCreator(user.Id,
                                                                                                                       user.EMail,
                                                                                                                       user.Name,
                                                                                                                       PasswordReset.SecurityToken1,
                                                                                                                       user.MobilePhone.HasValue,
                                                                                                                       "https://" + Request.Host.SimpleString,
                                                                                                                       DefaultLanguage));

                                                     if (MailResultTask.Wait(60000))
                                                         MailSentResults.Add(MailResultTask.Result);

                                                     #endregion

                                                     #region Send SMS...

                                                     if (_SMSAPI != null && user.MobilePhone.HasValue)
                                                     {

                                                         SMSSentResults.Add(_SMSAPI.Send("Dear '" + user.Name + "' your 2nd security token for resetting your password is '" + PasswordReset.SecurityToken2 + "'!",
                                                                                         user.MobilePhone.Value.ToString()).
                                                                                    SetSender("CardiCloud").
                                                                                    Execute());

                                                     }

                                                     #endregion

                                                 }

                                             }
                                             catch (Exception e)
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.InternalServerError,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = DateTime.UtcNow,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = JSONObject.Create(
                                                                                             new JProperty("description", "Could not reset your password! " + e.Message)
                                                                                         ).ToUTF8Bytes(),
                                                            Connection                 = "close"
                                                        };

                                             }


                                             return new HTTPResponse.Builder(Request) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        Server                     = HTTPServer.DefaultServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "SET",
                                                        AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                        ContentType                = HTTPContentType.JSON_UTF8,
                                                        Content                    = JSONObject.Create(
                                                                                         new JProperty("numberOfAccountsFound", Users.Count)
                                                                                     ).ToUTF8Bytes(),
                                                        Connection                 = "close"
                                                    };

                                             },

                                             AllowReplacement: URIReplacement.Allow);

            #endregion

            #region SET         ~/setPassword

            // ------------------------------------------------------------------
            // curl -v -X SET \
            //      -H 'Accept:       application/json; charset=UTF-8' \
            //      -H 'Content-Type: application/json; charset=UTF-8' \
            //      -d "{ \
            //            \"securityToken1\": \"4tf7M62p5C92tE2d5CY74UWfx2S4jxSp2z5S3jM3\", \
            //            \"newPassword\":    \"bguf7tf8g\" \
            //          }" \
            //      http://127.0.0.1:2001/setPassword
            // ------------------------------------------------------------------
            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.SET,
                                         URLPathPrefix + "setPassword",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             #region Parse JSON

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out HTTPResponse HTTPResponse))
                                             {

                                                 // Slow down attackers!
                                                 Thread.Sleep(5000);

                                                 return HTTPResponse;

                                             }

                                             #region Parse SecurityToken1   [mandatory]

                                             if (!JSONObj.ParseMandatory("securityToken1",
                                                                         "security token #1",
                                                                         HTTPServer.DefaultServerName,
                                                                         SecurityToken_Id.TryParse,
                                                                         out SecurityToken_Id SecurityToken1,
                                                                         Request,
                                                                         out HTTPResponse ErrorResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion

                                             #region Parse SecurityToken2   [optional]

                                             if (JSONObj.ParseOptional("securityToken2",
                                                                       "security token #2",
                                                                       HTTPServer.DefaultServerName,
                                                                       SecurityToken_Id.TryParse,
                                                                       out SecurityToken_Id? SecurityToken2,
                                                                       Request,
                                                                       out ErrorResponse))
                                             {
                                                 if (ErrorResponse != null)
                                                    return HTTPResponse;
                                             }

                                             #endregion

                                             #region Parse NewPassword      [mandatory]

                                             if (!JSONObj.ParseMandatory("newPassword",
                                                                         "new password",
                                                                         HTTPServer.DefaultServerName,
                                                                         Password.TryParse,
                                                                         out Password NewPassword,
                                                                         Request,
                                                                         out ErrorResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion


                                             #region Verify token/password lengths...

                                             if (SecurityToken1.Length != 40 ||
                                                 NewPassword.Length < 6)
                                             {

                                                 // Slow down attackers!
                                                 Thread.Sleep(5000);

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = DateTime.UtcNow,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            Connection                 = "close"
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #endregion

                                             #region Is this a known/valid request?

                                             if (!PasswordResets.TryGetValue(SecurityToken1, out PasswordReset _PasswordReset))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = DateTime.UtcNow,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ContentType                = HTTPContentType.JSONLD_UTF8,
                                                            Content                    = JSONObject.Create(
                                                                                             new JProperty(
                                                                                                 "description",
                                                                                                 "Unknown security token 1!")
                                                                                         ).ToUTF8Bytes(),
                                                            Connection                 = "close"
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #region Is security token 2 used and valid?

                                             if (SecurityToken2.HasValue                &&
                                                 SecurityToken2.Value.ToString() != ""  &&
                                                 SecurityToken2.Value            != _PasswordReset.SecurityToken2)
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = DateTime.UtcNow,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ContentType                = HTTPContentType.JSONLD_UTF8,
                                                            Content                    = JSONObject.Create(
                                                                                             new JProperty(
                                                                                                 "description",
                                                                                                 "Invalid security token 2!")
                                                                                         ).ToUTF8Bytes(),
                                                            Connection                 = "close"
                                                        }.AsImmutable;

                                             }

                                             #endregion


                                             foreach (var userId in _PasswordReset.UserIds)
                                             {

                                                 await WriteToLogfile(NotificationMessageType.Parse("resetPassword"),
                                                                      JSONObject.Create(

                                                                          new JProperty("login",                 userId.ToString()),

                                                                          new JProperty("newPassword", new JObject(
                                                                              new JProperty("salt",              NewPassword.Salt.UnsecureString()),
                                                                              new JProperty("passwordHash",      NewPassword.UnsecureString)
                                                                          )),

                                                                          new JProperty("securityToken1",        SecurityToken1.ToString()),

                                                                          SecurityToken2.HasValue
                                                                              ? new JProperty("securityToken2",  SecurityToken2.ToString())
                                                                              : null

                                                                      ),
                                                                      this.UsersAPIPath + DefaultPasswordFile,
                                                                      Robot.Id);

                                                 _LoginPasswords.Remove(userId);

                                                 _LoginPasswords.Add(userId, new LoginPassword(userId, NewPassword));

                                                 await Remove(_PasswordReset);

                                                 #region Send e-mail...

                                                 var MailSentResult = MailSentStatus.failed;
                                                 var user           = Get(userId);

                                                 var MailResultTask = APISMTPClient.Send(PasswordChangedEMailCreator(user.Id,
                                                                                                                     user.EMail,
                                                                                                                     user.Name,
                                                                                                                     "https://" + Request.Host.SimpleString,
                                                                                                                     DefaultLanguage));

                                                 if (MailResultTask.Wait(60000))
                                                     MailSentResult = MailResultTask.Result;

                                                 #endregion

                                             }

                                             return new HTTPResponse.Builder(Request) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        Server                     = HTTPServer.DefaultServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "SET",
                                                        AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                        ContentType                = HTTPContentType.JSON_UTF8,
                                                        Content                    = JSONObject.Create(
                                                                                         new JProperty("numberOfAccountsFound", Users.Count())
                                                                                     ).ToUTF8Bytes(),
                                                        SetCookie                  = CookieName + "=; Expires=" + DateTime.UtcNow.ToRfc1123() +
                                                                                         (HTTPCookieDomain.IsNotNullOrEmpty()
                                                                                             ? "; Domain=" + HTTPCookieDomain
                                                                                             : "") +
                                                                                         "; Path=/",
                                                        Connection                 = "close"
                                                    }.AsImmutable;

                                             },

                                             AllowReplacement: URIReplacement.Allow);

            #endregion


            #region ~/users

            #region GET         ~/users

            // -------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2000/users
            // -------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "users",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Try to get HTTP user and its organizations

                                             TryGetHTTPUser(Request,
                                                            out User                   HTTPUser,
                                                            out HashSet<Organization>  HTTPOrganizations,
                                                            out HTTPResponse           Response,
                                                            Recursive: true);

                                             #endregion


                                             var includeFilter           = Request.QueryString.CreateStringFilter<User>("include",
                                                                                                                        (user, include) => user.Id.ToString().Contains(include) ||
                                                                                                                                           user.Name.IndexOf(include, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                                                                                                                           user.Description.Matches(include, IgnoreCase: true));

                                             var skip                    = Request.QueryString.GetUInt64 ("skip");
                                             var take                    = Request.QueryString.GetUInt64 ("take");

                                             var includeCryptoHash       = Request.QueryString.GetBoolean("includeCryptoHash", true);

                                             var expand                  = Request.QueryString.GetStrings("expand", true);
                                             //var expandTags              = expand.Contains("tags")              ? InfoStatus.Expand : InfoStatus.ShowIdOnly;


                                             //ToDo: Getting the expected total count might be very expensive!
                                             var _ExpectedCount = Organizations.ULongCount();

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode                 = HTTPStatusCode.OK,
                                                     Server                         = HTTPServer.DefaultServerName,
                                                     Date                           = DateTime.UtcNow,
                                                     AccessControlAllowOrigin       = "*",
                                                     AccessControlAllowMethods      = "GET, COUNT, OPTIONS",
                                                     AccessControlAllowHeaders      = "Content-Type, Accept, Authorization",
                                                     ETag                           = "1",
                                                     ContentType                    = HTTPContentType.JSON_UTF8,
                                                     Content                        = HTTPOrganizations.
                                                                                          SafeSelectMany(organization => organization.Users).
                                                                                          Distinct      ().
                                                                                          Where         (includeFilter).
                                                                                          OrderBy       (user => user.Name).
                                                                                          ToJSON        (skip,
                                                                                                         take,
                                                                                                         false, //Embedded
                                                                                                         GetUserSerializator(Request, HTTPUser),
                                                                                                         includeCryptoHash).
                                                                                          ToUTF8Bytes(),
                                                     X_ExpectedTotalNumberOfItems   = _ExpectedCount,
                                                     Connection                     = "close",
                                                     Vary                           = "Accept"
                                                 }.AsImmutable);

                                         });

            #endregion

            // -------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/users
            // -------------------------------------------------------------------
            //HTTPServer.ITEMS_GET(UriTemplate: URLPathPrefix + "users",
            //                     Dictionary: _Users,
            //                     Filter: user => user.PrivacyLevel == PrivacyLevel.World,
            //                     ToJSONDelegate: JSON_IO.ToJSON);

            #region DEAUTH      ~/users

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.DEAUTH,
                                          HTTPPath.Parse("/users"),
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: Request =>

                                              Task.FromResult(
                                                  new HTTPResponse.Builder(Request) {
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
                                          HTTPPath.Parse("/users/{UserId}"),
                                          HTTPContentType.JSON_UTF8,
                                          HTTPRequestLogger:   AddUserRequest,
                                          HTTPResponseLogger:  AddUserResponse,
                                          HTTPDelegate:        async Request => {

                                              #region Get HTTP user and its organizations

                                              // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                              if (!TryGetHTTPUser(Request,
                                                                  out User                   HTTPUser,
                                                                  out HashSet<Organization>  HTTPOrganizations,
                                                                  out HTTPResponse           ErrorResponse,
                                                                  AccessLevel:               Access_Levels.ReadWrite,
                                                                  Recursive:                 true))
                                              {
                                                  return ErrorResponse;
                                              }

                                              #endregion

                                              #region Check UserId URI parameter

                                              if (!Request.ParseUserId(this,
                                                                       out User_Id? UserIdURI,
                                                                       out ErrorResponse))
                                              {
                                                  return ErrorResponse;
                                              }

                                              #endregion

                                              #region Parse JSON and create the new user...

                                              if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out ErrorResponse))
                                                  return ErrorResponse;

                                              #region Parse UserId           [optional]

                                              // Verify that a given user identification
                                              //   is at least valid.
                                              if (JSONObj.ParseOptionalStruct2("@id",
                                                                              "user identification",
                                                                              HTTPServer.DefaultHTTPServerName,
                                                                              User_Id.TryParse,
                                                                              out User_Id? UserIdBody,
                                                                              Request,
                                                                              out ErrorResponse))
                                              {

                                                  if (ErrorResponse != null)
                                                      return ErrorResponse;

                                              }

                                              if (!UserIdURI.HasValue && !UserIdBody.HasValue)
                                              {

                                                  return new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                             Server                     = HTTPServer.DefaultServerName,
                                                             Date                       = DateTime.UtcNow,
                                                             AccessControlAllowOrigin   = "*",
                                                             AccessControlAllowMethods  = "GET, SET",
                                                             AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                             ContentType                = HTTPContentType.JSON_UTF8,
                                                             Content                    = JSONObject.Create(
                                                                                              new JProperty("description", "The user identification is missing!")
                                                                                          ).ToUTF8Bytes()
                                                         }.AsImmutable;

                                              }

                                              if (UserIdURI.HasValue && UserIdBody.HasValue && UserIdURI.Value != UserIdBody.Value)
                                              {

                                                  return new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                             Server                     = HTTPServer.DefaultServerName,
                                                             Date                       = DateTime.UtcNow,
                                                             AccessControlAllowOrigin   = "*",
                                                             AccessControlAllowMethods  = "GET, SET",
                                                             AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                             ContentType                = HTTPContentType.JSON_UTF8,
                                                             Content                    = JSONObject.Create(
                                                                                              new JProperty("description", "The optional user identification given within the JSON body does not match the one given in the URI!")
                                                                                          ).ToUTF8Bytes()
                                                         }.AsImmutable;

                                              }

                                              if (!User_Id.TryParse(Request.ParsedURIParameters[0], out User_Id UserId) ||
                                                   UserId.Length < MinLoginLenght)
                                              {

                                                  return new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                             Server                     = HTTPServer.DefaultServerName,
                                                             Date                       = DateTime.UtcNow,
                                                             AccessControlAllowOrigin   = "*",
                                                             AccessControlAllowMethods  = "GET, SET",
                                                             AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                             ContentType                = HTTPContentType.JSON_UTF8,
                                                             Content                    = JSONObject.Create(
                                                                                              new JProperty("description", "The given user identification is invalid!")
                                                                                          ).ToUTF8Bytes()
                                                         }.AsImmutable;

                                              }

                                              if (_Users.ContainsKey(UserId))
                                              {

                                                  return new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                             Server                     = HTTPServer.DefaultServerName,
                                                             Date                       = DateTime.UtcNow,
                                                             AccessControlAllowOrigin   = "*",
                                                             AccessControlAllowMethods  = "GET, SET",
                                                             AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                             ContentType                = HTTPContentType.JSON_UTF8,
                                                             Content                    = JSONObject.Create(
                                                                                              new JProperty("description", "The given user identification already exists!")
                                                                                          ).ToUTF8Bytes()
                                                         }.AsImmutable;

                                              }

                                              #endregion

                                              #region Parse Context          [mandatory]

                                              if (!JSONObj.ParseMandatory("@context",
                                                                          "JSON-LinkedData context information",
                                                                          HTTPServer.DefaultServerName,
                                                                          out String Context,
                                                                          Request,
                                                                          out ErrorResponse))
                                              {
                                                  return ErrorResponse;
                                              }

                                              if (Context != User.JSONLDContext)
                                              {

                                                  return new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                             Server                     = HTTPServer.DefaultServerName,
                                                             Date                       = DateTime.UtcNow,
                                                             AccessControlAllowOrigin   = "*",
                                                             AccessControlAllowMethods  = "GET, SET",
                                                             AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                             ContentType                = HTTPContentType.JSON_UTF8,
                                                             Content                    = JSONObject.Create(
                                                                                              new JProperty("description", @"The given JSON-LD ""@context"" information '" + Context + "' is not supported!")
                                                                                          ).ToUTF8Bytes()
                                                         }.AsImmutable;

                                              }

                                              #endregion

                                              #region Parse Name             [mandatory]

                                              if (!JSONObj.ParseMandatory("name",
                                                                          "Username",
                                                                          HTTPServer.DefaultServerName,
                                                                          out String Name,
                                                                          Request,
                                                                          out ErrorResponse))
                                              {
                                                  return ErrorResponse;
                                              }

                                              if (Name.IsNullOrEmpty() || Name.Length < 4)
                                              {

                                                  return new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                             Server                     = HTTPServer.DefaultServerName,
                                                             Date                       = DateTime.UtcNow,
                                                             AccessControlAllowOrigin   = "*",
                                                             AccessControlAllowMethods  = "GET, SET",
                                                             AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                             ContentType                = HTTPContentType.JSON_UTF8,
                                                             Content                    = JSONObject.Create(
                                                                                              new JProperty("description", "Invalid user name!")
                                                                                          ).ToUTF8Bytes()
                                                         }.AsImmutable;

                                              }

                                              #endregion

                                              #region Parse E-Mail           [mandatory]

                                              if (!JSONObj.ParseMandatory("email",
                                                                          "E-Mail",
                                                                          HTTPServer.DefaultServerName,
                                                                          SimpleEMailAddress.Parse,
                                                                          out SimpleEMailAddress UserEMail,
                                                                          Request,
                                                                          out ErrorResponse))
                                              {
                                                  return ErrorResponse;
                                              }

                                              //ToDo: See rfc5322 for more complex regular expression!
                                              //      [a-z0-9!#$%&'*+/=?^_`{|}~-]+
                                              // (?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*
                                              //                 @
                                              // (?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+
                                              //    [a-z0-9](?:[a-z0-9-]*[a-z0-9])?
                                              var matches = Regex.Match(JSONObj.GetOptional("email").Trim(), @"([^\@]+)\@([^\@]+\.[^\@]{2,})");

                                              if (!matches.Success)
                                              {

                                                  return new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                             Server                     = HTTPServer.DefaultServerName,
                                                             Date                       = DateTime.UtcNow,
                                                             AccessControlAllowOrigin   = "*",
                                                             AccessControlAllowMethods  = "GET, SET",
                                                             AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                             ContentType                = HTTPContentType.JSON_UTF8,
                                                             Content                    = JSONObject.Create(
                                                                                              new JProperty("description", "Invalid e-mail address!")
                                                                                          ).ToUTF8Bytes()
                                                         }.AsImmutable;

                                              }

                                              //ToDo: CNAMEs are also okay according to rfc5321#section-5
                                              var MailServerA     = DNSClient.Query<A>   (matches.Groups[2].Value);
                                              var MailServerAAAA  = DNSClient.Query<AAAA>(matches.Groups[2].Value);
                                              var MailServerMX    = DNSClient.Query<MX>  (matches.Groups[2].Value);

                                              Task.WaitAll(MailServerA, MailServerAAAA, MailServerMX);

                                              if (!MailServerA.   Result.Any() &
                                                  !MailServerAAAA.Result.Any() &
                                                  !MailServerMX.  Result.Any())
                                              {

                                                  return new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                             Server                     = HTTPServer.DefaultServerName,
                                                             Date                       = DateTime.UtcNow,
                                                             AccessControlAllowOrigin   = "*",
                                                             AccessControlAllowMethods  = "GET, SET",
                                                             AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                             ContentType                = HTTPContentType.JSON_UTF8,
                                                             Content                    = JSONObject.Create(
                                                                                              new JProperty("description", "Invalid domain name of the given e-mail address!")
                                                                                          ).ToUTF8Bytes()
                                                         }.AsImmutable;

                                              }

                                              #endregion

                                              #region Parse AccessLevel      [mandatory]

                                              if (!JSONObj.ParseMandatory("accessLevel",
                                                                          "access level",
                                                                          HTTPServer.DefaultServerName,
                                                                          out String AccessLevel,
                                                                          Request,
                                                                          out ErrorResponse))
                                              {
                                                  return ErrorResponse;
                                              }

                                              if (AccessLevel != "guest" && AccessLevel != "member" && AccessLevel != "admin")
                                              {

                                                  return new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                             Server                     = HTTPServer.DefaultServerName,
                                                             Date                       = DateTime.UtcNow,
                                                             AccessControlAllowOrigin   = "*",
                                                             AccessControlAllowMethods  = "GET, SET",
                                                             AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                             ContentType                = HTTPContentType.JSON_UTF8,
                                                             Content                    = JSONObject.Create(
                                                                                              new JProperty("description", "Invalid user access level!")
                                                                                          ).ToUTF8Bytes()
                                                         }.AsImmutable;

                                              }

                                              #endregion

                                              #region Parse MobilePhone      [optional]

                                              if (JSONObj.ParseOptional("mobilePhone",
                                                                        "mobile phone number",
                                                                        HTTPServer.DefaultServerName,
                                                                        PhoneNumber.TryParse,
                                                                        out PhoneNumber? MobilePhone,
                                                                        Request,
                                                                        out ErrorResponse))
                                              {

                                                  if (ErrorResponse != null)
                                                      return ErrorResponse;

                                              }

                                              #endregion

                                              #region Parse Description      [optional]

                                              if (JSONObj.ParseOptional("description",
                                                                        "user description",
                                                                        HTTPServer.DefaultHTTPServerName,
                                                                        out I18NString Description,
                                                                        Request,
                                                                        out ErrorResponse))
                                              {

                                                  if (ErrorResponse != null)
                                                      return ErrorResponse;

                                              }

                                              #endregion

                                              #region Parse Organization     [optional]

                                              Organization _Organization = null;

                                              // Verify that a given user identification
                                              //   is at least valid.
                                              if (JSONObj.ParseOptionalStruct2("organization",
                                                                              "organization",
                                                                              HTTPServer.DefaultHTTPServerName,
                                                                              Organization_Id.TryParse,
                                                                              out Organization_Id? OrganizationId,
                                                                              Request,
                                                                              out ErrorResponse))
                                              {

                                                  if (ErrorResponse != null)
                                                      return ErrorResponse;

                                                  if (!_Organizations.TryGetValue(OrganizationId.Value, out _Organization))
                                                  {

                                                      return new HTTPResponse.Builder(Request) {
                                                                 HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                                 Server                     = HTTPServer.DefaultServerName,
                                                                 Date                       = DateTime.UtcNow,
                                                                 AccessControlAllowOrigin   = "*",
                                                                 AccessControlAllowMethods  = "GET, SET",
                                                                 AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                 ContentType                = HTTPContentType.JSON_UTF8,
                                                                 Content                    = JSONObject.Create(
                                                                                                  new JProperty("description", "The given user identification already exists!")
                                                                                              ).ToUTF8Bytes()
                                                             }.AsImmutable;

                                                  }

                                              }

                                              #endregion

                                              PgpPublicKeyRing _PublicKeyRing = null;

                                              #endregion


                                              var MailSentResult = MailSentStatus.failed;
                                              com.GraphDefined.SMSApi.API.Response.SMSAPIResponseStatus SMSSentResult = null;

                                              try
                                              {

                                                  var SetPasswordRequest  = await ResetPassword(UserIdURI.Value,
                                                                                                SecurityToken_Id.Random(40, _Random),
                                                                                                SecurityToken_Id.Parse(_Random.RandomString(5) + "-" + _Random.RandomString(5)));

                                                  #region Send e-mail...

                                                  var MailResultTask  = APISMTPClient.Send(NewUserSignUpEMailCreator(UserIdURI.Value,
                                                                                                                     new EMailAddress(Name,
                                                                                                                                      UserEMail,
                                                                                                                                      _PublicKeyRing),
                                                                                                                     Name,
                                                                                                                     SetPasswordRequest.SecurityToken1,
                                                                                                                     MobilePhone.HasValue,
                                                                                                                     "https://" + Request.Host.SimpleString,
                                                                                                                     DefaultLanguage));

                                                  if (MailResultTask.Wait(60000))
                                                      MailSentResult  = MailResultTask.Result;

                                                  #endregion

                                                  #region Send SMS...

                                                  if (_SMSAPI != null && MobilePhone.HasValue)
                                                  {

                                                                                   // Be careful what to send to endusers!
                                                                                   // "Your new account is 'hsadgsagd'!" makes them type also ' and ! characters!
                                                      SMSSentResult = _SMSAPI.Send("Dear '" + Name + "' your 2nd security token for your new account is: " + SetPasswordRequest.SecurityToken2,
                                                                                   MobilePhone.Value.ToString()).
                                                                              SetSender("CardiCloud").
                                                                              Execute();

                                                  }

                                                  #endregion

                                                  if (MailSentResult == MailSentStatus.ok) //ToDo: Verify SMS!
                                                  {

                                                      var NewUser = await CreateUser(Id:             UserId,
                                                                                     Name:           Name,
                                                                                     EMail:          UserEMail,
                                                                                     MobilePhone:    MobilePhone,
                                                                                     Description:    Description,
                                                                                     PublicKeyRing:  _PublicKeyRing,
                                                                                     CurrentUserId:  HTTPUser.Id);

                                                      if (_Organization != null)
                                                      {

                                                          switch (AccessLevel)
                                                          {

                                                              case "guest":
                                                                  await AddToOrganization(NewUser, User2OrganizationEdges.IsGuest,   _Organization);
                                                                  break;

                                                              case "member":
                                                                  await AddToOrganization(NewUser, User2OrganizationEdges.IsMember,  _Organization);
                                                                  break;

                                                              case "admin":
                                                                  await AddToOrganization(NewUser, User2OrganizationEdges.IsAdmin,   _Organization);
                                                                  break;

                                                          }

                                                      }

                                                  }

                                                  else if (MailSentResult != MailSentStatus.ok)
                                                  {

                                                      return new HTTPResponse.Builder(Request) {
                                                                 HTTPStatusCode             = HTTPStatusCode.InternalServerError,
                                                                 Server                     = HTTPServer.DefaultServerName,
                                                                 Date                       = DateTime.UtcNow,
                                                                 AccessControlAllowOrigin   = "*",
                                                                 AccessControlAllowMethods  = "ADD, SET, GET",
                                                                 AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                 ContentType                = HTTPContentType.JSON_UTF8,
                                                                 Content                    = JSONObject.Create(
                                                                                                  new JProperty("description", "Could not send an e-mail to the user!")
                                                                                              ).ToUTF8Bytes()
                                                             }.AsImmutable;

                                                  }

                                                  //else if (SMSSentResult != ???)
                                                  //{
                                                  //}


                                                  #region Send Admin-Mail...

                                                  var AdminMail = new TextEMailBuilder() {
                                                      From        = Robot.EMail,
                                                      To          = APIAdminEMails,
                                                      Subject     = "New user registered: " + Name + "/" + UserId + " <" + UserEMail + ">",
                                                      Text        = "New user registered: " + Name + "/" + UserId + " <" + UserEMail + ">",
                                                      Passphrase  = APIPassphrase
                                                  };

                                                  var MailResultTask2 = APISMTPClient.Send(AdminMail).Wait(30000);

                                                  #endregion

                                              }
                                              catch (Exception e)
                                              {

                                                  while (e.InnerException != null)
                                                      e = e.InnerException;

                                                  return new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode             = HTTPStatusCode.InternalServerError,
                                                             Server                     = HTTPServer.DefaultServerName,
                                                             Date                       = DateTime.UtcNow,
                                                             AccessControlAllowOrigin   = "*",
                                                             AccessControlAllowMethods  = "ADD, SET, GET",
                                                             AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                             ContentType                = HTTPContentType.JSON_UTF8,
                                                             Content                    = JSONObject.Create(
                                                                                              new JProperty("description", "Could not create the given user! " + e.Message)
                                                                                          ).ToUTF8Bytes()
                                                         }.AsImmutable;

                                              }

                                              return new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode              = HTTPStatusCode.Created,
                                                         Server                      = HTTPServer.DefaultServerName,
                                                         Date                        = DateTime.UtcNow,
                                                         AccessControlAllowOrigin    = "*",
                                                         AccessControlAllowMethods   = "ADD, SET, GET",
                                                         AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                         Connection                  = "close"
                                                     }.AsImmutable;

                                         });

            #endregion

            #region EXISTS      ~/users/{UserId}

            // ---------------------------------------------------------------------------------
            // curl -v -X EXITS -H "Accept: application/json" http://127.0.0.1:2100/users/ahzf
            // ---------------------------------------------------------------------------------
            HTTPServer.ITEM_EXISTS<User_Id, User>(UriTemplate: URLPathPrefix + "users/{UserId}",
                                                  ParseIdDelegate: User_Id.TryParse,
                                                  ParseIdError: Text => "Invalid user identification '" + Text + "'!",
                                                  TryGetItemDelegate: _Users.TryGetValue,
                                                  ItemFilterDelegate: user => user.PrivacyLevel == PrivacyLevel.World,
                                                  TryGetItemError: userId => "Unknown user '" + userId + "'!");

            #endregion

            #region OPTIONS     ~/users/{UserId}

            // -----------------------------------------------------------------------------------
            // curl -X OPTIONS -v -H "Accept: application/json" http://127.0.0.1:2100/users/ahzf
            // -----------------------------------------------------------------------------------
            //HTTPServer.ITEM_GET<User_Id, User>(UriTemplate:         URIPrefix + "users/{UserId}",
            //                                   ParseIdDelegate:     User_Id.TryParse,
            //                                   ParseIdError:        Text => "Invalid user identification '" + Text + "'!",
            //                                   TryGetItemDelegate:  _Users.TryGetValue,
            //                                   ItemFilterDelegate:  user   => user.PrivacyLevel == PrivacyLevel.World,
            //                                   TryGetItemError:     userId => "Unknown user '" + userId + "'!",
            //                                   ToJSONDelegate:      user   => user.ToJSON(IncludeCryptoHash: true));

            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix + "users/{UserId}",
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = DateTime.UtcNow,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = "GET, SET",
                                                     AccessControlAllowHeaders  = "X-PINGOTHER, Content-Type, Accept, Authorization, X-App-Version",
                                                     AccessControlMaxAge        = 3600,
                                                     //ETag                       = "1",
                                                     CacheControl               = "public",
                                                     //Expires                    = "Mon, 25 Jun 2015 21:31:12 GMT",
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         });


            #region Get HTTP user and its organizations

            // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
            //if (!TryGetHTTPUser(Request,
            //                    out User                   HTTPUser,
            //                    out HashSet<Organization>  HTTPOrganizations,
            //                    out HTTPResponse           Response,
            //                    Recursive: true))
            //{
            //    return Task.FromResult(Response);
            //}

            #endregion


            #endregion

            #region GET         ~/users/{UserId}

            // ------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/users/ahzf
            // ------------------------------------------------------------------------
            //HTTPServer.ITEM_GET<User_Id, User>(UriTemplate:         URIPrefix + "users/{UserId}",
            //                                   ParseIdDelegate:     User_Id.TryParse,
            //                                   ParseIdError:        Text => "Invalid user identification '" + Text + "'!",
            //                                   TryGetItemDelegate:  _Users.TryGetValue,
            //                                   ItemFilterDelegate:  user   => user.PrivacyLevel == PrivacyLevel.World,
            //                                   TryGetItemError:     userId => "Unknown user '" + userId + "'!",
            //                                   ToJSONDelegate:      user   => user.ToJSON(IncludeCryptoHash: true));

            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "users/{UserId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             TryGetHTTPUser(Request,
                                                            out User                   HTTPUser,
                                                            out HashSet<Organization>  HTTPOrganizations,
                                                            Recursive: true);

                                             #endregion

                                             #region Check UserId URI parameter

                                             Request.ParseUser(this,
                                                               out User_Id?      UserId,
                                                               out User          User,
                                                               out HTTPResponse  HTTPResponse);

                                             #endregion


                                             // Anonymous HTTP user
                                             if (HTTPUser == null)
                                             {

                                                 #region 1. UserId is unknown or not a public profile

                                                 if (User == null || User.PrivacyLevel != PrivacyLevel.World)
                                                     return Task.FromResult(
                                                         new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode              = HTTPStatusCode.NotFound,
                                                             Server                      = HTTPServer.DefaultServerName,
                                                             Date                        = DateTime.UtcNow,
                                                             AccessControlAllowOrigin    = "*",
                                                             AccessControlAllowMethods   = "GET",
                                                             AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                             Connection                  = "close",
                                                             Vary                        = "Accept"
                                                         }.AsImmutable);

                                                 #endregion

                                                 #region 2. A public profile

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode              = HTTPStatusCode.OK,
                                                         Server                      = HTTPServer.DefaultServerName,
                                                         Date                        = DateTime.UtcNow,
                                                         AccessControlAllowOrigin    = "*",
                                                         AccessControlAllowMethods   = "GET, SET",
                                                         AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                         ContentType                 = HTTPContentType.HTML_UTF8,
                                                         Content                     = User.ToJSON().ToUTF8Bytes(),
                                                         Connection                  = "close",
                                                         Vary                        = "Accept"
                                                     }.AsImmutable);

                                                 #endregion

                                             }


                                             // Authenticated HTTP user

                                             #region 1. UserId is unknown

                                             if (User == null)
                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode              = HTTPStatusCode.NotFound,
                                                         Server                      = HTTPServer.DefaultServerName,
                                                         Date                        = DateTime.UtcNow,
                                                         AccessControlAllowOrigin    = "*",
                                                         AccessControlAllowMethods   = "GET",
                                                         AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                         Connection                  = "close",
                                                         Vary                        = "Accept"
                                                     }.AsImmutable);

                                             #endregion

                                             var UserJSON = User.ToJSON();

                                             #region 2. You request _your own_ or you are a valid _admin_ => r/w access

                                             if (HTTPUser.Id == User.Id || CanImpersonate(HTTPUser, User))
                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode              = HTTPStatusCode.OK,
                                                         Server                      = HTTPServer.DefaultServerName,
                                                         Date                        = DateTime.UtcNow,
                                                         AccessControlAllowOrigin    = "*",
                                                         AccessControlAllowMethods   = "GET, SET",
                                                         AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                         ContentType                 = HTTPContentType.HTML_UTF8,
                                                         Content                     = UserJSON.
                                                                                           AddAndReturn(new JProperty("youCanEdit", true)).
                                                                                           ToUTF8Bytes(),
                                                         Connection                  = "close",
                                                         Vary                        = "Accept"
                                                     }.AsImmutable);

                                             #endregion

                                             // same org/sub-orgs

                                             #region 3. A public profile => r/o access

                                             if (User.PrivacyLevel == PrivacyLevel.Plattform)
                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode              = HTTPStatusCode.OK,
                                                         Server                      = HTTPServer.DefaultServerName,
                                                         Date                        = DateTime.UtcNow,
                                                         AccessControlAllowOrigin    = "*",
                                                         AccessControlAllowMethods   = "GET, SET",
                                                         AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                         ContentType                 = HTTPContentType.HTML_UTF8,
                                                         Content                     = UserJSON.ToUTF8Bytes(),
                                                         Connection                  = "close",
                                                         Vary                        = "Accept"
                                                     }.AsImmutable);

                                             #endregion


                                             return Task.FromResult(
                                                              new HTTPResponse.Builder(Request) {
                                                                        HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                        Server                     = HTTPServer.DefaultServerName,
                                                                        Date                       = DateTime.UtcNow,
                                                                        AccessControlAllowOrigin   = "*",
                                                                        AccessControlAllowMethods  = "GET, SET",
                                                                        AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                        Connection                 = "close"
                                                                    }.AsImmutable);

                                         });


            #region Get HTTP user and its organizations

            // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
            //if (!TryGetHTTPUser(Request,
            //                    out User                   HTTPUser,
            //                    out HashSet<Organization>  HTTPOrganizations,
            //                    out HTTPResponse           Response,
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
                                         URLPathPrefix + "users/{UserId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPRequestLogger:   SetUserRequest,
                                         HTTPResponseLogger:  SetUserResponse,
                                         HTTPDelegate:        async Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out User                   HTTPUser,
                                                                 out HashSet<Organization>  HTTPOrganizations,
                                                                 out HTTPResponse           Response,
                                                                 AccessLevel:               Access_Levels.ReadWrite,
                                                                 Recursive:                 true))
                                             {
                                                 return Response;
                                             }

                                             #endregion

                                             #region Check UserId URI parameter

                                             if (!Request.ParseUserId(this,
                                                                      out User_Id?      UserIdURI,
                                                                      out HTTPResponse  HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion

                                             #region Parse JSON

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out HTTPResponse))
                                                 return HTTPResponse;

                                             if (!User.TryParseJSON(JSONObj,
                                                                    out User    _User,
                                                                    out String  ErrorResponse,
                                                                    UserIdURI))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
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
                                                        }.AsImmutable;

                                             }

                                             #endregion


                                             // Has the current HTTP user the required
                                             // access rights to update?
                                             if (HTTPUser.Id != _User.Id)
                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                                            Server                      = HTTPServer.DefaultServerName,
                                                            Date                        = DateTime.UtcNow,
                                                            AccessControlAllowOrigin    = "*",
                                                            AccessControlAllowMethods   = "GET, SET, CHOWN",
                                                            AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                            Connection                  = "close"
                                                        }.AsImmutable;


                                             await AddOrUpdate(_User,
                                                               null,
                                                               null,
                                                               HTTPUser.Id);


                                             return new HTTPResponse.Builder(Request) {
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
                                                    }.AsImmutable;

                                         });

            #endregion

            #region AUTH        ~/users/{UserId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.AUTH,
                                          HTTPPath.Parse("/users/{UserId}"),
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate:  async Request => {

                                              #region Check JSON body...

                                              //var Body = HTTPRequest.ParseJSONRequestBody();
                                              //if (Body.HasErrors)
                                              //    return Body.Error;

                                              if (!Request.TryParseJObjectRequestBody(out JObject LoginData, out HTTPResponse _HTTPResponse))
                                                  return _HTTPResponse;


                                              if (!LoginData.HasValues)
                                                  return new HTTPResponse.Builder(Request) {
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
                                                         }.AsImmutable;

                                              #endregion

                                              #region Verify username

                                              // The login is taken from the URI, not from the JSON!
                                              var Login = Request.ParsedURIParameters[0];

                                              if (Login.Length < MinLoginLenght)
                                              {

                                                  return new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                             Server          = HTTPServer.DefaultServerName,
                                                             ContentType     = HTTPContentType.JSON_UTF8,
                                                             Content         = new JObject(
                                                                                   new JProperty("@context",     SignInOutContext),
                                                                                   new JProperty("statuscode",   400),
                                                                                   new JProperty("property",     "user identification"),
                                                                                   new JProperty("description",  "The login is too short!")
                                                                               ).ToString().ToUTF8Bytes(),
                                                             CacheControl    = "private",
                                                             Connection      = "close"
                                                         }.AsImmutable;

                                              }

                                              LoginData["username"] = Login;

                                              #endregion

                                              #region Verify realm

                                              var Realm = LoginData.GetString("realm");

                                              if (Realm.IsNotNullOrEmpty() &&
                                                  Realm.Length < MinRealmLenght)

                                              {

                                                  return new HTTPResponse.Builder(Request) {
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
                                                         }.AsImmutable;

                                              }

                                              #endregion

                                              #region Verify password

                                              var Password = LoginData.GetString("password");

                                              if (Password.IsNullOrEmpty())
                                              {

                                                  return new HTTPResponse.Builder(Request) {
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
                                                         }.AsImmutable;

                                              }

                                              var passwordQuality = PasswordQualityCheck(Password);

                                              if (passwordQuality < 1.0)
                                              {

                                                  return new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                             Server          = HTTPServer.DefaultServerName,
                                                             ContentType     = HTTPContentType.JSON_UTF8,
                                                             Content         = new JObject(
                                                                                   new JProperty("@context",     SignInOutContext),
                                                                                   new JProperty("statuscode",   400),
                                                                                   new JProperty("property",     "password"),
                                                                                   new JProperty("description",  "The password does not match the password quality criteria!")
                                                                              ).ToString().ToUTF8Bytes(),
                                                             CacheControl    = "private",
                                                             Connection      = "close"
                                                         }.AsImmutable;

                                              }

                                              LoginData["passwordQuality"] = passwordQuality;

                                              #endregion

                                              #region Check login or e-mail address and password(s)

                                              var validUsers = new HashSet<User>();

                                              if ((Realm.IsNotNullOrEmpty()
                                                       ? User_Id.TryParse(Login, Realm, out User_Id       _UserId)
                                                       : User_Id.TryParse(Login,        out               _UserId))       &&
                                                  _Users.         TryGetValue(_UserId,  out User          _User)          &&
                                                  _LoginPasswords.TryGetValue(_UserId,  out LoginPassword _LoginPassword) &&
                                                  _LoginPassword.VerifyPassword(Password))
                                              {
                                                  validUsers.Add(_User);
                                              }

                                              if (validUsers.Count == 0)
                                              {
                                                  foreach (var user in _Users.Values)
                                                  {
                                                      if (String.Equals(Login, user.EMail.Address.ToString(), StringComparison.OrdinalIgnoreCase))
                                                      {
                                                          if (_LoginPasswords.TryGetValue(user.Id, out LoginPassword loginPassword) &&
                                                              loginPassword.VerifyPassword(Password))
                                                          {
                                                              validUsers.Add(user);
                                                          }
                                                      }
                                                  }
                                              }

                                              if (!validUsers.Any())
                                                  return new HTTPResponse.Builder(Request) {
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
                                                         };

                                              if (validUsers.Count > 1)
                                                  return new HTTPResponse.Builder(Request) {
                                                             HTTPStatusCode  = HTTPStatusCode.MultipleChoices,
                                                             Server          = HTTPServer.DefaultServerName,
                                                             ContentType     = HTTPContentType.JSON_UTF8,
                                                             Content         = new JObject(
                                                                                   new JProperty("@context",     SignInOutContext),
                                                                                   new JProperty("property",     "login"),
                                                                                   new JProperty("description",  "Multiple matching user accounts found: Please use your login name!")
                                                                               ).ToString().ToUTF8Bytes(),
                                                             CacheControl    = "private",
                                                             Connection      = "close"
                                                         };

                                              #endregion

                                              var validUser = validUsers.First();

                                              #region Check EULA

                                              var acceptsEULA = LoginData["acceptsEULA"].Value<Boolean>();

                                              if (!validUser.AcceptedEULA.HasValue)
                                              {

                                                  if (!acceptsEULA)
                                                  {

                                                      return new HTTPResponse.Builder(Request) {
                                                                 HTTPStatusCode  = HTTPStatusCode.Unauthorized,
                                                                 Server          = HTTPServer.DefaultServerName,
                                                                 ContentType     = HTTPContentType.JSON_UTF8,
                                                                 Content         = new JObject(
                                                                                       new JProperty("@context",     SignInOutContext),
                                                                                       new JProperty("showEULA",     true),
                                                                                       new JProperty("description",  "Please accept the end-user license agreement!")
                                                                                   ).ToString().ToUTF8Bytes(),
                                                                 CacheControl    = "private",
                                                                 Connection      = "close"
                                                             }.AsImmutable;

                                                  }

                                                  await Update(validUser.Id,
                                                               user => user.AcceptedEULA = DateTime.UtcNow);

                                              }

                                              #endregion


                                              var SHA256Hash    = new SHA256Managed();
                                              var SecurityToken = SHA256Hash.ComputeHash((Guid.NewGuid().ToString() + validUser.Id).ToUTF8Bytes()).ToHexString();

                                              return new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode  = HTTPStatusCode.Created,
                                                         ContentType     = HTTPContentType.TEXT_UTF8,
                                                         Content         = new JObject(
                                                                               new JProperty("@context",  SignInOutContext),
                                                                               new JProperty("login",     validUser.Id.ToString()),
                                                                               new JProperty("username",  validUser.Name),
                                                                               new JProperty("email",     validUser.EMail.Address.ToString())
                                                                           ).ToUTF8Bytes(),
                                                         CacheControl    = "private",
                                                         SetCookie       = CookieName + "=login="    + validUser.Id.ToString().ToBase64() +
                                                                                     ":username=" + validUser.Name.ToBase64() +
                                                                                   (IsAdmin(validUser) == Access_Levels.ReadOnly  ? ":isAdminRO" : "") +
                                                                                   (IsAdmin(validUser) == Access_Levels.ReadWrite ? ":isAdminRW" : "") +
                                                                                ":securitytoken=" + SecurityToken +
                                                                                     "; Expires=" + DateTime.UtcNow.Add(SignInSessionLifetime).ToRfc1123() +
                                                                                      (HTTPCookieDomain.IsNotNullOrEmpty()
                                                                                          ? "; Domain=" + HTTPCookieDomain
                                                                                          : "") +
                                                                                        "; Path=/",
                                                         // secure;"
                                                         Connection = "close"
                                                     }.AsImmutable;

                                          });

            #endregion

            #region DEAUTH      ~/users/{UserId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                          HTTPMethod.DEAUTH,
                                          HTTPPath.Parse("/users/{UserId}"),
                                          HTTPContentType.JSON_UTF8,
                                          HTTPDelegate: Request =>

                                              Task.FromResult(
                                                  new HTTPResponse.Builder(Request) {
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

            #region IMPERSONATE ~/users/{UserId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.IMPERSONATE,
                                         HTTPPath.Parse("/users/{UserId}"),
                                         HTTPContentType.JSON_UTF8,
                                         HTTPRequestLogger:   ImpersonateUserRequest,
                                         HTTPResponseLogger:  ImpersonateUserResponse,
                                         HTTPDelegate:        async Request => {

                                             #region Get astronaut and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetAstronaut(Request,
                                                                  out User                       Astronaut,
                                                                  out IEnumerable<Organization>  AstronautOrganizations,
                                                                  out HTTPResponse           Response,
                                                                  Recursive: true))
                                             {
                                                 return Response;
                                             }

                                             #endregion

                                             #region Check UserId URI parameter

                                             if (!Request.ParseUserId(this,
                                                                      out User_Id?      UserIdURI,
                                                                      out HTTPResponse  HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             if (!TryGet(UserIdURI.Value, out User UserURI))
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode              = HTTPStatusCode.NotFound,
                                                            Server                      = HTTPServer.DefaultServerName,
                                                            Date                        = DateTime.UtcNow,
                                                            AccessControlAllowOrigin    = "*",
                                                            AccessControlAllowMethods   = "IMPERSONATE",
                                                            AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                            Connection                  = "close"
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             #region Is the current user allowed to impersonate the given user?

                                             if (!CanImpersonate(Astronaut, UserURI))
                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                                            Server                      = HTTPServer.DefaultServerName,
                                                            Date                        = DateTime.UtcNow,
                                                            AccessControlAllowOrigin    = "*",
                                                            AccessControlAllowMethods   = "IMPERSONATE",
                                                            AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                            Connection                  = "close"
                                                        }.AsImmutable;

                                             #endregion


                                             #region Register security token

                                             var SHA256Hash       = new SHA256Managed();
                                             var SecurityTokenId  = SecurityToken_Id.Parse(SHA256Hash.ComputeHash(
                                                                                               String.Concat(Guid.NewGuid().ToString(),
                                                                                                             UserURI.Id).
                                                                                               ToUTF8Bytes()
                                                                                           ).ToHexString());

                                             var Expires          = DateTime.UtcNow.Add(SignInSessionLifetime);

                                             lock (HTTPCookies)
                                             {

                                                 HTTPCookies.Add(SecurityTokenId,
                                                                    new SecurityToken(UserURI.Id,
                                                                                      Expires,
                                                                                      Astronaut.Id));

                                                 File.AppendAllText(this.UsersAPIPath + DefaultHTTPCookiesFile,
                                                                    SecurityTokenId + ";" + UserURI.Id + ";" + Expires.ToIso8601() + ";" + Astronaut.Id + Environment.NewLine);

                                             }

                                             #endregion


                                             return new HTTPResponse.Builder(Request) {
                                                        HTTPStatusCode  = HTTPStatusCode.Created,
                                                        ContentType     = HTTPContentType.TEXT_UTF8,
                                                        Content         = new JObject(
                                                                              new JProperty("@context",  SignInOutContext),
                                                                              new JProperty("login",     UserURI.Id.ToString()),
                                                                              new JProperty("username",  UserURI.Name),
                                                                              new JProperty("email",     UserURI.EMail.Address.ToString())
                                                                          ).ToUTF8Bytes(),
                                                        CacheControl    = "private",
                                                        SetCookie       = CookieName + "=login="    + UserURI.Id.ToString().ToBase64() +
                                                                                    ":astronaut=" + Astronaut.Id.ToString().ToBase64() +
                                                                                    ":username=" + UserURI.Name.ToBase64() +
                                                                                  (IsAdmin(UserURI) == Access_Levels.ReadOnly  ? ":isAdminRO" : "") +
                                                                                  (IsAdmin(UserURI) == Access_Levels.ReadWrite ? ":isAdminRW" : "") +
                                                                               ":securitytoken=" + SecurityTokenId +
                                                                                    "; Expires=" + DateTime.UtcNow.Add(SignInSessionLifetime).ToRfc1123() +
                                                                                     (HTTPCookieDomain.IsNotNullOrEmpty()
                                                                                         ? "; Domain=" + HTTPCookieDomain
                                                                                         : "") +
                                                                                       "; Path=/",
                                                        // secure;"
                                                        Connection = "close"
                                                    }.AsImmutable;

                                         });

            #endregion

            #region DEPERSONATE ~/users/{UserId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.DEPERSONATE,
                                         HTTPPath.Parse("/users/{UserId}"),
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get astronaut and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetAstronaut(Request,
                                                                  out User                       Astronaut,
                                                                  out IEnumerable<Organization>  AstronautOrganizations,
                                                                  out HTTPResponse           Response,
                                                                  Recursive: true))
                                             {
                                                 return Task.FromResult(Response);
                                             }

                                             #endregion

                                             #region Check UserId URI parameter

                                             if (!Request.ParseUserId(this,
                                                                      out User_Id?      UserIdURI,
                                                                      out HTTPResponse  HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse);
                                             }

                                             if (!TryGet(UserIdURI.Value, out User UserURI))
                                             {

                                                 return Task.FromResult(
                                                            new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode              = HTTPStatusCode.NotFound,
                                                                Server                      = HTTPServer.DefaultServerName,
                                                                Date                        = DateTime.UtcNow,
                                                                AccessControlAllowOrigin    = "*",
                                                                AccessControlAllowMethods   = "DEPERSONATE",
                                                                AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                                Connection                  = "close"
                                                            }.AsImmutable);

                                             }

                                             #endregion

                                             #region Is the current user allowed to impersonate the given user?

                                             //if (UserURI.Id.ToString() == "ahzf" ||
                                             //    UserURI.Id.ToString() == "lars")
                                             //{

                                             //    return Task.FromResult(
                                             //               new HTTPResponse.Builder(Request) {
                                             //                   HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                             //                   Server                      = HTTPServer.DefaultServerName,
                                             //                   Date                        = DateTime.UtcNow,
                                             //                   AccessControlAllowOrigin    = "*",
                                             //                   AccessControlAllowMethods   = "IMPERSONATE",
                                             //                   AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                             //                   Connection                  = "close"
                                             //               }.AsImmutable);

                                             //}

                                             #endregion


                                             #region Switch back to astronaut user identification...

                                             var SHA256Hash       = new SHA256Managed();
                                             var SecurityTokenId  = SecurityToken_Id.Parse(SHA256Hash.ComputeHash(
                                                                                               String.Concat(Guid.NewGuid().ToString(),
                                                                                                             Astronaut.Id).
                                                                                               ToUTF8Bytes()
                                                                                           ).ToHexString());

                                             var Expires          = DateTime.UtcNow.Add(SignInSessionLifetime);

                                             lock (HTTPCookies)
                                             {

                                                 HTTPCookies.Add(SecurityTokenId,
                                                                 new SecurityToken(Astronaut.Id,
                                                                                   Expires));

                                                 File.AppendAllText(this.UsersAPIPath + DefaultHTTPCookiesFile,
                                                                    SecurityTokenId + ";" + Astronaut.Id + ";" + Expires.ToIso8601() + Environment.NewLine);

                                             }

                                             #endregion


                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode  = HTTPStatusCode.Created,
                                                     ContentType     = HTTPContentType.TEXT_UTF8,
                                                     Content         = new JObject(
                                                                           new JProperty("@context",  SignInOutContext),
                                                                           new JProperty("login",     Astronaut.Id.ToString()),
                                                                           new JProperty("username",  Astronaut.Name),
                                                                           new JProperty("email",     Astronaut.EMail.Address.ToString())
                                                                       ).ToUTF8Bytes(),
                                                     CacheControl    = "private",
                                                     SetCookie       = CookieName + "=login="    + Astronaut.Id.ToString().ToBase64() +
                                                                                 ":username=" + Astronaut.Name.ToBase64() +
                                                                               (IsAdmin(Astronaut) == Access_Levels.ReadOnly  ? ":isAdminRO" : "") +
                                                                               (IsAdmin(Astronaut) == Access_Levels.ReadWrite ? ":isAdminRW" : "") +
                                                                            ":securitytoken=" + SecurityTokenId +
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


            #region GET         ~/users/{UserId}/profilephoto

            HTTPServer.RegisterFilesystemFile(HTTPHostname.Any,
                                              URLPathPrefix + "users/{UserId}/profilephoto",
                                              URIParams => "LocalHTTPRoot/data/Users/" + URIParams[0] + ".png",
                                              DefaultFile: "HTTPRoot/images/defaults/DefaultUser.png");

            #endregion

            #region GET         ~/users/{UserId}/notifications

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/users/ahzf/notifications
            // --------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "users/{UserId}/notifications",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out User                   HTTPUser,
                                                                 out HashSet<Organization>  HTTPOrganizations,
                                                                 out HTTPResponse           Response,
                                                                 Recursive: true))
                                             {
                                                 return Task.FromResult(Response);
                                             }

                                             #endregion

                                             return Task.FromResult(new HTTPResponse.Builder(Request) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        Server                     = HTTPServer.DefaultServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, SET",
                                                        AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                        ETag                       = "1",
                                                        ContentType                = HTTPContentType.JSON_UTF8,
                                                        Content                    = GetNotificationInfos(HTTPUser).ToUTF8Bytes(),
                                                        Connection                 = "close"
                                                    }.AsImmutable);

            });

            #endregion

            #region SET         ~/users/{UserId}/notifications

            // ---------------------------------------------------------------------------------------------
            // curl -v -X SET \
            //      -H "Accept:       application/json; charset=utf-8" \
            //      -H "Content-Type: application/json; charset=utf-8" \
            //      -d "{ \
            //          }" \
            //      http://127.0.0.1:2000/users/214080158
            // ---------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.SET,
                                         URLPathPrefix + "users/{UserId}/notifications",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPRequestLogger:   SetUserNotificationsRequest,
                                         HTTPResponseLogger:  SetUserNotificationsResponse,
                                         HTTPDelegate:        async Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out User                   HTTPUser,
                                                                 out HashSet<Organization>  HTTPOrganizations,
                                                                 out HTTPResponse           HTTPResponse,
                                                                 AccessLevel:               Access_Levels.ReadWrite,
                                                                 Recursive:                 true))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion

                                             #region Check UserId URI parameter

                                             if (!Request.ParseUser(this,
                                                                    out User_Id?  UserIdURI,
                                                                    out User      User,
                                                                    out           HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion

                                             #region Has the current HTTP user the required access rights to update?

                                             if (UserIdURI != HTTPUser.Id)
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                                            Server                      = HTTPServer.DefaultServerName,
                                                            Date                        = DateTime.UtcNow,
                                                            AccessControlAllowOrigin    = "*",
                                                            AccessControlAllowMethods   = "GET, SET, CHOWN",
                                                            AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                            Connection                  = "close"
                                                        }.AsImmutable;

                                             }

                                             #endregion


                                             #region Parse JSON and new notifications...

                                             if (!Request.TryParseJArrayRequestBody(out JArray JSONArray, out HTTPResponse))
                                                 return HTTPResponse;

                                             String ErrorString = null;

                                             if (JSONArray.Count > 0)
                                             {

                                                 var JSONObjects = JSONArray.Cast<JObject>().ToArray();

                                                 if (!JSONObjects.Any())
                                                     goto fail;

                                                 String context = null;

                                                 foreach (var JSONObject in JSONObjects)
                                                 {

                                                     context = JSONObject["@context"]?.Value<String>();

                                                     if (context.IsNullOrEmpty())
                                                         goto fail;

                                                     switch (context)
                                                     {

                                                         case EMailNotification.JSONLDContext:
                                                             if (!EMailNotification.TryParse(JSONObject, out EMailNotification eMailNotification))
                                                             {
                                                                 ErrorString = "Could not parse e-mail notification!";
                                                                 goto fail;
                                                             }
                                                             await AddNotification(HTTPUser, eMailNotification, HTTPUser.Id);
                                                             break;

                                                         case SMSNotification.JSONLDContext:
                                                             if (!SMSNotification.  TryParse(JSONObject, out SMSNotification   smsNotification))
                                                             {
                                                                 ErrorString = "Could not parse sms notification!";
                                                                 goto fail;
                                                             }
                                                             await AddNotification(HTTPUser, smsNotification, HTTPUser.Id);
                                                             break;

                                                         case HTTPSNotification.JSONLDContext:
                                                             if (!HTTPSNotification.TryParse(JSONObject, out HTTPSNotification httpsNotification))
                                                             {
                                                                 ErrorString = "Could not parse https notification!";
                                                                 goto fail;
                                                             }
                                                             await AddNotification(HTTPUser, httpsNotification, HTTPUser.Id);
                                                             break;

                                                         case TelegramNotification.JSONLDContext:
                                                             if (!TelegramNotification.TryParse(JSONObject, out TelegramNotification telegramNotification))
                                                             {
                                                                 ErrorString = "Could not parse Telegram notification!";
                                                                 goto fail;
                                                             }
                                                             await AddNotification(HTTPUser, telegramNotification, HTTPUser.Id);
                                                             break;

                                                         default:
                                                             goto fail;

                                                     }

                                                 }

                                             }

                                             goto goon;

                                             #region fail...

                                             fail:

                                             return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = DateTime.UtcNow,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "GET, SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ETag                       = "1",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = JSONObject.Create(
                                                                                             new JProperty("description", ErrorString ?? "Invalid array of notifications!")
                                                                                         ).ToUTF8Bytes()
                                                        }.AsImmutable;

                                             #endregion

                                             goon:

                                             #endregion


                                             return new HTTPResponse.Builder(Request) {
                                                        HTTPStatusCode              = HTTPStatusCode.OK,
                                                        Server                      = HTTPServer.DefaultServerName,
                                                        Date                        = DateTime.UtcNow,
                                                        AccessControlAllowOrigin    = "*",
                                                        AccessControlAllowMethods   = "GET, SET",
                                                        AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                        ContentType                 = HTTPContentType.JSON_UTF8,
                                                        Content                     = GetNotificationInfos(HTTPUser).ToUTF8Bytes(),
                                                        Connection                  = "close"
                                                    }.AsImmutable;

                                         });

            #endregion

            #region DELETE      ~/users/{UserId}/notifications

            // ---------------------------------------------------------------------------------------------
            // curl -v -X DELETE \
            //      -H "Accept:       application/json; charset=utf-8" \
            //      -H "Content-Type: application/json; charset=utf-8" \
            //      -d "{ \
            //          }" \
            //      http://127.0.0.1:2000/users/214080158
            // ---------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.DELETE,
                                         URLPathPrefix + "users/{UserId}/notifications",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPRequestLogger:   DeleteUserNotificationsRequest,
                                         HTTPResponseLogger:  DeleteUserNotificationsResponse,
                                         HTTPDelegate:        async Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out User                   HTTPUser,
                                                                 out HashSet<Organization>  HTTPOrganizations,
                                                                 out HTTPResponse               HTTPResponse,
                                                                 AccessLevel:               Access_Levels.ReadWrite,
                                                                 Recursive:                 true))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion

                                             #region Check UserId URI parameter

                                             if (!Request.ParseUser(this,
                                                                    out User_Id?  UserIdURI,
                                                                    out User      User,
                                                                    out           HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion

                                             #region Has the current HTTP user the required access rights to update?

                                             if (UserIdURI != HTTPUser.Id)
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                                            Server                      = HTTPServer.DefaultServerName,
                                                            Date                        = DateTime.UtcNow,
                                                            AccessControlAllowOrigin    = "*",
                                                            AccessControlAllowMethods   = "GET, SET, CHOWN",
                                                            AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                            Connection                  = "close"
                                                        }.AsImmutable;

                                             }

                                             #endregion


                                             #region Parse JSON and new notifications...

                                             if (!Request.TryParseJArrayRequestBody(out JArray JSONArray, out HTTPResponse))
                                                 return HTTPResponse;

                                             String ErrorString = null;

                                             if (JSONArray.Count > 0)
                                             {

                                                 var JSONObjects = JSONArray.Cast<JObject>().ToArray();

                                                 if (!JSONObjects.Any())
                                                     goto fail;

                                                 String context = null;

                                                 foreach (var JSONObject in JSONObjects)
                                                 {

                                                     context = JSONObject["@context"]?.Value<String>();

                                                     if (context.IsNullOrEmpty())
                                                         goto fail;

                                                     switch (context)
                                                     {

                                                         case TelegramNotification.JSONLDContext:
                                                             if (!TelegramNotification.TryParse(JSONObject, out TelegramNotification telegramNotification))
                                                             {
                                                                 ErrorString = "Could not parse Telegram notification!";
                                                                 goto fail;
                                                             }
                                                             await RemoveNotification(HTTPUser, telegramNotification, HTTPUser.Id);
                                                             break;

                                                         case SMSNotification.JSONLDContext:
                                                             if (!SMSNotification.  TryParse(JSONObject, out SMSNotification   smsNotification))
                                                             {
                                                                 ErrorString = "Could not parse sms notification!";
                                                                 goto fail;
                                                             }
                                                             await RemoveNotification(HTTPUser, smsNotification, HTTPUser.Id);
                                                             break;

                                                         case HTTPSNotification.JSONLDContext:
                                                             if (!HTTPSNotification.TryParse(JSONObject, out HTTPSNotification httpsNotification))
                                                             {
                                                                 ErrorString = "Could not parse https notification!";
                                                                 goto fail;
                                                             }
                                                             await RemoveNotification(HTTPUser, httpsNotification, HTTPUser.Id);
                                                             break;

                                                         case EMailNotification.JSONLDContext:
                                                             if (!EMailNotification.TryParse(JSONObject, out EMailNotification eMailNotification))
                                                             {
                                                                 ErrorString = "Could not parse e-mail notification!";
                                                                 goto fail;
                                                             }
                                                             await RemoveNotification(HTTPUser, eMailNotification, HTTPUser.Id);
                                                             break;

                                                         default:
                                                             goto fail;

                                                     }

                                                 }

                                             }

                                             goto goon;

                                             #region fail...

                                             fail:

                                             return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = DateTime.UtcNow,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "GET, SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            ETag                       = "1",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = JSONObject.Create(
                                                                                             new JProperty("description", ErrorString ?? "Invalid array of notifications!")
                                                                                         ).ToUTF8Bytes()
                                                        }.AsImmutable;

                                             #endregion

                                             goon:

                                             #endregion


                                             return new HTTPResponse.Builder(Request) {
                                                        HTTPStatusCode              = HTTPStatusCode.OK,
                                                        Server                      = HTTPServer.DefaultServerName,
                                                        Date                        = DateTime.UtcNow,
                                                        AccessControlAllowOrigin    = "*",
                                                        AccessControlAllowMethods   = "GET, SET",
                                                        AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                        ContentType                 = HTTPContentType.JSON_UTF8,
                                                        Content                     = GetNotificationInfos(HTTPUser).ToUTF8Bytes(),
                                                        Connection                  = "close"
                                                    }.AsImmutable;

                                         });

            #endregion

            #region GET         ~/users/{UserId}/organizations

            // ------------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2000/users/{UserId}/organizations
            // ------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "users/{UserId}/organizations",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out User                   HTTPUser,
                                                                 out HashSet<Organization>  HTTPOrganizations,
                                                                 out HTTPResponse           Response,
                                                                 Recursive: true))
                                             {
                                                 return Task.FromResult(Response);
                                             }

                                             #endregion

                                             var AllMyOrganizations = new OrganizationInfo(NoOwner, HTTPUser).Childs;

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = DateTime.UtcNow,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = "GET, COUNT, OPTIONS",
                                                     AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                     ETag                       = "1",
                                                     ContentType                = HTTPContentType.JSON_UTF8,
                                                     Content                    = AllMyOrganizations.ToJSON().ToUTF8Bytes()
                                                 }.AsImmutable);

                                         });

            #endregion

            #region GET         ~/users/{UserId}/APIKeys

            // --------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/users/ahzf/APIKeys
            // --------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "users/{UserId}/APIKeys",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out User                   HTTPUser,
                                                                 out HashSet<Organization>  HTTPOrganizations,
                                                                 out HTTPResponse           Response,
                                                                 Recursive: true))
                                             {
                                                 return Task.FromResult(Response);
                                             }

                                             #endregion

                                             var __APIKeys = _APIKeys.Values.Where(apikey => apikey.User == HTTPUser).ToArray();

                                             return Task.FromResult(__APIKeys != null

                                                                        ? new HTTPResponse.Builder(Request) {
                                                                                  HTTPStatusCode             = HTTPStatusCode.OK,
                                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                                  Date                       = DateTime.UtcNow,
                                                                                  AccessControlAllowOrigin   = "*",
                                                                                  AccessControlAllowMethods  = "GET, SET",
                                                                                  AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                                  ETag                       = "1",
                                                                                  ContentType                = HTTPContentType.JSON_UTF8,
                                                                                  Content                    = JSONArray.Create(

                                                                                                                   __APIKeys.Select(_ => _.ToJSON(true))

                                                                                                               ).ToUTF8Bytes(),
                                                                                  Connection                 = "close"
                                                                              }.AsImmutable

                                                                        : new HTTPResponse.Builder(Request) {
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

            #region SET         ~/users/{UserId}/password


            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.SET,
                                         URLPathPrefix + "users/{UserId}/password",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPRequestLogger:   ChangePasswordRequest,
                                         HTTPResponseLogger:  ChangePasswordResponse,
                                         HTTPDelegate:        async Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out User                   HTTPUser,
                                                                 out HashSet<Organization>  HTTPOrganizations,
                                                                 out HTTPResponse           Response,
                                                                 AccessLevel:               Access_Levels.ReadWrite,
                                                                 Recursive:                 true))
                                             {
                                                 return Response;
                                             }

                                             #endregion

                                             #region Check UserId URI parameter

                                             if (!Request.ParseUserId(this,
                                                                      out User_Id?      UserIdURI,
                                                                      out HTTPResponse  HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion

                                             #region Parse JSON

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out HTTPResponse))
                                                 return HTTPResponse;

                                             var ErrorResponse    = "";
                                             var CurrentPassword  = JSONObj.GetString("currentPassword");
                                             var NewPassword      = JSONObj.GetString("newPassword");

                                             if (CurrentPassword.IsNullOrEmpty() || NewPassword.IsNullOrEmpty())
                                             {

                                                 return new HTTPResponse.Builder(Request) {
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
                                                            }.AsImmutable;

                                             }

                                             #endregion


                                             // Has the current HTTP user the required
                                             // access rights to update?
                                             if (HTTPUser.Id != UserIdURI.Value)
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                                            Server                      = HTTPServer.DefaultServerName,
                                                            Date                        = DateTime.UtcNow,
                                                            AccessControlAllowOrigin    = "*",
                                                            AccessControlAllowMethods   = "GET, SET, CHOWN",
                                                            AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                            Connection                  = "close"
                                                        }.AsImmutable;

                                             }

                                             if (TryChangePassword(UserIdURI.Value,
                                                                   Password.Parse(NewPassword),
                                                                   CurrentPassword,
                                                                   HTTPUser.Id).Result)
                                             {

                                                 var MailSentResult = await APISMTPClient.Send(PasswordChangedEMailCreator(HTTPUser.Id,
                                                                                                                           HTTPUser.EMail,
                                                                                                                           HTTPUser.Name,
                                                                                                                           "https://" + Request.Host.SimpleString,
                                                                                                                           DefaultLanguage));

                                                 return new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode              = HTTPStatusCode.OK,
                                                                Server                      = HTTPServer.DefaultServerName,
                                                                Date                        = DateTime.UtcNow,
                                                                AccessControlAllowOrigin    = "*",
                                                                AccessControlAllowMethods   = "SET",
                                                                AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                                Connection                  = "close"
                                                            }.AsImmutable;
                                             }

                                             else
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                                                Server                      = HTTPServer.DefaultServerName,
                                                                Date                        = DateTime.UtcNow,
                                                                AccessControlAllowOrigin    = "*",
                                                                AccessControlAllowMethods   = "SET",
                                                                AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                                Connection                  = "close"
                                                            }.AsImmutable;

                                             }


                                         });

            #endregion

            #endregion

            #region ~/organizations

            #region GET         ~/organizations

            // ---------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2000/organizations
            // ---------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "organizations",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Try to get HTTP user and its organizations

                                             TryGetHTTPUser(Request,
                                                            out User                   HTTPUser,
                                                            out HashSet<Organization>  HTTPOrganizations,
                                                            out HTTPResponse           Response,
                                                            Recursive: true);

                                             #endregion

                                             var includeFilter           = Request.QueryString.CreateStringFilter<Organization>("include",
                                                                                                                                (organization, include) => organization.Id.ToString().Contains(include) ||
                                                                                                                                                           organization.Name.       Matches(include, IgnoreCase: true) ||
                                                                                                                                                           organization.Description.Matches(include, IgnoreCase: true));

                                             var skip                    = Request.QueryString.GetUInt64 ("skip");
                                             var take                    = Request.QueryString.GetUInt64 ("take");

                                             var includeCryptoHash       = Request.QueryString.GetBoolean("includeCryptoHash", true);

                                             var expand                  = Request.QueryString.GetStrings("expand", true);
                                             var expandMembers           = expand.Contains("members")           ? InfoStatus.Expand : InfoStatus.ShowIdOnly;
                                             var expandParents           = expand.Contains("parents")           ? InfoStatus.Expand : InfoStatus.ShowIdOnly;
                                             var expandSubOrganizations  = expand.Contains("subOrganizations")  ? InfoStatus.Expand : InfoStatus.ShowIdOnly;
                                             var expandTags              = expand.Contains("tags")              ? InfoStatus.Expand : InfoStatus.ShowIdOnly;


                                             //ToDo: Getting the expected total count might be very expensive!
                                             var _ExpectedCount = Organizations.ULongCount();

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode                 = HTTPStatusCode.OK,
                                                     Server                         = HTTPServer.DefaultServerName,
                                                     Date                           = DateTime.UtcNow,
                                                     AccessControlAllowOrigin       = "*",
                                                     AccessControlAllowMethods      = "GET, COUNT, OPTIONS",
                                                     AccessControlAllowHeaders      = "Content-Type, Accept, Authorization",
                                                     ETag                           = "1",
                                                     ContentType                    = HTTPContentType.JSON_UTF8,
                                                     Content                        = HTTPOrganizations.
                                                                                          OrderBy(organization => organization.Name.FirstText()).
                                                                                          Where  (includeFilter).
                                                                                          ToJSON (skip,
                                                                                                  take,
                                                                                                  false, //Embedded
                                                                                                  expandMembers,
                                                                                                  expandParents,
                                                                                                  expandSubOrganizations,
                                                                                                  expandTags,
                                                                                                  GetOrganizationSerializator(Request, HTTPUser),
                                                                                                  includeCryptoHash).
                                                                                          ToUTF8Bytes(),
                                                     X_ExpectedTotalNumberOfItems   = _ExpectedCount,
                                                     Connection                     = "close",
                                                     Vary                           = "Accept"
                                                 }.AsImmutable);

                                         });

            #endregion

            #region COUNT       ~/organizations

            // ------------------------------------------------------------------------------------
            // curl -v -X COUNT -H "Accept: application/json" http://127.0.0.1:2000/organizations
            // ------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.COUNT,
                                         URLPathPrefix + "organizations",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out User                   HTTPUser,
                                                                 out HashSet<Organization>  HTTPOrganizations,
                                                                 out HTTPResponse           Response,
                                                                 Recursive: true))
                                             {
                                                 return Task.FromResult(Response);
                                             }

                                             #endregion

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = DateTime.UtcNow,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = "GET, COUNT, OPTIONS",
                                                     AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                     ETag                       = "1",
                                                     ContentType                = HTTPContentType.JSON_UTF8,
                                                     Content                    = JSONObject.Create(
                                                                                      new JProperty("count", Organizations.ULongCount())
                                                                                  ).ToUTF8Bytes(),
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         });

            #endregion

            #region Add         ~/organizations

            // ---------------------------------------------------------------------------------------------
            // curl -v -X Add \
            //      -H "Accept:       application/json; charset=utf-8" \
            //      -H "Content-Type: application/json; charset=utf-8" \
            //      -d "{ \
            //              \"@context\" :             \"https://cardi-link.cloud/contexts/cardidb+json/user\", \
            //              \"name\" :                 { \"deu\" : \"ACME Inc.\" },\
            //              \"description\" :          { \"deu\" : \"Testing123!\" },\
            //              \"parentOrganization\" :   \"GraphDefined\", \
            //              \"address\" :         { \
            //                                      \"country\" :      \"Germany\",
            //                                      \"postalCode\" :   \"00749\",
            //                                      \"city\" :         { \"deu\": \"Jena\" },
            //                                      \"street\" :       \"Biberweg 18\",
            //                                      \"houseNumber\" :  \"18\",
            //                                      \"floorLevel\" :   \"2\"
            //                                    }, \
            //              \"privacyLevel\" :    \"World\" \
            //          }" \
            //      http://127.0.0.1:2000/organizations
            // ---------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.ADD,
                                         URLPathPrefix + "organizations",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPRequestLogger:  AddOrganizationRequest,
                                         HTTPResponseLogger: AddOrganizationResponse,
                                         HTTPDelegate:       async Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out User                   HTTPUser,
                                                                 out HashSet<Organization>  HTTPOrganizations,
                                                                 out HTTPResponse           Response,
                                                                 AccessLevel:               Access_Levels.ReadWrite,
                                                                 Recursive:                 true))
                                             {
                                                 return Response;
                                             }

                                             #endregion

                                             #region Parse JSON and create the new child organization...

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out HTTPResponse HTTPResponse))
                                                 return HTTPResponse;

                                             if (Organization.TryParseJSON(JSONObj,
                                                                           out Organization    newOrganization,
                                                                           out String          ErrorResponse,
                                                                           OrganizationIdURI:  Organization_Id.Random()))
                                             {

                                                 #region Parse parent organization

                                                 if (!JSONObj.ParseMandatory("parentOrganization",
                                                                             "parent organization",
                                                                             HTTPServer.DefaultHTTPServerName,
                                                                             Organization_Id.TryParse,
                                                                             out Organization_Id ParentOrganizationId,
                                                                             Request,
                                                                             out HTTPResponse))
                                                 {
                                                     return HTTPResponse;
                                                 }

                                                 if (!_Organizations.TryGetValue(ParentOrganizationId, out Organization ParentOrganization))
                                                 {

                                                     return new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                                Server                     = HTTPServer.DefaultServerName,
                                                                Date                       = DateTime.UtcNow,
                                                                AccessControlAllowOrigin   = "*",
                                                                AccessControlAllowMethods  = "GET, SET",
                                                                AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                ContentType                = HTTPContentType.JSON_UTF8,
                                                                Content                    = JSONObject.Create(
                                                                                                 new JProperty("description",  "Unknown parent organization!")
                                                                                             ).ToUTF8Bytes()
                                                            }.AsImmutable;

                                                 }

                                                 #endregion

                                                 try
                                                 {

                                                     var _NewChildOrganization = await Add(newOrganization,
                                                                                           ParentOrganization: ParentOrganization,
                                                                                           CurrentUserId:      HTTPUser.Id);

                                                     return new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode              = HTTPStatusCode.Created,
                                                                Server                      = HTTPServer.DefaultServerName,
                                                                Date                        = DateTime.UtcNow,
                                                                AccessControlAllowOrigin    = "*",
                                                                AccessControlAllowMethods   = "GET, SET",
                                                                AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                                ContentType                 = HTTPContentType.JSON_UTF8,
                                                                Content                     = _NewChildOrganization.ToJSON().ToUTF8Bytes(),
                                                                Connection                  = "close"
                                                            }.AsImmutable;

                                                 }
                                                 catch (Exception e)
                                                 {

                                                     return new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                                Server                     = HTTPServer.DefaultServerName,
                                                                Date                       = DateTime.UtcNow,
                                                                AccessControlAllowOrigin   = "*",
                                                                AccessControlAllowMethods  = "GET, SET",
                                                                AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                ContentType                = HTTPContentType.JSON_UTF8,
                                                                Content                    = JSONObject.Create(
                                                                                                 new JProperty("description",  "Could not create the given child organization! " + e.Message)
                                                                                             ).ToUTF8Bytes()
                                                            }.AsImmutable;

                                                 }

                                             }

                                             #endregion

                                             return new HTTPResponse.Builder(Request) {
                                                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                        Server                     = HTTPServer.DefaultServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, SET",
                                                        AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                        ContentType                = HTTPContentType.JSON_UTF8,
                                                        Content                    = JSONObject.Create(
                                                                                         new JProperty("description",  "Could not parse the given child organization data!")
                                                                                     ).ToUTF8Bytes()
                                                    }.AsImmutable;

                                         });

            #endregion

            #endregion

            #region ~/organizations/{OrganizationId}

            #region GET         ~/organizations/{OrganizationId}

            // -------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2000/organizations/214080158
            // -------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "organizations/{OrganizationId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Try to get HTTP user and its organizations

                                             TryGetHTTPUser(Request,
                                                            out User                   HTTPUser,
                                                            out HashSet<Organization>  HTTPOrganizations,
                                                            out HTTPResponse           Response,
                                                            Recursive: true);

                                             #endregion

                                             #region Check OrganizationId URI parameter

                                             if (!Request.ParseOrganization(this,
                                                                            out Organization_Id?  OrganizationId,
                                                                            out Organization      Organization,
                                                                            out HTTPResponse      HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse);
                                             }

                                             #endregion

                                             var showMgt                 = Request.QueryString.GetBoolean("showMgt", false);

                                             var expand                  = Request.QueryString.GetStrings("expand",  false);
                                             var expandMembers           = expand.Contains("members")           ? InfoStatus.Expand : InfoStatus.ShowIdOnly;
                                             var expandParents           = expand.Contains("parents")           ? InfoStatus.Expand : InfoStatus.ShowIdOnly;
                                             var expandSubOrganizations  = expand.Contains("subOrganizations")  ? InfoStatus.Expand : InfoStatus.ShowIdOnly;
                                             var expandTags              = expand.Contains("tags")              ? InfoStatus.Expand : InfoStatus.ShowIdOnly;

                                             var includeCryptoHash       = Request.QueryString.GetBoolean("includeCryptoHash", true);

                                             return Task.FromResult(
                                                        (Organization.PrivacyLevel == PrivacyLevel.Private &&
                                                         !HTTPOrganizations.Contains(Organization))

                                                            ? new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = DateTime.UtcNow,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, EXISTS",
                                                                  AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                  Connection                 = "close"
                                                              }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.OK,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = DateTime.UtcNow,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET, EXISTS",
                                                                  AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                  ContentType                = HTTPContentType.JSON_UTF8,
                                                                  Content                    = (showMgt == true
                                                                                                    ? new OrganizationInfo2(Organization,
                                                                                                                           HTTPUser).ToJSON(false,
                                                                                                                                            expandMembers,
                                                                                                                                            expandParents,
                                                                                                                                            expandSubOrganizations,
                                                                                                                                            expandTags,
                                                                                                                                            includeCryptoHash)
                                                                                                    : Organization.ToJSON(false,
                                                                                                                          expandMembers,
                                                                                                                          expandParents,
                                                                                                                          expandSubOrganizations,
                                                                                                                          expandTags,
                                                                                                                          includeCryptoHash)).ToUTF8Bytes(),
                                                                  Connection                 = "close",
                                                                  Vary                       = "Accept"
                                                              }.AsImmutable);

                                         });

            #endregion

            #region EXISTS      ~/organizations/{OrganizationId}

            // ---------------------------------------------------------
            // curl -v -X EXISTS http://127.0.0.1:2000/organizations/7
            // ---------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.EXISTS,
                                         URLPathPrefix + "organizations/{OrganizationId}",
                                         HTTPDelegate: Request => {

                                             #region Try to get HTTP user and its organizations

                                             TryGetHTTPUser(Request,
                                                            out User                   HTTPUser,
                                                            out HashSet<Organization>  HTTPOrganizations,
                                                            out HTTPResponse           Response,
                                                            Recursive: true);

                                             #endregion

                                             #region Check OrganizationId URI parameter

                                             if (!Request.ParseOrganization(this,
                                                                            out Organization_Id?  OrganizationId,
                                                                            out Organization      Organization,
                                                                            out HTTPResponse      HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse);
                                             }

                                             #endregion


                                             return Task.FromResult(
                                                        new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode             = HTTPStatusCode.OK,
                                                                Server                     = HTTPServer.DefaultServerName,
                                                                Date                       = DateTime.UtcNow,
                                                                AccessControlAllowOrigin   = "*",
                                                                AccessControlAllowMethods  = "GET, EXISTS",
                                                                AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                Connection                 = "close"
                                                            }.AsImmutable);

                                         });

            #endregion

            #region ADD         ~/organizations/{organizationId}

            // ---------------------------------------------------------------------------------------------
            // curl -v -X ADD \
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
                                         HTTPMethod.ADD,
                                         URLPathPrefix + "organizations/{organizationId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPRequestLogger:  AddOrganizationRequest,
                                         HTTPResponseLogger: AddOrganizationResponse,
                                         HTTPDelegate:       async Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out User                   HTTPUser,
                                                                 out HashSet<Organization>  HTTPOrganizations,
                                                                 out HTTPResponse           Response,
                                                                 AccessLevel:               Access_Levels.ReadWrite,
                                                                 Recursive:                 true))
                                             {
                                                 return Response;
                                             }

                                             #endregion

                                             #region Check OrganizationId URI parameter

                                             if (!Request.ParseOrganizationId(this,
                                                                              out Organization_Id?  OrganizationIdURI,
                                                                              out HTTPResponse      HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion

                                             #region Parse JSON and create the new child organization...

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out HTTPResponse))
                                                 return HTTPResponse;

                                             if (Organization.TryParseJSON(JSONObj,
                                                                           out Organization  NewChildOrganization,
                                                                           out String        ErrorResponse,
                                                                           OrganizationIdURI))
                                             {

                                                 // {
                                                 //     "@id":                             newChildOrganizationId,
                                                 //     "@context":                        "https://opendata.social/contexts/UsersAPI+json/organization",
                                                 //     "parentOrganization":              organization["@id"],
                                                 //     "name":                            { "eng": newChildOrganizationName != "" ? newChildOrganizationName : newChildOrganizationId },
                                                 //     "admins": [{
                                                 //         "@id":       SignInUser,
                                                 //         "@context":  "https://opendata.social/contexts/UsersAPI+json/user",
                                                 //         "name":      Username,
                                                 //         "email":     UserEMail
                                                 //     }],
                                                 //     "members":                         [],
                                                 //     "youAreMember":                    true,
                                                 //     "youCanAddMembers":                true,
                                                 //     "youCanCreateChildOrganizations":  true,
                                                 //     "childs":                          []
                                                 // }

                                                 #region Parse parent organization

                                                 if (!JSONObj.ParseMandatory("parentOrganization",
                                                                             "parent organization",
                                                                             HTTPServer.DefaultHTTPServerName,
                                                                             Organization_Id.TryParse,
                                                                             out Organization_Id ParentOrganizationId,
                                                                             Request,
                                                                             out HTTPResponse))
                                                 {
                                                     return HTTPResponse;
                                                 }

                                                 if (!_Organizations.TryGetValue(ParentOrganizationId, out Organization ParentOrganization))
                                                 {

                                                     return new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                                Server                     = HTTPServer.DefaultServerName,
                                                                Date                       = DateTime.UtcNow,
                                                                AccessControlAllowOrigin   = "*",
                                                                AccessControlAllowMethods  = "GET, SET",
                                                                AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                ContentType                = HTTPContentType.JSON_UTF8,
                                                                Content                    = JSONObject.Create(
                                                                                                 new JProperty("description",  "Unknown parent organization!")
                                                                                             ).ToUTF8Bytes()
                                                            }.AsImmutable;

                                                 }

                                                 #endregion

                                                 #region Parse admins

                                                 if (!JSONObj.ParseMandatory("admins",
                                                                             "organization admins",
                                                                             HTTPServer.DefaultHTTPServerName,
                                                                             out JArray AdminsJSON,
                                                                             Request,
                                                                             out HTTPResponse))
                                                 {
                                                     return HTTPResponse;
                                                 }


                                                 User Admin      = null;
                                                 var Admins      = new List<User>();
                                                 var AdminNames  = AdminsJSON.Select(admin => (admin as JObject)).
                                                                              Where(admin => admin != null).
                                                                              Select(admin => User_Id.TryParse(admin["@id"].Value<String>())).
                                                                              ToArray();

                                                 foreach (var admin in AdminNames)
                                                 {

                                                     if (!admin.HasValue)
                                                     {

                                                         return new HTTPResponse.Builder(Request) {
                                                                    HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                                    Server                     = HTTPServer.DefaultServerName,
                                                                    Date                       = DateTime.UtcNow,
                                                                    AccessControlAllowOrigin   = "*",
                                                                    AccessControlAllowMethods  = "GET, SET",
                                                                    AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                                    Content                    = JSONObject.Create(
                                                                                                     new JProperty("description",  "Invalid admin user '" + admin.Value  + "'!")
                                                                                                 ).ToUTF8Bytes()
                                                                }.AsImmutable;

                                                     }

                                                     Admin = Get(admin.Value);

                                                     if (Admin == null)
                                                     {

                                                         return new HTTPResponse.Builder(Request) {
                                                                    HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                                    Server                     = HTTPServer.DefaultServerName,
                                                                    Date                       = DateTime.UtcNow,
                                                                    AccessControlAllowOrigin   = "*",
                                                                    AccessControlAllowMethods  = "GET, SET",
                                                                    AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                    ContentType                = HTTPContentType.JSON_UTF8,
                                                                    Content                    = JSONObject.Create(
                                                                                                     new JProperty("description",  "Unknown admin user '" + admin.Value + "'!")
                                                                                                 ).ToUTF8Bytes()
                                                                }.AsImmutable;

                                                     }

                                                     Admins.Add(Admin);

                                                 }

                                                 #endregion


                                                 try
                                                 {

                                                     var _NewChildOrganization = await CreateOrganization(NewChildOrganization.Id,
                                                                                                          NewChildOrganization.Name,
                                                                                                          NewChildOrganization.Description,
                                                                                                          ParentOrganization:  ParentOrganization,
                                                                                                          CurrentUserId:       HTTPUser.Id);

                                                     foreach (var admin in Admins)
                                                         await AddToOrganization(admin, User2OrganizationEdges.IsAdmin, _NewChildOrganization);

                                                 }
                                                 catch (Exception e)
                                                 {

                                                     return new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                                Server                     = HTTPServer.DefaultServerName,
                                                                Date                       = DateTime.UtcNow,
                                                                AccessControlAllowOrigin   = "*",
                                                                AccessControlAllowMethods  = "GET, SET",
                                                                AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                ContentType                = HTTPContentType.JSON_UTF8,
                                                                Content                    = JSONObject.Create(
                                                                                                 new JProperty("description",  "Could not create the given child organization! " + e.Message)
                                                                                             ).ToUTF8Bytes()
                                                            }.AsImmutable;

                                                 }

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode              = HTTPStatusCode.Created,
                                                            Server                      = HTTPServer.DefaultServerName,
                                                            Date                        = DateTime.UtcNow,
                                                            AccessControlAllowOrigin    = "*",
                                                            AccessControlAllowMethods   = "GET, SET",
                                                            AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                            Connection                  = "close"
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             return new HTTPResponse.Builder(Request) {
                                                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                        Server                     = HTTPServer.DefaultServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, SET",
                                                        AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                        ContentType                = HTTPContentType.JSON_UTF8,
                                                        Content                    = JSONObject.Create(
                                                                                         new JProperty("description",  "Could not parse the given child organization data!")
                                                                                     ).ToUTF8Bytes()
                                                    }.AsImmutable;

                                         });

            #endregion

            #region SET         ~/organizations/{organizationId}

            // ---------------------------------------------------------------------------------------------
            // curl -v -X SET \
            //      -H "Accept:       application/json; charset=utf-8" \
            //      -H "Content-Type: application/json; charset=utf-8" \
            //      -d "{ \
            //              \"@id\" :             \"214080158\", \
            //              \"@context\" :        \"https://cardi-link.cloud/contexts/cardidb+json/organization\", \
            //              \"description\" :     { \"deu\" : \"Test AED in Erlangen Raum Yavin 4\" },\
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
            //      http://127.0.0.1:2000/organizations/214080158
            // ---------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.SET,
                                         URLPathPrefix + "organizations/{organizationId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPRequestLogger:  SetOrganizationHTTPRequest,
                                         HTTPResponseLogger: SetOrganizationHTTPResponse,
                                         HTTPDelegate:       async Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out User                   HTTPUser,
                                                                 out HashSet<Organization>  HTTPOrganizations,
                                                                 out HTTPResponse           Response,
                                                                 AccessLevel:               Access_Levels.ReadWrite,
                                                                 Recursive:                 true))
                                             {
                                                 return Response;
                                             }

                                             #endregion

                                             #region Check OrganizationId URI parameter

                                             if (!Request.ParseOrganizationId(this,
                                                                              out Organization_Id?  OrganizationIdURI,
                                                                              out HTTPResponse      HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion

                                             #region Parse JSON and create the new child organization...

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out HTTPResponse))
                                                 return HTTPResponse;

                                             if (Organization.TryParseJSON(JSONObj,
                                                                           out Organization  UpdatedOrganization,
                                                                           out String        ErrorResponse,
                                                                           OrganizationIdURI))
                                             {

                                                 try
                                                 {

                                                     var _NewChildOrganization = await Update(UpdatedOrganization,
                                                                                              CurrentUserId:       HTTPUser.Id);

                                                 }
                                                 catch (Exception e)
                                                 {

                                                     return new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                                Server                     = HTTPServer.DefaultServerName,
                                                                Date                       = DateTime.UtcNow,
                                                                AccessControlAllowOrigin   = "*",
                                                                AccessControlAllowMethods  = "GET, SET",
                                                                AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                ContentType                = HTTPContentType.JSON_UTF8,
                                                                Content                    = JSONObject.Create(
                                                                                                 new JProperty("description",  "Could not create the given child organization! " + e.Message)
                                                                                             ).ToUTF8Bytes()
                                                            }.AsImmutable;

                                                 }

                                                 return new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode              = HTTPStatusCode.Created,
                                                            Server                      = HTTPServer.DefaultServerName,
                                                            Date                        = DateTime.UtcNow,
                                                            AccessControlAllowOrigin    = "*",
                                                            AccessControlAllowMethods   = "GET, SET",
                                                            AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                            Connection                  = "close"
                                                        }.AsImmutable;

                                             }

                                             #endregion

                                             return new HTTPResponse.Builder(Request) {
                                                        HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                        Server                     = HTTPServer.DefaultServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, SET",
                                                        AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                        ContentType                = HTTPContentType.JSON_UTF8,
                                                        Content                    = JSONObject.Create(
                                                                                         new JProperty("description",  "Could not parse the given child organization data!")
                                                                                     ).ToUTF8Bytes()
                                                    }.AsImmutable;

                                         });

            #endregion

            #region DELETE      ~/organizations/{organizationId}

            // ---------------------------------------------------------------------------------------------
            // curl -v -X DELETE \
            //      -H "Accept:       application/json; charset=utf-8" \
            //      -H "Content-Type: application/json; charset=utf-8" \
            //      http://127.0.0.1:2000/organizations/214080158
            // ---------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.DELETE,
                                         URLPathPrefix + "organizations/{organizationId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPRequestLogger:  DeleteOrganizationRequest,
                                         HTTPResponseLogger: DeleteOrganizationResponse,
                                         HTTPDelegate:       async Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out User                   HTTPUser,
                                                                 out HashSet<Organization>  HTTPOrganizations,
                                                                 out HTTPResponse           Response,
                                                                 AccessLevel:               Access_Levels.ReadWrite,
                                                                 Recursive:                 true))
                                             {
                                                 return Response;
                                             }

                                             #endregion

                                             #region Check Organization

                                             if (!Request.ParseOrganization(this,
                                                                            out Organization_Id?  OrganizationIdURI,
                                                                            out Organization      Organization,
                                                                            out HTTPResponse      HTTPResponse))
                                             {
                                                 return HTTPResponse;
                                             }

                                             #endregion

                                             //ToDo: Check admin!

                                             if (!HTTPOrganizations.Contains(Organization))
                                                 return new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                Server                     = HTTPServer.DefaultServerName,
                                                                Date                       = DateTime.UtcNow,
                                                                AccessControlAllowOrigin   = "*",
                                                                AccessControlAllowMethods  = "GET, SET, DELETE",
                                                                AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                ContentType                = HTTPContentType.JSON_UTF8,
                                                                Content                    = JSONObject.Create(
                                                                                                 new JProperty("description",  "Unknown parent organization!")
                                                                                             ).ToUTF8Bytes()
                                                            }.AsImmutable;



                                             try
                                             {

                                                 var _NewChildOrganization = await Remove(OrganizationIdURI.Value,
                                                                                          CurrentUserId:  HTTPUser.Id);

                                             }
                                             catch (Exception e)
                                             {

                                                 return new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                         Server                     = HTTPServer.DefaultServerName,
                                                         Date                       = DateTime.UtcNow,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = "GET, SET",
                                                         AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                         ContentType                = HTTPContentType.JSON_UTF8,
                                                         Content                    = JSONObject.Create(
                                                                                             new JProperty("description",  "Could not delete the given organization! " + e.Message)
                                                                                         ).ToUTF8Bytes()
                                                     }.AsImmutable;

                                             }

                                             return new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode              = HTTPStatusCode.OK,
                                                     Server                      = HTTPServer.DefaultServerName,
                                                     Date                        = DateTime.UtcNow,
                                                     AccessControlAllowOrigin    = "*",
                                                     AccessControlAllowMethods   = "GET, SET",
                                                     AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                     Connection                  = "close"
                                                 }.AsImmutable;

                                     });

            #endregion

            #endregion


            #region GET         ~/groups

            // ------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/groups
            // ------------------------------------------------------------------
            HTTPServer.ITEMS_GET(UriTemplate: URLPathPrefix + "groups",
                                 Dictionary: _Groups,
                                 Filter: group => group.PrivacyLevel == PrivacyLevel.World,
                                 ToJSONDelegate: JSON_IO.ToJSON);

            #endregion


            #region EXISTS      ~/groups/{GroupId}

            // -------------------------------------------------------------------------------------------
            // curl -v -X EXITS -H "Accept: application/json" http://127.0.0.1:2100/groups/OK-Lab%20Jena
            // -------------------------------------------------------------------------------------------
            HTTPServer.ITEM_EXISTS<Group_Id, Group>(UriTemplate: URLPathPrefix + "groups/{GroupId}",
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
            HTTPServer.ITEM_GET<Group_Id, Group>(UriTemplate:         URLPathPrefix + "groups/{GroupId}",
                                                 ParseIdDelegate:     Group_Id.TryParse,
                                                 ParseIdError:        Text => "Invalid group identification '" + Text + "'!",
                                                 TryGetItemDelegate:  _Groups.TryGetValue,
                                                 ItemFilterDelegate:  group => group.PrivacyLevel == PrivacyLevel.World,
                                                 TryGetItemError:     groupId => "Unknown group '" + groupId + "'!",
                                                 ToJSONDelegate:      _ => _.ToJSON());

            #endregion



            #region ~/tags

            #endregion


            #region GET         ~/blogPostings

            // --------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/blogPostings
            // --------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "blogPostings",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             #region Get HTTP user and its organizations

                                             TryGetHTTPUser(Request,
                                                            out User                   HTTPUser,
                                                            out HashSet<Organization>  HTTPOrganizations,
                                                            out HTTPResponse           Response,
                                                            Recursive: true);

                                             #endregion

                                             var skip                    = Request.QueryString.GetUInt64  ("skip");
                                             var take                    = Request.QueryString.GetUInt64  ("take");
                                             var since                   = Request.QueryString.GetDateTime("since");

                                             var includeCryptoHash       = Request.QueryString.GetBoolean ("includeCryptoHash", true);

                                             var expand                  = Request.QueryString.GetStrings ("expand", true);
                                             var expandTags              = expand.Contains("tags")         ? InfoStatus.Expand : InfoStatus.ShowIdOnly;


                                             return new HTTPResponse.Builder(Request) {
                                                        HTTPStatusCode                = HTTPStatusCode.OK,
                                                        Server                        = HTTPServer.DefaultServerName,
                                                        Date                          = DateTime.UtcNow,
                                                        AccessControlAllowOrigin      = "*",
                                                        AccessControlAllowMethods     = "GET",
                                                        AccessControlAllowHeaders     = "Content-Type, Accept, Authorization",
                                                        ContentType                   = HTTPContentType.JSON_UTF8,
                                                        Content                       = BlogPostings.
                                                                                            OrderBy(posting => posting.PublicationDate).
                                                                                            Where  (posting => !since.HasValue || posting.PublicationDate >= since.Value).
                                                                                            //Where  (organization => HTTPOrganizations.Contains(organization.Owner) ||
                                                                                            //                            Admins.InEdges(HTTPUser).
                                                                                            //                                   Any(edgelabel => edgelabel == User2GroupEdges.IsAdmin)).
                                                                                            ToJSON(skip,
                                                                                                   take,
                                                                                                   false, //Embedded
                                                                                                   expandTags,
                                                                                                   GetBlogPostingSerializator(Request, HTTPUser),
                                                                                                   includeCryptoHash).
                                                                                            ToUTF8Bytes(),
                                                        Connection                    = "close"
                                                    };

                                         });

            #endregion


            #region GET    .well-known/openpgpkey/policy

            #endregion

            #region HEAD   .well-known/openpgpkey/hu/{Id}

            #endregion

            #region GET    .well-known/openpgpkey/hu/{Id}

            // application/octet-string

            // EMailAddress.toLower().SHA1().ZBase32() == 32 octet string
            // Z-Base-32 method RFC 6189 section 5.1.6

            // https://www.ietf.org/id/draft-koch-openpgp-webkey-service-06.txt
            // The server MUST NOT return an ASCII armored version of the key.

            #endregion

        }

        #endregion

        #region (private) ReadDatabaseFiles()

        private async Task ReadDatabaseFiles()
        {

            DebugX.Log("Reading all UsersAPI database files...");

            #region Read DefaultUsersAPIFile

            try
            {

                JObject JSONLine;
                String  JSONCommand;
                JObject JSONObject;

                // Info: File.Exists(...) is harmful!
                File.ReadLines(this.UsersAPIPath + DefaultUsersAPIFile).ForEachCounted(async (line, linenumber) => {

                    if (line.IsNeitherNullNorEmpty() &&
                       !line.StartsWith("#")         &&
                       !line.StartsWith("//"))
                    {

                        try
                        {

                            JSONLine                  = JObject.Parse(line);
                            JSONCommand               = (JSONLine.First as JProperty)?.Name;
                            JSONObject                = (JSONLine.First as JProperty)?.Value as JObject;
                            CurrentDatabaseHashValue  =  JSONLine["sha256hash"]?["hashValue"]?.Value<String>();

                            if (JSONCommand.IsNotNullOrEmpty() && JSONObject != null)
                                await ProcessCommand(JSONCommand, JSONObject);

                        }
                        catch (Exception e)
                        {
                            DebugX.Log("Could not read database file '" + this.UsersAPIPath + DefaultUsersAPIFile + "' line " + linenumber + ": " + e.Message);
                        }

                    }

                });

            }
            catch (FileNotFoundException fe)
            { }
            catch (Exception e)
            {
                DebugX.LogT("Could not read database file '" + this.UsersAPIPath + DefaultUsersAPIFile + "': " + e.Message);
            }

            #endregion

            #region Read Password file...

            try
            {

                JObject JSONLine;
                String  JSONCommand;
                JObject JSONObject;

                // Info: File.Exists(...) is harmful!
                File.ReadLines(this.UsersAPIPath + DefaultPasswordFile).ForEachCounted((line, linenumber) => {

                    if (line.IsNeitherNullNorEmpty() &&
                       !line.StartsWith("#")         &&
                       !line.StartsWith("//"))
                    {

                        try
                        {

                            JSONLine                  = JObject.Parse(line);
                            JSONCommand               = (JSONLine.First as JProperty)?.Name;
                            JSONObject                = (JSONLine.First as JProperty)?.Value as JObject;
                            CurrentDatabaseHashValue  =  JSONLine["sha256hash"]?["hashValue"]?.Value<String>();

                            if (JSONCommand.IsNotNullOrEmpty() &&
                                JSONObject  != null            &&
                                User_Id.TryParse(JSONObject["login"].Value<String>(), out User_Id Login))
                            {

                                switch (JSONCommand)
                                {

                                    #region AddPassword

                                        case "addPassword":
                                        case "AddPassword":

                                            if (!_LoginPasswords.ContainsKey(Login))
                                            {

                                                _LoginPasswords.Add(Login,
                                                                    new LoginPassword(Login,
                                                                                      Password.ParseHash(JSONObject["newPassword"]["salt"].        Value<String>(),
                                                                                                         JSONObject["newPassword"]["passwordHash"].Value<String>())));

                                            }

                                            else
                                                DebugX.Log("Invalid 'AddPassword' command in '" + this.UsersAPIPath + DefaultPasswordFile + "' line " + linenumber + "!");

                                            break;

                                        #endregion

                                    #region ChangePassword

                                        case "changePassword":
                                        case "ChangePassword":

                                            if (_LoginPasswords.TryGetValue(Login, out LoginPassword _LoginPassword) &&
                                                _LoginPassword.Password.     UnsecureString   == JSONObject["currentPassword"]["passwordHash"].Value<String>() &&
                                                _LoginPassword.Password.Salt.UnsecureString() == JSONObject["currentPassword"]["salt"].        Value<String>())
                                            {

                                                _LoginPasswords[Login] = new LoginPassword(Login,
                                                                                           Password.ParseHash(JSONObject["newPassword"]["salt"].        Value<String>(),
                                                                                                              JSONObject["newPassword"]["passwordHash"].Value<String>()));

                                            }

                                            else
                                                DebugX.Log("Invalid 'ChangePassword' command in '" + this.UsersAPIPath + DefaultPasswordFile + "' line " + linenumber + "!");

                                            break;

                                        #endregion

                                    #region ResetPassword

                                        case "resetPassword":
                                        case "ResetPassword":

                                            if (_LoginPasswords.ContainsKey(Login))
                                                _LoginPasswords.Remove(Login);

                                            _LoginPasswords.Add(Login,
                                                                new LoginPassword(Login,
                                                                                  Password.ParseHash(JSONObject["newPassword"]["salt"].        Value<String>(),
                                                                                                     JSONObject["newPassword"]["passwordHash"].Value<String>())));

                                            break;

                                        #endregion

                                    default:
                                        DebugX.Log("Unknown command '" + JSONCommand + "' in password file '" + this.UsersAPIPath + DefaultPasswordFile + "' line " + linenumber + "!");
                                        break;

                                }

                            }

                            else
                                DebugX.Log("Could not read password file '" + this.UsersAPIPath + DefaultPasswordFile + "' line " + linenumber + "!");

                        }
                        catch (Exception e)
                        {
                            DebugX.Log("Could not read password file '" + this.UsersAPIPath + DefaultPasswordFile + "' line " + linenumber + ": " + e.Message);
                        }

                    }

                });

            }
            catch (FileNotFoundException fe)
            { }
            catch (Exception e)
            {
                DebugX.LogT("Could not read password file '" + this.UsersAPIPath + DefaultPasswordFile + "' failed: " + e.Message);
            }

            #endregion

            #region Read HTTPCookiesFile file...

            lock (HTTPCookies)
            {

                try
                {

                    File.ReadLines(this.UsersAPIPath + DefaultHTTPCookiesFile).ForEachCounted((line, linenumber) => {

                        try
                        {

                            var Tokens           = line.Split(new Char[] { ';' }, StringSplitOptions.None);

                            var SecurityTokenId  = SecurityToken_Id.Parse(Tokens[0]);
                            var Login            = User_Id.         Parse(Tokens[1]);
                            var Expires          = DateTime.        Parse(Tokens[2]);
                            var Astronaut        = Tokens.Length == 4
                                                       ? new User_Id?(User_Id.Parse(Tokens[3]))
                                                       : null;

                            if (!HTTPCookies.ContainsKey(SecurityTokenId) &&
                                _LoginPasswords.ContainsKey(Login) &&
                                Expires > DateTime.UtcNow)
                            {

                                HTTPCookies.Add(SecurityTokenId,
                                                new SecurityToken(Login,
                                                                  Expires,
                                                                  Astronaut));

                            }

                        }
                        catch (Exception e)
                        {
                            DebugX.Log("Could not read HTTP cookies file '" + this.UsersAPIPath + DefaultHTTPCookiesFile + "' line " + linenumber + ": " + e.Message);
                        }

                    });

                }
                catch (FileNotFoundException fe)
                { }
                catch (Exception e)
                {
                    DebugX.Log("Could not read HTTP cookies file '" + this.UsersAPIPath + DefaultHTTPCookiesFile + "': " + e.Message);
                }


                // Write filtered (no invalid users, no expired tokens) tokens back to file...
                try
                {

                    File.WriteAllLines(this.UsersAPIPath + DefaultHTTPCookiesFile,
                                       HTTPCookies.Select(token => token.Key + ";" + token.Value.ToLogLine()));

                }
                catch (Exception e)
                {
                    DebugX.Log("Could not update HTTP cookies file '" + this.UsersAPIPath + DefaultHTTPCookiesFile + "': " + e.Message);
                }

            }

            #endregion

            #region Read PasswordResets file...

            try
            {

                JObject  JSONLine;
                String   JSONCommand;
                JObject  JSONObject;

                var Now     = DateTime.UtcNow;
                var MaxAge  = TimeSpan.FromDays(7);

                // Info: File.Exists(...) is harmful!
                File.ReadLines(this.UsersAPIPath + DefaultPasswordResetsFile).ForEachCounted((line, linenumber) => {

                    if (line.IsNeitherNullNorEmpty() &&
                       !line.StartsWith("#")         &&
                       !line.StartsWith("//"))
                    {

                        try
                        {

                            JSONLine                  = JObject.Parse(line);
                            JSONCommand               = (JSONLine.First as JProperty)?.Name;
                            JSONObject                = (JSONLine.First as JProperty)?.Value as JObject;
                            //CurrentDatabaseHashValue  =  JSONLine["sha256hash"]?["hashValue"]?.Value<String>();

                            if (JSONCommand.IsNotNullOrEmpty() &&
                                JSONObject  != null &&
                                PasswordReset.TryParseJSON(JSONObject,
                                                           out PasswordReset _PasswordReset,
                                                           out String        ErrorResponse))
                            {

                                if (ErrorResponse == null)
                                {

                                    switch (JSONCommand.ToLower())
                                    {

                                        #region Add

                                            case "add":

                                                if (!PasswordResets.ContainsKey(_PasswordReset.SecurityToken1))
                                                {
                                                    if (Now - _PasswordReset.Timestamp <= MaxAge)
                                                    {
                                                        PasswordResets.Add(_PasswordReset.SecurityToken1,
                                                                           _PasswordReset);
                                                    }
                                                }

                                                else
                                                    DebugX.Log("Invalid 'Add' command in '" + this.UsersAPIPath + DefaultPasswordResetsFile + "' line " + linenumber + "!");

                                                break;

                                            #endregion

                                        #region Remove

                                            case "remove":
                                                PasswordResets.Remove(_PasswordReset.SecurityToken1);
                                                break;

                                            #endregion

                                        default:
                                            DebugX.Log("Unknown command '" + JSONCommand + "' in password file '" + this.UsersAPIPath + DefaultPasswordResetsFile + "' line " + linenumber + "!");
                                            break;

                                    }

                                }

                            }

                            else
                                DebugX.Log("Could not read password file '" + this.UsersAPIPath + DefaultPasswordResetsFile + "' line " + linenumber + "!");

                        }
                        catch (Exception e)
                        {
                            DebugX.Log("Could not read password file '" + this.UsersAPIPath + DefaultPasswordResetsFile + "' line " + linenumber + ": " + e.Message);
                        }

                    }

                });

            }
            catch (FileNotFoundException fe)
            { }
            catch (Exception e)
            {
                DebugX.LogT("Could not read password file '" + this.UsersAPIPath + DefaultPasswordResetsFile + "': " + e.Message);
            }

            #endregion

            DebugX.Log("Read all UsersAPI database files...");

        }

        #endregion

        //ToDo: Receive Network Database Commands

        #region (private) ProcessCommand(Command, Parameters)

        private async Task ProcessCommand(String   Command,
                                          JObject  JSONObject)
        {

            switch (Command)
            {

                #region CreateUser

                case "createUser":
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

                #region addUser

                case "addUser":

                    if (User.TryParseJSON(JSONObject,
                                          out _User,
                                          out ErrorResponse))
                    {
                        _Users.AddAndReturnValue(_User.Id, _User);
                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", ErrorResponse));

                    break;

                #endregion

                #region addIfNotExistsUser

                case "addIfNotExistsUser":
                case "AddIfNotExistsUser":

                    if (User.TryParseJSON(JSONObject,
                                          out _User,
                                          out ErrorResponse))
                    {

                        if (!_Users.ContainsKey(_User.Id))
                        {
                            _User.API = this;
                            _Users.AddAndReturnValue(_User.Id, _User);
                        }

                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", ErrorResponse));

                    break;

                #endregion

                #region addOrUpdateUser

                case "addOrUpdateUser":
                case "AddOrUpdateUser":

                    if (User.TryParseJSON(JSONObject,
                                          out _User,
                                          out ErrorResponse))
                    {

                        if (_Users.TryGetValue(_User.Id, out User OldUser))
                        {
                            _Users.Remove(OldUser.Id);
                            OldUser.CopyAllEdgesTo(_User);
                        }

                        _Users.Add(_User.Id, _User);

                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", ErrorResponse));

                    break;

                #endregion

                #region updateUser

                case "updateUser":
                case "UpdateUser":

                    if (User.TryParseJSON(JSONObject,
                                          out _User,
                                          out ErrorResponse))
                    {

                        if (_Users.TryGetValue(_User.Id, out User OldUser))
                        {

                            _Users.Remove(OldUser.Id);
                            _User.API = this;
                            OldUser.CopyAllEdgesTo(_User);

                            _Users.Add(_User.Id, _User);

                        }

                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", ErrorResponse));

                    break;

                #endregion

                #region removeUser

                case "removeUser":

                    if (User.TryParseJSON(JSONObject,
                                          out _User,
                                          out ErrorResponse))
                    {

                        if (TryGet(_User.Id, out User __User))
                        {

                            // this --edge--> organization
                            foreach (var edge in __User.User2Organization_OutEdges)
                                edge.Target.RemoveInEdge(edge);

                        }

                        _Users.Remove(_User.Id);

                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", ErrorResponse));

                    break;

                #endregion

                #region removeUserId

                case "removeUserId":

                    if (JSONObject.ParseOptional("@id",
                                                 "User identification to remove",
                                                 User_Id.TryParse,
                                                 out User_Id UserId,
                                                 out ErrorResponse))
                    {

                        if (TryGet(UserId, out User __User))
                        {

                            // this --edge--> organization
                            foreach (var edge in __User.User2Organization_OutEdges)
                                edge.Target.RemoveInEdge(edge);

                        }

                        _Users.Remove(UserId);

                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", ErrorResponse));

                    break;

                #endregion


                #region CreateOrganization

                case "createOrganization":
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

                #region addOrganization

                case "addOrganization":

                    if (Organization.TryParseJSON(JSONObject,
                                                  out _Organization,
                                                  out ErrorResponse))
                    {

                        if (!_Organizations.ContainsKey(_Organization.Id))
                        {
                            _Organization.API = this;
                            _Organizations.Add(_Organization.Id, _Organization);
                        }

                        else
                            DebugX.Log("Organization '" + _Organization.Id + "' already exists!");

                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", ErrorResponse));

                    break;

                #endregion

                #region addIfNotExistsOrganization

                case "addIfNotExistsOrganization":

                    if (Organization.TryParseJSON(JSONObject,
                                                  out _Organization,
                                                  out ErrorResponse))
                    {

                        if (!_Organizations.ContainsKey(_Organization.Id))
                        {
                            _Organization.API = this;
                            _Organizations.AddAndReturnValue(_Organization.Id, _Organization);
                        }

                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", ErrorResponse));

                    break;

                #endregion

                #region AddOrUpdateOrganization

                case "addOrUpdateOrganization":

                    if (Organization.TryParseJSON(JSONObject,
                                                  out _Organization,
                                                  out ErrorResponse))
                    {


                        if (_Organizations.TryGetValue(_Organization.Id, out Organization OldOrganization))
                        {
                            _Organizations.Remove(OldOrganization.Id);
                            _Organization.API = this;
                            OldOrganization.CopyAllEdgesTo(_Organization);
                        }

                        _Organizations.Add(_Organization.Id, _Organization);

                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", ErrorResponse));

                    break;

                #endregion

                #region UpdateOrganization

                case "updateOrganization":

                    if (Organization.TryParseJSON(JSONObject,
                                                  out _Organization,
                                                  out ErrorResponse))
                    {

                        if (_Organizations.TryGetValue(_Organization.Id, out Organization OldOrganization))
                        {

                            _Organizations.Remove(OldOrganization.Id);
                            _Organization.API = this;
                            OldOrganization.CopyAllEdgesTo(_Organization);

                            _Organizations.Add(_Organization.Id, _Organization);

                        }

                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", ErrorResponse));

                    break;

                #endregion

                #region RemoveOrganization

                case "removeOrganization":

                    if (Organization.TryParseJSON(JSONObject,
                                                  out _Organization,
                                                  out ErrorResponse))
                    {

                        if (TryGet(_Organization.Id, out Organization __Organization))
                        {

                            // this --edge--> other_organization
                            foreach (var edge in __Organization.Organization2OrganizationOutEdges)
                                edge.Target.RemoveInEdge(edge);

                            // this <--edge-- other_organization
                            foreach (var edge in __Organization.Organization2OrganizationInEdges)
                                edge.Source.RemoveOutEdge(edge);

                            // this <--edge-- user
                            foreach (var edge in __Organization.User2OrganizationEdges)
                                edge.Source.RemoveOutEdge(edge);

                        }

                        _Organizations.Remove(_Organization.Id);

                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", ErrorResponse));

                    break;

                #endregion

                #region RemoveOrganizationId

                case "removeOrganizationId":

                    if (JSONObject.ParseOptional("@id",
                                                 "Organization identification to remove",
                                                 Organization_Id.TryParse,
                                                 out Organization_Id OrganizationId,
                                                 out ErrorResponse))
                    {

                        if (TryGet(OrganizationId, out Organization __Organization))
                        {

                            // this --edge--> other_organization
                            foreach (var edge in __Organization.Organization2OrganizationOutEdges)
                                edge.Target.RemoveInEdge(edge);

                            // this <--edge-- other_organization
                            foreach (var edge in __Organization.Organization2OrganizationInEdges)
                                edge.Source.RemoveOutEdge(edge);

                            // this <--edge-- user
                            foreach (var edge in __Organization.User2OrganizationEdges)
                                edge.Source.RemoveOutEdge(edge);

                        }

                        _Organizations.Remove(OrganizationId);

                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", ErrorResponse));

                    break;

                #endregion


                #region AddUserToOrganization

                case "addUserToOrganization":
                case "AddUserToOrganization":

                    if (!User_Id.TryParse(JSONObject["user"]?.Value<String>(), out User_Id U2O_UserId))
                    {
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", "Invalid user identification '" + JSONObject["user"]?.Value<String>() + "'!"));
                        break;
                    }

                    if (!TryGet(U2O_UserId, out User U2O_User))
                    {
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", "Unknown user '" + U2O_UserId + "'!"));
                        break;
                    }


                    if (!Organization_Id.TryParse(JSONObject["organization"]?.Value<String>(), out Organization_Id U2O_OrganizationId))
                    {
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", "Invalid organization identification '" + JSONObject["user"]?.Value<String>() + "'!"));
                        break;
                    }

                    if (!TryGet(U2O_OrganizationId, out Organization U2O_Organization))
                    {
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", "Unknown organization '" + U2O_OrganizationId + "'!"));
                        break;
                    }


                    if (!Enum.TryParse(JSONObject["edge"].Value<String>(), out User2OrganizationEdges U2O_EdgeLabel))
                    {
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", "Unknown edge label '" + JSONObject["edge"].Value<String>() + "'!"));
                        break;
                    }


                    U2O_Organization.LinkUser(U2O_User.AddOutgoingEdge(U2O_EdgeLabel,
                                                                       U2O_Organization,
                                                                       JSONObject.ParseMandatory_PrivacyLevel()));

                    break;

                #endregion

                #region LinkOrganizations

                case "linkOrganizations":
                case "LinkOrganizations":

                    if (!Organization_Id.TryParse(JSONObject["organizationOut"]?.Value<String>(), out Organization_Id O2O_OrganizationIdOut))
                    {
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", "Invalid outgoing organization identification '" + JSONObject["user"]?.Value<String>() + "'!"));
                        break;
                    }

                    if (!TryGet(O2O_OrganizationIdOut, out Organization O2O_OrganizationOut))
                    {
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", "Unknown outgoing organization '" + O2O_OrganizationIdOut + "'!"));
                        break;
                    }


                    if (!Organization_Id.TryParse(JSONObject["organizationIn"]?.Value<String>(), out Organization_Id O2O_OrganizationIdIn))
                    {
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", "Invalid incoming organization identification '" + JSONObject["user"]?.Value<String>() + "'!"));
                        break;
                    }

                    if (!TryGet(O2O_OrganizationIdIn, out Organization O2O_OrganizationIn))
                    {
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", "Unknown incoming organization '" + O2O_OrganizationIdIn + "'!"));
                        break;
                    }


                    if (!Enum.TryParse(JSONObject["edge"].Value<String>(), out Organization2OrganizationEdges O2O_EdgeLabel))
                    {
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", "Unknown edge label '" + JSONObject["edge"].Value<String>() + "'!"));
                        break;
                    }


                    O2O_OrganizationIn.AddInEdge(O2O_OrganizationOut.AddOutEdge(O2O_EdgeLabel,
                                                                                O2O_OrganizationIn,
                                                                                JSONObject.ParseMandatory_PrivacyLevel()));

                    break;

                #endregion


                #region CreateGroup

                case "createGroup":
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


                #region AddUserToGroup

                case "addUserToGroup":
                case "AddUserToGroup":

                    if (!User_Id.TryParse(JSONObject["user"]?.Value<String>(), out User_Id U2G_UserId))
                    {
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", "Invalid user identification '" + JSONObject["user"]?.Value<String>() + "'!"));
                        break;
                    }

                    if (!TryGet(U2G_UserId, out User U2G_User))
                    {
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", "Unknown user '" + U2G_UserId + "'!"));
                        break;
                    }


                    if (!Group_Id.TryParse(JSONObject["group"]?.Value<String>(), out Group_Id U2G_GroupId))
                    {
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", "Invalid group identification '" + JSONObject["user"]?.Value<String>() + "'!"));
                        break;
                    }

                    if (!TryGet(U2G_GroupId, out Group U2G_Group))
                    {
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", "Unknown group '" + U2G_GroupId + "'!"));
                        break;
                    }


                    if (!Enum.TryParse(JSONObject["edge"].Value<String>(), out User2GroupEdges U2G_EdgeLabel))
                    {
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", "Unknown edge label '" + JSONObject["edge"].Value<String>() + "'!"));
                        break;
                    }


                    U2G_Group.AddIncomingEdge(U2G_User.AddOutgoingEdge(U2G_EdgeLabel,
                                                                       U2G_Group,
                                                                       JSONObject.ParseMandatory_PrivacyLevel()));

                    break;

                #endregion


                #region AddNotification

                case "addNotification":
                case "AddNotification":

                    if (JSONObject["userId"  ]?.Value<String>().IsNotNullOrEmpty() == true &&
                        JSONObject["@context"]?.Value<String>().IsNotNullOrEmpty() == true &&
                        User_Id.TryParse(JSONObject["userId"]?.Value<String>(), out UserId) &&
                        TryGet(UserId, out _User))
                    {

                        switch (JSONObject["@context"]?.Value<String>())
                        {

                            case TelegramNotification.JSONLDContext:

                                var telegramNotification = TelegramNotification.Parse(JSONObject);

                                if (telegramNotification != null)
                                    _User.AddNotification(telegramNotification);

                                else
                                    DebugX.Log(String.Concat(nameof(UsersAPI), " Could not parse the given Telegram notification!"));

                                break;


                            case EMailNotification.JSONLDContext:

                                var emailnotification = EMailNotification.Parse(JSONObject);

                                if (emailnotification != null)
                                    _User.AddNotification(emailnotification);

                                else
                                    DebugX.Log(String.Concat(nameof(UsersAPI), " Could not parse the given e-mail notification!"));

                                break;


                            case SMSNotification.JSONLDContext:

                                var smsnotification = SMSNotification.Parse(JSONObject);

                                if (smsnotification != null)
                                    _User.AddNotification(smsnotification);

                                else
                                    DebugX.Log(String.Concat(nameof(UsersAPI), " Could not parse the given SMS notification!"));

                                break;


                            case HTTPSNotification.JSONLDContext:

                                var httpsnotification = HTTPSNotification.Parse(JSONObject);

                                if (httpsnotification != null)
                                    _User.AddNotification(httpsnotification);

                                else
                                    DebugX.Log(String.Concat(nameof(UsersAPI), " Could not parse the given HTTPS notification!"));

                                break;


                        }

                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " Could not parse the given 'add notification' command!"));

                    break;

                #endregion

                #region RemoveNotification

                case "removeNotification":

                    if (JSONObject["userId"  ]?.Value<String>().IsNotNullOrEmpty() == true &&
                        JSONObject["@context"]?.Value<String>().IsNotNullOrEmpty() == true &&
                        User_Id.TryParse(JSONObject["userId"]?.Value<String>(), out UserId) &&
                        TryGet(UserId, out _User))
                    {

                        switch (JSONObject["@context"]?.Value<String>())
                        {

                            case TelegramNotification.JSONLDContext:

                                var telegramNotification = TelegramNotification.Parse(JSONObject);

                                if (telegramNotification != null)
                                    _User.RemoveNotification(telegramNotification);

                                else
                                    DebugX.Log(String.Concat(nameof(UsersAPI), " Could not parse the given Telegram notification!"));

                                break;


                            case EMailNotification.JSONLDContext:

                                var emailnotification = EMailNotification.Parse(JSONObject);

                                if (emailnotification != null)
                                    _User.RemoveNotification(emailnotification);

                                else
                                    DebugX.Log(String.Concat(nameof(UsersAPI), " Could not parse the given e-mail notification!"));

                                break;


                            case SMSNotification.JSONLDContext:

                                var smsnotification = SMSNotification.Parse(JSONObject);

                                if (smsnotification != null)
                                    _User.RemoveNotification(smsnotification);

                                else
                                    DebugX.Log(String.Concat(nameof(UsersAPI), " Could not parse the given SMS notification!"));

                                break;


                            case HTTPSNotification.JSONLDContext:

                                var httpsnotification = HTTPSNotification.Parse(JSONObject);

                                if (httpsnotification != null)
                                    _User.RemoveNotification(httpsnotification);

                                else
                                    DebugX.Log(String.Concat(nameof(UsersAPI), " Could not parse the given HTTPS notification!"));

                                break;

                        }

                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " Could not parse the given 'remove notification' command!"));

                    break;

                #endregion


                #region AddAPIKey

                case "addAPIKey":
                case "AddAPIKey":

                    if (APIKeyInfo.TryParseJSON(JSONObject,
                                                out APIKeyInfo _APIKey,
                                                _Users.TryGetValue,
                                                out ErrorResponse))
                    {
                        _APIKeys.AddAndReturnValue(_APIKey.APIKey, _APIKey);
                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", ErrorResponse));

                    break;

                #endregion


                default:
                    DebugX.Log(String.Concat(nameof(UsersAPI), " I don't know what to do with database command '", Command, "'!"));
                    break;

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


        async void ReceiveTelegramMessage(Object Sender, Telegram.Bot.Args.MessageEventArgs e)
        {

            var messageText = e?.Message?.Text;

            if (messageText.IsNeitherNullNorEmpty())
                messageText.Trim();

            if (messageText.IsNotNullOrEmpty())
            {

                var command = messageText.Split(' ');

                switch (command[0])
                {

                    case "/system":
                        await this.TelegramAPI.SendTextMessageAsync(
                            chatId:  e.Message.Chat,
                            text:    "I'm running on: " + Environment.MachineName + " and use " + (Environment.WorkingSet / 1024 /1024) + " MBytes RAM"
                        );
                        break;

                    case "/echo":
                        await this.TelegramAPI.SendTextMessageAsync(
                            chatId:  e.Message.Chat,
                            text:    "Hello " + e.Message.From.FirstName + " " + e.Message.From.LastName + "!\nYou said:\n" + e.Message.Text
                        );
                        break;

                }

            }
        }


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

        #region (protected) TryGetHTTPUser (Request, out User)

        protected Boolean TryGetHTTPUser(HTTPRequest Request, out User User)
        {

            #region Get user from cookie...

            if (Request.Cookies != null                                                                         &&
                Request. Cookies.TryGet     (CookieName,             out HTTPCookie        Cookie)              &&
                         Cookie. TryGet     (SecurityTokenCookieKey, out String            Value)               &&
                SecurityToken_Id.TryParse   (Value,                  out SecurityToken_Id  SecurityTokenId)     &&
                HTTPCookies.     TryGetValue(SecurityTokenId,        out SecurityToken     SecurityInformation) &&
                DateTime.UtcNow < SecurityInformation.Expires                                                   &&
                TryGet(SecurityInformation.UserId, out User))
            {
                return true;
            }

            #endregion

            #region Get user from Basic-Auth...

            if (Request.Authorization?.HTTPCredentialType == HTTPAuthenticationTypes.Basic &&
                User_Id.TryParse(Request.Authorization.Username, out User_Id UserId)       &&
                TryGet                 (UserId, out User)                              &&
                _LoginPasswords.TryGetValue(UserId, out LoginPassword Password)            &&
                Password.VerifyPassword(Request.Authorization.Password))
            {
                return true;
            }

            #endregion

            #region Get user from API Key...

            if (Request.API_Key.HasValue &&
                TryGetAPIKeyInfo(Request.API_Key.Value, out APIKeyInfo apiKeyInfo) &&
                (!apiKeyInfo.NotAfter.HasValue || DateTime.UtcNow < apiKeyInfo.NotAfter) &&
                 !apiKeyInfo.IsDisabled)
            {
                User = apiKeyInfo.User;
                return true;
            }

            #endregion

            User = null;
            return false;

        }

        #endregion

        #region (protected) TryGetAstronaut(Request, out User)

        protected Boolean TryGetAstronaut(HTTPRequest Request, out User User)
        {

            #region Get user from cookie...

            if (Request.Cookies != null                                                                         &&
                Request. Cookies.TryGet     (CookieName,             out HTTPCookie        Cookie)              &&
                         Cookie. TryGet     (SecurityTokenCookieKey, out String            Value)               &&
                SecurityToken_Id.TryParse   (Value,                  out SecurityToken_Id  SecurityTokenId)     &&
                HTTPCookies.     TryGetValue(SecurityTokenId,        out SecurityToken     SecurityInformation) &&
                DateTime.UtcNow < SecurityInformation.Expires                                                   &&
                TryGet(SecurityInformation.Astronaut ?? SecurityInformation.UserId, out User))
            {
                return true;
            }

            #endregion

            User = null;
            return false;

        }

        #endregion

        #region (protected) TryGetHTTPUser (Request, User, Organizations, Response, AccessLevel = ReadOnly, Recursive = false)

        protected Boolean TryGetHTTPUser(HTTPRequest                Request,
                                         out User                   User,
                                         out HashSet<Organization>  Organizations,
                                         out HTTPResponse           Response,
                                         Access_Levels              AccessLevel  = Access_Levels.ReadOnly,
                                         Boolean                    Recursive    = false)
        {

            if (!TryGetHTTPUser(Request, out User))
            {

                //if (Request.RemoteSocket.IPAddress.IsIPv4 &&
                //    Request.RemoteSocket.IPAddress.IsLocalhost)
                //{
                //    User           = Admins.User2GroupInEdges(edgelabel => edgelabel == User2GroupEdges.IsAdmin).FirstOrDefault()?.Source;
                //    Organizations  = User.Organizations(RequireReadWriteAccess, Recursive);
                //    Response       = null;
                //    return true;
                //}

                Organizations  = new HashSet<Organization>();

                Response       = new HTTPResponse.Builder(Request) {
                                     HTTPStatusCode  = HTTPStatusCode.Unauthorized,
                                     Location        = URLPathPrefix + "login",
                                     Date            = DateTime.Now,
                                     Server          = HTTPServer.DefaultServerName,
                                     CacheControl    = "private, max-age=0, no-cache",
                                     Connection      = "close"
                                 };

                return false;

            }

            Organizations  = User != null
                                 ? new HashSet<Organization>(User.Organizations(AccessLevel, Recursive))
                                 : new HashSet<Organization>();

            Response       = null;

            return true;

        }

        #endregion

        #region (protected) TryGetAstronaut(Request, User, Organizations, Response, AccessLevel = ReadOnly, Recursive = false)

        protected Boolean TryGetAstronaut(HTTPRequest                    Request,
                                          out User                       User,
                                          out IEnumerable<Organization>  Organizations,
                                          out HTTPResponse           Response,
                                          Access_Levels                  AccessLevel  = Access_Levels.ReadOnly,
                                          Boolean                        Recursive    = false)
        {

            if (!TryGetAstronaut(Request, out User))
            {

                //if (Request.RemoteSocket.IPAddress.IsIPv4 &&
                //    Request.RemoteSocket.IPAddress.IsLocalhost)
                //{
                //    User           = Admins.User2GroupInEdges(edgelabel => edgelabel == User2GroupEdges.IsAdmin).FirstOrDefault()?.Source;
                //    Organizations  = User.Organizations(RequireReadWriteAccess, Recursive);
                //    Response       = null;
                //    return true;
                //}

                Organizations  = new Organization[0];
                Response       = new HTTPResponse.Builder(Request) {
                                     HTTPStatusCode  = HTTPStatusCode.Unauthorized,
                                     Location        = URLPathPrefix + "login",
                                     Date            = DateTime.Now,
                                     Server          = HTTPServer.DefaultServerName,
                                     CacheControl    = "private, max-age=0, no-cache",
                                     Connection      = "close"
                                 };

                return false;

            }

            Organizations = User?.Organizations(AccessLevel, Recursive) ?? new Organization[0];
            Response      = null;
            return true;

        }

        #endregion

        #region (protected) TryGetHTTPUser (Request, User, Organizations,           AccessLevel = ReadOnly, Recursive = false)

        protected void TryGetHTTPUser(HTTPRequest                Request,
                                      out User                   User,
                                      out HashSet<Organization>  Organizations,
                                      Access_Levels              AccessLevel  = Access_Levels.ReadOnly,
                                      Boolean                    Recursive    = false)
        {

            if (!TryGetHTTPUser(Request, out User))
            {

                if (Request.HTTPSource.IPAddress.IsIPv4 &&
                    Request.HTTPSource.IPAddress.IsLocalhost)
                {
                    User           = Admins.User2GroupInEdges(edgelabel => edgelabel == User2GroupEdges.IsAdmin_ReadWrite).FirstOrDefault()?.Source;
                    Organizations  = new HashSet<Organization>(User.Organizations(AccessLevel, Recursive));
                    return;
                }

                Organizations  = null;
                return;

            }

            Organizations  = User != null
                                 ? new HashSet<Organization>(User.Organizations(AccessLevel, Recursive))
                                 : new HashSet<Organization>();

        }

        #endregion


        #region WriteToLogfile(MessageType, JSONData,                                    CurrentUserId = null)

        /// <summary>
        /// Write data to a log file.
        /// </summary>
        /// <param name="MessageType">The type of the message.</param>
        /// <param name="JSONData">The JSON data of the message.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public Task WriteToLogfile(NotificationMessageType  MessageType,
                                   JObject                  JSONData,
                                   User_Id?                 CurrentUserId  = null)

            => WriteToLogfile(MessageType,
                              JSONData,
                              this.UsersAPIPath + DefaultUsersAPIFile,
                              CurrentUserId);

        #endregion

        #region WriteToLogfile(MessageType, JSONData, Logfilename = DefaultUsersAPIFile, CurrentUserId = null)

        /// <summary>
        /// Write data to a log file.
        /// </summary>
        /// <param name="MessageType">The type of the message.</param>
        /// <param name="JSONData">The JSON data of the message.</param>
        /// <param name="Logfilename">An optional log file name.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task WriteToLogfile(NotificationMessageType  MessageType,
                                         JObject                  JSONData,
                                         String                   Logfilename    = DefaultUsersAPIFile,
                                         User_Id?                 CurrentUserId  = null)
        {

            if (!DisableLogfile || !DisableNotifications)
            {

                try
                {

                    var Now          = DateTime.UtcNow;

                    var JSONMessage  = new JObject(
                                           new JProperty(MessageType.ToString(),  JSONData),
                                           new JProperty("userId",                (CurrentUserId ?? CurrentAsyncLocalUserId.Value ?? Robot.Id).ToString()),
                                           new JProperty("systemId",              SystemId.ToString()),
                                           new JProperty("timestamp",             Now.ToIso8601()),
                                           new JProperty("sha256hash",            new JObject(
                                               new JProperty("nonce",                 Guid.NewGuid().ToString().Replace("-", "")),
                                               new JProperty("parentHash",            CurrentDatabaseHashValue)
                                           ))
                                       );

                    var SHA256                = new SHA256Managed();
                    CurrentDatabaseHashValue  = SHA256.ComputeHash(Encoding.Unicode.GetBytes(JSONMessage.ToString(Newtonsoft.Json.Formatting.None))).
                                                       Select(value => String.Format("{0:x2}", value)).
                                                       Aggregate();

                    (JSONMessage["sha256hash"] as JObject)?.Add(new JProperty("hashValue",  CurrentDatabaseHashValue));


                    #region Write to logfile

                    if (!DisableLogfile)
                    {

                        try
                        {

                            await LogFileSemaphore.WaitAsync();

                            var retry       = 0;
                            var maxRetries  = 23;

                            do
                            {

                                try
                                {

                                    File.AppendAllText(Logfilename,
                                                       JSONMessage.ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine);

                                    retry = maxRetries;

                                }
                                catch (IOException ioEx)
                                {
                                    DebugX.Log("Retry " + retry + ": Could not write message '" + MessageType + "' to logfile '" + Logfilename + "': " + ioEx.Message);
                                    await Task.Delay(10);
                                    retry++;
                                }
                                catch (Exception e)
                                {
                                    DebugX.Log("Retry " + retry + ": Could not write message '" + MessageType + "' to logfile '" + Logfilename + "': " + e.Message);
                                    await Task.Delay(10);
                                    retry++;
                                }

                            } while (retry < maxRetries);

                        }
                        catch (Exception e)
                        {
                            //ToDo: Handle WriteToLogfileAndNotify(...Write to logfile...) exceptions!
                        }
                        finally
                        {
                            LogFileSemaphore.Release();
                        }

                    }

                    #endregion

                }
                catch (Exception e)
                {
                    //ToDo: Handle WriteToLogfileAndNotify(...) exceptions!
                }

            }

        }

        #endregion


        #region WriteToCustomLogfile(Logfilename, Lock, Data)

        public async Task WriteToCustomLogfile(String         Logfilename,
                                               SemaphoreSlim  Lock,
                                               String         Data)
        {

            if (!DisableLogfile)
            {

                try
                {

                    await Lock.WaitAsync();

                    var retry       = 0;
                    var maxRetries  = 23;

                    do
                    {

                        try
                        {

                            File.AppendAllText(Logfilename,
                                               Data +
                                               Environment.NewLine);

                            retry = maxRetries;

                        }
                        catch (IOException ioEx)
                        {
                            DebugX.Log("Retry " + retry + ": Could not write custom logfile '" + Logfilename + "': " + ioEx.Message);
                            await Task.Delay(10);
                            retry++;
                        }
                        catch (Exception e)
                        {
                            DebugX.Log("Retry " + retry + ": Could not write custom logfile '" + Logfilename + "': " + e.Message);
                            await Task.Delay(10);
                            retry++;
                        }

                    } while (retry < maxRetries);

                }
                catch (Exception e)
                {
                    //ToDo: Handle WriteToCustomLogfile(...) exceptions!
                }
                finally
                {
                    Lock.Release();
                }

            }

        }

        #endregion


        #region AddEventSource(HTTPEventSourceId, URITemplate, IncludeFilterAtRuntime, CreateState, ...)

        public void AddEventSource<TData, TState>(HTTPEventSource_Id                             HTTPEventSourceId,
                                                  HTTPPath                                       URITemplate,

                                                  Func<TState, User, HTTPEvent<TData>, Boolean>  IncludeFilterAtRuntime,
                                                  Func<TState>                                   CreatePerRequestState,

                                                  HTTPHostname?                                  Hostname                   = null,
                                                  HTTPMethod?                                    HttpMethod                 = null,
                                                  HTTPContentType                                HTTPContentType            = null,

                                                  HTTPAuthentication                             URIAuthentication          = null,
                                                  HTTPAuthentication                             HTTPMethodAuthentication   = null,

                                                  HTTPDelegate                                   DefaultErrorHandler        = null)
        {

            if (IncludeFilterAtRuntime == null)
                IncludeFilterAtRuntime = (s, u, e) => true;

            if (TryGet(HTTPEventSourceId, out IHTTPEventSource<TData> _EventSource))
            {

                HTTPServer.AddMethodCallback(Hostname        ?? HTTPHostname.Any,
                                             HttpMethod      ?? HTTPMethod.GET,
                                             URITemplate,
                                             HTTPContentType ?? HTTPContentType.EVENTSTREAM,
                                             URIAuthentication:         URIAuthentication,
                                             HTTPMethodAuthentication:  HTTPMethodAuthentication,
                                             DefaultErrorHandler:       DefaultErrorHandler,
                                             HTTPDelegate:              Request => {

                                                 #region Get HTTP user and its organizations

                                                 // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                                 if (!TryGetHTTPUser(Request,
                                                                     out User                   HTTPUser,
                                                                     out HashSet<Organization>  HTTPOrganizations,
                                                                     out HTTPResponse           Response,
                                                                     AccessLevel:               Access_Levels.ReadWrite,
                                                                     Recursive:                 true))
                                                 {
                                                     return Task.FromResult(Response);
                                                 }

                                                 #endregion

                                                 var State        = CreatePerRequestState != null ? CreatePerRequestState() : default(TState);
                                                 var _HTTPEvents  = _EventSource.GetAllEventsGreater(Request.GetHeaderField_UInt64("Last-Event-ID")).
                                                                                 Where  (_event => IncludeFilterAtRuntime(State,
                                                                                                                          HTTPUser,
                                                                                                                          _event)).
                                                                                 Reverse().
                                                                                 Skip   (Request.QueryString.GetUInt64("skip")).
                                                                                 Take   (Request.QueryString.GetUInt64("take")).
                                                                                 Reverse().
                                                                                 Aggregate(new StringBuilder(),
                                                                                           (stringBuilder, httpEvent) => stringBuilder.Append(httpEvent.SerializedHeader).
                                                                                                                                       AppendLine(httpEvent.SerializedData).
                                                                                                                                       AppendLine()).
                                                                                 Append(Environment.NewLine).
                                                                                 Append("retry: ").Append((UInt32) _EventSource.RetryIntervall.TotalMilliseconds).
                                                                                 Append(Environment.NewLine).
                                                                                 Append(Environment.NewLine).
                                                                                 ToString();

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode  = HTTPStatusCode.OK,
                                                         Server          = HTTPServer.DefaultHTTPServerName,
                                                         ContentType     = HTTPContentType.EVENTSTREAM,
                                                         CacheControl    = "no-cache",
                                                         Connection      = "keep-alive",
                                                         KeepAlive       = new KeepAliveType(TimeSpan.FromSeconds(2 * _EventSource.RetryIntervall.TotalSeconds)),
                                                         Content         = _HTTPEvents.ToUTF8Bytes()
                                                     }.AsImmutable);

                                             });


                HTTPServer.AddMethodCallback(Hostname        ?? HTTPHostname.Any,
                                             HttpMethod      ?? HTTPMethod.GET,
                                             URITemplate,
                                             HTTPContentType ?? HTTPContentType.JSON_UTF8,
                                             URIAuthentication:         URIAuthentication,
                                             HTTPMethodAuthentication:  HTTPMethodAuthentication,
                                             DefaultErrorHandler:       DefaultErrorHandler,
                                             HTTPDelegate:              Request => {

                                                 #region Get HTTP user and its organizations

                                                 // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                                 if (!TryGetHTTPUser(Request,
                                                                     out User                   HTTPUser,
                                                                     out HashSet<Organization>  HTTPOrganizations,
                                                                     out HTTPResponse           Response,
                                                                     AccessLevel:               Access_Levels.ReadWrite,
                                                                     Recursive:                 true))
                                                 {
                                                     return Task.FromResult(Response);
                                                 }

                                                 #endregion

                                                 var State        = CreatePerRequestState != null ? CreatePerRequestState() : default(TState);
                                                 var _HTTPEvents  = _EventSource.Where(httpEvent => IncludeFilterAtRuntime(State,
                                                                                                                           HTTPUser,
                                                                                                                           httpEvent)).
                                                                                 Skip (Request.QueryString.GetUInt64("skip")).
                                                                                 Take (Request.QueryString.GetUInt64("take")).
                                                                                 Aggregate(new StringBuilder().AppendLine("["),
                                                                                           (stringBuilder, httpEvent) => stringBuilder.Append    (@"[""").
                                                                                                                                       Append    (httpEvent.Subevent ?? "").
                                                                                                                                       Append    (@""",").
                                                                                                                                       Append    (httpEvent.SerializedData).
                                                                                                                                       AppendLine("],")).
                                                                                 ToString().
                                                                                 TrimEnd();


                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode  = HTTPStatusCode.OK,
                                                         Server          = HTTPServer.DefaultHTTPServerName,
                                                         ContentType     = HTTPContentType.JSON_UTF8,
                                                         CacheControl    = "no-cache",
                                                         Connection      = "keep-alive",
                                                         KeepAlive       = new KeepAliveType(TimeSpan.FromSeconds(2 * _EventSource.RetryIntervall.TotalSeconds)),
                                                         Content         = (_HTTPEvents.Length > 1
                                                                                ? _HTTPEvents.Remove(_HTTPEvents.Length - 1, 1) + Environment.NewLine + "]"
                                                                                : "]").ToUTF8Bytes()
                                                     }.AsImmutable);

                                             });

            }

            else
                throw new ArgumentException("Event source '" + HTTPEventSourceId + "' could not be found!", nameof(HTTPEventSourceId));

        }

        #endregion

        #region AddEventSource(HTTPEventSourceId, URITemplate, IncludeFilterAtRuntime, CreateState, ...)

        public void AddEventSource<TData, TState>(HTTPEventSource_Id                                                        HTTPEventSourceId,
                                                  HTTPPath                                                                  URITemplate,

                                                  Func<TState, User, IEnumerable<Organization>, HTTPEvent<TData>, Boolean>  IncludeFilterAtRuntime,
                                                  Func<TState>                                                              CreatePerRequestState,

                                                  HTTPHostname?                                                             Hostname                   = null,
                                                  HTTPMethod?                                                               HttpMethod                 = null,
                                                  HTTPContentType                                                           HTTPContentType            = null,

                                                  HTTPAuthentication                                                        URIAuthentication          = null,
                                                  HTTPAuthentication                                                        HTTPMethodAuthentication   = null,

                                                  HTTPDelegate                                                              DefaultErrorHandler        = null)
        {

            if (IncludeFilterAtRuntime == null)
                IncludeFilterAtRuntime = (s, u, o, e) => true;

            if (TryGet<TData>(HTTPEventSourceId, out IHTTPEventSource<TData> _EventSource))
            {

                HTTPServer.AddMethodCallback(Hostname        ?? HTTPHostname.Any,
                                             HttpMethod      ?? HTTPMethod.GET,
                                             URITemplate,
                                             HTTPContentType ?? HTTPContentType.EVENTSTREAM,
                                             URIAuthentication:         URIAuthentication,
                                             HTTPMethodAuthentication:  HTTPMethodAuthentication,
                                             DefaultErrorHandler:       DefaultErrorHandler,
                                             HTTPDelegate:              Request => {

                                                 #region Get HTTP user and its organizations

                                                 // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                                 if (!TryGetHTTPUser(Request,
                                                                     out User                   HTTPUser,
                                                                     out HashSet<Organization>  HTTPOrganizations,
                                                                     out HTTPResponse           Response,
                                                                     AccessLevel:               Access_Levels.ReadWrite,
                                                                     Recursive:                 true))
                                                 {
                                                     return Task.FromResult(Response);
                                                 }

                                                 #endregion

                                                 var State        = CreatePerRequestState != null ? CreatePerRequestState() : default(TState);
                                                 var _HTTPEvents  = _EventSource.GetAllEventsGreater(Request.GetHeaderField_UInt64("Last-Event-ID")).
                                                                                 Where  (httpEvent => IncludeFilterAtRuntime(State,
                                                                                                                             HTTPUser,
                                                                                                                             HTTPOrganizations,
                                                                                                                             httpEvent)).
                                                                                 Reverse().
                                                                                 Skip   (Request.QueryString.GetUInt64("skip")).
                                                                                 Take   (Request.QueryString.GetUInt64("take")).
                                                                                 Reverse().
                                                                                 Aggregate(new StringBuilder(),
                                                                                           (stringBuilder, httpEvent) => stringBuilder.Append(httpEvent.SerializedHeader).
                                                                                                                                       AppendLine(httpEvent.SerializedData).
                                                                                                                                       AppendLine()).
                                                                                 Append(Environment.NewLine).
                                                                                 Append("retry: ").Append((UInt32) _EventSource.RetryIntervall.TotalMilliseconds).
                                                                                 Append(Environment.NewLine).
                                                                                 Append(Environment.NewLine).
                                                                                 ToString();

                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode  = HTTPStatusCode.OK,
                                                         Server          = HTTPServer.DefaultHTTPServerName,
                                                         ContentType     = HTTPContentType.EVENTSTREAM,
                                                         CacheControl    = "no-cache",
                                                         Connection      = "keep-alive",
                                                         KeepAlive       = new KeepAliveType(TimeSpan.FromSeconds(2 * _EventSource.RetryIntervall.TotalSeconds)),
                                                         Content         = _HTTPEvents.ToUTF8Bytes()
                                                     }.AsImmutable);

                                             });



                HTTPServer.AddMethodCallback(Hostname        ?? HTTPHostname.Any,
                                             HttpMethod      ?? HTTPMethod.GET,
                                             URITemplate,
                                             HTTPContentType ?? HTTPContentType.JSON_UTF8,
                                             URIAuthentication:         URIAuthentication,
                                             HTTPMethodAuthentication:  HTTPMethodAuthentication,
                                             DefaultErrorHandler:       DefaultErrorHandler,
                                             HTTPDelegate:              Request => {

                                                 #region Get HTTP user and its organizations

                                                 // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                                 if (!TryGetHTTPUser(Request,
                                                                     out User                   HTTPUser,
                                                                     out HashSet<Organization>  HTTPOrganizations,
                                                                     out HTTPResponse           Response,
                                                                     AccessLevel:               Access_Levels.ReadWrite,
                                                                     Recursive:                 true))
                                                 {
                                                     return Task.FromResult(Response);
                                                 }

                                                 #endregion

                                                 var State        = CreatePerRequestState != null ? CreatePerRequestState() : default(TState);
                                                 var _HTTPEvents  = _EventSource.Where(httpEvent => IncludeFilterAtRuntime(State,
                                                                                                                           HTTPUser,
                                                                                                                           HTTPOrganizations,
                                                                                                                           httpEvent)).
                                                                                 Skip (Request.QueryString.GetUInt64("skip")).
                                                                                 Take (Request.QueryString.GetUInt64("take")).
                                                                                 Aggregate(new StringBuilder().AppendLine("["),
                                                                                           (stringBuilder, httpEvent) => stringBuilder.Append(@"[""").
                                                                                                                                       Append(httpEvent.Subevent ?? "").
                                                                                                                                       Append(@""",").
                                                                                                                                       Append(httpEvent.SerializedData).
                                                                                                                                       AppendLine("],")).
                                                                                 ToString().
                                                                                 TrimEnd();


                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode  = HTTPStatusCode.OK,
                                                         Server          = HTTPServer.DefaultHTTPServerName,
                                                         ContentType     = HTTPContentType.JSON_UTF8,
                                                         CacheControl    = "no-cache",
                                                         Connection      = "keep-alive",
                                                         KeepAlive       = new KeepAliveType(TimeSpan.FromSeconds(2 * _EventSource.RetryIntervall.TotalSeconds)),
                                                         Content         = (_HTTPEvents.Length > 1
                                                                                ? _HTTPEvents.Remove(_HTTPEvents.Length - 1, 1) + Environment.NewLine + "]"
                                                                                : "]").ToUTF8Bytes()
                                                     }.AsImmutable);

                                             });

            }

            else
                throw new ArgumentException("Event source '" + HTTPEventSourceId + "' could not be found!", nameof(HTTPEventSourceId));

        }

        #endregion


        #region Users

        #region Data

        protected readonly Dictionary<User_Id, User> _Users;

        /// <summary>
        /// Return an enumeration of all users.
        /// </summary>
        public IEnumerable<User> Users
        {
            get
            {
                try
                {
                    UsersSemaphore.Wait();
                    return _Users.Values.ToArray();
                }
                finally
                {
                    UsersSemaphore.Release();
                }

            }
        }

        #endregion


        #region Add           (User,   OnAdded = null,                   CurrentUserId = null)

        /// <summary>
        /// Add the given user to the API.
        /// </summary>
        /// <param name="User">A new user to be added to this API.</param>
        /// <param name="OnAdded">A delegate run whenever the user had been added successfully.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<User> Add(User          User,
                                    Action<User>  OnAdded        = null,
                                    User_Id?      CurrentUserId  = null)
        {

            try
            {

                await UsersSemaphore.WaitAsync();

                if (User.API != null && User.API != this)
                    throw new ArgumentException(nameof(User), "The given user is already attached to another API!");

                if (_Users.ContainsKey(User.Id))
                    throw new Exception("User '" + User.Id + "' already exists in this API!");

                if (User.Id.Length < MinLoginLenght)
                    throw new Exception("User '" + User.Id + "' is too short!");

                User.API = this;

                await WriteToLogfile(NotificationMessageType.Parse("addUser"),
                                     User.ToJSON(),
                                     CurrentUserId);

                _Users.Add(User.Id, User);

                OnAdded?.Invoke(User);

                return User;

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region AddIfNotExists(User,   OnAdded = null,                   CurrentUserId = null)

        /// <summary>
        /// When it has not been created before, add the given user to the API.
        /// </summary>
        /// <param name="User">A new user to be added to this API.</param>
        /// <param name="OnAdded">A delegate run whenever the user had been added successfully.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<User> AddIfNotExists(User          User,
                                               Action<User>  OnAdded        = null,
                                               User_Id?      CurrentUserId  = null)
        {

            try
            {

                await UsersSemaphore.WaitAsync();

                if (User.API != null && User.API != this)
                    throw new ArgumentException(nameof(User), "The given user is already attached to another API!");

                if (_Users.ContainsKey(User.Id))
                    return _Users[User.Id];

                if (User.Id.Length < MinLoginLenght)
                    throw new Exception("User '" + User.Id + "' is too short!");

                User.API = this;

                await WriteToLogfile(NotificationMessageType.Parse("addIfNotExistsUser"),
                                     User.ToJSON(),
                                     CurrentUserId);

                _Users.Add(User.Id, User);

                OnAdded?.Invoke(User);

                return User;

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region AddOrUpdate   (User,   OnAdded = null, OnUpdated = null, CurrentUserId = null)

        /// <summary>
        /// Add or update the given user to/within the API.
        /// </summary>
        /// <param name="User">A user.</param>
        /// <param name="OnAdded">A delegate run whenever the user had been added successfully.</param>
        /// <param name="OnUpdated">A delegate run whenever the user had been updated successfully.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<User> AddOrUpdate(User          User,
                                            Action<User>  OnAdded        = null,
                                            Action<User>  OnUpdated      = null,
                                            User_Id?      CurrentUserId  = null)
        {

            try
            {

                await UsersSemaphore.WaitAsync();

                if (User.API != null && User.API != this)
                    throw new ArgumentException(nameof(User), "The given user is already attached to another API!");

                if (User.Id.Length < MinLoginLenght)
                    throw new Exception("User '" + User.Id + "' is too short!");

                await WriteToLogfile(NotificationMessageType.Parse("addOrUpdateUser"),
                                     User.ToJSON(),
                                     CurrentUserId);

                User.API = this;

                if (_Users.TryGetValue(User.Id, out User OldUser))
                {
                    _Users.Remove(OldUser.Id);
                    OldUser.CopyAllEdgesTo(User);
                }

                _Users.Add(User.Id, User);

                if (OldUser != null)
                    OnUpdated?.Invoke(User);
                else
                    OnAdded?.  Invoke(User);

                return User;

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region Update        (User,                                     CurrentUserId = null)

        /// <summary>
        /// Update the given user to/within the API.
        /// </summary>
        /// <param name="User">A user.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<User> Update(User      User,
                                       User_Id?  CurrentUserId  = null)
        {

            try
            {

                await UsersSemaphore.WaitAsync();

                if (User.API != null && User.API != this)
                    throw new ArgumentException(nameof(User), "The given user is already attached to another API!");

                if (!_Users.TryGetValue(User.Id, out User OldUser))
                    throw new Exception("User '" + User.Id + "' does not exists in this API!");

                if (User.Id.Length < MinLoginLenght)
                    throw new Exception("User '" + User.Id + "' is too short!");


                await WriteToLogfile(NotificationMessageType.Parse("updateUser"),
                                     User.ToJSON(),
                                     CurrentUserId);

                User.API = this;

                _Users.Remove(OldUser.Id);
                OldUser.CopyAllEdgesTo(User);

                return _Users.AddAndReturnValue(User.Id, User);

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region Update        (UserId, UpdateDelegate,                   CurrentUserId = null)

        /// <summary>
        /// Update the given user.
        /// </summary>
        /// <param name="UserId">An user identification.</param>
        /// <param name="UpdateDelegate">A delegate to update the given user.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<User> Update(User_Id               UserId,
                                       Action<User.Builder>  UpdateDelegate,
                                       User_Id?              CurrentUserId  = null)
        {

            try
            {

                if (UpdateDelegate == null)
                    throw new Exception("The given update delegate must not be null!");

                await UsersSemaphore.WaitAsync();

                if (!_Users.TryGetValue(UserId, out User OldUser))
                    throw new Exception("User '" + UserId + "' does not exists in this API!");

                var Builder = OldUser.ToBuilder();
                UpdateDelegate(Builder);
                var NewUser = Builder.ToImmutable;

                await WriteToLogfile(NotificationMessageType.Parse("updateUser"),
                                     NewUser.ToJSON(),
                                     CurrentUserId);

                NewUser.API = this;

                _Users.Remove(OldUser.Id);
                OldUser.CopyAllEdgesTo(NewUser);

                return _Users.AddAndReturnValue(NewUser.Id, NewUser);

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion


        #region CreateUser           (Id, EMail, Password, Name = null, Description = null, PublicKeyRing = null, SecretKeyRing = null, MobilePhone = null, IsPublic = true, IsDisabled = false, IsAuthenticated = false)

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="Id">The unique identification of the user.</param>
        /// <param name="EMail">The primary e-mail of the user.</param>
        /// <param name="Password">An optional password of the user.</param>
        /// <param name="Name">An offical (multi-language) name of the user.</param>
        /// <param name="Description">An optional (multi-language) description of the user.</param>
        /// <param name="PublicKeyRing">An optional PGP/GPG public keyring of the user.</param>
        /// <param name="SecretKeyRing">An optional PGP/GPG secret keyring of the user.</param>
        /// <param name="Telephone">An optional telephone number of the user.</param>
        /// <param name="MobilePhone">An optional mobile telephone number of the user.</param>
        /// <param name="Homepage">The homepage of the user.</param>
        /// <param name="GeoLocation">An optional geographical location of the user.</param>
        /// <param name="Address">An optional address of the user.</param>
        /// <param name="PrivacyLevel">Whether the user will be shown in user listings, or not.</param>
        /// <param name="AcceptedEULA">Timestamp when the user accepted the End-User-License-Agreement.</param>
        /// <param name="IsAuthenticated">The user will not be shown in user listings, as its primary e-mail address is not yet authenticated.</param>
        /// <param name="IsDisabled">The user will be shown in user listings.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<User> CreateUser(User_Id             Id,
                                           SimpleEMailAddress  EMail,
                                           Password?           Password          = null,
                                           String              Name              = null,
                                           I18NString          Description       = null,
                                           PgpPublicKeyRing    PublicKeyRing     = null,
                                           PgpSecretKeyRing    SecretKeyRing     = null,
                                           PhoneNumber?        Telephone         = null,
                                           PhoneNumber?        MobilePhone       = null,
                                           String              Homepage          = null,
                                           GeoCoordinate?      GeoLocation       = null,
                                           Address             Address           = null,
                                           PrivacyLevel        PrivacyLevel      = PrivacyLevel.Private,
                                           DateTime?           AcceptedEULA      = null,
                                           Boolean             IsAuthenticated   = false,
                                           Boolean             IsDisabled        = false,
                                           String              DataSource        = "",
                                           User_Id?            CurrentUserId     = null)

            => await Add(new User(Id,
                                  EMail,
                                  Name,
                                  Description,
                                  PublicKeyRing,
                                  SecretKeyRing,
                                  Telephone,
                                  MobilePhone,
                                  Homepage,
                                  GeoLocation,
                                  Address,
                                  PrivacyLevel,
                                  AcceptedEULA,
                                  IsAuthenticated,
                                  IsDisabled,
                                  DataSource),

                         user => {

                             if (Password.HasValue &&
                                 !_TryChangePassword(user.Id,
                                                     Password.Value,
                                                     null,
                                                     CurrentUserId).Result)
                             {
                                 throw new ApplicationException("The password for '" + user.Id + "' could not be changed, as the given current password does not match!");
                             }

                         },

                         CurrentUserId);

        #endregion

        #region CreateUserIfNotExists(Id, EMail, Password, Name = null, Description = null, PublicKeyRing = null, SecretKeyRing = null, MobilePhone = null, IsPublic = true, IsDisabled = false, IsAuthenticated = false)

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="Id">The unique identification of the user.</param>
        /// <param name="EMail">The primary e-mail of the user.</param>
        /// <param name="Password">An optional password of the user.</param>
        /// <param name="Name">An offical (multi-language) name of the user.</param>
        /// <param name="Description">An optional (multi-language) description of the user.</param>
        /// <param name="PublicKeyRing">An optional PGP/GPG public keyring of the user.</param>
        /// <param name="SecretKeyRing">An optional PGP/GPG secret keyring of the user.</param>
        /// <param name="Telephone">An optional telephone number of the user.</param>
        /// <param name="MobilePhone">An optional telephone number of the user.</param>
        /// <param name="Homepage">The homepage of the user.</param>
        /// <param name="GeoLocation">An optional geographical location of the user.</param>
        /// <param name="Address">An optional address of the user.</param>
        /// <param name="PrivacyLevel">Whether the user will be shown in user listings, or not.</param>
        /// <param name="AcceptedEULA">Timestamp when the user accepted the End-User-License-Agreement.</param>
        /// <param name="IsAuthenticated">The user will not be shown in user listings, as its primary e-mail address is not yet authenticated.</param>
        /// <param name="IsDisabled">The user will be shown in user listings.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<User> CreateUserIfNotExists(User_Id             Id,
                                                      SimpleEMailAddress  EMail,
                                                      Password?           Password          = null,
                                                      String              Name              = null,
                                                      I18NString          Description       = null,
                                                      PgpPublicKeyRing    PublicKeyRing     = null,
                                                      PgpSecretKeyRing    SecretKeyRing     = null,
                                                      PhoneNumber?        Telephone         = null,
                                                      PhoneNumber?        MobilePhone       = null,
                                                      String              Homepage          = null,
                                                      GeoCoordinate?      GeoLocation       = null,
                                                      Address             Address           = null,
                                                      PrivacyLevel        PrivacyLevel      = PrivacyLevel.Private,
                                                      DateTime?           AcceptedEULA      = null,
                                                      Boolean             IsAuthenticated   = false,
                                                      Boolean             IsDisabled        = false,
                                                      String              DataSource        = "",
                                                      User_Id?            CurrentUserId     = null)


            => await AddIfNotExists(new User(Id,
                                             EMail,
                                             Name,
                                             Description,
                                             PublicKeyRing,
                                             SecretKeyRing,
                                             Telephone,
                                             MobilePhone,
                                             Homepage,
                                             GeoLocation,
                                             Address,
                                             PrivacyLevel,
                                             AcceptedEULA,
                                             IsAuthenticated,
                                             IsDisabled,
                                             DataSource),

                                    async user => {

                                        if (Password.HasValue &&
                                            !_TryChangePassword(user.Id,
                                                                Password.Value,
                                                                null,
                                                                CurrentUserId).Result)
                                        {
                                            throw new ApplicationException("The password for '" + user.Id + "' could not be changed, as the given current password does not match!");
                                        }

                                        //if (Password.HasValue &&
                                        //    !await _TrySetToPassword(user.Id,
                                        //                             Password.Value,
                                        //                             CurrentUserId))
                                        //{
                                        //    throw new ApplicationException("The password for '" + user.Id + "' could not be set!");
                                        //}

                                    },

                                    CurrentUserId);

        #endregion

        #region ChangePassword   (UserId, NewPassword, CurrentPassword = null, CurrentUserId = null)

        public async Task ChangePassword(User_Id   UserId,
                                         Password  NewPassword,
                                         String    CurrentPassword  = null,
                                         User_Id?  CurrentUserId    = null)
        {

            try
            {

                if (UserId.Length < MinLoginLenght)
                    throw new Exception("UserId '" + UserId + "' is too short!");

                await UsersSemaphore.WaitAsync();

                if (!_TryChangePassword(UserId,
                                        NewPassword,
                                        CurrentPassword,
                                        CurrentUserId).Result)
                {
                    throw new ApplicationException("The password for '" + UserId + "' could not be changed, as the given current password does not match!");
                }

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region TryChangePassword(UserId, NewPassword, CurrentPassword = null, CurrentUserId = null)

        protected async Task<Boolean> _TryChangePassword(User_Id   UserId,
                                                         Password  NewPassword,
                                                         String    CurrentPassword  = null,
                                                         User_Id?  CurrentUserId    = null)
        {

            if (UserId.Length < MinLoginLenght)
                throw new Exception("UserId '" + UserId + "' is too short!");

            #region AddPassword

            if (!_LoginPasswords.TryGetValue(UserId, out LoginPassword _LoginPassword))
            {

                await WriteToLogfile(NotificationMessageType.Parse("addPassword"),
                                     new JObject(
                                         new JProperty("login",         UserId.ToString()),
                                         new JProperty("newPassword", new JObject(
                                             new JProperty("salt",          NewPassword.Salt.UnsecureString()),
                                             new JProperty("passwordHash",  NewPassword.UnsecureString)
                                         ))
                                     ),
                                     this.UsersAPIPath + DefaultPasswordFile,
                                     CurrentUserId);

                _LoginPasswords.Add(UserId, new LoginPassword(UserId, NewPassword));

                return true;

            }

            #endregion

            #region ChangePassword

            else if (CurrentPassword.IsNotNullOrEmpty() && _LoginPassword.VerifyPassword(CurrentPassword))
            {

                await WriteToLogfile(NotificationMessageType.Parse("changePassword"),
                                     new JObject(
                                         new JProperty("login",         UserId.ToString()),
                                         new JProperty("currentPassword", new JObject(
                                             new JProperty("salt",          _LoginPassword.Password.Salt.UnsecureString()),
                                             new JProperty("passwordHash",  _LoginPassword.Password.UnsecureString)
                                         )),
                                         new JProperty("newPassword",     new JObject(
                                             new JProperty("salt",          NewPassword.Salt.UnsecureString()),
                                             new JProperty("passwordHash",  NewPassword.UnsecureString)
                                         ))
                                     ),
                                     this.UsersAPIPath + DefaultPasswordFile,
                                     CurrentUserId);

                _LoginPasswords[UserId] = new LoginPassword(UserId, NewPassword);

                return true;

            }

            #endregion

            else
                return false;

        }

        public async Task<Boolean> TryChangePassword(User_Id   Login,
                                                     Password  NewPassword,
                                                     String    CurrentPassword  = null,
                                                     User_Id?  CurrentUserId    = null)
        {

            try
            {

                await UsersSemaphore.WaitAsync();

                return await _TryChangePassword(Login,
                                                NewPassword,
                                                CurrentPassword,
                                                CurrentUserId);

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region VerifyPassword   (UserId, Password)

        public Boolean VerifyPassword(User_Id  UserId,
                                      String   Password)
        {

            try
            {

                if (UserId.Length < MinLoginLenght)
                    throw new Exception("UserId '" + UserId + "' is too short!");

                UsersSemaphore.Wait();

                return _LoginPasswords.TryGetValue(UserId, out LoginPassword LoginPassword) &&
                        LoginPassword.VerifyPassword(Password);

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region Get              (UserId)

        /// <summary>
        /// Get the user having the given unique identification.
        /// </summary>
        /// <param name="UserId">The unique identification of the user.</param>
        public User Get(User_Id  UserId)
        {

            try
            {

                UsersSemaphore.Wait();

                if (_Users.TryGetValue(UserId, out User User))
                    return User;

                return null;

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region SearchUsersByName(Username)

        /// <summary>
        /// Find all users having the given user name.
        /// </summary>
        /// <param name="Username">The name of a user (might not be unique).</param>
        public IEnumerable<User> SearchUsersByName(String  Username)
        {

            try
            {

                UsersSemaphore.Wait();

                var FoundUsers = new List<User>();

                foreach (var user in _Users.Values)
                    if (user.Name == Username)
                        FoundUsers.Add(user);

                return FoundUsers;

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region SearchUsersByName(Username, out Users)

        /// <summary>
        /// Find all users having the given user name.
        /// </summary>
        /// <param name="Username">The name of a user (might not be unique).</param>
        /// <param name="Users">An enumeration of matching users.</param>
        public Boolean SearchUsersByName(String Username, out IEnumerable<User> Users)
        {

            try
            {

                UsersSemaphore.Wait();

                var FoundUsers = new List<User>();

                foreach (var user in _Users.Values)
                    if (user.Name == Username)
                        FoundUsers.Add(user);

                Users = FoundUsers;

                return FoundUsers.Count > 0;

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion


        #region UserExists       (UserId)

        /// <summary>
        /// Get the user having the given unique identification.
        /// </summary>
        /// <param name="UserId">The unique identification of the user.</param>
        public Boolean UserExists(User_Id  UserId)
        {

            try
            {

                UsersSemaphore.Wait();

                return _Users.ContainsKey(UserId);

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region TryGet           (UserId, out User)

        /// <summary>
        /// Try to get the user having the given unique identification.
        /// </summary>
        /// <param name="UserId">The unique identification of the user.</param>
        /// <param name="User">The user.</param>
        public Boolean TryGet(User_Id   UserId,
                              out User  User)
        {

            try
            {

                UsersSemaphore.Wait();

                return _Users.TryGetValue(UserId, out User);

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion


        public UserContext SetUserContext(User User)
            => new UserContext(User.Id);

        public UserContext SetUserContext(User_Id UserId)
            => new UserContext(UserId);

        #endregion

        #region Organizations

        #region Data

        protected readonly Dictionary<Organization_Id, Organization> _Organizations;

        /// <summary>
        /// Return an enumeration of all organizations.
        /// </summary>
        public IEnumerable<Organization> Organizations
        {
            get
            {
                try
                {
                    OrganizationsSemaphore.Wait();
                    return _Organizations.Values.ToArray();
                }
                finally
                {
                    OrganizationsSemaphore.Release();
                }

            }
        }

        #endregion


        #region Add           (Organization,   ParentOrganization = null, CurrentUserId = null)

        /// <summary>
        /// Add the given organization to the API.
        /// </summary>
        /// <param name="Organization">A new organization to be added to this API.</param>
        /// <param name="ParentOrganization">The parent organization of the organization organization to be added.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<Organization> Add(Organization  Organization,
                                            Organization  ParentOrganization   = null,
                                            User_Id?      CurrentUserId        = null)
        {

            try
            {

                await OrganizationsSemaphore.WaitAsync();

                if (Organization.API != null && Organization.API != this)
                    throw new ArgumentException(nameof(Organization), "The given organization is already attached to another API!");

                if (_Organizations.ContainsKey(Organization.Id))
                    throw new Exception("Organization '" + Organization.Id + "' already exists in this API!");

                if (ParentOrganization != null && !_Organizations.ContainsKey(ParentOrganization.Id))
                    throw new Exception("Parent organization '" + ParentOrganization.Id + "' does not exists in this API!");

                Organization.API = this;


                // Check Admin!
                if (CurrentUserId.HasValue)
                {
                    if (!Get(CurrentUserId.Value).
                             Organizations(Access_Levels.ReadWrite, true).
                             Contains(ParentOrganization))
                    {
                        throw new Exception("Not allowed!");
                    }
                }


                await WriteToLogfile(NotificationMessageType.Parse("addOrganization"),
                                     Organization.ToJSON(),
                                     CurrentUserId);

                var newOrganization = _Organizations.AddAndReturnValue(Organization.Id, Organization);

                if (ParentOrganization != null)
                    await _LinkOrganizations(newOrganization,
                                             Organization2OrganizationEdges.IsChildOf,
                                             ParentOrganization,
                                             CurrentUserId:  CurrentUserId);

                return newOrganization;

            }
            finally
            {
                OrganizationsSemaphore.Release();
            }

        }

        #endregion

        #region AddIfNotExists(Organization,   ParentOrganization = null, CurrentUserId = null)

        /// <summary>
        /// When it has not been created before, add the given organization to the API.
        /// </summary>
        /// <param name="Organization">A new organization to be added to this API.</param>
        /// <param name="ParentOrganization">The parent organization of the organization to be added.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<Organization> AddIfNotExists(Organization  Organization,
                                                       Organization  ParentOrganization   = null,
                                                       User_Id?      CurrentUserId        = null)
        {

            try
            {

                await OrganizationsSemaphore.WaitAsync();

                if (Organization.API != null && Organization.API != this)
                    throw new ArgumentException(nameof(Organization), "The given organization is already attached to another API!");

                if (_Organizations.ContainsKey(Organization.Id))
                    return _Organizations[Organization.Id];

                if (ParentOrganization != null && !_Organizations.ContainsKey(ParentOrganization.Id))
                    throw new Exception("Parent organization '" + ParentOrganization.Id + "' does not exists in this API!");

                Organization.API = this;

                await WriteToLogfile(NotificationMessageType.Parse("addIfNotExistsOrganization"),
                                     Organization.ToJSON(),
                                     CurrentUserId);

                var NewOrg = _Organizations.AddAndReturnValue(Organization.Id, Organization);

                if (ParentOrganization != null)
                    await _LinkOrganizations(NewOrg, Organization2OrganizationEdges.IsChildOf, ParentOrganization, CurrentUserId: CurrentUserId);

                return NewOrg;

            }
            finally
            {
                OrganizationsSemaphore.Release();
            }

        }

        #endregion

        #region AddOrUpdate   (Organization,   ParentOrganization = null, CurrentUserId = null)

        /// <summary>
        /// Add or update the given organization to/within the API.
        /// </summary>
        /// <param name="Organization">A organization.</param>
        /// <param name="ParentOrganization">The parent organization of the organization to be added.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<Organization> AddOrUpdate(Organization  Organization,
                                                    Organization  ParentOrganization   = null,
                                                    User_Id?      CurrentUserId        = null)
        {

            try
            {

                await OrganizationsSemaphore.WaitAsync();

                if (Organization.API != null && Organization.API != this)
                    throw new ArgumentException(nameof(Organization), "The given organization is already attached to another API!");

                if (ParentOrganization != null && !_Organizations.ContainsKey(ParentOrganization.Id))
                    throw new Exception("Parent organization '" + ParentOrganization.Id + "' does not exists in this API!");

                if (_Organizations.TryGetValue(Organization.Id, out Organization OldOrganization))
                {
                    _Organizations.Remove(OldOrganization.Id);
                }

                Organization.API = this;

                await WriteToLogfile(NotificationMessageType.Parse("addOrUpdateOrganization"),
                                     Organization.ToJSON(),
                                     CurrentUserId);

                var NewOrg = _Organizations.AddAndReturnValue(Organization.Id, Organization);

                // ToDo: Copy edges!

                if (ParentOrganization != null)
                {
                    await _LinkOrganizations(NewOrg, Organization2OrganizationEdges.IsChildOf, ParentOrganization, CurrentUserId: CurrentUserId);
                    //ToDo: Update link to parent organization
                }

                return NewOrg;

            }
            finally
            {
                OrganizationsSemaphore.Release();
            }

        }

        #endregion

        #region Update        (Organization,                              CurrentUserId = null)

        /// <summary>
        /// Update the given organization within the API.
        /// </summary>
        /// <param name="Organization">A organization.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<Organization> Update(Organization  Organization,
                                               User_Id?      CurrentUserId  = null)
        {

            try
            {

                await OrganizationsSemaphore.WaitAsync();

                if (Organization.API != null && Organization.API != this)
                    throw new ArgumentException(nameof(Organization), "The given organization is already attached to another API!");

                if (!_Organizations.TryGetValue(Organization.Id, out Organization OldOrganization))
                    throw new Exception("Organization '" + Organization.Id + "' does not exists in this API!");

                else
                {

                    _Organizations.Remove(OldOrganization.Id);

                }

                Organization.API = this;

                await WriteToLogfile(NotificationMessageType.Parse("updateOrganization"),
                                     Organization.ToJSON(),
                                     CurrentUserId);

                OldOrganization.CopyAllEdgesTo(Organization);

                return _Organizations.AddAndReturnValue(Organization.Id, Organization);

            }
            finally
            {
                OrganizationsSemaphore.Release();
            }

        }

        #endregion

        #region Update        (OrganizationId, UpdateDelegate,            CurrentUserId = null)

        /// <summary>
        /// Update the given organization.
        /// </summary>
        /// <param name="OrganizationId">An organization identification.</param>
        /// <param name="UpdateDelegate">A delegate to update the given organization.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<Organization> Update(Organization_Id               OrganizationId,
                                               Action<Organization.Builder>  UpdateDelegate,
                                               User_Id?                      CurrentUserId  = null)
        {

            try
            {

                if (UpdateDelegate == null)
                    throw new Exception("The given update delegate must not be null!");

                await OrganizationsSemaphore.WaitAsync();

                if (!_Organizations.TryGetValue(OrganizationId, out Organization OldOrganization))
                    throw new Exception("Organization '" + OrganizationId + "' does not exists in this API!");

                var Builder = OldOrganization.ToBuilder();
                UpdateDelegate(Builder);
                var NewOrganization = Builder.ToImmutable;

                await WriteToLogfile(NotificationMessageType.Parse("updateOrganization"),
                                     NewOrganization.ToJSON(),
                                     CurrentUserId);

                NewOrganization.API = this;

                _Organizations.Remove(OldOrganization.Id);
                OldOrganization.CopyAllEdgesTo(NewOrganization);

                return _Organizations.AddAndReturnValue(NewOrganization.Id, NewOrganization);

            }
            finally
            {
                OrganizationsSemaphore.Release();
            }

        }

        #endregion

        #region Remove        (OrganizationId,                            CurrentUserId = null)

        /// <summary>
        /// Remove the given organization from this API.
        /// </summary>
        /// <param name="OrganizationId">The unique identification of the organization.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<Organization> Remove(Organization_Id  OrganizationId,
                                               User_Id?         CurrentUserId  = null)
        {

            try
            {

                await OrganizationsSemaphore.WaitAsync();

                if (_Organizations.TryGetValue(OrganizationId, out Organization Organization))
                {

                    // this --edge--> other_organization
                    foreach (var edge in Organization.Organization2OrganizationOutEdges)
                        edge.Target.RemoveInEdge(edge);

                    // this <--edge-- other_organization
                    foreach (var edge in Organization.Organization2OrganizationInEdges)
                        edge.Source.RemoveOutEdge(edge);

                    // this <--edge-- user
                    foreach (var edge in Organization.User2OrganizationEdges)
                        edge.Source.RemoveOutEdge(edge);


                    await WriteToLogfile(NotificationMessageType.Parse("removeOrganization"),
                                         Organization.ToJSON(),
                                         CurrentUserId);

                    _Organizations.Remove(OrganizationId);

                    //Organization.API = null;

                    return Organization;

                }

                return null;

            }
            finally
            {
                OrganizationsSemaphore.Release();
            }

        }

        #endregion


        #region CreateOrganization           (Id, Name = null, Description = null, ParentOrganization = null)

        public Task<Organization> CreateOrganization(Organization_Id           Id,
                                                     I18NString                Name                 = null,
                                                     I18NString                Description          = null,
                                                     String                    Website              = null,
                                                     EMailAddress              EMail                = null,
                                                     PhoneNumber?              Telephone            = null,
                                                     Address                   Address              = null,
                                                     GeoCoordinate?            GeoLocation          = null,
                                                     Func<Tags.Builder, Tags>  Tags                 = null,
                                                     PrivacyLevel              PrivacyLevel         = PrivacyLevel.Private,
                                                     Boolean                   IsDisabled           = false,
                                                     String                    DataSource           = "",
                                                     Organization              ParentOrganization   = null,
                                                     User_Id?                  CurrentUserId        = null)

            => Add(new Organization(Id,
                                    Name,
                                    Description,
                                    Website,
                                    EMail,
                                    Telephone,
                                    Address,
                                    GeoLocation,
                                    Tags,
                                    PrivacyLevel,
                                    IsDisabled,
                                    DataSource),
                   ParentOrganization,
                   CurrentUserId);

        #endregion

        #region CreateOrganizationIfNotExists(Id, Name = null, Description = null, ParentOrganization = null)

        public Task<Organization> CreateOrganizationIfNotExists(Organization_Id           Id,
                                                                I18NString                Name                 = null,
                                                                I18NString                Description          = null,
                                                                String                    Website              = null,
                                                                EMailAddress              EMail                = null,
                                                                PhoneNumber?              Telephone            = null,
                                                                Address                   Address              = null,
                                                                GeoCoordinate?            GeoLocation          = null,
                                                                Func<Tags.Builder, Tags>  Tags                 = null,
                                                                PrivacyLevel              PrivacyLevel         = PrivacyLevel.Private,
                                                                Boolean                   IsDisabled           = false,
                                                                String                    DataSource           = "",
                                                                Organization              ParentOrganization   = null,
                                                                User_Id?                  CurrentUserId        = null)

            => AddIfNotExists(new Organization(Id,
                                               Name,
                                               Description,
                                               Website,
                                               EMail,
                                               Telephone,
                                               Address,
                                               GeoLocation,
                                               Tags,
                                               PrivacyLevel,
                                               IsDisabled,
                                               DataSource),
                              ParentOrganization,
                              CurrentUserId);

        #endregion


        #region (protected internal) SetOrganizationRequest (Request)

        /// <summary>
        /// An event sent whenever set organization (data) request was received.
        /// </summary>
        public event RequestLogHandler OnSetOrganizationRequest;

        protected internal HTTPRequest SetOrganizationRequest(HTTPRequest Request)
        {

            OnSetOrganizationRequest?.Invoke(Request.Timestamp,
                                              HTTPServer,
                                              Request);

            return Request;

        }

        #endregion

        #region (protected internal) SetOrganizationResponse(Response)

        /// <summary>
        /// An event sent whenever a response on a set organization (data) request was sent.
        /// </summary>
        public event AccessLogHandler OnSetOrganizationResponse;

        protected internal HTTPResponse SetOrganizationResponse(HTTPResponse Response)
        {

            OnSetOrganizationResponse?.Invoke(Response.Timestamp,
                                               HTTPServer,
                                               Response.HTTPRequest,
                                               Response);

            return Response;

        }

        #endregion


        #region Contains      (OrganizationId)

        /// <summary>
        /// Whether this API contains a organization having the given unique identification.
        /// </summary>
        /// <param name="OrganizationId">The unique identification of the organization.</param>
        public Boolean Contains(Organization_Id OrganizationId)
        {

            try
            {

                OrganizationsSemaphore.Wait();

                return _Organizations.ContainsKey(OrganizationId);

            }
            finally
            {
                OrganizationsSemaphore.Release();
            }

        }

        #endregion

        #region Get           (OrganizationId)

        /// <summary>
        /// Get the organization having the given unique identification.
        /// </summary>
        /// <param name="OrganizationId">The unique identification of the organization.</param>
        public async Task<Organization> Get(Organization_Id  OrganizationId)
        {

            try
            {

                await OrganizationsSemaphore.WaitAsync();

                if (_Organizations.TryGetValue(OrganizationId, out Organization Organization))
                    return Organization;

                return null;

            }
            finally
            {
                OrganizationsSemaphore.Release();
            }

        }

        #endregion

        #region TryGet        (OrganizationId, out Organization)

        /// <summary>
        /// Try to get the organization having the given unique identification.
        /// </summary>
        /// <param name="OrganizationId">The unique identification of the organization.</param>
        /// <param name="Organization">The organization.</param>
        public Boolean TryGet(Organization_Id   OrganizationId,
                              out Organization  Organization)
        {

            try
            {

                OrganizationsSemaphore.Wait();

                return _Organizations.TryGetValue(OrganizationId, out Organization);

            }
            finally
            {
                OrganizationsSemaphore.Release();
            }

        }

        #endregion

        #region SearchOrganizationsByName(OrganizationName)

        /// <summary>
        /// Find all organizations having the given name.
        /// </summary>
        /// <param name="OrganizationName">The name of an organization (might not be unique).</param>
        public IEnumerable<Organization> SearchOrganizationsByName(String OrganizationName)
        {

            try
            {

                OrganizationsSemaphore.Wait();

                var FoundOrganizations = new List<Organization>();

                foreach (var organization in _Organizations.Values)
                    if (organization.Name.Any(i18npair => i18npair.Text == OrganizationName))
                        FoundOrganizations.Add(organization);

                return FoundOrganizations;

            }
            finally
            {
                OrganizationsSemaphore.Release();
            }

        }

        /// <summary>
        /// Find all organizations having the given name.
        /// </summary>
        /// <param name="OrganizationName">The name of an organization (might not be unique).</param>
        public IEnumerable<Organization> SearchOrganizationsByName(I18NString OrganizationName)
        {

            try
            {

                OrganizationsSemaphore.Wait();

                var FoundOrganizations = new List<Organization>();

                foreach (var organization in _Organizations.Values)
                    if (organization.Name == OrganizationName)
                        FoundOrganizations.Add(organization);

                return FoundOrganizations;

            }
            finally
            {
                OrganizationsSemaphore.Release();
            }

        }

        #endregion

        #region SearchOrganizationsByName(OrganizationName, out Organizations)

        /// <summary>
        /// Find all organizations having the given name.
        /// </summary>
        /// <param name="OrganizationName">The name of an organization (might not be unique).</param>
        /// <param name="Organizations">An enumeration of matching organizations.</param>
        public Boolean SearchOrganizationsByName(String OrganizationName, out IEnumerable<Organization> Organizations)
        {

            try
            {

                OrganizationsSemaphore.Wait();

                var FoundOrganizations = new List<Organization>();

                var sss = _Organizations.Where(o => o.Value.Name?.FirstText()?.StartsWith("P") == true);

                foreach (var organization in _Organizations.Values)
                    if (organization.Name.Any(i18npair => i18npair.Text == OrganizationName))
                        FoundOrganizations.Add(organization);

                Organizations = FoundOrganizations;

                return FoundOrganizations.Count > 0;

            }
            finally
            {
                OrganizationsSemaphore.Release();
            }

        }

        /// <summary>
        /// Find all organizations having the given name.
        /// </summary>
        /// <param name="OrganizationName">The name of an organization (might not be unique).</param>
        /// <param name="Organizations">An enumeration of matching organizations.</param>
        public Boolean SearchOrganizationsByName(I18NString OrganizationName, out IEnumerable<Organization> Organizations)
        {

            try
            {

                OrganizationsSemaphore.Wait();

                var FoundOrganizations = new List<Organization>();

                foreach (var organization in _Organizations.Values)
                    if (organization.Name == OrganizationName)
                        FoundOrganizations.Add(organization);

                Organizations = FoundOrganizations;

                return FoundOrganizations.Count > 0;

            }
            finally
            {
                OrganizationsSemaphore.Release();
            }

        }

        #endregion


        #region AddToOrganization(User, Edge, Organization, PrivacyLevel = Private)

        protected async Task<Boolean> _AddToOrganization(User                    User,
                                                         User2OrganizationEdges  Edge,
                                                         Organization            Organization,
                                                         PrivacyLevel            PrivacyLevel   = PrivacyLevel.Private,
                                                         User_Id?                CurrentUserId  = null)
        {

            if (!User.Edges(Organization).Any(edge => edge == Edge))
            {

                var edge = User.AddOutgoingEdge(Edge, Organization, PrivacyLevel);

                if (!Organization.User2OrganizationInEdgeLabels(User).Any(edgelabel => edgelabel == Edge))
                    Organization.LinkUser(edge);// User, Edge, PrivacyLevel);

                await WriteToLogfile(NotificationMessageType.Parse("addUserToOrganization"),
                                     new JObject(
                                         new JProperty("user",          User.        Id.ToString()),
                                         new JProperty("edge",          Edge.           ToString()),
                                         new JProperty("organization",  Organization.Id.ToString()),
                                         PrivacyLevel.ToJSON()
                                     ),
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        public async Task<Boolean> AddToOrganization(User                    User,
                                                     User2OrganizationEdges  Edge,
                                                     Organization            Organization,
                                                     PrivacyLevel            PrivacyLevel   = PrivacyLevel.Private,
                                                     User_Id?                CurrentUserId  = null)
        {

            try
            {

                await UsersSemaphore.        WaitAsync();
                await OrganizationsSemaphore.WaitAsync();

                return await _AddToOrganization(User,
                                                Edge,
                                                Organization,
                                                PrivacyLevel,
                                                CurrentUserId);

            }
            finally
            {
                OrganizationsSemaphore.Release();
                UsersSemaphore.        Release();
            }

        }

        #endregion

        #region LinkOrganizations  (OrganizationOut, EdgeLabel, OrganizationIn, Privacy = Public, CurrentUserId = null)

        protected async Task<Boolean> _LinkOrganizations(Organization                    OrganizationOut,
                                                         Organization2OrganizationEdges  EdgeLabel,
                                                         Organization                    OrganizationIn,
                                                         PrivacyLevel                    Privacy        = PrivacyLevel.World,
                                                         User_Id?                        CurrentUserId  = null)
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
                        OrganizationIn.AddInEdge(EdgeLabel, OrganizationOut, Privacy);
                    }

                    await WriteToLogfile(NotificationMessageType.Parse("linkOrganizations"),
                                         new JObject(
                                             new JProperty("organizationOut", OrganizationOut.Id.ToString()),
                                             new JProperty("edge",            EdgeLabel.         ToString()),
                                             new JProperty("organizationIn",  OrganizationIn. Id.ToString()),
                                             Privacy.ToJSON()
                                         ),
                                         CurrentUserId);

                    return true;

                }

                return false;

        }

        public async Task<Boolean> LinkOrganizations(Organization                    OrganizationOut,
                                                     Organization2OrganizationEdges  EdgeLabel,
                                                     Organization                    OrganizationIn,
                                                     PrivacyLevel                    Privacy        = PrivacyLevel.World,
                                                     User_Id?                        CurrentUserId  = null)
        {

            try
            {

                await OrganizationsSemaphore.WaitAsync();

                return await _LinkOrganizations(OrganizationOut,
                                                EdgeLabel,
                                                OrganizationIn,
                                                Privacy,
                                                CurrentUserId);

            }
            finally
            {
                OrganizationsSemaphore.Release();
            }

        }

        #endregion

        #region UnlinkOrganizations(OrganizationOut, EdgeLabel, OrganizationIn,                   CurrentUserId = null)

        protected async Task<Boolean> _UnlinkOrganizations(Organization                    OrganizationOut,
                                                           Organization2OrganizationEdges  EdgeLabel,
                                                           Organization                    OrganizationIn,
                                                           User_Id?                        CurrentUserId  = null)
        {

            if (OrganizationOut.
                    Organization2OrganizationOutEdges.
                    Where(edge => edge.Target    == OrganizationIn).
                    Any  (edge => edge.EdgeLabel == EdgeLabel))
            {

                OrganizationOut.RemoveOutEdges(EdgeLabel, OrganizationIn);

                if (OrganizationIn.
                        Organization2OrganizationInEdges.
                        Where(edge => edge.Source    == OrganizationOut).
                        Any  (edge => edge.EdgeLabel == EdgeLabel))
                {
                    OrganizationIn.RemoveInEdges(EdgeLabel, OrganizationOut);
                }

                await WriteToLogfile(NotificationMessageType.Parse("unlinkOrganizations"),
                                     new JObject(
                                         new JProperty("organizationOut", OrganizationOut.Id.ToString()),
                                         new JProperty("edge",            EdgeLabel.         ToString()),
                                         new JProperty("organizationIn",  OrganizationIn. Id.ToString())
                                     ),
                                     CurrentUserId);

                return true;

            }

            return false;

        }

        public async Task<Boolean> UnlinkOrganizations(Organization                    OrganizationOut,
                                                       Organization2OrganizationEdges  EdgeLabel,
                                                       Organization                    OrganizationIn,
                                                       User_Id?                        CurrentUserId  = null)
        {

            try
            {

                await OrganizationsSemaphore.WaitAsync();

                return await _UnlinkOrganizations(OrganizationOut,
                                                  EdgeLabel,
                                                  OrganizationIn,
                                                  CurrentUserId);

            }
            finally
            {
                OrganizationsSemaphore.Release();
            }

        }

        #endregion

        #endregion

        #region Groups

        #region Data

        protected readonly Dictionary<Group_Id, Group> _Groups;

        /// <summary>
        /// Return an enumeration of all groups.
        /// </summary>
        public IEnumerable<Group> Groups
        {
            get
            {
                try
                {
                    GroupsSemaphore.Wait();
                    return _Groups.Values.ToArray();
                }
                finally
                {
                    GroupsSemaphore.Release();
                }

            }
        }

        #endregion


        #region CreateGroup           (Id, Name = null, Description = null)

        public async Task<Group> CreateGroup(Group_Id   Id,
                                             User_Id    CurrentUserId,
                                             I18NString Name         = null,
                                             I18NString Description  = null)
        {

            try
            {

                await GroupsSemaphore.WaitAsync();

                if (_Groups.ContainsKey(Id))
                    throw new ArgumentException("The given group identification already exists!", nameof(Id));


                var Group = new Group(Id,
                                      Name,
                                      Description);

                if (Group.Id.ToString() != AdminGroupName)
                    await WriteToLogfile(NotificationMessageType.Parse("createGroup"),
                                         Group.ToJSON(),
                                         CurrentUserId);

                return _Groups.AddAndReturnValue(Group.Id, Group);

            }
            finally
            {
                GroupsSemaphore.Release();
            }

        }

        #endregion

        #region CreateGroupIfNotExists(Id, Name = null, Description = null)

        public async Task<Group> CreateGroupIfNotExists(Group_Id    Id,
                                                        User_Id     CurrentUserId,
                                                        I18NString  Name         = null,
                                                        I18NString  Description  = null)
        {

            try
            {

                await GroupsSemaphore.WaitAsync();

                if (_Groups.ContainsKey(Id))
                    return _Groups[Id];

                var Group = new Group(Id,
                                      Name,
                                      Description);

                if (Group.Id.ToString() != AdminGroupName)
                    await WriteToLogfile(NotificationMessageType.Parse("createGroup"),
                                         Group.ToJSON(),
                                         CurrentUserId);

                return _Groups.AddAndReturnValue(Group.Id, Group);

            }
            finally
            {
                GroupsSemaphore.Release();
            }

        }

        #endregion


        #region Contains(GroupId)

        /// <summary>
        /// Whether this API contains a group having the given unique identification.
        /// </summary>
        /// <param name="GroupId">The unique identification of the group.</param>
        public Boolean Contains(Group_Id GroupId)
        {

            try
            {

                GroupsSemaphore.Wait();

                return _Groups.ContainsKey(GroupId);

            }
            finally
            {
                GroupsSemaphore.Release();
            }

        }

        #endregion

        #region Get     (GroupId)

        /// <summary>
        /// Get the group having the given unique identification.
        /// </summary>
        /// <param name="GroupId">The unique identification of the group.</param>
        public async Task<Group> Get(Group_Id  GroupId)
        {

            try
            {

                await GroupsSemaphore.WaitAsync();

                if (_Groups.TryGetValue(GroupId, out Group Group))
                    return Group;

                return null;

            }
            finally
            {
                GroupsSemaphore.Release();
            }

        }

        #endregion

        #region TryGet  (GroupId, out Group)

        /// <summary>
        /// Try to get the group having the given unique identification.
        /// </summary>
        /// <param name="GroupId">The unique identification of the group.</param>
        /// <param name="Group">The group.</param>
        public Boolean TryGet(Group_Id   GroupId,
                              out Group  Group)
        {

            try
            {

                GroupsSemaphore.Wait();

                return _Groups.TryGetValue(GroupId, out Group);

            }
            finally
            {
                GroupsSemaphore.Release();
            }

        }

        #endregion

        #region SearchGroupsByName(GroupName)

        /// <summary>
        /// Find all groups having the given name.
        /// </summary>
        /// <param name="GroupName">The name of an group (might not be unique).</param>
        public IEnumerable<Group> SearchGroupsByName(String GroupName)
        {

            try
            {

                GroupsSemaphore.Wait();

                var FoundGroups = new List<Group>();

                foreach (var group in _Groups.Values)
                    if (group.Name.Any(i18npair => i18npair.Text == GroupName))
                        FoundGroups.Add(group);

                return FoundGroups;

            }
            finally
            {
                GroupsSemaphore.Release();
            }

        }

        /// <summary>
        /// Find all groups having the given name.
        /// </summary>
        /// <param name="GroupName">The name of an group (might not be unique).</param>
        public IEnumerable<Group> SearchGroupsByName(I18NString GroupName)
        {

            try
            {

                GroupsSemaphore.Wait();

                var FoundGroups = new List<Group>();

                foreach (var group in _Groups.Values)
                    if (group.Name == GroupName)
                        FoundGroups.Add(group);

                return FoundGroups;

            }
            finally
            {
                GroupsSemaphore.Release();
            }

        }

        #endregion

        #region SearchGroupsByName(GroupName, out Groups)

        /// <summary>
        /// Find all groups having the given name.
        /// </summary>
        /// <param name="GroupName">The name of an group (might not be unique).</param>
        /// <param name="Groups">An enumeration of matching groups.</param>
        public Boolean SearchGroupsByName(String GroupName, out IEnumerable<Group> Groups)
        {

            try
            {

                GroupsSemaphore.Wait();

                var FoundGroups = new List<Group>();

                foreach (var group in _Groups.Values)
                    if (group.Name.Any(i18npair => i18npair.Text == GroupName))
                        FoundGroups.Add(group);

                Groups = FoundGroups;

                return FoundGroups.Count > 0;

            }
            finally
            {
                GroupsSemaphore.Release();
            }

        }

        /// <summary>
        /// Find all groups having the given name.
        /// </summary>
        /// <param name="GroupName">The name of an group (might not be unique).</param>
        /// <param name="Groups">An enumeration of matching groups.</param>
        public Boolean SearchGroupsByName(I18NString GroupName, out IEnumerable<Group> Groups)
        {

            try
            {

                GroupsSemaphore.Wait();

                var FoundGroups = new List<Group>();

                foreach (var group in _Groups.Values)
                    if (group.Name == GroupName)
                        FoundGroups.Add(group);

                Groups = FoundGroups;

                return FoundGroups.Count > 0;

            }
            finally
            {
                GroupsSemaphore.Release();
            }

        }

        #endregion


        #region AddToGroup(User, Edge, Group, PrivacyLevel = Private)

        public async Task<Boolean> AddToGroup(User             User,
                                              User2GroupEdges  Edge,
                                              Group            Group,
                                              PrivacyLevel     PrivacyLevel   = PrivacyLevel.Private,
                                              User_Id?         CurrentUserId  = null)

        {

            try
            {

                await UsersSemaphore.WaitAsync();
                await GroupsSemaphore.WaitAsync();

                if (!User.OutEdges(Group).Any(edge => edge == Edge))
                {

                    User.AddOutgoingEdge(Edge, Group, PrivacyLevel);

                    if (!Group.Edges(Group).Any(edge => edge == Edge))
                        Group.AddIncomingEdge(User, Edge,  PrivacyLevel);

                    await WriteToLogfile(NotificationMessageType.Parse("addUserToGroup"),
                                         new JObject(
                                             new JProperty("user",   User.Id.ToString()),
                                             new JProperty("edge",   Edge.   ToString()),
                                             new JProperty("group",  Group.  ToString()),
                                             PrivacyLevel.ToJSON()
                                         ),
                                         CurrentUserId);

                    return true;

                }

                return false;

            }
            finally
            {
                UsersSemaphore.Release();
                GroupsSemaphore.Release();
            }

        }

        #endregion


        #region IsAdmin(User)

        /// <summary>
        /// Check if the given user is an API admin.
        /// </summary>
        /// <param name="User">A user.</param>
        public Access_Levels IsAdmin(User User)
        {

            if (User.Groups(User2GroupEdges.IsAdmin_ReadOnly).
                     Contains(Admins))
            {
                return Access_Levels.ReadOnly;
            }

            if (User.Groups(User2GroupEdges.IsAdmin_ReadWrite).
                     Contains(Admins))
            {
                return Access_Levels.ReadWrite;
            }

            return Access_Levels.None;

        }

        #endregion

        #region IsAdmin(UserId)

        /// <summary>
        /// Check if the given user is an API admin.
        /// </summary>
        /// <param name="UserId">A user identification.</param>
        public Access_Levels IsAdmin(User_Id UserId)
        {

            if (TryGet(UserId, out User User))
                return IsAdmin(User);

            return Access_Levels.None;

        }

        #endregion

        #endregion

        #region Reset user password

        #region ResetPassword(UserId,  SecurityToken1, SecurityToken2 = null)

        public Task<PasswordReset> ResetPassword(User_Id            UserId,
                                                 SecurityToken_Id   SecurityToken1,
                                                 SecurityToken_Id?  SecurityToken2 = null)

            => Add(new PasswordReset(new User_Id[] { UserId },
                                     SecurityToken1,
                                     SecurityToken2));

        #endregion

        #region ResetPassword(UserIds, SecurityToken1, SecurityToken2 = null)

        public Task<PasswordReset> ResetPassword(IEnumerable<User_Id>  UserIds,
                                                 SecurityToken_Id      SecurityToken1,
                                                 SecurityToken_Id?     SecurityToken2 = null)

            => Add(new PasswordReset(UserIds,
                                     SecurityToken1,
                                     SecurityToken2));

        #endregion

        #region Add   (passwordReset)

        public async Task<PasswordReset> Add(PasswordReset passwordReset)
        {

            try
            {

                await UsersSemaphore.WaitAsync();

                await WriteToLogfile(NotificationMessageType.Parse("add"),
                                     passwordReset.ToJSON(),
                                     this.UsersAPIPath + DefaultPasswordResetsFile);

                this.PasswordResets.Add(passwordReset.SecurityToken1, passwordReset);

                return passwordReset;

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region Remove(passwordReset)

        public async Task<PasswordReset> Remove(PasswordReset passwordReset)
        {

            try
            {

                await UsersSemaphore.WaitAsync();

                await WriteToLogfile(NotificationMessageType.Parse("remove"),
                                     passwordReset.ToJSON(),
                                     this.UsersAPIPath + DefaultPasswordResetsFile);

                this.PasswordResets.Remove(passwordReset.SecurityToken1);

                return passwordReset;

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #endregion

        #region API Keys

        protected readonly Dictionary<APIKey, APIKeyInfo> _APIKeys;

        /// <summary>
        /// Return an enumeration of all API keys.
        /// </summary>
        public IEnumerable<APIKeyInfo> APIKeys
            => _APIKeys.Values;


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

        #region IsValidAPIKey(APIKey)

        public Boolean IsValidAPIKey(APIKey APIKey)

            => TryGetAPIKeyInfo(APIKey, out APIKeyInfo apiKeyInfo) &&
                   (!apiKeyInfo.NotBefore.HasValue || DateTime.UtcNow >= apiKeyInfo.NotBefore) &&
                   (!apiKeyInfo.NotAfter. HasValue || DateTime.UtcNow <  apiKeyInfo.NotAfter)  &&
                    !apiKeyInfo.IsDisabled;

        #endregion

        #region GetAPIKeysFor   (User)

        /// <summary>
        /// Return all API keys for the given user.
        /// </summary>
        /// <param name="User">An user.</param>
        public IEnumerable<APIKeyInfo> GetAPIKeysFor(User User)
        {

            lock (_APIKeys)
            {
                return _APIKeys.Values.Where(apikey => apikey.User == User);
            }

        }

        #endregion

        #region AddAPIKey       (APIKey, UserId)

        public async Task<APIKeyInfo> AddAPIKey(APIKeyInfo  APIKey,
                                                User_Id     UserId)
        {

            try
            {

                await UsersSemaphore.WaitAsync();

                if (_APIKeys.ContainsKey(APIKey.APIKey))
                    return _APIKeys[APIKey.APIKey];

                await WriteToLogfile(NotificationMessageType.Parse("addAPIKey"),
                                     APIKey.ToJSON(true),
                                     UserId);

                return _APIKeys.AddAndReturnValue(APIKey.APIKey, APIKey);

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #endregion

        #region Notifications

        // ToDo: Add locks
        // ToDo: Add logging!

        #region AddNotification(User,   NotificationType,                           CurrentUserId = null)

        public async Task AddNotification<T>(User      User,
                                             T         NotificationType,
                                             User_Id?  CurrentUserId  = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                User.AddNotification(NotificationType,
                                     async update => await WriteToLogfile(NotificationMessageType.Parse("addNotification"),
                                                                          update.ToJSON(false).AddFirstAndReturn(new JProperty("userId", User.Id.ToString())),
                                                                          CurrentUserId));

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region AddNotification(UserId, NotificationType,                           CurrentUserId = null)

        public async Task AddNotification<T>(User_Id   UserId,
                                             T         NotificationType,
                                             User_Id?  CurrentUserId  = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                if (_Users.TryGetValue(UserId, out User User))
                {

                    User.AddNotification(NotificationType,
                                         async update => await WriteToLogfile(NotificationMessageType.Parse("addNotification"),
                                                                              update.ToJSON(false).AddFirstAndReturn(new JProperty("userId", User.Id.ToString())),
                                                                              CurrentUserId));

                }

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region AddNotification(User,   NotificationType, NotificationMessageType,  CurrentUserId = null)

        public async Task AddNotification<T>(User                     User,
                                             T                        NotificationType,
                                             NotificationMessageType  NotificationMessageType,
                                             User_Id?                 CurrentUserId  = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                User.AddNotification(NotificationType,
                                     NotificationMessageType,
                                     async update => await WriteToLogfile(NotificationMessageType.Parse("addNotification"),
                                                                          update.ToJSON(false).AddFirstAndReturn(new JProperty("userId", User.Id.ToString())),
                                                                          CurrentUserId));

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region AddNotification(UserId, NotificationType, NotificationMessageType,  CurrentUserId = null)

        public async Task AddNotification<T>(User_Id                  UserId,
                                             T                        NotificationType,
                                             NotificationMessageType  NotificationMessageType,
                                             User_Id?                 CurrentUserId  = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                if (_Users.TryGetValue(UserId, out User User))
                {

                    User.AddNotification(NotificationType,
                                         NotificationMessageType,
                                         async update => await WriteToLogfile(NotificationMessageType.Parse("addNotification"),
                                                                              update.ToJSON(false).AddFirstAndReturn(new JProperty("userId", User.Id.ToString())),
                                                                              CurrentUserId));

                }

                else
                    throw new ArgumentException("The given user '" + UserId + "' is unknown!");

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region AddNotification(User,   NotificationType, NotificationMessageTypes, CurrentUserId = null)

        public async Task AddNotification<T>(User                                  User,
                                             T                                     NotificationType,
                                             IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                             User_Id?                              CurrentUserId  = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                User.AddNotification(NotificationType,
                                     NotificationMessageTypes,
                                     async update => await WriteToLogfile(NotificationMessageType.Parse("addNotification"),
                                                                          update.ToJSON(false).AddFirstAndReturn(new JProperty("userId", User.Id.ToString())),
                                                                          CurrentUserId));

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region AddNotification(UserId, NotificationType, NotificationMessageTypes, CurrentUserId = null)

        public async Task AddNotification<T>(User_Id                               UserId,
                                             T                                     NotificationType,
                                             IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                             User_Id?                              CurrentUserId  = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                if (_Users.TryGetValue(UserId, out User User))
                {

                    User.AddNotification(NotificationType,
                                         NotificationMessageTypes,
                                         async update => await WriteToLogfile(NotificationMessageType.Parse("addNotification"),
                                                                              update.ToJSON(false).AddFirstAndReturn(new JProperty("userId", User.Id.ToString())),
                                                                              CurrentUserId));

                }

                else
                    throw new ArgumentException("The given user '" + UserId + "' is unknown!");

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion


        #region GetNotifications  (User,   NotificationMessageType = null)

        public IEnumerable<ANotification> GetNotifications(User                      User,
                                                           NotificationMessageType?  NotificationMessageType = null)

            => User.GetNotifications(NotificationMessageType);

        #endregion

        #region GetNotifications  (UserId, NotificationMessageType = null)

        public IEnumerable<ANotification> GetNotifications<T>(User_Id                   UserId,
                                                                  NotificationMessageType?  NotificationMessageType = null)

            => TryGet(UserId, out User User)
                   ? User.GetNotifications(NotificationMessageType)
                   : new ANotification[0];

        #endregion

        #region GetNotificationsOf(User,   params NotificationMessageTypes)

        public IEnumerable<T> GetNotificationsOf<T>(User                              User,
                                                    params NotificationMessageType[]  NotificationMessageTypes)

            where T : ANotification

            => User.GetNotificationsOf<T>(NotificationMessageTypes);

        #endregion

        #region GetNotificationsOf(UserId, params NotificationMessageTypes)

        public IEnumerable<T> GetNotificationsOf<T>(User_Id                           UserId,
                                                    params NotificationMessageType[]  NotificationMessageTypes)

            where T : ANotification

            => TryGet(UserId, out User User)
                   ? User.GetNotificationsOf<T>(NotificationMessageTypes)
                   : new T[0];

        #endregion


        #region GetNotificationsOf(Organization,   params NotificationMessageTypes)

        public IEnumerable<T> GetNotificationsOf<T>(Organization                      Organization,
                                                    params NotificationMessageType[]  NotificationMessageTypes)
            where T : ANotification

            => Organization.
                   GetMeAndAllMyParents(parent => parent != NoOwner).
                   SelectMany          (parent => parent.User2OrganizationEdges).
                   SelectMany          (edge   => GetNotificationsOf<T>(edge.Source.Id, NotificationMessageTypes));

        #endregion

        #region GetNotificationsOf(OrganizationId, params NotificationMessageTypes)

        public IEnumerable<T> GetNotificationsOf<T>(Organization_Id                   OrganizationId,
                                                    params NotificationMessageType[]  NotificationMessageTypes)

            where T : ANotification

            => TryGet(OrganizationId, out Organization Organization)
                   ? GetNotificationsOf<T>(Organization, NotificationMessageTypes)
                   : new T[0];

        #endregion


        #region GetNotifications  (User,   NotificationMessageTypeFilter)

        public IEnumerable<ANotification> GetNotifications(User                                    User,
                                                           Func<NotificationMessageType, Boolean>  NotificationMessageTypeFilter)

            => User.GetNotifications(NotificationMessageTypeFilter);

        #endregion

        #region GetNotifications  (UserId, NotificationMessageTypeFilter)

        public IEnumerable<ANotification> GetNotifications(User_Id                                 UserId,
                                                           Func<NotificationMessageType, Boolean>  NotificationMessageTypeFilter)

            => TryGet(UserId, out User User)
                   ? User.GetNotifications(NotificationMessageTypeFilter)
                   : new ANotification[0];

        #endregion

        #region GetNotificationsOf(User,   NotificationMessageTypeFilter)

        public IEnumerable<T> GetNotificationsOf<T>(User                                    User,
                                                    Func<NotificationMessageType, Boolean>  NotificationMessageTypeFilter)

            where T : ANotification

            => User.GetNotificationsOf<T>(NotificationMessageTypeFilter);

        #endregion

        #region GetNotificationsOf(UserId, NotificationMessageTypeFilter)

        public IEnumerable<T> GetNotificationsOf<T>(User_Id                                 UserId,
                                                    Func<NotificationMessageType, Boolean>  NotificationMessageTypeFilter)

            where T : ANotification

            => TryGet(UserId, out User User)
                   ? User.GetNotificationsOf<T>(NotificationMessageTypeFilter)
                   : new T[0];

        #endregion


        #region RemoveNotification(User,   NotificationType,                        CurrentUserId = null)

        public async Task RemoveNotification<T>(User      User,
                                                T         NotificationType,
                                                User_Id?  CurrentUserId  = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                User.RemoveNotification(NotificationType,
                                        async update => await WriteToLogfile(NotificationMessageType.Parse("removeNotification"),
                                                                             update.ToJSON(false).AddFirstAndReturn(new JProperty("userId", User.Id.ToString())),
                                                                             CurrentUserId));

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region RemoveNotification(UserId, NotificationType,                        CurrentUserId = null)

        public async Task RemoveNotification<T>(User_Id   UserId,
                                                T         NotificationType,
                                                User_Id?  CurrentUserId  = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                if (_Users.TryGetValue(UserId, out User User))
                {

                    User.RemoveNotification(NotificationType,
                                            async update => await WriteToLogfile(NotificationMessageType.Parse("removeNotification"),
                                                                                 update.ToJSON(false).AddFirstAndReturn(new JProperty("userId", User.Id.ToString())),
                                                                                 CurrentUserId));

                }

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #endregion

        #region Messages

        protected readonly Dictionary<Message_Id, Message> _Messages;

        /// <summary>
        /// Return an enumeration of all messages.
        /// </summary>
        public IEnumerable<Message> Messages
            => _Messages.Values;


        #region CreateMessage(Id, Sender, Receivers, Headline = null, Text = null)

        public async Task<Message> CreateMessage(User_Id               Sender,
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

        // Create Mailinglist

        #endregion

        #region DataLicenses

        protected readonly Dictionary<DataLicense_Id, DataLicense> _DataLicenses;

        /// <summary>
        /// Return an enumeration of all data licenses.
        /// </summary>
        public IEnumerable<DataLicense> DataLicenses
            => _DataLicenses.Values;


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

                WriteToLogfile(NotificationMessageType.Parse("createDataLicense"),
                               DataLicense.ToJSON(),
                               Robot.Id);

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


    }

}
