/*
 * Copyright (c) 2014-2017, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;

using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Illias.Votes;
using org.GraphDefined.Vanaheimr.Styx.Arrows;
using org.GraphDefined.Vanaheimr.Hermod.Mail;
using Newtonsoft.Json.Linq;

#endregion

namespace org.GraphDefined.OpenData
{

    /// <summary>
    /// An user.
    /// </summary>
    public class User : AEntity<User_Id>
    {

        #region Data

        /// <summary>
        /// The default max size of the aggregated EVSE operator status history.
        /// </summary>
        public const UInt16 DefaultUserStatusHistorySize = 50;


        private readonly ReactiveSet<MiniEdge<User, User2UserEdges,         User>>          _User2UserEdges;
        private readonly ReactiveSet<MiniEdge<User, User2GroupEdges,        Group>>         _User2GroupEdges;
        private readonly ReactiveSet<MiniEdge<User, User2OrganizationEdges, Organization>>  _User2OrganizationEdges;

        #endregion

        #region Properties

        /// <summary>
        /// The login of the user.
        /// </summary>
        [Mandatory]
        public String              Login
            => Id.ToString();

        /// <summary>
        /// The primary E-Mail address of the user.
        /// </summary>
        [Mandatory]
        public SimpleEMailAddress  EMail                { get; }

        /// <summary>
        /// The offical public name of the user.
        /// </summary>
        [Mandatory]
        public String              Name                 { get; }

        /// <summary>
        /// The PGP/GPG public keyring of the user.
        /// </summary>
        [Optional]
        public String              GPGPublicKeyRing     { get; }

        /// <summary>
        /// An optional (multi-language) description of the user.
        /// </summary>
        [Optional]
        public I18NString          Description          { get; }


        #region IsAuthenticated

        private Boolean _IsAuthenticated;

        /// <summary>
        /// The user will not be shown in user listings, as its
        /// primary e-mail address is not yet authenticated.
        /// </summary>
        [Mandatory]
        public Boolean IsAuthenticated
        {

            get
            {
                return _IsAuthenticated;
            }

            set
            {
                _IsAuthenticated = value;
            }

        }

        #endregion

        #region IsHidden / IsPublic

        private Boolean _IsHidden;

        /// <summary>
        /// The user will not be shown in user listings.
        /// </summary>
        [Mandatory]
        public Boolean IsHidden
        {

            get
            {
                return _IsHidden;
            }

            set
            {
                _IsHidden = value;
            }

        }

        /// <summary>
        /// The user will be shown in user listings.
        /// </summary>
        [Mandatory]
        public Boolean IsPublic
        {

            get
            {
                return !_IsHidden;
            }

            set
            {
                _IsHidden = !value;
            }

        }

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
        public IEnumerable<User2GroupEdges> Edges(Group Group)
            => _User2GroupEdges.
                   Where (edge => edge.Target == Group).
                   Select(edge => edge.EdgeLabel);

        #endregion

        #region FollowsGroups

        /// <summary>
        /// This user follows this other users.
        /// </summary>
        public IEnumerable<Group> FollowsGroups
        {
            get
            {
                return _User2GroupEdges.
                           Where(edge => edge.EdgeLabel == User2GroupEdges.follows).
                           Select(edge => edge.Target);
            }
        }

        #endregion

        #endregion

        #region Events

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="Login">The unique identification of the user.</param>
        /// <param name="EMail">The primary e-mail of the user.</param>
        /// <param name="Name">The offical (multi-language) name of the user.</param>
        /// <param name="GPGPublicKeyRing">The PGP/GPG public keyring of the user.</param>
        /// <param name="Description">An optional (multi-language) description of the user.</param>
        internal User(User_Id             Login,
                      SimpleEMailAddress  EMail,
                      String              Name              = null,
                      String              GPGPublicKeyRing  = null,
                      I18NString          Description       = null)

            : base(Login)

        {

            #region Initial checks

            if (Login == null)
                throw new ArgumentNullException(nameof(Login), "The login of the user must not be null!");

            #endregion

            #region Init data and properties

            this.EMail                     = EMail;
            this.Name                      = Name        != null ? Name        : "";
            this.Description               = Description != null ? Description : new I18NString();
            this.GPGPublicKeyRing          = GPGPublicKeyRing;
            this._IsAuthenticated          = false;
            this._IsHidden                 = false;

            this._User2UserEdges           = new ReactiveSet<MiniEdge<User, User2UserEdges,         User>>();
            this._User2GroupEdges          = new ReactiveSet<MiniEdge<User, User2GroupEdges,        Group>>();
            this._User2OrganizationEdges   = new ReactiveSet<MiniEdge<User, User2OrganizationEdges, Organization>>();

            #endregion

            #region Init events

            #endregion

            #region Link events

            #endregion

        }

        #endregion



        public MiniEdge<User, User2UserEdges, User>

            Follow(User          User,
                   PrivacyLevel  PrivacyLevel = PrivacyLevel.Private)

            => User.AddIncomingEdge(AddOutgoingEdge(User2UserEdges.follows, User, PrivacyLevel));

        //public MiniEdge<User, User2OrganizationEdges, Organization>

        //    Follow(Organization  Organization,
        //           PrivacyLevel  PrivacyLevel = PrivacyLevel.Private)

        //{
        //    return Organization.AddIncomingEdge(AddOutgoingEdge(User2OrganizationEdges.follows, Organization, PrivacyLevel));
        //}

        public MiniEdge<User, User2GroupEdges, Group>

            Join(Group         Group,
                 PrivacyLevel  PrivacyLevel = PrivacyLevel.Private)

            => Group.AddIncomingEdge(AddOutgoingEdge(User2GroupEdges.join, Group, PrivacyLevel));









        public MiniEdge<User, User2UserEdges, User>

            AddIncomingEdge(MiniEdge<User, User2UserEdges, User>  Edge)

            => _User2UserEdges.AddAndReturn(Edge);

        public MiniEdge<User, User2UserEdges, User>

            AddIncomingEdge(User            Source,
                            User2UserEdges  EdgeLabel,
                            PrivacyLevel    PrivacyLevel = PrivacyLevel.Public)

            => _User2UserEdges.AddAndReturn(new MiniEdge<User, User2UserEdges, User>(Source, EdgeLabel, this, PrivacyLevel));





        public MiniEdge<User, User2UserEdges, User>

            AddOutgoingEdge(User2UserEdges  EdgeLabel,
                            User            Target,
                            PrivacyLevel    PrivacyLevel = PrivacyLevel.Public)

            => _User2UserEdges.AddAndReturn(new MiniEdge<User, User2UserEdges, User>(this, EdgeLabel, Target, PrivacyLevel));

        public MiniEdge<User, User2OrganizationEdges, Organization>

            AddOutgoingEdge(User2OrganizationEdges  EdgeLabel,
                            Organization            Target,
                            PrivacyLevel            PrivacyLevel = PrivacyLevel.Public)

            => _User2OrganizationEdges.AddAndReturn(new MiniEdge<User, User2OrganizationEdges, Organization>(this, EdgeLabel, Target, PrivacyLevel));





        public MiniEdge<User, User2GroupEdges, Group>

            AddOutgoingEdge(User2GroupEdges EdgeLabel,
                            Group           Target,
                            PrivacyLevel    PrivacyLevel = PrivacyLevel.Public)

            => _User2GroupEdges.AddAndReturn(new MiniEdge<User, User2GroupEdges, Group>(this, EdgeLabel, Target, PrivacyLevel));




        #region IComparable<User> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is an user.
            var EVSE_User = Object as User;
            if ((Object) EVSE_User == null)
                throw new ArgumentException("The given object is not an user!");

            return CompareTo(EVSE_User);

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

            // Check if the given object is an user.
            var EVSE_User = Object as User;
            if ((Object) EVSE_User == null)
                return false;

            return this.Equals(EVSE_User);

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
        {
            return Id.GetHashCode();
        }

        #endregion

        #region ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()
        {
            return Id.ToString();
        }

        #endregion

        #region ToJSON()

        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        public JObject ToJSON()

            => new JObject(
                   new JProperty("login",        Login),
                   new JProperty("email",        EMail.ToString()),
                   new JProperty("name",         Name),
                   new JProperty("publickey",    GPGPublicKeyRing),
                   new JProperty("description",  Description)
               );

        #endregion

    }

}
