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
                                           String.Concat("UpdateUser", US, UserId, US, Username, US, Firstname, US, Lastname, US, ChatId, Environment.NewLine));

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
                                               String.Concat(UserId, US, Username, US, Firstname, US, Lastname, US, ChatId, Environment.NewLine));

                        }

                    }

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
                                           String.Concat("UpdateGroup", US, ChatId, US, Title, US, InviteLink, Environment.NewLine));

                    }

                    else
                    {

                        if (existingTelegramGroup.Title      != Title ||
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
                                               String.Concat("UpdateGroup", US, ChatId, US, Title, US, InviteLink, Environment.NewLine));

                        }

                    }

                }
            }

        }

        #endregion


        internal async void ReceiveTelegramMessage(Object Sender, Telegram.Bot.Args.MessageEventArgs e)
        {

            var messageFrom = e?.Message?.From;
            var messageChat = e?.Message?.Chat;
            var messageText = e?.Message?.Text;

            if (messageText.IsNeitherNullNorEmpty())
                messageText.Trim();

            if (messageChat != null)
            {

                if (messageChat.Id >= 0)
                {

                    Console.WriteLine($"Received a telegram text message from {e.Message.From.Username} in chat {e.Message.Chat.Id}.");

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

                    Console.WriteLine($"Received a telegram text message from {e.Message.From.Username} in group chat '{e.Message.Chat.Title}' / {e.Message.Chat.Id}.");

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


        public async void SendTelegram(String               MessageText,
                                       IEnumerable<String>  Usernames)
        {

            foreach (var username in Usernames)
            {
                if (UserByUsername.TryGetValue(username, out TelegramUser User))
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
