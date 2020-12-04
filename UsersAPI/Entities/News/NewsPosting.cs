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
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;

using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Styx.Arrows;

using System.Security.Cryptography;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;

using social.OpenData.UsersAPI;

#endregion

namespace social.OpenData.UsersAPI
{

//    public delegate Boolean NewsProviderDelegate(News_Id NewsId, out News News);

    public delegate JObject NewsToJSONDelegate(NewsPosting  NewsPosting,
                                               Boolean      Embedded            = false,
                                               InfoStatus   ExpandTags          = InfoStatus.ShowIdOnly,
                                               InfoStatus   ExpandDataLicenes   = InfoStatus.ShowIdOnly,
                                               InfoStatus   ExpandOwnerId       = InfoStatus.ShowIdOnly,
                                               Boolean      IncludeCryptoHash   = true);


    /// <summary>
    /// Extention methods for news postings.
    /// </summary>
    public static class NewsExtentions
    {

        #region ToJSON(this News, Skip = null, Take = null, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of newss.
        /// </summary>
        /// <param name="News">An enumeration of newss.</param>
        /// <param name="Skip">The optional number of newss to skip.</param>
        /// <param name="Take">The optional number of newss to return.</param>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        public static JArray ToJSON(this IEnumerable<NewsPosting>  News,
                                    UInt64?                 Skip                 = null,
                                    UInt64?                 Take                 = null,
                                    Boolean                 Embedded             = false,
                                    InfoStatus              ExpandTags           = InfoStatus.ShowIdOnly,
                                    InfoStatus              ExpandDataLicenses   = InfoStatus.ShowIdOnly,
                                    InfoStatus              ExpandOwnerId        = InfoStatus.ShowIdOnly,
                                    NewsToJSONDelegate      NewsToJSON           = null,
                                    Boolean                 IncludeCryptoHash    = true)


            => News?.Any() != true

                   ? new JArray()

                   : new JArray(News.
                                    Where            (dataSet =>  dataSet != null).
                                    //OrderByDescending(dataSet => dataSet.PublicationDate).
                                    SkipTakeFilter   (Skip, Take).
                                    SafeSelect       (news    => NewsToJSON != null
                                                                     ? NewsToJSON (news,
                                                                                   Embedded,
                                                                                   ExpandTags,
                                                                                   ExpandDataLicenses,
                                                                                   ExpandOwnerId,
                                                                                   IncludeCryptoHash)

                                                                     : news.ToJSON(Embedded,
                                                                                   ExpandTags,
                                                                                   ExpandDataLicenses,
                                                                                   ExpandOwnerId,
                                                                                   IncludeCryptoHash)));

        #endregion

        #region GeoFilter(this News, SearchCenter, RadiusKM, Skip = null, Take = null, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of News.
        /// </summary>
        /// <param name="News">An enumeration of News.</param>
        /// <param name="SearchCenter">The search center.</param>
        /// <param name="RadiusKM">The search radius.</param>
        /// <param name="Skip">The optional number of News to skip.</param>
        /// <param name="Take">The optional number of News to return.</param>
        public static IEnumerable<WithDistance<NewsPosting>> GeoFilter(this IEnumerable<NewsPosting>  News,
                                                                GeoCoordinate           SearchCenter,
                                                                Double                  RadiusKM,
                                                                UInt64?                 Skip   = null,
                                                                UInt64?                 Take   = null)

        {

            if (News == null || !News.Any())
                return new WithDistance<NewsPosting>[0];

            var results = new List<WithDistance<NewsPosting>>();

            foreach (var news in News)
            {

                if (!news.GeoLocations.SafeAny())
                    continue;

                var nearrest = news.GeoLocations.Select (geo  => new WithDistance<NewsPosting>(news,
                                                                                        geo.DistanceKM(SearchCenter),
                                                                                        DistanceMetricTypes.air)).
                                                        OrderBy(geod => geod.Distance).
                                                        FirstOrDefault();

                if (nearrest.Distance <= RadiusKM)
                    results.Add(nearrest);

            }

            return results.SkipTakeFilter(Skip, Take);

        }

        #endregion

