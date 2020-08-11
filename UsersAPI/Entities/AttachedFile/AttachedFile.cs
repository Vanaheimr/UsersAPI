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

using social.OpenData.UsersAPI;

#endregion

namespace social.OpenData.UsersAPI
{

    /// <summary>
    /// A attached file.
    /// </summary>
    public class AttachedFile : ADistributedEntity<AttachedFile_Id>
    {

        #region Data

        /// <summary>
        /// The JSON-LD context of this object.
        /// </summary>
        private const String JSONLDContext = "https://opendata.social/contexts/UsersAPI+json/attachedFile";

        #endregion

        #region Properties

        public AttachedFile_Id        Id               { get; }

        public I18NString             Description      { get; }

        public IEnumerable<HTTPPath>  Locations        { get; }

        public HTTPContentType        ContentType      { get; }

        public UInt64                 Size             { get; }

        public HTTPPath?              Icon             { get; }

        public DateTime               Created          { get; }

        public DateTime               LastModified     { get; }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Create a new attached file.
        /// </summary>
        private AttachedFile(AttachedFile_Id        Id,
                             I18NString             Description,
                             IEnumerable<HTTPPath>  Locations,
                             HTTPContentType        ContentType,
                             UInt64                 Size,
                             HTTPPath?              Icon          = null,
                             DateTime?              Created       = null,
                             DateTime?              LastModifed   = null,
                             String                 DataSource    = null)

            : base(Id,
                   DataSource)

