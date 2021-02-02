/*
 * Copyright (c) 2014-2021, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace social.OpenData.UsersAPI
{

    public static class AffectedExtentions
    {

        /// <summary>
        /// Whether the list of affected things is empty.
        /// </summary>
        public static Boolean IsEmpty(this Affected Affected)

            => Affected == null ||
             !(Affected.ServiceTickets.SafeAny() ||
               Affected.Users.         SafeAny() ||
               Affected.Organizations. SafeAny());


        /// <summary>
        /// Create a new CardiCloudAffected builder.
        /// </summary>
        public static Affected.Builder ToBuilder(this Affected Affected)

            => Affected == null
                   ? new Affected.Builder()
                   : new Affected.Builder(ServiceTicketLinks:  Affected.ServiceTickets,
                                          UserLinks:           Affected.Users,
                                          OrganizationLinks:   Affected.Organizations);

    }

    /// <summary>
    /// A list of things affected by a service ticket.
    /// </summary>
    public class Affected
    {

        #region Data

        /// <summary>
        /// An enumeration of affected or related service tickets.
        /// </summary>
        protected readonly HashSet<MessageHolder<ServiceTicket_Id, ServiceTicket>>  _ServiceTickets;

        /// <summary>
        /// An enumeration of users affected by a service ticket.
        /// </summary>
        protected readonly HashSet<MessageHolder<User_Id,          User>>           _Users;

        /// <summary>
        /// An enumeration of organizations affected by a service ticket.
        /// </summary>
        protected readonly HashSet<MessageHolder<Organization_Id,  Organization>>   _Organizations;

        #endregion

        #region Properties

        /// <summary>
        /// An enumeration of affected or related service tickets.
        /// </summary>
        public IEnumerable<MessageHolder<ServiceTicket_Id, ServiceTicket>>  ServiceTickets
            => _ServiceTickets;

        /// <summary>
        /// An enumeration of users affected by a service ticket.
        /// </summary>
        public IEnumerable<MessageHolder<User_Id, User>>                    Users
            => _Users;

        /// <summary>
        /// An enumeration of organizations affected by a service ticket.
        /// </summary>
        public IEnumerable<MessageHolder<Organization_Id, Organization>>    Organizations
            => _Organizations;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new list of things affected by a service ticket.
        /// </summary>
        /// <param name="ServiceTickets">Affected or related service tickets.</param>
        /// <param name="ServiceTicketIds">Affected or related service tickets.</param>
        /// <param name="ServiceTicketLinks">Affected or related service tickets.</param>
        /// 
        /// <param name="Users">An enumeration of users affected by a service ticket.</param>
        /// <param name="UserIds">An enumeration of users affected by a service ticket.</param>
        /// <param name="UserLinks">An enumeration of users affected by a service ticket.</param>
        /// 
        /// <param name="Organizations">An enumeration of organizations affected by a service ticket.</param>
        /// <param name="OrganizationIds">An enumeration of organizations affected by a service ticket.</param>
        /// <param name="OrganizationLinks">An enumeration of organizations affected by a service ticket.</param>
        public Affected(IEnumerable<ServiceTicket>                                   ServiceTickets       = null,
                        IEnumerable<ServiceTicket_Id>                                ServiceTicketIds     = null,
                        IEnumerable<MessageHolder<ServiceTicket_Id, ServiceTicket>>  ServiceTicketLinks   = null,

                        IEnumerable<User>                                            Users                = null,
                        IEnumerable<User_Id>                                         UserIds              = null,
                        IEnumerable<MessageHolder<User_Id, User>>                    UserLinks            = null,

                        IEnumerable<Organization>                                    Organizations        = null,
                        IEnumerable<Organization_Id>                                 OrganizationIds      = null,
                        IEnumerable<MessageHolder<Organization_Id, Organization>>    OrganizationLinks    = null)

        {

            _ServiceTickets  = new HashSet<MessageHolder<ServiceTicket_Id, ServiceTicket>>();
            _Users           = new HashSet<MessageHolder<User_Id,          User>>();
            _Organizations   = new HashSet<MessageHolder<Organization_Id,  Organization>>();


            if (ServiceTicketLinks.SafeAny())
                foreach (var serviceTicketLink in ServiceTicketLinks)
                    _ServiceTickets.Add(serviceTicketLink);

            if (ServiceTickets.SafeAny())
                foreach (var serviceTicket in ServiceTickets)
                    _ServiceTickets.Add(new MessageHolder<ServiceTicket_Id, ServiceTicket>(serviceTicket.Id, serviceTicket));

            if (ServiceTicketIds.SafeAny())
                foreach (var serviceTicketId in ServiceTicketIds)
                    _ServiceTickets.Add(new MessageHolder<ServiceTicket_Id, ServiceTicket>(serviceTicketId));


            if (UserLinks.SafeAny())
                foreach (var userLink in UserLinks)
                    _Users.Add(userLink);

            if (Users.SafeAny())
                foreach (var user in Users)
                    _Users.Add(new MessageHolder<User_Id,  User>(user.Id, user));

            if (UserIds.SafeAny())
                foreach (var userId in UserIds)
                    _Users.Add(new MessageHolder<User_Id,   User>(userId));


            if (OrganizationLinks.SafeAny())
                foreach (var organizationLink in OrganizationLinks)
                    _Organizations.Add(organizationLink);

            if (Organizations.SafeAny())
                foreach (var organization in Organizations)
                    _Organizations.Add(new MessageHolder<Organization_Id,  Organization>(organization.Id, organization));

            if (OrganizationIds.SafeAny())
                foreach (var organizationId in OrganizationIds)
                    _Organizations.Add(new MessageHolder<Organization_Id,   Organization>(organizationId));

        }

        #endregion


        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public virtual JObject ToJSON()

            => ToJSON(ExpandUserIds:          InfoStatus.ShowIdOnly,
                      ExpandOrganizationIds:  InfoStatus.ShowIdOnly);


        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON(InfoStatus  ExpandUserIds           = InfoStatus.ShowIdOnly,
                              InfoStatus  ExpandOrganizationIds   = InfoStatus.ShowIdOnly)

            => JSONObject.Create(

                   _ServiceTickets.SafeAny()
                       ? new JProperty("serviceTicketIds",  new JArray(_ServiceTickets.SafeSelect(serviceTicket => serviceTicket.Id.ToString())))
                       : null,

                   _Users.SafeAny()
                       ? new JProperty("userIds",           new JArray(_Users.         SafeSelect(user          => user.         Id.ToString())))
                       : null,

                   _Organizations.SafeAny()
                       ? new JProperty("organizationIds",   new JArray(_Organizations. SafeSelect(organization  => organization. Id.ToString())))
                       : null

               );

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out Affected, out ErrorResponse)

        /// <summary>
        /// Try to parse the given service ticket JSON.
        /// </summary>
        /// <param name="JSONObject">A JSON object.</param>
        /// <param name="ServiceTicketProvider">A delegate resolving service tickets.</param>
        /// <param name="UserProvider">A delegate resolving users.</param>
        /// <param name="OrganizationProvider">A delegate resolving organizations.</param>
        /// <param name="Affected">The parsed list of affected things.</param>
        /// <param name="ErrorResponse">An error message.</param>
        public static Boolean TryParseJSON(JObject                        JSONObject,
                                           ServiceTicketProviderDelegate  ServiceTicketProvider,
                                           UserProviderDelegate           UserProvider,
                                           OrganizationProviderDelegate   OrganizationProvider,
                                           out Affected                   Affected,
                                           out String                     ErrorResponse)
        {

            try
            {

                Affected = null;

                #region Parse ServiceTickets   [optional]

                var RelatedServiceTickets = new HashSet<MessageHolder<ServiceTicket_Id, ServiceTicket>>();

                if (JSONObject.ParseOptionalHashSet("serviceTicketIds",
                                                    "related service tickets",
                                                    ServiceTicket_Id.TryParse,
                                                    out HashSet<ServiceTicket_Id> RelatedServiceTicketIds,
                                                    out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                    if (ServiceTicketProvider != null)
                        foreach (var relatedServiceTicketId in RelatedServiceTicketIds)
                            if (ServiceTicketProvider(relatedServiceTicketId, out ServiceTicket relatedServiceTicket))
                                RelatedServiceTickets.Add(new MessageHolder<ServiceTicket_Id, ServiceTicket>(relatedServiceTicketId, relatedServiceTicket));

                }

                #endregion

                #region Parse Users            [optional]

                var RelatedUsers = new HashSet<MessageHolder<User_Id, User>>();

                if (JSONObject.ParseOptionalHashSet("userIds",
                                                    "related users",
                                                    User_Id.TryParse,
                                                    out HashSet<User_Id> RelatedUserIds,
                                                    out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                    if (UserProvider != null)
                        foreach (var relatedUserId in RelatedUserIds)
                            if (UserProvider(relatedUserId, out User relatedUser))
                                RelatedUsers.Add(new MessageHolder<User_Id, User>(relatedUserId, relatedUser));

                }

                #endregion

                #region Parse Organizations    [optional]

                var RelatedOrganizations = new HashSet<MessageHolder<Organization_Id, Organization>>();

                if (JSONObject.ParseOptionalHashSet("organizationIds",
                                                    "related organizations",
                                                    Organization_Id.TryParse,
                                                    out HashSet<Organization_Id> RelatedOrganizationIds,
                                                    out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                    if (OrganizationProvider != null)
                        foreach (var relatedOrganizationId in RelatedOrganizationIds)
                            if (OrganizationProvider(relatedOrganizationId, out Organization relatedOrganization))
                                RelatedOrganizations.Add(new MessageHolder<Organization_Id, Organization>(relatedOrganizationId, relatedOrganization));

                }

                #endregion


                Affected = new Affected(ServiceTicketLinks:  RelatedServiceTickets,
                                        UserLinks:           RelatedUsers,
                                        OrganizationLinks:   RelatedOrganizations);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse  = e.Message;
                Affected       = null;
                return false;
            }

        }

        #endregion


        #region (class) Builder

        /// <summary>
        /// An service ticket builder.
        /// </summary>
        public class Builder
        {

            #region Properties

            /// <summary>
            /// Affected or related service tickets.
            /// </summary>
            public HashSet<MessageHolder<ServiceTicket_Id, ServiceTicket>>  ServiceTickets        { get; }

            /// <summary>
            /// An enumeration of users affected by a service ticket.
            /// </summary>
            public HashSet<MessageHolder<User_Id, User>>                    Users                 { get; }

            /// <summary>
            /// An enumeration of organizations affected by a service ticket.
            /// </summary>
            public HashSet<MessageHolder<Organization_Id, Organization>>    Organizations         { get; }

            #endregion

            #region Constructor(s)

            /// <summary>
            /// Create a new list of things affected by a service ticket.
            /// </summary>
            /// <param name="ServiceTickets">Affected or related service tickets.</param>
            /// <param name="ServiceTicketIds">Affected or related service tickets.</param>
            /// <param name="ServiceTicketLinks">Affected or related service tickets.</param>
            /// 
            /// <param name="Users">An enumeration of users affected by a service ticket.</param>
            /// <param name="UserIds">An enumeration of users affected by a service ticket.</param>
            /// <param name="UserLinks">An enumeration of users affected by a service ticket.</param>
            /// 
            /// <param name="Organizations">An enumeration of organizations affected by a service ticket.</param>
            /// <param name="OrganizationIds">An enumeration of organizations affected by a service ticket.</param>
            /// <param name="OrganizationLinks">An enumeration of organizations affected by a service ticket.</param>
            public Builder(IEnumerable<ServiceTicket>                                   ServiceTickets       = null,
                           IEnumerable<ServiceTicket_Id>                                ServiceTicketIds     = null,
                           IEnumerable<MessageHolder<ServiceTicket_Id, ServiceTicket>>  ServiceTicketLinks   = null,

                           IEnumerable<User>                                            Users                = null,
                           IEnumerable<User_Id>                                         UserIds              = null,
                           IEnumerable<MessageHolder<User_Id, User>>                    UserLinks            = null,

                           IEnumerable<Organization>                                    Organizations        = null,
                           IEnumerable<Organization_Id>                                 OrganizationIds      = null,
                           IEnumerable<MessageHolder<Organization_Id, Organization>>    OrganizationLinks    = null)

            {

                this.ServiceTickets      = new HashSet<MessageHolder<ServiceTicket_Id, ServiceTicket>>();
                this.Users               = new HashSet<MessageHolder<User_Id,          User>>();
                this.Organizations       = new HashSet<MessageHolder<Organization_Id,  Organization>>();


                if (ServiceTicketLinks.SafeAny())
                    foreach (var relatedServiceTicketLink in ServiceTicketLinks)
                        this.ServiceTickets.Add(relatedServiceTicketLink);

                if (ServiceTickets.SafeAny())
                    foreach (var relatedServiceTicket in ServiceTickets)
                        this.ServiceTickets.Add(new MessageHolder<ServiceTicket_Id, ServiceTicket>(relatedServiceTicket.Id, relatedServiceTicket));

                if (ServiceTicketIds.SafeAny())
                    foreach (var relatedServiceTicketId in ServiceTicketIds)
                        this.ServiceTickets.Add(new MessageHolder<ServiceTicket_Id, ServiceTicket>(relatedServiceTicketId));


                if (UserLinks.SafeAny())
                    foreach (var userLink in UserLinks)
                        this.Users.Add(userLink);

                if (Users.SafeAny())
                    foreach (var user in Users)
                        this.Users.Add(new MessageHolder<User_Id,  User>(user.Id, user));

                if (UserIds.SafeAny())
                    foreach (var userId in UserIds)
                        this.Users.Add(new MessageHolder<User_Id,   User>(userId));


                if (OrganizationLinks.SafeAny())
                    foreach (var relatedOrganizationLink in OrganizationLinks)
                        this.Organizations. Add(relatedOrganizationLink);

                if (Organizations.SafeAny())
                    foreach (var relatedOrganization in Organizations)
                        this.Organizations. Add(new MessageHolder<Organization_Id,  Organization> (relatedOrganization.Id, relatedOrganization));

                if (OrganizationIds.SafeAny())
                    foreach (var relatedOrganizationId in OrganizationIds)
                        this.Organizations.Add(new MessageHolder<Organization_Id,   Organization> (relatedOrganizationId));

            }

            #endregion


            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the list of things affected by a service ticket.
            /// </summary>
            public static implicit operator Affected(Builder Builder)

                => Builder?.ToImmutable;


            /// <summary>
            /// Return an immutable version of the list of things affected by a service ticket.
            /// </summary>
            public Affected ToImmutable

                => new Affected(ServiceTicketLinks:  ServiceTickets,
                                UserLinks:           Users,
                                OrganizationLinks:   Organizations);

            #endregion

        }

        #endregion

    }

}
