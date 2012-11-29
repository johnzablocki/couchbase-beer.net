using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CouchbaseBeersWeb.Models;

namespace CouchbaseBeersWeb.Controllers
{
    public class CountriesController : Controller
    {
		public BreweryRepository BreweryRepository { get; set; }

		public CountriesController()
		{
			BreweryRepository = new BreweryRepository();
		}

        //
        // GET: /Countries/

        public ActionResult Index()
        {
			var countries = BreweryRepository.GetCountsByCountry();
            return View(countries);
        }

		public ActionResult Details(string country)
		{
			var breweriesByCountry = BreweryRepository.GetByCountry(country);
			return View(breweriesByCountry);
		}

    }
}
