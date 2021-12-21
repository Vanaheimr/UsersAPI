/*
 * Copyright (c) 2014-2021, Achim Friedland <achim.friedland@graphdefined.com>
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

using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;

using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Styx.Arrows;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;

#endregion

namespace social.OpenData.UsersAPI
{

//    public delegate Boolean NewsProviderDelegate(News_Id NewsId, out News News);

    public delegate JObject NewsPostingToJSONDelegate(NewsPosting  NewsPosting,
                                                      Boolean      Embedded            = false,
                                                      InfoStatus   ExpandTags          = InfoStatus.ShowIdOnly,
                                                      InfoStatus   ExpandAuthorId      = InfoStatus.ShowIdOnly,
                                                      Boolean      IncludeCryptoHash   = true);


    /// <summary>
    /// Extension methods for news postings.
    /// </summary>
    public static class NewsPostingExtensions
    {

        #region ToJSON(this News, Skip = null, Take = null, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of news postings.
        /// </summary>
        /// <param name="NewsPosting">An enumeration of news postings.</param>
        /// <param name="Skip">The optional number of news postings to skip.</param>
        /// <param name="Take">The optional number of news postings to return.</param>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        public static JArray ToJSON(this IEnumerable<NewsPosting>  NewsPosting,
                                    UInt64?                        Skip                = null,
                                    UInt64?                        Take                = null,
                                    Boolean                        Embedded            = false,
                                    InfoStatus                     ExpandTags          = InfoStatus.ShowIdOnly,
                                    InfoStatus                     ExpandAuthorId      = InfoStatus.ShowIdOnly,
                                    NewsPostingToJSONDelegate      NewsToJSON          = null,
                                    Boolean                        IncludeCryptoHash   = true)


            => NewsPosting?.Any() != true

                   ? new JArray()

                   : new JArray(NewsPosting.
                                    Where            (dataSet =>  dataSet != null).
                                    //OrderByDescending(dataSet => dataSet.PublicationDate).
                                    SkipTakeFilter   (Skip, Take).
                                    SafeSelect       (news    => NewsToJSON != null
                                                                     ? NewsToJSON (news,
                                                                                   Embedded,
                                                                                   ExpandTags,
                                                                                   ExpandAuthorId,
                                                                                   IncludeCryptoHash)

                                                                     : news.ToJSON(Embedded,
                                                                                   ExpandTags,
                                                                                   ExpandAuthorId,
                                                                                   IncludeCryptoHash)));

        #endregion

    }

    /// <summary>
    /// A news posting.
    /// </summary>
    public class NewsPosting : AEntity<NewsPosting_Id,
                                       NewsPosting>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of news.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://opendata.social/contexts/CardiCloudAPI/news");

        #endregion

        #region Properties

        #region API

        private Object _API;

        /// <summary>
        /// The API of this News.
        /// </summary>
        internal Object API
        {

            get
            {
                return _API;
            }

            set
            {

                if (_API != null)
                    throw new ArgumentException("Illegal attempt to change the API of this news posting!");

                if (value == null)
                    throw new ArgumentException("Illegal attempt to delete the API reference of this news posting!");

                _API = value;

            }

        }

        #endregion

        /// <summary>
        /// The (multi-language) headline of this news posting.
        /// </summary>
        [Mandatory]
        public I18NString                         Headline              { get; }

        /// <summary>
        /// The (multi-language) text of this news posting.
        /// </summary>
        [Mandatory]
        public I18NString                         Text                  { get; }

        /// <summary>
        /// The author of the news posting.
        /// </summary>
        [Mandatory]
        public User                               Author                { get; }

        /// <summary>
        /// The publication timestamp of this news posting.
        /// </summary>
        [Mandatory]
        public DateTime                           PublicationDate       { get; }

        /// <summary>
        /// An enumeration of multi-language tags and their relevance.
        /// </summary>
        [Optional]
        public IEnumerable<TagRelevance>          Tags                  { get; }

        /// <summary>
        /// Whether the news posting is currently hidden.
        /// </summary>
        [Optional]
        public Boolean                            IsHidden              { get; }

        /// <summary>
        /// All signatures of this news posting.
        /// </summary>
        public IEnumerable<Signature>             Signatures            { get; }

        #endregion

        #region Events

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new news.
        /// </summary>
        /// <param name="Text">The (multi-language) text of this news posting.</param>
        /// <param name="PublicationDate">The publication timestamp of this news posting.</param>
        /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
        /// <param name="IsHidden">Whether the news posting is currently hidden.</param>
        /// <param name="Signatures">All signatures of this news.</param>
        public NewsPosting(I18NString                  Headline,
                           I18NString                  Text,
                           User                        Author,
                           DateTime?                   PublicationDate   = null,
                           IEnumerable<TagRelevance>   Tags              = null,
                           Boolean                     IsHidden          = false,

                           IEnumerable<Signature>      Signatures        = null,

                           JObject                     CustomData        = default,
                           String                      DataSource        = default,
                           DateTime?                   LastChange        = default)

            : this(NewsPosting_Id.Random(),
                   Headline,
                   Text,
                   Author,
                   PublicationDate,
                   Tags,
                   IsHidden,
                   Signatures,

                   CustomData,
                   DataSource,
                   LastChange)

        { }


        /// <summary>
        /// Create a new Open Data news.
        /// </summary>
        /// <param name="Id">The unique identification of this news.</param>
        /// <param name="Text">The (multi-language) text of this news.</param>
        /// <param name="PublicationDate">The publication timestamp of this news posting.</param>
        /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
        /// <param name="IsHidden">Whether the news posting is currently hidden.</param>
        /// <param name="Signatures">All signatures of this news.</param>
        public NewsPosting(NewsPosting_Id              Id,
                           I18NString                  Headline,
                           I18NString                  Text,
                           User                        Author,
                           DateTime?                   PublicationDate   = null,
                           IEnumerable<TagRelevance>   Tags              = null,
                           Boolean                     IsHidden          = false,

                           IEnumerable<Signature>      Signatures        = null,

                           JObject                     CustomData        = default,
                           String                      DataSource        = default,
                           DateTime?                   LastChange        = default)

            : base(Id,
                   DefaultJSONLDContext,
                   CustomData,
                   DataSource,
                   LastChange)

        {

            if (Headline.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Headline),  "The given (multi-language) headline must not be null or empty!");

            if (Text.    IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text),      "The given (multi-language) text must not be null or empty!");

            this.Headline         = Headline;
            this.Text             = Text;
            this.Author           = Author          ?? throw new ArgumentNullException(nameof(Author), "The given author must not be null!");
            this.PublicationDate  = PublicationDate ?? Timestamp.Now;
            this.Tags             = Tags            ?? new TagRelevance[0];
            this.IsHidden         = IsHidden;

            this.Signatures       = Signatures      ?? new Signature[0];

            CalcHash();

        }

        #endregion


        #region ToJSON(Embedded = false, IncludeCryptoHash = false)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        /// <param name="IncludeCryptoHash">Include the crypto hash value of this object.</param>
        public override JObject ToJSON(Boolean Embedded           = false,
                                       Boolean IncludeCryptoHash  = false)

            => ToJSON(Embedded:            false,
                      ExpandTags:          InfoStatus.ShowIdOnly,
                      IncludeCryptoHash:   true);


        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        /// <param name="IncludeCryptoHash">Include the crypto hash value of this object.</param>
        public JObject ToJSON(Boolean     Embedded            = false,
                              InfoStatus  ExpandTags          = InfoStatus.ShowIdOnly,
                              InfoStatus  ExpandAuthorId      = InfoStatus.ShowIdOnly,
                              Boolean     IncludeCryptoHash   = false)

            => JSONObject.Create(

                   new JProperty("@id",                Id.             ToString()),

                   !Embedded
                       ? new JProperty("@context",     JSONLDContext.  ToString())
                       : null,

                   new JProperty("headline",           Headline.       ToJSON()),
                   new JProperty("text",               Text.           ToJSON()),

                   new JProperty("author",             JSONObject.Create(
                                                           new JProperty("@id",  Author.Id.ToString()),
                                                           new JProperty("name", Author.Name)
                                                       )),

                   new JProperty("publicationDate",    PublicationDate.ToIso8601()),

                   Tags.Any()
                       ? new JProperty("tags",         Tags.SafeSelect(tag => tag.ToJSON(ExpandTags)))
                       : null,

                   Signatures.Any()
                       ? new JProperty("signatures",   new JArray(Signatures.SafeSelect(signature => signature.ToJSON(Embedded: true))))
                       : null

               );

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out News, out ErrorResponse)

        public static Boolean TryParseJSON(JObject               JSONObject,
                                           UserProviderDelegate  UserProvider,
                                           out NewsPosting       NewsPosting,
                                           out String            ErrorResponse,
                                           NewsPosting_Id?       NewsIdURL  = null)
        {

            try
            {

                NewsPosting = null;

                #region Parse NewsId            [optional]

                // Verify that a given News identification
                //   is at least valid.
                if (JSONObject.ParseOptionalStruct("@id",
                                                   "News identification",
                                                   NewsPosting_Id.TryParse,
                                                   out NewsPosting_Id? NewsIdBody,
                                                   out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                if (!NewsIdURL.HasValue && !NewsIdBody.HasValue)
                {
                    ErrorResponse = "The News identification is missing!";
                    return false;
                }

                if (NewsIdURL.HasValue && NewsIdBody.HasValue && NewsIdURL.Value != NewsIdBody.Value)
                {
                    ErrorResponse = "The optional News identification given within the JSON body does not match the one given in the URI!";
                    return false;
                }

                #endregion

                #region Parse Context           [mandatory]

                if (!JSONObject.ParseMandatory("@context",
                                               "JSON-LinkedData context information",
                                               JSONLDContext.TryParse,
                                               out JSONLDContext Context,
                                               out ErrorResponse))
                {
                    ErrorResponse = @"The JSON-LD ""@context"" information is missing!";
                    return false;
                }

                if (Context != DefaultJSONLDContext)
                {
                    ErrorResponse = @"The given JSON-LD ""@context"" information '" + Context + "' is not supported!";
                    return false;
                }

                #endregion

                #region Parse Headline          [mandatory]

                if (!JSONObject.ParseMandatory("headline",
                                               "news headline",
                                               out I18NString Headline,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Text              [mandatory]

                if (!JSONObject.ParseMandatory("text",
                                               "news text",
                                               out I18NString Text,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Author            [mandatory]

                User Author = null;

                if (JSONObject["author"] is JObject authorJSON &&
                    authorJSON.ParseMandatory("@id",
                                              "author identification",
                                              User_Id.TryParse,
                                              out User_Id AuthorId,
                                              out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                    if (!UserProvider(AuthorId, out Author))
                    {
                        ErrorResponse = "The given author '" + AuthorId + "' is unknown!";
                        return false;
                    }

                }

                else
                    return false;

                #endregion

                #region Parse PublicationDate   [mandatory]

                if (!JSONObject.ParseMandatory("publicationDate",
                                               "publication date",
                                               out DateTime PublicationDate,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                var Tags             = new TagRelevance[0];

                #region Parse IsHidden          [optional]

                if (JSONObject.ParseOptional("isHidden",
                                             "is hidden",
                                             out Boolean? IsHidden,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                var Signatures       = new Signature[0];

                var CustomData = JSONObject["CustomData"] as JObject;

                #region Get   DataSource        [optional]

                var DataSource = JSONObject.GetOptional("dataSource");

                #endregion

                #region Get   LastChange        [optional]

                if (JSONObject.ParseOptional("lastChange",
                                             "last change",
                                             out DateTime? LastChange,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse CryptoHash        [optional]

                var CryptoHash    = JSONObject.GetOptional("cryptoHash");

                #endregion


                NewsPosting = new NewsPosting(NewsIdBody ?? NewsIdURL.Value,
                                              Headline,
                                              Text,
                                              Author,
                                              PublicationDate,
                                              Tags,
                                              IsHidden ?? false,
                                              Signatures,

                                              CustomData,
                                              DataSource,
                                              LastChange);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse  = e.Message;
                NewsPosting    = null;
                return false;
            }

        }

        #endregion


        #region CopyAllLinkedDataFrom(OldNews)

        public override void CopyAllLinkedDataFrom(NewsPosting OldNews)
        {

        }

        #endregion


        #region Operator overloading

        #region Operator == (NewsId1, NewsId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsId1">A News identification.</param>
        /// <param name="NewsId2">Another News identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (NewsPosting NewsId1, NewsPosting NewsId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(NewsId1, NewsId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) NewsId1 == null) || ((Object) NewsId2 == null))
                return false;

            return NewsId1.Equals(NewsId2);

        }

        #endregion

        #region Operator != (NewsId1, NewsId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsId1">A News identification.</param>
        /// <param name="NewsId2">Another News identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (NewsPosting NewsId1, NewsPosting NewsId2)
            => !(NewsId1 == NewsId2);

        #endregion

        #region Operator <  (NewsId1, NewsId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsId1">A News identification.</param>
        /// <param name="NewsId2">Another News identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (NewsPosting NewsId1, NewsPosting NewsId2)
        {

            if ((Object) NewsId1 == null)
                throw new ArgumentNullException(nameof(NewsId1), "The given NewsId1 must not be null!");

            return NewsId1.CompareTo(NewsId2) < 0;

        }

        #endregion

        #region Operator <= (NewsId1, NewsId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsId1">A News identification.</param>
        /// <param name="NewsId2">Another News identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (NewsPosting NewsId1, NewsPosting NewsId2)
            => !(NewsId1 > NewsId2);

        #endregion

        #region Operator >  (NewsId1, NewsId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsId1">A News identification.</param>
        /// <param name="NewsId2">Another News identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (NewsPosting NewsId1, NewsPosting NewsId2)
        {

            if ((Object) NewsId1 == null)
                throw new ArgumentNullException(nameof(NewsId1), "The given NewsId1 must not be null!");

            return NewsId1.CompareTo(NewsId2) > 0;

        }

        #endregion

        #region Operator >= (NewsId1, NewsId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsId1">A News identification.</param>
        /// <param name="NewsId2">Another News identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (NewsPosting NewsId1, NewsPosting NewsId2)
            => !(NewsId1 < NewsId2);

        #endregion

        #endregion

        #region IComparable<News> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public override Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            var News = Object as NewsPosting;
            if ((Object) News == null)
                throw new ArgumentException("The given object is not an News!");

            return CompareTo(News);

        }

        #endregion

        #region CompareTo(News)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="News">An News object to compare with.</param>
        public override Int32 CompareTo(NewsPosting News)
        {

            if ((Object) News == null)
                throw new ArgumentNullException("The given News must not be null!");

            return Id.CompareTo(News.Id);

        }

        #endregion

        #endregion

        #region IEquatable<News> Members

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

            var News = Object as NewsPosting;
            if ((Object) News == null)
                return false;

            return Equals(News);

        }

        #endregion

        #region Equals(News)

        /// <summary>
        /// Compares two Newss for equality.
        /// </summary>
        /// <param name="News">An News to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(NewsPosting News)
        {

            if ((Object) News == null)
                return false;

            return Id.Equals(News.Id);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
            => Id.GetHashCode();

        #endregion

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
        /// </summary>
        public override String ToString()
            => Id.ToString();

        #endregion


        #region ToBuilder(NewNewsPostingId = null)

        /// <summary>
        /// Return a builder for this news posting.
        /// </summary>
        /// <param name="NewNewsPostingId">An optional new news posting identification.</param>
        public Builder ToBuilder(NewsPosting_Id? NewNewsPostingId = null)

            => new Builder(NewNewsPostingId ?? Id,
                           Headline,
                           Text,
                           Author,
                           PublicationDate,
                           Tags,
                           IsHidden,
                           Signatures,

                           CustomData,
                           DataSource,
                           LastChange);

        #endregion

        #region (class) Builder

        /// <summary>
        /// A news posting builder.
        /// </summary>
        public new class Builder : AEntity<NewsPosting_Id,
                                          NewsPosting>.Builder
        {

            #region Properties

            /// <summary>
            /// The (multi-language) headline of this news posting.
            /// </summary>
            [Mandatory]
            public I18NString                         Headline              { get; set; }

            /// <summary>
            /// The (multi-language) text of this news posting.
            /// </summary>
            [Mandatory]
            public I18NString                         Text                  { get; set; }

            /// <summary>
            /// The author of this news posting.
            /// </summary>
            [Mandatory]
            public User                               Author                { get; set; }

            /// <summary>
            /// The publication timestamp of this news posting.
            /// </summary>
            [Mandatory]
            public DateTime?                          PublicationDate       { get; set; }

            /// <summary>
            /// An enumeration of multi-language tags and their relevance.
            /// </summary>
            [Optional]
            public List<TagRelevance>                 Tags                  { get; set; }

            /// <summary>
            /// Whether the news posting is currently hidden.
            /// </summary>
            [Optional]
            public Boolean                            IsHidden              { get; set; }

            /// <summary>
            /// All signatures of this news.
            /// </summary>
            public List<Signature>                    Signatures            { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new News builder.
            /// </summary>
            /// <param name="Id">The unique identification of this news.</param>
            /// <param name="Headline">The (multi-language) headline of this news posting.</param>
            /// <param name="Text">The (multi-language) text of this news posting.</param>
            /// <param name="Author">The author of this news posting.</param>
            /// <param name="PublicationDate">The publication timestamp of this news posting.</param>
            /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
            /// <param name="IsHidden">Whether the news posting is currently hidden.</param>
            /// <param name="Signatures">All signatures of this news.</param>
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            public Builder(NewsPosting_Id?             Id                = null,
                           I18NString                  Headline          = null,
                           I18NString                  Text              = null,
                           User                        Author            = null,
                           DateTime?                   PublicationDate   = null,
                           IEnumerable<TagRelevance>   Tags              = null,
                           Boolean                     IsHidden          = false,
                           IEnumerable<Signature>      Signatures        = null,

                           JObject                     CustomData        = default,
                           String                      DataSource        = default,
                           DateTime?                   LastChange        = default)

                : base(Id ?? NewsPosting_Id.Random(),
                       DefaultJSONLDContext,
                       CustomData,
                       DataSource,
                       LastChange)

            {

                this.Headline         = Headline;
                this.Text             = Text;
                this.Author           = Author;
                this.PublicationDate  = PublicationDate;
                this.Tags             = Tags != null
                                            ? new List<TagRelevance>(Tags)
                                            : new List<TagRelevance>();
                this.IsHidden         = IsHidden;
                this.Signatures       = Signatures != null
                                            ? new List<Signature>(Signatures)
                                            : new List<Signature>();

            }

            #endregion


            public NewsPosting Sign(ICipherParameters PrivateKey)
            {

                var news        = new NewsPosting(Id,
                                                  Headline,
                                                  Text,
                                                  Author,
                                                  PublicationDate,
                                                  Tags,
                                                  IsHidden,
                                                  Signatures);

                var ctext       = news.ToJSON(Embedded:           false,
                                              ExpandTags:         InfoStatus.ShowIdOnly,
                                              IncludeCryptoHash:  false).ToString(Newtonsoft.Json.Formatting.None);

                var BlockSize   = 32;

                var SHA256      = new SHA256Managed();
                var SHA256Hash  = SHA256.ComputeHash(ctext.ToUTF8Bytes());
                var signer      = SignerUtilities.GetSigner("NONEwithECDSA");
                signer.Init(true, PrivateKey);
                signer.BlockUpdate(SHA256Hash, 0, BlockSize);

                var signature   = signer.GenerateSignature().ToHexString();
                var signatures  = new List<Signature>(Signatures);
                signatures.Add(new Signature("json", "secp256k1", "DER+HEX", signature));

                return new NewsPosting(Id,
                                       Headline,
                                       Text,
                                       Author,
                                       PublicationDate,
                                       Tags,
                                       IsHidden,
                                       signatures);

            }


            public override void CopyAllLinkedDataFrom(NewsPosting OldEnity)
            {
            }

            public override int CompareTo(object obj)
            {
                return 0;
            }

            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the News.
            /// </summary>
            /// <param name="Builder">A News builder.</param>
            public static implicit operator NewsPosting(Builder Builder)

                => Builder?.ToImmutable;


            /// <summary>
            /// Return an immutable version of the News.
            /// </summary>
            public NewsPosting ToImmutable

                => new NewsPosting(Id,
                                   Headline,
                                   Text,
                                   Author,
                                   PublicationDate,
                                   Tags,
                                   IsHidden,
                                   Signatures,

                                   CustomData,
                                   DataSource,
                                   LastChange);

            #endregion

        }

        #endregion

    }

}
