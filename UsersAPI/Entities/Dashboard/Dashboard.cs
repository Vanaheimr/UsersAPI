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

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Styx.Arrows;

#endregion

namespace social.OpenData.UsersAPI
{

//    public delegate Boolean DashboardProviderDelegate(Dashboard_Id DashboardId, out Dashboard Dashboard);

    public delegate JObject DashboardToJSONDelegate(Dashboard   Dashboard,
                                                    Boolean     Embedded     = false,
                                                    InfoStatus  ExpandTags   = InfoStatus.ShowIdOnly);


    /// <summary>
    /// Extension methods for dashboards.
    /// </summary>
    public static class DashboardExtensions
    {

        #region ToJSON(this Dashboard, Skip = null, Take = null, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of dashboards.
        /// </summary>
        /// <param name="Dashboard">An enumeration of dashboards.</param>
        /// <param name="Skip">The optional number of dashboards to skip.</param>
        /// <param name="Take">The optional number of dashboards to return.</param>
        /// <param name="Embedded">Whether this data structure is embedded into another data structure.</param>
        public static JArray ToJSON(this IEnumerable<Dashboard>  Dashboard,
                                    UInt64?                      Skip              = null,
                                    UInt64?                      Take              = null,
                                    Boolean                      Embedded          = false,
                                    InfoStatus                   ExpandTags        = InfoStatus.ShowIdOnly,
                                    DashboardToJSONDelegate?     DashboardToJSON   = null)


            => Dashboard?.Any() != true

                   ? new JArray()

                   : new JArray(Dashboard.
                                    Where            (dashboard => dashboard is not null).
                                    OrderByDescending(dashboard => dashboard.CreationDate).
                                    SkipTakeFilter   (Skip, Take).
                                    SafeSelect       (dashboard => DashboardToJSON is not null
                                                                       ? DashboardToJSON (dashboard,
                                                                                          Embedded,
                                                                                          ExpandTags)
                                                                       : dashboard.ToJSON(Embedded,
                                                                                          ExpandTags)));

        #endregion

    }

    /// <summary>
    /// A dashboard.
    /// </summary>
    public class Dashboard : AEntity<Dashboard_Id,
                                     Dashboard>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of dashboards.
        /// </summary>
        public new readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://opendata.social/contexts/UsersAPI/dashboard");

        #endregion

        #region Properties

        #region API

        private Object _API;

        /// <summary>
        /// The DashboardsAPI of this Dashboard.
        /// </summary>
        internal Object API
        {

            get
            {
                return _API;
            }

            set
            {

                if (_API is not null)
                    throw new ArgumentException("Illegal attempt to change the API of this communicator!");

                if (value is null)
                    throw new ArgumentException("Illegal attempt to delete the API reference of this communicator!");

                _API = value;

            }

        }

        #endregion


        ///// <summary>
        ///// The (multi-language) name of this dashboard.
        ///// </summary>
        //[Mandatory]
        //public I18NString                         Name                  { get; }

        ///// <summary>
        ///// The (multi-language) description of this dashboard.
        ///// </summary>
        //[Optional]
        //public I18NString                         Description           { get; }

        /// <summary>
        /// The timestamp of the creation of this dashboard.
        /// </summary>
        [Mandatory]
        public DateTimeOffset                     CreationDate          { get; }

        /// <summary>
        /// An enumeration of multi-language tags and their relevance.
        /// </summary>
        [Optional]
        public IEnumerable<TagRelevance>          Tags                  { get; }

        /// <summary>
        /// Whether the dashboard is disabled.
        /// </summary>
        [Optional]
        public Boolean                            IsDisabled            { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new dashboard.
        /// </summary>
        /// <param name="Name">The (multi-language) name of this dashboard.</param>
        /// <param name="Description">The (multi-language) description of this dashboard.</param>
        /// <param name="CreationDate">The timestamp of the publication of this dashboard.</param>
        /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
        /// <param name="IsDisabled">Whether the dashboard is disabled.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        public Dashboard(I18NString                  Name,
                         I18NString?                 Description    = null,
                         DateTimeOffset?             CreationDate   = null,
                         IEnumerable<TagRelevance>?  Tags           = null,
                         Boolean?                    IsDisabled     = false,
                         String                      DataSource     = "")

