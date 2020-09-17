///*
// * Copyright (c) 2014-2020, Achim 'ahzf' Friedland <achim@graphdefined.org>
// * This file is part of OpenDataAPI <http://www.github.com/GraphDefined/OpenDataAPI>
// *
// * Licensed under the Apache License, Version 2.0 (the "License");
// * you may not use this file except in compliance with the License.
// * You may obtain a copy of the License at
// *
// *     http://www.apache.org/licenses/LICENSE-2.0
// *
// * Unless required by applicable law or agreed to in writing, software
// * distributed under the License is distributed on an "AS IS" BASIS,
// * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// * See the License for the specific language governing permissions and
// * limitations under the License.
// */

//#region Usings

//using System;
//using System.Linq;
//using System.Collections.Generic;

//using Newtonsoft.Json.Linq;

//using org.GraphDefined.Vanaheimr.Illias;
//using org.GraphDefined.Vanaheimr.Styx.Arrows;
//using org.GraphDefined.Vanaheimr.Hermod;

//using org.GraphDefined.Vanaheimr.Hermod.HTTP;
//using social.OpenData.UsersAPI;

//#endregion

//namespace social.OpenData.UsersAPI
//{

//    /// <summary>
//    /// A group.
//    /// </summary>
//    public class Group : AEntity<Group_Id>,
//                         IEntityClass<Group>
//    {

//        #region Data

//        /// <summary>
//        /// The default max size of the aggregated user groups status history.
//        /// </summary>
//        public const UInt16 DefaultGroupStatusHistorySize = 50;

//        /// <summary>
//        /// The JSON-LD context of this object.
//        /// </summary>
//        public const String JSONLDContext = "https://opendata.social/contexts/UsersAPI+json/group";


//        #endregion

//        #region Properties

//        #region API

//        private UsersAPI _API;

//        /// <summary>
//        /// The UsersAPI of this service ticket.
//        /// </summary>
//        public UsersAPI API
//        {

//            get
//            {
//                return _API;
//            }

//            set
//            {

//                if (_API == value)
//                    return;

//                if (_API != null)
//                    throw new ArgumentException("Illegal attempt to change the API of this users group!");

//                _API = value ?? throw new ArgumentException("Illegal attempt to delete the API reference of this users group!");

//            }

//        }

//        #endregion

//        /// <summary>
//        /// The offical (multi-language) name of the group.
//        /// </summary>
//        [Mandatory]
//        public I18NString    Name            { get; }

//        /// <summary>
//        /// An optional (multi-language) description of the group.
//        /// </summary>
//        [Optional]
//        public I18NString    Description     { get; }

//        /// <summary>
//        /// Whether the group will be shown in group listings, or not.
//        /// </summary>
//        [Mandatory]
//        public PrivacyLevel  PrivacyLevel    { get; }

//        /// <summary>
//        /// The user will be shown in group listings.
//        /// </summary>
//        [Mandatory]
//        public Boolean       IsDisabled      { get; }

//        #endregion

//        #region Events

//        #endregion

//        #region Constructor(s)

//        /// <summary>
//        /// Create a new user group.
//        /// </summary>
//        /// <param name="Id">The unique identification of the user group.</param>
//        /// <param name="Name">The offical (multi-language) name of the user group.</param>
//        /// <param name="Description">An optional (multi-language) description of the user group.</param>
//        /// <param name="PrivacyLevel">Whether the group will be shown in group listings, or not.</param>
//        /// <param name="IsDisabled">The group is disabled.</param>
//        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
//        public Group(Group_Id       Id,
//                     I18NString     Name           = null,
//                     I18NString     Description    = null,
//                     PrivacyLevel?  PrivacyLevel   = null,
//                     Boolean        IsDisabled     = false,
//                     String         DataSource     = "")

//            : base(Id,
//                   DataSource)

//        {

//            #region Init properties

//            this.Name          = Name         ?? new I18NString();
//            this.Description   = Description  ?? new I18NString();
//            this.PrivacyLevel  = PrivacyLevel ?? social.OpenData.UsersAPI.PrivacyLevel.World;
//            this.IsDisabled    = IsDisabled;

//            #endregion

//            #region Init edges


//            #endregion

//            CalcHash();

//        }

//        #endregion




//        #region ToJSON(IncludeHash = true)

