/*
 * Copyright (c) 2014-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// The unique parcel tracking identification.
    /// </summary>
    public readonly struct ParcelTracking_Id : IId<ParcelTracking_Id>
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
        /// The length of the parcel tracking identificator.
        /// </summary>
        public UInt64 Length
            => (UInt64) InternalId?.Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new parcel tracking identification based on the given string.
        /// </summary>
        private ParcelTracking_Id(String Text)
        {
            InternalId = Text;
        }

        #endregion


        #region Parse   (Text)

        /// <summary>
        /// Parse the given string as a parcel tracking identification.
        /// </summary>
        /// <param name="Text">A text-representation of a parcel tracking identification.</param>
        public static ParcelTracking_Id Parse(String Text)
        {

            if (TryParse(Text, out ParcelTracking_Id parcelTrackingId))
                return parcelTrackingId;

            throw new ArgumentException("Invalid text-representation of a parcel tracking identification: '" + Text + "'!",
                                        nameof(Text));

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a parcel tracking identification.
        /// </summary>
        /// <param name="Text">A text-representation of a parcel tracking identification.</param>
        public static ParcelTracking_Id? TryParse(String Text)
        {

            if (TryParse(Text, out ParcelTracking_Id parcelTrackingId))
                return parcelTrackingId;

            return default;

        }

        #endregion

        #region TryParse(Text, out ParcelTrackingId)

        /// <summary>
        /// Try to parse the given string as a parcel tracking identification.
        /// </summary>
        /// <param name="Text">A text-representation of a parcel tracking identification.</param>
        /// <param name="ParcelTrackingId">The parsed parcel tracking identification.</param>
        public static Boolean TryParse(String Text, out ParcelTracking_Id ParcelTrackingId)
        {

            Text = Text?.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    ParcelTrackingId = new ParcelTracking_Id(Text);
                    return true;
                }
                catch
                { }
            }

            ParcelTrackingId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this parcel tracking identification.
        /// </summary>
        public ParcelTracking_Id Clone

            => new ParcelTracking_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (ParcelTrackingIdId1, ParcelTrackingIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParcelTrackingIdId1">A parcel tracking identification.</param>
        /// <param name="ParcelTrackingIdId2">Another parcel tracking identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ParcelTracking_Id ParcelTrackingIdId1,
                                           ParcelTracking_Id ParcelTrackingIdId2)

            => ParcelTrackingIdId1.Equals(ParcelTrackingIdId2);

        #endregion

        #region Operator != (ParcelTrackingIdId1, ParcelTrackingIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParcelTrackingIdId1">A parcel tracking identification.</param>
        /// <param name="ParcelTrackingIdId2">Another parcel tracking identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ParcelTracking_Id ParcelTrackingIdId1,
                                           ParcelTracking_Id ParcelTrackingIdId2)

            => !ParcelTrackingIdId1.Equals(ParcelTrackingIdId2);

        #endregion

        #region Operator <  (ParcelTrackingIdId1, ParcelTrackingIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParcelTrackingIdId1">A parcel tracking identification.</param>
        /// <param name="ParcelTrackingIdId2">Another parcel tracking identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ParcelTracking_Id ParcelTrackingIdId1,
                                          ParcelTracking_Id ParcelTrackingIdId2)

            => ParcelTrackingIdId1.CompareTo(ParcelTrackingIdId2) < 0;

        #endregion

        #region Operator <= (ParcelTrackingIdId1, ParcelTrackingIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParcelTrackingIdId1">A parcel tracking identification.</param>
        /// <param name="ParcelTrackingIdId2">Another parcel tracking identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ParcelTracking_Id ParcelTrackingIdId1,
                                           ParcelTracking_Id ParcelTrackingIdId2)

            => ParcelTrackingIdId1.CompareTo(ParcelTrackingIdId2) <= 0;

        #endregion

        #region Operator >  (ParcelTrackingIdId1, ParcelTrackingIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParcelTrackingIdId1">A parcel tracking identification.</param>
        /// <param name="ParcelTrackingIdId2">Another parcel tracking identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ParcelTracking_Id ParcelTrackingIdId1,
                                          ParcelTracking_Id ParcelTrackingIdId2)

            => ParcelTrackingIdId1.CompareTo(ParcelTrackingIdId2) > 0;

        #endregion

        #region Operator >= (ParcelTrackingIdId1, ParcelTrackingIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParcelTrackingIdId1">A parcel tracking identification.</param>
        /// <param name="ParcelTrackingIdId2">Another parcel tracking identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ParcelTracking_Id ParcelTrackingIdId1,
                                           ParcelTracking_Id ParcelTrackingIdId2)

            => ParcelTrackingIdId1.CompareTo(ParcelTrackingIdId2) >= 0;

        #endregion

        #endregion

        #region IComparable<ParcelTrackingId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is ParcelTracking_Id parcelTrackingId
                   ? CompareTo(parcelTrackingId)
                   : throw new ArgumentException("The given object is not a parcel tracking identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(ParcelTrackingId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="ParcelTrackingId">An object to compare with.</param>
        public Int32 CompareTo(ParcelTracking_Id ParcelTrackingId)

            => String.Compare(InternalId,
                              ParcelTrackingId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<ParcelTrackingId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is ParcelTracking_Id parcelTrackingId &&
                   Equals(parcelTrackingId);

        #endregion

        #region Equals(ParcelTrackingId)

        /// <summary>
        /// Compares two ParcelTrackingIds for equality.
        /// </summary>
        /// <param name="ParcelTrackingId">A parcel tracking identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ParcelTracking_Id ParcelTrackingId)

            => String.Equals(InternalId,
                             ParcelTrackingId.InternalId,
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
