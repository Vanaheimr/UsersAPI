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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Illias.Votes;
using org.GraphDefined.Vanaheimr.Styx.Arrows;
using System.Security.Cryptography;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// A verification token.
    /// </summary>
    public class VerificationToken : IEquatable<VerificationToken>,
                                     IComparable<VerificationToken>,
                                     IComparable
    {

        #region Data

        private readonly String InternalId;

        #endregion

        #region Properties

        public User_Id  Login    { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new verification token.
        /// </summary>
        /// <param name="Seed">The cryptographic seed.</param>
        /// <param name="UserId">The user identification.</param>
        public VerificationToken(String   Seed,
                                 User_Id  UserId)
        {

            this.InternalId  = new SHA256Managed().ComputeHash((Guid.NewGuid().ToString() + Seed).ToUTF8Bytes()).ToHexString();
            this.Login       = UserId;

        }

        #endregion


        #region IComparable<VerificationToken> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is a verification token.
            var _VerificationToken = Object as VerificationToken;
            if ((Object) _VerificationToken == null)
                throw new ArgumentException("The given object is not a verification token!");

            return CompareTo(_VerificationToken);

        }

        #endregion

        #region CompareTo(VerificationToken)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="VerificationToken">An user groups object to compare with.</param>
        public Int32 CompareTo(VerificationToken VerificationToken)
        {

            if ((Object) VerificationToken == null)
                throw new ArgumentNullException("The given verification token must not be null!");

            return InternalId.CompareTo(VerificationToken.InternalId);

        }

        #endregion

        #endregion

        #region IEquatable<VerificationToken> Members

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

            // Check if the given object is a verification token.
            var _VerificationToken = Object as VerificationToken;
            if ((Object) _VerificationToken == null)
                return false;

            return this.Equals(_VerificationToken);

        }

        #endregion

        #region Equals(VerificationToken)

        /// <summary>
        /// Compares two verification tokens for equality.
        /// </summary>
        /// <param name="Operator">A verification token to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(VerificationToken Operator)
        {

            if ((Object) Operator == null)
                return false;

            return InternalId.Equals(Operator.InternalId);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            return InternalId.GetHashCode();
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
        {
            return InternalId.ToString();
        }

        #endregion

    }

}
