﻿/*
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
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;

#endregion

namespace social.OpenData.UsersAPI
{

    public delegate Boolean UserGroupProviderDelegate(UserGroup_Id UserGroupId, out UserGroup UserGroup);

    public delegate JObject UserGroupToJSONDelegate(UserGroup   UserGroup,
                                                    Boolean     Embedded                        = false,
                                                    InfoStatus  ExpandUsers                     = InfoStatus.ShowIdOnly,
                                                    InfoStatus  ExpandParentGroup               = InfoStatus.ShowIdOnly,
                                                    InfoStatus  ExpandSubgroups                 = InfoStatus.ShowIdOnly,
                                                    InfoStatus  ExpandAttachedFiles             = InfoStatus.ShowIdOnly,
                                                    InfoStatus  IncludeAttachedFileSignatures   = InfoStatus.ShowIdOnly,
                                                    Boolean     IncludeCryptoHash               = true);


    /// <summary>
    /// Extention methods for the user groups.
    /// </summary>
    public static partial class UserGroupExtentions
    {

        #region ToJSON(this UserGroups, Skip = null, Take = null, Embedded = false, ...)

        /// <summary>
        /// Return a JSON representation for the given enumeration of user groups.
        /// </summary>
        /// <param name="UserGroups">An enumeration of user groups.</param>
        /// <param name="Skip">The optional number of user groups to skip.</param>
        /// <param name="Take">The optional number of user groups to return.</param>
        /// <param name="Embedded">Whether this data is embedded into another data structure, e.g. into a user group.</param>
        public static JArray ToJSON(this IEnumerable<UserGroup>  UserGroups,
                                    UInt64?                      Skip                            = null,
                                    UInt64?                      Take                            = null,
                                    Boolean                      Embedded                        = false,
                                    InfoStatus                   ExpandUsers                     = InfoStatus.ShowIdOnly,
                                    InfoStatus                   ExpandParentGroup               = InfoStatus.ShowIdOnly,
                                    InfoStatus                   ExpandSubgroups                 = InfoStatus.ShowIdOnly,
                                    InfoStatus                   ExpandAttachedFiles             = InfoStatus.ShowIdOnly,
                                    InfoStatus                   IncludeAttachedFileSignatures   = InfoStatus.ShowIdOnly,
                                    UserGroupToJSONDelegate      UserGroupToJSON                 = null,
                                    Boolean                      IncludeCryptoHash               = true)


            => UserGroups?.Any() != true

                   ? new JArray()

                   : new JArray(UserGroups.
                                    OrderBy       (userGroup => userGroup.Id).
                                    SkipTakeFilter(Skip, Take).
                                    SafeSelect    (userGroup => UserGroupToJSON != null
                                                                            ? UserGroupToJSON (userGroup,
                                                                                                       Embedded,
                                                                                                       ExpandUsers,
                                                                                                       ExpandParentGroup,
                                                                                                       ExpandSubgroups,
                                                                                                       ExpandAttachedFiles,
                                                                                                       IncludeAttachedFileSignatures,
                                                                                                       IncludeCryptoHash)

                                                                            : userGroup.ToJSON(Embedded,
                                                                                                       ExpandUsers,
                                                                                                       ExpandParentGroup,
                                                                                                       ExpandSubgroups,
                                                                                                       ExpandAttachedFiles,
                                                                                                       IncludeAttachedFileSignatures,
                                                                                                       IncludeCryptoHash)));

        #endregion

    }


    /// <summary>
    /// A user group.
    /// </summary>
    public class UserGroup : AGroup<UserGroup_Id,
                                    UserGroup,
                                    User_Id,
                                    User>
    {

        #region Data

        /// <summary>
        /// The default JSON-LD context of user groups.
        /// </summary>
        public new readonly static JSONLDContext DefaultJSONLDContext = JSONLDContext.Parse("https://opendata.social/contexts/UsersAPI/userGroup");

        private readonly List<User2GroupEdge>   _User2GroupEdges;
        private readonly List<Group2UserEdge>   _Group2UserInEdges;
        private readonly List<Group2GroupEdge>  _Group2GroupEdgeTypes;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new user group.
        /// </summary>
        /// <param name="Id">The unique identification of the user group.</param>
        /// 
        /// <param name="Name">A multi-language name of the user group.</param>
        /// <param name="Description">A multi-language description of the user group.</param>
        /// <param name="Users">An enumeration of users.</param>
        /// <param name="ParentGroup">An optional parent user group.</param>
        /// <param name="Subgroups">Optional user subgroups.</param>
        /// 
        /// <param name="CustomData">Custom data to be stored with this user group.</param>
        /// <param name="AttachedFiles">Optional files attached to this user group.</param>
        /// <param name="JSONLDContext">The JSON-LD context of this user group.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        /// <param name="LastChange">The timestamp of the last changes within this user group. Can e.g. be used as a HTTP ETag.</param>
        public UserGroup(UserGroup_Id                 Id,

                         I18NString                   Name,
                         I18NString                   Description         = default,
                         IEnumerable<User>            Users               = default,
                         UserGroup                    ParentGroup         = default,
                         IEnumerable<UserGroup>       Subgroups           = default,

                         IEnumerable<User2GroupEdge>  User2GroupInEdges   = null,

                         JObject                      CustomData          = default,
                         IEnumerable<AttachedFile>    AttachedFiles       = default,
                         JSONLDContext?               JSONLDContext       = default,
                         String                       DataSource          = default,
                         DateTime?                    LastChange          = default)

            : base(Id,

                   Name,
                   Description,
                   Users,
                   ParentGroup,
                   Subgroups,

                   CustomData,
                   AttachedFiles,
                   JSONLDContext ?? DefaultJSONLDContext,
                   DataSource,
                   LastChange)

        {

            this._User2GroupEdges       = new List<User2GroupEdge>();
            this._Group2UserInEdges     = new List<Group2UserEdge>();
            this._Group2GroupEdgeTypes  = new List<Group2GroupEdge>();

            CalcHash();

        }

        #endregion


        #region ToJSON(...)

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
        /// <param name="IncludeCryptoHash">Include the crypto hash value of this object.</param>
        public override JObject ToJSON(Boolean Embedded           = false,
                                       Boolean IncludeCryptoHash  = false)

            => ToJSON(Embedded:                       false,
                      ExpandUsers:                    InfoStatus.ShowIdOnly,
                      ExpandParentGroup:              InfoStatus.ShowIdOnly,
                      ExpandSubgroups:                InfoStatus.ShowIdOnly,
                      ExpandAttachedFiles:            InfoStatus.ShowIdOnly,
                      IncludeAttachedFileSignatures:  InfoStatus.ShowIdOnly,
                      IncludeCryptoHash:              true);


        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure, e.g. into a UserGroup.</param>
        /// <param name="IncludeCryptoHash">Whether to include the cryptograhical hash value of this object.</param>
        public virtual JObject ToJSON(Boolean     Embedded                        = false,
                                      InfoStatus  ExpandUsers                     = InfoStatus.ShowIdOnly,
                                      InfoStatus  ExpandParentGroup               = InfoStatus.ShowIdOnly,
                                      InfoStatus  ExpandSubgroups                 = InfoStatus.ShowIdOnly,
                                      InfoStatus  ExpandAttachedFiles             = InfoStatus.ShowIdOnly,
                                      InfoStatus  IncludeAttachedFileSignatures   = InfoStatus.ShowIdOnly,
                                      Boolean     IncludeCryptoHash               = true)
        {


            var JSON = base.ToJSON(Embedded,
                                   false, //IncludeLastChange,
                                   IncludeCryptoHash,
                                   null,

                                   new JProperty("name",    Name.ToJSON()),

                                   Description.IsNeitherNullNorEmpty()
                                       ? new JProperty("description",    Description.ToJSON())
                                       : null,

                                   _User2GroupEdges.Where(edge => edge.EdgeLabel == User2GroupEdgeTypes.IsMember).SafeAny()
                                       ? new JProperty("isMember", new JArray(_User2GroupEdges.Where(edge => edge.EdgeLabel == User2GroupEdgeTypes.IsMember).Select(edge => edge.Source.Id.ToString())))
                                       : null,

                                   Members.SafeAny() && ExpandUsers != InfoStatus.Hidden
                                       ? ExpandSubgroups.Switch(
                                               () => new JProperty("memberIds",      new JArray(Members.SafeSelect(user => user.Id.ToString()))),
                                               () => new JProperty("members",        new JArray(Members.SafeSelect(user => user.   ToJSON(Embedded: true,
                                                                                                                                                    //ExpandParentGroup:  InfoStatus.Hidden,
                                                                                                                                                    //ExpandSubgroups:    InfoStatus.Expand,
                                                                                                                                                    IncludeCryptoHash:  IncludeCryptoHash)))))
                                       : null,

                                   ParentGroup != null      && ExpandParentGroup  != InfoStatus.Hidden
                                       ? ExpandParentGroup.Switch(
                                               () => new JProperty("parentGroupId",  ParentGroup.Id.ToString()),
                                               () => new JProperty("parentGroup",    ParentGroup.   ToJSON()))
                                       : null,

                                   Subgroups.SafeAny()      && ExpandSubgroups    != InfoStatus.Hidden
                                       ? ExpandSubgroups.Switch(
                                               () => new JProperty("subgroupsIds",   new JArray(Subgroups.SafeSelect(subgroup => subgroup.Id.ToString()))),
                                               () => new JProperty("subgroups",      new JArray(Subgroups.SafeSelect(subgroup => subgroup.   ToJSON(Embedded:           true,
                                                                                                                                                    ExpandParentGroup:  InfoStatus.Hidden,
                                                                                                                                                    ExpandSubgroups:    InfoStatus.Expanded,
                                                                                                                                                    IncludeCryptoHash:  IncludeCryptoHash)))))
                                       : null

                       );



            return JSON;

        }

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out UserGroup, out ErrorResponse)

        /// <summary>
        /// Try to parse the given user group JSON.
        /// </summary>
        /// <param name="JSONObject">A JSON object.</param>
        /// <param name="UserGroupProvider">A delegate resolving user groups.</param>
        /// <param name="UserProvider">A delegate resolving users.</param>
        /// <param name="UserGroup">The parsed user group.</param>
        /// <param name="ErrorResponse">An error message.</param>
        /// <param name="UserGroupIdURL">An optional UserGroup identification, e.g. from the HTTP URL.</param>
        public static Boolean TryParseJSON(JObject                    JSONObject,
                                           UserGroupProviderDelegate  UserGroupProvider,
                                           UserProviderDelegate       UserProvider,
                                           out UserGroup              UserGroup,
                                           out String                 ErrorResponse,
                                           UserGroup_Id?              UserGroupIdURL = null)
        {

            try
            {

                UserGroup = null;

                if (JSONObject?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse UserGroupId  [optional]

                // Verify that a given UserGroup identification
                //   is at least valid.
                if (JSONObject.ParseOptionalStruct("@id",
                                                   "UserGroup identification",
                                                   UserGroup_Id.TryParse,
                                                   out UserGroup_Id? UserGroupIdBody,
                                                   out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                if (!UserGroupIdURL.HasValue && !UserGroupIdBody.HasValue)
                {
                    ErrorResponse = "The UserGroup identification is missing!";
                    return false;
                }

                if (UserGroupIdURL.HasValue && UserGroupIdBody.HasValue && UserGroupIdURL.Value != UserGroupIdBody.Value)
                {
                    ErrorResponse = "The optional UserGroup identification given within the JSON body does not match the one given in the URI!";
                    return false;
                }

                #endregion

                #region Parse Context                       [mandatory]

                if (!JSONObject.ParseMandatory("@context",
                                               "JSON-LD context",
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

                #region Parse Name                          [mandatory]

                if (!JSONObject.ParseMandatory("name",
                                               "name",
                                               out I18NString Name,
                                               out ErrorResponse))
                {
                    return false;
                }

                #endregion

                #region Parse Description                   [optional]

                if (JSONObject.ParseOptional("description",
                                             "description",
                                             out I18NString Description,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion


                #region Parse ParentGroup identification    [optional]

                if (JSONObject.ParseOptionalStruct("parentGroupId",
                                                   "parentgroup identification",
                                                   UserGroup_Id.TryParse,
                                                   out UserGroup_Id? ParentGroupId,
                                                   out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                UserGroup ParentGroup = null;

                if (ParentGroupId.HasValue)
                    UserGroupProvider(ParentGroupId.Value, out ParentGroup);

                #endregion

                #region Parse Subgroup identifications      [optional]

                if (JSONObject.ParseOptional("SubgroupIds",
                                             "subgroup identifications",
                                             UserGroup_Id.TryParse,
                                             out IEnumerable<UserGroup_Id> SubgroupIds,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                List<UserGroup> Subgroups = null;

                if (SubgroupIds?.Any() == true)
                {

                    Subgroups = new List<UserGroup>();

                    foreach (var userGroupId in SubgroupIds)
                    {
                        if (UserGroupProvider(userGroupId, out UserGroup userGroup))
                            Subgroups.Add(userGroup);
                    }

                }

                #endregion

                #region Parse User identifications          [optional]

                if (JSONObject.ParseOptional("userIds",
                                             "user identifications",
                                             User_Id.TryParse,
                                             out IEnumerable<User_Id> UserIds,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                List<User> Users = null;

                if (UserIds?.Any() == true)
                {

                    Users = new List<User>();

                    foreach (var userId in UserIds)
                    {
                        if (UserProvider(userId, out User user))
                            Users.Add(user);
                    }

                }

                #endregion

 
                #region Get   DataSource       [optional]

                var DataSource = JSONObject.GetOptional("dataSource");

                #endregion


                #region Parse CryptoHash       [optional]

                var CryptoHash    = JSONObject.GetOptional("cryptoHash");

                #endregion


                UserGroup = new UserGroup(UserGroupIdBody ?? UserGroupIdURL.Value,

                                          Name,
                                          Description,
                                          Users,
                                          ParentGroup,
                                          Subgroups,

                                          null,

                                          null,
                                          null,
                                          Context,
                                          DataSource,
                                          null);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse  = e.Message;
                UserGroup      = null;
                return false;
            }

        }

        #endregion



        #region User  -> Group edges

        public User2GroupEdge AddIncomingEdge(User                 Source,
                                              User2GroupEdgeTypes  EdgeLabel,
                                              PrivacyLevel         PrivacyLevel = PrivacyLevel.Private)

            => _User2GroupEdges.
                   AddAndReturnElement(new User2GroupEdge(Source,
                                                          EdgeLabel,
                                                          this,
                                                          PrivacyLevel));


        public User2GroupEdge AddIncomingEdge(User2GroupEdge Edge)

            => _User2GroupEdges.
                   AddAndReturnElement(Edge);


        public IEnumerable<User2GroupEdge> User2GroupInEdges(Func<User2GroupEdgeTypes, Boolean> User2GroupEdgeFilter)
            => _User2GroupEdges.Where(edge => User2GroupEdgeFilter(edge.EdgeLabel));


        /// <summary>
        /// All organizations this user belongs to,
        /// filtered by the given edge label.
        /// </summary>
        /// <param name="User">Just return edges with the given user.</param>
        public IEnumerable<User2GroupEdgeTypes> InEdges(User User)

            => _User2GroupEdges.
                   Where (edge => edge.Source == User).
                   Select(edge => edge.EdgeLabel);


        public Boolean HasEdge(User2GroupEdgeTypes  EdgeLabel,
                               User                 User)

            => _User2GroupEdges.
                   Any(edge => edge.EdgeLabel == EdgeLabel && edge.Source == User);


        public UserGroup Add(IEnumerable<User2GroupEdge> Edges)
        {

            foreach (var edge in Edges)
                _User2GroupEdges.Add(edge);

            return this;

        }

        #endregion

        #region Group -> User  edges

        public Group2UserEdge AddOutgoingEdge(Group2UserEdgeTypes  EdgeLabel,
                                              User                 Target,
                                              PrivacyLevel         PrivacyLevel = PrivacyLevel.Private)

            => _Group2UserInEdges.
                   AddAndReturnElement(new Group2UserEdge(this,
                                                          EdgeLabel,
                                                          Target,
                                                          PrivacyLevel));

        public Group2UserEdge AddIncomingEdge(Group2UserEdge Edge)

            => _Group2UserInEdges.
                   AddAndReturnElement(Edge);


        /// <summary>
        /// All organizations this user belongs to,
        /// filtered by the given edge label.
        /// </summary>
        public IEnumerable<Group2UserEdgeTypes> OutEdges(User User)

            => _Group2UserInEdges.
                   Where (edge => edge.Target == User).
                   Select(edge => edge.EdgeLabel);


        public Boolean RemoveInEdge(Group2UserEdge Edge)

            => _Group2UserInEdges.
                   Remove(Edge);

        #endregion

        #region Group -> Group edges

        public Group2GroupEdge AddEdge(Group2GroupEdgeTypes  EdgeLabel,
                                       UserGroup             Target,
                                       PrivacyLevel          PrivacyLevel = PrivacyLevel.Private)

            => _Group2GroupEdgeTypes.
                   AddAndReturnElement(new Group2GroupEdge(this,
                                                           EdgeLabel,
                                                           Target,
                                                           PrivacyLevel));


        public Group2GroupEdge AddEdge(Group2GroupEdge Edge)

            => _Group2GroupEdgeTypes.
                   AddAndReturnElement(Edge);


        /// <summary>
        /// All organizations this user belongs to,
        /// filtered by the given edge label.
        /// </summary>
        public IEnumerable<Group2GroupEdgeTypes> Edges(UserGroup Group)

            => _Group2GroupEdgeTypes.
                   Where (edge => edge.Target == Group).
                   Select(edge => edge.EdgeLabel);

        #endregion



        #region CopyAllLinkedDataFrom(OldGroup)

        public override void CopyAllLinkedDataFrom(UserGroup OldGroup)
        {

            if (OldGroup._User2GroupEdges.Any() && !_User2GroupEdges.Any())
            {

                Add(OldGroup._User2GroupEdges);

                foreach (var edge in _User2GroupEdges)
                    edge.Target = this;

            }

        }

        #endregion


        #region Operator overloading

        #region Operator == (UserGroup1, UserGroup2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserGroup1">A user group.</param>
        /// <param name="UserGroup2">Another user group.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (UserGroup UserGroup1,
                                           UserGroup UserGroup2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(UserGroup1, UserGroup2))
                return true;

            // If one is null, but not both, return false.
            if ((UserGroup1 is null) || (UserGroup2 is null))
                return false;

            return UserGroup1.Equals(UserGroup2);

        }

        #endregion

        #region Operator != (UserGroup1, UserGroup2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserGroup1">A user group.</param>
        /// <param name="UserGroup2">Another user group.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (UserGroup UserGroup1,
                                           UserGroup UserGroup2)

            => !(UserGroup1 == UserGroup2);

        #endregion

        #region Operator <  (UserGroup1, UserGroup2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserGroup1">A user group.</param>
        /// <param name="UserGroup2">Another user group.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (UserGroup UserGroup1,
                                          UserGroup UserGroup2)
        {

            if (UserGroup1 is null)
                throw new ArgumentNullException(nameof(UserGroup1), "The given user group must not be null!");

            return UserGroup1.CompareTo(UserGroup2) < 0;

        }

        #endregion

        #region Operator <= (UserGroup1, UserGroup2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserGroup1">A user group.</param>
        /// <param name="UserGroup2">Another user group.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (UserGroup UserGroup1,
                                           UserGroup UserGroup2)

            => !(UserGroup1 > UserGroup2);

        #endregion

        #region Operator >  (UserGroup1, UserGroup2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserGroup1">A user group.</param>
        /// <param name="UserGroup2">Another user group.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (UserGroup UserGroup1,
                                          UserGroup UserGroup2)
        {

            if (UserGroup1 is null)
                throw new ArgumentNullException(nameof(UserGroup1), "The given user group must not be null!");

            return UserGroup1.CompareTo(UserGroup2) > 0;

        }

        #endregion

        #region Operator >= (UserGroup1, UserGroup2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="UserGroup1">A user group.</param>
        /// <param name="UserGroup2">Another user group.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (UserGroup UserGroup1,
                                           UserGroup UserGroup2)

            => !(UserGroup1 < UserGroup2);

        #endregion

        #endregion

        #region IComparable<UserGroup> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public override Int32 CompareTo(Object Object)
        {

            if (Object is UserGroup UserGroup)
                CompareTo(UserGroup);

            throw new ArgumentException("The given object is not a user group!");

        }

        #endregion

        #region CompareTo(UserGroup)

        /// <summary>
        /// Compares two user groups.
        /// </summary>
        /// <param name="UserGroup">A user group to compare with.</param>
        public override Int32 CompareTo(UserGroup UserGroup)
        {

            if (UserGroup is null)
                throw new ArgumentNullException(nameof(UserGroup), "The given user group must not be null!");

            return Id.CompareTo(UserGroup.Id);

        }

        #endregion

        #endregion

        #region IEquatable<UserGroup> Members

        #region Equals(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        /// <returns>true|false</returns>
        public override Boolean Equals(Object Object)
        {

            if (Object is UserGroup UserGroup)
                return Equals(UserGroup);

            return false;

        }

        #endregion

        #region Equals(UserGroup)

        /// <summary>
        /// Compares two user groups for equality.
        /// </summary>
        /// <param name="UserGroup">A user group to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public override Boolean Equals(UserGroup UserGroup)
        {

            if (UserGroup is null)
                return false;

            return Id.Equals(UserGroup.Id);

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


        #region ToBuilder(NewUserGroupId = null)

        /// <summary>
        /// Return a builder for this user group.
        /// </summary>
        /// <param name="NewUserGroupId">An optional new user group identification.</param>
        public Builder ToBuilder(UserGroup_Id? NewUserGroupId = null)

            => new Builder(NewUserGroupId ?? Id,

                           Description,
                           Name,
                           Members,
                           ParentGroup,
                           Subgroups,

                           CustomData,
                           AttachedFiles,
                           JSONLDContext,
                           DataSource,
                           LastChange);

        #endregion

        #region (class) Builder

        /// <summary>
        /// A user group builder.
        /// </summary>
        public new class Builder : AGroup<UserGroup_Id,
                                          UserGroup,
                                          User_Id,
                                          User>.Builder
        {

            #region Constructor(s)

            /// <summary>
            /// Create a new user group builder.
            /// </summary>
            /// <param name="Id">The unique identification of the user group.</param>
            /// 
            /// <param name="Name">A multi-language name of the user group.</param>
            /// <param name="Description">A multi-language description of the user group.</param>
            /// <param name="Users">An enumeration of users.</param>
            /// <param name="ParentGroup">An optional parent user group.</param>
            /// <param name="Subgroups">Optional user subgroups.</param>
            /// 
            /// <param name="CustomData">Custom data to be stored with this user group.</param>
            /// <param name="AttachedFiles">Optional files attached to this user group.</param>
            /// <param name="JSONLDContext">The JSON-LD context of this user group.</param>
            /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
            /// <param name="LastChange">The timestamp of the last changes within this user group. Can e.g. be used as a HTTP ETag.</param>
            public Builder(UserGroup_Id?              Id              = default,

                           I18NString                 Name            = default,
                           I18NString                 Description     = default,
                           IEnumerable<User>          Users           = default,
                           UserGroup                  ParentGroup     = default,
                           IEnumerable<UserGroup>     Subgroups       = default,

                           JObject                    CustomData      = default,
                           IEnumerable<AttachedFile>  AttachedFiles   = default,
                           JSONLDContext?             JSONLDContext   = default,
                           String                     DataSource      = default,
                           DateTime?                  LastChange      = default)

                : base(Id ?? UserGroup_Id.Random(),
                       JSONLDContext ?? DefaultJSONLDContext,
                       Name,
                       Description,
                       Users,
                       ParentGroup,
                       Subgroups,
                       CustomData,
                       AttachedFiles,
                       DataSource,
                       LastChange)

            { }

            #endregion


            #region CopyAllLinkedDataFrom(OldGroup)

            public override void CopyAllLinkedDataFrom(UserGroup OldGroup)
            {

                //if (OldGroup._User2GroupEdges.Any() && !_User2GroupEdges.Any())
                //{

                //    Add(OldGroup._User2GroupEdges);

                //    foreach (var edge in _User2GroupEdges)
                //        edge.Target = this;

                //}

            }

            #endregion

            #region ToImmutable

            /// <summary>
            /// Return an immutable version of the UserGroup.
            /// </summary>
            public static implicit operator UserGroup(Builder Builder)

                => Builder?.ToImmutable;


            /// <summary>
            /// Return an immutable version of the UserGroup.
            /// </summary>
            public UserGroup ToImmutable

                => new UserGroup(Id,

                                 Name,
                                 Description,
                                 Members,
                                 ParentGroup,
                                 Subgroups,

                                 null,

                                 CustomData,
                                 AttachedFiles,
                                 JSONLDContext,
                                 DataSource,
                                 LastChange);

            #endregion


            #region Operator overloading

            #region Operator == (Builder1, Builder2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Builder1">A user group builder.</param>
            /// <param name="Builder2">Another user group identification.</param>
            /// <returns>true|false</returns>
            public static Boolean operator == (Builder Builder1,
                                               Builder Builder2)
            {

                // If both are null, or both are same instance, return true.
                if (Object.ReferenceEquals(Builder1, Builder2))
                    return true;

                // If one is null, but not both, return false.
                if ((Builder1 is null) || (Builder2 is null))
                    return false;

                return Builder1.Equals(Builder2);

            }

            #endregion

            #region Operator != (Builder1, Builder2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Builder1">A user group builder.</param>
            /// <param name="Builder2">Another user group identification.</param>
            /// <returns>true|false</returns>
            public static Boolean operator != (Builder Builder1,
                                               Builder Builder2)

                => !(Builder1 == Builder2);

            #endregion

            #region Operator <  (Builder1, Builder2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Builder1">A user group builder.</param>
            /// <param name="Builder2">Another user group identification.</param>
            /// <returns>true|false</returns>
            public static Boolean operator < (Builder Builder1,
                                              Builder Builder2)
            {

                if (Builder1 is null)
                    throw new ArgumentNullException(nameof(Builder1), "The given user group must not be null!");

                return Builder1.CompareTo(Builder2) < 0;

            }

            #endregion

            #region Operator <= (Builder1, Builder2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Builder1">A user group builder.</param>
            /// <param name="Builder2">Another user group identification.</param>
            /// <returns>true|false</returns>
            public static Boolean operator <= (Builder Builder1,
                                               Builder Builder2)

                => !(Builder1 > Builder2);

            #endregion

            #region Operator >  (Builder1, Builder2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Builder1">A user group builder.</param>
            /// <param name="Builder2">Another user group identification.</param>
            /// <returns>true|false</returns>
            public static Boolean operator > (Builder Builder1,
                                              Builder Builder2)
            {

                if (Builder1 is null)
                    throw new ArgumentNullException(nameof(Builder1), "The given user group must not be null!");

                return Builder1.CompareTo(Builder2) > 0;

            }

            #endregion

            #region Operator >= (Builder1, Builder2)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Builder1">A user group builder.</param>
            /// <param name="Builder2">Another user group identification.</param>
            /// <returns>true|false</returns>
            public static Boolean operator >= (Builder Builder1,
                                               Builder Builder2)

                => !(Builder1 < Builder2);

            #endregion

            #endregion

            #region IComparable<UserGroup> Members

            #region CompareTo(Object)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Object">An object to compare with.</param>
            public override Int32 CompareTo(Object Object)
            {

                if (Object is UserGroup UserGroup)
                    CompareTo(UserGroup);

                throw new ArgumentException("The given object is not a user group!");

            }

            #endregion

            #region CompareTo(UserGroup)

            /// <summary>
            /// Compares two user groups.
            /// </summary>
            /// <param name="UserGroup">A user group to compare with.</param>
            public Int32 CompareTo(UserGroup UserGroup)

                => UserGroup is UserGroup
                       ? Id.CompareTo(UserGroup.Id)
                       : throw new ArgumentException("The given object is not an user group!", nameof(UserGroup));

            #endregion

            #endregion

            #region IEquatable<UserGroup> Members

            #region Equals(Object)

            /// <summary>
            /// Compares two instances of this object.
            /// </summary>
            /// <param name="Object">An object to compare with.</param>
            /// <returns>true|false</returns>
            public override Boolean Equals(Object Object)
            {

                if (Object is UserGroup UserGroup)
                    return Equals(UserGroup);

                return false;

            }

            #endregion

            #region Equals(UserGroup)

            /// <summary>
            /// Compares two user groups for equality.
            /// </summary>
            /// <param name="UserGroup">A user group to compare with.</param>
            /// <returns>True if both match; False otherwise.</returns>
            public Boolean Equals(UserGroup UserGroup)
            {

                if (UserGroup is null)
                    return false;

                return Id.Equals(UserGroup.Id);

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

        }

        #endregion

    }

}
