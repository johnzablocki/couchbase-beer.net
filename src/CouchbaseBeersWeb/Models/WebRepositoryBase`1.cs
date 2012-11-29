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
using Enyim.Caching.Memcached.Results;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Results.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace CouchbaseBeersWeb.Models
{
	public abstract class WebRepositoryBase<T> where T : ModelBase
	{
		private const string ITEMS_CLIENT_KEY = "CouchbaseClient";
		private readonly PluralizationService _pluralizationService;
		protected readonly string _DesignDocName;

		public WebRepositoryBase()
		{
			_pluralizationService = PluralizationService.CreateService(CultureInfo.CurrentCulture);
			_DesignDocName = _pluralizationService.Pluralize(typeof(T).Name.ToLower());			
		}

		protected CouchbaseClient _Client
		{
			get { return getCouchbaseClient(); }
		}

		public virtual IStoreOperationResult Create(T value)
		{
			return _Client.ExecuteStore(StoreMode.Add, BuildKey(value), serializeAndIgnoreId(value));
		}

		public virtual IStoreOperationResult Update(T value)
		{
			return _Client.ExecuteStore(StoreMode.Replace, value.Id, serializeAndIgnoreId(value));
		}

		public virtual IStoreOperationResult Save(T value)
		{
			var key = string.IsNullOrEmpty(value.Id) ? BuildKey(value) : value.Id;
			return _Client.ExecuteStore(StoreMode.Set, key, serializeAndIgnoreId(value));
		}

		public virtual IGetOperationResult<T> Get(string key)
		{
			var jsonResult = _Client.ExecuteGet<string>(key);
			var retval = new GetOperationResult<T>();
			jsonResult.Combine(retval);

			if (jsonResult.HasValue)
			{
				retval.Value = JsonConvert.DeserializeObject<T>(jsonResult.Value);
				retval.Value.Id = key; //_id is not stored in document, generic view merges it into JSON on return
			}

			return retval;
		}

		public virtual IEnumerable<T> GetAll(string startKey = null, string endKey = null, int limit = 50, StaleMode? stale = null)
		{
			var designDocName = _pluralizationService.Pluralize(_DesignDocName);
			var view = _Client.GetView<T>(designDocName, "all", true).Limit(limit);
			if (startKey != null) view.StartKey(startKey);
			if (endKey != null) view.EndKey(endKey);
			if (stale != null && stale.HasValue) view.Stale(stale.Value);
			return view;
		}

		public virtual IRemoveOperationResult Remove(string key)
		{
			return _Client.ExecuteRemove(key);
		}

		protected virtual IView<T> GetView(string name, bool isProjection = false)
		{
			return _Client.GetView<T>(_DesignDocName, name, !isProjection);
		}		

		protected virtual ISpatialView<T> GetSpatialView(string name, bool isProjection = false)
		{
			return _Client.GetSpatialView<T>(_DesignDocName, name, !isProjection);							
		}

		protected virtual IView<IViewRow> GetViewRaw(string name)
		{
			return _Client.GetView(_DesignDocName, name);
		}

		protected virtual ISpatialView<ISpatialViewRow> GetSpatialViewRaw(string name)
		{
			return _Client.GetSpatialView(_DesignDocName, name);
		}

		protected virtual string BuildKey(T model)
		{
			if (string.IsNullOrEmpty(model.Id))
			{
				model.Id = Guid.NewGuid().ToString();
			}

			return string.Concat(model.Type, "_", model.Id.Replace(" ", "_"));
		}

		private CouchbaseClient getCouchbaseClient()
		{
			var ctx = HttpContext.Current.ApplicationInstance.Context;

			if (!ctx.Items.Contains(ITEMS_CLIENT_KEY))
			{
				ctx.Items[ITEMS_CLIENT_KEY] = new CouchbaseClient();
			}

			return ctx.Items[ITEMS_CLIENT_KEY] as CouchbaseClient;
		}

		private string serializeAndIgnoreId(T obj)
		{
			var json = JsonConvert.SerializeObject(obj,
				new JsonSerializerSettings()
				{
					ContractResolver = new DocumentIdContractResolver(),
				});

			return json;
		}

		private class DocumentIdContractResolver : CamelCasePropertyNamesContractResolver
		{
			protected override List<MemberInfo>
			GetSerializableMembers(Type objectType)
			{
				return base.GetSerializableMembers(objectType).Where(o => o.Name != "Id").ToList();
			}
		}
	}
}