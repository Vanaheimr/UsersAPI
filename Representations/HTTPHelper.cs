﻿/*
 * Copyright (c) 2014-2016, Achim 'ahzf' Friedland <achim@graphdefined.org>
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

namespace org.GraphDefined.OpenData
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

        #region ITEMS_GET(...)

        public static void ITEMS_GET<TId, TItem>(this HTTPServer             HTTPServer,
                                                 String                      UriTemplate,
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
                                            String                      UriTemplate,
                                            IEnumerable<TItem>          Enumeration,
                                            ItemFilterDelegate<TItem>   Filter,
                                            ItemsToJSONDelegate<TItem>      ToJSONDelegate)
        {


            HTTPServer.AddMethodCallback(HTTPMethod.GET,
                                         UriTemplate,
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             var skip      = Request.QueryString.GetUInt32("skip");
                                             var take      = Request.QueryString.GetUInt32("take");

                                             var AllItems  = Enumeration.
                                                                 Skip(skip.HasValue ? skip.Value : 0).
                                                                 Where(item => Filter(item));

                                             if (take.HasValue)
                                                 AllItems = AllItems.
                                                                Take(take.Value);

                                             return new HTTPResponseBuilder(Request) {
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
                                                   String                          UriTemplate,
                                                   ParseIdDelegate<TId>            ParseIdDelegate,
                                                   Func<String, String>            ParseIdError,
                                                   TryGetItemDelegate<TId, TItem>  TryGetItemDelegate,
                                                   ItemFilterDelegate<TItem>       ItemFilterDelegate,
                                                   Func<TId,   String>             TryGetItemError)
        {


            HTTPServer.AddMethodCallback(HTTPMethod.EXISTS,
                                         UriTemplate,
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             TId   Id;
                                             TItem Item;

                                             if (!ParseIdDelegate(Request.ParsedURIParameters[0], out Id))
                                                 return new HTTPResponseBuilder(Request) {
                                                     HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                     Server          = HTTPServer.DefaultServerName,
                                                     ContentType     = HTTPContentType.JSON_UTF8,
                                                     Content         = JSON.ErrorMessage(ParseIdError(Request.ParsedURIParameters[0])).ToUTF8Bytes(),
                                                     CacheControl    = "no-cache",
                                                     Connection      = "close"
                                                 };

                                             if (!TryGetItemDelegate(Id, out Item) || !ItemFilterDelegate(Item))
                                                 return new HTTPResponseBuilder(Request) {
                                                     HTTPStatusCode  = HTTPStatusCode.NotFound,
                                                     Server          = HTTPServer.DefaultServerName,
                                                     ContentType     = HTTPContentType.JSON_UTF8,
                                                     Content         = JSON.ErrorMessage(TryGetItemError(Id)).ToUTF8Bytes(),
                                                     CacheControl    = "no-cache",
                                                     Connection      = "close"
                                                 };

                                             return new HTTPResponseBuilder(Request) {
                                                 HTTPStatusCode  = HTTPStatusCode.OK,
                                                 Server          = HTTPServer.DefaultServerName,
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
                                                String                          UriTemplate,
                                                ParseIdDelegate<TId>            ParseIdDelegate,
                                                Func<String, String>            ParseIdError,
                                                TryGetItemDelegate<TId, TItem>  TryGetItemDelegate,
                                                ItemFilterDelegate<TItem>       ItemFilterDelegate,
                                                Func<TId,   String>             TryGetItemError,
                                                ItemToJSONDelegate<TItem>       ToJSONDelegate)
        {


            HTTPServer.AddMethodCallback(HTTPMethod.GET,
                                         UriTemplate,
                                         HTTPContentType.JSON_UTF8,
                                         HTTPDelegate: Request => {

                                             TId   Id;
                                             TItem Item;

                                             if (!ParseIdDelegate(Request.ParsedURIParameters[0], out Id))
                                                 return new HTTPResponseBuilder(Request) {
                                                     HTTPStatusCode  = HTTPStatusCode.BadRequest,
                                                     Server          = HTTPServer.DefaultServerName,
                                                     ContentType     = HTTPContentType.JSON_UTF8,
                                                     Content         = JSON.ErrorMessage(ParseIdError(Request.ParsedURIParameters[0])).ToUTF8Bytes(),
                                                     CacheControl    = "no-cache",
                                                     Connection      = "close"
                                                 };

                                             if (!TryGetItemDelegate(Id, out Item) || !ItemFilterDelegate(Item))
                                                 return new HTTPResponseBuilder(Request) {
                                                     HTTPStatusCode  = HTTPStatusCode.NotFound,
                                                     Server          = HTTPServer.DefaultServerName,
                                                     ContentType     = HTTPContentType.JSON_UTF8,
                                                     Content         = JSON.ErrorMessage(TryGetItemError(Id)).ToUTF8Bytes(),
                                                     CacheControl    = "no-cache",
                                                     Connection      = "close"
                                                 };

                                             return new HTTPResponseBuilder(Request) {
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