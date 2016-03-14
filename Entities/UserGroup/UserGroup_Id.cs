/*
 * Copyright (c) 2014-2016, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using System.Text.RegularExpressions;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.OpenData
{

    /// <summary>
    /// The unique identification of an user group.
    /// </summary>
    public class UserGroup_Id : IId,
                                IEquatable<UserGroup_Id>,
                                IComparable<UserGroup_Id>

    {

        #region Data

        //ToDo: Replace with better randomness!
        private static readonly Random _Random = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// The internal identification.
        /// </summary>
        protected readonly String _Id;

        #endregion

        #region Properties

        #region Length

        /// <summary>
        /// Returns the length of the identificator.
        /// </summary>
        public UInt64 Length
        {
            get
            {
                return (UInt64) (_Id.Length);
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new user group identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the user group identification.</param>
        private UserGroup_Id(String String)
        {
            _Id = String.Trim();
        }

        #endregion


        #region Parse(Text)

        /// <summary>
        /// Parse the given string as an Electric Vehicle Charging Group Identification (EVCG Id)
        /// </summary>
        /// <param name="Text">A text representation of an Electric Vehicle Charging Group identification.</param>
        public static UserGroup_Id Parse(String Text)
        {
            return new UserGroup_Id(Text);
        }

        #endregion

        #region TryParse(Text, out GroupId)

        /// <summary>
        /// Parse the given string as an Electric Vehicle Charging Group Identification (EVCG Id)
        /// </summary>
        /// <param name="Text">A text representation of an Electric Vehicle Charging Group identification.</param>
        /// <param name="GroupId">The parsed Electric Vehicle Charging Group identification.</param>
        public static Boolean TryParse(String Text, out UserGroup_Id GroupId)
        {
            try
            {
                GroupId = new UserGroup_Id(Text);
                return true;
            }
            catch (Exception)
            {
                GroupId = null;
                return false;
            }
        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone an Group_Id.
        /// </summary>
        public UserGroup_Id Clone
        {
            get
            {

                return new UserGroup_Id(new String(_Id.ToCharArray()));

            }
        }

        #endregion


        #region Operator overloading

        #region Operator == (Group_Id1, Group_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Group_Id1">A Group_Id.</param>
        /// <param name="Group_Id2">Another Group_Id.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (UserGroup_Id Group_Id1, UserGroup_Id Group_Id2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Group_Id1, Group_Id2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) Group_Id1 == null) || ((Object) Group_Id2 == null))
                return false;

            return Group_Id1.Equals(Group_Id2);

        }

        #endregion

        #region Operator != (Group_Id1, Group_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Group_Id1">A Group_Id.</param>
        /// <param name="Group_Id2">Another Group_Id.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (UserGroup_Id Group_Id1, UserGroup_Id Group_Id2)
        {
            return !(Group_Id1 == Group_Id2);
        }

        #endregion

        #region Operator <  (Group_Id1, Group_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Group_Id1">A Group_Id.</param>
        /// <param name="Group_Id2">Another Group_Id.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (UserGroup_Id Group_Id1, UserGroup_Id Group_Id2)
        {

            if ((Object) Group_Id1 == null)
                throw new ArgumentNullException("The given Group_Id1 must not be null!");

            return Group_Id1.CompareTo(Group_Id2) < 0;

        }

        #endregion

        #region Operator <= (Group_Id1, Group_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Group_Id1">A Group_Id.</param>
        /// <param name="Group_Id2">Another Group_Id.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (UserGroup_Id Group_Id1, UserGroup_Id Group_Id2)
        {
            return !(Group_Id1 > Group_Id2);
        }

        #endregion

        #region Operator >  (Group_Id1, Group_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Group_Id1">A Group_Id.</param>
        /// <param name="Group_Id2">Another Group_Id.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (UserGroup_Id Group_Id1, UserGroup_Id Group_Id2)
        {

            if ((Object) Group_Id1 == null)
                throw new ArgumentNullException("The given Group_Id1 must not be null!");

            return Group_Id1.CompareTo(Group_Id2) > 0;

        }

        #endregion

        #region Operator >= (Group_Id1, Group_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Group_Id1">A Group_Id.</param>
        /// <param name="Group_Id2">Another Group_Id.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (UserGroup_Id Group_Id1, UserGroup_Id Group_Id2)
        {
            return !(Group_Id1 < Group_Id2);
        }

        #endregion

        #endregion

        #region IComparable<Group_Id> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an Group_Id.
            var Group_Id = Object as UserGroup_Id;
            if ((Object) Group_Id == null)
                throw new ArgumentException("The given object is not a Group_Id!");

            return CompareTo(Group_Id);

        }

        #endregion

        #region CompareTo(Group_Id)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Group_Id">An object to compare with.</param>
        public Int32 CompareTo(UserGroup_Id Group_Id)
        {

            if ((Object) Group_Id == null)
                throw new ArgumentNullException("The given Group_Id must not be null!");

            return this._Id.CompareTo(Group_Id._Id);

        }

        #endregion

        #endregion

        #region IEquatable<Group_Id> Members

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

            // Check if the given object is an Group_Id.
            var Group_Id = Object as UserGroup_Id;
            if ((Object) Group_Id == null)
                return false;

            return this.Equals(Group_Id);

        }

        #endregion

        #region Equals(Group_Id)

        /// <summary>
        /// Compares two Group_Ids for equality.
        /// </summary>
        /// <param name="GroupId">A Group_Id to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(UserGroup_Id GroupId)
        {

            if ((Object) GroupId == null)
                return false;

            return _Id.Equals(GroupId._Id);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            return _Id.GetHashCode();
        }

        #endregion

        #region ToString()

        /// <summary>
        /// Return a string represtentation of this object.
        /// </summary>
        public override String ToString()
        {
            return _Id;
        }

        #endregion

    }

}
