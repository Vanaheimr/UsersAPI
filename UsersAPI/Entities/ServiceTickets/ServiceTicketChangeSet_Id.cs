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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// The unique identification of a service ticket change set.
    /// </summary>
    public readonly struct ServiceTicketChangeSet_Id : IId,
                                                       IEquatable<ServiceTicketChangeSet_Id>,
                                                       IComparable<ServiceTicketChangeSet_Id>
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
        /// The length of the service ticket change set identificator.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId?.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new service ticket change set identification based on the given string.
        /// </summary>
        private ServiceTicketChangeSet_Id(String Text)
        {
            InternalId = Text;
        }

        #endregion


        #region (static) Random(Length)

        /// <summary>
        /// Create a new service ticket change set identification.
        /// </summary>
        /// <param name="Length">The expected length of the service ticket change set identification.</param>
        public static ServiceTicketChangeSet_Id Random(Byte Length = 15)

            => new (RandomExtensions.RandomString(Length).ToUpper());

        #endregion

        #region Parse   (Text)

        /// <summary>
        /// Parse the given string as a service ticket change set identification.
        /// </summary>
        /// <param name="Text">A text representation of a service ticket change set identification.</param>
        public static ServiceTicketChangeSet_Id Parse(String Text)
        {

            if (TryParse(Text, out ServiceTicketChangeSet_Id serviceTicketChangeSetId))
                return serviceTicketChangeSetId;

            throw new ArgumentException("Invalid text representation of a service ticket change set identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a service ticket change set identification.
        /// </summary>
        /// <param name="Text">A text representation of a service ticket change set identification.</param>
        public static ServiceTicketChangeSet_Id? TryParse(String Text)
        {

            if (TryParse(Text, out ServiceTicketChangeSet_Id serviceTicketChangeSetId))
                return serviceTicketChangeSetId;

            return null;

        }

        #endregion

        #region TryParse(Text, out ServiceTicketChangeSetId)

        /// <summary>
        /// Try to parse the given string as a service ticket change set identification.
        /// </summary>
        /// <param name="Text">A text representation of a service ticket change set identification.</param>
        /// <param name="ServiceTicketChangeSetId">The parsed service ticket change set identification.</param>
        public static Boolean TryParse(String Text, out ServiceTicketChangeSet_Id ServiceTicketChangeSetId)
        {

            Text = Text?.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ServiceTicketChangeSetId = new ServiceTicketChangeSet_Id(Text);
                    return true;
                }
                catch
                { }
            }

            ServiceTicketChangeSetId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this service ticket change set identification.
        /// </summary>
        public ServiceTicketChangeSet_Id Clone

            => new ServiceTicketChangeSet_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (ServiceTicketChangeSetIdId1, ServiceTicketChangeSetIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSetIdId1">A service ticket change set identification.</param>
        /// <param name="ServiceTicketChangeSetIdId2">Another service ticket change set identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ServiceTicketChangeSet_Id ServiceTicketChangeSetIdId1,
                                           ServiceTicketChangeSet_Id ServiceTicketChangeSetIdId2)

            => ServiceTicketChangeSetIdId1.Equals(ServiceTicketChangeSetIdId2);

        #endregion

        #region Operator != (ServiceTicketChangeSetIdId1, ServiceTicketChangeSetIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSetIdId1">A service ticket change set identification.</param>
        /// <param name="ServiceTicketChangeSetIdId2">Another service ticket change set identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ServiceTicketChangeSet_Id ServiceTicketChangeSetIdId1,
                                           ServiceTicketChangeSet_Id ServiceTicketChangeSetIdId2)

            => !ServiceTicketChangeSetIdId1.Equals(ServiceTicketChangeSetIdId2);

        #endregion

        #region Operator <  (ServiceTicketChangeSetIdId1, ServiceTicketChangeSetIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSetIdId1">A service ticket change set identification.</param>
        /// <param name="ServiceTicketChangeSetIdId2">Another service ticket change set identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ServiceTicketChangeSet_Id ServiceTicketChangeSetIdId1,
                                          ServiceTicketChangeSet_Id ServiceTicketChangeSetIdId2)

            => ServiceTicketChangeSetIdId1.CompareTo(ServiceTicketChangeSetIdId2) < 0;

        #endregion

        #region Operator <= (ServiceTicketChangeSetIdId1, ServiceTicketChangeSetIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSetIdId1">A service ticket change set identification.</param>
        /// <param name="ServiceTicketChangeSetIdId2">Another service ticket change set identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ServiceTicketChangeSet_Id ServiceTicketChangeSetIdId1,
                                           ServiceTicketChangeSet_Id ServiceTicketChangeSetIdId2)

            => ServiceTicketChangeSetIdId1.CompareTo(ServiceTicketChangeSetIdId2) <= 0;

        #endregion

        #region Operator >  (ServiceTicketChangeSetIdId1, ServiceTicketChangeSetIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSetIdId1">A service ticket change set identification.</param>
        /// <param name="ServiceTicketChangeSetIdId2">Another service ticket change set identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ServiceTicketChangeSet_Id ServiceTicketChangeSetIdId1,
                                          ServiceTicketChangeSet_Id ServiceTicketChangeSetIdId2)

            => ServiceTicketChangeSetIdId1.CompareTo(ServiceTicketChangeSetIdId2) > 0;

        #endregion

        #region Operator >= (ServiceTicketChangeSetIdId1, ServiceTicketChangeSetIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSetIdId1">A service ticket change set identification.</param>
        /// <param name="ServiceTicketChangeSetIdId2">Another service ticket change set identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ServiceTicketChangeSet_Id ServiceTicketChangeSetIdId1,
                                           ServiceTicketChangeSet_Id ServiceTicketChangeSetIdId2)

            => ServiceTicketChangeSetIdId1.CompareTo(ServiceTicketChangeSetIdId2) >= 0;

        #endregion

        #endregion

        #region IComparable<ServiceTicketChangeSetId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is ServiceTicketChangeSet_Id serviceTicketChangeSetId
                   ? CompareTo(serviceTicketChangeSetId)
                   : throw new ArgumentException("The given object is not a service ticket change set identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ServiceTicketChangeSetId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ServiceTicketChangeSetId">An object to compare with.</param>
        public Int32 CompareTo(ServiceTicketChangeSet_Id ServiceTicketChangeSetId)

            => String.Compare(InternalId,
                              ServiceTicketChangeSetId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

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

            => Object is ServiceTicketChangeSet_Id serviceTicketChangeSetId &&
                   Equals(serviceTicketChangeSetId);

        #endregion

        #region Equals(ServiceTicketChangeSetId)

        /// <summary>
        /// Compares two ServiceTicketChangeSetIds for equality.
        /// </summary>
        /// <param name="ServiceTicketChangeSetId">A service ticket change set identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ServiceTicketChangeSet_Id ServiceTicketChangeSetId)

            => String.Equals(InternalId,
                             ServiceTicketChangeSetId.InternalId,
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
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
