/*
 * Copyright (c) 2014-2017, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using System.Security;
using System.Runtime.InteropServices;

#endregion

namespace org.GraphDefined.OpenData.Users
{

    public class LoginPassword : IEquatable<LoginPassword>,
                                 IComparable<LoginPassword>,
                                 IComparable
    {

        #region Properties

        /// <summary>
        /// The login.
        /// </summary>
        public User_Id   Login      { get; }

        /// <summary>
        /// The password.
        /// </summary>
        public Password  Password   { get; }

        #endregion

        #region Constructor(s)

        public LoginPassword(User_Id   Login,
                             Password  Password)
        {

            this.Login     = Login;
            this.Password  = Password;

        }

        #endregion


        #region CheckPassword(Text)

        public Boolean CheckPassword(String Text)
            => this.Password.Verify(Text);

        #endregion

        #region CheckPassword(SecurePassword)

        public Boolean CheckPassword(Password Password)
            => Password.Equals(Password);

        #endregion


        #region IComparable<LoginPassword> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is a Login/Password object.
            var _LoginPassword = Object as LoginPassword;
            if ((Object) _LoginPassword == null)
                throw new ArgumentException("The given object is not an user group!");

            return CompareTo(_LoginPassword);

        }

        #endregion

        #region CompareTo(LoginPassword)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="LoginPassword">A Login/Password object to compare with.</param>
        public Int32 CompareTo(LoginPassword LoginPassword)
        {

            if ((Object) LoginPassword == null)
                throw new ArgumentNullException("The given Login/Password object must not be null!");

            return Login.CompareTo(LoginPassword.Login);

        }

        #endregion

        #endregion

        #region IEquatable<LoginPassword> Members

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

            // Check if the given object is a Login/Password object.
            var _LoginPassword = Object as LoginPassword;
            if ((Object) _LoginPassword == null)
                return false;

            return this.Equals(_LoginPassword);

        }

        #endregion

        #region Equals(LoginPassword)

        /// <summary>
        /// Compares two Login/Password objects for equality.
        /// </summary>
        /// <param name="LoginPassword">A Login/Password object to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(LoginPassword LoginPassword)
        {

            if ((Object) LoginPassword == null)
                return false;

            return Login.Equals(LoginPassword.Login);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            return Login.GetHashCode();
        }

        #endregion

        #region ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()
        {
            return Login.ToString();
        }

        #endregion

    }

}
