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
		private readonly string _typeNameLower;

		public WebRepositoryBase()
		{
			_pluralizationService = PluralizationService.CreateService(CultureInfo.CurrentCulture);
			_typeNameLower = typeof(T).Name.ToLower();
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
			var key = string.IsNullOrEmpty(value.Id) ? BuildKey(value) : value.Id ;
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
			var designDocName = _pluralizationService.Pluralize(_typeNameLower);
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

		protected virtual IView<T> GetView(string name)
		{
			var designDocName = _pluralizationService.Pluralize(_typeNameLower);
			return _Client.GetView<T>(designDocName, name, true);
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