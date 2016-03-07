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

namespace org.GraphDefined.UsersAPI
{

    public class LoginPassword : IEquatable<LoginPassword>, IComparable<LoginPassword>, IComparable
    {

        #region Properties

        #region Login

        private readonly User_Id _Login;

        public User_Id Login
        {
            get
            {
                return _Login;
            }
        }

        #endregion

        #region Realm

        private readonly String _Realm;

        public String Realm
        {
            get
            {
                return _Realm;
            }
        }

        #endregion

        #region Password

        private readonly SecureString _Password;

        public SecureString Password
        {
            get
            {
                return _Password;
            }
        }

        #endregion

        #endregion

        #region Constructor(s)

        public LoginPassword(User_Id  Login,
                             String   Password,
                             String   Realm     = null)
        {

            #region Initial checks

            #endregion

            this._Login     = Login;

            this._Password  = new SecureString();
            foreach (var character in Password)
                _Password.AppendChar(character);

            this._Realm = Realm.IsNotNullOrEmpty() ? Realm : String.Empty;

        }

        internal LoginPassword(User_Id       Login,
                               SecureString  Password,
                               String        Realm     = null,
                               String        Username  = null)
        {

            #region Initial checks

            #endregion

            this._Login     = Login;
            this._Password  = Password;
            this._Realm     = Realm.   IsNotNullOrEmpty() ? Realm    : String.Empty;

        }

        #endregion


        #region CheckPassword(Password)

        public Boolean CheckPassword(String Password)
        {

            var _Password  = new SecureString();

            foreach (var character in Password)
                _Password.AppendChar(character);

            return CheckPassword(_Password);

        }

        #endregion

        #region CheckPassword(SecurePassword)

        public Boolean CheckPassword(SecureString SecurePassword)
        {

            var bstr1 = IntPtr.Zero;
            var bstr2 = IntPtr.Zero;

            try
            {

                bstr1 = Marshal.SecureStringToBSTR(_Password);
                bstr2 = Marshal.SecureStringToBSTR(SecurePassword);

                var length1 = Marshal.ReadInt32(bstr1, -4);
                var length2 = Marshal.ReadInt32(bstr2, -4);

                if (length1 != length2)
                    return false;

                byte b1;
                byte b2;

                for (var x = 0; x < length1; ++x)
                {

                    b1 = Marshal.ReadByte(bstr1, x);
                    b2 = Marshal.ReadByte(bstr2, x);

                    if (b1 != b2)
                        return false;

                }

                return true;

            }
            finally
            {
                if (bstr2 != IntPtr.Zero) Marshal.ZeroFreeBSTR(bstr2);
                if (bstr1 != IntPtr.Zero) Marshal.ZeroFreeBSTR(bstr1);
            }

        }

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

            return _Login.CompareTo(LoginPassword._Login);

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

            return _Login.Equals(LoginPassword._Login);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            return _Login.GetHashCode();
        }

        #endregion

        #region ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()
        {
            return _Login.ToString();
        }

        #endregion

    }

}
