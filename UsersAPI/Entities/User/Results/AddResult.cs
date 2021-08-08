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

    public class AddResult<T>
    {

        public T           Object              { get; }

        public Boolean     IsSuccess           { get; }

        public String      Argument            { get; }

        public I18NString  ErrorDescription    { get; }


        public AddResult(T           Object,
                         Boolean     IsSuccess,
                         String      Argument          = null,
                         I18NString  ErrorDescription  = null)
        {

            this.Object            = Object;
            this.IsSuccess         = IsSuccess;
            this.Argument          = Argument;
            this.ErrorDescription  = ErrorDescription;

        }


        public static AddResult<T> Success<T>(T Object)

            => new AddResult<T>(Object,
                                true);


        public static AddResult<T> ArgumentError<T>(T       Object,
                                                    String  Argument,
                                                    String  Description)

            => new AddResult<T>(Object,
                                false,
                                Argument,
                                I18NString.Create(Languages.en,
                                                  Description));

        public static AddResult<T> ArgumentError<T>(T           Object,
                                                    String      Argument,
                                                    I18NString  Description)

            => new AddResult<T>(Object,
                                false,
                                Argument,
                                Description);

        public static AddResult<T> Failed<T>(T           Object,
                                             I18NString  Description)

            => new AddResult<T>(Object,
                                false,
                                null,
                                Description);

        public static AddResult<T> Failed<T>(T          Object,
                                             Exception  Exception)

            => new AddResult<T>(Object,
                                false,
                                null,
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
