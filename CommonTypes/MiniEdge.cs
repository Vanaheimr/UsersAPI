/*
 * Copyright (c) 2014-2015, Achim 'ahzf' Friedland <achim@graphdefined.org>
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#endregion

namespace org.GraphDefined.UsersAPI
{

    public struct MiniEdge<TSource, TEdge, TTarget> : IEquatable <MiniEdge<TSource, TEdge, TTarget>>,
                                                      IComparable<MiniEdge<TSource, TEdge, TTarget>>,
                                                      IComparable

        where TSource : IEntity
        where TEdge   : IComparable
        where TTarget : IEntity

    {

        #region Data

        #region Source

        private readonly TSource _Source;

        public TSource Source
        {
            get
            {
                return _Source;
            }
        }

        #endregion

        #region EdgeLabel

        private readonly TEdge _EdgeLabel;

        public TEdge EdgeLabel
        {
            get
            {
                return _EdgeLabel;
            }
        }

        #endregion

        #region Target

        private readonly TTarget _Target;

        public TTarget Target
        {
            get
            {
                return _Target;
            }
        }

        #endregion

        #region PrivacyLevel

        private readonly PrivacyLevel _PrivacyLevel;

        public PrivacyLevel PrivacyLevel
        {
            get
            {
                return _PrivacyLevel;
            }
        }

        #endregion

        #region Created

        private readonly DateTime _Created;

        public DateTime Created
        {
            get
            {
                return _Created;
            }
        }

        #endregion

        #region UserDefined

        //private readonly Dictionary<String, Object> _UserDefined;

        ///// <summary>
        ///// A lookup for user-defined properties.
        ///// </summary>
        //public Dictionary<String, Object> UserDefined
        //{
        //    get
        //    {
        //        return _UserDefined;
        //    }
        //}

        #endregion

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new miniedge.
        /// </summary>
        /// <param name="Source">The source of the edge.</param>
        /// <param name="EdgeLabel">The label of the edge.</param>
        /// <param name="Target">The target of the edge</param>
        /// <param name="PrivacyLevel">The level of privacy of this edge.</param>
        /// <param name="Created">The creation timestamp of the miniedge.</param>
        public MiniEdge(TSource       Source,
                        TEdge         EdgeLabel,
                        TTarget       Target,
                        PrivacyLevel  PrivacyLevel  = PrivacyLevel.Private,
                        DateTime?     Created       = null)
        {

            #region Initial checks

            if (Source == null)
                throw new ArgumentNullException("Id", "The given Id must not be null!");

            #endregion

            this._Source        = Source;
            this._Target        = Target;
            this._EdgeLabel     = EdgeLabel;
            this._PrivacyLevel  = PrivacyLevel;
            this._Created       = Created != null ? Created.Value : DateTime.Now;
         //   this._UserDefined   = new Dictionary<String, Object>();

        }

        #endregion


        #region IComparable<MiniEdge<TSource, TEdge, TTarget>> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException("The given object must not be null!");

            // Check if the given object is a miniedge.
            if (!(Object is MiniEdge<TSource, TEdge, TTarget>))
                throw new ArgumentException("The given object is not a miniedge!");

            return CompareTo((MiniEdge<TSource, TEdge, TTarget>)Object);

        }

        #endregion

        #region CompareTo(MiniEdge)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="MiniEdge">A miniedge to compare with.</param>
        public Int32 CompareTo(MiniEdge<TSource, TEdge, TTarget> MiniEdge)
        {

            if ((Object) MiniEdge == null)
                throw new ArgumentNullException("The given miniedge must not be null!");

            var source  = _Source.  CompareTo(MiniEdge._Source);
            var type    = _EdgeLabel.CompareTo(MiniEdge._EdgeLabel);
            var target  = _Target.  CompareTo(MiniEdge._Target);

            if (type != 0)
                return type;

            if (source != 0)
                return source;

            if (target != 0)
                return target;

            return 0;

        }

        #endregion

        #endregion

        #region IEquatable<MiniEdge> Members

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

            // Check if the given object is a miniedge.
            if (!(Object is MiniEdge<TSource, TEdge, TTarget>))
                return false;

            return this.Equals((MiniEdge<TSource, TEdge, TTarget>)Object);

        }

        #endregion

        #region Equals(User)

        /// <summary>
        /// Compares two miniedges for equality.
        /// </summary>
        /// <param name="User">A miniedge to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(MiniEdge<TSource, TEdge, TTarget> MiniEdge)
        {

            if ((Object) MiniEdge == null)
                return false;

            if (!_Source.Equals(MiniEdge._Source))
                return false;

            if (!_EdgeLabel.Equals(MiniEdge._EdgeLabel))
                return false;

            if (!_Target.Equals(MiniEdge._Target))
                return false;

            return true;

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Get the hashcode of this object.
        /// </summary>
        public override Int32 GetHashCode()
        {

            // Overflow is fine, just wrap
            // http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode

            unchecked
            {

                var hash = 17;

                hash = hash * 23 + _Source.      GetHashCode();
                hash = hash * 23 + _EdgeLabel.   GetHashCode();
                hash = hash * 23 + _Target.      GetHashCode();
                hash = hash * 23 + _PrivacyLevel.GetHashCode();
                hash = hash * 23 + _Created.     GetHashCode();

                return hash;

            }

        }

        #endregion

        #region ToString()

        /// <summary>
        /// Get a string representation of this object.
        /// </summary>
        public override String ToString()
        {

            var IsNotPublic = _PrivacyLevel != PrivacyLevel.Public ? String.Concat(" [", _PrivacyLevel.ToString(), "]") : "";

            return String.Concat(_Source, " --", EdgeLabel, IsNotPublic, "->", _Target);

        }

        #endregion

    }

}
