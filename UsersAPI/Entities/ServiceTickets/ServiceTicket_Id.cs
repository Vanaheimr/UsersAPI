/*
 * Copyright (c) 2014-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// The unique identification of a service ticket.
    /// </summary>
    public readonly struct ServiceTicket_Id : IId,
                                              IEquatable<ServiceTicket_Id>,
                                              IComparable<ServiceTicket_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the service ticket identificator.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId?.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new service ticket identification based on the given string.
        /// </summary>
        private ServiceTicket_Id(String Text)
        {
            InternalId = Text;
        }

        #endregion


        #region (static) Random(Length)

        /// <summary>
        /// Create a new service ticket identification.
        /// </summary>
        /// <param name="Length">The expected length of the service ticket identification.</param>
        public static ServiceTicket_Id Random(Byte Length = 15)

            => new (RandomExtensions.RandomString(Length).ToUpper());

        #endregion

        #region Parse   (Text)

        /// <summary>
        /// Parse the given string as a service ticket identification.
        /// </summary>
        /// <param name="Text">A text-representation of a service ticket identification.</param>
        public static ServiceTicket_Id Parse(String Text)
        {

            if (TryParse(Text, out ServiceTicket_Id serviceTicketId))
                return serviceTicketId;

            throw new ArgumentException("Invalid text-representation of a service ticket identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a service ticket identification.
        /// </summary>
        /// <param name="Text">A text-representation of a service ticket identification.</param>
        public static ServiceTicket_Id? TryParse(String Text)
        {

            if (TryParse(Text, out ServiceTicket_Id serviceTicketId))
                return serviceTicketId;

            return null;

        }

        #endregion

        #region TryParse(Text, out ServiceTicketId)

        /// <summary>
        /// Try to parse the given string as a service ticket identification.
        /// </summary>
        /// <param name="Text">A text-representation of a service ticket identification.</param>
        /// <param name="ServiceTicketId">The parsed service ticket identification.</param>
        public static Boolean TryParse(String Text, out ServiceTicket_Id ServiceTicketId)
        {

            Text = Text?.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ServiceTicketId = new ServiceTicket_Id(Text);
                    return true;
                }
                catch
                { }
            }

            ServiceTicketId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this service ticket identification.
        /// </summary>
        public ServiceTicket_Id Clone

            => new ServiceTicket_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (ServiceTicketIdId1, ServiceTicketIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketIdId1">A service ticket identification.</param>
        /// <param name="ServiceTicketIdId2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ServiceTicket_Id ServiceTicketIdId1,
                                           ServiceTicket_Id ServiceTicketIdId2)

            => ServiceTicketIdId1.Equals(ServiceTicketIdId2);

        #endregion

        #region Operator != (ServiceTicketIdId1, ServiceTicketIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketIdId1">A service ticket identification.</param>
        /// <param name="ServiceTicketIdId2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ServiceTicket_Id ServiceTicketIdId1,
                                           ServiceTicket_Id ServiceTicketIdId2)

            => !ServiceTicketIdId1.Equals(ServiceTicketIdId2);

        #endregion

        #region Operator <  (ServiceTicketIdId1, ServiceTicketIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketIdId1">A service ticket identification.</param>
        /// <param name="ServiceTicketIdId2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ServiceTicket_Id ServiceTicketIdId1,
                                          ServiceTicket_Id ServiceTicketIdId2)

            => ServiceTicketIdId1.CompareTo(ServiceTicketIdId2) < 0;

        #endregion

        #region Operator <= (ServiceTicketIdId1, ServiceTicketIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketIdId1">A service ticket identification.</param>
        /// <param name="ServiceTicketIdId2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ServiceTicket_Id ServiceTicketIdId1,
                                           ServiceTicket_Id ServiceTicketIdId2)

            => ServiceTicketIdId1.CompareTo(ServiceTicketIdId2) <= 0;

        #endregion

        #region Operator >  (ServiceTicketIdId1, ServiceTicketIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketIdId1">A service ticket identification.</param>
        /// <param name="ServiceTicketIdId2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ServiceTicket_Id ServiceTicketIdId1,
                                          ServiceTicket_Id ServiceTicketIdId2)

            => ServiceTicketIdId1.CompareTo(ServiceTicketIdId2) > 0;

        #endregion

        #region Operator >= (ServiceTicketIdId1, ServiceTicketIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketIdId1">A service ticket identification.</param>
        /// <param name="ServiceTicketIdId2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ServiceTicket_Id ServiceTicketIdId1,
                                           ServiceTicket_Id ServiceTicketIdId2)

            => ServiceTicketIdId1.CompareTo(ServiceTicketIdId2) >= 0;

        #endregion

        #endregion

        #region IComparable<ServiceTicketId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is ServiceTicket_Id serviceTicketId
                   ? CompareTo(serviceTicketId)
                   : throw new ArgumentException("The given object is not a service ticket identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ServiceTicketId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketId">An object to compare with.</param>
        public Int32 CompareTo(ServiceTicket_Id ServiceTicketId)

            => String.Compare(InternalId,
                              ServiceTicketId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

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

            => Object is ServiceTicket_Id serviceTicketId &&
                   Equals(serviceTicketId);

        #endregion

        #region Equals(ServiceTicketId)

        /// <summary>
        /// Compares two ServiceTicketIds for equality.
        /// </summary>
        /// <param name="ServiceTicketId">A service ticket identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ServiceTicket_Id ServiceTicketId)

            => String.Equals(InternalId,
                             ServiceTicketId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text-representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
