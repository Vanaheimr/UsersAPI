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
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using social.OpenData.UsersAPI;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// The first official response within a service ticket.
    /// </summary>
    public readonly struct FirstResponse : IEquatable<FirstResponse>,
                                           IComparable<FirstResponse>,
                                           IComparable
    {

        #region Properties

        /// <summary>
        /// The time span between the opening of the service ticket
        /// of the first official response.
        /// </summary>
        [Mandatory]
        public TimeSpan                   ResponseTime    { get; }

        /// <summary>
        /// The change set identification of the service ticket
        /// of the first official response.
        /// </summary>
        [Mandatory]
        public ServiceTicketChangeSet_Id  ChangeSetId     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a first official response within a service ticket.
        /// </summary>
        /// <param name="ResponseTime">The time span between the opening of the service ticket of the first official response.</param>
        /// <param name="ChangeSetId">The change set identification of the service ticket of the first official response.</param>
        public FirstResponse(TimeSpan                   ResponseTime,
                             ServiceTicketChangeSet_Id  ChangeSetId)
        {

            this.ResponseTime  = ResponseTime;
            this.ChangeSetId   = ChangeSetId;

        }

        #endregion


        #region (static) Parse   (JSON, CustomFirstResponseParser = null)

        /// <summary>
        /// Parse the given JSON representation of a first response.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="CustomFirstResponseParser">A delegate to parse custom first response JSON objects.</param>
        public static FirstResponse Parse(JObject                                     JSON,
                                          CustomJObjectParserDelegate<FirstResponse>  CustomFirstResponseParser   = null)
        {

            if (TryParse(JSON,
                         out FirstResponse firstResponse,
                         out String        ErrorResponse,
                         CustomFirstResponseParser))
            {
                return firstResponse;
            }

            throw new ArgumentException("The given JSON representation of a first response is invalid: " + ErrorResponse, nameof(JSON));

        }

        #endregion

        #region (static) Parse   (Text, CustomFirstResponseParser = null)

        /// <summary>
        /// Parse the given text representation of a first response.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="CustomFirstResponseParser">A delegate to parse custom first response JSON objects.</param>
        public static FirstResponse Parse(String                                      Text,
                                          CustomJObjectParserDelegate<FirstResponse>  CustomFirstResponseParser   = null)
        {

            if (TryParse(Text,
                         out FirstResponse firstResponse,
                         out String        ErrorResponse,
                         CustomFirstResponseParser))
            {
                return firstResponse;
            }

            throw new ArgumentException("The given text representation of a first response is invalid: " + ErrorResponse, nameof(Text));

        }

        #endregion

        #region (static) TryParse(JSON, out FirstResponse, out ErrorResponse, CustomFirstResponseParser = null)

        // Note: The following is needed to satisfy pattern matching delegates! Do not refactor it!

        /// <summary>
        /// Try to parse the given JSON representation of a first response.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="FirstResponse">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        public static Boolean TryParse(JObject            JSON,
                                       out FirstResponse  FirstResponse,
                                       out String         ErrorResponse)

            => TryParse(JSON,
                        out FirstResponse,
                        out ErrorResponse,
                        null);


        /// <summary>
        /// Try to parse the given JSON representation of a first response.
        /// </summary>
        /// <param name="JSON">The JSON to parse.</param>
        /// <param name="FirstResponse">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomFirstResponseParser">A delegate to parse custom first response JSON objects.</param>
        public static Boolean TryParse(JObject                                     JSON,
                                       out FirstResponse                           FirstResponse,
                                       out String                                  ErrorResponse,
                                       CustomJObjectParserDelegate<FirstResponse>  CustomFirstResponseParser)
        {

            try
            {

                FirstResponse = default;

                if (JSON?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse ResponseTime    [mandatory]

                if (!JSON.ParseMandatory("responseTime",
                                         "response time",
                                         out TimeSpan ResponseTime,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse ChangeSetId     [mandatory]

                if (!JSON.ParseMandatory("changeSetId",
                                         "change set identification",
                                         ServiceTicketChangeSet_Id.TryParse,
                                         out ServiceTicketChangeSet_Id ChangeSetId,
                                         out ErrorResponse))
                {
                    return false;
                }

                #endregion


                FirstResponse = new FirstResponse(ResponseTime,
                                                  ChangeSetId);


                if (CustomFirstResponseParser != null)
                    FirstResponse = CustomFirstResponseParser(JSON,
                                                              FirstResponse);

                return true;

            }
            catch (Exception e)
            {
                FirstResponse  = default;
                ErrorResponse  = "The given JSON representation of a first response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, out FirstResponse, out ErrorResponse, CustomFirstResponseParser = null)

        /// <summary>
        /// Try to parse the given text representation of a first response.
        /// </summary>
        /// <param name="Text">The text to parse.</param>
        /// <param name="FirstResponse">The parsed connector.</param>
        /// <param name="ErrorResponse">An optional error response.</param>
        /// <param name="CustomFirstResponseParser">A delegate to parse custom first response JSON objects.</param>
        public static Boolean TryParse(String                                      Text,
                                       out FirstResponse                           FirstResponse,
                                       out String                                  ErrorResponse,
                                       CustomJObjectParserDelegate<FirstResponse>  CustomFirstResponseParser   = null)
        {

            try
            {

                return TryParse(JObject.Parse(Text),
                                out FirstResponse,
                                out ErrorResponse,
                                CustomFirstResponseParser);

            }
            catch (Exception e)
            {
                FirstResponse  = default;
                ErrorResponse  = "The given text representation of a first response is invalid: " + e.Message;
                return false;
            }

        }

        #endregion

        #region ToJSON(CustomFirstResponseSerializer = null)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="CustomFirstResponseSerializer">A delegate to serialize custom first response JSON objects.</param>
        public JObject ToJSON(CustomJObjectSerializerDelegate<FirstResponse> CustomFirstResponseSerializer = null)
        {

            var JSON = JSONObject.Create(
                           new JProperty("responseTime",  Math.Round(ResponseTime.TotalSeconds, 0)),
                           new JProperty("changeSetId",   ChangeSetId.ToString())
                       );

            return CustomFirstResponseSerializer is not null
                       ? CustomFirstResponseSerializer(this, JSON)
                       : JSON;

        }

        #endregion


        #region Operator overloading

        #region Operator == (FirstResponse1, FirstResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FirstResponse1">A first response.</param>
        /// <param name="FirstResponse2">Another first response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (FirstResponse FirstResponse1,
                                           FirstResponse FirstResponse2)

            => FirstResponse1.Equals(FirstResponse2);

        #endregion

        #region Operator != (FirstResponse1, FirstResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FirstResponse1">A first response.</param>
        /// <param name="FirstResponse2">Another first response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (FirstResponse FirstResponse1,
                                           FirstResponse FirstResponse2)

            => !(FirstResponse1 == FirstResponse2);

        #endregion

        #region Operator <  (FirstResponse1, FirstResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FirstResponse1">A first response.</param>
        /// <param name="FirstResponse2">Another first response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (FirstResponse FirstResponse1,
                                          FirstResponse FirstResponse2)

            => FirstResponse1.CompareTo(FirstResponse2) < 0;

        #endregion

        #region Operator <= (FirstResponse1, FirstResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FirstResponse1">A first response.</param>
        /// <param name="FirstResponse2">Another first response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (FirstResponse FirstResponse1,
                                           FirstResponse FirstResponse2)

            => !(FirstResponse1 > FirstResponse2);

        #endregion

        #region Operator >  (FirstResponse1, FirstResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FirstResponse1">A first response.</param>
        /// <param name="FirstResponse2">Another first response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (FirstResponse FirstResponse1,
                                          FirstResponse FirstResponse2)

            => FirstResponse1.CompareTo(FirstResponse2) > 0;

        #endregion

        #region Operator >= (FirstResponse1, FirstResponse2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FirstResponse1">A first response.</param>
        /// <param name="FirstResponse2">Another first response.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (FirstResponse FirstResponse1,
                                           FirstResponse FirstResponse2)

            => !(FirstResponse1 < FirstResponse2);

        #endregion

        #endregion

        #region IComparable<FirstResponse> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is FirstResponse firstResponse
                   ? CompareTo(firstResponse)
                   : throw new ArgumentException("The given object is not a first response!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(FirstResponse)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FirstResponse">An object to compare with.</param>
        public Int32 CompareTo(FirstResponse FirstResponse)
        {

            var c = ResponseTime.CompareTo(FirstResponse.ResponseTime);

            if (c == 0)
                c = ChangeSetId.CompareTo(FirstResponse.ChangeSetId);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<FirstResponse> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is FirstResponse firstResponse &&
                   Equals(firstResponse);

        #endregion

        #region Equals(FirstResponse)

        /// <summary>
        /// Compares two first responses for equality.
        /// </summary>
        /// <param name="FirstResponse">A first response to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(FirstResponse FirstResponse)

            => ResponseTime.Equals(FirstResponse.ResponseTime) &&
               ChangeSetId. Equals(FirstResponse.ChangeSetId);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return ResponseTime.GetHashCode() * 3 ^
                       ChangeSetId. GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(ResponseTime,
                             " seconds, change set '",
                             ChangeSetId,
                             "'");

        #endregion

    }

}
