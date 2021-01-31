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

    /// <summary>
    /// The unique identification of a service ticket change set.
    /// </summary>
    public struct ServiceTicketChangeSet_Id : IId,
                                              IEquatable<ServiceTicketChangeSet_Id>,
                                              IComparable<ServiceTicketChangeSet_Id>
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
        /// The length of the service ticket change set identification.
        /// </summary>
        public UInt64 Length

            => (UInt64) InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new service ticket change set identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the service ticket change set identification.</param>
        private ServiceTicketChangeSet_Id(String  String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Random(Length)

        /// <summary>
        /// Create a new service ticket change set identification.
        /// </summary>
        /// <param name="Length">The expected length of the service ticket change set identification.</param>
        public static ServiceTicketChangeSet_Id Random(Byte Length = 10)

            => new ServiceTicketChangeSet_Id(_random.RandomString(Length).ToUpper());

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a service ticket change set identification.
        /// </summary>
        /// <param name="Text">A text representation of a service ticket change set identification.</param>
        public static ServiceTicketChangeSet_Id Parse(String Text)
        {

            if (TryParse(Text, out ServiceTicketChangeSet_Id _ServiceTicketChangeSetId))
                return _ServiceTicketChangeSetId;

            throw new ArgumentException("The given text representation of a service ticket change set identification is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a service ticket change set identification.
        /// </summary>
        /// <param name="Text">A text representation of a service ticket change set identification.</param>
        public static ServiceTicketChangeSet_Id? TryParse(String Text)
        {

            if (TryParse(Text, out ServiceTicketChangeSet_Id _ServiceTicketChangeSetId))
                return _ServiceTicketChangeSetId;

            return new ServiceTicketChangeSet_Id?();

        }

        #endregion

        #region (static) TryParse(Text, out ServiceTicketChangeSetId)

        /// <summary>
        /// Try to parse the given string as a service ticket change set identification.
        /// </summary>
        /// <param name="Text">A text representation of a service ticket change set identification.</param>
        /// <param name="ServiceTicketChangeSetId">The parsed service ticket change set identification.</param>
        public static Boolean TryParse(String Text, out ServiceTicketChangeSet_Id ServiceTicketChangeSetId)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a service ticket change set identification must not be null or empty!");

            #endregion

            try
            {
                ServiceTicketChangeSetId = new ServiceTicketChangeSet_Id(Text);
                return true;
            }
            catch (Exception)
            {
                ServiceTicketChangeSetId = default(ServiceTicketChangeSet_Id);
                return false;
            }

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this service ticket change set identification.
        /// </summary>

        public ServiceTicketChangeSet_Id Clone

            => new ServiceTicketChangeSet_Id(
                   new String(InternalId.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (ServiceTicketChangeSetId1, ServiceTicketChangeSetId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSetId1">A service ticket change set identification.</param>
        /// <param name="ServiceTicketChangeSetId2">Another service ticket change set identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ServiceTicketChangeSet_Id ServiceTicketChangeSetId1, ServiceTicketChangeSet_Id ServiceTicketChangeSetId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(ServiceTicketChangeSetId1, ServiceTicketChangeSetId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) ServiceTicketChangeSetId1 == null) || ((Object) ServiceTicketChangeSetId2 == null))
                return false;

            return ServiceTicketChangeSetId1.Equals(ServiceTicketChangeSetId2);

        }

        #endregion

        #region Operator != (ServiceTicketChangeSetId1, ServiceTicketChangeSetId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSetId1">A service ticket change set identification.</param>
        /// <param name="ServiceTicketChangeSetId2">Another service ticket change set identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ServiceTicketChangeSet_Id ServiceTicketChangeSetId1, ServiceTicketChangeSet_Id ServiceTicketChangeSetId2)
            => !(ServiceTicketChangeSetId1 == ServiceTicketChangeSetId2);

        #endregion

        #region Operator <  (ServiceTicketChangeSetId1, ServiceTicketChangeSetId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSetId1">A service ticket change set identification.</param>
        /// <param name="ServiceTicketChangeSetId2">Another service ticket change set identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ServiceTicketChangeSet_Id ServiceTicketChangeSetId1, ServiceTicketChangeSet_Id ServiceTicketChangeSetId2)
        {

            if ((Object) ServiceTicketChangeSetId1 == null)
                throw new ArgumentNullException(nameof(ServiceTicketChangeSetId1), "The given ServiceTicketChangeSetId1 must not be null!");

            return ServiceTicketChangeSetId1.CompareTo(ServiceTicketChangeSetId2) < 0;

        }

        #endregion

        #region Operator <= (ServiceTicketChangeSetId1, ServiceTicketChangeSetId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSetId1">A service ticket change set identification.</param>
        /// <param name="ServiceTicketChangeSetId2">Another service ticket change set identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ServiceTicketChangeSet_Id ServiceTicketChangeSetId1, ServiceTicketChangeSet_Id ServiceTicketChangeSetId2)
            => !(ServiceTicketChangeSetId1 > ServiceTicketChangeSetId2);

        #endregion

        #region Operator >  (ServiceTicketChangeSetId1, ServiceTicketChangeSetId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSetId1">A service ticket change set identification.</param>
        /// <param name="ServiceTicketChangeSetId2">Another service ticket change set identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ServiceTicketChangeSet_Id ServiceTicketChangeSetId1, ServiceTicketChangeSet_Id ServiceTicketChangeSetId2)
        {

            if ((Object) ServiceTicketChangeSetId1 == null)
                throw new ArgumentNullException(nameof(ServiceTicketChangeSetId1), "The given ServiceTicketChangeSetId1 must not be null!");

            return ServiceTicketChangeSetId1.CompareTo(ServiceTicketChangeSetId2) > 0;

        }

        #endregion

        #region Operator >= (ServiceTicketChangeSetId1, ServiceTicketChangeSetId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSetId1">A service ticket change set identification.</param>
        /// <param name="ServiceTicketChangeSetId2">Another service ticket change set identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ServiceTicketChangeSet_Id ServiceTicketChangeSetId1, ServiceTicketChangeSet_Id ServiceTicketChangeSetId2)
            => !(ServiceTicketChangeSetId1 < ServiceTicketChangeSetId2);

        #endregion

        #endregion

        #region IComparable<ServiceTicketChangeSetId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is ServiceTicketChangeSet_Id))
                throw new ArgumentException("The given object is not a service ticket change set identification!",
                                            nameof(Object));

            return CompareTo((ServiceTicketChangeSet_Id) Object);

        }

        #endregion

        #region CompareTo(ServiceTicketChangeSetId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSetId">An object to compare with.</param>
        public Int32 CompareTo(ServiceTicketChangeSet_Id ServiceTicketChangeSetId)
        {

            if ((Object) ServiceTicketChangeSetId == null)
                throw new ArgumentNullException(nameof(ServiceTicketChangeSetId),  "The given service ticket change set identification must not be null!");

            return String.Compare(InternalId, ServiceTicketChangeSetId.InternalId, StringComparison.OrdinalIgnoreCase);

        }

        #endregion

        #endregion

        #region IEquatable<ServiceTicketChangeSetId> Members

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

            if (!(Object is ServiceTicketChangeSet_Id))
                return false;

            return Equals((ServiceTicketChangeSet_Id) Object);

        }

        #endregion

        #region Equals(ServiceTicketChangeSetId)

        /// <summary>
        /// Compares two service ticket change set identifications for equality.
        /// </summary>
        /// <param name="ServiceTicketChangeSetId">An service ticket change set identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ServiceTicketChangeSet_Id ServiceTicketChangeSetId)
        {

            if ((Object) ServiceTicketChangeSetId == null)
                return false;

            return InternalId.Equals(ServiceTicketChangeSetId.InternalId, StringComparison.OrdinalIgnoreCase);

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
