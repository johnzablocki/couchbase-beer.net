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
using Couchbase;

namespace CouchbaseBeersWeb.Models
{	
	public class BreweryRepository : WebRepositoryBase<Brewery>
	{
		public IEnumerable<Brewery> GetAllByName(string startKey = null, string endKey = null, int limit = 0)
		{
			var view = GetView("by_name");

			if (!string.IsNullOrEmpty(startKey)) view.StartKey(startKey);
			if (!string.IsNullOrEmpty(endKey)) view.EndKey(startKey);
			if (limit > 0) view.Limit(limit);

			return view.Stale(StaleMode.False);
		}

		public IEnumerable<KeyValuePair<string, int>> GetCountsByCountry()
		{
			foreach (var row in GetViewRaw("by_country").Group(true))
			{
				yield return new KeyValuePair<string, int>(row.Info["key"] as string, Convert.ToInt32(row.Info["value"]));
			}
		}

		public Brewery GetWithBeerNames(string breweryId)
		{
			var view = GetViewRaw("all_with_beers")
				.StartKey(new object[] { breweryId, 0 })
				.EndKey(new object[] { breweryId, "\uefff", 1 });

			foreach (var item in view)
			{
				var brewery = Get(item.ItemId).Value;
				brewery.Beers = new List<Beer>();

				foreach (var childRow in view.Skip(1))
				{
					var key = childRow.Info["key"] as object[];
					brewery.Beers.Add(new Beer { Name = key[1].ToString() });
				}

				return brewery;
			}

			return null;
		}

		public IEnumerable<Brewery> GetByCountry(string country)
		{
			return GetView("by_country").Reduce(false).Key(country);
		}

		public IEnumerable<Brewery> GetByPoints(string boundingBox)
		{
			var points = boundingBox.Split(',').Select(s => float.Parse(s)).ToArray();
			return GetSpatialView("points").BoundingBox(points[0], points[1], points[2], points[3]);
		}

		protected override string BuildKey(Brewery brewery)
		{
			brewery.Id = brewery.Name;
			return base.BuildKey(brewery);
		}
	}
}