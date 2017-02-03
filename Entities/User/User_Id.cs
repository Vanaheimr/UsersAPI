/*
 * Copyright (c) 2014-2017, Achim 'ahzf' Friedland <achim@graphdefined.org>
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

namespace org.GraphDefined.OpenData
{

    /// <summary>
    /// The unique identification of an user.
    /// </summary>
    public struct User_Id : IId<User_Id>

    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        //ToDo: Replace with better randomness!
        private static readonly Random _Random = new Random(DateTime.Now.Millisecond);

        #endregion

        #region Properties

        /// <summary>
        /// The realm.
        /// </summary>
        public String Realm { get; }

        /// <summary>
        /// The length of the user identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId.Length + Realm.Length);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new user identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the user identification.</param>
        /// <param name="Realm">An optional realm of the user identification.</param>
        private User_Id(String  String,
                        String  Realm = "")
        {

            this.InternalId  = String;
            this.Realm       = Realm ?? "";

        }

        #endregion


        #region (static) Parse(Text, Realm = "")

        /// <summary>
        /// Parse the given string as an user identification.
        /// </summary>
        /// <param name="Text">A text representation of an user identification.</param>
        /// <param name="Realm">An optional realm of the user identification.</param>
        public static User_Id Parse(String Text, String Realm = "")
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an user identification must not be null or empty!");

            #endregion

            return new User_Id(Text, Realm);

        }

        #endregion

        #region (static) TryParse(Text, Realm = "")

        /// <summary>
        /// Try to parse the given string as an user identification.
        /// </summary>
        /// <param name="Text">A text representation of an user identification.</param>
        /// <param name="Realm">An optional realm of the user identification.</param>
        public static User_Id? TryParse(String Text, String Realm = "")
        {

            User_Id _UserId;

            if (TryParse(Text, Realm, out _UserId))
                return _UserId;

            return new User_Id?();

        }

        #endregion

        #region (static) TryParse(Text, out UserId)

        /// <summary>
        /// Try to parse the given string as an user identification.
        /// </summary>
        /// <param name="Text">A text representation of an user identification.</param>
        /// <param name="UserId">The parsed user identification.</param>
        public static Boolean TryParse(String Text, out User_Id UserId)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an user identification must not be null or empty!");

            #endregion

            try
            {
                UserId = new User_Id(Text);
                return true;
            }
            catch (Exception)
            {
                UserId = default(User_Id);
                return false;
            }

        }

        #endregion

        #region (static) TryParse(Text, Realm, out UserId)

        /// <summary>
        /// Try to parse the given string as an user identification.
        /// </summary>
        /// <param name="Text">A text representation of an user identification.</param>
        /// <param name="Realm">An optional realm of the user identification.</param>
        /// <param name="UserId">The parsed user identification.</param>
        public static Boolean TryParse(String Text, String Realm, out User_Id UserId)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an user identification must not be null or empty!");

            #endregion

            try
            {
                UserId = new User_Id(Text, Realm);
                return true;
            }
            catch (Exception)
            {
                UserId = default(User_Id);
                return false;
            }

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this user identification.
        /// </summary>

        public User_Id Clone

            => new User_Id(new String(InternalId.ToCharArray()),
                           new String(Realm.     ToCharArray()));

        #endregion


        #region Operator overloading

        #region Operator == (UserId1, UserId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserId1">A user identification.</param>
        /// <param name="UserId2">Another user identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (User_Id UserId1, User_Id UserId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(UserId1, UserId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) UserId1 == null) || ((Object) UserId2 == null))
                return false;

            return UserId1.Equals(UserId2);

        }

        #endregion

        #region Operator != (UserId1, UserId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserId1">A user identification.</param>
        /// <param name="UserId2">Another user identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (User_Id UserId1, User_Id UserId2)
            => !(UserId1 == UserId2);

        #endregion

        #region Operator <  (UserId1, UserId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserId1">A user identification.</param>
        /// <param name="UserId2">Another user identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (User_Id UserId1, User_Id UserId2)
        {

            if ((Object) UserId1 == null)
                throw new ArgumentNullException("The given UserId1 must not be null!");

            return UserId1.CompareTo(UserId2) < 0;

        }

        #endregion

        #region Operator <= (UserId1, UserId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserId1">A user identification.</param>
        /// <param name="UserId2">Another user identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (User_Id UserId1, User_Id UserId2)
            => !(UserId1 > UserId2);

        #endregion

        #region Operator >  (UserId1, UserId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserId1">A user identification.</param>
        /// <param name="UserId2">Another user identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (User_Id UserId1, User_Id UserId2)
        {

            if ((Object) UserId1 == null)
                throw new ArgumentNullException("The given UserId1 must not be null!");

            return UserId1.CompareTo(UserId2) > 0;

        }

        #endregion

        #region Operator >= (UserId1, UserId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserId1">A user identification.</param>
        /// <param name="UserId2">Another user identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (User_Id UserId1, User_Id UserId2)
            => !(UserId1 < UserId2);

        #endregion

        #endregion

        #region IComparable<UserId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is User_Id))
                throw new ArgumentException("The given object is not an user identification!",
                                            nameof(Object));

            return CompareTo((User_Id) Object);

        }

        #endregion

        #region CompareTo(UserId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserId">An object to compare with.</param>
        public Int32 CompareTo(User_Id UserId)
        {

            if ((Object) UserId == null)
                throw new ArgumentNullException(nameof(UserId),  "The given user identification must not be null!");

            var c = String.Compare(InternalId, UserId.InternalId, StringComparison.Ordinal);
            if (c != 0)
                return c;

            return String.Compare(Realm, UserId.Realm, StringComparison.Ordinal);

        }

        #endregion

        #endregion

        #region IEquatable<UserId> Members

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

            if (!(Object is User_Id))
                return false;

            return Equals((User_Id) Object);

        }

        #endregion

        #region Equals(UserId)

        /// <summary>
        /// Compares two user identifications for equality.
        /// </summary>
        /// <param name="UserId">An user identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(User_Id UserId)
        {

            if ((Object) UserId == null)
                return false;

            return InternalId.Equals(UserId.InternalId) &&
                   Realm.     Equals(UserId.Realm);

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
        /// Return a string represtentation of this object.
        /// </summary>
        public override String ToString()
            => InternalId;

        #endregion

    }

}
