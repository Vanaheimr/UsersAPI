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
using System.Linq;
using Newtonsoft.Json.Linq;
using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace social.OpenData.UsersAPI
{

    public abstract class AResult<T>
    {

        protected T           Object              { get; }

        public    Boolean     IsSuccess           { get; }

        public    String      Argument            { get; }

        public    I18NString  ErrorDescription    { get; }


        public AResult(T           Object,
                       Boolean     IsSuccess,
                       String      Argument          = null,
                       I18NString  ErrorDescription  = null)
        {

            this.Object            = Object;
            this.IsSuccess         = IsSuccess;
            this.Argument          = Argument;
            this.ErrorDescription  = ErrorDescription;

        }

        public JObject ToJSON()

            => JSONObject.Create(
                   ErrorDescription.Count == 1
                       ? new JProperty("description",  ErrorDescription.FirstText())
                       : new JProperty("description",  ErrorDescription.ToJSON())
               );

        public override String ToString()

            => IsSuccess
                    ? "Success"
                    : "Failed" + (ErrorDescription.IsNullOrEmpty()
                                        ? ": " + ErrorDescription.FirstText()
                                        : "!");

    }

}
