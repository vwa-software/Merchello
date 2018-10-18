namespace Merchello.FastTrack.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;

	using Merchello.Core;
	using Merchello.Core.Logging;
	using Merchello.Web;
	using Merchello.Web.Controllers;
	using Merchello.Web.Factories;
	using Merchello.Web.Models.ContentEditing;
	using Merchello.Web.Store.Factories;
	using Merchello.Web.Store.Models;
	using Merchello.Web.Store.Models.Async;
	using Newtonsoft.Json;
	using Umbraco.Web;
	using Umbraco.Web.Mvc;

	/*
		TODO: dit werkt nog niet doordat  in de merchello.ui.js een endpoint wordt gebruikt

		// post the form to update the basket quantities
        var url = MUI.Settings.Endpoints.basketSurface + 'UpdateBasket';

		Deze wijst naar de "Merchello" StoreBasketController.

	 */


	/// <summary>
	/// The default (generic) basket controller.
	/// </summary>
	[PluginController("FastTrack")]
	public class StoreBasketController : Merchello.Web.Store.Controllers.StoreBasketController
	{
		
		/// <summary>
		/// The factory responsible for building <see cref="ExtendedDataCollection"/>s when adding items to the basket.
		/// </summary>
		private readonly BasketItemExtendedDataFactory<StoreAddItemModel> _addItemExtendedDataFactory;

		private readonly MerchelloHelper merchello = null;


		/// <summary>
		/// Initializes a new instance of the <see cref="StoreBasketController"/> class.
		/// </summary>
		/// <remarks>
		/// This constructor allows you to inject your custom model factory overrides so that you can
		/// extended the various model interfaces with site specific models.  In this case, we have overridden 
		/// the BasketModelFactory and the AddItemModelFactory.  The BasketItemExtendedDataFactory has not been overridden.
		/// 
		/// Views rendered by this controller are placed in "/Views/QuickMartBasket/" and correspond to the method name.  
		/// 
		/// e.g.  the "AddToBasketForm" corresponds the the AddToBasketForm method in BasketControllerBase. 
		/// 
		/// This is just an generic MVC pattern and nothing to do with Umbraco
		/// </remarks>
		public StoreBasketController()
			: this(new BasketItemExtendedDataFactory<StoreAddItemModel>())
		{
			merchello = new MerchelloHelper(false);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StoreBasketController"/> class.
		/// </summary>
		/// <param name="addItemExtendedDataFactory">
		/// The <see cref="BasketItemExtendedDataFactory{StoreAddItemModel}"/>.
		/// </param>
		public StoreBasketController(BasketItemExtendedDataFactory<StoreAddItemModel> addItemExtendedDataFactory)
			: base(addItemExtendedDataFactory)
		{
			this._addItemExtendedDataFactory = addItemExtendedDataFactory;
			merchello = new MerchelloHelper(false);
		}


		/// <summary>
		/// Responsible for adding a product to the basket.
		/// </summary>
		/// <param name="model">
		/// The model.
		/// </param>
		/// <returns>
		/// The <see cref="ActionResult"/>.
		/// </returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public override ActionResult AddBasketItem(StoreAddItemModel model)
		{
			// Instantiating the ExtendedDataCollection in this manner allows for additional values 
			// to be added in the factory OnCreate override.
			// e.g. if you need to store custom extended data values, create your own factory
			// inheriting from BasketItemExtendedDataFactory and override the "OnCreate" method to store
			// any addition values you have added to the model
			var extendedData = this._addItemExtendedDataFactory.Create(model);

			// We've added some data modifiers that can handle such things as including taxes in product
			// pricing.  The data modifiers can either get executed when the item is added to the basket or
			// as a result from a MerchelloHelper query - you just don't want them to execute twice.


			try
			{
				var product = merchello.Query.Product.GetByKey(model.ProductKey);

				Web.Models.VirtualContent.IProductContent merchProduct = null;

				// Get merchello product from product key, stored in extendeddata.
				merchProduct = merchello.TypedProductContent(model.ProductKey);

				int packingUnit = 1;

				if (merchProduct != null)
				{
					// todo. future, nice to have, make this a Merchello property.
					packingUnit = Math.Max(1, merchProduct.GetPropertyValue<int>("ftProduct_PackingUnit"));
				}

				extendedData.SetValue("etcPackingUnit", packingUnit.ToString());

				// ensure the quantity on the model
				var quantity = Math.Max(packingUnit, model.Quantity);

				if (packingUnit > 1 && quantity % packingUnit != 0)
				{
					quantity = (int)(Math.Ceiling((decimal)(quantity / packingUnit)) * packingUnit);
				}

				// In the event the product has options we want to add the "variant" to the basket.
				// -- If a product that has variants is defined, the FIRST variant will be added to the cart. 
				// -- This was done so that we did not have to throw an error since the Master variant is no
				// -- longer valid for sale.
				if (model.OptionChoices != null && model.OptionChoices.Any())
				{
					var variant = product.GetProductVariantDisplayWithAttributes(model.OptionChoices);

					// Log the option choice for this variant in the extend data collection
					var choiceExplainations = new Dictionary<string, string>();
					foreach (var choice in variant.Attributes)
					{
						var option = product.ProductOptions.FirstOrDefault(x => x.Key == choice.OptionKey);
						if (option != null)
						{
							choiceExplainations.Add(option.Name, choice.Name);
						}
					}

					// store the choice explainations in the extended data collection
					extendedData.SetValue(Core.Constants.ExtendedDataKeys.BasketItemCustomerChoice, JsonConvert.SerializeObject(choiceExplainations));

					this.Basket.AddItem(variant, variant.Name, quantity, extendedData);
				}
				else
				{
					this.Basket.AddItem(product, product.Name, quantity, extendedData);
				}

				this.Basket.Save();

				// If this request is not an AJAX request return the redirect
				return this.HandleAddItemSuccess(model);
			}
			catch (Exception ex)
			{
				MultiLogHelper.Error<StoreBasketController>("Failed to add item to basket", ex);
				return this.HandleAddItemException(model, ex);
			}



		}


		/// <summary>
		/// Responsible for updating the quantities of items in the basket
		/// </summary>
		/// <param name="model">The <see cref="IBasketModel{TBasketItemModel}"/></param>
		/// <returns>Redirects to the current Umbraco page (generally the basket page)</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public override ActionResult UpdateBasket(StoreBasketModel model)
		{
			if (!this.ModelState.IsValid) return this.CurrentUmbracoPage();

			// The only thing that can be updated in this basket is the quantity
			foreach (var item in model.Items.Where(item => this.Basket.Items.First(x => x.Key == item.Key).Quantity != item.Quantity))
			{
				var basketitem = this.Basket.Items.First(x => x.Key == item.Key);
				if (basketitem.Quantity != item.Quantity)
				{
					// Get merchello product from product key, stored in extendeddata.
					var merchProduct = merchello.TypedProductContent(basketitem.ExtendedData.GetProductKey());

					int packingUnit = 1;

					if (merchProduct != null)
					{
						// todo. future, nice to have, make this a Merchello property.
						packingUnit = Math.Max(1, merchProduct.GetPropertyValue<int>("ftProduct_PackingUnit"));
					}

					if (packingUnit > 1 && item.Quantity > 0 && item.Quantity % packingUnit != 0)
					{
						item.Quantity = (int)(Math.Ceiling((decimal)((decimal)item.Quantity / packingUnit)) * packingUnit);
					}

					this.Basket.UpdateQuantity(item.Key, item.Quantity);

				}


			}

			this.Basket.Save();

			return this.HandleUpdateBasketSuccess(model);
		}




		

		///// <summary>
		///// Renders the add to basket form.
		///// </summary>
		///// <param name="model">
		///// The model.
		///// </param>
		///// <param name="view">
		///// The name of the view to render.
		///// </param>
		///// <returns>
		///// The <see cref="ActionResult"/>.
		///// </returns>
		//[ChildActionOnly]
		//public override ActionResult AddToBasketForm(StoreAddItemModel model, string view = "")
		//{

		//	Web.Models.VirtualContent.IProductContent merchProduct = null;

		//	// Get merchello product from product key, stored in extendeddata.
		//	merchProduct = merchello.TypedProductContent(model.ProductKey);

		//	int packingUnit = 1;

		//	if (merchProduct != null)
		//	{
		//		// todo. future, nice to have, make this a Merchello property.
		//		packingUnit = merchProduct.GetPropertyValue<int>("ftProduct_PackingUnit");
		//	}

		//	model.Quantity = packingUnit;

		//	return view.IsNullOrWhiteSpace() ? this.PartialView(model) : this.PartialView(view, model);
		//}

	}
}