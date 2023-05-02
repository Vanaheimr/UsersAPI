/*
 * Copyright (c) 2014-2023, Achim Friedland <achim.friedland@graphdefined.com>
 * This file is part of UsersAPI <https://github.com/Vanaheimr/UsersAPI>
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

using NUnit.Framework;

using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace social.OpenData.users.api.tests
{

    /// <summary>
    /// HTTP cookie name tests.
    /// </summary>
    [TestFixture]
    public class HTTPCookieNameTests
    {

        [Test]
        public void Parse_Null()
        {
            Assert.Throws<ArgumentException>(() => HTTPCookieName.Parse(null));
        }

        [Test]
        public void Parse_Empty()
        {
            Assert.Throws<ArgumentException>(() => HTTPCookieName.Parse(""));
        }

        [Test]
        public void Parse_Whitespace()
        {
            Assert.Throws<ArgumentException>(() => HTTPCookieName.Parse("   "));
        }

        [Test]
        public void TryParse_Null()
        {

            Assert.IsNull  (HTTPCookieName.TryParse(null));
            Assert.IsFalse (HTTPCookieName.TryParse(null).HasValue);

            Assert.IsFalse (HTTPCookieName.TryParse(null, out var httpCookieName));
            Assert.IsTrue  (httpCookieName.IsNullOrEmpty);
            Assert.AreEqual(0,  httpCookieName.Length);
            Assert.AreEqual("", httpCookieName.ToString());

        }

        [Test]
        public void TryParse_Empty()
        {

            Assert.IsNull  (HTTPCookieName.TryParse(""));
            Assert.IsFalse (HTTPCookieName.TryParse("").HasValue);

            Assert.IsFalse (HTTPCookieName.TryParse("", out var httpCookieName));
            Assert.IsTrue  (httpCookieName.IsNullOrEmpty);
            Assert.AreEqual(0,  httpCookieName.Length);
            Assert.AreEqual("", httpCookieName.ToString());

        }

        [Test]
        public void TryParse_Whitespace()
        {

            Assert.IsNull  (HTTPCookieName.TryParse("   "));
            Assert.IsFalse (HTTPCookieName.TryParse("   ").HasValue);

            Assert.IsFalse (HTTPCookieName.TryParse("   ", out var httpCookieName));
            Assert.IsTrue  (httpCookieName.IsNullOrEmpty);
            Assert.AreEqual(0,  httpCookieName.Length);
            Assert.AreEqual("", httpCookieName.ToString());

        }

        [Test]
        public void Length()
        {
            Assert.AreEqual(3, HTTPCookieName.Parse("abc").Length);
        }

        [Test]
        public void Equality()
        {
            Assert.AreEqual(HTTPCookieName.Parse("abc"), HTTPCookieName.Parse("abc"));
            Assert.IsTrue  (HTTPCookieName.Parse("abc").Equals(HTTPCookieName.Parse("abc")));
        }

        [Test]
        public void OperatorEquality()
        {
            Assert.IsTrue(HTTPCookieName.Parse("abc") == HTTPCookieName.Parse("abc"));
        }

        [Test]
        public void OperatorInequality()
        {
            Assert.IsFalse(HTTPCookieName.Parse("abc") != HTTPCookieName.Parse("abc"));
        }

        [Test]
        public void OperatorSmaller()
        {
            Assert.IsFalse(HTTPCookieName.Parse("abc") < HTTPCookieName.Parse("abc"));
            Assert.IsTrue (HTTPCookieName.Parse("abc") < HTTPCookieName.Parse("abc2"));
        }

        [Test]
        public void OperatorSmallerOrEquals()
        {
            Assert.IsTrue(HTTPCookieName.Parse("abc") <= HTTPCookieName.Parse("abc"));
            Assert.IsTrue(HTTPCookieName.Parse("abc") <= HTTPCookieName.Parse("abc2"));
        }

        [Test]
        public void OperatorBigger()
        {
            Assert.IsFalse(HTTPCookieName.Parse("abc")  > HTTPCookieName.Parse("abc"));
            Assert.IsTrue (HTTPCookieName.Parse("abc2") > HTTPCookieName.Parse("abc"));
        }

        [Test]
        public void OperatorBiggerOrEquals()
        {
            Assert.IsTrue(HTTPCookieName.Parse("abc")  >= HTTPCookieName.Parse("abc"));
            Assert.IsTrue(HTTPCookieName.Parse("abc2") >= HTTPCookieName.Parse("abc"));
        }

        [Test]
        public void HashCodeEquality()
        {
            Assert.AreEqual   (HTTPCookieName.Parse("abc").GetHashCode(), HTTPCookieName.Parse("abc"). GetHashCode());
            Assert.AreNotEqual(HTTPCookieName.Parse("abc").GetHashCode(), HTTPCookieName.Parse("abc2").GetHashCode());
        }

        [Test]
        public void DifferentCases_DictionaryKeyEquality()
        {

            var Lookup = new Dictionary<HTTPCookieName, String> {
                             { HTTPCookieName.Parse("abc01"), "DifferentCases_DictionaryKeyEquality()" }
                         };

            Assert.IsTrue(Lookup.ContainsKey(HTTPCookieName.Parse("abc01")));

        }

        [Test]
        public void Combine()
        {

            var cookie1  = HTTPCookieName.Parse("abc");
            var cookie2  = HTTPCookieName.Parse("def");
            var text     = "123";

            Assert.AreEqual(HTTPCookieName.Parse("abcdef"),  cookie1 + cookie2);
            Assert.AreEqual("abcdef",                       (cookie1 + cookie2).ToString());

            Assert.AreEqual(HTTPCookieName.Parse("abc123"),  cookie1 + text);
            Assert.AreEqual("abc123",                       (cookie1 + text).   ToString());

            Assert.AreEqual(HTTPCookieName.Parse("123def"),  text    + cookie2);
            Assert.AreEqual("123def",                       (text    + cookie2).ToString());

        }

        [Test]
        public void Test_ToString()
        {
            Assert.AreEqual("abc", HTTPCookieName.Parse("abc").ToString());
            Assert.AreEqual("aBc", HTTPCookieName.Parse("aBc").ToString());
        }

    }

}