        {

            this.Id            = Id;
            this.Description   = Description;
            this.Locations     = Locations;
            this.ContentType   = ContentType;
            this.Size          = Size;
            this.Icon          = Icon;
            this.Created       = Created     ?? DateTime.UtcNow;
            this.LastModified  = LastModifed ?? DateTime.UtcNow;
            //this.CryptoHashes  = 
            //this.Signatures    = 

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

            => ToJSON(Embedded:           false,
                      IncludeSignatures:  InfoStatus.Hidden,
                      IncludeCryptoHash:  true);



        /// <summary>
        /// Return a JSON representation of this object.
        /// </summary>
        /// <param name="Embedded">Whether this data is embedded into another data structure, e.g. into a AttachedFile.</param>
        /// <param name="IncludeCryptoHash">Whether to include the cryptograhical hash value of this object.</param>
        public JObject ToJSON(Boolean     Embedded            = false,
                              InfoStatus  IncludeSignatures   = InfoStatus.Hidden,
                              Boolean     IncludeCryptoHash   = true)
        {

            return JSONObject.Create(

                Id.ToJSON("@id"),

                Embedded
                    ? null
                    : new JProperty("@context",      JSONLDContext),

                Description?.ToJSON("description"),

                new JProperty("icon",                Icon.        ToString()),
                new JProperty("contentType",         ContentType. ToString()),
                new JProperty("created",             Created.     ToIso8601()),
                new JProperty("lastModified",        LastModified.ToString()),
                new JProperty("size",                Size),
                new JProperty("locations",           new JArray(Locations.SafeSelect(location => location.ToString()))),

                IncludeCryptoHash
                    ? new JProperty("cryptoHash", CurrentCryptoHash)
                    : null


            );

        }

        #endregion

        #region (static) TryParseJSON(JSONObject, ..., out AttachedFile, out ErrorResponse)

        /// <summary>
        /// Try to parse the given communicator group JSON.
        /// </summary>
        /// <param name="JSONObject">A JSON object.</param>
        /// <param name="AttachedFile">The parsed attached file.</param>
        /// <param name="ErrorResponse">An error message.</param>
        public static Boolean TryParseJSON(JObject           JSONObject,
                                           out AttachedFile  AttachedFile,
                                           out String        ErrorResponse,
                                           AttachedFile_Id?  AttachedFileIdURI   = null)
        {

            try
            {

                AttachedFile = null;

                if (JSONObject?.HasValues != true)
                {
                    ErrorResponse = "The given JSON object must not be null or empty!";
                    return false;
                }

                #region Parse AttachedFileId   [optional]

                // Verify that a given AttachedFile identification
                //   is at least valid.
                if (JSONObject.ParseOptionalStruct("@id",
                                                   "attached file identification",
                                                   AttachedFile_Id.TryParse,
                                                   out AttachedFile_Id? AttachedFileIdBody,
                                                   out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                if (!AttachedFileIdURI.HasValue && !AttachedFileIdBody.HasValue)
                {
                    ErrorResponse = "The AttachedFile identification is missing!";
                    return false;
                }

                if (AttachedFileIdURI.HasValue && AttachedFileIdBody.HasValue && AttachedFileIdURI.Value != AttachedFileIdBody.Value)
                {
                    ErrorResponse = "The optional AttachedFile identification given within the JSON body does not match the one given in the URI!";
                    return false;
                }

                #endregion

                #region Parse Context          [mandatory]

                if (!JSONObject.ParseMandatoryText("@context",
                                                   "JSON-LD context",
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

                #region Parse Description      [optional]

                if (JSONObject.ParseOptional("description",
                                             "description",
                                             out I18NString Description,
                                             out ErrorResponse))
                {

                    if (ErrorResponse != null)
                        return false;

                }

                #endregion

                var Locations     = new List<HTTPPath>();

                var ContentType   = HTTPContentType.JSONLD_UTF8;

                var Size          = 0UL;

                var Icon          = new HTTPPath?();

                var Created       = DateTime.UtcNow;

                var LastModified  = DateTime.UtcNow;

 
                #region Get   DataSource       [optional]

                var DataSource = JSONObject.GetOptional("dataSource");

                #endregion


                #region Parse CryptoHash       [optional]

                var CryptoHash    = JSONObject.GetOptional("cryptoHash");

                #endregion


                AttachedFile = new AttachedFile(Id:               AttachedFileIdBody ?? AttachedFileIdURI.Value,
                                                Description:      Description,
                                                Locations:        Locations,
                                                ContentType:      ContentType,
                                                Size:             Size,
                                                Icon:             Icon,
                                                Created:          Created,
                                                LastModifed:      LastModified,
                                                DataSource:       DataSource);

                ErrorResponse = null;
                return true;

            }
            catch (Exception e)
            {
                ErrorResponse = e.Message;
                AttachedFile  = null;
                return false;
            }

        }

        #endregion


        #region Operator overloading

        #region Operator == (AttachedFileId1, AttachedFileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AttachedFileId1">A defibrillator group identification.</param>
        /// <param name="AttachedFileId2">Another defibrillator group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator == (AttachedFile AttachedFileId1, AttachedFile AttachedFileId2)
        {

            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(AttachedFileId1, AttachedFileId2))
                return true;

            // If one is null, but not both, return false.
            if (((Object) AttachedFileId1 == null) || ((Object) AttachedFileId2 == null))
                return false;

            return AttachedFileId1.Equals(AttachedFileId2);

        }

        #endregion

        #region Operator != (AttachedFileId1, AttachedFileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AttachedFileId1">A defibrillator group identification.</param>
        /// <param name="AttachedFileId2">Another defibrillator group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator != (AttachedFile AttachedFileId1, AttachedFile AttachedFileId2)
            => !(AttachedFileId1 == AttachedFileId2);

        #endregion

        #region Operator <  (AttachedFileId1, AttachedFileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AttachedFileId1">A defibrillator group identification.</param>
        /// <param name="AttachedFileId2">Another defibrillator group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator < (AttachedFile AttachedFileId1, AttachedFile AttachedFileId2)
        {

            if ((Object) AttachedFileId1 == null)
                throw new ArgumentNullException(nameof(AttachedFileId1), "The given AttachedFileId1 must not be null!");

            return AttachedFileId1.CompareTo(AttachedFileId2) < 0;

        }

        #endregion

        #region Operator <= (AttachedFileId1, AttachedFileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AttachedFileId1">A defibrillator group identification.</param>
        /// <param name="AttachedFileId2">Another defibrillator group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator <= (AttachedFile AttachedFileId1, AttachedFile AttachedFileId2)
            => !(AttachedFileId1 > AttachedFileId2);

        #endregion

        #region Operator >  (AttachedFileId1, AttachedFileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AttachedFileId1">A defibrillator group identification.</param>
        /// <param name="AttachedFileId2">Another defibrillator group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator > (AttachedFile AttachedFileId1, AttachedFile AttachedFileId2)
        {

            if ((Object) AttachedFileId1 == null)
                throw new ArgumentNullException(nameof(AttachedFileId1), "The given AttachedFileId1 must not be null!");

            return AttachedFileId1.CompareTo(AttachedFileId2) > 0;

        }

        #endregion

        #region Operator >= (AttachedFileId1, AttachedFileId2)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AttachedFileId1">A defibrillator group identification.</param>
        /// <param name="AttachedFileId2">Another defibrillator group identification.</param>
        /// <returns>true|false</returns>
        public static Boolean operator >= (AttachedFile AttachedFileId1, AttachedFile AttachedFileId2)
            => !(AttachedFileId1 < AttachedFileId2);

        #endregion

        #endregion

        #region IComparable<AttachedFile> Members

        #region CompareTo(Object)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="Object">An object to compare with.</param>
        public Int32 CompareTo(Object Object)
        {

            if (Object == null)
                throw new ArgumentNullException(nameof(Object), "The given object must not be null!");

            if (!(Object is AttachedFile AttachedFile))
                throw new ArgumentException("The given object is not a defibrillator identification!",
                                            nameof(Object));

            return CompareTo(AttachedFile);

        }

        #endregion

        #region CompareTo(AttachedFile)

        /// <summary>
        /// Compares two instances of this object.
        /// </summary>
        /// <param name="AttachedFile">An object to compare with.</param>
        public Int32 CompareTo(AttachedFile AttachedFile)
        {

            if (AttachedFile is null)
                throw new ArgumentNullException(nameof(AttachedFile),  "The given attached file identification must not be null!");

            return Id.CompareTo(AttachedFile.Id);

        }

        #endregion

        #endregion

        #region IEquatable<AttachedFileId> Members

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

            if (!(Object is AttachedFile AttachedFile))
                return false;

            return Equals(AttachedFile);

        }

        #endregion

        #region Equals(AttachedFile)

        /// <summary>
        /// Compares two defibrillator identifications for equality.
        /// </summary>
        /// <param name="AttachedFile">An defibrillator identification to compare with.</param>
        /// <returns>True if both match; False otherwise.</returns>
        public Boolean Equals(AttachedFile AttachedFile)
        {

            if (AttachedFile is null)
                return false;

            return AttachedFile.Id.Equals(AttachedFile.Id);

        }

        #endregion

        #endregion

        #region GetHashCode()

        /// <summary>
        /// Return the HashCode of this object.
        /// </summary>
        /// <returns>The HashCode of this object.</returns>
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

}
