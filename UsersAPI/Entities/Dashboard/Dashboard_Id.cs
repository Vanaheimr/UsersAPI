/*
 * Copyright (c) 2014-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
    /// The unique identification of a dashboard.
    /// </summary>
    public struct Dashboard_Id : IId,
                                 IEquatable<Dashboard_Id>,
                                 IComparable<Dashboard_Id>

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
        /// The length of the dashboard identificator.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalId?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new dashboard identification based on the given string.
        /// </summary>
        private Dashboard_Id(String Text)
        {
            InternalId = Text;
        }

        #endregion


        #region (static) Random(Length)

        /// <summary>
        /// Create a new dashboard identification.
        /// </summary>
        /// <param name="Length">The expected length of the dashboard identification.</param>
        public static Dashboard_Id Random(Byte Length = 15)

            => new (RandomExtensions.RandomString(Length).ToUpper());

        #endregion

        #region Parse   (Text)

        /// <summary>
        /// Parse the given string as a dashboard identification.
        /// </summary>
        /// <param name="Text">A text representation of a dashboard identification.</param>
        public static Dashboard_Id Parse(String Text)
        {

            if (TryParse(Text, out Dashboard_Id dashboardId))
                return dashboardId;

            throw new ArgumentException($"Invalid text representation of a dashboard identification: '{Text}'!",
                                        nameof(Text));

        }

        #endregion

        #region TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a dashboard identification.
        /// </summary>
        /// <param name="Text">A text representation of a dashboard identification.</param>
        public static Dashboard_Id? TryParse(String Text)
        {

            if (TryParse(Text, out Dashboard_Id dashboardId))
                return dashboardId;

            return null;

        }

        #endregion

        #region TryParse(Text, out DashboardId)

        /// <summary>
        /// Try to parse the given string as a dashboard identification.
        /// </summary>
        /// <param name="Text">A text representation of a dashboard identification.</param>
        /// <param name="DashboardId">The parsed dashboard identification.</param>
        public static Boolean TryParse(String Text, out Dashboard_Id DashboardId)
        {

            Text = Text?.Trim();

            if (Text.IsNotNullOrEmpty())
            {
                try
                {
                    DashboardId = new Dashboard_Id(Text);
                    return true;
                }
                catch
                { }
            }

            DashboardId = default;
            return false;

        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone this dashboard identification.
        /// </summary>
        public Dashboard_Id Clone

            => new Dashboard_Id(
                   new String(InternalId?.ToCharArray())
               );

        #endregion


        #region Operator overloading

        #region Operator == (DashboardIdId1, DashboardIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DashboardIdId1">A dashboard identification.</param>
        /// <param name="DashboardIdId2">Another dashboard identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Dashboard_Id DashboardIdId1,
                                           Dashboard_Id DashboardIdId2)

            => DashboardIdId1.Equals(DashboardIdId2);

        #endregion

        #region Operator != (DashboardIdId1, DashboardIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DashboardIdId1">A dashboard identification.</param>
        /// <param name="DashboardIdId2">Another dashboard identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Dashboard_Id DashboardIdId1,
                                           Dashboard_Id DashboardIdId2)

            => !DashboardIdId1.Equals(DashboardIdId2);

        #endregion

        #region Operator <  (DashboardIdId1, DashboardIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DashboardIdId1">A dashboard identification.</param>
        /// <param name="DashboardIdId2">Another dashboard identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Dashboard_Id DashboardIdId1,
                                          Dashboard_Id DashboardIdId2)

            => DashboardIdId1.CompareTo(DashboardIdId2) < 0;

        #endregion

        #region Operator <= (DashboardIdId1, DashboardIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DashboardIdId1">A dashboard identification.</param>
        /// <param name="DashboardIdId2">Another dashboard identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Dashboard_Id DashboardIdId1,
                                           Dashboard_Id DashboardIdId2)

            => DashboardIdId1.CompareTo(DashboardIdId2) <= 0;

        #endregion

        #region Operator >  (DashboardIdId1, DashboardIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DashboardIdId1">A dashboard identification.</param>
        /// <param name="DashboardIdId2">Another dashboard identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Dashboard_Id DashboardIdId1,
                                          Dashboard_Id DashboardIdId2)

            => DashboardIdId1.CompareTo(DashboardIdId2) > 0;

        #endregion

        #region Operator >= (DashboardIdId1, DashboardIdId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DashboardIdId1">A dashboard identification.</param>
        /// <param name="DashboardIdId2">Another dashboard identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Dashboard_Id DashboardIdId1,
                                           Dashboard_Id DashboardIdId2)

            => DashboardIdId1.CompareTo(DashboardIdId2) >= 0;

        #endregion

        #endregion

        #region IComparable<DashboardId> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is Dashboard_Id dashboardId
                   ? CompareTo(dashboardId)
                   : throw new ArgumentException("The given object is not a dashboard identification!",
                                                 nameof(Object));

        #endregion

        #region CompareTo(DashboardId)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DashboardId">An object to compare with.</param>
        public Int32 CompareTo(Dashboard_Id DashboardId)

            => String.Compare(InternalId,
                              DashboardId.InternalId,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<DashboardId> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)

            => Object is Dashboard_Id dashboardId &&
                   Equals(dashboardId);

        #endregion

        #region Equals(DashboardId)

        /// <summary>
        /// Compares two DashboardIds for equality.
        /// </summary>
        /// <param name="DashboardId">A dashboard identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Dashboard_Id DashboardId)

            => String.Equals(InternalId,
                             DashboardId.InternalId,
                             StringComparison.OrdinalIgnoreCase);

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
