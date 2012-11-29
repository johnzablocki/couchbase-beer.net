#region [ License information          ]
/* ************************************************************
 * 
 *    @author Couchbase <info@couchbase.com>
 *    @copyright 2012 Couchbase, Inc.
 *    
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *    
 *        http://www.apache.org/licenses/LICENSE-2.0
 *    
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *    
 * ************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CouchbaseModelViews.Framework.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CouchbaseBeersWeb.Models
{
	[CouchbaseDesignDoc("breweries", "brewery")]
	[CouchbaseAllView]
	public class Brewery : ModelBase
	{
		[CouchbaseCollatedViewKey("all_with_beers", "beer", "name", "brewery_id")]
		public override string Id { get; set; }

		[CouchbaseViewKey("by_name", "name")]
		public string Name { get; set; }

		public string City { get; set; }
		
		public string State { get; set; }

		public string Code { get; set; }

		[CouchbaseViewKeyCount("by_country", "country")]
		public string Country { get; set; }

		public string Phone { get; set; }

		public string WebSite { get; set; }

		public DateTime Updated { get; set; }

		public string Description { get; set; }

		[JsonProperty("address")]
		public IList<string> Addresses { get; set; }
				
		public Dictionary<string, object> Geo { get; set; }
		
		public string GeoAccuracy 
		{
			get
			{
				return Geo != null && Geo.ContainsKey("accuracy") ? Geo["accuracy"] as string : "";
			}
		}

		[CouchbaseSpatialViewKey("points", "geo.lng", 0)]
		public float Longitude 
		{
			get
			{
				return Geo != null && Geo.ContainsKey("lng") ? Convert.ToSingle(Geo["lng"]) : 0f;
			}
		}

		[CouchbaseSpatialViewKey("points", "geo.lat", 1)]
		public float Latitude
		{
			get
			{				
				return Geo != null && Geo.ContainsKey("lat") ? Convert.ToSingle(Geo["lat"]) : 0f;
			}
		}

		public IList<Beer> Beers { get; set; }

		public override string Type
		{
			get { return "brewery"; }
		}
	}
}