/*
 * Copyright (c) 2014-2020, Achim 'ahzf' Friedland <achim@graphdefined.org>
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// A reference to a service ticket history entry.
    /// </summary>
    public class ServiceTicketHistoryReference
    {

        #region Properties

        /// <summary>
        /// The unique identification of the service ticket.
        /// </summary>
        public ServiceTicket_Id  Id            { get; }

        /// <summary>
        /// The current status of the service ticket.
        /// </summary>
        public I18NString        Description   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a reference to a service ticket history entry.
        /// </summary>
        /// <param name="Id">The unique identification of the service ticket.</param>
        public ServiceTicketHistoryReference(ServiceTicket_Id  Id,
                                             I18NString        Description = null)
        {

            this.Id           = Id;
            this.Description  = Description;

        }

        #endregion


        //#region Operator overloading

        //#region Operator == (ServiceTicketHistoryReference1, ServiceTicketHistoryReference2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="ServiceTicketHistoryReference1">A service ticket status.</param>
        ///// <param name="ServiceTicketHistoryReference2">Another service ticket status.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator == (ServiceTicketHistoryReference ServiceTicketHistoryReference1, ServiceTicketHistoryReference ServiceTicketHistoryReference2)
        //{

        //    // If both are null, or both are same instance, return true.
        //    if (Object.ReferenceEquals(ServiceTicketHistoryReference1, ServiceTicketHistoryReference2))
        //        return true;

        //    // If one is null, but not both, return false.
        //    if (((Object) ServiceTicketHistoryReference1 == null) || ((Object) ServiceTicketHistoryReference2 == null))
        //        return false;

        //    return ServiceTicketHistoryReference1.Equals(ServiceTicketHistoryReference2);

        //}

        //#endregion

        //#region Operator != (ServiceTicketHistoryReference1, ServiceTicketHistoryReference2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="ServiceTicketHistoryReference1">A service ticket status.</param>
        ///// <param name="ServiceTicketHistoryReference2">Another service ticket status.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator != (ServiceTicketHistoryReference ServiceTicketHistoryReference1, ServiceTicketHistoryReference ServiceTicketHistoryReference2)
        //    => !(ServiceTicketHistoryReference1 == ServiceTicketHistoryReference2);

        //#endregion

        //#region Operator <  (ServiceTicketHistoryReference1, ServiceTicketHistoryReference2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="ServiceTicketHistoryReference1">A service ticket status.</param>
        ///// <param name="ServiceTicketHistoryReference2">Another service ticket status.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator < (ServiceTicketHistoryReference ServiceTicketHistoryReference1, ServiceTicketHistoryReference ServiceTicketHistoryReference2)
        //{

        //    if ((Object) ServiceTicketHistoryReference1 == null)
        //        throw new ArgumentNullException(nameof(ServiceTicketHistoryReference1), "The given ServiceTicketHistoryReference1 must not be null!");

        //    return ServiceTicketHistoryReference1.CompareTo(ServiceTicketHistoryReference2) < 0;

        //}

        //#endregion

        //#region Operator <= (ServiceTicketHistoryReference1, ServiceTicketHistoryReference2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="ServiceTicketHistoryReference1">A service ticket status.</param>
        ///// <param name="ServiceTicketHistoryReference2">Another service ticket status.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator <= (ServiceTicketHistoryReference ServiceTicketHistoryReference1, ServiceTicketHistoryReference ServiceTicketHistoryReference2)
        //    => !(ServiceTicketHistoryReference1 > ServiceTicketHistoryReference2);

        //#endregion

        //#region Operator >  (ServiceTicketHistoryReference1, ServiceTicketHistoryReference2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="ServiceTicketHistoryReference1">A service ticket status.</param>
        ///// <param name="ServiceTicketHistoryReference2">Another service ticket status.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator > (ServiceTicketHistoryReference ServiceTicketHistoryReference1, ServiceTicketHistoryReference ServiceTicketHistoryReference2)
        //{

        //    if ((Object) ServiceTicketHistoryReference1 == null)
        //        throw new ArgumentNullException(nameof(ServiceTicketHistoryReference1), "The given ServiceTicketHistoryReference1 must not be null!");

        //    return ServiceTicketHistoryReference1.CompareTo(ServiceTicketHistoryReference2) > 0;

        //}

        //#endregion

        //#region Operator >= (ServiceTicketHistoryReference1, ServiceTicketHistoryReference2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="ServiceTicketHistoryReference1">A service ticket status.</param>
        ///// <param name="ServiceTicketHistoryReference2">Another service ticket status.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator >= (ServiceTicketHistoryReference ServiceTicketHistoryReference1, ServiceTicketHistoryReference ServiceTicketHistoryReference2)
        //    => !(ServiceTicketHistoryReference1 < ServiceTicketHistoryReference2);

        //#endregion

        //#endregion

        #region IComparable<ServiceTicketHistoryReference> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is ServiceTicketHistoryReference))
                throw new ArgumentException("The given object is not a ServiceTicketHistoryReference!",
                                            nameof(Object));

            return CompareTo((ServiceTicketHistoryReference) Object);

        }

        #endregion

        #region CompareTo(ServiceTicketHistoryReference)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketHistoryReference">An object to compare with.</param>
        public Int32 CompareTo(ServiceTicketHistoryReference ServiceTicketHistoryReference)
        {

            if ((Object) ServiceTicketHistoryReference == null)
                throw new ArgumentNullException(nameof(ServiceTicketHistoryReference), "The given ServiceTicketHistoryReference must not be null!");

            // Compare ServiceTicket Ids
            var _Result = Id.CompareTo(ServiceTicketHistoryReference.Id);

            //// If equal: Compare ServiceTicket status
            //if (_Result == 0)
            //    _Result = Description.CompareTo(ServiceTicketHistoryReference.Description);

            return _Result;

        }

        #endregion

        #endregion

        #region IEquatable<ServiceTicketHistoryReference> Members

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

            if (!(Object is ServiceTicketHistoryReference))
                return false;

            return Equals((ServiceTicketHistoryReference) Object);

        }

        #endregion

        #region Equals(ServiceTicketHistoryReference)

        /// <summary>
        /// Compares two ServiceTicket identifications for equality.
        /// </summary>
        /// <param name="ServiceTicketHistoryReference">A service ticket identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ServiceTicketHistoryReference ServiceTicketHistoryReference)
        {

            if ((Object) ServiceTicketHistoryReference == null)
                return false;

            return Id.Equals(ServiceTicketHistoryReference.Id);// &&
//                   Status.Equals(ServiceTicketHistoryReference.Status);

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
