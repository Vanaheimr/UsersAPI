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
using Org.BouncyCastle.Bcpg.OpenPgp;
using org.GraphDefined.Vanaheimr.BouncyCastle;

#endregion

namespace org.GraphDefined.OpenData.Users
{

    public enum Access_Level
    {
        ReadOnly,
        ReadWrite,
        Admin
    }

    /// <summary>
    /// Extention methods for the User.
    /// </summary>
    public static class UserExtentions
    {

        #region CopyAllEdgesTo(this OldUser, NewUser)

        public static void CopyAllEdgesTo(this User  OldUser,
                                          User       NewUser)
        {

            if (OldUser.__User2UserEdges.Any())
            {

                NewUser.Add(OldUser.__User2UserEdges);

                foreach (var edge in NewUser.__User2UserEdges)
                    edge.Source = NewUser;

            }

            if (OldUser.__User2OrganizationEdges.Any())
            {

                NewUser.Add(OldUser.__User2OrganizationEdges);

                foreach (var edge in NewUser.__User2OrganizationEdges)
                    edge.Source = NewUser;

            }

            if (OldUser.__User2GroupEdges.Any())
            {

                NewUser.Add(OldUser.__User2GroupEdges);

                foreach (var edge in NewUser.__User2GroupEdges)
                    edge.Source = NewUser;

            }

        }

        #endregion

    }

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


        #region PairedMessages messages

        private readonly List<MiniEdge<User, User2UserEdges, User>> _User2UserEdges;

        /// <summary>
        /// All PAIRED messages.
        /// </summary>
        public IEnumerable<MiniEdge<User, User2UserEdges, User>> __User2UserEdges
            => _User2UserEdges;

        /// <summary>
        /// Add a new PAIRED message.
        /// </summary>
        /// <param name="PairedMessage">A PAIRED message.</param>
        public MiniEdge<User, User2UserEdges, User> Add(MiniEdge<User, User2UserEdges, User> PairedMessage)
            => _User2UserEdges.AddAndReturnElement(PairedMessage);

        /// <summary>
        /// Add new PAIRED messages.
        /// </summary>
        /// <param name="PairedMessages">An enumeration of PAIRED messages.</param>
        public IEnumerable<MiniEdge<User, User2UserEdges, User>> Add(IEnumerable<MiniEdge<User, User2UserEdges, User>> PairedMessages)
            => _User2UserEdges.AddRangeAndReturnEnumeration(PairedMessages);

        #endregion

        #region PairedMessages messages

        private readonly List<MiniEdge<User, User2GroupEdges, Group>> _User2GroupEdges;

        /// <summary>
        /// All PAIRED messages.
        /// </summary>
        public IEnumerable<MiniEdge<User, User2GroupEdges, Group>> __User2GroupEdges
            => _User2GroupEdges;

        /// <summary>
        /// Add a new PAIRED message.
        /// </summary>
        /// <param name="PairedMessage">A PAIRED message.</param>
        public MiniEdge<User, User2GroupEdges, Group> Add(MiniEdge<User, User2GroupEdges, Group> PairedMessage)
            => _User2GroupEdges.AddAndReturnElement(PairedMessage);

        /// <summary>
        /// Add new PAIRED messages.
        /// </summary>
        /// <param name="PairedMessages">An enumeration of PAIRED messages.</param>
        public IEnumerable<MiniEdge<User, User2GroupEdges, Group>> Add(IEnumerable<MiniEdge<User, User2GroupEdges, Group>> PairedMessages)
            => _User2GroupEdges.AddRangeAndReturnEnumeration(PairedMessages);

        #endregion

        #region PairedMessages messages

        private readonly List<MiniEdge<User, User2OrganizationEdges, Organization>> _User2OrganizationEdges;

        /// <summary>
        /// All PAIRED messages.
        /// </summary>
        public IEnumerable<MiniEdge<User, User2OrganizationEdges, Organization>> __User2OrganizationEdges
            => _User2OrganizationEdges;

