namespace Merchello.FastTrack.Controllers.Payment
{
	using System.Linq;
	using System.Web.Mvc;
	using Merchello.FastTrack.Factories;
	using Merchello.FastTrack.Models;
	using Merchello.FastTrack.Models.Payment;
	using Merchello.Web.Factories;
	using Merchello.Web.Store.Controllers.Payment;

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

			var adress = CheckoutManager.Customer.GetShipToAddress();
			if (adress==null || string.IsNullOrEmpty(adress.CountryCode))
			{
				adress = CheckoutManager.Customer.GetBillToAddress();
			}

			if (adress != null)
			{
				adress.Address2 = model.Remarks;
				
				CheckoutManager.Customer.SaveShipToAddress(adress);
				CheckoutManager.Shipping.ClearShipmentRateQuotes();

				if (!string.IsNullOrEmpty(model.Remarks2))
				{
					CheckoutManager.Extended.AddNote(model.Remarks2);
				}

				var factory = new CheckoutShipRateQuoteModelFactory<FastTrackShipRateQuoteModel>();
				var quoteModel = factory.Create(Basket, adress, false);

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