﻿/*
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
    /// The unique identification of an user.
    /// </summary>
    public class User_Id : IId,
                           IEquatable<User_Id>,
                           IComparable<User_Id>

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
        /// Create a new user identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the user identification.</param>
        private User_Id(String String)
        {
            _Id = String.Trim();
        }

        #endregion


        #region Parse(Text)

        /// <summary>
        /// Parse the given string as an Electric Vehicle Charging Group Identification (EVCG Id)
        /// </summary>
        /// <param name="Text">A text representation of an Electric Vehicle Charging Group identification.</param>
        public static User_Id Parse(String Text)
        {
            return new User_Id(Text);
        }

        #endregion

        #region TryParse(Text, out UserId)

        /// <summary>
        /// Parse the given string as an Electric Vehicle Charging Group Identification (EVCG Id)
        /// </summary>
        /// <param name="Text">A text representation of an Electric Vehicle Charging Group identification.</param>
        /// <param name="UserId">The parsed Electric Vehicle Charging Group identification.</param>
        public static Boolean TryParse(String Text, out User_Id UserId)
        {
            try
            {
                UserId = new User_Id(Text);
                return true;
            }
            catch (Exception)
            {
                UserId = null;
                return false;
            }
        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone an User_Id.
        /// </summary>
        public User_Id Clone
        {
            get
            {

                return new User_Id(new String(_Id.ToCharArray()));

            }
        }

        #endregion


        #region Operator overloading

        #region Operator == (User_Id1, User_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="User_Id1">A User_Id.</param>
        /// <param name="User_Id2">Another User_Id.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (User_Id User_Id1, User_Id User_Id2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(User_Id1, User_Id2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) User_Id1 == null) || ((Object) User_Id2 == null))
                return false;

            return User_Id1.Equals(User_Id2);

        }

        #endregion

        #region Operator != (User_Id1, User_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="User_Id1">A User_Id.</param>
        /// <param name="User_Id2">Another User_Id.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (User_Id User_Id1, User_Id User_Id2)
        {
            return !(User_Id1 == User_Id2);
        }

        #endregion

        #region Operator <  (User_Id1, User_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="User_Id1">A User_Id.</param>
        /// <param name="User_Id2">Another User_Id.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (User_Id User_Id1, User_Id User_Id2)
        {

            if ((Object) User_Id1 == null)
                throw new ArgumentNullException("The given User_Id1 must not be null!");

            return User_Id1.CompareTo(User_Id2) < 0;

        }

        #endregion

        #region Operator <= (User_Id1, User_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="User_Id1">A User_Id.</param>
        /// <param name="User_Id2">Another User_Id.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (User_Id User_Id1, User_Id User_Id2)
        {
            return !(User_Id1 > User_Id2);
        }

        #endregion

        #region Operator >  (User_Id1, User_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="User_Id1">A User_Id.</param>
        /// <param name="User_Id2">Another User_Id.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (User_Id User_Id1, User_Id User_Id2)
        {

            if ((Object) User_Id1 == null)
                throw new ArgumentNullException("The given User_Id1 must not be null!");

            return User_Id1.CompareTo(User_Id2) > 0;

        }

        #endregion

        #region Operator >= (User_Id1, User_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="User_Id1">A User_Id.</param>
        /// <param name="User_Id2">Another User_Id.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (User_Id User_Id1, User_Id User_Id2)
        {
            return !(User_Id1 < User_Id2);
        }

        #endregion

        #endregion

        #region IComparable<User_Id> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an User_Id.
            var User_Id = Object as User_Id;
            if ((Object) User_Id == null)
                throw new ArgumentException("The given object is not a User_Id!");

            return CompareTo(User_Id);

        }

        #endregion

        #region CompareTo(User_Id)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="User_Id">An object to compare with.</param>
        public Int32 CompareTo(User_Id User_Id)
        {

            if ((Object) User_Id == null)
                throw new ArgumentNullException("The given User_Id must not be null!");

            return this._Id.CompareTo(User_Id._Id);

        }

        #endregion

        #endregion

        #region IEquatable<User_Id> Members

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

            // Check if the given object is an User_Id.
            var User_Id = Object as User_Id;
            if ((Object) User_Id == null)
                return false;

            return this.Equals(User_Id);

        }

        #endregion

        #region Equals(User_Id)

        /// <summary>
        /// Compares two User_Ids for equality.
        /// </summary>
        /// <param name="UserId">A User_Id to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(User_Id UserId)
        {

            if ((Object) UserId == null)
                return false;

            return _Id.Equals(UserId._Id);

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