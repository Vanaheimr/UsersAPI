﻿/*
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

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Styx.Arrows;
using org.GraphDefined.Vanaheimr.Hermod.Distributed;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using org.GraphDefined.Vanaheimr.Aegir;

#endregion

namespace org.GraphDefined.OpenData.Users
{

    /// <summary>
    /// Extention methods for OrganizationInfos.
    /// </summary>
    public static class OrganizationInfoExtentions
    {

        public static JArray ToJSON(this IEnumerable<OrganizationInfo> OrganizationInfos)
        {

            if (OrganizationInfos?.Any() == false)
                return new JArray();

            return JSONArray.Create(OrganizationInfos.Select(orgInfo => orgInfo.ToJSON()));

        }

    }

    public class OrganizationInfo : Organization
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of the object.
        /// </summary>
        public const String JSONLDContext = "https://opendata.social/contexts/UsersAPI+json/organizationInfo";

        #endregion

        #region Properties

        public   User                    You     { get; }
        public   IEnumerable<User>       Admins  { get; }
        public   IEnumerable<User>       Members { get; }

        internal List<OrganizationInfo>  internalChilds;
        public IEnumerable<OrganizationInfo> Childs
                => internalChilds;

        public   Boolean                 YouAreMember                      { get; }
        public   Boolean                 YouCanAddMembers                  { get; }
        public   Boolean                 YouCanCreateChildOrganizations    { get; }
        public   Boolean                 MemberOfChild                     { get; set; }

        #endregion

        #region Constructor(s)

        public OrganizationInfo(Organization  organization,
                                User          You,
                                Boolean       YouAreMember                    = false,
                                Boolean       YouCanAddMembers                = false,
                                Boolean       YouCanCreateChildOrganizations  = false)

            : base(organization.Id,
                   organization.Name,
                   organization.Description,
                   organization.EMail,
                   organization.PublicKeyRing,
                   organization.Telephone,
                   organization.GeoLocation,
                   organization.Address,
                   organization.PrivacyLevel,
                   organization.IsDisabled,
                   organization.DataSource,

                   organization.User2OrganizationEdges,
                   organization.Organization2UserEdges,
                   organization.Organization2OrganizationInEdges,
                   organization.Organization2OrganizationOutEdges)

        {

            this.You                             = You;
            this.Admins                          = _User2OrganizationEdges.Where(_ => _.EdgeLabel == Users.User2OrganizationEdges.IsAdmin). SafeSelect(edge => edge.Source).ToArray();
            this.Members                         = _User2OrganizationEdges.Where(_ => _.EdgeLabel == Users.User2OrganizationEdges.IsMember).SafeSelect(edge => edge.Source).ToArray();

            this.YouAreMember                    = YouAreMember                   || Admins.Contains(You) || Members.Contains(You);
            this.YouCanAddMembers                = YouCanAddMembers               || Admins.Contains(You);
            this.YouCanCreateChildOrganizations  = YouCanCreateChildOrganizations || Admins.Contains(You);

            #region GetChilds(Org, ...)

            IEnumerable<OrganizationInfo> GetChilds(Organization  Org,
                                                    Boolean       YouAreMemberRecursion,
                                                    Boolean       YouCanAddMembersRecursion,
                                                    Boolean       YouCanCreateChildOrganizationsRecursion)

                => Org.Organization2OrganizationInEdges.
                       Where     (edge => edge.EdgeLabel == Organization2OrganizationEdges.IsChildOf).
                       SafeSelect(edge => new OrganizationInfo(edge.Source,
                                                               You,
                                                               YouAreMemberRecursion,
                                                               YouCanAddMembersRecursion,
                                                               YouCanCreateChildOrganizationsRecursion));

            #endregion

            #region CheckYouMembership(OrgInfo)

            Boolean CheckYouMembership(OrganizationInfo OrgInfo)
            {

                if (OrgInfo.internalChilds == null || OrgInfo.internalChilds.Count == 0)
                    return OrgInfo.YouAreMember;

                var res = OrgInfo.YouAreMember;

                foreach (var ChildOrg in OrgInfo.Childs.ToArray())
                {

                    var resOfChild = CheckYouMembership(ChildOrg);

                    // Remove all child organizations if the given user is no direct member!
                    if (!resOfChild)
                        OrgInfo.internalChilds.Remove(ChildOrg);

                    res |= resOfChild;

                }

                return res;

            }

            #endregion

            var childInfos                       = GetChilds(organization,
                                                             this.YouAreMember,
                                                             this.YouCanAddMembers,
                                                             this.YouCanCreateChildOrganizations).ToList();

            foreach (var childInfo in childInfos)
                CheckYouMembership(childInfo);

            this.internalChilds = childInfos.Where(org => org.YouAreMember || org.internalChilds.Count > 0).ToList();

        }

        #endregion


        public JObject ToJSON()
        {

            var org      = base.ToJSON();

            org["@context"] = JSONLDContext;

            org.Add("youAreMember",                    YouAreMember);
            org.Add("youCanAddMembers",                YouCanAddMembers);
            org.Add("youCanCreateChildOrganizations",  YouCanCreateChildOrganizations);
            org.Add("admins",                          JSONArray.Create(Admins. SafeSelect(user => user.ToJSON())));

            if (YouAreMember)
                org.Add("members",                     JSONArray.Create(Members.SafeSelect(user => user.ToJSON())));

            org.Add("childs",                          new JArray(Childs.OrderBy(child => child.Id).Select(child => child.ToJSON())));

            return org;

        }

    }

}