        #region ToJSON(this NewsWithDistance, Skip = null, Take = null, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of News.
        /// </summary>
        /// <param name="NewsWithDistance<News>">An enumeration of News.</param>
        /// <param name="Skip">The optional number of News to skip.</param>
        /// <param name="Take">The optional number of News to return.</param>
        /// <param name="Embedded">Whether this data is embedded into another data structure, e.g. into a News.</param>
        public static JArray ToJSON(this IEnumerable<WithDistance<NewsPosting>>  NewsWithDistance,
                                    UInt64?                               Skip                 = null,
                                    UInt64?                               Take                 = null,
                                    Boolean                               Embedded             = false,
                                    InfoStatus                            ExpandTags           = InfoStatus.ShowIdOnly,
                                    InfoStatus                            ExpandDataLicenses   = InfoStatus.ShowIdOnly,
                                    InfoStatus                            ExpandOwnerId        = InfoStatus.ShowIdOnly,
                                    NewsToJSONDelegate                    NewsToJSON           = null,
                                    Boolean                               IncludeCryptoHash    = true)


            => NewsWithDistance == null || !NewsWithDistance.Any()

                   ? new JArray()

                   : new JArray(NewsWithDistance.
                                    SkipTakeFilter(Skip, Take).
                                    SafeSelect    (newsWithDistance => new JObject(
                                                                                    new JProperty("distance",        newsWithDistance.Distance),
                                                                                    new JProperty("distanceMetric",  newsWithDistance.DistanceMetric.ToString()),
                                                                                    new JProperty("News",            newsWithDistance.
                                                                                                                                  Element.
                                                                                                                                  ToJSON(Embedded,
                                                                                                                                         ExpandTags,
                                                                                                                                         ExpandDataLicenses,
                                                                                                                                         ExpandOwnerId,
                                                                                                                                         IncludeCryptoHash))
                                                                                )));

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


        public Organization                       Owner                 { get; }

        /// <summary>
        /// The (multi-language) headline of this news.
        /// </summary>
        [Mandatory]
        public I18NString                         Headline              { get; }

        /// <summary>
        /// The (multi-language) text of this news.
        /// </summary>
        [Mandatory]
        public I18NString                         Text                  { get; }

        /// <summary>
        /// The timestamp of the publication of this news.
        /// </summary>
        [Mandatory]
        public DateTime                           PublicationDate       { get; }

        /// <summary>
        /// Optional geographical locations of/for this news.
        /// </summary>
        [Optional]
        public IEnumerable<GeoCoordinate>         GeoLocations          { get; }

        /// <summary>
        /// An enumeration of multi-language tags and their relevance.
        /// </summary>
        [Optional]
        public IEnumerable<TagRelevance>          Tags                  { get; }


        /// <summary>
        /// The news is hidden.
        /// </summary>
        [Optional]
        public Boolean                            IsHidden              { get; }


        /// <summary>
        /// All signatures of this news.
        /// </summary>
        public IEnumerable<Signature>             Signatures            { get; }

        #endregion

        #region Events

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new news.
        /// </summary>
        /// <param name="Text">The (multi-language) text of this news.</param>
        /// <param name="PublicationDate">The timestamp of the publication of this news.</param>
        /// <param name="GeoLocation">An optional geographical location of this news.</param>
        /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
        /// <param name="IsHidden">The news is hidden.</param>
        /// <param name="Signatures">All signatures of this news.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        public NewsPosting(I18NString                  Headline,
                    I18NString                  Text,
                    DateTime?                   PublicationDate   = null,
                    IEnumerable<GeoCoordinate>  GeoLocations      = null,
                    IEnumerable<TagRelevance>   Tags              = null,

