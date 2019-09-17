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

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace org.GraphDefined.OpenData.Users
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

            #region UserNotifications

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

        }

        #endregion

        #endregion

    }

}
