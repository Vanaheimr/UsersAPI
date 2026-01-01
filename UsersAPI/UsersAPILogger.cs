/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System;

using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Logging;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// The Users API logger.
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

        #region UsersAPILogger(UsersAPI, Context = DefaultContext, LogfileCreator = null)

        /// <summary>
        /// Create a new UsersAPI logger using the default logging delegates.
        /// </summary>
        /// <param name="UsersAPI">A UsersAPI.</param>
        /// <param name="LoggingPath">The logging path.</param>
        /// <param name="Context">A context of this API.</param>
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        public UsersAPILogger(UsersAPI                UsersAPI,
                              String                  LoggingPath,
                              String                  Context         = DefaultContext,
                              LogfileCreatorDelegate  LogfileCreator  = null)

            : this(UsersAPI,
                   LoggingPath,
                   Context,
                   null,
                   null,
                   null,
                   null,
                   LogfileCreator: LogfileCreator)

        { }

        #endregion

        #region UsersAPILogger(UsersAPI, Context, ... Logging delegates ...)

        /// <summary>
        /// Create a new UsersAPI logger using the given logging delegates.
        /// </summary>
        /// <param name="UsersAPI">A UsersAPI.</param>
        /// <param name="LoggingPath">The logging path.</param>
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
        /// <param name="LogfileCreator">A delegate to create a log file from the given context and log file name.</param>
        public UsersAPILogger(UsersAPI                     UsersAPI,
                              String                       LoggingPath,
                              String                       Context,

                              HTTPRequestLoggerDelegate?   LogHTTPRequest_toConsole    = null,
                              HTTPResponseLoggerDelegate?  LogHTTPResponse_toConsole   = null,
                              HTTPRequestLoggerDelegate?   LogHTTPRequest_toDisc       = null,
                              HTTPResponseLoggerDelegate?  LogHTTPResponse_toDisc      = null,

                              HTTPRequestLoggerDelegate?   LogHTTPRequest_toNetwork    = null,
                              HTTPResponseLoggerDelegate?  LogHTTPResponse_toNetwork   = null,
                              HTTPRequestLoggerDelegate?   LogHTTPRequest_toHTTPSSE    = null,
                              HTTPResponseLoggerDelegate?  LogHTTPResponse_toHTTPSSE   = null,

                              HTTPResponseLoggerDelegate?  LogHTTPError_toConsole      = null,
                              HTTPResponseLoggerDelegate?  LogHTTPError_toDisc         = null,
                              HTTPResponseLoggerDelegate?  LogHTTPError_toNetwork      = null,
                              HTTPResponseLoggerDelegate?  LogHTTPError_toHTTPSSE      = null,

                              LogfileCreatorDelegate?      LogfileCreator              = null)

            : base(UsersAPI.HTTPServer,
                   LoggingPath,
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

                   LogfileCreator)

        {

            this.UsersAPI = UsersAPI ?? throw new ArgumentNullException(nameof(UsersAPI), "The given UsersAPI must not be null!");

            #region Users

            RegisterEvent2("AddUserRequest",
                           handler => UsersAPI.OnAddUserHTTPRequest += handler,
                           handler => UsersAPI.OnAddUserHTTPRequest -= handler,
                           "User", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("AddUserResponse",
                           handler => UsersAPI.OnAddUserHTTPResponse += handler,
                           handler => UsersAPI.OnAddUserHTTPResponse -= handler,
                           "User", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("SetUserRequest",
                           handler => UsersAPI.OnSetUserHTTPRequest += handler,
                           handler => UsersAPI.OnSetUserHTTPRequest -= handler,
                           "User", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("SetUserResponse",
                           handler => UsersAPI.OnSetUserHTTPResponse += handler,
                           handler => UsersAPI.OnSetUserHTTPResponse -= handler,
                           "User", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("ChangePasswordRequest",
                           handler => UsersAPI.OnChangePasswordRequest += handler,
                           handler => UsersAPI.OnChangePasswordRequest -= handler,
                           "User", "ChangePassword", "Password", "Request", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("ChangePasswordResponse",
                           handler => UsersAPI.OnChangePasswordResponse += handler,
                           handler => UsersAPI.OnChangePasswordResponse -= handler,
                           "User", "ChangePassword", "Password", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("ImpersonateUserRequest",
                           handler => UsersAPI.OnImpersonateUserRequest += handler,
                           handler => UsersAPI.OnImpersonateUserRequest -= handler,
                           "User", "Impersonate", "Request", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("ImpersonateUserResponse",
                           handler => UsersAPI.OnImpersonateUserResponse += handler,
                           handler => UsersAPI.OnImpersonateUserResponse -= handler,
                           "User", "Impersonate", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("SetUserNotificationsRequest",
                           handler => UsersAPI.OnSetUserNotificationsRequest  += handler,
                           handler => UsersAPI.OnSetUserNotificationsRequest  -= handler,
                           "SetUserNotifications", "Notifications", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("SetUserNotificationsResponse",
                           handler => UsersAPI.OnSetUserNotificationsResponse += handler,
                           handler => UsersAPI.OnSetUserNotificationsResponse -= handler,
                           "SetUserNotifications", "Notifications", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("DeleteUserNotificationsRequest",
                           handler => UsersAPI.OnDeleteUserNotificationsRequest += handler,
                           handler => UsersAPI.OnDeleteUserNotificationsRequest -= handler,
                           "DeleteUserNotifications", "Notifications", "Request", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("DeleteUserNotificationsResponse",
                           handler => UsersAPI.OnDeleteUserNotificationsResponse += handler,
                           handler => UsersAPI.OnDeleteUserNotificationsResponse -= handler,
                           "DeleteUserNotifications", "Notifications", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region Organizations

            RegisterEvent2("AddOrganizationRequest",
                           handler => UsersAPI.OnAddOrganizationHTTPRequest += handler,
                           handler => UsersAPI.OnAddOrganizationHTTPRequest -= handler,
                           "Organization", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("AddOrganizationResponse",
                           handler => UsersAPI.OnAddOrganizationHTTPResponse += handler,
                           handler => UsersAPI.OnAddOrganizationHTTPResponse -= handler,
                           "Organization", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("SetOrganizationRequest",
                           handler => UsersAPI.OnSetOrganizationHTTPRequest += handler,
                           handler => UsersAPI.OnSetOrganizationHTTPRequest -= handler,
                           "Organization", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("SetOrganizationResponse",
                           handler => UsersAPI.OnSetOrganizationHTTPResponse += handler,
                           handler => UsersAPI.OnSetOrganizationHTTPResponse -= handler,
                           "Organization", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("SetOrganizationNotificationsRequest",
                           handler => UsersAPI.OnSetOrganizationNotificationsRequest  += handler,
                           handler => UsersAPI.OnSetOrganizationNotificationsRequest  -= handler,
                           "SetOrganizationNotifications", "Organization", "Notifications", "Request",  "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("SetOrganizationNotificationsResponse",
                           handler => UsersAPI.OnSetOrganizationNotificationsResponse += handler,
                           handler => UsersAPI.OnSetOrganizationNotificationsResponse -= handler,
                           "SetOrganizationNotifications", "Organization", "Notifications", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("DeleteOrganizationNotificationsRequest",
                           handler => UsersAPI.OnDeleteOrganizationNotificationsRequest += handler,
                           handler => UsersAPI.OnDeleteOrganizationNotificationsRequest -= handler,
                           "DeleteOrganizationNotifications", "Organization", "Notifications", "Request", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("DeleteOrganizationNotificationsResponse",
                           handler => UsersAPI.OnDeleteOrganizationNotificationsResponse += handler,
                           handler => UsersAPI.OnDeleteOrganizationNotificationsResponse -= handler,
                           "DeleteOrganizationNotifications", "Organization", "Notifications", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("DeleteOrganizationRequest",
                           handler => UsersAPI.OnDeleteOrganizationHTTPRequest += handler,
                           handler => UsersAPI.OnDeleteOrganizationHTTPRequest -= handler,
                           "DeleteOrganizations", "Organization", "Request", "All").
                     RegisterDefaultConsoleLogTarget(this).
                     RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("DeleteOrganizationResponse",
                           handler => UsersAPI.OnDeleteOrganizationHTTPResponse += handler,
                           handler => UsersAPI.OnDeleteOrganizationHTTPResponse -= handler,
                           "DeleteOrganizations", "Organization", "Response", "All").
                     RegisterDefaultConsoleLogTarget(this).
                     RegisterDefaultDiscLogTarget(this);

            #endregion

            #region ServiceTickets

            RegisterEvent2("AddServiceTicketRequest",
                           handler => UsersAPI.OnAddServiceTicketRequest += handler,
                           handler => UsersAPI.OnAddServiceTicketRequest -= handler,
                           "ServiceTicket", "Request", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("AddServiceTicketResponse",
                           handler => UsersAPI.OnAddServiceTicketResponse += handler,
                           handler => UsersAPI.OnAddServiceTicketResponse -= handler,
                           "ServiceTicket", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("SetServiceTicketRequest",
                           handler => UsersAPI.OnSetServiceTicketRequest += handler,
                           handler => UsersAPI.OnSetServiceTicketRequest -= handler,
                           "ServiceTicket", "Request", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("SetServiceTicketResponse",
                           handler => UsersAPI.OnSetServiceTicketResponse += handler,
                           handler => UsersAPI.OnSetServiceTicketResponse -= handler,
                           "ServiceTicket", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("AddServiceTicketCommentRequest",
                           handler => UsersAPI.OnAddServiceTicketChangeSetRequest += handler,
                           handler => UsersAPI.OnAddServiceTicketChangeSetRequest -= handler,
                           "ServiceTicketComment", "ServiceTicket", "Request", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("AddServiceTicketCommentResponse",
                           handler => UsersAPI.OnAddServiceTicketChangeSetResponse += handler,
                           handler => UsersAPI.OnAddServiceTicketChangeSetResponse -= handler,
                           "ServiceTicketComment", "ServiceTicket", "Response", "All").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

            #region API

            RegisterEvent2("RestartRequest",
                           handler => UsersAPI.OnRestartHTTPRequest += handler,
                           handler => UsersAPI.OnRestartHTTPRequest -= handler,
                           "api", "restart", "request",  "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("RestartResponse",
                           handler => UsersAPI.OnRestartHTTPResponse += handler,
                           handler => UsersAPI.OnRestartHTTPResponse -= handler,
                           "api", "restart", "response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);


            RegisterEvent2("StopRequest",
                           handler => UsersAPI.OnStopHTTPRequest += handler,
                           handler => UsersAPI.OnStopHTTPRequest -= handler,
                           "api", "stop", "request", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            RegisterEvent2("StopResponse",
                           handler => UsersAPI.OnStopHTTPResponse += handler,
                           handler => UsersAPI.OnStopHTTPResponse -= handler,
                           "api", "stop", "response", "all").
                RegisterDefaultConsoleLogTarget(this).
                RegisterDefaultDiscLogTarget(this);

            #endregion

        }

        #endregion

        #endregion

    }

}
