using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using CouchbaseModelViews.Framework;

namespace CouchbaseBeersWeb
{
	public class ModelViewsConfig
	{
		public static void RegisterAssemblies(IEnumerable<Assembly> assemblies)
		{
			var builder = new ViewBuilder();
			builder.AddAssemblies(assemblies.ToList());
			var designDocs = builder.Build();
			var ddManager = new DesignDocManager();
			ddManager.Create(designDocs);			
		}
	}
}