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
    /// The unique identification of an user group.
    /// </summary>
    public readonly struct UserGroup_Id : IId<UserGroup_Id>
    {

        #region Data

        private static readonly Random _random = new Random(DateTime.Now.Millisecond);

        #endregion

        #region Properties

        /// <summary>
        /// The internal context.
        /// </summary>
        internal String  Context    { get; }

        /// <summary>
        /// The internal identification.
        /// </summary>
        internal String  Id         { get; }


        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty

            => Id.IsNullOrEmpty();

        /// <summary>
        /// The length of the user group identification.
        /// </summary>
        public UInt64 Length

            => (UInt64) ((Context.IsNotNullOrEmpty()
                              ? Context.Length + 1
                              : 0) +
                         (Id?.Length ?? 0));

        #endregion

        #region Constructor(s)

        #region UserGroup_Id(Text)

        /// <summary>
        /// Create a new user group identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of the user group identification.</param>
        private UserGroup_Id(String Text)

            : this(String.Empty,
                   Text)

        { }

        #endregion

        #region UserGroup_Id(Context, Text)

        /// <summary>
        /// Create a new user group identification based on the given text and context.
        /// </summary>
        /// <param name="Context">The text representation of the user group identification context.</param>
        /// <param name="Text">The text representation of the user group identification.</param>
        private UserGroup_Id(String  Context,
                                      String  Text)
        {

            this.Context  = Context?.Trim();
            this.Id       = Text?.   Trim();

        }

        #endregion

        #endregion


        #region (static) Random  (Length)

        /// <summary>
        /// Create a new random user group identification.
        /// </summary>
        /// <param name="Length">The expected length of the user group identification.</param>
        public static UserGroup_Id Random(Byte Length = 10)

            => new UserGroup_Id(_random.RandomString(Length).ToUpper());

        #endregion

        #region (static) Parse   (         Text)

        /// <summary>
        /// Parse the given string as an user group identification.
        /// </summary>
        /// <param name="Text">A text representation of an user group identification.</param>
        public static UserGroup_Id Parse(String Text)
        {

            if (TryParse(Text, out UserGroup_Id userGroupId))
                return userGroupId;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an user group identification must not be null or empty!");

            throw new ArgumentException("The given text representation of an user group identification is invalid!", nameof(Text));

        }

        #endregion

        #region (static) Parse   (Context, Text)

        /// <summary>
        /// Parse the given string as an user group identification.
        /// </summary>
        /// <param name="Context">The text representation of the user group identification context.</param>
        /// <param name="Text">A text representation of an user group identification.</param>
        public static UserGroup_Id Parse(String  Context,
                                                  String  Text)
        {

            if (TryParse(Context,
                         Text,
                         out UserGroup_Id userGroupId))
            {
                return userGroupId;
            }

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an user group identification must not be null or empty!");

            throw new ArgumentException("The given text representation of an user group identification is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(         Text)

        /// <summary>
        /// Try to parse the given text as an user group identification.
        /// </summary>
        /// <param name="Text">A text representation of an user group identification.</param>
        public static UserGroup_Id? TryParse(String Text)
        {

            if (TryParse(Text,
                         out UserGroup_Id userGroupId))
            {
                return userGroupId;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Context, Text)

        /// <summary>
        /// Try to parse the given text as an user group identification.
        /// </summary>
        /// <param name="Context">The text representation of the user group identification context.</param>
        /// <param name="Text">A text representation of an user group identification.</param>
        public static UserGroup_Id? TryParse(String  Context,
                                             String  Text)
        {

            if (TryParse(Context,
                         Text,
                         out UserGroup_Id userGroupId))
            {
                return userGroupId;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(         Text, out UserGroupId)

        /// <summary>
        /// Try to parse the given text as an user group identification.
        /// </summary>
        /// <param name="Text">A text representation of an user group identification.</param>
        /// <param name="UserGroupId">The parsed user group identification.</param>
        public static Boolean TryParse(String Text, out UserGroup_Id UserGroupId)
        {

            if (Text.IsNullOrEmpty())
            {
                UserGroupId = default;
                return false;
            }

            if (Text.Contains("."))
                return TryParse(Text.Substring(0, Text.LastIndexOf(".")),
                                Text.Substring(Text.LastIndexOf(".") + 1),
                                out UserGroupId);

            return TryParse(String.Empty,
                            Text,
                            out UserGroupId);

        }

        #endregion

        #region (static) TryParse(Context, Text, out UserGroupId)

        /// <summary>
        /// Try to parse the given context and text as an user group identification.
        /// </summary>
        /// <param name="Context">The text representation of the user group identification context.</param>
        /// <param name="Text">A text representation of an user group identification.</param>
        /// <param name="UserGroupId">The parsed user group identification.</param>
        public static Boolean TryParse(String            Context,
                                       String            Text,
                                       out UserGroup_Id  UserGroupId)
        {

            Text = Text?.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {

                    UserGroupId = new UserGroup_Id(Context?.Trim(),
                                                   Text);

                    return true;

                }
                catch (Exception)
                { }
            }

            UserGroupId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this user group identification.
        /// </summary>
        public UserGroup_Id Clone

            => new UserGroup_Id(
                   new String(Context?.ToCharArray()),
                   new String(Id?.     ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (UserGroupId1, UserGroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserGroupId1">A user group identification.</param>
        /// <param name="UserGroupId2">Another user group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (UserGroup_Id UserGroupId1,
                                           UserGroup_Id UserGroupId2)

            => UserGroupId1.Equals(UserGroupId2);

        #endregion

        #region Operator != (UserGroupId1, UserGroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserGroupId1">A user group identification.</param>
        /// <param name="UserGroupId2">Another user group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (UserGroup_Id UserGroupId1,
                                           UserGroup_Id UserGroupId2)

            => !(UserGroupId1 == UserGroupId2);

        #endregion

        #region Operator <  (UserGroupId1, UserGroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserGroupId1">A user group identification.</param>
        /// <param name="UserGroupId2">Another user group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (UserGroup_Id UserGroupId1,
                                          UserGroup_Id UserGroupId2)

            => UserGroupId1.CompareTo(UserGroupId2) < 0;

        #endregion

        #region Operator <= (UserGroupId1, UserGroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserGroupId1">A user group identification.</param>
        /// <param name="UserGroupId2">Another user group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (UserGroup_Id UserGroupId1,
                                           UserGroup_Id UserGroupId2)

            => !(UserGroupId1 > UserGroupId2);

        #endregion

        #region Operator >  (UserGroupId1, UserGroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserGroupId1">A user group identification.</param>
        /// <param name="UserGroupId2">Another user group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (UserGroup_Id UserGroupId1,
                                          UserGroup_Id UserGroupId2)

            => UserGroupId1.CompareTo(UserGroupId2) > 0;

        #endregion

        #region Operator >= (UserGroupId1, UserGroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserGroupId1">A user group identification.</param>
        /// <param name="UserGroupId2">Another user group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (UserGroup_Id UserGroupId1,
                                           UserGroup_Id UserGroupId2)

            => !(UserGroupId1 < UserGroupId2);

        #endregion

        #endregion

        #region IComparable<UserGroupId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

             => Object is UserGroup_Id userGroupId
                    ? CompareTo(userGroupId)
                    : throw new ArgumentException("The given object is not an user group identification!",
                                                  nameof(Object));

        #endregion

        #region CompareTo(UserGroupId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserGroupId">An object to compare with.</param>
        public Int32 CompareTo(UserGroup_Id UserGroupId)
        {

            var c = String.Compare(Context,
                                   UserGroupId.Context,
                                   StringComparison.OrdinalIgnoreCase);

            if (c == 0)
                c = String.Compare(Id,
                                   UserGroupId.Id,
                                   StringComparison.OrdinalIgnoreCase);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<UserGroupId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is UserGroup_Id userGroupId &&
                   Equals(userGroupId);

        #endregion

        #region Equals(UserGroupId)

        /// <summary>
        /// Compares two user group identifications for equality.
        /// </summary>
        /// <param name="UserGroupId">An user group identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(UserGroup_Id UserGroupId)

            => String.Equals(Context,
                             UserGroupId.Context,
                             StringComparison.OrdinalIgnoreCase) &&
               String.Equals(Id,
                             UserGroupId.Id,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => (Context?.ToLower().GetHashCode() * 3 ?? 0) ^
               (Id?.     ToLower().GetHashCode()     ?? 0);

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(Context.IsNotNullOrEmpty()
                                 ? Context + "."
                                 : "",
                             Id ?? "");

        #endregion

    }

}
