/*
 * Copyright (c) 2014-2021, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
    /// The unique identification of a news posting.
    /// </summary>
    public readonly struct NewsPosting_Id : IId,
                                            IEquatable<NewsPosting_Id>,
                                            IComparable<NewsPosting_Id>

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
        /// The length of the news posting identificator.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId?.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new news posting identification based on the given string.
        /// </summary>
        private NewsPosting_Id(String Text)
        {
            InternalId = Text;
        }

        #endregion


        #region (static) Random(Length)

        /// <summary>
        /// Create a new news posting identification.
        /// </summary>
        /// <param name="Length">The expected length of the news posting identification.</param>
        public static NewsPosting_Id Random(Byte Length = 15)

            => new NewsPosting_Id(_random.RandomString(Length));

        #endregion

        #region Parse   (Text)

        /// <summary>
        /// Parse the given string as a news posting identification.
        /// </summary>
        /// <param name="Text">A text-representation of a news posting identification.</param>
        public static NewsPosting_Id Parse(String Text)
        {

            if (TryParse(Text, out NewsPosting_Id newsPostingId))
                return newsPostingId;

            throw new ArgumentException("Invalid text-representation of a news posting identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a news posting identification.
        /// </summary>
        /// <param name="Text">A text-representation of a news posting identification.</param>
        public static NewsPosting_Id? TryParse(String Text)
        {

            if (TryParse(Text, out NewsPosting_Id newsPostingId))
                return newsPostingId;

            return null;

        }

        #endregion

        #region TryParse(Text, out NewsPostingId)

        /// <summary>
        /// Try to parse the given string as a news posting identification.
        /// </summary>
        /// <param name="Text">A text-representation of a news posting identification.</param>
        /// <param name="NewsPostingId">The parsed news posting identification.</param>
        public static Boolean TryParse(String Text, out NewsPosting_Id NewsPostingId)
        {

            Text = Text?.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    NewsPostingId = new NewsPosting_Id(Text);
                    return true;
                }
                catch
                { }
            }

            NewsPostingId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this news posting identification.
        /// </summary>
        public NewsPosting_Id Clone

            => new NewsPosting_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (NewsPostingId1, NewsPostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsPostingId1">A news posting identification.</param>
        /// <param name="NewsPostingId2">Another news posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (NewsPosting_Id NewsPostingId1,
                                           NewsPosting_Id NewsPostingId2)

            => NewsPostingId1.Equals(NewsPostingId2);

        #endregion

        #region Operator != (NewsPostingId1, NewsPostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsPostingId1">A news posting identification.</param>
        /// <param name="NewsPostingId2">Another news posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (NewsPosting_Id NewsPostingId1,
                                           NewsPosting_Id NewsPostingId2)

            => !NewsPostingId1.Equals(NewsPostingId2);

        #endregion

        #region Operator <  (NewsPostingId1, NewsPostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsPostingId1">A news posting identification.</param>
        /// <param name="NewsPostingId2">Another news posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (NewsPosting_Id NewsPostingId1,
                                          NewsPosting_Id NewsPostingId2)

            => NewsPostingId1.CompareTo(NewsPostingId2) < 0;

        #endregion

        #region Operator <= (NewsPostingId1, NewsPostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsPostingId1">A news posting identification.</param>
        /// <param name="NewsPostingId2">Another news posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (NewsPosting_Id NewsPostingId1,
                                           NewsPosting_Id NewsPostingId2)

            => NewsPostingId1.CompareTo(NewsPostingId2) <= 0;

        #endregion

        #region Operator >  (NewsPostingId1, NewsPostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsPostingId1">A news posting identification.</param>
        /// <param name="NewsPostingId2">Another news posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (NewsPosting_Id NewsPostingId1,
                                          NewsPosting_Id NewsPostingId2)

            => NewsPostingId1.CompareTo(NewsPostingId2) > 0;

        #endregion

        #region Operator >= (NewsPostingId1, NewsPostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsPostingId1">A news posting identification.</param>
        /// <param name="NewsPostingId2">Another news posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (NewsPosting_Id NewsPostingId1,
                                           NewsPosting_Id NewsPostingId2)

            => NewsPostingId1.CompareTo(NewsPostingId2) >= 0;

        #endregion

        #endregion

        #region IComparable<NewsPostingId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is NewsPosting_Id newsPostingId
                   ? CompareTo(newsPostingId)
                   : throw new ArgumentException("The given object is not a news posting identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(NewsPostingId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsPostingId">An object to compare with.</param>
        public Int32 CompareTo(NewsPosting_Id NewsPostingId)

            => String.Compare(InternalId,
                              NewsPostingId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<NewsPostingId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is NewsPosting_Id newsPostingId &&
                   Equals(newsPostingId);

        #endregion

        #region Equals(NewsPostingId)

        /// <summary>
        /// Compares two NewsPostingIds for equality.
        /// </summary>
        /// <param name="NewsPostingId">A news posting identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(NewsPosting_Id NewsPostingId)

            => String.Equals(InternalId,
                             NewsPostingId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId?.GetHashCode() ?? 0;

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text-representation of this object.
        /// </summary>
        public override String ToString()

            => InternalId ?? "";

        #endregion

    }

}
