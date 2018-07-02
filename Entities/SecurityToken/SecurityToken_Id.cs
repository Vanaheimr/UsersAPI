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
using org.GraphDefined.OpenData;

#endregion

namespace org.GraphDefined.OpenData.Users
{

    /// <summary>
    /// The unique identification of a security token.
    /// </summary>
    public struct SecurityToken_Id : IId,
                                     IEquatable<SecurityToken_Id>,
                                     IComparable<SecurityToken_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String  InternalId;

        private static readonly Random _Random = new Random();

        #endregion

        #region Properties

        /// <summary>
        /// The length of the security token identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId.Length);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new unique security token identification based on the given text representation.
        /// </summary>
        /// <param name="String">The text representation of the security token identification.</param>
        private SecurityToken_Id(String  String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Random  (Length = 40, SourceOfRandomness = null)

        /// <summary>
        /// Generate a new random security token identification.
        /// </summary>
        /// <param name="Length">The expected length of the random string.</param>
        /// <param name="SourceOfRandomness">The source of randomness.</param>
        public static SecurityToken_Id Random(UInt16  Length              = 40,
                                              Random  SourceOfRandomness  = null)

            => new SecurityToken_Id((SourceOfRandomness ?? _Random).RandomString(Length));

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given text representation of a security token identification.
        /// </summary>
        /// <param name="Text">A text representation of a security token identification.</param>
        public static SecurityToken_Id Parse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a security token identification must not be null or empty!");

            #endregion

            return new SecurityToken_Id(Text);

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given text representation of a security token identification.
        /// </summary>
        /// <param name="Text">A text representation of a security token identification.</param>
        public static SecurityToken_Id? TryParse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a security token identification must not be null or empty!");

            #endregion

            if (TryParse(Text, out SecurityToken_Id _SecurityTokenId))
                return _SecurityTokenId;

            return new SecurityToken_Id?();

        }

        #endregion

        #region (static) TryParse(Text, out SecurityTokenId)

        /// <summary>
        /// Try to parse the given text representation of a security token identification.
        /// </summary>
        /// <param name="Text">A text representation of a security token identification.</param>
        /// <param name="SecurityTokenId">The parsed security token identification.</param>
        public static Boolean TryParse(String Text, out SecurityToken_Id SecurityTokenId)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a security token identification must not be null or empty!");

            #endregion

            try
            {
                SecurityTokenId = new SecurityToken_Id(Text);
                return true;
            }
            catch (Exception)
            {
                SecurityTokenId = default(SecurityToken_Id);
                return false;
            }

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this security token identification.
        /// </summary>

        public SecurityToken_Id Clone

            => new SecurityToken_Id(
                   new String(InternalId.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (SecurityTokenId1, SecurityTokenId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SecurityTokenId1">A security token identification.</param>
        /// <param name="SecurityTokenId2">Another security token identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (SecurityToken_Id SecurityTokenId1, SecurityToken_Id SecurityTokenId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(SecurityTokenId1, SecurityTokenId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) SecurityTokenId1 == null) || ((Object) SecurityTokenId2 == null))
                return false;

            return SecurityTokenId1.Equals(SecurityTokenId2);

        }

        #endregion

        #region Operator != (SecurityTokenId1, SecurityTokenId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SecurityTokenId1">A security token identification.</param>
        /// <param name="SecurityTokenId2">Another security token identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (SecurityToken_Id SecurityTokenId1, SecurityToken_Id SecurityTokenId2)
            => !(SecurityTokenId1 == SecurityTokenId2);

        #endregion

        #region Operator <  (SecurityTokenId1, SecurityTokenId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SecurityTokenId1">A security token identification.</param>
        /// <param name="SecurityTokenId2">Another security token identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (SecurityToken_Id SecurityTokenId1, SecurityToken_Id SecurityTokenId2)
        {

            if ((Object) SecurityTokenId1 == null)
                throw new ArgumentNullException(nameof(SecurityTokenId1), "The given SecurityTokenId1 must not be null!");

            return SecurityTokenId1.CompareTo(SecurityTokenId2) < 0;

        }

        #endregion

        #region Operator <= (SecurityTokenId1, SecurityTokenId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SecurityTokenId1">A security token identification.</param>
        /// <param name="SecurityTokenId2">Another security token identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (SecurityToken_Id SecurityTokenId1, SecurityToken_Id SecurityTokenId2)
            => !(SecurityTokenId1 > SecurityTokenId2);

        #endregion

        #region Operator >  (SecurityTokenId1, SecurityTokenId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SecurityTokenId1">A security token identification.</param>
        /// <param name="SecurityTokenId2">Another security token identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (SecurityToken_Id SecurityTokenId1, SecurityToken_Id SecurityTokenId2)
        {

            if ((Object) SecurityTokenId1 == null)
                throw new ArgumentNullException(nameof(SecurityTokenId1), "The given SecurityTokenId1 must not be null!");

            return SecurityTokenId1.CompareTo(SecurityTokenId2) > 0;

        }

        #endregion

        #region Operator >= (SecurityTokenId1, SecurityTokenId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SecurityTokenId1">A security token identification.</param>
        /// <param name="SecurityTokenId2">Another security token identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (SecurityToken_Id SecurityTokenId1, SecurityToken_Id SecurityTokenId2)
            => !(SecurityTokenId1 < SecurityTokenId2);

        #endregion

        #endregion

        #region IComparable<SecurityTokenId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is SecurityToken_Id))
                throw new ArgumentException("The given object is not a security token identification!",
                                            nameof(Object));

            return CompareTo((SecurityToken_Id) Object);

        }

        #endregion

        #region CompareTo(SecurityTokenId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="SecurityTokenId">An object to compare with.</param>
        public Int32 CompareTo(SecurityToken_Id SecurityTokenId)
        {

            if ((Object) SecurityTokenId == null)
                throw new ArgumentNullException(nameof(SecurityTokenId),  "The given security token identification must not be null!");

            return String.Compare(InternalId, SecurityTokenId.InternalId, StringComparison.Ordinal);

        }

        #endregion

        #endregion

        #region IEquatable<SecurityTokenId> Members

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

            if (!(Object is SecurityToken_Id))
                return false;

            return Equals((SecurityToken_Id) Object);

        }

        #endregion

        #region Equals(SecurityTokenId)

        /// <summary>
        /// Compares two security token identifications for equality.
        /// </summary>
        /// <param name="SecurityTokenId">An security token identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(SecurityToken_Id SecurityTokenId)
        {

            if ((Object) SecurityTokenId == null)
                return false;

            return InternalId.Equals(SecurityTokenId.InternalId);

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
