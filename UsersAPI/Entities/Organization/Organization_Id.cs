﻿/*
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
    /// The unique identification of an organization.
    /// </summary>
    public struct Organization_Id : IId,
                                    IEquatable<Organization_Id>,
                                    IComparable<Organization_Id>
    {

        #region Data

        /// <summary>
        /// Private non-cryptographic random number generator.
        /// </summary>
        private static readonly Random _random = new Random(DateTime.Now.Millisecond);

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
        /// The length of the organization identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId?.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new organization identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the organization identification.</param>
        private Organization_Id(String String)
        {
            InternalId = String;
        }

        #endregion


        #region (static) Random(Length)

        /// <summary>
        /// Create a new user identification.
        /// </summary>
        /// <param name="Length">The expected length of the user identification.</param>
        public static Organization_Id Random(Byte Length = 15)

            => new Organization_Id(_random.RandomString(Length).ToUpper());

        #endregion

        #region (static) Parse(Text)

        /// <summary>
        /// Parse the given string as an organization identification.
        /// </summary>
        /// <param name="Text">A text representation of an organization identification.</param>
        public static Organization_Id Parse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an organization identification must not be null or empty!");

            #endregion

            return new Organization_Id(Text);

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as an organization identification.
        /// </summary>
        /// <param name="Text">A text representation of an organization identification.</param>
        public static Organization_Id? TryParse(String Text)
        {

            if (TryParse(Text, out Organization_Id _OrganizationId))
                return _OrganizationId;

            return new Organization_Id?();

        }

        #endregion

        #region (static) TryParse(Text, out OrganizationId)

        /// <summary>
        /// Try to parse the given string as an organization identification.
        /// </summary>
        /// <param name="Text">A text representation of an organization identification.</param>
        /// <param name="OrganizationId">The parsed organization identification.</param>
        public static Boolean TryParse(String Text, out Organization_Id OrganizationId)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an organization identification must not be null or empty!");

            #endregion

            try
            {
                OrganizationId = new Organization_Id(Text);
                return true;
            }
            catch (Exception)
            {
                OrganizationId = default(Organization_Id);
                return false;
            }

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone an organization identification.
        /// </summary>

        public Organization_Id Clone
            => new Organization_Id(new String(InternalId.ToCharArray()));

        #endregion


        #region IndexOf(Text)

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string
        /// in the current System.String object. A parameter specifies the type of search
        /// to use for the specified string.
        /// </summary>
        /// <param name="Text">The string to seek.</param>
        /// <returns>
        /// The index position of the value parameter if that string is found, or -1 if it
        /// is not. If value is System.String.Empty, the return value is 0.
        /// </returns>
        public Int32 IndexOf(String Text)
            => InternalId.IndexOf(Text, StringComparison.OrdinalIgnoreCase);

        #endregion


        #region Operator overloading

        #region Operator == (OrganizationId1, OrganizationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OrganizationId1">A organization identification.</param>
        /// <param name="OrganizationId2">Another organization identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Organization_Id OrganizationId1, Organization_Id OrganizationId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(OrganizationId1, OrganizationId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) OrganizationId1 == null) || ((Object) OrganizationId2 == null))
                return false;

            return OrganizationId1.Equals(OrganizationId2);

        }

        #endregion

        #region Operator != (OrganizationId1, OrganizationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OrganizationId1">A organization identification.</param>
        /// <param name="OrganizationId2">Another organization identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Organization_Id OrganizationId1, Organization_Id OrganizationId2)
            => !(OrganizationId1 == OrganizationId2);

        #endregion

        #region Operator <  (OrganizationId1, OrganizationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OrganizationId1">A organization identification.</param>
        /// <param name="OrganizationId2">Another organization identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Organization_Id OrganizationId1, Organization_Id OrganizationId2)
        {

            if ((Object) OrganizationId1 == null)
                throw new ArgumentNullException(nameof(OrganizationId1), "The given OrganizationId1 must not be null!");

            return OrganizationId1.CompareTo(OrganizationId2) < 0;

        }

        #endregion

        #region Operator <= (OrganizationId1, OrganizationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OrganizationId1">A organization identification.</param>
        /// <param name="OrganizationId2">Another organization identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Organization_Id OrganizationId1, Organization_Id OrganizationId2)
            => !(OrganizationId1 > OrganizationId2);

        #endregion

        #region Operator >  (OrganizationId1, OrganizationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OrganizationId1">A organization identification.</param>
        /// <param name="OrganizationId2">Another organization identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Organization_Id OrganizationId1, Organization_Id OrganizationId2)
        {

            if ((Object) OrganizationId1 == null)
                throw new ArgumentNullException(nameof(OrganizationId1), "The given OrganizationId1 must not be null!");

            return OrganizationId1.CompareTo(OrganizationId2) > 0;

        }

        #endregion

        #region Operator >= (OrganizationId1, OrganizationId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OrganizationId1">A organization identification.</param>
        /// <param name="OrganizationId2">Another organization identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Organization_Id OrganizationId1, Organization_Id OrganizationId2)
            => !(OrganizationId1 < OrganizationId2);

        #endregion

        #endregion

        #region IComparable<OrganizationId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is Organization_Id))
                throw new ArgumentException("The given object is not an organization identification!",
                                            nameof(Object));

            return CompareTo((Organization_Id) Object);

        }

        #endregion

        #region CompareTo(OrganizationId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OrganizationId">An object to compare with.</param>
        public Int32 CompareTo(Organization_Id OrganizationId)
        {

            if ((Object) OrganizationId == null)
                throw new ArgumentNullException(nameof(OrganizationId),  "The given organization identification must not be null!");

            return String.Compare(InternalId, OrganizationId.InternalId, StringComparison.OrdinalIgnoreCase);

        }

        #endregion

        #endregion

        #region IEquatable<OrganizationId> Members

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

            if (!(Object is Organization_Id))
                return false;

            return Equals((Organization_Id) Object);

        }

        #endregion

        #region Equals(OrganizationId)

        /// <summary>
        /// Compares two organization identifications for equality.
        /// </summary>
        /// <param name="OrganizationId">An organization identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Organization_Id OrganizationId)
        {

            if ((Object) OrganizationId == null)
                return false;

            return InternalId.ToLower().Equals(OrganizationId.InternalId?.ToLower());

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
