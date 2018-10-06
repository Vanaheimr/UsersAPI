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
    /// The unique identification of a group.
    /// </summary>
    public struct Group_Id : IId,
                             IEquatable<Group_Id>,
                             IComparable<Group_Id>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// The length of the group identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId?.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new group identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the group identification.</param>
        private Group_Id(String String)
        {
            InternalId = String;
        }

        #endregion


        #region (static) Parse(Text)

        /// <summary>
        /// Parse the given string as a group identification.
        /// </summary>
        /// <param name="Text">A text representation of a group identification.</param>
        public static Group_Id Parse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a group identification must not be null or empty!");

            #endregion

            return new Group_Id(Text);

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a group identification.
        /// </summary>
        /// <param name="Text">A text representation of a group identification.</param>
        public static Group_Id? TryParse(String Text)
        {

            if (TryParse(Text, out Group_Id _GroupId))
                return _GroupId;

            return new Group_Id?();

        }

        #endregion

        #region (static) TryParse(Text, out GroupId)

        /// <summary>
        /// Try to parse the given string as a group identification.
        /// </summary>
        /// <param name="Text">A text representation of a group identification.</param>
        /// <param name="GroupId">The parsed group identification.</param>
        public static Boolean TryParse(String Text, out Group_Id GroupId)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a group identification must not be null or empty!");

            #endregion

            try
            {
                GroupId = new Group_Id(Text);
                return true;
            }
            catch (Exception)
            {
                GroupId = default(Group_Id);
                return false;
            }

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone a group identification.
        /// </summary>

        public Group_Id Clone
            => new Group_Id(new String(InternalId.ToCharArray()));

        #endregion


        #region Operator overloading

        #region Operator == (GroupId1, GroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GroupId1">A group identification.</param>
        /// <param name="GroupId2">Another group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Group_Id GroupId1, Group_Id GroupId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(GroupId1, GroupId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) GroupId1 == null) || ((Object) GroupId2 == null))
                return false;

            return GroupId1.Equals(GroupId2);

        }

        #endregion

        #region Operator != (GroupId1, GroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GroupId1">A group identification.</param>
        /// <param name="GroupId2">Another group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Group_Id GroupId1, Group_Id GroupId2)
            => !(GroupId1 == GroupId2);

        #endregion

        #region Operator <  (GroupId1, GroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GroupId1">A group identification.</param>
        /// <param name="GroupId2">Another group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Group_Id GroupId1, Group_Id GroupId2)
        {

            if ((Object) GroupId1 == null)
                throw new ArgumentNullException(nameof(GroupId1), "The given GroupId1 must not be null!");

            return GroupId1.CompareTo(GroupId2) < 0;

        }

        #endregion

        #region Operator <= (GroupId1, GroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GroupId1">A group identification.</param>
        /// <param name="GroupId2">Another group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Group_Id GroupId1, Group_Id GroupId2)
            => !(GroupId1 > GroupId2);

        #endregion

        #region Operator >  (GroupId1, GroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GroupId1">A group identification.</param>
        /// <param name="GroupId2">Another group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Group_Id GroupId1, Group_Id GroupId2)
        {

            if ((Object) GroupId1 == null)
                throw new ArgumentNullException(nameof(GroupId1), "The given GroupId1 must not be null!");

            return GroupId1.CompareTo(GroupId2) > 0;

        }

        #endregion

        #region Operator >= (GroupId1, GroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GroupId1">A group identification.</param>
        /// <param name="GroupId2">Another group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Group_Id GroupId1, Group_Id GroupId2)
            => !(GroupId1 < GroupId2);

        #endregion

        #endregion

        #region IComparable<GroupId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is Group_Id))
                throw new ArgumentException("The given object is not a group identification!",
                                            nameof(Object));

            return CompareTo((Group_Id) Object);

        }

        #endregion

        #region CompareTo(GroupId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="GroupId">An object to compare with.</param>
        public Int32 CompareTo(Group_Id GroupId)
        {

            if ((Object) GroupId == null)
                throw new ArgumentNullException(nameof(GroupId),  "The given group identification must not be null!");

            return String.Compare(InternalId, GroupId.InternalId, StringComparison.Ordinal);

        }

        #endregion

        #endregion

        #region IEquatable<GroupId> Members

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

            if (!(Object is Group_Id))
                return false;

            return Equals((Group_Id) Object);

        }

        #endregion

        #region Equals(GroupId)

        /// <summary>
        /// Compares two group identifications for equality.
        /// </summary>
        /// <param name="GroupId">An group identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Group_Id GroupId)
        {

            if ((Object) GroupId == null)
                return false;

            return InternalId.Equals(GroupId.InternalId);

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

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => InternalId;

        #endregion

    }

}
