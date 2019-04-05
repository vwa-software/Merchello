namespace Merchello.FastTrack.Controllers
{
	using System;
	using System.Linq;
	using System.Web.Mvc;
	using Merchello.Core;
	using Merchello.Core.Models;
	using Merchello.FastTrack.Factories;
    using Merchello.FastTrack.Models;
    using Merchello.Web.Controllers;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Store.Models;

    using Umbraco.Core;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The default checkout summary controller.
    /// </summary>
    [PluginController("FastTrack")]
    public class CheckoutSummaryController : CheckoutSummaryControllerBase<FastTrackCheckoutSummaryModel, FastTrackBillingAddressModel, StoreAddressModel, StoreLineItemModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutSummaryController"/> class.
        /// </summary>
        public CheckoutSummaryController()
            : base(
                  new FastTrackCheckoutSummaryModelFactory(),
                  new CheckoutContextSettingsFactory())
        {
        }

		public override ActionResult InvoiceSummary(string view = "")
		{
			var viewData = new StoreViewData();
				
			// billing address
			FastTrackBillingAddressModel model = null;
			var BillingAddressFactory = new FastTrackBillingAddressModelFactory();


			// HKLiving, always use the default customer addres which comes from eTC.
			if (!this.CurrentCustomer.IsAnonymous)
			{
				ICustomerAddress defaultBilling = ((ICustomer)this.CurrentCustomer).DefaultCustomerAddress(AddressType.Billing);
				
				// check if default billing exists, fall back to first billing addres
				if (defaultBilling == null)
				{
					defaultBilling = ((ICustomer)this.CurrentCustomer).Addresses.FirstOrDefault(a => a.AddressType == AddressType.Billing);
				}

				// still no addres, fall back to first address
				if (defaultBilling == null)
				{
					defaultBilling = ((ICustomer)this.CurrentCustomer).Addresses.FirstOrDefault();
				}

				// Addres found, create the model.
				if (defaultBilling != null) model = BillingAddressFactory.Create((ICustomer)CurrentCustomer, defaultBilling);
			}

			if (model == null)
			{
				Merchello.Core.Models.IAddress address = CheckoutManager.Customer.GetBillToAddress();
				if (address != null && !string.IsNullOrEmpty(address.CountryCode))
				{
					model = BillingAddressFactory.Create(address);
				}
			}

			// If the model is still null at this point, there was an error in eTC
			if (model == null)
			{
				viewData.Messages = new string[] { "No adress found, please check your account settings." };
				ViewBag.MerchelloViewData = viewData;
				Logger.Warn(this.GetType(), "No adress found for user: " + ((ICustomer)this.CurrentCustomer).Email);
			}
			else
			{
				// save address to checkoutmanager		

				// Ensure billing address type is billing
				if (model.AddressType != AddressType.Billing) model.AddressType = AddressType.Billing;

				var address = BillingAddressFactory.Create(model);

				// Temporarily save the address in the checkout manager.
				this.CheckoutManager.Customer.SaveBillToAddress(address);
								
				model.WorkflowMarker = GetNextCheckoutWorkflowMarker(CheckoutStage.Payment);

				// we use the billing address factory here since we know the model FastTrackBillingAddressModel
				// and only want Merchello's IAddress
				var address2 = BillingAddressFactory.Create(model);

				address2.AddressType = AddressType.Shipping;
				CheckoutManager.Customer.SaveShipToAddress(address2);
				CheckoutManager.Shipping.ClearShipmentRateQuotes();

				var factory = new CheckoutShipRateQuoteModelFactory<FastTrackShipRateQuoteModel>();
				var quoteModel = factory.Create(Basket, address2);
				if (quoteModel != null && quoteModel.ProviderQuotes.Count() > 0)
				{
					var accepted = quoteModel.ProviderQuotes.FirstOrDefault();
					CheckoutManager.Shipping.SaveShipmentRateQuote(accepted);
				}
			}

			return base.InvoiceSummary(view);
		}
		/// <summary>
		/// Renders the Basket Summary.
		/// </summary>
		/// <param name="view">
		/// The optional view.
		/// </param>
		/// <returns>
		/// The <see cref="ActionResult"/>.
		/// </returns>
		[ChildActionOnly]
        public override ActionResult BasketSummary(string view = "")
        {
            var model = CheckoutSummaryFactory.Create(Basket, CheckoutManager);

            // EDIT ADDRESS BUTTON VISIBILITY
            // FastTrack implementation uses the notion of checkout stages in the UI
            // to determine what to display and the order in which to display them.  We can 
            // determine the stage by validating models at various points
            if (ValidateModel(model.ShippingAddress))
            {
                model.CheckoutStage = CheckoutStage.ShipRateQuote;
            }
            else if (ValidateModel(model.BillingAddress))
            {
                model.CheckoutStage = CheckoutStage.ShippingAddress;
            }
            
            return view.IsNullOrWhiteSpace() ? this.PartialView(model) : this.PartialView(view, model);
        }
    }
}