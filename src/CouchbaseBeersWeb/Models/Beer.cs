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

namespace CouchbaseBeersWeb.Models
{
	[CouchbaseDesignDoc("beers", "beer")]
	[CouchbaseAllView]
	public class Beer : ModelBase
	{
		[CouchbaseViewKey("by_name", "name")]
		public string Name { get; set; }

		public float ABV { get; set; }

		public float IBU { get; set; }

		public float SRM { get; set; }

		public float UPC { get; set; }

		public string BreweryId { get; set; }

		public DateTime Updated { get; set; }

		public string Description { get; set; }

		public string Style { get; set; }

		public string Category { get; set; }

		public override string Type { get { return "beer"; } }
	}
}