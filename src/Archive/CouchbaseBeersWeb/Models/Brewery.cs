using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CouchbaseBeersWeb.Models
{
	public class Brewery : ModelBase
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public string Counrty { get; set; }

		public string State { get; set; }
		
		public string City { get; set; }

		public string Address { get; set; }

		public string Phone { get; set; }

		public int MyProperty { get; set; }

		public override string Type
		{
			get { return "brewery"; }
		}
	}
}