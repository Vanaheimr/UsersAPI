/*
 * Copyright (c) 2014-2020, Achim 'ahzf' Friedland <achim@graphdefined.org>
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

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// A UsersAPI logger.
    /// </summary>
    public class UsersAPILogger : HTTPServerLogger
    {

        #region Data

        /// <summary>
        /// The default context of this logger.
        /// </summary>
        public const String DefaultContext = "UsersAPI";

        #endregion

        #region Properties

        /// <summary>
        /// The linked UsersAPI.
        /// </summary>
        public UsersAPI  UsersAPI  { get; }

        #endregion

        #region Constructor(s)

        #region UsersAPILogger(UsersAPI, Context = DefaultContext, LogFileCreator = null)

        /// <summary>
        /// Create a new UsersAPI logger using the default logging delegates.
        /// </summary>
        /// <param name="UsersAPI">A UsersAPI.</param>
        /// <param name="Context">A context of this API.</param>
        /// <param name="LogFileCreator">A delegate to create a log file from the given context and log file name.</param>
        public UsersAPILogger(UsersAPI                UsersAPI,
                              String                  Context         = DefaultContext,
                              LogfileCreatorDelegate  LogFileCreator  = null)

            : this(UsersAPI,
                   Context,
                   null,
                   null,
                   null,
                   null,
                   LogFileCreator: LogFileCreator)

        { }

        #endregion

        #region UsersAPILogger(UsersAPI, Context, ... Logging delegates ...)

        /// <summary>
        /// Create a new UsersAPI logger using the given logging delegates.
        /// </summary>
        /// <param name="UsersAPI">A UsersAPI.</param>
        /// <param name="Context">A context of this API.</param>
        /// 
        /// <param name="LogHTTPRequest_toConsole">A delegate to log incoming HTTP requests to console.</param>
        /// <param name="LogHTTPResponse_toConsole">A delegate to log HTTP requests/responses to console.</param>
        /// <param name="LogHTTPRequest_toDisc">A delegate to log incoming HTTP requests to disc.</param>
        /// <param name="LogHTTPResponse_toDisc">A delegate to log HTTP requests/responses to disc.</param>
        /// 
        /// <param name="LogHTTPRequest_toNetwork">A delegate to log incoming HTTP requests to a network target.</param>
        /// <param name="LogHTTPResponse_toNetwork">A delegate to log HTTP requests/responses to a network target.</param>
        /// <param name="LogHTTPRequest_toHTTPSSE">A delegate to log incoming HTTP requests to a HTTP server sent events source.</param>
        /// <param name="LogHTTPResponse_toHTTPSSE">A delegate to log HTTP requests/responses to a HTTP server sent events source.</param>
        /// 
        /// <param name="LogHTTPError_toConsole">A delegate to log HTTP errors to console.</param>
        /// <param name="LogHTTPError_toDisc">A delegate to log HTTP errors to disc.</param>
        /// <param name="LogHTTPError_toNetwork">A delegate to log HTTP errors to a network target.</param>
        /// <param name="LogHTTPError_toHTTPSSE">A delegate to log HTTP errors to a HTTP server sent events source.</param>
        /// 
        /// <param name="LogFileCreator">A delegate to create a log file from the given context and log file name.</param>
        public UsersAPILogger(UsersAPI                    UsersAPI,
                              String                      Context,

                              HTTPRequestLoggerDelegate   LogHTTPRequest_toConsole,
                              HTTPResponseLoggerDelegate  LogHTTPResponse_toConsole,
                              HTTPRequestLoggerDelegate   LogHTTPRequest_toDisc,
                              HTTPResponseLoggerDelegate  LogHTTPResponse_toDisc,

                              HTTPRequestLoggerDelegate   LogHTTPRequest_toNetwork    = null,
                              HTTPResponseLoggerDelegate  LogHTTPResponse_toNetwork   = null,
                              HTTPRequestLoggerDelegate   LogHTTPRequest_toHTTPSSE    = null,
                              HTTPResponseLoggerDelegate  LogHTTPResponse_toHTTPSSE   = null,

                              HTTPResponseLoggerDelegate  LogHTTPError_toConsole      = null,
                              HTTPResponseLoggerDelegate  LogHTTPError_toDisc         = null,
                              HTTPResponseLoggerDelegate  LogHTTPError_toNetwork      = null,
                              HTTPResponseLoggerDelegate  LogHTTPError_toHTTPSSE      = null,

                              LogfileCreatorDelegate      LogFileCreator              = null)

            : base(UsersAPI.HTTPServer,
                   Context,

                   LogHTTPRequest_toConsole,
                   LogHTTPResponse_toConsole,
                   LogHTTPRequest_toDisc,
                   LogHTTPResponse_toDisc,

                   LogHTTPRequest_toNetwork,
                   LogHTTPResponse_toNetwork,
                   LogHTTPRequest_toHTTPSSE,
                   LogHTTPResponse_toHTTPSSE,

                   LogHTTPError_toConsole,
                   LogHTTPError_toDisc,
                   LogHTTPError_toNetwork,
                   LogHTTPError_toHTTPSSE,

                   LogFileCreator)

        {

            this.UsersAPI = UsersAPI ?? throw new ArgumentNullException(nameof(UsersAPI), "The given UsersAPI must not be null!");

            #region Users

            RegisterEvent2("OnAddUserRequest",
                           handler => UsersAPI.OnAddUserRequest += handler,
                           handler => UsersAPI.OnAddUserRequest -= handler,
                           "User", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("OnAddUserResponse",
                           handler => UsersAPI.OnAddUserResponse += handler,
                           handler => UsersAPI.OnAddUserResponse -= handler,
                           "User", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("OnSetUserRequest",
                           handler => UsersAPI.OnSetUserRequest += handler,
                           handler => UsersAPI.OnSetUserRequest -= handler,
                           "User", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("OnSetUserResponse",
                           handler => UsersAPI.OnSetUserResponse += handler,
                           handler => UsersAPI.OnSetUserResponse -= handler,
                           "User", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("OnChangePasswordRequest",
                           handler => UsersAPI.OnChangePasswordRequest += handler,
                           handler => UsersAPI.OnChangePasswordRequest -= handler,
                           "User", "ChangePassword", "Password", "Request", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("OnChangePasswordResponse",
                           handler => UsersAPI.OnChangePasswordResponse += handler,
                           handler => UsersAPI.OnChangePasswordResponse -= handler,
                           "User", "ChangePassword", "Password", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("OnImpersonateUserRequest",
                           handler => UsersAPI.OnImpersonateUserRequest += handler,
                           handler => UsersAPI.OnImpersonateUserRequest -= handler,
                           "User", "Impersonate", "Request", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("OnImpersonateUserResponse",
                           handler => UsersAPI.OnImpersonateUserResponse += handler,
                           handler => UsersAPI.OnImpersonateUserResponse -= handler,
                           "User", "Impersonate", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("OnSetUserNotificationsRequest",
                           handler => UsersAPI.OnSetUserNotificationsRequest  += handler,
                           handler => UsersAPI.OnSetUserNotificationsRequest  -= handler,
                           "SetUserNotifications", "Notifications", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("OnSetUserNotificationsResponse",
                           handler => UsersAPI.OnSetUserNotificationsResponse += handler,
                           handler => UsersAPI.OnSetUserNotificationsResponse -= handler,
                           "SetUserNotifications", "Notifications", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("OnDeleteUserNotificationsRequest",
                           handler => UsersAPI.OnDeleteUserNotificationsRequest += handler,
                           handler => UsersAPI.OnDeleteUserNotificationsRequest -= handler,
                           "DeleteUserNotifications", "Notifications", "Request", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("OnDeleteUserNotificationsResponse",
                           handler => UsersAPI.OnDeleteUserNotificationsResponse += handler,
                           handler => UsersAPI.OnDeleteUserNotificationsResponse -= handler,
                           "DeleteUserNotifications", "Notifications", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Organizations

            RegisterEvent2("OnAddOrganizationRequest",
                           handler => UsersAPI.OnAddOrganizationRequest += handler,
                           handler => UsersAPI.OnAddOrganizationRequest -= handler,
                           "Organization", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("OnAddOrganizationResponse",
                           handler => UsersAPI.OnAddOrganizationResponse += handler,
                           handler => UsersAPI.OnAddOrganizationResponse -= handler,
                           "Organization", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("OnSetOrganizationRequest",
                           handler => UsersAPI.OnSetOrganizationHTTPRequest += handler,
                           handler => UsersAPI.OnSetOrganizationHTTPRequest -= handler,
                           "Organization", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("OnSetOrganizationResponse",
                           handler => UsersAPI.OnSetOrganizationHTTPResponse += handler,
                           handler => UsersAPI.OnSetOrganizationHTTPResponse -= handler,
                           "Organization", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("OnSetOrganizationNotificationsRequest",
                           handler => UsersAPI.OnSetOrganizationNotificationsRequest  += handler,
                           handler => UsersAPI.OnSetOrganizationNotificationsRequest  -= handler,
                           "SetOrganizationNotifications", "Organization", "Notifications", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("OnSetOrganizationNotificationsResponse",
                           handler => UsersAPI.OnSetOrganizationNotificationsResponse += handler,
                           handler => UsersAPI.OnSetOrganizationNotificationsResponse -= handler,
                           "SetOrganizationNotifications", "Organization", "Notifications", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("OnDeleteOrganizationNotificationsRequest",
                           handler => UsersAPI.OnDeleteOrganizationNotificationsRequest += handler,
                           handler => UsersAPI.OnDeleteOrganizationNotificationsRequest -= handler,
                           "DeleteOrganizationNotifications", "Organization", "Notifications", "Request", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("OnDeleteOrganizationNotificationsResponse",
                           handler => UsersAPI.OnDeleteOrganizationNotificationsResponse += handler,
                           handler => UsersAPI.OnDeleteOrganizationNotificationsResponse -= handler,
                           "DeleteOrganizationNotifications", "Organization", "Notifications", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("OnDeleteOrganizationRequest",
                           handler => UsersAPI.OnDeleteOrganizationRequest += handler,
                           handler => UsersAPI.OnDeleteOrganizationRequest -= handler,
                           "DeleteOrganizations", "Organization", "Request", "All").
                     RegisterDefaultConsoleLogTarget(this).
                     RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("OnDeleteOrganizationResponse",
                           handler => UsersAPI.OnDeleteOrganizationResponse += handler,
                           handler => UsersAPI.OnDeleteOrganizationResponse -= handler,
                           "DeleteOrganizations", "Organization", "Response", "All").
                     RegisterDefaultConsoleLogTarget(this).
                     RegisterDefaultDiscLogTarget(this);

            #endregion

        }

        #endregion

        #endregion

    }

}
