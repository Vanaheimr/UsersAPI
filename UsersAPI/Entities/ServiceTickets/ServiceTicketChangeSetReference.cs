/*
 * Copyright (c) 2014-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// A reference to a service ticket change set.
    /// </summary>
    public class ServiceTicketChangeSetReference
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
        /// Create a reference to a service ticket change set.
        /// </summary>
        /// <param name="Id">The unique identification of the service ticket.</param>
        public ServiceTicketChangeSetReference(ServiceTicket_Id  Id,
                                               I18NString        Description = null)
        {

            this.Id           = Id;
            this.Description  = Description;

        }

        #endregion


        //#region Operator overloading

        //#region Operator == (ServiceTicketChangeSetReference1, ServiceTicketChangeSetReference2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="ServiceTicketChangeSetReference1">A service ticket status.</param>
        ///// <param name="ServiceTicketChangeSetReference2">Another service ticket status.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator == (ServiceTicketChangeSetReference ServiceTicketChangeSetReference1, ServiceTicketChangeSetReference ServiceTicketChangeSetReference2)
        //{

        //    // If both are null, or both are same instance, return true.
        //    if (Object.ReferenceEquals(ServiceTicketChangeSetReference1, ServiceTicketChangeSetReference2))
        //        return true;

        //    // If one is null, but not both, return false.
        //    if (((Object) ServiceTicketChangeSetReference1 == null) || ((Object) ServiceTicketChangeSetReference2 == null))
        //        return false;

        //    return ServiceTicketChangeSetReference1.Equals(ServiceTicketChangeSetReference2);

        //}

        //#endregion

        //#region Operator != (ServiceTicketChangeSetReference1, ServiceTicketChangeSetReference2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="ServiceTicketChangeSetReference1">A service ticket status.</param>
        ///// <param name="ServiceTicketChangeSetReference2">Another service ticket status.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator != (ServiceTicketChangeSetReference ServiceTicketChangeSetReference1, ServiceTicketChangeSetReference ServiceTicketChangeSetReference2)
        //    => !(ServiceTicketChangeSetReference1 == ServiceTicketChangeSetReference2);

        //#endregion

        //#region Operator <  (ServiceTicketChangeSetReference1, ServiceTicketChangeSetReference2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="ServiceTicketChangeSetReference1">A service ticket status.</param>
        ///// <param name="ServiceTicketChangeSetReference2">Another service ticket status.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator < (ServiceTicketChangeSetReference ServiceTicketChangeSetReference1, ServiceTicketChangeSetReference ServiceTicketChangeSetReference2)
        //{

        //    if ((Object) ServiceTicketChangeSetReference1 == null)
        //        throw new ArgumentNullException(nameof(ServiceTicketChangeSetReference1), "The given ServiceTicketChangeSetReference1 must not be null!");

        //    return ServiceTicketChangeSetReference1.CompareTo(ServiceTicketChangeSetReference2) < 0;

        //}

        //#endregion

        //#region Operator <= (ServiceTicketChangeSetReference1, ServiceTicketChangeSetReference2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="ServiceTicketChangeSetReference1">A service ticket status.</param>
        ///// <param name="ServiceTicketChangeSetReference2">Another service ticket status.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator <= (ServiceTicketChangeSetReference ServiceTicketChangeSetReference1, ServiceTicketChangeSetReference ServiceTicketChangeSetReference2)
        //    => !(ServiceTicketChangeSetReference1 > ServiceTicketChangeSetReference2);

        //#endregion

        //#region Operator >  (ServiceTicketChangeSetReference1, ServiceTicketChangeSetReference2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="ServiceTicketChangeSetReference1">A service ticket status.</param>
        ///// <param name="ServiceTicketChangeSetReference2">Another service ticket status.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator > (ServiceTicketChangeSetReference ServiceTicketChangeSetReference1, ServiceTicketChangeSetReference ServiceTicketChangeSetReference2)
        //{

        //    if ((Object) ServiceTicketChangeSetReference1 == null)
        //        throw new ArgumentNullException(nameof(ServiceTicketChangeSetReference1), "The given ServiceTicketChangeSetReference1 must not be null!");

        //    return ServiceTicketChangeSetReference1.CompareTo(ServiceTicketChangeSetReference2) > 0;

        //}

        //#endregion

        //#region Operator >= (ServiceTicketChangeSetReference1, ServiceTicketChangeSetReference2)

        ///// <summary>
        ///// Compares two instances of this object.
        ///// </summary>
        ///// <param name="ServiceTicketChangeSetReference1">A service ticket status.</param>
        ///// <param name="ServiceTicketChangeSetReference2">Another service ticket status.</param>
        ///// <returns>true|false</returns>
        //public static Boolean operator >= (ServiceTicketChangeSetReference ServiceTicketChangeSetReference1, ServiceTicketChangeSetReference ServiceTicketChangeSetReference2)
        //    => !(ServiceTicketChangeSetReference1 < ServiceTicketChangeSetReference2);

        //#endregion

        //#endregion

        #region IComparable<ServiceTicketChangeSetReference> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is ServiceTicketChangeSetReference))
                throw new ArgumentException("The given object is not a ServiceTicketChangeSetReference!",
                                            nameof(Object));

            return CompareTo((ServiceTicketChangeSetReference) Object);

        }

        #endregion

        #region CompareTo(ServiceTicketChangeSetReference)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSetReference">An object to compare with.</param>
        public Int32 CompareTo(ServiceTicketChangeSetReference ServiceTicketChangeSetReference)
        {

            if ((Object) ServiceTicketChangeSetReference == null)
                throw new ArgumentNullException(nameof(ServiceTicketChangeSetReference), "The given ServiceTicketChangeSetReference must not be null!");

            // Compare ServiceTicket Ids
            var _Result = Id.CompareTo(ServiceTicketChangeSetReference.Id);

            //// If equal: Compare ServiceTicket status
            //if (_Result == 0)
            //    _Result = Description.CompareTo(ServiceTicketChangeSetReference.Description);

            return _Result;

        }

        #endregion

        #endregion

        #region IEquatable<ServiceTicketChangeSetReference> Members

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

            if (!(Object is ServiceTicketChangeSetReference))
                return false;

            return Equals((ServiceTicketChangeSetReference) Object);

        }

        #endregion

        #region Equals(ServiceTicketChangeSetReference)

        /// <summary>
        /// Compares two ServiceTicket identifications for equality.
        /// </summary>
        /// <param name="ServiceTicketChangeSetReference">A service ticket identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ServiceTicketChangeSetReference ServiceTicketChangeSetReference)
        {

            if ((Object) ServiceTicketChangeSetReference == null)
                return false;

            return Id.Equals(ServiceTicketChangeSetReference.Id);// &&
//                   Status.Equals(ServiceTicketChangeSetReference.Status);

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
