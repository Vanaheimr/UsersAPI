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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// The current status of a service ticket.
    /// </summary>
    public class ServiceTicketStatus : ACustomData,
                                       IEquatable <ServiceTicketStatus>,
                                       IComparable<ServiceTicketStatus>
    {

        #region Properties

        /// <summary>
        /// The unique identification of the service ticket.
        /// </summary>
        public ServiceTicket_Id                       Id       { get; }

        /// <summary>
        /// The current status of the service ticket.
        /// </summary>
        public Timestamped<ServiceTicketStatusTypes>  Status   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new service ticket status.
        /// </summary>
        /// <param name="Id">The unique identification of the service ticket.</param>
        /// <param name="Status">The current timestamped status of the service ticket.</param>
        /// <param name="CustomData">An optional dictionary of customer-specific data.</param>
        public ServiceTicketStatus(ServiceTicket_Id                       Id,
                                   Timestamped<ServiceTicketStatusTypes>  Status,
                                   IReadOnlyDictionary<String, Object>    CustomData  = null)

            : base(CustomData)

        {

            this.Id      = Id;
            this.Status  = Status;

        }

        #endregion


        #region (static) Snapshot(ServiceTicket)

        /// <summary>
        /// Take a snapshot of the current service ticket status.
        /// </summary>
        /// <param name="ServiceTicket">A service ticket.</param>
        public static ServiceTicketStatus Snapshot(ServiceTicket ServiceTicket)

            => new ServiceTicketStatus(ServiceTicket.Id,
                                       ServiceTicket.Status);

        #endregion


        #region Operator overloading

        #region Operator == (ServiceTicketStatus1, ServiceTicketStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketStatus1">A service ticket status.</param>
        /// <param name="ServiceTicketStatus2">Another service ticket status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ServiceTicketStatus ServiceTicketStatus1, ServiceTicketStatus ServiceTicketStatus2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ServiceTicketStatus1, ServiceTicketStatus2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ServiceTicketStatus1 == null) || ((Object) ServiceTicketStatus2 == null))
                return false;

            return ServiceTicketStatus1.Equals(ServiceTicketStatus2);

        }

        #endregion

        #region Operator != (ServiceTicketStatus1, ServiceTicketStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketStatus1">A service ticket status.</param>
        /// <param name="ServiceTicketStatus2">Another service ticket status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ServiceTicketStatus ServiceTicketStatus1, ServiceTicketStatus ServiceTicketStatus2)
            => !(ServiceTicketStatus1 == ServiceTicketStatus2);

        #endregion

        #region Operator <  (ServiceTicketStatus1, ServiceTicketStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketStatus1">A service ticket status.</param>
        /// <param name="ServiceTicketStatus2">Another service ticket status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ServiceTicketStatus ServiceTicketStatus1, ServiceTicketStatus ServiceTicketStatus2)
        {

            if ((Object) ServiceTicketStatus1 == null)
                throw new ArgumentNullException(nameof(ServiceTicketStatus1), "The given ServiceTicketStatus1 must not be null!");

            return ServiceTicketStatus1.CompareTo(ServiceTicketStatus2) < 0;

        }

        #endregion

        #region Operator <= (ServiceTicketStatus1, ServiceTicketStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketStatus1">A service ticket status.</param>
        /// <param name="ServiceTicketStatus2">Another service ticket status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ServiceTicketStatus ServiceTicketStatus1, ServiceTicketStatus ServiceTicketStatus2)
            => !(ServiceTicketStatus1 > ServiceTicketStatus2);

        #endregion

        #region Operator >  (ServiceTicketStatus1, ServiceTicketStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketStatus1">A service ticket status.</param>
        /// <param name="ServiceTicketStatus2">Another service ticket status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ServiceTicketStatus ServiceTicketStatus1, ServiceTicketStatus ServiceTicketStatus2)
        {

            if ((Object) ServiceTicketStatus1 == null)
                throw new ArgumentNullException(nameof(ServiceTicketStatus1), "The given ServiceTicketStatus1 must not be null!");

            return ServiceTicketStatus1.CompareTo(ServiceTicketStatus2) > 0;

        }

        #endregion

        #region Operator >= (ServiceTicketStatus1, ServiceTicketStatus2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketStatus1">A service ticket status.</param>
        /// <param name="ServiceTicketStatus2">Another service ticket status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ServiceTicketStatus ServiceTicketStatus1, ServiceTicketStatus ServiceTicketStatus2)
            => !(ServiceTicketStatus1 < ServiceTicketStatus2);

        #endregion

        #endregion

        #region IComparable<ServiceTicketStatus> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is ServiceTicketStatus))
                throw new ArgumentException("The given object is not a ServiceTicketStatus!",
                                            nameof(Object));

            return CompareTo((ServiceTicketStatus) Object);

        }

        #endregion

        #region CompareTo(ServiceTicketStatus)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketStatus">An object to compare with.</param>
        public Int32 CompareTo(ServiceTicketStatus ServiceTicketStatus)
        {

            if ((Object) ServiceTicketStatus == null)
                throw new ArgumentNullException(nameof(ServiceTicketStatus), "The given ServiceTicketStatus must not be null!");

            // Compare ServiceTicket Ids
            var _Result = Id.CompareTo(ServiceTicketStatus.Id);

            // If equal: Compare ServiceTicket status
            if (_Result == 0)
                _Result = Status.CompareTo(ServiceTicketStatus.Status);

            return _Result;

        }

        #endregion

        #endregion

        #region IEquatable<ServiceTicketStatus> Members

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

            if (!(Object is ServiceTicketStatus))
                return false;

            return Equals((ServiceTicketStatus) Object);

        }

        #endregion

        #region Equals(ServiceTicketStatus)

        /// <summary>
        /// Compares two ServiceTicket identifications for equality.
        /// </summary>
        /// <param name="ServiceTicketStatus">A service ticket identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ServiceTicketStatus ServiceTicketStatus)
        {

            if ((Object) ServiceTicketStatus == null)
                return false;

            return Id.    Equals(ServiceTicketStatus.Id) &&
                   Status.Equals(ServiceTicketStatus.Status);

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

                return Id.    GetHashCode() * 5 ^
                       Status.GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Id, " -> ",
                             Status.Value,
                             " since ",
                             Status.Timestamp.ToIso8601());

        #endregion

    }

}