            : this(Dashboard_Id.Random(),
                   Name,
                   Description,
                   CreationDate,
                   Tags,
                   IsDisabled,
                   DataSource)

        { }


        /// <summary>
        /// Create a new Open Data dashboard.
        /// </summary>
        /// <param name="Id">The unique identification of this dashboard.</param>
        /// <param name="Name">The (multi-language) name of this dashboard.</param>
        /// <param name="Description">The (multi-language) description of this dashboard.</param>
        /// <param name="CreationDate">The timestamp of the publication of this dashboard.</param>
        /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
        /// <param name="IsDisabled">Whether the dashboard is disabled.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        public Dashboard(Dashboard_Id                Id,
                         I18NString                  Name,
                         I18NString?                 Description    = null,
                         DateTimeOffset?             CreationDate   = null,
                         IEnumerable<TagRelevance>?  Tags           = null,
                         Boolean?                    IsDisabled     = false,
                         String                      DataSource     = "")

            : base(Id,
                   DefaultJSONLDContext,
                   Name,
                   Description,
                   null,
                   null,
                   null,
                   null,
                   DataSource)

        {

            //this.Name          = Name         ?? throw new ArgumentNullException(nameof(Name), "The given name must not be null!");
            //this.Description   = Description  ?? I18NString.Empty;
            this.CreationDate  = CreationDate ?? Timestamp.Now;
            this.Tags          = Tags         ?? [];
            this.IsDisabled    = IsDisabled   ?? false;

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
        public JObject ToJSON(Boolean     Embedded    = false,
                              InfoStatus  ExpandTags  = InfoStatus.ShowIdOnly)

            => JSONObject.Create(

                   new JProperty("@id",                  Id.ToString()),

                   !Embedded
                       ? new JProperty("@context",       JSONLDContext.ToString())
                       : null,

                   Name.IsNotNullOrEmpty()
                       ? new JProperty("name",           Name.ToJSON())
                       : null,

                   Description.IsNotNullOrEmpty()
                       ? new JProperty("description",    Description.ToJSON())
                       : null,

                   new JProperty("creationDate",         CreationDate.ToISO8601()),

                   Tags.Any()
                       ? new JProperty("tags",           Tags.SafeSelect(tag => tag.ToJSON(ExpandTags)))
                       : null,

                   new JProperty("isDisabled",           IsDisabled)

               );

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out Dashboard, out ErrorResponse)

        public static Boolean TryParseJSON(JObject         JSONObject,
                                           out Dashboard?  Dashboard,
                                           out String?     ErrorResponse,
                                           Dashboard_Id?   DashboardIdURL  = null)
        {

            try
            {

                Dashboard = null;

                #region Parse DashboardId      [optional]

                // Verify that a given Dashboard identification
                //   is at least valid.
                if (JSONObject.ParseOptional("@id",
                                             "dashboard identification",
                                             Dashboard_Id.TryParse,
                                             out Dashboard_Id? DashboardIdBody,
                                             out ErrorResponse))
                {

                    if (ErrorResponse is not null)
                        return false;

                }

                if (!DashboardIdURL.HasValue && !DashboardIdBody.HasValue)
                {
                    ErrorResponse = "The Dashboard identification is missing!";
                    return false;
                }

                if (DashboardIdURL.HasValue && DashboardIdBody.HasValue && DashboardIdURL.Value != DashboardIdBody.Value)
                {
                    ErrorResponse = "The optional Dashboard identification given within the JSON body does not match the one given in the URI!";
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

                #region Parse Name             [mandatory]

                if (!JSONObject.ParseMandatory("name",
                                               "dashboard name",
                                               out I18NString Name,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Description      [optional]

                if (!JSONObject.ParseOptional("description",
                                              "dashboard description",
                                              out I18NString Description,
                                              out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Creation date    [optional]

                if (!JSONObject.ParseOptional("creationDate",
                                              "creation date",
                                              out DateTime? CreationDate,
                                              out ErrorResponse))
                {
                    return false;
                }

                #endregion

                var Tags             = new TagRelevance[0];

                var IsDisabled       = JSONObject["isDisabled"]?.Value<Boolean>();

                #region Get   DataSource       [optional]

                var DataSource = JSONObject.GetOptional("dataSource");

                #endregion

                #region Parse CryptoHash       [optional]

                var CryptoHash    = JSONObject.GetOptional("cryptoHash");

                #endregion


                Dashboard = new Dashboard(DashboardIdBody ?? DashboardIdURL.Value,
                                          Name,
                                          Description,
                                          CreationDate,
                                          Tags,
                                          IsDisabled,
                                          DataSource);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse  = e.Message;
                Dashboard      = null;
                return false;
            }

        }

        #endregion


        #region CopyAllLinkedDataFrom(OldDashboard)

        public override void CopyAllLinkedDataFromBase(Dashboard OldDashboard)
        {

        }

        #endregion


        #region Operator overloading

        #region Operator == (DashboardId1, DashboardId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DashboardId1">A Dashboard identification.</param>
        /// <param name="DashboardId2">Another Dashboard identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (Dashboard DashboardId1, Dashboard DashboardId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(DashboardId1, DashboardId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) DashboardId1 is null) || ((Object) DashboardId2 is null))
                return false;

            return DashboardId1.Equals(DashboardId2);

        }

        #endregion

        #region Operator != (DashboardId1, DashboardId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DashboardId1">A Dashboard identification.</param>
        /// <param name="DashboardId2">Another Dashboard identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (Dashboard DashboardId1, Dashboard DashboardId2)
            => !(DashboardId1 == DashboardId2);

        #endregion

        #region Operator <  (DashboardId1, DashboardId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DashboardId1">A Dashboard identification.</param>
        /// <param name="DashboardId2">Another Dashboard identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (Dashboard DashboardId1, Dashboard DashboardId2)
        {

            if ((Object) DashboardId1 is null)
                throw new ArgumentNullException(nameof(DashboardId1), "The given DashboardId1 must not be null!");

            return DashboardId1.CompareTo(DashboardId2) < 0;

        }

        #endregion

        #region Operator <= (DashboardId1, DashboardId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DashboardId1">A Dashboard identification.</param>
        /// <param name="DashboardId2">Another Dashboard identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (Dashboard DashboardId1, Dashboard DashboardId2)
            => !(DashboardId1 > DashboardId2);

        #endregion

        #region Operator >  (DashboardId1, DashboardId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DashboardId1">A Dashboard identification.</param>
        /// <param name="DashboardId2">Another Dashboard identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (Dashboard DashboardId1, Dashboard DashboardId2)
        {

            if ((Object) DashboardId1 is null)
                throw new ArgumentNullException(nameof(DashboardId1), "The given DashboardId1 must not be null!");

            return DashboardId1.CompareTo(DashboardId2) > 0;

        }

        #endregion

        #region Operator >= (DashboardId1, DashboardId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="DashboardId1">A Dashboard identification.</param>
        /// <param name="DashboardId2">Another Dashboard identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (Dashboard DashboardId1, Dashboard DashboardId2)
            => !(DashboardId1 < DashboardId2);

        #endregion

        #endregion

        #region IComparable<Dashboard> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public override Int32 CompareTo(Object Object)
        {

            if (Object is null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            var Dashboard = Object as Dashboard;
            if ((Object) Dashboard is null)
                throw new ArgumentException("The given object is not an Dashboard!");

            return CompareTo(Dashboard);

        }

        #endregion

        #region CompareTo(Dashboard)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Dashboard">An Dashboard object to compare with.</param>
        public override Int32 CompareTo(Dashboard Dashboard)
        {

            if ((Object) Dashboard is null)
                throw new ArgumentNullException("The given Dashboard must not be null!");

            return Id.CompareTo(Dashboard.Id);

        }

        #endregion

        #endregion

        #region IEquatable<Dashboard> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is null)
                return false;

            var Dashboard = Object as Dashboard;
            if ((Object) Dashboard is null)
                return false;

            return Equals(Dashboard);

        }

        #endregion

        #region Equals(Dashboard)

        /// <summary>
        /// Compares two Dashboards for equality.
        /// </summary>
        /// <param name="Dashboard">An Dashboard to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(Dashboard Dashboard)
        {

            if ((Object) Dashboard is null)
                return false;

            return Id.Equals(Dashboard.Id);

        }

        #endregion

        #endregion

        #region (override) GetHashCode()

        /// <summary>
        /// Get the hash code of this object.
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


        #region ToBuilder(NewDashboardId = null)

        /// <summary>
        /// Return a builder for this Dashboard.
        /// </summary>
        /// <param name="NewDashboardId">An optional new Dashboard identification.</param>
        public Builder ToBuilder(Dashboard_Id? NewDashboardId = null)

            => new Builder(NewDashboardId ?? Id,
                           Name,
                           Description,
                           CreationDate,
                           Tags,
                           IsDisabled,
                           DataSource);

        #endregion

        #region (class) Builder

        /// <summary>
        /// A dashboard builder.
        /// </summary>
        public new class Builder
        {

            #region Properties

            /// <summary>
            /// The unique identification of this dashboard.
            /// </summary>
            public Dashboard_Id                       Id                    { get; set; }

            /// <summary>
            /// The (multi-language) name of this dashboard.
            /// </summary>
            [Mandatory]
            public I18NString                         Name                  { get; set; }

            /// <summary>
            /// The (multi-language) description of this dashboard.
            /// </summary>
            [Optional]
            public I18NString                         Description           { get; set; }

            /// <summary>
            /// The timestamp of the creation of this dashboard.
            /// </summary>
            [Mandatory]
            public DateTimeOffset                     CreationDate          { get; set; }

            /// <summary>
            /// An enumeration of multi-language tags and their relevance.
            /// </summary>
            [Optional]
            public IEnumerable<TagRelevance>          Tags                  { get; set; }

            /// <summary>
            /// Whether the dashboard is disabled.
            /// </summary>
            [Optional]
            public Boolean                            IsDisabled            { get; set; }

            /// <summary>
            /// The source of this information, e.g. an automatic importer.
            /// </summary>
            [Optional]
            public String                             DataSource            { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new dashboard builder.
            /// </summary>
            /// <param name="Name">The (multi-language) name of this dashboard.</param>
            /// <param name="Description">The (multi-language) description of this dashboard.</param>
            /// <param name="CreationDate">The timestamp of the publication of this dashboard.</param>
            /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
            /// <param name="IsDisabled">Whether the dashboard is disabled.</param>
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            public Builder(I18NString                 Name,
                           I18NString                 Description    = null,
                           DateTimeOffset?            CreationDate   = null,
                           IEnumerable<TagRelevance>  Tags           = null,
                           Boolean?                   IsDisabled     = false,
                           String                     DataSource     = "")

                : this(Dashboard_Id.Random(),
                       Name,
                       Description,
                       CreationDate,
                       Tags,
                       IsDisabled,
                       DataSource)

            { }


            /// <summary>
            /// Create a new Dashboard builder.
            /// </summary>
            /// <param name="Id">The unique identification of this dashboard.</param>
            /// <param name="Name">The (multi-language) name of this dashboard.</param>
            /// <param name="Description">The (multi-language) description of this dashboard.</param>
            /// <param name="CreationDate">The timestamp of the publication of this dashboard.</param>
            /// <param name="Tags">An enumeration of multi-language tags and their relevance.</param>
            /// <param name="IsDisabled">Whether the dashboard is disabled.</param>
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            public Builder(Dashboard_Id               Id,
                           I18NString                 Name,
                           I18NString                 Description    = null,
                           DateTimeOffset?            CreationDate   = null,
                           IEnumerable<TagRelevance>  Tags           = null,
                           Boolean?                   IsDisabled     = false,
                           String                     DataSource     = "")
            {

                this.Id            = Id;
                this.Name          = Name         ?? throw new ArgumentNullException(nameof(Name), "The given name must not be null!");
                this.Description   = Description  ?? I18NString.Empty;
                this.CreationDate  = CreationDate ?? DateTime.Now;
                this.Tags          = Tags is not null
                                         ? new List<TagRelevance>(Tags)
                                         : new List<TagRelevance>();
                this.IsDisabled    = IsDisabled   ?? false;
                this.DataSource    = DataSource;

            }

            #endregion

            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the Dashboard.
            /// </summary>
            /// <param name="Builder">A Dashboard builder.</param>
            public static implicit operator Dashboard(Builder Builder)

                => Builder?.ToImmutable;


            /// <summary>
            /// Return an immutable version of the Dashboard.
            /// </summary>
            public Dashboard ToImmutable

                => new Dashboard(Id,
                                 Name,
                                 Description,
                                 CreationDate,
                                 Tags,
                                 IsDisabled,
                                 DataSource);

            #endregion

        }

        #endregion

    }

}
