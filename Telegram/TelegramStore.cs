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
using System.Collections.Generic;
using System.IO;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Illias;
using Telegram.Bot;

#endregion

namespace org.GraphDefined.OpenData.Users
{

    public class TelegramStore
    {

        public class TelegramUser
        {

            public Int32   UserId       { get; }
            public String  Username     { get; }
            public String  Firstname    { get; }
            public String  Lastname     { get; }
            public Int64   ChatId       { get; }

            public TelegramUser(Int32   UserId,
                                String  Username,
                                String  Firstname,
                                String  Lastname,
                                Int64   ChatId)
            {

                this.UserId     = UserId;
                this.Username   = Username;
                this.Firstname  = Firstname;
                this.Lastname   = Lastname;
                this.ChatId     = ChatId;

            }

        }


        #region Data

        /// <summary>
        /// ASCII unit/cell separator
        /// </summary>
        protected const  Char                              US = (Char) 0x1F;

        private readonly TelegramBotClient                 TelegramAPI;

        private readonly Dictionary<String, TelegramUser>  ByUsername;

        private readonly Dictionary<Int64,  TelegramUser>  ByChatId;

        #endregion

        #region Properties

        /// <summary>
        /// The linked UsersAPI.
        /// </summary>
        public UsersAPI  UsersAPI  { get; }

        #endregion

        #region Constructor(s)

        public TelegramStore(TelegramBotClient TelegramAPI)

        {

            this.TelegramAPI  = TelegramAPI;
            this.ByUsername   = new Dictionary<String, TelegramUser>();
            this.ByChatId     = new Dictionary<Int64,  TelegramUser>();

            foreach (var line in File.ReadLines("TelegramStore.csv"))
            {
                if (line.IsNotNullOrEmpty() && !line.StartsWith("//") && !line.StartsWith("#"))
                {
                    try
                    {

                        var elements = line.Trim().Split(US);

                        UpdateUser(Int32.Parse(elements[0]),
                                   elements[1],
                                   elements[2],
                                   elements[3],
                                   Int64.Parse(elements[4]));

                    }
                    catch (Exception)
                    { }
                }
            }

        }

        #endregion



        public void UpdateUser(Int32   UserId,
                               String  Username,
                               String  Firstname,
                               String  Lastname,
                               Int64   ChatId)
        {

            lock (ByUsername)
            {
                lock (ByChatId)
                {

                    if (!ByUsername.TryGetValue(Username, out TelegramUser existingTelegramUser))
                    {

                        var newTelegramUser = new TelegramUser(UserId,
                                                               Username,
                                                               Firstname,
                                                               Lastname,
                                                               ChatId);

                        ByUsername.Add(Username, newTelegramUser);

                        if (ByChatId.ContainsKey(ChatId))
                            ByChatId.Remove(ChatId);

                        ByChatId.Add(ChatId, newTelegramUser);

                        File.AppendAllText("TelegramStore.csv",
                                           String.Concat(UserId, US, Username, US, Firstname, US, Lastname, US, ChatId, Environment.NewLine));

                    }

                    else
                    {

                        if (existingTelegramUser.ChatId != ChatId)
                        {

                            ByUsername.Remove(Username);
                            ByChatId.Remove(existingTelegramUser.ChatId);

                            var newTelegramUser = new TelegramUser(UserId,
                                                                   Username,
                                                                   Firstname,
                                                                   Lastname,
                                                                   ChatId);

                            ByUsername.Add(Username, newTelegramUser);

                            if (ByChatId.ContainsKey(ChatId))
                                ByChatId.Remove(ChatId);

                            ByChatId.Add(ChatId, newTelegramUser);

                            File.AppendAllText("TelegramStore.csv",
                                               String.Concat(UserId, US, Username, US, Firstname, US, Lastname, US, ChatId, Environment.NewLine));

                        }

                    }

                }
            }

        }


        internal async void ReceiveTelegramMessage(Object Sender, Telegram.Bot.Args.MessageEventArgs e)
        {

            var messageText = e?.Message?.Text;
            var messageChat = e?.Message?.Chat;

            if (messageText.IsNeitherNullNorEmpty())
                messageText.Trim();

            if (messageText.IsNotNullOrEmpty())
            {

                Console.WriteLine($"Received a telegram text message from {e.Message.From.Username} in chat {e.Message.Chat.Id}.");

                UpdateUser(e.Message.From.Id,
                           e.Message.From.Username,
                           e.Message.From.FirstName,
                           e.Message.From.LastName,
                           e.Message.Chat.Id);

                //await this.TelegramAPI.SendTextMessageAsync(
                //    chatId:  messageChat.Id,
                //    text:    "Hello " + e.Message.From.FirstName + " " + e.Message.From.LastName + "!\nYou said:\n" + e.Message.Text
                //);

            }
        }


        public async void SendTelegram(String               MessageText,
                                       IEnumerable<String>  Usernames)
        {

            foreach (var username in Usernames)
            {
                if (ByUsername.TryGetValue(username, out TelegramUser User))
                {
                    await this.TelegramAPI.SendTextMessageAsync(
                        chatId:  User.ChatId,
                        text:    MessageText
                    );
                }
            }

        }

    }

}
