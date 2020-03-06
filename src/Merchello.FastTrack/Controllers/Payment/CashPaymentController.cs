namespace Merchello.FastTrack.Controllers.Payment
{
	using System.Linq;
	using System.Web.Mvc;
    using Merchello.FastTrack.Factories;
	using Merchello.FastTrack.Models;
	using Merchello.FastTrack.Models.Payment;
	using Merchello.Web.Factories;
	using Merchello.Web.Store.Controllers.Payment;
    using Merchello.Core.Models;
    using Merchello.Core;

    using Umbraco.Core;
    using Umbraco.Web.Mvc;
    /// <summary>
    /// The FastTrack cash payment controller.
    /// </summary>
    [PluginController("FastTrack")]
    public class CashPaymentController : CashPaymentControllerBase<FastTrackPaymentModel>
    {
        public override ActionResult Process(FastTrackPaymentModel model)
		{
			// Save Remarks in the address2 line.
			CheckoutManager.Shipping.ClearShipmentRateQuotes();

            IAddress model2 = null;
            var ShippingFactory = new FastTrackShippingAddressModelFactory();

            var index = model.ShippingAddressIndex; //index of the chosen address
      
            var allAddresses = ((ICustomer)this.CurrentCustomer).Addresses.Where(a => a.AddressType == Core.AddressType.Shipping).OrderBy(a => a.IsDefault ? 0 : 1);

            var choosenAddress = allAddresses.ToArray()[model.ShippingAddressIndex -1];

            if (choosenAddress != null) model2 = ShippingFactory.Create((ICustomer)CurrentCustomer, choosenAddress);
                       
			if (choosenAddress != null)
			{
                var address2 = ShippingFactory.Create(model2);

                address2.AddressType = Core.AddressType.Shipping;

                CheckoutManager.Customer.SaveShipToAddress(address2);
				CheckoutManager.Shipping.ClearShipmentRateQuotes();

				if (!string.IsNullOrEmpty(model.Remarks2))
				{
					CheckoutManager.Extended.AddNote(model.Remarks2);
				}

				var factory = new CheckoutShipRateQuoteModelFactory<FastTrackShipRateQuoteModel>();
				var quoteModel = factory.Create(Basket, address2, false);

				if (quoteModel != null && quoteModel.ProviderQuotes.Count() > 0)
				{
					var accepted = quoteModel.ProviderQuotes.FirstOrDefault();
					CheckoutManager.Shipping.SaveShipmentRateQuote(accepted);
				}
				else {
					Logger.Warn(this.GetType(), "Shipping provider not found for invoice");
				}
			}
			else {
				Logger.Warn(this.GetType(), "Address not found for invoice");
			}

			return base.Process(model);
		}

		/// <summary>
		/// Handles the redirection for the receipt.
		/// </summary>
		/// <param name="model">
		/// The <see cref="FastTrackPaymentModel"/>.
		/// </param>
		/// <returns>
		/// The <see cref="ActionResult"/>.
		/// </returns>
		protected override ActionResult HandlePaymentSuccess(FastTrackPaymentModel model)
        {
            // Set the invoice key in the customer context (cookie)
            if (model.ViewData.Success)
            {
                CustomerContext.SetValue("invoiceKey", model.ViewData.InvoiceKey.ToString());
            }

							
			return model.ViewData.Success && !model.SuccessRedirectUrl.IsNullOrWhiteSpace() ?
                Redirect(model.SuccessRedirectUrl) :
                base.HandlePaymentSuccess(model);
        }
    }
}