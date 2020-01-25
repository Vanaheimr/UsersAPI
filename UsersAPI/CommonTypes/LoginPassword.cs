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

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// A user identification and password combination.
    /// </summary>
    public class LoginPassword : IEquatable<LoginPassword>,
                                 IComparable<LoginPassword>,
                                 IComparable
    {

        #region Properties

        /// <summary>
        /// The unique user identification.
        /// </summary>
        public User_Id   Login      { get; }

        /// <summary>
        /// The password of the user.
        /// </summary>
        public Password  Password   { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new user identification and password combination.
        /// </summary>
        /// <param name="Login">The unique user identification.</param>
        /// <param name="Password">The password of the user.</param>
        public LoginPassword(User_Id   Login,
                             Password  Password)
        {

            this.Login     = Login;
            this.Password  = Password;

        }

        #endregion


        #region VerifyPassword(Password)

        /// <summary>
        /// Verify the given password.
        /// </summary>
        /// <param name="Password">A password to verify.</param>
        public Boolean VerifyPassword(String Password)
            => this.Password.Verify(Password);

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

            return Equals(_LoginPassword);

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

            return Login.   Equals(LoginPassword.Login) &&
                   Password.Equals(LoginPassword.Password);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()

            => Login.   GetHashCode() ^
               Password.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => Login.ToString();

        #endregion

    }

}
