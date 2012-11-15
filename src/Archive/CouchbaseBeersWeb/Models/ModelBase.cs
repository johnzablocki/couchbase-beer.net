using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace CouchbaseBeersWeb.Models
{
	public abstract class ModelBase
	{
		public string Id { get; set; }

		public abstract string Type { get; }
	}
}