//        /// <summary>
//        /// Return a JSON representation of this object.
//        /// </summary>
//        /// <param name="Embedded">Whether this data is embedded into another data structure.</param>
//        /// <param name="IncludeCryptoHash">Include the crypto hash value of this object.</param>
//        public override JObject ToJSON(Boolean Embedded           = false,
//                                       Boolean IncludeCryptoHash  = false)

//            => JSONObject.Create(

//                   new JProperty("@id",               Id.         ToString()),
//                   new JProperty("@context",          JSONLDContext),
//                   new JProperty("name",              Name.       ToJSON()),
//                   new JProperty("description",       Description.ToJSON()),
//                   PrivacyLevel.ToJSON(),
//                   new JProperty("isDisabled",        IsDisabled),

//                   IncludeCryptoHash
//                       ? new JProperty("cryptoHash",  CurrentCryptoHash)
//                       : null

//               );

//        #endregion

//        #region (static) TryParseJSON(JSONObject, ..., out Group, out ErrorResponse)

//        public static Boolean TryParseJSON(JObject     JSONObject,
//                                           out Group   Group,
//                                           out String  ErrorResponse,
//                                           Group_Id?   GroupIdURI = null)
//        {

//            try
//            {

//                Group = null;

//                #region Parse GroupId          [optional]

//                // Verify that a given group identification
//                //   is at least valid.
//                if (JSONObject.ParseOptionalStruct("@id",
//                                                   "group identification",
//                                                   Group_Id.TryParse,
//                                                   out Group_Id? GroupIdBody,
//                                                   out ErrorResponse))
//                {

//                    if (ErrorResponse != null)
//                        return false;

//                }

//                if (!GroupIdURI.HasValue && !GroupIdBody.HasValue)
//                {
//                    ErrorResponse = "The group identification is missing!";
//                    return false;
//                }

//                if (GroupIdURI.HasValue && GroupIdBody.HasValue && GroupIdURI.Value != GroupIdBody.Value)
//                {
//                    ErrorResponse = "The optional group identification given within the JSON body does not match the one given in the URI!";
//                    return false;
//                }

//                #endregion

//                #region Parse Context          [mandatory]

//                if (!JSONObject.ParseMandatoryText("@context",
//                                                   "JSON-LinkedData context information",
//                                                   out String Context,
//                                                   out ErrorResponse))
//                {
//                    ErrorResponse = @"The JSON-LD ""@context"" information is missing!";
//                    return false;
//                }

//                if (Context != JSONLDContext)
//                {
//                    ErrorResponse = @"The given JSON-LD ""@context"" information '" + Context + "' is not supported!";
//                    return false;
//                }

//                #endregion

//                #region Parse Name             [optional]

//                if (JSONObject.ParseOptional("name",
//                                             "name",
//                                             out I18NString Name,
//                                             out ErrorResponse))
//                {

//                    if (ErrorResponse != null)
//                        return false;

//                }

//                #endregion

//                #region Parse Description      [optional]

//                if (JSONObject.ParseOptional("description",
//                                             "description",
//                                             out I18NString Description,
//                                             out ErrorResponse))
//                {

//                    if (ErrorResponse != null)
//                        return false;

//                }

//                #endregion

//                #region Parse PrivacyLevel     [optional]

//                if (JSONObject.ParseOptionalEnum("privacyLevel",
//                                             "privacy level",
//                                             out PrivacyLevel? PrivacyLevel,
//                                             out ErrorResponse))
//                {

//                    if (ErrorResponse != null)
//                        return false;

//                }

//                #endregion

//                var IsDisabled       = JSONObject["isDisabled"]?.     Value<Boolean>();

//                #region Get   DataSource       [optional]

//                var DataSource = JSONObject.GetOptional("dataSource");

//                #endregion

//                #region Parse CryptoHash       [optional]

//                var CryptoHash    = JSONObject.GetOptional("cryptoHash");

//                #endregion


//                Group = new Group(GroupIdBody ?? GroupIdURI.Value,
//                                  Name,
//                                  Description,
//                                  PrivacyLevel,
//                                  IsDisabled ?? false,
//                                  DataSource);

//                ErrorResponse = null;
//                return true;

//            }
//            catch (Exception e)
//            {
//                ErrorResponse  = e.Message;
//                Group  = null;
//                return false;
//            }

//        }

//        #endregion


//        #region CopyAllEdgesTo(NewGroup)

