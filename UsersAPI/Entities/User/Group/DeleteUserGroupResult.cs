/*
 * Copyright (c) 2014-2021, Achim 'ahzf' Friedland <achim@graphdefined.org>
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace social.OpenData.UsersAPI
{

    public class DeleteUserGroupResult
    {

        public Boolean     IsSuccess           { get; }

        public I18NString  ErrorDescription    { get; }


        private DeleteUserGroupResult(Boolean     IsSuccess,
                                      I18NString  ErrorDescription  = null)
        {
            this.IsSuccess         = IsSuccess;
            this.ErrorDescription  = ErrorDescription;
        }


        public static DeleteUserGroupResult Success

            => new DeleteUserGroupResult(true);

        public static DeleteUserGroupResult Failed(I18NString Reason)

            => new DeleteUserGroupResult(false,
                                         Reason);

        public static DeleteUserGroupResult Failed(Exception Exception)

            => new DeleteUserGroupResult(false,
                                         I18NString.Create(Languages.en,
                                                           Exception.Message));

        public override String ToString()

            => IsSuccess
                    ? "Success"
                    : "Failed" + (ErrorDescription.IsNullOrEmpty()
                                        ? ": " + ErrorDescription.FirstText()
                                        : "!");

    }

}
