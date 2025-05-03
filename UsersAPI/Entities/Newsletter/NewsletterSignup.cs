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
using org.GraphDefined.Vanaheimr.Hermod.Mail;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// A newsletter signup.
    /// </summary>
    public class NewsletterSignup
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of organizations.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://opendata.social/contexts/UsersAPI/newsletterSignup");

        #endregion

        #region Properties

        [Mandatory]
        public SimpleEMailAddress  EMailAddress    { get; }

        [Mandatory]
        public Newsletter_Id       NewsletterId    { get; }

        [Mandatory]
        public DateTime            Timestamp       { get; }

        #endregion

        #region Constructor(s)

        public NewsletterSignup(SimpleEMailAddress  EMailAddress,
                                Newsletter_Id       NewsletterId,
                                DateTime            Timestamp)
        {

            this.EMailAddress  = EMailAddress;
            this.NewsletterId  = NewsletterId;
            this.Timestamp     = Timestamp;

        }

        #endregion


        #region Operator overloading

        #region Operator == (NewsletterSignupId1, NewsletterSignupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsletterSignupId1">A newsletter signup identification.</param>
        /// <param name="NewsletterSignupId2">Another newsletter signup identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (NewsletterSignup NewsletterSignupId1,
                                           NewsletterSignup NewsletterSignupId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(NewsletterSignupId1, NewsletterSignupId2))
                return true;

            // If one is null, but not both, return false.
            if (NewsletterSignupId1 is null || NewsletterSignupId2 is null)
                return false;

            return NewsletterSignupId1.Equals(NewsletterSignupId2);

        }

        #endregion

        #region Operator != (NewsletterSignupId1, NewsletterSignupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsletterSignupId1">A newsletter signup identification.</param>
        /// <param name="NewsletterSignupId2">Another newsletter signup identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (NewsletterSignup NewsletterSignupId1,
                                           NewsletterSignup NewsletterSignupId2)

            => !(NewsletterSignupId1 == NewsletterSignupId2);

        #endregion

        #region Operator <  (NewsletterSignupId1, NewsletterSignupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsletterSignupId1">A newsletter signup identification.</param>
        /// <param name="NewsletterSignupId2">Another newsletter signup identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (NewsletterSignup NewsletterSignupId1,
                                          NewsletterSignup NewsletterSignupId2)
        {

            if (NewsletterSignupId1 is null)
                throw new ArgumentNullException(nameof(NewsletterSignupId1), "The given NewsletterSignupId1 must not be null!");

            return NewsletterSignupId1.CompareTo(NewsletterSignupId2) < 0;

        }

        #endregion

        #region Operator <= (NewsletterSignupId1, NewsletterSignupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsletterSignupId1">A newsletter signup identification.</param>
        /// <param name="NewsletterSignupId2">Another newsletter signup identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (NewsletterSignup NewsletterSignupId1,
                                           NewsletterSignup NewsletterSignupId2)

            => !(NewsletterSignupId1 > NewsletterSignupId2);

        #endregion

        #region Operator >  (NewsletterSignupId1, NewsletterSignupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsletterSignupId1">A newsletter signup identification.</param>
        /// <param name="NewsletterSignupId2">Another newsletter signup identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (NewsletterSignup NewsletterSignupId1,
                                          NewsletterSignup NewsletterSignupId2)
        {

            if (NewsletterSignupId1 is null)
                throw new ArgumentNullException(nameof(NewsletterSignupId1), "The given NewsletterSignupId1 must not be null!");

            return NewsletterSignupId1.CompareTo(NewsletterSignupId2) > 0;

        }

        #endregion

        #region Operator >= (NewsletterSignupId1, NewsletterSignupId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsletterSignupId1">A newsletter signup identification.</param>
        /// <param name="NewsletterSignupId2">Another newsletter signup identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (NewsletterSignup NewsletterSignupId1,
                                           NewsletterSignup NewsletterSignupId2)

            => !(NewsletterSignupId1 < NewsletterSignupId2);

        #endregion

        #endregion

        #region IComparable<NewsletterSignup> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two newsletter signups.
        /// </summary>
        /// <param name="Object">A newsletter signup to compare with.</param>
        public Int32 CompareTo(Object Object)

            => Object is NewsletterSignup newsletterSignup
                   ? CompareTo(newsletterSignup)
                   : throw new ArgumentException("The given object is not a newsletter signup!", nameof(Object));

        #endregion

        #region CompareTo(NewsletterSignup)

        /// <summary>
        /// Compares two newsletter signups.
        /// </summary>
        /// <param name="NewsletterSignup">A newsletter signup to compare with.</param>
        public Int32 CompareTo(NewsletterSignup NewsletterSignup)
        {

            if (NewsletterSignup is null)
                throw new ArgumentNullException(nameof(NewsletterSignup), "The given newsletter signup must not be null!");

            var c = EMailAddress.CompareTo(NewsletterSignup.EMailAddress);

            if (c == 0)
                c = NewsletterId.CompareTo(NewsletterSignup.NewsletterId);

            if (c == 0)
                c = Timestamp.   CompareTo(NewsletterSignup.Timestamp);

            return c;

        }

        #endregion

        #endregion

        #region IEquatable<NewsletterSignup> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two newsletter signups for equality.
        /// </summary>
        /// <param name="Object">A newsletter signup to compare with.</param>
        public override Boolean Equals(Object? Object)

            => Object is NewsletterSignup newsletterSignup &&
                  Equals(newsletterSignup);

        #endregion

        #region Equals(NewsletterSignup)

        /// <summary>
        /// Compares two newsletter signups for equality.
        /// </summary>
        /// <param name="NewsletterSignup">A newsletter signup to compare with.</param>
        public Boolean Equals(NewsletterSignup NewsletterSignup)

            => NewsletterSignup is not null &&

               EMailAddress.Equals(NewsletterSignup.EMailAddress) &&
               NewsletterId.Equals(NewsletterSignup.NewsletterId) &&
               Timestamp.   Equals(NewsletterSignup.Timestamp);

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {

                return EMailAddress.GetHashCode() * 5 ^
                       NewsletterId.GetHashCode() * 3 ^
                       Timestamp.   GetHashCode();

            }
        }

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()

            => String.Concat(
                   EMailAddress.ToString(), ", ",
                   NewsletterId.ToString(), ", ",
                   Timestamp.   ToISO8601()
               );

        #endregion

    }

}