//        public void CopyAllEdgesTo(Group NewGroup)
//        {

//            //if (_User2Organization_InEdges.Any() && !NewGroup._User2Organization_InEdges.Any())
//            //{

//            //    NewGroup.Add(_User2Organization_InEdges);

//            //    foreach (var edge in NewGroup._User2Organization_InEdges)
//            //        edge.Target = NewGroup;

//            //}

//            //if (_Organization2Organization_InEdges.Any() && !NewGroup._Organization2Organization_InEdges.Any())
//            //{

//            //    NewGroup.AddInEdges(_Organization2Organization_InEdges);

//            //    foreach (var edge in NewGroup._Organization2Organization_InEdges)
//            //        edge.Target = NewGroup;

//            //}

//            //if (_Organization2Organization_OutEdges.Any() && !NewGroup._Organization2Organization_OutEdges.Any())
//            //{

//            //    NewGroup.AddOutEdges(_Organization2Organization_OutEdges);

//            //    foreach (var edge in NewGroup._Organization2Organization_OutEdges)
//            //        edge.Source = NewGroup;

//            //}

//        }

//        #endregion


//        #region Operator overloading

//        #region Operator == (GroupId1, GroupId2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="GroupId1">A group identification.</param>
//        /// <param name="GroupId2">Another group identification.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator == (Group GroupId1, Group GroupId2)
//        {

//            // If both are null, or both are same instance, return true.
//            if (Object.ReferenceEquals(GroupId1, GroupId2))
//                return true;

//            // If one is null, but not both, return false.
//            if (((Object) GroupId1 == null) || ((Object) GroupId2 == null))
//                return false;

//            return GroupId1.Equals(GroupId2);

//        }

//        #endregion

//        #region Operator != (GroupId1, GroupId2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="GroupId1">A group identification.</param>
//        /// <param name="GroupId2">Another group identification.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator != (Group GroupId1, Group GroupId2)
//            => !(GroupId1 == GroupId2);

//        #endregion

//        #region Operator <  (GroupId1, GroupId2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="GroupId1">A group identification.</param>
//        /// <param name="GroupId2">Another group identification.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator < (Group GroupId1, Group GroupId2)
//        {

//            if ((Object) GroupId1 == null)
//                throw new ArgumentNullException(nameof(GroupId1), "The given GroupId1 must not be null!");

//            return GroupId1.CompareTo(GroupId2) < 0;

//        }

//        #endregion

//        #region Operator <= (GroupId1, GroupId2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="GroupId1">A group identification.</param>
//        /// <param name="GroupId2">Another group identification.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator <= (Group GroupId1, Group GroupId2)
//            => !(GroupId1 > GroupId2);

//        #endregion

//        #region Operator >  (GroupId1, GroupId2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="GroupId1">A group identification.</param>
//        /// <param name="GroupId2">Another group identification.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator > (Group GroupId1, Group GroupId2)
//        {

//            if ((Object) GroupId1 == null)
//                throw new ArgumentNullException(nameof(GroupId1), "The given GroupId1 must not be null!");

//            return GroupId1.CompareTo(GroupId2) > 0;

//        }

//        #endregion

//        #region Operator >= (GroupId1, GroupId2)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="GroupId1">A group identification.</param>
//        /// <param name="GroupId2">Another group identification.</param>
//        /// <returns>true|false</returns>
//        public static Boolean operator >= (Group GroupId1, Group GroupId2)
//            => !(GroupId1 < GroupId2);

//        #endregion

//        #endregion

//        #region IComparable<Group> Members

//        #region CompareTo(Object)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="Object">An object to compare with.</param>
//        public override Int32 CompareTo(Object Object)
//        {

//            if (Object == null)
//                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

//            var EVSE_Operator = Object as Group;
//            if ((Object) EVSE_Operator == null)
//                throw new ArgumentException("The given object is not a group!");

//            return CompareTo(EVSE_Operator);

//        }

//        #endregion

//        #region CompareTo(Group)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="Group">A group object to compare with.</param>
//        public Int32 CompareTo(Group Group)
//        {

//            if ((Object) Group == null)
//                throw new ArgumentNullException(nameof(Group), "The given group must not be null!");

//            return Id.CompareTo(Group.Id);

//        }

//        #endregion

//        #endregion

//        #region IEquatable<Group> Members

