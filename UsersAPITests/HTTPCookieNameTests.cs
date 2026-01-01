/*
 * Copyright (c) 2014-2026 GraphDefined GmbH <achim.friedland@graphdefined.com>
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
using NUnit.Framework.Legacy;

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

            ClassicAssert.IsNull  (HTTPCookieName.TryParse(null));
            ClassicAssert.IsFalse (HTTPCookieName.TryParse(null).HasValue);

            ClassicAssert.IsFalse (HTTPCookieName.TryParse(null, out var httpCookieName));
            ClassicAssert.IsTrue  (httpCookieName.IsNullOrEmpty);
            ClassicAssert.AreEqual(0,  httpCookieName.Length);
            ClassicAssert.AreEqual("", httpCookieName.ToString());

        }

        [Test]
        public void TryParse_Empty()
        {

            ClassicAssert.IsNull  (HTTPCookieName.TryParse(""));
            ClassicAssert.IsFalse (HTTPCookieName.TryParse("").HasValue);

            ClassicAssert.IsFalse (HTTPCookieName.TryParse("", out var httpCookieName));
            ClassicAssert.IsTrue  (httpCookieName.IsNullOrEmpty);
            ClassicAssert.AreEqual(0,  httpCookieName.Length);
            ClassicAssert.AreEqual("", httpCookieName.ToString());

        }

        [Test]
        public void TryParse_Whitespace()
        {

            ClassicAssert.IsNull  (HTTPCookieName.TryParse("   "));
            ClassicAssert.IsFalse (HTTPCookieName.TryParse("   ").HasValue);

            ClassicAssert.IsFalse (HTTPCookieName.TryParse("   ", out var httpCookieName));
            ClassicAssert.IsTrue  (httpCookieName.IsNullOrEmpty);
            ClassicAssert.AreEqual(0,  httpCookieName.Length);
            ClassicAssert.AreEqual("", httpCookieName.ToString());

        }

        [Test]
        public void Length()
        {
            ClassicAssert.AreEqual(3, HTTPCookieName.Parse("abc").Length);
        }

        [Test]
        public void Equality()
        {
            ClassicAssert.AreEqual(HTTPCookieName.Parse("abc"), HTTPCookieName.Parse("abc"));
            ClassicAssert.IsTrue  (HTTPCookieName.Parse("abc").Equals(HTTPCookieName.Parse("abc")));
        }

        [Test]
        public void OperatorEquality()
        {
            ClassicAssert.IsTrue(HTTPCookieName.Parse("abc") == HTTPCookieName.Parse("abc"));
        }

        [Test]
        public void OperatorInequality()
        {
            ClassicAssert.IsFalse(HTTPCookieName.Parse("abc") != HTTPCookieName.Parse("abc"));
        }

        [Test]
        public void OperatorSmaller()
        {
            ClassicAssert.IsFalse(HTTPCookieName.Parse("abc") < HTTPCookieName.Parse("abc"));
            ClassicAssert.IsTrue (HTTPCookieName.Parse("abc") < HTTPCookieName.Parse("abc2"));
        }

        [Test]
        public void OperatorSmallerOrEquals()
        {
            ClassicAssert.IsTrue(HTTPCookieName.Parse("abc") <= HTTPCookieName.Parse("abc"));
            ClassicAssert.IsTrue(HTTPCookieName.Parse("abc") <= HTTPCookieName.Parse("abc2"));
        }

        [Test]
        public void OperatorBigger()
        {
            ClassicAssert.IsFalse(HTTPCookieName.Parse("abc")  > HTTPCookieName.Parse("abc"));
            ClassicAssert.IsTrue (HTTPCookieName.Parse("abc2") > HTTPCookieName.Parse("abc"));
        }

        [Test]
        public void OperatorBiggerOrEquals()
        {
            ClassicAssert.IsTrue(HTTPCookieName.Parse("abc")  >= HTTPCookieName.Parse("abc"));
            ClassicAssert.IsTrue(HTTPCookieName.Parse("abc2") >= HTTPCookieName.Parse("abc"));
        }

        [Test]
        public void HashCodeEquality()
        {
            ClassicAssert.AreEqual   (HTTPCookieName.Parse("abc").GetHashCode(), HTTPCookieName.Parse("abc"). GetHashCode());
            ClassicAssert.AreNotEqual(HTTPCookieName.Parse("abc").GetHashCode(), HTTPCookieName.Parse("abc2").GetHashCode());
        }

        [Test]
        public void DifferentCases_DictionaryKeyEquality()
        {

            var Lookup = new Dictionary<HTTPCookieName, String> {
                             { HTTPCookieName.Parse("abc01"), "DifferentCases_DictionaryKeyEquality()" }
                         };

            ClassicAssert.IsTrue(Lookup.ContainsKey(HTTPCookieName.Parse("abc01")));

        }

        [Test]
        public void Combine()
        {

            var cookie1  = HTTPCookieName.Parse("abc");
            var cookie2  = HTTPCookieName.Parse("def");
            var text     = "123";

            ClassicAssert.AreEqual(HTTPCookieName.Parse("abcdef"),  cookie1 + cookie2);
            ClassicAssert.AreEqual("abcdef",                       (cookie1 + cookie2).ToString());

            ClassicAssert.AreEqual(HTTPCookieName.Parse("abc123"),  cookie1 + text);
            ClassicAssert.AreEqual("abc123",                       (cookie1 + text).   ToString());

            ClassicAssert.AreEqual(HTTPCookieName.Parse("123def"),  text    + cookie2);
            ClassicAssert.AreEqual("123def",                       (text    + cookie2).ToString());

        }

        [Test]
        public void Test_ToString()
        {
            ClassicAssert.AreEqual("abc", HTTPCookieName.Parse("abc").ToString());
            ClassicAssert.AreEqual("aBc", HTTPCookieName.Parse("aBc").ToString());
        }

    }

}
