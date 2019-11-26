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
    /// The unique identification of a service ticket history entry.
    /// </summary>
    public struct ServiceTicketHistory_Id : IId,
                                            IEquatable<ServiceTicketHistory_Id>,
                                            IComparable<ServiceTicketHistory_Id>
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
        /// The length of the service ticket history entry identification.
        /// </summary>
        public UInt64 Length

            => (UInt64) InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new service ticket history entry identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the service ticket history entry identification.</param>
        private ServiceTicketHistory_Id(String  String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Random(Length)

        /// <summary>
        /// Create a new service ticket history entry identification.
        /// </summary>
        /// <param name="Length">The expected length of the service ticket history entry identification.</param>
        public static ServiceTicketHistory_Id Random(Byte Length = 10)

            => new ServiceTicketHistory_Id(_random.RandomString(Length).ToUpper());

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a service ticket history entry identification.
        /// </summary>
        /// <param name="Text">A text representation of a service ticket history entry identification.</param>
        public static ServiceTicketHistory_Id Parse(String Text)
        {

            if (TryParse(Text, out ServiceTicketHistory_Id _ServiceTicketHistoryId))
                return _ServiceTicketHistoryId;

            throw new ArgumentException("The given text representation of a service ticket history entry identification is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a service ticket history entry identification.
        /// </summary>
        /// <param name="Text">A text representation of a service ticket history entry identification.</param>
        public static ServiceTicketHistory_Id? TryParse(String Text)
        {

            if (TryParse(Text, out ServiceTicketHistory_Id _ServiceTicketHistoryId))
                return _ServiceTicketHistoryId;

            return new ServiceTicketHistory_Id?();

        }

        #endregion

        #region (static) TryParse(Text, out ServiceTicketHistoryId)

        /// <summary>
        /// Try to parse the given string as a service ticket history entry identification.
        /// </summary>
        /// <param name="Text">A text representation of a service ticket history entry identification.</param>
        /// <param name="ServiceTicketHistoryId">The parsed service ticket history entry identification.</param>
        public static Boolean TryParse(String Text, out ServiceTicketHistory_Id ServiceTicketHistoryId)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a service ticket history entry identification must not be null or empty!");

            #endregion

            try
            {
                ServiceTicketHistoryId = new ServiceTicketHistory_Id(Text);
                return true;
            }
            catch (Exception)
            {
                ServiceTicketHistoryId = default(ServiceTicketHistory_Id);
                return false;
            }

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this service ticket history entry identification.
        /// </summary>

        public ServiceTicketHistory_Id Clone

            => new ServiceTicketHistory_Id(
                   new String(InternalId.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (ServiceTicketHistoryId1, ServiceTicketHistoryId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketHistoryId1">A service ticket history entry identification.</param>
        /// <param name="ServiceTicketHistoryId2">Another service ticket history entry identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ServiceTicketHistory_Id ServiceTicketHistoryId1, ServiceTicketHistory_Id ServiceTicketHistoryId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ServiceTicketHistoryId1, ServiceTicketHistoryId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ServiceTicketHistoryId1 == null) || ((Object) ServiceTicketHistoryId2 == null))
                return false;

            return ServiceTicketHistoryId1.Equals(ServiceTicketHistoryId2);

        }

        #endregion

        #region Operator != (ServiceTicketHistoryId1, ServiceTicketHistoryId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketHistoryId1">A service ticket history entry identification.</param>
        /// <param name="ServiceTicketHistoryId2">Another service ticket history entry identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ServiceTicketHistory_Id ServiceTicketHistoryId1, ServiceTicketHistory_Id ServiceTicketHistoryId2)
            => !(ServiceTicketHistoryId1 == ServiceTicketHistoryId2);

        #endregion

        #region Operator <  (ServiceTicketHistoryId1, ServiceTicketHistoryId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketHistoryId1">A service ticket history entry identification.</param>
        /// <param name="ServiceTicketHistoryId2">Another service ticket history entry identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ServiceTicketHistory_Id ServiceTicketHistoryId1, ServiceTicketHistory_Id ServiceTicketHistoryId2)
        {

            if ((Object) ServiceTicketHistoryId1 == null)
                throw new ArgumentNullException(nameof(ServiceTicketHistoryId1), "The given ServiceTicketHistoryId1 must not be null!");

            return ServiceTicketHistoryId1.CompareTo(ServiceTicketHistoryId2) < 0;

        }

        #endregion

        #region Operator <= (ServiceTicketHistoryId1, ServiceTicketHistoryId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketHistoryId1">A service ticket history entry identification.</param>
        /// <param name="ServiceTicketHistoryId2">Another service ticket history entry identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ServiceTicketHistory_Id ServiceTicketHistoryId1, ServiceTicketHistory_Id ServiceTicketHistoryId2)
            => !(ServiceTicketHistoryId1 > ServiceTicketHistoryId2);

        #endregion

        #region Operator >  (ServiceTicketHistoryId1, ServiceTicketHistoryId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketHistoryId1">A service ticket history entry identification.</param>
        /// <param name="ServiceTicketHistoryId2">Another service ticket history entry identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ServiceTicketHistory_Id ServiceTicketHistoryId1, ServiceTicketHistory_Id ServiceTicketHistoryId2)
        {

            if ((Object) ServiceTicketHistoryId1 == null)
                throw new ArgumentNullException(nameof(ServiceTicketHistoryId1), "The given ServiceTicketHistoryId1 must not be null!");

            return ServiceTicketHistoryId1.CompareTo(ServiceTicketHistoryId2) > 0;

        }

        #endregion

        #region Operator >= (ServiceTicketHistoryId1, ServiceTicketHistoryId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketHistoryId1">A service ticket history entry identification.</param>
        /// <param name="ServiceTicketHistoryId2">Another service ticket history entry identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ServiceTicketHistory_Id ServiceTicketHistoryId1, ServiceTicketHistory_Id ServiceTicketHistoryId2)
            => !(ServiceTicketHistoryId1 < ServiceTicketHistoryId2);

        #endregion

        #endregion

        #region IComparable<ServiceTicketHistoryId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is ServiceTicketHistory_Id))
                throw new ArgumentException("The given object is not a service ticket history entry identification!",
                                            nameof(Object));

            return CompareTo((ServiceTicketHistory_Id) Object);

        }

        #endregion

        #region CompareTo(ServiceTicketHistoryId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketHistoryId">An object to compare with.</param>
        public Int32 CompareTo(ServiceTicketHistory_Id ServiceTicketHistoryId)
        {

            if ((Object) ServiceTicketHistoryId == null)
                throw new ArgumentNullException(nameof(ServiceTicketHistoryId),  "The given service ticket history entry identification must not be null!");

            return String.Compare(InternalId, ServiceTicketHistoryId.InternalId, StringComparison.OrdinalIgnoreCase);

        }

        #endregion

        #endregion

        #region IEquatable<ServiceTicketHistoryId> Members

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

            if (!(Object is ServiceTicketHistory_Id))
                return false;

            return Equals((ServiceTicketHistory_Id) Object);

        }

        #endregion

        #region Equals(ServiceTicketHistoryId)

        /// <summary>
        /// Compares two service ticket history entry identifications for equality.
        /// </summary>
        /// <param name="ServiceTicketHistoryId">An service ticket history entry identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ServiceTicketHistory_Id ServiceTicketHistoryId)
        {

            if ((Object) ServiceTicketHistoryId == null)
                return false;

            return InternalId.Equals(ServiceTicketHistoryId.InternalId, StringComparison.OrdinalIgnoreCase);

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
