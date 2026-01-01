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
    /// The unique identification of a FAQ.
    /// </summary>
    public readonly struct FAQ_Id : IId,
                                    IEquatable<FAQ_Id>,
                                    IComparable<FAQ_Id>

    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly String InternalId;

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
        /// The length of the faq identificator.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new faq identification based on the given string.
        /// </summary>
        private FAQ_Id(String Text)
        {
            InternalId = Text;
        }

        #endregion


        #region (static) Random(Length)

        /// <summary>
        /// Create a new faq identification.
        /// </summary>
        /// <param name="Length">The expected length of the faq identification.</param>
        public static FAQ_Id Random(Byte Length = 15)

            => new (RandomExtensions.RandomString(Length).ToUpper());

        #endregion

        #region Parse   (Text)

        /// <summary>
        /// Parse the given string as a faq identification.
        /// </summary>
        /// <param name="Text">A text representation of a faq identification.</param>
        public static FAQ_Id Parse(String Text)
        {

            if (TryParse(Text, out FAQ_Id faqId))
                return faqId;

            throw new ArgumentException($"Invalid text representation of a faq identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a faq identification.
        /// </summary>
        /// <param name="Text">A text representation of a faq identification.</param>
        public static FAQ_Id? TryParse(String Text)
        {

            if (TryParse(Text, out FAQ_Id faqId))
                return faqId;

            return null;

        }

        #endregion

        #region TryParse(Text, out FAQId)

        /// <summary>
        /// Try to parse the given string as a faq identification.
        /// </summary>
        /// <param name="Text">A text representation of a faq identification.</param>
        /// <param name="FAQId">The parsed faq identification.</param>
        public static Boolean TryParse(String Text, out FAQ_Id FAQId)
        {

            Text = Text?.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    FAQId = new FAQ_Id(Text);
                    return true;
                }
                catch
                { }
            }

            FAQId = default;
            return false;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this faq identification.
        /// </summary>
        public FAQ_Id Clone()

            => new (
                   InternalId.CloneString()
               );

        #endregion


        #region Operator overloading

        #region Operator == (FAQIdId1, FAQIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FAQIdId1">A faq identification.</param>
        /// <param name="FAQIdId2">Another faq identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (FAQ_Id FAQIdId1,
                                           FAQ_Id FAQIdId2)

            => FAQIdId1.Equals(FAQIdId2);

        #endregion

        #region Operator != (FAQIdId1, FAQIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FAQIdId1">A faq identification.</param>
        /// <param name="FAQIdId2">Another faq identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (FAQ_Id FAQIdId1,
                                           FAQ_Id FAQIdId2)

            => !FAQIdId1.Equals(FAQIdId2);

        #endregion

        #region Operator <  (FAQIdId1, FAQIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FAQIdId1">A faq identification.</param>
        /// <param name="FAQIdId2">Another faq identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (FAQ_Id FAQIdId1,
                                          FAQ_Id FAQIdId2)

            => FAQIdId1.CompareTo(FAQIdId2) < 0;

        #endregion

        #region Operator <= (FAQIdId1, FAQIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FAQIdId1">A faq identification.</param>
        /// <param name="FAQIdId2">Another faq identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (FAQ_Id FAQIdId1,
                                           FAQ_Id FAQIdId2)

            => FAQIdId1.CompareTo(FAQIdId2) <= 0;

        #endregion

        #region Operator >  (FAQIdId1, FAQIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FAQIdId1">A faq identification.</param>
        /// <param name="FAQIdId2">Another faq identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (FAQ_Id FAQIdId1,
                                          FAQ_Id FAQIdId2)

            => FAQIdId1.CompareTo(FAQIdId2) > 0;

        #endregion

        #region Operator >= (FAQIdId1, FAQIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FAQIdId1">A faq identification.</param>
        /// <param name="FAQIdId2">Another faq identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (FAQ_Id FAQIdId1,
                                           FAQ_Id FAQIdId2)

            => FAQIdId1.CompareTo(FAQIdId2) >= 0;

        #endregion

        #endregion

        #region IComparable<FAQId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is FAQ_Id faqId
                   ? CompareTo(faqId)
                   : throw new ArgumentException("The given object is not a faq identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(FAQId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FAQId">An object to compare with.</param>
        public Int32 CompareTo(FAQ_Id FAQId)

            => String.Compare(InternalId,
                              FAQId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<FAQId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is FAQ_Id faqId &&
                   Equals(faqId);

        #endregion

        #region Equals(FAQId)

        /// <summary>
        /// Compares two FAQIds for equality.
        /// </summary>
        /// <param name="FAQId">A faq identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(FAQ_Id FAQId)

            => String.Equals(InternalId,
                             FAQId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the hash code of this object.
        /// </summary>
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
