/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com> <achim.friedland@graphdefined.com>
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Telegram.Bot;

using org.GraphDefined.Vanaheimr.Illias;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;

#endregion

namespace social.OpenData.UsersAPI
{

    public delegate Task OnSendTelegramRequestDelegate (DateTime                      LogTimestamp,
                                                        ITelegramStore                Sender,
                                                        EventTracking_Id              EventTrackingId,
                                                        I18NString                    Message,
                                                        IEnumerable<String>           Usernames);

    public delegate Task OnSendTelegramResponseDelegate(DateTime                      LogTimestamp,
                                                        ITelegramStore                Sender,
                                                        EventTracking_Id              EventTrackingId,
                                                        I18NString                    Message,
                                                        IEnumerable<String>           Usernames,
                                                        IEnumerable<MessageEnvelop>   Responses,
                                                        TimeSpan                      Runtime);


    public class TelegramUser
    {

        public Int32      UserId               { get; }
        public String     Username             { get; }
        public String     Firstname            { get; }
        public String     Lastname             { get; }
        public Int64      ChatId               { get; }
        public Languages  PreferredLanguage    { get; }

        public TelegramUser(Int32      UserId,
                            String     Username,
                            String     Firstname,
                            String     Lastname,
                            Int64      ChatId,
                            Languages  PreferredLanguage = Languages.en)
        {

            this.UserId             = UserId;
            this.Username           = Username;
            this.Firstname          = Firstname;
            this.Lastname           = Lastname;
            this.ChatId             = ChatId;
            this.PreferredLanguage  = PreferredLanguage;

        }

    }


    public class TelegramGroup
    {
        public Int64   ChatId        { get; }
        public String  Title         { get; }
        public String  InviteLink    { get; }

        public TelegramGroup(Int64   ChatId,
                             String  Title,
                             String  InviteLink)
        {

            this.ChatId      = ChatId;
            this.Title       = Title;
            this.InviteLink  = InviteLink;

        }

    }


    public class MessageEnvelop
    {

        public String                      Username    { get; }
        public Int64?                      ChatId      { get; }
        public Telegram.Bot.Types.Message  Message     { get; }

        public MessageEnvelop(String                      User,
                              Telegram.Bot.Types.Message  Message)
        {

            this.Username  = User;
            this.Message   = Message;

        }

        public MessageEnvelop(String                      User,
                              Int64?                      ChatId,
                              Telegram.Bot.Types.Message  Message)
        {

            this.Username  = User;
            this.ChatId    = ChatId;
            this.Message   = Message;

        }

    }



    public interface ITelegramStore
    {
        UsersAPI UsersAPI { get; }

        event OnSendTelegramRequestDelegate  OnSendTelegramRequest;
        event OnSendTelegramResponseDelegate OnSendTelegramResponse;

        /// <summary>
        /// Occurs when a <see cref="Message"/> is received.
        /// </summary>
        event EventHandler<Telegram.Bot.Args.MessageEventArgs> OnMessage;

        Task<IEnumerable<MessageEnvelop>> SendTelegram(I18NString Message, IEnumerable<string> Usernames, ParseMode ParseMode);
        Task<IEnumerable<MessageEnvelop>> SendTelegram(I18NString Message, ParseMode ParseMode, params string[] Usernames);
        Task<MessageEnvelop> SendTelegram(I18NString Message, string Username, ParseMode ParseMode);
        Task<MessageEnvelop> SendTelegram(string Message, string Username, ParseMode ParseMode);
        Task<IEnumerable<MessageEnvelop>> SendTelegrams(string Message, IEnumerable<string> Usernames, ParseMode ParseMode);
        //Task<IEnumerable<MessageEnvelop>> SendTelegrams(string Message, params string[] Usernames);
        void UpdateGroup(long ChatId, string Title, string InviteLink);
        void UpdateUser(int UserId, string Username, string Firstname, string Lastname, long ChatId);


        /// <summary>
        /// Use this method to send text messages. On success, the sent Description is returned.
        /// </summary>
        /// <param name="ChatId"><see cref="ChatId"/> for the target chat</param>
        /// <param name="Text">Text of the message to be sent</param>
        /// <param name="ParseMode">Change, if you want Telegram apps to show bold, italic, fixed-width text or inline URLs in your bot's message.</param>
        /// <param name="DisableWebPagePreview">Disables link previews for links in this message</param>
        /// <param name="DisableNotification">Sends the message silently. iOS users will not receive a notification, Android users will receive a notification with no sound.</param>
        /// <param name="ReplyToMessageId">If the message is a reply, ID of the original message</param>
        /// <param name="ReplyMarkup">Additional interface options. A JSON-serialized object for a custom reply keyboard, instructions to hide keyboard or to force a reply from the user.</param>
        /// <param name="CancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>On success, the sent Description is returned.</returns>
        /// <see href="https://core.telegram.org/bots/api#sendmessage"/>
        Task<Telegram.Bot.Types.Message> SendTextMessageAsync(ChatId             ChatId,
                                                              String             Text,
                                                              ParseMode          ParseMode               = default,
                                                              Boolean            DisableWebPagePreview   = default,
                                                              Boolean            DisableNotification     = default,
                                                              Int32              ReplyToMessageId        = default,
                                                              IReplyMarkup       ReplyMarkup             = default,
                                                              CancellationToken  CancellationToken       = default);


    }
}