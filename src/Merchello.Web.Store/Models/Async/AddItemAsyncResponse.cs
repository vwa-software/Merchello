namespace Merchello.Web.Store.Models.Async
{
	using System;
	using Merchello.Web.Models.Ui.Async;

	/// <summary>
	/// A response object to for an AJAX AddItem operation.
	/// </summary>
	public class AddItemAsyncResponse : AsyncResponse, IEmitsBasketItemCount
    {
        /// <summary>
        /// Gets or sets the basket item count.
        /// </summary>
        public int ItemCount { get; set; }
		public string Name { get;  set; }
		public string Id { get;  set; }
		public decimal Price { get;  set; }
		public string Category { get;  set; }
		public int Quantity { get;  set; }
	}
}