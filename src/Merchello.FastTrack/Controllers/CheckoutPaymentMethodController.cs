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
    using Merchello.Web.Store.Models;

    using Umbraco.Core;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// A controller responsible for handling FastTrack payment method selection operations.
    /// </summary>
    [PluginController("FastTrack")]
    public class CheckoutPaymentMethodController : CheckoutPaymentMethodControllerBase<FastTrackPaymentMethodModel>
    {

		

		/// <summary>
		/// Custome hk one - page view, set the first (cash) payment provider
		/// </summary>
		/// <param name="view">
		/// The optional view.
		/// </param>
		/// <returns>
		/// The <see cref="ActionResult"/>.
		/// </returns>
		[ChildActionOnly]
		public override ActionResult ResolvePayment(string view = "")
		{
			var viewData = new StoreViewData();

			var gatewayMethod = GatewayContext.Payment.GetPaymentGatewayMethods().FirstOrDefault();
			if (gatewayMethod == null)
			{
				var nullRef = new NullReferenceException("Payment method was not found");
				throw nullRef;
			}

			CheckoutManager.Payment.SavePaymentMethod(gatewayMethod.PaymentMethod);

			// todo SaveBillingAddress van checkoutaddresscontroller beter implementeren

			// if (!this.ModelState.IsValid) return this.CurrentUmbracoPage();

			// billing address
			FastTrackBillingAddressModel model = null;
			var BillingAddressFactory = new FastTrackBillingAddressModelFactory();

			// Determine if we already have an address saved in the checkout manager
			var address = CheckoutManager.Customer.GetBillToAddress();
			if (address != null && !string.IsNullOrEmpty(address.CountryCode))
			{
				model = BillingAddressFactory.Create(address);
			}
			else
			{
				// If not and the we have the configuration set to use the customer's default customer billing address
				// This can only be done if the customer is logged in.  e.g. Not an anonymous customer
				if (!this.CurrentCustomer.IsAnonymous)
				{
					var defaultBilling = ((ICustomer)this.CurrentCustomer).DefaultCustomerAddress(AddressType.Billing);
					if (defaultBilling != null) model = BillingAddressFactory.Create((ICustomer)CurrentCustomer, defaultBilling);
				}
			}

			// If the model is still null at this point, we need to generate a default model
			// for the country drop down list
			if (model == null) {
				viewData.Messages = new string[] { "No adress found" };
				ViewData["MerchelloViewData"] = ViewData;
			}
			else
			{
				model.UseForShipping = true;
				new Merchello.FastTrack.Controllers.CheckoutAddressController().SaveBillingAddress(model);
			}


			// Get the previously saved payment method
			// Merchello's PaymentMethod should have been called PaymentMethodSettings but legacy from early version
			var paymentMethod = this.CheckoutManager.Payment.GetPaymentMethod();

			if (paymentMethod == null)
			{
				return this.InvalidCheckoutStagePartial();
			}

			var att = GetGatewayMethodUiAttribute(paymentMethod);

			return view.IsNullOrWhiteSpace() ? PartialView(att) : PartialView(view, att);
		}

		///// <summary>
		///// Overrides the successful set payment operation.
		///// </summary>
		///// <param name="model">
		///// The model.
		///// </param>
		///// <returns>
		///// The <see cref="ActionResult"/>.
		///// </returns>
		//protected override ActionResult HandleSetPaymentMethodSuccess(FastTrackPaymentMethodModel model)
  //      {
  //          return !model.SuccessRedirectUrl.IsNullOrWhiteSpace() ?
  //              Redirect(model.SuccessRedirectUrl) :
  //              base.HandleSetPaymentMethodSuccess(model);
  //      }

    }
}