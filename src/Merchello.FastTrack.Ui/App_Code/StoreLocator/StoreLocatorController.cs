using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace VWA.Storelocator
{

	public class StoreLocatorController : Umbraco.Web.Mvc.SurfaceController
	{
		// GET: StoreLocator
		public ActionResult Index()
		{
			return View();
		}


		[HttpGet()]
		public JsonResult GetStoresAsJson(int id, string lat, string lng, string q)
		{
			IPublishedContent page = Umbraco.TypedContent(id);
			bool searchResultIsEmpty = false;

			IEnumerable<Store> dealers = (IEnumerable<Store>)this.ApplicationContext.ApplicationCache.RequestCache
			.GetCacheItem("vwa.stores", () =>
			{
				return GetStores(page, lat, lng,q,  ref searchResultIsEmpty);
			});

			return Json(dealers, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetStoresAsHtml(int id, string lat, string lng, string q)
		{
			IPublishedContent page = Umbraco.TypedContent(id);
			bool searchResultIsEmpty = false;
			
			IEnumerable<Store> stores = (IEnumerable<Store>)this.ApplicationContext.ApplicationCache.RequestCache
			.GetCacheItem("vwa.stores", () =>
			{
				return GetStores(page, lat, lng, q,  ref searchResultIsEmpty);
			});
			
			StoreLocatorResult model = new StoreLocatorResult() { Stores = stores };
			model.SearchResultIsEmpty = searchResultIsEmpty;

			return View(model);
		}

		private IEnumerable<Store> GetStores(IPublishedContent Model, string lat, string lng, string search,  ref bool searchResultIsEmpty)
		{

		
			bool isWebStore = Request["iswebstore"] == "1";
			bool isStore = Request["isstore"] == "1";

			// Beschikbare stores
			var stores = Model.Children("Dealer")
					.Where(x => x.IsVisible());

			if (isWebStore || isStore)
			{
				stores = stores.Where(a => a.GetPropertyValue<bool>("IsWebStore") == isWebStore || a.GetPropertyValue<bool>("IsStore") == isStore);
			}

			//if (!string.IsNullOrEmpty(search))
			//{
			//	// search opdracht in dropdown?
			//	search = search.ToLower();

			//	string[] searchableproperties = { "titleMain", "Adres", "Plaats", "Land" };


			//	stores = stores.Where(x =>
			//	{
			//		var found = false;
			//		foreach (string prop in searchableproperties)
			//		{
			//			if (x.GetPropertyValue<string>(prop).ToLower().Contains(search))
			//			{
			//				found = true;
			//				break;
			//			}
			//		}
			//		return found;
			//	});

			//	if (stores.Count() == 0)
			//	{
			//		// geen resultaten, alles tonen.
			//		searchResultIsEmpty = true;

			//		stores = Model.Children("Dealer")
			//			   .Where(x => x.IsVisible())
			//				   .OrderBy(x => x.Name);

			//	}
			//}


			double latitude = 0, longitude = 0;

			bool calculateDistance = false;

			if (!string.IsNullOrEmpty(lat) && !string.IsNullOrEmpty(lng))
			{

				if (double.TryParse(lat, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out latitude) && double.TryParse(lng, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out longitude))
				{
					calculateDistance = true;
				}
			}

			var storeModels = stores.Select(a => new Store(a)).ToList();

			if (calculateDistance)
			{
				foreach (var store in storeModels)
				{
					if (store.Latitude != 0 && store.Longitude != 0)
					{
						store.CalculatedDistance = GetDistance(latitude, longitude, (double)store.Latitude, (double)store.Longitude);
					}
					else
					{
						store.CalculatedDistance = double.MaxValue;
					}
				}
			}
			
			storeModels = storeModels.OrderBy(a => a.CalculatedDistance).Take(20).ToList();
			
			return storeModels;
		}


		public double GetDistance(double latitudeFrom, double longitudeFrom, double latitudeTo, double longitudeTo)
		{
			var d1 = latitudeFrom * (Math.PI / 180.0);
			var num1 = longitudeFrom * (Math.PI / 180.0);
			var d2 = latitudeTo * (Math.PI / 180.0);
			var num2 = longitudeTo * (Math.PI / 180.0) - num1;
			var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

			return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
		}

	}

	public class StoreLocatorResult
	{
		public IEnumerable<Store> Stores { get; set; }
		public bool SearchResultIsEmpty { get; internal set; }
	}

	
	public class Store
	{


		private readonly IPublishedContent _store;
		public Store(IPublishedContent storeAsContent)
		{
			_store = storeAsContent;
		}

		[ScriptIgnore]
		[JsonIgnore]
		public IPublishedContent Content { get { return _store; } }

		public double CalculatedDistance { get; set; }

		public string Name
		{
			get
			{
				return _store.GetPropertyValue<string>("titleMain");
			}
		}

		public decimal Latitude
		{
			get
			{
				return _store.GetPropertyValue<decimal>("latitude");
			}
		}

		public decimal Longitude
		{
			get
			{
				return _store.GetPropertyValue<decimal>("longitude");
			}
		}


		public string Image
		{
			get
			{
				return _store.GetPropertyValue<IPublishedContent>("FotoVestiging") == null ? "" : _store.GetPropertyValue<IPublishedContent>("FotoVestiging").Url;
			}
		}


		public string Adres
		{
			get
			{
				return _store.GetPropertyValue<string>("Adres");
			}
		}
		public string Postcode
		{
			get
			{
				return _store.GetPropertyValue<string>("Postcode");
			}
		}
		public string Tel
		{
			get
			{
				return _store.GetPropertyValue<string>("TelefoonNummer");
			}
		}

		public string Land
		{
			get
			{
				return _store.GetPropertyValue<string>("Land");
			}
		}
		public string Plaats
		{
			get
			{
				return _store.GetPropertyValue<string>("Plaats");
			}
		}
		public string Website
		{
			get
			{
				return _store.GetPropertyValue<string>("Website");
			}
		}
		public string WebsiteNaam
		{
			get
			{
				return System.Text.RegularExpressions.Regex.Replace(Website, @"https?://(www\.)?", "");
			}
		}

		public string Url
		{
			get
			{
				return _store.Url;
			}
		}
	}

}