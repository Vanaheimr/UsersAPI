/*
 * Copyright (c) 2014-2018, Achim 'ahzf' Friedland <achim@graphdefined.org>
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

namespace org.GraphDefined.OpenData.Users
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
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// The length of the organization identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId.Length;

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

            return String.Compare(InternalId, OrganizationId.InternalId, StringComparison.Ordinal);

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

            return InternalId.Equals(OrganizationId.InternalId);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => InternalId.GetHashCode();

        #endregion

        #region ToString()

        /// <summary>
        /// Return a string representation of this object.
        /// </summary>
        public override String ToString()
            => InternalId;

        #endregion

    }

}
