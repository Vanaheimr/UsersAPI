/*
 * Copyright (c) 2014-2020, Achim 'ahzf' Friedland <achim@graphdefined.org>
 * This file is part of Open Data Graph API <http://www.github.com/GraphDefined/OpenDataAPI>
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

    public delegate Boolean ParseIdDelegate    <TId>       (String Text, out TId   Id);
    public delegate Boolean TryGetItemDelegate <TId, TItem>(TId    Id,   out TItem Item);
    public delegate Boolean ItemFilterDelegate <TItem>     (TItem              Item);
    public delegate JObject ItemToJSONDelegate <TItem>     (TItem              Item);
    public delegate JObject ItemsToJSONDelegate<TItem>     (IEnumerable<TItem> Items);

    /// <summary>
    /// JSON content representation.
    /// </summary>
    public static class HTTPHelper
    {

        #region ErrorMessage(Message, Context = null)

        public static JObject ErrorMessage(String  Message,
                                           String  Context  = null)

            => new JObject(
                   new JProperty("@context",     Context ?? "https://opendata.social/contexts/UsersAPI+json/errors"),
                   new JProperty("description",  Message)
               );

        #endregion


        #region ITEMS_GET(...)

        public static void ITEMS_GET<TId, TItem>(this HTTPServer             HTTPServer,
                                                 HTTPPath                     UriTemplate,
                                                 Dictionary<TId, TItem>      Dictionary,
                                                 ItemFilterDelegate<TItem>   Filter,
                                                 ItemsToJSONDelegate<TItem>  ToJSONDelegate)
        {

            GET_ITEMS(HTTPServer,
                      UriTemplate,
                      Dictionary.Select(kvp => kvp.Value),
                      Filter,
                      ToJSONDelegate);

        }

        public static void GET_ITEMS<TItem>(this HTTPServer             HTTPServer,
                                            HTTPPath                     UriTemplate,
                                            IEnumerable<TItem>          Enumeration,
                                            ItemFilterDelegate<TItem>   Filter,
                                            ItemsToJSONDelegate<TItem>  ToJSONDelegate)
        {


            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         UriTemplate,
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             var skip      = Request.QueryString.GetUInt32("skip");
                                             var take      = Request.QueryString.GetUInt32("take");

                                             var AllItems  = Enumeration.
                                                                 Skip(skip.HasValue ? skip.Value : 0).
                                                                 Where(item => Filter(item));

                                             if (take.HasValue)
                                                 AllItems = AllItems.
                                                                Take(take.Value);

                                             return new HTTPResponse.Builder(Request) {
                                                                           HTTPStatusCode  = HTTPStatusCode.OK,
                                                                           Server          = HTTPServer.DefaultServerName,
                                                                           ContentType     = HTTPContentType.JSON_UTF8,
                                                                           Content         = ToJSONDelegate(AllItems).ToUTF8Bytes(),
                                                                           ETag            = "1",
                                                                           CacheControl    = "public",
                                                                           //Expires         = "Mon, 25 Jun 2015 21:31:12 GMT",
                                                                           Connection      = "close"
                                                                       };

                                         });

        }

        #endregion

        #region ITEM_EXISTS(...)

        public static void ITEM_EXISTS<TId, TItem>(this HTTPServer                 HTTPServer,
                                                   HTTPPath                         UriTemplate,
                                                   ParseIdDelegate<TId>            ParseIdDelegate,
                                                   Func<String, String>            ParseIdError,
                                                   TryGetItemDelegate<TId, TItem>  TryGetItemDelegate,
                                                   ItemFilterDelegate<TItem>       ItemFilterDelegate,
                                                   Func<TId,   String>             TryGetItemError,
                                                   String                          HTTPServerName  = HTTPServer.DefaultHTTPServerName)
        {


            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.EXISTS,
                                         UriTemplate,
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             if (!ParseIdDelegate(Request.ParsedURIParameters[0], out TId Id))
                                                 return new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode             = HTTPStatusCode.BadRequest,
                                                     Server                     = HTTPServerName,
                                                     Date                       = DateTime.UtcNow,
                                                     AccessControlAllowOrigin   = "*",
                                                     AccessControlAllowMethods  = "GET, EXISTS, COUNT",
                                                     AccessControlAllowHeaders  = "Content-Type, Accept, Authorization",
                                                     ETag                       = "1",
                                                     ContentType                = HTTPContentType.JSON_UTF8,
                                                     Content                    = ErrorMessage(ParseIdError(Request.ParsedURIParameters[0])).ToUTF8Bytes(),
                                                     CacheControl               = "no-cache",
                                                     Connection                 = "close"
                                                 };

                                             if (!TryGetItemDelegate(Id, out TItem Item) || !ItemFilterDelegate(Item))
                                                 return new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode  = HTTPStatusCode.NotFound,
                                                     Server          = HTTPServerName,
                                                     Date            = DateTime.UtcNow,
                                                     ContentType     = HTTPContentType.JSON_UTF8,
                                                     Content         = ErrorMessage(TryGetItemError(Id)).ToUTF8Bytes(),
                                                     CacheControl    = "no-cache",
                                                     Connection      = "close"
                                                 };

                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = HTTPServerName,
                                                 Date            = DateTime.UtcNow,
                                                 ETag            = "1",
                                                 CacheControl    = "public",
                                                 //Expires         = "Mon, 25 Jun 2015 21:31:12 GMT",
                                                 Connection      = "close"
                                             };

                                         });

        }

        #endregion

        #region ITEM_GET(...)

        public static void ITEM_GET<TId, TItem>(this HTTPServer                 HTTPServer,
                                                HTTPPath                         UriTemplate,
                                                ParseIdDelegate<TId>            ParseIdDelegate,
                                                Func<String, String>            ParseIdError,
                                                TryGetItemDelegate<TId, TItem>  TryGetItemDelegate,
                                                ItemFilterDelegate<TItem>       ItemFilterDelegate,
                                                Func<TId,   String>             TryGetItemError,
                                                ItemToJSONDelegate<TItem>       ToJSONDelegate)
        {


            HTTPServer.AddMethodCallback(HTTPHostname.Any,
                                         HTTPMethod.GET,
                                         UriTemplate,
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: async Request => {

                                             TId   Id;
                                             TItem Item;

                                             if (!ParseIdDelegate(Request.ParsedURIParameters[0], out Id))
                                                 return new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                     Server          = HTTPServer.DefaultServerName,
                                                     ContentType     = HTTPContentType.JSON_UTF8,
                                                     Content         = ErrorMessage(ParseIdError(Request.ParsedURIParameters[0])).ToUTF8Bytes(),
                                                     CacheControl    = "no-cache",
                                                     Connection      = "close"
                                                 };

                                             if (!TryGetItemDelegate(Id, out Item) || !ItemFilterDelegate(Item))
                                                 return new HTTPResponse.Builder(Request) {
                                                     HTTPStatusCode  = HTTPStatusCode.NotFound,
                                                     Server          = HTTPServer.DefaultServerName,
                                                     ContentType     = HTTPContentType.JSON_UTF8,
                                                     Content         = ErrorMessage(TryGetItemError(Id)).ToUTF8Bytes(),
                                                     CacheControl    = "no-cache",
                                                     Connection      = "close"
                                                 };

                                             return new HTTPResponse.Builder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = HTTPServer.DefaultServerName,
                                                 ContentType     = HTTPContentType.JSON_UTF8,
                                                 Content         = ToJSONDelegate(Item).ToUTF8Bytes(),
                                                 ETag            = "1",
                                                 CacheControl    = "public",
                                                 //Expires         = "Mon, 25 Jun 2015 21:31:12 GMT",
                                                 Connection      = "close"
                                             };

                                         });

        }

        #endregion

    }

}
