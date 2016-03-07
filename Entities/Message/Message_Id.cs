/*
 * Copyright (c) 2014-2015, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using System.Text.RegularExpressions;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace org.GraphDefined.UsersAPI
{

    /// <summary>
    /// The unique identification of a news item.
    /// </summary>
    public class Message_Id : IId,
                              IEquatable<Message_Id>,
                              IComparable<Message_Id>

    {

        #region Data

        //ToDo: Replace with better randomness!
        private static readonly Random _Random = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// The internal identification.
        /// </summary>
        protected readonly String _Id;

        #endregion

        #region Properties

        #region Length

        /// <summary>
        /// Returns the length of the identificator.
        /// </summary>
        public UInt64 Length
        {
            get
            {
                return (UInt64) (_Id.Length);
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new user group identification based on the given string.
        /// </summary>
        /// <param name="String">The string representation of the user group identification.</param>
        private Message_Id(String String)
        {
            _Id = String.Trim();
        }

        #endregion


        #region Parse(Text)

        /// <summary>
        /// Parse the given string as an Electric Vehicle Charging Group Identification (EVCG Id)
        /// </summary>
        /// <param name="Text">A text representation of an Electric Vehicle Charging Group identification.</param>
        public static Message_Id Parse(String Text)
        {
            return new Message_Id(Text);
        }

        #endregion

        #region TryParse(Text, out GroupId)

        /// <summary>
        /// Parse the given string as an Electric Vehicle Charging Group Identification (EVCG Id)
        /// </summary>
        /// <param name="Text">A text representation of an Electric Vehicle Charging Group identification.</param>
        /// <param name="GroupId">The parsed Electric Vehicle Charging Group identification.</param>
        public static Boolean TryParse(String Text, out Message_Id GroupId)
        {
            try
            {
                GroupId = new Message_Id(Text);
                return true;
            }
            catch (Exception)
            {
                GroupId = null;
                return false;
            }
        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone an Message_Id.
        /// </summary>
        public Message_Id Clone
        {
            get
            {

                return new Message_Id(new String(_Id.ToCharArray()));

            }
        }

        #endregion


        #region Operator overloading

        #region Operator == (Message_Id1, Message_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Message_Id1">A Message_Id.</param>
        /// <param name="Message_Id2">Another Message_Id.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Message_Id Message_Id1, Message_Id Message_Id2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Message_Id1, Message_Id2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) Message_Id1 == null) || ((Object) Message_Id2 == null))
                return false;

            return Message_Id1.Equals(Message_Id2);

        }

        #endregion

        #region Operator != (Message_Id1, Message_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Message_Id1">A Message_Id.</param>
        /// <param name="Message_Id2">Another Message_Id.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Message_Id Message_Id1, Message_Id Message_Id2)
        {
            return !(Message_Id1 == Message_Id2);
        }

        #endregion

        #region Operator <  (Message_Id1, Message_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Message_Id1">A Message_Id.</param>
        /// <param name="Message_Id2">Another Message_Id.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Message_Id Message_Id1, Message_Id Message_Id2)
        {

            if ((Object) Message_Id1 == null)
                throw new ArgumentNullException("The given Message_Id1 must not be null!");

            return Message_Id1.CompareTo(Message_Id2) < 0;

        }

        #endregion

        #region Operator <= (Message_Id1, Message_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Message_Id1">A Message_Id.</param>
        /// <param name="Message_Id2">Another Message_Id.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Message_Id Message_Id1, Message_Id Message_Id2)
        {
            return !(Message_Id1 > Message_Id2);
        }

        #endregion

        #region Operator >  (Message_Id1, Message_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Message_Id1">A Message_Id.</param>
        /// <param name="Message_Id2">Another Message_Id.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Message_Id Message_Id1, Message_Id Message_Id2)
        {

            if ((Object) Message_Id1 == null)
                throw new ArgumentNullException("The given Message_Id1 must not be null!");

            return Message_Id1.CompareTo(Message_Id2) > 0;

        }

        #endregion

        #region Operator >= (Message_Id1, Message_Id2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Message_Id1">A Message_Id.</param>
        /// <param name="Message_Id2">Another Message_Id.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Message_Id Message_Id1, Message_Id Message_Id2)
        {
            return !(Message_Id1 < Message_Id2);
        }

        #endregion

        #endregion

        #region IComparable<Message_Id> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an Message_Id.
            var Message_Id = Object as Message_Id;
            if ((Object) Message_Id == null)
                throw new ArgumentException("The given object is not a Message_Id!");

            return CompareTo(Message_Id);

        }

        #endregion

        #region CompareTo(Message_Id)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Message_Id">An object to compare with.</param>
        public Int32 CompareTo(Message_Id Message_Id)
        {

            if ((Object) Message_Id == null)
                throw new ArgumentNullException("The given Message_Id must not be null!");

            return this._Id.CompareTo(Message_Id._Id);

        }

        #endregion

        #endregion

        #region IEquatable<Message_Id> Members

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

            // Check if the given object is an Message_Id.
            var Message_Id = Object as Message_Id;
            if ((Object) Message_Id == null)
                return false;

            return this.Equals(Message_Id);

        }

        #endregion

        #region Equals(Message_Id)

        /// <summary>
        /// Compares two Message_Ids for equality.
        /// </summary>
        /// <param name="GroupId">A Message_Id to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Message_Id GroupId)
        {

            if ((Object) GroupId == null)
                return false;

            return _Id.Equals(GroupId._Id);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()
        {
            return _Id.GetHashCode();
        }

        #endregion

        #region ToString()

        /// <summary>
        /// Return a string represtentation of this object.
        /// </summary>
        public override String ToString()
        {
            return _Id;
        }

        #endregion

    }

}
