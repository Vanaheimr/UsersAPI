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

//    public delegate Boolean FAQProviderDelegate(FAQ_Id FAQId, out FAQ FAQ);

    public delegate JObject FAQToJSONDelegate(FAQ         FAQ,
                                              Boolean     Embedded         = false,
                                              InfoStatus  ExpandTags       = InfoStatus.ShowIdOnly,
                                              InfoStatus  ExpandAuthorId   = InfoStatus.ShowIdOnly);


    /// <summary>
    /// Extension methods for FAQs.
    /// </summary>
    public static class FAQExtensions
    {

        #region ToJSON(this FAQ, Skip = null, Take = null, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of FAQs.
        /// </summary>
        /// <param name="FAQ">An enumeration of FAQs.</param>
        /// <param name="Skip">The optional number of FAQs to skip.</param>
        /// <param name="Take">The optional number of FAQs to return.</param>
        /// <param name="Embedded">Whether this data structure is embedded into another data structure.</param>
        public static JArray ToJSON(this IEnumerable<FAQ>  FAQ,
                                    UInt64?                Skip             = null,
                                    UInt64?                Take             = null,
                                    Boolean                Embedded         = false,
                                    InfoStatus             ExpandTags       = InfoStatus.ShowIdOnly,
                                    InfoStatus             ExpandAuthorId   = InfoStatus.ShowIdOnly,
                                    FAQToJSONDelegate?     FAQToJSON        = null)


            => FAQ?.Any() != true

                   ? new JArray()

                   : new JArray(FAQ.
                                    Where            (dataSet =>  dataSet is not null).
                                    //OrderByDescending(dataSet => dataSet.PublicationDate).
                                    SkipTakeFilter   (Skip, Take).
                                    SafeSelect       (faq     => FAQToJSON is not null
                                                                     ? FAQToJSON (faq,
                                                                                  Embedded,
                                                                                  ExpandTags,
                                                                                  ExpandAuthorId)
                                                                     : faq.ToJSON(Embedded,
                                                                                  ExpandTags,
                                                                                  ExpandAuthorId)));

        #endregion

    }

    /// <summary>
    /// A FAQ.
    /// </summary>
    public class FAQ : AEntity<FAQ_Id,
                               FAQ>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of FAQs.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://opendata.social/contexts/CardiCloudAPI/FAQ");

        #endregion

        #region Properties

        #region API

        private Object _API;

        /// <summary>
        /// The FAQsAPI of this FAQ.
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
        /// The (multi-language) question of this FAQ.
        /// </summary>
        [Mandatory]
        public I18NString                         Question              { get; }

        /// <summary>
        /// The (multi-language) answer of this FAQ.
        /// </summary>
        [Mandatory]
        public I18NString                         Answer                { get; }

        /// <summary>
        /// The author of the news banner.
        /// </summary>
        [Mandatory]
        public User                               Author                { get; }

        /// <summary>
        /// The timestamp of the publication of this FAQ.
        /// </summary>
        [Mandatory]
        public DateTime                           PublicationDate       { get; }

        /// <summary>
        /// An enumeration of multi-language tags and their relevance.
        /// </summary>
        [Optional]
        public IEnumerable<TagRelevance>          Tags                  { get; }

        /// <summary>
        /// The FAQ is hidden.
        /// </summary>
        [Optional]
        public Boolean                            IsHidden              { get; }

        /// <summary>
        /// All signatures of this FAQ.
        /// </summary>
        public IEnumerable<Signature>             Signatures            { get; }

        #endregion

        #region Events

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new FAQ.
        /// </summary>
        /// <param name="Answer">The (multi-language) text of this FAQ.</param>
        /// <param name="PublicationDate">The timestamp of the publication of this FAQ.</param>
        /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
        /// <param name="IsHidden">The FAQ is hidden.</param>
        /// <param name="Signatures">All signatures of this FAQ.</param>
        public FAQ(I18NString                  Question,
                   I18NString                  Answer,
                   User                        Author,
                   DateTime?                   PublicationDate   = null,
                   IEnumerable<TagRelevance>   Tags              = null,
                   Boolean                     IsHidden          = false,

                   IEnumerable<Signature>      Signatures        = null,

                   JObject                     CustomData        = default,
                   String                      DataSource        = default,
                   DateTime?                   LastChange        = default)

            : this(FAQ_Id.Random(),
                   Question,
                   Answer,
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
        /// Create a new Open Data FAQ.
        /// </summary>
        /// <param name="Id">The unique identification of this FAQ.</param>
        /// <param name="Answer">The (multi-language) text of this FAQ.</param>
        /// <param name="PublicationDate">The timestamp of the publication of this FAQ.</param>
        /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
        /// <param name="IsHidden">The FAQ is hidden.</param>
        /// <param name="Signatures">All signatures of this FAQ.</param>
        public FAQ(FAQ_Id                      Id,
                   I18NString                  Question,
                   I18NString                  Answer,
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
                   LastChange,
                   null,
                   CustomData,
                   DataSource)

        {

            this.Question         = Question        ?? throw new ArgumentNullException(nameof(Question),  "The given question must not be null!");
            this.Answer           = Answer          ?? throw new ArgumentNullException(nameof(Answer),    "The given answer must not be null!");
            this.Author           = Author          ?? throw new ArgumentNullException(nameof(Author),    "The given author must not be null!");
            this.PublicationDate  = PublicationDate ?? DateTime.Now;
            this.Tags             = Tags            ?? new TagRelevance[0];
            this.IsHidden         = IsHidden;

            this.Signatures       = Signatures      ?? new Signature[0];

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

                   new JProperty("@id",                  Id.           ToString()),

                   !Embedded
                       ? new JProperty("@context",       JSONLDContext.ToString())
                       : null,

                   new JProperty("question",             Question.     ToJSON()),
                   new JProperty("answer",               Answer.       ToJSON()),

                   new JProperty("author",               JSONObject.Create(
                                                             new JProperty("@id",  Author.Id.ToString()),
                                                             new JProperty("name", Author.Name)
                                                         )),

                   new JProperty("publicationDate",      PublicationDate.ToIso8601()),

                   Tags.Any()
                       ? new JProperty("tags",           Tags.SafeSelect(tag => tag.ToJSON(ExpandTags)))
                       : null,

                   Signatures.Any()
                       ? new JProperty("signatures",     new JArray(Signatures.SafeSelect(signature => signature.ToJSON(Embedded: true))))
                       : null

               );

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out FAQ, out ErrorResponse)

        public static Boolean TryParseJSON(JObject               JSONObject,
                                           UserProviderDelegate  UserProvider,
                                           out FAQ?              FAQ,
                                           out String?           ErrorResponse,
                                           FAQ_Id?               FAQIdURL  = null)
        {

            try
            {

                FAQ = null;

                #region Parse FAQId             [optional]

                // Verify that a given FAQ identification
                //   is at least valid.
                if (JSONObject.ParseOptional("@id",
                                             "FAQ identification",
                                             FAQ_Id.TryParse,
                                             out FAQ_Id? FAQIdBody,
                                             out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                if (!FAQIdURL.HasValue && !FAQIdBody.HasValue)
                {
                    ErrorResponse = "The FAQ identification is missing!";
                    return false;
                }

                if (FAQIdURL.HasValue && FAQIdBody.HasValue && FAQIdURL.Value != FAQIdBody.Value)
                {
                    ErrorResponse = "The optional FAQ identification given within the JSON body does not match the one given in the URI!";
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

                #region Parse Question          [mandatory]

                if (!JSONObject.ParseMandatory("question",
                                               "FAQ headline",
                                               out I18NString Question,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Answer            [mandatory]

                if (!JSONObject.ParseMandatory("answer",
                                               "FAQ text",
                                               out I18NString Answer,
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

                    if (ErrorResponse is not null)
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

                    if (ErrorResponse is not null)
                        return false;

                }

                #endregion

                #region Parse CryptoHash        [optional]

                var CryptoHash    = JSONObject.GetOptional("cryptoHash");

                #endregion


                FAQ = new FAQ(FAQIdBody ?? FAQIdURL.Value,
                              Question,
                              Answer,
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
                FAQ            = null;
                return false;
            }

        }

        #endregion


        #region CopyAllLinkedDataFrom(OldFAQ)

        public override void CopyAllLinkedDataFrom(FAQ OldFAQ)
        {

        }

        #endregion


        #region Operator overloading

        #region Operator == (FAQId1, FAQId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FAQId1">A FAQ identification.</param>
        /// <param name="FAQId2">Another FAQ identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (FAQ FAQId1, FAQ FAQId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(FAQId1, FAQId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) FAQId1 == null) || ((Object) FAQId2 == null))
                return false;

            return FAQId1.Equals(FAQId2);

        }

        #endregion

        #region Operator != (FAQId1, FAQId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FAQId1">A FAQ identification.</param>
        /// <param name="FAQId2">Another FAQ identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (FAQ FAQId1, FAQ FAQId2)
            => !(FAQId1 == FAQId2);

        #endregion

        #region Operator <  (FAQId1, FAQId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FAQId1">A FAQ identification.</param>
        /// <param name="FAQId2">Another FAQ identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (FAQ FAQId1, FAQ FAQId2)
        {

            if ((Object) FAQId1 == null)
                throw new ArgumentNullException(nameof(FAQId1), "The given FAQId1 must not be null!");

            return FAQId1.CompareTo(FAQId2) < 0;

        }

        #endregion

        #region Operator <= (FAQId1, FAQId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FAQId1">A FAQ identification.</param>
        /// <param name="FAQId2">Another FAQ identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (FAQ FAQId1, FAQ FAQId2)
            => !(FAQId1 > FAQId2);

        #endregion

        #region Operator >  (FAQId1, FAQId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FAQId1">A FAQ identification.</param>
        /// <param name="FAQId2">Another FAQ identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (FAQ FAQId1, FAQ FAQId2)
        {

            if ((Object) FAQId1 == null)
                throw new ArgumentNullException(nameof(FAQId1), "The given FAQId1 must not be null!");

            return FAQId1.CompareTo(FAQId2) > 0;

        }

        #endregion

        #region Operator >= (FAQId1, FAQId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FAQId1">A FAQ identification.</param>
        /// <param name="FAQId2">Another FAQ identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (FAQ FAQId1, FAQ FAQId2)
            => !(FAQId1 < FAQId2);

        #endregion

        #endregion

        #region IComparable<FAQ> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public override Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            var FAQ = Object as FAQ;
            if ((Object) FAQ == null)
                throw new ArgumentException("The given object is not an FAQ!");

            return CompareTo(FAQ);

        }

        #endregion

        #region CompareTo(FAQ)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="FAQ">An FAQ object to compare with.</param>
        public override Int32 CompareTo(FAQ FAQ)
        {

            if ((Object) FAQ == null)
                throw new ArgumentNullException("The given FAQ must not be null!");

            return Id.CompareTo(FAQ.Id);

        }

        #endregion

        #endregion

        #region IEquatable<FAQ> Members

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

            var FAQ = Object as FAQ;
            if ((Object) FAQ == null)
                return false;

            return Equals(FAQ);

        }

        #endregion

        #region Equals(FAQ)

        /// <summary>
        /// Compares two FAQs for equality.
        /// </summary>
        /// <param name="FAQ">An FAQ to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(FAQ FAQ)
        {

            if ((Object) FAQ == null)
                return false;

            return Id.Equals(FAQ.Id);

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


        #region ToBuilder(NewFAQId = null)

        /// <summary>
        /// Return a builder for this FAQ.
        /// </summary>
        /// <param name="NewFAQId">An optional new FAQ identification.</param>
        public Builder ToBuilder(FAQ_Id? NewFAQId = null)

            => new Builder(NewFAQId ?? Id,
                           Question,
                           Answer,
                           Author,
                           PublicationDate,
                           Tags,
                           IsHidden,
                           Signatures,

                           CustomData,
                           DataSource,
                           LastChangeDate);

        #endregion

        #region (class) Builder

        /// <summary>
        /// A FAQ builder.
        /// </summary>
        public new class Builder : AEntity<FAQ_Id,
                                           FAQ>.Builder
        {

            #region Properties

            /// <summary>
            /// The (multi-language) headline of this FAQ.
            /// </summary>
            [Mandatory]
            public I18NString                         Question              { get; set; }

            /// <summary>
            /// The (multi-language) text of this FAQ.
            /// </summary>
            [Mandatory]
            public I18NString                         Answer                { get; set; }

            /// <summary>
            /// The author of this FAQ.
            /// </summary>
            [Mandatory]
            public User                               Author                { get; set; }

            /// <summary>
            /// The timestamp of the publication of this FAQ.
            /// </summary>
            [Mandatory]
            public DateTime?                          PublicationDate       { get; set; }

            /// <summary>
            /// An enumeration of multi-language tags and their relevance.
            /// </summary>
            [Optional]
            public List<TagRelevance>                 Tags                  { get; set; }

            /// <summary>
            /// The FAQ is hidden.
            /// </summary>
            [Optional]
            public Boolean                            IsHidden              { get; set; }

            /// <summary>
            /// All signatures of this FAQ.
            /// </summary>
            public List<Signature>                    Signatures            { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new FAQ builder.
            /// </summary>
            /// <param name="Id">The unique identification of this FAQ.</param>
            /// <param name="Answer">The (multi-language) text of this FAQ.</param>
            /// <param name="PublicationDate">The timestamp of the publication of this FAQ.</param>
            /// <param name="GeoLocation">An optional geographical location of this FAQ.</param>
            /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
            /// <param name="IsHidden">The FAQ is hidden.</param>
            /// <param name="Signatures">All signatures of this FAQ.</param>
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            public Builder(FAQ_Id?                    Id                = null,
                           I18NString                 Question          = null,
                           I18NString                 Answer            = null,
                           User                       Author            = null,
                           DateTime?                  PublicationDate   = null,
                           IEnumerable<TagRelevance>  Tags              = null,
                           Boolean                    IsHidden          = false,
                           IEnumerable<Signature>     Signatures        = null,

                           JObject                    CustomData        = default,
                           String                     DataSource        = default,
                           DateTime?                  LastChange        = default)

                : base(Id ?? FAQ_Id.Random(),
                       DefaultJSONLDContext,
                       LastChange,
                       null,
                       CustomData,
                       DataSource)

            {

                this.Id               = Id ?? FAQ_Id.Random();
                this.Question         = Question;
                this.Answer           = Answer;
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


            public FAQ Sign(ICipherParameters PrivateKey)
            {

                var FAQ        = new FAQ(Id,
                                         Question,
                                         Answer,
                                         Author,
                                         PublicationDate,
                                         Tags,
                                         IsHidden,
                                         Signatures,

                                         CustomData,
                                         DataSource,
                                         LastChangeDate);;

                var ctext       = FAQ.ToJSON  (Embedded:   false,
                                               ExpandTags: InfoStatus.ShowIdOnly).
                                      ToString(Newtonsoft.Json.Formatting.None);

                var BlockSize   = 32;

                var SHA256      = new SHA256Managed();
                var SHA256Hash  = SHA256.ComputeHash(ctext.ToUTF8Bytes());
                var signer      = SignerUtilities.GetSigner("NONEwithECDSA");
                signer.Init(true, PrivateKey);
                signer.BlockUpdate(SHA256Hash, 0, BlockSize);

                var signature   = signer.GenerateSignature().ToHexString();
                var signatures  = new List<Signature>(Signatures);
                signatures.Add(new Signature("json", "secp256k1", "DER+HEX", signature));

                return new FAQ(Id,
                               Question,
                               Answer,
                               Author,
                               PublicationDate,
                               Tags,
                               IsHidden,
                               signatures,

                               CustomData,
                               DataSource,
                               LastChangeDate);

            }


            public override void CopyAllLinkedDataFrom(FAQ OldEnity)
            {
            }

            public override int CompareTo(object obj)
            {
                return 0;
            }


            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the FAQ.
            /// </summary>
            /// <param name="Builder">A FAQ builder.</param>
            public static implicit operator FAQ(Builder Builder)

                => Builder?.ToImmutable;


            /// <summary>
            /// Return an immutable version of the FAQ.
            /// </summary>
            public FAQ ToImmutable

                => new FAQ(Id,
                           Question,
                           Answer,
                           Author,
                           PublicationDate,
                           Tags,
                           IsHidden,
                           Signatures,

                           CustomData,
                           DataSource,
                           LastChangeDate);

            #endregion

        }

        #endregion

    }

}
