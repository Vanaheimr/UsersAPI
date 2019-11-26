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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// The unique identification of a service ticket.
    /// </summary>
    public struct ServiceTicket_Id : IId,
                                     IEquatable<ServiceTicket_Id>,
                                     IComparable<ServiceTicket_Id>
    {

        #region Data

        private static readonly Random _random = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String  InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the service ticket identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new service ticket identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the service ticket identification.</param>
        private ServiceTicket_Id(String  String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Random(Length)

        /// <summary>
        /// Create a new service ticket identification.
        /// </summary>
        /// <param name="Length">The expected length of the service ticket identification.</param>
        public static ServiceTicket_Id Random(Byte Length = 15)

            => new ServiceTicket_Id(_random.RandomString(Length).ToUpper());

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a service ticket identification.
        /// </summary>
        /// <param name="Text">A text representation of a service ticket identification.</param>
        public static ServiceTicket_Id Parse(String Text)
        {

            if (TryParse(Text, out ServiceTicket_Id _ServiceTicketId))
                return _ServiceTicketId;

            throw new ArgumentException("The given text representation of a service ticket identification is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a service ticket identification.
        /// </summary>
        /// <param name="Text">A text representation of a service ticket identification.</param>
        public static ServiceTicket_Id? TryParse(String Text)
        {

            if (TryParse(Text, out ServiceTicket_Id _ServiceTicketId))
                return _ServiceTicketId;

            return new ServiceTicket_Id?();

        }

        #endregion

        #region (static) TryParse(Text, out ServiceTicketId)

        /// <summary>
        /// Try to parse the given string as a service ticket identification.
        /// </summary>
        /// <param name="Text">A text representation of a service ticket identification.</param>
        /// <param name="ServiceTicketId">The parsed service ticket identification.</param>
        public static Boolean TryParse(String Text, out ServiceTicket_Id ServiceTicketId)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a service ticket identification must not be null or empty!");

            #endregion

            try
            {
                ServiceTicketId = new ServiceTicket_Id(Text);
                return true;
            }
            catch (Exception)
            {
                ServiceTicketId = default(ServiceTicket_Id);
                return false;
            }

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this service ticket identification.
        /// </summary>

        public ServiceTicket_Id Clone

            => new ServiceTicket_Id(
                   new String(InternalId.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (ServiceTicketId1, ServiceTicketId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketId1">A service ticket identification.</param>
        /// <param name="ServiceTicketId2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ServiceTicket_Id ServiceTicketId1, ServiceTicket_Id ServiceTicketId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ServiceTicketId1, ServiceTicketId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ServiceTicketId1 == null) || ((Object) ServiceTicketId2 == null))
                return false;

            return ServiceTicketId1.Equals(ServiceTicketId2);

        }

        #endregion

        #region Operator != (ServiceTicketId1, ServiceTicketId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketId1">A service ticket identification.</param>
        /// <param name="ServiceTicketId2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ServiceTicket_Id ServiceTicketId1, ServiceTicket_Id ServiceTicketId2)
            => !(ServiceTicketId1 == ServiceTicketId2);

        #endregion

        #region Operator <  (ServiceTicketId1, ServiceTicketId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketId1">A service ticket identification.</param>
        /// <param name="ServiceTicketId2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ServiceTicket_Id ServiceTicketId1, ServiceTicket_Id ServiceTicketId2)
        {

            if ((Object) ServiceTicketId1 == null)
                throw new ArgumentNullException(nameof(ServiceTicketId1), "The given ServiceTicketId1 must not be null!");

            return ServiceTicketId1.CompareTo(ServiceTicketId2) < 0;

        }

        #endregion

        #region Operator <= (ServiceTicketId1, ServiceTicketId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketId1">A service ticket identification.</param>
        /// <param name="ServiceTicketId2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ServiceTicket_Id ServiceTicketId1, ServiceTicket_Id ServiceTicketId2)
            => !(ServiceTicketId1 > ServiceTicketId2);

        #endregion

        #region Operator >  (ServiceTicketId1, ServiceTicketId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketId1">A service ticket identification.</param>
        /// <param name="ServiceTicketId2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ServiceTicket_Id ServiceTicketId1, ServiceTicket_Id ServiceTicketId2)
        {

            if ((Object) ServiceTicketId1 == null)
                throw new ArgumentNullException(nameof(ServiceTicketId1), "The given ServiceTicketId1 must not be null!");

            return ServiceTicketId1.CompareTo(ServiceTicketId2) > 0;

        }

        #endregion

        #region Operator >= (ServiceTicketId1, ServiceTicketId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketId1">A service ticket identification.</param>
        /// <param name="ServiceTicketId2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ServiceTicket_Id ServiceTicketId1, ServiceTicket_Id ServiceTicketId2)
            => !(ServiceTicketId1 < ServiceTicketId2);

        #endregion

        #endregion

        #region IComparable<ServiceTicketId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is ServiceTicket_Id))
                throw new ArgumentException("The given object is not a service ticket identification!",
                                            nameof(Object));

            return CompareTo((ServiceTicket_Id) Object);

        }

        #endregion

        #region CompareTo(ServiceTicketId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketId">An object to compare with.</param>
        public Int32 CompareTo(ServiceTicket_Id ServiceTicketId)
        {

            if ((Object) ServiceTicketId == null)
                throw new ArgumentNullException(nameof(ServiceTicketId),  "The given service ticket identification must not be null!");

            return String.Compare(InternalId, ServiceTicketId.InternalId, StringComparison.OrdinalIgnoreCase);

        }

        #endregion

        #endregion

        #region IEquatable<ServiceTicketId> Members

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

            if (!(Object is ServiceTicket_Id))
                return false;

            return Equals((ServiceTicket_Id) Object);

        }

        #endregion

        #region Equals(ServiceTicketId)

        /// <summary>
        /// Compares two service ticket identifications for equality.
        /// </summary>
        /// <param name="ServiceTicketId">An service ticket identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ServiceTicket_Id ServiceTicketId)
        {

            if ((Object) ServiceTicketId == null)
                return false;

            return InternalId.Equals(ServiceTicketId.InternalId, StringComparison.OrdinalIgnoreCase);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => InternalId.ToLower().GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => InternalId;

        #endregion

    }

}
