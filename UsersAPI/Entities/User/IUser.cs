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

using Newtonsoft.Json.Linq;
using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Illias;
using Org.BouncyCastle.Bcpg.OpenPgp;
using social.OpenData.UsersAPI.Notifications;

namespace social.OpenData.UsersAPI
{
    public interface IUser : IEntity<User_Id>,
                             IEquatable<IUser>,
                             IComparable<IUser>,
                             IComparable
    {

        UsersAPI? API { get; set; }
        IEnumerable<User2UserEdge> __User2UserEdges { get; }
        DateTime? AcceptedEULA { get; }
        Address? Address { get; }
        IEnumerable<AttachedFile> AttachedFiles { get; }
        EMailAddress EMail { get; }
        IEnumerable<User> FollowsUsers { get; }
        IEnumerable<User> Genimi { get; }
        GeoCoordinate? GeoLocation { get; }
        string? Homepage { get; }
        bool IsAuthenticated { get; }
        bool IsDisabled { get; }
        IEnumerable<User> IsFollowedBy { get; }
        PhoneNumber? MobilePhone { get; }
        PrivacyLevel PrivacyLevel { get; }
        PgpPublicKeyRing? PublicKeyRing { get; }
        PgpSecretKeyRing? SecretKeyRing { get; }
        string? Telegram { get; }
        PhoneNumber? Telephone { get; }
        Use2AuthFactor Use2AuthFactor { get; }
        IEnumerable<User2UserGroupEdge> User2Group_OutEdges { get; }
        IEnumerable<User2OrganizationEdge> User2Organization_OutEdges { get; }
        Languages UserLanguage { get; }

        IEnumerable<User2OrganizationEdge> Add(IEnumerable<User2OrganizationEdge> Edges);
        IEnumerable<User2UserEdge> Add(IEnumerable<User2UserEdge> Edges);
        IEnumerable<User2UserGroupEdge> Add(IEnumerable<User2UserGroupEdge> Edges);
        User2OrganizationEdge Add(User2OrganizationEdge Edge);
        User2UserEdge Add(User2UserEdge Edge);
        User2UserGroupEdge Add(User2UserGroupEdge Edge);
        User2UserEdge AddIncomingEdge(User SourceUser, User2UserEdgeTypes EdgeLabel, PrivacyLevel PrivacyLevel = PrivacyLevel.World);
        User2UserEdge AddIncomingEdge(User2UserEdge Edge);
        User2OrganizationEdge AddOutgoingEdge(User2OrganizationEdgeLabel EdgeLabel, Organization Target, PrivacyLevel PrivacyLevel = PrivacyLevel.World);
        User2UserEdge AddOutgoingEdge(User2UserEdge Edge);
        User2UserEdge AddOutgoingEdge(User2UserEdgeTypes EdgeLabel, User Target, PrivacyLevel PrivacyLevel = PrivacyLevel.World);
        User2UserGroupEdge AddToUserGroup(User2UserGroupEdgeLabel EdgeLabel, UserGroup Target, PrivacyLevel PrivacyLevel = PrivacyLevel.World);
        User Clone(User_Id? NewUserId = null);
        int CompareTo(User User);
        void CopyAllLinkedDataFrom(User OldUser);
        IEnumerable<User2OrganizationEdgeLabel> EdgeLabels(Organization Organization);
        IEnumerable<User2UserGroupEdgeLabel> EdgeLabels(UserGroup UserGroup);
        IEnumerable<User2OrganizationEdge> Edges(Organization Organization);
        IEnumerable<User2OrganizationEdge> Edges(Organization Organization, User2OrganizationEdgeLabel EdgeLabel);
        IEnumerable<User2UserGroupEdge> Edges(User2UserGroupEdgeLabel EdgeLabel, UserGroup UserGroup);
        IEnumerable<User2UserGroupEdge> Edges(UserGroup UserGroup);
        bool Equals(object? Object);
        bool Equals(User User);
        int GetHashCode();
        JObject GetNotificationInfo(uint NotificationId);
        JObject GetNotificationInfos();
        IEnumerable<ANotification> GetNotifications(NotificationMessageType? NotificationMessageType = null);
        IEnumerable<ANotification> GetNotifications(Func<NotificationMessageType, bool> NotificationMessageTypeFilter);
        IEnumerable<T> GetNotificationsOf<T>(Func<NotificationMessageType, bool> NotificationMessageTypeFilter) where T : ANotification;
        IEnumerable<T> GetNotificationsOf<T>(params NotificationMessageType[] NotificationMessageTypes) where T : ANotification;
        bool HasAccessToOrganization(Access_Levels AccessLevel, Organization Organization, bool Recursive = true);
        bool HasAccessToOrganization(Access_Levels AccessLevel, Organization_Id OrganizationId, bool Recursive = true);
        bool HasEdge(User2UserGroupEdgeLabel EdgeLabel, UserGroup UserGroup);
        IEnumerable<Organization> Organizations(Access_Levels AccessLevel, bool Recursive = true);
        IEnumerable<Organization> ParentOrganizations();
        bool RemoveOutEdge(User2OrganizationEdge Edge);
        bool RemoveOutEdge(User2UserGroupEdge Edge);
        User.Builder ToBuilder(User_Id? NewUserId = null);
        JObject ToJSON(bool Embedded = false, InfoStatus ExpandOrganizations = InfoStatus.Hidden, InfoStatus ExpandGroups = InfoStatus.Hidden, bool IncludeLastChange = true, CustomJObjectSerializerDelegate<User>? CustomUserSerializer = null);
        string ToString();
        IEnumerable<User2UserGroupEdge> User2GroupOutEdges(Func<User2UserGroupEdgeLabel, bool> User2GroupEdgeFilter);
        IEnumerable<UserGroup> UserGroups(bool RequireReadWriteAccess = false, bool Recursive = false);
        IEnumerable<UserGroup> UserGroups(User2UserGroupEdgeLabel EdgeFilter);
    }
}