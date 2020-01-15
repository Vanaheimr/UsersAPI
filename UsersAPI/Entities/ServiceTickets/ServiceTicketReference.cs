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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// The current status of a service ticket.
    /// </summary>
    public class ServiceTicketReference
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public const String JSONLDContext = "opendata.social/contexts/UsersAPI+json/serviceTicketReference";

        #endregion

        #region Properties

        /// <summary>
        /// The unique identification of the service ticket reference.
        /// </summary>
        public ServiceTicket_Id  Id            { get; }

        /// <summary>
        /// A description of the service ticket reference.
        /// </summary>
        public I18NString        Description   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a reference to a service ticket.
        /// </summary>
        /// <param name="Id">The unique identification of the service ticket reference.</param>
        /// <param name="Description">A description of the service ticket reference.</param>
        public ServiceTicketReference(ServiceTicket_Id  Id,
                                      I18NString        Description = null)
        {

            this.Id           = Id;
            this.Description  = Description;

        }

        #endregion


        #region ToJSON(...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        public JObject ToJSON(Boolean Embedded  = false)

            => JSONObject.Create(

                   Id.ToJSON("@id"),

                   Embedded
                       ? null
                       : new JProperty("@context",  JSONLDContext),

                   Description.IsNeitherNullNorEmpty()
                          ? Description.ToJSON("description")
                          : null

               );

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out ServiceTicketComment, out ErrorResponse)

        /// <summary>
        /// Try to parse the given service ticket comment JSON.
        /// </summary>
        /// <param name="JSONObject">A JSON object.</param>
        /// <param name="ServiceTicketReference">The parsed service ticket reference.</param>
        /// <param name="ErrorResponse">An error message.</param>
        public static Boolean TryParseJSON(JObject                     JSONObject,
                                           out ServiceTicketReference  ServiceTicketReference,
                                           out String                  ErrorResponse)
        {

            try
            {

                ServiceTicketReference = null;

                if (JSONObject?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse Context           [mandatory]

                if (JSONObject.ParseOptional("@context",
                                             out String Context,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                    if (Context != JSONLDContext)
                    {
                        ErrorResponse = @"The given JSON-LD ""@context"" information '" + Context + "' is not supported!";
                        return false;
                    }

                }

                #endregion

                #region Parse ServiceTicketId   [mandatory]

                if (!JSONObject.ParseMandatory("serviceTicketId",
                                               "service ticket identification",
                                               ServiceTicket_Id.TryParse,
                                               out ServiceTicket_Id ServiceTicketId,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Comment           [optional]

                if (JSONObject.ParseOptional("comment",
                                             "comment",
                                             out I18NString Comment,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                ServiceTicketReference = new ServiceTicketReference(ServiceTicketId,
                                                                    Comment);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse  = e.Message;
                ServiceTicketReference = null;
                return false;
            }

        }

        #endregion


        //#region Operator overloading

        //#region Operator == (ServiceTicketReference1, ServiceTicketReference2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="ServiceTicketReference1">A service ticket status.</param>
        ///// <param name="ServiceTicketReference2">Another service ticket status.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator == (ServiceTicketReference ServiceTicketReference1, ServiceTicketReference ServiceTicketReference2)
        //{

        //    // If both are null, or both are same instance, return true.
        //    if (Object.ReferenceEquals(ServiceTicketReference1, ServiceTicketReference2))
        //        return true;

        //    // If one is null, but not both, return false.
        //    if (((Object) ServiceTicketReference1 == null) || ((Object) ServiceTicketReference2 == null))
        //        return false;

        //    return ServiceTicketReference1.Equals(ServiceTicketReference2);

        //}

        //#endregion

        //#region Operator != (ServiceTicketReference1, ServiceTicketReference2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="ServiceTicketReference1">A service ticket status.</param>
        ///// <param name="ServiceTicketReference2">Another service ticket status.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator != (ServiceTicketReference ServiceTicketReference1, ServiceTicketReference ServiceTicketReference2)
        //    => !(ServiceTicketReference1 == ServiceTicketReference2);

        //#endregion

        //#region Operator <  (ServiceTicketReference1, ServiceTicketReference2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="ServiceTicketReference1">A service ticket status.</param>
        ///// <param name="ServiceTicketReference2">Another service ticket status.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator < (ServiceTicketReference ServiceTicketReference1, ServiceTicketReference ServiceTicketReference2)
        //{

        //    if ((Object) ServiceTicketReference1 == null)
        //        throw new ArgumentNullException(nameof(ServiceTicketReference1), "The given ServiceTicketReference1 must not be null!");

        //    return ServiceTicketReference1.CompareTo(ServiceTicketReference2) < 0;

        //}

        //#endregion

        //#region Operator <= (ServiceTicketReference1, ServiceTicketReference2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="ServiceTicketReference1">A service ticket status.</param>
        ///// <param name="ServiceTicketReference2">Another service ticket status.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator <= (ServiceTicketReference ServiceTicketReference1, ServiceTicketReference ServiceTicketReference2)
        //    => !(ServiceTicketReference1 > ServiceTicketReference2);

        //#endregion

        //#region Operator >  (ServiceTicketReference1, ServiceTicketReference2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="ServiceTicketReference1">A service ticket status.</param>
        ///// <param name="ServiceTicketReference2">Another service ticket status.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator > (ServiceTicketReference ServiceTicketReference1, ServiceTicketReference ServiceTicketReference2)
        //{

        //    if ((Object) ServiceTicketReference1 == null)
        //        throw new ArgumentNullException(nameof(ServiceTicketReference1), "The given ServiceTicketReference1 must not be null!");

        //    return ServiceTicketReference1.CompareTo(ServiceTicketReference2) > 0;

        //}

        //#endregion

        //#region Operator >= (ServiceTicketReference1, ServiceTicketReference2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="ServiceTicketReference1">A service ticket status.</param>
        ///// <param name="ServiceTicketReference2">Another service ticket status.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator >= (ServiceTicketReference ServiceTicketReference1, ServiceTicketReference ServiceTicketReference2)
        //    => !(ServiceTicketReference1 < ServiceTicketReference2);

        //#endregion

        //#endregion

        #region IComparable<ServiceTicketReference> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is ServiceTicketReference))
                throw new ArgumentException("The given object is not a ServiceTicketReference!",
                                            nameof(Object));

            return CompareTo((ServiceTicketReference) Object);

        }

        #endregion

        #region CompareTo(ServiceTicketReference)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketReference">An object to compare with.</param>
        public Int32 CompareTo(ServiceTicketReference ServiceTicketReference)
        {

            if ((Object) ServiceTicketReference == null)
                throw new ArgumentNullException(nameof(ServiceTicketReference), "The given ServiceTicketReference must not be null!");

            // Compare ServiceTicket Ids
            var _Result = Id.CompareTo(ServiceTicketReference.Id);

            //// If equal: Compare ServiceTicket status
            //if (_Result == 0)
            //    _Result = Description.CompareTo(ServiceTicketReference.Description);

            return _Result;

        }

        #endregion

        #endregion

        #region IEquatable<ServiceTicketReference> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object == null)
                return false;

            if (!(Object is ServiceTicketReference))
                return false;

            return Equals((ServiceTicketReference) Object);

        }

        #endregion

        #region Equals(ServiceTicketReference)

        /// <summary>
        /// Compares two ServiceTicket identifications for equality.
        /// </summary>
        /// <param name="ServiceTicketReference">A service ticket identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ServiceTicketReference ServiceTicketReference)
        {

            if ((Object) ServiceTicketReference == null)
                return false;

            return Id.Equals(ServiceTicketReference.Id);// &&
//                   Status.Equals(ServiceTicketReference.Status);

        }

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

                return Id.GetHashCode();// * 5 ^
//                       Status.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Id,
                             Description.IsNeitherNullNorEmpty()
                                 ? Description.ToString()
                                 : "");

        #endregion

    }

}
