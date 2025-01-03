/*
 * Copyright (c) 2014-2025 GraphDefined GmbH <achim.friedland@graphdefined.com>
 * This file is part of UsersAPI <https://www.github.com/Vanaheimr/UsersAPI>
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

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// The unique identification of a news banner.
    /// </summary>
    public readonly struct NewsBanner_Id : IId,
                                           IEquatable<NewsBanner_Id>,
                                           IComparable<NewsBanner_Id>

    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

        /// <summary>
        /// Private non-cryptographic random number generator.
        /// </summary>
        private static readonly Random _random = new Random();

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the news banner identificator.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new news banner identification based on the given string.
        /// </summary>
        private NewsBanner_Id(String Text)
        {
            InternalId = Text;
        }

        #endregion


        #region (static) Random(Length)

        /// <summary>
        /// Create a new news banner identification.
        /// </summary>
        /// <param name="Length">The expected length of the news banner identification.</param>
        public static NewsBanner_Id Random(Byte Length = 15)

            => new (RandomExtensions.RandomString(Length).ToUpper());

        #endregion

        #region Parse   (Text)

        /// <summary>
        /// Parse the given string as a news banner identification.
        /// </summary>
        /// <param name="Text">A text representation of a news banner identification.</param>
        public static NewsBanner_Id Parse(String Text)
        {

            if (TryParse(Text, out NewsBanner_Id newsBannerId))
                return newsBannerId;

            throw new ArgumentException($"Invalid text representation of a news banner identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a news banner identification.
        /// </summary>
        /// <param name="Text">A text representation of a news banner identification.</param>
        public static NewsBanner_Id? TryParse(String Text)
        {

            if (TryParse(Text, out NewsBanner_Id newsBannerId))
                return newsBannerId;

            return null;

        }

        #endregion

        #region TryParse(Text, out NewsBannerId)

        /// <summary>
        /// Try to parse the given string as a news banner identification.
        /// </summary>
        /// <param name="Text">A text representation of a news banner identification.</param>
        /// <param name="NewsBannerId">The parsed news banner identification.</param>
        public static Boolean TryParse(String Text, out NewsBanner_Id NewsBannerId)
        {

            Text = Text?.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    NewsBannerId = new NewsBanner_Id(Text);
                    return true;
                }
                catch
                { }
            }

            NewsBannerId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this news banner identification.
        /// </summary>
        public NewsBanner_Id Clone

            => new NewsBanner_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (NewsBannerId1, NewsBannerId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsBannerId1">A news banner identification.</param>
        /// <param name="NewsBannerId2">Another news banner identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (NewsBanner_Id NewsBannerId1,
                                           NewsBanner_Id NewsBannerId2)

            => NewsBannerId1.Equals(NewsBannerId2);

        #endregion

        #region Operator != (NewsBannerId1, NewsBannerId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsBannerId1">A news banner identification.</param>
        /// <param name="NewsBannerId2">Another news banner identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (NewsBanner_Id NewsBannerId1,
                                           NewsBanner_Id NewsBannerId2)

            => !NewsBannerId1.Equals(NewsBannerId2);

        #endregion

        #region Operator <  (NewsBannerId1, NewsBannerId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsBannerId1">A news banner identification.</param>
        /// <param name="NewsBannerId2">Another news banner identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (NewsBanner_Id NewsBannerId1,
                                          NewsBanner_Id NewsBannerId2)

            => NewsBannerId1.CompareTo(NewsBannerId2) < 0;

        #endregion

        #region Operator <= (NewsBannerId1, NewsBannerId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsBannerId1">A news banner identification.</param>
        /// <param name="NewsBannerId2">Another news banner identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (NewsBanner_Id NewsBannerId1,
                                           NewsBanner_Id NewsBannerId2)

            => NewsBannerId1.CompareTo(NewsBannerId2) <= 0;

        #endregion

        #region Operator >  (NewsBannerId1, NewsBannerId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsBannerId1">A news banner identification.</param>
        /// <param name="NewsBannerId2">Another news banner identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (NewsBanner_Id NewsBannerId1,
                                          NewsBanner_Id NewsBannerId2)

            => NewsBannerId1.CompareTo(NewsBannerId2) > 0;

        #endregion

        #region Operator >= (NewsBannerId1, NewsBannerId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsBannerId1">A news banner identification.</param>
        /// <param name="NewsBannerId2">Another news banner identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (NewsBanner_Id NewsBannerId1,
                                           NewsBanner_Id NewsBannerId2)

            => NewsBannerId1.CompareTo(NewsBannerId2) >= 0;

        #endregion

        #endregion

        #region IComparable<NewsBannerId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is NewsBanner_Id newsBannerId
                   ? CompareTo(newsBannerId)
                   : throw new ArgumentException("The given object is not a news banner identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(NewsBannerId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsBannerId">An object to compare with.</param>
        public Int32 CompareTo(NewsBanner_Id NewsBannerId)

            => String.Compare(InternalId,
                              NewsBannerId.InternalId,
                              StringComparison.Ordinal);

        #endregion

        #endregion

        #region IEquatable<NewsBannerId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is NewsBanner_Id newsBannerId &&
                   Equals(newsBannerId);

        #endregion

        #region Equals(NewsBannerId)

        /// <summary>
        /// Compares two NewsBannerIds for equality.
        /// </summary>
        /// <param name="NewsBannerId">A news banner identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(NewsBanner_Id NewsBannerId)

            => String.Equals(InternalId,
                             NewsBannerId.InternalId,
                             StringComparison.Ordinal);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
