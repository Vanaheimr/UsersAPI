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
    /// The unique identification of an user.
    /// </summary>
    public readonly struct User_Id : IId,
                                     IEquatable<User_Id>,
                                     IComparable<User_Id>

    {

        #region Data

        /// <summary>
        /// Private non-cryptographic random number generator.
        /// </summary>
        private static readonly Random _random = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String  InternalId;

        /// <summary>
        /// The internal realm.
        /// </summary>
        private readonly String  Realm;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the user identification.
        /// </summary>
        public UInt64 Length

            => Realm.IsNotNullOrEmpty()
                   ? (UInt64) (InternalId.Length + 1 + Realm.Length)
                   : (UInt64) (InternalId.Length);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new user identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the user identification.</param>
        /// <param name="Realm">An optional realm of the user identification.</param>
        private User_Id(String  String,
                        String  Realm = null)
        {

            this.InternalId  = String.ToLower().Trim();
            this.Realm       = Realm?.ToLower() ?? "";

        }

        #endregion


        #region (static) Random(Length)

        /// <summary>
        /// Create a new user identification.
        /// </summary>
        /// <param name="Length">The expected length of the user identification.</param>
        public static User_Id Random(Byte Length = 15)

            => new User_Id(_random.RandomString(Length).ToUpper());

        #endregion

        #region (static) Parse   (Text, Realm = null)

        /// <summary>
        /// Parse the given string as an user identification.
        /// </summary>
        /// <param name="Text">A text representation of an user identification.</param>
        /// <param name="Realm">An optional realm of the user identification.</param>
        public static User_Id Parse(String Text, String Realm = null)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an user identification must not be null or empty!");

            if (Text.Contains("@"))
            {

                if (Realm.IsNotNullOrEmpty())
                    throw new ArgumentException("The given text representation of an user identification is invalid!", nameof(Text));

                var Splitted = Text.Split(new Char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

                if (Splitted.Length != 2)
                    throw new ArgumentException("The given text representation of an user identification is invalid!", nameof(Text));

                Text   = Splitted[0];
                Realm  = Splitted[1];

            }

            #endregion

            return new User_Id(Text, Realm);

        }

        #endregion

        #region (static) TryParse(Text, Realm = null)

        /// <summary>
        /// Try to parse the given string as an user identification.
        /// </summary>
        /// <param name="Text">A text representation of an user identification.</param>
        /// <param name="Realm">An optional realm of the user identification.</param>
        public static User_Id? TryParse(String Text, String Realm = null)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an user identification must not be null or empty!");

            if (Text.Contains("@"))
            {

                if (Realm.IsNotNullOrEmpty())
                    throw new ArgumentException("The given text representation of an user identification is invalid!", nameof(Text));

                var Splitted = Text.Split(new Char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

                if (Splitted.Length != 2)
                    throw new ArgumentException("The given text representation of an user identification is invalid!", nameof(Text));

                Text   = Splitted[0];
                Realm  = Splitted[1];

            }

            #endregion

            if (TryParse(Text, Realm, out User_Id _UserId))
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

            String Realm = null;

            if (Text.Contains("@"))
            {

                var Splitted = Text.Split(new Char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

                if (Splitted.Length != 2)
                    throw new ArgumentException("The given text representation of an user identification is invalid!", nameof(Text));

                Text   = Splitted[0];
                Realm  = Splitted[1];

            }

            #endregion

            try
            {
                UserId = new User_Id(Text, Realm);
                return true;
            }
            catch (Exception)
            {
                UserId = default;
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

            if (Text.Contains("@"))
            {

                if (Realm.IsNotNullOrEmpty())
                    throw new ArgumentException("The given text representation of an user identification is invalid!", nameof(Text));

                var Splitted = Text.Split(new Char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);

                if (Splitted.Length != 2)
                    throw new ArgumentException("The given text representation of an user identification is invalid!", nameof(Text));

                Text   = Splitted[0];
                Realm  = Splitted[1];

            }

            #endregion

            try
            {
                UserId = new User_Id(Text, Realm);
                return true;
            }
            catch (Exception)
            {
                UserId = default;
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
                throw new ArgumentNullException(nameof(UserId1), "The given UserId1 must not be null!");

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
                throw new ArgumentNullException(nameof(UserId1), "The given UserId1 must not be null!");

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

            var c = String.Compare(InternalId, UserId.InternalId, StringComparison.OrdinalIgnoreCase);
            if (c != 0)
                return c;

            return String.Compare(Realm, UserId.Realm, StringComparison.OrdinalIgnoreCase);

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

            return InternalId.Equals(UserId.InternalId, StringComparison.OrdinalIgnoreCase) &&
                   Realm.     Equals(UserId.Realm,      StringComparison.OrdinalIgnoreCase);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId.ToLower().GetHashCode() ^
               Realm.     ToLower().GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => Realm.IsNotNullOrEmpty()
                   ? InternalId + "@" + Realm
                   : InternalId;

        #endregion

    }

}
