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

			var gatewayMethod = GatewayContext.Payment.GetPaymentGatewayMethods().FirstOrDefault();
			if (gatewayMethod == null)
			{
				var nullRef = new NullReferenceException("Payment method was not found");
				throw nullRef;
			}

			CheckoutManager.Payment.SavePaymentMethod(gatewayMethod.PaymentMethod);
			
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