/*
 * Copyright (c) 2014-2020, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
    /// The unique identification of an attached file.
    /// </summary>
    public struct AttachedFile_Id : IId,
                                    IEquatable<AttachedFile_Id>,
                                    IComparable<AttachedFile_Id>
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
        private AttachedFile_Id(String  String)
        {
            this.InternalId  = String;
        }

        #endregion


        #region (static) Random(Length)

        /// <summary>
        /// Create a new service ticket identification.
        /// </summary>
        /// <param name="Length">The expected length of the service ticket identification.</param>
        public static AttachedFile_Id Random(Byte Length = 15)

            => new AttachedFile_Id(_random.RandomString(Length).ToUpper());

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as an attached file identification.
        /// </summary>
        /// <param name="Text">A text representation of an attached file identification.</param>
        public static AttachedFile_Id Parse(String Text)
        {

            if (TryParse(Text, out AttachedFile_Id _AttachedFileId))
                return _AttachedFileId;

            throw new ArgumentException("The given text representation of an attached file identification is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as an attached file identification.
        /// </summary>
        /// <param name="Text">A text representation of an attached file identification.</param>
        public static AttachedFile_Id? TryParse(String Text)
        {

            if (TryParse(Text, out AttachedFile_Id _AttachedFileId))
                return _AttachedFileId;

            return new AttachedFile_Id?();

        }

        #endregion

        #region (static) TryParse(Text, out AttachedFileId)

        /// <summary>
        /// Try to parse the given string as an attached file identification.
        /// </summary>
        /// <param name="Text">A text representation of an attached file identification.</param>
        /// <param name="AttachedFileId">The parsed service ticket identification.</param>
        public static Boolean TryParse(String Text, out AttachedFile_Id AttachedFileId)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an attached file identification must not be null or empty!");

            #endregion

            try
            {
                AttachedFileId = new AttachedFile_Id(Text);
                return true;
            }
            catch (Exception)
            {
                AttachedFileId = default(AttachedFile_Id);
                return false;
            }

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this service ticket identification.
        /// </summary>

        public AttachedFile_Id Clone

            => new AttachedFile_Id(
                   new String(InternalId.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (AttachedFileId1, AttachedFileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AttachedFileId1">A service ticket identification.</param>
        /// <param name="AttachedFileId2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AttachedFile_Id AttachedFileId1, AttachedFile_Id AttachedFileId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(AttachedFileId1, AttachedFileId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) AttachedFileId1 == null) || ((Object) AttachedFileId2 == null))
                return false;

            return AttachedFileId1.Equals(AttachedFileId2);

        }

        #endregion

        #region Operator != (AttachedFileId1, AttachedFileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AttachedFileId1">A service ticket identification.</param>
        /// <param name="AttachedFileId2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AttachedFile_Id AttachedFileId1, AttachedFile_Id AttachedFileId2)
            => !(AttachedFileId1 == AttachedFileId2);

        #endregion

        #region Operator <  (AttachedFileId1, AttachedFileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AttachedFileId1">A service ticket identification.</param>
        /// <param name="AttachedFileId2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (AttachedFile_Id AttachedFileId1, AttachedFile_Id AttachedFileId2)
        {

            if ((Object) AttachedFileId1 == null)
                throw new ArgumentNullException(nameof(AttachedFileId1), "The given AttachedFileId1 must not be null!");

            return AttachedFileId1.CompareTo(AttachedFileId2) < 0;

        }

        #endregion

        #region Operator <= (AttachedFileId1, AttachedFileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AttachedFileId1">A service ticket identification.</param>
        /// <param name="AttachedFileId2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (AttachedFile_Id AttachedFileId1, AttachedFile_Id AttachedFileId2)
            => !(AttachedFileId1 > AttachedFileId2);

        #endregion

        #region Operator >  (AttachedFileId1, AttachedFileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AttachedFileId1">A service ticket identification.</param>
        /// <param name="AttachedFileId2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (AttachedFile_Id AttachedFileId1, AttachedFile_Id AttachedFileId2)
        {

            if ((Object) AttachedFileId1 == null)
                throw new ArgumentNullException(nameof(AttachedFileId1), "The given AttachedFileId1 must not be null!");

            return AttachedFileId1.CompareTo(AttachedFileId2) > 0;

        }

        #endregion

        #region Operator >= (AttachedFileId1, AttachedFileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AttachedFileId1">A service ticket identification.</param>
        /// <param name="AttachedFileId2">Another service ticket identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (AttachedFile_Id AttachedFileId1, AttachedFile_Id AttachedFileId2)
            => !(AttachedFileId1 < AttachedFileId2);

        #endregion

        #endregion

        #region IComparable<AttachedFileId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is AttachedFile_Id))
                throw new ArgumentException("The given object is not an attached file identification!",
                                            nameof(Object));

            return CompareTo((AttachedFile_Id) Object);

        }

        #endregion

        #region CompareTo(AttachedFileId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AttachedFileId">An object to compare with.</param>
        public Int32 CompareTo(AttachedFile_Id AttachedFileId)
        {

            if ((Object) AttachedFileId == null)
                throw new ArgumentNullException(nameof(AttachedFileId),  "The given service ticket identification must not be null!");

            return String.Compare(InternalId, AttachedFileId.InternalId, StringComparison.OrdinalIgnoreCase);

        }

        #endregion

        #endregion

        #region IEquatable<AttachedFileId> Members

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

            if (!(Object is AttachedFile_Id))
                return false;

            return Equals((AttachedFile_Id) Object);

        }

        #endregion

        #region Equals(AttachedFileId)

        /// <summary>
        /// Compares two service ticket identifications for equality.
        /// </summary>
        /// <param name="AttachedFileId">An service ticket identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(AttachedFile_Id AttachedFileId)
        {

            if ((Object) AttachedFileId == null)
                return false;

            return InternalId.Equals(AttachedFileId.InternalId, StringComparison.OrdinalIgnoreCase);

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
