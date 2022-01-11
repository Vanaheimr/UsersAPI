/*
 * Copyright (c) 2014-2022 GraphDefined GmbH <achim.friedland@graphdefined.com>
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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Styx.Arrows;

#endregion

namespace social.OpenData.UsersAPI
{

//    public delegate Boolea news bannerProviderDelegate(News_Id NewsBannerId, out News News);

    public delegate JObject NewsBannerToJSONDelegate(NewsBanner  NewsBanner,
                                                     Boolean     Embedded            = false,
                                                     InfoStatus  ExpandTags          = InfoStatus.ShowIdOnly,
                                                     InfoStatus  ExpandAuthorId      = InfoStatus.ShowIdOnly,
                                                     Boolean     IncludeCryptoHash   = true);


    /// <summary>
    /// Extension methods for news banners.
    /// </summary>
    public static class NewsBannerExtensions
    {

        #region ToJSON(this NewsBanner, Skip = null, Take = null, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of news banners.
        /// </summary>
        /// <param name="NewsBanner">An enumeration of news banners.</param>
        /// <param name="Skip">The optional number of news banners to skip.</param>
        /// <param name="Take">The optional number of news banners to return.</param>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        public static JArray ToJSON(this IEnumerable<NewsBanner>  NewsBanner,
                                    UInt64?                       Skip                = null,
                                    UInt64?                       Take                = null,
                                    Boolean                       Embedded            = false,
                                    InfoStatus                    ExpandTags          = InfoStatus.ShowIdOnly,
                                    InfoStatus                    ExpandAuthorId      = InfoStatus.ShowIdOnly,
                                    NewsBannerToJSONDelegate      NewsBannerToJSON    = null,
                                    Boolean                       IncludeCryptoHash   = true)


            => NewsBanner?.Any() != true

                   ? new JArray()

                   : new JArray(NewsBanner.
                                    Where         (dataSet =>  dataSet != null).
                                    //OrderByDescending(dataSet => dataSet.PublicationDate).
                                    SkipTakeFilter(Skip, Take).
                                    SafeSelect    (newsBanner => NewsBannerToJSON != null
                                                                     ? NewsBannerToJSON (newsBanner,
                                                                                         Embedded,
                                                                                         ExpandTags,
                                                                                         ExpandAuthorId,
                                                                                         IncludeCryptoHash)

                                                                     : newsBanner.ToJSON(Embedded,
                                                                                         ExpandTags,
                                                                                         ExpandAuthorId,
                                                                                         IncludeCryptoHash)));

        #endregion

    }

    /// <summary>
    /// A news banner.
    /// </summary>
    public class NewsBanner : AEntity<NewsBanner_Id,
                                      NewsBanner>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of news banner.
        /// </summary>
        public readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://opendata.social/contexts/UsersAPI/newsBanner");

        #endregion

        #region Properties

        #region API

        private Object _API;

        /// <summary>
        /// The API of this news banner.
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
                    throw new ArgumentException("Illegal attempt to change the API of this news banner banner!");

                if (value == null)
                    throw new ArgumentException("Illegal attempt to delete the API reference of this news banner banner!");

                _API = value;

            }

        }

        #endregion

        /// <summary>
        /// The (multi-language) text of the news banner.
        /// </summary>
        [Mandatory]
        public I18NString   Text                  { get; }

        /// <summary>
        /// The timestamp of the publication start of this news banner.
        /// </summary>
        [Mandatory]
        public DateTime     StartTimestamp        { get; }

        /// <summary>
        /// The timestamp of the publication end of this news banner.
        /// </summary>
        [Mandatory]
        public DateTime     EndTimestamp          { get; }

        /// <summary>
        /// The author of the news banner.
        /// </summary>
        [Mandatory]
        public User         Author                { get; }

        /// <summary>
        /// Whether the news banner is currently hidden.
        /// </summary>
        [Optional]
        public Boolean      IsHidden              { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new news banner.
        /// </summary>
        /// <param name="Text">The (multi-language) text of the news banner.</param>
        /// <param name="StartTimestamp">The timestamp of the publication start of this news banner.</param>
        /// <param name="EndTimestamp">The timestamp of the publication end of this news banner.</param>
        /// <param name="Author">The author of the news banner.</param>
        /// <param name="IsHidden">Whether the news banner is currently hidden.</param>
        public NewsBanner(I18NString  Text,
                          DateTime    StartTimestamp,
                          DateTime    EndTimestamp,
                          User        Author,
                          Boolean     IsHidden     = false,

                          JObject     CustomData   = default,
                          String      DataSource   = default,
                          DateTime?   LastChange   = default)

            : this(NewsBanner_Id.Random(),
                   Text,
                   StartTimestamp,
                   EndTimestamp,
                   Author,
                   IsHidden,

                   CustomData,
                   DataSource,
                   LastChange)

        { }


        /// <summary>
        /// Create a new news banner.
        /// </summary>
        /// <param name="Id">The unique identification of the news banner.</param>
        /// <param name="Text">The (multi-language) text of the news banner.</param>
        /// <param name="StartTimestamp">The timestamp of the publication start of this news banner.</param>
        /// <param name="EndTimestamp">The timestamp of the publication end of this news banner.</param>
        /// <param name="Author">The author of the news banner.</param>
        /// <param name="IsHidden">Whether the news banner is currently hidden.</param>
        public NewsBanner(NewsBanner_Id  Id,
                          I18NString     Text,
                          DateTime       StartTimestamp,
                          DateTime       EndTimestamp,
                          User           Author,
                          Boolean        IsHidden     = false,

                          JObject        CustomData   = default,
                          String         DataSource   = default,
                          DateTime?      LastChange   = default)

            : base(Id,
                   DefaultJSONLDContext,
                   CustomData,
                   DataSource,
                   LastChange)

        {

            if (Text.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Text), "The given (multi-language) text must not be null or empty!");

            this.Text            = Text;
            this.StartTimestamp  = StartTimestamp;
            this.EndTimestamp    = EndTimestamp;
            this.Author          = Author ?? throw new ArgumentNullException(nameof(Author), "The given author must not be null!");
            this.IsHidden        = IsHidden;

            CalcHash();

        }

        #endregion


        #region ToJSON(Embedded = false, IncludeCryptoHash = false)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        /// <param name="IncludeCryptoHash">Include the crypto hash value of this object.</param>
        public override JObject ToJSON(Boolean  Embedded            = false,
                                       Boolean  IncludeCryptoHash   = false)

            => ToJSON(Embedded:           false,
                      ExpandTags:         InfoStatus.ShowIdOnly,
                      ExpandAuthorId:     InfoStatus.ShowIdOnly,
                      IncludeCryptoHash:  true);


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

                   new JProperty("@id",             Id.            ToString()),

                   !Embedded
                       ? new JProperty("@context",  JSONLDContext. ToString())
                       : null,

                   new JProperty("text",            Text.          ToJSON()),

                   new JProperty("startTimestamp",  StartTimestamp.ToIso8601()),
                   new JProperty("endTimestamp",    EndTimestamp.  ToIso8601()),
                   new JProperty("author",          JSONObject.Create(
                                                        new JProperty("@id",  Author.Id.ToString()),
                                                        new JProperty("name", Author.Name)
                                                    ))

               );

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out NewsBanner, out ErrorResponse)

        public static Boolean TryParseJSON(JObject               JSONObject,
                                           UserProviderDelegate  UserProvider,
                                           out NewsBanner        NewsBanner,
                                           out String            ErrorResponse,
                                           NewsBanner_Id?        NewsBannerIdURL  = null)
        {

            try
            {

                NewsBanner = null;

                #region Parse NewsBannerId      [optional]

                // Verify that a given news banner identification
                //   is at least valid.
                if (JSONObject.ParseOptionalStruct("@id",
                                                   "news banner identification",
                                                   NewsBanner_Id.TryParse,
                                                   out NewsBanner_Id? NewsBannerIdBody,
                                                   out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                if (!NewsBannerIdURL.HasValue && !NewsBannerIdBody.HasValue)
                {
                    ErrorResponse = "The news banner identification is missing!";
                    return false;
                }

                if (NewsBannerIdURL.HasValue && NewsBannerIdBody.HasValue && NewsBannerIdURL.Value != NewsBannerIdBody.Value)
                {
                    ErrorResponse = "The optional news banner identification given within the JSON body does not match the one given in the URI!";
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

                #region Parse Text              [mandatory]

                if (!JSONObject.ParseMandatory("text",
                                               "news text",
                                               out I18NString Text,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse StartTimestamp    [mandatory]

                if (!JSONObject.ParseMandatory("startTimestamp",
                                               "start timestamp",
                                               out DateTime StartTimestamp,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse EndTimestamp      [mandatory]

                if (!JSONObject.ParseMandatory("endTimestamp",
                                               "end timestamp",
                                               out DateTime EndTimestamp,
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


                NewsBanner = new NewsBanner(NewsBannerIdBody ?? NewsBannerIdURL.Value,
                                            Text,
                                            StartTimestamp,
                                            EndTimestamp,
                                            Author,
                                            IsHidden ?? false,

                                            CustomData,
                                            DataSource,
                                            LastChange);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse  = e.Message;
                NewsBanner     = null;
                return false;
            }

        }

        #endregion


        #region CopyAllLinkedDataFrom(OldNewsBanner)

        public override void CopyAllLinkedDataFrom(NewsBanner OldNewsBanner)
        {

        }

        #endregion


        #region Operator overloading

        #region Operator == (NewsBannerId1, NewsBannerId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsBannerId1">A news banner identification.</param>
        /// <param name="NewsBannerId2">Another news banner identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (NewsBanner NewsBannerId1, NewsBanner NewsBannerId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(NewsBannerId1, NewsBannerId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) NewsBannerId1 == null) || ((Object) NewsBannerId2 == null))
                return false;

            return NewsBannerId1.Equals(NewsBannerId2);

        }

        #endregion

        #region Operator != (NewsBannerId1, NewsBannerId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsBannerId1">A news banner identification.</param>
        /// <param name="NewsBannerId2">Another news banner identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (NewsBanner NewsBannerId1, NewsBanner NewsBannerId2)
            => !(NewsBannerId1 == NewsBannerId2);

        #endregion

        #region Operator <  (NewsBannerId1, NewsBannerId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsBannerId1">A news banner identification.</param>
        /// <param name="NewsBannerId2">Another news banner identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (NewsBanner NewsBannerId1, NewsBanner NewsBannerId2)
        {

            if ((Object) NewsBannerId1 == null)
                throw new ArgumentNullException(nameof(NewsBannerId1), "The given NewsBannerId1 must not be null!");

            return NewsBannerId1.CompareTo(NewsBannerId2) < 0;

        }

        #endregion

        #region Operator <= (NewsBannerId1, NewsBannerId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsBannerId1">A news banner identification.</param>
        /// <param name="NewsBannerId2">Another news banner identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (NewsBanner NewsBannerId1, NewsBanner NewsBannerId2)
            => !(NewsBannerId1 > NewsBannerId2);

        #endregion

        #region Operator >  (NewsBannerId1, NewsBannerId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsBannerId1">A news banner identification.</param>
        /// <param name="NewsBannerId2">Another news banner identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (NewsBanner NewsBannerId1, NewsBanner NewsBannerId2)
        {

            if ((Object) NewsBannerId1 == null)
                throw new ArgumentNullException(nameof(NewsBannerId1), "The given NewsBannerId1 must not be null!");

            return NewsBannerId1.CompareTo(NewsBannerId2) > 0;

        }

        #endregion

        #region Operator >= (NewsBannerId1, NewsBannerId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsBannerId1">A news banner identification.</param>
        /// <param name="NewsBannerId2">Another news banner identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (NewsBanner NewsBannerId1, NewsBanner NewsBannerId2)
            => !(NewsBannerId1 < NewsBannerId2);

        #endregion

        #endregion

        #region IComparable<NewsBanner> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public override Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            var News = Object as NewsBanner;
            if ((Object) News == null)
                throw new ArgumentException("The given object is not a news banner!");

            return CompareTo(News);

        }

        #endregion

        #region CompareTo(NewsBanner)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="NewsBanner">A news banner object to compare with.</param>
        public override Int32 CompareTo(NewsBanner NewsBanner)
        {

            if ((Object) NewsBanner == null)
                throw new ArgumentNullException("The given news banner must not be null!");

            return Id.CompareTo(NewsBanner.Id);

        }

        #endregion

        #endregion

        #region IEquatable<NewsBanner> Members

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

            var News = Object as NewsBanner;
            if ((Object) News == null)
                return false;

            return Equals(News);

        }

        #endregion

        #region Equals(NewsBanner)

        /// <summary>
        /// Compares two Newss for equality.
        /// </summary>
        /// <param name="NewsBanner">A news banner to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(NewsBanner NewsBanner)
        {

            if ((Object) NewsBanner == null)
                return false;

            return Id.Equals(NewsBanner.Id);

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


        #region ToBuilder(NewNewsBannerId = null)

        /// <summary>
        /// Return a builder for this news banner.
        /// </summary>
        /// <param name="NewNewsBannerId">An optional new news banner identification.</param>
        public Builder ToBuilder(NewsBanner_Id? NewNewsBannerId = null)

            => new Builder(NewNewsBannerId ?? Id,
                           Text,
                           StartTimestamp,
                           EndTimestamp,
                           Author,
                           IsHidden,

                           CustomData,
                           DataSource,
                           LastChange);

        #endregion

        #region (class) Builder

        /// <summary>
        /// A news banner builder.
        /// </summary>
        public new class Builder : AEntity<NewsBanner_Id,
                                           NewsBanner>.Builder
        {

            #region Properties

            #region API

            private Object _API;

            /// <summary>
            /// The API of this news banner.
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
                        throw new ArgumentException("Illegal attempt to change the API of this news banner banner!");

                    if (value == null)
                        throw new ArgumentException("Illegal attempt to delete the API reference of this news banner banner!");

                    _API = value;

                }

            }

            #endregion


            /// <summary>
            /// The (multi-language) text of the news banner.
            /// </summary>
            [Mandatory]
            public I18NString   Text                  { get; set; }

            /// <summary>
            /// The timestamp of the publication start of this news banner.
            /// </summary>
            [Mandatory]
            public DateTime?    StartTimestamp        { get; set; }

            /// <summary>
            /// The timestamp of the publication end of this news banner.
            /// </summary>
            [Mandatory]
            public DateTime?    EndTimestamp          { get; set; }

            /// <summary>
            /// The author of the news banner.
            /// </summary>
            [Mandatory]
            public User         Author                { get; set; }

            /// <summary>
            /// Whether the news banner is currently hidden.
            /// </summary>
            [Optional]
            public Boolean      IsHidden              { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new news banner builder.
            /// </summary>
            /// <param name="Id">The unique identification of the news banner.</param>
            /// <param name="Text">The (multi-language) text of the news banner.</param>
            /// <param name="StartTimestamp">The timestamp of the publication start of this news banner.</param>
            /// <param name="EndTimestamp">The timestamp of the publication end of this news banner.</param>
            /// <param name="Author">The author of the news banner.</param>
            /// <param name="IsHidden">Whether the news banner is currently hidden.</param>
            public Builder(NewsBanner_Id?  Id               = null,
                           I18NString      Text             = null,
                           DateTime?       StartTimestamp   = null,
                           DateTime?       EndTimestamp     = null,
                           User            Author           = null,
                           Boolean         IsHidden         = false,

                           JObject         CustomData       = default,
                           String          DataSource       = default,
                           DateTime?       LastChange       = default)

                : base(Id ?? NewsBanner_Id.Random(),
                       DefaultJSONLDContext,
                       CustomData,
                       DataSource,
                       LastChange)

            {

                this.Text            = Text;
                this.StartTimestamp  = StartTimestamp;
                this.EndTimestamp    = EndTimestamp;
                this.Author          = Author;
                this.IsHidden        = IsHidden;

            }

            #endregion



            public override void CopyAllLinkedDataFrom(NewsBanner OldEnity)
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
            public static implicit operator NewsBanner(Builder Builder)

                => Builder?.ToImmutable;


            /// <summary>
            /// Return an immutable version of the News.
            /// </summary>
            public NewsBanner ToImmutable

                => new NewsBanner(Id,
                                  Text,
                                  StartTimestamp ?? Timestamp.Now,
                                  EndTimestamp   ?? Timestamp.Now + TimeSpan.FromDays(14),
                                  Author,
                                  IsHidden,

                                  CustomData,
                                  DataSource,
                                  LastChange);

            #endregion

        }

        #endregion

    }

}
