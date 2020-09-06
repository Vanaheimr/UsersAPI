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

using org.GraphDefined.Vanaheimr.Aegir;
using org.GraphDefined.Vanaheimr.Illias;
using org.GraphDefined.Vanaheimr.Hermod;
using org.GraphDefined.Vanaheimr.Hermod.HTTP;
using org.GraphDefined.Vanaheimr.Hermod.Distributed;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// An abstract group.
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TGroup"></typeparam>
    /// <typeparam name="TMembers"></typeparam>
    public abstract class AGroup<TId, TGroup, TMembers> : ADistributedEntity<TId>,
                                                          IEntityClass<TGroup>

        where TId      : IId
        where TGroup   : class
        where TMembers : class

    {

        #region Properties

        #region API

        private Object _API;

        /// <summary>
        /// The CardiCloudAPI of this CommunicatorGroup.
        /// </summary>
        public Object API
        {

            get
            {
                return _API;
            }

            set
            {

                if (_API != null)
                    throw new ArgumentException("Illegal attempt to change the API of this CommunicatorGroup!");

                _API = value ?? throw new ArgumentException("Illegal attempt to delete the API reference of this CommunicatorGroup!");

            }

        }

        #endregion

        /// <summary>
        /// A multi-language description of this group.
        /// </summary>
        public I18NString                 Description       { get; }

        /// <summary>
        /// The members of this group.
        /// </summary>
        public IEnumerable<TMembers>      Members           { get; }

        /// <summary>
        /// An optional parent group.
        /// </summary>
        public TGroup                     ParentGroup       { get; }

        /// <summary>
        /// Optional subgroups.
        /// </summary>
        public IEnumerable<TGroup>        Subgroups         { get; }

        /// <summary>
        /// Optional files attached to this group.
        /// </summary>
        public IEnumerable<AttachedFile>  AttachedFiles     { get; }

        /// <summary>
        /// Whether the group will be shown in (public) listings.
        /// </summary>
        public PrivacyLevel               PrivacyLevel      { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new abstract group.
        /// </summary>
        /// <param name="Id">The unique identification of the group.</param>
        /// <param name="Description">A multi-language description of the group.</param>
        /// <param name="Members">The members of the group.</param>
        /// <param name="ParentGroup">An optional parent group.</param>
        /// <param name="Subgroups">Optional subgroups.</param>
        /// <param name="AttachedFiles">Optional files attached to this group.</param>
        /// 
        /// <param name="PrivacyLevel">Whether the group will be shown in (public) listings.</param>
        /// <param name="DataSource">The source of all this data, e.g. an automatic importer.</param>
        public AGroup(TId                        Id,
                      I18NString                 Description,
                      IEnumerable<TMembers>      Members,
                      TGroup                     ParentGroup     = null,
                      IEnumerable<TGroup>        Subgroups       = null,
                      IEnumerable<AttachedFile>  AttachedFiles   = null,

                      PrivacyLevel?              PrivacyLevel    = null,
                      String                     DataSource      = null)

            : base(Id,
                   DataSource)

        {
        
            this.Description     = Description    ?? I18NString.Empty;
            this.Members         = Members        ?? new TMembers[0];
            this.ParentGroup     = ParentGroup;
            this.Subgroups       = Subgroups      ?? new TGroup[0];
            this.AttachedFiles   = AttachedFiles  ?? new AttachedFile[0];

            this.PrivacyLevel    = PrivacyLevel   ?? social.OpenData.UsersAPI.PrivacyLevel.Private;

        }

        #endregion





        public abstract int CompareTo(TGroup other);
        public abstract void CopyAllEdgesTo(TGroup Enity);
        public abstract bool Equals(TGroup other);

    }

}
