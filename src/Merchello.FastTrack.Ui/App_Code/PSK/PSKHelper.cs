using Umbraco.Core.Models;
using Umbraco.Web;

namespace VWA
{
	public class PSKHelper
	{

		private const decimal ORDERMINIMUM = 750;

		/// <summary>
		/// Method checks whether the member is a PSK customer and the orderminimum is greater then 750
		/// </summary>
		/// <param name="orderTotal"></param>
		/// <param name="Member"></param>
		/// <returns></returns>
		public static bool CheckPSKOrderMinimum(decimal orderTotal, IPublishedContent Member)
		{

			if (Member.HasProperty("IsPsk") && Member.GetPropertyValue<bool>("IsPSK"))
			{
				return orderTotal >= ORDERMINIMUM;
			}

			return true;
		}

	}
}