        /// <summary>
        /// Add a new PAIRED message.
        /// </summary>
        /// <param name="PairedMessage">A PAIRED message.</param>
        public MiniEdge<User, User2OrganizationEdges, Organization> Add(MiniEdge<User, User2OrganizationEdges, Organization> PairedMessage)
            => _User2OrganizationEdges.AddAndReturnElement(PairedMessage);

        /// <summary>
        /// Add new PAIRED messages.
        /// </summary>
        /// <param name="PairedMessages">An enumeration of PAIRED messages.</param>
        public IEnumerable<MiniEdge<User, User2OrganizationEdges, Organization>> Add(IEnumerable<MiniEdge<User, User2OrganizationEdges, Organization>> PairedMessages)
            => _User2OrganizationEdges.AddRangeAndReturnEnumeration(PairedMessages);

        #endregion



        /// <summary>
        /// The JSON-LD context of the object.
        /// </summary>
        public const String JSONLDContext  = "https://opendata.social/contexts/UsersAPI+json/user";

        #endregion

        #region Properties

        #region API

        private Object _API;

        /// <summary>
        /// The UsersAPI of this user.
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
        public PgpPublicKeyRing    PublicKeyRing        { get; }

        /// <summary>
        /// The PGP/GPG secret keyring of the user.
        /// </summary>
        [Optional]
        public PgpSecretKeyRing    SecretKeyRing        { get; }

        /// <summary>
        /// The mobile telephone number of the user.
        /// </summary>
        [Optional]
        public PhoneNumber?        MobilePhone          { get; }

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
        /// <param name="Description">An optional (multi-language) description of the user.</param>
        /// <param name="PublicKeyRing">An optional PGP/GPG public keyring of the user.</param>
        /// <param name="SecretKeyRing">An optional PGP/GPG secret keyring of the user.</param>
        /// <param name="Telephone">An optional telephone number of the user.</param>
        /// <param name="GeoLocation">An optional geographical location of the user.</param>
        /// <param name="Address">An optional address of the user.</param>
        /// <param name="PrivacyLevel">Whether the user will be shown in user listings, or not.</param>
        /// <param name="IsAuthenticated">The user will not be shown in user listings, as its primary e-mail address is not yet authenticated.</param>
        /// <param name="IsDisabled">The user is disabled.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        internal User(User_Id                                                            Id,
                      SimpleEMailAddress                                                 EMail,
                      String                                                             Name                     = null,
                      I18NString                                                         Description              = null,
                      PgpPublicKeyRing                                                   PublicKeyRing            = null,
                      PgpSecretKeyRing                                                   SecretKeyRing            = null,
                      PhoneNumber?                                                       Telephone                = null,
                      GeoCoordinate?                                                     GeoLocation              = null,
                      Address                                                            Address                  = null,
                      PrivacyLevel?                                                      PrivacyLevel             = null,
                      Boolean                                                            IsDisabled               = false,
                      Boolean                                                            IsAuthenticated          = false,
                      String                                                             DataSource               = "",

