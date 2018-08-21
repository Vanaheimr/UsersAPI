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
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Warden;

using SMSApi.Api;
using org.GraphDefined.OpenData.Notifications;

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

            if (!UsersAPI.TryGetUser(UserId.Value, out User)) {

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

    public class PasswordReset
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of the object.
        /// </summary>
        public const String JSONLDContext = "https://cardi-link.cloud/contexts/cardidb+json/passwordReset";

        #endregion

        #region Properties

        public DateTime              Timestamp        { get; }
        public IEnumerable<User_Id>  UserIds          { get; }
        public SecurityToken_Id      SecurityToken1   { get; }
        public SecurityToken_Id?     SecurityToken2   { get; }

        #endregion

        #region Constructor(s)

        public PasswordReset(IEnumerable<User_Id>  UserIds,
                             SecurityToken_Id      SecurityToken1,
                             SecurityToken_Id?     SecurityToken2)

            : this(DateTime.UtcNow,
                   UserIds,
                   SecurityToken1,
                   SecurityToken2)

        { }

        public PasswordReset(DateTime              Timestamp,
                             IEnumerable<User_Id>  UserIds,
                             SecurityToken_Id      SecurityToken1,
                             SecurityToken_Id?     SecurityToken2)
        {

            this.Timestamp       = Timestamp;
            this.UserIds         = UserIds;
            this.SecurityToken1  = SecurityToken1;
            this.SecurityToken2  = SecurityToken2;

        }

        #endregion


        #region ToJSON(Embedded = true)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        public JObject ToJSON(Boolean Embedded = true)

            => JSONObject.Create(

                   Embedded
                       ? null
                       : new JProperty("@context",  JSONLDContext),

                   new JProperty("timestamp",       Timestamp.ToIso8601()),
                   new JProperty("userIds",         new JArray(UserIds.Select(user => user.ToString()))),
                   new JProperty("securityToken1",  SecurityToken1.ToString()),

                   SecurityToken2.HasValue
                       ? new JProperty("securityToken2",  SecurityToken2.ToString())
                       : null

               );

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out Communicator, out ErrorResponse, IgnoreContextMismatches = true)

        public static Boolean TryParseJSON(JObject              JSONObject,
                                           out PasswordReset    PasswordReset,
                                           out String           ErrorResponse,
                                           Boolean              IgnoreContextMismatches = true)

        {

            try
            {

                PasswordReset = null;

                if (JSONObject == null)
                {
                    ErrorResponse = "The given JSON object must not be null!";
                    return false;
                }

                #region Parse Context          [mandatory]

                if (!IgnoreContextMismatches)
                {

                    if (!JSONObject.ParseMandatory("@context",
                                                   "JSON-LD context",
                                                   out String Context,
                                                   out ErrorResponse))
                    {
                        ErrorResponse = @"The JSON-LD ""@context"" information is missing!";
                        return false;
                    }

                    if (Context != JSONLDContext)
                    {
                        ErrorResponse = @"The given JSON-LD ""@context"" information '" + Context + "' is not supported!";
                        return false;
                    }

                }

                #endregion

                #region Parse Timestamp        [mandatory]

                if (!JSONObject.ParseMandatory("timestamp",
                                               "timestamp",
                                               out DateTime Timestamp,
                                               out          ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse UserIds          [mandatory]

                if (!JSONObject.ParseMandatory("userIds",
                                               "user identifications",
                                               out JArray UserIdArray,
                                               out ErrorResponse))
                {
                    return false;
                }

                var UserIds = new User_Id[0];

                try
                {
                    UserIds = UserIdArray.Select(jsonvalue => User_Id.Parse(jsonvalue.Value<String>())).ToArray();
                }
                catch (Exception e)
                {
                    ErrorResponse = "The given array of users '" + UserIdArray + "' is invalid!";
                    return false;
                }

                if (UserIds.Length == 0)
                {
                    ErrorResponse = "The given array of users '" + UserIdArray + "' is invalid!";
                    return false;
                }

                #endregion

                #region Parse SecurityToken1   [mandatory]

                if (!JSONObject.ParseMandatory("securityToken1",
                                               "security token #1",
                                               SecurityToken_Id.TryParse,
                                               out SecurityToken_Id SecurityToken1,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse SecurityToken2   [optional]

                if (!JSONObject.ParseOptionalN("securityToken2",
                                               "security token #2",
                                               SecurityToken_Id.TryParse,
                                               out SecurityToken_Id? SecurityToken2,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion


                PasswordReset = new PasswordReset(Timestamp,
                                                  UserIds,
                                                  SecurityToken1,
                                                  SecurityToken2);

                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = e.Message;
                PasswordReset = null;
                return false;
            }

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

        private static readonly SemaphoreSlim LogFileSemaphore = new SemaphoreSlim(1, 1);


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
        public  const             String                              DefaultHTTPCookiesFile         = "UsersAPI_HTTPCookies.db";
        public  const             String                              DefaultPasswordResetsFile      = "UsersAPI_PasswordResets.db";
        //public  const             String                              DefaultGroupDBFile             = "UsersAPI_Groups.db";
        //public  const             String                              DefaultUser2GroupDBFile        = "UsersAPI_User2Group.db";
        public  const             String                              AdminGroupName                 = "Admins";


        private readonly          SMSAPI                              _SMSAPI;

        protected readonly Dictionary<SecurityToken_Id, SecurityToken> HTTPCookies;
        protected readonly Dictionary<SecurityToken_Id, PasswordReset> PasswordResets;

        public static readonly Regex JSONWhitespaceRegEx = new Regex(@"(\s)+", RegexOptions.IgnorePatternWhitespace);

        public static Organization NoOwner;

        private readonly Queue<NotificationMessage> _NotificationMessages;

        public IEnumerable<NotificationMessage> NotificationMessages
            => _NotificationMessages;

        #endregion

        #region Properties

        /// <summary>
        /// The current async local user identification to simplify API usage.
        /// </summary>
        protected internal static AsyncLocal<User_Id?> CurrentAsyncLocalUserId = new AsyncLocal<User_Id?>();

        public User          Robot        { get; }

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


        /// <summary>
        /// The credentials used for the SMS API.
        /// </summary>
        public Credentials               SMSAPICredentials          { get; }

        /// <summary>
        /// A list of admin SMS phonenumbers.
        /// </summary>
        public IEnumerable<PhoneNumber>  APIAdminSMS                { get; }


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
                                                                EMailAddress      EMail,
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
                                                                EMailAddress      EMail,
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

        #region MinPasswordLenght

        /// <summary>
        /// The minimal password length.
        /// </summary>
        public Byte MinPasswordLenght { get; }

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
        public Boolean       DisableNotifications       { get; }

        /// <summary>
        /// The logfile of this API.
        /// </summary>
        public String        LogfileName                { get; }


        public Warden        Warden                     { get; }

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


        #region (protected internal) SetUserNotificationsRequest (Request)

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

        #region (protected internal) SetUserNotificationsResponse(Response)

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
        /// <param name="APIPassphrase">A GPG passphrase for this API.</param>
        /// <param name="APIAdminEMails">A list of admin e-mail addresses.</param>
        /// <param name="APISMTPClient">A SMTP client for sending e-mails.</param>
        /// 
        /// <param name="SMSAPICredentials">The credentials for the SMS API.</param>
        /// <param name="APIAdminSMS">A list of admin SMS phonenumbers.</param>
        /// 
        /// <param name="CookieName">The name of the HTTP Cookie for authentication.</param>
        /// <param name="Language">The main language of the API.</param>
        /// <param name="LogoImage">The logo of the website.</param>
        /// <param name="NewUserSignUpEMailCurrentUserId">A delegate for sending a sign-up e-mail to a new user.</param>
        /// <param name="NewUserWelcomeEMailCurrentUserId">A delegate for sending a welcome e-mail to a new user.</param>
        /// <param name="ResetPasswordEMailCurrentUserId">A delegate for sending a reset password e-mail to a user.</param>
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
                        String                               APIPassphrase                      = null,
                        EMailAddressList                     APIAdminEMails                     = null,
                        SMTPClient                           APISMTPClient                      = null,

                        Credentials                          SMSAPICredentials                  = null,
                        IEnumerable<PhoneNumber>             APIAdminSMS                        = null,

                        HTTPCookieName?                      CookieName                         = null,
                        Languages                            Language                           = DefaultLanguage,
                        String                               LogoImage                          = null,
                        NewUserSignUpEMailCreatorDelegate    NewUserSignUpEMailCreator          = null,
                        NewUserWelcomeEMailCreatorDelegate   NewUserWelcomeEMailCreator         = null,
                        ResetPasswordEMailCreatorDelegate    ResetPasswordEMailCreator          = null,
                        PasswordChangedEMailCreatorDelegate  PasswordChangedEMailCreator        = null,
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
                   APIPassphrase,
                   APIAdminEMails,
                   APISMTPClient,

                   SMSAPICredentials,
                   APIAdminSMS,

                   CookieName,
                   Language,
                   LogoImage,
                   NewUserSignUpEMailCreator,
                   NewUserWelcomeEMailCreator,
                   ResetPasswordEMailCreator,
                   PasswordChangedEMailCreator,
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
        /// <param name="APIPassphrase">A GPG passphrase for this API.</param>
        /// <param name="APIAdminEMails">A list of admin e-mail addresses.</param>
        /// <param name="APISMTPClient">A SMTP client for sending e-mails.</param>
        /// 
        /// <param name="SMSAPICredentials">The credentials for the SMS API.</param>
        /// <param name="APIAdminSMS">A list of admin SMS phonenumbers.</param>
        /// 
        /// <param name="CookieName">The name of the HTTP Cookie for authentication.</param>
        /// <param name="Language">The main language of the API.</param>
        /// <param name="LogoImage">The logo of the website.</param>
        /// <param name="NewUserSignUpEMailCurrentUserId">A delegate for sending a sign-up e-mail to a new user.</param>
        /// <param name="NewUserWelcomeEMailCurrentUserId">A delegate for sending a welcome e-mail to a new user.</param>
        /// <param name="ResetPasswordEMailCurrentUserId">A delegate for sending a reset password e-mail to a user.</param>
        /// <param name="MinUserNameLenght">The minimal user name length.</param>
        /// <param name="MinRealmLenght">The minimal realm length.</param>
        /// <param name="MinPasswordLenght">The minimal password length.</param>
        /// <param name="SignInSessionLifetime">The sign-in session lifetime.</param>
        /// 
        /// <param name="SkipURITemplates">Skip URI templates.</param>
        /// <param name="DisableNotifications">Disable external notifications.</param>
        /// <param name="DisableLogfile">Disable the log file.</param>
        /// <param name="LogfileName">The name of the logfile for this API.</param>
        protected UsersAPI(HTTPServer                           HTTPServer,
                           HTTPHostname?                        HTTPHostname                  = null,
                           HTTPURI?                             URIPrefix                     = null,

                           String                               ServiceName                   = DefaultServiceName,
                           EMailAddress                         APIEMailAddress               = null,
                           String                               APIPassphrase                 = null,
                           EMailAddressList                     APIAdminEMails                = null,
                           SMTPClient                           APISMTPClient                 = null,

                           Credentials                          SMSAPICredentials             = null,
                           IEnumerable<PhoneNumber>             APIAdminSMS                   = null,

                           HTTPCookieName?                      CookieName                    = null,
                           Languages                            Language                      = DefaultLanguage,
                           String                               LogoImage                     = null,
                           NewUserSignUpEMailCreatorDelegate    NewUserSignUpEMailCreator     = null,
                           NewUserWelcomeEMailCreatorDelegate   NewUserWelcomeEMailCreator    = null,
                           ResetPasswordEMailCreatorDelegate    ResetPasswordEMailCreator     = null,
                           PasswordChangedEMailCreatorDelegate  PasswordChangedEMailCreator   = null,
                           Byte                                 MinUserNameLenght             = DefaultMinUserNameLenght,
                           Byte                                 MinRealmLenght                = DefaultMinRealmLenght,
                           Byte                                 MinPasswordLenght             = DefaultMinPasswordLenght,
                           TimeSpan?                            SignInSessionLifetime         = null,

                           Boolean                              SkipURITemplates              = false,
                           Boolean                              DisableNotifications          = false,
                           Boolean                              DisableLogfile                = false,
                           String                               LogfileName                   = DefaultLogfileName)

            : base(HTTPServer,
                   HTTPHostname,
                   URIPrefix,
                   ServiceName)

        {

            #region Initial checks

            if (NewUserSignUpEMailCreator  == null)
                throw new ArgumentNullException(nameof(NewUserSignUpEMailCreator),   "NewUserSignUpEMailCreator!");

            if (NewUserWelcomeEMailCreator == null)
                throw new ArgumentNullException(nameof(NewUserWelcomeEMailCreator),  "NewUserWelcomeEMailCreator!");

            if (ResetPasswordEMailCreator  == null)
                throw new ArgumentNullException(nameof(ResetPasswordEMailCreator),   "ResetPasswordEMailCreator!");

            #endregion

            #region Init data

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

            this.CookieName                   = CookieName ?? DefaultCookieName;
            this.Language                     = Language;
            this._LogoImage                   = LogoImage;
            this.NewUserSignUpEMailCreator    = NewUserSignUpEMailCreator;
            this.NewUserWelcomeEMailCreator   = NewUserWelcomeEMailCreator;
            this.ResetPasswordEMailCreator    = ResetPasswordEMailCreator;
            this.PasswordChangedEMailCreator  = PasswordChangedEMailCreator;
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

            this.HTTPCookies                  = new Dictionary<SecurityToken_Id, SecurityToken>();
            this.PasswordResets               = new Dictionary<SecurityToken_Id, PasswordReset>();

            this.DisableNotifications         = DisableNotifications;
            this.DisableLogfile               = DisableLogfile;
            this.LogfileName                  = LogfileName ?? DefaultLogfileName;

            this._APIKeys                     = new Dictionary<APIKey, APIKeyInfo>();

            this._NotificationMessages        = new Queue<NotificationMessage>();

            #endregion

            this.Admins  = CreateGroupIfNotExists(Group_Id.Parse(AdminGroupName),
                                                  Robot.Id,
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

            NoOwner = CreateOrganizationIfNotExists(Organization_Id.Parse("NoOwner"), CurrentUserId: Robot.Id);

            if (SMSAPICredentials != null)
            {
                this.SMSAPICredentials  = SMSAPICredentials;
                this._SMSAPI            = new SMSAPI(this.SMSAPICredentials);
            }

            this.Warden = new Warden(InitialDelay: TimeSpan.FromMinutes(3));

            if (!SkipURITemplates)
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
        /// <param name="URIPrefix">A common prefix for all URIs.</param>
        /// 
        /// <param name="ServiceName">The name of the service.</param>
        /// <param name="APIEMailAddress">An e-mail address for this API.</param>
        /// <param name="APIPassphrase">A GPG passphrase for this API.</param>
        /// <param name="APIAdminEMails">A list of admin e-mail addresses.</param>
        /// <param name="APISMTPClient">A SMTP client for sending e-mails.</param>
        /// 
        /// <param name="SMSAPICredentials">The credentials for the SMS API.</param>
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
        /// <param name="SkipURITemplates">Skip URI templates.</param>
        /// <param name="DisableNotifications">Disable external notifications.</param>
        /// <param name="DisableLogfile">Disable the log file.</param>
        /// <param name="LogfileName">The name of the logfile for this API.</param>
        public static UsersAPI AttachToHTTPAPI(HTTPServer                           HTTPServer,
                                               HTTPHostname?                        HTTPHostname                  = null,
                                               HTTPURI?                             URIPrefix                     = null,

                                               String                               ServiceName                   = DefaultServiceName,
                                               EMailAddress                         APIEMailAddress               = null,
                                               String                               APIPassphrase                 = null,
                                               EMailAddressList                     APIAdminEMails                = null,
                                               SMTPClient                           APISMTPClient                 = null,

                                               Credentials                          SMSAPICredentials             = null,
                                               IEnumerable<PhoneNumber>             APIAdminSMS                   = null,

                                               HTTPCookieName?                      CookieName                    = null,
                                               Languages                            DefaultLanguage               = Languages.eng,
                                               String                               LogoImage                     = null,
                                               NewUserSignUpEMailCreatorDelegate    NewUserSignUpEMailCreator     = null,
                                               NewUserWelcomeEMailCreatorDelegate   NewUserWelcomeEMailCreator    = null,
                                               ResetPasswordEMailCreatorDelegate    ResetPasswordEMailCreator     = null,
                                               PasswordChangedEMailCreatorDelegate  PasswordChangedEMailCreator   = null,
                                               Byte                                 MinUserNameLenght             = DefaultMinUserNameLenght,
                                               Byte                                 MinRealmLenght                = DefaultMinRealmLenght,
                                               Byte                                 MinPasswordLenght             = DefaultMinPasswordLenght,
                                               TimeSpan?                            SignInSessionLifetime         = null,

                                               Boolean                              SkipURITemplates              = false,
                                               Boolean                              DisableNotifications          = false,
                                               Boolean                              DisableLogfile                = false,
                                               String                               LogfileName                   = DefaultLogfileName)


            => new UsersAPI(HTTPServer,
                            HTTPHostname,
                            URIPrefix,

                            ServiceName,
                            APIEMailAddress,
                            APIPassphrase,
                            APIAdminEMails,
                            APISMTPClient,

                            SMSAPICredentials,
                            APIAdminSMS,

                            CookieName,
                            DefaultLanguage,
                            LogoImage,
                            NewUserSignUpEMailCreator,
                            NewUserWelcomeEMailCreator,
                            ResetPasswordEMailCreator,
                            PasswordChangedEMailCreator,
                            MinUserNameLenght,
                            MinRealmLenght,
                            MinPasswordLenght,
                            SignInSessionLifetime,

                            SkipURITemplates,
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


        #region (private) GetOrganizationSerializator (User)

        private OrganizationToJSONDelegate GetOrganizationSerializator(User User)
        {

            switch (User?.Id.ToString())
            {

                //case __issapi:
                //    return ISSNotificationExtentions.ToISSJSON;

                default:
                    return (Organization,
                            Embedded,
                            ExpandTags,
                            IncludeCryptoHash)

                            => Organization.ToJSON(Embedded,
                                                   ExpandTags,
                                                   IncludeCryptoHash);

            }

        }

        #endregion


        #region (private) CheckImpersonate(currentOrg, Astronaut, AstronautFound, Member, VetoUsers)

        private Boolean? CheckImpersonate(Organization currentOrg, User Astronaut, Boolean AstronautFound, User Member, HashSet<User> VetoUsers)
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

        public Boolean CanImpersonate(User Astronaut, User Member)
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


        #region (private) RegisterURITemplates()

        private void RegisterURITemplates()
        {

            #region /shared/UsersAPI

            HTTPServer.RegisterResourcesFolder(HTTPHostname.Any,
                                               URIPrefix + "shared/UsersAPI",
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

                                              if (Password.Length < MinPasswordLenght)
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

                                              }

                                              if (!_LoginPassword.VerifyPassword(Password))
                                              {

                                                  return Task.FromResult(
                                                      new HTTPResponse.Builder(Request) {
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

                                              var SHA256Hash       = new SHA256Managed();
                                              var SecurityTokenId  = SecurityToken_Id.Parse(SHA256Hash.ComputeHash(
                                                                                                String.Concat(Guid.NewGuid().ToString(),
                                                                                                              _LoginPassword.Login).
                                                                                                ToUTF8Bytes()
                                                                                            ).ToHexString());

                                              var Expires          = DateTime.UtcNow.Add(SignInSessionLifetime);

                                              lock (HTTPCookies)
                                              {

                                                  HTTPCookies.Add(SecurityTokenId,
                                                                  new SecurityToken(_LoginPassword.Login,
                                                                                    Expires));

                                                  File.AppendAllText(DefaultHTTPCookiesFile,
                                                                     SecurityTokenId + ";" + _LoginPassword.Login + ";" + Expires.ToIso8601() + Environment.NewLine);

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
                                                      SetCookie       = CookieName + "=login=" + _LoginPassword.Login.ToString().ToBase64() +
                                                                                  ":username=" + _User.Name.ToBase64() +
                                                                                  ":email="    + _User.EMail.Address.ToString().ToBase64() +
                                                                                (IsAdmin(_User) ? ":isAdmin" : "") +
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
                                         URIPrefix + "lostpassword",
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
                                         URIPrefix + "resetPassword",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Parse JSON

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out HTTPResponse HTTPResponse))
                                             {

                                                 // Slow down attackers!
                                                 Thread.Sleep(5000);

                                                 return Task.FromResult(HTTPResponse);

                                             }

                                             var idJSON = JSONObj["id"].Value<String>();

                                             if (idJSON != null)
                                                 idJSON = idJSON.Trim();

                                             if (idJSON.IsNullOrEmpty() || idJSON.Length < 4)
                                             {

                                                 // Slow down attackers!
                                                 Thread.Sleep(5000);

                                                 return Task.FromResult(
                                                            new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                                Server                     = HTTPServer.DefaultServerName,
                                                                Date                       = DateTime.UtcNow,
                                                                AccessControlAllowOrigin   = "*",
                                                                AccessControlAllowMethods  = "SET",
                                                                AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                Connection                 = "close"
                                                            }.AsImmutable);

                                             }

                                             #endregion

                                             #region Find user(s)...

                                             var Users = new HashSet<User>();

                                             if (User_Id.TryParse(idJSON, out User_Id UserId) &&
                                                 TryGetUser(UserId, out User User))
                                             {
                                                 Users.Add(User);
                                             }

                                             if (SimpleEMailAddress.TryParse(idJSON, out SimpleEMailAddress EMailAddress))
                                             {
                                                 foreach (var user in Users)
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

                                                 return Task.FromResult(
                                                            new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode             = HTTPStatusCode.NotFound,
                                                                Server                     = HTTPServer.DefaultServerName,
                                                                Date                       = DateTime.UtcNow,
                                                                AccessControlAllowOrigin   = "*",
                                                                AccessControlAllowMethods  = "SET",
                                                                AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                Connection                 = "close"
                                                            }.AsImmutable);

                                             }

                                             #endregion


                                             var PasswordReset    = ResetPassword(Users.Select(user => user.Id),
                                                                                  SecurityToken_Id.Random(40, _Random),
                                                                                  SecurityToken_Id.Parse(_Random.RandomString(5) + "-" + _Random.RandomString(5)));

                                             var MailSentResults  = new List<MailSentStatus>();
                                             var SMSSentResults   = new List<SMSApi.Api.Response.Status>();

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

                                                 return Task.FromResult(
                                                        new HTTPResponse.Builder(Request) {
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
                                                        }.AsImmutable);

                                             }


                                             return Task.FromResult(
                                                        new HTTPResponse.Builder(Request) {
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
                                                        }.AsImmutable);

                                             },

                                             AllowReplacement: URIReplacement.Allow);

            #endregion

            #region SET         ~/setPassword

            // ------------------------------------------------------------------
            // curl -v -X SET
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
                                         URIPrefix + "setPassword",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Parse JSON

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out HTTPResponse HTTPResponse))
                                             {

                                                 // Slow down attackers!
                                                 Thread.Sleep(5000);

                                                 return Task.FromResult(HTTPResponse);

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
                                                 return Task.FromResult(HTTPResponse);
                                             }

                                             #endregion

                                             #region Parse SecurityToken2   [optional]

                                             if (!JSONObj.ParseOptionalN("securityToken2",
                                                                         "security token #2",
                                                                         HTTPServer.DefaultServerName,
                                                                         SecurityToken_Id.TryParse,
                                                                         out SecurityToken_Id? SecurityToken2,
                                                                         Request,
                                                                         out ErrorResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse);
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
                                                 return Task.FromResult(HTTPResponse);
                                             }

                                             #endregion


                                             #region Verify token/password lengths...

                                             if (SecurityToken1.Length != 40 ||
                                                 NewPassword.Length < 6)
                                             {

                                                 // Slow down attackers!
                                                 Thread.Sleep(5000);

                                                 return Task.FromResult(
                                                            new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                                Server                     = HTTPServer.DefaultServerName,
                                                                Date                       = DateTime.UtcNow,
                                                                AccessControlAllowOrigin   = "*",
                                                                AccessControlAllowMethods  = "SET",
                                                                AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                Connection                 = "close"
                                                            }.AsImmutable);

                                             }

                                             #endregion

                                             #endregion

                                             #region Is this a known/valid request?

                                             if (!PasswordResets.TryGetValue(SecurityToken1, out PasswordReset _PasswordReset))
                                             {

                                                 return Task.FromResult(
                                                            new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                                Server                     = HTTPServer.DefaultServerName,
                                                                Date                       = DateTime.UtcNow,
                                                                AccessControlAllowOrigin   = "*",
                                                                AccessControlAllowMethods  = "SET",
                                                                AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                Connection                 = "close"
                                                            }.AsImmutable);

                                             }

                                             #endregion

                                             #region Is security token 2 used and valid?

                                             if (SecurityToken2.HasValue                &&
                                                 SecurityToken2.Value.ToString() != ""  &&
                                                 SecurityToken2.Value            != _PasswordReset.SecurityToken2)
                                             {

                                                 return Task.FromResult(
                                                        new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.Forbidden,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = DateTime.UtcNow,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = "SET",
                                                            AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                            Connection                 = "close"
                                                        }.AsImmutable);

                                             }

                                             #endregion


                                             foreach (var userId in _PasswordReset.UserIds)
                                             {

                                                 WriteToLogfileAndNotify(NotificationMessageType.Parse("resetPassword"),
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
                                                                         NoOwner,
                                                                         DefaultPasswordFile,
                                                                         Robot.Id);

                                                 _LoginPasswords.Remove(userId);

                                                 _LoginPasswords.Add(userId, new LoginPassword(userId, NewPassword));

                                                 Remove(_PasswordReset);

                                                 #region Send e-mail...

                                                 var MailSentResult = MailSentStatus.failed;
                                                 var user           = GetUser(userId);

                                                 var MailResultTask = APISMTPClient.Send(PasswordChangedEMailCreator(user.Id,
                                                                                                                     user.EMail,
                                                                                                                     user.Name,
                                                                                                                     "https://" + Request.Host.SimpleString,
                                                                                                                     DefaultLanguage));

                                                 if (MailResultTask.Wait(60000))
                                                     MailSentResult = MailResultTask.Result;

                                                 #endregion

                                             }

                                             return Task.FromResult(
                                                        new HTTPResponse.Builder(Request) {
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
                                                        }.AsImmutable);

                                             },

                                             AllowReplacement: URIReplacement.Allow);

            #endregion


            #region GET         ~/users

            // -------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/users
            // -------------------------------------------------------------------
            HTTPServer.ITEMS_GET(UriTemplate: URIPrefix + "users",
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
                                          HTTPURI.Parse("/users/{UserId}"),
                                          HTTPContentType.JSON_UTF8,
                                          HTTPRequestLogger:  AddUserRequest,
                                          HTTPResponseLogger: AddUserResponse,
                                          HTTPDelegate: Request => {

                                              #region Get HTTP user and its organizations

                                              // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                              if (!TryGetHTTPUser(Request,
                                                                  out User                       HTTPUser,
                                                                  out IEnumerable<Organization>  HTTPOrganizations,
                                                                  out HTTPResponse               ErrorResponse,
                                                                  AccessLevel:                   Access_Levels.ReadWrite,
                                                                  Recursive:                     true))
                                              {
                                                  return Task.FromResult(ErrorResponse);
                                              }

                                              #endregion

                                              #region Check UserId URI parameter

                                              if (!Request.ParseUserId(this,
                                                                       out User_Id? UserIdURI,
                                                                       out ErrorResponse))
                                              {
                                                  return Task.FromResult(ErrorResponse);
                                              }

                                              #endregion

                                              #region Parse JSON and create the new user...

                                              if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out ErrorResponse))
                                                  return Task.FromResult(ErrorResponse);

                                              #region Parse UserId           [optional]

                                              // Verify that a given user identification
                                              //   is at least valid.
                                              if (JSONObj.ParseOptionalN("@id",
                                                                         "user identification",
                                                                         HTTPServer.DefaultHTTPServerName,
                                                                         User_Id.TryParse,
                                                                         out User_Id? UserIdBody,
                                                                         Request,
                                                                         out ErrorResponse))
                                              {

                                                  if (ErrorResponse != null)
                                                      return Task.FromResult(ErrorResponse);

                                              }

                                              if (!UserIdURI.HasValue && !UserIdBody.HasValue)
                                              {

                                                  return Task.FromResult(
                                                             new HTTPResponse.Builder(Request) {
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
                                                             }.AsImmutable);

                                              }

                                              if (UserIdURI.HasValue && UserIdBody.HasValue && UserIdURI.Value != UserIdBody.Value)
                                              {

                                                  return Task.FromResult(
                                                             new HTTPResponse.Builder(Request) {
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
                                                             }.AsImmutable);

                                              }

                                              if (!User_Id.TryParse(Request.ParsedURIParameters[0], out User_Id UserId) ||
                                                   UserId.Length < MinLoginLenght)
                                              {

                                                  return Task.FromResult(
                                                             new HTTPResponse.Builder(Request) {
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
                                                             }.AsImmutable);

                                              }

                                              if (_Users.ContainsKey(UserId))
                                              {

                                                  return Task.FromResult(
                                                             new HTTPResponse.Builder(Request) {
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
                                                             }.AsImmutable);

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
                                                  return Task.FromResult(ErrorResponse);
                                              }

                                              if (Context != User.JSONLDContext)
                                              {

                                                  return Task.FromResult(
                                                             new HTTPResponse.Builder(Request) {
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
                                                             }.AsImmutable);

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
                                                  return Task.FromResult(ErrorResponse);
                                              }

                                              if (Name.IsNullOrEmpty() || Name.Length < 4)
                                              {

                                                  return Task.FromResult(
                                                             new HTTPResponse.Builder(Request) {
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
                                                             }.AsImmutable);

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
                                                  return Task.FromResult(ErrorResponse);
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

                                                  return Task.FromResult(
                                                             new HTTPResponse.Builder(Request) {
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
                                                             }.AsImmutable);

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

                                                  return Task.FromResult(
                                                             new HTTPResponse.Builder(Request) {
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
                                                             }.AsImmutable);

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
                                                  return Task.FromResult(ErrorResponse);
                                              }

                                              if (AccessLevel != "guest" && AccessLevel != "member" && AccessLevel != "admin")
                                              {

                                                  return Task.FromResult(
                                                             new HTTPResponse.Builder(Request) {
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
                                                             }.AsImmutable);

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
                                                      return Task.FromResult(ErrorResponse);

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
                                                      return Task.FromResult(ErrorResponse);

                                              }

                                              #endregion

                                              #region Parse Organization     [optional]

                                              Organization _Organization = null;

                                              // Verify that a given user identification
                                              //   is at least valid.
                                              if (JSONObj.ParseOptionalN("organization",
                                                                         "organization",
                                                                         HTTPServer.DefaultHTTPServerName,
                                                                         Organization_Id.TryParse,
                                                                         out Organization_Id? OrganizationId,
                                                                         Request,
                                                                         out ErrorResponse))
                                              {

                                                  if (ErrorResponse != null)
                                                      return Task.FromResult(ErrorResponse);

                                                  if (!_Organizations.TryGetValue(OrganizationId.Value, out _Organization))
                                                  {

                                                      return Task.FromResult(
                                                                 new HTTPResponse.Builder(Request) {
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
                                                                 }.AsImmutable);

                                                  }

                                              }

                                              #endregion

                                              PgpPublicKeyRing _PublicKeyRing = null;

                                              #region Parse password???

                                              Password? UserPassword = null;

                                              //if (!NewUserData.Contains("password"))
                                              //    return new HTTPResponse.Builder(Request) {
                                              //        HTTPStatusCode = HTTPStatusCode.BadRequest,
                                              //        Server = HTTPServer.DefaultServerName,
                                              //        ContentType = HTTPContentType.JSON_UTF8,
                                              //        Content = new JObject(
                                              //                               new JProperty("@context",     SignUpContext),
                                              //                               new JProperty("description",  "Missing \"password\" property!")
                                              //                          ).ToString().ToUTF8Bytes(),
                                              //        CacheControl = "public",
                                              //        Connection = "close"
                                              //    };

                                              //if (NewUserData.GetString("password").Length < MinPasswordLenght)
                                              //    return new HTTPResponse.Builder(Request) {
                                              //        HTTPStatusCode = HTTPStatusCode.BadRequest,
                                              //        Server = HTTPServer.DefaultServerName,
                                              //        ContentType = HTTPContentType.JSON_UTF8,
                                              //        Content = new JObject(
                                              //                               new JProperty("@context",     SignUpContext),
                                              //                               new JProperty("property",     "name"),
                                              //                               new JProperty("description",  "The password is too short!")
                                              //                          ).ToString().ToUTF8Bytes(),
                                              //        CacheControl = "public",
                                              //        Connection = "close"
                                              //    };

                                              #endregion

                                              #endregion


                                              var MailSentResult = MailSentStatus.failed;
                                              SMSApi.Api.Response.Status SMSSentResult = null;

                                              try
                                              {

                                                  var SetPasswordRequest  = ResetPassword(UserIdURI.Value,
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

                                                      var NewUser = CreateUser(Id:             UserId,
                                                                               Name:           Name,
                                                                               EMail:          UserEMail,
                                                                               MobilePhone:    MobilePhone,
                                                                               Description:    Description,
                                                                           //    Password:       Password.Parse(NewUserData.GetString("password")),
                                                                               PublicKeyRing:  _PublicKeyRing,
                                                                               CurrentUserId:  HTTPUser.Id);

                                                      if (_Organization != null)
                                                      {

                                                          switch (AccessLevel)
                                                          {

                                                              case "guest":
                                                                  AddToOrganization(NewUser, User2OrganizationEdges.IsGuest,   _Organization);
                                                                  break;

                                                              case "member":
                                                                  AddToOrganization(NewUser, User2OrganizationEdges.IsMember,  _Organization);
                                                                  break;

                                                              case "admin":
                                                                  AddToOrganization(NewUser, User2OrganizationEdges.IsAdmin,   _Organization);
                                                                  break;

                                                          }

                                                      }

                                                  }

                                                  else if (MailSentResult != MailSentStatus.ok)
                                                  {

                                                      return Task.FromResult(
                                                                 new HTTPResponse.Builder(Request) {
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
                                                                 }.AsImmutable);

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

                                                  return Task.FromResult(
                                                             new HTTPResponse.Builder(Request) {
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
                                                             }.AsImmutable);

                                              }

                                              return Task.FromResult(
                                                        new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode              = HTTPStatusCode.Created,
                                                            Server                      = HTTPServer.DefaultServerName,
                                                            Date                        = DateTime.UtcNow,
                                                            AccessControlAllowOrigin    = "*",
                                                            AccessControlAllowMethods   = "ADD, SET, GET",
                                                            AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                            Connection                  = "close"
                                                        }.AsImmutable);

                                         });

            #endregion

            #region EXISTS      ~/users/{UserId}

            // ---------------------------------------------------------------------------------
            // curl -v -X EXITS -H "Accept: application/json" http://127.0.0.1:2100/users/ahzf
            // ---------------------------------------------------------------------------------
            HTTPServer.ITEM_EXISTS<User_Id, User>(UriTemplate: URIPrefix + "users/{UserId}",
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
            HTTPServer.ITEM_GET<User_Id, User>(UriTemplate:         URIPrefix + "users/{UserId}",
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
            //                                         new HTTPResponse.Builder(Request) {
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
            //                                                  new HTTPResponse.Builder(Request) {
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
                                         URIPrefix + "users/{UserId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPRequestLogger:  SetUserRequest,
                                         HTTPResponseLogger: SetUserResponse,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out User                       HTTPUser,
                                                                 out IEnumerable<Organization>  HTTPOrganizations,
                                                                 out HTTPResponse               Response,
                                                                 AccessLevel:                   Access_Levels.ReadWrite,
                                                                 Recursive:                     true))
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

                                             #endregion

                                             #region Parse JSON

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out HTTPResponse))
                                                 return Task.FromResult(HTTPResponse);

                                             if (!User.TryParseJSON(JSONObj,
                                                                    out User    _User,
                                                                    out String  ErrorResponse,
                                                                    UserIdURI))
                                             {

                                                 return Task.FromResult(
                                                            new HTTPResponse.Builder(Request) {
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
                                                 return Task.FromResult(
                                                        new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                                            Server                      = HTTPServer.DefaultServerName,
                                                            Date                        = DateTime.UtcNow,
                                                            AccessControlAllowOrigin    = "*",
                                                            AccessControlAllowMethods   = "GET, SET, CHOWN",
                                                            AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                            Connection                  = "close"
                                                        }.AsImmutable);


                                             AddOrUpdate(_User,
                                                         HTTPUser.Id);


                                             return Task.FromResult(
                                                        new HTTPResponse.Builder(Request) {
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
                                                      new HTTPResponse.Builder(Request) {
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
                                              {

                                                  return Task.FromResult(
                                                      new HTTPResponse.Builder(Request) {
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

                                              }

                                              #endregion

                                              #region Verify realm

                                              if (LoginData.Contains("realm") &&
                                                  LoginData.GetString("realm").IsNotNullOrEmpty() &&
                                                  LoginData.GetString("realm").Length < MinRealmLenght)

                                              {

                                                  return Task.FromResult(
                                                      new HTTPResponse.Builder(Request) {
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

                                              }

                                              #endregion

                                              #region Verify password

                                              if (!LoginData.Contains("password"))
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
                                                                                new JProperty("description",  "Missing \"password\" property!")
                                                                           ).ToString().ToUTF8Bytes(),
                                                          CacheControl    = "private",
                                                          Connection      = "close"
                                                      }.AsImmutable);

                                              }

                                              if (LoginData.GetString("password").Length < MinPasswordLenght)
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
                                                                                new JProperty("description",  "The password is too short!")
                                                                           ).ToString().ToUTF8Bytes(),
                                                          CacheControl    = "private",
                                                          Connection      = "close"
                                                      }.AsImmutable);

                                              }

                                              #endregion

                                              #region Check login

                                              String _Realm = LoginData.GetString("realm");

                                              if (!(_Realm.IsNotNullOrEmpty()
                                                     ? User_Id.TryParse(LoginData.GetString("username"), _Realm, out User_Id _UserId)
                                                     : User_Id.TryParse(LoginData.GetString("username"),         out         _UserId)) ||
                                                  !_LoginPasswords.TryGetValue(_UserId, out LoginPassword _LoginPassword)              ||
                                                  !_Users.         TryGetValue(_UserId, out User          _User))
                                              {

                                                  return Task.FromResult(
                                                      new HTTPResponse.Builder(Request) {
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

                                              #endregion

                                              #region Check EULA

                                              var acceptsEULA = LoginData["acceptsEULA"].Value<Boolean>();

                                              if (!_User.AcceptedEULA.HasValue)
                                              {

                                                  if (!acceptsEULA)
                                                  {

                                                      return Task.FromResult(
                                                          new HTTPResponse.Builder(Request) {
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
                                                          }.AsImmutable);

                                                  }

                                                  lock (_Users)
                                                  {
                                                      var UUser = _User.ToBuilder();
                                                      UUser.AcceptedEULA = DateTime.UtcNow;
                                                      Update(UUser, _User.Id);
                                                  }

                                              }

                                              #endregion

                                              #region Check password

                                              if (!_LoginPassword.VerifyPassword(LoginData.GetString("password")))
                                              {

                                                  return Task.FromResult(
                                                      new HTTPResponse.Builder(Request) {
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
                                                  new HTTPResponse.Builder(Request) {
                                                      HTTPStatusCode  = HTTPStatusCode.Created,
                                                      ContentType     = HTTPContentType.TEXT_UTF8,
                                                      Content         = new JObject(
                                                                            new JProperty("@context",  SignInOutContext),
                                                                            new JProperty("login",     _LoginPassword.Login.ToString()),
                                                                            new JProperty("username",  _User.Name),
                                                                            new JProperty("email",     _User.EMail.Address.ToString())
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
                                         HTTPURI.Parse("/users/{UserId}"),
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get astronaut and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetAstronaut(Request,
                                                                  out User                       Astronaut,
                                                                  out IEnumerable<Organization>  AstronautOrganizations,
                                                                  out HTTPResponse               Response,
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

                                             if (!TryGetUser(UserIdURI.Value, out User UserURI))
                                             {

                                                 return Task.FromResult(
                                                            new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode              = HTTPStatusCode.NotFound,
                                                                Server                      = HTTPServer.DefaultServerName,
                                                                Date                        = DateTime.UtcNow,
                                                                AccessControlAllowOrigin    = "*",
                                                                AccessControlAllowMethods   = "IMPERSONATE",
                                                                AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                                Connection                  = "close"
                                                            }.AsImmutable);

                                             }

                                             #endregion


                                             #region Is the current user allowed to impersonate the given user?

                                             if (!CanImpersonate(Astronaut, UserURI))
                                                 return Task.FromResult(
                                                            new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                                                Server                      = HTTPServer.DefaultServerName,
                                                                Date                        = DateTime.UtcNow,
                                                                AccessControlAllowOrigin    = "*",
                                                                AccessControlAllowMethods   = "IMPERSONATE",
                                                                AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                                Connection                  = "close"
                                                            }.AsImmutable);


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

                                                 File.AppendAllText(DefaultHTTPCookiesFile,
                                                                    SecurityTokenId + ";" + UserURI.Id + ";" + Expires.ToIso8601() + ";" + Astronaut.Id + Environment.NewLine);

                                             }

                                             #endregion


                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
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
                                                                               (IsAdmin(Astronaut) ? ":isAdmin" : "") +
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

            #region DEPERSONATE ~/users/{UserId}

            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.DEPERSONATE,
                                         HTTPURI.Parse("/users/{UserId}"),
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get astronaut and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetAstronaut(Request,
                                                                  out User                       Astronaut,
                                                                  out IEnumerable<Organization>  AstronautOrganizations,
                                                                  out HTTPResponse               Response,
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

                                             if (!TryGetUser(UserIdURI.Value, out User UserURI))
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


                                             #region Register security token

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

                                                 File.AppendAllText(DefaultHTTPCookiesFile,
                                                                    SecurityTokenId + ";" + UserURI.Id + ";" + Expires.ToIso8601() + Environment.NewLine);

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
                                                                               (IsAdmin(Astronaut) ? ":isAdmin" : "") +
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
                                              URIPrefix + "users/{UserId}/profilephoto",
                                              URIParams => "LocalHTTPRoot/data/Users/" + URIParams[0] + ".png",
                                              DefaultFile: "HTTPRoot/images/defaults/DefaultUser.png");

            #endregion

            #region GET         ~/users/{UserId}/notifications

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/users/ahzf/notifications
            // --------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.GET,
                                         URIPrefix + "users/{UserId}/notifications",
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

                                             return Task.FromResult(new HTTPResponse.Builder(Request) {
                                                        HTTPStatusCode             = HTTPStatusCode.OK,
                                                        Server                     = HTTPServer.DefaultServerName,
                                                        Date                       = DateTime.UtcNow,
                                                        AccessControlAllowOrigin   = "*",
                                                        AccessControlAllowMethods  = "GET, SET",
                                                        AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                        ETag                       = "1",
                                                        ContentType                = HTTPContentType.JSON_UTF8,
                                                        Content                    = HTTPUser.GetNotificationInfos().ToUTF8Bytes(),
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
                                         URIPrefix + "users/{UserId}/notifications",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPRequestLogger:   SetUserNotificationsRequest,
                                         HTTPResponseLogger:  SetUserNotificationsResponse,
                                         HTTPDelegate:        Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out User                       HTTPUser,
                                                                 out IEnumerable<Organization>  HTTPOrganizations,
                                                                 out HTTPResponse               Response,
                                                                 AccessLevel:                   Access_Levels.ReadWrite,
                                                                 Recursive:                     true))
                                             {
                                                 return Task.FromResult(Response);
                                             }

                                             #endregion

                                             #region Check UserId URI parameter

                                             if (!Request.ParseUser(this,
                                                                    out User_Id?      UserIdURI,
                                                                    out User          User,
                                                                    out HTTPResponse  HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse);
                                             }

                                             #endregion


                                             #region Parse JSON

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out HTTPResponse))
                                                 return Task.FromResult(HTTPResponse);

                                             //if (!User.TryParseJSON(JSONObj,
                                             //                       out User    _User,
                                             //                       out String  ErrorResponse,
                                             //                       UserIdURI))
                                             //{

                                             //    return SetUserResponse(
                                             //               new HTTPResponse.Builder(Request) {
                                             //                   HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                             //                   Server                     = HTTPServer.DefaultServerName,
                                             //                   Date                       = DateTime.UtcNow,
                                             //                   AccessControlAllowOrigin   = "*",
                                             //                   AccessControlAllowMethods  = "GET, SET",
                                             //                   AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                             //                   ETag                       = "1",
                                             //                   ContentType                = HTTPContentType.JSON_UTF8,
                                             //                   Content                    = JSONObject.Create(
                                             //                                                    new JProperty("description",  ErrorResponse)
                                             //                                                ).ToUTF8Bytes()
                                             //               }.AsImmutable);

                                             //}

                                             #endregion


                                             //// Has the current HTTP user the required
                                             //// access rights to update?
                                             //if (HTTPUser.Id != _User.Id)
                                             //    return SetUserResponse(
                                             //           new HTTPResponse.Builder(Request) {
                                             //               HTTPStatusCode              = HTTPStatusCode.Forbidden,
                                             //               Server                      = HTTPServer.DefaultServerName,
                                             //               Date                        = DateTime.UtcNow,
                                             //               AccessControlAllowOrigin    = "*",
                                             //               AccessControlAllowMethods   = "GET, SET, CHOWN",
                                             //               AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                             //               Connection                  = "close"
                                             //           }.AsImmutable);


                                             //AddOrUpdate(_User,
                                             //            HTTPUser.Id);


                                             return Task.FromResult(
                                                        new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode              = HTTPStatusCode.OK,
                                                            Server                      = HTTPServer.DefaultServerName,
                                                            Date                        = DateTime.UtcNow,
                                                            AccessControlAllowOrigin    = "*",
                                                            AccessControlAllowMethods   = "GET, SET",
                                                            AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                            //ETag                        = _User.CurrentCryptoHash,
                                                            ContentType                 = HTTPContentType.JSON_UTF8,
                                                            Content                     = JSONObj.ToString().ToUTF8Bytes(),
                                                            Connection                  = "close"
                                                        }.AsImmutable);

                                         });

            #endregion

            #region GET         ~/users/{UserId}/organizations

            // ------------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2000/users/{UserId}/organizations
            // ------------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.GET,
                                         URIPrefix + "users/{UserId}/organizations",
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
                                         URIPrefix + "users/{UserId}/APIKeys",
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
                                         URIPrefix + "users/{UserId}/password",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPRequestLogger:  ChangePasswordRequest,
                                         HTTPResponseLogger: ChangePasswordResponse,
                                         HTTPDelegate: async Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out User                       HTTPUser,
                                                                 out IEnumerable<Organization>  HTTPOrganizations,
                                                                 out HTTPResponse               Response,
                                                                 AccessLevel:                   Access_Levels.ReadWrite,
                                                                 Recursive:                     true))
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
                                                                   HTTPUser.Id))
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


            #region GET         ~/groups

            // ------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2100/groups
            // ------------------------------------------------------------------
            HTTPServer.ITEMS_GET(UriTemplate: URIPrefix + "groups",
                                 Dictionary: _Groups,
                                 Filter: group => group.PrivacyLevel == PrivacyLevel.World,
                                 ToJSONDelegate: JSON_IO.ToJSON);

            #endregion

            #region EXISTS      ~/groups/{GroupId}

            // -------------------------------------------------------------------------------------------
            // curl -v -X EXITS -H "Accept: application/json" http://127.0.0.1:2100/groups/OK-Lab%20Jena
            // -------------------------------------------------------------------------------------------
            HTTPServer.ITEM_EXISTS<Group_Id, Group>(UriTemplate: URIPrefix + "groups/{GroupId}",
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
            HTTPServer.ITEM_GET<Group_Id, Group>(UriTemplate:         URIPrefix + "groups/{GroupId}",
                                                 ParseIdDelegate:     Group_Id.TryParse,
                                                 ParseIdError:        Text => "Invalid group identification '" + Text + "'!",
                                                 TryGetItemDelegate:  _Groups.TryGetValue,
                                                 ItemFilterDelegate:  group => group.PrivacyLevel == PrivacyLevel.World,
                                                 TryGetItemError:     groupId => "Unknown group '" + groupId + "'!",
                                                 ToJSONDelegate:      _ => _.ToJSON());

            #endregion


            #region GET         ~/organizations

            #region GET         ~/organizations

            // ---------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2000/organizations
            // ---------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.GET,
                                         URIPrefix + "organizations",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Try to get HTTP user and its organizations

                                             TryGetHTTPUser(Request,
                                                            out User                       HTTPUser,
                                                            out IEnumerable<Organization>  HTTPOrganizations,
                                                            out HTTPResponse               Response,
                                                            Recursive: true);

                                             #endregion

                                             var skip                    = Request.QueryString.GetUInt64 ("skip");
                                             var take                    = Request.QueryString.GetUInt64 ("take");

                                             var includeCryptoHash       = Request.QueryString.GetBoolean("includeCryptoHash", true);

                                             var expand                  = Request.QueryString.GetStrings("expand", true);
                                             var expandTags              = expand.Contains("tags")         ? InfoStatus.Expand : InfoStatus.ShowIdOnly;


                                             //ToDo: Getting the expected total count might be very expensive!
                                             var _ExpectedCount = Organizations.ULongCount();

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
                                                     Content                    = Organizations.
                                                                                        OrderBy(organization => organization.Id).
                                                                                        //Where  (organization => HTTPOrganizations.Contains(organization.Owner) ||
                                                                                        //                            Admins.InEdges(HTTPUser).
                                                                                        //                                   Any(edgelabel => edgelabel == User2GroupEdges.IsAdmin)).
                                                                                        ToJSON(skip,
                                                                                               take,
                                                                                               false, //Embedded
                                                                                               expandTags,
                                                                                               GetOrganizationSerializator(HTTPUser),
                                                                                               includeCryptoHash).
                                                                                        ToUTF8Bytes(),
                                                     X_ExpectedTotalNumberOfItems = _ExpectedCount,
                                                     Connection = "close"
                                                 }.AsImmutable);

                                         });

            #endregion

            #region COUNT       ~/organizations

            // ------------------------------------------------------------------------------------
            // curl -v -X COUNT -H "Accept: application/json" http://127.0.0.1:2000/organizations
            // ------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.COUNT,
                                         URIPrefix + "organizations",
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


            #region GET         ~/organizations/{OrganizationId}

            // -------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:2000/organizations/214080158
            // -------------------------------------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.GET,
                                         URIPrefix + "organizations/{OrganizationId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Try to get HTTP user and its organizations

                                             TryGetHTTPUser(Request,
                                                            out User                       HTTPUser,
                                                            out IEnumerable<Organization>  HTTPOrganizations,
                                                            out HTTPResponse               Response,
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
                                                        (Organization.PrivacyLevel == PrivacyLevel.Private && !HTTPOrganizations.Contains(Organization))

                                                            ? new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.Unauthorized,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = DateTime.UtcNow,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET",
                                                                  AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                  Connection                 = "close"
                                                              }.AsImmutable

                                                            : new HTTPResponse.Builder(Request) {
                                                                  HTTPStatusCode             = HTTPStatusCode.OK,
                                                                  Server                     = HTTPServer.DefaultServerName,
                                                                  Date                       = DateTime.UtcNow,
                                                                  AccessControlAllowOrigin   = "*",
                                                                  AccessControlAllowMethods  = "GET",
                                                                  AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                                  ContentType                = HTTPContentType.JSON_UTF8,
                                                                  Content                    = Organization.ToJSON().ToUTF8Bytes(),
                                                                  Connection                 = "close"
                                                              }.AsImmutable);

                                         });

            #endregion

            #region EXISTS      ~/organizations/{OrganizationId}

            // ---------------------------------------------------------
            // curl -v -X EXISTS http://127.0.0.1:2000/organizations/7
            // ---------------------------------------------------------
            HTTPServer.AddMethodCallback(Hostname,
                                         HTTPMethod.EXISTS,
                                         URIPrefix + "organizations/{OrganizationId}",
                                         HTTPDelegate: Request => {

                                             #region Try to get HTTP user and its organizations

                                             TryGetHTTPUser(Request,
                                                            out User                       HTTPUser,
                                                            out IEnumerable<Organization>  HTTPOrganizations,
                                                            out HTTPResponse               Response,
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

            #endregion

            #region EXISTS      ~/orgs/{OrgId}

            // ------------------------------------------------------------------------------------
            // curl -v -X EXITS -H "Accept: application/json" http://127.0.0.1:2100/orgs/Stadtrat
            // ------------------------------------------------------------------------------------
            HTTPServer.ITEM_EXISTS<Organization_Id, Organization>(UriTemplate:               URIPrefix + "organizations/{OrgId}",
                                                                  ParseIdDelegate:           Organization_Id.TryParse,
                                                                  ParseIdError:              Text  => "Invalid organization identification '" + Text + "'!",
                                                                  TryGetItemDelegate:        _Organizations.TryGetValue,
                                                                  ItemFilterDelegate:        org   => org.PrivacyLevel == PrivacyLevel.World,
                                                                  TryGetItemError:           orgId => "Unknown organization '" + orgId + "'!");

            #endregion

            #region Add         ~/organizations/{organizationId}

            // ---------------------------------------------------------------------------------------------
            // curl -v -X Add \
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
                                         URIPrefix + "organizations/{organizationId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPRequestLogger:  AddOrganizationRequest,
                                         HTTPResponseLogger: AddOrganizationResponse,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out User                       HTTPUser,
                                                                 out IEnumerable<Organization>  HTTPOrganizations,
                                                                 out HTTPResponse               Response,
                                                                 AccessLevel:                   Access_Levels.ReadWrite,
                                                                 Recursive:                     true))
                                             {
                                                 return Task.FromResult(Response);
                                             }

                                             #endregion

                                             #region Check OrganizationId URI parameter

                                             if (!Request.ParseOrganizationId(this,
                                                                              out Organization_Id?  OrganizationIdURI,
                                                                              out HTTPResponse      HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse);
                                             }

                                             #endregion

                                             #region Parse JSON and create the new child organization...

                                             if (!Request.TryParseJObjectRequestBody(out JObject JSONObj, out HTTPResponse))
                                                 return Task.FromResult(HTTPResponse);

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
                                                     return Task.FromResult(HTTPResponse);
                                                 }

                                                 if (!_Organizations.TryGetValue(ParentOrganizationId, out Organization ParentOrganization))
                                                 {

                                                     return Task.FromResult(
                                                                new HTTPResponse.Builder(Request) {
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
                                                                }.AsImmutable);

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
                                                     return Task.FromResult(HTTPResponse);
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

                                                         return Task.FromResult(
                                                                    new HTTPResponse.Builder(Request) {
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
                                                                    }.AsImmutable);

                                                     }

                                                     Admin = GetUser(admin.Value);

                                                     if (Admin == null)
                                                     {

                                                         return Task.FromResult(
                                                                    new HTTPResponse.Builder(Request) {
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
                                                                    }.AsImmutable);

                                                     }

                                                     Admins.Add(Admin);

                                                 }

                                                 #endregion


                                                 try
                                                 {

                                                     var _NewChildOrganization = CreateOrganization(NewChildOrganization.Id,
                                                                                                    NewChildOrganization.Name,
                                                                                                    NewChildOrganization.Description,
                                                                                                    ParentOrganization:  ParentOrganization,
                                                                                                    CurrentUserId:       HTTPUser.Id);

                                                     foreach (var admin in Admins)
                                                         AddToOrganization(admin, User2OrganizationEdges.IsAdmin, _NewChildOrganization);

                                                 }
                                                 catch (Exception e)
                                                 {

                                                     return Task.FromResult(
                                                                new HTTPResponse.Builder(Request) {
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
                                                                }.AsImmutable);

                                                 }

                                                 return Task.FromResult(
                                                        new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode              = HTTPStatusCode.Created,
                                                            Server                      = HTTPServer.DefaultServerName,
                                                            Date                        = DateTime.UtcNow,
                                                            AccessControlAllowOrigin    = "*",
                                                            AccessControlAllowMethods   = "GET, SET",
                                                            AccessControlAllowHeaders   = "Content-Type, Accept, Authorization",
                                                            Connection                  = "close"
                                                        }.AsImmutable);

                                             }

                                             #endregion

                                             return Task.FromResult(
                                                        new HTTPResponse.Builder(Request) {
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
                                                        }.AsImmutable);

                                         });

            #endregion


            #region ~/tags

            #endregion

        }

        #endregion

        #region (private) ReadDatabaseFiles()

        private void ReadDatabaseFiles()
        {

            DebugX.Log("Reading all UsersAPI database files...");

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
                                CurrentDatabaseHashValue  =  JSONLine["sha256hash"]?["hashValue"]?.Value<String>();

                                if (JSONCommand.IsNotNullOrEmpty() && JSONObject != null)
                                    ProcessCommand(JSONCommand, JSONObject);

                            }
                            catch (Exception e)
                            {
                                DebugX.Log("Could not read database file '" + DefaultUsersAPIFile + "' line " + linenumber + ": " + e.Message);
                            }

                        }

                    });

                }

            }
            catch (Exception e)
            {
                DebugX.LogT("ReadDatabaseFiles() -> DefaultUsersAPIFile failed: " + e.Message);
            }

            #endregion

            #region Read Password file...

            try
            {

                if (File.Exists(DefaultPasswordFile))
                {

                    JObject JSONLine;
                    String  JSONCommand;
                    JObject JSONObject;

                    File.ReadLines(DefaultPasswordFile).ForEachCounted((line, linenumber) => {

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
                                                DebugX.Log("Invalid 'AddPassword' command in '" + DefaultPasswordFile + "' line " + linenumber + "!");

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
                                                DebugX.Log("Invalid 'ChangePassword' command in '" + DefaultPasswordFile + "' line " + linenumber + "!");

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
                                            DebugX.Log("Unknown command '" + JSONCommand + "' in password file '" + DefaultPasswordFile + "' line " + linenumber + "!");
                                            break;

                                    }

                                }

                                else
                                    DebugX.Log("Could not read password file '" + DefaultPasswordFile + "' line " + linenumber + "!");

                            }
                            catch (Exception e)
                            {
                                DebugX.Log("Could not read password file '" + DefaultPasswordFile + "' line " + linenumber + ": " + e.Message);
                            }

                        }

                    });

                }

            }
            catch (Exception e)
            {
                DebugX.LogT("ReadDatabaseFiles() -> DefaultPasswordFile failed: " + e.Message);
            }

            #endregion

            #region Read HTTPCookiesFile file...

            if (File.Exists(DefaultHTTPCookiesFile))
            {

                lock (HTTPCookies)
                {

                    try
                    {

                        File.ReadLines(DefaultHTTPCookiesFile).ForEachCounted((line, linenumber) => {

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
                                DebugX.Log("Could not read HTTP cookies file '" + DefaultHTTPCookiesFile + "' line " + linenumber + ": " + e.Message);
                            }

                        });

                    }
                    catch (Exception e)
                    {
                        DebugX.Log("Could not read HTTP cookies file '" + DefaultHTTPCookiesFile + "': " + e.Message);
                    }


                    // Write filtered (no invalid users, no expired tokens) tokens back to file...
                    try
                    {

                        File.WriteAllLines(DefaultHTTPCookiesFile,
                                           HTTPCookies.Select(token => token.Key + ";" + token.Value.ToLogLine()));

                    }
                    catch (Exception e)
                    {
                        DebugX.Log("Could not update HTTP cookies file '" + DefaultHTTPCookiesFile + "': " + e.Message);
                    }

                }

            }

            #endregion

            #region Read PasswordResets file...

            try
            {

                if (File.Exists(DefaultPasswordResetsFile))
                {

                    JObject  JSONLine;
                    String   JSONCommand;
                    JObject  JSONObject;

                    var Now     = DateTime.UtcNow;
                    var MaxAge  = TimeSpan.FromDays(7);

                    File.ReadLines(DefaultPasswordResetsFile).ForEachCounted((line, linenumber) => {

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
                                                    DebugX.Log("Invalid 'Add' command in '" + DefaultPasswordResetsFile + "' line " + linenumber + "!");

                                                break;

                                            #endregion

                                            #region Remove

                                            case "remove":
                                                PasswordResets.Remove(_PasswordReset.SecurityToken1);
                                                break;

                                            #endregion

                                            default:
                                                DebugX.Log("Unknown command '" + JSONCommand + "' in password file '" + DefaultPasswordResetsFile + "' line " + linenumber + "!");
                                                break;

                                        }

                                    }

                                }

                                else
                                    DebugX.Log("Could not read password file '" + DefaultPasswordResetsFile + "' line " + linenumber + "!");

                            }
                            catch (Exception e)
                            {
                                DebugX.Log("Could not read password file '" + DefaultPasswordResetsFile + "' line " + linenumber + ": " + e.Message);
                            }

                        }

                    });

                }

            }
            catch (Exception e)
            {
                DebugX.LogT("ReadDatabaseFiles() -> DefaultPasswordResetsFile failed: " + e.Message);
            }

            #endregion

            DebugX.Log("Read all UsersAPI database files...");

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

                case "createUser":
                case "CreateUser":

                    if (User.TryParseJSON(JSONObject,
                                          out User    NewUser,
                                          out String  ErrorResponse))
                    {
                        _Users.AddAndReturnValue(NewUser.Id, NewUser);
                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", ErrorResponse));

                    break;

                #endregion

                #region AddOrUpdateUser

                case "addOrUpdateUser":
                case "AddOrUpdateUser":

                    if (User.TryParseJSON(JSONObject,
                                          out NewUser,
                                          out ErrorResponse))
                    {

                        if (_Users.TryGetValue(NewUser.Id, out User OldUser))
                        {
                            _Users.Remove(OldUser.Id);
                            OldUser.CopyAllEdgesTo(NewUser);
                        }

                        _Users.Add(NewUser.Id, NewUser);

                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", ErrorResponse));

                    break;

                #endregion

                #region UpdateUser

                case "updateUser":
                case "UpdateUser":

                    if (User.TryParseJSON(JSONObject,
                                          out NewUser,
                                          out ErrorResponse))
                    {

                        if (_Users.TryGetValue(NewUser.Id, out User OldUser))
                        {

                            _Users.Remove(OldUser.Id);
                            NewUser.API = this;
                            OldUser.CopyAllEdgesTo(NewUser);

                            _Users.Add(NewUser.Id, NewUser);

                        }

                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", ErrorResponse));

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

                #region AddIfNotExistsOrganization

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

                #region AddIfNotExistsOrganization

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
                            //OldOrganization.CopyAllEdgesTo(_Organization);
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
                            //OldOrganization.CopyAllEdgesTo(_Organization);

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
                        _Organizations.Remove(OrganizationId);
                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", ErrorResponse));

                    break;

                #endregion


                #region LinkOrganizations

                case "linkOrganizations":
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

                case "addUserToGroup":
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

                case "addUserToOrganization":
                case "AddUserToOrganization":

                    if (!User_Id.TryParse(JSONObject["user"]?.Value<String>(), out User_Id U2O_UserId))
                    {
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, ": ", "Invalid user identification '" + JSONObject["user"]?.Value<String>() + "'!"));
                        break;
                    }

                    if (!TryGetUser(U2O_UserId, out User U2O_User))
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

                    var U2O_Edge     = (User2OrganizationEdges) Enum.Parse(typeof(User2OrganizationEdges), JSONObject["edge"].Value<String>());
                    var U2O_Privacy  = JSONObject.ParseMandatory_PrivacyLevel();

                    if (!U2O_User.Edges(U2O_Organization).Any(edgelabel => edgelabel == U2O_Edge))
                        U2O_User.AddOutgoingEdge(U2O_Edge, U2O_Organization, U2O_Privacy);

                    if (!U2O_Organization.User2OrganizationInEdgeLabels(U2O_User).Any(edgelabel => edgelabel == U2O_Edge))
                        U2O_Organization.LinkUser(U2O_User, U2O_Edge, U2O_Privacy);

                    break;

                #endregion


                #region AddNotification

                case "addNotification":
                case "AddNotification":

                    if (JSONObject["userId"]?.Value<String>().IsNotNullOrEmpty() == true &&
                        JSONObject["type"  ]?.Value<String>().IsNotNullOrEmpty() == true)
                    {

                        if (User_Id.TryParse(JSONObject["userId"]?.Value<String>(), out User_Id UserId) &&
                            TryGetUser(UserId, out User User))
                        {

                            if (JSONObject["notificationId"]?.Value<String>().IsNotNullOrEmpty() == true)
                            {
                                switch (JSONObject["type"]?.Value<String>())
                                {

                                    case "EMailNotification":
                                        AddNotification(User,
                                                        EMailNotification.Parse(JSONObject),
                                                        NotificationMessageType.Parse(JSONObject["notificationId"]?.Value<String>()));
                                        break;

                                    case "SMSNotification":
                                        AddNotification(User,
                                                        SMSNotification.Parse(JSONObject),
                                                        NotificationMessageType.Parse(JSONObject["notificationId"]?.Value<String>()));
                                        break;

                                    case "HTTPSNotification":
                                        AddNotification(User,
                                                        HTTPSNotification.Parse(JSONObject),
                                                        NotificationMessageType.Parse(JSONObject["notificationId"]?.Value<String>()));
                                        break;

                                }
                            }

                            else
                            {
                                switch (JSONObject["type"]?.Value<String>())
                                {

                                    case "EMailNotification":
                                        AddNotification(User,
                                                        EMailNotification.Parse(JSONObject));
                                        break;

                                    case "SMSNotification":
                                        AddNotification(User,
                                                        SMSNotification.Parse(JSONObject));
                                        break;

                                    case "HTTPSNotification":
                                        AddNotification(User,
                                                        HTTPSNotification.Parse(JSONObject));
                                        break;

                                }
                            }

                        }

                    }

                    break;

                #endregion

                #region RemoveNotification

                case "RemoveNotification":

                    //if (JSONObject["userId"]?.Value<String>().IsNotNullOrEmpty() == true &&
                    //    JSONObject["type"  ]?.Value<String>().IsNotNullOrEmpty() == true)
                    //{

                    //    if (User_Id.TryParse(JSONObject["userId"]?.Value<String>(), out User_Id UserId) &&
                    //        TryGetUser(UserId, out User User))
                    //    {

                    //        if (JSONObject["notificationId"]?.Value<String>().IsNotNullOrEmpty() == true)
                    //        {
                    //            switch (JSONObject["type"]?.Value<String>())
                    //            {

                    //                case "EMailNotification":
                    //                    UnregisterNotification<EMailNotification>(UserId,
                    //                                                              NotificationMessageType.Parse(JSONObject["notificationId"]?.Value<String>()),
                    //                                                              a => a.EMailAddress == EMailNotification.Parse(JSONObject).EMailAddress);
                    //                    break;

                    //                case "SMSNotification":
                    //                    UnregisterNotification<SMSNotification>  (UserId,
                    //                                                              NotificationMessageType.Parse(JSONObject["notificationId"]?.Value<String>()),
                    //                                                              a => a.Phonenumber  == SMSNotification.  Parse(JSONObject).Phonenumber);
                    //                    break;

                    //            }
                    //        }

                    //        else
                    //        {
                    //            switch (JSONObject["type"]?.Value<String>())
                    //            {

                    //                case "EMailNotification":
                    //                    UnregisterNotification<EMailNotification>(UserId,
                    //                                                              a => a.EMailAddress == EMailNotification.Parse(JSONObject).EMailAddress);
                    //                    break;

                    //                case "SMSNotification":
                    //                    UnregisterNotification<SMSNotification>  (UserId,
                    //                                                              a => a.Phonenumber  == SMSNotification.  Parse(JSONObject).Phonenumber);
                    //                    break;

                    //            }
                    //        }

                    //    }

                    //}

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
                TryGetUser(SecurityInformation.Astronaut ?? SecurityInformation.UserId, out User))
            {
                return true;
            }

            #endregion

            User = null;
            return false;

        }

        #endregion

        #region (protected) TryGetHTTPUser (Request, User, Organizations, Response, AccessLevel = ReadOnly, Recursive = false)

        protected Boolean TryGetHTTPUser(HTTPRequest                    Request,
                                         out User                       User,
                                         out IEnumerable<Organization>  Organizations,
                                         out HTTPResponse               Response,
                                         Access_Levels                  AccessLevel  = Access_Levels.ReadOnly,
                                         Boolean                        Recursive    = false)
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

                Organizations  = new Organization[0];
                Response       = new HTTPResponse.Builder(Request) {
                                     HTTPStatusCode  = HTTPStatusCode.Unauthorized,
                                     Location        = URIPrefix + "login",
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

        #region (protected) TryGetAstronaut(Request, User, Organizations, Response, AccessLevel = ReadOnly, Recursive = false)

        protected Boolean TryGetAstronaut(HTTPRequest                    Request,
                                          out User                       User,
                                          out IEnumerable<Organization>  Organizations,
                                          out HTTPResponse               Response,
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
                                     Location        = URIPrefix + "login",
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

        protected void TryGetHTTPUser(HTTPRequest                    Request,
                                      out User                       User,
                                      out IEnumerable<Organization>  Organizations,
                                      Access_Levels                  AccessLevel  = Access_Levels.ReadOnly,
                                      Boolean                        Recursive    = false)
        {

            if (!TryGetHTTPUser(Request, out User))
            {

                if (Request.HTTPSource.IPAddress.IsIPv4 &&
                    Request.HTTPSource.IPAddress.IsLocalhost)
                {
                    User           = Admins.User2GroupInEdges(edgelabel => edgelabel == User2GroupEdges.IsAdmin).FirstOrDefault()?.Source;
                    Organizations  = User.Organizations(AccessLevel, Recursive);
                    return;
                }

                Organizations  = null;
                return;

            }

            Organizations = User?.Organizations(AccessLevel, Recursive);

        }

        #endregion


        #region WriteToLogfileAndNotify(MessageType, JSONData, Logfilename = DefaultUsersAPIFile)

        public void WriteToLogfileAndNotify(NotificationMessageType  MessageType,
                                            JObject                  JSONData,
                                            Organization             Owner,
                                            User_Id?                 CurrentUserId)
        {

            WriteToLogfileAndNotify(MessageType,
                                    JSONData,
                                    Owner != null ? new Organization[] { Owner } : new Organization[0],
                                    DefaultUsersAPIFile,
                                    CurrentUserId);

        }

        public void WriteToLogfileAndNotify(NotificationMessageType    MessageType,
                                            JObject                    JSONData,
                                            IEnumerable<Organization>  Owners,
                                            User_Id?                   CurrentUserId)
        {

            WriteToLogfileAndNotify(MessageType,
                                    JSONData,
                                    Owners,
                                    DefaultUsersAPIFile,
                                    CurrentUserId);

        }

        public void WriteToLogfileAndNotify(NotificationMessageType  MessageType,
                                            JObject                  JSONData,
                                            Organization             Owner,
                                            String                   Logfilename    = DefaultUsersAPIFile,
                                            User_Id?                 CurrentUserId  = null)

            => WriteToLogfileAndNotify(MessageType,
                                       JSONData,
                                       Owner != null ? new Organization[] { Owner } : new Organization[0],
                                       Logfilename,
                                       CurrentUserId);

        public void WriteToLogfileAndNotify(NotificationMessageType    MessageType,
                                            JObject                    JSONData,
                                            IEnumerable<Organization>  Owners,
                                            String                     Logfilename    = DefaultUsersAPIFile,
                                            User_Id?                   CurrentUserId  = null)
        {

            if (!DisableLogfile || !DisableNotifications)
            {
                lock (DefaultUsersAPIFile)
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
                    CurrentDatabaseHashValue  = SHA256.ComputeHash(Encoding.Unicode.GetBytes(JSONWhitespaceRegEx.Replace(JSONMessage.ToString(), " "))).
                                                       Select(value => String.Format("{0:x2}", value)).
                                                       Aggregate();

                    (JSONMessage["sha256hash"] as JObject)?.Add(new JProperty("hashValue",  CurrentDatabaseHashValue));

                    #region Write to logfile

                    if (!DisableLogfile)
                    {

                        var retry = false;

                        do
                        {

                            try
                            {

                                File.AppendAllText(Logfilename,
                                                   JSONWhitespaceRegEx.Replace(JSONMessage.ToString(), " ") +
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

                        } while (retry);

                    }

                    #endregion

                    #region Send notifications

                    if (!DisableNotifications)
                    {
                        lock (_NotificationMessages)
                        {
                            _NotificationMessages.Enqueue(new NotificationMessage(Now,
                                                                                  MessageType,
                                                                                  JSONMessage,
                                                                                  Owners));
                        }
                    }

                    #endregion

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

                    var retry = false;

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


        //public void WriteToLogfileAndNotify(NotificationMessageType    MessageType,
        //                                    JObject                    JSONData,
        //                                    IEnumerable<Organization>  Owners,
        //                                    String                     Logfilename    = DefaultUsersAPIFile,
        //                                    User_Id?                   CurrentUserId  = null)
        //{

        //    if (!DisableNotifications)
        //    {
        //        lock (_NotificationMessages)
        //        {

        //            _NotificationMessages.Enqueue(new NotificationMessage(DateTime.UtcNow,
        //                                                                  MessageType,
        //                                                                  JSONMessage,
        //                                                                  Owners));

        //        }
        //    }

        //}


        #region AddEventSource(HTTPEventSourceId, URITemplate, IncludeFilterAtRuntime, CreateState, ...)

        public void AddEventSource<TState>(HTTPEventSource_Id                      HTTPEventSourceId,
                                           HTTPURI                                 URITemplate,

                                           Func<TState, User, HTTPEvent, Boolean>  IncludeFilterAtRuntime,
                                           Func<TState>                            CreateState,

                                           HTTPHostname?                           Hostname                   = null,
                                           HTTPMethod?                             HttpMethod                 = null,
                                           HTTPContentType                         HTTPContentType            = null,

                                           HTTPAuthentication                      URIAuthentication          = null,
                                           HTTPAuthentication                      HTTPMethodAuthentication   = null,

                                           HTTPDelegate                            DefaultErrorHandler        = null)
        {

            if (IncludeFilterAtRuntime == null)
                IncludeFilterAtRuntime = (s, u, e) => true;

            if (TryGet(HTTPEventSourceId, out IHTTPEventSource _EventSource))
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
                                                                     out User                       HTTPUser,
                                                                     out IEnumerable<Organization>  HTTPOrganizations,
                                                                     out HTTPResponse               Response,
                                                                     AccessLevel:                   Access_Levels.ReadWrite,
                                                                     Recursive:                     true))
                                                 {
                                                     return Task.FromResult(Response);
                                                 }

                                                 #endregion

                                                 var State        = CreateState != null ? CreateState() : default(TState);
                                                 var _HTTPEvents  = _EventSource.GetAllEventsGreater(Request.GetHeaderField_UInt64("Last-Event-ID")).
                                                                                 Where (_event => IncludeFilterAtRuntime(State,
                                                                                                                         HTTPUser,
                                                                                                                         _event)).
                                                                                 Select(_event => _event.ToString()).
                                                                                 AggregateWith(Environment.NewLine) +
                                                                                 Environment.NewLine;

                                        //             _ResourceContent += Environment.NewLine + "retry: " + ((UInt32)_EventSource.RetryIntervall.TotalMilliseconds) + Environment.NewLine + Environment.NewLine;


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

            }

            else
                throw new ArgumentException("Event source '" + HTTPEventSourceId + "' could not be found!", nameof(HTTPEventSourceId));

        }

        public void AddEventSource<TState>(HTTPEventSource_Id                                                 HTTPEventSourceId,
                                           HTTPURI                                                            URITemplate,

                                           Func<TState, User, IEnumerable<Organization>, HTTPEvent, Boolean>  IncludeFilterAtRuntime,
                                           Func<TState>                                                       CreateState,

                                           HTTPHostname?                                                      Hostname                   = null,
                                           HTTPMethod?                                                        HttpMethod                 = null,
                                           HTTPContentType                                                    HTTPContentType            = null,

                                           HTTPAuthentication                                                 URIAuthentication          = null,
                                           HTTPAuthentication                                                 HTTPMethodAuthentication   = null,

                                           HTTPDelegate                                                       DefaultErrorHandler        = null)
        {

            if (IncludeFilterAtRuntime == null)
                IncludeFilterAtRuntime = (s, u, o, e) => true;

            if (TryGet(HTTPEventSourceId, out IHTTPEventSource _EventSource))
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
                                                                     out User                       HTTPUser,
                                                                     out IEnumerable<Organization>  HTTPOrganizations,
                                                                     out HTTPResponse               Response,
                                                                     AccessLevel:                   Access_Levels.ReadWrite,
                                                                     Recursive:                     true))
                                                 {
                                                     return Task.FromResult(Response);
                                                 }

                                                 #endregion

                                                 var State        = CreateState != null ? CreateState() : default(TState);
                                                 var _HTTPEvents  = _EventSource.GetAllEventsGreater(Request.GetHeaderField_UInt64("Last-Event-ID")).
                                                                        Where (_event => IncludeFilterAtRuntime(State,
                                                                                                                HTTPUser,
                                                                                                                HTTPOrganizations,
                                                                                                                _event)).
                                                                        Select(_event => _event.ToString()).
                                                                        AggregateWith(Environment.NewLine) +
                                                                        Environment.NewLine;

                                        //             _ResourceContent += Environment.NewLine + "retry: " + ((UInt32)_EventSource.RetryIntervall.TotalMilliseconds) + Environment.NewLine + Environment.NewLine;


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

            }

            else
                throw new ArgumentException("Event source '" + HTTPEventSourceId + "' could not be found!", nameof(HTTPEventSourceId));

        }

        #endregion


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

                WriteToLogfileAndNotify(NotificationMessageType.Parse("createDataLicense"),
                                        DataLicense.ToJSON(),
                                        NoOwner,
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

        #region Users

        #region Users

        protected readonly Dictionary<User_Id, User> _Users;

        public IEnumerable<User> Users
            => _Users.Values;

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
        /// <param name="GeoLocation">An optional geographical location of the user.</param>
        /// <param name="Address">An optional address of the user.</param>
        /// <param name="PrivacyLevel">Whether the user will be shown in user listings, or not.</param>
        /// <param name="AcceptedEULA">Timestamp when the user accepted the End-User-License-Agreement.</param>
        /// <param name="IsAuthenticated">The user will not be shown in user listings, as its primary e-mail address is not yet authenticated.</param>
        /// <param name="IsDisabled">The user will be shown in user listings.</param>
        public User CreateUser(User_Id             Id,
                               SimpleEMailAddress  EMail,
                               Password?           Password          = null,
                               String              Name              = null,
                               I18NString          Description       = null,
                               PgpPublicKeyRing    PublicKeyRing     = null,
                               PgpSecretKeyRing    SecretKeyRing     = null,
                               PhoneNumber?        Telephone         = null,
                               PhoneNumber?        MobilePhone       = null,
                               GeoCoordinate?      GeoLocation       = null,
                               Address             Address           = null,
                               PrivacyLevel        PrivacyLevel      = PrivacyLevel.World,
                               DateTime?           AcceptedEULA      = null,
                               Boolean             IsAuthenticated   = false,
                               Boolean             IsDisabled        = false,
                               User_Id?            CurrentUserId     = null)
        {

            lock (_Users)
            {

                if (_Users.ContainsKey(Id))
                    throw new ArgumentException("The given username already exists!", nameof(Id));


                var User = new User(Id,
                                    EMail,
                                    Name,
                                    Description,
                                    PublicKeyRing,
                                    SecretKeyRing,
                                    Telephone,
                                    MobilePhone,
                                    GeoLocation,
                                    Address,
                                    PrivacyLevel,
                                    AcceptedEULA,
                                    IsAuthenticated,
                                    IsDisabled);

                WriteToLogfileAndNotify(NotificationMessageType.Parse("createUser"),
                                        User.ToJSON(),
                                        NoOwner,
                                        CurrentUserId);

                if (Password.HasValue)
                    ChangePassword(Id, Password.Value, null, CurrentUserId);

                return _Users.AddAndReturnValue(User.Id, User);

            }

        }

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
        /// <param name="GeoLocation">An optional geographical location of the user.</param>
        /// <param name="Address">An optional address of the user.</param>
        /// <param name="PrivacyLevel">Whether the user will be shown in user listings, or not.</param>
        /// <param name="AcceptedEULA">Timestamp when the user accepted the End-User-License-Agreement.</param>
        /// <param name="IsAuthenticated">The user will not be shown in user listings, as its primary e-mail address is not yet authenticated.</param>
        /// <param name="IsDisabled">The user will be shown in user listings.</param>
        public User CreateUserIfNotExists(User_Id             Id,
                                          SimpleEMailAddress  EMail,
                                          Password?           Password          = null,
                                          String              Name              = null,
                                          I18NString          Description       = null,
                                          PgpPublicKeyRing    PublicKeyRing     = null,
                                          PgpSecretKeyRing    SecretKeyRing     = null,
                                          PhoneNumber?        Telephone         = null,
                                          PhoneNumber?        MobilePhone       = null,
                                          GeoCoordinate?      GeoLocation       = null,
                                          Address             Address           = null,
                                          PrivacyLevel        PrivacyLevel      = PrivacyLevel.World,
                                          DateTime?           AcceptedEULA      = null,
                                          Boolean             IsAuthenticated   = false,
                                          Boolean             IsDisabled        = false,
                                          User_Id?            CurrentUserId     = null)
        {

            lock (_Users)
            {

                if (_Users.ContainsKey(Id))
                    return _Users[Id];

                return CreateUser(Id,
                                  EMail,
                                  Password,
                                  Name,
                                  Description,
                                  PublicKeyRing,
                                  SecretKeyRing,
                                  Telephone,
                                  MobilePhone,
                                  GeoLocation,
                                  Address,
                                  PrivacyLevel,
                                  AcceptedEULA,
                                  IsAuthenticated,
                                  IsDisabled,
                                  CurrentUserId);

            }

        }

        #endregion

        #region AddOrUpdate(User, CurrentUserId = null)

        /// <summary>
        /// Add or update the given user to/within the API.
        /// </summary>
        /// <param name="User">A user.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public User AddOrUpdate(User      User,
                                User_Id?  CurrentUserId  = null)
        {

            lock (_Users)
            {

                if (User.API != null && User.API != this)
                    throw new ArgumentException(nameof(User), "The given user is already attached to another API!");

                WriteToLogfileAndNotify(NotificationMessageType.Parse("addOrUpdateUser"),
                                        User.ToJSON(),
                                        NoOwner,
                                        CurrentUserId);

                User.API = this;

                if (_Users.TryGetValue(User.Id, out User OldUser))
                {
                    _Users.Remove(OldUser.Id);
                    OldUser.CopyAllEdgesTo(User);
                }

                return _Users.AddAndReturnValue(User.Id, User);

            }

        }

        #endregion

        #region Update     (User, CurrentUserId = null)

        /// <summary>
        /// Update the given user to/within the API.
        /// </summary>
        /// <param name="User">A user.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public User Update(User      User,
                           User_Id?  CurrentUserId  = null)
        {

            lock (_Users)
            {

                if (User.API != null && User.API != this)
                    throw new ArgumentException(nameof(User), "The given user is already attached to another API!");

                if (!_Users.TryGetValue(User.Id, out User OldUser))
                    throw new Exception("User '" + User.Id + "' does not exists in this API!");


                WriteToLogfileAndNotify(NotificationMessageType.Parse("updateUser"),
                                        User.ToJSON(),
                                        NoOwner,
                                        CurrentUserId);

                User.API = this;

                _Users.Remove(OldUser.Id);
                OldUser.CopyAllEdgesTo(User);

                return _Users.AddAndReturnValue(User.Id, User);

            }

        }

        #endregion


        #region ChangePassword   (Login, NewPassword, CurrentPassword = null, CurrentUserId = null)

        public void ChangePassword(User_Id   Login,
                                   Password  NewPassword,
                                   String    CurrentPassword  = null,
                                   User_Id?  CurrentUserId    = null)
        {

            lock (_Users)
            {

                if (!TryChangePassword(Login,
                                       NewPassword,
                                       CurrentPassword,
                                       CurrentUserId))
                {
                    throw new ApplicationException("The password could not be changed, as the current password does not match!");
                }

            }

        }

        #endregion

        #region TryChangePassword(Login, NewPassword, CurrentPassword = null, CurrentUserId = null)

        public Boolean TryChangePassword(User_Id   Login,
                                         Password  NewPassword,
                                         String    CurrentPassword  = null,
                                         User_Id?  CurrentUserId    = null)
        {

            lock (_Users)
            {

                #region AddPassword

                if (!_LoginPasswords.TryGetValue(Login, out LoginPassword _LoginPassword))
                {

                    WriteToLogfileAndNotify(NotificationMessageType.Parse("addPassword"),
                                            new JObject(
                                                new JProperty("login",         Login.ToString()),
                                                new JProperty("newPassword", new JObject(
                                                    new JProperty("salt",          NewPassword.Salt.UnsecureString()),
                                                    new JProperty("passwordHash",  NewPassword.UnsecureString)
                                                ))
                                            ),
                                            NoOwner,
                                            DefaultPasswordFile,
                                            CurrentUserId);

                    _LoginPasswords.Add(Login, new LoginPassword(Login, NewPassword));

                    return true;

                }

                #endregion

                #region ChangePassword

                else if (CurrentPassword.IsNotNullOrEmpty() && _LoginPassword.VerifyPassword(CurrentPassword))
                {

                    WriteToLogfileAndNotify(NotificationMessageType.Parse("changePassword"),
                                            new JObject(
                                                new JProperty("login",         Login.ToString()),
                                                new JProperty("currentPassword", new JObject(
                                                    new JProperty("salt",          _LoginPassword.Password.Salt.UnsecureString()),
                                                    new JProperty("passwordHash",  _LoginPassword.Password.UnsecureString)
                                                )),
                                                new JProperty("newPassword",     new JObject(
                                                    new JProperty("salt",          NewPassword.Salt.UnsecureString()),
                                                    new JProperty("passwordHash",  NewPassword.UnsecureString)
                                                ))
                                            ),
                                            NoOwner,
                                            DefaultPasswordFile,
                                            CurrentUserId);

                    _LoginPasswords[Login] = new LoginPassword(Login, NewPassword);

                    return true;

                }

                #endregion

                else
                    return false;

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


        public UserContext SetUserContext(User User)
            => new UserContext(User.Id);

        public UserContext SetUserContext(User_Id UserId)
            => new UserContext(UserId);

        #endregion

        #region Reset user password

        #region ResetPassword(UserId,  SecurityToken1, SecurityToken2 = null)

        public PasswordReset ResetPassword(User_Id            UserId,
                                           SecurityToken_Id   SecurityToken1,
                                           SecurityToken_Id?  SecurityToken2 = null)

            => Add(new PasswordReset(new User_Id[] { UserId },
                                     SecurityToken1,
                                     SecurityToken2));

        #endregion

        #region ResetPassword(UserIds, SecurityToken1, SecurityToken2 = null)

        public PasswordReset ResetPassword(IEnumerable<User_Id>  UserIds,
                                           SecurityToken_Id      SecurityToken1,
                                           SecurityToken_Id?     SecurityToken2 = null)

            => Add(new PasswordReset(UserIds,
                                     SecurityToken1,
                                     SecurityToken2));

        #endregion

        #region Add   (passwordReset)

        public PasswordReset Add(PasswordReset passwordReset)
        {

            WriteToLogfileAndNotify(NotificationMessageType.Parse("add"),
                                    passwordReset.ToJSON(),
                                    NoOwner,
                                    DefaultPasswordResetsFile);

            this.PasswordResets.Add(passwordReset.SecurityToken1, passwordReset);

            return passwordReset;

        }

        #endregion

        #region Remove(passwordReset)

        public PasswordReset Remove(PasswordReset passwordReset)
        {

            WriteToLogfileAndNotify(NotificationMessageType.Parse("remove"),
                                    passwordReset.ToJSON(),
                                    NoOwner,
                                    DefaultPasswordResetsFile);

            this.PasswordResets.Remove(passwordReset.SecurityToken1);

            return passwordReset;

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

        public APIKeyInfo AddAPIKey(APIKeyInfo  APIKey,
                                    User_Id     UserId)
        {

            lock (_APIKeys)
            {

                if (_APIKeys.ContainsKey(APIKey.APIKey))
                    return _APIKeys[APIKey.APIKey];

                WriteToLogfileAndNotify(NotificationMessageType.Parse("addAPIKey"),
                                        APIKey.ToJSON(true),
                                        NoOwner,
                                        UserId);

                return _APIKeys.AddAndReturnValue(APIKey.APIKey, APIKey);

            }

        }

        #endregion

        #endregion

        #region Groups

        #region CreateGroup           (Id, Name = null, Description = null)

        public Group CreateGroup(Group_Id   Id,
                                 User_Id    CurrentUserId,
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
                    WriteToLogfileAndNotify(NotificationMessageType.Parse("createGroup"),
                                            Group.ToJSON(),
                                            NoOwner,
                                            CurrentUserId);

                return _Groups.AddAndReturnValue(Group.Id, Group);

            }

        }

        #endregion

        #region CreateGroupIfNotExists(Id, Name = null, Description = null)

        public Group CreateGroupIfNotExists(Group_Id    Id,
                                            User_Id     CurrentUserId,
                                            I18NString  Name         = null,
                                            I18NString  Description  = null)
        {

            lock (_Groups)
            {

                if (_Groups.ContainsKey(Id))
                    return _Groups[Id];

                return CreateGroup(Id,
                                   CurrentUserId,
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

        #region AddToGroup(User, Edge, Group, PrivacyLevel = Private)

        public Boolean AddToGroup(User             User,
                                  User2GroupEdges  Edge,
                                  Group            Group,
                                  PrivacyLevel     PrivacyLevel   = PrivacyLevel.Private,
                                  User_Id?         CurrentUserId  = null)

        {

            if (!User.OutEdges(Group).Any(edge => edge == Edge))
            {

                User.AddOutgoingEdge(Edge, Group, PrivacyLevel);

                if (!Group.Edges(Group).Any(edge => edge == Edge))
                    Group.AddIncomingEdge(User, Edge,  PrivacyLevel);

                WriteToLogfileAndNotify(NotificationMessageType.Parse("addUserToGroup"),
                                        new JObject(
                                            new JProperty("user",     User.Id.ToString()),
                                            new JProperty("edge",     Edge.   ToString()),
                                            new JProperty("group",    Group.  ToString()),
                                            PrivacyLevel.ToJSON()
                                        ),
                                        NoOwner,
                                        CurrentUserId);

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

        protected readonly Dictionary<Organization_Id, Organization> _Organizations;

        public IEnumerable<Organization> Organizations
            => _Organizations.Values;


        #region Add           (Organization, ParentOrganization = null, CurrentUserId = null)

        /// <summary>
        /// Add the given organization to the API.
        /// </summary>
        /// <param name="Organization">A new organization to be added to this API.</param>
        /// <param name="ParentOrganization">The parent organization of the organization organization to be added.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public Organization Add(Organization  Organization,
                                Organization  ParentOrganization   = null,
                                User_Id?      CurrentUserId        = null)
        {

            lock (_Organizations)
            {

                if (Organization.API != null && Organization.API != this)
                    throw new ArgumentException(nameof(Organization), "The given organization is already attached to another API!");

                if (_Organizations.ContainsKey(Organization.Id))
                    throw new Exception("Organization '" + Organization.Id + "' already exists in this API!");

                if (ParentOrganization != null && !_Organizations.ContainsKey(ParentOrganization.Id))
                    throw new Exception("Parent organization '" + ParentOrganization.Id + "' does not exists in this API!");

                Organization.API = this;

                WriteToLogfileAndNotify(NotificationMessageType.Parse("addOrganization"),
                                        Organization.ToJSON(),
                                        NoOwner,
                                        CurrentUserId);

                var NewOrg = _Organizations.AddAndReturnValue(Organization.Id, Organization);

                if (ParentOrganization != null)
                    LinkOrganizations(NewOrg, Organization2OrganizationEdges.IsChildOf, ParentOrganization, CurrentUserId: CurrentUserId);

                return NewOrg;

            }

        }

        #endregion

        #region AddIfNotExists(Organization, ParentOrganization = null, CurrentUserId = null)

        /// <summary>
        /// When it has not been created before, add the given organization to the API.
        /// </summary>
        /// <param name="Organization">A new organization to be added to this API.</param>
        /// <param name="ParentOrganization">The parent organization of the organization to be added.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public Organization AddIfNotExists(Organization  Organization,
                                           Organization  ParentOrganization   = null,
                                           User_Id?      CurrentUserId        = null)
        {

            lock (_Organizations)
            {

                if (Organization.API != null && Organization.API != this)
                    throw new ArgumentException(nameof(Organization), "The given organization is already attached to another API!");

                if (_Organizations.ContainsKey(Organization.Id))
                    return _Organizations[Organization.Id];

                if (ParentOrganization != null && !_Organizations.ContainsKey(ParentOrganization.Id))
                    throw new Exception("Parent organization '" + ParentOrganization.Id + "' does not exists in this API!");

                Organization.API = this;

                WriteToLogfileAndNotify(NotificationMessageType.Parse("addIfNotExistsOrganization"),
                                        Organization.ToJSON(),
                                        NoOwner,
                                        CurrentUserId);

                var NewOrg = _Organizations.AddAndReturnValue(Organization.Id, Organization);

                if (ParentOrganization != null)
                    LinkOrganizations(NewOrg, Organization2OrganizationEdges.IsChildOf, ParentOrganization, CurrentUserId: CurrentUserId);

                return NewOrg;

            }

        }

        #endregion

        #region AddOrUpdate   (Organization, ParentOrganization = null, CurrentUserId = null)

        /// <summary>
        /// Add or update the given organization to/within the API.
        /// </summary>
        /// <param name="Organization">A organization.</param>
        /// <param name="ParentOrganization">The parent organization of the organization to be added.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public Organization AddOrUpdate(Organization  Organization,
                                        Organization  ParentOrganization   = null,
                                        User_Id?      CurrentUserId        = null)
        {

            lock (_Organizations)
            {

                if (Organization.API != null && Organization.API != this)
                    throw new ArgumentException(nameof(Organization), "The given organization is already attached to another API!");

                if (ParentOrganization != null && !_Organizations.ContainsKey(ParentOrganization.Id))
                    throw new Exception("Parent organization '" + ParentOrganization.Id + "' does not exists in this API!");

                if (_Organizations.TryGetValue(Organization.Id, out Organization OldOrganization))
                {
                    _Organizations.Remove(OldOrganization.Id);
                }

                Organization.API = this;

                WriteToLogfileAndNotify(NotificationMessageType.Parse("addOrUpdateOrganization"),
                                        Organization.ToJSON(),
                                        Organization,
                                        CurrentUserId);

                var NewOrg = _Organizations.AddAndReturnValue(Organization.Id, Organization);

                if (ParentOrganization != null)
                {
                    LinkOrganizations(NewOrg, Organization2OrganizationEdges.IsChildOf, ParentOrganization, CurrentUserId: CurrentUserId);
                    //ToDo: Update link to parent organization
                }

                return NewOrg;

            }

        }

        #endregion

        #region Update        (Organization, CurrentUserId = null)

        /// <summary>
        /// Update the given organization within the API.
        /// </summary>
        /// <param name="Organization">A organization.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public Organization Update(Organization  Organization,
                                   User_Id?      CurrentUserId  = null)
        {

            lock (_Organizations)
            {

                if (Organization.API != null && Organization.API != this)
                    throw new ArgumentException(nameof(Organization), "The given organization is already attached to another API!");

                if (!_Organizations.TryGetValue(Organization.Id, out Organization OldOrganization))
                    throw new Exception("Organization '" + Organization.Id + "' does not exists in this API!");

                else
                {

                    _Organizations.Remove(OldOrganization.Id);

                }

                Organization.API = this;

                WriteToLogfileAndNotify(NotificationMessageType.Parse("updateOrganization"),
                                        Organization.ToJSON(),
                                        Organization,
                                        CurrentUserId);

                return _Organizations.AddAndReturnValue(Organization.Id, Organization);

            }

        }

        #endregion

        #region Remove        (OrganizationId, CurrentUserId = null)

        /// <summary>
        /// Remove the given organization from this API.
        /// </summary>
        /// <param name="OrganizationId">The unique identification of the organization.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public Organization Remove(Organization_Id  OrganizationId,
                                   User_Id?         CurrentUserId  = null)
        {

            lock (_Organizations)
            {

                if (_Organizations.TryGetValue(OrganizationId, out Organization Organization))
                {

                    WriteToLogfileAndNotify(NotificationMessageType.Parse("removeOrganization"),
                                            Organization.ToJSON(),
                                            Organization,
                                            CurrentUserId);

                    _Organizations.Remove(OrganizationId);

                    Organization.API = null;

                    return Organization;

                }

                return null;

            }

        }

        #endregion


        #region CreateOrganization           (Id, Name = null, Description = null, ParentOrganization = null)

        public Organization CreateOrganization(Organization_Id           Id,
                                               I18NString                Name                 = null,
                                               I18NString                Description          = null,
                                               SimpleEMailAddress?       EMail                = null,
                                               String                    PublicKeyRing        = null,
                                               PhoneNumber?              Telephone            = null,
                                               GeoCoordinate?            GeoLocation          = null,
                                               Address                   Address              = null,
                                               Func<Tags.Builder, Tags>  Tags                 = null,
                                               PrivacyLevel              PrivacyLevel         = PrivacyLevel.World,
                                               Boolean                   IsDisabled           = false,
                                               String                    DataSource           = "",
                                               Organization              ParentOrganization   = null,
                                               User_Id?                  CurrentUserId        = null)
        {

            lock (_Organizations)
            {

                if (_Organizations.ContainsKey(Id))
                    throw new ArgumentException("The given organization identification already exists!", nameof(Id));

                return Add(new Organization(Id,
                                            Name,
                                            Description,
                                            EMail,
                                            PublicKeyRing,
                                            Telephone,
                                            GeoLocation,
                                            Address,
                                            Tags,
                                            PrivacyLevel,
                                            IsDisabled,
                                            DataSource),
                           ParentOrganization);

            }

        }

        #endregion

        #region CreateOrganizationIfNotExists(Id, Name = null, Description = null, ParentOrganization = null)

        public Organization CreateOrganizationIfNotExists(Organization_Id           Id,
                                                          I18NString                Name                 = null,
                                                          I18NString                Description          = null,
                                                          SimpleEMailAddress?       EMail                = null,
                                                          String                    PublicKeyRing        = null,
                                                          PhoneNumber?              Telephone            = null,
                                                          GeoCoordinate?            GeoLocation          = null,
                                                          Address                   Address              = null,
                                                          Func<Tags.Builder, Tags>  Tags                 = null,
                                                          PrivacyLevel              PrivacyLevel         = PrivacyLevel.World,
                                                          Boolean                   IsDisabled           = false,
                                                          String                    DataSource           = "",
                                                          Organization              ParentOrganization   = null,
                                                          User_Id?                  CurrentUserId        = null)
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
                                          Tags,
                                          PrivacyLevel,
                                          IsDisabled,
                                          DataSource,
                                          ParentOrganization,
                                          CurrentUserId);

            }

        }

        #endregion


        #region AddToOrganization(User, Edge, Organization, PrivacyLevel = Private)

        public Boolean AddToOrganization(User                    User,
                                         User2OrganizationEdges  Edge,
                                         Organization            Organization,
                                         PrivacyLevel            PrivacyLevel   = PrivacyLevel.Private,
                                         User_Id?                CurrentUserId  = null)
        {

            if (!User.Edges(Organization).Any(edge => edge == Edge))
            {

                User.AddOutgoingEdge(Edge, Organization, PrivacyLevel);

                if (!Organization.User2OrganizationInEdgeLabels(User).Any(edgelabel => edgelabel == Edge))
                    Organization.LinkUser(User, Edge, PrivacyLevel);

                WriteToLogfileAndNotify(NotificationMessageType.Parse("addUserToOrganization"),
                                        new JObject(
                                            new JProperty("user",          User.        Id.ToString()),
                                            new JProperty("edge",          Edge.           ToString()),
                                            new JProperty("organization",  Organization.Id.ToString()),
                                            PrivacyLevel.ToJSON()
                                        ),
                                        Organization,
                                        CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region LinkOrganizations  (OrganizationOut, EdgeLabel, OrganizationIn, Privacy = Public, CurrentUserId = null)

        public Boolean LinkOrganizations(Organization                    OrganizationOut,
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

                WriteToLogfileAndNotify(NotificationMessageType.Parse("linkOrganizations"),
                                        new JObject(
                                            new JProperty("organizationOut", OrganizationOut.Id.ToString()),
                                            new JProperty("edge",            EdgeLabel.         ToString()),
                                            new JProperty("organizationIn",  OrganizationIn. Id.ToString()),
                                            Privacy.ToJSON()
                                        ),
                                        OrganizationOut,
                                        CurrentUserId);

                return true;

            }

            return false;

        }

        #endregion

        #region UnlinkOrganizations(OrganizationOut, EdgeLabel, OrganizationIn,                   CurrentUserId = null)

        public Boolean UnlinkOrganizations(Organization                    OrganizationOut,
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

                WriteToLogfileAndNotify(NotificationMessageType.Parse("unlinkOrganizations"),
                                        new JObject(
                                            new JProperty("organizationOut", OrganizationOut.Id.ToString()),
                                            new JProperty("edge",            EdgeLabel.         ToString()),
                                            new JProperty("organizationIn",  OrganizationIn. Id.ToString())
                                        ),
                                        new Organization[] { OrganizationOut, OrganizationIn },
                                        CurrentUserId);

                return true;

            }

            return false;

        }

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

            lock (_Organizations)
            {
                return _Organizations.ContainsKey(OrganizationId);
            }

        }

        #endregion

        #region Get           (OrganizationId)

        /// <summary>
        /// Get the organization having the given unique identification.
        /// </summary>
        /// <param name="OrganizationId">The unique identification of the organization.</param>
        public Organization Get(Organization_Id  OrganizationId)
        {

            lock (_Organizations)
            {

                if (_Organizations.TryGetValue(OrganizationId, out Organization Organization))
                    return Organization;

                return null;

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

            lock (_Organizations)
            {
                return _Organizations.TryGetValue(OrganizationId, out Organization);
            }

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

        #region AddNotification(User,   NotificationType)

        public NotificationStore AddNotification<T>(User  User,
                                                    T     NotificationType)

            where T : ANotification

            => User.AddNotification(NotificationType);

        #endregion

        #region AddNotification(UserId, NotificationType)

        public NotificationStore AddNotification<T>(User_Id  UserId,
                                                    T        NotificationType)

            where T : ANotification

            => TryGetUser(UserId, out User User)
                   ? User.AddNotification(NotificationType)
                   : null;

        #endregion

        #region AddNotification(User,   NotificationType, NotificationMessageType)

        public NotificationStore AddNotification<T>(User                     User,
                                                    T                        NotificationType,
                                                    NotificationMessageType  NotificationMessageType)

            where T : ANotification

            => User.AddNotification(NotificationType,
                                    NotificationMessageType);

        #endregion

        #region AddNotification(UserId, NotificationType, NotificationMessageType)

        public NotificationStore AddNotification<T>(User_Id                  UserId,
                                                    T                        NotificationType,
                                                    NotificationMessageType  NotificationMessageType)

            where T : ANotification

            => TryGetUser(UserId, out User User)
                   ? User.AddNotification(NotificationType,
                                          NotificationMessageType)
                   : null;

        #endregion

        #region AddNotification(User,   NotificationType, NotificationMessageTypes)

        public NotificationStore AddNotification<T>(User                                  User,
                                                    T                                     NotificationType,
                                                    IEnumerable<NotificationMessageType>  NotificationMessageTypes)

            where T : ANotification

            => User.AddNotification(NotificationType,
                                    NotificationMessageTypes);

        #endregion

        #region AddNotification(UserId, NotificationType, NotificationMessageTypes)

        public NotificationStore AddNotification<T>(User_Id                               UserId,
                                                    T                                     NotificationType,
                                                    IEnumerable<NotificationMessageType>  NotificationMessageTypes)

            where T : ANotification

            => TryGetUser(UserId, out User User)
                   ? User.AddNotification(NotificationType,
                                          NotificationMessageTypes)
                   : null;

        #endregion


        #region GetNotifications  (User,   NotificationMessageType = null)

        public IEnumerable<ANotification> GetNotifications(User                      User,
                                                           NotificationMessageType?  NotificationMessageType = null)

            => User.GetNotifications(NotificationMessageType);

        #endregion

        #region GetNotifications  (UserId, NotificationMessageType = null)

        public IEnumerable<ANotification> GetNotifications<T>(User_Id                   UserId,
                                                                  NotificationMessageType?  NotificationMessageType = null)

            => TryGetUser(UserId, out User User)
                   ? User.GetNotifications(NotificationMessageType)
                   : new ANotification[0];

        #endregion

        #region GetNotificationsOf(User,   NotificationMessageType = null)

        public IEnumerable<T> GetNotificationsOf<T>(User                      User,
                                                    NotificationMessageType?  NotificationMessageType = null)

            where T : ANotification

            => User.GetNotificationsOf<T>(NotificationMessageType);

        #endregion

        #region GetNotificationsOf(UserId, NotificationMessageType = null)

        public IEnumerable<T> GetNotificationsOf<T>(User_Id                   UserId,
                                                    NotificationMessageType?  NotificationMessageType = null)

            where T : ANotification

            => TryGetUser(UserId, out User User)
                   ? User.GetNotificationsOf<T>(NotificationMessageType)
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

            => TryGetUser(UserId, out User User)
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

            => TryGetUser(UserId, out User User)
                   ? User.GetNotificationsOf<T>(NotificationMessageTypeFilter)
                   : new T[0];

        #endregion



        //public Notifications UnregisterNotification<T>(User              User,
        //                                               Func<T, Boolean>  EqualityComparer)

        //    where T : ANotificationType

        //    => _Notifications.Remove(User,
        //                             EqualityComparer);

        //public Notifications UnregisterNotification<T>(User_Id           User,
        //                                               Func<T, Boolean>  EqualityComparer)

        //    where T : ANotificationType

        //    => _Notifications.Remove(User,
        //                             EqualityComparer);


        //public Notifications UnregisterNotification<T>(User              User,
        //                                               NotificationMessageType   NotificationMessageType,
        //                                               Func<T, Boolean>  EqualityComparer)

        //    where T : ANotificationType

        //    => _Notifications.Remove(User,
        //                             NotificationMessageType,
        //                             EqualityComparer);

        //public Notifications UnregisterNotification<T>(User_Id           User,
        //                                               NotificationMessageType   NotificationMessageType,
        //                                               Func<T, Boolean>  EqualityComparer)

        //    where T : ANotificationType

        //    => _Notifications.Remove(User,
        //                             NotificationMessageType,
        //                             EqualityComparer);

        #endregion


    }

}