                    IEnumerable<Signature>      Signatures        = null,
                    String                      DataSource        = "")

            : this(NewsPosting_Id.Random(),
                   Headline,
                   Text,
                   PublicationDate,
                   GeoLocations,
                   Tags,
                   Signatures,
                   DataSource)

        { }


        /// <summary>
        /// Create a new Open Data news.
        /// </summary>
        /// <param name="Id">The unique identification of this news.</param>
        /// <param name="Text">The (multi-language) text of this news.</param>
        /// <param name="PublicationDate">The timestamp of the publication of this news.</param>
        /// <param name="GeoLocation">An optional geographical location of this news.</param>
        /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
        /// <param name="IsHidden">The news is hidden.</param>
        /// <param name="Signatures">All signatures of this news.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        public NewsPosting(NewsPosting_Id                     Id,
                    I18NString                  Headline,
                    I18NString                  Text,
                    DateTime?                   PublicationDate   = null,
                    IEnumerable<GeoCoordinate>  GeoLocations      = null,
                    IEnumerable<TagRelevance>   Tags              = null,

                    IEnumerable<Signature>      Signatures        = null,
                    String                      DataSource        = "")

            : base(Id,
                   DefaultJSONLDContext,
                   null,
                   DataSource)

        {

            this.Headline         = Headline        ?? throw new ArgumentNullException(nameof(Headline), "The given headline must not be null!");
            this.Text             = Text            ?? throw new ArgumentNullException(nameof(Text),     "The given text must not be null!");
            this.PublicationDate  = PublicationDate ?? DateTime.Now;
            this.GeoLocations     = GeoLocations    ?? new GeoCoordinate[0];
            this.Tags             = Tags            ?? new TagRelevance[0];

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
        public JObject ToJSON(Boolean     Embedded             = false,
                              InfoStatus  ExpandTags           = InfoStatus.ShowIdOnly,
                              InfoStatus  ExpandDataLicenses   = InfoStatus.ShowIdOnly,
                              InfoStatus  ExpandOwnerId        = InfoStatus.ShowIdOnly,
                              Boolean     IncludeCryptoHash    = false)

            => JSONObject.Create(

                   new JProperty("@id",                  Id.ToString()),

                   !Embedded
                       ? new JProperty("@context",       JSONLDContext.ToString())
                       : null,

                   Headline.IsNeitherNullNorEmpty()
                       ? new JProperty("headline",       Headline.ToJSON())
                       : null,

                   Text.IsNeitherNullNorEmpty()
                       ? new JProperty("text",           Text.ToJSON())
                       : null,

                   new JProperty("publicationDate",      PublicationDate.ToIso8601()),

                   GeoLocations.SafeAny()
                       ? new JProperty("geoLocations",   new JArray(GeoLocations.Select(geolocation => geolocation.ToJSON())))
                       : null,

                   Tags.Any()
                       ? new JProperty("tags",           Tags.SafeSelect(tag => tag.ToJSON(ExpandTags)))
                       : null,

                   Signatures.Any()
                       ? new JProperty("signatures",     new JArray(Signatures.SafeSelect(signature => signature.ToJSON(Embedded: true))))
                       : null

               );

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out News, out ErrorResponse)

        public static Boolean TryParseJSON(JObject     JSONObject,
                                           out NewsPosting    News,
                                           out String  ErrorResponse,
                                           NewsPosting_Id?    NewsIdURL  = null)
        {

            try
            {

                News = null;

                #region Parse NewsId           [optional]

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

                #region Parse Context          [mandatory]

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

                #region Parse Headline         [mandatory]

                if (!JSONObject.ParseMandatory("headline",
                                               "news headline",
                                               out I18NString Headline,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Text             [mandatory]

                if (!JSONObject.ParseMandatory("text",
                                               "news text",
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

                var GeoLocations     = new GeoCoordinate[0];
                var Tags             = new TagRelevance[0];
                var Signatures       = new Signature[0];

                #region Get   DataSource       [optional]

                var DataSource = JSONObject.GetOptional("dataSource");

                #endregion

                #region Parse CryptoHash       [optional]

                var CryptoHash    = JSONObject.GetOptional("cryptoHash");

                #endregion


                News = new NewsPosting(NewsIdBody ?? NewsIdURL.Value,
                                       Headline,
                                       Text,
                                       PublicationDate,
                                       GeoLocations,
                                       Tags,
                                       Signatures,
                                       DataSource);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse  = e.Message;
                News           = null;
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


        #region ToBuilder(NewNewsId = null)

        /// <summary>
        /// Return a builder for this News.
        /// </summary>
        /// <param name="NewNewsId">An optional new News identification.</param>
        public Builder ToBuilder(NewsPosting_Id? NewNewsId = null)

            => new Builder(NewNewsId ?? Id,
                           Headline,
                           Text,
                           PublicationDate,
                           GeoLocations,
                           Tags,
                           Signatures,
                           DataSource);

        #endregion

        #region (class) Builder

        /// <summary>
        /// A news builder.
        /// </summary>
        public class Builder
        {

            #region Properties

            /// <summary>
            /// The unique identification of this news.
            /// </summary>
            public NewsPosting_Id                            Id                    { get; set; }

            /// <summary>
            /// The (multi-language) headline of this news.
            /// </summary>
            [Mandatory]
            public I18NString                         Headline              { get; set; }

            /// <summary>
            /// The (multi-language) text of this news.
            /// </summary>
            [Mandatory]
            public I18NString                         Text                  { get; set; }

            /// <summary>
            /// The timestamp of the publication of this news.
            /// </summary>
            [Mandatory]
            public DateTime                           PublicationDate       { get; set; }

            /// <summary>
            /// Optional geographical locations of/for this news.
            /// </summary>
            [Optional]
            public List<GeoCoordinate>                GeoLocations          { get; set; }

            /// <summary>
            /// An enumeration of multi-language tags and their relevance.
            /// </summary>
            [Optional]
            public List<TagRelevance>                 Tags                  { get; set; }

            /// <summary>
            /// All signatures of this news.
            /// </summary>
            public List<Signature>                    Signatures            { get; set; }

            /// <summary>
            /// The source of this information, e.g. an automatic importer.
            /// </summary>
            [Optional]
            public String                             DataSource            { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new news builder.
            /// </summary>
            /// <param name="Text">The (multi-language) text of this news.</param>
            /// <param name="PublicationDate">The timestamp of the publication of this news.</param>
            /// <param name="GeoLocation">An optional geographical location of this news.</param>
            /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
            /// <param name="PrivacyLevel">Whether the news will be shown in news listings, or not.</param>
            /// <param name="IsHidden">The news is hidden.</param>
            /// <param name="Signatures">All signatures of this news.</param>
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            public Builder(I18NString                  Headline, 
                           I18NString                  Text,
                           DateTime?                   PublicationDate   = null,
                           IEnumerable<GeoCoordinate>  GeoLocations      = null,
                           IEnumerable<TagRelevance>   Tags              = null,
                           Boolean                     IsHidden          = false,
                           IEnumerable<Signature>      Signatures        = null,
                           String                      DataSource        = "")

                : this(NewsPosting_Id.Random(),
                       Headline,
                       Text,
                       PublicationDate,
                       GeoLocations,
                       Tags,
                       Signatures,
                       DataSource)

            { }


            /// <summary>
            /// Create a new News builder.
            /// </summary>
            /// <param name="Id">The unique identification of this news.</param>
            /// <param name="Text">The (multi-language) text of this news.</param>
            /// <param name="PublicationDate">The timestamp of the publication of this news.</param>
            /// <param name="GeoLocation">An optional geographical location of this news.</param>
            /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
            /// <param name="PrivacyLevel">Whether the news will be shown in news listings, or not.</param>
            /// <param name="IsHidden">The news is hidden.</param>
            /// <param name="Signatures">All signatures of this news.</param>
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            public Builder(NewsPosting_Id                     Id,
                           I18NString                  Headline,
                           I18NString                  Text,
                           DateTime?                   PublicationDate   = null,
                           IEnumerable<GeoCoordinate>  GeoLocation       = null,
                           IEnumerable<TagRelevance>   Tags              = null,
                           IEnumerable<Signature>      Signatures        = null,
                           String                      DataSource        = "")
            {

                this.Id               = Id;
                this.Headline         = Headline        ?? throw new ArgumentNullException(nameof(Headline), "The given headline must not be null!");
                this.Text             = Text            ?? throw new ArgumentNullException(nameof(Text),     "The given text must not be null!");
                this.PublicationDate  = PublicationDate ?? DateTime.Now;
                this.GeoLocations     = GeoLocations != null
                                            ? new List<GeoCoordinate>(GeoLocations)
                                            : new List<GeoCoordinate>();
                this.Tags             = Tags != null
                                            ? new List<TagRelevance>(Tags)
                                            : new List<TagRelevance>();
                this.Signatures       = Signatures != null
                                            ? new List<Signature>(Signatures)
                                            : new List<Signature>();
                this.DataSource       = DataSource;

            }

            #endregion


            public NewsPosting Sign(ICipherParameters PrivateKey)
            {

                var news        = new NewsPosting(Id,
                                           Headline,
                                           Text,
                                           PublicationDate,
                                           GeoLocations,
                                           Tags,
                                           Signatures,
                                           DataSource);

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
                                PublicationDate,
                                GeoLocations,
                                Tags,
                                signatures,
                                DataSource);

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
                            PublicationDate,
                            GeoLocations,
                            Tags,
                            Signatures,
                            DataSource);

            #endregion

        }

        #endregion

    }

}
