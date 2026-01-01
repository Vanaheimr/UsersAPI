/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// Extension methods for newsletter identifications.
    /// </summary>
    public static class NewsletterIdExtensions
    {

        /// <summary>
        /// Indicates whether this newsletter identification is null or empty.
        /// </summary>
        /// <param name="NewsletterId">A newsletter identification.</param>
        public static Boolean IsNullOrEmpty(this Newsletter_Id? NewsletterId)
            => !NewsletterId.HasValue || NewsletterId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this newsletter identification is null or empty.
        /// </summary>
        /// <param name="NewsletterId">A newsletter identification.</param>
        public static Boolean IsNotNullOrEmpty(this Newsletter_Id? NewsletterId)
            => NewsletterId.HasValue && NewsletterId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a newsletter.
    /// </summary>
    public readonly struct Newsletter_Id : IId,
                                           IEquatable<Newsletter_Id>,
                                           IComparable<Newsletter_Id>
    {

        #region Data

        /// <summary>
        /// The internal newsletter identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this newsletter identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this newsletter identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the newsletter identificator.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new newsletter identification based on the given string.
        /// </summary>
        private Newsletter_Id(String Text)
        {
            InternalId = Text;
        }

        #endregion


        #region (static) Random  (Length = 23)

        /// <summary>
        /// Create a new random newsletter identification.
        /// </summary>
        /// <param name="Length">The expected length of the random newsletter identification.</param>
        public static Newsletter_Id Random(Byte Length = 23)

            => new (RandomExtensions.RandomString(Length).ToUpper());

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a newsletter identification.
        /// </summary>
        /// <param name="Text">A text representation of a newsletter identification.</param>
        public static Newsletter_Id Parse(String Text)
        {

            if (TryParse(Text, out Newsletter_Id blogPostingId))
                return blogPostingId;

            throw new ArgumentException($"Invalid text representation of a newsletter identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a newsletter identification.
        /// </summary>
        /// <param name="Text">A text representation of a newsletter identification.</param>
        public static Newsletter_Id? TryParse(String? Text)
        {

            if (Text is not null && TryParse(Text, out Newsletter_Id blogPostingId))
                return blogPostingId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out NewsletterId)

        /// <summary>
        /// Try to parse the given string as a newsletter identification.
        /// </summary>
        /// <param name="Text">A text representation of a newsletter identification.</param>
        /// <param name="NewsletterId">The parsed newsletter identification.</param>
        public static Boolean TryParse(String Text, out Newsletter_Id NewsletterId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    NewsletterId = new Newsletter_Id(Text);
                    return true;
                }
                catch
                { }
            }

            NewsletterId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this newsletter identification.
        /// </summary>
        public Newsletter_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (NewsletterId1, NewsletterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsletterId1">A newsletter identification.</param>
        /// <param name="NewsletterId2">Another newsletter identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Newsletter_Id NewsletterId1,
                                           Newsletter_Id NewsletterId2)

            => NewsletterId1.Equals(NewsletterId2);

        #endregion

        #region Operator != (NewsletterId1, NewsletterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsletterId1">A newsletter identification.</param>
        /// <param name="NewsletterId2">Another newsletter identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Newsletter_Id NewsletterId1,
                                           Newsletter_Id NewsletterId2)

            => !NewsletterId1.Equals(NewsletterId2);

        #endregion

        #region Operator <  (NewsletterId1, NewsletterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsletterId1">A newsletter identification.</param>
        /// <param name="NewsletterId2">Another newsletter identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Newsletter_Id NewsletterId1,
                                          Newsletter_Id NewsletterId2)

            => NewsletterId1.CompareTo(NewsletterId2) < 0;

        #endregion

        #region Operator <= (NewsletterId1, NewsletterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsletterId1">A newsletter identification.</param>
        /// <param name="NewsletterId2">Another newsletter identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Newsletter_Id NewsletterId1,
                                           Newsletter_Id NewsletterId2)

            => NewsletterId1.CompareTo(NewsletterId2) <= 0;

        #endregion

        #region Operator >  (NewsletterId1, NewsletterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsletterId1">A newsletter identification.</param>
        /// <param name="NewsletterId2">Another newsletter identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Newsletter_Id NewsletterId1,
                                          Newsletter_Id NewsletterId2)

            => NewsletterId1.CompareTo(NewsletterId2) > 0;

        #endregion

        #region Operator >= (NewsletterId1, NewsletterId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsletterId1">A newsletter identification.</param>
        /// <param name="NewsletterId2">Another newsletter identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Newsletter_Id NewsletterId1,
                                           Newsletter_Id NewsletterId2)

            => NewsletterId1.CompareTo(NewsletterId2) >= 0;

        #endregion

        #endregion

        #region IComparable<Newsletter_Id> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is Newsletter_Id blogPostingId
                   ? CompareTo(blogPostingId)
                   : throw new ArgumentException("The given object is not a newsletter identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(NewsletterId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsletterId">An object to compare with.</param>
        public Int32 CompareTo(Newsletter_Id NewsletterId)

            => String.Compare(InternalId,
                              NewsletterId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<Newsletter_Id> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object? Object)

            => Object is Newsletter_Id blogPostingId &&
                   Equals(blogPostingId);

        #endregion

        #region Equals(NewsletterId)

        /// <summary>
        /// Compares two newsletter identifications for equality.
        /// </summary>
        /// <param name="NewsletterId">A newsletter identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Newsletter_Id NewsletterId)

            => String.Equals(InternalId,
                             NewsletterId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => InternalId?.ToLower().GetHashCode() ?? 0;

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