                      IEnumerable<MiniEdge<User, User2UserEdges,         User>>          User2UserEdges           = null,
                      IEnumerable<MiniEdge<User, User2GroupEdges,        Group>>         User2GroupEdges          = null,
                      IEnumerable<MiniEdge<User, User2OrganizationEdges, Organization>>  User2OrganizationEdges   = null)

            : base(Id,
                   DataSource)

        {

            #region Initial checks

            if (Name != null)
                Name = Name.Trim();

            if (Name.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Name), "The givven username must not be null or empty!");

            #endregion

            #region Init properties

            this.EMail                    = Name.IsNotNullOrEmpty()
                                                ? new EMailAddress(Name, EMail, SecretKeyRing, PublicKeyRing)
                                                : new EMailAddress(      EMail, SecretKeyRing, PublicKeyRing);
            this.Name                     = Name;
            this.PublicKeyRing            = PublicKeyRing;
            this.SecretKeyRing            = SecretKeyRing;
            this.MobilePhone                = Telephone;
            this.Description              = Description  ?? new I18NString();
            this.GeoLocation              = GeoLocation;
            this.Address                  = Address;
            this.PrivacyLevel             = PrivacyLevel ?? OpenData.PrivacyLevel.Private;
            this.IsAuthenticated          = IsAuthenticated;
            this.IsDisabled               = IsDisabled;

            #endregion

            #region Init edges

            this._User2UserEdges          = User2UserEdges.        IsNeitherNullNorEmpty() ? new List<MiniEdge<User, User2UserEdges,         User>>        (User2UserEdges)         : new List<MiniEdge<User, User2UserEdges,         User>>();
            this._User2GroupEdges         = User2GroupEdges.       IsNeitherNullNorEmpty() ? new List<MiniEdge<User, User2GroupEdges,        Group>>       (User2GroupEdges)        : new List<MiniEdge<User, User2GroupEdges,        Group>>();
            this._User2OrganizationEdges  = User2OrganizationEdges.IsNeitherNullNorEmpty() ? new List<MiniEdge<User, User2OrganizationEdges, Organization>>(User2OrganizationEdges) : new List<MiniEdge<User, User2OrganizationEdges, Organization>>();

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

            => _User2UserEdges.AddAndReturnElement(Edge);

        public MiniEdge<User, User2UserEdges, User>

            AddIncomingEdge(User            Source,
                            User2UserEdges  EdgeLabel,
                            PrivacyLevel    PrivacyLevel = PrivacyLevel.World)

            => _User2UserEdges.AddAndReturnElement(new MiniEdge<User, User2UserEdges, User>(Source, EdgeLabel, this, PrivacyLevel));



        public MiniEdge<User, User2UserEdges, User>

            AddOutgoingEdge(User2UserEdges  EdgeLabel,
                            User            Target,
                            PrivacyLevel    PrivacyLevel = PrivacyLevel.World)

            => _User2UserEdges.AddAndReturnElement(new MiniEdge<User, User2UserEdges, User>(this, EdgeLabel, Target, PrivacyLevel));

        public MiniEdge<User, User2OrganizationEdges, Organization>

            AddOutgoingEdge(User2OrganizationEdges  EdgeLabel,
                            Organization            Target,
                            PrivacyLevel            PrivacyLevel = PrivacyLevel.World)

            => _User2OrganizationEdges.AddAndReturnElement(new MiniEdge<User, User2OrganizationEdges, Organization>(this, EdgeLabel, Target, PrivacyLevel));



        public MiniEdge<User, User2GroupEdges, Group>

            AddOutgoingEdge(User2GroupEdges EdgeLabel,
                            Group           Target,
                            PrivacyLevel    PrivacyLevel = PrivacyLevel.World)

            => _User2GroupEdges.AddAndReturnElement(new MiniEdge<User, User2GroupEdges, Group>(this, EdgeLabel, Target, PrivacyLevel));


        public IEnumerable<MiniEdge<User, User2GroupEdges, Group>> User2GroupOutEdges(Func<User2GroupEdges, Boolean> User2GroupEdgeFilter)
            => _User2GroupEdges.Where(edge => User2GroupEdgeFilter(edge.EdgeLabel));


        #region Organizations(RequireAdminAccess, RequireReadWriteAccess, Recursive)

        public IEnumerable<Organization> Organizations(Access_Level  AccessLevel,
                                                       Boolean       Recursive)
        {

            var AllMyOrganizations = new HashSet<Organization>();

            switch (AccessLevel)
            {

                case Access_Level.Admin:
                    foreach (var organization in _User2OrganizationEdges.
                                                     Where (edge => edge.EdgeLabel == User2OrganizationEdges.IsAdmin).
                                                     Select(edge => edge.Target))
                    {
                        AllMyOrganizations.Add(organization);
                    }
                    break;

                case Access_Level.ReadWrite:
                    foreach (var organization in _User2OrganizationEdges.
                                                     Where (edge => edge.EdgeLabel == User2OrganizationEdges.IsAdmin ||
                                                                    edge.EdgeLabel == User2OrganizationEdges.IsMember).
                                                     Select(edge => edge.Target))
                    {
                        AllMyOrganizations.Add(organization);
                    }
                    break;

                default:
                    foreach (var organization in _User2OrganizationEdges.
                                                     Where (edge => edge.EdgeLabel == User2OrganizationEdges.IsAdmin  ||
                                                                    edge.EdgeLabel == User2OrganizationEdges.IsMember ||
                                                                    edge.EdgeLabel == User2OrganizationEdges.IsVisitor).
                                                     Select(edge => edge.Target))
                    {
                        AllMyOrganizations.Add(organization);
                    }
                    break;

            }


            if (Recursive)
            {

                Organization[] Level2 = null;

                do
                {

                    Level2 = AllMyOrganizations.SelectMany(organization => organization.
                                                                               Organization2OrganizationInEdges.
                                                                               Where(edge => edge.EdgeLabel == Organization2OrganizationEdges.IsChildOf)).
                                                Select    (edge         => edge.Source).
                                                Where     (organization => !AllMyOrganizations.Contains(organization)).
                                                ToArray();

                    foreach (var organization in Level2)
                        AllMyOrganizations.Add(organization);

                } while (Level2.Length > 0);

            }

            return AllMyOrganizations;

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

                   new JProperty("@id",                  Id.ToString()),
                   new JProperty("@context",             JSONLDContext),
                   new JProperty("name",                 Name),
                   new JProperty("email",                EMail.Address.ToString()),

                   MobilePhone.HasValue
                       ? new JProperty("mobilePhone",    MobilePhone.ToString())
                       : null,

                   Description.IsNeitherNullNorEmpty()
                       ? new JProperty("description",    Description.ToJSON())
                       : null,

                   PublicKeyRing != null
                       ? new JProperty("publicKeyRing",  PublicKeyRing.GetEncoded().ToHexString())
                       : null,

                   SecretKeyRing != null
                       ? new JProperty("secretKeyRing",  SecretKeyRing.GetEncoded().ToHexString())
                       : null,

                   PrivacyLevel.ToJSON(),
                   new JProperty("isAuthenticated",      IsAuthenticated),
                   new JProperty("isDisabled",           IsDisabled),

                   new JProperty("signatures",           new JArray()),

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

                #region Parse Name             [mandatory]

                if (!JSONObject.ParseMandatory("name",
                                               "Username",
                                               out String Name,
                                               out ErrorResponse))
                {
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

                #region Parse mobilePhone      [optional]

                if (JSONObject.ParseOptional("mobilePhone",
                                             "mobile phone number",
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

                #region Parse PublicKeyRing    [optional]

                if (JSONObject.ParseOptional("publicKeyRing",
                                             "GPG/PGP public key ring",
                                             txt => OpenPGP.ReadPublicKeyRing(txt.HexStringToByteArray()),
                                             out PgpPublicKeyRing PublicKeyRing,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse SecretKeyRing    [optional]

                if (JSONObject.ParseOptional("secretKeyRing",
                                             "GPG/PGP secret key ring",
                                             txt => OpenPGP.ReadSecretKeyRing(txt.HexStringToByteArray()),
                                             out PgpSecretKeyRing SecretKeyRing,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse GeoLocation      [optional]

                if (!JSONObject.ParseOptionalN("geoLocation",
                                               "Geo location",
                                               GeoCoordinate.TryParseJSON,
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
                                             Vanaheimr.Illias.Address.Parse,
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
                                Description,
                                PublicKeyRing,
                                SecretKeyRing,
                                Telephone,
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


        #region Operator overloading

        #region Operator == (UserId1, UserId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserId1">A user identification.</param>
        /// <param name="UserId2">Another user identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (User UserId1, User UserId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(UserId1, UserId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) UserId1 == null) || ((Object) UserId2 == null))
                return false;

            return UserId1.Equals(UserId2);

        }

        #endregion

        #region Operator != (UserId1, UserId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserId1">A user identification.</param>
        /// <param name="UserId2">Another user identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (User UserId1, User UserId2)
            => !(UserId1 == UserId2);

        #endregion

        #region Operator <  (UserId1, UserId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserId1">A user identification.</param>
        /// <param name="UserId2">Another user identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (User UserId1, User UserId2)
        {

            if ((Object) UserId1 == null)
                throw new ArgumentNullException(nameof(UserId1), "The given UserId1 must not be null!");

            return UserId1.CompareTo(UserId2) < 0;

        }

        #endregion

        #region Operator <= (UserId1, UserId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserId1">A user identification.</param>
        /// <param name="UserId2">Another user identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (User UserId1, User UserId2)
            => !(UserId1 > UserId2);

        #endregion

        #region Operator >  (UserId1, UserId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserId1">A user identification.</param>
        /// <param name="UserId2">Another user identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (User UserId1, User UserId2)
        {

            if ((Object) UserId1 == null)
                throw new ArgumentNullException(nameof(UserId1), "The given UserId1 must not be null!");

            return UserId1.CompareTo(UserId2) > 0;

        }

        #endregion

        #region Operator >= (UserId1, UserId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserId1">A user identification.</param>
        /// <param name="UserId2">Another user identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (User UserId1, User UserId2)
            => !(UserId1 < UserId2);

        #endregion

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
                           Description,
                           PublicKeyRing,
                           SecretKeyRing,
                           MobilePhone,
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
            /// An optional (multi-language) description of the user.
            /// </summary>
            [Optional]
            public I18NString          Description          { get; set; }

            /// <summary>
            /// The PGP/GPG public keyring of the user.
            /// </summary>
            [Optional]
            public PgpPublicKeyRing    PublicKeyRing        { get; set; }

            /// <summary>
            /// The PGP/GPG secret keyring of the user.
            /// </summary>
            [Optional]
            public PgpSecretKeyRing    SecretKeyRing        { get; set; }

            /// <summary>
            /// The mobile telephone number of the user.
            /// </summary>
            [Optional]
            public PhoneNumber?        MobilePhone          { get; set; }

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
            /// <param name="Description">An optional (multi-language) description of the user.</param>
            /// <param name="PublicKeyRing">An optional PGP/GPG public keyring of the user.</param>
            /// <param name="SecretKeyRing">An optional PGP/GPG secret keyring of the user.</param>
            /// <param name="Telephone">An optional telephone number of the user.</param>
            /// <param name="GeoLocation">An optional geographical location of the user.</param>
            /// <param name="Address">An optional address of the user.</param>
            /// <param name="PrivacyLevel">Whether the user will be shown in user listings, or not.</param>
            /// <param name="IsDisabled">The user is disabled.</param>
            /// <param name="IsAuthenticated">The user will not be shown in user listings, as its primary e-mail address is not yet authenticated.</param>
            public Builder(User_Id             Id,
                           SimpleEMailAddress  EMail,
                           String              Name              = null,
                           I18NString          Description       = null,
                           PgpPublicKeyRing    PublicKeyRing     = null,
                           PgpSecretKeyRing    SecretKeyRing     = null,
                           PhoneNumber?        Telephone         = null,
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
                this.Description              = Description ?? new I18NString();
                this.PublicKeyRing            = PublicKeyRing;
                this.SecretKeyRing            = SecretKeyRing;
                this.MobilePhone                = Telephone;
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
                            Description,
                            PublicKeyRing,
                            SecretKeyRing,
                            MobilePhone,
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
