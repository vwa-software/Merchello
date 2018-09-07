namespace Merchello.FastTrack.Controllers
{
	using System;
	using System.Web.Mvc;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.FastTrack.Factories;
    using Merchello.FastTrack.Models;
    using Merchello.Web.Controllers;
    using Merchello.Web.Models.Ui;

    using Umbraco.Core;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The controller responsible for rendering and processing addresses in the default checkout process.
    /// </summary>
    [PluginController("FastTrack")]
    public class CheckoutAddressController : CheckoutAddressControllerBase<FastTrackBillingAddressModel, FastTrackCheckoutAddressModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutAddressController"/> class.
        /// </summary>
        public CheckoutAddressController()
            : base(
                  new FastTrackBillingAddressModelFactory(),
                  new FastTrackShippingAddressModelFactory())
        {
        }


		/// <summary>
		/// Saves the <see cref="ICheckoutAddressModel"/> for use in the checkout.
		/// </summary>
		/// <param name="model">
		/// The <see cref="ICheckoutAddressModel"/>.
		/// </param>
		/// <returns>
		/// Redirects or JSON response depending if called Async.
		/// </returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public override ActionResult SaveBillingAddress(FastTrackBillingAddressModel model)
		{
			//if (!this.ModelState.IsValid) return this.CurrentUmbracoPage();

			// Ensure billing address type is billing
			if (model.AddressType != AddressType.Billing) model.AddressType = AddressType.Billing;

			var address = BillingAddressFactory.Create(model);

			// Temporarily save the address in the checkout manager.
			this.CheckoutManager.Customer.SaveBillToAddress(address);

			if (!this.CurrentCustomer.IsAnonymous) this.SaveCustomerBillingAddress(model);

			model.WorkflowMarker = GetNextCheckoutWorkflowMarker(CheckoutStage.BillingAddress);


			// Handle HandleBillingAdressSaveSuccess

			// If the customer is logged in save the address to their default customer billing address for the next use
			if (!CurrentCustomer.IsAnonymous)
			{
				// In this implementation we are simply overwritting any previously saved addresses
				// This could can be extended to allow customers to manage multiple addresses of a given
				// type in other implementations.
				var customer = (ICustomer)CurrentCustomer;
				var existing = customer.DefaultCustomerAddress(AddressType.Billing);
				var caddress = BillingAddressFactory.Create(model, (ICustomer)CurrentCustomer, "Billing Address", AddressType.Billing);

				if (existing != null)
				{
					caddress.CreateDate = existing.CreateDate;
					caddress.Key = existing.Key;
				}

				((ICustomer)CurrentCustomer).SaveCustomerAddress(caddress);
			}

			// NOTE: We need to do a special check here to assert that the country code is valid as it
			// billing addresses by default can be associated any where in the world, whereas shiping
			// destinations are usually constrained to specific locations.  Some implementations may
			// opt to swap the order of the address collection to alleviate the need for this check, but
			// there are also cases, where items may not need to be shipped and the billing address is required
			// to create the invoice.
			if (model.UseForShipping && EnsureBillingAddressIsValidAsShippingAddress(model))
			{
				// we use the billing address factory here since we know the model FastTrackBillingAddressModel
				// and only want Merchello's IAddress
				var address2 = BillingAddressFactory.Create(model);

				CheckoutManager.Customer.SaveShipToAddress(address2);

				// In this implementation, we cannot save the customer shipping address to the customer as it may be a different model here
				// However, it is possible but more work would be required to ensure the model mapping

				// set the checkout stage
				model.WorkflowMarker = GetNextCheckoutWorkflowMarker(CheckoutStage.Payment);

			}

			return null;

		}



        /// <summary>
        /// Overrides the action for a successful shipping address save.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected override ActionResult HandleShippingAddressSaveSuccess(FastTrackCheckoutAddressModel model)
        {
            // If the customer is logged in save the address to their default customer shipping address for the next use
            if (!CurrentCustomer.IsAnonymous)
            {
                // In this implementation we are simply overwritting any previously saved addresses
                // This could can be extended to allow customers to manage multiple addresses of a given
                // type in other implementations.
                var customer = (ICustomer)CurrentCustomer;
                var existing = customer.DefaultCustomerAddress(AddressType.Shipping);
                var caddress = ShippingAddressFactory.Create(model, (ICustomer)CurrentCustomer, "Shipping Address", AddressType.Shipping);
                if (existing != null)
                {
                    caddress.CreateDate = existing.CreateDate;
                    caddress.Key = existing.Key;
                }

                ((ICustomer)CurrentCustomer).SaveCustomerAddress(caddress);
            }

			return null;


            //return !model.SuccessRedirectUrl.IsNullOrWhiteSpace() ?
            //    Redirect(model.SuccessRedirectUrl) :
            //    base.HandleShippingAddressSaveSuccess(model);
        }
    }
}