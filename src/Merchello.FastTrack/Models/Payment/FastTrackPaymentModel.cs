namespace Merchello.FastTrack.Models.Payment
{
    using Merchello.Web.Store.Models;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The cash payment model for the FastTrack store.
    /// </summary>
    public class FastTrackPaymentModel : StorePaymentModel, ISuccessRedirectUrl
    {
        /// <summary>
        /// Gets or sets the success redirect url.
        /// </summary>
        public string SuccessRedirectUrl { get; set; }

        /// <summary>
        /// Remarks of the order
        /// </summary>
        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

        /// <summary>
        /// Remarks of the order
        /// </summary>
        [DataType(DataType.MultilineText)]
        public string Remarks2 { get; set; }

        /// <summary>
		/// the option chosen
		/// </summary>
        public int ShippingAddressIndex { get; set; }
    }
}