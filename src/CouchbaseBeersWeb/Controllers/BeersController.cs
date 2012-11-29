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
        // GET: /Beers/

        public ActionResult Index(string startKey)
        {
			int pageSize = 25;
			var beers = BeerRepository.GetAllByName(startKey, null, pageSize+1);
			
			ViewBag.NextKey = beers.Count() > pageSize ? beers.ElementAt(pageSize).Name : 
														 beers.ElementAt(beers.Count()).Name;
            return View(beers.Take(pageSize));
        }

        //
        // GET: /Beers/Details/5

        public ActionResult Details(string id)
        {
			var beer = BeerRepository.Get(id).Value;
            return View(beer);
        }

        //
        // GET: /Beers/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Beers/Create

        [HttpPost]
        public ActionResult Create(Beer beer)
        {
            
			BeerRepository.Create(beer);
            return RedirectToAction("Index");           
        }

        //
        // GET: /Beers/Edit/5

        public ActionResult Edit(string id)
        {
			var beer = BeerRepository.Get(id).Value;
            return View(beer);
        }

        //
        // POST: /Beers/Edit/5

        [HttpPost]
        public ActionResult Edit(string id, Beer beer)
        {
			BeerRepository.Update(beer);
			return RedirectToAction("Index");            
        }

        //
        // GET: /Beers/Delete/5

        public ActionResult Delete(string id)
        {
			var beer = BeerRepository.Get(id).Value;
            return View(beer);
        }

        //
        // POST: /Beers/Delete/5

        [HttpPost]
        public ActionResult Delete(string id, Beer beer)
        {
			BeerRepository.Remove(id);
			return RedirectToAction("Index");
        }
    }
}
