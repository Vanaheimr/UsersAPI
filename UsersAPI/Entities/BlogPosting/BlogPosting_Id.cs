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

namespace social.OpenData.UsersAPI.Postings
{

    /// <summary>
    /// The unique identification of a blog posting.
    /// </summary>
    public struct BlogPosting_Id : IId,
                                   IEquatable<BlogPosting_Id>,
                                   IComparable<BlogPosting_Id>

    {

        #region Data

        private static readonly Random _random = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String  InternalId;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this identification is null or empty.
        /// </summary>
        public Boolean IsNullOrEmpty
            => InternalId.IsNullOrEmpty();

        /// <summary>
        /// The length of the posting identification.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new posting identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the posting identification.</param>
        private BlogPosting_Id(String  String)
        {
            this.InternalId  = String.ToLower();
        }

        #endregion


        #region (static) Random  (Size)

        /// <summary>
        /// Create a random blog posting identification.
        /// </summary>
        /// <param name="Size">The expected size of the blog posting identification.</param>
        public static BlogPosting_Id Random(UInt16? Size = 64)

            => new BlogPosting_Id(_random.RandomString(Size ?? 64));

        #endregion

        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a blog posting identification.
        /// </summary>
        /// <param name="Text">A text representation of a blog posting identification.</param>
        public static BlogPosting_Id Parse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a blog posting identification must not be null or empty!");

            #endregion

            return new BlogPosting_Id(Text);

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a blog posting identification.
        /// </summary>
        /// <param name="Text">A text representation of a blog posting identification.</param>
        public static BlogPosting_Id? TryParse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a blog posting identification must not be null or empty!");

            #endregion

            if (TryParse(Text, out BlogPosting_Id _PostingId))
                return _PostingId;

            return new BlogPosting_Id?();

        }

        #endregion

        #region (static) TryParse(Text, out PostingId)

        /// <summary>
        /// Try to parse the given string as a blog posting identification.
        /// </summary>
        /// <param name="Text">A text representation of a blog posting identification.</param>
        /// <param name="PostingId">The parsed posting identification.</param>
        public static Boolean TryParse(String Text, out BlogPosting_Id PostingId)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a blog posting identification must not be null or empty!");

            String Realm = null;

            #endregion

            try
            {
                PostingId = new BlogPosting_Id(Text);
                return true;
            }
            catch (Exception)
            {
                PostingId = default(BlogPosting_Id);
                return false;
            }

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this posting identification.
        /// </summary>

        public BlogPosting_Id Clone

            => new BlogPosting_Id(new String(InternalId.ToCharArray()));

        #endregion


        #region Operator overloading

        #region Operator == (PostingId1, PostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PostingId1">a blog posting identification.</param>
        /// <param name="PostingId2">Another posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (BlogPosting_Id PostingId1, BlogPosting_Id PostingId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(PostingId1, PostingId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) PostingId1 == null) || ((Object) PostingId2 == null))
                return false;

            return PostingId1.Equals(PostingId2);

        }

        #endregion

        #region Operator != (PostingId1, PostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PostingId1">a blog posting identification.</param>
        /// <param name="PostingId2">Another posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (BlogPosting_Id PostingId1, BlogPosting_Id PostingId2)
            => !(PostingId1 == PostingId2);

        #endregion

        #region Operator <  (PostingId1, PostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PostingId1">a blog posting identification.</param>
        /// <param name="PostingId2">Another posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (BlogPosting_Id PostingId1, BlogPosting_Id PostingId2)
        {

            if ((Object) PostingId1 == null)
                throw new ArgumentNullException(nameof(PostingId1), "The given PostingId1 must not be null!");

            return PostingId1.CompareTo(PostingId2) < 0;

        }

        #endregion

        #region Operator <= (PostingId1, PostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PostingId1">a blog posting identification.</param>
        /// <param name="PostingId2">Another posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (BlogPosting_Id PostingId1, BlogPosting_Id PostingId2)
            => !(PostingId1 > PostingId2);

        #endregion

        #region Operator >  (PostingId1, PostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PostingId1">a blog posting identification.</param>
        /// <param name="PostingId2">Another posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (BlogPosting_Id PostingId1, BlogPosting_Id PostingId2)
        {

            if ((Object) PostingId1 == null)
                throw new ArgumentNullException(nameof(PostingId1), "The given PostingId1 must not be null!");

            return PostingId1.CompareTo(PostingId2) > 0;

        }

        #endregion

        #region Operator >= (PostingId1, PostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PostingId1">a blog posting identification.</param>
        /// <param name="PostingId2">Another posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (BlogPosting_Id PostingId1, BlogPosting_Id PostingId2)
            => !(PostingId1 < PostingId2);

        #endregion

        #endregion

        #region IComparable<PostingId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is BlogPosting_Id))
                throw new ArgumentException("The given object is not a blog posting identification!",
                                            nameof(Object));

            return CompareTo((BlogPosting_Id) Object);

        }

        #endregion

        #region CompareTo(PostingId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PostingId">An object to compare with.</param>
        public Int32 CompareTo(BlogPosting_Id PostingId)
        {

            if ((Object) PostingId == null)
                throw new ArgumentNullException(nameof(PostingId),  "The given posting identification must not be null!");

            return String.Compare(InternalId, PostingId.InternalId, StringComparison.OrdinalIgnoreCase);

        }

        #endregion

        #endregion

        #region IEquatable<PostingId> Members

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

            if (!(Object is BlogPosting_Id))
                return false;

            return Equals((BlogPosting_Id) Object);

        }

        #endregion

        #region Equals(PostingId)

        /// <summary>
        /// Compares two posting identifications for equality.
        /// </summary>
        /// <param name="PostingId">a blog posting identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(BlogPosting_Id PostingId)
        {

            if ((Object) PostingId == null)
                return false;

            return InternalId.Equals(PostingId.InternalId, StringComparison.OrdinalIgnoreCase);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => InternalId.ToLower().GetHashCode();

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
