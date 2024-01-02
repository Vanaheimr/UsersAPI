/*
 * Copyright (c) 2014-2024 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Styx.Arrows;

#endregion

namespace social.OpenData.UsersAPI
{

//    public delegate Boolean PostingProviderDelegate(BlogPosting_Id BlogPostingId, out BlogPosting Posting);

    public delegate JObject BlogPostingToJSONDelegate(BlogPosting  BlogPosting,
                                                      Boolean      Embedded         = false,
                                                      InfoStatus   ExpandTags       = InfoStatus.ShowIdOnly,
                                                      InfoStatus   ExpandAuthorId   = InfoStatus.ShowIdOnly);


    /// <summary>
    /// Extension methods for blog postings.
    /// </summary>
    public static class BlogPostingExtensions
    {

        #region ToJSON(this BlogPostings, Skip = null, Take = null, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of blog postings.
        /// </summary>
        /// <param name="BlogPostings">An enumeration of blog postings.</param>
        /// <param name="Skip">The optional number of blog postings to skip.</param>
        /// <param name="Take">The optional number of blog postings to return.</param>
        /// <param name="Embedded">Whether this data structure is embedded into another data structure.</param>
        public static JArray ToJSON(this IEnumerable<BlogPosting>  BlogPostings,
                                    UInt64?                        Skip                = null,
                                    UInt64?                        Take                = null,
                                    Boolean                        Embedded            = false,
                                    InfoStatus                     ExpandTags          = InfoStatus.ShowIdOnly,
                                    InfoStatus                     ExpandAuthorId      = InfoStatus.ShowIdOnly,
                                    BlogPostingToJSONDelegate?     BlogPostingToJSON   = null)


            => BlogPostings?.Any() != true

                   ? new JArray()

                   : new JArray(BlogPostings.
                                    Where            (blogPosting => blogPosting is not null).
                                    OrderByDescending(blogPosting => blogPosting.PublicationDate).
                                    SkipTakeFilter   (Skip, Take).
                                    SafeSelect       (blogPosting => BlogPostingToJSON is not null
                                                                         ? BlogPostingToJSON (blogPosting,
                                                                                              Embedded,
                                                                                              ExpandTags)

                                                                         : blogPosting.ToJSON(Embedded,
                                                                                              ExpandTags)));

        #endregion

    }

    /// <summary>
    /// A blog posting.
    /// </summary>
    public class BlogPosting : AEntity<BlogPosting_Id,
                                       BlogPosting>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of organizations.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://opendata.social/contexts/UsersAPI/blogPosting");

        #endregion

        #region Properties

        #region API

        private Object? _API;

        /// <summary>
        /// The PostingsAPI of this Posting.
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
                    throw new ArgumentException("Illegal attempt to change the API of this communicator!");

                if (value == null)
                    throw new ArgumentException("Illegal attempt to delete the API reference of this communicator!");

                _API = value;

            }

        }

        #endregion

        /// <summary>
        /// The (multi-language) title of this blog posting.
        /// </summary>
        [Mandatory]
        public I18NString                         Title                 { get; }

        /// <summary>
        /// The (multi-language) teaser of this blog posting.
        /// </summary>
        [Mandatory]
        public I18NString                         Teaser                { get; }

        /// <summary>
        /// The teaser image of this blog posting.
        /// </summary>
        [Mandatory]
        public String                             TeaserImage           { get; }

        /// <summary>
        /// The (multi-language) text of this blog posting.
        /// </summary>
        [Mandatory]
        public I18NString                         Text                  { get; }

        /// <summary>
        /// The optional author(s) of this blog posting.
        /// </summary>
        [Optional]
        public IEnumerable<IUser>                 Authors               { get; }

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
        /// The optinal category of the blog posting.
        /// </summary>
        [Optional]
        public I18NString?                        Category              { get; }

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

        #endregion

        #region Events

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new blog posting.
        /// </summary>
        /// <param name="Title">The (multi-language) title of this blog posting.</param>
        /// <param name="Teaser">The (multi-language) teaser of this blog posting.</param>
        /// <param name="TeaserImage">The teaser image of this blog posting.</param>
        /// <param name="Text">The (multi-language) text of this blog posting.</param>
        /// <param name="Id">The unique identification of this blog posting.</param>
        /// <param name="Authors">The optional authors of this blog posting.</param>
        /// <param name="PublicationDate">The timestamp of the publication of this blog posting.</param>
        /// <param name="LastChangeDate">The timestamp of the last change of this blog posting.</param>
        /// <param name="GeoLocation">An optional geographical location of this blog posting.</param>
        /// <param name="Category">The optinal category of the blog posting.</param>
        /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
        /// <param name="PrivacyLevel">Whether the blog posting will be shown in blog posting listings, or not.</param>
        /// <param name="IsHidden">The blog posting is hidden.</param>
        /// 
        /// <param name="Signatures">All signatures of this blog posting.</param>
        /// <param name="CustomData">Custom data stored within this entity.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        public BlogPosting(I18NString                  Title,
                           I18NString                  Teaser,
                           String                      TeaserImage,
                           I18NString                  Text,
                           BlogPosting_Id?             Id                = null,
                           IEnumerable<IUser>?         Authors           = null,
                           DateTime?                   PublicationDate   = null,
                           DateTime?                   LastChangeDate    = null,
                           GeoCoordinate?              GeoLocation       = null,
                           I18NString?                 Category          = null,
                           IEnumerable<TagRelevance>?  Tags              = null,
                           PrivacyLevel?               PrivacyLevel      = null,
                           Boolean                     IsHidden          = false,

                           IEnumerable<Signature>?     Signatures        = null,
                           JObject?                    CustomData        = default,
                           String?                     DataSource        = default)

            : base(Id ?? BlogPosting_Id.Random(),
                   DefaultJSONLDContext,
                   null,
                   null,
                   Signatures,
                   CustomData,
                   null,
                   LastChangeDate,
                   DataSource)

        {

            this.Title            = Title;
            this.Teaser           = Teaser;
            this.TeaserImage      = TeaserImage;
            this.Text             = Text;
            this.Authors          = Authors         ?? Array.Empty<IUser>();
            this.PublicationDate  = PublicationDate ?? Timestamp.Now;
            this.GeoLocation      = GeoLocation;
            this.Category         = Category;
            this.Tags             = Tags            ?? Array.Empty<TagRelevance>();
            this.PrivacyLevel     = PrivacyLevel    ?? org.GraphDefined.Vanaheimr.Hermod.HTTP.PrivacyLevel.Private;
            this.IsHidden         = IsHidden;

        }

        #endregion


        #region ToJSON(Embedded = false)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data structure is embedded into another data structure.</param>
        public override JObject ToJSON(Boolean Embedded = false)

            => ToJSON(Embedded:    false,
                      ExpandTags:  InfoStatus.ShowIdOnly);


        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data structure is embedded into another data structure.</param>
        /// <param name="IncludeCryptoHash">Include the crypto hash value of this object.</param>
        public JObject ToJSON(Boolean     Embedded        = false,
                              InfoStatus  ExpandTags      = InfoStatus.ShowIdOnly,
                              InfoStatus  ExpandAuthorId  = InfoStatus.ShowIdOnly)

            => JSONObject.Create(

                   new JProperty("@id",                  Id.ToString()),

                   !Embedded
                       ? new JProperty("@context",       JSONLDContext.ToString())
                       : null,

                   new JProperty("title",                Title.      ToJSON()),
                   new JProperty("teaser",               Teaser.     ToJSON()),
                   new JProperty("teaserImage",          TeaserImage),
                   new JProperty("text",                 Text.       ToJSON()),

                   new JProperty("authors",              new JArray(Authors.Select(author => JSONObject.Create(
                                                                                                 new JProperty("@id",  author.Id.ToString()),
                                                                                                 new JProperty("name", author.Name)
                                                                                             )))),

                   new JProperty("publicationDate",      PublicationDate.ToIso8601()),
                   new JProperty("lastChangeDate",       LastChangeDate. ToIso8601()),

                   GeoLocation.HasValue
                       ? new JProperty("geoLocation",    GeoLocation.Value.ToJSON())
                       : null,

                   Category is not null && Category.IsNotNullOrEmpty()
                       ? new JProperty("category",       Category.ToJSON())
                       : null,

                   Tags is not null && Tags.Any()
                       ? new JProperty("tags",           new JArray(Tags.Select(tag => tag.ToJSON(ExpandTags))))
                       : null,

                   PrivacyLevel.ToJSON(),

                   Signatures is not null && Signatures.Any()
                       ? new JProperty("signatures",     new JArray(Signatures.SafeSelect(signature => signature.ToJSON(Embedded: true))))
                       : null

               );

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out BlogPosting, out ErrorResponse)

        public static Boolean TryParseJSON(JObject           JSONObject,
                                           out BlogPosting?  BlogPosting,
                                           out String?       ErrorResponse,
                                           BlogPosting_Id?   BlogPostingIdURL   = null)
        {

            try
            {

                BlogPosting = default;

                #region Parse BlogPostingId      [optional]

                // Verify that a given blog posting identification
                //   is at least valid.
                if (JSONObject.ParseOptional("@id",
                                             "blog posting identification",
                                             BlogPosting_Id.TryParse,
                                             out BlogPosting_Id? BlogPostingIdBody,
                                             out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                if (!BlogPostingIdURL.HasValue && !BlogPostingIdBody.HasValue)
                {
                    ErrorResponse = "The blog posting identification is missing!";
                    return false;
                }

                if (BlogPostingIdURL.HasValue && BlogPostingIdBody.HasValue && BlogPostingIdURL.Value != BlogPostingIdBody.Value)
                {
                    ErrorResponse = "The optional blog posting identification given within the JSON body does not match the one given in the URI!";
                    return false;
                }

                #endregion

                #region Parse Context            [mandatory]

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

                #region Parse Title              [mandatory]

                if (!JSONObject.ParseMandatory("title",
                                               "blog posting title",
                                               out I18NString Title,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Teaser             [mandatory]

                if (!JSONObject.ParseMandatory("teaser",
                                               "blog posting teaser",
                                               out I18NString Teaser,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse TeaserImage        [mandatory]

                var TeaserImage = JSONObject.GetString("teaserImage");

                //if (!JSONObject.ParseMandatory("teaserImage",
                //                               "blog posting teaser image",
                //                               URL.TryParse,
                //                               out URL TeaserImage,
                //                               out ErrorResponse))
                //{
                //    return false;
                //}

                #endregion

                #region Parse Text               [mandatory]

                if (!JSONObject.ParseMandatory("text",
                                               "blog posting text",
                                               out I18NString Text,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse PublicationDate    [mandatory]

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

                #region Parse GeoLocation        [optional]

                if (JSONObject.ParseOptionalStruct("geoLocation",
                                                   "Geo location",
                                                   GeoCoordinate.TryParseJSON,
                                                   out GeoCoordinate? GeoLocation,
                                                   out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse Category           [mandatory]

                if (!JSONObject.ParseMandatory("category",
                                               "blog posting category",
                                               out I18NString Category,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                var Tags             = Array.Empty<TagRelevance>();

                #region Parse PrivacyLevel       [optional]

                if (JSONObject.ParseOptionalEnum("privacyLevel",
                                             "privacy level",
                                             out PrivacyLevel? PrivacyLevel,
                                             out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                var IsHidden         = JSONObject["isHidden"]?.     Value<Boolean>();

                var Signatures       = Array.Empty<Signature>();

                #region Get   DataSource         [optional]

                var DataSource = JSONObject.GetOptional("dataSource");

                #endregion

                var CustomData       = JSONObject["CustomData"] as JObject;


                BlogPosting = new BlogPosting(Title,
                                              Teaser,
                                              TeaserImage,
                                              Text,
                                              BlogPostingIdBody ?? BlogPostingIdURL ?? BlogPosting_Id.Random(),
                                              null,
                                              PublicationDate,
                                              LastChangeDate,
                                              GeoLocation,
                                              Category,
                                              Tags,
                                              PrivacyLevel,
                                              IsHidden          ?? false,
                                              Signatures,
                                              CustomData,
                                              DataSource);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                BlogPosting    = null;
                ErrorResponse  = e.Message;
                return false;
            }

        }

        #endregion


        #region CopyAllLinkedDataFrom(OldPosting)

        public override void CopyAllLinkedDataFromBase(BlogPosting OldPosting)
        {



        }

        #endregion


        #region Operator overloading

        #region Operator == (BlogPostingId1, BlogPostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BlogPostingId1">A blog posting identification.</param>
        /// <param name="BlogPostingId2">Another blog posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (BlogPosting BlogPostingId1, BlogPosting BlogPostingId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(BlogPostingId1, BlogPostingId2))
                return true;

            // If one is null, but not both, return false.
            if (BlogPostingId1 is null || BlogPostingId2 is null)
                return false;

            return BlogPostingId1.Equals(BlogPostingId2);

        }

        #endregion

        #region Operator != (BlogPostingId1, BlogPostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BlogPostingId1">A blog posting identification.</param>
        /// <param name="BlogPostingId2">Another blog posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (BlogPosting BlogPostingId1, BlogPosting BlogPostingId2)
            => !(BlogPostingId1 == BlogPostingId2);

        #endregion

        #region Operator <  (BlogPostingId1, BlogPostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BlogPostingId1">A blog posting identification.</param>
        /// <param name="BlogPostingId2">Another blog posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (BlogPosting BlogPostingId1, BlogPosting BlogPostingId2)
        {

            if (BlogPostingId1 is null)
                throw new ArgumentNullException(nameof(BlogPostingId1), "The given BlogPostingId1 must not be null!");

            return BlogPostingId1.CompareTo(BlogPostingId2) < 0;

        }

        #endregion

        #region Operator <= (BlogPostingId1, BlogPostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BlogPostingId1">A blog posting identification.</param>
        /// <param name="BlogPostingId2">Another blog posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (BlogPosting BlogPostingId1, BlogPosting BlogPostingId2)
            => !(BlogPostingId1 > BlogPostingId2);

        #endregion

        #region Operator >  (BlogPostingId1, BlogPostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BlogPostingId1">A blog posting identification.</param>
        /// <param name="BlogPostingId2">Another blog posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (BlogPosting BlogPostingId1, BlogPosting BlogPostingId2)
        {

            if (BlogPostingId1 is null)
                throw new ArgumentNullException(nameof(BlogPostingId1), "The given BlogPostingId1 must not be null!");

            return BlogPostingId1.CompareTo(BlogPostingId2) > 0;

        }

        #endregion

        #region Operator >= (BlogPostingId1, BlogPostingId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="BlogPostingId1">A blog posting identification.</param>
        /// <param name="BlogPostingId2">Another blog posting identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (BlogPosting BlogPostingId1, BlogPosting BlogPostingId2)
            => !(BlogPostingId1 < BlogPostingId2);

        #endregion

        #endregion

        #region IComparable<BlogPosting> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public override Int32 CompareTo(Object Object)

            => Object is BlogPosting blogPosting
                   ? CompareTo(blogPosting)
                   : throw new ArgumentException("The given object is not a blog posting!", nameof(Object));

        #endregion

        #region CompareTo(BlogPosting)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Posting">A blog posting object to compare with.</param>
        public override Int32 CompareTo(BlogPosting BlogPosting)

            => BlogPosting is not null
                   ? Id.CompareTo(BlogPosting.Id)
                   : throw new ArgumentException("The given object is not a blog posting!", nameof(Object));

        #endregion

        #endregion

        #region IEquatable<BlogPosting> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object? Object)

            => Object is BlogPosting blogPosting &&
                  Equals(blogPosting);

        #endregion

        #region Equals(BlogPosting)

        /// <summary>
        /// Compares two Postings for equality.
        /// </summary>
        /// <param name="Posting">A blog posting to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(BlogPosting BlogPosting)

            => BlogPosting is not null &&
                   Id.Equals(BlogPosting.Id);

        #endregion

        #endregion

        #region (override) GetHashCode()

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


        #region ToBuilder(NewBlogPostingId = null)

        /// <summary>
        /// Return a builder for this blog posting.
        /// </summary>
        /// <param name="NewBlogPostingId">An optional new blog posting identification.</param>
        public Builder ToBuilder(BlogPosting_Id? NewBlogPostingId = null)

            => new (NewBlogPostingId ?? Id,
                    Title,
                    Teaser,
                    TeaserImage,
                    Text,
                    Authors,
                    PublicationDate,
                    LastChangeDate,
                    GeoLocation,
                    Category,
                    Tags,
                    PrivacyLevel,
                    IsHidden,
                    Signatures,
                    CustomData,
                    DataSource);

        #endregion

        #region (class) Builder

        /// <summary>
        /// A blog posting builder.
        /// </summary>
        public new class Builder : AEntity<BlogPosting_Id, BlogPosting>.Builder
        {

            #region Properties

            /// <summary>
            /// The (multi-language) title of this blog posting.
            /// </summary>
            [Mandatory]
            public I18NString?                        Title                 { get; set; }

            /// <summary>
            /// The teaser image of this blog posting.
            /// </summary>
            [Mandatory]
            public String?                            TeaserImage           { get; set; }

            /// <summary>
            /// The (multi-language) teaser of this blog posting.
            /// </summary>
            [Mandatory]
            public I18NString?                        Teaser                { get; set; }

            /// <summary>
            /// The (multi-language) text of this blog posting.
            /// </summary>
            [Mandatory]
            public I18NString?                        Text                  { get; set; }

            /// <summary>
            /// The optional authors of this blog posting.
            /// </summary>
            [Optional]
            public HashSet<IUser>                     Authors               { get; }

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
            /// The optional category of the blog posting.
            /// </summary>
            [Optional]
            public I18NString?                        Category              { get; set; }

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

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new blog posting builder.
            /// </summary>
            /// <param name="Id">The unique identification of this blog posting.</param>
            /// <param name="Title">The (multi-language) title of this blog posting.</param>
            /// <param name="Teaser">The (multi-language) teaser of this blog posting.</param>
            /// <param name="TeaserImage">The teaser image of this blog posting.</param>
            /// <param name="Text">The (multi-language) text of this blog posting.</param>
            /// <param name="Authors">The optional authors of this blog posting.</param>
            /// <param name="PublicationDate">The timestamp of the publication of this blog posting.</param>
            /// <param name="LastChangeDate">The timestamp of the last change of this blog posting.</param>
            /// <param name="GeoLocation">An optional geographical location of this blog posting.</param>
            /// <param name="Category">The optinal category of the blog posting.</param>
            /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
            /// <param name="PrivacyLevel">Whether the blog posting will be shown in blog posting listings, or not.</param>
            /// <param name="IsHidden">The blog posting is hidden.</param>
            /// 
            /// <param name="Signatures">All signatures of this blog posting.</param>
            /// <param name="CustomData">Custom data stored within this entity.</param>
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            public Builder(BlogPosting_Id?             Id                = null,
                           I18NString?                 Title             = null,
                           I18NString?                 Teaser            = null,
                           String?                     TeaserImage       = null,
                           I18NString?                 Text              = null,
                           IEnumerable<IUser>?         Authors           = null,
                           DateTime?                   PublicationDate   = null,
                           DateTime?                   LastChangeDate    = null,
                           GeoCoordinate?              GeoLocation       = null,
                           I18NString?                 Category          = null,
                           IEnumerable<TagRelevance>?  Tags              = null,
                           PrivacyLevel?               PrivacyLevel      = null,
                           Boolean                     IsHidden          = false,

                           IEnumerable<Signature>?     Signatures        = null,
                           JObject?                    CustomData        = null,
                           String?                     DataSource        = null)

                : base(Id ?? BlogPosting_Id.Random(),
                       DefaultJSONLDContext,
                       LastChangeDate,
                       Signatures,
                       CustomData,
                       null,
                       DataSource)

            {

                this.Id               = Id              ?? BlogPosting_Id.Random();
                this.Title            = Title;
                this.Teaser           = Teaser;
                this.TeaserImage      = TeaserImage;
                this.Text             = Text;
                this.Authors          = Authors is not null
                                            ? new HashSet<IUser>(Authors)
                                            : new HashSet<IUser>();
                this.PublicationDate  = PublicationDate ?? Timestamp.Now;
                this.GeoLocation      = GeoLocation;
                this.Category         = Category;
                this.Tags             = Tags is not null
                                            ? new HashSet<TagRelevance>(Tags)
                                            : new HashSet<TagRelevance>();
                this.PrivacyLevel     = PrivacyLevel    ?? org.GraphDefined.Vanaheimr.Hermod.HTTP.PrivacyLevel.Private;
                this.IsHidden         = IsHidden;

            }

            #endregion


            public BlogPosting Sign(ICipherParameters PrivateKey)
            {

                var posting     = new BlogPosting(
                                      Title,
                                      Teaser,
                                      TeaserImage,
                                      Text,
                                      Id,
                                      null,
                                      PublicationDate,
                                      LastChangeDate,
                                      GeoLocation,
                                      Category,
                                      Tags,
                                      PrivacyLevel,
                                      IsHidden,
                                      Signatures,
                                      CustomData,
                                      DataSource
                                  );

                var ctext       = posting.ToJSON(Embedded:    false,
                                                 ExpandTags:  InfoStatus.ShowIdOnly).ToString(Formatting.None);

                var SHA256Hash  = SHA256.HashData(ctext.ToUTF8Bytes());
                var signer      = SignerUtilities.GetSigner("NONEwithECDSA");
                signer.Init(true, PrivateKey);
                signer.BlockUpdate(SHA256Hash, 0, 32);

                var signature   = signer.GenerateSignature().ToHexString();
                var signatures  = new List<Signature>(Signatures) {
                                      new Signature("json", "secp256k1", "DER+HEX", signature)
                                  };

                return new BlogPosting(
                           Title,
                           Teaser,
                           TeaserImage,
                           Text,
                           Id,
                           null,
                           PublicationDate,
                           LastChangeDate,
                           GeoLocation,
                           Category,
                           Tags,
                           PrivacyLevel,
                           IsHidden,
                           signatures,
                           CustomData,
                           DataSource
                       );

            }


            public override void CopyAllLinkedDataFromBase(BlogPosting OldEnity)
            {
            }

            public override int CompareTo(object obj)
            {
                return 0;
            }

            public override bool Equals(BlogPosting? other)
            {
                throw new NotImplementedException();
            }

            public override int CompareTo(BlogPosting? other)
            {
                throw new NotImplementedException();
            }


            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the blog posting.
            /// </summary>
            /// <param name="Builder">A blog posting builder.</param>
            public static implicit operator BlogPosting(Builder Builder)

                => Builder.ToImmutable;


            /// <summary>
            /// Return an immutable version of the blog posting.
            /// </summary>
            public BlogPosting ToImmutable
            {
                get
                {

                    if (Title       is null || Title.      IsNullOrEmpty())
                        throw new ArgumentNullException(nameof(Title),       "The given title must not be null or empty!");

                    if (Teaser      is null || Teaser.     IsNullOrEmpty())
                        throw new ArgumentNullException(nameof(Teaser),      "The given teaser must not be null or empty!");

                    if (TeaserImage is null || TeaserImage.IsNullOrEmpty())
                        throw new ArgumentNullException(nameof(TeaserImage), "The given teaser image must not be null or empty!");

                    if (Text        is null || Text.       IsNullOrEmpty())
                        throw new ArgumentNullException(nameof(Text),        "The given text must not be null or empty!");

                    return new (Title,
                                Teaser,
                                TeaserImage,
                                Text,
                                Id,
                                Authors,
                                PublicationDate,
                                LastChangeDate,
                                GeoLocation,
                                Category,
                                Tags,
                                PrivacyLevel,
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
