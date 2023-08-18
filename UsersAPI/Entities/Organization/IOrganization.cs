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
using social.OpenData.UsersAPI.Notifications;

namespace social.OpenData.UsersAPI
{

    public interface IOrganization : IEntity<Organization_Id>,
                                     IEquatable<IOrganization>,
                                     IComparable<IOrganization>,
                                     IComparable
    {

        UsersAPI? API { get; set; }
        Address? Address { get; }
        IEnumerable<User> Admins { get; }
        IEnumerable<AttachedFile> AttachedFiles { get; }
        EMailAddress? EMail { get; }
        GeoCoordinate? GeoLocation { get; }
        IEnumerable<User> Guests { get; }
        bool IsDisabled { get; }
        IEnumerable<User> Members { get; }
        IEnumerable<Organization2OrganizationEdge> Organization2OrganizationInEdges { get; }
        IEnumerable<Organization2OrganizationEdge> Organization2OrganizationOutEdges { get; }
        IEnumerable<Organization> ParentOrganizations { get; }
        IEnumerable<Organization> SubOrganizations { get; }
        Tags? Tags { get; }
        PhoneNumber? Telephone { get; }
        IEnumerable<User2OrganizationEdge> User2OrganizationEdges { get; }
        IEnumerable<User> Users { get; }
        string? Website { get; }

        Organization2OrganizationEdge AddEdge(Organization2OrganizationEdge Edge);
        IEnumerable<Organization2OrganizationEdge> AddEdges(IEnumerable<Organization2OrganizationEdge> Edges);
        Organization2OrganizationEdge AddInEdge(Organization2OrganizationEdgeLabel EdgeLabel, Organization SourceOrganization, PrivacyLevel PrivacyLevel = PrivacyLevel.World);
        Organization2OrganizationEdge AddOutEdge(Organization2OrganizationEdgeLabel EdgeLabel, Organization TargetOrganization, PrivacyLevel PrivacyLevel = PrivacyLevel.World);
        User2OrganizationEdge AddUser(User Source, User2OrganizationEdgeLabel EdgeLabel, PrivacyLevel PrivacyLevel = PrivacyLevel.World);
        User2OrganizationEdge AddUser(User2OrganizationEdge Edge);
        IEnumerable<User2OrganizationEdge> AddUsers(IEnumerable<User2OrganizationEdge> Edges);
        int CompareTo(Organization Organization);
        void CopyAllLinkedDataFrom(Organization OldOrganization);
        IEnumerable<Organization2OrganizationEdgeLabel> EdgeLabels(Organization Organization);
        bool Equals(object Object);
        bool Equals(Organization Organization);
        IEnumerable<Organization> GetAllChilds(Func<Organization, bool> Include = null);
        IEnumerable<Organization> GetAllParents(Func<Organization, bool> Include = null);
        int GetHashCode();
        IEnumerable<Organization> GetMeAndAllMyChilds(Func<Organization, bool> Include = null);
        IEnumerable<Organization> GetMeAndAllMyParents(Func<Organization, bool> Include = null);
        JObject GetNotificationInfos();
        IEnumerable<ANotification> GetNotifications(NotificationMessageType? NotificationMessageType = null);
        IEnumerable<ANotification> GetNotifications(Func<NotificationMessageType, bool> NotificationMessageTypeFilter);
        IEnumerable<T> GetNotificationsOf<T>(Func<NotificationMessageType, bool> NotificationMessageTypeFilter) where T : ANotification;
        IEnumerable<T> GetNotificationsOf<T>(params NotificationMessageType[] NotificationMessageTypes) where T : ANotification;
        bool RemoveInEdge(Organization2OrganizationEdge Edge);
        void RemoveInEdges(Organization2OrganizationEdgeLabel EdgeLabel, Organization SourceOrganization);
        bool RemoveOutEdge(Organization2OrganizationEdge Edge);
        void RemoveOutEdges(Organization2OrganizationEdgeLabel EdgeLabel, Organization TargetOrganization);
        bool RemoveUser(User2OrganizationEdge Edge);
        void RemoveUser(User2OrganizationEdgeLabel EdgeLabel, User User);
        Organization.Builder ToBuilder(Organization_Id? NewOrganizationId = null);
        JObject ToJSON(bool Embedded = false);
        JObject ToJSON(bool Embedded = false, InfoStatus ExpandMembers = InfoStatus.ShowIdOnly, InfoStatus ExpandParents = InfoStatus.ShowIdOnly, InfoStatus ExpandSubOrganizations = InfoStatus.ShowIdOnly, InfoStatus ExpandTags = InfoStatus.ShowIdOnly, bool IncludeLastChange = true, CustomJObjectSerializerDelegate<Organization>? CustomOrganizationSerializer = null);
        string ToString();
        IEnumerable<User2OrganizationEdgeLabel> User2OrganizationInEdgeLabels(User User);
        IEnumerable<User2OrganizationEdge> User2OrganizationInEdges(User User);

    }

}
