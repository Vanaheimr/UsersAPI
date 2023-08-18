/*
 * Copyright (c) 2014-2023 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using System.Security.Cryptography;

using Newtonsoft.Json.Linq;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Styx.Arrows;

#endregion

namespace social.OpenData.UsersAPI
{

//    public delegate Boolean NewsProviderDelegate(News_Id NewsId, out News News);

    public delegate JObject NewsPostingToJSONDelegate(NewsPosting  NewsPosting,
                                                      Boolean      Embedded         = false,
                                                      InfoStatus   ExpandTags       = InfoStatus.ShowIdOnly,
                                                      InfoStatus   ExpandAuthorId   = InfoStatus.ShowIdOnly);


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
        /// <param name="Embedded">Whether this data structure is embedded into another data structure.</param>
        public static JArray ToJSON(this IEnumerable<NewsPosting>  NewsPosting,
                                    UInt64?                        Skip                = null,
                                    UInt64?                        Take                = null,
                                    Boolean                        Embedded            = false,
                                    InfoStatus                     ExpandTags          = InfoStatus.ShowIdOnly,
                                    InfoStatus                     ExpandAuthorId      = InfoStatus.ShowIdOnly,
                                    NewsPostingToJSONDelegate?     NewsPostingToJSON   = null)


            => NewsPosting?.Any() != true

                   ? new JArray()

                   : new JArray(NewsPosting.
                                    Where            (newsPosting =>  newsPosting is not null).
                                    //OrderByDescending(dataSet => dataSet.PublicationDate).
                                    SkipTakeFilter   (Skip, Take).
                                    SafeSelect       (newsPosting => NewsPostingToJSON is not null
                                                                         ? NewsPostingToJSON (newsPosting,
                                                                                              Embedded,
                                                                                              ExpandTags,
                                                                                              ExpandAuthorId)

                                                                         : newsPosting.ToJSON(Embedded,
                                                                                              ExpandTags,
                                                                                              ExpandAuthorId)));

        #endregion

    }

    /// <summary>
    /// A news posting.
    /// </summary>
    public class NewsPosting : AEntity<NewsPosting_Id, NewsPosting>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of news.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://opendata.social/contexts/CardiCloudAPI/news");

        #endregion

        #region Properties

        #region API

        private Object? _API;

        /// <summary>
        /// The API of this News.
        /// </summary>
        internal Object? API
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
        /// The optional author(s) of this news posting.
        /// </summary>
        [Optional]
        public IEnumerable<User>                  Authors               { get; }

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

        #endregion

        #region Events

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new news posting.
        /// </summary>
        /// <param name="Id">The unique identification of this news.</param>
        /// <param name="Text">The (multi-language) text of this news.</param>
        /// <param name="PublicationDate">The publication timestamp of this news posting.</param>
        /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
        /// <param name="PrivacyLevel">Whether the news posting will be shown in news posting listings, or not.</param>
        /// <param name="IsHidden">Whether the news posting is currently hidden.</param>
        /// 
        /// <param name="Signatures">All signatures of this news.</param>
        public NewsPosting(I18NString                  Headline,
                           I18NString                  Text,
                           NewsPosting_Id?             Id                = null,
                           IEnumerable<User>?          Authors           = null,
                           DateTime?                   PublicationDate   = null,
                           DateTime?                   LastChangeDate    = null,
                           IEnumerable<TagRelevance>?  Tags              = null,
                           PrivacyLevel?               PrivacyLevel      = null,
                           Boolean                     IsHidden          = false,

                           IEnumerable<Signature>?     Signatures        = null,
                           JObject?                    CustomData        = default,
                           String?                     DataSource        = default)

            : base(Id ?? NewsPosting_Id.Random(),
                   DefaultJSONLDContext,
                   null,
                   null,
                   Signatures,
                   CustomData,
                   null,
                   LastChangeDate,
                   DataSource)

        {

            if (Headline.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Headline),  "The given (multi-language) headline must not be null or empty!");

            if (Text.    IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text),      "The given (multi-language) text must not be null or empty!");

            this.Headline         = Headline;
            this.Text             = Text;
            this.Authors          = Authors         ?? Array.Empty<User>();
            this.PublicationDate  = PublicationDate ?? Timestamp.Now;
            this.Tags             = Tags            ?? Array.Empty<TagRelevance>();
            this.IsHidden         = IsHidden;

        }

        #endregion


        #region ToJSON(Embedded = false)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data structure is embedded into another data structure.</param>
        public override JObject ToJSON(Boolean Embedded = false)

            => ToJSON(Embedded:            false,
                      ExpandTags:          InfoStatus.ShowIdOnly);


        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data structure is embedded into another data structure.</param>
        /// <param name="IncludeCryptoHash">Include the crypto hash value of this object.</param>
        public JObject ToJSON(Boolean     Embedded            = false,
                              InfoStatus  ExpandTags          = InfoStatus.ShowIdOnly,
                              InfoStatus  ExpandAuthorId      = InfoStatus.ShowIdOnly)

            => JSONObject.Create(

                   new JProperty("@id",                Id.             ToString()),

                   !Embedded
                       ? new JProperty("@context",     JSONLDContext.  ToString())
                       : null,

                   new JProperty("headline",           Headline.       ToJSON()),
                   new JProperty("text",               Text.           ToJSON()),

                   new JProperty("authors",            new JArray(Authors.Select(author => JSONObject.Create(
                                                                                               new JProperty("@id",  author.Id.ToString()),
                                                                                               new JProperty("name", author.Name)
                                                                                           )))),

                   new JProperty("publicationDate",    PublicationDate.ToIso8601()),
                   new JProperty("lastChangeDate",     LastChangeDate. ToIso8601()),

                   Tags.Any()
                       ? new JProperty("tags",         new JArray(Tags.Select(tag => tag.ToJSON(ExpandTags))))
                       : null,

                   Signatures.Any()
                       ? new JProperty("signatures",   new JArray(Signatures.SafeSelect(signature => signature.ToJSON(Embedded: true))))
                       : null

               );

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out News, out ErrorResponse)

        public static Boolean TryParseJSON(JObject               JSONObject,
                                           UserProviderDelegate  UserProvider,
                                           out NewsPosting?      NewsPosting,
                                           out String?           ErrorResponse,
                                           NewsPosting_Id?       NewsIdURL  = null)
        {

            try
            {

                NewsPosting = null;

                #region Parse NewsId            [optional]

                // Verify that a given News identification
                //   is at least valid.
                if (JSONObject.ParseOptional("@id",
                                             "News identification",
                                             NewsPosting_Id.TryParse,
                                             out NewsPosting_Id? NewsIdBody,
                                             out ErrorResponse))
                {
                    if (ErrorResponse is not null)
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

                IUser? Author = null;

                if (JSONObject["author"] is JObject authorJSON &&
                    authorJSON.ParseMandatory("@id",
                                              "author identification",
                                              User_Id.TryParse,
                                              out User_Id AuthorId,
                                              out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                    if (!UserProvider(AuthorId, out Author))
                    {
                        ErrorResponse = "The given author '" + AuthorId + "' is unknown!";
                        return false;
                    }

                }

                else
                    return false;

                var Authors = new User[0];

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

                #region Parse LastChangeDate     [mandatory]

                if (!JSONObject.ParseMandatory("LastChangeDate",
                                               "last change date",
                                               out DateTime LastChangeDate,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                var Tags             = Array.Empty<TagRelevance>();

                #region Parse IsHidden          [optional]

                if (JSONObject.ParseOptional("isHidden",
                                             "is hidden",
                                             out Boolean? IsHidden,
                                             out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                var Signatures       = Array.Empty<Signature>();

                var CustomData = JSONObject["CustomData"] as JObject;

                #region Get   DataSource        [optional]

                var DataSource = JSONObject.GetOptional("dataSource");

                #endregion


                NewsPosting = new NewsPosting(Headline,
                                              Text,
                                              NewsIdBody ?? NewsIdURL.Value,
                                              Authors,
                                              PublicationDate,
                                              LastChangeDate,
                                              Tags,
                                              PrivacyLevel.World,
                                              IsHidden ?? false,
                                              Signatures,

                                              CustomData,
                                              DataSource);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                NewsPosting    = null;
                ErrorResponse  = e.Message;
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

            => new (NewNewsPostingId ?? Id,
                    Headline,
                    Text,
                    Authors,
                    PublicationDate,
                    LastChangeDate,
                    Tags,
                    IsHidden,
                    Signatures,

                    CustomData,
                    DataSource);

        #endregion

        #region (class) Builder

        /// <summary>
        /// A news posting builder.
        /// </summary>
        public new class Builder : AEntity<NewsPosting_Id, NewsPosting>.Builder
        {

            #region Properties

            /// <summary>
            /// The (multi-language) headline of this news posting.
            /// </summary>
            [Mandatory]
            public I18NString?                        Headline              { get; set; }

            /// <summary>
            /// The (multi-language) text of this news posting.
            /// </summary>
            [Mandatory]
            public I18NString?                        Text                  { get; set; }

            /// <summary>
            /// The optional author(s) of this news posting.
            /// </summary>
            [Optional]
            public HashSet<User>                      Authors               { get; }

            /// <summary>
            /// The publication timestamp of this news posting.
            /// </summary>
            [Mandatory]
            public DateTime?                          PublicationDate       { get; set; }

            /// <summary>
            /// An enumeration of multi-language tags and their relevance.
            /// </summary>
            [Optional]
            public HashSet<TagRelevance>              Tags                  { get; }

            /// <summary>
            /// Whether the news posting is currently hidden.
            /// </summary>
            [Optional]
            public Boolean                            IsHidden              { get; set; }

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
                           I18NString?                 Headline          = null,
                           I18NString?                 Text              = null,
                           IEnumerable<User>?          Authors           = null,
                           DateTime?                   PublicationDate   = null,
                           DateTime?                   LastChangeDate    = null,
                           IEnumerable<TagRelevance>?  Tags              = null,
                           Boolean                     IsHidden          = false,
                           IEnumerable<Signature>?     Signatures        = null,

                           JObject?                    CustomData        = default,
                           String?                     DataSource        = default)

                : base(Id ?? NewsPosting_Id.Random(),
                       DefaultJSONLDContext,
                       LastChangeDate,
                       Signatures,
                       CustomData,
                       null,
                       DataSource)

            {

                this.Headline         = Headline;
                this.Text             = Text;
                this.Authors          = Authors is not null
                                            ? new HashSet<User>(Authors)
                                            : new HashSet<User>();
                this.PublicationDate  = PublicationDate ?? Timestamp.Now;
                this.Tags             = Tags    is not null
                                            ? new HashSet<TagRelevance>(Tags)
                                            : new HashSet<TagRelevance>();
                this.IsHidden         = IsHidden;

            }

            #endregion


            public NewsPosting Sign(ICipherParameters PrivateKey)
            {

                var news        = new NewsPosting(Headline,
                                                  Text,
                                                  Id,
                                                  Authors,
                                                  PublicationDate,
                                                  LastChangeDate,
                                                  Tags,
                                                  PrivacyLevel.World,
                                                  IsHidden,
                                                  Signatures);

                var ctext       = news.ToJSON(Embedded:    false,
                                              ExpandTags:  InfoStatus.ShowIdOnly).ToString(Newtonsoft.Json.Formatting.None);

                var BlockSize   = 32;

                var SHA256      = new SHA256Managed();
                var SHA256Hash  = SHA256.ComputeHash(ctext.ToUTF8Bytes());
                var signer      = SignerUtilities.GetSigner("NONEwithECDSA");
                signer.Init(true, PrivateKey);
                signer.BlockUpdate(SHA256Hash, 0, BlockSize);

                var signature   = signer.GenerateSignature().ToHexString();
                var signatures  = new List<Signature>(Signatures);
                signatures.Add(new Signature("json", "secp256k1", "DER+HEX", signature));

                return new NewsPosting(Headline,
                                       Text,
                                       Id,
                                       Authors,
                                       PublicationDate,
                                       LastChangeDate,
                                       Tags,
                                       PrivacyLevel.World,
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

            public override bool Equals(NewsPosting? other)
            {
                throw new NotImplementedException();
            }

            public override int CompareTo(NewsPosting? other)
            {
                throw new NotImplementedException();
            }

            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the news posting.
            /// </summary>
            /// <param name="Builder">A news posting builder.</param>
            public static implicit operator NewsPosting(Builder Builder)

                => Builder.ToImmutable;


            /// <summary>
            /// Return an immutable version of the news posting.
            /// </summary>
            public NewsPosting ToImmutable
            {
                get
                {

                    if (Headline is null || Headline.IsNullOrEmpty())
                        throw new ArgumentNullException(nameof(Headline), "The given headline must not be null or empty!");

                    if (Text     is null || Text.    IsNullOrEmpty())
                        throw new ArgumentNullException(nameof(Text),     "The given text must not be null or empty!");

                    return new (Headline,
                                Text,
                                Id,
                                Authors,
                                PublicationDate,
                                LastChangeDate,
                                Tags,
                                PrivacyLevel.World,
                                IsHidden,
                                Signatures,
                                CustomData,
                                DataSource);

                }
            }

            #endregion

        }

        #endregion

    }

}
