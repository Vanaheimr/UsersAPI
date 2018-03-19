/*
 * Copyright (c) 2014-2018, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Hermod.Distributed;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Styx.Arrows;

#endregion

namespace org.GraphDefined.OpenData.Users
{

    /// <summary>
    /// A user.
    /// </summary>
    public class User : ADistributedEntity<User_Id>,
                        IEntityClass<User>
    {

        #region Data

        /// <summary>
        /// The default max size of the aggregated EVSE operator status history.
        /// </summary>
        public const UInt16 DefaultUserStatusHistorySize = 50;


        private readonly ReactiveSet<MiniEdge<User, User2UserEdges,         User>>          _User2UserEdges;
        private readonly ReactiveSet<MiniEdge<User, User2GroupEdges,        Group>>         _User2GroupEdges;
        private readonly ReactiveSet<MiniEdge<User, User2OrganizationEdges, Organization>>  _User2OrganizationEdges;

        /// <summary>
        /// The JSON-LD context of the object.
        /// </summary>
        public const String JSONLDContext  = "https://opendata.social/contexts/UsersAPI+json/user";

        #endregion

        #region Properties

        /// <summary>
        /// The primary E-Mail address of the user.
        /// </summary>
        [Mandatory]
        public EMailAddress        EMail                { get; }

        /// <summary>
        /// The offical public name of the user.
        /// </summary>
        [Optional]
        public String              Name                 { get; }

        /// <summary>
        /// The PGP/GPG public keyring of the user.
        /// </summary>
        [Optional]
        public String              PublicKeyRing        { get; }

        /// <summary>
        /// The telephone number of the user.
        /// </summary>
        [Optional]
        public PhoneNumber?        Telephone            { get; }

        /// <summary>
        /// An optional (multi-language) description of the user.
        /// </summary>
        [Optional]
        public I18NString          Description          { get; }

        /// <summary>
        /// The geographical location of this organization.
        /// </summary>
        public GeoCoordinate?      GeoLocation          { get; }

        /// <summary>
        /// The optional address of the organization.
        /// </summary>
        [Optional]
        public Address             Address              { get; }

        /// <summary>
        /// Whether the user will be shown in user listings, or not.
        /// </summary>
        [Mandatory]
        public PrivacyLevel        PrivacyLevel         { get; }

        /// <summary>
        /// The user will not be shown in user listings, as its
        /// primary e-mail address is not yet authenticated.
        /// </summary>
        [Mandatory]
        public Boolean             IsAuthenticated      { get; }

        /// <summary>
        /// The user is disabled.
        /// </summary>
        [Mandatory]
        public Boolean             IsDisabled           { get; }

        #region Genimi

        /// <summary>
        /// The gemini of this user.
        /// </summary>
        public IEnumerable<User> Genimi
        {
            get
            {
                return _User2UserEdges.
                           Where (edge => edge.EdgeLabel == User2UserEdges.gemini).
                           Select(edge => edge.Target);
            }
        }

        #endregion

        #region FollowsUsers

        /// <summary>
        /// This user follows this other users.
        /// </summary>
        public IEnumerable<User> FollowsUsers
        {
            get
            {
                return _User2UserEdges.
                           Where(edge => edge.EdgeLabel == User2UserEdges.follows).
                           Select(edge => edge.Target);
            }
        }

        #endregion

        #region IsFollowedBy

        /// <summary>
        /// This user is followed by this other users.
        /// </summary>
        public IEnumerable<User> IsFollowedBy
        {
            get
            {
                return _User2UserEdges.
                           Where(edge => edge.EdgeLabel == User2UserEdges.IsFollowedBy).
                           Select(edge => edge.Target);
            }
        }

        #endregion

        #region Groups()

        /// <summary>
        /// All groups this user belongs to.
        /// </summary>
        public IEnumerable<Group> Groups()
            => _User2GroupEdges.
                   Select(edge => edge.Target);

        #endregion

        #region Groups(EdgeFilter)

        /// <summary>
        /// All groups this user belongs to,
        /// filtered by the given edge label.
        /// </summary>
        public IEnumerable<Group> Groups(User2GroupEdges EdgeFilter)
            => _User2GroupEdges.
                   Where (edge => edge.EdgeLabel == EdgeFilter).
                   Select(edge => edge.Target);

        #endregion

        #region Edges(Group)

        /// <summary>
        /// All groups this user belongs to,
        /// filtered by the given edge label.
        /// </summary>
        public IEnumerable<User2GroupEdges> OutEdges(Group Group)
            => _User2GroupEdges.
                   Where (edge => edge.Target == Group).
                   Select(edge => edge.EdgeLabel);

        #endregion

        #region Edges(Organization)

        /// <summary>
        /// All organizations this user belongs to,
        /// filtered by the given edge label.
        /// </summary>
        public IEnumerable<User2OrganizationEdges> Edges(Organization Organization)
            => _User2OrganizationEdges.
                   Where (edge => edge.Target == Organization).
                   Select(edge => edge.EdgeLabel);

        #endregion

        #endregion

        #region Events

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="Id">The unique identification of the user.</param>
        /// <param name="EMail">The primary e-mail of the user.</param>
        /// <param name="Name">An offical (multi-language) name of the user.</param>
        /// <param name="PublicKeyRing">An optional PGP/GPG public keyring of the user.</param>
        /// <param name="Telephone">An optional telephone number of the user.</param>
        /// <param name="Description">An optional (multi-language) description of the user.</param>
        /// <param name="GeoLocation">An optional geographical location of the user.</param>
        /// <param name="Address">An optional address of the user.</param>
        /// <param name="PrivacyLevel">Whether the user will be shown in user listings, or not.</param>
        /// <param name="IsAuthenticated">The user will not be shown in user listings, as its primary e-mail address is not yet authenticated.</param>
        /// <param name="IsDisabled">The user is disabled.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        internal User(User_Id             Id,
                      SimpleEMailAddress  EMail,
                      String              Name              = null,
                      String              PublicKeyRing     = null,
                      PhoneNumber?        Telephone         = null,
                      I18NString          Description       = null,
                      GeoCoordinate?      GeoLocation       = null,
                      Address             Address           = null,
                      PrivacyLevel?       PrivacyLevel      = null,
                      Boolean             IsDisabled        = false,
                      Boolean             IsAuthenticated   = false,
                      String              DataSource        = "")

            : base(Id,
                   DataSource)

        {

            #region Init properties

            this.EMail                    = Name.IsNotNullOrEmpty()
                                                ? new EMailAddress(Name, EMail, null, null)
                                                : new EMailAddress(      EMail, null, null);
            this.Name                     = Name.IsNotNullOrEmpty()
                                                ? Name
                                                : "";
            this.PublicKeyRing            = PublicKeyRing;
            this.Telephone                = Telephone;
            this.Description              = Description  ?? new I18NString();
            this.GeoLocation              = GeoLocation;
            this.Address                  = Address;
            this.PrivacyLevel             = PrivacyLevel ?? Users.PrivacyLevel.World;
            this.IsAuthenticated          = IsAuthenticated;
            this.IsDisabled               = IsDisabled;

            #endregion

            #region Init edges

            this._User2UserEdges          = new ReactiveSet<MiniEdge<User, User2UserEdges,         User>>();
            this._User2GroupEdges         = new ReactiveSet<MiniEdge<User, User2GroupEdges,        Group>>();
            this._User2OrganizationEdges  = new ReactiveSet<MiniEdge<User, User2OrganizationEdges, Organization>>();

            #endregion

            CalcHash();

        }

        #endregion


        public MiniEdge<User, User2UserEdges, User>

            Follow(User          User,
                   PrivacyLevel  PrivacyLevel = PrivacyLevel.Private)

            => User.AddIncomingEdge(AddOutgoingEdge(User2UserEdges.follows, User, PrivacyLevel));




        public MiniEdge<User, User2UserEdges, User>

            AddIncomingEdge(MiniEdge<User, User2UserEdges, User>  Edge)

            => _User2UserEdges.AddAndReturn(Edge);

        public MiniEdge<User, User2UserEdges, User>

            AddIncomingEdge(User            Source,
                            User2UserEdges  EdgeLabel,
                            PrivacyLevel    PrivacyLevel = PrivacyLevel.World)

            => _User2UserEdges.AddAndReturn(new MiniEdge<User, User2UserEdges, User>(Source, EdgeLabel, this, PrivacyLevel));



        public MiniEdge<User, User2UserEdges, User>

            AddOutgoingEdge(User2UserEdges  EdgeLabel,
                            User            Target,
                            PrivacyLevel    PrivacyLevel = PrivacyLevel.World)

            => _User2UserEdges.AddAndReturn(new MiniEdge<User, User2UserEdges, User>(this, EdgeLabel, Target, PrivacyLevel));

        public MiniEdge<User, User2OrganizationEdges, Organization>

            AddOutgoingEdge(User2OrganizationEdges  EdgeLabel,
                            Organization            Target,
                            PrivacyLevel            PrivacyLevel = PrivacyLevel.World)

            => _User2OrganizationEdges.AddAndReturn(new MiniEdge<User, User2OrganizationEdges, Organization>(this, EdgeLabel, Target, PrivacyLevel));



        public MiniEdge<User, User2GroupEdges, Group>

            AddOutgoingEdge(User2GroupEdges EdgeLabel,
                            Group           Target,
                            PrivacyLevel    PrivacyLevel = PrivacyLevel.World)

            => _User2GroupEdges.AddAndReturn(new MiniEdge<User, User2GroupEdges, Group>(this, EdgeLabel, Target, PrivacyLevel));


        public IEnumerable<MiniEdge<User, User2GroupEdges, Group>> User2GroupOutEdges(Func<User2GroupEdges, Boolean> User2GroupEdgeFilter)
            => _User2GroupEdges.Where(edge => User2GroupEdgeFilter(edge.EdgeLabel));


        #region Organizations(RequireReadWriteAccess = false, Recursive = false)

        public IEnumerable<Organization> Organizations(Boolean RequireReadWriteAccess  = false,
                                                       Boolean Recursive               = false)
        {

            var _Organizations = RequireReadWriteAccess

                                     ? _User2OrganizationEdges.
                                           Where (edge => edge.EdgeLabel == User2OrganizationEdges.IsAdmin ||
                                                          edge.EdgeLabel == User2OrganizationEdges.IsMember).
                                           Select(edge => edge.Target).
                                           ToHashSet()

                                     : _User2OrganizationEdges.
                                           Where (edge => edge.EdgeLabel == User2OrganizationEdges.IsAdmin  ||
                                                          edge.EdgeLabel == User2OrganizationEdges.IsMember ||
                                                          edge.EdgeLabel == User2OrganizationEdges.IsVisitor).
                                           Select(edge => edge.Target).
                                           ToHashSet();

            if (Recursive)
            {

                Organization[] Level2 = null;

                do
                {

                    Level2 = _Organizations.SelectMany(organization => organization.
                                                                           Organization2OrganizationInEdges.
                                                                           Where(edge => edge.EdgeLabel == Organization2OrganizationEdges.IsChildOf)).
                                            Select    (edge         => edge.Target).
                                            Where     (organization => !_Organizations.Contains(organization)).
                                            ToArray();

                    foreach (var organization in Level2)
                        _Organizations.Add(organization);

                } while (Level2.Length > 0);

            }

            return _Organizations;

        }

        #endregion

        #region Groups(RequireReadWriteAccess = false, Recursive = false)

        public IEnumerable<Group> Groups(Boolean RequireReadWriteAccess  = false,
                                         Boolean Recursive               = false)
        {

            var _Groups = RequireReadWriteAccess

                                     ? _User2GroupEdges.
                                           Where (edge => edge.EdgeLabel == User2GroupEdges.IsAdmin ||
                                                          edge.EdgeLabel == User2GroupEdges.IsMember).
                                           Select(edge => edge.Target).
                                           ToHashSet()

                                     : _User2GroupEdges.
                                           Where (edge => edge.EdgeLabel == User2GroupEdges.IsAdmin  ||
                                                          edge.EdgeLabel == User2GroupEdges.IsMember ||
                                                          edge.EdgeLabel == User2GroupEdges.IsVisitor).
                                           Select(edge => edge.Target).
                                           ToHashSet();

            //if (Recursive)
            //{

            //    Group[] Level2 = null;

            //    do
            //    {

            //        Level2 = _Groups.SelectMany(group => group.
            //                                                 Group2GroupInEdges.
            //                                                 Where(edge => edge.EdgeLabel == Group2GroupEdges.IsChildOf)).
            //                         Select    (edge  => edge.Target).
            //                         Where     (group => !_Groups.Contains(group)).
            //                         ToArray();

            //        foreach (var organization in Level2)
            //            _Groups.Add(organization);

            //    } while (Level2.Length > 0);

            //}

            return _Groups;

        }

        #endregion


        #region ToJSON(IncludeCryptoHash = true)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="IncludeCryptoHash">Include the hash value of this object.</param>
        public override JObject ToJSON(Boolean IncludeCryptoHash = true)

            => JSONObject.Create(

                   new JProperty("@id",                 Id.ToString()),
                   new JProperty("@context",            JSONLDContext),
                   new JProperty("name",                Name),
                   new JProperty("email",               EMail.Address.ToString()),

                   PublicKeyRing != null
                       ? new JProperty("publickey",     PublicKeyRing)
                       : null,

                   Telephone != null
                       ? new JProperty("telephone",     Telephone)
                       : null,

                   IEnumerableExtensions.IsNeitherNullNorEmpty(Description)
                       ? new JProperty("description",   Description.ToJSON())
                       : null,

                   PrivacyLevel.ToJSON(),
                   new JProperty("isAuthenticated",     IsAuthenticated),
                   new JProperty("isDisabled",          IsDisabled),

                   new JProperty("signatures",          new JArray()),

                   IncludeCryptoHash
                       ? new JProperty("hash", CurrentCryptoHash)
                       : null

               );

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out User, out ErrorResponse)

        public static Boolean TryParseJSON(JObject     JSONObject,
                                           out User    User,
                                           out String  ErrorResponse,
                                           User_Id?    UserIdURI = null)
        {

            try
            {

                User = null;

                #region Parse UserId           [optional]

                // Verify that a given user identification
                //   is at least valid.
                if (!JSONObject.ParseOptionalN("@id",
                                               "user identification",
                                               User_Id.TryParse,
                                               out User_Id? UserIdBody,
                                               out ErrorResponse))
                {
                    return false;
                }

                if (!UserIdURI.HasValue && !UserIdBody.HasValue)
                {
                    ErrorResponse = "The user identification is missing!";
                    return false;
                }

                if (UserIdURI.HasValue && UserIdBody.HasValue && UserIdURI.Value != UserIdBody.Value)
                {
                    ErrorResponse = "The optional user identification given within the JSON body does not match the one given in the URI!";
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

                #region Parse E-Mail           [mandatory]

                if (!JSONObject.ParseMandatory("email",
                                               "E-Mail",
                                               SimpleEMailAddress.Parse,
                                               out SimpleEMailAddress EMail,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Name             [mandatory]

                if (!JSONObject.ParseMandatory("name",
                                               "Username",
                                               out String Name,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse PublicKey        [optional]

                if (!JSONObject.ParseOptional("publicKey",
                                              out String PublicKey))
                {
                    return false;
                }

                #endregion

                #region Parse Telephone        [optional]

                if (JSONObject.ParseOptional("telephone",
                                             "user phone number",
                                             PhoneNumber.TryParse,
                                             out PhoneNumber? Telephone,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Description      [optional]

                if (!JSONObject.ParseOptional("description",
                                              "Description",
                                              out I18NString Description,
                                              out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse GeoLocation      [optional]

                if (!JSONObject.ParseOptionalN("geoLocation",
                                               "Geo location",
                                               GeoCoordinate.ParseGeoCoordinate,
                                               out GeoCoordinate? GeoLocation,
                                               out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Address          [optional]

                if (JSONObject.ParseOptional("address",
                                             "address",
                                             Vanaheimr.Hermod.Address.Parse,
                                             out Address Address,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse PrivacyLevel     [optional]

                if (JSONObject.ParseOptional("privacyLevel",
                                             "privacy level",
                                             out PrivacyLevel? PrivacyLevel,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                var IsAuthenticated  = JSONObject["isAuthenticated"]?.Value<Boolean>();

                var IsDisabled       = JSONObject["isDisabled"]?.     Value<Boolean>();

                #region Get   DataSource       [optional]

                var DataSource = JSONObject.GetOptional("dataSource");

                #endregion

                #region Parse CryptoHash       [optional]

                var CryptoHash    = JSONObject.GetOptional("cryptoHash");

                #endregion


                User = new User(UserIdBody ?? UserIdURI.Value,
                                EMail,
                                Name,
                                PublicKey,
                                Telephone,
                                Description,
                                GeoLocation,
                                Address,
                                PrivacyLevel,
                                IsAuthenticated ?? false,
                                IsDisabled      ?? false,
                                DataSource);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse  = e.Message;
                User  = null;
                return false;
            }

        }

        #endregion


        #region IComparable<User> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            var User = Object as User;
            if ((Object) User == null)
                throw new ArgumentException("The given object is not an user!");

            return CompareTo(User);

        }

        #endregion

        #region CompareTo(User)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="User">An user object to compare with.</param>
        public Int32 CompareTo(User User)
        {

            if ((Object) User == null)
                throw new ArgumentNullException("The given user must not be null!");

            return Id.CompareTo(User.Id);

        }

        #endregion

        #endregion

        #region IEquatable<User> Members

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

            var User = Object as User;
            if ((Object) User == null)
                return false;

            return Equals(User);

        }

        #endregion

        #region Equals(User)

        /// <summary>
        /// Compares two users for equality.
        /// </summary>
        /// <param name="User">An user to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(User User)
        {

            if ((Object) User == null)
                return false;

            return Id.Equals(User.Id);

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

        #region ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()
            => Id.ToString();

        #endregion


        #region ToBuilder(NewUserId = null)

        /// <summary>
        /// Return a builder for this user.
        /// </summary>
        /// <param name="NewUserId">An optional new user identification.</param>
        public Builder ToBuilder(User_Id? NewUserId = null)

            => new Builder(NewUserId ?? Id,
                           EMail.Address,
                           Name,
                           PublicKeyRing,
                           Telephone,
                           Description,
                           GeoLocation,
                           Address,
                           PrivacyLevel,
                           IsDisabled,
                           IsAuthenticated);

        #endregion

        #region (class) Builder

        /// <summary>
        /// A user.
        /// </summary>
        public class Builder
        {

            #region Properties

            /// <summary>
            /// The unique identification of the user.
            /// </summary>
            public User_Id             Id                   { get; set; }

            /// <summary>
            /// The primary E-Mail address of the user.
            /// </summary>
            [Mandatory]
            public EMailAddress        EMail                { get; set; }

            /// <summary>
            /// The offical public name of the user.
            /// </summary>
            [Optional]
            public String              Name                 { get; set; }

            /// <summary>
            /// The PGP/GPG public keyring of the user.
            /// </summary>
            [Optional]
            public String              PublicKeyRing        { get; set; }

            /// <summary>
            /// The telephone number of the user.
            /// </summary>
            [Optional]
            public PhoneNumber?        Telephone            { get; set; }

            /// <summary>
            /// An optional (multi-language) description of the user.
            /// </summary>
            [Optional]
            public I18NString          Description          { get; set; }

            /// <summary>
            /// The geographical location of this organization.
            /// </summary>
            public GeoCoordinate?      GeoLocation          { get; set; }

            /// <summary>
            /// The optional address of the organization.
            /// </summary>
            [Optional]
            public Address             Address              { get; set; }

            /// <summary>
            /// Whether the user will be shown in user listings, or not.
            /// </summary>
            [Mandatory]
            public PrivacyLevel        PrivacyLevel         { get; set; }

            /// <summary>
            /// The user is disabled.
            /// </summary>
            [Mandatory]
            public Boolean             IsDisabled           { get; set; }

            /// <summary>
            /// The user will not be shown in user listings, as its
            /// primary e-mail address is not yet authenticated.
            /// </summary>
            [Mandatory]
            public Boolean             IsAuthenticated      { get; set; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new user builder.
            /// </summary>
            /// <param name="Id">The unique identification of the user.</param>
            /// <param name="EMail">The primary e-mail of the user.</param>
            /// <param name="Name">An offical (multi-language) name of the user.</param>
            /// <param name="PublicKeyRing">An optional PGP/GPG public keyring of the user.</param>
            /// <param name="Telephone">An optional telephone number of the user.</param>
            /// <param name="Description">An optional (multi-language) description of the user.</param>
            /// <param name="GeoLocation">An optional geographical location of the user.</param>
            /// <param name="Address">An optional address of the user.</param>
            /// <param name="PrivacyLevel">Whether the user will be shown in user listings, or not.</param>
            /// <param name="IsDisabled">The user is disabled.</param>
            /// <param name="IsAuthenticated">The user will not be shown in user listings, as its primary e-mail address is not yet authenticated.</param>
            public Builder(User_Id             Id,
                           SimpleEMailAddress  EMail,
                           String              Name              = null,
                           String              PublicKeyRing     = null,
                           PhoneNumber?        Telephone         = null,
                           I18NString          Description       = null,
                           GeoCoordinate?      GeoLocation       = null,
                           Address             Address           = null,
                           PrivacyLevel        PrivacyLevel      = PrivacyLevel.World,
                           Boolean             IsDisabled        = false,
                           Boolean             IsAuthenticated   = false)
            {

                #region Init properties

                this.Id                       = Id;
                this.EMail                    = Name.IsNotNullOrEmpty()
                                                    ? new EMailAddress(Name, EMail, null, null)
                                                    : new EMailAddress(      EMail, null, null);
                this.Name                     = Name.IsNotNullOrEmpty()
                                                    ? Name
                                                    : "";
                this.PublicKeyRing            = PublicKeyRing;
                this.Telephone                = Telephone;
                this.Description              = Description ?? new I18NString();
                this.GeoLocation              = GeoLocation;
                this.Address                  = Address;
                this.PrivacyLevel             = PrivacyLevel;
                this.IsDisabled               = IsDisabled;
                this.IsAuthenticated          = IsAuthenticated;

                #endregion

                #region Init edges

                //this._User2UserEdges          = new ReactiveSet<MiniEdge<User, User2UserEdges,         User>>();
                //this._User2GroupEdges         = new ReactiveSet<MiniEdge<User, User2GroupEdges,        Group>>();
                //this._User2OrganizationEdges  = new ReactiveSet<MiniEdge<User, User2OrganizationEdges, Organization>>();

                #endregion

            }

            #endregion


            #region Build()

            /// <summary>
            /// Return an immutable version of the user.
            /// </summary>
            public User Build()

                => new User(Id,
                            EMail.Address,
                            Name,
                            PublicKeyRing,
                            Telephone,
                            Description,
                            GeoLocation,
                            Address,
                            PrivacyLevel,
                            IsAuthenticated,
                            IsDisabled);

            #endregion

        }

        #endregion

    }

}
