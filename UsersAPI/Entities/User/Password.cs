﻿/*
 * Copyright (c) 2014-2021, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using System.Text;
using System.Security;
using System.Security.Cryptography;

using org.GraphDefined.Vanaheimr.Illias;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// A password.
    /// </summary>
    public readonly struct Password : IEquatable<Password>
    {

        #region Data

        /// <summary>
        /// Private non-cryptographic random number generator.
        /// </summary>
        private static readonly Random _random = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// The internal identification.
        /// </summary>
        private readonly SecureString InternalPassword;

        #endregion

        #region Properties

        /// <summary>
        /// The salt of the password.
        /// </summary>
        /// <remarks>It is a SecureString just because we can ;)</remarks>
        public SecureString  Salt   { get; }

        /// <summary>
        /// Return the internal password as an unsecured string,
        /// e.g. in order to store it on disc.
        /// </summary>
        internal String UnsecureString
            => InternalPassword.UnsecureString();

        /// <summary>
        /// The length of the password.
        /// </summary>
        public UInt16 Length
            => (UInt16) InternalPassword.UnsecureString().Length;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new password based on the given string.
        /// </summary>
        /// <param name="Salt">The salt of the password.</param>
        /// <param name="Password">The string representation of the password.</param>
        private Password(SecureString  Salt,
                         SecureString  Password)
        {

            this.Salt              = Salt;
            this.InternalPassword  = Password;

        }

        #endregion


        #region (static) Random  (PasswordLength = 16, LengthOfSalt = 16)

        /// <summary>
        /// Create a new random password.
        /// </summary>
        /// <param name="PasswordLength">The expected length of the password.</param>
        /// <param name="LengthOfSalt">The optional length of the random salt.</param>
        public static Password Random(Byte PasswordLength  = 16,
                                      Byte LengthOfSalt    = 16)

            => Parse(_random.RandomString(PasswordLength),
                     LengthOfSalt);

        #endregion

        #region (static) Parse   (Text, LengthOfSalt = 16)

        /// <summary>
        /// Parse the given string as a password.
        /// </summary>
        /// <param name="Text">A text representation of a password.</param>
        /// <param name="LengthOfSalt">The optional length of the random salt.</param>
        public static Password Parse(String  Text,
                                     Byte    LengthOfSalt = 16)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a password must not be null or empty!");

            #endregion

            // Salt...
            var Salt            = _random.RandomString(LengthOfSalt);
            var SecureSalt      = new SecureString();

            foreach (var character in Salt)
                SecureSalt.AppendChar(character);

            // Password...
            var SecurePassword  = new SecureString();
            var HashAlgorithm   = new SHA256Managed();
            var HashedPassword  = HashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(Salt + ":" + Text));

            foreach (var character in HashedPassword.ToHexString(ToLower: true))
                SecurePassword.AppendChar(character);

            return new Password(SecureSalt,
                                SecurePassword);

        }

        #endregion

        #region (static) TryParse(Text, LengthOfSalt = 16)

        /// <summary>
        /// Try to parse the given string as a password.
        /// </summary>
        /// <param name="Text">A text representation of a password.</param>
        /// <param name="LengthOfSalt">The optional length of the random salt.</param>
        public static Password? TryParse(String  Text,
                                         Byte    LengthOfSalt = 16)
        {

            if (TryParse(Text,
                         out Password _Password,
                         LengthOfSalt))
            {
                return _Password;
            }

            return new Password?();

        }

        #endregion

        #region (static) TryParse(Text, out Password, LengthOfSalt = 16)

        /// <summary>
        /// Try to parse the given string as a password.
        /// </summary>
        /// <param name="Text">A text representation of a password.</param>
        /// <param name="Password">The parsed password.</param>
        public static Boolean TryParse(String        Text,
                                       out Password  Password)

            => TryParse(Text,
                        out Password,
                        16);

        /// <summary>
        /// Try to parse the given string as a password.
        /// </summary>
        /// <param name="Text">A text representation of a password.</param>
        /// <param name="Password">The parsed password.</param>
        /// <param name="LengthOfSalt">The optional length of the random salt.</param>
        public static Boolean TryParse(String        Text,
                                       out Password  Password,
                                       Byte          LengthOfSalt)
        {

            #region Initial checks

            if (Text != null)
                Text = Text.Trim();

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given text representation of a password must not be null or empty!");

            #endregion

            try
            {

                Password = Parse(Text,
                                 LengthOfSalt);

                return true;

            }
            catch (Exception)
            {
                Password = default(Password);
                return false;
            }

        }

        #endregion


        #region (static) ParseHash   (Salt, HashedPassword)

        /// <summary>
        /// Parse the given strings as a salted and SHA256 hashed password.
        /// </summary>
        /// <param name="Salt">The salt of the password.</param>
        /// <param name="HashedPassword">A text representation of a SHA256 hashed password.</param>
        public static Password ParseHash(String  Salt,
                                         String  HashedPassword)
        {

            #region Initial checks

            if (HashedPassword != null)
                HashedPassword = HashedPassword.Trim().ToLower();

            if (HashedPassword.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(HashedPassword), "The given SHA256 hashed representation of a password must not be null or empty!");

            #endregion

            // Salt...
            var SecureSalt      = new SecureString();

            foreach (var character in Salt)
                SecureSalt.AppendChar(character);


            // Password...
            var SecurePassword  = new SecureString();

            foreach (var character in HashedPassword)
                SecurePassword.AppendChar(character);

            return new Password(SecureSalt,
                                SecurePassword);

        }

        #endregion

        #region (static) TryParseHash(Salt, HashedPassword)

        /// <summary>
        /// Try to parse the given strings as a salted and SHA256 hashed password.
        /// </summary>
        /// <param name="Salt">The salt of the SHA256 hashed password.</param>
        /// <param name="HashedPassword">A text representation of a SHA256 hashed password.</param>
        public static Password? TryParseHash(String  Salt,
                                             String  HashedPassword)
        {

            if (TryParseHash(Salt,
                             HashedPassword,
                             out Password _Password))
            {
                return _Password;
            }

            return new Password?();

        }

        #endregion

        #region (static) TryParseHash(Salt, HashedPassword, out Password)

        /// <summary>
        /// Try to parse the given strings as a salted and SHA256 hashed password.
        /// </summary>
        /// <param name="Salt">The salt of the SHA256 hashed password.</param>
        /// <param name="HashedPassword">A text representation of a SHA256 hashed password.</param>
        /// <param name="Password">The parsed password.</param>
        public static Boolean TryParseHash(String        Salt,
                                           String        HashedPassword,
                                           out Password  Password)
        {

            #region Initial checks

            if (HashedPassword != null)
                HashedPassword = HashedPassword.Trim().ToLower();

            if (HashedPassword.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(HashedPassword), "The given SHA256 hashed representation of a password must not be null or empty!");

            #endregion

            try
            {

                Password = ParseHash(Salt,
                                     HashedPassword);

                return true;

            }
            catch (Exception)
            {
                Password = default(Password);
                return false;
            }

        }

        #endregion


        #region Clone

        /// <summary>
        /// Clone a password.
        /// </summary>
        public Password Clone

            => new Password(Salt,
                            InternalPassword.Copy());

        #endregion


        #region Verify(PlainPassword)

        /// <summary>
        /// Verify the given password.
        /// </summary>
        /// <param name="PlainPassword"></param>
        public Boolean Verify(String PlainPassword)
        {

            var CheckPassword   = new SecureString();
            var HashAlgorithm   = new SHA256Managed();
            var HashedPassword  = HashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(Salt.UnsecureString() + ":" + PlainPassword));

            return InternalPassword.UnsecureString().Equals(HashedPassword.ToHexString(ToLower: true));

        }

        #endregion


        #region Operator overloading

        #region Operator == (Password1, Password2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Password1">A password.</param>
        /// <param name="Password2">Another password.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Password Password1, Password Password2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(Password1, Password2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) Password1 == null) || ((Object) Password2 == null))
                return false;

            return Password1.Equals(Password2);

        }

        #endregion

        #region Operator != (Password1, Password2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Password1">A password.</param>
        /// <param name="Password2">Another password.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Password Password1, Password Password2)
            => !(Password1 == Password2);

        #endregion

        #endregion

        #region IEquatable<Password> Members

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

            if (!(Object is Password))
                return false;

            return Equals((Password) Object);

        }

        #endregion

        #region Equals(Password)

        /// <summary>
        /// Compares two passwords for equality.
        /// </summary>
        /// <param name="Password">An password to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(Password Password)
        {

            if ((Object) Password == null)
                return false;

            return Salt.            UnsecureString().Equals(Password.Salt.            UnsecureString()) &&
                   InternalPassword.UnsecureString().Equals(Password.InternalPassword.UnsecureString());

            //var bstr1 = IntPtr.Zero;
            //var bstr2 = IntPtr.Zero;

            //try
            //{

            //    bstr1 = Marshal.SecureStringToBSTR(SecurePassword);
            //    bstr2 = Marshal.SecureStringToBSTR(Password.SecurePassword);

            //    var length1 = Marshal.ReadInt32(bstr1, -4);
            //    var length2 = Marshal.ReadInt32(bstr2, -4);

            //    if (length1 != length2)
            //        return false;

            //    byte b1;
            //    byte b2;

            //    for (var x = 0; x < length1; ++x)
            //    {

            //        b1 = Marshal.ReadByte(bstr1, x);
            //        b2 = Marshal.ReadByte(bstr2, x);

            //        if (b1 != b2)
            //            return false;

            //    }

            //    return true;

            //}
            //finally
            //{
            //    if (bstr2 != IntPtr.Zero) Marshal.ZeroFreeBSTR(bstr2);
            //    if (bstr1 != IntPtr.Zero) Marshal.ZeroFreeBSTR(bstr1);
            //}

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
        public override Int32 GetHashCode()

            => Salt.            GetHashCode() ^
               InternalPassword.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => nameof(Password);

        #endregion

    }

}
