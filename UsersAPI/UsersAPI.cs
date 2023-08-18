/*
 * Copyright (c) 2014-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of UsersAPI <https://www.github.com/Vanaheimr/UsersAPI>
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

using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

using Newtonsoft.Json.Linq;

using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Styx.Arrows;
using org.GraphDefined.Vanaheimr.Warden;
using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.BouncyCastle;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.DNS;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.SMTP;
using org.GraphDefined.Vanaheimr.Hermod.Sockets;
using org.GraphDefined.Vanaheimr.Hermod.Sockets.TCP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

using SMSAPI     = com.GraphDefined.SMSApi.API;
using SMSAPIResp = com.GraphDefined.SMSApi.API.Response;

using org.GraphDefined.Vanaheimr.Hermod.HTTP.Notifications;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// Extension method for the Users API.
    /// </summary>
    public static class UsersAPIExtensions
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
        public static Boolean ParseUserId(this HTTPRequest           HTTPRequest,
                                          UsersAPI                   UsersAPI,
                                          out User_Id?               UserId,
                                          out HTTPResponse.Builder?  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest is null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (UsersAPI    is null)
                throw new ArgumentNullException(nameof(UsersAPI),     "The given Users API must not be null!");

            #endregion

            UserId        = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            UserId = User_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!UserId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
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
        public static Boolean ParseUser(this HTTPRequest           HTTPRequest,
                                        UsersAPI                   UsersAPI,
                                        out User_Id?               UserId,
                                        out IUser?                 User,
                                        out HTTPResponse.Builder?  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest is null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (UsersAPI    is null)
                throw new ArgumentNullException(nameof(UsersAPI),     "The given Users API must not be null!");

            #endregion

            UserId        = null;
            User          = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            UserId = User_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!UserId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
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
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown UserId!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion


        #region ParseUserGroupId(this HTTPRequest, UsersAPI, out UsersAPI,                   out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the user identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="UsersAPI">The Users API.</param>
        /// <param name="UserGroupId">The parsed unique user identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseUserGroupId(this HTTPRequest           HTTPRequest,
                                               UsersAPI                   UsersAPI,
                                               out UserGroup_Id?          UserGroupId,
                                               out HTTPResponse.Builder?  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest is null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (UsersAPI    is null)
                throw new ArgumentNullException(nameof(UsersAPI),     "The given Users API must not be null!");

            #endregion

            UserGroupId   = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            UserGroupId = UserGroup_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!UserGroupId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid UserGroupId!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseUserGroup  (this HTTPRequest, UsersAPI, out UserGroupId, out UserGroup, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the user identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="UsersAPI">The Users API.</param>
        /// <param name="UserGroupId">The parsed unique user identification.</param>
        /// <param name="UserGroup">The resolved user.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when user identification was found; false else.</returns>
        public static Boolean ParseUserGroup(this HTTPRequest           HTTPRequest,
                                             UsersAPI                   UsersAPI,
                                             out UserGroup_Id?          UserGroupId,
                                             out IUserGroup?            UserGroup,
                                             out HTTPResponse.Builder?  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest is null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (UsersAPI    is null)
                throw new ArgumentNullException(nameof(UsersAPI),     "The given Users API must not be null!");

            #endregion

            UserGroupId   = null;
            UserGroup     = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            UserGroupId = UserGroup_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!UserGroupId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid UserGroupId!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            if (!UsersAPI.TryGetUserGroup(UserGroupId.Value, out UserGroup)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown UserGroupId!"" }".ToUTF8Bytes(),
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
        public static Boolean ParseOrganizationId(this HTTPRequest           HTTPRequest,
                                                  UsersAPI                   UsersAPI,
                                                  out Organization_Id?       OrganizationId,
                                                  out HTTPResponse.Builder?  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest is null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (UsersAPI    is null)
                throw new ArgumentNullException(nameof(UsersAPI),     "The given Organizations API must not be null!");

            #endregion

            OrganizationId  = null;
            HTTPResponse    = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            OrganizationId = Organization_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!OrganizationId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
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
        public static Boolean ParseOrganization(this HTTPRequest           HTTPRequest,
                                                UsersAPI                   UsersAPI,
                                                out Organization_Id?       OrganizationId,
                                                out IOrganization?         Organization,
                                                out HTTPResponse.Builder?  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest is null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (UsersAPI    is null)
                throw new ArgumentNullException(nameof(UsersAPI),     "The given Organizations API must not be null!");

            #endregion

            OrganizationId  = null;
            Organization    = null;
            HTTPResponse    = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            OrganizationId = Organization_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!OrganizationId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid OrganizationId!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            if (!UsersAPI.TryGetOrganization(OrganizationId.Value, out Organization)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown OrganizationId!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion


        #region ParseBlogPostingId(this HTTPRequest, UsersAPI, out BlogPostingId,                  out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the Blog identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="UsersAPI">The UsersAPI.</param>
        /// <param name="BlogPostingId">The parsed unique blog posting identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when Blog identification was found; false else.</returns>
        public static Boolean ParseBlogPostingId(this HTTPRequest           HTTPRequest,
                                                 UsersAPI                   UsersAPI,
                                                 out BlogPosting_Id?        BlogPostingId,
                                                 out HTTPResponse.Builder?  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest is null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (UsersAPI    is null)
                throw new ArgumentNullException(nameof(UsersAPI),     "The given UsersAPI must not be null!");

            #endregion

            BlogPostingId  = null;
            HTTPResponse   = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            BlogPostingId = BlogPosting_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!BlogPostingId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid BlogId!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseBlogPosting  (this HTTPRequest, UsersAPI, out BlogPostingId, out BlogPosting, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the Blog identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="UsersAPI">The UsersAPI.</param>
        /// <param name="BlogPostingId">The parsed unique blog posting identification.</param>
        /// <param name="BlogPosting">The resolved Blog.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when Blog identification was found; false else.</returns>
        public static Boolean ParseBlogPosting(this HTTPRequest           HTTPRequest,
                                               UsersAPI                   UsersAPI,
                                               out BlogPosting_Id?        BlogPostingId,
                                               out BlogPosting?           BlogPosting,
                                               out HTTPResponse.Builder?  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest is null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (UsersAPI    is null)
                throw new ArgumentNullException(nameof(UsersAPI),     "The given UsersAPI must not be null!");

            #endregion

            BlogPostingId  = null;
            BlogPosting    = null;
            HTTPResponse   = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            BlogPostingId = BlogPosting_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!BlogPostingId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid BlogId!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            if (!UsersAPI.TryGetBlogPosting(BlogPostingId.Value, out BlogPosting)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown BlogId!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion


        #region ParseNewsPostingId(this HTTPRequest, UsersAPI, out NewsPostingId,                  out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the News identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="UsersAPI">The UsersAPI.</param>
        /// <param name="NewsPostingId">The parsed unique news posting identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when News identification was found; false else.</returns>
        public static Boolean ParseNewsPostingId(this HTTPRequest           HTTPRequest,
                                                 UsersAPI                   UsersAPI,
                                                 out NewsPosting_Id?        NewsPostingId,
                                                 out HTTPResponse.Builder?  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest is null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (UsersAPI    is null)
                throw new ArgumentNullException(nameof(UsersAPI),     "The given UsersAPI must not be null!");

            #endregion

            NewsPostingId  = null;
            HTTPResponse   = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            NewsPostingId = NewsPosting_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!NewsPostingId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid NewsId!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseNewsPosting  (this HTTPRequest, UsersAPI, out NewsPostingId, out NewsPosting, out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the News identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="UsersAPI">The UsersAPI.</param>
        /// <param name="NewsPostingId">The parsed unique news posting identification.</param>
        /// <param name="NewsPosting">The resolved News.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when News identification was found; false else.</returns>
        public static Boolean ParseNewsPosting(this HTTPRequest           HTTPRequest,
                                               UsersAPI                   UsersAPI,
                                               out NewsPosting_Id?        NewsPostingId,
                                               out NewsPosting?           NewsPosting,
                                               out HTTPResponse.Builder?  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest is null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (UsersAPI    is null)
                throw new ArgumentNullException(nameof(UsersAPI),     "The given UsersAPI must not be null!");

            #endregion

            NewsPostingId  = null;
            NewsPosting    = null;
            HTTPResponse   = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            NewsPostingId = NewsPosting_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!NewsPostingId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid NewsId!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            if (!UsersAPI.TryGetNewsPosting(NewsPostingId.Value, out NewsPosting)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown NewsId!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion


        #region ParseNewsBannerId (this HTTPRequest, UsersAPI, out NewsBannerId,                   out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the News identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="UsersAPI">The UsersAPI.</param>
        /// <param name="NewsBannerId">The parsed unique news posting identification.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when News identification was found; false else.</returns>
        public static Boolean ParseNewsId(this HTTPRequest           HTTPRequest,
                                          UsersAPI                   UsersAPI,
                                          out NewsBanner_Id?         NewsBannerId,
                                          out HTTPResponse.Builder?  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest is null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (UsersAPI    is null)
                throw new ArgumentNullException(nameof(UsersAPI),     "The given UsersAPI must not be null!");

            #endregion

            NewsBannerId  = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            NewsBannerId = NewsBanner_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!NewsBannerId.HasValue)
            {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid NewsId!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion

        #region ParseNewsBanner   (this HTTPRequest, UsersAPI, out NewsBannerId,  out NewsBanner,  out HTTPResponse)

        /// <summary>
        /// Parse the given HTTP request and return the News identification
        /// for the given HTTP hostname and HTTP query parameter
        /// or an HTTP error response.
        /// </summary>
        /// <param name="HTTPRequest">A HTTP request.</param>
        /// <param name="UsersAPI">The UsersAPI.</param>
        /// <param name="NewsBannerId">The parsed unique news posting identification.</param>
        /// <param name="NewsBanner">The resolved News.</param>
        /// <param name="HTTPResponse">A HTTP error response.</param>
        /// <returns>True, when News identification was found; false else.</returns>
        public static Boolean ParseNewsBanner(this HTTPRequest           HTTPRequest,
                                              UsersAPI                   UsersAPI,
                                              out NewsBanner_Id?         NewsBannerId,
                                              out NewsBanner?            NewsBanner,
                                              out HTTPResponse.Builder?  HTTPResponse)
        {

            #region Initial checks

            if (HTTPRequest is null)
                throw new ArgumentNullException(nameof(HTTPRequest),  "The given HTTP request must not be null!");

            if (UsersAPI    is null)
                throw new ArgumentNullException(nameof(UsersAPI),     "The given UsersAPI must not be null!");

            #endregion

            NewsBannerId  = null;
            NewsBanner    = null;
            HTTPResponse  = null;

            if (HTTPRequest.ParsedURLParameters.Length < 1) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    Connection      = "close"
                };

                return false;

            }

            NewsBannerId = NewsBanner_Id.TryParse(HTTPRequest.ParsedURLParameters[0]);

            if (!NewsBannerId.HasValue) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.BadRequest,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Invalid NewsId!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            if (!UsersAPI.TryGetNewsBanner(NewsBannerId.Value, out NewsBanner)) {

                HTTPResponse = new HTTPResponse.Builder(HTTPRequest) {
                    HTTPStatusCode  = HTTPStatusCode.NotFound,
                    Server          = UsersAPI.HTTPServer.DefaultServerName,
                    Date            = Timestamp.Now,
                    ContentType     = HTTPContentType.JSON_UTF8,
                    Content         = @"{ ""description"": ""Unknown NewsId!"" }".ToUTF8Bytes(),
                    Connection      = "close"
                };

                return false;

            }

            return true;

        }

        #endregion


    }


    /// <summary>
    /// Managing users and organizations et al. within a HTTP API.
    /// </summary>
    public class UsersAPI : HTTPExtAPI
    {

        #region Data

        /// <summary>
        /// The default HTTP server name.
        /// </summary>
        public new const           String         DefaultHTTPServerName                = "GraphDefined Users API";

        /// <summary>
        /// The default HTTP service name.
        /// </summary>
        public new const           String         DefaultHTTPServiceName               = "GraphDefined Users API";

        /// <summary>
        /// The default HTTP server port.
        /// </summary>
        public new static readonly IPPort         DefaultHTTPServerPort                = IPPort.Parse(2305);

        public const               String         DefaultUsersAPI_DatabaseFileName     = "UsersAPI.db";
        public const               String         DefaultUsersAPI_LogfileName          = "UsersAPI.log";
        public const               String         DefaultNewsletterSignupFile          = "NewsletterSignups.db";
        public const               String         DefaultNewsletterEMailsFile          = "NewsletterEMails.db";

        protected static readonly  SemaphoreSlim  ServiceTicketsSemaphore              = new (1, 1);
        protected static readonly  SemaphoreSlim  LogFileSemaphore                     = new (1, 1);
        protected static readonly  SemaphoreSlim  UsersSemaphore                       = new (1, 1);
        protected static readonly  SemaphoreSlim  UserGroupsSemaphore                  = new (1, 1);
        protected static readonly  SemaphoreSlim  APIKeysSemaphore                     = new (1, 1);
        protected static readonly  SemaphoreSlim  OrganizationsSemaphore               = new (1, 1);
        protected static readonly  SemaphoreSlim  OrganizationGroupsSemaphore          = new (1, 1);
        protected static readonly  SemaphoreSlim  MessagesSemaphore                    = new (1, 1);
        protected static readonly  SemaphoreSlim  NotificationMessagesSemaphore        = new (1, 1);
        protected static readonly  SemaphoreSlim  DashboardsSemaphore                  = new (1, 1);
        protected static readonly  SemaphoreSlim  BlogPostingsSemaphore                = new (1, 1);
        protected static readonly  SemaphoreSlim  NewsPostingsSemaphore                = new (1, 1);
        protected static readonly  SemaphoreSlim  NewsBannersSemaphore                 = new (1, 1);
        protected static readonly  SemaphoreSlim  FAQsSemaphore                        = new (1, 1);

        /// <summary>
        /// The HTTP root for embedded ressources.
        /// </summary>
        public new const          String                                        HTTPRoot                                = "social.OpenData.UsersAPI.HTTPRoot.";

        public  const             Byte                                          DefaultMinMessageIdLength               = 8;
        public  const             Byte                                          DefaultMinNotificationMessageIdLength   = 8;
        public  const             Byte                                          DefaultMinNewsPostingIdLength           = 8;
        public  const             Byte                                          DefaultMinNewsBannerIdLength            = 8;
        public  const             Byte                                          DefaultMinFAQIdLength                   = 8;

        public  static readonly   PasswordQualityCheckDelegate                  DefaultPasswordQualityCheck             = password => password.Length >= 8 ? 1.0f : 0;
        public  static readonly   TimeSpan                                      DefaultMaxSignInSessionLifetime         = TimeSpan.FromDays(30);

        public  const             String                                        SignUpContext                           = "";
        public  const             String                                        SignInOutContext                        = "";

        protected readonly        Dictionary<IIPAddress,       DateTime>          _NewsletterRemoteIPAddresses;
        protected readonly        Dictionary<SecurityToken_Id, NewsletterSignup>  _NewsletterSignups;
        protected readonly        Dictionary<Newsletter_Id,    EMailAddress>      _NewsletterEMails;

        protected static readonly String[]  Split1  = { "\r\n" };
        protected static readonly String[]  Split2  = { ": " };
        protected static readonly String[]  Split3  = { " " };
        protected static readonly Char[]    Split4  = { ',' };
        protected static readonly Char[]    Split5  = { '|' };

        #endregion

        #region Properties

        /// <summary>
        /// The API database file.
        /// </summary>
        public String                         DatabaseFileName                   { get; }


        /// <summary>
        /// The API version hash (git commit hash value).
        /// </summary>
        public String                         APIVersionHash                     { get; }


        public Organization_Id                AdminOrganizationId                { get; }

        public String                         UsersAPIPath                       { get; }
        public String                         NotificationsPath                  { get; }
        public String                         SMTPLoggingPath                    { get; }
        public String                         TelegramLoggingPath                { get; }
        public String                         SMSAPILoggingPath                  { get; }

        /// <summary>
        /// The e-mail address used for the default newsletter.
        /// </summary>
        public EMailAddress                   NewsletterEMailAddress             { get; set; }

        /// <summary>
        /// The e-mail address used for the default newsletter.
        /// </summary>
        public String                         NewsletterGPGPassphrase            { get; set; }

        /// <summary>
        /// The passphrase of the PGP/GPG secret key of the API.
        /// </summary>
        public String                         APIRobotGPGPassphrase              { get; }

        /// <summary>
        /// A SMTP client to be used by the API.
        /// </summary>
        public ISMTPClient                    SMTPClient                         { get; }

        /// <summary>
        /// The SMSAPI.
        /// </summary>
        public SMSAPI.ISMSClient              SMSClient                          { get; }

        /// <summary>
        /// The (default) SMS sender name.
        /// </summary>
        public String                         SMSSenderName                      { get; }

        ///// <summary>
        ///// The Telegram API access token of the bot.
        ///// </summary>
        //public String                        TelegramBotToken                   { get; }

        ///// <summary>
        ///// The Telegram API client.
        ///// </summary>
        //public TelegramBotClient             TelegramAPI                        { get; }

        /// <summary>
        /// The Telegram user store.
        /// </summary>
        public ITelegramStore                 TelegramClient                      { get; }

        public HTTPCookieName                 CookieName                         { get; }

        public HTTPCookieName                 SessionCookieName                  { get; }

        /// <summary>
        /// Force the web browser to send cookies only via HTTPS.
        /// </summary>
        public Boolean                        UseSecureCookies                   { get; }


        /// <summary>
        /// The default language used within this API.
        /// </summary>
        public Languages                      DefaultLanguage                    { get; }

        /// <summary>
        /// The maximum sign-in session lifetime.
        /// </summary>
        public TimeSpan                       MaxSignInSessionLifetime           { get; }

        /// <summary>
        /// The minimal user identification length.
        /// </summary>
        public Byte                           MinUserIdLength                    { get; }

        /// <summary>
        /// The minimal realm length.
        /// </summary>
        public Byte                           MinRealmLength                     { get; }

        /// <summary>
        /// A delegate to ensure a minimal password quality.
        /// </summary>
        public PasswordQualityCheckDelegate   PasswordQualityCheck               { get; }

        /// <summary>
        /// The minimal user name length.
        /// </summary>
        public Byte                           MinUserNameLength                  { get; }

        /// <summary>
        /// The minimal user group identification length.
        /// </summary>
        public Byte                           MinUserGroupIdLength               { get; }

        /// <summary>
        /// The minimal API key length.
        /// </summary>
        public UInt16                         MinAPIKeyLength                    { get; }

        /// <summary>
        /// The minimal message identification length.
        /// </summary>
        public Byte                           MinMessageIdLength                 { get; }

        /// <summary>
        /// The minimal message identification length.
        /// </summary>
        public Byte                           MinOrganizationIdLength            { get; }

        /// <summary>
        /// The minimal message identification length.
        /// </summary>
        public Byte                           MinOrganizationGroupIdLength       { get; }

        /// <summary>
        /// The minimal notification message identification length.
        /// </summary>
        public Byte                           MinNotificationMessageIdLength     { get; }

        /// <summary>
        /// The minimal blog posting identification length.
        /// </summary>
        public Byte                           MinBlogPostingIdLength             { get; }

        /// <summary>
        /// The minimal news posting identification length.
        /// </summary>
        public Byte                           MinNewsPostingIdLength             { get; }

        /// <summary>
        /// The minimal news banner identification length.
        /// </summary>
        public Byte                           MinNewsBannerIdLength              { get; }

        /// <summary>
        /// The minimal FAQ identification length.
        /// </summary>
        public Byte                           MinFAQIdLength                     { get; }


        /// <summary>
        /// The current hash value of the API.
        /// </summary>
        public String                         CurrentDatabaseHashValue           { get; protected set; }

        /// <summary>
        /// Disable external notifications.
        /// </summary>
        public Boolean                        DisableNotifications               { get; set; }


        private readonly HashSet<URLWithAPIKey>  remoteAuthServers;

        /// <summary>
        /// Servers for remote authorization.
        /// </summary>
        public IEnumerable<URLWithAPIKey>    RemoteAuthServers
            => RemoteAuthServers;


        private readonly HashSet<APIKey_Id>  remoteAuthAPIKeys;

        /// <summary>
        /// API key for incoming remote authorizations.
        /// </summary>
        public IEnumerable<APIKey_Id>         RemoteAuthAPIKeys
            => remoteAuthAPIKeys;


        /// <summary>
        /// An optional HTML template.
        /// </summary>
        public String                         BlogTemplate                       { get; protected set; }

        #endregion

        #region Events

        #region (protected internal) AddUsersRequest (Request)

        /// <summary>
        /// An event sent whenever add users request was received.
        /// </summary>
        public HTTPRequestLogEvent OnAddUsersRequest = new ();

        /// <summary>
        /// An event sent whenever add users request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task AddUsersHTTPRequest(DateTime     Timestamp,
                                                    HTTPAPI      API,
                                                    HTTPRequest  Request)

            => OnAddUsersRequest?.WhenAll(Timestamp,
                                          API ?? this,
                                          Request);

        #endregion

        #region (protected internal) AddUsersResponse(Response)

        /// <summary>
        /// An event sent whenever a response on an add users request was sent.
        /// </summary>
        public HTTPResponseLogEvent OnAddUsersResponse = new ();

        /// <summary>
        /// An event sent whenever a response on an add users request was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task AddUsersHTTPResponse(DateTime      Timestamp,
                                                     HTTPAPI       API,
                                                     HTTPRequest   Request,
                                                     HTTPResponse  Response)

            => OnAddUsersResponse?.WhenAll(Timestamp,
                                           API ?? this,
                                           Request,
                                           Response);

        #endregion


        #region (protected internal) AddUserHTTPRequest (Request)

        /// <summary>
        /// An event sent whenever add user request was received.
        /// </summary>
        public HTTPRequestLogEvent OnAddUserHTTPRequest = new ();

        /// <summary>
        /// An event sent whenever add user request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task AddUserHTTPRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   HTTPRequest  Request)

            => OnAddUserHTTPRequest?.WhenAll(Timestamp,
                                             API ?? this,
                                             Request);

        #endregion

        #region (protected internal) AddUserHTTPResponse(Response)

        /// <summary>
        /// An event sent whenever a response on an add user request was sent.
        /// </summary>
        public HTTPResponseLogEvent OnAddUserHTTPResponse = new ();

        /// <summary>
        /// An event sent whenever a response on an add user request was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task AddUserHTTPResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    HTTPRequest   Request,
                                                    HTTPResponse  Response)

            => OnAddUserHTTPResponse?.WhenAll(Timestamp,
                                              API ?? this,
                                              Request,
                                              Response);

        #endregion


        #region (protected internal) SetUserHTTPRequest (Request)

        /// <summary>
        /// An event sent whenever set user request was received.
        /// </summary>
        public HTTPRequestLogEvent OnSetUserHTTPRequest = new ();

        /// <summary>
        /// An event sent whenever set user request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task SetUserHTTPRequest(DateTime     Timestamp,
                                                   HTTPAPI      API,
                                                   HTTPRequest  Request)

            => OnSetUserHTTPRequest?.WhenAll(Timestamp,
                                             API ?? this,
                                             Request);

        #endregion

        #region (protected internal) SetUserHTTPResponse(Response)

        /// <summary>
        /// An event sent whenever a response on a set user request was sent.
        /// </summary>
        public HTTPResponseLogEvent OnSetUserHTTPResponse = new ();

        /// <summary>
        /// An event sent whenever a response on a set user request was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task SetUserHTTPResponse(DateTime      Timestamp,
                                                    HTTPAPI       API,
                                                    HTTPRequest   Request,
                                                    HTTPResponse  Response)

            => OnSetUserHTTPResponse?.WhenAll(Timestamp,
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


        // ---------------------------------------------------------------


        #region (protected internal) OnAddOrganizationHTTPRequest (Request)

        /// <summary>
        /// An event sent whenever add organization request was received.
        /// </summary>
        public HTTPRequestLogEvent OnAddOrganizationHTTPRequest = new HTTPRequestLogEvent();

        /// <summary>
        /// An event sent whenever add organization request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task AddOrganizationHTTPRequest(DateTime     Timestamp,
                                                           HTTPAPI      API,
                                                           HTTPRequest  Request)

            => OnAddOrganizationHTTPRequest?.WhenAll(Timestamp,
                                                     API ?? this,
                                                     Request);

        #endregion

        #region (protected internal) OnAddOrganizationHTTPResponse(Response)

        /// <summary>
        /// An event sent whenever a response on an add organization request was sent.
        /// </summary>
        public HTTPResponseLogEvent OnAddOrganizationHTTPResponse = new HTTPResponseLogEvent();

        /// <summary>
        /// An event sent whenever a response on an add organization request was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task AddOrganizationHTTPResponse(DateTime      Timestamp,
                                                            HTTPAPI       API,
                                                            HTTPRequest   Request,
                                                            HTTPResponse  Response)

            => OnAddOrganizationHTTPResponse?.WhenAll(Timestamp,
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


        #region (protected internal) DeleteOrganizationHTTPRequest (Request)

        /// <summary>
        /// An event sent whenever delete organization request was received.
        /// </summary>
        public HTTPRequestLogEvent OnDeleteOrganizationHTTPRequest = new HTTPRequestLogEvent();

        /// <summary>
        /// An event sent whenever delete organization request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task DeleteOrganizationHTTPRequest(DateTime     Timestamp,
                                                              HTTPAPI      API,
                                                              HTTPRequest  Request)

            => OnDeleteOrganizationHTTPRequest?.WhenAll(Timestamp,
                                                        API ?? this,
                                                        Request);

        #endregion

        #region (protected internal) DeleteOrganizationHTTPResponse(Response)

        /// <summary>
        /// An event sent whenever a response on a delete organization request was sent.
        /// </summary>
        public HTTPResponseLogEvent OnDeleteOrganizationHTTPResponse = new HTTPResponseLogEvent();

        /// <summary>
        /// An event sent whenever a response on a delete organization request was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task DeleteOrganizationHTTPResponse(DateTime      Timestamp,
                                                               HTTPAPI       API,
                                                               HTTPRequest   Request,
                                                               HTTPResponse  Response)

            => OnDeleteOrganizationHTTPResponse?.WhenAll(Timestamp,
                                                         API ?? this,
                                                         Request,
                                                         Response);

        #endregion



        #region (protected internal) SetOrganizationNotificationsRequest    (Request)

        /// <summary>
        /// An event sent whenever set organization notifications request was received.
        /// </summary>
        public HTTPRequestLogEvent OnSetOrganizationNotificationsRequest = new HTTPRequestLogEvent();

        /// <summary>
        /// An event sent whenever set organization notifications request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task SetOrganizationNotificationsRequest(DateTime     Timestamp,
                                                                    HTTPAPI      API,
                                                                    HTTPRequest  Request)

            => OnSetOrganizationNotificationsRequest?.WhenAll(Timestamp,
                                                              API ?? this,
                                                              Request);

        #endregion

        #region (protected internal) SetOrganizationNotificationsResponse   (Response)

        /// <summary>
        /// An event sent whenever a response on a set organization notifications request was sent.
        /// </summary>
        public HTTPResponseLogEvent OnSetOrganizationNotificationsResponse = new HTTPResponseLogEvent();

        /// <summary>
        /// An event sent whenever a response on a set organization notifications request was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task SetOrganizationNotificationsResponse(DateTime      Timestamp,
                                                                     HTTPAPI       API,
                                                                     HTTPRequest   Request,
                                                                     HTTPResponse  Response)

            => OnSetOrganizationNotificationsResponse?.WhenAll(Timestamp,
                                                               API ?? this,
                                                               Request,
                                                               Response);

        #endregion


        #region (protected internal) DeleteOrganizationNotificationsRequest (Request)

        /// <summary>
        /// An event sent whenever set organization notifications request was received.
        /// </summary>
        public HTTPRequestLogEvent OnDeleteOrganizationNotificationsRequest = new HTTPRequestLogEvent();

        /// <summary>
        /// An event sent whenever set organization notifications request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task DeleteOrganizationNotificationsRequest(DateTime     Timestamp,
                                                                       HTTPAPI      API,
                                                                       HTTPRequest  Request)

            => OnDeleteOrganizationNotificationsRequest?.WhenAll(Timestamp,
                                                                 API ?? this,
                                                                 Request);

        #endregion

        #region (protected internal) DeleteOrganizationNotificationsResponse(Response)

        /// <summary>
        /// An event sent whenever a response on a set organization notifications request was sent.
        /// </summary>
        public HTTPResponseLogEvent OnDeleteOrganizationNotificationsResponse = new HTTPResponseLogEvent();

        /// <summary>
        /// An event sent whenever a response on a set organization notifications request was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task DeleteOrganizationNotificationsResponse(DateTime      Timestamp,
                                                                        HTTPAPI       API,
                                                                        HTTPRequest   Request,
                                                                        HTTPResponse  Response)

            => OnDeleteOrganizationNotificationsResponse?.WhenAll(Timestamp,
                                                                  API ?? this,
                                                                  Request,
                                                                  Response);

        #endregion

        // ---------------------------------------------------------------------

        #region (protected internal) AddServiceTicketRequest (Request)

        /// <summary>
        /// An event sent whenever a add service ticket request was received.
        /// </summary>
        public HTTPRequestLogEvent OnAddServiceTicketRequest = new HTTPRequestLogEvent();

        /// <summary>
        /// An event sent whenever a add service ticket request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task AddServiceTicketRequest(DateTime     Timestamp,
                                                        HTTPAPI      API,
                                                        HTTPRequest  Request)

            => OnAddServiceTicketRequest?.WhenAll(Timestamp,
                                                  API ?? this,
                                                  Request);

        #endregion

        #region (protected internal) AddServiceTicketResponse(Response)

        /// <summary>
        /// An event sent whenever a add service ticket response was sent.
        /// </summary>
        public HTTPResponseLogEvent OnAddServiceTicketResponse = new HTTPResponseLogEvent();

        /// <summary>
        /// An event sent whenever a add service ticket response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task AddServiceTicketResponse(DateTime      Timestamp,
                                                         HTTPAPI       API,
                                                         HTTPRequest   Request,
                                                         HTTPResponse  Response)

            => OnAddServiceTicketResponse?.WhenAll(Timestamp,
                                                   API ?? this,
                                                   Request,
                                                   Response);

        #endregion


        #region (protected internal) SetServiceTicketRequest (Request)

        /// <summary>
        /// An event sent whenever a set service ticket request was received.
        /// </summary>
        public HTTPRequestLogEvent OnSetServiceTicketRequest = new HTTPRequestLogEvent();

        /// <summary>
        /// An event sent whenever a set service ticket request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task SetServiceTicketRequest(DateTime     Timestamp,
                                                        HTTPAPI      API,
                                                        HTTPRequest  Request)

            => OnSetServiceTicketRequest?.WhenAll(Timestamp,
                                                  API ?? this,
                                                  Request);

        #endregion

        #region (protected internal) SetServiceTicketResponse(Response)

        /// <summary>
        /// An event sent whenever a set service ticket response was sent.
        /// </summary>
        public HTTPResponseLogEvent OnSetServiceTicketResponse = new HTTPResponseLogEvent();

        /// <summary>
        /// An event sent whenever a set service ticket response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task SetServiceTicketResponse(DateTime      Timestamp,
                                                         HTTPAPI       API,
                                                         HTTPRequest   Request,
                                                         HTTPResponse  Response)

            => OnSetServiceTicketResponse?.WhenAll(Timestamp,
                                                   API ?? this,
                                                   Request,
                                                   Response);

        #endregion


        #region (protected internal) AddServiceTicketChangeSetRequest (Request)

        /// <summary>
        /// An event sent whenever a add service ticket change set request was received.
        /// </summary>
        public HTTPRequestLogEvent OnAddServiceTicketChangeSetRequest = new HTTPRequestLogEvent();

        /// <summary>
        /// An event sent whenever a add service ticket change set request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task AddServiceTicketChangeSetRequest(DateTime     Timestamp,
                                                                 HTTPAPI      API,
                                                                 HTTPRequest  Request)

            => OnAddServiceTicketChangeSetRequest?.WhenAll(Timestamp,
                                                           API ?? this,
                                                           Request);

        #endregion

        #region (protected internal) AddServiceTicketChangeSetResponse(Response)

        /// <summary>
        /// An event sent whenever a add service ticket change set response was sent.
        /// </summary>
        public HTTPResponseLogEvent OnAddServiceTicketChangeSetResponse = new HTTPResponseLogEvent();

        /// <summary>
        /// An event sent whenever a add service ticket change set response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task AddServiceTicketChangeSetResponse(DateTime      Timestamp,
                                                                  HTTPAPI       API,
                                                                  HTTPRequest   Request,
                                                                  HTTPResponse  Response)

            => OnAddServiceTicketChangeSetResponse?.WhenAll(Timestamp,
                                                            API ?? this,
                                                            Request,
                                                            Response);

        #endregion


        // ---------------------------------------------------------------------

        #region (protected internal) RestartRequest (Request)

        /// <summary>
        /// An event sent whenever a restart request was received.
        /// </summary>
        public HTTPRequestLogEvent OnRestartHTTPRequest = new HTTPRequestLogEvent();

        /// <summary>
        /// An event sent whenever a restart request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task RestartRequest(DateTime     Timestamp,
                                               HTTPAPI      API,
                                               HTTPRequest  Request)

            => OnRestartHTTPRequest?.WhenAll(Timestamp,
                                             API ?? this,
                                             Request);

        #endregion

        #region (protected internal) RestartResponse(Response)

        /// <summary>
        /// An event sent whenever a restart response was sent.
        /// </summary>
        public HTTPResponseLogEvent OnRestartHTTPResponse = new HTTPResponseLogEvent();

        /// <summary>
        /// An event sent whenever a restart response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task RestartResponse(DateTime      Timestamp,
                                                HTTPAPI       API,
                                                HTTPRequest   Request,
                                                HTTPResponse  Response)

            => OnRestartHTTPResponse?.WhenAll(Timestamp,
                                              API ?? this,
                                              Request,
                                              Response);

        #endregion


        #region (protected internal) StopRequest (Request)

        /// <summary>
        /// An event sent whenever a stop request was received.
        /// </summary>
        public HTTPRequestLogEvent OnStopHTTPRequest = new HTTPRequestLogEvent();

        /// <summary>
        /// An event sent whenever a stop request was received.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        protected internal Task StopRequest(DateTime     Timestamp,
                                            HTTPAPI      API,
                                            HTTPRequest  Request)

            => OnStopHTTPRequest?.WhenAll(Timestamp,
                                          API ?? this,
                                          Request);

        #endregion

        #region (protected internal) StopResponse(Response)

        /// <summary>
        /// An event sent whenever a stop response was sent.
        /// </summary>
        public HTTPResponseLogEvent OnStopHTTPResponse = new HTTPResponseLogEvent();

        /// <summary>
        /// An event sent whenever a stop response was sent.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the request.</param>
        /// <param name="API">The HTTP API.</param>
        /// <param name="Request">A HTTP request.</param>
        /// <param name="Response">A HTTP response.</param>
        protected internal Task StopResponse(DateTime      Timestamp,
                                             HTTPAPI       API,
                                             HTTPRequest   Request,
                                             HTTPResponse  Response)

            => OnStopHTTPResponse?.WhenAll(Timestamp,
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
        /// <param name="HTTPHostname">The HTTP hostname for all URLs within this API.</param>
        /// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        /// <param name="HTTPServerPort">A TCP port to listen on.</param>
        /// <param name="BasePath">When the API is served from an optional subdirectory path.</param>
        /// <param name="HTTPServerName">The default HTTP servername, used whenever no HTTP Host-header has been given.</param>
        /// 
        /// <param name="URLPathPrefix">A common prefix for all URLs.</param>
        /// <param name="HTTPServiceName">The name of the HTTP service.</param>
        /// <param name="HTMLTemplate">An optional HTML template.</param>
        /// <param name="APIVersionHashes">The API version hashes (git commit hash values).</param>
        /// 
        /// <param name="ServerCertificateSelector">An optional delegate to select a SSL/TLS server certificate.</param>
        /// <param name="ClientCertificateValidator">An optional delegate to verify the SSL/TLS client certificate used for authentication.</param>
        /// <param name="ClientCertificateSelector">An optional delegate to select the SSL/TLS client certificate used for authentication.</param>
        /// <param name="AllowedTLSProtocols">The SSL/TLS protocol(s) allowed for this connection.</param>
        /// 
        /// <param name="ServerThreadName">The optional name of the TCP server thread.</param>
        /// <param name="ServerThreadPriority">The optional priority of the TCP server thread.</param>
        /// <param name="ServerThreadIsBackground">Whether the TCP server thread is a background thread or not.</param>
        /// <param name="ConnectionIdBuilder">An optional delegate to build a connection identification based on IP socket information.</param>
        /// <param name="ConnectionTimeout">The TCP client timeout for all incoming client connections in seconds (default: 30 sec).</param>
        /// <param name="MaxClientConnections">The maximum number of concurrent TCP client connections (default: 4096).</param>
        /// 
        /// <param name="AdminOrganizationId">The admins' organization identification.</param>
        /// <param name="APIRobotEMailAddress">An e-mail address for this API.</param>
        /// <param name="APIRobotGPGPassphrase">A GPG passphrase for this API.</param>
        /// <param name="SMTPClient">A SMTP client for sending e-mails.</param>
        /// <param name="SMSClient">A SMS client for sending SMS.</param>
        /// <param name="SMSSenderName">The (default) SMS sender name.</param>
        /// <param name="TelegramClient">A Telegram client for sendind and receiving Telegrams.</param>
        /// 
        /// <param name="PasswordQualityCheck">A delegate to ensure a minimal password quality.</param>
        /// <param name="CookieName">The name of the HTTP Cookie for authentication.</param>
        /// <param name="UseSecureCookies">Force the web browser to send cookies only via HTTPS.</param>
        /// <param name="MaxSignInSessionLifetime">The maximum sign-in session lifetime.</param>
        /// <param name="DefaultLanguage">The default language of the API.</param>
        /// <param name="MinUserIdLength">The minimal user identification length.</param>
        /// <param name="MinUserNameLength">The minimal user name length.</param>
        /// <param name="MinRealmLength">The minimal realm length.</param>
        /// <param name="MinUserGroupIdLength">The minimal user group identification length.</param>
        /// <param name="MinAPIKeyLength">The minimal API key length.</param>
        /// 
        /// <param name="DisableMaintenanceTasks">Disable all maintenance tasks.</param>
        /// <param name="MaintenanceInitialDelay">The initial delay of the maintenance tasks.</param>
        /// <param name="MaintenanceEvery">The maintenance intervall.</param>
        /// 
        /// <param name="DisableWardenTasks">Disable all warden tasks.</param>
        /// <param name="WardenInitialDelay">The initial delay of the warden tasks.</param>
        /// <param name="WardenCheckEvery">The warden intervall.</param>
        /// 
        /// <param name="RemoteAuthServers">Servers for remote authorization.</param>
        /// <param name="RemoteAuthAPIKeys">API keys for incoming remote authorizations.</param>
        /// 
        /// <param name="IsDevelopment">This HTTP API runs in development mode.</param>
        /// <param name="DevelopmentServers">An enumeration of server names which will imply to run this service in development mode.</param>
        /// <param name="SkipURLTemplates">Skip URL templates.</param>
        /// <param name="DatabaseFileName">The name of the database file for this API.</param>
        /// <param name="DisableNotifications">Disable external notifications.</param>
        /// <param name="DisableLogging">Disable the log file.</param>
        /// <param name="LoggingPath">The path for all logfiles.</param>
        /// <param name="LogfileName">The name of the logfile.</param>
        /// <param name="LogfileCreator">A delegate for creating the name of the logfile for this API.</param>
        /// <param name="DNSClient">The DNS client of the API.</param>
        /// <param name="AutoStart">Whether to start the API automatically.</param>
        public UsersAPI(HTTPHostname?                        HTTPHostname                     = null,
                        String?                              ExternalDNSName                  = null,
                        IPPort?                              HTTPServerPort                   = null,
                        HTTPPath?                            BasePath                         = null,
                        String?                              HTTPServerName                   = DefaultHTTPServerName,

                        HTTPPath?                            URLPathPrefix                    = null,
                        String?                              HTTPServiceName                  = DefaultHTTPServiceName,
                        String?                              HTMLTemplate                     = null,
                        JObject?                             APIVersionHashes                 = null,

                        ServerCertificateSelectorDelegate?   ServerCertificateSelector        = null,
                        RemoteCertificateValidationHandler?  ClientCertificateValidator       = null,
                        LocalCertificateSelectionHandler?    ClientCertificateSelector        = null,
                        SslProtocols?                        AllowedTLSProtocols              = null,
                        Boolean?                             ClientCertificateRequired        = null,
                        Boolean?                             CheckCertificateRevocation       = null,

                        ServerThreadNameCreatorDelegate?     ServerThreadNameCreator          = null,
                        ServerThreadPriorityDelegate?        ServerThreadPrioritySetter       = null,
                        Boolean?                             ServerThreadIsBackground         = null,
                        ConnectionIdBuilder?                 ConnectionIdBuilder              = null,
                        TimeSpan?                            ConnectionTimeout                = null,
                        UInt32?                              MaxClientConnections             = null,

                        Organization_Id?                     AdminOrganizationId              = null,
                        EMailAddress?                        APIRobotEMailAddress             = null,
                        String?                              APIRobotGPGPassphrase            = null,
                        ISMTPClient?                         SMTPClient                       = null,
                        SMSAPI.ISMSClient?                   SMSClient                        = null,
                        String?                              SMSSenderName                    = null,
                        ITelegramStore?                      TelegramClient                   = null,

                        PasswordQualityCheckDelegate?        PasswordQualityCheck             = null,
                        HTTPCookieName?                      CookieName                       = null,
                        Boolean                              UseSecureCookies                 = true,
                        TimeSpan?                            MaxSignInSessionLifetime         = null,
                        Languages?                           DefaultLanguage                  = null,
                        Byte?                                MinUserIdLength                  = null,
                        Byte?                                MinRealmLength                   = null,
                        Byte?                                MinUserNameLength                = null,
                        Byte?                                MinUserGroupIdLength             = null,
                        UInt16?                              MinAPIKeyLength                  = null,
                        Byte?                                MinMessageIdLength               = null,
                        Byte?                                MinOrganizationIdLength          = null,
                        Byte?                                MinOrganizationGroupIdLength     = null,
                        Byte?                                MinNotificationMessageIdLength   = null,
                        Byte?                                MinNewsPostingIdLength           = null,
                        Byte?                                MinNewsBannerIdLength            = null,
                        Byte?                                MinFAQIdLength                   = null,

                        Boolean?                             DisableMaintenanceTasks          = null,
                        TimeSpan?                            MaintenanceInitialDelay          = null,
                        TimeSpan?                            MaintenanceEvery                 = null,

                        Boolean?                             DisableWardenTasks               = null,
                        TimeSpan?                            WardenInitialDelay               = null,
                        TimeSpan?                            WardenCheckEvery                 = null,

                        IEnumerable<URLWithAPIKey>?          RemoteAuthServers                = null,
                        IEnumerable<APIKey_Id>?              RemoteAuthAPIKeys                = null,

                        Boolean?                             IsDevelopment                    = null,
                        IEnumerable<String>?                 DevelopmentServers               = null,
                        Boolean                              SkipURLTemplates                 = false,
                        String?                              DatabaseFileName                 = DefaultUsersAPI_DatabaseFileName,
                        Boolean                              DisableNotifications             = false,
                        Boolean                              DisableLogging                   = false,
                        String?                              LoggingPath                      = null,
                        String?                              LogfileName                      = DefaultUsersAPI_LogfileName,
                        LogfileCreatorDelegate?              LogfileCreator                   = null,
                        DNSClient?                           DNSClient                        = null,
                        Boolean                              AutoStart                        = false)

            : base(HTTPHostname,
                   ExternalDNSName,
                   HTTPServerPort,
                   BasePath,
                   HTTPServerName  ?? DefaultHTTPServerName,

                   URLPathPrefix,
                   HTTPServiceName ?? DefaultHTTPServiceName,
                   HTMLTemplate,
                   APIVersionHashes,

                   ServerCertificateSelector,
                   ClientCertificateValidator,
                   ClientCertificateSelector,
                   AllowedTLSProtocols,
                   ClientCertificateRequired,
                   CheckCertificateRevocation,

                   ServerThreadNameCreator,
                   ServerThreadPrioritySetter,
                   ServerThreadIsBackground,
                   ConnectionIdBuilder,
                   ConnectionTimeout,
                   MaxClientConnections,

                   AdminOrganizationId,
                   APIRobotEMailAddress,
                   APIRobotGPGPassphrase,
                   SMTPClient,
                   //SMSClient,
                   //SMSSenderName,
                   //TelegramClient,

                   PasswordQualityCheck,
                   CookieName,
                   UseSecureCookies,
                   MaxSignInSessionLifetime,
                   DefaultLanguage,
                   MinUserIdLength,
                   MinRealmLength,
                   MinUserNameLength,
                   MinUserGroupIdLength,
                   MinAPIKeyLength,
                   MinMessageIdLength,
                   MinOrganizationIdLength,
                   MinOrganizationGroupIdLength,
                   MinNotificationMessageIdLength,
                   MinNewsPostingIdLength,
                   MinNewsBannerIdLength,
                   MinFAQIdLength,

                   DisableMaintenanceTasks,
                   MaintenanceInitialDelay,
                   MaintenanceEvery,

                   DisableWardenTasks,
                   WardenInitialDelay,
                   WardenCheckEvery,

                   RemoteAuthServers,
                   RemoteAuthAPIKeys,

                   IsDevelopment,
                   DevelopmentServers,
                   SkipURLTemplates,
                   DatabaseFileName,
                   DisableNotifications,
                   DisableLogging,
                   LoggingPath,
                   LogfileName,
                   LogfileCreator,
                   DNSClient,
                   false) // AutoStart

        {

            #region Inital checks

            if (HTTPServer is null)
                throw new ArgumentNullException(nameof(HTTPServer),            "The given HTTP server must not be null!");

            if (APIRobotEMailAddress is null)
                throw new ArgumentNullException(nameof(APIRobotEMailAddress),  "The given API robot e-mail address must not be null!");

            #endregion

            #region Init data

            this.dataLicenses                    = new Dictionary<OpenDataLicense_Id,         OpenDataLicense>();
            this._NewsletterRemoteIPAddresses    = new Dictionary<IIPAddress,                 DateTime>();
            this._NewsletterSignups              = new Dictionary<SecurityToken_Id,           NewsletterSignup>();
            this._NewsletterEMails               = new Dictionary<Newsletter_Id,              EMailAddress>();
            this._Messages                       = new Dictionary<Message_Id,                 Message>();
            this._ServiceTickets                 = new ConcurrentDictionary<ServiceTicket_Id, ServiceTicket>();
            this._BlogPostings                   = new Dictionary<BlogPosting_Id,             BlogPosting>();
            this._NewsPostings                   = new Dictionary<NewsPosting_Id,             NewsPosting>();
            this._NewsBanners                    = new Dictionary<NewsBanner_Id,              NewsBanner>();
            this._FAQs                           = new Dictionary<FAQ_Id,                     FAQ>();


            this.APIVersionHash                  = APIVersionHashes?[nameof(UsersAPI)]?.Value<String>()?.Trim();

            if (this.APIVersionHash.IsNullOrEmpty())
                this.APIVersionHash              = "unknown";

            this.DatabaseFileName                = this.LoggingPath + (DatabaseFileName ?? DefaultUsersAPI_DatabaseFileName);

            this.UsersAPIPath                    = this.LoggingPath + "UsersAPI"       + Path.DirectorySeparatorChar;
            this.NotificationsPath               = this.LoggingPath + "Notifications"  + Path.DirectorySeparatorChar;
            this.SMTPLoggingPath                 = this.LoggingPath + "SMTPClient"     + Path.DirectorySeparatorChar;
            this.TelegramLoggingPath             = this.LoggingPath + "Telegram"       + Path.DirectorySeparatorChar;
            this.SMSAPILoggingPath               = this.LoggingPath + "SMSAPIClient"   + Path.DirectorySeparatorChar;

            this.DisableNotifications            = DisableNotifications;

            if (!DisableLogging)
            {
                Directory.CreateDirectory(this.UsersAPIPath);
                Directory.CreateDirectory(this.NotificationsPath);
                Directory.CreateDirectory(this.SMTPLoggingPath);
                Directory.CreateDirectory(this.TelegramLoggingPath);
                Directory.CreateDirectory(this.SMSAPILoggingPath);
            }

            CurrentAsyncLocalUserId.Value        = Robot.Id;

            this.AdminOrganizationId             = AdminOrganizationId            ?? DefaultAdminOrganizationId;
            this.APIRobotGPGPassphrase           = APIRobotGPGPassphrase;
            this.SMTPClient                      = SMTPClient                     ?? throw new ArgumentNullException(nameof(SMTPClient), "The given API SMTP client must not be null!");

            this.CookieName                      = CookieName                     ?? DefaultCookieName;
            this.SessionCookieName               = this.CookieName + "Session";
            this.UseSecureCookies                = UseSecureCookies;
            this.DefaultLanguage                 = DefaultLanguage                ?? DefaultDefaultLanguage;

            this.MinUserIdLength                 = MinUserIdLength                ?? DefaultMinUserIdLength;
            this.MinRealmLength                  = MinRealmLength                 ?? DefaultMinRealmLength;
            this.MinUserNameLength               = MinUserNameLength              ?? DefaultMinUserNameLength;
            this.MinUserGroupIdLength            = MinUserGroupIdLength           ?? DefaultMinUserGroupIdLength;
            this.MinAPIKeyLength                 = MinAPIKeyLength                ?? DefaultMinAPIKeyLength;
            this.MinMessageIdLength              = MinMessageIdLength             ?? DefaultMinMessageIdLength;
            this.MinOrganizationIdLength         = MinOrganizationIdLength        ?? DefaultMinOrganizationIdLength;
            this.MinOrganizationGroupIdLength    = MinOrganizationGroupIdLength   ?? DefaultMinOrganizationGroupIdLength;
            this.MinNotificationMessageIdLength  = MinNotificationMessageIdLength ?? DefaultMinNotificationMessageIdLength;
            this.MinNewsPostingIdLength          = MinNewsPostingIdLength         ?? DefaultMinNewsPostingIdLength;
            this.MinNewsBannerIdLength           = MinNewsBannerIdLength          ?? DefaultMinNewsBannerIdLength;
            this.MinFAQIdLength                  = MinFAQIdLength                 ?? DefaultMinFAQIdLength;

            this.PasswordQualityCheck            = PasswordQualityCheck           ?? DefaultPasswordQualityCheck;
            this.MaxSignInSessionLifetime        = MaxSignInSessionLifetime       ?? DefaultMaxSignInSessionLifetime;

            this.remoteAuthServers               = RemoteAuthServers is not null   ? new HashSet<URLWithAPIKey>(RemoteAuthServers) : new HashSet<URLWithAPIKey>();
            this.remoteAuthAPIKeys               = RemoteAuthAPIKeys is not null   ? new HashSet<APIKey_Id>    (RemoteAuthAPIKeys) : new HashSet<APIKey_Id>();

            #endregion

            #region Reflect data licenses

            foreach (var dataLicense in typeof(OpenDataLicense).GetFields(System.Reflection.BindingFlags.Public |
                                                                      System.Reflection.BindingFlags.Static).
                                                            Where (fieldinfo => fieldinfo.ReflectedType == typeof(OpenDataLicense) &&
                                                                                fieldinfo.FieldType     == typeof(OpenDataLicense)).
                                                            Select(fieldinfo => fieldinfo.GetValue(OpenDataLicense.None)).
                                                            Cast<OpenDataLicense>())
            {

                dataLicenses.Add(dataLicense.Id,
                                 dataLicense);

            }

            #endregion


            #region Setup SMSAPI

            this.SMSClient      = SMSClient;
            this.SMSSenderName  = SMSSenderName ?? "GraphDefined";

            #endregion

            #region Setup Telegram

            this.TelegramClient = TelegramClient;

            if (this.TelegramClient != null)
                this.TelegramClient.OnMessage += ReceiveTelegramMessage;

            #endregion

            #region Warden: Observe CPU/RAM => Send admin e-mails...

            Warden.EveryMinutes(15,
                                Environment.OSVersion.Platform == PlatformID.Unix
                                    ? new DriveInfo("/")
                                    : new DriveInfo(Directory.GetCurrentDirectory()),
                                async (timestamp, driveInfo, ct) => {

                                    var MBytesFree       = driveInfo.AvailableFreeSpace / 1024 / 1024;
                                    var HDPercentageFree = 100 * driveInfo.AvailableFreeSpace / driveInfo.TotalSize;

                                    if (HDPercentageFree < 3 &&
                                        TryGetOrganization(this.AdminOrganizationId, out var adminOrganization))
                                    {

                                        // lowStorage_MessageType
                                        await SMTPClient.Send(new HTMLEMailBuilder {
                                                                     From           = Robot.EMail,
                                                                     To             = EMailAddressList.Create(adminOrganization.Admins.Select(admin => admin.EMail)),
                                                                     Passphrase     = APIRobotGPGPassphrase,
                                                                     Subject        = ServiceName + " is low on disc (<" + HDPercentageFree + "%, " + MBytesFree + " MB free)",
                                                                     HTMLText       = ServiceName + " is low on disc (<" + HDPercentageFree + "%, " + MBytesFree + " MB free)" + Environment.NewLine + Environment.NewLine,
                                                                     PlainText      = ServiceName + " is low on disc (<" + HDPercentageFree + "%, " + MBytesFree + " MB free)" + Environment.NewLine + Environment.NewLine,
                                                                     SecurityLevel  = EMailSecurity.autosign
                                                                 }).ConfigureAwait(false);

                                    }

                                });

            #endregion

            RegisterNotifications().Wait();

            if (!SkipURLTemplates)
                RegisterURLTemplates();

            DebugX.Log(nameof(UsersAPI) + " version '" + APIVersionHash + "' initialized...");

            if (AutoStart)
                Start();

        }

        #endregion

        #endregion

        #region (static) AttachToHTTPAPI(HTTPServer, URLPrefix = "/", ...)

        ///// <summary>
        ///// Attach this Open Data HTTP API to the given HTTP server.
        ///// </summary>
        ///// <param name="HTTPServer">An existing HTTP server.</param>
        ///// <param name="HTTPHostname">The HTTP hostname for all URLs within this API.</param>
        ///// <param name="ServiceName">The name of the service.</param>
        ///// <param name="ExternalDNSName">The offical URL/DNS name of this service, e.g. for sending e-mails.</param>
        ///// <param name="URLPathPrefix">A common prefix for all URLs.</param>
        ///// <param name="HTMLTemplate">An optional HTML template.</param>
        ///// <param name="APIVersionHashes">The API version hashes (git commit hash values).</param>
        ///// 
        ///// <param name="AdminOrganizationId">The API admin organization identification.</param>
        ///// <param name="APIRobotEMailAddress">An e-mail address for this API.</param>
        ///// <param name="APIRobotGPGPassphrase">A GPG passphrase for this API.</param>
        ///// <param name="SMTPClient">A SMTP client for sending e-mails.</param>
        ///// <param name="SMSClient">A SMS client for sending SMS.</param>
        ///// <param name="SMSSenderName">The (default) SMS sender name.</param>
        ///// <param name="TelegramBotToken">The Telegram API access token of the bot.</param>
        ///// 
        ///// <param name="PasswordQualityCheck">A delegate to ensure a minimal password quality.</param>
        ///// <param name="CookieName">The name of the HTTP Cookie for authentication.</param>
        ///// <param name="UseSecureCookies">Force the web browser to send cookies only via HTTPS.</param>
        ///// <param name="MaxSignInSessionLifetime">The maximum sign-in session lifetime.</param>
        ///// <param name="DefaultLanguage">The default language of the API.</param>
        ///// <param name="MinUserNameLength">The minimal user name length.</param>
        ///// <param name="MinRealmLength">The minimal realm length.</param>
        ///// 
        ///// <param name="DisableMaintenanceTasks">Disable all maintenance tasks.</param>
        ///// <param name="MaintenanceInitialDelay">The initial delay of the maintenance tasks.</param>
        ///// <param name="MaintenanceEvery">The maintenance intervall.</param>
        ///// <param name="DisableWardenTasks">Disable all warden tasks.</param>
        ///// <param name="WardenInitialDelay">The initial delay of the warden tasks.</param>
        ///// <param name="WardenCheckEvery">The warden intervall.</param>
        ///// 
        ///// <param name="SkipURLTemplates">Skip URL templates.</param>
        ///// <param name="DisableNotifications">Disable external notifications.</param>
        ///// <param name="DisableLogfile">Disable the log file.</param>
        ///// <param name="LogfileName">The name of the logfile for this API.</param>
        //public static UsersAPI AttachToHTTPAPI(HTTPServer                           HTTPServer,
        //                                       HTTPHostname?                        HTTPHostname                     = null,
        //                                       String                               ServiceName                      = "GraphDefined Users API",
        //                                       String                               ExternalDNSName                  = null,
        //                                       HTTPPath?                            URLPathPrefix                    = null,
        //                                       HTTPPath?                            BasePath                         = null,
        //                                       String                               HTMLTemplate                     = null,
        //                                       JObject                              APIVersionHashes                 = null,

        //                                       Organization_Id?                     AdminOrganizationId              = null,
        //                                       EMailAddress                         APIRobotEMailAddress             = null,
        //                                       String                               APIRobotGPGPassphrase            = null,
        //                                       ISMTPClient                          SMTPClient                       = null,
        //                                       ISMSClient                           SMSClient                        = null,
        //                                       String                               SMSSenderName                    = null,
        //                                       String                               TelegramBotToken                 = null,

        //                                       PasswordQualityCheckDelegate         PasswordQualityCheck             = null,
        //                                       HTTPCookieName?                      CookieName                       = null,
        //                                       Boolean                              UseSecureCookies                 = true,
        //                                       TimeSpan?                            MaxSignInSessionLifetime         = null,
        //                                       Languages                            DefaultLanguage                  = Languages.en,
        //                                       Byte?                                MinUserIdLength                  = null,
        //                                       Byte?                                MinRealmLength                   = null,
        //                                       Byte?                                MinUserNameLength                = null,
        //                                       Byte?                                MinUserGroupIdLength             = null,
        //                                       UInt16?                              MinAPIKeyLength                  = null,
        //                                       Byte?                                MinMessageIdLength               = null,
        //                                       Byte?                                MinOrganizationIdLength          = null,
        //                                       Byte?                                MinOrganizationGroupIdLength     = null,
        //                                       Byte?                                MinNotificationMessageIdLength   = null,
        //                                       Byte?                                MinNewsPostingIdLength           = null,
        //                                       Byte?                                MinNewsBannerIdLength            = null,
        //                                       Byte?                                MinFAQIdLength                   = null,

        //                                       Boolean                              DisableMaintenanceTasks          = false,
        //                                       TimeSpan?                            MaintenanceInitialDelay          = null,
        //                                       TimeSpan?                            MaintenanceEvery                 = null,
        //                                       Boolean                              DisableWardenTasks               = false,
        //                                       TimeSpan?                            WardenInitialDelay               = null,
        //                                       TimeSpan?                            WardenCheckEvery                 = null,

        //                                       Boolean                              SkipURLTemplates                 = false,
        //                                       Boolean                              DisableNotifications             = false,
        //                                       Boolean                              DisableLogfile                   = false,
        //                                       String                               LogfileName                      = DefaultLogfileName)


        //    => new UsersAPI(HTTPServer,
        //                    HTTPHostname,
        //                    ServiceName,
        //                    ExternalDNSName,
        //                    URLPathPrefix,
        //                    BasePath,
        //                    HTMLTemplate,
        //                    APIVersionHashes,

        //                    AdminOrganizationId,
        //                    APIRobotEMailAddress,
        //                    APIRobotGPGPassphrase,
        //                    SMTPClient,
        //                    SMSClient,
        //                    SMSSenderName,
        //                    TelegramBotToken,

        //                    PasswordQualityCheck,
        //                    CookieName,
        //                    UseSecureCookies,
        //                    MaxSignInSessionLifetime,
        //                    DefaultLanguage,
        //                    MinUserIdLength,
        //                    MinRealmLength,
        //                    MinUserNameLength,
        //                    MinUserGroupIdLength,
        //                    MinAPIKeyLength,
        //                    MinMessageIdLength,
        //                    MinOrganizationIdLength,
        //                    MinOrganizationGroupIdLength,
        //                    MinNotificationMessageIdLength,
        //                    MinNewsPostingIdLength,
        //                    MinNewsBannerIdLength,
        //                    MinFAQIdLength,

        //                    DisableMaintenanceTasks,
        //                    MaintenanceInitialDelay,
        //                    MaintenanceEvery,
        //                    DisableWardenTasks,
        //                    WardenInitialDelay,
        //                    WardenCheckEvery,

        //                    SkipURLTemplates,
        //                    DisableNotifications,
        //                    DisableLogfile,
        //                    LogfileName);

        #endregion


        #region Notifications...

        #region (static) NotificationMessageTypes

        public static NotificationMessageType lowStorage_MessageType                          = NotificationMessageType.Parse("lowStorage");

        public static NotificationMessageType addMessage_MessageType                          = NotificationMessageType.Parse("addMessage");
        public static NotificationMessageType addMessageIfNotExists_MessageType               = NotificationMessageType.Parse("addMessageIfNotExists");
        public static NotificationMessageType addOrUpdateMessage_MessageType                  = NotificationMessageType.Parse("addOrUpdateMessage");
        public static NotificationMessageType updateMessage_MessageType                       = NotificationMessageType.Parse("updateMessage");
        public static NotificationMessageType removeMessage_MessageType                       = NotificationMessageType.Parse("removeMessage");

        public static NotificationMessageType addNotificationMessage_MessageType              = NotificationMessageType.Parse("addNotificationMessage");
        public static NotificationMessageType addNotificationMessageIfNotExists_MessageType   = NotificationMessageType.Parse("addNotificationMessageIfNotExists");
        public static NotificationMessageType addOrUpdateNotificationMessage_MessageType      = NotificationMessageType.Parse("addOrUpdateNotificationMessage");
        public static NotificationMessageType updateNotificationMessage_MessageType           = NotificationMessageType.Parse("updateNotificationMessage");
        public static NotificationMessageType removeNotificationMessage_MessageType           = NotificationMessageType.Parse("removeNotificationMessage");

        public static NotificationMessageType addNotification_MessageType                     = NotificationMessageType.Parse("addNotification");
        public static NotificationMessageType removeNotification_MessageType                  = NotificationMessageType.Parse("removeNotification");

        public static NotificationMessageType addServiceTicket_MessageType                    = NotificationMessageType.Parse("addServiceTicket");
        public static NotificationMessageType addServiceTicketIfNotExists_MessageType         = NotificationMessageType.Parse("addServiceTicketIfNotExists");
        public static NotificationMessageType addOrUpdateServiceTicket_MessageType            = NotificationMessageType.Parse("addOrUpdateServiceTicket");
        public static NotificationMessageType updateServiceTicket_MessageType                 = NotificationMessageType.Parse("updateServiceTicket");
        public static NotificationMessageType removeServiceTicket_MessageType                 = NotificationMessageType.Parse("removeServiceTicket");
        public static NotificationMessageType changeServiceTicketStatus_MessageType           = NotificationMessageType.Parse("changeServiceTicketStatus");

        public static NotificationMessageType addDashboard_MessageType                        = NotificationMessageType.Parse("addDashboard");
        public static NotificationMessageType addDashboardIfNotExists_MessageType             = NotificationMessageType.Parse("addDashboardIfNotExists");
        public static NotificationMessageType addOrUpdateDashboard_MessageType                = NotificationMessageType.Parse("addOrUpdateDashboard");
        public static NotificationMessageType updateDashboard_MessageType                     = NotificationMessageType.Parse("updateDashboard");
        public static NotificationMessageType removeDashboard_MessageType                     = NotificationMessageType.Parse("removeDashboard");

        public static NotificationMessageType addBlogPosting_MessageType                      = NotificationMessageType.Parse("addBlogPosting");
        public static NotificationMessageType addBlogPostingIfNotExists_MessageType           = NotificationMessageType.Parse("addBlogPostingIfNotExists");
        public static NotificationMessageType addOrUpdateBlogPosting_MessageType              = NotificationMessageType.Parse("addOrUpdateBlogPosting");
        public static NotificationMessageType updateBlogPosting_MessageType                   = NotificationMessageType.Parse("updateBlogPosting");
        public static NotificationMessageType removeBlogPosting_MessageType                   = NotificationMessageType.Parse("removeBlogPosting");

        public static NotificationMessageType addNewsPosting_MessageType                      = NotificationMessageType.Parse("addNewsPosting");
        public static NotificationMessageType addNewsPostingIfNotExists_MessageType           = NotificationMessageType.Parse("addNewsPostingIfNotExists");
        public static NotificationMessageType addOrUpdateNewsPosting_MessageType              = NotificationMessageType.Parse("addOrUpdateNewsPosting");
        public static NotificationMessageType updateNewsPosting_MessageType                   = NotificationMessageType.Parse("updateNewsPosting");
        public static NotificationMessageType removeNewsPosting_MessageType                   = NotificationMessageType.Parse("removeNewsPosting");

        public static NotificationMessageType addNewsBanner_MessageType                       = NotificationMessageType.Parse("addNewsBanner");
        public static NotificationMessageType addNewsBannerIfNotExists_MessageType            = NotificationMessageType.Parse("addNewsBannerIfNotExists");
        public static NotificationMessageType addOrUpdateNewsBanner_MessageType               = NotificationMessageType.Parse("addOrUpdateNewsBanner");
        public static NotificationMessageType updateNewsBanner_MessageType                    = NotificationMessageType.Parse("updateNewsBanner");
        public static NotificationMessageType removeNewsBanner_MessageType                    = NotificationMessageType.Parse("removeNewsBanner");

        public static NotificationMessageType addFAQ_MessageType                              = NotificationMessageType.Parse("addFAQ");
        public static NotificationMessageType addFAQIfNotExists_MessageType                   = NotificationMessageType.Parse("addFAQIfNotExists");
        public static NotificationMessageType addOrUpdateFAQ_MessageType                      = NotificationMessageType.Parse("addOrUpdateFAQ");
        public static NotificationMessageType updateFAQ_MessageType                           = NotificationMessageType.Parse("updateFAQ");
        public static NotificationMessageType removeFAQ_MessageType                           = NotificationMessageType.Parse("removeFAQ");
        public static NotificationMessageType changeFAQAdminStatus_MessageType                = NotificationMessageType.Parse("changeFAQAdminStatus");
        public static NotificationMessageType changeFAQStatus_MessageType                     = NotificationMessageType.Parse("changeFAQStatus");

        #endregion

        #region (private) RegisterNotifications()

        public static NotificationGroup_Id ServiceTicketsNotifications   = NotificationGroup_Id.Parse("Service Ticket Notifications");
        public static NotificationGroup_Id UsersManagementNotifications  = NotificationGroup_Id.Parse("Users Management Notifications");

        private async Task RegisterNotifications()
        {

            await AddNotificationGroup(new NotificationGroup(
                                           ServiceTicketsNotifications,
                                           I18NString.Create(Languages.en, "Service Tickets"),
                                           I18NString.Create(Languages.en, "Service Ticket notifications"),
                                           NotificationVisibility.Customers,
                                           new NotificationMessageDescription[] {
                                               new NotificationMessageDescription(I18NString.Create(Languages.en, "(New) service ticket added"),                  I18NString.Create(Languages.en, ""), NotificationVisibility.Customers, NotificationTag.NewUserDefault, addServiceTicket_MessageType),
                                               new NotificationMessageDescription(I18NString.Create(Languages.en, "(New) service ticket added (did not exist)"),  I18NString.Create(Languages.en, ""), NotificationVisibility.System,    NotificationTag.NewUserDefault, addServiceTicketIfNotExists_MessageType),
                                               new NotificationMessageDescription(I18NString.Create(Languages.en, "(New) service ticket added or updated"),       I18NString.Create(Languages.en, ""), NotificationVisibility.System,    NotificationTag.NewUserDefault, addOrUpdateServiceTicket_MessageType),
                                               new NotificationMessageDescription(I18NString.Create(Languages.en, "ServiceTicket updated"),                       I18NString.Create(Languages.en, ""), NotificationVisibility.Customers, NotificationTag.NewUserDefault, updateServiceTicket_MessageType),
                                               //new NotificationMessageDescription(I18NString.Create(Languages.en, "ServiceTicket removed"),                       I18NString.Create(Languages.en, ""), NotificationVisibility.Customers, NotificationTag.NewUserDefault, removeServiceTicket_MessageType),
                                               new NotificationMessageDescription(I18NString.Create(Languages.en, "ServiceTicket status changed"),                I18NString.Create(Languages.en, ""), NotificationVisibility.Customers, NotificationTag.NewUserDefault, changeServiceTicketStatus_MessageType)
                                           }));

            await AddNotificationGroup(new NotificationGroup(
                                           UsersManagementNotifications,
                                           I18NString.Create(Languages.en, "Users Management"),
                                           I18NString.Create(Languages.en, "Users Management notifications"),
                                           NotificationVisibility.Customers,
                                           new NotificationMessageDescription[] {
                                               new NotificationMessageDescription(I18NString.Create(Languages.en, "New user created"),                            I18NString.Create(Languages.en, "A new user was added to portal."),                   NotificationVisibility.Admins,     addUser_MessageType),
                                               new NotificationMessageDescription(I18NString.Create(Languages.en, "User added to organization"),                  I18NString.Create(Languages.en, "The user was added to an organization."),            NotificationVisibility.Customers,  addUserToOrganization_MessageType),
                                               new NotificationMessageDescription(I18NString.Create(Languages.en, "User information updated"),                    I18NString.Create(Languages.en, "The user information was updated."),                 NotificationVisibility.Customers,  updateUser_MessageType),
                                               new NotificationMessageDescription(I18NString.Create(Languages.en, "User removed from portal"),                    I18NString.Create(Languages.en, "The user was removed from the portal."),             NotificationVisibility.Admins,     deleteUser_MessageType),
                                               new NotificationMessageDescription(I18NString.Create(Languages.en, "User removed from organization"),              I18NString.Create(Languages.en, "The user was removed from an organization."),        NotificationVisibility.Customers,  removeUserFromOrganization_MessageType),

                                               new NotificationMessageDescription(I18NString.Create(Languages.en, "New organization created"),                    I18NString.Create(Languages.en, "A new organization was created."),                   NotificationVisibility.Admins,     addOrganization_MessageType),
                                               new NotificationMessageDescription(I18NString.Create(Languages.en, "Sub organization added to organization"),      I18NString.Create(Languages.en, "A sub organization was added to an organization."),  NotificationVisibility.Customers,  linkOrganizations_MessageType),
                                               new NotificationMessageDescription(I18NString.Create(Languages.en, "Organization information updated"),            I18NString.Create(Languages.en, "An organization information was updated."),          NotificationVisibility.Customers,  updateOrganization_MessageType),
                                               new NotificationMessageDescription(I18NString.Create(Languages.en, "Organization removed"),                        I18NString.Create(Languages.en, "An organization was removed."),                      NotificationVisibility.Admins,     deleteOrganization_MessageType),
                                               new NotificationMessageDescription(I18NString.Create(Languages.en, "Sub organization removed from organization"),  I18NString.Create(Languages.en, "A sub organization removed from an organization."),  NotificationVisibility.Customers,  unlinkOrganizations_MessageType),
                                           }));

        }

        #endregion

        #region E-Mail headers / footers

        /// <summary>
        /// The type of e-mails send.
        /// This might influence the content of the common e-mail headers and footers.
        /// </summary>
        public enum EMailType
        {

            /// <summary>
            /// A normal e-mail.
            /// </summary>
            Normal,

            /// <summary>
            /// A system e-mail.
            /// </summary>
            System,

            /// <summary>
            /// A notification e-mail.
            /// </summary>
            Notification

        }

        /// <summary>
        /// The common header of HTML notification e-mails.
        /// </summary>
        public virtual String HTMLEMailHeader(String     ExternalDNSName,
                                              HTTPPath?  BasePath,
                                              EMailType  EMailType)

            => String.Concat("<!DOCTYPE html>\r\n",
                             "<html>\r\n",
                               "<head>\r\n",
                                   "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\" />\r\n",
                               "</head>\r\n",
                               "<body style=\"background-color: #ececec\">\r\n",
                                 "<div style=\"width: 600px\">\r\n",
                                   "<div style=\"border-bottom: 1px solid #AAAAAA; margin-bottom: 20px\">\r\n",
                                       "<img src=\"", ExternalDNSName, (BasePath?.ToString() ?? ""), "\" style=\"width: 250px; padding-right: 10px\" alt=\"Organization\">\r\n",
                                   "</div>\r\n",
                                   "<div style=\"border-bottom: 1px solid #AAAAAA; padding-left: 6px; padding-bottom: 40px; margin-bottom: 10px;\">\r\n");


        /// <summary>
        /// The common footer of HTML notification e-mails.
        /// </summary>
        public virtual String HTMLEMailFooter(String     ExternalDNSName,
                                              HTTPPath?  BasePath,
                                              EMailType  EMailType)

            => String.Concat(      "</div>\r\n",
                                   EMailType == EMailType.Notification
                                       ? "<div style=\"color: #AAAAAA; font-size: 80%; padding-bottom: 10px\">\r\n" +
                                             "If you no longer wish to receive this kind of notification e-mails you can unsubscribe <a href=\"https://" + ExternalDNSName + (BasePath?.ToString() ?? "") + "/notifications\">here</a>.<br />\r\n" +
                                         "</div>\r\n"
                                       : "",
                                   "<div style=\"color: #AAAAAA; font-size: 70%\">\r\n",
                                       "(c) GraphDefined GmbH<br />\r\n",
                                   "</div>\r\n",
                                 "</div>\r\n",
                               "</body>\r\n",
                             "</html>\r\n\r\n");


        /// <summary>
        /// The common header of plain text notification e-mails.
        /// </summary>
        public virtual String TextEMailHeader(String     ExternalDNSName,
                                              HTTPPath?  BasePath,
                                              EMailType  EMailType)

            => String.Concat("GraphDefined Users API\r\n",
                             "----------------------\r\n\r\n");


        /// <summary>
        /// The common footer of plain text notification e-mails.
        /// </summary>
        public virtual String TextEMailFooter(String     ExternalDNSName,
                                              HTTPPath?  BasePath,
                                              EMailType  EMailType)

            => String.Concat("\r\n\r\n---------------------------------------------------------------\r\n",
                             EMailType == EMailType.Notification
                                 ? "If you no longer wish to receive this kind of notification e-mails you can unsubscribe here: https://" + ExternalDNSName + (BasePath?.ToString() ?? "") + "/notifications.\r\n\r\n"
                                 : "",
                             "Users API\r\n",
                             "(c) GraphDefined GmbH\r\n\r\n");

        #endregion

        #region E-Mail templates

        #region NewUserSignUpEMailCreatorDelegate

        /// <summary>
        /// A delegate for sending a sign-up e-mail to a new user.
        /// </summary>
        public virtual EMail NewUserSignUpEMailCreator(IUser             User,
                                                       EMailAddressList  EMailRecipients,
                                                       SecurityToken_Id  SecurityToken,
                                                       Boolean           Use2FactorAuth,
                                                       Languages         Language,
                                                       EventTracking_Id? EventTrackingId)

            =>  new HTMLEMailBuilder() {

                    From           = Robot.EMail,
                    To             = EMailRecipients,
                    Passphrase     = APIRobotGPGPassphrase,
                    Subject        = "Your " + ServiceName + " account has been created",

                    HTMLText       = String.Concat(
                                         HTMLEMailHeader(ExternalDNSName, BasePath, EMailType.System),
                                             "Dear ", User.Name, ",<br /><br />" + Environment.NewLine,
                                             "your " + ServiceName + " account has been created!<br /><br />" + Environment.NewLine,
                                             "Please click the following link to set a new password for your account" + (Use2FactorAuth ? " and check your mobile phone for an additional security token" : "") + "...<br /><br />" + Environment.NewLine,
                                             "<a href=\"https://" + ExternalDNSName + (BasePath?.ToString() ?? "") + "/setPassword?" + SecurityToken + (Use2FactorAuth ? "&2factor" : "") + "\" style=\"text-decoration: none; color: #FFFFFF; background-color: #ff7300; Border: solid #ff7300; border-width: 10px 20px; line-height: 2; font-weight: bold; text-align: center; cursor: pointer; display: inline-block; border-radius: 4px; margin-top: 20px; font-size: 70%\">Set a new password</a>" + Environment.NewLine,
                                         HTMLEMailFooter(ExternalDNSName, BasePath, EMailType.System)
                                     ),

                    PlainText      = String.Concat(
                                         TextEMailHeader(ExternalDNSName, BasePath, EMailType.System) +
                                             "Dear ", User.Name, ", " + Environment.NewLine +
                                             "your " + ServiceName + " account has been created!" + Environment.NewLine + Environment.NewLine +
                                             "Please click the following link to set a new password for your account" + (Use2FactorAuth ? " and check your mobile phone for an additional security token" : "") + "..." + Environment.NewLine + Environment.NewLine +
                                             "https://" + ExternalDNSName + (BasePath?.ToString() ?? "") + "/setPassword?" + SecurityToken + (Use2FactorAuth ? "&2factor" : "") +
                                         TextEMailFooter(ExternalDNSName, BasePath, EMailType.System)
                                    ),

                    SecurityLevel  = EMailSecurity.autosign

                }.AsImmutable;

        #endregion

        #region NewUserWelcomeEMailCreatorDelegate

        /// <summary>
        /// A delegate for sending a welcome e-mail to a new user.
        /// </summary>
        public virtual EMail NewUserWelcomeEMailCreator(IUser              User,
                                                        EMailAddressList   EMailRecipients,
                                                        Languages          Language,
                                                        EventTracking_Id?  EventTrackingId)

            => new HTMLEMailBuilder() {

                   From           = Robot.EMail,
                   To             = EMailRecipients,
                   Passphrase     = APIRobotGPGPassphrase,
                   Subject        = "Welcome to " + ServiceName + "...",

                   HTMLText       = String.Concat(
                                        HTMLEMailHeader(ExternalDNSName, BasePath, EMailType.System) +
                                            "Dear " + User.Name + ",<br /><br />" + Environment.NewLine +
                                            "welcome to your new " + ServiceName + " account!<br /><br />" + Environment.NewLine +
                                            "<a href=\"https://" + ExternalDNSName + (BasePath?.ToString() ?? "") + "/login\" style=\"text-decoration: none; color: #FFFFFF; background-color: #ff7300; Border: solid #ff7300; border-width: 10px 20px; line-height: 2; font-weight: bold; text-align: center; cursor: pointer; display: inline-block; border-radius: 4px; margin-top: 20px; font-size: 70%\">Login</a>" + Environment.NewLine +
                                        HTMLEMailFooter(ExternalDNSName, BasePath, EMailType.System)
                                    ),

                   PlainText      = String.Concat(
                                        TextEMailHeader(ExternalDNSName, BasePath, EMailType.System) +
                                            "Dear " + User.Name + "," + Environment.NewLine +
                                            "welcome to your new " + ServiceName + " account!" + Environment.NewLine + Environment.NewLine +
                                            "Please login via: https://" + ExternalDNSName + (BasePath?.ToString() ?? "") + "/login" + Environment.NewLine + Environment.NewLine +
                                        TextEMailFooter(ExternalDNSName, BasePath, EMailType.System)
                                    ),

                   SecurityLevel  = EMailSecurity.autosign

               }.//AddAttachment("Hi there!".ToUTF8Bytes(), "welcome.txt", MailContentTypes.text_plain).
                 AsImmutable;

        #endregion

        #region ResetPasswordEMailCreatorDelegate

        /// <summary>
        /// A delegate for sending a reset password e-mail to a user.
        /// </summary>
        public virtual EMail ResetPasswordEMailCreator(IUser              User,
                                                       EMailAddressList   EMailRecipients,
                                                       SecurityToken_Id   SecurityToken,
                                                       Boolean            Use2FactorAuth,
                                                       Languages          Language,
                                                       EventTracking_Id?  EventTrackingId)

            => new HTMLEMailBuilder() {

                   From           = Robot.EMail,
                   To             = EMailRecipients,
                   Passphrase     = APIRobotGPGPassphrase,
                   Subject        = ServiceName + " password reset...",

                   HTMLText       = String.Concat(
                                        HTMLEMailHeader(ExternalDNSName, BasePath, EMailType.System) +
                                            "Dear " + User.Name + ",<br /><br />" + Environment.NewLine +
                                            "someone - hopefully you - requested us to change your password!<br />" + Environment.NewLine +
                                            "If this request was your intention, please click the following link to set a new password...<br /><br />" + Environment.NewLine +
                                            "<a href=\"https://" + ExternalDNSName + (BasePath?.ToString() ?? "") + "/setPassword?" + SecurityToken + (Use2FactorAuth ? "&2factor" : "") + "\" style=\"text-decoration: none; color: #FFFFFF; background-color: #ff7300; Border: solid #ff7300; border-width: 10px 20px; line-height: 2; font-weight: bold; text-align: center; cursor: pointer; display: inline-block; border-radius: 4px; margin-top: 20px; font-size: 70%\">Set a new password</a>" + Environment.NewLine +
                                        HTMLEMailFooter(ExternalDNSName, BasePath, EMailType.System)
                                    ),

                   PlainText      = String.Concat(
                                        TextEMailHeader(ExternalDNSName, BasePath, EMailType.System) +
                                            "Dear " + User.Name + "," + Environment.NewLine +
                                            "someone - hopefully you - requested us to change your password!" + Environment.NewLine +
                                            "If this request was your intention, please click the following link to set a new password..." + Environment.NewLine + Environment.NewLine +
                                            "https://" + ExternalDNSName + (BasePath?.ToString() ?? "") + "/setPassword?" + SecurityToken + (Use2FactorAuth ? "&2factor" : "") +
                                        TextEMailFooter(ExternalDNSName, BasePath, EMailType.System)
                                    ),

                   SecurityLevel  = EMailSecurity.autosign

               }.AsImmutable;

        #endregion

        #region PasswordChangedEMailCreatorDelegate

        /// <summary>
        /// A delegate for sending a reset password e-mail to a user.
        /// </summary>
        public virtual EMail PasswordChangedEMailCreator(IUser              User,
                                                         EMailAddressList   EMailRecipients,
                                                         Languages          Language,
                                                         EventTracking_Id?  EventTrackingId)

            => new HTMLEMailBuilder() {

                   From           = Robot.EMail,
                   To             = EMailRecipients,
                   Passphrase     = APIRobotGPGPassphrase,
                   Subject        = "Your " + ServiceName + " password changed...",

                   HTMLText       = String.Concat(
                                        HTMLEMailHeader(ExternalDNSName, BasePath, EMailType.System) +
                                            "Dear " + User.Name + ",<br /><br />" + Environment.NewLine +
                                            "your password has successfully been changed!<br />" + Environment.NewLine +
                                            "<a href=\"https://" + ExternalDNSName + (BasePath?.ToString() ?? "") + "/login?" + User.Id + "\" style=\"text-decoration: none; color: #FFFFFF; background-color: #ff7300; Border: solid #ff7300; border-width: 10px 20px; line-height: 2; font-weight: bold; text-align: center; cursor: pointer; display: inline-block; border-radius: 4px; margin-top: 20px; font-size: 70%\">Login</a>" + Environment.NewLine +
                                        HTMLEMailFooter(ExternalDNSName, BasePath, EMailType.System)
                                    ),

                   PlainText      = String.Concat(
                                        TextEMailHeader(ExternalDNSName, BasePath, EMailType.System) +
                                            "Dear " + User.Name + "," + Environment.NewLine +
                                            "your password has successfully been changed!" + Environment.NewLine +
                                            "https://" + ExternalDNSName + (BasePath?.ToString() ?? "") + "/login?" + User.Id +
                                        TextEMailFooter(ExternalDNSName, BasePath, EMailType.System)
                                    ),

                   SecurityLevel  = EMailSecurity.autosign

               }.AsImmutable;

        #endregion


        #region NewServiceTicketMessageReceivedDelegate

        ///// <summary>
        ///// A delegate for sending e-mail notifications about received service ticket messages to users.
        ///// </summary>
        //public delegate EMail NewServiceTicketMessageReceivedDelegate(ServiceTicket    ParsedMessage,
        //                                                              EMailAddressList  EMailRecipients);

        //private static readonly Func<String, EMailAddress, String, NewServiceTicketMessageReceivedDelegate>

        //    __NewServiceTicketMessageReceivedDelegate = (BaseURL,
        //                                                 APIEMailAddress,
        //                                                 APIPassphrase)

        //        => (ParsedMessage,
        //            EMailRecipients)

        //            =>  new HTMLEMailBuilder() {

        //                    From            = APIEMailAddress,
        //                    To              = EMailAddressListBuilder.Create(EMailRecipients),
        //                    Passphrase      = APIPassphrase,
        //                    Subject         = "...", //ParsedMessage.EMailSubject,

        //                    HTMLText        = HTMLEMailHeader +
        //                                          //ParsedMessage.EMailBody.Replace("\r\n", "<br />\r\n") + Environment.NewLine +
        //                                      HTMLEMailFooter,

        //                    PlainText       = TextEMailHeader +
        //                                          //ParsedMessage.EMailBody + Environment.NewLine +
        //                                      TextEMailFooter,

        //                    SecurityLevel   = EMailSecurity.autosign

        //                };

        #endregion

        #region ServiceTicketStatusChangedEMailDelegate

        ///// <summary>
        ///// A delegate for sending e-mail notifications about service ticket status changes to users.
        ///// </summary>
        //public delegate EMail ServiceTicketStatusChangedEMailDelegate(ServiceTicket                         ServiceTicket,
        //                                                              Timestamped<ServiceTicketStatusTypes>  OldStatus,
        //                                                              Timestamped<ServiceTicketStatusTypes>  NewStatus,
        //                                                              EMailAddressList                       EMailRecipients);

        //private static readonly Func<String, EMailAddress, String, ServiceTicketStatusChangedEMailDelegate>

        //    __ServiceTicketStatusChangedEMailDelegate = (BaseURL,
        //                                                 APIEMailAddress,
        //                                                 APIPassphrase)

        //        => (ServiceTicket,
        //            OldStatus,
        //            NewStatus,
        //            EMailRecipients)

        //            => new HTMLEMailBuilder() {

        //                From           = APIEMailAddress,
        //                To             = EMailAddressListBuilder.Create(EMailRecipients),
        //                Passphrase     = APIPassphrase,
        //                Subject        = String.Concat("ServiceTicket '",        ServiceTicket.Id,
        //                                               "' status change from '", OldStatus.Value,
        //                                               " to '",                  NewStatus.Value, "'!"),

        //                HTMLText       = String.Concat(HTMLEMailHeader,
        //                                               "The status of service ticket <b>'", ServiceTicket.Id, "'</b> (Owner: '", ServiceTicket.Author,
        //                                               "'), changed from <i>'", OldStatus.Value, "'</i> (since ", OldStatus.Timestamp.ToIso8601(), ") ",
        //                                               " to <i>'", NewStatus.Value, "'</i>!<br /><br />",
        //                                               HTMLEMailFooter),

        //                PlainText      = String.Concat(TextEMailHeader,
        //                                               "The status of service ticket '", ServiceTicket.Id, "' (Owner: '", ServiceTicket.Author,
        //                                               "'), changed from '", OldStatus.Value, "' (since ", OldStatus.Timestamp.ToIso8601(), ") ",
        //                                               " to '", NewStatus.Value, "'!\r\r\r\r",
        //                                               TextEMailFooter),

        //                SecurityLevel  = EMailSecurity.autosign

        //            };

        #endregion

        #region ServiceTicketChangedEMailDelegate

        ///// <summary>
        ///// A delegate for sending e-mail notifications about service ticket changes to users.
        ///// </summary>
        //public delegate EMail ServiceTicketChangedEMailDelegate(ServiceTicket             ServiceTicket,
        //                                                        NotificationMessageType    MessageType,
        //                                                        NotificationMessageType[]  AdditionalMessageTypes,
        //                                                        EMailAddressList           EMailRecipients);

        //private static readonly Func<String, EMailAddress, String, ServiceTicketChangedEMailDelegate>

        //    __ServiceTicketChangedEMailDelegate = (BaseURL,
        //                                           APIEMailAddress,
        //                                           APIPassphrase)

        //        => (ServiceTicket,
        //            MessageType,
        //            AdditionalMessageTypes,
        //            EMailRecipients)

        //            => new HTMLEMailBuilder() {

        //                From           = APIEMailAddress,
        //                To             = EMailAddressListBuilder.Create(EMailRecipients),
        //                Passphrase     = APIPassphrase,
        //                Subject        = String.Concat("ServiceTicket data '", ServiceTicket.Id, "' was changed'!"),

        //                HTMLText       = String.Concat(HTMLEMailHeader,
        //                                               "The data of service ticket <b>'", ServiceTicket.Id, "'</b> (Owner: '", ServiceTicket.Author,
        //                                               "') was changed!<br /><br />",
        //                                               HTMLEMailFooter),

        //                PlainText      = String.Concat(TextEMailHeader,
        //                                               "The data of service ticket '", ServiceTicket.Id, "' (Owner: '", ServiceTicket.Author,
        //                                               "') was changed!\r\r\r\r",
        //                                               TextEMailFooter),

        //                SecurityLevel  = EMailSecurity.autosign

        //            };

        #endregion

        #endregion

        #region (protected) SendSMS(Text, To, Sender = null)

        /// <summary>
        /// Send a SMS to the given phone number.
        /// </summary>
        /// <param name="Text">The text of the SMS.</param>
        /// <param name="To">The phone number of the recipient.</param>
        /// <param name="Sender">An optional sender name.</param>
        protected virtual SMSAPIResp.SMSAPIResponseStatus SendSMS(String   Text,
                                                                  String   To,
                                                                  String?  Sender  = null)
        {

            if (SMSClient is not null)
            {

                var smsSend = SMSClient.Send(Text, To);

                if (smsSend != null)
                    return smsSend.SetSender(Sender ?? SMSSenderName).Execute();

            }

            return SMSAPIResp.SMSAPIResponseStatus.Failed("No SMSAPI defined!");

        }

        /// <summary>
        /// Send a SMS to the given phone number.
        /// </summary>
        /// <param name="Text">The text of the SMS.</param>
        /// <param name="To">The phone numbers of the recipients.</param>
        /// <param name="Sender">An optional sender name.</param>
        protected SMSAPIResp.SMSAPIResponseStatus SendSMS(String    Text,
                                                          String[]  To,
                                                          String?   Sender  = null)
        {

            if (SMSClient is not null)
            {

                var smsSend = SMSClient.Send(Text, To);

                if (smsSend is not null)
                    return smsSend.SetSender(Sender ?? SMSSenderName).Execute();

            }

            return SMSAPIResp.SMSAPIResponseStatus.Failed("No SMSAPI defined!");

        }

        #endregion

        #region (protected) SendHTTPSNotifications(AllNotificationURLs, JSONNotification)

        #region (protected) LogRequest(...)

        protected Task LogRequest(DateTime     Timestamp,
                                  IHTTPClient  Client,
                                  String       RemoteHost,
                                  HTTPRequest  Request)
        {

            return Task.Run(() => {

                using (var Logfile = File.AppendText(this.NotificationsPath +
                                                     RemoteHost + "-Requests-" + Request.Timestamp.ToString("yyyy-MM") + ".log"))
                {

                    Logfile.WriteLine(
                        String.Concat(Request.HTTPSource.ToString(), Environment.NewLine,
                                      Request.Timestamp.ToIso8601(), Environment.NewLine,
                                      Request.EventTrackingId, Environment.NewLine,
                                      Request.EntirePDU, Environment.NewLine,
                                      "======================================================================================"));

                }

            });

        }

        #endregion

        #region (protected) LogResponse(...)

        protected Task LogResponse(DateTime      Timestamp,
                                   IHTTPClient   Client,
                                   String        RemoteHost,
                                   HTTPRequest   Request,
                                   HTTPResponse  Response)
        {

            return Task.Run(() => {

                Response.AppendToLogfile(this.NotificationsPath +
                                         RemoteHost + "-Responses-" + Request.Timestamp.ToString("yyyy-MM") + ".log");

            });

        }

        #endregion

        protected async Task SendHTTPSNotifications(IEnumerable<HTTPSNotification>  AllNotifications,
                                                    JObject                         JSONNotification)
        {

            if (!DisableNotifications)
            {

                try
                {

                    foreach (var notification in AllNotifications)
                    {

                        HTTPRequest? request     = null;
                        HTTPResponse result      = HTTPResponse.ClientError(request);
                        Byte TransmissionRetry   = 0;
                        Byte MaxNumberOfRetries  = 3;

                        do
                        {

                            try
                            {

                                using (var httpsClient = new HTTPSClient(
                                                             notification.RemoteURL,
                                                             RemoteCertificateValidator:  notification.RemoteURL.Protocol == URLProtocols.https
                                                                                              ? (sender, certificate, chain, policyErrors) => (true, Array.Empty<String>())
                                                                                              : null,
                                                             ClientCertificateSelector:   null,
                                                             ClientCert:                  null,
                                                             HTTPUserAgent:               null,
                                                             RequestTimeout:              null,
                                                             DNSClient:                   DNSClient
                                                         ))
                                {

                                    DebugX.Log("Sending HTTPS-notification to: " + notification.RemoteURL);

                                    request  = new HTTPRequest.Builder(httpsClient) {
                                                   HTTPMethod     = notification.Method,
                                                   Host           = notification.RemoteURL.Hostname,
                                                   Path           = notification.RemoteURL.Path,
                                                   Content        = new JArray(JSONNotification).ToUTF8Bytes(),
                                                   ContentType    = HTTPContentType.JSON_UTF8,
                                                   UserAgent      = "UsersAPI Notification API",
                                                   API_Key        = notification.APIKey.HasValue
                                                                        ? notification.APIKey
                                                                        : null,
                                                   Authorization  = notification.BasicAuthenticationLogin.IsNotNullOrEmpty()
                                                                        ? HTTPBasicAuthentication.Create(
                                                                              notification.BasicAuthenticationLogin,
                                                                              notification.BasicAuthenticationPassword
                                                                          )
                                                                        : null
                                    };

                                    result  = await httpsClient.Execute(Request:              request,
                                                                        RequestLogDelegate:   (timestamp, client, req)       => LogRequest (timestamp, client, notification.RemoteURL.Hostname.ToString(), req),
                                                                        ResponseLogDelegate:  (timestamp, client, req, resp) => LogResponse(timestamp, client, notification.RemoteURL.Hostname.ToString(), req, resp),
                                                                        CancellationToken:    default,
                                                                        EventTrackingId:      EventTracking_Id.New,
                                                                        RequestTimeout:       notification.RequestTimeout,
                                                                        NumberOfRetry:        TransmissionRetry++);

                                }

                                result ??= HTTPResponse.ClientError(request);

                            }
                            catch (Exception e)
                            {
                                DebugX.LogException(e, "SendHTTPSNotification");
                            }

                        }
                        // Try to resend the HTTP message, when there had been networking errors...
                        while (result.HTTPStatusCode == HTTPStatusCode.RequestTimeout &&
                               TransmissionRetry++ < MaxNumberOfRetries);

                        // If it failed: Write entire message on disc/logfile
                        //               Reread the logfile later and try to resend the message!

                    }

                }
                catch (Exception e)
                {
                    DebugX.LogException(e, "SendHTTPSNotifications");
                }

            }

        }

        #endregion

        #region ReceiveTelegramMessage(Sender, e)

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
                        await this.TelegramClient.SendTextMessageAsync(
                            ChatId:  e.Message.Chat,
                            Text:    "I'm running on: " + Environment.MachineName + " and use " + (Environment.WorkingSet / 1024 /1024) + " MBytes RAM"
                        );
                        break;

                    case "/echo":
                        await this.TelegramClient.SendTextMessageAsync(
                            ChatId:  e.Message.Chat,
                            Text:    "Hello " + e.Message.From.FirstName + " " + e.Message.From.LastName + "!\nYou said:\n" + e.Message.Text
                        );
                        break;

                }

            }
        }

        #endregion

        #endregion

        #region (private) RegisterURLTemplates()

        #region (private)   GenerateCookieUserData(ValidUser, Astronaut = null)

        private String GenerateCookieUserData(IUser   User,
                                              IUser?  Astronaut  = null)

            => String.Concat("=login=",            User.     Id.      ToString().ToBase64(),
                             Astronaut is not null
                                 ? ":astronaut=" + Astronaut.Id.      ToString().ToBase64()
                                 : "",
                             ":username=",         User.Name.FirstText().ToBase64(),
                             ":email=",            User.EMail.Address.ToString().ToBase64(),
                             ":language=",         User.UserLanguage. AsText().  ToBase64(),
                             IsAdmin(User) == Access_Levels.ReadOnly  ? ":isAdminRO" : "",
                             IsAdmin(User) == Access_Levels.ReadWrite ? ":isAdminRW" : "");


        #endregion

        #region (private)   GenerateCookieSettings(Expires)

        private String GenerateCookieSettings(DateTime Expires)

            => String.Concat("; Expires=",  Expires.ToRfc1123(),
                             HTTPCookieDomain.IsNotNullOrEmpty()
                                 ? "; Domain=" + HTTPCookieDomain
                                 : "",
                             "; Path=",     URLPathPrefix.ToString(),
                             "; SameSite=strict",
                             UseSecureCookies
                                 ? "; secure"
                                 : "");

        #endregion

        #region (protected) TryGetSecurityTokenFromCookie(Request)

        protected SecurityToken_Id? TryGetSecurityTokenFromCookie(HTTPRequest Request)
        {

            if (Request.Cookies is null)
                return null;

            if (Request.Cookies. TryGet  (SessionCookieName,  out var cookie) &&
                cookie is not null &&
                cookie.Any() &&
                SecurityToken_Id.TryParse(cookie.First().Key, out var securityTokenId))
            {
                return securityTokenId;
            }

            return null;

        }

        #endregion

        #region (protected) TryGetSecurityTokenFromCookie(Request, SecurityTokenId)

        protected Boolean TryGetSecurityTokenFromCookie(HTTPRequest Request, out SecurityToken_Id SecurityTokenId)
        {

            if (Request.Cookies  is not null &&
                Request.Cookies. TryGet  (SessionCookieName,           out var cookie) &&
                cookie is not null &&
                SecurityToken_Id.TryParse(cookie.FirstOrDefault().Key, out     SecurityTokenId))
            {
                return true;
            }

            SecurityTokenId = default;
            return false;

        }

        #endregion

        #region (protected) TryGetHTTPUser (Request, out User)

        protected Boolean TryGetHTTPUser(HTTPRequest Request, out IUser? User)
        {

            #region Get user from cookie...

            if (Request.Cookies is not null                                                            &&
                Request.Cookies. TryGet     (SessionCookieName,           out var cookie)              &&
                cookie is not null &&
                SecurityToken_Id.TryParse   (cookie.FirstOrDefault().Key, out var securityTokenId)     &&
                _HTTPCookies.    TryGetValue(securityTokenId,             out var securityInformation) &&
                Timestamp.Now < securityInformation.Expires                                            &&
                TryGetUser(securityInformation.UserId, out User))
            {
                return true;
            }

            #endregion

            #region Get user from Basic-Auth...

            if (Request.Authorization is HTTPBasicAuthentication basicAuth)
            {

                #region Find username or e-mail addresses...

                var possibleUsers = new HashSet<IUser>();
                var validUsers    = new HashSet<IUser>();

                if (User_Id.TryParse   (basicAuth.Username, out var _UserId) &&
                    users.  TryGetValue(_UserId,            out var _User))
                {
                    possibleUsers.Add(_User);
                }

                if (possibleUsers.Count == 0)
                {
                    foreach (var user2 in users.Values)
                    {
                        if (String.Equals(basicAuth.Username,
                                          user2.EMail.Address.ToString(),
                                          StringComparison.OrdinalIgnoreCase))
                        {
                            possibleUsers.Add(user2);
                        }
                    }
                }

                if (possibleUsers.Count > 0)
                {
                    foreach (var possibleUser in possibleUsers)
                    {
                        if (loginPasswords.TryGetValue(possibleUser.Id, out var loginPassword) &&
                            loginPassword.VerifyPassword(basicAuth.Password))
                        {
                            validUsers.Add(possibleUser);
                        }
                    }
                }

                #endregion

                #region HTTP Basic Auth is ok!

                var user = validUsers.FirstOrDefault();

                if (user is not null &&
                    user.AcceptedEULA.HasValue &&
                    user.AcceptedEULA.Value < Timestamp.Now)
                {
                    User = user;
                    return true;
                }

                #endregion

            }

            #endregion

            #region Get user from API Key...

            if (TryGetValidAPIKey(Request.API_Key, out var apiKey) &&
                apiKey is not null &&
                TryGetUser(apiKey.UserId, out User))
            {
                return true;
            }

            #endregion

            User = null;
            return false;

        }

        #endregion

        #region (protected) TryGetAstronaut(Request, out User)

        protected Boolean TryGetAstronaut(HTTPRequest Request, out IUser? User)
        {

            // Get user from cookie...
            if (Request.Cookies is not null                                                            &&
                Request.Cookies. TryGet     (SessionCookieName,           out var cookie)              &&
                cookie is not null &&
                SecurityToken_Id.TryParse   (cookie.FirstOrDefault().Key, out var securityTokenId)     &&
                _HTTPCookies.    TryGetValue(securityTokenId,             out var securityInformation) &&
                Timestamp.Now < securityInformation.Expires                                            &&
                TryGetUser(securityInformation.SuperUserId ?? securityInformation.UserId, out User))
            {
                return true;
            }

            User = null;
            return false;

        }

        #endregion

        #region (protected) TryGetHTTPUser (Request, User, Organizations, Response, AccessLevel = ReadOnly, Recursive = false)

        protected Boolean TryGetHTTPUser(HTTPRequest                 Request,
                                         out IUser?                  User,
                                         out HashSet<IOrganization>  Organizations,
                                         out HTTPResponse.Builder?   Response,
                                         Access_Levels               AccessLevel  = Access_Levels.ReadOnly,
                                         Boolean                     Recursive    = false)
        {

            //if (Request.RemoteSocket.IPAddress.IsIPv4 &&
            //    Request.RemoteSocket.IPAddress.IsLocalhost)
            //{
            //    User           = Admins.User2GroupInEdges(edgelabel => edgelabel == User2GroupEdges.IsAdmin).FirstOrDefault()?.Source;
            //    Organizations  = User.Organizations(RequireReadWriteAccess, Recursive);
            //    Response       = null;
            //    return true;
            //}

            Organizations  = TryGetHTTPUser(Request, out User) && User is not null
                                 ? new HashSet<IOrganization>(User.Organizations(AccessLevel, Recursive))
                                 : new HashSet<IOrganization>();

            Response       = Organizations.SafeAny()
                                 ? null
                                 : new HTTPResponse.Builder(Request) {
                                       HTTPStatusCode      = HTTPStatusCode.Unauthorized,
                                       Location            = Location.From(URLPathPrefix + "login?redirect=" + Request.Path.ToString()),
                                       Date                = Timestamp.Now,
                                       Server              = HTTPServer.DefaultServerName,
                                       CacheControl        = "private, max-age=0, no-cache",
                                       XLocationAfterAuth  = Request.Path,
                                       Connection          = "close"
                                   };

            return Organizations.SafeAny();

        }

        #endregion

        #region (protected) TryGetAstronaut(Request, User, Organizations, Response, AccessLevel = ReadOnly, Recursive = false)

        protected Boolean TryGetSuperUser(HTTPRequest                 Request,
                                          out IUser?                  User,
                                          out HashSet<IOrganization>  Organizations,
                                          out HTTPResponse.Builder?   Response,
                                          Access_Levels               AccessLevel  = Access_Levels.ReadOnly,
                                          Boolean                     Recursive    = false)
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

                Organizations  = new HashSet<IOrganization>();
                Response       = new HTTPResponse.Builder(Request) {
                                     HTTPStatusCode  = HTTPStatusCode.Unauthorized,
                                     Location        = Location.From(URLPathPrefix + "login"),
                                     Date            = Timestamp.Now,
                                     Server          = HTTPServer.DefaultServerName,
                                     CacheControl    = "private, max-age=0, no-cache",
                                     Connection      = "close"
                                 };

                return false;

            }

            Organizations  = User is not null
                                 ? new HashSet<IOrganization>(User.Organizations(AccessLevel, Recursive))
                                 : new HashSet<IOrganization>();

            Response       = null;

            return true;

        }

        #endregion

        #region (protected) TryGetHTTPUser (Request, User, Organizations,           AccessLevel = ReadOnly, Recursive = false)

        //protected void TryGetHTTPUser(HTTPRequest                Request,
        //                              out User                   User,
        //                              out HashSet<Organization>  Organizations,
        //                              Access_Levels              AccessLevel  = Access_Levels.ReadOnly,
        //                              Boolean                    Recursive    = false)
        //{

        //    if (!TryGetHTTPUser(Request, out User))
        //    {

        //        //if (Request.HTTPSource.IPAddress.IsIPv4 &&
        //        //    Request.HTTPSource.IPAddress.IsLocalhost)
        //        //{
        //        //    User           = AdminOrganization.User2OrganizationInEdges(edge => edge label == User2OrganizationEdgeTypes.IsAdmin).FirstOrDefault()?.Source;
        //        //    Organizations  = new HashSet<Organization>(User.Organizations(AccessLevel, Recursive));
        //        //    return;
        //        //}

        //        Organizations  = null;
        //        return;

        //    }

        //    Organizations  = User != null
        //                         ? new HashSet<Organization>(User.Organizations(AccessLevel, Recursive))
        //                         : new HashSet<Organization>();

        //}

        #endregion

        #region AddEventSource(HTTPEventSourceId, URLTemplate, IncludeFilterAtRuntime, CreateState, ...)

        public void AddEventSource<TData, TState>(HTTPEventSource_Id                              HTTPEventSourceId,
                                                  HTTPAPI                                         HTTPAPI,
                                                  HTTPPath                                        URLTemplate,

                                                  Func<TState, IUser, HTTPEvent<TData>, Boolean>  IncludeFilterAtRuntime,
                                                  Func<TState>                                    CreatePerRequestState,

                                                  HTTPHostname?                                   Hostname                   = null,
                                                  HTTPMethod?                                     HttpMethod                 = null,
                                                  HTTPContentType?                                HTTPContentType            = null,

                                                  HTTPAuthentication?                             URLAuthentication          = null,
                                                  HTTPAuthentication?                             HTTPMethodAuthentication   = null,

                                                  HTTPDelegate?                                   DefaultErrorHandler        = null)
        {

            if (IncludeFilterAtRuntime == null)
                IncludeFilterAtRuntime = (s, u, e) => true;

            if (TryGet(HTTPEventSourceId, out IHTTPEventSource<TData> _EventSource))
            {

                AddMethodCallback(Hostname        ?? HTTPHostname.Any,
                                  HttpMethod      ?? HTTPMethod.GET,
                                  URLTemplate,
                                  HTTPContentType ?? HTTPContentType.EVENTSTREAM,
                                  URLAuthentication:         URLAuthentication,
                                  HTTPMethodAuthentication:  HTTPMethodAuthentication,
                                  DefaultErrorHandler:       DefaultErrorHandler,
                                  HTTPDelegate:              Request => {

                                      #region Get HTTP user and its organizations

                                                 // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                                 if (!TryGetHTTPUser(Request,
                                                                     out var HTTPUser,
                                                                     out var HTTPOrganizations,
                                                                     out var Response,
                                                                     AccessLevel:  Access_Levels.ReadWrite,
                                                                     Recursive:    true))
                                                 {
                                                     return Task.FromResult(Response.AsImmutable);
                                                 }

                                                 #endregion

                                      var State        = CreatePerRequestState != null ? CreatePerRequestState() : default(TState);
                                      var _HTTPEvents  = _EventSource.GetAllEventsGreater(Request.GetHeaderField(HTTPRequestHeaderField.LastEventId)).
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


                AddMethodCallback(Hostname        ?? HTTPHostname.Any,
                                  HttpMethod      ?? HTTPMethod.GET,
                                  URLTemplate,
                                  HTTPContentType ?? HTTPContentType.JSON_UTF8,
                                  URLAuthentication:         URLAuthentication,
                                  HTTPMethodAuthentication:  HTTPMethodAuthentication,
                                  DefaultErrorHandler:       DefaultErrorHandler,
                                  HTTPDelegate:              Request => {

                                      #region Get HTTP user and its organizations

                                      // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                      if (!TryGetHTTPUser(Request,
                                                          out var HTTPUser,
                                                          out var HTTPOrganizations,
                                                          out var Response,
                                                          AccessLevel:  Access_Levels.ReadWrite,
                                                          Recursive:    true))
                                      {
                                          return Task.FromResult(Response.AsImmutable);
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

        #region AddEventSource(HTTPEventSourceId, URLTemplate, IncludeFilterAtRuntime, CreateState, ...)

        public void AddEventSource<TData, TState>(HTTPEventSource_Id                                                          HTTPEventSourceId,
                                                  HTTPAPI                                                                     HTTPAPI,
                                                  HTTPPath                                                                    URLTemplate,

                                                  Func<TState, IUser, IEnumerable<IOrganization>, HTTPEvent<TData>, Boolean>  IncludeFilterAtRuntime,
                                                  Func<TState>                                                                CreatePerRequestState,

                                                  HTTPHostname?                                                               Hostname                   = null,
                                                  HTTPMethod?                                                                 HttpMethod                 = null,
                                                  HTTPContentType?                                                            HTTPContentType            = null,

                                                  HTTPAuthentication?                                                         URLAuthentication          = null,
                                                  HTTPAuthentication?                                                         HTTPMethodAuthentication   = null,

                                                  HTTPDelegate?                                                               DefaultErrorHandler        = null)
        {

            if (IncludeFilterAtRuntime == null)
                IncludeFilterAtRuntime = (s, u, o, e) => true;

            if (TryGet<TData>(HTTPEventSourceId, out var _EventSource))
            {

                AddMethodCallback(Hostname        ?? HTTPHostname.Any,
                                  HttpMethod      ?? HTTPMethod.GET,
                                  URLTemplate,
                                  HTTPContentType ?? HTTPContentType.EVENTSTREAM,
                                  URLAuthentication:         URLAuthentication,
                                  HTTPMethodAuthentication:  HTTPMethodAuthentication,
                                  DefaultErrorHandler:       DefaultErrorHandler,
                                  HTTPDelegate:              Request => {

                                      #region Get HTTP user and its organizations

                                                 // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                                 if (!TryGetHTTPUser(Request,
                                                                     out var HTTPUser,
                                                                     out var HTTPOrganizations,
                                                                     out var Response,
                                                                     AccessLevel:  Access_Levels.ReadWrite,
                                                                     Recursive:    true))
                                                 {
                                                     return Task.FromResult(Response.AsImmutable);
                                                 }

                                                 #endregion

                                      var State        = CreatePerRequestState is not null ? CreatePerRequestState() : default;
                                      var _HTTPEvents  = _EventSource.GetAllEventsGreater(Request.GetHeaderField(HTTPRequestHeaderField.LastEventId)).
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


                AddMethodCallback(Hostname        ?? HTTPHostname.Any,
                                  HttpMethod      ?? HTTPMethod.GET,
                                  URLTemplate,
                                  HTTPContentType ?? HTTPContentType.JSON_UTF8,
                                  URLAuthentication:         URLAuthentication,
                                  HTTPMethodAuthentication:  HTTPMethodAuthentication,
                                  DefaultErrorHandler:       DefaultErrorHandler,
                                  HTTPDelegate:              Request => {

                                      #region Get HTTP user and its organizations

                                      // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                      if (!TryGetHTTPUser(Request,
                                                          out var HTTPUser,
                                                          out var HTTPOrganizations,
                                                          out var Response,
                                                          AccessLevel:  Access_Levels.ReadWrite,
                                                          Recursive:    true))
                                      {
                                          return Task.FromResult(Response.AsImmutable);
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


        #region Manage HTTP Resources

        #region (protected override) GetResourceStream      (ResourceName)

        protected override Stream? GetResourceStream(String ResourceName)

            => GetResourceStream(ResourceName,
                                 new Tuple<String, System.Reflection.Assembly>(UsersAPI.HTTPRoot, typeof(UsersAPI).Assembly),
                                 new Tuple<String, System.Reflection.Assembly>(HTTPAPI. HTTPRoot, typeof(HTTPAPI). Assembly));

        #endregion

        #region (protected override) GetResourceMemoryStream(ResourceName)

        protected override MemoryStream? GetResourceMemoryStream(String ResourceName)

            => GetResourceMemoryStream(ResourceName,
                                       new Tuple<String, System.Reflection.Assembly>(UsersAPI.HTTPRoot, typeof(UsersAPI).Assembly),
                                       new Tuple<String, System.Reflection.Assembly>(HTTPAPI. HTTPRoot, typeof(HTTPAPI). Assembly));

        #endregion

        #region (protected override) GetResourceString      (ResourceName)

        protected override String GetResourceString(String ResourceName)

            => GetResourceString(ResourceName,
                                 new Tuple<String, System.Reflection.Assembly>(UsersAPI.HTTPRoot, typeof(UsersAPI).Assembly),
                                 new Tuple<String, System.Reflection.Assembly>(HTTPAPI. HTTPRoot, typeof(HTTPAPI). Assembly));

        #endregion

        #region (protected override) GetResourceBytes       (ResourceName)

        protected override Byte[] GetResourceBytes(String ResourceName)

            => GetResourceBytes(ResourceName,
                                new Tuple<String, System.Reflection.Assembly>(UsersAPI.HTTPRoot, typeof(UsersAPI).Assembly),
                                new Tuple<String, System.Reflection.Assembly>(HTTPAPI. HTTPRoot, typeof(HTTPAPI). Assembly));

        #endregion

        #region (protected override) MixWithHTMLTemplate    (ResourceName)

        protected override String MixWithHTMLTemplate(String ResourceName)

            => MixWithHTMLTemplate(ResourceName,
                                   new Tuple<String, System.Reflection.Assembly>(UsersAPI.HTTPRoot, typeof(UsersAPI).Assembly),
                                   new Tuple<String, System.Reflection.Assembly>(HTTPAPI. HTTPRoot, typeof(HTTPAPI). Assembly));

        #endregion

        #endregion

        private void RegisterURLTemplates()
        {

            HTTPServer.AddAuth  (request => {

                // Allow OPTIONS requests / call pre-flight requests in cross-origin resource sharing (CORS)
                if (request.HTTPMethod == HTTPMethod.OPTIONS)
                    return Anonymous;

                var user = CheckHTTPCookie   (request, RemoteAuthServersMaxHopCount: 1).Result ??
                           CheckHTTPAPIKey   (request) ??
                           CheckHTTPBasicAuth(request);

                if (user is not null)
                    return user;

                #region Allow some URLs for anonymous access...

                if (request.Path.StartsWith(URLPathPrefix + "/shared/UsersAPI/defaults") ||
                    request.Path.StartsWith(URLPathPrefix + "/shared/UsersAPI/webfonts") ||
                    request.Path.StartsWith(URLPathPrefix + "/shared/UsersAPI/login")    ||
                    request.Path.StartsWith(URLPathPrefix + "/shared/UsersAPI/js")       ||
                    request.Path.StartsWith(URLPathPrefix + "/defaults")                 ||
                    request.Path.StartsWith(URLPathPrefix + "/favicon.ico")              ||

                    request.Path.StartsWith(URLPathPrefix + "/login")                                              ||
                    request.Path.StartsWith(URLPathPrefix + "/css")                                                ||
                    request.Path.StartsWith(URLPathPrefix + "/webfonts")                                           ||
                    request.Path.StartsWith(URLPathPrefix + "/images")                                             ||
                    request.Path.StartsWith(URLPathPrefix + "/lostPassword")                                       ||
                    request.Path.StartsWith(URLPathPrefix + "/resetPassword")                                      ||
                    request.Path.StartsWith(URLPathPrefix + "/setPassword")                                        ||
                    request.Path.StartsWith(URLPathPrefix + "/newsletters")                                        ||
                   (request.Path.StartsWith(URLPathPrefix + "/users/") && request.HTTPMethod.ToString() == "AUTH") ||

                    // Special API keys!
                    request.Path == (URLPathPrefix + "/changeSets")    ||
                    request.Path == (URLPathPrefix + "/securityToken") ||

                   (request.Path == (URLPathPrefix + "/serviceCheck")  && request.HTTPMethod.ToString() == "GET") ||
                   (request.Path == (URLPathPrefix + "/serviceCheck")  && request.HTTPMethod.ToString() == "POST"))
                {

                    return Anonymous;

                }

                #endregion

                return null;

            });

            HTTPServer.AddFilter(request => {

                #region Check EULA

                if (request.User is User user &&
                    (!user.AcceptedEULA.HasValue ||
                      user.AcceptedEULA.Value > Timestamp.Now))
                {
                    return new HTTPResponse.Builder(request) {
                        HTTPStatusCode            = HTTPStatusCode.FailedDependency,
                        Date                      = Timestamp.Now,
                        Server                    = HTTPServer.DefaultServerName,
                        AccessControlAllowOrigin  = "*",
                        AccessControlMaxAge       = 3600,
                        CacheControl              = "private, max-age=0, no-cache",
                        ContentType               = HTTPContentType.JSON_UTF8,
                        Content                   = JSONObject.Create(new JProperty("message", "Please accept the EULA within the portal!")).ToUTF8Bytes(),
                        Connection                = "close"
                    };
                }

                #endregion

                #region Failed/TryNextMethod... redirect web browsers to /login

                if (request.User is null)
                {

                    if (request.HTTPMethod == HTTPMethod.AUTH)
                        return null;

                    if (request.HTTPMethod == HTTPMethod.GET &&
                        request.Accept.BestMatchingContentType(HTTPContentType.HTML_UTF8) == HTTPContentType.HTML_UTF8)
                    {
                        return new HTTPResponse.Builder(request) {
                            HTTPStatusCode      = HTTPStatusCode.TemporaryRedirect,
                            Location            = Location.From(URLPathPrefix + ("/login?redirect=" + request.Path.ToString())),
                            Date                = Timestamp.Now,
                            Server              = HTTPServer.DefaultServerName,
                            //SetCookie           = String.Concat(CookieName, "=; Expires=", Timestamp.Now.ToRfc1123(),
                            //                                    HTTPCookieDomain.IsNotNullOrEmpty()
                            //                                        ? "; Domain=" + HTTPCookieDomain
                            //                                        : "",
                            //                                    "; Path=", URLPathPrefix),
                            XLocationAfterAuth  = request.Path,
                            CacheControl        = "private, max-age=0, no-cache",
                            Connection          = "close"
                        };
                    }

                    else
                        return new HTTPResponse.Builder(request) {
                            HTTPStatusCode            = HTTPStatusCode.Unauthorized,
                            Date                      = Timestamp.Now,
                            Server                    = HTTPServer.DefaultServerName,
                            AccessControlAllowOrigin  = "*",
                            AccessControlMaxAge       = 3600,
                            CacheControl              = "private, max-age=0, no-cache",
                            ContentType               = HTTPContentType.JSON_UTF8,
                            Content                   = JSONObject.Create(new JProperty("message", "Invalid login!")).ToUTF8Bytes(),
                            Connection                = "close"
                        };

                }

                #endregion

                return null;

            });


            #region /shared/UsersAPI

            HTTPServer.RegisterResourcesFolder(this,
                                               HTTPHostname.Any,
                                               URLPathPrefix + "shared/UsersAPI",
                                               HTTPRoot.Substring(0, HTTPRoot.Length - 1),
                                               typeof(UsersAPI).Assembly);

            #endregion


            #region GET         ~/signup

            #region HTML_UTF8

            // -------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/signup
            // -------------------------------------------------------------
            //HTTPServer.AddMethodCallback(HTTPHostname.Any,
            //                             HTTPMethod.GET,
            //                             URLPathPrefix + "signup",
            //                             HTTPContentType.HTML_UTF8,
            //                             HTTPDelegate: async Request => {

            //                                 var _MemoryStream1 = new MemoryStream();
            //                                 GetUsersAPIRessource("template.html").SeekAndCopyTo(_MemoryStream1, 0);
            //                                 var Template = _MemoryStream1.ToArray().ToUTF8String();

            //                                 var _MemoryStream2 = new MemoryStream();
            //                                 typeof(UsersAPI).Assembly.GetManifestResourceStream(HTTPRoot + "SignUp.SignUp-" + DefaultLanguage.ToString() + ".html").SeekAndCopyTo(_MemoryStream2, 0);
            //                                 var HTML     = Template.Replace("<%= content %>",   _MemoryStream2.ToArray().ToUTF8String());

            //                                 //if (LogoImage != null)
            //                                 //    HTML = HTML.Replace("<%= logoimage %>", String.Concat(@"<img src=""", LogoImage, @""" /> "));

            //                                 return new HTTPResponse.Builder(Request) {
            //                                     HTTPStatusCode  = HTTPStatusCode.OK,
            //                                     ContentType     = HTTPContentType.HTML_UTF8,
            //                                     Content         = HTML.ToUTF8Bytes(),
            //                                     Connection      = "close"
            //                                 };

            //                             }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #endregion

            #region GET         ~/verificationtokens/{VerificationToken}

            #region HTML_UTF8

            //// ----------------------------------------------------------------------------------------------
            //// curl -v -H "Accept: text/html" http://127.0.0.1:2100/verificationtokens/0vu04w2hgf0w2h4bv08w
            //// ----------------------------------------------------------------------------------------------
            //HTTPServer.AddMethodCallback(HTTPHostname.Any,
            //                             HTTPMethod.GET,
            //                             HTTPPath.Parse("/verificationtokens/{VerificationToken}"),
            //                             HTTPContentType.HTML_UTF8,
            //                             HTTPDelegate: async Request => {

            //                                 if (VerificationToken. TryParse   (Request.ParsedURLParameters[0], out VerificationToken verificationToken) &&
            //                                    _VerificationTokens.TryGetValue(verificationToken,              out User user))
            //                                 {

            //                                     _VerificationTokens.Remove(verificationToken);

            //                                     await UpdateUser(user.Id,
            //                                                      _user => {
            //                                                          _user.IsAuthenticated = true;
            //                                                      },
            //                                                      null,
            //                                                      Request.EventTrackingId,
            //                                                      Robot.Id);

            //                                     #region Send New-User-Welcome-E-Mail

            //                                     var MailSentResult = MailSentStatus.failed;

            //                                     var NewUserMail = NewUserWelcomeEMailCreator(User:             user,// VerificationToken.Login.ToString(),
            //                                                                                  EMailRecipients:  user.EMail,
            //                                                                                  //DNSHostname:       "https://" + Request.Host.SimpleString,
            //                                                                                  Language:          DefaultLanguage);

            //                                     if (NewUserMail != null)
            //                                     {

            //                                         var MailResultTask = APISMTPClient.Send(NewUserMail);

            //                                         if (MailResultTask.Wait(60000))
            //                                             MailSentResult = MailResultTask.Result;

            //                                         if (MailSentResult == MailSentStatus.ok)
            //                                         {

            //                                             #region Send Admin-Mail...

            //                                             var AdminMail = new TextEMailBuilder() {
            //                                                 From        = Robot.EMail,
            //                                                 To          = APIAdminEMails,
            //                                                 Subject     = "New user activated: " + user.Id.ToString() + " at " + Timestamp.Now.ToString(),
            //                                                 Text        = "New user activated: " + user.Id.ToString() + " at " + Timestamp.Now.ToString(),
            //                                                 Passphrase  = APIPassphrase
            //                                             };

            //                                             var AdminMailResultTask = APISMTPClient.Send(AdminMail).Wait(30000);

            //                                             #endregion

            //                                             return new HTTPResponse.Builder(Request) {
            //                                                        HTTPStatusCode  = HTTPStatusCode.Created,
            //                                                        Server          = HTTPServer.DefaultServerName,
            //                                                        ContentType     = HTTPContentType.JSON_UTF8,
            //                                                        Content         = new JObject(
            //                                                                              new JProperty("@context", ""),
            //                                                                              new JProperty("@id",   user.Id.   ToString()),
            //                                                                              new JProperty("email", user.EMail.ToString())
            //                                                                          ).ToString().ToUTF8Bytes(),
            //                                                        CacheControl    = "public",
            //                                                        //Expires         = "Mon, 25 Jun 2015 21:31:12 GMT",
            //                                                        Connection      = "close"
            //                                                    }.AsImmutable;

            //                                         }

            //                                     }

            //                                     #endregion

            //                                     return new HTTPResponse.Builder(Request) {
            //                                                HTTPStatusCode  = HTTPStatusCode.OK,
            //                                                Server          = HTTPServer.DefaultServerName,
            //                                                ContentType     = HTTPContentType.HTML_UTF8,
            //                                                Content         = ("Account '" + user.Id + "' activated!").ToUTF8Bytes(),
            //                                                CacheControl    = "public",
            //                                                ETag            = "1",
            //                                                //Expires         = "Mon, 25 Jun 2015 21:31:12 GMT",
            //                                                Connection      = "close"
            //                                            }.AsImmutable;

            //                                 }

            //                                 return new HTTPResponse.Builder(Request) {
            //                                            HTTPStatusCode  = HTTPStatusCode.NotFound,
            //                                            Server          = HTTPServer.DefaultServerName,
            //                                            ContentType     = HTTPContentType.HTML_UTF8,
            //                                            Content         = "VerificationToken not found!".ToUTF8Bytes(),
            //                                            CacheControl    = "public",
            //                                            Connection      = "close"
            //                                        }.AsImmutable;

            //                              }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #endregion


            #region GET         ~/login

            // ------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/login
            // ------------------------------------------------------------
            AddMethodCallback(HTTPHostname.Any,
                              HTTPMethod.GET,
                              URLPathPrefix + "login",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request =>

                                 Task.FromResult(
                                     new HTTPResponse.Builder(Request) {
                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                         Server                     = HTTPServer.DefaultServerName,
                                         Date                       = Timestamp.Now,
                                         AccessControlAllowOrigin   = "*",
                                         AccessControlAllowMethods  = new[] { "GET" },
                                         AccessControlAllowHeaders  = new[] { "Content-Type", "Accept", "Authorization" },
                                         ContentType                = HTTPContentType.HTML_UTF8,
                                         Content                    = GetResourceBytes($"login.login-{DefaultLanguage}.html"),
                                         Connection                 = "close"
                                     }.AsImmutable),

                              AllowReplacement: URLReplacement.Allow);

            #endregion

            #region GET         ~/lostPassword

            // -------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/lostPassword
            // -------------------------------------------------------------------
            AddMethodCallback(HTTPHostname.Any,
                              HTTPMethod.GET,
                              URLPathPrefix + "lostPassword",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request =>

                                 Task.FromResult(
                                     new HTTPResponse.Builder(Request) {
                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                         Server                     = HTTPServer.DefaultServerName,
                                         Date                       = Timestamp.Now,
                                         AccessControlAllowOrigin   = "*",
                                         AccessControlAllowMethods  = new[] { "GET" },
                                         AccessControlAllowHeaders  = new[] { "Content-Type", "Accept", "Authorization" },
                                         ContentType                = HTTPContentType.HTML_UTF8,
                                         Content                    = GetResourceBytes($"login.lostPassword-{DefaultLanguage}.html"),
                                         Connection                 = "close"
                                     }.AsImmutable),

                              AllowReplacement: URLReplacement.Allow);

            #endregion

            #region GET         ~/setPassword

            // ------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/setPassword
            // ------------------------------------------------------------------
            AddMethodCallback(HTTPHostname.Any,
                              HTTPMethod.GET,
                              URLPathPrefix + "setPassword",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request =>

                                 Task.FromResult(
                                     new HTTPResponse.Builder(Request) {
                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                         Server                     = HTTPServer.DefaultServerName,
                                         Date                       = Timestamp.Now,
                                         AccessControlAllowOrigin   = "*",
                                         AccessControlAllowMethods  = new[] { "GET" },
                                         AccessControlAllowHeaders  = new[] { "Content-Type", "Accept", "Authorization" },
                                         ContentType                = HTTPContentType.HTML_UTF8,
                                         ContentStream              = GetResourceStream("login.setPassword-" + DefaultLanguage.ToString() + ".html"),
                                         Connection                 = "close"
                                     }.AsImmutable),

                              AllowReplacement: URLReplacement.Allow);

            #endregion


            #region GET         ~/profile

            // --------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/profile
            // --------------------------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.GET,
                              URLPathPrefix + "profile",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out var HTTPUser,
                                                                 out var HTTPOrganizations,
                                                                 out var Response,
                                                                 Recursive: true))
                                             {
                                                 return Task.FromResult(Response.AsImmutable);
                                             }

                                             #endregion


                                  return Task.FromResult(
                                      new HTTPResponse.Builder(Request) {
                                          HTTPStatusCode              = HTTPStatusCode.OK,
                                          Server                      = HTTPServer.DefaultServerName,
                                          Date                        = Timestamp.Now,
                                          AccessControlAllowOrigin    = "*",
                                          AccessControlAllowMethods   = new[] { "GET" },
                                          AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                          ContentType                 = HTTPContentType.HTML_UTF8,
                                          Content                     = MixWithHTMLTemplate("profile.profile.shtml").ToUTF8Bytes(),
                                          Connection                  = "close",
                                          Vary                        = "Accept"
                                      }.AsImmutable);

                              });

            #endregion


            #region ~/users

            #region GET         ~/users

            #region HTML

            // -------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/users
            // -------------------------------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.GET,
                              URLPathPrefix + "users",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its users

                                  // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                  if (!TryGetHTTPUser(Request,
                                                      out var HTTPUser,
                                                      out var HTTPOrganizations,
                                                      out var Response,
                                                      Recursive: true))
                                  {
                                      return Task.FromResult(Response.AsImmutable);
                                  }

                                  #endregion

                                  return Task.FromResult(
                                          new HTTPResponse.Builder(Request) {
                                              HTTPStatusCode             = HTTPStatusCode.OK,
                                              Server                     = HTTPServer.DefaultServerName,
                                              Date                       = Timestamp.Now,
                                              AccessControlAllowOrigin   = "*",
                                              AccessControlAllowMethods  = new[] { "GET" },
                                              AccessControlAllowHeaders  = new[] { "Content-Type", "Accept", "Authorization" },
                                              ContentType                = HTTPContentType.HTML_UTF8,
                                              Content                    = MixWithHTMLTemplate("user.users.shtml").ToUTF8Bytes(),
                                              Connection                 = "close",
                                              Vary                       = "Accept"
                                          }.AsImmutable);

                              });

            #endregion

            #endregion

            #region EXISTS      ~/users/{UserId}

            // ---------------------------------------------------------------------------------
            // curl -v -X EXITS -H "Accept: application/json" http://127.0.0.1:2100/users/ahzf
            // ---------------------------------------------------------------------------------
            HTTPServer.ITEM_EXISTS<User_Id, IUser>(this,
                                                   URLPathPrefix + "users/{UserId}",
                                                   User_Id.TryParse,
                                                   text   => "Invalid user identification '" + text + "'!",
                                                   users.TryGetValue,
                                                   user   => user.PrivacyLevel == PrivacyLevel.World,
                                                   userId => "Unknown user '" + userId + "'!");

            #endregion

            #region GET         ~/users/{UserId}

            #region HTML

            // ------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/users/Organization
            // ------------------------------------------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.GET,
                              URLPathPrefix + "users/{UserId}",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its organizations

                                  // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                  if (!TryGetHTTPUser(Request,
                                                      out var HTTPUser,
                                                      out var HTTPOrganizations,
                                                      out var HTTPResponse,
                                                      Recursive: true))
                                  {
                                      return Task.FromResult(HTTPResponse.AsImmutable);
                                  }

                                  #endregion

                                  #region Check UserId URL parameter

                                  if (!Request.ParseUser(this,
                                                         out var UserId,
                                                         out var User,
                                                         out     HTTPResponse))
                                  {
                                      return Task.FromResult(HTTPResponse.AsImmutable);
                                  }

                                  #endregion


                                  // You request your own profile or you are a valid *admin*
                                  return Task.FromResult(User == HTTPUser || CanImpersonate(HTTPUser,
                                                                                            User,
                                                                                            Access_Levels.ReadOnly)

                                              ? new HTTPResponse.Builder(Request) {
                                                    HTTPStatusCode              = HTTPStatusCode.OK,
                                                    Server                      = HTTPServer.DefaultServerName,
                                                    Date                        = Timestamp.Now,
                                                    AccessControlAllowOrigin    = "*",
                                                    AccessControlAllowMethods   = new[] { "GET" },
                                                    AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                                    ContentType                 = HTTPContentType.HTML_UTF8,
                                                    Content                     = MixWithHTMLTemplate("user.user.shtml").ToUTF8Bytes(),
                                                    Connection                  = "close",
                                                    Vary                        = "Accept"
                                                }.AsImmutable

                                              : new HTTPResponse.Builder(Request) {
                                                    HTTPStatusCode              = HTTPStatusCode.Unauthorized,
                                                    Server                      = HTTPServer.DefaultServerName,
                                                    Date                        = Timestamp.Now,
                                                    AccessControlAllowOrigin    = "*",
                                                    AccessControlAllowMethods   = new[] { "GET" },
                                                    AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                                    Connection                  = "close"
                                                }.AsImmutable);

                              });

            #endregion

            #endregion

            #region GET         ~/users/{UserId}/profilephoto

            HTTPServer.RegisterFilesystemFile(this,
                                              HTTPHostname.Any,
                                              URLPathPrefix + "users/{UserId}/profilephoto",
                                              URLParams => "LocalHTTPRoot/data/Users/" + URLParams[0] + ".png",
                                              DefaultFile: "HTTPRoot/images/defaults/DefaultUser.png");

            #endregion

            #region GET         ~/users/{UserId}/notifications

            #region HTML

            // -------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/users/ahzf/notifications
            // -------------------------------------------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.GET,
                              URLPathPrefix + "users/{UserId}/notifications",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its organizations

                                  // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                  if (!TryGetHTTPUser(Request,
                                                      out var HTTPUser,
                                                      out var HTTPOrganizations,
                                                      out var Response,
                                                      Recursive: true))
                                  {
                                      return Task.FromResult(Response.AsImmutable);
                                  }

                                  #endregion

                                  #region Check UserId URL parameter

                                  if (!Request.ParseUser(this,
                                                         out var UserId,
                                                         out var User,
                                                         out var HTTPResponse))
                                  {
                                      return Task.FromResult(HTTPResponse.AsImmutable);
                                  }

                                  #endregion


                                  // You request your own profile or you are a valid *admin*
                                  return Task.FromResult(User == HTTPUser || CanImpersonate(HTTPUser, User)

                                              ? new HTTPResponse.Builder(Request) {
                                                    HTTPStatusCode              = HTTPStatusCode.OK,
                                                    Server                      = HTTPServer.DefaultServerName,
                                                    Date                        = Timestamp.Now,
                                                    AccessControlAllowOrigin    = "*",
                                                    AccessControlAllowMethods   = new[] { "GET" },
                                                    AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                                    ContentType                 = HTTPContentType.HTML_UTF8,
                                                    Content                     = MixWithHTMLTemplate("user.notifications.shtml").ToUTF8Bytes(),
                                                    Connection                  = "close",
                                                    Vary                        = "Accept"
                                                }.AsImmutable

                                              : new HTTPResponse.Builder(Request) {
                                                    HTTPStatusCode              = HTTPStatusCode.Unauthorized,
                                                    Server                      = HTTPServer.DefaultServerName,
                                                    Date                        = Timestamp.Now,
                                                    AccessControlAllowOrigin    = "*",
                                                    AccessControlAllowMethods   = new[] { "GET" },
                                                    AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                                    Connection                  = "close"
                                                }.AsImmutable);

                              });

            #endregion

            #endregion

            #region GET         ~/users/{UserId}/notifications/{notificationId}

            #region HTML

            // ------------------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/users/ahzf/notifications/{notificationId}
            // ------------------------------------------------------------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.GET,
                              URLPathPrefix + "users/{UserId}/notifications/{notificationId}",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its organizations

                                  // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                  if (!TryGetHTTPUser(Request,
                                                      out var HTTPUser,
                                                      out var HTTPOrganizations,
                                                      out var Response,
                                                      Recursive: true))
                                  {
                                      return Task.FromResult(Response.AsImmutable);
                                  }

                                  #endregion

                                  #region Check UserId URL parameter

                                  if (!Request.ParseUser(this,
                                                         out var UserId,
                                                         out var User,
                                                         out var HTTPResponse))
                                  {
                                      return Task.FromResult(HTTPResponse.AsImmutable);
                                  }

                                  #endregion

                                  #region Get notificationId URL parameter

                                  if (Request.ParsedURLParameters.Length < 2 ||
                                      !UInt32.TryParse(Request.ParsedURLParameters[1], out var NotificationId))
                                  {

                                      return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                     Server          = HTTPServer.DefaultServerName,
                                                     Date            = Timestamp.Now,
                                                     Connection      = "close"
                                                 }.AsImmutable);

                                  }

                                  #endregion


                                  // You request your own profile or you are a valid *admin*
                                  return Task.FromResult(User == HTTPUser || CanImpersonate(HTTPUser, User)

                                              ? new HTTPResponse.Builder(Request) {
                                                    HTTPStatusCode              = HTTPStatusCode.OK,
                                                    Server                      = HTTPServer.DefaultServerName,
                                                    Date                        = Timestamp.Now,
                                                    AccessControlAllowOrigin    = "*",
                                                    AccessControlAllowMethods   = new[] { "GET" },
                                                    AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                                    ContentType                 = HTTPContentType.HTML_UTF8,
                                                    Content                     = MixWithHTMLTemplate("user.editNotification.shtml").ToUTF8Bytes(),
                                                    Connection                  = "close",
                                                    Vary                        = "Accept"
                                                }.AsImmutable

                                              : new HTTPResponse.Builder(Request) {
                                                    HTTPStatusCode              = HTTPStatusCode.Unauthorized,
                                                    Server                      = HTTPServer.DefaultServerName,
                                                    Date                        = Timestamp.Now,
                                                    AccessControlAllowOrigin    = "*",
                                                    AccessControlAllowMethods   = new[] { "GET" },
                                                    AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                                    Connection                  = "close"
                                                }.AsImmutable);

                              });

            #endregion

            #endregion

            #region GET         ~/users/{UserId}/notification/_new

            // ------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:2100/users/ahzf/notification/_new
            // ------------------------------------------------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.GET,
                              URLPathPrefix + "users/{UserId}/notification/_new",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its organizations

                                  // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                  if (!TryGetHTTPUser(Request,
                                                      out var HTTPUser,
                                                      out var HTTPOrganizations,
                                                      out var Response,
                                                      Recursive: true))
                                  {
                                      return Task.FromResult(Response.AsImmutable);
                                  }

                                  #endregion

                                  #region Check UserId URL parameter

                                  if (!Request.ParseUser(this,
                                                         out var UserId,
                                                         out var User,
                                                         out var HTTPResponse))
                                  {
                                      return Task.FromResult(HTTPResponse.AsImmutable);
                                  }

                                  if (User != HTTPUser)
                                  {
                                      return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode              = HTTPStatusCode.Unauthorized,
                                                     Server                      = HTTPServer.DefaultServerName,
                                                     Date                        = Timestamp.Now,
                                                     AccessControlAllowOrigin    = "*",
                                                     AccessControlAllowMethods   = new[] { "GET" },
                                                     AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                                     Connection                  = "close"
                                                 }.AsImmutable);
                                  }

                                  #endregion


                                  return Task.FromResult(
                                      new HTTPResponse.Builder(Request) {
                                          HTTPStatusCode              = HTTPStatusCode.OK,
                                          Server                      = HTTPServer.DefaultServerName,
                                          Date                        = Timestamp.Now,
                                          AccessControlAllowOrigin    = "*",
                                          AccessControlAllowMethods   = new[] { "GET" },
                                          AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                          ContentType                 = HTTPContentType.HTML_UTF8,
                                          Content                     = MixWithHTMLTemplate("user.editNotification.shtml").ToUTF8Bytes(),
                                          Connection                  = "close",
                                          Vary                        = "Accept"
                                      }.AsImmutable);

                              });

            #endregion

            #endregion

            #region ~/userGroups

            #region GET         ~/userGroups

            #region HTML

            // -------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/userGroups
            // -------------------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "userGroups",
                                         HTTPContentType.HTML_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its users

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out var HTTPUser,
                                                                 out var HTTPOrganizations,
                                                                 out var Response,
                                                                 Recursive:                 true))
                                             {
                                                 return Task.FromResult(Response.AsImmutable);
                                             }

                                             #endregion

                                             return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         Server                     = HTTPServer.DefaultServerName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = new[] { "GET" },
                                                         AccessControlAllowHeaders  = new[] { "Content-Type", "Accept", "Authorization" },
                                                         ContentType                = HTTPContentType.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("user.userGroups.shtml").ToUTF8Bytes(),
                                                         Connection                 = "close",
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                         });

            #endregion

            #endregion


            #region GET         ~/userGroups/{UserGroupId}

            #region HTML

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/userGroups/{UserGroupId}
            // --------------------------------------------------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.GET,
                              URLPathPrefix + "userGroups/{UserGroupId}",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its users

                                  // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                  if (!TryGetHTTPUser(Request,
                                                      out var HTTPUser,
                                                      out var HTTPOrganizations,
                                                      out var Response,
                                                      Recursive:                 true))
                                  {
                                      return Task.FromResult(Response.AsImmutable);
                                  }

                                  #endregion

                                  return Task.FromResult(
                                          new HTTPResponse.Builder(Request) {
                                              HTTPStatusCode             = HTTPStatusCode.OK,
                                              Server                     = HTTPServer.DefaultServerName,
                                              Date                       = Timestamp.Now,
                                              AccessControlAllowOrigin   = "*",
                                              AccessControlAllowMethods  = new[] { "GET" },
                                              AccessControlAllowHeaders  = new[] { "Content-Type", "Accept", "Authorization" },
                                              ContentType                = HTTPContentType.HTML_UTF8,
                                              Content                    = MixWithHTMLTemplate("user.userGroup.shtml").ToUTF8Bytes(),
                                              Connection                 = "close",
                                              Vary                       = "Accept"
                                          }.AsImmutable);

                              });

            #endregion

            #endregion

            #endregion


            #region ~/organizations

            #region GET         ~/organizations

            #region HTML

            // ----------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/organizations
            // ----------------------------------------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.GET,
                              URLPathPrefix + "organizations",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its organizations

                                  // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                  if (!TryGetHTTPUser(Request,
                                                      out var HTTPUser,
                                                      out var HTTPOrganizations,
                                                      out var Response,
                                                      Recursive: true))
                                  {
                                      return Task.FromResult(Response.AsImmutable);
                                  }

                                  #endregion

                                  return Task.FromResult(
                                          new HTTPResponse.Builder(Request) {
                                              HTTPStatusCode             = HTTPStatusCode.OK,
                                              Server                     = HTTPServer.DefaultServerName,
                                              Date                       = Timestamp.Now,
                                              AccessControlAllowOrigin   = "*",
                                              AccessControlAllowMethods  = new[] { "OPTIONS", "GET", "COUNT", "ADD" },
                                              AccessControlAllowHeaders  = new[] { "Content-Type", "Accept", "Authorization" },
                                              ContentType                = HTTPContentType.HTML_UTF8,
                                              Content                    = MixWithHTMLTemplate("organization.organizations.shtml").ToUTF8Bytes(),
                                              Connection                 = "close",
                                              Vary                       = "Accept"
                                          }.AsImmutable);

                              });

            #endregion

            #endregion

            #region GET         ~/organizations/{OrganizationId}

            #region HTML

            // ------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/organizations/GraphDefined
            // ------------------------------------------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.GET,
                              URLPathPrefix + "organizations/{OrganizationId}",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its organizations

                                  // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                  if (!TryGetHTTPUser(Request,
                                                      out var HTTPUser,
                                                      out var HTTPOrganizations,
                                                      out var Response,
                                                      Recursive: true))
                                  {
                                      return Task.FromResult(Response.AsImmutable);
                                  }

                                  #endregion

                                  #region Check OrganizationId URL parameter

                                  if (!Request.ParseOrganization(this,
                                                                 out var OrganizationId,
                                                                 out var Organization,
                                                                 out var HTTPResponse))
                                  {
                                      return Task.FromResult(HTTPResponse.AsImmutable);
                                  }

                                  #endregion


                                  if (Organization != null && HTTPOrganizations.Contains(Organization))
                                  {

                                      return Task.FromResult(
                                          new HTTPResponse.Builder(Request) {
                                              HTTPStatusCode              = HTTPStatusCode.OK,
                                              Server                      = HTTPServer.DefaultServerName,
                                              Date                        = Timestamp.Now,
                                              AccessControlAllowOrigin    = "*",
                                              AccessControlAllowMethods   = new[] { "GET" },
                                              AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                              ContentType                 = HTTPContentType.HTML_UTF8,
                                              Content                     = MixWithHTMLTemplate("organization.organization.shtml").ToUTF8Bytes(),
                                              Connection                  = "close",
                                              Vary                        = "Accept"
                                          }.AsImmutable);

                                  }

                                  return Task.FromResult(
                                             new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode              = HTTPStatusCode.Unauthorized,
                                                 Server                      = HTTPServer.DefaultServerName,
                                                 Date                        = Timestamp.Now,
                                                 AccessControlAllowOrigin    = "*",
                                                 AccessControlAllowMethods   = new[] { "GET" },
                                                 AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                                 Connection                  = "close"
                                             }.AsImmutable);

                              });

            #endregion

            #endregion

            #region GET         ~/organizations/{OrganizationId}/address

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/organizations/GraphDefined/address
            // --------------------------------------------------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.GET,
                              URLPathPrefix + "organizations/{OrganizationId}/address",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its organizations

                                  // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                  if (!TryGetHTTPUser(Request,
                                                      out var HTTPUser,
                                                      out var HTTPOrganizations,
                                                      out var Response,
                                                      AccessLevel:  Access_Levels.ReadOnly,
                                                      Recursive:    true))
                                  {
                                      return Task.FromResult(Response.AsImmutable);
                                  }

                                  #endregion

                                  #region Check OrganizationId URL parameter

                                  if (!Request.ParseOrganization(this,
                                                                 out var OrganizationId,
                                                                 out var Organization,
                                                                 out var HTTPResponse))
                                  {
                                      return Task.FromResult(HTTPResponse.AsImmutable);
                                  }

                                  #endregion


                                  if (Organization != null && HTTPOrganizations.Contains(Organization))
                                  {

                                      return Task.FromResult(
                                          new HTTPResponse.Builder(Request) {
                                              HTTPStatusCode              = HTTPStatusCode.OK,
                                              Server                      = HTTPServer.DefaultServerName,
                                              Date                        = Timestamp.Now,
                                              AccessControlAllowOrigin    = "*",
                                              AccessControlAllowMethods   = new[] { "GET" },
                                              AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                              ContentType                 = HTTPContentType.HTML_UTF8,
                                              Content                     = MixWithHTMLTemplate("organization.address.shtml").ToUTF8Bytes(),
                                              Connection                  = "close",
                                              Vary                        = "Accept"
                                          }.AsImmutable);

                                  }

                                  return Task.FromResult(
                                             new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode              = HTTPStatusCode.Unauthorized,
                                                 Server                      = HTTPServer.DefaultServerName,
                                                 Date                        = Timestamp.Now,
                                                 AccessControlAllowOrigin    = "*",
                                                 AccessControlAllowMethods   = new[] { "GET" },
                                                 AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                                 Connection                  = "close"
                                             }.AsImmutable);

                              });

            #endregion

            #region GET         ~/organizations/{OrganizationId}/members

            // -----------------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/organizations/GraphDefined/members
            // -----------------------------------------------------------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.GET,
                              URLPathPrefix + "organizations/{OrganizationId}/members",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its organizations

                                  // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                  if (!TryGetHTTPUser(Request,
                                                      out var HTTPUser,
                                                      out var HTTPOrganizations,
                                                      out var Response,
                                                      AccessLevel:  Access_Levels.ReadOnly,
                                                      Recursive:    true))
                                  {
                                      return Task.FromResult(Response.AsImmutable);
                                  }

                                  #endregion

                                  #region Check OrganizationId URL parameter

                                  if (!Request.ParseOrganization(this,
                                                                 out var OrganizationId,
                                                                 out var Organization,
                                                                 out var HTTPResponse))
                                  {
                                      return Task.FromResult(HTTPResponse.AsImmutable);
                                  }

                                  #endregion


                                  if (Organization != null && HTTPOrganizations.Contains(Organization))
                                  {

                                      return Task.FromResult(
                                          new HTTPResponse.Builder(Request) {
                                              HTTPStatusCode              = HTTPStatusCode.OK,
                                              Server                      = HTTPServer.DefaultServerName,
                                              Date                        = Timestamp.Now,
                                              AccessControlAllowOrigin    = "*",
                                              AccessControlAllowMethods   = new[] { "GET" },
                                              AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                              ContentType                 = HTTPContentType.HTML_UTF8,
                                              Content                     = MixWithHTMLTemplate("organization.members.shtml").ToUTF8Bytes(),
                                              Connection                  = "close",
                                              Vary                        = "Accept"
                                          }.AsImmutable);

                                  }

                                  return Task.FromResult(
                                             new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode              = HTTPStatusCode.Unauthorized,
                                                 Server                      = HTTPServer.DefaultServerName,
                                                 Date                        = Timestamp.Now,
                                                 AccessControlAllowOrigin    = "*",
                                                 AccessControlAllowMethods   = new[] { "GET" },
                                                 AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                                 Connection                  = "close"
                                             }.AsImmutable);

                              });

            #endregion

            #region GET         ~/organizations/{OrganizationId}/newMember

            // -----------------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/organizations/GraphDefined/newMember
            // -----------------------------------------------------------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.GET,
                              URLPathPrefix + "organizations/{OrganizationId}/newMember",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its organizations

                                  // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                  if (!TryGetHTTPUser(Request,
                                                      out var HTTPUser,
                                                      out var HTTPOrganizations,
                                                      out var Response,
                                                      AccessLevel:  Access_Levels.Admin,
                                                      Recursive:    true))
                                  {
                                      return Task.FromResult(Response.AsImmutable);
                                  }

                                  #endregion

                                  #region Check OrganizationId URL parameter

                                  if (!Request.ParseOrganization(this,
                                                                 out var OrganizationId,
                                                                 out var Organization,
                                                                 out var HTTPResponse))
                                  {
                                      return Task.FromResult(HTTPResponse.AsImmutable);
                                  }

                                  #endregion


                                  if (Organization != null && HTTPOrganizations.Contains(Organization))
                                      return Task.FromResult(
                                          new HTTPResponse.Builder(Request) {
                                              HTTPStatusCode              = HTTPStatusCode.OK,
                                              Server                      = HTTPServer.DefaultServerName,
                                              Date                        = Timestamp.Now,
                                              AccessControlAllowOrigin    = "*",
                                              AccessControlAllowMethods   = new[] { "GET" },
                                              AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                              ContentType                 = HTTPContentType.HTML_UTF8,
                                              Content                     = MixWithHTMLTemplate("organization.newMember.shtml").ToUTF8Bytes(),
                                              Connection                  = "close",
                                              Vary                        = "Accept"
                                          }.AsImmutable);


                                  return Task.FromResult(
                                             new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode              = HTTPStatusCode.Unauthorized,
                                                 Server                      = HTTPServer.DefaultServerName,
                                                 Date                        = Timestamp.Now,
                                                 AccessControlAllowOrigin    = "*",
                                                 AccessControlAllowMethods   = new[] { "GET" },
                                                 AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                                 Connection                  = "close"
                                             }.AsImmutable);

                              });

            #endregion

            #region GET         ~/organizations/{OrganizationId}/subOrganizations

            // -----------------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/organizations/GraphDefined/subOrganizations
            // -----------------------------------------------------------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.GET,
                              URLPathPrefix + "organizations/{OrganizationId}/subOrganizations",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its organizations

                                  // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                  if (!TryGetHTTPUser(Request,
                                                      out var HTTPUser,
                                                      out var HTTPOrganizations,
                                                      out var Response,
                                                      AccessLevel:  Access_Levels.ReadOnly,
                                                      Recursive:    true))
                                  {
                                      return Task.FromResult(Response.AsImmutable);
                                  }

                                  #endregion

                                  #region Check OrganizationId URL parameter

                                  if (!Request.ParseOrganization(this,
                                                                 out var OrganizationId,
                                                                 out var Organization,
                                                                 out var HTTPResponse))
                                  {
                                      return Task.FromResult(HTTPResponse.AsImmutable);
                                  }

                                  #endregion


                                  if (Organization != null && HTTPOrganizations.Contains(Organization))
                                  {

                                      return Task.FromResult(
                                          new HTTPResponse.Builder(Request) {
                                              HTTPStatusCode              = HTTPStatusCode.OK,
                                              Server                      = HTTPServer.DefaultServerName,
                                              Date                        = Timestamp.Now,
                                              AccessControlAllowOrigin    = "*",
                                              AccessControlAllowMethods   = new[] { "GET" },
                                              AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                              ContentType                 = HTTPContentType.HTML_UTF8,
                                              Content                     = MixWithHTMLTemplate("organization.subOrganizations.shtml").ToUTF8Bytes(),
                                              Connection                  = "close",
                                              Vary                        = "Accept"
                                          }.AsImmutable);

                                  }

                                  return Task.FromResult(
                                             new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode              = HTTPStatusCode.Unauthorized,
                                                 Server                      = HTTPServer.DefaultServerName,
                                                 Date                        = Timestamp.Now,
                                                 AccessControlAllowOrigin    = "*",
                                                 AccessControlAllowMethods   = new[] { "GET" },
                                                 AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                                 Connection                  = "close"
                                             }.AsImmutable);

                              });

            #endregion

            #region GET         ~/organizations/{OrganizationId}/newSubOrganization

            // -----------------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/organizations/GraphDefined/newSubOrganization
            // -----------------------------------------------------------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.GET,
                              URLPathPrefix + "organizations/{OrganizationId}/newSubOrganization",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its organizations

                                  // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                  if (!TryGetHTTPUser(Request,
                                                      out var HTTPUser,
                                                      out var HTTPOrganizations,
                                                      out var Response,
                                                      AccessLevel:  Access_Levels.Admin,
                                                      Recursive:    true))
                                  {
                                      return Task.FromResult(Response.AsImmutable);
                                  }

                                  #endregion

                                  #region Check OrganizationId URL parameter

                                  if (!Request.ParseOrganization(this,
                                                                 out var OrganizationId,
                                                                 out var Organization,
                                                                 out var HTTPResponse))
                                  {
                                      return Task.FromResult(HTTPResponse.AsImmutable);
                                  }

                                  #endregion


                                  if (Organization != null && HTTPOrganizations.Contains(Organization))
                                  {

                                      return Task.FromResult(
                                          new HTTPResponse.Builder(Request) {
                                              HTTPStatusCode              = HTTPStatusCode.OK,
                                              Server                      = HTTPServer.DefaultServerName,
                                              Date                        = Timestamp.Now,
                                              AccessControlAllowOrigin    = "*",
                                              AccessControlAllowMethods   = new[] { "GET" },
                                              AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                              ContentType                 = HTTPContentType.HTML_UTF8,
                                              Content                     = MixWithHTMLTemplate("organization.newSubOrganization.shtml").ToUTF8Bytes(),
                                              Connection                  = "close",
                                              Vary                        = "Accept"
                                          }.AsImmutable);

                                  }

                                  return Task.FromResult(
                                             new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode              = HTTPStatusCode.Unauthorized,
                                                 Server                      = HTTPServer.DefaultServerName,
                                                 Date                        = Timestamp.Now,
                                                 AccessControlAllowOrigin    = "*",
                                                 AccessControlAllowMethods   = new[] { "GET" },
                                                 AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                                 Connection                  = "close"
                                             }.AsImmutable);

                              });

            #endregion

            #endregion

            #region ~/organizationGroups

            #region GET         ~/organizationGroups

            #region HTML

            // --------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/organizationGroups
            // --------------------------------------------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.GET,
                              URLPathPrefix + "organizationGroups",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP organization and its organizations

                                  // Will return HTTP 401 Unauthorized, when the HTTP organization is unknown!
                                  if (!TryGetHTTPUser(Request,
                                                      out var HTTPUser,
                                                      out var HTTPOrganizations,
                                                      out var Response,
                                                      Recursive: true))
                                  {
                                      return Task.FromResult(Response.AsImmutable);
                                  }

                                  #endregion

                                  return Task.FromResult(
                                          new HTTPResponse.Builder(Request) {
                                              HTTPStatusCode             = HTTPStatusCode.OK,
                                              Server                     = HTTPServer.DefaultServerName,
                                              Date                       = Timestamp.Now,
                                              AccessControlAllowOrigin   = "*",
                                              AccessControlAllowMethods  = new[] { "GET" },
                                              AccessControlAllowHeaders  = new[] { "Content-Type", "Accept", "Authorization" },
                                              ContentType                = HTTPContentType.HTML_UTF8,
                                              Content                    = MixWithHTMLTemplate("organization.organizationGroups.shtml").ToUTF8Bytes(),
                                              Connection                 = "close",
                                              Vary                       = "Accept"
                                          }.AsImmutable);

                              });

            #endregion

            #endregion

            #endregion


            #region ~/notifications

            #region GET         ~/notifications

            #region HTML

            // ----------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/notifications
            // ----------------------------------------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.GET,
                              URLPathPrefix + "notifications",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its organizations

                                  // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                  if (!TryGetHTTPUser(Request,
                                                      out var HTTPUser,
                                                      out var HTTPOrganizations,
                                                      out var Response,
                                                      Recursive: true))
                                  {
                                      return Task.FromResult(Response.AsImmutable);
                                  }

                                  #endregion


                                  return Task.FromResult(
                                      new HTTPResponse.Builder(Request) {
                                          HTTPStatusCode              = HTTPStatusCode.OK,
                                          Server                      = HTTPServer.DefaultServerName,
                                          Date                        = Timestamp.Now,
                                          AccessControlAllowOrigin    = "*",
                                          AccessControlAllowMethods   = new[] { "GET" },
                                          AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                          ContentType                 = HTTPContentType.HTML_UTF8,
                                          Content                     = MixWithHTMLTemplate("notification.messages.shtml").ToUTF8Bytes(),
                                          Connection                  = "close",
                                          Vary                        = "Accept"
                                      }.AsImmutable);

                              });

            #endregion

            #endregion

            #region GET         ~/notifications/{notificationId}

            // ------------------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/notifications/{notificationId}
            // ------------------------------------------------------------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.GET,
                              URLPathPrefix + "notifications/{notificationId}",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its organizations

                                  // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                  if (!TryGetHTTPUser(Request,
                                                      out var HTTPUser,
                                                      out var HTTPOrganizations,
                                                      out var Response,
                                                      Recursive: true))
                                  {
                                      return Task.FromResult(Response.AsImmutable);
                                  }

                                  #endregion

                                  #region Get notificationId URL parameter

                                  if (Request.ParsedURLParameters.Length < 1 ||
                                      !UInt32.TryParse(Request.ParsedURLParameters[0], out UInt32 NotificationId))
                                  {

                                      return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                     Server          = HTTPServer.DefaultServerName,
                                                     Date            = Timestamp.Now,
                                                     Connection      = "close"
                                                 }.AsImmutable);

                                  }

                                  #endregion


                                  return Task.FromResult(
                                      new HTTPResponse.Builder(Request) {
                                          HTTPStatusCode              = HTTPStatusCode.OK,
                                          Server                      = HTTPServer.DefaultServerName,
                                          Date                        = Timestamp.Now,
                                          AccessControlAllowOrigin    = "*",
                                          AccessControlAllowMethods   = new[] { "GET" },
                                          AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                          ContentType                 = HTTPContentType.HTML_UTF8,
                                          Content                     = MixWithHTMLTemplate("notification.editNotification.shtml").ToUTF8Bytes(),
                                          Connection                  = "close",
                                          Vary                        = "Accept"
                                      }.AsImmutable);

                              });

            #endregion

            #region GET         ~/notificationSettings

            // ---------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/notificationSettings
            // ---------------------------------------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.GET,
                              URLPathPrefix + "notificationSettings",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its organizations

                                  // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                  if (!TryGetHTTPUser(Request,
                                                      out var HTTPUser,
                                                      out var HTTPOrganizations,
                                                      out var Response,
                                                      Recursive: true))
                                  {
                                      return Task.FromResult(Response.AsImmutable);
                                  }

                                  #endregion


                                  return Task.FromResult(
                                      new HTTPResponse.Builder(Request) {
                                          HTTPStatusCode              = HTTPStatusCode.OK,
                                          Server                      = HTTPServer.DefaultServerName,
                                          Date                        = Timestamp.Now,
                                          AccessControlAllowOrigin    = "*",
                                          AccessControlAllowMethods   = new[] { "GET" },
                                          AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                          ContentType                 = HTTPContentType.HTML_UTF8,
                                          Content                     = MixWithHTMLTemplate("notification.settings.shtml").ToUTF8Bytes(),
                                          Connection                  = "close",
                                          Vary                        = "Accept"
                                      }.AsImmutable);

                              });

            #endregion


            #region GET         ~/newNotification

            #region HTML

            // ----------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/newNotification
            // ----------------------------------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.GET,
                              URLPathPrefix + "newNotification",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its organizations

                                  // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                  if (!TryGetHTTPUser(Request,
                                                      out var HTTPUser,
                                                      out var HTTPOrganizations,
                                                      out var Response,
                                                      Recursive: true))
                                  {
                                      return Task.FromResult(Response.AsImmutable);
                                  }

                                  #endregion


                                  return Task.FromResult(
                                      new HTTPResponse.Builder(Request) {
                                          HTTPStatusCode              = HTTPStatusCode.OK,
                                          Server                      = HTTPServer.DefaultServerName,
                                          Date                        = Timestamp.Now,
                                          AccessControlAllowOrigin    = "*",
                                          AccessControlAllowMethods   = new[] { "GET" },
                                          AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
                                          ContentType                 = HTTPContentType.HTML_UTF8,
                                          Content                     = MixWithHTMLTemplate("notification.editNotification.shtml").ToUTF8Bytes(),
                                          Connection                  = "close",
                                          Vary                        = "Accept"
                                      }.AsImmutable);

                              });

            #endregion

            #endregion

            #endregion


            #region ~/dashboard

            #region GET         ~/dashboard

            //// ----------------------------------------------------------------
            //// curl -v -H "Accept: text/html" http://127.0.0.1:3001/dashboard
            //// ----------------------------------------------------------------
            //HTTPServer.AddMethodCallback(Hostname,
            //                             HTTPMethod.GET,
            //                             URLPathPrefix + "dashboard",
            //                             HTTPContentType.HTML_UTF8,
            //                             HTTPDelegate: Request => {

            //                                 #region Get HTTP user and its organizations

            //                                 // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
            //                                 if (!TryGetHTTPUser(Request,
            //                                                     out var HTTPUser,
            //                                                     out var HTTPOrganizations,
            //                                                     out var Response,
            //                                                     Recursive:                 true))
            //                                 {
            //                                     return Task.FromResult(Response.AsImmutable);
            //                                 }

            //                                 #endregion


            //                                 return Task.FromResult(
            //                                     new HTTPResponse.Builder(Request) {
            //                                         HTTPStatusCode              = HTTPStatusCode.OK,
            //                                         Server                      = HTTPServer.DefaultServerName,
            //                                         Date                        = Timestamp.Now,
            //                                         AccessControlAllowOrigin    = "*",
            //                                         AccessControlAllowMethods   = new[] { "GET" },
            //                                         AccessControlAllowHeaders   = new[] { "Content-Type", "Accept", "Authorization" },
            //                                         ContentType                 = HTTPContentType.HTML_UTF8,
            //                                         Content                     = MixWithHTMLTemplate("dashboard.dashboard.shtml").ToUTF8Bytes(),
            //                                         Connection                  = "close",
            //                                         Vary                        = "Accept"
            //                                     }.AsImmutable);

            //                             }, AllowReplacement: URLReplacement.Allow);

            #endregion

            #endregion



            #region ~/newsletterss

            #region SIGNUP    ~/newsletters

            AddMethodCallback(HTTPHostname.Any,
                              HTTPMethod.SIGNUP,
                              URLPathPrefix + "/newsletters",
                              HTTPContentType.JSON_UTF8,
                              HTTPDelegate: Request => {

                                  #region Check remote IP address

                                  var remoteIPAddress = Request.RemoteSocket.IPAddress;

                                  //if (_NewsletterRemoteIPAddresses.TryGetValue(remoteIPAddress, out var lastSignUp) &&
                                  //    lastSignUp > Timestamp.Now - TimeSpan.FromMinutes(1))
                                  //{

                                  //    return Task.FromResult(
                                  //        new HTTPResponse.Builder(Request) {
                                  //            HTTPStatusCode  = HTTPStatusCode.PreconditionRequired,
                                  //            Server          = HTTPServer.DefaultServerName,
                                  //            ContentType     = HTTPContentType.JSON_UTF8,
                                  //            Content         = new JObject(
                                  //                                  //new JProperty("@context",     SignInOutContext),
                                  //                                  //new JProperty("statuscode",   400),
                                  //                                  new JProperty("property",     "email"),
                                  //                                  new JProperty("description",  "Too many requests. Please come back later!")
                                  //                              ).ToString().ToUTF8Bytes(),
                                  //            CacheControl     = "private",
                                  //            Connection       = "close"
                                  //        }.AsImmutable);

                                  //}

                                  #endregion

                                  #region Check JSON HTTP body...

                                  if (!Request.TryParseJObjectRequestBody(out var jsonRequest,
                                                                          out var httpResponse,
                                                                          AllowEmptyHTTPBody: false) ||
                                       jsonRequest is null)
                                  {
                                      return Task.FromResult(httpResponse!.AsImmutable);
                                  }

                                  #endregion

                                  #region Verify e-mail

                                  var emailString = jsonRequest["email"]?.Value<String>()?.Replace(";", "");

                                  if (emailString is null || emailString.IsNullOrEmpty())
                                  {

                                      return Task.FromResult(
                                          new HTTPResponse.Builder(Request) {
                                              HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                              Server          = HTTPServer.DefaultServerName,
                                              ContentType     = HTTPContentType.JSON_UTF8,
                                              Content         = new JObject(
                                                                    //new JProperty("@context",     SignInOutContext),
                                                                    //new JProperty("statuscode",   400),
                                                                    new JProperty("property",     "email"),
                                                                    new JProperty("description",  "The given e-mail address must not be null or empty!")
                                                                ).ToString().ToUTF8Bytes(),
                                              CacheControl     = "private",
                                              Connection       = "close"
                                          }.AsImmutable);

                                  }

                                  var email = SimpleEMailAddress.TryParse(emailString);

                                  if (email is null)
                                  {

                                      return Task.FromResult(
                                          new HTTPResponse.Builder(Request) {
                                              HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                              Server          = HTTPServer.DefaultServerName,
                                              ContentType     = HTTPContentType.JSON_UTF8,
                                              Content         = new JObject(
                                                                    //new JProperty("@context",     SignInOutContext),
                                                                    //new JProperty("statuscode",   400),
                                                                    new JProperty("property",     "email"),
                                                                    new JProperty("description",  "The given e-mail address '" + emailString + "' is invalid!")
                                                                ).ToString().ToUTF8Bytes(),
                                              CacheControl     = "private",
                                              Connection       = "close"
                                          }.AsImmutable);

                                  }

                                  #endregion

                                  #region Verify newsletterId

                                  var newsletterId = Newsletter_Id.TryParse(jsonRequest["newsletterId"]?.Value<String>() ?? "");

                                  if (newsletterId is null || newsletterId.IsNullOrEmpty())
                                  {

                                      return Task.FromResult(
                                          new HTTPResponse.Builder(Request) {
                                              HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                              Server          = HTTPServer.DefaultServerName,
                                              ContentType     = HTTPContentType.JSON_UTF8,
                                              Content         = new JObject(
                                                                    //new JProperty("@context",     SignInOutContext),
                                                                    //new JProperty("statuscode",   400),
                                                                    new JProperty("property",     "newsletterId"),
                                                                    new JProperty("description",  "The given newsletter identification '" + (jsonRequest["newsletterId"]?.Value<String>() ?? "") + "'must not be null or empty!")
                                                                ).ToString().ToUTF8Bytes(),
                                              CacheControl    = "private",
                                              Connection      = "close"
                                          }.AsImmutable);

                                  }

                                  #endregion

                                  #region Check newsletter subscriptions

                                  lock (_NewsletterEMails)
                                  {

                                      if (_NewsletterEMails.Values.Any(emailAddress => emailAddress.Address == email))

                                      return Task.FromResult(
                                          new HTTPResponse.Builder(Request) {
                                              HTTPStatusCode  = HTTPStatusCode.Conflict,
                                              Server          = HTTPServer.DefaultServerName,
                                              ContentType     = HTTPContentType.JSON_UTF8,
                                              Content         = new JObject(
                                                                    //new JProperty("@context",     SignInOutContext),
                                                                    //new JProperty("statuscode",   400),
                                                                    new JProperty("property",     "email"),
                                                                    new JProperty("description",  "The given e-mail address '" + emailString + "' is already subscribed!")
                                                                ).ToString().ToUTF8Bytes(),
                                              CacheControl     = "private",
                                              Connection       = "close"
                                          }.AsImmutable);

                                  }

                                  #endregion

                                  #region Register newsletter signup

                                  lock (_NewsletterRemoteIPAddresses)
                                  {

                                      foreach (var outdated in _NewsletterRemoteIPAddresses.Where(kvp => kvp.Value < Timestamp.Now - TimeSpan.FromMinutes(1)).ToArray())
                                      {
                                          _NewsletterRemoteIPAddresses.Remove(outdated.Key);
                                      }

                                      if (_NewsletterRemoteIPAddresses.ContainsKey(remoteIPAddress))
                                          _NewsletterRemoteIPAddresses[remoteIPAddress] = Timestamp.Now;
                                      else
                                          _NewsletterRemoteIPAddresses.Add(remoteIPAddress, Timestamp.Now);

                                  }

                                  var securityTokenId  = SecurityToken_Id.Parse(
                                                             SHA256.HashData(
                                                                 String.Concat(
                                                                     emailString,
                                                                     RandomExtensions.RandomHexString(30)
                                                                 ).ToUTF8Bytes()
                                                             ).ToHexString()
                                                         );

                                  var expires          = Timestamp.Now + TimeSpan.FromDays(1);

                                  lock (_NewsletterSignups)
                                  {

                                      this._NewsletterSignups.Add(
                                          securityTokenId,
                                          new NewsletterSignup(
                                              email.       Value,
                                              newsletterId.Value,
                                              expires
                                          )
                                      );

                                      File.AppendAllText(UsersAPIPath + DefaultNewsletterSignupFile,
                                                         String.Concat("add;", securityTokenId, ";", email.Value, ";", expires.ToIso8601(), Environment.NewLine));

                                  }

                                  if (NewsletterEMailAddress is not null)
                                      Task.Run(async () =>{
                                          try
                                          {

                                              var aa = await SMTPClient.Send(new HTMLEMailBuilder() {

                                                                                 From           = NewsletterEMailAddress,
                                                                                 To             = EMailAddressListBuilder.Create(email.Value),
                                                                                 Passphrase     = NewsletterGPGPassphrase,
                                                                                 Subject        = "Your newsletter subscription",

                                                                                 HTMLText       = $"Thank you subscribing to our '{newsletterId}' newsletter.<br /> To complete your sign up please click <a href=\"https://open.charging.cloud/newsletters/signups/{securityTokenId}\">here</a>.",

                                                                                 PlainText      = $"Thank you subscribing to our '{newsletterId}' newsletter.\r\nTo complete your sign up please visit https://open.charging.cloud/newsletters/signups/{securityTokenId}.",

                                                                                 SecurityLevel  = EMailSecurity.autosign

                                                                             });

                                          } catch (Exception ex)
                                          {
                                              DebugX.LogException(ex, "Exception when sending newsletter e-mail");
                                          }

                                      var xx = 23;
                                          });

                                  #endregion


                                  return Task.FromResult(
                                      new HTTPResponse.Builder(Request) {
                                          HTTPStatusCode  = HTTPStatusCode.Created,
                                          CacheControl    = "private",
                                          Connection      = "close"
                                      }.AsImmutable);

                              });

            #endregion

            #region GET       ~/newsletters/signups/{securityTokenId}

            AddMethodCallback(HTTPHostname.Any,
                              HTTPMethod.GET,
                              URLPathPrefix + "/newsletters/signups/{securityTokenId}",
                              HTTPDelegate: Request => {

                                  if (SecurityToken_Id.TryParse(Request.ParsedURLParameters[0], out var securityTokenId) &&
                                      _NewsletterSignups.TryGetValue(securityTokenId, out var newsletterSignup) &&
                                      newsletterSignup is not null &&
                                      newsletterSignup.Timestamp >= Timestamp.Now)
                                  {

                                      return Task.FromResult(
                                          new HTTPResponse.Builder(Request) {
                                              HTTPStatusCode  = HTTPStatusCode.OK,
                                              Server          = HTTPServer.DefaultServerName,
                                              ContentType     = HTTPContentType.HTML_UTF8,
                                              Content         = GetResourceString($"newsletters.{newsletterSignup.NewsletterId}-{DefaultLanguage}.html").
                                                                    Replace("{securityTokenId}", securityTokenId.              ToString()).
                                                                    Replace("{eMailAddress}",    newsletterSignup.EMailAddress.ToString()).
                                                                    Replace("{newsletterId}",    newsletterSignup.NewsletterId.ToString()).
                                                                    ToUTF8Bytes(),
                                              CacheControl    = "private",
                                              Connection      = "close"
                                          }.AsImmutable);

                                  }

                                  return Task.FromResult(
                                      new HTTPResponse.Builder(Request) {
                                          HTTPStatusCode  = HTTPStatusCode.NotFound,
                                          Server          = HTTPServer.DefaultServerName,
                                          CacheControl    = "private",
                                          Connection      = "close"
                                      }.AsImmutable);

                              });

            #endregion

            #region VALIDATE  ~/newsletters/signups/{securityTokenId}

            AddMethodCallback(HTTPHostname.Any,
                              HTTPMethod.VALIDATE,
                              URLPathPrefix + "/newsletters/signups/{securityTokenId}",
                              HTTPDelegate: Request => {

                                  if (SecurityToken_Id.TryParse(Request.ParsedURLParameters[0], out var securityTokenId) &&
                                      _NewsletterSignups.TryGetValue(securityTokenId, out var newsletterSignup) &&
                                      newsletterSignup is not null &&
                                      newsletterSignup.Timestamp >= Timestamp.Now)
                                  {

                                      lock (_NewsletterSignups)
                                      {

                                          File.AppendAllText(UsersAPIPath + DefaultNewsletterSignupFile,
                                                             String.Concat("remove;", securityTokenId, ";", newsletterSignup.EMailAddress, ";", newsletterSignup.NewsletterId, Environment.NewLine));

                                          _NewsletterSignups.Remove(securityTokenId);

                                      }

                                      lock (_NewsletterSignups)
                                      {

                                          _NewsletterEMails.Add(newsletterSignup.NewsletterId,
                                                                newsletterSignup.EMailAddress);

                                          File.AppendAllText(UsersAPIPath + DefaultNewsletterEMailsFile,
                                                             String.Concat("add;", newsletterSignup.EMailAddress, ";", newsletterSignup.NewsletterId, ";", Timestamp.Now.ToIso8601(), Environment.NewLine));

                                      }

                                      return Task.FromResult(
                                          new HTTPResponse.Builder(Request) {
                                              HTTPStatusCode  = HTTPStatusCode.Created,
                                              Server          = HTTPServer.DefaultServerName,
                                              CacheControl    = "private",
                                              Connection      = "close"
                                          }.AsImmutable);

                                  }

                                  return Task.FromResult(
                                      new HTTPResponse.Builder(Request) {
                                          HTTPStatusCode  = HTTPStatusCode.NotFound,
                                          Server          = HTTPServer.DefaultServerName,
                                          CacheControl    = "private",
                                          Connection      = "close"
                                      }.AsImmutable);

                              });

            #endregion

            #endregion

            #region ~/blog

            #region OPTIONS     ~/blog

            // -----------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/blog
            // -----------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.OPTIONS,
                              URLPathPrefix + "blog",
                              HTTPDelegate: Request => {

                                  return Task.FromResult(
                                      new HTTPResponse.Builder(Request) {
                                          HTTPStatusCode             = HTTPStatusCode.OK,
                                          Server                     = HTTPServer.DefaultServerName,
                                          Date                       = Timestamp.Now,
                                          AccessControlAllowOrigin   = "*",
                                          AccessControlAllowMethods  = new[] { "GET", "OPTIONS" },
                                          AccessControlAllowHeaders  = new[] { "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" },
                                          Connection                 = "close"
                                      }.AsImmutable);

                              });

            #endregion

            #region GET         ~/blog

            // ------------------------------------
            // curl -v http://127.0.0.1:4100/blog
            // ------------------------------------
            AddMethodCallback(HTTPHostname.Any,
                              HTTPMethod.GET,
                              URLPathPrefix + "/blog",
                              HTTPContentType.HTML_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its organizations

                                  // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                  //if (!TryGetHTTPUser(Request,
                                  //                    out var HTTPUser,
                                  //                    out var HTTPOrganizations,
                                  //                    out var Response,
                                  //                    Recursive:                 true))
                                  //{
                                  //    return Task.FromResult(Response.AsImmutable);
                                  //}

                                  #endregion

                                  return Task.FromResult(
                                          new HTTPResponse.Builder(Request) {
                                              HTTPStatusCode             = HTTPStatusCode.OK,
                                              Server                     = HTTPServer.DefaultServerName,
                                              Date                       = Timestamp.Now,
                                              AccessControlAllowOrigin   = "*",
                                              AccessControlAllowMethods  = new[] { "GET", "OPTIONS" },
                                              AccessControlAllowHeaders  = new[] { "Content-Type", "Accept", "Authorization" },
                                              ContentType                = HTTPContentType.HTML_UTF8,
                                              Content                    = MixWithHTMLTemplate(BlogTemplate, "blog.blogPostings.shtml").ToUTF8Bytes(),
                                              Connection                 = "close",
                                              Vary                       = "Accept"
                                          }.AsImmutable);

                              });

            #endregion


            #region OPTIONS     ~/blog/postings

            // --------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/blog/postings
            // --------------------------------------------------------
            AddMethodCallback(Hostname,
                              HTTPMethod.OPTIONS,
                              URLPathPrefix + "blog/postings",
                              HTTPDelegate: Request => {

                                  return Task.FromResult(
                                      new HTTPResponse.Builder(Request) {
                                          HTTPStatusCode             = HTTPStatusCode.OK,
                                          Server                     = HTTPServer.DefaultServerName,
                                          Date                       = Timestamp.Now,
                                          AccessControlAllowOrigin   = "*",
                                          AccessControlAllowMethods  = new[] { "GET", "COUNT", "OPTIONS" },
                                          AccessControlAllowHeaders  = new[] { "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" },
                                          Connection                 = "close"
                                      }.AsImmutable);

                              });

            #endregion

            #region GET         ~/blog/postings

            #region JSON

            // ---------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/blog/postings
            // ---------------------------------------------------------------------------
            AddMethodCallback(
                              Hostname,
                              HTTPMethod.GET,
                              URLPathPrefix + "blog/postings",
                              HTTPContentType.JSON_UTF8,
                              HTTPDelegate: Request => {

                                  #region Get HTTP user and its organizations

                                  TryGetHTTPUser(Request,
                                                 out var HTTPUser,
                                                 out var HTTPOrganizations,
                                                 out var Response,
                                                 Recursive: true);

                                  #endregion


                                  var withMetadata        = Request.QueryString.GetBoolean    ("withMetadata", false);
                                  var matchFilter         = Request.QueryString.CreateStringFilter<BlogPosting>("match",
                                                                                                                (blogPosting, pattern) => blogPosting.Id.ToString().Contains(pattern) ||
                                                                                                                                          blogPosting.Title.Matches(pattern, IgnoreCase: true) ||
                                                                                                                                          blogPosting.Text. Matches(pattern, IgnoreCase: true));

                                  var from                = Request.QueryString.TryGetDateTime("from");
                                  var to                  = Request.QueryString.TryGetDateTime("to");
                                  var skip                = Request.QueryString.GetUInt64     ("skip");
                                  var take                = Request.QueryString.GetUInt64     ("take");

                                  var includeCryptoHash   = Request.QueryString.GetBoolean    ("includeCryptoHash", true);

                                  var expand              = Request.QueryString.GetStrings    ("expand");
                                  var expandTags          = expand.ContainsIgnoreCase("tags")     ? InfoStatus.Expanded : InfoStatus.ShowIdOnly;
                                  var expandAuthorId      = expand.ContainsIgnoreCase("authorId") ? InfoStatus.Expanded : InfoStatus.ShowIdOnly;

                                  var now                 = Timestamp.Now;

                                  var allBlog             = _BlogPostings.Values.
                                                                Where(posting => posting.PublicationDate <= now).
                                                                ToArray();
                                  var totalCount          = _BlogPostings.ULongCount();

                                  var filteredBlog        = allBlog.
                                                                Where(posting => !from.HasValue || posting.PublicationDate >= from.Value).
                                                                Where(posting => !to.  HasValue || posting.PublicationDate <  to.  Value).
                                                                Where(matchFilter).
                                                                ToArray();
                                  var filteredCount       = filteredBlog.ULongCount();

                                  var JSONResults         = filteredBlog.
                                                                OrderByDescending(posting => posting.PublicationDate).
                                                                ToJSON(skip,
                                                                       take,
                                                                       false, //Embedded
                                                                       expandTags,
                                                                       expandAuthorId,
                                                                       GetBlogPostingSerializator(Request, HTTPUser));


                                  return Task.FromResult(
                                             new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode                = HTTPStatusCode.OK,
                                                 Server                        = HTTPServer.DefaultServerName,
                                                 Date                          = Timestamp.Now,
                                                 AccessControlAllowOrigin      = "*",
                                                 AccessControlAllowMethods     = new[] { "GET", "COUNT", "OPTIONS" },
                                                 AccessControlAllowHeaders     = new[] { "Content-Type", "Accept", "Authorization" },
                                                 ContentType                   = HTTPContentType.JSON_UTF8,
                                                 Content                       = withMetadata
                                                                                     ? JSONObject.Create(
                                                                                           new JProperty("totalCount",     totalCount),
                                                                                           new JProperty("filteredCount",  filteredCount),
                                                                                           new JProperty("blogPostings",   JSONResults)
                                                                                       ).ToUTF8Bytes()
                                                                                     : JSONResults.ToUTF8Bytes(),
                                                 X_ExpectedTotalNumberOfItems  = filteredCount,
                                                 Connection                    = "close",
                                                 Vary                          = "Accept"
                                             }.AsImmutable);

                              });

            #endregion

            #region HTML

            // ---------------------------------------------
            // curl -v http://127.0.0.1:4100/blog/postings
            // ---------------------------------------------
            AddMethodCallback(
                                         HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "/blog/postings",
                                         HTTPContentType.HTML_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out var HTTPUser,
                                             //                    out var HTTPOrganizations,
                                             //                    out var Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Task.FromResult(Response.AsImmutable);
                                             //}

                                             #endregion

                                             return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         Server                     = HTTPServer.DefaultServerName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = new[] { "GET", "COUNT", "OPTIONS" },
                                                         AccessControlAllowHeaders  = new[] { "Content-Type", "Accept", "Authorization" },
                                                         ContentType                = HTTPContentType.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate(BlogTemplate, "blog.blogPostings.shtml").ToUTF8Bytes(),
                                                         Connection                 = "close",
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);

                                         });

            #endregion

            #endregion

            #region COUNT       ~/blog/postings

            // ------------------------------------------------------
            // curl -v -X COUNT http://127.0.0.1:3001/blog/postings
            // ------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.COUNT,
                                         URLPathPrefix + "blog",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             var since  = Request.QueryString.GetDateTime("since");

                                             return Task.FromResult(
                                                        new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode                = HTTPStatusCode.OK,
                                                            Server                        = HTTPServer.DefaultServerName,
                                                            Date                          = Timestamp.Now,
                                                            AccessControlAllowOrigin      = "*",
                                                            AccessControlAllowMethods     = new[] { "GET", "COUNT", "OPTIONS" },
                                                            AccessControlAllowHeaders     = new[] { "Content-Type", "Accept", "Authorization" },
                                                            ContentType                   = HTTPContentType.JSON_UTF8,
                                                            Content                       = JSONObject.Create(
                                                                                                new JProperty("count",
                                                                                                              _BlogPostings.Values.ULongCount(blog => !since.HasValue || blog.PublicationDate >= since.Value))
                                                                                            ).ToUTF8Bytes(),
                                                            Connection                    = "close"
                                                     }.AsImmutable);

                                         });

            #endregion


            #region OPTIONS     ~/blog/postings/{postingId}

            // -------------------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/blog/postings/214080158
            // -------------------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix + "blog/postings/{postingId}",
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = new[] { "GET", "COUNT", "SEARCH", "OPTIONS" },
                                                     AccessControlAllowHeaders  = new[] { "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" },
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         });

            #endregion

            #region GET         ~/blog/postings/{postingId}

            #region JSON

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/blog/postings/214080158
            // --------------------------------------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "blog/postings/{postingId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Check BlogPostingId URI parameter

                                             if (!Request.ParseBlogPosting(this,
                                                                           out BlogPosting_Id?       BlogPostingId,
                                                                           out BlogPosting           BlogPosting,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse.AsImmutable);
                                             }

                                             #endregion

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out var HTTPUser,
                                             //                    out var HTTPOrganizations,
                                             //                    out var Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Task.FromResult(Response.AsImmutable);
                                             //}

                                             #endregion


                                             var expand             = Request.QueryString.GetStrings("expand");
                                             var expandTags         = expand.ContainsIgnoreCase("tags")     ? InfoStatus.Expanded : InfoStatus.ShowIdOnly;
                                             var expandAuthorId     = expand.ContainsIgnoreCase("authorId") ? InfoStatus.Expanded : InfoStatus.ShowIdOnly;

                                             return Task.FromResult(
                                                        new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.OK,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = new[] { "GET", "SET" },
                                                            AccessControlAllowHeaders  = new[] { "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" },
                                                            ETag                       = "1",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = GetBlogPostingSerializator(Request, Anonymous)
                                                                                                            (BlogPosting,
                                                                                                             false, //Embedded
                                                                                                             expandTags,
                                                                                                             expandAuthorId).
                                                                                                         ToUTF8Bytes(),
                                                            Connection                 = "close",
                                                            Vary                       = "Accept"
                                                        }.AsImmutable);

                                         });

            #endregion

            #region HTML

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/blog/postings/214080158
            // --------------------------------------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "blog/postings/{postingId}",
                                         HTTPContentType.HTML_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             //if (!TryGetHTTPUser(Request,
                                             //                    out var HTTPUser,
                                             //                    out var HTTPOrganizations,
                                             //                    out var Response,
                                             //                    Recursive:                 true))
                                             //{
                                             //    return Task.FromResult(Response.AsImmutable);
                                             //}

                                             #endregion

                                             #region Check BlogPostingId URI parameter

                                             if (!Request.ParseBlogPosting(this,
                                                                           out BlogPosting_Id?       BlogPostingId,
                                                                           out BlogPosting           BlogPosting,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse.AsImmutable);
                                             }

                                             #endregion


                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = new[] { "GET" },
                                                     AccessControlAllowHeaders  = new[] { "Content-Type", "Accept", "Authorization" },
                                                     ContentType                = HTTPContentType.HTML_UTF8,
                                                     Content                    = MixWithHTMLTemplate(BlogTemplate, "blog.blogPosting.shtml").ToUTF8Bytes(),
                                                     Connection                 = "close",
                                                     Vary                       = "Accept"
                                                 }.AsImmutable);

                                         });

            #endregion

            #endregion

            #endregion

            #region ~/newsPostings

            #region OPTIONS     ~/newsPostings

            // ---------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/newsPostings
            // ---------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix + "newsPostings",
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = new[] { "GET", "COUNT", "SEARCH", "OPTIONS" },
                                                     AccessControlAllowHeaders  = new[] { "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" },
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         });

            #endregion

            #region GET         ~/newsPostings

            #region JSON

            // ------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/newsPostings
            // ------------------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "newsPostings",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             TryGetHTTPUser(Request,
                                                            out var HTTPUser,
                                                            out var HTTPOrganizations,
                                                            out var Response,
                                                            Recursive: true);

                                             #endregion


                                             var withMetadata        = Request.QueryString.GetBoolean    ("withMetadata", false);
                                             var matchFilter         = Request.QueryString.CreateStringFilter<NewsPosting>("match",
                                                                                                                           (newsPosting, pattern) => newsPosting.Id.ToString().Contains(pattern) ||
                                                                                                                                                     newsPosting.Headline.Matches(pattern, IgnoreCase: true) ||
                                                                                                                                                     newsPosting.Text.    Matches(pattern, IgnoreCase: true));

                                             var from                = Request.QueryString.TryGetDateTime("from");
                                             var to                  = Request.QueryString.TryGetDateTime("to");
                                             var skip                = Request.QueryString.GetUInt64     ("skip");
                                             var take                = Request.QueryString.GetUInt64     ("take");

                                             var includeCryptoHash   = Request.QueryString.GetBoolean    ("includeCryptoHash", true);

                                             var expand              = Request.QueryString.GetStrings    ("expand");
                                             var expandTags          = expand.ContainsIgnoreCase("tags")     ? InfoStatus.Expanded : InfoStatus.ShowIdOnly;
                                             var expandAuthorId      = expand.ContainsIgnoreCase("authorId") ? InfoStatus.Expanded : InfoStatus.ShowIdOnly;

                                             var now                 = Timestamp.Now;

                                             var allNews             = _NewsPostings.Values.
                                                                           Where(posting => posting.PublicationDate <= now).
                                                                           ToArray();
                                             var totalCount          = _NewsPostings.ULongCount();

                                             var filteredNews        = allNews.
                                                                           Where(posting => !from.HasValue || posting.PublicationDate >= from.Value).
                                                                           Where(posting => !to.  HasValue || posting.PublicationDate <  to.  Value).
                                                                           Where(matchFilter).
                                                                           ToArray();
                                             var filteredCount       = filteredNews.ULongCount();

                                             var JSONResults         = filteredNews.
                                                                           OrderByDescending(posting => posting.PublicationDate).
                                                                           ToJSON(skip,
                                                                                  take,
                                                                                  false, //Embedded
                                                                                  expandTags,
                                                                                  expandAuthorId,
                                                                                  GetNewsPostingSerializator(Request, HTTPUser));


                                             return Task.FromResult(
                                                        new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode                = HTTPStatusCode.OK,
                                                            Server                        = HTTPServer.DefaultServerName,
                                                            Date                          = Timestamp.Now,
                                                            AccessControlAllowOrigin      = "*",
                                                            AccessControlAllowMethods     = new[] { "GET" },
                                                            AccessControlAllowHeaders     = new[] { "Content-Type", "Accept", "Authorization" },
                                                            ContentType                   = HTTPContentType.JSON_UTF8,
                                                            Content                       = withMetadata
                                                                                                ? JSONObject.Create(
                                                                                                      new JProperty("totalCount",     totalCount),
                                                                                                      new JProperty("filteredCount",  filteredCount),
                                                                                                      new JProperty("newsPostings",   JSONResults)
                                                                                                  ).ToUTF8Bytes()
                                                                                                : JSONResults.ToUTF8Bytes(),
                                                            X_ExpectedTotalNumberOfItems  = filteredCount,
                                                            Connection                    = "close",
                                                            Vary                          = "Accept"
                                                        }.AsImmutable);

                                         });

            #endregion

            #region HTML

            // -----------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/newsPostings
            // -----------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "newsPostings",
                                         HTTPContentType.HTML_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out var HTTPUser,
                                                                 out var HTTPOrganizations,
                                                                 out var Response,
                                                                 Recursive:                 true))
                                             {
                                                 return Task.FromResult(Response.AsImmutable);
                                             }

                                             #endregion

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = new[] { "GET" },
                                                     AccessControlAllowHeaders  = new[] { "Content-Type", "Accept", "Authorization" },
                                                     ContentType                = HTTPContentType.HTML_UTF8,
                                                     Content                    = MixWithHTMLTemplate("newsPosting.newsPostings.shtml").ToUTF8Bytes(),
                                                     Connection                 = "close",
                                                     Vary                       = "Accept"
                                                 }.AsImmutable);

                                         });

            #endregion

            #endregion

            #region COUNT       ~/newsPostings

            // ---------------------------------------------------------------------------
            // curl -v -X COUNT -H "Accept: application/json" http://127.0.0.1:3001/newsPostings
            // ---------------------------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.COUNT,
                                         URLPathPrefix + "newsPostings",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             var since  = Request.QueryString.GetDateTime("since");

                                             return Task.FromResult(
                                                        new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode                = HTTPStatusCode.OK,
                                                            Server                        = HTTPServer.DefaultServerName,
                                                            Date                          = Timestamp.Now,
                                                            AccessControlAllowOrigin      = "*",
                                                            AccessControlAllowMethods     = new[] { "GET" },
                                                            AccessControlAllowHeaders     = new[] { "Content-Type", "Accept", "Authorization" },
                                                            ContentType                   = HTTPContentType.JSON_UTF8,
                                                            Content                       = JSONObject.Create(
                                                                                                new JProperty("count",
                                                                                                              _NewsPostings.Values.ULongCount(news => !since.HasValue || news.PublicationDate >= since.Value))
                                                                                            ).ToUTF8Bytes(),
                                                            Connection                    = "close"
                                                     }.AsImmutable);

                                         });

            #endregion


            #region OPTIONS          ~/newsPostings/{postingId}

            // -------------------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/newsPostings/214080158
            // -------------------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix + "newsPostings/{postingId}",
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = new[] { "GET", "COUNT", "SEARCH", "OPTIONS" },
                                                     AccessControlAllowHeaders  = new[] { "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" },
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         });

            #endregion

            #region GET              ~/newsPostings/{postingId}

            #region JSON

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/newsPostings/214080158
            // --------------------------------------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "newsPostings/{postingId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Check NewsPostingId URI parameter

                                             if (!Request.ParseNewsPosting(this,
                                                                           out NewsPosting_Id?       NewsPostingId,
                                                                           out NewsPosting           NewsPosting,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse.AsImmutable);
                                             }

                                             #endregion

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out var HTTPUser,
                                                                 out var HTTPOrganizations,
                                                                 out var Response,
                                                                 Recursive:                 true))
                                             {
                                                 return Task.FromResult(Response.AsImmutable);
                                             }

                                             #endregion


                                             var expand             = Request.QueryString.GetStrings("expand");
                                             var expandTags         = expand.ContainsIgnoreCase("tags")     ? InfoStatus.Expanded : InfoStatus.ShowIdOnly;
                                             var expandAuthorId     = expand.ContainsIgnoreCase("authorId") ? InfoStatus.Expanded : InfoStatus.ShowIdOnly;

                                             return Task.FromResult(
                                                        new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode             = HTTPStatusCode.OK,
                                                            Server                     = HTTPServer.DefaultServerName,
                                                            Date                       = Timestamp.Now,
                                                            AccessControlAllowOrigin   = "*",
                                                            AccessControlAllowMethods  = new[] { "GET", "SET" },
                                                            AccessControlAllowHeaders  = new[] { "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" },
                                                            ETag                       = "1",
                                                            ContentType                = HTTPContentType.JSON_UTF8,
                                                            Content                    = GetNewsPostingSerializator(Request, HTTPUser)
                                                                                                            (NewsPosting,
                                                                                                             false, //Embedded
                                                                                                             expandTags,
                                                                                                             expandAuthorId).
                                                                                                         ToUTF8Bytes(),
                                                            Connection                 = "close",
                                                            Vary                       = "Accept"
                                                        }.AsImmutable);

                                         });

            #endregion

            #region HTML

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/newsPostings/214080158
            // --------------------------------------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "newsPostings/{postingId}",
                                         HTTPContentType.HTML_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out var HTTPUser,
                                                                 out var HTTPOrganizations,
                                                                 out var Response,
                                                                 Recursive:                 true))
                                             {
                                                 return Task.FromResult(Response.AsImmutable);
                                             }

                                             #endregion

                                             #region Check NewsPostingId URI parameter

                                             if (!Request.ParseNewsPosting(this,
                                                                           out NewsPosting_Id?       NewsPostingId,
                                                                           out NewsPosting           NewsPosting,
                                                                           out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse.AsImmutable);
                                             }

                                             #endregion


                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = new[] { "GET" },
                                                     AccessControlAllowHeaders  = new[] { "Content-Type", "Accept", "Authorization" },
                                                     ContentType                = HTTPContentType.HTML_UTF8,
                                                     Content                    = MixWithHTMLTemplate("News.News.shtml").ToUTF8Bytes(),
                                                     Connection                 = "close",
                                                     Vary                       = "Accept"
                                                 }.AsImmutable);

                                         });

            #endregion

            #endregion

            #endregion

            #region ~/newsBanners

            #region OPTIONS     ~/newsBanners

            // ---------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/newsBanners
            // ---------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix + "newsBanners",
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = new[] { "GET", "COUNT", "SEARCH", "OPTIONS" },
                                                     AccessControlAllowHeaders  = new[] { "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" },
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         });

            #endregion

            #region GET         ~/newsBanners

            #region JSON

            // ------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/newsBanners
            // ------------------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "newsBanners",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             TryGetHTTPUser(Request,
                                                            out var HTTPUser,
                                                            out var HTTPOrganizations,
                                                            out var Response,
                                                            Recursive: true);

                                             #endregion


                                             var withMetadata        = Request.QueryString.GetBoolean    ("withMetadata", false);
                                             var matchFilter         = Request.QueryString.CreateStringFilter<NewsBanner>("match",
                                                                                                                          (newsBanner, pattern) => newsBanner.Id.ToString().Contains(pattern) ||
                                                                                                                                                   newsBanner.Text.Matches(pattern, IgnoreCase: true));

                                             var from                = Request.QueryString.TryGetDateTime("from");
                                             var to                  = Request.QueryString.TryGetDateTime("to");
                                             var skip                = Request.QueryString.GetUInt64     ("skip");
                                             var take                = Request.QueryString.GetUInt64     ("take");

                                             var expand              = Request.QueryString.GetStrings    ("expand");
                                             var expandTags          = expand.ContainsIgnoreCase("tags")         ? InfoStatus.Expanded : InfoStatus.ShowIdOnly;
                                             var expandDataLicenses  = expand.ContainsIgnoreCase("dataLicenses") ? InfoStatus.Expanded : InfoStatus.ShowIdOnly;
                                             var expandOwnerId       = expand.ContainsIgnoreCase("ownerId")      ? InfoStatus.Expanded : InfoStatus.ShowIdOnly;

                                             var now                 = Timestamp.Now;

                                             var allNews             = _NewsBanners.Values.
                                                                           ToArray();
                                             var totalCount          = _NewsBanners.ULongCount();

                                             var filteredNews        = allNews.
                                                                           Where(matchFilter).
                                                                           Where(banner => !from.HasValue || banner.StartTimestamp >= from.Value).
                                                                           Where(banner => !to.  HasValue || banner.EndTimestamp   <  to.  Value).
                                                                           ToArray();
                                             var filteredCount       = filteredNews.ULongCount();

                                             var JSONResults         = filteredNews.
                                                                           OrderByDescending(banner => banner.StartTimestamp).
                                                                           ToJSON(skip,
                                                                                  take,
                                                                                  false, //Embedded
                                                                                  expandTags,
                                                                                  expandOwnerId,
                                                                                  GetNewsBannerSerializator(Request, HTTPUser));


                                             return Task.FromResult(
                                                        new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode                = HTTPStatusCode.OK,
                                                            Server                        = HTTPServer.DefaultServerName,
                                                            Date                          = Timestamp.Now,
                                                            AccessControlAllowOrigin      = "*",
                                                            AccessControlAllowMethods     = new[] { "GET" },
                                                            AccessControlAllowHeaders     = new[] { "Content-Type", "Accept", "Authorization" },
                                                            ContentType                   = HTTPContentType.JSON_UTF8,
                                                            Content                       = withMetadata
                                                                                                ? JSONObject.Create(
                                                                                                      new JProperty("totalCount",     totalCount),
                                                                                                      new JProperty("filteredCount",  filteredCount),
                                                                                                      new JProperty("newsBanners",    JSONResults)
                                                                                                  ).ToUTF8Bytes()
                                                                                                : JSONResults.ToUTF8Bytes(),
                                                            X_ExpectedTotalNumberOfItems  = filteredCount,
                                                            Connection                    = "close",
                                                            Vary                          = "Accept"
                                                        }.AsImmutable);

                                         });

            #endregion

            #region HTML

            // -----------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/newsBanners
            // -----------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "newsBanners",
                                         HTTPContentType.HTML_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out var HTTPUser,
                                                                 out var HTTPOrganizations,
                                                                 out var Response,
                                                                 Recursive:                 true))
                                             {
                                                 return Task.FromResult(Response.AsImmutable);
                                             }

                                             #endregion

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = new[] { "GET" },
                                                     AccessControlAllowHeaders  = new[] { "Content-Type", "Accept", "Authorization" },
                                                     ContentType                = HTTPContentType.HTML_UTF8,
                                                     Content                    = MixWithHTMLTemplate("newsBanner.newsBanners.shtml").ToUTF8Bytes(),
                                                     Connection                 = "close",
                                                     Vary                       = "Accept"
                                                 }.AsImmutable);

                                         });

            #endregion

            #endregion

            #region COUNT       ~/newsBanners

            // ---------------------------------------------------------------------------
            // curl -v -X COUNT -H "Accept: application/json" http://127.0.0.1:3001/newsBanners
            // ---------------------------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.COUNT,
                                         URLPathPrefix + "newsBanners",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             var since  = Request.QueryString.GetDateTime("since");

                                             return Task.FromResult(
                                                        new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode                = HTTPStatusCode.OK,
                                                            Server                        = HTTPServer.DefaultServerName,
                                                            Date                          = Timestamp.Now,
                                                            AccessControlAllowOrigin      = "*",
                                                            AccessControlAllowMethods     = new[] { "GET" },
                                                            AccessControlAllowHeaders     = new[] { "Content-Type", "Accept", "Authorization" },
                                                            ContentType                   = HTTPContentType.JSON_UTF8,
                                                            Content                       = JSONObject.Create(
                                                                                                new JProperty("count",
                                                                                                              _NewsBanners.Values.ULongCount(news => !since.HasValue || news.StartTimestamp >= since.Value))
                                                                                            ).ToUTF8Bytes(),
                                                            Connection                    = "close"
                                                     }.AsImmutable);

                                         });

            #endregion


            #region OPTIONS          ~/newsBanners/{bannerId}

            // -------------------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/newsBanners/214080158
            // -------------------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix + "newsBanners/{bannerId}",
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = new[] { "GET", "COUNT", "SEARCH", "OPTIONS" },
                                                     AccessControlAllowHeaders  = new[] { "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" },
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         });

            #endregion

            #region GET              ~/newsBanners/{bannerId}

            #region JSON

            // -----------------------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/newsBanners/214080158
            // -----------------------------------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "newsBanners/{bannerId}",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Check NewsId URI parameter

                                             if (!Request.ParseNewsBanner(this,
                                                                          out NewsBanner_Id?        NewsBannerId,
                                                                          out NewsBanner            NewsBanner,
                                                                          out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse.AsImmutable);
                                             }

                                             #endregion

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out var HTTPUser,
                                                                 out var HTTPOrganizations,
                                                                 out var Response,
                                                                 Recursive:                 true))
                                             {
                                                 return Task.FromResult(Response.AsImmutable);
                                             }

                                             #endregion

                                             //var includeCryptoHash   = Request.QueryString.GetBoolean("includeCryptoHash", true);

                                             var expand              = Request.QueryString.GetStrings("expand");
                                             var expandTags          = expand.ContainsIgnoreCase("tags")         ? InfoStatus.Expanded : InfoStatus.ShowIdOnly;
                                             var expandOwnerId       = expand.ContainsIgnoreCase("ownerId")      ? InfoStatus.Expanded : InfoStatus.ShowIdOnly;

                                             return Task.FromResult(
                                                       new HTTPResponse.Builder(Request) {
                                                                HTTPStatusCode             = HTTPStatusCode.OK,
                                                                Server                     = HTTPServer.DefaultServerName,
                                                                Date                       = Timestamp.Now,
                                                                AccessControlAllowOrigin   = "*",
                                                                AccessControlAllowMethods  = new[] { "GET", "SET" },
                                                                AccessControlAllowHeaders  = new[] { "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" },
                                                                ETag                       = "1",
                                                                ContentType                = HTTPContentType.JSON_UTF8,
                                                                Content                    = GetNewsBannerSerializator(Request, HTTPUser)
                                                                                                                       (NewsBanner,
                                                                                                                        false, //Embedded
                                                                                                                        expandTags,
                                                                                                                        expandOwnerId).
                                                                                                                    ToUTF8Bytes(),
                                                                Connection                 = "close",
                                                                Vary                       = "Accept"
                                                            }.AsImmutable);

                                         });

            #endregion

            #region HTML

            // --------------------------------------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/newsBanners/214080158
            // --------------------------------------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "newsBanners/{bannerId}",
                                         HTTPContentType.HTML_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out var HTTPUser,
                                                                 out var HTTPOrganizations,
                                                                 out var Response,
                                                                 Recursive:                 true))
                                             {
                                                 return Task.FromResult(Response.AsImmutable);
                                             }

                                             #endregion

                                             #region Check NewsBannerId URI parameter

                                             if (!Request.ParseNewsBanner(this,
                                                                          out NewsBanner_Id?        NewsBannerId,
                                                                          out NewsBanner            NewsBanner,
                                                                          out HTTPResponse.Builder  HTTPResponse))
                                             {
                                                 return Task.FromResult(HTTPResponse.AsImmutable);
                                             }

                                             #endregion


                                                 return Task.FromResult(
                                                     new HTTPResponse.Builder(Request) {
                                                         HTTPStatusCode             = HTTPStatusCode.OK,
                                                         Server                     = HTTPServer.DefaultServerName,
                                                         Date                       = Timestamp.Now,
                                                         AccessControlAllowOrigin   = "*",
                                                         AccessControlAllowMethods  = new[] { "GET" },
                                                         AccessControlAllowHeaders  = new[] { "Content-Type", "Accept", "Authorization" },
                                                         ContentType                = HTTPContentType.HTML_UTF8,
                                                         Content                    = MixWithHTMLTemplate("News.News.shtml").ToUTF8Bytes(),
                                                         Connection                 = "close",
                                                         Vary                       = "Accept"
                                                     }.AsImmutable);


                                         });

            #endregion

            #endregion

            #endregion

            #region ~/FAQs

            #region OPTIONS     ~/FAQs

            // ---------------------------------------------------------
            // curl -X OPTIONS -v http://127.0.0.1:3001/FAQs
            // ---------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.OPTIONS,
                                         URLPathPrefix + "FAQs",
                                         HTTPDelegate: Request => {

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = new[] { "GET", "COUNT", "SEARCH", "OPTIONS" },
                                                     AccessControlAllowHeaders  = new[] { "X-PINGOTHER", "Content-Type", "Accept", "Authorization", "X-App-Version" },
                                                     Connection                 = "close"
                                                 }.AsImmutable);

                                         });

            #endregion

            #region GET         ~/FAQs

            #region JSON

            // ------------------------------------------------------------------
            // curl -v -H "Accept: application/json" http://127.0.0.1:3001/FAQs
            // ------------------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "FAQs",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             TryGetHTTPUser(Request,
                                                            out var HTTPUser,
                                                            out var HTTPOrganizations,
                                                            out var Response,
                                                            Recursive: true);

                                             #endregion


                                             var withMetadata        = Request.QueryString.GetBoolean    ("withMetadata", false);
                                             var matchFilter         = Request.QueryString.CreateStringFilter<FAQ>("match",
                                                                                                                   (faq, pattern) => faq.Id.ToString().Contains(pattern) ||
                                                                                                                                     faq.Question.Matches(pattern, IgnoreCase: true) ||
                                                                                                                                     faq.Answer.  Matches(pattern, IgnoreCase: true));

                                             var from                = Request.QueryString.TryGetDateTime("from");
                                             var to                  = Request.QueryString.TryGetDateTime("to");
                                             var skip                = Request.QueryString.GetUInt64     ("skip");
                                             var take                = Request.QueryString.GetUInt64     ("take");

                                             var expand              = Request.QueryString.GetStrings    ("expand");
                                             var expandTags          = expand.ContainsIgnoreCase("tags")     ? InfoStatus.Expanded : InfoStatus.ShowIdOnly;
                                             var expandAuthorId      = expand.ContainsIgnoreCase("authorId") ? InfoStatus.Expanded : InfoStatus.ShowIdOnly;

                                             var now                 = Timestamp.Now;

                                             var allFAQs             = _FAQs.Values.
                                                                           Where(posting => posting.PublicationDate <= now).
                                                                           ToArray();
                                             var totalCount          = allFAQs.ULongCount();

                                             var filteredFAQs        = allFAQs.
                                                                           Where(matchFilter).
                                                                           Where(posting => !from.HasValue || posting.PublicationDate >= from.Value).
                                                                           Where(posting => !to.  HasValue || posting.PublicationDate <  to.  Value).
                                                                           ToArray();
                                             var filteredCount       = filteredFAQs.ULongCount();

                                             var JSONResults         = filteredFAQs.
                                                                           OrderBy(posting => posting.PublicationDate).
                                                                           ToJSON(skip,
                                                                                  take,
                                                                                  false, //Embedded
                                                                                  expandTags,
                                                                                  expandAuthorId,
                                                                                  GetFAQSerializator(Request, HTTPUser));


                                             return Task.FromResult(
                                                        new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode                = HTTPStatusCode.OK,
                                                            Server                        = HTTPServer.DefaultServerName,
                                                            Date                          = Timestamp.Now,
                                                            AccessControlAllowOrigin      = "*",
                                                            AccessControlAllowMethods     = new[] { "GET" },
                                                            AccessControlAllowHeaders     = new[] { "Content-Type", "Accept", "Authorization" },
                                                            ContentType                   = HTTPContentType.JSON_UTF8,
                                                            Content                       = withMetadata
                                                                                                ? JSONObject.Create(
                                                                                                      new JProperty("totalCount",     totalCount),
                                                                                                      new JProperty("filteredCount",  filteredCount),
                                                                                                      new JProperty("FAQs",           JSONResults)
                                                                                                  ).ToUTF8Bytes()
                                                                                                : JSONResults.ToUTF8Bytes(),
                                                            X_ExpectedTotalNumberOfItems  = filteredCount,
                                                            Connection                    = "close",
                                                            Vary                          = "Accept"
                                                        }.AsImmutable);

                                         });

            #endregion

            #region HTML

            // -----------------------------------------------------------
            // curl -v -H "Accept: text/html" http://127.0.0.1:3001/FAQs
            // -----------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.GET,
                                         URLPathPrefix + "FAQs",
                                         HTTPContentType.HTML_UTF8,
                                         HTTPDelegate: Request => {

                                             #region Get HTTP user and its organizations

                                             // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
                                             if (!TryGetHTTPUser(Request,
                                                                 out var HTTPUser,
                                                                 out var HTTPOrganizations,
                                                                 out var Response,
                                                                 Recursive:                 true))
                                             {
                                                 return Task.FromResult(Response.AsImmutable);
                                             }

                                             #endregion

                                             return Task.FromResult(
                                                 new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.OK,
                                                     Server                     = HTTPServer.DefaultServerName,
                                                     Date                       = Timestamp.Now,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = new[] { "GET" },
                                                     AccessControlAllowHeaders  = new[] { "Content-Type", "Accept", "Authorization" },
                                                     ContentType                = HTTPContentType.HTML_UTF8,
                                                     Content                    = MixWithHTMLTemplate("FAQ.FAQs.shtml").ToUTF8Bytes(),
                                                     Connection                 = "close",
                                                     Vary                       = "Accept"
                                                 }.AsImmutable);

                                         });

            #endregion

            #endregion

            #region COUNT       ~/FAQs

            // ---------------------------------------------------------------------------
            // curl -v -X COUNT -H "Accept: application/json" http://127.0.0.1:3001/FAQs
            // ---------------------------------------------------------------------------
            AddMethodCallback(
                                         Hostname,
                                         HTTPMethod.COUNT,
                                         URLPathPrefix + "FAQs",
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             var since  = Request.QueryString.GetDateTime("since");

                                             return Task.FromResult(
                                                        new HTTPResponse.Builder(Request) {
                                                            HTTPStatusCode                = HTTPStatusCode.OK,
                                                            Server                        = HTTPServer.DefaultServerName,
                                                            Date                          = Timestamp.Now,
                                                            AccessControlAllowOrigin      = "*",
                                                            AccessControlAllowMethods     = new[] { "GET" },
                                                            AccessControlAllowHeaders     = new[] { "Content-Type", "Accept", "Authorization" },
                                                            ContentType                   = HTTPContentType.JSON_UTF8,
                                                            Content                       = JSONObject.Create(
                                                                                                new JProperty("count",
                                                                                                              _FAQs.Values.ULongCount(news => !since.HasValue || news.PublicationDate >= since.Value))
                                                                                            ).ToUTF8Bytes(),
                                                            Connection                    = "close"
                                                     }.AsImmutable);

                                         });

            #endregion


            #region GET         ~/FAQ/_new

            //// ---------------------------------------------------------------
            //// curl -v -H "Accept: text/html" http://127.0.0.1:3001/FAQ/_new
            //// ---------------------------------------------------------------
            //HTTPServer.AddMethodCallback(Hostname,
            //                             HTTPMethod.GET,
            //                             URIPrefix + "FAQ/_new",
            //                             HTTPContentType.HTML_UTF8,
            //                             HTTPDelegate: Request => {

            //                                 #region Get HTTP user and its organizations

            //                                 // Will return HTTP 401 Unauthorized, when the HTTP user is unknown!
            //                                 if (!TryGetHTTPUser(Request,
            //                                                     out var HTTPUser,
            //                                                     out var HTTPOrganizations,
            //                                                     out HTTPResponse           Response,
            //                                                     Recursive:                 true))
            //                                 {
            //                                     return Task.FromResult(Response);
            //                                 }

            //                                 #endregion

            //                                 var _MemoryStream1 = new MemoryStream();
            //                                 GetType().Assembly.GetManifestResourceStream(HTTPRoot + "template.html").SeekAndCopyTo(_MemoryStream1, 0);
            //                                 var Template = _MemoryStream1.ToArray().ToUTF8String();

            //                                 var _MemoryStream2 = new MemoryStream();
            //                                 GetType().Assembly.GetManifestResourceStream(HTTPRoot + "FAQ.newQuestion.shtml").SeekAndCopyTo(_MemoryStream2, 3);

            //                                 return Task.FromResult(
            //                                     new HTTPResponse.Builder(Request) {
            //                                         HTTPStatusCode             = HTTPStatusCode.OK,
            //                                         Server                     = HTTPServer.DefaultServerName,
            //                                         Date                       = Timestamp.Now,
            //                                         AccessControlAllowOrigin   = "*",
            //                                         AccessControlAllowMethods  = new[] { "GET", "SET" },
            //                                         AccessControlAllowHeaders  = new[] { "Content-Type", "Accept", "Authorization" },
            //                                         ETag                       = "1",
            //                                         ContentType                = HTTPContentType.HTML_UTF8,
            //                                         Content                    = Template.Replace("<%= content %>", _MemoryStream2.ToArray().ToUTF8String()).
            //                                                                                            Replace("<%= logoimage %>", String.Concat(@"<img src=""", LogoImage, @""" /> ")).
            //                                                                                            //Replace("/defibrillator/defibrillator.min.css", "/defibrillator/newDefibrillator.min.css").
            //                                                                                            //Replace("/defibrillator/defibrillator.js",      "/defibrillator/newDefibrillator.js").
            //                                                                                            //Replace("StartDefibrillator(true);",            "StartNewDefibrillator(true);").
            //                                                                                            ToUTF8Bytes(),
            //                                         Connection                 = "close"
            //                                     }.AsImmutable);

            //                             });

            #endregion

            #endregion


        }

        #endregion

        #region Database file...

        #region (protected) ReadDatabaseFiles(ProcessEventDelegate, DatabaseFileName = null)

        /// <summary>
        /// Read the database file.
        /// </summary>
        /// <param name="ProcessEventDelegate">A delegate to process each database entry.</param>
        /// <param name="DatabaseFileName">The optional database file name.</param>
        protected async Task ReadDatabaseFiles(Func<String, JObject, String, UInt64?, Task>  ProcessEventDelegate,
                                               String?                                       DatabaseFileName = null)
        {

            if (DisableLogging)
                return;

            String databaseFileName = DatabaseFileName ?? this.DatabaseFileName;

            DebugX.Log("Reloading database file '" + databaseFileName + "'...");

            try
            {

                JObject JSONLine;
                String  JSONCommand;

                File.ReadLines(databaseFileName).ForEachCounted(async (line, lineNumber) => {

                    if (line.IsNeitherNullNorEmpty() &&
                       !line.StartsWith("#") &&
                       !line.StartsWith("//"))
                    {

                        try
                        {

                            JSONLine  = JObject.Parse(line);

                            if (JSONLine.First is JProperty jsonProperty)
                            {

                                JSONCommand  = jsonProperty.Name;

                                if (JSONCommand.IsNotNullOrEmpty() &&
                                    jsonProperty.Value is JObject jsonObject)
                                {

                                    CurrentDatabaseHashValue = JSONLine?["sha256hash"]?["hashValue"]?.Value<String>() ?? "";

                                    await ProcessEventDelegate(JSONCommand,
                                                               jsonObject,
                                                               databaseFileName,
                                                               lineNumber);

                                }

                            }

                        }
                        catch (Exception e)
                        {
                            DebugX.Log("Could not (re-)load database file ''" + databaseFileName + "' line " + lineNumber + ": " + e.Message);
                        }

                    }

                });

            }
            catch (FileNotFoundException)
            {
                DebugX.LogT("Could not find database file '" + databaseFileName + "'!");
            }
            catch (Exception e)
            {
                DebugX.LogT("Could not (re-)load database file '" + databaseFileName + "': " + e.Message);
            }



            DebugX.Log("Reloading all UsersAPI database helper files...");


            DebugX.Log("Reloading of all UsersAPI database helper files finished...");

        }

        #endregion



        #region (protected) LoadChangeSetsFromAPI(ProcessEventDelegate, LastKnownSHA256HashValue = null)

        /// <summary>
        /// Read the database file.
        /// </summary>
        /// <param name="ProcessEventDelegate">A delegate to process each database entry.</param>
        /// <param name="LastKnownSHA256HashValue">The optional last known SHA256 hash value.</param>
        public async Task LoadChangeSetsFromAPI(Func<String, JObject, String, UInt64?, Task>  ProcessEventDelegate,
                                                String?                                       LastKnownSHA256HashValue = null)
        {

            try
            {

                JArray? jsonChangeSets = null;

                #region Get change sets from remote API

                var retries     = -1;
                var maxRetries  = 3;

                var _remoteAuthServers = Array.Empty<URLWithAPIKey>();

                lock (remoteAuthServers)
                {
                    _remoteAuthServers = remoteAuthServers.ToArray();
                }

                var usedRemoteAuthServer = _remoteAuthServers.FirstOrDefault();

                do
                {

                    retries++;

                    foreach (var remoteAuthServer in _remoteAuthServers)
                    {

                        try
                        {

                            #region Upstream HTTP(S) request...

                            var httpresult = await HTTPClientFactory.Create(remoteAuthServer.URL,
                                                                            //VirtualHostname,
                                                                            //Description,
                                                                            //RemoteCertificateValidator,
                                                                            //ClientCertificateSelector,
                                                                            //ClientCert,
                                                                            //HTTPUserAgent,
                                                                            //RequestTimeout,
                                                                            //TransmissionRetryDelay,
                                                                            //MaxNumberOfRetries,
                                                                            //UseHTTPPipelining,
                                                                            //HTTPLogger,
                                                                            DNSClient: DNSClient).

                                                        Execute(client => client.GETRequest(remoteAuthServer.URL.Path + (LastKnownSHA256HashValue is not null ? "changeSets?skipUntil=" + LastKnownSHA256HashValue : "changeSets"),
                                                                                            requestbuilder => {
                                                                                                requestbuilder.Host         = remoteAuthServer.URL.Hostname;
                                                                                                requestbuilder.API_Key      = remoteAuthServer.APIKeyId;
                                                                                                requestbuilder.Accept.Add(HTTPContentType.JSON_UTF8);
                                                                                            }),

                                                                //RequestLogDelegate:   OnGetCDRsHTTPRequest,
                                                                //ResponseLogDelegate:  OnGetCDRsHTTPResponse,
                                                                //CancellationToken:    CancellationToken,
                                                                //EventTrackingId:      EventTrackingId,
                                                                RequestTimeout: TimeSpan.FromSeconds(5)).

                                                        ConfigureAwait(false);

                            #endregion

                            #region HTTPStatusCode.OK

                            if (httpresult.HTTPStatusCode == HTTPStatusCode.OK)
                            {

                                jsonChangeSets        = JArray.Parse(httpresult.HTTPBody.ToUTF8String());
                                usedRemoteAuthServer  = remoteAuthServer;

                                DebugX.Log("Loaded " + jsonChangeSets.Count + " remote change sets from '" + remoteAuthServer.URL.ToString() + (LastKnownSHA256HashValue is not null ? "/changeSets?skipUntil=" + LastKnownSHA256HashValue : "/changeSets"));

                            }

                            #endregion

                        }
                        catch (Exception e)
                        {
                            DebugX.Log("Could not load remote change set from '" + remoteAuthServer.URL.ToString() + (LastKnownSHA256HashValue is not null ? "/changeSets?skipUntil=" + LastKnownSHA256HashValue : "/changeSets") + ": " + e.Message);
                        }

                    }

                } while (jsonChangeSets is null && retries < maxRetries);

                #endregion

                #region Process change sets

                if (jsonChangeSets is not null)
                {

                    jsonChangeSets.ForEachCounted(async (line, lineNumber) =>
                    {

                        if (line is JObject JSONLine)
                        {

                            try
                            {

                                if (JSONLine.First is JProperty jsonProperty)
                                {

                                    var JSONCommand = jsonProperty.Name;

                                    if (JSONCommand.IsNotNullOrEmpty() &&
                                        jsonProperty.Value is JObject jsonObject)
                                    {

                                        CurrentDatabaseHashValue = JSONLine?["sha256hash"]?["hashValue"]?.Value<String>() ?? "";

                                        await ProcessEventDelegate(JSONCommand,
                                                                   jsonObject,
                                                                   usedRemoteAuthServer.URL.ToString(),
                                                                   lineNumber);

                                    }

                                }

                            }
                            catch (Exception e)
                            {
                                DebugX.Log("Could not load remote change set from '" + usedRemoteAuthServer.URL.ToString() + (LastKnownSHA256HashValue is not null ? "/changeSets?skipUntil=" + LastKnownSHA256HashValue : "/changeSets") + "' line " + lineNumber + ": " + e.Message);
                            }

                        }

                    });

                }

                #endregion

            }
            catch (Exception e)
            {
                DebugX.LogT("Could not load remote change set: " + e.Message);
            }

        }

        #endregion

        #region (protected) LoadChangeSets(Since = null, SkipUntil = null, MatchFilter = null, Skip = null, Take = null, DatabaseFileName = null)

        /// <summary>
        /// Read all change sets from the database file.
        /// </summary>
        /// <param name="DatabaseFileName">The optional database file name.</param>
        protected async Task<(IEnumerable<JObject>, UInt64, String)>

            LoadChangeSetsFromDisc(DateTime?               Since              = null,
                                   String?                 SkipUntil          = null,
                                   Func<String, Boolean>?  MatchFilter        = null,
                                   UInt64?                 Skip               = null,
                                   UInt64?                 Take               = null,
                                   String?                 DatabaseFileName   = null)

        {

            if (MatchFilter is null)
                MatchFilter = line => true;

            var     jsonList           = new List<JObject>();
            String  databaseFileName   = DatabaseFileName ?? this.DatabaseFileName;
            var     totalCount         = 0UL;
            var     skipUntil_reached  = SkipUntil is null;
            var     lastValidLine      = "";
            var     ETag               = "";

            try
            {

                var lines = await File.ReadAllLinesAsync(databaseFileName);

                lines?.ForEachCounted((line, lineNumber) =>
                {

                    if (line.IsNeitherNullNorEmpty() &&
                       !line.StartsWith("#")         &&
                       !line.StartsWith("//"))
                    {

                        try
                        {

                            var jsonLine = JObject.Parse(line);

                            if (jsonLine is JObject &&
                                jsonLine.First is JProperty jsonProperty)
                            {

                                if (jsonProperty.Name.IsNotNullOrEmpty() &&
                                    jsonProperty.Value is JObject jsonObject)
                                {

                                    totalCount++;
                                    lastValidLine = line;

                                    var skip = false;

                                    if (Since is not null)
                                    {

                                        var timestamp = jsonLine["timestamp"]?.Value<DateTime>();

                                        if (timestamp is not null)
                                        {
                                            if (timestamp < Since.Value)
                                                skip = true;
                                        }

                                    }

                                    if (skip              == false &&
                                        skipUntil_reached == false &&
                                        SkipUntil is not null)
                                    {

                                        var sha256hash = jsonLine["sha256hash"]?["hashValue"]?.Value<String>();

                                        if (sha256hash is not null)
                                        {
                                            if (sha256hash == SkipUntil)
                                                skipUntil_reached = true;
                                        }

                                    }

                                    if (skip == false && skipUntil_reached && MatchFilter(line))
                                        jsonList.Add(jsonLine);

                                }

                            }

                        }
                        catch (Exception e)
                        {
                            DebugX.Log("Could not load change set file ''" + databaseFileName + "' line " + lineNumber + ": " + e.Message);
                        }

                    }

                });

                ETag = JObject.Parse(lastValidLine)?["sha256hash"]?["hashValue"]?.Value<String>() ?? "0";

            }
            catch (FileNotFoundException)
            {
                DebugX.LogT("Could not find change set file '" + databaseFileName + "'!");
            }
            catch (Exception e)
            {
                DebugX.LogT("Could not (re-)load change set file '" + databaseFileName + "': " + e.Message);
            }

            return (jsonList.SkipTakeFilter(Skip, Take),
                    totalCount,
                    ETag);

        }

        #endregion

        //ToDo: Receive Network Database Events

        #region (protected virtual) ProcessEvent(Command, Data, Sender = null, LineNumber = null)

        /// <summary>
        /// Process a database event.
        /// </summary>
        /// <param name="Command">The event command.</param>
        /// <param name="Data">The event data.</param>
        /// <param name="Sender">The event sender or file name.</param>
        /// <param name="LineNumber">The event line number within the event file.</param>
        protected virtual async Task ProcessEvent(String   Command,
                                                  JObject  Data,
                                                  String?  Sender       = null,
                                                  UInt64?  LineNumber   = null)
        {

            #region Initial checks

            if (Command.IsNullOrEmpty() || Data == null)
                return;

            NewsPosting      newsPosting;
            NewsBanner       newsBanner;
            FAQ              faq;
            String           ErrorResponse;

            #endregion

            switch (Command)
            {

                #region Add news posting if not exists

                case "addNewsPostingIfNotExists":

                    if (NewsPosting.TryParseJSON(Data,
                                                 users.TryGetValue,
                                                 out newsPosting,
                                                 out ErrorResponse))
                    {

                        if (!_NewsPostings.ContainsKey(newsPosting.Id))
                        {
                            newsPosting.API = this;
                            _NewsPostings.AddAndReturnValue(newsPosting.Id, newsPosting);
                        }

                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, Sender.IsNotNullOrEmpty() ? " via " + Sender : "", LineNumber.HasValue ? ", line " + LineNumber.Value : "", ": ", ErrorResponse));

                    break;

                #endregion


                #region Add news banner if not exists

                case "addNewsBannerIfNotExists":

                    if (NewsBanner.TryParseJSON(Data,
                                                users.TryGetValue,
                                                out newsBanner,
                                                out ErrorResponse))
                    {

                        if (!_NewsBanners.ContainsKey(newsBanner.Id))
                        {
                            newsBanner.API = this;
                            _NewsBanners.AddAndReturnValue(newsBanner.Id, newsBanner);
                        }

                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, Sender.IsNotNullOrEmpty() ? " via " + Sender : "", LineNumber.HasValue ? ", line " + LineNumber.Value : "", ": ", ErrorResponse));

                    break;

                #endregion


                #region Add FAQ if not exists

                case "addFAQIfNotExists":

                    if (FAQ.TryParseJSON(Data,
                                         users.TryGetValue,
                                         out faq,
                                         out ErrorResponse))
                    {

                        if (!_FAQs.ContainsKey(faq.Id))
                        {
                            faq.API = this;
                            _FAQs.AddAndReturnValue(faq.Id, faq);
                        }

                    }

                    else
                        DebugX.Log(String.Concat(nameof(UsersAPI), " ", Command, Sender.IsNotNullOrEmpty() ? " via " + Sender : "", LineNumber.HasValue ? ", line " + LineNumber.Value : "", ": ", ErrorResponse));

                    break;

                #endregion


                default:
                    DebugX.Log(String.Concat(nameof(UsersAPI), ": does not know what to do with event '", Command,
                                             Sender.IsNotNullOrEmpty() ? " via " + Sender : "",
                                             LineNumber.HasValue ? ", line " + LineNumber.Value : "",
                                             "'!"));
                    break;

            }

        }

        #endregion


        #region (protected internal) WriteToDatabaseFile(              MessageType, JSONData, EventTrackingId, ...)

        /// <summary>
        /// Write data to a log file.
        /// </summary>
        /// <param name="MessageType">The type of the message.</param>
        /// <param name="JSONData">The JSON data of the message.</param>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal Task WriteToDatabaseFile(NotificationMessageType  MessageType,
                                                    JObject                  JSONData,
                                                    EventTracking_Id?        EventTrackingId,
                                                    User_Id?                 CurrentUserId   = null)

            => WriteToDatabaseFile(DatabaseFileName,
                                   MessageType,
                                   JSONData,
                                   EventTrackingId,
                                   CurrentUserId);

        #endregion

        #region (protected internal) WriteToDatabaseFile(DatabaseFile, MessageType, JSONData, EventTrackingId, ...)

        /// <summary>
        /// Write data to a database file.
        /// </summary>
        /// <param name="DatabaseFile">The database file.</param>
        /// <param name="MessageType">The type of the message.</param>
        /// <param name="JSONData">The JSON data of the message.</param>
        /// <param name="EventTrackingId">An unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task WriteToDatabaseFile(String                   DatabaseFile,
                                                          NotificationMessageType  MessageType,
                                                          JObject                  JSONData,
                                                          EventTracking_Id?        EventTrackingId,
                                                          User_Id?                 CurrentUserId     = null)
        {

            if (!DisableLogging || !DisableNotifications)
            {

                try
                {

                    var Now          = Timestamp.Now;

                    var JSONMessage  = new JObject(
                                           new JProperty(MessageType.ToString(),  JSONData),
                                           new JProperty("eventTrackingId",       (EventTrackingId ?? EventTracking_Id.New).ToString()),
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


                    #region Write to database file

                    if (!DisableLogging)
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

                                    File.AppendAllText(DatabaseFile ?? DatabaseFileName,
                                                       JSONMessage.ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine);

                                    retry = maxRetries;

                                }
                                catch (IOException ioEx)
                                {
                                    DebugX.Log("Retry " + retry + ": Could not write message '" + MessageType + "' to logfile '" + DatabaseFile + "': " + ioEx.Message);
                                    await Task.Delay(10);
                                    retry++;
                                }
                                catch (Exception e)
                                {
                                    DebugX.Log("Retry " + retry + ": Could not write message '" + MessageType + "' to logfile '" + DatabaseFile + "': " + e.Message);
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

        #region WriteCommentToDatabaseFile(Comment = null, DatabaseFile = APIDatabaseFile, ...)

        /// <summary>
        /// Write a comment or just an empty comment to a database file.
        /// </summary>
        /// <param name="Comment">An optional comment.</param>
        /// <param name="DatabaseFile">An optional database file.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task WriteCommentToDatabaseFile(String            Comment           = null,
                                                     String            DatabaseFile      = null,
                                                     EventTracking_Id? EventTrackingId   = null,
                                                     User_Id?          CurrentUserId     = null)
        {

            if (!DisableLogging || !DisableNotifications)
            {

                try
                {

                    if (!DisableLogging)
                    {

                        try
                        {

                            await LogFileSemaphore.WaitAsync();

                            var retry       = 0;
                            var maxRetries  = 23;
                            var text1       = (Comment ?? "no comment!") + (CurrentUserId.HasValue ? "by " + CurrentUserId.ToString() + " " : "");
                            var text2       = "# --" + (text1 != null ? "< " + text1 + " >" : "");
                            var text3       = text2 + new String('-', Math.Max(10, 200 - text2.Length)) + Environment.NewLine;

                            do
                            {

                                try
                                {
                                    File.AppendAllText(DatabaseFile ?? DatabaseFileName, text3);
                                    retry = maxRetries;
                                }
                                catch (IOException ioEx)
                                {
                                    DebugX.Log("Retry " + retry + ": Could not write comment '" + Comment + "' to logfile '" + DatabaseFile + "': " + ioEx.Message);
                                    await Task.Delay(10);
                                    retry++;
                                }
                                catch (Exception e)
                                {
                                    DebugX.Log("Retry " + retry + ": Could not write comment '" + Comment + "' to logfile '" + DatabaseFile + "': " + e.Message);
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

            if (!DisableLogging)
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

        #endregion


        #region Messages

        // ToDo: Create Mailinglists

        #region Data

        /// <summary>
        /// An enumeration of all messages.
        /// </summary>
        protected internal readonly Dictionary<Message_Id, Message> _Messages;

        /// <summary>
        /// An enumeration of all messages.
        /// </summary>
        public IEnumerable<Message> Messages
        {
            get
            {
                try
                {
                    return MessagesSemaphore.Wait(SemaphoreSlimTimeout)
                               ? _Messages.Values.ToArray()
                               : Array.Empty<Message>();
                }
                finally
                {
                    try
                    {
                        MessagesSemaphore.Release();
                    }
                    catch
                    { }
                }
            }
        }

        #endregion


        #region (protected internal) WriteToDatabaseFileAndNotify(Message, MessageType,  OldMessage = null, ...)

        /// <summary>
        /// Write the given message to the database and send out notifications.
        /// </summary>
        /// <param name="Message">The message.</param>
        /// <param name="MessageType">The user notification.</param>
        /// <param name="OldMessage">The old/updated message.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task WriteToDatabaseFileAndNotify(Message                  Message,
                                                                   NotificationMessageType  MessageType,
                                                                   Message?                 OldMessage        = null,
                                                                   EventTracking_Id?        EventTrackingId   = null,
                                                                   User_Id?                 CurrentUserId     = null)
        {

            if (Message is null)
                throw new ArgumentNullException(nameof(Message),  "The given message must not be null or empty!");

            if (MessageType.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(MessageType),  "The given message type must not be null or empty!");


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(MessageType,
                                      Message.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            await SendNotifications(Message,
                                    MessageType,
                                    OldMessage,
                                    eventTrackingId,
                                    CurrentUserId);

        }

        #endregion

        #region (protected internal) SendNotifications           (Message, MessageTypes, OldMessage = null, ...)

        /// <summary>
        /// Send message notifications.
        /// </summary>
        /// <param name="Message">The message.</param>
        /// <param name="MessageType">The user notification.</param>
        /// <param name="OldMessage">The old/updated message.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task SendNotifications(Message                  Message,
                                                        NotificationMessageType  MessageType,
                                                        Message?                 OldMessage        = null,
                                                        EventTracking_Id?        EventTrackingId   = null,
                                                        User_Id?                 CurrentUserId     = null)
        {

            if (Message is null)
                throw new ArgumentNullException(nameof(Message),  "The given message must not be null or empty!");

            if (MessageType.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(MessageType),  "The given message type must not be null or empty!");


            await SendNotifications(Message,
                                    new[] { MessageType },
                                    OldMessage,
                                    EventTrackingId,
                                    CurrentUserId);

        }


        /// <summary>
        /// Send message notifications.
        /// </summary>
        /// <param name="Message">The message.</param>
        /// <param name="MessageTypes">The user notifications.</param>
        /// <param name="OldMessage">The old/updated message.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task SendNotifications(Message                               Message,
                                                        IEnumerable<NotificationMessageType>  MessageTypes,
                                                        Message?                              OldMessage        = null,
                                                        EventTracking_Id?                     EventTrackingId   = null,
                                                        User_Id?                              CurrentUserId     = null)
        {

            if (Message is null)
                throw new ArgumentNullException(nameof(Message),   "The given message must not be null or empty!");

            var messageTypesHash = new HashSet<NotificationMessageType>(MessageTypes.Where(messageType => !messageType.IsNullOrEmpty));

            if (messageTypesHash.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(MessageTypes),  "The given enumeration of message types must not be null or empty!");

            if (messageTypesHash.Contains(addUserIfNotExists_MessageType))
                messageTypesHash.Add(addUser_MessageType);

            if (messageTypesHash.Contains(addOrUpdateUser_MessageType))
                messageTypesHash.Add(OldMessage == null
                                       ? addUser_MessageType
                                       : updateUser_MessageType);

            var messageTypes = messageTypesHash.ToArray();


            if (!DisableNotifications)
            {


            }

        }

        #endregion

        #region (protected internal) GetMessageSerializator(Request, User)

        //protected internal MessageToJSONDelegate GetMessageSerializator(HTTPRequest  Request,
        //                                                               User         User)
        //{

        //    switch (User?.Id.ToString())
        //    {

        //        default:
        //            return (message,
        //                    embedded,
        //                    ExpandTags,
        //                    ExpandAuthorId,
        //                    includeCryptoHash)

        //                    => message.ToJSON(embedded,
        //                                          ExpandTags,
        //                                          ExpandAuthorId,
        //                                          includeCryptoHash);

        //    }

        //}

        #endregion


        #region AddMessage           (Message, OnAdded = null,                   CurrentUserId = null)

        /// <summary>
        /// A delegate called whenever a message was added.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the message was added.</param>
        /// <param name="Message">The added message.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public delegate Task OnMessageAddedDelegate(DateTime          Timestamp,
                                                    Message           Message,
                                                    EventTracking_Id? EventTrackingId   = null,
                                                    User_Id?          CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a message was added.
        /// </summary>
        public event OnMessageAddedDelegate OnMessageAdded;


        #region (protected internal) _AddMessage(Message,                                OnAdded = null, ...)

        /// <summary>
        /// Add the given message to the API.
        /// </summary>
        /// <param name="Message">A new message to be added to this API.</param>
        /// <param name="OnAdded">A delegate run whenever the message has been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task<Message> _AddMessage(Message                             Message,
                                                           Action<Message, EventTracking_Id>?  OnAdded           = null,
                                                           EventTracking_Id?                   EventTrackingId   = null,
                                                           User_Id?                            CurrentUserId     = null)
        {

            if (Message is null)
                throw new ArgumentNullException(nameof(Message),
                                                "The given message must not be null!");

            if (Message.API != null && Message.API != this)
                throw new ArgumentException    ("The given message is already attached to another API!",
                                                nameof(Message));

            if (_Messages.ContainsKey(Message.Id))
                throw new ArgumentException    ("User group identification '" + Message.Id + "' already exists!",
                                                nameof(Message));

            if (Message.Id.Length < MinMessageIdLength)
                throw new ArgumentException    ("User group identification '" + Message.Id + "' is too short!",
                                                nameof(Message));

            Message.API = this;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(addMessage_MessageType,
                                      Message.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            _Messages.Add(Message.Id, Message);


            var OnMessageAddedLocal = OnMessageAdded;
            if (OnMessageAddedLocal is not null)
                await OnMessageAddedLocal.Invoke(Timestamp.Now,
                                                     Message,
                                                     eventTrackingId,
                                                     CurrentUserId);

            await SendNotifications(Message,
                                    addUser_MessageType,
                                    null,
                                    eventTrackingId,
                                    CurrentUserId);

            OnAdded?.Invoke(Message,
                            eventTrackingId);

            return Message;

        }

        #endregion

        #region AddMessage             (Message,                                OnAdded = null, ...)

        /// <summary>
        /// Add the given message.
        /// </summary>
        /// <param name="Message">A new message.</param>
        /// <param name="OnAdded">A delegate run whenever the message has been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<Message> AddMessage(Message                             Message,
                                              Action<Message, EventTracking_Id>?  OnAdded           = null,
                                              EventTracking_Id?                   EventTrackingId   = null,
                                              User_Id?                            CurrentUserId     = null)
        {

            if (Message is null)
                throw new ArgumentNullException(nameof(Message), "The given message must not be null!");

            try
            {

                return (await MessagesSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _AddMessage(Message,
                                                    OnAdded,
                                                    EventTrackingId,
                                                    CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    MessagesSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion

        #region AddMessageIfNotExists(Message, OnAdded = null,                   CurrentUserId = null)

        #region (protected internal) _AddMessageIfNotExists(Message,                                OnAdded = null, ...)

        /// <summary>
        /// When it has not been created before, add the given message to the API.
        /// </summary>
        /// <param name="Message">A new message to be added to this API.</param>
        /// <param name="OnAdded">A delegate run whenever the message has been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task<Message> _AddMessageIfNotExists(Message                             Message,
                                                                      Action<Message, EventTracking_Id>?  OnAdded           = null,
                                                                      EventTracking_Id?                   EventTrackingId   = null,
                                                                      User_Id?                            CurrentUserId     = null)
        {

            if (Message is null)
                throw new ArgumentNullException(nameof(Message),
                                                "The given message must not be null!");

            if (Message.API != null && Message.API != this)
                throw new ArgumentException    ("The given message is already attached to another API!",
                                                nameof(Message));

            if (_Messages.ContainsKey(Message.Id))
                return _Messages[Message.Id];

            if (Message.Id.Length < MinMessageIdLength)
                throw new ArgumentException    ("User group identification '" + Message.Id + "' is too short!",
                                                nameof(Message));

            Message.API = this;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(addMessageIfNotExists_MessageType,
                                      Message.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            _Messages.Add(Message.Id, Message);

            var OnMessageAddedLocal = OnMessageAdded;
            if (OnMessageAddedLocal is not null)
                await OnMessageAddedLocal.Invoke(Timestamp.Now,
                                                     Message,
                                                     eventTrackingId,
                                                     CurrentUserId);

            await SendNotifications(Message,
                                    addMessageIfNotExists_MessageType,
                                    null,
                                    eventTrackingId,
                                    CurrentUserId);

            OnAdded?.Invoke(Message,
                            eventTrackingId);

            return Message;

        }

        #endregion

        #region AddMessageIfNotExists             (Message,                                OnAdded = null, ...)

        /// <summary>
        /// Add the given message.
        /// </summary>
        /// <param name="Message">A new message.</param>
        /// <param name="OnAdded">A delegate run whenever the message has been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<Message> AddMessageIfNotExists(Message                             Message,
                                                         Action<Message, EventTracking_Id>?  OnAdded           = null,
                                                         EventTracking_Id?                   EventTrackingId   = null,
                                                         User_Id?                            CurrentUserId     = null)
        {

            try
            {

                return (await MessagesSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _AddMessageIfNotExists(Message,
                                                             OnAdded,
                                                             EventTrackingId,
                                                             CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    MessagesSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion

        #region AddOrUpdateMessage   (Message, OnAdded = null, OnUpdated = null, ...)

        #region (protected internal) _AddOrUpdateMessage   (Message,   OnAdded = null, OnUpdated = null, ...)

        /// <summary>
        /// Add or update the given message to/within the API.
        /// </summary>
        /// <param name="Message">A message.</param>
        /// <param name="OnAdded">A delegate run whenever the message has been added successfully.</param>
        /// <param name="OnUpdated">A delegate run whenever the message has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional message identification initiating this command/request.</param>
        protected internal async Task<Message> _AddOrUpdateMessage(Message                             Message,
                                                                   Action<Message, EventTracking_Id>?  OnAdded           = null,
                                                                   Action<Message, EventTracking_Id>?  OnUpdated         = null,
                                                                   EventTracking_Id?                   EventTrackingId   = null,
                                                                   User_Id?                            CurrentUserId     = null)
        {

            if (Message is null)
                throw new ArgumentNullException(nameof(Message),
                                                "The given message must not be null!");

            if (Message.API != null && Message.API != this)
                throw new ArgumentException    ("The given message is already attached to another API!",
                                                nameof(Message));

            if (_Messages.ContainsKey(Message.Id))
                return _Messages[Message.Id];

            if (Message.Id.Length < MinMessageIdLength)
                throw new ArgumentException    ("Message identification '" + Message.Id + "' is too short!",
                                                nameof(Message));

            Message.API = this;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(addOrUpdateMessage_MessageType,
                                      Message.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            if (_Messages.TryGetValue(Message.Id, out var OldMessage))
            {
                _Messages.Remove(OldMessage.Id);
                Message.CopyAllLinkedDataFrom(OldMessage);
            }

            _Messages.Add(Message.Id, Message);

            if (OldMessage != null)
            {

                var OnMessageUpdatedLocal = OnMessageUpdated;
                if (OnMessageUpdatedLocal is not null)
                    await OnMessageUpdatedLocal.Invoke(Timestamp.Now,
                                                           Message,
                                                           OldMessage,
                                                           eventTrackingId,
                                                           CurrentUserId);

                await SendNotifications(Message,
                                        updateMessage_MessageType,
                                        OldMessage,
                                        eventTrackingId,
                                        CurrentUserId);

                OnUpdated?.Invoke(Message,
                                  eventTrackingId);

            }
            else
            {

                var OnMessageAddedLocal = OnMessageAdded;
                if (OnMessageAddedLocal is not null)
                    await OnMessageAddedLocal?.Invoke(Timestamp.Now,
                                                          Message,
                                                          eventTrackingId,
                                                          CurrentUserId);

                await SendNotifications(Message,
                                        addMessage_MessageType,
                                        null,
                                        eventTrackingId,
                                        CurrentUserId);

                OnAdded?.Invoke(Message,
                                eventTrackingId);

            }

            return Message;

        }

        #endregion

        #region AddOrUpdateMessage   (Message,   OnAdded = null, OnUpdated = null, ...)

        /// <summary>
        /// Add or update the given message to/within the API.
        /// </summary>
        /// <param name="Message">A message.</param>
        /// <param name="OnAdded">A delegate run whenever the message has been added successfully.</param>
        /// <param name="OnUpdated">A delegate run whenever the message has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional message identification initiating this command/request.</param>
        public async Task<Message> AddOrUpdateMessage(Message                             Message,
                                                      Action<Message, EventTracking_Id>?  OnAdded           = null,
                                                      Action<Message, EventTracking_Id>?  OnUpdated         = null,
                                                      EventTracking_Id?                   EventTrackingId   = null,
                                                      User_Id?                            CurrentUserId     = null)
        {

            if (Message is null)
                throw new ArgumentNullException(nameof(Message), "The given message must not be null!");

            try
            {

                return (await MessagesSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _AddOrUpdateMessage(Message,
                                                            OnAdded,
                                                            OnUpdated,
                                                            EventTrackingId,
                                                            CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    MessagesSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion

        #region UpdateMessage        (Message,                 OnUpdated = null, ...)

        /// <summary>
        /// A delegate called whenever a message was updated.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the message was updated.</param>
        /// <param name="Message">The updated message.</param>
        /// <param name="OldMessage">The old message.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">The invoking message identification</param>
        public delegate Task OnMessageUpdatedDelegate(DateTime          Timestamp,
                                                      Message           Message,
                                                      Message           OldMessage,
                                                      EventTracking_Id? EventTrackingId   = null,
                                                      User_Id?          CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a message was updated.
        /// </summary>
        public event OnMessageUpdatedDelegate OnMessageUpdated;


        #region (protected internal) _UpdateMessage(Message, OnUpdated = null, ...)

        /// <summary>
        /// Update the given message to/within the API.
        /// </summary>
        /// <param name="Message">A message.</param>
        /// <param name="OnUpdated">A delegate run whenever the message has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional message identification initiating this command/request.</param>
        protected internal async Task<Message> _UpdateMessage(Message                             Message,
                                                              Action<Message, EventTracking_Id>?  OnUpdated         = null,
                                                              EventTracking_Id?                   EventTrackingId   = null,
                                                              User_Id?                            CurrentUserId     = null)
        {

            if (Message is null)
                throw new ArgumentNullException(nameof(Message),
                                                "The given message must not be null!");

            if (Message.API != null && Message.API != this)
                throw new ArgumentException    ("The given message is already attached to another API!",
                                                nameof(Message));

            if (!_Messages.TryGetValue(Message.Id, out var OldMessage))
                throw new ArgumentException    ("The given message '" + Message.Id + "' does not exists in this API!",
                                                nameof(Message));

            Message.API = this;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(updateMessage_MessageType,
                                      Message.ToJSON(),
                                      eventTrackingId,
                                      CurrentUserId);

            _Messages.Remove(OldMessage.Id);
            Message.CopyAllLinkedDataFrom(OldMessage);


            var OnMessageUpdatedLocal = OnMessageUpdated;
            if (OnMessageUpdatedLocal is not null)
                await OnMessageUpdatedLocal.Invoke(Timestamp.Now,
                                                       Message,
                                                       OldMessage,
                                                       eventTrackingId,
                                                       CurrentUserId);

            await SendNotifications(Message,
                                    updateMessage_MessageType,
                                    OldMessage,
                                    eventTrackingId,
                                    CurrentUserId);

            OnUpdated?.Invoke(Message,
                              eventTrackingId);

            return Message;

        }

        #endregion

        #region UpdateMessage             (Message, OnUpdated = null, ...)

        /// <summary>
        /// Update the given message to/within the API.
        /// </summary>
        /// <param name="Message">A message.</param>
        /// <param name="OnUpdated">A delegate run whenever the message has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional message identification initiating this command/request.</param>
        public async Task<Message> UpdateMessage(Message                             Message,
                                                 Action<Message, EventTracking_Id>?  OnUpdated         = null,
                                                 EventTracking_Id?                   EventTrackingId   = null,
                                                 User_Id?                            CurrentUserId     = null)
        {

            if (Message is null)
                throw new ArgumentNullException(nameof(Message), "The given message must not be null!");

            try
            {

                return (await MessagesSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _UpdateMessage(Message,
                                                       OnUpdated,
                                                       EventTrackingId,
                                                       CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    MessagesSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion


        #region (protected internal) _UpdateMessage(MessageId, UpdateDelegate, OnUpdated = null, ...)

        /// <summary>
        /// Update the given message.
        /// </summary>
        /// <param name="MessageId">An message identification.</param>
        /// <param name="UpdateDelegate">A delegate to update the given message.</param>
        /// <param name="OnUpdated">A delegate run whenever the message has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional message identification initiating this command/request.</param>
        protected internal async Task<Message> _UpdateMessage(Message_Id                          MessageId,
                                                              Action<Message.Builder>             UpdateDelegate,
                                                              Action<Message, EventTracking_Id>?  OnUpdated         = null,
                                                              EventTracking_Id?                   EventTrackingId   = null,
                                                              User_Id?                            CurrentUserId     = null)
        {

            if (MessageId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(MessageId),
                                                "The given message identification must not be null or empty!");

            if (UpdateDelegate == null)
                throw new ArgumentNullException(nameof(UpdateDelegate),
                                                "The given update delegate must not be null!");

            if (!_Messages.TryGetValue(MessageId, out var OldMessage))
                throw new ArgumentException    ("The given message '" + MessageId + "' does not exists in this API!",
                                                nameof(MessageId));

            var Builder = OldMessage.ToBuilder();
            UpdateDelegate(Builder);
            var NewMessage = Builder.ToImmutable;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(updateMessage_MessageType,
                                      NewMessage.ToJSON(),
                                      eventTrackingId,
                                      CurrentUserId);

            _Messages.Remove(OldMessage.Id);
            NewMessage.CopyAllLinkedDataFrom(OldMessage);


            var OnMessageUpdatedLocal = OnMessageUpdated;
            if (OnMessageUpdatedLocal is not null)
                await OnMessageUpdatedLocal.Invoke(Timestamp.Now,
                                                       NewMessage,
                                                       OldMessage,
                                                       eventTrackingId,
                                                       CurrentUserId);

            await SendNotifications(NewMessage,
                                    updateMessage_MessageType,
                                    OldMessage,
                                    eventTrackingId,
                                    CurrentUserId);

            OnUpdated?.Invoke(NewMessage,
                              eventTrackingId);

            return NewMessage;

        }

        #endregion

        #region UpdateMessage             (MessageId, UpdateDelegate, OnUpdated = null, ...)

        /// <summary>
        /// Update the given message.
        /// </summary>
        /// <param name="MessageId">An message identification.</param>
        /// <param name="UpdateDelegate">A delegate to update the given message.</param>
        /// <param name="OnUpdated">A delegate run whenever the message has been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional message identification initiating this command/request.</param>
        public async Task<Message> UpdateMessage(Message_Id                          MessageId,
                                                 Action<Message.Builder>             UpdateDelegate,
                                                 Action<Message, EventTracking_Id>?  OnUpdated         = null,
                                                 EventTracking_Id?                   EventTrackingId   = null,
                                                 User_Id?                            CurrentUserId     = null)
        {

            if (MessageId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(MessageId), "The given message identification must not be null or empty!");

            try
            {

                return (await MessagesSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _UpdateMessage(MessageId,
                                                       UpdateDelegate,
                                                       OnUpdated,
                                                       EventTrackingId,
                                                       CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    MessagesSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion

        #region RemoveMessage(Message, OnRemoved = null, ...)

        /// <summary>
        /// A delegate called whenever a message was removed.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the message was removed.</param>
        /// <param name="Message">The removed message.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">The invoking message identification</param>
        public delegate Task OnMessageRemovedDelegate(DateTime           Timestamp,
                                                      Message            Message,
                                                      EventTracking_Id?  EventTrackingId   = null,
                                                      User_Id?           CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a message was removed.
        /// </summary>
        public event OnMessageRemovedDelegate OnMessageRemoved;


        #region (class) DeleteMessageResult

        public class DeleteMessageResult
        {

            public Boolean     IsSuccess           { get; }

            public I18NString  ErrorDescription    { get; }


            private DeleteMessageResult(Boolean     IsSuccess,
                                          I18NString  ErrorDescription  = null)
            {
                this.IsSuccess         = IsSuccess;
                this.ErrorDescription  = ErrorDescription;
            }


            public static DeleteMessageResult Success

                => new DeleteMessageResult(true);

            public static DeleteMessageResult Failed(I18NString Reason)

                => new DeleteMessageResult(false,
                                             Reason);

            public static DeleteMessageResult Failed(Exception Exception)

                => new DeleteMessageResult(false,
                                             I18NString.Create(Languages.en,
                                                               Exception.Message));

            public override String ToString()

                => IsSuccess
                       ? "Success"
                       : "Failed" + (ErrorDescription.IsNullOrEmpty()
                                         ? ": " + ErrorDescription.FirstText()
                                         : "!");

        }

        #endregion

        #region (protected internal virtual) CanDeleteMessage(Message)

        /// <summary>
        /// Determines whether the message can safely be removed from the API.
        /// </summary>
        /// <param name="Message">The message to be removed.</param>
        protected internal virtual I18NString CanDeleteMessage(Message Message)
        {
            return new I18NString(Languages.en, "Currently not possible!");
        }

        #endregion


        #region (protected internal) _RemoveMessage(Message, OnRemoved = null, ...)

        /// <summary>
        /// Remove the given message from the API.
        /// </summary>
        /// <param name="Message">The message to be removed from this API.</param>
        /// <param name="OnRemoved">A delegate run whenever the message has been removed successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional message identification initiating this command/request.</param>
        protected internal async Task<DeleteMessageResult> _RemoveMessage(Message                             Message,
                                                                          Action<Message, EventTracking_Id>?  OnRemoved         = null,
                                                                          EventTracking_Id?                   EventTrackingId   = null,
                                                                          User_Id?                            CurrentUserId     = null)
        {

            if (Message is null)
                throw new ArgumentNullException(nameof(Message),
                                                "The given message must not be null!");

            if (Message.API != this || !_Messages.TryGetValue(Message.Id, out Message MessageToBeRemoved))
                throw new ArgumentException    ("The given message '" + Message.Id + "' does not exists in this API!",
                                                nameof(Message));


            var result = CanDeleteMessage(Message);

            if (result == null)
            {

                var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

                await WriteToDatabaseFile(removeMessage_MessageType,
                                          Message.ToJSON(false),
                                          eventTrackingId,
                                          CurrentUserId);

                _Messages.Remove(Message.Id);


                var OnMessageRemovedLocal = OnMessageRemoved;
                if (OnMessageRemovedLocal is not null)
                    await OnMessageRemovedLocal?.Invoke(Timestamp.Now,
                                                            Message,
                                                            eventTrackingId,
                                                            CurrentUserId);

                await SendNotifications(Message,
                                        removeMessage_MessageType,
                                        null,
                                        eventTrackingId,
                                        CurrentUserId);

                OnRemoved?.Invoke(Message,
                                  eventTrackingId);

                return DeleteMessageResult.Success;

            }
            else
                return DeleteMessageResult.Failed(result);

        }

        #endregion

        #region RemoveMessage             (Message, OnRemoved = null, ...)

        /// <summary>
        /// Remove the given message from the API.
        /// </summary>
        /// <param name="Message">The message to be removed from this API.</param>
        /// <param name="OnRemoved">A delegate run whenever the message has been removed successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional message identification initiating this command/request.</param>
        public async Task<DeleteMessageResult> RemoveMessage(Message                             Message,
                                                             Action<Message, EventTracking_Id>?  OnRemoved         = null,
                                                             EventTracking_Id?                   EventTrackingId   = null,
                                                             User_Id?                            CurrentUserId     = null)
        {

            if (Message is null)
                throw new ArgumentNullException(nameof(Message), "The given message must not be null!");

            try
            {

                return (await MessagesSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _RemoveMessage(Message,
                                                       OnRemoved,
                                                       EventTrackingId,
                                                       CurrentUserId)

                            : null;

            }
            catch (Exception e)
            {
                return DeleteMessageResult.Failed(e);
            }
            finally
            {
                try
                {
                    MessagesSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion



        #region MessageExists(MessageId)

        /// <summary>
        /// Determines whether the given message identification exists within this API.
        /// </summary>
        /// <param name="MessageId">The unique identification of an message.</param>
        protected internal Boolean _MessageExists(Message_Id MessageId)

            => !MessageId.IsNullOrEmpty && _Messages.ContainsKey(MessageId);


        /// <summary>
        /// Determines whether the given message identification exists within this API.
        /// </summary>
        /// <param name="MessageId">The unique identification of an message.</param>
        public Boolean MessageExists(Message_Id MessageId)
        {

            try
            {

                if (MessagesSemaphore.Wait(SemaphoreSlimTimeout) &&
                    _MessageExists(MessageId))
                {
                    return true;
                }

            }
            catch
            { }
            finally
            {
                try
                {
                    MessagesSemaphore.Release();
                }
                catch
                { }
            }

            return false;

        }

        #endregion

        #region GetMessage   (MessageId)

        /// <summary>
        /// Get the message having the given unique identification.
        /// </summary>
        /// <param name="MessageId">The unique identification of an message.</param>
        protected internal Message _GetMessage(Message_Id MessageId)
        {

            if (!MessageId.IsNullOrEmpty && _Messages.TryGetValue(MessageId, out Message message))
                return message;

            return null;

        }


        /// <summary>
        /// Get the message having the given unique identification.
        /// </summary>
        /// <param name="MessageId">The unique identification of the message.</param>
        public Message GetMessage(Message_Id MessageId)
        {

            try
            {

                if (MessagesSemaphore.Wait(SemaphoreSlimTimeout))
                    return _GetMessage(MessageId);

            }
            catch
            { }
            finally
            {
                try
                {
                    MessagesSemaphore.Release();
                }
                catch
                { }
            }

            return null;

        }

        #endregion

        #region TryGetMessage(MessageId, out Message)

        /// <summary>
        /// Try to get the message having the given unique identification.
        /// </summary>
        /// <param name="MessageId">The unique identification of an message.</param>
        /// <param name="Message">The message.</param>
        protected internal Boolean _TryGetMessage(Message_Id MessageId, out Message Message)
        {

            if (!MessageId.IsNullOrEmpty && _Messages.TryGetValue(MessageId, out Message message))
            {
                Message = message;
                return true;
            }

            Message = null;
            return false;

        }


        /// <summary>
        /// Try to get the message having the given unique identification.
        /// </summary>
        /// <param name="MessageId">The unique identification of an message.</param>
        /// <param name="Message">The message.</param>
        public Boolean TryGetMessage(Message_Id   MessageId,
                                         out Message  Message)
        {

            try
            {

                if (MessagesSemaphore.Wait(SemaphoreSlimTimeout) &&
                    _TryGetMessage(MessageId, out Message message))
                {
                    Message = message;
                    return true;
                }

            }
            catch
            { }
            finally
            {
                try
                {
                    MessagesSemaphore.Release();
                }
                catch
                { }
            }

            Message = null;
            return false;

        }

        #endregion

        #endregion

        #region Notifications

        private JObject GetNotifications(IUser User)
        {

            var notificationsJSON = User.GetNotificationInfos();

            notificationsJSON.AddFirst(new JProperty("notificationGroups", new JArray(
                                           _NotificationGroups.Values.Select(notificationGroup => notificationGroup.ToJSON())
                                      )));

            return notificationsJSON;

        }

        private JObject GetNotification(IUser User, UInt32 NotificationId)
        {

            var notificationJSON = User.GetNotificationInfo(NotificationId);

            notificationJSON.AddFirst(new JProperty("notificationGroups", new JArray(
                                          _NotificationGroups.Values.Select(notificationGroup => notificationGroup.ToJSON())
                                     )));

            return notificationJSON;

        }

        private JObject GetNotifications(IOrganization Organization)
        {

            var notificationsJSON = Organization.GetNotificationInfos();

            notificationsJSON.AddFirst(new JProperty("notificationGroups", new JArray(
                                           _NotificationGroups.Values.Select(notificationGroup => notificationGroup.ToJSON())
                                      )));

            return notificationsJSON;

        }




        // ToDo: Add locks
        // ToDo: Add logging!

        #region AddNotification(User,           NotificationType,                           CurrentUserId = null)

        protected async Task _AddNotification<T>(IUser              User,
                                                 T                  NotificationType,
                                                 EventTracking_Id?  EventTrackingId   = null,
                                                 User_Id?           CurrentUserId     = null)

            where T : ANotification

        {

            User.AddNotification(NotificationType,
                                 async update => await WriteToDatabaseFile(addNotification_MessageType,
                                                                           update.ToJSON(false).AddFirstAndReturn(new JProperty("userId", User.Id.ToString())),
                                                                           EventTrackingId,
                                                                           CurrentUserId));

        }


        public async Task AddNotification<T>(IUser              User,
                                             T                  NotificationType,
                                             EventTracking_Id?  EventTrackingId   = null,
                                             User_Id?           CurrentUserId     = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                await _AddNotification(User,
                                       NotificationType,
                                       EventTrackingId,
                                       CurrentUserId);

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region AddNotification(UserId,         NotificationType,                           CurrentUserId = null)

        public async Task AddNotification<T>(User_Id           UserId,
                                             T                 NotificationType,
                                             EventTracking_Id? EventTrackingId   = null,
                                             User_Id?          CurrentUserId     = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                if (users.TryGetValue(UserId, out var User))
                {

                    User.AddNotification(NotificationType,
                                         async update => await WriteToDatabaseFile(addNotification_MessageType,
                                                                                   update.ToJSON(false).AddFirstAndReturn(new JProperty("userId", User.Id.ToString())),
                                                                                   EventTrackingId,
                                                                                   CurrentUserId));

                }

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region AddNotification(User,           NotificationType, NotificationMessageType,  CurrentUserId = null)

        public async Task AddNotification<T>(IUser                    User,
                                             T                        NotificationType,
                                             NotificationMessageType  NotificationMessageType,
                                             EventTracking_Id?        EventTrackingId   = null,
                                             User_Id?                 CurrentUserId     = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                User.AddNotification(NotificationType,
                                     NotificationMessageType,
                                     async update => await WriteToDatabaseFile(addNotification_MessageType,
                                                                               update.ToJSON(false).AddFirstAndReturn(new JProperty("userId", User.Id.ToString())),
                                                                               EventTrackingId,
                                                                               CurrentUserId));

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region AddNotification(UserId,         NotificationType, NotificationMessageType,  CurrentUserId = null)

        public async Task AddNotification<T>(User_Id                  UserId,
                                             T                        NotificationType,
                                             NotificationMessageType  NotificationMessageType,
                                             EventTracking_Id?        EventTrackingId   = null,
                                             User_Id?                 CurrentUserId     = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                if (users.TryGetValue(UserId, out var User))
                {

                    User.AddNotification(NotificationType,
                                         NotificationMessageType,
                                         async update => await WriteToDatabaseFile(addNotification_MessageType,
                                                                                   update.ToJSON(false).AddFirstAndReturn(new JProperty("userId", User.Id.ToString())),
                                                                                   EventTrackingId,
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

        #region AddNotification(User,           NotificationType, NotificationMessageTypes, ...)

        public async Task AddNotification<T>(IUser                                 User,
                                             T                                     NotificationType,
                                             IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                             EventTracking_Id?                     EventTrackingId   = null,
                                             User_Id?                              CurrentUserId     = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                User.AddNotification(NotificationType,
                                     NotificationMessageTypes,
                                     async update => await WriteToDatabaseFile(addNotification_MessageType,
                                                                               update.ToJSON(false).AddFirstAndReturn(new JProperty("userId", User.Id.ToString())),
                                                                               EventTrackingId,
                                                                               CurrentUserId));

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region AddNotification(UserId,         NotificationType, NotificationMessageTypes, ...)

        public async Task AddNotification<T>(User_Id                               UserId,
                                             T                                     NotificationType,
                                             IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                             EventTracking_Id?                     EventTrackingId   = null,
                                             User_Id?                              CurrentUserId     = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                if (users.TryGetValue(UserId, out var User))
                {

                    User.AddNotification(NotificationType,
                                         NotificationMessageTypes,
                                         async update => await WriteToDatabaseFile(addNotification_MessageType,
                                                                                   update.ToJSON(false).AddFirstAndReturn(new JProperty("userId", User.Id.ToString())),
                                                                                   EventTrackingId,
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


        #region AddNotification(Organization,   NotificationType,                           CurrentUserId = null)

        public async Task AddNotification<T>(IOrganization      Organization,
                                             T                  NotificationType,
                                             EventTracking_Id?  EventTrackingId   = null,
                                             User_Id?           CurrentUserId     = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                Organization.AddNotification(NotificationType,
                                             async update => await WriteToDatabaseFile(addNotification_MessageType,
                                                                                       update.ToJSON(false).AddFirstAndReturn(new JProperty("organizationId", Organization.Id.ToString())),
                                                                                       EventTrackingId,
                                                                                       CurrentUserId));

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region AddNotification(OrganizationId, NotificationType,                           CurrentUserId = null)

        public async Task AddNotification<T>(Organization_Id   OrganizationId,
                                             T                 NotificationType,
                                             EventTracking_Id? EventTrackingId   = null,
                                             User_Id?          CurrentUserId     = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                if (organizations.TryGetValue(OrganizationId, out var Organization))
                {

                    Organization.AddNotification(NotificationType,
                                                 async update => await WriteToDatabaseFile(addNotification_MessageType,
                                                                                           update.ToJSON(false).AddFirstAndReturn(new JProperty("organizationId", Organization.Id.ToString())),
                                                                                           EventTrackingId,
                                                                                           CurrentUserId));

                }

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region AddNotification(Organization,   NotificationType, NotificationMessageType,  CurrentUserId = null)

        public async Task AddNotification<T>(IOrganization            Organization,
                                             T                        NotificationType,
                                             NotificationMessageType  NotificationMessageType,
                                             EventTracking_Id?        EventTrackingId   = null,
                                             User_Id?                 CurrentUserId     = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                Organization.AddNotification(NotificationType,
                                             NotificationMessageType,
                                             async update => await WriteToDatabaseFile(addNotification_MessageType,
                                                                                       update.ToJSON(false).AddFirstAndReturn(new JProperty("organizationId", Organization.Id.ToString())),
                                                                                       EventTrackingId,
                                                                                       CurrentUserId));

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region AddNotification(OrganizationId, NotificationType, NotificationMessageType,  CurrentUserId = null)

        public async Task AddNotification<T>(Organization_Id          OrganizationId,
                                             T                        NotificationType,
                                             NotificationMessageType  NotificationMessageType,
                                             EventTracking_Id?        EventTrackingId   = null,
                                             User_Id?                 CurrentUserId     = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                if (organizations.TryGetValue(OrganizationId, out var Organization))
                {

                    Organization.AddNotification(NotificationType,
                                                 NotificationMessageType,
                                                 async update => await WriteToDatabaseFile(addNotification_MessageType,
                                                                                           update.ToJSON(false).AddFirstAndReturn(new JProperty("organizationId", Organization.Id.ToString())),
                                                                                           EventTrackingId,
                                                                                           CurrentUserId));

                }

                else
                    throw new ArgumentException("The given organization '" + OrganizationId + "' is unknown!");

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region AddNotification(Organization,   NotificationType, NotificationMessageTypes, ...)

        public async Task AddNotification<T>(IOrganization                         Organization,
                                             T                                     NotificationType,
                                             IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                             EventTracking_Id?                     EventTrackingId   = null,
                                             User_Id?                              CurrentUserId     = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                Organization.AddNotification(NotificationType,
                                             NotificationMessageTypes,
                                             async update => await WriteToDatabaseFile(addNotification_MessageType,
                                                                                       update.ToJSON(false).AddFirstAndReturn(new JProperty("organizationId", Organization.Id.ToString())),
                                                                                       EventTrackingId,
                                                                                       CurrentUserId));

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region AddNotification(OrganizationId, NotificationType, NotificationMessageTypes, ...)

        public async Task AddNotification<T>(Organization_Id                       OrganizationId,
                                             T                                     NotificationType,
                                             IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                             EventTracking_Id?                     EventTrackingId   = null,
                                             User_Id?                              CurrentUserId     = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                if (organizations.TryGetValue(OrganizationId, out var Organization))
                {

                    Organization.AddNotification(NotificationType,
                                                 NotificationMessageTypes,
                                                 async update => await WriteToDatabaseFile(addNotification_MessageType,
                                                                                           update.ToJSON(false).AddFirstAndReturn(new JProperty("organizationId", Organization.Id.ToString())),
                                                                                           EventTrackingId,
                                                                                           CurrentUserId));

                }

                else
                    throw new ArgumentException("The given organization '" + OrganizationId + "' is unknown!");

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion



        #region GetNotifications  (User,           NotificationMessageType = null)

        public IEnumerable<ANotification> GetNotifications(IUser                     User,
                                                           NotificationMessageType?  NotificationMessageType = null)
        {

            if (UsersSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return User.GetNotifications(NotificationMessageType);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        UsersSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return Array.Empty<ANotification>();

        }

        #endregion

        #region GetNotifications  (UserId,         NotificationMessageType = null)

        public IEnumerable<ANotification> GetNotifications(User_Id                   UserId,
                                                           NotificationMessageType?  NotificationMessageType = null)
        {

            if (UsersSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    if (users.TryGetValue(UserId, out var user))
                        return user.GetNotifications(NotificationMessageType);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        UsersSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return Array.Empty<ANotification>();

        }

        #endregion


        #region GetNotificationsOf(User,           params NotificationMessageTypes)

        public IEnumerable<T> GetNotificationsOf<T>(IUser                             User,
                                                    params NotificationMessageType[]  NotificationMessageTypes)

            where T : ANotification

        {

            if (UsersSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    return User.GetNotificationsOf<T>(NotificationMessageTypes);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        UsersSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return Array.Empty<T>();

        }

        #endregion

        #region GetNotificationsOf(UserId,         params NotificationMessageTypes)

        public IEnumerable<T> GetNotificationsOf<T>(User_Id                           UserId,
                                                    params NotificationMessageType[]  NotificationMessageTypes)

            where T : ANotification

        {

            if (UsersSemaphore.Wait(SemaphoreSlimTimeout))
            {
                try
                {

                    if (users.TryGetValue(UserId, out var user))
                        return user.GetNotificationsOf<T>(NotificationMessageTypes);

                }
                catch
                { }
                finally
                {
                    try
                    {
                        UsersSemaphore.Release();
                    }
                    catch
                    { }
                }
            }

            return Array.Empty<T>();

        }

        #endregion

        #region GetNotificationsOf(Organization,   params NotificationMessageTypes)

        public IEnumerable<T> GetNotificationsOf<T>(Organization                      Organization,
                                                    params NotificationMessageType[]  NotificationMessageTypes)

            where T : ANotification

        {

            if (UsersSemaphore.Wait(SemaphoreSlimTimeout))
            {
                if (OrganizationsSemaphore.Wait(SemaphoreSlimTimeout))
                {
                    try
                    {

                        return Organization.
                                   GetMeAndAllMyParents(parent => parent != NoOwner).
                                   SelectMany          (parent => parent.User2OrganizationEdges).
                                   SelectMany          (edge   => edge.Source.GetNotificationsOf<T>(NotificationMessageTypes));

                    }
                    catch
                    { }
                    finally
                    {
                        try
                        {
                            UsersSemaphore.Release();
                        }
                        catch
                        { }

                        try
                        {
                            OrganizationsSemaphore.Release();
                        }
                        catch
                        { }
                    }
                }
                else
                    UsersSemaphore.Release();
            }

            return Array.Empty<T>();

        }

        #endregion

        #region GetNotificationsOf(OrganizationId, params NotificationMessageTypes)

        public IEnumerable<T> GetNotificationsOf<T>(Organization_Id                   OrganizationId,
                                                    params NotificationMessageType[]  NotificationMessageTypes)

            where T : ANotification

        {

            if (UsersSemaphore.Wait(SemaphoreSlimTimeout))
            {
                if (OrganizationsSemaphore.Wait(SemaphoreSlimTimeout))
                {
                    try
                    {

                        if (organizations.TryGetValue(OrganizationId, out var organization))
                            return organization.
                                       GetMeAndAllMyParents(parent => parent != NoOwner).
                                       SelectMany          (parent => parent.User2OrganizationEdges).
                                       SelectMany          (edge   => edge.Source.GetNotificationsOf<T>(NotificationMessageTypes));

                    }
                    catch
                    { }
                    finally
                    {
                        try
                        {
                            UsersSemaphore.Release();
                        }
                        catch
                        { }

                        try
                        {
                            OrganizationsSemaphore.Release();
                        }
                        catch
                        { }
                    }
                }
                else
                    UsersSemaphore.Release();
            }

            return Array.Empty<T>();

        }

        #endregion

        #region GetNotificationsOf(UserGroup,      params NotificationMessageTypes)

        public IEnumerable<T> GetNotificationsOf<T>(UserGroup                         UserGroup,
                                                    params NotificationMessageType[]  NotificationMessageTypes)

            where T : ANotification

        {

            if (UsersSemaphore.Wait(SemaphoreSlimTimeout))
            {
                if (UserGroupsSemaphore.Wait(SemaphoreSlimTimeout))
                {
                    try
                    {

                        return UserGroup.
                                   GetMeAndAllMyParents().
                                   SelectMany(parent => parent.User2UserGroupEdges).
                                   SelectMany(edge   => edge.Source.GetNotificationsOf<T>(NotificationMessageTypes));

                    }
                    catch
                    { }
                    finally
                    {
                        try
                        {
                            UsersSemaphore.Release();
                        }
                        catch
                        { }

                        try
                        {
                            UserGroupsSemaphore.Release();
                        }
                        catch
                        { }
                    }
                }
                else
                    UsersSemaphore.Release();
            }

            return Array.Empty<T>();

        }

        #endregion

        #region GetNotificationsOf(UserGroupId,    params NotificationMessageTypes)

        public IEnumerable<T> GetNotificationsOf<T>(UserGroup_Id                      UserGroupId,
                                                    params NotificationMessageType[]  NotificationMessageTypes)

            where T : ANotification

        {

            if (UsersSemaphore.Wait(SemaphoreSlimTimeout))
            {
                if (UserGroupsSemaphore.Wait(SemaphoreSlimTimeout))
                {
                    try
                    {

                        if (userGroups.TryGetValue(UserGroupId, out var userGroup))
                            return userGroup.
                                       GetMeAndAllMyParents().
                                       SelectMany(parent => parent.User2UserGroupEdges).
                                       SelectMany(edge   => edge.Source.GetNotificationsOf<T>(NotificationMessageTypes));

                    }
                    catch
                    { }
                    finally
                    {
                        try
                        {
                            UsersSemaphore.Release();
                        }
                        catch
                        { }

                        try
                        {
                            UserGroupsSemaphore.Release();
                        }
                        catch
                        { }
                    }
                }
                else
                    UsersSemaphore.Release();
            }

            return Array.Empty<T>();

        }

        #endregion



        #region GetNotifications  (User,   NotificationMessageTypeFilter)

        public IEnumerable<ANotification> GetNotifications(IUser                                   User,
                                                           Func<NotificationMessageType, Boolean>  NotificationMessageTypeFilter)

            => User.GetNotifications(NotificationMessageTypeFilter);

        #endregion

        #region GetNotifications  (UserId, NotificationMessageTypeFilter)

        public IEnumerable<ANotification> GetNotifications(User_Id                                 UserId,
                                                           Func<NotificationMessageType, Boolean>  NotificationMessageTypeFilter)

            => TryGetUser(UserId, out var user) && user is not null
                   ? user.GetNotifications(NotificationMessageTypeFilter)
                   : Array.Empty<ANotification>();

        #endregion

        #region GetNotificationsOf(User,   NotificationMessageTypeFilter)

        public IEnumerable<T> GetNotificationsOf<T>(IUser                                   User,
                                                    Func<NotificationMessageType, Boolean>  NotificationMessageTypeFilter)

            where T : ANotification

            => User.GetNotificationsOf<T>(NotificationMessageTypeFilter);

        #endregion

        #region GetNotificationsOf(UserId, NotificationMessageTypeFilter)

        public IEnumerable<T> GetNotificationsOf<T>(User_Id                                 UserId,
                                                    Func<NotificationMessageType, Boolean>  NotificationMessageTypeFilter)

            where T : ANotification

            => TryGetUser(UserId, out var user) && user is not null
                   ? user.GetNotificationsOf<T>(NotificationMessageTypeFilter)
                   : Array.Empty<T>();

        #endregion


        #region RemoveNotification(User,           NotificationType, ...)

        public async Task RemoveNotification<T>(IUser              User,
                                                T                  NotificationType,
                                                EventTracking_Id?  EventTrackingId   = null,
                                                User_Id?           CurrentUserId     = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                await User.RemoveNotification(NotificationType,
                                              async update => await WriteToDatabaseFile(removeNotification_MessageType,
                                                                                        update.ToJSON(false).AddFirstAndReturn(new JProperty("userId", User.Id.ToString())),
                                                                                        EventTrackingId,
                                                                                        CurrentUserId));

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region RemoveNotification(UserId,         NotificationType, ...)

        public async Task RemoveNotification<T>(User_Id           UserId,
                                                T                 NotificationType,
                                                EventTracking_Id? EventTrackingId   = null,
                                                User_Id?          CurrentUserId     = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                if (users.TryGetValue(UserId, out var user) && user is not null)
                {

                    user.RemoveNotification(NotificationType,
                                            async update => await WriteToDatabaseFile(removeNotification_MessageType,
                                                                                      update.ToJSON(false).AddFirstAndReturn(new JProperty("userId", UserId.ToString())),
                                                                                      EventTrackingId,
                                                                                      CurrentUserId));

                }

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion


        #region RemoveNotification(Organization,   NotificationType, ...)

        public async Task RemoveNotification<T>(IOrganization      Organization,
                                                T                  NotificationType,
                                                EventTracking_Id?  EventTrackingId   = null,
                                                User_Id?           CurrentUserId     = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                await Organization.RemoveNotification(NotificationType,
                                                      async update => await WriteToDatabaseFile(removeNotification_MessageType,
                                                                                                update.ToJSON(false).AddFirstAndReturn(new JProperty("organizationId", Organization.Id.ToString())),
                                                                                                EventTrackingId,
                                                                                                CurrentUserId));

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region RemoveNotification(OrganizationId, NotificationType, ...)

        public async Task RemoveNotification<T>(Organization_Id   OrganizationId,
                                                T                 NotificationType,
                                                EventTracking_Id? EventTrackingId   = null,
                                                User_Id?          CurrentUserId     = null)

            where T : ANotification

        {

            try
            {

                await UsersSemaphore.WaitAsync();

                if (organizations.TryGetValue(OrganizationId, out var organization) && organization is not null)
                {

                    organization.RemoveNotification(NotificationType,
                                                    async update => await WriteToDatabaseFile(removeNotification_MessageType,
                                                                                              update.ToJSON(false).AddFirstAndReturn(new JProperty("organizationId", OrganizationId.ToString())),
                                                                                              EventTrackingId,
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

        #region Notification Groups

        #region Data

        /// <summary>
        /// An enumeration of all notification groups.
        /// </summary>
        protected readonly Dictionary<NotificationGroup_Id, NotificationGroup> _NotificationGroups = new ();

        /// <summary>
        /// An enumeration of all notification groups.
        /// </summary>
        public IEnumerable<NotificationGroup> NotificationGroups
        {
            get
            {

                if (UsersSemaphore.Wait(SemaphoreSlimTimeout))
                {
                    try
                    {

                        return _NotificationGroups.Values.ToArray();

                    }
                    finally
                    {
                        try
                        {
                            UsersSemaphore.Release();
                        }
                        catch
                        { }
                    }
                }

                return Array.Empty<NotificationGroup>();

            }
        }

        #endregion


        protected Boolean _AddNotificationGroup(NotificationGroup NotificationGroup)
        {
            _NotificationGroups.Add(NotificationGroup.Id, NotificationGroup);
            return true;
        }

        public async Task<Boolean> AddNotificationGroup(NotificationGroup NotificationGroup)
        {

            try
            {

                await UsersSemaphore.WaitAsync();

                return _AddNotificationGroup(NotificationGroup);

            }
            finally
            {
                UsersSemaphore.Release();
            }

        }

        #endregion

        #region Dashboards

        #region Data

        /// <summary>
        /// An enumeration of all dashboards.
        /// </summary>
        protected readonly Dictionary<Dashboard_Id, Dashboard> _Dashboards;

        /// <summary>
        /// An enumeration of all dashboards.
        /// </summary>
        public IEnumerable<Dashboard> Dashboards
        {
            get
            {
                try
                {
                    return DashboardsSemaphore.Wait(SemaphoreSlimTimeout)
                               ? _Dashboards.Values.ToArray()
                               : new Dashboard[0];
                }
                finally
                {
                    try
                    {
                        DashboardsSemaphore.Release();
                    }
                    catch
                    { }
                }
            }
        }

        #endregion


        #region AddDashboard           (Dashboard,                   CurrentUserId = null)

        /// <summary>
        /// Add the given dashboard to the API.
        /// </summary>
        /// <param name="Dashboard">A new dashboard to be added to this API.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<Dashboard> AddDashboard(Dashboard  Dashboard,
                                                  User_Id?   CurrentUserId  = null)
        {

            try
            {

                await DashboardsSemaphore.WaitAsync();

                if (Dashboard.API != null && Dashboard.API != this)
                    throw new ArgumentException(nameof(Dashboard), "The given dashboard is already attached to another API!");

                if (_Dashboards.ContainsKey(Dashboard.Id))
                    throw new Exception("Dashboard '" + Dashboard.Id + "' already exists in this API!");

                Dashboard.API = this;


                await WriteToDatabaseFile(addDashboard_MessageType,
                                          Dashboard.ToJSON(),
                                          EventTracking_Id.New,
                                          CurrentUserId);

                var newDashboard = _Dashboards.AddAndReturnValue(Dashboard.Id, Dashboard);

                return newDashboard;

            }
            finally
            {
                DashboardsSemaphore.Release();
            }

        }

        #endregion

        #region AddDashboardIfNotExists(Dashboard,                   CurrentUserId = null)

        /// <summary>
        /// When it has not been created before, add the given dashboard to the API.
        /// </summary>
        /// <param name="Dashboard">A new dashboard to be added to this API.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<Dashboard> AddDashboardIfNotExists(Dashboard  Dashboard,
                                                             User_Id?   CurrentUserId  = null)
        {

            try
            {

                await DashboardsSemaphore.WaitAsync();

                if (Dashboard.API != null && Dashboard.API != this)
                    throw new ArgumentException(nameof(Dashboard), "The given dashboard is already attached to another API!");

                if (_Dashboards.ContainsKey(Dashboard.Id))
                    return _Dashboards[Dashboard.Id];

                Dashboard.API = this;

                await WriteToDatabaseFile(addDashboardIfNotExists_MessageType,
                                          Dashboard.ToJSON(),
                                          EventTracking_Id.New,
                                          CurrentUserId);

                var newDashboard = _Dashboards.AddAndReturnValue(Dashboard.Id, Dashboard);

                return newDashboard;

            }
            finally
            {
                DashboardsSemaphore.Release();
            }

        }

        #endregion

        #region AddOrUpdateDashboard   (Dashboard,                   CurrentUserId = null)

        /// <summary>
        /// Add or update the given dashboard to/within the API.
        /// </summary>
        /// <param name="Dashboard">A dashboard.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<Dashboard> AddOrUpdateDashboard(Dashboard  Dashboard,
                                                          User_Id?   CurrentUserId  = null)
        {

            try
            {

                await DashboardsSemaphore.WaitAsync();

                if (Dashboard.API != null && Dashboard.API != this)
                    throw new ArgumentException(nameof(Dashboard), "The given dashboard is already attached to another API!");

                if (_Dashboards.TryGetValue(Dashboard.Id, out Dashboard OldDashboard))
                {
                    _Dashboards.Remove(OldDashboard.Id);
                }

                Dashboard.API = this;

                await WriteToDatabaseFile(addOrUpdateDashboard_MessageType,
                                          Dashboard.ToJSON(),
                                          EventTracking_Id.New,
                                          CurrentUserId);

                var newDashboard = _Dashboards.AddAndReturnValue(Dashboard.Id, Dashboard);

                // ToDo: Copy edges!

                return newDashboard;

            }
            finally
            {
                DashboardsSemaphore.Release();
            }

        }

        #endregion

        #region UpdateDashboard        (Dashboard,                   CurrentUserId = null)

        /// <summary>
        /// Update the given dashboard within the API.
        /// </summary>
        /// <param name="Dashboard">A dashboard.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<Dashboard> UpdateDashboard(Dashboard  Dashboard,
                                                     User_Id?   CurrentUserId  = null)
        {

            try
            {

                await DashboardsSemaphore.WaitAsync();

                if (Dashboard.API != null && Dashboard.API != this)
                    throw new ArgumentException(nameof(Dashboard), "The given dashboard is already attached to another API!");

                if (!_Dashboards.TryGetValue(Dashboard.Id, out Dashboard OldDashboard))
                    throw new Exception("Dashboard '" + Dashboard.Id + "' does not exists in this API!");

                else
                {

                    _Dashboards.Remove(OldDashboard.Id);

                }

                Dashboard.API = this;

                await WriteToDatabaseFile(updateDashboard_MessageType,
                                          Dashboard.ToJSON(),
                                          EventTracking_Id.New,
                                          CurrentUserId);

                Dashboard.CopyAllLinkedDataFrom(OldDashboard);

                return _Dashboards.AddAndReturnValue(Dashboard.Id, Dashboard);

            }
            finally
            {
                DashboardsSemaphore.Release();
            }

        }

        #endregion

        #region UpdateDashboard        (DashboardId, UpdateDelegate, ...)

        /// <summary>
        /// Update the given dashboard.
        /// </summary>
        /// <param name="DashboardId">An dashboard identification.</param>
        /// <param name="UpdateDelegate">A delegate to update the given dashboard.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<Dashboard> UpdateDashboard(Dashboard_Id               DashboardId,
                                                     Action<Dashboard.Builder>  UpdateDelegate,
                                                     User_Id?                   CurrentUserId  = null)
        {

            try
            {

                if (UpdateDelegate == null)
                    throw new Exception("The given update delegate must not be null!");

                await DashboardsSemaphore.WaitAsync();

                if (!_Dashboards.TryGetValue(DashboardId, out Dashboard OldDashboard))
                    throw new Exception("Dashboard '" + DashboardId + "' does not exists in this API!");

                var Builder = OldDashboard.ToBuilder();
                UpdateDelegate(Builder);
                var NewDashboard = Builder.ToImmutable;

                await WriteToDatabaseFile(updateDashboard_MessageType,
                                          NewDashboard.ToJSON(),
                                          EventTracking_Id.New,
                                          CurrentUserId);

                NewDashboard.API = this;

                _Dashboards.Remove(OldDashboard.Id);
                NewDashboard.CopyAllLinkedDataFrom(OldDashboard);

                return _Dashboards.AddAndReturnValue(NewDashboard.Id, NewDashboard);

            }
            finally
            {
                DashboardsSemaphore.Release();
            }

        }

        #endregion


        #region (protected internal) SetDashboardRequest (Request)

        /// <summary>
        /// An event sent whenever set dashboard (data) request was received.
        /// </summary>
        public event RequestLogHandler OnSetDashboardRequest;

        protected internal HTTPRequest SetDashboardRequest(HTTPRequest Request)
        {

            OnSetDashboardRequest?.Invoke(Request.Timestamp,
                                          HTTPServer,
                                          Request);

            return Request;

        }

        #endregion

        #region (protected internal) SetDashboardResponse(Response)

        /// <summary>
        /// An event sent whenever a response on a set dashboard (data) request was sent.
        /// </summary>
        public event AccessLogHandler OnSetDashboardResponse;

        protected internal HTTPResponse SetDashboardResponse(HTTPResponse Response)
        {

            OnSetDashboardResponse?.Invoke(Response.Timestamp,
                                           HTTPServer,
                                           Response.HTTPRequest,
                                           Response);

            return Response;

        }

        #endregion


        #region DashboardExists        (DashboardId)

        /// <summary>
        /// Whether this API contains a dashboard having the given unique identification.
        /// </summary>
        /// <param name="DashboardId">The unique identification of the dashboard.</param>
        public Boolean DashboardExists(Dashboard_Id DashboardId)
        {

            try
            {

                DashboardsSemaphore.Wait();

                return _Dashboards.ContainsKey(DashboardId);

            }
            finally
            {
                DashboardsSemaphore.Release();
            }

        }

        #endregion

        #region GetDashboard           (DashboardId)

        /// <summary>
        /// Get the dashboard having the given unique identification.
        /// </summary>
        /// <param name="DashboardId">The unique identification of the dashboard.</param>
        public async Task<Dashboard> GetDashboard(Dashboard_Id  DashboardId)
        {

            try
            {

                await DashboardsSemaphore.WaitAsync();

                if (_Dashboards.TryGetValue(DashboardId, out Dashboard Dashboard))
                    return Dashboard;

                return null;

            }
            finally
            {
                DashboardsSemaphore.Release();
            }

        }

        #endregion

        #region TryGetDashboard        (DashboardId, out Dashboard)

        /// <summary>
        /// Try to get the dashboard having the given unique identification.
        /// </summary>
        /// <param name="DashboardId">The unique identification of the dashboard.</param>
        /// <param name="Dashboard">The dashboard.</param>
        public Boolean TryGetDashboard(Dashboard_Id   DashboardId,
                                       out Dashboard  Dashboard)
        {

            try
            {

                DashboardsSemaphore.Wait();

                return _Dashboards.TryGetValue(DashboardId, out Dashboard);

            }
            finally
            {
                DashboardsSemaphore.Release();
            }

        }

        #endregion


        #region RemoveDashboard        (DashboardId,                 CurrentUserId = null)

        /// <summary>
        /// Remove the given dashboard from this API.
        /// </summary>
        /// <param name="DashboardId">The unique identification of the dashboard.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<Dashboard> RemoveDashboard(Dashboard_Id  DashboardId,
                                                     User_Id?      CurrentUserId  = null)
        {

            try
            {

                await DashboardsSemaphore.WaitAsync();

                if (_Dashboards.TryGetValue(DashboardId, out Dashboard Dashboard))
                {

                    await WriteToDatabaseFile(removeDashboard_MessageType,
                                              Dashboard.ToJSON(),
                                              EventTracking_Id.New,
                                              CurrentUserId);

                    _Dashboards.Remove(DashboardId);

                    //Dashboard.API = null;

                    return Dashboard;

                }

                return null;

            }
            finally
            {
                DashboardsSemaphore.Release();
            }

        }

        #endregion

        #endregion


        #region ServiceTickets

        #region Data

        protected internal readonly ConcurrentDictionary<ServiceTicket_Id, ServiceTicket> _ServiceTickets;

        /// <summary>
        /// Return an enumeration of all service tickets.
        /// </summary>
        public IEnumerable<ServiceTicket> ServiceTickets
        {
            get
            {
                try
                {
                    ServiceTicketsSemaphore.Wait();
                    return _ServiceTickets.Values.ToArray();
                }
                finally
                {
                    ServiceTicketsSemaphore.Release();
                }

            }
        }

        #endregion


        #region (protected internal) WriteToDatabaseFileAndNotify(ServiceTicket, MessageType, OldServiceTicket = null, ...)

        protected internal async Task WriteToDatabaseFileAndNotify(ServiceTicket            ServiceTicket,
                                                                   NotificationMessageType  MessageType,
                                                                   ServiceTicket            OldServiceTicket   = null,
                                                                   EventTracking_Id?        EventTrackingId    = null,
                                                                   User_Id?                 CurrentUserId      = null)
        {

            if (ServiceTicket is null)
                throw new ArgumentNullException(nameof(ServiceTicket),  "The given service ticket must not be null!");

            if (MessageType.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(MessageType),    "The given message type must not be null or empty!");


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(MessageType,
                                      ServiceTicket.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            await SendNotifications(ServiceTicket,
                                    MessageType,
                                    OldServiceTicket,
                                    eventTrackingId,
                                    CurrentUserId);

        }

        #endregion

        #region (protected internal) SendNotifications           (ServiceTicket, MessageTypes, OldServiceTicket = null, ...)

        /// <summary>
        /// Send service ticket notifications.
        /// </summary>
        /// <param name="ServiceTicket">The service ticket.</param>
        /// <param name="MessageType">The service ticket notification.</param>
        /// <param name="OldServiceTicket">The old/updated service ticket.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">The invoking user identification.</param>
        protected internal virtual Task SendNotifications(ServiceTicket            ServiceTicket,
                                                          NotificationMessageType  MessageType,
                                                          ServiceTicket            OldServiceTicket   = null,
                                                          EventTracking_Id?        EventTrackingId    = null,
                                                          User_Id?                 CurrentUserId      = null)

            => SendNotifications(ServiceTicket,
                                 new NotificationMessageType[] { MessageType },
                                 OldServiceTicket,
                                 EventTrackingId,
                                 CurrentUserId);


        /// <summary>
        /// Send service ticket notifications.
        /// </summary>
        /// <param name="ServiceTicket">The service ticket.</param>
        /// <param name="MessageTypes">The service ticket notifications.</param>
        /// <param name="OldServiceTicket">The old/updated service ticket.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">The invoking user identification.</param>
        protected internal async virtual Task SendNotifications(ServiceTicket                         ServiceTicket,
                                                                IEnumerable<NotificationMessageType>  MessageTypes,
                                                                ServiceTicket                         OldServiceTicket   = null,
                                                                EventTracking_Id?                     EventTrackingId    = null,
                                                                User_Id?                              CurrentUserId      = null)
        {

            if (ServiceTicket is null)
                throw new ArgumentNullException(nameof(ServiceTicket),  "The given service ticket must not be null or empty!");

            var messageTypesHash = new HashSet<NotificationMessageType>(MessageTypes.Where(messageType => !messageType.IsNullOrEmpty));

            if (messageTypesHash.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(MessageTypes),   "The given enumeration of message types must not be null or empty!");

            if (messageTypesHash.Contains(addServiceTicketIfNotExists_MessageType))
                messageTypesHash.Add(addServiceTicket_MessageType);

            if (messageTypesHash.Contains(addOrUpdateServiceTicket_MessageType))
                messageTypesHash.Add(OldServiceTicket == null
                                       ? addServiceTicket_MessageType
                                       : updateServiceTicket_MessageType);

            var messageTypes = messageTypesHash.ToArray();


            if (!DisableNotifications)
            {

                #region Telegram Notifications

                if (TelegramClient != null)
                {
                    try
                    {

                        var AllTelegramNotifications = ServiceTicket.Author.
                                                                     GetNotificationsOf<EMailNotification>(messageTypes).
                                                                     ToSafeHashSet();

                        if (ServiceTicket.Affected != null)
                        {

                            if (ServiceTicket.Affected.Users.SafeAny())
                            {
                                foreach (var user in ServiceTicket.Affected.Users)
                                    foreach (var notification in user.Message.GetNotificationsOf<EMailNotification>(messageTypes))
                                        AllTelegramNotifications.Add(notification);
                            }

                            if (ServiceTicket.Affected.Organizations.SafeAny())
                            {
                                foreach (var organization in ServiceTicket.Affected.Organizations)
                                    foreach (var notification in organization.Message.GetNotificationsOf<EMailNotification>(messageTypes))
                                        AllTelegramNotifications.Add(notification);
                            }

                        }

                        if (AllTelegramNotifications.SafeAny())
                        {

                            //await TelegramStore.SendTelegrams("ServiceTicket '" + ServiceTicket.Id + "' sent '" + MessageType + "'!",
                            //                                  AllTelegramNotifications.Select(telegramNotification => telegramNotification.Username));

                        }

                    }
                    catch (Exception e)
                    {
                        DebugX.LogException(e);
                    }
                }

                #endregion

                #region SMS Notifications

                try
                {

                    var AllSMSNotifications = ServiceTicket.Author.
                                                            GetNotificationsOf<SMSNotification>(messageTypes).
                                                            ToSafeHashSet();

                    if (AllSMSNotifications.SafeAny())
                    {

                        //SendSMS("ServiceTicket '" + ServiceTicket.Id + "' sent '" + MessageType + "'!",
                        //        AllSMSNotifications.Select(smsPhoneNumber => smsPhoneNumber.PhoneNumber.ToString()).ToArray(),
                        //        SMSSenderName);

                    }

                }
                catch (Exception e)
                {
                    DebugX.LogException(e);
                }

                #endregion

                #region HTTPS Notifications

                try
                {

                    var AllHTTPSNotifications = ServiceTicket.Author.
                                                              GetNotificationsOf<HTTPSNotification>(messageTypes).
                                                              ToSafeHashSet();

                    if (AllHTTPSNotifications.SafeAny())
                    {

                        #region Create JSON...

                        JObject JSONNotification = null;

                        if (messageTypes.Contains(addServiceTicket_MessageType))
                            JSONNotification = new JObject(
                                                   new JProperty("addServiceTicket",
                                                       ServiceTicket.ToJSON()
                                                   ),
                                                   new JProperty("timestamp", Timestamp.Now.ToIso8601())
                                               );

                        else if (messageTypes.Contains(addServiceTicketIfNotExists_MessageType))
                            JSONNotification = new JObject(
                                                   new JProperty("addServiceTicketIfNotExists",
                                                       ServiceTicket.ToJSON()
                                                   ),
                                                   new JProperty("timestamp", Timestamp.Now.ToIso8601())
                                               );

                        else if (messageTypes.Contains(addOrUpdateServiceTicket_MessageType))
                            JSONNotification = new JObject(
                                                   new JProperty("addOrUpdateServiceTicket",
                                                       ServiceTicket.ToJSON()
                                                   ),
                                                   new JProperty("timestamp", Timestamp.Now.ToIso8601())
                                               );

                        else if (messageTypes.Contains(updateServiceTicket_MessageType))
                            JSONNotification = new JObject(
                                                   new JProperty("updateServiceTicket",
                                                       ServiceTicket.ToJSON()
                                                   ),
                                                   new JProperty("timestamp", Timestamp.Now.ToIso8601())
                                               );

                        else if (messageTypes.Contains(removeServiceTicket_MessageType))
                            JSONNotification = new JObject(
                                                   new JProperty("removeServiceTicket",
                                                       ServiceTicket.ToJSON()
                                                   ),
                                                   new JProperty("timestamp", Timestamp.Now.ToIso8601())
                                               );

                        else if (messageTypes.Contains(changeServiceTicketStatus_MessageType))
                            JSONNotification = new JObject(
                                                   new JProperty("changeServiceTicketStatus",
                                                       ServiceTicket.ToJSON()
                                                   ),
                                                   new JProperty("timestamp", Timestamp.Now.ToIso8601())
                                               );

                        #endregion

                        await SendHTTPSNotifications(AllHTTPSNotifications,
                                                     JSONNotification);

                    }

                }
                catch (Exception e)
                {
                    DebugX.LogException(e);
                }

                #endregion

                #region EMailNotifications

                if (SMTPClient != null)
                {
                    try
                    {

                        var AllEMailNotifications = new HashSet<EMailNotification>();

                        // Add author
                        ServiceTicket.Author.GetNotificationsOf<EMailNotification>(messageTypes).
                             ForEach(notificationemail => AllEMailNotifications.Add(notificationemail));

                        // Add device owners

                        if (AllEMailNotifications.SafeAny())
                        {

                            //await APISMTPClient.Send(__ServiceTicketChangedEMailDelegate(ExternalDNSName, Robot.EMail, APIPassphrase)
                            //                         (ServiceTicket,
                            //                          MessageType,
                            //                          MessageTypes,
                            //                          EMailAddressList.Create(AllEMailNotifications.Select(emailnotification => emailnotification.EMailAddress))
                            //                         ));

                        }

                    } catch (Exception e)
                    {
                        DebugX.LogException(e);
                    }
                }

                #endregion

            }

        }

        #endregion


        #region AddServiceTicket           (ServiceTicket,   CurrentUserId = null)

        /// <summary>
        /// Add the given service ticket to the API.
        /// </summary>
        /// <param name="ServiceTicket">A service ticket.</param>
        /// <param name="AfterAddition">A delegate to call after the service ticket was added to the API.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<TServiceTicket> AddServiceTicket<TServiceTicket>(TServiceTicket          ServiceTicket,
                                                                           Action<TServiceTicket>  AfterAddition     = null,
                                                                           EventTracking_Id?       EventTrackingId   = null,
                                                                           User_Id?                CurrentUserId     = null)

            where TServiceTicket : ServiceTicket

        {

            if (ServiceTicket is null)
                throw new ArgumentNullException(nameof(ServiceTicket), "The given service ticket must not be null!");

            if (ServiceTicket.API != null && ServiceTicket.API != this)
                throw new ArgumentException("The given service ticket is already attached to another API!", nameof(ServiceTicket));

            try
            {

                await ServiceTicketsSemaphore.WaitAsync();

                if (_ServiceTickets.ContainsKey(ServiceTicket.Id))
                    throw new Exception("ServiceTicket '" + ServiceTicket.Id + "' already exists in this API!");

                await WriteToDatabaseFileAndNotify(ServiceTicket,
                                                   addServiceTicket_MessageType,
                                                   CurrentUserId: CurrentUserId);

                ServiceTicket.API = this;

                _ServiceTickets.AddOrUpdate(ServiceTicket.Id,
                                            id                     => ServiceTicket,
                                            (id, oldServiceTicket) => ServiceTicket);

                AfterAddition?.Invoke(ServiceTicket);

                return ServiceTicket;

            }
            finally
            {
                ServiceTicketsSemaphore.Release();
            }

        }

        #endregion

        #region AddServiceTicketIfNotExists(ServiceTicket,   CurrentUserId = null)

        /// <summary>
        /// Add the given service ticket to the API.
        /// </summary>
        /// <param name="ServiceTicket">A service ticket.</param>
        /// <param name="WhenNotExisted">A delegate to call when the service ticket did not exist before.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<TServiceTicket> AddServiceTicketIfNotExists<TServiceTicket>(TServiceTicket          ServiceTicket,
                                                                                      Action<TServiceTicket>  WhenNotExisted  = null,
                                                                                      User_Id?                CurrentUserId   = null)

            where TServiceTicket : ServiceTicket

        {

            if (ServiceTicket is null)
                throw new ArgumentNullException(nameof(ServiceTicket), "The given service ticket must not be null!");

            if (ServiceTicket.API != null && ServiceTicket.API != this)
                throw new ArgumentException(nameof(ServiceTicket), "The given service ticket is already attached to another API!");

            try
            {

                await ServiceTicketsSemaphore.WaitAsync();

                if (_ServiceTickets.TryGetValue(ServiceTicket.Id, out ServiceTicket OldServiceTicket))
                    return OldServiceTicket as TServiceTicket;

                await WriteToDatabaseFileAndNotify(ServiceTicket,
                                                   addServiceTicketIfNotExists_MessageType,
                                                   CurrentUserId: CurrentUserId);

                ServiceTicket.API = this;

                _ServiceTickets.AddOrUpdate(ServiceTicket.Id,
                                            id                     => ServiceTicket,
                                            (id, oldServiceTicket) => ServiceTicket);

                WhenNotExisted?.Invoke(ServiceTicket);

                return ServiceTicket;

            }
            finally
            {
                ServiceTicketsSemaphore.Release();
            }

        }

        #endregion

        #region AddOrUpdateServiceTicket   (ServiceTicket, DisableAnalyzeServiceTicketStatus = false,   CurrentUserId = null)

        /// <summary>
        /// Add or update the given service ticket to/within the API.
        /// </summary>
        /// <param name="ServiceTicket">A service ticket.</param>
        /// <param name="AfterAddOrUpdate">A delegate to call after the service ticket was added to or updated within the API.</param>
        /// <param name="DoNotAnalyzeTheServiceTicketStatus">Do not analyze the service ticket status.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<TServiceTicket> AddOrUpdateServiceTicket<TServiceTicket>(TServiceTicket                          ServiceTicket,
                                                                                   Action<TServiceTicket, TServiceTicket>  AfterAddOrUpdate                     = null,
                                                                                   Boolean                                 DoNotAnalyzeTheServiceTicketStatus   = false,
                                                                                   User_Id?                                CurrentUserId                        = null)

            where TServiceTicket : ServiceTicket

        {

            if (ServiceTicket is null)
                throw new ArgumentNullException(nameof(ServiceTicket), "The given service ticket must not be null!");

            if (ServiceTicket.API != null && ServiceTicket.API != this)
                throw new ArgumentException(nameof(ServiceTicket), "The given service ticket is already attached to another API!");

            ServiceTicket  OldServiceTicket                = null;
            DateTime       Now                             = Timestamp.Now;
            Boolean        FastAnalyzeServiceTicketStatus  = false;

            try
            {

                await ServiceTicketsSemaphore.WaitAsync();

                ServiceTicket.API = this;

                if (_ServiceTickets.TryRemove(ServiceTicket.Id, out OldServiceTicket))
                {

                    ServiceTicket.CopyAllLinkedDataFrom(OldServiceTicket);

                    //// Only run when the admin status changed!
                    //if (!DoNotAnalyzeTheServiceTicketStatus &&
                    //    OldServiceTicket.AdminStatus.Value != ServiceTicketAdminStatusTypes.Monitored &&
                    //    OldServiceTicket.AdminStatus.Value != ServiceTicketAdminStatusTypes.Tracked   &&
                    //   (ServiceTicket.   AdminStatus.Value == ServiceTicketAdminStatusTypes.Monitored ||
                    //    ServiceTicket.   AdminStatus.Value == ServiceTicketAdminStatusTypes.Tracked))
                    //{
                    //    FastAnalyzeServiceTicketStatus = true;
                    //}

                }

                await WriteToDatabaseFileAndNotify(ServiceTicket,
                                                   addOrUpdateServiceTicket_MessageType,
                                                   OldServiceTicket,
                                                   EventTracking_Id.New,
                                                   CurrentUserId);

                _ServiceTickets.AddOrUpdate(ServiceTicket.Id,
                                            id                     => ServiceTicket,
                                            (id, oldServiceTicket) => ServiceTicket);

                AfterAddOrUpdate?.Invoke(OldServiceTicket as TServiceTicket,
                                         ServiceTicket);

            }
            finally
            {
                ServiceTicketsSemaphore.Release();
            }


            #region Analyze service ticket status...

            //// AnalyzeServiceTicketStatus(ServiceTicket) might enter this method again!
            //if (FastAnalyzeServiceTicketStatus)
            //    await AnalyzeServiceTicketStatus(ServiceTicket);

            #endregion

            #region Call OnServiceTicket(Admin)StatusChanged events...

            if (OldServiceTicket != null)
            {

                if (OldServiceTicket.Status != ServiceTicket.Status)
                    OnServiceTicketStatusChanged?.Invoke(Now,
                                                         ServiceTicket.Id,
                                                         OldServiceTicket.Status,
                                                         ServiceTicket.Status);

            }

            #endregion

            return ServiceTicket;

        }

        #endregion

        #region UpdateServiceTicket        (ServiceTicket, DisableAnalyzeServiceTicketStatus = false,   CurrentUserId = null)

        /// <summary>
        /// Update the given service ticket within the API.
        /// </summary>
        /// <param name="ServiceTicket">A service ticket.</param>
        /// <param name="AfterUpdate">A delegate to call after the service ticket was updated within the API.</param>
        /// <param name="DoNotAnalyzeTheServiceTicketStatus">Do not analyze the service ticket status.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<TServiceTicket> UpdateServiceTicket<TServiceTicket>(TServiceTicket          ServiceTicket,
                                                                              Action<TServiceTicket>  AfterUpdate                          = null,
                                                                              Boolean                 DoNotAnalyzeTheServiceTicketStatus   = false,
                                                                              User_Id?                CurrentUserId                        = null)

            where TServiceTicket : ServiceTicket

        {

            if (ServiceTicket is null)
                throw new ArgumentNullException(nameof(ServiceTicket), "The given service ticket must not be null!");

            if (ServiceTicket.API != null && ServiceTicket.API != this)
                throw new ArgumentException(nameof(ServiceTicket), "The given service ticket is already attached to another API!");

            ServiceTicket OldServiceTicket;
            DateTime       Now = Timestamp.Now;

            try
            {

                await ServiceTicketsSemaphore.WaitAsync();

                if (!_ServiceTickets.TryGetValue(ServiceTicket.Id, out OldServiceTicket))
                    throw new Exception("ServiceTicket '" + ServiceTicket.Id + "' does not exists in this API!");

                await WriteToDatabaseFileAndNotify(ServiceTicket,
                                                   updateServiceTicket_MessageType,
                                                   OldServiceTicket,
                                                   EventTracking_Id.New,
                                                   CurrentUserId);

                //if (ServiceTicket.API == null)
                    ServiceTicket.API = this;

                _ServiceTickets.TryRemove(OldServiceTicket.Id, out ServiceTicket removedServiceTicket);
                ServiceTicket.CopyAllLinkedDataFrom(OldServiceTicket);

                _ServiceTickets.AddOrUpdate(ServiceTicket.Id,
                                            id                     => ServiceTicket,
                                            (id, oldServiceTicket) => ServiceTicket);

                AfterUpdate?.Invoke(ServiceTicket);

            }
            finally
            {
                ServiceTicketsSemaphore.Release();
            }


            #region Analyze service ticket status...

            //// Only run when the admin status changed!
            //// AnalyzeServiceTicketStatus(ServiceTicket) might enter this method again!
            //if (!DoNotAnalyzeTheServiceTicketStatus &&
            //    OldServiceTicket.AdminStatus.Value != ServiceTicketAdminStatusTypes.Monitored &&
            //    OldServiceTicket.AdminStatus.Value != ServiceTicketAdminStatusTypes.Tracked   &&
            //   (ServiceTicket.   AdminStatus.Value == ServiceTicketAdminStatusTypes.Monitored ||
            //    ServiceTicket.   AdminStatus.Value == ServiceTicketAdminStatusTypes.Tracked))
            //{
            //    await AnalyzeServiceTicketStatus(ServiceTicket);
            //}

            #endregion

            #region Call OnServiceTicket(Admin)StatusChanged events...

            if (OldServiceTicket.Status != ServiceTicket.Status)
                OnServiceTicketStatusChanged?.Invoke(Now,
                                                     ServiceTicket.Id,
                                                     OldServiceTicket.Status,
                                                     ServiceTicket.Status);

            #endregion


            return ServiceTicket;

        }

        #endregion

        #region UpdateServiceTicket        (ServiceTicketId, UpdateDelegate, DoNotAnalyzeTheServiceTicketStatus = false, ...)

        /// <summary>
        /// Update the given service ticket.
        /// </summary>
        /// <param name="ServiceTicketId">A service ticket identification.</param>
        /// <param name="UpdateDelegate">A delegate to update the given service ticket.</param>
        /// <param name="AfterUpdate">A delegate to call after the service ticket was updated within the API.</param>
        /// <param name="DoNotAnalyzeTheServiceTicketStatus">Do not analyze the service ticket status.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<TServiceTicket> UpdateServiceTicket<TServiceTicket>(ServiceTicket_Id                      ServiceTicketId,
                                                                              Func<TServiceTicket, TServiceTicket>  UpdateDelegate,
                                                                              Action<TServiceTicket>                AfterUpdate                          = null,
                                                                              Boolean                               DoNotAnalyzeTheServiceTicketStatus   = false,
                                                                              User_Id?                              CurrentUserId                        = null)
            where TServiceTicket : ServiceTicket
        {

            TServiceTicket  castedOldServiceTicket;
            TServiceTicket  ServiceTicket;
            DateTime        Now = Timestamp.Now;

            try
            {

                if (UpdateDelegate == null)
                    throw new Exception("The given update delegate must not be null!");

                await ServiceTicketsSemaphore.WaitAsync();

                if (!_ServiceTickets.TryGetValue(ServiceTicketId, out ServiceTicket OldServiceTicket))
                    throw new Exception("ServiceTicket '" + ServiceTicketId + "' does not exists in this API!");

                castedOldServiceTicket = OldServiceTicket as TServiceTicket;

                if (castedOldServiceTicket == null)
                    throw new Exception("ServiceTicket '" + ServiceTicketId + "' is not of type TServiceTicket!");

                ServiceTicket = UpdateDelegate(castedOldServiceTicket);
                ServiceTicket.API = this;

                await WriteToDatabaseFileAndNotify(ServiceTicket,
                                                   updateServiceTicket_MessageType,
                                                   castedOldServiceTicket,
                                                   EventTracking_Id.New,
                                                   CurrentUserId);

                _ServiceTickets.TryRemove(OldServiceTicket.Id, out ServiceTicket RemovedServiceTicket);
                //OldServiceTicket.CopyAllEdgesTo(ServiceTicket);
                _ServiceTickets.AddOrUpdate(ServiceTicket.Id,
                                            id                     => ServiceTicket,
                                            (id, oldServiceTicket) => ServiceTicket);

                AfterUpdate?.Invoke(ServiceTicket);

            }
            finally
            {
                ServiceTicketsSemaphore.Release();
            }


            #region Analyze service ticket status...

            //// Only run when the admin status changed!
            //// AnalyzeServiceTicketStatus(ServiceTicket) might enter this method again!
            //if (!DoNotAnalyzeTheServiceTicketStatus &&
            //    OldServiceTicket.AdminStatus.Value != ServiceTicketAdminStatusTypes.Monitored &&
            //    OldServiceTicket.AdminStatus.Value != ServiceTicketAdminStatusTypes.Tracked   &&
            //   (ServiceTicket.   AdminStatus.Value == ServiceTicketAdminStatusTypes.Monitored ||
            //    ServiceTicket.   AdminStatus.Value == ServiceTicketAdminStatusTypes.Tracked))
            //{
            //    await AnalyzeServiceTicketStatus(ServiceTicket);
            //}

            #endregion

            #region Call OnServiceTicket(Admin)StatusChanged events...

            if (castedOldServiceTicket.Status != ServiceTicket.Status)
                OnServiceTicketStatusChanged?.Invoke(Now,
                                                     ServiceTicket.Id,
                                                     castedOldServiceTicket.Status,
                                                     ServiceTicket.Status);

            #endregion


            return ServiceTicket;

        }

        #endregion

        #region RemoveServiceTicket        (ServiceTicketId, ...)

        /// <summary>
        /// Remove the given service ticket from this API.
        /// </summary>
        /// <param name="ServiceTicketId">The unique identification of the service ticket.</param>
        /// <param name="AfterRemoval">A delegate to call after the service ticket was removed from the API.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<ServiceTicket> RemoveServiceTicket(ServiceTicket_Id        ServiceTicketId,
                                                              Action<ServiceTicket>  AfterRemoval    = null,
                                                              User_Id?                CurrentUserId   = null)
        {

            try
            {

                await ServiceTicketsSemaphore.WaitAsync();

                if (_ServiceTickets.TryGetValue(ServiceTicketId, out ServiceTicket ServiceTicket))
                {

                    await WriteToDatabaseFileAndNotify(ServiceTicket,
                                                       removeServiceTicket_MessageType,
                                                       CurrentUserId: CurrentUserId);

                    _ServiceTickets.TryRemove(ServiceTicketId, out ServiceTicket RemovedServiceTicket);

                    ServiceTicket.API = null;

                    AfterRemoval?.Invoke(ServiceTicket);

                    return ServiceTicket;

                }

                return null;

            }
            finally
            {
                ServiceTicketsSemaphore.Release();
            }

        }

        #endregion


        #region ServiceTicketExists        (ServiceTicketId)

        /// <summary>
        /// Whether this API contains a service ticket having the given unique identification.
        /// </summary>
        /// <param name="ServiceTicketId">The unique identification of the service ticket.</param>
        public Boolean ServiceTicketExists(ServiceTicket_Id ServiceTicketId)
        {

            try
            {

                ServiceTicketsSemaphore.Wait();

                return _ServiceTickets.ContainsKey(ServiceTicketId);

            }
            finally
            {
                ServiceTicketsSemaphore.Release();
            }

        }

        #endregion

        #region GetServiceTicket           (ServiceTicketId)

        /// <summary>
        /// Get the service ticket having the given unique identification.
        /// </summary>
        /// <param name="ServiceTicketId">The unique identification of the service ticket.</param>
        public async Task<ServiceTicket> GetServiceTicket(ServiceTicket_Id  ServiceTicketId)
        {

            try
            {

                await ServiceTicketsSemaphore.WaitAsync();

                if (_ServiceTickets.TryGetValue(ServiceTicketId, out ServiceTicket serviceTicket))
                    return serviceTicket;

                return null;

            }
            finally
            {
                ServiceTicketsSemaphore.Release();
            }

        }

        #endregion

        #region TryGetServiceTicket        (ServiceTicketId, out ServiceTicket)

        /// <summary>
        /// Try to get the service ticket having the given unique identification.
        /// </summary>
        /// <param name="ServiceTicketId">The unique identification of the service ticket.</param>
        /// <param name="ServiceTicket">The service ticket.</param>
        public Boolean TryGetServiceTicket<TServiceTicket>(ServiceTicket_Id    ServiceTicketId,
                                                           out TServiceTicket  ServiceTicket)

            where TServiceTicket : ServiceTicket

        {

            try
            {

                ServiceTicketsSemaphore.Wait();

                if (_ServiceTickets.TryGetValue(ServiceTicketId, out ServiceTicket serviceTicket))
                {
                    ServiceTicket = serviceTicket as TServiceTicket;
                    return ServiceTicket != null;
                }

                ServiceTicket = null;
                return false;

            }
            finally
            {
                ServiceTicketsSemaphore.Release();
            }

        }

        #endregion


        /// <summary>
        /// A delegate used whenever a service ticket status changed.
        /// </summary>
        /// <param name="Timestamp">The timestamp of the event.</param>
        /// <param name="ServiceTicketId">The unique service ticket identification.</param>
        /// <param name="OldStatus">The old status.</param>
        /// <param name="NewStatus">The new status.</param>
        public delegate Task ServiceTicketStatusChangedDelegate     (DateTime                               Timestamp,
                                                                     ServiceTicket_Id                       ServiceTicketId,
                                                                     Timestamped<ServiceTicketStatusTypes>  OldStatus,
                                                                     Timestamped<ServiceTicketStatusTypes>  NewStatus);

        /// <summary>
        /// An event sent whenever a service ticket status changed.
        /// </summary>
        public event ServiceTicketStatusChangedDelegate       OnServiceTicketStatusChanged;

        #endregion

        #region BlogPostings

        #region Data

        /// <summary>
        /// An enumeration of all blog postings.
        /// </summary>
        protected internal readonly Dictionary<BlogPosting_Id, BlogPosting> _BlogPostings;

        /// <summary>
        /// An enumeration of all blog postings.
        /// </summary>
        public IEnumerable<BlogPosting> BlogPostings
        {
            get
            {
                try
                {
                    return BlogPostingsSemaphore.Wait(SemaphoreSlimTimeout)
                               ? _BlogPostings.Values.ToArray()
                               : new BlogPosting[0];
                }
                finally
                {
                    try
                    {
                        BlogPostingsSemaphore.Release();
                    }
                    catch
                    { }
                }
            }
        }

        #endregion


        #region (protected internal) WriteToDatabaseFileAndNotify(BlogPosting, MessageType,  OldBlogPosting = null, ...)

        /// <summary>
        /// Write the given blog posting to the database and send out notifications.
        /// </summary>
        /// <param name="BlogPosting">The blog posting.</param>
        /// <param name="MessageType">The user notification.</param>
        /// <param name="OldBlogPosting">The old/updated blog posting.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task WriteToDatabaseFileAndNotify(BlogPosting              BlogPosting,
                                                          NotificationMessageType  MessageType,
                                                          BlogPosting              OldBlogPosting    = null,
                                                          EventTracking_Id?        EventTrackingId   = null,
                                                          User_Id?                 CurrentUserId     = null)
        {

            if (BlogPosting is null)
                throw new ArgumentNullException(nameof(BlogPosting),  "The given blog posting must not be null or empty!");

            if (MessageType.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(MessageType),  "The given message type must not be null or empty!");


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(MessageType,
                                      BlogPosting.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            await SendNotifications(BlogPosting,
                                    MessageType,
                                    OldBlogPosting,
                                    eventTrackingId,
                                    CurrentUserId);

        }

        #endregion

        #region (protected internal) SendNotifications           (BlogPosting, MessageTypes, OldBlogPosting = null, ...)

        /// <summary>
        /// Send blog posting notifications.
        /// </summary>
        /// <param name="BlogPosting">The blog posting.</param>
        /// <param name="MessageType">The user notification.</param>
        /// <param name="OldBlogPosting">The old/updated blog posting.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task SendNotifications(BlogPosting              BlogPosting,
                                               NotificationMessageType  MessageType,
                                               BlogPosting              OldBlogPosting    = null,
                                               EventTracking_Id?        EventTrackingId   = null,
                                               User_Id?                 CurrentUserId     = null)
        {

            if (BlogPosting is null)
                throw new ArgumentNullException(nameof(BlogPosting),  "The given blog posting must not be null or empty!");

            if (MessageType.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(MessageType),  "The given message type must not be null or empty!");


            await SendNotifications(BlogPosting,
                                    new NotificationMessageType[] { MessageType },
                                    OldBlogPosting,
                                    EventTrackingId,
                                    CurrentUserId);

        }


        /// <summary>
        /// Send blog posting notifications.
        /// </summary>
        /// <param name="BlogPosting">The blog posting.</param>
        /// <param name="MessageTypes">The user notifications.</param>
        /// <param name="OldBlogPosting">The old/updated blog posting.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task SendNotifications(BlogPosting                           BlogPosting,
                                               IEnumerable<NotificationMessageType>  MessageTypes,
                                               BlogPosting                           OldBlogPosting    = null,
                                               EventTracking_Id?                     EventTrackingId   = null,
                                               User_Id?                              CurrentUserId     = null)
        {

            if (BlogPosting is null)
                throw new ArgumentNullException(nameof(BlogPosting),   "The given blog posting must not be null or empty!");

            var messageTypesHash = new HashSet<NotificationMessageType>(MessageTypes.Where(messageType => !messageType.IsNullOrEmpty));

            if (messageTypesHash.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(MessageTypes),  "The given enumeration of message types must not be null or empty!");

            if (messageTypesHash.Contains(addUserIfNotExists_MessageType))
                messageTypesHash.Add(addUser_MessageType);

            if (messageTypesHash.Contains(addOrUpdateUser_MessageType))
                messageTypesHash.Add(OldBlogPosting == null
                                       ? addUser_MessageType
                                       : updateUser_MessageType);

            var messageTypes = messageTypesHash.ToArray();


            if (!DisableNotifications)
            {


            }

        }

        #endregion

        #region (protected internal) GetBlogPostingSerializator(Request, User)

        protected internal BlogPostingToJSONDelegate GetBlogPostingSerializator(HTTPRequest  Request,
                                                                                IUser        User)
        {

            switch (User?.Id.ToString())
            {

                default:
                    return (blogPosting,
                            embedded,
                            ExpandTags,
                            ExpandAuthorId)

                            => blogPosting.ToJSON(embedded,
                                                  ExpandTags,
                                                  ExpandAuthorId);

            }

        }

        #endregion


        #region AddBlogPosting           (BlogPosting, OnAdded = null,                   CurrentUserId = null)

        /// <summary>
        /// A delegate called whenever a blog posting was added.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the blog posting was added.</param>
        /// <param name="BlogPosting">The added blog posting.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public delegate Task OnBlogPostingAddedDelegate(DateTime          Timestamp,
                                                        BlogPosting       BlogPosting,
                                                        EventTracking_Id? EventTrackingId   = null,
                                                        User_Id?          CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a blog posting was added.
        /// </summary>
        public event OnBlogPostingAddedDelegate OnBlogPostingAdded;


        #region (protected internal) _AddBlogPosting(BlogPosting,                                OnAdded = null, ...)

        /// <summary>
        /// Add the given blog posting to the API.
        /// </summary>
        /// <param name="BlogPosting">A new blog posting to be added to this API.</param>
        /// <param name="OnAdded">A delegate run whenever the blog posting had been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task<BlogPosting> _AddBlogPosting(BlogPosting                            BlogPosting,
                                                          Action<BlogPosting, EventTracking_Id>  OnAdded           = null,
                                                          EventTracking_Id?                      EventTrackingId   = null,
                                                          User_Id?                               CurrentUserId     = null)
        {

            if (BlogPosting is null)
                throw new ArgumentNullException(nameof(BlogPosting),
                                                "The given blog posting must not be null!");

            if (BlogPosting.API != null && BlogPosting.API != this)
                throw new ArgumentException    ("The given blog posting is already attached to another API!",
                                                nameof(BlogPosting));

            if (_BlogPostings.ContainsKey(BlogPosting.Id))
                throw new ArgumentException    ("User group identification '" + BlogPosting.Id + "' already exists!",
                                                nameof(BlogPosting));

            if (BlogPosting.Id.Length < MinBlogPostingIdLength)
                throw new ArgumentException    ("User group identification '" + BlogPosting.Id + "' is too short!",
                                                nameof(BlogPosting));

            BlogPosting.API = this;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(addBlogPosting_MessageType,
                                      BlogPosting.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            _BlogPostings.Add(BlogPosting.Id, BlogPosting);


            var OnBlogPostingAddedLocal = OnBlogPostingAdded;
            if (OnBlogPostingAddedLocal is not null)
                await OnBlogPostingAddedLocal?.Invoke(Timestamp.Now,
                                                      BlogPosting,
                                                      eventTrackingId,
                                                      CurrentUserId);

            await SendNotifications(BlogPosting,
                                    addUser_MessageType,
                                    null,
                                    eventTrackingId,
                                    CurrentUserId);

            OnAdded?.Invoke(BlogPosting,
                            eventTrackingId);

            return BlogPosting;

        }

        #endregion

        #region AddBlogPosting             (BlogPosting,                                OnAdded = null, ...)

        /// <summary>
        /// Add the given blog posting.
        /// </summary>
        /// <param name="BlogPosting">A new blog posting.</param>
        /// <param name="OnAdded">A delegate run whenever the blog posting had been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<BlogPosting> AddBlogPosting(BlogPosting                            BlogPosting,
                                                      Action<BlogPosting, EventTracking_Id>  OnAdded           = null,
                                                      EventTracking_Id?                      EventTrackingId   = null,
                                                      User_Id?                               CurrentUserId     = null)
        {

            if (BlogPosting is null)
                throw new ArgumentNullException(nameof(BlogPosting), "The given blog posting must not be null!");

            try
            {

                return (await BlogPostingsSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _AddBlogPosting(BlogPosting,
                                                    OnAdded,
                                                    EventTrackingId,
                                                    CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    BlogPostingsSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion

        #region AddBlogPostingIfNotExists(BlogPosting, OnAdded = null,                   CurrentUserId = null)

        #region (protected internal) _AddBlogPostingIfNotExists(BlogPosting,                                OnAdded = null, ...)

        /// <summary>
        /// When it has not been created before, add the given blog posting to the API.
        /// </summary>
        /// <param name="BlogPosting">A new blog posting to be added to this API.</param>
        /// <param name="OnAdded">A delegate run whenever the blog posting had been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task<BlogPosting> _AddBlogPostingIfNotExists(BlogPosting                            BlogPosting,
                                                                     Action<BlogPosting, EventTracking_Id>  OnAdded           = null,
                                                                     EventTracking_Id?                      EventTrackingId   = null,
                                                                     User_Id?                               CurrentUserId     = null)
        {

            if (BlogPosting is null)
                throw new ArgumentNullException(nameof(BlogPosting),
                                                "The given blog posting must not be null!");

            if (BlogPosting.API != null && BlogPosting.API != this)
                throw new ArgumentException    ("The given blog posting is already attached to another API!",
                                                nameof(BlogPosting));

            if (_BlogPostings.ContainsKey(BlogPosting.Id))
                return _BlogPostings[BlogPosting.Id];

            if (BlogPosting.Id.Length < MinBlogPostingIdLength)
                throw new ArgumentException    ("User group identification '" + BlogPosting.Id + "' is too short!",
                                                nameof(BlogPosting));

            BlogPosting.API = this;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(addBlogPostingIfNotExists_MessageType,
                                      BlogPosting.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            _BlogPostings.Add(BlogPosting.Id, BlogPosting);

            var OnBlogPostingAddedLocal = OnBlogPostingAdded;
            if (OnBlogPostingAddedLocal is not null)
                await OnBlogPostingAddedLocal?.Invoke(Timestamp.Now,
                                                      BlogPosting,
                                                      eventTrackingId,
                                                      CurrentUserId);

            await SendNotifications(BlogPosting,
                                    addBlogPostingIfNotExists_MessageType,
                                    null,
                                    eventTrackingId,
                                    CurrentUserId);

            OnAdded?.Invoke(BlogPosting,
                            eventTrackingId);

            return BlogPosting;

        }

        #endregion

        #region AddBlogPostingIfNotExists             (BlogPosting,                                OnAdded = null, ...)

        /// <summary>
        /// Add the given blog posting.
        /// </summary>
        /// <param name="BlogPosting">A new blog posting.</param>
        /// <param name="OnAdded">A delegate run whenever the blog posting had been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<BlogPosting> AddBlogPostingIfNotExists(BlogPosting                            BlogPosting,
                                                                 Action<BlogPosting, EventTracking_Id>  OnAdded           = null,
                                                                 EventTracking_Id?                      EventTrackingId   = null,
                                                                 User_Id?                               CurrentUserId     = null)
        {

            try
            {

                return (await BlogPostingsSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _AddBlogPostingIfNotExists(BlogPosting,
                                                             OnAdded,
                                                             EventTrackingId,
                                                             CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    BlogPostingsSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion

        #region AddOrUpdateBlogPosting   (BlogPosting, OnAdded = null, OnUpdated = null, ...)

        #region (protected internal) _AddOrUpdateBlogPosting   (BlogPosting,   OnAdded = null, OnUpdated = null, ...)

        /// <summary>
        /// Add or update the given blog posting to/within the API.
        /// </summary>
        /// <param name="BlogPosting">A blog posting.</param>
        /// <param name="OnAdded">A delegate run whenever the blog posting had been added successfully.</param>
        /// <param name="OnUpdated">A delegate run whenever the blog posting had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional blog posting identification initiating this command/request.</param>
        protected internal async Task<BlogPosting> _AddOrUpdateBlogPosting(BlogPosting                            BlogPosting,
                                                                           Action<BlogPosting, EventTracking_Id>  OnAdded           = null,
                                                                           Action<BlogPosting, EventTracking_Id>  OnUpdated         = null,
                                                                           EventTracking_Id?                      EventTrackingId   = null,
                                                                           User_Id?                               CurrentUserId     = null)
        {

            if (BlogPosting is null)
                throw new ArgumentNullException(nameof(BlogPosting),
                                                "The given blog posting must not be null!");

            if (BlogPosting.API != null && BlogPosting.API != this)
                throw new ArgumentException    ("The given blog posting is already attached to another API!",
                                                nameof(BlogPosting));

            if (_BlogPostings.ContainsKey(BlogPosting.Id))
                return _BlogPostings[BlogPosting.Id];

            if (BlogPosting.Id.Length < MinBlogPostingIdLength)
                throw new ArgumentException    ("BlogPosting identification '" + BlogPosting.Id + "' is too short!",
                                                nameof(BlogPosting));

            BlogPosting.API = this;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(addOrUpdateBlogPosting_MessageType,
                                      BlogPosting.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            if (_BlogPostings.TryGetValue(BlogPosting.Id, out BlogPosting OldBlogPosting))
            {
                _BlogPostings.Remove(OldBlogPosting.Id);
                BlogPosting.CopyAllLinkedDataFrom(OldBlogPosting);
            }

            _BlogPostings.Add(BlogPosting.Id, BlogPosting);

            if (OldBlogPosting != null)
            {

                var OnBlogPostingUpdatedLocal = OnBlogPostingUpdated;
                if (OnBlogPostingUpdatedLocal is not null)
                    await OnBlogPostingUpdatedLocal?.Invoke(Timestamp.Now,
                                                            BlogPosting,
                                                            OldBlogPosting,
                                                            eventTrackingId,
                                                            CurrentUserId);

                await SendNotifications(BlogPosting,
                                        updateBlogPosting_MessageType,
                                        OldBlogPosting,
                                        eventTrackingId,
                                        CurrentUserId);

                OnUpdated?.Invoke(BlogPosting,
                                  eventTrackingId);

            }
            else
            {

                var OnBlogPostingAddedLocal = OnBlogPostingAdded;
                if (OnBlogPostingAddedLocal is not null)
                    await OnBlogPostingAddedLocal?.Invoke(Timestamp.Now,
                                                          BlogPosting,
                                                          eventTrackingId,
                                                          CurrentUserId);

                await SendNotifications(BlogPosting,
                                        addBlogPosting_MessageType,
                                        null,
                                        eventTrackingId,
                                        CurrentUserId);

                OnAdded?.Invoke(BlogPosting,
                                eventTrackingId);

            }

            return BlogPosting;

        }

        #endregion

        #region AddOrUpdateBlogPosting   (BlogPosting,   OnAdded = null, OnUpdated = null, ...)

        /// <summary>
        /// Add or update the given blog posting to/within the API.
        /// </summary>
        /// <param name="BlogPosting">A blog posting.</param>
        /// <param name="OnAdded">A delegate run whenever the blog posting had been added successfully.</param>
        /// <param name="OnUpdated">A delegate run whenever the blog posting had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional blog posting identification initiating this command/request.</param>
        public async Task<BlogPosting> AddOrUpdateBlogPosting(BlogPosting                            BlogPosting,
                                                              Action<BlogPosting, EventTracking_Id>  OnAdded           = null,
                                                              Action<BlogPosting, EventTracking_Id>  OnUpdated         = null,
                                                              EventTracking_Id?                      EventTrackingId   = null,
                                                              User_Id?                               CurrentUserId     = null)
        {

            if (BlogPosting is null)
                throw new ArgumentNullException(nameof(BlogPosting), "The given blog posting must not be null!");

            try
            {

                return (await BlogPostingsSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _AddOrUpdateBlogPosting(BlogPosting,
                                                            OnAdded,
                                                            OnUpdated,
                                                            EventTrackingId,
                                                            CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    BlogPostingsSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion

        #region UpdateBlogPosting        (BlogPosting,                 OnUpdated = null, ...)

        /// <summary>
        /// A delegate called whenever a blog posting was updated.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the blog posting was updated.</param>
        /// <param name="BlogPosting">The updated blog posting.</param>
        /// <param name="OldBlogPosting">The old blog posting.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">The invoking blog posting identification</param>
        public delegate Task OnBlogPostingUpdatedDelegate(DateTime          Timestamp,
                                                          BlogPosting       BlogPosting,
                                                          BlogPosting       OldBlogPosting,
                                                          EventTracking_Id? EventTrackingId   = null,
                                                          User_Id?          CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a blog posting was updated.
        /// </summary>
        public event OnBlogPostingUpdatedDelegate OnBlogPostingUpdated;


        #region (protected internal) _UpdateBlogPosting(BlogPosting, OnUpdated = null, ...)

        /// <summary>
        /// Update the given blog posting to/within the API.
        /// </summary>
        /// <param name="BlogPosting">A blog posting.</param>
        /// <param name="OnUpdated">A delegate run whenever the blog posting had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional blog posting identification initiating this command/request.</param>
        protected internal async Task<BlogPosting> _UpdateBlogPosting(BlogPosting                            BlogPosting,
                                                             Action<BlogPosting, EventTracking_Id>  OnUpdated         = null,
                                                             EventTracking_Id?                      EventTrackingId   = null,
                                                             User_Id?                               CurrentUserId     = null)
        {

            if (BlogPosting is null)
                throw new ArgumentNullException(nameof(BlogPosting),
                                                "The given blog posting must not be null!");

            if (BlogPosting.API != null && BlogPosting.API != this)
                throw new ArgumentException    ("The given blog posting is already attached to another API!",
                                                nameof(BlogPosting));

            if (!_BlogPostings.TryGetValue(BlogPosting.Id, out BlogPosting OldBlogPosting))
                throw new ArgumentException    ("The given blog posting '" + BlogPosting.Id + "' does not exists in this API!",
                                                nameof(BlogPosting));

            BlogPosting.API = this;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(updateBlogPosting_MessageType,
                                      BlogPosting.ToJSON(),
                                      eventTrackingId,
                                      CurrentUserId);

            _BlogPostings.Remove(OldBlogPosting.Id);
            BlogPosting.CopyAllLinkedDataFrom(OldBlogPosting);


            var OnBlogPostingUpdatedLocal = OnBlogPostingUpdated;
            if (OnBlogPostingUpdatedLocal is not null)
                await OnBlogPostingUpdatedLocal?.Invoke(Timestamp.Now,
                                                        BlogPosting,
                                                        OldBlogPosting,
                                                        eventTrackingId,
                                                        CurrentUserId);

            await SendNotifications(BlogPosting,
                                    updateBlogPosting_MessageType,
                                    OldBlogPosting,
                                    eventTrackingId,
                                    CurrentUserId);

            OnUpdated?.Invoke(BlogPosting,
                              eventTrackingId);

            return BlogPosting;

        }

        #endregion

        #region UpdateBlogPosting             (BlogPosting, OnUpdated = null, ...)

        /// <summary>
        /// Update the given blog posting to/within the API.
        /// </summary>
        /// <param name="BlogPosting">A blog posting.</param>
        /// <param name="OnUpdated">A delegate run whenever the blog posting had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional blog posting identification initiating this command/request.</param>
        public async Task<BlogPosting> UpdateBlogPosting(BlogPosting                            BlogPosting,
                                                         Action<BlogPosting, EventTracking_Id>  OnUpdated         = null,
                                                         EventTracking_Id?                      EventTrackingId   = null,
                                                         User_Id?                               CurrentUserId     = null)
        {

            if (BlogPosting is null)
                throw new ArgumentNullException(nameof(BlogPosting), "The given blog posting must not be null!");

            try
            {

                return (await BlogPostingsSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _UpdateBlogPosting(BlogPosting,
                                                       OnUpdated,
                                                       EventTrackingId,
                                                       CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    BlogPostingsSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion


        #region (protected internal) _UpdateBlogPosting(BlogPostingId, UpdateDelegate, OnUpdated = null, ...)

        /// <summary>
        /// Update the given blog posting.
        /// </summary>
        /// <param name="BlogPostingId">An blog posting identification.</param>
        /// <param name="UpdateDelegate">A delegate to update the given blog posting.</param>
        /// <param name="OnUpdated">A delegate run whenever the blog posting had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional blog posting identification initiating this command/request.</param>
        protected internal async Task<BlogPosting> _UpdateBlogPosting(BlogPosting_Id                         BlogPostingId,
                                                             Action<BlogPosting.Builder>            UpdateDelegate,
                                                             Action<BlogPosting, EventTracking_Id>  OnUpdated         = null,
                                                             EventTracking_Id?                      EventTrackingId   = null,
                                                             User_Id?                               CurrentUserId     = null)
        {

            if (BlogPostingId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(BlogPostingId),
                                                "The given blog posting identification must not be null or empty!");

            if (UpdateDelegate == null)
                throw new ArgumentNullException(nameof(UpdateDelegate),
                                                "The given update delegate must not be null!");

            if (!_BlogPostings.TryGetValue(BlogPostingId, out BlogPosting OldBlogPosting))
                throw new ArgumentException    ("The given blog posting '" + BlogPostingId + "' does not exists in this API!",
                                                nameof(BlogPostingId));

            var Builder = OldBlogPosting.ToBuilder();
            UpdateDelegate(Builder);
            var NewBlogPosting = Builder.ToImmutable;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(updateBlogPosting_MessageType,
                                      NewBlogPosting.ToJSON(),
                                      eventTrackingId,
                                      CurrentUserId);

            _BlogPostings.Remove(OldBlogPosting.Id);
            NewBlogPosting.CopyAllLinkedDataFrom(OldBlogPosting);


            var OnBlogPostingUpdatedLocal = OnBlogPostingUpdated;
            if (OnBlogPostingUpdatedLocal is not null)
                await OnBlogPostingUpdatedLocal?.Invoke(Timestamp.Now,
                                                        NewBlogPosting,
                                                        OldBlogPosting,
                                                        eventTrackingId,
                                                        CurrentUserId);

            await SendNotifications(NewBlogPosting,
                                    updateBlogPosting_MessageType,
                                    OldBlogPosting,
                                    eventTrackingId,
                                    CurrentUserId);

            OnUpdated?.Invoke(NewBlogPosting,
                              eventTrackingId);

            return NewBlogPosting;

        }

        #endregion

        #region UpdateBlogPosting             (BlogPostingId, UpdateDelegate, OnUpdated = null, ...)

        /// <summary>
        /// Update the given blog posting.
        /// </summary>
        /// <param name="BlogPostingId">An blog posting identification.</param>
        /// <param name="UpdateDelegate">A delegate to update the given blog posting.</param>
        /// <param name="OnUpdated">A delegate run whenever the blog posting had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional blog posting identification initiating this command/request.</param>
        public async Task<BlogPosting> UpdateBlogPosting(BlogPosting_Id                         BlogPostingId,
                                                         Action<BlogPosting.Builder>            UpdateDelegate,
                                                         Action<BlogPosting, EventTracking_Id>  OnUpdated         = null,
                                                         EventTracking_Id?                      EventTrackingId   = null,
                                                         User_Id?                               CurrentUserId     = null)
        {

            if (BlogPostingId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(BlogPostingId), "The given blog posting identification must not be null or empty!");

            try
            {

                return (await BlogPostingsSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _UpdateBlogPosting(BlogPostingId,
                                                       UpdateDelegate,
                                                       OnUpdated,
                                                       EventTrackingId,
                                                       CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    BlogPostingsSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion


        #region BlogPostingExists(BlogPostingId)

        /// <summary>
        /// Determines whether the given blog posting identification exists within this API.
        /// </summary>
        /// <param name="BlogPostingId">The unique identification of an blog posting.</param>
        protected internal Boolean _BlogPostingExists(BlogPosting_Id BlogPostingId)

            => !BlogPostingId.IsNullOrEmpty && _BlogPostings.ContainsKey(BlogPostingId);


        /// <summary>
        /// Determines whether the given blog posting identification exists within this API.
        /// </summary>
        /// <param name="BlogPostingId">The unique identification of an blog posting.</param>
        public Boolean BlogPostingExists(BlogPosting_Id BlogPostingId)
        {

            try
            {

                if (BlogPostingsSemaphore.Wait(SemaphoreSlimTimeout) &&
                    _BlogPostingExists(BlogPostingId))
                {
                    return true;
                }

            }
            catch
            { }
            finally
            {
                try
                {
                    BlogPostingsSemaphore.Release();
                }
                catch
                { }
            }

            return false;

        }

        #endregion

        #region GetBlogPosting   (BlogPostingId)

        /// <summary>
        /// Get the blog posting having the given unique identification.
        /// </summary>
        /// <param name="BlogPostingId">The unique identification of an blog posting.</param>
        protected internal BlogPosting _GetBlogPosting(BlogPosting_Id BlogPostingId)
        {

            if (!BlogPostingId.IsNullOrEmpty && _BlogPostings.TryGetValue(BlogPostingId, out BlogPosting blogPosting))
                return blogPosting;

            return null;

        }


        /// <summary>
        /// Get the blog posting having the given unique identification.
        /// </summary>
        /// <param name="BlogPostingId">The unique identification of the blog posting.</param>
        public BlogPosting GetBlogPosting(BlogPosting_Id BlogPostingId)
        {

            try
            {

                if (BlogPostingsSemaphore.Wait(SemaphoreSlimTimeout))
                    return _GetBlogPosting(BlogPostingId);

            }
            catch
            { }
            finally
            {
                try
                {
                    BlogPostingsSemaphore.Release();
                }
                catch
                { }
            }

            return null;

        }

        #endregion

        #region TryGetBlogPosting(BlogPostingId, out BlogPosting)

        /// <summary>
        /// Try to get the blog posting having the given unique identification.
        /// </summary>
        /// <param name="BlogPostingId">The unique identification of an blog posting.</param>
        /// <param name="BlogPosting">The blog posting.</param>
        protected internal Boolean _TryGetBlogPosting(BlogPosting_Id BlogPostingId, out BlogPosting BlogPosting)
        {

            if (!BlogPostingId.IsNullOrEmpty && _BlogPostings.TryGetValue(BlogPostingId, out BlogPosting blogPosting))
            {
                BlogPosting = blogPosting;
                return true;
            }

            BlogPosting = null;
            return false;

        }


        /// <summary>
        /// Try to get the blog posting having the given unique identification.
        /// </summary>
        /// <param name="BlogPostingId">The unique identification of an blog posting.</param>
        /// <param name="BlogPosting">The blog posting.</param>
        public Boolean TryGetBlogPosting(BlogPosting_Id   BlogPostingId,
                                         out BlogPosting  BlogPosting)
        {

            try
            {

                if (BlogPostingsSemaphore.Wait(SemaphoreSlimTimeout) &&
                    _TryGetBlogPosting(BlogPostingId, out BlogPosting blogPosting))
                {
                    BlogPosting = blogPosting;
                    return true;
                }

            }
            catch
            { }
            finally
            {
                try
                {
                    BlogPostingsSemaphore.Release();
                }
                catch
                { }
            }

            BlogPosting = null;
            return false;

        }

        #endregion


        #region RemoveBlogPosting(BlogPosting, OnRemoved = null, ...)

        /// <summary>
        /// A delegate called whenever a blog posting was removed.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the blog posting was removed.</param>
        /// <param name="BlogPosting">The removed blog posting.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">The invoking blog posting identification</param>
        public delegate Task OnBlogPostingRemovedDelegate(DateTime          Timestamp,
                                                          BlogPosting       BlogPosting,
                                                          EventTracking_Id? EventTrackingId   = null,
                                                          User_Id?          CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a blog posting was removed.
        /// </summary>
        public event OnBlogPostingRemovedDelegate OnBlogPostingRemoved;


        #region (class) DeleteBlogPostingResult

        public class DeleteBlogPostingResult
        {

            public Boolean     IsSuccess           { get; }

            public I18NString  ErrorDescription    { get; }


            private DeleteBlogPostingResult(Boolean     IsSuccess,
                                          I18NString  ErrorDescription  = null)
            {
                this.IsSuccess         = IsSuccess;
                this.ErrorDescription  = ErrorDescription;
            }


            public static DeleteBlogPostingResult Success

                => new DeleteBlogPostingResult(true);

            public static DeleteBlogPostingResult Failed(I18NString Reason)

                => new DeleteBlogPostingResult(false,
                                             Reason);

            public static DeleteBlogPostingResult Failed(Exception Exception)

                => new DeleteBlogPostingResult(false,
                                             I18NString.Create(Languages.en,
                                                               Exception.Message));

            public override String ToString()

                => IsSuccess
                       ? "Success"
                       : "Failed" + (ErrorDescription.IsNullOrEmpty()
                                         ? ": " + ErrorDescription.FirstText()
                                         : "!");

        }

        #endregion

        #region (protected internal virtual) CanDeleteBlogPosting(BlogPosting)

        /// <summary>
        /// Determines whether the blog posting can safely be removed from the API.
        /// </summary>
        /// <param name="BlogPosting">The blog posting to be removed.</param>
        protected internal virtual I18NString CanDeleteBlogPosting(BlogPosting BlogPosting)
        {
            return new I18NString(Languages.en, "Currently not possible!");
        }

        #endregion


        #region (protected internal) _RemoveBlogPosting(BlogPosting, OnRemoved = null, ...)

        /// <summary>
        /// Remove the given blog posting from the API.
        /// </summary>
        /// <param name="BlogPosting">The blog posting to be removed from this API.</param>
        /// <param name="OnRemoved">A delegate run whenever the blog posting had been removed successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional blog posting identification initiating this command/request.</param>
        protected internal async Task<DeleteBlogPostingResult> _RemoveBlogPosting(BlogPosting                            BlogPosting,
                                                                         Action<BlogPosting, EventTracking_Id>  OnRemoved         = null,
                                                                         EventTracking_Id?                      EventTrackingId   = null,
                                                                         User_Id?                               CurrentUserId     = null)
        {

            if (BlogPosting is null)
                throw new ArgumentNullException(nameof(BlogPosting),
                                                "The given blog posting must not be null!");

            if (BlogPosting.API != this || !_BlogPostings.TryGetValue(BlogPosting.Id, out BlogPosting BlogPostingToBeRemoved))
                throw new ArgumentException    ("The given blog posting '" + BlogPosting.Id + "' does not exists in this API!",
                                                nameof(BlogPosting));


            var result = CanDeleteBlogPosting(BlogPosting);

            if (result == null)
            {

                var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

                await WriteToDatabaseFile(removeBlogPosting_MessageType,
                                          BlogPosting.ToJSON(false),
                                          eventTrackingId,
                                          CurrentUserId);

                _BlogPostings.Remove(BlogPosting.Id);


                var OnBlogPostingRemovedLocal = OnBlogPostingRemoved;
                if (OnBlogPostingRemovedLocal is not null)
                    await OnBlogPostingRemovedLocal?.Invoke(Timestamp.Now,
                                                            BlogPosting,
                                                            eventTrackingId,
                                                            CurrentUserId);

                await SendNotifications(BlogPosting,
                                        removeBlogPosting_MessageType,
                                        null,
                                        eventTrackingId,
                                        CurrentUserId);

                OnRemoved?.Invoke(BlogPosting,
                                  eventTrackingId);

                return DeleteBlogPostingResult.Success;

            }
            else
                return DeleteBlogPostingResult.Failed(result);

        }

        #endregion

        #region RemoveBlogPosting             (BlogPosting, OnRemoved = null, ...)

        /// <summary>
        /// Remove the given blog posting from the API.
        /// </summary>
        /// <param name="BlogPosting">The blog posting to be removed from this API.</param>
        /// <param name="OnRemoved">A delegate run whenever the blog posting had been removed successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional blog posting identification initiating this command/request.</param>
        public async Task<DeleteBlogPostingResult> RemoveBlogPosting(BlogPosting                            BlogPosting,
                                                                     Action<BlogPosting, EventTracking_Id>  OnRemoved         = null,
                                                                     EventTracking_Id?                      EventTrackingId   = null,
                                                                     User_Id?                               CurrentUserId     = null)
        {

            if (BlogPosting is null)
                throw new ArgumentNullException(nameof(BlogPosting), "The given blog posting must not be null!");

            try
            {

                return (await BlogPostingsSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _RemoveBlogPosting(BlogPosting,
                                                       OnRemoved,
                                                       EventTrackingId,
                                                       CurrentUserId)

                            : null;

            }
            catch (Exception e)
            {
                return DeleteBlogPostingResult.Failed(e);
            }
            finally
            {
                try
                {
                    BlogPostingsSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion

        #endregion

        #region NewsPostings

        #region Data

        /// <summary>
        /// An enumeration of all news postings.
        /// </summary>
        protected internal readonly Dictionary<NewsPosting_Id, NewsPosting> _NewsPostings;

        /// <summary>
        /// An enumeration of all news postings.
        /// </summary>
        public IEnumerable<NewsPosting> NewsPostings
        {
            get
            {
                try
                {
                    return NewsPostingsSemaphore.Wait(SemaphoreSlimTimeout)
                               ? _NewsPostings.Values.ToArray()
                               : new NewsPosting[0];
                }
                finally
                {
                    try
                    {
                        NewsPostingsSemaphore.Release();
                    }
                    catch
                    { }
                }
            }
        }

        #endregion


        #region (protected internal) WriteToDatabaseFileAndNotify(NewsPosting, MessageType,  OldNewsPosting = null, ...)

        /// <summary>
        /// Write the given news posting to the database and send out notifications.
        /// </summary>
        /// <param name="NewsPosting">The news posting.</param>
        /// <param name="MessageType">The user notification.</param>
        /// <param name="OldNewsPosting">The old/updated news posting.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task WriteToDatabaseFileAndNotify(NewsPosting              NewsPosting,
                                                          NotificationMessageType  MessageType,
                                                          NewsPosting              OldNewsPosting    = null,
                                                          EventTracking_Id?        EventTrackingId   = null,
                                                          User_Id?                 CurrentUserId     = null)
        {

            if (NewsPosting is null)
                throw new ArgumentNullException(nameof(NewsPosting),  "The given news posting must not be null or empty!");

            if (MessageType.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(MessageType),  "The given message type must not be null or empty!");


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(MessageType,
                                      NewsPosting.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            await SendNotifications(NewsPosting,
                                    MessageType,
                                    OldNewsPosting,
                                    eventTrackingId,
                                    CurrentUserId);

        }

        #endregion

        #region (protected internal) SendNotifications           (NewsPosting, MessageTypes, OldNewsPosting = null, ...)

        /// <summary>
        /// Send news posting notifications.
        /// </summary>
        /// <param name="NewsPosting">The news posting.</param>
        /// <param name="MessageType">The user notification.</param>
        /// <param name="OldNewsPosting">The old/updated news posting.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task SendNotifications(NewsPosting              NewsPosting,
                                               NotificationMessageType  MessageType,
                                               NewsPosting              OldNewsPosting    = null,
                                               EventTracking_Id?        EventTrackingId   = null,
                                               User_Id?                 CurrentUserId     = null)
        {

            if (NewsPosting is null)
                throw new ArgumentNullException(nameof(NewsPosting),  "The given news posting must not be null or empty!");

            if (MessageType.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(MessageType),  "The given message type must not be null or empty!");


            await SendNotifications(NewsPosting,
                                    new NotificationMessageType[] { MessageType },
                                    OldNewsPosting,
                                    EventTrackingId,
                                    CurrentUserId);

        }


        /// <summary>
        /// Send news posting notifications.
        /// </summary>
        /// <param name="NewsPosting">The news posting.</param>
        /// <param name="MessageTypes">The user notifications.</param>
        /// <param name="OldNewsPosting">The old/updated news posting.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task SendNotifications(NewsPosting                           NewsPosting,
                                               IEnumerable<NotificationMessageType>  MessageTypes,
                                               NewsPosting                           OldNewsPosting    = null,
                                               EventTracking_Id?                     EventTrackingId   = null,
                                               User_Id?                              CurrentUserId     = null)
        {

            if (NewsPosting is null)
                throw new ArgumentNullException(nameof(NewsPosting),   "The given news posting must not be null or empty!");

            var messageTypesHash = new HashSet<NotificationMessageType>(MessageTypes.Where(messageType => !messageType.IsNullOrEmpty));

            if (messageTypesHash.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(MessageTypes),  "The given enumeration of message types must not be null or empty!");

            if (messageTypesHash.Contains(addUserIfNotExists_MessageType))
                messageTypesHash.Add(addUser_MessageType);

            if (messageTypesHash.Contains(addOrUpdateUser_MessageType))
                messageTypesHash.Add(OldNewsPosting == null
                                       ? addUser_MessageType
                                       : updateUser_MessageType);

            var messageTypes = messageTypesHash.ToArray();


            if (!DisableNotifications)
            {


            }

        }

        #endregion

        #region (protected internal) GetNewsPostingSerializator(Request, User)

        protected internal NewsPostingToJSONDelegate GetNewsPostingSerializator(HTTPRequest  Request,
                                                                                IUser        User)
        {

            switch (User?.Id.ToString())
            {

                default:
                    return (newsPosting,
                            embedded,
                            ExpandTags,
                            ExpandAuthorId)

                            => newsPosting.ToJSON(embedded,
                                                  ExpandTags,
                                                  ExpandAuthorId);

            }

        }

        #endregion


        #region AddNewsPosting           (NewsPosting, OnAdded = null,                   CurrentUserId = null)

        /// <summary>
        /// A delegate called whenever a news posting was added.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the news posting was added.</param>
        /// <param name="NewsPosting">The added news posting.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public delegate Task OnNewsPostingAddedDelegate(DateTime          Timestamp,
                                                        NewsPosting       NewsPosting,
                                                        EventTracking_Id? EventTrackingId   = null,
                                                        User_Id?          CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a news posting was added.
        /// </summary>
        public event OnNewsPostingAddedDelegate OnNewsPostingAdded;


        #region (protected internal) _AddNewsPosting(NewsPosting,                                OnAdded = null, ...)

        /// <summary>
        /// Add the given news posting to the API.
        /// </summary>
        /// <param name="NewsPosting">A new news posting to be added to this API.</param>
        /// <param name="OnAdded">A delegate run whenever the news posting had been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task<NewsPosting> _AddNewsPosting(NewsPosting                            NewsPosting,
                                                          Action<NewsPosting, EventTracking_Id>  OnAdded           = null,
                                                          EventTracking_Id?                      EventTrackingId   = null,
                                                          User_Id?                               CurrentUserId     = null)
        {

            if (NewsPosting is null)
                throw new ArgumentNullException(nameof(NewsPosting),
                                                "The given news posting must not be null!");

            if (NewsPosting.API != null && NewsPosting.API != this)
                throw new ArgumentException    ("The given news posting is already attached to another API!",
                                                nameof(NewsPosting));

            if (_NewsPostings.ContainsKey(NewsPosting.Id))
                throw new ArgumentException    ("User group identification '" + NewsPosting.Id + "' already exists!",
                                                nameof(NewsPosting));

            if (NewsPosting.Id.Length < MinNewsPostingIdLength)
                throw new ArgumentException    ("User group identification '" + NewsPosting.Id + "' is too short!",
                                                nameof(NewsPosting));

            NewsPosting.API = this;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(addNewsPosting_MessageType,
                                      NewsPosting.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            _NewsPostings.Add(NewsPosting.Id, NewsPosting);


            var OnNewsPostingAddedLocal = OnNewsPostingAdded;
            if (OnNewsPostingAddedLocal is not null)
                await OnNewsPostingAddedLocal?.Invoke(Timestamp.Now,
                                                      NewsPosting,
                                                      eventTrackingId,
                                                      CurrentUserId);

            await SendNotifications(NewsPosting,
                                    addUser_MessageType,
                                    null,
                                    eventTrackingId,
                                    CurrentUserId);

            OnAdded?.Invoke(NewsPosting,
                            eventTrackingId);

            return NewsPosting;

        }

        #endregion

        #region AddNewsPosting             (NewsPosting,                                OnAdded = null, ...)

        /// <summary>
        /// Add the given news posting.
        /// </summary>
        /// <param name="NewsPosting">A new news posting.</param>
        /// <param name="OnAdded">A delegate run whenever the news posting had been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<NewsPosting> AddNewsPosting(NewsPosting                            NewsPosting,
                                                      Action<NewsPosting, EventTracking_Id>  OnAdded           = null,
                                                      EventTracking_Id?                      EventTrackingId   = null,
                                                      User_Id?                               CurrentUserId     = null)
        {

            if (NewsPosting is null)
                throw new ArgumentNullException(nameof(NewsPosting), "The given news posting must not be null!");

            try
            {

                return (await NewsPostingsSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _AddNewsPosting(NewsPosting,
                                                    OnAdded,
                                                    EventTrackingId,
                                                    CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    NewsPostingsSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion

        #region AddNewsPostingIfNotExists(NewsPosting, OnAdded = null,                   CurrentUserId = null)

        #region (protected internal) _AddNewsPostingIfNotExists(NewsPosting,                                OnAdded = null, ...)

        /// <summary>
        /// When it has not been created before, add the given news posting to the API.
        /// </summary>
        /// <param name="NewsPosting">A new news posting to be added to this API.</param>
        /// <param name="OnAdded">A delegate run whenever the news posting had been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task<NewsPosting> _AddNewsPostingIfNotExists(NewsPosting                            NewsPosting,
                                                                     Action<NewsPosting, EventTracking_Id>  OnAdded           = null,
                                                                     EventTracking_Id?                      EventTrackingId   = null,
                                                                     User_Id?                               CurrentUserId     = null)
        {

            if (NewsPosting is null)
                throw new ArgumentNullException(nameof(NewsPosting),
                                                "The given news posting must not be null!");

            if (NewsPosting.API != null && NewsPosting.API != this)
                throw new ArgumentException    ("The given news posting is already attached to another API!",
                                                nameof(NewsPosting));

            if (_NewsPostings.ContainsKey(NewsPosting.Id))
                return _NewsPostings[NewsPosting.Id];

            if (NewsPosting.Id.Length < MinNewsPostingIdLength)
                throw new ArgumentException    ("User group identification '" + NewsPosting.Id + "' is too short!",
                                                nameof(NewsPosting));

            NewsPosting.API = this;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(addNewsPostingIfNotExists_MessageType,
                                      NewsPosting.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            _NewsPostings.Add(NewsPosting.Id, NewsPosting);

            var OnNewsPostingAddedLocal = OnNewsPostingAdded;
            if (OnNewsPostingAddedLocal is not null)
                await OnNewsPostingAddedLocal?.Invoke(Timestamp.Now,
                                                      NewsPosting,
                                                      eventTrackingId,
                                                      CurrentUserId);

            await SendNotifications(NewsPosting,
                                    addNewsPostingIfNotExists_MessageType,
                                    null,
                                    eventTrackingId,
                                    CurrentUserId);

            OnAdded?.Invoke(NewsPosting,
                            eventTrackingId);

            return NewsPosting;

        }

        #endregion

        #region AddNewsPostingIfNotExists             (NewsPosting,                                OnAdded = null, ...)

        /// <summary>
        /// Add the given news posting.
        /// </summary>
        /// <param name="NewsPosting">A new news posting.</param>
        /// <param name="OnAdded">A delegate run whenever the news posting had been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<NewsPosting> AddNewsPostingIfNotExists(NewsPosting                            NewsPosting,
                                                                 Action<NewsPosting, EventTracking_Id>  OnAdded           = null,
                                                                 EventTracking_Id?                      EventTrackingId   = null,
                                                                 User_Id?                               CurrentUserId     = null)
        {

            try
            {

                return (await NewsPostingsSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _AddNewsPostingIfNotExists(NewsPosting,
                                                             OnAdded,
                                                             EventTrackingId,
                                                             CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    NewsPostingsSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion

        #region AddOrUpdateNewsPosting   (NewsPosting, OnAdded = null, OnUpdated = null, ...)

        #region (protected internal) _AddOrUpdateNewsPosting   (NewsPosting,   OnAdded = null, OnUpdated = null, ...)

        /// <summary>
        /// Add or update the given news posting to/within the API.
        /// </summary>
        /// <param name="NewsPosting">A news posting.</param>
        /// <param name="OnAdded">A delegate run whenever the news posting had been added successfully.</param>
        /// <param name="OnUpdated">A delegate run whenever the news posting had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional news posting identification initiating this command/request.</param>
        protected internal async Task<NewsPosting> _AddOrUpdateNewsPosting(NewsPosting                            NewsPosting,
                                                                  Action<NewsPosting, EventTracking_Id>  OnAdded           = null,
                                                                  Action<NewsPosting, EventTracking_Id>  OnUpdated         = null,
                                                                  EventTracking_Id?                      EventTrackingId   = null,
                                                                  User_Id?                               CurrentUserId     = null)
        {

            if (NewsPosting is null)
                throw new ArgumentNullException(nameof(NewsPosting),
                                                "The given news posting must not be null!");

            if (NewsPosting.API != null && NewsPosting.API != this)
                throw new ArgumentException    ("The given news posting is already attached to another API!",
                                                nameof(NewsPosting));

            if (_NewsPostings.ContainsKey(NewsPosting.Id))
                return _NewsPostings[NewsPosting.Id];

            if (NewsPosting.Id.Length < MinNewsPostingIdLength)
                throw new ArgumentException    ("NewsPosting identification '" + NewsPosting.Id + "' is too short!",
                                                nameof(NewsPosting));

            NewsPosting.API = this;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(addOrUpdateNewsPosting_MessageType,
                                      NewsPosting.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            if (_NewsPostings.TryGetValue(NewsPosting.Id, out NewsPosting OldNewsPosting))
            {
                _NewsPostings.Remove(OldNewsPosting.Id);
                NewsPosting.CopyAllLinkedDataFrom(OldNewsPosting);
            }

            _NewsPostings.Add(NewsPosting.Id, NewsPosting);

            if (OldNewsPosting != null)
            {

                var OnNewsPostingUpdatedLocal = OnNewsPostingUpdated;
                if (OnNewsPostingUpdatedLocal is not null)
                    await OnNewsPostingUpdatedLocal?.Invoke(Timestamp.Now,
                                                            NewsPosting,
                                                            OldNewsPosting,
                                                            eventTrackingId,
                                                            CurrentUserId);

                await SendNotifications(NewsPosting,
                                        updateNewsPosting_MessageType,
                                        OldNewsPosting,
                                        eventTrackingId,
                                        CurrentUserId);

                OnUpdated?.Invoke(NewsPosting,
                                  eventTrackingId);

            }
            else
            {

                var OnNewsPostingAddedLocal = OnNewsPostingAdded;
                if (OnNewsPostingAddedLocal is not null)
                    await OnNewsPostingAddedLocal?.Invoke(Timestamp.Now,
                                                          NewsPosting,
                                                          eventTrackingId,
                                                          CurrentUserId);

                await SendNotifications(NewsPosting,
                                        addNewsPosting_MessageType,
                                        null,
                                        eventTrackingId,
                                        CurrentUserId);

                OnAdded?.Invoke(NewsPosting,
                                eventTrackingId);

            }

            return NewsPosting;

        }

        #endregion

        #region AddOrUpdateNewsPosting   (NewsPosting,   OnAdded = null, OnUpdated = null, ...)

        /// <summary>
        /// Add or update the given news posting to/within the API.
        /// </summary>
        /// <param name="NewsPosting">A news posting.</param>
        /// <param name="OnAdded">A delegate run whenever the news posting had been added successfully.</param>
        /// <param name="OnUpdated">A delegate run whenever the news posting had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional news posting identification initiating this command/request.</param>
        public async Task<NewsPosting> AddOrUpdateNewsPosting(NewsPosting                            NewsPosting,
                                                              Action<NewsPosting, EventTracking_Id>  OnAdded           = null,
                                                              Action<NewsPosting, EventTracking_Id>  OnUpdated         = null,
                                                              EventTracking_Id?                      EventTrackingId   = null,
                                                              User_Id?                               CurrentUserId     = null)
        {

            if (NewsPosting is null)
                throw new ArgumentNullException(nameof(NewsPosting), "The given news posting must not be null!");

            try
            {

                return (await NewsPostingsSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _AddOrUpdateNewsPosting(NewsPosting,
                                                            OnAdded,
                                                            OnUpdated,
                                                            EventTrackingId,
                                                            CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    NewsPostingsSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion

        #region UpdateNewsPosting        (NewsPosting,                 OnUpdated = null, ...)

        /// <summary>
        /// A delegate called whenever a news posting was updated.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the news posting was updated.</param>
        /// <param name="NewsPosting">The updated news posting.</param>
        /// <param name="OldNewsPosting">The old news posting.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">The invoking news posting identification</param>
        public delegate Task OnNewsPostingUpdatedDelegate(DateTime          Timestamp,
                                                          NewsPosting       NewsPosting,
                                                          NewsPosting       OldNewsPosting,
                                                          EventTracking_Id? EventTrackingId   = null,
                                                          User_Id?          CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a news posting was updated.
        /// </summary>
        public event OnNewsPostingUpdatedDelegate OnNewsPostingUpdated;


        #region (protected internal) _UpdateNewsPosting(NewsPosting, OnUpdated = null, ...)

        /// <summary>
        /// Update the given news posting to/within the API.
        /// </summary>
        /// <param name="NewsPosting">A news posting.</param>
        /// <param name="OnUpdated">A delegate run whenever the news posting had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional news posting identification initiating this command/request.</param>
        protected internal async Task<NewsPosting> _UpdateNewsPosting(NewsPosting                            NewsPosting,
                                                             Action<NewsPosting, EventTracking_Id>  OnUpdated         = null,
                                                             EventTracking_Id?                      EventTrackingId   = null,
                                                             User_Id?                               CurrentUserId     = null)
        {

            if (NewsPosting is null)
                throw new ArgumentNullException(nameof(NewsPosting),
                                                "The given news posting must not be null!");

            if (NewsPosting.API != null && NewsPosting.API != this)
                throw new ArgumentException    ("The given news posting is already attached to another API!",
                                                nameof(NewsPosting));

            if (!_NewsPostings.TryGetValue(NewsPosting.Id, out NewsPosting OldNewsPosting))
                throw new ArgumentException    ("The given news posting '" + NewsPosting.Id + "' does not exists in this API!",
                                                nameof(NewsPosting));

            NewsPosting.API = this;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(updateNewsPosting_MessageType,
                                      NewsPosting.ToJSON(),
                                      eventTrackingId,
                                      CurrentUserId);

            _NewsPostings.Remove(OldNewsPosting.Id);
            NewsPosting.CopyAllLinkedDataFrom(OldNewsPosting);


            var OnNewsPostingUpdatedLocal = OnNewsPostingUpdated;
            if (OnNewsPostingUpdatedLocal is not null)
                await OnNewsPostingUpdatedLocal?.Invoke(Timestamp.Now,
                                                        NewsPosting,
                                                        OldNewsPosting,
                                                        eventTrackingId,
                                                        CurrentUserId);

            await SendNotifications(NewsPosting,
                                    updateNewsPosting_MessageType,
                                    OldNewsPosting,
                                    eventTrackingId,
                                    CurrentUserId);

            OnUpdated?.Invoke(NewsPosting,
                              eventTrackingId);

            return NewsPosting;

        }

        #endregion

        #region UpdateNewsPosting             (NewsPosting, OnUpdated = null, ...)

        /// <summary>
        /// Update the given news posting to/within the API.
        /// </summary>
        /// <param name="NewsPosting">A news posting.</param>
        /// <param name="OnUpdated">A delegate run whenever the news posting had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional news posting identification initiating this command/request.</param>
        public async Task<NewsPosting> UpdateNewsPosting(NewsPosting                            NewsPosting,
                                                         Action<NewsPosting, EventTracking_Id>  OnUpdated         = null,
                                                         EventTracking_Id?                      EventTrackingId   = null,
                                                         User_Id?                               CurrentUserId     = null)
        {

            if (NewsPosting is null)
                throw new ArgumentNullException(nameof(NewsPosting), "The given news posting must not be null!");

            try
            {

                return (await NewsPostingsSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _UpdateNewsPosting(NewsPosting,
                                                       OnUpdated,
                                                       EventTrackingId,
                                                       CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    NewsPostingsSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion


        #region (protected internal) _UpdateNewsPosting(NewsPostingId, UpdateDelegate, OnUpdated = null, ...)

        /// <summary>
        /// Update the given news posting.
        /// </summary>
        /// <param name="NewsPostingId">An news posting identification.</param>
        /// <param name="UpdateDelegate">A delegate to update the given news posting.</param>
        /// <param name="OnUpdated">A delegate run whenever the news posting had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional news posting identification initiating this command/request.</param>
        protected internal async Task<NewsPosting> _UpdateNewsPosting(NewsPosting_Id                         NewsPostingId,
                                                             Action<NewsPosting.Builder>            UpdateDelegate,
                                                             Action<NewsPosting, EventTracking_Id>  OnUpdated         = null,
                                                             EventTracking_Id?                      EventTrackingId   = null,
                                                             User_Id?                               CurrentUserId     = null)
        {

            if (NewsPostingId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(NewsPostingId),
                                                "The given news posting identification must not be null or empty!");

            if (UpdateDelegate == null)
                throw new ArgumentNullException(nameof(UpdateDelegate),
                                                "The given update delegate must not be null!");

            if (!_NewsPostings.TryGetValue(NewsPostingId, out NewsPosting OldNewsPosting))
                throw new ArgumentException    ("The given news posting '" + NewsPostingId + "' does not exists in this API!",
                                                nameof(NewsPostingId));

            var Builder = OldNewsPosting.ToBuilder();
            UpdateDelegate(Builder);
            var NewNewsPosting = Builder.ToImmutable;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(updateNewsPosting_MessageType,
                                      NewNewsPosting.ToJSON(),
                                      eventTrackingId,
                                      CurrentUserId);

            _NewsPostings.Remove(OldNewsPosting.Id);
            NewNewsPosting.CopyAllLinkedDataFrom(OldNewsPosting);


            var OnNewsPostingUpdatedLocal = OnNewsPostingUpdated;
            if (OnNewsPostingUpdatedLocal is not null)
                await OnNewsPostingUpdatedLocal?.Invoke(Timestamp.Now,
                                                        NewNewsPosting,
                                                        OldNewsPosting,
                                                        eventTrackingId,
                                                        CurrentUserId);

            await SendNotifications(NewNewsPosting,
                                    updateNewsPosting_MessageType,
                                    OldNewsPosting,
                                    eventTrackingId,
                                    CurrentUserId);

            OnUpdated?.Invoke(NewNewsPosting,
                              eventTrackingId);

            return NewNewsPosting;

        }

        #endregion

        #region UpdateNewsPosting             (NewsPostingId, UpdateDelegate, OnUpdated = null, ...)

        /// <summary>
        /// Update the given news posting.
        /// </summary>
        /// <param name="NewsPostingId">An news posting identification.</param>
        /// <param name="UpdateDelegate">A delegate to update the given news posting.</param>
        /// <param name="OnUpdated">A delegate run whenever the news posting had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional news posting identification initiating this command/request.</param>
        public async Task<NewsPosting> UpdateNewsPosting(NewsPosting_Id                         NewsPostingId,
                                                         Action<NewsPosting.Builder>            UpdateDelegate,
                                                         Action<NewsPosting, EventTracking_Id>  OnUpdated         = null,
                                                         EventTracking_Id?                      EventTrackingId   = null,
                                                         User_Id?                               CurrentUserId     = null)
        {

            if (NewsPostingId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(NewsPostingId), "The given news posting identification must not be null or empty!");

            try
            {

                return (await NewsPostingsSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _UpdateNewsPosting(NewsPostingId,
                                                       UpdateDelegate,
                                                       OnUpdated,
                                                       EventTrackingId,
                                                       CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    NewsPostingsSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion


        #region NewsPostingExists(NewsPostingId)

        /// <summary>
        /// Determines whether the given news posting identification exists within this API.
        /// </summary>
        /// <param name="NewsPostingId">The unique identification of an news posting.</param>
        protected internal Boolean _NewsPostingExists(NewsPosting_Id NewsPostingId)

            => !NewsPostingId.IsNullOrEmpty && _NewsPostings.ContainsKey(NewsPostingId);


        /// <summary>
        /// Determines whether the given news posting identification exists within this API.
        /// </summary>
        /// <param name="NewsPostingId">The unique identification of an news posting.</param>
        public Boolean NewsPostingExists(NewsPosting_Id NewsPostingId)
        {

            try
            {

                if (NewsPostingsSemaphore.Wait(SemaphoreSlimTimeout) &&
                    _NewsPostingExists(NewsPostingId))
                {
                    return true;
                }

            }
            catch
            { }
            finally
            {
                try
                {
                    NewsPostingsSemaphore.Release();
                }
                catch
                { }
            }

            return false;

        }

        #endregion

        #region GetNewsPosting   (NewsPostingId)

        /// <summary>
        /// Get the news posting having the given unique identification.
        /// </summary>
        /// <param name="NewsPostingId">The unique identification of an news posting.</param>
        protected internal NewsPosting _GetNewsPosting(NewsPosting_Id NewsPostingId)
        {

            if (!NewsPostingId.IsNullOrEmpty && _NewsPostings.TryGetValue(NewsPostingId, out NewsPosting newsPosting))
                return newsPosting;

            return null;

        }


        /// <summary>
        /// Get the news posting having the given unique identification.
        /// </summary>
        /// <param name="NewsPostingId">The unique identification of the news posting.</param>
        public NewsPosting GetNewsPosting(NewsPosting_Id NewsPostingId)
        {

            try
            {

                if (NewsPostingsSemaphore.Wait(SemaphoreSlimTimeout))
                    return _GetNewsPosting(NewsPostingId);

            }
            catch
            { }
            finally
            {
                try
                {
                    NewsPostingsSemaphore.Release();
                }
                catch
                { }
            }

            return null;

        }

        #endregion

        #region TryGetNewsPosting(NewsPostingId, out NewsPosting)

        /// <summary>
        /// Try to get the news posting having the given unique identification.
        /// </summary>
        /// <param name="NewsPostingId">The unique identification of an news posting.</param>
        /// <param name="NewsPosting">The news posting.</param>
        protected internal Boolean _TryGetNewsPosting(NewsPosting_Id NewsPostingId, out NewsPosting NewsPosting)
        {

            if (!NewsPostingId.IsNullOrEmpty && _NewsPostings.TryGetValue(NewsPostingId, out NewsPosting newsPosting))
            {
                NewsPosting = newsPosting;
                return true;
            }

            NewsPosting = null;
            return false;

        }


        /// <summary>
        /// Try to get the news posting having the given unique identification.
        /// </summary>
        /// <param name="NewsPostingId">The unique identification of an news posting.</param>
        /// <param name="NewsPosting">The news posting.</param>
        public Boolean TryGetNewsPosting(NewsPosting_Id   NewsPostingId,
                                         out NewsPosting  NewsPosting)
        {

            try
            {

                if (NewsPostingsSemaphore.Wait(SemaphoreSlimTimeout) &&
                    _TryGetNewsPosting(NewsPostingId, out NewsPosting newsPosting))
                {
                    NewsPosting = newsPosting;
                    return true;
                }

            }
            catch
            { }
            finally
            {
                try
                {
                    NewsPostingsSemaphore.Release();
                }
                catch
                { }
            }

            NewsPosting = null;
            return false;

        }

        #endregion


        #region RemoveNewsPosting(NewsPosting, OnRemoved = null, ...)

        /// <summary>
        /// A delegate called whenever a news posting was removed.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the news posting was removed.</param>
        /// <param name="NewsPosting">The removed news posting.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">The invoking news posting identification</param>
        public delegate Task OnNewsPostingRemovedDelegate(DateTime          Timestamp,
                                                          NewsPosting       NewsPosting,
                                                          EventTracking_Id? EventTrackingId   = null,
                                                          User_Id?          CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a news posting was removed.
        /// </summary>
        public event OnNewsPostingRemovedDelegate OnNewsPostingRemoved;


        #region (class) DeleteNewsPostingResult

        public class DeleteNewsPostingResult
        {

            public Boolean     IsSuccess           { get; }

            public I18NString  ErrorDescription    { get; }


            private DeleteNewsPostingResult(Boolean     IsSuccess,
                                          I18NString  ErrorDescription  = null)
            {
                this.IsSuccess         = IsSuccess;
                this.ErrorDescription  = ErrorDescription;
            }


            public static DeleteNewsPostingResult Success

                => new DeleteNewsPostingResult(true);

            public static DeleteNewsPostingResult Failed(I18NString Reason)

                => new DeleteNewsPostingResult(false,
                                             Reason);

            public static DeleteNewsPostingResult Failed(Exception Exception)

                => new DeleteNewsPostingResult(false,
                                             I18NString.Create(Languages.en,
                                                               Exception.Message));

            public override String ToString()

                => IsSuccess
                       ? "Success"
                       : "Failed" + (ErrorDescription.IsNullOrEmpty()
                                         ? ": " + ErrorDescription.FirstText()
                                         : "!");

        }

        #endregion

        #region (protected internal virtual) CanDeleteNewsPosting(NewsPosting)

        /// <summary>
        /// Determines whether the news posting can safely be removed from the API.
        /// </summary>
        /// <param name="NewsPosting">The news posting to be removed.</param>
        protected internal virtual I18NString CanDeleteNewsPosting(NewsPosting NewsPosting)
        {
            return new I18NString(Languages.en, "Currently not possible!");
        }

        #endregion


        #region (protected internal) _RemoveNewsPosting(NewsPosting, OnRemoved = null, ...)

        /// <summary>
        /// Remove the given news posting from the API.
        /// </summary>
        /// <param name="NewsPosting">The news posting to be removed from this API.</param>
        /// <param name="OnRemoved">A delegate run whenever the news posting had been removed successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional news posting identification initiating this command/request.</param>
        protected internal async Task<DeleteNewsPostingResult> _RemoveNewsPosting(NewsPosting                            NewsPosting,
                                                                         Action<NewsPosting, EventTracking_Id>  OnRemoved         = null,
                                                                         EventTracking_Id?                      EventTrackingId   = null,
                                                                         User_Id?                               CurrentUserId     = null)
        {

            if (NewsPosting is null)
                throw new ArgumentNullException(nameof(NewsPosting),
                                                "The given news posting must not be null!");

            if (NewsPosting.API != this || !_NewsPostings.TryGetValue(NewsPosting.Id, out NewsPosting NewsPostingToBeRemoved))
                throw new ArgumentException    ("The given news posting '" + NewsPosting.Id + "' does not exists in this API!",
                                                nameof(NewsPosting));


            var result = CanDeleteNewsPosting(NewsPosting);

            if (result == null)
            {

                var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

                await WriteToDatabaseFile(removeNewsPosting_MessageType,
                                          NewsPosting.ToJSON(false),
                                          eventTrackingId,
                                          CurrentUserId);

                _NewsPostings.Remove(NewsPosting.Id);


                var OnNewsPostingRemovedLocal = OnNewsPostingRemoved;
                if (OnNewsPostingRemovedLocal is not null)
                    await OnNewsPostingRemovedLocal?.Invoke(Timestamp.Now,
                                                            NewsPosting,
                                                            eventTrackingId,
                                                            CurrentUserId);

                await SendNotifications(NewsPosting,
                                        removeNewsPosting_MessageType,
                                        null,
                                        eventTrackingId,
                                        CurrentUserId);

                OnRemoved?.Invoke(NewsPosting,
                                  eventTrackingId);

                return DeleteNewsPostingResult.Success;

            }
            else
                return DeleteNewsPostingResult.Failed(result);

        }

        #endregion

        #region RemoveNewsPosting             (NewsPosting, OnRemoved = null, ...)

        /// <summary>
        /// Remove the given news posting from the API.
        /// </summary>
        /// <param name="NewsPosting">The news posting to be removed from this API.</param>
        /// <param name="OnRemoved">A delegate run whenever the news posting had been removed successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional news posting identification initiating this command/request.</param>
        public async Task<DeleteNewsPostingResult> RemoveNewsPosting(NewsPosting                            NewsPosting,
                                                                     Action<NewsPosting, EventTracking_Id>  OnRemoved         = null,
                                                                     EventTracking_Id?                      EventTrackingId   = null,
                                                                     User_Id?                               CurrentUserId     = null)
        {

            if (NewsPosting is null)
                throw new ArgumentNullException(nameof(NewsPosting), "The given news posting must not be null!");

            try
            {

                return (await NewsPostingsSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _RemoveNewsPosting(NewsPosting,
                                                       OnRemoved,
                                                       EventTrackingId,
                                                       CurrentUserId)

                            : null;

            }
            catch (Exception e)
            {
                return DeleteNewsPostingResult.Failed(e);
            }
            finally
            {
                try
                {
                    NewsPostingsSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion

        #endregion

        #region NewsBanners

        #region Data

        /// <summary>
        /// An enumeration of all news banners.
        /// </summary>
        protected internal readonly Dictionary<NewsBanner_Id, NewsBanner> _NewsBanners;

        /// <summary>
        /// An enumeration of all news banners.
        /// </summary>
        public IEnumerable<NewsBanner> NewsBanners
        {
            get
            {
                try
                {
                    return NewsBannersSemaphore.Wait(SemaphoreSlimTimeout)
                               ? _NewsBanners.Values.ToArray()
                               : new NewsBanner[0];
                }
                finally
                {
                    try
                    {
                        NewsBannersSemaphore.Release();
                    }
                    catch
                    { }
                }
            }
        }

        #endregion


        #region (protected internal) WriteToDatabaseFileAndNotify(NewsBanner, MessageType,  OldNewsBanner = null, ...)

        /// <summary>
        /// Write the given news banner to the database and send out notifications.
        /// </summary>
        /// <param name="NewsBanner">The news banner.</param>
        /// <param name="MessageType">The user notification.</param>
        /// <param name="OldNewsBanner">The old/updated news banner.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task WriteToDatabaseFileAndNotify(NewsBanner              NewsBanner,
                                                          NotificationMessageType  MessageType,
                                                          NewsBanner              OldNewsBanner    = null,
                                                          EventTracking_Id?        EventTrackingId   = null,
                                                          User_Id?                 CurrentUserId     = null)
        {

            if (NewsBanner is null)
                throw new ArgumentNullException(nameof(NewsBanner),  "The given news banner must not be null or empty!");

            if (MessageType.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(MessageType),  "The given message type must not be null or empty!");


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(MessageType,
                                      NewsBanner.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            await SendNotifications(NewsBanner,
                                    MessageType,
                                    OldNewsBanner,
                                    eventTrackingId,
                                    CurrentUserId);

        }

        #endregion

        #region (protected internal) SendNotifications           (NewsBanner, MessageTypes, OldNewsBanner = null, ...)

        /// <summary>
        /// Send news banner notifications.
        /// </summary>
        /// <param name="NewsBanner">The news banner.</param>
        /// <param name="MessageType">The user notification.</param>
        /// <param name="OldNewsBanner">The old/updated news banner.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task SendNotifications(NewsBanner              NewsBanner,
                                               NotificationMessageType  MessageType,
                                               NewsBanner              OldNewsBanner    = null,
                                               EventTracking_Id?        EventTrackingId   = null,
                                               User_Id?                 CurrentUserId     = null)
        {

            if (NewsBanner is null)
                throw new ArgumentNullException(nameof(NewsBanner),  "The given news banner must not be null or empty!");

            if (MessageType.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(MessageType),  "The given message type must not be null or empty!");


            await SendNotifications(NewsBanner,
                                    new NotificationMessageType[] { MessageType },
                                    OldNewsBanner,
                                    EventTrackingId,
                                    CurrentUserId);

        }


        /// <summary>
        /// Send news banner notifications.
        /// </summary>
        /// <param name="NewsBanner">The news banner.</param>
        /// <param name="MessageTypes">The user notifications.</param>
        /// <param name="OldNewsBanner">The old/updated news banner.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task SendNotifications(NewsBanner                           NewsBanner,
                                               IEnumerable<NotificationMessageType>  MessageTypes,
                                               NewsBanner                           OldNewsBanner    = null,
                                               EventTracking_Id?                     EventTrackingId   = null,
                                               User_Id?                              CurrentUserId     = null)
        {

            if (NewsBanner is null)
                throw new ArgumentNullException(nameof(NewsBanner),   "The given news banner must not be null or empty!");

            var messageTypesHash = new HashSet<NotificationMessageType>(MessageTypes.Where(messageType => !messageType.IsNullOrEmpty));

            if (messageTypesHash.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(MessageTypes),  "The given enumeration of message types must not be null or empty!");

            if (messageTypesHash.Contains(addUserIfNotExists_MessageType))
                messageTypesHash.Add(addUser_MessageType);

            if (messageTypesHash.Contains(addOrUpdateUser_MessageType))
                messageTypesHash.Add(OldNewsBanner == null
                                       ? addUser_MessageType
                                       : updateUser_MessageType);

            var messageTypes = messageTypesHash.ToArray();


            if (!DisableNotifications)
            {


            }

        }

        #endregion

        #region (protected internal) GetNewsBannerSerializator(Request, User)

        protected internal NewsBannerToJSONDelegate GetNewsBannerSerializator(HTTPRequest  Request,
                                                                              IUser        User)
        {

            switch (User?.Id.ToString())
            {

                default:
                    return (newsBanner,
                            embedded,
                            ExpandTags,
                            ExpandAuthorId)

                            => newsBanner.ToJSON(embedded,
                                                  ExpandTags,
                                                  ExpandAuthorId);

            }

        }

        #endregion


        #region AddNewsBanner           (NewsBanner, OnAdded = null,                   CurrentUserId = null)

        /// <summary>
        /// A delegate called whenever a news banner was added.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the news banner was added.</param>
        /// <param name="NewsBanner">The added news banner.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public delegate Task OnNewsBannerAddedDelegate(DateTime          Timestamp,
                                                        NewsBanner       NewsBanner,
                                                        EventTracking_Id? EventTrackingId   = null,
                                                        User_Id?          CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a news banner was added.
        /// </summary>
        public event OnNewsBannerAddedDelegate OnNewsBannerAdded;


        #region (protected internal) _AddNewsBanner(NewsBanner,                                OnAdded = null, ...)

        /// <summary>
        /// Add the given news banner to the API.
        /// </summary>
        /// <param name="NewsBanner">A new news banner to be added to this API.</param>
        /// <param name="OnAdded">A delegate run whenever the news banner had been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task<NewsBanner> _AddNewsBanner(NewsBanner                            NewsBanner,
                                                          Action<NewsBanner, EventTracking_Id>  OnAdded           = null,
                                                          EventTracking_Id?                      EventTrackingId   = null,
                                                          User_Id?                               CurrentUserId     = null)
        {

            if (NewsBanner is null)
                throw new ArgumentNullException(nameof(NewsBanner),
                                                "The given news banner must not be null!");

            if (NewsBanner.API != null && NewsBanner.API != this)
                throw new ArgumentException    ("The given news banner is already attached to another API!",
                                                nameof(NewsBanner));

            if (_NewsBanners.ContainsKey(NewsBanner.Id))
                throw new ArgumentException    ("User group identification '" + NewsBanner.Id + "' already exists!",
                                                nameof(NewsBanner));

            if (NewsBanner.Id.Length < MinNewsBannerIdLength)
                throw new ArgumentException    ("User group identification '" + NewsBanner.Id + "' is too short!",
                                                nameof(NewsBanner));

            NewsBanner.API = this;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(addNewsBanner_MessageType,
                                      NewsBanner.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            _NewsBanners.Add(NewsBanner.Id, NewsBanner);


            var OnNewsBannerAddedLocal = OnNewsBannerAdded;
            if (OnNewsBannerAddedLocal is not null)
                await OnNewsBannerAddedLocal?.Invoke(Timestamp.Now,
                                                      NewsBanner,
                                                      eventTrackingId,
                                                      CurrentUserId);

            await SendNotifications(NewsBanner,
                                    addUser_MessageType,
                                    null,
                                    eventTrackingId,
                                    CurrentUserId);

            OnAdded?.Invoke(NewsBanner,
                            eventTrackingId);

            return NewsBanner;

        }

        #endregion

        #region AddNewsBanner             (NewsBanner,                                OnAdded = null, ...)

        /// <summary>
        /// Add the given news banner.
        /// </summary>
        /// <param name="NewsBanner">A new news banner.</param>
        /// <param name="OnAdded">A delegate run whenever the news banner had been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<NewsBanner> AddNewsBanner(NewsBanner                            NewsBanner,
                                                      Action<NewsBanner, EventTracking_Id>  OnAdded           = null,
                                                      EventTracking_Id?                      EventTrackingId   = null,
                                                      User_Id?                               CurrentUserId     = null)
        {

            if (NewsBanner is null)
                throw new ArgumentNullException(nameof(NewsBanner), "The given news banner must not be null!");

            try
            {

                return (await NewsBannersSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _AddNewsBanner(NewsBanner,
                                                    OnAdded,
                                                    EventTrackingId,
                                                    CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    NewsBannersSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion

        #region AddNewsBannerIfNotExists(NewsBanner, OnAdded = null,                   CurrentUserId = null)

        #region (protected internal) _AddNewsBannerIfNotExists(NewsBanner,                                OnAdded = null, ...)

        /// <summary>
        /// When it has not been created before, add the given news banner to the API.
        /// </summary>
        /// <param name="NewsBanner">A new news banner to be added to this API.</param>
        /// <param name="OnAdded">A delegate run whenever the news banner had been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task<NewsBanner> _AddNewsBannerIfNotExists(NewsBanner                            NewsBanner,
                                                                     Action<NewsBanner, EventTracking_Id>  OnAdded           = null,
                                                                     EventTracking_Id?                      EventTrackingId   = null,
                                                                     User_Id?                               CurrentUserId     = null)
        {

            if (NewsBanner is null)
                throw new ArgumentNullException(nameof(NewsBanner),
                                                "The given news banner must not be null!");

            if (NewsBanner.API != null && NewsBanner.API != this)
                throw new ArgumentException    ("The given news banner is already attached to another API!",
                                                nameof(NewsBanner));

            if (_NewsBanners.ContainsKey(NewsBanner.Id))
                return _NewsBanners[NewsBanner.Id];

            if (NewsBanner.Id.Length < MinNewsBannerIdLength)
                throw new ArgumentException    ("User group identification '" + NewsBanner.Id + "' is too short!",
                                                nameof(NewsBanner));

            NewsBanner.API = this;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(addNewsBannerIfNotExists_MessageType,
                                      NewsBanner.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            _NewsBanners.Add(NewsBanner.Id, NewsBanner);

            var OnNewsBannerAddedLocal = OnNewsBannerAdded;
            if (OnNewsBannerAddedLocal is not null)
                await OnNewsBannerAddedLocal?.Invoke(Timestamp.Now,
                                                      NewsBanner,
                                                      eventTrackingId,
                                                      CurrentUserId);

            await SendNotifications(NewsBanner,
                                    addNewsBannerIfNotExists_MessageType,
                                    null,
                                    eventTrackingId,
                                    CurrentUserId);

            OnAdded?.Invoke(NewsBanner,
                            eventTrackingId);

            return NewsBanner;

        }

        #endregion

        #region AddNewsBannerIfNotExists             (NewsBanner,                                OnAdded = null, ...)

        /// <summary>
        /// Add the given news banner.
        /// </summary>
        /// <param name="NewsBanner">A new news banner.</param>
        /// <param name="OnAdded">A delegate run whenever the news banner had been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<NewsBanner> AddNewsBannerIfNotExists(NewsBanner                            NewsBanner,
                                                                 Action<NewsBanner, EventTracking_Id>  OnAdded           = null,
                                                                 EventTracking_Id?                      EventTrackingId   = null,
                                                                 User_Id?                               CurrentUserId     = null)
        {

            try
            {

                return (await NewsBannersSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _AddNewsBannerIfNotExists(NewsBanner,
                                                             OnAdded,
                                                             EventTrackingId,
                                                             CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    NewsBannersSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion

        #region AddOrUpdateNewsBanner   (NewsBanner, OnAdded = null, OnUpdated = null, ...)

        #region (protected internal) _AddOrUpdateNewsBanner   (NewsBanner,   OnAdded = null, OnUpdated = null, ...)

        /// <summary>
        /// Add or update the given news banner to/within the API.
        /// </summary>
        /// <param name="NewsBanner">A news banner.</param>
        /// <param name="OnAdded">A delegate run whenever the news banner had been added successfully.</param>
        /// <param name="OnUpdated">A delegate run whenever the news banner had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional news banner identification initiating this command/request.</param>
        protected internal async Task<NewsBanner> _AddOrUpdateNewsBanner(NewsBanner                            NewsBanner,
                                                                  Action<NewsBanner, EventTracking_Id>  OnAdded           = null,
                                                                  Action<NewsBanner, EventTracking_Id>  OnUpdated         = null,
                                                                  EventTracking_Id?                      EventTrackingId   = null,
                                                                  User_Id?                               CurrentUserId     = null)
        {

            if (NewsBanner is null)
                throw new ArgumentNullException(nameof(NewsBanner),
                                                "The given news banner must not be null!");

            if (NewsBanner.API != null && NewsBanner.API != this)
                throw new ArgumentException    ("The given news banner is already attached to another API!",
                                                nameof(NewsBanner));

            if (_NewsBanners.ContainsKey(NewsBanner.Id))
                return _NewsBanners[NewsBanner.Id];

            if (NewsBanner.Id.Length < MinNewsBannerIdLength)
                throw new ArgumentException    ("NewsBanner identification '" + NewsBanner.Id + "' is too short!",
                                                nameof(NewsBanner));

            NewsBanner.API = this;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(addOrUpdateNewsBanner_MessageType,
                                      NewsBanner.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            if (_NewsBanners.TryGetValue(NewsBanner.Id, out NewsBanner OldNewsBanner))
            {
                _NewsBanners.Remove(OldNewsBanner.Id);
                NewsBanner.CopyAllLinkedDataFrom(OldNewsBanner);
            }

            _NewsBanners.Add(NewsBanner.Id, NewsBanner);

            if (OldNewsBanner != null)
            {

                var OnNewsBannerUpdatedLocal = OnNewsBannerUpdated;
                if (OnNewsBannerUpdatedLocal is not null)
                    await OnNewsBannerUpdatedLocal?.Invoke(Timestamp.Now,
                                                            NewsBanner,
                                                            OldNewsBanner,
                                                            eventTrackingId,
                                                            CurrentUserId);

                await SendNotifications(NewsBanner,
                                        updateNewsBanner_MessageType,
                                        OldNewsBanner,
                                        eventTrackingId,
                                        CurrentUserId);

                OnUpdated?.Invoke(NewsBanner,
                                  eventTrackingId);

            }
            else
            {

                var OnNewsBannerAddedLocal = OnNewsBannerAdded;
                if (OnNewsBannerAddedLocal is not null)
                    await OnNewsBannerAddedLocal?.Invoke(Timestamp.Now,
                                                          NewsBanner,
                                                          eventTrackingId,
                                                          CurrentUserId);

                await SendNotifications(NewsBanner,
                                        addNewsBanner_MessageType,
                                        null,
                                        eventTrackingId,
                                        CurrentUserId);

                OnAdded?.Invoke(NewsBanner,
                                eventTrackingId);

            }

            return NewsBanner;

        }

        #endregion

        #region AddOrUpdateNewsBanner   (NewsBanner,   OnAdded = null, OnUpdated = null, ...)

        /// <summary>
        /// Add or update the given news banner to/within the API.
        /// </summary>
        /// <param name="NewsBanner">A news banner.</param>
        /// <param name="OnAdded">A delegate run whenever the news banner had been added successfully.</param>
        /// <param name="OnUpdated">A delegate run whenever the news banner had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional news banner identification initiating this command/request.</param>
        public async Task<NewsBanner> AddOrUpdateNewsBanner(NewsBanner                            NewsBanner,
                                                              Action<NewsBanner, EventTracking_Id>  OnAdded           = null,
                                                              Action<NewsBanner, EventTracking_Id>  OnUpdated         = null,
                                                              EventTracking_Id?                      EventTrackingId   = null,
                                                              User_Id?                               CurrentUserId     = null)
        {

            if (NewsBanner is null)
                throw new ArgumentNullException(nameof(NewsBanner), "The given news banner must not be null!");

            try
            {

                return (await NewsBannersSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _AddOrUpdateNewsBanner(NewsBanner,
                                                            OnAdded,
                                                            OnUpdated,
                                                            EventTrackingId,
                                                            CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    NewsBannersSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion

        #region UpdateNewsBanner        (NewsBanner,                 OnUpdated = null, ...)

        /// <summary>
        /// A delegate called whenever a news banner was updated.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the news banner was updated.</param>
        /// <param name="NewsBanner">The updated news banner.</param>
        /// <param name="OldNewsBanner">The old news banner.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">The invoking news banner identification</param>
        public delegate Task OnNewsBannerUpdatedDelegate(DateTime          Timestamp,
                                                          NewsBanner       NewsBanner,
                                                          NewsBanner       OldNewsBanner,
                                                          EventTracking_Id? EventTrackingId   = null,
                                                          User_Id?          CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a news banner was updated.
        /// </summary>
        public event OnNewsBannerUpdatedDelegate OnNewsBannerUpdated;


        #region (protected internal) _UpdateNewsBanner(NewsBanner, OnUpdated = null, ...)

        /// <summary>
        /// Update the given news banner to/within the API.
        /// </summary>
        /// <param name="NewsBanner">A news banner.</param>
        /// <param name="OnUpdated">A delegate run whenever the news banner had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional news banner identification initiating this command/request.</param>
        protected internal async Task<NewsBanner> _UpdateNewsBanner(NewsBanner                            NewsBanner,
                                                             Action<NewsBanner, EventTracking_Id>  OnUpdated         = null,
                                                             EventTracking_Id?                      EventTrackingId   = null,
                                                             User_Id?                               CurrentUserId     = null)
        {

            if (NewsBanner is null)
                throw new ArgumentNullException(nameof(NewsBanner),
                                                "The given news banner must not be null!");

            if (NewsBanner.API != null && NewsBanner.API != this)
                throw new ArgumentException    ("The given news banner is already attached to another API!",
                                                nameof(NewsBanner));

            if (!_NewsBanners.TryGetValue(NewsBanner.Id, out NewsBanner OldNewsBanner))
                throw new ArgumentException    ("The given news banner '" + NewsBanner.Id + "' does not exists in this API!",
                                                nameof(NewsBanner));

            NewsBanner.API = this;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(updateNewsBanner_MessageType,
                                      NewsBanner.ToJSON(),
                                      eventTrackingId,
                                      CurrentUserId);

            _NewsBanners.Remove(OldNewsBanner.Id);
            NewsBanner.CopyAllLinkedDataFrom(OldNewsBanner);


            var OnNewsBannerUpdatedLocal = OnNewsBannerUpdated;
            if (OnNewsBannerUpdatedLocal is not null)
                await OnNewsBannerUpdatedLocal?.Invoke(Timestamp.Now,
                                                        NewsBanner,
                                                        OldNewsBanner,
                                                        eventTrackingId,
                                                        CurrentUserId);

            await SendNotifications(NewsBanner,
                                    updateNewsBanner_MessageType,
                                    OldNewsBanner,
                                    eventTrackingId,
                                    CurrentUserId);

            OnUpdated?.Invoke(NewsBanner,
                              eventTrackingId);

            return NewsBanner;

        }

        #endregion

        #region UpdateNewsBanner             (NewsBanner, OnUpdated = null, ...)

        /// <summary>
        /// Update the given news banner to/within the API.
        /// </summary>
        /// <param name="NewsBanner">A news banner.</param>
        /// <param name="OnUpdated">A delegate run whenever the news banner had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional news banner identification initiating this command/request.</param>
        public async Task<NewsBanner> UpdateNewsBanner(NewsBanner                            NewsBanner,
                                                         Action<NewsBanner, EventTracking_Id>  OnUpdated         = null,
                                                         EventTracking_Id?                      EventTrackingId   = null,
                                                         User_Id?                               CurrentUserId     = null)
        {

            if (NewsBanner is null)
                throw new ArgumentNullException(nameof(NewsBanner), "The given news banner must not be null!");

            try
            {

                return (await NewsBannersSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _UpdateNewsBanner(NewsBanner,
                                                       OnUpdated,
                                                       EventTrackingId,
                                                       CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    NewsBannersSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion


        #region (protected internal) _UpdateNewsBanner(NewsBannerId, UpdateDelegate, OnUpdated = null, ...)

        /// <summary>
        /// Update the given news banner.
        /// </summary>
        /// <param name="NewsBannerId">An news banner identification.</param>
        /// <param name="UpdateDelegate">A delegate to update the given news banner.</param>
        /// <param name="OnUpdated">A delegate run whenever the news banner had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional news banner identification initiating this command/request.</param>
        protected internal async Task<NewsBanner> _UpdateNewsBanner(NewsBanner_Id                         NewsBannerId,
                                                             Action<NewsBanner.Builder>            UpdateDelegate,
                                                             Action<NewsBanner, EventTracking_Id>  OnUpdated         = null,
                                                             EventTracking_Id?                      EventTrackingId   = null,
                                                             User_Id?                               CurrentUserId     = null)
        {

            if (NewsBannerId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(NewsBannerId),
                                                "The given news banner identification must not be null or empty!");

            if (UpdateDelegate == null)
                throw new ArgumentNullException(nameof(UpdateDelegate),
                                                "The given update delegate must not be null!");

            if (!_NewsBanners.TryGetValue(NewsBannerId, out NewsBanner OldNewsBanner))
                throw new ArgumentException    ("The given news banner '" + NewsBannerId + "' does not exists in this API!",
                                                nameof(NewsBannerId));

            var Builder = OldNewsBanner.ToBuilder();
            UpdateDelegate(Builder);
            var NewNewsBanner = Builder.ToImmutable;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(updateNewsBanner_MessageType,
                                      NewNewsBanner.ToJSON(),
                                      eventTrackingId,
                                      CurrentUserId);

            _NewsBanners.Remove(OldNewsBanner.Id);
            NewNewsBanner.CopyAllLinkedDataFrom(OldNewsBanner);


            var OnNewsBannerUpdatedLocal = OnNewsBannerUpdated;
            if (OnNewsBannerUpdatedLocal is not null)
                await OnNewsBannerUpdatedLocal?.Invoke(Timestamp.Now,
                                                        NewNewsBanner,
                                                        OldNewsBanner,
                                                        eventTrackingId,
                                                        CurrentUserId);

            await SendNotifications(NewNewsBanner,
                                    updateNewsBanner_MessageType,
                                    OldNewsBanner,
                                    eventTrackingId,
                                    CurrentUserId);

            OnUpdated?.Invoke(NewNewsBanner,
                              eventTrackingId);

            return NewNewsBanner;

        }

        #endregion

        #region UpdateNewsBanner             (NewsBannerId, UpdateDelegate, OnUpdated = null, ...)

        /// <summary>
        /// Update the given news banner.
        /// </summary>
        /// <param name="NewsBannerId">An news banner identification.</param>
        /// <param name="UpdateDelegate">A delegate to update the given news banner.</param>
        /// <param name="OnUpdated">A delegate run whenever the news banner had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional news banner identification initiating this command/request.</param>
        public async Task<NewsBanner> UpdateNewsBanner(NewsBanner_Id                         NewsBannerId,
                                                         Action<NewsBanner.Builder>            UpdateDelegate,
                                                         Action<NewsBanner, EventTracking_Id>  OnUpdated         = null,
                                                         EventTracking_Id?                      EventTrackingId   = null,
                                                         User_Id?                               CurrentUserId     = null)
        {

            if (NewsBannerId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(NewsBannerId), "The given news banner identification must not be null or empty!");

            try
            {

                return (await NewsBannersSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _UpdateNewsBanner(NewsBannerId,
                                                       UpdateDelegate,
                                                       OnUpdated,
                                                       EventTrackingId,
                                                       CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    NewsBannersSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion


        #region NewsBannerExists(NewsBannerId)

        /// <summary>
        /// Determines whether the given news banner identification exists within this API.
        /// </summary>
        /// <param name="NewsBannerId">The unique identification of an news banner.</param>
        protected internal Boolean _NewsBannerExists(NewsBanner_Id NewsBannerId)

            => !NewsBannerId.IsNullOrEmpty && _NewsBanners.ContainsKey(NewsBannerId);


        /// <summary>
        /// Determines whether the given news banner identification exists within this API.
        /// </summary>
        /// <param name="NewsBannerId">The unique identification of an news banner.</param>
        public Boolean NewsBannerExists(NewsBanner_Id NewsBannerId)
        {

            try
            {

                if (NewsBannersSemaphore.Wait(SemaphoreSlimTimeout) &&
                    _NewsBannerExists(NewsBannerId))
                {
                    return true;
                }

            }
            catch
            { }
            finally
            {
                try
                {
                    NewsBannersSemaphore.Release();
                }
                catch
                { }
            }

            return false;

        }

        #endregion

        #region GetNewsBanner   (NewsBannerId)

        /// <summary>
        /// Get the news banner having the given unique identification.
        /// </summary>
        /// <param name="NewsBannerId">The unique identification of an news banner.</param>
        protected internal NewsBanner _GetNewsBanner(NewsBanner_Id NewsBannerId)
        {

            if (!NewsBannerId.IsNullOrEmpty && _NewsBanners.TryGetValue(NewsBannerId, out NewsBanner newsBanner))
                return newsBanner;

            return null;

        }


        /// <summary>
        /// Get the news banner having the given unique identification.
        /// </summary>
        /// <param name="NewsBannerId">The unique identification of the news banner.</param>
        public NewsBanner GetNewsBanner(NewsBanner_Id NewsBannerId)
        {

            try
            {

                if (NewsBannersSemaphore.Wait(SemaphoreSlimTimeout))
                    return _GetNewsBanner(NewsBannerId);

            }
            catch
            { }
            finally
            {
                try
                {
                    NewsBannersSemaphore.Release();
                }
                catch
                { }
            }

            return null;

        }

        #endregion

        #region TryGetNewsBanner(NewsBannerId, out NewsBanner)

        /// <summary>
        /// Try to get the news banner having the given unique identification.
        /// </summary>
        /// <param name="NewsBannerId">The unique identification of an news banner.</param>
        /// <param name="NewsBanner">The news banner.</param>
        protected internal Boolean _TryGetNewsBanner(NewsBanner_Id NewsBannerId, out NewsBanner NewsBanner)
        {

            if (!NewsBannerId.IsNullOrEmpty && _NewsBanners.TryGetValue(NewsBannerId, out NewsBanner newsBanner))
            {
                NewsBanner = newsBanner;
                return true;
            }

            NewsBanner = null;
            return false;

        }


        /// <summary>
        /// Try to get the news banner having the given unique identification.
        /// </summary>
        /// <param name="NewsBannerId">The unique identification of an news banner.</param>
        /// <param name="NewsBanner">The news banner.</param>
        public Boolean TryGetNewsBanner(NewsBanner_Id   NewsBannerId,
                                         out NewsBanner  NewsBanner)
        {

            try
            {

                if (NewsBannersSemaphore.Wait(SemaphoreSlimTimeout) &&
                    _TryGetNewsBanner(NewsBannerId, out NewsBanner newsBanner))
                {
                    NewsBanner = newsBanner;
                    return true;
                }

            }
            catch
            { }
            finally
            {
                try
                {
                    NewsBannersSemaphore.Release();
                }
                catch
                { }
            }

            NewsBanner = null;
            return false;

        }

        #endregion


        #region RemoveNewsBanner(NewsBanner, OnRemoved = null, ...)

        /// <summary>
        /// A delegate called whenever a news banner was removed.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the news banner was removed.</param>
        /// <param name="NewsBanner">The removed news banner.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">The invoking news banner identification</param>
        public delegate Task OnNewsBannerRemovedDelegate(DateTime          Timestamp,
                                                          NewsBanner       NewsBanner,
                                                          EventTracking_Id? EventTrackingId   = null,
                                                          User_Id?          CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a news banner was removed.
        /// </summary>
        public event OnNewsBannerRemovedDelegate OnNewsBannerRemoved;


        #region (class) DeleteNewsBannerResult

        public class DeleteNewsBannerResult
        {

            public Boolean     IsSuccess           { get; }

            public I18NString  ErrorDescription    { get; }


            private DeleteNewsBannerResult(Boolean     IsSuccess,
                                          I18NString  ErrorDescription  = null)
            {
                this.IsSuccess         = IsSuccess;
                this.ErrorDescription  = ErrorDescription;
            }


            public static DeleteNewsBannerResult Success

                => new DeleteNewsBannerResult(true);

            public static DeleteNewsBannerResult Failed(I18NString Reason)

                => new DeleteNewsBannerResult(false,
                                             Reason);

            public static DeleteNewsBannerResult Failed(Exception Exception)

                => new DeleteNewsBannerResult(false,
                                             I18NString.Create(Languages.en,
                                                               Exception.Message));

            public override String ToString()

                => IsSuccess
                       ? "Success"
                       : "Failed" + (ErrorDescription.IsNullOrEmpty()
                                         ? ": " + ErrorDescription.FirstText()
                                         : "!");

        }

        #endregion

        #region (protected internal virtual) CanDeleteNewsBanner(NewsBanner)

        /// <summary>
        /// Determines whether the news banner can safely be removed from the API.
        /// </summary>
        /// <param name="NewsBanner">The news banner to be removed.</param>
        protected internal virtual I18NString CanDeleteNewsBanner(NewsBanner NewsBanner)
        {
            return new I18NString(Languages.en, "Currently not possible!");
        }

        #endregion


        #region (protected internal) _RemoveNewsBanner(NewsBanner, OnRemoved = null, ...)

        /// <summary>
        /// Remove the given news banner from the API.
        /// </summary>
        /// <param name="NewsBanner">The news banner to be removed from this API.</param>
        /// <param name="OnRemoved">A delegate run whenever the news banner had been removed successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional news banner identification initiating this command/request.</param>
        protected internal async Task<DeleteNewsBannerResult> _RemoveNewsBanner(NewsBanner                            NewsBanner,
                                                                         Action<NewsBanner, EventTracking_Id>  OnRemoved         = null,
                                                                         EventTracking_Id?                      EventTrackingId   = null,
                                                                         User_Id?                               CurrentUserId     = null)
        {

            if (NewsBanner is null)
                throw new ArgumentNullException(nameof(NewsBanner),
                                                "The given news banner must not be null!");

            if (NewsBanner.API != this || !_NewsBanners.TryGetValue(NewsBanner.Id, out NewsBanner NewsBannerToBeRemoved))
                throw new ArgumentException    ("The given news banner '" + NewsBanner.Id + "' does not exists in this API!",
                                                nameof(NewsBanner));


            var result = CanDeleteNewsBanner(NewsBanner);

            if (result == null)
            {

                var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

                await WriteToDatabaseFile(removeNewsBanner_MessageType,
                                          NewsBanner.ToJSON(false),
                                          eventTrackingId,
                                          CurrentUserId);

                _NewsBanners.Remove(NewsBanner.Id);


                var OnNewsBannerRemovedLocal = OnNewsBannerRemoved;
                if (OnNewsBannerRemovedLocal is not null)
                    await OnNewsBannerRemovedLocal?.Invoke(Timestamp.Now,
                                                            NewsBanner,
                                                            eventTrackingId,
                                                            CurrentUserId);

                await SendNotifications(NewsBanner,
                                        removeNewsBanner_MessageType,
                                        null,
                                        eventTrackingId,
                                        CurrentUserId);

                OnRemoved?.Invoke(NewsBanner,
                                  eventTrackingId);

                return DeleteNewsBannerResult.Success;

            }
            else
                return DeleteNewsBannerResult.Failed(result);

        }

        #endregion

        #region RemoveNewsBanner             (NewsBanner, OnRemoved = null, ...)

        /// <summary>
        /// Remove the given news banner from the API.
        /// </summary>
        /// <param name="NewsBanner">The news banner to be removed from this API.</param>
        /// <param name="OnRemoved">A delegate run whenever the news banner had been removed successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional news banner identification initiating this command/request.</param>
        public async Task<DeleteNewsBannerResult> RemoveNewsBanner(NewsBanner                            NewsBanner,
                                                                     Action<NewsBanner, EventTracking_Id>  OnRemoved         = null,
                                                                     EventTracking_Id?                      EventTrackingId   = null,
                                                                     User_Id?                               CurrentUserId     = null)
        {

            if (NewsBanner is null)
                throw new ArgumentNullException(nameof(NewsBanner), "The given news banner must not be null!");

            try
            {

                return (await NewsBannersSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _RemoveNewsBanner(NewsBanner,
                                                       OnRemoved,
                                                       EventTrackingId,
                                                       CurrentUserId)

                            : null;

            }
            catch (Exception e)
            {
                return DeleteNewsBannerResult.Failed(e);
            }
            finally
            {
                try
                {
                    NewsBannersSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion

        #endregion

        #region FAQs

        #region Data

        /// <summary>
        /// An enumeration of all FAQs.
        /// </summary>
        protected internal readonly Dictionary<FAQ_Id, FAQ> _FAQs;

        /// <summary>
        /// An enumeration of all FAQs.
        /// </summary>
        public IEnumerable<FAQ> FAQs
        {
            get
            {
                try
                {
                    return FAQsSemaphore.Wait(SemaphoreSlimTimeout)
                               ? _FAQs.Values.ToArray()
                               : new FAQ[0];
                }
                finally
                {
                    try
                    {
                        FAQsSemaphore.Release();
                    }
                    catch
                    { }
                }
            }
        }

        #endregion


        #region (protected internal) WriteToDatabaseFileAndNotify(FAQ, MessageType,  OldFAQ = null, ...)

        /// <summary>
        /// Write the given FAQ to the database and send out notifications.
        /// </summary>
        /// <param name="FAQ">The FAQ.</param>
        /// <param name="MessageType">The user notification.</param>
        /// <param name="OldFAQ">The old/updated FAQ.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task WriteToDatabaseFileAndNotify(FAQ              FAQ,
                                                          NotificationMessageType  MessageType,
                                                          FAQ              OldFAQ    = null,
                                                          EventTracking_Id?        EventTrackingId   = null,
                                                          User_Id?                 CurrentUserId     = null)
        {

            if (FAQ is null)
                throw new ArgumentNullException(nameof(FAQ),  "The given FAQ must not be null or empty!");

            if (MessageType.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(MessageType),  "The given message type must not be null or empty!");


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(MessageType,
                                      FAQ.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            await SendNotifications(FAQ,
                                    MessageType,
                                    OldFAQ,
                                    eventTrackingId,
                                    CurrentUserId);

        }

        #endregion

        #region (protected internal) SendNotifications           (FAQ, MessageTypes, OldFAQ = null, ...)

        /// <summary>
        /// Send FAQ notifications.
        /// </summary>
        /// <param name="FAQ">The FAQ.</param>
        /// <param name="MessageType">The user notification.</param>
        /// <param name="OldFAQ">The old/updated FAQ.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task SendNotifications(FAQ              FAQ,
                                               NotificationMessageType  MessageType,
                                               FAQ              OldFAQ    = null,
                                               EventTracking_Id?        EventTrackingId   = null,
                                               User_Id?                 CurrentUserId     = null)
        {

            if (FAQ is null)
                throw new ArgumentNullException(nameof(FAQ),  "The given FAQ must not be null or empty!");

            if (MessageType.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(MessageType),  "The given message type must not be null or empty!");


            await SendNotifications(FAQ,
                                    new NotificationMessageType[] { MessageType },
                                    OldFAQ,
                                    EventTrackingId,
                                    CurrentUserId);

        }


        /// <summary>
        /// Send FAQ notifications.
        /// </summary>
        /// <param name="FAQ">The FAQ.</param>
        /// <param name="MessageTypes">The user notifications.</param>
        /// <param name="OldFAQ">The old/updated FAQ.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task SendNotifications(FAQ                           FAQ,
                                               IEnumerable<NotificationMessageType>  MessageTypes,
                                               FAQ                           OldFAQ    = null,
                                               EventTracking_Id?                     EventTrackingId   = null,
                                               User_Id?                              CurrentUserId     = null)
        {

            if (FAQ is null)
                throw new ArgumentNullException(nameof(FAQ),   "The given FAQ must not be null or empty!");

            var messageTypesHash = new HashSet<NotificationMessageType>(MessageTypes.Where(messageType => !messageType.IsNullOrEmpty));

            if (messageTypesHash.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(MessageTypes),  "The given enumeration of message types must not be null or empty!");

            if (messageTypesHash.Contains(addUserIfNotExists_MessageType))
                messageTypesHash.Add(addUser_MessageType);

            if (messageTypesHash.Contains(addOrUpdateUser_MessageType))
                messageTypesHash.Add(OldFAQ == null
                                       ? addUser_MessageType
                                       : updateUser_MessageType);

            var messageTypes = messageTypesHash.ToArray();


            if (!DisableNotifications)
            {


            }

        }

        #endregion

        #region (protected internal) GetFAQSerializator(Request, User)

        protected internal FAQToJSONDelegate GetFAQSerializator(HTTPRequest  Request,
                                                                IUser        User)
        {

            switch (User?.Id.ToString())
            {

                default:
                    return (faq,
                            embedded,
                            ExpandTags,
                            ExpandAuthorId)

                            => faq.ToJSON(embedded,
                                          ExpandTags,
                                          ExpandAuthorId);

            }

        }

        #endregion


        #region AddFAQ           (FAQ, OnAdded = null,                   CurrentUserId = null)

        /// <summary>
        /// A delegate called whenever a FAQ was added.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the FAQ was added.</param>
        /// <param name="FAQ">The added FAQ.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public delegate Task OnFAQAddedDelegate(DateTime          Timestamp,
                                                        FAQ       FAQ,
                                                        EventTracking_Id? EventTrackingId   = null,
                                                        User_Id?          CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a FAQ was added.
        /// </summary>
        public event OnFAQAddedDelegate OnFAQAdded;


        #region (protected internal) _AddFAQ(FAQ,                                OnAdded = null, ...)

        /// <summary>
        /// Add the given FAQ to the API.
        /// </summary>
        /// <param name="FAQ">A new FAQ to be added to this API.</param>
        /// <param name="OnAdded">A delegate run whenever the FAQ had been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task<FAQ> _AddFAQ(FAQ                            FAQ,
                                                          Action<FAQ, EventTracking_Id>  OnAdded           = null,
                                                          EventTracking_Id?                      EventTrackingId   = null,
                                                          User_Id?                               CurrentUserId     = null)
        {

            if (FAQ is null)
                throw new ArgumentNullException(nameof(FAQ),
                                                "The given FAQ must not be null!");

            if (FAQ.API != null && FAQ.API != this)
                throw new ArgumentException    ("The given FAQ is already attached to another API!",
                                                nameof(FAQ));

            if (_FAQs.ContainsKey(FAQ.Id))
                throw new ArgumentException    ("User group identification '" + FAQ.Id + "' already exists!",
                                                nameof(FAQ));

            if (FAQ.Id.Length < MinFAQIdLength)
                throw new ArgumentException    ("User group identification '" + FAQ.Id + "' is too short!",
                                                nameof(FAQ));

            FAQ.API = this;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(addFAQ_MessageType,
                                      FAQ.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            _FAQs.Add(FAQ.Id, FAQ);


            var OnFAQAddedLocal = OnFAQAdded;
            if (OnFAQAddedLocal is not null)
                await OnFAQAddedLocal?.Invoke(Timestamp.Now,
                                                      FAQ,
                                                      eventTrackingId,
                                                      CurrentUserId);

            await SendNotifications(FAQ,
                                    addUser_MessageType,
                                    null,
                                    eventTrackingId,
                                    CurrentUserId);

            OnAdded?.Invoke(FAQ,
                            eventTrackingId);

            return FAQ;

        }

        #endregion

        #region AddFAQ             (FAQ,                                OnAdded = null, ...)

        /// <summary>
        /// Add the given FAQ.
        /// </summary>
        /// <param name="FAQ">A new FAQ.</param>
        /// <param name="OnAdded">A delegate run whenever the FAQ had been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<FAQ> AddFAQ(FAQ                            FAQ,
                                                      Action<FAQ, EventTracking_Id>  OnAdded           = null,
                                                      EventTracking_Id?                      EventTrackingId   = null,
                                                      User_Id?                               CurrentUserId     = null)
        {

            if (FAQ is null)
                throw new ArgumentNullException(nameof(FAQ), "The given FAQ must not be null!");

            try
            {

                return (await FAQsSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _AddFAQ(FAQ,
                                                    OnAdded,
                                                    EventTrackingId,
                                                    CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    FAQsSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion

        #region AddFAQIfNotExists(FAQ, OnAdded = null,                   CurrentUserId = null)

        #region (protected internal) _AddFAQIfNotExists(FAQ,                                OnAdded = null, ...)

        /// <summary>
        /// When it has not been created before, add the given FAQ to the API.
        /// </summary>
        /// <param name="FAQ">A new FAQ to be added to this API.</param>
        /// <param name="OnAdded">A delegate run whenever the FAQ had been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        protected internal async Task<FAQ> _AddFAQIfNotExists(FAQ                            FAQ,
                                                                     Action<FAQ, EventTracking_Id>  OnAdded           = null,
                                                                     EventTracking_Id?                      EventTrackingId   = null,
                                                                     User_Id?                               CurrentUserId     = null)
        {

            if (FAQ is null)
                throw new ArgumentNullException(nameof(FAQ),
                                                "The given FAQ must not be null!");

            if (FAQ.API != null && FAQ.API != this)
                throw new ArgumentException    ("The given FAQ is already attached to another API!",
                                                nameof(FAQ));

            if (_FAQs.ContainsKey(FAQ.Id))
                return _FAQs[FAQ.Id];

            if (FAQ.Id.Length < MinFAQIdLength)
                throw new ArgumentException    ("User group identification '" + FAQ.Id + "' is too short!",
                                                nameof(FAQ));

            FAQ.API = this;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(addFAQIfNotExists_MessageType,
                                      FAQ.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            _FAQs.Add(FAQ.Id, FAQ);

            var OnFAQAddedLocal = OnFAQAdded;
            if (OnFAQAddedLocal is not null)
                await OnFAQAddedLocal?.Invoke(Timestamp.Now,
                                                      FAQ,
                                                      eventTrackingId,
                                                      CurrentUserId);

            await SendNotifications(FAQ,
                                    addFAQIfNotExists_MessageType,
                                    null,
                                    eventTrackingId,
                                    CurrentUserId);

            OnAdded?.Invoke(FAQ,
                            eventTrackingId);

            return FAQ;

        }

        #endregion

        #region AddFAQIfNotExists             (FAQ,                                OnAdded = null, ...)

        /// <summary>
        /// Add the given FAQ.
        /// </summary>
        /// <param name="FAQ">A new FAQ.</param>
        /// <param name="OnAdded">A delegate run whenever the FAQ had been added successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional user identification initiating this command/request.</param>
        public async Task<FAQ> AddFAQIfNotExists(FAQ                            FAQ,
                                                                 Action<FAQ, EventTracking_Id>  OnAdded           = null,
                                                                 EventTracking_Id?                      EventTrackingId   = null,
                                                                 User_Id?                               CurrentUserId     = null)
        {

            try
            {

                return (await FAQsSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _AddFAQIfNotExists(FAQ,
                                                             OnAdded,
                                                             EventTrackingId,
                                                             CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    FAQsSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion

        #region AddOrUpdateFAQ   (FAQ, OnAdded = null, OnUpdated = null, ...)

        #region (protected internal) _AddOrUpdateFAQ   (FAQ,   OnAdded = null, OnUpdated = null, ...)

        /// <summary>
        /// Add or update the given FAQ to/within the API.
        /// </summary>
        /// <param name="FAQ">A FAQ.</param>
        /// <param name="OnAdded">A delegate run whenever the FAQ had been added successfully.</param>
        /// <param name="OnUpdated">A delegate run whenever the FAQ had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional FAQ identification initiating this command/request.</param>
        protected internal async Task<FAQ> _AddOrUpdateFAQ(FAQ                            FAQ,
                                                                  Action<FAQ, EventTracking_Id>  OnAdded           = null,
                                                                  Action<FAQ, EventTracking_Id>  OnUpdated         = null,
                                                                  EventTracking_Id?                      EventTrackingId   = null,
                                                                  User_Id?                               CurrentUserId     = null)
        {

            if (FAQ is null)
                throw new ArgumentNullException(nameof(FAQ),
                                                "The given FAQ must not be null!");

            if (FAQ.API != null && FAQ.API != this)
                throw new ArgumentException    ("The given FAQ is already attached to another API!",
                                                nameof(FAQ));

            if (_FAQs.ContainsKey(FAQ.Id))
                return _FAQs[FAQ.Id];

            if (FAQ.Id.Length < MinFAQIdLength)
                throw new ArgumentException    ("FAQ identification '" + FAQ.Id + "' is too short!",
                                                nameof(FAQ));

            FAQ.API = this;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(addOrUpdateFAQ_MessageType,
                                      FAQ.ToJSON(false),
                                      eventTrackingId,
                                      CurrentUserId);

            if (_FAQs.TryGetValue(FAQ.Id, out FAQ OldFAQ))
            {
                _FAQs.Remove(OldFAQ.Id);
                FAQ.CopyAllLinkedDataFrom(OldFAQ);
            }

            _FAQs.Add(FAQ.Id, FAQ);

            if (OldFAQ != null)
            {

                var OnFAQUpdatedLocal = OnFAQUpdated;
                if (OnFAQUpdatedLocal is not null)
                    await OnFAQUpdatedLocal?.Invoke(Timestamp.Now,
                                                            FAQ,
                                                            OldFAQ,
                                                            eventTrackingId,
                                                            CurrentUserId);

                await SendNotifications(FAQ,
                                        updateFAQ_MessageType,
                                        OldFAQ,
                                        eventTrackingId,
                                        CurrentUserId);

                OnUpdated?.Invoke(FAQ,
                                  eventTrackingId);

            }
            else
            {

                var OnFAQAddedLocal = OnFAQAdded;
                if (OnFAQAddedLocal is not null)
                    await OnFAQAddedLocal?.Invoke(Timestamp.Now,
                                                          FAQ,
                                                          eventTrackingId,
                                                          CurrentUserId);

                await SendNotifications(FAQ,
                                        addFAQ_MessageType,
                                        null,
                                        eventTrackingId,
                                        CurrentUserId);

                OnAdded?.Invoke(FAQ,
                                eventTrackingId);

            }

            return FAQ;

        }

        #endregion

        #region AddOrUpdateFAQ   (FAQ,   OnAdded = null, OnUpdated = null, ...)

        /// <summary>
        /// Add or update the given FAQ to/within the API.
        /// </summary>
        /// <param name="FAQ">A FAQ.</param>
        /// <param name="OnAdded">A delegate run whenever the FAQ had been added successfully.</param>
        /// <param name="OnUpdated">A delegate run whenever the FAQ had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional FAQ identification initiating this command/request.</param>
        public async Task<FAQ> AddOrUpdateFAQ(FAQ                            FAQ,
                                                              Action<FAQ, EventTracking_Id>  OnAdded           = null,
                                                              Action<FAQ, EventTracking_Id>  OnUpdated         = null,
                                                              EventTracking_Id?                      EventTrackingId   = null,
                                                              User_Id?                               CurrentUserId     = null)
        {

            if (FAQ is null)
                throw new ArgumentNullException(nameof(FAQ), "The given FAQ must not be null!");

            try
            {

                return (await FAQsSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _AddOrUpdateFAQ(FAQ,
                                                            OnAdded,
                                                            OnUpdated,
                                                            EventTrackingId,
                                                            CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    FAQsSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion

        #region UpdateFAQ        (FAQ,                 OnUpdated = null, ...)

        /// <summary>
        /// A delegate called whenever a FAQ was updated.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the FAQ was updated.</param>
        /// <param name="FAQ">The updated FAQ.</param>
        /// <param name="OldFAQ">The old FAQ.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">The invoking FAQ identification</param>
        public delegate Task OnFAQUpdatedDelegate(DateTime          Timestamp,
                                                          FAQ       FAQ,
                                                          FAQ       OldFAQ,
                                                          EventTracking_Id? EventTrackingId   = null,
                                                          User_Id?          CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a FAQ was updated.
        /// </summary>
        public event OnFAQUpdatedDelegate OnFAQUpdated;


        #region (protected internal) _UpdateFAQ(FAQ, OnUpdated = null, ...)

        /// <summary>
        /// Update the given FAQ to/within the API.
        /// </summary>
        /// <param name="FAQ">A FAQ.</param>
        /// <param name="OnUpdated">A delegate run whenever the FAQ had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional FAQ identification initiating this command/request.</param>
        protected internal async Task<FAQ> _UpdateFAQ(FAQ                            FAQ,
                                                             Action<FAQ, EventTracking_Id>  OnUpdated         = null,
                                                             EventTracking_Id?                      EventTrackingId   = null,
                                                             User_Id?                               CurrentUserId     = null)
        {

            if (FAQ is null)
                throw new ArgumentNullException(nameof(FAQ),
                                                "The given FAQ must not be null!");

            if (FAQ.API != null && FAQ.API != this)
                throw new ArgumentException    ("The given FAQ is already attached to another API!",
                                                nameof(FAQ));

            if (!_FAQs.TryGetValue(FAQ.Id, out FAQ OldFAQ))
                throw new ArgumentException    ("The given FAQ '" + FAQ.Id + "' does not exists in this API!",
                                                nameof(FAQ));

            FAQ.API = this;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(updateFAQ_MessageType,
                                      FAQ.ToJSON(),
                                      eventTrackingId,
                                      CurrentUserId);

            _FAQs.Remove(OldFAQ.Id);
            FAQ.CopyAllLinkedDataFrom(OldFAQ);


            var OnFAQUpdatedLocal = OnFAQUpdated;
            if (OnFAQUpdatedLocal is not null)
                await OnFAQUpdatedLocal?.Invoke(Timestamp.Now,
                                                        FAQ,
                                                        OldFAQ,
                                                        eventTrackingId,
                                                        CurrentUserId);

            await SendNotifications(FAQ,
                                    updateFAQ_MessageType,
                                    OldFAQ,
                                    eventTrackingId,
                                    CurrentUserId);

            OnUpdated?.Invoke(FAQ,
                              eventTrackingId);

            return FAQ;

        }

        #endregion

        #region UpdateFAQ             (FAQ, OnUpdated = null, ...)

        /// <summary>
        /// Update the given FAQ to/within the API.
        /// </summary>
        /// <param name="FAQ">A FAQ.</param>
        /// <param name="OnUpdated">A delegate run whenever the FAQ had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional FAQ identification initiating this command/request.</param>
        public async Task<FAQ> UpdateFAQ(FAQ                            FAQ,
                                                         Action<FAQ, EventTracking_Id>  OnUpdated         = null,
                                                         EventTracking_Id?                      EventTrackingId   = null,
                                                         User_Id?                               CurrentUserId     = null)
        {

            if (FAQ is null)
                throw new ArgumentNullException(nameof(FAQ), "The given FAQ must not be null!");

            try
            {

                return (await FAQsSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _UpdateFAQ(FAQ,
                                                       OnUpdated,
                                                       EventTrackingId,
                                                       CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    FAQsSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion


        #region (protected internal) _UpdateFAQ(FAQId, UpdateDelegate, OnUpdated = null, ...)

        /// <summary>
        /// Update the given FAQ.
        /// </summary>
        /// <param name="FAQId">An FAQ identification.</param>
        /// <param name="UpdateDelegate">A delegate to update the given FAQ.</param>
        /// <param name="OnUpdated">A delegate run whenever the FAQ had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional FAQ identification initiating this command/request.</param>
        protected internal async Task<FAQ> _UpdateFAQ(FAQ_Id                         FAQId,
                                                             Action<FAQ.Builder>            UpdateDelegate,
                                                             Action<FAQ, EventTracking_Id>  OnUpdated         = null,
                                                             EventTracking_Id?                      EventTrackingId   = null,
                                                             User_Id?                               CurrentUserId     = null)
        {

            if (FAQId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(FAQId),
                                                "The given FAQ identification must not be null or empty!");

            if (UpdateDelegate == null)
                throw new ArgumentNullException(nameof(UpdateDelegate),
                                                "The given update delegate must not be null!");

            if (!_FAQs.TryGetValue(FAQId, out FAQ OldFAQ))
                throw new ArgumentException    ("The given FAQ '" + FAQId + "' does not exists in this API!",
                                                nameof(FAQId));

            var Builder = OldFAQ.ToBuilder();
            UpdateDelegate(Builder);
            var NewFAQ = Builder.ToImmutable;


            var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

            await WriteToDatabaseFile(updateFAQ_MessageType,
                                      NewFAQ.ToJSON(),
                                      eventTrackingId,
                                      CurrentUserId);

            _FAQs.Remove(OldFAQ.Id);
            NewFAQ.CopyAllLinkedDataFrom(OldFAQ);


            var OnFAQUpdatedLocal = OnFAQUpdated;
            if (OnFAQUpdatedLocal is not null)
                await OnFAQUpdatedLocal?.Invoke(Timestamp.Now,
                                                        NewFAQ,
                                                        OldFAQ,
                                                        eventTrackingId,
                                                        CurrentUserId);

            await SendNotifications(NewFAQ,
                                    updateFAQ_MessageType,
                                    OldFAQ,
                                    eventTrackingId,
                                    CurrentUserId);

            OnUpdated?.Invoke(NewFAQ,
                              eventTrackingId);

            return NewFAQ;

        }

        #endregion

        #region UpdateFAQ             (FAQId, UpdateDelegate, OnUpdated = null, ...)

        /// <summary>
        /// Update the given FAQ.
        /// </summary>
        /// <param name="FAQId">An FAQ identification.</param>
        /// <param name="UpdateDelegate">A delegate to update the given FAQ.</param>
        /// <param name="OnUpdated">A delegate run whenever the FAQ had been updated successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional FAQ identification initiating this command/request.</param>
        public async Task<FAQ> UpdateFAQ(FAQ_Id                         FAQId,
                                                         Action<FAQ.Builder>            UpdateDelegate,
                                                         Action<FAQ, EventTracking_Id>  OnUpdated         = null,
                                                         EventTracking_Id?                      EventTrackingId   = null,
                                                         User_Id?                               CurrentUserId     = null)
        {

            if (FAQId.IsNullOrEmpty)
                throw new ArgumentNullException(nameof(FAQId), "The given FAQ identification must not be null or empty!");

            try
            {

                return (await FAQsSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _UpdateFAQ(FAQId,
                                                       UpdateDelegate,
                                                       OnUpdated,
                                                       EventTrackingId,
                                                       CurrentUserId)

                            : null;

            }
            finally
            {
                try
                {
                    FAQsSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion


        #region FAQExists(FAQId)

        /// <summary>
        /// Determines whether the given FAQ identification exists within this API.
        /// </summary>
        /// <param name="FAQId">The unique identification of an FAQ.</param>
        protected internal Boolean _FAQExists(FAQ_Id FAQId)

            => !FAQId.IsNullOrEmpty && _FAQs.ContainsKey(FAQId);


        /// <summary>
        /// Determines whether the given FAQ identification exists within this API.
        /// </summary>
        /// <param name="FAQId">The unique identification of an FAQ.</param>
        public Boolean FAQExists(FAQ_Id FAQId)
        {

            try
            {

                if (FAQsSemaphore.Wait(SemaphoreSlimTimeout) &&
                    _FAQExists(FAQId))
                {
                    return true;
                }

            }
            catch
            { }
            finally
            {
                try
                {
                    FAQsSemaphore.Release();
                }
                catch
                { }
            }

            return false;

        }

        #endregion

        #region GetFAQ   (FAQId)

        /// <summary>
        /// Get the FAQ having the given unique identification.
        /// </summary>
        /// <param name="FAQId">The unique identification of an FAQ.</param>
        protected internal FAQ _GetFAQ(FAQ_Id FAQId)
        {

            if (!FAQId.IsNullOrEmpty && _FAQs.TryGetValue(FAQId, out FAQ faq))
                return faq;

            return null;

        }


        /// <summary>
        /// Get the FAQ having the given unique identification.
        /// </summary>
        /// <param name="FAQId">The unique identification of the FAQ.</param>
        public FAQ GetFAQ(FAQ_Id FAQId)
        {

            try
            {

                if (FAQsSemaphore.Wait(SemaphoreSlimTimeout))
                    return _GetFAQ(FAQId);

            }
            catch
            { }
            finally
            {
                try
                {
                    FAQsSemaphore.Release();
                }
                catch
                { }
            }

            return null;

        }

        #endregion

        #region TryGetFAQ(FAQId, out FAQ)

        /// <summary>
        /// Try to get the FAQ having the given unique identification.
        /// </summary>
        /// <param name="FAQId">The unique identification of an FAQ.</param>
        /// <param name="FAQ">The FAQ.</param>
        protected internal Boolean _TryGetFAQ(FAQ_Id FAQId, out FAQ FAQ)
        {

            if (!FAQId.IsNullOrEmpty && _FAQs.TryGetValue(FAQId, out FAQ faq))
            {
                FAQ = faq;
                return true;
            }

            FAQ = null;
            return false;

        }


        /// <summary>
        /// Try to get the FAQ having the given unique identification.
        /// </summary>
        /// <param name="FAQId">The unique identification of an FAQ.</param>
        /// <param name="FAQ">The FAQ.</param>
        public Boolean TryGetFAQ(FAQ_Id   FAQId,
                                         out FAQ  FAQ)
        {

            try
            {

                if (FAQsSemaphore.Wait(SemaphoreSlimTimeout) &&
                    _TryGetFAQ(FAQId, out FAQ faq))
                {
                    FAQ = faq;
                    return true;
                }

            }
            catch
            { }
            finally
            {
                try
                {
                    FAQsSemaphore.Release();
                }
                catch
                { }
            }

            FAQ = null;
            return false;

        }

        #endregion


        #region RemoveFAQ(FAQ, OnRemoved = null, ...)

        /// <summary>
        /// A delegate called whenever a FAQ was removed.
        /// </summary>
        /// <param name="Timestamp">The timestamp when the FAQ was removed.</param>
        /// <param name="FAQ">The removed FAQ.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">The invoking FAQ identification</param>
        public delegate Task OnFAQRemovedDelegate(DateTime          Timestamp,
                                                          FAQ       FAQ,
                                                          EventTracking_Id? EventTrackingId   = null,
                                                          User_Id?          CurrentUserId     = null);

        /// <summary>
        /// An event fired whenever a FAQ was removed.
        /// </summary>
        public event OnFAQRemovedDelegate OnFAQRemoved;


        #region (class) DeleteFAQResult

        public class DeleteFAQResult
        {

            public Boolean     IsSuccess           { get; }

            public I18NString  ErrorDescription    { get; }


            private DeleteFAQResult(Boolean     IsSuccess,
                                          I18NString  ErrorDescription  = null)
            {
                this.IsSuccess         = IsSuccess;
                this.ErrorDescription  = ErrorDescription;
            }


            public static DeleteFAQResult Success

                => new DeleteFAQResult(true);

            public static DeleteFAQResult Failed(I18NString Reason)

                => new DeleteFAQResult(false,
                                             Reason);

            public static DeleteFAQResult Failed(Exception Exception)

                => new DeleteFAQResult(false,
                                             I18NString.Create(Languages.en,
                                                               Exception.Message));

            public override String ToString()

                => IsSuccess
                       ? "Success"
                       : "Failed" + (ErrorDescription.IsNullOrEmpty()
                                         ? ": " + ErrorDescription.FirstText()
                                         : "!");

        }

        #endregion

        #region (protected internal virtual) CanDeleteFAQ(FAQ)

        /// <summary>
        /// Determines whether the FAQ can safely be removed from the API.
        /// </summary>
        /// <param name="FAQ">The FAQ to be removed.</param>
        protected internal virtual I18NString CanDeleteFAQ(FAQ FAQ)
        {
            return new I18NString(Languages.en, "Currently not possible!");
        }

        #endregion


        #region (protected internal) _RemoveFAQ(FAQ, OnRemoved = null, ...)

        /// <summary>
        /// Remove the given FAQ from the API.
        /// </summary>
        /// <param name="FAQ">The FAQ to be removed from this API.</param>
        /// <param name="OnRemoved">A delegate run whenever the FAQ had been removed successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional FAQ identification initiating this command/request.</param>
        protected internal async Task<DeleteFAQResult> _RemoveFAQ(FAQ                            FAQ,
                                                                         Action<FAQ, EventTracking_Id>  OnRemoved         = null,
                                                                         EventTracking_Id?                      EventTrackingId   = null,
                                                                         User_Id?                               CurrentUserId     = null)
        {

            if (FAQ is null)
                throw new ArgumentNullException(nameof(FAQ),
                                                "The given FAQ must not be null!");

            if (FAQ.API != this || !_FAQs.TryGetValue(FAQ.Id, out FAQ FAQToBeRemoved))
                throw new ArgumentException    ("The given FAQ '" + FAQ.Id + "' does not exists in this API!",
                                                nameof(FAQ));


            var result = CanDeleteFAQ(FAQ);

            if (result == null)
            {

                var eventTrackingId = EventTrackingId ?? EventTracking_Id.New;

                await WriteToDatabaseFile(removeFAQ_MessageType,
                                          FAQ.ToJSON(false),
                                          eventTrackingId,
                                          CurrentUserId);

                _FAQs.Remove(FAQ.Id);


                var OnFAQRemovedLocal = OnFAQRemoved;
                if (OnFAQRemovedLocal is not null)
                    await OnFAQRemovedLocal?.Invoke(Timestamp.Now,
                                                            FAQ,
                                                            eventTrackingId,
                                                            CurrentUserId);

                await SendNotifications(FAQ,
                                        removeFAQ_MessageType,
                                        null,
                                        eventTrackingId,
                                        CurrentUserId);

                OnRemoved?.Invoke(FAQ,
                                  eventTrackingId);

                return DeleteFAQResult.Success;

            }
            else
                return DeleteFAQResult.Failed(result);

        }

        #endregion

        #region RemoveFAQ             (FAQ, OnRemoved = null, ...)

        /// <summary>
        /// Remove the given FAQ from the API.
        /// </summary>
        /// <param name="FAQ">The FAQ to be removed from this API.</param>
        /// <param name="OnRemoved">A delegate run whenever the FAQ had been removed successfully.</param>
        /// <param name="EventTrackingId">An optional unique event tracking identification for correlating this request with other events.</param>
        /// <param name="CurrentUserId">An optional FAQ identification initiating this command/request.</param>
        public async Task<DeleteFAQResult> RemoveFAQ(FAQ                            FAQ,
                                                                     Action<FAQ, EventTracking_Id>  OnRemoved         = null,
                                                                     EventTracking_Id?                      EventTrackingId   = null,
                                                                     User_Id?                               CurrentUserId     = null)
        {

            if (FAQ is null)
                throw new ArgumentNullException(nameof(FAQ), "The given FAQ must not be null!");

            try
            {

                return (await FAQsSemaphore.WaitAsync(SemaphoreSlimTimeout))

                            ? await _RemoveFAQ(FAQ,
                                                       OnRemoved,
                                                       EventTrackingId,
                                                       CurrentUserId)

                            : null;

            }
            catch (Exception e)
            {
                return DeleteFAQResult.Failed(e);
            }
            finally
            {
                try
                {
                    FAQsSemaphore.Release();
                }
                catch
                { }
            }

        }

        #endregion

        #endregion

        #endregion

        #region DataLicenses

        protected internal readonly Dictionary<OpenDataLicense_Id, OpenDataLicense> dataLicenses;

        /// <summary>
        /// Return an enumeration of all data licenses.
        /// </summary>
        public IEnumerable<OpenDataLicense> DataLicenses
            => dataLicenses.Values;


        #region CreateDataLicense           (Id, Description, params URLs)

        /// <summary>
        /// Create a new data license.
        /// </summary>
        /// <param name="Id">The unique identification of the data license.</param>
        /// <param name="Description">The description of the data license.</param>
        /// <param name="URLs">Optional URLs for more information on the data license.</param>
        public OpenDataLicense CreateDataLicense(OpenDataLicense_Id  Id,
                                                 I18NString          Description,
                                                 params URL[]        URLs)
        {

            lock (dataLicenses)
            {

                if (dataLicenses.ContainsKey(Id))
                    throw new ArgumentException("The given data license already exists!", nameof(Id));


                var DataLicense = new OpenDataLicense(Id,
                                                      Description,
                                                      URLs);

                WriteToDatabaseFile(NotificationMessageType.Parse("createDataLicense"),
                                    DataLicense.ToJSON(),
                                    EventTracking_Id.New,
                                    Robot.Id);

                return dataLicenses.AddAndReturnValue(DataLicense.Id, DataLicense);

            }

        }

        #endregion

        #region CreateDataLicenseIfNotExists(Id, Description, params URLs)

        /// <summary>
        /// Create a new data license.
        /// </summary>
        /// <param name="Id">The unique identification of the data license.</param>
        /// <param name="Description">The description of the data license.</param>
        /// <param name="URLs">Optional URLs for more information on the data license.</param>
        public OpenDataLicense CreateDataLicenseIfNotExists(OpenDataLicense_Id  Id,
                                                            I18NString          Description,
                                                            params URL[]        URLs)
        {

            lock (dataLicenses)
            {

                if (dataLicenses.ContainsKey(Id))
                    return dataLicenses[Id];

                return CreateDataLicense(Id,
                                         Description,
                                         URLs);

            }

        }

        #endregion


        #region GetDataLicense   (DataLicenseId)

        /// <summary>
        /// Get the data license having the given unique identification.
        /// </summary>
        /// <param name="DataLicenseId">The unique identification of the data license.</param>
        public OpenDataLicense? GetDataLicense(OpenDataLicense_Id  DataLicenseId)
        {
            lock (dataLicenses)
            {

                if (dataLicenses.TryGetValue(DataLicenseId, out var dataLicense))
                    return dataLicense;

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
        public Boolean TryGetDataLicense(OpenDataLicense_Id    DataLicenseId,
                                         out OpenDataLicense?  DataLicense)
        {
            lock (dataLicenses)
            {
                return dataLicenses.TryGetValue(DataLicenseId, out DataLicense);
            }
        }

        #endregion

        #endregion


    }

}
