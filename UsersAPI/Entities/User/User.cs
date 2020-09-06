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
using social.OpenData.UsersAPI;

#endregion

namespace social.OpenData.UsersAPI
{

    public delegate Boolean UserProviderDelegate(User_Id UserId, out User User);

    public delegate JObject UserToJSONDelegate(User     User,
                                               Boolean  Embedded            = false,
                                               Boolean  IncludeCryptoHash   = true);

    public delegate User OverwriteUserDelegate(User User);


    /// <summary>
    /// Extention methods for Users.
    /// </summary>
    public static class UserExtentions
    {

        #region ToJSON(this Users, Skip = null, Take = null, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of Users.
        /// </summary>
        /// <param name="Users">An enumeration of Users.</param>
        /// <param name="Skip">The optional number of Users to skip.</param>
        /// <param name="Take">The optional number of Users to return.</param>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        public static JArray ToJSON(this IEnumerable<User>  Users,
                                    UInt64?                 Skip                = null,
                                    UInt64?                 Take                = null,
                                    Boolean                 Embedded            = false,
                                    UserToJSONDelegate      UserToJSON          = null,
                                    Boolean                 IncludeCryptoHash   = true)


            => Users?.Any() != true

                   ? new JArray()

                   : new JArray(Users.
                                    Where     (dataSet =>  dataSet != null).
                                    OrderBy   (dataSet => dataSet.Id).
                                    SkipTakeFilter(Skip, Take).
                                    SafeSelect(User => UserToJSON != null
                                                                    ? UserToJSON (User,
                                                                                  Embedded,
                                                                                  IncludeCryptoHash)

                                                                    : User.ToJSON(Embedded,
                                                                                  IncludeCryptoHash)));

