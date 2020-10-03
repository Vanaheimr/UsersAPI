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
    /// The unique identification of an organization group.
    /// </summary>
    public readonly struct OrganizationGroup_Id : IId<OrganizationGroup_Id>
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
        /// The length of the organization group identification.
        /// </summary>
        public UInt64 Length

            => (UInt64) ((Context.IsNotNullOrEmpty()
                              ? Context.Length + 1
                              : 0) +
                         (Id?.Length ?? 0));

        #endregion

        #region Constructor(s)

        #region OrganizationGroup_Id(Text)

        /// <summary>
        /// Create a new organization group identification based on the given text.
        /// </summary>
        /// <param name="Text">The text representation of the organization group identification.</param>
        private OrganizationGroup_Id(String Text)

            : this(String.Empty,
                   Text)

        { }

        #endregion

        #region OrganizationGroup_Id(Context, Text)

        /// <summary>
        /// Create a new organization group identification based on the given text and context.
        /// </summary>
        /// <param name="Context">The text representation of the organization group identification context.</param>
        /// <param name="Text">The text representation of the organization group identification.</param>
        private OrganizationGroup_Id(String  Context,
                                      String  Text)
        {

            this.Context  = Context?.Trim();
            this.Id       = Text?.   Trim();

        }

        #endregion

        #endregion


        #region (static) Random  (Length)

        /// <summary>
        /// Create a new random organization group identification.
        /// </summary>
        /// <param name="Length">The expected length of the organization group identification.</param>
        public static OrganizationGroup_Id Random(Byte Length = 10)

            => new OrganizationGroup_Id(_random.RandomString(Length).ToUpper());

        #endregion

        #region (static) Parse   (         Text)

        /// <summary>
        /// Parse the given string as an organization group identification.
        /// </summary>
        /// <param name="Text">A text representation of an organization group identification.</param>
        public static OrganizationGroup_Id Parse(String Text)
        {

            if (TryParse(Text, out OrganizationGroup_Id organizationGroupId))
                return organizationGroupId;

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an organization group identification must not be null or empty!");

            throw new ArgumentException("The given text representation of an organization group identification is invalid!", nameof(Text));

        }

        #endregion

        #region (static) Parse   (Context, Text)

        /// <summary>
        /// Parse the given string as an organization group identification.
        /// </summary>
        /// <param name="Context">The text representation of the organization group identification context.</param>
        /// <param name="Text">A text representation of an organization group identification.</param>
        public static OrganizationGroup_Id Parse(String  Context,
                                                 String  Text)
        {

            if (TryParse(Context,
                         Text,
                         out OrganizationGroup_Id organizationGroupId))
            {
                return organizationGroupId;
            }

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of an organization group identification must not be null or empty!");

            throw new ArgumentException("The given text representation of an organization group identification is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(         Text)

        /// <summary>
        /// Try to parse the given text as an organization group identification.
        /// </summary>
        /// <param name="Text">A text representation of an organization group identification.</param>
        public static OrganizationGroup_Id? TryParse(String Text)
        {

            if (TryParse(Text,
                         out OrganizationGroup_Id organizationGroupId))
            {
                return organizationGroupId;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(Context, Text)

        /// <summary>
        /// Try to parse the given text as an organization group identification.
        /// </summary>
        /// <param name="Context">The text representation of the organization group identification context.</param>
        /// <param name="Text">A text representation of an organization group identification.</param>
        public static OrganizationGroup_Id? TryParse(String  Context,
                                                     String  Text)
        {

            if (TryParse(Context,
                         Text,
                         out OrganizationGroup_Id organizationGroupId))
            {
                return organizationGroupId;
            }

            return null;

        }

        #endregion

        #region (static) TryParse(         Text, out OrganizationGroupId)

        /// <summary>
        /// Try to parse the given text as an organization group identification.
        /// </summary>
        /// <param name="Text">A text representation of an organization group identification.</param>
        /// <param name="OrganizationGroupId">The parsed organization group identification.</param>
        public static Boolean TryParse(String Text, out OrganizationGroup_Id OrganizationGroupId)
        {

            if (Text.IsNullOrEmpty())
            {
                OrganizationGroupId = default;
                return false;
            }

            if (Text.Contains("."))
                return TryParse(Text.Substring(0, Text.LastIndexOf(".")),
                                Text.Substring(Text.LastIndexOf(".") + 1),
                                out OrganizationGroupId);

            return TryParse(String.Empty,
                            Text,
                            out OrganizationGroupId);

        }

        #endregion

        #region (static) TryParse(Context, Text, out OrganizationGroupId)

        /// <summary>
        /// Try to parse the given context and text as an organization group identification.
        /// </summary>
        /// <param name="Context">The text representation of the organization group identification context.</param>
        /// <param name="Text">A text representation of an organization group identification.</param>
        /// <param name="OrganizationGroupId">The parsed organization group identification.</param>
        public static Boolean TryParse(String                    Context,
                                       String                    Text,
                                       out OrganizationGroup_Id  OrganizationGroupId)
        {

            Text = Text?.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {

                    OrganizationGroupId = new OrganizationGroup_Id(Context?.Trim(),
                                                                     Text);

                    return true;

                }
                catch (Exception)
                { }
            }

            OrganizationGroupId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this organization group identification.
        /// </summary>
        public OrganizationGroup_Id Clone

            => new OrganizationGroup_Id(
                   new String(Context?.ToCharArray()),
                   new String(Id?.     ToCharArray())
               );

        #endregion


        //ToDo: Add "readonly" after upgrading to C# 8.0!

        #region Statics methods

        #region Unknown

        /// <summary>
        /// The model of the device is unknown.
        /// </summary>
        public static OrganizationGroup_Id Models_Unknown

            => new OrganizationGroup_Id("models", "Unknown");

        #endregion

        #region COM001

        ///// <summary>
        ///// The model of the device is COM001.
        ///// </summary>
        //public static OrganizationGroup_Id Models_COM001

        //    => new OrganizationGroup_Id("models", "COM001");

        #endregion

        #endregion


        #region Operator overloading

        #region Operator == (OrganizationGroupId1, OrganizationGroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OrganizationGroupId1">A organization group identification.</param>
        /// <param name="OrganizationGroupId2">Another organization group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (OrganizationGroup_Id OrganizationGroupId1,
                                           OrganizationGroup_Id OrganizationGroupId2)

            => OrganizationGroupId1.Equals(OrganizationGroupId2);

        #endregion

        #region Operator != (OrganizationGroupId1, OrganizationGroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OrganizationGroupId1">A organization group identification.</param>
        /// <param name="OrganizationGroupId2">Another organization group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (OrganizationGroup_Id OrganizationGroupId1,
                                           OrganizationGroup_Id OrganizationGroupId2)

            => !(OrganizationGroupId1 == OrganizationGroupId2);

        #endregion

        #region Operator <  (OrganizationGroupId1, OrganizationGroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OrganizationGroupId1">A organization group identification.</param>
        /// <param name="OrganizationGroupId2">Another organization group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (OrganizationGroup_Id OrganizationGroupId1,
                                          OrganizationGroup_Id OrganizationGroupId2)

            => OrganizationGroupId1.CompareTo(OrganizationGroupId2) < 0;

        #endregion

        #region Operator <= (OrganizationGroupId1, OrganizationGroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OrganizationGroupId1">A organization group identification.</param>
        /// <param name="OrganizationGroupId2">Another organization group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (OrganizationGroup_Id OrganizationGroupId1,
                                           OrganizationGroup_Id OrganizationGroupId2)

            => !(OrganizationGroupId1 > OrganizationGroupId2);

        #endregion

        #region Operator >  (OrganizationGroupId1, OrganizationGroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OrganizationGroupId1">A organization group identification.</param>
        /// <param name="OrganizationGroupId2">Another organization group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (OrganizationGroup_Id OrganizationGroupId1,
                                          OrganizationGroup_Id OrganizationGroupId2)

            => OrganizationGroupId1.CompareTo(OrganizationGroupId2) > 0;

        #endregion

        #region Operator >= (OrganizationGroupId1, OrganizationGroupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OrganizationGroupId1">A organization group identification.</param>
        /// <param name="OrganizationGroupId2">Another organization group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (OrganizationGroup_Id OrganizationGroupId1,
                                           OrganizationGroup_Id OrganizationGroupId2)

            => !(OrganizationGroupId1 < OrganizationGroupId2);

        #endregion

        #endregion

        #region IComparable<OrganizationGroupId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

             => Object is OrganizationGroup_Id organizationGroupId
                    ? CompareTo(organizationGroupId)
                    : throw new ArgumentException("The given object is not an organization group identification!",
                                                  nameof(Object));

        #endregion

        #region CompareTo(OrganizationGroupId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="OrganizationGroupId">An object to compare with.</param>
        public Int32 CompareTo(OrganizationGroup_Id OrganizationGroupId)
        {

            var c = String.Compare(Context,
                                   OrganizationGroupId.Context,
                                   StringComparison.OrdinalIgnoreCase);

            if (c == 0)
                c = String.Compare(Id,
                                   OrganizationGroupId.Id,
                                   StringComparison.OrdinalIgnoreCase);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<OrganizationGroupId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is OrganizationGroup_Id organizationGroupId &&
                   Equals(organizationGroupId);

        #endregion

        #region Equals(OrganizationGroupId)

        /// <summary>
        /// Compares two organization group identifications for equality.
        /// </summary>
        /// <param name="OrganizationGroupId">An organization group identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(OrganizationGroup_Id OrganizationGroupId)

            => String.Equals(Context,
                             OrganizationGroupId.Context,
                             StringComparison.OrdinalIgnoreCase) &&
               String.Equals(Id,
                             OrganizationGroupId.Id,
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
