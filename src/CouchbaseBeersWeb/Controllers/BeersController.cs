using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CouchbaseBeersWeb.Models;

namespace CouchbaseBeersWeb.Controllers
{
    public class BeersController : Controller
    {
		public BeerRepository BeerRepository { get; set; }

		public BeersController()
		{
			BeerRepository = new BeerRepository();
		}

        //
        // GET: /Beer/

        public ActionResult Index(string startKey, string endKey, int limit = 50)
        {
			var items = BeerRepository.GetAll(startKey, endKey, limit);
            return View(items);
        }

		public ActionResult Show(string id)
		{
			var item = BeerRepository.Get(id);
			return View(item.Value);
		}
		
		[HttpGet]
		public ActionResult Edit(string id)
		{
			var item = BeerRepository.Get(id);
			return View(item.Value);
		}

		[HttpPost]
		public ActionResult Edit(string id, Beer beer)
		{
			if (ModelState.IsValid)
			{
				var savedBeer = BeerRepository.Get(id);
				savedBeer.Value.Name = beer.Name;
				savedBeer.Value.Description = beer.Description;
				savedBeer.Value.ABV = beer.ABV;
				savedBeer.Value.IBU = beer.IBU;
				savedBeer.Value.SRM = beer.SRM;
								
				BeerRepository.Save(id, savedBeer.Value);
			}
			
			return RedirectToAction("Show", new { id = id });
		}
    }
}
