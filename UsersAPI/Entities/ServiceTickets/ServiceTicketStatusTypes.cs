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
    /// The unique identification of a communicator.
    /// </summary>
    public readonly partial struct ServiceTicketStatusTypes : IId,
                                                              IEquatable<ServiceTicketStatusTypes>,
                                                              IComparable<ServiceTicketStatusTypes>
    {

        #region Data

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly IEnumerable<String>  InternalId;

        private readonly String               InternalText;

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
            => InternalId.IsNeitherNullNorEmpty();

        /// <summary>
        /// The length of the service ticket status.
        /// </summary>
        public UInt64 Length
            => (UInt64) (InternalText?.Length ?? 0);

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new unique service ticket status based on the given string.
        /// </summary>
        /// <param name="Status">The string representation of the service ticket status.</param>
        private ServiceTicketStatusTypes(params String[]  Status)
        {
            this.InternalId    = Status;
            this.InternalText  = Status.AggregateWith('.');
        }

        #endregion


        #region (static) Parse   (Text)

        /// <summary>
        /// Parse the given string as a service ticket status.
        /// </summary>
        /// <param name="Text">A text representation of a service ticket status.</param>
        public static ServiceTicketStatusTypes Parse(String Text)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a service ticket status must not be null or empty!");

            #endregion

            if (TryParse(Text, out ServiceTicketStatusTypes status))
                return status;

            throw new ArgumentException("The given text representation of a service ticket status is invalid!", nameof(Text));

        }

        /// <summary>
        /// Parse the given string as a service ticket status.
        /// </summary>
        /// <param name="Text">A text representation of a service ticket status.</param>
        public static ServiceTicketStatusTypes Create(params String[] Text)
        {

            #region Initial checks

            if (Text != null && Text.Any())
                Text = Text.Select (_ => _?.Trim()).
                            Where  (_ => _.IsNotNullOrEmpty()).
                            ToArray();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a service ticket status must not be null or empty!");

            #endregion

            if (TryParse(Text, out ServiceTicketStatusTypes status))
                return status;

            throw new ArgumentException("The given text representation of a service ticket status is invalid!", nameof(Text));

        }

        /// <summary>
        /// Parse the given string as a service ticket status.
        /// </summary>
        /// <param name="Text">A text representation of a service ticket status.</param>
        public static ServiceTicketStatusTypes Parse(IEnumerable<String> Text)
        {

            #region Initial checks

            if (Text != null && Text.Any())
                Text = Text.Select (_ => _?.Trim()).
                            Where  (_ => _.IsNotNullOrEmpty());

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a service ticket status must not be null or empty!");

            #endregion

            if (TryParse(Text, out ServiceTicketStatusTypes status))
                return status;

            throw new ArgumentException("The given text representation of a service ticket status is invalid!", nameof(Text));

        }

        #endregion

        #region (static) TryParse(Text)

        /// <summary>
        /// Try to parse the given string as a service ticket status.
        /// </summary>
        /// <param name="Text">A text representation of a service ticket status.</param>
        public static ServiceTicketStatusTypes? TryParse(String Text)
        {

            if (TryParse(Text, out ServiceTicketStatusTypes status))
                return status;

            return new ServiceTicketStatusTypes?();

        }

        /// <summary>
        /// Try to parse the given enumeration of strings as a service ticket status.
        /// </summary>
        /// <param name="Text">A text representation of a service ticket status.</param>
        public static ServiceTicketStatusTypes? TryCreate(params String[] Text)
        {

            if (TryParse(Text, out ServiceTicketStatusTypes status))
                return status;

            return new ServiceTicketStatusTypes?();

        }

        /// <summary>
        /// Try to parse the given enumeration of strings as a service ticket status.
        /// </summary>
        /// <param name="Text">A text representation of a service ticket status.</param>
        public static ServiceTicketStatusTypes? TryParse(IEnumerable<String> Text)
        {

            if (TryParse(Text, out ServiceTicketStatusTypes status))
                return status;

            return new ServiceTicketStatusTypes?();

        }

        #endregion

        #region (static) TryParse(Text, out Status)

        /// <summary>
        /// Try to parse the given string as a service ticket status.
        /// </summary>
        /// <param name="Text">A text representation of a service ticket status.</param>
        /// <param name="Status">The parsed service ticket status.</param>
        public static Boolean TryParse(String Text, out ServiceTicketStatusTypes Status)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
            {
                Status = default;
                return false;
            }

            #endregion

            if (Text.Contains("."))
            {

                var Splitted = Text.Split(new Char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                if (Splitted.Length > 0)
                {
                    Status = new ServiceTicketStatusTypes(Splitted.Select(_ => _.Trim()).ToArray());
                    return true;
                }

            }

            Status = new ServiceTicketStatusTypes(Text);
            return true;

        }
        public static Boolean TryParse(IEnumerable<String> Text, out ServiceTicketStatusTypes Status)
        {

            #region Initial checks

            if (Text != null && Text.Any())
                Text = Text.Select (_ => _?.Trim()).
                            Where  (_ => _.IsNotNullOrEmpty()).
                            ToArray();

            if (Text.IsNullOrEmpty())
            {
                Status = default;
                return false;
            }

            #endregion

            if (Text.Any(_ => _.Contains(".")))
                Text = Text.AggregateWith(".").Split(new Char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            Status = new ServiceTicketStatusTypes(Text.ToArray());
            return true;

        }

        #endregion

        #region Clone()

        /// <summary>
        /// Clone this service ticket status.
        /// </summary>
        public ServiceTicketStatusTypes Clone()

            => new (
                   InternalId.AggregateWith('.')
               );

        #endregion


        /// <summary>
        /// The service ticket is new.
        /// </summary>
        public static ServiceTicketStatusTypes New
            => Parse("new");

        /// <summary>
        /// The service ticket was received and needs further analysis.
        /// </summary>
        public static ServiceTicketStatusTypes Analysis
            => Parse("analysis");

        /// <summary>
        /// The service ticket was closed.
        /// </summary>
        public static ServiceTicketStatusTypes Closed
            => Parse("closed");


        #region Operator overloading

        #region Operator == (Status1, Status2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Status1">A service ticket status.</param>
        /// <param name="Status2">Another service ticket status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (ServiceTicketStatusTypes Status1, ServiceTicketStatusTypes Status2)
            => Status1.Equals(Status2);

        #endregion

        #region Operator != (Status1, Status2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Status1">A service ticket status.</param>
        /// <param name="Status2">Another service ticket status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (ServiceTicketStatusTypes Status1, ServiceTicketStatusTypes Status2)
            => !Status1.Equals(Status2);

        #endregion

        #region Operator <  (Status1, Status2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Status1">A service ticket status.</param>
        /// <param name="Status2">Another service ticket status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (ServiceTicketStatusTypes Status1, ServiceTicketStatusTypes Status2)
            => Status1.CompareTo(Status2) < 0;

        #endregion

        #region Operator <= (Status1, Status2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Status1">A service ticket status.</param>
        /// <param name="Status2">Another service ticket status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (ServiceTicketStatusTypes Status1, ServiceTicketStatusTypes Status2)
            => Status1.CompareTo(Status2) <= 0;

        #endregion

        #region Operator >  (Status1, Status2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Status1">A service ticket status.</param>
        /// <param name="Status2">Another service ticket status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (ServiceTicketStatusTypes Status1, ServiceTicketStatusTypes Status2)
            => Status1.CompareTo(Status2) > 0;

        #endregion

        #region Operator >= (Status1, Status2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Status1">A service ticket status.</param>
        /// <param name="Status2">Another service ticket status.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (ServiceTicketStatusTypes Status1, ServiceTicketStatusTypes Status2)
            => Status1.CompareTo(Status2) >= 0;

        #endregion

        #endregion

        #region IComparable<Status> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object is null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is ServiceTicketStatusTypes))
                throw new ArgumentException("The given object is not a service ticket status!",
                                            nameof(Object));

            return CompareTo((ServiceTicketStatusTypes) Object);

        }

        #endregion

        #region CompareTo(Status)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Status">An object to compare with.</param>
        public Int32 CompareTo(ServiceTicketStatusTypes Status)

            => String.Compare(InternalText,
                              Status.InternalText,
                              StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region IEquatable<Status> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is null)
                return false;

            if (!(Object is ServiceTicketStatusTypes))
                return false;

            return Equals((ServiceTicketStatusTypes) Object);

        }

        #endregion

        #region Equals(Status)

        /// <summary>
        /// Compares two service ticket status for equality.
        /// </summary>
        /// <param name="Status">An service ticket status to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(ServiceTicketStatusTypes Status)

            => InternalText.Equals(Status.InternalText,
                                   StringComparison.OrdinalIgnoreCase);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
            => InternalText.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => InternalText;

        #endregion

    }

}
