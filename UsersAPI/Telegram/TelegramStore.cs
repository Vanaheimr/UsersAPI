/*
 * Copyright (c) 2014-2021, Achim Friedland <achim.friedland@graphdefined.com>
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

#endregion

namespace social.OpenData.UsersAPI
{

    public class TelegramStore
    {

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

            public String                      Username       { get; }
            public Int64?                      ChatId     { get; }
            public Telegram.Bot.Types.Message  Message    { get; }

            public MessageEnvelop(String                      User,
                                  Telegram.Bot.Types.Message  Message)
            {

                this.Username     = User;
                this.Message  = Message;

            }

            public MessageEnvelop(String                      User,
                                  Int64?                      ChatId,
                                  Telegram.Bot.Types.Message  Message)
            {

                this.Username     = User;
                this.ChatId   = ChatId;
                this.Message  = Message;

            }

        }


        #region Data

        /// <summary>
        /// ASCII unit/cell separator
        /// </summary>
        protected const  Char                               US = (Char) 0x1F;

        private readonly TelegramBotClient                  TelegramAPI;

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

        public delegate Task OnSendTelegramRequestDelegate (DateTime                      LogTimestamp,
                                                            TelegramStore                 Sender,
                                                            EventTracking_Id              EventTrackingId,
                                                            I18NString                    Message,
                                                            IEnumerable<String>           Usernames);

        public event OnSendTelegramRequestDelegate OnSendTelegramRequest;


        public delegate Task OnSendTelegramResponseDelegate(DateTime                      LogTimestamp,
                                                            TelegramStore                 Sender,
                                                            EventTracking_Id              EventTrackingId,
                                                            I18NString                    Message,
                                                            IEnumerable<String>           Usernames,
                                                            IEnumerable<MessageEnvelop>   Responses,
                                                            TimeSpan                      Runtime);

        public event OnSendTelegramResponseDelegate OnSendTelegramResponse;

        #endregion

        #region Constructor(s)

        public TelegramStore(TelegramBotClient TelegramAPI)

        {

            this.TelegramAPI     = TelegramAPI;
            this.UserByUsername  = new Dictionary<String, TelegramUser>();
            this.UserByChatId    = new Dictionary<Int64,  TelegramUser>();
            this.GroupByTitle    = new Dictionary<String, TelegramGroup>();
            this.GroupByChatId   = new Dictionary<Int64,  TelegramGroup>();

            try
            {

                foreach (var line in File.ReadLines("TelegramStore.csv"))
                {
                    if (line.IsNotNullOrEmpty() && !line.StartsWith("//") && !line.StartsWith("#"))
                    {
                        try
                        {

                            var elements = line.Trim().Split(US);

                            switch (elements[0])
                            {

                                case "updateUser":
                                    UpdateUser(Int32.Parse(elements[1]),
                                               elements[2],
                                               elements[3],
                                               elements[4],
                                               Int64.Parse(elements[5]));
                                    break;

                                case "updateGroup":
                                    UpdateUser(Int32.Parse(elements[1]),
                                               elements[2],
                                               elements[3],
                                               elements[4],
                                               Int64.Parse(elements[5]));
                                    break;

                            }

                        }
                        catch (Exception)
                        { }
                    }
                }

            }
            catch (Exception)
            { }

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

                            File.AppendAllText("TelegramStore.csv",
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

                                File.AppendAllText("TelegramStore.csv",
                                                   String.Concat("updateUser", US, UserId, US, Username, US, Firstname, US, Lastname, US, ChatId, Environment.NewLine));

                            }

                        }

                    }
                    catch (Exception)
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

                            File.AppendAllText("TelegramStore.csv",
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

                                File.AppendAllText("TelegramStore.csv",
                                                   String.Concat("updateGroup", US, ChatId, US, Title, US, InviteLink, Environment.NewLine));

                            }

                        }

                    }
                    catch (Exception)
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

                if (messageChat != null)
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
            catch (Exception)
            { }

        }



        #region SendTelegram (Message, Username)

        /// <summary>
        /// Send a Telegram to the given user.
        /// </summary>
        /// <param name="Message">The text of the message.</param>
        /// <param name="Username">The name of the user.</param>
        public async Task<MessageEnvelop> SendTelegram(String  Message,
                                                       String  Username)
        {

            #region Initial checks

            Message   = Message?.Trim();
            Username  = Username?.Trim();

            if (Message.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Message),   "The given message must not be null or empty!");

            if (Username.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Username),  "The given username must not be null or empty!");

            MessageEnvelop responseMessage;

            #endregion

            var eventTrackingId  = EventTracking_Id.New;
            var message          = I18NString.Create(Languages.en, Message);
            var usernames        = new String[] { Username };

            #region Send OnSendTelegramRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                if (OnSendTelegramRequest != null)
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
                DebugX.Log(e, nameof(TelegramStore) + "." + nameof(OnSendTelegramRequest));
            }

            #endregion


            if (UserByUsername.TryGetValue(Username, out TelegramUser User))
            {
                responseMessage = new MessageEnvelop(Username,
                                                     User.ChatId,
                                                     await TelegramAPI.SendTextMessageAsync(
                                                                           ChatId:  User.ChatId,
                                                                           Text:    Message
                                                                       ));
            }

            else
                responseMessage = new MessageEnvelop(Username,
                                                     new Telegram.Bot.Types.Message() {
                                                         Text = "Unknown Telegram user '" + Username + "'!"
                                                     });


            #region Send OnSendTelegramResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                if (OnSendTelegramResponse != null)
                    await Task.WhenAll(OnSendTelegramResponse.GetInvocationList().
                                       Cast<OnSendTelegramResponseDelegate>().
                                       Select(e => e(Endtime,
                                                     this,
                                                     eventTrackingId,
                                                     message,
                                                     usernames,
                                                     new MessageEnvelop[] { responseMessage },
                                                     Endtime - StartTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TelegramStore) + "." + nameof(OnSendTelegramResponse));
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
                                                       String      Username)
        {

            #region Initial checks

            Username  = Username?.Trim();

            if (Message.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Message),   "The given message must not be null or empty!");

            if (Username.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Username),  "The given username must not be null or empty!");

            MessageEnvelop responseMessage;

            #endregion

            var eventTrackingId  = EventTracking_Id.New;
            var usernames        = new String[] { Username };

            #region Send OnSendTelegramRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                if (OnSendTelegramRequest != null)
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
                DebugX.Log(e, nameof(TelegramStore) + "." + nameof(OnSendTelegramRequest));
            }

            #endregion


            if (UserByUsername.TryGetValue(Username, out TelegramUser User))
                responseMessage = new MessageEnvelop(Username,
                                                     User.ChatId,
                                                     await TelegramAPI.SendTextMessageAsync(
                                                                           ChatId:  User.ChatId,
                                                                           Text:    Message[User.PreferredLanguage] ?? Message[Languages.en]
                                                                       ));

            else
                responseMessage = new MessageEnvelop(Username,
                                                     new Telegram.Bot.Types.Message() {
                                                         Text = "Unknown Telegram user '" + Username + "'!"
                                                     });


            #region Send OnSendTelegramResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                if (OnSendTelegramResponse != null)
                    await Task.WhenAll(OnSendTelegramResponse.GetInvocationList().
                                       Cast<OnSendTelegramResponseDelegate>().
                                       Select(e => e(Endtime,
                                                     this,
                                                     eventTrackingId,
                                                     Message,
                                                     usernames,
                                                     new MessageEnvelop[] { responseMessage },
                                                     Endtime - StartTime))).
                                       ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DebugX.Log(e, nameof(TelegramStore) + "." + nameof(OnSendTelegramResponse));
            }

            #endregion

            return responseMessage;

        }

        #endregion

        #region SendTelegrams(Message, Usernames)

        /// <summary>
        /// Send a Telegram to the given users.
        /// </summary>
        /// <param name="Message">The text of the message.</param>
        /// <param name="Usernames">An enumeration of usernames.</param>
        public Task<IEnumerable<MessageEnvelop>> SendTelegrams(String           Message,
                                                               params String[]  Usernames)

            => SendTelegrams(Message, Usernames as IEnumerable<String>);


        /// <summary>
        /// Send a Telegram to the given users.
        /// </summary>
        /// <param name="Message">The text of the message.</param>
        /// <param name="Usernames">An enumeration of usernames.</param>
        public async Task<IEnumerable<MessageEnvelop>> SendTelegrams(String               Message,
                                                                     IEnumerable<String>  Usernames)
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
            var message          = I18NString.Create(Languages.en, Message);

            #region Send OnSendTelegramRequest event

            var StartTime = DateTime.UtcNow;

            try
            {

                if (OnSendTelegramRequest != null)
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
                DebugX.Log(e, nameof(TelegramStore) + "." + nameof(OnSendTelegramRequest));
            }

            #endregion


            foreach (var username in Usernames)
            {

                if (UserByUsername.TryGetValue(username, out TelegramUser User))
                    responseMessages.Add(new MessageEnvelop(username,
                                                            User.ChatId,
                                                            await TelegramAPI.SendTextMessageAsync(
                                                                                  ChatId:  User.ChatId,
                                                                                  Text:    Message
                                                                              )));

                else
                    responseMessages.Add(new MessageEnvelop(username,
                                                            new Telegram.Bot.Types.Message() {
                                                                Text = "Unknown Telegram user '" + username + "'!"
                                                            }));

            }


            #region Send OnSendTelegramResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                if (OnSendTelegramResponse != null)
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
                DebugX.Log(e, nameof(TelegramStore) + "." + nameof(OnSendTelegramResponse));
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
                                                              params String[]  Usernames)

            => SendTelegram(Message, Usernames as IEnumerable<String>);


        /// <summary>
        /// Send a multi-language Telegram to the given users in their preferred language.
        /// </summary>
        /// <param name="Message">The multi-language text of the message.</param>
        /// <param name="Usernames">An enumeration of usernames.</param>
        public async Task<IEnumerable<MessageEnvelop>> SendTelegram(I18NString           Message,
                                                                    IEnumerable<String>  Usernames)
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

            var StartTime = DateTime.UtcNow;

            try
            {

                if (OnSendTelegramRequest != null)
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
                DebugX.Log(e, nameof(TelegramStore) + "." + nameof(OnSendTelegramRequest));
            }

            #endregion


            foreach (var username in Usernames)
            {

                if (UserByUsername.TryGetValue(username, out TelegramUser User))
                    responseMessages.Add(new MessageEnvelop(username,
                                                            User.ChatId,
                                                            await TelegramAPI.SendTextMessageAsync(
                                                                                  ChatId:  User.ChatId,
                                                                                  Text:    Message[User.PreferredLanguage] ?? Message[Languages.en]
                                                                              )));

                else
                    responseMessages.Add(new MessageEnvelop(username,
                                                            new Telegram.Bot.Types.Message() {
                                                                Text = "Unknown Telegram user '" + username + "'!"
                                                            }));

            }


            #region Send OnSendTelegramResponse event

            var Endtime = DateTime.UtcNow;

            try
            {

                if (OnSendTelegramResponse != null)
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
                DebugX.Log(e, nameof(TelegramStore) + "." + nameof(OnSendTelegramResponse));
            }

            #endregion

            return responseMessages;

        }

        #endregion


    }

}