        #endregion

    }



    /// <summary>
    /// An Open Data user.
    /// </summary>
    public class User : ADistributedEntity<User_Id>,
                        IEntityClass<User>
    {

        #region Data

        /// <summary>
        /// The default max size of the aggregated EVSE operator status history.
        /// </summary>
        public const UInt16 DefaultUserStatusHistorySize = 50;

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        public const String JSONLDContext  = "https://opendata.social/contexts/UsersAPI/user";

        #endregion

        #region Properties

        #region API

        private UsersAPI _API;

        /// <summary>
        /// The UsersAPI of this user.
        /// </summary>
        internal UsersAPI API
        {

            get
            {
                return _API;
            }

            set
            {

                if (_API == value)
                    return;

                if (_API != null)
                    throw new ArgumentException("Illegal attempt to change the API of this user!");

                _API = value ?? throw new ArgumentException("Illegal attempt to delete the API reference of this user!");

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
        /// The language setting of the user.
        /// </summary>
        [Mandatory]
        public Languages           UserLanguage         { get; }

        /// <summary>
        /// The telephone number of the user.
        /// </summary>
        [Optional]
        public PhoneNumber?        Telephone            { get; }

        /// <summary>
        /// The mobile telephone number of the user.
        /// </summary>
        [Optional]
        public PhoneNumber?        MobilePhone          { get; }

        /// <summary>
        /// The telegram user name.
        /// </summary>
        [Optional]
        public String              Telegram             { get; }

        /// <summary>
        /// The homepage of the user.
        /// </summary>
        [Optional]
        public String              Homepage             { get; }

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
        /// Timestamp when the user accepted the End-User-License-Agreement.
        /// </summary>
        [Mandatory]
        public DateTime?           AcceptedEULA         { get; }

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

        #endregion

        #region Edges

        #region User <-> User         edges

        private readonly List<User2UserEdge> _User2UserEdges;

        public IEnumerable<User2UserEdge> __User2UserEdges
            => _User2UserEdges;


        public User2UserEdge

            Add(User2UserEdge Edge)

                => _User2UserEdges.AddAndReturnElement(Edge);


        public IEnumerable<User2UserEdge>

            Add(IEnumerable<User2UserEdge> Edges)

                => _User2UserEdges.AddAndReturnList(Edges);



        #region AddIncomingEdge(Edge)

        public User2UserEdge

            AddIncomingEdge(User2UserEdge  Edge)

            => _User2UserEdges.AddAndReturnElement(Edge);

        #endregion

        #region AddIncomingEdge(SourceUser, EdgeLabel, PrivacyLevel = PrivacyLevel.World)

        public User2UserEdge

            AddIncomingEdge(User            SourceUser,
                            User2UserEdgeTypes  EdgeLabel,
                            PrivacyLevel    PrivacyLevel = PrivacyLevel.World)

            => _User2UserEdges.AddAndReturnElement(new User2UserEdge(SourceUser, EdgeLabel, this, PrivacyLevel));

        #endregion

        #region AddOutgoingEdge(Edge)

        public User2UserEdge

            AddOutgoingEdge(User2UserEdge  Edge)

            => _User2UserEdges.AddAndReturnElement(Edge);

        #endregion

        #region AddOutgoingEdge(SourceUser, EdgeLabel, PrivacyLevel = PrivacyLevel.World)

        public User2UserEdge

            AddOutgoingEdge(User2UserEdgeTypes  EdgeLabel,
                            User            Target,
                            PrivacyLevel    PrivacyLevel = PrivacyLevel.World)

            => _User2UserEdges.AddAndReturnElement(new User2UserEdge(this, EdgeLabel, Target, PrivacyLevel));

        #endregion

        #endregion

        #region User  -> Organization edges

        private readonly List<User2OrganizationEdge> _User2Organization_OutEdges;

        public IEnumerable<User2OrganizationEdge> User2Organization_OutEdges
            => _User2Organization_OutEdges;


        public User2OrganizationEdge

            Add(User2OrganizationEdge Edge)

                => _User2Organization_OutEdges.AddAndReturnElement(Edge);


        public IEnumerable<User2OrganizationEdge>

            Add(IEnumerable<User2OrganizationEdge> Edges)

                => _User2Organization_OutEdges.AddAndReturnList(Edges);




        public User2OrganizationEdge

            AddOutgoingEdge(User2OrganizationEdgeTypes  EdgeLabel,
                            Organization            Target,
                            PrivacyLevel            PrivacyLevel = PrivacyLevel.World)

            => _User2Organization_OutEdges.AddAndReturnElement(new User2OrganizationEdge(this, EdgeLabel, Target, PrivacyLevel));



        public User2GroupEdge

            AddOutgoingEdge(User2GroupEdgeTypes EdgeLabel,
                            Group           Target,
                            PrivacyLevel    PrivacyLevel = PrivacyLevel.World)

            => _User2Group_OutEdges.AddAndReturnElement(new User2GroupEdge(this, EdgeLabel, Target, PrivacyLevel));


        public IEnumerable<User2GroupEdge> User2GroupOutEdges(Func<User2GroupEdgeTypes, Boolean> User2GroupEdgeFilter)
            => _User2Group_OutEdges.Where(edge => User2GroupEdgeFilter(edge.EdgeLabel));


        #region Organizations(RequireAdminAccess, RequireReadWriteAccess, Recursive)

        public IEnumerable<Organization> Organizations(Access_Levels  AccessLevel,
                                                       Boolean        Recursive)
        {

            var AllMyOrganizations = new HashSet<Organization>();

            switch (AccessLevel)
            {

                case Access_Levels.Admin:
                    foreach (var organization in _User2Organization_OutEdges.
                                                     Where (edge => edge.EdgeLabel == User2OrganizationEdgeTypes.IsAdmin).
                                                     Select(edge => edge.Target))
                    {
                        AllMyOrganizations.Add(organization);
                    }
                    break;

                case Access_Levels.ReadWrite:
                    foreach (var organization in _User2Organization_OutEdges.
                                                     Where (edge => edge.EdgeLabel == User2OrganizationEdgeTypes.IsAdmin ||
                                                                    edge.EdgeLabel == User2OrganizationEdgeTypes.IsMember).
                                                     Select(edge => edge.Target))
                    {
                        AllMyOrganizations.Add(organization);
                    }
                    break;

                default:
                    foreach (var organization in _User2Organization_OutEdges.
                                                     Where (edge => edge.EdgeLabel == User2OrganizationEdgeTypes.IsAdmin  ||
                                                                    edge.EdgeLabel == User2OrganizationEdgeTypes.IsMember ||
                                                                    edge.EdgeLabel == User2OrganizationEdgeTypes.IsGuest).
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
                                                                               Where(edge => edge.EdgeLabel == Organization2OrganizationEdgeTypes.IsChildOf)).
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

        public Boolean RemoveOutEdge(User2OrganizationEdge Edge)
            => _User2Organization_OutEdges.Remove(Edge);

        #endregion

        #region User  -> Group        edges

        private readonly List<User2GroupEdge> _User2Group_OutEdges;

        public IEnumerable<User2GroupEdge> User2Group_OutEdges
            => _User2Group_OutEdges;



        public User2GroupEdge

            Add(User2GroupEdge Edge)

                => _User2Group_OutEdges.AddAndReturnElement(Edge);


        public IEnumerable<User2GroupEdge>

            Add(IEnumerable<User2GroupEdge> Edges)

                => _User2Group_OutEdges.AddAndReturnList(Edges);



        #region Groups(RequireReadWriteAccess = false, Recursive = false)

        public IEnumerable<Group> Groups(Boolean RequireReadWriteAccess  = false,
                                         Boolean Recursive               = false)
        {

            var _Groups = RequireReadWriteAccess

                                     ? _User2Group_OutEdges.
                                           Where (edge => edge.EdgeLabel == User2GroupEdgeTypes.IsAdmin_ReadWrite ||
                                                          edge.EdgeLabel == User2GroupEdgeTypes.IsMember).
                                           Select(edge => edge.Target).
                                           ToList()

                                     : _User2Group_OutEdges.
                                           Where (edge => edge.EdgeLabel == User2GroupEdgeTypes.IsAdmin_ReadWrite  ||
                                                          edge.EdgeLabel == User2GroupEdgeTypes.IsMember ||
                                                          edge.EdgeLabel == User2GroupEdgeTypes.IsVisitor).
                                           Select(edge => edge.Target).
                                           ToList();

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

            return new HashSet<Group>(_Groups);

        }

        #endregion

        public Boolean RemoveOutEdge(User2GroupEdge Edge)
            => _User2Group_OutEdges.Remove(Edge);

        #endregion


        #region Genimi

        /// <summary>
        /// The gemini of this user.
        /// </summary>
        public IEnumerable<User> Genimi
        {
            get
            {
                return _User2UserEdges.
                           Where (edge => edge.EdgeLabel == User2UserEdgeTypes.gemini).
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
                           Where(edge => edge.EdgeLabel == User2UserEdgeTypes.follows).
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
                           Where(edge => edge.EdgeLabel == User2UserEdgeTypes.IsFollowedBy).
                           Select(edge => edge.Target);
            }
        }

        #endregion

        #region Groups()

        /// <summary>
        /// All groups this user belongs to.
        /// </summary>
        public IEnumerable<Group> Groups()
            => _User2Group_OutEdges.
                   Select(edge => edge.Target);

        #endregion

        #region Groups(EdgeFilter)

        /// <summary>
        /// All groups this user belongs to,
        /// filtered by the given edge label.
        /// </summary>
        public IEnumerable<Group> Groups(User2GroupEdgeTypes EdgeFilter)
            => _User2Group_OutEdges.
                   Where (edge => edge.EdgeLabel == EdgeFilter).
                   Select(edge => edge.Target);

        #endregion

        #region Edges(Group)

        /// <summary>
        /// All groups this user belongs to,
        /// filtered by the given edge label.
        /// </summary>
        public IEnumerable<User2GroupEdgeTypes> OutEdges(Group Group)
            => _User2Group_OutEdges.
                   Where (edge => edge.Target == Group).
                   Select(edge => edge.EdgeLabel);

        #endregion

        #region Edges(Organization)

        /// <summary>
        /// All organizations this user belongs to,
        /// filtered by the given edge label.
        /// </summary>
        public IEnumerable<User2OrganizationEdgeTypes> Edges(Organization Organization)
            => _User2Organization_OutEdges.
                   Where (edge => edge.Target == Organization).
                   Select(edge => edge.EdgeLabel);

        #endregion

        #endregion

        #region Events

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new Open Data user.
        /// </summary>
        /// <param name="Id">The unique identification of the user.</param>
        /// <param name="EMail">The primary e-mail of the user.</param>
        /// <param name="Name">An offical (multi-language) name of the user.</param>
        /// <param name="Description">An optional (multi-language) description of the user.</param>
        /// <param name="PublicKeyRing">An optional PGP/GPG public keyring of the user.</param>
        /// <param name="SecretKeyRing">An optional PGP/GPG secret keyring of the user.</param>
        /// <param name="UserLanguage">The language setting of the user.</param>
        /// <param name="Telephone">An optional telephone number of the user.</param>
        /// <param name="MobilePhone">An optional mobile telephone number of the user.</param>
        /// <param name="Telegram">The telegram user name.</param>
        /// <param name="Homepage">The homepage of the user.</param>
        /// <param name="GeoLocation">An optional geographical location of the user.</param>
        /// <param name="Address">An optional address of the user.</param>
        /// <param name="PrivacyLevel">Whether the user will be shown in user listings, or not.</param>
        /// <param name="AcceptedEULA">Timestamp when the user accepted the End-User-License-Agreement.</param>
        /// <param name="IsDisabled">The user is disabled.</param>
        /// <param name="IsAuthenticated">The user will not be shown in user listings, as its primary e-mail address is not yet authenticated.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        internal User(User_Id                             Id,
                      SimpleEMailAddress                  EMail,
                      String                              Name                     = null,
                      I18NString                          Description              = null,
                      PgpPublicKeyRing                    PublicKeyRing            = null,
                      PgpSecretKeyRing                    SecretKeyRing            = null,
                      Languages                           UserLanguage             = Languages.eng,
                      PhoneNumber?                        Telephone                = null,
                      PhoneNumber?                        MobilePhone              = null,
                      String                              Telegram                 = null,
                      String                              Homepage                 = null,
                      GeoCoordinate?                      GeoLocation              = null,
                      Address                             Address                  = null,
                      PrivacyLevel?                       PrivacyLevel             = null,
                      DateTime?                           AcceptedEULA             = null,
                      Boolean                             IsDisabled               = false,
                      Boolean                             IsAuthenticated          = false,
                      String                              DataSource               = "",

                      IEnumerable<ANotification>          Notifications            = null,

                      IEnumerable<User2UserEdge>          User2UserEdges           = null,
                      IEnumerable<User2GroupEdge>         User2GroupEdges          = null,
                      IEnumerable<User2OrganizationEdge>  User2OrganizationEdges   = null)

            : base(Id,
                   DataSource)

        {

            #region Initial checks

            if (Name != null)
                Name = Name.Trim();

            if (Name.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(Name), "The given username must not be null or empty!");

            #endregion

            this.EMail                        = Name.IsNotNullOrEmpty()
                                                    ? new EMailAddress(Name, EMail, SecretKeyRing, PublicKeyRing)
                                                    : new EMailAddress(      EMail, SecretKeyRing, PublicKeyRing);
            this.Name                         = Name;
            this.PublicKeyRing                = PublicKeyRing;
            this.SecretKeyRing                = SecretKeyRing;
            this.UserLanguage                 = UserLanguage;
            this.Telephone                    = Telephone;
            this.MobilePhone                  = MobilePhone;
            this.Telegram                     = Telegram;
            this.Homepage                     = Homepage;
            this.Description                  = Description  ?? new I18NString();
            this.GeoLocation                  = GeoLocation;
            this.Address                      = Address;
            this.PrivacyLevel                 = PrivacyLevel ?? social.OpenData.UsersAPI.PrivacyLevel.Private;
            this.AcceptedEULA                 = AcceptedEULA;
            this.IsAuthenticated              = IsAuthenticated;
            this.IsDisabled                   = IsDisabled;

            this._NotificationStore           = new NotificationStore();

            if (Notifications.SafeAny())
                _NotificationStore.Add(Notifications);

            // Init edges
            this._User2UserEdges              = User2UserEdges.        IsNeitherNullNorEmpty() ? new List<User2UserEdge>        (User2UserEdges)         : new List<User2UserEdge>();
            this._User2Group_OutEdges         = User2GroupEdges.       IsNeitherNullNorEmpty() ? new List<User2GroupEdge>       (User2GroupEdges)        : new List<User2GroupEdge>();
            this._User2Organization_OutEdges  = User2OrganizationEdges.IsNeitherNullNorEmpty() ? new List<User2OrganizationEdge>(User2OrganizationEdges) : new List<User2OrganizationEdge>();

            CalcHash();

        }

        #endregion


        #region Notifications

        private readonly NotificationStore _NotificationStore;

        #region (internal) AddNotification(Notification,                           OnUpdate = null)

        internal T AddNotification<T>(T          Notification,
                                      Action<T>  OnUpdate  = null)

            where T : ANotification

            => _NotificationStore.Add(Notification,
                                  OnUpdate);

        #endregion

        #region (internal) AddNotification(Notification, NotificationMessageType,  OnUpdate = null)

        internal T AddNotification<T>(T                        Notification,
                                      NotificationMessageType  NotificationMessageType,
                                      Action<T>                OnUpdate  = null)

            where T : ANotification

            => _NotificationStore.Add(Notification,
                                  NotificationMessageType,
                                  OnUpdate);

        #endregion

        #region (internal) AddNotification(Notification, NotificationMessageTypes, OnUpdate = null)

        internal T AddNotification<T>(T                                     Notification,
                                      IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                      Action<T>                             OnUpdate  = null)

            where T : ANotification

            => _NotificationStore.Add(Notification,
                                  NotificationMessageTypes,
                                  OnUpdate);

        #endregion


        #region GetNotifications  (NotificationMessageType = null)

        public IEnumerable<ANotification> GetNotifications(NotificationMessageType?  NotificationMessageType = null)
        {
            lock (_NotificationStore)
            {
                return _NotificationStore.GetNotifications(NotificationMessageType);
            }
        }

        #endregion

        #region GetNotificationsOf(params NotificationMessageTypes)

        public IEnumerable<T> GetNotificationsOf<T>(params NotificationMessageType[] NotificationMessageTypes)

            where T : ANotification

        {

            lock (_NotificationStore)
            {
                return _NotificationStore.GetNotificationsOf<T>(NotificationMessageTypes);
            }

        }

        #endregion

        #region GetNotifications  (NotificationMessageTypeFilter)

        public IEnumerable<ANotification> GetNotifications(Func<NotificationMessageType, Boolean> NotificationMessageTypeFilter)
        {
            lock (_NotificationStore)
            {
                return _NotificationStore.GetNotifications(NotificationMessageTypeFilter);
            }
        }

        #endregion

        #region GetNotificationsOf(NotificationMessageTypeFilter)

        public IEnumerable<T> GetNotificationsOf<T>(Func<NotificationMessageType, Boolean> NotificationMessageTypeFilter)

            where T : ANotification

        {

            lock (_NotificationStore)
            {
                return _NotificationStore.GetNotificationsOf<T>(NotificationMessageTypeFilter);
            }

        }

        #endregion


        #region GetNotificationInfo(NotificationId)

        public JObject GetNotificationInfo(UInt32 NotificationId)
        {

            var notification = _NotificationStore.ToJSON(NotificationId);

            notification.Add(new JProperty("user", JSONObject.Create(

                                     new JProperty("name",  EMail.OwnerName),
                                     new JProperty("email", EMail.Address.ToString()),

                                     MobilePhone.HasValue
                                         ? new JProperty("phoneNumber", MobilePhone.Value.ToString())
                                         : null

                                 )));

            return notification;

        }

        #endregion

        #region GetNotificationInfos()

        public JObject GetNotificationInfos()

            => JSONObject.Create(new JProperty("user", JSONObject.Create(

                                     new JProperty("name",               EMail.OwnerName),
                                     new JProperty("email",              EMail.Address.ToString()),

                                     MobilePhone.HasValue
                                         ? new JProperty("phoneNumber",  MobilePhone.Value.ToString())
                                         : null

                                 )),
                                 new JProperty("notifications",  _NotificationStore.ToJSON()));

        #endregion


        #region (internal) RemoveNotification(NotificationType,                           OnRemoval = null)

        internal T RemoveNotification<T>(T          NotificationType,
                                         Action<T>  OnRemoval  = null)

            where T : ANotification

            => _NotificationStore.Remove(NotificationType,
                                     OnRemoval);

        #endregion

        #endregion


        #region ToJSON(Embedded = false, IncludeCryptoHash = false)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        /// <param name="IncludeCryptoHash">Include the crypto hash value of this object.</param>
        public override JObject ToJSON(Boolean Embedded           = false,
                                       Boolean IncludeCryptoHash  = false)

            => ToJSON(Embedded,
                      InfoStatus.Hidden,
                      InfoStatus.Hidden,
                      false);


        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        /// <param name="ExpandOrganizations">Whether to expand the organizations this user is a member of.</param>
        /// <param name="ExpandGroups">Whether to expand the groups this user is a member of.</param>
        /// <param name="IncludeCryptoHash">Include the crypto hash value of this object.</param>
        public JObject ToJSON(Boolean     Embedded               = false,
                              InfoStatus  ExpandOrganizations   = InfoStatus.Hidden,
                              InfoStatus  ExpandGroups          = InfoStatus.Hidden,
                              Boolean     IncludeCryptoHash      = false)

            => JSONObject.Create(

                   new JProperty("@id",                  Id.ToString()),

                   !Embedded
                       ? new JProperty("@context",       JSONLDContext)
                       : null,

                   new JProperty("name",                 Name),

                   Description.IsNeitherNullNorEmpty()
                       ? new JProperty("description",    Description.ToJSON())
                       : null,

                   new JProperty("email",                EMail.Address.ToString()),

                   PublicKeyRing != null
                       ? new JProperty("publicKeyRing",  PublicKeyRing.GetEncoded().ToHexString())
                       : null,

                   SecretKeyRing != null
                       ? new JProperty("secretKeyRing",  SecretKeyRing.GetEncoded().ToHexString())
                       : null,

                   new JProperty("language",             UserLanguage.AsText()),

                   Telephone.HasValue
                       ? new JProperty("telephone",      Telephone.ToString())
                       : null,

                   MobilePhone.HasValue
                       ? new JProperty("mobilePhone",    MobilePhone.ToString())
                       : null,

                   Telegram.IsNotNullOrEmpty()
                       ? new JProperty("telegram",       Telegram)
                       : null,

                   Homepage.IsNotNullOrEmpty()
                       ? new JProperty("homepage",       Homepage.ToString())
                       : null,

                   PrivacyLevel.ToJSON(),

                   AcceptedEULA.HasValue
                       ? new JProperty("acceptedEULA",   AcceptedEULA.Value.ToIso8601())
                       : null,

                   new JProperty("isAuthenticated",      IsAuthenticated),
                   new JProperty("isDisabled",           IsDisabled),

                   //new JProperty("signatures",           new JArray()),

                   //ExpandOrganizations.Switch(
                   //    () => new JProperty("organizationIds",   Owner.Id.ToString()),
                   //    () => new JProperty("organizations",     Owner.ToJSON())),


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
                if (JSONObject.ParseOptionalStruct("@id",
                                                   "user identification",
                                                   User_Id.TryParse,
                                                   out User_Id? UserIdBody,
                                                   out ErrorResponse))
                {

                    if (ErrorResponse != null)
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

                if (!JSONObject.ParseMandatoryText("@context",
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

                if (!JSONObject.ParseMandatoryText("name",
                                                   "Username",
                                                   out String Name,
                                                   out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Description      [optional]

                if (JSONObject.ParseOptional("description",
                                             "user description",
                                             out I18NString Description,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse E-Mail           [mandatory]

                if (!JSONObject.ParseMandatory("email",
                                               "E-Mail",
                                               SimpleEMailAddress.TryParse,
                                               out SimpleEMailAddress EMail,
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

                #region Parse Language         [optional]

                if (!JSONObject.ParseOptional("language",
                                              "user language",
                                              LanguagesExtentions.Parse,
                                              out Languages? UserLanguage,
                                              out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Telephone        [optional]

                if (JSONObject.ParseOptional("telephone",
                                             "phone number",
                                             PhoneNumber.TryParse,
                                             out PhoneNumber? Telephone,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse MobilePhone      [optional]

                if (JSONObject.ParseOptional("mobilePhone",
                                             "mobile phone number",
                                             PhoneNumber.TryParse,
                                             out PhoneNumber? MobilePhone,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Telegram         [optional]

                if (JSONObject.ParseOptional("telegram",
                                             "telegram user name",
                                             out String Telegram,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                #region Parse Homepage         [optional]

                if (JSONObject.ParseOptional("homepage",
                                             "homepage",
                                             out String Homepage,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

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

                #region Parse Address          [optional]

                if (JSONObject.ParseOptionalJSON("address",
                                             "address",
                                             org.GraphDefined.Vanaheimr.Illias.Address.TryParse,
                                             out Address Address,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

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

                #region Parse AcceptedEULA     [optional]

                if (JSONObject.ParseOptional("acceptedEULA",
                                             "accepted EULA",
                                             out DateTime? AcceptedEULA,
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
                                UserLanguage ?? Languages.eng,
                                Telephone,
                                MobilePhone,
                                Telegram,
                                Homepage,
                                GeoLocation,
                                Address,
                                PrivacyLevel,
                                AcceptedEULA,
                                IsAuthenticated ?? false,
                                IsDisabled      ?? false,
                                DataSource);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse  = e.Message;
                User           = null;
                return false;
            }

        }

        #endregion


        #region CopyAllEdgesTo(NewUser)

        public void CopyAllEdgesTo(User NewUser)
        {

            if (__User2UserEdges.Any() && !NewUser.__User2UserEdges.Any())
            {

                NewUser.Add(__User2UserEdges);

                foreach (var edge in NewUser.__User2UserEdges)
                    edge.Source = NewUser;

            }

            if (User2Organization_OutEdges.Any() && !NewUser.User2Organization_OutEdges.Any())
            {

                NewUser.Add(User2Organization_OutEdges);

                foreach (var edge in NewUser.User2Organization_OutEdges)
                    edge.Source = NewUser;

            }

            if (User2Group_OutEdges.Any() && !NewUser.User2Group_OutEdges.Any())
            {

                NewUser.Add(User2Group_OutEdges);

                foreach (var edge in NewUser.User2Group_OutEdges)
                    edge.Source = NewUser;

            }

            if (_NotificationStore.SafeAny() && !NewUser._NotificationStore.SafeAny())
                NewUser._NotificationStore.Add(_NotificationStore);

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
        public override Int32 CompareTo(Object Object)
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

        #region (override) ToString()

        /// <summary>
        /// Return a text representation of this object.
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
                           UserLanguage,
                           Telephone,
                           MobilePhone,
                           Telegram,
                           Homepage,
                           GeoLocation,
                           Address,
                           PrivacyLevel,
                           AcceptedEULA,
                           IsDisabled,
                           IsAuthenticated,
                           DataSource,

                           _NotificationStore,

                           _User2UserEdges,
                           _User2Group_OutEdges,
                           _User2Organization_OutEdges);

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
            /// The language setting of the user.
            /// </summary>
            [Mandatory]
            public Languages           UserLanguage         { get; set; }

            /// <summary>
            /// An optional telephone number of the user.
            /// </summary>
            [Optional]
            public PhoneNumber?        Telephone            { get; set; }

            /// <summary>
            /// An optional mobile telephone number of the user.
            /// </summary>
            [Optional]
            public PhoneNumber?        MobilePhone          { get; set; }

            /// <summary>
            /// The telegram user name.
            /// </summary>
            [Optional]
            public String              Telegram             { get; set; }

            /// <summary>
            /// An optional homepage of the user.
            /// </summary>
            [Optional]
            public String              Homepage             { get; set; }

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
            /// Timestamp when the user accepted the End-User-License-Agreement.
            /// </summary>
            [Mandatory]
            public DateTime?           AcceptedEULA         { get; set; }

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

            /// <summary>
            /// The source of this information, e.g. an automatic importer.
            /// </summary>
            [Optional]
            public String              DataSource          { get; set; }

            #endregion

            #region Edges

            #region User2UserEdges

            private readonly List<User2UserEdge> _User2UserEdges;

            /// <summary>
            /// All PAIRED messages.
            /// </summary>
            public IEnumerable<User2UserEdge> __User2UserEdges
                => _User2UserEdges;

            /// <summary>
            /// Add a new PAIRED message.
            /// </summary>
            /// <param name="PairedMessage">A PAIRED message.</param>
            public User2UserEdge Add(User2UserEdge PairedMessage)
                => _User2UserEdges.AddAndReturnElement(PairedMessage);

            /// <summary>
            /// Add new PAIRED messages.
            /// </summary>
            /// <param name="PairedMessages">An enumeration of PAIRED messages.</param>
            public IEnumerable<User2UserEdge> Add(IEnumerable<User2UserEdge> PairedMessages)
                => _User2UserEdges.AddAndReturnList(PairedMessages);

            #endregion

            #region User2GroupEdges

            private readonly List<User2GroupEdge> _User2Group_OutEdges;

            /// <summary>
            /// All PAIRED messages.
            /// </summary>
            public IEnumerable<User2GroupEdge> __User2GroupEdges
                => _User2Group_OutEdges;

            /// <summary>
            /// Add a new PAIRED message.
            /// </summary>
            /// <param name="PairedMessage">A PAIRED message.</param>
            public User2GroupEdge Add(User2GroupEdge PairedMessage)
                => _User2Group_OutEdges.AddAndReturnElement(PairedMessage);

            /// <summary>
            /// Add new PAIRED messages.
            /// </summary>
            /// <param name="PairedMessages">An enumeration of PAIRED messages.</param>
            public IEnumerable<User2GroupEdge> Add(IEnumerable<User2GroupEdge> PairedMessages)
                => _User2Group_OutEdges.AddAndReturnList(PairedMessages);

            #endregion

            #region User2OrganizationEdges

            private readonly List<User2OrganizationEdge> _User2Organization_OutEdges;

            /// <summary>
            /// All PAIRED messages.
            /// </summary>
            public IEnumerable<User2OrganizationEdge> __User2OrganizationEdges
                => _User2Organization_OutEdges;

            /// <summary>
            /// Add a new PAIRED message.
            /// </summary>
            /// <param name="PairedMessage">A PAIRED message.</param>
            public User2OrganizationEdge Add(User2OrganizationEdge PairedMessage)
                => _User2Organization_OutEdges.AddAndReturnElement(PairedMessage);

            /// <summary>
            /// Add new PAIRED messages.
            /// </summary>
            /// <param name="PairedMessages">An enumeration of PAIRED messages.</param>
            public IEnumerable<User2OrganizationEdge> Add(IEnumerable<User2OrganizationEdge> PairedMessages)
                => _User2Organization_OutEdges.AddAndReturnList(PairedMessages);

            #endregion

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
            /// <param name="UserLanguage">The language setting of the user.</param>
            /// <param name="Telephone">An optional telephone number of the user.</param>
            /// <param name="MobilePhone">An optional telephone number of the user.</param>
            /// <param name="Homepage">The homepage of the user.</param>
            /// <param name="GeoLocation">An optional geographical location of the user.</param>
            /// <param name="Address">An optional address of the user.</param>
            /// <param name="PrivacyLevel">Whether the user will be shown in user listings, or not.</param>
            /// <param name="AcceptedEULA">Timestamp when the user accepted the End-User-License-Agreement.</param>
            /// <param name="IsDisabled">The user is disabled.</param>
            /// <param name="IsAuthenticated">The user will not be shown in user listings, as its primary e-mail address is not yet authenticated.</param>
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            public Builder(User_Id                             Id,
                           SimpleEMailAddress                  EMail,
                           String                              Name                     = null,
                           I18NString                          Description              = null,
                           PgpPublicKeyRing                    PublicKeyRing            = null,
                           PgpSecretKeyRing                    SecretKeyRing            = null,
                           Languages                           UserLanguage             = Languages.eng,
                           PhoneNumber?                        Telephone                = null,
                           PhoneNumber?                        MobilePhone              = null,
                           String                              Telegram                 = null,
                           String                              Homepage                 = null,
                           GeoCoordinate?                      GeoLocation              = null,
                           Address                             Address                  = null,
                           PrivacyLevel                        PrivacyLevel             = PrivacyLevel.World,
                           DateTime?                           AcceptedEULA             = null,
                           Boolean                             IsDisabled               = false,
                           Boolean                             IsAuthenticated          = false,
                           String                              DataSource               = "",

                           IEnumerable<ANotification>          Notifications            = null,

                           IEnumerable<User2UserEdge>          User2UserEdges           = null,
                           IEnumerable<User2GroupEdge>         User2GroupEdges          = null,
                           IEnumerable<User2OrganizationEdge>  User2OrganizationEdges   = null)

            {

                this.Id                           = Id;
                this.EMail                        = Name.IsNotNullOrEmpty()
                                                        ? new EMailAddress(Name, EMail, null, null)
                                                        : new EMailAddress(      EMail, null, null);
                this.Name                         = Name.IsNotNullOrEmpty()
                                                        ? Name
                                                        : "";
                this.Description                  = Description ?? new I18NString();
                this.PublicKeyRing                = PublicKeyRing;
                this.SecretKeyRing                = SecretKeyRing;
                this.UserLanguage                 = UserLanguage;
                this.Telephone                    = Telephone;
                this.MobilePhone                  = MobilePhone;
                this.Telegram                     = Telegram;
                this.Homepage                     = Homepage;
                this.GeoLocation                  = GeoLocation;
                this.Address                      = Address;
                this.PrivacyLevel                 = PrivacyLevel;
                this.AcceptedEULA                 = AcceptedEULA;
                this.IsDisabled                   = IsDisabled;
                this.IsAuthenticated              = IsAuthenticated;
                this.DataSource                   = DataSource;

                this._Notifications               = new NotificationStore();

                if (Notifications.SafeAny())
                    _Notifications.Add(Notifications);

                // Init edges
                this._User2UserEdges              = User2UserEdges.        IsNeitherNullNorEmpty() ? new List<User2UserEdge>        (User2UserEdges)         : new List<User2UserEdge>();
                this._User2Group_OutEdges         = User2GroupEdges.       IsNeitherNullNorEmpty() ? new List<User2GroupEdge>       (User2GroupEdges)        : new List<User2GroupEdge>();
                this._User2Organization_OutEdges  = User2OrganizationEdges.IsNeitherNullNorEmpty() ? new List<User2OrganizationEdge>(User2OrganizationEdges) : new List<User2OrganizationEdge>();

                if (Notifications.SafeAny())
                    _Notifications.Add(Notifications);

            }

            #endregion


            #region Notifications

            private readonly NotificationStore _Notifications;

            #region (internal) AddNotification(Notification,                           OnUpdate = null)

            internal T AddNotification<T>(T          Notification,
                                          Action<T>  OnUpdate  = null)

                where T : ANotification

                => _Notifications.Add(Notification,
                                      OnUpdate);

            #endregion

            #region (internal) AddNotification(Notification, NotificationMessageType,  OnUpdate = null)

            internal T AddNotification<T>(T                        Notification,
                                          NotificationMessageType  NotificationMessageType,
                                          Action<T>                OnUpdate  = null)

                where T : ANotification

                => _Notifications.Add(Notification,
                                      NotificationMessageType,
                                      OnUpdate);

            #endregion

            #region (internal) AddNotification(Notification, NotificationMessageTypes, OnUpdate = null)

            internal T AddNotification<T>(T                                     Notification,
                                          IEnumerable<NotificationMessageType>  NotificationMessageTypes,
                                          Action<T>                             OnUpdate  = null)

                where T : ANotification

                => _Notifications.Add(Notification,
                                      NotificationMessageTypes,
                                      OnUpdate);

            #endregion


            #region GetNotifications  (NotificationMessageType = null)

            public IEnumerable<ANotification> GetNotifications(NotificationMessageType?  NotificationMessageType = null)
            {
                lock (_Notifications)
                {
                    return _Notifications.GetNotifications(NotificationMessageType);
                }
            }

            #endregion

            #region GetNotificationsOf(params NotificationMessageTypes)

            public IEnumerable<T> GetNotificationsOf<T>(params NotificationMessageType[] NotificationMessageTypes)

                where T : ANotification

            {

                lock (_Notifications)
                {
                    return _Notifications.GetNotificationsOf<T>(NotificationMessageTypes);
                }

            }

            #endregion

            #region GetNotifications  (NotificationMessageTypeFilter)

            public IEnumerable<ANotification> GetNotifications(Func<NotificationMessageType, Boolean> NotificationMessageTypeFilter)
            {
                lock (_Notifications)
                {
                    return _Notifications.GetNotifications(NotificationMessageTypeFilter);
                }
            }

            #endregion

            #region GetNotificationsOf(NotificationMessageTypeFilter)

            public IEnumerable<T> GetNotificationsOf<T>(Func<NotificationMessageType, Boolean> NotificationMessageTypeFilter)

                where T : ANotification

            {

                lock (_Notifications)
                {
                    return _Notifications.GetNotificationsOf<T>(NotificationMessageTypeFilter);
                }

            }

            #endregion


            #region GetNotificationInfos()

            public JObject GetNotificationInfos()

                => JSONObject.Create(new JProperty("user", JSONObject.Create(

                                         new JProperty("name",               EMail.OwnerName),
                                         new JProperty("email",              EMail.Address.ToString()),

                                         MobilePhone.HasValue
                                             ? new JProperty("phoneNumber",  MobilePhone.Value.ToString())
                                             : null

                                     )),
                                     new JProperty("notifications",  _Notifications.ToJSON()));

            #endregion


            #region (internal) RemoveNotification(NotificationType,                           OnRemoval = null)

            internal T RemoveNotification<T>(T          NotificationType,
                                             Action<T>  OnRemoval  = null)

                where T : ANotification

                => _Notifications.Remove(NotificationType,
                                         OnRemoval);

            #endregion

            #endregion


            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the user.
            /// </summary>
            /// <param name="Builder">A user builder.</param>
            public static implicit operator User(Builder Builder)

                => Builder?.ToImmutable;


            /// <summary>
            /// Return an immutable version of the user.
            /// </summary>
            public User ToImmutable

                => new User(Id,
                            EMail.Address,
                            Name,
                            Description,
                            PublicKeyRing,
                            SecretKeyRing,
                            UserLanguage,
                            Telephone,
                            MobilePhone,
                            Telegram,
                            Homepage,
                            GeoLocation,
                            Address,
                            PrivacyLevel,
                            AcceptedEULA,
                            IsDisabled,
                            IsAuthenticated,
                            DataSource,

                            _Notifications,

                           _User2UserEdges,
                           _User2Group_OutEdges,
                           _User2Organization_OutEdges);

            #endregion

        }

        #endregion

    }

}
