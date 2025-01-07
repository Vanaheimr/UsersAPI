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
    /// Extension methods for blog posting identifications.
    /// </summary>
    public static class BlogPostingIdExtensions
    {

        /// <summary>
        /// Indicates whether this blog posting identification is null or empty.
        /// </summary>
        /// <param name="BlogPostingId">A blog posting identification.</param>
        public static Boolean IsNullOrEmpty(this BlogPosting_Id? BlogPostingId)
            => !BlogPostingId.HasValue || BlogPostingId.Value.IsNullOrEmpty;

        /// <summary>
        /// Indicates whether this blog posting identification is null or empty.
        /// </summary>
        /// <param name="BlogPostingId">A blog posting identification.</param>
        public static Boolean IsNotNullOrEmpty(this BlogPosting_Id? BlogPostingId)
            => BlogPostingId.HasValue && BlogPostingId.Value.IsNotNullOrEmpty;

    }


    /// <summary>
    /// The unique identification of a blog posting.
    /// </summary>
    public readonly struct BlogPosting_Id : IId,
                                            IEquatable<BlogPosting_Id>,
                                            IComparable<BlogPosting_Id>
    {

        #region Data

        /// <summary>
        /// The internal blog posting identification.
        /// </summary>
        private readonly String InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this blog posting identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// Indicates whether this blog posting identification is NOT null or empty.
        /// </summary>
        public Boolean IsNotNullOrEmpty
            => InternalId.IsNotNullOrEmpty();

        /// <summary>
        /// The length of the blog posting identificator.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new blog posting identification based on the given string.
        /// </summary>
        private BlogPosting_Id(String Text)
        {
            InternalId = Text;
        }

        #endregion


        #region (static) Random  (Length = 23)

        /// <summary>
        /// Create a new random blog posting identification.
        /// </summary>
        /// <param name="Length">The expected length of the random blog posting identification.</param>
        public static BlogPosting_Id Random(Byte Length = 23)

            => new (RandomExtensions.RandomString(Length).ToUpper());

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a blog posting identification.
        /// </summary>
        /// <param name="Text">A text representation of a blog posting identification.</param>
        public static BlogPosting_Id Parse(String Text)
        {

            if (TryParse(Text, out BlogPosting_Id blogPostingId))
                return blogPostingId;

            throw new ArgumentException($"Invalid text representation of a blog posting identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a blog posting identification.
        /// </summary>
        /// <param name="Text">A text representation of a blog posting identification.</param>
        public static BlogPosting_Id? TryParse(String? Text)
        {

            if (Text is not null && TryParse(Text, out BlogPosting_Id blogPostingId))
                return blogPostingId;

            return null;

        }

        #endregion

        #region (static) TryParse(Text, out BlogPostingId)

        /// <summary>
        /// Try to parse the given string as a blog posting identification.
        /// </summary>
        /// <param name="Text">A text representation of a blog posting identification.</param>
        /// <param name="BlogPostingId">The parsed blog posting identification.</param>
        public static Boolean TryParse(String Text, out BlogPosting_Id BlogPostingId)
        {

            Text = Text.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    BlogPostingId = new BlogPosting_Id(Text);
                    return true;
                }
                catch
                { }
            }

            BlogPostingId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this blog posting identification.
        /// </summary>
        public BlogPosting_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (BlogPostingId1, BlogPostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BlogPostingId1">A blog posting identification.</param>
        /// <param name="BlogPostingId2">Another blog posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (BlogPosting_Id BlogPostingId1,
                                           BlogPosting_Id BlogPostingId2)

            => BlogPostingId1.Equals(BlogPostingId2);

        #endregion

        #region Operator != (BlogPostingId1, BlogPostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BlogPostingId1">A blog posting identification.</param>
        /// <param name="BlogPostingId2">Another blog posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (BlogPosting_Id BlogPostingId1,
                                           BlogPosting_Id BlogPostingId2)

            => !BlogPostingId1.Equals(BlogPostingId2);

        #endregion

        #region Operator <  (BlogPostingId1, BlogPostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BlogPostingId1">A blog posting identification.</param>
        /// <param name="BlogPostingId2">Another blog posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (BlogPosting_Id BlogPostingId1,
                                          BlogPosting_Id BlogPostingId2)

            => BlogPostingId1.CompareTo(BlogPostingId2) < 0;

        #endregion

        #region Operator <= (BlogPostingId1, BlogPostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BlogPostingId1">A blog posting identification.</param>
        /// <param name="BlogPostingId2">Another blog posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (BlogPosting_Id BlogPostingId1,
                                           BlogPosting_Id BlogPostingId2)

            => BlogPostingId1.CompareTo(BlogPostingId2) <= 0;

        #endregion

        #region Operator >  (BlogPostingId1, BlogPostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BlogPostingId1">A blog posting identification.</param>
        /// <param name="BlogPostingId2">Another blog posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (BlogPosting_Id BlogPostingId1,
                                          BlogPosting_Id BlogPostingId2)

            => BlogPostingId1.CompareTo(BlogPostingId2) > 0;

        #endregion

        #region Operator >= (BlogPostingId1, BlogPostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BlogPostingId1">A blog posting identification.</param>
        /// <param name="BlogPostingId2">Another blog posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (BlogPosting_Id BlogPostingId1,
                                           BlogPosting_Id BlogPostingId2)

            => BlogPostingId1.CompareTo(BlogPostingId2) >= 0;

        #endregion

        #endregion

        #region IComparable<BlogPosting_Id> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object? Object)

            => Object is BlogPosting_Id blogPostingId
                   ? CompareTo(blogPostingId)
                   : throw new ArgumentException("The given object is not a blog posting identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(BlogPostingId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BlogPostingId">An object to compare with.</param>
        public Int32 CompareTo(BlogPosting_Id BlogPostingId)

            => String.Compare(InternalId,
                              BlogPostingId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<BlogPosting_Id> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object? Object)

            => Object is BlogPosting_Id blogPostingId &&
                   Equals(blogPostingId);

        #endregion

        #region Equals(BlogPostingId)

        /// <summary>
        /// Compares two blog posting identifications for equality.
        /// </summary>
        /// <param name="BlogPostingId">A blog posting identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(BlogPosting_Id BlogPostingId)

            => String.Equals(InternalId,
                             BlogPostingId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
        /// <returns>The hash code of this object.</returns>
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