//        #region Equals(Object)

//        /// <summary>
//        /// Compares two instances of this object.
//        /// </summary>
//        /// <param name="Object">An object to compare with.</param>
//        /// <returns>true|false</returns>
//        public override Boolean Equals(Object Object)
//        {

//            if (Object == null)
//                return false;

//            var Group = Object as Group;
//            if ((Object) Group == null)
//                return false;

//            return Equals(Group);

//        }

//        #endregion

//        #region Equals(Group)

//        /// <summary>
//        /// Compares two groups for equality.
//        /// </summary>
//        /// <param name="Group">A group to compare with.</param>
//        /// <returns>True if both match; False otherwise.</returns>
//        public Boolean Equals(Group Group)
//        {

//            if ((Object) Group == null)
//                return false;

//            return Id.Equals(Group.Id);

//        }

//        #endregion

//        #endregion

//        #region GetHashCode()

//        /// <summary>
//        /// Get the hashcode of this object.
//        /// </summary>
//        public override Int32 GetHashCode()
//            => Id.GetHashCode();

//        #endregion

//        #region (override) ToString()

//        /// <summary>
//        /// Return a text representation of this object.
//        /// </summary>
//        public override String ToString()
//            => Id.ToString();

//        #endregion


//        #region ToBuilder(NewGroupId = null)

//        /// <summary>
//        /// Return a builder for this group.
//        /// </summary>
//        /// <param name="NewGroupId">An optional new group identification.</param>
//        public Builder ToBuilder(Group_Id? NewGroupId = null)

//            => new Builder(NewGroupId ?? Id,
//                           Name,
//                           Description);

//        #endregion

//        #region (class) Builder

//        /// <summary>
//        /// A group builder.
//        /// </summary>
//        public class Builder
//        {

//            #region Properties

//            /// <summary>
//            /// The unique identification of the group.
//            /// </summary>
//            public Group_Id      Id               { get; set; }

//            /// <summary>
//            /// The offical public name of the group.
//            /// </summary>
//            [Optional]
//            public I18NString    Name             { get; set; }

//            /// <summary>
//            /// An optional (multi-language) description of the group.
//            /// </summary>
//            [Optional]
//            public I18NString    Description      { get; set; }

//            /// <summary>
//            /// Whether the group will be shown in group listings, or not.
//            /// </summary>
//            [Mandatory]
//            public PrivacyLevel  PrivacyLevel     { get; set; }

//            /// <summary>
//            /// The group is disabled.
//            /// </summary>
//            [Mandatory]
//            public Boolean       IsDisabled       { get; set; }

//            #endregion

//            #region Constructor(s)

//            /// <summary>
//            /// Create a new group builder.
//            /// </summary>
//            /// <param name="Id">The unique identification of the group.</param>
//            /// <param name="Name">An offical (multi-language) name of the group.</param>
//            /// <param name="Description">An optional (multi-language) description of the group.</param>
//            /// <param name="PrivacyLevel">Whether the group will be shown in group listings, or not.</param>
//            /// <param name="IsDisabled">The group is disabled.</param>
//            public Builder(Group_Id      Id,
//                           I18NString    Name           = null,
//                           I18NString    Description    = null,
//                           PrivacyLevel  PrivacyLevel   = PrivacyLevel.World,
//                           Boolean       IsDisabled     = false)
//            {

//                #region Init properties

//                this.Id            = Id;
//                this.Name          = Name        ?? new I18NString();
//                this.Description   = Description ?? new I18NString();
//                this.PrivacyLevel  = PrivacyLevel;
//                this.IsDisabled    = IsDisabled;

//                #endregion

//                #region Init edges

//                //this._User2UserEdges          = new ReactiveSet<MiniEdge<User, User2UserEdges,         User>>();
//                //this._User2GroupEdges         = new ReactiveSet<MiniEdge<User, User2GroupEdges,        Group>>();
//                //this._User2GroupEdges  = new ReactiveSet<MiniEdge<User, User2GroupEdges, Group>>();

//                #endregion

//            }

//            #endregion


//            #region Build()

//            /// <summary>
//            /// Return an immutable version of the group.
//            /// </summary>
//            public Group Build()

//                => new Group(Id,
//                             Name,
//                             Description,
//                             PrivacyLevel,
//                             IsDisabled);

//            #endregion

//        }

//        #endregion

//    }

//}
