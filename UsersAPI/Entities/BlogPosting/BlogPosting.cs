/*
 * Copyright (c) 2014-2019, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Org.BouncyCastle.Bcpg.OpenPgp;

using social.OpenData.UsersAPI.Notifications;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.Distributed;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Styx.Arrows;
using org.GraphDefined.Vanaheimr.BouncyCastle;
using System.Security.Cryptography;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;

#endregion

namespace social.OpenData.UsersAPI.Postings
{

//    public delegate Boolean PostingProviderDelegate(BlogPosting_Id PostingId, out BlogPosting Posting);

    public delegate JObject BlogPostingToJSONDelegate(BlogPosting  BlogPosting,
                                                      Boolean      Embedded            = false,
                                                      InfoStatus   ExpandTags          = InfoStatus.ShowIdOnly,
                                                      Boolean      IncludeCryptoHash   = true);


    /// <summary>
    /// Extention methods for blog postings.
    /// </summary>
    public static class BlogPostingExtentions
    {

        #region ToJSON(this BlogPostings, Skip = null, Take = null, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of blog postings.
        /// </summary>
        /// <param name="BlogPostings">An enumeration of blog postings.</param>
        /// <param name="Skip">The optional number of blog postings to skip.</param>
        /// <param name="Take">The optional number of blog postings to return.</param>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        public static JArray ToJSON(this IEnumerable<BlogPosting>  BlogPostings,
                                    UInt64?                        Skip                = null,
                                    UInt64?                        Take                = null,
                                    Boolean                        Embedded            = false,
                                    InfoStatus                     ExpandTags          = InfoStatus.ShowIdOnly,
                                    BlogPostingToJSONDelegate      BlogPostingToJSON   = null,
                                    Boolean                        IncludeCryptoHash   = true)


            => BlogPostings?.Any() != true

                   ? new JArray()

                   : new JArray(BlogPostings.
                                    Where     (dataSet =>  dataSet != null).
                                    OrderBy   (dataSet => dataSet.Id).
                                    SkipTakeFilter(Skip, Take).
                                    SafeSelect(BlogPosting => BlogPostingToJSON != null
                                                                    ? BlogPostingToJSON (BlogPosting,
                                                                                         Embedded,
                                                                                         ExpandTags,
                                                                                         IncludeCryptoHash)

                                                                    : BlogPosting.ToJSON(Embedded,
                                                                                         ExpandTags,
                                                                                         IncludeCryptoHash)));

        #endregion

    }

    /// <summary>
    /// An Open Data blog posting.
    /// </summary>
    public class BlogPosting : ADistributedEntity<BlogPosting_Id>,
                               IEntityClass<BlogPosting>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public const String JSONLDContext  = "https://opendata.social/contexts/BlogPostingsAPI+json/BlogPosting";

        #endregion

        #region Properties

        #region API

        private Object _API;

        /// <summary>
        /// The PostingsAPI of this Posting.
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
                    throw new ArgumentException("Illegal attempt to change the API of this communicator!");

                if (value == null)
                    throw new ArgumentException("Illegal attempt to delete the API reference of this communicator!");

                _API = value;

            }

        }

        #endregion

        /// <summary>
        /// The (multi-language) text of this blog posting.
        /// </summary>
        [Mandatory]
        public I18NString                         Text                  { get; }

        /// <summary>
        /// The timestamp of the publication of this blog posting.
        /// </summary>
        [Mandatory]
        public DateTime                           PublicationDate       { get; }

        /// <summary>
        /// An optional geographical location of this blog posting.
        /// </summary>
        [Optional]
        public GeoCoordinate?                     GeoLocation           { get; }

        /// <summary>
        /// An enumeration of multi-language tags and their relevance.
        /// </summary>
        [Optional]
        public IEnumerable<TagRelevance>          Tags                  { get; }

        /// <summary>
        /// Whether the blog posting will be shown in blog posting listings, or not.
        /// </summary>
        [Mandatory]
        public PrivacyLevel                       PrivacyLevel          { get; }

        /// <summary>
        /// The blog posting is hidden.
        /// </summary>
        [Optional]
        public Boolean                            IsHidden              { get; }


        /// <summary>
        /// All signatures of this blog posting.
        /// </summary>
        public IEnumerable<BlogPostingSignature>  Signatures            { get; }

        #endregion

        #region Events

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Open Data blog posting.
        /// </summary>
        /// <param name="Text">The (multi-language) text of this blog posting.</param>
        /// <param name="PublicationDate">The timestamp of the publication of this blog posting.</param>
        /// <param name="GeoLocation">An optional geographical location of this blog posting.</param>
        /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
        /// <param name="PrivacyLevel">Whether the blog posting will be shown in blog posting listings, or not.</param>
        /// <param name="IsHidden">The blog posting is hidden.</param>
        /// <param name="Signatures">All signatures of this blog posting.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        public BlogPosting(I18NString                         Text,
                           DateTime?                          PublicationDate   = null,
                           GeoCoordinate?                     GeoLocation       = null,
                           IEnumerable<TagRelevance>          Tags              = null,
                           PrivacyLevel?                      PrivacyLevel      = null,
                           Boolean                            IsHidden          = false,
                           IEnumerable<BlogPostingSignature>  Signatures        = null,
                           String                             DataSource        = "")

            : this(BlogPosting_Id.Random(),
                   Text,
                   PublicationDate,
                   GeoLocation,
                   Tags,
                   PrivacyLevel,
                   IsHidden,
                   Signatures,
                   DataSource)

        { }


        /// <summary>
        /// Create a new Open Data blog posting.
        /// </summary>
        /// <param name="Id">The unique identification of this blog posting.</param>
        /// <param name="Text">The (multi-language) text of this blog posting.</param>
        /// <param name="PublicationDate">The timestamp of the publication of this blog posting.</param>
        /// <param name="GeoLocation">An optional geographical location of this blog posting.</param>
        /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
        /// <param name="PrivacyLevel">Whether the blog posting will be shown in blog posting listings, or not.</param>
        /// <param name="IsHidden">The blog posting is hidden.</param>
        /// <param name="Signatures">All signatures of this blog posting.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        public BlogPosting(BlogPosting_Id                     Id,
                           I18NString                         Text,
                           DateTime?                          PublicationDate   = null,
                           GeoCoordinate?                     GeoLocation       = null,
                           IEnumerable<TagRelevance>          Tags              = null,
                           PrivacyLevel?                      PrivacyLevel      = null,
                           Boolean                            IsHidden          = false,
                           IEnumerable<BlogPostingSignature>  Signatures        = null,
                           String                             DataSource        = "")

            : base(Id,
                   DataSource)

        {

            this.Text             = Text;
            this.PublicationDate  = PublicationDate ?? DateTime.Now;
            this.GeoLocation      = GeoLocation;
            this.Tags             = Tags            ?? new TagRelevance[0];
            this.PrivacyLevel     = PrivacyLevel    ?? social.OpenData.UsersAPI.PrivacyLevel.Private;
            this.IsHidden         = false;
            this.Signatures       = Signatures      ?? new BlogPostingSignature[0];

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
        public JObject ToJSON(Boolean     Embedded             = false,
                              InfoStatus  ExpandTags           = InfoStatus.ShowIdOnly,
                              Boolean     IncludeCryptoHash    = false)

            => JSONObject.Create(

                   new JProperty("@id",                  Id.ToString()),

                   !Embedded
                       ? new JProperty("@context",       JSONLDContext)
                       : null,

                   Text.IsNeitherNullNorEmpty()
                       ? new JProperty("text",           Text.ToJSON())
                       : null,

                   new JProperty("publicationDate",      PublicationDate.ToIso8601()),

                   GeoLocation.HasValue
                       ? new JProperty("geoLocation",    GeoLocation.ToJSON())
                       : null,

                   Tags.Any()
                       ? new JProperty("tags",           Tags.SafeSelect(tag => tag.ToJSON(ExpandTags)))
                       : null,

                   PrivacyLevel.ToJSON(),

                   Signatures.Any()
                       ? new JProperty("signatures",     new JArray(Signatures.SafeSelect(signature => signature.ToJSON(Embedded: true))))
                       : null,

                   IncludeCryptoHash
                       ? new JProperty("hash", CurrentCryptoHash)
                       : null

               );

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out Posting, out ErrorResponse)

        public static Boolean TryParseJSON(JObject          JSONObject,
                                           out BlogPosting  Posting,
                                           out String       ErrorResponse,
                                           BlogPosting_Id?  PostingIdURI  = null)
        {

            try
            {

                Posting = null;

                #region Parse PostingId        [optional]

                // Verify that a given Posting identification
                //   is at least valid.
                if (JSONObject.ParseOptionalStruct("@id",
                                                   "Posting identification",
                                                   BlogPosting_Id.TryParse,
                                                   out BlogPosting_Id? PostingIdBody,
                                                   out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                if (!PostingIdURI.HasValue && !PostingIdBody.HasValue)
                {
                    ErrorResponse = "The Posting identification is missing!";
                    return false;
                }

                if (PostingIdURI.HasValue && PostingIdBody.HasValue && PostingIdURI.Value != PostingIdBody.Value)
                {
                    ErrorResponse = "The optional Posting identification given within the JSON body does not match the one given in the URI!";
                    return false;
                }

                #endregion

                #region Parse Context          [mandatory]

                if (!JSONObject.ParseMandatory("@context",
                                               "JSON-LinkedData context information",
                                               out String Context,
                                               out ErrorResponse))
                {
                    ErrorResponse = @"The JSON-LD ""@context"" information is missing!";
                    return false;
                }

                if (Context != JSONLDContext)
                {
                    ErrorResponse = @"The given JSON-LD ""@context"" information '" + Context + "' is not supported!";
                    return false;
                }

                #endregion

                #region Parse Text             [mandatory]

                if (!JSONObject.ParseMandatory("text",
                                               "blog posting text",
                                               out I18NString Text,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                var PublicationDate  = DateTime.Now;

                #region Parse GeoLocation      [optional]

                if (JSONObject.ParseOptionalStruct("geoLocation",
                                                   "Geo location",
                                                   GeoCoordinate.TryParseJSON,
                                                   out GeoCoordinate? GeoLocation,
                                                   out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                var Tags             = new TagRelevance[0];

                #region Parse PrivacyLevel     [optional]

                if (JSONObject.ParseOptionalEnum("privacyLevel",
                                             "privacy level",
                                             out PrivacyLevel? PrivacyLevel,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                var IsHidden         = JSONObject["isHidden"]?.     Value<Boolean>();

                var Signatures       = new BlogPostingSignature[0];

                #region Get   DataSource       [optional]

                var DataSource = JSONObject.GetOptional("dataSource");

                #endregion

                #region Parse CryptoHash       [optional]

                var CryptoHash    = JSONObject.GetOptional("cryptoHash");

                #endregion


                Posting = new BlogPosting(PostingIdBody ?? PostingIdURI.Value,
                                          Text,
                                          PublicationDate,
                                          GeoLocation,
                                          Tags,
                                          PrivacyLevel,
                                          IsHidden      ?? false,
                                          Signatures,
                                          DataSource);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse  = e.Message;
                Posting           = null;
                return false;
            }

        }

        #endregion


        #region CopyAllEdgesTo(NewPosting)

        public void CopyAllEdgesTo(BlogPosting NewPosting)
        {



        }

        #endregion


        #region Operator overloading

        #region Operator == (PostingId1, PostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PostingId1">A Posting identification.</param>
        /// <param name="PostingId2">Another Posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (BlogPosting PostingId1, BlogPosting PostingId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(PostingId1, PostingId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) PostingId1 == null) || ((Object) PostingId2 == null))
                return false;

            return PostingId1.Equals(PostingId2);

        }

        #endregion

        #region Operator != (PostingId1, PostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PostingId1">A Posting identification.</param>
        /// <param name="PostingId2">Another Posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (BlogPosting PostingId1, BlogPosting PostingId2)
            => !(PostingId1 == PostingId2);

        #endregion

        #region Operator <  (PostingId1, PostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PostingId1">A Posting identification.</param>
        /// <param name="PostingId2">Another Posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (BlogPosting PostingId1, BlogPosting PostingId2)
        {

            if ((Object) PostingId1 == null)
                throw new ArgumentNullException(nameof(PostingId1), "The given PostingId1 must not be null!");

            return PostingId1.CompareTo(PostingId2) < 0;

        }

        #endregion

        #region Operator <= (PostingId1, PostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PostingId1">A Posting identification.</param>
        /// <param name="PostingId2">Another Posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (BlogPosting PostingId1, BlogPosting PostingId2)
            => !(PostingId1 > PostingId2);

        #endregion

        #region Operator >  (PostingId1, PostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PostingId1">A Posting identification.</param>
        /// <param name="PostingId2">Another Posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (BlogPosting PostingId1, BlogPosting PostingId2)
        {

            if ((Object) PostingId1 == null)
                throw new ArgumentNullException(nameof(PostingId1), "The given PostingId1 must not be null!");

            return PostingId1.CompareTo(PostingId2) > 0;

        }

        #endregion

        #region Operator >= (PostingId1, PostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="PostingId1">A Posting identification.</param>
        /// <param name="PostingId2">Another Posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (BlogPosting PostingId1, BlogPosting PostingId2)
            => !(PostingId1 < PostingId2);

        #endregion

        #endregion

        #region IComparable<Posting> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            var Posting = Object as BlogPosting;
            if ((Object) Posting == null)
                throw new ArgumentException("The given object is not an Posting!");

            return CompareTo(Posting);

        }

        #endregion

        #region CompareTo(Posting)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Posting">An Posting object to compare with.</param>
        public Int32 CompareTo(BlogPosting Posting)
        {

            if ((Object) Posting == null)
                throw new ArgumentNullException("The given Posting must not be null!");

            return Id.CompareTo(Posting.Id);

        }

        #endregion

        #endregion

        #region IEquatable<Posting> Members

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

            var Posting = Object as BlogPosting;
            if ((Object) Posting == null)
                return false;

            return Equals(Posting);

        }

        #endregion

        #region Equals(Posting)

        /// <summary>
        /// Compares two Postings for equality.
        /// </summary>
        /// <param name="Posting">An Posting to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(BlogPosting Posting)
        {

            if ((Object) Posting == null)
                return false;

            return Id.Equals(Posting.Id);

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


        #region ToBuilder(NewPostingId = null)

        /// <summary>
        /// Return a builder for this Posting.
        /// </summary>
        /// <param name="NewPostingId">An optional new Posting identification.</param>
        public Builder ToBuilder(BlogPosting_Id? NewPostingId = null)

            => new Builder(NewPostingId ?? Id,
                           Text,
                           PublicationDate,
                           GeoLocation,
                           Tags,
                           PrivacyLevel,
                           IsHidden,
                           Signatures,
                           DataSource);

        #endregion

        #region (class) Builder

        /// <summary>
        /// An Open Data blog posting builder.
        /// </summary>
        public class Builder
        {

            #region Properties

            /// <summary>
            /// The unique identification of this blog posting.
            /// </summary>
            public BlogPosting_Id                     Id                    { get; set; }

            /// <summary>
            /// The (multi-language) text of this blog posting.
            /// </summary>
            [Mandatory]
            public I18NString                         Text                  { get; set; }

            /// <summary>
            /// The timestamp of the publication of this blog posting.
            /// </summary>
            [Mandatory]
            public DateTime                           PublicationDate       { get; set; }

            /// <summary>
            /// An optional geographical location of this blog posting.
            /// </summary>
            [Optional]
            public GeoCoordinate?                     GeoLocation           { get; set; }

            /// <summary>
            /// An enumeration of multi-language tags and their relevance.
            /// </summary>
            [Optional]
            public IEnumerable<TagRelevance>          Tags                  { get; set; }

            /// <summary>
            /// Whether the blog posting will be shown in blog posting listings, or not.
            /// </summary>
            [Mandatory]
            public PrivacyLevel                       PrivacyLevel          { get; set; }

            /// <summary>
            /// The blog posting is hidden.
            /// </summary>
            [Optional]
            public Boolean                            IsHidden              { get; set; }


            /// <summary>
            /// All signatures of this blog posting.
            /// </summary>
            public IEnumerable<BlogPostingSignature>  Signatures            { get; set; }

            /// <summary>
            /// The source of this information, e.g. an automatic importer.
            /// </summary>
            [Optional]
            public String                             DataSource            { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new Posting builder.
            /// </summary>
            /// <param name="Text">The (multi-language) text of this blog posting.</param>
            /// <param name="PublicationDate">The timestamp of the publication of this blog posting.</param>
            /// <param name="GeoLocation">An optional geographical location of this blog posting.</param>
            /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
            /// <param name="PrivacyLevel">Whether the blog posting will be shown in blog posting listings, or not.</param>
            /// <param name="IsHidden">The blog posting is hidden.</param>
            /// <param name="Signatures">All signatures of this blog posting.</param>
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            public Builder(I18NString                         Text,
                           DateTime?                          PublicationDate  = null,
                           GeoCoordinate?                     GeoLocation      = null,
                           IEnumerable<TagRelevance>          Tags             = null,
                           PrivacyLevel?                      PrivacyLevel     = null,
                           Boolean                            IsHidden         = false,
                           IEnumerable<BlogPostingSignature>  Signatures       = null,
                           String                             DataSource       = "")

                : this(BlogPosting_Id.Random(),
                       Text,
                       PublicationDate,
                       GeoLocation,
                       Tags,
                       PrivacyLevel,
                       IsHidden,
                       Signatures,
                       DataSource)

            { }


            /// <summary>
            /// Create a new Posting builder.
            /// </summary>
            /// <param name="Id">The unique identification of this blog posting.</param>
            /// <param name="Text">The (multi-language) text of this blog posting.</param>
            /// <param name="PublicationDate">The timestamp of the publication of this blog posting.</param>
            /// <param name="GeoLocation">An optional geographical location of this blog posting.</param>
            /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
            /// <param name="PrivacyLevel">Whether the blog posting will be shown in blog posting listings, or not.</param>
            /// <param name="IsHidden">The blog posting is hidden.</param>
            /// <param name="Signatures">All signatures of this blog posting.</param>
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            public Builder(BlogPosting_Id                     Id,
                           I18NString                         Text,
                           DateTime?                          PublicationDate  = null,
                           GeoCoordinate?                     GeoLocation      = null,
                           IEnumerable<TagRelevance>          Tags             = null,
                           PrivacyLevel?                      PrivacyLevel     = null,
                           Boolean                            IsHidden         = false,
                           IEnumerable<BlogPostingSignature>  Signatures       = null,
                           String                             DataSource       = "")

            {

                this.Id               = Id;
                this.Text             = Text;
                this.PublicationDate  = PublicationDate ?? DateTime.Now;
                this.GeoLocation      = GeoLocation;
                this.Tags             = Tags         ?? new TagRelevance[0];
                this.PrivacyLevel     = PrivacyLevel ?? social.OpenData.UsersAPI.PrivacyLevel.Private;
                this.IsHidden         = false;
                this.Signatures       = Signatures   ?? new BlogPostingSignature[0];
                this.DataSource       = DataSource;

            }

            #endregion


            public BlogPosting Sign(ICipherParameters PrivateKey)
            {

                var posting     = new BlogPosting(Id,
                                                  Text,
                                                  PublicationDate,
                                                  GeoLocation,
                                                  Tags,
                                                  PrivacyLevel,
                                                  IsHidden,
                                                  Signatures,
                                                  DataSource);

                var ctext       = posting.ToJSON(Embedded:           false,
                                                 ExpandTags:         InfoStatus.ShowIdOnly,
                                                 IncludeCryptoHash:  false).ToString(Newtonsoft.Json.Formatting.None);

                var BlockSize   = 32;

                var SHA256      = new SHA256Managed();
                var SHA256Hash  = SHA256.ComputeHash(ctext.ToUTF8Bytes());
                var signer      = SignerUtilities.GetSigner("NONEwithECDSA");
                signer.Init(true, PrivateKey);
                signer.BlockUpdate(SHA256Hash, 0, BlockSize);

                var signature   = signer.GenerateSignature().ToHexString();
                var signatures  = new List<BlogPostingSignature>(Signatures);
                signatures.Add(new BlogPostingSignature("json", "secp256k1", "DER+HEX", signature));

                return new BlogPosting(Id,
                                       Text,
                                       PublicationDate,
                                       GeoLocation,
                                       Tags,
                                       PrivacyLevel,
                                       IsHidden,
                                       signatures,
                                       DataSource);

            }


            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the Posting.
            /// </summary>
            /// <param name="Builder">A Posting builder.</param>
            public static implicit operator BlogPosting(Builder Builder)

                => Builder?.ToImmutable;


            /// <summary>
            /// Return an immutable version of the Posting.
            /// </summary>
            public BlogPosting ToImmutable

                => new BlogPosting(Id,
                                   Text,
                                   PublicationDate,
                                   GeoLocation,
                                   Tags,
                                   PrivacyLevel,
                                   IsHidden,
                                   Signatures,
                                   DataSource);

            #endregion

        }

        #endregion

    }

}
