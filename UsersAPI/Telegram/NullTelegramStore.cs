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

    public class NullTelegramStore : ITelegramStore
    {

        #region Data

        /// <summary>
        /// ASCII unit/cell separator
        /// </summary>
        protected const  Char                               US = (Char) 0x1F;

        private readonly Dictionary<String, TelegramUser>   UserByUsername;

        private readonly Dictionary<Int64,  TelegramUser>   UserByChatId;

        private readonly Dictionary<String, TelegramGroup>  GroupByTitle;

        private readonly Dictionary<Int64,  TelegramGroup>  GroupByChatId;

        #endregion

        #region Properties

        /// <summary>
        /// The linked UsersAPI.
        /// </summary>
        public UsersAPI  UsersAPI  { get; }

        #endregion

        #region Events

        public event OnSendTelegramRequestDelegate                     OnSendTelegramRequest;

        public event OnSendTelegramResponseDelegate                    OnSendTelegramResponse;

        public event EventHandler<Telegram.Bot.Args.MessageEventArgs>  OnMessage;

        #endregion

        #region Constructor(s)

        public NullTelegramStore()

        {

            this.UserByUsername  = new Dictionary<String, TelegramUser>();
            this.UserByChatId    = new Dictionary<Int64,  TelegramUser>();
            this.GroupByTitle    = new Dictionary<String, TelegramGroup>();
            this.GroupByChatId   = new Dictionary<Int64,  TelegramGroup>();

        }

        #endregion


        #region UpdateUser(...)

        public void UpdateUser(Int32   UserId,
                               String  Username,
                               String  Firstname,
                               String  Lastname,
                               Int64   ChatId)
        {

            lock (UserByUsername)
            {
                lock (UserByChatId)
                {

                    try
                    {

                        if (!UserByUsername.TryGetValue(Username, out TelegramUser existingTelegramUser))
                        {

                            var newTelegramUser = new TelegramUser(UserId,
                                                                   Username,
                                                                   Firstname,
                                                                   Lastname,
                                                                   ChatId);

                            UserByUsername.Add(Username, newTelegramUser);

                            if (UserByChatId.ContainsKey(ChatId))
                                UserByChatId.Remove(ChatId);

                            UserByChatId.Add(ChatId, newTelegramUser);

                            System.IO.File.AppendAllText("TelegramStore.csv",
                                                         String.Concat("updateUser", US, UserId, US, Username, US, Firstname, US, Lastname, US, ChatId, Environment.NewLine));

                        }

                        else
                        {

                            if (existingTelegramUser.ChatId != ChatId)
                            {

                                UserByUsername.Remove(Username);
                                UserByChatId.Remove(existingTelegramUser.ChatId);

                                var newTelegramUser = new TelegramUser(UserId,
                                                                       Username,
                                                                       Firstname,
                                                                       Lastname,
                                                                       ChatId);

                                UserByUsername.Add(Username, newTelegramUser);

                                if (UserByChatId.ContainsKey(ChatId))
                                    UserByChatId.Remove(ChatId);

                                UserByChatId.Add(ChatId, newTelegramUser);

                                System.IO.File.AppendAllText("TelegramStore.csv",
                                                             String.Concat("updateUser", US, UserId, US, Username, US, Firstname, US, Lastname, US, ChatId, Environment.NewLine));

                            }

                        }

                    }
                    catch
                    { }

                }
            }

        }

        #endregion

        #region UpdateGroup(...)

        public void UpdateGroup(Int64   ChatId,
                                String  Title,
                                String  InviteLink)
        {

            lock (GroupByTitle)
            {
                lock (GroupByChatId)
                {

                    try
                    {

                        if (Title.IsNeitherNullNorEmpty())
                            Title = Title.Trim();

                        if (!GroupByChatId.TryGetValue(ChatId, out TelegramGroup existingTelegramGroup))
                        {

                            var newTelegramGroup = new TelegramGroup(ChatId,
                                                                     Title,
                                                                     InviteLink);

                            GroupByChatId.Add(ChatId, newTelegramGroup);

                            if (GroupByTitle.ContainsKey(Title))
                                GroupByTitle.Remove(Title);

                            GroupByTitle.Add(Title, newTelegramGroup);

                            System.IO.File.AppendAllText("TelegramStore.csv",
                                                         String.Concat("updateGroup", US, ChatId, US, Title, US, InviteLink, Environment.NewLine));

                        }

                        else
                        {

                            if (existingTelegramGroup.Title != Title ||
                                existingTelegramGroup.InviteLink != InviteLink)
                            {

                                GroupByChatId.Remove(ChatId);
                                GroupByTitle.Remove(existingTelegramGroup.Title);

                                var newTelegramGroup = new TelegramGroup(ChatId,
                                                                         Title,
                                                                         InviteLink);

                                GroupByChatId.Add(ChatId, newTelegramGroup);

                                if (GroupByTitle.ContainsKey(Title))
                                    GroupByTitle.Remove(Title);

                                GroupByTitle.Add(Title, newTelegramGroup);

                                System.IO.File.AppendAllText("TelegramStore.csv",
                                                             String.Concat("updateGroup", US, ChatId, US, Title, US, InviteLink, Environment.NewLine));

                            }

                        }

                    }
                    catch
                    { }

                }
            }

        }

        #endregion


        internal async void ReceiveTelegramMessage(Object                              Sender,
                                                   Telegram.Bot.Args.MessageEventArgs  Telegram)
        {

            try
            {

                var messageFrom = Telegram?.Message?.From;
                var messageChat = Telegram?.Message?.Chat;
                var messageText = Telegram?.Message?.Text;

                if (messageText.IsNeitherNullNorEmpty())
                    messageText.Trim();

                if (messageChat is not null)
                {

                    if (messageChat.Id >= 0)
                    {

                        Console.WriteLine($"Received a telegram text message from {Telegram.Message.From.Username} in chat {Telegram.Message.Chat.Id}.");

                        UpdateUser(messageFrom.Id,
                                   messageFrom.Username,
                                   messageFrom.FirstName,
                                   messageFrom.LastName,
                                   messageChat.Id);

                        //await this.TelegramAPI.SendTextMessageAsync(
                        //    chatId:  messageChat.Id,
                        //    text:    "Hello " + e.Message.From.FirstName + " " + e.Message.From.LastName + "!\nYou said:\n" + e.Message.Text
                        //);

                    }

                    else
                    {

                        Console.WriteLine($"Received a telegram text message from {Telegram.Message.From.Username} in group chat '{Telegram.Message.Chat.Title}' / {Telegram.Message.Chat.Id}.");

                        UpdateGroup(messageChat.Id,
                                    messageChat.Title,
                                    messageChat.InviteLink);

                        //await this.TelegramAPI.SendTextMessageAsync(
                        //    chatId:  messageChat.Id,
                        //    text:    "Hello " + e.Message.From.FirstName + " " + e.Message.From.LastName + "!\nYou said:\n" + e.Message.Text
                        //);

                    }

                }

            }
            catch
            { }

        }


        private readonly List<Tuple<String, IEnumerable<String>>> telegrams = new List<Tuple<String, IEnumerable<String>>>();

        public IEnumerable<Tuple<String, IEnumerable<String>>> Telegrams
            => telegrams;



        #region SendTelegram (Message, Username,  ParseMode)

        /// <summary>
        /// Send a Telegram to the given user.
        /// </summary>
        /// <param name="Message">The text of the message.</param>
        /// <param name="Username">The name of the user.</param>
        public async Task<MessageEnvelop> SendTelegram(String     Message,
                                                       String     Username,
                                                       ParseMode  ParseMode)
        {

            #region Initial checks

            Message   = Message?.Trim();
            Username  = Username?.Trim();

            if (Message.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Message),   "The given message must not be null or empty!");

            if (Username.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Username),  "The given username must not be null or empty!");

            #endregion

            var eventTrackingId  = EventTracking_Id.New;
            var message          = I18NString.Create(Message);
            var usernames        = new String[] { Username };

            #region Send OnSendTelegramRequest event

            var StartTime = Timestamp.Now;

            try
            {

                if (OnSendTelegramRequest is not null)
                    await Task.WhenAll(OnSendTelegramRequest.GetInvocationList().
                                       Cast<OnSendTelegramRequestDelegate>().
                                       Select(e => e(StartTime,
                                                     this,
                                                     eventTrackingId,
                                                     message,
                                                     usernames))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(TelegramStore) + "." + nameof(OnSendTelegramRequest));
            }

            #endregion


            telegrams.Add(new Tuple<String, IEnumerable<String>>(Message, new String[] { Username }));

            var responseMessage = new MessageEnvelop(Username,
                                                     new Telegram.Bot.Types.Message() {
                                                         Text = "Ok"
                                                     });


            #region Send OnSendTelegramResponse event

            var Endtime = Timestamp.Now;

            try
            {

                if (OnSendTelegramResponse is not null)
                    await Task.WhenAll(OnSendTelegramResponse.GetInvocationList().
                                       Cast<OnSendTelegramResponseDelegate>().
                                       Select(e => e(Endtime,
                                                     this,
                                                     eventTrackingId,
                                                     message,
                                                     usernames,
                                                     new MessageEnvelop[] {
                                                        responseMessage
                                                     },
                                                     Endtime - StartTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(TelegramStore) + "." + nameof(OnSendTelegramResponse));
            }

            #endregion

            return responseMessage;

        }


        /// <summary>
        /// Send a multi-language Telegram to the given user in his/her preferred language.
        /// </summary>
        /// <param name="Message">The  multi-language text of the message.</param>
        /// <param name="Username">The name of the user.</param>
        public async Task<MessageEnvelop> SendTelegram(I18NString  Message,
                                                       String      Username,
                                                       ParseMode   ParseMode)
        {

            #region Initial checks

            Username  = Username?.Trim();

            if (Message.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Message),   "The given message must not be null or empty!");

            if (Username.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Username),  "The given username must not be null or empty!");

            #endregion

            var eventTrackingId  = EventTracking_Id.New;
            var usernames        = new String[] { Username };

            #region Send OnSendTelegramRequest event

            var StartTime = Timestamp.Now;

            try
            {

                if (OnSendTelegramRequest is not null)
                    await Task.WhenAll(OnSendTelegramRequest.GetInvocationList().
                                       Cast<OnSendTelegramRequestDelegate>().
                                       Select(e => e(StartTime,
                                                     this,
                                                     eventTrackingId,
                                                     Message,
                                                     usernames))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(TelegramStore) + "." + nameof(OnSendTelegramRequest));
            }

            #endregion


            telegrams.Add(new Tuple<String, IEnumerable<String>>(Message[Languages.en], new String[] { Username }));


            var responseMessage = new MessageEnvelop(Username,
                                                     new Telegram.Bot.Types.Message() {
                                                         Text = "Ok"
                                                     });


            #region Send OnSendTelegramResponse event

            var Endtime = Timestamp.Now;

            try
            {

                if (OnSendTelegramResponse is not null)
                    await Task.WhenAll(OnSendTelegramResponse.GetInvocationList().
                                       Cast<OnSendTelegramResponseDelegate>().
                                       Select(e => e(Endtime,
                                                     this,
                                                     eventTrackingId,
                                                     Message,
                                                     usernames,
                                                     new MessageEnvelop[] {
                                                        responseMessage
                                                     },
                                                     Endtime - StartTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(TelegramStore) + "." + nameof(OnSendTelegramResponse));
            }

            #endregion

            return responseMessage;

        }

        #endregion

        #region SendTelegrams(Message, Usernames, ParseMode)

        ///// <summary>
        ///// Send a Telegram to the given users.
        ///// </summary>
        ///// <param name="Message">The text of the message.</param>
        ///// <param name="Usernames">An enumeration of usernames.</param>
        //public Task<IEnumerable<MessageEnvelop>> SendTelegrams(String           Message,
        //                                                       params String[]  Usernames)

        //    => SendTelegrams(Message, Usernames as IEnumerable<String>);


        /// <summary>
        /// Send a Telegram to the given users.
        /// </summary>
        /// <param name="Message">The text of the message.</param>
        /// <param name="Usernames">An enumeration of usernames.</param>
        public async Task<IEnumerable<MessageEnvelop>> SendTelegrams(String               Message,
                                                                     IEnumerable<String>  Usernames,
                                                                     ParseMode            ParseMode)
        {

            #region Initial checks

            Message    = Message?.Trim();
            Usernames  = Usernames.SafeSelect(username => username?.Trim()).
                                   SafeWhere (username => !username.IsNullOrEmpty()).
                                   ToArray();

            if (Message.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Message),    "The given message must not be null or empty!");

            if (Usernames.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Usernames),  "The given enumeration of usernames must not be null or empty!");

            var responseMessages = new List<MessageEnvelop>();

            #endregion

            var eventTrackingId  = EventTracking_Id.New;
            var message          = I18NString.Create(Message);

            #region Send OnSendTelegramRequest event

            var StartTime = Timestamp.Now;

            try
            {

                if (OnSendTelegramRequest is not null)
                    await Task.WhenAll(OnSendTelegramRequest.GetInvocationList().
                                       Cast<OnSendTelegramRequestDelegate>().
                                       Select(e => e(StartTime,
                                                     this,
                                                     eventTrackingId,
                                                     message,
                                                     Usernames))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(TelegramStore) + "." + nameof(OnSendTelegramRequest));
            }

            #endregion


            telegrams.Add(new Tuple<String, IEnumerable<String>>(Message, Usernames));

            foreach (var username in Usernames)
            {

                responseMessages.Add(new MessageEnvelop(username,
                                                        username.GetHashCode(),
                                                        new Telegram.Bot.Types.Message() {
                                                            Text = "Ok"
                                                        }));

            }


            #region Send OnSendTelegramResponse event

            var Endtime = Timestamp.Now;

            try
            {

                if (OnSendTelegramResponse is not null)
                    await Task.WhenAll(OnSendTelegramResponse.GetInvocationList().
                                       Cast<OnSendTelegramResponseDelegate>().
                                       Select(e => e(Endtime,
                                                     this,
                                                     eventTrackingId,
                                                     message,
                                                     Usernames,
                                                     responseMessages,
                                                     Endtime - StartTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(TelegramStore) + "." + nameof(OnSendTelegramResponse));
            }

            #endregion

            return responseMessages;

        }



        /// <summary>
        /// Send a multi-language Telegram to the given users in their preferred language.
        /// </summary>
        /// <param name="Message">The multi-language text of the message.</param>
        /// <param name="Usernames">An enumeration of usernames.</param>
        public Task<IEnumerable<MessageEnvelop>> SendTelegram(I18NString       Message,
                                                              ParseMode        ParseMode,
                                                              params String[]  Usernames)

            => SendTelegram(Message, Usernames as IEnumerable<String>, ParseMode);


        /// <summary>
        /// Send a multi-language Telegram to the given users in their preferred language.
        /// </summary>
        /// <param name="Message">The multi-language text of the message.</param>
        /// <param name="Usernames">An enumeration of usernames.</param>
        public async Task<IEnumerable<MessageEnvelop>> SendTelegram(I18NString           Message,
                                                                    IEnumerable<String>  Usernames,
                                                                    ParseMode            ParseMode)
        {

            #region Initial checks

            Usernames  = Usernames.SafeSelect(username => username?.Trim()).
                                   SafeWhere (username => !username.IsNullOrEmpty()).
                                   ToArray();

            if (Message.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Message),    "The given message must not be null or empty!");

            if (Usernames.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Usernames),  "The given enumeration of usernames must not be null or empty!");

            var responseMessages = new List<MessageEnvelop>();

            #endregion

            var eventTrackingId  = EventTracking_Id.New;

            #region Send OnSendTelegramRequest event

            var StartTime = Timestamp.Now;

            try
            {

                if (OnSendTelegramRequest is not null)
                    await Task.WhenAll(OnSendTelegramRequest.GetInvocationList().
                                       Cast<OnSendTelegramRequestDelegate>().
                                       Select(e => e(StartTime,
                                                     this,
                                                     eventTrackingId,
                                                     Message,
                                                     Usernames))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(TelegramStore) + "." + nameof(OnSendTelegramRequest));
            }

            #endregion


            telegrams.Add(new Tuple<String, IEnumerable<String>>(Message[Languages.en], Usernames));

            foreach (var username in Usernames)
            {

                responseMessages.Add(new MessageEnvelop(username,
                                                        username.GetHashCode(),
                                                        new Telegram.Bot.Types.Message() {
                                                            Text = "Ok"
                                                        }));

            }


            #region Send OnSendTelegramResponse event

            var Endtime = Timestamp.Now;

            try
            {

                if (OnSendTelegramResponse is not null)
                    await Task.WhenAll(OnSendTelegramResponse.GetInvocationList().
                                       Cast<OnSendTelegramResponseDelegate>().
                                       Select(e => e(Endtime,
                                                     this,
                                                     eventTrackingId,
                                                     Message,
                                                     Usernames,
                                                     responseMessages,
                                                     Endtime - StartTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.LogException(e, nameof(TelegramStore) + "." + nameof(OnSendTelegramResponse));
            }

            #endregion

            return responseMessages;

        }

        #endregion



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
        public Task<Telegram.Bot.Types.Message> SendTextMessageAsync(ChatId             ChatId,
                                                                     String             Text,
                                                                     ParseMode          ParseMode               = default,
                                                                     Boolean            DisableWebPagePreview   = default,
                                                                     Boolean            DisableNotification     = default,
                                                                     Int32              ReplyToMessageId        = default,
                                                                     IReplyMarkup       ReplyMarkup             = default,
                                                                     CancellationToken  CancellationToken       = default)

            => Task.FromResult(
                   new Telegram.Bot.Types.Message() {
                       Text = "Ok"
                   });


        #region Clear()

        public void Clear()
        {
            telegrams.Clear();
        }

        #endregion


    }

}
